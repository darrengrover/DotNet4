using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Dynamic.DataLayer
{

    #region Jensen units
    public class JensenUnits
    {
        private int gasEnergyUnit = 74;

        public int GasEnergyUnit
        {
            get { return gasEnergyUnit; }
            set { gasEnergyUnit = value; }
        }
        public string GasEnergyUnitName
        {
            get { return GetUnitName(gasEnergyUnit); }
        }
        private int gasVolumeUnit = 19;

        public int GasVolumeUnit
        {
            get { return gasVolumeUnit; }
            set { gasVolumeUnit = value; }
        }
        private int electricalEnergyUnit = 74;

        public int ElectricalEnergyUnit
        {
            get { return electricalEnergyUnit; }
            set { electricalEnergyUnit = value; }
        }

        public string ElectricalEnergyUnitName
        {
            get { return GetUnitName(electricalEnergyUnit); }
        }

        private int waterVolumeUnit = 35;

        public int WaterVolumeUnit
        {
            get { return waterVolumeUnit; }
            set { waterVolumeUnit = value; }
        }

        public string WaterVolumeUnitName
        {
            get { return GetUnitName(waterVolumeUnit); }
        }
        private int weightUnit = 28;

        public int WeightUnit
        {
            get { return weightUnit; }
            set { weightUnit = value; }
        }

        public string WeightUnitName
        {
            get { return GetUnitName(weightUnit); }
        }
        private int temperatureUnit = 10;

        public int TemperatureUnit
        {
            get { return temperatureUnit; }
            set { temperatureUnit = value; }
        }

        public void USUnits()
        {
            gasEnergyUnit = 74;
            electricalEnergyUnit = 74;
            gasVolumeUnit = 27;
            waterVolumeUnit = 50;
            weightUnit = 31;
            temperatureUnit = 11;
        }

        public void MetricUnits()
        {
            gasEnergyUnit = 74;
            electricalEnergyUnit = 74;
            gasVolumeUnit = 19;
            waterVolumeUnit = 35;
            weightUnit = 28;
            temperatureUnit = 10;
        }

        public JensenUnits(Boolean ismetric)
        {
            if (!ismetric)
                USUnits();
        }

        public string GetUnitName(int unitID)
        {
            string unitString = string.Empty;
            if (unitID == 10) unitString = "°C";
            if (unitID == 11) unitString = "°F";
            if (unitID == 12) unitString = "m/min";
            if (unitID == 13) unitString = "m/sec";
            if (unitID == 14) unitString = "V";
            if (unitID == 15) unitString = "ft/min";
            if (unitID == 16) unitString = "A";
            if (unitID == 17) unitString = "bar";
            if (unitID == 18) unitString = "%";
            if (unitID == 19) unitString = "m³";
            if (unitID == 20) unitString = "m";
            if (unitID == 21) unitString = "m²";
            if (unitID == 22) unitString = "ft";
            if (unitID == 23) unitString = "ft²";
            if (unitID == 24) unitString = "min";
            if (unitID == 25) unitString = "sec";
            if (unitID == 26) unitString = "pH";
            if (unitID == 27) unitString = "ft³";
            if (unitID == 28) unitString = "kg";
            if (unitID == 29) unitString = "g";
            if (unitID == 30) unitString = "dB";
            if (unitID == 31) unitString = "lbs";
            if (unitID == 32) unitString = "RGB"; //???
            if (unitID == 33) unitString = "Pieces";
            if (unitID == 34) unitString = "Batches";
            if (unitID == 35) unitString = "L";
            if (unitID == 36) unitString = "cd";
            if (unitID == 37) unitString = "J";
            if (unitID == 38) unitString = "lx";
            if (unitID == 39) unitString = "W";
            if (unitID == 40) unitString = "Hz";
            if (unitID == 41) unitString = "N";
            if (unitID == 42) unitString = "lm";
            if (unitID == 43) unitString = "Bq";
            if (unitID == 44) unitString = "Pa";
            if (unitID == 45) unitString = "PSI";
            if (unitID == 46) unitString = "mS";
            if (unitID == 47) unitString = "ml";
            if (unitID == 48) unitString = "gal/min";
            if (unitID == 49) unitString = "l/min";
            if (unitID == 50) unitString = "gal";
            if (unitID == 51) unitString = "Cycles";
            if (unitID == 52) unitString = "kg/cycle";
            if (unitID == 53) unitString = "lbs/cycle";
            if (unitID == 54) unitString = "Cycle/h";
            if (unitID == 55) unitString = "l/kg";
            if (unitID == 56) unitString = "gal/lb";
            if (unitID == 57) unitString = "RPM";
            if (unitID == 58) unitString = "ml/kg";
            if (unitID == 59) unitString = "fl oz/lb";
            if (unitID == 60) unitString = "Strokes";
            if (unitID == 61) unitString = "mm";
            if (unitID == 62) unitString = "cm";
            if (unitID == 63) unitString = "dm/min";
            if (unitID == 64) unitString = "d°C";//10x deg C!
            if (unitID == 65) unitString = "dbar";
            if (unitID == 66) unitString = "dA";
            if (unitID == 67) unitString = "Pieces/h";
            if (unitID == 68) unitString = "kg/h";
            if (unitID == 69) unitString = "MachineID";//???
            if (unitID == 70) unitString = "Wh";
            if (unitID == 71) unitString = "dm³";
            if (unitID == 72) unitString = "%";
            if (unitID == 73) unitString = "oz";
            if (unitID == 74) unitString = "kWh";
            if (unitID == 75) unitString = "fl oz";
            if (unitID == 76) unitString = "mS"; //mSiemens
            if (unitID == 77) unitString = "°dH"; //water hardness
            if (unitID == 78) unitString = "";
            if (unitID == 79) unitString = "";
            if (unitID == 80) unitString = "";
            if (unitID == 81) unitString = "";
            if (unitID == 82) unitString = "";
            if (unitID == 83) unitString = "";
            if (unitID == 84) unitString = "";
            if (unitID == 85) unitString = "";
            if (unitID == 86) unitString = "";
            if (unitID == 87) unitString = "";
            if (unitID == 88) unitString = "";
            if (unitID == 89) unitString = "";
            return unitString;
        }

        public Double ConvertUnit(Double valuein, int unitin, int unitout)
        {
            Double valueout = valuein;

            if ((unitin == 19) && (unitout == 35))
                valueout = valuein * 1000;
            if ((unitin == 71) && (unitout == 19))
                valueout = valuein / 10;
            if ((unitin == 71) && (unitout == 35))
                valueout = valuein * 100;
            if ((unitin == 28) && (unitout == 31))
                valueout = valuein * 2.20462262;
               


            return valueout;
        }
    }

    #endregion

    #region Images

    public class Images : DataList
    {
        public Images()
        {
            Lifespan = 24.0;
            ListType = typeof(ImageRec);
            TblName = "tblImages";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ImageID", "ImageID", true, false);
            dBFieldMappings.AddMapping("ImageBitmap", "ImageBitmap", false, false, false, true);
            dBFieldMappings.AddMapping("ImageThumbnail", "ImageThumbnail");
            dBFieldMappings.AddMapping("ImageIndex", "ImageIndex");
            dBFieldMappings.AddMapping("ImageTable", "ImageTable");
            dBFieldMappings.AddMapping("ImageTableID", "ImageTableID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public ImageRec GetByIndexTableAndID(int index, string table, int id)
        {
            ImageRec rec = null;
            foreach (DataItem item in this)
            {
                ImageRec ir = (ImageRec)item;
                if ((ir.ImageIndex == index)
                    && (ir.ImageTable == table)
                    && (ir.ImageTableID == id))
                {
                    rec = ir;
                }
            }
            return rec;
        }


    }

    public class ImageRec : DataItem
    {
        private int imageID;
        private byte[] imageBitmap;
        private byte[] imageThumbnail;
        private int imageIndex;
        private string imageTable;
        private int imageTableID;

        private bool gotImage = false;

        public int ImageID
        {
            get { return imageID; }
            set
            {
                imageID = AssignNotify(ref imageID, value, "ImageID");
                ID = imageID;
                PrimaryKey = imageID;
            }
        }

        public byte[] ImageBitmap
        {
            get
            {
                if (!gotImage)
                    GetImageBitmap();
                return imageBitmap;
            }
            set { imageBitmap = AssignNotify(ref imageBitmap, value, "ImageBitmap"); }
        }

        public byte[] ImageThumbnail
        {
            get { return imageThumbnail; }
            set { imageThumbnail = AssignNotify(ref imageThumbnail, value, "ImageThumbnail"); }
        }

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = AssignNotify(ref imageIndex, value, "ImageIndex"); }
        }

        public string ImageTable
        {
            get { return imageTable; }
            set { imageTable = AssignNotify(ref imageTable, value, "ImageTable"); }
        }

        public int ImageTableID
        {
            get { return imageTableID; }
            set { imageTableID = AssignNotify(ref imageTableID, value, "ImageTableID"); }
        }

        public ImageSource ImageDataSource
        {
            get
            {
                return BytesToImageSource(ImageBitmap);
            }
        }

        private void GetImageBitmap()
        {
            Images tempImages = new Images();
            //tempImages.IsValid = false;
            //tempImages.SetIDRestriction("ImageID", imageID);
            //tempImages.AddRestriction("ImageID", imageID);
            //tempImages.ReadLargeField = true;
            //tempImages.IsValid = false;
            SqlDataAccess da=SqlDataAccess.Singleton;
            tempImages = (Images)da.DBDataListSelect(tempImages);
            if (tempImages != null)
            {
                ImageRec tempImageRec = (ImageRec)tempImages.GetById(imageID);
                {
                    if (tempImageRec != null)
                    {
                        imageBitmap = tempImageRec.imageBitmap;
                        gotImage = (imageBitmap!=null);
                    }
                }
            }
        }


    }

    #endregion

    #region Memos
    public class Memos : DataList
    {
        public Memos()
        {
            Lifespan = 1.0;
            ListType = typeof(Memo);
            TblName = "tblMemos";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("MemoID", "MemoID", true, false);
            dBFieldMappings.AddMapping("MemoText", "MemoText");
            dBFieldMappings.AddMapping("MemoIndex", "MemoIndex");
            dBFieldMappings.AddMapping("MemoTable", "MemoTable");
            dBFieldMappings.AddMapping("MemoTableID", "MemoTableID");
            dBFieldMappings.AddMapping("MemoRefID", "MemoRefID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class Memo : DataItem
    {
        private int memoID;
        private string memoText;
        private int memoIndex;
        private string memoTable;
        private int memoTableID;
        private int memoRefID;

        public int MemoID
        {
            get { return memoID; }
            set
            {
                memoID = AssignNotify(ref memoID, value, "MemoID");
                ID = memoID;
                PrimaryKey = memoID;
            }
        }

        public string MemoText
        {
            get { return memoText; }
            set { memoText = AssignNotify(ref memoText, value, "MemoText"); }
        }

        public int MemoIndex
        {
            get { return memoIndex; }
            set { memoIndex = AssignNotify(ref memoIndex, value, "MemoIndex"); }
        }

        public string MemoTable
        {
            get { return memoTable; }
            set { memoTable = AssignNotify(ref memoTable, value, "MemoTable"); }
        }

        public int MemoTableID
        {
            get { return memoTableID; }
            set { memoTableID = AssignNotify(ref memoTableID, value, "MemoTableID"); }
        }

        public int MemoRefID
        {
            get { return memoRefID; }
            set { memoRefID = AssignNotify(ref memoRefID, value, "MemoRefID"); }
        }
    }

    #endregion

    #region MemoReferences

    public class MemoReferences : DataList
    {
        public MemoReferences()
        {
            Lifespan = 1.0;
            ListType = typeof(MemoReferenceRec);
            TblName = "tblMemoReferences";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("MemoRefID", "MemoRefID", true, false);
            dBFieldMappings.AddMapping("MemoReference", "MemoReference");
            dBFieldMappings.AddMapping("MemoSubReference", "MemoSubReference");
            dBFieldMappings.AddMapping("NrMemos", "NrMemos");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class MemoReferenceRec : DataItem
    {
        private int memoRefID;
        private string memoReference;
        private string memoSubReference;
        private int nrMemos;

        public int MemoRefID
        {
            get { return memoRefID; }
            set
            {
                memoRefID = AssignNotify(ref memoRefID, value, "MemoRefID");
                ID = memoRefID;
                PrimaryKey = memoRefID;
            }
        }

        public string MemoReference
        {
            get { return memoReference; }
            set { memoReference = AssignNotify(ref memoReference, value, "MemoReference"); }
        }

        public string MemoSubReference
        {
            get { return memoSubReference; }
            set { memoSubReference = AssignNotify(ref memoSubReference, value, "MemoSubReference"); }
        }

        public int NrMemos
        {
            get { return nrMemos; }
            set { nrMemos = AssignNotify(ref nrMemos, value, "NrMemos"); }
        }

    }

    #endregion

    #region Tags

    partial class SqlDataAccess
    {
        //public Tags GetAllTags()
        //{
        //    return GetAllTags(false);
        //}

        public Tags GetAllTags(Tags recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Tags();
            recs = (Tags)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        //public Tag GetTag(int ReferenceID, string TagReferenceTable)
        //{
        //    Tag ret = null;
        //    Tags ts = GetAllTags();
        //    if (ts != null)
        //    {
        //        ret = (Tag)ts.Find(item => ((((Tag)item).TagReferenceTable == TagReferenceTable) && ((Tag)item).ReferenceID == ReferenceID));
        //    }
        //    return ret;
        //}

    }

    public class Tags : DataList
    {
        public Tags()
        {
            Lifespan = 1.0;
            ListType = typeof(Tag);
            TblName = "tblTags";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("TagID", "TagID", true, false);
            dBFieldMappings.AddMapping("TagData", "TagData");
            dBFieldMappings.AddMapping("ReferenceTable", "TagReferenceTable");
            dBFieldMappings.AddMapping("ReferenceID", "ReferenceID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public Tag GetTag(int referenceid, string tagreferencetable)
        {
            Tag ret = null;
            ret = (Tag)this.Find(item => ((((Tag)item).TagReferenceTable == tagreferencetable) && ((Tag)item).ReferenceID == referenceid));
            return ret;
        }

        public Tag GetTag(string tagdata)
        {
            Tag ret = null;
            ret = (Tag)this.Find(item => (((Tag)item).TagData == tagdata));
            return ret;
        }

    }

    public class Tag : DataItem
    {
        private int tagID;
        private string tagData;
        private string tagReferenceTable;
        private int referenceID;

        public int TagID
        {
            get { return tagID; }
            set
            {
                tagID = AssignNotify(ref tagID, value, "TagID");
                ID = tagID;
                PrimaryKey = tagID;
            }
        }

        public string TagData
        {
            get { return tagData; }
            set
            {
                tagData = AssignNotify(ref tagData, value, "TagData");
                ItemName = tagReferenceTable + tagData;
            }
        }

        public string TagReferenceTable
        {
            get { return tagReferenceTable; }
            set
            {
                tagReferenceTable = AssignNotify(ref tagReferenceTable, value, "TagReferenceTable");
                ItemName = tagReferenceTable + tagData;
            }
        }

        public int ReferenceID
        {
            get { return referenceID; }
            set
            {
                referenceID = AssignNotify(ref referenceID, value, "ReferenceID");
                ID2 = referenceID;
            }
        }

        public override string ToString()
        {
            return tagData;
        }
    }

    #endregion

    #region TagReaderLog

    public class TagReaderLog : DataList
    {
        public TagReaderLog()
        {
            Lifespan = 1.0;
            ListType = typeof(TagReaderLogRec);
            TblName = "tblTagReaderLog";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("LogID", "LogID", true, true);
            dBFieldMappings.AddMapping("LocationID", "LocationID");
            dBFieldMappings.AddMapping("TagData", "TagData");
            dBFieldMappings.AddMapping("ScanTime", "ScanTime");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class TagReaderLogRec : DataItem
    {
        private int logID = 0;
        private int locationID;
        private string tagData;
        private DateTime scanTime;

        public int LogID
        {
            get { return logID; }
            set
            {
                logID = AssignNotify(ref logID, value, "LogID");
                ID = logID;
                PrimaryKey = logID;
            }
        }

        public int LocationID
        {
            get { return locationID; }
            set { locationID = AssignNotify(ref locationID, value, "LocationID"); }
        }

        public string TagData
        {
            get { return tagData; }
            set { tagData = AssignNotify(ref tagData, value, "TagData"); }
        }

        public DateTime ScanTime
        {
            get { return scanTime; }
            set { scanTime = AssignNotify(ref scanTime, value, "ScanTime"); }
        }

    }

    #endregion

    #region TagReaderLocations

    public class TagReaderLocations : DataList
    {
        public TagReaderLocations()
        {
            Lifespan = 1.0;
            ListType = typeof(TagReaderLocation);
            TblName = "tblTagReaderLocations";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("LocationID", "LocationID", true, false);
            dBFieldMappings.AddMapping("LocationIP", "LocationIP");
            dBFieldMappings.AddMapping("TcpPort", "TcpPort");
            dBFieldMappings.AddMapping("Machine_IDJensen", "MachineID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("MaxLogins", "MaxLogins");
            dBFieldMappings.AddMapping("ReaderType", "ReaderType");
            dBFieldMappings.AddMapping("ReaderIndex", "ReaderIndex");
            dBFieldMappings.AddMapping("LoginTerminalID", "LoginTerminalID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class TagReaderLocation : DataItem
    {
        private int locationID;
        private string locationIP;
        private int tcpPort = 10001;
        private int machineID;
        private int subID;
        private int maxLogins;
        private int readerType = 1;
        private int readerIndex = 1;
        private int loginTerminalID = 0;

        public int LocationID
        {
            get { return locationID; }
            set
            {
                locationID = AssignNotify(ref locationID, value, "LocationID");
                ID = locationID;
                PrimaryKey = locationID;
            }
        }

        public string LocationIP
        {
            get { return locationIP; }
            set
            {
                locationIP = AssignNotify(ref locationIP, value, "LocationIP");
                ItemName = value.Trim();
            }
        }

        public int TcpPort
        {
            get { return tcpPort; }
            set { tcpPort = AssignNotify(ref tcpPort, value, "TcpPort"); }
        }

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }

        public int MaxLogins
        {
            get { return maxLogins; }
            set { maxLogins = AssignNotify(ref maxLogins, value, "MaxLogins"); }
        }

        public int ReaderType
        {
            get { return readerType; }
            set { readerType = AssignNotify(ref readerType, value, "ReaderType"); }
        }

        public int ReaderIndex
        {
            get { return readerIndex; }
            set { readerIndex = AssignNotify(ref readerIndex, value, "ReaderIndex"); }
        }

        public int LoginTerminalID
        {
            get { return loginTerminalID; }
            set { loginTerminalID = AssignNotify(ref loginTerminalID, value, "LoginTerminalID"); }
        }
    }

    #endregion

    #region LoginTerminals

    public class LoginTerminals : DataList
    {
        public LoginTerminals()
        {
            Lifespan = 1.0;
            ListType = typeof(LoginTerminal);
            TblName = "tblLoginTerminals";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("LoginTerminalID", "LoginTerminalID", true, false);
            dBFieldMappings.AddMapping("LocationID", "LocationID");
            dBFieldMappings.AddMapping("TerminalName", "TerminalName");
            dBFieldMappings.AddMapping("TerminalIP", "TerminalIP");
            dBFieldMappings.AddMapping("FavouriteID", "FavouriteID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class LoginTerminal : DataItem
    {
        private int loginTerminalID;
        private int locationID;
        private string terminalName;
        private string terminalIP;
        private int favouriteID;

        public int LoginTerminalID
        {
            get { return loginTerminalID; }
            set
            {
                loginTerminalID = AssignNotify(ref loginTerminalID, value, "LoginTerminalID");
                ID = loginTerminalID;
                PrimaryKey = loginTerminalID;
            }
        }

        public int LocationID
        {
            get { return locationID; }
            set { locationID = AssignNotify(ref locationID, value, "LocationID"); }
        }

        public string TerminalName
        {
            get { return terminalName; }
            set { terminalName = AssignNotify(ref terminalName, value, "TerminalName"); }
        }

        public string TerminalIP
        {
            get { return terminalIP; }
            set { terminalIP = AssignNotify(ref terminalIP, value, "TerminalIP"); }
        }

        public int FavouriteID
        {
            get { return favouriteID; }
            set { favouriteID = AssignNotify(ref favouriteID, value, "FavouriteID"); }
        }
    }

    #endregion

    #region LoginTerminalMachines

    public class LoginTerminalMachines : DataList
    {
        public LoginTerminalMachines()
        {
            Lifespan = 1.0;
            ListType = typeof(LoginTerminalMachine);
            TblName = "tblLoginTerminalMachines";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("LoginTermMachineID", "LoginTermMachineID", true, false);
            dBFieldMappings.AddMapping("LoginTerminalID", "LoginTerminalID");
            dBFieldMappings.AddMapping("Machine_IDJensen", "MachineID");
            dBFieldMappings.AddMapping("FavouriteID", "FavouriteID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class LoginTerminalMachine : DataItem
    {
        private int loginTermMachineID;
        private int loginTerminalID;
        private int machineID;
        private int favouriteID;

        public int LoginTermMachineID
        {
            get { return loginTermMachineID; }
            set
            {
                loginTermMachineID = AssignNotify(ref loginTermMachineID, value, "LoginTermMachineID");
                ID = loginTermMachineID;
                PrimaryKey = loginTermMachineID;
            }
        }

        public int LoginTerminalID
        {
            get { return loginTerminalID; }
            set { loginTerminalID = AssignNotify(ref loginTerminalID, value, "LoginTerminalID"); }
        }

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int FavouriteID
        {
            get { return favouriteID; }
            set { favouriteID = AssignNotify(ref favouriteID, value, "FavouriteID"); }
        }
    }

    #endregion

    #region RemoteOperatorLogins

    partial class SqlDataAccess
    {

        public RemoteOperatorLogs GetRemoteOperatorLogs(RemoteOperatorLogs recs, DateTime startTime, DateTime endTime, Boolean allowcache)
        {
            if (recs == null)
                recs = new RemoteOperatorLogs();
            recs.ClearSelectRestrictions();
            recs.AddSelectRestriction(new RestrictionRec("LogTime", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
            recs = (RemoteOperatorLogs)DBDataListSelect(recs, !allowcache, allowcache);
            return recs;
        }

    }
    public class RemoteOperatorLogsGroupedByOperator : DataList
    {
        public RemoteOperatorLogsGroupedByOperator()
        {
            Lifespan = 1.0;
            ListType = typeof(RemoteOperatorLog);
            TblName = "tblRemoteOperatorLog";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RemoteOperatorLogID", "RemoteOperatorLogID", true, true);
            dBFieldMappings.AddMapping("LogTime", "LogTime");
            dBFieldMappings.AddMapping("State", "State");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubregType", "SubregType");
            dBFieldMappings.AddMapping("SubregTypeID", "SubregTypeID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("JgLogDataRemoteID", "JgLogDataRemoteID");
            dBFieldMappings.AddMapping("UpdateTime", "UpdateTime");

            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            DefaultSqlSelectCommand = @"SELECT 
                                        t.[RemoteOperatorLogID], 
                                        t.[LogTime], 
                                        t.[MachineID], 
                                        t.[OperatorID], 
                                        t.[SubID], 
                                        t.[State], 
                                        t.[jgLogDataRemoteID], 
                                        t.[UpdateTime], 
                                        t.[SubregType], 
                                        t.[SubregTypeID]
                                        FROM (SELECT        
                                                MAX([LogTime]) AS MAX_TIME, 
                                                [OperatorID]
                                                FROM [dbo].[tblRemoteOperatorLog]
                                                GROUP BY OperatorID) AS m 
                                        INNER JOIN [dbo].[tblRemoteOperatorLog] AS t 
                                        ON t.[OperatorID] = m.[OperatorID] AND t.[LogTime] = m.MAX_TIME";
        }

        public RemoteOperatorLog GetByOperatorId(int operatorid)
        {
            RemoteOperatorLog remoteoperatorlog = null;
            foreach (RemoteOperatorLog log in this)
            {
                if (log.OperatorID == operatorid)
                {
                    remoteoperatorlog = log;
                    break;
                }
            }
            return remoteoperatorlog;
        }
    }
    public class RemoteOperatorLogs : DataList
    {
        public RemoteOperatorLogs()
        {
            Lifespan = 1.0;
            ListType = typeof(RemoteOperatorLog);
            TblName = "tblRemoteOperatorLog";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RemoteOperatorLogID", "RemoteOperatorLogID", true, true);
            dBFieldMappings.AddMapping("LogTime", "LogTime");
            dBFieldMappings.AddMapping("State", "State");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("SubregType", "SubregType");
            dBFieldMappings.AddMapping("SubregTypeID", "SubregTypeID");
            dBFieldMappings.AddMapping("SubID", "SubID");
            dBFieldMappings.AddMapping("JgLogDataRemoteID", "JgLogDataRemoteID");
            dBFieldMappings.AddMapping("UpdateTime", "UpdateTime");

            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
            CreateTableCommand = @"CREATE TABLE [dbo].[tblRemoteOperatorLog](
	                                        [RemoteOperatorLogID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	                                        [LogTime] [datetime] NOT NULL,
	                                        [MachineID] [int] NOT NULL,
	                                        [OperatorID] [int] NOT NULL,
	                                        [SubID] [int] NOT NULL,
	                                        [SubregType] [int] NOT NULL,
	                                        [SubregTypeID] [int] NOT NULL,
	                                        [State] [int] NOT NULL,
	                                        [jgLogDataRemoteID] [int] NULL,
	                                        [UpdateTime] [datetime] NULL,
                                            CONSTRAINT [PK_tblRemoteOperatorLog] PRIMARY KEY CLUSTERED 
                                        (
	                                        [RemoteOperatorLogID] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON
                                                    , ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]";
        }



        public void SortByOperatorTimeRemote()
        {
            SortOperatorLogs sol = new SortOperatorLogs();
            this.Sort(sol);
        }
    }

    public class SortOperatorLogs : IComparer<DataItem>
    {
        public int Compare(DataItem x, DataItem y)
        {
            RemoteOperatorLog a = (RemoteOperatorLog)x;
            RemoteOperatorLog b = (RemoteOperatorLog)y;
            int result = a.OperatorID.CompareTo(b.OperatorID);
            //if (result == 0)
            //{
            //    result = a.LogTime.CompareTo(b.LogTime);
            //}
            if (result == 0)
            {
                result = a.RemoteOperatorLogID.CompareTo(b.RemoteOperatorLogID);
            }
            return result;
        }
    }

    public class RemoteOperatorLog : DataItem, IComparable<DataItem>
    {
        private Int64 remoteOperatorLogID;
        private DateTime logTime;
        private int state;
        private int operatorID;
        private int machineID;
        private int subID;
        private int subregType = 0;
        private int subregTypeID = 0;
        private int jgLogDataRemoteID = -1;
        private DateTime updateTime = DateTime.MaxValue;

        public Int64 RemoteOperatorLogID
        {
            get { return remoteOperatorLogID; }
            set
            {
                remoteOperatorLogID = AssignNotify(ref remoteOperatorLogID, value, "RemoteOperatorLogID");
                ID = remoteOperatorLogID;
                PrimaryKey = remoteOperatorLogID;
            }
        }

        public int State
        {
            get { return state; }
            set { state = AssignNotify(ref state, value, "State"); }
        }

        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = AssignNotify(ref operatorID, value, "OperatorID"); }
        }

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }

        public int SubregType
        {
            get { return subregType; }
            set { subregType = AssignNotify(ref subregType, value, "SubregType"); }
        }

        public int SubregTypeID
        {
            get { return subregTypeID; }
            set { subregTypeID = AssignNotify(ref subregTypeID, value, "SubregTypeID"); }
        }

        public int JgLogDataRemoteID
        {
            get { return jgLogDataRemoteID; }
            set { jgLogDataRemoteID = AssignNotify(ref jgLogDataRemoteID, value, "jgLogDataRemoteID"); }
        }

        public DateTime LogTime
        {
            get { return logTime; }
            set { logTime = AssignNotify(ref logTime, value, "LogTime"); }
        }

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = AssignNotify(ref updateTime, value, "UpdateTime"); }
        }

        public override int CompareTo(DataItem other)
        {
            RemoteOperatorLog oplog = (RemoteOperatorLog)other;
            return RemoteOperatorLogID.CompareTo(oplog.RemoteOperatorLogID);
        }
    }

    #endregion


    #region OperatorStates

    partial class SqlDataAccess
    {

        public OperatorStates GetOperatorStates(OperatorStates operatorstates, RemoteOperatorLogs remoteoperatorlogs)
        {
            DateTime startTime = DateTime.Today;
            DateTime endTime = DateTime.Now;
            return GetOperatorStates(operatorstates, remoteoperatorlogs, startTime, endTime);
        }

        public OperatorStates GetOperatorStates(OperatorStates operatorstates, RemoteOperatorLogs remoteoperatorlogs, DateTime startTime, DateTime endTime)
        {
            if (operatorstates == null)
                operatorstates = new OperatorStates();
            else
                operatorstates.Clear();
            try
            {
                if (remoteoperatorlogs == null)
                    remoteoperatorlogs = new RemoteOperatorLogs();
                remoteoperatorlogs.ClearSelectRestrictions();
                remoteoperatorlogs.AddSelectRestriction(new RestrictionRec("LogTime", startTime, endTime, ComparisonType.MoreThanEqualALessThanB));
                remoteoperatorlogs = (RemoteOperatorLogs)DBDataListSelect(remoteoperatorlogs, true);
                if (remoteoperatorlogs != null)
                {
                    Operators ops = GetOperators(null, false);
                    remoteoperatorlogs.Sort();
                    foreach (RemoteOperatorLog log in remoteoperatorlogs)
                    {
                        OperatorState os = (OperatorState)operatorstates.GetById(log.OperatorID);
                        if (os == null)
                        {
                            os = new OperatorState()
                            {
                                OperatorID = log.OperatorID
                            };
                            Operator op = (Operator)ops.GetById(log.OperatorID);
                            if (op != null)
                                os.ItemName = op.ShortDescription;
                            operatorstates.Add(os);
                        }
                        os.MachineID = log.MachineID;
                        os.SubID = log.SubID;
                        os.LogTime = log.LogTime;
                        os.State = log.State;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
            return operatorstates;
        }

    }

    public class OperatorStates : DataList
    {
        public OperatorStates()
        {
            ListType = typeof(OperatorState);
        }
    }

    public class OperatorState : DataItem , IComparable<DataItem>
    {
        private int operatorID;
        private int machineID = -1;
        private int subID = -1;
        private DateTime logTime = DateTime.MinValue;
        private int state = 0;

        public int OperatorID
        {
            get { return operatorID; }
            set
            {
                operatorID = AssignNotify(ref operatorID, value, "OperatorID");
                ID = operatorID;
                PrimaryKey = operatorID;
            }
        }
        public
            int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int SubID
        {
            get { return subID; }
            set { subID = AssignNotify(ref subID, value, "SubID"); }
        }

        public DateTime LogTime
        {
            get { return logTime; }
            set { logTime = AssignNotify(ref logTime, value, "LogTime"); }
        }

        public int State
        {
            get { return state; }
            set { state = AssignNotify(ref state, value, "State"); }
        }

        public Boolean IsLoggedIn
        {
            get { return state == 1; }
        }

        public override int CompareTo(DataItem other)
        {
            OperatorState rec = (OperatorState)other;
            return ItemName.CompareTo(rec.ItemName);
        }

    }

    #endregion

    #region ResourceImages

    partial class SqlDataAccess
    {

        public ResourceImages GetAllResourceImages(ResourceImages recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new ResourceImages();
            recs = (ResourceImages)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

        public ResourceImages GetAllResourceImages(ResourceImages recs)
        {
            return GetAllResourceImages(recs, false);
        }

    }

    public class ResourceImages : DataList
    {
        public ResourceImages()
        {
            Lifespan = 23.0;
            ListType = typeof(ResourceImage);
            TblName = "tblResourceImages";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ResourceImageID", "ResourceImageID", true, false);
            dBFieldMappings.AddMapping("ImageName", "ImageName");
            dBFieldMappings.AddMapping("ImageData", "ImageData");
            dBFieldMappings.AddMapping("ImageFormat", "ImageFormat");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void AddImage()
        {
            ResourceImage rec = new ResourceImage();
            rec.ResourceImageID = GetNextID();
            rec.ForceNew = true;
            rec.ImageName = "New Image " + rec.ResourceImageID;
            this.Add(rec);
        }
    }

    public class ResourceImage : DataItem
    {
        private int resourceImageID;
        private string imageName;        
        private byte[] imageData;
        private string imageFormat;

        public int ResourceImageID
        {
            get { return resourceImageID; }
            set
            {
                resourceImageID = AssignNotify(ref resourceImageID, value, "ResourceImageID");
                ID = resourceImageID;
                PrimaryKey = resourceImageID;
            }
        }

        public string ImageName
        {
            get { return imageName; }
            set
            {
                imageName = AssignNotify(ref imageName, value, "ImageName");
                ItemName = imageName;
            }
        }

        public byte[] ImageData
        {
            get { return imageData; }
            set { imageData = AssignNotify(ref imageData, value, "ImageData"); }
        }

        public ImageSource ImageDataSource
        {
            get
            {
                return BytesToImageSource(ImageData);
            }
        }

        public string ImageFormat
        {
            get { return imageFormat; }
            set { imageFormat = AssignNotify(ref imageFormat, value, "ImageFormat"); }
        }
    }


    #endregion

    #region Feedback
    public class FeedbackRecs : DataList
    {
        public FeedbackRecs()
        {
            Lifespan = 1.0;
            ListType = typeof(FeedbackRec);
            TblName = "tblFeedback";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Recnum", "Recnum", true, true);
            dBFieldMappings.AddMapping("LogTime", "LogTime");
            dBFieldMappings.AddMapping("AppName", "AppName");
            dBFieldMappings.AddMapping("Feedback", "Feedback");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class FeedbackRec : DataItem
    {
        private int recnum;
        private DateTime logTime;
        private string appName;
        private string feedback;

        public int Recnum
        {
            get { return recnum; }
            set
            {
                recnum = AssignNotify(ref recnum, value, "Recnum");
                ID = recnum;
                PrimaryKey = recnum;
            }
        }

        public DateTime LogTime
        {
            get { return logTime; }
            set { logTime = AssignNotify(ref logTime, value, "LogTime"); }
        }

        public string AppName
        {
            get { return appName; }
            set { appName = AssignNotify(ref appName, value, "AppName"); }
        }

        public string Feedback
        {
            get { return feedback; }
            set { feedback = AssignNotify(ref feedback, value, "Feedback"); }
        }
    }

    #endregion




}
