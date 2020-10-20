using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using SharpNetwork.Core;
using SharpNetwork.SimpleWebSocket;

namespace SimpleWebSocketPipe
{
    public class WebSocketServer
    {
        public Server ActualServer { get; private set; }

        public WebSocketServer(string certFile = "", string certKey = "")
        {
            ActualServer = new Server();
            ActualServer.SetIoFilter(new MessageCodec(1024 * 1024 * 10)); // set max buffer size t0 10m ...
            ActualServer.SetIoHandler(new ServerNetworkEventHandler());

            try
            {
                string certFilepath = certFile == null ? "" : String.Copy(certFile).Trim();

                if (certFilepath.Length > 0)
                {
                    certFilepath = certFilepath.Replace('\\', '/');
                    if (certFilepath[0] != '/' && certFilepath.IndexOf(":/") != 1) // if it is not abs path
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
                        if (folder != null && folder.Length > 0) certFilepath = folder.Replace('\\', '/') + "/" + certFilepath;
                    }

                    //Console.WriteLine("Try to load cert: " + certFilepath);

                    if (File.Exists(certFilepath))
                    {
                        if (certKey == null) ActualServer.SetCert(new X509Certificate2(certFilepath));
                        else ActualServer.SetCert(new X509Certificate2(certFilepath, certKey));
                        //Console.WriteLine("Loaded cert: " + certFilepath);
                    }
                    else
                    {
                        //Console.WriteLine("Cert file not found: " + certFilepath);
                        throw new Exception("Cert file not found: " + certFilepath);
                    }
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Failed to setup SSL: " + ex.Message);
                throw new Exception("Failed to setup SSL: " + ex.Message);
            }

        }
    }

    public class ServerNetworkEventHandler : NetworkEventHandler
    {
        Stream m_StandardOutput = null;

        void CloseStandardOutput()
        {
            try
            {
                if (m_StandardOutput != null)
                {
                    m_StandardOutput.Close();
                    m_StandardOutput.Dispose();
                    m_StandardOutput = null;
                }
            }
            catch { }
        }
        void OpenStandardOutput()
        {
            CloseStandardOutput();
            m_StandardOutput = Console.OpenStandardOutput();
        }

        public ServerNetworkEventHandler() : base()
        {
            IsOrderlyProcess = true;
        }

        public override void OnConnect(Session session)
        {
            base.OnConnect(session);

            //session.MaxWriteQueueSize = m_MediaServer.OutputQueueSize;
            session.FitWriteQueueAction = Session.ACT_KEEP_OLD;

            //if (m_MediaServer.OutputSocketBufferSize > 0)
            //    session.SetSendBufferSize(m_MediaServer.OutputSocketBufferSize);

            //m_StandardOutput = File.Create("test.webm");
            OpenStandardOutput();
        }

        public override void OnHandshake(Session session)
        {
            base.OnHandshake(session);
        }

        public override void OnDisconnect(Session session)
        {
            base.OnDisconnect(session);
            CloseStandardOutput();
            Environment.Exit(0);
        }

        public override void OnError(Session session, int errortype, Exception error)
        {
            base.OnError(session, errortype, error);
            //Console.Write("OnError");
        }

        protected override void ProcessMessage(SessionContext ctx)
        {
            //Console.Write("ProcessMessage");

            Session session = ctx.Session;
            WebMessage msg = (WebMessage)ctx.Data;

            if (m_StandardOutput != null)
                m_StandardOutput.Write(msg.RawContent, 0, msg.RawContent.Length);
        }

    }
}
