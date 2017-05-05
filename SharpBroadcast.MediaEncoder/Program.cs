using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SharpBroadcast.MediaEncoder
{
    static class Program
    {
        static Mutex mutex = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("KeepOnlyOneInstanceRunning"))
            {
                if (appSettings["KeepOnlyOneInstanceRunning"].ToString().ToLower() == "true")
                {
                    bool canRun = false;
                    mutex = new Mutex(true, "SharpBroadcast.MediaEncoder", out canRun);

                    if (!canRun)
                    {
                        MessageBox.Show("Another encoder application instance is already running.");
                        return;
                    }
                }
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex = null;
                }
            }
        }
    }
}
