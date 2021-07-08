using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Bitstamp.DataPuller
{
    public static class Program
    {
        static ServiceBase[] _services;
        static void Main(string[] args)
        {
            _services = new ServiceBase[] { new MainService() };
            HandleUnhandledException();

            if (Environment.UserInteractive)
            {
                if (Debugger.IsAttached)
                    RunInteractiveServices(_services);
                else
                {
                    try
                    {
                        bool hasCommands = args.Length > 0;
                        for (int i = 0; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "install":
                                    ManagedInstallerClass.InstallHelper(new string[] { typeof(Program).Assembly.Location });
                                    break;
                                case "uninstall":
                                    ManagedInstallerClass.InstallHelper(new string[] { "/u", typeof(Program).Assembly.Location });
                                    break;
                                case "start":
                                    foreach (var service in _services)
                                    {
                                        ServiceController sc = new ServiceController(service.ServiceName);
                                        sc.Start();
                                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                    }
                                    break;
                                case "stop":
                                    foreach (var service in _services)
                                    {
                                        ServiceController sc = new ServiceController(service.ServiceName);
                                        sc.Stop();
                                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                                    }
                                    break;
                                default:
                                    hasCommands = false;
                                    break;
                            }
                        }
                        if (!hasCommands)
                        {
                            Console.WriteLine("Usage : {0} [command] [command ...]", AppDomain.CurrentDomain.FriendlyName);
                            Console.WriteLine("Commands : ");
                            Console.WriteLine(" - install : Install the services");
                            Console.WriteLine(" - uninstall : Uninstall the services");
                            Console.WriteLine(" - start : Starts the services");
                            Console.WriteLine(" - stop : Stops the services");
                        }
                    }
                    catch (Exception ex)
                    {
                        var oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error : {0}", ex.GetBaseException().Message);
                        Console.ForegroundColor = oldColor;
                    }
                }
            }
            else ServiceBase.Run(_services);
        }
        public static void HandleUnhandledException()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException((Exception)e.ExceptionObject);
        }
        static void HandleUnhandledException(Exception e)
        {
            Log.Instance.Fatal(e);
            CloseService();
        }
        static void RunInteractiveServices(ServiceBase[] servicesToRun)
        {
            Console.WriteLine();
            Console.WriteLine("Start the services in interactive mode.");
            Console.WriteLine();

            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Starting {0} ... ", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.WriteLine("Started");
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to stop services et finish process...");
            Console.ReadKey(true);
            Console.WriteLine();

            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Stopping {0} ... ", service.ServiceName);
                onStopMethod.Invoke(service, null);
                Console.WriteLine("Stopped");
            }

            Console.WriteLine();
            Console.WriteLine("All services are stopped.");
            if (Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.Write("=== Press a key to quit ===");
                Console.ReadKey(true);
            }
        }
        public static void CloseService()
        {
            Thread closeThread = new Thread(delegate ()
            {
                MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var service in _services)
                    onStopMethod.Invoke(service, null);
            });
            closeThread.Start();
        }
    }
}
