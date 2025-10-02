using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ge_Mac.DataLayer;

namespace Ge_Mac.Settings
{
    public class ApplicationUserSettings
    {
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private string appName;
        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }
        private int appID = -1;
        private int userID = -1;
        private bool allowDefault = true;
        public bool AllowDefault
        {
            get { return allowDefault; }
            set { allowDefault = value; }
        }
        private bool enforceType = true;
        public bool EnforceType
        {
            get { return enforceType; }
            set { enforceType = value; }
        }


        public ApplicationUserSettings()
        {
            userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower();
            appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string[] split = appName.Split(new Char[] { '.' });
            appName = split[0].ToLower();
        }

        public AppSetting GetAppSetting(string settingName)
        {
            SqlDataAccess da = Ge_Mac.DataLayer.SqlDataAccess.Singleton;
            AppSettings appSettings = da.GetAllAppSettings();
            AppSetting appSetting = appSettings.GetByName(settingName);
            return appSetting;
        }

        public AppUserSettingJoin GetAppUserSettingJoin(string appName, string userName, string settingName)
        {
            SqlDataAccess da = Ge_Mac.DataLayer.SqlDataAccess.Singleton;
            AppUserSettingsJoin appUserSettings = da.GetAllAppUserJoinSettings();
            AppUserSettingJoin appUserSetting = appUserSettings.GetByNames(userName, appName, settingName);
            if (appUserSetting != null)
            {
                appID = appUserSetting.AppID;
                userID = appUserSetting.AppUserID;
            }
            return appUserSetting;
        }

        public string GetSetting(string settingName, int settingType)
        {
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if (appSetting != null)
            {
                if ((appSetting.SettingType == settingType) || (!enforceType))
                {
                    aString = appSetting.DefaultValue;
                }
            }
            return aString;
        }

        public void PutSetting(string settingName, string settingValue)
        {
            AppSetting appSetting = GetAppSetting(settingName);
            if (appSetting != null)
            {
                appSetting.DefaultValue = settingValue;
            }
        }

        public string GetSettingString(string settingName)
        {
            string aString = GetSetting(settingName, 0);
            return aString;
        }

        public bool GetSettingBool(string settingName)
        {
            string aString = GetSetting(settingName, 2);
            bool aBool;
            try
            {
                aBool = bool.Parse(aString);
            }
            catch
            {
                aBool = false;
            }
            return aBool;
        }

        public int GetSettingInt(string settingName)
        {
            string aString = GetSetting(settingName, 1);
            int anInt;
            try
            {
                anInt = int.Parse(aString);
            }
            catch
            {
                anInt = -1;
            }
            return anInt;
        }

        public double GetSettingDouble(string settingName)
        {
            string aString = GetSetting(settingName, 3);
            double aDouble;
            try
            {
                aDouble = double.Parse(aString);
            }
            catch
            {
                aDouble = -1;
            }
            return aDouble;
        }

        public string GetCustomSetting(string appName, string userName, string settingName, int settingType)
        {
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            string aString = string.Empty;
            if (appUserSetting != null)
            {
                if ((appUserSetting.SettingType == settingType) || (!enforceType))
                {
                    aString = appUserSetting.SettingValue;
                }
            }
            else
            {
                if (allowDefault)
                {
                    aString = GetSetting(settingName, settingType);
                }
            }
            return aString;
        }

