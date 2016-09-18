using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public interface IMediaServer
    {
        string ServerName { get; }

        int InputBufferSize { get; set; }

        int OutputQueueSize { get; set; }
        int OutputBufferSize { get; set; }
        int OutputSocketBufferSize { get; set; }

        int InputPort { get; }
        int OutputPort { get; }

        IServerLogger Logger { get; }

        MediaChannel GetChannel(string channelName);

        List<MediaChannelState> GetChannelStates();
        void UpdateState(string channelName, MediaChannelState state);
        void RemoveState(string channelName);

        List<string> GetSourceWhitelist();
        void SetSourceWhitelist(List<string> list);

        void SetClientValidator(IClientValidator validator);

        void SetSSL(string certFile, string certKey);

        void ValidateClient(object client);
        void RemoveClient(object client);
        List<object> GetClients(string channelName);

        bool IsWorking();

        bool Start(int inputPort = -1, int outputPort = -1, List<string> inputWhiteList = null);
        void Stop();

        

    }
}
