using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data
        private PrintRequests printRequestsCache = null;

        public void InvalidatePrintRequests()
        {
            if (printRequestsCache != null)
                printRequestsCache.IsValid = false;
        }

        private bool PrintRequestsAreCached()
        {
            bool test = (printRequestsCache != null);
            if (test)
            {
                test = printRequestsCache.IsValid;
            }
            return test;
        }

        const string CompletePrintRequestCommand =
            @" UPDATE dbo.tblPrintRequest
                SET PrintCompletedTime = GETDATE(), PrintComplete=1
                WHERE RecNum = @RecNum";

        const string AllPrintRequestsCommand =
                @"SELECT TOP 10 [RecNum]
                       , [SourceMachine]
                       , [DeviceDestination]
                       , [FormatID]
                       , [PrintRequestedTime]
                       , [PrintCompletedTime]
                       , [PrintComplete]
                       , [BatchSourceID]
                       , [BatchID]
                  FROM [dbo].[tblPrintRequest]
                  WHERE PrintCompletedTime IS NULL
                    AND BatchID >= @MinBatchID
                    AND PrintRequestedTime > DATEADD(MINUTE, -30, GETDATE())";

        /// <summary>
        /// Returns only print requests entered in the last 30 minutes.
        /// all other requests will be ignored
        /// </summary>
        /// <returns></returns>
        public PrintRequests GetPrintRequests30mins(int MinBatchID)
        {
            try
            {
                const string commandString = AllPrintRequestsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (printRequestsCache == null) printRequestsCache = new PrintRequests();
                    command.Parameters.AddWithValue("@MinBatchID", MinBatchID);
                    command.DataFill(printRequestsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return printRequestsCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Marks the print request as complete
        /// </summary>
        /// <param name="printRequest"></param>
        /// <returns></returns>
        public bool PrintComplete(PrintRequest printRequest)
        {
            const string commandString = CompletePrintRequestCommand;

            try
            {
                using(SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printRequest.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    Debug.WriteLine("Print completed");
                }
                return true;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                Debug.WriteLine(ex.Message);
                return false;
                throw;
            }
        }

        #endregion

        #region Insert Data
        /// <summary>
        /// Generates a print request into the database
        /// </summary>
        /// <param name="printRequest">Print request class</param>
        /// <returns></returns>
        public Boolean CreatePrintRequest(PrintRequest printRequest)
        {
            const string PrintRequestString =
                @"INSERT INTO [dbo].tblPrintRequest
                  ( SourceMachine
                  , DeviceDestination
                  , FormatID
                  , BatchSourceID
                  , BatchID
                  )
                  VALUES
                  ( @SourceMachine
                  , @DeviceDestination
                  , @FormatID
                  , @BatchSourceID
                  , @BatchID
                  )

                  SELECT @@ROWCOUNT";
            try
            {
                using (SqlCommand command = new SqlCommand(PrintRequestString))
                {
                    command.Parameters.AddWithValue("@SourceMachine", printRequest.SourceMachine);
                    command.Parameters.AddWithValue("@DeviceDestination", printRequest.DeviceDestination);
                    command.Parameters.AddWithValue("@FormatID", printRequest.FormatID);
                    command.Parameters.AddWithValue("@BatchSourceID", printRequest.BatchSourceID);
                    command.Parameters.AddWithValue("@BatchID", printRequest.BatchID);

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    InvalidatePrintRequests();
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
        #endregion

        #region Update Data

        #endregion

        #region Delete Data

        #endregion
    }
        
    #region Data Collection Class
    public class PrintRequests : List<PrintRequest>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblPrintRequest";
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
            int SourceMachinePos = dr.GetOrdinal("SourceMachine");
            int DeviceDestinationPos = dr.GetOrdinal("DeviceDestination");
            int FormatIDPos = dr.GetOrdinal("FormatID");
            int PrintRequestedTimePos = dr.GetOrdinal("PrintRequestedTime");
            int PrintCompletedTimePos = dr.GetOrdinal("PrintCompletedTime");
            int PrintCompletePos = dr.GetOrdinal("PrintComplete");
            int BatchSourceIDPos = dr.GetOrdinal("BatchSourceID");
            int BatchIDPos = dr.GetOrdinal("BatchID");
            #endregion

            this.Clear();
            while (dr.Read())
            {
                PrintRequest request = new PrintRequest()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    SourceMachine = dr.GetInt16(SourceMachinePos),
                    DeviceDestination = dr.GetInt16(DeviceDestinationPos),
                    FormatID = dr.GetInt16(FormatIDPos),
                    PrintRequestedTime = dr.GetDateTime(PrintRequestedTimePos),
                    PrintCompletedTime = dr.IsDBNull(PrintCompletedTimePos) ? DateTime.MinValue : dr.GetDateTime(PrintCompletedTimePos),
                    PrintComplete = dr.GetBoolean(PrintCompletePos),
                    BatchSourceID = dr.IsDBNull(BatchSourceIDPos) ? -1 : dr.GetInt32(BatchSourceIDPos),
                    BatchID = dr.GetInt32(BatchIDPos)
                };

                // Add to collection
                this.Add(request);
            }
            isValid = true;
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            return this.Count;
        }
    }
    #endregion
    

    #region Print Request Class
    public class PrintRequest
    {
        public int RecNum { get; set; }
        public int SourceMachine { get; set; }
        public int DeviceDestination { get; set; }
        public int FormatID { get; set; }
        public DateTime PrintRequestedTime { get; set; }
        public DateTime PrintCompletedTime { get; set; }
        public bool PrintComplete { get; set; }
        public int BatchSourceID { get; set; }
        public int BatchID { get; set; }

        #region Constructors
        /// <summary>
        /// Creates a print request class
        /// </summary>
        public PrintRequest()
        {
        }

        /// <summary>
        /// Creates a print request class
        /// </summary>
        /// <param name="SourceMachine">The machine that requested the print</param>
        /// <param name="DeviceDestination">The destination at which the print is top take place</param>
        /// <param name="PrintFormat">the format of the print</param>
        /// <param name="BatchSourceID">The source ID of the batch to print</param>
        /// <param name="BatchID">The batchID of the batch to print</param>
        public PrintRequest(int sourceMachine, int deviceDestination, int formatID, int batchSourceID, int batchID)
        {
            SourceMachine = sourceMachine;
            DeviceDestination = deviceDestination;
            FormatID = formatID;
            BatchSourceID = batchSourceID;
            BatchID = batchID;
        }

        /// <summary>
        /// Creates a print request class
        /// </summary>
        /// <param name="SourceMachine">The machine that requested the print</param>
        /// <param name="DeviceDestination">The destination at which the print is top take place</param>
        /// <param name="PrintFormat">the format of the print</param>
        /// <param name="BatchSourceID">The source ID of the batch to print</param>
        /// <param name="BatchID">The batchID of the batch to print</param>
        /// <param name="PrintRequestedTime">The time at which the print was requested</param>
        /// <param name="PrintCompletedTime">The time at which the print was completed</param>
        public PrintRequest(int sourceMachine, int deviceDestination, int printFormat,
            int batchSourceID, int batchID, DateTime printRequestedTime, DateTime printCompletedTime)
            : this(sourceMachine, deviceDestination, printFormat, batchSourceID, batchID)
        {
            PrintRequestedTime = printRequestedTime;
            PrintCompletedTime = printCompletedTime;
        }
        #endregion
    }
    #endregion
}
