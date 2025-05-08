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
using xiEnumSystem;

namespace XICore
{
    public class XIInfraOneClickComponent : XIDefinitionBase
    {

        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string OneClickID;
        public string sDisplayMode { get; set; }
        public string sStructureName { get; set; }
        public string sCondition { get; set; }
        public string sSearchText { get; set; }
        public string sSearchType { get; set; }
        public string Fields { get; set; }
        public string Optrs { get; set; }
        public string Values { get; set; }
        public string iUserID { get; set; }
        public string iRoleID { get; set; }
        public string iOrgID { get; set; }
        public string iAppID { get; set; }
        public string iUserLevel { get; set; }
        public string sParentFK { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        XIDStructure oXIDStructure = new XIDStructure();

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult XILoad(List<CNV> oParams, bool bIsLockStep = false)
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
            XID1Click o1ClickC = new XID1Click();
            try
            {
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName == XIConstant.Param_StrName).Select(m => m.sValue).FirstOrDefault();
                //var iTemplate = oParams.Where(m => m.sName == XIConstant.Param_sTemplate).Select(m => m.sValue).FirstOrDefault();
                sSearchText = oParams.Where(m => m.sName == XIConstant.Param_SearchText).Select(m => m.sValue).FirstOrDefault();
                sSearchType = oParams.Where(m => m.sName == XIConstant.Param_SearchType).Select(m => m.sValue).FirstOrDefault();
                Fields = oParams.Where(m => m.sName == "Fields").Select(m => m.sValue).FirstOrDefault();
                Optrs = oParams.Where(m => m.sName == "Optrs").Select(m => m.sValue).FirstOrDefault();
                Values = oParams.Where(m => m.sName == "Values").Select(m => m.sValue).FirstOrDefault();
                // var InsertParameter = oParams.Where(m => m.sName == "InsertParameter").Select(m => m.sValue).FirstOrDefault();
                iUserID = oParams.Where(m => m.sName == XIConstant.Param_UserID).Select(m => m.sValue).FirstOrDefault();
                iRoleID = oParams.Where(m => m.sName == XIConstant.Param_RoleID).Select(m => m.sValue).FirstOrDefault();
                iOrgID = oParams.Where(m => m.sName == XIConstant.Param_OrgID).Select(m => m.sValue).FirstOrDefault(); 
                if (string.IsNullOrEmpty(iOrgID))
                {
                    iOrgID = oParams.Where(m => m.sName.ToLower() == "iOrganizationID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                }
                iAppID = oParams.Where(m => m.sName == XIConstant.Param_ApplicationID).Select(m => m.sValue).FirstOrDefault();
                iUserLevel = oParams.Where(m => m.sName == XIConstant.Param_UserLevel).Select(m => m.sValue).FirstOrDefault();
                var sUIDRef = oParams.Where(m => m.sName == "sReference").Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var sIDRef = oParams.Where(m => m.sName == "sIDRef").Select(m => m.sValue).FirstOrDefault();
                Dictionary<string, string> IDEParams = new Dictionary<string, string>();
                //if (!string.IsNullOrEmpty(sGUID))
                //{
                //    oCache.sSessionID = sSessionID;
                //    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                //    sGUID = ParentGUID;
                //}
                sDisplayMode = oParams.Where(m => m.sName == XIConstant.Param_DisplayMode).Select(m => m.sValue).FirstOrDefault();
                var sTabOneclick = "";
                var WrapperParms = new List<CNV>();
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
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    OneClickID = WrapperParms.Where(m => m.sName == XIConstant.XIP_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    //if (int.TryParse(sOneClickID, out OneClickID))
                    //{

                    //}
                    //else
                    //{
                    //    OneClickID = 0;
                    //}
                    var Mode = WrapperParms.Where(m => m.sName == XIConstant.Param_Mode).Select(m => m.sValue).FirstOrDefault();
                    var iNodeVisualID = 0;
                    var NodeVisualIDXIGUID = Guid.Empty;
                    var NodeVisualID = WrapperParms.Where(m => m.sName == "{XIP|FKiVisualisationIDXIGUID}").Select(m => m.sValue).FirstOrDefault();
                    sTabOneclick = WrapperParms.Where(m => m.sName == "TabOneClickParam").Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(NodeVisualID, out iNodeVisualID);
                    Guid.TryParse(NodeVisualID, out NodeVisualIDXIGUID);
                    string sNodeFK = string.Empty;
                    string sNodeFKVal = string.Empty;
                    if (iNodeVisualID > 0 || (NodeVisualIDXIGUID != null && NodeVisualIDXIGUID != Guid.Empty))
                    {
                        var oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, null, NodeVisualIDXIGUID.ToString());
                        if (oXIvisual.XiVisualisationNVs != null && oXIvisual.XiVisualisationNVs.Count() > 0)
                        {
                            sNodeFK = oXIvisual.XiVisualisationNVs.Where(m => m.sName.ToLower() == "sparentfk").Select(m => m.sValue).FirstOrDefault();
                            sNodeFKVal = oXIvisual.XiVisualisationNVs.Where(m => m.sName.ToLower() == "sparentfkvalue").Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sNodeFK) && !string.IsNullOrEmpty(sNodeFKVal))
                            {
                                sNodeFKVal = oCache.Get_ParamVal(sSessionID, sGUID, null, sNodeFKVal);
                            }
                        }
                    }
                    sParentFK = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentFKColumn).Select(m => m.sValue).FirstOrDefault();
                    int iParentInsID = 0;
                    var sParentInsID = "";
                    if (!string.IsNullOrEmpty(sParentFK))
                    {
                        if (!string.IsNullOrEmpty(sNodeFK))
                        {
                            if (sNodeFK.ToLower() == sParentFK.ToLower())
                            {
                                sParentInsID = sNodeFKVal;
                            }
                        }
                        else
                        {
                            sParentInsID = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentInsID).Select(m => m.sValue).FirstOrDefault();

                        }
                        int.TryParse(sParentInsID, out iParentInsID);

