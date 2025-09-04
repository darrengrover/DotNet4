using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region BatchUpdate
        public int GetLastBatchUpdateId()
        {
            const string commandString =
                @"SELECT TOP 1 [bu].[RecNum]
                  FROM [dbo].[tblBatchUpdates] bu
                  ORDER BY RecNum DESC";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    int id = (int?)command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup) ?? 0;
                    return id;
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

        public BatchUpdates GetBatchUpdateDetails(int startRec, int endRec)
        {
            try
            {
                const string commandString = @"
                    SELECT RecNum
                         , EditDate
                         , SystemID
                         , SourceID
                         , BatchID
                         , DeleteBatch
                    FROM [dbo].[tblBatchUpdates]
                    WHERE RecNum BETWEEN @StartRec AND @EndRec";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@StartRec", startRec + 1);
                    command.Parameters.AddWithValue("@EndRec", endRec);
                    BatchUpdates batches = new BatchUpdates();
                    command.DataFill(batches, SqlDataConnection.DBConnection.JensenGroup);
                    return batches;
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

        #region CFUpdate
        public int GetLastCFUpdateId()
        {
            const string commandString =
                @"SELECT TOP 1 [cfu].[RecNum]
                  FROM [dbo].[tblCFUpdate] cfu
                  ORDER BY RecNum DESC";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    int id = (int?)command.ExecuteScalar(SqlDataConnection.DBConnection.Rail) ?? 0;
                    return id;
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

        public CFUpdates GetCFUpdateDetails(int startRec, int endRec)
        {
            try
            {
                const string commandString = @"
                    SELECT RecNum
                         , SystemID
                         , SortCategory_Names
                         , SortCategory_Data
                         , SortStation
                         , Customer
                         , RequestedUpdateTime
                         , CompletedUpdateTime
                    FROM [dbo].[tblCFUpdate]
                    WHERE RecNum BETWEEN @StartRec AND @EndRec";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@StartRec", startRec + 1);
                    command.Parameters.AddWithValue("@EndRec", endRec);
                    CFUpdates cfUpdates = new CFUpdates();
                    command.DataFill(cfUpdates, SqlDataConnection.DBConnection.Rail);
                    return cfUpdates;
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

        #region SequenceUpdate
        public int GetLastSequenceUpdateId()
        {
            const string commandString =
                @"SELECT TOP 1 [cfu].[RecNum]
                  FROM [dbo].[tblSequenceUpdates] cfu
                  ORDER BY RecNum DESC";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    int id = (int?)command.ExecuteScalar(SqlDataConnection.DBConnection.Rail) ?? 0;
                    return id;
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

        public SequenceUpdates GetSequenceUpdateDetails(int startRec, int endRec)
        {
            try
            {
                const string commandString = @"
                    SELECT RecNum
                         , EditDate
                         , SystemID
                         , ID
                         , Type
                    FROM [dbo].[tblSequenceUpdates]
                    WHERE RecNum BETWEEN @StartRec AND @EndRec";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@StartRec", startRec + 1);
                    command.Parameters.AddWithValue("@EndRec", endRec);
                    SequenceUpdates sequenceUpdates = new SequenceUpdates();
                    command.DataFill(sequenceUpdates, SqlDataConnection.DBConnection.Rail);
                    return sequenceUpdates;
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
    public class BatchUpdates : List<BatchUpdate>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            #region Field Positions
            int RecNumPos = dr.GetOrdinal("RecNum");
            int EditDatePos = dr.GetOrdinal("EditDate");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int SourceIDPos = dr.GetOrdinal("SourceID");
            int BatchIDPos = dr.GetOrdinal("BatchID");
            int DeleteBatchPos = dr.GetOrdinal("DeleteBatch");
            #endregion

            while (dr.Read())
            {
                BatchUpdate batchUpdate = new BatchUpdate()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    EditDate = dr.GetDateTime(EditDatePos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    SourceID = dr.GetInt32(SourceIDPos),
                    BatchID = dr.GetInt32(BatchIDPos),
                    DeleteBatch = dr.GetBoolean(DeleteBatchPos)
                };

                // Add to sort categories collection
                this.Add(batchUpdate);
            }

            return this.Count;
        }
    }

    public class CFUpdates : List<CFUpdate>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            #region Field Positions
            int RecNumPos = dr.GetOrdinal("RecNum");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int SortCategory_NamesPos = dr.GetOrdinal("SortCategory_Names");
            int SortCategory_DataPos = dr.GetOrdinal("SortCategory_Data");
            int SortStationPos = dr.GetOrdinal("SortStation");
            int CustomerPos = dr.GetOrdinal("Customer");
            int RequestedUpdateTimePos = dr.GetOrdinal("RequestedUpdateTime");
            int CompletedUpdateTimePos = dr.GetOrdinal("CompletedUpdateTime");
            #endregion

            while (dr.Read())
            {
                CFUpdate cfUpdate = new CFUpdate()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    SortCategory_Names = dr.GetInt16(SortCategory_NamesPos),
                    SortCategory_Data = dr.GetInt16(SortCategory_DataPos),
                    SortStation = dr.GetInt32(SortStationPos),
                    Customer = dr.GetInt32(CustomerPos),
                    RequestedUpdateTime = dr.GetDateTime(RequestedUpdateTimePos),
                    CompletedUpdateTime = !dr.IsDBNull(CompletedUpdateTimePos) ? (DateTime?)dr.GetDateTime(CompletedUpdateTimePos) : null
                };

                // Add to sort categories collection
                this.Add(cfUpdate);
            }

            return this.Count;
        }
    }

    public class SequenceUpdates : List<SequenceUpdate>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            #region Field Positions
            int RecNumPos = dr.GetOrdinal("RecNum");
            int EditDatePos = dr.GetOrdinal("EditDate");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int IDPos = dr.GetOrdinal("ID");
            int TypePos = dr.GetOrdinal("Type");
            #endregion

            while (dr.Read())
            {
                SequenceUpdate sequenceUpdate = new SequenceUpdate()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    EditDate = dr.GetDateTime(EditDatePos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    ID = dr.GetInt32(IDPos),
                    Type = dr.GetInt32(TypePos)
                };

                // Add to sort categories collection
                this.Add(sequenceUpdate);
            }

            return this.Count;
        }
    }
    #endregion

    #region Item Class
    public class BatchUpdate
    {
        public int RecNum { get; set; }
        public DateTime EditDate { get; set; }
        public int SystemID { get; set; }
        public int SourceID { get; set; }
        public int BatchID { get; set; }
        public bool DeleteBatch { get; set; }
    }

    public class CFUpdate
    {
        public int RecNum { get; set; }
        public int SystemID { get; set; }
        public short SortCategory_Names { get; set; }
        public short SortCategory_Data { get; set; }
        public int SortStation { get; set; }
        public int Customer { get; set; }
        public DateTime RequestedUpdateTime { get; set; }
        public DateTime? CompletedUpdateTime { get; set; }
    }

    public class SequenceUpdate
    {
        public int RecNum { get; set; }
        public DateTime EditDate { get; set; }
        public int SystemID { get; set; }
        public int ID { get; set; }
        public int Type { get; set; }
    }
    #endregion
}
