using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region caches
        private ProductionSet refProductionSetCache = null;

        public void InvalidateRefProductionSet()
        {
            if (refProductionSetCache != null)
                refProductionSetCache.IsValid = false;
        }

        private bool RefProductionSetIsCached()
        {
            bool test = (refProductionSetCache != null);
            if (test)
            {
                test = refProductionSetCache.IsValid;
            }
            return test;
        }
        private ProductionSet productionSetCache = null;

        public void InvalidateProductionSet()
        {
            if (productionSetCache != null)
                productionSetCache.IsValid = false;
        }

        private bool ProductionSetIsCached()
        {
            bool test = (productionSetCache != null);
            if (test)
            {
                test = productionSetCache.IsValid;
            }
            return test;
        }
        #endregion

        #region Select Data
        public ProductionSet GetTodaysProduction(MachineArea machineArea)
        {
            //if (ProductionSetIsCached())
            //{
            //    return productionSetCache;
            //}
            const String strSQL = @"dbo.spGetTodayAreaProduction";
            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MachineArea", machineArea.ToString());

                    if (productionSetCache == null)
                    {
                        productionSetCache = new ProductionSet();
                        productionSetCache.Lifespan = 1.0 / 60.0;
                    }
                    command.DataFill(productionSetCache, SqlDataConnection.DBConnection.JensenGroup);

                    return productionSetCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                return null;
            }
        }

        public ProductionSet GetTodaysReferenceProduction(MachineArea machineArea)
        {
            const String strSQL = @"dbo.spGetReferenceAreaProduction";
            //if (RefProductionSetIsCached())
            //{
            //    return refProductionSetCache;
            //}
            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MachineArea", machineArea.ToString());

                    if (refProductionSetCache == null) refProductionSetCache = new ProductionSet();
                    command.DataFill(refProductionSetCache, SqlDataConnection.DBConnection.JensenGroup);

                    return refProductionSetCache;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    ExceptionHandler.Handle(ex);
                    Debugger.Break();
                }

                return null;
            }
        }
        #endregion
    }

    #region Data Collection Class

    public class ProductionSet : List<Production>, IDataFiller
    {
        private double lifespan = 24.0;
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
                    if (test)
                    {
                        test = (lastRead.Date == DateTime.Today);
                    }
                }
                return test;
            }
            set
            { isValid = value; }
        }
        public int Fill(SqlDataReader dr)
        {
            int LogDatePos = dr.GetOrdinal("LogTime");
            int MachineGroupIDPos = dr.GetOrdinal("MachineGroupID");
            int GroupDescriptionPos = dr.GetOrdinal("GroupDescription");
            int ValuePos = dr.GetOrdinal("Value");

            this.Clear();
            while (dr.Read())
            {
                Production _Prod = new Production();
                _Prod.LogTime = dr.GetDateTime(LogDatePos);
                _Prod.MachineGroupID = dr.GetInt32(MachineGroupIDPos);
                _Prod.GroupDescription = dr.GetString(GroupDescriptionPos);
                _Prod.Value = dr.GetInt32(ValuePos);

                // Add this to the collection
                this.Add(_Prod);
            }

            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;
            return this.Count;
        }

        /// <summary>
        /// Finds the first instance of Production in this collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Production GetByGroupId(int id)
        {
            return this.Find(delegate(Production prod)
            {
                return prod.MachineGroupID == id;
            });
        }

        public Production GetByIDTime(int id, DateTime aTime)
        {
            return this.Find(delegate(Production prod)
            {
                return (prod.MachineGroupID == id) && (prod.LogTime == aTime);
            });
        }

        public List<Production> FindAllByGroup(int GroupID)
        {
            FindProduction predicate = new FindProduction(new Production { MachineGroupID = GroupID });
            return FindAll(predicate.ByGroupID);
        }

        public class FindProduction
        {
            public Production production { get; set; }

            // Initializes with suffix we want to match.
            public FindProduction(Production p)
            {
                production = p;
            }

            // Gets the predicate.  Now it's possible to re-use this predicate with various suffixes.
            public Predicate<Production> ByGroupID
            {
                get
                {
                    return IsMatch;
                }
            }

            private bool IsMatch(Production p)
            {
                return p.MachineGroupID == production.MachineGroupID;
            }
        }
    }
    #endregion

    #region Item Class

    public class Production
    {
        public DateTime LogTime { get; set; }
        public int MachineGroupID { get; set; }
        public String GroupDescription { get; set; }
        public int Value { get; set; }
        public int[] Weeks { get; set; }

        public Production()
        {
            Weeks = new int[7];
        }

        public void AverageMid5()
        {
            Array.Sort(Weeks);
            int x=0;
            for (int i = 1; i < 6; i++)
            {
                x += Weeks[i];
            }
            Value = x / 5;
        }
    }

    #endregion
}
