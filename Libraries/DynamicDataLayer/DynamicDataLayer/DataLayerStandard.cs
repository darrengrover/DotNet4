using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace Dynamic.DataLayer
{
    #region Articles

    partial class SqlDataAccess
    {
        public Articles GetArticles(Articles recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Articles();
            recs = (Articles)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }


    }

    public class Articles : DataList
    {
        public Articles()
        {
            Lifespan = 1.0;
            ListType = typeof(Article);
            TblName = "tblArticles";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("IdJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("QuickRef", "QuickRef");
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            dBFieldMappings.AddMapping("Retired", "Retired");
            dBFieldMappings.AddMapping("Weight_KG", "Weight_KG");
            dBFieldMappings.AddMapping("ArticleGroup", "ArticleGroup");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class Article : DataItem
    {
        #region private fields

        private int recNum;
        private int idJensen;
        private int quickRef;
        private string extRef;
        private string shortDescription;
        private string longDescription;
        private int backColour;
        private int foreColour;
        private bool retired;
        private double weight_KG;
        private int articleGroup;

        #endregion

        #region Data Column Properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = value;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public int QuickRef
        {
            get { return quickRef; }
            set { quickRef = AssignNotify(ref quickRef, value, "QuickRef"); }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string ArticleName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "ArticleName"); }
        }

        public string ArticleNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        public bool Retired
        {
            get { return retired; }
            set
            {
                retired = AssignNotify(ref retired, value, "Retired");
                Active = !retired;
            }
        }

        public double Weight_KG
        {
            get { return weight_KG; }
            set { weight_KG = AssignNotify(ref weight_KG, value, "Weight_KG"); ; }
        }

        public int ArticleGroup
        {
            get { return articleGroup; }
            set { articleGroup = AssignNotify(ref articleGroup, value, "ArticleGroup"); ; }
        }


        #endregion
    }

    #endregion


    #region ArticleGroups

    public class ArticleGroups : DataList
    {
        public ArticleGroups()
        {
            Lifespan = 1.0;
            ListType = typeof(ArticleGroup);
            TblName = "tblArticleGroups";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("idJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            //ArticleGroup dataItem = (ArticleGroup)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class ArticleGroup : DataItem
    {
        #region private fields

        private int recNum = -1;
        private int idJensen = -1;
        private string extRef = string.Empty;
        private string shortDescription = string.Empty;
        private string longDescription = string.Empty;
        private int backColour = 0;
        private int foreColour = 0;

        #endregion

        #region properties
        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = recNum;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string ArticleGroupName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "ArticleGroupName"); }
        }

        public string ArticleGroupNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        #endregion
    }

    #endregion


    #region Customers

    partial class SqlDataAccess
    {

        public Customers GetCustomers(Customers recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Customers();
            recs = (Customers)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }
            
    }

    public class Customers : DataList
    {
        public Customers()
        {
            Lifespan = 1.0;
            ListType = typeof(Customer);
            TblName = "tblCustomers";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("idJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("QuickRef", "QuickRef");
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            dBFieldMappings.AddMapping("Retired", "Retired");
            //Customer dataItem = (Customer)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }


    }

    public class Customer : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private int recNum = -1;
        private int idJensen = -1;
        private int quickRef = -1;
        private string extRef = string.Empty;
        private int customerType = 0;
        private string shortDescription = string.Empty;
        private string longDescription = string.Empty;
        private int backColour = 0;
        private int foreColour = 0;
        private bool retired = false;

        #endregion

        #region Data Column Properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = recNum;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public int QuickRef
        {
            get { return quickRef; }
            set { quickRef = AssignNotify(ref quickRef, value, "QuickRef"); }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public int CustomerType
        {
            get { return customerType; }
            set { customerType = AssignNotify(ref customerType, value, "CustomerType"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string CustomerName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "CustomerName"); }
        }

        public string CustomerNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        public bool Retired
        {
            get { return retired; }
            set
            {
                retired = AssignNotify(ref retired, value, "Retired");
                Active = !retired;
            }
        }



        #endregion
    }

    #endregion


    #region Operators


    partial class SqlDataAccess
    {
        public Operators GetOperators(Operators recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new Operators();
            recs = (Operators)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

    }

    public class Operators : DataList
    {
        private Tags operatorTags = null;

        public Tags OperatorTags
        {
            get { return operatorTags; }
            set
            {
                operatorTags = value;
                foreach (Operator rec in this)
                    rec.Tags = operatorTags;
            }
        }

        public Operators()
        {
            Lifespan = 0.25;
            ListType = typeof(Operator);
            TblName = "tblOperators";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("idJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("QuickRef", "QuickRef");
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            dBFieldMappings.AddMapping("Retired", "Retired");
            Operator dataItem = (Operator)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void AddOperator()
        {
            Operator rec = new Operator();
            rec.RecNum = -1;
            rec.IdJensen = GetNextID();
            rec.QuickRef = -1;
            rec.ExtRef = string.Empty;
            rec.ShortDescription = "New Operator " + rec.IdJensen;
            rec.LongDescription = rec.ShortDescription;
            rec.ForceNew = true;
            rec.Tags = operatorTags;
            this.Add(rec);
        }

        public override Boolean UpdateToDB()
        {
            bool test = base.UpdateToDB();
            if (test && (OperatorTags != null))
                test = OperatorTags.UpdateToDB();
            return test;
        }

    }

    public class Operator : DataItem, INotifyPropertyChanged
    {
        #region private fields
        private int recNum = -1;
        private int idJensen = -1;
        private int quickRef = -1;
        private string extRef = string.Empty;
        private string shortDescription = string.Empty;
        private string longDescription = string.Empty;
        private int backColour = 0;
        private int foreColour = 0;
        private bool retired = false;
        #endregion

        #region Data Column Properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                //SqlDataAccess da = SqlDataAccess.Singleton;
                //if(da.DatabaseVersion < 2.0)
                    PrimaryKey = recNum;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
                //SqlDataAccess da = SqlDataAccess.Singleton;
                //if (da.DatabaseVersion >= 2.0)
                    PrimaryKey = idJensen;
                getTag();
            }
        }

        public int QuickRef
        {
            get { return quickRef; }
            set { quickRef = AssignNotify(ref quickRef, value, "QuickRef"); }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string OperatorName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "OperatorName"); }
        }

        public string OperatorNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        public bool Retired
        {
            get { return retired; }
            set
            {
                retired = AssignNotify(ref retired, value, "Retired");
                Active = !retired;
            }
        }

        #endregion

        #region permissions
        private Permissions fullUserPermissionsList = new Permissions();
        private OperatorPermissions operatorPermissionsFromDb;
        public OperatorPermissions OperatorPermissionsFromDb
        {
            get { return operatorPermissionsFromDb; }
            set
            {
                operatorPermissionsFromDb = value;
                getOperatorPermissions();
            }
        }
        private Permissions permissionsAvailableFromDb;
        public Permissions PermissionsAvailableFromDb
        {
            get { return permissionsAvailableFromDb; }
            set
            {
                permissionsAvailableFromDb = value;
                getOperatorPermissions();
            }
        }
        private void getOperatorPermissions()
        {
            if (operatorPermissionsFromDb != null && permissionsAvailableFromDb != null)
            {
                var thisOperatorPermissionsFromDb = operatorPermissionsFromDb.OfType<OperatorPermission>().FirstOrDefault(o => o.OperatorId == idJensen);
                fullUserPermissionsList.Clear();
                foreach (var dataItem in permissionsAvailableFromDb)
                {
                    var permission = dataItem as Permission;
                    if(permission != null)
                    {
                        var permissionToAdd = new Permission { PermissionName = permission.PermissionName };
                        if (thisOperatorPermissionsFromDb != null)
                            permissionToAdd.DefaultValue = thisOperatorPermissionsFromDb.IsAllowedTo(permission);
                        else
                            permissionToAdd.DefaultValue = permission.DefaultValue;

                        fullUserPermissionsList.Add(permissionToAdd);
                    }
                    
                }
            }
        }

        public bool IsAllowedToManuallyLogout
        {
            get 
            { 
                if(fullUserPermissionsList.Count > 0)
                    return fullUserPermissionsList.HasPermission("AllowLogOut");

                return QuickRef != 0; // zero will disable manaul logout.
            }
        }
        #endregion

        #region tag properties
        private Tag tag = null;
        private Tags tags = null;
        public Tags Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                getTag();
            }
        }
 
        public void ClearTagInfo()
        {
            tag = null;
            decimalCardID = string.Empty;
            hexCardID = string.Empty;
        }

        public void StoreTagInfo()
        {
            if (tags != null)
            {
                if (tag != null)
                {
                    if (!GetCardIDAlreadyInUse())
                    {
                        tag.TagData = hexCardID;
                        Debug.WriteLine(hexCardID);
                    }
                    else
                    {
                        Debug.WriteLine(decimalCardID + " already in use");
                    }
                }
                else
                {
                    if ((hexCardID != string.Empty) && (hexCardID != "00000000"))
                    {
                        if (tags != null)
                        {
                            tag = new Tag();
                            tag.TagID = -1;
                            tag.TagData = hexCardID;
                            tag.ReferenceID = idJensen;
                            tag.TagReferenceTable = "tblOperators";
                            tags.Add(tag);
                        }
                    }
                }
            }
        }

        public void RemoveTag()
        {
            if (tag != null)
            {
                if (tags != null)
                {
                    tag.TagData = "00000000";
                    tag.DeleteRecord = true;
                }
                tag = null;
            }
            decimalCardID = string.Empty;
            Notify("CardID");
        }

        private void getTag()
        {
            if (tags != null)
                tag = tags.GetTag(idJensen, "tblOperators");
            if (tag != null)
                decimalCardID = HexToDecimalCardID(tag.TagData);
            else
                decimalCardID = string.Empty;
        }

        private string HexToDecimalCardID(string hex)
        {
            string dec = string.Empty;
            if (hex.Length >= 8)
            {
                string a = hex.Substring(0, 2);
                string b = hex.Substring(2, 2);
                string c = hex.Substring(4, 2);
                string d = hex.Substring(6, 2);
                int ai = 0;
                int bi = 0;
                int ci = 0;
                int di = 0;
                int decAgain = int.Parse(a, System.Globalization.NumberStyles.HexNumber);
                int i;
                if (int.TryParse(a, System.Globalization.NumberStyles.HexNumber, null, out i))
                    ai = i;
                if (int.TryParse(b, System.Globalization.NumberStyles.HexNumber, null, out i))
                    bi = i;
                if (int.TryParse(c, System.Globalization.NumberStyles.HexNumber, null, out i))
                    ci = i;
                if (int.TryParse(d, System.Globalization.NumberStyles.HexNumber, null, out i))
                    di = i;
                int cardno = (bi * 256) + ai;
                int seriesno = (di * 256) + ci;
                dec = seriesno.ToString() + cardno.ToString("00000");
            }

            return dec;
        }

        private string DecimalToHexCardID(string dec)
        {
            int ai = 0;
            int bi = 0;
            int ci = 0;
            int di = 0;
            if (dec.Length > 5)
            {
                string cardnostr = dec.Substring(dec.Length - 5, 5);
                string seriesnostr = dec.Substring(0, dec.Length - 5);
                int cardno = 0;
                int seriesno = 0;
                int i;
                if (int.TryParse(cardnostr, out i))
                    cardno = i;
                if (int.TryParse(seriesnostr, out i))
                    seriesno = i;
                ai = cardno % 256;
                bi = cardno / 256;
                ci = seriesno % 256;
                di = seriesno / 256;
            }
            string hex = ai.ToString("X2") + bi.ToString("X2") + ci.ToString("X2") + di.ToString("X2");
            return hex;
        }

        private string getCardID()
        {
            string cardid = decimalCardID;
            if (tag != null)
            {
                hexCardID = tag.TagData;
                decimalCardID = HexToDecimalCardID(hexCardID);
            }
            return cardid;
        }

        private Boolean cardAlreadyInUse = false;

        public Boolean CardAlreadyInUse
        {
            get { return cardAlreadyInUse; }
            set { cardAlreadyInUse = value; }
        }

        private Boolean GetCardIDAlreadyInUse()
        {
            Boolean test = false;
            if (tags != null)
            {
                Tag t = tags.GetTag(hexCardID);
                test = ((t != null) && (t.ReferenceID != idJensen));
            }
            cardAlreadyInUse = test;
            return test;
        }

        private void setCardID(string decimalID)
        {
            string oldDecimal = decimalCardID;
            string oldHex = hexCardID;
            decimalCardID = decimalID;
            hexCardID = DecimalToHexCardID(decimalCardID);

            if (tag != null)
            {
                if (!GetCardIDAlreadyInUse())
                {
                    tag.TagData = hexCardID;
                    Debug.WriteLine(hexCardID);
                }
                else
                {
                    decimalCardID = oldDecimal;
                    hexCardID = oldHex;
                    Debug.WriteLine(decimalCardID + " already in use");
                }
            }
            else
            {
                if ((hexCardID != string.Empty) && (hexCardID != "00000000"))
                {
                    if (tags != null)
                    {
                        tag = new Tag();
                        tag.TagID = (int)tags.NextID();
                        tag.ForceNew = true;
                        tag.TagData = hexCardID;
                        tag.ReferenceID = idJensen;
                        tag.TagReferenceTable = "tblOperators";
                        tags.Add(tag);
                    }
                }
            }
        }

        public Boolean HasCardID
        {
            get { return tag != null; }
        }

        public Boolean CardIDValid
        {
            get
            {
                return (decimalCardID.Length > 5)
                    && (!CardAlreadyInUse);
            }
        }


        private string decimalCardID = string.Empty;
        private string hexCardID = string.Empty;

        public string CardID
        {
            get
            {
                return getCardID();
            }
            set
            {
                if (decimalCardID != value)
                {
                    setCardID(value);
                    HasChanged = true;
                    Notify("CardID");
                }
            }
        }
        #endregion
        public override string ToString()
        {
            return ShortDescription;
        }
    }

    #endregion


    #region ProcessCodes

    partial class SqlDataAccess
    {

        public ProcessCodes GetAllProcessCodes(ProcessCodes recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new ProcessCodes();
            recs = (ProcessCodes)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            recs.ValidateProcessCodes();
            return recs;
        }


    }

    public class ProcessCodes : DataList
    {
        Machines machines = null;
        public ProcessCodes()
        {
            Lifespan = 1.0;
            ListType = typeof(ProcessCode);
            TblName = "tblProcessCodes";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("Machine_idJensen", "Machine_idJensen");
            dBFieldMappings.AddMapping("Customer_idJensen", "Customer_idJensen");
            dBFieldMappings.AddMapping("Article_idJensen", "Article_idJensen");
            dBFieldMappings.AddMapping("SortCategory_idJensen", "SortCategory_idJensen");
            dBFieldMappings.AddMapping("ProcessCode", "ProcessCodeID");
            dBFieldMappings.AddMapping("ProcessName", "ProcessName");
            dBFieldMappings.AddMapping("Production_Norm", "Production_Norm");
            dBFieldMappings.AddMapping("Production_NoFlow_Time", "Production_NoFlow_Time");
            dBFieldMappings.AddMapping("Count_Exit_Point", "Count_Exit_Point");
            ProcessCode dataItem = (ProcessCode)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

        public void ValidateProcessCodes()
        {
            SqlDataAccess da=SqlDataAccess.Singleton;
            machines = da.GetAllMachines(machines, false);
            foreach (ProcessCode pc in this)
            {
                Machine machine = (Machine)machines.GetById(pc.Machine_idJensen);
                if (machine != null)
                {
                    if (machine.ProcessCodeType == 0)
                    {
                        pc.SortCategory_idJensen = -1;
                    }
                    if (machine.ProcessCodeType == 1)
                    {
                        pc.Article_idJensen = -1;
                    }
                }
            }
        }

        public ProcessCode GetProcessCode(int MachineID, int CustID, int ArtID, int SortID, int ProcCode)
        {
            ProcessCode rec = null;
            ProcessCode pc = null;
            int i = 0;
            while ((i < this.Count) && (pc == null))
            {
                rec = (ProcessCode)this[i];
                if ((rec.Machine_idJensen == MachineID)
                    && (rec.Customer_idJensen == CustID)
                    && (rec.Article_idJensen == ArtID)
                    && (rec.SortCategory_idJensen == SortID)
                    && (rec.ProcessCodeID == ProcCode))
                    pc = rec;
                i++;
            }
            return pc;
        }

        public ProcessCode GetByCustArtCatMachine(int custID, int artID, int catID, int machID)
        {
            ProcessCode rec = null;
            foreach (ProcessCode pc in this)
            {
                if ((pc.Customer_idJensen == custID)
                    && (pc.Article_idJensen == artID)
                    && (pc.SortCategory_idJensen == catID)
                    && (pc.Machine_idJensen == machID))
                    rec = pc;
            }
            return rec;
        }
    }

    public class ProcessCode : DataItem
    {
        #region private fields

        private int recNum;
        private int machine_idJensen;
        private int customer_idJensen;
        private int article_idJensen;
        private int sortCategory_idJensen;
        private int processCodeID;
        private string processName;
        private int production_Norm;
        private int production_NoFlow_Time;
        private bool count_Exit_Point;

        #endregion

        #region properties

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

        public int Machine_idJensen
        {
            get { return machine_idJensen; }
            set { machine_idJensen = AssignNotify(ref machine_idJensen, value, "Machine_idJensen"); }
        }

        public int Customer_idJensen
        {
            get { return customer_idJensen; }
            set { customer_idJensen = AssignNotify(ref customer_idJensen, value, "Customer_idJensen"); }
        }

        public int Article_idJensen
        {
            get { return article_idJensen; }
            set { article_idJensen = AssignNotify(ref article_idJensen, value, "Article_idJensen"); }
        }

        public int SortCategory_idJensen
        {
            get { return sortCategory_idJensen; }
            set { sortCategory_idJensen = AssignNotify(ref sortCategory_idJensen, value, "SortCategory_idJensen"); }
        }

        public int ProcessCodeID
        {
            get { return processCodeID; }
            set { processCodeID = AssignNotify(ref processCodeID, value, "ProcessCodeID"); }
        }

        public string ProcessName
        {
            get { return processName; }
            set { processName = AssignNotify(ref processName, value, "ProcessName"); }
        }

        public int Production_Norm
        {
            get { return production_Norm; }
            set { production_Norm = AssignNotify(ref production_Norm, value, "Production_Norm"); }
        }

        public int Production_NoFlow_Time
        {
            get { return production_NoFlow_Time; }
            set { production_NoFlow_Time = AssignNotify(ref production_NoFlow_Time, value, "Production_NoFlow_Time"); }
        }

        public bool Count_Exit_Point
        {
            get { return count_Exit_Point; }
            set { count_Exit_Point = AssignNotify(ref count_Exit_Point, value, "Count_Exit_Point"); }
        }

        #endregion
    }


    #endregion


    #region ProcessNames

    partial class SqlDataAccess
    {


        public ProcessNames GetProcessNames(ProcessNames recs, bool noCacheRead)
        {
            if (recs == null)
                recs = new ProcessNames();
            recs = (ProcessNames)DBDataListSelect(recs, noCacheRead, !noCacheRead);
            return recs;
        }

    }

    public class ProcessNames : DataList
    {
        public ProcessNames()
        {
            Lifespan = 1.0;
            ListType = typeof(ProcessName);
            IsReadOnly = true;
            TblName = "tblProcessNames";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("Machine_IDJensen", "MachineID");
            dBFieldMappings.AddMapping("ProcessCode", "ProcessCode");
            dBFieldMappings.AddMapping("ProcessName", "ProcName");
            ProcessName dataItem = (ProcessName)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }

    }

    public class ProcessName : DataItem
    {
        #region private fields

        private int machineID;
        private int processCode;
        private string processName;

        #endregion

        #region properties

        public int MachineID
        {
            get { return machineID; }
            set
            {
                machineID = AssignNotify(ref machineID, value, "MachineID");
                ID = machineID;
            }
        }

        public int ProcessCode
        {
            get { return processCode; }
            set
            {
                processCode = AssignNotify(ref processCode, value, "ProcessCode");
                ID2 = processCode;
            }
        }

        public string ProcName
        {
            get { return processName; }
            set
            {
                processName = AssignNotify(ref processName, value, "ProcName");
                ItemName = processName;
            }
        }

        #endregion
    }

    #endregion


    #region SortCategories

    partial class SqlDataAccess
    {
        public SortCategories GetSortCategories(SortCategories recs, bool noCache)
        {
            if (recs==null)
                recs = new SortCategories();
            recs = (SortCategories)DBDataListSelect(recs, noCache, !noCache);
            return recs;
        }

    }

    public class SortCategories : DataList
    {
        public SortCategories()
        {
            Lifespan = 1.0;
            ListType = typeof(SortCategory);
            TblName = "tblSortCategory";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecNum", "RecNum", false, true);
            dBFieldMappings.AddMapping("idJensen", "IdJensen", true, false);
            dBFieldMappings.AddMapping("QuickRef", "QuickRef");
            dBFieldMappings.AddMapping("ExtRef", "ExtRef");
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("LongDescription", "LongDescription");
            dBFieldMappings.AddMapping("BackColour", "BackColour");
            dBFieldMappings.AddMapping("ForeColour", "ForeColour");
            SortCategory dataItem = (SortCategory)NewItem();
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class SortCategory : DataItem, INotifyPropertyChanged
    {
        #region private fields

        private int recNum = -1;
        private int idJensen = -1;
        private int quickRef = -1;
        private string extRef = string.Empty;
        private string shortDescription = string.Empty;
        private string longDescription = string.Empty;
        private int backColour = 0;
        private int foreColour = 0;

        #endregion

        #region Data Column Properties

        public int RecNum
        {
            get { return recNum; }
            set
            {
                recNum = AssignNotify(ref recNum, value, "RecNum");
                PrimaryKey = recNum;
            }
        }

        public int IdJensen
        {
            get { return idJensen; }
            set
            {
                idJensen = AssignNotify(ref idJensen, value, "IdJensen");
                ID = idJensen;
            }
        }

        public int QuickRef
        {
            get { return quickRef; }
            set { quickRef = AssignNotify(ref quickRef, value, "QuickRef"); }
        }

        public string ExtRef
        {
            get
            {
                if (extRef != null)
                    return extRef;
                else
                    return string.Empty;
            }
            set { extRef = AssignNotify(ref extRef, value, "ExtRef"); }
        }

        public string ShortDescription
        {
            get { return shortDescription; }
            set
            {
                shortDescription = AssignNotify(ref shortDescription, value, "ShortDescription");
                ItemName = shortDescription;
            }
        }

        public string SortCategoryName
        {
            get { return ShortDescription; }
            set { ShortDescription = AssignNotify(ref shortDescription, value, "SortCategoryName"); }
        }

        public string SortCategoryNameID
        {
            get { return NameAndID; }
        }

        public string ShortDescAndID
        {
            get { return NameAndID; }
        }

        public string LongDescription
        {
            get { return longDescription; }
            set { longDescription = AssignNotify(ref longDescription, value, "LongDescription"); }
        }

        public int BackColour
        {
            get { return backColour; }
            set { backColour = AssignNotify(ref backColour, value, "BackColour"); }
        }

        public int ForeColour
        {
            get { return foreColour; }
            set { foreColour = AssignNotify(ref foreColour, value, "ForeColour"); }
        }

        #endregion
    }

    #endregion

}
