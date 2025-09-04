using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Ge_Mac.DataLayer
{

    public partial class SqlDataAccess
    {
        public double DatabaseVersion = 1.2;//default value
        public bool CycleStopBit = false;
        public bool DisplayWeightInPounds = false;
        public int CalendarWeekRule = 0;
        public int WeekStartDay = 1;


        public DateTime ServerTimeLastRead = DateTime.Now;
        public DateTime ServerTime = DateTime.Now;

        public int ServerTimeOffset = 0;

        public double MinimumRateSeconds = 10;

        public void GetServerTimeOffset()
        {
            GetServerTime();
            TimeSpan ts = ServerTimeLastRead.Subtract(ServerTime);
            ServerTimeOffset = (int)Math.Round(ts.TotalHours, 0);
        }

        public void GetServerTime()
        {
            const string commandString = @"SELECT GETDATE()";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection,SqlDataConnection.DBConnection.JensenGroup))
                    if (dr.Read())
                    {
                        ServerTime = dr.GetDateTime(0);
                        ServerTimeLastRead = DateTime.Now;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        #region Diagnostics
        //public static void ShowDebugException(Exception ex)
        //{
        //    const string exFormat =
        //        "RAIL:============================================================================" +
        //        "\r\nError: {0}\r\n" +
        //        "=================================================================================" +
        //        "\r\nTarget: {3}\r\nSource: {1}\r\n\r\nStackTrace:\r\n{2}\r\n\r\nInnerException:\r\n{4}";
        //    string exMessage = string.Format(exFormat, ex.Message, ex.Source, ex.StackTrace, ex.TargetSite, ex.InnerException);
        //    Trace.WriteLine(exMessage);
        //}
        #endregion

        # region constructor

        // private constructor and thread-safe singleton instance property

        private static volatile SqlDataAccess singleton;
        private static object syncRoot = new Object();

        private SqlDataAccess() { }

        public static SqlDataAccess Singleton
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncRoot)
                    {
                        if (singleton == null)
                            singleton = new SqlDataAccess();
                    }
                }
                return singleton;
            }
        }


        # endregion


        #region Caches
        public void InvalidateAllCaches()
        {
            InvalidateArticles();
            InvalidateArticleGroups();
            InvalidateBatches();
            InvalidateBatchesLight();
            InvalidateCompoundBatches();
            InvalidateCustomers();
            InvalidateDef_Units();
            InvalidateMachineGroups();
            InvalidateMachines();
            InvalidateMachineSubIDs();
            InvalidateOperators();
            InvalidatePrinterCodes();
            InvalidatePrinters();
            InvalidatePrintFormats();
            InvalidatePrintRequests();
            InvalidateProcessCodes();
            InvalidateProductionSet();
            InvalidateRefProductionSet();
            InvalidateSequences();
            InvalidateSequenceSteps();
            InvalidateSettings();
            InvalidateShortTrips();
            InvalidateSortCategories();
        }

        #endregion

    }

    public class XMLDataAccess
    {

        #region constructor
        // private constructor and thread-safe singleton instance property

        private static volatile XMLDataAccess singleton;
        private static object syncRoot = new Object();

        private XMLDataAccess()
        {
        }

        public static XMLDataAccess Singleton
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncRoot)
                    {
                        if (singleton == null)
                            singleton = new XMLDataAccess();
                    }
                }
                return singleton;
            }
        }

        #endregion

        public SettingsXMLNode ReadSettings(string xmlSettingsString)
        {
            SettingsXML settingsXML;
            SettingsXMLNode currentNode = null;
            int level = 0;
            try
            {
                using (XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(xmlSettingsString)))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (currentNode == null)
                            {
                                currentNode = new SettingsXMLNode(null, "Settings", 0);
                            }
                            else
                            {
                                settingsXML = currentNode.Children;
                                string tagName = reader.Name;
                                currentNode = new SettingsXMLNode(currentNode, tagName, level);
                                settingsXML.Add(currentNode);
                            }
                            level++;
                            if (reader.Read())
                            {
                                currentNode.Value = reader.Value.Trim();
                            }
                        }
                        else
                        {
                            if (!reader.EOF)
                            {
                                if (currentNode != null)
                                {
                                    if (currentNode.Parent != null)
                                    {
                                        currentNode = currentNode.Parent;
                                        level--;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("malformed xml " + ex.Message); }
            return currentNode;
        }

        public List<string[]> GetSettings(SettingsXMLNode topNode, string[] keyChain)
        {
            if (topNode != null)
                return topNode.GetSettings(keyChain);
            else
                return null;
        }

        public List<string[]> GetSettings(SettingsXMLNode topNode, string key)
        {
            if (topNode != null)
                return topNode.GetSettings(key);
            else
                return null;
        }


        public int ReadIntSetting(string settings, string[] keychain, int defaultSetting)
        {
            int returnSetting = defaultSetting;
            SettingsXMLNode XmlSettings = ReadSettings(settings);
            List<string[]> settingsList = GetSettings(XmlSettings, keychain);
            if (settingsList.Count > 0)
            {
                string settingStr = settingsList[0][0];
                int i = 0;
                if (int.TryParse(settingStr, out i))
                    returnSetting = i;
            }
            return returnSetting;
        }

        public Double ReadDoubleSetting(string settings, string[] keychain, Double defaultSetting)
        {
            Double returnSetting = defaultSetting;
            SettingsXMLNode XmlSettings = ReadSettings(settings);
            List<string[]> settingsList = GetSettings(XmlSettings, keychain);
            if (settingsList.Count > 0)
            {
                string settingStr = settingsList[0][0];
                Double d = 0;
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-GB");
                if (Double.TryParse(settingStr, NumberStyles.AllowDecimalPoint, culture, out d))
                    returnSetting = d;
            }
            return returnSetting;
        }

        public bool ReadBoolSetting(string settingsx, string[] keychain, bool defaultSetting)
        {
            bool returnSetting = defaultSetting;
            SettingsXMLNode XmlSettings = ReadSettings(settingsx);
            List<string[]> settings = GetSettings(XmlSettings, keychain);
            if (settings.Count > 0)
            {
                string settingStr = settings[0][0];
                bool b = false;
                if (bool.TryParse(settingStr, out b))
                    returnSetting = b;
            }
            return returnSetting;
        }

        public string ReadStringSetting(string settingsx, string[] keychain, string defaultSetting)
        {
            string returnSetting = defaultSetting;
            SettingsXMLNode XmlSettings = ReadSettings(settingsx);
            List<string[]> settings = GetSettings(XmlSettings, keychain);
            if (settings.Count > 0)
            {
                returnSetting = settings[0][0];
                while (returnSetting.IndexOf('"') >= 0)
                {
                    int idx = returnSetting.IndexOf('"');
                    returnSetting = returnSetting.Remove(idx, 1);
                }
            }
            return returnSetting;
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


    #region gemac datalist

    public class DataList : List<DataItem>, INotifyCollectionChanged
    {
        #region properties
        private double lifespan = 24.0;
        private string tblName = "tblVisioDiagrams";
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
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }      
        #endregion

        #region find
        public DataItem GetById(int id)
        {
            return this.Find(delegate(DataItem dataItem)
            {
                return dataItem.ID == id;
            });
        }

        #endregion

        #region Events

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(DataItem dataItem)
        {
            base.Add(dataItem);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dataItem));
            }
        }

        new public void Remove(DataItem dataItem)
        {
            base.RemoveAt(this.IndexOf(dataItem));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, dataItem));
            }
        }

        #endregion


        #region Constructor
        public DataList()
        {
            //Nothing to do
        }

        #endregion


        #region Event Methods

        #endregion
    }

    #endregion


    #region Data Class
    public abstract class DataItem : IEditableObject
    {
        /// <summary>Record Has been Edited</summary>
        [XmlIgnoreAttribute()]
        public bool HasChanged { get; set; }

        [XmlIgnoreAttribute()]
        public bool ForceNew { get; set; }
        //public abstract int ID { get; set; }
        public virtual int ID { get; set; }

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public virtual bool IsNew
        {
            get
            {
                return ((ID <= -1) || ForceNew);
            }
        }

        /// <summary>The record exists in the database</summary>
        public virtual bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }

        public bool IsReadOnly { get; set; }

        public bool NoDelete { get; set; }

        public bool CanDelete
        {
            get { return IsExisting && !IsReadOnly && !NoDelete; }
        }

        public bool CanUpdate
        {
            get { return HasChanged && !IsReadOnly; }
        }

        public bool CanUndo
        {
            get { return HasChanged && !IsReadOnly; }
        }
        public bool CanAdd
        {
            get { return !IsReadOnly; }
        }
        /// <summary>The primary key for the record</summary>
        //public abstract int PrimaryKey { get; set; }

        protected ICopyableObject ActiveData = null;
        protected ICopyableObject BackupData = null;
        protected bool IsEditing = false;

        #region IEditableObject
        void IEditableObject.BeginEdit()
        {
            if (!IsEditing)
            {
                if (ActiveData != null)
                {
                    this.BackupData = ActiveData.ShallowCopy();
                    IsEditing = true;
                }
            }
        }

        void IEditableObject.CancelEdit()
        {
            if (IsEditing)
            {
                if (ActiveData != null)
                {
                    this.ActiveData = BackupData;
                    IsEditing = false;
                }
            }
        }

        void IEditableObject.EndEdit()
        {
            this.BackupData = null;
            IsEditing = false;
        }
        #endregion

        #region Notify
        public bool AssignNotify(bool item, bool value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public int AssignNotify(int item, int value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public short AssignNotify(short item, short value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public double AssignNotify(double item, double value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public decimal AssignNotify(decimal item, decimal value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public string AssignNotify(string item, string value, string notify)
        {
            if (item != value)
            {
                NotifyPropertyChanged(notify);
                item = value;
            }
            return value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(String info)
        {
            HasChanged = true;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
            }
        }
        #endregion    
    }
    #endregion

    #region Interfaces
    public interface IDataFiller
    {
        int Fill(SqlDataReader dr);
    }

    public interface IDataFillerSingle
    {
        int FillSingle(SqlDataReader dr);
    }

    public interface IXmlDataFiller
    {
        void FillXml(XmlReader dr);
    }

    public interface ICopyableObject
    {
        ICopyableObject ShallowCopy();
    }
    #endregion
}
