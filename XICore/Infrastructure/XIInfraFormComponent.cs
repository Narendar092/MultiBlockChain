using Microsoft.SqlServer.Management.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;
using static iTextSharp.text.pdf.AcroFields;

namespace XICore
{
    public class XIInfraFormComponent : XIDefinitionBase
    {

        public string sBOName { get; set; }
        public string sGroupName { get; set; }
        public string sLockGroup { get; set; }
        public string sHiddenGroup { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOverrideGroup { get; set; }
        public string sBOActionCode { get; set; }
        public string sBOUpdateAction { get; set; }
        public string sLayoutGuid { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public CResult XILoad(List<CNV> oParams,int iOrgID=0)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            string sParGUID = string.Empty;
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
            XIBODisplay oBODisplay = new XIBODisplay();
            try
            {
                long iInstanceID = 0; string GroupID = string.Empty; string sInstanceID = string.Empty; var sRowAttr = string.Empty;
                sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                XIIXI oXII = new XIIXI();
                //if (!string.IsNullOrEmpty(sGUID))
                //{
                //    oCache.sSessionID = sSessionID;
                //    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                //    sGUID = ParentGUID;
                //}
                var iDataSourceID = 0;
                //First set all properties by extracting from oParams
                var ActiveBO = string.Empty;
                sGroupName = oParams.Where(m => m.sName == XIConstant.Param_Group).Select(m => m.sValue).FirstOrDefault();
                List<string> oAccordGroupNames = sGroupName.Split(',').ToList().Select(x => x.Trim()).ToList();
                Dictionary<string, List<string>> oAccordGroupAttr = new Dictionary<string, List<string>>();
                bool bShowAccordian = oAccordGroupNames.Count() > 1;
                sLockGroup = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_LockGroup.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sHiddenGroup = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_HiddenGroup.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sBOActionCode = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BOActionCode.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sBOUpdateAction = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BOUpdateAction.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sLayoutGuid = oParams.Where(m => m.sName.ToLower() == "sLayoutGuid".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var FormMode = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_DisplayMode.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var sCoreDB = oParams.Where(m => m.sName.ToLower() == "sCoreDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sIDRef = oParams.Where(m => m.sName.ToLower() == "sIDRef".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var parentxiguid = oCache.Get_ParamVal(sSessionID, sGUID, null, "parentxiguid");//oParams.Where(m => m.sName.ToLower() == "parentxiguid".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var parent1clickxiguid = oCache.Get_ParamVal(sSessionID, sGUID, null, "Parent1Click"); //oParams.Where(m => m.sName.ToLower() == "Parent1Click".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BO.ToLower()).Select(m => m.sValue).FirstOrDefault() != null)
                {
                    if (!oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BO.ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                    {
                        sBOName = oParams.Where(m => m.sName == XIConstant.Param_BO).Select(m => m.sValue).FirstOrDefault();
                    }
                }
                if (oParams.Where(m => m.sName == XIConstant.Param_InstanceID).Select(m => m.sValue).FirstOrDefault() == "-MainDriverID")
                {
                    var ID = oCache.Get_ParamVal(sSessionID, sGUID, null, "-MainDriverID"); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|Driver_T.id}", ID, null, null);
                    ActiveBO = sBOName;
                }
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam1".ToLower())).ToList();
                if (WatchParam != null && WatchParam.Count() > 0)
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
                List<XIVisualisation> Visualisations = new List<XIVisualisation>();
                var IDEParams = oCache.Get_Paramobject(sSessionID, sGUID, null, "IDEParams");
                if (IDEParams != null && IDEParams.nSubParams != null && IDEParams.nSubParams.Count() > 0)
                {
                    var sParentBO = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentBO.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentBO))
                    {
                        int iParentBOIID = 0;
                        var sParentBOIID = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentBOIID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        int.TryParse(sParentBOIID, out iParentBOIID);
                        if (iParentBOIID == 0 || (!string.IsNullOrEmpty(sParentBOIID)))
                        {
                            var oParentI = oXII.BOI(sParentBO, sParentBOIID);
                            if (oParentI != null && oParentI.Attributes.Count > 0 && oParentI.Attributes.ContainsKey(oParentI.BOD.sPrimaryKey))
                            {
                                int.TryParse(oParentI.Attributes[oParentI.BOD.sPrimaryKey].sValue, out iParentBOIID);
                            }
                        }
                        List<XIVisualisationNV> XiVisualisationNVs = new List<XIVisualisationNV>();
                        if (iParentBOIID > 0 || (!string.IsNullOrEmpty(sParentBOIID)))
                        {
                            XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sAskFK", sValue = "No" });
                        }
                        else
                        {
                            XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sAskFK", sValue = "Yes" });
                        }
                        var sParentFKCol = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentFKCol.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        var sParentName = IDEParams.nSubParams.Where(m => m.sName.ToLower() == XIConstant.Param_ParentName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sFKBO", sValue = sParentBO });
                        XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sFKCol", sValue = sParentFKCol });
                        XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sFKVal", sValue = sParentBOIID });
                        XiVisualisationNVs.Add(new XIVisualisationNV { sName = "sFKName", sValue = sParentName });
                        Visualisations.Add(new XIVisualisation { Name = "ASKFK", NVs = XiVisualisationNVs });
                    }
                }
                List<CNV> MergeAttrs = new List<CNV>();
                //if (WatchParam != null && WatchParam.Count() > 0)
                //{
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    ActiveBO = WrapperParms.Where(m => m.sName == XIConstant.XIP_ActiveBO).Select(m => m.sValue).FirstOrDefault();// oXIAPI.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");
                    var XIPBOID = WrapperParms.Where(m => m.sName == XIConstant.Param_InstanceID).Select(m => m.sValue).FirstOrDefault(); // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(XIPBOID))
                    {
                        //iInstanceID = Convert.ToInt32(XIPBOID);
                        sInstanceID = XIPBOID;
                    }
                    else
                    {
                        iInstanceID = 0;
                    }
                    if (!(string.IsNullOrEmpty(ActiveBO)))
                    {
                        sBOName = ActiveBO;
                    }
                    MergeAttrs = WrapperParms.Where(m => m.sType.ToLower() == "merge").ToList();
                }
                else
                {
                    if (oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BO.ToLower()).Select(m => m.sValue).FirstOrDefault() != null)
                    {
                        sBOName = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_BO.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    }
                    if (sBOName != null && (sBOName.StartsWith("{XIP|") || sBOName.StartsWith("-") || sBOName.StartsWith("{-")))
                    {
                        sBOName = oCache.Get_ParamVal(sSessionID, sGUID, null, sBOName);
                    }

                    sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iParentInstanceID}");
                    if (string.IsNullOrEmpty(sInstanceID))
                    {
                        sInstanceID = oParams.Where(m => m.sName == XIConstant.Param_InstanceID).Select(m => m.sValue).FirstOrDefault();
                    }
                    if (sInstanceID != null && (sInstanceID.StartsWith("{XIP|") || sInstanceID.StartsWith("-") || sInstanceID.StartsWith("{-")))
                    {
                        sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        //var iInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                        if (long.TryParse(sInstanceID, out iInstanceID))
                        {

                        }
                    }
                    sRowAttr = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|sRowAttr}");
                    if (!string.IsNullOrEmpty(sBOName) && sBOName.ToLower() == "Adv1Query Attribute Details".ToLower())
                    {
                        var o1ClickID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|XI1ClickXIGUID}");
                        var AttrID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|AttributeIDXIGUID}");
                        var AttrDataTypeID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|DataType}");
                        if (!string.IsNullOrEmpty(AttrID) && (string.IsNullOrEmpty(AttrDataTypeID) || AttrDataTypeID == "0"))
                        {
                            List<CNV> oPara = new List<CNV>();
                            CNV Param = new CNV();
                            Param.sName = "XIGUID";
                            Param.sValue = AttrID;
                            oPara.Add(Param);
                            var DataTypeID = oXII.BOI("XIBOAttribute", "", "", oPara);
                            if (DataTypeID != null)
                            {
                                var TypeID = DataTypeID.Attributes["TypeID"].sValue;
                                if (TypeID != null && TypeID == "10") { TypeID = "60"; }
                                if (TypeID != null && TypeID == "110") { TypeID = "150"; }
                                oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|DataType}", TypeID, null, null);
                            }
                            // oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|DataType}", DataTypeID, null, null);
                        }
                        List<CNV> oParams1 = new List<CNV>();
                        CNV Params = new CNV();
                        Params.sName = "FKiAttrXIGUID";
                        Params.sValue = AttrID;
                        oParams1.Add(Params);
                        Params = new CNV();
                        Params.sName = "FKiOneClickIDXIGUID";
                        Params.sValue = o1ClickID;
                        oParams1.Add(Params);
                        var oInstanceID = oXII.BOI(sBOName, "", "", oParams1);
                        if (oInstanceID != null && oInstanceID.Attributes.Count > 0)
                        {
                            sInstanceID = oInstanceID.Attributes["XIGUID"].sValue;
                        }
                    }
                    GroupID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iGroupID}");// oParams.Where(m => m.sName.ToLower() == "{XIP|ActiveBO}".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    var iNannoOrgID = 0;
                    var NannoOrgID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iNannoOrgID");
                    int.TryParse(NannoOrgID, out iNannoOrgID);
                    if (iNannoOrgID > 0)
                    {
                        XIDXI oXID = new XIDXI();
                        oXID.sOrgDatabase = sCoreDB;
                        oCR = oXID.Get_OrgDefinition("", iNannoOrgID.ToString());
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            var oOrgD = (XIDOrganisation)oCR.oResult;
                            if (oOrgD.bNannoApp)
                            {
                                var sDatabaseName = oOrgD.DatabaseName;
                                oCR = oXID.Get_DataSourceDefinition(sDatabaseName);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var DataSource = (XIDataSource)oCR.oResult;
                                    iDataSourceID = DataSource.ID;
                                }
                            }
                        }
                    }
                    //if (string.IsNullOrEmpty(ActiveBO))
                    //{
                    //    ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");// oParams.Where(m => m.sName.ToLower() == "{XIP|ActiveBO}".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    //}
                    //var Prm = "{XIP|" + ActiveBO + ".id}";
                    //var XIPBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    //if (ActiveBO == null)
                    //{
                    //    Prm = "{XIP|" + sBOName + ".id}";
                    //    XIPBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                    //}
                    //if (XIPBOIID != null)
                    //{
                    //    if (long.TryParse(XIPBOIID, out iInstanceID))
                    //    {

                    //    }
                    //}
                    //else
                    //{
                    //    iInstanceID = 0;
                    //}
                    //if (!(string.IsNullOrEmpty(ActiveBO)) && sBOName == "{XIP|ActiveBO}")
                    //{
                    //    sBOName = ActiveBO;
                    //}
                    ////string sInstanceID = oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault();
                    //if (!string.IsNullOrEmpty(sInstanceID) && iInstanceID == 0)
                    //{
                    //    //var iInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, sInstanceID);
                    //    if (long.TryParse(sInstanceID, out iInstanceID))
                    //    {

                    //    }
                    //}
                }
                //}
                XIDBO oBOD = new XIDBO();
                if (!string.IsNullOrEmpty(sBOName) && !sBOName.StartsWith("{XIP|"))
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Params loaded successfully for Form Component" });
                    XIIBO oBOI = new XIIBO();
                    XIDXI oXID = new XIDXI();
                    var oBOD1 = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);

                    if (oBOD1.iOrgObject == 1)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sBOName,null,null,null,0,iOrgID);
                    }
                    else
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, sBOName);
                    }
                    XIDBO oBODCopy = (XIDBO)oBOD.Clone(oBOD);
                    oXII.BOD = oBODCopy;
                    if (iInstanceID == -1)
                    {
                        iInstanceID = 0;
                        sInstanceID = "";
                    }
                    if (!string.IsNullOrEmpty(GroupID))
                    {
                        int iGroupID = Convert.ToInt32(GroupID);
                        iInstanceID = 0;
                        sGroupName = oBODCopy.Groups.Where(x => x.Value.ID == iGroupID).Select(x => x.Value.GroupName).FirstOrDefault();
                        oAccordGroupNames = sGroupName.Split(',').ToList().Select(x => x.Trim()).ToList();
                        bShowAccordian = oAccordGroupNames.Count() > 1;
                    }
                    if (!string.IsNullOrEmpty(oBODCopy.sTraceAttribute))
                    {
                        var TraceAttrs = oBODCopy.sTraceAttribute.Split(',');
                        foreach (var TraceAttr in TraceAttrs)
                        {
                            if ((oBOD.BOID == 717 || (oBOD.Attributes.ContainsKey(TraceAttr) && oBOD.AttributeD(TraceAttr).bIsTrace))/* && !string.IsNullOrEmpty(oBOD.sTraceAttribute)*/)
                            {

                                var sTraceID = string.Empty;
                                var iAttrID = oBOD.AttributeD(TraceAttr).ID;
                                var sAttrName = oBOD.AttributeD(TraceAttr).Name;
                                //if (oBOD.BOID == 717)
                                //{
                                //    var oBOTrace = oXII.BOI(oBOD.Name, iInstanceID.ToString());
                                //    if (oBOTrace != null && oBOTrace.Attributes.Count() > 0)
                                //    {
                                //        sTraceID = oBOTrace.AttributeI("fkivalidtraceid").sValue;
                                //    }
                                //}
                                //else
                                //{
                                List<CNV> oNVs = new List<CNV>();
                                oXII.iSwitchDataSrcID = oBOD.iDataSource;
                                oXII.SwitchDataSrcIDXIGUID = oBOD.iDataSourceXIGUID;
                                oNVs.Add(new CNV { sName = "FKiBOID", sValue = oBOD.BOID.ToString() });
                                oNVs.Add(new CNV { sName = "FKiAttrID", sValue = iAttrID.ToString() });
                                oNVs.Add(new CNV { sName = "iInstanceID", sValue = iInstanceID.ToString() });
                                var oBOTrace = oXII.BOI("TraceTransactions", "", "", oNVs);
                                //sTraceID = "40";
                                if (oBOTrace != null && oBOTrace.Attributes.Count() > 0)
                                {
                                    sTraceID = oBOTrace.AttributeI("fkivalidtraceid").sValue;
                                }
                                //}

                                if (!string.IsNullOrEmpty(sTraceID))
                                {
                                    oXII.iSwitchDataSrcID = oBOD.iDataSource;
                                    oXII.SwitchDataSrcIDXIGUID = oBOD.iDataSourceXIGUID;
                                    var oTraceI = oXII.BOI("refValidTrace_T", sTraceID);
                                    if (oTraceI != null && oTraceI.Attributes.Count() > 0)
                                    {
                                        var bButtons = oTraceI.AttributeI("bShowButtons").sValue;
                                        var sTraceVisibleGroup = oTraceI.AttributeI("sVisibleGroup").sValue;
                                        var sTraceLockGroup = oTraceI.AttributeI("sLockGroup").sValue;
                                        var sTraceSummaryGroup = oTraceI.AttributeI("sSummaryGroup").sValue;
                                        if (!string.IsNullOrEmpty(sTraceVisibleGroup))
                                        {
                                            sGroupName = sTraceVisibleGroup;
                                        }
                                        if (!string.IsNullOrEmpty(sTraceLockGroup))
                                        {
                                            sLockGroup = sTraceLockGroup;
                                        }
                                        if (!string.IsNullOrEmpty(bButtons) && bButtons.ToLower() == "true")
                                        {
                                            var sCurrentTrace = oTraceI.AttributeI("sName").sValue;
                                            var iCount = sCurrentTrace.Count(m => m == '_');
                                            XID1Click o1Click = new XID1Click();
                                            o1Click.BOID = oTraceI.BOD.BOID;
                                            //     XIDXI oXID = new XIDXI();
                                            //oXID.sOrgDatabase = sCoreDB;
                                            var oDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBODCopy.iDataSourceXIGUID.ToString());
                                            o1Click.sSwitchDB = oDataSource.sName;
                                            o1Click.Query = "select * from refValidTrace_T where sname like '" + sCurrentTrace + "%' and FKiBOID=" + oBODCopy.BOID + " and FKiAttrID=" + iAttrID;
                                            var Response = o1Click.OneClick_Run();
                                            if (Response != null && Response.Count() > 0)
                                            {
                                                foreach (var BOI in Response.Values.ToList())
                                                {
                                                    var sNextTrace = BOI.AttributeI("sName").sValue;
                                                    var iNextCount = sNextTrace.Count(m => m == '_');
                                                    if (sNextTrace != sCurrentTrace && iNextCount == (iCount + 1))
                                                    {
                                                        var index = sNextTrace.LastIndexOf("_");
                                                        var Code = sNextTrace.Substring(index, sNextTrace.Length - index);
                                                        if (!string.IsNullOrEmpty(Code))
                                                        {
                                                            List<CNV> oParam = new List<CNV>();
                                                            oParam.Add(new CNV { sName = "sName", sValue = Code.Replace("_", "") });
                                                            oParam.Add(new CNV { sName = "FKiBOID", sValue = oBODCopy.BOID.ToString() });
                                                            oParam.Add(new CNV { sName = "FKiAttrID", sValue = iAttrID.ToString() });
                                                            var oStageI = oXII.BOI("RefTraceStage", null, null, oParam);
                                                            if (oStageI != null && oStageI.Attributes.Count() > 0)
                                                            {
                                                                var Name = oStageI.AttributeI("sDescription").sValue;
                                                                var FKiXILinkID = oStageI.AttributeI("FKiXILinkID").sValue;
                                                                var FKiXILinkIDGUID = oStageI.AttributeI("FKiXILinkIDXIGUID").sValue;
                                                                int iXILinkID = 0;
                                                                int.TryParse(FKiXILinkID, out iXILinkID);
                                                                Guid XILinkGUID = Guid.Empty;
                                                                Guid.TryParse(FKiXILinkIDGUID, out XILinkGUID);
                                                                if (Visualisations.Count() == 0)
                                                                {
                                                                    Visualisations.Add(new XIVisualisation() { Name = "tracebuttons" });
                                                                    Visualisations.FirstOrDefault().NVs = new List<XIVisualisationNV>();
                                                                }
                                                                var Vis = Visualisations.FirstOrDefault().NVs.Where(m => m.sName.ToLower() == Name.ToLower()).FirstOrDefault();
                                                                if (Vis == null)
                                                                {
                                                                    if (XILinkGUID != null && XILinkGUID != Guid.Empty)
                                                                    {
                                                                        Visualisations.FirstOrDefault().NVs.Add(new XIVisualisationNV { sName = Name + "-" + sAttrName, sType = "TraceBtn", sValue = XILinkGUID + "_" + oStageI.AttributeI("iStatusValue1").sValue });
                                                                    }
                                                                    else if (iXILinkID > 0)
                                                                    {
                                                                        Visualisations.FirstOrDefault().NVs.Add(new XIVisualisationNV { sName = Name + "-" + sAttrName, sType = "TraceBtn", sValue = iXILinkID + "_" + oStageI.AttributeI("iStatusValue1").sValue });
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(sInstanceID))
                    {

                        var oGUIDParams1 = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        var siInstanceID = oGUIDParams1.NMyInstance.Where(k => k.Key.ToLower() == "{XIP|1ClickID}".ToLower()).FirstOrDefault();
                        if (!string.IsNullOrEmpty(siInstanceID.Key))
                        {
                            if (!string.IsNullOrEmpty(siInstanceID.Value.sValue))
                            {
                                sInstanceID = siInstanceID.Value.sValue.ToString();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sInstanceID))
                    {
                        //if(!string.IsNullOrEmpty(ActiveFK))
                        //{
                        //    List<CNV> oNVParams = new List<CNV>();
                        //    CNV oCNV = new CNV();
                        //    oCNV.sName = ActiveFK;
                        //    oCNV.sValue = iInstanceID.ToString();
                        //    oNVParams.Add(oCNV);
                        //    oBOI = oXII.BOI(sBOName, null, sGroupName, oNVParams);
                        //}
                        //else
                        //{
                        Guid InsGUID = Guid.Empty;
                        Guid.TryParse(sInstanceID, out InsGUID);
                        if (iDataSourceID > 0)
                        {
                            oXII.iSwitchDataSrcID = iDataSourceID;
                        }
                        List<CNV> oWhrParams = new List<CNV>();
                        if (!string.IsNullOrEmpty(sRowAttr))
                        {
                            var oAttrD = oBOD.Attributes.Values.ToList().Where(m => m.LabelName.ToLower() == sRowAttr.ToLower()).FirstOrDefault();
                            if (oAttrD != null)
                            {
                                oWhrParams.Add(new CNV { sName = oAttrD.Name, sValue = sInstanceID });
                                oBOI = oXII.BOI(sBOName, null, sGroupName, oWhrParams);
                            }
                        }
                        else
                        {
                            oBOI = oXII.BOI(sBOName, sInstanceID, oAccordGroupNames[0]);
                            oAccordGroupAttr[oAccordGroupNames[0]] = oBOI.Attributes.Keys.ToList();
                            if (bShowAccordian)
                            {
                                for (int i = 1; i < oAccordGroupNames.Count(); i++)
                                {
                                    var oBOIAcc = oXII.BOI(sBOName, sInstanceID, oAccordGroupNames[i]);
                                    oAccordGroupAttr[oAccordGroupNames[i]] = oBOIAcc.Attributes.Keys.ToList();
                                    oBOI.Attributes = oBOI.Attributes.Concat(oBOIAcc.Attributes).ToDictionary(x => x.Key, x => x.Value);
                                }
                            }
                        }

                        //}

                        if (oBOI != null)
                        {
                            if (oBOI.Attributes.Values.Count() > 0)
                            {
                                if (!oBOI.Attributes.ContainsKey("xiguid"))
                                {
                                    if (InsGUID != null && InsGUID != Guid.Empty)
                                    {
                                        oBOI.SetAttribute("xiguid", InsGUID.ToString());
                                    }
                                }
                            }
                            oBOI.iBODID = oBOI.BOD.BOID;
                            //oBOI.ResloveFKFields();
                            //oBOI.FormatAttrs();
                        }
                        var DefaultAttrs = oBOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.DefaultValue)).ToList();
                        if (oBOI != null && DefaultAttrs != null && DefaultAttrs.Count() > 0)
                        {
                            foreach (var attr in DefaultAttrs)
                            {
                                var oAttrI = oBOI.AttributeI(attr.Name);
                                if (oAttrI != null)
                                {
                                    if (string.IsNullOrEmpty(oAttrI.sValue))
                                    {
                                        if (attr.TypeID == 150 || attr.TypeID == 110)
                                        {
                                            var DefaultVal = Utility.GetDefaultDateResolvedValue(attr.DefaultValue, attr.Format);
                                            oBOI.AttributeI(attr.Name).sValue = DefaultVal;
                                        }
                                        else
                                        {
                                            oBOI.AttributeI(attr.Name).sValue = attr.DefaultValue;
                                        }
                                    }
                                }
                            }
                        }
                        var FormatAttrs = oBOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.Format) /*|| !string.IsNullOrEmpty(m.sDBDateFormat)*/).ToList(); //IF your attribute is date time or date it is mandatory to give this format dd-MMM-yyyy in this column sDBDateFormat 
                        if (oBOI != null && FormatAttrs != null && FormatAttrs.Count() > 0)
                        {
                            foreach (var attr in FormatAttrs)
                            {
                                var oAttrI = oBOI.AttributeI(attr.Name);
                                if (oAttrI != null)
                                {
                                    if (!string.IsNullOrEmpty(oAttrI.sValue))
                                    {
                                        if (attr.TypeID == 150 || attr.TypeID == 110)
                                        {
                                            if (!string.IsNullOrEmpty(attr.Format) && attr.iDateType != 0)
                                            {
                                                var DefaultVal = Utility.GetDefaultDateResolvedValue(oAttrI.sValue, attr.Format);
                                                oBOI.AttributeI(attr.Name).sValue = DefaultVal;
                                            }
                                            else
                                            {
                                                var DefaultVal = Utility.GetDefaultDateResolvedValue(oAttrI.sValue, attr.sDBDateFormat);
                                                oBOI.AttributeI(attr.Name).sValue = DefaultVal;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var oFileAttrs = oBODCopy.Attributes.Values.Where(m => m.FKiFileTypeID > 0).ToList();
                        foreach (var oAttr in oFileAttrs)
                        {
                            List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                            var sName = oAttr.Name;
                            var sFileID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sFileID))
                            {
                                List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                                var NewFileID = sFileID.Split(',').ToList();
                                foreach (var item in NewFileID)
                                {
                                    if (!string.IsNullOrEmpty(item.ToString()))
                                    {
                                        XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                                        XIInfraDocs oDocs = new XIInfraDocs();
                                        if (oAttr.FKiFileTypeID == 1)
                                        {
                                            int iDocID = 0;
                                            int.TryParse(item, out iDocID);
                                            if (iDocID > 0)
                                            {
                                                oDocs.ID = Convert.ToInt32(item);
                                                var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                                if (sImagePathDetails != null)
                                                {
                                                    ImagePathDetails.AddRange(sImagePathDetails);
                                                }
                                            }
                                            else
                                            {
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
                                        else
                                        {
                                            int iDocID = 0;
                                            int.TryParse(item, out iDocID);
                                            if (iDocID > 0)
                                            {
                                                oDocs.ID = Convert.ToInt32(item);
                                                var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                                if (sImagePathDetails != null)
                                                {
                                                    ImagePathDetails.AddRange(sImagePathDetails);
                                                }
                                            }
                                            else
                                            {
                                                oXIDocTypes.ID = oAttr.FKiFileTypeID;
                                                var oXIDocDetails = (XIInfraDocTypes)oXIDocTypes.Get_FileDocTypes().oResult;
                                                if (oXIDocDetails != null)
                                                {
                                                    int pos = item.LastIndexOf("\\") + 1;
                                                    string sFileName = item.Substring(pos, item.Length - pos);
                                                    if (!string.IsNullOrEmpty(sFileName) && sFileName.ToString().Split('.').Count() > 0)
                                                    {
                                                        var PathType = "/Files/" + sFileName.Split('.').Last().Replace("~", "");///Files/jpg
                                                        oXIDocDetails.Path = PathType;//oXIDocDetails.Path.Replace("~", "");
                                                        sPDFPathDetails.Add(new XIDropDown { Expression = oXIDocDetails.Path + "//" + item, text = sFileName });
                                                        ImagePathDetails.AddRange(sPDFPathDetails);
                                                    }
                                                    else
                                                    {
                                                        oXIDocDetails.Path = oXIDocDetails.Path.Replace("~", "");
                                                        sPDFPathDetails.Add(new XIDropDown { Expression = oXIDocDetails.Path + "//" + item, text = sFileName });
                                                        ImagePathDetails.AddRange(sPDFPathDetails);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            oBOI.Attributes.Values.Where(x => x.sName.ToLower() == sName.ToLower()).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                            //oBOD.Attributes.Values.Where(s => s.Name == sName).Where(s => s.ID == oAttr.ID).Select(s => { s.ImagePathDetails = ImagePathDetails; return s; }).ToList();
                        }
                        oBOI.BOD = oBOD;
                        if (oBODCopy.Name == "XIAlgorithmLines_T" && sGroupName == "Create")
                        {
                            oBOI.Attributes.Values.Where(x => x.sName != "sIndent" && x.sName != "iOrder").ToList().ForEach(m => m.sValue = "");
                            if (oParams.Where(m => m.sName == "sOperator").Select(m => m.sValue).FirstOrDefault() == "+")
                            {
                                oBOI.Attributes.Values.Where(x => x.sName == "iOrder").ToList().ForEach(m => m.sValue = (Convert.ToInt32(m.sValue) + 1).ToString());
                            }
                            else if (oParams.Where(m => m.sName == "sOperator").Select(m => m.sValue).FirstOrDefault() == "-")
                            {
                                oBOI.Attributes.Values.Where(x => x.sName == "iOrder").ToList().ForEach(m => m.sValue = (Convert.ToInt32(m.sValue) - 1).ToString());
                            }
                        }
                        if (oBODCopy.bUID)
                        {
                            oBOI.SetAttribute("XIGUID", sInstanceID);
                        }
                        oBODisplay.BOInstance = oBOI;
                        //var DependencyFieldsList = oBODCopy.Attributes.Values.Where(m => !string.IsNullOrEmpty(m.sDepBOFieldIDs)).ToList();
                        var DependencyFieldsList = oBODCopy.Attributes.Values.Where(m => !string.IsNullOrEmpty(m.FKiDepBOFieldIDsXIGUID)).ToList();
                        if (DependencyFieldsList != null && DependencyFieldsList.Count > 0)
                        {
                            foreach (var DependecyField in DependencyFieldsList)
                            {
                                if (!string.IsNullOrEmpty(DependecyField.sEventHandler) && /*DependecyField.sEventHandler.Contains("onload") &&*/ oBOI.Attributes.ContainsKey(DependecyField.Name) && !string.IsNullOrEmpty(oBOI.Attributes[DependecyField.Name].sValue))
                                {
                                    var ParentIID = oBOI.Attributes[DependecyField.Name].sValue;
                                    //var sDepBOFieldIDs = DependecyField.sDepBOFieldIDs.Split(',');
                                    var sDepBOFieldIDs = DependecyField.FKiDepBOFieldIDsXIGUID.Split(',');
                                    foreach (var Field in sDepBOFieldIDs)
                                    {
                                        int iFieldID = 0;
                                        Guid DepGuid = new Guid();
                                        if (!string.IsNullOrEmpty(Field))
                                        {
                                            XIDAttribute oFieldD = new XIDAttribute();
                                            if (Guid.TryParse(Field, out DepGuid))
                                            {
                                                oFieldD = oBODCopy.Attributes.Values.Where(m => m.XIGUID == DepGuid).FirstOrDefault();
                                            }
                                            else
                                            {
                                                int.TryParse(Field, out iFieldID);
                                                oFieldD = oBODCopy.Attributes.Values.Where(m => m.ID == iFieldID).FirstOrDefault();
                                            }
                                            if (oFieldD != null && oBOD.Attributes.ContainsKey(oFieldD.Name) && !string.IsNullOrEmpty(oBOD.Attributes[oFieldD.Name].sFKBOName))
                                            {
                                                var sBOName = oBOD.Attributes[oFieldD.Name].sFKBOName;
                                                Dictionary<string, object> Params = new Dictionary<string, object>();
                                                Params["Name"] = sBOName;
                                                string sSelectFields = string.Empty;
                                                sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType";
                                                var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                                                //var FKBOD = Load_BO(FKBO.Name, FKBO.BOID);
                                                //var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                                                //sBODataSource = oXID.GetBODataSource(FKBOD.iDataSource, oBOD.FKiApplicationID);
                                                if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                                                {
                                                    var oResult = oXID.Get_DependencyAutoCompleteSearchList(Field, sGUID, FKBOD.Name, ParentIID, DependecyField.sFKBOName, 10).oResult;
                                                    var DDL = (List<XIDFieldOptionList>)oResult;
                                                    if (DDL != null)
                                                    {
                                                        List<XIDropDown> FKDDL = new List<XIDropDown>();
                                                        FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                                        if (oBOI.Attributes.ContainsKey(oFieldD.Name.ToLower()))
                                                        {
                                                            oBOI.Attributes[oFieldD.Name.ToLower()].FieldDDL = FKDDL;
                                                            oBOI.BOD.Attributes[oFieldD.Name.ToLower()].FieldDDL = FKDDL;
                                                            oBOI.BOD.Attributes[oFieldD.Name.ToLower()].bIsHidden = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Load bo instance with group
                        oBOI.BOD = oBOD;
                        if (!string.IsNullOrEmpty(sGroupName))
                        {
                            oBOI.LoadBOI(sGroupName);
                            oBODisplay.BOInstance = oBOI;
                            Dictionary<string, XIIAttribute> attributes = new Dictionary<string, XIIAttribute>();
                            foreach (var item in oBOI.Attributes)
                            {
                                if (!string.IsNullOrEmpty(item.Value.sDefaultDate))
                                {
                                    item.Value.sValue = Utility.GetDefaultDateResolvedValue(item.Value.sDefaultDate, item.Value.Format);
                                }
                                attributes.Add(item.Key, item.Value);
                            }
                            oAccordGroupAttr[oAccordGroupNames[0]] = oBOI.Attributes.Keys.ToList();
                            var DefaultAttrs = oBOD.Attributes.Values.ToList().Where(m => !string.IsNullOrEmpty(m.DefaultValue)).ToList();
                            if (DefaultAttrs != null && DefaultAttrs.Count() > 0)
                            {
                                foreach (var attr in DefaultAttrs)
                                {
                                    var oAttrI = oBOI.AttributeI(attr.Name);
                                    if (oAttrI != null)
                                    {
                                        if (!string.IsNullOrEmpty(oAttrI.sValue))
                                        {
                                            if (attr.TypeID == 150 || attr.TypeID == 110)
                                            {
                                                var DefaultVal = Utility.GetDefaultDateResolvedValue(attr.DefaultValue, attr.Format);
                                                oBOI.AttributeI(attr.Name).sValue = DefaultVal;
                                            }
                                            else
                                            {
                                                oBOI.AttributeI(attr.Name).sValue = attr.DefaultValue;
                                            }
                                        }
                                    }
                                }
                            }
                            //var WatchParam2 = oParams.Where(m => m.sName.ToLower().Contains("watchparam2".ToLower())).ToList();
                            var sGroupOverride = oCache.Get_Paramobject(sSessionID, sGUID, null, "{XIP|sGroupOverride}");
                            if (!string.IsNullOrEmpty(sGroupOverride.sValue))
                            {
                                oBOI.LoadBOI(sGroupOverride.sValue);
                                oBODisplay.BOInstance = oBOI;
                                foreach (var item in oBOI.Attributes)
                                {
                                    if (!string.IsNullOrEmpty(item.Value.sDefaultDate))
                                    {
                                        item.Value.sDefaultDate = Utility.GetDefaultDateResolvedValue(item.Value.sDefaultDate, item.Value.Format);
                                    }
                                    attributes.Add(item.Key, item.Value);
                                }
                                oBODisplay.BOInstance.Attributes = attributes;
                            }
                            var value = oCache.Get_Paramobject(sSessionID, sGUID, null, "{XIP|sTransCode}");
                            oBODisplay.BOInstance.Attributes.Values.Where(m => m.sName.ToLower() == "sTransCode".ToLower()).ToList().ForEach(m => m.sValue = value.sValue);
                        }
                        if (oBODisplay.BOInstance != null && !oBODisplay.BOInstance.Attributes.ContainsKey("xiguid") && oBOI.BOD.Attributes.ContainsKey("xiguid"))
                        {
                            var GUIDAttr = oBOI.BOD.AttributeD("xiguid");
                            oBODisplay.BOInstance.Attributes.Add(GUIDAttr.Name, new XIIAttribute { sName = GUIDAttr.Name, Format = GUIDAttr.Format, sDefaultDate = GUIDAttr.sDefaultDate, iOneClickID = GUIDAttr.iOneClickID, bDirty = false });
                        }
                    }
                    string sVisualisation = oParams.Where(m => m.sName.ToLower() == "Visualisation".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    Guid VisualisationGUID = Guid.Empty;
                    int VisualisationID = 0;
                    int.TryParse(sVisualisation, out VisualisationID);
                    Guid.TryParse(sVisualisation, out VisualisationGUID);
                    XIVisualisation oXIDVisual = new XIVisualisation();
                    var useLayoutGUID = "";
                    if (!string.IsNullOrEmpty(sVisualisation) || VisualisationID != 0 && VisualisationGUID != Guid.Empty)
                    {
                        string sVisualisationID = "";
                        if (VisualisationID != 0)
                        {
                            sVisualisationID = VisualisationID.ToString();
                            sVisualisation = "";
                        }
                        if (VisualisationGUID != Guid.Empty)
                        {
                            sVisualisationID = VisualisationGUID.ToString();
                            sVisualisation = "";
                        }
                        var oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisualisation, sVisualisationID);
                        if (oXIvisual != null && oXIvisual.XiVisualID > 0)
                        {
                            oXIDVisual = (XIVisualisation)oXIvisual.GetCopy();
                            if (oXIDVisual.XiVisualisationNVs != null)
                            {
                                useLayoutGUID = oXIDVisual.XiVisualisationNVs.Where(x => x.sName.ToLower() == "UseLayoutGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                            }
                        }
                    }
                    var FKAttributes = oBODCopy.Attributes.Where(m => m.Value.FKiType > 0 && !string.IsNullOrEmpty(m.Value.sFKBOName) && (m.Value.iOneClickID > 0 || (m.Value.iOneClickIDXIGUID != null && m.Value.iOneClickIDXIGUID != Guid.Empty))).ToList();
                    foreach (var item in FKAttributes)
                    {
                        if (item.Value.iOneClickID > 0 || (item.Value.iOneClickIDXIGUID != null && item.Value.iOneClickIDXIGUID != Guid.Empty))
                        {
                            string sBODataSource = string.Empty;
                            var sBOName = item.Value.sFKBOName;
                            Dictionary<string, object> Params = new Dictionary<string, object>();
                            Params["Name"] = sBOName;
                            string sSelectFields = string.Empty;
                            sSelectFields = "Name,BOID,iDataSource,sSize,TableName,sPrimaryKey,sType,iDataSourceXIGUID,XIGUID";
                            var FKBOD = Connection.Select<XIDBO>("XIBO_T_N", Params, sSelectFields).FirstOrDefault();
                            //var FKBOD = Load_BO(FKBO.Name, FKBO.BOID);
                            //var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                            sBODataSource = oXID.GetBODataSource(FKBOD.iDataSourceXIGUID.ToString(), oBODCopy.FKiApplicationID);
                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOSize = FKBOD.sSize;
                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOName = FKBOD.Name;
                            if (FKBOD.sSize == "10")//maximum number of results in dropdown -- To Do
                            {
                                var Con = new XIDBAPI(sBODataSource);
                                if (FKBOD.sType != null && FKBOD.sType.ToLower() == "reference")
                                {
                                    string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                    XICacheInstance oGUIDParams = new XICacheInstance();
                                    if (!string.IsNullOrEmpty(useLayoutGUID) && useLayoutGUID == "true")
                                    {
                                        oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sLayoutGuid, null);
                                    }
                                    else
                                    {
                                        oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                    }
                                    List<CNV> nParms = new List<CNV>();
                                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                    var oResult = oXID.Get_AutoCompleteList(suid, "", nParms, 0, "", iOrgID);
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
                                else
                                {
                                    string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                    XICacheInstance oGUIDParams = new XICacheInstance();
                                    if (!string.IsNullOrEmpty(useLayoutGUID) && useLayoutGUID == "true")
                                    {
                                        oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sLayoutGuid, null);
                                    }
                                    else
                                    {
                                        oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                    }
                                    List<CNV> nParms = new List<CNV>();
                                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                    var oResult = oXID.Get_AutoCompleteList(suid, "", nParms, 0, "", iOrgID);
                                    List<XIDropDown> FKDDL = new List<XIDropDown>();
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                        FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                    }
                                    if (oBOI.Attributes.ContainsKey(item.Value.Name.ToLower()))
                                    {
                                        oBOI.BOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                        oBOI.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                    }
                                }
                            }
                        }
                    }

                    var FKAttribute = oBODCopy.Attributes.Where(m => m.Value.FKiType > 0 && string.IsNullOrEmpty(m.Value.sFKBOName) && (m.Value.iOneClickID > 0 || (m.Value.iOneClickIDXIGUID != null && m.Value.iOneClickIDXIGUID != Guid.Empty))).ToList();
                    foreach (var item in FKAttribute)
                    {
                        if (item.Value.iOneClickID > 0 || (item.Value.iOneClickIDXIGUID != null || item.Value.iOneClickIDXIGUID != Guid.Empty))
                        {
                            string sBODataSource = string.Empty;
                            var i1ClickID = item.Value.iOneClickID;
                            XID1Click OneClickDef = new XID1Click();
                            XID1Click o1Def = new XID1Click();
                            if (item.Value.iOneClickID > 0 || (item.Value.iOneClickIDXIGUID != null || item.Value.iOneClickIDXIGUID != Guid.Empty))
                            {
                                if (item.Value.iOneClickIDXIGUID != null && item.Value.iOneClickIDXIGUID != Guid.Empty)
                                {
                                    o1Def = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, item.Value.iOneClickIDXIGUID.ToString());
                                }
                                else if (item.Value.iOneClickID > 0)
                                {
                                    o1Def = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, item.Value.iOneClickID.ToString());
                                }
                            }
                            XIDBO oBODef = new XIDBO();
                            if (o1Def.BOID > 0 || (o1Def.BOIDXIGUID != null || o1Def.BOIDXIGUID != Guid.Empty))
                            {
                                oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1Def.BOIDXIGUID.ToString());
                            }
                            else if (o1Def.BOID > 0)
                            {
                                oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1Def.BOID.ToString());
                            }
                            var iBOID = o1Def.BOID;

                            sBODataSource = oBODef.iDataSource == 0 ? "" : oXID.GetBODataSource(oBODef.iDataSourceXIGUID.ToString(), oBODef.FKiApplicationID);
                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOSize = oBODef.sSize;
                            oBOD.Attributes[item.Value.Name.ToLower()].sFKBOName = oBODef.Name;
                            if (oBODef.sSize == "10")//maximum number of results in dropdown -- To Do
                            {
                                var Con = new XIDBAPI(sBODataSource);
                                if (oBODef.sType != null && oBODef.sType.ToLower() == "reference")
                                {
                                    string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                    List<CNV> nParms = new List<CNV>();
                                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                    var oResult = oXID.Get_AutoCompleteList(suid, "", nParms, 0, "", iOrgID);
                                    List<XIDropDown> FKDDL = new List<XIDropDown>();
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                        FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                    }
                                    if (oBOI.Attributes.ContainsKey(item.Value.Name.ToLower()))
                                    {
                                        oBOI.BOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                        oBOI.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                    }
                                }
                                else
                                {
                                    string suid = "1click_" + Convert.ToString(item.Value.iOneClickIDXIGUID);
                                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                    List<CNV> nParms = new List<CNV>();
                                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                    var oResult = oXID.Get_AutoCompleteList(suid, "", nParms, 0, "", iOrgID);
                                    List<XIDropDown> FKDDL = new List<XIDropDown>();
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                        FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                    }
                                    if (oBOI.Attributes.ContainsKey(item.Value.Name.ToLower()))
                                    {
                                        oBOI.BOD.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                        oBOI.Attributes[item.Value.Name.ToLower()].FieldDDL = FKDDL;
                                    }
                                }
                            }
                        }
                    }
                    oBOI.DependentFields = new List<string>();
                    var MergeFields = oBODCopy.Attributes.Values.Where(m => m.iMergeFieldID > 0).ToList();
                    var DependentFields = oBODCopy.Attributes.Values.Where(m => m.iDependentFieldID > 0).ToList();
                    if (MergeFields != null && MergeFields.Count() > 0)
                    {
                        foreach (var item in MergeFields)
                        {
                            var oMergeBOD = oBODCopy.Attributes.Values.Where(m => m.ID == item.iMergeFieldID).FirstOrDefault();
                            if (oBOI.Attributes.Values.Select(m => m.sName).ToList().Contains(oMergeBOD.Name))
                            {
                                oBOI.DependentFields.Add(oMergeBOD.Name);
                            }
                        }
                    }
                    if (DependentFields != null && DependentFields.Count() > 0)
                    {
                        foreach (var item in DependentFields)
                        {
                            var oDependentBOD = oBODCopy.Attributes.Values.Where(m => m.ID == item.iDependentFieldID).FirstOrDefault();
                            if (oBOI.Attributes.Values.Select(m => m.sName).ToList().Contains(oDependentBOD.Name))
                            {
                                oBOI.DependentFields.Add(oDependentBOD.Name);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sGroupName))
                    {
                        if (bShowAccordian)
                        {
                            List<string> groupAttr = new List<string>();
                            foreach (List<string> attrs in oAccordGroupAttr.Values)
                            {
                                groupAttr.AddRange(attrs.ConvertAll(x => x.ToLower()));
                            }
                            if (groupAttr.Count() > 0)
                            {
                                oBODisplay.BOInstance.Attributes.Values.Where(m => groupAttr.Any(n => n == m.sName.ToLower())).ToList().ForEach(m => m.bDirty = true);
                            }
                        }
                        else
                        {
                            var GroupFields = oBOI.BOD.GroupD(sGroupName).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                            var oGrpD = oBOI.BOD.GroupD(sGroupName);
                            var oAllGroupFields = Utility.GetBOGroupFields(GroupFields, oGrpD.bIsCrtdBy, oGrpD.bIsCrtdWhn, oGrpD.bIsUpdtdBy, oGrpD.bIsUpdtdWhn);
                            if (!string.IsNullOrEmpty(oAllGroupFields))
                            {
                                var oGrpFields = oAllGroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(m => m.ToLower());
                                oBODisplay.BOInstance.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(m => m.bDirty = true);
                            }

                        }

                    }
                    else if (oBOI.Attributes.Values.Count() > 0)
                    {
                        oBODisplay.BOInstance.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                    }
                    //oBODisplay = oXIAPI.GetFormData(sBOName, sGroupName, iInstanceID, string.Empty, iUserID, sOrgName, sDatabase, null);

                    List<XIVisualisation> oXIVisualisations = new List<XIVisualisation>();
                    string DisplayMode = string.Empty;

                    if (oXIDVisual != null && oXIDVisual.XiVisualisationNVs != null && oXIDVisual.XiVisualisationNVs.Count() > 0)
                    {
                        if (oXIDVisual.XiVisualisationNVs != null && oXIDVisual.XiVisualisationNVs.ToList().Where(m => m.sName.ToLower() == "fkipolicyid".ToLower()).Select(m => m.sValue).FirstOrDefault() != null)
                        {
                            var sPolicyVisualValue = oXIDVisual.XiVisualisationNVs.ToList().Where(m => m.sName.ToLower() == "fkipolicyid".ToLower()).Select(m => m.sValue).FirstOrDefault();
                            var iACPolicyID = oCache.Get_ParamVal(sSessionID, sGUID, null, sPolicyVisualValue);
                            if (!string.IsNullOrEmpty(iACPolicyID))
                            {
                                var oPolDef = oXII.BOI("ACPolicy_T", iACPolicyID.ToString());
                                if (!string.IsNullOrEmpty(oPolDef.Attributes["sPolicyNo"].sValue))
                                {
                                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sPolicyNo}", oPolDef.Attributes["sPolicyNo"].sValue, null, null);
                                }
                            }
                        }
                        if (oXIDVisual != null && oXIDVisual.XiVisualisationNVs != null)
                        {

                            foreach (var oVisualisation in oXIDVisual.XiVisualisationNVs)
                            {
                                if (oVisualisation.sName.ToLower() == "displaymode")
                                {
                                    DisplayMode = oVisualisation.sValue;
                                }
                                if (oVisualisation.sName != null && oVisualisation.sName.ToLower() == "LockGroup".ToLower() && string.IsNullOrEmpty(sLockGroup))
                                {
                                    sLockGroup = oVisualisation.sValue;
                                }
                                if (oVisualisation.sName != null && oVisualisation.sName.ToLower() == "HiddenGroup".ToLower() && string.IsNullOrEmpty(sHiddenGroup))
                                {
                                    sHiddenGroup = oVisualisation.sValue;
                                }
                                if (oVisualisation.sValue.StartsWith("xi."))
                                {
                                    XIDScript oXIScript = new XIDScript();
                                    oXIScript.sScript = oVisualisation.sValue.ToString();
                                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oVisualisation.sValue = (string)oCR.oResult;
                                    }
                                }
                                else if (oVisualisation.sValue.Contains("xi.s") || oVisualisation.sValue.Contains("xi.r"))
                                {
                                    XIDScript oXIScript = new XIDScript();
                                    oXIScript.sScript = oVisualisation.sValue.ToString();
                                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oVisualisation.sValue = (string)oCR.oResult;
                                    }
                                }
                                if (oVisualisation.sValue != null && oVisualisation.sValue.IndexOf("{XIP") >= 0)
                                {
                                    var oVisualTypeID = oBODCopy.Attributes.Values.ToList().Where(m => m.Name.ToLower() == oVisualisation.sName.ToLower()).Select(m => m.TypeID).FirstOrDefault();
                                    if (oVisualTypeID == 150)
                                    {
                                        var sPreviousVisualValue = oVisualisation.sValue;
                                        var oVisualSplit = oVisualisation.sValue.Contains("+") ? oVisualisation.sValue.Split(new string[] { " + " }, StringSplitOptions.RemoveEmptyEntries).ToList() : oVisualisation.sValue.Contains("-") ? oVisualisation.sValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                                        oVisualisation.sValue = oVisualSplit[0];
                                        oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                                        oVisualisation.sValue = sPreviousVisualValue.Replace(sPreviousVisualValue.Split('+').FirstOrDefault(), oVisualisation.sValue);
                                        oVisualisation.sValue = Utility.GetDateResolvedValue(oVisualisation.sValue, XIConstant.Date_Format);
                                        if (oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) || string.IsNullOrEmpty(oVisualisation.sValue))
                                        {
                                            oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                        }
                                    }
                                    else
                                    {
                                        if (oXIDVisual.XiVisualisationNVs.Any(x => x.sName.ToLower() == "UseLayoutGUID".ToLower() && x.sValue == "true"))
                                        {
                                            if (!string.IsNullOrEmpty(oVisualisation.sValue) && oVisualisation.sValue.ToLower().StartsWith("merge-"))
                                            {
                                                var sMergeParam = oVisualisation.sValue.Replace("merge-", "");
                                                oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sLayoutGuid, null, sMergeParam);
                                            }
                                            else
                                                oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sLayoutGuid, null, oVisualisation.sValue);
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(oVisualisation.sValue) && oVisualisation.sValue.ToLower().StartsWith("merge-"))
                                            {
                                                var sMergeParam = oVisualisation.sValue.Replace("merge-", "");
                                                oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, sMergeParam);
                                            }
                                            else
                                            {
                                                oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                                            }
                                            if (oVisualisation.sValue != null && oVisualisation.sValue.ToLower().Contains("option value"))
                                            {
                                                oVisualTypeID = 10;
                                            }
                                        }
                                        if (oVisualTypeID == 10)
                                        {
                                            var sPreviousVisualValue = oVisualisation.sValue;
                                            if (!string.IsNullOrEmpty(sPreviousVisualValue))
                                            {
                                                var oVisualSplit = oVisualisation.sValue.Contains("+") ? oVisualisation.sValue.Split(new string[] { " + " }, StringSplitOptions.RemoveEmptyEntries).ToList() : oVisualisation.sValue.Contains("-") ? oVisualisation.sValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                                                var oVisRemoveSpecialChar = oVisualisation.sValue.Replace(" + ", "");
                                                if (oVisualSplit.Count > 0)
                                                {
                                                    if (oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) || string.IsNullOrEmpty(oVisualisation.sValue))
                                                    {
                                                        oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisRemoveSpecialChar, bDirty = true };
                                                        oBOI.Attributes[oVisualisation.sName.ToLower()].FieldDDL1 = oVisualSplit;
                                                    }
                                                }
                                            }
                                        }
                                        if (oBODisplay.BOInstance != null && oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) && string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName.ToLower()).sValue))
                                        {
                                            oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()] = new XIIAttribute { sName = oVisualisation.sName.ToLower(), sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                        }
                                        else if (!string.IsNullOrEmpty(oVisualisation.sType) && oVisualisation.sType.ToLower() == "override" && !string.IsNullOrEmpty(oVisualisation.sValue))
                                        {
                                            oBODisplay.BOInstance.Attributes[oVisualisation.sName] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                        }
                                    }
                                    //if (oVisualisation.sValue.Contains("+") || oVisualisation.sValue.Contains("-"))
                                    //{
                                    //    var sPreviousVisualValue = oVisualisation.sValue;
                                    //    var oVisualSplit = oVisualisation.sValue.Contains("+") ? oVisualisation.sValue.Split(new string[] { " + " }, StringSplitOptions.RemoveEmptyEntries).ToList() : oVisualisation.sValue.Contains("-") ? oVisualisation.sValue.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                                    //    oVisualisation.sValue = oVisualSplit[0];
                                    //    int iAddDays = 0;
                                    //    if (oVisualSplit.Count() > 1)
                                    //    {
                                    //        iAddDays = Convert.ToInt32(oVisualSplit[1]);
                                    //    }
                                    //    string sFormat = "yyyy-MM-dd";
                                    //    oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                                    //    DateTime oVisualDate = Convert.ToDateTime(oVisualisation.sValue);
                                    //    if (sPreviousVisualValue.Contains("+"))
                                    //    {
                                    //        oVisualisation.sValue = oVisualDate.AddDays(iAddDays).Date.ToString(sFormat);
                                    //    }
                                    //    else
                                    //    {
                                    //        oVisualisation.sValue = oVisualDate.AddDays(-iAddDays).Date.ToString(sFormat);
                                    //    }
                                    //    if (oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName) && string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue))
                                    //    {
                                    //        oBODisplay.BOInstance.Attributes[oVisualisation.sName] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                                    //    if (oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName) && string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue))
                                    //    {
                                    //        oBODisplay.BOInstance.Attributes[oVisualisation.sName] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                    //    }
                                    //}
                                }
                                else
                                {
                                    if (oVisualisation.sName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || oVisualisation.sName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                                    {
                                        if (!string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue))
                                        {
                                            string sValue = oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue;
                                            string sFormat = XIConstant.DateTime_Format; //"dd-MMM-yyyy HH:mm";
                                            string sFormattedValue = String.Format("{0:" + sFormat + "}", Convert.ToDateTime(sValue));
                                            oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue = sFormattedValue;
                                            oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sPreviousValue = sFormattedValue;
                                        }
                                        oVisualisation.sValue = Utility.GetDateResolvedValue(oVisualisation.sValue, XIConstant.DateTime_Format);
                                    }
                                    else
                                    {
                                        var oVisualTypeID = oBODCopy.Attributes.Values.ToList().Where(m => m.Name.ToLower() == oVisualisation.sName.ToLower()).Select(m => m.TypeID).FirstOrDefault();
                                        if (oVisualTypeID == 150)
                                        {
                                            string sFormat = XIConstant.Date_Format;
                                            var Format = oBODCopy.Attributes.Values.ToList().Where(m => m.Name.ToLower() == oVisualisation.sName.ToLower()).Select(m => m.iDateType).FirstOrDefault();
                                            if (Format == 20)
                                            {
                                                sFormat = oBODCopy.Attributes.Values.ToList().Where(m => m.Name.ToLower() == oVisualisation.sName.ToLower()).Select(m => m.Format).FirstOrDefault();//"dd/MM/yyyy";
                                                oVisualisation.sValue = Utility.GetDateResolvedValue(oVisualisation.sValue, sFormat);
                                            }
                                            else
                                            {
                                                oVisualisation.sValue = Utility.GetDateResolvedValue(oVisualisation.sValue, XIConstant.Date_Format); //"dd-MMM-yyyy"
                                            }
                                            if (oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) || string.IsNullOrEmpty(oVisualisation.sValue))
                                            {
                                                oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                            }
                                            if (!string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue))
                                            {
                                                string sValue = oBODisplay.BOInstance.AttributeI(oVisualisation.sName).sValue;
                                                string sFormattedValue = String.Format("{0:" + sFormat + "}", Convert.ToDateTime(sValue));
                                            }
                                        }
                                    }
                                    if (oBODisplay.BOInstance != null && oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) && string.IsNullOrEmpty(oBODisplay.BOInstance.AttributeI(oVisualisation.sName.ToLower()).sValue))
                                    {
                                        oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()] = new XIIAttribute { sName = oVisualisation.sName, sValue = oVisualisation.sValue, sPreviousValue = oVisualisation.sValue, bDirty = true };
                                    }
                                    else if (oBODisplay.BOInstance != null && oBODisplay.BOInstance.Attributes.ContainsKey(oVisualisation.sName.ToLower()) && !string.IsNullOrEmpty(oVisualisation.sType) && oVisualisation.sType.ToLower() == "override" && !string.IsNullOrEmpty(oVisualisation.sValue))
                                    {
                                        oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()].sValue = oVisualisation.sValue;
                                        oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()].sPreviousValue = oVisualisation.sValue;
                                        oBODisplay.BOInstance.Attributes[oVisualisation.sName.ToLower()].bDirty = true;
                                    }
                                }
                                if (oVisualisation.sName != null && oVisualisation.sName.ToLower() == XIConstant.Param_OverrideGroup.ToLower())
                                {
                                    sOverrideGroup = oVisualisation.sValue;
                                }
                                if (!string.IsNullOrEmpty(sOverrideGroup))
                                {
                                    var oGroupD = oBOD.GroupD(sOverrideGroup);
                                    if (oGroupD != null && !string.IsNullOrEmpty(oGroupD.BOFieldNames))
                                    {
                                        var oAllGroupFields = Utility.GetBOGroupFields(oGroupD.BOFieldNames, oGroupD.bIsCrtdBy, oGroupD.bIsCrtdWhn, oGroupD.bIsUpdtdBy, oGroupD.bIsUpdtdWhn);
                                        var GrpFields = oAllGroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        foreach (var items in GrpFields)
                                        {
                                            if (!oBODisplay.BOInstance.Attributes.ContainsKey(items))
                                            {
                                                oBODisplay.BOInstance.Attributes[items] = new XIIAttribute() { sName = items, bDirty = true };
                                            }
                                        }
                                    }
                                }

                                if(oVisualisation.sName != null && oVisualisation.sName.ToLower() == "htmlmergeattribute")
                                {
                                    string sInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iInstanceID}");
                                    //string sInsID = "8";
                                    string sVisValue = oVisualisation.sValue;
                                    string targetAttr = sVisValue.Split(':')[0];
                                    string sToInterpolate = sVisValue.Split(':')[1];
                                    string pattern = @"{[a-zA-Z_\.]+}";
                                    Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                                    var matches = regex.Matches(sToInterpolate);
                                    foreach (var item in matches)
                                    {
                                        string[] parts = item.ToString().Split('.');
                                        string sBOName = parts[0].Split('{')[1];
                                        string sAttrName = parts[1].Split('}')[0];
                                        string sAttrValue = oXII.BOI(sBOName, sInsID, sAttrName).AttributeI(sAttrName).sValue;
                                        sToInterpolate = Regex.Replace(sToInterpolate, item.ToString(), sAttrValue);
                                    }
                                    oBOI.Attributes[targetAttr.ToLower()].sValue = sToInterpolate;
                                    oBOI.Attributes[targetAttr.ToLower()].bDirty = true;
                                }
                            }
                            foreach (var oVisualisationList in oXIDVisual.XiVisualisationLists)
                            {
                                if (!string.IsNullOrEmpty(oVisualisationList.ListName) && oVisualisationList.ListName.ToLower() == "adautosave")
                                {
                                    var oNVs = oXIDVisual.XiVisualisationNVs.Where(m => m.XiVisualListID == oVisualisationList.XiVisualListID).ToList();
                                    foreach (var oNV in oNVs)
                                    {
                                        if (!oBODisplay.BOInstance.Attributes.ContainsKey(oNV.sName.ToLower()))
                                        {
                                            oBODisplay.BOInstance.Attributes[oNV.sName] = new XIIAttribute { sName = oNV.sName, sValue = oNV.sValue, sPreviousValue = oNV.sValue, bDirty = true, bIsHidden = true };
                                        }
                                        else
                                        {
                                            oBODisplay.BOInstance.Attributes[oNV.sName.ToLower()] = new XIIAttribute { sName = oNV.sName.ToLower(), sValue = oNV.sValue, sPreviousValue = oNV.sValue, bDirty = true, bIsHidden = true, FieldDDL = oBODisplay.BOInstance.Attributes[oNV.sName.ToLower()].FieldDDL };
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(oVisualisationList.ListName) && oVisualisationList.ListName.ToLower() == "restrictautosave")
                                {
                                        var oNVs = oXIDVisual.XiVisualisationNVs.Where(m => m.XiVisualListID == oVisualisationList.XiVisualListID).ToList();
                                    foreach (var oNV in oNVs)
                                    {
                                        if (oXIDVisual.XiVisualisationNVs.Any(x => x.sName.ToLower() == "UseLayoutGUID".ToLower() && x.sValue == "true") && (!string.IsNullOrEmpty(sLayoutGuid)))
                                        {
                                            oCache.Set_ParamVal(sSessionID, sLayoutGuid, oBOI.BOD.Name, oNV.sName, oNV.sValue, "", null);
                                        }
                                        else
                                        {
                                            oCache.Set_ParamVal(sSessionID, sGUID, oBOI.BOD.Name, oNV.sName, oNV.sValue, "", null);
                                        }
                                    }
                                }
                            }
                            oXIVisualisations.Add(oXIDVisual);
                        }
                    }
                    if (!string.IsNullOrEmpty(sLockGroup))
                    {
                        string sPrimaryKey = string.Empty;
                        sPrimaryKey = oBOI.BOD.sPrimaryKey;
                        var GroupFields = oBOI.BOD.GroupD(sLockGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                        var oGroupD = oBOI.BOD.GroupD(sLockGroup);
                        var oAllGroupFields = Utility.GetBOGroupFields(GroupFields, oGroupD.bIsCrtdBy, oGroupD.bIsCrtdWhn, oGroupD.bIsUpdtdBy, oGroupD.bIsUpdtdWhn);
                        if (!string.IsNullOrEmpty(oAllGroupFields))
                        {
                            var oGrpFields = oAllGroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            oBODisplay.BOInstance.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bLock = true);
                            oBODisplay.BOInstance.Attributes.Values.Where(m => m.sName == sPrimaryKey).ToList().ForEach(m => m.bLock = true);
                        }
                    }
                    if (!string.IsNullOrEmpty(sHiddenGroup))
                    {
                        string sPrimaryKey = string.Empty;
                        sPrimaryKey = oBOI.BOD.sPrimaryKey;
                        var GroupFields = oBOI.BOD.GroupD(sHiddenGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                        var oGroupD = oBOI.BOD.GroupD(sHiddenGroup);
                        var oAllGroupFields = Utility.GetBOGroupFields(GroupFields, oGroupD.bIsCrtdBy, oGroupD.bIsCrtdWhn, oGroupD.bIsUpdtdBy, oGroupD.bIsUpdtdWhn);
                        if (!string.IsNullOrEmpty(oAllGroupFields) && oBODisplay.BOInstance != null)
                        {
                            var oGrpFields = oAllGroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            oBODisplay.BOInstance.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bIsHidden = true);
                        }
                    }
                    string OverrideAttribute = oParams.Where(m => m.sName.ToLower() == "OverrideAttribute".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(OverrideAttribute))
                    {
                        var OverrideFields = OverrideAttribute.Split('_');
                        if (OverrideFields.Count() > 1)
                        {
                            var Attribute = oBOI.BOD.Attributes.Where(x => x.Key == OverrideFields[1].ToLower()).Select(t => t.Value).FirstOrDefault();
                            var iACPolicyID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + OverrideFields[1] + "}");
                            var OverrideValue = oXII.BOI(Attribute.sFKBOName, iACPolicyID, OverrideFields[2]);
                            oBOI.Attributes.Where(x => x.Key == OverrideFields[0]).ToList().ForEach(x => x.Value.sValue = OverrideValue.AttributeI(OverrideFields[2]).sValue);
                        }
                    }
                    var HiddenAttrs = oBOI.BOD.Attributes.Values.Where(m => m.bIsHidden == true).ToList();
                    if (HiddenAttrs != null && HiddenAttrs.Count() > 0)
                    {
                        foreach (var Attr in HiddenAttrs)
                        {
                            if (oBODisplay.BOInstance.Attributes.ContainsKey(Attr.Name.ToLower()))
                            {
                                oBODisplay.BOInstance.Attributes[Attr.Name.ToLower()].bIsHidden = true;
                            }
                        }
                    }
                    if (MergeAttrs != null && MergeAttrs.Count() > 0)
                    {

                        foreach (var Attr in MergeAttrs)
                        {
                            if (oBODisplay.BOInstance.Attributes.ContainsKey(Attr.sName))
                            {
                                oBODisplay.BOInstance.Attributes[Attr.sName].sValue = Attr.sValue;
                                oBODisplay.BOInstance.Attributes[Attr.sName].bIsHidden = true;
                            }
                            else
                            {
                                oBODisplay.BOInstance.Attributes.Add(Attr.sName, new XIIAttribute() { sName = Attr.sName, sValue = Attr.sValue, bIsHidden = true, bDirty = true });
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sBOActionCode))
                    {
                        Dictionary<string, object> NVs = new Dictionary<string, object>();
                        NVs["FKiBOID"] = oBODCopy.BOID;
                        var Actions = Connection.Select<XIDBOAction>("XIBOAction_T", NVs).ToList();
                        if (Actions != null && Actions.Count() > 0)
                        {
                            var MatchedActions = Actions.Where(m => m.sCode.ToLower().Contains(sBOActionCode.ToLower())).ToList();
                            oBODisplay.BOInstance.Actions = MatchedActions;
                        }
                    }
                    if (!string.IsNullOrEmpty(sBOUpdateAction))
                    {
                        Visualisations = null;
                        Visualisations = Visualisations ?? new List<XIVisualisation>();
                        if (Visualisations.Count() == 0)
                        {
                            Visualisations.Add(new XIVisualisation() { Name = "updateaction", NVs = new List<XIVisualisationNV>() });
                        }
                        Visualisations.FirstOrDefault().NVs.Add(new XIVisualisationNV { sName = "sUpdateAction", sValue = sBOUpdateAction });
                    }
                    if (!string.IsNullOrEmpty(FormMode) && FormMode.ToLower() == "inline")
                    {
                        oBODisplay.BOInstance.Actions = oBODisplay.BOInstance.Actions ?? new List<XIDBOAction>();
                        var ID = oCache.Get_ParamVal(sSessionID, sGUID, null, "1ClickID");
                        if (!string.IsNullOrEmpty(ID))
                        {
                            var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, ID);
                            if (o1ClickD.Actions != null && o1ClickD.Actions.Count() > 0)
                            {
                                var InlineActions = o1ClickD.Actions.Where(m => m.iType == 20).ToList();
                                if (InlineActions != null && InlineActions.Count() > 0)
                                {
                                    foreach (var action in InlineActions)
                                    {
                                        var oActionD = (XIDBOAction)oCache.GetObjectFromCache(XIConstant.CacheBOAction, null, action.FKiActionID.ToString());
                                        oBODisplay.BOInstance.Actions.Add(oActionD);
                                    }
                                }
                            }

                        }
                    }
                    //Check Value type to show icon
                    var ValueTypeAttrs = oBOD.Attributes.Values.ToList().Where(m => m.bValueType == true).ToList();
                    foreach (var valuetype in ValueTypeAttrs)
                    {
                        var AttrI = oBODisplay.BOInstance.AttributeI(valuetype.Name);
                        if (AttrI != null && !string.IsNullOrEmpty(AttrI.sValue))
                        {
                            oCR = AttrI.Check_ValueTypeForMetaIcon(AttrI.sValue);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                AttrI.sValueTypeIcon = (string)oCR.oResult;
                            }
                        }
                    }
                    oBODisplay.BOInstance.iBODID = oBODCopy.BOID;
                    oBODisplay.BOInstance.BOIDXIGUID = oBODCopy.XIGUID;
                    oBODisplay.BOInstance.BOD = null;
                }
                else
                {
                    oCR.sMessage = "Config Error: XIInfraFormComponent_XILoad() : BO Name is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                    oBODisplay.BOInstance.sErrorMessage = "Mandatory parameters not passed to load the Form component";
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                if (oBODisplay.BOInstance != null)
                {
                    oBODisplay.BOInstance.sHierarchy = oParams.Where(m => m.sName == XIConstant.Param_Hierarchy).Select(m => m.sValue).FirstOrDefault();
                }
                oBODisplay.Visualisations = Visualisations;
                oBODisplay.oAccordGroupAttr = oAccordGroupAttr;
                Guid ParentXIGUID = Guid.Empty;
                Guid.TryParse(parentxiguid, out ParentXIGUID);
                oBODisplay.BOInstance.ParentXIGUID = ParentXIGUID;
                Guid Parent1ClickXIGUID = Guid.Empty;
                Guid.TryParse(parent1clickxiguid, out Parent1ClickXIGUID);
                oBODisplay.BOInstance.Parent1ClickGUID = Parent1ClickXIGUID;
                oCResult.oResult = oBODisplay;
            }
            catch (Exception ex)
            {
                oBODisplay.BOInstance.sErrorMessage = "Something went wrong while loading the Form component";
                oCResult.oResult = oBODisplay;
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}