using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Transactions;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        public delegate void FeedbackDelegate(string feedback);
        public delegate void PercentDelegate(int feedback);
        public delegate void PercentFeedbackDelegate(string aString, int aPercent);

        public PercentFeedbackDelegate TextPercentFeedback = null;
        public FeedbackDelegate TextFeedback = null;
        public PercentDelegate PercentFeedback = null;

        private void Feedback(string aString, int aPercent)
        {
            if (TextPercentFeedback != null)
            {
                TextPercentFeedback(aString, aPercent);
            }
        }

        private void Feedback(int aPercent)
        {
            if (PercentFeedback != null)
            {
                PercentFeedback(aPercent);
            }
        }

        private void Feedback(string aString)
        {
            if (TextFeedback != null)
            {
                TextFeedback(aString);
            }
        }

        private ProcessCodes processCodesCache = null;

        public void InvalidateProcessCodes()
        {
            if (processCodesCache != null)
                processCodesCache.IsValid = false;
        }

        private bool ProcessCodesAreCached()
        {
            bool test = (processCodesCache != null);
            if (test)
            {
                test = processCodesCache.IsValid;
            }
            return test;
        }

        #region Select Data

        const string allProcessCodesCommand =
            @"SELECT * FROM [dbo].[tblProcessCodes] pc
               ORDER BY Machine_idJensen, Customer_idJensen, Article_idJensen ";

        public ProcessCodes GetAllProcessCodes()
        {
            if (ProcessCodesAreCached())
            {
                return processCodesCache;
            }
            try
            {
                const string commandString = allProcessCodesCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (processCodesCache==null) processCodesCache = new ProcessCodes();
                    command.DataFill(processCodesCache, SqlDataConnection.DBConnection.JensenGroup);
                    return processCodesCache;
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

        public ProcessCodes GetAllProcessCodes(Customer customer)
        {
            ProcessCodes processCodes = GetAllProcessCodes();
            ProcessCodes procCodes = new ProcessCodes();
            foreach (ProcessCode pc in processCodes)
            {
                if (pc.Customer_idJensen == customer.idJensen)
                {
                    procCodes.Add(pc);
                }
            }
            return procCodes;
        }

        public ProcessCodes GetAllProcessCodes(Machine machine)
        {
            ProcessCodes processCodes = GetAllProcessCodes();
            ProcessCodes procCodes = new ProcessCodes();
            foreach (ProcessCode pc in processCodes)
            {
                if (pc.Machine_idJensen == machine.idJensen)
                {
                    procCodes.Add(pc);
                }
            }
            return procCodes;
        }
                    
        public ProcessCode GetProcessCode(int codeID)
        {
            ProcessCodes processCodes = GetAllProcessCodes();
            ProcessCode processCode = processCodes.GetById(codeID);
            return processCode;
        }
        #endregion

        #region Insert Data
        public void InsertNewProcessCode(ProcessCode processCode)
        {
            const string commandString1 =
                @"INSERT INTO [dbo].[tblProcessCodes]
                  ( [Machine_idJensen]
                  , [Customer_idJensen]
                  , [Article_idJensen]
                  , [SortCategory_idJensen]
                  , [ProcessCode]
                  , [ProcessName]
                  , [Production_Norm]
                  , [Production_NoFlow_Time]
                  , [Count_Exit_Point]
                  )
                  VALUES
                  ( @Machine_idJensen
                  , @Customer_idJensen
                  , @Article_idJensen
                  , @SortCategory_idJensen
                  , @ProcessCode
                  , @ProcessName
                  , @Production_Norm
                  , @Production_NoFlow_Time
                  , @Count_Exit_Point
                  )

                  SELECT CAST(@@IDENTITY AS INT)";  
            
            const string commandString2 =
                @"INSERT INTO [dbo].[tblProcessCodes]
                  ( [Machine_idJensen]
                  , [Customer_idJensen]
                  , [Article_idJensen]
                  , [SortCategory_idJensen]
                  , [ProcessCode]
                  , [ProcessName]
                  , [Production_Norm]
                  , [Production_NoFlow_Time]
                  , [Count_Exit_Point]
                  , [StackCount]
                  )
                  VALUES
                  ( @Machine_idJensen
                  , @Customer_idJensen
                  , @Article_idJensen
                  , @SortCategory_idJensen
                  , @ProcessCode
                  , @ProcessName
                  , @Production_Norm
                  , @Production_NoFlow_Time
                  , @Count_Exit_Point
                  , @StackCount
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                if (!processCode.Retired)
                {
                    string commandString = commandString1;
                    if (DatabaseVersion >= 2.0)
                    {
                        commandString = commandString2;
                    }

                    using (SqlCommand command = new SqlCommand(commandString))
                    {
                        command.Parameters.AddWithValue("@Machine_idJensen", processCode.Machine_idJensen);
                        command.Parameters.AddWithValue("@Customer_idJensen", processCode.Customer_idJensen);
                        command.Parameters.AddWithValue("@Article_idJensen", processCode.Article_idJensen);
                        command.Parameters.AddWithValue("@SortCategory_idJensen", processCode.SortCategory_idJensen);
                        command.Parameters.AddWithValue("@ProcessCode", processCode.ProcessCodeID);
                        command.Parameters.AddWithValue("@ProcessName", processCode.ProcessName);
                        command.Parameters.AddWithValue("@Production_Norm", processCode.Production_Norm);
                        command.Parameters.AddWithValue("@Production_NoFlow_Time", processCode.Production_NoFlow_Time);
                        command.Parameters.AddWithValue("@Count_Exit_Point", processCode.Count_Exit_Point);
                        if (DatabaseVersion >= 2.0)
                        {
                            command.Parameters.AddWithValue("@StackCount", processCode.Stack_Count);
                        }

                        try
                        {
                            object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                            if (RecNum != null)
                            {
                                processCode.RecNum = (int)RecNum;
                                processCode.HasChanged = false;
                            }
                            InvalidateProcessCodes();
                        }
                        catch (SqlException ex)
                        {
                            const int insertError = 2601;
                            const int constraintError = 547;

                            if ((ex.Number != insertError) && (ex.Number != constraintError))
                            {
                                throw;
                            }
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
        public void UpdateProcessCodeDetails(ProcessCode processCode)
        {
            const string commandString1 =
                @"UPDATE [dbo].[tblProcessCodes]
                  SET Machine_idJensen = @Machine_idJensen
                    , Customer_idJensen = @Customer_idJensen
                    , Article_idJensen = @Article_idJensen
                    , SortCategory_idJensen = @SortCategory_idJensen
                    , ProcessCode = @ProcessCode
                    , ProcessName = @ProcessName
                    , Production_Norm = @Production_Norm
                    , Production_NoFlow_Time = @Production_NoFlow_Time
                    , Count_Exit_Point = @Count_Exit_Point
                  WHERE RecNum = @RecNum";

            const string commandString2 =
                @"UPDATE [dbo].[tblProcessCodes]
                  SET Machine_idJensen = @Machine_idJensen
                    , Customer_idJensen = @Customer_idJensen
                    , Article_idJensen = @Article_idJensen
                    , SortCategory_idJensen = @SortCategory_idJensen
                    , ProcessCode = @ProcessCode
                    , ProcessName = @ProcessName
                    , Production_Norm = @Production_Norm
                    , Production_NoFlow_Time = @Production_NoFlow_Time
                    , Count_Exit_Point = @Count_Exit_Point
                    , StackCount = @StackCount
                  WHERE RecNum = @RecNum";

            try
            {
                string commandString = commandString1;
                if (DatabaseVersion >= 2.0)
                {
                    commandString = commandString2;
                }
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", processCode.RecNum);
                    command.Parameters.AddWithValue("@Machine_idJensen", processCode.Machine_idJensen);
                    command.Parameters.AddWithValue("@Customer_idJensen", processCode.Customer_idJensen);
                    command.Parameters.AddWithValue("@Article_idJensen", processCode.Article_idJensen);
                    command.Parameters.AddWithValue("@SortCategory_idJensen", processCode.SortCategory_idJensen);
                    command.Parameters.AddWithValue("@ProcessCode", processCode.ProcessCodeID);
                    command.Parameters.AddWithValue("@ProcessName", processCode.ProcessName);
                    command.Parameters.AddWithValue("@Production_Norm", processCode.Production_Norm);
                    command.Parameters.AddWithValue("@Production_NoFlow_Time", processCode.Production_NoFlow_Time);
                    command.Parameters.AddWithValue("@Count_Exit_Point", processCode.Count_Exit_Point);
                    if (DatabaseVersion >= 2.0)
                    {
                        command.Parameters.AddWithValue("@StackCount", processCode.Stack_Count);
                    }
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    processCode.HasChanged = false;
                    InvalidateProcessCodes();
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

        #region Delete Data
        /// <summary>
        /// Delete a process code permanently from the database.
        /// </summary>
        /// <param name="processCode"></param>
        public void DeleteProcessCodeDetails(ProcessCode processCode)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblProcessCodes]
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", processCode.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateProcessCodes();
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

        #region sync codes

        public enum SyncType
        {
            Machine,
            Customer
        }

        public ProcessCodes CloneProcessCodeList(ProcessCodes existing, short sourceID, short destID, SyncType aSync)
        {
            ProcessCodes clones = new ProcessCodes();
            SqlDataAccess da = SqlDataAccess.singleton;
            Machines ms = da.GetAllMachines();
            foreach (ProcessCode pc in existing)
            {
                if (((aSync==SyncType.Machine)&&(pc.Machine_idJensen == sourceID))||
                    ((aSync==SyncType.Customer)&&(pc.Customer_idJensen==sourceID)))
                {
                    //copy
                    ProcessCode clone = pc.Clone();

                    //modify
                    clone.RecNum = -1;
                    if (aSync == SyncType.Machine)
                    {
                        clone.Machine_idJensen = destID;
                        //Machine m = ms[destID];
                        Machine m = ms.GetById(destID);
                        if (m != null)
                        {
                            clone.Count_Exit_Point = m.MachineCountExitPoint;
                        }
                    }
                    if (aSync == SyncType.Customer)
                    {
                        clone.Customer_idJensen = destID;
                    }

                    //add
                    clones.Add(clone);
                }
            }
            return clones;
        }

        public int ProcessCodeDifferences(ProcessCodes pcsA, ProcessCodes pcsB, short id, SyncType aSync)
        {
            int syncDiffCount = 0;
            foreach (ProcessCode pc in pcsA)
            {
                if (((aSync == SyncType.Machine) && (pc.Machine_idJensen == id)) ||
                     ((aSync == SyncType.Customer) && (pc.Customer_idJensen == id)))
                {
                    if (!pcsB.Contains(pc))
                    {
                        syncDiffCount++;
                    }
                }
            }
            return syncDiffCount;
        }

        public void ProcessCodeSync(ProcessCodes existingPCs, ProcessCodes newPCs, short id, SyncType aSync, bool canDelete)
        {
            //remove all codes from existing
            if (canDelete)
            {
                foreach (ProcessCode pc in existingPCs)
                {
                    if (((aSync == SyncType.Machine) && (pc.Machine_idJensen == id)) ||
                       ((aSync == SyncType.Customer) && (pc.Customer_idJensen == id)))
                    {
                        pc.Retired = true;
                    }
                }
            }
            //copy all codes from source to dest
            foreach (ProcessCode pc in newPCs)
            {
                if (((aSync == SyncType.Machine) && (pc.Machine_idJensen == id)) ||
                     ((aSync == SyncType.Customer) && (pc.Customer_idJensen == id)))
                {
                    existingPCs.Add(pc);
                }
            }
        }

        public void StoreAllProcessCodes(ProcessCodes pcs)
        {
            using (SqlCommand command = new SqlCommand())
            {
                try
                {
                    int tot = pcs.Count;
                    int i = 0;
                    int perc = 0;

                    foreach (ProcessCode pc in pcs)
                    {
                        perc = 100 * i / tot;
                        i++;
                        Feedback(perc);
                        //insert it if its new
                        if (pc.IsNew)
                        {
                            InsertNewProcessCode(pc);
                        }
                        if (pc.HasChanged && !pc.IsNew && !pc.Retired)
                        {
                            UpdateProcessCodeDetails(pc);
                        }
                        if (pc.Retired)
                        {
                            DeleteProcessCodeDetails(pc);
                        }
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Store All Process Codes failed with :" + ex.Message);
                }
            }
        }

        public void UpdateProcessCodeNorms(int machineID, int oldNorm, int newNorm)
        {
            const string commandString =
                @"UPDATE [dbo].[tblProcessCodes]
                  SET Production_Norm = @NewNorm 
                  WHERE Machine_idJensen = @MachineID
                    AND Production_Norm = @OldNorm";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@MachineID", machineID);
                    command.Parameters.AddWithValue("@OldNorm", oldNorm);
                    command.Parameters.AddWithValue("@NewNorm", newNorm);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateProcessCodes();
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

        public void UpdateProcessCodeNoFlows(int machineID, int oldNoFlow, int newNoFlow)
        {
            const string commandString =
                @"UPDATE [dbo].[tblProcessCodes]
                  SET Production_Noflow_Time = @NewNoFlow 
                  WHERE Machine_idJensen = @MachineID
                    AND Production_Noflow_Time = @OldNoFlow";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@MachineID", machineID);
                    command.Parameters.AddWithValue("@OldNoFlow", oldNoFlow);
                    command.Parameters.AddWithValue("@NewNoFlow", newNoFlow);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateProcessCodes();
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
    public class ProcessCodes : List<ProcessCode>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblProcessCodes";
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
            SqlDataAccess da = SqlDataAccess.Singleton;

            int RecNumPos = dr.GetOrdinal("RecNum");
            int Machine_idJensenPos = dr.GetOrdinal("Machine_idJensen");
            int Customer_idJensenPos = dr.GetOrdinal("Customer_idJensen");
            int Article_idJensenPos = dr.GetOrdinal("Article_idJensen");
            int SortCategory_idJensenPos = dr.GetOrdinal("SortCategory_idJensen");
            int ProcessCodePos = dr.GetOrdinal("ProcessCode");
            int ProcessNamePos = dr.GetOrdinal("ProcessName");
            int Production_NormPos = dr.GetOrdinal("Production_Norm");
            int Production_NoFlow_TimePos = dr.GetOrdinal("Production_NoFlow_Time");
            int Count_Exit_PointPos = dr.GetOrdinal("Count_Exit_Point");
            int Stack_CountPos = 0;
            if (da.DatabaseVersion >= 2.0)
            {
                Stack_CountPos = dr.GetOrdinal("StackCount");
            }

            this.Clear();
            while (dr.Read())
            {
                var processCode = new ProcessCode();
                processCode.RecNum = dr.GetInt32(RecNumPos);
                if (da.DatabaseVersion >= 2.1)
                {
                    processCode.Machine_idJensen = dr.GetInt32(Machine_idJensenPos);
                    processCode.Customer_idJensen = dr.GetInt32(Customer_idJensenPos);
                    processCode.Article_idJensen = dr.GetInt32(Article_idJensenPos);
                    processCode.SortCategory_idJensen = dr.GetInt32(SortCategory_idJensenPos);
                    processCode.ProcessCodeID = dr.GetInt32(ProcessCodePos);
                }
                else
                {
                    processCode.Machine_idJensen = dr.GetInt16(Machine_idJensenPos);
                    processCode.Customer_idJensen = dr.GetInt16(Customer_idJensenPos);
                    processCode.Article_idJensen = dr.GetInt16(Article_idJensenPos);
                    processCode.SortCategory_idJensen = dr.GetInt16(SortCategory_idJensenPos);
                    processCode.ProcessCodeID = dr.GetInt16(ProcessCodePos);
                }
                processCode.ProcessName = dr.GetString(ProcessNamePos);
                processCode.Production_Norm = dr.GetInt32(Production_NormPos);
                processCode.Production_NoFlow_Time = dr.GetInt32(Production_NoFlow_TimePos);
                processCode.Count_Exit_Point = dr.GetBoolean(Count_Exit_PointPos);
                if (da.DatabaseVersion >= 2.0)
                    processCode.Stack_Count = dr.GetInt32(Stack_CountPos);
                processCode.HasChanged = false;

                this.Add(processCode);

            }
            lastRead = da.ServerTime;
            lastDBUpdate = lastRead;
            isValid = true;
            neverExpire = false;

            return this.Count;
        }

        public ProcessCode GetById(int id)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return processCode.ProcessCodeID == id;
            });
        }
        public ProcessCode GetByIds(int pcID, int custID, int artID, int machID)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return ((processCode.ProcessCodeID == pcID)
                    && (processCode.Article_idJensen == artID)
                    && (processCode.Customer_idJensen == custID)
                    && (processCode.Machine_idJensen == machID));
            });
        }
        public ProcessCode GetByIds(int custID, int artID, int machID)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return ((processCode.Customer_idJensen == custID)
                    && (processCode.Article_idJensen == artID)
                    && (artID > -1)
                    && (processCode.Machine_idJensen == machID));
            });
        }

        public ProcessCode GetByCustArtCatMachine(int custID, int artID, int catID, int machID)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return ((processCode.Customer_idJensen == custID)
                    && (processCode.Article_idJensen == artID)
                    && (processCode.SortCategory_idJensen == catID)
                    && (processCode.Machine_idJensen == machID));
            });
        }

        public ProcessCode GetByCustArtMachine(int custID, int artID, int machID)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return ((processCode.Customer_idJensen == custID)
                    && (processCode.Article_idJensen == artID)
                    && (processCode.Machine_idJensen == machID));
            });
        }

        public ProcessCode GetByCustCatMachine(int custID, int catID, int machID)
        {
            return this.Find(delegate(ProcessCode processCode)
            {
                return ((processCode.Customer_idJensen == custID)
                    && (processCode.SortCategory_idJensen == catID)
                    && (processCode.Machine_idJensen == machID));
            });
        }
        #region machine alias codes
        public void AddMachineAliasCodes()
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            MachineLogAliases mlas = da.GetAllMachineAliases(true);
            if (mlas != null)
            {
                ProcessCodes deletePCs = new ProcessCodes();
                foreach (MachineLogAlias mla in mlas)
                {
                    Machine sourceM = da.GetMachine(mla.SourceID);
                    Machine destM = da.GetMachine(mla.DestID);
                    if (sourceM != null)
                    {
                        ProcessCodes sourcePCs = da.GetAllProcessCodes(sourceM);
                        if (sourcePCs != null)
                        {
                            foreach (ProcessCode spc in sourcePCs)
                            {
                                ProcessCode delPC = GetByCustArtCatMachine(spc.Customer_idJensen, spc.Article_idJensen, spc.SortCategory_idJensen, mla.DestID);
                                if (delPC != null)
                                    deletePCs.Add(delPC);
                                ProcessCode aliasPC = spc.Copy();
                                aliasPC.Machine_idJensen=(short)mla.DestID;
                                Add(aliasPC);
                            }
                        }
                    }
                }
                foreach (ProcessCode delPC in deletePCs)
                {
                    Remove(delPC);
                }
            }
        }
        #endregion


        public void AddNames(Articles articles, Customers customers)
        {
            Article article = null;
            Customer customer = null;
            foreach (var processCode in this)
            {
                if (article == null || article.idJensen != processCode.Article_idJensen)
                    article = articles.GetById(processCode.Article_idJensen);
                if (article != null)
                    processCode.Article_ShortDescription = article.ShortDescription;
                if (customer == null || customer.idJensen != processCode.Customer_idJensen)
                    customer = customers.GetById(processCode.Customer_idJensen);
                if (customer != null)
                    processCode.Customer_ShortDescription = customer.ShortDescription;
            }
        }
    }
    #endregion

    #region Item Class
    public class ProcessCode : DataItem, IEquatable<ProcessCode>
    {
        ProcessNames pns;

        #region Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal int Machine_idJensen;
            internal int Customer_idJensen;
            internal int Article_idJensen;
            internal int SortCategory_idJensen;
            internal int ProcessCodeID;
            internal string ProcessName;
            internal int Production_Norm;
            internal int Production_NoFlow_Time;
            internal bool Count_Exit_Point;
            internal int Stack_Count;
            internal bool Retired;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public ProcessCode()
        {
            ActiveData = (ICopyableObject)new DataRecord();
            SqlDataAccess da = SqlDataAccess.Singleton;
            activeData.Retired = false;
            pns = da.GetProcessNames();
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

        public bool Retired
        {
            get
            {
                return this.activeData.Retired;
            }
            set
            {
                this.activeData.Retired = value;
            }
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
                this.activeData.RecNum = value;
                HasChanged = true;
            }
        }

        public int Machine_idJensen
        {
            get
            {
                return this.activeData.Machine_idJensen;
            }
            set
            {
                this.activeData.Machine_idJensen = value;
                HasChanged = true;
            }
        }

        public string Machine_ShortDescription
        {
            get
            {
                string mdesc = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null) mdesc = m.ShortDescription;
                return mdesc;
            }
        }

        public int Machine_SubIDs
        {
            get
            {
                int subs = 1;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null) subs = m.Positions;
                return subs;
            }
        }

        public bool IsDefaultCountExit
        {
            get
            {
                bool cxp = false;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null) cxp = (m.MachineCountExitPoint && m.UseProcessCodeCount);
                return cxp;
            }
        }

        public int Customer_idJensen
        {
            get
            {
                return this.activeData.Customer_idJensen;
            }
            set
            {
                this.activeData.Customer_idJensen = value;
                HasChanged = true;
            }
        }

        private string _customerShortDescription;

        public string Customer_ShortDescription
        {
            get
            {
                //if (!string.IsNullOrEmpty(_customerShortDescription))
                //    return _customerShortDescription;
                //else
                //{
                SqlDataAccess da = SqlDataAccess.Singleton;
                Customer c = da.GetCustomer(this.activeData.Customer_idJensen);
                if (c != null) _customerShortDescription = c.ShortDescription;
                return _customerShortDescription;
                //}
            }
            set { _customerShortDescription = value; }

        }

        public int Article_idJensen
        {
            get
            {
                return this.activeData.Article_idJensen;
            }
            set
            {
                HasChanged |= (this.activeData.Article_idJensen != value);
                this.activeData.Article_idJensen = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null)
                {
                    if (!m.ProcessCodeCategory)
                    {
                        this.activeData.SortCategory_idJensen = -1;
                    }
                }
            }
        }

        private string _articleShortDescription;
        public string Article_ShortDescription
        {
            get
            {
                //if (!string.IsNullOrEmpty(_articleShortDescription))
                //    return _articleShortDescription;
                //else
                //{
                SqlDataAccess da = SqlDataAccess.Singleton;
                Article a = da.GetArticle(this.activeData.Article_idJensen);
                if (a != null) _articleShortDescription = a.ShortDescription;
                return _articleShortDescription;
                //}
            }
            set { _articleShortDescription = value; }
        }

        public int SortCategory_idJensen
        {
            get
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null)
                {
                    if (m.ProcessCodeArticle)
                    {
                        return -1;
                    }
                    else
                    {
                        return this.activeData.SortCategory_idJensen;
                    }
                }
                else return this.activeData.SortCategory_idJensen;
            }
            set
            {
                HasChanged |= (this.activeData.SortCategory_idJensen != value);
                this.activeData.SortCategory_idJensen = value;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null)
                {
                    if (m.ProcessCodeCategory)
                    {
                        this.activeData.Article_idJensen = -1;
                    }
                }
            }
        }

        public string SortCategory_ShortDescription
        {
            get
            {
                string sdesc = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                SortCategory s = da.GetSortCategory(this.activeData.SortCategory_idJensen);
                if (s != null) sdesc = s.ShortDescription;
                return sdesc;
            }
        }

        private void EvaluateProcessName()
        {
            ProcessName pn=pns.GetByIDs(this.Machine_idJensen, this.ProcessCodeID);
            if (pn != null)
            {
                this.activeData.ProcessName = pn.ProcName.Trim();
            }
        }

        public int ProcessCodeID
        {
            get
            {
                return this.activeData.ProcessCodeID;
            }
            set
            {
                this.activeData.ProcessCodeID = value;
                HasChanged = true;
            }
        }

        public string ProcessName
        {
            get
            {
                string pns = this.activeData.ProcessName.Trim();
                if (pns == string.Empty)
                {
                    EvaluateProcessName();
                }
                return this.activeData.ProcessName;
            }
            set
            {
                this.activeData.ProcessName = value;
                if (this.activeData.ProcessName == string.Empty)
                {
                   EvaluateProcessName();
                }
                HasChanged = true;
            }
        }

        public int Production_Norm
        {
            get
            {
                return this.activeData.Production_Norm;
            }
            set
            {
                this.activeData.Production_Norm = value;
                HasChanged = true;
            }
        }

        public int Production_NoFlow_Time
        {
            get
            {
                return this.activeData.Production_NoFlow_Time;
            }
            set
            {
                this.activeData.Production_NoFlow_Time = value;
                HasChanged = true;
            }
        }

        public bool Count_Exit_Point
        {
            get
            {
                return this.activeData.Count_Exit_Point;
            }
            set
            {
                this.activeData.Count_Exit_Point = value;
                HasChanged = true;
            }
        }
        public int Stack_Count
        {
            get
            {
                return this.activeData.Stack_Count;
            }
            set
            {
                this.activeData.Stack_Count = value;
                HasChanged = true;
            }
        }
        public int Machine_ProcessCodeType
        {
            get
            {
                int mPCType = 0;
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machine m = da.GetMachine(this.activeData.Machine_idJensen);
                if (m != null) mPCType = m.ProcessCodeType;
                return mPCType;
            }
        }

        public string Article_Category_Desc
        {
            get
            {
                string aDesc = string.Empty;
                if (Machine_ProcessCodeType == 0)
                {
                    aDesc = Article_ShortDescription;
                }
                if (Machine_ProcessCodeType == 1)
                {
                    aDesc = SortCategory_ShortDescription;
                }
                if (Machine_ProcessCodeType == 2)
                {
                    aDesc = Article_ShortDescription + ", " + SortCategory_ShortDescription;
                }
                return aDesc;
            }
        }

        public string ProcessCode_Title
        {
            get
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                Machines ms = da.GetAllMachines();
                Machine m = ms.GetById(Machine_idJensen);
                Customers cs = da.GetAllActiveCustomers();
                Customer c = cs.GetById(Customer_idJensen);
                string aTitle = string.Empty;
                if (m != null)
                    aTitle = c.ShortDescription + " ";
                if (c != null)
                    aTitle += m.ShortDescription + " ";
                aTitle += Article_Category_Desc;
                return aTitle;
            }
        }


        #endregion

        #region Equatable
        public bool Equals(ProcessCode other)
        {
            if (this.Article_idJensen == other.Article_idJensen
                & this.Customer_idJensen == other.Customer_idJensen
                & this.Machine_idJensen == other.Machine_idJensen
                & this.SortCategory_idJensen == other.SortCategory_idJensen
                & this.ProcessCodeID == other.ProcessCodeID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    
        #endregion

        #region Clone
        public ProcessCode Clone()
        {
            ProcessCode processCode = new ProcessCode()
            {
                RecNum = this.RecNum,
                Machine_idJensen = this.Machine_idJensen,
                Customer_idJensen = this.Customer_idJensen,
                Article_idJensen = this.Article_idJensen,
                SortCategory_idJensen = this.SortCategory_idJensen,
                ProcessCodeID = this.ProcessCodeID,
                ProcessName = this.ProcessName,
                Production_Norm = this.Production_Norm,
                Production_NoFlow_Time = this.Production_NoFlow_Time,
                Count_Exit_Point = this.Count_Exit_Point,
                Stack_Count = this.Stack_Count,
                HasChanged = false
            };
            return processCode;
        }

        #endregion
        #region copy

        public ProcessCode Copy()
        {
            ProcessCode ldRec = (ProcessCode)MemberwiseClone();
            return ldRec;
        }

        #endregion
    }
    #endregion
}
