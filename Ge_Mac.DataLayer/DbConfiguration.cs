using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
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
                DbConfigurationXml dbConfigXml;
                FileStream myFileStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                dbConfigXml = (DbConfigurationXml)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();

                return dbConfigXml;
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

    //public class DbConfiguration : ConfigurationSection
    //{
    //    /// <summary>Gets the current DbConfiguration details</summary>
    //    /// <returns>The DbConfiguration details</returns>
    //    [Obsolete]
    //    public static DbConfiguration GetConfig()
    //    {
    //        DbConfiguration config = (DbConfiguration)
    //            ConfigurationManager.GetSection("dbConfigurationGroup/dbConfiguration");

    //        return config;
    //    }

    //    /// <summary>ConnectionString for the Ge-Mac Database.</summary>
    //    [Obsolete]
    //    [ConfigurationProperty("gemacConnectionString", DefaultValue = "", IsRequired = true)]
    //    public string GemacConnectionString
    //    {
    //        get
    //        {
    //            return (string)this["gemacConnectionString"];
    //        }
    //        set
    //        {
    //            this["gemacConnectionString"] = value;
    //        }
    //    }

    //    /// <summary>ConnectionString for the Jensen Database.</summary>
    //    [Obsolete]
    //    [ConfigurationProperty("jegrConnectionString", DefaultValue = "", IsRequired = true)]
    //    public string JegrConnectionString
    //    {
    //        get
    //        {
    //            return (string)this["jegrConnectionString"];
    //        }
    //        set
    //        {
    //            this["jegrConnectionString"] = value;
    //        }
    //    }

    //    /// <summary>ConnectionString for the Jensen Public Database.</summary>
    //    [Obsolete]
    //    [ConfigurationProperty("publicConnectionString", DefaultValue = "", IsRequired = true)]
    //    public string PublicConnectionString
    //    {
    //        get
    //        {
    //            return (string)this["publicConnectionString"];
    //        }
    //        set
    //        {
    //            this["publicConnectionString"] = value;
    //        }
    //    }

    //    #region Not Needed
    //    //// Create a "font" element.
    //    //[ConfigurationProperty("font")]
    //    //public FontElement Font
    //    //{
    //    //    get
    //    //    {
    //    //        return (FontElement)this["font"];
    //    //    }
    //    //    set
    //    //    { this["font"] = value; }
    //    //}

    //    //// Create a "color element."
    //    //[ConfigurationProperty("color")]
    //    //public ColorElement Color
    //    //{
    //    //    get
    //    //    {
    //    //        return (ColorElement)this["color"];
    //    //    }
    //    //    set
    //    //    { this["color"] = value; }
    //    //}
    //    #endregion
    //}

    #region Not Needed
    // Define the "font" element
    // with "name" and "size" attributes.
    public class FontElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "Arial", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("size", DefaultValue = "12", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 24, MinValue = 6)]
        public int Size
        {
            get
            { return (int)this["size"]; }
            set
            { this["size"] = value; }
        }
    }

    // Define the "color" element 
    // with "background" and "foreground" attributes.
    public class ColorElement : ConfigurationElement
    {
        [ConfigurationProperty("background", DefaultValue = "FFFFFF", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\GHIJKLMNOPQRSTUVWXYZ", MinLength = 6, MaxLength = 6)]
        public String Background
        {
            get
            {
                return (String)this["background"];
            }
            set
            {
                this["background"] = value;
            }
        }

        [ConfigurationProperty("foreground", DefaultValue = "000000", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\GHIJKLMNOPQRSTUVWXYZ", MinLength = 6, MaxLength = 6)]
        public String Foreground
        {
            get
            {
                return (String)this["foreground"];
            }
            set
            {
                this["foreground"] = value;
            }
        }
    }
    #endregion
}
