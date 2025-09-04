using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data
        private Printers printersCache = null;

        public void InvalidatePrinters()
        {
            if (printersCache != null)
                printersCache.IsValid = false;
        }

        private bool PrintersAreCached()
        {
            bool test = (printersCache != null);
            if (test)
            {
                test = printersCache.IsValid;
            }
            return test;
        }
        const string allPrintersCommand =
            @"SELECT [RecNum]
                   , [idJensen]
                   , [PrinterName]
                   , [PrinterDescription]
                   , [PrinterIP]
                   , [PrinterPort]
                   , [PrinterType]
              FROM [dbo].[tblPrinters]";

        public Printers GetAllPrinters()
        {
            if (PrintersAreCached())
            {
                return printersCache;
            }
            try
            {
                const string commandString = allPrintersCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (printersCache == null) printersCache = new Printers();
                    command.DataFill(printersCache, SqlDataConnection.DBConnection.JensenGroup);
                    return printersCache;
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

        public Printer GetPrinter(int idjensen)
        {
            Printers printers = GetAllPrinters();
            Printer printer = printers.GetById(idjensen);
            return printer;
        }

        #endregion

        #region Next Record

        public int NextPrinterRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblPrinters";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblPrinters',
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

        #endregion

        #region Insert Data
        public void InsertNewPrinter(Printer printer)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblPrinters]
                  ( [idJensen]
                  , [PrinterName]
                  , [PrinterDescription]
                  , [PrinterIP]
                  , [PrinterPort]
                  , [PrinterType]
                 )
                  VALUES
                  ( @idJensen
                  , @PrinterName
                  , @PrinterDescription
                  , @PrinterIP
                  , @PrinterPort
                  , @PrinterType
                 )
        
                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", printer.idJensen);
                    command.Parameters.AddWithValue("@PrinterName", printer.PrinterName);

                    SqlParameter p = command.Parameters.Add("@PrinterDescription", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = printer.PrinterDescription == null ? DBNull.Value : (object)printer.PrinterDescription;

                    p = command.Parameters.Add("@PrinterIP", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = printer.PrinterIP == null ? DBNull.Value : (object)printer.PrinterIP;

                    command.Parameters.AddWithValue("@PrinterPort", printer.PrinterPort);
                    command.Parameters.AddWithValue("@PrinterType", printer.PrinterType);
                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            printer.RecNum = (int)RecNum;
                            printer.HasChanged = false;
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
                    InvalidatePrinters();
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
        public void UpdatePrinterDetails(Printer printer)
        {
            const string commandString =
                @"UPDATE [dbo].[tblPrinters]
                  SET idJensen = @idJensen
                    , PrinterName = @PrinterName
                    , PrinterDescription = @PrinterDescription
                    , PrinterIP = @PrinterIP
                    , PrinterPort = @PrinterPort
                    , PrinterType = @PrinterType
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printer.RecNum);
                    command.Parameters.AddWithValue("@idJensen", printer.idJensen);
                    command.Parameters.AddWithValue("@PrinterName", printer.PrinterName);
                    command.Parameters.AddWithValue("@PrinterDescription", printer.PrinterDescription);
                    command.Parameters.AddWithValue("@PrinterIP", printer.PrinterIP);
                    command.Parameters.AddWithValue("@PrinterPort", printer.PrinterPort);
                    command.Parameters.AddWithValue("@PrinterType", printer.PrinterType);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    printer.HasChanged = false;
                }
                InvalidatePrinters();
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
        public void DeletePrinterDetails(Printer printer)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblPrinters]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printer.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidatePrinters();
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
    public class Printers : List<Printer>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblPrinters";
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
            int RecNumPos = dr.GetOrdinal("RecNum");
            int idJensenPos = dr.GetOrdinal("idJensen");
            int PrinterNamePos = dr.GetOrdinal("PrinterName");
            int PrinterDescriptionPos = dr.GetOrdinal("PrinterDescription");
            int PrinterIPPos = dr.GetOrdinal("PrinterIP");
            int PrinterPortPos = dr.GetOrdinal("PrinterPort");
            int PrinterTypePos = dr.GetOrdinal("PrinterType");

            this.Clear();
            while (dr.Read())
            {
                Printer printer = new Printer()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    idJensen = dr.GetInt16(idJensenPos),
                    PrinterName = dr.GetString(PrinterNamePos),
                    PrinterDescription = dr.GetString(PrinterDescriptionPos),
                    PrinterIP = dr.GetString(PrinterIPPos),
                    PrinterPort = dr.GetInt32(PrinterPortPos),
                    PrinterType = dr.GetString(PrinterTypePos),
                    HasChanged = false
                };

                this.Add(printer);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public Printer GetById(int id)
        {
            return this.Find(delegate(Printer printer)
            {
                return printer.idJensen == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class Printer : DataItem
    {
        #region Printer Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal string PrinterName;
            internal string PrinterDescription;
            internal string PrinterIP;
            internal string PrinterType;
            internal int PrinterPort;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Printer()
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
        /*public override int PrimaryKey
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                this.activeData.RecNum = value;
            }
        }*/

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

        public string PrinterName
        {
            get
            {
                return this.activeData.PrinterName;
            }
            set
            {
                this.activeData.PrinterName = value;
                HasChanged = true;
            }
        }

        public string PrinterDescription
        {
            get
            {
                return this.activeData.PrinterDescription;
            }
            set
            {
                this.activeData.PrinterDescription = value;
                HasChanged = true;
            }
        }

        public string PrinterIP
        {
            get
            {
                return this.activeData.PrinterIP.Trim();
            }
            set
            {
                this.activeData.PrinterIP = value;
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

        public int PrinterPort
        {
            get
            {
                return this.activeData.PrinterPort;
            }
            set
            {
                this.activeData.PrinterPort = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
