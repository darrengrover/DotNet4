using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ErrorLogging
{

    public class ErrorLogger
    {
        public string ErrorLogFileName;
        private ErrorRecords errorRecords;
        public bool ReverseInsertMode = true;
        public int maxLines = 1000;
        public bool AutoLog = true;

        public ErrorLogger()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string appName = Path.ChangeExtension(Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),"log");
            ErrorLogFileName = Path.Combine(folder, appName); 
            errorRecords=new ErrorRecords();
        }

        public void AddError(string message, string identifier, int severity)
        {
            ErrorRecord rec = new ErrorRecord(message, identifier, severity);
            errorRecords.Add(rec);
            if (AutoLog)
                LogErrorsToFile();
        }

        public List<string> ReadLogFile()
        {
            List<string> lines = new List<string>();
            if (File.Exists(ErrorLogFileName))
            {
                using (StreamReader r = new StreamReader(ErrorLogFileName))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        public void StoreLogFile(List<string> lines)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ErrorLogFileName))
            {
                for (int i = 0; (i < lines.Count) && (i < maxLines); i++)
                {
                    file.WriteLine(lines[i]);
                }
            }
        }

        public void LogErrorsToFile()
        {
            List<string> lines = ReadLogFile();
            foreach (ErrorRecord rec in errorRecords)
            {
                if (!rec.Stored)
                {
                    string line = rec.ErrorTime.ToString(@"yyyy/MM/dd HH:mm:ss.fff") +
                        ", " + rec.Severity.ToString() + ", " + rec.Identifier + ", " + rec.Message;
                    if (ReverseInsertMode)
                        lines.Insert(0, line);
                    else
                        lines.Add(line);
                    rec.Stored = true;
                }
            }
            StoreLogFile(lines);
        }

    }

    public class ErrorRecords : List<ErrorRecord>
    { }

    public class ErrorRecord
    {
        public DateTime ErrorTime { get; set; }
        public string Message { get; set; }
        public string Identifier { get; set; }
        public int Severity { get; set; }
        public bool Stored { get; set; }

        public ErrorRecord(DateTime errorTime, string message, string identifier, int severity)
        {
            ErrorTime = errorTime;
            Message = message;
            Identifier = identifier;
            Severity = severity;
            Stored = false;
        }

        public ErrorRecord(string message, string identifier, int severity)
        : this(DateTime.Now, message, identifier, severity)
        {}

    }
}
