using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class AacHandler : IMediaHandler
    {
        public readonly byte[] MAGIC_TAG = { 0xff, 0xf1, 0x50, 0x80 };
        public const int MAGIC_TAG_LEN = 4;

        public const int MAX_CACHE_SIZE = 1024 * 1024;

        public const string MEDIA_TYPE = "aac";

        public AacHandler()
        {
            // ...
        }

        public string GetMediaType()
        {
            return MEDIA_TYPE;
        }

        private byte[] GenAudioData(List<List<byte>> chunks)
        {
            List<byte> finalData = new List<byte>();

            foreach (var item in chunks) finalData.AddRange(item);

            return finalData.ToArray();
        }

        public void HandleInput(IMediaServer mediaServer, List<MediaChannel> channels, Stream inputStream, string mediaInfo)
        {
            try
            {
                byte[] magicTag = new byte[MAGIC_TAG_LEN];
                Array.Copy(MAGIC_TAG, magicTag, MAGIC_TAG_LEN);

                if (mediaInfo != null && mediaInfo.Length > 0)
                {
                    foreach (var channel in channels)
                    {
                        channel.SetWelcomeText(mediaInfo);
                        channel.Process(new BufferData(channel.GetWelcomeText()));
                    }
                }

                bool foundFirstMagicTag = false;
                int magicByteIndex = 0;

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

                            if (magicByteIndex < MAGIC_TAG_LEN)
                            {
                                magicTag[magicByteIndex] = (byte)currentByte;
                                magicByteIndex++;
                                continue;
                            }

                            bool foundTag = false;

                            if (currentByte == magicTag.Last() && cache.Count >= MAGIC_TAG_LEN - 1)
                            {
                                foundTag = true;
                                for (var i = 1; i <= MAGIC_TAG_LEN - 1; i++)
                                {
                                    if (cache[cache.Count - i] != magicTag[MAGIC_TAG_LEN - 1 - i])
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
                                        byte[] data = GenAudioData(currentChunks);
                                        foreach (var channel in channels) channel.Process(new BufferData(data, data.Length));

                                        currentChunks.Clear();
                                        totalBytes = 0;
                                    }
                                }

                                cache.Clear();
                                cache.AddRange(magicTag);
                            }
                            else
                            {
                                cache.Add((byte)currentByte);
                            }

                            if (cache.Count > MAX_CACHE_SIZE)
                            {
                                mediaServer.Logger.Error("Exceeded max cache size when read AAC input.");
                                foundError = true;
                                break;
                            }
                        }

                        if (foundError) break;
                        
                    }
                }
                catch (Exception ex)
                {
                    mediaServer.Logger.Error("AAC input error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("AAC input process error in thread: " + ex.Message);
            }
        }
    }
}
