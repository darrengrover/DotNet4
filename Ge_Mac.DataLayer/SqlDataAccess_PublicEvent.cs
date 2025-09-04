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
 
 
        #endregion

        #region Insert Data
        public void InsertNewDataRec(PublicMachineEvent pMEv)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblMachineEventDetails]
                       ([Start_RemoteID]
                       ,[MachineID]
                       ,[SubID]
                       ,[EventID]
                       ,[JGLogData_MessageA]
                       ,[Start_TimeStamp]
                       ,[End_TimeStamp]
                       ,[Severity])
                 VALUES
                       (@Start_RemoteID
                       ,@MachineID
                       ,@SubID
                       ,@EventID
                       ,@JGLogData_MessageA
                       ,@Start_TimeStamp
                       ,@End_TimeStamp
                       ,@Severity)";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@Start_RemoteID", pMEv.Remote_RecNum);
                    command.Parameters.AddWithValue("@MachineID", pMEv.MachineID);
                    command.Parameters.AddWithValue("@SubID", pMEv.SubID);
                    command.Parameters.AddWithValue("@EventID", pMEv.EventID);
                    command.Parameters.AddWithValue("@JGLogData_MessageA", pMEv.MessageA);
                    command.Parameters.AddWithValue("@Start_TimeStamp", pMEv.Start_TimeStamp);
                    command.Parameters.AddWithValue("@End_TimeStamp", pMEv.End_TimeStamp);
                    command.Parameters.AddWithValue("@Severity", pMEv.Severity);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
                        pMEv.HasChanged = false;
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
        #endregion

        #region Delete Data
  
        #endregion
    }

    #region Data Collections

    public class PublicMachineEvents : List<PublicMachineEvent>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int Remote_RecNumPos = dr.GetOrdinal("Remote_RecNum");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int EventIDPos = dr.GetOrdinal("EventID");
            int MessageAPos = dr.GetOrdinal("MessageA");
            //int MessageBPos = dr.GetOrdinal("MessageB");
            int StartTimeStampPos = dr.GetOrdinal("Start_TimeStamp");
            int EndTimeStampPos = dr.GetOrdinal("End_TimeStamp");
            int SeverityPos = dr.GetOrdinal("Severity");

            while (dr.Read())
            {
                PublicMachineEvent publicMachineEvent = new PublicMachineEvent()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    Remote_RecNum = dr.GetInt32(Remote_RecNumPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    SubID = dr.GetInt32(SubIDPos),
                    EventID = dr.GetInt32(EventIDPos),
                    MessageA = dr.GetString(MessageAPos),
                    // MessageB=dr.GetString(MessageBPos),
                    Start_TimeStamp = dr.GetDateTime(StartTimeStampPos),
                    End_TimeStamp = dr.GetDateTime(EndTimeStampPos),
                    Severity = dr.GetInt32(SeverityPos),
                    HasChanged = false
                };
                this.Add(publicMachineEvent);
            }

            return this.Count;
        }

        public PublicMachineEvent GetByIds(int MachineID, int Subid, int EvID)
        {
            return this.Find(delegate(PublicMachineEvent opRec)
            {
                return ((opRec.MachineID == MachineID)
                    && (opRec.SubID == Subid)
                    && (opRec.EventID == EvID));
            });
        }


    }
    
    #endregion

    #region Item Classes

    public class PublicMachineEvent : DataItem
    {
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int Remote_RecNum;
            internal int MachineID;
            internal int SubID;
            internal int EventID;
            internal string MessageA;
            internal string MessageB = string.Empty;
            internal DateTime Start_TimeStamp;
            internal DateTime End_TimeStamp;
            internal int Severity;
            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }

        #region Constructor
        public PublicMachineEvent()
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

        public int RecNum
        {
            get
            {
                return this.activeData.RecNum;
            }
            set
            {
                HasChanged |= this.activeData.RecNum != value;
                this.activeData.RecNum = value;
            }
        }

        public int Remote_RecNum
        {
            get
            {
                return this.activeData.Remote_RecNum;
            }
            set
            {
                HasChanged |= this.activeData.Remote_RecNum != value;
                this.activeData.Remote_RecNum = value;
            }
        }

        public override int ID
        {
            get
            {
                return RecNum;
            }
            set
            {
                if (RecNum != value)
                {
                    RecNum = value;
                    NotifyPropertyChanged("ID");
                }
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
                HasChanged |= this.activeData.MachineID != value;
                this.activeData.MachineID = value;
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
                HasChanged |= this.activeData.SubID != value;
                this.activeData.SubID = value;
            }
        }

        public int EventID
        {
            get
            {
                return this.activeData.EventID;
            }
            set
            {
                HasChanged |= this.activeData.EventID != value;
                this.activeData.EventID = value;
            }
        }

        public string MessageA
        {
            get
            {
                return this.activeData.MessageA;
            }
            set
            {
                HasChanged |= this.activeData.MessageA != value;
                this.activeData.MessageA = value;
            }
        }
        public string MessageB
        {
            get
            {
                return this.activeData.MessageB;
            }
            set
            {
                HasChanged |= this.activeData.MessageB != value;
                this.activeData.MessageB = value;
            }
        }

        public DateTime Start_TimeStamp
        {
            get
            {
                return this.activeData.Start_TimeStamp;
            }
            set
            {
                HasChanged |= this.activeData.Start_TimeStamp != value;
                this.activeData.Start_TimeStamp = value;
            }
        }

        public DateTime End_TimeStamp
        {
            get
            {
                return this.activeData.End_TimeStamp;
            }
            set
            {
                HasChanged |= this.activeData.End_TimeStamp != value;
                this.activeData.End_TimeStamp = value;
            }
        }

        public int Severity
        {
            get
            {
                return this.activeData.Severity;
            }
            set
            {
                HasChanged |= this.activeData.Severity != value;
                this.activeData.Severity = value;
            }
        }

    }
    #endregion
}
