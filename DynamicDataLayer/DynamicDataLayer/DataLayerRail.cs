using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace Dynamic.DataLayer
{

    #region SortingStations

    public class SortingStations : DataList
    {
        public SortingStations()
        {
            Lifespan = 1.0;
            ListType = typeof(SortingStation);
            TblName = "tblSortingStations";
            DbName = "Rail_DB";
            allowStoreToCache = false;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("StnID", "StnID", true, false);
            dBFieldMappings.AddMapping("SystemID", "SystemID");
            dBFieldMappings.AddMapping("CategID", "CategID");
            dBFieldMappings.AddMapping("CustID", "CustID");
            dBFieldMappings.AddMapping("StationGroup", "StationGroup");
            dBFieldMappings.AddMapping("EditDate", "EditDate");
            dBFieldMappings.AddMapping("SentDate", "SentDate");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class SortingStation : DataItem
    {
        private int stnID;
        private int systemID;
        private int categID;
        private int custID;
        private int stationGroup;
        private DateTime editDate;
        private DateTime sentDate;

        public int StnID
        {
            get { return stnID; }
            set
            {
                stnID = AssignNotify(ref stnID, value, "StnID");
                ID = Int32sTo64(systemID, stnID);
                PrimaryKey = ID;
            }
        }
        public int SystemID
        {
            get { return systemID; }
            set
            {
                systemID = AssignNotify(ref systemID, value, "SystemID");
                ID = Int32sTo64(systemID, stnID);
                PrimaryKey = ID;
            }
        }
        public int CategID
        {
            get { return categID; }
            set { categID = AssignNotify(ref categID, value, "CategID"); }
        }
        public int CustID
        {
            get { return custID; }
            set { custID = AssignNotify(ref custID, value, "CustID"); }
        }
        public int StationGroup
        {
            get { return stationGroup; }
            set { stationGroup = AssignNotify(ref stationGroup, value, "StationGroup"); }
        }
        public DateTime EditDate
        {
            get { return editDate; }
            set { editDate = AssignNotify(ref editDate, value, "EditDate"); }
        }
        public DateTime SentDate
        {
            get { return sentDate; }
            set { sentDate = AssignNotify(ref sentDate, value, "SentDate"); }
        }

    }

    #endregion

    #region SortingSubs

    public class SortingSubs : DataList
    {
        public SortingSubs()
        {
            Lifespan = 1.0;
            ListType = typeof(SortingSub);
            TblName = "tblSorting_Sub";
            DbName = "Rail_DB";
            allowStoreToCache = false;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("StnID", "StnID", true, false);
            dBFieldMappings.AddMapping("SystemID", "SystemID");
            dBFieldMappings.AddMapping("Weight", "Weight");
            dBFieldMappings.AddMapping("ItemCount", "ItemCount");
            dBFieldMappings.AddMapping("Raw_Zero", "RawZero");
            dBFieldMappings.AddMapping("Raw_Span", "RawSpan");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class SortingSub : DataItem
    {
        private int stnID;
        private int systemID;
        private int weight;
        private int itemCount;
        private int rawZero;
        private int rawSpan;

        public int StnID
        {
            get { return stnID; }
            set
            {
                stnID = AssignNotify(ref stnID, value, "StnID");
                ID = Int32sTo64(systemID, stnID);
                PrimaryKey = ID;
            }
        }
        public int SystemID
        {
            get { return systemID; }
            set
            {
                systemID = AssignNotify(ref systemID, value, "SystemID");
                ID = Int32sTo64(systemID, stnID);
                PrimaryKey = ID;
            }
        }

        public int Weight
        {
            get { return weight; }
            set { weight = AssignNotify(ref weight, value, "Weight"); }
        }

        public int ItemCount
        {
            get { return itemCount; }
            set { itemCount = AssignNotify(ref itemCount, value, "ItemCount"); }
        }

        public int RawZero
        {
            get { return rawZero; }
            set { rawZero = AssignNotify(ref rawZero, value, "RawZero"); }
        }

        public int RawSpan
        {
            get { return rawSpan; }
            set { rawSpan = AssignNotify(ref rawSpan, value, "RawSpan"); }
        }


    }

    #endregion

    #region CategoriesRail
    
    public class CategoriesRail : DataList
    {
        public CategoriesRail()
        {
            Lifespan = 1.0;
            ListType = typeof(CategoryRail);
            TblName = "tblCategories";
            DbName = "Rail_DB";
            allowStoreToCache = false;
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("CategID", "CategID", true, false);
            dBFieldMappings.AddMapping("SystemID", "SystemID");
            dBFieldMappings.AddMapping("GroupID", "GroupID");
            dBFieldMappings.AddMapping("ReleaseWeight", "ReleaseWeight");
            dBFieldMappings.AddMapping("WeightMin", "WeightMin");
            dBFieldMappings.AddMapping("WeightApproach", "WeightApproach");
            dBFieldMappings.AddMapping("WeightOver", "WeightOver");
            dBFieldMappings.AddMapping("WeightPerPiece", "WeightPerPiece");
            dBFieldMappings.AddMapping("UseDefaultPercentWeights", "UseDefaultPercentWeights");
            dBFieldMappings.AddMapping("BatchSize", "BatchSize");
            dBFieldMappings.AddMapping("Aux1", "Aux1");
            dBFieldMappings.AddMapping("Aux2", "Aux2");
            dBFieldMappings.AddMapping("DynamicPercent", "DynamicPercent");
            dBFieldMappings.AddMapping("DynamicDryTime", "DynamicDryTime");
            dBFieldMappings.AddMapping("IsGrouped", "IsGrouped");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }


    public class CategoryRail : DataItem
    {
        # region private
        private int categID;
        private int systemID;
        private int groupID;
        private int releaseWeight;
        private int weightMin;
        private int weightApproach;
        private int weightOver;
        private double weightPerPiece;
        private bool useDefaultPercentWeights;
        private int batchSize;
        private int aux1;
        private int aux2;
        private double dynamicPercent;
        private double dynamicDryTime;
        private int isGrouped;
        #endregion

        public int CategID
        {
            get { return categID; }
            set
            {
                categID = AssignNotify(ref categID, value, "CategID");
                ID = Int32sTo64(systemID, categID);
                PrimaryKey = ID;
            }
        }
        public int SystemID
        {
            get { return systemID; }
            set
            {
                systemID = AssignNotify(ref systemID, value, "SystemID");
                ID = Int32sTo64(systemID, categID);
                PrimaryKey = ID;
            }
        }
        public int GroupID
        {
            get { return groupID; }
            set { groupID = AssignNotify(ref groupID, value, "GroupID"); }
        }
        public int ReleaseWeight
        {
            get { return releaseWeight; }
            set { releaseWeight = AssignNotify(ref releaseWeight, value, "ReleaseWeight"); }
        }
        public int WeightMin
        {
            get { return weightMin; }
            set { weightMin = AssignNotify(ref weightMin, value, "WeightMin"); }
        }
        public int WeightApproach
        {
            get { return weightApproach; }
            set { weightApproach = AssignNotify(ref weightApproach, value, "WeightApproach"); }
        }
        public int WeightOver
        {
            get { return weightOver; }
            set { weightOver = AssignNotify(ref weightOver, value, "WeightOver"); }
        }
        public double WeightPerPiece
        {
            get { return weightPerPiece; }
            set { weightPerPiece = AssignNotify(ref weightPerPiece, value, "WeightPerPiece"); }
        }
        public bool UseDefaultPercentWeights
        {
            get { return useDefaultPercentWeights; }
            set { useDefaultPercentWeights = AssignNotify(ref useDefaultPercentWeights, value, "UseDefaultPercentWeights"); }
        }
        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = AssignNotify(ref batchSize, value, "BatchSize"); }
        }
        public int Aux1
        {
            get { return aux1; }
            set { aux1 = AssignNotify(ref aux1, value, "Aux1"); }
        }
        public int Aux2
        {
            get { return aux2; }
            set { aux2 = AssignNotify(ref aux2, value, "Aux2"); }
        }
        public double DynamicPercent
        {
            get { return dynamicPercent; }
            set { dynamicPercent = AssignNotify(ref dynamicPercent, value, "DynamicPercent"); }
        }
        public double DynamicDryTime
        {
            get { return dynamicDryTime; }
            set { dynamicDryTime = AssignNotify(ref dynamicDryTime, value, "DynamicDryTime"); }
        }
        public int IsGrouped
        {
            get { return isGrouped; }
            set { isGrouped = AssignNotify(ref isGrouped, value, "IsGrouped"); }
        }

    }

    #endregion

    #region WeightConfigurations 
    //created in error probably never used!
    public class WeightConfigurations : DataList
    {
        public WeightConfigurations()
        {
            Lifespan = 1.0;
            ListType = typeof(WeightConfiguration);
            TblName = "tblWeightConfiguration";
            DbName = "Rail_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("WeightID", "WeightID");
            dBFieldMappings.AddMapping("Description_GB", "DescriptionGB");
            dBFieldMappings.AddMapping("Description_Local", "DescriptionLocal");
            dBFieldMappings.AddMapping("Setting", "Setting");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class WeightConfiguration : DataItem
    {
        private int recNum;
        private int weightID;
        private string descriptionGB;
        private string descriptionLocal;
        private int setting;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = recNum;
                PrimaryKey = ID;
            }
        }
 
        public int WeightID
        {
            get { return weightID; }
            set { weightID = AssignNotify(ref weightID, value, "WeightID"); }
        }

        public string DescriptionGB
        {
            get { return descriptionGB; }
            set { descriptionGB = AssignNotify(ref descriptionGB, value, "DescriptionGB"); }
        }

        public string DescriptionLocal
        {
            get { return descriptionLocal; }
            set { descriptionLocal = AssignNotify(ref descriptionLocal, value, "DescriptionLocal"); }
        }

        public int Setting
        {
            get { return setting; }
            set { setting = AssignNotify(ref setting, value, "Setting"); }
        }

    }

    #endregion


}
