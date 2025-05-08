using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XISystem;
using xiEnumSystem;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    public class XIVisualisation
    {
        public int XiVisualID { get; set; }
        public Guid XIGUID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public List<XIVisualisationNV> XiVisualisationNVs { get; set; }
        public List<XIVisualisationList> XiVisualisationLists { get; set; }
        [NotMapped]
        public List<XIDropDown> XiVisualisationDDLs { get; set; }
        public int StatusTypeID { get; set; }
        public string[] NVPairs { get; set; }
        public string[] LNVPairs { get; set; }
        public List<XIVisualisationNV> NVs { get; set; }
        public List<XIVisualisationList> Lists { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        #region VisualisationList

        public CResult Get_VisualisationDef(int iVisualID)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            try
            {
                XIVisualisation oVi = new XIVisualisation();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (iVisualID > 0)
                {
                    Params["XiVisualID"] = iVisualID;
                    oVi = Connection.Select<XIVisualisation>("XiVisualisations", Params).FirstOrDefault();
                }
                XIVisualisationNV oVNVs = new XIVisualisationNV();
                Dictionary<string, object> oNVParam = new Dictionary<string, object>();
                oNVParam["XiVisualID"] = iVisualID;
                oVi.NVs = Connection.Select<XIVisualisationNV>("XiVisualisationNVs", oNVParam).ToList();
                XIVisualisationList oVList = new XIVisualisationList();
                Dictionary<string, object> oNVList = new Dictionary<string, object>();
                oNVList["XiVisualID"] = iVisualID;
                oVi.Lists = Connection.Select<XIVisualisationList>("XiVisualisationLists", oNVList).ToList();
                if (iVisualID == 0)
                {
                    oVi.FKiApplicationID = FKiApplicationID;
                }
                oCResult.oResult = oVi;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        #endregion VisualisationList
    }

    public class XIVisualisationNV
    {
        public int ID { get; set; }
        public int XiVisualID { get; set; }
        public Guid XiVisualIDXIGUID { get; set; }
        public int XiVisualListID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sType { get; set; }
    }

    public class XIVisualisationList
    {
        public int XiVisualListID { get; set; }
        public int XiVisualID { get; set; }
        public Guid XiVisualIDXIGUID { get; set; }
        public string ListName { get; set; }
        public virtual List<XIVisualisationNV> XiVisualisationListNVs { get; set; }
    }

    public static class XIGUIDConversion
    {
        /* HERE GUID CAN BE CONVERTED INTO ID (INT) */
        public static string GetInstanceIDByGUID(string sBO, string sGUIDValue, string Guidcolumn = "")
        {
            if (!string.IsNullOrEmpty(sBO) && !string.IsNullOrEmpty(sGUIDValue))
            {
                // Verify GUID or Not
                if (string.IsNullOrEmpty(Guidcolumn))
                    Guidcolumn = XIConstant.XiGUIDColumn;
                if (Utility.IsValidGUID(sGUIDValue))
                {
                    // Get Instanceid of BO
                    Dictionary<string, XIIBO> oXIBOI = new Dictionary<string, XIIBO>();
                    QueryEngine oQE = new QueryEngine();

                    //string sWhereCondition = $"{Guidcolumn}={sGUIDValue}";
                    string sWhereCondition = Guidcolumn + "=" + sGUIDValue;
                    var oQResult = oQE.Execute_QueryEngine(sBO, "id", sWhereCondition);
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                    }
                    if (oXIBOI.Count() > 0)
                    {
                        var BOValue = oXIBOI.Select(d => d.Value).FirstOrDefault();
                        if (BOValue != null)
                        {
                            string sreqvalue = BOValue.AttributeI("id").sValue;
                            if (!string.IsNullOrEmpty(sreqvalue))
                                return sreqvalue;
                        }
                    }
                }
            }
            return sGUIDValue;
        }

        public static List<CNV> GetXIGUIDList(List<CNV> oParams)
        {
            var sRegexp = new Regex(Regex.Escape("{XIP|") + "(.+?)" + Regex.Escape(".id}"));
            foreach (var item in oParams)
            {
                string BoName = sRegexp.Matches(item.sName).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().FirstOrDefault();
                if (!string.IsNullOrEmpty(BoName))
                {
                    string sGUID = item.sValue;
                    string sValue = GetInstanceIDByGUID(BoName, item.sValue);
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        item.sValue = sValue;
                        oParams.Where(d => d.sValue == sGUID).ToList().ForEach(r => r.sValue = sValue);
                    }
                }
            }
            return oParams;
        }

    }
}