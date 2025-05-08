using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XIDatabase;
using XISystem;
using static XIDatabase.XIDBAPI;

namespace XICore
{
    public class XIDComponent : XIDefinitionBase
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sType { get; set; }
        public string sClass { get; set; }
        public string sHTMLPage { get; set; }
        public int FKiApplicationID { get; set; }
        public bool bMatrix { get; set; }
        public string sCode { get; set; }
        [DapperIgnore]
        public int iQSIID { get; set; }
        [DapperIgnore]
        public Guid iQSIIDXIGUID { get; set; }
        [DapperIgnore] 
        public int iOrgID { get; set; }
        [DapperIgnore]
        public string sContext { get; set; }
        [DapperIgnore]
        public string sGUID { get; set; }
        public Guid XIGUID { get; set; }
        [ForeignKey("ID")]
        public virtual List<XIDComponentsNV> XIDComponentsNV { get; set; }
        public virtual List<XIDComponentTrigger> XIDComponentTrigger { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        private List<CNV> oMynParams = new List<CNV>();
        public List<CNV> nParams
        {
            get
            {
                return oMynParams;
            }
            set
            {
                oMynParams = value;
            }
        }

        private List<XIDComponentsNV> oMyNVs = new List<XIDComponentsNV>();
        public List<XIDComponentsNV> NVs
        {
            get
            {
                return oMyNVs;
            }
            set
            {
                oMyNVs = value;
            }
        }

        private List<XIDComponentParam> oMyParams = new List<XIDComponentParam>();
        public List<XIDComponentParam> Params
        {
            get
            {
                return oMyParams;
            }
            set
            {
                oMyParams = value;
            }
        }

        private List<XIDComponentTrigger> oMyTriggers = new List<XIDComponentTrigger>();
        public List<XIDComponentTrigger> Triggers
        {
            get
            {
                return oMyTriggers;
            }
            set
            {
                oMyTriggers = value;
            }
        }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult LoadComponent(string sContextType, string iContextID, int BODID = 0, bool bIsStepLock = false)
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
                XIDXI oXID = new XIDXI();
                XIDComponent oXIComponent = new XIDComponent();
                XIInfraCache oCache = new XIInfraCache();
                XIDComponent oXICom = new XIDComponent();
                if (!string.IsNullOrEmpty(sContext) && sContext.ToLower() == "fixedtemplate")
                {
                    var oComDef = oXID.Get_ComponentDefinition(sName, ID.ToString());
                    if (oComDef.bOK)
                    {
                        oXICom = (XIDComponent)oComDef.oResult;
                    }
                }
                else
                {
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        oXICom = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, sName, XIGUID.ToString()); //oXID.Get_ComponentDefinition(sName, ID.ToString());
                    }
                    else if (ID > 0)
                        oXICom = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, sName, ID.ToString()); //oXID.Get_ComponentDefinition(sName, ID.ToString());
                }
                //(XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, sName, ID.ToString());
                if (oXICom == null)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading component definition" });
                    return oCResult;
                }
                var XICompD = (XIDComponent)oXICom;
                var Copy = (XIDComponent)XICompD.Clone(XICompD);
                //var Copy = (XIDComponent)(oXIComponent.Clone(oXICom));  //(XIDComponent)oXIComponent.Clone(oXICom);
                oXIComponent = GetParamsByContext(Copy, sContextType, iContextID.ToString());
                oXIComponent.Params.Where(m => m.sValue == "{XIP|BODID}").ToList().ForEach(m => m.sValue = BODID.ToString());
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component Definition Loaded Successfully" });

                List<CNV> Params = new List<CNV>();
                if (nParams != null && nParams.Count() > 0)
                {
                    Params.AddRange(nParams.Select(m => new CNV { sName = m.sName, sValue = m.sValue }));
                    var register = nParams.Where(m => m.sName.ToLower() == "register").FirstOrDefault();
                    if (register != null)
                    {
                        if (!string.IsNullOrEmpty(register.sValue))
                        {
                            Params.Add(new CNV { sName = "name", sValue = oXIComponent.sName + "_" + iContextID, sType = "register" });
                        }
                    }
                }

                //Params.Add(new cNameValuePairs { sName = "Context", sValue = sType, sContext = sContext });
                if (sContextType == "QSStep")
                {
                    Params.Add(new CNV { sName = "FKiQSInstanceID", sValue = iQSIID.ToString(), sType = "autoset", sContext = sContext });
                    Params.Add(new CNV { sName = "FKiQSInstanceIDXIGUID", sValue = iQSIIDXIGUID.ToString(), sType = "autoset", sContext = sContext });
                }
                else if (sContextType == "QSStepSection" && sContext.ToLower() != "fixedtemplate")
                {
                    if (iQSIIDXIGUID != null && iQSIIDXIGUID != Guid.Empty)
                    {
                        Params.Add(new CNV { sName = "FKiQSInstanceIDXIGUID", sValue = iQSIIDXIGUID.ToString(), sType = "autoset", sContext = sContext });
                        //Params.Add(new CNV { sName = "FKiQSInstanceIDXIGUID", sValue = iQSIIDXIGUID.ToString() });
                    }
                    else if (iQSIID > 0)
                    {
                        Params.Add(new CNV { sName = "FKiQSInstanceID", sValue = iQSIID.ToString(), sType = "autoset", sContext = sContext });
                    }
                }
                else if (sContextType == "QS")
                {
                    Params.Add(new CNV { sName = "FKiQSInstanceID", sValue = iQSIID.ToString(), sType = "autoset", sContext = sContext });
                    Params.Add(new CNV { sName = "FKiQSInstanceIDXIGUID", sValue = iQSIIDXIGUID.ToString(), sType = "autoset", sContext = sContext });
                }
                if (oXIComponent.Params.Count() > 0)
                {
                    var register = oXIComponent.Params.Where(m => m.sName.ToLower() == "register").FirstOrDefault();
                    if (register != null)
                    {
                        if (!string.IsNullOrEmpty(register.sValue))
                        {
                            Params.Add(new CNV { sName = "name", sValue = oXIComponent.sName + "_" + iContextID, sType = "register" });
                        }
                    }
                }
                //Merger params

                var sSessionID = Params.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    if (oXIComponent.Params.Count() > 0)
                    {
                        var oParams = oXIComponent.Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue }).ToList();
                        oParams = (List<CNV>)oCache.ResolveParameters(oParams, sSessionID, sGUID);
                        Params.AddRange(oParams.Select(m => new CNV { sName = m.sName, sValue = m.sValue, sContext = sContext }));
                        oCache.SetXIParams(Params, sGUID, sSessionID);
                    }
                    else
                    {
                        Params.AddRange(oXIComponent.NVs.Select(m => new CNV { sName = m.sName, sValue = m.sValue, sContext = sContext }));
                        oCache.SetXIParams(Params, sGUID, sSessionID);
                    }
                }
                else
                {
                    sGUID = "ComponentDirectLoad";
                    Params.AddRange(oXIComponent.Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue, sContext = sContext }));
                    oCache.SetXIParams(Params, sGUID, sSessionID);
                }
                var oComponentParams = new List<CNV>();
                oComponentParams.AddRange(Params.Select(m => new CNV { sName = m.sName, sValue = m.sValue, sType = m.sType, nSubParams = m.nSubParams }));
                oComponentParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
                object Response = null;
                switch (oXIComponent.sName)
                {
                    case "Form Component":
                        XIInfraFormComponent oFC = new XIInfraFormComponent();
                        Response = oFC.XILoad(oComponentParams);
                        break;
                    case "OneClickComponent":
                        XIInfraOneClickComponent oOCC = new XIInfraOneClickComponent();
                        Response = oOCC.XILoad(oComponentParams, bIsStepLock);
                        break;
                    case "XITreeStructure":
                        XIInfraTreeStructureComponent oTSC = new XIInfraTreeStructureComponent();
                        Response = oTSC.XILoad(oComponentParams);
                        break;
                    case "QSComponent":
                        XIInfraQSComponent oQSC = new XIInfraQSComponent();
                        Response = oQSC.XILoad(oComponentParams);
                        break;
                    case "Tab Component":
                        XIInfraTabComponent oTC = new XIInfraTabComponent();
                        Response = oTC.XILoad(oComponentParams);
                        break;
                    case "MenuComponent":
                        XIInfraMenuComponent oMC = new XIInfraMenuComponent();
                        Response = oMC.XILoad(oComponentParams);
                        break;
                    case "Grid Component":
                        XIInfraGridComponent oGC = new XIInfraGridComponent();
                        Response = oGC.XILoad(oComponentParams);
                        break;
                    case "HTML Component":
                        XIInfraHtmlComponent oHC = new XIInfraHtmlComponent();
                        Response = oHC.XILoad(oComponentParams);
                        break;
                    case "InboxComponent":
                        XIInfraInboxComponent oIC = new XIInfraInboxComponent();
                        Response = oIC.XILoad(oComponentParams);
                        break;
                    case "ReportComponent":
                        XIInfraReportComponent oRC = new XIInfraReportComponent();
                        Response = oRC.XILoad(oComponentParams);
                        break;
                    case "XilinkComponent":
                        XIInfraXILinkComponent oXC = new XIInfraXILinkComponent();
                        Response = oXC.XILoad(oComponentParams);
                        break;
                    case "FieldOriginComponent":
                        XIInfraFieldOriginComponent oXFO = new XIInfraFieldOriginComponent();
                        Response = oXFO.XILoad(oComponentParams);
                        break;
                    case "PieChartComponent":
                        XIInfraPieChartComponent oPC = new XIInfraPieChartComponent();
                        Response = oPC.XILoad(oComponentParams);
                        break;
                    case "CombinationChartComponent":
                        XIInfraCombinationChartComponent oCC = new XIInfraCombinationChartComponent();
                        Response = oCC.XILoad(oComponentParams);
                        break;
                    case "GaugeChartComponent":
                        XIInfraGaugeChartComponent oGCC = new XIInfraGaugeChartComponent();
                        Response = oGCC.XILoad(oComponentParams);
                        break;
                    case "DashBoardChartComponent":
                        XIInfraDashBoardChartComponent oDC = new XIInfraDashBoardChartComponent();
                        Response = oDC.XILoad(oComponentParams);
                        break;
                    case "DailerComponent":
                        XIInfraDailerComponent oDaC = new XIInfraDailerComponent();
                        Response = oDaC.XILoad(oComponentParams);
                        break;
                    case "GroupComponent":
                        XIInfraGroupComponent oPG = new XIInfraGroupComponent();
                        Response = oPG.XILoad(oComponentParams);
                        break;
                    case "MappingComponent":
                        XIInfraMappingComponent oMPC = new XIInfraMappingComponent();
                        Response = oMPC.XILoad(oComponentParams);
                        break;
                    case "MultiRowComponent":
                        XIInfraMultiRowComponent oMRC = new XIInfraMultiRowComponent();
                        Response = oMRC.XILoad(oComponentParams);
                        break;
                    case "QuoteReportDataComponent":
                        XIInfraQuoteReportDataComponent oRDC = new XIInfraQuoteReportDataComponent();
                        Response = oRDC.XILoad(oComponentParams);
                        break;
                    case "CheckboxComponent":
                        XIInfraCheckboxComponent oCBC = new XIInfraCheckboxComponent();
                        Response = oCBC.XILoad(oComponentParams);
                        break;
                    case "AM4PriceChartComponent":
                        XIInfraAM4PriceChartComponent oAMPr = new XIInfraAM4PriceChartComponent();
                        Response = oAMPr.XILoad(oComponentParams);
                        break;
                    case "KPIGroupingComponent":
                        XIInfraKPIGroupingComponent oKPIGC = new XIInfraKPIGroupingComponent();
                        Response = oKPIGC.XILoad(oComponentParams);
                        break;
                    case "KPIComponent":
                        XIInfraKPIComponent oKPI = new XIInfraKPIComponent();
                        Response = oKPI.XILoad(oComponentParams);
                        break;
                    case "AM4PieChartComponent":
                        XIInfraAM4PieChartComponent oAMP = new XIInfraAM4PieChartComponent();
                        Response = oAMP.XILoad(oComponentParams);
                        break;
                    case "AM4SemiPieChartComponent":
                        XIInfraAM4SemiPieChartComponent oAMSP = new XIInfraAM4SemiPieChartComponent();
                        Response = oAMSP.XILoad(oComponentParams);
                        break;
                    case "AM4BarChartComponent":
                        XIInfraAM4BarChartComponent oAMB = new XIInfraAM4BarChartComponent();
                        Response = oAMB.XILoad(oComponentParams);
                        break;
                    case "AM4ColumnBarChartComponent":
                        XIInfraAM4ColumnBarChartComponent oAMMB = new XIInfraAM4ColumnBarChartComponent();
                        Response = oAMMB.XILoad(oComponentParams);
                        break;
                    case "AM4ColumnLineChartComponent":
                        XIInfraAM4ColumnLineChartComponent oAMCL = new XIInfraAM4ColumnLineChartComponent();
                        Response = oAMCL.XILoad(oComponentParams);
                        break;
                    case "AM4HeatChartComponent":
                        XIInfraAM4HeatChartComponent oAMH = new XIInfraAM4HeatChartComponent();
                        Response = oAMH.XILoad(oComponentParams);
                        break;
                    case "AM4LineChartComponent":
                        XIInfraAM4LineChartComponent oAML = new XIInfraAM4LineChartComponent();
                        Response = oAML.XILoad(oComponentParams);
                        break;
                    case "AM4GaugeChartComponent":
                        XIInfraAM4GaugeChartComponent oAMG = new XIInfraAM4GaugeChartComponent();
                        Response = oAMG.XILoad(oComponentParams);
                        break;
                    case "AM4MultiBarwithLineChartComponent":
                        XIInfraAM4MultiBarwithLineChartComponent oAMMUB = new XIInfraAM4MultiBarwithLineChartComponent();
                        Response = oAMMUB.XILoad(oComponentParams);
                        break;
                    case "AccountComponent":
                        XIInfraAccountComponent oAC = new XIInfraAccountComponent();
                        Response = oAC.XILoad(oComponentParams);
                        break;
                    case "DynamicTreeComponent":
                        XIInfraDynamicTreeComponent oDT = new XIInfraDynamicTreeComponent();
                        Response = oDT.XILoad(oComponentParams);
                        break;
                    case "XIValuesComponent":
                        XIInfraXIValuesComponent oXIV = new XIInfraXIValuesComponent();
                        Response = oXIV.XILoad(oComponentParams);
                        break;
                    case "RulesComponent":
                        XIInfraRulesComponent oRlz = new XIInfraRulesComponent();
                        Response = oRlz.XILoad(oComponentParams);
                        break;
                    case "PaymentComponent":
                        XIInfraPayment oPayment = new XIInfraPayment();
                        Response = oPayment.XILoad(oComponentParams);
                        break;
                    case "AM4FunnelChartComponent":
                        XIInfraAM4FunnelComponent oFunnel = new XIInfraAM4FunnelComponent();
                        Response = oFunnel.XILoad(oComponentParams);
                        break;
                    case "RoleDashBoardComponent":
                        XIInfraRoleDashBoardComponent oADB = new XIInfraRoleDashBoardComponent();
                        Response = oADB.XILoad(oComponentParams);
                        break;
                    default:
                        break;
                }

                //var URL = oXIComponent.sClass.Split('/').ToList().First();
                //var sClass = oXIComponent.sClass.Split('/').ToList().Last();

                //if (!string.IsNullOrEmpty(sClass))
                //{
                //Creating Instance
                //Assembly exceutable;
                //Type Ltype;
                //object objclass;
                //if (oXIComponent.sType.ToLower() == "External".ToLower())
                //{
                //    exceutable = Assembly.Load(URL);
                //    Ltype = exceutable.GetType(URL + "." + sClass);
                //    objclass = Activator.CreateInstance(Ltype);
                //}
                //else
                //{
                //    exceutable = Assembly.GetExecutingAssembly();
                //    Ltype = exceutable.GetType("XIInfrastructure.XIInfraQSComponent");
                //    objclass = Activator.CreateInstance(Ltype);
                //}

                //Invoking Method
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Started" });
                //MethodInfo method = Ltype.GetMethod("XILoad");
                //object[] parametersArray = new object[] { oComponentParams };
                //object Response = (object)method.Invoke(objclass, parametersArray);
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Component XILoad Method Invoke Completed" });
                if (Response != null)
                {
                    oCR = (CResult)Response;
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oCResult.oResult = oCR.oResult;
                    }
                    else
                    {
                        oCResult.oResult = null;
                    }
                }

                //}
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

        public XIDComponent GetParamsByContext(XIDComponent oXIComponent, string sType, string ID)
        {
            int iID = 0;
            Guid XIGUID = Guid.Empty;
            int.TryParse(ID, out iID);
            Guid.TryParse(ID, out XIGUID);
            if (string.IsNullOrEmpty(sType))
            {
                oXIComponent.Params.Clear();
                return oXIComponent;
            }
            List<XIDComponentParam> nParams = new List<XIDComponentParam>();
            if (sType.ToLower() == "QSStep".ToLower())
            {
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iStepDefinitionIDXIGUID == XIGUID).ToList();
                    }
                    else if (iID > 0)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iStepDefinitionID == iID).ToList();
                    }
                    var newParams = oXIComponent.NVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new XIDComponentParam();
                        Newparm.ID = 0;
                        Newparm.sName = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oXIComponent.ID;
                        Newparm.iStepDefinitionID = iID;
                        Newparm.iStepDefinitionIDXIGUID = XIGUID;
                        nParams.Add(Newparm);
                    }
                    oXIComponent.Params = nParams;
                }
                else
                {
                    oXIComponent.Params.Clear();
                }
            }
            else if (sType.ToLower() == "QSStepSection".ToLower())
            {
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    List<XIDComponentParam> Params = new List<XIDComponentParam>();
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iStepSectionIDXIGUID == XIGUID).ToList();
                    }
                    else if (iID > 0)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iStepSectionID == iID).ToList();
                    }
                    var Except = oXIComponent.NVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    Params.AddRange(nParams);
                    foreach (var items in Except)
                    {
                        var NewParam = new XIDComponentParam();
                        NewParam.ID = 0;
                        NewParam.FKiComponentID = oXIComponent.ID;
                        NewParam.iStepSectionID = iID;
                        NewParam.iStepSectionIDXIGUID = XIGUID;
                        NewParam.sName = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        NewParam.sValue = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Params.Add(NewParam);
                    }
                    oXIComponent.Params = Params;
                }
                else
                {
                    oXIComponent.Params.Clear();
                }
            }
            else if (sType.ToLower() == "XiLink".ToLower())
            {
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iXiLinkIDXIGUID == XIGUID).ToList();
                    }
                    else if (iID > 0)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iXiLinkID == iID).ToList();
                    }
                    var newParams = oXIComponent.NVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new XIDComponentParam();
                        Newparm.ID = 0;
                        Newparm.sName = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oXIComponent.ID;
                        Newparm.iXiLinkID = iID;
                        Newparm.iXiLinkIDXIGUID = XIGUID;
                        nParams.Add(Newparm);
                    }
                    oXIComponent.Params = nParams;
                }
                else
                {
                    oXIComponent.Params.Clear();
                }
            }
            else if (sType.ToLower() == "Layout".ToLower())
            {
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iLayoutMappingIDXIGUID == XIGUID).ToList();
                    }
                    else if (iID > 0)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iLayoutMappingID == iID).ToList();
                    }
                    var newParams = oXIComponent.NVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new XIDComponentParam();
                        Newparm.ID = 0;
                        Newparm.sName = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oXIComponent.ID;
                        Newparm.iLayoutMappingID = iID;
                        Newparm.iLayoutMappingIDXIGUID = XIGUID;
                        nParams.Add(Newparm);
                    }
                    oXIComponent.Params = nParams;
                }
                else
                {
                    oXIComponent.Params.Clear();
                }
            }
            else if (sType.ToLower() == "OneClick".ToLower())
            {
                if (iID > 0 || (XIGUID != null && XIGUID != Guid.Empty))
                {
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iQueryIDXIGUID == XIGUID).ToList();
                    }
                    else if (iID > 0)
                    {
                        nParams = oXIComponent.Params.Where(m => m.iQueryID == iID).ToList();
                    }
                    var newParams = oXIComponent.NVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new XIDComponentParam();
                        Newparm.ID = 0;
                        Newparm.sName = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oXIComponent.NVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oXIComponent.ID;
                        Newparm.iQueryID = iID;
                        Newparm.iQueryIDXIGUID = XIGUID;
                        nParams.Add(Newparm);
                    }
                    oXIComponent.Params = nParams;
                }
                else
                {
                    oXIComponent.Params.Clear();
                }
            }
            return oXIComponent;
        }

        public void GetComponentParams(string Type, string iID)
        {
            int PKID = 0;
            Guid UID = Guid.Empty;
            int.TryParse(iID, out PKID);
            Guid.TryParse(iID, out UID);
            Dictionary<string, object> oParams = new Dictionary<string, object>();
            if (PKID > 0)
            {
                oParams["FKiComponentID"] = ID;
                if (Type.ToLower() == "layout")
                {
                    oParams["iLayoutMappingID"] = PKID;
                }
                else if (Type.ToLower() == "xilink")
                {
                    oParams["iXiLinkID"] = PKID;
                }
                else if (Type.ToLower() == "step")
                {
                    oParams["iStepDefinitionID"] = PKID;
                }
                else if (Type.ToLower() == "section")
                {
                    oParams["iStepSectionID"] = PKID;
                }
                else if (Type.ToLower() == "1click")
                {
                    oParams["iQueryID"] = PKID;
                }
                else if (Type.ToLower() == "Attribute".ToLower())
                {
                    oParams["FKiAttributeID"] = PKID;
                }
            }
            else if (UID != null && UID != Guid.Empty)
            {
                oParams["FKiComponentIDXIGUID"] = XIGUID;
                if (Type.ToLower() == "layout")
                {
                    oParams["iLayoutMappingIDXIGUID"] = UID;
                }
                else if (Type.ToLower() == "xilink")
                {
                    oParams["iXiLinkIDXIGUID"] = UID;
                }
                else if (Type.ToLower() == "step")
                {
                    oParams["iStepDefinitionIDXIGUID"] = UID;
                }
                else if (Type.ToLower() == "section")
                {
                    oParams["iStepSectionIDXIGUID"] = UID;
                }
                else if (Type.ToLower() == "1click")
                {
                    oParams["iQueryIDXIGUID"] = UID;
                }
                else if (Type.ToLower() == "Attribute".ToLower())
                {
                    oParams["FKiAttributeID"] = UID;
                }
            }
            Params = Connection.Select<XIDComponentParam>("XIComponentParams_T", oParams).ToList();
        }
        public CResult Preview()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIDQS oQSD = new XIDQS();
                if (sName.ToLower() == XIConstant.QSComponent.ToLower())
                {
                    XIDQSStep oStepD = new XIDQSStep();
                    var iQSDID = Params.Where(m => m.sName.ToLower() == "iqsdid").Select(m => m.sValue).FirstOrDefault();
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString());
                    if (oQSD.Steps != null && oQSD.Steps.Count() > 0)
                    {
                        //oStepD = oQSD.Steps.Values.Where(m => m.sIsHidden == "off" || m.sIsHidden == null).OrderBy(m => m.iOrder).FirstOrDefault();
                        oStepD = oQSD.Steps.Values.Where(m => m.sIsHidden == "off" || m.sIsHidden == null).OrderBy(m => m.ID).FirstOrDefault();
                        if (oStepD != null)
                        {
                            oStepD.oDefintion = oStepD;
                            var oStepContent = oStepD.Preview();
                            if (oStepContent.bOK && oStepContent.oResult != null)
                            {
                                //oQSD.oContent[XIConstant.ContentStep] = oStepContent.oResult;
                                var oSteDef = (XIDQSStep)oStepContent.oResult;
                                if (oSteDef.iLayoutID > 0)
                                {
                                    XIDLayout oLayoutD = new XIDLayout();
                                    oLayoutD.ID = oSteDef.iLayoutID;
                                    var oLayContent = oLayoutD.Preview();
                                    if (oLayContent.bOK && oLayContent.oResult != null)
                                    {
                                        oStepD.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                                    }
                                    oQSD.oContent[XIConstant.ContentQuestionSet] = oStepD;
                                }
                            }
                        }
                    }
                    this.oContent[XIConstant.ContentQuestionSet] = oQSD;
                }
                else
                {
                    this.oContent[XIConstant.ContentQuestionSet] = this;
                }
                oCResult.oResult = this;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading XiLink Definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Save_ComponentParams(string sContext, string UID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Save Component params by context";//expalin about this method logic
            try
            {
                int iID = 0;
                int.TryParse(UID, out iID);
                Guid guid = Guid.Empty;
                Guid.TryParse(UID, out guid);

                oTrace.oParams.Add(new CNV { sName = "sContext", sValue = sContext });
                oTrace.oParams.Add(new CNV { sName = "UID", sValue = UID });
                var ParamIDs = string.Empty;
                var IDs = new List<string>();
                if (!string.IsNullOrEmpty(sContext))//check mandatory params are passed or not
                {
                    if (Params != null && Params.Count() > 0)
                    {
                        XIConfigs oConfig = new XIConfigs();
                        foreach (var items in Params.Where(m => m.sValue != null || (m.XIGUID != null && m.XIGUID != Guid.Empty)).ToList())
                        {
                            XIDComponentParam oParam = new XIDComponentParam();
                            oParam.XIGUID = items.XIGUID;
                            oParam.ID = items.ID;
                            oParam.sName = items.sName;
                            oParam.sValue = items.sValue;
                            oParam.FKiComponentID = ID;
                            oParam.FKiComponentIDXIGUID = XIGUID;
                            if (sContext.ToLower() == "Layout".ToLower())
                            {
                                oParam.iLayoutMappingID = iID;
                                oParam.iLayoutMappingIDXIGUID = guid;
                            }
                            else if (sContext.ToLower() == "XiLink".ToLower())
                            {
                                oParam.iXiLinkID = iID;
                                oParam.iXiLinkIDXIGUID = guid;
                            }
                            else if (sContext.ToLower() == "QSStep".ToLower())
                            {
                                oParam.iStepDefinitionID = iID;
                                oParam.iStepDefinitionIDXIGUID = guid;
                            }
                            else if (sContext.ToLower() == "QSStepSection".ToLower())
                            {
                                oParam.iStepSectionID = iID;
                                oParam.iStepSectionIDXIGUID = guid;
                            }
                            else if (sContext.ToLower() == "Query".ToLower())
                            {
                                oParam.iQueryID = iID;
                                oParam.iQueryIDXIGUID = guid;
                            }
                            oCR = oConfig.Save_ComponentParam(oParam);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var ParamI = (XIIBO)oCR.oResult;
                                if (ParamI.Attributes.ContainsKey("xiguid"))
                                {
                                    IDs.Add(ParamI.AttributeI("xiguid").sValue);
                                }
                                else if (ParamI.Attributes.ContainsKey("id"))
                                {
                                    IDs.Add(ParamI.AttributeI("id").sValue);
                                }
                                oTrace.oTrace.Add(oCR.oTrace);
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.oResult = "Failure";
                            }
                        }
                        if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                        {
                            ParamIDs = string.Join(",", IDs);
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = ParamIDs;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                        oTrace.sMessage = "Params missing";
                        oCResult.oResult = "Failure";
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sContext =" + sContext + " or iID=" + iID + " is missing";
                    oCResult.oResult = "Failure";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }

    public class XIDComponentsNV
    {
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sType { get; set; }
    }

    public class XIDComponentParam
    {
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public Guid FKiComponentIDXIGUID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
        public int iLayoutMappingID { get; set; }
        public int iXiLinkID { get; set; }
        public int iStepDefinitionID { get; set; }
        public int iStepSectionID { get; set; }
        public int iQueryID { get; set; }
        public Guid iStepDefinitionIDXIGUID { get; set; }
        public Guid iLayoutMappingIDXIGUID { get; set; }
        public Guid iQueryIDXIGUID { get; set; }
        public Guid iStepSectionIDXIGUID { get; set; }
        public Guid iXiLinkIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
        public string sValueTypeIcon { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }
    }

    public class XIDComponentTrigger
    {
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
    }
}