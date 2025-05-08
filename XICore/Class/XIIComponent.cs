using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIIComponent : XIInstanceBase
    {
        public bool bFlag { get; set; }
        public string sGUID { get; set; }
        public int FKiStepDefinitonID { get; set; }
        public Nullable<Guid> FKiStepDefinitonIDXIGUID { get; set; }
        public string sCallHierarchy { get; set; }
        public CResult Load()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                var sSessionID = HttpContext.Current.Session.SessionID;
                var iUserOrg = HttpContext.Current.Session["iUserOrg"];
                var AppID = HttpContext.Current.Session["ApplicationID"];
                var AppXIGUID = HttpContext.Current.Session["ApplicationIDXIGUID"];
                XIInfraCache oCache = new XIInfraCache();
                //XIInstanceBase oInstBase = new XIInstanceBase();
                XIIComponent oCompI = new XIIComponent();
                XIDComponent oCompD = new XIDComponent();
                oCompD = (XIDComponent)oDefintion;
                var oParams = oCompD.Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue }).ToList();
                oParams = (List<CNV>)oCache.ResolveParameters(oParams, sSessionID, sGUID);
                oParams.Add(new CNV { sName = "sGUID", sValue = sGUID, sType = "Default" });
                oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID, sType = "Default" });
                oParams.Add(new CNV { sName = "iUserID", sValue = oInfo.iUserID.ToString(), sType = "Default" });
                oParams.Add(new CNV { sName = XIConstant.Param_CallHierarchy, sValue = sCallHierarchy, sType = "Default" });
                oParams.Add(new CNV { sName = "iOrgID", sValue = oInfo.iOrganizationID.ToString(), sType = "Default" });
                oParams.Add(new CNV { sName = "{XIP|iOrgID}", sValue = oInfo.iOrganizationID.ToString(), sType = "Default" });
                if (iUserOrg != null)
                {
                    oParams.Add(new CNV { sName = "{XIP|iUserOrgID}", sValue = iUserOrg.ToString(), sType = "Default" });
                }

                oParams.Add(new CNV { sName = "sConfigDatabase", sValue = oInfo.sDatabaseName, sType = "Default" });
                oParams.Add(new CNV { sName = "{XIP|sConfigDatabase}", sValue = oInfo.sDatabaseName, sType = "Default" });
                oParams.Add(new CNV { sName = "sCoreDatabase", sValue = oInfo.sCoreDataBase, sType = "Default" });
                oParams.Add(new CNV { sName = "{XIP|sCoreDatabase}", sValue = oInfo.sCoreDataBase, sType = "Default" });
                oParams.Add(new CNV { sName = XIConstant.Param_ApplicationID, sValue = AppID.ToString(), sType = "Default" });
                oParams.Add(new CNV { sName = XIConstant.Param_ApplicationXIGUID, sValue = AppXIGUID.ToString(), sType = "Default" });
                oParams.Add(new CNV { sName = "{XIP|" + XIConstant.Param_ApplicationID + "}", sValue = AppID.ToString(), sType = "Default" });
                var sMainDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                oParams.Add(new CNV { sName = "sMainDatabase", sValue = sMainDatabase, sType = "Default" });
                oParams.Add(new CNV { sName = "{XIP|sMainDatabase}", sValue = sMainDatabase, sType = "Default" });
                oParams.Add(new CNV { sName = "iRoleID", sValue = oInfo.iRoleID.ToString(), sType = "Default" });
                oParams.Add(new CNV { sName = "iUserLevel", sValue = oInfo.iLevel.ToString(), sType = "Default" });
                var sReferenceID = "";
                if (oInfo.iRoleID != 55)
                {
                    sReferenceID = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + oInfo.iOrganizationID + "_" + oInfo.iRoleID + "_" + XIConstant.IDRef_internal);
                    oParams.Add(new CNV { sName = "sIDRef", sValue = sReferenceID, sType = "Default" });
                }
                else
                {
                    sReferenceID = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + oInfo.iOrganizationID + "_" + XIConstant.IDRef_public);
                    oParams.Add(new CNV { sName = "sIDRef", sValue = sReferenceID, sType = "Default" });
                }

                List<CNV> Params = new List<CNV>();
                if (oParams != null && oParams.Count() > 0)
                {
                    Params.AddRange(oParams.Select(m => new CNV { sName = m.sName, sValue = m.sValue, sType = m.sType }));
                    var register = Params.Where(m => m.sName.ToLower() == "register").FirstOrDefault();
                    if (register != null)
                    {
                        if (!string.IsNullOrEmpty(register.sValue))
                        {
                            Params.Add(new CNV { sName = "name", sValue = oCompD.sName + "_" + FKiStepDefinitonIDXIGUID, sType = "register" });
                        }
                        if (Params.Count() > 0)
                        {
                            oCache.SetXIParams(Params, sGUID, sSessionID);
                        }
                    }
                }
                int iVisualID = 0;
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
                    string svisualID = WrapperParms.Where(m => m.sName == XIConstant.XIP_VisualisationID).Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(svisualID, out iVisualID))
                    {

                    }
                    else
                    {
                        iVisualID = 0;
                    }
                }
                List<XIVisualisation> oXIVisualisations = new List<XIVisualisation>();
                string sVisualisation = oParams.Where(m => m.sName.ToLower() == "Visualisation".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sVisualisation) || iVisualID > 0)
                {
                    var oXIvisual = new XIVisualisation();
                    if (iVisualID > 0)
                    {
                        oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, "", iVisualID.ToString());
                    }
                    else
                    {
                        oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisualisation, null);
                    }
                    var oXIDVisual = (XIVisualisation)oXIvisual.GetCopy();
                    if (oXIDVisual.XiVisualisationNVs != null)
                    {
                        foreach (var oVisualisation in oXIDVisual.XiVisualisationNVs)
                        {
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
                            else if (oVisualisation.sValue.StartsWith("{XIP|"))
                            {
                                oVisualisation.sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, oVisualisation.sValue);
                            }
                        }
                    }
                    if (oXIDVisual != null)
                    {
                        oXIVisualisations.Add(oXIDVisual);
                    }
                }
                object Response = null;
                if (oCompD.bMatrix)
                {
                    if (string.IsNullOrEmpty(oCompD.sCode))
                    {
                        oCompD.sCode = xiEnumSystem.EnumMatrixAction.Component.ToString();
                    }
                    XIMatrix oXIMatrix = new XIMatrix();
                    oXIMatrix.MatrixAction(oCompD.sCode, xiEnumSystem.EnumMatrixAction.Component, "", 0, oCompD.XIGUID.ToString(), oCompD.sName, oParams);
                }
                if (oXIVisualisations.Count() == 0)
                {
                    oXIVisualisations.Add(new XIVisualisation());
                }
                if (oXIVisualisations.FirstOrDefault().XiVisualisationNVs == null)
                {
                    oXIVisualisations.FirstOrDefault().XiVisualisationNVs = new List<XIVisualisationNV>();
                }
                if (!string.IsNullOrEmpty(sReferenceID) && sReferenceID.ToLower() == "xiguid")
                {
                    oXIVisualisations.FirstOrDefault().XiVisualisationNVs.Add(new XIVisualisationNV() { sName = "HiddenData", sValue = "yes" });
                }
                //Invoking Method
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Started" });
                switch (oCompD.sName)
                {
                    case "Form Component":
                        XIInfraFormComponent oFC = new XIInfraFormComponent();
                        Response = oFC.XILoad(oParams);
                        break;
                    case "AccountComponent":
                        XIInfraAccountComponent oAC = new XIInfraAccountComponent();
                        Response = oAC.XILoad(oParams);
                        break;
                    case "OneClickComponent":
                        XIInfraOneClickComponent oOCC = new XIInfraOneClickComponent();
                        Response = oOCC.XILoad(oParams);
                        break;
                    case "XITreeStructure":
                        XIInfraTreeStructureComponent oTSC = new XIInfraTreeStructureComponent();
                        Response = oTSC.XILoad(oParams);
                        break;
                    case "QSComponent":
                        XIInfraQSComponent oQSC = new XIInfraQSComponent();
                        Response = oQSC.XILoad(oParams);
                        break;
                    case "Tab Component":
                        XIInfraTabComponent oTC = new XIInfraTabComponent();
                        Response = oTC.XILoad(oParams);
                        break;
                    case "MenuComponent":
                        XIInfraMenuComponent oMC = new XIInfraMenuComponent();
                        Response = oMC.XILoad(oParams);
                        break;
                    case "Grid Component":
                        XIInfraGridComponent oGC = new XIInfraGridComponent();
                        Response = oGC.XILoad(oParams);
                        break;
                    case "HTML Component":
                        XIInfraHtmlComponent oHC = new XIInfraHtmlComponent();
                        Response = oHC.XILoad(oParams);
                        break;
                    case "InboxComponent":
                        XIInfraInboxComponent oIC = new XIInfraInboxComponent();
                        Response = oIC.XILoad(oParams);
                        break;
                    case "XilinkComponent":
                        XIInfraXILinkComponent oXC = new XIInfraXILinkComponent();
                        Response = oXC.XILoad(oParams);
                        break;
                    case "XIApplicationComponent":
                        XIInfraApplicationComponent oXA = new XIInfraApplicationComponent();
                        Response = oXA.XILoad(oParams);
                        break;
                    case "GroupComponent":
                        XIInfraGroupComponent oXG = new XIInfraGroupComponent();
                        Response = oXG.XILoad(oParams);
                        break;
                    case "LayoutComponent":
                        XIInfraLayoutComponent oXL = new XIInfraLayoutComponent();
                        Response = oXL.XILoad(oParams);
                        break;
                    case "MenuNodeComponent":
                        XIInfraMenuNodeComponent oXMN = new XIInfraMenuNodeComponent();
                        Response = oXMN.XILoad(oParams);
                        break;
                    case "ScriptComponent":
                        XIInfraScriptComponent oXS = new XIInfraScriptComponent();
                        Response = oXS.XILoad(oParams);
                        break;
                    case "LayoutMappingComponent":
                        XIInfraLayoutMappingComponent oXIM = new XIInfraLayoutMappingComponent();
                        Response = oXIM.XILoad(oParams);
                        break;
                    case "LayoutDetailsComponent":
                        XIInfraLayoutDetailsComponent oXLD = new XIInfraLayoutDetailsComponent();
                        Response = oXLD.XILoad(oParams);
                        break;
                    case "ReportComponent":
                        XIInfraReportComponent oXRC = new XIInfraReportComponent();
                        Response = oXRC.XILoad(oParams);
                        break;
                    case "DialogComponent":
                        XIInfraDialogComponent oXRD = new XIInfraDialogComponent();
                        Response = oXRD.XILoad(oParams);
                        break;
                    case "FieldOriginComponent":
                        XIInfraFieldOriginComponent oXFdef = new XIInfraFieldOriginComponent();
                        Response = oXFdef.XILoad(oParams);
                        break;
                    case "XIParameterComponent":
                        XIInfraXIParameterComponent oXIPC = new XIInfraXIParameterComponent();
                        Response = oXIPC.XILoad(oParams);
                        break;
                    case "DataTypeComponent":
                        XIInfraDataTypeComponent oXDT = new XIInfraDataTypeComponent();
                        Response = oXDT.XILoad(oParams);
                        break;
                    case "XIComponentComponent":
                        XIInfraXIComponentComponent oXIXC = new XIInfraXIComponentComponent();
                        Response = oXIXC.XILoad(oParams);
                        break;
                    case "XIBOComponent":
                        XIInfraXIBOComponent oXIBO = new XIInfraXIBOComponent();
                        Response = oXIBO.XILoad(oParams);
                        break;
                    case "XIBOAttributeComponent":
                        XIInfraXIBOAttributeComponent oXIBOAttr = new XIInfraXIBOAttributeComponent();
                        Response = oXIBOAttr.XILoad(oParams);
                        break;
                    case "XIBOScriptComponent":
                        XIInfraXIBOScriptComponent oXIBOSC = new XIInfraXIBOScriptComponent();
                        Response = oXIBOSC.XILoad(oParams);
                        break;
                    case "XIBOStructureComponent":
                        XIInfraXIBOStructureComponent oXIBOSt = new XIInfraXIBOStructureComponent();
                        Response = oXIBOSt.XILoad(oParams);
                        break;
                    case "XIDataSourceComponent":
                        XIInfraXIDataSounceComponent oXID = new XIInfraXIDataSounceComponent();
                        Response = oXID.XILoad(oParams);
                        break;
                    case "QueryManagementComponent":
                        XIInfraQueryManagementComponent oXIQ = new XIInfraQueryManagementComponent();
                        Response = oXIQ.XILoad(oParams);
                        break;
                    case "QSConfigComponent":
                        XIInfraQSConfigComponent oXIQS = new XIInfraQSConfigComponent();
                        Response = oXIQS.XILoad(oParams);
                        break;
                    case "QSStepConfigComponent":
                        XIInfraQSStepConfigComponent oXIStep = new XIInfraQSStepConfigComponent();
                        Response = oXIStep.XILoad(oParams);
                        break;
                    case "QSSectionConfigComponent":
                        XIInfraQSSectionConfigComponent oXISec = new XIInfraQSSectionConfigComponent();
                        Response = oXISec.XILoad(oParams);
                        break;
                    case "VisualisationComponent":
                        XIInfraVisualisationComponent oXIVisual = new XIInfraVisualisationComponent();
                        Response = oXIVisual.XILoad(oParams);
                        break;
                    case "XIUrlMappingComponent":
                        XIInfraUrlMappingComponent oUrl = new XIInfraUrlMappingComponent();
                        Response = oUrl.XILoad(oParams);
                        break;
                    case "QSLinkComponent":
                        XIInfraQSLinkComponent oLink = new XIInfraQSLinkComponent();
                        Response = oLink.XILoad(oParams);
                        break;
                    case "QSLinkDefinationComponent":
                        XIInfraQSLinkDefinationComponent oLDef = new XIInfraQSLinkDefinationComponent();
                        Response = oLDef.XILoad(oParams);
                        break;
                    case "PieChartComponent":
                        XIInfraPieChartComponent oPC = new XIInfraPieChartComponent();
                        Response = oPC.XILoad(oParams);
                        break;
                    case "CombinationChartComponent":
                        XIInfraCombinationChartComponent oCC = new XIInfraCombinationChartComponent();
                        Response = oCC.XILoad(oParams);
                        break;
                    case "GaugeChartComponent":
                        XIInfraGaugeChartComponent oGCC = new XIInfraGaugeChartComponent();
                        Response = oGCC.XILoad(oParams);
                        break;
                    case "DashBoardChartComponent":
                        XIInfraDashBoardChartComponent oDC = new XIInfraDashBoardChartComponent();
                        Response = oDC.XILoad(oParams);
                        break;
                    case "DailerComponent":
                        XIInfraDailerComponent oDaC = new XIInfraDailerComponent();
                        Response = oDaC.XILoad(oParams);
                        break;
                    case "MappingComponent":
                        XIInfraMappingComponent oXMC = new XIInfraMappingComponent();
                        Response = oXMC.XILoad(oParams);
                        break;
                    case "MultiRowComponent":
                        XIInfraMultiRowComponent oXMR = new XIInfraMultiRowComponent();
                        Response = oXMR.XILoad(oParams);
                        break;
                    case "XIInfraXIBOUIComponent":
                        XIInfraXIBOUIDetailsComponent oBOUI = new XIInfraXIBOUIDetailsComponent();
                        Response = oBOUI.XILoad(oParams);
                        break;
                    case "QuoteReportDataComponent":
                        XIInfraQuoteReportDataComponent oRDC = new XIInfraQuoteReportDataComponent();
                        Response = oRDC.XILoad(oParams);
                        break;
                    case "AM4PieChartComponent":
                        XIInfraAM4PieChartComponent oAMP = new XIInfraAM4PieChartComponent();
                        Response = oAMP.XILoad(oParams);
                        break;
                    case "KPIGroupingComponent":
                        XIInfraKPIGroupingComponent oKPIGC = new XIInfraKPIGroupingComponent();
                        Response = oKPIGC.XILoad(oParams);
                        break;
                    case "KPIComponent":
                        XIInfraKPIComponent oKPI = new XIInfraKPIComponent();
                        Response = oKPI.XILoad(oParams);
                        break;
                    case "AM4SemiPieChartComponent":
                        XIInfraAM4SemiPieChartComponent oAMSP = new XIInfraAM4SemiPieChartComponent();
                        Response = oAMSP.XILoad(oParams);
                        break;
                    case "AM4BarChartComponent":
                        XIInfraAM4BarChartComponent oAMB = new XIInfraAM4BarChartComponent();
                        Response = oAMB.XILoad(oParams);
                        break;
                    case "AM4ColumnBarChartComponent":
                        XIInfraAM4ColumnBarChartComponent oAMMB = new XIInfraAM4ColumnBarChartComponent();
                        Response = oAMMB.XILoad(oParams);
                        break;
                    case "AM4ColumnLineChartComponent":
                        XIInfraAM4ColumnLineChartComponent oAMCL = new XIInfraAM4ColumnLineChartComponent();
                        Response = oAMCL.XILoad(oParams);
                        break;
                    case "AM4GaugeChartComponent":
                        XIInfraAM4GaugeChartComponent oAMG = new XIInfraAM4GaugeChartComponent();
                        Response = oAMG.XILoad(oParams);
                        break;
                    case "AM4HeatChartComponent":
                        XIInfraAM4HeatChartComponent oAMH = new XIInfraAM4HeatChartComponent();
                        Response = oAMH.XILoad(oParams);
                        break;
                    case "AM4LineChartComponent":
                        XIInfraAM4LineChartComponent oAML = new XIInfraAM4LineChartComponent();
                        Response = oAML.XILoad(oParams);
                        break;
                    case "AM4PriceChartComponent":
                        XIInfraAM4PriceChartComponent oAMPr = new XIInfraAM4PriceChartComponent();
                        Response = oAMPr.XILoad(oParams);
                        break;
                    case "AM4MultiBarwithLineChartComponent":
                        XIInfraAM4MultiBarwithLineChartComponent oAMMUB = new XIInfraAM4MultiBarwithLineChartComponent();
                        Response = oAMMUB.XILoad(oParams);
                        break;
                    case "FeedComponent":
                        XIInfraHTMLBasicComponent oFeed = new XIInfraHTMLBasicComponent();
                        Response = oFeed.XILoad(oParams);
                        break;
                    case "DocumentTreeComponent":
                        XIInfraDocumentTreeComponent oDocC = new XIInfraDocumentTreeComponent();
                        Response = oDocC.XILoad(oParams);
                        break;
                    case "UserCreationComponent":
                        XIInfraUserCreationComponent oUC = new XIInfraUserCreationComponent();
                        Response = oUC.XILoad(oParams);
                        break;
                    case "DynamicTreeComponent":
                        XIInfraDynamicTreeComponent oDTC = new XIInfraDynamicTreeComponent();
                        Response = oDTC.XILoad(oParams);
                        break;
                    case "KPICircleComponent":
                        XIInfraKPICircleComponent oKPIC = new XIInfraKPICircleComponent();
                        Response = oKPIC.XILoad(oParams);
                        break;
                    case "XIValuesComponent":
                        XIInfraXIValuesComponent oXIVC = new XIInfraXIValuesComponent();
                        Response = oXIVC.XILoad(oParams);
                        break;
                    case "RulesComponent":
                        XIInfraRulesComponent oRlz = new XIInfraRulesComponent();
                        Response = oRlz.XILoad(oParams);
                        break;
                    case "SmartSearchComponent":
                        XIInfraSmartSearchComponent oSS = new XIInfraSmartSearchComponent();
                        Response = oSS.XILoad(oParams);
                        break;
                    case "CodeEditorComponent":
                        XIInfraCodeEditorComponent oCE = new XIInfraCodeEditorComponent();
                        Response = oCE.XILoad(oParams);
                        break;
                    case "AccordionComponent":
                        XIInfraAccordionComponent oAcprC = new XIInfraAccordionComponent();
                        Response = oAcprC.XILoad(oParams);
                        break;
                    case "PaymentComponent":
                        XIInfraPayment oPayment = new XIInfraPayment();
                        Response = oPayment.XILoad(oParams);
                        break;
                    case "AM4FunnelChartComponent":
                        XIInfraAM4FunnelComponent oFunnel = new XIInfraAM4FunnelComponent();
                        Response = oFunnel.XILoad(oParams);
                        break;
                    case "RoleDashBoardComponent":
                        XIInfraRoleDashBoardComponent oADB = new XIInfraRoleDashBoardComponent();
                        Response = oADB.XILoad(oParams);
                        break;
                    default:
                        break;
                }
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Completed" });
                if (Response != null)
                {
                    string sActiveMenu = Singleton.Instance.ActiveMenu;
                    if (!string.IsNullOrEmpty(sActiveMenu))
                    {
                        XIDComponentParam oComponentparams = new XIDComponentParam();
                        oComponentparams.sName = "-ActiveMenu";
                        oComponentparams.sValue = sActiveMenu;
                        oCompD.Params.Add(oComponentparams);
                    }
                    oCompI.oDefintion = oCompD;
                    oCompI.sGUID = sGUID;
                    oCR = (CResult)Response;
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        switch (oCompD.sName)
                        {
                            case "Form Component":
                                oCompI.oContent[XIConstant.FormComponent] = oCR.oResult;
                                break;
                            case "OneClickComponent":
                                oCompI.oContent[XIConstant.OneClickComponent] = oCR.oResult;
                                break;
                            case "XITreeStructure":
                                oCompI.oContent[XIConstant.XITreeStructure] = oCR.oResult;
                                break;
                            case "QSComponent":
                                oCompI.oContent[XIConstant.QSComponent] = oCR.oResult;
                                break;
                            case "Tab Component":
                                oCompI.oContent[XIConstant.TabComponent] = oCR.oResult;
                                break;
                            case "MenuComponent":
                                oCompI.oContent[XIConstant.MenuComponent] = oCR.oResult;
                                break;
                            case "Grid Component":
                                oCompI.oContent[XIConstant.GridComponent] = oCR.oResult;
                                break;
                            case "HTML Component":
                                oCompI.oContent[XIConstant.HTMLComponent] = oCR.oResult;
                                break;
                            case "InboxComponent":
                                oCompI.oContent[XIConstant.InboxComponent] = oCR.oResult;
                                break;
                            case "XilinkComponent":
                                oCompI.oContent[XIConstant.XilinkComponent] = oCR.oResult;
                                break;
                            case "XIApplicationComponent":
                                oCompI.oContent[XIConstant.XIApplicationComponent] = oCR.oResult;
                                break;
                            case "GroupComponent":
                                oCompI.oContent[XIConstant.GroupComponent] = oCR.oResult;
                                break;
                            case "LayoutComponent":
                                oCompI.oContent[XIConstant.LayoutComponent] = oCR.oResult;
                                break;
                            case "MenuNodeComponent":
                                oCompI.oContent[XIConstant.MenuNodeComponent] = oCR.oResult;
                                break;
                            case "ScriptComponent":
                                oCompI.oContent[XIConstant.ScriptComponent] = oCR.oResult;
                                break;
                            case "LayoutMappingComponent":
                                oCompI.oContent[XIConstant.LayoutMappingComponent] = oCR.oResult;
                                break;
                            case "LayoutDetailsComponent":
                                oCompI.oContent[XIConstant.LayoutDetailsComponent] = oCR.oResult;
                                break;
                            case "ReportComponent":
                                oCompI.oContent[XIConstant.ReportComponent] = oCR.oResult;
                                break;
                            case "DialogComponent":
                                oCompI.oContent[XIConstant.DialogComponent] = oCR.oResult;
                                break;
                            case "FieldOriginComponent":
                                oCompI.oContent[XIConstant.FieldOriginComponent] = oCR.oResult;
                                break;
                            case "XIParameterComponent":
                                oCompI.oContent[XIConstant.XIParameterComponent] = oCR.oResult;
                                break;
                            case "DataTypeComponent":
                                oCompI.oContent[XIConstant.DataTypeComponent] = oCR.oResult;
                                break;
                            case "XIComponentComponent":
                                oCompI.oContent[XIConstant.XIComponentComponent] = oCR.oResult;
                                break;
                            case "XIBOComponent":
                                oCompI.oContent[XIConstant.XIBOComponent] = oCR.oResult;
                                break;
                            case "XIBOAttributeComponent":
                                oCompI.oContent[XIConstant.XIBOAttributeComponent] = oCR.oResult;
                                break;
                            case "XIBOScriptComponent":
                                oCompI.oContent[XIConstant.XIBOScriptComponent] = oCR.oResult;
                                break;
                            case "XIBOStructureComponent":
                                oCompI.oContent[XIConstant.XIBOStructureComponent] = oCR.oResult;
                                break;
                            case "QueryManagementComponent":
                                oCompI.oContent[XIConstant.QueryManagementComponent] = oCR.oResult;
                                break;
                            case "QSConfigComponent":
                                oCompI.oContent[XIConstant.QSConfigComponent] = oCR.oResult;
                                break;
                            case "QSStepConfigComponent":
                                oCompI.oContent[XIConstant.QSStepConfigComponent] = oCR.oResult;
                                break;
                            case "QSSectionConfigComponent":
                                oCompI.oContent[XIConstant.QSSectionConfigComponent] = oCR.oResult;
                                break;
                            case "VisualisationComponent":
                                oCompI.oContent[XIConstant.VisualisationComponent] = oCR.oResult;
                                break;
                            case "XIUrlMappingComponent":
                                oCompI.oContent[XIConstant.XIUrlMappingComponent] = oCR.oResult;
                                break;
                            case "XIDataSourceComponent":
                                oCompI.oContent[XIConstant.XIDataSourceComponent] = oCR.oResult;
                                break;
                            case "QSLinkComponent":
                                oCompI.oContent[XIConstant.QSLinkComponent] = oCR.oResult;
                                break;
                            case "QSLinkDefinationComponent":
                                oCompI.oContent[XIConstant.QSLinkDefinationComponent] = oCR.oResult;
                                break;
                            case "MappingComponent":
                                oCompI.oContent[XIConstant.MappingComponent] = oCR.oResult;
                                break;
                            case "MultiRowComponent":
                                oCompI.oContent[XIConstant.MultiRowComponent] = oCR.oResult;
                                break;
                            case "XIInfraXIBOUIComponent":
                                oCompI.oContent[XIConstant.XIInfraXIBOUIComponent] = oCR.oResult;
                                break;
                            case "CombinationChartComponent":
                                oCompI.oContent[XIConstant.CombinationChartComponent] = oCR.oResult;
                                break;
                            case "PieChartComponent":
                                oCompI.oContent[XIConstant.PieChartComponent] = oCR.oResult;
                                break;
                            case "GaugeChartComponent":
                                oCompI.oContent[XIConstant.GaugeChartComponent] = oCR.oResult;
                                break;
                            case "DailerComponent":
                                oCompI.oContent[XIConstant.DailerComponent] = oCR.oResult;
                                break;
                            case "QuoteReportDataComponent":
                                oCompI.oContent[XIConstant.QuoteReportDataComponent] = oCR.oResult;
                                break;

                            case "FeedComponent":
                                oCompI.oContent[XIConstant.FeedComponent] = oCR.oResult;
                                break;
                            case "DocumentTreeComponent":
                                oCompI.oContent[XIConstant.DocumentTreeComponent] = oCR.oResult;
                                break;
                            case "KPIGroupingComponent":
                                oCompI.oContent[XIConstant.KPIGroupingComponent] = oCR.oResult;
                                break;
                            case "KPIComponent":
                                oCompI.oContent[XIConstant.KPIComponent] = oCR.oResult;
                                break;
                            case "AM4LineChartComponent":
                                oCompI.oContent[XIConstant.AM4LineChartComponent] = oCR.oResult;
                                break;
                            case "AM4PriceChartComponent":
                                oCompI.oContent[XIConstant.AM4PriceChartComponent] = oCR.oResult;
                                break;
                            case "AM4PieChartComponent":
                                oCompI.oContent[XIConstant.AM4PieChartComponent] = oCR.oResult;
                                break;
                            case "AM4SemiPieChartComponent":
                                oCompI.oContent[XIConstant.AM4SemiPieChartComponent] = oCR.oResult;
                                break;
                            case "AM4BarChartComponent":
                                oCompI.oContent[XIConstant.AM4BarChartComponent] = oCR.oResult;
                                break;
                            case "AM4ColumnBarChartComponent":
                                oCompI.oContent[XIConstant.AM4ColumnBarChartComponent] = oCR.oResult;
                                break;
                            case "AM4ColumnLineChartComponent":
                                oCompI.oContent[XIConstant.AM4ColumnLineChartComponent] = oCR.oResult;
                                break;
                            case "AM4HeatChartComponent":
                                oCompI.oContent[XIConstant.AM4HeatChartComponent] = oCR.oResult;
                                break;
                            case "AM4GaugeChartComponent":
                                oCompI.oContent[XIConstant.AM4GaugeChartComponent] = oCR.oResult;
                                break;
                            case "AM4MultiBarwithLineChartComponent":
                                oCompI.oContent[XIConstant.AM4MultiBarwithLineChartComponent] = oCR.oResult;
                                break;
                            case "ReportDataComponent":
                                oCompI.oContent[XIConstant.ReportDataComponent] = oCR.oResult;
                                break;
                            case "UserCreationComponent":
                                oCompI.oContent[XIConstant.UserCreationComponent] = oCR.oResult;
                                break;
                            case "DynamicTreeComponent":
                                oCompI.oContent[XIConstant.DynamicTreeComponent] = oCR.oResult;
                                break;
                            case "KPICircleComponent":
                                oCompI.oContent[XIConstant.KPICircleComponent] = oCR.oResult;
                                break;
                            case "XIValuesComponent":
                                oCompI.oContent[XIConstant.XIValuesComponent] = oCR.oResult;
                                break;
                            case "RulesComponent":
                                XIInfraRulesComponent oRlz = new XIInfraRulesComponent();
                                Response = oRlz.XILoad(oParams);
                                break;
                            case "SmartSearchComponent":
                                oCompI.oContent[XIConstant.SmartSearchComponent] = oCR.oResult;
                                break;
                            case "CodeEditorComponent":
                                oCompI.oContent[XIConstant.CodeEditorComponent] = oCR.oResult;
                                break;
                            case "AccordionComponent":
                                oCompI.oContent[XIConstant.AccordionComponent] = oCR.oResult;
                                break;
                            case "PaymentComponent":
                                oCompI.oContent[XIConstant.PaymentComponent] = oCR.oResult;
                                break;
                            case "AM4FunnelChartComponent":
                                oCompI.oContent[XIConstant.AM4FunnelChartComponent] = oCR.oResult;
                                break;
                            case "RoleDashBoardComponent":
                                oCompI.oContent[XIConstant.RoleDashBoardComponent] = oCR.oResult;
                                break;
                            default:
                                break;
                        }
                        oCompI.oVisualisation = oXIVisualisations;
                        //oInstBase.oContent[XIConstant.ContentXIComponent] = oCompI;
                        oCResult.oResult = oCompI;
                    }
                    else
                    {
                        oCResult.oResult = null;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always            
        }

        public CResult Load_old()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                var sSessionID = HttpContext.Current.Session.SessionID;
                XIInfraCache oCache = new XIInfraCache();
                //XIInstanceBase oInstBase = new XIInstanceBase();
                XIIComponent oCompI = new XIIComponent();
                XIDComponent oCompD = new XIDComponent();
                oCompD = (XIDComponent)oDefintion;
                var URL = oCompD.sClass.Split('/').ToList().First();
                var sClassName = oCompD.sClass.Split('/').ToList().Last();
                if (!string.IsNullOrEmpty(sClassName))
                {

                    var oParams = oCompD.Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue }).ToList();
                    oParams = (List<CNV>)oCache.ResolveParameters(oParams, sSessionID, sGUID);
                    oParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
                    oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID });
                    oParams.Add(new CNV { sName = "iUserID", sValue = oInfo.iUserID.ToString() });
                    oParams.Add(new CNV { sName = "sRole", sValue = oInfo.sRoleName.ToString() });

                    List<CNV> Params = new List<CNV>();
                    if (oParams != null && oParams.Count() > 0)
                    {
                        Params.AddRange(oParams.Select(m => new CNV { sName = m.sName, sValue = m.sValue }));
                        var register = Params.Where(m => m.sName.ToLower() == "register").FirstOrDefault();
                        if (register != null)
                        {
                            if (!string.IsNullOrEmpty(register.sValue))
                            {
                                Params.Add(new CNV { sName = "name", sValue = oCompD.sName + "_" + FKiStepDefinitonID, sType = "register" });
                            }
                            if (Params.Count() > 0)
                            {
                                oCache.SetXIParams(Params, sGUID, sSessionID);
                            }
                        }
                    }

                    List<XIVisualisation> oXIVisualisations = new List<XIVisualisation>();
                    string sVisualisation = oParams.Where(m => m.sName.ToLower() == "Visualisation".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sVisualisation))
                    {
                        var oXIvisual = (XIVisualisation)oCache.GetObjectFromCache(XIConstant.CacheVisualisation, sVisualisation, null);
                        if (oXIvisual != null)
                        {
                            oXIVisualisations.Add(oXIvisual);
                        }
                    }
                    //Creating Instance
                    Assembly exceutable;
                    Type Ltype;
                    object objclass;
                    if (oCompD.sType.ToLower() == "External".ToLower())
                    {
                        exceutable = Assembly.Load(URL);
                        Ltype = exceutable.GetType(URL + "." + sClassName);
                        objclass = Activator.CreateInstance(Ltype);
                    }
                    else
                    {
                        exceutable = Assembly.GetExecutingAssembly();
                        Ltype = exceutable.GetType("XIInfrastructure.XIInfraQSComponent");
                        objclass = Activator.CreateInstance(Ltype);
                    }
                    //Invoking Method
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Started" });
                    MethodInfo method = Ltype.GetMethod("XILoad");
                    object[] parametersArray = new object[] { oParams };
                    object Response = (object)method.Invoke(objclass, parametersArray);
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Completed" });

                    if (Response != null)
                    {
                        string sActiveMenu = Singleton.Instance.ActiveMenu;
                        if (!string.IsNullOrEmpty(sActiveMenu))
                        {
                            XIDComponentParam oComponentparams = new XIDComponentParam();
                            oComponentparams.sName = "-ActiveMenu";
                            oComponentparams.sValue = sActiveMenu;
                            oCompD.Params.Add(oComponentparams);
                        }
                        oCompI.oDefintion = oCompD;
                        oCompI.sGUID = sGUID;
                        oCR = (CResult)Response;
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            switch (oCompD.sName)
                            {
                                case "Form Component":
                                    oCompI.oContent[XIConstant.FormComponent] = oCR.oResult;
                                    break;
                                case "OneClickComponent":
                                    oCompI.oContent[XIConstant.OneClickComponent] = oCR.oResult;
                                    break;
                                case "XITreeStructure":
                                    oCompI.oContent[XIConstant.XITreeStructure] = oCR.oResult;
                                    break;
                                case "QSComponent":
                                    oCompI.oContent[XIConstant.QSComponent] = oCR.oResult;
                                    break;
                                case "Tab Component":
                                    oCompI.oContent[XIConstant.TabComponent] = oCR.oResult;
                                    break;
                                case "MenuComponent":
                                    oCompI.oContent[XIConstant.MenuComponent] = oCR.oResult;
                                    break;
                                case "Grid Component":
                                    oCompI.oContent[XIConstant.GridComponent] = oCR.oResult;
                                    break;
                                case "HTML Component":
                                    oCompI.oContent[XIConstant.HTMLComponent] = oCR.oResult;
                                    break;
                                case "InboxComponent":
                                    oCompI.oContent[XIConstant.InboxComponent] = oCR.oResult;
                                    break;
                                case "MappingComponent":
                                    oCompI.oContent[XIConstant.MappingComponent] = oCR.oResult;
                                    break;
                                case "MultiRowComponent":
                                    oCompI.oContent[XIConstant.MultiRowComponent] = oCR.oResult;
                                    break;
                                case "DynamicTreeComponent":
                                    oCompI.oContent[XIConstant.DynamicTreeComponent] = oCR.oResult;
                                    break;
                                case "RulesComponent":
                                    XIInfraRulesComponent oRlz = new XIInfraRulesComponent();
                                    Response = oRlz.XILoad(oParams);
                                    break;
                                default:
                                    break;
                            }
                            oCompI.oVisualisation = oXIVisualisations;
                            //oInstBase.oContent[XIConstant.ContentXIComponent] = oCompI;
                            oCResult.oResult = oCompI;
                        }
                        else
                        {
                            oCResult.oResult = null;
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always            
        }
    }
}