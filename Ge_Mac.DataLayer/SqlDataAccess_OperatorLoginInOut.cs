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
        #region Select Data
        const string allOperatorLoginInOutCommand =
            @"SELECT [RecNum]
                   , [OperatorID]
                   , [MachineID]
                   , [SubID]
                   , [TimeStamp_Login]
                   , [TimeStamp_Logout]
              FROM [dbo].[tblOperatorLoginInOut]";

        public OperatorLoginInOuts GetAllOperatorLoginInOuts()
        {
            try
            {
                const string commandString = allOperatorLoginInOutCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    OperatorLoginInOuts operators = new OperatorLoginInOuts();
                    command.DataFill(operators, SqlDataConnection.DBConnection.JensenPublic);
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

        public OperatorLoginInOuts GetAllActiveOperatorLoginInOuts()
        {
            try
            {
                const string commandString =
                    allOperatorLoginInOutCommand +
                    " WHERE [TimeStamp_Logout] IS NULL";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    OperatorLoginInOuts operators = new OperatorLoginInOuts();
                    command.DataFill(operators, SqlDataConnection.DBConnection.JensenPublic);
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

        public OperatorLoginInOut GetOperatorLoginInOut(int MachineID, int StationID)
        {
            try
            {
                const string commandString =
                    allOperatorLoginInOutCommand +
                    @" WHERE [MachineID] = @MachineID
                       AND   [SubID] = @StationID";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@MachineID", MachineID);
                    command.Parameters.AddWithValue("@StationID", StationID);
                    OperatorLoginInOuts operators = new OperatorLoginInOuts();

                    command.DataFill(operators, SqlDataConnection.DBConnection.JensenPublic);
                    if (operators.Count > 0)
                    {
                        return operators[0];
                    }
                    else
                    {
                        Debug.Write("Nothing");
                        return null;
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

        public OperatorLoginInOuts GetUpdatedOperatorLoginInOuts(DateTime checkDate)
        {
            try
            {
                const string commandString =
                    allOperatorLoginInOutCommand +
                    @" WHERE [TimeStamp_Login] > @CheckDate
                       OR    [TimeStamp_Logout] > @CheckDate
                       ORDER BY RecNum";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@CheckDate", checkDate);
                    OperatorLoginInOuts stations = new OperatorLoginInOuts();
                    command.DataFill(stations, SqlDataConnection.DBConnection.JensenPublic);
                    return stations;
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

    #region Data Collection Class
    public class OperatorLoginInOuts : List<OperatorLoginInOut>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int OperatorIDPos = dr.GetOrdinal("OperatorID");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int TimeStamp_LoginPos = dr.GetOrdinal("TimeStamp_Login");
            int TimeStamp_LogoutPos = dr.GetOrdinal("TimeStamp_Logout");

            while (dr.Read())
            {
                OperatorLoginInOut oper = new OperatorLoginInOut()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    OperatorID = dr.GetInt16(OperatorIDPos),
                    MachineID = dr.GetInt16(MachineIDPos),
                    SubID = dr.GetInt32(SubIDPos),
                    TimeStamp_Login = dr.GetDateTime(TimeStamp_LoginPos),
                    TimeStamp_Logout = dr.IsDBNull(TimeStamp_LogoutPos) ? null : (DateTime?)dr.GetDateTime(TimeStamp_LogoutPos),
                    HasChanged = false
                };

                this.Add(oper);
            }

            return this.Count;
        }

        public OperatorLoginInOut GetById(int id)
        {
            return this.Find(delegate(OperatorLoginInOut cust)
            {
                return cust.RecNum == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class OperatorLoginInOut : DataItem
    {
        #region OperatorLoginInOut Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short OperatorID;
            internal short MachineID;
            internal int SubID;
            internal DateTime TimeStamp_Login;
            internal DateTime? TimeStamp_Logout;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public OperatorLoginInOut()
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

        #region Data Column Properties
        /*public override int PrimaryKey
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                this.activeData.RecNum = value;
            }
        }*/

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

        public short OperatorID
        {
            get
            {
                return this.activeData.OperatorID;
            }
            set
            {
                this.activeData.OperatorID = value;
                HasChanged = true;
            }
        }

        public short MachineID
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


        public int SubID
        {
            get
            {
                return this.activeData.SubID;
            }
            set
            {
                this.activeData.SubID = value;
                HasChanged = true;
            }
        }

        public DateTime TimeStamp_Login
        {
            get
            {
                return this.activeData.TimeStamp_Login;
            }
            set
            {
                this.activeData.TimeStamp_Login = value;
                HasChanged = true;
            }
        }

        public DateTime? TimeStamp_Logout
        {
            get
            {
                return this.activeData.TimeStamp_Logout;
            }
            set
            {
                this.activeData.TimeStamp_Logout = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
