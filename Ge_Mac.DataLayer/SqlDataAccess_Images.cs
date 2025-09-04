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
        private ImageRecs imagesCache = null;

        //public void InvalidateImageRecs()
        //{
        //    if (imagesCache != null)
        //        imagesCache.IsValid = false;
        //}

        private bool ImageRecsAreCached()
        {
            bool test = (imagesCache != null);
            if (test)
            {
                test = imagesCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string allImagesCommand =
            @"SELECT [ImageID]
                  ,[ImageBitmap]
                  ,[ImageThumbnail]
                  ,[ImageIndex]
                  ,[ImageTable]
                  ,[ImageTableID]
              FROM [dbo].[tblImages]
              ORDER BY ImageID,ImageTable,ImageTableID";

        const string allTableImagesCommand =
                @"SELECT [ImageID]
                  ,[ImageBitmap]
                  ,[ImageThumbnail]
                  ,[ImageIndex]
                  ,[ImageTable]
                  ,[ImageTableID]
              FROM [dbo].[tblImages]";

        public ImageRecs GetAllImages()
        {
            if (ImageRecsAreCached())
            {
                return imagesCache;
            }
            if (DatabaseVersion >= 1.3)
            {
                try
                {
                    const string commandString = allImagesCommand;

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (imagesCache == null) imagesCache = new ImageRecs();
                        command.DataFill(imagesCache, SqlDataConnection.DBConnection.JensenGroup);
                        return imagesCache;
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
            else
            {
                return null;
            }
        }

        public ImageRecs GetAllImages(string TableName)
        {
            if (ImageRecsAreCached())
            {
                return imagesCache;
            }
            if (DatabaseVersion >= 1.3)
            {
                try
                {
                    string commandString = string.Format("{0} WHERE ImageTable='{1}'", allTableImagesCommand, TableName);

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (imagesCache == null) imagesCache = new ImageRecs();
                        command.DataFill(imagesCache, SqlDataConnection.DBConnection.JensenGroup);
                        return imagesCache;
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
            else
            {
                return null;
            }
        }

        public byte[] GetImage(string imageTable,int imageIndex, int imageTableID)
        {
            byte[] pngData = null;
            string sqlString = @"SELECT [ImageBitmap]
                  FROM [dbo].[tblImages]
                  WHERE ImageTable=@ImageTable
                    AND ImageIndex=@ImageIndex
                    AND ImageTableID=@ImageTableID";
            using (SqlCommand command = new SqlCommand(sqlString))
            {
                command.Parameters.AddWithValue("@ImageTable", imageTable);
                command.Parameters.AddWithValue("@ImageIndex", imageIndex);
                command.Parameters.AddWithValue("@ImageTableID", imageTableID);
                SqlDataReader dr = command.ExecuteReader(SqlDataConnection.DBConnection.JensenGroup);
                if (dr.Read())
                {
                    pngData = (byte[])dr["ImageBitmap"];
                }
            }
            return pngData;
        }

        public ImageRec GetImageRec(int id)
        {
            ImageRecs imageRecs = GetAllImages();
            ImageRec imageRec = null;
            if (imageRecs != null)
            {
                imageRec = imageRecs.GetById(id);
            }
            return imageRec;
         }

        public ImageRec GetImageRec(string imageTable, int imageIndex, int imageTableID)
        {
            ImageRecs imageRecs = GetAllImages();
            ImageRec imageRec = null;
            if (imageRecs != null)
            {
                imageRec = imageRecs.GetByTableIDIndex(imageTable,imageTableID,imageIndex);
            }
            return imageRec;
        }

        #endregion

        #region Next Record

        public int NextImageRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblImages',
		                                            @idName = N'ImageID'
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
        public void InsertNewImage(ImageRec imageRec)
        {
            const string commandString =@"INSERT INTO [dbo].[tblImages]
                       ([ImageID]
                       ,[ImageBitmap]
                       ,[ImageThumbnail]
                       ,[ImageIndex]
                       ,[ImageTable]
                       ,[ImageTableID])
                 VALUES
                       (@ImageID
                       ,@ImageBitmap
                       ,@ImageThumbnail
                       ,@ImageIndex
                       ,@ImageTable
                       ,@ImageTableID)";


            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ImageID", imageRec.ImageID);
                    command.Parameters.AddWithValue("@ImageBitmap", imageRec.ImageBitmap);
                    command.Parameters.AddWithValue("@ImageThumbnail", imageRec.ImageThumbnail);
                    command.Parameters.AddWithValue("@ImageIndex", imageRec.ImageIndex);
                    command.Parameters.AddWithValue("@ImageTable", imageRec.ImageTable);
                    command.Parameters.AddWithValue("@ImageTableID", imageRec.ImageTableID);

                    try
                    {
                        command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                        imageRec.HasChanged = true;

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
        public void UpdateImageDetails(ImageRec imageRec)
        {
            const string commandString =
              @"UPDATE [dbo].[tblImages]
                   SET [ImageBitmap] = @ImageBitmap
                      ,[ImageThumbnail] = @ImageThumbnail
                      ,[ImageIndex] = @ImageIndex
                      ,[ImageTable] = @ImageTable
                      ,[ImageTableID] = @ImageTableID
                 WHERE [ImageID] = @ImageID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ImageID", imageRec.ImageID);
                    command.Parameters.AddWithValue("@ImageBitmap", imageRec.ImageBitmap);
                    command.Parameters.AddWithValue("@ImageThumbnail", imageRec.ImageThumbnail);
                    command.Parameters.AddWithValue("@ImageIndex", imageRec.ImageIndex);
                    command.Parameters.AddWithValue("@ImageTable", imageRec.ImageTable);
                    command.Parameters.AddWithValue("@ImageTableID", imageRec.ImageTableID);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    imageRec.HasChanged = false;
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
        public void DeleteImageRecord(ImageRec imageRec)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblImages]
                  WHERE ImageID = @ImageID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ImageID", imageRec.ImageID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
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
    public class ImageRecs : List<ImageRec>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblImages";
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
            int ImageIDPos = dr.GetOrdinal("ImageID");
            //int ImageBitmapPos = dr.GetOrdinal("ImageBitmap");// will this be used?
            int ImageThumbnailPos = dr.GetOrdinal("ImageThumbnail");
            int ImageIndexPos = dr.GetOrdinal("ImageIndex");
            int ImageTablePos = dr.GetOrdinal("ImageTable");
            int ImageTableIDPos = dr.GetOrdinal("ImageTableID");
            this.Clear();
            while (dr.Read())
            {
                ImageRec imageRec = new ImageRec()
                {
                    ImageID = dr.GetInt32(ImageIDPos),
                    //ImageBitmap = dr.GetBytes(ImageBitmapPos),
                    ImageThumbnail = (byte[])dr["ImageThumbnail"],
                    ImageIndex = dr.GetInt32(ImageIndexPos),
                    ImageTable = dr.GetString(ImageTablePos),
                    ImageTableID = dr.GetInt32(ImageTableIDPos),
                    HasChanged = false
                };
                this.Add(imageRec);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            neverExpire = false;
            IsValid = true;

            return this.Count;
        }

        public ImageRec GetById(int id)
        {
            return this.Find(delegate(ImageRec imageRec)
            {
                return imageRec.ImageID == id;
            });
        }

        public ImageRec GetByTableIDIndex(string ImageTable, int ImageTableID, int ImageIndex)
        {
            return this.Find(imageRec => (imageRec.ImageTableID == ImageTableID)
                                        && (imageRec.ImageTable == ImageTable)
                                        && (imageRec.ImageIndex == ImageIndex));
        }

    }
    #endregion

    #region Item Class
    public class ImageRec : DataItem
    {
        #region ImageRec Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int ImageID;
            internal byte[] ImageBitmap;
            internal byte[] ImageThumbnail;
            internal int ImageIndex;
            internal string ImageTable;
            internal int ImageTableID;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ImageRec()
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
                return this.activeData.ImageID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.ImageID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int ImageID
        {
            get
            {
                return this.activeData.ImageID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageID != value;
                this.activeData.ImageID = value;
            }
        }

        public byte[] ImageBitmap
        {
            get
            {
                return this.activeData.ImageBitmap;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageBitmap != value;
                this.activeData.ImageBitmap = value;
            }
        }

        public byte[] ImageThumbnail
        {
            get
            {
                return this.activeData.ImageThumbnail;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageThumbnail != value;
                this.activeData.ImageThumbnail = value;
            }
        }

        public int ImageIndex
        {
            get
            {
                return this.activeData.ImageIndex;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageIndex != value;
                this.activeData.ImageIndex = value;
            }
        }

        public string ImageTable
        {
            get
            {
                return this.activeData.ImageTable;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageTable != value;
                this.activeData.ImageTable = value;
            }
        }

        public int ImageTableID
        {
            get
            {
                return this.activeData.ImageTableID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ImageTableID != value;
                this.activeData.ImageTableID = value;
            }
        }

        #endregion
    }
    #endregion
}
