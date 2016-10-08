using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{

    public class PcmHandler : IMediaHandler // it should be raw pcm data (pcm_u8)
    {
        public const int MAX_CACHE_SIZE = 1024 * 1024;

        public const string MEDIA_TYPE = "pcm";

        public PcmHandler()
        {
            // ...
        }

        public string GetMediaType()
        {
            return MEDIA_TYPE;
        }

        public void HandleInput(IMediaServer mediaServer, List<MediaChannel> channels, Stream inputStream, string mediaInfo)
        {
            try
            {
                if (mediaInfo != null && mediaInfo.Length > 0)
                {
                    foreach (var channel in channels)
                    {
                        channel.SetWelcomeText(mediaInfo);
                        channel.Process(new BufferData(channel.GetWelcomeText()));
                    }
                }

                // PcmHandler will ignore MediaServer.InputBufferSize
                int inputBufferSize = mediaServer.OutputBufferSize > 0 ? mediaServer.OutputBufferSize : 1;

                try
                {
                    while (mediaServer.IsWorking())
                    {
                        int realSize = 0;
                        byte[] data = new byte[inputBufferSize];
                        try
                        {
                            realSize = inputStream.Read(data, 0, data.Length);
                        }
                        catch (Exception ex)
                        {
                            realSize = 0;
                            mediaServer.Logger.Error("PCM input error: " + ex.Message);
                        }

                        if (realSize <= 0) break;

                        foreach (var channel in channels) channel.Process(new BufferData(data, realSize));
                    }
                }
                catch (Exception ex)
                {
                    mediaServer.Logger.Error("PCM input error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("PCM input process error in thread: " + ex.Message);
            }
        }
    }
}
