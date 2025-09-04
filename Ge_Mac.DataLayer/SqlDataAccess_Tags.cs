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
        #region Caches

        private Tags tagsCache = null;
        private LoginTerminals loginTerminalsCache = null;
        private TagReaderLocations tagReaderLocationsCache = null;
        private LoginTerminalMachines loginTerminalMachinesCache = null;

        private bool TagsAreCached()
        {
            bool test = (tagsCache != null);
            if (test)
            {
                test = tagsCache.IsValid;
            }
            return test;
        }

        private bool TagReaderLocationsAreCached()
        {
            bool test = (tagReaderLocationsCache != null);
            if (test)
            {
                test = tagReaderLocationsCache.IsValid;
            }
            return test;
        }

        private bool LoginTerminalsAreCached()
        {
            bool test = (loginTerminalsCache != null);
            if (test)
            {
                test = loginTerminalsCache.IsValid;
            }
            return test;
        }

        private bool LoginTerminalMachinesAreCached()
        {
            bool test = (loginTerminalMachinesCache != null);
            if (test)
            {
                test = loginTerminalMachinesCache.IsValid;
            }
            return test;
        }




        #endregion
 
        #region Select Data
        #region TagReaderLocations

        public Tags GetAllTags()
        {
            return GetAllTags(false);
        }

        public Tags GetAllTags(bool noCache)
        {
            if (!noCache && TagsAreCached())
            {
                return tagsCache;
            }
            try
            {
                const string commandString = @"SELECT [TagID]
                                                      ,[TagData]
                                                      ,[ReferenceTable]
                                                      ,[ReferenceID]
                                                  FROM [dbo].[tblTags]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (tagsCache == null) tagsCache = new Tags();
                    command.DataFill(tagsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return tagsCache;
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

        public TagReaderLocations GetAllTagReaderLocations()
        {
            if (TagReaderLocationsAreCached())
            {
                return tagReaderLocationsCache;
            }
            try
            {
                const string commandString = @"SELECT [LocationID]
                                                      ,[LocationIP]
                                                      ,[Machine_idJensen]
                                                      ,[SubID]
                                                  FROM [dbo].[tblTagReaderLocations]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (tagReaderLocationsCache == null) tagReaderLocationsCache = new TagReaderLocations();
                    command.DataFill(tagReaderLocationsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return tagReaderLocationsCache;
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

        #region LoginTerminals

        public LoginTerminals GetAllLoginTerminals()
        {
            if (LoginTerminalsAreCached())
            {
                return loginTerminalsCache;
            }
            try
            {
                const string commandString = @"SELECT [LoginTerminalID]
                                                      ,[LocationID]
                                                      ,[TerminalName]
                                                      ,[TerminalIP]
                                                      ,[FavouriteID]
                                                      ,[ReaderType]
                                                      ,[ReaderPort]
                                                  FROM [dbo].[tblLoginTerminals]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (loginTerminalsCache == null) loginTerminalsCache = new LoginTerminals();
                    command.DataFill(loginTerminalsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return loginTerminalsCache;
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

        #region LoginTerminalMachines

        public LoginTerminalMachines GetAllLoginTerminalMachines()
        {
            if (LoginTerminalMachinesAreCached())
            {
                return loginTerminalMachinesCache;
            }
            try
            {
                const string commandString = @"SELECT [LoginTermMachineID]
                                                      ,[LoginTerminalID]
                                                      ,[Machine_idJensen]
                                                      ,[FavouriteID]
                                                  FROM [dbo].[tblLoginTerminalMachines]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (loginTerminalMachinesCache == null) loginTerminalMachinesCache = new LoginTerminalMachines();
                    command.DataFill(loginTerminalMachinesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return loginTerminalMachinesCache;
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

        #region RemoteOperatorLogs

//        public RemoteOperatorLogs GetUncommittedOperatorLogs()
//        {
//            try
//            {
//                const string commandString = @"SELECT rol.[RemoteOperatorLogID]
//                                                      ,rol.[LogTime]
//                                                      ,rol.[MachineID]
//                                                      ,rol.[OperatorID]
//                                                      ,rol.[SubID]
//                                                      ,rol.[State]
//                                                      ,min (ld.remoteID) as jgLogDataRemoteID
//                                                      ,min (ld.Timestamp) as updatetime
//                                                  FROM [tblRemoteOperatorLog] rol left OUTER JOIN  tblJGLogData ld
//                                                  on rol.machineid=ld.MachineID
//                                                  AND ld.RegType=1
//                                                   AND ld.TimeStamp>=rol.LogTime
//                                                  group by rol.RemoteOperatorLogID,rol.LogTime,rol.MachineID,rol.OperatorID,rol.Subid,rol.State
//                                                  order by rol.remoteOperatorlogid";

//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    RemoteOperatorLogs remoteOperatorLogs = new RemoteOperatorLogs();
//                    command.DataFill(remoteOperatorLogs, SqlDataConnection.DBConnection.JensenGroup);
//                    return remoteOperatorLogs;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ExceptionHandler.Handle(ex);
//                    Debugger.Break();
//                }
//                throw;
//            }
//        }

        #endregion
        #endregion

        #region Next Record


        public int NextTagReaderLocationRecord()
        {
            return NextRecord("tblTagReaderLocations", "LocationID");
        }

        public int NextLoginTerminalRecord()
        {
            return NextRecord("tblLoginTerminals", "LoginTerminalID");
        }

        public int NextLoginTerminalMachineRecord()
        {
            return NextRecord("tblLoginTerminalMachines", "LoginTermMachineID");
        }

        public int NextTagRecord()
        {
            return NextRecord("tblTags", "TagID");
        }

        public int NextTagReaderLog()
        {
            return NextRecord("tblTagReaderLog", "LogID");
        }

        public int NextRemoteOperatorLog()
        {
            return NextRecord("tblRemoteOperatorLog", "RemoteOperatorLogID");
        }


        #endregion

        #region Insert Data
        public void InsertNewTagReaderLocation(TagReaderLocation tagReaderLocation)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblTagReaderLocations]
                       ([LocationID]
                       ,[LocationIP]
                       ,[Machine_idJensen]
                       ,[SubID])
                 VALUES
                       (@LocationID
                       ,@LocationIP
                       ,@Machine_idJensen
                       ,@SubID)";

            try
            {
                if (tagReaderLocation.LocationID < 0)
                    tagReaderLocation.LocationID = NextTagReaderLocationRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LocationID", tagReaderLocation.LocationID);
                    command.Parameters.AddWithValue("@LocationIP", tagReaderLocation.LocationIP);
                    command.Parameters.AddWithValue("@Machine_idJensen", tagReaderLocation.Machine_idJensen);
                    command.Parameters.AddWithValue("@SubID", tagReaderLocation.SubID);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        tagReaderLocation.HasChanged = false;
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

        public void InsertNewLoginTerminal(LoginTerminal loginTerminal)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblLoginTerminals]
                                    ([LoginTerminalID]
                                    ,[LocationID]
                                    ,[TerminalName]
                                    ,[TerminalIP]
                                    ,[FavouriteID]
                                    ,[ReaderType]
                                    ,[ReaderPort])
                                VALUES
                                    (@LoginTerminalID
                                    ,@LocationID
                                    ,@TerminalName
                                    ,@TerminalIP
                                    ,@FavouriteID
                                    ,@ReaderType
                                    ,@ReaderPort)";
            try
            {
                if (loginTerminal.LoginTerminalID < 0)
                    loginTerminal.LoginTerminalID = NextLoginTerminalRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LoginTerminalID", loginTerminal.LoginTerminalID);
                    command.Parameters.AddWithValue("@LocationID", loginTerminal.LocationID);
                    command.Parameters.AddWithValue("@TerminalName", loginTerminal.TerminalName);
                    command.Parameters.AddWithValue("@TerminalIP", loginTerminal.TerminalIP);
                    command.Parameters.AddWithValue("@FavouriteID", loginTerminal.FavouriteID);
                    command.Parameters.AddWithValue("@ReaderType", loginTerminal.ReaderType);
                    command.Parameters.AddWithValue("@ReaderPort", loginTerminal.ReaderPort);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        loginTerminal.HasChanged = false;
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

        public void InsertNewLoginTerminalMachine(LoginTerminalMachine loginTerminalMachine)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblLoginTerminalMachines]
                                   ([LoginTermMachineID]
                                   ,[LoginTerminalID]
                                   ,[Machine_idJensen]
                                   ,[FavouriteID])
                             VALUES
                                   (@LoginTermMachineID
                                   ,@LoginTerminalID
                                   ,@Machine_idJensen
                                   ,@FavouriteID)";
            try
            {
                if (loginTerminalMachine.LoginTermMachineID < 0)
                    loginTerminalMachine.LoginTermMachineID = NextLoginTerminalMachineRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LoginTermMachineID", loginTerminalMachine.LoginTermMachineID);
                    command.Parameters.AddWithValue("@LoginTerminalID", loginTerminalMachine.LoginTerminalID);
                    command.Parameters.AddWithValue("@Machine_idJensen", loginTerminalMachine.Machine_idJensen);
                    command.Parameters.AddWithValue("@FavouriteID", loginTerminalMachine.FavouriteID);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        loginTerminalMachine.HasChanged = false;
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

        public void InsertNewTag(Tag tag)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblTags]
                                    ([TagID]
                                    ,[TagData]
                                    ,[ReferenceTable]
                                    ,[ReferenceID])
                                VALUES
                                    (@TagID
                                    ,@TagData
                                    ,@ReferenceTable
                                    ,@ReferenceID)";
            try
            {
                if (tag.TagID < 0)
                    tag.TagID = NextTagRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@TagID", tag.TagID);
                    command.Parameters.AddWithValue("@TagData", tag.TagData);
                    command.Parameters.AddWithValue("@ReferenceTable", tag.ReferenceTable);
                    command.Parameters.AddWithValue("@ReferenceID", tag.ReferenceID);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        tag.HasChanged = false;
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

        public void InsertRemoteOperatorLog(RemoteOperatorLog rec)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblRemoteOperatorLog]
                               ([LogTime]
                               ,[MachineID]
                               ,[OperatorID]
                               ,[SubID]
                               ,[State])
                         VALUES
                               (@LogTime
                               ,@MachineID
                               ,@OperatorID
                               ,@SubID
                               ,@State)";
            try
            {
                if (rec.RemoteOperatorLogID < 0)
                    rec.RemoteOperatorLogID = NextRemoteOperatorLog();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    //command.Parameters.AddWithValue("@RemoteOperatorLogID", rec.RemoteOperatorLogID);
                    command.Parameters.AddWithValue("@LogTime", rec.LogTime);
                    command.Parameters.AddWithValue("@MachineID", rec.MachineID);
                    command.Parameters.AddWithValue("@OperatorID", rec.OperatorID);
                    command.Parameters.AddWithValue("@SubID", rec.SubID);
                    command.Parameters.AddWithValue("@State", rec.State);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        rec.HasChanged = false;
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

        public void UpdateTagReaderLocation(TagReaderLocation tagReaderLocation)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblTagReaderLocations]
                                   SET [LocationIP] = @LocationIP
                                      ,[Machine_idJensen] = @Machine_idJensen
                                      ,[SubID] = @SubID
                                 WHERE [LocationID] = @LocationID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LocationID", tagReaderLocation.LocationID);
                    command.Parameters.AddWithValue("@LocationIP", tagReaderLocation.LocationIP);
                    command.Parameters.AddWithValue("@Machine_idJensen", tagReaderLocation.Machine_idJensen);
                    command.Parameters.AddWithValue("@SubID", tagReaderLocation.SubID);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    tagReaderLocation.HasChanged = false;
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

        public void UpdateLoginTerminalMachine(LoginTerminalMachine loginTerminalMachine)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblLoginTerminalMachines]
                                SET [LoginTerminalID] = @LoginTerminalID
                                    ,[Machine_idJensen] = @Machine_idJensen
                                    ,[FavouriteID] = @FavouriteID
                                WHERE [LoginTermMachineID] = @LoginTermMachineID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LoginTermMachineID", loginTerminalMachine.LoginTermMachineID);
                    command.Parameters.AddWithValue("@LoginTerminalID", loginTerminalMachine.LoginTerminalID);
                    command.Parameters.AddWithValue("@Machine_idJensen", loginTerminalMachine.Machine_idJensen);
                    command.Parameters.AddWithValue("@FavouriteID", loginTerminalMachine.FavouriteID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    loginTerminalMachine.HasChanged = false;
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

        public void UpdateLoginTerminal(LoginTerminal loginTerminal)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblLoginTerminals]
                                SET [LocationID] = @LocationID
                                    ,[TerminalName] = @TerminalName
                                    ,[TerminalIP] = @TerminalIP
                                    ,[FavouriteID] = @FavouriteID
                                    ,[ReaderType] = @ReaderType
                                    ,[ReaderPort] = @ReaderPort
                                WHERE [LoginTerminalID] = @LoginTerminalID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@LoginTerminalID", loginTerminal.LoginTerminalID);
                    command.Parameters.AddWithValue("@LocationID", loginTerminal.LocationID);
                    command.Parameters.AddWithValue("@TerminalName", loginTerminal.TerminalName);
                    command.Parameters.AddWithValue("@TerminalIP", loginTerminal.TerminalIP);
                    command.Parameters.AddWithValue("@FavouriteID", loginTerminal.FavouriteID);
                    command.Parameters.AddWithValue("@ReaderType", loginTerminal.ReaderType);
                    command.Parameters.AddWithValue("@ReaderPort", loginTerminal.ReaderPort);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    loginTerminal.HasChanged = false;
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

        public void UpdateTag(Tag tag)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblTags]
                                SET [TagData] = @TagData
                                    ,[ReferenceTable] = @ReferenceTable
                                    ,[ReferenceID] = @ReferenceID
                                WHERE [TagID] = @TagID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@TagID", tag.TagID);
                    command.Parameters.AddWithValue("@TagData", tag.TagData);
                    command.Parameters.AddWithValue("@ReferenceTable", tag.ReferenceTable);
                    command.Parameters.AddWithValue("@ReferenceID", tag.ReferenceID);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    tag.HasChanged = false;
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

        public void UpdateRemoteOperatorLog(RemoteOperatorLog rec)
        {
            const string commandString =
                            @"UPDATE [dbo].[tblRemoteOperatorLog]
                                       SET [LogTime] = @LogTime
                                          ,[MachineID] = @MachineID
                                          ,[OperatorID] = @OperatorID
                                          ,[SubID] = @SubID
                                          ,[State] = @State
                                          ,[jgLogDataRemoteID] = @jgLogDataRemoteID
                                          ,[UpdateTime] = @UpdateTime
                                     WHERE [RemoteOperatorLogID] = @RemoteOperatorLogID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RemoteOperatorLogID", rec.RemoteOperatorLogID);
                    command.Parameters.AddWithValue("@LogTime", rec.LogTime);
                    command.Parameters.AddWithValue("@MachineID", rec.MachineID);
                    command.Parameters.AddWithValue("@OperatorID", rec.OperatorID);
                    command.Parameters.AddWithValue("@SubID", rec.SubID); 
                    command.Parameters.AddWithValue("@State", rec.State);
                    command.Parameters.AddWithValue("@jgLogDataRemoteID", rec.jgLogdataRemoteID);
                    command.Parameters.AddWithValue("@UpdateTime", rec.UpdateTime);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    rec.HasChanged = false;
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

        public void DeleteTagReaderLocation(TagReaderLocation tagReaderLocation)
        {
            DeleteTableRecord("tblTagReaderLocations", "LocationID", tagReaderLocation.LocationID);
        }

        public void DeleteTag(Tag tag)
        {
            DeleteTableRecord("tblTags", "TagID", tag.TagID);
        }

        public void DeleteLoginTerminal(LoginTerminal loginTerminal)
        {
            DeleteTableRecord("tblLoginTerminals", "LoginTerminalID", loginTerminal.LoginTerminalID);
        }

        public void DeleteLoginTerminalMachine(LoginTerminalMachine loginTerminalMachine)
        {
            DeleteTableRecord("tblLoginTerminalMachines", "LoginTermMachineID", loginTerminalMachine.LoginTermMachineID);
        }

        public void DeleteRemoteOperatorLog(RemoteOperatorLog remoteOperatorLog)
        {
            DeleteTableRecord("tblRemoteOperatorLog", "RemoteOperatorLogID", remoteOperatorLog.RemoteOperatorLogID);
        }

        #endregion
    }

    #region Data Collections
    public class Tags : List<Tag>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Tag tag)
        {
            base.Add(tag);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tag));
            }
        }

        new public void Remove(Tag tag)
        {
            base.RemoveAt(this.IndexOf(tag));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tag));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblTags";
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
            int TagIDPos = dr.GetOrdinal("TagID");
            int TagDataPos = dr.GetOrdinal("TagData");
            int ReferenceTablePos = dr.GetOrdinal("ReferenceTable");
            int ReferenceIDPos = dr.GetOrdinal("ReferenceID");


            this.Clear();
            while (dr.Read())
            {
                Tag tag = new Tag()
                {
                    TagID = dr.GetInt32(TagIDPos),
                    TagData = dr.GetString(TagDataPos),
                    ReferenceTable = dr.GetString(ReferenceTablePos),
                    ReferenceID = dr.GetInt32(ReferenceIDPos),
                    HasChanged = false
                };

                this.Add(tag);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public Tag GetById(int id)
        {
            return this.Find(delegate(Tag tag)
            {
                return tag.TagID == id;
            });
        }

        public Tag GetByRefs(string refTable, int refID)
        {
            return this.Find(delegate(Tag tag)
            {
                return (tag.ReferenceTable == refTable)
                    && (tag.ReferenceID == refID);
            });
        }

        public Tag GetByTag(string tagData)
        {
            return this.Find(delegate(Tag tag)
            {
                return (tag.TagData == tagData);
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (Tag tag in this)
            {
                if (tag.IsNew)
                {
                    if (!tag.DeleteRecord)
                        da.InsertNewTag(tag);
                }
                else
                {
                    if (tag.HasChanged)
                    {
                        if (tag.DeleteRecord)
                            da.DeleteTag(tag);
                        else
                            da.UpdateTag(tag);
                    }
                }
            }
            Tags delList = new Tags();
            foreach (Tag rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (Tag rec in delList)
            {
                this.Remove(rec);
            }
        }

        public int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (Tag rec in this)
                    {
                        found |= (rec.TagID == id);
                    }
                } while (found);
            }
            return id;
        }
    }

    public class TagReaderLocations : List<TagReaderLocation>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(TagReaderLocation tagReaderLocation)
        {
            base.Add(tagReaderLocation);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tagReaderLocation));
            }
        }

        new public void Remove(TagReaderLocation tagReaderLocation)
        {
            base.RemoveAt(this.IndexOf(tagReaderLocation));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tagReaderLocation));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblTagReaderLocations";
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
            int LocationIDPos = dr.GetOrdinal("LocationID");
            int LocationIPPos = dr.GetOrdinal("LocationIP");
            int Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            int SubIDPos = dr.GetOrdinal("SubID");

            this.Clear();
            while (dr.Read())
            {
                TagReaderLocation tagReaderLocation = new TagReaderLocation()
                {
                    LocationID = dr.GetInt32(LocationIDPos),
                    LocationIP = dr.GetString(LocationIPPos),
                    Machine_idJensen = dr.GetInt32(Machine_idJensenPos),
                    SubID = dr.GetInt32(SubIDPos),
                    HasChanged = false
                };

                this.Add(tagReaderLocation);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public TagReaderLocation GetById(int id)
        {
            return this.Find(delegate(TagReaderLocation tagReaderLocation)
            {
                return tagReaderLocation.LocationID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (TagReaderLocation tagReaderLocation in this)
            {
                if (tagReaderLocation.IsNew)
                {
                    if (!tagReaderLocation.DeleteRecord)
                        da.InsertNewTagReaderLocation(tagReaderLocation);
                }
                else
                {
                    if (tagReaderLocation.DeleteRecord)
                        da.DeleteTagReaderLocation(tagReaderLocation);
                    else
                        da.UpdateTagReaderLocation(tagReaderLocation);
                }
            }
            TagReaderLocations delList = new TagReaderLocations();
            foreach (TagReaderLocation rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (TagReaderLocation rec in delList)
            {
                this.Remove(rec);
            }
        }

    }
    public class LoginTerminals : List<LoginTerminal>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(LoginTerminal loginTerminal)
        {
            base.Add(loginTerminal);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, loginTerminal));
            }
        }

        new public void Remove(LoginTerminal loginTerminal)
        {
            base.RemoveAt(this.IndexOf(loginTerminal));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, loginTerminal));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblLoginTerminals";
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
            int LoginTerminalPos = dr.GetOrdinal("LoginTerminalID");
            int LocationIDPos = dr.GetOrdinal("LocationID");
            int TerminalPos = dr.GetOrdinal("TerminalName");
            int TerminalIPPos = dr.GetOrdinal("TerminalIP");
            int FavouriteIDPos = dr.GetOrdinal("FavouriteID");
            int ReaderTypePos = dr.GetOrdinal("ReaderType");
            int ReaderPortPos = dr.GetOrdinal("ReaderPort");

            this.Clear();
            while (dr.Read())
            {
                LoginTerminal loginTerminal = new LoginTerminal()
                {
                    LoginTerminalID = dr.GetInt32(LoginTerminalPos),
                    LocationID = dr.GetInt32(LocationIDPos),
                    TerminalName = dr.GetString(TerminalPos),
                    TerminalIP = dr.GetString(TerminalIPPos),
                    FavouriteID = dr.GetInt32(FavouriteIDPos),
                    ReaderType = dr.GetInt32(ReaderTypePos),
                    ReaderPort = dr.GetString(ReaderPortPos),
                    HasChanged = false
                };

                this.Add(loginTerminal);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public LoginTerminal GetById(int id)
        {
            return this.Find(delegate(LoginTerminal loginTerminal)
            {
                return loginTerminal.LoginTerminalID == id;
            });
        }

        public LoginTerminal GetByName(string aName)
        {
            return this.Find(delegate(LoginTerminal loginTerminal)
            {
                return loginTerminal.TerminalName == aName;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (LoginTerminal loginTerminal in this)
            {
                if (loginTerminal.IsNew)
                {
                    if (!loginTerminal.DeleteRecord)
                        da.InsertNewLoginTerminal(loginTerminal);
                }
                else
                {
                    if (loginTerminal.DeleteRecord)
                        da.DeleteLoginTerminal(loginTerminal);
                    else
                        da.UpdateLoginTerminal(loginTerminal);
                }
            }
            LoginTerminals delList = new LoginTerminals();
            foreach (LoginTerminal rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (LoginTerminal rec in delList)
            {
                this.Remove(rec);
            }
        }

    }
    public class LoginTerminalMachines : List<LoginTerminalMachine>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(LoginTerminalMachine loginTerminalMachine)
        {
            base.Add(loginTerminalMachine);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, loginTerminalMachine));
            }
        }

        new public void Remove(LoginTerminalMachine loginTerminalMachine)
        {
            base.RemoveAt(this.IndexOf(loginTerminalMachine));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, loginTerminalMachine));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblLoginTerminalMachines";
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
            int LoginTermMachineIDPos = dr.GetOrdinal("LoginTermMachineID");
            int LoginTerminalIDPos = dr.GetOrdinal("LoginTerminalID");
            int Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            int FavouriteIDPos = dr.GetOrdinal("FavouriteID");

            this.Clear();
            while (dr.Read())
            {
                LoginTerminalMachine loginTerminalMachine = new LoginTerminalMachine()
                {
                    LoginTermMachineID = dr.GetInt32(LoginTermMachineIDPos),
                    LoginTerminalID = dr.GetInt32(LoginTerminalIDPos),
                    Machine_idJensen = dr.GetInt32(Machine_idJensenPos),
                    FavouriteID = dr.GetInt32(FavouriteIDPos),
                    HasChanged = false
                };

                this.Add(loginTerminalMachine);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public LoginTerminalMachine GetById(int id)
        {
            return this.Find(delegate(LoginTerminalMachine loginTerminalMachine)
            {
                return loginTerminalMachine.LoginTermMachineID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (LoginTerminalMachine loginTerminalMachine in this)
            {
                if (loginTerminalMachine.IsNew)
                {
                    if (!loginTerminalMachine.DeleteRecord)
                        da.InsertNewLoginTerminalMachine(loginTerminalMachine);
                }
                else
                {
                    if (loginTerminalMachine.DeleteRecord)
                        da.DeleteLoginTerminalMachine(loginTerminalMachine);
                    else
                        da.UpdateLoginTerminalMachine(loginTerminalMachine);
                }
            }
            LoginTerminalMachines delList = new LoginTerminalMachines();
            foreach (LoginTerminalMachine rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (LoginTerminalMachine rec in delList)
            {
                this.Remove(rec);
            }
        }

    }
    public class RemoteOperatorLogs : List<RemoteOperatorLog>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(RemoteOperatorLog rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(RemoteOperatorLog rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblRemoteOperatorLog";
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
            int RemoteOperatorLogIDPos = dr.GetOrdinal("RemoteOperatorLogID");
            int LogTimePos = dr.GetOrdinal("LogTime");
            int OperatorIDPos = dr.GetOrdinal("OperatorID");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int StatePos = dr.GetOrdinal("State");
            int jgLogdataRemoteIDPos = dr.GetOrdinal("jgLogdataRemoteID");
            int UpdateTimePos = dr.GetOrdinal("UpdateTime");

            DateTime longAgo = DateTime.Now.AddYears(-10);

            this.Clear();
            while (dr.Read())
            {
                RemoteOperatorLog remoteOperatorLog = new RemoteOperatorLog();
                remoteOperatorLog.RemoteOperatorLogID = dr.GetInt32(RemoteOperatorLogIDPos);
                remoteOperatorLog.LogTime = dr.GetDateTime(LogTimePos);
                remoteOperatorLog.OperatorID = dr.GetInt32(OperatorIDPos);
                remoteOperatorLog.MachineID = dr.GetInt32(MachineIDPos);
                remoteOperatorLog.SubID = dr.GetInt32(SubIDPos);
                remoteOperatorLog.State = dr.GetInt32(StatePos);
                remoteOperatorLog.jgLogdataRemoteID = dr.IsDBNull(jgLogdataRemoteIDPos) ? -1 : dr.GetInt32(jgLogdataRemoteIDPos);
                remoteOperatorLog.UpdateTime = dr.IsDBNull(UpdateTimePos) ? longAgo : dr.GetDateTime(UpdateTimePos);
                remoteOperatorLog.HasChanged = false;
                this.Add(remoteOperatorLog);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public RemoteOperatorLog GetById(int id)
        {
            return this.Find(delegate(RemoteOperatorLog rec)
            {
                return rec.RemoteOperatorLogID == id;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (RemoteOperatorLog rec in this)
            {
                if (rec.IsNew)
                {
                    da.InsertRemoteOperatorLog(rec);
                }
                else
                {
                    da.UpdateRemoteOperatorLog(rec);
                }
            }
        }

    }
    #endregion

    #region Items

    public class Tag : DataItem, IComparable<Tag>, INotifyPropertyChanged
    {
        #region Tag Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int TagID;
            internal string TagData;
            internal string ReferenceTable;
            internal int ReferenceID;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Tag()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        #region Data Column Properties

        public int TagID
        {
            get
            {
                return activeData.TagID;
            }
            set
            {
                if (activeData.TagID != value)
                {
                    activeData.TagID = value;
                    NotifyPropertyChanged("TagID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return TagID;
            }
            set
            {
                if (TagID != value)
                {
                    TagID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string TagData
        {
            get
            {
                return activeData.TagData;
            }
            set
            {
                if (activeData.TagData != value)
                {
                    activeData.TagData = value;
                    NotifyPropertyChanged("TagData");
                }
            }
        }

        public string ReferenceTable
        {
            get
            {
                return activeData.ReferenceTable;
            }
            set
            {
                if (activeData.ReferenceTable != value)
                {
                    activeData.ReferenceTable = value;
                    NotifyPropertyChanged("ReferenceTable");
                }
            }
        }

        public int ReferenceID
        {
            get
            {
                return activeData.ReferenceID;
            }
            set
            {
                if (activeData.ReferenceID != value)
                {
                    activeData.ReferenceID = value;
                    NotifyPropertyChanged("ReferenceID");
                }
            }
        }

        private string getReferenceName()
        {
            string aString = string.Empty;
            SqlDataAccess da=SqlDataAccess.Singleton;
            if (ReferenceTable=="tblOperators")
            {
                Operators ops = da.GetAllActiveOperators();
                if (ops != null)
                {
                    Operator op = ops.GetById(ReferenceID);
                    if (op != null)
                    {
                        aString = op.ShortDescAndID;
                    }
                }
            }
            return aString;
        }

        private void setReferenceName(string aString)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ReferenceTable == "tblOperators")
            {
                Operators ops = da.GetAllActiveOperators();
                if (ops != null)
                {
                    Operator op = ops.GetByNameID(aString);
                    if (op != null)
                    {
                        ReferenceID = op.idJensen;
                        NotifyPropertyChanged("ShortDescAndID");
                    }
                }
            }
        }

        public string ShortDescAndID
        {
            get
            {
                return getReferenceName();
            }
            set
            {
                setReferenceName(value);
            }
        }

 
        public bool DeleteRecord
        {
            get
            {
                return activeData.DeleteRecord;
            }
            set
            {
                if (activeData.DeleteRecord != value)
                {
                    activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }
        
        #endregion


        public int CompareTo(Tag other) { return TagData.CompareTo(other.TagData); }
    }

    public class TagReaderLog : DataItem, IComparable<TagReaderLog>, INotifyPropertyChanged
    {
        #region TagReaderLog Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int LogID;
            internal int LocationID;
            internal int TagID;
            internal DateTime ScanTime;
            internal int State;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public TagReaderLog()
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

        #region Data Column Properties

        public int LogID
        {
            get
            {
                return activeData.LogID;
            }
            set
            {
                if (activeData.LogID != value)
                {
                    activeData.LogID = value;
                    NotifyPropertyChanged("LogID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return LogID;
            }
            set
            {
                if (LogID != value)
                {
                    LogID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int LocationID
        {
            get
            {
                return activeData.LocationID;
            }
            set
            {
                if (activeData.LocationID != value)
                {
                    activeData.LocationID = value;
                    NotifyPropertyChanged("LocationID");
                }
            }
        }

        public int TagID
        {
            get
            {
                return activeData.TagID;
            }
            set
            {
                if (activeData.TagID != value)
                {
                    activeData.TagID = value;
                    NotifyPropertyChanged("TagID");
                }
            }
        }

        public DateTime ScanTime
        {
            get
            {
                return activeData.ScanTime;
            }
            set
            {
                if (activeData.ScanTime != value)
                {
                    activeData.ScanTime = value;
                    NotifyPropertyChanged("ScanTime");
                }
            }
        }

        public int SubID
        {
            get
            {
                return activeData.State;
            }
            set
            {
                if (activeData.State != value)
                {
                    activeData.State = value;
                    NotifyPropertyChanged("SubID");
                }
            }
        }

        #endregion

        public int CompareTo(TagReaderLog other) { return ScanTime.CompareTo(other.ScanTime); }
    }

    public class LoginTerminal : DataItem, IComparable<LoginTerminal>, INotifyPropertyChanged
    {
        #region LoginTerminals Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int LoginTerminalID;
            internal string TerminalName;
            internal string TerminalIP;
            internal int FavouriteID;
            internal int LocationID;
            internal int ReaderType;
            internal string ReaderPort;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public LoginTerminal()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        #region Data Column Properties

        public int LoginTerminalID
        {
            get
            {
                return activeData.LoginTerminalID;
            }
            set
            {
                if (activeData.LoginTerminalID != value)
                {
                    activeData.LoginTerminalID = value;
                    NotifyPropertyChanged("LoginTerminalID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return LoginTerminalID;
            }
            set
            {
                if (LoginTerminalID != value)
                {
                    LoginTerminalID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string TerminalName
        {
            get
            {
                return activeData.TerminalName;
            }
            set
            {
                if (activeData.TerminalName != value)
                {
                    activeData.TerminalName = value;
                    NotifyPropertyChanged("TerminalName");
                }
            }
        }

        public string TerminalIP
        {
            get
            {
                return activeData.TerminalIP;
            }
            set
            {
                if (activeData.TerminalIP != value)
                {
                    activeData.TerminalIP = value;
                    NotifyPropertyChanged("TerminalIP");
                }
            }
        }

        public int LocationID
        {
            get
            {
                return activeData.LocationID;
            }
            set
            {
                if (activeData.LocationID != value)
                {
                    activeData.LocationID = value;
                    NotifyPropertyChanged("LocationID");
                }
            }
        }

        public int ReaderType
        {
            get
            {
                return activeData.ReaderType;
            }
            set
            {
                if (activeData.ReaderType != value)
                {
                    activeData.ReaderType = value;
                    NotifyPropertyChanged("ReaderType");
                }
            }
        }

        public string ReaderPort
        {
            get
            {
                return activeData.ReaderPort;
            }
            set
            {
                if (activeData.ReaderPort != value)
                {
                    activeData.ReaderPort = value;
                    NotifyPropertyChanged("ReaderPort");
                }
            }
        }

        public int FavouriteID
        {
            get
            {
                return activeData.FavouriteID;
            }
            set
            {
                if (activeData.FavouriteID != value)
                {
                    activeData.FavouriteID = value;
                    NotifyPropertyChanged("FavouriteID");
                }
            }
        }

        public string FavouriteName
        {
            get
            {
                return GetFavouriteName(FavouriteID);
            }
            set
            {
                SetFavouriteID(value);
                NotifyPropertyChanged("FavouriteName");                
            }
        }


        private string GetFavouriteName(int id)
        {
            string aString = "No Favourite";
            if (id > 0)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioFavourites fs = da.GetAllVisioFavourites();
                if (fs != null)
                {
                    VisioFavourite f = fs.GetById(id);
                    if (f != null)
                        aString = f.FavouriteName;
                }
            }
            return aString;
        }

        private void SetFavouriteID(string favouriteName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioFavourites fs = da.GetAllVisioFavourites();
            if (fs != null)
            {
                VisioFavourite f = fs.GetByName(favouriteName);
                if (f != null)
                {
                    id = f.FavouriteID;
                }
            }
            FavouriteID = id;
        }

        public bool DeleteRecord
        {
            get
            {
                return activeData.DeleteRecord;
            }
            set
            {
                if (activeData.DeleteRecord != value)
                {
                    activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion


        public int CompareTo(LoginTerminal other) { return TerminalName.CompareTo(other.TerminalName); }
    }

    public class LoginTerminalMachine : DataItem, INotifyPropertyChanged
    {
        #region MachineLoginTerminals Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int LoginTermMachineID;
            internal int LoginTerminalID;
            internal int Machine_idJensen;
            internal int FavouriteID;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public LoginTerminalMachine()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        #region Data Column Properties

        public int LoginTermMachineID
        {
            get
            {
                return activeData.LoginTermMachineID;
            }
            set
            {
                if (activeData.LoginTermMachineID != value)
                {
                    activeData.LoginTermMachineID = value;
                    NotifyPropertyChanged("LoginTermMachineID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return LoginTermMachineID;
            }
            set
            {
                if (LoginTermMachineID != value)
                {
                    LoginTermMachineID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        public int LoginTerminalID
        {
            get
            {
                return activeData.LoginTerminalID;
            }
            set
            {
                if (activeData.LoginTerminalID != value)
                {
                    activeData.LoginTerminalID = value;
                    NotifyPropertyChanged("LoginTerminalID");
                }
            }
        }


        public string TerminalName
        {
            get
            {
                return GetTerminalName(LoginTerminalID);
            }
            set
            {
                SetTerminalID(value);
                NotifyPropertyChanged("TerminalName");
            }
        }

        private string GetTerminalName(int id)
        {
            string aString = "No Terminal";
            if (id > 0)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                LoginTerminals lts = da.GetAllLoginTerminals();
                if (lts != null)
                {
                    LoginTerminal lt = lts.GetById(id);
                    if (lt != null)
                        aString = lt.TerminalName;
                }
            }
            return aString;
        }

        private void SetTerminalID(string terminalName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            LoginTerminals lts = da.GetAllLoginTerminals();
            if (lts != null)
            {
                LoginTerminal lt = lts.GetByName(terminalName);
                if (lt != null)
                {
                    id = lt.LoginTerminalID;
                }
            }
            LoginTerminalID = id;
        }

        public int Machine_idJensen
        {
            get
            {
                return activeData.Machine_idJensen;
            }
            set
            {
                if (activeData.Machine_idJensen != value)
                {
                    activeData.Machine_idJensen = value;
                    NotifyPropertyChanged("Machine_idJensen");
                }
            }
        }

        public string MachineName
        {
            get
            {
                return GetMachineName(Machine_idJensen);
            }
            set
            {
                SetMachineID(value);
                NotifyPropertyChanged("MachineName");
            }
        }

        private string GetMachineName(int id)
        {
            string aString = "No Machine";
            if (id > 0)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetById(id);
                    if (m != null)
                        aString = m.ShortDescAndID;
                }
            }
            return aString;
        }

        private void SetMachineID(string machineName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines();
            if (ms != null)
            {
                Machine m = ms.GetByNameID(machineName);
                if (m != null)
                {
                    id = m.idJensen;
                }
            }
            Machine_idJensen = id;
        }
        
        public int FavouriteID
        {
            get
            {
                return activeData.FavouriteID;
            }
            set
            {
                if (activeData.FavouriteID != value)
                {
                    activeData.FavouriteID = value;
                    NotifyPropertyChanged("FavouriteID");
                }
            }
        }

        public string FavouriteName
        {
            get
            {
                return GetFavouriteName(FavouriteID);
            }
            set
            {
                SetFavouriteID(value);
                NotifyPropertyChanged("FavouriteName");
            }
        }


        private string GetFavouriteName(int id)
        {
            string aString = "No Favourite";
            if (id > 0)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                VisioFavourites fs = da.GetAllVisioFavourites();
                if (fs != null)
                {
                    VisioFavourite f = fs.GetById(id);
                    if (f != null)
                        aString = f.FavouriteName;
                }
            }
            return aString;
        }

        private void SetFavouriteID(string favouriteName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            VisioFavourites fs = da.GetAllVisioFavourites();
            if (fs != null)
            {
                VisioFavourite f = fs.GetByName(favouriteName);
                if (f != null)
                {
                    id = f.FavouriteID;
                }
            }
            FavouriteID = id;
        }

        public bool DeleteRecord
        {
            get
            {
                return activeData.DeleteRecord;
            }
            set
            {
                if (activeData.DeleteRecord != value)
                {
                    activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion


    }

    public class TagReaderLocation : DataItem, IComparable<TagReaderLocation>, INotifyPropertyChanged
    {
        #region TagReaderLocations Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int LocationID;
            internal string LocationIP;
            internal bool MultiMachines;
            internal int Machine_idJensen;
            internal int SubID;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public TagReaderLocation()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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


        #region Data Column Properties

        public int LocationID
        {
            get
            {
                return activeData.LocationID;
            }
            set
            {
                if (activeData.LocationID != value)
                {
                    activeData.LocationID = value;
                    NotifyPropertyChanged("LocationID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return LocationID;
            }
            set
            {
                if (LocationID != value)
                {
                    LocationID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string LocationIP
        {
            get
            {
                return activeData.LocationIP;
            }
            set
            {
                if (activeData.LocationIP != value)
                {
                    activeData.LocationIP = value;
                    NotifyPropertyChanged("LocationIP");
                }
            }
        }

        public bool MultiMachines
        {
            get
            {
                return activeData.MultiMachines;
            }
            set
            {
                activeData.MultiMachines = value;
                NotifyPropertyChanged("MultiMachines");
                if (value)
                {
                    MachineName = string.Empty;
                    SubID = -1;
                }
            }
        }

        public int Machine_idJensen
        {
            get
            {
                return activeData.Machine_idJensen;
            }
            set
            {
                if (activeData.Machine_idJensen != value)
                {
                    activeData.Machine_idJensen = value;
                    NotifyPropertyChanged("Machine_idJensen");
                }
            }
        }

        public string MachineName
        {
            get
            {
                return GetMachineName(Machine_idJensen);
            }
            set
            {
                SetMachineID(value);
                if (value != string.Empty)
                    MultiMachines = false;
                NotifyPropertyChanged("MachineName");
            }
        }

        private string GetMachineName(int id)
        {
            string aString = "No Machine";
            if (id > 0)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                if (ms != null)
                {
                    Machine m = ms.GetById(id);
                    if (m != null)
                        aString = m.ShortDescAndID;
                }
            }
            return aString;
        }

        private void SetMachineID(string machineName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines();
            if (ms != null)
            {
                Machine m = ms.GetByNameID(machineName);
                if (m != null)
                {
                    id = m.idJensen;
                }
            }
            Machine_idJensen = id;
        }

        public int SubID
        {
            get
            {
                return activeData.SubID;
            }
            set
            {
                if (activeData.SubID != value)
                {
                    activeData.SubID = value;
                    NotifyPropertyChanged("SubID");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return activeData.DeleteRecord;
            }
            set
            {
                if (activeData.DeleteRecord != value)
                {
                    activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

        public int CompareTo(TagReaderLocation other) { return LocationIP.CompareTo(other.LocationIP); }
    }

    public class RemoteOperatorLog : DataItem, IComparable<RemoteOperatorLog>, INotifyPropertyChanged
    {
        #region RemoteOperatorLog Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RemoteOperatorLogID;
            internal DateTime LogTime;
            internal int State;
            internal int OperatorID;
            internal int MachineID;
            internal int SubID;
            internal int jgLogdataRemoteID;
            internal DateTime UpdateTime;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public RemoteOperatorLog()
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


        #region Data Column Properties

        public int RemoteOperatorLogID
        {
            get
            {
                return activeData.RemoteOperatorLogID;
            }
            set
            {
                if (activeData.RemoteOperatorLogID != value)
                {
                    activeData.RemoteOperatorLogID = value;
                    NotifyPropertyChanged("RemoteOperatorLogID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return RemoteOperatorLogID;
            }
            set
            {
                if (RemoteOperatorLogID != value)
                {
                    RemoteOperatorLogID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public DateTime LogTime
        {
            get
            {
                return activeData.LogTime;
            }
            set
            {
                if (activeData.LogTime != value)
                {
                    activeData.LogTime = value;
                    NotifyPropertyChanged("LogTime");
                }
            }
        }

        public int OperatorID
        {
            get
            {
                return activeData.OperatorID;
            }
            set
            {
                if (activeData.OperatorID != value)
                {
                    activeData.OperatorID = value;
                    NotifyPropertyChanged("OperatorID");
                }
            }
        }

        public int MachineID
        {
            get
            {
                return activeData.MachineID;
            }
            set
            {
                if (activeData.MachineID != value)
                {
                    activeData.MachineID = value;
                    NotifyPropertyChanged("MachineID");
                }
            }
        }

        public int SubID
        {
            get
            {
                return activeData.SubID;
            }
            set
            {
                if (activeData.SubID != value)
                {
                    activeData.SubID = value;
                    NotifyPropertyChanged("SubID");
                }
            }
        }

        public int State
        {
            get
            {
                return activeData.State;
            }
            set
            {
                if (activeData.State != value)
                {
                    activeData.State = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        public int jgLogdataRemoteID
        {
            get
            {
                return activeData.jgLogdataRemoteID;
            }
            set
            {
                if (activeData.jgLogdataRemoteID != value)
                {
                    activeData.jgLogdataRemoteID = value;
                    NotifyPropertyChanged("jgLogdataRemoteID");
                }
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return activeData.UpdateTime;
            }
            set
            {
                if (activeData.UpdateTime != value)
                {
                    activeData.UpdateTime = value;
                    NotifyPropertyChanged("UpdateTime");
                }
            }
        }

        #endregion

        public int CompareTo(RemoteOperatorLog other) { return jgLogdataRemoteID.CompareTo(other.jgLogdataRemoteID); }
    }

    #endregion


}
