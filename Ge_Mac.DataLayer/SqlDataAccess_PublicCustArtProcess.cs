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

        public CustArtProcess GetCustArtProcesses(DateTime startTime, DateTime endTime)
        {
            return GetCustArtProcesses(startTime, endTime, string.Empty);
        }

        public CustArtProcess GetConsumption(DateTime startTime, DateTime endTime, string Conditions)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string tblName = "tblcustartprocess";
            if (da.CombinedProduction)
                tblName = "viewConsumption";
            string commandString = @"SELECT [RecNum]
                            ,[DateHour]
                            ,[MachineID]
                            ,[Machine_ExtRef]
                            ,[Machine_LongDescription]
                            ,[Machine_NormValue]
                            ,[Machine_Positions]
                            ,[SubID]
                            ,[MachineGroup_ExtRef]
                            ,[MachineArea]
                            ,[MachineGroup_LongDescription]
                            ,[CountExitPointData] 
	                        ,[ExcludePower]
	                        ,[ProductionRefData] 
                            ,[Air_Consumption]
                            ,[CustomerID]
                            ,[Customer_ExtRef]
                            ,[Customer_LongDescription]
                            ,[SortCategoryID]
                            ,[SortCategory_ExtRef]
                            ,[SortCategory_LongDescription]
                            ,[ArticleID]
                            ,[Article_ExtRef]
                            ,[Article_LongDescription]
                            ,[ProcessCode]
                            ,[ProcessName]
                            ,[Pieces_Counter]
                            ,[Weight_Counter]
                            ,[Batch_Counter]
                            ,[Production_Time]
                            ,[Stop_Time]
                            ,[NoFlow_Time]
                            ,[Fault_Time]
                            ,[Water_Consumption]
                            ,[Gas_Consumption]
                            ,[Steam_Consumption]
                            ,[Electricity_Consumption]
                            ,[Reject_Counter]
                            ,[Rewash_Counter]
                        FROM [dbo]." + tblName
                            + @" WHERE [DateHour]>=@StartTime
                            AND [DateHour]<@EndTime
                        " + Conditions;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    CustArtProcess cap = new CustArtProcess();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.DataFill(cap, SqlDataConnection.DBConnection.JensenPublic);
                    return cap;
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

        public CustArtProcess GetAllCustArtProcess(DateTime startTime, DateTime endTime, string Conditions)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string tblName = "tblcustartprocess";
            if (da.CombinedProduction)
                tblName = "viewAllCustArtProcess";
            string commandString = @"SELECT [RecNum]
                            ,[DateHour]
                            ,[MachineID]
                            ,[Machine_ExtRef]
                            ,[Machine_LongDescription]
                            ,[Machine_NormValue]
                            ,[Machine_Positions]
                            ,[SubID]
                            ,[MachineGroup_ExtRef]
                            ,[MachineArea]
                            ,[MachineGroup_LongDescription]
                            ,[CountExitPointData] 
	                        ,[ExcludePower]
	                        ,[ProductionRefData] 
                            ,[Air_Consumption]
                            ,[CustomerID]
                            ,[Customer_ExtRef]
                            ,[Customer_LongDescription]
                            ,[SortCategoryID]
                            ,[SortCategory_ExtRef]
                            ,[SortCategory_LongDescription]
                            ,[ArticleID]
                            ,[Article_ExtRef]
                            ,[Article_LongDescription]
                            ,[ProcessCode]
                            ,[ProcessName]
                            ,[Pieces_Counter]
                            ,[Weight_Counter]
                            ,[Batch_Counter]
                            ,[Production_Time]
                            ,[Stop_Time]
                            ,[NoFlow_Time]
                            ,[Fault_Time]
                            ,[Water_Consumption]
                            ,[Gas_Consumption]
                            ,[Steam_Consumption]
                            ,[Electricity_Consumption]
                            ,[Reject_Counter]
                            ,[Rewash_Counter]
                        FROM [dbo]." + tblName
                            + @" WHERE [DateHour]>=@StartTime
                            AND [DateHour]<@EndTime
                        " + Conditions;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    CustArtProcess cap = new CustArtProcess();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.DataFill(cap, SqlDataConnection.DBConnection.JensenPublic);
                    return cap;
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

        public ProductionRecs GetProductionRecs(DateTime startTime, DateTime endTime, string Conditions)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString = @"SELECT [RecNum]
                      ,[DateHour]
                      ,[SiteID]
                      ,[MachineID]
                      ,[MachineExtRef]
                      ,[MachineLongDescription]
                      ,[MachineNorm]
                      ,[MachinePositions]
                      ,[SubID]
                      ,[MachineGroupID]
                      ,[MachineGroupExtRef]
                      ,[MachineArea]
                      ,[MachineGroupLongDescription]
                      ,[CustomerID]
                      ,[CustomerExtRef]
                      ,[CustomerLongDescription]
                      ,[SortCategoryID]
                      ,[SortCategoryExtRef]
                      ,[SortCategoryLongDescription]
                      ,[ArticleID]
                      ,[ArticleExtRef]
                      ,[ArticleLongDescription]
                      ,[OperatorID]
                      ,[OperatorExtRef]
                      ,[OperatorLongDescription]
                      ,[ProcessCode]
                      ,[ProcessName]
                      ,[ProcessNorm]
                      ,[PiecesCounter]
                      ,[RejectCounter]
                      ,[RewashCounter]
                      ,[BatchCounter]
                      ,[WeightCounter]
                      ,[WeightUnits]
                      ,[ProductionTime]
                      ,[StopTime]
                      ,[NoFlowTime]
                      ,[FaultTime]
                      ,[FreshWater]
                      ,[WasteWater]
                      ,[WaterUnits]
                      ,[GasEnergy]
                      ,[GasEnergyUnits]
                      ,[GasVolume]
                      ,[GasVolumeUnits]
                      ,[ElectricalEnergy]
                      ,[ElectricityUnits]
                      ,[SteamConsumption]
                      ,[SteamUnits]
                      ,[AirConsumption]
                      ,[AirUnits]
                      ,[CountExitPointData]
                      ,[MachineCountExitPoint]
                      ,[OperatorCountExitPoint]
                      ,[ProcessCountExitPoint]
                      ,[ProductionRefData]
                      ,[ExcludePower]
                  FROM [tblProduction]
                  WHERE [DateHour]>=@StartTime
                        AND [DateHour]<@EndTime
