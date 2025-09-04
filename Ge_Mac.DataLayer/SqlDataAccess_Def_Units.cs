using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data

        private Def_Units def_UnitsCache = null;
        public void InvalidateDef_Units()
        {
            if (def_UnitsCache != null)
                def_UnitsCache.IsValid = false;
        }

        private bool Def_UnitsAreCached()
        {
            bool test = (def_UnitsCache != null);
            if (test)
            {
                test = def_UnitsCache.IsValid;
            }
            return test;
        }

        const string AllDef_UnitsCommand =
            @"SELECT RecNum
                   , idJensen
                   , ShortDescription
                   , LongDescription
                   , AllowedKPIUnit
              FROM [dbo].[tblDef_Units]
                ORDER BY ShortDescription";
        
        const string AllKPIDef_UnitsCommand =
            @"SELECT RecNum
                   , idJensen
                   , ShortDescription
                   , LongDescription
                   , AllowedKPIUnit
              FROM [dbo].[tblDef_Units]
              WHERE AllowedKPIUnit = 1
                ORDER BY ShortDescription";

        public Def_Units GetAllDef_Units()
        {
            if (Def_UnitsAreCached())
            {
                return def_UnitsCache;
            }
            try
            {
                const string commandString = AllDef_UnitsCommand ;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (def_UnitsCache == null) def_UnitsCache = new Def_Units();
                    command.DataFill(def_UnitsCache, SqlDataConnection.DBConnection.JensenGroup);
                    return def_UnitsCache;
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

        public Def_Units GetAllKPIDef_Units() // not cached 
        {
            try
            {
                const string commandString = AllKPIDef_UnitsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    Def_Units def_Units = new Def_Units();
                    command.DataFill(def_Units, SqlDataConnection.DBConnection.JensenGroup);
                    return def_Units;
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

        public Def_Unit GetDef_Unit(int idJensen)
        {
            Def_Units def_units = GetAllDef_Units();
            Def_Unit def_unit = def_units.GetById(idJensen);
            return def_unit;
        }

        #endregion

        #region Insert Data
        public void InsertNewDef_Unit(Def_Unit def_Unit)
        {
            const string commandString =
                @"INSERT INTO [dbo].tblDef_Units
                  ( idJensen
                  , ShortDescription
                  , LongDescription
                  , AllowedKPIUnit
                  )
                  VALUES
                  ( @idJensen
                  , @ShortDescription
                  , @LongDescription
                  , @AllowedKPIUnit
                  )

                  SELECT CAST(@@IDENTITY AS INT)";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@idJensen", def_Unit.idJensen);
                    command.Parameters.AddWithValue("@ShortDescription", def_Unit.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", def_Unit.LongDescription);
                    command.Parameters.AddWithValue("@AllowedKPIUnit", def_Unit.AllowedKPIUnit);

                    try
                    {
                        object RecNum = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);

                        if (RecNum != null)
                        {
                            def_Unit.RecNum = (int)RecNum;
                            def_Unit.HasChanged = false;
                        }
                        InvalidateDef_Units();
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
        public void UpdateDef_UnitDetails(Def_Unit def_Unit)
        {
            const string commandString =
                @"UPDATE [dbo].tblDef_Units
                  SET idJensen = @idJensen
                    , ShortDescription = @ShortDescription
                    , LongDescription = @LongDescription
                    , AllowedKPIUnit = @AllowedKPIUnit

                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", def_Unit.RecNum);
                    command.Parameters.AddWithValue("@idJensen", def_Unit.idJensen);
                    command.Parameters.AddWithValue("@ShortDescription", def_Unit.ShortDescription);
                    command.Parameters.AddWithValue("@LongDescription", def_Unit.LongDescription);
                    command.Parameters.AddWithValue("@AllowedKPIUnit", def_Unit.AllowedKPIUnit);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    def_Unit.HasChanged = false;
                }
                InvalidateDef_Units();
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
        /// Delete a def unit permanently from the database.
        /// </summary>
        /// <param name="def_Unit"></param>
        public void DeleteDef_UnitDetails(Def_Unit def_Unit)
        {
            const string commandString =
                @"DELETE FROM [dbo].tblDef_Units
                  WHERE RecNum = @RecNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", def_Unit.RecNum);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                }
                InvalidateDef_Units();
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
    public class Def_Units : List<Def_Unit>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblDef_Units";
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


        #region Field Positions
        int RecNumPos;
        int idJensenPos;
        int ShortDescriptionPos;
        int LongDescriptionPos;
        int AllowedKPIUnitPos;

        public void SetFieldPositions(SqlDataReader dr)
        {
            RecNumPos = dr.GetOrdinal("RecNum");
            idJensenPos = dr.GetOrdinal("idJensen");
            ShortDescriptionPos = dr.GetOrdinal("ShortDescription");
            LongDescriptionPos = dr.GetOrdinal("LongDescription");
            AllowedKPIUnitPos = dr.GetOrdinal("AllowedKPIUnit");
        }
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Machines Collection
                Def_Unit machine = FillDef_Unit(dr);
                this.Add(machine);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public Def_Unit FillDef_Unit(SqlDataReader dr)
        {
            Def_Unit def_Unit = new Def_Unit()
            {
                RecNum = dr.GetInt32(RecNumPos),
                idJensen = dr.GetInt16(idJensenPos),
                ShortDescription = dr.GetString(ShortDescriptionPos),
                LongDescription = dr.GetString(LongDescriptionPos),
                AllowedKPIUnit = dr.GetBoolean(AllowedKPIUnitPos)
            };

            return def_Unit;
        }

        public Def_Unit GetById(int id)
        {
            return this.Find(delegate(Def_Unit def_Unit)
            {
                return def_Unit.idJensen == id;
            });
        }
    }


    #endregion

    #region Item Class

    public class Def_Unit : DataItem
    {
        #region Def_Units Data Record
        protected class DataRecord : ICopyableObject
        {
            internal int RecNum;
            internal short idJensen;
            internal string ShortDescription;
            internal string LongDescription;
            internal bool AllowedKPIUnit;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public Def_Unit()
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

        public short idJensen
        {
            get
            {
                return this.activeData.idJensen;
            }
            set
            {
                this.activeData.idJensen = value;
                HasChanged = true;
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
                this.activeData.ShortDescription = value;
                HasChanged = true;
            }
        }

        public string LongDescription
        {
            get
            {
                return this.activeData.LongDescription;
            }
            set
            {
                this.activeData.LongDescription = value;
                HasChanged = true;
            }
        }

        public bool AllowedKPIUnit
        {
            get
            {
                return this.activeData.AllowedKPIUnit;
            }
            set
            {
                this.activeData.AllowedKPIUnit = value;
                HasChanged = true;
            }
        }

        #endregion
    }
    #endregion
}
