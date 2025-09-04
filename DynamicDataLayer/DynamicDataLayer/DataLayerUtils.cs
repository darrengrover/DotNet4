using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dynamic.DataLayer
{
    #region Permissions
    partial class SqlDataAccess
    {
        public Permissions GetPermissions(Permissions recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Permissions();
            try
            {
                recs = (Permissions)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            }
            catch (Exception ex)
            {}
            return recs;
        }
        public OperatorPermissions GetOperatorPermissions(OperatorPermissions recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new OperatorPermissions();
            try
            {
                recs = (OperatorPermissions)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            }
            catch (Exception ex)
            {}
            return recs;
        }
    }
    public class Permissions : DataList
    {
        public Permissions()
        {
            Lifespan = 1.0;
            ListType = typeof(Permission);
            TblName = "tblPermissions";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("PermissionName", "PermissionName", true, false);
            dBFieldMappings.AddMapping("DefaultValue", "DefaultValue");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
        public bool HasPermission(string permissionToCheck)
        {
            var permission = this.OfType<Permission>().FirstOrDefault(p => p.PermissionName == permissionToCheck);
            return permission != null ? permission.DefaultValue : true;
        }
    }

    public class Permission : DataItem
    {
        #region private fields
        private string permissionName;
        private bool defaultValue;
        #endregion

        #region public fields

        public string PermissionName
        {
            get { return permissionName; }
            set
            {
                permissionName = AssignNotify(ref permissionName, value, "PermissionName");
            }
        }

        public bool DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = AssignNotify(ref defaultValue, value, "DefaultValue"); }
        }

        #endregion

    }
    public class OperatorPermissions : DataList
    {
        public OperatorPermissions()
        {
            Lifespan = 1.0;
            ListType = typeof(OperatorPermission);
            TblName = "tblOperatorPermissions";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("OperatorId", "OperatorId", true, false);
            dBFieldMappings.AddMapping("Permissions", "Permissions");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class OperatorPermission : DataItem
    {
        #region private fields
        private int operatorId;
        private string permissions;
        #endregion

        #region public fields

        public int OperatorId
        {
            get { return operatorId; }
            set
            {
                operatorId = AssignNotify(ref operatorId, value, "OperatorId");
                ID = operatorId;
            }
        }

        public string Permissions
        {
            get { return permissions; }
            set 
            { 
                permissions = AssignNotify(ref permissions, value, "Permissions");
                GenerateUserPermissions();
            }
        }

        #endregion
        private Dictionary<string, bool> userPermissions = new Dictionary<string, bool>();
        private void GenerateUserPermissions()
        {
            try
            {
                userPermissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissions);
            }
            catch (Exception ex)
            {
                userPermissions = new Dictionary<string, bool>();
            }
        }
        private bool permissionExists(string permissionName)
        {
            return Permissions.Contains(permissionName) && userPermissions != null && userPermissions.ContainsKey(permissionName);
        }
        public bool IsAllowedTo(Permission permission)
        {
            if (permissionExists(permission.PermissionName))
                return userPermissions[permission.PermissionName];
            return permission.DefaultValue;
        }
    }
    #endregion

    #region Audit

    public class AuditControls : DataList
    {
        public AuditControls()
        {
            Lifespan = 1.0;
            ListType = typeof(AuditControl);
            TblName = "tblAuditControl";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ControlID", "ControlID", true, false);
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("DataType", "DataType");
            dBFieldMappings.AddMapping("IsAudited", "IsAudited");
            dBFieldMappings.AddMapping("DetailAudit", "DetailAudit");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AuditControl : DataItem
    {
        #region private fields

        private int controlID;
        private int appID;
        private string dataType;
        private bool isAudited;
        private bool detailAudit;

        #endregion

        #region properties

        public int ControlID
        {
            get { return controlID; }
            set
            {
                controlID = AssignNotify(ref controlID, value, "ControlID");
                ID = controlID;
                PrimaryKey = controlID;
            }
        }
        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }

        public string DataType
        {
            get { return dataType; }
            set
            {
                dataType = AssignNotify(ref dataType, value, "DataType");
                ItemName = dataType;
            }
        }
        public bool IsAudited
        {
            get { return isAudited; }
            set { isAudited = AssignNotify(ref isAudited, value, "IsAudited"); }
        }
        public bool DetailAudit
        {
            get { return detailAudit; }
            set { detailAudit = AssignNotify(ref detailAudit, value, "DetailAudit"); }
        }

        //public String AppDesc
        //{
        //    get
        //    {
        //        return GetAppDesc(AppID);
        //    }
        //    set
        //    {
        //        SetAppID(value);
        //        NotifyPropertyChanged("AppDesc");
        //    }
        //}

        //private string GetAppDesc(int id)
        //{
        //    string aString = string.Empty;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    Apps aps = da.GetAllApps();
        //    if (aps != null)
        //    {
        //        App au = aps.GetById(id);
        //        if (au != null)
        //            aString = au.AppDesc;
        //    }
        //    return aString;
        //}

        //private void SetAppID(string appName)
        //{
        //    int id = -1;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    Apps aps = da.GetAllApps();
        //    if (aps != null)
        //    {
        //        App au = aps.GetByDesc(appName);
        //        if (au != null)
        //            id = au.AppID;
        //    }
        //    if (id > -1)
        //        activeData.AppID = id;
        //}


        #endregion

    }

    public class AuditLogs : DataList
    {
        public AuditLogs()
        {
            Lifespan = 1.0;
            ListType = typeof(AuditLog);
            TblName = "tblAuditLog";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AuditLogID", "AuditLogID", true, false);
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("AppUserID", "AppUserID");
            dBFieldMappings.AddMapping("EventTime", "EventTime");
            dBFieldMappings.AddMapping("Narrative", "Narrative");
            dBFieldMappings.AddMapping("DataType", "DataType");
            dBFieldMappings.AddMapping("DataXML", "DataXML");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AuditLog : DataItem
    {
        #region private fields

        private int auditLogID;
        private int appID;
        private int appUserID;
        private DateTime eventTime;
        private string narrative;
        private string dataType;
        private string dataXML;

        #endregion

        #region Properties

        public int AuditLogID
        {
            get { return auditLogID; }
            set
            {
                auditLogID = AssignNotify(ref auditLogID, value, "AuditLogID");
                ID = auditLogID;
                PrimaryKey = auditLogID;
            }
        }

        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }

        public int AppUserID
        {
            get { return appUserID; }
            set { appUserID = AssignNotify(ref appUserID, value, "AppUserID"); }
        }

        public DateTime EventTime
        {
            get { return eventTime; }
            set { eventTime = AssignNotify(ref eventTime, value, "EventTime"); }
        }

        public string Narrative
        {
            get { return narrative; }
            set { narrative = AssignNotify(ref narrative, value, "Narrative"); }
        }

        public string DataType
        {
            get { return dataType; }
            set
            {
                dataType = AssignNotify(ref dataType, value, "DataType");
                ItemName = dataType;
            }
        }

        public string DataXML
        {
            get { return dataXML; }
            set { dataXML = AssignNotify(ref dataXML, value, "DataXML"); }
        }

        //public String UserDesc
        //{
        //    get
        //    {
        //        return GetUserDesc(AppUserID);
        //    }
        //    set
        //    {
        //        SetUserID(value);
        //        NotifyPropertyChanged("UserDesc");
        //    }
        //}

        //private string GetUserDesc(int uid)
        //{
        //    string aString = string.Empty;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    AppUsers aus = da.GetAllAppUsers();
        //    if (aus != null)
        //    {
        //        AppUser au = aus.GetById(uid);
        //        if (au != null)
        //            aString = au.UserDesc;
        //    }
        //    return aString;
        //}

        //private void SetUserID(string userDesc)
        //{
        //    int id = -1;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    AppUsers aus = da.GetAllAppUsers();
        //    if (aus != null)
        //    {
        //        AppUser au = aus.GetByDesc(userDesc);
        //        if (au != null)
        //            id = au.AppUserID;
        //    }
        //    if (id > -1)
        //        activeData.AppUserID = id;
        //}

        //public int AppUserID
        //{
        //    get
        //    {
        //        return this.activeData.AppUserID;
        //    }
        //    set
        //    {
        //        if (this.activeData.AppUserID != value)
        //        {
        //            this.activeData.AppUserID = value;
        //            NotifyPropertyChanged("AppUserID");
        //        }
        //    }
        //}

        //public String AppDesc
        //{
        //    get
        //    {
        //        return GetAppDesc(AppID);
        //    }
        //    set
        //    {
        //        SetAppID(value);
        //        NotifyPropertyChanged("AppDesc");
        //    }
        //}

        //private string GetAppDesc(int id)
        //{
        //    string aString = string.Empty;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    Apps aps = da.GetAllApps();
        //    if (aps != null)
        //    {
        //        App au = aps.GetById(id);
        //        if (au != null)
        //            aString = au.AppDesc;
        //    }
        //    return aString;
        //}

        //private void SetAppID(string appName)
        //{
        //    int id = -1;
        //    SqlDataAccess da = SqlDataAccess.Singleton;
        //    Apps aps = da.GetAllApps();
        //    if (aps != null)
        //    {
        //        App au = aps.GetByDesc(appName);
        //        if (au != null)
        //            id = au.AppID;
        //    }
        //    if (id > -1)
        //        activeData.AppID = id;
        //}


        #endregion


    }

    #endregion

    #region Security/Rights

    #region AppRights
    public class AppRights : DataList
    {
        public AppRights()
        {
            Lifespan = 1.0;
            ListType = typeof(AppRight);
            TblName = "tblAppRights";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RightsID", "ID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppRight : DataItem
    { }
    #endregion
    
    #region AppUserGroups
    public class AppUserGroups : DataList
    {
        public AppUserGroups()
        {
            Lifespan = 1.0;
            ListType = typeof(AppUserGroup);
            TblName = "tblAppUserGroups";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("UserGroupID", "ID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppUserGroup : DataItem
    { }
    #endregion

    #region AppUserGroupMembers

    public class AppUserGroupMembers : DataList
    {
        public AppUserGroupMembers()
        {
            Lifespan = 1.0;
            ListType = typeof(AppUserGroupMember);
            TblName = "tblAppUserGroupMembers";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("MemberID", "MemberID", true, false);
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("AppUserID", "AppUserID");
            dBFieldMappings.AddMapping("UserGroupID", "UserGroupID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppUserGroupMember : DataItem
    {
        #region private fields

        private int memberID;
        private int appID;
        private int appUserID;
        private int userGroupID;


        #endregion

        #region properties

        public int MemberID
        {
            get { return memberID; }
            set
            {
                memberID = AssignNotify(ref memberID, value, "MemberID");
                ID = memberID;
                PrimaryKey = memberID;
            }
        }
        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }
        public int AppUserID
        {
            get { return appUserID; }
            set { appUserID = AssignNotify(ref appUserID, value, "AppUserID"); }
        }
        public int UserGroupID
        {
            get { return userGroupID; }
            set { userGroupID = AssignNotify(ref userGroupID, value, "UserGroupID"); }
        }

        #endregion
    }

    #endregion

    #region AppUserRights

    public class AppUserRights : DataList
    {
        public AppUserRights()
        {
            Lifespan = 1.0;
            ListType = typeof(AppUserRight);
            TblName = "tblAppUserRights";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AppUserRightsID", "AppUserRightsID", true, false);
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("AppUserID", "AppUserID");
            dBFieldMappings.AddMapping("RightsID", "RightsID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppUserRight : DataItem
    {
        #region private fields

        private int appUserRightsID;
        private int appID;
        private int appUserID;
        private int rightsID;


        #endregion

        #region properties

        public int AppUserRightsID
        {
            get { return appUserRightsID; }
            set
            {
                appUserRightsID = AssignNotify(ref appUserRightsID, value, "AppUserRightsID");
                ID = appUserRightsID;
                PrimaryKey = appUserRightsID;
            }
        }
        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }
        public int AppUserID
        {
            get { return appUserID; }
            set { appUserID = AssignNotify(ref appUserID, value, "AppUserID"); }
        }
        public int RightsID
        {
            get { return rightsID; }
            set { rightsID = AssignNotify(ref rightsID, value, "RightsID"); }
        }

        #endregion
    }

    #endregion
    #endregion

    #region CockpitDatabaseNames
    public class CockpitDatabaseNames : DataList
    {
        public CockpitDatabaseNames()
        {
            Lifespan = 1.0;
            ListType = typeof(CockpitDatabaseName);
            TblName = "sys.databases";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("database_id", "ID", true, false);
            dBFieldMappings.AddMapping("Name", "ItemName");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            DefaultSqlSelectCommand = @"select database_id,name from sys.databases
                                        where not name in ('master','tempdb','model','msdb')
                                        order by database_id";
        }

    }

    public class CockpitDatabaseName : DataItem
    { }
    #endregion

    #region Holidays

    public class Holidays : DataList
    {
        public Holidays()
        {
            Lifespan = 1.0;
            ListType = typeof(Holiday);
            TblName = "tblHolidays";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DayID", "DayID", true, false);
            dBFieldMappings.AddMapping("IsWorking", "IsWorking");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class Holiday : DataItem
    {
        #region private fields
        private int holidayID;
        private DateTime holidayDate;
        #endregion

        #region public fields

        public int HolidayID
        {
            get { return holidayID; }
            set
            {
                holidayID = AssignNotify(ref holidayID, value, "HolidayID");
                ID = holidayID;
                PrimaryKey = holidayID;
            }
        }

        public DateTime HolidayDate
        {
            get { return holidayDate; }
            set { holidayDate = AssignNotify(ref holidayDate, value, "HolidayDate"); }
        }

        #endregion

    }

    #endregion

    #region Working Days

    public class WorkingDays : DataList
    {
        public WorkingDays()
        {
            Lifespan = 1.0;
            ListType = typeof(WorkingDay);
            TblName = "tblWorkingDays";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DayID", "DayID", true, false);
            dBFieldMappings.AddMapping("IsWorking", "IsWorking");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class WorkingDay : DataItem
    {
        #region private fields
        private int dayID;
        private bool isWorking;
        #endregion

        #region public fields

        public int DayID
        {
            get { return dayID; }
            set
            {
                dayID = AssignNotify(ref dayID, value, "DayID");
                ID = dayID;
                PrimaryKey = dayID;
            }
        }

        public bool IsWorking
        {
            get { return isWorking; }
            set { isWorking = AssignNotify(ref isWorking, value, "IsWorking"); }
        }

        #endregion

    }

    #endregion

    #region Schedules

    #region embedded classes
    public class ScheduleTimespan
    {
        private DateTime start;

        public DateTime Start
        {
            get { return start; }
            set
            {
                TimeSpan ts = value.TimeOfDay;
                DateTime dt = DateTime.MinValue;
                start = dt.AddHours(ts.TotalHours);
            }
        }

        private DateTime end;

        public DateTime End
        {
            get { return end; }
            set
            {
                TimeSpan ts = value.TimeOfDay;
                DateTime dt = DateTime.MinValue;
                end = dt.AddHours(ts.TotalHours);
            }
        }

        private double duration;

        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public ScheduleTimespan()
        {
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            duration = 0;
        }

        public ScheduleTimespan(DateTime _start, DateTime _end)
        {
            Start = _start;
            End = _end;
            duration = calcDuration();
        }

        public ScheduleTimespan(DateTime _start, double _duration)
        {
            Start = _start;
            duration = _duration;
            end = start.AddHours(duration);
        }

        public ScheduleTimespan(double _duration, DateTime _end)
        {
            duration = _duration;
            End = _end;
            start = end.AddHours(-_duration);
        }

        private double calcDuration()
        {
            TimeSpan ts = end.Subtract(start);
            return ts.TotalHours;
        }
    }
    
    public class ScheduleRepeats : List<ScheduleRepeat>
    { }

    public class ScheduleRepeat
    {
        public string RepeatName { get; set; }
        public int DurationType { get; set; }
        public double Duration { get; set; }
        public int YearType { get; set; }
        public List<int> YearList { get; set; }
        public int MonthType { get; set; }
        public List<int> MonthList { get; set; }
        public int NMonths { get; set; }
        public List<string> MonthDetail { get; set; }
        public int DOWType { get; set; }
        public int NWeeks { get; set; }
        public List<int> DOWList { get; set; }
        public int DateType { get; set; }
        public List<DateTime> IncludeDates { get; set; }
        public List<DateTime> ExcludeDates { get; set; }
        public int NDays { get; set; }
        public List<ScheduleTimespan> BetweenTimes { get; set; }
        public int TimeType { get; set; }
        public double NHours { get; set; }
        public double NMinutes { get; set; }
        public double NSeconds { get; set; }
        public List<ScheduleTimespan> ScheduleTimes { get; set; }
    }
    #endregion

    public class Schedules : DataList
    {
        public Schedules()
            : this("JEGR_Utils")
        { }

        public Schedules(string dbname)
        {
            Lifespan = 1.0;
            ListType = typeof(Schedule);
            TblName = "tblSchedules";
            DbName = dbname;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ScheduleID", "ScheduleID", true, false);
            dBFieldMappings.AddMapping("ScheduleName", "ScheduleName");
            dBFieldMappings.AddMapping("ScheduleType", "ScheduleType");
            dBFieldMappings.AddMapping("ScheduledStart", "ScheduledStart");
            dBFieldMappings.AddMapping("ScheduledEnd", "ScheduledEnd");
            dBFieldMappings.AddMapping("Repeats", "Repeats");
            dBFieldMappings.AddMapping("ActiveStart", "ActiveStart");
            dBFieldMappings.AddMapping("ActiveEnd", "ActiveEnd");
            dBFieldMappings.AddMapping("IsActive", "IsActive");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class Schedule : DataItem
    {
        #region private fields
        private int scheduleID;
        private string scheduleName;
        private int scheduleType;
        private DateTime scheduledStart;
        private DateTime scheduledEnd;
        private string repeats;
        private ScheduleRepeats scheduleRepeats; //yes repeats, a single schedule might have several - eventually!
        private DateTime activeStart;
        private DateTime activeEnd;
        private bool isActive;  
        #endregion

        public Schedule()
            : base()
        {
            scheduleID = -1;
            scheduleName = "New Schedule";
            scheduleType = 1;
            scheduledStart = DateTime.Now;
            scheduledEnd = DateTime.Now.AddHours(1);
            scheduleRepeats = null;//new ScheduleRepeats();
            repeats = string.Empty; //deserialise from scheduleRepeats
            activeStart = DateTime.Today;
            activeEnd = DateTime.MaxValue;
            isActive = false;
        }

        public int ScheduleID
        {
            get { return scheduleID; }
            set
            {
                scheduleID = AssignNotify(ref scheduleID, value, "ScheduleID");
                ID = scheduleID;
                PrimaryKey = scheduleID;
            }
        }

        public string ScheduleName
        {
            get { return scheduleName; }
            set
            {
                scheduleName = AssignNotify(ref scheduleName, value, "ScheduleName");
                ItemName = scheduleName;
            }
        }

        public int ScheduleType
        {
            get { return scheduleType; }
            set { scheduleType = AssignNotify(ref scheduleType, value, "ScheduleType"); }
        }

        public DateTime ScheduledStart
        {
            get { return scheduledStart; }
            set { scheduledStart = AssignNotify(ref scheduledStart, value, "ScheduledStart"); }
        }

        public DateTime ScheduledEnd
        {
            get { return scheduledEnd; }
            set { scheduledEnd = AssignNotify(ref scheduledEnd, value, "ScheduledEnd"); }
        }

        public string Repeats
        {
            get { return repeats; }
            set
            {
                repeats = AssignNotify(ref repeats, value, "Repeats");
                //deserialize scheduleRepeats from xml
            }
        }       
        
        public DateTime ActiveStart
        {
            get { return activeStart; }
            set { activeStart = AssignNotify(ref activeStart, value, "ActiveStart"); }
        }

        public DateTime ActiveEnd
        {
            get { return activeEnd; }
            set { activeEnd = AssignNotify(ref activeEnd, value, "ActiveEnd"); }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = AssignNotify(ref isActive, value, "ScheduledStart"); }
        }

        private Byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

    }

    #endregion

    #region Report Engine

    #region Report Definition

    public class RptDefinitions : DataList, INotifyCollectionChanged
    {
        public RptDefinitions()
        {
            Lifespan = 24.0;
            ListType = typeof(RptDefinition);
            TblName = "tblRptDefinitions";
            DbName = "JEGR_Utils";
            MinID = 0;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ReportID", "ReportID", true, false);
            dBFieldMappings.AddMapping("ReportName", "ReportName");
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class RptDefinition : DataItem
    {
        #region private fields
        private int reportID;
        private string reportName;

        #endregion

        #region properties
        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID = reportID;
                PrimaryKey = reportID;
            }
        }

        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = AssignNotify(ref reportName, value, "ElementName");
                ItemName=reportName;
            }
        }

        #endregion
    }

    public class RptGroups : DataList, INotifyCollectionChanged
    {
        public RptGroups()
        {
            Lifespan = 24.0;
            ListType = typeof(RptGroup);
            TblName = "tblRptGroups";
            DbName = "JEGR_Utils";
            MinID = 0;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("GroupID", "GroupID", true, false);
            dBFieldMappings.AddMapping("ReportID", "ReportID");
            dBFieldMappings.AddMapping("GroupName", "GroupName");
            dBFieldMappings.AddMapping("GroupIndex", "GroupIndex");
            dBFieldMappings.AddMapping("BindingName", "BindingName");
            dBFieldMappings.AddMapping("SamePagePriority", "SamePagePriority");
            dBFieldMappings.AddMapping("RepeatHeader", "RepeatHeader");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class RptGroup : DataItem
    {
        #region private fields
        private int groupID;
        private int reportID;
        private string groupName;
        private int groupIndex;
        private string bindingName;
        private int samePagePriority;
        private bool repeatHeader;
        #endregion

        #region properties

        public int GroupID
        {
            get { return groupID; }
            set
            {
                groupID = AssignNotify(ref groupID, value, "GroupID");
                ID = groupID;
                PrimaryKey = groupID;
            }
        }

        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID2 = reportID;
            }
        }

        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = AssignNotify(ref groupName, value, "GroupName");
                ItemName = groupName;
            }
        }

        public int GroupIndex
        {
            get { return groupIndex; }
            set { groupIndex = AssignNotify(ref groupIndex, value, "GroupIndex"); }
        }

        public string BindingName
        {
            get { return bindingName; }
            set { bindingName = AssignNotify(ref bindingName, value, "BindingName"); }
        }

        public int SamePagePriority
        {
            get { return samePagePriority; }
            set { samePagePriority = AssignNotify(ref samePagePriority, value, "SamePagePriority"); }
        }

        public bool RepeatHeader
        {
            get { return repeatHeader; }
            set { repeatHeader = AssignNotify(ref repeatHeader, value, "RepeatHeader"); }
        }

        #endregion
    }

    public class RptSorts : DataList, INotifyCollectionChanged
    {
        public RptSorts()
        {
            Lifespan = 24.0;
            ListType = typeof(RptSorts);
            TblName = "tblRptSorts";
            DbName = "JEGR_Utils";
            MinID = 0;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("SortID", "SortID", true, false);
            dBFieldMappings.AddMapping("GroupID", "GroupID");
            dBFieldMappings.AddMapping("SortName", "SortName");
            dBFieldMappings.AddMapping("SortIndex", "SortIndex");
            dBFieldMappings.AddMapping("BindingName", "BindingName");
            dBFieldMappings.AddMapping("SortType", "SortType");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }   }

    public class RptSort : DataItem
    {
        #region private fields

        private int sortID;
        private int groupID;
        private string sortName;
        private int sortIndex;
        private string bindingName;
        private int sortType;
        private bool isReadOnly;
        private bool showInUI;

        #endregion

        #region properties

        public int SortID
        {
            get { return sortID; }
            set
            {
                sortID = AssignNotify(ref sortID, value, "SortID");
                ID = sortID;
                PrimaryKey = sortID;
            }
        }

        public int GroupID
        {
            get { return groupID; }
            set
            {
                groupID = AssignNotify(ref groupID, value, "GroupID");
                ID2 = groupID;
            }
        }

        public string SortName
        {
            get { return sortName; }
            set
            {
                sortName = AssignNotify(ref sortName, value, "SortName");
                ItemName = sortName;
            }
        }

        public int SortIndex
        {
            get { return sortIndex; }
            set { sortIndex = AssignNotify(ref sortIndex, value, "SortIndex"); }
        }

        public string BindingName
        {
            get { return bindingName; }
            set { bindingName = AssignNotify(ref bindingName, value, "BindingName"); }
        }

        public new int SortType
        {
            get { return sortType; }
            set { sortType = AssignNotify(ref sortType, value, "SortType"); }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = AssignNotify(ref isReadOnly, value, "IsReadOnly"); }
        }

        public bool ShowInUI
        {
            get { return showInUI; }
            set { showInUI = AssignNotify(ref showInUI, value, "ShowInUI"); }
        }

        #endregion
    }

    public class RptFilter : DataItem
    {
        #region private fields

        private int filterID;
        private int groupID;
        private string bindingName;
        private int filterType;
        private string filterValue;
        private string filterValue2;
        private bool isReadOnly;
        private bool showInUI;

        #endregion

        #region properties

        public int FilterID
        {
            get { return filterID; }
            set
            {
                filterID = AssignNotify(ref filterID, value, "FilterID");
                ID = filterID;
                PrimaryKey = filterID;
            }
        }

        public int GroupID
        {
            get { return groupID; }
            set
            {
                groupID = AssignNotify(ref groupID, value, "GroupID");
                ID2 = groupID;
            }
        }

        public string BindingName
        {
            get { return bindingName; }
            set { bindingName = AssignNotify(ref bindingName, value, "BindingName"); }
        }

        public int FilterType
        {
            get { return filterType; }
            set { filterType = AssignNotify(ref filterType, value, "FilterType"); }
        }

        public string FilterValue
        {
            get { return filterValue; }
            set { filterValue = AssignNotify(ref filterValue, value, "FilterValue"); }
        }

        public string FilterValue2
        {
            get { return filterValue2; }
            set { filterValue2 = AssignNotify(ref filterValue2, value, "FilterValue2"); }
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = AssignNotify(ref isReadOnly, value, "IsReadOnly"); }
        }

        public bool ShowInUI
        {
            get { return showInUI; }
            set { showInUI = AssignNotify(ref showInUI, value, "ShowInUI"); }
        }
 
        #endregion
    }

    public class RptSection : DataItem
    {
        #region private fields

        private int sectionID;
        private int groupID;
        private int sectionIndex;
        private double sectionWidth;
        private double sectionHeight;
        private double sectionLeft;
        private double sectionTop;

        #endregion

        #region properties

        public int SectionID
        {
            get { return sectionID; }
            set
            {
                sectionID = AssignNotify(ref sectionID, value, "SectionID");
                ID = sectionID;
                PrimaryKey = sectionID;
            }
        }

        public int GroupID
        {
            get { return groupID; }
            set
            {
                groupID = AssignNotify(ref groupID, value, "GroupID");
                ID2 = groupID;
            }
        }

        public int SectionIndex
        {
            get { return sectionIndex; }
            set { sectionIndex = AssignNotify(ref sectionIndex, value, "SectionIndex"); }
        }

        public double SectionWidth
        {
            get { return sectionWidth; }
            set { sectionWidth = AssignNotify(ref sectionWidth, value, "SectionWidth"); }
        }

        public double SectionHeight
        {
            get { return sectionHeight; }
            set { sectionHeight = AssignNotify(ref sectionHeight, value, "SectionHeight"); }
        }

        public double SectionLeft
        {
            get { return sectionLeft; }
            set { sectionLeft = AssignNotify(ref sectionLeft, value, "SectionLeft"); }
        }

        public double SectionTop
        {
            get { return sectionTop; }
            set { sectionTop = AssignNotify(ref sectionTop, value, "SectionTop"); }
        }
        #endregion

    }

    public class RptTitle : DataItem
    {
        private int reportTitleID;
        private int reportID;
        private double height;

        public int ReportTitleID
        {
            get { return reportTitleID; }
            set
            {
                reportTitleID = AssignNotify(ref reportTitleID, value, "ReportTitleID");
                ID = reportTitleID;
                PrimaryKey = reportTitleID;
            }
        }

        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID2 = reportID;
            }
        }

        public double Height
        {
            get { return height; }
            set { height = AssignNotify(ref height, value, "Height"); }
        }

    }

    public class RptTitleItem : DataItem
    {
        private int reportTitleID;
        private int positionItemID;

        public int ReportTitleID
        {
            get { return reportTitleID; }
            set
            {
                reportTitleID = AssignNotify(ref reportTitleID, value, "ReportTitleID");
                ID = reportTitleID;
                PrimaryKey = reportTitleID;
            }
        }

        public int PositionItemID
        {
            get { return positionItemID; }
            set
            {
                positionItemID = AssignNotify(ref positionItemID, value, "PositionItemID");
                ID2 = positionItemID;
            }
        }

    }

    public class RptGroupHeaderFooter : DataItem
    {
        #region private fields

        private int groupHeaderFooterID;
        private int groupID;
        private double height;
        private bool isHeader;

        #endregion

        #region properties

        public int GroupHeaderFooterID
        {
            get { return groupHeaderFooterID; }
            set
            {
                groupHeaderFooterID = AssignNotify(ref groupHeaderFooterID, value, "GroupHeaderFooterID");
                ID = groupHeaderFooterID;
                PrimaryKey = groupHeaderFooterID;
            }
        }

        public int GroupID
        {
            get { return groupID; }
            set
            {
                groupID = AssignNotify(ref groupID, value, "GroupID");
                ID2 = groupID;
            }
        }

        public double Height
        {
            get { return height; }
            set { height = AssignNotify(ref height, value, "Height"); }
        }

        public bool IsHeader
        {
            get { return isHeader; }
            set { isHeader = AssignNotify(ref isHeader, value, "IsHeader"); }
        }

        #endregion

    }

    public class RptGroupHeaderFooterItem : DataItem
    {
        private int groupHeaderFooterID;
        private int positionItemID;

        public int GroupHeaderFooterID
        {
            get { return groupHeaderFooterID; }
            set
            {
                groupHeaderFooterID = AssignNotify(ref groupHeaderFooterID, value, "GroupHeaderFooterID");
                ID = groupHeaderFooterID;
                PrimaryKey = groupHeaderFooterID;
            }
        }

        public int PositionItemID
        {
            get { return positionItemID; }
            set
            {
                positionItemID = AssignNotify(ref positionItemID, value, "PositionItemID");
                ID2 = positionItemID;
            }
        }

    }

    public class RptPageHeaderFooter : DataItem
    {
        private int pageHeaderFooterID;
        private int reportID;
        private double height;
        private bool isHeader;
        private bool firstPage = false;
        private bool oddPages = true;
        private bool evenPages = true;

        public int PageHeaderFooterID
        {
            get { return pageHeaderFooterID; }
            set
            {
                pageHeaderFooterID = AssignNotify(ref pageHeaderFooterID, value, "PageHeaderFooterID");
                ID = pageHeaderFooterID;
                PrimaryKey = pageHeaderFooterID;
            }
        }

        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID2 = reportID;
            }
        }

        public double Height
        {
            get { return height; }
            set { height = AssignNotify(ref height, value, "Height"); }
        }

        public bool IsHeader
        {
            get { return isHeader; }
            set { isHeader = AssignNotify(ref isHeader, value, "IsHeader"); }
        }

        public bool FirstPage
        {
            get { return firstPage; }
            set
            {
                firstPage = AssignNotify(ref firstPage, value, "FirstPage");
                if (value)
                {
                    OddPages = false;
                    EvenPages = false;
                }
            }
        }

        public bool OddPages
        {
            get { return oddPages; }
            set { oddPages = AssignNotify(ref oddPages, value, "OddPages"); }
        }

        public bool EvenPages
        {
            get { return evenPages; }
            set { evenPages = AssignNotify(ref evenPages, value, "EvenPages"); }
        }

        public bool AllPages
        {
            get { return oddPages && evenPages &&!firstPage; }
            set
            {
                if (value)
                {
                    OddPages = true;
                    EvenPages = true;
                    FirstPage = false;
                }
            }
        }
    }

    public class RptPageHeaderFooterItem : DataItem
    {
        private int pageHeaderFooterID;
        private int positionItemID;

        public int PageHeaderFooterID
        {
            get { return pageHeaderFooterID; }
            set
            {
                pageHeaderFooterID = AssignNotify(ref pageHeaderFooterID, value, "PageHeaderFooterID");
                ID = pageHeaderFooterID;
                PrimaryKey = pageHeaderFooterID;
            }
        }

        public int PositionItemID
        {
            get { return positionItemID; }
            set
            {
                positionItemID = AssignNotify(ref positionItemID, value, "PositionItemID");
                ID2 = positionItemID;
            }
        }

    }

    public class RptPage : DataItem
    {
        private int reportPageID;
        private int reportID;
        private string pageSize;
        private int orientation;
        private double leftMargin;
        private double rightMargin;
        private double topMargin;
        private double bottomMargin;

        public int ReportPageID
        {
            get { return reportPageID; }
            set
            {
                reportPageID = AssignNotify(ref reportPageID, value, "ReportPageID");
                ID = reportPageID;
                PrimaryKey = reportPageID;
            }
        }

        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID2 = reportID;
            }
        }

        public string PageSize
        {
            get { return pageSize; }
            set { pageSize = AssignNotify(ref pageSize, value, "PageSize"); }
        }

        public int Orientation
        {
            get { return orientation; }
            set { orientation = AssignNotify(ref orientation, value, "Orientation"); }
        }

        public double LeftMargin
        {
            get { return leftMargin; }
            set { leftMargin = AssignNotify(ref leftMargin, value, "LeftMargin"); }
        }

        public double RightMargin
        {
            get { return rightMargin; }
            set { rightMargin = AssignNotify(ref rightMargin, value, "RightMargin"); }
        }

        public double TopMargin
        {
            get { return topMargin; }
            set { topMargin = AssignNotify(ref topMargin, value, "TopMargin"); }
        }

        public double BottomMargin
        {
            get { return bottomMargin; }
            set { bottomMargin = AssignNotify(ref bottomMargin, value, "BottomMargin"); }
        }



    }

    public class RptTextStyle : DataItem
    {
        private int textStyleID;
        private string styleName;
        private string fontName;
        private double fontSize;
        private int fontStyle;
        private int fontColour;
        private int horizontalJustification;
        private int verticalJustification;

        public int TextStyleID
        {
            get { return textStyleID; }
            set
            {
                textStyleID = AssignNotify(ref textStyleID, value, "TextStyleID");
                ID = textStyleID;
                PrimaryKey = textStyleID;
            }
        }

        public string StyleName
        {
            get { return styleName; }
            set
            {
                styleName = AssignNotify(ref styleName, value, "StyleName");
                ItemName = styleName;
            }
        }

        public string FontName
        {
            get { return fontName; }
            set { fontName = AssignNotify(ref fontName, value, "FontName"); }
        }

        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = AssignNotify(ref fontSize, value, "FontSize"); }
        }

        public int FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = AssignNotify(ref fontStyle, value, "FontStyle"); }
        }

        public int FontColour
        {
            get { return fontColour; }
            set { fontColour = AssignNotify(ref fontColour, value, "FontColour"); }
        }

        public int HorizontalJustification
        {
            get { return horizontalJustification; }
            set { horizontalJustification = AssignNotify(ref horizontalJustification, value, "HorizontalJustification"); }
        }

        public int VerticalJustification
        {
            get { return verticalJustification; }
            set { verticalJustification = AssignNotify(ref verticalJustification, value, "VerticalJustification"); }
        }

    }

    public class RptPositionItem : DataItem
    {
        private int positionItemID;
        private int reportItemID;
        private int textStyleID;
        private double itemWidth;
        private double itemHeight;
        private double itemLeft;
        private double itemTop;
        private double itemAlignPointX;
        private double itemAlignPointY;

        public int PositionItemID
        {
            get { return positionItemID; }
            set
            {
                positionItemID = AssignNotify(ref positionItemID, value, "PositionItemID");
                ID = positionItemID;
                PrimaryKey = positionItemID;
            }
        }

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID2 = reportItemID;
            }
        }

        public int TextStyleID
        {
            get { return textStyleID; }
            set { textStyleID = AssignNotify(ref textStyleID, value, "TextStyleID"); }
        }

        public double ItemWidth
        {
            get { return itemWidth; }
            set { itemWidth = AssignNotify(ref itemWidth, value, "ItemWidth"); }
        }

        public double ItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = AssignNotify(ref itemHeight, value, "ItemHeight"); }
        }

        public double ItemLeft
        {
            get { return itemLeft; }
            set { itemLeft = AssignNotify(ref itemLeft, value, "ItemLeft"); }
        }

        public double ItemTop
        {
            get { return itemTop; }
            set { itemTop = AssignNotify(ref itemTop, value, "ItemTop"); }
        }

        public double ItemAlignPointX
        {
            get { return itemAlignPointX; }
            set { itemAlignPointX = AssignNotify(ref itemAlignPointX, value, "ItemAlignPointX"); }
        }

        public double ItemAlignPointY
        {
            get { return itemAlignPointY; }
            set { itemAlignPointY = AssignNotify(ref itemAlignPointY, value, "ItemAlignPointY"); }
        }



    }
     
	#endregion    
    
    #region Report Data

    public class RptItem : DataItem
    {
        private int reportItemID;
        private int reportID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int ReportID
        {
            get { return reportID; }
            set
            {
                reportID = AssignNotify(ref reportID, value, "ReportID");
                ID2 = reportID;
            }
        }

    }

    public class RptStandardFunction : DataItem
    {
        private int standardFunctionID;
        private string standardFunctionName;

        public int StandardFunctionID
        {
            get { return standardFunctionID; }
            set
            {
                standardFunctionID = AssignNotify(ref standardFunctionID, value, "StandardFunctionID");
                ID = standardFunctionID;
                PrimaryKey = standardFunctionID;
            }
        }

        public string StandardFunctionName
        {
            get { return standardFunctionName; }
            set
            {
                standardFunctionName = AssignNotify(ref standardFunctionName, value, "StandardFunctionName");
                ItemName = standardFunctionName;
            }
        }

    }

    public class RptItemStandardFunction : DataItem
    {
        private int reportItemID;
        private int standardFunctionID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int StandardFunctionID
        {
            get { return standardFunctionID; }
            set
            {
                standardFunctionID = AssignNotify(ref standardFunctionID, value, "StandardFunctionID");
                ID2 = standardFunctionID;
            }
        }
    }

    public class RptTextItem : DataItem
    {
        private int textItemID;
        //private string itemName;
        private string content;
        private bool canTranslate;

        public int TextItemID
        {
            get { return textItemID; }
            set
            {
                textItemID = AssignNotify(ref textItemID, value, "TextItemID");
                ID = textItemID;
                PrimaryKey = textItemID;
            }
        }

        public string Content
        {
            get { return content; }
            set { content = AssignNotify(ref content, value, "Content"); }
        }

        public bool CanTranslate
        {
            get { return canTranslate; }
            set { canTranslate = AssignNotify(ref canTranslate, value, "CanTranslate"); }
        }

    }

    public class RptItemText : DataItem
    {
        private int reportItemID;
        private int textItemID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int TextItemID
        {
            get { return textItemID; }
            set
            {
                textItemID = AssignNotify(ref textItemID, value, "TextItemID");
                ID2 = textItemID;
            }
        }
    }

    public class RptTextDataItem : DataItem
    {
        private int textDataItemID;
        //private string itemName;
        private int dataItemID;

        public int TextDataItemID
        {
            get { return textDataItemID; }
            set
            {
                textDataItemID = AssignNotify(ref textDataItemID, value, "TextDataItemID");
                ID = textDataItemID;
                PrimaryKey = textDataItemID;
            }
        }

        public int DataItemID
        {
            get { return dataItemID; }
            set
            {
                dataItemID = AssignNotify(ref dataItemID, value, "DataItemID");
                ID2 = dataItemID;
            }
        }

    }

    public class RptItemTextData : DataItem
    {
        private int reportItemID;
        private int textDataItemID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int TextDataItemID
        {
            get { return textDataItemID; }
            set
            {
                textDataItemID = AssignNotify(ref textDataItemID, value, "TextDataItemID");
                ID2 = textDataItemID;
            }
        }
    }

    public class RptNumericDataItem : DataItem
    {
        private int numericDataItemID;
        //private string itemName;
        private int dataItemID;

        public int NumericDataItemID
        {
            get { return numericDataItemID; }
            set
            {
                numericDataItemID = AssignNotify(ref numericDataItemID, value, "NumericDataItemID");
                ID = numericDataItemID;
                PrimaryKey = numericDataItemID;
            }
        }

        public int DataItemID
        {
            get { return dataItemID; }
            set
            {
                dataItemID = AssignNotify(ref dataItemID, value, "DataItemID");
                ID2 = dataItemID;
            }
        }

    }

    public class RptItemNumericData : DataItem
    {
        private int reportItemID;
        private int numericDataItemID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int NumericDataItemID
        {
            get { return numericDataItemID; }
            set
            {
                numericDataItemID = AssignNotify(ref numericDataItemID, value, "NumericDataItemID");
                ID2 = numericDataItemID;
            }
        }
    }

    public class RptDataItem : DataItem
    {
        private int dataItemID;
        //private string itemName;
        private string bindingName;

        public int DataItemID
        {
            get { return dataItemID; }
            set
            {
                dataItemID = AssignNotify(ref dataItemID, value, "DataItemID");
                ID = dataItemID;
                PrimaryKey = dataItemID;
            }
        }

        public string BindingName
        {
            get { return bindingName; }
            set { bindingName = AssignNotify(ref bindingName, value, "BindingName"); }
        }

    }

    public class RptImageItem : DataItem
    {
        private int imageItemID;
        private byte[] imageData;

        public int ImageItemID
        {
            get { return imageItemID; }
            set
            {
                imageItemID = AssignNotify(ref imageItemID, value, "ImageItemID");
                ID = imageItemID;
                PrimaryKey = imageItemID;
            }
        }

        public byte[] ImageData
        {
            get { return imageData; }
            set { imageData = AssignNotify(ref imageData, value, "ImageData"); }
        }

    }

    public class RptItemImages : DataItem
    {
        private int reportItemID;
        private int imageItemID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int ImageItemID
        {
            get { return imageItemID; }
            set
            {
                imageItemID = AssignNotify(ref imageItemID, value, "ImageItemID");
                ID2 = imageItemID;
            }
        }
    }

    public class RptImageDataItem : DataItem
    {
        private int imageDataItemID;
        //private string itemName;
        private int dataItemID;

        public int ImageDataItemID
        {
            get { return imageDataItemID; }
            set
            {
                imageDataItemID = AssignNotify(ref imageDataItemID, value, "ImageDataItemID");
                ID = imageDataItemID;
                PrimaryKey = imageDataItemID;
            }
        }

        public int DataItemID
        {
            get { return dataItemID; }
            set
            {
                dataItemID = AssignNotify(ref dataItemID, value, "DataItemID");
                ID2 = dataItemID;
            }
        }

    }

    public class RptItemImageData : DataItem
    {
        private int reportItemID;
        private int imageDataItemID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int ImageDataItemID
        {
            get { return imageDataItemID; }
            set
            {
                imageDataItemID = AssignNotify(ref imageDataItemID, value, "ImageDataItemID");
                ID2 = imageDataItemID;
            }
        }
    }

    public class RptAggregateFunction : DataItem
    {
        private int aggregateID;
        //private string itemName;
        private int aggregateType;
        private int dataItemID;

        public int AggregateID
        {
            get { return aggregateID; }
            set
            {
                aggregateID = AssignNotify(ref aggregateID, value, "AggregateID");
                ID = aggregateID;
                PrimaryKey = aggregateID;
            }
        }

        public int DataItemID
        {
            get { return dataItemID; }
            set
            {
                dataItemID = AssignNotify(ref dataItemID, value, "DataItemID");
                ID2 = dataItemID;
            }
        }

        public int AggregateType
        {
            get { return aggregateType; }
            set { aggregateType = AssignNotify(ref aggregateType, value, "AggregateType"); }
        }

    }

    public class RptItemAggregate : DataItem
    {
        private int reportItemID;
        private int aggregateID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int AggregateID
        {
            get { return aggregateID; }
            set
            {
                aggregateID = AssignNotify(ref aggregateID, value, "AggregateID");
                ID2 = aggregateID;
            }
        }
    }

    public class RptFunctionItem : DataItem
    {
        private int functionID;
        //private string itemName;
        private string definition;

        public int FunctionID
        {
            get { return functionID; }
            set
            {
                functionID = AssignNotify(ref functionID, value, "FunctionID");
                ID = functionID;
                PrimaryKey = functionID;
            }
        }

        public string Definition
        {
            get { return definition; }
            set { definition = AssignNotify(ref definition, value, "Definition"); }
        }

    }

    public class RptItemFunction : DataItem
    {
        private int reportItemID;
        private int functionID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int FunctionID
        {
            get { return functionID; }
            set
            {
                functionID = AssignNotify(ref functionID, value, "FunctionID");
                ID2 = functionID;
            }
        }
    }

    public class RptFunctionFieldItem : DataItem
    {
        private int reportItemID;
        private int functionID;
        private int itemIndex;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int FunctionID
        {
            get { return functionID; }
            set
            {
                functionID = AssignNotify(ref functionID, value, "FunctionID");
                ID2 = functionID;
            }
        }

        public int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = AssignNotify(ref itemIndex, value, "ItemIndex"); }
        }

    }

    public class RptChartItem : DataItem
    {
        private int chartID;
        //private string itemName;
        private int chartType;

        public int ChartID
        {
            get { return chartID; }
            set
            {
                chartID = AssignNotify(ref chartID, value, "ChartID");
                ID = chartID;
                PrimaryKey = chartID;
            }
        }

        public int ChartType
        {
            get { return chartType; }
            set { chartType = AssignNotify(ref chartType, value, "ChartType"); }
        }

    }

    public class RptItemChart : DataItem
    {
        private int reportItemID;
        private int chartID;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int ChartID
        {
            get { return chartID; }
            set
            {
                chartID = AssignNotify(ref chartID, value, "ChartID");
                ID2 = chartID;
            }
        }
    }

    public class RptChartFieldItem : DataItem
    {
        private int reportItemID;
        private int chartID;
        private int itemIndex;

        public int ReportItemID
        {
            get { return reportItemID; }
            set
            {
                reportItemID = AssignNotify(ref reportItemID, value, "ReportItemID");
                ID = reportItemID;
                PrimaryKey = reportItemID;
            }
        }

        public int ChartID
        {
            get { return chartID; }
            set
            {
                chartID = AssignNotify(ref chartID, value, "ChartID");
                ID2 = chartID;
            }
        }

        public int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = AssignNotify(ref itemIndex, value, "ItemIndex"); }
        }

    }


    #endregion
    
    #endregion

    #region Timekeeping

    #region Breaks

    partial class SqlDataAccess
    {
        //public TimekeepingBreaks GetAllBreaks()
        //{
        //    return GetAllBreaks(false);
        //}

        //public TimekeepingBreaks GetAllBreaks(bool noCacheRead)
        //{
        //    TimekeepingBreaks recs = new TimekeepingBreaks();
        //    recs = (TimekeepingBreaks)DBDataListSelect(recs, noCacheRead, !noCacheRead);
        //    return recs;
        //}

        public TimekeepingBreaks GetAllBreaks(TimekeepingBreaks recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new TimekeepingBreaks();
            recs = (TimekeepingBreaks)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public TimekeepingBreaks GetAllBreaks(TimekeepingBreaks recs)
        {
            return GetAllBreaks(recs, false);
        }

        //public TimekeepingBreak GetBreak(int id)
        //{
        //    TimekeepingBreak ret = null;
        //    TimekeepingBreaks recs = GetAllBreaks();
        //    if (recs != null)
        //    {
        //        ret = (TimekeepingBreak)recs.GetById(id);
        //    }
        //    return ret;
        //}
    }

    public class TimekeepingBreaks : DataList
    {
        public TimekeepingBreaks()
        {
            Lifespan = 1.0;
            ListType = typeof(TimekeepingBreak);
            TblName = "tblTimekeepingBreaks";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("BreakID", "BreakID", true, false);
            dBFieldMappings.AddMapping("BreakName", "BreakName");
            dBFieldMappings.AddMapping("Duration", "Duration");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void AddBreak()
        {
            TimekeepingBreak rec = new TimekeepingBreak();
            rec.BreakID = GetNextID();
            rec.BreakName = "New Break " + rec.BreakID;
            rec.Duration = 0;
            rec.ForceNew = true;
            this.Add(rec);
        }

    }

    public class TimekeepingBreak : DataItem
    {
        #region private fields

        private int breakID;
        private string breakName;
        private int duration;

        #endregion

        #region properties

        public int BreakID
        {
            get { return breakID; }
            set
            {
                breakID = AssignNotify(ref breakID, value, "BreakID");
                ID = breakID;
                PrimaryKey = breakID;
            }
        }
        public string BreakName
        {
            get { return breakName; }
            set
            {
                breakName = AssignNotify(ref breakName, value, "BreakName");
                ItemName = breakName;
            }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = AssignNotify(ref duration, value, "Duration"); }
        }



        #endregion

        public TimekeepingBreak()
            : base()
        {
            breakID = -1;
            breakName = string.Empty;
            duration = 0;
        }
    }

    #endregion

    #region TimekeepingKioskItems

    partial class SqlDataAccess
    {
        //public TimekeepingKioskItems GetAllTimekeepingKioskItems()
        //{
        //    return GetAllTimekeepingKioskItems(false);
        //}

        //public TimekeepingKioskItems GetAllTimekeepingKioskItems(bool noCacheRead)
        //{
        //    TimekeepingKioskItems recs = new TimekeepingKioskItems();
        //    recs = (TimekeepingKioskItems)DBDataListSelect(recs, noCacheRead, !noCacheRead);
        //    return recs;
        //}

        public TimekeepingKioskItems GetAllTimekeepingKioskItems(TimekeepingKioskItems recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new TimekeepingKioskItems();
            recs = (TimekeepingKioskItems)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public TimekeepingKioskItems GetAllTimekeepingKioskItems(TimekeepingKioskItems recs)
        {
            return GetAllTimekeepingKioskItems(recs, false);
        }

        //public TimekeepingKioskItem GetTimekeepingKioskItem(int id)
        //{
        //    TimekeepingKioskItem ret = null;
        //    TimekeepingKioskItems recs = GetAllTimekeepingKioskItems();
        //    if (recs != null)
        //    {
        //        ret = (TimekeepingKioskItem)recs.GetById(id);
        //    }
        //    return ret;
        //}
    }

    public class TimekeepingKioskItems : DataList
    {
        public TimekeepingKioskItems()
        {
            Lifespan = 1.0;
            ListType = typeof(TimekeepingKioskItem);
            TblName = "tblTimekeepingKioskItems";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("KioskItemID", "KioskItemID", true, false);
            dBFieldMappings.AddMapping("KioskID", "KioskID");
            dBFieldMappings.AddMapping("PositionID", "PositionID");
            dBFieldMappings.AddMapping("SubRegtype", "SubRegtype");
            dBFieldMappings.AddMapping("ItemID", "ItemID");
            dBFieldMappings.AddMapping("ItemSubID", "ItemSubID");
            dBFieldMappings.AddMapping("Caption", "Caption");
            dBFieldMappings.AddMapping("StartImageID", "StartImageID");
            dBFieldMappings.AddMapping("EndImageID", "EndImageID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void AddItem()
        {
            TimekeepingKioskItem rec = new TimekeepingKioskItem();
            rec.KioskItemID = GetNextID();
            rec.KioskID = 1;
            rec.PositionID = 1;
            rec.SubRegtype = 6;
            rec.ItemID = 1;
            rec.ItemSubID = -1;
            rec.Caption = "New Item " + rec.KioskItemID;
            rec.StartImageID = 0;
            rec.EndImageID = 0;
            rec.ForceNew = true;
            this.Add(rec);
        }

        public TimekeepingKioskItem GetByPosition(int kioskid, int positionid)
        {
            TimekeepingKioskItem item = null;
            int i = 0;
            while ((item == null) && (i < this.Count))
            {
                TimekeepingKioskItem rec = (TimekeepingKioskItem)this[i];
                if (rec != null)
                {
                    if ((rec.PositionID == positionid)
                        && ((rec.SubRegtype == 6) || (rec.KioskID == kioskid)))
                    {
                        item = rec;
                    }
                }
                i++;
            }
            return item;
        }

        public TimekeepingKioskItem GetByMachineSub(int kioskid, int machineid, int subid)
        {
            TimekeepingKioskItem item = null;
            int i = 0;
            while ((item == null) && (i < this.Count))
            {
                TimekeepingKioskItem rec = (TimekeepingKioskItem)this[i];
                if (rec != null)
                {
                    if ((rec.KioskID==kioskid)
                        && (rec.MachineID== machineid) 
                        && (rec.SubID == subid))
                    {
                        item = rec;
                    }
                }
                i++;
            }
            return item;
        }

    }

    public class TimekeepingKioskItem : DataItem
    {
        #region private fields

        private int kioskItemID;
        private int kioskID;
        private int positionID;
        private int subRegtype;
        private int itemID;
        private int itemSubID;
        private string caption = string.Empty;
        private int startImageID;
        private int endImageID;
        private string message = string.Empty;
        private Brush messageColor = Brushes.White;

        private ResourceImage startTkImage;
        private ResourceImage endTkImage;
        private ResourceImages resourceImages;

        #endregion

        #region Properties

        public int KioskItemID
        {
            get { return kioskItemID; }
            set
            {
                kioskItemID = AssignNotify(ref kioskItemID, value, "KioskItemID");
                ID = kioskItemID;
                PrimaryKey = kioskItemID;
            }
        }

        public int KioskID
        {
            get { return kioskID; }
            set { kioskID = AssignNotify(ref kioskID, value, "KioskID"); }
        }
        public int PositionID
        {
            get { return positionID; }
            set { positionID = AssignNotify(ref positionID, value, "PositionID"); }
        }
        public int SubRegtype
        {
            get { return subRegtype; }
            set { subRegtype = AssignNotify(ref subRegtype, value, "SubRegtype"); }
        }
        public int ItemID
        {
            get { return itemID; }
            set { itemID = AssignNotify(ref itemID, value, "ItemID"); }
        }
        public int ItemSubID
        {
            get { return itemSubID; }
            set { itemSubID = AssignNotify(ref itemSubID, value, "ItemSubID"); }
        }
        public string Message
        {
            get { return message; }
            set { message = AssignNotify(ref message, value, "Message"); }
        }
        public Brush MessageColor
        {
            get { return messageColor; }
            set { messageColor = AssignNotify(ref messageColor, value, "MessageColor"); }
        }
        public int SubRegtypeID
        {
            get
            {
                int srtID = 0;
                if (subRegtype == 6)
                    srtID = ItemID;
                return srtID;
            }
        }
        public int MachineID
        {
            get
            {
                int mid = -1;
                if (subRegtype == 0)
                    mid = ItemID;
                return mid;
            }
        }
        public int SubID
        {
            get { return itemSubID; }
        }
        public string Caption
        {
            get { return caption; }
            set { caption = AssignNotify(ref caption, value, "Caption"); }
        }
        private ResourceImage getResourceImage(int id)
        {
            ResourceImage rec = null;
            if (resourceImages == null)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                resourceImages = new ResourceImagesUtils();
                resourceImages = (ResourceImagesUtils)da.GetAllResourceImages(resourceImages, false);
            }
            rec = (ResourceImage)resourceImages.GetById(id);
            return rec;
        }

        private string getImageName(ResourceImage rec)
        {
            string name = "No Image";
            if (rec != null)
            {
                name = rec.ImageName;
            }
            return name;
        }

        private ResourceImage getImageFromName(string aname)
        {
            ResourceImage rec = null;
            if (resourceImages != null)
            {
                rec = (ResourceImage)resourceImages.GetByName(aname);
            }
            return rec;
        }

        public int StartImageID
        {
            get { return startImageID; }
            set
            {
                if (value != startImageID)
                {
                    startImageID = AssignNotify(ref startImageID, value, "StartImageID");
                    startTkImage = getResourceImage(startImageID);
                }
            }
        }

        public int EndImageID
        {
            get { return endImageID; }
            set
            {
                if (value != endImageID)
                {
                    endImageID = AssignNotify(ref endImageID, value, "EndImageID");
                    endTkImage = getResourceImage(endImageID);
                }
            }
        }

        public string StartImageName
        {
            get { return getImageName(startTkImage); }
            set
            {
                ResourceImage ri = getImageFromName(value);
                if ((ri != null) && (ri.ResourceImageID != startImageID))
                {
                    startTkImage = ri;
                    startImageID = ri.ResourceImageID;
                    Notify("StartImageName");
                    Notify("StartImageID");
                }
            }
        }
        public string EndImageName
        {
            get { return getImageName(endTkImage); }
            set
            {
                ResourceImage ri = getImageFromName(value);
                if ((ri != null) && (ri.ResourceImageID != startImageID))
                {
                    endTkImage = ri;
                    endImageID = ri.ResourceImageID;
                    Notify("EndImageName");
                    Notify("EndImageID");
                }
            }
        }

        public ImageSource StartImageSource
        {
            get
            {
                ImageSource imgSrc = null;
                if (startTkImage != null)
                {
                    imgSrc = startTkImage.ImageDataSource;
                }
                return imgSrc;
            }
        }

        public ImageSource EndImageSource
        {
            get
            {
                ImageSource imgSrc = null;
                if (endTkImage != null)
                {
                    imgSrc = endTkImage.ImageDataSource;
                }
                return imgSrc;
            }
        }

        #endregion
    }

    #endregion    

    #region Images


    public class ResourceImagesUtils : ResourceImages
    {
        public ResourceImagesUtils()
            : base()
        {
            DbName = "JEGR_Utils";
        }
    }

 
    #endregion
 
    #endregion

    #region emails

    public class EmailConfigs : DataList
    {
        public EmailConfigs()
        {
            Lifespan = 1.0;
            ListType = typeof(EmailConfig);
            TblName = "tblEmailConfigs";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("EmailConfigID", "EmailConfigID", true, false);
            dBFieldMappings.AddMapping("ConfigName", "ConfigName");
            dBFieldMappings.AddMapping("Host", "Host");
            dBFieldMappings.AddMapping("Port", "Port");
            dBFieldMappings.AddMapping("TimeoutmS", "TimeoutmS");
            dBFieldMappings.AddMapping("UserName", "UserName");
            dBFieldMappings.AddMapping("Password", "Password");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class EmailConfig : DataItem
    {
        #region private fields

        private int emailConfigID;
        private string configName;
        private string host;
        private int port;
        private int timeoutmS=10000;
        private string userName;
        private string password;

        #endregion

        #region properties

        public int EmailConfigID
        {
            get { return emailConfigID; }
            set
            {
                emailConfigID = AssignNotify(ref emailConfigID, value, "EmailConfigID");
                ID = emailConfigID;
                PrimaryKey = emailConfigID;
            }
        }

        public string ConfigName
        {
            get { return configName; }
            set
            {
                configName = AssignNotify(ref configName, value, "ConfigName");
                ItemName = configName;
            }
        }

        public string Host
        {
            get { return host; }
            set { host = AssignNotify(ref host, value, "Host"); }
        }

        public int Port
        {
            get { return port; }
            set { port = AssignNotify(ref port, value, "Port"); }
        }

        public int TimeoutmS
        {
            get { return timeoutmS; }
            set { timeoutmS = AssignNotify(ref timeoutmS, value, "TimeoutmS"); }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = AssignNotify(ref userName, value, "UserName"); }
        }

        public string Password
        {
            get { return password; }
            set { password = AssignNotify(ref password, value, "Password"); }
        }



        #endregion

    }

    public class EmailAttachments : DataList
    {
        public EmailAttachments()
        {
            Lifespan = 1.0;
            ListType = typeof(EmailAttachment);
            TblName = "tblEmailAttachments";
            DbName = "JEGR_Utils";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AttachmentID", "AttachmentID", true, false);
            dBFieldMappings.AddMapping("AttachmentName", "AttachmentName");
            dBFieldMappings.AddMapping("AttachmentType", "AttachmentType");
            dBFieldMappings.AddMapping("AttachmentSource", "AttachmentSource");
            dBFieldMappings.AddMapping("AttachmentIndex", "AttachmentIndex");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class EmailAttachment : DataItem
    {
        private int attachmentID;
        private string attachmentName;
        private int attachmentType;
        private string attachmentSource;
        private int attachmentIndex;

        public int AttachmentID
        {
            get { return attachmentID; }
            set
            {
                attachmentID = AssignNotify(ref attachmentID, value, "AttachmentID");
                ID = attachmentID;
                PrimaryKey = attachmentID;
            }
        }

        public string AttachmentName
        {
            get { return attachmentName; }
            set
            {
                attachmentName = AssignNotify(ref attachmentName, value, "AttachmentName");
                ItemName = attachmentName;
            }
        }

        public int AttachmentType
        {
            get { return attachmentType; }
            set { attachmentType = AssignNotify(ref attachmentType, value, "AttachmentType"); }
        }

        public string AttachmentSource
        {
            get { return attachmentSource; }
            set { attachmentSource = AssignNotify(ref attachmentSource, value, "AttachmentSource"); }
        }

        public int AttachmentIndex
        {
            get { return attachmentIndex; }
            set { attachmentIndex = AssignNotify(ref attachmentIndex, value, "AttachmentIndex"); }
        }

    
    }



    public class EmailAttachmentLookup : DataItem
    {
        private int attachmentID;
        private int emailID;

        public int AttachmentID
        {
            get { return attachmentID; }
            set
            {
                attachmentID = AssignNotify(ref attachmentID, value, "AttachmentID");
                ID = attachmentID;
            }
        }

        public int EmailID
        {
            get { return emailID; }
            set
            {
                emailID = AssignNotify(ref emailID, value, "EmailID");
                ID2 = emailID;
            }
        }
    }

    public class Email : DataItem
    {
        private int emailID;
        private string subject;
        private DateTime creationTimestamp;
        private string createdBy;
        private string createdReason;
        private int creationSourceID;
        private string emailTo;
        private string emailFrom;
        private string emailCC;
        private string emailBCC;
        private string message;
        private int status;
        private DateTime sendScheduleTime;
        private DateTime sendTimestamp;
        private string sendResult;

        public int EmailID
        {
            get { return emailID; }
            set
            {
                emailID = AssignNotify(ref emailID, value, "EmailID");
                ID = emailID;
                PrimaryKey = emailID;
            }
        }

        public string Subject
        {
            get { return subject; }
            set
            {
                subject = AssignNotify(ref subject , value, "");
                ItemName = subject;
            }
        }     

        public DateTime CreationTimestamp
        {
            get { return creationTimestamp; }
            set { creationTimestamp = AssignNotify(ref creationTimestamp, value, "CreationTimestamp"); }
        }

        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = AssignNotify(ref createdBy, value, "CreatedBy"); }
        }

        public string CreatedReason
        {
            get { return createdReason; }
            set { createdReason = AssignNotify(ref createdReason, value, "CreatedReason"); }
        }

        public int CreationSourceID
        {
            get { return creationSourceID; }
            set { creationSourceID = AssignNotify(ref creationSourceID, value, "CreationSourceID"); }
        }

        public string EmailTo
        {
            get { return emailTo; }
            set { emailTo = AssignNotify(ref emailTo, value, "EmailTo"); }
        }

        public string EmailFrom
        {
            get { return emailFrom; }
            set { emailFrom = AssignNotify(ref emailFrom, value, "EmailFrom"); }
        }

        public string EmailCC
        {
            get { return emailCC; }
            set { emailCC = AssignNotify(ref emailCC, value, "EmailCC"); }
        }

        public string EmailBCC
        {
            get { return emailBCC; }
            set { emailBCC = AssignNotify(ref emailBCC, value, "EmailBCC"); }
        }

        public string Message
        {
            get { return message; }
            set { message = AssignNotify(ref message, value, "Message"); }
        }

        public int Status
        {
            get { return status; }
            set { status = AssignNotify(ref status, value, "Status"); }
        }

        public DateTime SendScheduleTime
        {
            get { return sendScheduleTime; }
            set { sendScheduleTime = AssignNotify(ref sendScheduleTime, value, "SendScheduleTime"); }
        }

        public DateTime SendTimestamp
        {
            get { return sendTimestamp; }
            set { sendTimestamp = AssignNotify(ref sendTimestamp, value, "SendTimestamp"); }
        }

        public string SendResult
        {
            get { return sendResult; }
            set { sendResult = AssignNotify(ref sendResult, value, "SendResult"); }
        }


    }

    public class EmailContact : DataItem
    {
        private int contactID;
        private string title;
        private string firstname;
        private string lastname;
        private string jobTitle;
        private string jobFunction;
        private string emailAddress;

        public int ContactID
        {
            get { return contactID; }
            set
            {
                contactID = AssignNotify(ref contactID, value, "ContactID");
                ID = contactID;
                PrimaryKey = contactID;
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = AssignNotify(ref title, value, "Title");
                ItemName = FullName;
            }
        }

        public string Firstname
        {
            get { return firstname; }
            set
            {
                firstname = AssignNotify(ref firstname, value, "Firstname");
                ItemName = FullName;
            }
        }
        public string Lastname
        {
            get { return lastname; }
            set { lastname = AssignNotify(ref lastname, value, "Lastname"); }
        }
        public string JobTitle
        {
            get { return jobTitle; }
            set
            {
                jobTitle = AssignNotify(ref jobTitle, value, "JobTitle");
                ItemName = FullName;
            }
        }
        public string JobFunction
        {
            get { return jobFunction; }
            set
            {
                jobFunction = AssignNotify(ref jobFunction, value, "JobFunction");
                ItemName = FullName;
            }
        }
        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = AssignNotify(ref emailAddress, value, "EmailAddress"); }
        }

        public string FullName { get { return getFullName(); } }

        public string PersonalSalutation
        {
            get { return getPersonalSalutation(); }
        }

        public string FormalSalutation
        {
            get { return getFormalSalutation(); }
        }

        public string FunctionSalutation
        {
            get { return getFunctionSalutation(); }
        }

        private string getFullName()
        {
            string fullname = title;
            if (title != string.Empty) fullname += " ";
            fullname += firstname;
            if (firstname != string.Empty) fullname += " ";
            firstname += lastname;
            Notify("FullName"); 
            return fullname;
        }

        private string getPersonalSalutation()
        {
            string str = string.Empty;
            if (firstname != string.Empty)
                str += firstname;
            else
                str += getFullName();
            return str;
        }

        private string getFormalSalutation()
        {
            string str=string.Empty;
            if ((title != string.Empty) && (lastname != string.Empty))
                str = title + " " + lastname;
            else
                str = getFullName();
            return str;
        }

        private string getFunctionSalutation()
        {
            string str = string.Empty;
            if (jobFunction != string.Empty)
                str = jobFunction;
            else
                str = jobTitle;
            if (str == string.Empty)
                str = getFormalSalutation();
            return str;
        }
    }

    #endregion




}
