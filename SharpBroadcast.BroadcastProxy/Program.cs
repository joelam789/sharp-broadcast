using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SharpBroadcast.BroadcastProxy
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private static Mutex mutex = null;

        public static string SVC_NAME = "BroadcastProxyServer";
        public static string SVC_DESC = "Single direction broadcasting";

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
                string parameter = "";
                bool needDetails = false;

                if (args != null && args.Length >= 1)
                {
                    parameter = args[0];
                    parameter = parameter.Trim();

                    if (parameter == "install" || parameter == "uninstall"
                        || parameter == "silent-install" || parameter == "silent-uninstall")
                    {
                        if (parameter == "silent-install" || parameter == "silent-uninstall")
                        {
                            if (parameter == "silent-install") parameter = "install";
                            else if (parameter == "silent-uninstall") parameter = "uninstall";
                        }
                        else
                        {
                            // redirect console output to parent process, normally it should be "cmd"
                            AttachConsole(ATTACH_PARENT_PROCESS);
                            needDetails = true;
                        }

                        string svcName = "";

                        if (args.Length >= 2)
                        {
                            string[] svcNameArgs = new string[args.Length - 1];
                            for (int i = 1; i < args.Length; i++) svcNameArgs[i - 1] = args[i];

                            for (int i = 0; i < svcNameArgs.Length; i++)
                            {
                                if (svcName.Length == 0) svcName = svcNameArgs[i];
                                else svcName = svcName + " " + svcNameArgs[i];
                            }

                            svcName = svcName.Trim();
                        }

                        if (svcName.Length > 0) SVC_NAME = svcName;
                    }
                    else parameter = "";
                }

                parameter = parameter.Trim();
                if (parameter == "install" && SVC_NAME.Length > 0)
                {
                    Console.WriteLine("Start to install service with name [" + SVC_NAME + "]");
                    CommonLog.Info("Installing service...");
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        CommonLog.Info("OK");
                        Console.WriteLine("Installed service [" + SVC_NAME + "] successfully");
                        if (needDetails) Console.WriteLine("You might need to press enter to end the process");
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Error: " + ex.Message);
                        Console.WriteLine("Failed to install service [" + SVC_NAME + "]");
                        if (needDetails) Console.WriteLine("You might need to press enter to end the process");
                        return -1;
                    }
                }
                else if (parameter == "uninstall" && SVC_NAME.Length > 0)
                {
                    Console.WriteLine("Start to uninstall service [" + SVC_NAME + "]");
                    CommonLog.Info("Uninstalling service...");
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        CommonLog.Info("OK");
                        Console.WriteLine("Uninstalled service [" + SVC_NAME + "] successfully");
                        if (needDetails) Console.WriteLine("You might need to press enter to end the process");
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Error: " + ex.Message);
                        //Console.WriteLine("Failed to uninstall service [" + SVC_NAME + "]");
                        if (needDetails) Console.WriteLine("You might need to press enter to end the process");
                        return -1;
                    }
                }
                else // Run as a console app normally ...
                {
                    bool canRun = false;
                    mutex = new Mutex(true, "BroadcastProxyServer.Instance", out canRun);

                    if (!canRun)
                    {
                        MessageBox.Show("Another application instance is already running.");
                    }
                    else
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainForm());
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                bool canRun = false;
                mutex = new Mutex(true, "BroadcastProxyServer.Instance", out canRun);

                if (!canRun)
                {
                    throw new Exception("Another application instance is already running.");
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
				    {
					    new ProxyService()
				    };
                    ServiceBase.Run(ServicesToRun);
                }
            }

            return 0;
        }
    }
}
