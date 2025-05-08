using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XICore;
using XISystem;

namespace XICore
{
    public class XIInfraTreeStructureComponent : XIDefinitionBase
    {
        XIInfraCache oCache = new XIInfraCache();
        public string sOrgName { get; set; }
        public string sCode { get; set; }
        public int iBODID;
        public string sMode { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }

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
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            List<XIDStructure> oStruct = new List<XIDStructure>();
            try
            {
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = oParams.Where(m => m.sName.ToLower() == "iinstanceid").Select(m => m.sValue).FirstOrDefault();
                string BODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                //if (!string.IsNullOrEmpty(sGUID))
                //{
                //    oCache.sSessionID = sSessionID;
                //    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                //    sGUID = ParentGUID;
                //}
                var WrapperParms = new List<CNV>();
                var TreeParams = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
                if (WatchParam.Count() > 0)
                {
                    foreach (var items in WatchParam)
                    {
                        if (!string.IsNullOrEmpty(items.sValue))
                        {
                            var Prams = oCache.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                            if (Prams != null)
                            {
                                WrapperParms = Prams.nSubParams;
                            }
                        }
                    }
                }
                string ParentID = string.Empty;
                string BuildingID = string.Empty;
                string sSearchText = string.Empty;
                string sFolderName = string.Empty;
                string sLoadType = string.Empty;
                string sFilterType = string.Empty;
                string sBODID = oParams.Where(m => m.sName == XIConstant.Param_BODID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sBODID, out iBODID);
                Guid BOIDXIGUID = Guid.Empty;
                Guid.TryParse(sBODID, out BOIDXIGUID);
                sCode = oParams.Where(m => m.sName == XIConstant.Param_Code).Select(m => m.sValue).FirstOrDefault();
                sMode = oParams.Where(m => m.sName == XIConstant.Param_Mode).Select(m => m.sValue).FirstOrDefault();
                sSearchText = oParams.Where(m => m.sName == XIConstant.Param_SearchText).Select(m => m.sValue).FirstOrDefault();
                BuildingID = oParams.Where(m => m.sName == "BuildingID").Select(m => m.sValue).FirstOrDefault();
                sLoadType = oParams.Where(m => m.sName == XIConstant.Param_LoadType).Select(m => m.sValue).FirstOrDefault();

                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    ParentID = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentID).Select(m => m.sValue).FirstOrDefault();
                    BuildingID = WrapperParms.Where(m => m.sName == "BuildingID").Select(m => m.sValue).FirstOrDefault();
                    sFolderName = WrapperParms.Where(m => m.sName == "FolderName").Select(m => m.sValue).FirstOrDefault();
                    sFilterType = WrapperParms.Where(m => m.sName == "sFilterType").Select(m => m.sValue).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(ParentID))
                {
                    ParentID = oParams.Where(m => m.sName == XIConstant.Param_ParentID).Select(m => m.sValue).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(BuildingID))
                {
                    BuildingID = oParams.Where(m => m.sName == "BuildingID").Select(m => m.sValue).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(sFolderName))
                {
                    sFolderName = oParams.Where(m => m.sName == "FolderName").Select(m => m.sValue).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(sMode) && (sMode.ToLower() == "instancetree" || sMode.ToLower() == "instancetreefilter"))
                {
                    oStruct = new List<XIDStructure>();
                    if (!string.IsNullOrEmpty(sLoadType) && sLoadType.ToLower() == "onload")
                    {
                        oStruct.Add(new XIDStructure { sContext = sMode, sCode = sCode, BOID = iBODID, sType = sLoadType, FKiParentID = ParentID, sName = sFolderName, ID = Convert.ToInt32(BuildingID), sLinkingType = sFilterType });
                        oCResult.oResult = oStruct;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(sFolderName) || !string.IsNullOrEmpty(ParentID))
                        {
                            XIDStructure oStr = new XIDStructure();
                            var oResult = oStr.Get_SelfStructure(sFolderName, ParentID, iBODID, sSearchText, BuildingID, sMode);
                            Dictionary<string, List<XIIBO>> Data = new Dictionary<string, List<XIIBO>>();
                            if (oResult.oResult != null)
                            {
                                Data["XI Menu"] = ((Dictionary<string, XIIBO>)oResult.oResult).Values.ToList();
                            }
                            else
                            {
                                Data["XI Menu"] = new List<XIIBO>();
                            }
                            oStr.oStructureInstance = Data;
                            oStr.sMode = "instancetree";
                            oStr.sContext = "instancetree";
                            oStr.FKiParentID = ParentID;
                            oStr.sSearchText = sSearchText;
                            oStr.BOID = iBODID;
                            List<XIDStructure> oStrList = new List<XIDStructure>();
                            oStrList.Add(oStr);
                            oCResult.oResult = oStrList;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(sMode) && (sMode.ToLower() == "datatree"))
                {
                    var oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, BOIDXIGUID.ToString());
                    if (oStructD != null && oStructD.Count() > 0)
                    {
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID.ToString());
                        string sNameAttribute = oBOD.sNameAttribute;
                        if (!string.IsNullOrEmpty(sNameAttribute))
                        {
                            sNameAttribute = "sName";
                        }
                        var o1ClickXIGUID = oStructD.FirstOrDefault().FKi1ClickIDXIGUID;
                        if (o1ClickXIGUID != null && o1ClickXIGUID != Guid.Empty)
                        {
                            XID1Click o1Click = new XID1Click();
                            XID1Click o1ClickC = new XID1Click();
                            XIDStructure oStructure = new XIDStructure();
                            o1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, o1ClickXIGUID.ToString());
                            o1ClickC = (XID1Click)o1Click.Clone(o1Click);
                            XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                            List<CNV> nParms = new List<CNV>();
                            nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                            o1ClickC.ReplaceFKExpressions(nParms);
                            o1ClickC.Query = oStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                            var oResult = o1ClickC.OneClick_Run();
                            if (oResult != null && oResult.Count() > 0)
                            {
                                List<XIDStructure> oNVs = new List<XIDStructure>();
                                foreach (var boi in oResult.Values.ToList())
                                {
                                    var ID = boi.AttributeI("id").sValue;
                                    var sName = boi.AttributeI(sNameAttribute).sValue;
                                    Guid XIGUID = Guid.Empty;
                                    var GUID = boi.AttributeI("xiguid").sValue;
                                    Guid.TryParse(GUID, out XIGUID);
                                    oNVs.Add(new XIDStructure { sName = sName, ID = Convert.ToInt32(ID), XIGUID = oStructD.FirstOrDefault().XIGUID, FKiStepDefinitionIDXIGUID = oStructD.FirstOrDefault().FKiStepDefinitionIDXIGUID, sBO = oStructD.FirstOrDefault().sBO, sOutputArea = oStructD.FirstOrDefault().sOutputArea, sMode = oStructD.FirstOrDefault().sMode, sInsID = XIGUID.ToString(), sCSS = oStructD.FirstOrDefault().sCSS, FKi1ClickIDXIGUID=oStructD.FirstOrDefault().FKi1ClickIDXIGUID });
                                }
                                oNVs.FirstOrDefault().sContext = sMode;
                                oNVs.FirstOrDefault().sCode = sCode;
                                oNVs.FirstOrDefault().BOIDXIGUID = BOIDXIGUID;
                                oNVs.FirstOrDefault().sType = sLoadType;
                                oCResult.oResult = oNVs;
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(sMode) && sMode.ToLower() == "folder")
                {
                    XIInfraUsers oUser = new XIInfraUsers();
                    Dictionary<string, XIIBO> oResult1 = new Dictionary<string, XIIBO>();
                    List<XIIBO> BOIs = new List<XIIBO>();
                    var UserID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_UserID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    var Database = oParams.Where(m => m.sName.ToLower() == "scoredatabase").Select(m => m.sValue).FirstOrDefault();
                    var oUserI = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString(), Convert.ToInt32(UserID)).oResult;
                    if (oUserI.Role.sRoleName.ToLower() == "createifadmin")
                    {
                        XID1Click oD1Click = new XID1Click();
                        string sQuery = string.Empty;
                        sQuery = "select * from xidocumenttree_t WHERE bproject=1 and spageno=1 and " + XIConstant.Key_XIDeleted + " = 0 order by rOrder asc";
                        oD1Click.Query = sQuery;
                        oD1Click.Name = "xidocumenttree";
                        oResult1 = oD1Click.OneClick_Run(false);
                        foreach (var items in oResult1.Values)
                        {
                            var ID = items.Attributes["id"].sValue;
                            XID1Click oC1Click = new XID1Click();
                            string sCQuery = string.Empty;
                            sCQuery = "select * from xidocumenttree_t WHERE sParentID ='" + ID + "' and spageno=1 and " + XIConstant.Key_XIDeleted + " = 0 order by rOrder asc";
                            oC1Click.Query = sCQuery;
                            oC1Click.Name = "xidocumenttree";
                            var oCRes = oC1Click.OneClick_Run(false);
                            if (oCRes != null && oCRes.Values.Count() > 0)
                            {
                                items.bHasChilds = true;
                            }
                            items.Attributes.Values.ToList().Where(m => m.sName.ToLower() == "sparentid").FirstOrDefault().sValue = "#";
                            //items.AttributeI("sparentid").sValue = "#";
                        }
                    }
                    else
                    {
                        XIIXI oXI = new XIIXI();
                        List<CNV> WHrParams = new List<CNV>();
                        WHrParams.Add(new CNV { sName = "fkiuserid", sValue = UserID });
                        var oUP = oXI.BOI("userprojects", null, null, WHrParams);
                        if (oUP != null && oUP.Attributes.Count() > 0)
                        {
                            var sProject = oUP.AttributeI("sproject").sValue;
                            var ProjID = sProject.Split(',')[0];
                            var BOI = oXI.BOI("Project", ProjID);
                            if (BOI != null && BOI.Attributes.Count() > 0)
                            {
                                var ProName = BOI.AttributeI("fkinodeid").sValue;
                                XID1Click oD1Click = new XID1Click();
                                string sQuery = string.Empty;
                                sQuery = "select * from xidocumenttree_t WHERE id ='" + ProName + "' and spageno=1 and " + XIConstant.Key_XIDeleted + " = 0 order by rOrder asc";
                                oD1Click.Query = sQuery;
                                oD1Click.Name = "xidocumenttree";
                                oResult1 = oD1Click.OneClick_Run(false);
                                foreach (var items in oResult1.Values)
                                {
                                    var ID = items.Attributes["id"].sValue;
                                    XID1Click oC1Click = new XID1Click();
                                    string sCQuery = string.Empty;
                                    sCQuery = "select * from xidocumenttree_t WHERE sParentID ='" + ID + "' and spageno=1 and " + XIConstant.Key_XIDeleted + " = 0 order by rOrder asc";
                                    oC1Click.Query = sCQuery;
                                    oC1Click.Name = "xidocumenttree";
                                    var oCRes = oC1Click.OneClick_Run(false);
                                    if (oCRes != null && oCRes.Values.Count() > 0)
                                    {
                                        items.bHasChilds = true;
                                    }
                                    items.Attributes.Values.ToList().Where(m => m.sName.ToLower() == "sparentid").FirstOrDefault().sValue = "#";
                                    //items.AttributeI("sparentid").sValue = "#";
                                }
                            }
                        }
                    }

                    ParentID = "0";
                    BuildingID = "1";
                    XIDStructure oStr = new XIDStructure();
                    var oResult = oStr.Get_SelfStructure(sFolderName, ParentID, iBODID, sSearchText, BuildingID, "folder");
                    Dictionary<string, List<XIIBO>> Data = new Dictionary<string, List<XIIBO>>();
                    // Data["XI Menu1"] = BOIs; // ((Dictionary<string, XIIBO>)oResult.oResult).Values.ToList();
                    var RD = ((Dictionary<string, XIIBO>)oResult.oResult).Values.ToList();
                    Data["XI Menu"] = oResult1.Values.ToList();
                    oStr.oStructureInstance = Data;
                    oStr.sMode = "folder";
                    oStr.sContext = "folder";
                    oStr.FKiParentID = ParentID;
                    oStr.sSearchText = sSearchText;
                    oStr.BOID = iBODID;
                    List<XIDStructure> oStrList = new List<XIDStructure>();
                    oStrList.Add(oStr);
                    oCResult.oResult = oStrList;
                }
                else if (sMode.ToLower() == "script")
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, BODID);
                    string sScript = string.Empty;
                    if (sInstanceID != "0")
                    {
                        string sQuery = "select sScript from " + oBOD.TableName + " where id=" + sInstanceID;
                        XID1Click oXI1Click = new XID1Click();
                        oXI1Click.Query = sQuery;
                        oXI1Click.Name = oBOD.Name;
                        var oCRes = oXI1Click.OneClick_Run(false);
                        foreach (var items in oCRes.Values)
                        {
                            sScript = items.Attributes["sScript"].sValue;
                        }
                        if (oBOD.TableName == "XIAlgorithmLines_T")
                        {
                            sScript = sScript.Contains('{') && sScript.StartsWith("xi.s") ? sScript.Replace("{", "").Replace("}", "") : sScript;
                        }
                    }
                    //var sScript = "xi.s|{if|{eq|{xi.a|'ACJournalEntry_T',{xi.p|-iInstanceID},'FKiTransactionID','','iReconciled'},10},'Yes','No'}";
                    CScriptController oXIScript = new CScriptController();
                    //CCompiler oCCompiler = new CCompiler();
                    oCache = new XIInfraCache();
                    oCR = new CResult();
                    var oCacheObj = oCache.GetFromCache(sScript);
                    if (oCacheObj == null)
                    {
                        //oCR = oCCompiler.Compile_FromText(sScript);
                        oCR = oXIScript.API2_Serialise_From_String(sScript, oBOD.TableName == "XIAlgorithmLines_T" ? "algorithm" : null);
                        oCache.InsertIntoCache(oXIScript, sScript);
                    }
                    else
                    {
                        oXIScript = (CScriptController)oCacheObj;
                    }
                    List<CCodeLine> oScript = new List<CCodeLine>();
                    oScript.Add(oXIScript.oTopLine);
                    //oTree.Add(new XIDStructure { ID = iNodeID, FKiParentID = "#", sName = "XIScript", sContext = "script" });
                    oCR = Build_ScriptTree(oScript, iNodeID);
                    oTree.FirstOrDefault().sContext = "script";
                    oCResult.oResult = oTree;
                    oCResult.sScript = sScript;
                }
                else
                {
                    if (iBODID > 0 || (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty))
                    {
                        var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_InstanceID);
                        //var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBOIID");// oParams.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                        XIDXI oXID = new XIDXI();
                        var oTree = oCache.Get_ObjectSetCache("Tree_" + sGUID, sGUID, sSessionID);
                        if (oTree != null)
                        {
                            oStruct = (List<XIDStructure>)oTree;
                        }
                        if (oTree == null)
                        {
                            if (!string.IsNullOrEmpty(sLoadType) && sLoadType.ToLower() == "onload")
                            {
                                oStruct.Add(new XIDStructure { sContext = sMode, sCode = sCode, BOID = iBODID, BOIDXIGUID = BOIDXIGUID, sType = sLoadType });
                            }
                            else
                            {
                                List<XIDStructure> oStructD = new List<XIDStructure>();
                                if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                                {
                                   oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, BOIDXIGUID.ToString());
                                }
                                else
                                {
                                   oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, iBODID.ToString());
                                }
                                oStruct = (List<XIDStructure>)oXID.Clone(oStructD);
                                var iInsID = "";
                                foreach (var items in oStruct)
                                {
                                    if (items.sMode != null && items.sMode.ToLower() == "single")
                                    {
                                        var iID = oCache.Get_ParamVal(sSessionID, sGUID, null, items.sBO.ToLower() + ".id");//oParams.Where(m => m.sName.ToLower() == items.sName.ToLower() + ".id").Select(m => m.sValue).FirstOrDefault();
                                        if (!string.IsNullOrEmpty(iID))
                                        {
                                            //var oLIst = oIXI.BOI(items.sBO.ToLower(), iID).Structure(sCode).XILoad();
                                            items.sInsID = iID.ToString();
                                            iInsID= iID.ToString();
                                        }
                                        else
                                        {
                                             iInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|"+items.sBO.ToLower() + ".id}");
                                            items.sInsID = iInsID;
                                        }

                                    }
                                }

                                if (oStruct != null)
                                {
                                    if (oStruct.Count() > 0)
                                    {
                                        oStruct.ToList().ForEach(m => m.sContext = sMode);
                                    }
                                }
                                oCache.Set_ObjectSetCache(sSessionID, "Tree_" + sGUID, sGUID, oStruct);
                            }
                        }
                        //var Nodes = oXIStruct.GetXITreeStructure(iBODID, sCode);

                        if (WrapperParms == null || WrapperParms.Count() > 0)
                        {
                            var iAppID = 0;
                            string sBOName = string.Empty;
                            if (WrapperParms != null && WrapperParms.Count() > 0)
                            {
                                sBOName = WrapperParms.Where(m => m.sName == XIConstant.XIP_BOName).Select(m => m.sValue).FirstOrDefault();
                                iBOIID = WrapperParms.Where(m => m.sName == XIConstant.XIP_InstanceID).Select(m => m.sValue).FirstOrDefault();
                                if (!string.IsNullOrEmpty(sBOName) && !string.IsNullOrEmpty(iBOIID))
                                {
                                    oCache.Set_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_ActiveID, iBOIID, null, null);
                                }
                            }
                            else
                            {
                                var sApplicationID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|XI Application.id}");
                                if (int.TryParse(sApplicationID, out iAppID))
                                {
                                    if (iAppID != 0)
                                    {
                                        sBOName = "XI Application";
                                        oCache.Set_ObjectSetCache(sSessionID, sGUID, null, sBOName);
                                        oCache.Set_ObjectSetCache(sSessionID, sGUID, null, iAppID);
                                        iBOIID = iAppID.ToString();
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(sBOName) && !string.IsNullOrEmpty(iBOIID))
                            {
                                var BOD = (XIDBO)(oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, "0"));
                                var sNameAttribute = BOD.Get_BOAttributeValue(sBOName, Convert.ToInt32(iBOIID));
                                if (!string.IsNullOrEmpty(sNameAttribute))
                                {
                                    if (oStruct.Count() > 0)
                                    {
                                        var NodeName = oStruct.Where(m => m.sBO == sBOName).Select(m => m.sName).FirstOrDefault();
                                        if (!string.IsNullOrEmpty(NodeName))
                                        {
                                            if (NodeName.Contains('('))
                                            {
                                                var index = NodeName.IndexOf('(') - 2;
                                                NodeName = NodeName.Substring(0, index + 1);

                                            }
                                            oStruct.Where(m => m.sBO == sBOName).FirstOrDefault().sName = NodeName + " (" + sNameAttribute + ")";
                                            oStruct.Where(m => m.sBO == sBOName).FirstOrDefault().sInsID = iBOIID;
                                            oStruct.Where(m => m.sBO == sBOName).FirstOrDefault().sNameAttribute = sNameAttribute;
                                        }
                                    }
                                    oCache.Set_ObjectSetCache(sSessionID, "Tree_" + sGUID, sGUID, oStruct);
                                    oCache.Set_ParamVal(sSessionID, sGUID, null, "sBOName", sNameAttribute, null, null);
                                }
                            }
                            if (WrapperParms != null && WrapperParms.Count() > 0)
                            {
                                var ParamClear = WrapperParms.Where(m => m.sName == "ParamClear").Select(m => m.sValue).FirstOrDefault();
                                if (!string.IsNullOrEmpty(ParamClear) && ParamClear == "yes")
                                {
                                    var sBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                                    var Parent = oStruct.Where(m => m.sBO.ToLower() == sBO.ToLower()).FirstOrDefault();
                                    if (Parent != null)
                                    {
                                        oStruct = ClearParams(oStruct, Parent.FKiParentID.ToString());
                                        oCache.Set_ObjectSetCache(sSessionID, "Tree_" + sGUID, sGUID, oStruct);
                                    }
                                }
                            }
                        }
                        oCResult.oResult = oStruct;
                    }
                    else
                    {
                        oStruct = oStruct == null ? oStruct = new List<XIDStructure>() : oStruct;
                        oStruct.FirstOrDefault().sErrorMessage = "Mandatory parameters not passed to load the Tree component";
                        oCResult.oResult = oStruct;
                    }

                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oStruct = oStruct == null ? oStruct = new List<XIDStructure>() : oStruct;
                oStruct.FirstOrDefault().sErrorMessage = "Something went wrong while loading the Tree component";
                oCResult.oResult = oStruct;
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
    }
}