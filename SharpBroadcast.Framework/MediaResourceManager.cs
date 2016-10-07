using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class MediaResourceManager
    {
        protected Dictionary<string, IMediaServer> m_Servers = new Dictionary<string, IMediaServer>();
        protected Dictionary<string, IMediaHandler> m_Handlers = new Dictionary<string, IMediaHandler>();
        protected Dictionary<string, MediaChannel> m_Channels = new Dictionary<string, MediaChannel>();

        protected List<string> m_AvailableChannelNames = new List<string>();

        public bool IsAvailableChannelName(string channelName)
        {
            return m_AvailableChannelNames.Count <= 0 || m_AvailableChannelNames.Contains(channelName);
        }
        public List<string> GetAvailableChannelNames()
        {
            return new List<string>(m_AvailableChannelNames);
        }
        public MediaChannel GetChannel(string channelName, bool allowToCreateNew = true)
        {
            MediaChannel result = null;
            if (m_Channels.TryGetValue(channelName, out result)) return result;
            else lock (m_Channels) // lock it just when we have to modify it
            {
                if (m_Channels.TryGetValue(channelName, out result)) return result;
                if (!allowToCreateNew) return null;
                if (m_AvailableChannelNames.Count <= 0 || m_AvailableChannelNames.Contains(channelName))
                {
                    var channel = new MediaChannel(channelName);
                    foreach (var item in m_Servers)
                    {
                        if (item.Value.OutputPort > 0 && item.Value.IsWorking()) channel.AddServer(item.Value);
                    }
                    m_Channels[channelName] = channel;
                    return channel;
                }
                else return null;
            }
        }

        public void AddServer(IMediaServer mediaServer)
        {
            if (mediaServer == null) return;
            if (m_Servers.ContainsKey(mediaServer.ServerName)) m_Servers.Remove(mediaServer.ServerName);
            m_Servers.Add(mediaServer.ServerName, mediaServer);
        }
        public List<IMediaServer> GetServerList()
        {
            List<IMediaServer> list = new List<IMediaServer>();
            foreach (var item in m_Servers) list.Add(item.Value);
            return list;
        }

        // it is not thread safe ...
        public void AddHandler(IMediaHandler handler)
        {
            string mediaType = handler.GetMediaType();
            if (m_Handlers.ContainsKey(mediaType)) m_Handlers.Remove(mediaType);
            m_Handlers.Add(mediaType, handler);
        }

        // and this is not thread safe, too ... 
        // but "A Dictionary can support multiple readers concurrently, as long as the collection is not modified."
        public IMediaHandler GetHandler(string mediaType)
        {
            IMediaHandler result = null;
            if (m_Handlers.TryGetValue(mediaType, out result)) return result;
            else return null;
        }

        public int LoadHandlers(string nameSpace = "", Assembly assembly = null)
        {
            m_Handlers.Clear();

            Assembly targetAssembly = assembly;
            if (targetAssembly == null) targetAssembly = Assembly.GetExecutingAssembly();

            if (targetAssembly != null)
            {
                var types = targetAssembly.GetTypes();
                foreach (var t in types)
                {
                    if (t == null || t.Namespace == null) continue;
                    if ((nameSpace != null && nameSpace.Length > 0) ? t.Namespace == nameSpace : t.Namespace.Length > 0)
                    {
                        if (t.GetInterfaces().Contains(typeof(IMediaHandler)))
                        {
                            AddHandler((IMediaHandler)Activator.CreateInstance(t));
                        }
                    }
                }
            }

            return m_Handlers.Count;
        }

        public int SetAvailableChannelNames(List<string> channelNames)
        {
            m_AvailableChannelNames.Clear();
            m_AvailableChannelNames.AddRange(channelNames);
            return m_AvailableChannelNames.Count;
        }

        public int SetAvailableChannelNames(string availableNames)
        {
            List<string> channelNames = new List<string>();
            if (availableNames != null && availableNames.Length > 0)
            {
                var items = availableNames.Split(',').ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var channelName = item.Trim();
                        if (!channelNames.Contains(channelName))
                            channelNames.Add(channelName);
                    }
                }
            }
            return SetAvailableChannelNames(channelNames);
        }

        public void StopAll()
        {
            foreach (var item in m_Servers) item.Value.Stop();
            lock (m_Channels) foreach (var item in m_Channels) item.Value.Stop();
        }

        public void Clear()
        {
            StopAll();
            lock (m_Channels) m_Channels.Clear();
            m_Servers.Clear();
        }
    }
}
