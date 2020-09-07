using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpBroadcast.MediaEncoder
{
    public class SimpleLog
    {
        public enum LogType
        {
            INFO,
            WARN,
            ERROR
        };

        private class LogConfig
        {
            public string m_FolderPath = "";
            public readonly object m_SyncObject = new object();
            public LogConfig(string folderPath)
            {
                m_FolderPath = folderPath;
            }
        }

        private Dictionary<LogType, LogConfig> m_LogFolderPath = new Dictionary<LogType, LogConfig>();

        private static SimpleLog m_DefaultLogger = new SimpleLog("logs/info", "logs/warn", "logs/error");

        public SimpleLog(string infoLogFolderPath, string warnLogFolderPath, string errLogFolderPath, bool isRelative = true)
        {
            m_LogFolderPath.Clear();

            if (isRelative)
            {
                SetLogFolder(LogType.INFO, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), infoLogFolderPath));
                SetLogFolder(LogType.WARN, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), warnLogFolderPath));
                SetLogFolder(LogType.ERROR, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), errLogFolderPath));
            }
            else
            {
                SetLogFolder(LogType.INFO, infoLogFolderPath);
                SetLogFolder(LogType.WARN, warnLogFolderPath);
                SetLogFolder(LogType.ERROR, errLogFolderPath);
            }
        }

        public static SimpleLog GetDefaultLogger()
        {
            return m_DefaultLogger;
        }

        private void SetLogFolder(LogType logtype, string logFolderPath)
        {
            if (!m_LogFolderPath.ContainsKey(logtype))
            {
                m_LogFolderPath.Add(logtype, new LogConfig(logFolderPath));
            }
            else
            {
                m_LogFolderPath[logtype].m_FolderPath = logFolderPath;
            }

            // Create the log directory if not exist
            try
            {
                if (!Directory.Exists(m_LogFolderPath[logtype].m_FolderPath))
                {
                    Directory.CreateDirectory(m_LogFolderPath[logtype].m_FolderPath);
                }
            }
            catch (Exception ex)
            {
                // Ignore it
                Console.WriteLine(ex.ToString());
            }

        }

        public void WriteLog(String msg, LogType logtype = LogType.INFO)
        {
            WriteLogToConsole(msg, logtype);
            WriteLogToFile(msg, logtype);
        }

        public void WriteLogToConsole(String msg, LogType logtype = LogType.INFO)
        {
            string logmsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz") + " >> " + msg;
            if (logtype == LogType.ERROR) logmsg = "[ERROR] " + logmsg;
            else if (logtype == LogType.WARN) logmsg = "[WARN] " + logmsg;
            else logmsg = "[INFO] " + logmsg;
            Console.WriteLine(logmsg);
        }

        public void WriteLogToFile(String msg, LogType logtype = LogType.INFO)
        {
            try
            {
                if (m_LogFolderPath.ContainsKey(logtype) && m_LogFolderPath[logtype] != null)
                {
                    lock (m_LogFolderPath[logtype].m_SyncObject)
                    {
                        // Must close the file afterward, otherwise if software crashed, then log will be emptied.
                        using (StreamWriter writer = File.AppendText(Path.Combine(m_LogFolderPath[logtype].m_FolderPath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt")))
                        {
                            writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz") + " >> " + msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore it
                Console.WriteLine(ex.ToString());
            }
        }

        public static void Log(String msg, LogType logtype = LogType.INFO)
        {
            try
            {
                if (m_DefaultLogger != null)
                {
                    m_DefaultLogger.WriteLog(msg, logtype);
                }
            }
            catch { }
        }

        public static void Info(String msg)
        {
            Log(msg);
        }

        public static void Warn(String msg)
        {
            Log(msg, LogType.WARN);
        }

        public static void Error(String msg)
        {
            Log(msg, LogType.ERROR);
        }
    }
}
