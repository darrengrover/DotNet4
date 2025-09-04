using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.LoggingDataLayer
{
    public class DbConfigurationXml
    {
        const string ConfigFilename = "DatabaseConfig.xml";

        [XmlIgnore]
        public bool ConfigurationChanged { get; set; }

        [XmlIgnore]
        public bool IsNewConfig { get; set; }

        [XmlElement("ConfigurationEntry")]
        public List<DbConfigurationEntry> Entries = new List<DbConfigurationEntry>();

        public void Add(DbConfigurationEntry entry)
        {
            Entries.Add(entry);
        }

        public DbConfigurationEntry Find(string key)
        {
            return Entries.Find(
              delegate(DbConfigurationEntry entry)
              {
                  return entry.Name == key;
              });
        }

        #region Read Configuration
        /// <summary>Read the configuration.</summary>
        /// <returns>The DbConfiguration</returns>
        public static DbConfigurationXml Read()
        {
            string fullpath = ConfigFilename;
            DbConfigurationXml configuration = LoadLocal(fullpath);

            foreach (DbConfigurationEntry entry in configuration.Entries)
            {
                if (entry.IsEncrypted)
                {
                    // Decrypt GemacConnectionString

                    // Decrypt JegrConnectionString

                    // Decrypt PublicConnectionString
                }
                else
                {
                    entry.GemacConnectionString = entry.GemacConnectionStringXml;
                    entry.JegrConnectionString = entry.JegrConnectionStringXml;
                    entry.PublicConnectionString = entry.PublicConnectionStringXml;
                }
            }

            return configuration;
        }

        private static DbConfigurationXml LoadLocal(string fullpath)
        {
            try
            {
                XmlRootAttribute root = new XmlRootAttribute("DbConfiguration");
                XmlSerializer mySerializer = new XmlSerializer(typeof(DbConfigurationXml), root);
                FileStream myFileStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                DbConfigurationXml diagram = (DbConfigurationXml)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                return diagram;
            }
            catch (IOException)
            {
                return new DbConfigurationXml()
                {
                    IsNewConfig = true
                };
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

        #region Write Configuration
        /// <summary>Write the configuration.</summary>
        public void Write()
        {
            string fullpath = ConfigFilename;

            string bakfilepath = Path.ChangeExtension(fullpath, ".bak");
            if (File.Exists(fullpath))
            {
                File.Copy(fullpath, bakfilepath, true);
            }

            foreach (DbConfigurationEntry entry in this.Entries)
            {
                if (entry.IsEncrypted)
                {
                    // Encrypt GemacConnectionString

                    // Encrypt JegrConnectionString

                    // Encrypt PublicConnectionString

                }
                else
                {
                    entry.GemacConnectionStringXml = entry.GemacConnectionString;
                    entry.JegrConnectionStringXml = entry.JegrConnectionString;
                    entry.PublicConnectionStringXml = entry.PublicConnectionString;
                }
            }

            SaveLocal(this, fullpath);

            ConfigurationChanged = false;
        }

        private static void SaveLocal(DbConfigurationXml config, string fullpath)
        {
            XmlRootAttribute root = new XmlRootAttribute("DbConfiguration");
            XmlSerializer mySerializer = new XmlSerializer(typeof(DbConfigurationXml), root);
            StreamWriter myWriter = new StreamWriter(fullpath);
            mySerializer.Serialize(myWriter, config);
            myWriter.Close();
        }
        #endregion
    }

    [XmlType("ConfigurationEntry")]
    public class DbConfigurationEntry
    {
        [XmlAttribute("Encrypted")]
        public bool IsEncrypted { get; set; }

        /// <summary>ConnectionString for the Ge-Mac Database.</summary>
        [XmlIgnore]
        public string GemacConnectionString { get; set; }

        /// <summary>ConnectionString for the Jensen Database.</summary>
        [XmlIgnore]
        public string JegrConnectionString { get; set; }

        /// <summary>ConnectionString for the Jensen Public Database.</summary>
        [XmlIgnore]
        public string PublicConnectionString { get; set; }

        /// <summary>ConnectionString for the Ge-Mac Database.</summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>ConnectionString for the Ge-Mac Database.</summary>
        [XmlAttribute("GemacConnectionString")]
        public string GemacConnectionStringXml { get; set; }

        /// <summary>ConnectionString for the Jensen Database.</summary>
        [XmlAttribute("JegrConnectionString")]
        public string JegrConnectionStringXml { get; set; }

        /// <summary>ConnectionString for the Jensen Public Database.</summary>
        [XmlAttribute("PublicConnectionString")]
        public string PublicConnectionStringXml { get; set; }
    }
}