        public void PutCustomSetting(string appName, string userName, string settingName, string settingValue)
        {
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            if (appUserSetting != null)
            {
                appUserSetting.SettingValue=settingValue;
            }
            else //new one!
            {
                //whats the appid, userid and setting id?
                SqlDataAccess da = SqlDataAccess.Singleton;
                //int nextrec = da.NextAppUserSettingRecord();
                int nextrec = da.NextAppUserSettingCached();
                AppUserSettingsJoin ausjs = da.GetAllAppUserJoinSettings();
                Apps apps = da.GetAllApps();
                App app = apps.GetByName(appName);
                AppUsers appUsers = da.GetAllAppUsers();
                AppUser appUser = appUsers.GetByName(userName);
                AppSettings appSettings = da.GetAllAppSettings();
                AppSetting appSetting = appSettings.GetByName(settingName);

                //create a new ausj for the ui                
                appUserSetting = new AppUserSettingJoin();
                appUserSetting.AppUserSettingID = nextrec;
                appUserSetting.AppID = app.AppID;
                appUserSetting.AppName = app.AppName;
                appUserSetting.AppUserID = appUser.AppUserID;
                appUserSetting.UserName = appUser.UserName;
                appUserSetting.UserDesc = appUser.UserDesc;
                appUserSetting.SettingID = appSetting.SettingID;
                appUserSetting.SettingName = appSetting.SettingName;
                appUserSetting.SettingType = appSetting.SettingType;
                appUserSetting.SettingValue = settingValue;
                ausjs.Add(appUserSetting);   

            }
        }

