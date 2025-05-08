using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static XIDatabase.XIDBAPI;
using XISystem;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    [Table("XILayout_T")]
    public class XIDLayout : XIInstanceBase
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Please enter layout name ")]
        [StringLength(512, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 2)]
        public string LayoutName { get; set; }
        [Required(ErrorMessage = "Please enter layout structure code ")]
        public string LayoutCode { get; set; }
        [Required(ErrorMessage = "Select Layout Type")]
        public string LayoutType { get; set; }
        [Required(ErrorMessage = "Select Layout Level")]
        public string LayoutLevel { get; set; }
        public string Authentication { get; set; }
        public int XiParameterID { get; set; }
        public Guid XiParameterIDXIGUID { get; set; }
        public int iThemeID { get; set; }
        public Guid iThemeIDXIGUID { get; set; }
        public string sSiloAccess { get; set; }
        public bool bIsTaskBar { get; set; }
        public string sTaskBarPosition { get; set; }
        public bool bAddToParentTaskbar { get; set; }
        public bool bIsFluid { get; set; }
        public string sCssClass { get; set; }
        [DapperIgnore]
        public List<string> arrSiloAccess { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlXiParameters { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlXIThemes { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlApplications { get; set; }
        [NotMapped]
        public string sThemeName { get; set; }
        [NotMapped]
        public string sGUID { get; set; }
        [DapperIgnore]
        public string sNewGUID { get; set; }
        [DapperIgnore]
        public string sContext { get; set; }
        [DapperIgnore]
        public string sID { get; set; }
        [NotMapped]
        public List<XIDropDown> LayoutsList { get; set; }
        [NotMapped]
        public List<XIDropDown> LayoutThemes { get; set; }
        [NotMapped]
        public int iNotificationCount { get; set; }
        [NotMapped]
        public Dictionary<string, XIIBO> nNotificationInstance { get; set; }

        public List<CNV> oLayoutParams = new List<CNV>();

        private List<XIDLayoutDetails> oMyLayoutDetails = new List<XIDLayoutDetails>();

        public List<XIDLayoutDetails> LayoutDetails
        {
            get
            {
                return oMyLayoutDetails;
            }
            set
            {
                oMyLayoutDetails = value;
            }
        }

        private List<XIDLayoutMapping> oMyLayoutMappings = new List<XIDLayoutMapping>();

        public List<XIDLayoutMapping> LayoutMappings
        {
            get
            {
                return oMyLayoutMappings;
            }
            set
            {
                oMyLayoutMappings = value;
            }
        }

        public bool bUseParentGUID { get; set; }
        //My Code
        public int StatusTypeID { get; set; }

        [DapperIgnore]
        public List<XIDropDown> XiLinks { get; set; }
        [DapperIgnore]
        public List<XIDropDown> XIComponents { get; set; }
        [DapperIgnore]
        public List<XIDropDown> Dialogs { get; set; }
        [DapperIgnore]
        public List<XIDropDown> Popups { get; set; }
        [DapperIgnore]
        public List<XIDLayoutMapping> Mappings { get; set; }
        [DapperIgnore]
        public List<XIDropDown> Steps { get; set; }
        [DapperIgnore]
        public string PopupName { get; set; }
        [DapperIgnore]
        public int PopupID { get; set; }
        [DapperIgnore]
        public int DialogID { get; set; }
        [DapperIgnore]
        public Guid DialogIDXIGUID { get; set; }
        //My code 3/12/2018

        [DapperIgnore]
        public List<XIDLayoutDetails> Details { get; set; }

        public Guid XIGUID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }

        public CResult oCResult = new CResult();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);


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
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                //XIInstanceBase oInstBase = new XIInstanceBase();
                XIInfraCache oCache = new XIInfraCache();
                XIDLayout oLayoutD = new XIDLayout();
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                XIDXI oXIID = new XIDXI();
                var oUserPreference = oXIID.Get_UserPreference(oInfo.iUserID.ToString(), oInfo.sCoreDataBase);
                if (oUserPreference.oResult != null)
                {
                    var result = (XIInfraUserPreference)oUserPreference.oResult;
                    oLayoutParams.Add(new CNV { sName = "{XIP|iUserPreferenceID}", sValue = result.ID.ToString(), sType = "Default" });
                }
                if (XIGUID != null && XIGUID != Guid.Empty)
                {
                    oLayoutD = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, XIGUID.ToString());
                }
                else if (ID > 0)
                {
                    oLayoutD = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, ID.ToString());
                }
                if (oLayoutD != null && (oLayoutD.ID > 0 || (oLayoutD.XIGUID != null && oLayoutD.XIGUID != Guid.Empty)))
                {
                    string sSessionID = HttpContext.Current.Session.SessionID;
                    XIDLayout oLayoutC = (XIDLayout)oLayoutD.Clone(oLayoutD);
                    string sParentGUID = sGUID;
                    if (!oLayoutC.bUseParentGUID)
                    {
                        sGUID = string.Empty;
                        Singleton.Instance.sGUID = null;
                        Singleton.Instance.sActiveGUID.Remove(sSessionID);
                    }
                    else
                    {

                    }
                    //if (Singleton.Instance.sActiveGUID.ContainsKey(sSessionID))
                    //{
                    //    sGUID = Singleton.Instance.sActiveGUID[sSessionID];
                    //}
                    if (string.IsNullOrEmpty(sGUID))
                    {
                        //Singleton.Instance.GetGuid();
                        Singleton.Instance.sActiveGUID[sSessionID] = Guid.NewGuid().ToString(); //Singleton.Instance.sGUID;
                        //sGUID = Singleton.Instance.sGUID;
                        sGUID = Guid.NewGuid().ToString();//Singleton.Instance.sActiveGUID[sSessionID]; // not Required checked with ravi
                        oLayoutC.sGUID = sGUID;
                    }
                    else
                    {
                        oLayoutC.sGUID = Guid.NewGuid().ToString();
                        //Singleton.Instance.oParentGUID[oLayoutC.sGUID] = sGUID; // not Required checked with ravi
                    }
                    if (!string.IsNullOrEmpty(sContext) && sContext == "XILink")
                    {
                        if (string.IsNullOrEmpty(sParentGUID))
                        {
                            sParentGUID = oLayoutC.sGUID;
                        }
                        sGUID = oLayoutC.sGUID;
                    }
                    var AppID = HttpContext.Current.Session["ApplicationID"];
                    if (oLayoutParams != null && oLayoutParams.Count() > 0)
                    {
                        oLayoutParams.Add(new CNV { sName = "{XIP|iUserID}", sValue = oInfo.iUserID.ToString(), sType="Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sUserName}", sValue = oInfo.sUserName, sType = "Default" });
                        var Roleid = oCache.Get_ParamVal(sSessionID, sParentGUID, null, "{XIP|iRoleID}");
                        if (!string.IsNullOrEmpty(Roleid))
                        {
                            oLayoutParams.Add(new CNV { sName = "{XIP|iRoleID}", sValue = Roleid.ToString(), sType = "Default" });
                        }
                        else
                        {
                        oLayoutParams.Add(new CNV { sName = "{XIP|iRoleID}", sValue = oInfo.iRoleID.ToString(), sType = "Default" });
                        }
                        oLayoutParams.Add(new CNV { sName = "{XIP|sName}", sValue = oInfo.sName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iAppID}", sValue = AppID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iAppIDXIGUID}", sValue = oInfo.iApplicationIDXIGUID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iOrgID}", sValue = oInfo.iOrganizationID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "sConfigDatabase", sValue = oInfo.sDatabaseName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sConfigDatabase}", sValue = oInfo.sDatabaseName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "sCoreDatabase", sValue = oInfo.sCoreDataBase, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sCoreDatabase}", sValue = oInfo.sCoreDataBase, sType = "Default" });
                        var sMainDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                        oLayoutParams.Add(new CNV { sName = "sMainDatabase", sValue = sMainDatabase, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sMainDatabase}", sValue = sMainDatabase, sType = "Default" });
                        var childaddpopup = oLayoutParams.Where(m => m.sName.ToLower() == "childaddpopup").Select(m => m.sValue).FirstOrDefault();
                        if(!string.IsNullOrEmpty(childaddpopup) && childaddpopup.ToLower() == "yes")
                        {
                            oLayoutParams.Add(new CNV { sName = "parentxiguid", sValue = sParentGUID, sType = "Default" });
                        }
                        oCache.MergeXILinkParameters(null, sGUID, oLayoutParams, sSessionID);
                    }
                    else
                    {
                        oLayoutParams = new List<CNV>();
                        oLayoutParams.Add(new CNV { sName = "{XIP|iUserID}", sValue = oInfo.iUserID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sUserName}", sValue = oInfo.sUserName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iRoleID}", sValue = oInfo.iRoleID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sName}", sValue = oInfo.sName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iAppID}", sValue = AppID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iAppIDXIGUID}", sValue = oInfo.iApplicationIDXIGUID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|iOrgID}", sValue = oInfo.iOrganizationID.ToString(), sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "parentxiguid", sValue = sParentGUID, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "sConfigDatabase", sValue = oInfo.sDatabaseName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sConfigDatabase}", sValue = oInfo.sDatabaseName, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "sCoreDatabase", sValue = oInfo.sCoreDataBase, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sCoreDatabase}", sValue = oInfo.sCoreDataBase, sType = "Default" });
                        var sMainDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                        oLayoutParams.Add(new CNV { sName = "sMainDatabase", sValue = sMainDatabase, sType = "Default" });
                        oLayoutParams.Add(new CNV { sName = "{XIP|sMainDatabase}", sValue = sMainDatabase, sType = "Default" });
                        //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|XI Application.id}", oInfo.iApplicationID.ToString(), null, null);
                        oCache.MergeXILinkParameters(null, sGUID, oLayoutParams, sSessionID);
                    }
                    string sChildGUID = string.Empty;
                    if (oLayoutC.XiParameterIDXIGUID != null && oLayoutC.XiParameterIDXIGUID != Guid.Empty)
                    {
                        sChildGUID = oLayoutC.sGUID;
                        oCache.AddParamsToGUID(oLayoutC.XiParameterIDXIGUID.ToString(), sParentGUID, sChildGUID);
                    }
                    else if (oLayoutC.XiParameterID > 0)
                    {
                        sChildGUID = oLayoutC.sGUID;
                        oCache.AddParamsToGUID(oLayoutC.XiParameterID.ToString(), sParentGUID, sChildGUID);
                    }
                    foreach (var items in oLayoutC.LayoutMappings)
                    {
                        if (items.ContentType.ToLower() == XIConstant.ContentXILink)
                        {
                            if (items.XiLinkID > 0 || (items.XiLinkIDXIGUID != null && items.XiLinkIDXIGUID != Guid.Empty))
                            {
                                XIILink oXIL = new XIILink();
                                oXIL.sGUID = sGUID;
                                oXIL.iXILinkID = items.XiLinkID;
                                oXIL.XILinkGUID = items.XiLinkIDXIGUID;
                                var oInnerRes = oXIL.Load();
                                if (oInnerRes.bOK && oInnerRes.oResult != null)
                                {
                                    items.oContent[XIConstant.ContentXILink] = (XIILink)oInnerRes.oResult;
                                }
                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentXIComponent)
                        {
                            if (items.XiLinkID > 0 || (items.XiLinkIDXIGUID != null && items.XiLinkIDXIGUID != Guid.Empty))
                            {
                                XIIComponent oXICompI = new XIIComponent();
                                Guid ComponentGUID = Guid.Empty;
                                if (items.XiLinkIDXIGUID != null && items.XiLinkIDXIGUID != Guid.Empty)
                                {
                                    Guid.TryParse(items.XiLinkIDXIGUID.ToString(), out ComponentGUID);
                                }
                                XIDComponent oXIComponent = new XIDComponent();
                                if (ComponentGUID != null && ComponentGUID != Guid.Empty)
                                {
                                    oXIComponent = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, ComponentGUID.ToString());
                                }
                                else if (items.XiLinkID > 0)
                                {
                                    oXIComponent = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, items.XiLinkID.ToString());
                                }
                                oXIComponent = (XIDComponent)oXIComponent.Clone(oXIComponent);
                                if (items.PlaceHolderIDXIGUID != null && items.PlaceHolderIDXIGUID != Guid.Empty)
                                {
                                    oXIComponent.GetComponentParams("Layout", items.PlaceHolderIDXIGUID.ToString());
                                }
                                else if (items.PlaceHolderID > 0)
                                {
                                    oXIComponent.GetComponentParams("Layout", items.PlaceHolderID.ToString());
                                }
                                if (oXIComponent != null && oXIComponent.Params != null && oXIComponent.Params.Count() > 0)
                                {
                                    oXICompI.sGUID = sGUID;
                                    oXICompI.oDefintion = oXIComponent;
                                    oXICompI.sCallHierarchy = "Layout_" + oLayoutC.ID + ":LayoutMappingID_" + items.ID + ":Component_" + items.XiLinkID;
                                    var Data = oXICompI.Load();
                                    //var Instanceb = (XIIComponent)oXICompI.Load().oResult;
                                    //oXIComponent.oContent = oXICompI.oContent;
                                    if (Data != null && Data.oResult != null)
                                    {
                                        items.oContent[XIConstant.ContentXIComponent] = (XIIComponent)Data.oResult;
                                    }
                                }

                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentStep)
                        {
                            XIIQSStep oStepI = new XIIQSStep();
                            oStepI.ID = items.XiLinkID;
                            XIDQSStep oXIStepC = new XIDQSStep();
                            XIDQSStep oXIStepD = new XIDQSStep();
                            int iStepID = 0;
                            int.TryParse(items.XiLinkID.ToString(), out iStepID);
                            Guid StepGUID = Guid.Empty;
                            if (items.XiLinkIDXIGUID != null && items.XiLinkIDXIGUID != Guid.Empty)
                            {
                                Guid.TryParse(items.XiLinkIDXIGUID.ToString(), out StepGUID);
                            }
                            if (StepGUID != null && StepGUID != Guid.Empty)
                            {
                                oXIStepD = (XIDQSStep)oCache.GetObjectFromCache(XIConstant.CacheQSStep, null, StepGUID.ToString(), sSessionID, sGUID);
                            }
                            else if (iStepID > 0)
                            {
                                oXIStepD = (XIDQSStep)oCache.GetObjectFromCache(XIConstant.CacheQSStep, null, items.XiLinkID.ToString(), sSessionID, sGUID);
                            }
                            //oXIStepC = (XIDQSStep)oXIStepD.Clone(oXIStepD);
                            oStepI.oDefintion = oXIStepD;
                            oStepI.sGUID = sGUID;
                            //oStepI.FKStepGUID = items.XiLinkIDGUID;
                            var oCresult = oStepI.Load();

                            if (oCresult.bOK && oCresult.oResult != null)
                            {
                                items.oContent[XIConstant.ContentStep] = (XIIQSStep)oCresult.oResult;
                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentHTML)
                        {
                            items.oContent[XIConstant.ContentHTML] = items.HTMLCode;
                        }
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    //oInstBase.oContent[XIConstant.ContentLayout] = oLayoutC;
                    oCResult.oResult = oLayoutC;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout Content" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
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
                XIDefinitionBase oDefinition = new XIDefinitionBase();
                XIInfraCache oCache = new XIInfraCache();
                XIDLayout oLayoutD = new XIDLayout();
                oLayoutD = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, ID.ToString());
                if (oLayoutD != null)
                {
                    XIDLayout oLayoutC = (XIDLayout)oLayoutD.Clone(oLayoutD);
                    foreach (var items in oLayoutC.LayoutMappings)
                    {
                        if (items.ContentType.ToLower() == XIConstant.ContentXILink)
                        {
                            if (items.XiLinkID > 0)
                            {
                                XIILink oXIL = new XIILink();
                                oXIL.iXILinkID = items.XiLinkID;
                                var oInnerRes = oXIL.Preview();
                                if (oInnerRes.bOK && oInnerRes.oResult != null)
                                {
                                    items.oContent[XIConstant.ContentXILink] = oInnerRes.oResult;
                                }
                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentXIComponent)
                        {
                            if (items.XiLinkID > 0)
                            {
                                XIIComponent oXICompI = new XIIComponent();
                                XIDComponent oXIComponent = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, items.XiLinkID.ToString());
                                oXIComponent = (XIDComponent)oXIComponent.Clone(oXIComponent);
                                oXIComponent.GetComponentParams("Layout", items.PlaceHolderID.ToString());
                                if (oXIComponent != null)
                                {
                                    if (oXIComponent.sName.ToLower() == XIConstant.QSComponent.ToLower())
                                    {
                                        var oCompContent = oXIComponent.Preview();
                                        if (oCompContent.bOK && oCompContent.oResult != null)
                                        {
                                            items.oContent[XIConstant.ContentXIComponent] = oCompContent.oResult;
                                        }
                                    }
                                    else
                                    {
                                        items.oContent[XIConstant.ContentXIComponent] = oXIComponent;
                                    }
                                }
                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentStep)
                        {
                            XIDQSStep oStepD = new XIDQSStep();
                            oStepD = (XIDQSStep)oCache.GetObjectFromCache(XIConstant.CacheQSStep, null, items.XiLinkID.ToString());
                            if (oStepD != null)
                            {
                                oStepD.oDefintion = oStepD;
                                oStepD.ID = items.XiLinkID;
                                var oStepContent = oStepD.Preview();
                                if (oStepContent.bOK && oStepContent.oResult != null)
                                {
                                    items.oContent[XIConstant.ContentStep] = oStepContent.oResult;
                                }
                            }
                        }
                        else if (items.ContentType.ToLower() == XIConstant.ContentHTML)
                        {
                            items.oContent[XIConstant.ContentHTML] = items.HTMLCode;
                        }
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oDefinition.oContent[XIConstant.ContentLayout] = oLayoutC;
                    oCResult.oResult = oDefinition;
                }
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

        //My Code
        public XIDLayout Get_XILayoutDetails()
        {
            XIDLayout oXILayout = new XIDLayout();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {

                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    if (XIGUID != null && XIGUID != Guid.Empty)
                    {
                        Params["XIGUID"] = XIGUID;
                        oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                    }
                    else if (ID != 0)
                    {
                        Params["ID"] = ID;
                        oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(LayoutName))
                    {
                        Params["LayoutName"] = LayoutName;
                        oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                    }
                    if (ID == 0)
                    {
                        ID = oXILayout.ID;
                    }
                    //Get all the List of layout details
                    Dictionary<string, object> LayoutDetailsParams = new Dictionary<string, object>();
                    List<XIDLayoutDetails> oXILayoutDetails = new List<XIDLayoutDetails>();
                    oXILayoutDetails = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", LayoutDetailsParams).ToList();
                    var Details = oXILayoutDetails.Where(m => m.LayoutIDXIGUID == oXILayout.XIGUID).Select(m => new XIDLayoutDetails { LayoutID = ID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
                    oXILayout.LayoutDetails = Details;
                    //Common GetApplicationsDDL
                    List<XIDropDown> AllBOs = new List<XIDropDown>();
                    List<XIDApplication> oXIApp = new List<XIDApplication>();
                    Dictionary<string, object> ApplParams = new Dictionary<string, object>();
                    oXIApp = Connection.Select<XIDApplication>("XIApplication_T", ApplParams).ToList();
                    AllBOs = (from c in oXIApp
                              select new XIDropDown { text = c.sApplicationName, Value = c.ID }).ToList();
                    oXILayout.ddlApplications = AllBOs;
                    //Common GetXiParametersDDL and GetThemesDDL
                    // Layouts.ddlXiParameters = Common.GetXiParametersDDL(sDatabase);
                    List<XIDropDown> XiParameters = new List<XIDropDown>();
                    List<XIParameter> oXIParameters = new List<XIParameter>();
                    Dictionary<string, object> XIParams = new Dictionary<string, object>();
                    oXIParameters = Connection.Select<XIParameter>("XIParameters", XIParams).ToList();
                    XiParameters = oXIParameters.Where(m => m.StatusTypeID == 10).Select(m => new XIDropDown { Value = m.XiParameterID, text = m.Name }).ToList();
                    XiParameters.Insert(0, new XIDropDown { Value = 0, text = "--Select--" });
                    oXILayout.ddlXiParameters = XiParameters;
                    // Layouts.ddlXIThemes = Common.GetThemesDDL(sDatabase);
                    List<XIDropDown> ThemeTypes = new List<XIDropDown>();
                    ///check if theme class exists
                    List<XIDMasterData> oXIMasterData = new List<XIDMasterData>();
                    Dictionary<string, object> XIThemeParams = new Dictionary<string, object>();
                    oXIMasterData = Connection.Select<XIDMasterData>("XIMasterData_T", XIThemeParams).ToList();
                    ThemeTypes = oXIMasterData.Where(m => m.Name.ToLower() == "Themes".ToLower()).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.Expression }).ToList();
                    oXILayout.ddlXIThemes = ThemeTypes;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oXILayout;
        }

        ////Layout Mapping
        public XIDLayout Get_XILayoutMapDetails()
        {
            List<XIDLayoutDetails> oXILayoutDtls = new List<XIDLayoutDetails>();
            XIDLayoutDetails oXILayoutDtl = new XIDLayoutDetails();
            XIDLayout oXILayout = new XIDLayout();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    string sPK = "ID";
                    //Dictionary<string, object> LayoutDetailParams = new Dictionary<string, object>();
                    //if (ID != 0)
                    //{
                    //    LayoutDetailParams["LayoutID"] = ID;
                    //}
                    //oXILayoutDtl = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", LayoutDetailParams).FirstOrDefault();
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    if (ID != 0)
                    {
                        Params["ID"] = ID;
                    }
                    else if (!string.IsNullOrEmpty(sID))
                    {
                        int iLayoutID = 0;
                        int.TryParse(sID, out iLayoutID);
                        if (iLayoutID > 0)
                        {
                            Params["ID"] = sID;
                        }
                        else
                        {
                            sPK = "XIGUID";
                            Params["XIGUID"] = sID;
                        }
                    }
                    oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                    Dictionary<string, object> LayoutDetailsParams = new Dictionary<string, object>();
                    oXILayoutDtls = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", LayoutDetailsParams).ToList();
                    var Details = new List<XIDLayoutDetails>();
                    if (sPK == "ID")
                    {

                        Details = oXILayoutDtls.Where(m => m.LayoutID == ID).Select(m => new XIDLayoutDetails { LayoutID = ID, LayoutIDXIGUID = oXILayout.XIGUID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceHolderIDXIGUID = m.XIGUID, PlaceholderUniqueName = m.PlaceholderUniqueName, sCssClass = m.sCssClass }).ToList();
                    }
                    else
                    {
                        Guid oGUID = new Guid();
                        Guid.TryParse(sID, out oGUID);
                        Details = oXILayoutDtls.Where(m => m.LayoutIDXIGUID == oGUID).Select(m => new XIDLayoutDetails { LayoutID = ID, LayoutIDXIGUID = oXILayout.XIGUID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceHolderIDXIGUID = m.XIGUID, PlaceholderUniqueName = m.PlaceholderUniqueName, sCssClass=m.sCssClass }).ToList();

                    }
                    //var Details = oXILayoutDtls.Where(m => m.LayoutID == ID).Select(m => new XIDLayoutDetails { LayoutID = ID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
                    oXILayout.Details = Details;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oXILayout;
        }


        ////Inline Mapping
        public XIDLayout Get_XIInlineMapDetails()
        {
            List<XIDLayoutMapping> oXILayoutMapList = new List<XIDLayoutMapping>();
            XIDLayoutDetails oXILayoutDtl = new XIDLayoutDetails();
            XIDLayout oXILayout = new XIDLayout();
            List<XIDLayoutDetails> oXIDLayoutDetailsList = new List<XIDLayoutDetails>();
            try
            {
                if (this.oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    string sPK = "ID";
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    if (ID != 0)
                    {
                        Params["ID"] = ID;
                    }
                    else if (!string.IsNullOrEmpty(sID))
                    {
                        int iLayoutID = 0;
                        int.TryParse(sID, out iLayoutID);
                        if (iLayoutID > 0)
                        {
                            Params["ID"] = sID;
                        }
                        else
                        {
                            sPK = "XIGUID";
                            Params["XIGUID"] = sID;
                        }
                    }
                    oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();//.Select(m => new XIDLayout { LayoutCode = m.LayoutCode, ID = m.ID, LayoutName = m.LayoutName, LayoutType = m.LayoutType }).FirstOrDefault();
                    Dictionary<string, object> LayoutDetailsParams = new Dictionary<string, object>();

                    if (sPK == "ID")
                    {
                        LayoutDetailsParams["LayoutID"] = sID;
                    }
                    else
                    {
                        LayoutDetailsParams["LayoutIDXIGUID"] = sID;
                    }
                    oXIDLayoutDetailsList = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", LayoutDetailsParams).ToList();
                    //var Details = oXIDLayoutDetailsList.Where(m => m.LayoutID == ID).Select(m => new XIDLayoutDetails { LayoutID = ID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
                    oXILayout.Details = oXIDLayoutDetailsList;
                    //added type as extra value compared to the old cde as the edit form dosnt have value for type dropdown

                    if (oXILayout.Details != null && oXILayout.Details.Count() > 0)
                    {
                        oXILayout.Mappings = new List<XIDLayoutMapping>();
                        foreach (var detail in oXILayout.Details)
                        {
                            Dictionary<string, object> MapParams = new Dictionary<string, object>();
                            MapParams["PopupLayoutIDXIGUID"] = oXILayout.XIGUID;
                            MapParams["PlaceHolderIDXIGUID"] = detail.XIGUID;
                            MapParams["StatusTypeID"] = "10";
                            var Mapping = Connection.Select<XIDLayoutMapping>("XILayoutMapping_T", MapParams).FirstOrDefault();
                            if (Mapping != null && Mapping.ID > 0)
                            {
                                oXILayout.Mappings.Add(Mapping);
                            }
                        }
                    }

                    //Dictionary<string, object> MapParams = new Dictionary<string, object>();
                    //oXILayoutMapList = Connection.Select<XIDLayoutMapping>("XILayoutMapping_T", MapParams).ToList();
                    //var MapDetails = oXILayoutMapList.Where(m => m.PopupLayoutID == ID).Select(m => new XIDLayoutMapping { Type = m.Type, PopupLayoutID = m.PopupLayoutID, PlaceHolderID = m.PlaceHolderID, XiLinkID = m.XiLinkID, StatusTypeID = m.StatusTypeID, ContentType = m.ContentType, HTMLCode = m.HTMLCode }).ToList();
                    //oXILayout.Mappings = MapDetails;

                    if (!string.IsNullOrEmpty(sID))
                    {
                        Dictionary<string, object> oLayDailogDet = new Dictionary<string, object>();
                        if (sPK == "ID")
                        {
                            oLayDailogDet["LayoutID"] = sID;
                        }
                        else
                        {
                            oLayDailogDet["LayoutIDXIGUID"] = sID;
                        }
                        var oLayDailogDetDef = Connection.Select<XIDDialog>("XIDialog_T", oLayDailogDet).FirstOrDefault();
                        if (oLayDailogDetDef != null)
                        {
                            oXILayout.DialogID = oLayDailogDetDef.ID;
                            oXILayout.DialogIDXIGUID = oLayDailogDetDef.XIGUID;
                        }
                    }

                    //List<XIDropDown> Dialogs = new List<XIDropDown>();
                    //Dictionary<string, object> DialogsParams = new Dictionary<string, object>();
                    //var DialogsList = Connection.Select<XIDDialog>("XIDialog_T", DialogsParams).ToList();
                    //Dialogs = DialogsList.Where(m => m.StatusTypeID == 10).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.DialogName }).ToList();
                    //oXILayout.Dialogs = Dialogs;
                    //List<XIDropDown> XiLinks = new List<XIDropDown>();
                    //Dictionary<string, object> XILinkParams = new Dictionary<string, object>();
                    //var XILinkList = Connection.Select<XILink>("XILink_T", XILinkParams).ToList();
                    //XiLinks = XILinkList.Where(m => m.StatusTypeID == 10).ToList().Select(m => new XIDropDown { Value = m.XiLinkID, text = m.Name }).ToList();
                    //oXILayout.XiLinks = XiLinks;

                    List<XIDropDown> XIComponents = new List<XIDropDown>();
                    Dictionary<string, object> XICompParams = new Dictionary<string, object>();
                    var XIComponentDetails = Connection.Select<XIDComponent>("XIComponents_XC_T", XICompParams).ToList();
                    XIComponents = XIComponentDetails.Where(m => m.StatusTypeID == 10).ToList().Select(m => new XIDropDown { Value = m.ID, text = m.sName }).ToList();
                    oXILayout.XIComponents = XIComponents;

                    //List<XIDropDown> QSStepTemplates = new List<XIDropDown>();
                    //Dictionary<string, object> XIQSParams = new Dictionary<string, object>();
                    //var XIStepDetails = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", XIQSParams).ToList();

                    //QSStepTemplates = (from c in XIStepDetails.ToList()
                    //                   select new XIDropDown { text = c.sName, Value = c.ID }).ToList();
                    //oXILayout.Steps = QSStepTemplates;

                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oXILayout;
        }


        //Saving Layout details
        public CResult Save_Layout(string sCoreDatabase)
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
                XIDLayout Layout = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Layout = Connection.Update<XIDLayout>(this, "XILayout_T", "ID");
                oCResult.oResult = Layout;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult Get_LayoutByName()
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
                XIDLayout oXILayout = new XIDLayout();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(LayoutName))
                {
                    Params["LayoutName"] = LayoutName;
                    oXILayout = Connection.Select<XIDLayout>("XILayout_T", Params).FirstOrDefault();
                }
                oCResult.oResult = oXILayout;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}
