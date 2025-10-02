using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Dynamic.DataLayer;
using MVVMHelpers;

namespace TPL.TagReaders
{
    public class TagReaderServiceFramework
    {
        private TPLTagReaders tagReaders;

        public TPLTagReaders TagReaders
        {
            get { return tagReaders; }
            set { tagReaders = value; }
        }
        private SqlDataAccess da;
        //private int timercount = 0;
        private ServiceState currentState;
        private System.Timers.Timer timer;
        private bool isBusy = false;
        //private int datareadminute = 0;

        public delegate void FeedbackDelegate(string feedback);
        public FeedbackDelegate TextFeedback = null;
        public Boolean DebugMode;
        public Boolean DBDebugMode;
        private FeedbackRecs feedbackRecs;
        private DateTime lastForcedLogout = DateTime.MinValue;
        private DateTime logoutTime = DateTime.Today.AddHours(23).AddMinutes(30);
        private List<DateTime> logoutTimes = new List<DateTime>();
        
        public string AutoLogoutTimes
        {
            get
            {
                return string.Join("|", logoutTimes.Select(d => d.ToShortTimeString()));
            }
            set
            {
                logoutTimes = new List<DateTime>();
                if (!string.IsNullOrEmpty(value))
                {
                    string[] times = value.Split('|');

                    foreach (string time in times)
                    {
                        // hh:mm
                        string[] t = time.Split(':');
                        if (t.Count() >= 2)
                        {
                            int h = 23;
                            if (t[0].Length > 2) t[0] = t[0].Substring(t[0].Length - 2, 2);
                            bool test = Int32.TryParse(t[0], out h);
                            if (test)
                            {
                                int m = 30;
                                test = Int32.TryParse(t[1], out m);
                                if (test)
                                {
                                    logoutTimes.Add(DateTime.Today.AddHours(h).AddMinutes(m));
                                }
                            }
                        }
                    }
                    if (logoutTimes.Count > 0)
                        logoutTimes.Sort();
                }
            }
        }
        private bool autoLogout = false;

        public bool AutoLogout
        {
            get { return autoLogout; }
            set { autoLogout = value; }
        }
        private bool useOperatorPermissions = false;

        public bool UseOperatorPermissions
        {
            get { return useOperatorPermissions; }
            set 
            { 
                useOperatorPermissions = value;
                if (tagReaders != null)
                    tagReaders.UseOperatorPermissions = value;
            }
        }
        public TagReaderServiceFramework(string connectionString, FeedbackDelegate feedback)
        {
            da = SqlDataAccess.Singleton;
            DBConnection dBConnection = new DBConnection();            
            dBConnection.DBName = "JEGR_DB";
            dBConnection.ConnectionString = connectionString;
            dBConnection.SiteName = "default";
            da.DBConnections.Add(dBConnection);
            da.SiteName = "default";            
            
            TextFeedback = feedback;
            tagReaders = new TPLTagReaders();
            tagReaders.TextFeedback = Feedback;
            feedbackRecs = new FeedbackRecs();
            currentState = ServiceState.Stopped;
            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(DoTiming);
        }

