using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data
        const string getPublicOperatorsCmd =

        @"SELECT [RecNum]
                  ,[DateHour]
                  ,[MachineID]
                  ,[Machine_ExtRef]
                  ,[Machine_LongDescription]
                  ,[Machine_NormValue]
                  ,[Machine_Positions]
                  ,[MachineGroup_ExtRef]
                  ,[MachineArea]
                  ,[MachineGroup_LongDescription]
                  ,[SubID]
                  ,[OperatorID]
                  ,[Operator_ExtRef]
                  ,[Operator_LongDescription]
                  ,[Pieces_Counter]
                  ,[Weight_Counter]
                  ,[Production_Time]
                  ,[Stop_Time]
                  ,[NoFlow_Time]
                  ,[Fault_Time]
                  ,[Reject_Counter]
                  ,[Rewash_Counter]
              FROM [dbo].[tblOperatorProcess]
              WHERE [DateHour]>=@StartTime
                AND [DateHour]<@EndTime
                AND OperatorID>=0 ";

        const string getCombinedCmd =
            @"SELECT [RecNum]
                  ,[DateHour]
                  ,[MachineID]
                  ,[Machine_ExtRef]
                  ,[Machine_LongDescription]
                  ,[NormValue] as [Machine_NormValue]
                  ,[Machine_Positions]
                  ,[MachineGroups_ExtRef] as [MachineGroup_ExtRef]
                  ,[MachineGroups_Area] as [MachineArea]
                  ,[MachineGroups_LongDescription] as [MachineGroup_LongDescription]
                  ,[SubID]
                  ,[OperatorID]
                  ,[Operator_ExtRef]
                  ,[Operator_LongDescription]
                  ,[Pieces_Counter]
                  ,[Weight_Counter]
                  ,[Production_Time]
                  ,[Stop_Time]
                  ,[NoFlow_Time]
                  ,[Fault_Time]
                  ,[Reject_Counter]
                  ,[Rewash_Counter]
              FROM [dbo].[viewOperatorProcess]
              WHERE [DateHour]>=@StartTime
                AND [DateHour]<@EndTime
                AND OperatorID>=0 ";

        public OperatorProcess GetPublicOperators(DateTime startTime, DateTime endTime)
        {
            return GetPublicOperators(startTime, endTime, "");
        }

        public OperatorProcess GetPublicOperators(DateTime startTime,DateTime endTime, string conditions)
        {
            try
            {
                SqlDataAccess da = SqlDataAccess.singleton;
                string cmd = getPublicOperatorsCmd;
                if (da.combinedProduction)
                    cmd = getCombinedCmd;
                string commandString = cmd + conditions + 
                    " ORDER BY Operator_LongDescription, Machine_LongDescription" ;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    OperatorProcess pOperators = new OperatorProcess();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.DataFill(pOperators , SqlDataConnection.DBConnection.JensenPublic);
                    return pOperators;
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

        #region Insert Data
        public void InsertNewDataRec(PublicOperator pOperator)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblOperatorProcess]
                           ([DateHour]
                           ,[MachineID]
                           ,[Machine_ExtRef]
                           ,[Machine_LongDescription]
                           ,[Machine_NormValue]
                           ,[Machine_Positions]
                           ,[MachineGroup_ExtRef]
                           ,[MachineArea]
                           ,[MachineGroup_LongDescription]
                           ,[SubID]
                           ,[OperatorID]
                           ,[Operator_ExtRef]
                           ,[Operator_LongDescription]
                           ,[Pieces_Counter]
                           ,[Weight_Counter]
                           ,[Production_Time]
                           ,[Stop_Time]
                           ,[NoFlow_Time]
                           ,[Fault_Time]
                           ,[Reject_Counter]
                           ,[Rewash_Counter])
                 VALUES
                           (@DateHour
                           ,@MachineID
                           ,@Machine_ExtRef
                           ,@Machine_LongDescription
                           ,@Machine_NormValue
                           ,@Machine_Positions
                           ,@MachineGroup_ExtRef
                           ,@MachineArea
                           ,@MachineGroup_LongDescription
                           ,@SubID
                           ,@OperatorID
                           ,@Operator_ExtRef
                           ,@Operator_LongDescription
                           ,@Pieces_Counter
                           ,@Weight_Counter
                           ,@Production_Time
                           ,@Stop_Time
                           ,@NoFlow_Time
                           ,@Fault_Time
                           ,@Reject_Counter
                           ,@Rewash_Counter)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", pOperator.DateHour);
                    command.Parameters.AddWithValue("@MachineID", pOperator.MachineID);
                    command.Parameters.AddWithValue("@Machine_ExtRef", pOperator.Machine_ExtRef);
                    command.Parameters.AddWithValue("@Machine_LongDescription", pOperator.Machine_LongDescription);
                    command.Parameters.AddWithValue("@Machine_NormValue", pOperator.Machine_NormValue);
                    command.Parameters.AddWithValue("@Machine_Positions", pOperator.Machine_Positions);
                    command.Parameters.AddWithValue("@MachineGroup_ExtRef", pOperator.MachineGroup_ExtRef);
                    command.Parameters.AddWithValue("@MachineArea", pOperator.MachineArea);
                    command.Parameters.AddWithValue("@MachineGroup_LongDescription", pOperator.MachineGroup_LongDescription);
                    command.Parameters.AddWithValue("@SubID", pOperator.SubID);
                    command.Parameters.AddWithValue("@OperatorID", pOperator.OperatorID);
                    command.Parameters.AddWithValue("@Operator_ExtRef", pOperator.Operator_ExtRef);
                    command.Parameters.AddWithValue("@Operator_LongDescription", pOperator.Operator_LongDescription);
                    command.Parameters.AddWithValue("@Pieces_Counter", pOperator.Pieces_Counter);
                    command.Parameters.AddWithValue("@Weight_Counter", pOperator.Weight_Counter);
                    command.Parameters.AddWithValue("@Production_Time", pOperator.Production_Time);
                    command.Parameters.AddWithValue("@Stop_Time", pOperator.Stop_Time);
                    command.Parameters.AddWithValue("@NoFlow_Time", pOperator.NoFlow_Time);
                    command.Parameters.AddWithValue("@Fault_Time", pOperator.Fault_Time);
                    command.Parameters.AddWithValue("@Reject_Counter", pOperator.Reject_Counter);
                    command.Parameters.AddWithValue("@Rewash_Counter", pOperator.Rewash_Counter);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
                        pOperator.HasChanged = false;
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
        public void InsertNewDataRec(PublicOperatorLog pOpLog)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblOperatorLog]
                           ([Remote_RecNum]
                           ,[MachineID]
                           ,[SubID]
                           ,[OperatorID]
                           ,[TimeStamp]
                           ,[State])
                     VALUES
                           (@Remote_RecNum
                           ,@MachineID
                           ,@SubID
                           ,@OperatorID
                           ,@TimeStamp
                           ,@State)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@Remote_RecNum", pOpLog.Remote_RecNum);
                    command.Parameters.AddWithValue("@MachineID", pOpLog.MachineID);
                    command.Parameters.AddWithValue("@SubID", pOpLog.SubID);
                    command.Parameters.AddWithValue("@OperatorID", pOpLog.OperatorID);
                    command.Parameters.AddWithValue("@TimeStamp", pOpLog.TimeStamp);
                    command.Parameters.AddWithValue("@State", pOpLog.State);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
                        pOpLog.HasChanged = false;
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
        public void UpdateOperatorDetails(PublicOperator pOperator)
        {
            const string commandString =
                @"UPDATE [JEGR_Public].[dbo].[tblOperatorProcess]
                   SET [DateHour] = @DateHour
                      ,[MachineID] = @MachineID
                      ,[Machine_ExtRef] = @Machine_ExtRef
                      ,[Machine_LongDescription] = @Machine_LongDescription
                      ,[Machine_NormValue] = @Machine_NormValue
                      ,[Machine_Positions] = @Machine_Positions
                      ,[MachineGroup_ExtRef] = @MachineGroup_ExtRef
                      ,[MachineArea] = @MachineArea
                      ,[MachineGroup_LongDescription] = @MachineGroup_LongDescription
                      ,[SubID] = @SubID
                      ,[OperatorID] = @OperatorID
                      ,[Operator_ExtRef] = @Operator_ExtRef
                      ,[Operator_LongDescription] = @Operator_LongDescription
                      ,[Pieces_Counter] = @Pieces_Counter
                      ,[Weight_Counter] = @Weight_Counter
                      ,[Production_Time] = @Production_Time
                      ,[Stop_Time] = @Stop_Time
                      ,[NoFlow_Time] = @NoFlow_Time
                      ,[Fault_Time] = @Fault_Time
                      ,[Reject_Counter] = @Reject_Counter
                      ,[Rewash_Counter] = @Rewash_Counter
                    WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", pOperator.DateHour);
                    command.Parameters.AddWithValue("@MachineID", pOperator.MachineID);
                    command.Parameters.AddWithValue("@Machine_ExtRef", pOperator.Machine_ExtRef);
                    command.Parameters.AddWithValue("@Machine_LongDescription", pOperator.Machine_LongDescription);
                    command.Parameters.AddWithValue("@Machine_NormValue", pOperator.Machine_NormValue);
                    command.Parameters.AddWithValue("@Machine_Positions", pOperator.Machine_Positions);
                    command.Parameters.AddWithValue("@MachineGroup_ExtRef", pOperator.MachineGroup_ExtRef);
                    command.Parameters.AddWithValue("@MachineArea", pOperator.MachineArea);
                    command.Parameters.AddWithValue("@MachineGroup_LongDescription", pOperator.MachineGroup_LongDescription);
                    command.Parameters.AddWithValue("@SubID", pOperator.SubID);
                    command.Parameters.AddWithValue("@OperatorID", pOperator.OperatorID);
                    command.Parameters.AddWithValue("@Operator_ExtRef", pOperator.Operator_ExtRef);
                    command.Parameters.AddWithValue("@Operator_LongDescription", pOperator.Operator_LongDescription);
                    command.Parameters.AddWithValue("@Pieces_Counter", pOperator.Pieces_Counter);
                    command.Parameters.AddWithValue("@Weight_Counter", pOperator.Weight_Counter);
                    command.Parameters.AddWithValue("@Production_Time", pOperator.Production_Time);
                    command.Parameters.AddWithValue("@Stop_Time", pOperator.Stop_Time);
                    command.Parameters.AddWithValue("@NoFlow_Time", pOperator.NoFlow_Time);
                    command.Parameters.AddWithValue("@Fault_Time", pOperator.Fault_Time);
                    command.Parameters.AddWithValue("@Reject_Counter", pOperator.Reject_Counter);
                    command.Parameters.AddWithValue("@Rewash_Counter", pOperator.Rewash_Counter);
                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
                        pOperator.HasChanged = false;
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

        #region Delete Data
  
        public void DeletePublicOperatorDetails(PublicOperator pOperator)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblOperatorProcess]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", pOperator.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
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

        public void DeleteOperatorDateHour(DateTime dateHour)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblOperatorProcess]
                    WHERE DateHour > DATEADD(MINUTE,-1,@datehour)
                    and DateHour<DATEADD(MINUTE,1,@datehour)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", dateHour);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
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

        public void DeleteOperatorLogDateHour(DateTime dateHour)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblOperatorLog]
                    WHERE TimeStamp >= @datehour
                    and TimeStamp<DATEADD(Hour,1,@datehour)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", dateHour);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
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
    }

    #region Data Collections
    public class OperatorProcess : List<PublicOperator>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int DateHourPos = dr.GetOrdinal("DateHour");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int OperatorIDPos = dr.GetOrdinal("OperatorID");
            int Pieces_CounterPos = dr.GetOrdinal("Pieces_Counter");
            int Weight_CounterPos = dr.GetOrdinal("Weight_Counter");
            int Production_TimePos = dr.GetOrdinal("Production_Time");
            int Stop_TimePos = dr.GetOrdinal("Stop_Time");
            int Fault_TimePos = dr.GetOrdinal("Fault_Time");
            int Reject_CounterPos = dr.GetOrdinal("Reject_Counter");
            int Rewash_CounterPos = dr.GetOrdinal("Rewash_Counter");

            SqlDataAccess da = SqlDataAccess.Singleton;

            while (dr.Read())
            {
                PublicOperator dailyOperator = new PublicOperator();
                dailyOperator.RecNum = dr.GetInt32(RecNumPos);
                dailyOperator.DateHour = dr.GetDateTime(DateHourPos);
                if (da.CombinedProduction)
                    dailyOperator.MachineID = dr.GetInt32(MachineIDPos);
                else
                    dailyOperator.MachineID = dr.GetInt16(MachineIDPos);
                dailyOperator.SubID = dr.GetInt32(SubIDPos);
                dailyOperator.OperatorID = dr.GetInt32(OperatorIDPos);
                dailyOperator.Pieces_Counter = dr.GetInt32(Pieces_CounterPos);
                dailyOperator.Weight_Counter = dr.GetDecimal(Weight_CounterPos);
                dailyOperator.Production_Time = dr.GetInt32(Production_TimePos);
                dailyOperator.Stop_Time = dr.GetInt32(Stop_TimePos);
                dailyOperator.Fault_Time = dr.GetInt32(Fault_TimePos);
                dailyOperator.Reject_Counter = dr.GetInt32(Reject_CounterPos);
                dailyOperator.Rewash_Counter = dr.GetInt32(Rewash_CounterPos);
                dailyOperator.HasChanged = false;
                this.Add(dailyOperator);
            }

            return this.Count;
        }

        public PublicOperator GetByIds(int MachineID, int Subid, int OpID)
        {
            return this.Find(delegate(PublicOperator opRec)
            {
                return ((opRec.MachineID == MachineID)
                    && (opRec.SubID == Subid)
                    && (opRec.OperatorID == OpID));
            });
        }

    }

    public class OperatorProcessLogs : List<PublicOperatorLog>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int Remote_RecNumPos = dr.GetOrdinal("Remote_RecNum");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int OperatorIDPos = dr.GetOrdinal("OperatorID");
            int TimeStampPos = dr.GetOrdinal("TimeStamp");
            int StatePos = dr.GetOrdinal("State");

            while (dr.Read())
            {
                PublicOperatorLog dailyOperatorLog = new PublicOperatorLog()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    Remote_RecNum = dr.GetInt32(Remote_RecNumPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    SubID = dr.GetInt32(SubIDPos),
                    OperatorID = dr.GetInt32(OperatorIDPos),
                    TimeStamp = dr.GetDateTime(TimeStampPos),
                    State = dr.GetInt32(StatePos),
                    HasChanged = false
                };
                this.Add(dailyOperatorLog);
            }

            return this.Count;
        }

    }
    
    #endregion

    #region Item Classes

    public class PublicOperatorLog : DataItem
    {
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int Remote_RecNum;
            internal int MachineID;
            internal int SubID;
            internal int OperatorID;
            internal DateTime TimeStamp;
            internal int State;
            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #region Constructor
        public PublicOperatorLog()
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

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return this.activeData.RecNum == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.RecNum != -1;
            }
        }
        #endregion

        public int RecNum
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                HasChanged |= this.activeData.RecNum != value;
                this.activeData.RecNum = value;
            }
        }

        public int Remote_RecNum
        {
            get
            {
                return this.activeData.Remote_RecNum;
            }
            set
            {
                HasChanged |= this.activeData.Remote_RecNum != value;
                this.activeData.Remote_RecNum = value;
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.MachineID;
            }
            set
            {
                HasChanged |= this.activeData.MachineID != value;
                this.activeData.MachineID = value;
            }
        }

        public int SubID
        {
            get
            {
                return this.activeData.SubID;
            }
            set
            {
                HasChanged |= this.activeData.SubID != value;
                this.activeData.SubID = value;
            }
        }

        public int OperatorID
        {
            get
            {
                return this.activeData.OperatorID;
            }
            set
            {
                HasChanged |= this.activeData.OperatorID != value;
                this.activeData.OperatorID = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return this.activeData.TimeStamp;
            }
            set
            {
                HasChanged |= this.activeData.TimeStamp != value;
                this.activeData.TimeStamp = value;
            }
        }

        public int State
        {
            get
            {
                return this.activeData.State;
            }
            set
            {
                HasChanged |= this.activeData.State != value;
                this.activeData.State = value;
            }
        }

    }


    public class PublicOperator : DataItem
    {
        private bool shouldStoreData = false;
        #region DailyOperator Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal DateTime DateHour;
            internal int MachineID;
            internal string Machine_ExtRef;
            internal string Machine_LongDescription;
            internal int Machine_NormValue;
            internal int Machine_Positions;
            internal string MachineGroup_ExtRef;
            internal string MachineArea;
            internal string MachineGroup_LongDescription;
            internal int SubID;
            internal int OperatorID;
            internal string Operator_ExtRef;
            internal string Operator_LongDescription;
            internal int Pieces_Counter;
            internal decimal Weight_Counter;
            internal int Production_Time;
            internal int Stop_Time;
            internal int NoFlow_Time;
            internal int Fault_Time;
            internal int Reject_Counter;
            internal int Rewash_Counter;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public PublicOperator()
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

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return this.activeData.RecNum == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.RecNum != -1;
            }
        }
        #endregion


        public int RecNum
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                this.activeData.RecNum = value;
                HasChanged = true;
            }
        }
        public DateTime DateHour
        {
            get
            {
                return this.activeData.DateHour;
            }
            set
            {
                this.activeData.DateHour = value;
                HasChanged = true;
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.MachineID;
            }
            set
            {
                this.activeData.MachineID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines machines = da.GetAllMachines();
                Machine m = machines.GetById(this.activeData.MachineID);
                if (m != null)
                {
                    MachineGroups machineGroups = da.GetAllMachineGroups();
                    if (m.ExtRef != string.Empty)
                        this.activeData.Machine_ExtRef = m.ExtRef;
                    else
                        this.activeData.Machine_ExtRef = value.ToString();
                    this.activeData.Machine_LongDescription = m.LongDescription;
                    this.activeData.Machine_NormValue = m.NormValue;
                    this.activeData.Machine_Positions = m.Positions;
                    shouldStoreData = m.OperatorCountExitPoint;
                    MachineGroup mg = machineGroups.GetById(m.MachineGroup_idJensen);
                    if (mg != null)
                    {
                        this.activeData.MachineArea = mg.MachineArea;
                        if (mg.ExtRef != string.Empty)
                            this.activeData.MachineGroup_ExtRef = mg.ExtRef;
                        else
                            this.activeData.MachineGroup_ExtRef = mg.idJensen.ToString();
                        this.activeData.MachineGroup_LongDescription = mg.LongDescription;
                    }
                    else
                    {
                        this.activeData.MachineArea = "";
                        this.activeData.MachineGroup_ExtRef = "";
                        this.activeData.MachineGroup_LongDescription = "";
                    }
                }
                else
                {
                    this.activeData.Machine_ExtRef = "";
                    this.activeData.Machine_LongDescription = "";
                    this.activeData.Machine_NormValue = 0;
                    this.activeData.Machine_Positions = 0;
                    this.activeData.MachineArea = "";
                    this.activeData.MachineGroup_ExtRef = "";
                    this.activeData.MachineGroup_LongDescription = "";
                }
                HasChanged = true;
            }
        }

        public bool ShouldStoreData
        {
            get
            {
                return (shouldStoreData && HasChanged);
            }
        }

        public string Machine_ExtRef
        {
            get { return this.activeData.Machine_ExtRef; }
        }

        public string Machine_LongDescription
        {
            get { return this.activeData.Machine_LongDescription; }
        }

        public int Machine_NormValue
        {
            get { return this.activeData.Machine_NormValue; }
        }

        public int Machine_Positions
        {
            get { return this.activeData.Machine_Positions; }
        }

        public string MachineArea
        {
            get { return this.activeData.MachineArea; }
        }

        public string MachineGroup_ExtRef
        {
            get { return this.activeData.MachineGroup_ExtRef; }
        }

        public string MachineGroup_LongDescription
        {
            get { return this.activeData.MachineGroup_LongDescription; }
        }

        public int SubID
        {
            get
            {
                return this.activeData.SubID;
            }
            set
            {
                this.activeData.SubID = value;
                HasChanged = true;
            }
        }

        public int OperatorID
        {
            get
            {
                return this.activeData.OperatorID;
            }
            set
            {
                this.activeData.OperatorID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Operators operators = da.GetAllActiveOperators();
                Operator o = operators.GetById(this.activeData.OperatorID);
                if (o != null)
                {
                    if (o.ExtRef != string.Empty)
                        this.activeData.Operator_ExtRef = o.ExtRef;
                    else
                        this.activeData.Operator_ExtRef = value.ToString();
                    this.activeData.Operator_LongDescription = o.LongDescription;
                }
                else
                {
                    this.activeData.Operator_ExtRef = "";
                    this.activeData.Operator_LongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string Operator_ExtRef
        {
            get { return this.activeData.Operator_ExtRef; }
        }

        public string Operator_LongDescription
        {
            get { return this.activeData.Operator_LongDescription; }
        }

        public int Pieces_Counter
        {
            get
            {
                return this.activeData.Pieces_Counter;
            }
            set
            {
                HasChanged |= (this.activeData.Pieces_Counter != value);
                this.activeData.Pieces_Counter = value;
            }
        }

        public decimal Weight_Counter
        {
            get
            {
                return this.activeData.Weight_Counter;
            }
            set
            {
                HasChanged |= (this.activeData.Weight_Counter != value);
                this.activeData.Weight_Counter = value;
            }
        }

        public int Production_Time
        {
            get
            {
                return this.activeData.Production_Time;
            }
            set
            {
                HasChanged |= (this.activeData.Production_Time != value);
                this.activeData.Production_Time = value;
            }
        }

        public int Stop_Time
        {
            get
            {
                return this.activeData.Stop_Time;
            }
            set
            {
                HasChanged |= (this.activeData.Stop_Time != value);
                this.activeData.Stop_Time = value;
            }
        }

        public int NoFlow_Time
        {
            get
            {
                return this.activeData.NoFlow_Time;
            }
            set
            {
                if (this.activeData.NoFlow_Time != value)
                {
                    HasChanged |= (this.activeData.NoFlow_Time != value);
                    this.activeData.NoFlow_Time = value;
                }
            }
        }

        public int Fault_Time
        {
            get
            {
                return this.activeData.Fault_Time;
            }
            set
            {
                HasChanged |= (this.activeData.Fault_Time != value);
                this.activeData.Fault_Time = value;
            }
        }

 
        public int Reject_Counter
        {
            get
            {
                return this.activeData.Reject_Counter;
            }
            set
            {
                HasChanged |= (this.activeData.Reject_Counter != value);
                this.activeData.Reject_Counter = value;
            }
        }

        public int Rewash_Counter
        {
            get
            {
                return this.activeData.Rewash_Counter;
            }
            set
            {
                HasChanged |= (this.activeData.Rewash_Counter != value);
                this.activeData.Rewash_Counter = value;
            }
        }
 

    }
    #endregion
}
