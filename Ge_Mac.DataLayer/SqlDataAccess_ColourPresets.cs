using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
       
        #region Select Data
        const string allColourPresetsCommand =
            @"SELECT [ColourPresetID]
	              ,[PresetName]
	              ,[ForeColour]
	              ,[BackColour]
              FROM [dbo].[tblColourPresets] 
              ORDER BY ColourPresetID";

        public ColourPresets GetAllColourPresets()
        {
            ColourPresets colourPresets = null;
            if (DatabaseVersion >= 1.3)
            {
                try
                {
                    const string commandString = allColourPresetsCommand;

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        colourPresets = new ColourPresets();
                        command.DataFill(colourPresets, SqlDataConnection.DBConnection.JensenGroup);
                        return colourPresets;
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
            return colourPresets;
        }

        #endregion

    }


    #region Data Collection Classes
    public class ColourPresets : List<ColourPreset>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblColourPresets";
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
            int ColourPresetIDPos = dr.GetOrdinal("ColourPresetID");
            int PresetNamePos = dr.GetOrdinal("PresetName");
            int ForeColourPos = dr.GetOrdinal("ForeColour");
            int BackColourPos = dr.GetOrdinal("BackColour");

            this.Clear();
            while (dr.Read())
            {
                ColourPreset colourPreset = new ColourPreset()
                {
                    ColourPresetID = dr.GetInt32(ColourPresetIDPos),
                    PresetName = dr.GetString(PresetNamePos),
                    ForeColour = dr.GetInt32(ForeColourPos),
                    BackColour = dr.GetInt32(BackColourPos),
                    HasChanged = false
                };

                this.Add(colourPreset);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            neverExpire = false;
            IsValid = true;

            return this.Count;
        }

        public ColourPreset GetById(int id)
        {
            return this.Find(colourPreset => colourPreset.ColourPresetID == id);
        }

        public ColourPreset GetByName(string aName)
        {
            return this.Find(colourPreset => colourPreset.PresetName == aName);
        }

    }
    #endregion

    #region Item Classes
 
    public class ColourPreset : DataItem
    {
        #region ColourPreset Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int ColourPresetID;
            internal string PresetName;
            internal int ForeColour;
            internal int BackColour;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ColourPreset()
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
                return ColourPresetID;
            }
            set
            {
                if (ColourPresetID != value)
                {
                    ColourPresetID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        #endregion

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return this.activeData.ColourPresetID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.ColourPresetID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int ColourPresetID
        {
            get
            {
                return this.activeData.ColourPresetID;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.ColourPresetID != value;
                this.activeData.ColourPresetID = value;
            }
        }

        public string PresetName
        {
            get
            {
                return this.activeData.PresetName;
            }
            set
            {
                HasChanged = HasChanged || this.activeData.PresetName != value;
                this.activeData.PresetName = value;
            }
        }

        public int BackColour
        {
            get
            {
                return this.activeData.BackColour;
            }
            set
            {
                this.activeData.BackColour = value;
                HasChanged = true;
            }
        }

        public int ForeColour
        {
            get
            {
                return this.activeData.ForeColour;
            }
            set
            {
                this.activeData.ForeColour = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
