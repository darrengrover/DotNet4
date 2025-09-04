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

        const string AllDischargerCallPatterns =
            @"SELECT PatternID
                   , PatternDescription
                   , Customer
                   , AutoSkip
              FROM [dbo].[tblDischargerCall_Patterns]";

        public DischargerCall_Patterns GetAllDischargerCall_Patterns()
        {
            try
            {
                const string commandString = AllDischargerCallPatterns;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    DischargerCall_Patterns patterns = new DischargerCall_Patterns();
                    command.DataFill(patterns, SqlDataConnection.DBConnection.Rail);
                    return patterns;
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

        #region Next Record

        public int NextPatternRecord()
        {
            const string commandString = @"DECLARE	@return_value int
                                            EXEC	@return_value = [dbo].[FirstID]
		                                            @TableName = N'tblDischargerCall_Patterns',
		                                            @idName = N'PatternID'
                                         SELECT @return_value";

            int nextID = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.Rail);
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
            return (int)nextID;
        }

        #endregion

        #region Insert Data
        public int InsertNewPattern(DischargerCall_Pattern pattern)
        {
            const string commandString =
                @"INSERT INTO [dbo].tblDischargerCall_Patterns
                  ( PatternID
                  , PatternDescription
                  , Customer
                  , AutoSkip
                  )
                  VALUES
                  ( @PatternID
                  , @PatternDescription
                  , @Customer
                  , @AutoSkip
                  )

                  SELECT @@ROWCOUNT";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", pattern.PatternID);
                    command.Parameters.AddWithValue("@PatternDescription", pattern.PatternDescription);
                    command.Parameters.AddWithValue("@Customer", pattern.Customer);
                    command.Parameters.AddWithValue("@AutoSkip", pattern.AutoSkip);

                    try
                    {
                        object patternID = command.ExecuteScalar(SqlDataConnection.DBConnection.Rail);

                        if (patternID != null)
                        {
                            pattern.PatternID = (int)patternID;
                            pattern.HasChanged = false;
                        }
                        return pattern.PatternID;
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;

                        if (ex.Number != insertError)
                        {
                            throw;
                        }
                        return -1;
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
                return -1;
                throw;
            }
        }
        #endregion

        #region Update Data
        public int UpdateDischargerCall_Pattern(DischargerCall_Pattern pattern)
        {
            const string commandString =
                @"UPDATE [dbo].tblDischargerCall_Patterns
                  SET PatternDescription = @PatternDescription
                    , Customer = @Customer
                    , AutoSkip = @AutoSkip

                  WHERE PatternID = @PatternID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", pattern.PatternID);
                    command.Parameters.AddWithValue("@PatternDescription", pattern.PatternDescription);
                    command.Parameters.AddWithValue("@Customer", pattern.Customer);
                    command.Parameters.AddWithValue("@AutoSkip", pattern.AutoSkip);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                    pattern.HasChanged = false;
                }
                return 0;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                return -1;
                throw;
            }
        }

        #endregion

        #region Delete Data
        /// <summary>
        /// Delete a call pattern permanently from the database.
        /// </summary>
        /// <param name="customer"></param>
        public int DeleteDischargerCall_Pattern(DischargerCall_Pattern pattern)
        {
            const string commandString =
                @"DELETE FROM [dbo].tblDischargerCall_Patterns
                  WHERE PatternID = @PatternID";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", pattern.PatternID);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                }
                return 0;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }
                return -1;
                throw;
            }
        }

        #endregion
    }

    #region Data Collection Class
    public class DischargerCall_Patterns : List<DischargerCall_Pattern>, IDataFiller
    {
 
        #region Field Positions
        int PatternIDPos;
        int PatternDescriptionPos;
        int CustomerPos;
        int AutoSkipPos;

        public DischargerCall_Pattern GetById(int id)
        {
            return this.Find(delegate(DischargerCall_Pattern pattern)
            {
                return pattern.PatternID == id;
            });
        }

        public DischargerCall_Pattern GetByDescription(string patterndescription)
        {
            return this.Find(delegate(DischargerCall_Pattern pattern)
            {
                return pattern.PatternDescription == patterndescription;
            });
        }


        public void SetFieldPositions(SqlDataReader dr)
        {
            PatternIDPos = dr.GetOrdinal("PatternID");
            PatternDescriptionPos = dr.GetOrdinal("PatternDescription");
            CustomerPos = dr.GetOrdinal("Customer");
            AutoSkipPos = dr.GetOrdinal("AutoSkip");
        }
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Collection
                DischargerCall_Pattern pattern = FillPattern(dr);
                this.Add(pattern);
            }
            return this.Count;
        }

        public DischargerCall_Pattern FillPattern(SqlDataReader dr)
        {
            DischargerCall_Pattern pattern = new DischargerCall_Pattern()
            {
                PatternID = dr.GetInt32(PatternIDPos),
                PatternDescription = dr.GetString(PatternDescriptionPos),
                Customer = dr.GetInt32(CustomerPos),
                AutoSkip = dr.GetInt32(AutoSkipPos),
                HasChanged = false
            };

            return pattern;
        }

    }
    #endregion

    #region Item Class

    public class DischargerCall_Pattern : DataItem
    {
        #region Discharger Call Pattern
        protected class DataRecord : ICopyableObject
        {
            internal int PatternID;
            internal string PatternDescription;
            internal int Customer;
            internal int AutoSkip;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public DischargerCall_Pattern()
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
                return this.activeData.PatternID == -1;
            }
        }

        /// <summary>The record exists in the database</summary>
        public override bool IsExisting
        {
            get
            {
                return this.activeData.PatternID != -1;
            }
        }
        #endregion

        #region Data Column Properties

        public int PatternID
        {
            get
            {
                return this.activeData.PatternID;
            }
            set
            {
                this.activeData.PatternID = value;
                HasChanged = true;
            }
        }

        public string PatternDescription
        {
            get
            {
                return this.activeData.PatternDescription;
            }
            set
            {
                this.activeData.PatternDescription = value;
                HasChanged = true;
            }
        }

        public int Customer
        {
            get
            {
                return this.activeData.Customer;
            }
            set
            {
                this.activeData.Customer = value;
                HasChanged = true;
            }
        }

        public int AutoSkip
        {
            get
            {
                return this.activeData.AutoSkip;
            }
            set
            {
                this.activeData.AutoSkip = value;
                HasChanged = true;
            }
        }

        #endregion
    }

    #endregion
}
