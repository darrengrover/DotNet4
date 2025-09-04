using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using Ge_Mac.LoggingAndExceptions;

namespace Ge_Mac.DataLayer
{
    public partial class SqlDataAccess
    {
        #region Caches

        MachineKpis todaysMachineKPIs;
        MachineKpis histMachineKPIs;

        private bool TestHistoricMachineKPIs()
        {
            bool test = false;
            test = histMachineKPIs != null;
            if (test)
            {
                test = histMachineKPIs.Count > 0;
                if (test)
                {
                    test = histMachineKPIs[0].RecTimeStamp.DayOfWeek == this.ServerTime.DayOfWeek;
                }
            }
            return test;
        }

        private bool TestTodaysMachineKPIs()
        {
            bool test = (todaysMachineKPIs != null);
            if (test)
            {
                test = todaysMachineKPIs.IsValid;
            }
            return test;
        }

        #endregion
        #region Select Data
        public KPISet GetTodaysKPI(MachineArea machineArea)
        {
            const String strSQL =
                @"dbo.spGetGroupKPI";

            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@MachineArea", machineArea.ToString());
                    
                    KPISet kpi = new KPISet();
                    command.DataFill(kpi, SqlDataConnection.DBConnection.JensenGroup);

                    return kpi;
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
                throw;
            }
        }

        public KPISet GetTodaysKPIm(MachineArea machineArea)
        {
            string mArea = "WT";
            if (machineArea == MachineArea.FT)
            {
                mArea = "FT";
            }

            if (machineArea == MachineArea.GT)
            {
                mArea = "GT";
            }

            return GetGroupKPI(mArea);
        }

        public MachineKpis GetMachineSubidKPITotals(DateTime start, DateTime end)
        {
            return GetMachineSubidKPITotals(start, end, string.Empty);
        }

        public MachineKpis GetMachineSubidKPITotals(DateTime start, DateTime end, string machinelist)
        {
            string strSQL = @"SELECT MIN( mk.[RecNum]) as RecNum
                                          ,mk.[RecTimeStamp]
                                          ,mk.[MachineID]
                                          ,SUM(mk.[Value]) as Value
                                          ,MIN(mk.[Unit]) as Unit
                                          ,MIN(mk.[KpiID]) as KpiID
                                          ,mk.[CountExitPointData]  ";
            if (DatabaseVersion >= 1.95)
            {
                strSQL += @"              ,SUM(mk.[OperatorProduction]) as OperatorProduction
                                          ,SUM(mk.[OperatorHours] ) as OperatorHours ";
            }
            if (DatabaseVersion >= 1.98)
            {
                strSQL += @"              ,mk.SubID as SubID
                                          ,mk.OperatorID as OperatorID
                                          ,MIN(mk.[KpiType]) as KpiType
                                          ,MIN(mk.KpiSubType) as KpiSubType
                                          ,MIN(mk.KpiSubTypeID) as  KpiSubTypeID
                                          ,SUM(mk.ExpectedValue) as ExpectedValue ";
            }
            strSQL += @"
                                      FROM [JEGR_DB].[dbo].[tblMachineKPI] mk
                                    WHERE RecTimeStamp>=@start
                                        AND RecTimeStamp<@end
                                        AND mk.CountExitPointData = 1";
            if (DatabaseVersion >= 1.98) strSQL += @"
                                        AND mk.KpiType=1 
                                        ";
            if (machinelist != string.Empty)
                strSQL += @"         AND mk.machineid in (" + machinelist + ") ";
            if (DatabaseVersion >= 1.98)

                strSQL += @"
                                    GROUP BY MachineID, SubID, Operatorid, RecTimeStamp, CountExitPointData
                                   ORDER BY MachineID, subid, operatorid, RecTimeStamp";
            else
                strSQL += @"
                                    GROUP BY MachineID,RecTimeStamp, CountExitPointData
                                    ORDER BY MachineID, RecTimeStamp";
            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    //command.CommandType = System.Data.CommandType.TableDirect;
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);

                    MachineKpis kpi = new MachineKpis();
                    command.DataFill(kpi, SqlDataConnection.DBConnection.JensenGroup);

