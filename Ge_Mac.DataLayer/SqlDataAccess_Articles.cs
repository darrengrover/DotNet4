using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private Articles articlesCache = null;

        public void InvalidateArticles()
        {
            if (articlesCache != null)
                articlesCache.IsValid = false;
        }

        private bool ArticlesAreCached()
        {
            bool test = (articlesCache != null);
            if (test)
            {
                test = articlesCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string allArticlesCommand1 =
            @"SELECT [RecNum]
                   , [idJensen]
                   , [QuickRef]
                   , [ExtRef]
                   , [ShortDescription]
                   , [LongDescription]
                   , [BackColour]
                   , [ForeColour]
                   , [Retired]
                   , [Weight_KG]
                   , [ArticleGroup]
              FROM [dbo].[tblArticles]";

        const string allArticlesCommand2 =
    @"SELECT [RecNum]
                   , [idJensen]
                   , [QuickRef]
                   , [ExtRef]
                   , [ShortDescription]
                   , [LongDescription]
                   , [BackColour]
                   , [ForeColour]
                   , [Retired]
                   , [Weight_KG]
                   , [StackCount]
                   , [ArticleGroup]
              FROM [dbo].[tblArticles]";

        private string allArticlesCommand
        {
            get
            {
                if (DatabaseVersion >= 2.0)
                    return allArticlesCommand2;
                else
                    return allArticlesCommand1;
            }
        }

        public Articles GetAllArticles() // not cached only used in editing.
        {
            try
            {
                string commandString = allArticlesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Articles allArticles = new Articles();
                    command.DataFill(allArticles, SqlDataConnection.DBConnection.JensenGroup);
                    return allArticles;
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

        public Articles GetAllActiveArticles()
        {
            if (ArticlesAreCached())
            {
                return articlesCache;
            }
            try
            {
                string commandString =
                    allArticlesCommand +
                    " WHERE Retired = 0";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (articlesCache == null) articlesCache = new Articles();
                    command.DataFill(articlesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return articlesCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                Debug.WriteLine("GetAllActiveArticles(): " + ex.Message);
                throw;
            }
        }

        public Article GetArticle(int idJensen)
        {
            Articles articles = GetAllActiveArticles();
            Article article = articles.GetById(idJensen);

            return article;
         }
        #endregion

        #region Next Record

        public int NextArticleRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblArticles";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblArticles',
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
        public void InsertNewArticle(Article article)
        {
            const string commandString1 =
                @"INSERT INTO [dbo].[tblArticles]
                  ( idJensen
                  , QuickRef
                  , ExtRef
                  , ShortDescription
                  , LongDescription
                  , BackColour
                  , ForeColour
                  , Retired
                  , [Weight_KG]
                  , [ArticleGroup]
                  )
                  VALUES
                  ( @idJensen
                  , @QuickRef
                  , @ExtRef
                  , @ShortDescription
                  , @LongDescription
                  , @BackColour
                  , @ForeColour
                  , @Retired
                  , @Weight_KG
                  , @ArticleGroup
                  )
        
                  SELECT CAST(@@IDENTITY AS INT)";

            const string commandString2 =
                @"INSERT INTO [dbo].[tblArticles]
                  ( idJensen
                  , QuickRef
                  , ExtRef
                  , ShortDescription
                  , LongDescription
                  , BackColour
                  , ForeColour
                  , Retired
                  , [Weight_KG]
                  , [StackCount]
                  , [ArticleGroup]
                  )
                  VALUES
                  ( @idJensen
                  , @QuickRef
                  , @ExtRef
                  , @ShortDescription
                  , @LongDescription
                  , @BackColour
                  , @ForeColour
                  , @Retired
                  , @Weight_KG
                  , @StackCount
                  , @ArticleGroup
                  )
        
                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                string commandString = commandString1;
                if (DatabaseVersion >= 2.0)
                {
                    commandString = commandString2;
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", article.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", article.QuickRef);

                    SqlParameter pEF = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    pEF.IsNullable = true;
                    pEF.Value = article.ExtRef == null ? DBNull.Value : (object)article.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", article.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", article.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", article.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", article.ForeColour);
                    command.Parameters.AddWithValue("@Retired", article.Retired);
                    command.Parameters.AddWithValue("@Weight_KG", article.Weight_KG);
                    command.Parameters.AddWithValue("@ArticleGroup", article.ArticleGroup);
                    if (DatabaseVersion >= 2.0)
                    {
                        command.Parameters.AddWithValue("@StackCount", article.Stack_Count);
                    }
                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            article.RecNum = (int)RecNum;
                            article.HasChanged = false;
                        }
                        InvalidateArticles();
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
        public void UpdateArticleDetails(Article article)
        {
            const string commandString1 =
                @"UPDATE [dbo].[tblArticles]
                  SET idJensen = @idJensen
                    , QuickRef = @QuickRef
                    , ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , BackColour = @BackColour
                    , ForeColour = @ForeColour
                    , Retired = @Retired
                    , Weight_KG = @Weight_KG
                    , ArticleGroup = @ArticleGroup
                  WHERE RecNum = @RecNum";

            const string commandString2 =
                @"UPDATE [dbo].[tblArticles]
                  SET idJensen = @idJensen
                    , QuickRef = @QuickRef
                    , ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , BackColour = @BackColour
                    , ForeColour = @ForeColour
                    , Retired = @Retired
                    , Weight_KG = @Weight_KG
                    , StackCount = @StackCount
                    , ArticleGroup = @ArticleGroup
                  WHERE RecNum = @RecNum";

            try
            {
                string commandString = commandString1;
                if (DatabaseVersion >= 2.0)
                {
                    commandString = commandString2;
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", article.RecNum);
                    command.Parameters.AddWithValue("@idJensen", article.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", article.QuickRef);

                    SqlParameter pEF = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    pEF.IsNullable = true;
                    pEF.Value = article.ExtRef == null ? DBNull.Value : (object)article.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", article.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", article.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", article.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", article.ForeColour);
                    command.Parameters.AddWithValue("@Retired", article.Retired);
                    command.Parameters.AddWithValue("@Weight_KG", article.Weight_KG);
                    command.Parameters.AddWithValue("@ArticleGroup", article.ArticleGroup);
                    if (DatabaseVersion >= 2.0)
                    {
                        command.Parameters.AddWithValue("@StackCount", article.Stack_Count);
                    }
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    article.HasChanged = false;
                }
                InvalidateArticles();
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
        /// Delete an article permanently from the database.
        /// This should not normally be used as the articles are normally retired.
        /// </summary>
        /// <param name="article"></param>
        public void DeleteArticleDetails(Article article)
        {
            const string commandString =
                /*@"DELETE FROM [dbo].[tblArticles]
                  WHERE RecNum = @RecNum";*/
                @"UPDATE [dbo].[tblArticles]
                  SET Retired = 1
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", article.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateArticles();
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
    public class Articles : List<Article>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Article art)
        {
            base.Add(art);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, art));
            }
        }

        new public void Remove(Article art)
        {
            base.RemoveAt(this.IndexOf(art));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, art));
            }
        }

        #endregion
        private double lifespan = 1.0;
        private string tblName = "tblArticles";
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
            SqlDataAccess da = SqlDataAccess.Singleton;

            int RecNumPos = dr.GetOrdinal("RecNum");
            int idJensenPos = dr.GetOrdinal("idJensen");
            int QuickRefPos = dr.GetOrdinal("QuickRef");
            int ExtRefPos = dr.GetOrdinal("ExtRef");
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int BackColourPos = dr.GetOrdinal("BackColour");
            int ForeColourPos = dr.GetOrdinal("ForeColour");
            int RetiredPos = dr.GetOrdinal("Retired");
            int Weight_KGPos = dr.GetOrdinal("Weight_KG");
            int ArticleGroupPos = dr.GetOrdinal("ArticleGroup");
            int Stack_CountPos = 0;
            if (da.DatabaseVersion >= 2.0)
            {
                Stack_CountPos = dr.GetOrdinal("StackCount");
            }
            this.Clear();
            while (dr.Read())
            {
                Article article = new Article();

                article.RecNum = dr.GetInt32(RecNumPos);
                article.idJensen = dr.GetInt16(idJensenPos);
                article.QuickRef = dr.IsDBNull(QuickRefPos) ? (byte)0 : dr.GetByte(QuickRefPos);
                article.ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos);
                article.ShortDescription = dr.GetString(ShortDescriptionPos);
                article.LongDescription = dr.GetString(LongDescriptionPos);
                article.BackColour = dr.GetInt32(BackColourPos);
                article.ForeColour = dr.GetInt32(ForeColourPos);
                article.Retired = dr.GetBoolean(RetiredPos);
                article.Weight_KG = dr.IsDBNull(Weight_KGPos) ? 0 : dr.GetDecimal(Weight_KGPos);
                if (da.DatabaseVersion >= 2.0)
                    article.Stack_Count = dr.GetInt32(Stack_CountPos);
                article.ArticleGroup = dr.GetInt32(ArticleGroupPos);
                article.HasChanged = false;

                this.Add(article);

            }
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            neverExpire = false;
            IsValid = true;

            return this.Count;
        }

        public Article GetById(int id)
        {
            return this.Find(article => article.idJensen == id);
        }

        public Article GetByNameID(string aName)
        {
            return this.Find(article => article.ShortDescAndID==aName);
        }
    }
    #endregion

    #region Item Class
    public class Article : DataItem, INotifyPropertyChanged
    {
        #region Article Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal byte QuickRef;
            internal string ExtRef;
            internal string ShortDescription;
            internal string LongDescription;
            internal int BackColour;
            internal int ForeColour;
            internal bool Retired;
            internal decimal Weight_KG;
            internal int Stack_Count;
            internal int ArticleGroup;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Article()
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

        public int RecNum
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                if (this.activeData.RecNum != value)
                {
                    this.activeData.RecNum = value;
                    NotifyPropertyChanged("RecNum");
                }
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
                if (this.activeData.idJensen != value)
                {
                    this.activeData.idJensen = value;
                    NotifyPropertyChanged("idJensen");
                }
            }
        }

        public byte QuickRef
        {
            get
            {
                return this.activeData.QuickRef;
            }
            set
            {
                if (this.activeData.QuickRef != value)
                {
                    this.activeData.QuickRef = value;
                    NotifyPropertyChanged("QuickRef");
                }
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
                if (this.activeData.ExtRef != value)
                {
                    this.activeData.ExtRef = value;
                    NotifyPropertyChanged("ExtRef");
                }
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
                if (this.activeData.ShortDescription != value)
                {
                    this.activeData.ShortDescription = value;
                    NotifyPropertyChanged("ShortDescription");
                }
            }
        }

        public string ShortDescAndID
        {
            get
            {
                return ShortDescription + " (" + idJensen.ToString() + ")";
            }
        }

        public string ArticleNameID
        {
            get
            {
                return ShortDescAndID;
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
                if (this.activeData.LongDescription != value)
                {
                    this.activeData.LongDescription = value;
                    NotifyPropertyChanged("LongDescription");
                }
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
                if (this.activeData.BackColour != value)
                {
                    this.activeData.BackColour = value;
                    NotifyPropertyChanged("BackColour");
                }
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
                if (this.activeData.ForeColour != value)
                {
                    this.activeData.ForeColour = value;
                    NotifyPropertyChanged("ForeColour");
                }
            }
        }

        public bool Retired
        {
            get
            {
                return this.activeData.Retired;
            }
            set
            {
                if (this.activeData.Retired != value)
                {
                    this.activeData.Retired = value;
                    NotifyPropertyChanged("Retired");
                }
            }
        }

        public decimal Weight_KG
        {
            get
            {
                return this.activeData.Weight_KG;
            }
            set
            {
                if (this.activeData.Weight_KG != value)
                {
                    this.activeData.Weight_KG = value;
                    NotifyPropertyChanged("Weight_KG");
                }
            }
        }
        public int Stack_Count
        {
            get
            {
                return this.activeData.Stack_Count;
            }
            set
            {
                if (this.activeData.Stack_Count != value)
                {
                    this.activeData.Stack_Count = value;
                    NotifyPropertyChanged("Stack_Count");
                }
            }
        }
        public int ArticleGroup
        {
            get
            {
                return this.activeData.ArticleGroup;
            }
            set
            {
                if (this.activeData.ArticleGroup != value)
                {
                    this.activeData.ArticleGroup = value;
                    NotifyPropertyChanged("ArticleGroup");
                }
            }
        }


        public string ArticleGroupName
        {
            get
            {
                string agname = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                ArticleGroup ag = da.GetArticleGroup(ArticleGroup);
                if (ag != null)
                {
                    agname = ag.ShortDescription;
                }
                return agname;
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion
    
    }
    #endregion
}
