using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace OperatorLogin
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstance.Make();

            base.OnStartup(e);
        }
    }

    public static class SingleInstance
    {

        internal static void Make() // Single instance per machine
        {
            Make(SingleInstanceModes.PerSession);
        }

        internal static void Make(SingleInstanceModes singleInstanceModes) //single instance per session
        {
            var appName = Application.Current.GetType().Assembly.ManifestModule.ScopeName;

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var keyUserName = windowsIdentity != null ? windowsIdentity.User.ToString() : String.Empty;

            var eventWaitHandleName = string.Format("{0}{1}", appName,
                singleInstanceModes == SingleInstanceModes.PerSession ? keyUserName : String.Empty
                );

            try
            {
                using (var eventWaitHandle = EventWaitHandle.OpenExisting(eventWaitHandleName))
                {
                    //Inform first instance.
                    eventWaitHandle.Set();
                }

                // And close...
                Environment.Exit(0);
            }
            catch
            {
                // It's first instance.
                // Register EventWaitHandle.
                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventWaitHandleName))
                {
                    ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, OtherInstanceAttemptedToStart, null, Timeout.Infinite, false);
                }

                RemoveApplicationsStartupDeadlock();
            }
        }

        private static void OtherInstanceAttemptedToStart(Object state, Boolean timedOut)
        {
            RemoveApplicationsStartupDeadlock();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Application.Current.MainWindow.Activate();
                }
                catch { }
            }));
        }

        internal static DispatcherTimer AutoExitApp;

        public static void RemoveApplicationsStartupDeadlock()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AutoExitApp = new DispatcherTimer(TimeSpan.FromSeconds(6), DispatcherPriority.ApplicationIdle,
                        (o, args) =>
                        {
                            if (Application.Current.Windows.Cast<Window>().Count(window => !Double.IsNaN(window.Left)) == 0)
                            {
                                Environment.Exit(0);
                            }
                        },
                        Application.Current.Dispatcher
                    );
            }), DispatcherPriority.ApplicationIdle);
        }
    }

    public enum SingleInstanceModes
    {
        PerMachine = 0,
        PerSession,
    }
}
