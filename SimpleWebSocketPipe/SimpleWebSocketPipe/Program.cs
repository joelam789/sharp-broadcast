using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWebSocketPipe
{
    class Program
    {
        public static string m_CertFile = "";
        public static string m_CertPassword = "";

        public static int m_ServerPort = 0;

        public static WebSocketServer m_Server = null;

        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            foreach (var key in allKeys)
            {
                if (key == "CertFile") m_CertFile = appSettings[key];
                else if (key == "CertPassword") m_CertPassword = appSettings[key];
                else if (key == "ServerPort") m_ServerPort = Convert.ToInt32(appSettings[key].ToString());
            }

            if (m_ServerPort > 0)
            {
                m_Server = new WebSocketServer(m_CertFile, m_CertPassword);
                m_Server.ActualServer.Start(m_ServerPort);
            }

            Thread.Sleep(200);

            if (m_Server.ActualServer.GetState() > 0)
            {
                Console.ReadLine();
            }

            m_Server.ActualServer.Stop();

            Thread.Sleep(200);
        }
    }
}
