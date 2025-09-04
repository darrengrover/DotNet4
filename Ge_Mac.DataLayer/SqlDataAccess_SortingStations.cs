using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Sorting Stations
        const string allSortingStationsCommandString =
            @"SELECT [StnID]
                   , [SystemID]
                   , [CategID]
                   , [CustID]
                   , [StationGroup]
              FROM [dbo].[tblSortingStations]";

        public SortingStations GetAllSortingStations()
        {
            try
            {
                const string commandString = allSortingStationsCommandString;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    SortingStations sortingStations = new SortingStations();
                    command.DataFill(sortingStations, SqlDataConnection.DBConnection.Rail);
                    return sortingStations;
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

        public SortingStations GetAllSortingStations(int systemId)
        {
            try
            {
                const string commandString = allSortingStationsCommandString +
                    @" WHERE SystemID = @SystemId";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);

                    SortingStations sortingStations = new SortingStations();
                    command.DataFill(sortingStations, SqlDataConnection.DBConnection.Rail);
                    return sortingStations;
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

        public SortingStation GetSortingStation(int systemId, int stationId)
        {
            try
            {
                const string commandString = allSortingStationsCommandString +
                    @" WHERE SystemID = @SystemId
                       AND   StnID = @StnID";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@SystemId", systemId);
                    command.Parameters.AddWithValue("@StnId", stationId);

                    SortingStations sortingStations = new SortingStations();
                    command.DataFill(sortingStations, SqlDataConnection.DBConnection.Rail);
                    if (sortingStations.Count > 0)
                    {
                        return sortingStations[0];
                    }
                    else
                    {
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

        #endregion
    }

    public class SortingStations : List<SortingStation>, IDataFiller
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
            int StnIDPos = dr.GetOrdinal("StnID");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int CategIDPos = dr.GetOrdinal("CategID");
            int CustIDPos = dr.GetOrdinal("CustID");
            int StationGroupPos = dr.GetOrdinal("StationGroup");

            this.Clear();
            while (dr.Read())
            {
                SortingStation sequence = new SortingStation()
                {
                    StnID = dr.GetInt32(StnIDPos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    CategID = dr.GetInt16(CategIDPos),
                    CustID = dr.GetInt32(CustIDPos),
                    StationGroup = dr.GetInt32(StationGroupPos)
                };

                // Add to sort categories collection
                this.Add(sequence);
            }

            return this.Count;
        }

        public new void Sort()
        {
            SortingStationComparer comparer = new SortingStationComparer();
            base.Sort(comparer);
        }
    }

    public class SortingStationComparer : IComparer<SortingStation>
    {
        public int Compare(SortingStation x, SortingStation y)
        {
            if (x == null)
            {
                if (y == null)
                { // If x is null and y is null, they're equal. 
                    return 0;
                }
                else
                { // If x is null and y is not null, y is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                if (y == null)
                { // ...and y is null, x is greater.
                    return 1;
                }
                else
                {
                    if (x.SystemID > y.SystemID)
                    {
                        return 1;
                    }
                    else if (x.SystemID < y.SystemID)
                    {
                        return -1;
                    }

                    if (x.SystemID > y.SystemID)
                    {
                        return 1;
                    }
                    else if (x.SystemID < y.SystemID)
                    {
                        return -1;
                    }

                    if (x.StnID > y.StnID)
                    {
                        return 1;
                    }
                    else if (x.StnID < y.StnID)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }

    #region SortingStations Class
    public class SortingStation
    {
        public int StnID        { get; set; }
        public int SystemID     { get; set; }
        public short CategID    { get; set; }
        public int CustID       { get; set; }
        public int StationGroup { get; set; }
    }
    #endregion
}
