using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data

        private PrinterCodes printerCodesCache = null;

        public void InvalidatePrinterCodes()
        {
            if (printerCodesCache != null)
                printerCodesCache.IsValid = false;
        }

        private bool PrinterCodesAreCached()
        {
            bool test = (printerCodesCache != null);
            if (test)
            {
                test = printerCodesCache.IsValid;
            }
            return test;
        }

        const string allPrinterCodesCommand =
            @"SELECT [CodeID]
                   , [CodeName]
                   , [CodeDesc]
                   , [CodeSequence]
                   , [PrinterType]
             FROM [dbo].[tblPrinterCodes]
                ORDER BY CodeID";

        public PrinterCodes GetPrinterCodes(string printerType)
        {
            PrinterCodes pcs = GetAllPrinterCodes();
            PrinterCodes pcsPrinter = new PrinterCodes();
            foreach (PrinterCode pc in pcs)
            {
                if (pc.PrinterType == printerType)
                {
                    pcsPrinter.Add(pc);
                }
            }
            return pcsPrinter;
        }

        public PrinterCodes GetAllPrinterCodes()
        {
            if (PrinterCodesAreCached())
            {
                return printerCodesCache;
            }
       
            try
            {
                const string commandString = allPrinterCodesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (printerCodesCache == null) printerCodesCache = new PrinterCodes();
                    command.DataFill(printerCodesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return printerCodesCache;
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

        #endregion

        #region Next Record

        public int NextPrinterCodeRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblPrinterCodes',
		                                            @idName = N'CodeID'
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

        #endregion

        #region Insert Data
        public void InsertNewPrinterCode(PrinterCode printerCode)
        {
            const string commandString =
                  @"INSERT INTO [dbo].[tblPrinterCodes]
                                ([CodeID]
                                ,[CodeName]
                                ,[CodeDesc]
                                ,[CodeSequence])
                                , [PrinterType]
                                VALUES
                                (@CodeID
                                ,@CodeName
                                ,@CodeDesc
                                ,@CodeSequence)       
                                , @PrinterType
                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@CodeID", printerCode.CodeID);
                    command.Parameters.AddWithValue("@CodeName", printerCode.CodeName);
                    command.Parameters.AddWithValue("@CodeDesc", printerCode.CodeDesc);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    byte[] b = encoding.GetBytes(printerCode.CodeSequence);
                    command.Parameters.AddWithValue("@CodeSequence", b);
                    command.Parameters.AddWithValue("@PrinterType", printerCode.PrinterType);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            printerCode.CodeID = (int)RecNum;
                            printerCode.HasChanged = false;
                        }
                        InvalidatePrinterCodes();
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
        public void UpdatePrinterDetails(PrinterCode printerCode)
        {
            const string commandString =
                @"UPDATE [dbo].[tblPrinterCodes]
                  SET CodeName = @CodeName
                    , CodeDesc = @CodeDesc
                    , CodeSequence = @CodeSequence
                    , PrinterType = @PrinterType
                  WHERE CodeID = @CodeID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@CodeID", printerCode.CodeID);
                    command.Parameters.AddWithValue("@CodeName", printerCode.CodeName);
                    command.Parameters.AddWithValue("@CodeDesc", printerCode.CodeDesc);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    byte[] b = encoding.GetBytes(printerCode.CodeSequence);
                    command.Parameters.AddWithValue("@CodeSequence", b);
                    command.Parameters.AddWithValue("@PrinterType", printerCode.PrinterType);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    printerCode.HasChanged = false;
                }
                InvalidatePrinterCodes();
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
        /// Delete a printer permanently from the database.
        /// </summary>
        /// <param name="printer"></param>
        public void DeletePrinterDetails(PrinterCode printerCode)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblPrinterCodes]
                  WHERE CodeID = @CodeID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printerCode.CodeID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidatePrinterCodes();
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
    public class PrinterCodes : List<PrinterCode>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblPrinterCodes";
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
            int CodeIDPos = dr.GetOrdinal("CodeID");
            int CodeNamePos = dr.GetOrdinal("CodeName");
            int CodeDescPos = dr.GetOrdinal("CodeDesc");
            int CodeSequencePos = dr.GetOrdinal("CodeSequence");
            int PrinterTypePos = dr.GetOrdinal("PrinterType");

            this.Clear();
            while (dr.Read())
            {
                SqlBytes bytes = dr.GetSqlBytes(CodeSequencePos);
                PrinterCode printerCode = new PrinterCode()
                {
                    CodeID = dr.GetInt32(CodeIDPos),
                    CodeName = dr.GetString(CodeNamePos),
                    CodeDesc = dr.GetString(CodeDescPos),
                    CodeSequence = System.Text.Encoding.ASCII.GetString(bytes.Buffer),
                    PrinterType = dr.GetString(PrinterTypePos),
                    HasChanged = false
                };

                this.Add(printerCode);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public PrinterCode GetById(int id)
        {
            return this.Find(delegate(PrinterCode printerCode)
            {
                return printerCode.CodeID == id;
            });
        }

        public PrinterCode GetByName(string aName)
        {
            return this.Find(delegate(PrinterCode printerCode)
            {
                return printerCode.CodeName == aName;
            });
        }
    }
    #endregion

    #region Item Class
    public class PrinterCode : DataItem
    {
        #region PrinterCode Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int CodeID;
            internal string CodeName;
            internal string CodeDesc;
            internal string CodeSequence;
            internal string PrinterType;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public PrinterCode()
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
                return this.activeData.CodeID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.CodeID != -1;
            }
        }
        #endregion

        #region Data Column Properties
        /*public override int PrimaryKey
        {
            get
            {
                return this.activeData.CodeID;
            }
            set
            {
                this.activeData.CodeID = value;
            }
        }*/

        public int CodeID
        {
            get
            {
                return this.activeData.CodeID;
            }
            set
            {
                this.activeData.CodeID = value;
                HasChanged = true;
            }
        }

        public string CodeName
        {
            get
            {
                return this.activeData.CodeName.Trim();
            }
            set
            {
                this.activeData.CodeName = value;
                HasChanged = true;
            }
        }

        public string CodeDesc
        {
            get
            {
                return this.activeData.CodeDesc;
            }
            set
            {
                this.activeData.CodeDesc = value;
                HasChanged = true;
            }
        }

        public string CodeSequence
        {
            get
            {
                return this.activeData.CodeSequence;
            }
            set
            {
                this.activeData.CodeSequence = value;
                HasChanged = true;
            }
        }

        public string PrinterType
        {
            get
            {
                return this.activeData.PrinterType;
            }
            set
            {
                this.activeData.PrinterType = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
