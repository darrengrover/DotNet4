using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using MVVMHelpers;

namespace Dynamic.DataLayer
{
    public partial class SqlDataAccess
    {
        #region private fields

        //connections
        private DBConnections dBConnections;
        public DBConnections DBConnections
        {
            get { return dBConnections; }
            set { dBConnections = value; }
        }
        private SqlConnection sqlConnection;
        //private SqlConnection sqlConnection
        //{
        //    get { return _sqlConnection; }
        //    set { _sqlConnection = value; }
        //}

        //caches
        private CacheItems cacheItems;

        public CacheItems CacheItems
        {
            get { return cacheItems; }
            set { cacheItems = value; }
        }

        //table updates
        private LastTableUpdates lastTableUpdates;

        public LastTableUpdates LastTableUpdates
        {
            get { return lastTableUpdates; }
            set { lastTableUpdates = value; }
        }

        #endregion

        #region properties
        private string siteName = "default";

        public string SiteName
        {
            get { return siteName; }
            set
            {
                if (siteName != value)
                {
                    cacheItems.InvalidateCaches();
                    lastTableUpdates.LastRead = DateTime.Now.AddYears(-1);
                    siteName = value;
                    sqlConnection = null;
                }
            }
        }

        private string dBName = "JEGR_DB";

        public string DBName
        {
            get { return dBName; }
            set
            {
                if (dBName != value)
                {
                    lastTableUpdates.LastRead = DateTime.Now.AddYears(-1);
                    dBName = value;
                    sqlConnection = null;
                }
            }
        }

        private bool useServerTime = true;
        public bool UseServerTime
        {
            get { return useServerTime; }
            set { useServerTime = value; }
        }

        private DateTime serverTimeLastRead = DateTime.MinValue;

        public DateTime ServerTimeLastRead
        {
            get { return serverTimeLastRead; }
            set { serverTimeLastRead = value; }
        }
        private DateTime serverTime = DateTime.MinValue;

        public DateTime ServerTime
        {
            get
            {
                if (useServerTime)
                    return serverTime;
                else
                    return DateTime.Now;
            }
            set { serverTime = value; }
        }
        private int serverTimeOffset = 0;

        public int ServerTimeOffset
        {
            get { return serverTimeOffset; }
            set { serverTimeOffset = value; }
        }

        public SqlConnection SqlConnection
        {
            get
            {
                return GetConnection();
            }
        }

        public void DBConnectionsToXML()
        {
            string filename = "DBConnections.xml";
            using (FileStream fs = File.Create(filename))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(dBConnections.GetType());
                x.Serialize(fs, dBConnections);
            }
        }

