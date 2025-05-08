using iText.Kernel.Pdf.Tagging;
using iTextSharp.tool.xml.html;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using XICore;
using XIDatabase;
using XISystem;
using static iTextSharp.text.pdf.AcroFields;

namespace XICore
{

    public class XIRuleGroup {
        public int ID { get; set; }
        public int FKiRuleID { get; set; }
        public int iGroupID { get; set; }
        public int iRuleNo { get; set; }
        public string sRuleOperator { get; set; }
        public string FKiOperator { get; set; }
        public string FKiWhereValue { get; set; }
        public int iParentRootID { get; set; }
        public bool XIDeleted { get; set; }
        public Guid XIGUID { get; set; }
        public Guid? BOAttributeIDXIGUID { get; set; }
        public Guid? FKiRuleIDXIGUID { get; set; }
        public Guid? BOIDXIGUID { get; set; }
        public Guid? QSFieldOriginIDXIGUID { get; set; }
        public List<XIRuleGroup> oChildGroups { get; set; }
    }

    public class XIRulesHTMLComponentInput
    {
        public List<XIRuleGroup> oRuleGroup { get; set; }
        public XIDBO oBOD { get; set; }
        public string sGroupName { get; set; }
        public string sMode { get; set; }
    }
    public class XIInfraRulesComponent : XIDefinitionBase
    {
        XIInfraCache oCache = new XIInfraCache();
        public string sOrgName { get; set; }
        public string sCode { get; set; }
        public int iBODID;
        public string sMode { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }


        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        XIDStructure oXIStruct = new XIDStructure();
        List<XIDStructure> oTree = new List<XIDStructure>();
        int iNodeID = 0;

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
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
                XIInfraCache oCache = new XIInfraCache();
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = oParams.Where(m => m.sName.ToLower() == "boiid").Select(m => m.sValue).FirstOrDefault();
                string BODID = oParams.Where(m => m.sName.ToLower() == "bodid").Select(m => m.sValue).FirstOrDefault(); // RuleIDXIGUID
                //string BODID = "4173D43E-16A0-4779-A368-F54FB9405706";
                string sCurrentsGUID = string.Empty;
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sCurrentsGUID = sGUID;
                    sGUID = ParentGUID;
                }
                int iBODID = 0;
                int.TryParse(BODID, out iBODID);
                XIIXI oXII = new XIIXI();
                XIIBO oBO = new XIIBO();
                XIDBO oBOD = null;
                var oBOIList = new List<XIIBO>();



                oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|RuleDetailsID}", BODID, null, null);
                var sRuleID = "";
                List<XIRuleGroup> oRuleGroup = LoadRuleGroups(BODID, 0);
                sRuleID = oRuleGroup.FirstOrDefault().FKiRuleID.ToString();
                var RuleI = oXII.BOI("XI Rules", BODID);
                int iGroupID = 0;
                string sGroupName = string.Empty;
                string sMode = String.Empty;

                if (RuleI != null && RuleI.Attributes.Count > 0 && RuleI.Attributes.ContainsKey("imode"))
                {
                    sMode = RuleI.Attributes["imode"].sValue == "30" ? "qs" : "object";
                    oCache.Set_ParamVal(sSessionID, sGUID, "", "sRuleMode", RuleI.Attributes["imode"].sValue, "", null);
                    oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "sRuleMode", RuleI.Attributes["imode"].sValue, "", null);
                    oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "{XIP|iGroupID}", RuleI.Attributes["imode"].iValue == 30 ? "29077" : "18598", "", null);
                    if (RuleI.Attributes["imode"].iValue == 30)
                    {
                        iGroupID = 29077;
                        oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "{XIP|QSDefinitionIDXIGUID}", RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue, "", null);
                        oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|QSDefinitionIDXIGUID}", RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue, "", null);
                    }
                    else
                    {
                        iGroupID = 18598;
                    }
                    oBOD = LoadRuleGroupAttributes(iGroupID, RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue);
                    sGroupName = oBOD.Groups.Where(g => g.Value.ID == iGroupID).Select(x => x.Value.GroupName).SingleOrDefault();
                }
                oCResult.oResult = new XIRulesHTMLComponentInput { oRuleGroup = oRuleGroup, oBOD = oBOD, sGroupName = sGroupName, sMode  =  sMode};
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tree Structure Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }

            return oCResult;
        }
        public CResult XILoad_Old(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
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
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = oParams.Where(m => m.sName.ToLower() == "boiid").Select(m => m.sValue).FirstOrDefault();
                string BODID = oParams.Where(m => m.sName.ToLower() == "bodid").Select(m => m.sValue).FirstOrDefault();
                string sCurrentsGUID = string.Empty;
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sCurrentsGUID = sGUID;
                    sGUID = ParentGUID;
                }
                int iBODID = 0;
                int.TryParse(BODID, out iBODID);
                //oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|RuleBOID}", BODID, "", null);
                //oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|BOID}", BODID, "", null);
                //oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|iParentID}", BODID, "", null);
                XIIXI oXII = new XIIXI();
                //XIDBO oBOD = new XIDBO();
                //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", BODID);
                XIIBO oBO = new XIIBO();
                //List<CNV> WhrParams = new List<CNV>();
                //WhrParams.Add(new CNV { sName = "BOID", sValue = BODID });
                //oXII.BOI("XI Rule Groups", "", "", WhrParams);
                var oBOIList = new List<XIIBO>();
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                XIWhereParams oWP = new XIWhereParams();
                oWP.sField = "FKiRuleIDXIGUID";
                oWP.sOperator = "=";
                oWP.sValue = BODID;
                oWParams.Add(oWP);
                oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|RuleDetailsID}", BODID, null, null);
                oWParams.Add(new XIWhereParams { sField = "izxDeleted" , sOperator = "=" , sValue = "0" });
                List<SqlParameter> SqlParams = new List<SqlParameter>();
                SqlParams.Add(new SqlParameter { ParameterName = "@FKiRuleIDXIGUID", Value = BODID });
                SqlParams.Add(new SqlParameter { ParameterName = "@izxDeleted", Value = "0" });
                //load requirement template definition of productid and FKiTransactionTypeID
                oQE.AddBO("XI Rule Groups", "", oWParams);
                CResult oCresult = oQE.BuildQuery();
                List<CNV> oNV = new List<CNV>();
                var boid = "";
                var QSGUID = "";
                var sRuleID = "";
                //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query build successfully" });
                if (oCresult.bOK && oCresult.oResult != null)
                {
                    var sSql = (string)oCresult.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    oEE.SqlParams = SqlParams;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query executed successfully" });
                        oBOIList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        foreach(var oBOI in oBOIList)
                        {
                            if(oBOI.Attributes.ContainsKey("sruleoperator"))
                            {
                                oNV.Add(new CNV { sName = oBOI.Attributes["sruleoperator"].sValue, sValue = oBOI.Attributes["id"].sValue, sType= oBOI.Attributes["iparentrootid"].sValue });
                            }
                            if(oBOI.Attributes.ContainsKey("boid") && !string.IsNullOrEmpty(oBOI.Attributes["boid"].sValue))
                            {
                                boid = oBOI.Attributes["boid"].sValue;
                            }
                            if (oBOI.Attributes.ContainsKey("fkiruleid") && !string.IsNullOrEmpty(oBOI.Attributes["fkiruleid"].sValue))
                            {
                                sRuleID = oBOI.Attributes["fkiruleid"].sValue;
                            }
                        }
                    }
                }

                var RuleI = oXII.BOI("XI Rules", sRuleID);
                if (RuleI != null && RuleI.Attributes.Count > 0 && RuleI.Attributes.ContainsKey("imode"))
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, "", "sRuleMode", RuleI.Attributes["imode"].sValue, "", null);
                    oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "sRuleMode", RuleI.Attributes["imode"].sValue, "", null);
                    oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "{XIP|iGroupID}", RuleI.Attributes["imode"].iValue == 30 ? "29077" : "18598", "", null);
                    if(RuleI.Attributes["imode"].iValue == 30)
                    {
                        oCache.Set_ParamVal(sSessionID, sCurrentsGUID, "", "{XIP|QSDefinitionIDXIGUID}", RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue, "", null);
                        oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|QSDefinitionIDXIGUID}", RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue, "", null);
                    }
                }
                if (RuleI != null && RuleI.Attributes.Count > 0 && RuleI.Attributes.ContainsKey("sstructureCode"))
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, "", "sRuleStructureCode", RuleI.Attributes["sstructureCode"].sValue, "", null);
                }
                if (RuleI.Attributes.ContainsKey("fkiqsdefinitionidxiguid") && !string.IsNullOrEmpty(RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue))
                {
                    QSGUID = RuleI.Attributes["fkiqsdefinitionidxiguid"].sValue;
                }
                oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|iParentBOID}", boid, "", null);
                oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|iParentQSID}", QSGUID, "", null);
                oCache.Set_ParamVal(sSessionID, sGUID, "", "boid", boid, "autoset", null);
                //oBO.iBODID = iBODID;
                oCResult.oResult = oNV;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tree Structure Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }

            return oCResult;
        }
        public List<XIDStructure> ClearParams(List<XIDStructure> oStruct, string sParent)
        {
            if (sParent == "#")
            {
                var Parent = oStruct.Where(m => m.FKiParentID.ToLower() == "#").FirstOrDefault();
                if (Parent.sName.Contains('('))
                {
                    oStruct.Where(m => m.FKiParentID.ToLower() == "#").FirstOrDefault().sName = Parent.sName.Substring(0, Parent.sName.IndexOf('(')).Trim();
                    oStruct.Where(m => m.FKiParentID.ToLower() == "#").FirstOrDefault().sInsID = "";
                }
            }
            var Childs = oStruct.Where(m => m.FKiParentID.ToLower() == sParent.ToString()).ToList();
            foreach (var items in Childs)
            {
                var subChilds = oStruct.Where(m => m.FKiParentID.ToLower() == items.ID.ToString()).ToList();
                if (subChilds != null && subChilds.Count() > 0)
                {
                    if (items.sName.Contains('('))
                    {
                        items.sName = items.sName.Substring(0, items.sName.IndexOf('(')).Trim();
                        items.sInsID = "";
                    }
                    ClearParams(oStruct, items.ID.ToString());
                }
                else
                {
                    if (items.sName.Contains('('))
                    {
                        items.sName = items.sName.Substring(0, items.sName.IndexOf('(')).Trim();
                        items.sInsID = "";
                    }
                }
            }
            return oStruct;
        }
        public CResult Build_ScriptTree(List<CCodeLine> oScript, int iParentID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name; ;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                foreach (var items in oScript)
                {
                    oTrace.oParams.Add(new CNV { sName = "Script", sValue = items.sLine });
                    Random generator = new Random();
                    iNodeID++;
                    if (string.IsNullOrEmpty(items.sOperator))
                    {
                        if (iNodeID == 1)
                        {
                            oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = "#", sName = "XIScript" });
                        }
                        else
                        {
                            if (items.sLine.Contains(':'))
                            {
                                var values = items.sLine.Split(':');
                                for (int i = 0; i < values.Length; i++)
                                {
                                    if (values[i] != "-")
                                    {
                                        oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = iParentID.ToString(), sName = values[i] });
                                        iParentID = iNodeID;
                                        if (i + 1 < values.Length)
                                        {
                                            iNodeID++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = iParentID.ToString(), sName = items.sLine });
                                if (iNodeID == 2)
                                    iParentID = iNodeID;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(items.sOperator) && (items.sOperator == "ag" || items.sOperator == "m"))
                        {
                            if (iNodeID == 1)
                            {
                                oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = "#", sName = items.sOperator == "ag" ? "Algorithm" : "Method" });
                            }
                        }
                        else
                        {
                            oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = iParentID.ToString(), sName = items.sOperator });
                        }
                    }


                    if (items.NCodeLines.Count() > 0)
                    {
                        oCR = Build_ScriptTree(items.NCodeLines.Values.ToList(), iNodeID);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            if (oTrace.iStatus != (int)xiEnumSystem.xiFuncResult.xiError)
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCResult.oResult = "Success";
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                }

                // oTree.Where(x => x.ID == iNodeID).First().FKiParentID = (iNodeID - 2).ToString();
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        private List<XIRuleGroup> LoadRuleGroups(string sRuleIDXIGUID, int iParentID)
        {
            List<XIIBO> oBOIList = new List<XIIBO>();
            List<XIRuleGroup> oRuleGroup = new List<XIRuleGroup>();
            QueryEngine oQE = new QueryEngine();
            List<XIWhereParams> oWParams = new List<XIWhereParams>();
            List<SqlParameter> SqlParams = new List<SqlParameter>();
            oWParams.Add(new XIWhereParams { sField = "XIDeleted", sOperator = "=", sValue = "0" });
            if (!string.IsNullOrEmpty(sRuleIDXIGUID)) {
                oWParams.Add(new XIWhereParams { sField = "FKiRuleIDXIGUID", sOperator = "=", sValue = sRuleIDXIGUID });
                SqlParams.Add(new SqlParameter { ParameterName = "@FKiRuleIDXIGUID", Value = sRuleIDXIGUID });
            }
            oWParams.Add(new XIWhereParams { sField = "iParentRootID", sOperator = "=", sValue = iParentID.ToString() });

            SqlParams.Add(new SqlParameter { ParameterName = "@iParentRootID", Value = iParentID.ToString() });
            SqlParams.Add(new SqlParameter { ParameterName = "@XIDeleted", Value = "0" });

            oQE.AddBO("XI Rule Groups", "", oWParams);
            CResult oCresult = oQE.BuildQuery();
            if (oCresult.bOK && oCresult.oResult != null)
            {
                var sSql = (string)oCresult.oResult;
                ExecutionEngine oEE = new ExecutionEngine();
                oEE.XIDataSource = oQE.XIDataSource;
                oEE.sSQL = sSql;
                oEE.SqlParams = SqlParams;
                var oQResult = oEE.Execute();
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oRuleGroup = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList().Select(x => new XIRuleGroup
                    {
                        ID = x.Attributes["id"].iValue,
                        FKiRuleID = x.Attributes["fkiruleid"].iValue,
                        iGroupID = x.Attributes["igroupid"].iValue,
                        iRuleNo = x.Attributes["iruleno"].iValue,
                        sRuleOperator = x.Attributes["sruleoperator"].sValue,
                        FKiOperator = x.Attributes["fkioperator"].sValue,
                        FKiWhereValue = x.Attributes["fkiwherevalue"].sValue,
                        iParentRootID = x.Attributes["iparentrootid"].iValue,
                        XIDeleted = x.Attributes["xideleted"].bValue,
                        XIGUID = new Guid(x.Attributes["xiguid"].sValue),
                        BOAttributeIDXIGUID = x.Attributes["boattributeidxiguid"].sValue == "" ? null : (Guid?)new Guid(x.Attributes["boattributeidxiguid"].sValue),
                        FKiRuleIDXIGUID = x.Attributes["fkiruleidxiguid"].sValue == "" ? null : (Guid?)new Guid(x.Attributes["fkiruleidxiguid"].sValue),
                        BOIDXIGUID = x.Attributes["boidxiguid"].sValue == "" ? null : (Guid?)new Guid(x.Attributes["boidxiguid"].sValue),
                        QSFieldOriginIDXIGUID = x.Attributes["qsfieldoriginidxiguid"].sValue == "" ? null : (Guid?)new Guid(x.Attributes["qsfieldoriginidxiguid"].sValue),
                    }).ToList();
                    foreach (XIRuleGroup g in oRuleGroup)
                    {
                        g.oChildGroups = LoadRuleGroups("", g.ID);
                    }
                }
            }
            return oRuleGroup;
        }
        private XIDBO LoadRuleGroupAttributes(int iGroupID, string sQSDefinitionIDXIGUID)
        {
            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "XI Rule Groups");
            if (iGroupID == 29077)
            {
                Guid oneClickGUID = oBOD.Attributes.Where(a => a.Value.Name.ToLower() == "QSFieldOriginIDXIGUID".ToLower()).Select(x => x.Value.iOneClickIDXIGUID).SingleOrDefault();
                XIDXI oXIDXI = new XIDXI();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "{XIP|QSDefinitionIDXIGUID}", sValue = sQSDefinitionIDXIGUID });
                var oResult = oXIDXI.Get_AutoCompleteList("1click_" + oneClickGUID.ToString(), oBOD.Name, oParams);
                List<XIDropDown> FKDDL = new List<XIDropDown>();
                if (oResult.bOK && oResult.oResult != null)
                {
                    var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                    FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                }
                oBOD.Attributes.Where(a => a.Value.Name.ToLower() == "QSFieldOriginIDXIGUID".ToLower()).SingleOrDefault().Value.FieldDDL = FKDDL;
            }
            string sHiddenGroup = "HideGroup";
            List<string> oAttributes = oBOD.Groups.Where(x => x.Value.ID == iGroupID).Select(x => x.Value.BOFieldNames).SingleOrDefault().Split(',').ToList().Select(x => x.Trim()).ToList();
            oBOD.Attributes = oBOD.Attributes.Where(att => oAttributes.Contains(att.Value.Name)).ToDictionary(x => x.Key, x => x.Value);
            List<string> oHiddenAttributes = oBOD.Groups.Where(x => x.Value.GroupName == sHiddenGroup).Select(x => x.Value.BOFieldNames).SingleOrDefault().Split(',').ToList().Select(x => x.Trim()).ToList();
            foreach (var item in oBOD.Attributes)
            {
                if (oHiddenAttributes.Contains(item.Value.Name))
                {
                    item.Value.bIsHidden = true;
                }
            }
            return oBOD;
        }
    }
}