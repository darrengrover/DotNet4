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
        private LastTableUpdates lastTableUpdatesCache = null;

        public LastTableUpdates LastTableUpdatesCache
        {
            get { return lastTableUpdatesCache; }
            set { lastTableUpdatesCache = value; }
        }

        public void InvalidateLastTableUpdates()
        {
            if (lastTableUpdatesCache != null)
                lastTableUpdatesCache.IsValid = false;
        }

        private bool LastTableUpdatesAreCached()
        {
            bool test = (lastTableUpdatesCache != null);
            if (test)
            {
                test = lastTableUpdatesCache.IsValid;
            }
            return test;
        }
        
        #region Select Data
        const string tblUpdatesCommand =
            @"SELECT 
                TableName			
		        , LastUpdate 
              FROM [dbo].[fnLastTblUpdate]()";

        public DateTime TableLastUpdated(string tblName)
        {
            DateTime longAgo = DateTime.Now.AddYears(-1);
            DateTime lastUpdateTime = longAgo;
            LastTableUpdates ltus = GetLastTableUpdates();
            LastTableUpdate ltu = ltus.GetByName(tblName);
            if (ltu != null)
            {
                lastUpdateTime = ltu.LastUpdated;
            }
            return lastUpdateTime;
        }

        public LastTableUpdates GetDatabaseTableUpdateTimes(SqlDataConnection.DBConnection dbConnection)
        {
            try
            {
                const string commandString = tblUpdatesCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    LastTableUpdates ltu = new LastTableUpdates();
                    ltu.Lifespan = 0.9 * MinimumRateSeconds / 3600;
                    command.DataFill(ltu, dbConnection);
                    return ltu;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                }
                throw;
            }
        }


        public LastTableUpdates GetLastTableUpdates() 
        {
            if (LastTableUpdatesAreCached())
            {
                return lastTableUpdatesCache;
            }
            try
            {
                GetServerTime();
                if (lastTableUpdatesCache == null) lastTableUpdatesCache = new LastTableUpdates();
                lastTableUpdatesCache = GetDatabaseTableUpdateTimes(SqlDataConnection.DBConnection.JensenGroup);
                return lastTableUpdatesCache;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    //Debugger.Break();
                }
                throw;
            }
        }

 
        #endregion

    }


    #region Data Collection Class
    public class LastTableUpdates : List<LastTableUpdate>, IDataFiller
    {
        private double lifespan = 1.0 / 3600;
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
                bool test = isValid && (this.Count >= 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    DateTime testTime = lastRead.AddHours(lifespan);
                    DateTime rightNow = DateTime.Now;
                    test = testTime > rightNow; //must ignore server time
                }
                return test || neverExpire;
            }
            set
            { isValid = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            int TableNamePos = dr.GetOrdinal("TableName");
            int LastUpdatedPos = dr.GetOrdinal("LastUpdate");

            this.Clear();
            while (dr.Read())
            {
                LastTableUpdate lastTableUpdate = new LastTableUpdate()
                {
                    TableName = dr.GetString(TableNamePos),
                    LastUpdated = dr.GetDateTime(LastUpdatedPos),
                    HasChanged = false
                };

                this.Add(lastTableUpdate);
            }
            lastRead = DateTime.Now;
            IsValid = true;

            return this.Count;
        }

        public void TimezoneFix(int diffHours)
        {
            foreach (LastTableUpdate ltu in this)
            {
                ltu.LastUpdated=ltu.LastUpdated.AddHours(diffHours);
            }
        }

        public LastTableUpdate GetByName(string aName)
        {
            return this.Find(delegate(LastTableUpdate lastTableUpdate)
            {
                return (lastTableUpdate.TableName.ToLower() == aName.ToLower());
            });
        }
    }
    #endregion

    #region Item Class
    public class LastTableUpdate : DataItem
    {
        #region LastTableUpdate Data Record
        protected class DataRecord : ICopyableObject
        {
            internal String TableName;
            internal DateTime LastUpdated;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public LastTableUpdate()
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
        #endregion

        #region Record Status Properties

        /// <summary>This is a new record, ie Not yet created in the database</summary>
        public override bool IsNew
        {
            get
            {
                return true;
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

        public String TableName
        {
            get
            {
                return this.activeData.TableName;
            }
            set
            {
                this.activeData.TableName = value;
                HasChanged = true;
            }
        }

        public DateTime LastUpdated
        {
            get
            {
                return this.activeData.LastUpdated;
            }
            set
            {
                this.activeData.LastUpdated = value;
                //this.activeData.LastUpdated.AddHours(
                HasChanged = true;
            }
        }

   
        #endregion
    }
    #endregion
}
