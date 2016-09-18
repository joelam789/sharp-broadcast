using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class MediaChannel
    {
        protected List<byte> m_WelcomeData = new List<byte>();
        protected string m_WelcomeText = "";

        public string ChannelName { get; private set; }

        public MediaDispatcher Dispatcher { get; private set; }

        public List<ChannelServerItem> OutputList { get; private set; }

        public MediaChannel(string channelName)
        {
            ChannelName = channelName;
            OutputList = new List<ChannelServerItem>();
            Dispatcher = new MediaDispatcher(OutputList);
        }

        public bool AddServer(IMediaServer server)
        {
            if (Dispatcher.IsWorking()) return false; // coz we will not lock OutputList
            OutputList.Add(new ChannelServerItem(server, server.GetClients(ChannelName)));
            return true;
        }

        public void Process(BufferData buffer)
        {
            if (!Dispatcher.IsWorking()) return;
            Dispatcher.Buffers.Enqueue(buffer);
            Dispatcher.Process();
        }

        public void Start()
        {
            Dispatcher.Start();
        }

        public void Stop()
        {
            Dispatcher.Stop();
        }

        public bool IsWorking()
        {
            return Dispatcher.IsWorking();
        }

        public byte[] GetWelcomeData()
        {
            byte[] result = null;
            if (m_WelcomeData.Count <= 0) return result;
            lock (m_WelcomeData)
            {
                result = m_WelcomeData.ToArray();
            }
            return result;
        }

        public void SetWelcomeData(byte[] data)
        {
            if (data == null) return;
            lock (m_WelcomeData)
            {
                m_WelcomeData.Clear();
                m_WelcomeData.AddRange(data);
            }
        }

        public string GetWelcomeText()
        {
            string result = "";
            if (m_WelcomeText == null || m_WelcomeText.Length <= 0) return result;
            lock (m_WelcomeText)
            {
                result = String.Copy(m_WelcomeText);
            }
            return result;
        }

        public void SetWelcomeText(string text)
        {
            if (text == null) return;
            lock (m_WelcomeText)
            {
                m_WelcomeText = String.Copy(text);
            }
        }
    }

    public class ChannelServerItem
    {
        public IMediaServer Server { get; private set; }
        public List<object> Clients { get; private set; }

        public ChannelServerItem(IMediaServer server, List<object> clients)
        {
            Server = server;
            Clients = clients;
        }
    }

    public class ChannelClientItem
    {
        public IMediaServer Server { get; private set; }
        public object Client { get; private set; }

        public ChannelClientItem(IMediaServer server, object client)
        {
            Server = server;
            Client = client;
        }
    }


    public class MediaChannelState
    {
        public string ChannelName { get; set; }

        public string AddressInfo { get; set; }

        public string ServerInfo { get; set; }

        public string MediaInfo { get; set; }

        public int ClientCount { get; set; }

        public MediaChannelState()
        {
            ChannelName = "";
            AddressInfo = "";
            ServerInfo = "";
            MediaInfo = "";
            ClientCount = 0;
        }

        public MediaChannelState(MediaChannelState src)
        {
            ChannelName = src.ChannelName;
            AddressInfo = src.AddressInfo;
            ServerInfo = src.ServerInfo;
            MediaInfo = src.MediaInfo;
            ClientCount = src.ClientCount;
        }
    }
}
