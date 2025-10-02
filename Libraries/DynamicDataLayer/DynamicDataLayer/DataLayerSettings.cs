using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dynamic.DataLayer
{

    #region standard settings

    partial class SqlDataAccess
    {
        public AppSettings GetAllAppSettings(AppSettings recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new AppSettings();
            recs = (AppSettings)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }
    }

    public class Apps : DataList
    {
        public Apps()
        {
            Lifespan = 1.0;
            ListType = typeof(App);
            TblName = "tblApps";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AppID", "AppID", true, false);
            dBFieldMappings.AddMapping("AppName", "AppName");
            dBFieldMappings.AddMapping("AppDesc", "AppDesc");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class App : DataItem
    {
        #region private fields

        private int appID;
        private string appName;
        private string appDesc;

        #endregion

        public int AppID
        {
            get { return appID; }
            set
            {
                appID = AssignNotify(ref appID, value, "AppID");
                ID = appID;
                PrimaryKey = appID;
            }
        }
        public string AppName
        {
            get { return appName; }
            set
            {
                appName = AssignNotify(ref appName, value, "AppName");
                ItemName = appName;
            }
        }
        public string AppDesc
        {
            get { return appDesc; }
            set { appDesc = AssignNotify(ref appDesc, value, "AppName"); }
        }
    }


    public class AppUsers : DataList
    {
        public AppUsers()
        {
            Lifespan = 1.0;
            ListType = typeof(AppUser);
            TblName = "tblAppUsers";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AppUserID", "AppUserID", true, false);
            dBFieldMappings.AddMapping("UserName", "UserName");
            dBFieldMappings.AddMapping("UserDesc", "UserDesc");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppUser : DataItem
    {
        #region private fields

        private int appUserID;
        private string userName;
        private string userDesc;


        #endregion

        public int AppUserID
        {
            get { return appUserID; }
            set
            {
                appUserID = AssignNotify(ref appUserID, value, "AppUserID");
                ID = appUserID;
                PrimaryKey = appUserID;
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = AssignNotify(ref userName, value, "UserName");
                ItemName = userName;
            }
        }

        public string UserDesc
        {
            get { return userDesc; }
            set { userDesc = AssignNotify(ref userDesc, value, "UserDesc"); }
        }   
    }


    public class AppSettings : DataList
    {
        public AppSettings()
        {
            Lifespan = 1.0;
            ListType = typeof(AppSetting);
            TblName = "tblSettings";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("SettingID", "SettingID", true, false);
            dBFieldMappings.AddMapping("SettingName", "SettingName");
            dBFieldMappings.AddMapping("SettingDesc", "SettingDesc");
            dBFieldMappings.AddMapping("SettingType", "SettingType");
            dBFieldMappings.AddMapping("DefaultValue", "DefaultValue");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void GetDouble(string settingName, out Double settingvalue, Double defaultvalue)
        {
            settingvalue = defaultvalue;
            AppSetting rec = (AppSetting)GetByName(settingName);
            if (rec != null)
            {
                Double d = 0.0;
                if (Double.TryParse(rec.DefaultValue, out d))
                    settingvalue = d;
            }
        }

        public void GetString(string settingName, out string settingvalue, string defaultvalue)
        {
            settingvalue = defaultvalue;
            AppSetting rec = (AppSetting)GetByName(settingName);
            if (rec != null)
            {
                settingvalue = rec.DefaultValue;
            }
        }

        public void GetBoolean(string settingName, out Boolean settingvalue, Boolean defaultvalue)
        {
            settingvalue = defaultvalue;
            AppSetting rec = (AppSetting)GetByName(settingName);
            if (rec != null)
            {
                Boolean b = false;
                if (Boolean.TryParse(rec.DefaultValue, out b))
                    settingvalue = b;
            }
        }

        public void GetInteger(string settingName, out int settingvalue, int defaultvalue)
        {
            settingvalue = defaultvalue;
            AppSetting rec = (AppSetting)this.GetByName(settingName);
            if (rec != null)
            {
                int i = 0;
                if (Int32.TryParse(rec.DefaultValue, out i))
                    settingvalue = i;
            }
        }

        public void SetDouble(string settingName, Double settingvalue)
        {
            AppSetting rec = (AppSetting)this.GetByName(settingName);
            if (rec != null)
            {
                rec.DefaultValue = settingvalue.ToString();
            }
            else
            {
                rec = new AppSetting();
                rec.ForceNew = true;
                rec.SettingID = this.GetNextID();
                rec.SettingName = settingName;
                rec.SettingDesc = settingName + " auto created";
                rec.DefaultValue = settingvalue.ToString();
                rec.SettingType = 3;
                this.Add(rec);
            }
        }

        public void SetInt(string settingName, int settingvalue)
        {
            AppSetting rec = (AppSetting)this.GetByName(settingName);
            if (rec != null)
            {
                rec.DefaultValue = settingvalue.ToString();
            }
            else
            {
                rec = new AppSetting();
                rec.ForceNew = true;
                rec.SettingID = this.GetNextID();
                rec.SettingName = settingName;
                rec.SettingDesc = settingName + " auto created";
                rec.DefaultValue = settingvalue.ToString();
                rec.SettingType = 1;
                this.Add(rec);
            }
        }

        public void SetBoolean(string settingName, Boolean settingvalue)
        {
            AppSetting rec = (AppSetting)this.GetByName(settingName);
            if (rec != null)
            {
                rec.DefaultValue = settingvalue.ToString();
            }
            else
            {
                rec = new AppSetting();
                rec.ForceNew = true;
                rec.SettingID = this.GetNextID();
                rec.SettingName = settingName;
                rec.SettingDesc = settingName + " auto created";
                rec.DefaultValue = settingvalue.ToString();
                rec.SettingType = 2;
                this.Add(rec);
            }
        }

        public void SetString(string settingName, string settingvalue)
        {
            AppSetting rec = (AppSetting)this.GetByName(settingName);
            if (rec != null)
            {
                rec.DefaultValue = settingvalue.ToString();
            }
            else
            {
                rec = new AppSetting();
                rec.ForceNew = true;
                rec.SettingID = this.GetNextID();
                rec.SettingName = settingName;
                rec.SettingDesc = settingName + " auto created";
                rec.DefaultValue = settingvalue;
                rec.SettingType = 0;
                this.Add(rec);
            }
        }
    }

    public class AppSetting : DataItem
    {
        #region private fields

        private int settingID;
        private string settingName;
        private string settingDesc;
        private int settingType;
        private string defaultValue;

        #endregion

        public int SettingID
        {
            get { return settingID; }
            set
            {
                settingID = AssignNotify(ref settingID, value, "SettingID");
                ID = settingID;
                PrimaryKey = settingID;
            }
        }

        public string SettingName
        {
            get { return settingName; }
            set
            {
                settingName = AssignNotify(ref settingName, value, "SettingName");
                ItemName = value;
            }
        }

        public string SettingDesc
        {
            get { return settingDesc; }
            set { settingDesc = AssignNotify(ref settingDesc, value, "SettingDesc"); }
        }

        public int SettingType
        {
            get { return settingType; }
            set { settingType = AssignNotify(ref settingType, value, "SettingType"); }
        }

        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = AssignNotify(ref defaultValue, value, "DefaultValue"); }
        }
    }


    public class AppUserSettings : DataList
    {
        public AppUserSettings()
        {
            Lifespan = 1.0;
            ListType = typeof(AppUserSetting);
            TblName = "tblAppUserSettings";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("AppUserSettingID", "AppUserSettingID", true, false);
            dBFieldMappings.AddMapping("SettingValue", "SettingValue");
            dBFieldMappings.AddMapping("AppUserID", "AppUserID");
            dBFieldMappings.AddMapping("AppID", "AppID");
            dBFieldMappings.AddMapping("SettingID", "SettingID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class AppUserSetting : DataItem
    {
        #region private fields

        private int appUserSettingID;
        private string settingValue;
        private int appUserID;
        private int appID;
        private int settingID;

        #endregion 

        public int AppUserSettingID
        {
            get { return appUserSettingID; }
            set
            {
                appUserSettingID = AssignNotify(ref appUserSettingID, value, "AppUserSettingID");
                ID = appUserSettingID;
                PrimaryKey = appUserSettingID;
            }
        }

        public string SettingValue
        {
            get { return settingValue; }
            set { settingValue = AssignNotify(ref settingValue, value, "SettingValue"); }
        }

        public int AppUserID
        {
            get { return appUserID; }
            set { appUserID = AssignNotify(ref appUserID, value, "AppUserID"); }
        }

        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }

        public int SettingID
        {
            get { return settingID; }
            set { settingID = AssignNotify(ref settingID, value, "SettingID"); }
        }
   }

    #endregion

    #region colour settings
    public class ColourPresets : DataList
    {
        public ColourPresets()
        {
            Lifespan = 1.0;
            ListType = typeof(ColourPreset);
            TblName = "tblColourPresets";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ColourPresetID", "ColourPresetID", true, false);
            dBFieldMappings.AddMapping("PresetName", "PresetName");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class ColourPreset : DataItem
    {
        #region private fields

        private int colourPresetID;
        private string presetName;
        private int foreColour;
        private int backColour;

        #endregion

        #region properties

        public int ColourPresetID
        {
            get { return colourPresetID; }
            set
            {
                colourPresetID = AssignNotify(ref colourPresetID, value, "ColourPresetID");
                ID = colourPresetID;
                PrimaryKey = colourPresetID;
            }
        }

        public string PresetName
        {
            get { return presetName; }
            set
            {
                presetName = AssignNotify(ref presetName, value, "PresetName");
                ItemName = presetName;
            }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        #endregion

    }
    
    #endregion

    #region Display Settings
    
    partial class SqlDataAccess
    {
        public Displays GetDisplays(Displays recs)
        {         
            return GetDisplays(recs, false);
        }

        public Displays GetDisplays(Displays recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Displays();
            recs = (Displays)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public DisplayElements GetDisplayElements(DisplayElements recs)
        {
            return GetDisplayElements(recs, false);
        }

        public DisplayElements GetDisplayElements(DisplayElements recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new DisplayElements();
            recs = (DisplayElements)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public DisplayItems GetDisplayItems(DisplayItems recs)
        {
            return GetDisplayItems(recs, false);
        }

        public DisplayItems GetDisplayItems(DisplayItems recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new DisplayItems();
            recs = (DisplayItems)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public DisplayTexts GetDisplayTexts(DisplayTexts recs)
        {
            return GetDisplayTexts(recs, false);
        }

        public DisplayTexts GetDisplayTexts(DisplayTexts recs, bool noCacheRead)
        {
            if (recs==null)
                recs = new DisplayTexts();
            recs = (DisplayTexts)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }
    }

    public class Display : DataItem, INotifyPropertyChanged 
    {
        #region private fields

        private int displayID;
        private string displayName;
        //private string description;
        private int gridColumns;
        private int gridRows;
        private string settings = string.Empty;

        #endregion

        #region properties

        public int DisplayID
        {
            get { return displayID; }
            set 
            {
                displayID = AssignNotify(ref displayID, value, "DisplayID");
                ID = displayID;
                PrimaryKey = displayID;
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(displayName))
                {
                    if (!string.IsNullOrEmpty(ItemName))
                        displayName = ItemName;
                    else
                        displayName = "New Display";
                }
                return displayName;
            }
            set
            {
                displayName = AssignNotify(ref displayName, value, "DisplayName");
                ItemName = displayName;
            }
        }

        public string DisplayNameID
        {
            get { return NameAndID; }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        public int GridColumns
        {
            get { return gridColumns; }
            set { gridColumns = AssignNotify(ref gridColumns, value, "GridColumns"); }
        }

        public int GridRows
        {
            get { return gridRows; }
            set { gridRows = AssignNotify(ref gridRows, value, "GridRows"); }
        }

        public string Settings
        {
            get { return settings; }
            set { settings = AssignNotify(ref settings, value, "Settings"); }
        }

        #endregion
    }

    public class Displays : DataList, INotifyCollectionChanged
    {
        public Displays()
        {
            Lifespan = 24.0;
            ListType = typeof(Display);
            TblName = "tblDisplays";
            DbName = "JEGR_DB";
            MinID = 0;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DisplayID", "DisplayID", true, false);
            dBFieldMappings.AddMapping("DisplayName", "DisplayName");
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("GridColumns", "GridColumns");
            dBFieldMappings.AddMapping("GridRows", "GridRows");
            dBFieldMappings.AddMapping("Settings", "Settings");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }


    public class DisplayElement : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private int elementID;
        private string elementName;
        //private string description;
        private string settings;

        #endregion

        #region properties

        public int ElementID
        {
            get { return elementID; }
            set
            {
                elementID = AssignNotify(ref elementID, value, "ElementID");
                ID = elementID;
                PrimaryKey = elementID;
            }
        }

        public string ElementName
        {
            get { return elementName; }
            set
            {
                elementName = AssignNotify(ref elementName, value, "ElementName");
                ItemName = elementName;
            }
        }

        public string ElementNameID
        {
            get { return NameAndID; }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        public string Settings
        {
            get { return settings; }
            set { settings = AssignNotify(ref settings, value, "Settings"); }
        }

        #endregion
    }

    public class DisplayElements : DataList, INotifyCollectionChanged
    {
        public DisplayElements()
        {
            Lifespan = 24.0;
            ListType = typeof(DisplayElement);
            TblName = "tblDisplayElements";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ElementID", "ElementID", true, false);
            dBFieldMappings.AddMapping("ElementName", "ElementName");
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("Settings", "Settings");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class DisplayItem : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private int displayItemID;
        private int displayID;
        private int elementID;
        private int itemIndex;
        private int topRow;
        private int leftColumn;
        private int rowSpan;
        private int columnSpan;
        private string settings;

        #endregion

        #region properties

        public int DisplayItemID
        {
            get { return displayItemID; }
            set
            {
                displayItemID = AssignNotify(ref displayItemID, value, "DisplayItemID");
                ID = displayItemID;
                PrimaryKey = displayItemID;
            }
        }

        public int DisplayID
        {
            get { return displayID; }
            set
            {
                displayID = AssignNotify(ref displayID, value, "DisplayID");
                RaisePropertyChanged("DisplayNameID");
            }
        }

        public int ElementID
        {
            get { return elementID; }
            set { elementID = AssignNotify(ref elementID, value, "ElementID"); }
        }

        public int ItemIndex
        {
            get { return itemIndex; }
            set { itemIndex = AssignNotify(ref itemIndex, value, "ItemIndex"); }
        }

        public int TopRow
        {
            get { return topRow; }
            set { topRow = AssignNotify(ref topRow, value, "TopRow"); }
        }

        public int LeftColumn
        {
            get { return leftColumn; }
            set { leftColumn = AssignNotify(ref leftColumn, value, "LeftColumn"); }
        }

        public int RowSpan
        {
            get { return rowSpan; }
            set { rowSpan = AssignNotify(ref rowSpan, value, "RowSpan"); }
        }

        public int ColumnSpan
        {
            get { return columnSpan; }
            set { columnSpan = AssignNotify(ref columnSpan, value, "ColumnSpan"); }
        }

        private Display GetDisplay(string nameID)
        {
            Display rec = null;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Displays recs = new Displays();
            recs = (Displays)da.DBDataListSelect(recs);
            if (recs != null)
            {
                if (nameID != string.Empty)
                    rec = (Display)recs.GetByNameID(nameID);
                else
                    rec = (Display)recs.GetById(displayID);
            }
            return rec;
        }

        private string GetDisplayNameID()
        {
            string astring = string.Empty;
            Display rec = GetDisplay(string.Empty);
            if (rec != null)
                astring = rec.NameAndID;
            return astring;
        }

        private void SetDisplayID(string nameID)
        {
            Display rec = GetDisplay(nameID);
            if (rec != null)
            {
                DisplayID = rec.DisplayID;
            }
        }

        public string DisplayNameID
        {
            get { return GetDisplayNameID(); }
            set
            {
                SetDisplayID(value);
                if (HasChanged)
                    RaisePropertyChanged("DisplayNameID");
            }
        }

        private DisplayElement GetElement(string nameID)
        {
            DisplayElement rec = null;
            SqlDataAccess da = SqlDataAccess.Singleton;
            DisplayElements recs = new DisplayElements();
            recs = (DisplayElements)da.DBDataListSelect(recs);
            if (recs != null)
            {
                if (nameID != string.Empty)
                    rec = (DisplayElement)recs.GetByNameID(nameID);
                else
                    rec = (DisplayElement)recs.GetById(elementID);
            }
            return rec;
        }

        private string GetElementNameID()
        {
            string astring = string.Empty;
            DisplayElement rec = GetElement(string.Empty);
            if (rec != null)
                astring = rec.NameAndID;
            return astring;
        }

        private void SetElementID(string nameID)
        {
            DisplayElement rec = GetElement(nameID);
            if (rec != null)
            {
                ElementID = rec.ElementID;
            }
        }

        public string ElementNameID
        {
            get { return GetElementNameID(); }
            set
            {
                SetElementID(value);
                if (HasChanged)
                    RaisePropertyChanged("ElementNameID");
            }
        }

        public string ElementName
        {
            get
            {
                string astring = string.Empty;
                DisplayElement rec = GetElement(string.Empty);
                if (rec != null)
                    astring = rec.ItemName;
                return astring;
            }
        }

        public string Settings
        {
            get { return settings; }
            set { settings = AssignNotify(ref settings, value, "Settings"); }
        }



        #endregion
    }

    public class DisplayItems : DataList, INotifyCollectionChanged
    {
        public DisplayItems()
        {
            Lifespan = 24.0;
            ListType = typeof(DisplayItem);
            TblName = "tblDisplayItems";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DisplayItemID", "DisplayItemID", true, false);
            dBFieldMappings.AddMapping("DisplayID", "DisplayID");
            dBFieldMappings.AddMapping("ElementID", "ElementID");
            dBFieldMappings.AddMapping("ItemIndex", "ItemIndex");
            dBFieldMappings.AddMapping("TopRow", "TopRow");
            dBFieldMappings.AddMapping("LeftColumn", "LeftColumn");
            dBFieldMappings.AddMapping("RowSpan", "RowSpan");
            dBFieldMappings.AddMapping("ColumnSpan", "ColumnSpan");
            dBFieldMappings.AddMapping("Settings", "Settings");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            CreateTableCommand = @"";
        }
    }

    public class DisplayTexts : DataList, INotifyCollectionChanged
    {
        public DisplayTexts()
        {
            Lifespan = 24.0;
            ListType = typeof(DisplayText);
            TblName = "tblDisplayTexts";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DisplayTextID", "DisplayTextID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("Text", "Text");
            dBFieldMappings.AddMapping("ScheduleXml", "ScheduleXml");
            dBFieldMappings.AddMapping("ActiveDisplayIDs", "ActiveDisplayIDs");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            CreateTableCommand = @"CREATE TABLE [dbo].[tblDisplayTexts](
	                                    [DisplayTextID] [int] NOT NULL,
	                                    [Text] [nvarchar](max) NOT NULL,
	                                    [ScheduleXML] [nvarchar](max) NOT NULL,
	                                    [ActiveDisplayIDs] [nvarchar](max) NOT NULL,
                                     CONSTRAINT [PK_tblDisplayTexts] PRIMARY KEY CLUSTERED 
                                        ([DisplayTextID] ASC)
                                            WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, 
                                                ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]";
        }
    }

    public class DisplayText : DataItem
    {
        private int displayTextID;
        private string text;
        private string scheduleXml;
        private Schedule schedule = null;

        private DateTime scheduledStart;
        private DateTime scheduledEnd;
        private bool isActive;

        private Displays activeDisplays = null;
        private string activeDisplayIDs;
        private Displays inActiveDisplays = null;


        #region Properties
        public int DisplayTextID
        {
            get { return displayTextID; }
            set
            {
                displayTextID = AssignNotify(ref displayTextID, value, "DisplayTextID");
                ID = displayTextID;
                PrimaryKey = displayTextID;
            }
        }

        public string Text
        {
            get { return text; }
            set { text = AssignNotify(ref text, value, "Text"); }
        }

        public string ScheduleXml
        {
            get { return scheduleXml; }
            set
            {
                scheduleXml = AssignNotify(ref scheduleXml, value, "ScheduleXml");
                getScheduleFromXml();
            }
        }

        public Schedule Schedule //detail item - not directly stored - only as ScheduleXML field
        {
            get
            {
                if (schedule == null)
                    getScheduleFromXml();
                return schedule;
            }
            set
            {
                schedule = value;
                Notify("Schedule");
                setScheduleToXml();
            }
        }

        public string ActiveDisplayIDs
        {
            get { return activeDisplayIDs; }
            set
            {
                activeDisplayIDs = AssignNotify(ref activeDisplayIDs, value, "ActiveDisplayIDs");
                IDsToDisplays(value);
            }
        }

        public Boolean IsDisplayActive(int id)
        {
            Display d = (Display)activeDisplays.GetById(id);
            return (d != null);
        }

        public Displays ActiveDisplays
        {
            get { return activeDisplays; }
            set
            {
                activeDisplays = value; //assign notify to allow for data save!
                DisplayListToIDs();
                Notify("ActiveDisplays");
            }
        }

        public Displays InActiveDisplays
        {
            get { return inActiveDisplays; }
            set
            {
                inActiveDisplays = value;
                Notify("InActiveDisplays");
            }
        }

        public DateTime ScheduledStart
        {
            get
            {
                scheduledStart = schedule.ScheduledStart;
                return scheduledStart;
            }
            set
            {
                schedule.ScheduledStart = value;
                scheduledStart = AssignNotify(ref scheduledStart, value, "ScheduledStart");
            }
        }

        public string StartTimeStr
        {
            get { return getStartTimeStr(); }
            set
            {
                setStartTime(value);
                Notify("StartTimeStr");
            }
        }

        public Decimal StartTimeHour
        {
            get { return scheduledStart.Hour; }
            set
            {
                ScheduledStart = setHour(scheduledStart, (int)value);
                Notify("StartTimeHour");
            }
        }

        public Decimal StartTimeMin
        {
            get { return scheduledStart.Minute; }
            set
            {
                ScheduledStart = setMin(scheduledStart, (int)value);
                Notify("StartTimeMin");
            }
        }

        public Decimal EndTimeHour
        {
            get { return scheduledEnd.Hour; }
            set
            {
                ScheduledEnd = setHour(scheduledEnd, (int)value);
                Notify("EndTimeHour");
            }
        }

        public Decimal EndTimeMin
        {
            get { return scheduledEnd.Minute; }
            set
            {
                ScheduledEnd = setMin(scheduledEnd, (int)value);
                Notify("EndTimeMin");
            }
        }

        private DateTime setHour(DateTime dateTime, int hour)
        {
            dateTime = dateTime.AddHours(-1 * dateTime.Hour);
            dateTime = dateTime.AddHours(hour);
            return dateTime;
        }

        private DateTime setMin(DateTime dateTime, int min)
        {
            dateTime = dateTime.AddMinutes(-1 * dateTime.Minute);
            dateTime = dateTime.AddMinutes(min);
            return dateTime;
        }

        private string getStartTimeStr()
        {
            string aString = string.Empty;
            if (scheduledStart.Second == 0)
                aString = scheduledStart.ToLongTimeString();
            else
                aString = scheduledStart.ToShortTimeString();
            return aString;
        }

        private bool isNumeric(char c)
        {
            char[] numeric={'0','1','2','3','4','5','6','7','8','9'};
            return numeric.Contains(c);
        }

        private DateTime parseTimeString(string timestring, DateTime datetime)
        {
            timestring = timestring.Trim();    
            string[] timesegments=new string[3];
            DateTime outTime = datetime.Date;
            int i=0;
            int tSeg = 0;
            while ((i < timestring.Length) && (tSeg < 3))
            {
                if (isNumeric(timestring[i]))
                    timesegments[tSeg] += timestring[i];
                else
                    tSeg++;
                i++;
            }
            int h = 0;
            int m = 0;
            int s = 0;
            if (int.TryParse(timesegments[0], out h))
                outTime.AddHours(h);
            if (int.TryParse(timesegments[1], out m))
                outTime.AddMinutes(m);
            if (int.TryParse(timesegments[2], out s))
                outTime.AddSeconds(s);
            return outTime;
        }

        private void setStartTime(string astring)
        {
            ScheduledStart = parseTimeString(astring, ScheduledStart);
        }

        public DateTime ScheduledEnd
        {
            get
            {
                scheduledEnd = schedule.ScheduledEnd;
                return scheduledEnd;
            }
            set
            {
                schedule.ScheduledEnd = value;
                scheduledEnd = AssignNotify(ref scheduledEnd, value, "ScheduledEnd");
            }
        }

        public Boolean IsActive
        {
            get
            {
                isActive = schedule.IsActive;
                return isActive;
            }
            set
            {
                schedule.IsActive = value;
                isActive = AssignNotify(ref isActive, value, "IsActive");
            }
        }

        public Boolean WithoutEnd
        {
            get { return scheduledEnd == DateTime.MaxValue; }
            set
            {
                Boolean test = (scheduledEnd == DateTime.MaxValue);
                if (value != test)
                {
                    if (test)
                        ScheduledEnd = DateTime.Today.AddDays(1);
                    else
                        ScheduledEnd = DateTime.MaxValue;
                    Notify("WithoutEnd");
                    Notify("WithEnd");
                }
            }
        }

        public Boolean WithEnd
        {
            get { return scheduledEnd != DateTime.MaxValue; }
        }

        #endregion

        #region xml stuff
        private string DataItemToXml(DataItem di) // debug this and push into dataitem class definition
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(di.GetType());
            StringWriter sww = new StringWriter();
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww);
            xmlSerializer.Serialize(writer, di);
            var xml = sww.ToString();
            return xml;
        }

        private DataItem XmlToDataItem(string xml, DataItem di) // debug this and push into dataitem class definition
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(di.GetType());
            StringReader stringReader = stringReader = new StringReader(xml);
            System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(stringReader);
            di = (DataItem)xmlSerializer.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return di;
        }

        private void DisplayListToIDs()
        {
            if (activeDisplays != null)
            {
                activeDisplayIDs = activeDisplays.GetIDListString();
            }
        }

        Displays displays = null;

        private void IDsToDisplays(string idstring)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (displays == null)
                displays = new Displays();
            displays = da.GetDisplays(displays);
            activeDisplays.Clear();
            if (displays != null)
            {
                int[] ids = GetIDsFromString(idstring);
                if (ids != null)
                {
                    foreach (int id in ids)
                    {
                        Display d = (Display)displays.GetById(id);
                        if (d != null)
                        {
                            activeDisplays.Add(d);
                        }
                    }
                }
            }
            evaluateInActive();
        }

        private void getScheduleFromXml()
        {
            if (schedule == null)
                schedule = new Schedule();
            if (scheduleXml != string.Empty)
            {
                schedule = (Schedule)XmlToDataItem(scheduleXml, schedule);
            }
        }

        private void setScheduleToXml()
        {
            ScheduleXml = DataItemToXml(schedule);
        }

        
        #endregion

        public DisplayText()
            : base()
        {
            schedule = new Schedule();
            SqlDataAccess da = SqlDataAccess.Singleton;
            inActiveDisplays = new Displays();
            da.DBDataListSelect(inActiveDisplays);
            activeDisplays = new Displays();
        }

        public override void Housekeeping()
        {
            setScheduleToXml();
            Notify("ScheduleXml");
            DisplayListToIDs();
            Notify("ActiveDisplayIDs");
            HasChanged = true;
        }

        public void SetActive(Display display)
        {
            ActiveDisplays.Add(display);
            ActiveDisplays.Sort();
            ActiveDisplays.Reset();
            InActiveDisplays.Remove(display);
            HasChanged = true;
        }

        public void SetInActive(Display display)
        {
            InActiveDisplays.Add(display);
            InActiveDisplays.Sort();
            InActiveDisplays.Reset();
            ActiveDisplays.Remove(display);
            HasChanged = true;
        }

        public void SetAllActive()
        {
            foreach (Display display in InActiveDisplays)
            {
                ActiveDisplays.Add(display);
            }
            ActiveDisplays.Sort();
            ActiveDisplays.Reset();
            InActiveDisplays.Clear();
            InActiveDisplays.Reset();
            HasChanged = true;
        }

        public void SetAllInActive()
        {
            foreach (Display display in ActiveDisplays)
            {
                InActiveDisplays.Add(display);
            }
            InActiveDisplays.Sort();
            InActiveDisplays.Reset();
            ActiveDisplays.Clear();
            ActiveDisplays.Reset();
            HasChanged = true;
        }


        private void evaluateInActive()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (displays == null)
                displays = new Displays();
            displays = da.GetDisplays(displays);
            inActiveDisplays.Clear();
            foreach (Display display in displays)
            {
                Display delDisplay = (Display)ActiveDisplays.GetById(display.ID);
                if (delDisplay == null)
                    inActiveDisplays.Add(display);
            }
            Notify("ActiveDisplays");
            Notify("InActiveDisplays");
            HasChanged = true;
        }

        #region IComparable

        public int CompareTo(DisplayText other)
        {
            int result = Description.CompareTo(other.Description);
            return result;
        }

        #endregion

    }


    #endregion

    public class ConsumptionSetting : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private int settingID;
        private int consumptionType;
        private int unitID;
        private double unitCost;
        private double factor;
        private DateTime start;
        private Boolean isCurrent;
 

        #endregion

        #region properties

        public int SettingID
        {
            get { return settingID; }
            set
            {
                settingID = AssignNotify(ref settingID, value, "SettingID");
                ID = settingID;
                PrimaryKey = settingID;
            }
        }

        public int ConsumptionType
        {
            get { return consumptionType; }
            set { consumptionType = AssignNotify(ref consumptionType, value, "ConsumptionType"); }
        }

        public string ConsumptionTypeStr
        {
            get
            {
                string cts;
                switch (ConsumptionType)
                {
                    case 20: cts = "Gas Consumption";
                        break;
                    case 30: cts = "Fresh Water Consumption";
                        break;
                    case 35: cts = "Waste Water Generation";
                        break;
                    case 40: cts = "Electricity Consumption";
                        break;
                    case 50: cts = "Steam Consumption";
                        break;
                    default: cts = "";
                        break;
                }
                return cts;
            }
            set
            {
                int ct = 0;
                if (value == "Gas Consumption") ct = 20;
                if (value == "Fresh Water Consumption") ct = 30;
                if (value == "Fresh Water Consumption") ct = 35;
                if (value == "Electricity Consumption") ct = 40;
                if (value == "Steam Consumption") ct = 50;
                if (ct > 0)
                {
                    ConsumptionType = ct;
                    Notify("");
                }
            }
        }      

        public int UnitID
        {
            get { return unitID; }
            set { unitID = AssignNotify(ref unitID, value, "UnitID"); }
        }

        public double UnitCost
        {
            get { return unitCost; }
            set { unitCost = AssignNotify(ref unitCost, value, "UnitCost"); }
        }

        public double Factor
        {
            get { return factor; }
            set { factor = AssignNotify(ref factor, value, "Factor"); }
        }

        public DateTime Start
        {
            get { return start; }
            set { start = AssignNotify(ref start, value, "Start"); }
        }

        public Boolean IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = AssignNotify(ref isCurrent, value, "IsCurrent"); }
        }

        #endregion
    }

    public class ConsumptionSettings : DataList, INotifyCollectionChanged
    {
        public ConsumptionSettings()
        {
            Lifespan = 24.0;
            ListType = typeof(ConsumptionSetting);
            TblName = "tblConsumptionSettings";
            DbName = "JEGR_DB";
            MinID = 0;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("SettingID", "SettingID", true, false);
            dBFieldMappings.AddMapping("ConsumptionType", "ConsumptionType");
            dBFieldMappings.AddMapping("UnitID", "UnitID");
            dBFieldMappings.AddMapping("UnitCost", "UnitCost");
            dBFieldMappings.AddMapping("Factor", "Factor");
            dBFieldMappings.AddMapping("Start", "Start");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }


    #region xml Settings

    public class SettingsXMLNode
    {
        public string TagName { get; set; }
        public string Value { get; set; }
        public SettingsXMLNode Parent { get; set; }
        public SettingsXML Children { get; set; }
        public int Level { get; set; }

        public SettingsXMLNode(SettingsXMLNode parent, string tagName, int level)
        {
            Parent = parent;
            TagName = tagName;
            Level = level;
            Value = string.Empty;
            Children = new SettingsXML();
        }

        public string ToValueString()
        {
            string aString = string.Empty;
            if (Parent != null)
                aString = Parent.ToValueString() + "| ";
            aString += Level.ToString() + " '" + TagName + "' [" + Value + "]";
            return aString;
        }

        public void ToStrings(List<string> strings)
        {
            string aString = ToValueString();
            strings.Add(aString);
            foreach (SettingsXMLNode node in Children)
            {
                node.ToStrings(strings);
            }
        }

        public KeyValuePair<string, string> NodeKeyPair()
        {
            return new KeyValuePair<string, string>(TagName, Value);
        }

        public void NodeKeyPairs(List<KeyValuePair<string, string>> keyPairs)
        {
            if (Parent != null)
                Parent.NodeKeyPairs(keyPairs);
            keyPairs.Add(NodeKeyPair());
        }

        public void AddChildrenToList(SettingsXML aList)
        {
            aList.Add(this);
            foreach (SettingsXMLNode node in Children)
            {
                node.AddChildrenToList(aList);
            }
        }

        public List<string[]> GetSettings(string key)
        {
            string[] keyChain = new string[2] { "Settings", key };
            return GetSettings(keyChain);
        }

        public List<string[]> GetSettings(string[] keyChain)
        {
            SettingsXML linearList = new SettingsXML();
            AddChildrenToList(linearList);
            List<string[]> settings = new List<string[]>();
            List<KeyValuePair<string, string>> keyPairs;
            int chainlength = keyChain.Length;
            string[] setting;
            int depth;
            bool match;

            foreach (SettingsXMLNode node in linearList)
            {
                if (node.Level == chainlength - 1)
                {
                    keyPairs = new List<KeyValuePair<string, string>>();
                    node.NodeKeyPairs(keyPairs);
                    if (keyPairs.Count >= chainlength)
                    {
                        depth = 0;
                        match = true;
                        setting = new string[chainlength - 1];
                        while ((depth < chainlength) && match)
                        {
                            match = keyPairs[depth].Key.ToLower() == keyChain[depth].ToLower();
                            if (match)
                            {
                                if (depth > 0)
                                    setting[depth - 1] = keyPairs[depth].Value;
                            }
                            depth++;
                        }
                        if (match)
                            settings.Add(setting);
                    }
                }
            }
            return settings;
        }
    }

    public class SettingsXML : List<SettingsXMLNode>
    { }

    #endregion


}
