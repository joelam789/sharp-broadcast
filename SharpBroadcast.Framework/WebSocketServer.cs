using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using SharpNetwork;
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
            ActualServer.SetIoHandler(new ServerNetworkEventHandler(new MessageDispatcher(), server));

            try
            {
                if (certFile != null && certFile.Length > 0 && File.Exists(certFile))
                {
                    if (certKey == null) ActualServer.SetCert(new X509Certificate2(certFile));
                    else ActualServer.SetCert(new X509Certificate2(certFile, certKey));
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

        public ServerNetworkEventHandler(MessageDispatcher dispatcher, IMediaServer mediaServer)
            : base(dispatcher)
        {
            m_MediaServer = mediaServer;
        }

        public override void OnConnect(Session session)
        {
            base.OnConnect(session);

            session.SetMaxMessageQueueSize(Session.IO_SEND, m_MediaServer.OutputQueueSize);
            session.SetQueueOverflowAction(Session.IO_SEND, Session.ACT_KEEP_NEW);
            if (m_MediaServer.OutputSocketBufferSize > 0)
                session.SetBufferSize(Session.IO_SEND, m_MediaServer.OutputSocketBufferSize);
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

        public override void OnError(Session session, int errortype, string errormsg)
        {
            base.OnError(session, errortype, errormsg);

            if (errormsg != null && errormsg.Length > 0)
                m_MediaServer.Logger.Error(errormsg);
        }

    }
}
