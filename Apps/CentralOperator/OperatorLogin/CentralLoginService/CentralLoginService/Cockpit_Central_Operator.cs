using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Threading;
using Dynamic.DataLayer;
using TPL.TagReaders;

namespace CentralLoginService
{
    public partial class Cockpit_Central_Operator : ServiceBase
    {

        private TagReaderServiceFramework tagReaderServiceFramework;
        private ServiceState currentState = ServiceState.Stopped;
        private SynchronizationContext uiSyncContext;


        private bool debugMode = false;

        public Cockpit_Central_Operator()
        {
            InitializeComponent();
            ServiceName = "Disney_Central_Operator";
            CanStop = true;
            CanPauseAndContinue = true;
            CanHandleSessionChangeEvent = true;
            debugMode = Properties.Settings.Default.DebugMode;
            tagReaderServiceFramework = new TagReaderServiceFramework(Properties.Settings.Default.JEGRConnection, Feedback);
            tagReaderServiceFramework.UseOperatorPermissions = Properties.Settings.Default.UseOperatorPermissions;
            SetState(currentState);
            uiSyncContext = SynchronizationContext.Current;
        }
        
        private void SetState(ServiceState State)
        {
            currentState = State;
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                //bug fix - set database path here - currently set in constructor (req reboot or re-install to fix bad path)
                //EventLog.WriteEntry("Cockpit_Central_Operator Service Starting " + Properties.Settings.Default.JEGRConnection);
                debugMode = Properties.Settings.Default.DebugMode;
                tagReaderServiceFramework.DebugMode = debugMode;
                tagReaderServiceFramework.DBDebugMode = debugMode;
                tagReaderServiceFramework.AutoLogout = Properties.Settings.Default.AutoLogout;
                tagReaderServiceFramework.AutoLogoutTimes = Properties.Settings.Default.AutoLogoutTime;
                SetState(tagReaderServiceFramework.Start());
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Cockpit_Central_Operator Service Startup Exception : " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Cockpit_Central_Operator Service Stopping");
            SetState(tagReaderServiceFramework.Stop());
        }

        protected override void OnPause()
        {
            EventLog.WriteEntry("Cockpit_Central_Operator Service Pausing");
            SetState(tagReaderServiceFramework.Pause());
        }

        protected override void OnContinue()
        {
            EventLog.WriteEntry("Cockpit_Central_Operator Service Resuming");
            SetState(tagReaderServiceFramework.Resume());
        }

        public void Feedback(string aString)
        {
            try
            {
                if (aString.Contains("Exception"))
                    EventLog.WriteEntry(aString, System.Diagnostics.EventLogEntryType.Error);
                else
                    EventLog.WriteEntry(aString, System.Diagnostics.EventLogEntryType.Information);
                //tagReaderServiceFramework.DBFeedback(aString);
            }
            catch (Exception ex)
            { 
                EventLog.WriteEntry(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
