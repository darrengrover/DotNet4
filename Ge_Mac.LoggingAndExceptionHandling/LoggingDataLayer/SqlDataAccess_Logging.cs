using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;
using Ge_Mac.Logging;

namespace Ge_Mac.LoggingDataLayer
{
    public partial class SqlDataAccess
    {
        #region Select Data
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

    #region RailEvent Class
    /// <summary>
    /// A particular event to be saved
    /// </summary>
    public class RailEvent
    {
        #region Public Properties

        public int RecNum                  { get; set; }
        public int SystemID                { get; set; }
        public int? AlarmID                { get; set; }
        public RailEventType EventType     { get; set; }
        public RailEventAction EventAction { get; set; }
        public String EventItem            { get; set; }
        public int EventID                 { get; set; }
        public int UserID                  { get; set; }
        public String DescriptionGB        { get; set; }
        public String Value_PreChange      { get; set; }
        public String Value_PostChange     { get; set; }
        public DateTime EventTime          { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new empty event
        /// </summary>
        public RailEvent()
        {
            this.EventType = RailEventType.Alarm;
            this.EventAction = RailEventAction.System;
        }

        /// <summary>
        /// Logs an alarm in the event log
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="AlarmID">The alarm in that has come on/off</param>
        /// <param name="EventAction">The event action on or off</param>
        public RailEvent(int systemID, int alarmID, RailEventAction eventAction)
        {
            this.SystemID = systemID;
            this.EventType = RailEventType.Alarm;
            this.EventAction = eventAction;
            this.AlarmID = alarmID;
            this.UserID = 0;
        }

        /// <summary>
        /// Creates a populated event object
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="EventType">Type of event that as taken place</param>
        /// <param name="EventAction">Action within the event</param>
        public RailEvent(int systemID, RailEventType eventType, RailEventAction eventAction)
        {
            this.SystemID = systemID;
            this.EventType = eventType;
            this.EventAction = eventAction;
        }

        /// <summary>
        /// Creates a populated event object
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="EventType">Type of event that as taken place</param>
        /// <param name="EventAction">Action within the event</param>
        /// <param name="EventItem">The item on which the event took place</param>
        public RailEvent(int systemID, RailEventType eventType, RailEventAction eventAction,
            String eventItem)
            : this(systemID, eventType, eventAction)
        {
            this.EventItem = eventItem;
        }

        /// <summary>
        /// Creates a populated event object
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="EventType">Type of event that as taken place</param>
        /// <param name="EventAction">Action within the event</param>
        /// /// <param name="EventItem">The item on which the event took place</param>
        /// <param name="UserID">User Id of the the person doing the event</param>
        public RailEvent(int systemID, RailEventType eventType, RailEventAction eventAction,
            String eventItem, int userID)
            : this(systemID, eventType, eventAction)
        {
            this.EventItem = eventItem;
            this.UserID = userID;
        }

        /// <summary>
        /// Creates a populated event object
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="EventType">Type of event that as taken place</param>
        /// <param name="EventAction">Action within the event</param>
        /// /// <param name="EventItem">The item on which the event took place</param>
        /// <param name="UserID">User Id of the the person doing the event</param>
        /// <param name="DescriptionGB">GB description of what took place</param>
        public RailEvent(int systemID, RailEventType eventType, RailEventAction eventAction,
            String eventItem, int userID, String descriptionGB)
            : this(systemID, eventType, eventAction)
        {
            this.EventItem = eventItem;
            this.UserID = userID;
            this.DescriptionGB = descriptionGB;
        }

        /// <summary>
        /// Creates a populated event object
        /// </summary>
        /// <param name="SystemID">System ID in which the event took place</param>
        /// <param name="EventType">Type of event that as taken place</param>
        /// <param name="EventAction">Action within the event</param>
        /// /// <param name="EventItem">The item on which the event took place</param>
        /// <param name="UserID">User Id of the the person doing the event</param>
        /// <param name="DescriptionGB">GB description of what took place</param>
        /// <param name="ValuePreChange">Values of the change prior to the event</param>
        /// <param name="ValuePostChange">values of the change after the change</param>
        public RailEvent(int systemID, RailEventType eventType, RailEventAction eventAction,
            String eventItem, int userID, string descriptionGB, string valuePreChange, string valuePostChange)
            : this(systemID, eventType, eventAction)
        {
            this.EventItem = eventItem;
            this.UserID = userID;
            this.DescriptionGB = descriptionGB;
            this.Value_PreChange = valuePreChange;
            this.Value_PostChange = valuePostChange;
        }

        #endregion
    }
    #endregion
}