        public void XMLToDBConnections()
        {
            dBConnections = new DBConnections();
            string filename = "DBConnections.xml";
            try
            {
                if (File.Exists(filename))
                {
                    using (FileStream fs = File.OpenRead(filename))
                    {
                        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(DBConnections));
                        dBConnections = (DBConnections)x.Deserialize(fs);
                    }
                }
                else
                {
                    Debug.WriteLine("XMLToDBConnections " + filename + " not found!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
            }
        }
        private double _databaseVersion = 1.2;
        public double DatabaseVersion
        {
            get { return _databaseVersion; }
            set
            {
                if (value != _databaseVersion)
                {
                    _databaseVersion = value;
                }
            }
        }

        #endregion

        #region constructor
        // private constructor and thread-safe singleton instance property

        private static volatile SqlDataAccess singleton = null;
        private static object syncRoot = new Object();

        private SqlDataAccess() 
        {
            XMLToDBConnections();
            cacheItems=new CacheItems();
            foreach (DataItem di in dBConnections)
            {
                di.HasChanged = false;
            }
            lastTableUpdates = new LastTableUpdates();
        }

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

        #endregion

        #region methods

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
                    using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection, GetConnection()))
                    {
                        if ((dr != null) && (dr.Read()))
                        {
                            ServerTime = dr.GetDateTime(0);
                            ServerTimeLastRead = DateTime.Now;
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        private string GetConnectionString()
        {
            return GetConnectionString(siteName, dBName);
        }

        public string GetConnectionString(string sitename, string dbname)
        {
            string connectionstring = string.Empty;
            if (DBConnections != null)
            {
                //DBConnection dbc = dBConnections.GetBySiteDB(sitename, dbname);
                DBConnection dbc = dBConnections.GetBySite(sitename);
                if (dbc != null)
                {
                    connectionstring = dbc.ConnectionString;
                }
            }
            return connectionstring;
        }

        private SqlConnection GetConnection()
        {
            return GetConnection(GetConnectionString());
        }

        private SqlConnection GetConnection(string aConnectionString)
        {
            //if ((sqlConnection == null) || (sqlConnection.State != ConnectionState.Open))
            //{
            //    if (sqlConnection == null)
            //        Debug.WriteLine("GetConnection (null connection) " + aConnectionString);
            //    sqlConnection = new SqlConnection(aConnectionString);
            //}
            //return sqlConnection; 
            return new SqlConnection(aConnectionString); 
        }

        public void DeleteTableRecord(string tblName, string fieldName, Int64 idValue)
        {
            string commandString = "DELETE FROM " + tblName + " WHERE [" + fieldName + "] = @" + fieldName;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@" + fieldName, idValue);
                    command.ExecuteNonQuery(SqlConnection);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in DeleteTableRecord() TableName = " + tblName + " Fieldname = " 
                    + fieldName + " idValue " + idValue + ". " + ex.Message);
                throw;
            }
        }

        public void TruncateTable(string tblName)
        {
            string commandString = "TRUNCATE TABLE " + tblName;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.ExecuteNonQuery(SqlConnection);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in TruncateTable() TableName = " + tblName + ". " + ex.Message);
                throw;
            }
        }

        public void ExecuteNonQuery(string dbName, string commandString)
        {
            try
            {
                DBName = dbName;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.ExecuteNonQuery(SqlConnection);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in ExecuteNonQuery() dbName = " + dbName + " Command = " + commandString + ". " + ex.Message);
                throw;
            }
        }

        public Object ExecuteScalar(string dbName, string commandString)
        {
            Object scalarResult=null;
            try
            {
                DBName = dbName;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    scalarResult = command.ExecuteScalar(SqlConnection);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in ExecuteScalar() dbName = " + dbName + " Command = " + commandString + ". " + ex.Message);
            }
            return scalarResult;
        }

        public int NextRecord(string aTable, string aField)
        {
            string commandString = "DECLARE @return_value int " +
                "EXEC @return_value = [" + dBName + "].[dbo].[FirstID] " +
                        "@TableName = N'" + aTable +
                        "', @idName = N'" + aField +
                        "' SELECT @return_value";
            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    object spResult = command.ExecuteScalar(sqlConnection);
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
        public DataList DBDataListSelect(DataList dataList)
        {
            return DBDataListSelect(dataList, !dataList.AllowStoreToCache, dataList.AllowStoreToCache);
        }

        public DataList DBDataListSelect(DataList dataList, bool noCacheRead)
        {
            return DBDataListSelect(dataList, noCacheRead, dataList.AllowStoreToCache);
        }

        public DataList DBDataListSelect(DataList dataList, bool noCacheRead, bool allowStoreToCache)
        {
            try
            {
                dataList.SelectException = string.Empty;
                if (!dataList.IsValid || noCacheRead)
                {
                    bool getFromDB = true;
                    if (!noCacheRead && allowStoreToCache)
                        getFromDB = !(cacheItems.GetDataList(ref dataList));
                    if (getFromDB)
                    {
                        dataList.DBSelect();
                        dataList.UpdatedFromDB = true;
                        if (allowStoreToCache)
                            cacheItems.AddDataList(dataList);
                    }
                }
                else
                {
                    dataList.UpdatedFromDB = false;
                    if (dataList.TblName != "fnLastTblUpdate()")
                        cacheItems.RemoveInvalidCaches();
                }

            }
            catch (Exception ex)
            {
                dataList.SelectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (dataList.RaiseException)
                { throw ex; }
            }
            return dataList;
        }

        public DateTime TableLastUpdated(string tblName)
        {
            DateTime updateTime = DateTime.Now.AddYears(-1);
            lastTableUpdates = (LastTableUpdates)DBDataListSelect(lastTableUpdates, false, true);
            if (lastTableUpdates != null)
            {
                LastTableUpdate ltu = (LastTableUpdate)lastTableUpdates.GetByName(tblName);
                if (ltu != null)
                {
                    updateTime = ltu.LastUpdated;
                }
            }
            GetServerTime();

            return updateTime;
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
                if (Double.TryParse(settingStr, out d))
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

    //public class SettingsXMLNode
    //{
    //    public string TagName { get; set; }
    //    public string Value { get; set; }
    //    public SettingsXMLNode Parent { get; set; }
    //    public SettingsXML Children { get; set; }
    //    public int Level { get; set; }

    //    public SettingsXMLNode(SettingsXMLNode parent, string tagName, int level)
    //    {
    //        Parent = parent;
    //        TagName = tagName;
    //        Level = level;
    //        Value = string.Empty;
    //        Children = new SettingsXML();
    //    }

    //    public string ToValueString()
    //    {
    //        string aString = string.Empty;
    //        if (Parent != null)
    //            aString = Parent.ToValueString() + "| ";
    //        aString += Level.ToString() + " '" + TagName + "' [" + Value + "]";
    //        return aString;
    //    }

    //    public void ToStrings(List<string> strings)
    //    {
    //        string aString = ToValueString();
    //        strings.Add(aString);
    //        foreach (SettingsXMLNode node in Children)
    //        {
    //            node.ToStrings(strings);
    //        }
    //    }

    //    public KeyValuePair<string, string> NodeKeyPair()
    //    {
    //        return new KeyValuePair<string, string>(TagName, Value);
    //    }

    //    public void NodeKeyPairs(List<KeyValuePair<string, string>> keyPairs)
    //    {
    //        if (Parent != null)
    //            Parent.NodeKeyPairs(keyPairs);
    //        keyPairs.Add(NodeKeyPair());
    //    }

    //    public void AddChildrenToList(SettingsXML aList)
    //    {
    //        aList.Add(this);
    //        foreach (SettingsXMLNode node in Children)
    //        {
    //            node.AddChildrenToList(aList);
    //        }
    //    }

    //    public List<string[]> GetSettings(string key)
    //    {
    //        string[] keyChain = new string[2] { "Settings", key };
    //        return GetSettings(keyChain);
    //    }

    //    public List<string[]> GetSettings(string[] keyChain)
    //    {
    //        SettingsXML linearList = new SettingsXML();
    //        AddChildrenToList(linearList);
    //        List<string[]> settings = new List<string[]>();
    //        List<KeyValuePair<string, string>> keyPairs;
    //        int chainlength = keyChain.Length;
    //        string[] setting;
    //        int depth;
    //        bool match;

    //        foreach (SettingsXMLNode node in linearList)
    //        {
    //            if (node.Level == chainlength - 1)
    //            {
    //                keyPairs = new List<KeyValuePair<string, string>>();
    //                node.NodeKeyPairs(keyPairs);
    //                if (keyPairs.Count >= chainlength)
    //                {
    //                    depth = 0;
    //                    match = true;
    //                    setting = new string[chainlength - 1];
    //                    while ((depth < chainlength) && match)
    //                    {
    //                        match = keyPairs[depth].Key.ToLower() == keyChain[depth].ToLower();
    //                        if (match)
    //                        {
    //                            if (depth > 0)
    //                                setting[depth - 1] = keyPairs[depth].Value;
    //                        }
    //                        depth++;
    //                    }
    //                    if (match)
    //                        settings.Add(setting);
    //                }
    //            }
    //        }
    //        return settings;
    //    }
    //}

    //public class SettingsXML : List<SettingsXMLNode>
    //{ }

    #endregion

    public static class SqlClientExtension
    {
        #region properties
        public static int Timeout = 30;


        #endregion

        #region DataTable
        /// <summary>Read data into a datatable</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        /// <returns>The filled datatable</returns>
        public static DataTable ReadTable(this SqlCommand command, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }

            command.CommandTimeout = Timeout;

            DataTable dataTable;

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                dataTable = new DataTable();
                dbAdapter.Fill(dataTable);
            }

            return dataTable;
        }

        /// <summary>Read data into a datatable</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        /// <returns>The filled datatable</returns>
        public static int Fill(this SqlDataAdapter adapter, DataSet dataSet, string tableName, SqlConnection dBConnection)
        {
            SqlCommand command = adapter.SelectCommand;
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }
            command.CommandTimeout = Timeout;

            int records = 0;

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                records = dbAdapter.Fill(dataSet, tableName);
            }

            return records;
        }

        #endregion

        #region Reader
        /// <summary>
        /// Opens the specified connection and executes the reader.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="dBConnection">The Ge-Mac Connection Type</param>
        /// <returns>The Open Reader, ready to Read</returns>
        public static SqlDataReader ExecuteReader(this SqlCommand command, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }
            command.CommandTimeout = Timeout;

            SqlDataReader reader;

            command.Connection.Open();

            using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
            {
                reader = command.ExecuteReader();
            }

            return reader;
        }

        /// <summary>
        /// Opens the specified connection and executes the reader.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="behaviour">The Command Behavior</param>
        /// <param name="dBConnection">The Ge-Mac Connection Type</param>
        /// <returns>The Open Reader, ready to Read</returns>
        public static SqlDataReader ExecuteReader(this SqlCommand command, CommandBehavior behaviour,
            SqlConnection dBConnection, int timeoutSeconds)
        {
            SqlDataReader reader = null;
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)            
                command.Connection = dBConnection;
            
            if (timeoutSeconds <= 0)
                command.CommandTimeout = Timeout;
            else
                command.CommandTimeout = timeoutSeconds;
            if (command.Connection != null)
            {
                command.Connection.Open();

                using (SqlDataAdapter dbAdapter = new SqlDataAdapter(command))
                {
                    reader = command.ExecuteReader(behaviour);
                }
            }
            return reader;
        }

        public static SqlDataReader ExecuteReader(this SqlCommand command, CommandBehavior behaviour, SqlConnection dBConnection)
        {

            SqlDataReader reader = ExecuteReader(command, behaviour, dBConnection, -1);

            return reader;
        }
        #endregion

        #region Collection
        /// <summary>Read data into a class collection</summary>
        /// <param name="dc">The class to fill</param>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static int DataFill(this SqlCommand command, IDataFiller dc, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }
            command.CommandTimeout = Timeout;

            int recordsRead = 0;

            // Open the connection if necessary
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    recordsRead = dc.Fill(dr);
                }
            }
            else
            {
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    recordsRead = dc.Fill(dr);
                }
            }

            return recordsRead;
        }


        #endregion

        #region Single Class
        /// <summary>Read data into a single class</summary>
        /// <param name="dc">The class to fill</param>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static bool DataFillSingle(this SqlCommand command, IDataFillerSingle dc, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }
            command.CommandTimeout = Timeout;

            int recordsRead;

            // Open the connection if necessary
            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    recordsRead = dc.FillSingle(dr);
                }
            }
            else
            {
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    recordsRead = dc.FillSingle(dr);
                }
            }

            return recordsRead > 0;
        }
        #endregion

        #region No Resultset
        /// <summary>Execute a sql statement that returns no data</summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">The connection to use</param>
        public static int ExecuteNonQuery(this SqlCommand command, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }

            command.CommandTimeout = Timeout;

            bool close = false;

            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                close = true;
            }

            int recordsAffected = 0;

            try
            {
                recordsAffected = command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (close)
                {
                    command.Connection.Close();
                }
            }

            return recordsAffected;
        }

        /// <summary>
        /// Execute a sql statement that returns a single value
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="DBConnection">the connection to use</param>
        /// <returns></returns>
        public static object ExecuteScalar(this SqlCommand command, SqlConnection dBConnection)
        {
            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
            {
                command.Connection = dBConnection;
            }
            command.CommandTimeout = Timeout;

            bool close = false;

            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
                close = true;
            }

            object scalarValue = null;

            try
            {
                scalarValue = command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (close)
                {
                    command.Connection.Close();
                }
            }

            return scalarValue;
        }
        #endregion
    }


    public class DataList : List<DataItem>, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Member

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public virtual void Reset()
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public virtual new void Add(DataItem item)
        {
            item.DBFieldMappings = dBFieldMappings;
            base.Add(item);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public virtual new void Remove(DataItem item)
        {
            base.RemoveAt(this.IndexOf(item));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
        }
  
        #endregion

        #region properties
        protected Type ListType = typeof(DataItem);
        #region lifespan properties
        private double lifespan = 1.0;
        public double Lifespan
        {
            get { return lifespan; }
            set { lifespan = value; }
        }
        protected double minLifespan = 1.0 / 3600;
        public double MinLifespan
        {
            get { return minLifespan; }
            set { minLifespan = value; }
        }
        protected DateTime lastRead;
        public DateTime LastRead
        {
            get { return lastRead; }
            set { lastRead = value; }
        }
        private bool updatedFromDB = false;
        public bool UpdatedFromDB
        {
            get { return updatedFromDB; }
            set { updatedFromDB = value; }
        }

        protected bool allowStoreToCache = true;
        public bool AllowStoreToCache
        {
            get { return allowStoreToCache; }
            set { allowStoreToCache = value; }
        }
        private bool allowExpire = true;
        public bool AllowExpire
        {
            get { return allowExpire; }
            set { allowExpire = value; }
        }

        private bool isDisposed = false;

        protected bool IsDisposed
        {
            get { return isDisposed; }
            set { isDisposed = value; }
        }
        protected bool isValid = false;

        public bool GetIsValid()
        {
            string thisclass=this.GetType().ToString();
            bool test = isValid && (lastRead != null) && (allowExpire);
            try
            {
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastTableUpdate = da.TableLastUpdated(tblName);
                    DateTime testTime = lastRead.AddHours(Lifespan);
                    //if ( thisclass== "Dynamic.DataLayer.RemoteOperatorLogs")
                    //{
                    //    Debug.WriteLine(this.GetType() + " " + isValid + " Count " + this.Count + " lastRead " + lastRead + " servertime " + da.ServerTime + " lasttableupdate " + lastTableUpdate);
                    //}                    
                    test = ((testTime > da.ServerTime) //still in lifespan
                        && (lastRead.Date == da.ServerTime.Date)); //still same day
                    if (test)
                    {
                        int x = lastTableUpdate.CompareTo(lastRead.AddSeconds(0.95));
                        test = (x <= 0);
                        if (!test) // if it has just expired, it must last as least as long as the min lifespan.
                        {
                            testTime = lastRead.AddHours(MinLifespan);
                            test = testTime > da.ServerTime;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return test || !allowExpire;
        }

        public void SetIsValid(bool isvalid)
        {
            //Debug.WriteLine(this.GetType()+" SetIsValid "+ isvalid);
            isValid = isvalid;
            if (!isValid)
                allowExpire = true;
        }
        public virtual bool IsValid
        {
            get
            {
                return GetIsValid();
            }
            set
            {
                SetIsValid(value);
            }
        }
        private bool isReadOnly = false;
        protected bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        } 
        #endregion

        #region restrictions

        private RestrictionRecs selectRestrictionRecs;
        private RestrictionRecs deleteRestrictionRecs;
        private RestrictionRecs updateRestrictionRecs;

        public bool IsRestricted
        {
            get { return ((selectRestrictionRecs != null) && (selectRestrictionRecs.Count > 0)); }
        }

        private void ResetRestrictions(RestrictionRecs restrictionRecs)
        {
            if (restrictionRecs == null)
                restrictionRecs = new RestrictionRecs();
            else
                restrictionRecs.Clear();
        }

        public void ResetSelectRestrictions()
        {
            ResetRestrictions(selectRestrictionRecs);
            //if (selectRestrictionRecs == null)
            //    selectRestrictionRecs = new RestrictionRecs();
            //else
            //    selectRestrictionRecs.Clear();
        }

        public void ResetDeleteRestrictions()
        {
            ResetRestrictions(deleteRestrictionRecs);
        }


        public void ResetUpdateRestrictions()
        {
            ResetRestrictions(updateRestrictionRecs);
        }

        public void AddRestriction(RestrictionRec restriction, ref RestrictionRecs restrictionRecs)
        {
            if (restrictionRecs == null)
                restrictionRecs = new RestrictionRecs();
            restrictionRecs.Add(restriction);
        }

        public void ClearRestrictions(ref RestrictionRecs restrictionRecs, ref String sqlStr)
        {
            if (restrictionRecs != null)
            {
                restrictionRecs.Clear();
            }
            sqlStr = string.Empty;
        }

        public void ClearRestrictions()
        {
            ClearSelectRestrictions();
            ClearDeleteRestrictions();
            ClearUpdateRestrictions();
        }

        public void AddSelectRestriction(RestrictionRec restriction)
        {
            AddRestriction(restriction, ref selectRestrictionRecs);
        }

        public void ClearSelectRestrictions()
        {
            ClearRestrictions(ref selectRestrictionRecs, ref defaultSqlSelectCondition);
        }

        public void AddDeleteRestriction(RestrictionRec restriction)
        {
            AddRestriction(restriction, ref deleteRestrictionRecs);
        }

        public void ClearDeleteRestrictions()
        {
            ClearRestrictions(ref deleteRestrictionRecs, ref defaultSqlDeleteCondition);
        }

        public void AddUpdateRestriction(RestrictionRec restriction)
        {
            AddRestriction(restriction, ref updateRestrictionRecs);
        }

        public void ClearUpdateRestrictions()
        {
            ClearRestrictions(ref updateRestrictionRecs, ref defaultSqlUpdateCondition);
        }

        //public void AddRestriction(string fieldname, string equalto)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, equalto);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}
        //public void AddRestriction(string fieldname, DateTime min, DateTime max)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, min, max);
        //    if (selectRestrictionRecs==null)
        //        selectRestrictionRecs=new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestrictionThisDay(string fieldname, DateTime thisday)
        //{
        //    DateTime dtA = thisday.Date;
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, dtA, dtA.AddDays(1), ComparisonType.MoreThanEqualALessThanB);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestrictionToday(string fieldname)
        //{
        //    DateTime dtA = DateTime.Today;
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, dtA, DateTime.Now, ComparisonType.MoreThanEqualALessThanB);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestrictionThisHour(string fieldname, DateTime thishour)
        //{
        //    DateTime dtA = new DateTime(thishour.Year, thishour.Month, thishour.Day, thishour.Hour, 0, 0);
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, dtA, dtA.AddHours(1), ComparisonType.MoreThanEqualALessThanB);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestriction(string fieldname, Int64 min, Int64 max)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, min, max);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestriction(string fieldname, Int64 min)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, min);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestriction(string fieldname, double min, double max)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, min, max);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //}

        //public void AddRestriction(string fieldname, string min, string max)
        //{
        //    RestrictionRec restrictionRec = new RestrictionRec(fieldname, min, max);
        //    if (selectRestrictionRecs == null)
        //        selectRestrictionRecs = new RestrictionRecs();
        //    selectRestrictionRecs.Add(restrictionRec);
        //} 

        #endregion

        private bool raiseException = false;

        public bool RaiseException
        {
            get { return raiseException; }
            set
            {
                raiseException = value;
                dBFieldMappings.RaiseException = value;
            }
        }

        private string selectException = string.Empty;

        public string SelectException
        {
            get { return selectException; }
            set { selectException = value; }
        }

        private Boolean selectResult = false;

        public Boolean SelectResult
        {
            get { return selectResult; }
            set { selectResult = value; }
        }

        private int timeoutSeconds = 30;

        public int TimeoutSeconds
        {
            get { return timeoutSeconds; }
            set { timeoutSeconds = value; }
        }

        protected bool forceUpdate = false;
        [XmlIgnore]
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        protected bool isSorted = false;
        [XmlIgnore]
        public bool IsSorted
        {
            get { return isSorted; }
            set { isSorted = value; }
        }

        private bool readLargeField = false;
        [XmlIgnore]
        public bool ReadLargeField
        {
            get { return readLargeField; }
            set { readLargeField = value; }
        }

        private int minID = 1;

        [XmlIgnore]
        public int MinID
        {
            get { return minID; }
            set { minID = value; }
        }

        private List<String> selectFieldNames;
        public List<String> SelectFieldNames
        {
            get { return selectFieldNames; }
            set { selectFieldNames = value; }
        }
        private List<String> insertFieldNames;

        public List<String> InsertFieldNames
        {
            get { return insertFieldNames; }
            set { insertFieldNames = value; }
        }
        private List<String> updateFieldNames;

        public List<String> UpdateFieldNames
        {
            get { return updateFieldNames; }
            set { updateFieldNames = value; }
        }
        

        #region sqlCommands;
        private SqlDataAccess da;
        private string tblName = "";
        public string TblName
        {
            get { return tblName; }
            set { tblName = value; }
        }

        public String FullTblName
        {
            get { return String.Format("{0}.{1}.{2}", DbName, SchemaName, TblName); }
        }

        private string dbName = "JEGR_DB";

        [XmlIgnore]
        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }
        private string schemaName = "dbo";

        [XmlIgnore]
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }
        protected DateTime lastTableUpdate;

        protected SqlColumns sqlColumns;
        protected DBFieldMappings dBFieldMappings;

        private string createTableCommand = string.Empty;
        public string CreateTableCommand
        {
            get { return createTableCommand; }
            set { createTableCommand = value; }
        }

        private string defaultSqlSelectCommand = string.Empty;
        protected string DefaultSqlSelectCommand
        {
            get { return defaultSqlSelectCommand; }
            set { defaultSqlSelectCommand = value; }
        }

        private string defaultSqlSelectCondition = string.Empty;

        public string DefaultSqlSelectCondition
        {
            get { return defaultSqlSelectCondition; }
            set
            {
                defaultSqlSelectCondition = value;
            }
        }
        private string sqlUpdateCommand = string.Empty;

        protected string SqlUpdateCommand
        {
            get { return sqlUpdateCommand; }
            set { sqlUpdateCommand = value; }
        }

        private string defaultSqlUpdateCondition = string.Empty;

        public string DefaultSqlUpdateCondition
        {
            get { return defaultSqlUpdateCondition; }
            set
            {
                defaultSqlUpdateCondition = value;
            }
        }
        private string sqlDeleteCommand = string.Empty;

        protected string SqlDeleteCommand
        {
            get { return sqlDeleteCommand; }
            set { sqlDeleteCommand = value; }
        }

        private string defaultSqlDeleteCondition = string.Empty;

        public string DefaultSqlDeleteCondition
        {
            get { return defaultSqlDeleteCondition; }
            set
            {
                defaultSqlDeleteCondition = value;
            }
        }
        protected string sqlInsertCommand = string.Empty;

        public string SqlInsertCommand
        {
            get { return sqlInsertCommand; }
            set { sqlInsertCommand = value; }
        }
        #endregion

        #endregion

        

        #region methods

        #region comparison
        public void CompareList(DataList compareList)
        {
            if ((compareList != null) 
                && (compareList.ListType == this.ListType)
                && (compareList.Count > 0)
                && (this.Count > 0))
            {
                foreach (DataItem item in compareList)
                {
                    DataItem thisItem = this.GetById(item.ID);
                }
            }
        }


        #endregion

        #region database select

        private List<String> allTableFields()
        {
            List<String> fields=new List<String>();
            foreach (DBFieldMapping mapping in dBFieldMappings)
            {
                fields.Add(mapping.DBColumn);
            }
            return fields;
        }

        private void checkFieldNameLists()
        {
            if (SelectFieldNames == null)
                SelectFieldNames = allTableFields();
            if (InsertFieldNames == null)
                InsertFieldNames = allTableFields();
            if (UpdateFieldNames == null)
                UpdateFieldNames = allTableFields();
        }

        protected virtual void GetDBFields(SqlDataReader dr)
        {
            try
            {
                if (sqlColumns == null)
                    sqlColumns = new SqlColumns();
                if (sqlColumns.Count == 0)
                {
                    if ((dr != null) && (dr.FieldCount > 0))
                    {
                        //for (int i = 0; i < dr.FieldCount; i++)
                        //{
                        //    SqlColumn colInfo = new SqlColumn();
                        //    colInfo.ColumnName = dr.GetName(i);
                        //    colInfo.ColumnOrdinal = i;
                        //    colInfo.DataType = dr.GetFieldType(i);
                        //    sqlColumns.Add(colInfo);
                        //}

                        DataTable clmData = dr.GetSchemaTable();

                        if (clmData != null)
                        {
                            foreach (DataRow col in clmData.Rows)
                            {
                                SqlColumn column = new SqlColumn();

                                column.AllowDBNull = col.Field<Boolean>("AllowDBNull");
                                column.ColumnName = col.Field<String>("ColumnName");
                                column.DataType = col.Field<Type>("DataType");
                                column.IsIdentity = col.Field<Boolean>("IsIdentity");
                                column.IsKey = col.Field<Boolean>("IsKey");
                                column.ColumnOrdinal = col.Field<Int32>("ColumnOrdinal");
                                column.ColumnSize = col.Field<Int32>("ColumnSize");

                                sqlColumns.Add(column);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
        }

        public virtual Boolean GetTableFields(SqlConnection dBConnection)
        {
            if ((SelectFieldNames == null) || (UpdateFieldNames == null) || (InsertFieldNames == null) || (sqlColumns == null))
            {
                if (tblName != string.Empty)
                {
                    string commandString = @"SELECT TOP 0 * FROM " + FullTblName;
                    try
                    {
                        SqlDataAccess da = SqlDataAccess.Singleton;
                        using (SqlCommand command = new SqlCommand(commandString))
                        {
                            if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                            {
                                command.Connection = dBConnection;
                            }
                            command.CommandTimeout = SqlClientExtension.Timeout;

                            // Open the connection if necessary
                            if (command.Connection.State == ConnectionState.Closed)
                            {
                                command.Connection.Open();
                                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.CloseConnection))
                                {
                                    GetDBFields(dr);
                                }
                            }
                            else
                            {
                                using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                                {
                                    GetDBFields(dr);
                                }
                            }
                        }
                        checkFieldNameLists();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GetTableFields " + this.tblName + Environment.NewLine + ex.Message);
                    }
                }
            }
            return ((sqlColumns != null) && (sqlColumns.Count > 0));
        }

        public virtual string BuildSelectCommandString(SqlConnection dBConnection)
        {
            string commandString = defaultSqlSelectCommand;
            try
            {
                if (commandString == string.Empty)
                {
                    if (GetTableFields(dBConnection))
                    {
                        int actualColumns = 0;
                        commandString = "SELECT ";
                        for (int i = 0; i < sqlColumns.Count; i++)
                        {
                            SqlColumn sCol = sqlColumns[i];
                            DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                            if ((dbMapping != null)
                                && (SelectFieldNames.Contains(dbMapping.DBColumn))
                                && ((!dbMapping.IsLargeField) || (dbMapping.IsLargeField && readLargeField)))
                            {
                                if (actualColumns > 0)
                                    commandString += ", ";
                                commandString += sCol.ColumnName;
                                actualColumns++;
                            }
                        }
                        int trailingCommaPos = commandString.Length - 2; //eliminate potential trailing comma
                        if (commandString[trailingCommaPos] == ',')
                        {
                            commandString = commandString.Remove(trailingCommaPos, 1);
                        }
                        commandString += " FROM " + FullTblName;
                    }
                }
                defaultSqlSelectCommand = commandString;
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return commandString;
        }

        public string DateTimeToSqlString(DateTime datetime)
        {
            return datetime.ToString("yyyyMMdd HH:mm");
        }

        public virtual string BuildConditionsString(SqlConnection dBConnection, RestrictionRecs restrictionrecs, String conditionString)
        {
            try
            {
                if ((restrictionrecs != null) && (restrictionrecs.Count > 0))
                {
                    if (GetTableFields(dBConnection))
                    {
                        conditionString = string.Empty;
                        foreach (RestrictionRec rec in restrictionrecs)
                        {
                            string elementString = rec.ElementSQL;
                            if (elementString != string.Empty)
                            {
                                if (conditionString != string.Empty)
                                    conditionString += " AND ";
                                conditionString += elementString;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return conditionString;
        }

        public virtual int DBSelect()
        {
            int selected = 0;
            try
            {
                selectException = string.Empty;
                da = SqlDataAccess.Singleton;
                da.DBName = dbName;
                SqlConnection dBConnection = da.SqlConnection;
                string sqlString = BuildSelectCommandString(dBConnection);
                defaultSqlSelectCondition = BuildConditionsString(dBConnection, selectRestrictionRecs, defaultSqlSelectCondition);
                selected = DBSelect(dBConnection, sqlString, defaultSqlSelectCondition);
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return selected;
        }

        public virtual int DBSelect(SqlConnection dBConnection, string sqlstring, string conditions)
        {
            int count = 0;
            try
            {
                selectResult = false;
                string commandString = sqlstring;
                if (conditions != string.Empty)
                {
                    commandString += " WHERE " + conditions;
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                    {
                        command.Connection = dBConnection;
                    }
                    command.CommandTimeout = timeoutSeconds;

                    // Open the connection if necessary
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        command.Connection.Open();
                        using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.KeyInfo))
                        {
                            count = Fill(dr);
                        }
                    }
                    else
                    {
                        using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                        {
                            count = Fill(dr);
                        }
                    }
                }
                //if (raiseException)
                //    Debug.WriteLine(MethodInfo.GetCurrentMethod() + " Records Read = " + this.Count);

                selectResult = true;
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return count;
        }

        #endregion

        #region database methods

        public int NextDBRecord(string idFieldName)
        {
            da = SqlDataAccess.Singleton;
            return da.NextRecord(tblName, idFieldName);
        }

        public Int64 NextID()
        {
            Int64 nextID = minID - 1;
            foreach (DataItem di in this)
            {
                if (di.ID > nextID)
                    nextID = di.ID;
            }
            nextID++;
            
            return nextID;
        }

        #region drop table

        public virtual bool DBDropTable() //requires testing
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            bool test = false;
            if (!isReadOnly)
            {
                try
                {
                    string commandString = "DROP TABLE " + FullTblName;
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        command.ExecuteNonQuery(command.Connection);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DBDropTable " + this.tblName + Environment.NewLine + ex.Message);
                }
            }
            return test;
        }

        #endregion

        #region test table exists

        public virtual bool DBTableExists() 
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            bool test = false;
            if (!isReadOnly)
            {
                try
                {
                    string commandString =
                        "SELECT CASE WHEN EXISTS((SELECT * FROM information_schema.tables WHERE table_name = '"
                        + FullTblName + "')) THEN 1 ELSE 0 END";
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        //command.ExecuteNonQuery(command.Connection);
                        test = (int)command.ExecuteScalar(command.Connection) == 1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DBTableExists " + this.tblName + Environment.NewLine + ex.Message);
                }
            }
            return test;
        }

        #endregion

        #region create table

        public virtual bool DBCreateTable() 
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            bool test = false;
            if (!isReadOnly && createTableCommand!=string.Empty)
            {
                try
                {
                    string commandString = createTableCommand;
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        command.ExecuteNonQuery(command.Connection);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DBDropTable " + this.tblName + Environment.NewLine + ex.Message);
                }
            }
            return test;
        }

        #endregion

        #region database insert

        public virtual string BuildInsertCommandString(SqlConnection dBConnection)
        {
            string commandString = sqlInsertCommand;
            if (commandString == string.Empty)
            {
                if (GetTableFields(dBConnection))
                {
                    int actualColumns = 0;
                    commandString = "INSERT INTO " + FullTblName + " (";
                    for (int i = 0; i < sqlColumns.Count; i++)
                    {
                        SqlColumn sCol = sqlColumns[i];
                        DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                        if ((dbMapping != null)
                            && (!dbMapping.IsIdentity)
                            && (InsertFieldNames.Contains(dbMapping.DBColumn)))
                        {
                            if (actualColumns > 0)
                                commandString += ", ";
                            commandString += sCol.ColumnName;
                            actualColumns++;
                        }
                    }
                    actualColumns = 0;
                    commandString += ") VALUES (";
                    for (int i = 0; i < sqlColumns.Count; i++)
                    {
                        SqlColumn sCol = sqlColumns[i];
                        DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                        if ((dbMapping != null)
                            && (!dbMapping.IsIdentity)
                            && (InsertFieldNames.Contains(dbMapping.DBColumn)))
                        {
                            if (actualColumns > 0)
                                commandString += ", ";
                            commandString += "@" + sCol.ColumnName;
                            actualColumns++;
                        }
                    }
                    commandString += ")";
                }
            }
            sqlInsertCommand = commandString;
            return commandString;
        }

        public virtual bool DBInsert(DataItem dataItem)
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            string sqlString = BuildInsertCommandString(dBConnection);
            return DBInsert(dBConnection, sqlString, dataItem);
        }

        public virtual bool DBInsert(SqlConnection dBConnection, string sqlstring, DataItem dataItem)
        {
            bool test = true;
            if (!isReadOnly)
            {
                try
                {
                    string commandString = sqlstring;
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        SetParameters(command, dataItem, false, InsertFieldNames);
                        command.ExecuteNonQuery(command.Connection);
                        dataItem.HasChanged = false;
                        dataItem.ForceNew = false;
                    }
                }
                catch (Exception ex)
                {
                    test = false;
                    Debug.WriteLine("DBInsert " + this.tblName + Environment.NewLine + ex.Message);
                }
            }
            return test;
        }

        #endregion

        #region database update

        public virtual string BuildUpdateCommandString(SqlConnection dBConnection)
        {
            string commandString = sqlUpdateCommand;
            if (commandString == string.Empty)
            {
                if (GetTableFields(dBConnection))
                {
                    int actualColumns = 0;
                    commandString = "UPDATE " + FullTblName + " SET ";
                    for (int i = 0; i < sqlColumns.Count; i++)
                    {
                        SqlColumn sCol = sqlColumns[i];
                        DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                        if ((dbMapping != null) && (!dbMapping.IsIdentity) && (!dbMapping.IsIDField) && (!dbMapping.IsReadOnly)
                            && (UpdateFieldNames.Contains(dbMapping.DBColumn)))
                        {
                            if (actualColumns > 0)
                                commandString += ", ";
                            commandString += sCol.ColumnName + " = @" + sCol.ColumnName;
                            actualColumns++;
                        }
                    }
                    commandString += " WHERE ";
                    int keyCount = 0;
                    for (int i = 0; i < sqlColumns.Count; i++)
                    {
                        SqlColumn sCol = sqlColumns[i];
                        DBFieldMapping dbMapping = dBFieldMappings.GetByDBColumn(sCol.ColumnName);
                        if ((dbMapping != null) && (dbMapping.IsIDField))
                        {
                            keyCount++;
                            if (keyCount > 1) commandString += " AND ";
                            commandString += sCol.ColumnName + " = @" + sCol.ColumnName;
                        }
                    }
                }
            }
            sqlUpdateCommand = commandString;
            return commandString;
        }

        public virtual bool DBUpdate(DataItem dataItem)
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            string sqlString = BuildUpdateCommandString(dBConnection);
            defaultSqlUpdateCondition = BuildConditionsString(dBConnection, updateRestrictionRecs, defaultSqlUpdateCondition);
            return DBUpdate(dBConnection, sqlString, dataItem, defaultSqlUpdateCondition);
        }

        public virtual bool DBUpdate(SqlConnection dBConnection, string sqlstring, DataItem dataItem, String conditions)
        {
            bool test = true;
            if (!isReadOnly)
            {
                try
                {
                    string commandString = sqlstring;
                    if (conditions != string.Empty)
                    {
                        commandString += " AND " + conditions;
                    } 
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        SetParameters(command, dataItem, true, UpdateFieldNames);
                        command.ExecuteNonQuery(command.Connection);
                        dataItem.HasChanged = false;
                        dataItem.ForceNew = false;
                    }
                    test = true;
                }
                catch (Exception ex)
                {
                    test = false;
                    Debug.WriteLine("DBUpdate " + this.tblName + Environment.NewLine + ex.Message);
                }
            }
            return test;
        }

        #endregion

        #region dbdelete

        public virtual void DBTruncateTable()
        {
            if (!isReadOnly)
            {
                da = SqlDataAccess.Singleton;
                da.TruncateTable(FullTblName);
            }
        }

        public virtual Boolean DBDeleteRecord(DataItem dataItem)
        {
            bool test = false;
            try
            {
                if (!isReadOnly)
                {
                    da = SqlDataAccess.Singleton;
                    DBFieldMapping dbFieldMapping = dBFieldMappings.GetIDField();
                    if (dbFieldMapping != null)
                    {
                        da.DeleteTableRecord(FullTblName, dbFieldMapping.DBColumn, dataItem.ID);
                    }
                }
                test = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.GetType().Name + " " + MethodInfo.GetCurrentMethod() + ex.Message);
                test = false;
            }
            return test;
        }

        public virtual string BuildDeleteCommandString(SqlConnection dBConnection)
        {
            string commandString = sqlDeleteCommand;
            if (commandString == string.Empty)
            {
                commandString = "DELETE FROM " + FullTblName;
            }
            sqlUpdateCommand = commandString;
            return commandString;
        }

        public virtual Boolean DBDelete()
        {
            da = SqlDataAccess.Singleton;
            da.DBName = dbName;
            SqlConnection dBConnection = da.SqlConnection;
            string sqlString = BuildDeleteCommandString(dBConnection);
            raiseException = true;
            defaultSqlDeleteCondition = BuildConditionsString(dBConnection, deleteRestrictionRecs, defaultSqlDeleteCondition);
            return DBDelete(dBConnection, sqlString, defaultSqlDeleteCondition);
        }

        public virtual Boolean DBDelete(SqlConnection dBConnection, string sqlstring, string conditions)
        {
            Boolean test = false;
            try
            {
                string commandString = sqlstring;
                if (conditions != string.Empty)
                {
                    commandString += " WHERE " + conditions;
                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                        {
                            command.Connection = dBConnection;
                        }
                        command.CommandTimeout = SqlClientExtension.Timeout;
                        command.ExecuteNonQuery(command.Connection);
                        Debug.WriteLine(commandString);
                    }
                    test = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DBDelete " + this.tblName + Environment.NewLine + ex.Message);
            }
            return test;
        }

        #endregion

        #region database fill methods

        protected virtual void SetLastReadTime()
        {
            try
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                lastRead = da.ServerTime;
                lastTableUpdate = lastRead;
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
        }

        protected virtual int GetFieldIndex(string fieldName)
        {
            int x = -1;
            SqlColumn ci = sqlColumns.GetByName(fieldName);
            if (ci != null)
                x = ci.ColumnOrdinal;
            return x;
        }

        protected int GetFieldValue(SqlDataReader dr, string fieldName, int defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected int GetFieldValue(SqlDataReader dr, int fieldIndex, int defaultValue)
        {
            int value = defaultValue;
            if (fieldIndex >= 0)
            {
                SqlColumn ci = sqlColumns.GetById(fieldIndex);
                if (ci.DataType.Equals(typeof(Int32)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt32(fieldIndex);
                if (ci.DataType.Equals(typeof(Int16)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt16(fieldIndex);
            }
            return value;
        }

        protected short GetFieldValue(SqlDataReader dr, string fieldName, ref short value, short defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected short GetFieldValue(SqlDataReader dr, int fieldIndex, short defaultValue)
        {
            short value = defaultValue;
            if (fieldIndex >= 0)
            {
                SqlColumn ci = sqlColumns.GetById(fieldIndex);
                if (ci.DataType.Equals(typeof(Int32)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : (short)dr.GetInt32(fieldIndex);
                if (ci.DataType.Equals(typeof(Int16)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt16(fieldIndex);
            }
            return value;
        }

        protected double GetFieldValue(SqlDataReader dr, string fieldName, double defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected double GetFieldValue(SqlDataReader dr, int fieldIndex, double defaultValue)
        {
            double value = defaultValue;
            if (fieldIndex >= 0)
            {
                SqlColumn ci = sqlColumns.GetById(fieldIndex);
                if (ci.DataType.Equals(typeof(decimal)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : (double)dr.GetDecimal(fieldIndex);
                if (ci.DataType.Equals(typeof(Int32)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt32(fieldIndex);
                if (ci.DataType.Equals(typeof(Int16)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt16(fieldIndex);
            }
            return value;
        }

        protected decimal GetFieldValue(SqlDataReader dr, string fieldName, decimal defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected decimal GetFieldValue(SqlDataReader dr, int fieldIndex, decimal defaultValue)
        {
            decimal value = defaultValue;
            if (fieldIndex >= 0)
            {
                SqlColumn ci = sqlColumns.GetById(fieldIndex);
                if (ci.DataType.Equals(typeof(decimal)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetDecimal(fieldIndex);
                if (ci.DataType.Equals(typeof(Int32)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt32(fieldIndex);
                if (ci.DataType.Equals(typeof(Int16)))
                    value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetInt16(fieldIndex);
            }
            return value;
        }

        protected string GetFieldValue(SqlDataReader dr, string fieldName, string defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected string GetFieldValue(SqlDataReader dr, int fieldIndex, string defaultValue)
        {
            string value = defaultValue;
            if (fieldIndex >= 0)
            {
                value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetString(fieldIndex);
            }
            return value;
        }

        protected Boolean GetFieldValue(SqlDataReader dr, string fieldName, Boolean defaultValue)
        {
            int fieldIndex = GetFieldIndex(fieldName);
            return GetFieldValue(dr, fieldIndex, defaultValue);
        }

        protected Boolean GetFieldValue(SqlDataReader dr, int fieldIndex, Boolean defaultValue)
        {
            Boolean value = defaultValue;
            if (fieldIndex >= 0)
            {
                value = dr.IsDBNull(fieldIndex) ? defaultValue : dr.GetBoolean(fieldIndex);
            }
            return value;
        }

        protected virtual void FillRecords(SqlDataReader dr) 
        {
            int recno = 0;
            try
            {
                if (dBFieldMappings != null && sqlColumns != null)
                {
                    this.Clear();
                    while (dr.Read())
                    {
                        recno++;
                        DataItem dataItem = NewItem();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string dbColName = dr.GetName(i);
                            DBFieldMapping dbf = dBFieldMappings.GetByDBColumn(dbColName);
                            SqlColumn sqlColumn = sqlColumns.GetByName(dbColName);
                            if (dbf != null && sqlColumn != null)
                            {
                                Object o = dr.GetValue(i);
                                if (o != DBNull.Value)
                                {
                                    dbf.SetValue(dataItem, o);
                                }
                                else
                                {
                                    //if (raiseException)
                                    //    Debug.WriteLine(MethodInfo.GetCurrentMethod() + "object is dbnull for field " + i + " Record " + recno);
                                }
                            }
                            else
                            {
                                //if (raiseException)
                                //    Debug.WriteLine(MethodInfo.GetCurrentMethod() + "DBFieldMapping is null for " + dbColName + " Record " + recno);
                            }
                        }
                        dataItem.HasChanged = false;
                        this.Add(dataItem);
                    }
                    //if (raiseException)
                    //    Debug.WriteLine(MethodInfo.GetCurrentMethod() + "Total records = " + recno + " - " + this.Count);
                }
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
        }

        protected virtual void SetParameters(SqlCommand sqlCommand, DataItem dataItem, Boolean allowIdentity, List<String> fieldNames)
        {
            if ((dBFieldMappings != null) && (sqlColumns != null))
            {
                for (int i = 0; i < dBFieldMappings.Count; i++)
                {
                    DBFieldMapping dbMapping = dBFieldMappings[i];
                    string dbColName = dbMapping.DBColumn;
                    SqlColumn sqlColumn = sqlColumns.GetByName(dbColName);
                    if ((dbMapping != null) && (dbMapping.FieldPropertyInfo != null)
                        && (((!dbMapping.IsIdentity) || allowIdentity)
                        || (dbMapping.IsIDField)
                        || (fieldNames.Contains(dbMapping.DBColumn))))
                    {
    
                        // FOR FUTURE TESTING 13/08/2015
                        //Object obj = dbMapping.FieldPropertyInfo.GetValue(dataItem, null);
                        //if (obj.GetType() == typeof(DateTime) && obj.Equals(DateTime.MinValue))
                        //    sqlCommand.Parameters.AddWithValue("@" + dbColName, DBNull.Value);
                        //else
                            sqlCommand.Parameters.AddWithValue("@" + dbColName, dbMapping.FieldPropertyInfo.GetValue(dataItem, null));
                    }
                }
            }
        }

        public virtual void SetNextID(DataItem dataItem)
        {
            if (dBFieldMappings != null)
            {
                DBFieldMapping idFieldMapping = dBFieldMappings.GetIDField();
                if (idFieldMapping != null)
                {
                    int nextid = GetNextID();
                    idFieldMapping.SetValue(dataItem, nextid);
                }
            }
        }

        public virtual DataItem NewItem()
        {
            DataItem dataItem = (DataItem)Activator.CreateInstance(ListType);
            dataItem.DBFieldMappings = dBFieldMappings;
            return dataItem;
        }

        public virtual int Fill(SqlDataReader dr)
        {
            try
            {
                GetDBFields(dr);
                FillRecords(dr);
                SetLastReadTime();
                IsValid = true;
                //bool test = GetIsValid();
                //if (raiseException)
                //    Debug.WriteLine(MethodInfo.GetCurrentMethod() + " Records Read = "+this.Count);
            }
            catch (Exception ex)
            {
                selectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return this.Count;
        }
 
        #endregion
 
        #region updatedb

        public virtual Boolean UpdateToDB()
        {
            int inserts = 0;
            int deletes = 0;
            int updates = 0;
            bool test = true;
            if (ForceUpdate)
            {
                DBTruncateTable();
                foreach (DataItem dataItem in this)
                {
                    if (test)
                    {
                        dataItem.Housekeeping();
                        if (!dataItem.DeleteRecord)
                        {
                            inserts++;
                            test = DBInsert(dataItem);
                        }
                    }
                }
            }
            else
            {
                foreach (DataItem dataItem in this)
                {
                    if (test)
                    {
                        dataItem.Housekeeping();
                        if (dataItem.IsNew)
                        {
                            if (!dataItem.DeleteRecord)
                            {
                                test = DBInsert(dataItem);
                                inserts++;
                            }
                        }
                        else
                        {
                            if (dataItem.HasChanged || dataItem.DeleteUnwrittenRecord)
                            {
                                if (dataItem.DeleteRecord)
                                {
                                    test = DBDeleteRecord(dataItem);
                                    deletes++;
                                }
                                else
                                {
                                    test = DBUpdate(dataItem);
                                    updates++;
                                }
                            }
                        }
                    }
                }
                DataList delList = new DataList();
                foreach (DataItem dataItem in this)
                {
                    if (dataItem.DeleteRecord)
                        delList.Add(dataItem);
                }
                foreach (DataItem dataItem in delList)
                {
                    this.Remove(dataItem);
                }
            }
            if ((inserts > 0) || (updates > 0) || (deletes > 0))
            {
                Reset();
                //Debug.WriteLine(tblName + " Inserts = " + inserts + " Updates = " + updates + " Deletes = " + deletes);
            }
            return test;
        }

        #endregion
       
        #endregion

        public virtual DataList ActiveList(DataList activeList)
        {
            activeList.Clear();
            foreach (DataItem dataItem in this)
            {
                if (dataItem.Active)
                    activeList.Add(dataItem);
            }
            return activeList;
        }

        protected virtual int GetNextID()
        {
            int id = 0;
            bool found;
            {
                do
                {
                    id++;
                    found = false;
                    foreach (DataItem rec in this)
                    {
                        found |= (rec.ID == id);
                    }
                } while (found);
            }
            return id;
        }

        public virtual String GetIDListString()
        {
            string ids = string.Empty;
            Sort();
            DataItem di=null;
            for (int i=0;i<this.Count;i++)
            {
                di=this[i];
                ids += di.ID;
                if (i < this.Count - 1)
                {
                    ids += ",";
                }
            }
            return ids;
        }

        public void SetDeleteUnwritten()
        {
            foreach (DataItem di in this)
                di.DeleteUnwrittenRecord = true;
        }

        #endregion

        #region find

        public virtual DataItem GetById(Int64 id)
        {
            return this.Find(delegate(DataItem item)
            {
                return item.ID == id;
            });
        }

        public virtual DataItem GetById2(Int64 id)
        {
            return this.Find(delegate(DataItem item)
            {
                return item.ID2 == id;
            });
        }

        public virtual DataItem GetById(Int64 id, Int64 id2)
        {
            return this.Find(delegate(DataItem item)
            {
                return (item.ID == id) && (item.ID2 == id2);
            });
        }

        public virtual DataItem GetByName(string aName)
        {
            return this.Find(delegate(DataItem dataItem)
            {
                return dataItem.ItemName == aName;
            });
        }

        public DataItem GetByNameID(string aNameID)
        {
            return this.Find(delegate(DataItem dataItem)
            {
                return dataItem.NameAndID == aNameID;
            });
        }

        public Int64 GetIDbyNameID(string aNameID)
        {
            Int64 id = -1;
            DataItem item = GetByNameID(aNameID);
            if (item != null)
            {
                id = item.ID;
            }
            return id;
        }

        #endregion

    }

    public enum SortType
    {
        SortByID, SortByID2, SortByName, SortByDescription, SortCustom
    }

    public class DataItemSorter : IComparer
    {
        private SortType sortType = SortType.SortByName;
        private Boolean sortAscending = true;

        public DataItemSorter()
        {}

        public DataItemSorter(SortType sorttype, Boolean sortascending)
            : this()
        {
            sortType = sorttype;
            sortAscending = sortascending;
        }

        public virtual int CompareID(DataItem item1, DataItem item2)
        {
            if (sortAscending)
                return item1.ID.CompareTo(item2.ID);
            else
                return item2.ID.CompareTo(item1.ID);
        }

        public virtual int CompareID2(DataItem item1, DataItem item2)
        {
            if (sortAscending)
                return item1.ID2.CompareTo(item2.ID2);
            else
                return item2.ID2.CompareTo(item1.ID2);
        }

        public virtual int CompareName(DataItem item1, DataItem item2)
        {
            if (sortAscending)
                return item1.ItemName.CompareTo(item2.ItemName);
            else
                return item2.ItemName.CompareTo(item1.ItemName);
        }

        public virtual int CompareDescription(DataItem item1, DataItem item2)
        {
            if (sortAscending)
                return item1.Description.CompareTo(item2.Description);
            else
                return item2.Description.CompareTo(item1.Description);
        }

        public virtual int CompareCustom(DataItem item1, DataItem item2)
        {
            return 0;//override this!
        }

        public virtual int Compare(object obj1, object obj2)
        {
            DataItem item1 = obj1 as DataItem;
            DataItem item2 = obj2 as DataItem;
            int result = 0;
            if (sortType == SortType.SortByID)
                result = CompareID(item1, item2);
            if (sortType == SortType.SortByID2)
                result = CompareID2(item1, item2);
            if (sortType == SortType.SortByName)
                result = CompareName(item1, item2);
            if (sortType == SortType.SortByDescription)
                result = CompareDescription(item1, item2); 
            if (sortType == SortType.SortCustom)
                result = CompareCustom(item1, item2);
            return result;
        }
    }


    [XmlInclude(typeof(DBConnection)), XmlInclude(typeof(Operator))]
    public class DataItem : ObservableObject, IComparable<DataItem>, IEditableObject
    {
        #region properties

        [XmlIgnore]
        public bool DBChanged { get; set; }

        private bool deleteRecord = false;
        [XmlIgnore]
        public bool DeleteRecord
        {
            get { return deleteRecord || DeleteUnwrittenRecord || DeleteUnchangedRecord; }
            set { deleteRecord = AssignNotify(ref deleteRecord, value, "DeleteRecord"); }
        }

        [XmlIgnore]
        public DBFieldMappings DBFieldMappings { get; set; }

        [XmlIgnore]
        public virtual bool Active { get; set; }

        [XmlIgnore]
        public bool ForceNew { get; set; }

        private Int64 iD = 0;
        [XmlIgnore]
        public virtual Int64 ID
        {
            get { return iD; }
            set { iD = AssignNotify(ref iD, value, "ID"); }
        }

        private Int64 iD2 = 0;
        [XmlIgnore]
        public virtual Int64 ID2
        {
            get { return iD2; }
            set { iD2 = AssignNotify(ref iD2, value, "ID2"); }
        }

        private Int64 primaryKey = 0;
        [XmlIgnore]
        public virtual Int64 PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = AssignNotify(ref primaryKey, value, "PrimaryKey"); }
        }

        private string itemName = string.Empty;
        [XmlIgnore]
        public /*virtual*/ string ItemName
        {
            get { return itemName; }
            set { itemName = AssignNotify(ref itemName, value, "ItemName"); }
        }

        private string description = string.Empty;
        [XmlIgnore]
        public /*virtual*/ string Description
        {
            get { return description; }
            set { description = AssignNotify(ref description, value, "Description"); }
        }

        [XmlIgnore]
        public virtual DataList dataList { get; set; }

        [XmlIgnore]
        public virtual string NameAndID
        {
            get
            {
                return ItemName + " (" + ID.ToString() + ")";
            }
        }

        [XmlIgnore]
        public virtual bool IsNew
        {
            get
            {
                return ((PrimaryKey <= -1) || ForceNew);
            }
        }

        [XmlIgnore]
        public virtual bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }

        #endregion

        #region copy

        protected virtual DataItem NewItem() 
        {
            DataItem dataItem = (DataItem)Activator.CreateInstance(this.GetType());
            return dataItem;
        }

        public virtual DataItem Copy() 
        {
            DataItem dataItem = (DataItem)MemberwiseClone();
            dataItem.ForceNew = true;
            return dataItem;
        }

        public virtual void CopyBack(DataItem diFrom)
        {
            if ((DBFieldMappings != null)
                && (DBFieldMappings.Count > 0))
            {
                foreach (DBFieldMapping dbf in DBFieldMappings)
                {
                    if (dbf != null)
                    {
                        Object o = dbf.FieldPropertyInfo.GetValue(diFrom, null);
                        if (o != DBNull.Value)
                        {
                            dbf.SetValue(this, o);
                        }
                    }
                }
            }
        }

        #endregion

        #region IEditableObject

        //public virtual DataItem ActiveData { get; set; }

        public virtual DataItem BackupData { get; set; }

        [XmlIgnore]
        public virtual bool IsEditing { get; set; }

        void IEditableObject.BeginEdit()
        {
            if (!IsEditing)
            {
                BackupData = this.Copy();
                IsEditing = true;
            }
        }

        void IEditableObject.CancelEdit()
        {
            //if (IsEditing)
            //{
            //    this.ActiveData = BackupData;
            //    IsEditing = false;
            //}
        }

        void IEditableObject.EndEdit()
        {
            this.BackupData = null;
            IsEditing = false;
        }

        #endregion

        #region IComparable //refactor to use DataItemSorter
        private SortType sortType = SortType.SortByName;

        [XmlIgnore]
        public SortType SortType
        {
            get { return sortType; }
            set { sortType = value; }
        }

        private Boolean sortAscending = true;

        [XmlIgnore]
        public Boolean SortAscending
        {
            get { return sortAscending; }
            set { sortAscending = value; }
        }

        public virtual int CompareToID(DataItem other)
        {
            if (sortAscending)
            {
                return ID.CompareTo(other.ID);
            }
            else
            {
                return other.ID.CompareTo(ID);
            }
        }

        public virtual int CompareToName(DataItem other)
        {
            if (sortAscending)
            {
                return ItemName.CompareTo(other.ItemName);
            }
            else
            {
                return other.ItemName.CompareTo(ItemName);
            }
        }

        public virtual int CompareToDesc(DataItem other)
        {
            if (sortAscending)
            {
                return Description.CompareTo(other.Description);
            }
            else
            {
                return other.Description.CompareTo(Description);
            }
        }

        public virtual int CompareToCustom(DataItem other)
        {
            return CompareToName(other);
        }

        //public int CompareTo(DataItem other) { return ItemName.CompareTo(other.ItemName); }
        public virtual int CompareTo(DataItem other)
        {
            int result = 0;
            switch (SortType)
            {
                case SortType.SortByID:
                    result = CompareToID(other);
                    break;
                case SortType.SortByName:
                    result = CompareToName(other);
                    break;
                case SortType.SortByDescription:
                    result = CompareToName(other);
                    break;
                case SortType.SortCustom:
                    result = CompareToDesc(other);
                    break;
                default:
                    result = CompareToCustom(other);
                    break;
            }
            return result;
        }

        #endregion

        #region conversion methods


        public static void ResizeImage(byte[] imageBytes, int width, int height)
        {
            // decode the image to the requested width and height
            BitmapSource imageSource = BytesToImageSource(imageBytes, width, height);

            // encode the image using the original format
            byte[] encodedBytes = ImageSourceToBytes(imageSource);

        }

        public static BitmapSource BytesToImageSource(byte[] imageData)
        {
            return BytesToBitmapSource(imageData);
        }

        public BitmapImage BytesToBitmapImage(Byte[] imageData)
        {
            if (imageData == null) return null;
            Stream stream = new MemoryStream(imageData);

            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static BitmapSource BytesToBitmapSource(Byte[] imageData)
        {
            BitmapSource bitmapSource=null;
            try
            {

                if (imageData == null) return null;
                Stream stream = new MemoryStream(imageData);

                PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                bitmapSource = decoder.Frames[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BytesToBitmapSource " + ex.Message);
            }
            return bitmapSource;
        }

        public static BitmapSource BytesToImageSource(byte[] imageData, int decodePixelWidth, int decodePixelHeight)
        {
            if (imageData == null) return null;
            Stream stream = new MemoryStream(imageData);
            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bitmapSource = decoder.Frames[0];
            return bitmapSource;
        }

        public System.Drawing.Color ColorFromInt(int color)
        {
            string str = "#" + color.ToString("X8");
            System.Drawing.ColorConverter conv = new System.Drawing.ColorConverter();
            return (System.Drawing.Color)conv.ConvertFromString(str);
        }

        public Color ColorFromLong(int longColor)
        {
            string str = "#" + longColor.ToString("X8");
            return (Color)ColorConverter.ConvertFromString(str);
        }

        public static byte[] ImageSourceToBytes(ImageSource image)
        {
            string preferredFormat = ".png";
            byte[] result = null;
            BitmapEncoder encoder = null;
            switch (preferredFormat.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
            }

            if (image is BitmapSource)
            {
                MemoryStream stream = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = new byte[stream.Length];
                BinaryReader br = new BinaryReader(stream);
                br.Read(result, 0, (int)stream.Length);
                br.Close();
                stream.Close();
            }
            return result;
        }

        public Int64 Int32sTo64(int a, int b)
        {
            return ((long)a << 32) + b;
        }

        public int[] GetIDsFromString(String aString)
        {
            int[] ids = null;
            char[] separators = { ' ', ',' };
            string[] stringIDs = aString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (stringIDs.Length > 0)
            {
                ids = new int[stringIDs.Length];
                for (int i = 0; i < stringIDs.Length; i++)
                {
                    int x = 0;
                    if (int.TryParse(stringIDs[i], out x))
                        ids[i] = x;
                }
            }
            return ids;
        }

        #endregion

        #region orm

        public string[] ListDataProperties()
        {
            Type t = this.GetType();
            PropertyInfo[] p = t.GetProperties();
            ArrayList properties = new ArrayList();
            foreach (PropertyInfo pi in p)
            {
                string s = string.Empty;
                try
                {
                    s = pi.Name;
                    try
                    {
                        s += ": " + pi.GetValue(this, null).ToString();
                    }
                    catch (Exception ex)
                    {
                        s += ": " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    s += ": " + ex.Message;
                }
                properties.Add(s); //could instead write these out to some parameters.
            } 
            string[] values = new string[properties.Count];
            properties.CopyTo(values);
            return values;
        }

        //private DataItem dbItem = null;
        private DataItem undoItem = null;
        private DataItem redoItem = null;

        public bool CanUndo
        {
            get { return undoItem != null; }
        }

        public bool CanRedo
        {
            get { return redoItem != null; }
        }

        #endregion

        public virtual void Housekeeping()
        { }

    }

    #region mapping classes

    public class SqlColumns : List<SqlColumn>
    {

        public SqlColumn GetById(int id)
        {
            return this.Find(delegate(SqlColumn item)
            {
                return item.ColumnOrdinal == id;
            });
        }

        public SqlColumn GetByName(string aName)
        {
            string test = "";
            return this.Find(delegate(SqlColumn dataItem)
            {               
                return string.Equals(dataItem.ColumnName, aName, StringComparison.CurrentCultureIgnoreCase);
            });
        }
    }

    public class SqlColumn
    {
        public string ColumnName { get; set; }
        public Int32 ColumnSize { get; set; }
        public bool IsKey { get; set; }
        public Type DataType { get; set; }
        public bool AllowDBNull { get; set; }
        public bool IsIdentity { get; set; }
        public Int32 ColumnOrdinal { get; set; }
    }

    public class DBFieldMappings : List<DBFieldMapping>
    {
        private Boolean raiseException = false;
        public Boolean RaiseException
        {
            get { return raiseException; }
            set
            {
                raiseException = value;
                foreach (DBFieldMapping dbFieldMapping in this)
                {
                    dbFieldMapping.RaiseException = value;
                    exceptionInfo = string.Empty;
                }
            }
        }
        private string exceptionInfo = string.Empty;

        public void AddMapping(string dbColumn, string fieldName, bool idField, bool identity, bool isreadonly, bool islargefield)
        {
            DBFieldMapping dBFieldMapping = new DBFieldMapping();
            dBFieldMapping.DBColumn = dbColumn;
            dBFieldMapping.FieldPropertyName = fieldName;
            dBFieldMapping.IsLargeField = islargefield;
            dBFieldMapping.IsIDField = idField;
            dBFieldMapping.IsReadOnly = isreadonly;
            dBFieldMapping.IsIdentity = identity;
            this.Add(dBFieldMapping);
        }

        public void AddMapping(string dbColumn, string fieldName, bool isreadonly)
        {
            AddMapping(dbColumn, fieldName, false, false, isreadonly, false);
        }

        public void AddMapping(string dbColumn, string fieldName)
        {
            AddMapping(dbColumn, fieldName, false, false, false, false);
        }

        public void AddMapping(string dbColumn, string fieldName, bool idField, bool identity)
        {
            AddMapping(dbColumn, fieldName, idField, identity, false, false);
        }

        public void AddPropertyInfos(PropertyInfo[] pInfos)
        {
            for (int i = 0; i < pInfos.Length; i++)
            {
                PropertyInfo pi = pInfos[i];
                DBFieldMapping dbf = GetByFieldName(pi.Name);
                if (dbf != null)
                {
                    dbf.FieldPropertyInfo = pi;
                }
            }
        }

        public DBFieldMapping GetByDBColumn(string aName)
        {
            return this.Find(delegate(DBFieldMapping dataItem)
            {
                return dataItem.DBColumn.ToLower() == aName.ToLower();
            });
        }

        public DBFieldMapping GetByFieldName(string aName)
        {
            return this.Find(delegate(DBFieldMapping dataItem)
            {
                return dataItem.FieldPropertyName == aName;
            });
        }

        public DBFieldMapping GetIDField()
        {
            return this.Find(delegate(DBFieldMapping dataItem)
            {
                return dataItem.IsIDField;
            });
        }
    }

    public class DBFieldMappingException : Exception
    {
        public DBFieldMappingException()
            : base() { }

        public DBFieldMappingException(string message)
            : base(message) { }

        public DBFieldMappingException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public DBFieldMappingException(string message, Exception innerException)
            : base(message, innerException) { }

        public DBFieldMappingException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

    }

    public class DBFieldMapping
    {
        private Boolean raiseException = false;
        public Boolean RaiseException
        {
            get { return raiseException; }
            set
            {
                raiseException = value;
                exceptionInfo = string.Empty;
            }
        }

        private string exceptionInfo = string.Empty;
        public string DBColumn { get; set; }
        public string FieldPropertyName { get; set; }
        public PropertyInfo FieldPropertyInfo { get; set; }
        private bool isIDField = false;
        public bool IsIDField
        {
            get { return isIDField; }
            set { isIDField = value; }
        }
        private bool isReadOnly = false;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        } 
        private bool isIdentity = false;
        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        private bool isLargeField = false;

        public bool IsLargeField
        {
            get { return isLargeField; }
            set { isLargeField = value; }
        }

        private bool isNumberOrChar()
        {
            return ((FieldPropertyInfo.PropertyType == typeof(byte))
                    || (FieldPropertyInfo.PropertyType == typeof(char))
                    || (FieldPropertyInfo.PropertyType == typeof(sbyte))
                    || (FieldPropertyInfo.PropertyType == typeof(short))
                    || (FieldPropertyInfo.PropertyType == typeof(ushort))
                    || (FieldPropertyInfo.PropertyType == typeof(int))
                    || (FieldPropertyInfo.PropertyType == typeof(uint))
                    || (FieldPropertyInfo.PropertyType == typeof(long))
                    || (FieldPropertyInfo.PropertyType == typeof(ulong))
                    || (FieldPropertyInfo.PropertyType == typeof(double))
                    || (FieldPropertyInfo.PropertyType == typeof(decimal))
                    || (FieldPropertyInfo.PropertyType == typeof(float)));
        }

        private bool isBool()
        {
            return (FieldPropertyInfo.PropertyType == typeof(bool));
        }

        private bool isString()
        {
            return (FieldPropertyInfo.PropertyType == typeof(string));
        }

        private bool isDateTime()
        {
            return (FieldPropertyInfo.PropertyType == typeof(DateTime));
        }

        private bool AssignmentCompatibility(Type inType, Type outType)
        {
            bool isCompatible=false;
            try
            {
                isCompatible = ((inType == outType) ||
                    ((inType == typeof(SByte) || inType == typeof(Byte) || inType == typeof(Int16) || inType == typeof(Int32)) && (outType == typeof(Int32) || outType == typeof(Int64))));
            }
            catch (Exception ex)
            {
                exceptionInfo += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return isCompatible;
        }

        private bool CanConvert(Type inType, Type outType)
        {
            return ((inType == typeof(Decimal) && (outType == typeof(double))));
        }

        private Object ConvertTypes(Type inType, Type outType, Object obj)
        {
            Object outObj = null;
            try
            {
                if (outType == typeof(double) && inType == typeof(decimal))
                {
                    decimal dec = (decimal)obj;
                    double d = (double)dec;
                    outObj = d;
                }
            }
            catch (Exception ex)
            {
                exceptionInfo += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return outObj;
        }

        public Object CheckType(Type inType, Type outType, Object obj)
        {
            Object outObj = null;
            try
            {
                if (AssignmentCompatibility(inType, outType))
                {
                    outObj = obj;
                }
                else
                {
                    if (CanConvert(inType, outType))
                    {
                        outObj = ConvertTypes(inType, outType, obj);
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionInfo += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return outObj;
        }

        public void SetValue(DataItem dataItem, Object obj)
        {
            try
            {
                Type inType = obj.GetType();
                Type outType = FieldPropertyInfo.PropertyType;
                FieldPropertyInfo.SetValue(dataItem, CheckType(inType, outType, obj), null);
            }
            catch (Exception ex)
            {
                exceptionInfo += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                {
                    throw new DBFieldMappingException("DBFieldMappingException! " + exceptionInfo);
                }
            }

        }

        //public Object GetValue(DataItem dataItem, object value)
        //{
        //    if (value == null)
        //    {
        //        if (isString())
        //            value = (Object)String.Empty;

        //    }
        //    return value;
        //}

        public DateTime GetValue(DataItem dataItem, DateTime value)
        {
            value = DateTime.MinValue;
            try
            {
                if (isDateTime())
                    value = (DateTime)FieldPropertyInfo.GetValue(dataItem, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }


        public int GetValue(DataItem dataItem, int value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (int)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToInt32(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public short GetValue(DataItem dataItem, short value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (short)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToInt16(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public sbyte GetValue(DataItem dataItem, sbyte value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (sbyte)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToSByte(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public long GetValue(DataItem dataItem, long value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (long)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToInt64(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public byte GetValue(DataItem dataItem, byte value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (byte)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToByte(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public ushort GetValue(DataItem dataItem, ushort value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (ushort)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToUInt16(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public uint GetValue(DataItem dataItem, uint value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (uint)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToUInt32(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public ulong GetValue(DataItem dataItem, ulong value)
        {
            value = 0;
            try
            {
                if (isNumberOrChar())
                    value = (ulong)FieldPropertyInfo.GetValue(dataItem, null);
                if (isBool())
                {
                    if ((bool)FieldPropertyInfo.GetValue(dataItem, null))
                        value = 1;
                    else
                        value = 0;
                }
                if (isString())
                    value = Convert.ToUInt64(FieldPropertyInfo.GetValue(dataItem, null));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in GetValue() " + ex.Message);
            }
            return value;
        }

        public bool GetValue(DataItem dataItem, bool value)
        {
            value = false;
            if (isNumberOrChar())
            {
                int i = 0;
                i = GetValue(dataItem, i);
                value = i > 0;
            }
            if (isBool())
                value = (bool)FieldPropertyInfo.GetValue(dataItem, null);
            if (isString())
            {
                string s = (string)FieldPropertyInfo.GetValue(dataItem, null);
                s = s.ToLower().Trim();
                value = ((s == "true") || (s == "on") || (s == "yes"));
            }
            return value;
        }

        public string GetValue(DataItem dataItem, string value)
        {
            value = string.Empty;
            if (isNumberOrChar())
            {
                int i = 0;
                i = GetValue(dataItem, i);
                value = i.ToString();
            }
            if (isBool())
            {
                value = "False";
                bool b = (bool)FieldPropertyInfo.GetValue(dataItem, null);
                if (b) value = "True";
            }
            if (isString())
            {
                value = (string)FieldPropertyInfo.GetValue(dataItem, null);
            }
            return value;
        }

    }
 
    #endregion

    #region database connection classes

    [XmlRoot("DBConnections")]
    //[XmlInclude(typeof(DBConnections)), XmlInclude(typeof(Operators))]
    public class DBConnections : DataList
    {
        #region Constructor / Create Item
        public DBConnections()
        {
            ListType = typeof(DBConnection);
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        #endregion

        #region specific finds

        public DBConnection GetBySiteDB(string sitename, string dbname)
        {
            int i = 0;
            bool test = false;
            DBConnection dbConnection = null;
            while (!test && i < this.Count)
            {
                DBConnection dataItem = (DBConnection)this[i];
                test = ((dataItem.SiteName.ToLower() == sitename.ToLower())
                    && (dataItem.DBName.ToLower() == dbname.ToLower()));
                if (test)
                    dbConnection = dataItem;
                i++;
            }
            return dbConnection;
        }

        public DBConnection GetBySite(string sitename)
        {
            int i = 0;
            bool test = false;
            DBConnection dbConnection = null;
            while (!test && i < this.Count)
            {
                DBConnection dataItem = (DBConnection)this[i];
                test = (dataItem.SiteName.ToLower() == sitename.ToLower());
                if (test)
                    dbConnection = dataItem;
                i++;
            }
            return dbConnection;
        }

        #endregion
    }

    [XmlInclude(typeof(DBConnections)), XmlInclude(typeof(Operators))]
    public class DBConnection : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private string siteName = string.Empty;
        private string dBName = string.Empty;
        private string connectionString = string.Empty;

        #endregion

        #region properties

        public string SiteName
        {
            get { return siteName; }
            set { siteName = AssignNotify(ref siteName, value, "SiteName"); }
        }

        public string DBName
        {
            get { return dBName; }
            set { dBName = AssignNotify(ref dBName, value, "DBName"); }
        }

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = AssignNotify(ref connectionString, value, "ConnectionString"); }
        }

        #endregion

    }

    #endregion

    #region cache management classes

    public class CacheItems : List<CacheItem>
    {

        public CacheItems()
        {
        }

        public DataList GetDataList(String datalistname)
        {
            DataList dl = null;
            CacheItem cacheItem = getByName(datalistname);
            if (cacheItem != null)
                dl = cacheItem.CacheDataList;
            return dl;
        }

        public DataItem GetDataItem(String datalistname, int id)
        {
            DataItem di = null;
            DataList dl = GetDataList(datalistname);
            if (dl != null)
            {
                di = dl.GetById(id);
            }
            return di;
        }

        private CacheItem getByType(Type type)
        {
            return this.Find(delegate(CacheItem item)
            {
                return (item.CacheType == type)
                    && (item.IsRestricted == false);
            });
        }

        private CacheItem getByName(String listname)
        {
            return this.Find(delegate(CacheItem item)
            {
                return ((item.ListName == listname)
                    && (item.IsValid));
            });
        }

        private CacheItem getByType(Type type, String dbname)
        {
            return this.Find(delegate(CacheItem item)
            {
                return ((item.CacheType == type)
                    && (item.DbName == dbname)
                    && (item.IsValid));
            });
        }

        public void RemoveInvalidCaches()
        {
            try
            {
                CacheItems deleteCIs = new CacheItems();
                int count = this.Count;
                for (int i = 0; i < count; i++)
                {
                    CacheItem cacheItem = this[i];
                    if ((cacheItem != null) && (!cacheItem.IsValid))
                    {
                        if (cacheItem.CanDelete)
                        {
                            deleteCIs.Add(cacheItem);
                        }
                    }
                    count = this.Count;
                }

                foreach (CacheItem cacheItem in deleteCIs)
                {
                    this.Remove(cacheItem);
                    cacheItem.Dispose();
                    Debug.WriteLine("RemoveInvalidCaches: CacheItem Disposed - " + cacheItem.CacheDataList.TblName);
                }
                deleteCIs.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + " " + ex.Message);
            }
            
        }

        public void MaintainCaches()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    CacheItem cacheItem = this[i];
                    {
                        if (!cacheItem.IsValid)
                        {
                            cacheItem.CacheDataList = da.DBDataListSelect(cacheItem.CacheDataList, true, false);
                            cacheItem.CacheDataList.Reset();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("CacheItems.MaintainCaches " + ex.Message);
                }
            }
        }

        public void InvalidateCache(String listname)
        {
            CacheItem cacheItem = getByName(listname);
            if (cacheItem != null)
            {
                this.Remove(cacheItem);
            }
        }

        public void InvalidateCache(DataList dataList)
        {
            CacheItem cacheItem = getByType(dataList.GetType(), dataList.DbName);
            if (cacheItem != null)
            {
                this.Remove(cacheItem);
            }
        }

        public void InvalidateCaches()
        {
            this.Clear();
        }

        public void AddDataList(DataList dataList)
        {
            CacheItem cacheItem = new CacheItem(dataList);
            this.Add(cacheItem);
            //should remove all other instances of this data in cache
        }

        public void AddDataList(DataList datalist, Type cachetype, String dbname, String listname, Boolean directfromdb, 
            Boolean maintainupdate, int updatepriority, bool candelete)
        {
            CacheItem cacheItem = new CacheItem(datalist, cachetype, dbname, listname, directfromdb, maintainupdate, updatepriority, candelete);
            this.Add(cacheItem);
        }

        public bool GetDataList_old(ref DataList dataList)
        {
            bool gotList = false;
            CacheItem cacheItem = getByType(dataList.GetType(), dataList.DbName);
            if (cacheItem != null)
            {
                if (cacheItem.IsValid)
                {
                    dataList = cacheItem.CacheDataList;
                    gotList = true;
                }
                else
                {
                    this.Remove(cacheItem);
                    cacheItem.Dispose();
                }
            }
            return gotList;
        }

        private DataList copyDataList(DataList source, DataList dest)
        {
            try
            {
                if ((source != null) && (dest != null))
                {
                    dest.Clear();
                    foreach (DataItem item in source)
                    {
                        dest.Add(item);
                    }
                    dest.SelectResult = true;
                    dest.UpdatedFromDB = false;
                }
            }
            catch (Exception ex)
            {
                dest.SelectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (dest.RaiseException)
                { throw ex; }
            }
            return dest;
        }

        public bool GetDataList(ref DataList dataList)
        {
            bool gotList = false;
            try
            {
                CacheItem cacheItem = getByType(dataList.GetType(), dataList.DbName);
                if (cacheItem != null)
                {
                    if (cacheItem.IsValid && cacheItem.CacheDataList.Count > 0)
                    {
                        dataList = copyDataList(cacheItem.CacheDataList, dataList);
                        gotList = true;
                    }
                    else
                    {
                        this.Remove(cacheItem);
                        cacheItem.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                dataList.SelectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (dataList.RaiseException)
                { throw ex; }
            }

            return gotList;
        }
    
    }

    public class CacheItem : IDisposable
    {
        private DataList cacheDataList;
        private Boolean disposed;

        public Boolean IsValid
        {
            get
            {
                return !disposed
                    && cacheDataList != null
                    && cacheDataList.IsValid;
            }
            //set
            //{
            //    if (!disposed && cacheDataList != null)
            //        cacheDataList.IsValid = false;
            //}
        }
        public DataList CacheDataList
        {
            get { return cacheDataList; }
            set { cacheDataList = value; }
        }
        private Type cacheType;
        public Type CacheType
        {
            get { return cacheType; }
            set { cacheType = value; }
        }
        private String dbName;
        public String DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }
        private String listName;
        public String ListName
        {
            get { return listName; }
            set { listName = value; }
        }
        private Boolean directFromDB;
        public Boolean DirectFromDB
        {
            get { return directFromDB; }
            set { directFromDB = value; }
        }
        private Boolean maintainUpdate;
        public Boolean MaintainUpdate
        {
            get { return maintainUpdate; }
            set { maintainUpdate = value; }
        }
        private int updatePriority;
        public int UpdatePriority
        {
            get { return updatePriority; }
            set { updatePriority = value; }
        } 
        public Boolean IsRestricted
        {
            get { return cacheDataList.IsRestricted; }
        }
        private Boolean canDelete = false;
        public Boolean CanDelete
        {
            get { return canDelete; }
            set { canDelete = value; }
        }

        public CacheItem(DataList datalist)
            : this(datalist, datalist.GetType(), datalist.DbName, datalist.GetType().ToString(), false, true, 5, true)
        { }

        public CacheItem(DataList datalist, Type cachetype, String dbname, String listname, Boolean directfromdb, 
            Boolean maintainupdate, int updatepriority, bool candelete)
        {
            cacheDataList = datalist;
            cacheType = cachetype;
            dbName = dbname;
            listName = listname;
            DirectFromDB = directfromdb;
            MaintainUpdate = maintainupdate;
            UpdatePriority = updatepriority;
            CanDelete = true;
            disposed = false;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(Boolean disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (cacheDataList != null)
                    {
                        cacheDataList.IsValid = false;
                        cacheDataList.Clear();
                    }
                }
                disposed = true;
            }
        }

        #endregion


    }

    #endregion

    public enum RestrictionType
    {
        rtInt,
        rtIntList,
        rtBool,
        rtDouble,
        rtDateTime,
        rtString,
        rtNull,
        rtDBField
    }

    public enum ComparisonType
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanEqual,
        MoreThan,
        MoreThanEqual,
        MoreThanALessThanB,
        MoreThanEqualALessThanB,
        MoreThanALessThanEqualB,
        MoreThanEqualALessThanEqualB,
        InList,
        NotInList,
        CaseIndependantEqual,
        StringContainsField,
        FieldContainsString,
        StringStartingWith,
        BoolOR,
        BoolNOR,
        BoolXOR,
        BoolXNOR
    }

    public enum PresetRestriction
    {
        Today,
        ThisWeek,
        ThisMonth,
        ThisYear,
        Yesterday,
        LastWeek
    }

    public class RestrictionRecs : List<RestrictionRec>
    { }
    
    public class RestrictionRec
    {
        #region private fields
        private string fieldName;
        private Int64 intA;
        private Int64 intB;
        private List<int> intList;
        private bool boolA;
        private bool boolB;
        private double doubleA;
        private double doubleB;
        private DateTime dateTimeA;
        private DateTime dateTimeB;
        private string stringA;
        private string stringB;

        private RestrictionType restrictionType;
        private ComparisonType comparisonType;
        
        #endregion

        #region Constructors

        #region dbnull/dbfield
        public RestrictionRec(string fieldname)
            : this(fieldname, RestrictionType.rtNull, ComparisonType.Equal)
        { }

        public RestrictionRec(string fieldname, RestrictionType restrictiontype, ComparisonType comparisontype)
        {
            fieldName = fieldname;
            restrictionType = restrictiontype;
            comparisonType = comparisontype;
        }

        #endregion

        #region bools
        public RestrictionRec(string fieldname, bool boola)
            : this(fieldname, RestrictionType.rtBool, ComparisonType.Equal)
        {
            boolA = boola;
        }

        public RestrictionRec(string fieldname, bool boola, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtBool, comparisontype)
        {
            boolA = boola;
        }

        public RestrictionRec(string fieldname, bool boola, bool boolb)
            : this(fieldname, RestrictionType.rtBool, ComparisonType.MoreThanEqualALessThanB)
        {
            boolA = boola;
            boolB = boolb;
        }

        public RestrictionRec(string fieldname, bool boola, bool boolb, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtBool, comparisontype)
        {
            boolA = boola;
            boolB = boolb;
        }

        #endregion

        #region ints

        public RestrictionRec(string fieldname, Int64 inta)
            : this(fieldname, RestrictionType.rtInt, ComparisonType.Equal)
        {
            intA = inta;
        }

        public RestrictionRec(string fieldname, Int64 inta, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtInt, comparisontype)
        {
            intA = inta;
        }

        public RestrictionRec(string fieldname, Int64 inta, Int64 intb)
            : this(fieldname, RestrictionType.rtInt, ComparisonType.MoreThanEqualALessThanB)
        {
            intA = inta;
            intB = intb;
        }

        public RestrictionRec(string fieldname, Int64 inta, Int64 intb, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtInt, comparisontype)
        {
            intA = inta;
            intB = intb;
        }

        public RestrictionRec(string fieldname, List<int> intlist, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtIntList, comparisontype)
        {
            intList = intlist;
        }

        public RestrictionRec(string fieldname, List<int> intlist)
            : this(fieldname, RestrictionType.rtIntList, ComparisonType.InList)
        {
            intList = intlist;
        }

        #endregion

        #region doubles

        public RestrictionRec(string fieldname, double doublea)
            : this(fieldname, RestrictionType.rtDouble, ComparisonType.Equal)
        {
            doubleA = doublea;
        }

        public RestrictionRec(string fieldname, double doublea, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtDouble, comparisontype)
        {
            doubleA = doublea;
        }

        public RestrictionRec(string fieldname, double doublea, double doubleb)
            : this(fieldname, RestrictionType.rtDouble, ComparisonType.MoreThanEqualALessThanB)
        {
            doubleA = doublea;
            doubleB = doubleb;
        }

        public RestrictionRec(string fieldname, double doublea, double doubleb, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtDouble, comparisontype)
        {
            doubleA = doublea;
            doubleB = doubleb;
        }

        #endregion

        #region datetimes

        public RestrictionRec(string fieldname, DateTime datetimea)
            : this(fieldname, RestrictionType.rtDateTime, ComparisonType.Equal)
        {
            dateTimeA = datetimea;
        }

        public RestrictionRec(string fieldname, PresetRestriction presetRestriction)
            : this(fieldname,DateTime.Today,presetRestriction)
        { }

        public RestrictionRec(string fieldname, DateTime datatimea, PresetRestriction presetRestriction)
            : this(fieldname)
        {
            restrictionType = RestrictionType.rtDateTime;
             if (presetRestriction == PresetRestriction.Today)
            {
                dateTimeA = datatimea.Date;
                dateTimeB = dateTimeA.AddDays(1);
                comparisonType = ComparisonType.MoreThanEqualALessThanB;
            }
            if (presetRestriction == PresetRestriction.Yesterday)
            {
                dateTimeA = datatimea.Date.AddDays(-1);
                dateTimeB = datatimea.Date;
                comparisonType = ComparisonType.MoreThanEqualALessThanB;
            }     
            //last week, etc
        }

        public RestrictionRec(string fieldname, DateTime datetimea, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtDateTime, comparisontype)
        {
            dateTimeA = datetimea;
        }

        public RestrictionRec(string fieldname, DateTime datetimea, DateTime datetimeb)
            : this(fieldname, RestrictionType.rtDateTime, ComparisonType.MoreThanEqualALessThanB)
        {
            dateTimeA = datetimea;
            dateTimeB = datetimeb;
        }

        public RestrictionRec(string fieldname, DateTime datetimea, DateTime datetimeb, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtDateTime, comparisontype)
        {
            dateTimeA = datetimea;
            dateTimeB = datetimeb;
        }

        public RestrictionRec(string fieldname, DateTime datetime, string customType)
            : this(fieldname, datetime, ComparisonType.MoreThanEqualALessThanB)
        {
            fieldName = fieldname;
            if (customType.ToLower() == "thisday")
            {
                dateTimeA = datetime.Date;
                dateTimeB = datetime.Date.AddDays(1);
            }

            if (customType.ToLower() == "thishour")
            {
                dateTimeA = new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0);
                dateTimeB = datetime.Date.AddHours(1);
            }
        }

        #endregion

        #region strings
        public RestrictionRec(string fieldname, string stringa)
            : this(fieldname, RestrictionType.rtString, ComparisonType.Equal)
        {
            stringA = stringa;
        }

        public RestrictionRec(string fieldname, string stringa, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtString, comparisontype)
        {
            stringA = stringa;
        }

        public RestrictionRec(string fieldname, string stringa, string stringb)
            : this(fieldname, RestrictionType.rtString, ComparisonType.MoreThanEqualALessThanB)
        {
            stringA = stringa;
            stringB = stringb;
        }

        public RestrictionRec(string fieldname, string stringa, string stringb, ComparisonType comparisontype)
            : this(fieldname, RestrictionType.rtString, comparisontype)
        {
            stringA = stringa;
            stringB = stringb;
        }

        #endregion
        
        #endregion

        public string ElementSQL
        {
            get { return getElementSQL(); }
        }

        private string getElementSQL()
        {
            string sql = string.Empty;
            switch (restrictionType)
            {
                case RestrictionType.rtNull:
                    sql = dbNullSQL();
                    break;
                case RestrictionType.rtInt:
                    sql = intSQL();
                    break;
                case RestrictionType.rtIntList:
                    sql = intListSQL();
                    break;
                case RestrictionType.rtDateTime:
                    sql = datetimeSQL();
                    break;
                case RestrictionType.rtString:
                    sql = stringSQL();
                    break;
                case RestrictionType.rtDouble:
                    sql = doubleSQL();
                    break;
                case RestrictionType.rtDBField:
                    sql = dbfieldSQL();
                    break;
                default: 
                    sql = string.Empty;
                    break;
            }
            return sql;
        }

        private string dbNullSQL()
        {
            string sql = string.Empty;
            return sql;
        }

        private string dbfieldSQL()
        {
            string sql = string.Empty;
            return sql;
        }

        private string intSQL()
        {
            string sql = string.Empty; 
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    sql = " " + fieldName + " = " + intA.ToString() + " "; 
                    break;
                case ComparisonType.NotEqual:
                    sql = " " + fieldName + " != " + intA.ToString() + " ";
                    break;
                case ComparisonType.MoreThan:
                    sql = " " + fieldName + " > " + intA.ToString() + " ";
                    break;
                case ComparisonType.MoreThanEqual:
                    sql = " " + fieldName + " >= " + intA.ToString() + " ";
                    break;
                case ComparisonType.LessThan:
                    sql = " " + fieldName + " < " + intA.ToString() + " ";
                    break;
                case ComparisonType.LessThanEqual:
                    sql = " " + fieldName + " <= " + intA.ToString() + " ";
                    break;
                case ComparisonType.MoreThanALessThanB:
                    sql = " (" + fieldName + " > " + intA.ToString() + " AND "
                        + fieldName + " < " + intB.ToString() + ") ";
                    break;
                case ComparisonType.MoreThanEqualALessThanB:
                    sql = " (" + fieldName + " >= " + intA.ToString() + " AND "
                        + fieldName + " < " + intB.ToString() + ") ";
                    break;
                case ComparisonType.MoreThanALessThanEqualB:
                    sql = " (" + fieldName + " > " + intA.ToString() + " AND "
                        + fieldName + " <= " + intB.ToString() + ") ";
                    break;
                case ComparisonType.MoreThanEqualALessThanEqualB:
                    sql = " (" + fieldName + " >= " + intA.ToString() + " AND "
                        + fieldName + " <= " + intB.ToString() + ") ";
                    break;
                default:
                    sql = string.Empty;
                    break;            
            }
            return sql;
        }

        private string bracketIntArrayStr()
        {
            string arraystr = "(";
            for (int i = 0; i < intList.Count; i++)
            {
                arraystr += " " + intList[i].ToString();
                if (i != intList.Count - 1)
                    arraystr += ",";
            }
            arraystr += ")";
            return arraystr;
        }

        private string intListSQL()
        {
            string sql = string.Empty;
            switch (comparisonType)
            {
                case ComparisonType.InList:
                    sql = " " + fieldName + " IN " + bracketIntArrayStr() + " ";
                    break;
                case ComparisonType.NotInList:
                    sql = " " + fieldName + " NOT IN " + bracketIntArrayStr() + " ";
                    break;
            }
            return sql;
        }

        private string doubleSQL()
        {
            string sql = string.Empty;
            string fmt = "0.0000";
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    sql = " " + fieldName + " = " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.NotEqual:
                    sql = " " + fieldName + " != " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.MoreThan:
                    sql = " " + fieldName + " > " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.MoreThanEqual:
                    sql = " " + fieldName + " >= " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.LessThan:
                    sql = " " + fieldName + " < " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.LessThanEqual:
                    sql = " " + fieldName + " <= " + doubleA.ToString(fmt) + " ";
                    break;
                case ComparisonType.MoreThanALessThanB:
                    sql = " (" + fieldName + " > " + doubleA.ToString(fmt) + " AND "
                        + fieldName + " < " + doubleB.ToString(fmt) + ") ";
                    break;
                case ComparisonType.MoreThanEqualALessThanB:
                    sql = " (" + fieldName + " >= " + doubleA.ToString(fmt) + " AND "
                        + fieldName + " < " + doubleB.ToString(fmt) + ") ";
                    break;
                case ComparisonType.MoreThanALessThanEqualB:
                    sql = " (" + fieldName + " > " + doubleA.ToString(fmt) + " AND "
                        + fieldName + " <= " + doubleB.ToString(fmt) + ") ";
                    break;
                case ComparisonType.MoreThanEqualALessThanEqualB:
                    sql = " (" + fieldName + " >= " + doubleA.ToString(fmt) + " AND "
                        + fieldName + " <= " + doubleB.ToString(fmt) + ") ";
                    break;
                default:
                    sql = string.Empty;
                    break;
            }
            return sql;
        }

        private string stringSQL()
        {
            string sql = string.Empty;
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    sql = " " + fieldName + " = '" + stringA + "' ";
                    break;
                case ComparisonType.NotEqual:
                    sql = " " + fieldName + " != '" + stringA + "' ";
                    break;
                default:
                    sql = string.Empty;
                    break;
            }
            return sql;
        }

        private string datetimeSQL()
        {
            string sql = string.Empty; // may want to consider different formatting
            string fmt = "yyyyMMdd HH:mm:ss";
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    sql = " " + fieldName + " = '" + dateTimeA.ToString(fmt) + "' "; //nb practically useless!
                    break;
                case ComparisonType.NotEqual:
                    sql = " " + fieldName + " != '" + dateTimeA.ToString(fmt) + "' ";//nb practically useless!
                    break;
                case ComparisonType.MoreThan:
                    sql = " " + fieldName + " > '" + dateTimeA.ToString(fmt) + "' ";
                    break;
                case ComparisonType.MoreThanEqual:
                    sql = " " + fieldName + " >= '" + dateTimeA.ToString(fmt) + "' ";
                    break;
                case ComparisonType.LessThan:
                    sql = " " + fieldName + " < '" + dateTimeA.ToString(fmt) + "' ";
                    break;
                case ComparisonType.LessThanEqual:
                    sql = " " + fieldName + " <= '" + dateTimeA.ToString(fmt) + "' ";
                    break;
                case ComparisonType.MoreThanALessThanB:
                    sql = " (" + fieldName + " > '" + dateTimeA.ToString(fmt) + "' AND "
                        + fieldName + " < '" + dateTimeB.ToString(fmt) + "') ";
                    break;
                case ComparisonType.MoreThanEqualALessThanB:
                    sql = " (" + fieldName + " >= '" + dateTimeA.ToString(fmt) + "' AND " //most useful
                        + fieldName + " < '" + dateTimeB.ToString(fmt) + "') ";
                    break;
                case ComparisonType.MoreThanALessThanEqualB:
                    sql = " (" + fieldName + " > '" + dateTimeA.ToString(fmt) + "' AND "
                        + fieldName + " <= '" + dateTimeB.ToString(fmt) + "') ";
                    break;
                case ComparisonType.MoreThanEqualALessThanEqualB:
                    sql = " (" + fieldName + " >= '" + dateTimeA.ToString(fmt) + "' AND "
                        + fieldName + " <= '" + dateTimeB.ToString(fmt) + "') ";
                    break;
                default:
                    sql = string.Empty;
                    break;            
            }
            return sql;
        }
    }

    #region table updates


    public class LastTableUpdates : DataList //refactor for multiple databases
    {
        #region Constructor / Create Item

        public LastTableUpdates()
        {
            Lifespan = 2.0 / 3600.0; //2 seconds
            DbName = "JEGR_DB";
            TblName = "fnLastTblUpdate()";
            DefaultSqlSelectCommand = @"SELECT TableName, LastUpdate 
                                      FROM "+DbName+".[dbo].[fnLastTblUpdate]()";
            IsReadOnly = true;
            LastTableUpdate dataItem = (LastTableUpdate)NewItem();
            Type t = dataItem.GetType();
            PropertyInfo[] pInfos = t.GetProperties();
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("TableName", "TableName");
            dBFieldMappings.AddMapping("LastUpdate", "LastUpdated");
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public LastTableUpdates(string dbname)
        {
            Lifespan = 2.0 / 3600.0; //2 seconds
            IsReadOnly = true;
            if (DbName != string.Empty)
                DbName = dbname;
            else
                DbName = "JEGR_DB";
            DefaultSqlSelectCommand = @"SELECT TableName, LastUpdate 
                                      FROM " + DbName + ".[dbo].[fnLastTblUpdate]()";
            LastTableUpdate dataItem = (LastTableUpdate)NewItem();
            Type t = dataItem.GetType();
            PropertyInfo[] pInfos = t.GetProperties();
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("TableName", "TableName");
            dBFieldMappings.AddMapping("LastUpdate", "LastUpdated");
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public override DataItem NewItem()
        {
            LastTableUpdate dataItem = new LastTableUpdate();
            return dataItem;
        }

        #endregion

        public override bool IsValid // overrides base that checks for table updates!
        {
            get
            {
                bool test = isValid && (this.Count >= 0) && (lastRead != null) && (AllowExpire);
                if (test)
                {
                    DateTime testTime = lastRead.AddHours(Lifespan);
                    test = testTime > DateTime.Now; //must ignore server time
                }
                return test || !AllowExpire;
            }
            set
            { isValid = value; }
        }

    }

    public class LastTableUpdate : DataItem
    {
        #region private fields

        private String tableName = string.Empty;
        private DateTime lastUpdated;

        #endregion

        #region properties

        public String TableName
        {
            get { return tableName; }
            set
            {
                tableName = AssignNotify(ref tableName, value, "TableName");
                ItemName = tableName;
            }
        }

        public DateTime LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = AssignNotify(ref lastUpdated, value, "LastUpdated"); }
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

    public interface ICopyable
    {
        ICopyable Copy();
    }
    #endregion

}