                        if (!string.IsNullOrEmpty(sIDRef) && sIDRef == "xiguid")
                        {

                            var sBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sBO);
                            if (BOD != null && BOD.Attributes.Count > 0 && BOD.Attributes.ContainsKey(sParentFK))
                            {
                                var sFKBO = BOD.Attributes[sParentFK].sFKBOName;
                                if (!string.IsNullOrEmpty(sFKBO))
                                {
                                    var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sFKBO);
                                    if (FKBOD != null && FKBOD.bUID)
                                    {
                                        //sParentFK = sParentFK + "XIGUID";
                                    }
                                }
                                else if (BOD.bUID && iParentInsID == 0 && !string.IsNullOrEmpty(sParentInsID) && sParentInsID != "0")
                                {
                                    //sParentFK = "XIGUID";
                                }
                            }
                            else if (BOD.bUID && iParentInsID == 0 && !string.IsNullOrEmpty(sParentInsID) && sParentInsID != "0")
                            {
                                //sParentFK = "XIGUID";
                            }
                        }
                        if (!string.IsNullOrEmpty(sParentInsID) && sParentInsID != "0")
                        {
                            sCondition = sParentFK + "=" + sParentInsID;
                        }
                        else
                        {
                            sCondition = sParentFK + "=0";
                        }
                    }
                    var sAspectWhere = WrapperParms.Where(m => m.sName == XIConstant.Param_AspectWhere).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sAspectWhere))
                    {
                        if (!string.IsNullOrEmpty(sCondition))
                        {
                            sCondition = sCondition + " and " + sAspectWhere;
                        }
                        else
                        {
                            sCondition = sAspectWhere;
                        }
                    }

                    var sTreeGUID = WrapperParms.Where(m => m.sName == XIConstant.Param_TreeGUID).Select(m => m.sValue).FirstOrDefault();
                    var iNodeID = WrapperParms.Where(m => m.sName == XIConstant.Param_NodeID).Select(m => m.sValue).FirstOrDefault();
                    IDEParams[XIConstant.Param_Mode] = Mode;
                    IDEParams[XIConstant.Param_TreeGUID] = sTreeGUID;
                    IDEParams[XIConstant.Param_NodeID] = iNodeID;
                }
                //if (oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault() != null)
                //{
                //    if (WrapperParms != null && WrapperParms.Count() > 0)
                //    {
                //        OneClickID = Convert.ToInt32(WrapperParms.Where(m => m.sName == "i1ClickID").Select(m => m.sValue).FirstOrDefault());
                //    }
                //    else
                //    {
                //        OneClickID = Convert.ToInt32(oParams.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault());
                //    }
                //}
                else if (oParams.Where(m => m.sName == XIConstant.Param_1ClickID).FirstOrDefault() != null)
                {
                    OneClickID = oParams.Where(m => m.sName == XIConstant.Param_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(OneClickID) && (OneClickID.Contains("{XIP|1ClickID}") || OneClickID.Contains("{XIP|XI1Click.id}")))
                    {
                        XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        List<CNV> nParms = new List<CNV>();
                        nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        OneClickID = nParms.Where(m => m.sName == OneClickID).Select(m => m.sValue).FirstOrDefault();
                    }
                    //if (int.TryParse(sOneClickID, out OneClickID))
                    //{

                    //}
                    //else
                    //{
                    //    OneClickID = 0;
                    //}
                    var sParentFK = oParams.Where(m => m.sName == XIConstant.Param_ParentFKColumn).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentFK))
                    {
                        var sParentInsID = oParams.Where(m => m.sName == XIConstant.Param_ParentInsID).Select(m => m.sValue).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sParentInsID))
                        {
                            sCondition = sParentFK + "=" + sParentInsID;
                        }
                        else
                        {
                            //sCondition = sParentFK + "=-1";
                        }
                    }
                }
                else
                {
                    OneClickID = "";
                }
                XID1Click o1ClickD = new XID1Click();
                //Get BO Definition
                XIDBO oBOD = new XIDBO();
                if (!string.IsNullOrEmpty(OneClickID))
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID);
                    if (!string.IsNullOrEmpty(o1ClickD.sActionOneClickType))
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-iActionOneClickType}", o1ClickD.sActionOneClickType.ToString(), null, null);
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    if (o1ClickC.bMatrix)
                    {
                        if (string.IsNullOrEmpty(o1ClickC.sCode))
                        {
                            o1ClickC.sCode = xiEnumSystem.EnumMatrixAction.OneClick.ToString();
                        }
                        XIMatrix oXIMatrix = new XIMatrix();
                        oXIMatrix.MatrixAction(o1ClickC.sCode, xiEnumSystem.EnumMatrixAction.OneClick, "", 0, o1ClickD.XIGUID.ToString(), o1ClickD.Name, oParams);
                    }

                    List<string> Whrparams = new List<string>();
                    string sWhrclause = "";
                    var sActiveBo = oCache.Get_ParamVal(sSessionID, sGUID, "", "BO");
                    var ActiveBoD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sActiveBo);
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOID.ToString());
                    }
                    if (sTabOneclick == "yes" && ActiveBoD.bUID && !string.IsNullOrEmpty(sIDRef) && sIDRef == "xiguid")
                    {
                        o1ClickC.IsFkGUID = true;
                        o1ClickC.sParentBO = sActiveBo;
                        oCache.Set_ParamVal(sSessionID, sGUID, "", "{XIP|" + o1ClickD.ID + "_IsFKGUID}", "yes", "", null);
                    }
                    var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                    if (WhiteListCheck == "yes")
                    {
                        XIIBO oBOI = new XIIBO();
                        int RoleID = 0;
                        int.TryParse(iRoleID, out RoleID);
                        int OrgID = 0;
                        int.TryParse(iOrgID, out OrgID);
                        int AppID = 0;
                        int.TryParse(iAppID, out AppID);
                        int UserLevel = 0;
                        int.TryParse(iUserLevel, out UserLevel);
                        oCR = oBOI.Check_Whitelist(oBOD.BOID, RoleID, OrgID, AppID, "query", oBOD.iLevel, UserLevel);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            var bUNAuth = (bool)oCR.oResult;
                            if (bUNAuth)
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                return oCResult;
                            }
                        }
                    }
                    o1ClickC.BOD = oBOD;
                    o1ClickC.sBOName = oBOD.Name;
                    if (!string.IsNullOrEmpty(sParentFK) && (sParentFK.ToLower() == "OrganizationID".ToLower() || sParentFK.ToLower() == "OrganisationID".ToLower() || sParentFK.ToLower() == "FKiOrgID".ToLower() || sParentFK.ToLower() == "FKiOrgID".ToLower()))
                    {
                        if (oBOD.Attributes.ContainsKey("fkiapplicationid"))
                        {
                            sCondition = "fkiapplicationid=" + iAppID + " and " + sCondition;
                        }
                        else if (oBOD.Attributes.ContainsKey("fkiappid"))
                        {
                            sCondition = "fkiappid=" + iAppID + " and " + sCondition;
                        }
                    }
                    if (!string.IsNullOrEmpty(sCondition))
                    {
                        o1ClickC.sParentWhere = sCondition;
                    }
                    if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.ResultList.ToString()))
                    {
                        //if (oBOD.bUID || o1ClickC.bIsHidden)
                        //{
                            o1ClickC.oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = "XIGUID", sValue = "{XIP|XIGUID}" });
                            o1ClickC.oOneClickParameters.Add(new XID1ClickParameter { iType = 10, sName = oBOD.sPrimaryKey, sValue = "{XIP|iInstanceID}" });
                        //}
                    }
                    //Get Headings of 1-Click
                    o1ClickC.Get_1ClickHeadings();
                    if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()))
                    {
                        XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        List<CNV> nParms = new List<CNV>();
                        nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        o1ClickC.ReplaceFKExpressions(nParms);
                        o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                        //if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()))
                        //{
                        //    o1ClickC.bIsResolveFK = true;
                        //}      
                        if (!string.IsNullOrEmpty(Fields) && !string.IsNullOrEmpty(Optrs) && !string.IsNullOrEmpty(Values))
                        {
                            o1ClickC.Fields = Fields;
                            o1ClickC.Optrs = Optrs;
                            o1ClickC.Values = Values;
                            o1ClickC.SearchType = sSearchType;
                        }

                        string sHideSelect = string.Empty;
                        if (o1ClickC.oOneClickParameters.Where(x => x.iType == 10).Count() > 0)
                        {
                            if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()))
                            {
                                var ohideParams = o1ClickC.oOneClickParameters.Where(x => x.iType == 10).Select(m => m.sName).ToList();
                                if (ohideParams != null && ohideParams.Count() > 0)
                                {
                                    var sConcatStr = ohideParams.Select(f => "CONCAT('" + f + "','__'," + oBOD.TableName + "." + f + ")").ToList();
                                    if (ohideParams.Count() > 1)
                                    {
                                        sHideSelect = "CONCAT(" + string.Join(",':',", sConcatStr) + ") AS 'HiddenData'";
                                    }
                                    else
                                    {
                                        sHideSelect = sConcatStr.FirstOrDefault() + " AS 'HiddenData'";
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(sHideSelect))
                        {
                            o1ClickC.Query = o1ClickC.AddSelectPart(o1ClickC.Query, sHideSelect, "append");
                        }
                        Dictionary<string, XIIBO> oRes = new Dictionary<string, XIIBO>();
                        if (o1ClickC.IsStoredProcedure)
                        {
                            var SPParms = o1ClickC.oOneClickParameters.Where(x => x.iType == 30).ToList();
                            if (SPParms != null && SPParms.Count() > 0)
                            {
                                o1ClickC.NVs = o1ClickC.NVs ?? new List<CNV>();
                                foreach (var items in SPParms)
                                {
                                    var sResValue = nParms.Where(m => m.sName == items.sName).FirstOrDefault();
                                    if (sResValue != null)
                                    {
                                        o1ClickC.NVs.Add(new CNV { sName = items.sName, sValue = sResValue.sValue });
                                    }
                                }
                            }
                            o1ClickC.Query = o1ClickC.Name;
                            oRes = o1ClickC.OneClick_Run();
                            o1ClickC.oDataSet = oRes;
                        }
                        else
                        {
                            o1ClickC.ReplaceFKExpressions(oParams, true);
                            oRes = o1ClickC.OneClick_Execute(null, o1ClickC);
                        }
                        oRes.Values.ToList().ForEach(m => m.BOD = oBOD);
                        string sButtons = string.Empty;
                        string sCheckBox = string.Empty;
                        string sEditBtn = string.Empty;
                        string sCopyBtn = string.Empty;
                        string sDeleteBtn = string.Empty;
                        string sViewbtn = string.Empty;
                        string sAddBottombtn = string.Empty, sAddTopbtn = string.Empty;
                        string sOrderIncrementbtn = string.Empty, sOrderDecrementbtn = string.Empty, sCompilebtn = string.Empty, sAssignbtn = string.Empty;
                        //if (o1ClickC.bIsEdit)
                        //{
                        //    sEditBtn = "<input type='button' class='btn btn-theme' value='Edit' onclick ='fncEditBO(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        //}
                        if (o1ClickC.bIsCopy)
                        {
                            sCopyBtn = "<input type='button' class='btn btn-theme' value='Copy' onclick ='fncCopyBO(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsDelete)
                        {
                            sDeleteBtn = "<input type='button' class='btn btn-theme' value='Delete' onclick ='fncDeleteBO(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsView)
                        {
                            sViewbtn = "<input type='button' class='btn lbluebtn' value='View' onclick ='fncViewBO(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsCheckbox) // * TO DO THIS IS HARD CODED FOR RECONCILLIATION NEED TO CHANGE IT LATER
                        {
                            sCheckBox = "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsAddTop)
                        {
                            sAddTopbtn = "<input type='button' class='btn btn-theme lbluebtn' value='+' onclick ='fncAddInstanceTop(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsAddBottom)
                        {
                            sAddBottombtn = "<input type='button' class='btn btn-theme lbluebtn' value='-' onclick ='fncAddInstanceBottom(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsOrderIncrement)
                        {
                            sOrderIncrementbtn = "<input type='button' class='btn btn-theme lbluebtn' value='↑' onclick ='fncOrderIncrement(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.sBOName + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.bIsOrderDecrement)
                        {
                            sOrderDecrementbtn = "<input type='button' class='btn btn-theme lbluebtn' value='↓' onclick ='fncOrderDecrement(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.sBOName + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\")' />";
                        }
                        if (o1ClickC.sCompile != "0")
                        {
                            sCompilebtn = "<input type='button' class='btn btn-theme lbluebtn' value='Compile' onclick ='XIRun(this," + o1ClickC.sCompile + ",\"" + o1ClickC.XIGUID + "\",\"" + sGUID + "\",\"" + o1ClickC.sBOName + "\",false)' />";
                        }
                        if (!string.IsNullOrEmpty(o1ClickC.sAssign) && o1ClickC.sAssign != "0")
                        {
                            sAssignbtn = "<input type='button' class='btn btn-theme lbluebtn' value='Assign' onclick ='fnc1ClickBtn(this," + o1ClickC.sAssign + ",\"" + o1ClickC.XIGUID + "\",\"" + sGUID + "\",\"" + o1ClickC.sBOName + "\",false)' />";
                        }
                        sButtons = sEditBtn + sCopyBtn + sDeleteBtn + sViewbtn + sAddTopbtn + sAddBottombtn + sOrderIncrementbtn + sOrderDecrementbtn + sCompilebtn + sAssignbtn;
                        if (o1ClickC.Actions != null && o1ClickC.Actions.Count() > 0)
                        {
                            sCopyBtn = "<input type='button' class='btn btn-theme' value='Copy' onclick ='fncCopyBO(this," + o1ClickC.XIGUID + "," + o1ClickC.CreateGroupIDXIGUID + "," + o1ClickC.BOIDXIGUID + "," + o1ClickC.iCreateXILinkIDXIGUID + ")' />";
                            sDeleteBtn = "<input type='button' class='btn btn-theme' value='Delete' onclick ='fncDeleteBO(this," + o1ClickC.XIGUID + "," + o1ClickC.CreateGroupIDXIGUID + "," + o1ClickC.BOIDXIGUID + "," + o1ClickC.iCreateXILinkIDXIGUID + ")' />";
                            sButtons = sCopyBtn + sDeleteBtn;
                        }
                        foreach (var Link in o1ClickC.MyLinks.Where(n => n.iType == 30))
                        {

                            //var
                            var btn = "<input type='button' class='btn btn-theme' value='" + Link.sName + "' onclick='fncExecuteXIAlgorithm(this," + Link.FKiXILinkIDXIGUID + ")' />";
                            sButtons = sButtons + btn;
                        }
                        //if (!string.IsNullOrEmpty(sButtons))
                        //{
                        //    o1ClickC.Headings.Add("Actions");
                        //}
                        o1ClickC.oDataSet.ToList().ForEach(m =>
                        {
                            if (!string.IsNullOrEmpty(sButtons))
                            {
                                XIIAttribute oAttrI = new XIIAttribute();
                                oAttrI.sName = "Actions";
                                oAttrI.sValue = sButtons;
                                m.Value.Attributes["Actions"] = oAttrI;
                            }
                            if (!string.IsNullOrEmpty(sCheckBox))
                            {
                                Dictionary<string, XIIAttribute> nAttributes = new Dictionary<string, XIIAttribute>();
                                XIIAttribute oAttrI = new XIIAttribute();
                                oAttrI.sName = "Select";
                                oAttrI.sValue = sCheckBox;
                                //m.Value.Attributes["Actions"] = oAttrI;
                                nAttributes["Select"] = oAttrI;
                                //m.Value.Attributes.Intersect(0, "Actions", oAttrI);
                                foreach (var item in m.Value.Attributes)
                                {
                                    nAttributes[item.Key] = item.Value;
                                }
                                m.Value.Attributes = nAttributes;
                            }
                        });
                        foreach (var oInstance in o1ClickC.oDataSet.Values)
                        {
                            var oFileAttrs = oBOD.Attributes.Values.Where(m => m.FKiFileTypeID > 0).ToList();
                            if (!o1ClickC.bIsMultiBO)
                            {
                                List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                                foreach (var oAttr in oFileAttrs)
                                {
                                    var sName = oAttr.Name;
                                    var sFileID = oInstance.Attributes.Values.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(sFileID))
                                    {
                                        List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                                        var NewFileID = sFileID.Split(',').ToList();
                                        foreach (var item in NewFileID)
                                        {
                                            if (!string.IsNullOrEmpty(item.ToString()))
                                            {
                                                XIInfraDocs oDocs = new XIInfraDocs();
                                                oDocs.ID = Convert.ToInt32(item);
                                                if (oDocs.ID > 0)
                                                {
                                                    var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                                    if (sImagePathDetails != null)
                                                    {
                                                        ImagePathDetails.AddRange(sImagePathDetails);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    oInstance.Attributes.Values.Where(x => x.sName.ToLower() == sName.ToLower()).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                                }
                                if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()))
                                {
                                    if (oInstance.Attributes.Values.Where(x => x.sName.StartsWith("r")).ToList().Count() > 0)
                                    {
                                        foreach (var item in oInstance.Attributes.Values.Where(x => x.sName.StartsWith("r") && !x.sName.StartsWith("ref")).ToList())
                                        {
                                            item.sValue = string.IsNullOrEmpty(item.sValue) ? item.sValue :
                                                (!string.IsNullOrEmpty(item.sValue) && item.sValue.StartsWith("£")) ? item.sValue :
                                                (Convert.ToDecimal(item.sValue)).ToString("c", CultureInfo.CreateSpecificCulture("en-GB"));
                                        }
                                    }
                                }
                            }
                            var FileAttrs = oBOD.Attributes.Values.ToList().Where(k => k.bSelfReferenceFile == true).ToList();
                            if (FileAttrs != null && FileAttrs.Count() > 0)
                            {
                                foreach (var fileattr in FileAttrs)
                                {
                                    List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                                    var DocID = oInstance.AttributeI("id").sValue;
                                    if (!string.IsNullOrEmpty(DocID))
                                    {
                                        XIInfraDocs oDocs = new XIInfraDocs();
                                        oDocs.ID = Convert.ToInt32(DocID);
                                        if (oDocs.ID > 0)
                                        {
                                            var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                            if (sImagePathDetails != null)
                                            {
                                                ImagePathDetails.AddRange(sImagePathDetails);
                                            }
                                        }
                                    }
                                    oInstance.Attributes.Values.Where(x => x.sName.ToLower() == fileattr.Name.ToLower()).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                                }
                            }
                        }
                        if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()))
                        {
                            if ((o1ClickC.FKiComponentID > 0 || (o1ClickC.FKiComponentIDXIGUID != null && o1ClickC.FKiComponentIDXIGUID != Guid.Empty)) && o1ClickC.XIComponent != null)
                            {
                                if (o1ClickC.XIComponent.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                                {
                                    string iLayoutID = string.Empty;
                                    var iTemplateID = string.Empty;
                                    if (o1ClickC.XIComponent.Params != null && o1ClickC.XIComponent.Params.Count() > 0)
                                    {
                                        iLayoutID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "slayout").Select(m => m.sValue).FirstOrDefault();
                                        iTemplateID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "stemplate").Select(m => m.sValue).FirstOrDefault();
                                        sStructureName = sStructureName == "" ? null : sStructureName;
                                        sStructureName = sStructureName == null ? o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "sstructurename").Select(m => m.sValue).FirstOrDefault() : sStructureName;
                                    }
                                    XIContentEditors oContent = new XIContentEditors();
                                    List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                                    if (!string.IsNullOrEmpty(iLayoutID))
                                    {
                                        var oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, iLayoutID);
                                        oContent = new XIContentEditors();
                                        oContent.Content = oLayout.LayoutCode;
                                    }
                                    else if (!string.IsNullOrEmpty(iTemplateID))
                                    {
                                        oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, iTemplateID);
                                        if (oContentDef != null && oContentDef.Count() > 0)
                                        {
                                            oContent = oContentDef.FirstOrDefault();
                                        }
                                    }
                                    o1ClickC.RepeaterResult = new List<string>();
                                    int iResultCount = 1;
                                    XIContentEditors oConent = new XIContentEditors();
                                    CResult Result = new CResult();
                                    XIBOInstance oBOIns = new XIBOInstance();
                                    if (oRes.Values.Count() > 0 && o1ClickC.Code == "EXPORT")
                                    {
                                        List<XIIBO> oList = oRes.Values.ToList();
                                        oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                        oBOIns.oStructureInstance.Add(o1ClickC.sBOName, oList);
                                        Result = oContent.MergeContentTemplate(oContent, oBOIns);
                                        if (Result.bOK && Result.oResult != null)
                                        {
                                            var oBOIHTML = Result.oResult.ToString();
                                            o1ClickC.RepeaterResult.Add(oBOIHTML);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var oBOI in oRes.Values)
                                        {
                                            string sNewGUID = Guid.NewGuid().ToString();
                                            var disable = "unchecked";
                                            var oCacheI = oCache.Get_XICache();
                                            var oCacheAddonI = oCacheI.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sGUID).NMyInstance.Where(m => m.Key.Contains("Addon_")).Select(m => m.Value.sValue).ToList();
                                            if (oBOI != null && oBOI.Attributes != null)
                                            {
                                                if (oBOI.Attributes.ContainsKey("id"))
                                                {
                                                    if (oCacheAddonI.Contains(oBOI.Attributes["id"].sValue))
                                                    {
                                                        disable = "checked";
                                                    }
                                                }
                                            }
                                            List<XIIBO> nBOI = new List<XIIBO>();
                                            if (oBOI.iBODID == 0)
                                            {
                                                oBOI.iBODID = oBOD.BOID;
                                            }
                                            nBOI.Add(oBOI);
                                            var sInstanceID = oBOI.AttributeI(oBOD.sPrimaryKey).sValue;
                                            oCache.Set_ParamVal(sSessionID, sNewGUID, null, "{-iInstanceID}", sInstanceID, null, null);
                                            oCache.Set_ParamVal(sSessionID, sNewGUID, null, "{XIP|" + oBOD.Name + ".id}", sInstanceID, null, null);
                                            oCache.Set_ParamVal(sSessionID, sNewGUID, null, "{XIP|ActiveBO}", oBOD.Name, null, null);
                                            oCache.Set_ParamVal(sSessionID, sGUID, null, "{xip|-iUserOrg}", iOrgID, null, null);
                                            oConent.sGUID = sGUID;
                                            oConent.sSessionID = sSessionID;
                                            /* uncomment SaveErrorToDB() lines to log performance to DB.*/
                                            if (!string.IsNullOrEmpty(sStructureName))
                                            {
                                                var oCResultLog = new CResult();
                                                oCResultLog.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResultLog.iLogLevel = (int)EnumXIErrorPriority.Critical;
                                                oCResultLog.sMessage = "Monitoring structure loading";
                                                //SaveErrortoDB(oCResultLog);
                                                var watch = System.Diagnostics.Stopwatch.StartNew();
                                                watch.Start();
                                                XIIXI oIXI = new XIIXI();
                                                //var oStructobj = oIXI.BOI(oBOD.Name, sInstanceID).Structure(sStructureName).XILoad(null, true);
                                                var oBOInstance = oIXI.BOI(oBOD.Name, sInstanceID);
                                                var oBOStructure = oBOInstance.Structure(sStructureName);
                                                var watchStructLoad = System.Diagnostics.Stopwatch.StartNew();
                                                watchStructLoad.Start();
                                                var oStructobj = oBOStructure.XILoad(null, true);
                                                watchStructLoad.Stop();
                                                double dStructLoadTime = watchStructLoad.ElapsedMilliseconds / 1000.0;
                                                oCResultLog.sMessage = "Time taken to load structure: " + dStructLoadTime.ToString();
                                                //SaveErrortoDB(oCResultLog);
                                                //Result = oConent.MergeTemplateContent(oContent, oStructobj);
                                                Result = oConent.MergeContentTemplate(oContent, oStructobj);
                                                watch.Stop();
                                                double dElapsedTime = watch.ElapsedMilliseconds / 1000.0;
                                                oCResultLog.sMessage = "Time taken to load structure and merge template: " + dElapsedTime.ToString();
                                                //SaveErrortoDB(oCResultLog);

                                            }
                                            else
                                            {
                                                oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                                oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                                                //Result = oConent.MergeTemplateContent(oContent, oBOIns);
                                                Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                            }
                                            if (Result.bOK && Result.oResult != null)
                                            {
                                                var oBOIHTML = Result.oResult.ToString();
                                                var regx = new Regex("{.*?}");
                                                var Matches = regx.Matches(oBOIHTML);
                                                if (Matches.Count > 0)
                                                {
                                                    var bIsDisabled = string.Empty;
                                                    if (bIsLockStep)
                                                    {
                                                        bIsDisabled = "Disabled=" + "Disabled";
                                                    }
                                                    foreach (var match in Matches)
                                                    {
                                                        if (match.ToString().ToLower() == "{editbtn}")
                                                        {
                                                            var EditBtn = "<input type=\"button\" value=\"Edit\" class=\"btn btn-theme\" data-id=" + sInstanceID + " " + bIsDisabled + " />";
                                                            oBOIHTML = oBOIHTML.Replace("{EditBtn}", EditBtn);
                                                        }
                                                        else if (match.ToString().ToLower() == "{deletebtn}")
                                                        {
                                                            var Count = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|sDriverSNo_" + sInstanceID + "}");
                                                            int iCount = 0;
                                                            if (int.TryParse(Count, out iCount))
                                                            { }
                                                            var DeleteBtn = "<input type=\"button\" value=\"Delete\" class=\"btn btn-theme\" id=\"Delete_" + oBOD.Name + "_" + sInstanceID + "\" data-id=" + sInstanceID + " data-sNo=" + iCount + " onclick=\"DeleteBO('" + oBOI.Attributes[oBOD.sPrimaryKey].sValue + "','" + sGUID + "','" + oBOD.Name + "','" + o1ClickC.XIComponent.sName.ToLower() + "' ,this " + " )\" " + bIsDisabled + " />";
                                                            oBOIHTML = oBOIHTML.Replace("{DeleteBtn}", DeleteBtn);
                                                        }
                                                        else if (match.ToString().ToLower() == "{more details}" || match.ToString().ToLower() == "{view quote}" || match.ToString().ToLower() == "{view}")
                                                        {
                                                            //List<CNV> fncParams = new List<CNV>();
                                                            //fncParams.Add(new CNV { sName = "{-iInstanceID}", sValue = sInstanceID });
                                                            //fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                            //fncParams.Add(new CNV { sName = "{XIP|ActiveBO}", sValue = oBOD.Name });
                                                            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                            //string sJSON = oSerializer.Serialize(fncParams);
                                                            //string sInputValue = Regex.Replace(match.ToString().Replace("{", "").Replace("}", ""), @"\s+", "");
                                                            var MoreDetailsBtn = "<input type=\"button\" onclick=XILinkLoadJson(" + o1ClickC.RowXiLinkIDXIGUID + ",'" + sNewGUID + "') value=\"" + match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " " + bIsDisabled + " />";
                                                            //var MoreDetailsBtn = "<input type=\"button\" value=\""+ match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"ViewMore('"+ sInputValue + "_" + sInstanceID + "')\" />";
                                                            oBOIHTML = oBOIHTML.Replace(match.ToString(), MoreDetailsBtn);
                                                        }
                                                        else if (match.ToString().ToLower() == "{remove}" || match.ToString().ToLower() == "{add}")
                                                        {
                                                            var RemoveBtn = "<input type=\"checkbox\" class=\"switch-input\" Onchange =\"fnc1clickremove('" + o1ClickC.XIGUID + "','" + o1ClickC.CreateGroupIDXIGUID + "','" + o1ClickC.BOIDXIGUID + "','" + sInstanceID + "' ,this " + ")\" " + disable + " " + bIsDisabled + " />";
                                                            oBOIHTML = oBOIHTML.Replace("{Remove}", RemoveBtn);
                                                        }
                                                        //else if (match.ToString().ToLower() == "{add}")
                                                        //{
                                                        //    string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                        //    var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();
                                                        //    //var Btn = "<input type=\"button\" value=\"Add\" class=\"btn btn-theme\" id=\"Add_" + oBOD.Name + "_" + sInstanceID + "\"  data-id=" + sInstanceID + " onclick=\"fnc1clickcreate('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + oLink.FKiXILinkID + "','" + sInstanceID + "')\"  " + disable + " />";
                                                        //    var Btn = "<input type=\"button\" value=\"Add\" class=\"btn btn-sm btn-success table-btn ConvetBtn\" data-type=\"notselected\" id=\"Add_" + oBOD.Name + "_" + sInstanceID + "\"  data-id=" + sInstanceID + " onclick=\"fnAddon('" + sInstanceID + "')\"  " + disable + " />";
                                                        //    oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                        //}
                                                        else if (match.ToString().ToLower() == "{buynow}")
                                                        {
                                                            var BuyBtn = "<input type=\"button\" value=\"BuyNow\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"BuyQuoteBtn('" + oBOI.AttributeI("rfinalquote").sValue + "','" + sInstanceID + "',this " + ")\" " + bIsDisabled + " />";
                                                            oBOIHTML = oBOIHTML.Replace(match.ToString(), BuyBtn);
                                                        }
                                                        else if (match.ToString().ToLower() == "{remove quote}")
                                                        {
                                                            var DeleteBtn = "<input type=\"button\" value=\"Remove\" class=\"btn btn-theme\" id=\"Delete_" + oBOD.Name + "_" + sInstanceID + "\" data-id=" + sInstanceID + " data-Remove=\"Quote_" + sInstanceID + "\" onclick=\"DeleteBO('" + oBOI.Attributes[oBOD.sPrimaryKey].sValue + "','" + sGUID + "','" + oBOD.Name + "','" + o1ClickC.XIComponent.sName.ToLower() + "' ,this " + " )\" " + bIsDisabled + " />";
                                                            oBOIHTML = oBOIHTML.Replace("{Remove Quote}", DeleteBtn);
                                                        }
                                                        else if (match.ToString().ToLower() == "{snewguid}")
                                                        {
                                                            oBOIHTML = oBOIHTML.Replace(match.ToString(), string.Format("'{0}'", sNewGUID));
                                                        }
                                                        else
                                                        {
                                                            string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                            var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();
                                                            //if (Code.StartsWith("Close"))
                                                            //{
                                                            //    if (oLink != null)
                                                            //    {
                                                            //        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"CloseDetails('PolicyDetails_" + sInstanceID + "')\" />";
                                                            //        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                            //    }
                                                            //}
                                                            if (Code.StartsWith("Make a Change"))
                                                            {
                                                                if (oLink != null)
                                                                {
                                                                    var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaChange('" + sInstanceID + "')\" />";
                                                                    oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                                }
                                                            }
                                                            else if (Code.StartsWith("Make a Claim"))
                                                            {
                                                                if (oLink != null)
                                                                {
                                                                    var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaClaim('" + sInstanceID + "')\" />";
                                                                    oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (oLink != null)
                                                                {
                                                                    string bIsLoading = "no";
                                                                    if (o1ClickC.Name.ToLower() == "Additional Driver List".ToLower())
                                                                    {
                                                                        var sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|sDriverSNo_" + sInstanceID + "}");
                                                                        if (string.IsNullOrEmpty(sValue))
                                                                        {
                                                                            oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sDriverSNo_" + sInstanceID + "}", oBOI.AttributeI("iDriverNo").sValue, null, null);
                                                                        }
                                                                        bIsLoading = "yes";
                                                                    }
                                                                    List<CNV> fncParams = new List<CNV>();//{XIP|ACPolicy_T.id}
                                                                    fncParams.Add(new CNV { sName = oBOD.Name, sValue = sInstanceID });
                                                                    fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                                    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                                    string sJSON = oSerializer.Serialize(fncParams);
                                                                    //var Btn = "<input type=\"button\" onclick=XILinkLoadJson(" + oLink.FKiXILinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + oLink.sName + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                                    var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " data-IsLoading=" + bIsLoading + " onclick=\"fnc1clickcreate('" + o1ClickC.XIGUID + "','" + o1ClickC.CreateGroupIDXIGUID + "','" + o1ClickC.BOIDXIGUID + "','" + oLink.FKiXILinkIDXIGUID + "','" + sInstanceID + "', this " + " )\" " + bIsDisabled + " />";
                                                                    oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                o1ClickC.RepeaterResult.Add(oBOIHTML);
                                            }
                                            iResultCount++;
                                        }
                                    }
                                }
                                else if (o1ClickC.XIComponent.sName.ToLower() == XIConstant.FormComponent.ToLower())
                                {
                                    Dictionary<string, XIIBO> oDataSet = new Dictionary<string, XIIBO>();
                                    var oComp = o1ClickC.XIComponent;
                                    string sGroupName = oComp.Params.Where(m => m.sName.ToLower() == "Group".ToLower()).Select(m => m.sValue).FirstOrDefault();
                                    string sHiddenGroup = oComp.Params.Where(m => m.sName.ToLower() == "HiddenGroup".ToLower()).Select(m => m.sValue).FirstOrDefault();
                                    if (oRes.Count() == 0)
                                    {
                                        if (string.IsNullOrEmpty(sGroupName))
                                        {
                                            sGroupName = "create";
                                        }
                                        XIIBO oBOI = new XIIBO();
                                        oBOI.iBODID = o1ClickC.BOID;
                                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickC.BOD.BOID.ToString());// o1ClickC.BOD;
                                        oBOI.LoadBOI(sGroupName);
                                        if (!oBOI.Attributes.ContainsKey("xiguid") && oBOI.BOD.Attributes.ContainsKey("xiguid"))
                                        {
                                            var GUIDAttr = oBOI.BOD.AttributeD("xiguid");
                                            oBOI.Attributes.Add(GUIDAttr.Name, new XIIAttribute { sName = GUIDAttr.Name, Format = GUIDAttr.Format, sDefaultDate = GUIDAttr.sDefaultDate, iOneClickID = GUIDAttr.iOneClickID, sValue = null, bDirty = false });
                                        }
                                        var FKAttributes = oBOI.BOD.Attributes.Where(m => m.Value.FKiType > 0 && !string.IsNullOrEmpty(m.Value.sFKBOName) && m.Value.iOneClickID > 0).ToList();
                                        XIDXI oXID = new XIDXI();
                                        foreach (var item in FKAttributes)
                                        {
                                            if (item.Value.iOneClickID > 0)
                                            {
                                                string sBODataSource = string.Empty;
                                                var sBOName = item.Value.sFKBOName;
                                                Dictionary<string, object> Params = new Dictionary<string, object>();
                                                Params["Name"] = sBOName;
                                                string sSelectFields = string.Empty;
                                                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                                                var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                                                sBODataSource = oXID.GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBOI.BOD.FKiApplicationID);
                                                oBOI.BOD.Attributes[item.Value.Name.ToLower()].sFKBOSize = FKBOD.sSize;
                                                oBOI.BOD.Attributes[item.Value.Name.ToLower()].sFKBOName = FKBOD.Name;
                                                if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                                                {
                                                    var Con = new XIDBAPI(sBODataSource);
                                                    if (FKBOD.sType != null && FKBOD.sType.ToLower() == "reference")
                                                    {
                                                        string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                                        var oResult = oXID.Get_AutoCompleteList(suid, "", nParms);
                                                        List<XIDropDown> FKDDL = new List<XIDropDown>();
                                                        if (oResult.bOK && oResult.oResult != null)
                                                        {
                                                            var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                                            FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                                        }
                                                        if (oBOI.Attributes.ContainsKey(item.Value.Name.ToLower()))
                                                        {
                                                            oBOI.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(sHiddenGroup))
                                        {
                                            string sPrimaryKey = string.Empty;
                                            sPrimaryKey = oBOI.BOD.sPrimaryKey;
                                            var GroupFields = oBOI.BOD.GroupD(sHiddenGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                                            if (!string.IsNullOrEmpty(GroupFields))
                                            {
                                                var oGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                                oBOI.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bIsHidden = true);
                                            }
                                        }
                                        oBOI.BOD = null;

                                        oDataSet["0"] = oBOI;
                                        o1ClickC.oDataSet = oDataSet;
                                        o1ClickC.sIsFirstTime = "yes";
                                        o1ClickC.IsDisableSave = true;
                                    }
                                    else
                                    {
                                        o1ClickC.IsDisableSave = false;
                                        foreach (var boi in oRes.Values)
                                        {
                                            var FKAttributes = boi.BOD.Attributes.Where(m => m.Value.FKiType > 0 && !string.IsNullOrEmpty(m.Value.sFKBOName) && m.Value.iOneClickID > 0).ToList();
                                            XIDXI oXID = new XIDXI();
                                            foreach (var item in FKAttributes)
                                            {
                                                if (item.Value.iOneClickID > 0)
                                                {
                                                    string sBODataSource = string.Empty;
                                                    var sBOName = item.Value.sFKBOName;
                                                    Dictionary<string, object> Params = new Dictionary<string, object>();
                                                    Params["Name"] = sBOName;
                                                    string sSelectFields = string.Empty;
                                                    sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                                                    var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                                                    sBODataSource = oXID.GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), boi.BOD.FKiApplicationID);
                                                    boi.BOD.Attributes[item.Value.Name.ToLower()].sFKBOSize = FKBOD.sSize;
                                                    boi.BOD.Attributes[item.Value.Name.ToLower()].sFKBOName = FKBOD.Name;
                                                    if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                                                    {
                                                        var Con = new XIDBAPI(sBODataSource);
                                                        if (FKBOD.sType != null && FKBOD.sType.ToLower() == "reference")
                                                        {
                                                            string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                                            var oResult = oXID.Get_AutoCompleteList(suid, "", nParms);
                                                            List<XIDropDown> FKDDL = new List<XIDropDown>();
                                                            if (oResult.bOK && oResult.oResult != null)
                                                            {
                                                                var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                                                FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                                            }
                                                            if (boi.Attributes.ContainsKey(item.Value.Name.ToLower()))
                                                            {
                                                                boi.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(sHiddenGroup))
                                            {
                                                var GroupFields = oBOD.GroupD(sHiddenGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                                                if (!string.IsNullOrEmpty(GroupFields))
                                                {
                                                    var oGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                                    boi.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bIsHidden = true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    if (o1ClickC.bIsCopy || o1ClickC.IsEdit || o1ClickC.bIsDelete || o1ClickC.bIsView)
                    //    {
                    //        o1ClickC.Headings.Add("Actions");
                    //        o1ClickC.TableColumns.Add("Actions");
                    //    }
                    //}
                    //Result = oXIRepo.GetHeadingsForList(OneClickID, null, sDatabase, 0, iUserID, sOrgName);
                    o1ClickC.sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sDisplayMode))
                    {
                        o1ClickC.ActionType = "View";
                    }
                    o1ClickC.BO = o1ClickC.BOD.XIGUID.ToString();
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraOneClickComponent_XILoad() : 1-Click ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                    o1ClickC.sErrorMessage = "Mandatory parameters not passed to load the 1Q component";
                }
                o1ClickC.BOD = null;
                if (!string.IsNullOrEmpty(sSearchText))
                {
                    o1ClickC.SearchText = sSearchText;
                }
                if (!string.IsNullOrEmpty(sSearchType))
                {
                    o1ClickC.SearchType = sSearchType;
                }
                if (bIsLockStep)
                {
                    o1ClickC.IsRowClick = false;
                }
                if (!string.IsNullOrEmpty(sIDRef) && sIDRef == "xiguid")
                {
                    if (o1ClickC.oVisualisation == null)
                    {
                        o1ClickC.oVisualisation = new List<XIVisualisation>();
                    }
                    if (o1ClickC.oVisualisation.Count() == 0)
                    {
                        o1ClickC.oVisualisation.Add(new XIVisualisation());
                    }
                    if (o1ClickC.oVisualisation.FirstOrDefault().XiVisualisationNVs == null)
                    {
                        o1ClickC.oVisualisation.FirstOrDefault().XiVisualisationNVs = new List<XIVisualisationNV>();
                    }
                    o1ClickC.oVisualisation.FirstOrDefault().XiVisualisationNVs.Add(new XIVisualisationNV() { sName = "HiddenData", sValue = "yes" });
                }
                o1ClickC.FilterGroup = IDEParams;
                o1ClickC.LayoutGuid = sGUID;
                oCResult.oResult = o1ClickC;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                if (o1ClickC.ID > 0 && !string.IsNullOrEmpty(iUserID))
                {
                    InsertOneClickAudit(o1ClickC.XIGUID.ToString(), iUserID);
                }
            }
            catch (Exception ex)
            {
                o1ClickC.sErrorMessage = "Something went wrong while loading the 1Q component";
                oCResult.oResult = o1ClickC;
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing 1-Click Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public string SubStringNoation(string HtmlContentTable, string sStartFormat, string sEndFormat)
        {
            try
            {
                string sFinalString = "";
                if (HtmlContentTable.Contains(sEndFormat))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf(sEndFormat);
                    int iStartPosi = HtmlContentTable.LastIndexOf(sStartFormat, iCollectionIndex);
                    int iStringLength = sEndFormat.Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        private string InsertOneClickAudit(string iOneClickID, string iUserID)
        {
            XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, iOneClickID.ToString());
            var oCopy = (XID1Click)o1ClickD.Clone(o1ClickD);
            XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
            List<CNV> nParams = new List<CNV>();
            nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
            oCopy.ReplaceFKExpressions(nParams);
            XIIBO oBOI = new XIIBO();
            XIDBO oBOD = new XIDBO();
            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XI1ClickAudit");
            oBOI.BOD = oBOD;
            oBOI.Attributes["FKi1ClickIDXIGUID".ToLower()] = new XIIAttribute { sName = "FKi1ClickIDXIGUID", sValue = iOneClickID.ToString() };
            oBOI.Attributes["FKi1ClickIDXIGUID".ToLower()].bDirty = true;
            oBOI.Attributes["FKiUserID".ToLower()] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString() };
            oBOI.Attributes["FKiUserID".ToLower()].bDirty = true;
            if (!string.IsNullOrEmpty(o1ClickD.sLog) && o1ClickD.sLog.ToLower() != "No".ToLower())
            {
                if (o1ClickD.sLog.ToLower() == "Query".ToLower())
                {
                    oBOI.Attributes["sQuery".ToLower()] = new XIIAttribute { sName = "sQuery", sValue = oCopy.Query };
                    oBOI.Attributes["sQuery".ToLower()].bDirty = true;
                }
                else if (o1ClickD.sLog.ToLower() == "User executed".ToLower())
                {
                    oBOI.Attributes["sQuery".ToLower()] = new XIIAttribute { sName = "sQuery", sValue = "-" };
                    oBOI.Attributes["sQuery".ToLower()].bDirty = true;
                }
                var oResults = oBOI.Save(oBOI);
            }
            return null;
        }
    }
}