        public void DelCustomSetting(string appName, string userName, string settingName)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            AppUserSettingsJoin ausjs = da.GetAllAppUserJoinSettings();
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            if (appUserSetting != null)
            {
                ausjs.Remove(appUserSetting);
            }
        }


        public string GetCustomSettingString(string appName, string userName, string settingName)
        {
            string aString = GetCustomSetting(appName, userName, settingName, 0);
            return aString;
        }

        public string GetCustomSettingString(string settingName)
        {
            string aString = GetCustomSettingString(appName, userName, settingName);
            return aString;
        }

        public bool GetCustomSettingBool(string appName, string userName, string settingName)
        {
            string aString = GetCustomSetting(appName, userName, settingName, 2);
            bool aBool;
            try
            {
                aBool = bool.Parse(aString);
            }
            catch
            {
                aBool = false;
            }
            return aBool;
        }

        public bool GetCustomSettingBool(string settingName)
        {
            bool aBool = GetCustomSettingBool(appName, userName, settingName);
            return aBool;
        }

        public int GetCustomSettingInt(string appName, string userName, string settingName)
        {
            string aString = GetCustomSetting(appName, userName, settingName, 1);
            int anInt;
            try
            {
                anInt = int.Parse(aString);
            }
            catch
            {
                anInt = -1;
            }
            return anInt;
        }

        public int GetCustomSettingInt(string settingName)
        {
            int anInt = GetCustomSettingInt(appName, userName, settingName);
            return anInt;
        }

        public double GetCustomSettingDouble(string appName, string userName, string settingName)
        {
            string aString = GetCustomSetting(appName, userName, settingName, 3);
            double aDouble;
            try
            {
                aDouble = double.Parse(aString);
            }
            catch
            {
                aDouble = -1;
            }
            return aDouble;
        }

        public double GetCustomSettingDouble(string settingName)
        {
            double aDouble = GetCustomSettingDouble(appName, userName, settingName);
            return aDouble;
        }

        public void SaveAllSettings()
        {
            //get all settings data 
            SqlDataAccess da = SqlDataAccess.Singleton;
            Apps apps = da.GetAllApps();
            AppUsers appUsers = da.GetAllAppUsers();
            AppSettings settings = da.GetAllAppSettings();
            AppUserSettingsJoin ausjs= da.GetAllAppUserJoinSettings();

            //delete all 
            da.DeleteAllSettings();

            //insert all default settings
            foreach (AppSetting appSetting in settings)
            {
                da.InsertNewSetting(appSetting);
            }

            //insert all cached apps
            foreach (App app in apps)
            {
                da.InsertNewApp(app);
            }

            //insert all cached users
            foreach (AppUser appUser in appUsers)
            {
                da.InsertNewUser(appUser);
            }

            //insert all cached settings with valid apps and users
            foreach (AppUserSettingJoin ausj in ausjs)
            {
                App app = apps.GetById(ausj.AppID);
                AppUser au = appUsers.GetById(ausj.AppUserID);
                if ((app != null) && (au != null))
                {
                    AppUserSetting aus = new AppUserSetting();
                    aus.AppID = ausj.AppID;
                    aus.AppUserID = ausj.AppUserID;
                    aus.SettingID = ausj.SettingID;
                    aus.SettingValue = ausj.SettingValue;
                    aus.AppUserSettingID = da.NextAppUserSettingRecord();
                    //aus.AppUserSettingID = da.NextAppUserSettingCached();
                    da.InsertNewAppUserSetting(aus);
                }
            }
            da.InvalidateSettings();
        }


        public string GetAppSettingString(string settingName)
        {
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if (appSetting != null)
            {
                aString = appSetting.DefaultValue;
            }
            return aString;
        }

        public bool GetAppSettingBool(string settingName)
        {
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if ((appSetting != null) && (appSetting.SettingType == 2))
            {
                aString = appSetting.DefaultValue;
            }
            return ((aString == "True") || (aString == "true"));
        }

        public int GetAppSettingInt(string settingName)
        {
            int anInt = -1;
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if ((appSetting != null) && (appSetting.SettingType == 1))
            {
                aString = appSetting.DefaultValue;
            }
            try
            {
                anInt = int.Parse(aString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Bad integer setting for " + settingName + " set to " + aString
                    + ". " + ex.Message);
            }
            return anInt;
        }

        public long GetAppSettingLong(string settingName)
        {
            long aLong = -1;
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if ((appSetting != null) && (appSetting.SettingType == 4))
            {
                aString = appSetting.DefaultValue;
            }
            try
            {
                aLong = long.Parse(aString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Bad long setting for " + settingName + " set to " + aString
                    + ". " + ex.Message);
            }
            return aLong;
        }

        public double GetAppSettingDouble(string settingName)
        {
            double adouble = -1;
            AppSetting appSetting = GetAppSetting(settingName);
            string aString = string.Empty;
            if ((appSetting != null) && (appSetting.SettingType == 3))
            {
                aString = appSetting.DefaultValue;
            }
            try
            {
                adouble = double.Parse(aString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Bad double setting for " + settingName + " set to " + aString
                    + ". " + ex.Message);
            }
            return adouble;
        }

        public string GetAppUserSettingString(string settingName)
        {
            string aString = GetAppUserSettingString(appName, userName, settingName);
            return aString;
        }

        public string GetAppUserSettingString(string appName, string userName, string settingName)
        {
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            string aString = string.Empty;
            if (appUserSetting != null)
            {
                aString = appUserSetting.SettingValue;
            }
            else
            {
                aString = GetAppSettingString(settingName);
            }
            return aString;
        }

        public bool GetAppUserSettingBool(string appName, string userName, string settingName)
        {
            string aString = GetAppUserSettingString(appName, userName, settingName);
            return ((aString == "True") || (aString == "true"));
        }

        public bool GetAppUserSettingBool(string settingName)
        {
            string aString = GetAppUserSettingString(appName, userName, settingName);
            return ((aString == "True") || (aString == "true"));
        }

        public int GetAppUserSettingInt(string appName, string userName, string settingName)
        {
            int anInt = -1;
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            string aString = string.Empty;
            if ((appUserSetting != null) && (appUserSetting.SettingType == 1))
            {
                {
                    aString = appUserSetting.SettingValue;
                }
                try
                {
                    anInt = int.Parse(aString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Bad integer setting for " + settingName + " set to " + aString
                        + ". " + ex.Message);
                }
            }
            else
            {
                anInt = GetAppSettingInt(settingName);
            }
            return anInt;
        }

        public int GetAppUserSettingInt(string settingName)
        {
            int anInt = GetAppUserSettingInt(appName, userName, settingName);
            return anInt;
        }

        public double GetAppUserSettingDouble(string appName, string userName, string settingName)
        {
            double aDouble = -1;
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            string aString = string.Empty;
            if ((appUserSetting != null) && (appUserSetting.SettingType == 3))
            {
                {
                    aString = appUserSetting.SettingValue;
                }
                try
                {
                    aDouble = double.Parse(aString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Bad double setting for " + settingName + " set to " + aString
                        + ". " + ex.Message);
                }
            }
            else
            {
                aDouble = GetAppSettingDouble(settingName);
            }
            return aDouble;
        }

        public double GetAppUserSettingDouble(string settingName)
        {
            double aDouble = GetAppUserSettingDouble(appName, userName, settingName);
            return aDouble;
        }

        public long GetAppUserSettingLong(string appName, string userName, string settingName)
        {
            long aLong = -1;
            AppUserSettingJoin appUserSetting = GetAppUserSettingJoin(appName, userName, settingName);
            string aString = string.Empty;
            if ((appUserSetting != null) && (appUserSetting.SettingType == 4))
            {
                {
                    aString = appUserSetting.SettingValue;
                }
                try
                {
                    aLong = long.Parse(aString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Bad long setting for " + settingName + " set to " + aString
                        + ". " + ex.Message);
                }
            }
            else
            {
                aLong = GetAppSettingLong(settingName);
            }
            return aLong;
        }

        public long GetAppUserSettingLong(string settingName)
        {
            long aLong = GetAppUserSettingLong(appName, userName, settingName);
            return aLong;
        }

        public void PutAppSetting(AppSetting newAppSetting)
        {
            try
            {
                SqlDataAccess da = Ge_Mac.DataLayer.SqlDataAccess.Singleton;
                AppSetting appSetting = GetAppSetting(newAppSetting.SettingName);
                if (appSetting != null)
                {
                    if (appSetting.DefaultValue != newAppSetting.DefaultValue)
                    {
                        appSetting.DefaultValue = newAppSetting.DefaultValue;
                        da.UpdateSetting(appSetting);
                    }
                }
                else
                {
                    //newAppSetting.SettingID = da.NextSettingRecord();
                    newAppSetting.SettingID = da.NextSettingCached();
                    da.InsertNewSetting(newAppSetting);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Store Setting Failed " + ex.Message);
            }
        }

        public void UserUpdateSetting(string settingName, string settingValue)
        {
            try
            {
                SqlDataAccess da = Ge_Mac.DataLayer.SqlDataAccess.Singleton;
                AppUserSettingJoin ausJ = GetAppUserSettingJoin(appName, userName, settingName);
                AppUserSetting aus;
                if (ausJ != null) //does the user setting exist?
                {
                    aus = da.GetAppUserSetting(ausJ.AppUserSettingID);
                    if (aus.SettingValue != settingValue)
                    {
                        aus.SettingValue = settingValue;
                        da.UpdateAppUserSetting(aus);
                    }
                }
                else
                {
                    aus = new AppUserSetting();
                    //aus.AppUserSettingID = da.NextAppUserSettingRecord();
                    aus.AppUserSettingID = da.NextAppUserSettingCached();
                    AppUser appUser = da.GetAppUser(userName);
                    if (appUser == null) //does current user need creating?
                    {
                        appUser = new AppUser();
                        //appUser.AppUserID = da.NextAppUserRecord();
                        appUser.AppUserID = da.NextAppUserCached();
                        appUser.UserName = userName;
                        appUser.UserDesc = userName + " auto created in " + appName;
                        da.InsertNewUser(appUser);
                    }
                    aus.AppUserID = appUser.AppUserID;
                    App app = da.GetApp(appName);
                    aus.AppID = app.AppID;
                    AppSetting appSetting = GetAppSetting(settingName);
                    aus.SettingID = appSetting.SettingID;
                    aus.SettingValue = settingValue;
                    da.InsertNewAppUserSetting(aus);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("User Update Setting Failed for: User " + userName + ", Application "
                    + appName + ", Setting " + settingName + ", Value " + settingValue
                    + ". With the following exception: " + ex.Message);
            }
        }



    }
}
