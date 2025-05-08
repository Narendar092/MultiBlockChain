using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using XIDatabase;
using XISystem;
using static iText.Kernel.Pdf.Colorspace.PdfPattern;

namespace XICore
{
    public class XIILink : XIInstanceBase
    {
        public long iXILinkID;
        public Guid XILinkGUID;
        string sStartAction;
        long i1ClickID = 0;
        string sSearchType;
        string sBOName;
        long iBODID;
        long iBOIID;
        string sGroup;
        long iDialogID;
        Guid DialogGUID;
        long iPopupID;
        string sMenu;
        Guid MenuGUID;
        long iQSDID;
        string sQSType;//Tells public or private QuestionSet
        string sMode;
        string sStepName;
        long iLayoutID;
        Guid LayoutGUID;
        string sVisualisation;
        string sContentType;//Not sure of usage
        string sBespokeURL;//Not used till now
        string sDatabase;
        public string sGUID;
        public bool bIsDialog;
        public string sOutput;
        public List<CNV> oParams = new List<CNV>();

        XIInfraCache oCache = new XIInfraCache();

        public XILink oXILinkD = new XILink();

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
                XIILink oXILinkI = new XIILink();
                XILink oXILinkC = new XILink();
                if (iXILinkID > 0 || (XILinkGUID != null && XILinkGUID != Guid.Empty))
                {
                    if (XILinkGUID != null && XILinkGUID != Guid.Empty)
                    {
                        oXILinkD = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, XILinkGUID.ToString());
                    }
                    else if (iXILinkID > 0)
                    {
                        oXILinkD = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, iXILinkID.ToString());
                    }
                    if (oXILinkD != null)
                    {
                        oXILinkC = (XILink)oXILinkD.Clone(oXILinkD);
                        if (oXILinkC.bMatrix)
                        {
                            if (string.IsNullOrEmpty(oXILinkC.sCode))
                            {
                                oXILinkC.sCode = xiEnumSystem.EnumMatrixAction.XILink.ToString();
                            }
                            XIMatrix oXIMatrix = new XIMatrix();
                            oXIMatrix.MatrixAction(oXILinkC.sCode, xiEnumSystem.EnumMatrixAction.XILink, "", 0, oXILinkD.XIGUID.ToString(), oXILinkD.Name, oParams);
                        }
                        //if(oXILinkC.XiLinkNVs != null && oXILinkC.XiLinkNVs.Count() > 0)
                        //{
                        //    oCache.MergeXILinkParameters(oXILinkC, oXILinkI.sGUID);
                        //}                        
                        XIRun(oXILinkC);
                        if (iDialogID > 0 || (DialogGUID != null && DialogGUID != Guid.Empty))
                        {
                            //XIInstanceBase oDialogBase = new XIInstanceBase();
                            XIDDialog oDialogD = new XIDDialog();
                            if (DialogGUID != null && DialogGUID != Guid.Empty)
                            {
                                oDialogD = (XIDDialog)oCache.GetObjectFromCache(XIConstant.CacheDialog, null, DialogGUID.ToString());
                            }
                            else if (iDialogID > 0)
                            {
                                oDialogD = (XIDDialog)oCache.GetObjectFromCache(XIConstant.CacheDialog, null, iDialogID.ToString());
                            }
                            if (oDialogD != null && (oDialogD.ID > 0 || oDialogD.XIGUID != null && oDialogD.XIGUID != Guid.Empty))
                            {
                                XIDDialog oDialogC = (XIDDialog)oDialogD.Clone(oDialogD);
                                if (oDialogC.LayoutID > 0 || (oDialogC.LayoutIDXIGUID != null && oDialogC.LayoutIDXIGUID != Guid.Empty))
                                {
                                    if (oDialogC.FKiBOID > 0 || (oDialogC.FKiBOIDXIGUID != null && oDialogC.FKiBOIDXIGUID != Guid.Empty))
                                    {
                                        var instID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceIDM.ToLower()).Select(m => m.sValue).FirstOrDefault();
                                        XIIBO oBOI = new XIIBO();
                                        int iID = 0;
                                        Guid uID = Guid.Empty;
                                        int.TryParse(instID, out iID);
                                        Guid.TryParse(instID, out uID);
                                        if (iID > 0 || (uID != null && uID != Guid.Empty))
                                        {
                                            oCR = oBOI.Get_BODialogLabel(oDialogC.FKiBOIDXIGUID.ToString(), instID);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oDialogC.sLabel = (string)oCR.oResult;
                                            }
                                            oCR = oBOI.Get_GroupLabel(oDialogC.FKiBOIDXIGUID.ToString(), instID);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oDialogC.sGroupLabel = (string)oCR.oResult;
                                            }
                                        }
                                    }
                                    XIDLayout oLayoutD = new XIDLayout();
                                    oLayoutD.ID = oDialogC.LayoutID;
                                    if (oParams != null && oParams.Count() > 0)
                                    {
                                        oLayoutD.oLayoutParams = oParams;
                                    }
                                    oLayoutD.XIGUID = oDialogC.LayoutIDXIGUID;
                                    oLayoutD.sGUID = sGUID;
                                    oLayoutD.sContext = "XILink";
                                    var oLayContent = oLayoutD.Load();
                                    if (oLayContent.bOK && oLayContent.oResult != null)
                                    {
                                        oDialogC.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                                        //oDialogBase.oContent[XIConstant.ContentDialog] = oDialogC;
                                    }

                                }

