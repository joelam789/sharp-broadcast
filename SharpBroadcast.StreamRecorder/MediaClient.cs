using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

using WebSocket4Net;

namespace SharpBroadcast.StreamRecorder
{
    public class MediaClient
    {
        public static int DEFAULT_MAX_CACHE_SIZE = 1024 * 1024; // 1M
        public static int DEFAULT_MAX_RECORD_SIZE = 1024 * 1024 * 256; // 256M

        public static string DEFAULT_CONVERTER_CMD = "ffmpeg"; // must be ffmpeg, but the version could be different

        public static string DEFAULT_STREAM_DATA_FOLDER = "streams"; // to save raw stream data
        public static string DEFAULT_RECORD_FILE_FOLDER = "records"; // to save final output record files

        private TaskFactory m_ConvertTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(1));

        private MemoryStream m_VideoCache = new MemoryStream();
        private int m_VideoCacheSize = 0;

        private MemoryStream m_AudioCache = new MemoryStream();
        private int m_AudioCacheSize = 0;

        private string m_ConverterCmd = "ffmpeg";
        private string m_CallbackUrl = "";

        private string m_StreamDataFolder = "";
        private string m_RecordFileFolder = "";

        private string m_RawVideoFilePath = "";
        private string m_RawAudioFilePath = "";

        private string m_RawVideoType = "";
        private string m_RawAudioType = "";

        private int m_CurrentRecordSize = 0;

        private decimal m_VideoStartOffset = 0;
        private decimal m_AudioStartOffset = 0;

        private int m_MustCreateOutputFile = 0;

        public int MaxCacheSize { get; private set; }
        public int MaxRecordSize { get; private set; }

        public string ChannelName { get; private set; }
        public string ChannelURL { get; private set; }
        public WebSocket Socket { get; private set; }
        public ClientInfo Info { get; private set; }

        public bool IsReceiving { get; private set; }
        public bool IsRecording { get; private set; }
        public bool IsConverting { get; private set; }

        public string StreamDataFileName { get; private set; }
        public string RecordContentType { get; private set; }

