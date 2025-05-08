using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XICore;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIInfraGridComponent : XIDefinitionBase
    {

        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sOrgDatabase { get; set; }
        public string OneClickID;
        public string sOrgName { get; set; }
        public string sContext { get; set; }
        public int iSectionInstanceID { get; set; }
        public int iStepInstanceID { get; set; }
        public string AttributeName1 { get; set; }
        public string AttributeValue1 { get; set; }
        public string AttributeName2 { get; set; }
        public string AttributeValue2 { get; set; }
        public string sSection { get; set; }
        public string sDisplayMode { get; set; }
        public string sLockGroup { get; set; }
        public string sSearchText { get; set; }
        public string sSearchType { get; set; }
        public string Fields { get; set; }
        public string Optrs { get; set; }
        public string Values { get; set; }
        public string sCondition { get; set; }
        public string sVisualisation { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        XIInfraCache oCache = new XIInfraCache();

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
            try
            {
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sDisplayMode = oParams.Where(m => m.sName == "DisplayMode").Select(m => m.sValue).FirstOrDefault();
                sLockGroup = oParams.Where(m => m.sName.ToLower() == "LockGroup".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sSearchText = oParams.Where(m => m.sName == XIConstant.Param_SearchText).Select(m => m.sValue).FirstOrDefault();
                sSearchType = oParams.Where(m => m.sName == XIConstant.Param_SearchType).Select(m => m.sValue).FirstOrDefault();
                Fields = oParams.Where(m => m.sName == "Fields").Select(m => m.sValue).FirstOrDefault();
                Optrs = oParams.Where(m => m.sName == "Optrs").Select(m => m.sValue).FirstOrDefault();
                Values = oParams.Where(m => m.sName == "Values").Select(m => m.sValue).FirstOrDefault();
                sVisualisation = oParams.Where(m => m.sName == "Visualisation").Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var sIDRef = oParams.Where(m => m.sName == "sIDRef").Select(m => m.sValue).FirstOrDefault();
                int iPageCount = 0;
                var FKiVisualisationID = 0;
                Guid FKiVisualisationIDXIGUID = Guid.Empty;
                string sHiddenGroup = string.Empty;
                var sPageCount = oParams.Where(m => m.sName == "iPageCount").Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sPageCount, out iPageCount);
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
                XID1Click o1ClickD = new XID1Click();
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    string sOneClickID = WrapperParms.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sOneClickID))
                    {
                        OneClickID = sOneClickID;
                    }
                    else
                    {
                        //IDE
                        OneClickID = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_1ClickID);
                        //int.TryParse(s1ClickID, out OneClickID);

                        var sParentFKColumn = oCache.Get_ParamVal(sSessionID, sGUID, null, "ParentFKColumn");
                        if (!string.IsNullOrEmpty(sIDRef) && sIDRef == "xiguid" && !string.IsNullOrEmpty(sParentFKColumn))
                        {

                            var sBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sBO);
                            if (BOD != null && BOD.Attributes.Count > 0 && BOD.Attributes.ContainsKey(sParentFKColumn))
                            {
                                var sFKBO = BOD.Attributes[sParentFKColumn].sFKBOName;
                                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sFKBO);
                                if (FKBOD != null && FKBOD.bUID)
                                {
                                    //sParentFKColumn = sParentFKColumn + "XIGUID";
                                }
                            }
                        }
                        var iParentInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, "ParentInsID");

                        if (!string.IsNullOrEmpty(sParentFKColumn) && iParentInsID != "0")
                        {
                            sCondition = sParentFKColumn + "=" + iParentInsID;
                        }
                    }

                    //IDE
                    var sFKiVisualisationID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|FKiVisualisationIDXIGUID}");
                    Guid.TryParse(sFKiVisualisationID, out FKiVisualisationIDXIGUID);
                }
                else
                {
                    OneClickID = oParams.Where(m => m.sName.ToLower() == "1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    //int.TryParse(s1ClickID, out OneClickID);
                }
                var classID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ClassID}");
                
                if (!string.IsNullOrEmpty(OneClickID))
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID);
                    if (!string.IsNullOrEmpty(o1ClickD.sActionOneClickType))
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|-iActionOneClickType}", o1ClickD.sActionOneClickType.ToString(), null, null);
                    var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    //Get BO Definition
                    XIDBO oBOD = new XIDBO();
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickD.BOID.ToString());
                    }
                    o1ClickC.BOD = oBOD;
                    o1ClickC.sBOName = oBOD.Name;
                    if (!string.IsNullOrEmpty(sCondition))
                    {
                        o1ClickC.sParentWhere = sCondition;
                    }
                    o1ClickC.sLockGroup = sLockGroup;
                    //Get Headings of 1-Click
                    o1ClickC.Get_1ClickHeadings();
                    XICacheInstance oGUIDParam = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParam.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    o1ClickC.ReplaceFKExpressions(nParms);
                    //o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                    //o1ClickC.bIsResolveFK = true;
                    if (sSearchType == "NaturalSearch")
                    {
                        o1ClickC.SearchText = sSearchText;
                        o1ClickC.SearchType = sSearchType;
                    }
                    else if (sSearchType == "FilterSearch" && (Fields != "" && Optrs != "" && Values != ""))
                    {
                        o1ClickC.Fields = Fields;
                        o1ClickC.Optrs = Optrs;
                        o1ClickC.Values = Values;
                        o1ClickC.SearchType = sSearchType;
                    }
                    else if (sSearchType == "FilterSearch" && (Fields == "" && Optrs == "" && Values == ""))
                    {
                        o1ClickC.Fields = Fields;
                        o1ClickC.Optrs = Optrs;
                        o1ClickC.Values = Values;
                        o1ClickC.SearchType = sSearchType;
                    }
                    else if (sSearchType == null && (Fields == null && Optrs == null && Values == null))
                    {
                        o1ClickC.Fields = Fields;
                        o1ClickC.Optrs = Optrs;
                        o1ClickC.Values = Values;
                        o1ClickC.SearchType = sSearchType;
                    }
                    string sHideSelect = string.Empty;
                    if (o1ClickC.oOneClickParameters.Where(x => x.iType == 10).Count() > 0)
                    {
                        if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()))
                        {
                            var ohideParams = o1ClickC.oOneClickParameters.Where(x => x.iType == 10).Select(m => m.sName).ToList();
                            if (ohideParams != null && ohideParams.Count() > 0)
                            {
                                var oHideparm1 = ohideParams.Where(m => m.Contains(".")).ToList();
                                var oHideparm2 = ohideParams.Where(m => !m.Contains(".")).ToList();
                                var sConcatStr = oHideparm2.Select(f => "CONCAT('" + f + "','__'," + oBOD.TableName + "." + f + ")").ToList();
                                var sConcatStr2 = oHideparm1.Select(f => "CONCAT('" + f + "','__'," + f + ")").ToList();
                                sConcatStr = sConcatStr.Concat(sConcatStr2).ToList();
                                if (sConcatStr.Count() > 1)
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
                    o1ClickC.iSkip = iPageCount * o1ClickC.iPaginationCount;
                    o1ClickC.iTake = o1ClickC.iPaginationCount;
                    o1ClickC.iTotaldisplayRecords = iPageCount;
                    o1ClickC.bIsGrid = true;
                    Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                    o1ClickC.BOD = null;
                    oCResult.oResult = o1ClickC;
                    string sButtons = string.Empty;
                    string sCheckBox = string.Empty;
                    string sEditBtn = string.Empty;
                    string sCopyBtn = string.Empty;
                    string sDeleteBtn = string.Empty;
                    string sViewbtn = string.Empty;
                    if (o1ClickC.bIsCopy)
                    {
                        sCopyBtn = "<input type='button' class='btn btn-theme' value='Copy' onclick ='fncCopyBO(this," + o1ClickC.XIGUID + "," + o1ClickC.CreateGroupIDXIGUID + "," + o1ClickC.BOIDXIGUID + "," + o1ClickC.iCreateXILinkIDXIGUID + ")' />";
                    }
                    if (o1ClickC.bIsDelete)
                    {
                        sDeleteBtn = "<input type='button' class='btn btn-theme' value='Delete' onclick ='fncDeleteBO(this," + o1ClickC.XIGUID + "," + o1ClickC.CreateGroupIDXIGUID + "," + o1ClickC.BOIDXIGUID + "," + o1ClickC.iCreateXILinkIDXIGUID + ")' />";
                    }
                    if (o1ClickC.bIsView)
                    {
                        sViewbtn = "<input type='button' class='btn lbluebtn' value='View' onclick ='fncGridViewBO(this,\"" + o1ClickC.XIGUID + "\",\"" + o1ClickC.CreateGroupIDXIGUID + "\",\"" + o1ClickC.BOIDXIGUID + "\",\"" + o1ClickC.iCreateXILinkIDXIGUID + "\",\"" + o1ClickC.sBOName + "\")' />";
                    }
                    if (o1ClickC.bIsCheckbox) // * TO DO THIS IS HARD CODED FOR RECONCILLIATION NEED TO CHANGE IT LATER
                    {
                        sCheckBox = "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this," + o1ClickC.XIGUID + "," + o1ClickC.CreateGroupIDXIGUID + "," + o1ClickC.BOIDXIGUID + "," + o1ClickC.iCreateXILinkIDXIGUID + ")' />";
                    }
                    sButtons = sEditBtn + sCopyBtn + sDeleteBtn + sViewbtn;
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
                        if (o1ClickC.MyLinks.Count > 0)
                        {
                            foreach (var oLink in o1ClickC.MyLinks)
                            {
                                var sNewGUID = Guid.NewGuid();
                                var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  onclick=\"Execute1ClickLick('" + oLink.FKiXILinkIDXIGUID + "',this, '" + sNewGUID + "')\" />";
                                if (!string.IsNullOrEmpty(Btn))
                                {
                                    XIIAttribute oAttrI = new XIIAttribute();
                                    oAttrI.sName = "Links";
                                    oAttrI.sValue = Btn;
                                    m.Value.Attributes["Links"] = oAttrI;
                                }
                            }
                        }
                    });
                    var ValueTypeAttrs = oBOD.Attributes.Values.ToList().Where(m => m.bValueType == true).ToList();
                    foreach (var boi in o1ClickC.oDataSet.Values)
                    {
                        List<XIDScript> oXIDScripts = new List<XIDScript>();
                        if (oBOD.Name.ToLower() == "requirement_t")
                        {
                            if (boi.Attributes.ContainsKey("dDue"))
                            {
                                var dDueDate = boi.Attributes["dDue"].sValue;
                                //var sDueDate = Convert.ToDateTime(dDueDate).ToString("yyyy-MM-dd");
                                //var sCurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
                                if (!string.IsNullOrEmpty(dDueDate))
                                {
                                    var sDueDate = Convert.ToDateTime(dDueDate).Date;
                                    var sCurrentDate = DateTime.Now.Date;
                                    var days = sDueDate.Subtract(sCurrentDate).TotalDays;
                                    if (boi.Attributes.ContainsKey("iDueInDays"))
                                    {
                                        if (days >= 0)
                                        {
                                            boi.Attributes["iDueInDays"] = new XIIAttribute { sName = "iDueInDays", sValue = days.ToString() };
                                        }
                                        else
                                        {
                                            string sDueDays = "OverDue";
                                            boi.Attributes["iDueInDays"] = new XIIAttribute { sName = "iDueInDays", sValue = sDueDays };
                                        }
                                    }
                                }
                            }
                            //if (o1ClickC.ID == 2258)
                            //{
                            //    boi.GetScriptResult();
                            //}
                            if (o1ClickC.oScripts != null && o1ClickC.oScripts.Count() > 0)
                            {
                                boi.GetScriptResult(o1ClickC.oScripts);
                            }
                        }
                        foreach (var valuetype in ValueTypeAttrs)
                        {
                            var AttrI = boi.AttributeI(valuetype.Name);
                            if (AttrI != null && !string.IsNullOrEmpty(AttrI.sValue))
                            {
                                oCR = AttrI.Check_ValueTypeForMetaIcon(AttrI.sValue);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    boi.AttributeI(valuetype.Name).sValueTypeIcon = (string)oCR.oResult;
                                }
                            }
                        }
                        var oFileAttrs = oBOD.Attributes.Values.Where(m => m.FKiFileTypeID > 0).ToList();
                        List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                        foreach (var oAttr in oFileAttrs)
                        {
                            var sName = oAttr.Name;
                            var sFileID = boi.Attributes.Values.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sFileID))
                            {
                                List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                                var NewFileID = sFileID.Split(',').ToList();
                                foreach (var item in NewFileID)
                                {
                                    if (!string.IsNullOrEmpty(item.ToString()))
                                    {
                                        XIInfraDocs oDocs = new XIInfraDocs();
                                        int iDocID = 0;
                                        int.TryParse(item, out iDocID);
                                        if (iDocID > 0)
                                        {
                                            oDocs.ID = iDocID;
                                            var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                            if (sImagePathDetails != null)
                                            {
                                                ImagePathDetails.AddRange(sImagePathDetails);
                                            }
                                        }
                                        else
                                        {
                                            XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                                            oXIDocTypes.ID = oAttr.FKiFileTypeID;
                                            var oXIDocDetails = (XIInfraDocTypes)oXIDocTypes.Get_FileDocTypes().oResult;
                                            if (oXIDocDetails != null)
                                            {
                                                int pos = item.LastIndexOf("\\") + 1;
                                                string sFileName = item.Substring(pos, item.Length - pos);
                                                oXIDocDetails.Path = oXIDocDetails.Path.Replace("~", "");
                                                sPDFPathDetails.Add(new XIDropDown { Expression = oXIDocDetails.Path + "//" + item, text = sFileName });
                                                ImagePathDetails.AddRange(sPDFPathDetails);
                                            }
                                        }
                                    }
                                }
                            }
                            boi.Attributes.Values.Where(x => x.sName.ToLower() == sName.ToLower()).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                        }
                    }
                    //IDE
                    List<XIVisualisation> oXIVisualisations = new List<XIVisualisation>();
                    if (o1ClickC.FKiVisualisationID > 0)
                    {
                        FKiVisualisationID = o1ClickC.FKiVisualisationID;
                    }
                    if(o1ClickC.FKiVisualisationIDXIGUID != null && o1ClickC.FKiVisualisationIDXIGUID != Guid.Empty)
                    {
                        FKiVisualisationIDXIGUID = o1ClickC.FKiVisualisationIDXIGUID;
                    }
                    if (FKiVisualisationID > 0 || (FKiVisualisationIDXIGUID != null && FKiVisualisationIDXIGUID != Guid.Empty))
                    {
                        var oXIvisual = new XIVisualisation();
                        if (FKiVisualisationIDXIGUID != null && FKiVisualisationIDXIGUID != Guid.Empty)
                        {
                            oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, null, FKiVisualisationIDXIGUID.ToString());
                        }
                        else if(FKiVisualisationID > 0)
                        {
                            oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, null, FKiVisualisationID.ToString());
                        }
                        var oXIDVisual = (XIVisualisation)oXIvisual.GetCopy();
                        if (oXIDVisual != null)
                        {
                            foreach (var oVisualisation in oXIDVisual.XiVisualisationNVs)
                            {
                                if (oVisualisation.sName != null && oVisualisation.sName.ToLower() == "LockGroup".ToLower() && string.IsNullOrEmpty(sLockGroup))
                                {
                                    sLockGroup = oVisualisation.sValue;
                                }
                                if (oVisualisation.sName != null && oVisualisation.sName.ToLower() == "HiddenGroup".ToLower() && string.IsNullOrEmpty(sHiddenGroup))
                                {
                                    sHiddenGroup = oVisualisation.sValue;
                                }
                                foreach (var oBOI in o1ClickC.oDataSet.Values)
                                {
                                    if (oVisualisation.sValue != null && oVisualisation.sValue.IndexOf("{XIP") >= 0)
                                    {
                                        oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                                        if (oBOI.Attributes.ContainsKey(oVisualisation.sName) && string.IsNullOrEmpty(oBOI.AttributeI(oVisualisation.sName).sValue))
                                        {
                                            oBOI.Attributes[oVisualisation.sName] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, bDirty = true };
                                        }
                                    }
                                    else
                                    {
                                        if (oVisualisation.sName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || oVisualisation.sName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                        {
                                            if (!string.IsNullOrEmpty(oBOI.AttributeI(oVisualisation.sName).sValue))
                                            {
                                                string sValue = oBOI.AttributeI(oVisualisation.sName).sValue;
                                                string sFormat = XIConstant.DateTime_Format; //"dd-MMM-yyyy HH:mm";
                                                string sFormattedValue = String.Format("{0:" + sFormat + "}", Convert.ToDateTime(sValue));
                                                oBOI.AttributeI(oVisualisation.sName).sValue = sFormattedValue;
                                            }
                                            oVisualisation.sValue = Utility.GetDateResolvedValue(oVisualisation.sValue, XIConstant.DateTime_Format);
                                        }
                                        if (oBOI.Attributes.ContainsKey(oVisualisation.sName.ToLower()))
                                        {
                                            // oBOI.Attributes[oVisualisation.sName] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, bDirty = true };
                                        }
                                    }
                                }

                            }
                            oXIVisualisations.Add(oXIDVisual);
                        }
                    }
                    List<string> LockAttrs = new List<string>();
                    if (!string.IsNullOrEmpty(sVisualisation))
                    {
                        var oVisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisualisation);
                        if (oVisual != null && oVisual.XiVisualisationNVs != null)
                        {
                            LockAttrs = oVisual.XiVisualisationNVs.Where(m => m.sValue.ToLower() == "lock").Select(m => m.sName).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(sLockGroup))
                    {
                        o1ClickD.sLockGroup = sLockGroup;
                        string sPrimaryKey = string.Empty;
                        sPrimaryKey = oBOD.sPrimaryKey;
                        var GroupFields = oBOD.GroupD(sLockGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                        if (!string.IsNullOrEmpty(GroupFields))
                        {
                            var oGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            oGrpFields.AddRange(LockAttrs.ConvertAll(m => m.ToLower()));
                            foreach (var oBOI in o1ClickC.oDataSet.Values)
                            {
                                oBOI.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bLock = true);
                                oBOI.Attributes.Values.Where(m => m.sName == sPrimaryKey).ToList().ForEach(m => m.bLock = true);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sHiddenGroup))
                    {
                        o1ClickD.sHiddenGroup = sHiddenGroup;
                        string sPrimaryKey = string.Empty;
                        sPrimaryKey = oBOD.sPrimaryKey;
                        var GroupFields = oBOD.GroupD(sHiddenGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                        var oGroupD = oBOD.GroupD(sHiddenGroup);
                        var oAllGroupFields = Utility.GetBOGroupFields(GroupFields, oGroupD.bIsCrtdBy, oGroupD.bIsCrtdWhn, oGroupD.bIsUpdtdBy, oGroupD.bIsUpdtdWhn);
                        if (!string.IsNullOrEmpty(oAllGroupFields) && oBOD != null && oBOD.Attributes.Count > 0)
                        {
                            var oGrpFields = oAllGroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            oBOD.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.Name.ToLower())).ToList().ForEach(c => c.bIsHidden = true);
                        }
                    }

                    if (!string.IsNullOrEmpty(sSearchType))
                    {
                        o1ClickD.Fields = Fields;
                        o1ClickD.Values = Values;
                        o1ClickD.Optrs = Optrs;
                        o1ClickD.SearchType = sSearchType;
                        if (!string.IsNullOrEmpty(sSearchText))
                        {
                            o1ClickD.SearchText = sSearchText;
                        }
                    }
                    o1ClickC.FKiVisualisationID = FKiVisualisationID;
                    o1ClickC.FKiVisualisationIDXIGUID = FKiVisualisationIDXIGUID;
                    o1ClickC.sGUID = sGUID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    o1ClickC.oVisualisation = oXIVisualisations;
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraGridComponent_XILoad() : 1-Click ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
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

                //we get model properties
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
    }
}