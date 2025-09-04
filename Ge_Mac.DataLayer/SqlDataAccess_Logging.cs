using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;
using Ge_Mac.LoggingDataLayer;
using Ge_Mac.Logging;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data

        const string allEventLogsCommand =
            @"SELECT [RecNum]
                   , [SystemID]
                   , [AlarmID]
                   , [EventType]
                   , [EventAction]
                   , [EventItem]
                   , [EventID]
                   , [UserID]
                   , [Description_GB]
                   , [Value_preChange]
                   , [Value_postChange]
                   , [EventTime]
              FROM [dbo].[tblEventLog]";

        public RailEvents GetAllEventLogs()
        {
            try
            {
                const string commandString = allEventLogsCommand;

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    RailEvents users = new RailEvents();
                    command.DataFill(users, SqlDataConnection.DBConnection.Rail);
                    return users;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                // Log the error
                Trace.WriteLine(string.Format("{0} at {1} in {2}\r\nStackTrace:{3}",
                    ex.Message, ex.TargetSite, ex.Source, ex.StackTrace), "Error");

                throw;
            }
        }

        public RailEvent GetEventLog(int id)
        {
            try
            {
                const string commandString = allEventLogsCommand
                    + " WHERE RecNum = @RecNum";

                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecNum", id);
                    RailEvents railEvents = new RailEvents();

                    command.DataFill(railEvents, SqlDataConnection.DBConnection.Rail);
                    if (railEvents.Count > 0)
                    {
                        return railEvents[0];
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

                // Log the error
                Trace.WriteLine(string.Format("{0} at {1} in {2}\r\nStackTrace:{3}",
                    ex.Message, ex.TargetSite, ex.Source, ex.StackTrace), "Error");

                throw;
            }
        }

        /// <summary>
        /// Gets the next event ID in order to save this event
        /// </summary>
        /// <returns>Returns the ID if OK (-1 if error)</returns>
        public int GetNextEventID()
        {
            const string commandString = "dbo.spGetPointer";

            try
            {
                // Requests the next ID from the database to save with the collection of events in the class.
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add the parameters
                    command.Parameters.AddWithValue("@PointerID", "Events");
                    SqlParameter pointerValue = command.Parameters.Add("@PointerValue", SqlDbType.Int);
                    pointerValue.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail);

                    return (int)pointerValue.Value;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                // Log the error
                Trace.WriteLine(string.Format("{0} at {1} in {2}\r\nStackTrace:{3}",
                    ex.Message, ex.TargetSite, ex.Source, ex.StackTrace), "Error");

                throw;
            }
        }
        #endregion

        #region Insert Data
        /// <summary>
        /// Saves the event to the database using the EventID supplied
        /// </summary>
        /// <param name="Event">Instance of event</param>
        /// <param name="EventID">ID of the event to be saved under</param>
        /// <returns>true if the entry is written successfully</returns>
        public bool SaveEvent(RailEvent Event, int EventID)
        {
            const string commandString = "dbo.spLogEvent";

            try
            {
                // Save the event to the database using the event ID specified
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add the parameters
                    command.Parameters.AddWithValue("@SystemID", Event.SystemID);
                    command.Parameters.AddWithValue("@AlarmID", Event.AlarmID);
                    command.Parameters.AddWithValue("@EventType", (int)Event.EventType);
                    command.Parameters.AddWithValue("@EventAction", (int)Event.EventAction);
                    command.Parameters.AddWithValue("@EventItem", Event.EventItem);
                    command.Parameters.AddWithValue("@EventID", EventID);
                    command.Parameters.AddWithValue("@UserID", Event.UserID);
                    command.Parameters.AddWithValue("@Description_GB", Event.DescriptionGB);
                    command.Parameters.AddWithValue("@Value_Pre", Event.Value_PreChange);
                    command.Parameters.AddWithValue("@Value_Post", Event.Value_PostChange);

                    return (command.ExecuteNonQuery(SqlDataConnection.DBConnection.Rail) == 1);
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                // Log the error
                Trace.WriteLine(string.Format("{0} at {1} in {2}\r\nStackTrace:{3}",
                    ex.Message, ex.TargetSite, ex.Source, ex.StackTrace), "Error");

                throw;
            }
        }
        #endregion
    }

    #region Data Collection Class
    public class RailEvents : List<RailEvent>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int AlarmIDPos = dr.GetOrdinal("AlarmID");
            int EventTypePos = dr.GetOrdinal("EventType");
            int EventActionPos = dr.GetOrdinal("EventAction");
            int EventItemPos = dr.GetOrdinal("EventItem");
            int EventIDPos = dr.GetOrdinal("EventID");
            int UserIDPos = dr.GetOrdinal("UserID");
            int Description_GBPos = dr.GetOrdinal("Description_GB");
            int Value_preChangePos = dr.GetOrdinal("Value_preChange");
            int Value_postChangePos = dr.GetOrdinal("Value_postChange");
            int EventTimePos = dr.GetOrdinal("EventTime");

            while (dr.Read())
            {
                RailEvent railEvent = new RailEvent()
                {
                    RecNum = dr.GetInt32(RecNumPos),
                    SystemID = dr.GetInt32(SystemIDPos),
                    AlarmID = dr.IsDBNull(AlarmIDPos) ? null : (int?)dr.GetInt32(AlarmIDPos),
                    EventType = (RailEventType)dr.GetInt32(EventTypePos),
                    EventAction = (RailEventAction)dr.GetInt32(EventActionPos),
                    EventItem = dr.IsDBNull(EventItemPos) ? null : dr.GetString(EventItemPos),
                    EventID = dr.GetInt32(EventIDPos),
                    UserID = dr.GetInt32(UserIDPos),
                    DescriptionGB = dr.GetString(Description_GBPos),
                    Value_PreChange = dr.IsDBNull(Value_preChangePos) ? null : dr.GetString(Value_preChangePos),
                    Value_PostChange = dr.IsDBNull(Value_postChangePos) ? null : dr.GetString(Value_postChangePos)
                };

                // Add to railEvent collection
                this.Add(railEvent);
            }

            return this.Count;
        }

        public RailEvent GetById(int id)
        {
            return this.Find(delegate(RailEvent railEvent)
            {
                return railEvent.RecNum == id;
            });
        }
    }
    #endregion
}
