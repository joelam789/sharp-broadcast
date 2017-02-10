using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Common.Logging;

namespace SharpBroadcast.StreamRecorder
{
    public interface IServerLogger
    {
        void Info(string msg);

        void Debug(string msg);

        void Warn(string msg);

        void Error(string msg);
    }

    public static class CommonLog
    {
        static readonly IServerLogger m_Logger = null;

        static Form m_Form = null;
        static RichTextBox m_TextBox = null;

        static CommonLog()
        {
            m_Logger = new ServerLogger();
        }

        public static void SetGuiControl(Form form, RichTextBox textbox)
        {
            m_Form = form;
            m_TextBox = textbox;
        }

        private static void TryToSendLogToGui(string msg)
        {
            if (m_Form != null && m_TextBox != null)
            {
                m_Form.BeginInvoke((Action)(() =>
                {
                    if (m_TextBox.Lines.Length > 256)
                    {
                        List<string> finalLines = m_TextBox.Lines.ToList();
                        finalLines.RemoveRange(0, 128);
                        m_TextBox.Lines = finalLines.ToArray();
                    }
                    m_TextBox.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + msg + Environment.NewLine);
                    m_TextBox.SelectionStart = m_TextBox.Text.Length;
                    m_TextBox.ScrollToCaret();
                }));
            }
        }

        public static IServerLogger GetLogger()
        {
            return m_Logger;
        }

        public static void Info(string msg)
        {
            if (m_Logger != null) m_Logger.Info(msg);
            TryToSendLogToGui("[INFO] " + msg);
        }

        public static void Debug(string msg)
        {
            if (m_Logger != null) m_Logger.Debug(msg);
            TryToSendLogToGui("[DEBUG] " + msg);
        }

        public static void Warn(string msg)
        {
            if (m_Logger != null) m_Logger.Warn(msg);
            TryToSendLogToGui("[WARN] " + msg);
        }

        public static void Error(string msg)
        {
            if (m_Logger != null) m_Logger.Error(msg);
            TryToSendLogToGui("[ERROR] " + msg);
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
