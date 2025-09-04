using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Cached Data

        private Machines machinesCache = null;
        private MachineSubIDs machineSubIDsCache = null;
        private MachineLogAliases machineAliasesCache = null;

        public void InvalidateMachines()
        {
            if (machinesCache != null)
                machinesCache.IsValid = false;
        }

        private bool MachinesAreCached()
        {
            bool test = (machinesCache != null);
            if (test)
            {
                test = machinesCache.IsValid;
            }
            return test;
        }

        public void InvalidateMachineAliases()
        {
            if (machineAliasesCache != null)
                machineAliasesCache.IsValid = false;
        }

        private bool MachineAliasesAreCached()
        {
            bool test = (machineAliasesCache != null);
            if (test)
            {
                test = machineAliasesCache.IsValid;
            }
            return test;
        }

        public void InvalidateMachineSubIDs()
        {
            if (machineSubIDsCache != null)
                machineSubIDsCache.IsValid = false;
        }

        public bool MachineSubIDsAreCached()
        {
            bool test = (machineSubIDsCache != null);
            if (test)
            {
                test = machineSubIDsCache.IsValid;
            }
            return test;
        }        
        #endregion
        
        #region Select Data

        const string AllMachinesCommand =
            @"SELECT m.*
                      , ms.[RecNum] AS RecNumMS
                      , [Machine_idJensen]
                      , [Status]
                      , [PieceCount]
                      , [OnTime]
                      , [RunTime]
                      , [UpdateTime]
                      , [LastMessage]
              FROM [dbo].[tblMachines] m, [dbo].[tblMachineStatus] ms
                      WHERE m.idJensen = ms.Machine_idJensen
              ORDER BY m.idJensen ";

        const string AllMachinesCommand199 =
    @"SELECT m.*
                      , ms.[RecNum] AS RecNumMS
                      , [Machine_idJensen]
                      , [Status]
                      , [PieceCount]
                      , [OnTime]
                      , [RunTime]
                      , [UpdateTime]
                      , [LastMessage]
                      , [LastSuccessfulPing]
              FROM [dbo].[tblMachines] m, [dbo].[tblMachineStatus] ms
                      WHERE m.idJensen = ms.Machine_idJensen
              ORDER BY m.idJensen ";

