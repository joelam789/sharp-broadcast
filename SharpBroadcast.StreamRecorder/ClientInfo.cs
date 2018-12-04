using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.StreamRecorder
{
    public class ClientInfo
    {
        public string ChannelName { get; set; }
        public string ChannelURL { get; set; }
        public string MediaInfo { get; set; }
        public string Status { get; set; }
        public string LastDataTime { get; set; }

        public int ErrorTimes { get; set; }
        public DateTime LastActiveTime { get; set; }

        public ClientInfo()
        {
            ChannelName = "";
            ChannelURL = "";

            MediaInfo = "";

            Status = "closed";
            LastDataTime = "?";

            ErrorTimes = 0;
            LastActiveTime = DateTime.Now;
        }
    }

    public class RecordConfig
    {
        public string Converter { get; set; }
        public string Callback { get; set; }

        public string StreamDataFolder { get; set; }
        public string RecordFileFolder { get; set; }
        public string RecordContentType { get; set; }

        public int MustCreateOutputFile { get; set; }

        public decimal VideoStartOffset { get; set; }
        public decimal AudioStartOffset { get; set; }

        public int MaxCacheSize { get; set; }
        public int MaxRecordSize { get; set; }

        public RecordConfig()
        {
            Converter = "ffmpeg";
            Callback = "";

            StreamDataFolder = "";
            RecordFileFolder = "";
            RecordContentType = "auto";

            MustCreateOutputFile = 0;

            VideoStartOffset = 0;
            AudioStartOffset = 0;

            MaxCacheSize = 0;
            MaxRecordSize = 0;
        }
    }
}
