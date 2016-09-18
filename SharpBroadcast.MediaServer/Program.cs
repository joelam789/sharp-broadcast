using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SharpBroadcast.MediaServer
{
    public static class Program
    {
        public static string SVC_NAME = "SharpBroadcastMediaServer";

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CommonLog.Error(((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            // start the program ...

            if (Environment.UserInteractive)
            {
                //string parameter = string.Concat(args);

                string parameter = "";

                if (args != null && args.Length >= 2)
                {
                    parameter = args[0];
                    parameter = parameter.Trim();

                    if (parameter == "install" || parameter == "uninstall")
                    {

                        string[] svcNameArgs = new string[args.Length - 1];
                        for (int i = 1; i < args.Length; i++) svcNameArgs[i - 1] = args[i];

                        string svcName = "";
                        for (int i = 0; i < svcNameArgs.Length; i++)
                        {
                            if (svcName.Length == 0) svcName = svcNameArgs[i];
                            else svcName = svcName + " " + svcNameArgs[i];
                        }

                        svcName = svcName.Trim();
                        if (svcName.Length > 0) SVC_NAME = svcName;
                    }
                    else parameter = "";
                }

                parameter = parameter.Trim();
                if (parameter == "install" && SVC_NAME.Length > 0)
                {
                    CommonLog.Info("Install service ...");
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        CommonLog.Info("OK");
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Error: " + ex.Message);
                        return -1;
                    }
                }
                else if (parameter == "uninstall" && SVC_NAME.Length > 0)
                {
                    CommonLog.Info("Uninstall service ...");
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        CommonLog.Info("OK");
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Error: " + ex.Message);
                        return -1;
                    }
                }
                else // Run as a console app normally ...
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                    Environment.Exit(0);
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
				{ 
					new MediaService() 
				};
                ServiceBase.Run(ServicesToRun);
            }

            return 0;
        }
    }
}
