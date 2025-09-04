using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region caches
        private AppSettings appSettingsCache = null;
        private AppUserSettingsJoin appUserSettingsJoinCache = null;
        private AppUserSettings appUserSettingsCache = null;
        private AppUsers appUsersCache = null;
        private Apps appsCache = null;
        private ReportSettings reportSettingsCache = null;

        public void InvalidateSettings()
        {
            if (appSettingsCache != null)
                appSettingsCache.IsValid = false;
            if (appUserSettingsCache != null)
                appUserSettingsCache.IsValid = false;
            if (appUserSettingsJoinCache != null)
                appUserSettingsJoinCache.IsValid = false;
            if (appUsersCache != null)
                appUsersCache.IsValid = false;
            if (appsCache != null)
                appsCache.IsValid = false;
        }

        private bool AppSettingsAreCached()
        {
            bool test = (appSettingsCache != null);
            if (test)
            {
                test = appSettingsCache.IsValid;
            }
            return test;
        }

        private bool AppsAreCached()
        {
            bool test = (appsCache != null);
            if (test)
            {
                test = appsCache.IsValid;
            }
            return test;
        }

        private bool AppUsersAreCached()
        {
            bool test = (appUsersCache != null);
            if (test)
            {
                test = appUsersCache.IsValid;
            }
            return test;
        }

        private bool AppUserSettingsAreCached()
        {
            bool test = (appUserSettingsCache != null);
            if (test)
            {
                test = appUserSettingsCache.IsValid;
            }
            return test;
        }

        private bool AppUserSettingsJoinAreCached()
        {
            bool test = (appUserSettingsJoinCache != null);
            if (test)
            {
                test = appUserSettingsJoinCache.IsValid;
            }
            return test;
        }

        private bool ReportSettingsAreCached()
        {
            bool test = (reportSettingsCache != null);
            if (test)
            {
                test = reportSettingsCache.IsValid;
            }
            return test;
        }

        #endregion

        #region Select Data
        const string allSettingsCommand =
            @"SELECT [SettingID]
                   , [SettingName]
                   , [SettingDesc]
                   , [SettingType]
                   , [DefaultValue]
              FROM [dbo].[tblSettings]
              ORDER BY SettingID";

        const string allAppsCommand =
            @"SELECT [AppID]
                   , [AppName]
                   , [AppDesc]
              FROM [dbo].[tblApps]
              ORDER BY AppID";

        const string allAppUsersCommand =
            @"SELECT [AppUserID]
                   , [UserName]
                   , [UserDesc]
              FROM [dbo].[tblAppUsers]
              ORDER BY AppUserID";

        const string allAppUserSettingsCommand =
            @"SELECT [AppUserSettingID]
                   , [SettingValue]
                   , [AppUserID]
                   , [AppID]
                   , [SettingID]
            FROM [dbo].[tblAppUserSettings]
            ORDER BY AppUserSettingID";


        const string allAppUserSettingsJoinCommand =
            @"SELECT aus.AppUserSettingID,
                aus.SettingValue,
                au.AppUserID,
                au.UserName,
                au.UserDesc,
                a.AppID,
                a.AppName,
                a.AppDesc,
                s.SettingID,
                s.SettingName,
                s.SettingDesc,
                s.SettingType,
                s.DefaultValue
            FROM [dbo].[tblAppUserSettings] aus, 
                [dbo].[tblAppUsers] au, 
                [dbo].[tblApps] a, 
                [dbo].[tblSettings] s
            WHERE 
                aus.AppID=a.AppID
                and aus.AppUserID = au.AppUserID
                and aus.SettingID = s.SettingID
            ORDER BY s.SettingID";

        public AppSettings GetAllAppSettings()
        {
            return GetAllAppSettings(false);
        }

        public AppSettings GetAllAppSettings(bool noCache)
        {
            if (!noCache && AppSettingsAreCached())
            {
                return appSettingsCache;
            }
            try
            {
                const string commandString = allSettingsCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (appSettingsCache == null)
                        appSettingsCache = new AppSettings();
                    command.DataFill(appSettingsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return appSettingsCache;
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

        public AppSetting GetAppSetting(int SettingID)
        {
            AppSettings appSettings = GetAllAppSettings();
            AppSetting appSetting = appSettings.GetById(SettingID);

            return appSetting;
        }

        public Apps GetAllApps()
        {
            return GetAllApps(false);
        }

        public Apps GetAllApps(bool noCache)
        {
            if (!noCache && AppsAreCached())
            {
                return appsCache;
            }
            try
            {
                const string commandString = allAppsCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (appsCache == null) appsCache = new Apps();
                    command.DataFill(appsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return appsCache;
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

        public App GetApp(int appID)
        {
            Apps apps = GetAllApps();
            App app = apps.GetById(appID);

            return app;
        }

        public App GetApp(string appName)
        {
            Apps apps = GetAllApps();
            App app = apps.GetByName(appName);

            return app;
        }

        public AppUsers GetAllAppUsers()
        {
            return GetAllAppUsers(false);
        }

        public AppUsers GetAllAppUsers(bool noCache)
        {
            if (!noCache && AppUsersAreCached())
            {
                return appUsersCache;
            }
            try
            {
                const string commandString = allAppUsersCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (appUsersCache == null) appUsersCache = new AppUsers();
                    command.DataFill(appUsersCache, SqlDataConnection.DBConnection.JensenGroup);
                    return appUsersCache;
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

        public AppUser GetAppUser(int appUserID)
        {
            AppUsers appUsers = GetAllAppUsers();
            AppUser appUser = appUsers.GetById(appUserID);
            return appUser;
        }

        public AppUser GetAppUser(string userName)
        {
            AppUsers appUsers = GetAllAppUsers();
            AppUser appUser = appUsers.GetByName(userName);
            return appUser;
        }

        public AppUserSettingsJoin GetAllAppUserJoinSettings()
        {
            if (AppUserSettingsJoinAreCached())
            {
                return appUserSettingsJoinCache;
            }
            try
            {
                const string commandString = allAppUserSettingsJoinCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (appUserSettingsJoinCache == null) appUserSettingsJoinCache = new AppUserSettingsJoin();
                    command.DataFill(appUserSettingsJoinCache, SqlDataConnection.DBConnection.JensenGroup);
                    return appUserSettingsJoinCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    //Debugger.Break();
                }
                throw;
            }
        }

        public AppUserSettings GetAllAppUserSettings()
        {
            return GetAllAppUserSettings(false);
        }

        public AppUserSettings GetAllAppUserSettings(bool noCache)
        {
            if (!noCache && AppUserSettingsAreCached())
            {
                return appUserSettingsCache;
            }
            try
            {
                const string commandString = allAppUserSettingsCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (appUserSettingsCache == null) appUserSettingsCache = new AppUserSettings();
                    command.DataFill(appUserSettingsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return appUserSettingsCache;
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

        public AppUserSetting GetAppUserSetting(int appUserSettingID)
        {
            AppUserSettings appUserSettings = GetAllAppUserSettings();
            AppUserSetting appUserSetting = appUserSettings.GetById(appUserSettingID);
            return appUserSetting;
        }

        public ReportSettings GetCurrentReportSettings(int appID, int userID)
        {
            ReportSettings rss = new ReportSettings();
            ReportSettings reportSettings = GetAllReportSettings();
            foreach (ReportSetting rs in reportSettings)
            {
                if (((rs.AppID == 0) || (rs.AppID == appID))
                    && ((rs.AppUserID == 0) || (rs.AppUserID == userID))
                    && rs.IsActive)
                    rss.Add(rs);
            }
            return rss;
        }

        public ReportSettings GetAllReportSettings()
        {
            return GetAllReportSettings(false);
        }

        public ReportSettings GetAllReportSettings(bool noCache)
        {
            if (!noCache && AppsAreCached())
            {
                return reportSettingsCache;
            }
            try
            {
                const string commandString = @"SELECT [SettingID]
                                                  ,[ReportID]
                                                  ,[AppID]
                                                  ,[AppUserID]
                                                  ,[AreaID]
                                                  ,[TreeName]
                                                  ,[BranchName]
                                                  ,[SubTree]
                                                  ,[Title]
                                                  ,[Description]
                                                  ,[GroupIDs]
                                                  ,[MachineIDs]
                                                  ,[SubIDs]
                                                  ,[Switches]
                                                  ,[IsActive]
                                              FROM [dbo].[tblReportSettings]";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (reportSettingsCache == null) reportSettingsCache = new ReportSettings();
                    command.DataFill(reportSettingsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return reportSettingsCache;
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

        public ReportDefaults CreateCockpitDefaultReports()
        {
            ReportDefaults cockpitReportDefaults = new ReportDefaults();
            ReportDefault rd = new ReportDefault(101, "Washroom", "", "Production by Customer/Article");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(102, "Washroom", "", "Production by Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(103, "Washroom", "", "Production by Process/Machine");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(104, "Washroom", "", "Daily Production by Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(105, "Washroom", "", "Batch Details");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(106, "Washroom", "", "Batch by Customer");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(107, "Washroom", "", "Batch by Process");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(108, "Washroom", "", "Batch Tracking");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(109, "Washroom", "", "Batch Track & Trace");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(110, "Washroom", "", "Chemicals by Machine");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(111, "Washroom", "", "Chemicals by Batch");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(112, "Washroom", "", "Chemical by Cust. & Cat.");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(201, "Flatwork Finishing", "", "by Customer/Article");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(202, "Flatwork Finishing", "", "by Article/Machine");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(203, "Flatwork Finishing", "", "by Machine/Process");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(204, "Flatwork Finishing", "", "by Machine/Process/Station");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(301, "Garment Finishing", "", "Loaded by Customer/Article");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(302, "Garment Finishing", "", "Loaded by Article/Machine");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(303, "Garment Finishing", "", "Loaded by Machine/Process");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(304, "Garment Finishing", "", "by Machine/Process/Station");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(401, "Operator", "", "Operator by Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(402, "Operator", "", "Login/Logout");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(403, "Operator", "", "Pieces per Hour");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(404, "Operator", "", "Operator Production Time");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(501, "General", "", "Events by Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(502, "General", "", "Events Detailed by Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(503, "General", "", "Most Frequent Events");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(504, "General", "", "Production Result");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(505, "General", "", "Synthesis of Production");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(601, "List", "", "Articles");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(602, "List", "", "Customers");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(603, "List", "", "Machines");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(604, "List", "", "Operators");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(605, "List", "", "Cust/Art/Proc.");
            cockpitReportDefaults.Add(rd);
            rd = new ReportDefault(606, "List", "", "Sort Category");
            cockpitReportDefaults.Add(rd);

            return cockpitReportDefaults;
        }




        #endregion

        #region Next Record

        public int NextSettingRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblSettings',
		                                            @idName = N'SettingID'
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

        public int NextSettingCached()
        {
            AppSettings settings = GetAllAppSettings();
            int maxSetting = 0;
            foreach (AppSetting setting in settings)
            {
                if (setting.SettingID > maxSetting)
                {
                    maxSetting = setting.SettingID;
                }
            }
            return maxSetting + 1;
        }

        public int NextAppUserRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblAppUsers',
		                                            @idName = N'AppUserID'
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

        public int NextAppUserCached()
        {
            AppUsers users = GetAllAppUsers();
            int maxUser = 0;
            foreach (AppUser aUser in users)
            {
                if (aUser.AppUserID > maxUser)
                {
                    maxUser = aUser.AppUserID;
                }
            }
            return maxUser + 1;
        }

        public int NextAppRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblApps',
		                                            @idName = N'AppID'
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

        public int NextAppCached()
        {
            Apps apps = GetAllApps();
            int maxApp = 0;
            foreach (App app in apps)
            {
                if (app.AppID > maxApp)
                {
                    maxApp = app.AppID;
                }
            }
            return maxApp + 1;
        }

        public int NextAppUserSettingRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblAppUserSettings',
		                                            @idName = N'AppUserSettingID'
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

        public int NextAppUserSettingCached()
        {
            AppUserSettingsJoin ausjs = GetAllAppUserJoinSettings();
            int maxAusj = 0;
            foreach (AppUserSettingJoin ausj in ausjs)
            {
                if (ausj.AppUserSettingID > maxAusj)
                {
                    maxAusj = ausj.AppUserSettingID;
                }
            }
            return maxAusj + 1;
        }

        public int NextReportSettingRecord()
        {
            return NextRecord("tblReportSettings", "SettingID");
        }


        
        #endregion

        #region Insert Data

        public void InsertNewApp(App app)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblApps]
                    ( AppID
                    , AppName
                    , AppDesc
                    )
                    VALUES
                    ( @AppID
                    , @AppName
                    , @AppDesc
                    )";
            try
            {
                if (app.AppID < 0)
                    app.AppID = NextAppRecord(); 
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppID", app.AppID);
                    command.Parameters.AddWithValue("@AppName", app.AppName);
                    command.Parameters.AddWithValue("@AppDesc", app.AppDesc);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        app.HasChanged = false;
                        app.ForceNew = false;
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

        public void InsertNewAppUser(AppUser appUser)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblAppUsers]
                    ( AppUserID
                    , UserName
                    , UserDesc
                    )
                    VALUES
                    ( @AppUserID
                    , @UserName
                    , @UserDesc
                    )";
            try
            {
                if (appUser.AppUserID < 0)
                    appUser.AppUserID = NextAppUserRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppUserID", appUser.AppUserID);
                    command.Parameters.AddWithValue("@UserName", appUser.UserName);
                    command.Parameters.AddWithValue("@UserDesc", appUser.UserDesc);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        appUser.HasChanged = false;
                        appUser.ForceNew = false;
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

        public void InsertNewSetting(AppSetting appSetting)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblSettings]
                  ( SettingID
                  , SettingName
                  , SettingDesc
                  , SettingType
                  , DefaultValue
                  )
                  VALUES
                  ( @SettingID
                  , @SettingName
                  , @SettingDesc
                  , @SettingType
                  , @DefaultValue
                  )";

            try
            {
                if (appSetting.SettingID < 0)
                    appSetting.SettingID = NextSettingRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SettingID", appSetting.SettingID);
                    command.Parameters.AddWithValue("@SettingName", appSetting.SettingName);
                    command.Parameters.AddWithValue("@SettingDesc", appSetting.SettingDesc);
                    command.Parameters.AddWithValue("@SettingType", appSetting.SettingType);
                    command.Parameters.AddWithValue("@DefaultValue", appSetting.DefaultValue);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        appSetting.HasChanged = false;
                        appSetting.ForceNew = false;
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

        public void InsertNewAppUserSetting(AppUserSetting appUserSetting)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblAppUserSettings]
                  ( AppUserSettingID
                  , SettingValue
                  , SettingID
                  , AppUserID
                  , AppID
                  )
                  VALUES
                  ( @AppUserSettingID
                  , @SettingValue
                  , @SettingID
                  , @AppUserID
                  , @AppID
                  )";

            try
            {
                if (appUserSetting.AppUserSettingID < 0)
                    appUserSetting.AppUserSettingID = NextAppUserSettingRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppUserSettingID", appUserSetting.AppUserSettingID);
                    command.Parameters.AddWithValue("@SettingValue", appUserSetting.SettingValue);
                    command.Parameters.AddWithValue("@SettingID", appUserSetting.SettingID);
                    command.Parameters.AddWithValue("@AppUserID", appUserSetting.AppUserID);
                    command.Parameters.AddWithValue("@AppID", appUserSetting.AppID);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        appUserSetting.HasChanged = false;
                        appUserSetting.ForceNew = false;
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

        public void InsertNewReportSetting(ReportSetting reportSetting)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblReportSettings]
                               ([SettingID]
                               ,[ReportID]
                               ,[AppID]
                               ,[AppUserID]
                               ,[AreaID]
                               ,[TreeName]
                               ,[BranchName]
                               ,[SubTree]
                               ,[Title]
                               ,[Description]
                               ,[GroupIDs]
                               ,[MachineIDs]
                               ,[SubIDs]
                               ,[Switches]
                               ,[IsActive])
                         VALUES
                               (@SettingID
                               ,@ReportID
                               ,@AppID
                               ,@AppUserID
                               ,@AreaID
                               ,@TreeName
                               ,@BranchName
                               ,@SubTree
                               ,@Title
                               ,@Description
                               ,@GroupIDs
                               ,@MachineIDs
                               ,@SubIDs
                               ,@Switches
                               ,@IsActive)";

            try
            {
                if (reportSetting.SettingID < 0)
                    reportSetting.SettingID = NextReportSettingRecord();
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SettingID", reportSetting.SettingID);
                    command.Parameters.AddWithValue("@ReportID", reportSetting.ReportID);
                    command.Parameters.AddWithValue("@AppID", reportSetting.AppID);
                    command.Parameters.AddWithValue("@AppUserID", reportSetting.AppUserID);
                    command.Parameters.AddWithValue("@AreaID", reportSetting.AreaID);
                    command.Parameters.AddWithValue("@TreeName", reportSetting.TreeName);
                    command.Parameters.AddWithValue("@BranchName", reportSetting.BranchName);
                    command.Parameters.AddWithValue("@SubTree", reportSetting.SubTree);
                    command.Parameters.AddWithValue("@Title", reportSetting.Title);
                    command.Parameters.AddWithValue("@Description", reportSetting.Description);
                    command.Parameters.AddWithValue("@GroupIDs", reportSetting.GroupIDs);
                    command.Parameters.AddWithValue("@MachineIDs", reportSetting.MachineIDs);
                    command.Parameters.AddWithValue("@SubIDs", reportSetting.SubIDs);
                    command.Parameters.AddWithValue("@Switches", reportSetting.Switches);
                    command.Parameters.AddWithValue("@IsActive", reportSetting.IsActive);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        reportSetting.HasChanged = false;
                        reportSetting.ForceNew = false;
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
        public void UpdateSetting(AppSetting appSetting)
        {
            const string commandString =
                @"UPDATE [dbo].[tblSettings]
                  SET SettingName = @SettingName
                    , SettingDesc = @SettingDesc
                    , SettingType = @SettingType
                    , DefaultValue = @DefaultValue
                  WHERE SettingID = @SettingID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SettingID", appSetting.SettingID);
                    command.Parameters.AddWithValue("@SettingName", appSetting.SettingName);
                    command.Parameters.AddWithValue("@SettingDesc", appSetting.SettingDesc);
                    command.Parameters.AddWithValue("@SettingType", appSetting.SettingType);
                    command.Parameters.AddWithValue("@DefaultValue", appSetting.DefaultValue);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    appSetting.HasChanged = false;
                }
                InvalidateSettings();
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

        public void UpdateApp(App app)
        {
            const string commandString =
                @"UPDATE [dbo].[tblApps]
                  SET AppName = @AppName
                    , AppDesc = @AppDesc
                  WHERE AppID = @AppID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppID", app.AppID);
                    command.Parameters.AddWithValue("@AppName", app.AppName);
                    command.Parameters.AddWithValue("@AppDesc", app.AppDesc);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    app.HasChanged = false;
                }
                InvalidateSettings();
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

        public void UpdateAppUser(AppUser appUser)
        {
            const string commandString =
                @"UPDATE [dbo].[tblAppUsers]
                  SET UserName = @UserName
                    , UserDesc = @UserDesc
                  WHERE AppUserID = @AppUserID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppUserID", appUser.AppUserID);
                    command.Parameters.AddWithValue("@UserName", appUser.UserName);
                    command.Parameters.AddWithValue("@UserDesc", appUser.UserDesc);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    appUser.HasChanged = false;
                }
                InvalidateSettings();
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

        public void UpdateAppUserSetting(AppUserSetting appUserSetting)
        {
            const string commandString =
                @"UPDATE [dbo].[tblAppUserSettings]
                  SET SettingValue = @SettingValue
                    , SettingID = @SettingID
                    , AppUserID = @AppUserID
                    , AppID = @AppID
                  WHERE AppUserSettingID = @AppUserSettingID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@AppUserSettingID", appUserSetting.AppUserSettingID);
                    command.Parameters.AddWithValue("@SettingValue", appUserSetting.SettingValue);
                    command.Parameters.AddWithValue("@SettingID", appUserSetting.SettingID);
                    command.Parameters.AddWithValue("@AppUserID", appUserSetting.AppUserID);
                    command.Parameters.AddWithValue("@AppID", appUserSetting.AppID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    appUserSetting.HasChanged = false;
                }
                InvalidateSettings();
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

        public void UpdateReportSetting(ReportSetting reportSetting)
        {
            const string commandString =
                @"UPDATE [dbo].[tblReportSettings]
                               SET [ReportID] = @ReportID
                                  ,[AppID] = @AppID
                                  ,[AppUserID] = @AppUserID
                                  ,[AreaID] = @AreaID
                                  ,[TreeName] = @TreeName
                                  ,[BranchName] = @BranchName
                                  ,[SubTree] = @SubTree
                                  ,[Title] = @Title
                                  ,[Description] = @Description
                                  ,[GroupIDs] = @GroupIDs
                                  ,[MachineIDs] = @MachineIDs
                                  ,[SubIDs] = @SubIDs
                                  ,[Switches] = @Switches
                                  ,[IsActive] = @IsActive
                             WHERE [SettingID] = @SettingID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SettingID", reportSetting.SettingID);
                    command.Parameters.AddWithValue("@ReportID", reportSetting.ReportID);
                    command.Parameters.AddWithValue("@AppID", reportSetting.AppID);
                    command.Parameters.AddWithValue("@AppUserID", reportSetting.AppUserID);
                    command.Parameters.AddWithValue("@AreaID", reportSetting.AreaID);
                    command.Parameters.AddWithValue("@TreeName", reportSetting.TreeName);
                    command.Parameters.AddWithValue("@BranchName", reportSetting.BranchName);
                    command.Parameters.AddWithValue("@SubTree", reportSetting.SubTree);
                    command.Parameters.AddWithValue("@Title", reportSetting.Title);
                    command.Parameters.AddWithValue("@Description", reportSetting.Description);
                    command.Parameters.AddWithValue("@GroupIDs", reportSetting.GroupIDs);
                    command.Parameters.AddWithValue("@MachineIDs", reportSetting.MachineIDs);
                    command.Parameters.AddWithValue("@SubIDs", reportSetting.SubIDs);
                    command.Parameters.AddWithValue("@Switches", reportSetting.Switches);
                    command.Parameters.AddWithValue("@IsActive", reportSetting.IsActive);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    reportSetting.HasChanged = false;
                }
                InvalidateSettings();
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

        public void DeleteReportSetting(ReportSetting reportSetting)
        {
            DeleteTableRecord("tblReportSettings", "SettingID", reportSetting.SettingID);
        }

        public void DeleteApp(App app)
        {
            DeleteTableRecord("tblApps", "AppID", app.AppID);
        }

        public void DeleteAppUser(AppUser appUser)
        {
            DeleteTableRecord("tblAppUsers", "AppUserID", appUser.AppUserID);
        }

        public void DeleteSetting(AppSetting appSetting)
        {
            DeleteTableRecord("tblSettings", "SettingID", appSetting.SettingID);
        }

        public void DeleteAppUserSetting(AppUserSetting appUserSetting)
        {
            DeleteTableRecord("tblAppUserSettings", "AppUserSettingID", appUserSetting.AppUserSettingID);
        }

        public void DeleteSettings()
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblSettings]";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
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

        public void DeleteAppUserSettings()
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblAppUserSettings]";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
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
        public void DeleteApps()
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblApps]";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
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

        public void DeleteAppUsers()
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblAppUsers]";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
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

        public void DeleteAllSettings()
        {
            DeleteApps();
            DeleteAppUsers();
            DeleteSettings();
            DeleteAppUserSettings();
        }

 
        public void DeleteAllReportSettings()
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblReportSettings]";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
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

    #region Settings SuperClass
    [XmlRoot("GroupSettings")]
    public class GroupSettings
    {
        public AppSettings appSettings;
        public Apps apps;
        public AppUsers appUsers;
        public AppUserSettings appUserSettings;

        public GroupSettings()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            //appSettings = da.GetAllAppSettings(true);
            //apps = da.GetAllApps(true);
            //appUsers = da.GetAllAppUsers(true);
            //appUserSettings = da.GetAllAppUserSettings(true);
        }

        public GroupSettings(AppSettings s, Apps a, AppUsers u, AppUserSettings aus)
        {
            appSettings = s;
            apps = a;
            appUsers = u;
            appUserSettings = aus;
        }

        public bool Serialize(string filename)
        {
            bool test = false;
            try
            {
                GroupSettings groupSettings = new GroupSettings();
                groupSettings.appSettings = this.appSettings;
                groupSettings.apps = this.apps;
                groupSettings.appUsers = this.appUsers;
                groupSettings.appUserSettings = this.appUserSettings;
                XmlSerializer serializer = new XmlSerializer(typeof(GroupSettings));
                FileStream fs = new FileStream(filename, FileMode.Create);
                serializer.Serialize(fs, groupSettings);
                fs.Close();
                test = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return test;
        }

        private void Clear()
        {
            appSettings.Clear();
            apps.Clear();
            appUsers.Clear();
            appUserSettings.Clear();
        }

        private void Reset()
        {
            appSettings.Reset();
            apps.Reset();
            appUsers.Reset();
            appUserSettings.Reset();
        }

        public bool DeSerialize(string filename)
        {
            bool test = false;
            try
            {
                //Clear();
                //Reset();
                XmlSerializer serializer = new XmlSerializer(typeof(GroupSettings));
                FileStream fs = new FileStream(filename, FileMode.Open);
                GroupSettings tempSite = (GroupSettings)serializer.Deserialize(fs);
                fs.Close();
                foreach (AppSetting appSetting in tempSite.appSettings)
                {
                    appSettings.Add(appSetting);
                }
                foreach (App app in tempSite.apps)
                {
                    apps.Add(app);
                }
                foreach (AppUser au in tempSite.appUsers)
                {
                    appUsers.Add(au);
                }
                foreach (AppUserSetting aus in tempSite.appUserSettings)
                {
                    appUserSettings.Add(aus);
                }
                //appSettings.ForceDBUpdate();
                //apps.ForceDBUpdate();
                //appUsers.ForceDBUpdate();
                //appUserSettings.ForceDBUpdate();
                apps.ForceUpdate = true;
                appUsers.ForceUpdate = true;
                appSettings.ForceUpdate = true;
                appUserSettings.ForceUpdate = true;
                Reset();
                test = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return test;
        }
    }

    #endregion

    #region Data Collection Classes
    public class AppSettings : List<AppSetting>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        private bool allowNotify = true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (allowNotify && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(AppSetting setting)
        {
            base.Add(setting);
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, setting));
            }
        }

        new public void Remove(AppSetting setting)
        {
            base.RemoveAt(this.IndexOf(setting));
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, setting));
            }
        }
        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblSettings";
        private DateTime lastDBUpdate;
        [XmlIgnoreAttribute()]
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        [XmlIgnoreAttribute()]
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        [XmlIgnoreAttribute()]
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        [XmlIgnoreAttribute()]
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
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
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
                    foreach (AppSetting rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public int Fill(SqlDataReader dr)
        {
            int SettingIDPos = dr.GetOrdinal("SettingID");
            int SettingNamePos = dr.GetOrdinal("SettingName");
            int SettingDescPos = dr.GetOrdinal("SettingDesc");
            int SettingTypePos = dr.GetOrdinal("SettingType");
            int DefaultValuePos = dr.GetOrdinal("DefaultValue");
            allowNotify = false;
            this.Clear();
            while (dr.Read())
            {
                AppSetting appSetting = new AppSetting()
                {
                    SettingID = dr.GetInt32(SettingIDPos),
                    SettingName = dr.GetString(SettingNamePos),
                    SettingDesc = dr.GetString(SettingDescPos),
                    SettingType = dr.GetInt32(SettingTypePos),
                    DefaultValue = dr.GetString(DefaultValuePos),
                    HasChanged = false
                };

                // Add to settings collection
                this.Add(appSetting);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            allowNotify = true;

            return this.Count;
        }

        public AppSetting GetById(int id)
        {
            return this.Find(delegate(AppSetting appSetting)
            {
                return appSetting.SettingID == id;
            });
        }

        public AppSetting GetByName(string settingName)
        {
            return this.Find(delegate(AppSetting appSetting)
            {
                return appSetting.SettingName == settingName;
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (AppSetting rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewSetting(rec);
                }
            }
            else
            {
                foreach (AppSetting rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewSetting(rec);
                    }
                    else
                    {
                        if (rec.DeleteRecord)
                            da.DeleteSetting(rec);
                        else
                            da.UpdateSetting(rec);
                    }
                }
                AppSettings delList = new AppSettings();
                foreach (AppSetting rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (AppSetting rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class Apps : List<App>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        private bool allowNotify = true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (allowNotify && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(App app)
        {
            base.Add(app);
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, app));
            }
        }

        new public void Remove(App app)
        {
            base.RemoveAt(this.IndexOf(app));
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, app));
            }
        }
        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblApps";
        private DateTime lastDBUpdate;
        [XmlIgnoreAttribute()]
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        [XmlIgnoreAttribute()]
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        [XmlIgnoreAttribute()]
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        [XmlIgnoreAttribute()]
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    test = (lastDBUpdate <= lastRead);
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
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }


        public int Fill(SqlDataReader dr)
        {
            int AppIDPos = dr.GetOrdinal("AppID");
            int AppNamePos = dr.GetOrdinal("AppName");
            int AppDescPos = dr.GetOrdinal("AppDesc");
            allowNotify = false;
            this.Clear();
            while (dr.Read())
            {
                App app = new App()
                {
                    AppID = dr.GetInt32(AppIDPos),
                    AppName = dr.GetString(AppNamePos).ToLower(),
                    AppDesc = dr.GetString(AppDescPos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(app);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            allowNotify = true;
            return this.Count;
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
                    foreach (App rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public App GetById(int id)
        {
            return this.Find(delegate(App app)
            {
                return app.AppID == id;
            });
        }

        public App GetByName(string appName)
        {
            return this.Find(delegate(App app)
            {
                return (app.AppName == appName);
            });
        }

        public App GetByDesc(string appDesc)
        {
            return this.Find(delegate(App app)
            {
                return (app.AppDesc == appDesc);
            });
        }

        public void ForceDBUpdate()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.TruncateGroupTable(tblName);
            foreach (App ap in this)
            {
                if (!ap.DeleteRecord)
                {
                    da.InsertNewApp(ap);
                }
            }
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (App rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewApp(rec);
                }
            }
            else
            {
                foreach (App app in this)
                {
                    if (app.IsNew)
                    {
                        if (!app.DeleteRecord)
                            da.InsertNewApp(app);
                    }
                    else
                    {
                        if (app.DeleteRecord)
                            da.DeleteApp(app);
                        else
                            da.UpdateApp(app);
                    }
                }
                Apps delList = new Apps();
                foreach (App rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (App rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class AppUsers : List<AppUser>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        private bool allowNotify = true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (allowNotify && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(AppUser au)
        {
            base.Add(au);
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, au));
            }
        }

        new public void Remove(AppUser au)
        {
            base.RemoveAt(this.IndexOf(au));
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, au));
            }
        }
        #endregion

        #region properties and fields
        private double lifespan = 1.0;
        private string tblName = "tblAppUsers";
        private DateTime lastDBUpdate;
        [XmlIgnore]
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        [XmlIgnore]
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        [XmlIgnore]
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        [XmlIgnore]
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
        private bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        #endregion

        public int Fill(SqlDataReader dr)
        {
            int AppUserIDPos = dr.GetOrdinal("AppUserID");
            int UserNamePos = dr.GetOrdinal("UserName");
            int UserDescPos = dr.GetOrdinal("UserDesc");
            allowNotify = false;
            this.Clear();
            while (dr.Read())
            {
                AppUser appUser = new AppUser()
                {
                    AppUserID = dr.GetInt32(AppUserIDPos),
                    UserName = dr.GetString(UserNamePos).ToLower(),
                    UserDesc = dr.GetString(UserDescPos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(appUser);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            allowNotify = true;

            return this.Count;
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
                    foreach (AppUser rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public AppUser GetById(int id)
        {
            return this.Find(delegate(AppUser appUser)
            {
                return appUser.AppUserID == id;
            });
        }

        public AppUser GetByName(string userName)
        {
            return this.Find(delegate(AppUser appUser)
            {
                return (appUser.UserName == userName);
            });
        }

        public AppUser GetByDesc(string userDesc)
        {
            return this.Find(delegate(AppUser appUser)
            {
                return (appUser.UserDesc == userDesc);
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (AppUser rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewAppUser(rec);
                }
            }
            else
            {
                foreach (AppUser rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewAppUser(rec);
                    }
                    else
                    {
                        if (rec.HasChanged)
                        {
                            if (rec.DeleteRecord)
                                da.DeleteAppUser(rec);
                            else
                                da.UpdateAppUser(rec);
                        }
                    }
                }
                AppUsers delList = new AppUsers();
                foreach (AppUser rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (AppUser rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class AppUserSettings : List<AppUserSetting>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        private bool allowNotify = true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (allowNotify && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(AppUserSetting aus)
        {
            base.Add(aus);
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, aus));
            }
        }

        new public void Remove(AppUserSetting aus)
        {
            base.RemoveAt(this.IndexOf(aus));
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, aus));
            }
        }
        #endregion

        #region properties and fields
        private double lifespan = 1.0;
        private string tblName = "tblAppUserSettings";
        private DateTime lastDBUpdate;
        [XmlIgnoreAttribute()]
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        private DateTime lastRead;
        [XmlIgnoreAttribute()]
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool neverExpire = false;
        [XmlIgnoreAttribute()]
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        [XmlIgnoreAttribute()]
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

        private bool forceUpdate = false;
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }
        
        #endregion

        public int Fill(SqlDataReader dr)
        {
            int AppUserSettingIDPos = dr.GetOrdinal("AppUserSettingID");
            int SettingValuePos = dr.GetOrdinal("SettingValue");
            int AppUserIDPos = dr.GetOrdinal("AppUserID");
            int AppIDPos = dr.GetOrdinal("AppID");
            int SettingIDPos = dr.GetOrdinal("SettingID");
            allowNotify = false;
            this.Clear();
            while (dr.Read())
            {
                AppUserSetting appUserSetting = new AppUserSetting()
                {
                    AppUserSettingID = dr.GetInt32(AppUserSettingIDPos),
                    SettingValue = dr.GetString(SettingValuePos),
                    AppUserID = dr.GetInt32(AppUserIDPos),
                    AppID = dr.GetInt32(AppIDPos),
                    SettingID = dr.GetInt32(SettingIDPos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(appUserSetting);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            allowNotify = true;

            return this.Count;
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
                    foreach (AppUserSetting rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public AppUserSetting GetById(int id)
        {
            return this.Find(delegate(AppUserSetting appUserSetting)
            {
                return appUserSetting.AppUserSettingID == id;
            });
        }

        public void ForceDBUpdate()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            da.TruncateGroupTable(tblName);
            foreach (AppUserSetting aus in this)
            {
                if (!aus.DeleteRecord)
                {
                    da.InsertNewAppUserSetting(aus);
                }
            }
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (ForceUpdate)
            {
                da.TruncateGroupTable(tblName);
                foreach (AppUserSetting rec in this)
                {
                    if (!rec.DeleteRecord)
                        da.InsertNewAppUserSetting(rec);
                }
            }
            else
            {
                foreach (AppUserSetting rec in this)
                {
                    if (rec.IsNew)
                    {
                        if (!rec.DeleteRecord)
                            da.InsertNewAppUserSetting(rec);
                    }
                    else
                    {
                        if (rec.DeleteRecord)
                            da.DeleteAppUserSetting(rec);
                        else
                            da.UpdateAppUserSetting(rec);
                    }
                }
                AppUserSettings delList = new AppUserSettings();
                foreach (AppUserSetting rec in this)
                {
                    if (rec.DeleteRecord)
                        delList.Add(rec);
                }
                foreach (AppUserSetting rec in delList)
                {
                    this.Remove(rec);
                }
            }
        }
    }

    public class AppUserSettingsJoin : List<AppUserSettingJoin>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblAppUserSettings";
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
            int AppUserSettingIDPos = dr.GetOrdinal("AppUserSettingID");
            int SettingValuePos = dr.GetOrdinal("SettingValue");
            int AppUserIDPos = dr.GetOrdinal("AppUserID");
            int UserNamePos = dr.GetOrdinal("UserName");
            int UserDescPos = dr.GetOrdinal("UserDesc");
            int AppIDPos = dr.GetOrdinal("AppID");
            int AppNamePos = dr.GetOrdinal("AppName");
            int AppDescPos = dr.GetOrdinal("AppDesc");
            int SettingIDPos = dr.GetOrdinal("SettingID");
            int SettingNamePos = dr.GetOrdinal("SettingName");
            int SettingDescPos = dr.GetOrdinal("SettingDesc");
            int SettingTypePos = dr.GetOrdinal("SettingType");
            int DefaultValuePos = dr.GetOrdinal("DefaultValue");
            this.Clear();
            while (dr.Read())
            {
                AppUserSettingJoin appUserSetting = new AppUserSettingJoin()
                {
                    AppUserSettingID = dr.GetInt32(AppUserSettingIDPos),
                    SettingValue = dr.GetString(SettingValuePos),
                    AppUserID = dr.GetInt32(AppUserIDPos),
                    UserName = dr.GetString(UserNamePos).ToLower(),
                    UserDesc = dr.GetString(UserDescPos),
                    AppID = dr.GetInt32(AppIDPos),
                    AppName = dr.GetString(AppNamePos).ToLower(),
                    AppDesc = dr.GetString(AppDescPos),
                    SettingID = dr.GetInt32(SettingIDPos),
                    SettingName = dr.GetString(SettingNamePos),
                    SettingDesc = dr.GetString(SettingDescPos),
                    SettingType = dr.GetInt32(SettingTypePos),
                    DefaultValue = dr.GetString(DefaultValuePos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(appUserSetting);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public AppUserSettingJoin GetById(int id)
        {
            return this.Find(delegate(AppUserSettingJoin appUserSetting)
            {
                return appUserSetting.AppUserSettingID == id;
            });
        }

        public AppUserSettingJoin GetByNames(string userName, string appName, string settingName)
        {
            return this.Find(delegate(AppUserSettingJoin appUserSetting)
            {
                return (appUserSetting.UserName == userName)
                    && (appUserSetting.AppName == appName)
                    && (appUserSetting.SettingName == settingName);
            });
        }
    }

    public class ReportSettings : List<ReportSetting>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        private bool allowNotify = true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (allowNotify && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(ReportSetting rs)
        {
            base.Add(rs);
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rs));
            }
        }

        new public void Remove(ReportSetting rs)
        {
            base.RemoveAt(this.IndexOf(rs));
            if (allowNotify && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rs));
            }
        }
        #endregion

        #region properties and fields
        private double lifespan = 1.0;
        private string tblName = "tblReportSettings";
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

        private bool forceUpdate = false;
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }
        
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            int SettingIDPos = dr.GetOrdinal("SettingID");
            int ReportIDPos = dr.GetOrdinal("ReportID");
            int AreaIDPos = dr.GetOrdinal("AreaID");
            int AppIDPos = dr.GetOrdinal("AppID");
            int TreeNamePos = dr.GetOrdinal("TreeName");
            int BranchNamePos = dr.GetOrdinal("BranchName");
            int SubTreePos = dr.GetOrdinal("SubTree");
            int TitlePos = dr.GetOrdinal("Title");
            int DescriptionPos = dr.GetOrdinal("Description");
            int GroupIDsPos = dr.GetOrdinal("GroupIDs");
            int MachineIDsPos = dr.GetOrdinal("MachineIDs");
            int SubIDsPos = dr.GetOrdinal("SubIDs");
            int SwitchesPos = dr.GetOrdinal("Switches");
            int IsActiveIDPos = dr.GetOrdinal("IsActive");
            allowNotify = false;
            this.Clear();
            while (dr.Read())
            {
                ReportSetting rs = new ReportSetting()
                {
                    SettingID = dr.GetInt32(SettingIDPos),
                    ReportID = dr.GetInt32(ReportIDPos),
                    AppID = dr.GetInt32(AppIDPos),
                    AreaID = dr.GetInt32(AreaIDPos),
                    TreeName = dr.GetString(TreeNamePos),
                    BranchName = dr.GetString(BranchNamePos),
                    SubTree = dr.GetString(SubTreePos),
                    Title = dr.GetString(TitlePos),
                    Description = dr.GetString(DescriptionPos),
                    GroupIDs = dr.GetString(GroupIDsPos),
                    MachineIDs = dr.GetString(MachineIDsPos),
                    SubIDs = dr.GetString(SubIDsPos),
                    Switches = dr.GetString(SwitchesPos),
                    IsActive = dr.GetBoolean(IsActiveIDPos),
                    HasChanged = false
                };

                // Add to collection
                this.Add(rs);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            allowNotify = true;

            return this.Count;
        }

        #endregion


        public ReportSetting GetById(int id)
        {
            return this.Find(delegate(ReportSetting reportSetting)
            {
                return reportSetting.SettingID == id;
            });
        }

        public ReportSetting GetByFullPath(string fullPath)
        {
            return this.Find(delegate(ReportSetting reportSetting)
            {
                return (reportSetting.TreeFullPath == fullPath);
            });
        }

        public void UpdateToDB()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            foreach (ReportSetting rs in this)
            {
                if (rs.IsNew)
                {
                    if (!rs.DeleteRecord)
                        da.InsertNewReportSetting(rs);
                }
                else
                {
                    if (rs.HasChanged)
                    {
                        if (rs.DeleteRecord)
                            da.DeleteReportSetting(rs);
                        else
                            da.UpdateReportSetting(rs);
                    }
                }
            }
            ReportSettings delList = new ReportSettings();
            foreach (ReportSetting rec in this)
            {
                if (rec.DeleteRecord)
                    delList.Add(rec);
            }
            foreach (ReportSetting rec in delList)
            {
                this.Remove(rec);
            }
        }

        public HashSet<string> GetTreeNames()
        {
            HashSet<string> hash = new HashSet<string>();
            foreach (ReportSetting rs in this)
            {
                if (rs.IsActive)
                    hash.Add(rs.TreeName);
            }
            return hash;
        }

        public HashSet<string> GetSubTreeNames(string treeName)
        {
            HashSet<string> hash = new HashSet<string>();
            foreach (ReportSetting rs in this)
            {
                if (rs.TreeName == treeName && rs.SubTree!=string.Empty && rs.IsActive)
                    hash.Add(rs.SubTree);
            }
            return hash;
        }

    }

    public class ReportDefaults : List<ReportDefault>
    {

        public ReportDefault GetById(int id)
        {
            return this.Find(delegate(ReportDefault reportDefault)
            {
                return reportDefault.ReportID == id;
            });
        }

    }

    #endregion

    #region Item Classes
 
    public class AppSetting : DataItem, INotifyPropertyChanged
    {
        #region AppSetting Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int SettingID;
            internal string SettingName;
            internal string SettingDesc;
            internal int SettingType;
            internal string DefaultValue;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AppSetting()
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

        public override int ID
        {
            get
            {
                return (SettingID);
            }
            set
            {
                if (SettingID != value)
                {
                    SettingID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        #endregion

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        [XmlIgnoreAttribute()]
        public override bool IsNew
        {
            get
            {
                return ((this.activeData.SettingID == -1)||ForceNew);
            }
        }


        #endregion

        #region Data Column Properties

        public int SettingID
        {
            get
            {
                return this.activeData.SettingID;
            }
            set
            {
                if (this.activeData.SettingID != value)
                {
                    this.activeData.SettingID = value;
                    NotifyPropertyChanged("SettingID");
                }
            }
        }

        public string SettingName
        {
            get
            {
                return this.activeData.SettingName;
            }
            set
            {
                if (this.activeData.SettingName != value)
                {
                    this.activeData.SettingName = value;
                    NotifyPropertyChanged("SettingName");
                }
            }
        }

        public string SettingDesc
        {
            get
            {
                return this.activeData.SettingDesc;
            }
            set
            {
                if (this.activeData.SettingDesc != value)
                {
                    this.activeData.SettingDesc = value;
                    NotifyPropertyChanged("SettingDesc");
                }
            }
        }

        public int SettingType
        {
            get
            {
                return this.activeData.SettingType;
            }
            set
            {
                if (this.activeData.SettingType != value)
                {
                    this.activeData.SettingType = value;
                    NotifyPropertyChanged("SettingType");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public string SettingsType
        {
            get
            {
                string aString;
                switch (SettingType)
                {
                    case 0: aString = "String";
                        break;
                    case 1: aString = "Integer";
                        break;
                    case 2: aString = "Boolean";
                        break;
                    case 3: aString = "Double";
                        break;
                    case 4: aString = "Long";
                        break;
                    default: aString = string.Empty;
                        break;
                }
                return aString;
            }
            set
            {
                int anInt = SettingType;
                if (value == "String")
                    anInt = 0;
                if (value == "Integer")
                    anInt = 1;
                if (value == "Boolean")
                    anInt = 2;
                if (value == "Double")
                    anInt = 3;
                if (value == "Long")
                    anInt = 4;
                if (anInt != SettingType)
                {
                    SettingType = anInt;
                    NotifyPropertyChanged("SettingsType");
                }
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.activeData.DefaultValue;
            }
            set
            {
                if (this.activeData.DefaultValue != value)
                {
                    string astring = value;
                    if (SettingType == 2)
                    {
                        astring = astring.ToLower();
                        if (astring.Contains("t") || astring.Contains("y"))
                            astring = "True";
                        else
                        {
                            astring = "False";
                        }
                    }
                    this.activeData.DefaultValue = astring;
                    NotifyPropertyChanged("DefaultValue");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
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

    public class AppUserSettingJoin : DataItem
    {
        #region AppUserSetting Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int AppUserSettingID;
            internal string SettingValue;
            internal int AppUserID;
            internal string UserName;
            internal string UserDesc;
            internal int AppID;
            internal string AppName;
            internal string AppDesc;
            internal int SettingID;
            internal string SettingName;
            internal string SettingDesc;
            internal int SettingType;
            internal string DefaultValue;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AppUserSettingJoin()
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
  
        public override int ID
        {
            get
            {
                return (AppUserSettingID);
            }
            set
            {
                if (AppUserSettingID != value)
                {
                    AppUserSettingID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        
        #endregion



        #region Data Column Properties
        /*public override int PrimaryKey
        {
            get
            {
                return this.activeData.AppUserSettingID;
            }
            set
            {
                this.activeData.AppUserSettingID = value;
            }
        }*/

        public int SettingID
        {
            get
            {
                return this.activeData.SettingID;
            }
            set
            {
                this.activeData.SettingID = value;
                HasChanged = true;
            }
        }

        public int AppUserSettingID
        {
            get
            {
                return this.activeData.AppUserSettingID;
            }
            set
            {
                this.activeData.AppUserSettingID = value;
                HasChanged = true;
            }
        }


        public string SettingValue
        {
            get
            {
                return this.activeData.SettingValue;
            }
            set
            {
                this.activeData.SettingValue = value;
                HasChanged = true;
            }
        }

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                this.activeData.AppUserID = value;
                HasChanged = true;
            }
        }

        public string UserName
        {
            get
            {
                return this.activeData.UserName;
            }
            set
            {
                this.activeData.UserName = value;
                HasChanged = true;
            }
        }

        public string UserDesc
        {
            get
            {
                return this.activeData.UserDesc;
            }
            set
            {
                this.activeData.UserDesc = value;
                HasChanged = true;
            }
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                this.activeData.AppID = value;
                HasChanged = true;
            }
        }

        public string AppName
        {
            get
            {
                return this.activeData.AppName;
            }
            set
            {
                this.activeData.AppName = value;
                HasChanged = true;
            }
        }

        public string AppDesc
        {
            get
            {
                return this.activeData.AppDesc;
            }
            set
            {
                this.activeData.AppDesc = value;
                HasChanged = true;
            }
        }

        public string SettingName
        {
            get
            {
                return this.activeData.SettingName;
            }
            set
            {
                this.activeData.SettingName = value;
                HasChanged = true;
            }
        }

        public string SettingDesc
        {
            get
            {
                return this.activeData.SettingDesc;
            }
            set
            {
                this.activeData.SettingDesc = value;
                HasChanged = true;
            }
        }

        public int SettingType
        {
            get
            {
                return this.activeData.SettingType;
            }
            set
            {
                this.activeData.SettingType = value;
                HasChanged = true;
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.activeData.DefaultValue;
            }
            set
            {
                this.activeData.DefaultValue = value;
                HasChanged = true;
            }
        }

        #endregion

    }

    public class App : DataItem, INotifyPropertyChanged
    {
        #region App Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int AppID;
            internal string AppName;
            internal string AppDesc;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public App()
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

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        [XmlIgnoreAttribute()]
        public override bool IsNew
        {
            get
            {
                return this.activeData.AppID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        [XmlIgnoreAttribute()]
        public override bool IsExisting
        {
            get
            {
                return this.activeData.AppID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return AppID;
            }
            set
            {
                if (AppID != value)
                {
                    AppID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }



        public string AppName
        {
            get
            {
                return this.activeData.AppName;
            }
            set
            {
                if (this.activeData.AppName != value)
                {
                    this.activeData.AppName = value.ToLower();
                    NotifyPropertyChanged("AppName");
                }
            }
        }

        public string AppDesc
        {
            get
            {
                return this.activeData.AppDesc;
            }
            set
            {
                if (this.activeData.AppDesc != value)
                {
                    this.activeData.AppDesc = value;
                    NotifyPropertyChanged("AppDesc");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
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

    public class AppUser : DataItem, INotifyPropertyChanged
    {
        #region AppUser Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int AppUserID;
            internal string UserName;
            internal string UserDesc;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AppUser()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            DeleteRecord = false;
            HasChanged = false;
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

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                if (this.activeData.AppUserID != value)
                {
                    this.activeData.AppUserID = value;
                    NotifyPropertyChanged("AppUserID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return AppUserID;
            }
            set
            {
                if (AppUserID != value)
                {
                    AppUserID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string UserName
        {
            get
            {
                return this.activeData.UserName;
            }
            set
            {
                if (this.activeData.UserName != value)
                {
                    this.activeData.UserName = value.ToLower();
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        public string UserDesc
        {
            get
            {
                return this.activeData.UserDesc;
            }
            set
            {
                if (this.activeData.UserDesc != value)
                {
                    this.activeData.UserDesc = value;
                    NotifyPropertyChanged("UserDesc");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
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

    public class AppUserSetting : DataItem, INotifyPropertyChanged
    {
        #region AppUserSetting Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int AppUserSettingID;
            internal string SettingValue;
            internal int AppUserID;
            internal int AppID;
            internal int SettingID;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public AppUserSetting()
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

        public int AppUserSettingID
        {
            get
            {
                return this.activeData.AppUserSettingID;
            }
            set
            {
                if (this.activeData.AppUserSettingID != value)
                {
                    this.activeData.AppUserSettingID = value;
                    NotifyPropertyChanged("AppUserSettingID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return AppUserSettingID;
            }
            set
            {
                if (AppUserSettingID != value)
                {
                    AppUserSettingID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int SettingID
        {
            get
            {
                return this.activeData.SettingID;
            }
            set
            {
                if (this.activeData.SettingID != value)
                {
                    this.activeData.SettingID = value;
                    NotifyPropertyChanged("SettingID");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public String SettingName
        {
            get
            {
                return GetSettingName(SettingID);
            }
            set
            {
                SetSettingID(value);
                NotifyPropertyChanged("SettingName");
            }
        }

        private string GetSettingName(int uid)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppSettings aps = da.GetAllAppSettings();
            if (aps != null)
            {
                AppSetting au = aps.GetById(uid);
                if (au != null)
                    aString = au.SettingName;
            }
            return aString;
        }

        private void SetSettingID(string SettingName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppSettings aps = da.GetAllAppSettings();
            if (aps != null)
            {
                AppSetting au = aps.GetByName(SettingName);
                if (au != null)
                    id = au.SettingID;
            }
            if (id > -1)
                activeData.SettingID = id;
        }

        public string SettingValue
        {
            get
            {
                return this.activeData.SettingValue;
            }
            set
            {
                if (this.activeData.SettingValue != value)
                {
                    this.activeData.SettingValue = value;
                    NotifyPropertyChanged("SettingValue");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public String UserDesc
        {
            get
            {
                return GetUserDesc(AppUserID);
            }
            set
            {
                SetUserID(value);
                NotifyPropertyChanged("UserDesc");
            }
        }

        private string GetUserDesc(int uid)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetById(uid);
                if (au != null)
                    aString = au.UserDesc;
            }
            return aString;
        }

        private void SetUserID(string userDesc)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetByDesc(userDesc);
                if (au != null)
                    id = au.AppUserID;
            }
            if (id > -1)
                activeData.AppUserID = id;
        }

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                if (this.activeData.AppUserID != value)
                {
                    this.activeData.AppUserID = value;
                    NotifyPropertyChanged("AppUserID");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public String AppDesc
        {
            get
            {
                return GetAppDesc(AppID);
            }
            set
            {
                SetAppID(value);
                NotifyPropertyChanged("AppDesc");
            }
        }

        private string GetAppDesc(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetById(id);
                if (au != null)
                    aString = au.AppDesc;
            }
            return aString;
        }

        private void SetAppID(string appName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetByDesc(appName);
                if (au != null)
                    id = au.AppID;
            }
            if (id > -1)
                activeData.AppID = id;
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        [XmlIgnoreAttribute()]
        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
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

    public class ReportSetting : DataItem, INotifyPropertyChanged
    {
        #region ReportSetting Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int SettingID;
            internal int ReportID;
            internal int AppID;
            internal int AppUserID;
            internal int AreaID;
            internal string TreeName;
            internal string BranchName;
            internal string SubTree;
            internal string Title;
            internal string Description;
            internal string GroupIDs;
            internal string MachineIDs;
            internal string SubIDs;
            internal string Switches;
            internal bool IsActive;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }

        }
        #endregion

        #region Constructor
        public ReportSetting()
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

        public int SettingID
        {
            get
            {
                return activeData.SettingID;
            }
            set
            {
                if (activeData.SettingID != value)
                {
                    activeData.SettingID = value;
                    NotifyPropertyChanged("SettingID");
                }
            }
        }

        public override int ID
        {
            get
            {
                return SettingID;
            }
            set
            {
                if (SettingID != value)
                {
                    SettingID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public int ReportID
        {
            get
            {
                return this.activeData.ReportID;
            }
            set
            {
                if (this.activeData.ReportID != value)
                {
                    this.activeData.ReportID = value;
                    NotifyPropertyChanged("ReportID");
                }
            }
        }

        public String AppDesc
        {
            get
            {
                return GetAppDesc(AppID);
            }
            set
            {
                SetAppID(value);
                NotifyPropertyChanged("AppDesc");
            }
        }

        private string GetAppDesc(int id)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetById(id);
                if (au != null)
                    aString = au.AppDesc;
            }
            return aString;
        }

        private void SetAppID(string appName)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps aps = da.GetAllApps();
            if (aps != null)
            {
                App au = aps.GetByDesc(appName);
                if (au != null)
                    id = au.AppID;
            }
            if (id > -1)
                activeData.AppID = id;
        }

        public int AppID
        {
            get
            {
                return this.activeData.AppID;
            }
            set
            {
                if (this.activeData.AppID != value)
                {
                    this.activeData.AppID = value;
                    NotifyPropertyChanged("AppID");
                }
            }
        }

        public String UserDesc
        {
            get
            {
                return GetUserDesc(AppUserID);
            }
            set
            {
                SetUserID(value);
                NotifyPropertyChanged("UserDesc");
            }
        }

        private string GetUserDesc(int uid)
        {
            string aString = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetById(uid);
                if (au != null)
                    aString = au.UserDesc;
            }
            return aString;
        }

        private void SetUserID(string userDesc)
        {
            int id = -1;
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUsers aus = da.GetAllAppUsers();
            if (aus != null)
            {
                AppUser au = aus.GetByDesc(userDesc);
                if (au != null)
                    id = au.AppUserID;
            }
            if (id > -1)
                activeData.AppUserID = id;
        }

        public int AppUserID
        {
            get
            {
                return this.activeData.AppUserID;
            }
            set
            {
                if (this.activeData.AppUserID != value)
                {
                    this.activeData.AppUserID = value;
                    NotifyPropertyChanged("AppUserID");
                }
            }
        }
        
        public string AreaName
        {
            get
            {
                string aString = "Washroom";
                switch (AreaID)
                {
                    case 0: aString = "Washroom";
                        break;
                    case 1: aString = "Flatwork Finishing";
                        break;
                    case 2: aString = "Garment Finishing";
                        break;
                    case 3: aString = "Operator";
                        break;
                    case 4: aString = "General";
                        break;
                    case 5: aString = "List";
                        break;
                    default: aString = "Washroom";
                        break;
                }
                return aString;
            }
            set
            {
                if (value == "Washroom") AreaID = 0;
                if (value == "Flatwork Finishing") AreaID = 1;
                if (value == "Garment Finishing") AreaID = 2;
                if (value == "Operator") AreaID = 3;
                if (value == "General") AreaID = 4;
                if (value == "List") AreaID = 5;
            }
        }

        public int AreaID
        {
            get
            {
                return this.activeData.AreaID;
            }
            set
            {
                if (this.activeData.AreaID != value)
                {
                    this.activeData.AreaID = value;
                    NotifyPropertyChanged("AreaID");
                    NotifyPropertyChanged("AreaName");
                }
            }
        }

        public string TreeName
        {
            get
            {
                return this.activeData.TreeName;
            }
            set
            {
                if (this.activeData.TreeName != value)
                {
                    this.activeData.TreeName = value;
                    NotifyPropertyChanged("TreeName");
                }
            }
        }

        public string BranchName
        {
            get
            {
                return this.activeData.BranchName;
            }
            set
            {
                if (this.activeData.BranchName != value)
                {
                    this.activeData.BranchName = value;
                    NotifyPropertyChanged("BranchName");
                }
            }
        }

        public string SubTree
        {
            get
            {
                return this.activeData.SubTree;
            }
            set
            {
                if (this.activeData.SubTree != value)
                {
                    this.activeData.SubTree = value;
                    NotifyPropertyChanged("SubTree");
                }
            }
        }

        public string TreeFullPath
        {
            get
            {
                string aString = TreeName;
                if (SubTree != string.Empty)
                    aString += "/" + SubTree;
                aString += "/" + BranchName;
                return aString;
            }
        }

        public string Title
        {
            get
            {
                return this.activeData.Title;
            }
            set
            {
                if (this.activeData.Title != value)
                {
                    this.activeData.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get
            {
                return this.activeData.Description;
            }
            set
            {
                if (this.activeData.Description != value)
                {
                    this.activeData.Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public string GroupIDs
        {
            get
            {
                return this.activeData.GroupIDs;
            }
            set
            {
                if (this.activeData.GroupIDs != value)
                {
                    this.activeData.GroupIDs = value;
                    NotifyPropertyChanged("GroupIDs");
                }
            }
        }

        public string MachineIDs
        {
            get
            {
                return this.activeData.MachineIDs;
            }
            set
            {
                if (this.activeData.MachineIDs != value)
                {
                    this.activeData.MachineIDs = value;
                    NotifyPropertyChanged("MachineIDs");
                }
            }
        }

        public string SubIDs
        {
            get
            {
                return this.activeData.SubIDs;
            }
            set
            {
                if (this.activeData.SubIDs != value)
                {
                    this.activeData.SubIDs = value;
                    NotifyPropertyChanged("SubIDs");
                }
            }
        }

        public string Switches
        {
            get
            {
                return this.activeData.Switches;
            }
            set
            {
                if (this.activeData.Switches != value)
                {
                    this.activeData.Switches = value;
                    NotifyPropertyChanged("Switches");
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return this.activeData.IsActive;
            }
            set
            {
                if (this.activeData.IsActive != value)
                {
                    this.activeData.IsActive = value;
                    NotifyPropertyChanged("IsActive");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        } 
        #endregion
    }

    public class ReportDefault
    {
        public int ReportID {get;set;}
        public string TreeName {get;set;}
        public string SubTree {get;set;}
        public string BranchName {get;set;}

        public ReportDefault(int reportID, string treeName, string subTree, string branchName)
        {
            ReportID = reportID;
            TreeName = treeName;
            SubTree = subTree;
            BranchName = branchName;
        }
    }


    #endregion
}
