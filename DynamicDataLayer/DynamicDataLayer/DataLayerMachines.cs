using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dynamic.DataLayer
{
    #region Machines


    partial class SqlDataAccess
    {
        public Machines GetAllMachines(Machines recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Machines();
            recs = (Machines)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public Machines GetAllMachinesNoExpire(Machines recs)
        {
            recs = GetAllMachines(recs, true);
            recs.AllowExpire = false;
            return recs;
        }

        public void ExpireAllMachineCaches()
        {
            DataList dataList=new Machines();
            foreach (CacheItem ci in cacheItems)
            {
                if (ci.CacheDataList is Machines)
                {
                    ci.CacheDataList.AllowExpire = false;
                    ci.CacheDataList.IsValid = false;
                }
            }
        }

        public MachineSubIDs GetMachineSubIDs(MachineSubIDs recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new MachineSubIDs();
            recs = (MachineSubIDs)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

    }
    
    public class Machines : DataList
    {
        public Machines()
        {
            Lifespan = 1.0;
            ListType = typeof(Machine);
            TblName = "tblMachines";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("IdJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("IP_Address", "IPAddress");
            dBFieldMappings.AddMapping("FTP_Path_CustArt", "FtpPathCustArt");
            dBFieldMappings.AddMapping("FTP_Path_Operators", "FtpPathOperators");
            dBFieldMappings.AddMapping("AllowCustomers", "AllowCustomers");
            dBFieldMappings.AddMapping("SendConfiguration", "SendConfiguration");
            dBFieldMappings.AddMapping("NormValue", "NormValue");
            dBFieldMappings.AddMapping("NormUnit_idJensen", "NormUnit");
            dBFieldMappings.AddMapping("MachineGroup_idJensen", "MachineGroupID");
            dBFieldMappings.AddMapping("PingStatus", "PingStatus");
            dBFieldMappings.AddMapping("Positions", "Positions");
            dBFieldMappings.AddMapping("ProcessCodeType", "ProcessCodeType");
            dBFieldMappings.AddMapping("CustArtUpdate", "CustArtUpdate");
            dBFieldMappings.AddMapping("OperatorUpdate", "OperatorUpdate");
            dBFieldMappings.AddMapping("BatchSizeFactor", "BatchSizeFactor");
            dBFieldMappings.AddMapping("ShowBatchText", "ShowBatchText");
            dBFieldMappings.AddMapping("UseMachineCount", "UseMachineCount");
            dBFieldMappings.AddMapping("MachineCountExitPoint", "MachineCountExitPoint");
            dBFieldMappings.AddMapping("CommandLine", "CommandLine");
            dBFieldMappings.AddMapping("CommandDescription", "CommandDescription");
            dBFieldMappings.AddMapping("BatchUpper", "BatchUpper");
            dBFieldMappings.AddMapping("BatchLower", "BatchLower");
            dBFieldMappings.AddMapping("NoFlow", "NoFlow");
            dBFieldMappings.AddMapping("OperatorCountExitPoint", "OperatorCountExitPoint");
            dBFieldMappings.AddMapping("ExcludeFromPowerCalc", "ExcludeFromPowerCalc");
            dBFieldMappings.AddMapping("ProductionReference", "ProductionReference");
            dBFieldMappings.AddMapping("ProductionGroupID", "ProductionGroupID");
            dBFieldMappings.AddMapping("Settings", "Settings");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    #region Machine class
    public class Machine : DataItem
    {
        #region Private fields

        private int recNum;
        private int idJensen;
        private string extRef;
        private string shortDescription;
        private string longDescription;
        private string ipAddress;
        private string ftpPathCustArt;
        private string ftpPathOperators;
        private bool allowCustomers;
        private bool sendConfiguration;
        private int normValue;
        private int oldNorm; //not stored
        private int normUnit;
        private int machineGroupID;
        private int productionGroupID = -1;
        private bool pingStatus;
        private int positions;
        private int processCodeType;
        private DateTime custArtUpdate = DateTime.MinValue;
        private DateTime operatorUpdate = DateTime.MinValue;
        private double batchSizeFactor;
        private double cockpitBatchSizeFactor;
        private double railBatchSizeFactor;
        private bool showBatchText;
        private bool useMachineCount;
        private bool machineCountExitPoint;
        private string commandLine;
        private string commandDescription;
        private int batchUpper;
        private int batchLower;
        private int noFlow;
        private int oldNoFlow; //not stored
        private bool operatorProductionCountExitPoint = false;//new! 
        private bool operatorTimeCountExitPoint = false;//new! 
        private bool operatorMultipleCountExitPoint = false;//new! 
        private bool excludeFromPowerCalc = false;
        private bool productionReference = false;
        private string settings = string.Empty; //new! holder for xml settings
        private bool overallMetering = false; //not stored populated from settings        
        
        
        #endregion


        #region properties
 
        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = value;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string MachineName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "MachineName"); }
        }

        public string MachineNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public string IPAddress
        {
            get { return ipAddress.Trim(); }
            set { ipAddress = AssignNotify(ref ipAddress, value, "IPAddress"); }
        }

        public string FtpPathCustArt
        {
            get { return ftpPathCustArt; }
            set { ftpPathCustArt = AssignNotify(ref ftpPathCustArt, value, "FtpPathCustArt"); }
        }

        public string FtpPathOperators
        {
            get { return ftpPathOperators; }
            set { ftpPathOperators = AssignNotify(ref ftpPathOperators, value, "FtpPathOperators"); }
        }

        public bool AllowCustomers
        {
            get { return allowCustomers; }
            set { allowCustomers = AssignNotify(ref allowCustomers, value, "AllowCustomers"); }
        }

        public bool SendConfiguration
        {
            get { return sendConfiguration; }
            set { sendConfiguration = AssignNotify(ref sendConfiguration, value, "SendConfiguration"); }
        }

        public int NormValue
        {
            get { return normValue; }
            set
            {
                normValue = AssignNotify(ref normValue, value, "NormValue");
                oldNorm = value;
            }
        }

        public bool NormChanged
        {
            get { return (!IsNew && HasChanged && (normValue != oldNorm)); }
        }

        public int OldNorm
        {
            get { return oldNorm; }
            set { oldNorm = value; }
        }

        public int NormUnit
        {
            get { return normUnit; }
            set { normUnit = AssignNotify(ref normUnit, value, "NormUnit"); }
        }

        public int MachineGroupID
        {
            get { return machineGroupID; }
            set { machineGroupID = AssignNotify(ref machineGroupID, value, "MachineGroupID"); }
        }

        public int ProductionGroupID
        {
            get { return productionGroupID; }
            set { productionGroupID = AssignNotify(ref productionGroupID, value, "ProductionGroupID"); }
        }

        public bool PingStatus
        {
            get { return pingStatus; }
            set { pingStatus = AssignNotify(ref pingStatus, value, "PingStatus"); }
        }

        public int Positions
        {
            get { return positions; }
            set { positions = AssignNotify(ref positions, value, "Positions"); }
        }

        public int ProcessCodeType
        {
            get { return processCodeType; }
            set { processCodeType = AssignNotify(ref processCodeType, value, "ProcessCodeType"); }
        }

        public DateTime CustArtUpdate
        {
            get { return custArtUpdate; }
            set { custArtUpdate = AssignNotify(ref custArtUpdate, value, "CustArtUpdate"); }
        }

        public DateTime OperatorUpdate
        {
            get { return operatorUpdate; }
            set { operatorUpdate = AssignNotify(ref operatorUpdate, value, "OperatorUpdate"); }
        }

        public double BatchSizeFactor
        {
            get { return batchSizeFactor; }
            set { batchSizeFactor = AssignNotify(ref batchSizeFactor, value, "BatchSizeFactor"); }
        }

        public bool ShowBatchText
        {
            get { return showBatchText; }
            set { showBatchText = AssignNotify(ref showBatchText, value, "ShowBatchText"); }
        }

        public bool UseMachineCount
        {
            get { return useMachineCount; }
            set { useMachineCount = AssignNotify(ref useMachineCount, value, "UseMachineCount"); }
        }

        public bool UseProcessCodeCount
        {
            get { return !UseMachineCount; }
        }

        public bool MachineCountExitPoint
        {
            get { return machineCountExitPoint; }
            set { machineCountExitPoint = AssignNotify(ref machineCountExitPoint, value, "MachineCountExitPoint"); }
        }

        public bool OperatorCountExitPoint //legacy property
        {
            get
            {
                return operatorProductionCountExitPoint || operatorTimeCountExitPoint;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                //if ((OperatorCountExitPoint != value) && (da.DatabaseVersion < 1.97))
                if (OperatorCountExitPoint != value)
                {
                    OperatorProductionCountExitPoint = value;
                    OperatorTimeCountExitPoint = value;
                    Notify("OperatorCountExitPoint");
                }
            }
        }

        public bool OperatorProductionCountExitPoint
        {
            get { return operatorProductionCountExitPoint; }
            set
            {
                operatorProductionCountExitPoint = AssignNotify(ref operatorProductionCountExitPoint, value, "OperatorProductionCountExitPoint");
                writeXmlSettings();
            }
        }

        public bool OperatorTimeCountExitPoint
        {
            get { return operatorTimeCountExitPoint; }
            set
            {
                operatorTimeCountExitPoint = AssignNotify(ref operatorTimeCountExitPoint, value, "OperatorTimeCountExitPoint");
                writeXmlSettings();
            }
        }

        public bool OperatorMultipleCountExitPoint
        {
            get { return operatorMultipleCountExitPoint; }
            set
            {
                operatorMultipleCountExitPoint = AssignNotify(ref operatorMultipleCountExitPoint, value, "OperatorMultipleCountExitPoint");
                writeXmlSettings();
            }
        }


        public bool ExcludeFromPowerCalc
        {
            get { return excludeFromPowerCalc; }
            set { excludeFromPowerCalc = AssignNotify(ref excludeFromPowerCalc, value, "ExcludeFromPowerCalc"); }
        }

        public bool ProductionReference
        {
            get { return productionReference; }
            set { productionReference = AssignNotify(ref productionReference, value, "ProductionReference"); }
        }

        public string CommandLine
        {
            get { return commandLine; }
            set { commandLine = AssignNotify(ref commandLine, value, "CommandLine"); }
        }

        public string CommandDescription
        {
            get { return commandDescription; }
            set { commandDescription = AssignNotify(ref commandDescription, value, "CommandDescription"); }
        }

        public int BatchUpper
        {
            get { return batchUpper; }
            set { batchUpper = AssignNotify(ref batchUpper, value, "BatchUpper"); }
        }

        public int BatchLower
        {
            get { return batchLower; }
            set { batchLower = AssignNotify(ref batchLower, value, "BatchLower"); }
        }

        public int NoFlow
        {
            get { return noFlow; }
            set { noFlow = AssignNotify(ref noFlow, value, "NoFlow"); }
        }

        public int OldNoFlow
        {
            get { return oldNoFlow; }
            set { oldNoFlow = value; }
        }

        public bool NoFlowChanged
        {
            get { return (!IsNew && HasChanged && (NoFlow != OldNoFlow)); }
        }


        public double CockpitBatchSizeFactor
        {
            get { return cockpitBatchSizeFactor; }
            set
            {
                cockpitBatchSizeFactor = AssignNotify(ref cockpitBatchSizeFactor, value, "CockpitBatchSizeFactor");
                writeXmlSettings();
            }
        }

        public double RailBatchSizeFactor
        {
            get { return railBatchSizeFactor; }
            set
            {
                railBatchSizeFactor = AssignNotify(ref railBatchSizeFactor, value, "RailBatchSizeFactor");
                writeXmlSettings();
            }
        }

        public bool OverallMetering
        {
            get { return overallMetering; }
            set
            {
                overallMetering = AssignNotify(ref overallMetering, value, "OverallMetering");
                writeXmlSettings();
            }
        }
        
        public string Settings
        {
            get { return settings; }
            set
            {
                settings = AssignNotify(ref settings, value, "Settings");
                readXmlSettings();
            }
        }

        public bool HasSettings
        {
            get
            {
                return Settings != string.Empty;
            }
        }

        private string genXMLSettingKey(string propertyname, Boolean value)
        {
            string key = string.Format(@"<{0}> {1} </{0}>", propertyname, value);
            return key;
        }

        private string genXMLSettingKey(string propertyname, string value)
        {
            string key = string.Format(@"<{0}> {1} </{0}>", propertyname, value);
            return key;
        }


        private void readXmlSettings()
        {
            if (!string.IsNullOrEmpty(Settings))
            {
                XMLDataAccess Xmlda = XMLDataAccess.Singleton;
                overallMetering = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OverallMetering" }, overallMetering);
                operatorTimeCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorTimeCountExitPoint" }, operatorTimeCountExitPoint);
                operatorMultipleCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorMultipleCountExitPoint" }, operatorMultipleCountExitPoint);
                operatorProductionCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorProductionCountExitPoint" }, operatorProductionCountExitPoint);
                cockpitBatchSizeFactor = Xmlda.ReadDoubleSetting(Settings, new string[2] { "Settings", "CockpitBatchSizeFactor" }, cockpitBatchSizeFactor);
                railBatchSizeFactor = Xmlda.ReadDoubleSetting(Settings, new string[2] { "Settings", "RailBatchSizeFactor" }, railBatchSizeFactor);
                if (cockpitBatchSizeFactor <= 0) cockpitBatchSizeFactor = BatchSizeFactor;
                if (railBatchSizeFactor <= 0) railBatchSizeFactor = BatchSizeFactor;
            }
        }

        private void writeXmlSettings()
        {
            string settings = @" <Settings>  ";
            settings += genXMLSettingKey("OverallMetering", overallMetering);
            settings += genXMLSettingKey("OperatorTimeCountExitPoint", operatorTimeCountExitPoint);
            settings += genXMLSettingKey("OperatorMultipleCountExitPoint", operatorMultipleCountExitPoint);
            settings += genXMLSettingKey("OperatorProductionCountExitPoint", operatorProductionCountExitPoint);
            settings += genXMLSettingKey("CockpitBatchSizeFactor", cockpitBatchSizeFactor.ToString("F4"));
            settings += genXMLSettingKey("RailBatchSizeFactor", railBatchSizeFactor.ToString("F4"));
            settings += @" </Settings> ";
            Settings = settings;
        }
        
        #endregion
    }

    #endregion
    #endregion

    #region MachineGroups

    partial class SqlDataAccess
    {

        public MachineGroups GetAllMachineGroups(MachineGroups recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new MachineGroups();
            recs = (MachineGroups)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

    }

    public class MachineGroups : DataList
    {
        public MachineGroups()
        {
            Lifespan = 1.0;
            ListType = typeof(MachineGroup);
            TblName = "tblMachineGroups";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("IdJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("MachineArea", "MachineArea");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("DisplayInMaster", "DisplayInMaster");
            dBFieldMappings.AddMapping("UnitsKPI", "UnitsKPI");
            dBFieldMappings.AddMapping("HourlyKPI", "HourlyKPI");
            dBFieldMappings.AddMapping("DailyKPI", "DailyKPI");
            dBFieldMappings.AddMapping("GraphKPI", "GraphKPI");
            dBFieldMappings.AddMapping("StartTime", "StartTime");
            dBFieldMappings.AddMapping("EndTime", "EndTime");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    #region MachineGroup

    public class MachineGroup : DataItem
    {
        #region private fields

        private int recNum;
        private int idJensen;
        private string extRef;
        private string machineArea;
        private string shortDescription;
        private string longDescription;
        private bool displayInMaster;
        private int unitsKPI;
        private int hourlyKPI;
        private int dailyKPI;
        private bool graphKPI;
        private int startTime;
        private int endTime;

        #endregion


        #region properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = value;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string MachineArea
        {
            get { return machineArea; }
            set { machineArea = AssignNotify(ref machineArea, value, "MachineArea"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string MachineGroupName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "MachineGroupName"); }
        }

        public string MachineGroupNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public bool DisplayInMaster
        {
            get { return displayInMaster; }
            set { displayInMaster = AssignNotify(ref displayInMaster, value, "DisplayInMaster"); }
        }

        public int UnitsKPI
        {
            get { return unitsKPI; }
            set { unitsKPI = AssignNotify(ref unitsKPI, value, "UnitsKPI"); }
        }

        public int HourlyKPI
        {
            get { return hourlyKPI; }
            set { hourlyKPI = AssignNotify(ref hourlyKPI, value, "HourlyKPI"); }
        }

        public int DailyKPI
        {
            get { return dailyKPI; }
            set { dailyKPI = AssignNotify(ref dailyKPI, value, "DailyKPI"); }
        }

        public bool GraphKPI
        {
            get { return graphKPI; }
            set { graphKPI = AssignNotify(ref graphKPI, value, "GraphKPI"); }
        }

        public int StartTime
        {
            get { return startTime; }
            set { startTime = AssignNotify(ref startTime, value, "StartTime"); }
        }

        public int EndTime
        {
            get { return endTime; }
            set { endTime = AssignNotify(ref endTime, value, "EndTime"); }
        }

        #endregion
    }

    #endregion
    #endregion

    #region MachineStatus

    public class MachineStatuses : DataList
    {
        public MachineStatuses()
        {
            Lifespan = 1.0;
            ListType = typeof(MachineStatus);
            TblName = "tblMachineStatus";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("Machine_idJensen", "MachineID", true, false);
            dBFieldMappings.AddMapping("Status", "Status");
            dBFieldMappings.AddMapping("PieceCount", "PieceCount");
            dBFieldMappings.AddMapping("OnTime", "OnTime");
            dBFieldMappings.AddMapping("RunTime", "RunTime");
            dBFieldMappings.AddMapping("UpdateTime", "UpdateTime");
            dBFieldMappings.AddMapping("LastMessage", "LastMessage");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    #region MachineStatus class

    public class MachineStatus : DataItem
    {
        #region Private fields

        private int recNum;
        private int machineID;
        private int status;
        private int pieceCount;
        private int onTime;
        private int runTime;
        private DateTime updateTime;
        private DateTime lastMessage;

        #endregion

        #region properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = value;
            }
        }

        public int MachineID
        {
            get { return machineID; }
            set
            {
                machineID = AssignNotify(ref machineID, value, "MachineID");
                ID = machineID;
            }
        }

        public int Status
        {
            get { return status; }
            set { status = AssignNotify(ref status, value, "Status"); }
        }

        public int PieceCount
        {
            get { return pieceCount; }
            set { pieceCount = AssignNotify(ref pieceCount, value, "PieceCount"); }
        }

        public int OnTime
        {
            get { return onTime; }
            set { onTime = AssignNotify(ref onTime, value, "OnTime"); }
        }

        public int RunTime
        {
            get { return runTime; }
            set { runTime = AssignNotify(ref runTime, value, "RunTime"); }
        }

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = AssignNotify(ref updateTime, value, "UpdateTime"); }
        }

        public DateTime LastMessage
        {
            get { return lastMessage; }
            set { lastMessage = AssignNotify(ref lastMessage, value, "LastMessage"); }
        }



        #endregion
    }


    #endregion
    #endregion

    #region MachineSubids

    public class MachineSubIDs : DataList
    {
        public MachineSubIDs()
        {
            Lifespan = 1.0;
            ListType = typeof(MachineSubID);
            TblName = "tblMachineSubID";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            //dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            //dBFieldMappings.AddMapping("Machine_idJensen", "MachineID", true, false);
            //dBFieldMappings.AddMapping("SubID", "SubID", true, false); 
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Machine_idJensen", "MachineID", false, false, true, false);
            dBFieldMappings.AddMapping("SubID", "SubID", false, false, true, false);
            dBFieldMappings.AddMapping("Operator_idJensen", "OperatorID");
            dBFieldMappings.AddMapping("Customer_idJensen", "CustomerID");
            dBFieldMappings.AddMapping("Article_idJensen", "ArticleID");
            dBFieldMappings.AddMapping("Operator_Remote", "OperatorRemote");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public DataItem GetByIDSubID(int id, int subid)
        {
            DataItem di = null;
            foreach (DataItem dataItem in this)
            {
                MachineSubID msub=(MachineSubID)dataItem;
                if ((msub.MachineID == id) && (msub.SubID == subid))
                {
                    di = dataItem;
                    break;
                }
            }
            return di;
        }
    }

    public class MachineSubID : DataItem
    {
        private int recNum;
        private int machineID;
        private int subID;
        private int operatorID;
        private int customerID;
        private int articleID;
        private int operatorRemote;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum"); 
                PrimaryKey = value;
            }
        }

        public int MachineID
        {
            get { return machineID; }
            set
            {
                machineID = AssignNotify(ref machineID, value, "MachineID");
                ID = machineID;
            }
        }

        public int SubID
        {
            get { return subID; }
            set
            {
                subID = AssignNotify(ref subID, value, "SubID");
                ID2 = subID;
            }
        }

        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
        }

        public int CustomerID
        {
            get { return customerID; }
            set { customerID = AssignNotify(ref customerID, value, "CustomerID"); }
        }

        public int ArticleID
        {
            get { return articleID; }
            set { articleID = AssignNotify(ref articleID, value, "ArticleID"); }
        }

        public int OperatorRemote
        {
            get { return operatorRemote; }
            set { operatorRemote = AssignNotify(ref operatorRemote, value, "OperatorRemote"); }
        }
  
    }

    #endregion

    #region MachineAliases

    partial class SqlDataAccess
    {
        public MachineAliases GetAllMachineAliases(MachineAliases recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new MachineAliases();
            recs = (MachineAliases)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }
    }

    public class MachineAliases : DataList
    {
        public MachineAliases()
        {
            Lifespan = 1.0;
            ListType = typeof(MachineAlias);
            TblName = "tblMachineAlias";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AliasID", "AliasID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            dBFieldMappings.AddMapping("SubIDList", "SubIDList");
            dBFieldMappings.AddMapping("DestID", "DestID");
            dBFieldMappings.AddMapping("SourceIndex", "SourceIndex");
            dBFieldMappings.AddMapping("ConversionType", "ConversionType");
            dBFieldMappings.AddMapping("Switches", "Switches");
            dBFieldMappings.AddMapping("Active", "Active");
            dBFieldMappings.AddMapping("RemoteID", "RemoteID");
            MachineAlias dataItem = (MachineAlias)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            DefaultSqlSelectCommand = @"SELECT ma.[AliasID]
                                            ,ma.[Description]
                                            ,ma.[SourceID]
                                            ,ma.[SubIDList]
                                            ,ma.[DestID]
                                            ,ma.[SourceIndex]
                                            ,ma.[ConversionType]
                                            ,ma.[Switches]
                                            ,ma.[Active]
                                            ,ISNULL(MAX(ld.remoteid),-1) AS RemoteID
                                        FROM [dbo].[tblMachineAlias] ma LEFT OUTER JOIN dbo.tblJGLogData ld
                                        ON ld.MachineID=ma.[DestID]                                         
                                        GROUP BY [AliasID]
                                            ,ma.[Description]
                                            ,ma.[SourceID]
                                            ,ma.[SubIDList]
                                            ,ma.[DestID]
                                            ,ma.[SourceIndex]
                                            ,ma.[ConversionType]
                                            ,ma.[Switches]
                                            ,ma.[Active]
                                        ORDER BY AliasID  ";
        }

        public int MaxRemoteID()
        {
            int remoteid = -1;
            foreach (MachineAlias mla in this)
            {
                if (mla.RemoteID > remoteid)
                    remoteid = mla.RemoteID;
            }
            return remoteid;
        }

        public int MaxRemoteID(int machineid)
        {
            int remoteid = -1;
            foreach (MachineAlias mla in this)
            {
                if ((mla.RemoteID > remoteid) && (mla.DestID == machineid))
                    remoteid = mla.RemoteID;
            }
            return remoteid;
        }

        public MachineAliases GetBySourceMachine(int machineID)
        {
            MachineAliases mlas = new MachineAliases();
            foreach (MachineAlias mla in this)
            {
                if (mla.SourceID == machineID)
                {
                    mlas.Add(mla);
                }
            }
            return mlas;
        }

        public List<int> GetSourceMachineIDs()
        {
            List<int> mList = new List<int>();
            foreach (MachineAlias rec in this)
            {
                if (!mList.Contains(rec.SourceID))
                    mList.Add(rec.SourceID);
            }
            return mList;
        }
    }

    #region machine alias class
    public class MachineAlias : DataItem
    {
        #region Private fields

        private int aliasID;
        //private string description;
        private int sourceID;
        private string subIDList;
        private int destID;
        private int sourceIndex;
        private int conversionType;
        private string switches;
        private bool active;
        private int remoteID;

        #endregion

        #region Properties
        public int AliasID
        {
            get { return aliasID; }
            set
            {
                aliasID = AssignNotify(ref aliasID, value, "AliasID");
                ID = aliasID;
                PrimaryKey = aliasID;
            }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set
        //    {
        //        description = AssignNotify(ref description, value, "Description");
        //        ItemName = description;
        //    }
        //}

        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(ref sourceID, value, "SourceID"); }
        }

        public string SubIDList
        {
            get { return subIDList; }
            set { subIDList = AssignNotify(ref subIDList, value, "SubIDList"); }
        }

        public int DestID
        {
            get { return destID; }
            set { destID = AssignNotify(ref destID, value, "DestID"); }
        }

        public int SourceIndex
        {
            get { return sourceIndex; }
            set { sourceIndex = AssignNotify(ref sourceIndex, value, "SourceIndex"); }
        }

        public int ConversionType
        {
            get { return conversionType; }
            set { conversionType = AssignNotify(ref conversionType, value, "ConversionType"); }
        }

        public string Switches
        {
            get { return switches; }
            set { switches = AssignNotify(ref switches, value, "Switches"); }
        }

        public override bool Active
        {
            get { return active; }
            set { active = AssignNotify(ref active, value, "Active"); }
        }

        public int RemoteID
        {
            get { return remoteID; }
            set { remoteID = AssignNotify(ref remoteID, value, "RemoteID"); }
        }

        #endregion
    }
    #endregion
 
    #endregion

    #region KPI

    partial class SqlDataAccess
    {
        public MachineKpis GetDayMachineKpis(DateTime aStart, DateTime anEnd, Boolean noCacheRead, Boolean allowStoreToCache, Machines machines)
        {
            MachineKpis recs = new MachineKpis();
            recs.TblName = "tblMachineKPI";
            recs.AddSelectRestriction(new RestrictionRec("RecTimeStamp", aStart, anEnd, ComparisonType.MoreThanALessThanEqualB));
            recs.TimeoutSeconds = 120;
            recs = (MachineKpis)DBDataListSelect(recs, noCacheRead, allowStoreToCache);
            recs.Machines = machines;
            return recs;
        }

        public MachineKpis GetMachineDayConsumptionKpis(Boolean noCacheRead, Boolean allowStoreToCache, Machines machines)
        {
            MachineKpis recs = new MachineKpis();
            recs.TblName = "tblMachineKPI";
            recs.AddSelectRestriction(new RestrictionRec("KpiType", 10, ComparisonType.MoreThan));
            recs.TimeoutSeconds = 120;
            recs = (MachineKpis)DBDataListSelect(recs, noCacheRead, allowStoreToCache);
            recs.Machines = machines;
            return recs;
        }

        public Boolean DeleteMachineKpisDay(DateTime recdate)
        {
            Boolean test = false;
            try
            {
                MachineKpis recs = new MachineKpis();
                recs.TblName = "tblMachineKPI";
                recs.AddDeleteRestriction(new RestrictionRec("RecTimeStamp", recdate, PresetRestriction.Today));
                test = recs.DBDelete();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
            }
            return test;
        }

    }


    public class MachineKpis : DataList
    {
        public Machines Machines { get; set; }
        SqlDataAccess da;
        private int kpiInterval=300;
        private Boolean isMetric = true;


        public Boolean IsMetric
        {
          get { return isMetric; }
          set { isMetric = value; 
              if (isMetric)
                  jensenUnits.MetricUnits();
              else 
                  jensenUnits.USUnits();
          }
        }

        private JensenUnits jensenUnits;

        public int KpiInterval
        {
            get { return kpiInterval; }
            set { kpiInterval = value; }
        }

        public MachineKpis()
        {
            Lifespan = 1.0/60.0; //1 minutes
            ListType = typeof(MachineKpi);
            TblName = "tblMachineKpi";
            DbName = "JEGR_DB";
            jensenUnits=new JensenUnits(true);
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("RecTimeStamp", "RecTimeStamp");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("Value", "Value");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("OperatorProduction", "OperatorProduction");
            dBFieldMappings.AddMapping("OperatorHours", "OperatorHours");
            dBFieldMappings.AddMapping("Unit", "Unit");
            dBFieldMappings.AddMapping("KpiID", "KpiID");
            dBFieldMappings.AddMapping("CountExitPointData", "CountExitPointData");
            dBFieldMappings.AddMapping("KpiType", "KpiType");
            dBFieldMappings.AddMapping("KpiSubType", "KpiSubType");
            dBFieldMappings.AddMapping("KpiSubTypeID", "KpiSubTypeID");
            dBFieldMappings.AddMapping("ExpectedValue", "ExpectedValue");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            da = SqlDataAccess.Singleton;
        }

        public void debugToCSV()
        {
            foreach (MachineKpi kpi in this)
            {
                Debug.WriteLine(kpi.ToCSV()+Environment.NewLine);
            }
        }

        public MachineKpi GetCurrentMachineSubidKpi(DateTime datetime, int machineid, int subid, KpiDataType datatype)
        {
            MachineKpi rec = null;
            DateTime current = DateTime.MinValue;
            DateTime thisday = datetime.Date;
            foreach (MachineKpi mkpi in this)
            {
                if (rec != null)
                    current = rec.RecTimeStamp;
                if ((mkpi.MachineID == machineid)
                    && (mkpi.SubID == subid)
                    && (mkpi.RecTimeStamp <= datetime)
                    && (mkpi.RecTimeStamp >= thisday)
                    && (current < mkpi.RecTimeStamp)
                    && (mkpi.DataType == datatype))
                    rec = mkpi;
            }
            return rec;
        }

        public MachineKpis GetCurrentMachineSubidKpis(DateTime datetime, int machineid, int subid, KpiDataType datatype)
        {
            MachineKpis currentKpis = new MachineKpis();
            DateTime recordTime = datetime.Date;
            DateTime thisday = datetime.Date;
            foreach (MachineKpi rec in this)
            {
                if (rec.RecTimeStamp > recordTime)
                    recordTime = rec.RecTimeStamp;
            }
            foreach (MachineKpi rec in this)
            {
                if ((rec.RecTimeStamp == recordTime)
                    && (rec.MachineID == machineid)
                    && (rec.SubID == subid)
                    && (rec.DataType == datatype))
                {
                    currentKpis.Add(rec);
                }
            }
            return currentKpis;
        }

        public MachineKpis GetHistoricMachineSubidKpis(DateTime datetime, int machineid, int subid, KpiDataType datatype)
        {
            MachineKpis currentKpis = new MachineKpis();
            DateTime thisday = datetime.Date;
            foreach (MachineKpi rec in this)
            {
                if ((rec.RecTimeStamp <= datetime)
                    && (rec.RecTimeStamp.Date==thisday)
                    && (rec.MachineID == machineid)
                    && (rec.SubID == subid)
                    && (rec.DataType == datatype))
                {
                    currentKpis.Add(rec);
                }
            }
            return currentKpis;
        }

        private int quickFindTime(DateTime time)
        {
            int lo = 0;
            int hi = this.Count - 1;
            int target = 0;
            for (int i = 0; i < 8; i++)
            {
                target = (lo + ((hi - lo) / 2));
                MachineKpi kpi = (MachineKpi)this[target];
                if (kpi.RecTimeStamp < time)
                {
                    lo = target;
                }
                if (kpi.RecTimeStamp >= time)
                {
                    hi = target;
                }
            }
            return lo;
        }

        private int quickFindMachine(int machineid)
        {
            int lo = 0;
            int hi = this.Count - 1;
            int target = 0;
            for (int i = 0; i < 8; i++)
            {
                target = (lo + ((hi - lo) / 2));
                MachineKpi kpi = (MachineKpi)this[target];
                if (kpi.MachineID < machineid)
                {
                    lo = target;
                }
                if (kpi.MachineID >= machineid)
                {
                    hi = target;
                }
            }
            return lo;
        }

        public MachineKpi _GetMachineSubidKpi(DateTime datetime, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            if (!IsSorted)
                SortByTimeMachineSub();
            MachineKpi rec = null;
            if (this.Count > 0)
            {
                int i = quickFindTime(datetime);
                while ((i < this.Count) && (rec == null))
                {
                    MachineKpi mkpi = (MachineKpi)this[i];
                    if ((mkpi.RecTimeStamp == datetime)
                        && (mkpi.MachineID == machineid)
                        && (mkpi.SubID == subid)
                        && (mkpi.OperatorID == operatorid)
                        && (mkpi.DataType == datatype))
                        rec = mkpi;
                    i++;
                }
            }
            return rec;
        }

        public MachineKpi GetMachineSubidKpi(DateTime datetime, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            if (!IsSorted)
                SortByTimeMachineSub(); 
            MachineKpi rec = null;
            if (this.Count > 0)
            {
                int i = 0;// quickFindTime(datetime);
                while ((i < this.Count) && (rec == null))
                {
                    MachineKpi mkpi = (MachineKpi)this[i];
                    if ((mkpi.RecTimeStamp == datetime)
                        && (mkpi.MachineID == machineid)
                        && (mkpi.SubID == subid)
                        && (mkpi.OperatorID == operatorid)
                        && (mkpi.DataType == datatype))
                        rec = mkpi;
                    i++;
                }
            }
            return rec;
        }

        public MachineKpi GetMachineSubidKpi(int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            if (!IsSorted)
                SortByMachineSub();
            MachineKpi rec = null;
            if (this.Count > 0)
            {
                int i = 0;// quickFindMachine(machineid);
                while ((i < this.Count) && (rec == null))
                {
                    MachineKpi mkpi = (MachineKpi)this[i];
                    if ((mkpi.MachineID == machineid)
                        && (mkpi.SubID == subid)
                        && (mkpi.OperatorID == operatorid)
                        && (mkpi.DataType == datatype))
                        rec = mkpi;
                    i++;
                }
            }
            return rec;
        }

        public MachineKpi GetPreviousMachineSubidKpi(DateTime datetime, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            MachineKpi rec = null;
            DateTime findtime=datetime;
            while ((rec == null) && (findtime > datetime.Date))
            {
                findtime = findtime.AddMinutes(-1 * kpiInterval);
                rec = GetMachineSubidKpi(findtime, machineid, subid, operatorid, datatype);
            }
            return rec;
        }

        public KpiDataType GetKpiDataType(int regtype, int subregtype, int subregtypeid)
        {
            KpiDataType datatype = KpiDataType.Unknown;
            if (regtype == 1) datatype = KpiDataType.Production;
            if (regtype == 2)
            {
                if (subregtype == 20) datatype = KpiDataType.ElectricityConsumption;
                if (subregtype == 30)
                {
                    if (subregtypeid == 77) datatype = KpiDataType.FreshwaterConsumption;
                    if (subregtypeid == 900) datatype = KpiDataType.WasteWater;
                }
                if (subregtype == 40) datatype = KpiDataType.GasConsumption;
            }
            if (regtype == 6) datatype = KpiDataType.Operator;
            return datatype;
        }

        private MachineKpi createMachineSubidKpi(DateTime periodStart, DateTime periodEnd, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            MachineKpi rec = new MachineKpi();
            Machine m = (Machine)Machines.GetById(machineid);
            int unit = -1;
            if (m != null)
                unit = m.NormUnit;
            rec.RecTimeStamp = periodEnd;
            rec.LastOpLoginOut = periodStart;
            rec.MachineID = machineid;
            rec.SubID = subid;
            rec.OperatorID = operatorid;
            rec.KpiID = 1;
            rec.Value = 0;
            rec.PreviousOpHours = 0;
            rec.CurrentOpHours = 0;
            rec.KpiType = 1;
            rec.KpiSubType = 0;
            rec.KpiSubTypeID = 0;
            rec.Unit = unit;
            if ((datatype == KpiDataType.Production) || (datatype == KpiDataType.DayProduction))
            {
                rec.KpiType = 1;
                if (datatype == KpiDataType.DayProduction) rec.KpiType = 11;
                rec.KpiSubType = 0;
                rec.KpiSubTypeID = 0;
                rec.Unit = unit;
            }
            if ((datatype == KpiDataType.FreshwaterConsumption) || (datatype == KpiDataType.DayFreshwaterConsumption))
            {
                rec.KpiType = 2;
                if (datatype == KpiDataType.DayFreshwaterConsumption) rec.KpiType = 12;
                rec.KpiSubType = 30;
                rec.KpiSubTypeID = 77;
                rec.Unit = jensenUnits.WaterVolumeUnit;
            }
            if ((datatype == KpiDataType.WasteWater) || (datatype == KpiDataType.DayWasteWater))
            {
                rec.KpiType = 2;
                if (datatype == KpiDataType.DayWasteWater) rec.KpiType = 12;
                rec.KpiSubType = 30;
                rec.KpiSubTypeID = 900;
                rec.Unit = jensenUnits.WaterVolumeUnit;
            }
            if ((datatype == KpiDataType.GasConsumption) || (datatype == KpiDataType.DayGasConsumption))
            {
                rec.KpiType = 2;
                if (datatype == KpiDataType.DayGasConsumption) rec.KpiType = 12;
                rec.KpiSubType = 40;
                rec.KpiSubTypeID = 0;
                rec.Unit = jensenUnits.GasEnergyUnit;
            }
            if ((datatype == KpiDataType.ElectricityConsumption) || (datatype == KpiDataType.DayElectricityConsumption))
            {
                rec.KpiType = 2;
                if (datatype == KpiDataType.DayElectricityConsumption) rec.KpiType = 12;
                rec.KpiSubType = 20;
                rec.KpiSubTypeID = 0;
                rec.Unit = jensenUnits.ElectricalEnergyUnit;
            }
            rec.ForceNew = true;
            this.Add(rec);
            return rec;
        }

        public MachineKpi GetOrCreateMachineSubidKpi(DateTime periodStart, DateTime periodEnd, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            MachineKpi rec = GetMachineSubidKpi(periodEnd, machineid, subid, operatorid, datatype);
            if (rec == null)
                rec = createMachineSubidKpi(periodStart, periodEnd, machineid, subid, operatorid, datatype);
            return rec;
        }

        private KpiDataType getDailyKpiDataType(KpiDataType datatype)
        {
            KpiDataType dailyType = KpiDataType.Unknown;
            switch (datatype)
            {
                case KpiDataType.Production:
                    dailyType = KpiDataType.DayProduction;
                    break;
                case KpiDataType.ElectricityConsumption:
                    dailyType=KpiDataType.DayElectricityConsumption;
                    break;
                case KpiDataType.GasConsumption:
                    dailyType=KpiDataType.DayGasConsumption;
                    break;
                case KpiDataType.FreshwaterConsumption:
                    dailyType=KpiDataType.DayFreshwaterConsumption;
                    break;
                case KpiDataType.WasteWater:
                    dailyType = KpiDataType.DayWasteWater;
                    break;
                default:
                    dailyType = KpiDataType.Unknown;
                    break;
            }
            return dailyType;

        }

        public MachineKpi GetOrCreateMachineSubidDailyKpi(DateTime periodStart, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            KpiDataType lookupType = getDailyKpiDataType(datatype);
            DateTime lookupTime = periodStart.Date.AddMinutes(5);
            MachineKpi rec = GetMachineSubidKpi(lookupTime, machineid, subid, operatorid, lookupType);
            if (rec == null)
                rec = createMachineSubidKpi(lookupTime, lookupTime, machineid, subid, operatorid, lookupType);
            return rec;
        }

        public MachineKpi GetOrCreateRunningMachineSubidKpi(DateTime periodStart, DateTime periodEnd, int machineid, int subid, int operatorid, KpiDataType datatype)
        {
            if (datatype == KpiDataType.Operator)
                datatype = KpiDataType.Production;
            MachineKpi rec = GetMachineSubidKpi(machineid, subid, operatorid, datatype);
            if (rec == null)
                rec = createMachineSubidKpi(periodStart, periodEnd, machineid, subid, operatorid, datatype);
            return rec;
        }

        public Double GetMachineKpiValues(DateTime datetime, int machineid, KpiDataType datatype)
        {
            double value = 0;
            int positions = 1;
            Machine machine = (Machine)Machines.GetById(machineid);
            if (machine != null)
            {
                positions = machine.Positions;

                for (int i = 1; i <= positions; i++)
                {
                    MachineKpis machineKpis = GetHistoricMachineSubidKpis(datetime, machineid, i, datatype);
                    if (machineKpis != null)
                    {
                        foreach (MachineKpi machineKpi in machineKpis)
                        {
                            value += machineKpi.Value;
                        }
                    }
                }
            }
            return value;
        }

        public void SortByTimeMachineSub()
        {
            SortKpiDataTimeIDs skd = new SortKpiDataTimeIDs();
            this.Sort(skd);
            IsSorted = true;
        }

        public void SortByMachineSub()
        {
            SortKpiDataIDs skd = new SortKpiDataIDs();
            this.Sort(skd);
            IsSorted = true;
        }

    }

    public class MachineKpi : DataItem
    {
        private int recNum;
        private DateTime recTimeStamp;
        private DateTime lastTimeStamp;
        private DateTime lastOpLoginOut;
        private int machineID;
        private int subID = 1;
        private double value;
        private int operatorID = -1;
        private double operatorProduction;
        private double operatorHours;
        private bool operatorLoggedIn = false;
        private double previousOpHours;
        private double currentOpHours;
        private int unit;
        private int kpiID = 1;
        private bool countExitPointData;
        private int kpiType = 0;
        private int kpiSubType = 1;
        private int kpiSubTypeID = 0;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = value;
                ID = value;
            }
        }

        public DateTime RecTimeStamp
        {
            get { return recTimeStamp; }
            set { recTimeStamp = AssignNotify(ref recTimeStamp, value, "RecTimeStamp"); }
        }

        public DateTime LastTimeStamp
        {
            get { return lastTimeStamp; }
            set { lastTimeStamp = AssignNotify(ref lastTimeStamp, value, "LastTimeStamp"); }
        }

        public DateTime LastOpLoginOut
        {
            get { return lastOpLoginOut; }
            set { lastOpLoginOut = AssignNotify(ref lastOpLoginOut, value, "LastOpLoginOut"); }
        }

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }

        public double Value
        {
            get { return Math.Round(this.value, 5); }
            set { this.value = AssignNotify(ref this.value, value, "Value"); }
        }

        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
        }

        public double OperatorProduction
        {
            get { return Math.Round(operatorProduction, 5); }
            set { operatorProduction = AssignNotify(ref operatorProduction, value, "OperatorProduction"); }
        }

        public double OperatorHours
        {
            get { return Math.Round(operatorHours, 5); }
            set { operatorHours = AssignNotify(ref operatorHours, value, "OperatorHours"); }
        }

        public double PreviousOpHours
        {
            get { return previousOpHours; }
            set { previousOpHours = AssignNotify(ref previousOpHours, value, "PreviousOpHours"); }
        }

        public double CurrentOpHours
        {
            get { return currentOpHours; }
            set
            {
                currentOpHours = AssignNotify(ref currentOpHours, value, "CurrentOpHours");
                //OperatorHours = currentOpHours + PreviousOpHours;
                OperatorHours = currentOpHours;
            }
        }

        public Boolean OperatorLoggedIn
        {
            get { return operatorLoggedIn; }
            set { operatorLoggedIn = AssignNotify(ref operatorLoggedIn, value, "OperatorLoggedIn"); }
        }

        public int Unit
        {
            get { return unit; }
            set { unit = AssignNotify(ref unit, value, "Unit"); }
        }

        public int KpiID
        {
            get { return kpiID; }
            set { kpiID = AssignNotify(ref kpiID, value, "KpiID"); }
        }

        public bool CountExitPointData
        {
            get { return countExitPointData; }
            set { countExitPointData = AssignNotify(ref countExitPointData, value, "CountExitPointData"); }
        }

        public int KpiType
        {
            get { return kpiType; }
            set
            {
                kpiType = AssignNotify(ref kpiType, value, "KpiType");
                //kpiID = kpiType + 1;
            }
        }

        public int KpiSubType
        {
            get { return kpiSubType; }
            set { kpiSubType = AssignNotify(ref kpiSubType, value, "KpiSubType"); }
        }

        public int KpiSubTypeID
        {
            get { return kpiSubTypeID; }
            set { kpiSubTypeID = AssignNotify(ref kpiSubTypeID, value, "KpiSubTypeID"); }
        }

        public int ExpectedValue
        {
            get { return (int)expectedValueDouble; }
            set { expectedValueDouble = AssignNotify(ref expectedValueDouble, value, "ExpectedValue"); }
        }

        private double expectedValueDouble;

        public double ExpectedValueDouble
        {
            get { return expectedValueDouble; }
            set { expectedValueDouble = AssignNotify(ref expectedValueDouble, value, "ExpectedValue"); }
        }

        public KpiDataType DataType
        {
            get
            {
                KpiDataType dataType = KpiDataType.Unknown;
                if ((kpiType == 1) && (kpiSubType == 0) && (kpiSubTypeID == 0)) //production
                    dataType = KpiDataType.Production;
                if ((kpiType == 2) && (kpiSubType == 30) && (kpiSubTypeID == 77)) //freshwater
                    dataType = KpiDataType.FreshwaterConsumption;
                if ((kpiType == 2) && (kpiSubType == 30) && (kpiSubTypeID == 900)) //wastewater
                    dataType = KpiDataType.WasteWater;
                if ((kpiType == 2) && (kpiSubType == 20) && (kpiSubTypeID == 0)) //electricity
                    dataType = KpiDataType.ElectricityConsumption;
                if ((kpiType == 2) && (kpiSubType == 40) && (kpiSubTypeID == 0)) //gas
                    dataType = KpiDataType.GasConsumption;
                if ((kpiType == 11) && (kpiSubType == 0) && (kpiSubTypeID == 0)) //production daily summary
                    dataType = KpiDataType.DayProduction;
                if ((kpiType == 12) && (kpiSubType == 30) && (kpiSubTypeID == 77)) //freshwater daily summary
                    dataType = KpiDataType.DayFreshwaterConsumption;
                if ((kpiType == 12) && (kpiSubType == 30) && (kpiSubTypeID == 900)) //wastewater daily summary
                    dataType = KpiDataType.DayWasteWater;
                if ((kpiType == 12) && (kpiSubType == 20) && (kpiSubTypeID == 0)) //electricity daily summary
                    dataType = KpiDataType.DayElectricityConsumption;
                if ((kpiType == 12) && (kpiSubType == 40) && (kpiSubTypeID == 0)) //gas daily summary
                    dataType = KpiDataType.DayGasConsumption;

                return dataType;
            }
        }

        public int CompareTo(MachineKpi b)
        {
            int compare = this.RecTimeStamp.CompareTo(b.RecTimeStamp);
            if (compare == 0)
                compare = this.MachineID.CompareTo(b.MachineID);
            if (compare == 0)
                compare = this.SubID.CompareTo(b.SubID);
            return compare;
        }

        public string ToCSV()
        {
            string aString = RecNum + " " + RecTimeStamp + " " + MachineID + " " + SubID + " " + Value + " " + OperatorID + " " + OperatorHours + " " + ExpectedValue + " " + DataType + " " + HasChanged + " " + DeleteUnwrittenRecord + " " + IsNew;

            return aString;
        }


    }

    public class SortKpiDataTimeIDs : IComparer<DataItem>
    {
        public int Compare(DataItem x, DataItem y)
        {
            MachineKpi a = (MachineKpi)x;
            MachineKpi b = (MachineKpi)y;
            int result = a.RecTimeStamp.CompareTo(b.RecTimeStamp);
            if (result == 0)
            {
                result = a.MachineID.CompareTo(b.MachineID);
            }
            if (result == 0)
            {
                result = a.SubID.CompareTo(b.SubID);
            }
            if (result == 0)
            {
                result = a.RecNum.CompareTo(b.RecNum);
            }
            return result;
        }
    }


    public class SortKpiDataIDs : IComparer<DataItem>
    {
        public int Compare(DataItem x, DataItem y)
        {
            MachineKpi a = (MachineKpi)x;
            MachineKpi b = (MachineKpi)y;
            int result = a.MachineID.CompareTo(b.MachineID);
            if (result == 0)
            {
                result = a.SubID.CompareTo(b.SubID);
            }
            if (result == 0)
            {
                result = a.OperatorID.CompareTo(b.OperatorID);
            }
            if (result == 0)
            {
                result = a.RecNum.CompareTo(b.RecNum);
            }
            return result;
        }
    }

    public enum KpiDataType
    {
        Production,
        Operator,
        ElectricityConsumption,
        GasConsumption,
        FreshwaterConsumption,
        WasteWater,
        DayProduction,
        DayElectricityConsumption,
        DayGasConsumption,
        DayFreshwaterConsumption,
        DayWasteWater,
        Unknown
    }


    #endregion


}