--and machineid=8104
                        " + Conditions;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    ProductionRecs prods = new ProductionRecs();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.DataFill(prods, SqlDataConnection.DBConnection.JensenPublic);
                    return prods;
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

        public CustArtProcess GetCustArtProcesses(DateTime startTime, DateTime endTime, string Conditions)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string tblName = "tblcustartprocess";
            if (da.CombinedProduction)
                tblName = "viewCustArtProcess";
            string commandString =
                @"SELECT [RecNum]
                            ,[DateHour]
                            ,[MachineID]
                            ,[Machine_ExtRef]
                            ,[Machine_LongDescription]
                            ,[Machine_NormValue]
                            ,[Machine_Positions]
                            ,[SubID]
                            ,[MachineGroup_ExtRef]
                            ,[MachineArea]
                            ,[MachineGroup_LongDescription]";
            if (da.DatabaseVersion >= 1.4)
            {
                commandString += @"
                        ,[CountExitPointData] 
	                    ,[ExcludePower]
	                    ,[ProductionRefData] 
                        ,[Air_Consumption]";
            }
            commandString += @"
                   
                            ,[CustomerID]
                            ,[Customer_ExtRef]
                            ,[Customer_LongDescription]
                            ,[SortCategoryID]
                            ,[SortCategory_ExtRef]
                            ,[SortCategory_LongDescription]
                            ,[ArticleID]
                            ,[Article_ExtRef]
                            ,[Article_LongDescription]
                            ,[ProcessCode]
                            ,[ProcessName]
                            ,[Pieces_Counter]
                            ,[Weight_Counter]
                            ,[Batch_Counter]
                            ,[Production_Time]
                            ,[Stop_Time]
                            ,[NoFlow_Time]
                            ,[Fault_Time]
                            ,[Water_Consumption]
                            ,[Gas_Consumption]
                            ,[Steam_Consumption]
                            ,[Electricity_Consumption]
                            ,[Reject_Counter]
                            ,[Rewash_Counter]
                        FROM [dbo]." + tblName
                            + @" WHERE [DateHour]>=@StartTime
                            AND [DateHour]<@EndTime
                        " + Conditions;         
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    CustArtProcess cap = new CustArtProcess();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.DataFill(cap, SqlDataConnection.DBConnection.JensenPublic);
                    return cap;
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

        public void InsertNewDataRec(ProductionRec prod)
        {
            string commandString = @"INSERT INTO [dbo].[tblProduction]
                                                           ([DateHour]
                                                           ,[SiteID]
                                                           ,[MachineID]
                                                           ,[MachineExtRef]
                                                           ,[MachineLongDescription]
                                                           ,[MachineNorm]
                                                           ,[MachinePositions]
                                                           ,[SubID]
                                                           ,[MachineGroupID]
                                                           ,[MachineGroupExtRef]
                                                           ,[MachineArea]
                                                           ,[MachineGroupLongDescription]
                                                           ,[CustomerID]
                                                           ,[CustomerExtRef]
                                                           ,[CustomerLongDescription]
                                                           ,[SortCategoryID]
                                                           ,[SortCategoryExtRef]
                                                           ,[SortCategoryLongDescription]
                                                           ,[ArticleID]
                                                           ,[ArticleExtRef]
                                                           ,[ArticleLongDescription]
                                                           ,[OperatorID]
                                                           ,[OperatorExtRef]
                                                           ,[OperatorLongDescription]
                                                           ,[ProcessCode]
                                                           ,[ProcessName]
                                                           ,[ProcessNorm]
                                                           ,[PiecesCounter]
                                                           ,[RejectCounter]
                                                           ,[RewashCounter]
                                                           ,[BatchCounter]
                                                           ,[WeightCounter]
                                                           ,[WeightUnits]
                                                           ,[ProductionTime]
                                                           ,[StopTime]
                                                           ,[NoFlowTime]
                                                           ,[FaultTime]
                                                           ,[FreshWater]
                                                           ,[WasteWater]
                                                           ,[WaterUnits]
                                                           ,[GasEnergy]
                                                           ,[GasEnergyUnits]
                                                           ,[GasVolume]
                                                           ,[GasVolumeUnits]
                                                           ,[ElectricalEnergy]
                                                           ,[ElectricityUnits]
                                                           ,[SteamConsumption]
                                                           ,[SteamUnits]
                                                           ,[AirConsumption]
                                                           ,[AirUnits]
                                                           ,[CountExitPointData]
                                                           ,[MachineCountExitPoint]
                                                           ,[OperatorCountExitPoint]
                                                           ,[ProcessCountExitPoint]
                                                           ,[ProductionRefData]
                                                           ,[ExcludePower])
                                                     VALUES
                                                           (@DateHour
                                                           ,@SiteID
                                                           ,@MachineID
                                                           ,@MachineExtRef
                                                           ,@MachineLongDescription
                                                           ,@MachineNorm
                                                           ,@MachinePositions
                                                           ,@SubID
                                                           ,@MachineGroupID
                                                           ,@MachineGroupExtRef
                                                           ,@MachineArea
                                                           ,@MachineGroupLongDescription
                                                           ,@CustomerID
                                                           ,@CustomerExtRef
                                                           ,@CustomerLongDescription
                                                           ,@SortCategoryID
                                                           ,@SortCategoryExtRef
                                                           ,@SortCategoryLongDescription
                                                           ,@ArticleID
                                                           ,@ArticleExtRef
                                                           ,@ArticleLongDescription
                                                           ,@OperatorID
                                                           ,@OperatorExtRef
                                                           ,@OperatorLongDescription
                                                           ,@ProcessCode
                                                           ,@ProcessName
                                                           ,@ProcessNorm
                                                           ,@PiecesCounter
                                                           ,@RejectCounter
                                                           ,@RewashCounter
                                                           ,@BatchCounter
                                                           ,@WeightCounter
                                                           ,@WeightUnits
                                                           ,@ProductionTime
                                                           ,@StopTime
                                                           ,@NoFlowTime
                                                           ,@FaultTime
                                                           ,@FreshWater
                                                           ,@WasteWater
                                                           ,@WaterUnits
                                                           ,@GasEnergy
                                                           ,@GasEnergyUnits
                                                           ,@GasVolume
                                                           ,@GasVolumeUnits
                                                           ,@ElectricalEnergy
                                                           ,@ElectricityUnits
                                                           ,@SteamConsumption
                                                           ,@SteamUnits
                                                           ,@AirConsumption
                                                           ,@AirUnits
                                                           ,@CountExitPointData
                                                           ,@MachineCountExitPoint
                                                           ,@OperatorCountExitPoint
                                                           ,@ProcessCountExitPoint
                                                           ,@ProductionRefData
                                                           ,@ExcludePower)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", prod.DateHour);
                    command.Parameters.AddWithValue("@SiteID", prod.SiteID);
                    command.Parameters.AddWithValue("@MachineID", prod.MachineID);
                    command.Parameters.AddWithValue("@MachineExtRef", prod.MachineExtRef);
                    command.Parameters.AddWithValue("@MachineLongDescription", prod.MachineLongDescription);
                    command.Parameters.AddWithValue("@MachineNorm", prod.MachineNorm);
                    command.Parameters.AddWithValue("@MachinePositions", prod.MachinePositions);
                    command.Parameters.AddWithValue("@SubID", prod.SubID);
                    command.Parameters.AddWithValue("@MachineGroupID", prod.MachineGroupID);
                    command.Parameters.AddWithValue("@MachineGroupExtRef", prod.MachineGroupExtRef);
                    command.Parameters.AddWithValue("@MachineArea", prod.MachineArea);
                    command.Parameters.AddWithValue("@MachineGroupLongDescription", prod.MachineGroupLongDescription);
                    command.Parameters.AddWithValue("@CustomerID", prod.CustomerID);
                    command.Parameters.AddWithValue("@CustomerExtRef", prod.CustomerExtRef);
                    command.Parameters.AddWithValue("@CustomerLongDescription", prod.CustomerLongDescription);
                    command.Parameters.AddWithValue("@SortCategoryID", prod.SortCategoryID);
                    command.Parameters.AddWithValue("@SortCategoryExtRef", prod.SortCategoryExtRef);
                    command.Parameters.AddWithValue("@SortCategoryLongDescription", prod.SortCategoryLongDescription);
                    command.Parameters.AddWithValue("@ArticleID", prod.ArticleID);
                    command.Parameters.AddWithValue("@ArticleExtRef", prod.ArticleExtRef);
                    command.Parameters.AddWithValue("@ArticleLongDescription", prod.ArticleLongDescription);
                    command.Parameters.AddWithValue("@OperatorID", prod.OperatorID);
                    command.Parameters.AddWithValue("@OperatorExtRef", prod.OperatorExtRef);
                    command.Parameters.AddWithValue("@OperatorLongDescription", prod.OperatorLongDescription);
                    command.Parameters.AddWithValue("@ProcessCode", prod.ProcessCode);
                    command.Parameters.AddWithValue("@ProcessName", prod.ProcessName);
                    command.Parameters.AddWithValue("@ProcessNorm", prod.ProcessNorm);
                    command.Parameters.AddWithValue("@PiecesCounter", prod.PiecesCounter);
                    command.Parameters.AddWithValue("@WeightCounter", prod.WeightCounter);
                    command.Parameters.AddWithValue("@WeightUnits", prod.WeightUnits);
                    command.Parameters.AddWithValue("@RejectCounter", prod.RejectCounter);
                    command.Parameters.AddWithValue("@RewashCounter", prod.RewashCounter); 
                    command.Parameters.AddWithValue("@BatchCounter", prod.BatchCounter);
                    command.Parameters.AddWithValue("@ProductionTime", prod.ProductionTime);
                    command.Parameters.AddWithValue("@StopTime", prod.StopTime);
                    command.Parameters.AddWithValue("@NoFlowTime", prod.NoFlowTime);
                    command.Parameters.AddWithValue("@FaultTime", prod.FaultTime);
                    command.Parameters.AddWithValue("@FreshWater", prod.FreshWater);
                    command.Parameters.AddWithValue("@WasteWater", prod.WasteWater);
                    command.Parameters.AddWithValue("@WaterUnits", prod.WaterUnits);
                    command.Parameters.AddWithValue("@GasEnergy", prod.GasEnergy);
                    command.Parameters.AddWithValue("@GasEnergyUnits", prod.GasEnergyUnits);
                    command.Parameters.AddWithValue("@GasVolume", prod.GasVolume);
                    command.Parameters.AddWithValue("@GasVolumeUnits", prod.GasVolumeUnits);
                    command.Parameters.AddWithValue("@ElectricalEnergy", prod.ElectricalEnergy);
                    command.Parameters.AddWithValue("@ElectricityUnits", prod.ElectricityUnits);
                    command.Parameters.AddWithValue("@SteamConsumption", prod.SteamConsumption);
                    command.Parameters.AddWithValue("@SteamUnits", prod.SteamUnits);
                    command.Parameters.AddWithValue("@AirConsumption", prod.AirConsumption);
                    command.Parameters.AddWithValue("@AirUnits", prod.AirUnits);
                    command.Parameters.AddWithValue("@CountExitPointData", prod.CountExitPointData);
                    command.Parameters.AddWithValue("@MachineCountExitPoint", prod.MachineCountExitPoint);
                    command.Parameters.AddWithValue("@OperatorCountExitPoint", prod.OperatorCountExitPoint);
                    command.Parameters.AddWithValue("@ProcessCountExitPoint", prod.ProcessCountExitPoint); 
                    command.Parameters.AddWithValue("@ExcludePower", prod.ExcludePower);
                    command.Parameters.AddWithValue("@ProductionRefData", prod.ProductionRefData);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenPublic);

                        if (RecNum != null)
                        {
                            prod.RecNum = (int)RecNum;
                            prod.HasChanged = false;
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

        public void InsertNewDataRec(CustArtRec CustArtRec)
        {
            string commandString =
                @"INSERT INTO [dbo].[tblCustArtProcess]
                   ([DateHour]
                   ,[MachineID]
                   ,[Machine_ExtRef]
                   ,[Machine_LongDescription]
                   ,[Machine_NormValue]
                   ,[Machine_Positions]
                   ,[SubID]
                   ,[MachineGroup_ExtRef]
                   ,[MachineArea]
                   ,[MachineGroup_LongDescription]";
            if (this.DatabaseVersion >= 1.4)
            {
                commandString += @"
                        ,[CountExitPointData] 
	                    ,[ExcludePower]
	                    ,[ProductionRefData] 
                        ,[Air_Consumption]";
            }
            commandString += @"
                   ,[CustomerID]
                   ,[Customer_ExtRef]
                   ,[Customer_LongDescription]
                   ,[SortCategoryID]
                   ,[SortCategory_ExtRef]
                   ,[SortCategory_LongDescription]
                   ,[ArticleID]
                   ,[Article_ExtRef]
                   ,[Article_LongDescription]
                   ,[ProcessCode]
                   ,[ProcessName]
                   ,[Pieces_Counter]
                   ,[Weight_Counter]
                   ,[Batch_Counter]
                   ,[Production_Time]
                   ,[Stop_Time]
                   ,[NoFlow_Time]
                   ,[Fault_Time]
                   ,[Water_Consumption]
                   ,[Gas_Consumption]
                   ,[Steam_Consumption]
                   ,[Electricity_Consumption]
                   ,[Reject_Counter]
                   ,[Rewash_Counter])
             VALUES
                   (@DateHour
                   ,@MachineID
                   ,@Machine_ExtRef
                   ,@Machine_LongDescription
                   ,@Machine_NormValue
                   ,@Machine_Positions
                   ,@SubID
                   ,@MachineGroup_ExtRef
                   ,@MachineArea
                   ,@MachineGroup_LongDescription";
            if (this.DatabaseVersion >= 1.4)
            {
                commandString += @"
                        ,@CountExitPointData
	                    ,@ExcludePower
	                    ,@ProductionRefData
	                    ,@Air_Consumption";
            }
            commandString += @"
                  ,@CustomerID
                   ,@Customer_ExtRef
                   ,@Customer_LongDescription
                   ,@SortCategoryID
                   ,@SortCategory_ExtRef
                   ,@SortCategory_LongDescription
                   ,@ArticleID
                   ,@Article_ExtRef
                   ,@Article_LongDescription
                   ,@ProcessCode
                   ,@ProcessName
                   ,@Pieces_Counter
                   ,@Weight_Counter
                   ,@Batch_Counter
                   ,@Production_Time
                   ,@Stop_Time
                   ,@NoFlow_Time
                   ,@Fault_Time
                   ,@Water_Consumption
                   ,@Gas_Consumption
                   ,@Steam_Consumption
                   ,@Electricity_Consumption
                   ,@Reject_Counter
                   ,@Rewash_Counter)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DateHour", CustArtRec.DateHour);
                    command.Parameters.AddWithValue("@MachineID", CustArtRec.MachineID);
                    command.Parameters.AddWithValue("@Machine_ExtRef", CustArtRec.Machine_ExtRef);
                    command.Parameters.AddWithValue("@Machine_LongDescription", CustArtRec.Machine_LongDescription);
                    command.Parameters.AddWithValue("@Machine_NormValue", CustArtRec.Machine_NormValue);
                    command.Parameters.AddWithValue("@Machine_Positions", CustArtRec.Machine_Positions);
                    command.Parameters.AddWithValue("@SubID", CustArtRec.SubID);
                    command.Parameters.AddWithValue("@MachineGroup_ExtRef", CustArtRec.MachineGroup_ExtRef);
                    command.Parameters.AddWithValue("@MachineArea", CustArtRec.MachineArea);
                    command.Parameters.AddWithValue("@MachineGroup_LongDescription", CustArtRec.MachineGroup_LongDescription);
                    if (this.DatabaseVersion >= 1.4)
                    {
                        command.Parameters.AddWithValue("@CountExitPointData", CustArtRec.CountExitPointData);
                        command.Parameters.AddWithValue("@ExcludePower", CustArtRec.ExcludePower);
                        command.Parameters.AddWithValue("@ProductionRefData", CustArtRec.ProductionRefData);
                        command.Parameters.AddWithValue("@Air_Consumption", CustArtRec.Air_Consumption);
                    }
                    command.Parameters.AddWithValue("@CustomerID", CustArtRec.CustomerID);
                    command.Parameters.AddWithValue("@Customer_ExtRef", CustArtRec.Customer_ExtRef);
                    command.Parameters.AddWithValue("@Customer_LongDescription", CustArtRec.Customer_LongDescription);
                    command.Parameters.AddWithValue("@SortCategoryID", CustArtRec.SortCategoryID);
                    command.Parameters.AddWithValue("@SortCategory_ExtRef", CustArtRec.SortCategory_ExtRef);
                    command.Parameters.AddWithValue("@SortCategory_LongDescription", CustArtRec.SortCategory_LongDescription);
                    command.Parameters.AddWithValue("@ArticleID", CustArtRec.ArticleID);
                    command.Parameters.AddWithValue("@Article_ExtRef", CustArtRec.Article_ExtRef);
                    command.Parameters.AddWithValue("@Article_LongDescription", CustArtRec.Article_LongDescription);
                    command.Parameters.AddWithValue("@ProcessCode", CustArtRec.ProcessCode);
                    command.Parameters.AddWithValue("@ProcessName", CustArtRec.ProcessName);
                    command.Parameters.AddWithValue("@Pieces_Counter", CustArtRec.Pieces_Counter);
                    command.Parameters.AddWithValue("@Weight_Counter", CustArtRec.Weight_Counter);
                    command.Parameters.AddWithValue("@Batch_Counter", CustArtRec.Batch_Counter);
                    command.Parameters.AddWithValue("@Production_Time", CustArtRec.Production_Time);
                    command.Parameters.AddWithValue("@Stop_Time", CustArtRec.Stop_Time);
                    command.Parameters.AddWithValue("@NoFlow_Time", CustArtRec.NoFlow_Time);
                    command.Parameters.AddWithValue("@Fault_Time", CustArtRec.Fault_Time);
                    command.Parameters.AddWithValue("@Water_Consumption", CustArtRec.Water_Consumption);
                    command.Parameters.AddWithValue("@Gas_Consumption", CustArtRec.Gas_Consumption);
                    command.Parameters.AddWithValue("@Steam_Consumption", CustArtRec.Steam_Consumption);
                    command.Parameters.AddWithValue("@Electricity_Consumption", CustArtRec.Electricity_Consumption);
                    command.Parameters.AddWithValue("@Reject_Counter", CustArtRec.Reject_Counter);
                    command.Parameters.AddWithValue("@Rewash_Counter", CustArtRec.Rewash_Counter);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenPublic);

                        if (RecNum != null)
                        {
                            CustArtRec.RecNum = (int)RecNum;
                            CustArtRec.HasChanged = false;
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
        #endregion

        #region Update Data
//        public void UpdateOperatorDetails(CustArtRec CustArtRec)
//        {
//            SqlDataAccess da = SqlDataAccess.Singleton;
//            string commandString =
//                @"UPDATE dbo.tblCustArtProcess
//                        SET [DateHour] = @DateHourdatetime
//                        ,[MachineID] = @MachineID
//                        ,[Machine_ExtRef] = @Machine_ExtRef
//                        ,[Machine_LongDescription] = @Machine_LongDescription
//                        ,[Machine_NormValue] = @Machine_NormValue
//                        ,[Machine_Positions] = @Machine_Positions
//                        ,[SubID] = @SubID
//                        ,[MachineGroup_ExtRef] = @MachineGroup_ExtRef
//                        ,[MachineArea] = @MachineArea
//                        ,[MachineGroup_LongDescription] = @MachineGroup_LongDescription";
//            if (da.DatabaseVersion >= 1.4)
//            {
//                commandString += @"
//                        ,[CountExitPointData] = @CountExitPointData
//	                    ,[ExcludePower] = @ExcludePower
//	                    ,[ProductionRefData] = @ProductionRefData
//                        ,[Air_Consumption] = @Air_Consumption";
//            }
//            commandString += @"
//                        ,[CustomerID] = @CustomerID
//                        ,[Customer_ExtRef] = @Customer_ExtRef
//                        ,[Customer_LongDescription] = @Customer_LongDescription
//                        ,[SortCategoryID] = @SortCategoryID
//                        ,[SortCategory_ExtRef] = @SortCategory_ExtRef
//                        ,[SortCategory_LongDescription] = @SortCategory_LongDescription
//                        ,[ArticleID] = @ArticleID
//                        ,[Article_ExtRef] = @Article_ExtRef
//                        ,[Article_LongDescription] = @Article_LongDescription
//                        ,[ProcessCode] = @ProcessCode
//                        ,[ProcessName] = @ProcessName
//                        ,[Pieces_Counter] = @Pieces_Counter
//                        ,[Weight_Counter] = @Weight_Counter
//                        ,[Batch_Counter] = @Batch_Counter
//                        ,[Production_Time] = @Production_Time
//                        ,[Stop_Time] = @Stop_Time
//                        ,[NoFlow_Time] = @NoFlow_Time
//                        ,[Fault_Time] = @Fault_Time
//                        ,[Water_Consumption] = @Water_Consumption
//                        ,[Gas_Consumption] = @Gas_Consumption
//                        ,[Steam_Consumption] = @Steam_Consumption
//                        ,[Electricity_Consumption] = @Electricity_Consumption
//                        ,[Reject_Counter] = @Reject_Counter
//                        ,[Rewash_Counter] = @Rewash_Counter                    
//                    WHERE RecNum = @RecNum";
//            try
//            {
//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    command.Parameters.AddWithValue("@RecNum", CustArtRec.RecNum);
//                    command.Parameters.AddWithValue("@DateHour", CustArtRec.DateHour);
//                    command.Parameters.AddWithValue("@MachineID", CustArtRec.MachineID);
//                    command.Parameters.AddWithValue("@Machine_ExtRef", CustArtRec.Machine_ExtRef);
//                    command.Parameters.AddWithValue("@Machine_LongDescription", CustArtRec.Machine_LongDescription);
//                    command.Parameters.AddWithValue("@Machine_NormValue", CustArtRec.Machine_NormValue);
//                    command.Parameters.AddWithValue("@Machine_Positions", CustArtRec.Machine_Positions);
//                    command.Parameters.AddWithValue("@SubID", CustArtRec.SubID);
//                    command.Parameters.AddWithValue("@MachineGroup_ExtRef", CustArtRec.MachineGroup_ExtRef);
//                    command.Parameters.AddWithValue("@MachineArea", CustArtRec.MachineArea);
//                    command.Parameters.AddWithValue("@MachineGroup_LongDescription", CustArtRec.MachineGroup_LongDescription);
//                    if (da.DatabaseVersion >= 1.4)
//                    {
//                        command.Parameters.AddWithValue("@CountExitPointData", CustArtRec.CountExitPointData);
//                        command.Parameters.AddWithValue("@ExcludePower", CustArtRec.ExcludePower);
//                        command.Parameters.AddWithValue("@ProductionRefData", CustArtRec.ProductionRefData);
//                        command.Parameters.AddWithValue("@Air_Consumption", CustArtRec.Air_Consumption);
//                    }
//                    command.Parameters.AddWithValue("@CustomerID", CustArtRec.CustomerID);
//                    command.Parameters.AddWithValue("@Customer_ExtRef", CustArtRec.Customer_ExtRef);
//                    command.Parameters.AddWithValue("@Customer_LongDescription", CustArtRec.Customer_LongDescription);
//                    command.Parameters.AddWithValue("@SortCategoryID", CustArtRec.SortCategoryID);
//                    command.Parameters.AddWithValue("@SortCategory_ExtRef", CustArtRec.SortCategory_ExtRef);
//                    command.Parameters.AddWithValue("@SortCategory_LongDescription", CustArtRec.SortCategory_LongDescription);
//                    command.Parameters.AddWithValue("@ArticleID", CustArtRec.ArticleID);
//                    command.Parameters.AddWithValue("@Article_ExtRef", CustArtRec.Article_ExtRef);
//                    command.Parameters.AddWithValue("@Article_LongDescription", CustArtRec.Article_LongDescription);
//                    command.Parameters.AddWithValue("@ProcessCode", CustArtRec.ProcessCode);
//                    command.Parameters.AddWithValue("@ProcessName", CustArtRec.ProcessName);
//                    command.Parameters.AddWithValue("@Pieces_Counter", CustArtRec.Pieces_Counter);
//                    command.Parameters.AddWithValue("@Weight_Counter", CustArtRec.Weight_Counter);
//                    command.Parameters.AddWithValue("@Batch_Counter", CustArtRec.Batch_Counter);
//                    command.Parameters.AddWithValue("@Production_Time", CustArtRec.Production_Time);
//                    command.Parameters.AddWithValue("@Stop_Time", CustArtRec.Stop_Time);
//                    command.Parameters.AddWithValue("@NoFlow_Time", CustArtRec.NoFlow_Time);
//                    command.Parameters.AddWithValue("@Fault_Time", CustArtRec.Fault_Time);
//                    command.Parameters.AddWithValue("@Water_Consumption", CustArtRec.Water_Consumption);
//                    command.Parameters.AddWithValue("@Gas_Consumption", CustArtRec.Gas_Consumption);
//                    command.Parameters.AddWithValue("@Steam_Consumption", CustArtRec.Steam_Consumption);
//                    command.Parameters.AddWithValue("@Electricity_Consumption", CustArtRec.Electricity_Consumption);
//                    command.Parameters.AddWithValue("@Reject_Counter", CustArtRec.Reject_Counter);
//                    command.Parameters.AddWithValue("@Rewash_Counter", CustArtRec.Rewash_Counter);
//                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
//                    CustArtRec.HasChanged = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ExceptionHandler.Handle(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }
        #endregion

        #region Delete Data

        public void DeleteCustArtDetails(CustArtRec CustArtRec)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblCustArtProcess]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", CustArtRec.RecNum);
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

        public void DeleteProductionDateHour(DateTime dateHour)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblProduction]
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

        public void DeleteCustArtDateHour(DateTime dateHour)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblCustArtProcess]
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
        #endregion
    }

    #region Data Collection Class
    public class CustArtProcess : List<CustArtRec>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecNumPos = dr.GetOrdinal("RecNum");
            int DateHourPos = dr.GetOrdinal("DateHour");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int CountExitPointDataPos = -1;
            int ExcludePowerPos = -1;
            int ProductionRefDataPos = -1;
            int Air_ConsumptionPos = -1;
            if (da.DatabaseVersion >= 1.4)
            {
                CountExitPointDataPos = dr.GetOrdinal("CountExitPointData");
                ExcludePowerPos = dr.GetOrdinal("ExcludePower");
                ProductionRefDataPos = dr.GetOrdinal("ProductionRefData");
                Air_ConsumptionPos = dr.GetOrdinal("Air_Consumption");
            }
            int SubIDPos = dr.GetOrdinal("SubID");
            int CustomerIDPos = dr.GetOrdinal("CustomerID");
            int SortCategoryIDPos = dr.GetOrdinal("SortCategoryID");
            int ArticleIDPos = dr.GetOrdinal("ArticleID");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int Pieces_CounterPos = dr.GetOrdinal("Pieces_Counter");
            int Weight_CounterPos = dr.GetOrdinal("Weight_Counter");
            int Batch_CounterPos = dr.GetOrdinal("Batch_Counter");
            int Production_TimePos = dr.GetOrdinal("Production_Time");
            int Stop_TimePos = dr.GetOrdinal("Stop_Time");
            int NoFlow_TimePos = dr.GetOrdinal("NoFlow_Time");
            int Fault_TimePos = dr.GetOrdinal("Fault_Time");
            int Water_ConsumptionPos = dr.GetOrdinal("Water_Consumption");
            int Gas_ConsumptionPos = dr.GetOrdinal("Gas_Consumption");
            int Steam_ConsumptionPos = dr.GetOrdinal("Steam_Consumption");
            int Electricity_ConsumptionPos = dr.GetOrdinal("Electricity_Consumption");
            int Reject_CounterPos = dr.GetOrdinal("Reject_Counter");
            int Rewash_CounterPos = dr.GetOrdinal("Rewash_Counter");

            while (dr.Read())
            {
                CustArtRec custArtRec = new CustArtRec();
                custArtRec.RecNum = dr.GetInt32(RecNumPos);
                custArtRec.DateHour = dr.GetDateTime(DateHourPos);
                if (da.CombinedProduction)
                {
                    custArtRec.MachineID = dr.GetInt32(MachineIDPos);
                    custArtRec.SortCategoryID = dr.GetInt32(SortCategoryIDPos);
                    custArtRec.ArticleID = dr.GetInt32(ArticleIDPos);
                    custArtRec.ProcessCode = dr.GetInt32(ProcessCodePos);
                }
                else
                {
                    custArtRec.MachineID = dr.GetInt16(MachineIDPos);
                    custArtRec.SortCategoryID = dr.GetInt16(SortCategoryIDPos);
                    custArtRec.ArticleID = dr.GetInt16(ArticleIDPos);
                    custArtRec.ProcessCode = dr.GetInt16(ProcessCodePos);
                }
                custArtRec.SubID = dr.GetInt32(SubIDPos);
                custArtRec.CustomerID = dr.GetInt32(CustomerIDPos);
                custArtRec.Pieces_Counter = dr.GetInt32(Pieces_CounterPos);
                custArtRec.Weight_Counter = dr.GetDecimal(Weight_CounterPos);
                custArtRec.Batch_Counter = dr.GetInt32(Batch_CounterPos);
                custArtRec.Production_Time = dr.GetInt32(Production_TimePos);
                custArtRec.Stop_Time = dr.GetInt32(Stop_TimePos);
                custArtRec.Fault_Time = dr.GetInt32(Fault_TimePos);
                custArtRec.Water_Consumption = dr.GetDecimal(Water_ConsumptionPos);
                custArtRec.Gas_Consumption = dr.GetDecimal(Gas_ConsumptionPos);
                custArtRec.Steam_Consumption = dr.GetDecimal(Steam_ConsumptionPos);
                custArtRec.Electricity_Consumption = dr.GetDecimal(Electricity_ConsumptionPos);
                custArtRec.Reject_Counter = dr.GetInt32(Reject_CounterPos);
                custArtRec.Rewash_Counter = dr.GetInt32(Rewash_CounterPos);
                custArtRec.HasChanged = false;

                if (da.DatabaseVersion >= 1.4)
                {
                    custArtRec.CountExitPointData = dr.GetBoolean(CountExitPointDataPos);
                    custArtRec.ExcludePower = dr.GetBoolean(ExcludePowerPos);
                    custArtRec.ProductionRefData = dr.GetBoolean(ProductionRefDataPos);
                    custArtRec.Air_Consumption = dr.GetDecimal(Air_ConsumptionPos);
                }
                else
                {
                    custArtRec.CountExitPointData = true;
                    custArtRec.ExcludePower = true;
                    custArtRec.ProductionRefData = false;
                    custArtRec.Air_Consumption = 0;
                }
                this.Add(custArtRec);
            }

            return this.Count;
        }

        public CustArtRec GetByIds(int MachineID, int Subid, int CustID, int ArtID, int SortID, int ProcCode)
        {
            return this.Find(delegate(CustArtRec capRec)
            {
                return ((capRec.MachineID == MachineID)
                    && (capRec.SubID == Subid)
                    && (capRec.CustomerID == CustID)
                    && (capRec.ArticleID == ArtID)
                    && (capRec.SortCategoryID == SortID)
                    && (capRec.ProcessCode == ProcCode));
            });
        }
    }

    public class ProductionRecs : List<ProductionRec>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecNumPos = dr.GetOrdinal("RecNum");
            int DateHourPos = dr.GetOrdinal("DateHour");
            int SiteIDPos = dr.GetOrdinal("SiteID");
            int MachineGroupIDPos = dr.GetOrdinal("MachineGroupID");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int CustomerIDPos = dr.GetOrdinal("CustomerID");
            int SortCategoryIDPos = dr.GetOrdinal("SortCategoryID");
            int ArticleIDPos = dr.GetOrdinal("ArticleID");
            int OperatorIDPos = dr.GetOrdinal("OperatorID");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int PiecesCounterPos = dr.GetOrdinal("PiecesCounter");
            int RejectCounterPos = dr.GetOrdinal("RejectCounter");
            int RewashCounterPos = dr.GetOrdinal("RewashCounter");
            int BatchCounterPos = dr.GetOrdinal("BatchCounter");
            int WeightCounterPos = dr.GetOrdinal("WeightCounter");
            int WeightUnitsPos = dr.GetOrdinal("WeightUnits");
            int ProductionTimePos = dr.GetOrdinal("ProductionTime");
            int StopTimePos = dr.GetOrdinal("StopTime");
            int NoFlowTimePos = dr.GetOrdinal("NoFlowTime");
            int FaultTimePos = dr.GetOrdinal("FaultTime");
            int FreshWaterPos = dr.GetOrdinal("FreshWater");
            int WasteWaterPos = dr.GetOrdinal("WasteWater");
            int WaterUnitsPos = dr.GetOrdinal("WaterUnits");
            int GasEnergyPos = dr.GetOrdinal("GasEnergy");
            int GasEnergyUnitsPos = dr.GetOrdinal("GasEnergyUnits");
            int GasVolumePos = dr.GetOrdinal("GasVolume");
            int GasVolumeUnitsPos = dr.GetOrdinal("GasVolumeUnits");
            int ElectricalEnergyPos = dr.GetOrdinal("ElectricalEnergy");
            int ElectricityUnitsPos = dr.GetOrdinal("ElectricityUnits");
            int SteamConsumptionPos = dr.GetOrdinal("SteamConsumption");
            int SteamUnitsPos = dr.GetOrdinal("SteamUnits");
            int AirConsumptionPos = dr.GetOrdinal("AirConsumption");
            int AirUnitsPos = dr.GetOrdinal("AirUnits");
            int MachineCountExitPointPos = dr.GetOrdinal("MachineCountExitPoint");
            int OperatorCountExitPointPos = dr.GetOrdinal("OperatorCountExitPoint");
            int ProcessCountExitPointPos = dr.GetOrdinal("ProcessCountExitPoint");
            int ProductionRefDataPos = dr.GetOrdinal("ProductionRefData");
            int ExcludePowerPos = dr.GetOrdinal("ExcludePower");

            while (dr.Read())
            {
                ProductionRec prodRec = new ProductionRec();
                prodRec.RecNum = dr.GetInt32(RecNumPos);
                prodRec.DateHour = dr.GetDateTime(DateHourPos);
                prodRec.SiteID = dr.GetInt32(SiteIDPos);
                prodRec.MachineGroupID = dr.GetInt32(MachineGroupIDPos);
                prodRec.MachineID = dr.GetInt32(MachineIDPos);
                prodRec.SubID = dr.GetInt32(SubIDPos);
                prodRec.CustomerID = dr.GetInt32(CustomerIDPos);
                prodRec.SortCategoryID = dr.GetInt32(SortCategoryIDPos);
                prodRec.ArticleID = dr.GetInt32(ArticleIDPos);
                prodRec.OperatorID = dr.GetInt32(OperatorIDPos);
                prodRec.ProcessCode = dr.GetInt32(ProcessCodePos);
                prodRec.PiecesCounter = dr.GetInt32(PiecesCounterPos);
                prodRec.RejectCounter = dr.GetInt32(RejectCounterPos);
                prodRec.RewashCounter = dr.GetInt32(RewashCounterPos);
                prodRec.BatchCounter = dr.GetInt32(BatchCounterPos);
                prodRec.WeightCounter = dr.GetDecimal(WeightCounterPos);
                prodRec.WeightUnits = dr.GetInt32(WeightUnitsPos);
                prodRec.ProductionTime = dr.GetInt32(ProductionTimePos);
                prodRec.StopTime = dr.GetInt32(StopTimePos);
                prodRec.NoFlowTime = dr.GetInt32(NoFlowTimePos);
                prodRec.FaultTime = dr.GetInt32(FaultTimePos);
                prodRec.FreshWater = dr.GetDecimal(FreshWaterPos);
                prodRec.WasteWater = dr.GetDecimal(WasteWaterPos);
                prodRec.WaterUnits = dr.GetInt32(WaterUnitsPos);
                prodRec.GasEnergy = dr.GetDecimal(GasEnergyPos);
                prodRec.GasEnergyUnits = dr.GetInt32(GasEnergyUnitsPos);
                prodRec.GasVolume = dr.GetDecimal(GasVolumePos);
                prodRec.GasVolumeUnits = dr.GetInt32(GasVolumeUnitsPos);
                prodRec.ElectricalEnergy = dr.GetDecimal(ElectricalEnergyPos);
                prodRec.ElectricityUnits = dr.GetInt32(ElectricityUnitsPos);
                prodRec.SteamConsumption = dr.GetDecimal(SteamConsumptionPos);
                prodRec.SteamUnits = dr.GetInt32(SteamUnitsPos);
                prodRec.AirConsumption = dr.GetDecimal(AirConsumptionPos);
                prodRec.AirUnits = dr.GetInt32(AirUnitsPos);
                prodRec.MachineCountExitPoint = dr.GetBoolean(MachineCountExitPointPos);
                prodRec.OperatorCountExitPoint = dr.GetBoolean(ProcessCountExitPointPos);
                prodRec.ProcessCountExitPoint = dr.GetBoolean(ProductionRefDataPos);
                prodRec.ExcludePower = dr.GetBoolean(ExcludePowerPos);
                prodRec.ProductionRefData = dr.GetBoolean(ProductionRefDataPos);
                prodRec.HasChanged = false;
                this.Add(prodRec);
            }
            return this.Count;
        }

        public ProductionRec GetByIds(int MachineID, int Subid, int CustID, int ArtID, int SortID, int OpID, int ProcCode)
        {
            return this.Find(delegate(ProductionRec prodRec)
            {
                return ((prodRec.MachineID == MachineID)
                    && (prodRec.SubID == Subid)
                    && (prodRec.CustomerID == CustID)
                    && (prodRec.ArticleID == ArtID)
                    && (prodRec.SortCategoryID == SortID)
                    && (prodRec.ProcessCode == ProcCode)
                    && (prodRec.OperatorID == OpID));
            });
        }


    }
    #endregion

    #region Item Classes 
    public class CustArtRec : DataItem
    {
        #region CustArtRec Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal DateTime DateHour;
            internal int MachineID;
            internal string Machine_ExtRef;
            internal string Machine_LongDescription;
            internal int Machine_NormValue;
            internal int Machine_Positions;
            internal int SubID;
            internal string MachineGroup_ExtRef;
            internal string MachineArea;
            internal string MachineGroup_LongDescription;
            internal bool CountExitPointData;
            internal bool ExcludePower;
            internal bool ProductionRefData;
            internal int CustomerID;
            internal string Customer_ExtRef;
            internal string Customer_LongDescription;
            internal int SortCategoryID;
            internal string SortCategory_ExtRef;
            internal string SortCategory_LongDescription;
            internal int ArticleID;
            internal string Article_ExtRef;
            internal string Article_LongDescription;
            internal int ProcessCode;
            internal string ProcessName;
            internal int Pieces_Counter;
            internal decimal Weight_Counter;
            internal int Batch_Counter;
            internal int Production_Time;
            internal int Stop_Time;
            internal int NoFlow_Time;
            internal int Fault_Time;
            internal decimal Water_Consumption;
            internal decimal Water_Energy;
            internal decimal Gas_Consumption;
            internal decimal Steam_Consumption;
            internal decimal Electricity_Consumption;
            internal decimal Air_Consumption;
            internal int Reject_Counter;
            internal int Rewash_Counter;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public CustArtRec()
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
                    MachineGroup mg = machineGroups.GetById(m.MachineGroup_idJensen);
                    if (mg != null)
                    {
                        this.activeData.MachineArea = mg.MachineArea;
                        this.activeData.MachineGroup_ExtRef = mg.ExtRef;
                        //if (mg.ExtRef != string.Empty)
                        //    this.activeData.MachineGroup_ExtRef = mg.ExtRef;
                        //else
                        //    this.activeData.MachineGroup_ExtRef = mg.idJensen.ToString();
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

        public bool ShouldStoreData
        {
            get
            {
                bool test = CountExitPointData || ProductionRefData || !ExcludePower;
                return test;
            }
        }

        public bool CountExitPointData
        {
            get
            {
                return this.activeData.CountExitPointData;
            }
            set
            {
                HasChanged |= this.activeData.CountExitPointData != value;
                this.activeData.CountExitPointData = value;
            }
        }

        public bool ExcludePower
        {
            get
            {
                return this.activeData.ExcludePower;
            }
            set
            {
                HasChanged |= this.activeData.ExcludePower != value;
                this.activeData.ExcludePower = value;
            }
        }

        public bool ProductionRefData
        {
            get
            {
                return this.activeData.ProductionRefData;
            }
            set
            {
                HasChanged |= this.activeData.ProductionRefData != value;
                this.activeData.ProductionRefData = value;
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

        public int CustomerID
        {
            get
            {
                return this.activeData.CustomerID;
            }
            set
            {
                this.activeData.CustomerID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Customers customers = da.GetAllActiveCustomers();
                Customer c = customers.GetById(this.activeData.CustomerID);
                if (c != null)
                {
                    if (c.ExtRef != string.Empty)
                        this.activeData.Customer_ExtRef = c.ExtRef;
                    else
                        this.activeData.Customer_ExtRef = value.ToString();
                    this.activeData.Customer_LongDescription = c.LongDescription;
                }
                else
                {
                    this.activeData.Customer_ExtRef = "";
                    this.activeData.Customer_LongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string Customer_ExtRef
        {
            get { return this.activeData.Customer_ExtRef; }
        }

        public string Customer_LongDescription
        {
            get { return this.activeData.Customer_LongDescription; }
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
                SqlDataAccess da = SqlDataAccess.Singleton;
                SortCategories sortCategories = da.GetAllActiveSortCategories();
                SortCategory s = sortCategories.GetById(this.activeData.SortCategoryID);
                if (s != null)
                {
                    if (s.ExtRef != string.Empty)
                        this.activeData.SortCategory_ExtRef = s.ExtRef;
                    else 
                        this.activeData.SortCategory_ExtRef = value.ToString();
                    this.activeData.SortCategory_LongDescription = s.LongDescription;
                }
                else
                {
                    this.activeData.SortCategory_ExtRef = "";
                    this.activeData.SortCategory_LongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string SortCategory_ExtRef
        {
            get { return this.activeData.SortCategory_ExtRef; }
        }
        public string SortCategory_LongDescription
        {
            get { return this.activeData.SortCategory_LongDescription; }
        }

        public int ArticleID
        {
            get
            {
                return this.activeData.ArticleID;
            }
            set
            {
                this.activeData.ArticleID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Articles articles = da.GetAllActiveArticles();
                Article a = articles.GetById(this.activeData.ArticleID);
                if (a != null)
                {
                    if (a.ExtRef != string.Empty)
                        this.activeData.Article_ExtRef = a.ExtRef;
                    else
                        this.activeData.Article_ExtRef = value.ToString();
                    this.activeData.Article_LongDescription = a.LongDescription;
                }
                else
                {
                    this.activeData.Article_ExtRef = "";
                    this.activeData.Article_LongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string Article_ExtRef
        {
            get { return this.activeData.Article_ExtRef; }
        }
        public string Article_LongDescription
        {
            get { return this.activeData.Article_LongDescription; }
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
                SqlDataAccess da = SqlDataAccess.Singleton;
                ProcessNames processNames = da.GetProcessNames();
                ProcessName pn = processNames.GetByIDs(this.activeData.MachineID, this.activeData.ProcessCode);
                if (pn != null)
                {
                    this.activeData.ProcessName = pn.ProcName;
                }
                else this.activeData.ProcessName = "";
                HasChanged = true;
            }
        }

        public string ProcessName
        {
            get { return this.activeData.ProcessName; }
        }

        public int Pieces_Counter
        {
            get
            {
                return this.activeData.Pieces_Counter;
            }
            set
            {
                if (this.activeData.Pieces_Counter != value)
                {
                    this.activeData.Pieces_Counter = value;
                    HasChanged = true;
                }
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
                if (this.activeData.Weight_Counter != value)
                {
                    this.activeData.Weight_Counter = value;
                    HasChanged = true;
                }
            }
        }

        public int Batch_Counter
        {
            get
            {
                return this.activeData.Batch_Counter;
            }
            set
            {
                if (this.activeData.Batch_Counter != value)
                {
                    this.activeData.Batch_Counter = value;
                    HasChanged = true;
                }
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
                if (this.activeData.Production_Time != value)
                {
                    this.activeData.Production_Time = value;
                    HasChanged = true;
                }
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
                if (this.activeData.Stop_Time != value)
                {
                    this.activeData.Stop_Time = value;
                    HasChanged = true;
                }
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
                    this.activeData.NoFlow_Time = value;
                    HasChanged = true;
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
                if (this.activeData.Fault_Time != value)
                {
                    this.activeData.Fault_Time = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Water_Consumption
        {
            get
            {
                return this.activeData.Water_Consumption;
            }
            set
            {
                if (this.activeData.Water_Consumption != value)
                {
                    this.activeData.Water_Consumption = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Water_Energy
        {
            get
            {
                return this.activeData.Water_Energy;
            }
            set
            {
                if (this.activeData.Water_Energy != value)
                {
                    this.activeData.Water_Energy = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Gas_Consumption
        {
            get
            {
                return this.activeData.Gas_Consumption;
            }
            set
            {
                if (this.activeData.Gas_Consumption != value)
                {
                    this.activeData.Gas_Consumption = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Steam_Consumption
        {
            get
            {
                return this.activeData.Steam_Consumption;
            }
            set
            {
                if (this.activeData.Steam_Consumption != value)
                {
                    this.activeData.Steam_Consumption = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Electricity_Consumption
        {
            get
            {
                return this.activeData.Electricity_Consumption;
            }
            set
            {
                if (this.activeData.Electricity_Consumption != value)
                {
                    this.activeData.Electricity_Consumption = value;
                    HasChanged = true;
                }
            }
        }

        public decimal Air_Consumption
        {
            get
            {
                return this.activeData.Air_Consumption;
            }
            set
            {
                if (this.activeData.Air_Consumption != value)
                {
                    this.activeData.Air_Consumption = value;
                    HasChanged = true;
                }
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
                if (this.activeData.Reject_Counter != value)
                {
                    this.activeData.Reject_Counter = value;
                    HasChanged = true;
                }
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
                if (this.activeData.Rewash_Counter != value)
                {
                    this.activeData.Rewash_Counter = value;
                    HasChanged = true;
                }

            }
        }


    }

    public class ProductionRec : DataItem
    { 
        #region ProductionRec Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal DateTime DateHour;
            internal int SiteID;
            internal int MachineID;
            internal string MachineExtRef;
            internal string MachineLongDescription;
            internal int MachineNorm;
            internal int MachinePositions;
            internal int SubID;
            internal int MachineGroupID;
            internal string MachineGroupExtRef;
            internal string MachineArea;
            internal string MachineGroupLongDescription;
            internal int CustomerID;
            internal string CustomerExtRef;
            internal string CustomerLongDescription;
            internal int SortCategoryID;
            internal string SortCategoryExtRef;
            internal string SortCategoryLongDescription;
            internal int ArticleID;
            internal string ArticleExtRef;
            internal string ArticleLongDescription;            
            internal int OperatorID;
            internal string OperatorExtRef;
            internal string OperatorLongDescription;
            internal int ProcessCode;
            internal string ProcessName;
            internal int ProcessNorm;
            internal int PiecesCounter;
            internal int RejectCounter;
            internal int RewashCounter;
            internal int BatchCounter;            
            internal decimal WeightCounter;
            internal int WeightUnits;
            internal int ProductionTime;
            internal int StopTime;
            internal int NoFlowTime;
            internal int FaultTime;
            internal decimal FreshWater;
            internal decimal WasteWater;
            internal int WaterUnits;
            internal decimal GasEnergy;
            internal int GasEnergyUnits;
            internal decimal GasVolume;
            internal int GasVolumeUnits;
            internal decimal ElectricalEnergy;
            internal int ElectricityUnits;
            internal decimal SteamConsumption;
            internal int SteamUnits;
            internal decimal AirConsumption;
            internal int AirUnits;
            //internal bool CountExitPointData;
            internal bool MachineCountExitPoint;
            internal bool OperatorCountExitPoint;
            internal bool ProcessCountExitPoint;
            internal bool ExcludePower;
            internal bool ProductionRefData;
            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ProductionRec()
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

        public int SiteID
        {
            get
            {
                return this.activeData.SiteID;
            }
            set
            {
                this.activeData.SiteID = value;
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
                        this.activeData.MachineExtRef = m.ExtRef;
                    else
                        this.activeData.MachineExtRef = value.ToString();
                    this.activeData.MachineLongDescription = m.LongDescription;
                    this.activeData.MachineNorm = m.NormValue;
                    this.activeData.MachinePositions = m.Positions;
                    MachineGroup mg = machineGroups.GetById(m.MachineGroup_idJensen);
                    if (mg != null)
                    {
                        activeData.MachineArea = mg.MachineArea;
                        activeData.MachineGroupExtRef = mg.ExtRef;
                        activeData.MachineGroupLongDescription = mg.LongDescription;
                        activeData.MachineGroupID = mg.idJensen;
                    }
                    else
                    {
                        activeData.MachineArea = "";
                        activeData.MachineGroupExtRef = "";
                        activeData.MachineGroupLongDescription = "";
                        activeData.MachineGroupID = -1;
                    }
                }
                else
                {
                    this.activeData.MachineExtRef = "";
                    this.activeData.MachineLongDescription = "";
                    this.activeData.MachineNorm = 0;
                    this.activeData.MachinePositions = 1;
                    this.activeData.MachineArea = "";
                    this.activeData.MachineGroupExtRef = "";
                    this.activeData.MachineGroupLongDescription = "";
                }
                if (activeData.MachinePositions == 0)
                    activeData.MachinePositions = 1;
                HasChanged = true;
            }
        }

        public string MachineExtRef
        {
            get { return this.activeData.MachineExtRef; }
        }

        public string MachineLongDescription
        {
            get { return this.activeData.MachineLongDescription; }
        }

        public int MachineNorm
        {
            get { return this.activeData.MachineNorm; }
        }

        public int MachinePositions
        {
            get { return this.activeData.MachinePositions; }
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

        public int MachineGroupID
        {
            get
            {
                return this.activeData.MachineGroupID;
            }
            set
            {
                this.activeData.MachineGroupID = value;
                HasChanged = true;
            }
        }

        public string MachineArea
        {
            get { return this.activeData.MachineArea; }
        }

        public string MachineGroupExtRef
        {
            get { return this.activeData.MachineGroupExtRef; }
        }

        public string MachineGroupLongDescription
        {
            get { return this.activeData.MachineGroupLongDescription; }
        }

        public int CustomerID
        {
            get
            {
                return this.activeData.CustomerID;
            }
            set
            {
                this.activeData.CustomerID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Customers customers = da.GetAllActiveCustomers();
                Customer c = customers.GetById(this.activeData.CustomerID);
                if (c != null)
                {
                    if (c.ExtRef != string.Empty)
                        this.activeData.CustomerExtRef = c.ExtRef;
                    else
                        this.activeData.CustomerExtRef = value.ToString();
                    this.activeData.CustomerLongDescription = c.LongDescription;
                }
                else
                {
                    this.activeData.CustomerExtRef = "";
                    this.activeData.CustomerLongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string CustomerExtRef
        {
            get { return this.activeData.CustomerExtRef; }
        }

        public string CustomerLongDescription
        {
            get { return this.activeData.CustomerLongDescription; }
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
                SqlDataAccess da = SqlDataAccess.Singleton;
                SortCategories sortCategories = da.GetAllActiveSortCategories();
                SortCategory s = sortCategories.GetById(this.activeData.SortCategoryID);
                if (s != null)
                {
                    if (s.ExtRef != string.Empty)
                        this.activeData.SortCategoryExtRef = s.ExtRef;
                    else 
                        this.activeData.SortCategoryExtRef = value.ToString();
                    this.activeData.SortCategoryLongDescription = s.LongDescription;
                }
                else
                {
                    this.activeData.SortCategoryExtRef = "";
                    this.activeData.SortCategoryLongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string SortCategoryExtRef
        {
            get { return this.activeData.SortCategoryExtRef; }
        }
 
        public string SortCategoryLongDescription
        {
            get { return this.activeData.SortCategoryLongDescription; }
        }

        public int ArticleID
        {
            get
            {
                return this.activeData.ArticleID;
            }
            set
            {
                this.activeData.ArticleID = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Articles articles = da.GetAllActiveArticles();
                Article a = articles.GetById(this.activeData.ArticleID);
                if (a != null)
                {
                    if (a.ExtRef != string.Empty)
                        this.activeData.ArticleExtRef = a.ExtRef;
                    else
                        this.activeData.ArticleExtRef = value.ToString();
                    this.activeData.ArticleLongDescription = a.LongDescription;
                }
                else
                {
                    this.activeData.ArticleExtRef = "";
                    this.activeData.ArticleLongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string ArticleExtRef
        {
            get { return this.activeData.ArticleExtRef; }
        }

        public string ArticleLongDescription
        {
            get { return this.activeData.ArticleLongDescription; }
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
                        this.activeData.OperatorExtRef = o.ExtRef;
                    else
                        this.activeData.OperatorExtRef = value.ToString();
                    this.activeData.OperatorLongDescription = o.LongDescription;
                }
                else
                {
                    this.activeData.OperatorExtRef = "";
                    this.activeData.OperatorLongDescription = value.ToString();
                }
                HasChanged = true;
            }
        }

        public string OperatorExtRef
        {
            get { return this.activeData.OperatorExtRef; }
        }

        public string OperatorLongDescription
        {
            get { return this.activeData.OperatorLongDescription; }
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
                this.activeData.ProcessNorm = 0;
                this.activeData.ProcessName = ""; 
                if (MachineID >= 0)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    ProcessNames processNames = da.GetProcessNames();
                    ProcessName pn = processNames.GetByIDs(this.activeData.MachineID, this.activeData.ProcessCode);
                    if (pn != null)
                    {
                        this.activeData.ProcessName = pn.ProcName;
                    }
                    if (CustomerID >= 0)
                    {
                        Machines ms = da.GetAllMachines();
                        Machine m = ms.GetById(MachineID);
                        if (m != null)
                        {
                            ProcessCodes pcs = da.GetAllProcessCodes(m);
                            if (pcs != null)
                            {
                                ProcessCode pc = null;
                                if (m.ProcessCodeType == 0)
                                {
                                    pc = pcs.GetByCustArtMachine(CustomerID, ArticleID, MachineID);
                                }
                                if (m.ProcessCodeType == 1)
                                {
                                    pc = pcs.GetByCustCatMachine(CustomerID, SortCategoryID, MachineID);
                                }
                                if (pc != null)
                                {
                                    this.activeData.ProcessNorm = pc.Production_Norm;
                                }
                            }
                        }
                    }
                }
                HasChanged = true;
            }
        }

        public string ProcessName
        {
            get { return this.activeData.ProcessName; }
        }

        public int ProcessNorm
        {
            get
            {
                //if (activeData.ProcessNorm > 0)
                //    return activeData.ProcessNorm;
                //else
                //    return activeData.MachineNorm;
                return activeData.ProcessNorm;
            }
            set
            {
                if (activeData.ProcessNorm != value)
                {
                    activeData.ProcessNorm = value;
                    HasChanged = true;
                }
            }
        }

        private int normTimeSeconds()
        {
            int nts = 0;
            SqlDataAccess da = SqlDataAccess.Singleton;
            switch (da.NormCalcMode)
            {
                case 0:
                    nts = ProductionTime + FaultTime;
                    break;
                case 1:
                    nts = ProductionTime;
                    break;
                case 2:
                    nts = ProductionTime + FaultTime + StopTime;
                    break;
                default:
                    nts = ProductionTime + FaultTime;
                    break;
            }
            return nts;
        }

        public int CalcNorm
        {
            get
            {
                Double norm = MachineNorm / MachinePositions;
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.UseProcessNorm)
                    norm = ProcessNorm;
                norm = norm * (normTimeSeconds() / 3600.0);
                return (int)norm;
            }
        }

        public int StationNorm
        {
            get
            {
                Double norm = MachineNorm / MachinePositions;
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.UseProcessNorm)
                    norm = ProcessNorm;
                return (int)norm;
            }
        }

        public int PiecesCounter
        {
            get
            {
                return this.activeData.PiecesCounter;
            }
            set
            {
                if (this.activeData.PiecesCounter != value)
                {
                    this.activeData.PiecesCounter = value;
                    HasChanged = true;
                }
            }
        }

        public int BatchCounter
        {
            get
            {
                return this.activeData.BatchCounter;
            }
            set
            {
                if (this.activeData.BatchCounter != value)
                {
                    this.activeData.BatchCounter = value;
                    HasChanged = true;
                }
            }
        }

        public int RejectCounter
        {
            get
            {
                return this.activeData.RejectCounter;
            }
            set
            {
                if (this.activeData.RejectCounter != value)
                {
                    this.activeData.RejectCounter = value;
                    HasChanged = true;
                }
            }
        }

        public int RewashCounter
        {
            get
            {
                return this.activeData.RewashCounter;
            }
            set
            {
                if (this.activeData.RewashCounter != value)
                {
                    this.activeData.RewashCounter = value;
                    HasChanged = true;
                }

            }
        }      

        public decimal WeightCounter
        {
            get
            {
                return this.activeData.WeightCounter;
            }
            set
            {
                if (this.activeData.WeightCounter != value)
                {
                    this.activeData.WeightCounter = value;
                    HasChanged = true;
                }
            }
        }

        public int WeightUnits
        {
            get
            {
                return activeData.WeightUnits;
            }
            set
            {
                if (activeData.WeightUnits != value)
                {
                    activeData.WeightUnits = value;
                    HasChanged = true;
                }
            }
        }

        public int ProductionTime
        {
            get
            {
                return this.activeData.ProductionTime;
            }
            set
            {
                if (this.activeData.ProductionTime != value)
                {
                    this.activeData.ProductionTime = value;
                    HasChanged = true;
                }
            }
        }

        public int StopTime
        {
            get
            {
                return this.activeData.StopTime;
            }
            set
            {
                if (this.activeData.StopTime != value)
                {
                    this.activeData.StopTime = value;
                    HasChanged = true;
                }
            }
        }

        public int NoFlowTime
        {
            get
            {
                return this.activeData.NoFlowTime;
            }
            set
            {
                if (this.activeData.NoFlowTime != value)
                {
                    this.activeData.NoFlowTime = value;
                    HasChanged = true;
                }
            }
        }

        public int FaultTime
        {
            get
            {
                return this.activeData.FaultTime;
            }
            set
            {
                if (this.activeData.FaultTime != value)
                {
                    this.activeData.FaultTime = value;
                    HasChanged = true;
                }
            }
        }

        public decimal FreshWater
        {
            get
            {
                return this.activeData.FreshWater;
            }
            set
            {
                if (this.activeData.FreshWater != value)
                {
                    this.activeData.FreshWater = value;
                    HasChanged = true;
                }
            }
        }

        public decimal WasteWater
        {
            get
            {
                return this.activeData.WasteWater;
            }
            set
            {
                if (this.activeData.WasteWater != value)
                {
                    this.activeData.WasteWater = value;
                    HasChanged = true;
                }
            }
        }

        public int WaterUnits
        {
            get
            {
                return activeData.WaterUnits;
            }
            set
            {
                if (activeData.WaterUnits != value)
                {
                    activeData.WaterUnits = value;
                    HasChanged = true;
                }
            }
        }

        public decimal GasEnergy
        {
            get
            {
                return this.activeData.GasEnergy;
            }
            set
            {
                if (this.activeData.GasEnergy != value)
                {
                    this.activeData.GasEnergy = value;
                    HasChanged = true;
                }
            }
        }

        public int GasEnergyUnits
        {
            get
            {
                return activeData.GasEnergyUnits;
            }
            set
            {
                if (activeData.GasEnergyUnits != value)
                {
                    activeData.GasEnergyUnits = value;
                    HasChanged = true;
                }
            }
        }

        public decimal GasVolume
        {
            get
            {
                return this.activeData.GasVolume;
            }
            set
            {
                if (this.activeData.GasVolume != value)
                {
                    this.activeData.GasVolume = value;
                    HasChanged = true;
                }
            }
        }

        public int GasVolumeUnits
        {
            get
            {
                return activeData.GasVolumeUnits;
            }
            set
            {
                if (activeData.GasVolumeUnits != value)
                {
                    activeData.GasVolumeUnits = value;
                    HasChanged = true;
                }
            }
        }

        public decimal ElectricalEnergy
        {
            get
            {
                return this.activeData.ElectricalEnergy;
            }
            set
            {
                if (this.activeData.ElectricalEnergy != value)
                {
                    this.activeData.ElectricalEnergy = value;
                    HasChanged = true;
                }
            }
        }

        public int ElectricityUnits
        {
            get
            {
                return activeData.ElectricityUnits;
            }
            set
            {
                if (activeData.ElectricityUnits != value)
                {
                    activeData.ElectricityUnits = value;
                    HasChanged = true;
                }
            }
        }

        public decimal SteamConsumption
        {
            get
            {
                return this.activeData.SteamConsumption;
            }
            set
            {
                if (this.activeData.SteamConsumption != value)
                {
                    this.activeData.SteamConsumption = value;
                    HasChanged = true;
                }
            }
        }

        public int SteamUnits
        {
            get
            {
                return activeData.SteamUnits;
            }
            set
            {
                if (activeData.SteamUnits != value)
                {
                    activeData.SteamUnits = value;
                    HasChanged = true;
                }
            }
        }

        public decimal AirConsumption
        {
            get
            {
                return this.activeData.AirConsumption;
            }
            set
            {
                if (this.activeData.AirConsumption != value)
                {
                    this.activeData.AirConsumption = value;
                    HasChanged = true;
                }
            }
        }

        public int AirUnits
        {
            get
            {
                return activeData.AirUnits;
            }
            set
            {
                if (activeData.AirUnits != value)
                {
                    activeData.AirUnits = value;
                    HasChanged = true;
                }
            }
        }

        public bool ShouldStoreData
        {
            get
            {
                bool test = CountExitPointData || ProductionRefData || !ExcludePower;
                return test;
            }
        }

        public bool CountExitPointData
        {
            get
            {
                return MachineCountExitPoint || OperatorCountExitPoint || ProcessCountExitPoint;
            }
            //set
            //{
            //    HasChanged |= this.activeData.CountExitPointData != value;
            //    this.activeData.CountExitPointData = value;
            //}
        }

        public bool MachineCountExitPoint
        {
            get
            {
                return this.activeData.MachineCountExitPoint;
            }
            set
            {
                HasChanged |= this.activeData.MachineCountExitPoint != value;
                this.activeData.MachineCountExitPoint = value;
            }
        }

        public bool OperatorCountExitPoint
        {
            get
            {
                return this.activeData.OperatorCountExitPoint;
            }
            set
            {
                HasChanged |= this.activeData.OperatorCountExitPoint != value;
                this.activeData.OperatorCountExitPoint = value;
            }
        }

        public bool ProcessCountExitPoint
        {
            get
            {
                return this.activeData.ProcessCountExitPoint;
            }
            set
            {
                HasChanged |= this.activeData.ProcessCountExitPoint != value;
                this.activeData.ProcessCountExitPoint = value;
            }
        }

        public bool ExcludePower
        {
            get
            {
                return this.activeData.ExcludePower;
            }
            set
            {
                HasChanged |= this.activeData.ExcludePower != value;
                this.activeData.ExcludePower = value;
            }
        }

        public bool ProductionRefData
        {
            get
            {
                return this.activeData.ProductionRefData;
            }
            set
            {
                HasChanged |= this.activeData.ProductionRefData != value;
                this.activeData.ProductionRefData = value;
            }
        }

   }
    #endregion
}
