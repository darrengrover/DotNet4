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

        const string AllDischargerCallPatternsSub =
            @"SELECT PatternID
                   , StepNum
                   , CategID
                   , Quantity
                   , Mode
              FROM [dbo].[tblDischargerCall_Patterns_Sub]";

        public DischargerCall_PatternsSubs GetAllDischargerCall_PatternsSubs()
        {
            try
            {
                const string commandString = AllDischargerCallPatternsSub + " ORDER BY PatternID, StepNum";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    DischargerCall_PatternsSubs patterns = new DischargerCall_PatternsSubs();
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

        public DischargerCall_PatternsSubs GetAllDischargerCall_PatternsSubs(int PatternID)
        {
            try
            {
                const string commandString = AllDischargerCallPatternsSub + " WHERE PatternID = @PatternID ORDER BY StepNum";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    DischargerCall_PatternsSubs patterns = new DischargerCall_PatternsSubs();
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

//        public int NextPatternRecord()
//        {
//            const string commandString = @"DECLARE	@return_value int
//                                            EXEC	@return_value = [dbo].[FirstID]
//		                                            @TableName = N'tblDischargerCall_PatternsSub',
//		                                            @idName = N'PatternID'
//                                         SELECT @return_value";

//            int nextID = 0;
//            try
//            {
//                using (SqlCommand command = new SqlCommand(commandString))
//                {
//                    object spResult = command.ExecuteScalar(SqlDataConnection.DBConnection.JensenGroup);
//                    if (spResult != null)
//                    {
//                        if (spResult.ToString() != string.Empty)
//                        {
//                            nextID = (int)spResult;
//                            nextID++;
//                        }
//                    }
//                }
//            }

//            catch (SqlException)
//            {
//                throw;
//            }
//            return (short)nextID;
//        }

        #endregion

        #region Insert Data
        public int InsertNewPatternSub(DischargerCall_PatternSub patternSub)
        {
            const string commandString =
                @"INSERT INTO [dbo].tblDischargerCall_Patterns_Sub
                  ( PatternID
                  , StepNum
                  , CategID
                  , Quanttiy
                  , Mode
                  )
                  VALUES
                  ( @PatternID
                  , @StepNum
                  , @CategID
                  , @Quantity
                  , @Mode
                  )

                  SELECT @@ROWCOUNT";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", patternSub.PatternID);
                    command.Parameters.AddWithValue("@StepNum", patternSub.StepNum);
                    command.Parameters.AddWithValue("@CategID", patternSub.CategID);
                    command.Parameters.AddWithValue("@Quantity", patternSub.Quantity);
                    command.Parameters.AddWithValue("@Mode", patternSub.Mode);

                    try
                    {
                        command.ExecuteScalar(SqlDataConnection.DBConnection.Rail);
                        return 0;
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
        public int UpdateDischargerCall_PatternSub(DischargerCall_PatternSub patternSub)
        {
            const string commandString =
                @"UPDATE [dbo].tblDischargerCall_Patterns_Sub
                  SET CategID = @CategID
                    , Quantity = @Quantity
                    , Mode = @Mode

                  WHERE PatternID = @PatternID
                    AND StepNum = @StepNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", patternSub.PatternID);
                    command.Parameters.AddWithValue("@StepNum", patternSub.StepNum);
                    command.Parameters.AddWithValue("@CategID", patternSub.CategID);
                    command.Parameters.AddWithValue("@Quantity", patternSub.Quantity);
                    command.Parameters.AddWithValue("@Mode", patternSub.Mode);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                    patternSub.HasChanged = false;
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
        public int DeleteDischargerCall_PatternSub(DischargerCall_PatternSub patternSub)
        {
            const string commandString =
                @"DELETE FROM [dbo].tblDsichargerCall_Patterns_Sub
                  WHERE PatternID = @PatternID
                    AND StepNum = @StepNum";

            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@PatternID", patternSub.PatternID);
                    command.Parameters.AddWithValue("@StepNum", patternSub.StepNum);
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
    public class DischargerCall_PatternsSubs : List<DischargerCall_PatternSub>, IDataFiller
    {

        #region Field Positions
        int PatternIDPos;
        int StepNumPos;
        int CategIDPos;
        int QuantityPos;
        int ModePos;

        public DischargerCall_PatternSub GetById(int Pattern, int Step)
        {
            return this.Find(delegate(DischargerCall_PatternSub pattern)
            {
                return (pattern.PatternID == Pattern && pattern.StepNum == Step);
            });
        }

        public void SetFieldPositions(SqlDataReader dr)
        {
            PatternIDPos = dr.GetOrdinal("PatternID");
            StepNumPos = dr.GetOrdinal("StepNum");
            CategIDPos = dr.GetOrdinal("CategID");
            QuantityPos = dr.GetOrdinal("Quantity");
            ModePos = dr.GetOrdinal("Mode");
        }
        #endregion

        public int Fill(SqlDataReader dr)
        {
            SetFieldPositions(dr);

            this.Clear();
            while (dr.Read())
            {
                // Add to Collection
                DischargerCall_PatternSub pattern = FillPattern(dr);
                this.Add(pattern);
            }
            return this.Count;
        }

        public DischargerCall_PatternSub FillPattern(SqlDataReader dr)
        {
            DischargerCall_PatternSub pattern = new DischargerCall_PatternSub()
            {
                PatternID = dr.GetInt32(PatternIDPos),
                StepNum = dr.GetInt32(StepNumPos),
                CategID = dr.GetInt32(CategIDPos),
                Quantity = dr.GetInt32(QuantityPos),
                Mode = dr.GetInt32(ModePos)
            };

            return pattern;
        }

    }
    #endregion

    #region Item Class

    public class DischargerCall_PatternSub : DataItem
    {
        #region Discharger Call PatternSub
        protected class DataRecord : ICopyableObject
        {
            internal int PatternID;
            internal int StepNum;
            internal int CategID;
            internal int Quantity;
            internal int Mode;

            public ICopyableObject ShallowCopy()
            {
                return (ICopyableObject)MemberwiseClone();
            }
        }
        #endregion

        #region Constructor
        public DischargerCall_PatternSub()
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

        public int StepNum
        {
            get
            {
                return this.activeData.StepNum;
            }
            set
            {
                this.activeData.StepNum = value;
                HasChanged = true;
            }
        }

        public int CategID
        {
            get
            {
                return this.activeData.CategID;
            }
            set
            {
                this.activeData.CategID = value;
                HasChanged = true;
            }
        }

        public int Quantity
        {
            get
            {
                return this.activeData.Quantity;
            }
            set
            {
                this.activeData.Quantity = value;
                HasChanged = true;
            }
        }

        public int Mode
        {
            get
            {
                return this.activeData.Mode;
            }
            set
            {
                this.activeData.Mode = value;
                HasChanged = true;
            }
        }

        #endregion
    }

    #endregion
}
