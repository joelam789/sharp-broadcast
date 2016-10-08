using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class H264Handler : IMediaHandler 
    {
        public readonly byte[] MAGIC_TAG = { 0x0, 0x0, 0x0, 0x1 };
        public const int MAGIC_TAG_LEN = 4;

        public const int MAX_CACHE_SIZE = 1024 * 1024;

        public const string MEDIA_TYPE = "h264";

        public H264Handler()
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
                
                bool foundFirstMagicTag = false;

                int currentByte = 0;

                List<byte> cache = new List<byte>();

                int inputBufferSize = mediaServer.InputBufferSize > 0 ? mediaServer.InputBufferSize : 1;
                byte[] inputBuffer = new byte[inputBufferSize];

                int currentSize = 0;
                bool foundError = false;

                try
                {
                    while (mediaServer.IsWorking())
                    {
                        currentSize = inputStream.Read(inputBuffer, 0, inputBuffer.Length);
                        if (currentSize <= 0) break;
                        foundError = false;

                        for (var currentIdx = 0; currentIdx < currentSize; currentIdx++)
                        {
                            currentByte = inputBuffer[currentIdx];

                            bool foundTag = false;

                            if (currentByte == MAGIC_TAG.Last() && cache.Count >= MAGIC_TAG_LEN - 1)
                            {
                                foundTag = true;
                                for (var i = 1; i <= MAGIC_TAG_LEN - 1; i++)
                                {
                                    if (cache[cache.Count - i] != MAGIC_TAG[MAGIC_TAG_LEN - 1 - i])
                                    {
                                        foundTag = false;
                                        break;
                                    }
                                }
                                if (foundTag) cache.RemoveRange(cache.Count - (MAGIC_TAG_LEN - 1), MAGIC_TAG_LEN - 1);
                            }

                            if (foundTag)
                            {
                                if (!foundFirstMagicTag)
                                {
                                    foundFirstMagicTag = true;
                                }
                                else
                                {
                                    // here we would ignore MediaServer.OutputBufferSize ...

                                    byte[] data = cache.ToArray();
                                    foreach (var channel in channels) channel.Process(new BufferData(data, data.Length));
                                }

                                cache.Clear();
                                cache.AddRange(MAGIC_TAG);
                            }
                            else
                            {
                                cache.Add((byte)currentByte);
                            }

                            if (cache.Count > MAX_CACHE_SIZE)
                            {
                                mediaServer.Logger.Error("Exceeded max cache size when read H264 input.");
                                foundError = true;
                                break;
                            }
                        }

                        if (foundError) break;
                    }
                }
                catch (Exception ex)
                {
                    mediaServer.Logger.Error("H264 input error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("H264 input process error in thread: " + ex.Message);
            }
        }
    }
    
}
