using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

using SharpNetwork.Core;
using SharpNetwork.SimpleWebSocket;

namespace SharpBroadcast.Framework
{
    public class MediaDispatcher
    {
        public ConcurrentQueue<BufferData> Buffers { get; private set; }

        private bool m_IsWorking = false;
        private bool m_IsDispatching = false;

        private Thread m_WorkerThread = null;
        private ManualResetEvent m_Watcher = null;

        private List<ChannelServerItem> m_OutputList;

        private List<TaskFactory> m_OutputTaskPools = new List<TaskFactory>();

        public MediaDispatcher(List<ChannelServerItem> outputList)
        {
            Buffers = new ConcurrentQueue<BufferData>();

            m_IsWorking = false;

            m_OutputList = outputList;

            m_Watcher = new ManualResetEvent(false);

            for (var i = 0; i < Environment.ProcessorCount; i++)
                m_OutputTaskPools.Add(new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(1)));

        }

        private void Dispatch()
        {
            m_IsDispatching = true;
            while (m_IsWorking)
            {
                try
                {
                    if (m_IsWorking == true)
                    {
                        m_Watcher.Reset();
                        m_Watcher.WaitOne();
                    }

                    if (m_IsWorking == false) break;

                    BufferData buffer = null;
                    List<object> list = new List<object>();
                    List<object> badlist = new List<object>();
                    while (m_IsWorking && Buffers.TryDequeue(out buffer))
                    {
                        if (buffer != null && buffer.Length > 0)
                        {
                            list.Clear();
                            badlist.Clear();

                            foreach (var outputItem in m_OutputList)
                            {
                                lock (outputItem.Clients)
                                {
                                    list.AddRange(outputItem.Clients);
                                }
                            }

                            if (list.Count <= 0) continue;

                            foreach (var client in list)
                            {
                                Session session = client as Session;
                                if (session == null) continue;
                                if (session.GetState() > 0)
                                {
                                    try
                                    {
                                        //if (buffer.Buffer != null) session.Send(new WebMessage(buffer.Buffer, buffer.Length));
                                        //else session.Send(new WebMessage(buffer.Text));
                                        var pool = m_OutputTaskPools[session.GetRemotePort() % m_OutputTaskPools.Count];
                                        if (pool != null) pool.StartNew((obj) =>
                                        {
                                            OutputWebData data = obj as OutputWebData;
                                            try
                                            {
                                                if (data != null) data.Sender.Send(data.Message);
                                            }
                                            catch
                                            {
                                                if (data != null && data.Sender != null && data.Sender.UserData != null)
                                                    (data.Sender.UserData as IMediaServer).RemoveClient(client);
                                                if (data != null && data.Sender != null) data.Sender.Close();
                                            }
                                        }, new OutputWebData(session,
                                                buffer.Buffer != null 
                                                ? new WebMessage(buffer.Buffer, buffer.Length)
                                                : new WebMessage(buffer.Text)));

                                    }
                                    catch { badlist.Add(client); }
                                }
                                else badlist.Add(client);
                            }
                            
                            foreach (var client in badlist)
                            {
                                Session session = client as Session;
                                if (session != null && session.UserData != null) (session.UserData as IMediaServer).RemoveClient(client);
                                if (session != null) session.Close();
                            }
                        }
                    }
                }
                catch { }
            }
            m_IsDispatching = false;
        }

        public void Process()
        {
            try
            {
                if (m_IsWorking) m_Watcher.Set();
            }
            catch { }
        }

        public void Start()
        {
            lock (m_Watcher)
            {
                if (m_IsWorking == false)
                {
                    m_IsWorking = true;
                    m_WorkerThread = new Thread(new ThreadStart(Dispatch));
                    m_WorkerThread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (m_Watcher)
            {
                if (m_IsWorking == true)
                {
                    m_IsWorking = false;
                    try { if (m_Watcher != null) m_Watcher.Set(); }
                    catch { }
                    try 
                    {
                        if (m_WorkerThread != null)
                        {
                            // give some time to the worker thread to exit normally
                            for (int i = 0; i < 100; i++)
                            {
                                if (m_IsDispatching) Thread.Sleep(1);
                                else break;
                            }
                            m_WorkerThread.Abort();
                        }
                    }
                    catch { }
                    m_WorkerThread = null;
                }
            }
        }

        public bool IsWorking()
        {
            return m_IsWorking;
        }
    }

    public class BufferData
    {
        public byte[] Buffer { get; private set; }
        public string Text { get; private set; }
        public int Length { get; private set; }

        public BufferData(byte[] buffer, int length)
        {
            Buffer = buffer;
            Length = length;

            Text = null;
        }

        public BufferData(string text)
        {
            Text = text;
            Length = Text.Length;

            Buffer = null;
        }
    }

    public class OutputWebData
    {
        public Session Sender { get; private set; }
        public WebMessage Message { get; private set; }

        public OutputWebData(Session sender, WebMessage msg)
        {
            Sender = sender;
            Message = msg;
        }
    }
}
