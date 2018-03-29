using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using SharpNetwork.Core;
using SharpNetwork.SimpleWebSocket;

namespace SharpBroadcast.Framework
{
    public class WebSocketServer
    {
        public Server ActualServer { get; private set; }

        public WebSocketServer(IMediaServer server, string certFile = "", string certKey = "")
        {
            ActualServer = new Server();
            ActualServer.SetIoFilter(new MessageCodec());
            ActualServer.SetIoHandler(new ServerNetworkEventHandler(server));

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
                    //server.Logger.Info("Try to load cert: " + certFilepath);

                    if (File.Exists(certFilepath))
                    {
                        if (certKey == null) ActualServer.SetCert(new X509Certificate2(certFilepath));
                        else ActualServer.SetCert(new X509Certificate2(certFilepath, certKey));

                        Console.WriteLine("Loaded cert: " + certFilepath);
                        server.Logger.Info("Loaded cert: " + certFilepath);
                    }
                    else
                    {
                        Console.WriteLine("Cert file not found: " + certFilepath);
                        server.Logger.Error("Cert file not found: " + certFilepath);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to setup SSL: " + ex.Message);
                server.Logger.Error("Failed to setup SSL: " + ex.Message);
            }

        }
    }

    public class ServerNetworkEventHandler : NetworkEventHandler
    {
        IMediaServer m_MediaServer = null;

        public ServerNetworkEventHandler(IMediaServer mediaServer): base()
        {
            m_MediaServer = mediaServer;
        }

        public override void OnConnect(Session session)
        {
            base.OnConnect(session);

            session.MaxWriteQueueSize = m_MediaServer.OutputQueueSize;
            session.FitWriteQueueAction = Session.ACT_KEEP_OLD;

            if (m_MediaServer.OutputSocketBufferSize > 0)
                session.SetSendBufferSize(m_MediaServer.OutputSocketBufferSize);
        }

        public override void OnHandshake(Session session)
        {
            base.OnHandshake(session);

            if (m_MediaServer.IsWorking())
            {
                session.UserData = m_MediaServer;
                m_MediaServer.ValidateClient(session);
            }
        }

        public override void OnDisconnect(Session session)
        {
            if (m_MediaServer.IsWorking()) m_MediaServer.RemoveClient(session);

            base.OnDisconnect(session);
        }

        public override void OnError(Session session, int errortype, Exception error)
        {
            base.OnError(session, errortype, error);

            if (Session.IsProcessError(errortype)) m_MediaServer.Logger.Error(error.ToString());
            else m_MediaServer.Logger.Error(error.Message);
        }

    }
}
