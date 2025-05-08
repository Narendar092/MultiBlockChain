using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;
using XIDatabase;
using System.Configuration;
using System.Data;
using static iTextSharp.text.pdf.AcroFields;

namespace XICore
{
    public class XIInfraKPIComponent:XIDefinitionBase
    {

        public int iRoleID;

        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
            }
            try
            {
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                var sRoleID = oParams.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCoreDatabase = oParams.Where(m => m.sName.ToLower() == "sCoreDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var o1ClickID = oParams.Where(m => m.sName.ToLower() == "s1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var KPIID = oParams.Where(m => m.sName.ToLower() == "Kpi ID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iApplicationID = 0;
                var sAppID = oParams.Where(m => m.sName == XIConstant.Param_ApplicationID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sAppID, out iApplicationID);
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sRoleID, out iRoleID);
                //if (iRoleID > 0)
                //{
                Guid KpiIDXIGUID = new Guid();
                int iKpiID = 0;
                string sQuery = string.Empty;
                if (int.TryParse(KPIID, out iKpiID))
                {
                   sQuery = "select * from XIKPI_T where ID =" + KPIID + " and XIDeleted=0 and iStatus=10";
                }
                if (Guid.TryParse(KPIID, out KpiIDXIGUID))
                {
                    sQuery = "select * from XIKPI_T where XIGUID ='" + KPIID + "' and XIDeleted=0 and iStatus=10";
                }
                string sReturnValue = string.Empty;
                XID1Click oXI1Click = new XID1Click();
                oXI1Click.Query = sQuery;
                oXI1Click.Name = "XIKpi";
                oXI1Click.BOIDXIGUID = new Guid("3193A422-39AF-400E-AAEC-F60367500D2B");
                var QueryResult = oXI1Click.Execute_Query();
                    List<XIDKPI> KPIs = new List<XIDKPI>();
                    if (QueryResult.Rows.Count > 0)
                    {
                        KPIs = (from DataRow row in QueryResult.Rows
                                  select new XIDKPI
                                  {
                                      XIGUID = Convert.ToString(row["XIGUID"]),
                                      FKi1ClickIDXIGUID = Convert.ToString(row["FKi1ClickIDXIGUID"]),
                                      FKiComponentIDXIGUID = Convert.ToString(row["FKiComponentIDXIGUID"]),
                                      sColors = Convert.ToString(row["sColors"]),
                                      FKiKPIGroupIDXIGUID = Convert.ToString(row["FKiKPIGroupIDXIGUID"]),
                                      bToolTip = Convert.ToBoolean(row["bToolTip"]),
                                      bGridLines = Convert.ToBoolean(row["bGridLines"]),
                                      sKpiTitle = Convert.ToString(row["sKpiTitle"]),
                                      bIsCursor = Convert.ToBoolean(row["bIsCursor"]),
                                      bIsLegends = Convert.ToBoolean(row["bIsLegends"]),
                                      sLegendPosition = Convert.ToString(row["sLegendPosition"]),
                                      iRefreshingType = Convert.ToInt32(row["iRefreshingtype"]),
                                      iSetinterval = Convert.ToInt32(row["iSetinterval"]),
                                      RowXiLinkIDXIGUID = Convert.ToString(row["RowXiLinkIDXIGUID"])
                                  }).ToList();
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                var IndividualKpi = KPIs.FirstOrDefault();//KPIs.GroupBy(k => k.XIGUID).ToDictionary(t=>t.Key,t=>t.ToList());
                    oCResult.oResult = IndividualKpi;
                //}
                //else 
                //{
                //    oCR.sMessage = "Config Error: XIInfraKPIGroupingComponent_XILoad() : Role ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                //    oCR.sCode = "Config Error";
                //    //SaveErrortoDB(oCR);
                //}
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing KPI Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                //throw ex;
            }
            return oCResult;
        }
    }
}