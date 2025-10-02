using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Dynamic.DataLayer
{
    public class KbroRfIDReader : DataItem
    {
        public int ReaderID { get; set; }
        public string ReaderIP { get; set; }
        public int Port { get; set; }
        public int ReaderFunction { get; set; }

    }

    public class KbroRfIDReaders : DataList
    {
        public KbroRfIDReaders()
        {
            Lifespan = 1.0;
            ListType = typeof(KbroRfIDReader);
            TblName = "tblKbroCrateReaders";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("ReaderID", "ReaderID", true, false);
            dBFieldMappings.AddMapping("ReaderIP", "ReaderIP");
            dBFieldMappings.AddMapping("Port", "Port");
            dBFieldMappings.AddMapping("ReaderFunction", "ReaderFunction");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class KbroRfIDLogRecord : DataItem
    {
        public int RecordID { get; set; }
        public int ReaderID { get; set; }
        public int OperatorID { get; set; }
        public DateTime RecordTimestamp { get; set; }

    }

    public class KbroRfIDLogRecords : DataList
    {
        public KbroRfIDLogRecords()
        {
            Lifespan = 1.0;
            ListType = typeof(KbroRfIDLogRecord);
            TblName = "tblKbroCrateReaderLogs";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("RecordID", "RecordID", true, true);
            dBFieldMappings.AddMapping("ReaderID", "ReaderID");
            dBFieldMappings.AddMapping("OperatorID", "OperatorID");
            dBFieldMappings.AddMapping("RecordTimestamp", "RecordTimestamp");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);
        }
    }

    public class FavershamMoistureRecord : DataItem
    {
        public int SortID { get; set; }
        public string ShortDescription { get; set; }
        public int BatchPairs { get; set; }
        public double SoiledKg { get; set; }
        public double ExtractedKg { get; set; }
        public double ExtractionPercent { get; set; }
        public double DryKg { get; set; }
        public double DryPercent { get; set; }
        public double EvaporatedKg { get; set; }
        public double KWh { get; set; }
        public double KWhPerKg { get; set; }

    }

    public class FavershamMoistureRecords : DataList
    {
        private DateTime start = DateTime.Today;

        public DateTime Start
        {
            get { return start; }
            set { start = value; }
        }
        private DateTime end = DateTime.Today.AddDays(1);

        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }

        public FavershamMoistureRecords()
        {
            Lifespan = 1.0;
            ListType = typeof(FavershamMoistureRecord);
            TblName = "";
            DbName = "JEGR_DB";
            dBFieldMappings = new DBFieldMappings();
            dBFieldMappings.AddMapping("CategoryID", "SortID", true, false);
            dBFieldMappings.AddMapping("ShortDescription", "ShortDescription");
            dBFieldMappings.AddMapping("Batch Pairs", "BatchPairs");
            dBFieldMappings.AddMapping("Soiled Kg", "SoiledKg");
            dBFieldMappings.AddMapping("Extracted Kg", "ExtractedKg");
            dBFieldMappings.AddMapping("Extraction%", "ExtractionPercent");
            dBFieldMappings.AddMapping("Dry Kg", "DryKg");
            dBFieldMappings.AddMapping("Dry%", "DryPercent");
            dBFieldMappings.AddMapping("Evapourated Kg", "EvaporatedKg");
            dBFieldMappings.AddMapping("KWh", "KWh");
            dBFieldMappings.AddMapping("KWh/Kg", "KWhPerKg");
            PropertyInfo[] pInfos = ListType.GetProperties();
            dBFieldMappings.AddPropertyInfos(pInfos);

        }

        public override int DBSelect()
        {
            int count = 0;
            try
            {
                SelectException = string.Empty;
                SqlDataAccess da = SqlDataAccess.Singleton;
                da.DBName = DbName;
                SqlConnection dBConnection = da.SqlConnection;
                string commandString = @"EXECUTE [JEGR_DB].[dbo].[spGetMoistureRetentionData] 
                                           @Start
                                          ,@End";

                SelectResult = false;
 
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    
                    if (command.Connection == null || command.Connection.ConnectionString.Length == 0)
                    {
                        command.Connection = dBConnection;
                    }
                    command.CommandTimeout = TimeoutSeconds;
                    command.Parameters.AddWithValue("@Start", start);
                    command.Parameters.AddWithValue("@End", end);
                    // Open the connection if necessary
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        command.Connection.Open();
                        using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.KeyInfo))
                        {
                            count = Fill(dr);
                        }
                    }
                    else
                    {
                        using (SqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                        {
                            count = Fill(dr);
                        }
                    }
                }

                SelectResult = true;
            }
            catch (Exception ex)
            {
                SelectException += MethodInfo.GetCurrentMethod() + ex.Message + Environment.NewLine;
                Debug.WriteLine(MethodInfo.GetCurrentMethod() + ex.Message);
                if (RaiseException)
                { throw ex; }
            }
            return count;
        }


    }

}
