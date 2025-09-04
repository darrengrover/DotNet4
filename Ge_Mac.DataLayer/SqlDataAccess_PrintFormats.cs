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
        private PrintFormats printFormatsCache = null;

        public void InvalidatePrintFormats()
        {
            if (printFormatsCache != null)
                printFormatsCache.IsValid = false;
        }

        private bool PrintFormatsAreCached()
        {
            bool test = (printFormatsCache != null);
            if (test)
            {
                test = printFormatsCache.IsValid;
            }
            return test;
        }
        const string allPrintFormatsCommand =
            @"SELECT [RecNum]
                   , [idJensen]
                   , [ShortDescription]
                   , [LongDescription]
                   , [Template]
              FROM [dbo].[tblPrintFormats]";

        public PrintFormats GetAllPrintFormats()
        {
            if (PrintFormatsAreCached())
            {
                return printFormatsCache;
            }
            try
            {
                const string commandString = allPrintFormatsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (printFormatsCache == null) printFormatsCache = new PrintFormats();
                    command.DataFill(printFormatsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return printFormatsCache;
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

        public PrintFormat GetPrintFormat(int idjensen)
        {
            PrintFormats printFormats = GetAllPrintFormats();
            PrintFormat printFormat = printFormats.GetById(idjensen);
            return printFormat;
        }

        #endregion

        #region Next Record

        public int NextPrintFormatRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblPrintFormats";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblPrintFormats',
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
        public void InsertNewPrintFormat(PrintFormat printFormat)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblPrintFormats]
                  ( idJensen
                  , ShortDescription
                  , LongDescription
                  , Template
                  )
                  VALUES
                  ( @idJensen
                  , @ShortDescription
                  , @LongDescription
                  , @Template
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", printFormat.idJensen);
                    command.Parameters.AddWithValue("@ShortDescription", printFormat.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", printFormat.LongDescription);
                    command.Parameters.AddWithValue("@Template", printFormat.Template);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            printFormat.RecNum = (int)RecNum;
                            printFormat.HasChanged = false;
                        }
                        InvalidatePrintFormats();
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
        public void UpdatePrintFormatDetails(PrintFormat printFormat)
        {
            const string commandString =
                @"UPDATE [dbo].[tblPrintFormats]
                  SET idJensen = @idJensen
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , Template = @Template
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printFormat.RecNum);
                    command.Parameters.AddWithValue("@idJensen", printFormat.idJensen);
                    command.Parameters.AddWithValue("@ShortDescription", printFormat.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", printFormat.LongDescription);
                    command.Parameters.AddWithValue("@Template", printFormat.Template);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    printFormat.HasChanged = false;
                }
                InvalidatePrintFormats();
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
        /// Delete a customer permanently from the database.
        /// This should not normally be used as the customers are normally retired.
        /// </summary>
        /// <param name="customer"></param>
        public void DeletePrintFormatDetails(PrintFormat printFormat)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblPrintFormats]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", printFormat.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidatePrintFormats();
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
    public class PrintFormats : List<PrintFormat>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblPrintFormats";
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
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int TemplatePos = dr.GetOrdinal("Template");

            this.Clear();
            while (dr.Read())
            {
                PrintFormat printFormat = new PrintFormat()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    idJensen = dr.GetInt16(idJensenPos),
                    ShortDescription = dr.GetString(ShortDescriptionPos),
                    LongDescription = dr.GetString(LongDescriptionPos),
                    Template = dr.GetString(TemplatePos),
                    HasChanged = false
                };

                this.Add(printFormat);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            return this.Count;
        }

        public PrintFormat GetById(int id)
        {
            return this.Find(delegate(PrintFormat printFormat)
            {
                return printFormat.idJensen == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class PrintFormat : DataItem
    {
        #region Print Format Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal string ShortDescription;
            internal string LongDescription;
            internal string Template;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public PrintFormat()
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

        public string Template
        {
            get
            {
                return this.activeData.Template;
            }
            set
            {
                this.activeData.Template = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
