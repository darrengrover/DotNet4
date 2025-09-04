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
        private MemoRecs memosCache = null;

        private bool MemosAreCached()
        {
            bool test = (memosCache != null);
            if (test)
            {
                test = memosCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string allMemosCommand =
            @"SELECT mem.[MemoID]
                  ,mem.[MemoText]
                  ,mem.[MemoIndex]
                  ,mem.[MemoTable]
                  ,mem.[MemoTableID]
                  ,mem.[MemoRefID]
	              ,mr.[MemoReference]
	              ,mr.[MemoSubReference]
	              ,mr.[NrMemos]
              FROM [dbo].[tblMemos] mem, [dbo].[tblMemoReferences] mr
              WHERE mem.MemoRefID=mr.MemoRefID
              ORDER BY MemoID,MemoReference,MemoSubReference";

        public MemoRecs GetAllMemos()
        {
            if (MemosAreCached())
            {
                return memosCache;
            }
            if (DatabaseVersion >= 1.3)
            {
                try
                {
                    const string commandString = allMemosCommand;

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (memosCache == null) memosCache = new MemoRecs();
                        command.DataFill(memosCache, SqlDataConnection.DBConnection.JensenGroup);
                        return memosCache;
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

        const string allMemoRefsCommand =
            @"SELECT [MemoRefID]
                  ,[MemoReference]
                  ,[MemoSubReference]
                  ,[NrMemos]
              FROM [dbo].[tblMemoReferences]
                          ORDER BY MemoRefID";

        public MemoRefs GetAllMemoRefs()
        {
            if (DatabaseVersion >= 1.3)
            {
                try
                {
                    const string commandString = allMemoRefsCommand;

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        MemoRefs memoRefs = new MemoRefs();
                        command.DataFill(memoRefs, SqlDataConnection.DBConnection.JensenGroup);
                        return memoRefs;
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

        #endregion

        #region Next Record

        public int NextMemoRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblMemos',
		                                            @idName = N'MemoID'
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
        public void InsertNewMemo(MemoRec memoRec)
        {
            const string commandString = @"INSERT INTO [dbo].[tblMemos]
                   ([MemoID]
                   ,[MemoText]
                   ,[MemoIndex]
                   ,[MemoTable]
                   ,[MemoTableID]
                   ,[MemoRefID])
             VALUES
                   (@MemoID
                   ,@MemoText
                   ,@MemoIndex
                   ,@MemoTable
                   ,@MemoTableID
                   ,@MemoRefID)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@MemoID", memoRec.MemoID);
                    command.Parameters.AddWithValue("@MemoText", memoRec.MemoText);
                    command.Parameters.AddWithValue("@MemoIndex", memoRec.MemoIndex);
                    command.Parameters.AddWithValue("@MemoTable", memoRec.MemoTable);
                    command.Parameters.AddWithValue("@MemoTableID",memoRec.MemoTableID);
                    command.Parameters.AddWithValue("@MemoRefID", memoRec.MemoRefID);

                    try
                    {
                        command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                        memoRec.HasChanged = false;

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
        public void UpdateMemos(MemoRec memoRec)
        {
            const string commandString =
              @"UPDATE [dbo].[tblMemos]
               SET [MemoText] = @MemoText
                  ,[MemoIndex] = @MemoIndex
                  ,[MemoTable] = @MemoTable
                  ,[MemoTableID] = @MemoTableID
                  ,[MemoRefID] = @MemoRefID
               WHERE [MemoID] = @MemoID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@MemoID", memoRec.MemoID);
                    command.Parameters.AddWithValue("@MemoText", memoRec.MemoText);
                    command.Parameters.AddWithValue("@MemoIndex", memoRec.MemoIndex);
                    command.Parameters.AddWithValue("@MemoTable", memoRec.MemoTable);
                    command.Parameters.AddWithValue("@MemoTableID", memoRec.MemoTableID);
                    command.Parameters.AddWithValue("@MemoRefID", memoRec.MemoRefID);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    memoRec.HasChanged = false;
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
        public void DeleteMemoRecord(MemoRec memoRec)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblMemos]
                  WHERE MemoID = @ImageID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@ImageID", memoRec.MemoID);
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


    #region Data Collection Classes
    public class MemoRecs : List<MemoRec>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblMemos";
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
            int MemoIDPos = dr.GetOrdinal("MemoID");
            int MemoTextPos = dr.GetOrdinal("MemoText");
            int MemoIndexPos = dr.GetOrdinal("MemoIndex");
            int MemoTablePos = dr.GetOrdinal("MemoTable");
            int MemoTableIDPos = dr.GetOrdinal("MemoTableID");
            int MemoRefIDPos = dr.GetOrdinal("MemoRefID");
            int MemoReferencePos = dr.GetOrdinal("MemoReference");
            int MemoSubReferencePos = dr.GetOrdinal("MemoSubReference");
            int NrMemosPos = dr.GetOrdinal("NrMemos");

            this.Clear();
            while (dr.Read())
            {
                MemoRec memoRec = new MemoRec()
                {
                    MemoID = dr.GetInt32(MemoIDPos),
                    MemoText = dr.GetString(MemoTextPos),
                    MemoIndex = dr.GetInt32(MemoIndexPos),
                    MemoTable = dr.GetString(MemoTablePos),
                    MemoTableID = dr.GetInt32(MemoTableIDPos),
                    MemoRefID = dr.GetInt32(MemoRefIDPos),
                    MemoReference = dr.GetString(MemoReferencePos),
                    MemoSubReference = dr.GetString(MemoSubReferencePos),
                    NrMemos = dr.GetInt32(NrMemosPos),
                    HasChanged = false
                };

                this.Add(memoRec);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            neverExpire = false;
            IsValid = true;

            return this.Count;
        }

        public MemoRec GetById(int id)
        {
            return this.Find(delegate(MemoRec memoRec)
            {
                return memoRec.MemoID == id;
            });
        }

        public MemoRec GetByIndexRef(int memoIndex, int refid)
        {
            return this.Find(delegate(MemoRec memoRec)
            {
                return memoRec.MemoRefID == refid && memoRec.MemoIndex == memoIndex;
            });
        }
        public MemoRec GetByTableIdIndexRef(string memoTable, int tableID, int memoIndex, int refID)
        {
            return this.Find(memoRec => (memoRec.MemoTable == memoTable)
                && (memoRec.MemoTableID == tableID)
                && (memoRec.MemoIndex == memoIndex)
                && (memoRec.MemoRefID == refID));
        }

    }
    public class MemoRefs : List<MemoRef>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblMemoReferences";
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
            int MemoRefIDPos = dr.GetOrdinal("MemoRefID");
            int MemoReferencePos = dr.GetOrdinal("MemoReference");
            int MemoSubReferencePos = dr.GetOrdinal("MemoSubReference");
            int NrMemosPos = dr.GetOrdinal("NrMemos");

            this.Clear();
            while (dr.Read())
            {
                MemoRef memoRef = new MemoRef()
                {
                    MemoRefID = dr.GetInt32(MemoRefIDPos),
                    MemoReference = dr.GetString(MemoReferencePos),
                    MemoSubReference = dr.GetString(MemoSubReferencePos),
                    NrMemos = dr.GetInt32(NrMemosPos),
                    HasChanged = false
                };

                this.Add(memoRef);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            neverExpire = false;
            IsValid = true;

            return this.Count;
        }

        public MemoRef GetById(int id)
        {
            return this.Find(delegate(MemoRef memoRef)
            {
                return memoRef.MemoRefID == id;
            });
        }

        public MemoRef GetByMemoRefComposite(string aRef)
        {
            return this.Find(memoRef => (memoRef.MemoRefComposite == aRef));
        }

    }
    #endregion

    #region Item Classes
    public class MemoRec : DataItem
    {
        #region MemoRec Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int MemoID;
            internal string MemoText;
            internal int MemoIndex;
            internal string MemoTable;
            internal int MemoTableID;
            internal int MemoRefID;
            internal string MemoReference;
            internal string MemoSubReference;
            internal int NrMemos;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public MemoRec()
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
                return this.activeData.MemoID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.MemoID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int MemoID
        {
            get
            {
                return this.activeData.MemoID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoID != value;
                this.activeData.MemoID = value;
            }
        }

        public string MemoText
        {
            get
            {
                return this.activeData.MemoText;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoText != value;
                this.activeData.MemoText = value;
            }
        }

        public int MemoIndex
        {
            get
            {
                return this.activeData.MemoIndex;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoIndex != value;
                this.activeData.MemoIndex = value;
            }
        }

        public string MemoTable
        {
            get
            {
                return this.activeData.MemoTable;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoTable != value;
                this.activeData.MemoTable = value;
            }
        }

        public int MemoTableID
        {
            get
            {
                return this.activeData.MemoTableID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoTableID != value;
                this.activeData.MemoTableID = value;
            }
        }

        public int MemoRefID
        {
            get
            {
                return this.activeData.MemoRefID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoRefID != value;
                this.activeData.MemoRefID = value;
            }
        }

        public string MemoReference
        {
            get
            {
                return this.activeData.MemoReference;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoReference != value;
                this.activeData.MemoReference = value;
            }
        }

        public string MemoSubReference
        {
            get
            {
                return this.activeData.MemoSubReference;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoSubReference != value;
                this.activeData.MemoSubReference = value;
            }
        }

        public int NrMemos
        {
            get
            {
                return this.activeData.NrMemos;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.NrMemos != value;
                this.activeData.NrMemos = value;
            }
        }
        #endregion
    }
 
    public class MemoRef : DataItem
    {
        #region MemoRef Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int MemoRefID;
            internal string MemoReference;
            internal string MemoSubReference;
            internal int NrMemos;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public MemoRef()
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
                return this.activeData.MemoRefID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.MemoRefID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int MemoRefID
        {
            get
            {
                return this.activeData.MemoRefID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoRefID != value;
                this.activeData.MemoRefID = value;
            }
        }

        public string MemoReference
        {
            get
            {
                return this.activeData.MemoReference;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoReference != value;
                this.activeData.MemoReference = value;
            }
        }

        public string MemoSubReference
        {
            get
            {
                return this.activeData.MemoSubReference;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.MemoSubReference != value;
                this.activeData.MemoSubReference = value;
            }
        }

        public string MemoRefComposite
        {
            get
            {
                string aString = MemoReference;
                if (MemoSubReference != string.Empty)
                {
                    aString += ", " + MemoSubReference;
                }
                return aString;
            }
        }

        public int NrMemos
        {
            get
            {
                return this.activeData.NrMemos;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.NrMemos != value;
                this.activeData.NrMemos = value;
            }
        }

        #endregion
    }
    #endregion
}
