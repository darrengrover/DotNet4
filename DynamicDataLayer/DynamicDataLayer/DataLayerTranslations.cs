using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Dynamic.DataLayer
{

    #region Cockpit translations

    #region Resource_Defs
    public class Resource_Defs : DataList
    {
        public Resource_Defs()
        {
            Lifespan = 1.0;
            ListType = typeof(Resource_Def);
            DbName = "JEGR_DB";
            TblName = "tblResource_Def";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Recnum", "Recnum", true, true);
            dBFieldMappings.AddMapping("IDResource", "IDResource");
            dBFieldMappings.AddMapping("ResourceType", "ResourceType");
            dBFieldMappings.AddMapping("ResDescription", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }


    }

    public class Resource_Def : DataItem
    {
        #region private fields

        private int recnum = -1;
        private string iDResource = string.Empty;
        private string resourceType = string.Empty;

        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get { return recnum; }
            set
            {
                recnum = AssignNotify(ref recnum, value, "Recnum");
                ID = recnum;
                PrimaryKey = recnum;
            }
        }

        public string IDResource
        {
            get { return iDResource; }
            set { iDResource = AssignNotify(ref iDResource, value, "IDResource"); }
        }

        public string ResourceType
        {
            get { return resourceType; }
            set { resourceType = AssignNotify(ref resourceType, value, "ResourceType"); }
        }

        #endregion
    }
    #endregion

    #region Resource_Langs
    public class Resource_Langs : DataList
    {
        public Resource_Langs()
        {
            Lifespan = 1.0;
            ListType = typeof(Resource_Lang);
            DbName = "JEGR_DB";
            TblName = "tblResource_Lang";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Recnum", "Recnum", true, true);
            dBFieldMappings.AddMapping("IDLang", "IDLang");
            dBFieldMappings.AddMapping("CultureName", "CultureName");
            dBFieldMappings.AddMapping("ResDescription", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public DataItem GetByIDLang(string idlang)
        {
            DataItem di = null;
            foreach (DataItem dataItem in this)
            {
                Resource_Lang msub = (Resource_Lang)dataItem;
                if (msub.IDLang == idlang)
                {
                    di = dataItem;
                    break;
                }
            }
            return di;
        }

        public DataItem GetByCultureName(string culturename)
        {
            DataItem di = null;
            foreach (DataItem dataItem in this)
            {
                Resource_Lang msub = (Resource_Lang)dataItem;
                if (msub.CultureName == culturename)
                {
                    di = dataItem;
                    break;
                }
            }
            return di;
        }

    }

    public class Resource_Lang : DataItem
    {
        #region private fields

        private int recnum = -1;
        private string iDLang = string.Empty;
        private string cultureName = string.Empty;

        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get { return recnum; }
            set
            {
                recnum = AssignNotify(ref recnum, value, "Recnum");
                ID = recnum;
                PrimaryKey = recnum;
            }
        }

        public string IDLang
        {
            get { return iDLang; }
            set { iDLang = AssignNotify(ref iDLang, value, "IDLang"); }
        }

        public string CultureName
        {
            get { return cultureName; }
            set { cultureName = AssignNotify(ref cultureName, value, "CultureName"); }
        }

        #endregion
    }
    #endregion

    #region Resource_Def2Langs
    public class Resource_Def2Langs : DataList
    {
        public Resource_Def2Langs()
        {
            Lifespan = 1.0;
            ListType = typeof(Resource_Def2Lang);
            DbName = "JEGR_DB";
            TblName = "tblResource_Def2Lang";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Recnum", "Recnum", true, true);
            dBFieldMappings.AddMapping("IDResource", "IDResource");
            dBFieldMappings.AddMapping("IDLang", "IDLang");
            dBFieldMappings.AddMapping("ResourceText", "ResourceText");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public DataItem GetByIDResourceIDLang(string idresource, string currentidLang)
        {
            DataItem di = null;
            foreach (DataItem dataItem in this)
            {
                Resource_Def2Lang rec = (Resource_Def2Lang)dataItem;
                if ((rec.IDLang == currentidLang) && (rec.IDResource == idresource))
                {
                    di = dataItem;
                    break;
                }
            }
            return di;
        }

    }

    public class Resource_Def2Lang : DataItem
    {
        #region private fields

        private int recnum = -1;
        private string iDResource = string.Empty;
        private string iDLang = string.Empty;
        private string resourceText = string.Empty;

        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get { return recnum; }
            set
            {
                recnum = AssignNotify(ref recnum, value, "Recnum");
                ID = recnum;
                PrimaryKey = recnum;
            }
        }

        public string IDResource
        {
            get { return iDResource; }
            set { iDResource = AssignNotify(ref iDResource, value, "IDResource"); }
        }

        public string IDLang
        {
            get { return iDLang; }
            set { iDLang = AssignNotify(ref iDLang, value, "IDLang"); }
        }

        public string ResourceText
        {
            get { return resourceText; }
            set { resourceText = AssignNotify(ref resourceText, value, "ResourceText"); }
        }

        #endregion
    }
    #endregion

    #region Resource_Types
    public class Resource_Types : DataList
    {
        public Resource_Types()
        {
            Lifespan = 1.0;
            ListType = typeof(Resource_Type);
            DbName = "JEGR_DB";
            TblName = "tblResource_Def";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Recnum", "Recnum", true, true);
            dBFieldMappings.AddMapping("ResourceType", "ResourceType");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class Resource_Type : DataItem
    {
        #region private fields

        private int recnum = -1;
        private string resourceType = string.Empty;

        #endregion

        #region Data Column Properties

        public int Recnum
        {
            get { return recnum; }
            set
            {
                recnum = AssignNotify(ref recnum, value, "Recnum");
                ID = recnum;
                PrimaryKey = recnum;
            }
        }

        public string ResourceType
        {
            get { return resourceType; }
            set { resourceType = AssignNotify(ref resourceType, value, "ResourceType"); }
        }

        #endregion
    }
    #endregion


    #region Translations
    partial class SqlDataAccess
    {

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

        private Resource_Types GetAllResourceTypes()
        {
            Resource_Types recs = new Resource_Types();
            recs = (Resource_Types)DBDataListSelect(recs, true, false);
            return recs;
        }

        private Resource_Defs GetAllResourceDefs()
        {
            Resource_Defs recs = new Resource_Defs();
            recs = (Resource_Defs)DBDataListSelect(recs, true, false);
            return recs;
        }

        private Resource_Langs GetAllResourceLangs()
        {
            Resource_Langs recs = new Resource_Langs();
            recs = (Resource_Langs)DBDataListSelect(recs, true, false);
            return recs;
        }

        private Resource_Def2Langs GetAllResourceDef2Langs()
        {
            Resource_Def2Langs recs = new Resource_Def2Langs();
            recs = (Resource_Def2Langs)DBDataListSelect(recs, true, false);
            return recs;
        }
            

        public void GetAllLanguages()
        {
            GetAllLanguages(false);
        }

        public void GetAllLanguages(bool noCacheRead)
        {
            if (noCacheRead || !LanguagesAreCached())
            {
                resource_TypesCache = GetAllResourceTypes();
                resource_DefsCache = GetAllResourceDefs();
                resource_LangsCache = GetAllResourceLangs();
                resource_Def2LangsCache = GetAllResourceDef2Langs();
            }
        }

        private string currentidLang = "English (UK)";

        public string CurrentidLang
        {
            get { return currentidLang; }
            set
            {
                GetAllLanguages();
                if (resource_LangsCache != null)
                {
                    Resource_Lang rl = (Resource_Lang)resource_LangsCache.GetByIDLang(value);
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
                    Resource_Lang rl = (Resource_Lang)resource_LangsCache.GetByCultureName(value);
                    if (rl != null)
                    {
                        currentidLang = rl.IDLang;
                    }
                }
                currentCultureName = value;
            }
        }

        public string Translate(string idresource)
        {
            string aString = "." + idresource;
            if (resource_Def2LangsCache != null)
            {
                Resource_Def2Lang d2l = (Resource_Def2Lang)resource_Def2LangsCache.GetByIDResourceIDLang(idresource, currentidLang);
                if (d2l != null)
                    aString = d2l.ResourceText;
            }
            return aString;
        }
    }

    #endregion


    #endregion

    #region new translations

    #region TranslationContentTypes
    public class TranslationContentTypes : DataList
    {
        public TranslationContentTypes()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationContentType);
            DbName = "JEGR_Utils";
            TblName = "tblTranslationContentTypes";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ContentTypeID", "ID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class TranslationContentType : DataItem
    { }
    #endregion


    #region TranslationStatuses
    public class TranslationStatusList : DataList
    {
        public TranslationStatusList()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationStatus);
            DbName = "JEGR_Utils";
            TblName = "tblTranslationStatus";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("StatusID", "ID", true, false);
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class TranslationStatus : DataItem
    { }
    #endregion


    #region TranslationCultures
    public class TranslationCultures : DataList, INotifyCollectionChanged
    {
        public TranslationCultures()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationCulture);
            DbName = "JEGR_Utils";
            TblName = "tblTranslationCultures";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("CultureID", "CultureID", true, false);
            dBFieldMappings.AddMapping("CultureName", "CultureName");
            dBFieldMappings.AddMapping("LanguageName", "LanguageName");
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class TranslationCulture : DataItem
    {
        #region private fields

        private int cultureID;
        private string cultureName;
        private string languageName;
        //private string description;

        #endregion

        #region Data Column Properties

        public int CultureID
        {
            get { return cultureID; }
            set
            {
                cultureID = AssignNotify(ref cultureID, value, "CultureID");
                ID = cultureID;
                PrimaryKey = cultureID;
            }
        }

        public string CultureName
        {
            get { return cultureName; }
            set
            {
                cultureName = AssignNotify(ref cultureName, value, "CultureName");
                ItemName = cultureName;
            }
        }

        public string LanguageName
        {
            get { return languageName; }
            set { languageName = AssignNotify(ref languageName, value, "LanguageName"); }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        #endregion
    }
    #endregion


    #region TranslationUIControls
    public class TranslationUIControls : DataList
    {
        public TranslationUIControls()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationUIControl);
            DbName = "JEGR_Utils";
            TblName = "tblTranslationUIControls";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("UIControlID", "UIControlID", true, false);
            dBFieldMappings.AddMapping("ControlName", "ControlName");
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("DefaultContent", "DefaultContent");
            dBFieldMappings.AddMapping("SkipTranslation", "SkipTranslation");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class TranslationUIControl : DataItem
    {
        #region private fields

        private int uIControlID = -1;
        private int appID = -1;

        private string controlName = string.Empty;
        private string description = string.Empty;
        private string defaultContent = string.Empty;
        private bool skipTranslation = false;

        #endregion

        #region Data Column Properties

        public int UIControlID
        {
            get { return uIControlID; }
            set
            {
                uIControlID = AssignNotify(ref uIControlID, value, "UIControlID");
                ID = uIControlID;
                PrimaryKey = uIControlID;
            }
        }

        public int AppID
        {
            get { return appID; }
            set { appID = AssignNotify(ref appID, value, "AppID"); }
        }

        public string ControlName //control and parent form/library name
        {
            get { return controlName; }
            set
            {
                controlName = AssignNotify(ref controlName, value, "ControlName");
                ItemName = controlName;
            }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        public string DefaultContent
        {
            get { return defaultContent; }
            set { defaultContent = AssignNotify(ref defaultContent, value, "DefaultContent"); }
        }

        public bool SkipTranslation
        {
            get { return skipTranslation; }
            set { skipTranslation = AssignNotify(ref skipTranslation, value, "SkipTranslation"); }
        }

        #endregion
    }
    #endregion


    #region Translations
    public class Translations : DataList
    {
        public Translations()
        {
            Lifespan = 1.0;
            ListType = typeof(Translation);
            DbName = "JEGR_Utils";
            TblName = "tblTranslations";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("TranslationID", "TranslationID", true, false);
            dBFieldMappings.AddMapping("CultureID", "CultureID");
            dBFieldMappings.AddMapping("TextIdent", "TextIdent");
            dBFieldMappings.AddMapping("Description", "Description");
            dBFieldMappings.AddMapping("Text", "Text");
            dBFieldMappings.AddMapping("StatusID", "StatusID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class Translation : DataItem
    {
        #region private fields

        private int translationID = -1;
        private int cultureID = -1;
        private string text = string.Empty;
        private int statusID = -1;

        #endregion

        #region Data Column Properties

        public int TranslationID
        {
            get { return translationID; }
            set
            {
                translationID = AssignNotify(ref translationID, value, "TranslationID");
                ID = translationID;
                PrimaryKey = translationID;
            }
        }

        public int CultureID
        {
            get { return cultureID; }
            set { cultureID = AssignNotify(ref cultureID, value, "CultureID"); }
        }

        //public string TextIdent
        //{
        //    get { return textIdent; }
        //    set
        //    {
        //        textIdent = AssignNotify(ref textIdent, value, "TextIdent");
        //        ItemName = textIdent;
        //    }
        //}

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        public string Text
        {
            get { return text; }
            set { text = AssignNotify(ref text, value, "Text"); }
        }

        public int StatusID
        {
            get { return statusID; }
            set { statusID = AssignNotify(ref statusID, value, "StatusID"); }
        }

        #endregion
    }
    #endregion


    #region TranslationDictionary
    public class TranslationDictionary : DataList
    {
        public TranslationDictionary()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationDictionaryItem);
            DbName = "JEGR_Utils";
            TblName = "tblTranslations";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("DictionaryID", "DictionaryID", true, false);
            dBFieldMappings.AddMapping("TextIdent", "TextIdent");
            dBFieldMappings.AddMapping("NativeText", "NativeText");
            dBFieldMappings.AddMapping("HomographID", "HomographID");
            dBFieldMappings.AddMapping("ContentTypeID", "ContentTypeID");
            dBFieldMappings.AddMapping("Description", "Description");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }


    public class TranslationDictionaryItem : DataItem
    {
        #region private fields

        private int dictionaryID = -1;
        //private string textIdent = string.Empty;
        private string nativeText = string.Empty;
        private int contextID = 0;
        private int contentTypeID = -1;
        private string description = string.Empty;

        #endregion

        #region Data Column Properties

        public int TranslationID
        {
            get { return dictionaryID; }
            set
            {
                dictionaryID = AssignNotify(ref dictionaryID, value, "TranslationID");
                ID = dictionaryID;
                PrimaryKey = dictionaryID;
            }
        }

        public int ContentTypeID
        {
            get { return contentTypeID; }
            set { contentTypeID = AssignNotify(ref contentTypeID, value, "ContentTypeID"); }
        }

        //public string TextIdent
        //{
        //    get { return textIdent; }
        //    set
        //    {
        //        textIdent = AssignNotify(ref textIdent, value, "TextIdent");
        //        ItemName = textIdent;
        //    }
        //}

        public int ContextID
        {
            get { return contextID; }
            set { contextID = AssignNotify(ref contextID, value, "ContextID"); }
        }

        public string NativeText
        {
            get { return nativeText; }
            set { nativeText = AssignNotify(ref nativeText, value, "NativeText"); }
        }

        //public string Description
        //{
        //    get { return description; }
        //    set { description = AssignNotify(ref description, value, "Description"); }
        //}

        #endregion
    }
    #endregion


    #region TranslationUILookups
    public class TranslationUILookups : DataList
    {
        public TranslationUILookups()
        {
            Lifespan = 1.0;
            ListType = typeof(TranslationUILookup);
            DbName = "JEGR_Utils";
            TblName = "tblTranslationUILookup";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("LookupID", "LookupID", true, false);
            dBFieldMappings.AddMapping("TranslationID", "TranslationID");
            dBFieldMappings.AddMapping("UIControlID", "UIControlID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class TranslationUILookup : DataItem
    {
        #region private fields

        private int lookupID = -1;
        private int translationID = -1;
        private int uIControlID = -1;

        #endregion

        #region Data Column Properties

        public int LookupID
        {
            get { return lookupID; }
            set
            {
                lookupID = AssignNotify(ref lookupID, value, "LookupID");
                ID = lookupID;
                PrimaryKey = lookupID;
            }
        }

        public int TranslationID
        {
            get { return translationID; }
            set { translationID = AssignNotify(ref translationID, value, "TranslationID"); }
        }

        public int UIControlID
        {
            get { return uIControlID; }
            set { uIControlID = AssignNotify(ref uIControlID, value, "UIControlID"); }
        }

        #endregion
    }
    #endregion

    #endregion


}
