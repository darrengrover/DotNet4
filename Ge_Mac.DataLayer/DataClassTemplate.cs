using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using System.Data;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data
//        const string allCustomersCommand =
//            @"SELECT RecNum
//                   , idJensen
//                   , QuickRef
//                   , ExtRef
//                   , CustomerType
//                   , ShortDescription
//                   , LongDescription
//                   , BackColour
//                   , ForeColour
//                   , Retired
//              FROM dbo.[tblCustomers]";

//        public Customers GetAllCustomers()
//        {
//            try
//            {
//                const string commandString = allCustomersCommand;

//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    Customers customers = new Customers();
//                    DataFill(customers, command, DBConnection.JensenGroup);
//                    return customers;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }

//        public Customers GetAllActiveCustomers()
//        {
//            try
//            {
//                const string commandString =
//                    allCustomersCommand +
//                    " WHERE Retired = 0";

//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    Customers customers = new Customers();
//                    DataFill(customers, command, DBConnection.JensenGroup);
//                    return customers;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }

//        public Customer GetCustomer(int idjensen)
//        {
//            try
//            {
//                const string commandString =
//                    allCustomersCommand +
//                    " WHERE idjensen = @idjensen";

//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    command.Parameters.AddWithValue("@idjensen", idjensen);
//                    Customers customers = new Customers();

//                    DataFill(customers, command, DBConnection.JensenGroup);
//                    if (customers.Count > 0)
//                    {
//                        return customers[0];
//                    }
//                    else
//                    {
//                        Console.Write("Nothing");
//                        return null;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }
        #endregion

        #region Insert Data
//        public void InsertNewCustomer(Customer customer)
//        {
//            const string commandString =
//                @"INSERT INTO [dbo].tblCustomers
//                  ( idJensen
//                  , QuickRef
//                  , ExtRef
//                  , CustomerType
//                  , ShortDescription
//                  , LongDescription
//                  , BackColour
//                  , ForeColour
//                  , Retired
//                  )
//                  VALUES
//                  ( @idJensen
//                  , @QuickRef
//                  , @ExtRef
//                  , @CustomerType
//                  , @ShortDescription
//                  , @LongDescription
//                  , @BackColour
//                  , @ForeColour
//                  , @Retired
//                  )
//
//                  SELECT CAST(@@IDENTITY AS INT)";

//            try
//            {
//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    command.Parameters.AddWithValue("@idJensen", customer.idJensen);
//                    command.Parameters.AddWithValue("@QuickRef", customer.QuickRef);

//                    SqlParameter p = command.Parameters.Add("@ExtRef", SqlDbType.NVarChar);
//                    p.IsNullable = true;
//                    p.Value = customer.ExtRef == null ? DBNull.Value : (object)customer.ExtRef;

//                    command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
//                    command.Parameters.AddWithValue("@ShortDescription", customer.ShortDescription);
//                    command.Parameters.AddWithValue("@LongDescription", customer.LongDescription);
//                    command.Parameters.AddWithValue("@BackColour", customer.BackColour);
//                    command.Parameters.AddWithValue("@ForeColour", customer.ForeColour);
//                    command.Parameters.AddWithValue("@Retired", customer.Retired);

//                    try
//                    {
//                        object RecNum = ExecuteScalar(command, DBConnection.JensenGroup);

//                        if (RecNum != null)
//                        {
//                            customer.RecNum = (int)RecNum;
//                        }
//                    }
//                    catch (SqlException ex)
//                    {
//                        const int insertError = 2601;

//                        if (ex.Number != insertError)
//                        {
//                            throw;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }
        #endregion

        #region Update Data
//        public void UpdateCustomerDetails(Customer customer)
//        {
//            const string commandString =
//                @"UPDATE [dbo].tblCustomers
//                  SET idJensen = @idJensen
//                    , QuickRef = @QuickRef
//                    , ExtRef = @ExtRef
//                    , CustomerType = @CustomerType
//                    , ShortDescription = @ShortDescription
//                    , LongDescription = @LongDescription
//                    , BackColour = @BackColour
//                    , ForeColour = @ForeColour
//                    , Retired = @Retired
//                  WHERE RecNum = @RecNum";

