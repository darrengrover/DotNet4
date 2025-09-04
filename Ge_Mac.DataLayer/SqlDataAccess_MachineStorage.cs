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
        private MachinesStorage machinesStorageCache = null;

        public void InvalidateMachinesStorage()
        {
            if (machinesStorageCache != null)
                machinesStorageCache.IsValid = false;
        }

        private bool MachinesStorageIsCached()
        {
            bool test = (machinesStorageCache != null);
            if (test)
            {
                test = machinesStorageCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string getMachineStorageCommand =
            @"SELECT TOP 1
                ld.recnum,[TimeStamp], MachineID, Value, Unit  
                FROM tbljglogdata ld,tblMachineGroups mg,tblMachines m
                WHERE ld.RegType= 7 
                 AND ld.Subregtype = 100
                 AND ld.SubRegTypeID = 1
                 AND ld.State=0
                 AND ld.MachineID=m.idJensen
                 AND m.MachineGroup_idJensen=mg.idJensen
                 AND mg.MachineArea='GT'
                 
                ORDER BY ld.recNum desc";

        const string getMachineStorageFunctionCommand =
            @"Select Recnum,[TimeStamp], MachineID, Value, Unit  
                FROM dbo.fnGetGTStorage() ";

        public MachinesStorage GetMachinesStorage() 
        {
            if (MachinesStorageIsCached())
            {
                return machinesStorageCache;
            }
            try
            {
                const string commandString = getMachineStorageFunctionCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (machinesStorageCache == null) machinesStorageCache = new MachinesStorage();
                    command.DataFill(machinesStorageCache, SqlDataConnection.DBConnection.JensenGroup);
                    return machinesStorageCache;
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

        #endregion

    }


    #region Data Collection Class
    public class MachinesStorage : List<MachineStorage>, IDataFiller
    {
        private double lifespan = 1.0 / 60.0;
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
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    DateTime testTime = lastRead.AddHours(lifespan);
                    test = testTime > da.ServerTime;
                }
                return test;
            }
            set
            { isValid = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int TimeStampPos = dr.GetOrdinal("TimeStamp");
            int MachineIDPos = dr.GetOrdinal("machineID");
            int StoragePos = dr.GetOrdinal("value");
            int UnitPos = dr.GetOrdinal("unit");

            this.Clear();
            while (dr.Read())
            {
                MachineStorage machineStorage = new MachineStorage()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    Timestamp = dr.GetDateTime(TimeStampPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    Storage = dr.GetDecimal(StoragePos),
                    Unit = dr.GetInt32(UnitPos),
                    HasChanged = false
                };

                this.Add(machineStorage);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

    }
    #endregion

    #region Item Class
    public class MachineStorage : DataItem
    {
        #region MachineStorage Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal DateTime timestamp;
            internal int machineID;
            internal decimal storage;
            internal int unit;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public MachineStorage()
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
                return RecNum;
            }
            set
            {
                if (RecNum != value)
                {
                    RecNum = value;
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
                return this.activeData.RecNum == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.RecNum != -1;
            }
        }
        #endregion

        #region Data Column Properties
        public int RecNum
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                this.activeData.RecNum = value;
                HasChanged = true;
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return this.activeData.timestamp;
            }
            set
            {
                this.activeData.timestamp = value;
                HasChanged = true;
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.machineID;
            }
            set
            {
                this.activeData.machineID = value;
                HasChanged = true;
            }
        }

        public decimal Storage
        {
            get
            {
                return this.activeData.storage;
            }
            set
            {
                this.activeData.storage = value;
                HasChanged = true;
            }
        }

        public int Unit
        {
            get
            {
                return this.activeData.unit;
            }
            set
            {
                this.activeData.unit = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
