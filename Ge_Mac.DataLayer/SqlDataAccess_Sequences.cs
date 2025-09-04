using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Sequences
        private Sequences sequencesCache = null;

        public void InvalidateSequences()
        {
            if (sequencesCache != null)
                sequencesCache.IsValid = false;
        }

        private bool SequencesAreCached()
        {
            bool test = (sequencesCache != null);
            if (test)
            {
                test = sequencesCache.IsValid;
            }
            return test;
        }

        const string allSequencesCommandString =
            @"SELECT RecNum
                   , SystemID
                   , SequenceID
                   , SequenceRef
                   , CurrentValue
                   , CycleStop
                   , RequestedValue
                   , RequestedCycleStop
                   , UpdateTime
                   , 'seqSys_' + CAST(SequenceID AS VARCHAR) AS idResource
              FROM [dbo].[tblSequences]";

        const string allActiveSequencesCommandString =
            @"SELECT RecNum
                   , SystemID
                   , SequenceID
                   , SequenceRef
                   , CurrentValue
                   , CycleStop
                   , RequestedValue
                   , RequestedCycleStop
                   , UpdateTime
                   , 'seqSys_' + CAST(SequenceID AS VARCHAR) AS idResource
              FROM [dbo].[tblSequences]
                WHERE SequenceRef is not null";

        public Sequences GetAllActiveSequences()
        {
            if (SequencesAreCached())
            {
                return sequencesCache;
            }
            try
            {
                const string commandString = allActiveSequencesCommandString;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (sequencesCache == null) sequencesCache = new Sequences();
                    command.DataFill(sequencesCache, SqlDataConnection.DBConnection.Rail);
                    return sequencesCache;
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

        public Sequences GetAllSequences()
        {
            try
            {
                const string commandString = allSequencesCommandString;
                Sequences sequences;
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    sequences = new Sequences();
                    command.DataFill(sequences, SqlDataConnection.DBConnection.Rail);
                    return sequences;
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

        public Sequences GetAllActiveSequences(int systemId)
        {
            Sequences sequences = GetAllActiveSequences();
            Sequences seqs = new Sequences();
            foreach (Sequence s in sequences)
            {
                if ((s.SystemID == systemId)
                    && (s.SequenceRef != string.Empty))
                {
                    seqs.Add(s);
                }
            }
            return seqs;
        }

        public Sequence GetSequence(int systemId, int sequenceId)
        {
            Sequences sequences = GetAllActiveSequences();
            Sequence sequence = sequences.GetById(systemId, sequenceId);
            return sequence;
        }

        public void UpdateSequenceStep(Sequence sequence)
        {
            const string commandString =
                @"UPDATE [dbo].[tblSequences]
                  SET RequestedValue = @RequestedValue
                    , UpdateTime = GETDATE()
                  WHERE [SystemId] = @SystemID
                  AND   [SequenceID] = @SequenceID;";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@RequestedValue", sequence.RequestedValue);
                command.Parameters.AddWithValue("@SystemID", sequence.SystemID);
                command.Parameters.AddWithValue("@SequenceID", sequence.SequenceID);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateSequences();
            }
        }

        /// <summary>
        /// Update the sequence cycle stop according to the value in the passed sequence.
        /// </summary>
        /// <param name="sequence"></param>
        public void UpdateSequenceCycle(Sequence sequence)
        {
            const string commandString =
                @"UPDATE [dbo].[tblSequences]
                  SET RequestedCycleStop = @RequestedCycleStop
                    , UpdateTime = GETDATE()
                  WHERE [SystemId] = @SystemID
                  AND   [SequenceID] = @SequenceID;";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@RequestedCycleStop", sequence.RequestedCycleStop);
                command.Parameters.AddWithValue("@SystemID", sequence.SystemID);
                command.Parameters.AddWithValue("@SequenceID", sequence.SequenceID);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateSequences();
            }
        }

        #endregion
    }

    public class Sequences : List<Sequence>, IDataFiller
    {
        private double lifespan = 1.0 / 3600.0;
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
            int RecNumPos = dr.GetOrdinal("RecNum");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int SequenceIDPos = dr.GetOrdinal("SequenceID");
            int SequenceRefPos = dr.GetOrdinal("SequenceRef");
            int CurrentValuePos = dr.GetOrdinal("CurrentValue");
            int CycleStopPos = dr.GetOrdinal("CycleStop");
            int RequestedValuePos = dr.GetOrdinal("RequestedValue");
            int RequestedCycleStopPos = dr.GetOrdinal("RequestedCycleStop");
            int UpdateTimePos = dr.GetOrdinal("UpdateTime");
            int idResourcePos = dr.GetOrdinal("idResource");

            SqlDataAccess da=SqlDataAccess.Singleton;
            double dbVersion =da.DatabaseVersion;

            this.Clear();
            while (dr.Read())
            {
                Sequence sequence = new Sequence();
                
                sequence.RecNum = dr.GetInt32(RecNumPos);
                sequence.SystemID = dr.GetInt32(SystemIDPos);
                sequence.SequenceID = dr.GetInt32(SequenceIDPos);
                sequence.SequenceRef = !dr.IsDBNull(SequenceRefPos) ? dr.GetString(SequenceRefPos) : string.Empty;
                sequence.CurrentValue = dr.GetInt32(CurrentValuePos);
                //if (dbVersion >= 1.6)
                if (!da.CycleStopBit)
                    sequence.CycleStop = dr.GetInt32(CycleStopPos);
                else
                {
                    bool cStop = dr.GetBoolean(CycleStopPos);
                    sequence.CycleStop = 0;
                    if (cStop)
                        sequence.CycleStop = 1;
                }
                sequence.RequestedValue = dr.GetInt32(RequestedValuePos);
                sequence.RequestedCycleStop = dr.GetInt32(RequestedCycleStopPos);
                sequence.UpdateTime = !dr.IsDBNull(UpdateTimePos) ? (DateTime?)dr.GetDateTime(UpdateTimePos) : null;
                sequence.idResource = dr.GetString(idResourcePos);
               
                // Add to sort categories collection
                this.Add(sequence);
            }
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public new void Sort()
        {
            SequenceComparer comparer = new SequenceComparer();
            base.Sort(comparer);
        }

        public Sequence GetById(int systemID, int sequenceID)
        {
            return this.Find(delegate(Sequence sequence)
            {
                return ((sequence.SystemID == systemID)
                    && (sequence.SequenceID == sequenceID)
                    && (sequence.SequenceRef != string.Empty));
            });
        }

    }

    public class SequenceComparer : IComparer<Sequence>
    {
        public int Compare(Sequence x, Sequence y)
        {
            int valueX = (x != null) ? x.SequenceID + 1 : 0;
            int valueY = (y != null) ? y.SequenceID + 1 : 0;
            return Math.Sign(valueX - valueY);
        }
    }

    #region Sequence Class
    public class Sequence
    {
        public int RecNum             { get; set; }
        public int SystemID           { get; set; }
        public int SequenceID         { get; set; }
        public string SequenceRef     { get; set; }
        public int CurrentValue       { get; set; }
        public int CycleStop         { get; set; }
        public int RequestedValue     { get; set; }
        public int RequestedCycleStop { get; set; }
        public DateTime? UpdateTime   { get; set; }
        public string idResource      { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", SequenceID, SequenceRef);
        }
    }
    #endregion
}