        public ServiceState Start()
        {
            try
            {
                Feedback("Cockpit_Central_Operator Service Starting");
                tagReaders.DebugMode = DebugMode;
                tagReaders.Start();
                timer.Start();
                currentState = ServiceState.Running;
                if (autoLogout)
                {
                    Feedback("Autologout enabled, set for: " + AutoLogoutTimes + Environment.NewLine);
                }
                else
                {
                    Feedback("Autologout disabled");
                }
                Feedback("Current logins = " + tagReaders.EvalCurrentLogins() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Feedback("Exception: Cockpit_Central_Operator Service Startup Exception, at:" + DateTime.Now + " Message: " + ex.Message);

            } 
            return currentState;
        }

        public ServiceState Stop()
        {
            if (timer != null)
            {
                Feedback("Cockpit Central Operator Service Stopping");
                timer.Stop();
                tagReaders.Stop();
                currentState = ServiceState.Stopped;
            }
            return currentState;
        }

        public ServiceState Pause()
        {
            if (timer != null)
            {
                timer.Stop();
                currentState = ServiceState.Paused;
            }
            return currentState;
        }

        public ServiceState Resume()
        {
            if (timer != null)
            {
                timer.Start();
                currentState = ServiceState.Running;
            }
            return currentState;
        }

        public void Ping()
        {
            Feedback("Pinging Tag Readers");
            tagReaders.PingReaders();
            Feedback("Finished Pinging");
        }

        public void Feedback(string info)
        {
            if (TextFeedback != null)
                TextFeedback(info);
            else
                Debug.WriteLine(info);
        }

        private int updateMinute = -1;
        private int autoLogoutMin = -1;
        private DateTime _clearedDueToRetentionLimit = DateTime.MinValue;
        private bool evalLogoutTime()
        {
            bool test = false;
            foreach (DateTime d in logoutTimes)
            {
                if ((DateTime.Now.Hour == d.Hour) && (DateTime.Now.Minute >= d.Minute) && (DateTime.Now.Minute <= (d.Minute + 5)))
                {
                    test = (lastForcedLogout < DateTime.Today.AddHours(d.Hour).AddMinutes(d.Minute));
                    if (test)
                    {
                        test = tagReaders.OperatorStates.Count > 0;
                        if (!test)
                        {
                            Feedback("Autologout failed - No operator states data " + DateTime.Now);
                        }
                        else
                        {
                            logoutTime = DateTime.Today.AddHours(d.Hour).AddMinutes(d.Minute);
                            break;
                        }
                    }
                }
            }
            return test;
        }
        private DateTime lastlogintotal = DateTime.MinValue;

        private bool evalLastLogoutTime()
        {
            bool test = false;
            TimeSpan ts = DateTime.Now.Subtract(lastlogintotal);
            test = (ts.TotalMinutes > 14);
            return test;
        }
        private bool evalRetentionClearDateTime()
        {
            bool test = false;
            TimeSpan ts = DateTime.Now.Subtract(_clearedDueToRetentionLimit);
            test = (ts.TotalMinutes > 29);
            return test;
        }
        private void DoTiming(object sender, ElapsedEventArgs e)
        {
            if (!isBusy)
            {
                isBusy = true;
                try
                {
                    tagReaders.ReloadData(true);
                    tagReaders.Read();

                    int min = DateTime.Now.Minute;
                    if (AutoLogout && min != autoLogoutMin && evalLogoutTime())
                    {
                        autoLogoutMin = min; // only check once a minute
                        int logouts;
                        Feedback("Autologout Commencing");
                        logouts = tagReaders.AutoLogoutNow();
                        lastForcedLogout = DateTime.Today.AddHours(logoutTime.Hour).AddMinutes(logoutTime.Minute);
                        Feedback("Autologout Completed");
                    }
                    if (min % 15 == 0) // every 15 minutes??
                    {
                        if (evalLastLogoutTime())
                        {
                            Feedback("Current logins = " + tagReaders.EvalCurrentLogins() + Environment.NewLine);
                            lastlogintotal = DateTime.Now;
                        }
                    }
                    if (min != updateMinute)
                    {
                        updateMinute = min;// only do once a minute
                        if (DBDebugMode) StoreFeedback();
                        tagReaders.SetUpdateAllLEDsNow();
                    }                    
                    if (min % 30 == 0 && evalRetentionClearDateTime()) // every 30 minutes but only once in that minute
                    {
                        tagReaders.clearExcessData();
                        _clearedDueToRetentionLimit = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    Feedback("Exception: Cockpit_Central_Operator Service, at:" + DateTime.Now + " Message: " + ex.Message);
                }
                finally
                {
                    isBusy = false;
                }
            }
        }

        public void DBFeedback(string aString)
        {
            try
            {
                if (DBDebugMode)
                {
                    //EventLog.WriteEntry(aString, System.Diagnostics.EventLogEntryType.Information);
                    if (feedbackRecs != null)
                    {
                        //SqlDataAccess da = SqlDataAccess.Singleton;
                        FeedbackRec rec = new FeedbackRec();
                        rec.Recnum = -1;
                        rec.ForceNew = true;
                        rec.LogTime = DateTime.Now;
                        rec.AppName = "Cockpit Central Operator";
                        rec.Feedback = aString;
                        feedbackRecs.Add(rec);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void StoreFeedback()
        {
            feedbackRecs.UpdateToDB();
            feedbackRecs.Clear();
        }


    }

    public class TPLTagReaders
    {
        SqlDataAccess da;
        public RfIdeasTagReaders TcpReaders;
        public delegate void FeedbackDelegate(string feedback);
        public FeedbackDelegate TextFeedback = null;
        public Tags TagsRfid;
        private Tags tags;
        private Operators operatorsRfid;
        public OperatorLoginStates OperatorStates;        
        private RemoteOperatorLogsGroupedByOperator remoteOperatorLogs;
        private JGLogData logData;
        private MachineSubIDs machineSubIDs;
        private TagReaderLocations tagReaderLocs;
        private TagReaderLog tagReaderLog;
        private Permissions availablePermissionsFromDb;
        private OperatorPermissions operatorPermissionsFromDb;
        public bool DebugMode { get; set; }
        public bool UseOperatorPermissions { get; set; }

        public TPLTagReaders()
        {
            
            TcpReaders = new RfIdeasTagReaders();
            DebugMode = false;
            OperatorStates = new OperatorLoginStates();
            OperatorStates.StatesFeedback = StatesFeedback;
            operatorsRfid = new Operators();
            TagsRfid = new Tags();
            tags = new Tags();
            remoteOperatorLogs = new RemoteOperatorLogsGroupedByOperator();
            remoteOperatorLogs.UpdateFieldNames = new List<string>();
            remoteOperatorLogs.UpdateFieldNames.Add("Operator_Remote"); 
            logData = new JGLogData();
            machineSubIDs = new MachineSubIDs();
            tagReaderLog = new TagReaderLog();
            availablePermissionsFromDb = new Permissions();
            operatorPermissionsFromDb = new OperatorPermissions();
            da = SqlDataAccess.Singleton;
            GetData();        
        }
        public void StatesFeedback(string info)
        {
            if ((TextFeedback != null) && DebugMode)
                TextFeedback(info);
            else
                Debug.WriteLine(info);
        }
        public void GetData()
        {
            getReadersData();
            ReloadData(true);
        }

        private void getReadersData()
        {
            if (tagReaderLocs == null)
                tagReaderLocs = new TagReaderLocations();
            tagReaderLocs.RaiseException = true;
            tagReaderLocs = (TagReaderLocations)da.DBDataListSelect(tagReaderLocs, false, true);
            if (tagReaderLocs.UpdatedFromDB)
            {
                TcpReaders.Clear();
                foreach (TagReaderLocation tr in tagReaderLocs)
                {
                    addTcpReader(tr.LocationID, tr.LocationIP, tr.TcpPort, tr.MachineID, tr.SubID, tr.MaxLogins);
                }
                TcpReaders.Reset();
            }
            if (tagReaderLocs.Count == 0)
            {
                feedback(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + " TagReaderLocations Count = 0. UpdatedFromDB = " + tagReaderLocs.UpdatedFromDB);                
            }
        }

        private void addTcpReader(int locationid, string ip, int port, int machine, int sub, int maxlogins)
        {
            RfIdeasTcpReader tcpReader = new RfIdeasTcpReader(locationid, ip, port, machine, sub, maxlogins, DebugMode);
            TcpReaders.Add(tcpReader);
        }
        private void getOperatorsData(bool useCache)
        {
            operatorsRfid.RaiseException = true;
            operatorsRfid = da.GetOperators(operatorsRfid, !useCache);
            getOperatorPermissions(false);
            if (operatorsRfid.UpdatedFromDB)
            {               
                OperatorStates.Clear();
                foreach (Operator op in operatorsRfid)
                {
                    addOperatorState(op.IdJensen, -1, -1, DateTime.MinValue, DateTime.MinValue, false);
                }
                operatorsRfid.Reset();
                OperatorStates.Reset();
            }
            if (operatorsRfid.Count == 0)
            {
                feedback(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + " Operator Count = 0. UpdatedFromDB = " + operatorsRfid.UpdatedFromDB);
                operatorsRfid.IsValid = false;
            }
        }
        private void getOperatorPermissions(bool useCache)
        {
            if (UseOperatorPermissions)
            {
                availablePermissionsFromDb = da.GetPermissions(availablePermissionsFromDb, !useCache);
                operatorPermissionsFromDb = da.GetOperatorPermissions(operatorPermissionsFromDb, !useCache);
                foreach (Operator op in operatorsRfid)
                {
                    op.PermissionsAvailableFromDb = availablePermissionsFromDb;
                    op.OperatorPermissionsFromDb = operatorPermissionsFromDb;
                }
            }
        }
        private void addOperatorState(int operatorid, int machineid, int subid, DateTime login, DateTime logout, bool isloggedin)
        {
            OperatorLoginState opState = new OperatorLoginState(operatorid, machineid, subid, login, logout, isloggedin, remoteOperatorLogs, logData, machineSubIDs);
            OperatorStates.Add(opState);
        }

        private void getTags(bool useCache)
        {
            tags.RaiseException = true;
            tags = da.GetAllTags(tags, !useCache);
            if (tags.UpdatedFromDB)
            {
                TagsRfid.Clear();
                foreach (Tag tag in tags)
                {
                    if (tag.TagReferenceTable == "tblOperators")
                    {
                        addTag(tag.ReferenceID, tag.TagData);
                    }
                }
                TagsRfid.Reset();
            }
            //if (TagsRfid.Count == 0)
            //{
            //    TextFeedback(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + " TagsRfid Count = 0. UpdatedFromDB = " + operatorsRfid.UpdatedFromDB);
            //}
        }

        private void addTag(int id, string data)
        {
            Tag tag = new Tag();
            tag.ID = id - 1;
            tag.TagData = data;
            tag.TagReferenceTable = "tblOperators";
            tag.ReferenceID = id;
            TagsRfid.Add(tag);
        }
        private void PrintToFile(string path, string msg)
        {
            if (DebugMode)
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(DateTime.Now + "| " + msg + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    feedback(ex.Message + Environment.NewLine);
                }
            }
        }
        private void processRemoteOperatorLog()
        {
            if (remoteOperatorLogs.UpdatedFromDB || operatorsRfid.UpdatedFromDB)
            {
                // remoteOperatorLogs is ordered by RemoteOperatorLogId
                foreach (RemoteOperatorLog log in remoteOperatorLogs)
                {
                    PrintToFile("processRemoteOperatorLog.log", "LogTime: " + log.LogTime + ", RemoteId: " + log.RemoteOperatorLogID + ", State: " + log.State + ", SubregType: " + log.SubregType + ", OperartorId: " + log.OperatorID + ", MachineId: " + log.MachineID + ", SubId: " + log.SubID);
                    bool isLoggedIn = (log.State == 1) && (log.SubregType == 0);
                    DateTime loginTime = DateTime.MinValue;
                    DateTime logoutTime = DateTime.MaxValue;
                    bool isClockInOut = (log.SubregType == 5);
                    bool isBreak = (log.SubregType == 6);
                    if (isLoggedIn)
                        loginTime = log.LogTime;
                    else
                        logoutTime = log.LogTime;
                    OperatorLoginState opState = (OperatorLoginState)OperatorStates.GetByOperatorID(log.OperatorID);
                    if (opState == null) //shouldn't happen
                    {
                        PrintToFile("processRemoteOperatorLog.log", "opState not found so creating");
                        opState = new OperatorLoginState(log.OperatorID, log.MachineID, log.SubID, loginTime, logoutTime, isLoggedIn, remoteOperatorLogs, logData, machineSubIDs);
                        OperatorStates.Add(opState);
                    }

                    if (log.SubregType == 0)
                    {
                        opState.ClockedIn = true;
                        opState.MachineID = log.MachineID;
                        opState.SubID = log.SubID;
                        if (isLoggedIn)
                            opState.LoginTime = loginTime;
                        else
                            opState.LogoutTime = logoutTime;
                        opState.IsLoggedIn = isLoggedIn;

                        PrintToFile("processRemoteOperatorLog.log", "SubregType==0: IsLoggedIn==" + isLoggedIn);
                    }
                    if (isClockInOut)
                    {
                        opState.ClockedIn = (log.State == 1);
                        opState.MachineID = -1;
                        opState.SubID = -1;
                        PrintToFile("processRemoteOperatorLog.log", "IsClockedIn==" + opState.ClockedIn);
                    }
                    if (log.SubregType == 6)
                    {
                        opState.ClockedIn = true;
                        opState.MachineID = -1;
                        opState.SubID = -1;
                        if (log.State == 1)
                        {
                            opState.BreakID = log.SubregTypeID;
                        }
                        else
                        {
                            opState.BreakID = -1;
                        }
                        PrintToFile("processRemoteOperatorLog.log", "SubregType==6: BreakId==" + opState.BreakID);
                    }

                }
                SetUpdateAllLEDsNow();
                //DebugOperatorState();
            }
        }
        private void DebugOperatorState()
        {
            foreach (var opstate in OperatorStates)
            {
                PrintToFile("operatorStates.log", "OperatorId: " + opstate.OperatorID + ", MachineId: " + opstate.MachineID + ", SubId: " + opstate.SubID + ", LoginTime: " + opstate.LoginTime + ", LogoutTime: " + opstate.LogoutTime + ", IsClockedIn: " + opstate.ClockedIn + ", IsLoggedIn: " + opstate.IsLoggedIn + ", BreakId: " + opstate.BreakID);
            }
        }
        private void getMachineSubids(bool useCache)
        {
            machineSubIDs.RaiseException = true;
            machineSubIDs = da.GetMachineSubIDs(machineSubIDs, !useCache);
            if (machineSubIDs.Count == 0)
            {
                feedback(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + " Machine Subids = 0. UpdatedFromDB = " + machineSubIDs.UpdatedFromDB);
                machineSubIDs.IsValid = false;
            }
        }

        public void SetUpdateAllLEDsNow()
        {
            foreach (RfIdeasTcpReader tcpReader in TcpReaders)
            {
                tcpReader.UpdateLEDNow = true;
                PrintToFile("ReaderLEDState.log", "Reader=" + tcpReader.IpAddress + ", LED=" + tcpReader.CurrentLED);
            }
        }

        private void getRemoteOperatorLog(bool useCache)
        {
            remoteOperatorLogs.RaiseException = true;
            remoteOperatorLogs.ClearSelectRestrictions();
            remoteOperatorLogs.AddSelectRestriction(new RestrictionRec("LogTime", DateTime.Today.AddDays(-1), ComparisonType.MoreThanEqual));
            remoteOperatorLogs = (RemoteOperatorLogsGroupedByOperator)da.DBDataListSelect(remoteOperatorLogs, !useCache, useCache);
            remoteOperatorLogs.UpdateFieldNames = new List<string>();
            remoteOperatorLogs.UpdateFieldNames.Add("Operator_Remote");
        }

        public void ReloadData(bool useCache)
        {
            try
            {
                getOperatorsData(useCache);
            }
            catch (Exception ex)
            {
                debugInfo(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
            try
            {
                getMachineSubids(useCache);
            }
            catch (Exception ex)
            {
                debugInfo(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
            try
            {
                getRemoteOperatorLog(useCache);
            }
            catch (Exception ex)
            {
                debugInfo(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
            try
            {
                processRemoteOperatorLog();
            }
            catch (Exception ex)
            {
                debugInfo(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
            try
            {
                getTags(useCache);
            }
            catch (Exception ex)
            {
                debugInfo(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
        }

        public int EvalCurrentLogins()
        {
            int count = 0;
            foreach (OperatorLoginState ols in OperatorStates)
            {
                if (ols.IsLoggedIn)
                {
                    count++;
                }
            }
            return count;
        }

        public int AutoLogoutNow()
        {
            int count = EvalCurrentLogins();
            if (count > 0)
            {
                feedback("Attempting to log out " + count + " operators" + Environment.NewLine);
                foreach (OperatorLoginState ols in OperatorStates)
                {
                    if (ols.IsLoggedIn)
                    {
                        ols.Logout(DateTime.Now);
                    }
                    
                }
                EvalCurrentLogins();
            }
            
            clearExcessData();
            SetUpdateAllLEDsNow();

            return count;
        }

        private void feedback(string aString)
        {
            if (TextFeedback != null)
            {
                TextFeedback(aString);
            }
        }

        private void debugInfo(string aString)
        {
            if ((TextFeedback != null) && DebugMode)
            {
                PrintToFile("CentralOperator.log", aString);
                TextFeedback(aString + Environment.NewLine);
            }
        }

        public void Start()
        {
            ReloadData(false);
            InitialiseReaderConnections();
            SetUpdateAllLEDsNow();
        }

        public void InitialiseReaderConnections()
        {
            foreach (RfIdeasTcpReader tcpReader in TcpReaders)
            {
                OperatorLoginState opState = (OperatorLoginState)OperatorStates.GetByMachineSubID(tcpReader.MachineID,tcpReader.SubID);
                bool isloggedin = false;
                if (opState != null)
                    isloggedin = opState.IsLoggedIn;
                tcpReader.AllowFeedback = DebugMode;
                tcpReader.StartMonitorTask(isloggedin);
            }
        }

        public void PingReaders()
        {
            Ping ping = new Ping();
            PingReply reply;
            foreach (RfIdeasTcpReader tcpReader in TcpReaders)
            {
                reply = ping.Send(tcpReader.IpAddress);
                if (reply.Status == IPStatus.Success)
                    feedback("Ping " + tcpReader.IpAddress.ToString() + " Reply" + Environment.NewLine);
                else
                    feedback("Ping " + tcpReader.IpAddress.ToString() + " Fail" + Environment.NewLine);
            }
        }

        public void Read()
        {
            foreach (RfIdeasTcpReader tcpReader in TcpReaders)
            {
                try
                {
                    read(tcpReader);
                }
                catch (Exception ex)
                {
                    debugInfo("Error during read: " + ex.Message + Environment.NewLine);
                    feedback("Error during read: " + ex.Message + Environment.NewLine);
                }
            }
        }

        private void logCardRead(RfIdeasTcpReader tcpReader, string cardid)
        {
            TagReaderLogRec rec = new TagReaderLogRec();
            rec.LocationID = tcpReader.LocationID;
            rec.ScanTime = DateTime.Now;
            rec.TagData = cardid;
            rec.ForceNew = true;
            tagReaderLog.Add(rec);
            tagReaderLog.UpdateToDB();
        }
        private string TestHarness(int locationID, out int machineid, out int subid)
        {
            string cardId = string.Empty;
            machineid = 0;
            subid = 0;

            if (locationID == 1)
            {
                cardId = "7C339600";
                machineid = 2115;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "5CBB9500";
                machineid = 2;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "10679600";
                machineid = 2180;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "01F89500";
                machineid = 2173;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "4F6B9600";
                machineid = 2179;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "6B989400";
                machineid = 2174;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "3C6B9600";
                machineid = 2177;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "57669500";
                machineid = 2142;
                subid = 1;
            }
            if (locationID == 1)
            {
                cardId = "04049600";
                machineid = 2132;
                subid = 3;
            }
            if (locationID == 1)
            {
                cardId = "65619400";
                machineid = 2122;
                subid = 4;
            }
            if (locationID == 1)
            {
                cardId = "75089600";
                machineid = 2132;
                subid = 2;
            }
            if (locationID == 1)
            {
                cardId = "24B39500";
                machineid = 2122;
                subid = 3;
            }
            if (locationID == 1)
            {
                cardId = "1AF99500";
                machineid = 2142;
                subid = 2;
            }
            if (locationID == 1)
            {
                cardId = "AE0D9400";
                machineid = 2142;
                subid = 3;
            }
            if (locationID == 1)
            {
                cardId = "FF359600";
                machineid = 2115;
                subid = 5;
            }
            if (locationID == 1)
            {
                cardId = "B7309800";
                machineid = 2115;
                subid = 2;
            }
            return cardId;
        }
        public void read(RfIdeasTcpReader tcpReader)
        {
            try
            {
                int machineid = 0;
                int subid = 0;
                bool canLogin = true;
                string cardID = tcpReader.ReadCardID(out machineid, out subid);
                //string cardID = TestHarness(tcpReader.LocationID, out machineid, out subid);
                if (cardID != string.Empty)
                {
                    logCardRead(tcpReader, cardID);
                    //debugInfo(cardID + ", " + machineid.ToString() + ", " + subid.ToString() + Environment.NewLine);
                    Tag t = (Tag)TagsRfid.GetByName("tblOperators" + cardID);
                    if (t != null)
                    {
                        Operator op = (Operator)operatorsRfid.GetById(t.ReferenceID);
                        if (!op.Retired) // retired operators can't log in or out.
                        {
                            OperatorLoginState opState = (OperatorLoginState)OperatorStates.GetByOperatorID(t.ReferenceID);
                            if (opState != null)
                            {

                                tcpReader.HasBadRead = false;
                                if (opState.BreakID > 0)
                                {
                                    opState.BreakEnd(DateTime.Now, opState.BreakID);
                                    opState.BreakID = -1;
                                }
                                if (opState.IsLoggedIn) //this operator is logged in
                                {
                                    if ((opState.MachineID == machineid) && (opState.SubID == subid)) //this machine station?
                                    {
                                        if (!UseOperatorPermissions || op.IsAllowedToManuallyLogout)
                                        {
                                            //debugInfo("Just Logout " + machineid.ToString() + ", " + subid.ToString() + ", " + op.NameAndID + Environment.NewLine);
                                            opState.Logout(DateTime.Now);
                                            canLogin = false; //don't login again - simple logout
                                            tcpReader.RedLED();
                                            tcpReader.IsActiveStation = false;
                                        }
                                        else
                                        {
                                            canLogin = false; //don't login again - simple logout
                                            debugInfo("Operator Permission | Logout Disallowed | " + machineid.ToString() + ", " + subid.ToString());
                                            tcpReader.OrangeLED();
                                        }
                                    }
                                    else //no somewhere else
                                    {
                                        //debugInfo("Logout other location " + opState.MachineID.ToString() + ", " + opState.SubID.ToString() + ", "
                                            //+ op.NameAndID + Environment.NewLine);
                                        opState.Logout(DateTime.Now);
                                        foreach (RfIdeasTcpReader reader in TcpReaders)
                                        {
                                            if ((opState.MachineID == reader.MachineID)
                                                && (opState.SubID == reader.SubID))
                                            {
                                                reader.RedLED();
                                                reader.IsActiveStation = false;
                                            }
                                        }
                                    }
                                }
                                if (canLogin)
                                {
                                    if (tcpReader.MaxLogins == 1)  //if !allow multi then logout
                                    {
                                        OperatorLoginState otherOperator = (OperatorLoginState)OperatorStates.GetLoggedInByMachineSubID(machineid, subid);
                                        if (otherOperator != null) //another operator is logged in here!
                                        {
                                            //debugInfo("Logout Other Operator " + machineid.ToString() + ", " + subid.ToString() + ", "
                                                //+ otherOperator.OperatorID.ToString() + Environment.NewLine);
                                            otherOperator.Logout(DateTime.Now); //log them out
                                        }
                                    }
                                    if (OperatorStates.TotalLoggedInByMachineSubID(machineid, subid) < tcpReader.MaxLogins)
                                    {
                                        //debugInfo("Login " + machineid.ToString() + ", " + subid.ToString() + ", " + op.NameAndID + Environment.NewLine);
                                        opState.Login(machineid, subid, DateTime.Now);//login this user
                                        tcpReader.GreenLED();
                                        tcpReader.IsActiveStation = true;
                                    }
                                }
                                else
                                {
                                    // if there are still operators logged on to this station - make the light green again.
                                    if (OperatorStates.TotalLoggedInByMachineSubID(machineid, subid) > 0)
                                    {
                                        tcpReader.GreenLED();
                                        tcpReader.IsActiveStation = true;
                                    }
                                }
                                EvalCurrentLogins();
                            }

                        }
                        else
                        {
                            //operator is retired  
                            debugInfo("Retired | Bad Read | " + machineid.ToString() + ", " + subid.ToString() + ", " + op.NameAndID + Environment.NewLine);
                            tcpReader.OrangeLED();
                            tcpReader.HasBadRead = true;
                            tcpReader.BadReadTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        //invalid tag
                        debugInfo("Invalid | Bad Read | " + machineid.ToString() + ", " + subid.ToString());
                        tcpReader.OrangeLED();
                        tcpReader.HasBadRead = true;
                        tcpReader.BadReadTime = DateTime.Now;
                    }
                }
                else
                {
                    if (tcpReader.HasBadRead)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(tcpReader.BadReadTime);
                        if (ts.TotalMilliseconds > 1000)
                        {
                            debugInfo("Bad Read");
                            OperatorLoginState opState = (OperatorLoginState)OperatorStates.GetLoggedInByMachineSubID(tcpReader.MachineID, tcpReader.SubID);
                            Boolean isloggedin = false;
                            if (opState != null)
                                isloggedin = opState.IsLoggedIn;
                            if (isloggedin)
                                tcpReader.GreenLED();
                            else
                                tcpReader.RedLED();
                            tcpReader.HasBadRead = false;
                            tcpReader.IsActiveStation = isloggedin;
                        }
                    }
                }
                if (tcpReader.UpdateLEDNow)
                {
                    //debugInfo("Update led now"+Environment.NewLine);
                    OperatorLoginState opState = (OperatorLoginState)OperatorStates.GetLoggedInByMachineSubID(tcpReader.MachineID, tcpReader.SubID);
                    if (opState != null)
                    {
                        //Debug.WriteLine("update all leds now " + tcpReader.MachineID + ", " + tcpReader.SubID + " Operator " + opState.OperatorID + " loggedin " + opState.IsLoggedIn);
                        if (opState.IsLoggedIn)
                        {
                            tcpReader.GreenLED();
                        }
                        else
                        {
                            tcpReader.RedLED();
                        }
                    }
                    else
                        tcpReader.RedLED();
                    tcpReader.UpdateLEDNow = false;
                }
                string fb;
                while (tcpReader.IsFeedBack)
                {
                    fb = tcpReader.ReadFeedback();
                    if (fb != string.Empty)
                        feedback(fb + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                tcpReader.OrangeLED();
                debugInfo("Error in read cycle: " + ex.Message + Environment.NewLine);
            }
        }

        public void Stop()
        {
            foreach (RfIdeasTcpReader tcpReader in TcpReaders)
            {
                tcpReader.Disconnect();
            }
        }

        public void clearExcessData()
        {
            try
            {
                logData.Clear();
                tagReaderLog.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
        }

        public string GetInfo(string key)
        {
            String aString = string.Empty;
            return aString;
        }
    }

    public enum ServiceState
    {
        Stopped,
        Running,
        Paused
    }


    public class RfIdeasTagReaders : List<RfIdeasTcpReader>, INotifyCollectionChanged
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
            {
                _lock.EnterWriteLock();
                try
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        new public void Add(RfIdeasTcpReader rec)
        {
            _lock.EnterWriteLock();
            try
            {
                base.Add(rec);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(RfIdeasTcpReader rec)
        {
            _lock.EnterWriteLock();
            try
            {
                base.RemoveAt(this.IndexOf(rec));
            }
            finally
            {
                _lock.ExitWriteLock();
            }            
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }

        #endregion
    }

    public class RfIdeasTcpReader : INotifyPropertyChanged
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private StringQueue CommandQ;
        private StringQueue CardReadQ;
        private StringQueue FeedbackQ;

        private bool _updateLEDNow = true;
        public bool UpdateLEDNow
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _updateLEDNow;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _updateLEDNow = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private bool allowFeedback = false;

        public bool AllowFeedback
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return allowFeedback;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    allowFeedback = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private bool _isActiveStation = false;
        public bool IsActiveStation
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _isActiveStation;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _isActiveStation = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private DateTime badReadTime = DateTime.MinValue;

        public DateTime BadReadTime
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return badReadTime;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    badReadTime = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private bool hasBadRead = false;

        public bool HasBadRead
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return hasBadRead;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    hasBadRead = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public CancellationTokenSource cancellationTokenSource;

        private CancellationToken cancellationToken;

        private Task commsTask;

        private bool isGo = true;

        public bool IsGo
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return isGo;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    isGo = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private bool _hasChanged;
        public bool HasChanged
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _hasChanged;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _hasChanged = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private int _locationId;
        public int LocationID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _locationId;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _locationId = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            HasChanged = true;
            if (PropertyChanged != null)
            {
                _lock.EnterWriteLock();
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                    PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private string ipAddress;

        public string IpAddress
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ipAddress;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    ipAddress = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public string Description
        {
            get { return ipAddress + ", " + port.ToString(); }
        }

        private int maxLogins;

        public int MaxLogins
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return maxLogins;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set
            {
                _lock.EnterWriteLock();
                try
                {
                    maxLogins = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private int port;

        public int Port
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return port;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    port = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private int machineID;

        public int MachineID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return machineID;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    machineID = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private int subID;

        public int SubID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return subID;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    subID = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private string cardID;

        public string CardID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return cardID;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                if (CardID != value)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        cardID = value;
                        NotifyPropertyChanged("CardID");
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }

        }

        private string status;

        public string Status
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return status;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                if (Status != value)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        status = value;
                        NotifyPropertyChanged("Status");
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }
        private string _currentLED;

        public string CurrentLED
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _currentLED;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                if (Status != value)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        _currentLED = value;
                        NotifyPropertyChanged("CurrentLED");
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }
        private bool isConnected = false;

        public bool IsConnected
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return isConnected;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                if (IsConnected != value)
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        isConnected = value;
                        NotifyPropertyChanged("IsConnected");
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }

        public RfIdeasTcpReader(int locationid, string ipaddress, int _port, int machineid, int sub, int maxlogins, bool allowfeedback)
        {
            LocationID = locationid;
            IpAddress = ipaddress;
            Port = _port;
            MachineID = machineid;
            SubID = sub;
            MaxLogins = maxlogins;
            AllowFeedback = allowfeedback;
            CommandQ = new StringQueue();
            CardReadQ = new StringQueue();
            FeedbackQ = new StringQueue();
        }

        public bool IsFeedBack
        {
            get { return (FeedbackQ.Count > 0); }
        }

        public string ReadFeedback()
        {
            string feedback = string.Empty;
            string fb = string.Empty;
            bool gotFeedback = FeedbackQ.TryDequeue(out fb);
            if (gotFeedback)
            {
                if (fb.Trim() != string.Empty)
                {
                    feedback += ipAddress + " " + fb;
                }
            }
            return feedback;
        }

        public bool StartMonitorTask(bool loggedin)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            IsGo = true;
            Task MonitorTask;
            MonitorTask = new Task(new Action<object>(MonitorCommsProcess), loggedin);
            MonitorTask.Start();
            return true;
        }

        public void MonitorCommsProcess(Object obj)
        {
            try
            {
                IsActiveStation = (bool)obj;
                while (IsGo)
                {
                    Debug.WriteLine("MonitorCommsProcess");
                    SendCommand("rfid:cmd.echo=true");
                    SendCommand("rfid:cmd.prompt=false");
                    if (IsActiveStation)
                        GreenLED();
                    else
                        RedLED();
                    //Debug.WriteLine("MonitorCommsProcess for " + IpAddress.ToString());
                    commsTask = new Task(() => commsProcess(), cancellationToken);
                    commsTask.Start();
                    try
                    {
                        Task.WaitAny(commsTask);
                        cancellationToken.WaitHandle.WaitOne(1000);//cool off period before starting new task
                        //Debug.WriteLine("Stopped Monitor Comms Process");
                    }
                    catch (AggregateException ex)
                    {
                        Debug.WriteLine("MonitorCommsProcess" + ex.Message);
                        if (allowFeedback) FeedbackQ.Enqueue("MonitorCommsProcess" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MonitorCommsProcess" + ex.Message);
                if (allowFeedback) FeedbackQ.Enqueue("MonitorCommsProcess" + ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                IsConnected = false;
                IsGo = false;
                cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public string ReadCardID(out int machineid, out int subid)
        {
            string card = string.Empty;
            machineid = MachineID;
            subid = SubID;
            if (IsConnected)
            {
                card = ReadCardID();
            }
            return card;
        }
        private string ReadCardID()
        {
            string card = string.Empty;
            // cardReadQ is a buffer of everything returned from a card reader
            // it contains the initialisation acknowledgments on start up
            // and regular information about the state of the device
            // such as led status
            if (CardReadQ.Count < 2)
            {
                // this requests any card ID data from the reader
                // and is performed very often to capture card reads.
                SendCommand("rfid:qid.id");
            }
            else
            {
                bool gotCardID = CardReadQ.TryDequeue(out card);
                if (gotCardID)
                {
                    if (card != string.Empty)
                    {
                        int begin = card.IndexOf("{") + 1;
                        int end = card.IndexOf("}");
                        if ((begin > 0) && (end > begin))
                        {
                            card = card.Substring(begin, end - begin);
                            begin = card.IndexOf("0x");
                            while (begin >= 0)
                            {
                                card = card.Substring(begin + 2);
                                begin = card.IndexOf("0x");
                            }
                            while (card.Length < 8)
                                card += "0";
                            if (card == "00000000")
                                card = string.Empty;
                        }
                        else card = string.Empty;
                    }
                }
                else card = string.Empty;
            }
            return card;
        }
        
        public int ReadLUID()
        {
            SendCommand("rfid:dev.luid?");
            return 1;
        }

        public void RedLED()
        {
            SendCommand("rfid:out.led=1");
            CurrentLED = "RED";
        }

        public void GreenLED()
        {
            SendCommand("rfid:out.led=2");
            CurrentLED = "GREEN";
        }

        public void OrangeLED()
        {
            SendCommand("rfid:out.led=3");
            CurrentLED = "ORANGE";
        }

        public void NoLED()
        {
            SendCommand("rfid:out.led=0");
            CurrentLED = "NO LED";
        }

        public void Beep()
        {
            SendCommand("rfid:beep.now=1");
        }

        private void SendCommand(string cmd)
        {
            CommandQ.Enqueue(cmd);
        }

        private bool canConnect()
        {
            bool test = false;
            try
            {
                Ping ping = new Ping();
                PingReply reply;
                reply = ping.Send(IpAddress);
                test=(reply.Status == IPStatus.Success);
                if (test)
                {
                    //if (allowFeedback) FeedbackQ.Enqueue("Ping " + IpAddress.ToString() + " Reply");
                }
                else
                {
                    //if (allowFeedback) FeedbackQ.Enqueue("Ping " + IpAddress.ToString() + " Fail");
                }
            }
            catch (Exception ex)
            {
                if (allowFeedback) FeedbackQ.Enqueue("Exception in CanConnect " + ex.Message);
                Debug.WriteLine("Exception in CanConnect " + ex.Message);
            }            
            return test;
        }

        private void commsProcess()
        {
            Debug.WriteLine("commsProcess for " + ipAddress.ToString());
            TcpClient tcpClient = null;
            bool error = !canConnect();
            int nocardCount = 0;
            try
            {
                if (!error)
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipAddress, port);
                    IsConnected = tcpClient.Connected;
                    error = !isConnected;
                    
                    while (IsGo && !error)
                    {   //wait for command    
                        if (CommandQ.Count > 0)
                        {
                            string cmd;
                            bool gotCommand = CommandQ.TryDequeue(out cmd);
                            if (gotCommand)
                            {   //send command 
                                //if (allowFeedback)
                                //    FeedbackQ.Enqueue("sending command " + cmd + " to " + ipAddress.ToString());
                                string aString = string.Empty;
                                try
                                {
                                    if (tcpClient != null)
                                    {
                                        if (tcpClient.Connected)
                                        {
                                            String str = cmd + Environment.NewLine;
                                            NetworkStream stm = tcpClient.GetStream();

                                            ASCIIEncoding asen = new ASCIIEncoding();
                                            byte[] ba = asen.GetBytes(str);

                                            stm.Write(ba, 0, ba.Length);

                                            byte[] bb = new byte[100];
                                            while (stm.DataAvailable)
                                            {
                                                int k = stm.Read(bb, 0, 100);
                                                for (int i = 0; i < k; i++)
                                                {
                                                    char achar=Convert.ToChar(bb[i]);
                                                    if ((achar=='\n')||(achar=='\r'))
                                                        achar=' ';
                                                    aString += achar;
                                                }
                                                aString += Environment.NewLine;
                                                //Debug.WriteLine("Read " + k + " bytes " + aString);                                          
                                            }
                                            if (stm.DataAvailable)
                                            {
                                                Debug.WriteLine("Read overflow");
                                            }

                                        }
                                        else error = true;
                                    }
                                    else error = true;
                                }
                                catch (Exception ex)
                                {
                                    error = true;
                                    if (allowFeedback) FeedbackQ.Enqueue("commsProcess " + ex.Message);
                                }
                                //queue response
                                if (aString != string.Empty)
                                {
                                    //Debug.WriteLine("reader response " + aString);
                                    if (!(aString.Contains("{0x0000,0,0x00,0;0x00}")||(aString.Contains("{0xFFFF,0,0x00,0;0x00}")))) //test for no card
                                    {
                                        CardReadQ.Enqueue(aString);
                                        //if (allowFeedback) FeedbackQ.Enqueue("Card Read: " + aString);
                                        nocardCount = 0;
                                    }
                                    else
                                    {
                                        if ((allowFeedback) && (nocardCount == 0))
                                        {
                                            //FeedbackQ.Enqueue("No Card"); // this is written a lot and is fairly pointless.
                                            nocardCount++;
                                        }
                                    }
                                }
                            }
                            else cancellationToken.WaitHandle.WaitOne(10);
                        }
                        cancellationToken.WaitHandle.WaitOne(100);
                    }
                }
            }
            catch (Exception ex)
            {
                if (allowFeedback) FeedbackQ.Enqueue("commsProcess " + ex.Message);
            }
            finally
            {
                if (tcpClient != null)
                    tcpClient.Close();
                IsConnected = false;
                if (allowFeedback) FeedbackQ.Enqueue("Disconnected from " + ipAddress + ":" + port);
            }
            cancellationToken.WaitHandle.WaitOne(30000);//wait for a bit then try again
            //if this fails it will be called again by MonitorCommsProcess until !isGo
        }
    
    }




    public class OperatorLoginStates : List<OperatorLoginState>, INotifyCollectionChanged
    {
        public delegate void FeedbackDelegate(string feedback);
        public FeedbackDelegate StatesFeedback = null;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void Feedback(string info)
        {
            if (StatesFeedback != null)
                StatesFeedback(info);
            else
                Debug.WriteLine(info);
        }
        public void Reset()
        {
            if (CollectionChanged != null)
            {
                _lock.EnterWriteLock();
                try
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public new void Add(OperatorLoginState rec)
        {
            _lock.EnterWriteLock();
            try
            {
                base.Add(rec);
                rec.StateFeedback = Feedback;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        public new void Remove(OperatorLoginState rec)
        {
            _lock.EnterWriteLock();
            try
            {
                base.RemoveAt(this.IndexOf(rec));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }
        #endregion

        public OperatorLoginState GetByOperatorID(int id)
        {
            _lock.EnterReadLock();
            try
            {
                return this.Find(delegate(OperatorLoginState item)
                {
                    return item.OperatorID == id;
                });
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public OperatorLoginState GetLoggedInByMachineSubID(int machineid, int subid)
        {
            _lock.EnterReadLock();
            try
            {
                return this.Find(delegate(OperatorLoginState item)
                {
                    return ((item.MachineID == machineid)
                        && (item.SubID == subid)
                        && (item.IsLoggedIn));
                });
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public OperatorLoginState GetByMachineSubID(int machineid, int subid)
        {
            _lock.EnterReadLock();
            try
            {
                return this.Find(delegate(OperatorLoginState item)
                {
                    return ((item.MachineID == machineid)
                        && (item.SubID == subid));
                });
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public int TotalLoggedInByMachineSubID(int machineid, int subid)
        {
            int count = 0;
            _lock.EnterReadLock();
            try
            {
                foreach (OperatorLoginState rec in this)
                {
                    if ((rec.MachineID == machineid)
                        && (rec.SubID == subid)
                            && (rec.IsLoggedIn))
                    {
                        count++;
                    }
                }
                return count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    public class OperatorLoginState : ObservableObject
    {
        public delegate void FeedbackDelegate(string feedback);
        public FeedbackDelegate StateFeedback = null;
        public void Feedback(string info)
        {
            if (StateFeedback != null)
                StateFeedback(info);
            else
                Debug.WriteLine(info);
        }
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private int _logId;
        public int LogId
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _logId;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _logId = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private int _operatorId;
        public int OperatorID 
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _operatorId;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
        private int machineID;

        public int MachineID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return machineID;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    machineID = AssignNotify(ref machineID, value, "MachineID");
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }


        private int subID;

        public int SubID
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return subID;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    subID = AssignNotify(ref subID, value, "SubID");
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private DateTime _loginTime;
        public DateTime LoginTime
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _loginTime;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _loginTime = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private DateTime _logoutTime;
        public DateTime LogoutTime
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _logoutTime;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _logoutTime = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return isLoggedIn;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    isLoggedIn = AssignNotify(ref isLoggedIn, value, "IsLoggedIn");
                    //Feedback("Memory: Operator= " + OperatorID + ", Machine= " + MachineID + ", SubId= " + SubID + ", IsLoggedIn=" + value + Environment.NewLine);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        private bool _clockedIn;
        public bool ClockedIn 
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _clockedIn;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _clockedIn = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            } 
        }
        private int _breakId;
        public int BreakID 
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _breakId;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _breakId = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            } 
        }

        public bool InBreak(int id)
        {
            return (id == BreakID) && (id > 0);
        }


        private readonly RemoteOperatorLogsGroupedByOperator remoteOperatorLogs;
        private readonly JGLogData logData;
        private readonly MachineSubIDs machineSubIDs;
        private readonly SqlDataAccess da;

        public OperatorLoginState(int opid, int machineid, int subid, DateTime login, DateTime logout,
            bool isloggedin, RemoteOperatorLogsGroupedByOperator remoteoperatorlogs, JGLogData logdata, MachineSubIDs machinesubids)
        {
            _operatorId = opid;
            MachineID = machineid;
            SubID = subid;
            LoginTime = login;
            LogoutTime = logout;
            IsLoggedIn = isloggedin;
            remoteOperatorLogs = remoteoperatorlogs;
            logData = logdata;
            machineSubIDs = machinesubids;
            da = SqlDataAccess.Singleton;
        }


        public void Login(int machineid, int subid, DateTime login)
        {
            MachineID = machineid;
            SubID = subid;
            LoginTime = login;
            LogoutTime = DateTime.MaxValue;
            LogInOut(1);
            IsLoggedIn = true;
        }

        public void ClockIn(DateTime clockintime)
        {
            JGLogDataRec rec = createLogDataRecord(clockintime, 1);
            rec.SubRegType = 5;
            logData.Add(rec);
            logData.UpdateToDB();
            RemoteOperatorLog rol = createOperatorLogRecord(clockintime, 1);
            rol.SubregType = 5;
            remoteOperatorLogs.Add(rol);
            remoteOperatorLogs.UpdateToDB();
        }

        public void BreakEnd(DateTime endtime, int breakid)
        {
            JGLogDataRec rec = createLogDataRecord(endtime, 0);
            rec.SubRegType = 6;
            rec.SubRegTypeID = breakid;
            logData.Add(rec);
            logData.UpdateToDB();
            RemoteOperatorLog rol = createOperatorLogRecord(endtime, 0);
            rol.SubregType = 6;
            remoteOperatorLogs.Add(rol);
            remoteOperatorLogs.UpdateToDB();
        }

        public void Logout(DateTime logout)
        {
            LogoutTime = logout;
            LogInOut(0);
            IsLoggedIn = false;
        }

        private JGLogDataRec createLogDataRecord(DateTime logtime, int state)
        {
            JGLogDataRec ldr = new JGLogDataRec();
            ldr.RecNum = 0;
            ldr.RemoteID = -1;
            ldr.CompanyID = 99;
            ldr.TimeStamp = logtime;
            ldr.MachineID = MachineID;
            ldr.PositionID = -1;
            ldr.SubID = SubID;
            ldr.SubIDName = string.Empty;
            ldr.RegType = 6;
            ldr.SubRegType = 0;
            ldr.SubRegTypeID = 0;
            ldr.State = state;
            ldr.MessageA = "Operator";
            ldr.MessageB = "Operator";
            ldr.BatchID = -1;
            ldr.SourceID = -1;
            ldr.ProcessCode = -1;
            ldr.ProcessName = string.Empty;
            ldr.CustomerID = -1;
            ldr.SortCategoryID = -1;
            ldr.ArticleID = -1;
            ldr.OperatorID = OperatorID;
            ldr.Value = 0;
            ldr.Unit = -1;
            ldr.ForceNew = true;
            return ldr;
        }

        private RemoteOperatorLog createOperatorLogRecord(DateTime logtime, int state)
        {
            RemoteOperatorLog remoteOperatorLog = new RemoteOperatorLog();
            remoteOperatorLog.RemoteOperatorLogID = 0;
            remoteOperatorLog.ForceNew = true;
            remoteOperatorLog.LogTime = logtime;
            remoteOperatorLog.MachineID = MachineID;
            remoteOperatorLog.OperatorID = OperatorID;
            remoteOperatorLog.SubID = SubID;
            remoteOperatorLog.State = state;
            return remoteOperatorLog;
        }
        private void insertOrUpdateMachineSubid(int machineid, int subid, int state)
        {
            MachineSubID machineSubid = (MachineSubID)machineSubIDs.GetByIDSubID(machineID, subID);
            int opRemoteField = analyseOperatorRemoteField(state);
            if (machineSubid == null) //shouldn't happen
            {
                machineSubid = new MachineSubID();
                machineSubid.MachineID = MachineID;
                machineSubid.SubID = SubID;
                machineSubid.ForceNew = true;
                machineSubIDs.Add(machineSubid);
            }

            machineSubid.OperatorRemote = opRemoteField;
            machineSubIDs.UpdateToDB();

            cleanupMachineSubIdTable(machineSubid.MachineID, machineSubid.SubID, opRemoteField);
        }
        private int analyseOperatorRemoteField(int state)
        {
            return (state == 0 ? -1 : OperatorID);
        }
        private bool cleanupMachineSubIdTable(int _machineID, int _subID, int _value)
        {
            bool success = true;
            try
            {
                // this has been replaced by datalayer update
                //da.ExecuteNonQuery("JEGR_DB", string.Format("UPDATE [JEGR_DB].[dbo].[tblMachineSubID] SET [Operator_Remote] = {0} WHERE [Machine_idJensen]={1} AND [SubID]={2}", _value, _machineID, _subID));
                
                // if this is a log in by an operator, this ensures that the operator is not logged into and other machine / subid in tblMachineSubId.
                if(_value != -1)
                    da.ExecuteNonQuery("JEGR_DB", string.Format("UPDATE [JEGR_DB].[dbo].[tblMachineSubID] SET [Operator_Remote] = -1 WHERE [Operator_Remote]={0} AND ([Machine_idJensen]<>{1} OR [SubID]<>{2})", OperatorID, _machineID, _subID));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                success = false;
            }
            return success;
        }

        private void LogInOut(int state)
        {
            try
            {
                DateTime logtime = LoginTime;
                if (state == 0)
                    logtime = LogoutTime;
                try
                {
                    logData.Add(createLogDataRecord(logtime, state));
                    logData.UpdateToDB();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Logdata: " + ex.Message);
                }
                try
                {
                    var oldRecord = remoteOperatorLogs.GetByOperatorId(OperatorID);
                    remoteOperatorLogs.Add(createOperatorLogRecord(logtime, state));
                    remoteOperatorLogs.UpdateToDB();
                    if (oldRecord != null)
                        remoteOperatorLogs.Remove(oldRecord);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RemoteOperatorLogs: " + ex.Message);
                }
                try
                {
                    insertOrUpdateMachineSubid(machineID, subID, state);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MachineSubIds: " + ex.Message);
                }
            }
            catch (Exception ex)
            {               
                Debug.WriteLine(ex.Message);
            }
        }


    }

    public class StringQueue : ConcurrentQueue<string>
    { 
    }


}