//        const string AllMachineStatusCommand =
//            @"SELECT [RecNum]
//                   , [Machine_idJensen]
//                   , [Status]
//                   , [PieceCount]
//                   , [OnTime]
//                   , [RunTime]
//                   , [UpdateTime]
//                   , [LastMessage]
//              FROM [dbo].[tblMachineStatus]";
        const string AllMachineStatusCommand = @"SELECT * FROM [dbo].[tblMachineStatus]";


        public Machines GetAllMachines()
        {
            return GetAllMachines(false);
        }

        public Machines GetMachineCache()
        {
            return machinesCache;
        }

        public Machines GetAllMachines(bool noCache, bool preserveCache)
        {
            Machines ms=GetAllMachines(noCache);
            if (!preserveCache)
            {
                machinesCache = ms;
            }
            return ms;
        }

        public Machines GetAllMachines(bool noCache)
        {
            if (MachinesAreCached() && !noCache)
            {
                return machinesCache;
            }
            try
            {
                string commandString;
                if (DatabaseVersion >= 1.99)
                {
                    commandString = AllMachinesCommand199;
                }
                else
                {
                    commandString = AllMachinesCommand;
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Machines ms;
                    if (noCache || (machinesCache == null))
                    {
                        ms = new Machines();
                    }
                    else
                    {
                        ms = machinesCache;
                    }
                    command.DataFill(ms, SqlDataConnection.DBConnection.JensenGroup);
                    if (!noCache)
                    {
                        machinesCache = ms;
                    }
                    return ms;
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

        public Machine GetMachine(int idJensen)
        {
            Machines machines = GetAllMachines();
            Machine machine = machines.GetById(idJensen);
            return machine;
        }

        public void GetUpdatedMachinesStatus(Machines machines) //remove after polling!
        {
            try
            {
                const string commandString =
                    AllMachineStatusCommand +
                    @" WHERE Machine_idJensen = @Machine_idJensen
                       AND   UpdateTime > @UpdateTime";

                foreach (Machine machine in machines)
                {
                    if (machine.MachineStatus != null)
                    {
                        using (SqlCommand command = new SqlCommand(commandString))
                        {
                            command.Parameters.AddWithValue("@Machine_idJensen", machine.idJensen);
                            command.Parameters.AddWithValue("@UpdateTime", machine.MachineStatus.UpdateTime);

                            MachineStatus status = MachineStatus.First(command, SqlDataConnection.DBConnection.JensenGroup);

                            if (status != null)
                            {
                                machine.MachineStatus = status;
                            }
                        }
                    }
                }
                InvalidateMachines();
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

        public MachineSubIDs GetAllMachineSubIDs()
        {
            return GetAllMachineSubIDs(false, -1);        
        }

        public MachineSubIDs GetAllMachineSubIDs(bool noCache)
        {
            return GetAllMachineSubIDs(noCache, -1);
        }

        public MachineSubIDs GetAllMachineSubIDs(int machineID)
        {
            return GetAllMachineSubIDs(true, machineID);
        }

        public MachineSubIDs GetAllMachineSubIDs(bool noCache, int machineID)
        {
            if (MachineSubIDsAreCached() && !noCache)
            {
                return machineSubIDsCache;
            }
            try
            {
                string commandString =
                   @"SELECT [RecNum]
                      ,[Machine_idJensen]
                      ,[SubID]
                      ,[Operator_idJensen]
                      ,[Customer_idJensen]
                      ,[Article_idJensen]
                    ";
                if (this.DatabaseVersion >= 1.7)
                    commandString += @",[Operator_Remote]
                        ";
                commandString += @"FROM [JEGR_DB].[dbo].[tblMachineSubID]
                    WHERE Machine_idJensen >0
                    AND SubID >0";
                if (machineID > 0)
                {
                    commandString += " AND Machine_idJensen = @Machine_idJensen ";
                }
                commandString += " ORDER BY Machine_idJensen, SubID";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (machineID > 0)
                        command.Parameters.AddWithValue("@Machine_idJensen", machineID);
                    if (machineSubIDsCache == null)
                        machineSubIDsCache = new MachineSubIDs();
                    command.DataFill(machineSubIDsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return machineSubIDsCache;
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

        public MachineLogAliases GetAllMachineAliases(bool noCache)
        {
            if (!noCache && MachineAliasesAreCached())
            {
                return machineAliasesCache;
            }
            try
            {
                const string commandString = @"SELECT [AliasID]
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
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (machineAliasesCache == null) machineAliasesCache = new MachineLogAliases();
                    command.DataFill(machineAliasesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return machineAliasesCache;
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

        #region Next Record

        public int NextMachineRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblMachines";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblMachines',
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
            return nextID;
        }

        public int NextMachineAliasRecord()
        {
            return NextRecord("tblMachineAlias", "AliasID");
        }

        #endregion

        #region Insert Data

        public void InsertNewMachineSubID(MachineSubID machineSubID)
        {
            const string commandString = @"INSERT INTO [dbo].[tblMachineSubID]
                                                           ([Machine_idJensen]
                                                           ,[SubID]
                                                           ,[Operator_idJensen]
                                                           ,[Customer_idJensen]
                                                           ,[Article_idJensen]
                                                           ,[Operator_Remote])
                                                     VALUES
                                                           (@Machine_idJensen
                                                           ,@SubID
                                                           ,@Operator_idJensen
                                                           ,@Customer_idJensen
                                                           ,@Article_idJensen
                                                           ,@Operator_Remote)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@Machine_idJensen", machineSubID.Machine_idJensen);
                    command.Parameters.AddWithValue("@SubID", machineSubID.SubID);
                    command.Parameters.AddWithValue("@Operator_idJensen", machineSubID.Operator_idJensen);
                    command.Parameters.AddWithValue("@Customer_idJensen", machineSubID.Customer_idJensen);
                    command.Parameters.AddWithValue("@Article_idJensen", machineSubID.Article_idJensen);
                    command.Parameters.AddWithValue("@Operator_Remote", machineSubID.Operator_Remote);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        machineSubID.HasChanged = false;
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;
                        Debug.WriteLine(ex.Message);

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
                    Debug.WriteLine(ex.Message);
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                throw;
            }
 
        }

        public void InsertNewMachine(Machine machine)
        {
            const string commandString12 =
            @"DECLARE	@return_value int

            EXEC	@return_value = [dbo].[spInsertMachines]
	                @idJensen ,
	                @EXTREF,
	                @SHORTDESCRIPTION ,
	                @LONGDESCRIPTION,
	                @IP_ADDRESS,
	                @FTP_PATH_CUSTART,
	                @FTP_PATH_OPERATORS,
	                @ALLOWCUSTOMERS,
	                @SENDCONFIGURATION,
	                @NORMVALUE,
	                @NORMUNIT_IDJENSEN,
	                @MACHINEGROUP_IDJENSEN,
	                @PINGSTATUS,
	                @POSITIONS,
                    @ProcessCodeType,
                    @CustArtUpdate,
                    @OperatorUpdate,
                    @BatchSizeFactor,
                    @ShowBatchText,
                    @UseMachineCount,
                    @MachineCountExitPoint,
                    @CommandLine, 
                    @CommandDescription, 
                    @BatchUpper, 
                    @BatchLower, 
	                @NOFLOW

            SELECT	'Return Value' = @return_value";

            const string commandString14 =
                @"DECLARE	@return_value int

                EXEC	@return_value = [dbo].[spInsertMachines]
	                    @idJensen ,
	                    @EXTREF,
	                    @SHORTDESCRIPTION ,
	                    @LONGDESCRIPTION,
	                    @IP_ADDRESS,
	                    @FTP_PATH_CUSTART,
	                    @FTP_PATH_OPERATORS,
	                    @ALLOWCUSTOMERS,
	                    @SENDCONFIGURATION,
	                    @NORMVALUE,
	                    @NORMUNIT_IDJENSEN,
	                    @MACHINEGROUP_IDJENSEN,
	                    @PINGSTATUS,
	                    @POSITIONS,
                        @ProcessCodeType,
                        @CustArtUpdate,
                        @OperatorUpdate,
                        @BatchSizeFactor,
                        @ShowBatchText,
                        @UseMachineCount,
                        @MachineCountExitPoint,
                        @OperatorCountExitPoint,
                        @ExcludeFromPowerCalc,
                        @ProductionReference,
                        @CommandLine, 
                        @CommandDescription, 
                        @BatchUpper, 
                        @BatchLower, 
	                    @NOFLOW

                SELECT	'Return Value' = @return_value";

            const string commandString197 =
                @"DECLARE	@return_value int

                EXEC	@return_value = [dbo].[spInsertMachines]
	                    @idJensen ,
	                    @EXTREF,
	                    @SHORTDESCRIPTION ,
	                    @LONGDESCRIPTION,
	                    @IP_ADDRESS,
	                    @FTP_PATH_CUSTART,
	                    @FTP_PATH_OPERATORS,
	                    @ALLOWCUSTOMERS,
	                    @SENDCONFIGURATION,
	                    @NORMVALUE,
	                    @NORMUNIT_IDJENSEN,
	                    @MACHINEGROUP_IDJENSEN,
	                    @PINGSTATUS,
	                    @POSITIONS,
                        @ProcessCodeType,
                        @CustArtUpdate,
                        @OperatorUpdate,
                        @BatchSizeFactor,
                        @ShowBatchText,
                        @UseMachineCount,
                        @MachineCountExitPoint,
                        @OperatorCountExitPoint,
                        @ExcludeFromPowerCalc,
                        @ProductionReference,
                        @CommandLine, 
                        @CommandDescription, 
                        @BatchUpper, 
                        @BatchLower, 
	                    @NOFLOW,
                        @ProductionGroupID,
                        @Settings

                SELECT	'Return Value' = @return_value";

            try
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                string commandString = commandString12;
                if ((da.DatabaseVersion >= 1.4) && (da.DatabaseVersion < 1.97))
                    commandString = commandString14;

                if (da.DatabaseVersion >= 1.97)
                    commandString = commandString197;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", machine.idJensen);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = machine.ExtRef == null ? DBNull.Value : (object)machine.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", machine.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", machine.LongDescription);
                    command.Parameters.AddWithValue("@IP_Address", machine.IP_Address);

                    p = command.Parameters.Add("@FTP_Path_CustArt", SqlDbType.VarChar);
                    p.IsNullable = true;
                    p.Value = machine.FTP_Path_CustArt == null ? DBNull.Value : (object)machine.FTP_Path_CustArt;

                    p = command.Parameters.Add("@FTP_Path_Operators", SqlDbType.VarChar);
                    p.IsNullable = true;
                    p.Value = machine.FTP_Path_Operators == null ? DBNull.Value : (object)machine.FTP_Path_Operators;

                    command.Parameters.AddWithValue("@AllowCustomers", machine.AllowCustomers);
                    command.Parameters.AddWithValue("@SendConfiguration", machine.SendConfiguration);
                    command.Parameters.AddWithValue("@NormValue", machine.NormValue);
                    command.Parameters.AddWithValue("@NormUnit_idJensen", machine.NormUnit_idJensen);
                    command.Parameters.AddWithValue("@NoFlow", machine.NoFlow);
                    command.Parameters.AddWithValue("@MachineGroup_idJensen", machine.MachineGroup_idJensen);
                    command.Parameters.AddWithValue("@PingStatus", machine.PingStatus);
                    command.Parameters.AddWithValue("@Positions", machine.Positions);
                    command.Parameters.AddWithValue("@ProcessCodeType", machine.ProcessCodeType);
                    p = command.Parameters.Add("@CustArtUpdate", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = machine.CustArtUpdate == null ? DBNull.Value : (object)machine.CustArtUpdate;

                    p = command.Parameters.Add("@OperatorUpdate", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = machine.OperatorUpdate == null ? DBNull.Value : (object)machine.OperatorUpdate;

                    command.Parameters.AddWithValue("@BatchSizeFactor", machine.BatchSizeFactor);
                    command.Parameters.AddWithValue("@ShowBatchText", machine.ShowBatchText);
                    command.Parameters.AddWithValue("@UseMachineCount", machine.UseMachineCount);
                    command.Parameters.AddWithValue("@MachineCountExitPoint", machine.MachineCountExitPoint);
                    if (da.DatabaseVersion >= 1.4)
                    {
                        command.Parameters.AddWithValue("@OperatorCountExitPoint", machine.OperatorCountExitPoint);
                        command.Parameters.AddWithValue("@ExcludeFromPowerCalc", machine.ExcludeFromPowerCalc);
                        command.Parameters.AddWithValue("@ProductionReference", machine.ProductionReference);
                    }
                    if (da.DatabaseVersion >= 1.97)
                    {
                        command.Parameters.AddWithValue("@ProductionGroupID", machine.ProductionGroupID);
                        command.Parameters.AddWithValue("@Settings", machine.Settings);
                    }
                    command.Parameters.AddWithValue("@CommandLine", machine.CommandLine);
                    command.Parameters.AddWithValue("@CommandDescription", machine.CommandDesc);
                    command.Parameters.AddWithValue("@BatchUpper", machine.BatchUpper);
                    command.Parameters.AddWithValue("@BatchLower", machine.BatchLower);

                    command.Parameters.Add("@Ret", SqlDbType.Int);
                    command.Parameters["@Ret"].Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        object spResult=command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (spResult != null)
                        {
                            machine.RecNum = (int)spResult;
                            machine.HasChanged = false;
                        }
                        InvalidateMachines();
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

        public void InsertNewMachineAlias(MachineLogAlias machineAlias)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblMachineAlias]
                                   ([AliasID]
                                   ,[Description]
                                   ,[SourceID]
                                   ,[SubIDList]
                                   ,[DestID]
                                   ,[SourceIndex]
                                   ,[ConversionType]
                                   ,[Switches]
                                   ,[Active])
                             VALUES
                                   (@AliasID
                                   ,@Description
                                   ,@SourceID
                                   ,@SubIDList
                                   ,@DestID
                                   ,@SourceIndex
                                   ,@ConversionType
                                   ,@Switches
                                   ,@Active)";
            try
            {
                if (machineAlias.AliasID < 0)
                    machineAlias.AliasID = NextMachineAliasRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AliasID", machineAlias.AliasID);
                    command.Parameters.AddWithValue("@Description", machineAlias.Description);
                    command.Parameters.AddWithValue("@SourceID", machineAlias.SourceID);
                    command.Parameters.AddWithValue("@SubIDList", machineAlias.SubIDList);
                    command.Parameters.AddWithValue("@DestID", machineAlias.DestID);
                    command.Parameters.AddWithValue("@SourceIndex", machineAlias.SourceIndex);
                    command.Parameters.AddWithValue("@ConversionType", machineAlias.ConversionType);
                    command.Parameters.AddWithValue("@Switches", machineAlias.Switches);
                    command.Parameters.AddWithValue("@Active", machineAlias.Active);
                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        machineAlias.HasChanged = false;
                        machineAlias.ForceNew = false;
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

        public void UpdateMachineDetails(Machine machine)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            double dbVer = da.DatabaseVersion;
            string commandString =
                @"UPDATE [dbo].tblMachines
                  SET idJensen = @idJensen
                    , ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , IP_Address = @IP_Address
                    , FTP_Path_CustArt = @FTP_Path_CustArt
                    , FTP_Path_Operators = @FTP_Path_Operators
                    , AllowCustomers = @AllowCustomers
                    , SendConfiguration = @SendConfiguration
                    , NormValue = @NormValue
                    , NormUnit_idJensen = @NormUnit_idJensen
                    , NoFlow = @NoFlow
                    , MachineGroup_idJensen = @MachineGroup_idJensen
                    , PingStatus = @PingStatus
                    , Positions = @Positions
                    , ProcessCodeType = @ProcessCodeType
                    , CustArtUpdate = @CustArtUpdate
                    , OperatorUpdate = @OperatorUpdate
                    , BatchSizeFactor = @BatchSizeFactor
                    , ShowBatchText = @ShowBatchText
                    , UseMachineCount = @UseMachineCount
                    , MachineCountExitPoint = @MachineCountExitPoint";
            if (dbVer >= 1.4)
            {
                commandString += @", OperatorCountExitPoint = @OperatorCountExitPoint
                    , ExcludeFromPowerCalc = @ExcludeFromPowerCalc
                    , ProductionReference = @ProductionReference";
            }
            if (dbVer >= 1.9)
            {
                commandString += @", ProductionGroupID = @ProductionGroupID ";
            }
            if (dbVer >= 1.97)
            {
                commandString += @", Settings = @Settings ";
            }
            commandString += @", CommandLine = @CommandLine
                    , CommandDescription = @CommandDescription
                    , BatchUpper = @BatchUpper
                    , BatchLower = @BatchLower

                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", machine.RecNum);
                    command.Parameters.AddWithValue("@idJensen", machine.idJensen);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = machine.ExtRef == null ? DBNull.Value : (object)machine.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", machine.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", machine.LongDescription);
                    command.Parameters.AddWithValue("@IP_Address", machine.IP_Address);
                    command.Parameters.AddWithValue("@FTP_Path_CustArt", machine.FTP_Path_CustArt);
                    command.Parameters.AddWithValue("@FTP_Path_Operators", machine.FTP_Path_Operators);
                    command.Parameters.AddWithValue("@AllowCustomers", machine.AllowCustomers);
                    command.Parameters.AddWithValue("@SendConfiguration", machine.SendConfiguration);
                    command.Parameters.AddWithValue("@NormValue", machine.NormValue);
                    command.Parameters.AddWithValue("@NormUnit_idJensen", machine.NormUnit_idJensen);
                    command.Parameters.AddWithValue("@NoFlow", machine.NoFlow);
                    command.Parameters.AddWithValue("@MachineGroup_idJensen", machine.MachineGroup_idJensen);
                    command.Parameters.AddWithValue("@PingStatus", machine.PingStatus);
                    command.Parameters.AddWithValue("@Positions", machine.Positions);
                    command.Parameters.AddWithValue("@ProcessCodeType", machine.ProcessCodeType);
                    p = command.Parameters.Add("@CustArtUpdate", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = machine.CustArtUpdate == null ? DBNull.Value : (object)machine.CustArtUpdate;

                    p = command.Parameters.Add("@OperatorUpdate", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = machine.OperatorUpdate == null ? DBNull.Value : (object)machine.OperatorUpdate;

                    command.Parameters.AddWithValue("@BatchSizeFactor", machine.BatchSizeFactor);
                    command.Parameters.AddWithValue("@ShowBatchText", machine.ShowBatchText);
                    command.Parameters.AddWithValue("@UseMachineCount", machine.UseMachineCount);
                    command.Parameters.AddWithValue("@MachineCountExitPoint", machine.MachineCountExitPoint);
                    command.Parameters.AddWithValue("@CommandLine", machine.CommandLine);
                    command.Parameters.AddWithValue("@CommandDescription", machine.CommandDesc);
                    command.Parameters.AddWithValue("@BatchUpper", machine.BatchUpper);
                    command.Parameters.AddWithValue("@BatchLower", machine.BatchLower);
                    if (dbVer >= 1.4)
                    {
                        command.Parameters.AddWithValue("@OperatorCountExitPoint", machine.OperatorCountExitPoint);
                        command.Parameters.AddWithValue("@ExcludeFromPowerCalc", machine.ExcludeFromPowerCalc);
                        command.Parameters.AddWithValue("@ProductionReference", machine.ProductionReference);
                    }
                    if (dbVer >= 1.9)
                    {
                        command.Parameters.AddWithValue("@ProductionGroupID", machine.ProductionGroupID);
                    }
                    if (dbVer >= 1.97)
                    {
                        command.Parameters.AddWithValue("@Settings", machine.Settings);
                    }
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    machine.HasChanged = false;
                }
                InvalidateMachines();
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

        public Boolean UpdateMachineState(MachineStatus machineStatus)
        {
            try
            {
                const string commandString =
                    @"UPDATE dbo.tblMachineStatus
                      SET Status = @Status
                        , PieceCount = @PieceCount
                        , OnTime = @OnTime
                        , RunTime = @RunTime
                        , UpdateTime = @UpdateTime
                        , LastMessage = @LastMessage
                      WHERE Machine_idJensen = @Machine_idJensen

                      SELECT @@ROWCOUNT";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@Status", machineStatus.Status);
                    command.Parameters.AddWithValue("@PieceCount", machineStatus.PieceCount);
                    command.Parameters.AddWithValue("@OnTime", machineStatus.OnTime);
                    command.Parameters.AddWithValue("@RunTime", machineStatus.RunTime);
                    command.Parameters.AddWithValue("@Machine_idJensen", machineStatus.Machine_idJensen);
                    command.Parameters.AddWithValue("@UpdateTime", machineStatus.UpdateTime);
                    SqlParameter p = command.Parameters.Add("@LastMessage", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = machineStatus.LastMessage == null ? DBNull.Value : (object)machineStatus.LastMessage;

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    InvalidateMachines();
                    return (RecordCount != null && (int)RecordCount > 0);
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

        public Boolean UpdateMachineLastMessage(int MachineID)
        {
            const string commandstring =
                @"UPDATE tblMachineStatus
                  SET LastMessage = GETDATE()
                  WHERE Machine_idJensen = @MachineID";
            using (SqlCommand command = new SqlCommand(commandstring))
            {
                command.Parameters.AddWithValue("@MachineID", MachineID);
                InvalidateMachines();
                return (command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup) > 0);
            }
        }

        public void UpdateMachineSubID(MachineSubID machineSubID)
        {
            const string commandstring = @"UPDATE [dbo].[tblMachineSubID]
                                                   SET [Machine_idJensen] = @Machine_idJensen
                                                      ,[SubID] = @SubID
                                                      ,[Operator_idJensen] = @Operator_idJensen
                                                      ,[Customer_idJensen] = @Customer_idJensen
                                                      ,[Article_idJensen] = @Article_idJensen
                                                      ,[Operator_Remote] = @Operator_Remote
                                                 WHERE [RecNum] = @RecNum";
            try
            {
                using (SqlCommand command = new SqlCommand(commandstring))
                {
                    command.Parameters.AddWithValue("@Recnum", machineSubID.RecNum);
                    command.Parameters.AddWithValue("@Machine_idJensen", machineSubID.Machine_idJensen);
                    command.Parameters.AddWithValue("@SubID", machineSubID.SubID);
                    command.Parameters.AddWithValue("@Operator_idJensen", machineSubID.Operator_idJensen);
                    command.Parameters.AddWithValue("@Customer_idJensen", machineSubID.Customer_idJensen);
                    command.Parameters.AddWithValue("@Article_idJensen", machineSubID.Article_idJensen);
                    command.Parameters.AddWithValue("@Operator_Remote", machineSubID.Operator_Remote);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);


                    machineSubID.HasChanged = false;

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

        public void UpdateMachineAlias(MachineLogAlias machineAlias)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblMachineAlias]
                                       SET [Description] = @Description
                                          ,[SourceID] = @SourceID
                                          ,[SubIDList] = @SubIDList
                                          ,[DestID] = @DestID
                                          ,[SourceIndex] = @SourceIndex
                                          ,[ConversionType] = @ConversionType
                                          ,[Switches] = @Switches
                                          ,[Active] = @Active
                                     WHERE [AliasID] = @AliasID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AliasID", machineAlias.AliasID);
                    command.Parameters.AddWithValue("@Description", machineAlias.Description);
                    command.Parameters.AddWithValue("@SourceID", machineAlias.SourceID);
                    command.Parameters.AddWithValue("@SubIDList", machineAlias.SubIDList);
                    command.Parameters.AddWithValue("@DestID", machineAlias.DestID);
                    command.Parameters.AddWithValue("@SourceIndex", machineAlias.SourceIndex);
                    command.Parameters.AddWithValue("@ConversionType", machineAlias.ConversionType);
                    command.Parameters.AddWithValue("@Switches", machineAlias.Switches);
                    command.Parameters.AddWithValue("@Active", machineAlias.Active);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    machineAlias.HasChanged = false;
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

        public void DeleteMachineDetails(Machine machine)
        {
            const string commandString =
             @"DECLARE	@return_value int
               EXEC	@return_value = [dbo].[spDeleteMachines]
               @idJensen
               SELECT	'Return Value' = @return_value";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", machine.idJensen);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    InvalidateMachines();
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

        public void DeleteMachineAlias(MachineLogAlias machineAlias)
        {
            DeleteTableRecord("tblMachineAlias", "AliasID", machineAlias.AliasID);
        }
        
        #endregion
    }

    #region Data Collection Class

    #region Machines Class
    public class Machines : List<Machine>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Machine machine)
        {
            base.Add(machine);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, machine));
            }
        }

        new public void Remove(Machine machine)
        {
            base.RemoveAt(this.IndexOf(machine));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, machine));
            }
        }
        #endregion


        private double lifespan = 1.0 / 60.0; //1 minute
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
                    DateTime tblud1 = da.TableLastUpdated("tblMachines");
                    DateTime tblud2 = da.TableLastUpdated("tblMachineStatus");
                    lastDBUpdate = tblud2;
                    if (tblud1 > tblud2) lastDBUpdate = tblud1;
                    int x=lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
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
        int ShortDescriptionPos;
        int LongDescriptionPos;
        int IP_AddressPos;
        int FTP_Path_CustArtPos;
        int FTP_Path_OperatorsPos;
        int AllowCustomersPos;
        int SendConfigurationPos;
        int NormValuePos;
        int NormUnit_idJensenPos;
        int NoFlowPos;
        int MachineGroup_idJensenPos;
        int ProductionGroupIDPos;
        int PingStatusPos;
        int PositionsPos;
        int ProcessCodeTypePos = -1;
        int CustArtUpdatePos;
        int OperatorUpdatePos;
        int BatchSizeFactorPos;
        int ShowBatchTextPos;
        int UseMachineCountPos;
        int MachineCountExitPos;
        int OperatorCountExitPos = -1;
        int ExcludeFromPowerCalcPos = -1;
        int ProductionReferencePos = -1;
        int CommandLinePos;
        int CommandDescPos;
        int BatchUpperPos;
        int BatchLowerPos;
        int SettingsPos;

        int RecNumMSPos;
        int Machine_idJensenPos;
        int StatusPos;
        int PieceCountPos;
        int OnTimePos;
        int RunTimePos;
        int UpdateTimePos;
        int LastMessagePos;
        int LastSuccessfulPingPos;

        public void SetFieldPositions(SqlDataReader dr)
        {
            RecNumPos = dr.GetOrdinal("RecNum");
            idJensenPos = dr.GetOrdinal("idJensen");
            ExtRefPos = dr.GetOrdinal("ExtRef");
            ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            LongDescriptionPos = dr.GetOrdinal("LongDescription");
            IP_AddressPos = dr.GetOrdinal("IP_Address");
            FTP_Path_CustArtPos = dr.GetOrdinal("FTP_Path_CustArt");
            FTP_Path_OperatorsPos = dr.GetOrdinal("FTP_Path_Operators");
            AllowCustomersPos = dr.GetOrdinal("AllowCustomers");
            SendConfigurationPos = dr.GetOrdinal("SendConfiguration");
            NormValuePos = dr.GetOrdinal("NormValue");
            NormUnit_idJensenPos = dr.GetOrdinal("NormUnit_idJensen");
            NoFlowPos = dr.GetOrdinal("NoFlow");
            MachineGroup_idJensenPos = dr.GetOrdinal("MachineGroup_idJensen");
            PingStatusPos = dr.GetOrdinal("PingStatus");
            PositionsPos = dr.GetOrdinal("Positions");
            CustArtUpdatePos = dr.GetOrdinal("CustArtUpdate");
            OperatorUpdatePos = dr.GetOrdinal("OperatorUpdate");
            BatchSizeFactorPos = dr.GetOrdinal("BatchSizeFactor");
            ShowBatchTextPos = dr.GetOrdinal("ShowBatchText");
            UseMachineCountPos = dr.GetOrdinal("UseMachineCount");
            MachineCountExitPos = dr.GetOrdinal("MachineCountExitPoint");
            CommandLinePos = dr.GetOrdinal("CommandLine");
            CommandDescPos = dr.GetOrdinal("CommandDescription");
            BatchUpperPos = dr.GetOrdinal("BatchUpper");
            BatchLowerPos = dr.GetOrdinal("BatchLower");

            RecNumMSPos = dr.GetOrdinal("RecNumMS");
            Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            StatusPos = dr.GetOrdinal("Status");
            PieceCountPos = dr.GetOrdinal("PieceCount");
            OnTimePos = dr.GetOrdinal("OnTime");
            RunTimePos = dr.GetOrdinal("RunTime");
            UpdateTimePos = dr.GetOrdinal("UpdateTime");
            LastMessagePos = dr.GetOrdinal("LastMessage");
            ProcessCodeTypePos = dr.GetOrdinal("ProcessCodeType");
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (da.DatabaseVersion >= 1.4)
            {
                try
                {
                    OperatorCountExitPos = dr.GetOrdinal("OperatorCountExitPoint");
                    ExcludeFromPowerCalcPos = dr.GetOrdinal("ExcludeFromPowerCalc");
                    ProductionReferencePos = dr.GetOrdinal("ProductionReference");
                }
                catch
                {
                    Debug.WriteLine("1.4 Metadata error for Machines");
                }
            }
            if (da.DatabaseVersion >= 1.9)
            {
                try
                {
                    ProductionGroupIDPos = dr.GetOrdinal("ProductionGroupID");
                }
                catch
                {
                    Debug.WriteLine("1.9 Metadata error for Machines");
                }
            }
            if (da.DatabaseVersion >= 1.97)
            {
                try
                {
                    SettingsPos = dr.GetOrdinal("Settings");
                }
                catch
                {
                    Debug.WriteLine("1.97 Metadata error for Machines");
                }
            }
            if (da.DatabaseVersion >= 1.99)
            {
                try
                {
                    LastSuccessfulPingPos = dr.GetOrdinal("LastSuccessfulPing");
                }
                catch
                {
                    Debug.WriteLine("1.99 Metadata error for Machines");
                }
            }
        }
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);
            this.Clear();

            while (dr.Read())
            {
                // Add to Machines Collection
                Machine machine = FillMachine(dr);
                this.Add(machine);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public Machine FillMachine(SqlDataReader dr)
        {
            Machine machine;
            machine = new Machine()
            {
                RecNum = dr.GetInt32(RecNumPos),
                idJensen = dr.GetInt16(idJensenPos),
                ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                ShortDescription = dr.GetString(ShortDescriptionPos),
                LongDescription = dr.GetString(LongDescriptionPos),
                IP_Address = dr.IsDBNull(IP_AddressPos) ? string.Empty : dr.GetString(IP_AddressPos),
                FTP_Path_CustArt = dr.IsDBNull(FTP_Path_CustArtPos) ? string.Empty : dr.GetString(FTP_Path_CustArtPos),
                FTP_Path_Operators = dr.IsDBNull(FTP_Path_OperatorsPos) ? string.Empty : dr.GetString(FTP_Path_OperatorsPos),
                AllowCustomers = dr.GetBoolean(AllowCustomersPos),
                SendConfiguration = dr.GetBoolean(SendConfigurationPos),
                OldNorm = dr.GetInt32(NormValuePos),
                NormUnit_idJensen = dr.GetInt16(NormUnit_idJensenPos),
                NoFlow = dr.GetInt32(NoFlowPos),
                MachineGroup_idJensen = dr.IsDBNull(MachineGroup_idJensenPos) ? (short)0 : dr.GetInt16(MachineGroup_idJensenPos),
                PingStatus = dr.GetBoolean(PingStatusPos),
                Positions = dr.GetInt32(PositionsPos),
                ProcessCodeType = dr.GetInt16(ProcessCodeTypePos),
                CustArtUpdate = dr.IsDBNull(CustArtUpdatePos) ? null : (DateTime?)dr.GetDateTime(CustArtUpdatePos),
                OperatorUpdate = dr.IsDBNull(OperatorUpdatePos) ? null : (DateTime?)dr.GetDateTime(OperatorUpdatePos),
                BatchSizeFactor = (double)dr.GetDecimal(BatchSizeFactorPos),
                ShowBatchText = dr.GetBoolean(ShowBatchTextPos),
                UseMachineCount = dr.GetBoolean(UseMachineCountPos),
                MachineCountExitPoint = dr.GetBoolean(MachineCountExitPos),
                CommandLine = dr.GetString(CommandLinePos),
                CommandDesc = dr.GetString(CommandDescPos),
                BatchUpper = dr.GetInt32(BatchUpperPos),
                BatchLower = dr.GetInt32(BatchLowerPos)
            };
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (da.DatabaseVersion >= 1.4)
            {
                machine.OperatorCountExitPoint = dr.GetBoolean(OperatorCountExitPos);
                machine.ExcludeFromPowerCalc = dr.GetBoolean(ExcludeFromPowerCalcPos);
                machine.ProductionReference = dr.GetBoolean(ProductionReferencePos);                
            }
            else
            {
                machine.OperatorCountExitPoint = machine.MachineCountExitPoint;
                machine.ExcludeFromPowerCalc = true;
                machine.ProductionReference = false;
            }
            if (da.DatabaseVersion >= 1.9)
            {
                machine.ProductionGroupID = dr.GetInt32(ProductionGroupIDPos);
            }
            if (da.DatabaseVersion >= 1.97)
            {
                machine.Settings = dr.GetString(SettingsPos);
            }
            if (!dr.IsDBNull(RecNumMSPos))
            {
                machine.MachineStatus = FillMachineStatus(dr);
            }

            machine.HasChanged = false;

            return machine;
        }

        public MachineStatus FillMachineStatus(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (da.DatabaseVersion >= 1.99)
            {
                return new MachineStatus()
                {
                    RecNum = dr.GetInt32(RecNumMSPos),
                    Machine_idJensen = dr.GetInt16(Machine_idJensenPos),
                    Status = dr.GetInt16(StatusPos),
                    PieceCount = dr.GetInt32(PieceCountPos),
                    OnTime = dr.GetInt32(OnTimePos),
                    RunTime = dr.GetInt32(RunTimePos),
                    UpdateTime = dr.GetDateTime(UpdateTimePos),
                    LastMessage = dr.IsDBNull(LastMessagePos) ? null : (DateTime?)dr.GetDateTime(LastMessagePos),
                    LastSuccessfulPing = dr.IsDBNull(LastSuccessfulPingPos) ? null : (DateTime?)dr.GetDateTime(LastSuccessfulPingPos),
                    HasChanged = false
                };
            }
            else
            {
                return new MachineStatus()
                {
                    RecNum = dr.GetInt32(RecNumMSPos),
                    Machine_idJensen = dr.GetInt16(Machine_idJensenPos),
                    Status = dr.GetInt16(StatusPos),
                    PieceCount = dr.GetInt32(PieceCountPos),
                    OnTime = dr.GetInt32(OnTimePos),
                    RunTime = dr.GetInt32(RunTimePos),
                    UpdateTime = dr.GetDateTime(UpdateTimePos),
                    LastMessage = dr.IsDBNull(LastMessagePos) ? null : (DateTime?)dr.GetDateTime(LastMessagePos),
                    HasChanged = false
                };            
            }
        }

        public Machine GetById(int id)
        {
            return this.Find(delegate(Machine machine)
            {
                return machine.idJensen == id;
            });
        }

        public Machine GetByName(string aName)
        {
            return this.Find(delegate(Machine machine)
            {
                return machine.ShortDescription == aName;
            });
        }

        public Machine GetByNameID(string aNameID)
        {
            return this.Find(delegate(Machine machine)
            {
                return machine.ShortDescAndID == aNameID;
            });
        }


    }
    #endregion

    #region MachineSubIDs Class
    public class MachineSubIDs : List<MachineSubID>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(MachineSubID subid)
        {
            base.Add(subid);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, subid));
            }
        }

        new public void Remove(MachineSubID subid)
        {
            base.RemoveAt(this.IndexOf(subid));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, subid));
            }
        }
        #endregion

        //private double lifespan = 1.0 / 3600;
        private double lifespan = 1.0 / 3600.0; //1 minute
        private string tblName = "tblMachineSubids";
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
        int Machine_idJensenPos;
        int SubIDPos;
        int Operator_idJensenPos;
        int Customer_idJensenPos;
        int Article_idJensenPos;
        int Operator_RemotePos = -1;

        public void SetFieldPositions(SqlDataReader dr)
        {
            RecNumPos = dr.GetOrdinal("RecNum");
            Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            SubIDPos = dr.GetOrdinal("SubID");
            Operator_idJensenPos = dr.GetOrdinal("Operator_idJensen");
            Customer_idJensenPos = dr.GetOrdinal("Customer_idJensen");
            Article_idJensenPos = dr.GetOrdinal("Article_idJensen");
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (da.DatabaseVersion >= 1.7)
            {
                try
                {
                    Operator_RemotePos = dr.GetOrdinal("Operator_Remote");
                }
                catch
                {
                    Debug.WriteLine("1.7 Metadata error for MachineSubIDs");
                }
            }
        }

    
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Machines Collection
                MachineSubID machineSubID = FillMachineSubID(dr);
                this.Add(machineSubID);
            }

            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public MachineSubID FillMachineSubID(SqlDataReader dr)
        {
            MachineSubID machineSubID = new MachineSubID();
            machineSubID.RecNum = dr.GetInt32(RecNumPos);
            machineSubID.Machine_idJensen = dr.GetInt16(Machine_idJensenPos);
            machineSubID.SubID = dr.GetInt32(SubIDPos);
            machineSubID.Operator_idJensen = (int)dr.GetInt16(Operator_idJensenPos);
            machineSubID.Customer_idJensen = dr.GetInt32(Customer_idJensenPos);
            machineSubID.Article_idJensen = dr.GetInt16(Article_idJensenPos);
            if (Operator_RemotePos > 0)
            {
                int RemoteOp = dr.GetInt32(Operator_RemotePos);
                if ((machineSubID.Operator_idJensen <= 0) && (RemoteOp > 0))
                    machineSubID.Operator_idJensen = RemoteOp;
                machineSubID.Operator_Remote = RemoteOp;
            }
            machineSubID.HasChanged = false;

            return machineSubID;
        }

        public MachineSubID GetByMachineSubID(int machineID, int subID)
        {
            return Find(delegate(MachineSubID mID) 
            { 
                return (mID.Machine_idJensen == machineID) 
                && (mID.SubID == subID); 
            });
        }

        public MachineSubIDs GetByRemoteOperator(int operatorID)
        {
            MachineSubIDs msubs = new MachineSubIDs();
            foreach (MachineSubID msub in this)
            {
                if (msub.Operator_Remote == operatorID)
                    msubs.Add(msub);
            }
            return msubs;
        }
    }
    #endregion

    #region machinealiases class
    public class MachineLogAliases : List<MachineLogAlias>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(MachineLogAlias diag)
        {
            base.Add(diag);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, diag));
            }
        }

        new public void Remove(MachineLogAlias diag)
        {
            base.RemoveAt(this.IndexOf(diag));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, diag));
            }
        }

        #endregion

        #region properties
        private double lifespan = 24.0;
        private string tblName = "tblMachineAlias";
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
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        #endregion

        #region methods
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int AliasIDPos = dr.GetOrdinal("AliasID");
            int DescriptionPos = dr.GetOrdinal("Description");
            int SourceIDPos = dr.GetOrdinal("SourceID");
            int SubIDListPos = dr.GetOrdinal("SubIDList");
            int DestIDPos = dr.GetOrdinal("DestID");
            int SourceIndexPos = dr.GetOrdinal("SourceIndex");
            int ConversionTypePos = dr.GetOrdinal("ConversionType");
            int SwitchesPos = dr.GetOrdinal("Switches");
            int ActivePos = dr.GetOrdinal("Active");
            int RemoteIDPos = dr.GetOrdinal("RemoteID");

            this.Clear();
            while (dr.Read())
            {
                MachineLogAlias machineAlias = new MachineLogAlias();
                machineAlias.AliasID = dr.GetInt32(AliasIDPos);
                machineAlias.Description = dr.GetString(DescriptionPos);
                machineAlias.SourceID = dr.GetInt32(SourceIDPos);
                machineAlias.SubIDList = dr.GetString(SubIDListPos);
                machineAlias.DestID = dr.GetInt32(DestIDPos);
                machineAlias.SourceIndex = dr.GetInt32(SourceIndexPos);
                machineAlias.ConversionType = dr.GetInt32(ConversionTypePos);
                machineAlias.Switches = dr.GetString(SwitchesPos);
                machineAlias.Active = dr.GetBoolean(ActivePos);
                machineAlias.RemoteID = dr.GetInt32(RemoteIDPos);
                this.Add(machineAlias);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (MachineLogAlias rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public int MaxRemoteID()
        {
            int remoteid = -1;
            foreach (MachineLogAlias mla in this)
            {
                if (mla.RemoteID > remoteid) 
                    remoteid = mla.RemoteID;
            }
            return remoteid;
        }

        #endregion

        #region find
        public MachineLogAlias GetById(int id)
        {
            return this.Find(delegate(MachineLogAlias visioDiagram)
            {
                return visioDiagram.AliasID == id;
            });
        }

        public MachineLogAliases GetBySourceMachine(int machineID)
        {
            MachineLogAliases mlas = new MachineLogAliases();
            foreach (MachineLogAlias mla in this)
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
            List<int> mList =new List<int>();
            foreach (MachineLogAlias mla in this)
            {
                if (!mList.Contains(mla.SourceID))
                    mList.Add(mla.SourceID);
            }
            return mList;
        }

        #endregion

        public void DeleteAllFromDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.TruncateGroupTable("tblMachineAlias");
        }



        #region updatedb

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (MachineLogAlias rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewMachineAlias(rec);
                }
            }
            else
            {
                foreach (MachineLogAlias rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewMachineAlias(rec);
                    }
                    else
                    {
                        if (rec.HasChanged)
                        {
                            if (rec.DeleteRecord)
                                da.DeleteMachineAlias(rec);
                            else
                                da.UpdateMachineAlias(rec);
                        }
                    }
                }
                MachineLogAliases delList = new MachineLogAliases();
                foreach (MachineLogAlias rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (MachineLogAlias rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }

        #endregion
    }

    #endregion

    #region MachineStatusDetails Class (Public)
    public class MachineStatusDetails : List<MachineStatusDetail>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(MachineStatusDetail rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(MachineStatusDetail rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }
        #endregion

        //private double lifespan = 1.0 / 3600;
        private double lifespan = 1.0 / 3600.0; //1 minute
        private string tblName = "tblMachineStatusDetails";
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
        int Start_RemoteIDPos;
        int MachineIDPos;
        int MachineStatePos;
        int Start_TimeStampPos;
        int End_TimeStampPos;

        public void SetFieldPositions(SqlDataReader dr)
        {
            RecNumPos = dr.GetOrdinal("RecNum");
            Start_RemoteIDPos = dr.GetOrdinal("Start_RemoteID");
            MachineIDPos = dr.GetOrdinal("MachineID");
            MachineStatePos = dr.GetOrdinal("MachineState");
            Start_TimeStampPos = dr.GetOrdinal("Start_TimeStamp");
            End_TimeStampPos = dr.GetOrdinal("End_TimeStamp");
        }
        #endregion

        public MachineStatusDetail FillMachineStatus(SqlDataReader dr)
        {
            MachineStatusDetail msd = new MachineStatusDetail();
            msd.RecNum = dr.GetInt32(RecNumPos);
            msd.Start_RemoteID = dr.GetInt16(Start_RemoteIDPos);
            msd.MachineID = dr.GetInt16(MachineIDPos);
            msd.MachineState = dr.GetInt32(MachineStatePos);
            msd.Start_TimeStamp = dr.GetDateTime(Start_TimeStampPos);
            msd.End_TimeStamp = dr.IsDBNull(End_TimeStampPos) ? null : (DateTime?)dr.GetDateTime(End_TimeStampPos);
            return msd;
        }

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Machines Collection
                MachineStatusDetail msd = FillMachineStatus(dr);
                this.Add(msd);
            }

            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public MachineStatusDetail GetByMachineID(int machineID)
        {
            return Find(delegate(MachineStatusDetail msd)
            {
                return (msd.MachineID == machineID);
            });
        }
    }
    #endregion

    #endregion

    #region bubble texts
    public class BubbleTexts : List<BubbleText>
    {

    }

    public class BubbleText : DataItem
    {
        protected class DataRecord : ICopyableObject
        {
            internal string bubbleContent;
            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        public BubbleText()
        {
            ActiveData = (ICopyableObject)new DataRecord();
        }

        public BubbleText(string aString)
            : this()
        {
            BubbleContent = aString;
        }

        public string BubbleContent
        {
            get
            {
                return this.activeData.bubbleContent;
            }

            set
            {
                this.activeData.bubbleContent = value;
            }
        }

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
                return false;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return true;
            }
        }
        #endregion

     }

    #endregion

    #region Item Classes

    #region Machine Class
    public class Machine : DataItem, IComparable<Machine>, INotifyPropertyChanged
    {
        #region Machine Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal string ExtRef;
            internal string ShortDescription;
            internal string LongDescription;
            internal string IP_Address;
            internal string FTP_Path_CustArt;
            internal string FTP_Path_Operators;
            internal bool AllowCustomers;
            internal bool SendConfiguration;
            internal int NormValue;
            internal int OldNorm; //not stored
            internal int NoFlow; 
            internal int OldNoFlow; //not stored
            internal short NormUnit_idJensen;
            internal short MachineGroup_idJensen;
            internal int ProductionGroupID;
            internal bool PingStatus;
            internal MachineStatus MachineStatus;
            internal int Positions;
            internal int ProcessCodeType;
            internal DateTime? CustArtUpdate;
            internal DateTime? OperatorUpdate;
            internal double BatchSizeFactor;
            internal double CockpitBatchSizeFactor;
            internal double RailBatchSizeFactor;
            internal bool ShowBatchText;
            internal bool UseMachineCount;
            internal bool MachineCountExitPoint;
            //internal bool OperatorCountExitPoint = false;
            internal bool OperatorProductionCountExitPoint = false;//new! 
            internal bool OperatorTimeCountExitPoint = false;//new! 
            internal bool OperatorMultipleCountExitPoint = false;//new! 
            internal bool ExcludeFromPowerCalc = false;
            internal bool ProductionReference = false;
            internal int BatchUpper; 
            internal int BatchLower; 
            internal string CommandLine; 
            internal string CommandDesc; 
            internal string Settings = string.Empty; //new! holder for xml settings
            internal bool OverallMetering = false; //not stored populated from settings
            internal string XMLRPCPort = "0"; //not stored populated from settings

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Machine()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            OldNoFlow = -1;
            activeData.ProductionGroupID = -1;
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
                if (this.activeData.RecNum != value)
                {
                    this.activeData.RecNum = value;
                    NotifyPropertyChanged("RecNum");
                }
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
                if (this.activeData.idJensen != value)
                {
                    this.activeData.idJensen = value;
                    NotifyPropertyChanged("idJensen");
                }
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
                if (this.activeData.ExtRef != value)
                {
                    this.activeData.ExtRef = value;
                    NotifyPropertyChanged("ExtRef");
                }
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
                if (this.activeData.ShortDescription != value)
                {
                    this.activeData.ShortDescription = value;
                    NotifyPropertyChanged("ShortDescription");
                }
            }
        }

        public string ShortDescAndID
        {
            get
            {
                return ShortDescription + " (" + idJensen.ToString() + ")";
            }
        }

        public string MachineName
        {
            get
            {
                return ShortDescAndID;
            }
        }

        public string LongDescription
        {
            get
            {
                if (activeData.LongDescription != string.Empty)
                    return this.activeData.LongDescription;
                else
                    return activeData.ShortDescription;
            }
            set
            {
                if (this.activeData.LongDescription != value)
                {
                    this.activeData.LongDescription = value;
                    NotifyPropertyChanged("LongDescription");
                }
            }
        }

        public string IP_Address
        {
            get
            {
                return this.activeData.IP_Address.Trim();
            }
            set
            {
                if (this.activeData.IP_Address != value)
                {
                    this.activeData.IP_Address = value;
                    NotifyPropertyChanged("IP_Address");
                }
            }
        }

        public string FTP_Path_CustArt
        {
            get
            {
                return this.activeData.FTP_Path_CustArt;
            }
            set
            {
                if (this.activeData.FTP_Path_CustArt != value)
                {
                    this.activeData.FTP_Path_CustArt = value;
                    NotifyPropertyChanged("FTP_Path_CustArt");
                }
            }
        }

        public string FTP_Path_Operators
        {
            get
            {
                return this.activeData.FTP_Path_Operators;
            }
            set
            {
                if (this.activeData.FTP_Path_Operators != value)
                {
                    this.activeData.FTP_Path_Operators = value;
                    NotifyPropertyChanged("FTP_Path_Operators");
                }
            }
        }

        public bool AllowCustomers
        {
            get
            {
                return this.activeData.AllowCustomers;
            }
            set
            {
                if (this.activeData.AllowCustomers != value)
                {
                    this.activeData.AllowCustomers = value;
                    NotifyPropertyChanged("AllowCustomers");
                }
            }
        }

        public bool SendConfiguration
        {
            get
            {
                return this.activeData.SendConfiguration;
            }
            set
            {
                if (this.activeData.SendConfiguration != value)
                {
                    this.activeData.SendConfiguration = value;
                    NotifyPropertyChanged("SendConfiguration");
                }
            }
        }

        public int NormValue
        {
            get
            {
                return this.activeData.NormValue;
            }
            set
            {
                if (this.activeData.NormValue != value)
                {
                    this.activeData.NormValue = value;
                    NotifyPropertyChanged("NormValue");
                }
            }
        }

        public bool NormChanged
        {
            get
            {
                return (!IsNew && HasChanged && (this.activeData.NormValue != this.activeData.OldNorm));
            }
        }

        public int OldNorm
        {
            get
            {
                return this.activeData.OldNorm;
            }
            set
            {
                if (this.activeData.OldNorm != value)
                {
                    NormValue = value;
                    this.activeData.OldNorm = value;
                    NotifyPropertyChanged("OldNorm");
                }
            }
        }

        public int NoFlow
        {
            get
            {
                return this.activeData.NoFlow;
            }
            set
            {
                if (this.activeData.NoFlow != value)
                {
                    this.activeData.NoFlow = value;
                    NotifyPropertyChanged("NoFlow");
                }
            }
        }

        public int OldNoFlow
        {
            get
            {
                return this.activeData.OldNoFlow;
            }
            set
            {
                if (this.activeData.OldNoFlow != value)
                {
                    this.activeData.OldNoFlow = value;
                    NotifyPropertyChanged("OldNoFlow");
                    NoFlow = value;
                }
            }
        }

        public bool NoFlowChanged
        {
            get
            {
                return (!IsNew && HasChanged && (this.activeData.NoFlow 
                    != this.activeData.OldNoFlow));
            }
        }

        public short NormUnit_idJensen
        {
            get
            {
                return this.activeData.NormUnit_idJensen;
            }
            set
            {
                if (this.activeData.NormUnit_idJensen != value)
                {
                    this.activeData.NormUnit_idJensen = value;
                    NotifyPropertyChanged("NormUnit_idJensen");
                }
            }
        }

        public short MachineGroup_idJensen
        {
            get
            {
                return this.activeData.MachineGroup_idJensen;
            }
            set
            {
                if (this.activeData.MachineGroup_idJensen != value)
                {
                    this.activeData.MachineGroup_idJensen = value;
                    NotifyPropertyChanged("MachineGroup_idJensen");
                }
            }
        }

        public string MachineAreaName
        {
            get
            {
                string ma = string.Empty;
                SqlDataAccess da=SqlDataAccess.Singleton;
                MachineGroup mg = da.GetMachineGroup(MachineGroup_idJensen);
                if (mg != null)
                {
                    ma = mg.MachineAreaName;
                }
                return ma;
            }
        }

        public int ProductionGroupID
        {
            get
            {
                return activeData.ProductionGroupID;
            }
            set
            {
                if (activeData.ProductionGroupID != value)
                {
                    activeData.ProductionGroupID = value;
                    NotifyPropertyChanged("ProductionGroupID");
                }
            }
        }

        public string MachineGroup_ShortDescription
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                MachineGroup mg = da.GetMachineGroup(this.activeData.MachineGroup_idJensen);
                if (mg != null)
                    aString = mg.ShortDescription;
                return aString;
            }
        }

        public bool PingStatus
        {
            get
            {
                return this.activeData.PingStatus;
            }
            set
            {
                if (this.activeData.PingStatus != value)
                {
                    this.activeData.PingStatus = value;
                    NotifyPropertyChanged("PingStatus");
                }
            }
        }

        public DateTime? CustArtUpdate
        {
            get
            {
                return this.activeData.CustArtUpdate;
            }
            set
            {
                if (this.activeData.CustArtUpdate != value)
                {
                    this.activeData.CustArtUpdate = value;
                    NotifyPropertyChanged("CustArtUpdate");
                }
            }
        }

        public DateTime? OperatorUpdate
        {
            get
            {
                return this.activeData.OperatorUpdate;
            }
            set
            {
                if (this.activeData.OperatorUpdate != value)
                {
                    this.activeData.OperatorUpdate = value;
                    NotifyPropertyChanged("OperatorUpdate");
                }
            }
        }
        public double BatchSizeFactor
        {
            get
            {
                return this.activeData.BatchSizeFactor;
            }
            set
            {
                if (this.activeData.BatchSizeFactor != value)
                {
                    this.activeData.BatchSizeFactor = value;
                    NotifyPropertyChanged("BatchSizeFactor");
                }
            }
        }

        public double CockpitBatchSizeFactor
        {
            get
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.DatabaseVersion < 1.97)
                    return activeData.BatchSizeFactor;
                else
                    return activeData.CockpitBatchSizeFactor;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.DatabaseVersion < 1.97)
                {
                    if (this.activeData.BatchSizeFactor != value)
                    {
                        this.activeData.BatchSizeFactor = value;
                        NotifyPropertyChanged("CockpitBatchSizeFactor");
                    }
                }
                else
                {
                    if (activeData.CockpitBatchSizeFactor != value)
                    {
                        activeData.CockpitBatchSizeFactor = value;
                        NotifyPropertyChanged("CockpitBatchSizeFactor");
                        writeXmlSettings();
                    }
                }
            }
        }

        public double RailBatchSizeFactor
        {
            get
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.DatabaseVersion < 1.97)
                    return activeData.BatchSizeFactor;
                else
                    return activeData.RailBatchSizeFactor;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.DatabaseVersion < 1.97)
                {
                    if (this.activeData.BatchSizeFactor != value)
                    {
                        this.activeData.BatchSizeFactor = value;
                        NotifyPropertyChanged("RailBatchSizeFactor");
                    }
                }
                else
                {
                    if (activeData.RailBatchSizeFactor != value)
                    {
                        activeData.RailBatchSizeFactor = value;
                        NotifyPropertyChanged("RailBatchSizeFactor");
                        writeXmlSettings();
                    }
                }
            }
        }

        public bool ShowBatchText
        {
            get
            {
                return this.activeData.ShowBatchText;
            }
            set
            {
                if (this.activeData.ShowBatchText != value)
                {
                    this.activeData.ShowBatchText = value;
                    NotifyPropertyChanged("ShowBatchText");
                }
            }
        }

        public bool UseMachineCount
        {
            get
            {
                return this.activeData.UseMachineCount;
            }
            set
            {
                if (this.activeData.UseMachineCount != value)
                {
                    this.activeData.UseMachineCount = value;
                    NotifyPropertyChanged("UseMachineCount");
                    NotifyPropertyChanged("UseProcessCodeCount");
                }
            }
        }

        public bool UseProcessCodeCount
        {
            get
            {
                return !UseMachineCount;
            }
            set
            {
                UseMachineCount = !value;
            }
        }

        public bool MachineCountExitPoint
        {
            get
            {
                return this.activeData.MachineCountExitPoint;
            }
            set
            {
                if (this.activeData.MachineCountExitPoint != value)
                {
                    this.activeData.MachineCountExitPoint = value;
                    NotifyPropertyChanged("MachineCountExitPoint");
                }
            }
        }

        public bool OperatorCountExitPoint //legacy property
        {
            get
            {
                return activeData.OperatorProductionCountExitPoint || activeData.OperatorTimeCountExitPoint;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if ((OperatorCountExitPoint != value) && (da.DatabaseVersion<1.97))
                {
                    OperatorProductionCountExitPoint = value;
                    OperatorTimeCountExitPoint = value;
                    NotifyPropertyChanged("OperatorCountExitPoint");
                }
            }
        }

        public bool OperatorProductionCountExitPoint
        {
            get
            {
                return activeData.OperatorProductionCountExitPoint;
            }
            set
            {
                if (activeData.OperatorProductionCountExitPoint != value)
                {
                    activeData.OperatorProductionCountExitPoint = value;
                    NotifyPropertyChanged("OperatorProductionCountExitPoint");
                    writeXmlSettings();
                }
            }
        }

        public bool OperatorTimeCountExitPoint
        {
            get
            {
                return activeData.OperatorTimeCountExitPoint;
            }
            set
            {
                if (activeData.OperatorTimeCountExitPoint != value)
                {
                    activeData.OperatorTimeCountExitPoint = value;
                    NotifyPropertyChanged("OperatorTimeCountExitPoint");
                    writeXmlSettings();
                }
            }
        }

        public bool OperatorMultipleCountExitPoint
        {
            get
            {
                return activeData.OperatorMultipleCountExitPoint;
            }
            set
            {
                if (activeData.OperatorMultipleCountExitPoint != value)
                {
                    activeData.OperatorMultipleCountExitPoint = value;
                    NotifyPropertyChanged("OperatorMultipleCountExitPoint");
                    writeXmlSettings();
                }
            }
        }

        public bool ExcludeFromPowerCalc
        {
            get
            {
                return this.activeData.ExcludeFromPowerCalc;
            }
            set
            {
                if (this.activeData.ExcludeFromPowerCalc != value)
                {
                    this.activeData.ExcludeFromPowerCalc = value;
                    NotifyPropertyChanged("ExcludeFromPowerCalc");
                }
            }
        }

        public bool ProductionReference
        {
            get
            {
                return this.activeData.ProductionReference;
            }
            set
            {
                if (this.activeData.ProductionReference != value)
                {
                    this.activeData.ProductionReference = value;
                    NotifyPropertyChanged("ProductionReference");
                }
            }
        }

        public bool NotMachineCountExitPoint
        {
            get
            {
                return !MachineCountExitPoint;
            }
            set
            {
                MachineCountExitPoint = !value;
            }
        }

        public MachineStatus MachineStatus
        {
            get
            {
                return this.activeData.MachineStatus;
            }
            set
            {
                if (this.activeData.MachineStatus != value)
                {
                    this.activeData.MachineStatus = value;
                    NotifyPropertyChanged("MachineStatus");
                }
            }
        }

        public short Status
        {
            get
            {
                return this.activeData.MachineStatus.Status;
            }
            set
            {
                if (this.activeData.MachineStatus.Status != value)
                {
                    this.activeData.MachineStatus.Status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        private string DateTimeToStr(DateTime aDateTime)
        {
            string aString=aDateTime.ToString("HH:mm dd/MM/yyyy");
            return aString;
        }

        private DateTime StrToDateTime(string aString)
        {
            DateTime dt = DateTime.MinValue;
            DateTime parsed;
            if (DateTime.TryParse(aString, out parsed))
                dt = parsed;
            return dt;
        }

        public String UpdateTimeStr
        {
            get
            {
                return DateTimeToStr(this.activeData.MachineStatus.UpdateTime);
            }
            set
            {
                UpdateTime = StrToDateTime(value);
                NotifyPropertyChanged("UpdateTimeStr");
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return this.activeData.MachineStatus.UpdateTime;
            }
            set
            {
                if (this.activeData.MachineStatus.UpdateTime != value)
                {
                    this.activeData.MachineStatus.UpdateTime = value;
                    NotifyPropertyChanged("UpdateTime");
                    NotifyPropertyChanged("UpdateTimeStr");
                }
            }
        }

        public String LastMessageStr
        {
            get
            {
                if (LastMessage != null)
                {
                    DateTime lm = (DateTime)LastMessage;
                    string aString = DateTimeToStr(lm);
                    return aString;
                }
                else return string.Empty;
            }
            set
            {
                if (value != string.Empty)
                {
                    this.activeData.MachineStatus.LastMessage = StrToDateTime(value);
                    NotifyPropertyChanged("LastMessageStr");
                }
                else 
                {
                    this.activeData.MachineStatus.LastMessage=null;
                    NotifyPropertyChanged("LastMessageStr");
                }
            }
        }

        public DateTime? LastMessage
        {
            get
            {
                return this.activeData.MachineStatus.LastMessage;
            }
            set
            {
                if (this.activeData.MachineStatus.LastMessage != value)
                {
                    this.activeData.MachineStatus.LastMessage = value;
                    NotifyPropertyChanged("LastMessage");
                    NotifyPropertyChanged("LastMessageStr");
                }
            }
        }

        public int OnTime
        {
            get
            {
                return this.activeData.MachineStatus.OnTime;
            }
            set
            {
                if (this.activeData.MachineStatus.OnTime != value)
                {
                    this.activeData.MachineStatus.OnTime = value;
                    NotifyPropertyChanged("OnTime");
                }
            }
        }

        public int RunTime
        {
            get
            {
                return this.activeData.MachineStatus.RunTime;
            }
            set
            {
                if (this.activeData.MachineStatus.RunTime != value)
                {
                    this.activeData.MachineStatus.RunTime = value;
                    NotifyPropertyChanged("RunTime");
                }
            }
        }

        public int PieceCount
        {
            get
            {
                return this.activeData.MachineStatus.PieceCount;
            }
            set
            {
                if (this.activeData.MachineStatus.PieceCount != value)
                {
                    this.activeData.MachineStatus.PieceCount = value;
                    NotifyPropertyChanged("PieceCount");
                }
            }
        }

        public int Positions
        {
            get
            {
                if (activeData.Positions > 0)
                    return this.activeData.Positions;
                else return 1;
            }
            set
            {
                if ((this.activeData.Positions != value) && (value != 0))
                {
                    this.activeData.Positions = value;
                    NotifyPropertyChanged("Positions");
                }
            }
        }

        public int ProcessCodeType
        {
            get
            {
                return this.activeData.ProcessCodeType;
            }
            set
            {
                if (this.activeData.ProcessCodeType != value)
                {
                    this.activeData.ProcessCodeType = value;
                    NotifyPropertyChanged("ProcessCodeType");
                    NotifyPropertyChanged("ProcessCodeArticle");
                    NotifyPropertyChanged("ProcessCodeCategory");
                }
            }
        }

        public bool ProcessCodeArticle
        {
            get
            {
                return ProcessCodeType == 0;
            }
            set
            {
                if (value)
                    ProcessCodeType = 0;
                else
                    ProcessCodeType = 1;
            }
        }

        public bool ProcessCodeCategory
        {
            get
            {
                return this.activeData.ProcessCodeType == 1;
            }
            set
            {
                if (value)
                    this.activeData.ProcessCodeType = 1;
                else
                    this.activeData.ProcessCodeType = 0;
            }
        }

        public int BatchUpper
        {
            get
            {
                return this.activeData.BatchUpper;
            }
            set
            {
                if (this.activeData.BatchUpper != value)
                {
                    this.activeData.BatchUpper = value;
                    NotifyPropertyChanged("BatchUpper");
                }
            }
        }

        public int BatchLower
        {
            get
            {
                return this.activeData.BatchLower;
            }
            set
            {
                if (this.activeData.BatchLower != value)
                {
                    this.activeData.BatchLower = value;
                    NotifyPropertyChanged("BatchLower");
                }
            }
        }

        public string CommandLine
        {
            get
            {
                return this.activeData.CommandLine;
            }
            set
            {
                if (this.activeData.CommandLine != value)
                {
                    this.activeData.CommandLine = value;
                    NotifyPropertyChanged("CommandLine");
                }
            }
        }

        public string CommandDesc
        {
            get
            {
                return this.activeData.CommandDesc;
            }
            set
            {
                if (this.activeData.CommandDesc != value)
                {
                    this.activeData.CommandDesc = value;
                    NotifyPropertyChanged("CommandDesc");
                }
            }
        }


        private void readXmlSettings()
        {
            XMLDataAccess Xmlda = XMLDataAccess.Singleton;
            activeData.OverallMetering = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OverallMetering" }, activeData.OverallMetering);
            activeData.OperatorTimeCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorTimeCountExitPoint" }, activeData.OperatorTimeCountExitPoint);
            activeData.OperatorMultipleCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorMultipleCountExitPoint" }, activeData.OperatorMultipleCountExitPoint);
            activeData.OperatorProductionCountExitPoint = Xmlda.ReadBoolSetting(Settings, new string[2] { "Settings", "OperatorProductionCountExitPoint" }, activeData.OperatorProductionCountExitPoint);
            activeData.CockpitBatchSizeFactor = Xmlda.ReadDoubleSetting(Settings, new string[2] { "Settings", "CockpitBatchSizeFactor" }, activeData.CockpitBatchSizeFactor);
            activeData.XMLRPCPort = Xmlda.ReadStringSetting(Settings, new string[2] { "Settings", "XmlRPCPort" }, activeData.XMLRPCPort);
            if (activeData.CockpitBatchSizeFactor <= 0) activeData.CockpitBatchSizeFactor = BatchSizeFactor;
            if (activeData.RailBatchSizeFactor <= 0) activeData.RailBatchSizeFactor = BatchSizeFactor;
            activeData.RailBatchSizeFactor = Xmlda.ReadDoubleSetting(Settings, new string[2] { "Settings", "RailBatchSizeFactor" }, activeData.RailBatchSizeFactor);
        }

        public string Settings
        {
            get
            {
                return activeData.Settings;
            }
            set
            {
                if (activeData.Settings != value)
                {
                    activeData.Settings = value;
                    NotifyPropertyChanged("Settings");
                    readXmlSettings();
                }
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
            string key = String.Format(@"<{0}> {1} </{0}>", propertyname, value);
            return key;
        }

        private string genXMLSettingKey(string propertyname, String value)
        {
            string key = String.Format(@"<{0}> {1} </{0}>", propertyname, value);
            return key;
        }



        private void writeXmlSettings()
        {
            string settings = @" <Settings>  ";
            settings += genXMLSettingKey("OverallMetering", OverallMetering);
            settings += genXMLSettingKey("OperatorTimeCountExitPoint", OperatorTimeCountExitPoint);
            settings += genXMLSettingKey("OperatorMultipleCountExitPoint", OperatorMultipleCountExitPoint);
            settings += genXMLSettingKey("OperatorProductionCountExitPoint", OperatorProductionCountExitPoint);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
            settings += genXMLSettingKey("CockpitBatchSizeFactor", CockpitBatchSizeFactor.ToString("F4", culture));
            settings += genXMLSettingKey("RailBatchSizeFactor", RailBatchSizeFactor.ToString("F4", culture));
            settings += genXMLSettingKey("XmlRPCPort", XMLRPCPort);
            settings += @" </Settings> ";
            activeData.Settings = settings;
        }

        public bool OverallMetering
        {
            get
            {
                return activeData.OverallMetering;
            }
            set
            {
                if (activeData.OverallMetering != value)
                {
                    activeData.OverallMetering = value;
                    NotifyPropertyChanged("OverallMetering");
                    writeXmlSettings();
                }
            }
        }

        public string XMLRPCPort
        {
            get
            {
                return activeData.XMLRPCPort;
            }
            set
            {
                if (activeData.XMLRPCPort != value)
                {
                    activeData.XMLRPCPort = value;
                    NotifyPropertyChanged("XMLRPCPort");
                    writeXmlSettings();
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion

        public int CompareTo(Machine other) { return ShortDescription.CompareTo(other.ShortDescription); }
 
    }
    #endregion

    #region MachineSubID Class
    public class MachineSubID : DataItem, INotifyPropertyChanged
    {
        #region Machine SubID Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short Machine_idJensen;
            internal int SubID;
            internal int Operator_idJensen;
            internal int Operator_Remote;
            internal int Customer_idJensen;
            internal short Article_idJensen;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public MachineSubID()
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
                if (this.activeData.RecNum != value)
                {
                    this.activeData.RecNum = value;
                    NotifyPropertyChanged("RecNum");
                }
            }
        }

        public short Machine_idJensen
        {
            get
            {
                return this.activeData.Machine_idJensen;
            }
            set
            {
                if (this.activeData.Machine_idJensen != value)
                {
                    this.activeData.Machine_idJensen = value;
                    NotifyPropertyChanged("Machine_idJensen");
                }
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
                if (this.activeData.SubID != value)
                {
                    this.activeData.SubID = value;
                    NotifyPropertyChanged("SubID");
                }
            }
        }

        public int Operator_idJensen
        {
            get
            {
                if (this.activeData.Operator_Remote > 0)
                    return this.activeData.Operator_Remote;
                else
                    return this.activeData.Operator_idJensen;
            }
            set
            {
                if ((activeData.Operator_Remote > 0) && (activeData.Operator_Remote != value))
                {
                    activeData.Operator_Remote = value;
                    NotifyPropertyChanged("Operator_Remote");
                }
                if (this.activeData.Operator_idJensen != value)
                {
                    activeData.Operator_idJensen = value;
                    NotifyPropertyChanged("Operator_idJensen");
                }
            }
        }

        private string GetOperatorNameID()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            Operators ops = da.GetAllActiveOperators();
            string aString = string.Empty;
            if (ops != null)
            {
                Operator op = ops.GetById(Operator_idJensen);
                if (op != null)
                    aString = op.OperatorNameID;
            }
            return aString;
        }

        private string GetCustomerNameID()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            Customers custs = da.GetAllActiveCustomers();
            string aString = string.Empty;
            if (custs != null)
            {
                Customer cust = custs.GetById(Customer_idJensen);
                if (cust!=null)
                    aString = cust.CustomerNameID;
            }
            return aString;
        }

        private string GetArticleNameID()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            Articles arts = da.GetAllActiveArticles();
            string aString = string.Empty;
            if (arts != null)
            {
                Article art = arts.GetById(Article_idJensen);
                if (art != null)
                    aString = art.ArticleNameID;
            }
            return aString;
        }

        private int SetOperatorName(string aName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int id = -1;
            Operators ops = da.GetAllActiveOperators();
            if (ops != null)
            {
                Operator op = ops.GetByNameID(aName);
                id = op.idJensen;
            }
            return id;
        }

        private int SetCustomerName(string aName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int id = -1;
            Customers custs = da.GetAllActiveCustomers();
            if (custs != null)
            {
                Customer cust = custs.GetByNameID(aName);
                id = cust.idJensen;
            }
            return id;
        }

        private short SetArticleName(string aName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            short id = -1;
            Articles arts = da.GetAllActiveArticles();
            if (arts != null)
            {
                Article art = arts.GetByNameID(aName);
                id = art.idJensen;
            }
            return id;
        }

        public string OperatorNameID
        {
            get
            {
                return GetOperatorNameID();
            }
            set
            {
                int id = SetOperatorName(value);
                if (Operator_idJensen != id)
                {
                    if (Operator_idJensen != id)
                    {
                        Operator_idJensen = id;
                        NotifyPropertyChanged("OperatorNameID");
                    }
                }
            }
        }

        public string CustomerNameID
        {
            get
            {
                return GetCustomerNameID();
            }
            set
            {
                int id = SetCustomerName(value);
                if (Customer_idJensen != id)
                {
                    if (Customer_idJensen != id)
                    {
                        Customer_idJensen = id;
                        NotifyPropertyChanged("CustomerNameID");
                    }
                }
            }
        }

        public string ArticleNameID
        {
            get
            {
                return GetArticleNameID();
            }
            set
            {
                short id = SetArticleName(value);
                if (Article_idJensen != id)
                {
                    if (Article_idJensen != id)
                    {
                        Article_idJensen = id;
                        NotifyPropertyChanged("ArticleNameID");
                    }
                }
            }
        }

        public int Operator_Remote
        {
            get
            {
                return this.activeData.Operator_Remote;
            }
            set
            {
                if (this.activeData.Operator_Remote != value)
                {
                    this.activeData.Operator_Remote = value;
                    NotifyPropertyChanged("Operator_Remote");
                }
            }
        }


        public int Customer_idJensen
        {
            get
            {
                return this.activeData.Customer_idJensen;
            }
            set
            {
                if (this.activeData.Customer_idJensen != value)
                {
                    this.activeData.Customer_idJensen = value;
                    NotifyPropertyChanged("Customer_idJensen");
                }
            }
        }

        public short Article_idJensen
        {
            get
            {
                return this.activeData.Article_idJensen;
            }
            set
            {
                if (this.activeData.Article_idJensen != value)
                {
                    this.activeData.Article_idJensen = value;
                    NotifyPropertyChanged("Article_idJensen");
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion


    }
    #endregion

    #region Machine Status Class
    public class MachineStatus : DataItem, IDataFillerSingle
    {
        #region Machine Status Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short Machine_idJensen;
            internal short Status;
            internal int PieceCount;
            internal int OnTime;
            internal int RunTime;
            internal DateTime UpdateTime;
            internal DateTime? LastMessage;
            internal DateTime? LastSuccessfulPing;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor of machine status class
        /// </summary>
        public MachineStatus()
        {
            ActiveData = (ICopyableObject)new DataRecord();
        }

        /// <summary>
        /// Machine Status with essential data
        /// </summary>
        /// <param name="Machine_idJensen">The jensen ID of the machine</param>
        /// <param name="Status">The status of the machine</param>
        /// <param name="pieceCount">The total number of pieces counted on this machine</param>
        /// <param name="onTime">Power up hours of the machine</param>
        /// <param name="runTime">The running hours of the machine</param>
        public MachineStatus(short machine_idJensen, short status, int pieceCount, int onTime, int runTime)
        {
            ActiveData = (ICopyableObject)new DataRecord();
            this.Machine_idJensen = machine_idJensen;
            this.Status = status;
            this.PieceCount = pieceCount;
            this.OnTime = onTime;
            this.RunTime = runTime;
        }

        /// <summary>
        /// Machine status class with core data
        /// </summary>
        /// <param name="Machine_idJensen">The jensen ID of the machine</param>
        /// <param name="Status">The status of the machine</param>
        /// <param name="pieceCount">The total number of pieces counted on this machine</param>
        /// <param name="onTime">Power up hours of the machine</param>
        /// <param name="runTime">The running hours of the machine</param>
        /// <param name="UpdateTime">the most recent update time for the machine</param>
        public MachineStatus(short machine_idJensen, short status, int pieceCount,
            int onTime, int runTime, DateTime updateTime)
            : this(machine_idJensen, status, pieceCount, onTime, runTime)
        {
            this.UpdateTime = updateTime;
        }

        /// <summary>
        /// Machine status with all data
        /// </summary>
        /// <param name="RecNum">The unique record number for the machine status entry</param>
        /// <param name="Machine_idJensen">The jensen ID of the machine</param>
        /// <param name="Status">The status of the machine</param>
        /// <param name="pieceCount">The total number of pieces counted on this machine</param>
        /// <param name="onTime">Power up hours of the machine</param>
        /// <param name="runTime">The running hours of the machine</param>
        /// <param name="UpdateTime">the most recent update time for the machine</param>
        public MachineStatus(int recNum, short machine_idJensen, short status, int pieceCount,
            int onTime, int runTime, DateTime updateTime)
            : this(machine_idJensen, status, pieceCount, onTime, runTime, updateTime)
        {
            this.RecNum = recNum;
        }

        /// <summary>
        /// Machine status with all data - Post Version 1.99
        /// </summary>
        /// <param name="RecNum">The unique record number for the machine status entry</param>
        /// <param name="Machine_idJensen">The jensen ID of the machine</param>
        /// <param name="Status">The status of the machine</param>
        /// <param name="pieceCount">The total number of pieces counted on this machine</param>
        /// <param name="onTime">Power up hours of the machine</param>
        /// <param name="runTime">The running hours of the machine</param>
        /// <param name="UpdateTime">the most recent update time for the machine</param>
        /// <param name="LastSuccessfulPing">the most recent time the machine responded to a ping</param>
        public MachineStatus(int recNum, short machine_idJensen, short status, int pieceCount,
            int onTime, int runTime, DateTime updateTime, DateTime lastPing)
            : this(machine_idJensen, status, pieceCount, onTime, runTime, updateTime)
        {
            this.LastSuccessfulPing = lastPing;
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

        public short Machine_idJensen
        {
            get
            {
                return this.activeData.Machine_idJensen;
            }
            set
            {
                this.activeData.Machine_idJensen = value;
                HasChanged = true;
            }
        }

        public short Status
        {
            get
            {
                return this.activeData.Status;
            }
            set
            {
                this.activeData.Status = value;
                HasChanged = true;
            }
        }

        public int PieceCount
        {
            get
            {
                return this.activeData.PieceCount;
            }
            set
            {
                this.activeData.PieceCount = value;
                HasChanged = true;
            }
        }

        public int OnTime
        {
            get
            {
                return this.activeData.OnTime;
            }
            set
            {
                this.activeData.OnTime = value;
                HasChanged = true;
            }
        }

        public int RunTime
        {
            get
            {
                return this.activeData.RunTime;
            }
            set
            {
                this.activeData.RunTime = value;
                HasChanged = true;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return this.activeData.UpdateTime;
            }
            set
            {
                this.activeData.UpdateTime = value;
                HasChanged = true;
            }
        }

        public DateTime? LastMessage
        {
            get
            {
                return this.activeData.LastMessage;
            }
            set
            {
                this.activeData.LastMessage = value;
                HasChanged = true;
            }
        }

        public DateTime? LastSuccessfulPing
        {
            get
            {
                return this.activeData.LastSuccessfulPing;
            }
            set
            {
                this.activeData.LastSuccessfulPing = value;
                HasChanged = true;
            }
        }
        #endregion

        #region Field Positions
        int RecNumPos;
        int Machine_idJensenPos;
        int StatusPos;
        int PieceCountPos;
        int OnTimePos;
        int RunTimePos;
        int UpdateTimePos;
        int LastMessagePos;
        int LastSuccessfulPingPos;

        public void SetFieldPositions(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;

            RecNumPos = dr.GetOrdinal("RecNum");
            Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            StatusPos = dr.GetOrdinal("Status");
            PieceCountPos = dr.GetOrdinal("PieceCount");
            OnTimePos = dr.GetOrdinal("OnTime");
            RunTimePos = dr.GetOrdinal("RunTime");
            UpdateTimePos = dr.GetOrdinal("UpdateTime");
            LastMessagePos = dr.GetOrdinal("LastMessage");

            if (da.DatabaseVersion >= 1.99)
            {
                LastSuccessfulPingPos = dr.GetOrdinal("LastSuccessfulPing");
            }
        }
        #endregion

        public static MachineStatus First(SqlCommand command, SqlDataConnection.DBConnection dbConnection)
        {
            MachineStatus status = new MachineStatus();
            bool hasRead = command.DataFillSingle(status, dbConnection);

            if (hasRead)
            {
                return status;
            }
            else
            {
                return null;
            }
        }

        public int FillSingle(SqlDataReader dr)
        {
            if (dr.HasRows)
            {
                dr.Read();
                SetFieldPositions(dr);
                FillMachineStatus(dr);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void FillMachineStatus(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;

            RecNum = dr.GetInt32(RecNumPos);
            Machine_idJensen = dr.GetInt16(Machine_idJensenPos);
            Status = dr.GetInt16(StatusPos);
            PieceCount = dr.GetInt32(PieceCountPos);
            OnTime = dr.GetInt32(OnTimePos);
            RunTime = dr.GetInt32(RunTimePos);
            UpdateTime = dr.GetDateTime(UpdateTimePos);
            LastMessage = dr.IsDBNull(LastMessagePos) ? null : (DateTime?)dr.GetDateTime(LastMessagePos);

            if (da.DatabaseVersion >= 1.99)
            {
                LastSuccessfulPing = dr.IsDBNull(LastSuccessfulPingPos) ? null : (DateTime?)dr.GetDateTime(LastSuccessfulPingPos);
            }
        }
    }
    #endregion

    #region machine alias class
    public class MachineLogAlias : DataItem
    {
        private int aliasID;
        private string description;
        private int sourceID;
        private string subIDList;
        private int destID;
        private int sourceIndex;        
        private int conversionType;        
        private string switches;
        private bool active;
        private int remoteID;
        private bool deleteRecord = false;


        public int AliasID
        {
            get { return aliasID; }
            set { aliasID = AssignNotify(aliasID, value, "AliasID"); }
        }

        public override int ID
        {
            get { return aliasID; }
            set { aliasID = AssignNotify(aliasID, value, "ID"); }
        }

        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(sourceID, value, "SourceID"); }
        }

        public string Description
        {
            get { return description; }
            set { description = AssignNotify(description, value, "Description"); }
        }

        public string SubIDList
        {
            get { return subIDList; }
            set { subIDList = AssignNotify(subIDList, value, "SubIDList"); }
        }

        public int DestID
        {
            get { return destID; }
            set { destID = AssignNotify(destID, value, "DestID"); }
        }

        public int SourceIndex
        {
            get { return sourceIndex; }
            set { sourceIndex = AssignNotify(sourceIndex, value, "SourceIndex"); }
        }

        public int ConversionType
        {
            get { return conversionType; }
            set { conversionType = AssignNotify(conversionType, value, "ConversionType"); }
        }

        public string Switches
        {
            get { return switches; }
            set { switches = AssignNotify(switches, value, "Switches"); }
        }

        public bool Active
        {
            get { return active; }
            set { active = AssignNotify(active, value, "Active"); }
        }

        public int RemoteID
        {
            get { return remoteID; }
            set { remoteID = AssignNotify(remoteID, value, "RemoteID"); }
        }

        public bool DeleteRecord
        {
            get { return deleteRecord; }
            set { deleteRecord = AssignNotify(deleteRecord, value, "DeleteRecord"); }
        }
   }
    #endregion

    #region Machine Status Details (public)
    public class MachineStatusDetail : DataItem
    {
        #region Machine Status Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int Start_RemoteID;
            internal int MachineID;
            internal int MachineState;
            internal DateTime Start_TimeStamp;
            internal DateTime? End_TimeStamp;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor of machine status class
        /// </summary>
        public MachineStatusDetail()
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

        public int Start_RemoteID
        {
            get
            {
                return this.activeData.Start_RemoteID;
            }
            set
            {
                this.activeData.Start_RemoteID = value;
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
                HasChanged = true;
            }
        }

        public int MachineState
        {
            get
            {
                return this.activeData.MachineState;
            }
            set
            {
                this.activeData.MachineState = value;
                HasChanged = true;
            }
        }

        public DateTime Start_TimeStamp
        {
            get
            {
                return this.activeData.Start_TimeStamp;
            }
            set
            {
                this.activeData.Start_TimeStamp = value;
                HasChanged = true;
            }
        }

        public DateTime? End_TimeStamp
        {
            get
            {
                return this.activeData.End_TimeStamp;
            }
            set
            {
                this.activeData.End_TimeStamp = value;
                HasChanged = true;
            }
        }


        #endregion


    }
    #endregion


    
    #endregion
}
