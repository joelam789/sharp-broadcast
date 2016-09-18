using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpBroadcast.MediaEncoder
{
    public class ProcessIoWrapper : IDisposable
    {
        public const int IO_BUF_SIZE = 256;

        public const int FLAG_INPUT  = 1;
        public const int FLAG_OUTPUT = 2;
        public const int FLAG_ERROR  = 8;

        private Process m_RunningProcess = null;

        private Thread m_StandardOutputThread = null;

        private Thread m_StandardErrorThread = null;

        private Timer m_Timer = null;

        private string m_ExitingFlag = "";

        private bool m_IsTerminating = false;

        private StringBuilder m_Buffer = null;

        private ConcurrentQueue<string> m_InputQueue = new ConcurrentQueue<string>();

        public Action<string> OnStandardOutputTextRead { get; set; }

        public Action<string> OnStandardErrorTextRead { get; set; }

        public Action OnProcessExited { get; set; }

        public Process RunningProcess
        {
            get { return m_RunningProcess; }
        }

        private void OnRunningProcessExited(object sender, EventArgs e)
        {
            if (OnProcessExited != null)
            {
                try
                {
                    lock (m_ExitingFlag)
                    {
                        if (m_ExitingFlag.Length <= 0)
                        {
                            m_ExitingFlag = "done";
                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    OnProcessExited();
                                }
                                catch { }
                            });
                        }
                    }
                }
                catch { }
            }
        }

        public ProcessIoWrapper(string file, string args, 
                                int redirectionFlag = FLAG_INPUT | FLAG_OUTPUT | FLAG_ERROR,
                                Encoding encoding = null)
        {
            OnStandardOutputTextRead = null;
            OnStandardErrorTextRead = null;
            OnProcessExited = null;

            ProcessStartInfo pinfo = new ProcessStartInfo(file, args);

            pinfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

            pinfo.UseShellExecute = false;
            pinfo.RedirectStandardInput = (redirectionFlag & FLAG_INPUT) != 0;
            pinfo.RedirectStandardOutput = (redirectionFlag & FLAG_OUTPUT) != 0;
            pinfo.RedirectStandardError = (redirectionFlag & FLAG_ERROR) != 0;
            pinfo.CreateNoWindow = true;

            if (encoding == null)
            {
                if (pinfo.RedirectStandardOutput) pinfo.StandardOutputEncoding = Encoding.UTF8;
                if (pinfo.RedirectStandardError) pinfo.StandardErrorEncoding = Encoding.UTF8;
            }
            else
            {
                if (pinfo.RedirectStandardOutput) pinfo.StandardOutputEncoding = encoding;
                if (pinfo.RedirectStandardError) pinfo.StandardErrorEncoding = encoding;
            }

            m_RunningProcess = new Process();
            m_RunningProcess.StartInfo = pinfo;
            m_RunningProcess.Exited += new EventHandler(OnRunningProcessExited);

            m_Buffer = new StringBuilder(IO_BUF_SIZE);
            m_IsTerminating = false;
            m_ExitingFlag = "";
        }

        public ProcessIoWrapper(ProcessStartInfo pinfo)
        {
            OnStandardOutputTextRead = null;
            OnStandardErrorTextRead = null;
            OnProcessExited = null;

            m_RunningProcess = new Process();
            m_RunningProcess.StartInfo = pinfo;
            m_RunningProcess.Exited += new EventHandler(OnRunningProcessExited);

            m_Buffer = new StringBuilder(IO_BUF_SIZE);
            m_IsTerminating = false;
            m_ExitingFlag = "";
        }

        public void Dispose()
        {
            StopRunningProcess();
        }

        public bool StartProcess()
        {
            StopMonitoringProcessOutput();

            while (m_InputQueue.Count > 0)
            {
                string line = "";
                if (!m_InputQueue.TryDequeue(out line)) break;
            }

            m_IsTerminating = false;
            m_ExitingFlag = "";

            lock (m_Buffer)
            {
                if (m_RunningProcess == null) return false;
                m_RunningProcess.Start();
            }

            try
            {
                bool tracedOutput = false;
                bool tracedError = false;

                if (m_RunningProcess.StartInfo.RedirectStandardOutput)
                {
                    m_StandardOutputThread = new Thread(new ThreadStart(ReadStandardOutputThreadMethod));
                    m_StandardOutputThread.IsBackground = true;
                    m_StandardOutputThread.Start();
                    tracedOutput = true;
                }

                if (m_RunningProcess.StartInfo.RedirectStandardError)
                {
                    m_StandardErrorThread = new Thread(new ThreadStart(ReadStandardErrorThreadMethod));
                    m_StandardErrorThread.IsBackground = true;
                    m_StandardErrorThread.Start();
                    tracedError = true;
                }

                if (m_Timer == null) m_Timer = new Timer(CheckRunning, this, 10, 50);

                return (m_RunningProcess != null && !m_RunningProcess.HasExited) && (tracedOutput || tracedError);
            }
            catch { }

            return false;
        }

        private void CheckRunning(object param)
        {
            try
            {
                bool gone = false;
                lock (m_Buffer)
                {
                    gone = m_RunningProcess == null || m_RunningProcess.HasExited;
                }
                if (gone) OnRunningProcessExited(null, null);
            }
            catch { }
        }

        public bool WriteStandardInput(string text)
        {
            lock (m_Buffer)
            {
                try
                {
                    if (m_RunningProcess == null || m_RunningProcess.HasExited
                        || !m_RunningProcess.StartInfo.RedirectStandardInput) return false;

                    m_InputQueue.Enqueue(text); // make sure input will be processed in correct order
                }
                catch { return false; }
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    string line = "";
                    while (!m_IsTerminating && m_InputQueue.TryDequeue(out line))
                    {
                        lock (m_Buffer)
                        {
                            if (m_RunningProcess != null && !m_RunningProcess.HasExited
                                && m_RunningProcess.StartInfo.RedirectStandardInput)
                                m_RunningProcess.StandardInput.WriteLine(line);
                        }
                        Thread.Sleep(50);
                    }
                }
                catch { }
            });

            return true;
        }

        private void ReadStream(int firstCharRead, StreamReader streamReader, bool isStandardOutput)
        {
            lock (m_Buffer)
            {
                int ch = 0;
                
                m_Buffer.Length = 0;
                m_Buffer.Append((char)firstCharRead);
                while (streamReader.Peek() > -1)
                {
                    ch = streamReader.Read();
                    m_Buffer.Append((char)ch);
                    if (ch == '\n') SendBufferToEvent(m_Buffer, isStandardOutput);
                }

                SendBufferToEvent(m_Buffer, isStandardOutput);
            }
        }

        private void SendBufferToEvent(StringBuilder buffer, bool isStandardOutput)
        {
            try
            {
                if (buffer.Length > 0)
                {
                    if (isStandardOutput && OnStandardOutputTextRead != null)
                    {
                        OnStandardOutputTextRead(buffer.ToString());
                    }
                    else if (!isStandardOutput && OnStandardErrorTextRead != null)
                    {
                        OnStandardErrorTextRead(buffer.ToString());
                    }
                    buffer.Length = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process Output Error:" + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }

        private void ReadStandardOutputThreadMethod()
        {
            try
            {
                int ch = 0;
                while (!m_IsTerminating && m_RunningProcess != null && (ch = m_RunningProcess.StandardOutput.Read()) > -1)
                    ReadStream(ch, m_RunningProcess.StandardOutput, true);
            }
            catch (Exception ex)
            {
                if(!m_IsTerminating) Console.WriteLine("Process Error:" + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }

        private void ReadStandardErrorThreadMethod()
        {
            try
            {
                int ch = 0;
                while (!m_IsTerminating && m_RunningProcess != null && (ch = m_RunningProcess.StandardError.Read()) > -1)
                    ReadStream(ch, m_RunningProcess.StandardError, false);
            }
            catch (Exception ex)
            {
                if (!m_IsTerminating) Console.WriteLine("Process Error:" + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }

        public bool HasExited()
        {
            lock (m_Buffer)
            {
                if (m_RunningProcess == null) return true;
                else return m_RunningProcess.HasExited;
            }
        }

        public void StopMonitoringProcessOutput()
        {
            try
            {
                if (m_StandardOutputThread != null) m_StandardOutputThread.Abort();
                m_StandardOutputThread = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process Monitoring Error: " + ex.Message + "\nStack Trace:\n" + ex.StackTrace);
            }

            try
            {
                if (m_StandardErrorThread != null) m_StandardErrorThread.Abort();
                m_StandardErrorThread = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Process Monitoring Error: " + ex.Message + "\nStack Trace:\n" + ex.StackTrace);
            }

            if (m_Timer != null)
            {
                m_Timer.Dispose();
                m_Timer = null;
            }
        }

        public void StopRunningProcess()
        {
            StopMonitoringProcessOutput();

            try
            {
                int errtimes = 0;
                while (m_InputQueue.Count > 0)
                {
                    string line = "";
                    if (!m_InputQueue.TryDequeue(out line)) errtimes++;
                    if (errtimes > 128) break;
                }
            }
            catch { }
            

            try
            {
                m_IsTerminating = true;
                Thread.Sleep(50);

                lock (m_Buffer)
                {
                    if (m_RunningProcess != null)
                    {
                        for (var i = 0; i < 5; i++)
                        {
                            try
                            {
                                if (m_RunningProcess.HasExited) break;
                                else m_RunningProcess.Kill();
                                Thread.Sleep(100);
                            }
                            catch { }
                        }
                        try
                        {
                            m_RunningProcess.Dispose();
                        }
                        catch { }
                    }

                    m_RunningProcess = null;
                }
            }
            catch { }

            Thread.Sleep(50);

            OnRunningProcessExited(null, null);

            m_IsTerminating = false;
        }
    }
}
