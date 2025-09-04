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

        private Customers customersCache = null;

        public void InvalidateCustomers()
        {
            if (customersCache != null)
                customersCache.IsValid = false;
        }

        private bool CustomersAreCached()
        {
            bool test = (customersCache != null);
            if (test)
            {
                test = customersCache.IsValid;
            }
            return test;
        }

        const string allCustomersCommand =
            @"SELECT RecNum
                   , idJensen
                   , QuickRef
                   , ExtRef
                   , CustomerType
                   , ShortDescription
                   , LongDescription
                   , BackColour
                   , ForeColour
                   , Retired
              FROM dbo.[tblCustomers]";

        public Customers GetAllCustomers() // not cached only used in editing.
        {
            try
            {
                const string commandString = allCustomersCommand;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Customers allCustomers = new Customers();
                    command.DataFill(allCustomers, SqlDataConnection.DBConnection.JensenGroup);
                    return allCustomers;
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

        public Customers GetAllActiveCustomers()
        {
            if (CustomersAreCached())
            {
                return customersCache;
            }
            try
            {
                const string commandString =
                    allCustomersCommand +
                    " WHERE Retired = 0";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (customersCache == null) customersCache = new Customers();
                    command.DataFill(customersCache, SqlDataConnection.DBConnection.JensenGroup);
                    return customersCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public Customer GetCustomer(int idjensen)
        {
            Customers customers = GetAllActiveCustomers();
            Customer customer = customers.GetById(idjensen);

            return customer;
        }

        #endregion

        #region Next Record

        public int NextCustomerRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblCustomers',
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
        public void InsertNewCustomer(Customer customer)
        {
            const string commandString =
                @"INSERT INTO [dbo].tblCustomers
                  ( idJensen
                  , QuickRef
                  , ExtRef
                  , CustomerType
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
                  , @CustomerType
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
                    command.Parameters.AddWithValue("@idJensen", customer.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", customer.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    if (DatabaseVersion < 2.0)
                    {
                        p.IsNullable = true;
                        p.Value = customer.ExtRef == null ? DBNull.Value : (object)customer.ExtRef;
                    }
                    else
                    {
                        p.IsNullable = false;
                        p.Value = customer.ExtRef == null ? "" : (object)customer.ExtRef;
                    }

                    command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
                    command.Parameters.AddWithValue("@ShortDescription", customer.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", customer.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", customer.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", customer.ForeColour);
                    command.Parameters.AddWithValue("@Retired", customer.Retired);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            customer.RecNum = (int)RecNum;
                            customer.HasChanged = false;
                        }
                        InvalidateCustomers();
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
        public void UpdateCustomerDetails(Customer customer)
        {
            const string commandString =
                @"UPDATE [dbo].tblCustomers
                  SET idJensen = @idJensen
                    , QuickRef = @QuickRef
                    , ExtRef = @ExtRef
                    , CustomerType = @CustomerType
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
                    command.Parameters.AddWithValue("@RecNum", customer.RecNum);
                    command.Parameters.AddWithValue("@idJensen", customer.idJensen);
                    command.Parameters.AddWithValue("@QuickRef", customer.QuickRef);

                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
                    p.IsNullable = true;
                    p.Value = customer.ExtRef == null ? DBNull.Value : (object)customer.ExtRef;

                    command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
                    command.Parameters.AddWithValue("@ShortDescription", customer.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", customer.LongDescription);
                    command.Parameters.AddWithValue("@BackColour", customer.BackColour);
                    command.Parameters.AddWithValue("@ForeColour", customer.ForeColour);
                    command.Parameters.AddWithValue("@Retired", customer.Retired);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    customer.HasChanged = false;
                }
                InvalidateCustomers();
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
        /// Delete a customer permanently from the database.
        /// This should not normally be used as the customers are normally retired.
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteCustomerDetails(Customer customer)
        {
            const string commandString =
                /*@"DELETE FROM [dbo].tblCustomers
                  WHERE RecNum = @RecNum";*/
                @"UPDATE [dbo].tblCustomers
                  SET Retired = 1
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", customer.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateCustomers();
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

    public class Customers : List<Customer>, IDataFiller, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Reset()
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        new public void Add(Customer customer)
        {
            base.Add(customer);
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, customer));
            }
        }

        new public void Remove(Customer customer)
        {
            base.RemoveAt(this.IndexOf(customer));
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, customer));
            }
        }

        #endregion
        
        private double lifespan = 1.0;
        private string tblName = "tblCustomers";
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
            int CustomerTypePos = dr.GetOrdinal("CustomerType");
            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
            int BackColourPos = dr.GetOrdinal("BackColour");
            int ForeColourPos = dr.GetOrdinal("ForeColour");
            int RetiredPos = dr.GetOrdinal("Retired");

            this.Clear();

            while (dr.Read())
            {
                Customer customer = new Customer()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    idJensen = dr.GetInt32(idJensenPos),
                    QuickRef = dr.IsDBNull(QuickRefPos) ? (short)0 : dr.GetInt16(QuickRefPos),
                    ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos),
                    CustomerType = dr.GetInt32(CustomerTypePos),
                    ShortDescription = dr.GetString(ShortDescriptionPos),
                    LongDescription = dr.GetString(LongDescriptionPos),
                    BackColour = dr.GetInt32(BackColourPos),
                    ForeColour = dr.GetInt32(ForeColourPos),
                    Retired = dr.GetBoolean(RetiredPos),
                    HasChanged = false
                };

                this.Add(customer);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public Customer GetById(int id)
        {
            return this.Find(delegate(Customer cust)
            {
                return cust.idJensen == id;
            });
        }

        public Customer GetByName(string aName)
        {
            return this.Find(delegate(Customer cust)
            {
                return cust.ShortDescription == aName;
            });
        }

        public Customer GetByNameID(string aName)
        {
            return this.Find(delegate(Customer cust)
            {
                return cust.ShortDescAndID == aName;
            });
        }

        public List<Customer> FindAllByName(string name)
        {
            FindCustomers predicate = new FindCustomers(new Customer() { ShortDescription = name });
            return FindAll(predicate.ByName);
        }

        public class FindCustomers
        {
            public Customer customer { get; set; }

            // Initializes with suffix we want to match.
            public FindCustomers(Customer c)
            {
                customer = c;
            }

            // Gets the predicate.  Now it's possible to re-use this predicate with various suffixes.
            public Predicate<Customer> ByName
            {
                get
                {
                    return IsMatchingName;
                }
            }

            private bool IsMatchingName(Customer c)
            {
                return (c.ShortDescription == customer.ShortDescription);
            }
        }
    }

    #endregion

    #region Customer Class
    public class Customer : DataItem, IComparable<Customer>, INotifyPropertyChanged
    {
        #region Customer Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int idJensen;
            internal short QuickRef;
            internal string ExtRef;
            internal int CustomerType;
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
        public Customer()
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

        public int idJensen
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

        public int CustomerType
        {
            get
            {
                return this.activeData.CustomerType;
            }
            set
            {
                if (this.activeData.CustomerType != value)
                {
                    this.activeData.CustomerType = value;
                    NotifyPropertyChanged("CustomerType");
                }
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

        public string ShortDescAndID
        {
            get
            {
                return ShortDescription + " (" + idJensen.ToString() + ")";
            }
        }

        public string CustomerNameID
        {
            get
            {
                return ShortDescAndID;
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

        #region Sorting
        public static Comparison<Customer> NameComparison = 
            delegate(Customer c1, Customer c2)
            {
                return c1.ShortDescription.CompareTo(c2.ShortDescription);
            };

        public static Comparison<Customer> IDComparison = 
            delegate(Customer c1, Customer c2)
        {
            return c1.idJensen.CompareTo(c2.idJensen);
        };

        public int CompareTo(Customer other) { return ShortDescription.CompareTo(other.ShortDescription); }
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
