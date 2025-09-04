using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Ge_Mac.LoggingDataLayer
{
    public class SqlDataConnection
    {
        public static int Timeout = 30;

        private static DbConfigurationXml xmlConfig = null;
        private static string RailConnectionString = null;

        private static SqlConnection RailConnection;

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

        private static void GetDbConfiguration(string key)
        {
            if (xmlConfig == null)
            {
                xmlConfig = DbConfigurationXml.Read();
                if (!xmlConfig.IsNewConfig)
                {
                    DbConfigurationEntry entry = xmlConfig.Find(key);
                    RailConnectionString = entry.GemacConnectionString;
                }
                else
                {
                    // error - no database configuration!
                }
            }
        }

        public static SqlConnection GetConnection(DBConnection dBConnection)
        {
            GetDbConfiguration(ConfigName);

            switch (dBConnection)
            {
                case DBConnection.Rail:
                    if (RailConnection == null)
                    {
                        RailConnection = new SqlConnection(RailConnectionString);
                    }
                    return RailConnection;
                default:
                    return null;
            }
        }
        #endregion
    }
}