//            try
//            {
//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    command.Parameters.AddWithValue("@RecNum", customer.RecNum);
//                    command.Parameters.AddWithValue("@idJensen", customer.idJensen);
//                    command.Parameters.AddWithValue("@QuickRef", customer.QuickRef);
//                    command.Parameters.AddWithValue("@ExtRef", customer.ExtRef);
//                    command.Parameters.AddWithValue("@CustomerType", customer.CustomerType);
//                    command.Parameters.AddWithValue("@ShortDescription", customer.ShortDescription);
//                    command.Parameters.AddWithValue("@LongDescription", customer.LongDescription);
//                    command.Parameters.AddWithValue("@BackColour", customer.BackColour);
//                    command.Parameters.AddWithValue("@ForeColour", customer.ForeColour);
//                    command.Parameters.AddWithValue("@Retired", customer.Retired);
//                    ExecuteNonQuery(command, DBConnection.JensenGroup);
//                    customer.HasChanged = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }
        #endregion

        #region Delete Data
//        /// <summary>
//        /// Delete a customer permanently from the database.
//        /// This should not normally be used as the customers are normally retired.
//        /// </summary>
//        /// <param name="customer"></param>
//        public void DeleteCustomerDetails(Customer customer)
//        {
//            const string commandString =
//                @"DELETE FROM [dbo].tblCustomers
//                  WHERE RecNum = @RecNum";

//            try
//            {
//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    command.Parameters.AddWithValue("@RecNum", customer.RecNum);
//                    ExecuteNonQuery(command, DBConnection.JensenGroup);
//                }
//            }
//            catch (Exception ex)
//            {
//                if (Debugger.IsAttached)
//                {
//                    ShowDebugException(ex);
//                    Debugger.Break();
//                }

//                throw;
//            }
//        }
        #endregion
        public int ExtRefMode { get; set; }

        public bool ReadOnlyMachines { get; set; }

        public bool ReadOnlyMachineGroups { get; set; }

        public bool NoDeleteMachines { get; set; }

        public bool NoDeleteMachineGroups { get; set; }

        public bool ReadOnlyOperators { get; set; }
        public bool HideNoArticle { get; set; }
        public bool HideNoCustomer { get; set; }
        public bool HideZeroProduction { get; set; }
    }

    #region Data Collection Class
//    public class Customers : List<Customer>, IDataFiller
//    {
//        public int Fill(SqlDataReader dr)
//        {
//            int RecNumPos = dr.GetOrdinal("RecNum");
//            int idJensenPos = dr.GetOrdinal("idJensen");
//            int QuickRefPos = dr.GetOrdinal("QuickRef");
//            int ExtRefPos = dr.GetOrdinal("ExtRef");
//            int CustomerTypePos = dr.GetOrdinal("CustomerType");
//            int ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
//            int LongDescriptionPos = dr.GetOrdinal("LongDescription");
//            int BackColourPos = dr.GetOrdinal("BackColour");
//            int ForeColourPos = dr.GetOrdinal("ForeColour");
//            int RetiredPos = dr.GetOrdinal("Retired");

//            while (dr.Read())
//            {
//                Customer customer = new Customer();
//                customer.RecNum = dr.GetInt32(RecNumPos);
//                customer.idJensen = dr.GetInt32(idJensenPos);
//                customer.QuickRef = dr.IsDBNull(QuickRefPos) ? 0 : dr.GetInt16(QuickRefPos);
//                customer.ExtRef = dr.IsDBNull(ExtRefPos) ? string.Empty : dr.GetString(ExtRefPos);
//                customer.CustomerType = dr.GetInt32(CustomerTypePos);
//                customer.ShortDescription = dr.GetString(ShortDescriptionPos);
//                customer.LongDescription = dr.GetString(LongDescriptionPos);
//                customer.BackColour = dr.GetInt32(BackColourPos);
//                customer.ForeColour = dr.GetInt32(ForeColourPos);
//                customer.Retired = dr.GetBoolean(RetiredPos);
//                customer.HasChanged = false;

//                // Add to customers collection
//                this.Add(customer);
//            }

//            return this.Count;
//        }

//        public Customer GetById(int id)
//        {
//            return this.Find(delegate(Customer cust)
//            {
//                return cust.idJensen == id;
//            });
//        }
//    }
    #endregion

    #region Item Class
