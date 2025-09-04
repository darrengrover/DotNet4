using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorLog
{
    public class ErrorRecords : List<ErrorRecord>
    { }

    public class ErrorRecord
    {
        public DateTime ErrorTime { get; set; }
        public string Message { get; set; }
        public string Identifier { get; set; }
        public int Severity { get; set; }

        public ErrorRecord(DateTime errorTime, string message, string identifier, int severity)
        {
            ErrorTime = errorTime;
            Message = message;
            Identifier = identifier;
            Severity = severity;
        }

        public ErrorRecord(string message, string identifier, int severity)
        : this(DateTime.Now, message, identifier, severity)
        {}

    }
}
