using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class MpegHandler : IMediaHandler
    {
        public const string HEADER_TAG = "jsmp";
        public const Int16 VIDEO_DEFAULT_WIDTH = 320;
        public const Int16 VIDEO_DEFAULT_HEIGHT = 240;

        public const string MEDIA_TYPE = "mpeg";

        protected Dictionary<string, byte[]> m_Headers = new Dictionary<string, byte[]>();

        public MpegHandler()
        {
            // ...
        }

        public string GetMediaType()
        {
            return MEDIA_TYPE;
        }

        private static byte[] GenHeader(Int16 videoWidth, Int16 videoHeight)
        {
            byte[] header = null;
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] tag = ASCIIEncoding.Default.GetBytes(HEADER_TAG);
                byte[] width = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(videoWidth));
                byte[] height = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(videoHeight));

                stream.Write(tag, 0, tag.Length);
                stream.Write(width, 0, width.Length);
                stream.Write(height, 0, height.Length);

                byte[] buffer = stream.ToArray();

                header = new byte[buffer.Length];
                Buffer.BlockCopy(buffer, 0, header, 0, header.Length);
            }
            return header;
        }

        public byte[] GetWelcomeData(string param)
        {
            byte[] data = null;

            lock (m_Headers)
            {
                if (m_Headers.ContainsKey(param)) data = m_Headers[param];
            }

            return data;
        }

        public void HandleInput(IMediaServer mediaServer, List<MediaChannel> channels, Stream inputStream, string mediaInfo)
        {
            try
            {
                var channel = channels.First();
                string sourceName = channel.ChannelName;

                if (sourceName.Length > 0)
                {
                    Int16 width = VIDEO_DEFAULT_WIDTH;
                    Int16 height = VIDEO_DEFAULT_HEIGHT;

                    string vinfo = "";

                    // new jsmepg would not need "header" message anymore
                    /*
                    lock (m_Headers)
                    {
                        if (m_Headers.ContainsKey(sourceName)) m_Headers.Remove(sourceName); // refresh it

                        if (mediaInfo.Length > 0)
                        {
                            try
                            {
                                var mpegInfo = mediaInfo.Replace("/", "");
                                if (mpegInfo.Contains('(') && mpegInfo.Contains(')'))
                                    mpegInfo = mpegInfo.Substring(mpegInfo.IndexOf('(') + 1,
                                        mpegInfo.IndexOf(')') - mpegInfo.IndexOf('(') - 1);

                                vinfo = mpegInfo;
                                var parts = (vinfo.Split('@')[0]).Split('x');

                                if (parts.Length > 0) width = Convert.ToInt16(parts[0]);
                                if (parts.Length > 1) height = Convert.ToInt16(parts[1]);
                            }
                            catch
                            {
                                width = VIDEO_DEFAULT_WIDTH;
                                height = VIDEO_DEFAULT_HEIGHT;
                            }
                        }

                        byte[] header = GenHeader(width, height);
                        m_Headers.Add(sourceName, header);
                    }
                    */

                    if (mediaInfo.Length > 0)
                    {
                        try
                        {
                            var mpegInfo = mediaInfo.Replace("/", "");
                            if (mpegInfo.Contains('(') && mpegInfo.Contains(')'))
                                mpegInfo = mpegInfo.Substring(mpegInfo.IndexOf('(') + 1,
                                    mpegInfo.IndexOf(')') - mpegInfo.IndexOf('(') - 1);

                            vinfo = mpegInfo;
                            var parts = (vinfo.Split('@')[0]).Split('x');

                            if (parts.Length > 0) width = Convert.ToInt16(parts[0]);
                            if (parts.Length > 1) height = Convert.ToInt16(parts[1]);
                        }
                        catch
                        {
                            width = VIDEO_DEFAULT_WIDTH;
                            height = VIDEO_DEFAULT_HEIGHT;
                        }
                    }

                    if (vinfo.Length <= 0) vinfo = width + "x" + height;
                }

                // new jsmepg would not need "header" message anymore
                //channel.SetWelcomeData(GetWelcomeData(sourceName));

                int inputBufferSize = mediaServer.InputBufferSize > 0 ? mediaServer.InputBufferSize : 1;

                while (mediaServer.IsWorking())
                {
                    int realSize = 0;
                    byte[] data = new byte[inputBufferSize]; // MpegHandler will ignore MediaServer.OutputBufferSize
                    try
                    {
                        realSize = inputStream.Read(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        realSize = 0;
                        mediaServer.Logger.Error("MPEG1 input error: " + ex.Message);
                    }

                    if (realSize <= 0) break;

                    channel.Process(new BufferData(data, realSize));

                }

                if (sourceName.Length > 0)
                {
                    lock (m_Headers)
                    {
                        if (m_Headers.ContainsKey(sourceName)) m_Headers.Remove(sourceName);
                    }
                }
            }
            catch (Exception ex)
            {
                mediaServer.Logger.Error("MPEG1 input process error in thread: " + ex.Message);
            }
        }
    }
}
