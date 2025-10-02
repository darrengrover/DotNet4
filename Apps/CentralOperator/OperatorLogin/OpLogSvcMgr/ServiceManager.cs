using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Dynamic.DataLayer;

namespace OpLogSvcMgr
{
    public delegate void FeedbackLogDelegate(string feedback, string feedbackType, int level);

    public class ServiceManager : IDisposable
    {
        private SqlDataAccess da;

        public string ServiceName = "Cockpit Public Push";

        private string siteName = @"kbro";

        private string connectionString = @"";

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        private bool debugMode = false;

        public bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }

        private System.Timers.Timer timer;

        private Boolean disposed;
        private Boolean isBusy;
        bool isDBSetup = false;

        private int feedbackDetail = 2;

        public int FeedbackDetail
        {
            get { return feedbackDetail; }
            set { feedbackDetail = value; }
        }

        public FeedbackLogDelegate FeedbackLog;

        public ServiceManager()
        {
            disposed = false;
            isBusy = false;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(Boolean disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    FeedbackLog = null;
                }
                disposed = true;
            }
        }
        
        #endregion

        private void setupConnection()
        {
            da = SqlDataAccess.Singleton;
            da.DBConnections.Clear(); //in case this is not the first time!
            DBConnection dBConnection = new DBConnection();
            dBConnection.DBName = "JEGR_DB";
            dBConnection.ConnectionString = connectionString;
            dBConnection.SiteName = siteName;
            da.DBConnections.Add(dBConnection);
            da.SiteName = siteName;
        }

        public void Start()
        {
            try
            {
                double interval = 60000;
                FeedbackTypeLevel(ServiceName + " Service Starting", "Information", 0);
                setupConnection();
                timer = new System.Timers.Timer();
                timer.Interval = interval;
                timer.AutoReset = true;
                timer.Elapsed += new ElapsedEventHandler(DoTiming);
                timer.Start();
            }
            catch (Exception ex)
            {
                FeedbackTypeLevel(ServiceName + " Service Startup Exception : " + ex.Message, "Error", 0);
            }
        }

        private void disposeConnection()
        {
            da.DBConnections.Clear();
            da = null;
        }

        public Boolean Stop()
        {
            Boolean haveStopped = false;
            if (!isBusy)
            {
                FeedbackTypeLevel(ServiceName + " Service Stopping", "Information", 0);
                timer.Stop();
                timer.Dispose();
                timer = null;
                disposeConnection();
                FeedbackTypeLevel(ServiceName + " Service Stopped", "Information", 0);
                haveStopped = true;
            }
            else
            {
                FeedbackTypeLevel(ServiceName + " Service Busy, not stopped", "Warning", 0);
            }
            return haveStopped;
        }

        public void Pause()
        {
            timer.Stop();
            FeedbackTypeLevel(ServiceName + " Service Pausing", "Information", 0);
        }

        public void Resume()
        {
            timer.Start();
            FeedbackTypeLevel(ServiceName + " Service Resuming", "Information", 0);
        }

        public void Feedback(string aString)
        {
            if (FeedbackLog != null)
            {
                FeedbackLog(aString, "Information", 0);
            }
        }

        public void FeedbackTypeLevel(string aString, string feedbacktype, int level)
        {
            if (FeedbackLog != null)
                FeedbackLog(aString, feedbacktype, level);
        }

        private void DataStart()
        {
            isDBSetup = true;
        }

        private void DoTiming(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!disposed && !isBusy)
                {
                    isBusy = true;
                    timer.Stop();
                    if (!isDBSetup)
                        DataStart();
                    if (isDBSetup)
                    {
                        //do stuff!
                        DateTime nowTime = DateTime.Now;
                        DateTime today = DateTime.Today;

                        int minute = nowTime.Minute;
                    }
                }
            }
            catch (Exception ex)
            {
                FeedbackTypeLevel(ServiceName + " Service, Exception at:" + e.SignalTime + " Message: "
                    + ex.Message, "Error", 0);
            }
            finally
            {
                isBusy = false;
                timer.Start();
            }
        }


    }
}
