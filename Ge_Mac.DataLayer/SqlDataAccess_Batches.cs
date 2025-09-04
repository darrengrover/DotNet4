using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {

        #region settings
        private double gasVolumeTokWh = 10;

        public double GasVolumeTokWh
        {
            get { return gasVolumeTokWh; }
            set { gasVolumeTokWh = value; }
        }

        private int normCalcMode = 0;

        public int NormCalcMode
        {
            get { return normCalcMode; }
            set { normCalcMode = value; }
        }

        private bool combinedProduction = false;

        public bool CombinedProduction
        {
            get { return combinedProduction; }
            set { combinedProduction = value; }
        }
        private bool useProcessNorm = false;

        public bool UseProcessNorm
        {
            get { return useProcessNorm; }
            set { useProcessNorm = value; }
        }

        private bool useActiveSubids = false;

        public bool UseActiveSubids
        {
            get { return useActiveSubids; }
            set { useActiveSubids = value; }
        }


        #endregion

        #region caches
        private BatchDetailsList batchesLightCache = null;
        private Batches batchesCache = null;
        private CompoundBatches compoundBatchesCache = null;

        public void InvalidateBatches()
        {
            if (batchesCache != null)
                batchesCache.IsValid = false;
        }

        private bool BatchesAreCached()
        {
            bool test = (batchesCache != null);
            if (test)
            {
                test = batchesCache.IsValid;
            }
            return test;
        }

        public void InvalidateBatchesLight()
        {
            if (batchesLightCache != null)
                batchesLightCache.IsValid = false;
        }

        private bool BatchesLightAreCached()
        {
            bool test = (batchesLightCache != null);
            if (test)
            {
                test = batchesLightCache.IsValid;
            }
            return test;
        }

        public void InvalidateCompoundBatches()
        {
            if (compoundBatchesCache != null)
                compoundBatchesCache.IsValid = false;
        }

        private bool CompoundBatchesAreCached()
        {
            bool test = (compoundBatchesCache != null);
            if (test)
            {
                test = compoundBatchesCache.IsValid;
            }
            return test;
        }
        
        #endregion

        #region Select Data

        const string allBatchLightCommand =
            @"SELECT	bd.RecNum
			            , bd.Created
	                    , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Article_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , CAST(0 AS SMALLINT) AS MachineID
                        , CAST(0 AS SMALLINT) AS PositionID
                FROM tblBatchDetails bd
	                INNER JOIN tblCustomers c
		                on bd.Customer_idJensen = c.idJensen
	                INNER JOIN tblSortCategory sc
		                on bd.SortCategory_idJensen = sc.idJensen
                    ORDER BY bd.RecNum DESC";

        const string allBatchLightRangeCommand =
            @"SELECT	bd.RecNum
			            , bd.Created
	                    , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Article_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , CAST(0 AS SMALLINT) AS MachineID
                        , CAST(0 AS SMALLINT) AS PositionID
                FROM tblBatchDetails bd, tblCustomers c, tblSortCategory sc,tblJGLogData ld
                WHERE ld.TimeStamp>=@StartTime
                    AND ld.TimeStamp<@EndTime
                    AND bd.BatchID=ld.BatchID
                    AND bd.Customer_idJensen = c.idJensen
                    AND bd.SortCategory_idJensen = sc.idJensen
                GROUP BY bd.recnum,created,bd.sourceid,sourcesubid,bd.batchid,customer_idjensen,sortcategory_idjensen,
                    article_idjensen,weight_kg,articlecount             
                ORDER BY bd.RecNum DESC";

        const string BatchLightCommand =
            @"SELECT	bd.RecNum
			            , bd.Created
	                    , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Article_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , CAST(0 AS SMALLINT) AS MachineID
                        , CAST(0 AS SMALLINT) AS PositionID
                FROM tblBatchDetails bd
	                INNER JOIN tblCustomers c
		                on bd.Customer_idJensen = c.idJensen
	                INNER JOIN tblSortCategory sc
		                on bd.SortCategory_idJensen = sc.idJensen
                WHERE bd.BatchID = @BatchID
                    AND bd.SourceID = @SourceID";

        /*const string allActiveBatchLightCommand =
            @"SELECT	bd.RecNum
		                , bd.Created
		                , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.Article_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , rt.MachineID
                        , rt.PositionID
                FROM tblBatchDetails bd
	                INNER JOIN tblCustomers c
		                ON bd.Customer_idJensen = c.idJensen
	                INNER JOIN tblSortCategory sc
		                ON bd.SortCategory_idJensen = sc.idJensen
	                JOIN tblBatchRealTime rt
		                ON bd.BatchID = rt.BatchID
		                AND bd.SourceID = rt.SourceID";*/

        const string allActiveBatchLightCommand =
            @"SELECT bd.RecNum
		                , bd.Created
		                , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.Article_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , rt.MachineID
                        , rt.PositionID
                FROM tblBatchDetails bd, tblBatchRealTime rt
                WHERE bd.BatchID = rt.BatchID
                    AND bd.SourceID = rt.SourceID
                    AND bd.BatchID <> 0
                    AND bd.SourceID <> 0";
                    //AND bd.Customer_idJensen in (select idjensen from tblCustomers)
                    //AND ((bd.SortCategory_idJensen in (select idjensen from tblSortCategory))
                    //    OR (bd.Article_idJensen in (select idjensen from tblArticles)))";

        
        const string allBatchesCommand =
            @"SELECT ISNULL([plc].[RecNum], 0) AS RecNum_PLC
                   , ISNULL([plc].[SystemID], ISNULL([bdr].[MachineID], 0)) AS SystemID
                   , ISNULL([plc].[BatchID], [bd].[BatchID]) AS BatchID_PLC
                   , ISNULL([plc].[SourceID], [bd].[SourceID]) AS SourceID_PLC
                   , ISNULL([plc].[SourceSubID], [bd].[SourceSubID]) AS SourceSubID_PLC
                   , ISNULL([plc].[StoreLocation], 0) AS StoreLocation
                   , ISNULL([plc].[Grouping], 0) AS Grouping
                   , ISNULL([plc].[DropLocation], 0) AS DropLocation
                   , ISNULL([plc].[Call_ID], 0) AS Call_ID
                   , ISNULL([plc].[OverWeight], 0) AS OverWeight
                   , ISNULL([plc].[BagEmpty], 0) AS BagEmpty
                   , ISNULL([plc].[BagClosed], 0) AS BagClosed
                   , ISNULL([plc].[ReverseBinUnload], 0) AS ReverseBinUnload
                   , ISNULL([plc].[BagDeleted], 0) AS BagDeleted
                   , ISNULL([plc].[Maintenance], 0) AS Maintenance

                   , [bd].[RecNum] AS RecNum_BD
                   , [bd].[Created] AS Created
                   , [bd].[SourceID] AS SourceID_BD
                   , [bd].[SourceSubID] AS SourceSubID_BD
                   , [bd].[BatchID] AS BatchID_BD
                   , [bd].[Customer_idJensen]
 		           , [bd].[Article_idJensen]
                   , [bd].[SortCategory_idJensen]
                   , [bd].[Weight_Kg]
                   , [bd].[ArticleCount]

                   , [bdr].[RecNum] AS RecNum_BDR
                   , [bdr].[MachineID] AS MachineID_BDR
                   , [bdr].[PositionID]
                   , [bdr].[SourceID] AS SourceID_BDR
                   , [bdr].[BatchID] AS BatchID_BDR
              FROM JEGR_DB.dbo.[tblBatchDetails] bd
              LEFT JOIN JEGR_DB.dbo.[tblBatchRealTime] bdr
                      ON bd.BatchID = bdr.BatchID
                     AND bd.SourceID = bdr.SourceID
              LEFT JOIN [dbo].[tblBatchDetails_PLC] plc
                     ON plc.BatchID = bdr.BatchID
                    AND plc.SourceID = bdr.SourceID
                    AND plc.SystemID = bdr.MachineID";

        const string allActiveBatchesCommand =
            @"SELECT ISNULL([plc].[RecNum], 0) AS RecNum_PLC
                   , ISNULL([plc].[SystemID], ISNULL([bdr].[MachineID], 0)) AS SystemID
                   , ISNULL([plc].[BatchID], [bdr].[BatchID]) AS BatchID_PLC
                   , ISNULL([plc].[SourceID], [bd].[SourceID]) AS SourceID_PLC
                   , ISNULL([plc].[SourceSubID], [bd].[SourceSubID]) AS SourceSubID_PLC
                   , ISNULL([plc].[StoreLocation], 0) AS StoreLocation
                   , ISNULL([plc].[Grouping], 0) AS Grouping
                   , ISNULL([plc].[DropLocation], 0) AS DropLocation
                   , ISNULL([plc].[Call_ID], 0) AS Call_ID
                   , ISNULL([plc].[OverWeight], 0) AS OverWeight
                   , ISNULL([plc].[BagEmpty], 0) AS BagEmpty
                   , ISNULL([plc].[BagClosed], 0) AS BagClosed
                   , ISNULL([plc].[ReverseBinUnload], 0) AS ReverseBinUnload
                   , ISNULL([plc].[BagDeleted], 0) AS BagDeleted
                   , ISNULL([plc].[Maintenance], 0) AS Maintenance
                   , ISNULL([plc].[Special], 0) AS Special
                   , ISNULL([plc].[BagNumber], 0) AS BagNumber
                   , ISNULL([plc].[spare], 0) AS spare

                   , [bd].[RecNum] AS RecNum_BD
                   , [bd].[Created] AS Created_BD
                   , [bd].[SourceID] AS SourceID_BD
                   , [bd].[SourceSubID] AS SourceSubID_BD
                   , [bd].[BatchID] AS BatchID_BD
                   , [bd].[Customer_idJensen]
 		           , [bd].[Article_idJensen]
                   , [bd].[SortCategory_idJensen]
                   , [bd].[Weight_Kg]
                   , [bd].[ArticleCount]

                   , [bdr].[RecNum] AS RecNum_BDR
                   , [bdr].[MachineID] AS MachineID_BDR
                   , [bdr].[PositionID]
                   , [bdr].[SourceID] AS SourceID_BDR
                   , [bdr].[BatchID] AS BatchID_BDR
              FROM JEGR_DB.dbo.[tblBatchDetails] bd
              INNER JOIN JEGR_DB.dbo.[tblBatchRealTime] bdr
                      ON bd.BatchID = bdr.BatchID
                     AND bd.SourceID = bdr.SourceID
              LEFT JOIN [dbo].[tblBatchDetails_PLC] plc
                     ON plc.BatchID = bdr.BatchID
                    AND plc.SourceID = bdr.SourceID
                    AND plc.SystemID = bdr.MachineID";



        public Batches GetAllActiveBatches()
        {
            if (BatchesAreCached())
            {
                return batchesCache;
            }
            try
            {
                using (SqlCommand command = new SqlCommand(allActiveBatchesCommand))
                {
                    if (batchesCache == null) batchesCache = new Batches();
                    command.DataFill(batchesCache, SqlDataConnection.DBConnection.Rail);
                    return batchesCache;
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

        public Batches GetVisioBatches()
        {
            if (BatchesAreCached())
            {
                return batchesCache;
            }
            try
            {
                const string visioBatchesCommand =
                                        @"SELECT 0 AS RecNum_PLC
                   ,0 AS SystemID
                   , [bdr].[BatchID] AS BatchID_PLC
                   , [bd].[SourceID] AS SourceID_PLC
                   , [bd].[SourceSubID] AS SourceSubID_PLC
                   , 0 AS StoreLocation
                   , CAST(0 as SMALLINT) AS Grouping
                   , 0 AS DropLocation
                   , 0 AS Call_ID
                   , 0 AS Special
                   , '' AS BagNumber
                   , '' AS Spare
                   , CAST(0 as BIT) AS OverWeight
                   , CAST(0 as BIT) AS BagEmpty
                   , CAST(0 as BIT) AS BagClosed
                   , CAST(0 as BIT) AS ReverseBinUnload
                   , CAST(0 as BIT) AS BagDeleted
                   , CAST(0 as BIT) as Maintenance

                   , [bd].[RecNum] AS RecNum_BD
                   , [bd].[Created] AS Created_BD
                   , [bd].[SourceID] AS SourceID_BD
                   , [bd].[SourceSubID] AS SourceSubID_BD
                   , [bd].[BatchID] AS BatchID_BD
                   , [bd].[Customer_idJensen]
 		           , [bd].[Article_idJensen]
                   , [bd].[SortCategory_idJensen]
                   , [bd].[Weight_Kg]
                   , [bd].[ArticleCount]

                   , [bdr].[RecNum] AS RecNum_BDR
                   , [bdr].[MachineID] AS MachineID_BDR
                   , [bdr].[PositionID]
                   , [bdr].[SourceID] AS SourceID_BDR
                   , [bdr].[BatchID] AS BatchID_BDR
              FROM [dbo].[tblBatchDetails] bd
              INNER JOIN [dbo].[tblBatchRealTime] bdr
                      ON bd.BatchID = bdr.BatchID
                     AND bd.SourceID = bdr.SourceID";

                using (SqlCommand command = new SqlCommand(visioBatchesCommand))
                {
                    if (batchesCache == null) batchesCache = new Batches();
                    command.DataFill(batchesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return batchesCache;
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

        public BatchDetails_PLC GetActiveBatch(int sourceId, int batchId)
        {
            Batches batches = GetAllActiveBatches();
            BatchDetails_PLC batch = batches.GetById(batchId, sourceId);
            return batch;
        }

        public BatchDetails_PLC GetVisioBatch(int sourceId, int batchId)
        {
            Batches batches = GetVisioBatches();
            BatchDetails_PLC batch = batches.GetById(batchId, sourceId);
            return batch;
        }

        public BatchDetails_PLC GetActiveBatch(int systemId, int sourceId, int batchId)
        {
            Batches batches = GetAllActiveBatches();
            BatchDetails_PLC batch = batches.GetById(batchId, sourceId, systemId);
            return batch;
        }

        public BatchDetails_PLC GetActiveBatch(int systemId, int sourceId, int batchId, int positionId)
        {
            Batches batches = GetAllActiveBatches();
            BatchDetails_PLC batch = batches.GetById(batchId, sourceId, systemId, positionId);
            return batch;
        }

        public List<BatchDetails_PLC> GetActiveBatches(int systemId, int sourceId, int batchId)
        {
            Batches batches = GetAllActiveBatches();
            Batches activeBatches = new Batches();
            foreach (BatchDetails_PLC batch in batches)
            {
                if ((batch.SystemID == systemId)
                    && (batch.SourceID == sourceId)
                    && (batch.BatchID == batchId))
                {
                    activeBatches.Add(batch);
                }
            }
            return activeBatches;
        }

        public BatchDetailsList GetBatchesLight()
        {
            try
            {
                const string commandString = allBatchLightCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    BatchDetailsList bdl = new BatchDetailsList();
                    command.DataFill(bdl, SqlDataConnection.DBConnection.JensenGroup);

                    return bdl;
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

        public BatchDetailsList GetBatchesLight(DateTime start, DateTime end)
        {
            try
            {
                const string commandString = allBatchLightRangeCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@StartTime", start);
                    command.Parameters.AddWithValue("@EndTime", end);
                    BatchDetailsList bdl = new BatchDetailsList();
                    command.DataFill(bdl, SqlDataConnection.DBConnection.JensenGroup);

                    return bdl;
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

        private BatchDetailsList FilterValidBatches(BatchDetailsList bdl)
        {
            SqlDataAccess da = SqlDataAccess.singleton;
            Customers cs = da.GetAllActiveCustomers();
            SortCategories scs = da.GetAllActiveSortCategories();
            Articles arts = da.GetAllActiveArticles();
            BatchDetailsList delList = new BatchDetailsList();
            foreach (BatchDetails bd in bdl)
            {
                Customer c = cs.GetById(bd.Customer_idJensen);
                SortCategory sc = scs.GetById(bd.SortCategory_idJensen);
                Article a = arts.GetById(bd.Article_idJensen);
                if ((c == null) || ((sc==null) && (a==null)))
                {
                    delList.Add(bd);
                }
            }
            foreach (BatchDetails bdx in delList)
            {
                bdl.Remove(bdx);
            }
            return bdl;
        }

        public BatchDetailsList GetActiveBatchesLight()
        {
            if (BatchesLightAreCached())
            {
                return batchesLightCache;
            }
            try
            {
                const string commandString = allActiveBatchLightCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (batchesLightCache == null) batchesLightCache = new BatchDetailsList();
                    command.DataFill(batchesLightCache, SqlDataConnection.DBConnection.JensenGroup);
                    //batchesLightCache = FilterValidBatches(batchesLightCache);

                    return batchesLightCache;
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

        public BatchDetailsList GetChildBatches()
        {
            try
            {
                const string commandString = @"SELECT bd.RecNum
		                , bd.Created
		                , bd.SourceID
		                , bd.SourceSubID
		                , bd.BatchID
		                , bd.Customer_idJensen
		                , bd.Article_idJensen
		                , bd.SortCategory_idJensen
		                , bd.Weight_Kg
		                , bd.ArticleCount
                        , rt.MachineID
                        , rt.PositionID
                FROM tblBatchDetails bd, tblBatchRealTime rt, tblBatchCompound bc
                WHERE bc.Parent_BatchID = rt.BatchID
                    AND bc.Parent_SourceID = rt.SourceID
                    AND bc.Child_BatchID=bd.BatchID
                    AND bc.Child_SourceID=bd.SourceID
                    AND bd.BatchID <> 0
                    AND bd.SourceID <> 0"; 

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    BatchDetailsList bdl = new BatchDetailsList();
                    command.DataFill(bdl, SqlDataConnection.DBConnection.JensenGroup);
                    //batchesLightCache = FilterValidBatches(batchesLightCache);

                    return bdl;
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

        public BatchDetails GetBatchLight(int sourceId, int batchId)
        {

            try
            {
                const string commandString = BatchLightCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    BatchDetailsList bdl = new BatchDetailsList();
                    command.Parameters.AddWithValue("@BatchID", batchId);
                    command.Parameters.AddWithValue("@SourceID", sourceId);
                    command.DataFill(bdl, SqlDataConnection.DBConnection.JensenGroup);

                    BatchDetails batchDetails = null;

                    if (bdl.Count > 0)
                    {
                        batchDetails = bdl[bdl.Count-1];
                    }

                    return batchDetails;
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

        public CompoundBatches GetActiveCompoundBatches(bool noCache)
        {
            if (CompoundBatchesAreCached() && !noCache)
            {
                return compoundBatchesCache;
            }
            try
            {
                if (DatabaseVersion >= 1.8)
                {
                    const string commandString = @"SELECT Min(bc.[RecNum])AS RecNum
                                                      ,Min(bc.[Parent_BatchID]) AS Parent_BatchID
                                                      ,Min(bc.[Parent_SourceID]) AS Parent_SourceID
                                                      ,Min(bc.[Child_Index]) AS Child_Index
                                                      ,Min(bc.[Child_BatchID]) AS Child_BatchID
                                                      ,Min(bc.[Child_SourceID]) AS Child_SourceID
                                                  FROM [dbo].[tblBatchCompound] bc, [dbo].[tblBatchRealTime] brt
                                                  WHERE bc.Parent_BatchID=brt.BatchID
	                                                AND bc.Parent_SourceID=brt.SourceID 
	                                              GROUP BY Parent_BatchID, Parent_SourceID, Child_Index, Child_BatchID, Child_SourceID
                                                  ORDER BY Parent_BatchID, Parent_SourceID, Child_Index "; //batches might appear in more than one place!

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (compoundBatchesCache == null) compoundBatchesCache = new CompoundBatches();
                        command.DataFill(compoundBatchesCache, SqlDataConnection.DBConnection.JensenGroup);
                    }
                }
                return compoundBatchesCache;
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

        public int NextBatchRecord(int SourceID)
        {
            const string commandString = @"select max(BatchID) from dbo.tblBatchDetails 
                                            where sourceID = @SourceID";

            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SourceID",SourceID);
                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    if (spResult != null)
                    {
                        if (spResult.ToString() != string.Empty)
                        {
                            nextID = (int)spResult;
                        }
                    }
                }
                nextID++;
            }

            catch (SqlException)
            {
                throw;
            }
            return nextID;
        }

        #endregion

        #region Insert Data

        public Boolean CreateNewBatch(BatchDetails batch)
        {
            const string NewBatchString =
                @"INSERT INTO [dbo].tblBatchDetails
                    (   SourceID
                    ,   SourceSubID
                    ,   BatchID
                    ,   Customer_idJensen
                    ,   SortCategory_idJensen
                    ,   Weight_Kg
                    ,   ArticleCount )
                VALUES (
                        @SourceID
                    ,   @SourceSubID
                    ,   @BatchID
                    ,   @Customer
                    ,   @SortCategory
                    ,   @Weight
                    ,   @ArticleCount) 
                SELECT @@ROWCOUNT";

            try
            {
                using (SqlCommand command = new SqlCommand(NewBatchString))
                {
                    command.Parameters.AddWithValue("@SourceID", batch.SourceID);
                    command.Parameters.AddWithValue("@SourceSubID", batch.SourceSubID);
                    command.Parameters.AddWithValue("@BatchID", batch.BatchID);
                    command.Parameters.AddWithValue("@Customer", batch.Customer_idJensen);
                    command.Parameters.AddWithValue("@SortCategory", batch.SortCategory_idJensen);
                    command.Parameters.AddWithValue("@Weight", batch.Weight_Kg);
                    command.Parameters.AddWithValue("@ArticleCount", batch.ArticleCount);

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    InvalidateBatches();
                    if (RecordCount == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return false;
            }

        }

        public void InsertNewBatch(BatchDetails_PLC batchDetails)
        {
            const string commandString = @"
                  INSERT INTO [dbo].[tblAddedBatches]
                  ( [SystemID]
                  , [PosnID]
                  , [CategID]
                  , [CustID]
                  , [Weight_Kg]
                  , [DropLocation]
                  , [StoreLocation]
                  , [Grouping]
                  , [ArticleCount]
                  , [Overweight]
                  , [EmptyBag]
                  , [BagClosed]
                  , [Maintenance]
                  , [Special]
                  , [BagNumber]
                  , [spare]
                  , [EditDate]
                  )
                  VALUES
                  ( @SystemID
                  , @PositionID
                  , @CategoryID
                  , @CustomerID
                  , @BatchWeight
                  , @DropLocation
                  , @StoreLocation
                  , @Grouping
                  , @ArticleCount
                  , @Overweight
                  , @EmptyBag
                  , @BagClosed
                  , @Maintenance
                  , @Special
                  , @BagNumber
                  , @spare
                  , GETDATE()
                  )";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@SystemID", batchDetails.SystemID);
                command.Parameters.AddWithValue("@PositionID", batchDetails.BatchRealTime.PositionID);
                command.Parameters.AddWithValue("@CategoryID", batchDetails.BatchDetails.SortCategory_idJensen);
                command.Parameters.AddWithValue("@CustomerID", batchDetails.BatchDetails.Customer_idJensen);
                command.Parameters.AddWithValue("@BatchWeight", batchDetails.BatchDetails.Weight_Kg);
                command.Parameters.AddWithValue("@DropLocation", batchDetails.DropLocation);
                command.Parameters.AddWithValue("@StoreLocation", batchDetails.StoreLocation);
                command.Parameters.AddWithValue("@Grouping", batchDetails.Grouping);
                command.Parameters.AddWithValue("@ArticleCount", batchDetails.BatchDetails.ArticleCount);

                command.Parameters.AddWithValue("@Overweight", batchDetails.OverWeight);
                command.Parameters.AddWithValue("@EmptyBag", batchDetails.BagEmpty);
                command.Parameters.AddWithValue("@BagClosed", batchDetails.BagClosed);
                command.Parameters.AddWithValue("@Maintenance", batchDetails.Maintenance);
                command.Parameters.AddWithValue("@Special", batchDetails.Special);
                command.Parameters.AddWithValue("@BagNumber", batchDetails.BagNumber);
                if (batchDetails.spare == null)
                {
                    command.Parameters.AddWithValue("@spare", "");
                }
                else
                {
                    command.Parameters.AddWithValue("@spare", batchDetails.spare);
                }
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }

        public void CreateBatchDetails(BatchDetails batchDetails)
        {
            const string commandString = @"
                INSERT INTO [JEGR_DB].[dbo].[tblBatchDetails]
                           ([Created]
                           ,[SourceID]
                           ,[SourceSubID]
                           ,[BatchID]
                           ,[Customer_idJensen]
                           ,[SortCategory_idJensen]
                           ,[Weight_Kg]
                           ,[ArticleCount]
                           ,[Article_idJensen])
                     VALUES
                           (@Created
                           ,@SourceID
                           ,@SourceSubID
                           ,@BatchID
                           ,@Customerid
                           ,@SortCategoryid
                           ,@BatchWeight
                           ,@ArticleCount
                           ,@Articleid)";
            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@Created", batchDetails.Created);
                command.Parameters.AddWithValue("@SourceId", batchDetails.SourceID);
                command.Parameters.AddWithValue("@SourceSubID", batchDetails.SourceSubID);
                command.Parameters.AddWithValue("@BatchId", batchDetails.BatchID);
                command.Parameters.AddWithValue("@CustomerId", batchDetails.Customer_idJensen);
                command.Parameters.AddWithValue("@SortCategoryId", batchDetails.SortCategory_idJensen);
                command.Parameters.AddWithValue("@BatchWeight", batchDetails.Weight_Kg);
                command.Parameters.AddWithValue("@ArticleCount", batchDetails.ArticleCount);
                command.Parameters.AddWithValue("@Articleid", batchDetails.Article_idJensen);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }

        public void CreateBatchRealtime(BatchRealTime batchRealTime)
        {
            const string commandString = @"
                    INSERT INTO [JEGR_DB].[dbo].[tblBatchRealTime]
                               ([MachineID]
                               ,[PositionID]
                               ,[SourceID]
                               ,[BatchID])
                         VALUES
                               (@MachineID
                               ,@PositionID
                               ,@SourceID
                               ,@BatchID)";
            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@BatchId", batchRealTime.BatchID);
                command.Parameters.AddWithValue("@SourceId", batchRealTime.SourceID);
                command.Parameters.AddWithValue("@MachineID", batchRealTime.MachineID);
                command.Parameters.AddWithValue("@PositionID", batchRealTime.PositionID);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }



        }

        public bool BatchRealTimeExists(int MachineID, int PositionID)
        {
            int test = 0;
            const string commandString =
                @"select count(MachineID) from [dbo].tblBatchRealTime 
                    where MachineID = @MachineID
                    and PositionID = @PositionID";
            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@MachineID", MachineID);
                command.Parameters.AddWithValue("@PositionID", PositionID);

                object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                if (RecordCount != null)
                {
                    test = (int)RecordCount;
                }
            }
            return test > 0;
        }

        public void WriteBatchRealTime(BatchRealTime batchRealTime)
        {
            if (BatchRealTimeExists(batchRealTime.MachineID, batchRealTime.PositionID))
            {
                UpdateBatchRealTime(batchRealTime);
            }
            else
            {
                CreateBatchRealtime(batchRealTime);
            }
        }

        public void CreateBatch(BatchDetails_PLC batchDetails)
        {
            CreateBatchDetails(batchDetails.BatchDetails);
            WriteBatchRealTime(batchDetails.BatchRealTime);
        }

        #endregion

        #region Update Data

        public void UpdateBatchRealTime(BatchRealTime batchRealTime)
        {
            const string commandString =
                @"UPDATE [JEGR_DB].[dbo].[tblBatchRealTime] 
                    SET BatchId = @BatchId
                        ,SourceId = @SourceId
                    WHERE [MachineID] = @MachineID
                  AND   [PositionID] = @PositionID";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@BatchId", batchRealTime.BatchID);
                command.Parameters.AddWithValue("@SourceId", batchRealTime.SourceID);
                command.Parameters.AddWithValue("@MachineID", batchRealTime.MachineID);
                command.Parameters.AddWithValue("@PositionID", batchRealTime.PositionID);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }

        public void UpdateBatchDetails(BatchDetails_PLC batchDetails)
        {
            const string commandString =
                @"EXEC [JEGR_DB].[dbo].[spUpdateBatchDetails] @SortCategoryId, @CustomerId, @BatchWeight,
                        @ArticleCount, @BatchId, @SourceId;

                    IF EXISTS 
	                    (
	                    SELECT 1
	                    FROM [dbo].[tblBatchDetails_PLC]
	                    WHERE [BatchId] = @BatchId
	                    AND   [SourceId] = @SourceId
	                    AND   [SystemId] = @SystemID
	                    )
	                    BEGIN
		                    UPDATE [dbo].[tblBatchDetails_PLC]
		                    SET [StoreLocation] = @StorageLine
		                    , [DropLocation] = @WasherDrop
		                    , [Grouping] = @LoopDrop
		                    , [Overweight] = @Overweight
		                    , [BagEmpty] = @BagEmpty
		                    , [BagClosed] = @BagClosed
                            , [Maintenance] = @Maintenance
                            , [BagNumber] = @BagNumber
		                    WHERE [BatchId] = @BatchId
		                    AND   [SourceId] = @SourceId
		                    AND   [SystemId] = @SystemID
	                    END
	                ELSE
                        BEGIN
		                    INSERT INTO [dbo].[tblBatchDetails_PLC]
		                    (
			                    BatchID,
			                    SourceID,
			                    SystemID,
			                    StoreLocation,
			                    DropLocation,
			                    Grouping,
			                    OverWeight,
			                    BagEmpty,
			                    BagClosed,
                                Maintenance,
                                BagNumber
		                    )
		                    VALUES
		                    (
			                    @BatchId,
			                    @SourceId,
			                    @SystemID,
			                    @StorageLine,
			                    @WasherDrop,
			                    @LoopDrop,
			                    @Overweight,
			                    @BagEmpty,
			                    @BagClosed,
                                @Maintenance,
                                @BagNumber
		                    )
                        END;

                  EXEC [dbo].[spAddEditedBatch] @BatchId, @SystemID, @SourceID";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@BatchId", batchDetails.BatchID);
                command.Parameters.AddWithValue("@SystemId", batchDetails.SystemID);
                command.Parameters.AddWithValue("@SourceId", batchDetails.SourceID);
                command.Parameters.AddWithValue("@CustomerId", batchDetails.BatchDetails.Customer_idJensen);
                command.Parameters.AddWithValue("@SortCategoryId", batchDetails.BatchDetails.SortCategory_idJensen);
                command.Parameters.AddWithValue("@BatchWeight", batchDetails.BatchDetails.Weight_Kg);
                command.Parameters.AddWithValue("@ArticleCount", batchDetails.BatchDetails.ArticleCount);
                //command.Parameters.AddWithValue("@PositionId", batchDetails.BatchRealTime.PositionID);
                command.Parameters.AddWithValue("@StorageLine", batchDetails.StoreLocation);
                command.Parameters.AddWithValue("@WasherDrop", batchDetails.DropLocation);
                command.Parameters.AddWithValue("@LoopDrop", batchDetails.Grouping);
                command.Parameters.AddWithValue("@Overweight", batchDetails.OverWeight);
                command.Parameters.AddWithValue("@BagEmpty", batchDetails.BagEmpty);
                command.Parameters.AddWithValue("@BagClosed", batchDetails.BagClosed);
                command.Parameters.AddWithValue("@Maintenance", batchDetails.Maintenance);
                command.Parameters.AddWithValue("@BagNumber", batchDetails.BagNumber);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }

        public Boolean UpdateRealtimeBatch(BatchRealTime realtimeBatch)
        {
            const string UpdateRealtimeBatchCommand =
                @"IF EXISTS 
                     ( SELECT 1
                       FROM [dbo].tblBatchRealTime
                       WHERE MachineID = @MachineID
                       AND   PositionID = @PositionID
                     )
                     BEGIN
                         UPDATE [dbo].tblBatchRealTime
                         SET SourceID = @SourceID
                           , BatchID = @BatchID
                         WHERE MachineID = @MachineID
                         AND   PositionID = @PositionID
                     END
                  ELSE
                     BEGIN
                         INSERT INTO [dbo].tblBatchRealTime
                         ( MachineID
                         , PositionID
                         , SourceID
                         , BatchID
                         )
                         VALUES
                         ( @MachineID
                         , @PositionID
                         , @SourceID
                         , @BatchID
                         )
                     END

                  SELECT @@ROWCOUNT";

            try
            {
                using (SqlCommand command = new SqlCommand(UpdateRealtimeBatchCommand))
                {
                    command.Parameters.AddWithValue("@SourceID", realtimeBatch.SourceID);
                    command.Parameters.AddWithValue("@BatchID", realtimeBatch.BatchID);
                    command.Parameters.AddWithValue("@MachineID", realtimeBatch.MachineID);
                    command.Parameters.AddWithValue("@PositionID", realtimeBatch.PositionID);

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    InvalidateBatches();
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

        public void UpdateBatchDetails(BatchDetails batchDetails)
        {
            const string commandString = @"
                Update [JEGR_DB].[dbo].[tblBatchDetails]
                    SET [Created] = @Created
                       ,[SourceID] = @SourceID
                       ,[SourceSubID] = @SourceSubID
                       ,[BatchID] = @BatchID
                       ,[Customer_idJensen] = @Customerid
                       ,[SortCategory_idJensen] = @SortCategoryid
                       ,[Weight_Kg] = @BatchWeight
                       ,[ArticleCount] = @ArticleCount
                       ,[Article_idJensen] = @Articleid
                    where batchid=@batchid and Sourceid=@sourceid";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@Created", batchDetails.Created);
                command.Parameters.AddWithValue("@SourceId", batchDetails.SourceID);
                command.Parameters.AddWithValue("@SourceSubID", batchDetails.SourceSubID);
                command.Parameters.AddWithValue("@BatchId", batchDetails.BatchID);
                command.Parameters.AddWithValue("@CustomerId", batchDetails.Customer_idJensen);
                command.Parameters.AddWithValue("@SortCategoryId", batchDetails.SortCategory_idJensen);
                command.Parameters.AddWithValue("@BatchWeight", batchDetails.Weight_Kg);
                command.Parameters.AddWithValue("@ArticleCount", batchDetails.ArticleCount);
                command.Parameters.AddWithValue("@Articleid", batchDetails.Article_idJensen);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }

        #endregion

        #region Delete Data
        public void DeleteBatch(BatchDetails_PLC batchDetails)
        {
            const string commandString = @"
                  INSERT INTO [dbo].[tblEditedBatches]
                  ( [EditDate]
                  , [BagID]
                  , [SystemID]
                  , [SourceID]
                  , [DeleteBatch]
                  )
                  VALUES
                  ( GETDATE()
                  , @BagID
                  , @SystemID
                  , @SourceID
                  , 1
                  )";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@BagID", batchDetails.BatchID);
                command.Parameters.AddWithValue("@SystemID", batchDetails.SystemID);
                command.Parameters.AddWithValue("@SourceID", batchDetails.SourceID);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateBatches();
            }
        }
        #endregion
    }

    #region Data Collection Classes
    public class BatchDetailsList : List<BatchDetails>, IDataFiller
    {
        //private double lifespan = 1.0 / 3600.0; //1 second
        private double lifespan = 1.0 / 60.0; //1 minute
        private string tblName = "tblBatchDetails";
        private string tblName1 = "tblBatchRealTime";
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
                    DateTime aTime = da.TableLastUpdated(tblName);
                    DateTime bTime = da.TableLastUpdated(tblName1);
                    if (bTime > aTime) aTime = bTime;
                    lastDBUpdate = aTime;
                    int x = lastDBUpdate.CompareTo(lastRead);
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
            int RecNumBDPos = dr.GetOrdinal("RecNum");
            int CreatedPos = dr.GetOrdinal("Created");
            int SourceIDBDPos = dr.GetOrdinal("SourceID");
            int SourceSubIDBDPos = dr.GetOrdinal("SourceSubID");
            int BatchIDBDPos = dr.GetOrdinal("BatchID");
            int CustomerIdJensenPos = dr.GetOrdinal("Customer_idJensen");
            int SortCategoryIdJensenPos = dr.GetOrdinal("SortCategory_idJensen");
            int ArticleIdJensenPos = dr.GetOrdinal("Article_idJensen");
            int WeightKgPos = dr.GetOrdinal("Weight_Kg");
            int ArticleCountPos = dr.GetOrdinal("ArticleCount");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int PositionPos = dr.GetOrdinal("PositionID");

            this.Clear();
            while (dr.Read())
            {
               BatchDetails batchDetails = new BatchDetails()
               {
                   RecNum = dr.GetInt32(RecNumBDPos),
                   Created = dr.GetDateTime(CreatedPos),
                   SourceID = dr.GetInt32(SourceIDBDPos),
                   SourceSubID = dr.GetByte(SourceSubIDBDPos),
                   BatchID = dr.GetInt32(BatchIDBDPos),
                   Customer_idJensen = dr.GetInt32(CustomerIdJensenPos),
                   SortCategory_idJensen = dr.GetInt32(SortCategoryIdJensenPos),
                   Article_idJensen = dr.GetInt32(ArticleIdJensenPos),
                   Weight_Kg = dr.GetDecimal(WeightKgPos),
                   ArticleCount = dr.GetInt32(ArticleCountPos),
                   MachineID = dr.GetInt16(MachineIDPos),
                   PositionID = dr.GetInt16(PositionPos)
               };

                // Add to collection
                this.Add(batchDetails);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public BatchDetails GetById(int SourceID, int BatchID)
        {
            return this.Find(delegate(BatchDetails batch)
            {
                return ((batch.BatchID == BatchID) & (batch.SourceID == SourceID));
            });
        }
    }

    public class Batches : List<BatchDetails_PLC>, IDataFiller
    {
        private double lifespan = 1.0 / 1200.0; //5 seconds
        private string tblName = "tblBatchRealtime";
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
            #region Field Positions
            int RecNumPLCPos = dr.GetOrdinal("RecNum_PLC");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int BatchIDPLCPos = dr.GetOrdinal("BatchID_PLC");
            int SourceIDPLCPos = dr.GetOrdinal("SourceID_PLC");
            int SourceSubIDPLCPos = dr.GetOrdinal("SourceSubID_PLC");
            int StoreLocationPos = dr.GetOrdinal("StoreLocation");
            int GroupingPos = dr.GetOrdinal("Grouping");
            int DropLocationPos = dr.GetOrdinal("DropLocation");
            int Call_IDPos = dr.GetOrdinal("Call_ID");
            int OverWeightPos = dr.GetOrdinal("OverWeight");
            int EmptyBagPos = dr.GetOrdinal("BagEmpty");
            int BagClosedPos = dr.GetOrdinal("BagClosed");
            int ReverseBinUnloadPos = dr.GetOrdinal("ReverseBinUnload");
            int BagDeletedPos = dr.GetOrdinal("BagDeleted");
            int MaintenancePos = dr.GetOrdinal("Maintenance");
            int SpecialPos = dr.GetOrdinal("Special");
            int BagNumberPos = dr.GetOrdinal("BagNumber");
            int sparePos = dr.GetOrdinal("spare");

            int RecNumBDPos = dr.GetOrdinal("RecNum_BD");
            int CreatedBDPos = dr.GetOrdinal("Created_BD");
            int SourceIDBDPos = dr.GetOrdinal("SourceID_BD");
            int SourceSubIDBDPos = dr.GetOrdinal("SourceSubID_BD");
            int BatchIDBDPos = dr.GetOrdinal("BatchID_BD");
            int CustomerIdJensenPos = dr.GetOrdinal("Customer_idJensen");
            int SortCategoryIdJensenPos = dr.GetOrdinal("SortCategory_idJensen");
            int ArticleIdJensenPos = dr.GetOrdinal("Article_idJensen");
            int WeightKgPos = dr.GetOrdinal("Weight_Kg");
            int ArticleCountPos = dr.GetOrdinal("ArticleCount");

            int RecNumBDRPos = dr.GetOrdinal("RecNum_BDR");
            int MachineIDBDRPos = dr.GetOrdinal("MachineID_BDR");
            int PositionIDPos = dr.GetOrdinal("PositionID");
            int SourceIDBDRPos = dr.GetOrdinal("SourceID_BDR");
            int BatchIDBDRPos = dr.GetOrdinal("BatchID_BDR");
            #endregion

            this.Clear();
            while (dr.Read())
            {
                BatchDetails batchDetails = new BatchDetails()
                {
                    RecNum = dr.GetInt32(RecNumBDPos),
                    Created = dr.GetDateTime(CreatedBDPos),
                    SourceID = dr.GetInt32(SourceIDBDPos),
                    SourceSubID = dr.GetByte(SourceSubIDBDPos),
                    BatchID = dr.GetInt32(BatchIDBDPos),
                    Customer_idJensen = dr.GetInt32(CustomerIdJensenPos),
                    SortCategory_idJensen = dr.GetInt32(SortCategoryIdJensenPos),
                    Article_idJensen = dr.GetInt32(ArticleIdJensenPos),
                    Weight_Kg = dr.GetDecimal(WeightKgPos),
                    ArticleCount = dr.GetInt32(ArticleCountPos)
                };

                BatchDetails_PLC batch = new BatchDetails_PLC()
                {
                    RecNum = dr.GetInt32(RecNumPLCPos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    BatchID = batchDetails.BatchID,
                    SourceID = batchDetails.SourceID,
                    SourceSubID = batchDetails.SourceSubID,
                    StoreLocation = dr.GetInt32(StoreLocationPos),
                    Grouping = dr.GetInt16(GroupingPos),
                    DropLocation = dr.GetInt32(DropLocationPos),
                    Call_ID = dr.GetInt32(Call_IDPos),
                    OverWeight = dr.GetBoolean(OverWeightPos),
                    BagEmpty = dr.GetBoolean(EmptyBagPos),
                    BagClosed = dr.GetBoolean(BagClosedPos),
                    ReverseBinUnload = dr.GetBoolean(ReverseBinUnloadPos),
                    BagDeleted = dr.GetBoolean(BagDeletedPos),
                    Maintenance = dr.GetBoolean(MaintenancePos),
                    Special = dr.GetInt32(SpecialPos),
                    BagNumber = dr.GetString(BagNumberPos),
                    spare = dr.GetString(sparePos)
                };

                batch.BatchDetails = batchDetails;

                BatchRealTime batchRealTime = new BatchRealTime()
                {
                    RecNum = dr.IsDBNull(RecNumBDRPos) ? 0 : dr.GetInt32(RecNumBDRPos),
                    MachineID = dr.IsDBNull(MachineIDBDRPos) ? 0 : dr.GetInt16(MachineIDBDRPos),
                    PositionID = dr.IsDBNull(PositionIDPos) ? 0 : dr.GetInt16(PositionIDPos),
                    SourceID = batch.SourceID,
                    BatchID = batch.BatchID
                };

                batch.BatchRealTime = batchRealTime;

                // Add to collection
                this.Add(batch);
            }

            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public BatchDetails_PLC GetById(int BatchID, int SourceID)
        {
            return this.Find(delegate(BatchDetails_PLC batch)
            {
                return ((batch.BatchID == BatchID) && (batch.SourceID == SourceID));
            });
        }

        public BatchDetails_PLC GetById(int BatchID, int SourceID, int SystemID)
        {
            return this.Find(delegate(BatchDetails_PLC batch)
            {
                return ((batch.BatchID == BatchID)
                    && (batch.SourceID == SourceID)
                    && (batch.SystemID == SystemID));
            });
        }

        public BatchDetails_PLC GetById(int BatchID, int SourceID, int SystemID, int PositionID)
        {
            return this.Find(delegate(BatchDetails_PLC batch)
            {
                return ((batch.BatchID == BatchID)
                    && (batch.SourceID == SourceID)
                    && (batch.SystemID == SystemID)
                    && (batch.BatchRealTime.PositionID == SystemID));
            });
        }

    }

    public class CompoundBatches : List<BatchCompound>, IDataFiller
    {
        private double lifespan = 1.0 / 1200.0; //5 sec
        private string tblName = "tblBatchCompound";
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
            #region Field Positions
            int RecNumPos = dr.GetOrdinal("RecNum");
            int Parent_BatchIDPos = dr.GetOrdinal("Parent_BatchID");
            int Parent_SourceIDPos = dr.GetOrdinal("Parent_SourceID");
            int Child_IndexPos = dr.GetOrdinal("Child_Index");
            int Child_BatchIDPos = dr.GetOrdinal("Child_BatchID");
            int Child_SourceIDPos = dr.GetOrdinal("Child_SourceID");

            #endregion

            this.Clear();
            while (dr.Read())
            {
                BatchCompound batchCompound = new BatchCompound()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    Parent_BatchID = dr.GetInt32(Parent_BatchIDPos),
                    Parent_SourceID = dr.GetInt32(Parent_SourceIDPos),
                    Child_Index = dr.GetInt32(Child_IndexPos),
                    Child_BatchID = dr.GetInt32(Child_BatchIDPos),
                    Child_SourceID = dr.GetInt32(Child_SourceIDPos)
                };

                this.Add(batchCompound);
            }

            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public BatchCompound GetParent(int BatchID, int SourceID)
        {
            return this.Find(delegate(BatchCompound batch)
            {
                return ((batch.Parent_BatchID == BatchID) && (batch.Parent_SourceID == SourceID));
            });
        }

        public BatchCompound GetChild(int BatchID, int SourceID)
        {
            return this.Find(delegate(BatchCompound batch)
            {
                return ((batch.Child_BatchID == BatchID) && (batch.Child_SourceID == SourceID));
            });
        }

    }
  
    #endregion

    #region Batch Classes
    public class BatchDetails_PLC
    {
        public int  RecNum                 { get; set; }
        public int  SystemID               { get; set; }
        public int  BatchID                { get; set; }
        public int  SourceID               { get; set; }
        public int  SourceSubID            { get; set; }
        public int  StoreLocation          { get; set; }
        public int  Grouping               { get; set; }
        public int  DropLocation           { get; set; }
        public int  Call_ID                { get; set; }
        public bool OverWeight             { get; set; }
        public bool BagEmpty               { get; set; }
        public bool BagClosed              { get; set; }
        public bool ReverseBinUnload       { get; set; }
        public bool BagDeleted             { get; set; }
        public bool Maintenance            { get; set; }
        public int Special                 { get; set; }
        public string BagNumber            { get; set; }
        public string spare                { get; set; }
        public BatchDetails BatchDetails   { get; set; }
        public BatchRealTime BatchRealTime { get; set; }

        public static BatchDetails_PLC CreateBatchDetails(int batchId)
        {
            BatchDetails_PLC batch = new BatchDetails_PLC()
            {
                BatchID = batchId,
                BatchDetails = new BatchDetails()
                {
                    BatchID = batchId
                },
                BatchRealTime = new BatchRealTime()
                {
                    BatchID = batchId
                }
            };

            return batch;
        }
    }

    public class BatchDetails
    {
        public int RecNum                   { get; set; }
        public DateTime Created             { get; set; }
        public int SourceID                 { get; set; }
        public int SourceSubID              { get; set; }
        public int BatchID                  { get; set; }
        public int Customer_idJensen        { get; set; }
        public int SortCategory_idJensen    { get; set; }
        public int Article_idJensen         { get; set; }
        public decimal Weight_Kg            { get; set; }
        public int ArticleCount             { get; set; }
        public int MachineID                { get; set; }
        public int PositionID               { get; set; }
    }

    public class BatchRealTime
    {
        public int RecNum     { get; set; }
        public int MachineID  { get; set; }
        public int PositionID { get; set; }
        public int SourceID   { get; set; }
        public int BatchID    { get; set; }

        private bool testedCompound = false;
        private BatchCompound batchCompound = null;

        #region Constructors

        /// <summary>
        /// Creates an emtpy class
        /// </summary>
        public BatchRealTime()
        {

        }

        /// <summary>
        /// Creates a realtime batch class with core data
        /// </summary>
        /// <param name="MachineID">The machine in which the batch currently exists</param>
        /// <param name="PositionID">The position within the machine</param>
        /// <param name="SourceID">The source of the batch</param>
        /// <param name="BatchID">The batchID</param>
        public BatchRealTime(int MachineID, int PositionID, int SourceID, int BatchID)
        {
            this.MachineID = MachineID;
            this.PositionID = PositionID;
            this.SourceID = SourceID;
            this.BatchID = BatchID;
        }

        /// <summary>
        /// Creates a realtime batch class with full data
        /// </summary>
        /// <param name="RecNum">The unique record entry in the database</param>
        /// <param name="MachineID">The machine in which the batch currently exists</param>
        /// <param name="PositionID">The position within the machine</param>
        /// <param name="SourceID">The source of the batch</param>
        /// <param name="BatchID">The batchID</param>
        public BatchRealTime(int RecNum, int MachineID, int PositionID, int SourceID, int BatchID)
            : this(MachineID, PositionID, SourceID, BatchID)
        {
            this.RecNum = RecNum;
        }
        #endregion

        #region compound batches

        public bool IsCompoundParent
        {
            get
            {
                if (!testedCompound)
                    GetCompoundStatus();
                return (batchCompound != null);
            }
        }

        private void GetCompoundStatus()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            CompoundBatches cbs = da.GetActiveCompoundBatches(false);
            if (cbs != null)
            {
                batchCompound = cbs.GetParent(BatchID, SourceID);
            }
            testedCompound = true;
        }

        public int ChildBatchCount
        {
            get
            {
                int aCount = 0;
                if (IsCompoundParent)
                {
                    CompoundBatches cbs = GetChildBatches();
                    if (cbs != null)
                        aCount = cbs.Count;
                }
                return aCount;
            }
        }

        public CompoundBatches GetChildBatches()
        {
            CompoundBatches cbs = null;
            if (IsCompoundParent)
            {
                cbs = new CompoundBatches();
                SqlDataAccess da = SqlDataAccess.Singleton;
                CompoundBatches acbs = da.GetActiveCompoundBatches(false);
                if (acbs != null)
                {
                    foreach (BatchCompound bc in acbs)
                    {
                        if ((bc.Parent_BatchID == BatchID) && (bc.Parent_SourceID == SourceID))
                        {
                            cbs.Add(bc);
                        }
                    }
                }
            }
            return cbs;
        }
        #endregion
    }

    public class BatchCompound : DataItem, INotifyPropertyChanged
    {
        #region BatchCompound Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int Parent_BatchID;
            internal int Parent_SourceID;
            internal int Child_Index;
            internal int Child_BatchID;
            internal int Child_SourceID;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public BatchCompound()
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

        public int RecNum
        {
            get
            {
                return activeData.RecNum;
            }
            set
            {
                if (activeData.RecNum != value)
                {
                    activeData.RecNum = value;
                    NotifyPropertyChanged("RecNum");
                }
            }
        }

        public override int ID
        {
            get
            {
                return RecNum;
            }
            set
            {
                if (RecNum != value)
                {
                    RecNum = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int Parent_BatchID
        {
            get
            {
                return activeData.Parent_BatchID;
            }
            set
            {
                if (activeData.Parent_BatchID != value)
                {
                    activeData.Parent_BatchID = value;
                    NotifyPropertyChanged("Parent_BatchID");
                }
            }
        }

        public int Parent_SourceID
        {
            get
            {
                return activeData.Parent_SourceID;
            }
            set
            {
                if (activeData.Parent_SourceID != value)
                {
                    activeData.Parent_SourceID = value;
                    NotifyPropertyChanged("Parent_SourceID");
                }
            }
        }

        public int Child_Index
        {
            get
            {
                return activeData.Child_Index;
            }
            set
            {
                if (activeData.Child_Index != value)
                {
                    activeData.Child_Index = value;
                    NotifyPropertyChanged("Child_Index");
                }
            }
        }

        public int Child_BatchID
        {
            get
            {
                return activeData.Child_BatchID;
            }
            set
            {
                if (activeData.Child_BatchID != value)
                {
                    activeData.Child_BatchID = value;
                    NotifyPropertyChanged("Child_BatchID");
                }
            }
        }

        public int Child_SourceID
        {
            get
            {
                return activeData.Child_SourceID;
            }
            set
            {
                if (activeData.Child_SourceID != value)
                {
                    activeData.Child_SourceID = value;
                    NotifyPropertyChanged("Child_SourceID");
                }
            }
        }

        #endregion

        private BatchDetails batchDetails = null;

        public Customer ChildCustomer
        {
            get
            {
                Customer c = null;
                GetBatchDetail();
                if (batchDetails != null)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    c = da.GetCustomer(batchDetails.Customer_idJensen);
                }
                return c;
            }
        }

        public decimal ChildWeight
        {
            get
            {
                decimal w = 0;
                GetBatchDetail();
                if (batchDetails != null)
                {
                    w = batchDetails.Weight_Kg;
                }
                return w;
            }
        }

        public int ChildArticles
        {
            get
            {
                int a = 0;
                GetBatchDetail();
                if (batchDetails != null)
                {
                    a = batchDetails.ArticleCount;
                }
                return a;
            }
        }

        private void GetBatchDetail()
        {
            if (batchDetails == null)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                BatchDetailsList bdl = da.GetChildBatches();
                if (bdl != null)
                {
                    batchDetails = bdl.GetById(Child_SourceID, Child_BatchID);
                }
            }
        }
    }
    
    #endregion
}
