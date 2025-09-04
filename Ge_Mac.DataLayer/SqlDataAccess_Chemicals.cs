using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {

        #region caches
        public Chemicals ChemicalsCache = null;

        #endregion
        #region Select Data

        //chemical log data
        public ChemicalLogData GetChemicalLogData(DateTime startTime, DateTime endTime)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,bd.SortCategory_idJensen as SortCategoryID
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld, [dbo].tblBatchDetails bd
              WHERE [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime
                AND ld.RegType = 2 
				and bd.BatchID=ld.BatchID
				and bd.SourceID=ld.SourceID                
                AND ld.SubRegType = 50 --chemicals
              ORDER BY MachineID, BatchID, MessageA";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    ChemicalLogData chemicalLogData = new ChemicalLogData();
                    ChemicalsCache = new Chemicals();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(chemicalLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return chemicalLogData;
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

        public ChemicalLogData GetChemicalLogData(DateTime startTime, DateTime endTime, int batchID, int sourceID)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            string commandString =
               @"SELECT ld.[recNum]
                  ,ld.[RemoteID]
                  ,ld.[CompanyID]
                  ,ld.[TimeStamp]
                  ,ld.[MachineID]
                  ,ld.[PositionID]
                  ,ld.[SubID]
                  ,ld.[SubIDName]
                  ,ld.[RegType]
                  ,ld.[SubRegType]
                  ,ld.[SubRegTypeID]
                  ,ld.[State]
                  ,ld.[MessageA]
                  ,ld.[MessageB]
                  ,ld.[BatchID]
                  ,ld.[SourceID]
                  ,ld.[ProcessCode]
                  ,ld.[ProcessName]
                  ,ld.[CustNo]
                  ,ld.[SortCategoryID]
                  ,ld.[ArtNo]
                  ,ld.[OperatorNo]
                  ,ld.[Value]
                  ,ld.[Unit]
              FROM [dbo].[tblJGLogData] ld
              WHERE [TimeStamp]>=@StartTime
                AND [TimeStamp]<@EndTime
                AND ld.[BatchID] = @BatchID
                AND ld.[SourceID] = @SourceID
                AND ld.RegType = 2 
                AND ld.SubRegType = 50 --chemicals
              ORDER BY MachineID, BatchID, MessageA";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    ChemicalLogData chemicalLogData = new ChemicalLogData();
                    ChemicalsCache = new Chemicals();
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);
                    command.Parameters.AddWithValue("@BatchID", batchID);
                    command.Parameters.AddWithValue("@SourceID", sourceID);
                    SqlDataConnection.Timeout = 120;
                    command.DataFill(chemicalLogData, SqlDataConnection.DBConnection.JensenGroup);
                    return chemicalLogData;
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

    public class Chemicals : List<Chemical>
    {
        public void AddUpdate(int id, string localName, string englishName)
        {
            Chemical chem = getByID(id);
            {
                if (chem != null)
                {
                    if (!String.IsNullOrEmpty(localName))
                        chem.ChemicalNameLocal = localName;
                    if (!String.IsNullOrEmpty(englishName))
                        chem.ChemicalNameEnglish = englishName;
                }
                else
                {
                    chem = new Chemical()
                    {
                        ChemicalID = id,
                        ChemicalNameLocal = localName,
                        ChemicalNameEnglish = englishName
                    };
                    this.Add(chem);
                }
            }
        }

        public Chemical getByID(int id)
        {
            return this.Find(delegate(Chemical chem)
            {
                return chem.ChemicalID == id;
            });
        }
    }

    public class ChemicalBatches : List <ChemicalBatch>
    { }

    public class ChemicalLogData : List<ChemicalLog>, IDataFiller
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
                    DateTime testTime = lastRead.AddHours(lifespan);
                    test = testTime > da.ServerTime;
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
            int RecNumPos = dr.GetOrdinal("RecNum");
            int RemoteIDPos = dr.GetOrdinal("RemoteID");
            int CompanyIDPos = dr.GetOrdinal("CompanyID");
            int TimeStampPos = dr.GetOrdinal("TimeStamp");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int PositionIDPos = dr.GetOrdinal("PositionID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int SubIDNamePos = dr.GetOrdinal("SubIDName");
            int RegTypePos = dr.GetOrdinal("RegType");
            int SubRegTypePos = dr.GetOrdinal("SubRegType");
            int SubRegTypeIDPos = dr.GetOrdinal("SubRegTypeID");
            int StatePos = dr.GetOrdinal("State");
            int MessageAPos = dr.GetOrdinal("MessageA");
            int MessageBPos = dr.GetOrdinal("MessageB");
            int BatchIDPos = dr.GetOrdinal("BatchID");
            int SourceIDPos = dr.GetOrdinal("SourceID");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int ProcessNamePos = dr.GetOrdinal("ProcessName");
            int CustNoPos = dr.GetOrdinal("CustNo");
            int SortCategoryIDPos = dr.GetOrdinal("SortCategoryID");
            int ArtNoPos = dr.GetOrdinal("ArtNo");
            int OperatorNoPos = dr.GetOrdinal("OperatorNo");
            int ValuePos = dr.GetOrdinal("Value");
            int UnitPos = dr.GetOrdinal("Unit");

            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines(false);

            BatchDetailsList bdl = da.GetBatchesLight();

            while (dr.Read())
            {
                ChemicalLog chemicalLog = new ChemicalLog(bdl);
                try
                {
                    int subid = dr.GetInt32(SubIDPos);
                    int machineID = dr.GetInt32(MachineIDPos);
                    if (subid > 0)
                    {
                        int positions = 0;
                        Machine m = ms.GetById(machineID);
                        if (m != null)
                        {
                            positions = m.Positions;
                        }
                        if ((subid > positions) && (subid < 100))
                        {
                            subid = positions;
                        }
                    }
                    chemicalLog.RecNum = dr.GetInt32(RecNumPos);
                    chemicalLog.CompanyID = dr.GetInt32(CompanyIDPos);
                    chemicalLog.TimeStamp = dr.GetDateTime(TimeStampPos);
                    chemicalLog.MachineID = machineID;
                    chemicalLog.PositionID = dr.GetInt32(PositionIDPos);
                    chemicalLog.ChemicalID = dr.GetInt32(SubRegTypeIDPos);
                    chemicalLog.ChemicalNameLocal = dr.IsDBNull(MessageAPos) ? string.Empty : dr.GetString(MessageAPos);
                    chemicalLog.ChemicalNameEnglish = dr.IsDBNull(MessageBPos) ? string.Empty : dr.GetString(MessageBPos);
                    chemicalLog.BatchID = dr.GetInt32(BatchIDPos);
                    chemicalLog.SourceID = dr.GetInt32(SourceIDPos);
                    //chemicalLog.SourceID = 2; //temp fix
                    chemicalLog.CustomerID = dr.GetInt32(CustNoPos);
                    chemicalLog.SortCategoryID = dr.GetInt32(SortCategoryIDPos);
                    chemicalLog.Value = dr.GetDecimal(ValuePos);
                    chemicalLog.Unit = dr.GetInt32(UnitPos);
                    chemicalLog.BuildBatchInfo();
                    chemicalLog.HasChanged = false;

                    this.Add(chemicalLog);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return this.Count;
        }


        public void SetGroupType(int groupType)
        {
            foreach (ChemicalLog cl in this)
            {
                cl.DataType = groupType; 
            }
        }

 
    }

    public class Chemical
    {
        public int ChemicalID { get; set; }
        public string ChemicalNameLocal { get; set; }
        public string ChemicalNameEnglish { get; set; }
    }

    public class ChemicalBatch
    {
        public int ChemicalID { get; set; }
        public int BatchID { get; set; }
        public int SourceID { get; set; }
        public decimal Total { get; set; }
        public int Unit { get; set; }

        public string ChemicalNameLocal
        {
            get
            {
                return GetChemicalNameLocal();
            }
        }

        private string GetChemicalNameLocal()
        {
            string aName = string.Empty;
            SqlDataAccess da = SqlDataAccess.Singleton;
            Chemical chem = da.ChemicalsCache.getByID(ChemicalID);
            if (chem != null)
            {
                aName = chem.ChemicalNameLocal;
            }
            return aName;
        }                
    }

    public class ChemicalLog : DataItem, IComparable<ChemicalLog>
    {
        #region Customer Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal DateTime TimeStamp;
            internal int CompanyID;
            internal int MachineID;
            internal int CustomerID;
            internal int PositionID;
            internal int ChemicalID;
            internal int BatchID;
            internal int SourceID;
            internal decimal Value;
            internal int Unit;
            internal string ChemicalTranslation;
            internal string MachineTranslation;
            internal string QtyTranslation;
            internal int DataType;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ChemicalLog(BatchDetailsList batchDetailsList)
        {
            ActiveData = (ICopyableObject)new DataRecord();
            BatchdetailsList = batchDetailsList;
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
        #region crystal bodge fields

        public int DataType
        {
            get
            {
                return activeData.DataType;
            }
            set
            {
                activeData.DataType = value;
            }
        }

        public string GroupName
        {
            get
            {
                string aName = MachineName;
                if (DataType == 1)
                {
                    aName = CustomerName;
                }                
                if (DataType == 2)
                {
                    aName = SortCategoryName;
                }
                return aName;
            }
        }

        public int One
        {
            get
            {
                return 1;
            }
        }

        public string ChemicalTranslation
        {
            get
            {
                return this.activeData.ChemicalTranslation;
            }
            set
            {
                this.activeData.ChemicalTranslation = value;
            }
        }

        public string MachineTranslation
        {
            get
            {
                return this.activeData.MachineTranslation;
            }
            set
            {
                this.activeData.MachineTranslation = value;
            }
        }

        public string GroupTranslation
        {
            get
            {
                return this.activeData.MachineTranslation;
            }
            set
            {
                this.activeData.MachineTranslation = value;
            }
        }

        public string QtyTranslation
        {
            get
            {
                return this.activeData.QtyTranslation;
            }
            set
            {
                this.activeData.QtyTranslation = value;
            }
        }
 
        #endregion

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

        public int CompanyID
        {
            get
            {
                return this.activeData.CompanyID;
            }
            set
            {
                this.activeData.CompanyID = value;
                HasChanged = true;
            }
        }

        public int CustomerID
        {
            get { return this.activeData.CustomerID; }
            set
            {
                this.activeData.CustomerID = value;
                HasChanged = true; 
            }
        }

        public string CustomerName
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Customer c = da.GetCustomer(activeData.CustomerID);
                if (c != null)
                    aString = c.ShortDescAndID;
                else
                    aString = "(" + activeData.CustomerID.ToString() + ")";
                return aString;
            }
        }
        
        private int sortCategoryID;
        public int SortCategoryID
        {
            get { return sortCategoryID; }
            set
            {
                sortCategoryID = value;
            }
        }

        public string SortCategoryName
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                SortCategory c = da.GetSortCategory(SortCategoryID);
                if (c != null)
                    aString = c.ShortDescAndID;
                else
                    aString = "(" + SortCategoryID.ToString() + ")";
                return aString;
            }
        }
        
        private double weight;
        public double Weight
        {
            get { return weight; }
            set
            {
                weight = value;
            }
        }

        public int PositionID
        {
            get
            {
                return this.activeData.PositionID;
            }
            set
            {
                this.activeData.PositionID = value;
                HasChanged = true;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return this.activeData.TimeStamp;
            }
            set
            {
                this.activeData.TimeStamp = value;
                HasChanged = true;
            }
        }

        public int MachineID
        {
            get
            {
                return this.activeData.MachineID;
            }
            set
            {
                this.activeData.MachineID = value;
                HasChanged = true;
            }
        }

        public string MachineName
        {
            get
            {
                string aString = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.MachineID);
                if (m != null)
                    aString = m.ShortDescAndID;
                else
                    aString = "(" + this.activeData.MachineID.ToString() + ")";
                return aString;
            }
        }

        public int BatchID
        {
            get
            {
                return this.activeData.BatchID;
            }
            set
            {
                this.activeData.BatchID = value;
                HasChanged = true;
            }
        }

        public string BatchSourceIDs
        {
            get
            {
                string aString = BatchID.ToString() + " / " + SourceID.ToString();
                return aString;
            }
        }

        public int ChemicalID
        {
            get
            {
                return this.activeData.ChemicalID;
            }
            set
            {
                this.activeData.ChemicalID = value;
                HasChanged = true;
            }
        }

        public string ChemicalNameLocal
        {
            get
            {
                string aName = string.Empty;
                //aName = "Chemical " + ChemicalID.ToString();
                SqlDataAccess da = SqlDataAccess.Singleton;
                Chemical chemical = da.ChemicalsCache.getByID(ChemicalID);
                if (chemical != null)
                    aName = chemical.ChemicalNameLocal;
                return aName;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.ChemicalsCache != null)
                {
                    da.ChemicalsCache.AddUpdate(ChemicalID, value, string.Empty);
                }
            }
        }

        public string ChemicalNameEnglish
        {
            get
            {
                string aName = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Chemical chemical = da.ChemicalsCache.getByID(ChemicalID);
                if (chemical != null)
                    aName = chemical.ChemicalNameEnglish;
                return aName;
            }
            set
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (da.ChemicalsCache != null)
                {
                    da.ChemicalsCache.AddUpdate(ChemicalID, string.Empty, value);
                }
            }
        }

        public int SourceID
        {
            get
            {
                return this.activeData.SourceID;
            }
            set
            {
                this.activeData.SourceID = value;
                HasChanged = true;
            }
        }

        public decimal Value
        {
            get
            {
                return this.activeData.Value;
            }
            set
            {
                this.activeData.Value = value;
                HasChanged = true;
            }
        }

        public int Unit
        {
            get
            {
                return this.activeData.Unit;
            }
            set
            {
                this.activeData.Unit = value;
                HasChanged = true;
            }
        }

        public string UnitStr
        {
            get
            {
                string aString = string.Empty;
                switch (this.activeData.Unit)
                {
                    case 25:
                        aString = "s";
                        break;
                    case 29:
                        aString = "g";
                        break;
                    case 35:
                        aString = "l";
                        break;
                    case 47:
                        aString = "ml";
                        break;
                    case 50:
                        aString = "gal";
                        break;
                    case 73:
                        aString = "oz";
                        break;
                    case 75:
                        aString = "fl.oz";
                        break;
                    default:
                        aString = ".";
                        break;
                }
                return aString;
            }
        }

        private string batchInfo = string.Empty;

        public string BatchInfo
        {
            get { return batchInfo; }
            set { batchInfo = value; }
        }

        public BatchDetailsList BatchdetailsList { get; set; }

        public void BuildBatchInfo()
        {
            batchInfo = BatchID.ToString() + " / " + SourceID.ToString();
            if ((BatchID > 0) && (SourceID > 0))
            {
                if (BatchdetailsList != null)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    //batch info
                    BatchDetails bd = BatchdetailsList.GetById(SourceID, BatchID);
                    if (bd != null)
                    {
                        this.activeData.CustomerID = bd.Customer_idJensen;
                        sortCategoryID = bd.SortCategory_idJensen;
                        weight = (double)bd.Weight_Kg;
                    }
                    //customer info
                    Customers cs = da.GetAllActiveCustomers();
                    Customer c = null;
                    if (cs != null)
                    {
                        c = cs.GetById(CustomerID);
                        if (c != null)
                            batchInfo += ". " + c.LongDescription + "(" + CustomerID.ToString() + ")";
                        else
                            batchInfo += ". [" + CustomerID.ToString() + "]";
                    }
                    else batchInfo += ". [" + CustomerID.ToString() + "]";

                    //Sort Category info
                    SortCategories scs = da.GetAllActiveSortCategories();
                    SortCategory sc;
                    if (scs != null)
                    {
                        sc = scs.GetById(SortCategoryID);
                        if (sc != null)
                            batchInfo += " / " + sc.LongDescription + "(" + SortCategoryID.ToString() + ")";
                        else
                            batchInfo += " / [" + CustomerID.ToString() + "]";
                    }
                    else batchInfo += " / [" + CustomerID.ToString() + "]";

                    batchInfo += " " + weight.ToString() + " kg";
                }
            }
        }

        #region Sorting
        public static Comparison<ChemicalLog> NameComparison =
            delegate(ChemicalLog c1, ChemicalLog c2)
            {
                return c1.ChemicalNameLocal.CompareTo(c2.ChemicalNameLocal);
            };

        public static Comparison<ChemicalLog> IDComparison =
            delegate(ChemicalLog c1, ChemicalLog c2)
            {
                return c1.ChemicalID.CompareTo(c2.ChemicalID);
            };

        public int CompareTo(ChemicalLog other) { return ChemicalNameLocal.CompareTo(other.ChemicalNameLocal); }
        #endregion
    }
}