                                oXILinkI.oContent[XIConstant.ContentDialog] = oDialogC;
                            }
                        }
                        else if (iPopupID > 0)
                        {
                            XIDPopup oPopupD;
                            oPopupD = (XIDPopup)oCache.GetObjectFromCache(XIConstant.CachePopup, null, iPopupID.ToString());
                            if (oPopupD != null)
                            {
                                XIDPopup oPopupC = (XIDPopup)oPopupD.Clone(oPopupD);
                                if (oPopupC.LayoutID > 0)
                                {
                                    XIDLayout oLayoutD = new XIDLayout();
                                    oLayoutD.ID = oPopupC.LayoutID;
                                    if (oParams != null && oParams.Count() > 0)
                                    {
                                        oLayoutD.oLayoutParams = oParams;
                                    }
                                    var oLayContent = oLayoutD.Load();
                                    if (oLayContent.bOK && oLayContent.oResult != null)
                                    {
                                        oPopupC.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                                        //oDialogBase.oContent[XIConstant.ContentDialog] = oDialogC;
                                    }

                                }
                                oXILinkI.oContent[XIConstant.ContentPopup] = oPopupC;
                            }
                        }
                        else if (iLayoutID > 0 || (LayoutGUID != null && LayoutGUID != Guid.Empty))
                        {
                            XIDLayout oLayoutD = new XIDLayout();
                            oLayoutD.ID = Convert.ToInt32(iLayoutID);
                            oLayoutD.XIGUID = LayoutGUID;
                            if (oLayoutD != null)
                            {
                                oLayoutD.oLayoutParams = oParams;
                                oLayoutD.sGUID = sGUID;
                                var oLayContent = oLayoutD.Load();
                                if (oLayContent.bOK && oLayContent.oResult != null)
                                {
                                    oXILinkI.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                                }
                            }
                        }
                        else if (oXILinkC.FKiComponentID > 0 || (oXILinkC.FKiComponentIDXIGUID != null && oXILinkC.FKiComponentIDXIGUID != Guid.Empty))
                        {
                            XIIComponent oCompI = new XIIComponent();
                            XIDComponent oXIComponentC = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, oXILinkC.FKiComponentIDXIGUID.ToString());
                            var oCompC = (XIDComponent)oXIComponentC.Clone(oXIComponentC);
                            if (oCompC != null)
                            {
                                if (oCompC != null)
                                {
                                    oCompC.GetComponentParams("xilink", oXILinkC.XIGUID.ToString());
                                }
                                oCompI.sGUID = sGUID;
                                oCompI.oDefintion = oCompC;
                                oCompI.sCallHierarchy = "XILink_" + oXILinkC.XiLinkID + ":Component" + oXILinkC.FKiComponentID;
                                if (oParams != null)
                                {
                                    //oParams = oParams.Where(x => x.sName != "{-iInstanceID}").ToList();
                                    oCache.MergeXILinkParameters(null, sGUID, oParams, HttpContext.Current.Session.SessionID);
                                }
                                var oCompContent = oCompI.Load();
                                if (oCompContent.bOK && oCompContent.oResult != null)
                                {
                                    oXILinkI.oContent[XIConstant.ContentXIComponent] = oCompContent.oResult;
                                }

                            }
                        }
                    }
                }
                oXILinkI.oDefintion = oXILinkC;
                if (!string.IsNullOrEmpty(sOutput))
                {
                    oXILinkI.sOutput = sOutput;
                }
                //oInstBase.oContent[XIConstant.ContentXILink] = oXILinkI;
                oCResult.oResult = oXILinkI;
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
            return oCResult; // always

            //return oURL;
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
                XILink oXILinkC = new XILink();
                if (iXILinkID > 0)
                {
                    oXILinkD = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, iXILinkID.ToString());
                    if (oXILinkD != null)
                    {
                        oXILinkC = (XILink)oXILinkD.Clone(oXILinkD);
                        XIRun(oXILinkC);
                        if (iDialogID > 0)
                        {
                            XIDDialog oDialogD;
                            oDialogD = (XIDDialog)oCache.GetObjectFromCache(XIConstant.CacheDialog, null, iDialogID.ToString());
                            if (oDialogD != null)
                            {
                                XIDefinitionBase oDef = new XIDefinitionBase();
                                XIDDialog oDialogC = (XIDDialog)oDialogD.Clone(oDialogD);
                                if (oDialogC.LayoutID > 0)
                                {
                                    XIDLayout oLayoutD = new XIDLayout();
                                    oLayoutD.ID = oDialogC.LayoutID;
                                    var oLayContent = oLayoutD.Preview();
                                    if (oLayContent.bOK && oLayContent.oResult != null)
                                    {
                                        oDialogC.oContent[XIConstant.ContentLayout] = oLayContent.oResult;
                                    }
                                }
                                oDef.oContent[XIConstant.ContentDialog] = oDialogC;
                                oXILinkC.oContent[XIConstant.ContentDialog] = oDef;
                            }
                        }
                        else if (oXILinkC.FKiComponentID > 0)
                        {
                            XIDefinitionBase oDef = new XIDefinitionBase();
                            XIDComponent oXIComponentC = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, oXILinkC.FKiComponentID.ToString());
                            var oCompC = (XIDComponent)oXIComponentC.Clone(oXIComponentC);
                            if (oCompC != null)
                            {
                                if (oCompC != null)
                                {
                                    oCompC.GetComponentParams("xilink", oXILinkC.XiLinkID.ToString());
                                }
                                oDef.oContent[XIConstant.ContentXIComponent] = oCompC;
                                oXILinkC.oContent[XIConstant.ContentXIComponent] = oDef;
                            }
                        }
                    }
                }
                oDefinition.oContent[XIConstant.ContentXILink] = oXILinkC;
                oCResult.oResult = oDefinition;
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
            return oCResult; // always

            //return oURL;
        }


        public void XIRun(XILink oXilink, string sGUID = "")
        {
            try
            {
                if (oXilink != null)
                {
                    var sSessionID = HttpContext.Current.Session.SessionID;
                    i1ClickID = oXilink.OneClickID;
                    foreach (var NVPair in oXilink.XiLinkNVs.Where(m => m.XiLinkListID == 0))
                    {
                        if (NVPair.Name.ToLower() == "StartAction".ToLower())
                        {
                            //get the value and assign to property of 1click instance

                            if (NVPair.Value.ToLower() == "Search".ToLower())
                            {
                                sSearchType = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "SearchType".ToLower()).Select(m => m.Value).FirstOrDefault();
                                //get the value and assign to  firststep property of 1click instance
                            }
                            else if (NVPair.Value.ToLower() == "List".ToLower())
                            {
                                //get the value and assign to  firststep property of 1click instance }
                            }
                            else if (NVPair.Value.ToLower() == "InlineView".ToLower() || NVPair.Value.ToLower() == "InlineEdit".ToLower() || NVPair.Value.ToLower() == "CreateForm".ToLower() || NVPair.Value.ToLower() == "PopupLeftContent".ToLower())
                            {
                                sContentType = NVPair.Value;
                                sBOName = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BO".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sGroup = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Group".ToLower() || m.Name.ToLower() == "Save Group".ToLower() || m.Name.ToLower() == "Show Group".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sVisualisation = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "visualisation".ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else if (NVPair.Value.ToLower() == "Popup".ToLower() || NVPair.Value.ToLower() == "InlinePopup".ToLower() || NVPair.Value.ToLower() == "Inline".ToLower())
                            {
                                iPopupID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "PopupID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                            else if (NVPair.Value.ToLower() == "Bespoke".ToLower())
                            {
                                sBespokeURL = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Bespoke url".ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else if (NVPair.Value.ToLower() == "Menu".ToLower())
                            {
                                sMenu = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "MenuName".ToLower()).Select(m => m.Value).FirstOrDefault();
                                Guid.TryParse(sMenu, out MenuGUID);
                            }
                            else if (NVPair.Value.ToLower() == "Dialog".ToLower())
                            {
                                var Dialog = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "DialogID".ToLower()).Select(m => m.Value).FirstOrDefault();
                                long.TryParse(Dialog, out iDialogID);
                                Guid.TryParse(Dialog, out DialogGUID);
                            }
                            else if (NVPair.Value.ToLower() == "QuestionSet".ToLower())
                            {
                                iQSDID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QuestionSetID".ToLower()).Select(m => m.Value).FirstOrDefault());
                                sQSType = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "sQSType".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sMode = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Mode".ToLower()).Select(m => m.Value).FirstOrDefault();
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|BODID}".ToLower())
                                    {
                                        var sBODID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}");
                                        if (sBODID != null)
                                        {
                                            long.TryParse(sBODID.ToString(), out iBODID);
                                        }
                                    }
                                }
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{xip|boiid}".ToLower())
                                    {
                                        var sBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + sBOName + ".id}");
                                        if (sBOIID != null)
                                        {
                                            long.TryParse(sBOIID.ToString(), out iBOIID);
                                        }
                                    }
                                }
                            }
                            else if (NVPair.Value.ToLower() == "QSStep".ToLower())
                            {
                                iQSDID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QSDID".ToLower()).Select(m => m.Value).FirstOrDefault());
                                sStepName = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QSStepName".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sMode = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Mode".ToLower()).Select(m => m.Value).FirstOrDefault();
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|BODID}".ToLower())
                                    {
                                        var sBODID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}");
                                        if (sBODID != null)
                                        {
                                            long.TryParse(sBODID.ToString(), out iBODID);
                                        }
                                    }
                                }
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|boiid}".ToLower())
                                    {
                                        var ParamBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + sBOName + ".id}");
                                        if (ParamBOIID != null)
                                        {
                                            long.TryParse(ParamBOIID.ToString(), out iBOIID);
                                        }
                                    }
                                }
                            }
                            else if (NVPair.Value.ToLower() == "Layout".ToLower())
                            {
                                var Layout = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "LayoutID".ToLower()).Select(m => m.Value).FirstOrDefault();
                                long.TryParse(Layout, out iLayoutID);
                                Guid.TryParse(Layout, out LayoutGUID);
                            }
                            sStartAction = NVPair.Value;
                        }
                        else if (NVPair.Name.ToLower() == "ListClick".ToLower())
                        {
                            if (NVPair.Value.ToLower() == "Popup".ToLower())
                            {
                                iPopupID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "PopupID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                        }
                        sOutput = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Output".ToLower()).Select(m => m.Value).FirstOrDefault();
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        public CResult Merge_XILinkURL(string sSessionID, string sGUID, XILink XILink)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "save required QS params to load before reloading the question set";//expalin about this method logic
            try
            {
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(XILink.URL);
                foreach (var match in matches)
                {
                    if (match.ToString().Contains('|'))
                    {
                        var SplitPipe = match.ToString().Split('|').ToList();
                        if (SplitPipe != null && SplitPipe.Count() > 0)
                        {
                            if (SplitPipe[0].ToLower() == "xip")
                            {
                                var Prm = "{" + match.ToString() + "}";
                                var sValue = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                                XILink.URL = XILink.URL.Replace(Prm, sValue);
                            }
                        }
                    }
                }
                oCResult.oResult = XILink;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}
