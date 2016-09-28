using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public interface IMediaHandler
    {
        string GetMediaType();
        void HandleInput(IMediaServer mediaServer, MediaChannel channel, Stream inputStream, string mediaInfo);
    }
}
