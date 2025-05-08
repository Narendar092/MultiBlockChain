using iTextSharp.tool.xml.html;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using XIDatabase;
using XISystem;
using static iTextSharp.text.pdf.AcroFields;

namespace XICore
{
    public class XIInfraCache
    {
        public string sSessionID { get; set; }
        public string sParGUID = string.Empty;
        public string sKey { get; set; }
        public string oCachedObject { get; set; }
        public string sSize { get; set; }
        XIInstanceBase oIns = new XIInstanceBase();
        public void SetXIParams(List<CNV> oParams, string sGUID, string sSessionID)
        {
            XIInfraCache oCache = new XIInfraCache();
            // string sSessionID = HttpContext.Current.Session.SessionID;
            foreach (var items in oParams)
            {
                if (!string.IsNullOrEmpty(items.sValue))
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, items.sContext, items.sName, items.sValue, items.sType, items.nSubParams);
                }
            }
        }

        public string Get_ParamVal(string sSessionID, string sUID, string sContext, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            string sRuntimeVal = string.Empty;
            string sParamContext = string.Empty;
            if (sUID != null)
            {
                if (sParamName.StartsWith("-") && sParamName.Contains("."))
                {
                    var Splithiphen = sParamName.Split('.').ToList();
                    sParamContext = Splithiphen[0];
                    sParamName = Splithiphen[1];
                    if (!string.IsNullOrEmpty(sParamContext))
                    {
                        var subParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams;
                        if (subParams != null)
                        {
                            sRuntimeVal = subParams.Where(m => m.sName.ToLower() == sParamName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sContext))
                    {
                        sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sValue;
                    }
                    else
                    {
                        sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue;
                    }
                }
            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return sRuntimeVal;
        }

        public XICacheInstance Get_Paramobject(string sSessionID, string sUID, string sContext, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance sRuntimeVal = new XICacheInstance();
            if (sUID != null)
            {
                if (!string.IsNullOrEmpty(sContext))
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName);
                }
                else
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName);
                }

            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return sRuntimeVal;
        }

        public string Set_ParamVal(string sSessionID, string sUID, string sContext, string sParamName, string sParamValue, string sType, List<CNV> nSubParams)
        {
            XICacheInstance oCache = Get_XICache();
            if (sUID != null && sUID.Length > 0)
            {
                if (!string.IsNullOrEmpty(sContext))
                {
                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sValue = sParamValue;
                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sType = sType;
                }
                else
                {
                    if (sType != null && sType.ToLower() == "register".ToLower())
                    {
                        if (oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers == null)
                        {
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers = new List<CNV>();
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new CNV { sName = sParamName, sValue = sParamValue });
                        }
                        else
                        {
                            var IsExists = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Where(m => m.sValue == sParamValue).FirstOrDefault();
                            if (IsExists == null)
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new CNV { sName = sParamName, sValue = sParamValue });
                            }
                        }
                    }
                    else
                    {
                        if (oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue != null)
                        {
                            if (!string.IsNullOrEmpty(sParamValue) && !sParamValue.StartsWith("{XIP|"))
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sType = sType;
                                if (nSubParams != null && nSubParams.Count() > 0)
                                {
                                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                                }
                            }
                            else
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                                if (nSubParams != null && nSubParams.Count() > 0)
                                {
                                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                                }
                            }
                        }
                        else
                        {
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sType = sType;
                            if (nSubParams != null && nSubParams.Count() > 0)
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                            }
                        }

                    }
                }
            }
            return "TRUE";
        }
        public string Set_ScriptVal(string sSessionID, string sParamName, MethodInfo sParamValue)
        {
            XICacheInstance oCache = Get_XICache();
            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance(sParamName).Script = sParamValue;
            return "TRUE";
        }
        public MethodInfo Get_ScriptVal(string sSessionID, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            MethodInfo oMethodInfo = null;
            oMethodInfo = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance(sParamName).Script;
            return oMethodInfo;
        }

        public string Set_QsStructureObj(string sSessionID, string sUID, string sParamName, XIBOInstance sParamValue)
        {
            XICacheInstance oCache = Get_XICache();
            if (!string.IsNullOrEmpty(sUID))
            {
                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).oBOInstance = sParamValue;
            }
            return "TRUE";
        }
        public XIBOInstance Get_QsStructureObj(string sSessionID, string sUID, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            XIBOInstance oBOInstance = new XIBOInstance();
            if (!string.IsNullOrEmpty(sUID))
            {
                oBOInstance = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).oBOInstance;
            }
            return oBOInstance;
        }

        public XICacheInstance GetAllParamsUnderGUID(string sSessionID, string sUID, string sContext)
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance oGUIDParams = new XICacheInstance();
            if (!string.IsNullOrEmpty(sContext))
            {
                oGUIDParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext);
            }
            else
            {
                oGUIDParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID);
            }

            return oGUIDParams;
        }

        public XICacheInstance Get_XICache()
        {
            object obj;
            XICacheInstance oCacheobj = new XICacheInstance();
            if (HttpRuntime.Cache["XICache"] == null)
            {
                //XICacheInstance oCache = new XICacheInstance();
                HttpRuntime.Cache.Add("XICache", oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                //oCache = HttpRuntime.Cache["XICache"];
            }
            else
            {
                obj = HttpRuntime.Cache["XICache"];
                oCacheobj = (XICacheInstance)obj;
                return oCacheobj;
            }
            //Get_XICache();
            return oCacheobj;
        }

        public XICacheInstance Init_RuntimeParamSet(string sSessionID, string sNewUID, string sParentUID = "", string sContext = "")
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance oNewRTInst;
            if (!string.IsNullOrEmpty(sContext))
            {
                oNewRTInst = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sNewUID).NInstance("Con_" + sContext);
            }
            else
            {
                oNewRTInst = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sNewUID);
            }

            if (sParentUID != "") { oNewRTInst.NInstance("|XIParent").sValue = sParentUID; }
            return oNewRTInst;
        }

        public string Set_ActiveInstance(string sSessionID, string LayoutID, string sParamName, string sParamValue)
        {
            XICacheInstance oCache = Get_XICache();
            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("LID_" + LayoutID).NInstance(sParamName).sValue = sParamValue;
            return "TRUE";
        }

        public void ClearCache(string AppName)
        {
            CResult oCResult = new CResult();
            try
            {
                IDictionaryEnumerator cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();
                while (cacheEnumerator.MoveNext())
                {
                    if (!string.IsNullOrEmpty(AppName))
                    {
                        if (cacheEnumerator.Key.ToString().StartsWith(AppName + "_Definition_") || cacheEnumerator.Key.ToString().ToLower().StartsWith("SingleC#Compiler_".ToLower()) || cacheEnumerator.Key.ToString().ToLower().StartsWith("MultiC#Compiler_".ToLower()))
                            HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());
                    }
                    else
                    {
                        //HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());
                    }

                }
                int iUserID = 0;
                int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out iUserID);
                string sUser = string.Empty;
                if (iUserID > 0)
                {
                    sUser = HttpContext.Current.Session["sUserName"].ToString();
                }
                if (string.IsNullOrEmpty(sUser))
                {
                    sUser = "Public without Login";
                }
                oCResult.sMessage = "Cache Cleared on: " + DateTime.Now + " - By User: " + sUser;
                oIns.SaveErrortoDB(oCResult);
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIns.SaveErrortoDB(oCResult);
            }
        }

        public void Clear_PerformanceCache(string sKey)
        {
            CResult oCResult = new CResult();
            try
            {
                if (!string.IsNullOrEmpty(sKey))
                {
                    IDictionaryEnumerator cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();
                    while (cacheEnumerator.MoveNext())
                    {
                        if (cacheEnumerator.Key.ToString().StartsWith(sKey))
                            HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIns.SaveErrortoDB(oCResult);
            }
        }

        #region CacheObject

        //Get Object from Cache if not exists insert and get it
        public object GetObjectFromCache(string ObjType = "", string ObjName = "", string ObjID = "", string sSessionID = null, string sGUID = null, int iApplicationID = 0, int iOrgID = 0)
        {
            //var sSessionID = HttpContext.Current.Session.SessionID;
            if (iOrgID == 0 && HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                int iUserID = 0;
                var UserID = HttpContext.Current.Session["UserID"];
                if (UserID != null)
                {
                    var UserUID = HttpContext.Current.Session["UserID"].ToString();
                    int.TryParse(UserUID, out iUserID);
                }
                var sCoreDB = HttpContext.Current.Session["CoreDatabase"];
                string CoreDBName = string.Empty;
                if (sCoreDB != null)
                {
                    CoreDBName = HttpContext.Current.Session["CoreDatabase"].ToString();
                }
                if (iUserID > 0)
                {
                    XIInfraUsers oUser = new XIInfraUsers();
                    var oCR = oUser.Get_UserInfo(CoreDBName);
                    iOrgID = oCR.FKiOrgID;
                }
            }
            var CacheKey = CacheKeyBuilder(ObjType, ObjName, ObjID, "", iOrgID, false, iApplicationID); var CacheStatus = "ON";
            if (ObjType == "questionset")
            {
                CResult oCResult = new CResult();
                XIDefinitionBase oDef = new XIDefinitionBase();
                oCResult.sMessage = "finding,  questionset  key:" + CacheKey + " on " + System.DateTime.Now;
                oCResult.sCategory = "questionset key";
                oDef.SaveErrortoDB(oCResult);
            }
            try
            {
                CacheStatus = ConfigurationManager.AppSettings["Cache"];
            }
            catch (Exception ex)
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIns.SaveErrortoDB(oCResult);
            }
            object oCacheObj;
            if (CacheStatus != "OFF")
            {
                if (HttpRuntime.Cache[CacheKey] == null)
                {
                    var CacheObj = AddObjectToCache(ObjType, ObjName, ObjID, sSessionID, sGUID, iApplicationID, iOrgID);
                    HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    if (ObjType == "bo" || ObjType == "bo_all")
                    {
                        var BOD = (XIDBO)CacheObj;
                        if (BOD != null)
                        {
                            if (!string.IsNullOrEmpty(ObjName))
                            {
                                CacheKey = CacheKeyBuilder(ObjType, null, BOD.BOID.ToString(), "", iOrgID);
                                HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                            }
                            else if (!string.IsNullOrEmpty(ObjID) && ObjID != "0")
                            {
                                CacheKey = CacheKeyBuilder(ObjType, BOD.Name, null, "", iOrgID);
                                HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                            }
                            if (BOD.XIGUID != null && !string.IsNullOrEmpty(BOD.XIGUID.ToString()))
                            {
                                CacheKey = CacheKeyBuilder(ObjType, BOD.XIGUID.ToString(), null, "", iOrgID);
                                HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                            }
                            var OrgFks = BOD.Attributes.Values.Where(attr => attr.bOrgFK && (attr.FKiType == 10 || attr.FKiType == 40)).ToList();
                            if (OrgFks != null && OrgFks.Count() > 0)
                            {
                                var oCR = Attach_OrgData(BOD, iOrgID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var oBODCopy = (XIDBO)oCR.oResult;
                                    return oBODCopy;
                                }
                                else
                                {
                                    oCacheObj = HttpRuntime.Cache[CacheKey];
                                }
                            }
                            else
                            {
                            oCacheObj = HttpRuntime.Cache[CacheKey];
                            }
                        }
                        else
                        {
                            oCacheObj = HttpRuntime.Cache[CacheKey];
                        }
                    }
                    else if (ObjType == "questionset")
                    {
                        var oQSD = (XIDQS)CacheObj;
                        if (!string.IsNullOrEmpty(ObjName))
                        {
                            CacheKey = CacheKeyBuilder(ObjType, null, oQSD.ID.ToString(), "", iOrgID);
                            HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                        }
                        else if (!string.IsNullOrEmpty(ObjID) && ObjID != "0")
                        {
                            CacheKey = CacheKeyBuilder(ObjType, oQSD.sName, "", "", iOrgID);
                            HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                        }
                        if (oQSD.XIGUID != null && !string.IsNullOrEmpty(oQSD.XIGUID.ToString()))
                        {
                            CacheKey = CacheKeyBuilder(ObjType, oQSD.XIGUID.ToString(), null, "", iOrgID);
                            HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                        }
                        if (oQSD.OrgFieldOrigins != null && oQSD.OrgFieldOrigins.Count() > 0)
                        {
                            var oCR = Attach_QSOrgData(oQSD, iOrgID, sGUID);
                            oCacheObj = (XIDQS)oCR.oResult;
                        }
                        else
                        {
                            oCacheObj = oQSD;
                        }
                    }
                    else
                    {
                        oCacheObj = HttpRuntime.Cache[CacheKey];
                    }
                }
                else
                {
                    oCacheObj = HttpRuntime.Cache[CacheKey];
                    if (ObjType == "bo" || ObjType == "bo_all")
                    {
                        var BOD = (XIDBO)oCacheObj;
                        var OrgFks = BOD.Attributes.Values.Where(attr => attr.bOrgFK && (attr.FKiType == 10 || attr.FKiType == 40)).ToList();
                        if (OrgFks != null && OrgFks.Count() > 0)
                        {
                            var oCR = Attach_OrgData(BOD, iOrgID);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oBODCopy = (XIDBO)oCR.oResult;
                                return oBODCopy;
                            }
                        }
                    }
                    else if (ObjType == "questionset")
                    {
                        var oQSD = (XIDQS)oCacheObj;
                        if (oQSD.OrgFieldOrigins != null && oQSD.OrgFieldOrigins.Count() > 0)
                        {
                            var oCR = Attach_QSOrgData(oQSD, iOrgID, sGUID);
                            oCacheObj = (XIDQS)oCR.oResult;
                        }
                        else
                        {
                            oCacheObj = oQSD;
                        }
                    }
                }
            }
            else
            {
                oCacheObj = AddObjectToCache(ObjType, ObjName, ObjID);
            }
            return oCacheObj;
        }

        private object AddObjectToCache(string ObjType, string ObjName, string ObjID = "", string sSessionID = null, string sGUID = null, int iApplicationID = 0, int iOrgID = 0)
        {
            XIDXI oDXI = new XIDXI();
            switch (ObjType.ToLower())
            {
                case "bo":
                    XIDBO xiBOD = new XIDBO();
                    var oBODef = oDXI.Get_BODefinition(ObjName, ObjID);
                    if (oBODef.bOK && oBODef.oResult != null)
                    {
                        xiBOD = (XIDBO)oBODef.oResult;
                    }
                    return xiBOD;
                case "bo_all":
                    XIDBO xiBODAll = new XIDBO();
                    var xiBODef = oDXI.Get_BODefinitionALL(ObjName, ObjID);
                    if (xiBODef.bOK && xiBODef.oResult != null)
                    {
                        xiBODAll = (XIDBO)xiBODef.oResult;
                    }
                    return xiBODAll;
                case "xilink":
                    XILink xiLink = new XILink();
                    var oXiLinkDef = oDXI.Get_XILinkDefinition(ObjID, ObjName);
                    if (oXiLinkDef.bOK && oXiLinkDef.oResult != null)
                    {
                        xiLink = (XILink)oXiLinkDef.oResult;
                    }
                    return xiLink;
                case "url":
                    XIURLMappings xiURL = new XIURLMappings();
                    var oXiURLDef = oDXI.Get_URLDefinition(ObjName);
                    if (oXiURLDef.bOK && oXiURLDef.oResult != null)
                    {
                        xiURL = (XIURLMappings)oXiURLDef.oResult;
                    }
                    return xiURL;
                case "application":
                    XIDApplication xiApp = new XIDApplication();
                    var oXiAppDef = oDXI.Get_ApplicationDefinition(ObjName, ObjID);
                    if (oXiAppDef.bOK && oXiAppDef.oResult != null)
                    {
                        xiApp = (XIDApplication)oXiAppDef.oResult;
                    }
                    return xiApp;
                case "applicationotpsetting":
                    XIDApplicationOTPSetting xiAppOTP = new XIDApplicationOTPSetting();
                    var oXiAppOTPDef = oDXI.Get_ApplicationOTPSettingDefinition(ObjName, ObjID);
                    if (oXiAppOTPDef.bOK && oXiAppOTPDef.oResult != null)
                    {
                        xiAppOTP = (XIDApplicationOTPSetting)oXiAppOTPDef.oResult;
                    }
                    return xiAppOTP;
                case "organisation":
                    XIDOrganisation xiOrg = new XIDOrganisation();
                    var oXiOrgDef = oDXI.Get_OrgDefinition(ObjName, ObjID);
                    if (oXiOrgDef.bOK && oXiOrgDef.oResult != null)
                    {
                        xiOrg = (XIDOrganisation)oXiOrgDef.oResult;
                    }
                    return xiOrg;
                case "layout":
                    XIDLayout xiLayout = new XIDLayout();
                    var oXiLayoutDef = oDXI.Get_LayoutDefinition(ObjName, ObjID);
                    if (oXiLayoutDef.bOK && oXiLayoutDef.oResult != null)
                    {
                        xiLayout = (XIDLayout)oXiLayoutDef.oResult;
                    }
                    return xiLayout;
                case "questionset":
                    XIDQS xiQS = new XIDQS();
                    var oQSDef = oDXI.Get_QSDefinition(ObjName, ObjID, sSessionID, sGUID, iOrgID);
                    if (oQSDef.bOK && oQSDef.oResult != null)
                    {
                        xiQS = (XIDQS)oQSDef.oResult;
                    }
                    return xiQS;
                case "questionsetlite":
                    XIDQS xiQSD = new XIDQS();
                    var oQSD = oDXI.Get_QuestionSetDefinition(ObjName);
                    if (oQSD.bOK && oQSD.oResult != null)
                    {
                        xiQSD = (XIDQS)oQSD.oResult;
                    }
                    return xiQSD;
                case "qsstep":
                    XIDQSStep xiQSStep = new XIDQSStep();
                    var oQSStepDef = oDXI.Get_StepDefinition(ObjName, ObjID);
                    if (oQSStepDef.bOK && oQSStepDef.oResult != null)
                    {
                        xiQSStep = (XIDQSStep)oQSStepDef.oResult;
                    }
                    return xiQSStep;
                case "qssection":
                    XIDQSSection xiQSSection = new XIDQSSection();
                    var oQSSecDef = oDXI.Get_QSSectionDefinition(ObjID);
                    if (oQSSecDef.bOK && oQSSecDef.oResult != null)
                    {
                        xiQSSection = (XIDQSSection)oQSSecDef.oResult;
                    }
                    return xiQSSection;
                case "oneclick":
                    XID1Click o1ClickD = new XID1Click();
                    var o1ClickDef = oDXI.Get_1ClickDefinition(ObjName, ObjID);
                    if (o1ClickDef.bOK && o1ClickDef.oResult != null)
                    {
                        o1ClickD = (XID1Click)o1ClickDef.oResult;
                    }
                    return o1ClickD;
                case "component":
                    XIDComponent oComponentD = new XIDComponent();
                    var oCompnDef = oDXI.Get_ComponentDefinition(ObjName, ObjID);
                    if (oCompnDef.bOK && oCompnDef.oResult != null)
                    {
                        oComponentD = (XIDComponent)oCompnDef.oResult;
                    }
                    return oComponentD;
                case "datasource":
                    XIDataSource oDataSourceD = new XIDataSource();
                    var oDataSourceDedf = oDXI.Get_DataSourceDefinition(ObjName, ObjID, iApplicationID);
                    if (oDataSourceDedf.bOK && oDataSourceDedf.oResult != null)
                    {
                        oDataSourceD = (XIDataSource)oDataSourceDedf.oResult;
                    }
                    return oDataSourceD;
                //case "studiodatasource":
                //    string oDataSourceDS = string.Empty;
                //    var oDataSourceDedSf = oDXI.Get_StudioDataSourceDefinition(ObjName, ObjID);
                //    if (oDataSourceDedSf.bOK && oDataSourceDedSf.oResult != null)
                //    {
                //        oDataSourceDS = (string)oDataSourceDedSf.oResult;
                //    }
                //    return oDataSourceDS;
                case "structure":
                    List<XIDStructure> oStructD = new List<XIDStructure>();
                    XIDStructure oXIStruct = new XIDStructure();
                    var oStructDef = oXIStruct.GetXITreeStructure(ObjID, ObjName);
                    if (oStructDef.bOK && oStructDef.oResult != null)
                    {
                        oStructD = (List<XIDStructure>)oStructDef.oResult;
                    }
                    return oStructD;
                //case "xistructure":
                //    XIStructure oStrD = new XIStructure();
                //    XIDStructure oXIStr = new XIDStructure();
                //    var oStrDef = oXIStr.GetXIStructureDefinition(Convert.ToInt32(ObjID), ObjName);
                //    if (oStrDef.bOK && oStrDef.oResult != null)
                //    {
                //        oStrD = (XIStructure)oStrDef.oResult;
                //    }
                //    return oStrD;
                case "xiparamater":
                    XIParameter oXIParamD = new XIParameter();
                    var oXIParamDef = oDXI.Get_XIParameterDefinition(ObjName, ObjID.ToString());
                    if (oXIParamDef.bOK && oXIParamDef.oResult != null)
                    {
                        oXIParamD = (XIParameter)oXIParamDef.oResult;
                    }
                    return oXIParamD;
                case "dialog":
                    XIDDialog xiDialog = new XIDDialog();
                    var oXiDialogDef = oDXI.Get_DialogDefinition(ObjID);
                    if (oXiDialogDef.bOK && oXiDialogDef.oResult != null)
                    {
                        xiDialog = (XIDDialog)oXiDialogDef.oResult;
                    }
                    return xiDialog;
                case "template":
                    List<XIContentEditors> oTemplateD = new List<XIContentEditors>();
                    var oXiTemplateDef = oDXI.Get_ContentDefinition(ObjID, ObjName);
                    if (oXiTemplateDef.bOK && oXiTemplateDef.oResult != null)
                    {
                        oTemplateD = (List<XIContentEditors>)oXiTemplateDef.oResult;
                    }
                    return oTemplateD;
                case "visualisation":
                    XIVisualisation oXIvisualisationD = new XIVisualisation();
                    var oXIvisualisationDef = oDXI.Get_VisualisationDefinition(ObjID, ObjName);
                    if (oXIvisualisationDef.bOK && oXIvisualisationDef.oResult != null)
                    {
                        oXIvisualisationD = (XIVisualisation)oXIvisualisationDef.oResult;
                    }
                    return oXIvisualisationD;
                case "structurecode":
                    XIDStructure oXIStructD = new XIDStructure();
                    var oXIStructDef = oDXI.Get_StructureDefinition(ObjID);
                    if (oXIStructDef.bOK && oXIStructDef.oResult != null)
                    {
                        oXIStructD = (XIDStructure)oXIStructDef.oResult;
                    }
                    return oXIStructD;
                case "refdata":
                    object oRefData = 0;
                    var oRefDataDef = oDXI.Get_AutoCompleteList(ObjID, ObjName);
                    if (oRefDataDef.bOK && oRefDataDef.oResult != null)
                    {
                        oRefData = oRefDataDef.oResult;
                    }
                    return oRefData;
                case "menu":
                    List<XIMenu> oMenuD = new List<XIMenu>();
                    var oXIMenuDef = oDXI.Get_RightMenuDefinition(ObjName);
                    if (oXIMenuDef.bOK && oXIMenuDef.oResult != null)
                    {
                        oMenuD = (List<XIMenu>)oXIMenuDef.oResult;
                    }
                    return oMenuD;
                case "menunode":
                    List<XIMenu> oMenuNodeD = new List<XIMenu>();
                    var oXIMenuNodeDef = oDXI.Get_MenuNodeDefinition(ObjID, ObjName);
                    if (oXIMenuNodeDef.bOK && oXIMenuNodeDef.oResult != null)
                    {
                        oMenuNodeD = (List<XIMenu>)oXIMenuNodeDef.oResult;
                    }
                    return oMenuNodeD;
                case "createmenu":
                    XIAssignMenu oXIM = new XIAssignMenu();
                    List<XIAssignMenu> oCreateMenuD = new List<XIAssignMenu>();
                    var oXICreateMenuDef = oXIM.Get_RightMenuDefinition(ObjName);
                    if (oXICreateMenuDef.bOK && oXICreateMenuDef.oResult != null)
                    {
                        oCreateMenuD = (List<XIAssignMenu>)oXICreateMenuDef.oResult;
                    }
                    return oCreateMenuD;
                //case "Step":
                //    var oStep = oQSRepo.GetStepDefinitionByID(ID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                //    return oStep;
                //case "Layout":
                //    var oLayout = oQSRepo.GetLayoutByID(ID);
                //    return oLayout;
                case "doctypes":
                    XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                    var oXIDocTypesDef = oXIDocTypes.Get_FileDocTypes(ObjName, ObjID);
                    if (oXIDocTypesDef.bOK && oXIDocTypesDef.oResult != null)
                    {
                        oXIDocTypes = (XIInfraDocTypes)oXIDocTypesDef.oResult;
                    }
                    return oXIDocTypes;
                case "inbox":
                    List<XIDInbox> oInboxD = new List<XIDInbox>();
                    var oInboxDef = oDXI.Get_InboxDefinition(ObjID);
                    if (oInboxDef.bOK && oInboxDef.oResult != null)
                    {
                        oInboxD = (List<XIDInbox>)oInboxDef.oResult;
                    }
                    return oInboxD;
                case "popup":
                    XIDPopup xiPopup = new XIDPopup();
                    var oXiPopupDef = oDXI.Get_PopupDefinition(ObjID);
                    if (oXiPopupDef.bOK && oXiPopupDef.oResult != null)
                    {
                        xiPopup = (XIDPopup)oXiPopupDef.oResult;
                    }
                    return xiPopup;
                case "ioserverinfo":
                    XIIOServerDetails xiIOSever = new XIIOServerDetails();
                    int iServerID = 0;
                    if (int.TryParse(ObjName, out iServerID))
                    { }
                    var xiIOSeverDef = oDXI.Get_IOSServerDetails(Convert.ToInt32(ObjID), iServerID);
                    if (xiIOSeverDef.bOK && xiIOSeverDef.oResult != null)
                    {
                        xiIOSever = (XIIOServerDetails)xiIOSeverDef.oResult;
                    }
                    return xiIOSever;
                case "source":
                    XIDSource xiSource = new XIDSource();
                    var oxiSourceDef = oDXI.Get_SourceDefinition(ObjID);
                    if (oxiSourceDef.bOK && oxiSourceDef.oResult != null)
                    {
                        xiSource = (XIDSource)oxiSourceDef.oResult;
                    }
                    return xiSource;
                case "class":
                    XIDClass xiClass = new XIDClass();
                    var oxiClassDef = oDXI.Get_ClassDefinition(ObjID);
                    if (oxiClassDef.bOK && oxiClassDef.oResult != null)
                    {
                        xiClass = (XIDClass)oxiClassDef.oResult;
                    }
                    return xiClass;
                case "Report":
                    XIReports XIDRepo = new XIReports();
                    var oXIDRepo = XIDRepo.GenerateReport(null);
                    if (oXIDRepo.bOK && oXIDRepo.oResult != null)
                    {
                        XIDRepo = (XIReports)oXIDRepo.oResult;
                    }
                    return XIDRepo;
                case "fieldorigin":
                    XIDFieldOrigin oOrigin = new XIDFieldOrigin();
                    var oxiFOrigin = oDXI.Get_FieldOriginDefinition(ObjName, ObjID);
                    if (oxiFOrigin.bOK && oxiFOrigin.oResult != null)
                    {
                        oOrigin = (XIDFieldOrigin)oxiFOrigin.oResult;
                    }
                    return oOrigin;
                case "script":
                    XIDScript oScript = new XIDScript();
                    var oxiScript = oDXI.Get_ScriptDefinition(ObjName, ObjID);
                    if (oxiScript.bOK && oxiScript.oResult != null)
                    {
                        oScript = (XIDScript)oxiScript.oResult;
                    }
                    return oScript;
                case "boaction":
                    XIDBOAction oBOAction = new XIDBOAction();
                    var oxiBOAction = oDXI.Get_BOActionDefinition(ObjName, ObjID);
                    if (oxiBOAction.bOK && oxiBOAction.oResult != null)
                    {
                        oBOAction = (XIDBOAction)oxiBOAction.oResult;
                    }
                    return oBOAction;
                case "bodefault":
                    XIDBODefault oBODefault = new XIDBODefault();
                    var oxiBODefault = oDXI.Get_BODefault(ObjName, ObjID);
                    if (oxiBODefault.bOK && oxiBODefault.oResult != null)
                    {
                        oBODefault = (XIDBODefault)oxiBODefault.oResult;
                    }
                    return oBODefault;
                case "datatype":
                    XIDDataType oDataType = new XIDDataType();
                    var oXIDtype = oDXI.Get_DataTypeDefinition(ObjID);
                    if (oXIDtype.bOK && oXIDtype.oResult != null)
                    {
                        oDataType = (XIDDataType)oXIDtype.oResult;
                    }
                    return oDataType;
                case "widget":
                    XIDWidget oWidgetD = new XIDWidget();
                    var oWidget = oDXI.Get_WidgetDefinition(ObjName, ObjID);
                    if (oWidget.bOK && oWidget.oResult != null)
                    {
                        oWidgetD = (XIDWidget)oWidget.oResult;
                    }
                    return oWidgetD;
                case "algorithm":
                    XIDAlgorithm oAlogD = new XIDAlgorithm();
                    var oxiAlgo = oDXI.Get_XIAlgorithmDefinition(ObjName, ObjID);
                    if (oxiAlgo.bOK && oxiAlgo.oResult != null)
                    {
                        oAlogD = (XIDAlgorithm)oxiAlgo.oResult;
                    }
                    return oAlogD;
                case "whitelist":
                    Dictionary<string, object> WhiteList = new Dictionary<string, object>();
                    var Data = oDXI.Get_WhiteList();
                    if (Data.bOK && Data.oResult != null)
                    {
                        WhiteList = (Dictionary<string, object>)Data.oResult;
                    }
                    return WhiteList;
                case "configsetting":
                    string sSetting = string.Empty;
                    var oConfig = oDXI.Get_ConfigSetting(ObjName);
                    if (oConfig.bOK && oConfig.oResult != null)
                    {
                        sSetting = (string)oConfig.oResult;
                    }
                    return sSetting;
                case "linkaccess":
                    Dictionary<string, object> LinkAccess = new Dictionary<string, object>();
                    var Links = oDXI.Get_1LinkAccess();
                    if (Links.bOK && Links.oResult != null)
                    {
                        LinkAccess = (Dictionary<string, object>)Links.oResult;
                    }
                    return LinkAccess;
                case "queryaccess":
                    Dictionary<string, object> QueryAccess = new Dictionary<string, object>();
                    var Queries = oDXI.Get_1QueryAccess();
                    if (Queries.bOK && Queries.oResult != null)
                    {
                        QueryAccess = (Dictionary<string, object>)Queries.oResult;
                    }
                    return QueryAccess;
                case "sendgridaccount":
                    XIDSendGridAccountDetails oSendGridInfo = new XIDSendGridAccountDetails();
                    var oxiSendGridInfo = oDXI.Get_SendGridAccountDetails(ObjID);
                    if (oxiSendGridInfo.bOK && oxiSendGridInfo.oResult != null)
                    {
                        oSendGridInfo = (XIDSendGridAccountDetails)oxiSendGridInfo.oResult;
                    }
                    return oSendGridInfo;
                case "sendgridtemplate":
                    XIDSendGridTemplate oSGTemplate = new XIDSendGridTemplate();
                    var oxiSGTemplate = oDXI.Get_SendGridTemplate(ObjName, ObjID);
                    if (oxiSGTemplate.bOK && oxiSGTemplate.oResult != null)
                    {
                        oSGTemplate = (XIDSendGridTemplate)oxiSGTemplate.oResult;
                    }
                    return oSGTemplate;
                case "distribute":
                    XIDDistribute oDistribute = new XIDDistribute();
                    var oxiDistribute = oDXI.Get_DistributionDefinition(ObjName, ObjID);
                    if (oxiDistribute.bOK && oxiDistribute.oResult != null)
                    {
                        oDistribute = (XIDDistribute)oxiDistribute.oResult;
                    }
                    return oDistribute;
                case "xiaccount":
                    XIDAccount oAccount = new XIDAccount();
                    var oxiAccount = oDXI.Get_XIAccountDefinition(ObjName, ObjID);
                    if (oxiAccount.bOK && oxiAccount.oResult != null)
                    {
                        oAccount = (XIDAccount)oxiAccount.oResult;
                    }
                    return oAccount;
                case "xirefcomtype":
                    XIIBO orefComTypeI = new XIIBO();
                    var oxirefComTypeI = oDXI.Get_XIObjectDefinition(ObjName, ObjID);
                    if (oxirefComTypeI.bOK && oxirefComTypeI.oResult != null)
                    {
                        orefComTypeI = (XIIBO)oxirefComTypeI.oResult;
                    }
                    return orefComTypeI;
                case "xiapisetting":
                    XIIBO oAPISettingI = new XIIBO();
                    var oxiapisettingI = oDXI.Get_XIObjectDefinition(ObjName, ObjID);
                    if (oxiapisettingI.bOK && oxiapisettingI.oResult != null)
                    {
                        oAPISettingI = (XIIBO)oxiapisettingI.oResult;
                    }
                    return oAPISettingI;
                case "format":
                    XIDFormat oFormatI = new XIDFormat();
                    var oFormatDef = oDXI.Get_XIFormatDefinition(ObjName, ObjID);
                    if (oFormatDef.bOK && oFormatDef.oResult != null)
                    {
                        oFormatI = (XIDFormat)oFormatDef.oResult;
                    }
                    return oFormatI;
                case "doctype":
                    XIInfraDocTypes oDocType = new XIInfraDocTypes();
                    var oDocDef = oDocType.Get_FileDocTypes(ObjName, ObjID);
                    if (oDocDef.bOK && oDocDef.oResult != null)
                    {
                        oDocType = (XIInfraDocTypes)oDocDef.oResult;
                    }
                    return oDocType;
                case "qsparams":
                    List<XIRequiredParamDef> oQSParam = new List<XIRequiredParamDef>();
                    var oQSParamDef = oDXI.Get_XIRequiredParamDefinition(ObjName, ObjID);
                    if (oQSParamDef.bOK && oQSParamDef.oResult != null)
                    {
                        oQSParam = (List<XIRequiredParamDef>)oQSParamDef.oResult;
                    }
                    return oQSParam;
                case "bosignalr":
                    List<XISignalRsettings> oBOSignalR = new List<XISignalRsettings>();
                    var oBOSignalRDef = oDXI.Get_BOSignalRConfig(ObjID);
                    if (oBOSignalRDef.bOK && oBOSignalRDef.oResult != null)
                    {
                        oBOSignalR = (List<XISignalRsettings>)oBOSignalRDef.oResult;
                    }
                    return oBOSignalR;
                default:
                    return null;
            }
        }
        
        public CResult Attach_OrgData(XIDBO oBODe, int iOrgID)
        {
            CResult oCResult = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack
            {
                sClass = this.GetType().Name,
                sMethod = MethodBase.GetCurrentMethod().Name,
                iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess,
                sTask = "Attach org data to org fk attributes"
            };

            try
            {
                // Validate input parameters
                if (oBODe == null || oBODe.Attributes.Count == 0 || iOrgID <= 0)
                {
                    //throw new ArgumentException("Mandatory parameters missing: iOrgID or BO definition is null/empty.");
                    oCResult.oResult = oBODe;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }

                oTrace.oParams.Add(new CNV { sName = "iOrgID", sValue = iOrgID.ToString() });

                XIDBO oBODCopy = new XIDBO();
                var FKAttrs = new List<XIDAttribute>();
                // Get Foreign Key Attributes marked for organization-level data
                var FKAttrs1 = oBODe.Attributes.Values.Where(attr => attr.bOrgFK && (attr.FKiType == 10 || attr.FKiType == 40)).ToList();
                if (FKAttrs1.Count == 0)
                {
                    oBODCopy = oBODe;
                    //throw new InvalidOperationException("No organization-level foreign key attributes found.");
                }
                else
                {
                    XIInfraCache oCache = new XIInfraCache();
                    // Construct cache key
                    string sAppName = string.Empty;
                    if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["AppName"] != null)
                    {
                        sAppName = HttpContext.Current.Session["AppName"].ToString();
                    }
                    var DataCacheKey = sAppName + "_Definition_ReferenceData_" + oBODe.Name + "_" + oBODe.XIGUID + "_" + iOrgID;
                    var cachedData = oCache.GetFromCache(DataCacheKey) as XIDBO;
                    if (cachedData == null)
                    {
                        oBODCopy = (XIDBO)ObjectCloner.DeepClone(oBODe);
                        FKAttrs = oBODCopy.Attributes.Values.Where(attr => attr.bOrgFK && (attr.FKiType == 10 || attr.FKiType == 40)).ToList();

                        foreach (var FKAttr in FKAttrs)
                        {
                            if (FKAttr.FKiType == 10)
                            {
                                // Declare FKDDL at the beginning of the loop
                                List<XIDropDown> FKDDL = null;

                                // Validate FK attribute properties
                                if (string.IsNullOrEmpty(FKAttr.sFKBOName) || string.IsNullOrEmpty(FKAttr.FKTableName))
                                    continue;

                                // Retrieve BO definition from cache
                                var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, FKAttr.sFKBOName);
                                if (FKBOD == null)
                                    continue;

                                var oLabelD = FKBOD.GroupD("label");
                                if (oLabelD != null && !string.IsNullOrEmpty(oLabelD.BOFieldNames))
                                {
                                    // Fetch data from the database
                                    var o1ClickD = new XID1Click
                                    {
                                        BOIDXIGUID = FKBOD.XIGUID,
                                        Query = "SELECT " + oLabelD.BOFieldNames + " FROM " + FKBOD.TableName
                                    };

                                    var Results = o1ClickD.OneClick_Run();
                                    if (Results != null && Results.Count > 0)
                                    {
                                        FKDDL = Results
                                            //.Where(kvp => kvp.Value.Attributes.Any(attr => attr.Value.sName == "FKiOrgID" && attr.Value.sValue == iOrgID.ToString()))
                                            .Select(kvp => new XIDropDown
                                            {
                                                text = kvp.Value.AttributeI("id").sValue,
                                                Expression = kvp.Value.AttributeI("sName").sValue
                                            }).ToList();

                                        //oCache.InsertIntoCache(FKDDL, DataCacheKey);
                                    }
                                }


                                // Assign FKDDL to the attribute if not null
                                if (FKDDL != null && FKDDL.Count > 0)
                                {
                                    var lowerName = FKAttr.Name.ToLower();
                                    if (oBODCopy.Attributes.ContainsKey(lowerName))
                                    {
                                        oBODCopy.Attributes[lowerName].FieldDDL = FKDDL;
                                        oBODCopy.Attributes[lowerName].sFKBOSize = FKBOD.sSize;
                                    }
                                }
                            }
                            else if (FKAttr.FKiType == 40)
                            {
                                if (!string.IsNullOrEmpty(FKAttr.sFKBOName) && FKAttr.iOneClickIDXIGUID != null && FKAttr.iOneClickIDXIGUID != Guid.Empty)
                                {
                                    List<XIDropDown> FKDDL = new List<XIDropDown>();
                                    var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, FKAttr.sFKBOName);
                                    //var DataCacheKey = sAppName + "_Definition_ReferenceData_" + FKBOD.Name + "_" + FKBOD.XIGUID + "_" + FKAttr.iOneClickIDXIGUID + "_" + iOrgID;

                                    XIDXI oXID = new XIDXI();
                                    string suid = "1click_" + Convert.ToString(FKAttr.iOneClickIDXIGUID);
                                    List<CNV> oParams = new List<CNV>();
                                    oParams.Add(new CNV { sName = "{XIP|iAPPID}", sValue = FKBOD.FKiApplicationID.ToString() });
                                    oParams.Add(new CNV { sName = "{XIP|iOrgID}", sValue = iOrgID.ToString() });
                                    var oResult = oXID.Get_AutoCompleteList(suid, null, oParams);
                                    if (oResult.bOK && oResult.oResult != null)
                                    {
                                        if (FKBOD.sSize == "10")
                                        {
                                            var DDL = (List<XIDFieldOptionList>)oResult.oResult;
                                            FKDDL = DDL.Select(m => new XIDropDown { text = m.sOptionValue, Expression = m.sOptionName }).ToList();
                                        }
                                        else
                                        {
                                            var DDL = (Dictionary<string, string>)oResult.oResult;
                                            FKDDL = DDL.Select(m => new XIDropDown { text = m.Key, Expression = m.Value }).ToList();
                                        }
                                        //oCache.InsertIntoCache(FKDDL, DataCacheKey);
                                    }

                                    // Assign FKDDL to the attribute if not null
                                    if (FKDDL != null && FKDDL.Count > 0)
                                    {
                                        var lowerName = FKAttr.Name.ToLower();
                                        if (oBODCopy.Attributes.ContainsKey(lowerName))
                                        {
                                            oBODCopy.Attributes[lowerName].FieldDDL = FKDDL;
                                            oBODCopy.Attributes[lowerName].sFKBOSize = FKBOD.sSize;
                                        }
                                    }
                                }
                            }
                        }
                        oCache.InsertIntoCache(oBODCopy, DataCacheKey);
                    }
                    else
                    {
                        oBODCopy = cachedData;
                    }
                }

                oCResult.oResult = oBODCopy;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                // Log and handle exception
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                var stackTrace = new StackTrace(ex, true);
                int line = stackTrace.GetFrame(0)?.GetFileLineNumber() ?? -1;
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace;
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;

                var oDefBase = new XIDefinitionBase();
                oDefBase.SaveErrortoDB(oCResult);
            }
            finally
            {
                watch.Stop();
                oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
                oCResult.oTrace = oTrace;
            }

            return oCResult;
        }

        public CResult Attach_QSOrgData(XIDQS oQSD, int iOrgID, string sGUID)
        {
            CResult oCResult = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack
            {
                sClass = this.GetType().Name,
                sMethod = MethodBase.GetCurrentMethod().Name,
                iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess,
                sTask = "Attach org data to org QS fields option list"
            };

            try
            {
                // Validate input parameters
                if (oQSD == null || oQSD.OrgFieldOrigins.Count == 0 || iOrgID <= 0)
                {
                    //throw new ArgumentException("Mandatory parameters missing: iOrgID or BO definition is null/empty.");
                    oCResult.oResult = oQSD;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }

                oTrace.oParams.Add(new CNV { sName = "iOrgID", sValue = iOrgID.ToString() });
                XIDXI oXID = new XIDXI();
                XIDQS oQSDCopy = new XIDQS();
                Dictionary<Guid, XIDFieldOrigin> QSDDL = new Dictionary<Guid, XIDFieldOrigin>();

                XIInfraCache oCache = new XIInfraCache();
                // Construct cache key
                string sAppName = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["AppName"] != null)
                {
                    sAppName = HttpContext.Current.Session["AppName"].ToString();
                }
                var DataCacheKey = sAppName + "_Definition_ReferenceData_" + oQSD.sName + "_" + oQSD.XIGUID + "_" + iOrgID;
                var cachedData = oCache.GetFromCache(DataCacheKey) as XIDQS;
                if (cachedData == null)
                {
                    oQSDCopy = (XIDQS)ObjectCloner.DeepClone(oQSD);

                    foreach (var FKAttr in oQSD.OrgFieldOrigins)
                    {
                        XIDFieldOrigin oFD = new XIDFieldOrigin();
                        XIDFieldOrigin oOrgFD = new XIDFieldOrigin();
                        oFD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, FKAttr.ToString());
                        if (oFD.FKiBOIDXIGUID != null && oFD.FKiBOIDXIGUID != Guid.Empty)
                        {
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oFD.FKiBOIDXIGUID.ToString());
                            if (oBOD != null)
                            {
                                if (oBOD.sSize == "10")
                                {
                                    string sBODataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                                    if (oBOD.Groups.ContainsKey("label"))
                                    {
                                        var oGroupD = oBOD.Groups["label"];
                                        var Con = new XIDBAPI(sBODataSource);
                                        var LabelGroup = oGroupD.BOFieldNames;
                                        if (!string.IsNullOrEmpty(LabelGroup))
                                        {
                                            string FinalString = LabelGroup;
                                            string sWhrClause = "";

                                            if (oBOD.iOrgObject == 1)
                                            {
                                                sWhrClause = "[" + oBOD.TableName + "].FKiOrgID=" + iOrgID;
                                            }
                                            Dictionary<string, string> DDL = Con.SelectDDL(FinalString, oBOD.TableName, sWhrClause);
                                            var FKDDL1 = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                            oOrgFD.FieldDynamicOptionList = FKDDL1;
                                            oOrgFD.sBOSize = oBOD.sSize;
                                            QSDDL[FKAttr] = oOrgFD;
                                        }
                                    }
                                }
                            }
                            // Assign FKDDL to the attribute if not null                            
                        }
                        else if (oFD.FK1ClickIDXIGUID != null && oFD.FK1ClickIDXIGUID != Guid.Empty)
                        {
                            var o1ClickD = new XID1Click();
                            if (oFD.FK1ClickIDXIGUID != null && oFD.FK1ClickIDXIGUID != Guid.Empty)
                            {
                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, oFD.FK1ClickIDXIGUID.ToString());
                            }
                            else if (oFD.FK1ClickID > 0)
                            {
                                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, oFD.FK1ClickID.ToString());
                            }
                            XIDBO oBOD = new XIDBO();
                            if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                            {
                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                            }
                            else if (o1ClickD.BOID > 0)
                            {
                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                            }
                            int iDataSource = oBOD.iDataSource;
                            Guid iDataSourceXIGUID = oBOD.iDataSourceXIGUID;
                            string sConntection = oXID.GetBODataSource(iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                            var Connect = new XIDBAPI(sConntection);
                            o1ClickD.BOD = oBOD;
                            o1ClickD.Get_1ClickHeadings();
                            var PrimaryKey = "";
                            if (o1ClickD.bIsMultiBO == true)
                            {
                                var fields = "[" + o1ClickD.FromBos + "].";
                                fields = fields + string.Join(" [" + o1ClickD.FromBos + "].", o1ClickD.TableColumns.ToList());
                                var TableColumns = fields.Split(' ');
                                o1ClickD.TableColumns = TableColumns.ToList();
                                PrimaryKey = "[" + o1ClickD.FromBos + "]." + oBOD.sPrimaryKey;
                            }
                            o1ClickD.TableColumns.Remove("HiddenData");
                            o1ClickD.TableColumns.Remove("[" + oBOD.TableName + "].HiddenData");


                            var SelFields = string.Join(", ", o1ClickD.TableColumns.ToList());
                            XIDGroup oGroupD = new XIDGroup();
                            string FinalString = SelFields;
                            //option list showing id removed in Drop down list for the QS Fields
                            // string FinalString = oGroupD.ConcatanateFields(SelFields, " ");
                            //FinalString = (o1ClickD.bIsMultiBO == true ? PrimaryKey : oBOD.sPrimaryKey) + "," + FinalString;
                            var FinalQuery = o1ClickD.AddSelectPart(o1ClickD.Query, FinalString);
                            if (!FinalQuery.Contains("{XIP|"))
                            {
                                Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                    .ToDictionary(x => x.key, x => x.value);
                                oOrgFD.FieldDynamicOptionList = FKDDL;
                                oOrgFD.sBOSize = oBOD.sSize;
                                QSDDL[FKAttr] = oOrgFD;
                            }
                            else if (FinalQuery.Contains("{XIP|-") || FinalQuery.Contains("{XIP|"))
                            {
                                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                                List<CNV> nParams = new List<CNV>();
                                nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                                XID1Click oC1ClickD = (XID1Click)o1ClickD.Clone(o1ClickD);
                                oC1ClickD.Query = FinalQuery;
                                oC1ClickD.ReplaceFKExpressions(nParams);
                                FinalQuery = oC1ClickD.Query;
                                Dictionary<string, string> DDL = Connect.GetDDLItems(CommandType.Text, FinalQuery, null);
                                var FKDDL = DDL.Select(m => new XIDFieldOptionList { sOptionName = m.Value, sOptionValue = m.Key }).ToList();
                                var DDList = FKDDL.Where(m => !string.IsNullOrEmpty(m.sOptionName)).GroupBy(m => m.sOptionName).Select(m => m.FirstOrDefault()).ToList().Select(p => new { key = p.sOptionName, value = p.sOptionValue })
                                    .ToDictionary(x => x.key, x => x.value);
                                oOrgFD.FieldDynamicOptionList = FKDDL;
                                oOrgFD.sBOSize = oBOD.sSize;
                                QSDDL[FKAttr] = oOrgFD;
                            }
                        }
                    }
                    if(QSDDL != null && QSDDL.Count() > 0)
                    {
                        foreach(var Step in oQSD.Steps.Values.ToList())
                        {
                            foreach(var Sec in Step.Sections.Values.ToList())
                            {
                                foreach (var field in Sec.FieldDefs)
                                {
                                    if (QSDDL.ContainsKey(field.Value.FKiXIFieldOriginIDXIGUID))
                                    {
                                        var oFD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, field.Value.FKiXIFieldOriginIDXIGUID.ToString());
                                        oQSDCopy.Steps[Step.sName].Sections[Sec.XIGUID + "_Sec"].FieldDefs[oFD.sName].FieldOrigin.FieldDynamicOptionList = QSDDL[field.Value.FKiXIFieldOriginIDXIGUID].FieldDynamicOptionList;
                                        oQSDCopy.Steps[Step.sName].Sections[Sec.XIGUID + "_Sec"].FieldDefs[oFD.sName].FieldOrigin.sBOSize = QSDDL[field.Value.FKiXIFieldOriginIDXIGUID].sBOSize;
                                    }
                                }
                            }
                        }
                        oCache.InsertIntoCache(oQSDCopy, DataCacheKey);
                    }
                }
                else
                {
                    oQSDCopy = cachedData;
                }
                oCResult.oResult = oQSDCopy;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                // Log and handle exception
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                var stackTrace = new StackTrace(ex, true);
                int line = stackTrace.GetFrame(0)?.GetFileLineNumber() ?? -1;
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace;
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;

                var oDefBase = new XIDefinitionBase();
                oDefBase.SaveErrortoDB(oCResult);
            }
            finally
            {
                watch.Stop();
                oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
                oCResult.oTrace = oTrace;
            }

            return oCResult;
        }
        public void InsertIntoCache(object oCacheObj, string sCacheKey)
        {
            HttpRuntime.Cache.Insert(sCacheKey, oCacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public object GetFromCache(string sCacheKey)
        {
            object oCacheObj = HttpRuntime.Cache[sCacheKey];
            return oCacheObj;
        }

        public void UpdateCacheObject(string ObjName, string sGUID, object Obj, string sDatabase, int ID = 0)
        {
            HttpRuntime.Cache.Insert(ObjName + "_" + sGUID + "_" + ID, Obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            var QSet = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
        }

        public XIIQS Set_QuestionSetCache(string ObjName, string sGUID, string ID, XIIQS oCacheobj)
        {
            object obj;
            HttpRuntime.Cache.Remove(ObjName + "_" + sGUID + "_" + ID);
            obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
            HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return oCacheobj;
        }

        public bool Set_ObjectSetCache(string SessionID, string keyName, string sGUID, object oCacheobj)
        {
            //object obj;
            HttpRuntime.Cache.Remove(keyName + "_" + sGUID + "_" + SessionID);
            //obj = HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID];
            HttpRuntime.Cache.Add(keyName + "_" + sGUID + "_" + SessionID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return true;
        }
        public object Get_ObjectSetCache(string keyName, string sGUID, string SessionID)
        {
            object obj = null;
            //XIIQS oCacheobj = new XIIQS();
            if (HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID] != null)
            {
                //HttpRuntime.Cache.Add(keyName + "_" + sGUID + "_" + SessionID, obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                obj = HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID];
            }
            return obj;
        }

        public XIIQS Get_QuestionSetCache(string ObjName, string sGUID, string ID)
        {
            object obj;
            XIIQS oCacheobj = new XIIQS();
            if (HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID] == null)
            {
                HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            else
            {
                obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
                oCacheobj = (XIIQS)obj;
                return oCacheobj;
            }
            Get_QuestionSetCache(ObjName, sGUID, ID);
            return oCacheobj;
        }
        //public XiLinks GetXiLinkDetails(int XiLinkID, string sGUID, int iUserID, string sOrgName, string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    XiLinks oXilink = new XiLinks();
        //    oXilink = (XiLinks)GetObjectFromCache(XIConstant.CacheXILink, sGUID, iUserID, sOrgName, sDatabase, XiLinkID);
        //    return oXilink;
        //}    

        public string CacheKeyBuilder(string sObjType, string sName, string ID, string AppName = "", int iOrgID = 0, bool bOrg = false, int iAppID = 0)
        {
            string sCacheKey = string.Empty;
            var sAppName = string.Empty;// HttpContext.Current.Session["AppName"].ToString();// Singleton.Instance.sAppName;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["AppName"] != null)
                {
                    sAppName = HttpContext.Current.Session["AppName"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(AppName))
            {
                sCacheKey = sCacheKey + AppName.Replace(" ", "") + "_";
            }
            else if (!string.IsNullOrEmpty(sAppName))
            {
                sCacheKey = sCacheKey + sAppName.Replace(" ", "") + "_";
            }
            sCacheKey = sCacheKey + "Definition_";
            if (iAppID > 0)
            {
                sCacheKey = sCacheKey + iAppID + "_";
            }
            if (!string.IsNullOrEmpty(sObjType))
            {
                sCacheKey = sCacheKey + sObjType + "_";
            }
            if (!string.IsNullOrEmpty(sName))
            {
                sCacheKey = sCacheKey + sName + "_";
            }
            if (!string.IsNullOrEmpty(ID) && ID != "0")
            {
                sCacheKey = sCacheKey + ID + "_";
            }
            if (iOrgID > 0 && bOrg)
            {
                sCacheKey = sCacheKey + iOrgID + "_";
            }
            if (!string.IsNullOrEmpty(sCacheKey))
            {
                sCacheKey = sCacheKey.Substring(0, sCacheKey.Length - 1);
            }
            return sCacheKey;
        }

        public CResult MergeXILinkParameters(XILink oXiLink, string sGUID = "", List<CNV> oParams = null, string sSessionID = "")
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
                //string sSessionID = HttpContext.Current.Session.SessionID;
                if (oXiLink != null && oXiLink.XiLinkNVs != null && oXiLink.XiLinkNVs.Count() > 0)
                {
                    foreach (var param in oXiLink.XiLinkNVs)
                    {
                        string sResolvedValue = string.Empty;

                        if (!string.IsNullOrEmpty(param.Name) && !string.IsNullOrEmpty(param.Value))
                        {
                            CResult oCRes = new CResult();
                            oCRes = ResolveMe(param.Value, sSessionID, sGUID);
                            if (oCRes.bOK)
                            {
                                sResolvedValue = (string)oCRes.oResult;
                                if (param.Name == "{XIP|sAddonID}")
                                {
                                    sResolvedValue = sResolvedValue.StartsWith("|||") == true ? sResolvedValue.Substring(3, sResolvedValue.Length - 3).Replace("|||", ",") : sResolvedValue.Replace("|||", ",");
                                }
                                Set_ParamVal(sSessionID, sGUID, null, param.Name, sResolvedValue, param.sType, null);
                            }
                        }
                        if (param.Name == "{XIP|sAddonID}" && string.IsNullOrEmpty(param.Value))
                        {
                            sResolvedValue = "0";
                            Set_ParamVal(sSessionID, sGUID, null, param.Name, sResolvedValue, param.sType, null);
                        }
                    }
                }
                if (oParams != null && oParams.Count() > 0)
                {
                    foreach (var param in oParams)
                    {
                        string sResolvedValue = string.Empty;
                        if (!string.IsNullOrEmpty(param.sName) && !string.IsNullOrEmpty(param.sValue))
                        {
                            CResult oCRes = new CResult();
                            oCRes = ResolveMe(param.sValue, sSessionID, sGUID);
                            if (oCRes.bOK)
                            {
                                sResolvedValue = string.Empty;
                                sResolvedValue = (string)oCRes.oResult;
                                if (param.sName == "{XIP|sAddonID}")
                                {
                                    sResolvedValue = sResolvedValue.StartsWith("|||") == true ? sResolvedValue.Substring(3, sResolvedValue.Length - 3).Replace("|||", ",") : sResolvedValue.Replace("|||", ",");
                                }
                                Set_ParamVal(sSessionID, sGUID, null, param.sName, sResolvedValue, param.sType, null);
                            }
                        }
                        if (param.sName == "{XIP|sAddonID}" && string.IsNullOrEmpty(param.sValue))
                        {
                            sResolvedValue = "0";
                            Set_ParamVal(sSessionID, sGUID, null, param.sName, sResolvedValue, param.sType, null);
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xilink params into cache. XILinkID is " + oXiLink.XiLinkID });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIns.SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult AddParamsToGUID(string ParameterID, string sGUID, string sChildGUID)
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
                XIParameter oXIParam = null;
                if (string.IsNullOrEmpty(sChildGUID))
                {
                    sChildGUID = sGUID;
                }
                int iParameterID = 0;
                int.TryParse(ParameterID, out iParameterID);
                Guid ParameterGUID = Guid.Empty;
                Guid.TryParse(ParameterID, out ParameterGUID);
                if (iParameterID > 0 || (ParameterGUID != null && ParameterGUID != Guid.Empty))
                {
                    if (ParameterGUID != null && ParameterGUID != Guid.Empty)
                    {
                        oXIParam = (XIParameter)GetObjectFromCache(XIConstant.CacheXIParamater, null, ParameterGUID.ToString());
                    }
                    else if (iParameterID > 0)
                    {
                        oXIParam = (XIParameter)GetObjectFromCache(XIConstant.CacheXIParamater, null, iParameterID.ToString());
                    }
                    if (oXIParam != null)
                    {
                        string sSessionID = HttpContext.Current.Session.SessionID;
                        if (oXIParam.XiParameterNVs != null && oXIParam.XiParameterNVs.Count() > 0)
                        {
                            foreach (var param in oXIParam.XiParameterNVs.OrderBy(a => a.ID))
                            {
                                if (!string.IsNullOrEmpty(param.Value))
                                {
                                    CResult oCRes = new CResult();
                                    if (!string.IsNullOrEmpty(param.Type) && param.Type.ToLower() == "self")
                                    {
                                        oCRes = ResolveMe(param.Value, sSessionID, sChildGUID);
                                    }
                                    else
                                    {
                                        oCRes = ResolveMe(param.Value, sSessionID, sGUID);
                                    }
                                    if (oCRes.bOK)
                                    {
                                        string sResolvedValue = string.Empty;
                                        sResolvedValue = (string)oCRes.oResult;
                                        if (!string.IsNullOrEmpty(param.sContext))
                                        {
                                            Set_ParamVal(sSessionID, sChildGUID, param.sContext, param.Name, sResolvedValue, null, null);
                                        }
                                        else
                                        {
                                            Set_ParamVal(sSessionID, sChildGUID, null, param.Name, sResolvedValue, null, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache: XIParameter ID is " + ParameterID });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIns.SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public List<CNV> ResolveParameters(List<CNV> Params, string sSessionID, string sGUID)
        {
            if (!string.IsNullOrEmpty(sGUID))
            {
                foreach (var param in Params)
                {
                    string sResolvedValue = string.Empty;
                    CResult oCR = new CResult();
                    if (!string.IsNullOrEmpty(param.sValue))
                    {
                        oCR = ResolveMe(param.sValue, sSessionID, sGUID);
                        if (oCR.bOK)
                        {
                            sResolvedValue = (string)oCR.oResult;
                            Params.Where(m => m.sName.ToLower() == param.sName.ToLower()).FirstOrDefault().sValue = sResolvedValue;
                        }
                    }
                }
            }
            return Params;
        }

        public CResult ResolveMe(string sParam, string sSessionID, string sGUID)
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
                string sValue = string.Empty;
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(sParam);
                if (sParam.StartsWith("xi."))
                {
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sParam.ToString();
                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sValue = (string)oCR.oResult;
                    }
                }
                else if (sParam.Contains("xi.s") || sParam.Contains("xi.r"))
                {
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sParam.ToString();
                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sValue = (string)oCR.oResult;
                    }
                    oCResult.sMessage = "xiscript executed:" + sParam + " - Merged: " + oXIScript.sScript + " - Result: " + sValue;
                }
                else if (sParam.StartsWith("{-"))
                {
                    sValue = Get_ParamVal(sSessionID, sGUID, null, sParam);
                }
                else if (sParam.StartsWith("-xip"))
                {
                    sValue = Get_ParamVal(sSessionID, sGUID, null, sParam);
                }
                else if (sParam.StartsWith("xic."))
                {
                    XIAPI oAPI = new XIAPI();
                    var split = sParam.Split('.').ToList();
                    if (split[1].ToLower() == "iuserid")
                    {
                        var oUser = oAPI.GetUserID();
                        if (oUser.bOK && oUser.oResult != null)
                        {
                            sValue = ((int)oUser.oResult).ToString();
                        }
                    }
                    else if (split[1].ToLower() == "icampaignid")
                    {
                        var CampID = oAPI.GetCampaignID();
                        if (CampID.bOK && CampID.oResult != null)
                        {
                            sValue = ((string)CampID.oResult).ToString();
                        }
                    }
                }
                else if (matches.Count > 0)
                {
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
                                    sValue = Get_ParamVal(sSessionID, sGUID, null, Prm);
                                }
                                else if (SplitPipe[0].ToLower() == "xir")
                                {

                                }
                                else if (SplitPipe[0].ToLower() == "xis")
                                {
                                    XIDScript oXIScript = new XIDScript();
                                    oXIScript.sScript = match.ToString();
                                    oXIScript.Execute_Script(sGUID, sSessionID);
                                }
                            }
                        }
                    }
                }
                else
                {
                    sValue = sParam;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = sValue;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIns.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        #endregion CacheObject
        public string GetParentGUIDRecurrsive(string sGUID)
        {
            sParGUID = sGUID;
            var sParentGUID = Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
            if (!string.IsNullOrEmpty(sParentGUID))
            {
                GetParentGUIDRecurrsive(sParentGUID);
            }
            else
            {
                return sParGUID;
            }
            return sParGUID;
        }

        // REMOVING CACHE WITH KEY
        public static void RemoveCacheWithKey(string skey)
        {
            if (HttpRuntime.Cache[skey] != null)
            {
                HttpRuntime.Cache.Remove(skey);
            }
        }

        public void CacheDatabaseRecords(object obj)
        {
            string sKey = HttpContext.Current.Session.SessionID + "_" + XIConstant.RecordsCacheKey;
            RemoveCacheWithKey(sKey);
            HttpRuntime.Cache.Add(sKey, obj, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public CResult Remove_ConfigCache(XIIBO oBOI)
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
                var oAppD = new XIDApplication();
                var ID = oBOI.AttributeI(oBOI.BOD.sPrimaryKey).sValue;
                var sBOName = oBOI.BOD.Name;
                var AppName = string.Empty;
                oTrace.oParams.Add(new CNV { sName = "sBOName", sValue = sBOName });
                switch (sBOName.ToLower())
                {
                    case "xibo":
                    case "xi1click":
                        var sCacheKey = string.Empty;
                        if (sBOName.ToLower() == "xibo")
                        {
                            sCacheKey = "bo";
                        }
                        else if (sBOName.ToLower() == "xi1click")
                        {
                            sCacheKey = "oneclick";
                        }
                        var sAppID = oBOI.AttributeI("fkiapplicationid").sValue;
                        oAppD = (XIDApplication)GetObjectFromCache(XIConstant.CacheApplication, null, sAppID.ToString());
                        AppName = oAppD.sApplicationName;
                        var sName = oBOI.AttributeI("name").sValue;
                        oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID });
                        oTrace.oParams.Add(new CNV { sName = "sName", sValue = sName });
                        var sKey = CacheKeyBuilder(sCacheKey, sName, "", AppName);
                        RemoveCacheWithKey(sKey);
                        sKey = CacheKeyBuilder(sCacheKey, "", ID, AppName);
                        RemoveCacheWithKey(sKey);
                        if (sCacheKey == "bo")
                        {
                            sKey = CacheKeyBuilder("bo_all", sName, "", AppName);
                            RemoveCacheWithKey(sKey);
                            sKey = CacheKeyBuilder("bo_all", "", ID, AppName);
                            RemoveCacheWithKey(sKey);
                        }
                        break;
                    case "xiboattribute":
                    case "xibogroup":
                        var sBODID = oBOI.AttributeI("boid").sValue;
                        var iBODID = 0;
                        int.TryParse(sBODID, out iBODID);
                        if (iBODID > 0)
                        {
                            var BOD = (XIDBO)GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString());
                            sAppID = BOD.FKiApplicationID.ToString();
                            oAppD = (XIDApplication)GetObjectFromCache(XIConstant.CacheApplication, null, sAppID.ToString());
                            AppName = oAppD.sApplicationName;
                            sName = BOD.Name;
                            ID = BOD.BOID.ToString();
                            oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID });
                            oTrace.oParams.Add(new CNV { sName = "sName", sValue = sName });
                            sKey = CacheKeyBuilder("bo", sName, "", AppName);
                            RemoveCacheWithKey(sKey);
                            sKey = CacheKeyBuilder("bo", "", ID, AppName);
                            RemoveCacheWithKey(sKey);
                        }
                        break;
                    case "xi menu":
                        sAppID = oBOI.AttributeI("fkiapplicationid").sValue;
                        oAppD = (XIDApplication)GetObjectFromCache(XIConstant.CacheApplication, null, sAppID.ToString());
                        AppName = oAppD.sApplicationName;
                        sName = oBOI.AttributeI("rootname").sValue;
                        oTrace.oParams.Add(new CNV { sName = "sName", sValue = sName });
                        sKey = CacheKeyBuilder("menu", sName, "", AppName);
                        RemoveCacheWithKey(sKey);
                        break;
                    default:
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        break;
                }
                if (oTrace.iStatus != (int)xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.oResult = "Success";
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
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

        public string Clear_GuidCache(string sSessionID, string sGUID)
        {
            XICacheInstance oCache = Get_XICache();
            if (oCache.NMyInstance.ContainsKey("XISession"))
            {
                if (oCache.NMyInstance["XISession"].NMyInstance.ContainsKey("SS_" + sSessionID))
                {
                    if (oCache.NMyInstance["XISession"].NMyInstance["SS_" + sSessionID].NMyInstance.ContainsKey("UID_" + sGUID))
                    {
                        oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NMyInstance.Remove("UID_" + sGUID);
                    }
                }
            }
            return "Success";
        }

        public void Clear_DefInCache(string skey)
        {
            if (HttpRuntime.Cache[skey] != null)
            {
                HttpRuntime.Cache.Remove(skey);
                CResult oCResult = new CResult();
                XIDefinitionBase oDef = new XIDefinitionBase();
                oCResult.sMessage = "Cache: QS, Clear_DefInCache(), Cache cleared for key:" + skey + " on " + System.DateTime.Now;
                oCResult.sCategory = "C# Dynamic script";
                oDef.SaveErrortoDB(oCResult);
            }
            else
            {
                CResult oCResult = new CResult();
                XIDefinitionBase oDef = new XIDefinitionBase();
                oCResult.sMessage = "Cache: QS, Clear_DefInCache(), Cache cleared for key:" + skey + " on " + System.DateTime.Now;
                oCResult.sCategory = "C# Dynamic script";
                oDef.SaveErrortoDB(oCResult);
            }
        }
    }
}