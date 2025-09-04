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
        private ProcessNames processNamesCache = null;

        public void InvalidateProcessNames()
        {
            if (processNamesCache != null)
                processNamesCache.IsValid = false;
        }

        private bool ProcessNamesAreCached()
        {
            bool test = (processNamesCache != null);
            if (test)
            {
                test = processNamesCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string getProcessNamesCommand =
            @"SELECT [Machine_idJensen]
                  ,[ProcessCode]
                  ,[ProcessName]
              FROM [tblProcessNames]
              ORDER BY Machine_idJensen, ProcessCode";

        public ProcessNames GetProcessNames() 
        {
            if (ProcessNamesAreCached())
            {
                return processNamesCache;
            }
            try
            {
                const string commandString = getProcessNamesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (processNamesCache == null) processNamesCache = new ProcessNames();
                    command.DataFill(processNamesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return processNamesCache;
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
    public class ProcessNames : List<ProcessName>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblProcessNames";
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
            int MachineIDPos = dr.GetOrdinal("Machine_idJensen");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int ProcessNamePos = dr.GetOrdinal("ProcessName");

            this.Clear();
            while (dr.Read())
            {
                ProcessName processName = new ProcessName()
                {
                    MachineID = dr.GetInt32(MachineIDPos),
                    ProcessCode = dr.GetInt32(ProcessCodePos),
                    ProcName = dr.GetString(ProcessNamePos),
                    HasChanged = false
                };

                this.Add(processName);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        public ProcessName GetByIDs(int machineID, int processCode)
        {
            return this.Find(delegate(ProcessName processName)
            {
                return (processName.MachineID == machineID)
                    && (processName.ProcessCode == processCode);
            });
        }


    }
    #endregion

    #region Item Class
    public class ProcessName : DataItem
    {
        #region ProcessName Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int machineID;
            internal int processCode;
            internal string processName;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ProcessName()
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
 
        //public override int ID
        //{
        //    get
        //    {
        //        return RecNum;
        //    }
        //    set
        //    {
        //        if (RecNum != value)
        //        {
        //            RecNum = value;
        //            NotifyPropertyChanged("ID");
        //        }
        //    }
        //}
        #endregion

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return false;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Data Column Properties

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

        public int ProcessCode
        {
            get
            {
                return this.activeData.processCode;
            }
            set
            {
                this.activeData.processCode = value;
                HasChanged = true;
            }
        }

        public string ProcName
        {
            get
            {
                return this.activeData.processName;
            }
            set
            {
                this.activeData.processName = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
