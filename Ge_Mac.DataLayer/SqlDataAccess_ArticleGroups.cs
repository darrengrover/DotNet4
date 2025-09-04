using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private ArticleGroups articleGroupsCache = null;

        public void InvalidateArticleGroups()
        {
            if (articleGroupsCache != null)
                articleGroupsCache.IsValid = false;
        }

        private bool ArticleGroupsAreCached()
        {
            bool test = (articleGroupsCache != null);
            if (test)
            {
                test = articleGroupsCache.IsValid;
            }
            return test;
        }

        const string allArticleGroupsCommand =
            @"SELECT [RecNum]
                   , [ExtRef]
                   , [ShortDescription]
                   , [LongDescription]
                   , [BackColour]
                   , [ForeColour]
              FROM [dbo].[tblArticleGroups]";

        public ArticleGroups GetAllArticleGroups()
        {
            if (ArticleGroupsAreCached())
            {
                return articleGroupsCache;
            }
            try
            {
                const string commandString = allArticleGroupsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (articleGroupsCache == null) articleGroupsCache = new ArticleGroups();
                    command.DataFill(articleGroupsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return articleGroupsCache;
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

        public ArticleGroup GetArticleGroup(int RecNum)
        {
            ArticleGroups articleGroups = GetAllArticleGroups();
            ArticleGroup articleGroup = articleGroups.GetById(RecNum);

            return articleGroup;
        }
        #endregion

        #region Insert Data
        public void InsertNewArticleGroup(ArticleGroup articleGroup)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblArticleGroups]
                  ( ExtRef
                  , ShortDescription
                  , LongDescription
                  , BackColour
                  , ForeColour
                  )
                  VALUES
                  ( @ExtRef
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
                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = articleGroup.ExtRef == null ? DBNull.Value : (object)articleGroup.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", articleGroup.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", articleGroup.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", articleGroup.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", articleGroup.ForeColour);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            articleGroup.RecNum = (int)RecNum;
                            articleGroup.HasChanged = false;
                        }
                        InvalidateArticleGroups();
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
        public void UpdateArticleGroupDetails(ArticleGroup articleGroup)
        {
            const string commandString =
                @"UPDATE [dbo].[tblArticleGroups]
                  SET ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , BackColour = @BackColour
                    , ForeColour = @ForeColour
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", articleGroup.RecNum);
                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = articleGroup.ExtRef == null ? DBNull.Value : (object)articleGroup.ExtRef;
                    //command.Parameters.AddWithValue("@ExtRef", articleGroup.ExtRef);
                    command.Parameters.AddWithValue("@ShortDescription", articleGroup.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", articleGroup.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", articleGroup.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", articleGroup.ForeColour);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    articleGroup.HasChanged = false;
                }
                InvalidateArticleGroups();
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
        /// <param name="articleGroup"></param>
        public void DeleteArticleGroupDetails(ArticleGroup articleGroup)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblArticleGroups]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", articleGroup.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateArticleGroups();
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
    public class ArticleGroups : List<ArticleGroup>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblArticleGroups";
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
            int ExtRefPos = dr.GetOrdinal("ExtRef");
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int BackColourPos = dr.GetOrdinal("BackColour");
            int ForeColourPos = dr.GetOrdinal("ForeColour");
 
            this.Clear();

            while (dr.Read())
            {
                ArticleGroup articleGroup = new ArticleGroup()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                    ShortDescription = dr.GetString(ShortDescriptionPos),
                    LongDescription = dr.GetString(LongDescriptionPos),
                    BackColour = dr.GetInt32(BackColourPos),
                    ForeColour = dr.GetInt32(ForeColourPos),
                    HasChanged = false
                };

                // Add to articleGroup collection
                this.Add(articleGroup);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public ArticleGroup GetById(int id)
        {
            return this.Find(delegate(ArticleGroup articleGroup)
            {
                return articleGroup.RecNum == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class ArticleGroup : DataItem
    {
        #region Article Group Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal string ExtRef;
            internal string ShortDescription;
            internal string LongDescription;
            internal int BackColour;
            internal int ForeColour;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ArticleGroup()
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

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return ((activeData.RecNum == -1) || (ForceNew));
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
    }
    #endregion
}
