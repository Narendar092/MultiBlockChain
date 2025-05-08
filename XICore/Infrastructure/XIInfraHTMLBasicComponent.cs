using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraHTMLBasicComponent
    {

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                //XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
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
                var iGroupID = "0";
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    iGroupID = WrapperParms.Where(m => m.sName == "{XIP|iNannoGroupID}").Select(m => m.sValue).FirstOrDefault();
                }

                oParams.Add(new CNV { sName = "{XIP|iNannoGroupID}", sValue = iGroupID });
                var iCount = oParams.Where(m => m.sName.ToLower() == "iCount".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oParams.Add(new CNV { sName = "{XIP|iCount}", sValue = iCount });
                List<XIIBO> nBOI = new List<XIIBO>();
                var sMode = oParams.Where(m => m.sName.ToLower() == "sMode".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCoreDB = oParams.Where(m => m.sName.ToLower() == "sCoreDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var iPage = oCache.Get_ParamVal(sSessionID, sGUID, null, "iPage");
                oParams.Add(new CNV { sName = "iPage", sValue = iPage });
                var sLoadType = oCache.Get_ParamVal(sSessionID, sGUID, null, "sLoadType");
                oParams.Add(new CNV { sName = "sLoadType", sValue = sLoadType });
                if (!string.IsNullOrEmpty(sMode) && sMode.ToLower() == "nanno")
                {

                    XIIXI oXI = new XIIXI();
                    XIDXI oXID = new XIDXI();
                    XIIComponent oCompI = new XIIComponent();
                    int iOrgID = 0;
                    var sOrgID = oParams.Where(m => m.sName.ToLower() == "iOrgID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(sOrgID, out iOrgID);
                    if (iOrgID > 0)
                    {
                        oXID.sOrgDatabase = sCoreDB;
                        oCR = oXID.Get_OrgDefinition(null, iOrgID.ToString());
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            var oOrgD = (XIDOrganisation)oCR.oResult;
                            var sOrgDB = oOrgD.DatabaseName;
                            var s1Click = "XNannoUserMessageTo";
                            oCR = Get_Feed(s1Click, sOrgDB, oParams);
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oCompI.oContent["Left"] = oCR.oResult;
                            }
                            //s1Click = "XNannoUserMessageFrom";
                            //oCR = Get_Feed(s1Click, sOrgDB, oParams);
                            //oTrace.oTrace.Add(oCR.oTrace);
                            //if (oCR.bOK && oCR.oResult != null)
                            //{
                            //    oCompI.oContent["Right"] = oCR.oResult;
                            //}
                        }
                        oCResult.oResult = oCompI;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else
                {
                    var sBO = oParams.Where(m => m.sName.ToLower() == "sBO".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    int iInstanceID = 0;
                    var sInstanceID = oParams.Where(m => m.sName.ToLower() == "iInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(sInstanceID, out iInstanceID);
                    int i1ClickID = 0;
                    var s1ClickID = oParams.Where(m => m.sName.ToLower() == "i1clickid").Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(s1ClickID, out i1ClickID);
                    var sHTML = oParams.Where(m => m.sName.ToLower() == "sHTML".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    sHTML = "<h1>Widget</h1>";
                    var sOutput = string.Empty;
                    var CSS = new List<string>();
                    var Layouts = new List<string>();
                    if (!string.IsNullOrEmpty(sBO) && iInstanceID > 0)
                    {
                        XIIBO BOI = new XIIBO();
                        XIIXI oXI = new XIIXI();
                        BOI = oXI.BOI(sBO, iInstanceID.ToString());
                        oCR = MergeHTML(BOI, sHTML);

                        if (oCR.bOK && oCR.oResult != null)
                        {
                            sOutput = sOutput + (string)oCR.oResult;
                        }
                    }
                    else if (i1ClickID > 0 && !string.IsNullOrEmpty(sHTML))
                    {

                        XIIXI oXI = new XIIXI();
                        oCache = new XIInfraCache();
                        var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                        var Res = o1ClickD.OneClick_Run();
                        foreach (var BOI in Res.Values.ToList())
                        {
                            var Widget = BOI.AttributeI("fkiwidgetid").sValue;
                            var oWidI = oXI.BOI("XIWidget", Widget);
                            if (oWidI != null && oWidI.Attributes.Count() > 0)
                            {
                                CSS.Add(oWidI.AttributeI("scss").sValue);
                                var Layout = oWidI.AttributeI("fkilayoutid").sValue;
                                Layouts.Add(Layout);

                                //var oLayI = oXI.BOI("XI Layout", Layout);
                                //if (oLayI != null && oLayI.Attributes.Count() > 0)
                                //{
                                //    sHTML = oLayI.AttributeI("layoutcode").sValue;
                                //}
                            }
                            //oCR = MergeHTML(BOI, sHTML);
                            //if (oCR.bOK && oCR.oResult != null)
                            //{
                            //    sOutput = sOutput + (string)oCR.oResult;
                            //}
                        }
                    }
                    XIIComponent oCompI = new XIIComponent();
                    //oCompI.oContent[XIConstant.FeedComponent] = sOutput;
                    oCompI.oContent["CSS"] = CSS;
                    oCompI.oContent["Layout"] = Layouts;
                    oCResult.oResult = oCompI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
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

        private CResult MergeHTML(XIIBO BOI, string sHTML)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                var regex = @"\[\[.*?\]\]";
                var Rex = new Regex(regex);
                var match = Rex.Matches(sHTML);

                foreach (var mat in match)
                {
                    var Attr = mat.ToString().Replace("[[", "").Replace("]]", "");
                    var Splits = Attr.Split('.').ToList();
                    var sValue = string.Empty;
                    if (Splits[0] == "me")
                    {
                        sValue = BOI.AttributeI(Splits[1]).sValue;
                    }
                    sHTML = sHTML.Replace(mat.ToString(), sValue);
                }

                oCResult.oResult = sHTML;
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

        public CResult Get_Feed(string s1Click, string sOrgDB, List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "s1Click", sValue = s1Click });
                oTrace.oParams.Add(new CNV { sName = "sOrgDB", sValue = sOrgDB });
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                List<XIIBO> nBOI = new List<XIIBO>();
                var iUserOrg = oParams.Where(m => m.sName == "{XIP|iUserOrgID}").Select(m => m.sValue).FirstOrDefault();
                var iRoleID = oParams.Where(m => m.sName == "iRoleID").Select(m => m.sValue).FirstOrDefault();
                var iGroupID = oParams.Where(m => m.sName == "{XIP|iNannoGroupID}").Select(m => m.sValue).FirstOrDefault();
                int iCount = 0;
                var sCount = oParams.Where(m => m.sName.ToLower() == "iCount".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sCount, out iCount);
                var sPage = oParams.Where(m => m.sName.ToLower() == "iPage".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iPage = 0;
                if (!string.IsNullOrEmpty(sPage))
                {
                    int.TryParse(sPage, out iPage);
                }
                var iSkip = iPage * iCount;
                var sLoadType = oParams.Where(m => m.sName.ToLower() == "sLoadType".ToLower()).Select(m => m.sValue).FirstOrDefault();
                List<CNV> SPParams = new List<CNV>();
                SPParams.Add(new CNV { sName = "iPage", sValue = iPage.ToString() });
                SPParams.Add(new CNV { sName = "iCount", sValue = iCount.ToString() });
                SPParams.Add(new CNV { sName = "iOrgID", sValue = iUserOrg });
                SPParams.Add(new CNV { sName = "iGroupID", sValue = iGroupID });
                List<Dictionary<string, object>> Mes = new List<Dictionary<string, object>>();
                if (!string.IsNullOrEmpty(s1Click))//check mandatory params are passed or not
                {
                    var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, s1Click);
                    var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    //o1ClickC.ReplaceFKExpressions(oParams);
                    o1ClickC.NVs = SPParams;
                    o1ClickC.IsStoredProcedure = true;
                    o1ClickC.Query = "SP_NannoFeed"; //o1ClickC.Query + " OFFSET " + iSkip + " ROWS FETCH NEXT " + iCount + " ROWS ONLY";
                    var NannoTran = o1ClickC.OneClick_Run();
                    if (NannoTran != null && NannoTran.Count() > 0)
                    {
                        var NannoData = new List<XIIBO>();
                        if (!string.IsNullOrEmpty(sLoadType) && sLoadType == "Scroll")
                        {
                            NannoData = NannoTran.Values.ToList();
                        }
                        else
                        {
                            NannoData = NannoTran.Values.Reverse().ToList();
                        }
                        foreach (var BOI in NannoData)
                        {
                            int iNannoMessageID = 0;
                            var sNannoMessageID = BOI.AttributeI("FKiNannoMessageID").sValue;
                            int.TryParse(sNannoMessageID, out iNannoMessageID);
                            var iOrgTO = BOI.AttributeI("fkiorgidto").sValue;
                            var iOrgFrom = BOI.AttributeI("fkiorgidfrom").sValue;
                            var iNannoGroupID = BOI.AttributeI("FKiNannoGroupID").sValue;
                            if (iNannoMessageID > 0)
                            {
                                var AppInstID = string.Empty;
                                var oGroupI = oXI.BOI("Nanno Group", iNannoGroupID);
                                if (oGroupI != null && oGroupI.Attributes.Count() > 0)
                                {
                                    AppInstID = oGroupI.AttributeI("FKiNannoAppInstID").sValue;
                                }
                                var MessageI = oXI.BOI("Nanno Message", iNannoMessageID.ToString());
                                if (MessageI != null && MessageI.Attributes.Count() > 0)
                                {
                                    Dictionary<string, object> Data = new Dictionary<string, object>();
                                    if (iOrgFrom != iUserOrg)
                                    {
                                        Data["LeftMessage"] = MessageI;
                                    }
                                    else if (iOrgFrom == iUserOrg)
                                    {
                                        Data["RightMessage"] = MessageI;
                                    }
                                    Data["iNannoOrgID"] = iOrgFrom;
                                    var iOrgObjectTypeID = MessageI.AttributeI("FKiOrgObjectTypeID").sValue;
                                    var WhrPrms = new List<CNV>();
                                    WhrPrms.Add(new CNV { sName = "FKiOrgID", sValue = iUserOrg });
                                    WhrPrms.Add(new CNV { sName = "FKiNannoGroupID", sValue = iNannoGroupID });
                                    var shareRole = oXI.BOI("XOrgGroup", null, null, WhrPrms);
                                    if (shareRole != null && shareRole.Attributes.Count() > 0)
                                    {
                                        var ShareRoleID = shareRole.AttributeI("FKiShareRoleID").sValue;
                                        var Perm1Click = new XID1Click();
                                        Perm1Click.BOID = 1391;
                                        if (ShareRoleID == "0" || string.IsNullOrEmpty(ShareRoleID))
                                        {
                                            Perm1Click.Query = "select * from XINannoPermission_T where FKiRoleID =" + iRoleID + " and FKiOrgObjectTypeID=" + iOrgObjectTypeID + " and FKiNannoAppInstID=" + AppInstID;
                                        }
                                        else
                                        {
                                            Perm1Click.Query = "select * from XINannoPermission_T where FKiShareRoleID =" + ShareRoleID + " and FKiOrgObjectTypeID=" + iOrgObjectTypeID + " and FKiNannoAppInstID=" + AppInstID;
                                        }
                                        var Res = Perm1Click.OneClick_Run();
                                        if (Res != null && Res.Count() > 0)
                                        {
                                            Dictionary<string, string> Perm = new Dictionary<string, string>();
                                            foreach(var perm in Res.Values.ToList())
                                            {
                                                Perm[perm.AttributeI("iType").sValue] = perm.AttributeI("iPermission").sValue;
                                            }
                                            Data["Permission"] = Perm;
                                        }
                                    }

                                    var ObjectI = oXI.BOI("OrgObjectType", iOrgObjectTypeID.ToString());
                                    if (ObjectI != null && ObjectI.Attributes.Count() > 0)
                                    {
                                        var iWidgetID = ObjectI.AttributeI("FKiWidgetID").sValue;
                                        var oWidgetD = (XIDWidget)oCache.GetObjectFromCache(XIConstant.CacheWidget, null, iWidgetID);
                                        Data["Widget"] = oWidgetD;
                                    }
                                    Mes.Add(Data);
                                    //var iBODID = MessageI.AttributeI("iBODID").sValue;
                                    //var iBOIID = MessageI.AttributeI("iBOIID").sValue;
                                    //var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                                    //var o1Click = "select * from " + BOD.TableName + " where " + BOD.sPrimaryKey + "=" + iBOIID;
                                    //var o1ClickI = new XID1Click();
                                    ////o1ClickI.sSwitchDB = sOrgDB;
                                    //o1ClickI.Query = o1Click;
                                    //o1ClickI.BOID = Convert.ToInt32(iBODID);
                                    //var oIns = o1ClickI.OneClick_Run();
                                    //if (oIns != null && oIns.Count() > 0)
                                    //{
                                    //    nBOI.Add(oIns.Values.FirstOrDefault());
                                    //}
                                }
                            }
                        }
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = Mes;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: s1Click is missing";
                }
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