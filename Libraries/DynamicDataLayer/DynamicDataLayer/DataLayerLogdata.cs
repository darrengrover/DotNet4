using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Dynamic.DataLayer
{

    #region jglogdata

    partial class SqlDataAccess
    {
        public JGLogData GetJGLogAliasSourceData(DateTime startTime, DateTime endTime, int machineID, int remoteID)
        {
            JGLogData aliasLogData = new JGLogData();
            aliasLogData.TimeoutSeconds = 120;
            aliasLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
            aliasLogData.AddSelectRestriction(new RestrictionRec("MachineID", machineID, ComparisonType.Equal));
            aliasLogData.AddSelectRestriction(new RestrictionRec("RemoteID", remoteID, ComparisonType.MoreThan));
            aliasLogData.IsValid = false;
            aliasLogData = (JGLogData)DBDataListSelect(aliasLogData);
            return aliasLogData;
        }

        public JGLogData GetJGLogKPISourceData(DateTime startTime, DateTime endTime, Machines machines)
        {
            List<int> machineList = new List<int>();
            List<int> regTypeList = new List<int>();
            JGLogData kpiLogData = new JGLogData();
            kpiLogData.RaiseException = true;
            kpiLogData.TimeoutSeconds = 120;

            if (machines != null)
            {
                foreach (Machine machine in machines)
                {
                    if ((machine.UseMachineCount && machine.MachineCountExitPoint) || !machine.UseMachineCount || machine.OperatorCountExitPoint || !machine.ExcludeFromPowerCalc)
                        machineList.Add(machine.IdJensen);

                }
                //machineList.Add(2123);
                //machineList.Add(2122);
                //machineList.Add(2153);
                kpiLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
                kpiLogData.AddSelectRestriction(new RestrictionRec("MachineID", machineList, ComparisonType.InList));
                regTypeList.Add(0); //state changes
                regTypeList.Add(1); //production
                regTypeList.Add(2); //consumption
                regTypeList.Add(6); //operator login/out
                //regTypeList.Add(7); //measurement
                kpiLogData.AddSelectRestriction(new RestrictionRec("RegType", regTypeList, ComparisonType.InList));
                kpiLogData = (JGLogData)DBDataListSelect(kpiLogData, true, false);
            }
            kpiLogData.SortByMachineTimeRemote();
            kpiLogData.RaiseException = false;
            return kpiLogData;
        }


        public JGLogData GetJGLogMachineProductionData(DateTime startTime, DateTime endTime, List<int> machineList)
        {
            List<int> regTypeList = new List<int>();
            JGLogData kpiLogData = new JGLogData();
            kpiLogData.TimeoutSeconds = 120;
            if (machineList != null)
            {
                kpiLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
                kpiLogData.AddSelectRestriction(new RestrictionRec("MachineID", machineList, ComparisonType.InList));
                regTypeList.Add(1); //production
                kpiLogData.AddSelectRestriction(new RestrictionRec("RegType", regTypeList, ComparisonType.InList));
                kpiLogData = (JGLogData)DBDataListSelect(kpiLogData, true, false);
            }
            kpiLogData.SortByMachineTimeRemote();
            return kpiLogData;
        }

        public JGLogData GetJGLogOperatorData(DateTime startTime, DateTime endTime)
        {
            JGLogData jgLogData = new JGLogData();
            jgLogData.RaiseException = true;
            jgLogData.TimeoutSeconds = 120;
            jgLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
            jgLogData.AddSelectRestriction(new RestrictionRec("RegType", 6, ComparisonType.Equal));
            jgLogData = (JGLogData)DBDataListSelect(jgLogData, true, false);
            return jgLogData;
        }


        public JGLogData GetJGLogPublicSourceData(DateTime startTime, DateTime endTime, Machines machines)
        {
            List<int> machineList = new List<int>();
            List<int> regTypeList = new List<int>();
            JGLogData jgLogData = new JGLogData();
            jgLogData.RaiseException = true;
            jgLogData.TimeoutSeconds = 120;

            if (machines != null)
            {
                foreach (Machine machine in machines)
                {
                    if ((machine.UseMachineCount && machine.MachineCountExitPoint) || !machine.UseMachineCount || machine.OperatorCountExitPoint || !machine.ExcludeFromPowerCalc)
                        machineList.Add(machine.IdJensen);
                }
                jgLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
                jgLogData.AddSelectRestriction(new RestrictionRec("MachineID", machineList, ComparisonType.InList));
                //machineList.Add(2122); //debug stuff xmas 2016
                //jgLogData.AddSelectRestriction(new RestrictionRec("Subid",1));
                //jgLogData.AddSelectRestriction(new RestrictionRec("RegType", 1));
                //int subreg = 0;
                //jgLogData.AddSelectRestriction(new RestrictionRec("SubRegType", subreg));
                regTypeList.Add(0); //state                
                regTypeList.Add(1); //production
                regTypeList.Add(2); //consumption
                regTypeList.Add(6); //operator login/out
                regTypeList.Add(7); //measurement
                jgLogData.AddSelectRestriction(new RestrictionRec("RegType", regTypeList, ComparisonType.InList));
                jgLogData = (JGLogData)DBDataListSelect(jgLogData, true, false);
            }
            jgLogData.SortByMachineTimeRemote();
            return jgLogData;
        }

        public JGRTLogData GetPeriodRTLogData(DateTime startTime, DateTime endTime, Machines machines, int machineID)
        {
            List<int> regTypeList = new List<int>();
            JGRTLogData jgRTLogData = new JGRTLogData();
            Machine machine = null;
            if (machineID > 0)
            {
                if (machines != null)
                {
                    machine = (Machine)machines.GetById(machineID);
                }
            } 
            jgRTLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
            if (machine != null)
                jgRTLogData.AddSelectRestriction(new RestrictionRec("MachineID", machineID, ComparisonType.Equal));
            regTypeList.Add(7); //measurement
            jgRTLogData.AddSelectRestriction(new RestrictionRec("RegType", regTypeList, ComparisonType.InList));
            int batchid = 0;
            jgRTLogData.AddSelectRestriction(new RestrictionRec("BatchID", batchid, ComparisonType.MoreThan));
            jgRTLogData = (JGRTLogData)DBDataListSelect(jgRTLogData, true, false);

            jgRTLogData.SortByMachineTimeRemote();
            return jgRTLogData;
        }
   

        public JGRTLogData GetPeriodRTLogData(DateTime startTime, DateTime endTime, Machine machine)
        {
            List<int> regTypeList = new List<int>();
            JGRTLogData jgRTLogData = new JGRTLogData();
            jgRTLogData.AddSelectRestriction(new RestrictionRec("TimeStamp", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
            if (machine != null)
                jgRTLogData.AddSelectRestriction(new RestrictionRec("MachineID", machine.IdJensen, ComparisonType.Equal));
            regTypeList.Add(7); //measurement
            jgRTLogData.AddSelectRestriction(new RestrictionRec("RegType", regTypeList, ComparisonType.InList));
            int batchid = 0;
            jgRTLogData.AddSelectRestriction(new RestrictionRec("BatchID", batchid, ComparisonType.MoreThan));
            jgRTLogData = (JGRTLogData)DBDataListSelect(jgRTLogData, true, false);

            jgRTLogData.SortByMachineTimeRemote();
            return jgRTLogData;
        }
    }

    public class JGLogData : DataList
    {
        public JGLogData()
        {
            Lifespan = 1.0;
            ListType = typeof(JGLogDataRec);
            TblName = "tblJGLogData";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("RemoteID", "RemoteID");
            dBFieldMappings.AddMapping("CompanyID", "CompanyID");
            dBFieldMappings.AddMapping("TimeStamp", "TimeStamp");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("PositionID", "PositionID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("SubIDName", "SubIDName");
            dBFieldMappings.AddMapping("RegType", "RegType");
            dBFieldMappings.AddMapping("SubRegType", "SubRegType");
            dBFieldMappings.AddMapping("SubRegTypeID", "SubRegTypeID");
            dBFieldMappings.AddMapping("State", "State");
            dBFieldMappings.AddMapping("MessageA", "MessageA");
            dBFieldMappings.AddMapping("MessageB", "MessageB");
            dBFieldMappings.AddMapping("BatchID", "BatchID");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            dBFieldMappings.AddMapping("ProcessCode", "ProcessCode");
            dBFieldMappings.AddMapping("ProcessName", "ProcessName");
            dBFieldMappings.AddMapping("CustNo", "CustomerID");
            dBFieldMappings.AddMapping("SortCategoryID", "SortCategoryID");
            dBFieldMappings.AddMapping("ArtNo", "ArticleID");
            dBFieldMappings.AddMapping("OperatorNo", "OperatorID");
            dBFieldMappings.AddMapping("Value", "Value");
            dBFieldMappings.AddMapping("Unit", "Unit");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public override string BuildInsertCommandString(SqlConnection dBConnection)
        {
            string commandString = sqlInsertCommand;
            if (commandString == string.Empty)
            {
                if (GetTableFields(dBConnection))
                {
                    int actualColumns = 0;
                    commandString = "EXECUTE [dbo].[spInsertLogData] ";
                    for (int i = 0; i < sqlColumns.Count; i++)
                    {
                        SqlColumn sCol = sqlColumns[i];
                        DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                        if ((dbMapping != null) && (!dbMapping.IsIdentity))
                        {
                            if (actualColumns > 0)
                                commandString += ", "; 
                            commandString += "@" + sCol.ColumnName;
                            actualColumns++;
                        }
                    }
                }
            }
            sqlInsertCommand = commandString;
            return commandString;
        }

        public JGLogData GetByMachine(int machineid)
        {
            JGLogData jgLogData = new JGLogData();
            foreach (JGLogDataRec rec in this)
            {
                if (rec.MachineID == machineid)
                {
                    jgLogData.Add(rec);
                }
            }
            return jgLogData;
        }

        public void SortByMachineTimeRemote()
        {
            IsSorted = true;
            SortLogData sld = new SortLogData();
            this.Sort(sld);
        }

    }

    public class JGRTLogData : JGLogData
    {
        public JGRTLogData()
            : base()
        {
            TblName = "tblJGRTLogData";
        }
    }

    public class SortLogData : IComparer<DataItem>
    {
        public int Compare(DataItem x,DataItem y)
        {
            JGLogDataRec a = (JGLogDataRec)x;
            JGLogDataRec b = (JGLogDataRec)y;
            int result = a.MachineID.CompareTo(b.MachineID);
            //if (result == 0)
            //{
            //    result = a.SubID.CompareTo(b.SubID);
            //}
            if (result == 0)
            {
                result = a.TimeStamp.CompareTo(b.TimeStamp);
            }
            if (result == 0)
            {
                result = a.RemoteID.CompareTo(b.RemoteID);
            }
            return result;
        }
    }

    public class JGLogDataRec : DataItem 
    {
        #region private fields

        private int recNum;
        private int remoteID;
        private int companyID;
        private DateTime timeStamp;
        private int machineID;
        private int positionID;
        private int subID;
        private string subIDName;
        private int regType;
        private int subRegType;
        private int subRegTypeID;
        private int state;
        private string messageA;
        private string messageB;
        private int batchID;
        private int sourceID;
        private int processCode;
        private string processName;
        private int customerID;
        private int sortCategoryID;
        private int articleID;
        private int operatorID;
        private double value;
        private int unit;


        #endregion

        #region Data Column Properties

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
        public int CompanyID
        {
            get { return companyID; }
            set { companyID = AssignNotify(ref companyID, value, "CompanyID"); }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = AssignNotify(ref timeStamp, value, "TimeStamp"); }
        }
        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }
        public int PositionID
        {
            get { return positionID; }
            set { positionID = AssignNotify(ref positionID, value, "PositionID"); }
        }
        public int SubID
        {
            get { return subID; }
            set
            {
                int id = value;
                if (id == 0) id = -1;
                subID = AssignNotify(ref subID, id, "SubID");
            }
        }
        public string SubIDName
        {
            get { return subIDName; }
            set { subIDName = AssignNotify(ref subIDName, value, "SubIDName"); }
        }
        public int RegType
        {
            get { return regType; }
            set { regType = AssignNotify(ref regType, value, "RegType"); }
        }
        public int SubRegType
        {
            get { return subRegType; }
            set { subRegType = AssignNotify(ref subRegType, value, "SubRegType"); }
        }
        public int SubRegTypeID
        {
            get { return subRegTypeID; }
            set { subRegTypeID = AssignNotify(ref subRegTypeID, value, "SubRegTypeID"); }
        }
        public int State
        {
            get { return state; }
            set { state = AssignNotify(ref state, value, "State"); }
        }
        public string MessageA
        {
            get { return messageA; }
            set { messageA = AssignNotify(ref messageA, value, "MessageA"); }
        }
        public string MessageB
        {
            get { return messageB; }
            set { messageB = AssignNotify(ref messageB, value, "MessageB"); }
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
        public string ProcessName
        {
            get { return processName; }
            set { processName = AssignNotify(ref processName, value, "ProcessName"); }
        }
        public int CustomerID
        {
            get { return customerID; }
            set { customerID = AssignNotify(ref customerID, value, "CustomerID"); }
        }
        public int SortCategoryID
        {
            get { return sortCategoryID; }
            set { sortCategoryID = AssignNotify(ref sortCategoryID, value, "SortCategoryID"); }
        }
        public int ArticleID
        {
            get { return articleID; }
            set { articleID = AssignNotify(ref articleID, value, "ArticleID"); }
        }
        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
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

        #endregion

        //#region IComparable


        //public override int CompareToCustom(DataItem other)
        //{
        //    JGLogDataRec otherLog = (JGLogDataRec)other;
        //    int result = MachineID.CompareTo(otherLog.MachineID);
        //    if (result == 0)
        //    {
        //        result = SubID.CompareTo(otherLog.SubID);
        //    }
        //    if (result == 0)
        //    {
        //        result = TimeStamp.CompareTo(otherLog.TimeStamp);
        //    }
        //    if (result == 0)
        //    {
        //        result = RemoteID.CompareTo(otherLog.RemoteID);
        //    }
        //    return result;
        //}

        //#endregion

    }

    #endregion

    #region EcoSafeguard ??
    public class EcoSafeguardLog : DataList
    {
        public EcoSafeguardLog()
        {
            Lifespan = 1.0;
            ListType = typeof(EcoSafeguardLogRec);
            TblName = "tblEcoSafeguardLog";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("RemoteID", "RemoteID");
            dBFieldMappings.AddMapping("CompanyID", "CompanyID");
            dBFieldMappings.AddMapping("TimeStamp", "TimeStamp");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("SubIDName", "SubIDName");
            dBFieldMappings.AddMapping("RegType", "RegType");
            dBFieldMappings.AddMapping("SubRegType", "SubRegType");
            dBFieldMappings.AddMapping("SubRegTypeID", "SubRegTypeID");
            dBFieldMappings.AddMapping("State", "State");
            dBFieldMappings.AddMapping("Value", "Value");
            dBFieldMappings.AddMapping("Unit", "Unit");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class EcoSafeguardLogRec : DataItem
    {
        #region private fields

        private int recNum;
        private int remoteID;
        private int companyID;
        private DateTime timeStamp;
        private int machineID;
        private int subID;
        private int regType;
        private int subRegType;
        private int subRegTypeID;
        private int state;
        private double value;
        private int unit;


        #endregion

        #region Data Column Properties

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
        public int CompanyID
        {
            get { return companyID; }
            set { companyID = AssignNotify(ref companyID, value, "CompanyID"); }
        }
        public DateTime TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = AssignNotify(ref timeStamp, value, "TimeStamp"); }
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
        public int RegType
        {
            get { return regType; }
            set { regType = AssignNotify(ref regType, value, "RegType"); }
        }
        public int SubRegType
        {
            get { return subRegType; }
            set { subRegType = AssignNotify(ref subRegType, value, "SubRegType"); }
        }
        public int SubRegTypeID
        {
            get { return subRegTypeID; }
            set { subRegTypeID = AssignNotify(ref subRegTypeID, value, "SubRegTypeID"); }
        }
        public int State
        {
            get { return state; }
            set { state = AssignNotify(ref state, value, "State"); }
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

        #endregion
    } 
    #endregion



}
