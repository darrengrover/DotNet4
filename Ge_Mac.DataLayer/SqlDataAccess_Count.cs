using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {

        public int InsertArticleCount(ArticleCount articleCount)
        {
            const string InsertArticleCount =
                @"INSERT INTO [dbo].tblArticleCounts
                    ( 
                      SystemID
                    , CountID
                    , CountDescription
                    , Station
                    , Category
                    , Customer
                    , EventTime
                    , PulsePeriod
                    )
                    VALUES
                    (
                      @SystemID
                    , @CountID
                    , @CountDescription
                    , @Station
                    , @Category
                    , @Customer
                    , @EventTime
                    , @PulsePeriod
                    )

                  SELECT @@ROWCOUNT";

            try
            {
                using (SqlCommand command = new SqlCommand(InsertArticleCount))
                {
                    command.Parameters.AddWithValue("@SystemID", articleCount.SystemID);
                    command.Parameters.AddWithValue("@CountID", articleCount.CountID);
                    command.Parameters.AddWithValue("@CountDescription", articleCount.CountDescription);
                    command.Parameters.AddWithValue("@Station", articleCount.CountStation);
                    command.Parameters.AddWithValue("@Category", articleCount.CategoryID);
                    command.Parameters.AddWithValue("@Customer", articleCount.CustomerID);
                    command.Parameters.AddWithValue("@EventTime", articleCount.EventTime);
                    command.Parameters.AddWithValue("@PulsePeriod", articleCount.PulsePeriod);

                    object RecordCount = command.ExecuteScalar(SqlDataConnection.DBConnection.Rail);
                    return (int)RecordCount;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                return 0;
                throw;
            }
        }

        public ArticleCounts GetAllCounts()
        {
            try
            {
                string strSQL = @"SELECT
                                      RecNum
                                    , SystemID
                                    , CountID
                                    , CountDescription
                                    , Station
                                    , Category
                                    , Customer
                                    , EventTime
                                    , PulsePeriod
                                    FROM dbo.tblArticleCounts";

                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    ArticleCounts counts = new ArticleCounts();
                    command.DataFill(counts, SqlDataConnection.DBConnection.Rail);
                    return counts;
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

        public CountDescriptions GetCountDescriptions()
        {
            try
            {
                string strSQL = @"SELECT CountID, CountDescription FROM tblArticleCounts
                                GROUP BY CountID, CountDescription";
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    CountDescriptions countDesc = new CountDescriptions();
                    command.DataFill(countDesc, SqlDataConnection.DBConnection.Rail);
                    return countDesc;
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

        public int GetNextCountID()
        {

            try
            {
                
                string strSQL = "SELECT MAX(CountID) as MaxCountID FROM tblArticleCounts";
                using (SqlCommand cm = new SqlCommand(strSQL))
                {
                    object ReturnVal = cm.ExecuteScalar(SqlDataConnection.DBConnection.Rail);

                    if (ReturnVal != null)
                    {
                        return (int)ReturnVal + 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch { return 1; }
        }
    }

    public class ArticleCounts : List<ArticleCount>, IDataFiller
    {

        public int Fill(SqlDataReader dr)
        {
            int RecNumPos = dr.GetOrdinal("RecNum");
            int SystemIDPos = dr.GetOrdinal("SystemID");
            int CountIDPos = dr.GetOrdinal("CountID");
            int CountDescPos = dr.GetOrdinal("CountDescription");
            int CountStationPos = dr.GetOrdinal("Station");
            int CategoryPos = dr.GetOrdinal("Category");
            int CustomerPos = dr.GetOrdinal("Customer");
            int EventTimePos = dr.GetOrdinal("EventTime");
            int PulsePeriodPos = dr.GetOrdinal("PulsePeriod");

            while (dr.Read())
            {
                ArticleCount articleCount = new ArticleCount();
                articleCount.RecNum = dr.GetInt32(RecNumPos);
                articleCount.SystemID = dr.GetInt32(SystemIDPos);
                articleCount.CountID = dr.GetInt32(CountIDPos);
                articleCount.CountDescription = dr.IsDBNull(CountDescPos) ? "": dr.GetString(CountDescPos);
                articleCount.CountStation = dr.GetInt32(CountStationPos);
                articleCount.CategoryID = dr.GetInt32(CategoryPos);
                articleCount.CustomerID = dr.GetInt32(CustomerPos);
                articleCount.EventTime = !dr.IsDBNull(EventTimePos) ? (DateTime?)dr.GetDateTime(EventTimePos) : null;
                articleCount.PulsePeriod = dr.GetInt32(PulsePeriodPos);

                // Add to count collection
                this.Add(articleCount);
            }

            return this.Count;
        }

    }

    public class CountDescriptions : List<CountDescription>, IDataFiller
    {
        public int Fill(SqlDataReader dr)
        {
            int CountIDPos = dr.GetOrdinal("CountID");
            int CountDescPos = dr.GetOrdinal("CountDescription");

            while (dr.Read())
            {
                CountDescription countDescription = new CountDescription();
                countDescription.CountID = dr.GetInt32(CountIDPos);
                countDescription.Description = dr.IsDBNull(CountDescPos) ? "" : dr.GetString(CountDescPos);

                // add the descriptions collection
                this.Add(countDescription);
            }

            return this.Count;
        }
    }

    #region Article Count Class
    public class ArticleCount
    {
        public int RecNum { get; set; }
        public int SystemID { get; set; }
        public int CountID { get; set; }
        public string CountDescription { get; set; }
        public int CountStation { get; set; }
        public int CategoryID { get; set; }
        public int CustomerID { get; set; }
        public DateTime? EventTime { get; set; }
        public int PulsePeriod { get; set; }

        public ArticleCount()
        {
        }

        public ArticleCount(int SystemID,int CountID, string CountDescription, int CountStation, int CategoryID, int CustomerID, DateTime EventTime, int PulsePeriod)
        {
            this.SystemID = SystemID;
            this.CountID = CountID;
            this.CountDescription = CountDescription;
            this.CountStation = CountStation;
            this.CategoryID = CategoryID;
            this.CustomerID = CustomerID;
            this.EventTime = EventTime;
            this.PulsePeriod = PulsePeriod;
        }
    }
    #endregion

    #region Count Description
    public class CountDescription
    {
        public int CountID { get; set; }
        public string Description { get; set; }

        public CountDescription()
        { }
    }
    #endregion
}
