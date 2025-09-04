using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region caches

        private Resource_Defs resource_DefsCache = null;
        private Resource_Types resource_TypesCache = null;
        private Resource_Langs resource_LangsCache = null;
        private Resource_Def2Langs resource_Def2LangsCache = null;

        private bool LanguagesAreCached()
        {
            bool test = (resource_DefsCache != null) && (resource_TypesCache != null) && (resource_LangsCache != null) && (resource_Def2LangsCache != null);
            if (test)
            {
                test = (resource_DefsCache.IsValid) && (resource_TypesCache.IsValid) && (resource_LangsCache.IsValid) && (resource_Def2LangsCache.IsValid);
            }
            return test;
        }

        #endregion

        #region Select Data

        private Resource_Defs GetAllResourceDefs()
        {
            try
            {
                const string commandString = @"SELECT [RecNum]
                                                      ,[idResource]
                                                      ,[ResourceType]
                                                      ,[ResDescription]
                                                  FROM [dbo].[tblResource_Def]";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Resource_Defs recs = new Resource_Defs();
                    command.DataFill(recs, SqlDataConnection.DBConnection.JensenGroup);
                    return recs;
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

        private Resource_Types GetAllResourceTypes()
        {
            try
            {
                const string commandString = @"SELECT [RecNum]
                                                      ,[ResourceType]
                                                  FROM [dbo].[tblResource_Type]";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Resource_Types recs = new Resource_Types();
                    command.DataFill(recs, SqlDataConnection.DBConnection.JensenGroup);
                    return recs;
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

        private Resource_Langs GetAllResourceLangs()
        {
            try
            {
                const string commandString = @"SELECT [RecNum]
                                                      ,[idLang]
                                                      ,[CultureName]
                                                      ,[LangDescription]
                                                  FROM [dbo].[tblResource_Lang]";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Resource_Langs recs = new Resource_Langs();
                    command.DataFill(recs, SqlDataConnection.DBConnection.JensenGroup);
                    return recs;
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

        private Resource_Def2Langs GetAllResourceDef2Langs()
        {
            try
            {
                const string commandString = @"SELECT [RecNum]
                                                      ,[idResource]
                                                      ,[idLang]
                                                      ,[ResourceText]
                                                  FROM [dbo].[tblResource_Def2Lang]";
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Resource_Def2Langs recs = new Resource_Def2Langs();
                    command.DataFill(recs, SqlDataConnection.DBConnection.JensenGroup);
                    return recs;
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


        public void GetAllLanguages()
        {
            GetAllLanguages(false);
        }

        public void GetAllLanguages(bool noCache)
        {
            if (noCache || !LanguagesAreCached())
            {
                resource_DefsCache = GetAllResourceDefs();
                resource_TypesCache = GetAllResourceTypes();
                resource_LangsCache = GetAllResourceLangs();
                resource_Def2LangsCache = GetAllResourceDef2Langs();
            }
        }

        #endregion


        #region Insert Data

        //no insert data for languages - managed manually

        #endregion

        #region Update Data

        //no update data for languages - managed manually!

        #endregion

        #region Delete Data

        //no delete data for languages - managed manually!

        #endregion

        #region Translations

        private string currentidLang = "English (UK)";

        public string CurrentidLang
        {
            get { return currentidLang; }
            set
            {
                GetAllLanguages();
                if (resource_LangsCache != null)
                {
                    Resource_Lang rl = resource_LangsCache.GetByidLang(value);
                    if (rl != null)
                    {
                        currentCultureName = rl.CultureName;
                    }
                } 
                currentidLang = value;
            }
        }

        private string currentCultureName = "en-GB";

        public string CurrentCultureName
        {
            get { return currentCultureName; }
            set
            {
                GetAllLanguages();
                if (resource_LangsCache != null)
                {
                    Resource_Lang rl = resource_LangsCache.GetByCultureName(value);
                    if (rl != null)
                    {
                        currentidLang = rl.idLang;
                    }
                }
                currentCultureName = value;
            }
        }

        public string Translate(string idresource)
        {
            string aString = "[mr]" + idresource;
            if (resource_Def2LangsCache != null)
            {
                Resource_Def2Lang d2l = resource_Def2LangsCache.GetByidResourceidLang(idresource, currentidLang);
                if (d2l != null)
                    aString = d2l.ResourceText;
            }
            return aString;
        }

        #endregion
    }


    #region Data Collection Classes

    public class Resource_Types : List<Resource_Type>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Resource_Type rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(Resource_Type rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }
        #endregion

        # region properties
        private double lifespan = 1.0;
        private string tblName = "tblResource_Type";
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
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecnumPos = dr.GetOrdinal("Recnum");
            int ResourceTypePos = dr.GetOrdinal("ResourceType");

            this.Clear();
            while (dr.Read())
            {
                Resource_Type rec = new Resource_Type()
                {
                    Recnum = dr.GetInt32(RecnumPos),
                    ResourceType = dr.GetString(ResourceTypePos),
                    HasChanged = false
                };

                this.Add(rec);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        #endregion

        #region find

        public Resource_Type GetById(int id)
        {
            return this.Find(delegate(Resource_Type rec)
            {
                return rec.Recnum == id;
            });
        }

        #endregion
    }

    public class Resource_Defs : List<Resource_Def>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Resource_Def rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(Resource_Def rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }
        #endregion

        # region properties
        private double lifespan = 1.0;
        private string tblName = "tblResource_Def";
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
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecnumPos = dr.GetOrdinal("Recnum");
            int ResourceTypePos = dr.GetOrdinal("ResourceType");
            int IdResourcePos = dr.GetOrdinal("IdResource");
            int ResDescriptionPos = dr.GetOrdinal("ResDescription");

            this.Clear();
            while (dr.Read())
            {
                Resource_Def rec = new Resource_Def()
                {
                    Recnum = dr.GetInt32(RecnumPos),
                    idResource = dr.GetString(IdResourcePos),
                    ResourceType = dr.GetString(ResourceTypePos),
                    ResDescription = dr.GetString(ResDescriptionPos),
                    HasChanged = false
                };

                this.Add(rec);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        #endregion

        #region find

        public Resource_Def GetById(int id)
        {
            return this.Find(delegate(Resource_Def rec)
            {
                return rec.Recnum == id;
            });
        }

        #endregion
    }

    public class Resource_Def2Langs : List<Resource_Def2Lang>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Resource_Def2Lang rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(Resource_Def2Lang rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }
        #endregion

        # region properties
        private double lifespan = 1.0;
        private string tblName = "tblResource_Def2Lang";
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
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecnumPos = dr.GetOrdinal("Recnum");
            int idLangPos = dr.GetOrdinal("idLang");
            int idResourcePos = dr.GetOrdinal("idResource");
            int ResourceTextPos = dr.GetOrdinal("ResourceText");

            this.Clear();
            while (dr.Read())
            {
                Resource_Def2Lang rec = new Resource_Def2Lang()
                {
                    Recnum = dr.GetInt32(RecnumPos),
                    idLang = dr.GetString(idLangPos),
                    idResource = dr.GetString(idResourcePos),
                    ResourceText = dr.GetString(ResourceTextPos),
                    HasChanged = false
                };

                this.Add(rec);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        #endregion

        #region find

        public Resource_Def2Lang GetById(int id)
        {
            return this.Find(delegate(Resource_Def2Lang rec)
            {
                return rec.Recnum == id;
            });
        }

        public Resource_Def2Lang GetByidResourceidLang(string idRes, string idlang)
        {
            return this.Find(delegate(Resource_Def2Lang rec)
            {
                return (rec.idResource == idRes && rec.idLang == idlang);
            });
        }

        #endregion
    }

    public class Resource_Langs : List<Resource_Lang>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Resource_Lang rec)
        {
            base.Add(rec);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rec));
            }
        }

        new public void Remove(Resource_Lang rec)
        {
            base.RemoveAt(this.IndexOf(rec));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, rec));
            }
        }

        #endregion

        # region properties
        private double lifespan = 1.0;
        private string tblName = "tblResource_Lang";
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
        #endregion

        #region fill
        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            int RecnumPos = dr.GetOrdinal("Recnum");
            int idLangPos = dr.GetOrdinal("idLang");
            int CultureNamePos = dr.GetOrdinal("CultureName");
            int LangDescriptionPos = dr.GetOrdinal("LangDescription");

            this.Clear();
            while (dr.Read())
            {
                Resource_Lang rec = new Resource_Lang()
                {
                    Recnum = dr.GetInt32(RecnumPos),
                    idLang = dr.GetString(idLangPos),
                    CultureName = dr.GetString(CultureNamePos),
                    LangDescription = dr.GetString(LangDescriptionPos),
                    HasChanged = false
                };

                this.Add(rec);
            }
            LastRead = da.ServerTime;
            IsValid = true;

            return this.Count;
        }

        #endregion

        #region find

        public Resource_Lang GetById(int id)
        {
            return this.Find(delegate(Resource_Lang rec)
            {
                return rec.Recnum == id;
            });
        }

        public Resource_Lang GetByCultureName(string name)
        {
            return this.Find(delegate(Resource_Lang rec)
            {
                return rec.CultureName == name;
            });
        }

        public Resource_Lang GetByidLang(string lang)
        {
            return this.Find(delegate(Resource_Lang rec)
            {
                return rec.idLang == lang;
            });
        }
        #endregion
    }


    #endregion

    #region Item Classes

    public class Resource_Type : DataItem, INotifyPropertyChanged
    {
        #region Resource_Type Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int Recnum;
            internal string ResourceType;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Resource_Type()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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
            get { return activeData.Recnum <= -1; }
        }
        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }

        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get
            {
                return this.activeData.Recnum;
            }
            set
            {
                if (this.activeData.Recnum != value)
                {
                    this.activeData.Recnum = value;
                    NotifyPropertyChanged("Recnum");
                }
            }
        }

        public string ResourceType
        {
            get
            {
                return this.activeData.ResourceType;
            }
            set
            {
                if (this.activeData.ResourceType != value)
                {
                    this.activeData.ResourceType = value;
                    NotifyPropertyChanged("ResourceType");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion

    }

    public class Resource_Def : DataItem, INotifyPropertyChanged
    {
        #region Resource_Def Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int Recnum;
            internal string idResource;
            internal string ResourceType;
            internal string ResDescription;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #endregion

        #region Constructor
        public Resource_Def()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        public override bool IsNew
        {
            get { return Recnum == -1; }
        }
        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }
        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get
            {
                return this.activeData.Recnum;
            }
            set
            {
                if (this.activeData.Recnum != value)
                {
                    this.activeData.Recnum = value;
                    NotifyPropertyChanged("Recnum");
                }
            }
        }

        public string idResource
        {
            get
            {
                return this.activeData.idResource;
            }
            set
            {
                if (this.activeData.idResource != value)
                {
                    this.activeData.idResource = value;
                    NotifyPropertyChanged("IdResource");
                }
            }
        }

        public string ResourceType
        {
            get
            {
                return this.activeData.ResourceType;
            }
            set
            {
                if (this.activeData.ResourceType != value)
                {
                    this.activeData.ResourceType = value;
                    NotifyPropertyChanged("ResourceType");
                }
            }
        }

        public string ResDescription
        {
            get
            {
                return this.activeData.ResDescription;
            }
            set
            {
                if (this.activeData.ResDescription != value)
                {
                    this.activeData.ResDescription = value;
                    NotifyPropertyChanged("ResDescription");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion

    }

    public class Resource_Def2Lang : DataItem, INotifyPropertyChanged
    {
        #region Resource_Def Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int Recnum;
            internal string idResource;
            internal string idLang;
            internal string ResourceText;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #endregion

        #region Constructor
        public Resource_Def2Lang()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        public override bool IsNew
        {
            get { return Recnum == -1; }
        }
        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }
        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get
            {
                return this.activeData.Recnum;
            }
            set
            {
                if (this.activeData.Recnum != value)
                {
                    this.activeData.Recnum = value;
                    NotifyPropertyChanged("Recnum");
                }
            }
        }

        public string idLang
        {
            get
            {
                return this.activeData.idLang;
            }
            set
            {
                if (this.activeData.idLang != value)
                {
                    this.activeData.idLang = value;
                    NotifyPropertyChanged("idLang");
                }
            }
        }

        public string idResource
        {
            get
            {
                return this.activeData.idResource;
            }
            set
            {
                if (this.activeData.idResource != value)
                {
                    this.activeData.idResource = value;
                    NotifyPropertyChanged("idResource");
                }
            }
        }

        public string ResourceText
        {
            get
            {
                return this.activeData.ResourceText;
            }
            set
            {
                if (this.activeData.ResourceText != value)
                {
                    this.activeData.ResourceText = value;
                    NotifyPropertyChanged("ResourceText");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion

    }

    public class Resource_Lang : DataItem, INotifyPropertyChanged
    {
        #region Resource_Lang Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int Recnum;

            internal string idLang;
            internal string CultureName;
            internal string LangDescription;
            internal bool DeleteRecord;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #endregion

        #region Constructor
        public Resource_Lang()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            activeData.DeleteRecord = false;
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

        public override bool IsNew
        {
            get { return Recnum == -1; }
        }
        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return !IsNew;
            }
        }
        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get
            {
                return this.activeData.Recnum;
            }
            set
            {
                if (this.activeData.Recnum != value)
                {
                    this.activeData.Recnum = value;
                    NotifyPropertyChanged("Recnum");
                }
            }
        }

        public string idLang
        {
            get
            {
                return this.activeData.idLang;
            }
            set
            {
                if (this.activeData.idLang != value)
                {
                    this.activeData.idLang = value;
                    NotifyPropertyChanged("idLang");
                }
            }
        }

        public string CultureName
        {
            get
            {
                return this.activeData.CultureName;
            }
            set
            {
                if (this.activeData.CultureName != value)
                {
                    this.activeData.CultureName = value;
                    NotifyPropertyChanged("CultureName");
                }
            }
        }

        public string LangDescription
        {
            get
            {
                return this.activeData.LangDescription;
            }
            set
            {
                if (this.activeData.LangDescription != value)
                {
                    this.activeData.LangDescription = value;
                    NotifyPropertyChanged("LangDescription");
                }
            }
        }

        public bool DeleteRecord
        {
            get
            {
                return this.activeData.DeleteRecord;
            }
            set
            {
                if (this.activeData.DeleteRecord != value)
                {
                    this.activeData.DeleteRecord = value;
                    NotifyPropertyChanged("DeleteRecord");
                }
            }
        }

        #endregion

        #region Notify
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String info)
        //{
        //    HasChanged = true;
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(info));
        //        PropertyChanged(this, new PropertyChangedEventArgs("HasChanged"));
        //    }
        //}
        #endregion

    }

    #endregion
}
