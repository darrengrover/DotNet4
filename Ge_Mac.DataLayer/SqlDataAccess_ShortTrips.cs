using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region ShortTrips
        private ShortTrips shortTripsCache = null;

        public void InvalidateShortTrips()
        {
            if (shortTripsCache != null)
                shortTripsCache.IsValid = false;
        }

        private bool ShortTripsAreCached()
        {
            bool test = (shortTripsCache != null);
            if (test)
            {
                test = shortTripsCache.IsValid;
            }
            return test;
        }

        const string allShortTripsCommandString =
            @"SELECT SystemID
                   , Trip
                   , State
                   , RequestedValue
                   , UpdateTime
                   , 'tripSys_' + CAST(Trip AS VARCHAR) AS idResource
              FROM [dbo].[tblShortTrips]";

        public ShortTrips GetAllShortTrips()
        {
            if (ShortTripsAreCached())
            {
                return shortTripsCache;
            }
            try
            {
                const string commandString = allShortTripsCommandString;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    if (shortTripsCache == null) shortTripsCache = new ShortTrips();
                    command.DataFill(shortTripsCache, SqlDataConnection.DBConnection.Rail);
                    return shortTripsCache;
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

        public ShortTrips GetAllShortTrips(int systemId)
        {
            ShortTrips shortTrips = GetAllShortTrips();
            ShortTrips sTrips = new ShortTrips();
            foreach (ShortTrip st in shortTrips)
            {
                if (st.SystemID == systemId)
                {
                    sTrips.Add(st);
                }
            }
            return sTrips;
        }

        public ShortTrip GetShortTrip(int systemId, int tripId)
        {
            ShortTrips shortTrips = GetAllShortTrips();
            ShortTrip shortTrip = shortTrips.GetById(systemId, tripId);
            return shortTrip;
        }

        public void UpdateShortTripState(ShortTrip trip)
        {
            const string commandString =
                @"UPDATE [dbo].[tblShortTrips]
                  SET RequestedValue = @RequestedValue
                    , UpdateTime = GETDATE()
                  WHERE [SystemId] = @SystemID
                  AND   [Trip] = @Trip;";

            using (SqlCommand command = new SqlCommand(commandString))
            {
                command.Parameters.AddWithValue("@RequestedValue", trip.RequestedValue);
                command.Parameters.AddWithValue("@SystemID", trip.SystemID);
                command.Parameters.AddWithValue("@Trip", trip.Trip);
                command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);
                InvalidateShortTrips();
            }
        }
        #endregion
    }

    public class ShortTrips : List<ShortTrip>, IDataFiller
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
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int TripPos = dr.GetOrdinal("Trip");
            int StatePos = dr.GetOrdinal("State");
            int RequestedValuePos = dr.GetOrdinal("RequestedValue");
            int UpdateTimePos = dr.GetOrdinal("UpdateTime");
            int idResourcePos = dr.GetOrdinal("idResource");

            this.Clear();
            while (dr.Read())
            {
                ShortTrip sequence = new ShortTrip()
                {
                    SystemID = dr.GetInt32(SystemIDPos),
                    Trip = dr.GetInt32(TripPos),
                    State = dr.GetInt32(StatePos),
                    RequestedValue = dr.GetInt32(RequestedValuePos),
                    UpdateTime = !dr.IsDBNull(UpdateTimePos) ? (DateTime?)dr.GetDateTime(UpdateTimePos) : null,
                    idResource = dr.GetString(idResourcePos)
                };

                // Add to sort categories collection
                this.Add(sequence);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public new void Sort()
        {
            ShortTripComparer comparer = new ShortTripComparer();
            base.Sort(comparer);
        }

        public ShortTrip GetById(int systemID, int tripID)
        {
            return this.Find(delegate(ShortTrip shortTrip)
            {
                return ((shortTrip.SystemID == systemID)
                    && (shortTrip.Trip == tripID));
            });
        }
    }

    public class ShortTripComparer : IComparer<ShortTrip>
    {
        public int Compare(ShortTrip x, ShortTrip y)
        {
            int valueX = (x != null) ? x.Trip + 1 : 0;
            int valueY = (y != null) ? y.Trip + 1 : 0;
            return Math.Sign(valueX - valueY);
        }
    }

    #region Sequence Class
    public class ShortTrip
    {
        public int SystemID { get; set; }
        public int Trip { get; set; }
        public int State { get; set; }
        public int RequestedValue { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string idResource { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Trip);
        }
    }
    #endregion
}
