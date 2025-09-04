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


        public JGLogData GetJGLogReportData(DateTime startTime, DateTime endTime)
        {
            return GetJGLogReportData(startTime, endTime, string.Empty);
        }

        public JGLogData GetJGLogReportData(DateTime startTime, DateTime endTime, int machineID)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld
              WHERE ld.MachineID = @MachineID
                AND [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime

                AND ( -- type of log
	                    (ld.RegType = 0 AND ld.SubRegType = 0) --status
	                OR  (ld.RegType = 1 AND ld.SubRegType = 0) --production
	                OR  (ld.RegType = 1 AND ld.SubRegType in (2,4,6,9,10)) --reject count
	                OR  (ld.RegType = 1 AND ld.SubRegType = 3) --rewash
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 0) --steam usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 10) --air usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 20) --electricity usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 30 AND ld.SubRegTypeID = 77) --fresh water usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 40) --gas usage 
                    OR  (ld.RegType = 0 AND ld.SubRegType = 5) --CAR change
                    OR  (ld.RegType = 0 AND ld.SubRegType = 3) --Power
                    OR  (ld.RegType = 0 AND ld.SubRegType = 2) --Flow/No Flow
                    OR  (ld.RegType = 6 AND ld.SubRegType = 0) --Operator login/out
                    )
        
                     ORDER BY MachineID, RemoteID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.Parameters.AddWithValue("@MachineID", machineID);
                    SqlDataConnection.Timeout = 600;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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

        public JGLogData GetJGLogProductionReportData(DateTime startTime, DateTime endTime, string machines)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  --,ld.[RemoteID]
                  --,ld.[CompanyID]
                  ,DATEADD(Hour, DATEDIFF(Hour, 0, GETDATE()), 0) AS DateHour
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld
              WHERE ld.MachineID in (" + machines + @")
                AND [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime

                AND (ld.RegType = 1) --production

        
                     ORDER BY MachineID, RemoteID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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

        public JGLogData GetJGLogEntryExitData(int batchId, int state, string machines)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld
              WHERE 
                

                ld.RegType = 4
                AND (ld.SubRegType = 0)
                AND ld.State = @state
                AND ld.BatchId = @batchId
                AND ld.MachineID in (" + machines + @")
                    
              ORDER BY [TimeStamp] ASC       ";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@state", state);
                    command.Parameters.AddWithValue("@batchId", batchId);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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
        public JGLogData GetJGLogReportData(DateTime startTime, DateTime endTime, string logTableName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld, [dbo].[tblMachines] ms
              WHERE ld.MachineID = ms.idJensen
                AND [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime

                --AND MachineID in (8104)

                AND ( -- type of log
	                    (ld.RegType = 0 AND ld.SubRegType = 0) --status
                    OR  (ld.RegType = 1 AND ld.SubRegType = 0) --production
                    OR  (ld.RegType = 1 AND ld.SubRegType in (2,4,6,9,10)) --reject count
                    OR  (ld.RegType = 1 AND ld.SubRegType = 3) --rewash
                    OR  (ld.RegType = 2 AND ld.SubRegType = 0) --steam usage
                    OR  (ld.RegType = 2 AND ld.SubRegType = 10) --air usage
                    OR  (ld.RegType = 2 AND ld.SubRegType = 20) --electricity usage
                    OR  (ld.RegType = 2 AND ld.SubRegType = 30 AND ld.SubRegTypeID = 77) --fresh water usage
                    OR  (ld.RegType = 2 AND ld.SubRegType = 30 AND ld.SubRegTypeID = 900) --waste water usage
                    OR  (ld.RegType = 2 AND ld.SubRegType = 40) --gas usage 
                    OR  (ld.RegType = 0 AND ld.SubRegType = 5) --CAR change
                    OR  (ld.RegType = 0 AND ld.SubRegType = 3) --Power
                    OR  (ld.RegType = 0 AND ld.SubRegType = 2) --Flow/No Flow
                    OR  (ld.RegType = 6 AND ld.SubRegType = 0) --Operator login/out
                    ) " + Environment.NewLine;

            if (da.DatabaseVersion < 1.97)
            {
                commandString += @" AND -- count exit point ";
                if (da.DatabaseVersion >= 1.4)
                    commandString +=
                    @" 
                        (((ms.UseMachineCount=1) AND (ms.MachineCountExitPoint=1)) --machine cxp
                    OR (ms.UseMachineCount=0) -- maybe pc cxp
                    OR (ms.OperatorCountExitPoint=1) -- Operator cxp
                    OR (ms.ExcludeFromPowerCalc=0)) --include power calcs
                    ";
                else
                    commandString +=
                    @" (((ms.UseMachineCount=1) AND (ms.MachineCountExitPoint=1)) --machine cxp
                    OR (ms.UseMachineCount=0)) -- maybe pc cxp";
            }
            commandString += @" 
                    ORDER BY MachineID, RemoteID";//nb sorted post query in collection!
            try
            {
                if (logTableName != string.Empty)
                {
                    commandString = commandString.Replace("tblJGLogData", logTableName);
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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

        public JGLogData GetJGLogAliasData(DateTime startTime, DateTime endTime, int machineID, int remoteID)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString = @"SELECT [recNum]
                                              ,[RemoteID]
                                              ,[CompanyID]
                                              ,[TimeStamp]
                                              ,[MachineID]
                                              ,[PositionID]
                                              ,[SubID]
                                              ,[SubIDName]
                                              ,[RegType]
                                              ,[SubRegType]
                                              ,[SubRegTypeID]
                                              ,[State]
                                              ,[MessageA]
                                              ,[MessageB]
                                              ,[BatchID]
                                              ,[SourceID]
                                              ,[ProcessCode]
                                              ,[ProcessName]
                                              ,[CustNo]
                                              ,[SortCategoryID]
                                              ,[ArtNo]
                                              ,[OperatorNo]
                                              ,[Value]
                                              ,[Unit]
                                          FROM [JEGR_DB].[dbo].[tblJGLogData]
                                                      WHERE MachineID = @MachineID
                                                        AND RemoteID > @RemoteID
                                                        AND [TimeStamp]>=@StartTime
                                                        AND [TimeStamp]<@EndTime
                                                         ORDER BY MachineID, RemoteID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.Parameters.AddWithValue("@MachineID", machineID);
                    command.Parameters.AddWithValue("@RemoteID", remoteID);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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

        public JGLogData GetJGLogKPIData(DateTime startTime, DateTime endTime, string logTableName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld, [dbo].[tblMachines] ms
              WHERE ld.MachineID = ms.idJensen
                AND [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime

                --AND machineid=2113

                AND ( -- type of log
                        (ld.RegType = 0 AND ld.SubRegType = 0) --status
	                OR  (ld.RegType = 1 AND ld.SubRegType = 0) --production
	                OR  (ld.RegType = 1 AND ld.SubRegType in (2,4,6,9,10)) --reject count
	                OR  (ld.RegType = 1 AND ld.SubRegType = 3) --rewash
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 0) --steam usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 10) --air usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 20) --electricity usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 30 AND ld.SubRegTypeID = 77) --fresh water usage
 	                OR  (ld.RegType = 2 AND ld.SubRegType = 40) --gas usage 
                    OR  (ld.RegType = 6 AND ld.SubRegType = 0) --Operator login/out
                    )
                AND -- count exit point
                    ";
            if (da.DatabaseVersion >= 1.4)
                commandString +=
                @" (((ms.UseMachineCount=1) AND (ms.MachineCountExitPoint=1)) --machine cxp
                    OR (ms.UseMachineCount=0) -- maybe pc cxp
                    OR (ms.OperatorCountExitPoint=1) -- Operator cxp
                    OR (ms.ExcludeFromPowerCalc=0)) --include power calcs
                    ";
            else
                commandString +=
                @" (((ms.UseMachineCount=1) AND (ms.MachineCountExitPoint=1)) --machine cxp
                    OR (ms.UseMachineCount=0)) -- maybe pc cxp
                    ";
            commandString += " ORDER BY MachineID, RemoteID";
            try
            {
                if (logTableName != string.Empty)
                {
                    commandString = commandString.Replace("tblJGLogData", logTableName);
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    JGLogData JGLogData = new JGLogData();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(JGLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return JGLogData;
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
        public void InsertNewDataRec(JGLogDataRec logDataRec)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblJGLogData]
                   ([RemoteID]
                   ,[CompanyID]
                   ,[TimeStamp]
                   ,[MachineID]
                   ,[PositionID]
                   ,[SubID]
                   ,[SubIDName]
                   ,[RegType]
                   ,[SubRegType]
                   ,[SubRegTypeID]
                   ,[State]
                   ,[MessageA]
                   ,[MessageB]
                   ,[BatchID]
                   ,[SourceID]
                   ,[ProcessCode]
                   ,[ProcessName]
                   ,[CustNo]
                   ,[SortCategoryID]
                   ,[ArtNo]
                   ,[OperatorNo]
                   ,[Value]
                   ,[Unit])
             VALUES
                   (@RemoteID
                   ,@CompanyID
                   ,@TimeStamp
                   ,@MachineID
                   ,@PositionID
                   ,@SubID
                   ,@SubIDName 
                   ,@RegType
                   ,@SubRegType
                   ,@SubRegTypeID
                   ,@State
                   ,@MessageA 
                   ,@MessageB 
                   ,@BatchID
                   ,@SourceID
                   ,@ProcessCode
                   ,@ProcessName
                   ,@CustNo
                   ,@SortCategoryID
                   ,@ArtNo
                   ,@OperatorNo
                   ,@Value
                   ,@Unit
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RemoteID", logDataRec.RemoteID);
                    command.Parameters.AddWithValue("@CompanyID", logDataRec.CompanyID);
                    command.Parameters.AddWithValue("@TimeStamp", logDataRec.TimeStamp);
                    command.Parameters.AddWithValue("@MachineID", logDataRec.MachineID);
                    command.Parameters.AddWithValue("@PositionID", logDataRec.PositionID);
                    command.Parameters.AddWithValue("@SubID", logDataRec.SubID);
                    SqlParameter p = command.Parameters.Add("@SubIDName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.SubIDName == null ? DBNull.Value : (object)logDataRec.SubIDName;
                    command.Parameters.AddWithValue("@RegType", logDataRec.RegType);
                    command.Parameters.AddWithValue("@SubRegType", logDataRec.SubRegType);
                    command.Parameters.AddWithValue("@SubRegTypeID", logDataRec.SubRegTypeID);
                    command.Parameters.AddWithValue("@State", logDataRec.State);
                    p = command.Parameters.Add("@MessageA", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageA == null ? DBNull.Value : (object)logDataRec.MessageA;
                    p = command.Parameters.Add("@MessageB", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageB == null ? DBNull.Value : (object)logDataRec.MessageB;
                    command.Parameters.AddWithValue("@BatchID", logDataRec.BatchID);
                    command.Parameters.AddWithValue("@SourceID", logDataRec.SourceID);
                    command.Parameters.AddWithValue("@ProcessCode", logDataRec.ProcessCode);
                    p = command.Parameters.Add("@ProcessName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.ProcessName == null ? DBNull.Value : (object)logDataRec.ProcessName;
                    command.Parameters.AddWithValue("@CustNo", logDataRec.CustNo);
                    command.Parameters.AddWithValue("@SortCategoryID", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@ArtNo", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@OperatorNo", logDataRec.OperatorNo);
                    command.Parameters.AddWithValue("@Value", logDataRec.Value);
                    command.Parameters.AddWithValue("@Unit", logDataRec.Unit);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            logDataRec.RecNum = (int)RecNum;
                            logDataRec.HasChanged = false;
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

        public void InsertNewLogSP(JGLogDataRec logDataRec)
        {
            string commandString =
               @"EXECUTE [dbo].[spInsertLogData] 
                                   @RemoteID
                                  ,@CompanyID
                                  ,@TimeStamp
                                  ,@MachineID
                                  ,@PositionID
                                  ,@SubID
                                  ,@SubIDName
                                  ,@RegType
                                  ,@SubRegType
                                  ,@SubRegTypeID
                                  ,@State
                                  ,@MessageA
                                  ,@MessageB
                                  ,@BatchID
                                  ,@SourceID
                                  ,@ProcessCode
                                  ,@ProcessName
                                  ,@CustNo
                                  ,@SortCategoryID
                                  ,@ArtNo
                                  ,@OperatorNo
                                  ,@Value
                                  ,@Unit";
            if (DatabaseVersion>=1.99)
                commandString+=" ,@AllowDuplicateRemoteID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RemoteID", logDataRec.RemoteID);
                    command.Parameters.AddWithValue("@CompanyID", logDataRec.CompanyID);
                    SqlParameter p = command.Parameters.Add("@TimeStamp", SqlDbType.DateTime);
                    p.IsNullable = true;
                    p.Value = logDataRec.TimeStamp == DateTime.MinValue ? DBNull.Value : (object)logDataRec.TimeStamp;
                    //command.Parameters.AddWithValue("@TimeStamp", logDataRec.TimeStamp);
                    command.Parameters.AddWithValue("@MachineID", logDataRec.MachineID);
                    command.Parameters.AddWithValue("@PositionID", logDataRec.PositionID);
                    command.Parameters.AddWithValue("@SubID", logDataRec.SubID);
                    p = command.Parameters.Add("@SubIDName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.SubIDName == null ? DBNull.Value : (object)logDataRec.SubIDName;
                    command.Parameters.AddWithValue("@RegType", logDataRec.RegType);
                    command.Parameters.AddWithValue("@SubRegType", logDataRec.SubRegType);
                    command.Parameters.AddWithValue("@SubRegTypeID", logDataRec.SubRegTypeID);
                    command.Parameters.AddWithValue("@State", logDataRec.State);
                    p = command.Parameters.Add("@MessageA", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageA == null ? DBNull.Value : (object)logDataRec.MessageA;
                    p = command.Parameters.Add("@MessageB", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageB == null ? DBNull.Value : (object)logDataRec.MessageB;
                    command.Parameters.AddWithValue("@BatchID", logDataRec.BatchID);
                    command.Parameters.AddWithValue("@SourceID", logDataRec.SourceID);
                    command.Parameters.AddWithValue("@ProcessCode", logDataRec.ProcessCode);
                    p = command.Parameters.Add("@ProcessName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.ProcessName == null ? DBNull.Value : (object)logDataRec.ProcessName;
                    command.Parameters.AddWithValue("@CustNo", logDataRec.CustNo);
                    command.Parameters.AddWithValue("@SortCategoryID", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@ArtNo", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@OperatorNo", logDataRec.OperatorNo);
                    command.Parameters.AddWithValue("@Value", logDataRec.Value);
                    command.Parameters.AddWithValue("@Unit", logDataRec.Unit);
                    if (DatabaseVersion >= 1.99)
                        command.Parameters.AddWithValue("@AllowDuplicateRemoteID", 1);

                    try
                    {
                        command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                    }
                    catch (SqlException ex)
                    {
                        Debug.WriteLine(ex.Message);
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
        public void UpdateOperatorDetails(JGLogDataRec logDataRec)
        {
            const string commandString =
                @"UPDATE [dbo].[tblJGLogData]
                SET RemoteID=@RemoteID
                   ,CompanyID=@CompanyID
                   ,TimeStamp=@TimeStamp
                   ,MachineID=@MachineID
                   ,PositionID=@PositionID
                   ,SubID=@SubID
                   ,SubIDName=@SubIDName
                   ,RegType=@RegType
                   ,SubRegType=@SubRegType
                   ,SubRegTypeID=@SubRegTypeID
                   ,State=@State
                   ,MessageA=@MessageA
                   ,MessageB=@MessageB
                   ,BatchID=@BatchID
                   ,SourceID=@SourceID
                   ,ProcessCode=@ProcessCode
                   ,ProcessName=@ProcessName
                   ,CustNo=@CustNo
                   ,SortCategoryID=@SortCategoryID
                   ,ArtNo=@ArtNo
                   ,OperatorNo=@OperatorNo
                   ,Value=@Value
                   ,Unit=@Unit)                  
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RemoteID", logDataRec.RemoteID);
                    command.Parameters.AddWithValue("@CompanyID", logDataRec.CompanyID);
                    command.Parameters.AddWithValue("@TimeStamp", logDataRec.TimeStamp);
                    command.Parameters.AddWithValue("@MachineID", logDataRec.MachineID);
                    command.Parameters.AddWithValue("@PositionID", logDataRec.MachineID);
                    command.Parameters.AddWithValue("@SubID", logDataRec.SubID);
                    SqlParameter p = command.Parameters.Add("@SubIDName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.SubIDName == null ? DBNull.Value : (object)logDataRec.SubIDName;
                    command.Parameters.AddWithValue("@RegType", logDataRec.RegType);
                    command.Parameters.AddWithValue("@SubRegType", logDataRec.SubRegType);
                    command.Parameters.AddWithValue("@SubRegTypeID", logDataRec.SubRegTypeID);
                    command.Parameters.AddWithValue("@State", logDataRec.State);
                    p = command.Parameters.Add("@MessageA", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageA == null ? DBNull.Value : (object)logDataRec.MessageA;
                    p = command.Parameters.Add("@MessageB", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.MessageB == null ? DBNull.Value : (object)logDataRec.MessageB;
                    command.Parameters.AddWithValue("@BatchID", logDataRec.BatchID);
                    command.Parameters.AddWithValue("@SourceID", logDataRec.SourceID);
                    command.Parameters.AddWithValue("@ProcessCode", logDataRec.ProcessCode);
                    p = command.Parameters.Add("@ProcessName", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = logDataRec.ProcessName == null ? DBNull.Value : (object)logDataRec.ProcessName;
                    command.Parameters.AddWithValue("@CustNo", logDataRec.CustNo);
                    command.Parameters.AddWithValue("@SortCategoryID", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@ArtNo", logDataRec.ArtNo);
                    command.Parameters.AddWithValue("@OperatorNo", logDataRec.OperatorNo);
                    command.Parameters.AddWithValue("@Value", logDataRec.Value);
                    command.Parameters.AddWithValue("@Unit", logDataRec.Unit);
                    logDataRec.HasChanged = false;
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
        /// <summary>
        /// Delete a JGLogDataRec permanently from the database.
        /// This should not normally be used as the JGLogData are normally retired.
        /// </summary>
        /// <param name="oper"></param>
        public void DeleteOperatorDetails(JGLogDataRec oper)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblJGLogData]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", oper.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
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

    #region Data Collection Class
    public class JGLogData : List<JGLogDataRec>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int RemoteIDPos = dr.GetOrdinal("RemoteID");
            int CompanyIDPos = dr.GetOrdinal("CompanyID");
            int TimeStampPos = dr.GetOrdinal("TimeStamp");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int PositionIDPos = dr.GetOrdinal("PositionID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int SubIDNamePos = dr.GetOrdinal("SubIDName");
            int RegTypePos = dr.GetOrdinal("RegType");
            int SubRegTypePos = dr.GetOrdinal("SubRegType");
            int SubRegTypeIDPos = dr.GetOrdinal("SubRegTypeID");
            int StatePos = dr.GetOrdinal("State");
            int MessageAPos = dr.GetOrdinal("MessageA");
            int MessageBPos = dr.GetOrdinal("MessageB");
            int BatchIDPos = dr.GetOrdinal("BatchID");
            int SourceIDPos = dr.GetOrdinal("SourceID");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int ProcessNamePos = dr.GetOrdinal("ProcessName");
            int CustNoPos = dr.GetOrdinal("CustNo");
            int SortCategoryIDPos = dr.GetOrdinal("SortCategoryID");
            int ArtNoPos = dr.GetOrdinal("ArtNo");
            int OperatorNoPos = dr.GetOrdinal("OperatorNo");
            int ValuePos = dr.GetOrdinal("Value");
            int UnitPos = dr.GetOrdinal("Unit");

            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines(false);

            while (dr.Read())
            {
                JGLogDataRec logDataRec = new JGLogDataRec();
                try
                {
                    int subid = dr.GetInt32(SubIDPos);
                    int machineID = dr.GetInt32(MachineIDPos);
                    if (subid > 0)
                    {
                        int positions = 0;
                        Machine m = ms.GetById(machineID);
                        if (m != null)
                        {
                            positions = m.Positions;
                        }
                        if ((subid > positions) && (subid < 100))
                        {
                            subid = positions;
                        }
                    }
                    logDataRec.RecNum = dr.GetInt32(RecNumPos);
                    logDataRec.RemoteID = dr.GetInt32(RemoteIDPos);
                    logDataRec.CompanyID = dr.GetInt32(CompanyIDPos);
                    logDataRec.TimeStamp = dr.GetDateTime(TimeStampPos);
                    logDataRec.MachineID = machineID;
                    logDataRec.PositionID = dr.GetInt32(PositionIDPos);
                    logDataRec.SubID = subid; // dr.GetInt32(SubIDPos),
                    logDataRec.SubIDName = dr.IsDBNull(SubIDNamePos) ? string.Empty : dr.GetString(SubIDNamePos);
                    logDataRec.RegType = dr.GetInt32(RegTypePos);
                    logDataRec.SubRegType = dr.GetInt32(SubRegTypePos);
                    logDataRec.SubRegTypeID = dr.GetInt32(SubRegTypeIDPos);
                    logDataRec.State = dr.GetInt32(StatePos);
                    logDataRec.MessageA = dr.IsDBNull(MessageAPos) ? string.Empty : dr.GetString(MessageAPos);
                    logDataRec.MessageB = dr.IsDBNull(MessageBPos) ? string.Empty : dr.GetString(MessageBPos);
                    logDataRec.BatchID = dr.GetInt32(BatchIDPos);
                    logDataRec.SourceID = dr.GetInt32(SourceIDPos);
                    logDataRec.ProcessCode = dr.GetInt32(ProcessCodePos);
                    logDataRec.ProcessName = dr.IsDBNull(ProcessNamePos) ? string.Empty : dr.GetString(ProcessNamePos);
                    logDataRec.CustNo = dr.GetInt32(CustNoPos);
                    logDataRec.SortCategoryID = dr.IsDBNull(SortCategoryIDPos) ? -1 : dr.GetInt32(SortCategoryIDPos);
                    logDataRec.ArtNo = dr.GetInt32(ArtNoPos);
                    logDataRec.OperatorNo = dr.GetInt32(OperatorNoPos);
                    logDataRec.Value = dr.GetDecimal(ValuePos);
                    logDataRec.Unit = dr.GetInt32(UnitPos);
                    logDataRec.HasChanged = false;

                    this.Add(logDataRec);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return this.Count;
        }

        public JGLogData GetByMachine(int machineID)
        {
            JGLogData ldm = new JGLogData();
            for (int i = 0; i < this.Count; i++)
            {
                JGLogDataRec rec = this[i];

                if (rec.MachineID == machineID)
                {
                    ldm.Add(rec);
                }
            }
            ldm.Sort();
            //ldm.Sort((x, y) =>
            //{
            //    int result = x.MachineID.CompareTo(y.MachineID);
            //    //if (result == 0)
            //    //{
            //    //    result = x.SubID.CompareTo(y.SubID);
            //    //}
            //    if (result == 0)
            //    {
            //        result = x.TimeStamp.CompareTo(y.TimeStamp);
            //    }
            //    if (result == 0)
            //    {
            //        result = x.RemoteID.CompareTo(y.RemoteID);
            //    }
            //    return result;
            //}); 
            return ldm;
        }

        public DateTime GetLastLogTime(int machineid)
        {
            DateTime max = DateTime.MinValue;
            foreach (JGLogDataRec rec in this)
            {
                if ((rec.MachineID == machineid) && (rec.TimeStamp > max))
                    max = rec.TimeStamp;
            }
            return max;
        }
    }
    #endregion

    #region Item Class
    public class JGLogDataRec : DataItem, IComparable<JGLogDataRec>
    {
        #region JGLogDataRec Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int RemoteID;
            internal int CompanyID;
            internal DateTime TimeStamp;
            internal int MachineID;
            internal int PositionID;
            internal int SubID;
            internal string SubIDName;
            internal int RegType;
            internal int SubRegType;
            internal int SubRegTypeID;
            internal int State;
            internal string MessageA;
            internal string MessageB;
            internal int BatchID;
            internal int SourceID;
            internal int ProcessCode;
            internal string ProcessName;
            internal int CustNo;
            internal int SortCategoryID;
            internal int ArtNo;
            internal int OperatorNo;
            internal decimal Value;
            internal int Unit;


            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public JGLogDataRec()
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

        #region properties

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

        public int RemoteID
        {
            get
            {
                return this.activeData.RemoteID;
            }
            set
            {
                this.activeData.RemoteID = value;
                HasChanged = true;
            }
        }

        public int CompanyID
        {
            get
            {
                return this.activeData.CompanyID;
            }
            set
            {
                this.activeData.CompanyID = value;
                HasChanged = true;
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
                this.activeData.TimeStamp = value;
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

        public int PositionID
        {
            get
            {
                return this.activeData.PositionID;
            }
            set
            {
                this.activeData.PositionID = value;
                HasChanged = true;
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
                this.activeData.SubID = value;
                HasChanged = true;
            }
        }
        public string SubIDName
        {
            get
            {
                return this.activeData.SubIDName;
            }
            set
            {
                this.activeData.SubIDName = value;
                HasChanged = true;
            }
        }
        public int RegType
        {
            get
            {
                return this.activeData.RegType;
            }
            set
            {
                this.activeData.RegType = value;
                HasChanged = true;
            }
        }
        public int SubRegType
        {
            get
            {
                return this.activeData.SubRegType;
            }
            set
            {
                this.activeData.SubRegType = value;
                HasChanged = true;
            }
        }
        public int SubRegTypeID
        {
            get
            {
                return this.activeData.SubRegTypeID;
            }
            set
            {
                this.activeData.SubRegTypeID = value;
                HasChanged = true;
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
                this.activeData.State = value;
                HasChanged = true;
            }
        }
        public string MessageA
        {
            get
            {
                return this.activeData.MessageA;
            }
            set
            {
                this.activeData.MessageA = value;
                HasChanged = true;
            }
        }
        public string MessageB
        {
            get
            {
                return this.activeData.MessageB;
            }
            set
            {
                this.activeData.MessageB = value;
                HasChanged = true;
            }
        }
        public int BatchID
        {
            get
            {
                return this.activeData.BatchID;
            }
            set
            {
                this.activeData.BatchID = value;
                HasChanged = true;
            }
        }
        public int SourceID
        {
            get
            {
                return this.activeData.SourceID;
            }
            set
            {
                this.activeData.SourceID = value;
                HasChanged = true;
            }
        }
        public int ProcessCode
        {
            get
            {
                return this.activeData.ProcessCode;
            }
            set
            {
                this.activeData.ProcessCode = value;
                HasChanged = true;
            }
        }
        public string ProcessName
        {
            get
            {
                return this.activeData.ProcessName;
            }
            set
            {
                this.activeData.ProcessName = value;
                HasChanged = true;
            }
        }
        public int CustNo
        {
            get
            {
                return this.activeData.CustNo;
            }
            set
            {
                this.activeData.CustNo = value;
                HasChanged = true;
            }
        }
        public int SortCategoryID
        {
            get
            {
                return this.activeData.SortCategoryID;
            }
            set
            {
                this.activeData.SortCategoryID = value;
                HasChanged = true;
            }
        }
        public int ArtNo
        {
            get
            {
                return this.activeData.ArtNo;
            }
            set
            {
                this.activeData.ArtNo = value;
                HasChanged = true;
            }
        }
        public int OperatorNo
        {
            get
            {
                return this.activeData.OperatorNo;
            }
            set
            {
                this.activeData.OperatorNo = value;
                HasChanged = true;
            }
        }
        public decimal Value
        {
            get
            {
                return this.activeData.Value;
            }
            set
            {
                this.activeData.Value = value;
                HasChanged = true;
            }
        }
        public int Unit
        {
            get
            {
                return this.activeData.Unit;
            }
            set
            {
                this.activeData.Unit = value;
                HasChanged = true;
            }
        }

        public bool IsCountExitPointData()
        {
            bool test = false;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines();
            Machine m = ms.GetById(MachineID);
            if (m.UseMachineCount)
            {
                test = m.MachineCountExitPoint;
            }
            else
            {
                ProcessCodes pcs = da.GetAllProcessCodes();
                ProcessCode pc = pcs.GetByIds(ProcessCode, CustNo, ArtNo, MachineID);
                if (pc != null)
                {
                    test = pc.Count_Exit_Point;
                }
            }
            return test;
        }

        #endregion

        #region sort
        public int CompareTo(JGLogDataRec rec)
        {
            int ct = MachineID.CompareTo(rec.MachineID);
            if (ct == 0)
                ct = TimeStamp.CompareTo(rec.TimeStamp);
            if (ct == 0)
                ct = RemoteID.CompareTo(rec.RemoteID);
            if (ct == 0)
                ct = SubID.CompareTo(rec.SubID);
            if (ct == 0)
                ct = rec.SubRegTypeID.CompareTo(SubRegTypeID);
            return ct;
        }

        #endregion

        #region copy

        public JGLogDataRec Copy()
        {
            JGLogDataRec ldRec = (JGLogDataRec)MemberwiseClone();
            return ldRec;
        }

        #endregion
    }
    #endregion
}
