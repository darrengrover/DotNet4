using System;
using System.Collections.Generic;
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
        #region cache
        private DiagramPositions diagramPositionsCache = null;

        public void InvalidateDiagramPositions()
        {
            if (diagramPositionsCache != null)
                diagramPositionsCache.IsValid = false;
        }

        private bool DiagramPositionsAreCached()
        {
            bool test = (diagramPositionsCache != null);
            if (test)
            {
                test = diagramPositionsCache.IsValid;
            }
            return test;
        }
        # endregion

        #region Select Data
        const string allDiagramPositionsCommand =
            @"SELECT [DiagramPositionID]
                  ,[DiagramID]
                  ,[X]
                  ,[Y]
                  ,[MachineID]
                  ,[PLCPositionID]
              FROM [JEGR_DB].[dbo].[tblDiagramPositions]";

        public DiagramPositions GetAllDiagramPositions() 
        {
            if (DiagramPositionsAreCached())
            {
                return diagramPositionsCache;
            }
            try
            {
                const string commandString = allDiagramPositionsCommand;                  

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (diagramPositionsCache == null) diagramPositionsCache = new DiagramPositions();
                    command.DataFill(diagramPositionsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return diagramPositionsCache;
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

        public DiagramPosition GetDiagramPosition(int diagramPositionID)
        {
            DiagramPositions diagramPositions = GetAllDiagramPositions();
            DiagramPosition diagramPosition = diagramPositions.GetById(diagramPositionID);

            return diagramPosition;
         }
        #endregion

        #region Next Record

        public int NextDiagramPositionRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblDiagramPositions',
		                                            @idName = N'DiagramPositionID'
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
        public void InsertNewDiagramPosition(DiagramPosition diagramPosition)
        {
            const string commandString =
                @"INSERT INTO [dbo].[tblDiagramPositions]
                       ([DiagramPositionID]
                       ,[DiagramID]
                       ,[X]
                       ,[Y]
                       ,[MachineID]
                       ,[PLCPositionID])
                 VALUES
                       (@DiagramPositionID
                       ,@DiagramID
                       ,@X
                       ,@Y
                       ,@MachineID
                       ,@PLCPositionID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramPositionID", diagramPosition.DiagramPositionID);
                    command.Parameters.AddWithValue("@DiagramID", diagramPosition.DiagramID);
                    command.Parameters.AddWithValue("@X", diagramPosition.X);
                    command.Parameters.AddWithValue("@Y", diagramPosition.Y);
                    command.Parameters.AddWithValue("@MachineID", diagramPosition.MachineID);
                    command.Parameters.AddWithValue("@PLCPositionID", diagramPosition.PLCPositionID);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                        diagramPosition.HasChanged = false;
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
        public void UpdateDiagramPositionDetails(DiagramPosition diagramPosition)
        {
            const string commandString =
                @"UPDATE [JEGR_DB].[dbo].[tblDiagramPositions]
                   SET [DiagramID] = @DiagramID
                      ,[X] = @X
                      ,[Y] = @Y
                      ,[MachineID] = @MachineID
                      ,[PLCPositionID] = @PLCPositionID
                 WHERE [DiagramPositionID] = @DiagramPositionID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramPositionID", diagramPosition.DiagramPositionID);
                    command.Parameters.AddWithValue("@DiagramID", diagramPosition.DiagramID);
                    command.Parameters.AddWithValue("@X", diagramPosition.X);
                    command.Parameters.AddWithValue("@Y", diagramPosition.Y);
                    command.Parameters.AddWithValue("@MachineID", diagramPosition.MachineID);
                    command.Parameters.AddWithValue("@PLCPositionID", diagramPosition.PLCPositionID);

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    diagramPosition.HasChanged = false;
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
        public void DeleteDiagramPosition(DiagramPosition diagramPosition)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblDiagramPositions]
                    WHERE DiagramPositionID = @DiagramPositionID";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@DiagramPositionID", diagramPosition.DiagramPositionID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateDiagramPositions();
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
    public class DiagramPositions : DataList<DiagramPosition>, IDataFiller
    {

        private double lifespan = 1.0;
        private string tblName = "tblDiagramPositions";
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
            int DiagramPositionIDPos = dr.GetOrdinal("DiagramPositionID");
            int DiagramIDPos = dr.GetOrdinal("DiagramID");
            int XPos = dr.GetOrdinal("X");
            int YPos = dr.GetOrdinal("X");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int PLCPositionIDPos = dr.GetOrdinal("PLCPositionID");

            this.Clear();
            while (dr.Read())
            {
                DiagramPosition diagramPosition = new DiagramPosition()
                {
                    DiagramPositionID = dr.GetInt32(DiagramPositionIDPos),
                    DiagramID = dr.GetInt32(DiagramIDPos),
                    X = dr.GetDouble(XPos),
                    Y = dr.GetDouble(YPos),
                    MachineID = dr.GetInt32(MachineIDPos),
                    PLCPositionID = dr.GetInt32(PLCPositionIDPos),
                    HasChanged = false
                };

                this.Add(diagramPosition);
            }
            LastRead = DateTime.Now;
            IsValid = true;

            return this.Count;
        }

        public DiagramPosition GetById(int id)
        {
            return this.Find(delegate(DiagramPosition diagramPosition)
            {
                return diagramPosition.DiagramPositionID == id;
            });
        }
    }
    #endregion

    #region Item Class
    public class DiagramPosition : DataItem
    {
        #region DiagramPosition Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int DiagramPositionID;
            internal int DiagramID;
            internal double X;
            internal double Y;
            internal int MachineID;
            internal int PLCPositionID;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public DiagramPosition()
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
                return this.activeData.DiagramPositionID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.DiagramPositionID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int DiagramPositionID
        {
            get
            {
                return this.activeData.DiagramPositionID;
            }
            set
            {
                this.activeData.DiagramPositionID = value;
                HasChanged = true;
            }
        }

        public int DiagramID
        {
            get
            {
                return this.activeData.DiagramID;
            }
            set
            {
                this.activeData.DiagramID = value;
                HasChanged = true;
            }
        }


        public double X
        {
            get
            {
                return this.activeData.X;
            }
            set
            {
                this.activeData.X = value;
                HasChanged = true;
            }
        }

        public double Y
        {
            get
            {
                return this.activeData.Y;
            }
            set
            {
                this.activeData.Y = value;
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

        public int PLCPositionID
        {
            get
            {
                return this.activeData.PLCPositionID;
            }
            set
            {
                this.activeData.PLCPositionID = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
