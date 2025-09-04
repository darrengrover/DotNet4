using System;
using System.Collections.Generic;
using Ge_Mac.LoggingDataLayer;

namespace Ge_Mac.Logging
{
    /// <summary>
    /// Handles the saving of an event to the event log
    /// </summary>
    public class LogEvent
    {
        #region Public Properties

        public List<RailEvent> Events { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of an event log entry
        /// </summary>
        public LogEvent()
        {
            Events = new List<RailEvent>();
        }

        #endregion

        #region Public Methods
        public RailEvent GetItem(String EventItem)
        {
            foreach (RailEvent ev in Events)
            {
                if (ev.EventItem == EventItem)
                {
                    return ev;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds an event to the event log collection
        /// </summary>
        /// <param name="ev">Instance of event to be added</param>
        public void Add(RailEvent ev)
        {
            Events.Add(ev);
        }

        /// <summary>
        /// Updates the event log with all of the events in the event collection
        /// </summary>
        /// <returns>Returns false if failed to update and true is update completed ok</returns>
        public bool Update()
        {
            // Get new ID for all the events
            SqlDataAccess da = new SqlDataAccess();
            int NewEventID;

            try
            {
                NewEventID = da.GetNextEventID();
                if (NewEventID < 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
           
            try
            {
                // Loop around each of the events and save them using the event ID supplied
                foreach (RailEvent ev in Events)
                {
                    da.SaveEvent(ev, NewEventID);
                }

                // Completed ok
                return true;
            }
            catch // Save Error detected
            {
                return false;
            }
        }
        #endregion
    }
}
