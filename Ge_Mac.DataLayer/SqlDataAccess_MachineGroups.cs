using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{

    #region MachineArea Enum
    public enum MachineArea
    {
        WT,
        FT,
        GT
    }
    #endregion

    public partial class SqlDataAccess
    {

        #region Select Data
        private MachineGroups machineGroupsCache = null;

        public void InvalidateMachineGroups()
        {
            if (machineGroupsCache != null)
                machineGroupsCache.IsValid = false;
        }

        private bool MachineGroupsAreCached()
        {
            bool test = (machineGroupsCache != null);
            if (test)
            {
                test = machineGroupsCache.IsValid;
            }
            return test;
        }

        const string AllMachineGroupsCommand =
            @"SELECT RecNum
                   , idJensen
                   , ExtRef
                   , MachineArea
                   , ShortDescription
                   , LongDescription
                   , DisplayInMaster
                   , UnitsKPI
                   , HourlyKPI
                   , DailyKPI
                   , GraphKPI
                   , StartTime
                   , EndTime
              FROM [dbo].[tblMachineGroups]";

        public MachineGroups GetAllMachineGroups()
        {
            if (MachineGroupsAreCached())
            {
                return machineGroupsCache;
            }
            try
            {
                const string commandString = AllMachineGroupsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (machineGroupsCache == null) machineGroupsCache = new MachineGroups();
                    command.DataFill(machineGroupsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return machineGroupsCache;
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

        public MachineGroups GetKPIMachineGroups(MachineArea machineArea)
        {
            MachineGroups mgs = GetAllMachineGroups();
            MachineGroups kpiMgs = new MachineGroups();
            foreach (MachineGroup mg in mgs)
            {
                if ((mg.GraphKPI) && (mg.MachineArea==machineArea.ToString()))
                    kpiMgs.Add(mg);
            }
            return kpiMgs;
        }


        public MachineGroup GetMachineGroup(int idJensen)
        {
            MachineGroups machineGroups = GetAllMachineGroups();
            MachineGroup machineGroup = machineGroups.GetById(idJensen);
            return machineGroup;
        }

        public MachineGroup GetMachineGroupKPI(int idJensen)
        {
            MachineGroups machineGroups = GetAllMachineGroups();
            MachineGroup machineGroup = machineGroups.GetById(idJensen);
            return machineGroup;
        }

        public List<int> GetMachineAreaGroups(MachineArea machineArea)
        {
            const String strSQL =
                    @"SELECT idJensen
                      FROM dbo.tblMachineGroups
                      WHERE MachineArea = @MachineArea
                      AND   GraphKPI = 1";

            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@MachineArea", machineArea.ToString());
                    command.Connection = SqlDataConnection.GetConnection(SqlDataConnection.DBConnection.JensenGroup);
                    command.CommandTimeout = SqlDataConnection.Timeout;

                    // Open the connection if necessary
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        command.Connection.Open();
                    }

                    List<int> groupIds = new List<int>();

                    using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        const int GroupIdColumnId = 0;

                        while (dr.Read())
                        {
                            groupIds.Add(dr.GetInt16(GroupIdColumnId));
                        }
                    }

                    command.Connection.Close();

                    return groupIds;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                return null;
            }
        }
        #endregion

        #region Next Record

        public short NextMachineGroupRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblMachineGroups";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblMachineGroups',
		                                            @idName = N'idJensen'
                                         SELECT @return_value";

            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    if (spResult != null)
                    {
                        if (spResult.ToString() != string.Empty)
                        {
                            nextID = (int)spResult;
                            nextID++;
                        }
                    }
                }
            }

            catch (SqlException)
            {
                throw;
            }
            return (short)nextID;
        }

        #endregion

        #region Insert Data
        public void InsertNewMachineGroup(MachineGroup machineGroup)
        {
            const string commandString =
                @"INSERT INTO [dbo].tblMachineGroups
                  ( idJensen
                  , ExtRef
                  , MachineArea
                  , ShortDescription
                  , LongDescription
                  , DisplayInMaster
                  , UnitsKPI
                  , HourlyKPI
                  , DailyKPI
                  , GraphKPI
                  , StartTime
                  , EndTime
                  )
                  VALUES
                  ( @idJensen
                  , @ExtRef
                  , @MachineArea
                  , @ShortDescription
                  , @LongDescription
                  , @DisplayInMaster
                  , @UnitsKPI
                  , @HourlyKPI
                  , @DailyKPI
                  , @GraphKPI
                  , @StartTime
                  , @EndTime
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", machineGroup.idJensen);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = machineGroup.ExtRef == null ? DBNull.Value : (object)machineGroup.ExtRef;

                    command.Parameters.AddWithValue("@MachineArea", machineGroup.MachineArea);
                    command.Parameters.AddWithValue("@ShortDescription", machineGroup.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", machineGroup.LongDescription);
                    command.Parameters.AddWithValue("@DisplayInMaster", machineGroup.DisplayInMaster);
                    command.Parameters.AddWithValue("@UnitsKPI", machineGroup.UnitsKPI);
                    command.Parameters.AddWithValue("@HourlyKPI", machineGroup.HourlyKPI);
                    command.Parameters.AddWithValue("@DailyKPI", machineGroup.DailyKPI);
                    command.Parameters.AddWithValue("@GraphKPI", machineGroup.GraphKPI);
                    command.Parameters.AddWithValue("@StartTime", machineGroup.StartTime);
                    command.Parameters.AddWithValue("@EndTime", machineGroup.EndTime);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            machineGroup.RecNum = (int)RecNum;
                            machineGroup.HasChanged = false;
                        }
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
                InvalidateMachineGroups();
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
        public void UpdateMachineGroupDetails(MachineGroup machineGroup)
        {
            const string commandString =
                @"UPDATE [dbo].tblMachineGroups
                  SET idJensen = @idJensen
                    , ExtRef = @ExtRef
                    , MachineArea = @MachineArea
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , DisplayInMaster = @DisplayInMaster
                    , UnitsKPI = @UnitsKPI
                    , HourlyKPI = @HourlyKPI
                    , DailyKPI = @DailyKPI
                    , GraphKPI = @GraphKPI
                    , StartTime = @StartTime
                    , EndTime = @EndTime

                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", machineGroup.RecNum);
                    command.Parameters.AddWithValue("@idJensen", machineGroup.idJensen);
                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = machineGroup.ExtRef == null ? DBNull.Value : (object)machineGroup.ExtRef;
                    command.Parameters.AddWithValue("@MachineArea", machineGroup.MachineArea);
                    command.Parameters.AddWithValue("@ShortDescription", machineGroup.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", machineGroup.LongDescription);
                    command.Parameters.AddWithValue("@DisplayInMaster", machineGroup.DisplayInMaster);
                    command.Parameters.AddWithValue("@UnitsKPI", machineGroup.UnitsKPI);
                    command.Parameters.AddWithValue("@HourlyKPI", machineGroup.HourlyKPI);
                    command.Parameters.AddWithValue("@DailyKPI", machineGroup.DailyKPI);
                    command.Parameters.AddWithValue("@GraphKPI", machineGroup.GraphKPI);
                    command.Parameters.AddWithValue("@StartTime", machineGroup.StartTime);
                    command.Parameters.AddWithValue("@EndTime", machineGroup.EndTime);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    machineGroup.HasChanged = false;
                }
                InvalidateMachineGroups();
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
        /// <summary>
        /// Delete a machine group permanently from the database.
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteMachineGroupDetails(MachineGroup machineGroup)
        {
            const string commandString =
                @"DELETE FROM [dbo].tblMachineGroups
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", machineGroup.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateMachineGroups();
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

    #region Data Collection Class
    public class MachineGroups : List<MachineGroup>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblMachineGroups";
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

        #region Field Positions
        int RecNumPos;
        int idJensenPos;
        int ExtRefPos;
        int MachineAreaPos;
        int ShortDescriptionPos;
        int LongDescriptionPos;
        int DisplayInMasterPos;
        int UnitsKPIPos;
        int HourlyKPIPos;
        int DailyKPIPos;
        int GraphKPIPos;
        int StartTimePos;
        int EndTimePos;

        public MachineGroup GetById(int id)
        {
            return this.Find(delegate(MachineGroup machineGroup)
            {
                return machineGroup.idJensen == id;
            });
        }

        public MachineGroup GetByName(string aName)
        {
            return this.Find(delegate(MachineGroup machineGroup)
            {
                return machineGroup.ShortDescription == aName;
            });
        }

        public MachineGroup GetByIdKPI(int id)
        {
            return this.Find(delegate(MachineGroup machineGroup)
            {
                return (machineGroup.GraphKPI)
                    && (machineGroup.idJensen == id);
            });
        }


        public void SetFieldPositions(SqlDataReader dr)
        {
            RecNumPos = dr.GetOrdinal("RecNum");
            idJensenPos = dr.GetOrdinal("idJensen");
            ExtRefPos = dr.GetOrdinal("ExtRef");
            MachineAreaPos = dr.GetOrdinal("MachineArea");
            ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            LongDescriptionPos = dr.GetOrdinal("LongDescription");
            DisplayInMasterPos = dr.GetOrdinal("DisplayInMaster");
            UnitsKPIPos = dr.GetOrdinal("UnitsKPI");
            HourlyKPIPos = dr.GetOrdinal("HourlyKPI");
            DailyKPIPos = dr.GetOrdinal("DailyKPI");
            GraphKPIPos = dr.GetOrdinal("GraphKPI");
            StartTimePos = dr.GetOrdinal("StartTime");
            EndTimePos = dr.GetOrdinal("EndTime");
        }
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Machines Collection
                MachineGroup machine = FillMachineGroup(dr);
                this.Add(machine);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false; 
            
            return this.Count;
        }

        public MachineGroup FillMachineGroup(SqlDataReader dr)
        {
            MachineGroup machineGroup = new MachineGroup()
            {
                RecNum = dr.GetInt32(RecNumPos),
                idJensen = dr.GetInt16(idJensenPos),
                ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                MachineArea = dr.GetString(MachineAreaPos),
                ShortDescription = dr.GetString(ShortDescriptionPos),
                LongDescription = dr.GetString(LongDescriptionPos),
                DisplayInMaster = dr.GetBoolean(DisplayInMasterPos),
                UnitsKPI = dr.GetInt32(UnitsKPIPos),
                HourlyKPI = dr.GetInt32(HourlyKPIPos),
                DailyKPI = dr.GetInt32(DailyKPIPos),
                GraphKPI = dr.GetBoolean(GraphKPIPos),
                StartTime = dr.GetInt32(StartTimePos),
                EndTime = dr.GetInt32(EndTimePos)
            };

            return machineGroup;
        }

        public int MinStartTime()
        {
            int min = 24;
            foreach (MachineGroup mg in this)
            {
                if (mg.StartTime < min)
                    min = mg.StartTime;
            }
            return min;
        }

        public int MaxEndTime()
        {
            int max = 0;
            foreach (MachineGroup mg in this)
            {
                if (mg.EndTime > max)
                    max = mg.EndTime;
            }
            return max;
        }

    }
    #endregion

    #region Item Class

    public class MachineGroup : DataItem, IComparable<MachineGroup>
    {
        #region Machine Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal string ExtRef;
            internal string MachineArea;
            internal string ShortDescription;
            internal string LongDescription;
            internal bool DisplayInMaster;
            internal int UnitsKPI;
            internal int HourlyKPI;
            internal int DailyKPI;
            internal bool GraphKPI;
            internal int EndTime;
            internal int StartTime;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public MachineGroup()
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

        #region Data Column Properties

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

        public short idJensen
        {
            get
            {
                return this.activeData.idJensen;
            }
            set
            {
                this.activeData.idJensen = value;
                HasChanged = true;
            }
        }

        public string ExtRef
        {
            get
            {
                return this.activeData.ExtRef;
            }
            set
            {
                this.activeData.ExtRef = value;
                HasChanged = true;
            }
        }

        public string MachineArea
        {
            get
            {
                return this.activeData.MachineArea;
            }
            set
            {
                this.activeData.MachineArea = value;
                HasChanged = true;
            }
        }

        public string MachineAreaName
        {
            get
            {
                string ma = MachineArea;
                if (ma == "WT")
                    ma = "1. WT";
                if (ma == "FT")
                    ma = "2. FT";
                if (ma == "GT")
                    ma = "3. GT";
                return ma;
            }
        }

        public string ShortDescription
        {
            get
            {
                return this.activeData.ShortDescription;
            }
            set
            {
                this.activeData.ShortDescription = value;
                HasChanged = true;
            }
        }

        public string LongDescription
        {
            get
            {
                return this.activeData.LongDescription;
            }
            set
            {
                this.activeData.LongDescription = value;
                HasChanged = true;
            }
        }

        public bool DisplayInMaster
        {
            get
            {
                return this.activeData.DisplayInMaster;
            }
            set
            {
                this.activeData.DisplayInMaster = value;
                HasChanged = true;
            }
        }

        public int UnitsKPI
        {
            get
            {
                return this.activeData.UnitsKPI;
            }
            set
            {
                this.activeData.UnitsKPI = value;
                HasChanged = true;
            }
        }

        public int HourlyKPI
        {
            get
            {
                return this.activeData.HourlyKPI;
            }
            set
            {
                this.activeData.HourlyKPI = value;
                HasChanged = true;
            }
        }

        public int MachineSubsHourlyKPI
        {
            get
            {
                double kpi = 0;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines machines = da.GetAllMachines();
                foreach (Machine m in machines)
                {
                    if (m.MachineGroup_idJensen == this.idJensen)
                    {
                        kpi += (m.NormValue / m.Positions);
                    }
                }
                return (int)kpi;
            }
        }

        public int MachineGroupSubIDs
        {
            get
            {
                int subids = 0;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines machines = da.GetAllMachines();
                foreach (Machine m in machines)
                {
                    if ((m.MachineGroup_idJensen == this.idJensen) && (m.OperatorCountExitPoint))
                    {
                        subids += m.Positions;
                    }
                }
                if (subids == 0) subids = 1;
                return subids;
            }
        }

        public int GroupOperatorNorm
        {
            get
            {
                int subids = 0;
                int norms = 0;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines machines = da.GetAllMachines();
                foreach (Machine m in machines)
                {
                    if ((m.MachineGroup_idJensen == this.idJensen) && (m.OperatorProductionCountExitPoint))
                    {
                        subids += m.Positions;
                        norms += m.NormValue;
                    }
                }
                if (subids == 0) subids = 1;
                return norms / subids;
            }
        }


        public int MachinesHourlyKPI
        {
            get
            {
                int kpi = 0;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines machines = da.GetAllMachines();
                foreach (Machine m in machines)
                {
                    if (m.MachineGroup_idJensen == this.idJensen)
                    {
                        kpi += m.NormValue;
                    }
                }
                return (int)kpi;
            }
        }

        public int DailyKPI
        {
            get
            {
                return this.activeData.DailyKPI;
            }
            set
            {
                this.activeData.DailyKPI = value;
                HasChanged = true;
            }
        }

        public bool GraphKPI
        {
            get
            {
                return this.activeData.GraphKPI;
            }
            set
            {
                this.activeData.GraphKPI = value;
                HasChanged = true;
            }
        }

        public int EndTime
        {
            get
            {
                return this.activeData.EndTime;
            }
            set
            {
                this.activeData.EndTime = value;
                HasChanged = true;
            }
        }

        public int StartTime
        {
            get
            {
                return this.activeData.StartTime;
            }
            set
            {
                this.activeData.StartTime = value;
                HasChanged = true;
            }
        }

        #endregion

        #region Sorting
        public static Comparison<MachineGroup> NameComparison =
            delegate(MachineGroup mg1, MachineGroup mg2)
            {
                return mg1.ShortDescription.CompareTo(mg2.ShortDescription);
            };

        public static Comparison<MachineGroup> IDComparison =
            delegate(MachineGroup mg1, MachineGroup mg2)
            {
                return mg1.idJensen.CompareTo(mg2.idJensen);
            };

        public int CompareTo(MachineGroup other) { return ShortDescription.CompareTo(other.ShortDescription); }
        #endregion
    }

    #endregion
}