        public MediaClient(string channelName, string url, RecordConfig config = null)
        {
            ChannelName = channelName;
            ChannelURL = url;

            IsReceiving = false;
            IsRecording = false;
            IsConverting = false;

            if (config != null) m_MustCreateOutputFile = config.MustCreateOutputFile;

            m_ConverterCmd = config != null && config.Converter.Length > 0 ? config.Converter : DEFAULT_CONVERTER_CMD;
            m_CallbackUrl = config != null && config.Callback.Length > 0 ? config.Callback : "?";

            string currentFolder = GetCurrentFolder();

            m_StreamDataFolder = config != null ? config.StreamDataFolder : DEFAULT_STREAM_DATA_FOLDER;
            if (m_StreamDataFolder.Length > 0)
            {
                m_StreamDataFolder = m_StreamDataFolder.Replace('\\', '/');
                if (m_StreamDataFolder[0] != '/' && m_StreamDataFolder.IndexOf(":/") != 1) // if it is not abs path
                {
                    if (currentFolder != null && currentFolder.Length > 0)
                        m_StreamDataFolder = currentFolder.Replace('\\', '/') + "/" + m_StreamDataFolder;
                }
            }

            m_RecordFileFolder = config != null ? config.RecordFileFolder : DEFAULT_RECORD_FILE_FOLDER;
            if (m_RecordFileFolder.Length > 0)
            {
                m_RecordFileFolder = m_RecordFileFolder.Replace('\\', '/');
                if (m_RecordFileFolder[0] != '/' && m_RecordFileFolder.IndexOf(":/") != 1) // if it is not abs path
                {
                    if (currentFolder != null && currentFolder.Length > 0)
                        m_RecordFileFolder = currentFolder.Replace('\\', '/') + "/" + m_RecordFileFolder;
                }
            }

            m_VideoStartOffset = config != null && config.VideoStartOffset > 0 ? config.VideoStartOffset : 0;
            m_AudioStartOffset = config != null && config.AudioStartOffset > 0 ? config.AudioStartOffset : 0;

            m_RawVideoType = "";
            m_RawAudioType = "";

            m_RawVideoFilePath = "";
            m_RawAudioFilePath = "";

            m_CurrentRecordSize = 0;

            MaxCacheSize = config != null && config.MaxCacheSize > 0 ? config.MaxCacheSize : DEFAULT_MAX_CACHE_SIZE;
            MaxRecordSize = config != null && config.MaxRecordSize > 0 ? config.MaxRecordSize : DEFAULT_MAX_RECORD_SIZE;

            StreamDataFileName = "";
            RecordContentType = config != null ? config.RecordContentType : "auto";

            Info = new ClientInfo()
            {
                ChannelName = channelName,
                ChannelURL = url,
                ErrorTimes = 0,
                Status = "closed",
                LastActiveTime = DateTime.Now
            };

            Socket = new WebSocket(url);
            Socket.AllowUnstrustedCertificate = true;
            Socket.Closed += new EventHandler(WhenClosed);
            Socket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WhenError);
            Socket.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(WhenDataReceived);
            Socket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(WhenMessageReceived);
            Socket.Opened += new EventHandler(WhenOpened);

        }

        public static string GetCurrentFolder()
        {
            string folder = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            if (folder == null || folder.Trim().Length <= 0)
            {
                var entry = Assembly.GetEntryAssembly();
                var location = "";
                try
                {
                    if (entry != null) location = entry.Location;
                }
                catch { }
                if (location != null && location.Length > 0)
                {
                    folder = Path.GetDirectoryName(location);
                }
            }
            return (folder != null && folder.Length > 0) ? folder : "";
        }

        public void Open()
        {
            if (Info.Status != "closed") return;

            Info.Status = "opening";
            Info.LastActiveTime = DateTime.Now;
            try
            {
                Socket.Open();
            }
            catch (Exception ex)
            {
                Info.ErrorTimes += 1;
                Info.Status = "closed";
                CommonLog.Error("Failed to open socket to receive stream data: " + ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                Socket.Close();
            }
            catch { }
            finally
            {
                Info.Status = "closed";
                Info.LastActiveTime = DateTime.Now;
            }
        }

        public void Record(string outputFileName = "", bool needOverwrite = true)
        {
            lock (StreamDataFileName)
            {
                if (Info.Status == "closed")
                {
                    CommonLog.Error("Failed to record stream: channel [" + ChannelName + "] is closed.");
                    return;
                }
                if (Info.MediaInfo.Length <= 0)
                {
                    CommonLog.Error("Failed to record stream: channel [" + ChannelName + "] has no media info.");
                    return;
                }

                if (IsRecording || StreamDataFileName.Length > 0)
                {
                    CommonLog.Info("Channel [" + ChannelName + "] has already started recording.");
                    return;
                }

                string outputFile = outputFileName;
                if (outputFile == null || outputFile.Length <= 0)
                {
                    outputFile = ChannelName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                m_RawVideoFilePath = "";
                m_RawAudioFilePath = "";

                if (m_RawVideoType.Length > 0) m_RawVideoFilePath = m_StreamDataFolder + "/" + outputFile + "." + m_RawVideoType;
                if (m_RawAudioType.Length > 0) m_RawAudioFilePath = m_StreamDataFolder + "/" + outputFile + "." + m_RawAudioType;

                if (RecordContentType == "video") m_RawAudioFilePath = "";
                if (RecordContentType == "audio") m_RawVideoFilePath = "";

                if (needOverwrite)
                {
                    try
                    {
                        if (m_RawVideoFilePath.Length > 0 && File.Exists(m_RawVideoFilePath)) File.Delete(m_RawVideoFilePath);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to delete old data file: " + m_RawVideoFilePath);
                        CommonLog.Error(ex.ToString());
                    }

                    try
                    {
                        if (m_RawAudioFilePath.Length > 0 && File.Exists(m_RawAudioFilePath)) File.Delete(m_RawAudioFilePath);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to delete old data file: " + m_RawAudioFilePath);
                        CommonLog.Error(ex.ToString());
                    }
                }

                try
                {
                    if (m_RawVideoFilePath.Length > 0 && !File.Exists(m_RawVideoFilePath)) using (var fs = File.Create(m_RawVideoFilePath)) { }
                }
                catch (Exception ex)
                {
                    string errmsg = "Failed to create/open data file: " + m_RawVideoFilePath;
                    m_RawVideoFilePath = "";
                    CommonLog.Error(errmsg);
                    CommonLog.Error(ex.ToString());
                }

                try
                {
                    if (m_RawAudioFilePath.Length > 0 && !File.Exists(m_RawAudioFilePath)) using (var fs = File.Create(m_RawAudioFilePath)) { }
                }
                catch (Exception ex)
                {
                    string errmsg = "Failed to create/open data file: " + m_RawAudioFilePath;
                    m_RawAudioFilePath = "";
                    CommonLog.Error(errmsg);
                    CommonLog.Error(ex.ToString());
                }

                if (m_RawVideoFilePath.Length > 0 && File.Exists(m_RawVideoFilePath))
                {
                    m_RawVideoFilePath = Path.GetFullPath(m_RawVideoFilePath);

                    m_VideoCacheSize = 0;
                    if (m_VideoCache != null) m_VideoCache.Dispose();
                    m_VideoCache = new MemoryStream();

                    CommonLog.Info("Start to record video: " + m_RawVideoFilePath);
                }
                else m_RawVideoFilePath = "";

                if (m_RawAudioFilePath.Length > 0 && File.Exists(m_RawAudioFilePath))
                {
                    m_RawAudioFilePath = Path.GetFullPath(m_RawAudioFilePath);

                    m_AudioCacheSize = 0;
                    if (m_AudioCache != null) m_AudioCache.Dispose();
                    m_AudioCache = new MemoryStream();

                    CommonLog.Info("Start to record audio: " + m_RawAudioFilePath);
                }
                else m_RawAudioFilePath = "";

                if (m_RawVideoFilePath.Length > 0 || m_RawAudioFilePath.Length > 0)
                {
                    IsRecording = true;
                    StreamDataFileName = outputFile;
                    m_CurrentRecordSize = 0;
                }
                else
                {
                    IsRecording = false;
                    StreamDataFileName = "";
                }
            }
        }

        public void Export(string specificTarget = "")
        {
            lock (StreamDataFileName)
            {
                if (Info.Status == "closed")
                {
                    CommonLog.Error("Failed to export(save) record: channel [" + ChannelName + "] is closed.");
                    return;
                }
                if (Info.MediaInfo.Length <= 0)
                {
                    CommonLog.Error("Failed to export(save) record: channel [" + ChannelName + "] has no media info.");
                    return;
                }

                if (!IsRecording || StreamDataFileName.Length <= 0)
                {
                    CommonLog.Error("Failed to export(save) record: channel [" + ChannelName + "] has not started recording.");
                    return;
                }

                if (specificTarget.Length > 0 && specificTarget != StreamDataFileName)
                {
                    CommonLog.Error("Failed to export(save) record: channel [" + ChannelName + "] has not specified the target file path.");
                    return;
                }

                StreamDataFileName = ""; // stop writing received stream data to cache

                string videoSourceFile = "?";
                string audioSourceFile = "?";

                if (m_RawVideoFilePath.Length > 0 && File.Exists(m_RawVideoFilePath))
                {
                    if (m_VideoCacheSize > 0)
                    {
                        try
                        {
                            using (var fs = new FileStream(m_RawVideoFilePath, FileMode.Append, FileAccess.Write))
                            {
                                var writer = new BinaryWriter(fs);
                                writer.Write(m_VideoCache.ToArray());
                                writer.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonLog.Error(ex.ToString());
                        }

                        m_VideoCacheSize = 0;
                        if (m_VideoCache != null) m_VideoCache.Dispose();
                        m_VideoCache = new MemoryStream();
                    }

                    videoSourceFile = String.Copy(m_RawVideoFilePath);
                    m_RawVideoFilePath = "";
                }

                if (m_RawAudioFilePath.Length > 0 && File.Exists(m_RawAudioFilePath))
                {
                    if (m_AudioCacheSize > 0)
                    {
                        try
                        {
                            using (var fs = new FileStream(m_RawAudioFilePath, FileMode.Append, FileAccess.Write))
                            {
                                var writer = new BinaryWriter(fs);
                                writer.Write(m_AudioCache.ToArray());
                                writer.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonLog.Error(ex.ToString());
                        }

                        m_AudioCacheSize = 0;
                        if (m_AudioCache != null) m_AudioCache.Dispose();
                        m_AudioCache = new MemoryStream();
                    }

                    audioSourceFile = String.Copy(m_RawAudioFilePath);
                    m_RawAudioFilePath = "";
                }

                if (videoSourceFile.Length > 1 || audioSourceFile.Length > 1)
                {
                    m_ConvertTaskFactory.StartNew((Action<object>)((arg) =>
                    {
                        CallConvertProcess(arg);
                    }),
                    m_ConverterCmd + "," + m_RecordFileFolder + "," + videoSourceFile + "," + audioSourceFile + ","
                    + m_VideoStartOffset + "," + m_AudioStartOffset + "," + ChannelName + "," + m_CallbackUrl);
                }

                IsRecording = false;
                m_CurrentRecordSize = 0;
            }
        }

        private void CallConvertProcess(object rawfile)
        {
            var inputParts = rawfile.ToString().Trim().Split(',');
            if (inputParts.Length < 4) return;

            string converterCmd = inputParts[0].Trim();

            string channelName = "";
            if (inputParts.Length > 6) channelName = inputParts[6].Trim();
            if (channelName.Length <= 0) channelName = "?";

            string callbackUrl = "";
            if (inputParts.Length > 7) callbackUrl = inputParts[7].Trim();
            if (callbackUrl.Length < 2) callbackUrl = "";

            string outputFolder = inputParts[1].Trim();
            string inputVideoFile = inputParts[2].Trim();
            string inputAudioFile = inputParts[3].Trim();
            string outputFileName = "";
            if (inputVideoFile.Length > 1) outputFileName = Path.GetFileNameWithoutExtension(inputVideoFile);
            else if (inputAudioFile.Length > 1) outputFileName = Path.GetFileNameWithoutExtension(inputAudioFile);
            if (outputFileName.Length <= 0) return;

            string outputTempFile = "";
            string outputFile = outputFolder + "/" + outputFileName + ".mp4";

            try
            {
                if (outputFile.Length > 0 && File.Exists(outputFile)) File.Delete(outputFile);
            }
            catch (Exception ex)
            {
                CommonLog.Error(ex.ToString());
            }

            CommonLog.Info("Going to convert file: [" + inputVideoFile + "] + [" + inputAudioFile + "] => [" + outputFile + "]");

            List<string> convertParamList = new List<string>();
            if (inputVideoFile.Length > 1 && inputAudioFile.Length > 1)
            {
                string videoStartOffset = "0";
                string audioStartOffset = "0";
                if (inputParts.Length > 4) videoStartOffset = inputParts[4].Trim();
                if (inputParts.Length > 5) audioStartOffset = inputParts[5].Trim();

                if (videoStartOffset.Length > 0 && videoStartOffset != "0") audioStartOffset = "0"; // should not enable both

                string videoStartOffsetOption = "";
                string audioStartOffsetOption = "";
                if (videoStartOffset != "0") videoStartOffsetOption = " -ss " + videoStartOffset;
                if (audioStartOffset != "0") audioStartOffsetOption = " -ss " + audioStartOffset;

                if (videoStartOffsetOption.Length <= 0 && audioStartOffsetOption.Length <= 0)
                {
                    string convertParamLine = "";
                    convertParamLine = "-y -i \"" + inputVideoFile + "\" ";
                    if (inputAudioFile.Contains(".pcm")) convertParamLine += "-f u8 -ac 1 ";
                    convertParamLine += "-i \"" + inputAudioFile + "\" ";
                    convertParamLine += "-c:v copy ";
                    if (!inputAudioFile.Contains(".aac")) convertParamLine += "-c:a aac ";
                    else convertParamLine += "-c:a copy -bsf:a aac_adtstoasc ";
                    convertParamLine += "-f mp4 \"" + outputFile + "\"";
                    convertParamList.Add(convertParamLine);
                }
                else
                {
                    outputTempFile = outputFolder + "/" + outputFileName + ".tmp.mp4";

                    string convertParamLine1 = "";
                    convertParamLine1 = "-y -i \"" + inputVideoFile + "\" ";
                    if (inputAudioFile.Contains(".pcm")) convertParamLine1 += "-f u8 -ac 1 ";
                    convertParamLine1 += "-i \"" + inputAudioFile + "\" ";
                    convertParamLine1 += "-c:v copy -c:a copy ";
                    if (!inputAudioFile.Contains(".aac")) convertParamLine1 += "-c:a aac ";
                    else convertParamLine1 += "-c:a copy -bsf:a aac_adtstoasc ";
                    convertParamLine1 += "-f mp4 \"" + outputTempFile + "\"";
                    convertParamList.Add(convertParamLine1);

                    string convertParamLine2 = "";
                    convertParamLine2 = "-y " + videoStartOffsetOption + " -i \"" + outputTempFile + "\" ";
                    convertParamLine2 += audioStartOffsetOption + " -i \"" + outputTempFile + "\" ";
                    convertParamLine2 += "-map 0:v -map 1:a -c:v copy -c:a copy ";
                    convertParamLine2 += "-f mp4 \"" + outputFile + "\"";
                    convertParamList.Add(convertParamLine2);
                }

            }
            else if (inputVideoFile.Length > 1)
            {
                string convertParamLine = "";
                convertParamLine = "-y -i \"" + inputVideoFile + "\" ";
                convertParamLine += "-c:v copy -an ";
                convertParamLine += "-f mp4 \"" + outputFile + "\"";
                convertParamList.Add(convertParamLine);
            }
            else if (inputAudioFile.Length > 1)
            {
                string convertParamLine = "";
                convertParamLine = "-y ";
                if (inputAudioFile.Contains(".pcm")) convertParamLine += "-f u8 -ac 1 ";
                convertParamLine += "-i \"" + inputAudioFile + "\" ";
                convertParamLine += "-c:a copy -vn ";
                if (!inputAudioFile.Contains(".aac")) convertParamLine += "-c:a aac ";
                else convertParamLine += "-c:a copy -bsf:a aac_adtstoasc ";
                convertParamLine += "-f mp4 \"" + outputFile + "\"";
                convertParamList.Add(convertParamLine);
            }

            try
            {
                foreach (var paramLine in convertParamList)
                {
                    //CommonLog.Info(paramLine);

                    ProcessStartInfo pinfo = new ProcessStartInfo(converterCmd, paramLine);

                    pinfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    pinfo.UseShellExecute = false;
                    pinfo.CreateNoWindow = true;

                    using (Process process = Process.Start(pinfo))
                    {
                        process.WaitForExit();
                    }
                }

                if (outputTempFile.Length > 0)
                {
                    try
                    {
                        if (File.Exists(outputTempFile)) File.Delete(outputTempFile);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to delete convertion temp file: " + outputTempFile);
                        CommonLog.Error(ex.Message);
                    }
                }

                if (outputFile.Length > 0 && File.Exists(outputFile))
                {
                    try
                    {
                        if (inputVideoFile.Length > 1 && File.Exists(inputVideoFile)) File.Delete(inputVideoFile);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to delete video stream data file: " + inputVideoFile);
                        CommonLog.Error(ex.Message);
                    }
                    try
                    {
                        if (inputAudioFile.Length > 1 && File.Exists(inputAudioFile)) File.Delete(inputAudioFile);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to delete video stream data file: " + inputAudioFile);
                        CommonLog.Error(ex.Message);
                    }
                    CommonLog.Info("Finished converting: " + outputFile);

                    if (callbackUrl.Length > 0)
                    {
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(callbackUrl);
                            httpWebRequest.ContentType = "text/plain";
                            httpWebRequest.Method = "POST";

                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(channelName + "|" + outputFileName.Trim());
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                            {
                                string responseResult = sr.ReadToEnd();
                                if (responseResult == null) responseResult = "";
                                else responseResult = responseResult.Trim();
                                CommonLog.Info("Got remote callback response: " + responseResult);
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonLog.Error("Failed to send request to inform remote server: " + ex.Message);
                        }
                    }
                }
                else
                {
                    CommonLog.Info("Failed to convert live stream to MP4!");
                }

            }
            catch (Exception ex)
            {
                CommonLog.Error("Failed to convert live stream to MP4: " + ex.ToString());
            }

            if (outputFile.Length > 0)
            {
                if (!File.Exists(outputFile) && m_MustCreateOutputFile > 0)
                {
                    try
                    {
                        using (var fs = File.Create(outputFile)) { }
                    }
                    catch (Exception ex)
                    {
                        string errmsg = "Failed to create final output file: " + outputFile;
                        CommonLog.Error(errmsg);
                        CommonLog.Error(ex.ToString());
                    }
                }
            }

        }

        private void WhenOpened(object sender, EventArgs e)
        {
            Info.Status = "opened";

            IsReceiving = false;
            IsRecording = false;

            m_RawVideoType = "";
            m_RawAudioType = "";

            m_RawVideoFilePath = "";
            m_RawAudioFilePath = "";

            StreamDataFileName = "";
            m_CurrentRecordSize = 0;
        }

        private void WhenError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Info.ErrorTimes += 1;
            CommonLog.Error("Socket Error: " + e.Exception.Message);
            if (Info.ErrorTimes <= 5)
            {
                try
                {
                    Close();
                }
                catch { }
            }
            else
            {
                Info.Status = "closed";
            }
        }

        private void WhenClosed(object sender, EventArgs e)
        {
            Info.Status = "closed";

            IsReceiving = false;
            IsRecording = false;

            m_RawVideoType = "";
            m_RawAudioType = "";

            m_RawVideoFilePath = "";
            m_RawAudioFilePath = "";

            StreamDataFileName = "";
            m_CurrentRecordSize = 0;
        }

        private void WhenDataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            //IsReceiving = true;
            //Info.LastActiveTime = DateTime.Now;
            //Info.LastDataTime = Info.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                if (Info.Status == "closed") return;
                if (e.Data.Length <= 0) return;
                else
                {
                    IsReceiving = true;
                    Info.LastActiveTime = DateTime.Now;
                    Info.LastDataTime = Info.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            catch
            {
                return;
            }

            string overloadFileName = "";

            lock (StreamDataFileName)
            {
                Info.Status = IsRecording ? "recording" : "receiving";

                if (!IsRecording || StreamDataFileName.Length <= 0) return;

                m_CurrentRecordSize += e.Data.Length;
                if (m_CurrentRecordSize > MaxRecordSize)
                {
                    overloadFileName = String.Copy(StreamDataFileName);
                    CommonLog.Info("Exceeded max size limit of " + MaxRecordSize + " - " + overloadFileName);
                }
                else
                {
                    if (e.Data.Length > 4
                    && (e.Data[0] == 0 && e.Data[1] == 0 && e.Data[2] == 0 && e.Data[3] == 1)
                    && m_RawVideoFilePath.Length > 0)
                    {
                        m_VideoCache.Write(e.Data, 0, e.Data.Length);
                        m_VideoCacheSize += e.Data.Length;

                        if (m_VideoCacheSize >= MaxCacheSize)
                        {
                            if (m_RawVideoFilePath.Length > 0 && File.Exists(m_RawVideoFilePath))
                            {
                                try
                                {
                                    using (var fs = new FileStream(m_RawVideoFilePath, FileMode.Append, FileAccess.Write))
                                    {
                                        var writer = new BinaryWriter(fs);
                                        writer.Write(m_VideoCache.ToArray());
                                        writer.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonLog.Error(ex.ToString());
                                }
                            }
                            else CommonLog.Error("Video stream data file not found: " + m_RawVideoFilePath);

                            m_VideoCacheSize = 0;
                            if (m_VideoCache != null) m_VideoCache.Dispose();
                            m_VideoCache = new MemoryStream();
                        }
                    }
                    else if (e.Data.Length > 0 && m_RawAudioFilePath.Length > 0)
                    {
                        m_AudioCache.Write(e.Data, 0, e.Data.Length);
                        m_AudioCacheSize += e.Data.Length;

                        if (m_AudioCacheSize >= MaxCacheSize)
                        {
                            if (m_RawAudioFilePath.Length > 0 && File.Exists(m_RawAudioFilePath))
                            {
                                try
                                {
                                    using (var fs = new FileStream(m_RawAudioFilePath, FileMode.Append, FileAccess.Write))
                                    {
                                        var writer = new BinaryWriter(fs);
                                        writer.Write(m_AudioCache.ToArray());
                                        writer.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonLog.Error(ex.ToString());
                                }
                            }
                            else CommonLog.Error("Audio stream data file not found: " + m_RawAudioFilePath);

                            m_AudioCacheSize = 0;
                            if (m_AudioCache != null) m_AudioCache.Dispose();
                            m_AudioCache = new MemoryStream();
                        }
                    }

                } // end if filesize ok

            } // end of lock

            if (overloadFileName.Length > 0)
            {
                Task.Factory.StartNew(() => Export(overloadFileName));
            }
        }

        private void WhenMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            try
            {
                if (Info.Status == "closed") return;
                if (e.Message.Length <= 0) return;
                else
                {
                    IsReceiving = true;
                    Info.LastActiveTime = DateTime.Now;
                    Info.LastDataTime = Info.LastActiveTime.ToString("yyyy-MM-dd HH:mm:ss");
                }

            }
            catch
            {
                return;
            }

            Info.MediaInfo = e.Message.Trim();
            var parts = Info.MediaInfo.Split('|');
            if (parts.Length >= 2)
            {
                m_RawVideoType = "h264";
                m_RawAudioType = parts[1].Trim();
                if (parts[1].IndexOf('x') > 0 && parts[1].Length > 4) m_RawAudioType = parts[0].Trim();
            }
            else if (parts.Length >= 1)
            {
                if (Info.MediaInfo.IndexOf('x') > 0 && Info.MediaInfo.Length > 4)
                {
                    m_RawVideoType = "h264";
                    m_RawAudioType = "";
                }
                else
                {
                    m_RawVideoType = "";
                    m_RawAudioType = parts[0].Trim();
                }
            }

            if (m_RawAudioType.Length > 0 && m_RawAudioType.Contains('@'))
            {
                m_RawAudioType = m_RawAudioType.Split('@')[0].Trim();
            }

            if (m_RawAudioType.Length > 0 && m_RawAudioType.Contains('('))
            {
                m_RawAudioType = m_RawAudioType.Split('(')[0].Trim();
            }
        }
    }
}
