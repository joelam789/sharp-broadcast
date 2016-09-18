using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;
using SharpBroadcast.Framework;

namespace SharpBroadcast.MediaServer
{
    public static class CommonLog
    {
        static readonly IServerLogger m_Logger = null;

        static CommonLog()
        {
            m_Logger = new ServerLogger();
        }

        public static IServerLogger GetLogger()
        {
            return m_Logger;
        }

        public static void Info(string msg)
        {
            if (m_Logger != null) m_Logger.Info(msg);
        }

        public static void Debug(string msg)
        {
            if (m_Logger != null) m_Logger.Debug(msg);
        }

        public static void Warn(string msg)
        {
            if (m_Logger != null) m_Logger.Warn(msg);
        }

        public static void Error(string msg)
        {
            if (m_Logger != null) m_Logger.Error(msg);
        }
    }

    public class ServerLogger : IServerLogger
    {
        static ILog m_Logger = null;

        public ServerLogger()
        {
            if (m_Logger == null) m_Logger = LogManager.GetLogger(typeof(CommonLog).Name);
        }

        public void Info(string msg)
        {
            if (m_Logger != null) m_Logger.Info(msg);
        }

        public void Debug(string msg)
        {
            if (m_Logger != null) m_Logger.Debug(msg);
        }

        public void Warn(string msg)
        {
            if (m_Logger != null) m_Logger.Warn(msg);
        }

        public void Error(string msg)
        {
            if (m_Logger != null) m_Logger.Error(msg);
        }
    }
}
