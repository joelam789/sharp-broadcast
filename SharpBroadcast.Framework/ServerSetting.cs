using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class ServerSetting
    {
        public int InputPort { get; set; }

        public int InputBufferSize { get; set; }

        public List<string> InputWhitelist { get; set; }

        public int OutputPort { get; set; }

        public int OutputQueueSize { get; set; }

        public int OutputBufferSize { get; set; }

        public int OutputSocketBufferSize { get; set; }

        public string CertFile { get; set; }

        public string CertKey { get; set; }

        public ServerSetting()
        {
            InputPort = 9210;
            InputBufferSize = 8;
            InputWhitelist = new List<string>();
            OutputPort = 9220;
            OutputQueueSize = 256;
            OutputBufferSize = 256;
            OutputSocketBufferSize = 0;

            CertFile = "";
            CertKey = "";
        }
    }
}
