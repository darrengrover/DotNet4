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

        private SortCategories sortCategoriesCache = null;

        public void InvalidateSortCategories()
        {
            if (sortCategoriesCache != null)
                sortCategoriesCache.IsValid = false;
        }

        private bool SortCategoriesAreCached()
        {
            bool test = (sortCategoriesCache != null);
            if (test)
            {
                test = sortCategoriesCache.IsValid;
            }
            return test;
        }

        const string allSortCategoriesCommand =
            @"SELECT RecNum
                   , idJensen
                   , QuickRef
                   , ExtRef
                   , ShortDescription
                   , LongDescription
                   , BackColour
                   , ForeColour
              FROM [dbo].[tblSortCategory]
                ORDER BY idJensen";
        const string allActiveSortCategoriesCommand =
            @"SELECT RecNum
                   , idJensen
                   , QuickRef
                   , ExtRef
                   , ShortDescription
                   , LongDescription
                   , BackColour
                   , ForeColour
              FROM [dbo].[tblSortCategory]
              WHERE LEN(RTRIM(ShortDescription)) > 0
                ORDER BY idJensen";

        public SortCategories GetAllSortCategories()
        {
            try
            {
                const string commandString = allSortCategoriesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    SortCategories sortCategories = new SortCategories();
                    command.DataFill(sortCategories, SqlDataConnection.DBConnection.JensenGroup);
                    InvalidateSortCategories();
                    return sortCategories;
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

        public SortCategories GetAllActiveSortCategories()
        {
            if (SortCategoriesAreCached())
            {
                return sortCategoriesCache;
            }
            try
            {
                const string commandString =
                    allActiveSortCategoriesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (sortCategoriesCache == null) sortCategoriesCache = new SortCategories();
                    command.DataFill(sortCategoriesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return sortCategoriesCache;
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

        public SortCategory GetSortCategory(int idjensen)
        {
            SortCategories sortCategories = GetAllActiveSortCategories();
            SortCategory sortCategory = sortCategories.GetById(idjensen);

            return sortCategory;
        }

        #endregion

        #region Next Record

        public int NextSortCategoryRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblSortCategory";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblSortCategory',
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
        public void InsertNewSortCategory(SortCategory sortCategory)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblSortCategory]
                  ( idJensen
                  , QuickRef
                  , ExtRef
                  , ShortDescription
                  , LongDescription
                  , BackColour
                  , ForeColour
                  )
                  VALUES
                  ( @idJensen
                  , @QuickRef
                  , @ExtRef
                  , @ShortDescription
                  , @LongDescription
                  , @BackColour
                  , @ForeColour
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", sortCategory.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", sortCategory.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = sortCategory.ExtRef == null ? DBNull.Value : (object)sortCategory.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", sortCategory.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", sortCategory.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", sortCategory.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", sortCategory.ForeColour);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            sortCategory.RecNum = (int)RecNum;
                            sortCategory.HasChanged = false;
                        }
                        InvalidateSortCategories();
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
        public void UpdateSortCategoryDetails(SortCategory sortCategory)
        {
            const string commandString =
                @"UPDATE [dbo].[tblSortCategory]
                  SET idJensen = @idJensen
                    , QuickRef = @QuickRef
                    , ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , BackColour = @BackColour
                    , ForeColour = @ForeColour
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", sortCategory.RecNum);
                    command.Parameters.AddWithValue("@idJensen", sortCategory.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", sortCategory.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = sortCategory.ExtRef == null ? DBNull.Value : (object)sortCategory.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", sortCategory.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", sortCategory.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", sortCategory.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", sortCategory.ForeColour);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    sortCategory.HasChanged = false;
                    InvalidateSortCategories();
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
        /// Delete a sort category permanently from the database.
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteSortCategoryDetails(SortCategory sortCategory)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblSortCategory]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", sortCategory.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateSortCategories();
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
    public class SortCategories : List<SortCategory>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblSortCategory";
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
            int QuickRefPos = dr.GetOrdinal("QuickRef");
            int ExtRefPos = dr.GetOrdinal("ExtRef");
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int BackColourPos = dr.GetOrdinal("BackColour");
            int ForeColourPos = dr.GetOrdinal("ForeColour");

            this.Clear();
            while (dr.Read())
            {
                SortCategory sortCategory = new SortCategory()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    idJensen = dr.GetInt16(idJensenPos),
                    QuickRef = dr.IsDBNull(QuickRefPos) ? 0 : dr.GetInt16(QuickRefPos),
                    ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                    ShortDescription = dr.GetString(ShortDescriptionPos),
                    LongDescription = dr.GetString(LongDescriptionPos),
                    BackColour = dr.GetInt32(BackColourPos),
                    ForeColour = dr.GetInt32(ForeColourPos),
                    HasChanged = false
                };
                // Add to sort categories collection
                this.Add(sortCategory);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public SortCategory GetById(int id)
        {
            return this.Find(delegate(SortCategory cat)
            {
                return cat.idJensen == id;
            });
        }

        public SortCategory GetByName(string aName)
        {
            return this.Find(delegate(SortCategory cat)
            {
                return cat.ShortDescription == aName;
            });
        }
        
        public SortCategory GetByNameID(string aName)
        {
            return this.Find(delegate(SortCategory cat)
            {
                return cat.ShortDescAndID == aName;
            });
        }

    }
    #endregion

    #region SortCategory Class
    public class SortCategory : DataItem, IComparable<SortCategory>
    {
        #region SortCategory Data Record
        protected class DataRecord : ICopyableObject
        {
            public int RecNum;
            public int idJensen;
            public int QuickRef;
            public string ExtRef;
            public string ShortDescription;
            public string LongDescription;
            public int BackColour;
            public int ForeColour;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public SortCategory()
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

        #region Color Properties
        public Color BackgroundColour
        {
            get
            {
                return ColorFromInt(BackColour);
            }
            set
            {
                BackColour = (int)Convert.ToInt32(value.ToArgb().ToString());
            }
        }

        public Color ForegroundColour
        {
            get
            {
                return ColorFromInt(ForeColour);
            }
            set
            {
                ForeColour = (int)Convert.ToInt32(value.ToArgb().ToString());
            }
        }

        private Color ColorFromInt(int color)
        {
            string str = "#" + color.ToString("X8");
            ColorConverter conv = new ColorConverter();
            return (Color)conv.ConvertFromString(str);
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

        public int idJensen
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

        public int QuickRef
        {
            get
            {
                return this.activeData.QuickRef;
            }
            set
            {
                this.activeData.QuickRef = value;
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

        public string ShortDescAndID
        {
            get
            {
                return ShortDescription + " (" + idJensen.ToString() + ")";
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

        public int BackColour
        {
            get
            {
                return this.activeData.BackColour;
            }
            set
            {
                this.activeData.BackColour = value;
                HasChanged = true;
            }
        }

        public int ForeColour
        {
            get
            {
                return this.activeData.ForeColour;
            }
            set
            {
                this.activeData.ForeColour = value;
                HasChanged = true;
            }
        }

        #endregion

        #region Sorting
        public static Comparison<SortCategory> NameComparison =
            delegate(SortCategory c1, SortCategory c2)
            {
                return c1.ShortDescription.CompareTo(c2.ShortDescription);
            };

        public static Comparison<SortCategory> IDComparison =
            delegate(SortCategory c1, SortCategory c2)
            {
                return c1.idJensen.CompareTo(c2.idJensen);
            };

        public int CompareTo(SortCategory other) { return ShortDescription.CompareTo(other.ShortDescription); }
        #endregion

    }
    #endregion
}