//    public class Customer : DataItem
//    {
        #region Customer Data Record
//        protected class DataRecord : ICopyableObject
//        {
//            internal int RecNum;
//            internal short idJensen;
//            internal int QuickRef;
//            internal string ExtRef;
//            internal int CustomerType;
//            internal string ShortDescription;
//            internal string LongDescription;
//            internal int BackColour;
//            internal int ForeColour;
//            internal bool Retired;

//            public ICopyableObject ShallowCopy()
//            {
//                return (ICopyableObject)MemberwiseClone();
//            }
//        }
        #endregion

        #region Constructor
        //public Customer()
        //{
        //    ActiveData = (ICopyableObject)new DataRecord();
        //}
        #endregion

        #region Color Properties
//        public Color BackgroundColour
//        {
//            get
//            {
//                return ColorFromInt(BackColour);
//            }
//            set
//            {
//                BackColour = (int)Convert.ToInt32(value.ToArgb().ToString());
//            }
//        }

//        public Color ForegroundColour
//        {
//            get
//            {
//                return ColorFromInt(ForeColour);
//            }
//            set
//            {
//                ForeColour = (int)Convert.ToInt32(value.ToArgb().ToString());
//            }
//        }

//        private Color ColorFromInt(int color)
//        {
//            string str = "#" + color.ToString("X8");
//            ColorConverter conv = new ColorConverter();
//            return (Color)conv.ConvertFromString(str);
//        }
        #endregion

        #region Record Status Properties

//        /// <summary>Record Has been Edited</summary>
//        public override bool HasChanged { get; set; }

//        /// <summary>This is a new record, ie Not yet created in the database</summary>
//        public bool IsNew
//        {
//            get
//            {
//                return this.activeData.RecNum == -1;
//            }
//        }

//        /// <summary>The record exists in the database</summary>
//        public override bool IsExisting
//        {
//            get
//            {
//                return this.activeData.RecNum != -1;
//            }
//        }
        #endregion

        #region Data Column Properties
//        public int RecNum
//        {
//            get
//            {
//                return this.activeData.RecNum;
//            }
//            set
//            {
//                this.activeData.RecNum = value;
//                HasChanged = true;
//            }
//        }

//        public int idJensen
//        {
//            get
//            {
//                return this.activeData.idJensen;
//            }
//            set
//            {
//                this.activeData.idJensen = value;
//                HasChanged = true;
//            }
//        }

//        public int QuickRef
//        {
//            get
//            {
//                return this.activeData.QuickRef;
//            }
//            set
//            {
//                this.activeData.QuickRef = value;
//                HasChanged = true;
//            }
//        }

//        public string ExtRef
//        {
//            get
//            {
//                return this.activeData.ExtRef;
//            }
//            set
//            {
//                this.activeData.ExtRef = value;
//                HasChanged = true;
//            }
//        }

//        public int CustomerType
//        {
//            get
//            {
//                return this.activeData.CustomerType;
//            }
//            set
//            {
//                this.activeData.CustomerType = value;
//                HasChanged = true;
//            }
//        }

//        public string ShortDescription
//        {
//            get
//            {
//                return this.activeData.ShortDescription;
//            }
//            set
//            {
//                this.activeData.ShortDescription = value;
//                HasChanged = true;
//            }
//        }

//        public string LongDescription
//        {
//            get
//            {
//                return this.activeData.LongDescription;
//            }
//            set
//            {
//                this.activeData.LongDescription = value;
//                HasChanged = true;
//            }
//        }

//        public int BackColour
//        {
//            get
//            {
//                return this.activeData.BackColour;
//            }
//            set
//            {
//                this.activeData.BackColour = value;
//                HasChanged = true;
//            }
//        }

//        public int ForeColour
//        {
//            get
//            {
//                return this.activeData.ForeColour;
//            }
//            set
//            {
//                this.activeData.ForeColour = value;
//                HasChanged = true;
//            }
//        }

//        public bool Retired
//        {
//            get
//            {
//                return this.activeData.Retired;
//            }
//            set
//            {
//                this.activeData.Retired = value;
//                HasChanged = true;
//            }
//        }
        #endregion

//    }
    #endregion
}