                    return kpi;
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
                throw;
            }
        }

        public MachineKpis GetMachineKPIDaysDashboard(DateTime start, DateTime end, int machineID)
        {
            string strSQL = @"SELECT MIN( mk.[RecNum]) as RecNum
                                          ,mk.[RecTimeStamp]
                                          ,mk.[MachineID]
                                          ,SUM(mk.[Value]) as Value
                                          ,MIN(mk.[Unit]) as Unit
                                          ,MIN(mk.[KpiID]) as KpiID
                                          ,mk.[CountExitPointData]  ";
            if (DatabaseVersion >= 1.95)
            {
                strSQL += @"              ,SUM(mk.[OperatorProduction]) as OperatorProduction
                                          ,SUM(mk.[OperatorHours] ) as OperatorHours ";
            }
            if (DatabaseVersion >= 1.98)
            {
                strSQL += @"              ,Min(mk.SubID) as SubID
                                          ,-1 as OperatorID
                                          ,MIN(mk.[KpiType]) as KpiType
                                          ,MIN(mk.KpiSubType) as KpiSubType
                                          ,MIN(mk.KpiSubTypeID) as  KpiSubTypeID
                                          ,SUM(mk.ExpectedValue) as ExpectedValue ";
            }
            strSQL += @"
                                      FROM [JEGR_DB].[dbo].[tblMachineKPI] mk
                                    WHERE RecTimeStamp>=@start
                                        AND RecTimeStamp<@end
                                        AND MachineID=@MachineID
                                        --AND mk.CountExitPointData = 1";
            if (DatabaseVersion >= 1.98) strSQL += @"
                                        AND mk.KpiType=1";
            strSQL += @"            
                                    GROUP BY MachineID, RecTimeStamp, CountExitPointData
                                    ORDER BY MachineID, RecTimeStamp";
            try
            {
                using (SqlCommand command = new SqlCommand(strSQL))
                {
                    //command.CommandType = System.Data.CommandType.TableDirect;
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    command.Parameters.AddWithValue("@MachineID", machineID);

                    MachineKpis kpi = new MachineKpis();
                    command.DataFill(kpi, SqlDataConnection.DBConnection.JensenGroup);

                    return kpi;
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
                throw;
            }
        }

        private MachineKpis processMachineTotals(MachineKpis subTotalKpis)
        {
            MachineKpis totKpis = new MachineKpis();
            foreach (MachineKpi mkpi in subTotalKpis)
            {
                MachineKpi totMkpi = totKpis.getByIDTime(mkpi.MachineID,mkpi.RecTimeStamp);
                if (totMkpi == null)
                {
                    totMkpi = new MachineKpi();
                    totMkpi.MachineID = mkpi.MachineID;
                    totMkpi.RecTimeStamp = mkpi.RecTimeStamp;
                    totMkpi.SubID = -1;
                    totMkpi.Value = mkpi.Value;
                    totMkpi.CurrentOpHours = (double)mkpi.OperatorHours;
                    totMkpi.ExpectedValue = mkpi.ExpectedValue;
                    totMkpi.CountExitPointData = mkpi.CountExitPointData;
                    totMkpi.KpiType = 1;
                    totMkpi.MachineGroupID = mkpi.MachineGroupID;
                    totKpis.Add(totMkpi);
                }
                else
                {
                    totMkpi.Value += mkpi.Value;
                    totMkpi.CurrentOpHours += (double)mkpi.OperatorHours;
                    totMkpi.OperatorProduction += mkpi.OperatorProduction;
                }
            }
            return totKpis;
        }



        public MachineKpis GetDaysMachineKPIs(DateTime start, DateTime end)
        {
            MachineKpis subidTotalKpis;
            subidTotalKpis = GetMachineSubidKPITotals(start, end);
            MachineKpis totKpis = processMachineTotals(subidTotalKpis);
            return totKpis;
        }

        public MachineKpis GetTodaysMachineKPIs()
        {
            DateTime kpiDay = this.ServerTime.Date;
            DateTime end = kpiDay.AddDays(1);
            kpiDay = kpiDay.AddMinutes(5);
            MachineKpis totKpis = GetDaysMachineKPIs(kpiDay, end);
            return totKpis;
        }

        public MachineKpis GetHistoricMachineKPIs()
        {
            MachineKpis kpis = new MachineKpis();
            DateTime kpiDay = this.ServerTime.Date;
            DateTime end = kpiDay.AddDays(1);
            MachineKpis hkpis;
            kpiDay = kpiDay.AddMinutes(5);
            for (int i = 0; i < 7; i++)
            {
                kpiDay = kpiDay.Subtract(TimeSpan.FromDays(7));
                end = end.Subtract(TimeSpan.FromDays(7));
                hkpis = GetDaysMachineKPIs(kpiDay, end);
                foreach (MachineKpi mkpi in hkpis)
                {
                    kpis.Add(mkpi);
                }
            }
            return kpis;
        }

        public void ReadMachineKPIs()
        {
            if (!TestHistoricMachineKPIs())
                histMachineKPIs = GetHistoricMachineKPIs();
            if (!TestTodaysMachineKPIs())
                todaysMachineKPIs = GetTodaysMachineKPIs();
        }

        public double GetKpiDynamic(int MachineGroupID, DateTime targetTime)
        {
            int nrWeeks = 1;
            int firstWeek = 1;
            int lastWeek = 5;
            int divWeeks = 5;
            double kpiDynamic = 0;
            List<double> dynamic = new List<double>();
            double opTime = 0;
            for (int i = 0; i < 7; i++)
            {
                double x = GetKpiDay(MachineGroupID, i, targetTime, out opTime);
                dynamic.Add(x);
            }
            dynamic.Sort();

            for (int i = 0; i < 7; i++)
            {
                if (dynamic[i] > 0)
                    nrWeeks++;
            }
            if (nrWeeks < 7)
            {
                firstWeek = 0;
                lastWeek = nrWeeks;
                divWeeks = nrWeeks;
            }
            for (int i = (firstWeek); i <= (lastWeek); i++)
            {
                kpiDynamic += dynamic[i];
            }
            if (divWeeks == 0) divWeeks = 1;
            kpiDynamic /= divWeeks;
            return kpiDynamic;
        }

        public double GetKpiDay(int MachineGroupID, int WeekNo, DateTime targetTime, out double opHours)
        {
            double kpiDay = 0;
            opHours = 0;
            MachineKpis mKpis;
            if (WeekNo == 0)
            {
                mKpis = todaysMachineKPIs;
            }
            else
            {
                mKpis = histMachineKPIs;
            }
            DateTime end = targetTime.AddDays(-7 * WeekNo);
            DateTime start = end.Date;
            KPIMaxList idmList = new KPIMaxList();
            foreach (MachineKpi mkpi in mKpis)
            {
                if ((mkpi.RecTimeStamp > start) && (mkpi.RecTimeStamp <= end))
                {
                    if (mkpi.MachineGroupID == MachineGroupID)
                    {
                        double machineKPIValue = (double)mkpi.Value;
                        double operatorKPIHours = (double)mkpi.OperatorHours;
                        if ((OperatorKPIMode == "OperatorProduction") || (OperatorKPIMode == "Production"))
                        {
                            Machine m = this.GetMachine(mkpi.MachineID);
                            if ((m != null) && m.OperatorCountExitPoint && (OperatorKPIMode == "OperatorProduction"))
                            {
                                machineKPIValue = (double)mkpi.OperatorProduction;
                            }
                        }
                        idmList.TestMax(mkpi.MachineID, machineKPIValue, operatorKPIHours);
                    }
                }
            }
            foreach (KPIMax idm in idmList)
            {
                kpiDay += idm.max;
                opHours += idm.time;
            }
            return kpiDay;
        }

        public string HourlyKPIType { get; set; }
        public string OperatorKPIMode { get; set; }

        //public KPISet GetGroupKPI(string machineArea)
        //{
        //    MachineGroups mgs = this.GetAllMachineGroups();
        //    Def_Units dus = this.GetAllDef_Units();
        //    ReadMachineKPIs();
        //    KPISet groupKPI = new KPISet();
        //    try
        //    {
        //        DateTime targetTime = this.ServerTime;
        //        foreach (MachineGroup mg in mgs)
        //        {
        //            if ((mg.MachineArea == machineArea) && (mg.DisplayInMaster))
        //            {
        //                Def_Unit du = dus.GetById(mg.UnitsKPI);
        //                KPI kpi = new KPI();
        //                kpi.MachineArea = machineArea;
        //                if (du != null)
        //                {
        //                    kpi.Units = du.ShortDescription;
        //                }
        //                else
        //                {
        //                    kpi.Units = mg.UnitsKPI.ToString();
        //                }
        //                kpi.StartHrs = mg.StartTime;
        //                kpi.EndHrs = mg.EndTime;
        //                kpi.GroupDescription = mg.ShortDescription;
        //                kpi.FixedComparison = mg.DailyKPI;
        //                int offsetMins = -60;
        //                double factor = 1;
        //                if (HourlyKPIType == "Last30Mins")
        //                {
        //                    factor = 2;
        //                    offsetMins = -30;
        //                }
        //                if (HourlyKPIType == "Last15Mins")
        //                {
        //                    factor = 4;
        //                    offsetMins = -15;
        //                }
        //                double opTime = 0;
        //                double kpiday = GetKpiDay(mg.idJensen, 0, targetTime, out opTime);
        //                double kpidynamic = GetKpiDynamic(mg.idJensen, targetTime);
        //                double opTimePrev = 0;
        //                double kpidayPrev = GetKpiDay(mg.idJensen, 0, targetTime.AddMinutes(offsetMins), out opTimePrev);
        //                double kpidynamicPrev = GetKpiDynamic(mg.idJensen, targetTime.AddMinutes(offsetMins));
        //                kpi.Today = (int)kpiday;
        //                kpi.DynamicAverage = (int)kpidynamic;

        //                groupKPI.Add(kpi);
        //                kpi = new KPI();
        //                kpi.MachineArea = machineArea;
        //                if (du != null)
        //                    kpi.Units = du.ShortDescription + "/Hr";
        //                else
        //                    kpi.Units = "/Hr";
        //                kpi.StartHrs = mg.StartTime;
        //                kpi.EndHrs = mg.EndTime;
        //                kpi.GroupDescription = mg.ShortDescription;
        //                kpi.FixedComparison = mg.HourlyKPI;
        //                if (OperatorKPIMode == "NoOperator")
        //                {
        //                    if ((HourlyKPIType == "LastHour") || (HourlyKPIType == "Last30Mins") || (HourlyKPIType == "Last15Mins"))
        //                    {
        //                        kpi.Today = (int)(factor * (kpiday - kpidayPrev));
        //                        kpi.DynamicAverage = (int)(factor * (kpidynamic - kpidynamicPrev));
        //                    }
        //                    if (HourlyKPIType == "Today")
        //                    {
        //                        double elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
        //                        if (elapsedHours == 0) elapsedHours = 1.0;
        //                        double today = (double)kpiday / elapsedHours;
        //                        kpi.Today = (int)today;
        //                        double dynamic = (double)kpidynamic / elapsedHours;
        //                        kpi.DynamicAverage = (int)dynamic;
        //                    }
        //                }
        //                else
        //                {
        //                    double elapsedHours = opTime;
        //                    if (elapsedHours == 0)
        //                        elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
        //                    double today = 0;
        //                    if (elapsedHours != 0)
        //                        today = (double)kpiday / elapsedHours;
        //                    kpi.Today = (int)today;
        //                    elapsedHours = opTimePrev;
        //                    if (elapsedHours == 0)
        //                        elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
        //                    double dynamic = 0;
        //                    if (elapsedHours != 0)
        //                        dynamic = (double)kpidynamic / elapsedHours;
        //                    kpi.DynamicAverage = (int)dynamic;
        //                }
        //                groupKPI.Add(kpi);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //    return groupKPI;
        //}

        public KPISet GetGroupKPI(string machineArea)
        {
            MachineGroups mgs = this.GetAllMachineGroups();
            Def_Units dus = this.GetAllDef_Units();
            ReadMachineKPIs();
            KPISet groupKPI = new KPISet();
            try
            {
                DateTime targetTime = this.ServerTime;
                foreach (MachineGroup mg in mgs)
                {
                    if ((mg.MachineArea == machineArea) && (mg.DisplayInMaster))
                    {
                        Def_Unit du;
                        KPI kpi = new KPI();
                        kpi.MachineArea = machineArea;
                        double factor = 1;
                        double opTime;
                        double kpiday;
                        double kpidynamic;
                        double opTimePrev = 0;
                        double kpidayPrev;
                        double kpidynamicPrev;
                        du = dus.GetById(mg.UnitsKPI);
                        if (du != null)
                        {
                            kpi.Units = du.ShortDescription;
                        }
                        else
                        {
                            kpi.Units = mg.UnitsKPI.ToString();
                        }
                        kpi.StartHrs = mg.StartTime;
                        kpi.EndHrs = mg.EndTime;
                        kpi.GroupDescription = mg.ShortDescription;
                        int offsetMins = -60;
                        if (HourlyKPIType == "Last30Mins")
                        {
                            factor = 2;
                            offsetMins = -30;
                        }
                        if (HourlyKPIType == "Last15Mins")
                        {
                            factor = 4;
                            offsetMins = -15;
                        }
                        opTime = 0;
                        kpiday = GetKpiDay(mg.idJensen, 0, targetTime, out opTime);
                        kpidynamic = GetKpiDynamic(mg.idJensen, targetTime);

                        kpidayPrev = GetKpiDay(mg.idJensen, 0, targetTime.AddMinutes(offsetMins), out opTimePrev);
                        kpidynamicPrev = GetKpiDynamic(mg.idJensen, targetTime.AddMinutes(offsetMins));
                        kpi.Today = (int)kpiday;
                        kpi.DynamicAverage = (int)kpidynamic;
                        kpi.FixedComparison = mg.DailyKPI;
                        //if (OperatorKPIMode == "NoOperator")
                        //{
                        //    kpi.FixedComparison = mg.DailyKPI;
                        //}
                        //else
                        //{
                        //    //daily kpi fixed comparison for rentex mode.
                        //    //TimeSpan ts = targetTime.Subtract(DateTime.Today);
                        //    //double machineHours = ts.TotalHours - mg.StartTime;
                        //    if (opTime > 0)
                        //        //kpi.FixedComparison = mg.DailyKPI / mg.MachineGroupSubIDs;
                        //        kpi.FixedComparison = mg.GroupOperatorNorm;
                        //    else
                        //        kpi.FixedComparison = mg.DailyKPI;
                        //}

                        groupKPI.Add(kpi);
                        kpi = new KPI();
                        kpi.MachineArea = machineArea;
                        if (du != null)
                            kpi.Units = du.ShortDescription + "/Hr";
                        else
                            kpi.Units = "/Hr";
                        kpi.StartHrs = mg.StartTime;
                        kpi.EndHrs = mg.EndTime;
                        kpi.GroupDescription = mg.ShortDescription;
                        if (OperatorKPIMode == "NoOperator")
                        {
                            kpi.FixedComparison = mg.HourlyKPI;
                            if ((HourlyKPIType == "LastHour") || (HourlyKPIType == "Last30Mins") || (HourlyKPIType == "Last15Mins"))
                            {
                                kpi.Today = (int)(factor * (kpiday - kpidayPrev));
                                kpi.DynamicAverage = (int)(factor * (kpidynamic - kpidynamicPrev));
                            }
                            if (HourlyKPIType == "Today")
                            {
                                double elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
                                if (elapsedHours == 0) elapsedHours = 1.0;
                                double today = (double)kpiday / elapsedHours;
                                kpi.Today = (int)today;
                                double dynamic = (double)kpidynamic / elapsedHours;
                                kpi.DynamicAverage = (int)dynamic;
                            }
                        }
                        else
                        {
                            //if (opTime > 0)
                            //    kpi.FixedComparison = mg.HourlyKPI / mg.MachineGroupSubIDs;
                            //else
                            int norm = mg.GroupOperatorNorm;
                            if (norm==0)
                                norm=mg.HourlyKPI;
                            kpi.FixedComparison = norm;

                            double elapsedHours = opTime;
                            if (elapsedHours == 0)
                                elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
                            double today = 0;
                            if (elapsedHours != 0)
                                today = (double)kpiday / elapsedHours;
                            kpi.Today = (int)today;
                            elapsedHours = opTimePrev;
                            if (elapsedHours == 0)
                                elapsedHours = ((targetTime.Hour - kpi.StartHrs) + ((double)targetTime.Minute / 60.0));
                            double dynamic = 0;
                            if (elapsedHours != 0)
                                dynamic = (double)kpidynamic / elapsedHours;
                            kpi.DynamicAverage = (int)dynamic;
                        }
                        groupKPI.Add(kpi);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return groupKPI;
        }


        public ProductionSet GetTodaysReferenceProductionm(MachineArea machineArea)
        {
            string mArea = "WT";
            if (machineArea == MachineArea.FT)
            {
                mArea = "FT";
            }

            if (machineArea == MachineArea.GT)
            {
                mArea = "GT";
            }
            return GetReferenceAreaProduction(mArea);
        }

        public ProductionSet GetReferenceAreaProduction(string machineArea)
        {
            ProductionSet productionSet = new ProductionSet();
            MachineGroups mgs = this.GetAllMachineGroups();
            ReadMachineKPIs();
            int startHour=mgs[0].StartTime;
            int endHour=mgs[0].EndTime;
            foreach (MachineGroup mg in mgs)
            {
                if (mg.DisplayInMaster)
                {
                    if (mg.StartTime < startHour)
                    {
                        startHour = mg.StartTime;
                    }
                    if (mg.EndTime > endHour)
                    {
                        endHour = mg.EndTime;
                    }
                }
            }
            MachineKpis mKpis = histMachineKPIs;

            foreach (MachineGroup mg in mgs)
            {
                if ((mg.MachineArea == machineArea) && (mg.DisplayInMaster))
                {
                    foreach (MachineKpi mKpi in mKpis)
                    {
                        if ((mKpi.RecTimeStamp.Hour >= startHour) && (mKpi.RecTimeStamp.Hour < endHour)
                            && (mKpi.RecTimeStamp < this.ServerTime) && (mKpi.MachineGroupID == mg.idJensen))
                        {
                            DateTime recTime = this.ServerTime.Date;
                            recTime=recTime.Add(mKpi.RecTimeStamp.TimeOfDay);
                            Production production = productionSet.GetByIDTime(mKpi.MachineGroupID, recTime);
                            TimeSpan ts=this.ServerTime.Subtract(mKpi.RecTimeStamp.Date);
                            int week = (ts.Days / 7) - 1;
                            if (production == null)
                            {
                                production = new Production();
                                production.LogTime = recTime;
                                production.MachineGroupID = mKpi.MachineGroupID;
                                production.GroupDescription = mg.ShortDescription;
                                production.Weeks[week] = (int)mKpi.Value;
                                productionSet.Add(production);
                            }
                            else
                            {
                                production.Weeks[week] += (int)mKpi.Value;
                            }
                        }
                    }
                }
            }
            foreach (Production production in productionSet)
                production.AverageMid5();
            return productionSet;
        }

        public ProductionSet GetTodaysProductionm(MachineArea machineArea)
        {
            string mArea = "WT";
            if (machineArea == MachineArea.FT)
            {
                mArea = "FT";
            }

            if (machineArea == MachineArea.GT)
            {
                mArea = "GT";
            }
            return GetTodayAreaProduction(mArea);
        }

        public ProductionSet GetTodayAreaProduction(string machineArea)
        {
            ProductionSet productionSet = new ProductionSet();
            MachineGroups mgs = this.GetAllMachineGroups();
            ReadMachineKPIs();
            int startHour = mgs[0].StartTime;
            int endHour = mgs[0].EndTime;
            foreach (MachineGroup mg in mgs)
            {
                if (mg.DisplayInMaster)
                {
                    if (mg.StartTime < startHour)
                    {
                        startHour = mg.StartTime;
                    }
                    if (mg.EndTime > endHour)
                    {
                        endHour = mg.EndTime;
                    }
                }
            }
            MachineKpis mKpis = todaysMachineKPIs;

            foreach (MachineGroup mg in mgs)
            {
                if ((mg.MachineArea == machineArea) && (mg.DisplayInMaster))
                {
                    foreach (MachineKpi mKpi in mKpis)
                    {
                        if ((mKpi.RecTimeStamp.Hour >= startHour) && (mKpi.RecTimeStamp.Hour < endHour)
                            && (mKpi.RecTimeStamp < this.ServerTime) && (mKpi.MachineGroupID == mg.idJensen))
                        {
                            Production production = productionSet.GetByIDTime(mKpi.MachineGroupID, mKpi.RecTimeStamp);
                            if (production == null)
                            {
                                production = new Production();
                                production.LogTime = mKpi.RecTimeStamp;
                                production.MachineGroupID = mKpi.MachineGroupID;
                                production.GroupDescription = mg.ShortDescription;
                                production.Value = (int)mKpi.Value;
                                productionSet.Add(production);
                            }
                            else
                            {
                                production.Value += (int)mKpi.Value;
                            }
                        }
                    }
                }
            }
            return productionSet;
        }


        #endregion

        #region Insert data

        public Boolean UseMachineSubIDKPI = false;

        public void InsertNewDataRec(MachineKpi mkpi)
        {
            string commandString =
                @"INSERT INTO [dbo].[tblMachineKpi]
                           ([RecTimeStamp]
                           ,[MachineID]
                           ,[Value]
                           ,[Unit]
                           ,[KpiID]
                           ,[CountExitPointData])
                     VALUES
                            (@RecTimeStamp
                           ,@MachineID
                           ,@Value
                           ,@Unit
                           ,@KpiID
                           ,@CountExitPointData)";
            if (this.DatabaseVersion >= 1.95)
            {
                commandString = @"INSERT INTO [dbo].[tblMachineKPI]
                                       ([RecTimeStamp]
                                       ,[MachineID]
                                       ,[Value]
                                       ,[OperatorProduction]
                                       ,[OperatorHours]
                                       ,[Unit]
                                       ,[KpiID]
                                       ,[CountExitPointData])
                                 VALUES
                                       (@RecTimeStamp
                                       ,@MachineID
                                       ,@Value
                                       ,@OperatorProduction
                                       ,@OperatorHours
                                       ,@Unit
                                       ,@KpiID
                                       ,@CountExitPointData)";
            }
            if (this.DatabaseVersion >= 1.96)
            {
                commandString = @"INSERT INTO [dbo].[tblMachineKPI]
                                       ([RecTimeStamp]
                                       ,[MachineID]
                                       ,[SubID]
                                       ,[Value]
                                       ,[OperatorProduction]
                                       ,[OperatorHours]
                                       ,[Unit]
                                       ,[KpiID]
                                       ,[CountExitPointData])
                                 VALUES
                                       (@RecTimeStamp
                                       ,@MachineID
                                       ,@SubID
                                       ,@Value
                                       ,@OperatorProduction
                                       ,@OperatorHours
                                       ,@Unit
                                       ,@KpiID
                                       ,@CountExitPointData)";
            }
//            if (this.DatabaseVersion >= 1.97)
//            {
//                commandString = @"INSERT INTO [dbo].[tblMachineKPI]
//                                       ([RecTimeStamp]
//                                       ,[MachineID]
//                                       ,[SubID]
//                                       ,[Value]
//                                       ,[OperatorID]
//                                       ,[OperatorProduction]
//                                       ,[OperatorHours]
//                                       ,[Unit]
//                                       ,[KpiID]
//                                       ,[CountExitPointData])
//                                 VALUES
//                                       (@RecTimeStamp
//                                       ,@MachineID
//                                       ,@SubID
//                                       ,@Value
//                                       ,@OperatorID
//                                       ,@OperatorProduction
//                                       ,@OperatorHours
//                                       ,@Unit
//                                       ,@KpiID
//                                       ,@CountExitPointData)";
//            }
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@RecTimeStamp", mkpi.RecTimeStamp);
                    command.Parameters.AddWithValue("@MachineID", mkpi.MachineID);
                    command.Parameters.AddWithValue("@Value", mkpi.Value);
                    if (this.DatabaseVersion >= 1.95)
                    {
                        command.Parameters.AddWithValue("@OperatorProduction", mkpi.OperatorProduction);
                        command.Parameters.AddWithValue("@OperatorHours", mkpi.OperatorHours);
                    }
                    if (this.DatabaseVersion >= 1.96)
                    {
                        command.Parameters.AddWithValue("@SubID", mkpi.SubID);
                    }
                    //if (this.DatabaseVersion >= 1.97)
                    //{
                    //    command.Parameters.AddWithValue("@OperatorID", mkpi.OperatorID);
                    //}
                    command.Parameters.AddWithValue("@Unit", mkpi.UnitID);
                    command.Parameters.AddWithValue("@KpiID", mkpi.KpiID);
                    command.Parameters.AddWithValue("@CountExitPointData", mkpi.CountExitPointData);
 
                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;
                        if (ex.Number != insertError)
                        {
                            throw;
                        }
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

        public void InsertNewDataRec(PublicKpi pkpi)
        {
            string commandString =
                @"INSERT INTO [dbo].[tblKpi]
                               ([KpiID]
                               ,[DateHour]
                               ,[MachineID]
                               ,[SubID]
                               ,[KpiType]
                               ,[KpiTypeID]
                               ,[KpiSubTypeID]
                               ,[Value]
                               ,[Unit]
                               ,[KpiTime])
                         VALUES
                               (@KpiID
                               ,@DateHour
                               ,@MachineID
                               ,@SubID
                               ,@KpiType
                               ,@KpiTypeID
                               ,@KpiSubTypeID
                               ,@Value
                               ,@Unit
                               ,@KpiTime)";
            
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@KpiID", pkpi.KpiID);
                    command.Parameters.AddWithValue("@DateHour", pkpi.DateHour);
                    command.Parameters.AddWithValue("@MachineID", pkpi.MachineID);
                    command.Parameters.AddWithValue("@SubID", pkpi.SubID);
                    command.Parameters.AddWithValue("@KpiType", pkpi.KpiType);
                    command.Parameters.AddWithValue("@KpiTypeID", pkpi.KpiTypeID);
                    command.Parameters.AddWithValue("@KpiSubTypeID", pkpi.KpiSubTypeID);
                    command.Parameters.AddWithValue("@Value", pkpi.Value);
                    command.Parameters.AddWithValue("@Unit", pkpi.Unit);
                    command.Parameters.AddWithValue("@KpiTime", pkpi.KpiTime);

                    try
                    {
                        command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenPublic);
                    }
                    catch (SqlException ex)
                    {
                        const int insertError = 2601;
                        if (ex.Number != insertError)
                        {
                            throw;
                        }
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

        #region Delete Data

        public void DeleteMachineKpiPeriod(DateTime start, DateTime end)
        {
            const string commandString =
                @"DELETE FROM [dbo].[tblMachineKpi]
                    WHERE RecTimeStamp >= @start
                    and RecTimeStamp < @end";
            try
            {
                using (SqlCommand command = new SqlCommand(commandString))
                {
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                    command.ExecuteNonQuery(SqlDataConnection.DBConnection.JensenGroup);
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

    #region Data Collection Class

    public class MachineKpis : List<MachineKpi>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblMachineKPI";
        private DateTime lastDBUpdate;
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
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }

        public int Recnum { get; set; }
        public DateTime RecTimeStamp { get; set; }
        public int MachineID { get; set; }
        public int SubID { get; set; }
        public decimal Value { get; set; }
        public int UnitID { get; set; }
        public int KpiID { get; set; }

        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines();

            int RecnumPos = dr.GetOrdinal("Recnum");
            int RecTimeStampPos = dr.GetOrdinal("RecTimeStamp");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = 0;
            int ValuePos = dr.GetOrdinal("Value");
            int UnitIDPos = dr.GetOrdinal("Unit");
            int KpiIDPos = dr.GetOrdinal("KpiID");
            int CountExitPointDataPos = dr.GetOrdinal("CountExitPointData");
            int OperatorHourPos = 0;
            int OperatorProductionPos = 0;
            int KpiTypePos = 0;
            int KpiSubTypePos = 0;
            int KpiSubTypeIDPos = 0;
            int ExpectedValuePos = 0;
            int OperatorIDPos = 0;
            if (da.DatabaseVersion >= 1.95)
            {
                OperatorHourPos = dr.GetOrdinal("OperatorHours");
                OperatorProductionPos = dr.GetOrdinal("OperatorProduction");
            }
            if (da.DatabaseVersion >= 1.98)
            {
                OperatorIDPos = dr.GetOrdinal("OperatorID");
                SubIDPos = dr.GetOrdinal("SubID");
                KpiTypePos = dr.GetOrdinal("KpiType");
                KpiSubTypePos = dr.GetOrdinal("KpiSubType");
                KpiSubTypeIDPos = dr.GetOrdinal("KpiSubTypeID");
                ExpectedValuePos = dr.GetOrdinal("ExpectedValue");
            }

            this.Clear();
            while (dr.Read())
            {
                MachineKpi mKPI = new MachineKpi();
                mKPI.Recnum = dr.GetInt32(RecnumPos);
                mKPI.RecTimeStamp = dr.GetDateTime(RecTimeStampPos);
                mKPI.MachineID = dr.GetInt32(MachineIDPos);
                Machine m = ms.GetById(mKPI.MachineID);
                if (m != null)
                    mKPI.MachineGroupID = m.MachineGroup_idJensen;
                else
                    mKPI.MachineGroupID = 0;
                mKPI.Value = dr.GetDecimal(ValuePos);
                if (da.DatabaseVersion >= 1.95)
                {
                    mKPI.PreviousOpHours = (double)dr.GetDecimal(OperatorHourPos);
                    mKPI.OperatorProduction = dr.GetDecimal(OperatorProductionPos);
                }
                mKPI.UnitID = dr.GetInt32(UnitIDPos);
                mKPI.KpiID = dr.GetInt32(KpiIDPos);
                mKPI.CountExitPointData = dr.GetBoolean(CountExitPointDataPos);
                if (da.DatabaseVersion >= 1.98)
                {
                    mKPI.SubID = dr.GetInt32(SubIDPos);
                    mKPI.KpiType = dr.GetInt32(KpiTypePos);
                    mKPI.KpiSubType = dr.GetInt32(KpiSubTypePos);
                    mKPI.KpiSubTypeID = dr.GetInt32(KpiSubTypeIDPos);
                    mKPI.ExpectedValue = dr.GetInt32(ExpectedValuePos);
                    mKPI.OperatorID = dr.GetInt32(OperatorIDPos);
                }
                // add to the machineKPI collection
                this.Add(mKPI);
            }
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public MachineKpi GetById(int id)
        {
            return this.Find(delegate(MachineKpi machineKpi)
            {
                return (machineKpi.MachineID == id);
            });
        }

        public MachineKpi getByIDTime(int machineID, DateTime rectimestamp)
        {
            return this.Find(delegate(MachineKpi machineKpi)
            {
                return ((machineKpi.MachineID == machineID) 
                    &&(machineKpi.RecTimeStamp==rectimestamp));
            });
        }


        public MachineKpi GetByIds(int machineID, int subID)
        {
            return this.Find(delegate(MachineKpi machineKpi)
            {
                return ((machineKpi.MachineID == machineID)
                    && (machineKpi.SubID == subID));
            });
        }

        public MachineKpi GetByIdsTime(int machineID, int subID, DateTime rectimestamp)
        {
            return this.Find(delegate(MachineKpi machineKpi)
            {
                return ((machineKpi.MachineID == machineID)
                    && (machineKpi.SubID == subID)
                    && (machineKpi.RecTimeStamp == rectimestamp));
            });
        }

        public MachineKpi GetByIds(int machineID, int subID, int operatorid)
        {
            return this.Find(delegate(MachineKpi machineKpi)
            {
                return ((machineKpi.MachineID == machineID)
                    && (machineKpi.SubID == subID)
                    && (machineKpi.OperatorID == operatorid));
            });
        }

        public MachineKpis GetAllbyID(int machineID, int subID)
        {
            MachineKpis mkpis = new MachineKpis();
            foreach (MachineKpi kpi in this)
            {
                if ((kpi.MachineID == machineID)
                    && ((kpi.SubID == subID) || (subID <= 0)))
                    mkpis.Add(kpi);
            }
            return mkpis;
        }


    
    }

    public class PublicKpis : List<PublicKpi>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblKPI";
        private DateTime lastDBUpdate;
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
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }

        public int Recnum { get; set; }
        public DateTime RecTimeStamp { get; set; }
        public int MachineID { get; set; }
        public decimal Value { get; set; }
        public int UnitID { get; set; }
        public int KpiID { get; set; }

        public int Fill(SqlDataReader dr)
        {
            SqlDataAccess da = SqlDataAccess.Singleton;
            Machines ms = da.GetAllMachines();

            int KpiIDPos = dr.GetOrdinal("KpiID");
            int DateHourPos = dr.GetOrdinal("DateHour");
            int MachineIDPos = dr.GetOrdinal("MachineID");
            int SubIDPos = dr.GetOrdinal("SubID");
            int KpiTypePos = dr.GetOrdinal("KpiType");
            int KpiTypeIDPos = dr.GetOrdinal("KpiTypeID");
            int KpiSubTypeIDPos = dr.GetOrdinal("KpiSubTypeID");
            int ValuePos = dr.GetOrdinal("Value");
            int UnitIDPos = dr.GetOrdinal("Unit");
            int KpiTimePos = dr.GetOrdinal("KpiTime");


            this.Clear();
            while (dr.Read())
            {
                PublicKpi kPI = new PublicKpi();
                kPI.KpiID = dr.GetInt32(KpiIDPos);
                kPI.DateHour = dr.GetDateTime(DateHourPos);
                kPI.MachineID = dr.GetInt32(MachineIDPos);
                kPI.SubID = dr.GetInt32(SubIDPos);
                kPI.KpiType = dr.GetInt32(KpiTypePos);
                kPI.KpiTypeID = dr.GetInt32(KpiTypeIDPos);
                kPI.KpiSubTypeID = dr.GetInt32(KpiSubTypeIDPos);

                kPI.Value = dr.GetDecimal(ValuePos);

                kPI.Unit = dr.GetInt32(UnitIDPos);
                kPI.KpiTime = dr.GetInt32(KpiTimePos);

                this.Add(kPI);
            }
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }

        public PublicKpi GetById(int id)
        {
            return this.Find(delegate(PublicKpi machineKpi)
            {
                return (machineKpi.MachineID == id);
            });
        }

        public PublicKpi GetByIds(int machineID, int subID)
        {
            return this.Find(delegate(PublicKpi machineKpi)
            {
                return ((machineKpi.MachineID == machineID)
                    && (machineKpi.SubID == subID));
            });
        }
    }

    public class KPISet : List<KPI>, IDataFiller
    {
        private double lifespan = 1.0;
        private string tblName = "tblMachineGroupKPI";
        private DateTime lastDBUpdate;
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
        private bool neverExpire = false;
        public bool NeverExpire
        {
            get { return neverExpire; }
            set { neverExpire = value; }
        }
        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                bool test = isValid && (this.Count > 0) && (lastRead != null) && (!neverExpire);
                if (test)
                {
                    SqlDataAccess da = SqlDataAccess.Singleton;
                    lastDBUpdate = da.TableLastUpdated(tblName);
                    int x = lastDBUpdate.CompareTo(lastRead.AddSeconds(0.95));
                    test = (x <= 0);
                    if (test)
                    {
                        DateTime testTime = lastRead.AddHours(lifespan);
                        test = testTime > da.ServerTime;
                    }
                }
                return test || neverExpire;
            }
            set
            {
                isValid = value;
                if (!isValid)
                    neverExpire = false;
            }
        }


        public int Fill(SqlDataReader dr)
        {
            int MachineAreaPos = dr.GetOrdinal("MachineArea");
            int GroupDescription = dr.GetOrdinal("GroupDescription");
            int UnitsPos = dr.GetOrdinal("Units");
            int TodayPos = dr.GetOrdinal("Today");
            int DynamicAveragePos = dr.GetOrdinal("DynamicAverage");
            int FixedComparisonPos = dr.GetOrdinal("FixedComparison");
            int StartHrsPos = dr.GetOrdinal("StartHrs");
            int EndHrsPos = dr.GetOrdinal("EndHrs");

            this.Clear();
            while (dr.Read())
            {
                KPI _KPI = new KPI();
                _KPI.MachineArea = dr.GetString(MachineAreaPos);
                _KPI.GroupDescription = dr.GetString(GroupDescription);
                _KPI.Units = dr.GetString(UnitsPos);
                _KPI.Today = dr.GetInt32(TodayPos);
                _KPI.DynamicAverage = dr.GetInt32(DynamicAveragePos);
                _KPI.FixedComparison = dr.GetInt32(FixedComparisonPos);
                _KPI.StartHrs = dr.GetInt32(StartHrsPos);
                _KPI.EndHrs = dr.GetInt32(EndHrsPos);

                // add to the KPI collection
                this.Add(_KPI);
            }
            SqlDataAccess da = SqlDataAccess.Singleton;
            lastRead = da.ServerTime;
            isValid = true;

            return this.Count;
        }
    }
    #endregion

    #region Item Class
    public class KPI
    {
        public String MachineArea { get; set; }
        public String GroupDescription { get; set; }
        public String Units { get; set; }
        public int Today { get; set; }
        public int DynamicAverage { get; set; }
        public int FixedComparison { get; set; }
        public int StartHrs { get; set; }
        public int EndHrs { get; set; }

        public int TodayDynamicComp
        {
            get
            {
                if (this.DynamicAverage != 0)
                {
                    return (100 * this.Today) / this.DynamicAverage;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int TodayFixedComp
        {
            get
            {
                if (this.FixedComparison != 0)
                {
                    // hourly?
                    if (this.Units.IndexOf("Hr") > 0)
                    {
                        return (100 * this.Today) / this.FixedComparison;
                    }
                    else
                    {
                        SqlDataAccess da = SqlDataAccess.Singleton;
                        //int mins = (60 * DateTime.Now.Hour) + DateTime.Now.Minute;
                        int mins = (60 * da.ServerTime.Hour) + da.ServerTime.Minute;
                        int fxd = this.FixedComparison * (mins - (60 * StartHrs)) / (60 * (EndHrs - StartHrs));
                        if (fxd > this.FixedComparison)
                            fxd = this.FixedComparison;
                        //return (100 * this.Today) / this.FixedComparison;
                        return (100 * this.Today) / fxd;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class MachineInfos : List<MachineInfo>
    {
        public MachineInfo GetByIDs(int machineid, int subid)
        {
            return this.Find(delegate(MachineInfo machineKpi)
            {
                return ((machineKpi.MachineID == machineid)
                    && (machineKpi.SubID == subid));
            });
        }
    }

    public class MachineInfo
    {
        public int MachineID { get; set; }
        public int SubID { get; set; }
        public int OperatorID { get; set; }
    }

    public class MachineKpi : IComparable
    {
        public int Recnum{ get; set; }
        public DateTime RecTimeStamp { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public DateTime LastOpLoginOut { get; set; }
        public int MachineID { get; set; }
        public int SubID { get; set; }
        public int MachineGroupID { get; set; }
        public decimal Value { get; set; }
        public double PreviousOpHours { get; set; }
        public double CurrentOpHours { get; set; }
        public decimal OperatorProduction;
        public decimal OperatorHours
        {
            get { return (decimal)(PreviousOpHours+CurrentOpHours); }
        }
        private int operatorID = -1;
        public int OperatorID
        {
            get { return operatorID; }
            set { operatorID = value; }
        }
        public bool OperatorLoggedIn { get; set; }
        public int UnitID { get; set; }
        public int KpiID { get; set; }
        public bool CountExitPointData { get; set; }
        public int KpiType { get; set; }
	    public int KpiSubType { get; set; }
	    public int KpiSubTypeID { get; set; }
        public int ExpectedValue { get; set; }

        public string ToCsv()
        {
            return Recnum + ", " + RecTimeStamp.ToShortTimeString() + ", " + MachineID + ", " + SubID + ", " + Value + ", " + ExpectedValue;
        }

        public MachineKpi Clone()
        {
            return (MachineKpi)this.MemberwiseClone();
        }

        public int CompareTo(Object _other)
        {
            MachineKpi other = (MachineKpi)_other;
            int compareTo = this.MachineID.CompareTo(other.MachineID);
            if (compareTo == 0)
            {
                compareTo = this.SubID.CompareTo(other.SubID);
            }
            if (compareTo == 0)
            {
                compareTo = this.OperatorID.CompareTo(other.OperatorID);
            }
            if (compareTo == 0)
            {
                compareTo = this.KpiType.CompareTo(other.KpiType);
            }
            if (compareTo == 0)
            {
                compareTo = this.KpiSubType.CompareTo(other.KpiSubType);
            }
            if (compareTo == 0)
            {
                compareTo = this.KpiSubTypeID.CompareTo(other.KpiSubTypeID);
            }
            if (compareTo == 0)
            {
                compareTo = this.RecTimeStamp.CompareTo(other.RecTimeStamp);
            }
            return compareTo;
        }
    }

    public class PublicKpi
    {
        public int KpiID { get; set; }
        public DateTime DateHour { get; set; }
        public int MachineID { get; set; }
        public int SubID { get; set; }
        public int KpiType { get; set; }
        public int KpiTypeID { get; set; }
        public int KpiSubTypeID { get; set; }
        public decimal Value { get; set; }
        public int Unit { get; set; }
        public int KpiTime { get; set; }
    }

    public class KPIMaxList : List<KPIMax>
    {
        public KPIMax GetById(int id)
        {
            return this.Find(IDMax => IDMax.id == id);
        }

        public void TestMax(int id, double aVal, double timeVal)
        {
            KPIMax idm = GetById(id);
            if (idm == null)
            {
                idm = new KPIMax();
                idm.id = id;
                idm.max = aVal;
                idm.time = timeVal;
                this.Add(idm);
            }
            else
            {
                if (idm.max < aVal)
                {
                    idm.max = aVal;
                    idm.time = timeVal;
                }
            }
        }

    }

    public class KPIMax
    {
        public int id { get; set; }
        public double max { get; set; }
        public double time { get; set; }
    }
    #endregion
}
