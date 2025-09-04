using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using Ge_Mac.LoggingAndExceptions;


namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Caches

        private AuditControls auditControlsCache = null;
        private AuditLogs auditLogsCache = null;

        private bool AuditControlsAreCached()
        {
            bool test = (auditControlsCache != null);
            if (test)
            {
                test = auditControlsCache.IsValid;
            }
            return test;
        }

        private bool AuditLogsAreCached()
        {
            bool test = (auditLogsCache != null);
            if (test)
            {
                test = auditLogsCache.IsValid;
            }
            return test;
        }


        #endregion
 
        #region Select Data
        #region AuditControls

        public AuditControls GetAllAuditControls()
        {
            if (AuditControlsAreCached())
            {
                return auditControlsCache;
            }
            try
            {
                const string commandString = @"SELECT [ControlID]
                                                      ,[AppID]
                                                      ,[DataType]
                                                      ,[IsAudited]
                                                      ,[DetailAudit]
                                                  FROM [dbo].[tblAuditControl]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (auditControlsCache == null) auditControlsCache = new AuditControls();
                    command.DataFill(auditControlsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return auditControlsCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                throw;
            }
        }

        #endregion

        #region AuditLogs

        public AuditLogs GetAllAuditLogs()
        {
            if (AuditLogsAreCached())
            {
                return auditLogsCache;
            }
            try
            {
                const string commandString = @"SELECT [AuditLogID]
                                                      ,[AppID]
                                                      ,[AppUserID]
                                                      ,[EventTime]
                                                      ,[Narrative]
                                                      ,[DataType]
                                                      ,[DataXML]
                                                  FROM [dbo].[tblAuditLog]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (auditLogsCache == null) auditLogsCache = new AuditLogs();
                    command.DataFill(auditLogsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return auditLogsCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                throw;
            }
        }
        
        #endregion

        #endregion

        #region Next Record


        public int NextAuditLogRecord()
        {
            return NextRecord("tblAuditLog", "AuditLogID");
        }

        public int NextAuditControlRecord()
        {
            return NextRecord("tblAuditControl", "ControlID");
        }

        #endregion

        #region Insert Data
        public void InsertNewAuditControl(AuditControl auditControl)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblAuditControl]
                                   ([ControlID]
                                   ,[AppID]
                                   ,[DataType]
                                   ,[IsAudited]
                                   ,[DetailAudit])
                             VALUES
                                   (@ControlID
                                   ,@AppID
                                   ,@DataType
                                   ,@IsAudited
                                   ,@DetailAudit)";

            try
            {
                if (auditControl.ControlID < 0)
                    auditControl.ControlID = NextAuditControlRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ControlID", auditControl.ControlID);
                    command.Parameters.AddWithValue("@AppID", auditControl.AppID);
                    command.Parameters.AddWithValue("@DataType", auditControl.DataType);
                    command.Parameters.AddWithValue("@IsAudited", auditControl.IsAudited);
                    command.Parameters.AddWithValue("@DetailAudit", auditControl.DetailAudit);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        auditControl.HasChanged = false;
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;

                        if (ex.Number != insertError)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        public void InsertNewAuditLog(AuditLog auditLog)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblAuditLog]
                                   ([AuditLogID]
                                   ,[AppID]
                                   ,[AppUserID]
                                   ,[EventTime]
                                   ,[Narrative]
                                   ,[DataType]
                                   ,[DataXML])
                             VALUES
                                   (@AuditLogID
                                   ,@AppID
                                   ,@AppUserID
                                   ,@EventTime
                                   ,@Narrative
                                   ,@DataType
                                   ,@DataXML)";
            try
            {
                if (auditLog.AuditLogID < 0)
                    auditLog.AuditLogID = NextAuditLogRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AuditLogID", auditLog.AuditLogID);
                    command.Parameters.AddWithValue("@AppID", auditLog.AppID);
                    command.Parameters.AddWithValue("@AppUserID", auditLog.AppUserID);
                    command.Parameters.AddWithValue("@EventTime", auditLog.EventTime);
                    command.Parameters.AddWithValue("@Narrative", auditLog.Narrative);
                    command.Parameters.AddWithValue("@DataType", auditLog.DataType);
                    command.Parameters.AddWithValue("@DataXML", auditLog.DataXML);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        auditLog.HasChanged = false;
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;

                        if (ex.Number != insertError)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        #endregion

        #region Update Data

        public void UpdateAuditControl(AuditControl auditControl)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblAuditControl]
                                       SET [AppID] = @AppID
                                          ,[DataType] = @DataType
                                          ,[IsAudited] = @IsAudited
                                          ,[DetailAudit] = @DetailAudit
                                     WHERE [ControlID] = @ControlID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ControlID", auditControl.ControlID);
                    command.Parameters.AddWithValue("@AppID", auditControl.AppID);
                    command.Parameters.AddWithValue("@DataType", auditControl.DataType);
                    command.Parameters.AddWithValue("@IsAudited", auditControl.IsAudited);
                    command.Parameters.AddWithValue("@DetailAudit", auditControl.DetailAudit);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    auditControl.HasChanged = false;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                throw;
            }
        }

        #endregion

        #region Delete Data

        public void DeleteAuditControl(AuditControl auditControl)
        {
            DeleteTableRecord("tblAuditControl", "ControlID", auditControl.ControlID);
        }


        #endregion
    }

    #region Data Collections
    public class AuditControls : List<AuditControl>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(AuditControl auditControl)
        {
            base.Add(auditControl);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, auditControl));
            }
        }

        new public void Remove(AuditControl auditControl)
        {
            base.RemoveAt(this.IndexOf(auditControl));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, auditControl));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblAuditControl";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }

        public int Fill(SqlDataReader dr)
        {
            int ControlIDPos = dr.GetOrdinal("ControlID");
            int AppIDPos = dr.GetOrdinal("AppID");
            int DataTypePos = dr.GetOrdinal("DataType");
            int IsAuditedPos = dr.GetOrdinal("IsAudited");
            int DetailAuditPos = dr.GetOrdinal("DetailAudit");


            this.Clear();
            while (dr.Read())
            {
                AuditControl auditControl = new AuditControl()
                {
                    ControlID = dr.GetInt32(ControlIDPos),
                    AppID = dr.GetInt32(AppIDPos),
                    DataType = dr.GetString(DataTypePos),
                    IsAudited = dr.GetBoolean(IsAuditedPos),
                    DetailAudit = dr.GetBoolean(DetailAuditPos),
                    HasChanged = false
                };

                this.Add(auditControl);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public AuditControl GetById(int id)
        {
            return this.Find(delegate(AuditControl ac)
            {
                return ac.ControlID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (AuditControl auditControl in this)
            {
                if (auditControl.IsNew)
                {
                    if (!auditControl.DeleteRecord)
                        da.InsertNewAuditControl(auditControl);
                }
                else
                {
                    if (auditControl.DeleteRecord)
                        da.DeleteAuditControl(auditControl);
                    else
                        da.UpdateAuditControl(auditControl);
                }
            }
            AuditControls delList = new AuditControls();
            foreach (AuditControl rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (AuditControl rec in delList)
            {
                this.Remove(rec);
            }

        }
    }
    public class AuditLogs : List<AuditLog>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(AuditLog auditLog)
        {
            base.Add(auditLog);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, auditLog));
            }
        }

        new public void Remove(AuditLog auditLog)
        {
            base.RemoveAt(this.IndexOf(auditLog));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, auditLog));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblAuditLogs";
        private DateTime lastDBUpdate;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }

        public int Fill(SqlDataReader dr)
        {
            int AuditLogIDPos = dr.GetOrdinal("AuditLogID");
            int AppIDPos = dr.GetOrdinal("AppID");
            int AppUserIDPos = dr.GetOrdinal("AppUserID");
            int EventTimePos = dr.GetOrdinal("EventTime");
            int NarrativePos = dr.GetOrdinal("Narrative");
            int DataTypePos = dr.GetOrdinal("DataType");
            int DataXMLPos = dr.GetOrdinal("DataXML");

            this.Clear();
            while (dr.Read())
            {
                AuditLog auditLog = new AuditLog()
                {
                    AuditLogID = dr.GetInt32(AuditLogIDPos),
                    AppID = dr.GetInt32(AppIDPos),
                    AppUserID = dr.GetInt32(AppUserIDPos),
                    EventTime = dr.GetDateTime(EventTimePos),
                    Narrative = dr.GetString(NarrativePos),
                    DataType = dr.GetString(DataTypePos),
                    DataXML = dr.GetString(DataXMLPos),

                    HasChanged = false
                };

                this.Add(auditLog);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public AuditLog GetById(int id)
        {
            return this.Find(delegate(AuditLog auditLog)
            {
                return auditLog.AuditLogID == id;
            });
        }

    }
  
    #endregion

    #region Items

    public class AuditControl : DataItem, INotifyPropertyChanged
    {
        #region AuditControl Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int ControlID;
            internal int AppID;
            internal string DataType;
            internal bool IsAudited;
            internal bool DetailAudit;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AuditControl()
        {
            ActiveData = (ICopyableObject)new DataRecord();
        }
        #endregion

        #region Abstract Member Variable Properties
        private DataRecord activeData
        {
            get
            {
                return (DataRecord)ActiveData;
            }
            set
            {
                ActiveData = (ICopyableObject)value;
            }
        }

        private DataRecord backupData
        {
            get
            {
                return (DataRecord)BackupData;
            }
            set
            {
                BackupData = (ICopyableObject)value;
            }
        }
        #endregion

        #region Data Column Properties

        public int ControlID
        {
            get
            {
                return this.activeData.ControlID;
            }
            set
            {
                if (this.activeData.ControlID != value)
                {
                    this.activeData.ControlID = value;
                    NotifyPropertyChanged("ControlID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return ControlID;
            }
            set
            {
                if (ControlID != value)
                {
                    ControlID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public String AppDesc
        {
            get
            {
                return GetAppDesc(AppID);
            }
            set
            {
                SetAppID(value);
                NotifyPropertyChanged("AppDesc");
            }
        }

        private string GetAppDesc(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetById(id);
                if (au != null)
                    aString = au.AppDesc;
            }
            return aString;
        }

        private void SetAppID(string appName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetByDesc(appName);
                if (au != null)
                    id = au.AppID;
            }
            if (id > -1)
                activeData.AppID = id;
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        public string DataType
        {
            get
            {
                return this.activeData.DataType;
            }
            set
            {
                if (this.activeData.DataType != value)
                {
                    this.activeData.DataType = value;
                    NotifyPropertyChanged("DataType");
                }
            }
        }

        public bool IsAudited
        {
            get
            {
                return this.activeData.IsAudited;
            }
            set
            {
                if (this.activeData.IsAudited != value)
                {
                    this.activeData.IsAudited = value;
                    NotifyPropertyChanged("IsAudited");
                }
            }
        }

        public bool DetailAudit
        {
            get
            {
                return this.activeData.DetailAudit;
            }
            set
            {
                if (this.activeData.DetailAudit != value)
                {
                    this.activeData.DetailAudit = value;
                    NotifyPropertyChanged("DetailAudit");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion


    }

    public class AuditLog : DataItem, INotifyPropertyChanged
    {
        #region TagReaderLog Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int AuditLogID;
            internal int AppID;
            internal int AppUserID;
            internal DateTime EventTime;
            internal string Narrative;
            internal string DataType;
            internal string DataXML;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AuditLog()
        {
            ActiveData = (ICopyableObject)new DataRecord();
        }
        #endregion

        #region Abstract Member Variable Properties
        private DataRecord activeData
        {
            get
            {
                return (DataRecord)ActiveData;
            }
            set
            {
                ActiveData = (ICopyableObject)value;
            }
        }

        private DataRecord backupData
        {
            get
            {
                return (DataRecord)BackupData;
            }
            set
            {
                BackupData = (ICopyableObject)value;
            }
        }
        #endregion

        #region Data Column Properties

        public int AuditLogID
        {
            get
            {
                return this.activeData.AuditLogID;
            }
            set
            {
                if (this.activeData.AuditLogID != value)
                {
                    this.activeData.AuditLogID = value;
                    NotifyPropertyChanged("AuditLogID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return AuditLogID;
            }
            set
            {
                if (AuditLogID != value)
                {
                    AuditLogID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public String UserDesc
        {
            get
            {
                return GetUserDesc(AppUserID);
            }
            set
            {
                SetUserID(value);
                NotifyPropertyChanged("UserDesc");
            }
        }

        private string GetUserDesc(int uid)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetById(uid);
                if (au != null)
                    aString = au.UserDesc;
            }
            return aString;
        }

        private void SetUserID(string userDesc)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetByDesc(userDesc);
                if (au != null)
                    id = au.AppUserID;
            }
            if (id > -1)
                activeData.AppUserID = id;
        }

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                if (this.activeData.AppUserID != value)
                {
                    this.activeData.AppUserID = value;
                    NotifyPropertyChanged("AppUserID");
                }
            }
        }

        public String AppDesc
        {
            get
            {
                return GetAppDesc(AppID);
            }
            set
            {
                SetAppID(value);
                NotifyPropertyChanged("AppDesc");
            }
        }

        private string GetAppDesc(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetById(id);
                if (au != null)
                    aString = au.AppDesc;
            }
            return aString;
        }

        private void SetAppID(string appName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetByDesc(appName);
                if (au != null)
                    id = au.AppID;
            }
            if (id > -1)
                activeData.AppID = id;
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        public DateTime EventTime
        {
            get
            {
                return this.activeData.EventTime;
            }
            set
            {
                if (this.activeData.EventTime != value)
                {
                    this.activeData.EventTime = value;
                    NotifyPropertyChanged("EventTime");
                }
            }
        }

        public string Narrative
        {
            get
            {
                return activeData.Narrative;
            }
            set
            {
                if (activeData.Narrative != value)
                {
                    activeData.Narrative = value;
                    NotifyPropertyChanged("Narrative");
                }
            }
        }

        public string DataType
        {
            get
            {
                return activeData.DataType;
            }
            set
            {
                if (activeData.DataType != value)
                {
                    activeData.DataType = value;
                    NotifyPropertyChanged("DataType");
                }
            }
        }

        public string DataXML
        {
            get
            {
                return activeData.DataXML;
            }
            set
            {
                if (activeData.DataXML != value)
                {
                    activeData.DataXML = value;
                    NotifyPropertyChanged("DataXML");
                }
            }
        }

        #endregion


    }


    
    #endregion


}
