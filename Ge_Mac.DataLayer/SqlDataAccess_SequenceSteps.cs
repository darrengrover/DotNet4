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
        #region Sequence Steps
        private SequenceSteps sequenceStepsCache = null;

        public void InvalidateSequenceSteps()
        {
            if (sequenceStepsCache != null)
                sequenceStepsCache.IsValid = false;
        }

        private bool SequenceStepsAreCached()
        {
            bool test = (sequenceStepsCache != null);
            if (test)
            {
                test = sequenceStepsCache.IsValid;
            }
            return test;
        }

        const string allSequenceStepsCommandString =
            @"SELECT idSequenceSteps
                   , SequenceRef
                   , StepID
                   , StepDescription_GB
                   , idResource
                   , HazardStatus
              FROM [dbo].[tblSequenceSteps]
                ORDER BY SequenceRef, StepID";

        public SequenceSteps GetAllSequenceSteps()
        {
            if (SequenceStepsAreCached())
            {
                return sequenceStepsCache;
            }
            try
            {
                const string commandString = allSequenceStepsCommandString;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (sequenceStepsCache == null) sequenceStepsCache = new SequenceSteps();
                    command.DataFill(sequenceStepsCache, SqlDataConnection.DBConnection.Rail);
                    return sequenceStepsCache;
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

        public SequenceSteps GetSequenceSteps(string sequenceRef)
        {
            SequenceSteps sequenceSteps = GetAllSequenceSteps();
            SequenceSteps seqSteps = new SequenceSteps();
            foreach (SequenceStep ss in sequenceSteps)
            {
                if (ss.SequenceRef == sequenceRef)
                {
                    seqSteps.Add(ss);
                }
            }
            return seqSteps;
        }

        /// <summary>
        /// Get the Sequence Steps after applying constraints
        /// </summary>
        /// <param name="systemId">Sequence System</param>
        /// <param name="sequenceID">Sequence</param>
        /// <returns>The constrained steps</returns>
        public SequenceSteps GetSequenceSteps(int systemId, int sequenceID)
        {
            try
            {
                const string commandString = "[dbo].[spGetSequenceSteps]";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SystemId", systemId);
                    command.Parameters.AddWithValue("@SequenceID", sequenceID);

                    SequenceSteps sequenceSteps = new SequenceSteps();
                    command.DataFill(sequenceSteps, SqlDataConnection.DBConnection.Rail);
                    InvalidateSequenceSteps();
                    return sequenceSteps;

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

        public SequenceStep GetSequenceStep(string sequenceRef, int stepID)
        {
            SequenceSteps sequenceSteps = GetAllSequenceSteps();
            SequenceStep sequenceStep = sequenceSteps.GetById(sequenceRef, stepID);
            return sequenceStep;
        }

        #endregion
    }

    public class SequenceSteps : List<SequenceStep>, IDataFiller
    {
        private double lifespan = 1.0;
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
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    DateTime testTime = lastRead.AddHours(lifespan);
                    test = testTime > da.ServerTime;
                }
                return test;
            }
            set
            { isValid = value; }
        }

        public int Fill(SqlDataReader dr)
        {
            int idSequenceStepsPos = dr.GetOrdinal("idSequenceSteps");
            int SequenceRefPos = dr.GetOrdinal("SequenceRef");
            int StepIDPos = dr.GetOrdinal("StepID");
            int StepDescription_GBPos = dr.GetOrdinal("StepDescription_GB");
            int idResourcePos = dr.GetOrdinal("idResource");
            int HazardStatusPos = dr.GetOrdinal("HazardStatus");

            this.Clear();
            while (dr.Read())
            {
                SequenceStep sequenceStep = new SequenceStep()
                {
                    idSequenceSteps = dr.GetInt32(idSequenceStepsPos),
                    SequenceRef = dr.GetString(SequenceRefPos),
                    StepID = dr.GetInt32(StepIDPos),
                    StepDescription_GB = dr.GetString(StepDescription_GBPos),
                    idResource = dr.GetString(idResourcePos),
                    HazardStatus = dr.GetInt32(HazardStatusPos)
                };

                // Add to sort categories collection
                this.Add(sequenceStep);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public new void Sort()
        {
            SequenceStepComparer comparer = new SequenceStepComparer();
            base.Sort(comparer);
        }

        public SequenceStep GetById(string sequenceRef, int stepID)
        {
            return this.Find(delegate(SequenceStep sequenceStep)
            {
                return ((sequenceStep.SequenceRef == sequenceRef)
                    && (sequenceStep.StepID == stepID));
            });
        }
    }

    public class SequenceStepComparer : IComparer<SequenceStep>
    {
        public int Compare(SequenceStep x, SequenceStep y)
        {
            int refCompare = (x.SequenceRef ?? string.Empty).CompareTo(y.SequenceRef ?? string.Empty);
            if (refCompare != 0)
            {
                return refCompare;
            }

            int valueX = (x != null) ? x.StepID + 1 : 0;
            int valueY = (y != null) ? y.StepID + 1 : 0;
            return Math.Sign(valueX - valueY);
        }
    }

    #region Sequence Step Class
    public class SequenceStep
    {
        public int idSequenceSteps { get; set; }
        public string SequenceRef { get; set; }
        public int StepID { get; set; }
        public string StepDescription_GB { get; set; }
        public string idResource { get; set; }
        public int HazardStatus { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", StepID, SequenceRef);
        }
    }
    #endregion
}
