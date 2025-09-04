using System;
using System.Configuration;
using System.Data.SqlClient;
//using Ge_Mac.LoggingAndExceptions;
using System.Data;

namespace Ge_Mac.DataLayer
{
    public class SqlDataConnection
    {
        public static int Timeout = 30;

        //private static DbConfiguration config = null;
        private static DbConfigurationXml xmlConfig = null;

        public static DbConfigurationXml XmlConfig
        {
            get { return SqlDataConnection.xmlConfig; }
            set { SqlDataConnection.xmlConfig = value; }
        }
        private static string railConnectionString = null;
        public static string RailConnectionString
        {
            get { return SqlDataConnection.railConnectionString; }
        }
        private static string jEGRConnectionString = null;
        public static string JEGRConnectionString
        {
            get { return SqlDataConnection.jEGRConnectionString; }
        }
        private static string publicConnectionString = null;
        public static string PublicConnectionString
        {
            get { return SqlDataConnection.publicConnectionString; }
        }
        private static bool configSet = false;
        public static bool ConfigSet
        {
            get { return SqlDataConnection.configSet; }
            set { SqlDataConnection.configSet = value; }
        }


        private static string _ConfigName = "default";
        public static string ConfigName
        {
            get
            {
                return _ConfigName;
            }
            set
            {
                _ConfigName = value;
            }
        }

        #region Constructors
        public SqlDataConnection()
        {
            ConfigName = "default";
        }

        #endregion

        #region Connection Management
        public enum DBConnection
        {
            JensenGroup,
            JensenPublic,
            Rail
        }

        public static void SetDbConfiguration(string RailConnection, string JEGRConnection, string PublicConnection)
        {
            railConnectionString = RailConnection;
            jEGRConnectionString = JEGRConnection;
            publicConnectionString = PublicConnection;
            configSet = true;
        }

        public static void ReadDbConfiguration(string key)
        {
            if (!configSet)
            {
                if (xmlConfig == null)
                {
                    ConfigName = key;
                    xmlConfig = DbConfigurationXml.Read();
                    if (!xmlConfig.IsNewConfig)
                    {
                        DbConfigurationEntry entry = xmlConfig.Find(key);

                        if (entry != null)
                        {
                            railConnectionString = entry.GemacConnectionString;
                            jEGRConnectionString = entry.JegrConnectionString;
                            publicConnectionString = entry.PublicConnectionString;
                            configSet = true;
                        }
                        else
                        {
                            throw new Exception("Invalid Database connection key. Please ask for assistance.");
                        }
                    }
                    else
                    {
                        throw new Exception("Database Configuration could not be read. Please ask for assistance.");
                    }
                }
            }
        }

        public static void TestConnection(DBConnection dBConnection)
        {
            using (SqlConnection connection = GetConnection(dBConnection))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Failed to connect to the {0} database, Please ask for assistance.", dBConnection.ToString()));
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public static void ReConnect(string key)
        {
            ReadDbConfiguration(key);
            GetConnection(DBConnection.JensenGroup);
        }

        public static SqlConnection GetConnection(DBConnection dBConnection)
        {
            ReadDbConfiguration(ConfigName);

            switch (dBConnection)
            {
                case DBConnection.JensenGroup:
                    //if (JEGRConnection == null)
                    //{
                    //    JEGRConnection = new SqlConnection(JEGRConnectionString);
                    //}
                    //return JEGRConnection;

                    return new SqlConnection(jEGRConnectionString);
                case DBConnection.Rail:
                    //if (RailConnection == null)
                    //{
                    //    RailConnection = new SqlConnection(RailConnectionString);
                    //}
                    //return RailConnection;

                    return new SqlConnection(railConnectionString);
                case DBConnection.JensenPublic:
                    //if (PublicConnection == null)
                    //{
                    //    PublicConnection = new SqlConnection(PublicConnectionString);
                    //}
                    //return PublicConnection;

                    return new SqlConnection(publicConnectionString);
                default:
                    return null;
            }
        }
        #endregion
    }
}
