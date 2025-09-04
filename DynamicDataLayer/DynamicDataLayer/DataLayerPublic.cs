using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Dynamic.DataLayer
{

    public class PublicBatchDetails : DataList
    {
        public PublicBatchDetails()
        {
            Lifespan = 1.0;
            ListType = typeof(PublicBatchDetail);
            TblName = "tblBatchDetails";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("RemoteID", "RemoteID");
            dBFieldMappings.AddMapping("BatchID", "BatchID");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            dBFieldMappings.AddMapping("ProcessCode", "ProcessCode");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("CustomerID", "CustomerID");
            dBFieldMappings.AddMapping("Weight", "Weight");
            dBFieldMappings.AddMapping("TimeStamp", "TimeStamp");
            dBFieldMappings.AddMapping("Water_Consumption", "Water_Consumption");
            dBFieldMappings.AddMapping("Gas_Consumption", "Gas_Consumption");
            dBFieldMappings.AddMapping("Steam_Consumption", "Steam_Consumption");
            dBFieldMappings.AddMapping("Electricity_Consumption", "Electricity_Consumption");
            dBFieldMappings.AddMapping("SortCategoryID", "SortCategoryID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class PublicBatchDetail : DataItem
    {
        #region private fields
        private int recNum;
        private int remoteID;
        private int batchID;
        private int sourceID;
        private int processCode;
        private int machineID;
        private int customerID;
        private double weight;
        private DateTime timeStamp;
        private double water_Consumption;
        private double gas_Consumption;
        private double steam_Consumption;
        private double electricity_Consumption;
        private int sortCategoryID; 
        #endregion

        #region Properties
        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = value;
                PrimaryKey = value;
            }
        }
        public int RemoteID
        {
            get { return remoteID; }
            set { remoteID = AssignNotify(ref remoteID, value, "RemoteID"); }
        }
        public int BatchID
        {
            get { return batchID; }
            set { batchID = AssignNotify(ref batchID, value, "BatchID"); }
        }
        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(ref sourceID, value, "SourceID"); }
        }
        public int ProcessCode
        {
            get { return processCode; }
            set { processCode = AssignNotify(ref processCode, value, "ProcessCode"); }
        }
        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }
        public int CustomerID
        {
            get { return customerID; }
            set { customerID = AssignNotify(ref customerID, value, "CustomerID"); }
        }
        public double Weight
        {
            get { return weight; }
            set { weight = AssignNotify(ref weight, value, "Value"); }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = AssignNotify(ref timeStamp, value, "TimeStamp"); }
        }
        public double Water_Consumption
        {
            get { return water_Consumption; }
            set { water_Consumption = AssignNotify(ref water_Consumption, value, "Water_Consumption"); }
        }
        public double Gas_Consumption
        {
            get { return gas_Consumption; }
            set { gas_Consumption = AssignNotify(ref gas_Consumption, value, "Gas_Consumption"); }
        }
        public double Steam_Consumption
        {
            get { return steam_Consumption; }
            set { steam_Consumption = AssignNotify(ref steam_Consumption, value, "Steam_Consumption"); }
        }
        public double Electricity_Consumption
        {
            get { return electricity_Consumption; }
            set { electricity_Consumption = AssignNotify(ref electricity_Consumption, value, "Electricity_Consumption"); }
        }
        public int SortCategoryID
        {
            get { return sortCategoryID; }
            set { sortCategoryID = AssignNotify(ref sortCategoryID, value, "SortCategoryID"); }
        }     
        #endregion
    }

    public class PublicMachineEventDetails : DataList
    {
        public PublicMachineEventDetails()
        {
            Lifespan = 1.0;
            ListType = typeof(PublicMachineEventDetail);
            TblName = "tblMachineEventDetails";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Start_RemoteID", "StartRemoteID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("EventID", "EventID");
            dBFieldMappings.AddMapping("JGLogDataMessageA", "JGLogDataMessageA");
            dBFieldMappings.AddMapping("Start_Timestamp", "StartTimestamp");
            dBFieldMappings.AddMapping("End_Timestamp", "EndTimestamp");
            dBFieldMappings.AddMapping("Severity", "Severity");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class PublicMachineEventDetail : DataItem
    {
        #region private fields
        private int recNum;
        private int startRemoteID;
        private int machineID;
        private int subID;
        private int eventID;
        private string jgLogDataMessageA;
        private DateTime startTimestamp;
        private DateTime endTimestamp;
        private int severity;
        #endregion

        #region properties
        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = value;
                PrimaryKey = value;
            }
        }
        public int StartRemoteID
        {
            get { return startRemoteID; }
            set { startRemoteID = AssignNotify(ref startRemoteID, value, "StartRemoteID"); }
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
        public int EventID
        {
            get { return eventID; }
            set { eventID = AssignNotify(ref eventID, value, "EventID"); }
        }
        public string JGLogDataMessageA
        {
            get { return jgLogDataMessageA; }
            set { jgLogDataMessageA = AssignNotify(ref jgLogDataMessageA, value, "JGLogDataMessageA"); }
        }
        public DateTime StartTimestamp
        {
            get { return startTimestamp; }
            set { startTimestamp = AssignNotify(ref startTimestamp, value, "StartTimestamp"); }
        }
        public DateTime EndTimestamp
        {
            get { return endTimestamp; }
            set { endTimestamp = AssignNotify(ref endTimestamp, value, "EndTimestamp"); }
        }
        public int Severity
        {
            get { return severity; }
            set { severity = AssignNotify(ref severity, value, "Severity"); }
        }
        #endregion

    }

    public class PublicMachineStatusDetails : DataList
    {
        public PublicMachineStatusDetails()
        {
            Lifespan = 1.0;
            ListType = typeof(PublicMachineStatusDetail);
            TblName = "tblMachineStatusDetails";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Start_RemoteID", "StartRemoteID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("MachineState", "MachineState");
            dBFieldMappings.AddMapping("Start_Timestamp", "StartTimestamp");
            dBFieldMappings.AddMapping("End_Timestamp", "EndTimestamp");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class PublicMachineStatusDetail : DataItem
    {
        #region private fields
        private int recNum;
        private int startRemoteID;
        private int machineID;
        private int machineState;
        private DateTime startTimestamp;
        private DateTime endTimestamp;
        #endregion

        #region properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = value;
                PrimaryKey = value;
            }
        }
        public int StartRemoteID
        {
            get { return startRemoteID; }
            set { startRemoteID = AssignNotify(ref startRemoteID, value, "StartRemoteID"); }
        }
        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }
        public int MachineState
        {
            get { return machineState; }
            set { machineState = AssignNotify(ref machineState, value, "MachineState"); }
        }
        public DateTime StartTimestamp
        {
            get { return startTimestamp; }
            set { startTimestamp = AssignNotify(ref startTimestamp, value, "StartTimestamp"); }
        }
        public DateTime EndTimestamp
        {
            get { return endTimestamp; }
            set { endTimestamp = AssignNotify(ref endTimestamp, value, "EndTimestamp"); }
        }

        #endregion

    }

    partial class SqlDataAccess
    {
        public PublicOperatorLogs GetPublicOperatorLogsDay(PublicOperatorLogs recs, DateTime recdate, Boolean noCacheRead, Boolean allowStoreToCache)
        {
            if (recs == null)
                recs = new PublicOperatorLogs();
            recs.TimeoutSeconds = 120;
            recs.AddSelectRestriction(new RestrictionRec("Timestamp", recdate, PresetRestriction.Today));
            recs = (PublicOperatorLogs)DBDataListSelect(recs, noCacheRead, allowStoreToCache);
            return recs;
        }

        public void DeletePublicOperatorLogsDay(DateTime recdate)
        {
            PublicOperatorLogs recs = new PublicOperatorLogs();
            recs.AddDeleteRestriction(new RestrictionRec("Timestamp", recdate, PresetRestriction.Today));
            recs.DBDelete();
        }

    }

    public class PublicOperatorLogs : DataList
    {
        public PublicOperatorLogs()
        {
            Lifespan = 0.5;
            ListType = typeof(PublicOperatorLog);
            TblName = "tblOperatorLog";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Remote_Recnum", "RemoteRecnum");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("TimeStamp", "TimeStamp");
            dBFieldMappings.AddMapping("State", "State");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        private PublicOperatorLog GetByIDs(PublicOperatorLog rec)
        {
            PublicOperatorLog pol = null;
            foreach (PublicOperatorLog prodrec in this)
            {
                if ((prodrec.RemoteRecnum == rec.RemoteRecnum)
                    && (prodrec.MachineID == rec.MachineID)
                    && (prodrec.SubID == rec.SubID)
                    && (prodrec.OperatorID == rec.OperatorID)
                    && (prodrec.TimeStamp == rec.TimeStamp)
                    && (prodrec.State == rec.State))
                {
                    pol = prodrec;
                }
            }
            return pol;
        }

        public PublicOperatorLog GetOrCreate(PublicOperatorLog x)
        {
            PublicOperatorLog rec = GetByIDs(x);
            if (rec == null)
            {
                rec = new PublicOperatorLog();
                rec.ForceNew = true;
                this.Add(rec);
            }

            rec.RemoteRecnum = x.RemoteRecnum;
            rec.MachineID = x.MachineID;
            rec.SubID = x.SubID;
            rec.OperatorID = x.OperatorID;
            rec.TimeStamp = x.TimeStamp;
            rec.State = x.State;
            rec.DeleteUnwrittenRecord = false;

            return rec;
        }

    }

    public class PublicOperatorLog : DataItem
    {
        private int recNum;
        private int remoteRecnum;
        private int operatorID;
        private int machineID;
        private int subID;
        private DateTime timeStamp;
        private int state;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = value;
                PrimaryKey = value;
            }
        }
        public int RemoteRecnum
        {
            get { return remoteRecnum; }
            set { remoteRecnum = AssignNotify(ref remoteRecnum, value, "RemoteRecnum"); }
        }
        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
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
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = AssignNotify(ref timeStamp, value, "TimeStamp"); }
        }
        public int State
        {
            get { return state; }
            set { state = AssignNotify(ref state, value, "State"); }
        } 
    }


    partial class SqlDataAccess
    {
        public PublicProduction GetPublicProductionDay(PublicProduction recs, DateTime recdate, Boolean noCacheRead, Boolean allowStoreToCache)
        {
            if (recs==null)
                recs = new PublicProduction();
            recs.TblName = "tblProduction";
            recs.TimeoutSeconds = 120;
            recs.AddSelectRestriction(new RestrictionRec("DateHour", recdate, PresetRestriction.Today));
            recs = (PublicProduction)DBDataListSelect(recs, noCacheRead, allowStoreToCache);
            recs.Sort();
            return recs;
        }

        public void DeletePublicProductionDay(DateTime recdate)
        {
            PublicProduction recs = new PublicProduction();
            recs.TblName = "tblProduction";
            recs.AddDeleteRestriction(new RestrictionRec("DateHour", recdate, PresetRestriction.Today));
            recs.DBDelete();
        }
           

    }

    public class PublicProduction : DataList
    {
        public Machines Machines { get; set; }
        private MachineGroups machineGroups = null;
        public MachineGroups MachineGroups
        {
            get { return machineGroups; }
            set { machineGroups = value; }
        }
        private Customers customers = null;
        public Customers Customers
        {
            get { return customers; }
            set { customers = value; }
        }
        private Articles articles = null;
        public Articles Articles
        {
            get { return articles; }
            set { articles = value; }
        }
        private Operators operators = null;
        public Operators Operators
        {
            get { return operators; }
            set { operators = value; }
        }
        private SortCategories sortCategories = null;
        public SortCategories SortCategories
        {
            get { return sortCategories; }
            set { sortCategories = value; }
        }
        private ProcessCodes processCodes = null;
        public ProcessCodes ProcessCodes
        {
            get { return processCodes; }
            set { processCodes = value; }
        }
        private ProcessNames processNames = null;
        public ProcessNames ProcessNames
        {
            get { return processNames; }
            set { processNames = value; }
        }

        public PublicProduction()
        {
            Lifespan = 0.5;
            ListType = typeof(PublicProductionRec);
            TblName = "tblProduction";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("DateHour", "DateHour");
            dBFieldMappings.AddMapping("SiteID", "SiteID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("MachineExtRef", "MachineExtRef");
            dBFieldMappings.AddMapping("MachineLongDescription", "MachineLongDescription");
            dBFieldMappings.AddMapping("MachineNorm", "MachineNorm");
            dBFieldMappings.AddMapping("MachinePositions", "MachinePositions");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("MachineGroupID", "MachineGroupID");
            dBFieldMappings.AddMapping("MachineGroupExtRef", "MachineGroupExtRef");
            dBFieldMappings.AddMapping("MachineArea", "MachineArea");
            dBFieldMappings.AddMapping("MachineGroupLongDescription", "MachineGroupLongDescription");
            dBFieldMappings.AddMapping("CustomerID", "CustomerID");
            dBFieldMappings.AddMapping("CustomerExtRef", "CustomerExtRef");
            dBFieldMappings.AddMapping("CustomerLongDescription", "CustomerLongDescription");
            dBFieldMappings.AddMapping("SortCategoryID", "SortCategoryID");
            dBFieldMappings.AddMapping("SortCategoryExtRef", "SortCategoryExtRef");
            dBFieldMappings.AddMapping("SortCategoryLongDescription", "SortCategoryLongDescription");
            dBFieldMappings.AddMapping("ArticleID", "ArticleID");
            dBFieldMappings.AddMapping("ArticleExtRef", "ArticleExtRef");
            dBFieldMappings.AddMapping("ArticleLongDescription", "ArticleLongDescription");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("OperatorExtRef", "OperatorExtRef");
            dBFieldMappings.AddMapping("OperatorLongDescription", "OperatorLongDescription");
            dBFieldMappings.AddMapping("ProcessCode", "ProcessCode");
            dBFieldMappings.AddMapping("ProcessName", "ProcessName");
            dBFieldMappings.AddMapping("ProcessNorm", "ProcessNorm");
            dBFieldMappings.AddMapping("PiecesCounter", "PiecesCounter");
            dBFieldMappings.AddMapping("RejectCounter", "RejectCounter");
            dBFieldMappings.AddMapping("RewashCounter", "RewashCounter");
            dBFieldMappings.AddMapping("BatchCounter", "BatchCounter");
            dBFieldMappings.AddMapping("WeightCounter", "WeightCounter");
            dBFieldMappings.AddMapping("WeightUnits", "WeightUnits");
            dBFieldMappings.AddMapping("ProductionTime", "ProductionTime");
            dBFieldMappings.AddMapping("StopTime", "StopTime");
            dBFieldMappings.AddMapping("NoFlowTime", "NoFlowTime");
            dBFieldMappings.AddMapping("FaultTime", "FaultTime");
            dBFieldMappings.AddMapping("FreshWater", "FreshWater");
            dBFieldMappings.AddMapping("WasteWater", "WasteWater");
            dBFieldMappings.AddMapping("WaterUnits", "WaterUnits");
            dBFieldMappings.AddMapping("GasEnergy", "GasEnergy");
            dBFieldMappings.AddMapping("GasEnergyUnits", "GasEnergyUnits");
            dBFieldMappings.AddMapping("GasVolume", "GasVolume");
            dBFieldMappings.AddMapping("GasVolumeUnits", "GasVolumeUnits");
            dBFieldMappings.AddMapping("ElectricalEnergy", "ElectricalEnergy");
            dBFieldMappings.AddMapping("ElectricityUnits", "ElectricityUnits");
            dBFieldMappings.AddMapping("SteamConsumption", "SteamConsumption");
            dBFieldMappings.AddMapping("SteamUnits", "SteamUnits");
            dBFieldMappings.AddMapping("AirConsumption", "AirConsumption");
            dBFieldMappings.AddMapping("AirUnits", "AirUnits");
            dBFieldMappings.AddMapping("CountExitPointData", "CountExitPointData");
            dBFieldMappings.AddMapping("MachineCountExitPoint", "MachineCountExitPoint");
            dBFieldMappings.AddMapping("OperatorCountExitPoint", "OperatorCountExitPoint");
            dBFieldMappings.AddMapping("ProcessCountExitPoint", "ProcessCountExitPoint");
            dBFieldMappings.AddMapping("ExcludePower", "ExcludePower");
            dBFieldMappings.AddMapping("ProductionRefData", "ProductionRefData");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public PublicProductionRec GetByIDs(int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID)
        {
            PublicProductionRec rec = null;
            foreach (PublicProductionRec prodrec in this)
            {
                if ((prodrec.MachineID == MachineID)
                    && (prodrec.SubID == Subid)
                    && (prodrec.CustomerID == CustID)
                    && (prodrec.ArticleID == ArtID)
                    && (prodrec.SortCategoryID == SortID)
                    && (prodrec.OperatorID == OpID))
                    rec = prodrec;
            }
            return rec;
        }

        //public PublicProductionRec GetByIDs(int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID, int ProcCode)
        //{
        //    PublicProductionRec rec = null;
        //    foreach (PublicProductionRec prodrec in this)
        //    {
        //        if ((prodrec.MachineID == MachineID)
        //            && (prodrec.SubID == Subid)
        //            && (prodrec.CustomerID == CustID)
        //            && (prodrec.ArticleID == ArtID)
        //            && (prodrec.SortCategoryID == SortID)
        //            && (prodrec.OperatorID == OpID)
        //            && (prodrec.ProcessCode == ProcCode))
        //            rec = prodrec;
        //    }
        //    return rec;
        //}

        public PublicProductionRec GetPublicProductionRec(DateTime datetime, int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID)
        {
            PublicProductionRec rec = null;
            foreach (PublicProductionRec prodrec in this)
            {
                if ((prodrec.DateHour == datetime)
                    && (prodrec.MachineID == MachineID)
                    && (prodrec.SubID == Subid)
                    && (prodrec.CustomerID == CustID)
                    && (prodrec.ArticleID == ArtID)
                    && (prodrec.SortCategoryID == SortID)
                    && (prodrec.OperatorID == OpID))
                    rec = prodrec;
            }
            return rec;
        }

        //public PublicProductionRec GetPublicProductionRec(DateTime datetime, int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID, int ProcCode)
        //{
        //    PublicProductionRec rec = null;
        //    foreach (PublicProductionRec prodrec in this)
        //    {
        //        if ((prodrec.DateHour == datetime)
        //            && (prodrec.MachineID == MachineID)
        //            && (prodrec.SubID == Subid)
        //            && (prodrec.CustomerID == CustID)
        //            && (prodrec.ArticleID == ArtID)
        //            && (prodrec.SortCategoryID == SortID)
        //            && (prodrec.OperatorID == OpID)
        //            && (prodrec.ProcessCode == ProcCode))
        //            rec = prodrec;
        //    }
        //    return rec;
        //}

        public void AssignValues(PublicProductionRec source, ref PublicProductionRec dest)
        {
            dest.PiecesCounter = source.PiecesCounter;
            dest.WeightCounter = source.WeightCounter;
            dest.BatchCounter = source.BatchCounter;
            dest.ProductionTime = source.ProductionTime;
            dest.StopTime = source.StopTime;
            dest.FaultTime = source.FaultTime;
            dest.NoFlowTime = source.NoFlowTime;
            dest.FreshWater = source.FreshWater;
            dest.WasteWater = source.WasteWater;
            dest.GasEnergy = source.GasEnergy;
            dest.SteamConsumption = source.SteamConsumption;
            dest.ElectricalEnergy = source.ElectricalEnergy;
            dest.AirConsumption = source.AirConsumption;
            dest.RejectCounter = source.RejectCounter;
            dest.RewashCounter = source.RewashCounter;
            dest.DeleteUnwrittenRecord = false;
        }



        public PublicProductionRec CreateProductionRec(DateTime datetime, int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID, int ProcCode)
        {
            PublicProductionRec rec = new PublicProductionRec();
            rec.ForceNew = true;
            rec.DateHour = datetime;
            rec.MachineID = MachineID;
            rec.SubID = Subid;
            rec.CustomerID = CustID;
            rec.SortCategoryID = SortID;
            rec.OperatorID = OpID;
            rec.ProcessCode = ProcCode;
            rec.MachineExtRef = "";
            rec.MachineLongDescription = "";
            rec.MachineNorm = 0;
            rec.MachinePositions = 1;
            rec.MachineArea = "";
            rec.MachineGroupExtRef = "";
            rec.MachineGroupLongDescription = "";
            rec.MachineGroupID = -1;
            rec.CustomerExtRef = "";
            rec.CustomerLongDescription = CustID.ToString();
            rec.SortCategoryExtRef = "";
            rec.SortCategoryLongDescription = SortID.ToString();
            rec.ArticleExtRef = "";
            rec.ArticleLongDescription = ArtID.ToString();
            rec.OperatorExtRef = "";
            rec.OperatorLongDescription = OpID.ToString();
            SqlDataAccess da = SqlDataAccess.Singleton;
            bool cxp = false;
            if (Machines != null)
            {
                Machine m = (Machine)Machines.GetById(rec.MachineID);
                if (m != null)
                {
                    if (processCodes != null)
                    {
                        int sortid = SortID;
                        int artid = ArtID;
                        if (m.ProcessCodeType == 0)
                        {
                            sortid = -1;
                        }
                        if (m.ProcessCodeType == 1)
                        {
                            artid = -1;
                        }
                        ProcessCode pc = processCodes.GetProcessCode(MachineID, CustID, artid, sortid, ProcCode);
                        if (pc != null)
                        {
                            cxp = pc.Count_Exit_Point;
                            rec.ProcessNorm = pc.Production_Norm;
                        }
                    }
                    if (!string.IsNullOrEmpty(m.ExtRef))
                        rec.MachineExtRef = m.ExtRef;
                    else
                        rec.MachineExtRef = rec.MachineID.ToString();
                    rec.MachineLongDescription = m.LongDescription;
                    rec.MachineNorm = m.NormValue;
                    rec.MachinePositions = m.Positions;
                    rec.MachineCountExitPoint = (m.MachineCountExitPoint && m.UseMachineCount);
                    rec.OperatorCountExitPoint = (m.OperatorCountExitPoint);
                    rec.ProcessCountExitPoint = (m.UseProcessCodeCount && cxp);
                    rec.ExcludePower = m.ExcludeFromPowerCalc;
                    rec.ProductionRefData = m.ProductionReference;
                    if (machineGroups != null)
                    {
                        MachineGroup mg = (MachineGroup)machineGroups.GetById(m.MachineGroupID);
                        if (mg != null)
                        {
                            rec.MachineArea = mg.MachineArea;
                            rec.MachineGroupExtRef = mg.ExtRef;
                            rec.MachineGroupLongDescription = mg.LongDescription;
                            rec.MachineGroupID = mg.IdJensen;
                        }
                        else
                        {
                            Debug.WriteLine("null reference for " + m.MachineGroupID);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Machine is Null for " + rec.MachineID);
                }
            }
            else
            {
                Debug.WriteLine("Machines is Null");
            }

            if (rec.MachinePositions == 0)
                rec.MachinePositions = 1;

            if (customers != null)
            {
                Customer c = (Customer)customers.GetById(CustID);
                if (c != null)
                {
                    if (!string.IsNullOrEmpty(c.ExtRef))
                        rec.CustomerExtRef = c.ExtRef;
                    else
                        rec.CustomerExtRef = CustID.ToString();
                    rec.CustomerLongDescription = c.LongDescription;
                }
            }

            rec.SortCategoryID = SortID;
            if (sortCategories != null)
            {
                SortCategory sc = (SortCategory)sortCategories.GetById(SortID);
                if (sc != null)
                {
                    if (!string.IsNullOrEmpty(sc.ExtRef))
                        rec.SortCategoryExtRef = sc.ExtRef;
                    else
                        rec.SortCategoryExtRef = SortID.ToString();
                    rec.SortCategoryLongDescription = sc.LongDescription;
                }
            }
            rec.ArticleID = ArtID;
            if (articles != null)
            {
                Article art = (Article)articles.GetById(ArtID);
                if (art != null)
                {
                    if (!string.IsNullOrEmpty(art.ExtRef))
                        rec.ArticleExtRef = art.ExtRef;
                    else
                        rec.ArticleExtRef = ArtID.ToString();
                    rec.ArticleLongDescription = art.LongDescription;
                }
            }
            rec.OperatorID = OpID;
            if (operators != null)
            {
                Operator op = (Operator)operators.GetById(OpID);
                if (op != null)
                {
                    if (!string.IsNullOrEmpty(op.ExtRef))
                        rec.OperatorExtRef = op.ExtRef;
                    else
                        rec.OperatorExtRef = OpID.ToString();
                    rec.OperatorLongDescription = op.LongDescription;
                }
            }
            rec.PiecesCounter = 0;
            rec.WeightCounter = 0;
            rec.BatchCounter = 0;
            rec.ProductionTime = 0;
            rec.StopTime = 0;
            rec.FaultTime = 0;
            rec.NoFlowTime = 0;
            rec.FreshWater = 0;
            rec.WasteWater = 0;
            rec.GasEnergy = 0;
            rec.SteamConsumption = 0;
            rec.ElectricalEnergy = 0;
            rec.AirConsumption = 0;
            rec.RejectCounter = 0;
            rec.RewashCounter = 0;
            rec.ProcessCode = ProcCode;
            if (processNames != null)
            {
                ProcessName processname = (ProcessName)processNames.GetById(MachineID, ProcCode);
                if (processname != null)
                    rec.ProcessName = processname.ProcName;
                else
                    rec.ProcessName = string.Empty;
            }
            else
            {
                rec.ProcessName = string.Empty;
            }

            rec.HasChanged = false;
            return rec;
        }

        public PublicProductionRec GetOrCreateProductionRec(DateTime datetime, int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID, int ProcCode)
        {
            PublicProductionRec rec = GetPublicProductionRec(datetime, MachineID, Subid, CustID, ArtID, SortID, OpID);
            if (rec == null)
            {
                rec = CreateProductionRec(datetime, MachineID, Subid, CustID, ArtID, SortID, OpID, ProcCode);
                this.Add(rec);
            }
            return rec;
        }

    }

    public class PublicProductionRec : DataItem
    {
        #region private fields
        private int recNum;
        private DateTime dateHour;
        private int siteID;
        private int machineID;
        private string machineExtRef;
        private string machineLongDescription;
        private int machineNorm;
        private int machinePositions;
        private int subID;
        private int machineGroupID;
        private string machineGroupExtRef;
        private string machineArea;
        private string machineGroupLongDescription;
        private int customerID;
        private string customerExtRef;
        private string customerLongDescription;
        private int sortCategoryID;
        private string sortCategoryExtRef;
        private string sortCategoryLongDescription;
        private int articleID;
        private string articleExtRef;
        private string articleLongDescription;
        private int operatorID;
        private string operatorExtRef;
        private string operatorLongDescription;
        private int processCode;
        private string processName;
        private int processNorm;
        private int piecesCounter;
        private int rejectCounter;
        private int rewashCounter;
        private int batchCounter;
        private double weightCounter;
        private int weightUnits;
        private int productionTime;
        private int stopTime;
        private int noFlowTime;
        private int faultTime;
        private double freshWater;
        private double wasteWater;
        private int waterUnits;
        private double gasEnergy;
        private int gasEnergyUnits;
        private double gasVolume;
        private int gasVolumeUnits;
        private double electricalEnergy;
        private int electricityUnits;
        private double steamConsumption;
        private int steamUnits;
        private double airConsumption;
        private int airUnits;
        private bool countExitPointData;
        private bool machineCountExitPoint;
        private bool operatorCountExitPoint;
        private bool processCountExitPoint;
        private bool excludePower;
        private bool productionRefData;

        #endregion

        #region Properties
        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = value;
                PrimaryKey = value;
            }
        }
        public DateTime DateHour
        {
            get { return dateHour; }
            set { dateHour = AssignNotify(ref dateHour, value, "DateHour"); }
        }
        public int SiteID
        {
            get { return siteID; }
            set { siteID = AssignNotify(ref siteID, value, "SiteID"); }
        }
        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }
        public string MachineExtRef
        {
            get { return machineExtRef; }
            set { machineExtRef = AssignNotify(ref machineExtRef, value, "MachineExtRef"); }
        }
        public string MachineLongDescription
        {
            get { return machineLongDescription; }
            set { machineLongDescription = AssignNotify(ref machineLongDescription, value, "MachineLongDescription"); }
        }
        public int MachineNorm
        {
            get { return machineNorm; }
            set { machineNorm = AssignNotify(ref machineNorm, value, "MachineNorm"); }
        }
        public int MachinePositions
        {
            get { return machinePositions; }
            set { machinePositions = AssignNotify(ref machinePositions, value, "MachinePositions"); }
        }
        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }
        public int MachineGroupID
        {
            get { return machineGroupID; }
            set { machineGroupID = AssignNotify(ref machineGroupID, value, "MachineGroupID"); }
        }
        public string MachineGroupExtRef
        {
            get {
                if (machineGroupExtRef != null)
                    return machineGroupExtRef;
                else
                    return string.Empty;
            }
            set { machineGroupExtRef = AssignNotify(ref machineGroupExtRef, value, "MachineGroupExtRef"); }
        }
        public string MachineArea
        {
            get { return machineArea; }
            set { machineArea = AssignNotify(ref machineArea, value, "MachineArea"); }
        }
        public string MachineGroupLongDescription
        {
            get { return machineGroupLongDescription; }
            set { machineGroupLongDescription = AssignNotify(ref machineGroupLongDescription, value, "MachineGroupLongDescription"); }
        }
        public int CustomerID
        {
            get { return customerID; }
            set { customerID = AssignNotify(ref customerID, value, "CustomerID"); }
        }
        public string CustomerExtRef
        {
            get { return customerExtRef; }
            set { customerExtRef = AssignNotify(ref customerExtRef, value, "CustomerExtRef"); }
        }
        public string CustomerLongDescription
        {
            get { return customerLongDescription; }
            set { customerLongDescription = AssignNotify(ref customerLongDescription, value, "CustomerLongDescription"); }
        }
        public int SortCategoryID
        {
            get { return sortCategoryID; }
            set { sortCategoryID = AssignNotify(ref sortCategoryID, value, "SortCategoryID"); }
        }
        public string SortCategoryExtRef
        {
            get { return sortCategoryExtRef; }
            set { sortCategoryExtRef = AssignNotify(ref sortCategoryExtRef, value, "SortCategoryExtRef"); }
        }
        public string SortCategoryLongDescription
        {
            get { return sortCategoryLongDescription; }
            set { sortCategoryLongDescription = AssignNotify(ref sortCategoryLongDescription, value, "SortCategoryLongDescription"); }
        }
        public int ArticleID
        {
            get { return articleID; }
            set { articleID = AssignNotify(ref articleID, value, "ArticleID"); }
        }
        public string ArticleExtRef
        {
            get { return articleExtRef; }
            set { articleExtRef = AssignNotify(ref articleExtRef, value, "ArticleExtRef"); }
        }
        public string ArticleLongDescription
        {
            get { return articleLongDescription; }
            set { articleLongDescription = AssignNotify(ref articleLongDescription, value, "ArticleLongDescription"); }
        }
        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
        }
        public string OperatorExtRef
        {
            get { return operatorExtRef; }
            set { operatorExtRef = AssignNotify(ref operatorExtRef, value, "OperatorExtRef"); }
        }
        public string OperatorLongDescription
        {
            get { return operatorLongDescription; }
            set { operatorLongDescription = AssignNotify(ref operatorLongDescription, value, "OperatorLongDescription"); }
        }
        public int ProcessCode
        {
            get { return processCode; }
            set { processCode = AssignNotify(ref processCode, value, "ProcessCode"); }
        }
        public string ProcessName
        {
            get { return processName; }
            set { processName = AssignNotify(ref processName, value, "ProcessName"); }
        }
        public int ProcessNorm
        {
            get { return processNorm; }
            set { processNorm = AssignNotify(ref machineNorm, value, "ProcessNorm"); }
        }
        public int PiecesCounter
        {
            get { return piecesCounter; }
            set { piecesCounter = AssignNotify(ref piecesCounter, value, "PiecesCounter"); }
        }
        public int RejectCounter
        {
            get { return rejectCounter; }
            set { rejectCounter = AssignNotify(ref rejectCounter, value, "RejectCounter"); }
        }
        public int RewashCounter
        {
            get { return rewashCounter; }
            set { rewashCounter = AssignNotify(ref rewashCounter, value, "RewashCounter"); }
        }
        public int BatchCounter
        {
            get { return batchCounter; }
            set { batchCounter = AssignNotify(ref batchCounter, value, "BatchCounter"); }
        }
        public double WeightCounter
        {
            get { return Math.Round(weightCounter, 5); }
            set { weightCounter = AssignNotify(ref weightCounter, value, "WeightCounter"); }
        }
        public int WeightUnits
        {
            get { return weightUnits; }
            set { weightUnits = AssignNotify(ref weightUnits, value, "WeightUnits"); }
        }
        public int ProductionTime
        {
            get { return productionTime; }
            set { productionTime = AssignNotify(ref productionTime, value, "ProductionTime"); }
        }
        public int StopTime
        {
            get { return stopTime; }
            set { stopTime = AssignNotify(ref stopTime, value, "StopTime"); }
        }
        public int NoFlowTime
        {
            get { return noFlowTime; }
            set { noFlowTime = AssignNotify(ref noFlowTime, value, "NoFlowTime"); }
        }
        public int FaultTime
        {
            get { return faultTime; }
            set { faultTime = AssignNotify(ref faultTime, value, "FaultTime"); }
        }
        public double FreshWater
        {
            get { return Math.Round(freshWater, 5); }
            set { freshWater = AssignNotify(ref freshWater, value, "FreshWater"); }
        }
        public double WasteWater
        {
            get { return Math.Round(wasteWater, 5); }
            set { wasteWater = AssignNotify(ref wasteWater, value, "WasteWater"); }
        }
        public int WaterUnits
        {
            get { return waterUnits; }
            set { waterUnits = AssignNotify(ref waterUnits, value, "WaterUnits"); }
        }
        public double GasEnergy
        {
            get { return Math.Round(gasEnergy, 5); }
            set { gasEnergy = AssignNotify(ref gasEnergy, value, "GasEnergy"); }
        }
        public int GasEnergyUnits
        {
            get { return gasEnergyUnits; }
            set { gasEnergyUnits = AssignNotify(ref gasEnergyUnits, value, "GasEnergyUnits"); }
        }
        public double GasVolume
        {
            get { return Math.Round(gasVolume, 5); }
            set { gasVolume = AssignNotify(ref gasVolume, value, "GasVolume"); }
        }
        public int GasVolumeUnits
        {
            get { return gasVolumeUnits; }
            set { gasVolumeUnits = AssignNotify(ref gasVolumeUnits, value, "GasVolumeUnits"); }
        }
        public double ElectricalEnergy
        {
            get { return Math.Round(electricalEnergy, 5); }
            set { electricalEnergy = AssignNotify(ref electricalEnergy, value, "ElectricalEnergy"); }
        }
        public int ElectricityUnits
        {
            get { return electricityUnits; }
            set { electricityUnits = AssignNotify(ref electricityUnits, value, "ElectricityUnits"); }
        }
        public double SteamConsumption
        {
            get { return Math.Round(steamConsumption, 5); }
            set { steamConsumption = AssignNotify(ref steamConsumption, value, "SteamConsumption"); }
        }
        public int SteamUnits
        {
            get { return steamUnits; }
            set { steamUnits = AssignNotify(ref steamUnits, value, "SteamUnits"); }
        }
        public double AirConsumption
        {
            get { return Math.Round(airConsumption, 5); }
            set { airConsumption = AssignNotify(ref airConsumption, value, "AirConsumption"); }
        }
        public int AirUnits
        {
            get { return airUnits; }
            set { airUnits = AssignNotify(ref airUnits, value, "AirUnits"); }
        }
        public bool CountExitPointData
        {
            get { return countExitPointData; }
            set { setCountExitPointData(); }
        }

        private void setCountExitPointData()
        {
            countExitPointData = MachineCountExitPoint || OperatorCountExitPoint || ProcessCountExitPoint;
        }

        public bool MachineCountExitPoint
        {
            get { return machineCountExitPoint; }
            set
            {
                machineCountExitPoint = AssignNotify(ref machineCountExitPoint, value, "MachineCountExitPoint");
                setCountExitPointData();
            }
        }
        public bool OperatorCountExitPoint
        {
            get { return operatorCountExitPoint; }
            set { operatorCountExitPoint = AssignNotify(ref operatorCountExitPoint, value, "OperatorCountExitPoint"); }
        }
        public bool ProcessCountExitPoint
        {
            get { return processCountExitPoint; }
            set
            {
                processCountExitPoint = AssignNotify(ref processCountExitPoint, value, "ProcessCountExitPoint"); 
                setCountExitPointData();
            }
        }
        public bool ExcludePower
        {
            get { return excludePower; }
            set
            {
                excludePower = AssignNotify(ref excludePower, value, "ExcludePower");
                setCountExitPointData();
            }
        }
        public bool ProductionRefData
        {
            get { return productionRefData; }
            set { productionRefData = AssignNotify(ref productionRefData, value, "ProductionRefData"); }
        }


        #endregion

    }

    public class PublicKpis : DataList
    {
        public PublicKpis()
        {
            Lifespan = 1.0;
            ListType = typeof(PublicKpi);
            TblName = "tblKPI";
            DbName = "JEGR_Public";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("KpiID", "KpiID", true, true);
            dBFieldMappings.AddMapping("DateHour", "DateHour");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("KpiType", "KpiType");
            dBFieldMappings.AddMapping("KpiSubType", "KpiSubType");
            dBFieldMappings.AddMapping("KpiSubTypeID", "KpiSubTypeID");
            dBFieldMappings.AddMapping("Value", "Value");
            dBFieldMappings.AddMapping("Unit", "Unit");
            dBFieldMappings.AddMapping("KpiTime", "KpiTime");
            dBFieldMappings.AddMapping("ExpectedValue", "ExpectedValue");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public PublicKpi GetMachineSubKpi(DateTime dateHour, int machineID, int subID, KpiDataType datatype)
        {
            PublicKpi kpiRec = null;
            bool found = false;
            int index = 0;
            while (!found && index < this.Count)
            {
                kpiRec = (PublicKpi)this[index];
                found = (kpiRec.DateHour == dateHour) &&
                    (kpiRec.MachineID == machineID) &&
                    (kpiRec.SubID == subID) &&
                    (kpiRec.DataType == datatype);
                index++;
            }
            if (found)
                return kpiRec;
            else
                return null;
        }

        public PublicKpi GetByDateHourMachineSubKpis(DateTime dateHour, int machineID, int subID, int kpiType, int kpiSubType, int kpiSubTypeID)
        {
            PublicKpi kpiRec = null;
            bool found = false;
            int index = 0;
            while (!found && index < this.Count)
            {
                kpiRec = (PublicKpi)this[index];
                found = (kpiRec.DateHour==dateHour) &&
                    (kpiRec.MachineID == machineID) &&
                    (kpiRec.SubID == subID) &&
                    (kpiRec.KpiType == kpiType) &&
                    (kpiRec.KpiSubType == kpiSubType) &&
                    (kpiRec.KpiSubTypeID == kpiSubTypeID);
                index++;
            }
            if (found)
                return kpiRec;
            else
                return null;
        }

        public void DeleteDateHour(DateTime dateHour)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.ExecuteNonQuery(DbName, "DELETE FROM " + TblName + " WHERE DateHour = " + dateHour.ToString("yyyyMMdd HH:mm:ss"));
        }

        public PublicKpi GetOrCreateMachineSubidKpi(DateTime datetime, int machineid, int subid, KpiDataType datatype)
        {
            PublicKpi rec = GetMachineSubKpi(datetime, machineid, subid, datatype);
            if (rec == null)
            {
                rec = new PublicKpi();
                rec.DateHour = datetime;
                rec.MachineID = machineid;
                rec.SubID = subid;
                switch (datatype)
                {
                    case KpiDataType.Production:
                        rec.KpiType = 1;
                        rec.KpiSubType = 0;
                        rec.KpiSubTypeID = 0;
                        break;
                    case KpiDataType.FreshwaterConsumption:
                        rec.KpiType = 2;
                        rec.KpiSubType = 30;
                        rec.KpiSubTypeID = 77;
                        break;
                    case KpiDataType.WasteWater:
                        rec.KpiType = 2;
                        rec.KpiSubType = 30;
                        rec.KpiSubTypeID = 900;
                        break;
                    case KpiDataType.GasConsumption:
                        rec.KpiType = 2;
                        rec.KpiSubType = 40;
                        break;
                    case KpiDataType.ElectricityConsumption:
                        rec.KpiType = 2;
                        rec.KpiSubType = 20;
                        break;
                    default:
                        rec.KpiType = 1;
                        break;
                }
                rec.ForceNew = true;
                this.Add(rec);
            }
            return rec;
        }
   
    }

    public class PublicKpi : DataItem
    {
        #region private fields
        private int kpiID;
        private DateTime dateHour;
        private int machineID;
        private int subID;
        private int kpiType;
        private int kpiSubType;
        private int kpiSubTypeID;
        private double value;
        private int unit;
        private double kpiTime;
        private int expectedValue;
        #endregion

        #region Properties
        public int KpiID
        {
            get { return kpiID; }
            set
            {
                kpiID = AssignNotify(ref kpiID, value, "KpiID");
                ID = kpiID;
                PrimaryKey = kpiID;
            }
        }
        public DateTime DateHour
        {
            get { return dateHour; }
            set { dateHour = AssignNotify(ref dateHour, value, "DateHour"); }
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
        public int KpiType
        {
            get { return kpiType; }
            set { kpiType = AssignNotify(ref kpiType, value, "KpiType"); }
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
        public double Value
        {
            get { return this.value; }
            set { this.value = AssignNotify(ref this.value, value, "Value"); }
        }
        public int Unit
        {
            get { return unit; }
            set { unit = AssignNotify(ref unit, value, "Unit"); }
        }
        public double KpiTime
        {
            get { return kpiTime; }
            set { kpiTime = AssignNotify(ref kpiTime, value, "KpiTime"); }
        }
        public int ExpectedValue
        {
            get { return expectedValue; }
            set { expectedValue = AssignNotify(ref expectedValue, value, "ExpectedValue"); }
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

                return dataType;
            }
        }
        
        #endregion

    }

}
