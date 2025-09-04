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
        const string backupSetsCommand =
            @"SELECT [BackupSetID]
				  ,[BackupTimestamp]
				  ,[BackupName]
				  ,[NrMachines]
				  ,[NrCustomers]
				  ,[NrArticles]
				  ,[NrSortCategories]
				  ,[NrProcessCodes]
			  FROM [dbo].[tblBackupSets]
                ORDER BY BackupTimestamp desc";


        public BackupSets GetBackupSets() 
        {
            try
            {
                const string commandString = backupSetsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    BackupSets backupSets = new BackupSets();
                    command.DataFill(backupSets, SqlDataConnection.DBConnection.JensenGroup);
                    return backupSets;
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


        public BackupSet GetBackupSet(int backupSetID)
        {
            BackupSets BackupSets = GetBackupSets();
            BackupSet backupSet = BackupSets.GetById(backupSetID);

            return backupSet;
         }
        #endregion

    }

    #region Data Collection Class
    public class BackupSets : List<BackupSet>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblBackupSets";
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
            int BackupSetIDPos = dr.GetOrdinal("BackupSetID");
            int BackupTimestampPos = dr.GetOrdinal("BackupTimestamp");
            int BackupNamePos = dr.GetOrdinal("BackupName");
            int NrMachinesPos = dr.GetOrdinal("NrMachines");
            int NrCustomersPos = dr.GetOrdinal("NrCustomers");
            int NrArticlesPos = dr.GetOrdinal("NrArticles");
            int NrSortCategoriesPos = dr.GetOrdinal("NrSortCategories");
            int NrProcessCodesPos = dr.GetOrdinal("NrProcessCodes");

            this.Clear();
            while (dr.Read())
            {
                BackupSet backupSet = new BackupSet()
                {
                    BackupSetID = dr.GetInt32(BackupSetIDPos),
                    BackupTimestamp = dr.GetDateTime(BackupTimestampPos),
                    BackupName = dr.GetString(BackupNamePos),
                    NrMachines = dr.GetInt32(NrMachinesPos),
                    NrCustomers = dr.GetInt32(NrCustomersPos),
                    NrArticles = dr.GetInt32(NrArticlesPos),
                    NrSortCategories = dr.GetInt32(NrSortCategoriesPos),
                    NrProcessCodes = dr.GetInt32(NrProcessCodesPos),
                    HasChanged = false
                };

                this.Add(backupSet);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public BackupSet GetById(int id)
        {
            return this.Find(delegate(BackupSet backupSet)
            {
                return backupSet.BackupSetID == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class BackupSet : DataItem
    {
        #region BackupSet Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int BackupSetID;
            internal DateTime BackupTimestamp;
            internal string BackupName;
            internal int NrMachines;
            internal int NrCustomers;
            internal int NrArticles;
            internal int NrSortCategories;
            internal int NrProcessCodes;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public BackupSet()
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
                return BackupSetID;
            }
            set
            {
                if (BackupSetID != value)
                {
                    BackupSetID = value;
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
                return this.activeData.BackupSetID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.BackupSetID != -1;
            }
        }
        #endregion

        #region Data Column Properties
        /*public override int PrimaryKey
        {
            get
            {
                return this.activeData.BackupSetID;
            }
            set
            {
                this.activeData.BackupSetID = value;
            }
        }*/

        public int BackupSetID
        {
            get
            {
                return this.activeData.BackupSetID;
            }
            set
            {
                this.activeData.BackupSetID = value;
                HasChanged = true;
            }
        }

        public DateTime BackupTimestamp
        {
            get
            {
                return this.activeData.BackupTimestamp;
            }
            set
            {
                this.activeData.BackupTimestamp = value;
                HasChanged = true;
            }
        }

        public string BackupName
        {
            get
            {
                return this.activeData.BackupName;
            }
            set
            {
                this.activeData.BackupName = value;
                HasChanged = true;
            }
        }

        public int NrArticles
        {
            get
            {
                return this.activeData.NrArticles;
            }
            set
            {
                this.activeData.NrArticles = value;
                HasChanged = true;
            }
        }

        public int NrCustomers
        {
            get
            {
                return this.activeData.NrCustomers;
            }
            set
            {
                this.activeData.NrCustomers = value;
                HasChanged = true;
            }
        }

        public int NrProcessCodes
        {
            get
            {
                return this.activeData.NrProcessCodes;
            }
            set
            {
                this.activeData.NrProcessCodes = value;
                HasChanged = true;
            }
        }

        public int NrMachines
        {
            get
            {
                return this.activeData.NrMachines;
            }
            set
            {
                this.activeData.NrMachines = value;
                HasChanged = true;
            }
        }

        public int NrSortCategories
        {
            get
            {
                return this.activeData.NrSortCategories;
            }
            set
            {
                this.activeData.NrSortCategories = value;
                HasChanged = true;
            }
        }

        
        #endregion
    }
    #endregion
}
