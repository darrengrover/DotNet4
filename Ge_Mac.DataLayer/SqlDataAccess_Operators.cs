using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private Operators operatorsCache = null;

        public void InvalidateOperators()
        {
            if (operatorsCache != null)
                operatorsCache.IsValid = false;
        }

        private bool OperatorsAreCached()
        {
            bool test = (operatorsCache != null);
            if (test)
            {
                test = operatorsCache.IsValid;
            }
            return test;
        }

        const string allOperatorsCommand =
            @"SELECT [RecNum]
                    ,[idJensen]
                    ,[QuickRef]
                    ,[ExtRef]
                    ,[ShortDescription]
                    ,[LongDescription]
                    ,[BackColour]
                    ,[ForeColour]
                    ,[Retired]
              FROM [dbo].[tblOperators]";

        public Operators GetAllOperators()
        {
            try
            {
                const string commandString = allOperatorsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Operators operators = new Operators();
                    command.DataFill(operators, SqlDataConnection.DBConnection.JensenGroup);
                    return operators;
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

        public Operators GetAllActiveOperators()
        {
            return GetAllActiveOperators(false);
        }

        public Operators GetAllActiveOperators(bool noCache)
        {
            if (!noCache && OperatorsAreCached())
            {
                return operatorsCache;
            }
            try
            {
                const string commandString =
                    allOperatorsCommand +
                    " WHERE Retired = 0";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (operatorsCache == null)
                        operatorsCache = new Operators();
                    command.DataFill(operatorsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return operatorsCache;
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

        public Operator GetOperator(int idjensen)
        {
            Operators operators = GetAllActiveOperators();
            Operator op = operators.GetById(idjensen);

            return op;
        }

        #endregion

        #region Next Record

        public int NextOperatorRecord()
        {
            //const string commandString = @"select max(idJensen) from dbo.tblOperators";
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblOperators',
		                                            @idName = N'idJensen'
                                         SELECT @return_value";


            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
                    if (spResult != null)
                    {
                        if (spResult.ToString() != string.Empty)
                        {
                            nextID = (int)spResult;
                            nextID++;
                        }
                    }
                }
            }

            catch (SqlException)
            {
                throw;
            }
            return nextID;
        }

        #endregion

        #region Insert Data
        public void InsertNewOperator(Operator oper)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblOperators]
                  ( idJensen
                  , QuickRef
                  , ExtRef
                  , ShortDescription
                  , LongDescription
                  , BackColour
                  , ForeColour
                  , Retired
                  )
                  VALUES
                  ( @idJensen
                  , @QuickRef
                  , @ExtRef
                  , @ShortDescription
                  , @LongDescription
                  , @BackColour
                  , @ForeColour
                  , @Retired
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", oper.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", oper.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = oper.ExtRef == null ? DBNull.Value : (object)oper.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", oper.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", oper.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", oper.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", oper.ForeColour);
                    command.Parameters.AddWithValue("@Retired", oper.Retired);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            oper.RecNum = (int)RecNum;
                            oper.HasChanged = false;
                        }
                        InvalidateOperators();
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;

                        if (ex.Number != insertError)
                        {
                            throw;
                        }
                    }
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

        #region Update Data
        public void UpdateOperatorDetails(Operator oper)
        {
            const string commandString =
                @"UPDATE [dbo].[tblOperators]
                  SET idJensen = @idJensen
                    , QuickRef = @QuickRef
                    , ExtRef = @ExtRef
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , BackColour = @BackColour
                    , ForeColour = @ForeColour
                    , Retired = @Retired
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", oper.RecNum);
                    command.Parameters.AddWithValue("@idJensen", oper.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", oper.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = oper.ExtRef == null ? DBNull.Value : (object)oper.ExtRef;

                    command.Parameters.AddWithValue("@ShortDescription", oper.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", oper.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", oper.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", oper.ForeColour);
                    command.Parameters.AddWithValue("@Retired", oper.Retired);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    oper.HasChanged = false;
                }
                InvalidateOperators();
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

        #region Delete Data
        /// <summary>
        /// Delete a operator permanently from the database.
        /// This should not normally be used as the operators are normally retired.
        /// </summary>
        /// <param name="oper"></param>
        public void DeleteOperatorDetails(Operator oper)
        {
            const string commandString =
                /*@"DELETE FROM [dbo].[tblOperators]
                  WHERE RecNum = @RecNum";*/
                @"UPDATE [dbo].[tblOperators]
                  SET Retired = 1
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", oper.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateOperators();
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
    public class Operators : List<Operator>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Operator op)
        {
            base.Add(op);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, op));
            }
        }

        new public void Remove(Operator op)
        {
            base.RemoveAt(this.IndexOf(op));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, op));
            }
        }

        #endregion

        private double lifespan = 1.0;
        private string tblName = "tblOperators";
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
            int RecNumPos = dr.GetOrdinal("RecNum");
            int idJensenPos = dr.GetOrdinal("idJensen");
            int QuickRefPos = dr.GetOrdinal("QuickRef");
            int ExtRefPos = dr.GetOrdinal("ExtRef");
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int BackColourPos = dr.GetOrdinal("BackColour");
            int ForeColourPos = dr.GetOrdinal("ForeColour");
            int RetiredPos = dr.GetOrdinal("Retired");

            this.Clear();
            while (dr.Read())
            {
                Operator oper = new Operator()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    idJensen = dr.GetInt16(idJensenPos),
                    QuickRef = dr.IsDBNull(QuickRefPos) ? (short)0 : dr.GetInt16(QuickRefPos),
                    ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                    ShortDescription = dr.GetString(ShortDescriptionPos),
                    LongDescription = dr.GetString(LongDescriptionPos),
                    BackColour = dr.GetInt32(BackColourPos),
                    ForeColour = dr.GetInt32(ForeColourPos),
                    Retired = dr.GetBoolean(RetiredPos),
                    HasChanged = false
                };

                this.Add(oper);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public Operator GetById(int id)
        {
            return this.Find(delegate(Operator op)
            {
                return op.idJensen == id;
            });
        }


        public Operator GetByNameID(string aNameID)
        {
            return this.Find(delegate(Operator op)
            {
                return op.ShortDescAndID == aNameID;
            });
        }
    }
    #endregion

    #region Item Class
    public class Operator : DataItem, IComparable<Operator>, INotifyPropertyChanged
    {
        #region Operator Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal short QuickRef;
            internal string ExtRef;
            internal string ShortDescription;
            internal string LongDescription;
            internal int BackColour;
            internal int ForeColour;
            internal bool Retired;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Operator()
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

        #region Color Properties
        public Color BackgroundColour
        {
            get
            {
                return ColorFromInt(BackColour);
            }
            set
            {
                BackColour = (int)Convert.ToInt32(value.ToArgb().ToString());
            }
        }

        public Color ForegroundColour
        {
            get
            {
                return ColorFromInt(ForeColour);
            }
            set
            {
                ForeColour = (int)Convert.ToInt32(value.ToArgb().ToString());
            }
        }

        private Color ColorFromInt(int color)
        {
            string str = "#" + color.ToString("X8");
            ColorConverter conv = new ColorConverter();
            return (Color)conv.ConvertFromString(str);
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
                if (this.activeData.RecNum != value)
                {
                    this.activeData.RecNum = value;
                    NotifyPropertyChanged("RecNum");
                }
            }
        }

        public short idJensen
        {
            get
            {
                return this.activeData.idJensen;
            }
            set
            {
                if (this.activeData.idJensen != value)
                {
                    this.activeData.idJensen = value;
                    NotifyPropertyChanged("idJensen");
                    getTag();
                }
            }
        }

        public short QuickRef
        {
            get
            {
                return this.activeData.QuickRef;
            }
            set
            {
                if (this.activeData.QuickRef != value)
                {
                    this.activeData.QuickRef = value;
                    NotifyPropertyChanged("QuickRef");
                } 
            }
        }

        public string ExtRef
        {
            get
            {
                return this.activeData.ExtRef;
            }
            set
            {
                if (this.activeData.ExtRef != value)
                {
                    this.activeData.ExtRef = value;
                    NotifyPropertyChanged("ExtRef");
                }
            }
        }

        private Tag tag = null;
        private Tags tags = null;

        private void getTag()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (tags == null)
                tags = da.GetAllTags(false);
            if (tags != null)
                tag = tags.GetByRefs("tblOperators", idJensen);
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
        private string cardIDerror = "";
        public string CardIDError
        {
            get { return cardIDerror; }
            set { cardIDerror = value; }
        }
        private string cardInUsePrefix = "Card in use by";
        public string CardInUsePrefix
        {
            get { return cardInUsePrefix; }
            set { cardInUsePrefix = value; }
        }
        private string cardInUseByMsg = "";
        public string CardInUseByMsg
        {
            get { return cardInUseByMsg; }
            set { cardInUseByMsg = value; }
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
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (tags == null)
                tags = da.GetAllTags(false);
            if (tags != null)
            {
                Tag t = tags.GetByTag(hexCardID);
                test = ((t != null) && (t.ReferenceID != idJensen));
            }
            cardAlreadyInUse = test;
            return test;
        }
        private string GetCardUserMsg()
        {
            string msg = "";
            SqlDataAccess da = SqlDataAccess.Singleton;
            if (tags == null)
                tags = da.GetAllTags(false);
            if (tags != null)
            {
                Tag t = tags.GetByTag(hexCardID);
                if (t != null)
                {
                    msg = CardInUsePrefix + " " + t.ShortDescAndID;
                }
            }

            return msg;
        }
        private void setCardID(string decimalID)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            CardIDError = "";
            CardInUseByMsg = "";
            decimalCardID = decimalID;
            hexCardID = DecimalToHexCardID(decimalCardID);
            Debug.WriteLine("SetCardID() Decimal = " + decimalID + ". Hex = " + hexCardID);
            if ((hexCardID != "00000000") && (!GetCardIDAlreadyInUse()))
            {
                if (tag != null)
                {
                    if (CardIDValid)
                    {
                        if (!GetCardIDAlreadyInUse() && (hexCardID != "00000000"))
                        {
                            tag.TagData = hexCardID;
                            tag.ForceNew = false;
                            Debug.WriteLine(hexCardID);
                            if (tags != null)
                                tags.UpdateToDB();
                        }
                        else
                        {
                            Debug.WriteLine(decimalID + " already in use");
                            CardIDError = "Card already in use";
                            CardInUseByMsg = GetCardUserMsg();
                        }
                    }
                    else
                    {
                        int i;
                        Debug.WriteLine(decimalID + " invalid");
                        if (decimalCardID.Length < 6)
                        {
                            CardIDError = "Card ID too short";
                            Debug.WriteLine("Card ID too short");
                        }
                        if (GetCardIDAlreadyInUse())
                        {
                            CardIDError = "Card already in use";
                            CardInUseByMsg = GetCardUserMsg();
                            Debug.WriteLine("Card already in use");
                        }
                        if (hexCardID == "00000000")
                        {
                            CardIDError = "hexCardID = 00000000";
                            Debug.WriteLine("hexCardID = 00000000");
                        }
                        if (!int.TryParse(decimalCardID, out i))
                        {
                            CardIDError = "Bad int parse";
                            Debug.WriteLine("Bad int parse");
                        }
                    }
                }
                else
                {
                    if ((decimalCardID != string.Empty) && (decimalCardID != "000000"))
                    {
                        if (tags == null)
                            tags = da.GetAllTags(false);
                        if (tags != null)
                        {
                            tag = new Tag();
                            tag.TagID = tags.GetNextID();
                            tag.ForceNew = true;
                            tag.TagData = hexCardID;
                            tag.ReferenceID = idJensen;
                            tag.ReferenceTable = "tblOperators";
                            if (tag.TagData != "00000000")
                            {
                                tags.Add(tag);
                                tags.UpdateToDB();
                            }
                        }
                    }
                }
            }
            else
            {
                int i;
                Debug.WriteLine(decimalID + " invalid");
                if (decimalCardID.Length < 6)
                {
                    CardIDError = "Card ID too short";
                    Debug.WriteLine("Card ID too short");
                }
                if (GetCardIDAlreadyInUse())
                {
                    CardIDError = "Card already in use";
                    CardInUseByMsg = GetCardUserMsg();
                    Debug.WriteLine("Card already in use");
                }
                if (hexCardID == "00000000")
                {
                    CardIDError = "hexCardID = 00000000";
                    Debug.WriteLine("hexCardID = 00000000");
                }
                if (!int.TryParse(decimalCardID, out i))
                {
                    CardIDError = "Bad int parse";
                    Debug.WriteLine("Bad int parse");
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
                int i = 0;
                return (decimalCardID.Length > 5)
                    && (!CardAlreadyInUse)
                    && (hexCardID != "00000000")
                    && (int.TryParse(decimalCardID, out i));
            }
        }

        public void RemoveTag()
        {
            if (tag != null)
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Tags tags = da.GetAllTags(false);
                if (tags != null)
                {
                    tag.TagData = "DELETED";
                    tag.DeleteRecord = true;
                    tags.UpdateToDB();
                }
                tag = null;
            }
            decimalCardID = string.Empty;
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
                    NotifyPropertyChanged("CardID");
                }
            }
        }

        public Color CardIDColor
        {
            get
            {
                Color aColor = Color.Red;
                if (CardIDValid)
                {
                    aColor = Color.Black;
                }
                return aColor;
            }
        }

        public string ShortDescription
        {
            get
            {
                return this.activeData.ShortDescription;
            }
            set
            {
                if (this.activeData.ShortDescription != value)
                {
                    this.activeData.ShortDescription = value;
                    NotifyPropertyChanged("ShortDescription");
                }
            }
        }

        public string OperatorName
        {
            get
            {
                return ShortDescription;
            }
            set
            {
                if (this.activeData.ShortDescription != value)
                {
                    this.activeData.ShortDescription = value;
                    NotifyPropertyChanged("OperatorName");
                }
            }
        }

        public string OperatorNameID
        {
            get
            {
                return ShortDescAndID;
            }
        }

        public string ShortDescAndID
        {
            get
            {
                return ShortDescription + " (" + idJensen.ToString() + ")";
            }
        }

        public string LongDescription
        {
            get
            {
                if (activeData.LongDescription != string.Empty)
                    return this.activeData.LongDescription;
                else
                    return activeData.ShortDescription;
            }
            set
            {
                if (this.activeData.LongDescription != value)
                {
                    this.activeData.LongDescription = value;
                    NotifyPropertyChanged("LongDescription");
                }
            }
        }

        public int BackColour
        {
            get
            {
                return this.activeData.BackColour;
            }
            set
            {
                if (this.activeData.BackColour != value)
                {
                    this.activeData.BackColour = value;
                    NotifyPropertyChanged("BackColour");
                }
            }
        }

        public int ForeColour
        {
            get
            {
                return this.activeData.ForeColour;
            }
            set
            {
                if (this.activeData.ForeColour != value)
                {
                    this.activeData.ForeColour = value;
                    NotifyPropertyChanged("ForeColour");
                }
            }
        }

        public bool Retired
        {
            get
            {
                return this.activeData.Retired;
            }
            set
            {
                if (this.activeData.Retired != value)
                {
                    this.activeData.Retired = value;
                    NotifyPropertyChanged("Retired");
                }
            }
        }

        #endregion


        public int CompareTo(Operator other) { return ShortDescription.CompareTo(other.ShortDescription); }
     }
    #endregion
}
