using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.MediaEncoder
{
    public class CommandGenerator
    {
        public static string GenInputPart(string videoDevice, string audioDevice,
                                            string videoOptions, string audioOptions)
        {
            string input = "";

            if (videoDevice.Length > 0)
            {
                input += " -f dshow " + videoOptions + " -i video=\"" + videoDevice + "\"";
            }

            if (audioDevice.Length > 0)
            {
                input += " -f dshow " + audioOptions + " -i audio=\"" + audioDevice + "\"";
            }

            return input;
        }

        public static string GenInputPart(string sourceURL)
        {
            string input = "";
            string url = sourceURL == null ? "" : sourceURL.Trim();

            if (url.Length > 0)
            {
                if (url.StartsWith("[") && url.EndsWith("]"))
                {
                    input = url.Substring(1, url.Length - 2).Trim();
                    if (input.Length > 0) input = " " + input;
                }
                else
                {
                    input = " -re -i \"" + url + "\"";
                }
            }

            return input;
        }

        public static string GenVideoOutputPart(VideoOutputTask task)
        {
            string output = " -s " + task.Resolution 
                            + (task.FPS > 0 ? (" -r " + task.FPS) : "")
                            + (task.Bitrate > 0 ? (" -b:v " + task.Bitrate + "k ") : "") 
                            ;

            string serverUrl = task.ServerAddress.ToLower().Trim();
            if (!serverUrl.Contains("://")) serverUrl = "http://" + serverUrl;

            if (serverUrl.Last() == '/') serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);

            if (task.VideoType == "mpeg")
            {
                output += " -codec:v mpeg1video -bf 0 ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f mpegts ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.VideoType + "/" + task.Resolution 
                            + (task.FPS > 0 ? ("x" + task.FPS) : "")
                            + (task.Bitrate > 0 ? ("" + '@' + task.Bitrate + "kbps") : "") 
                            ;
            }
            else if (task.VideoType == "h264")
            {
                output += " -an -vcodec libx264 -tune zerolatency -pass 1 -coder 0 -bf 0 -flags -loop -wpredp 0 ";
                if (task.PixelFormat.Length > 0) output += " -pix_fmt " + task.PixelFormat;
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f h264 ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.VideoType + "/" + task.Resolution
                            + (task.FPS > 0 ? ("x" + task.FPS) : "")
                            + (task.Bitrate > 0 ? ("" + '@' + task.Bitrate + "kbps") : "")
                            ;
            }
            else if (task.VideoType == "rtmp")
            {
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f flv ";
                output += serverUrl + "/" + task.ChannelName
                            ;
            }
            else return "";

            return output;
        }

        public static string GenVideoOutputPart(List<VideoOutputTask> tasks)
        {
            string output = "";
            foreach (var task in tasks) output += GenVideoOutputPart(task);
            return output;
        }

        public static string GenNormalAudioOutputPart(AudioOutputTask task)
        {
            string output = " -vn " + (task.Bitrate > 0 ? (" -b:a " + task.Bitrate + "k ") : "");

            string serverUrl = task.ServerAddress.ToLower().Trim();
            if (!serverUrl.Contains("://")) serverUrl = "http://" + serverUrl;

            if (serverUrl.Last() == '/') serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);

            if (task.AudioType == "mp3")
            {
                output += " -acodec libmp3lame -reservoir 0 ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f mp3 ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType;
            }
            else if (task.AudioType == "ogg")
            {
                output += " -acodec libvorbis ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f ogg ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType;
            }
            else if (task.AudioType == "opus")
            {
                output += " -acodec libopus ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f opus ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType;
            }
            else if (task.AudioType == "pcm")
            {
                output += " -acodec pcm_u8 -ac 1 ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                string sampleRate = "";
                if (task.ExtraParam.Contains("-ar "))
                {
                    var extraParamParts = task.ExtraParam.Split(' ');
                    bool foundAR = false;
                    foreach (var item in extraParamParts)
                    {
                        var currentPart = item.Trim();
                        if (currentPart.Length <= 0) continue;
                        if (foundAR) sampleRate = currentPart;
                        else
                        {
                            foundAR = currentPart == "-ar";
                            if (foundAR) continue;
                        }
                        if (sampleRate.Length > 0) break;
                    }
                }
                output += " -f u8 ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType
                            + (sampleRate.Length > 0 ? ("/" + sampleRate) : "");
            }
            else if (task.AudioType == "aac")
            {
                output += " -codec:a libfdk_aac ";
                if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                output += " -f adts ";
                output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType;
            }
            else return "";

            return output;
        }

        public static string GenNormalAudioOutputPart(List<AudioOutputTask> tasks)
        {
            string output = "";
            foreach (var task in tasks) output += GenNormalAudioOutputPart(task);
            return output;
        }

        public static string GenAudioOutputPart(List<AudioOutputTask> tasks, string aacEncoder = "")
        {
            string output = "";

            if (aacEncoder == null || aacEncoder.Length <= 0)
            {
                output = GenNormalAudioOutputPart(tasks);
            }
            else
            {
                List<AudioOutputTask> audioTasks = new List<AudioOutputTask>();
                List<AudioOutputTask> aacTasks = new List<AudioOutputTask>();

                foreach (var task in tasks)
                {
                    if (task.AudioType == "aac") aacTasks.Add(task);
                    else audioTasks.Add(task);
                }

                output = GenNormalAudioOutputPart(audioTasks);
                if (aacTasks.Count > 0)
                {
                    output += " -vn -f nut pipe:1 | " + aacEncoder + " -y -i pipe:0 ";
                    foreach (var task in aacTasks)
                    {
                        string serverUrl = task.ServerAddress.ToLower().Trim();
                        if (!serverUrl.Contains("http://")) serverUrl = "http://" + serverUrl;
                        if (serverUrl.Last() == '/') serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);
                        output += " -vn -b:a " + task.Bitrate + "k " + " -codec:a libfdk_aac ";
                        if (task.ExtraParam.Length > 0) output += " " + task.ExtraParam + " ";
                        output += " -f adts ";
                        output += serverUrl + "/" + task.ChannelName + "/" + task.AudioType;
                    }
                }
            }

            return output;
        }

    }

    public class VideoOutputTask
    {
        public string VideoType { get; set; }

        public string Resolution { get; set; }

        public int FPS { get; set; }

        public int Bitrate { get; set; }

        public string PixelFormat { get; set; }

        public string ExtraParam { get; set; }

        public string ServerAddress { get; set; }

        public string ChannelName { get; set; }

        public VideoOutputTask()
        {
            VideoType = "h264";
            Resolution = "640x360";
            FPS = 25;
            Bitrate = 512;
            PixelFormat = "yuv420p";
            ExtraParam = "";
            ServerAddress = "127.0.0.1:9310";
            ChannelName = "test-video";
        }

        public VideoOutputTask(VideoOutputTask src)
        {
            VideoType = src.VideoType;
            Resolution = src.Resolution;
            FPS = src.FPS;
            Bitrate = src.Bitrate;
            PixelFormat = src.PixelFormat;
            ExtraParam = src.ExtraParam;
            ServerAddress = src.ServerAddress;
            ChannelName = src.ChannelName;
        }
    }

    public class VideoOutputTaskGroup
    {
        public List<VideoOutputTask> Tasks { get; set; }

        public VideoOutputTaskGroup()
        {
            Tasks = new List<VideoOutputTask>();
        }
    }

    public class AudioOutputTask
    {
        public string AudioType { get; set; }

        public int Bitrate { get; set; }

        public string ExtraParam { get; set; }

        public string ServerAddress { get; set; }

        public string ChannelName { get; set; }

        public AudioOutputTask()
        {
            AudioType = "mp3";
            Bitrate = 8;
            ExtraParam = "";
            ServerAddress = "127.0.0.1:9210";
            ChannelName = "test-audio";
        }

        public AudioOutputTask(AudioOutputTask src)
        {
            AudioType = src.AudioType;
            Bitrate = src.Bitrate;
            ExtraParam = src.ExtraParam;
            ServerAddress = src.ServerAddress;
            ChannelName = src.ChannelName;
        }
    }

    public class AudioOutputTaskGroup
    {
        public List<AudioOutputTask> Tasks { get; set; }

        public AudioOutputTaskGroup()
        {
            Tasks = new List<AudioOutputTask>();
        }
    }
}
