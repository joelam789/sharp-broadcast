using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class Mp3Handler : IMediaHandler
    {
        public const byte FIRST_SYNC_BYTE = 0xff;
        public const byte SECOND_SYNC_BYTE = 0xfb;

        public const int MAX_CACHE_SIZE = 1024 * 1024;

        public const string MEDIA_TYPE = "mp3";

        public Mp3Handler()
        {
            // ...
        }

        public string GetMediaType()
        {
            return MEDIA_TYPE;
        }

        private byte[] GenAudioData(List<byte> tag, List<byte> header, List<List<byte>> chunks)
        {
            List<byte> finalData = new List<byte>();

            int frameCount = chunks.Count + 1;
            int dataSize = header.Count;

            finalData.AddRange(tag);
            if (header.Count > 0) finalData.AddRange(header);

            foreach (var item in chunks)
            {
                dataSize += item.Count;
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

                bool foundFirstSyncWord = false;

                List<byte> tagPart = new List<byte>();
                List<byte> headerPart = new List<byte>();

                List<List<byte>> currentChunks = new List<List<byte>>();

                int totalBytes = 0;

                int currentByte = 0;
                int lastByte = 0;

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

                            if (currentByte == SECOND_SYNC_BYTE && lastByte == FIRST_SYNC_BYTE)
                            {
                                if (!foundFirstSyncWord)
                                {
                                    foundFirstSyncWord = true;
                                    tagPart.AddRange(cache);

                                    cache.Clear();
                                    cache.Add(FIRST_SYNC_BYTE);
                                    cache.Add(SECOND_SYNC_BYTE);
                                }
                                else
                                {
                                    currentChunks.Add(new List<byte>(cache));
                                    totalBytes += cache.Count;
                                    if (totalBytes >= mediaServer.OutputBufferSize)
                                    {
                                        byte[] data = GenAudioData(tagPart, headerPart, currentChunks);
                                        foreach (var channel in channels) channel.Process(new BufferData(data, data.Length));

                                        currentChunks.Clear();
                                        totalBytes = 0;
                                    }

                                    cache.Clear();
                                    cache.Add(FIRST_SYNC_BYTE);
                                    cache.Add(SECOND_SYNC_BYTE);
                                }

                                lastByte = currentByte;
                                continue;
                            }

                            if (lastByte == FIRST_SYNC_BYTE) cache.Add(FIRST_SYNC_BYTE);
                            if (currentByte != FIRST_SYNC_BYTE) cache.Add((byte)currentByte);
                            lastByte = currentByte;

                            if (cache.Count > MAX_CACHE_SIZE)
                            {
                                mediaServer.Logger.Error("Exceeded max cache size when read MP3 input.");
                                foundError = true;
                                break;
                            }

                        }

                        if (foundError) break;
                        
                    }
                }
                catch (Exception ex)
                {
                    mediaServer.Logger.Error("MP3 input error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("MP3 input process error in thread: " + ex.Message);
            }
        }
    }
}
