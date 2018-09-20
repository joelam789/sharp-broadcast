using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SharpBroadcast.BroadcastProxy
{
    partial class ProxyService : ServiceBase
    {
        Proxy m_Proxy = new Proxy();

        public ProxyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            CommonLog.Info("=== " + Program.SVC_NAME + " is starting ===");

            m_Proxy.Start();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.

            m_Proxy.Stop();

            CommonLog.Info("=== " + Program.SVC_NAME + " stopped ===");
        }
    }
}
