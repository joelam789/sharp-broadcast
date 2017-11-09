using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Newtonsoft.Json;

namespace SharpBroadcast.StreamRecorder
{
    partial class RecorderService : ServiceBase
    {
        private RecordServer m_Server = new RecordServer();

        public RecorderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            CommonLog.Info("=== Stream Recorder is starting ===");

            m_Server.Start();

        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.

            m_Server.Stop();

            CommonLog.Info("=== Stream Recorder stopped ===");
        }
    }
}
