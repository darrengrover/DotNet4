using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dynamic.DataLayer
{

    #region BatchDetails

    public class BatchDetails : DataList
    {
        public BatchDetails()
        {
            Lifespan = 1.0;
            ListType = typeof(BatchDetail);
            TblName = "tblBatchDetails";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Created", "Created");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            dBFieldMappings.AddMapping("SourceSubID", "SourceSubID");
            dBFieldMappings.AddMapping("BatchID", "BatchID");
            dBFieldMappings.AddMapping("Customer_IDJensen", "CustomerID");
            dBFieldMappings.AddMapping("SortCategory_IDJensen", "SortCategoryID");
            dBFieldMappings.AddMapping("Weight_kg", "Weight");
            dBFieldMappings.AddMapping("ArticleCount", "ArticleCount");
            dBFieldMappings.AddMapping("Article_IDJensen", "ArticleID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class BatchDetail : DataItem
    {
        private int recNum;
        private DateTime created;
        private int sourceID;
        private int sourceSubID;
        private int batchID;
        private int customerID;
        private int sortCategoryID;
        private double weight;
        private int articleCount;
        private int articleID;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = recNum;
                PrimaryKey = recNum;
            }
        }

        public DateTime Created
        {
            get { return created; }
            set { created = AssignNotify(ref created, value, "Created"); }
        }

        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(ref sourceID, value, "SourceID"); }
        }

        public int SourceSubID
        {
            get { return sourceSubID; }
            set { sourceSubID = AssignNotify(ref sourceSubID, value, "SourceSubID"); }
        }

        public int BatchID
        {
            get { return batchID; }
            set { batchID = AssignNotify(ref batchID, value, "BatchID"); }
        }

        public int CustomerID
        {
            get { return customerID; }
            set { customerID = AssignNotify(ref customerID, value, "CustomerID"); }
        }

        public int SortCategoryID
        {
            get { return sortCategoryID; }
            set { sortCategoryID = AssignNotify(ref sortCategoryID, value, "SortCategoryID"); }
        }

        public double Weight
        {
            get { return weight; }
            set { weight = AssignNotify(ref weight, value, "Weight"); }
        }

        public int ArticleCount
        {
            get { return articleCount; }
            set { articleCount = AssignNotify(ref articleCount, value, "ArticleCount"); }
        }

        public int ArticleID
        {
            get { return articleID; }
            set { articleID = AssignNotify(ref articleID, value, "ArticleID"); }
        }
    }

    #endregion

    #region BatchRealTime

    public class BatchRealTime : DataList
    {
        public BatchRealTime()
        {
            Lifespan = 1.0;
            ListType = typeof(BatchRealTimeRec);
            TblName = "tblBatchRealTime";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("MachineID", "MachineID");
            dBFieldMappings.AddMapping("PositionID", "PositionID");
            dBFieldMappings.AddMapping("BatchID", "BatchID");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class BatchRealTimeRec : DataItem
    {
        private int recNum;
        private int machineID;
        private int positionID;
        private int batchID;
        private int sourceID;

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                ID = recNum;
                PrimaryKey = recNum;
            }
        }

        public int MachineID
        {
            get { return machineID; }
            set { machineID = AssignNotify(ref machineID, value, "MachineID"); }
        }

        public int PositionID
        {
            get { return positionID; }
            set { positionID = AssignNotify(ref positionID, value, "PositionID"); }
        }

        public int BatchID
        {
            get { return batchID; }
            set { batchID = AssignNotify(ref batchID, value, "BatchID"); }
        }

        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(ref sourceID, value, "SourceID"); }
        }
    }
 
    #endregion

    #region BatchCompound

    public class BatchCompound : DataList
    {
        public BatchCompound()
        {
            Lifespan = 1.0;
            ListType = typeof(BatchCompoundRec);
            TblName = "tblBatchCompound";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Parent_BatchID", "ParentBatchID");
            dBFieldMappings.AddMapping("Parent_SourceID", "ParentSourceID");
            dBFieldMappings.AddMapping("Child_Index", "ChildIndex");
            dBFieldMappings.AddMapping("Child_BatchID", "ChildBatchID");
            dBFieldMappings.AddMapping("Child_SourceID", "ChildSourceID");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class BatchCompoundRec : DataItem
    {
        private int recnum;
        private int parentBatchID;
        private int parentSourceID;
        private int childIndex;
        private int childBatchID;
        private int childSourceID;

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

        public int ParentBatchID
        {
            get { return parentBatchID; }
            set { parentBatchID = AssignNotify(ref parentBatchID, value, "ParentBatchID"); }
        }

        public int ParentSourceID
        {
            get { return parentSourceID; }
            set { parentSourceID = AssignNotify(ref parentSourceID, value, "ParentSourceID"); }
        }

        public int ChildIndex
        {
            get { return childIndex; }
            set { childIndex = AssignNotify(ref childIndex, value, "ChildIndex"); }
        }

        public int ChildBatchID
        {
            get { return childBatchID; }
            set { childBatchID = AssignNotify(ref childBatchID, value, "ChildBatchID"); }
        }

        public int ChildSourceID
        {
            get { return childSourceID; }
            set { childSourceID = AssignNotify(ref childSourceID, value, "ChildSourceID"); }
        }
    }

    #endregion

    #region BatchDetailsPLC

    public class BatchDetailsPLC : DataList
    {
        public BatchDetailsPLC()
        {
            Lifespan = 1.0;
            ListType = typeof(BatchDetailsPLCRec);
            TblName = "tblBatchDetailsPLC";
            DbName = "Rail_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", true, true);
            dBFieldMappings.AddMapping("Timestamp", "Timestamp");
            dBFieldMappings.AddMapping("SystemID", "SystemID");
            dBFieldMappings.AddMapping("BatchID", "BatchID");
            dBFieldMappings.AddMapping("SourceID", "SourceID");
            dBFieldMappings.AddMapping("SourceSubID", "SourceSubID");            
            dBFieldMappings.AddMapping("StoreLocation", "StoreLocation");
            dBFieldMappings.AddMapping("Grouping", "Grouping");
            dBFieldMappings.AddMapping("DropLocation", "DropLocation");
            dBFieldMappings.AddMapping("Call_ID", "CallID");
            dBFieldMappings.AddMapping("Overweight", "Overweight");
            dBFieldMappings.AddMapping("BagEmpty", "BagEmpty");
            dBFieldMappings.AddMapping("BagClosed", "BagClosed");
            dBFieldMappings.AddMapping("ReverseBinUnload", "ReverseBinUnload");
            dBFieldMappings.AddMapping("BagDeleted", "BagDeleted");
            dBFieldMappings.AddMapping("BagOpened", "BagOpened");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class BatchDetailsPLCRec : DataItem
    {
        private int recnum;
        private DateTime timeStamp;
        private int systemID;
        private int batchID;
        private int sourceID;
        private int sourceSubID;
        private int storeLocation;
        private int grouping;
        private int dropLocation;
        private int callID;
        private bool overweight;
        private bool bagEmpty;
        private bool bagClosed;
        private bool reverseBinUnload;
        private bool bagDeleted;
        private DateTime bagOpened;

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

        public DateTime Timestamp
        {
            get { return timeStamp; }
            set { timeStamp = AssignNotify(ref timeStamp, value, "Timestamp"); }
        }

        public int SystemID
        {
            get { return systemID; }
            set { systemID = AssignNotify(ref systemID, value, "SystemID"); }
        }

        public int BatchID
        {
            get { return batchID; }
            set { batchID = AssignNotify(ref batchID, value, "BatchID"); }
        }

        public int SourceID
        {
            get { return sourceID; }
            set { sourceID = AssignNotify(ref sourceID, value, "SourceID"); }
        }

        public int SourceSubID
        {
            get { return sourceSubID; }
            set { sourceSubID = AssignNotify(ref sourceSubID, value, "SourceSubID"); }
        }

        public int StoreLocation
        {
            get { return storeLocation; }
            set { storeLocation = AssignNotify(ref storeLocation, value, "StoreLocation"); }
        }

        public int Grouping
        {
            get { return grouping; }
            set { grouping = AssignNotify(ref grouping, value, "Grouping"); }
        }

        public int DropLocation
        {
            get { return dropLocation; }
            set { dropLocation = AssignNotify(ref dropLocation, value, "DropLocation"); }
        }

        public int CallID
        {
            get { return callID; }
            set { callID = AssignNotify(ref callID, value, "CallID"); }
        }

        public bool Overweight
        {
            get { return overweight; }
            set { overweight = AssignNotify(ref overweight, value, "Overweight"); }
        }

        public bool BagEmpty
        {
            get { return bagEmpty; }
            set { bagEmpty = AssignNotify(ref bagEmpty, value, "BagEmpty"); }
        }

        public bool BagClosed
        {
            get { return bagClosed; }
            set { bagClosed = AssignNotify(ref bagClosed, value, "BagClosed"); }
        }

        public bool ReverseBinUnload
        {
            get { return reverseBinUnload; }
            set { reverseBinUnload = AssignNotify(ref reverseBinUnload, value, "ReverseBinUnload"); }
        }

        public bool BagDeleted
        {
            get { return bagDeleted; }
            set { bagDeleted = AssignNotify(ref bagDeleted, value, "BagDeleted"); }
        }

        public DateTime BagOpened
        {
            get { return bagOpened; }
            set { bagOpened = AssignNotify(ref bagOpened, value, "BagOpened"); }
        }

    }

    #endregion

}
