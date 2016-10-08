using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class OggHandler : IMediaHandler // should be FLAC in Ogg (see https://xiph.org/flac/ogg_mapping.html)
    {
        public readonly byte[] MAGIC_TAG = { Convert.ToByte('O'), Convert.ToByte('g'), Convert.ToByte('g'), Convert.ToByte('S') };
        public const int MAGIC_TAG_LEN = 4;

        public const int MAX_CACHE_SIZE = 1024 * 1024;

        public const string MEDIA_TYPE = "ogg";

        public OggHandler()
        {
            // ...
        }

        public string GetMediaType()
        {
            return MEDIA_TYPE;
        }

        private byte[] GenAudioData(List<byte> header, List<List<byte>> chunks)
        {
            List<byte> finalData = new List<byte>();

            if (header.Count > 0) finalData.AddRange(header);

            foreach (var item in chunks)
            {
                finalData.AddRange(item);
            }

            return finalData.ToArray();
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

                List<byte> headerData = new List<byte>();

                List<List<byte>> currentChunks = new List<List<byte>>();

                int totalBytes = 0;

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
                                    currentChunks.Add(new List<byte>(cache));
                                    totalBytes += cache.Count;
                                    if (totalBytes >= mediaServer.OutputBufferSize)
                                    {
                                        if (headerData.Count <= 0 && currentChunks.Count > 1)
                                        {
                                            headerData.AddRange(currentChunks[0]);
                                            headerData.AddRange(currentChunks[1]);
                                            currentChunks.RemoveRange(0, 2);
                                        }
                                        byte[] data = GenAudioData(headerData, currentChunks);
                                        foreach (var channel in channels) channel.Process(new BufferData(data, data.Length));

                                        currentChunks.Clear();
                                        totalBytes = 0;
                                    }
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
                                mediaServer.Logger.Error("Exceeded max cache size when read OGG input.");
                                foundError = true;
                                break;
                            }
                        }

                        if (foundError) break;
                    }
                }
                catch (Exception ex)
                {
                    mediaServer.Logger.Error("OGG input error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("OGG input process error in thread: " + ex.Message);
            }
        }
    }
}
