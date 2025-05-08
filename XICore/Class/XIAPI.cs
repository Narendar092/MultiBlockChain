using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIAPI : XIDefinitionBase
    {
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult GetUserRole()
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
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                if (oInfo.sRoleName != null)
                {
                    oCResult.oResult = oInfo.sRoleName;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting user role information" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult GetUserID()
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
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                //oInfo = oInfo.GetUserInfo();
                oCResult.oResult = oInfo.iUserID;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting user role information" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult GetCampaignID()
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
                var iCampaignID = HttpContext.Current.Session["iCampaignID"].ToString();
                oCResult.oResult = iCampaignID;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting user role information" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Parameter(List<CNV> Params)
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
                string sSessionID = Params.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sGUID = Params.Where(m => m.sName.ToLower() == "sguid").Select(m => m.sValue).FirstOrDefault();
                string sParamName = Params.Where(m => m.sName.ToLower() == "sParamName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var oResponse = oCache.Get_ParamVal(sSessionID, sGUID, null, sParamName);
                if (oResponse != null)
                {
                    sParamVal = (string)oResponse;
                }
                oCResult.oResult = sParamVal;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting parameter from cache using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult Set_Parameter(List<CNV> Params)
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
                string sSessionID = Params.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sGUID = Params.Where(m => m.sName.ToLower() == "sguid").Select(m => m.sValue).FirstOrDefault();
                string sParamName = Params.Where(m => m.sName.ToLower() == "sParamName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = Params.Where(m => m.sName.ToLower() == "sParamValue".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                var oResponse = oCache.Set_ParamVal(sSessionID, sGUID, null, sParamName, sParamVal, null, null);
                oCResult.oResult = sParamVal;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting parameter from cache using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult GetBOAttribute(List<CNV> Params)
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
                string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = Params.Where(m => m.sName.ToLower() == "sInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAttrName = Params.Where(m => m.sName.ToLower() == "sAttrName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sLoadByAttr = Params.Where(m => m.sName.ToLower() == "sLoadByAttr".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSession = Params.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sGUID = Params.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();

                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                XIIXI oXI = new XIIXI();
                string sAttrValue = string.Empty; Guid iDataSourceID;
                var oParams = new Dictionary<string, object>();
                //oParams["Name"] = sBOName;
                string BoTableName = oBOD.TableName; //Connection.Select<string>("XIBO_T_N", oParams, "TableName").FirstOrDefault();
                //oParams = new Dictionary<string, object>();
                //oParams["Name"] = sBOName;
                string sBODataSource = oBOD.iDataSourceXIGUID.ToString(); //Connection.Select<string>("XIBO_T_N", oParams, "iDataSource").FirstOrDefault();
                XIDXI oXID = new XIDXI();
                if (Guid.TryParse(sBODataSource, out iDataSourceID))
                {
                    var DataSource = oXID.GetBODataSource(iDataSourceID.ToString(), oBOD.FKiApplicationID);
                    XIDBAPI sConnection = new XIDBAPI(DataSource);
                    oParams = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(sLoadByAttr))
                    {
                        if (sLoadByAttr.Contains(":"))
                        {
                            var oLoadByAttrs = sLoadByAttr.Split('_');
                            if (oLoadByAttrs != null && oLoadByAttrs.Count() > 0)
                            {
                                foreach (var sAttr in oLoadByAttrs)
                                {
                                    if (!string.IsNullOrEmpty(sAttr))
                                    {
                                        var sAttrVal = sAttr.Split(':')[1];
                                        if (!string.IsNullOrEmpty(sAttrVal) && (sAttrVal.StartsWith("{-") || sAttrVal.StartsWith("{xip|")))
                                        {
                                            sAttrVal = oCache.Get_ParamVal(sSession, sGUID, null, sAttrVal);
                                        }
                                        oParams[sAttr.Split(':')[0]] = sAttrVal;
                                    }
                                }
                            }
                        }
                        else
                        {
                            oParams[sLoadByAttr] = sInstanceID;
                        }
                    }
                    else
                    {
                        Guid InsGUID = Guid.Empty;
                        Guid.TryParse(sInstanceID, out InsGUID);
                        if (InsGUID != null && InsGUID != Guid.Empty)
                        {
                            oParams["xiguid"] = InsGUID;
                        }
                        else
                        {
                            oParams["id"] = sInstanceID;
                        }
                    }
                    if (oParams != null && oParams.Count() > 0)
                    {
                        XIDAttribute oAttrD = new XIDAttribute();
                        if (oBOD.Attributes.ContainsKey(sAttrName))
                        {
                            oAttrD = oBOD.Attributes[sAttrName];
                        }
                        if (oAttrD != null)
                        {
                            if (oAttrD.TypeID == 150)
                            {
                                List<CNV> oWhrParms = new List<CNV>();
                                foreach (var parm in oParams.Keys.ToList())
                                {
                                    oWhrParms.Add(new CNV { sName = parm, sValue = oParams[parm].ToString() });
                                }
                                var oBOI = oXI.BOI(BoTableName, null, null, oWhrParms);
                                if (oBOI != null && oBOI.Attributes.Count() > 0)
                                {
                                    if (oBOI.Attributes.ContainsKey(sAttrName))
                                    {
                                        var dsAttrValue = oBOI.AttributeI(sAttrName).sValue; //sConnection.Select<DateTime>(BoTableName, oParams, sAttrName);
                                        if (!string.IsNullOrEmpty(dsAttrValue))
                                        {
                                            XIIBO oBO = new XIIBO();
                                            var dtValue = oBO.ConvertToDtTime(dsAttrValue.ToString());
                                            sAttrValue = dtValue.ToString("dd-MMM-yyyy");
                                        }
                                    }
                                }

                            }
                            else if (oAttrD.TypeID == 250)
                            {
                                var GUIDVal = sConnection.Select<Guid>(BoTableName, oParams, sAttrName).FirstOrDefault();
                                if (GUIDVal != null && GUIDVal != Guid.Empty)
                                {
                                    sAttrValue = GUIDVal.ToString();
                                }
                            }
                            else
                            {
                                sAttrValue = sConnection.Select<string>(BoTableName, oParams, sAttrName).FirstOrDefault();
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(sAttrValue))
                    {
                        sAttrValue = "0";
                    }
                }
                oCResult.oResult = sAttrValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting attribute from DB using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult GetQSI(List<CNV> Params)
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
                int iInstanceID = 0;
                string sAttrValue = string.Empty;
                //string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSessionID = Params.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = Params.Where(m => m.sName.ToLower() == "sInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (int.TryParse(sInstanceID, out iInstanceID))
                {
                }
                string sGUID = Params.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIIXI oIXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIIQS oQsInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, sInstanceID.ToString());
                if (oQsInstance.QSDefinition == null)
                {
                    oQsInstance = oIXI.GetQuestionSetInstanceByID("0", sInstanceID.ToString(), null, 0, 0, null);
                }
                oCResult.oResult = oQsInstance;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting XIValue from QS Instance using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult GetQSAttributeValue(List<CNV> Params)
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
                int iInstanceID = 0;
                string sAttrValue = string.Empty;
                //string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSessionID = Params.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = Params.Where(m => m.sName.ToLower() == "sInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (int.TryParse(sInstanceID, out iInstanceID))
                {
                }
                string sAttrName = Params.Where(m => m.sName.ToLower() == "sAttrName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sGUID = Params.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                XIIQS oQsInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, iInstanceID.ToString());
                //XIIXI oXII = new XIIXI();
                //XIIQS oQsInstance = oXII.GetQuestionSetInstanceByID(0, iInstanceID, null, 0, 0, null);
                XIDStructure oStructure = new XIDStructure();
                //var oQSNVPairs = oStructure.GetQSNamevaluepairs(oQsInstance);
                //if (oQSNVPairs.xiStatus == 0 && oQSNVPairs.oResult != null)
                //{
                //    Dictionary<string, XIIValue> oXIIList = (Dictionary<string, XIIValue>)oQSNVPairs.oResult;
                //    if (oXIIList[sAttrName] != null)
                //    {
                //        sAttrValue = oXIIList[sAttrName].sValue;
                //    }
                //}
                sAttrValue = oQsInstance.XIIValues(sAttrName);
                oCResult.oResult = sAttrValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting XIValue from QS Instance using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult OM(List<CNV> Params)
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
                string sMethodReference = Params.Where(m => m.sName.ToLower() == "sMethod".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUser = Params.Where(m => m.sName.ToLower() == "sUser".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sNotificationType = Params.Where(m => m.sName.ToLower() == "sNotificationType".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sDoc = Params.Where(m => m.sName.ToLower() == "sDoc".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();

                var sURL = sMethodReference.Split('.')[0];
                var sClass = sMethodReference.Split('.')[1];
                var sMethodName = sMethodReference.Split('.')[2];
                //Creating Instance
                Assembly exceutable;
                Type Ltype;
                object objclass;
                exceutable = Assembly.Load(sURL);
                Ltype = exceutable.GetType(sURL + "." + sClass);
                objclass = Activator.CreateInstance(Ltype);
                if (!string.IsNullOrEmpty(sMethodName))
                {
                    MethodInfo method = Ltype.GetMethod(sMethodName);
                    object oResult = new object();
                    object[] parametersArray = new object[] { sUser, sNotificationType, sDoc };
                    object Response = (object)method.Invoke(objclass, parametersArray);
                    if (((CResult)Response).bOK && ((CResult)Response).oResult != null)
                    {
                        oResult = ((CResult)Response).oResult;
                    }
                    oCResult.oResult = oResult;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error in OM in XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Call_OM(List<CNV> Params)
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
                string sMethodReference = Params.Where(m => m.sName.ToLower() == "sMethod".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //string sUser = Params.Where(m => m.sName.ToLower() == "sUser".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //string sNotificationType = Params.Where(m => m.sName.ToLower() == "sNotificationType".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //string sDoc = Params.Where(m => m.sName.ToLower() == "sDoc".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();

                var sURL = sMethodReference.Split('/')[0];
                var sClass = sMethodReference.Split('/')[1];
                var sMethodName = sMethodReference.Split('/')[2];
                //Creating Instance
                Assembly exceutable;
                Type Ltype;
                object objclass;
                exceutable = Assembly.Load(sURL);
                Ltype = exceutable.GetType(sURL + "." + sClass);
                objclass = Activator.CreateInstance(Ltype);
                if (!string.IsNullOrEmpty(sMethodName))
                {
                    MethodInfo method = Ltype.GetMethod(sMethodName);
                    object oResult = new object();
                    object[] parametersArray = new object[] { Params };
                    object Response = (object)method.Invoke(objclass, parametersArray);
                    if (((CResult)Response).bOK && ((CResult)Response).oResult != null)
                    {
                        oResult = ((CResult)Response).oResult;
                    }
                    oCResult.oResult = oResult;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error in OM in XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult Get_FKToObject(List<CNV> Params)
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
                string sFKAttr = string.Empty;
                string sBO = Params.Where(m => m.sName.ToLower() == "sBO".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sFKBO = Params.Where(m => m.sName.ToLower() == "sFKBO".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBO);
                if(oBOD != null && oBOD.Attributes.Count > 0)
                {
                    int FKiBODID = 0;
                    int.TryParse(sFKBO, out FKiBODID);
                    Guid FKBOGUID = Guid.Empty;
                    Guid.TryParse(sFKBO, out FKBOGUID);
                    if(FKBOGUID != null && FKBOGUID != Guid.Empty)
                    {
                        var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FKBOGUID.ToString());
                        sFKBO = FKBOD.Name;
                    }
                    else if(FKiBODID > 0)
                    {
                        var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FKiBODID.ToString());
                        sFKBO = FKBOD.Name;
                    }
                    var FKAttrs = oBOD.Attributes.Values.ToList().Where(m => m.sFKBOName != null && m.sFKBOName != string.Empty).ToList();
                    if(FKAttrs != null && FKAttrs.Count > 0)
                    {
                        var Attr = FKAttrs.Where(m => m.sFKBOName.ToLower() == sFKBO.ToLower()).FirstOrDefault();
                        if(Attr != null)
                        {
                            sFKAttr = Attr.Name;
                            oCResult.oResult = sFKAttr;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error in OM in XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        public CResult SaveBOAttribute(List<CNV> Params)
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
                string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = Params.Where(m => m.sName.ToLower() == "sInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAttrName = Params.Where(m => m.sName.ToLower() == "sAttrName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAttrValue = Params.Where(m => m.sName.ToLower() == "sAttrValue".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sLoadByAttr = Params.Where(m => m.sName.ToLower() == "sLoadByAttr".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string IsAudit = Params.Where(m => m.sName.ToLower() == "IsAudit".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParamVal = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                string sResult = string.Empty;
                var oResponse = new CResult();
                XIDXI oXID = new XIDXI();
                List<CNV> oWhereParams = new List<CNV>();
                XIIBO oBOI = new XIIBO();
                if (!string.IsNullOrEmpty(sLoadByAttr))
                {
                    //CNV oParams = new CNV();
                    //oParams.sName = sLoadByAttr;
                    //oParams.sValue = sInstanceID;
                    //oWhereParams.Add(oParams);


                    QueryEngine oQE = new QueryEngine();
                    List<XIWhereParams> oWParams = new List<XIWhereParams>();

                    XIIXI oXII = new XIIXI();
                    oWParams.Add(new XIWhereParams { sField = sLoadByAttr, sOperator = "=", sValue = sInstanceID });
                    oQE.AddBO(sBOName, "", oWParams);
                    CResult oCresult1 = oQE.BuildQuery();
                    if (oCresult1.bOK && oCresult1.oResult != null)
                    {
                        var sSql1 = (string)oCresult1.oResult;
                        ExecutionEngine oEE = new ExecutionEngine();
                        oEE.XIDataSource = oQE.XIDataSource;
                        oEE.sSQL = sSql1;
                        var oQResult = oEE.Execute();
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            var oBOIList1 = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                            var oBOD1 = oQE.QParams.FirstOrDefault().BOD;
                            oBOIList1.ForEach(x => x.BOD = oBOD1);
                            if (oBOIList1 != null && oBOIList1.Count > 0)
                            {
                                foreach (var oBOInstance in oBOIList1)
                                {
                                    if (IsAudit.ToLower() == "false")
                                    {
                                        oBOInstance.bIsAudit = false;
                                    }
                                    oBOInstance.SetAttribute(sAttrName, sAttrValue);
                                    oResponse = oBOI.Save(oBOInstance, false);
                                }
                            }
                        }
                    }
                    //oBOI = oXI.BOI(sBOName, null, null, oWhereParams);
                }
                else
                {
                    oBOI = oXI.BOI(sBOName, sInstanceID);
                    if (IsAudit.ToLower() == "false")
                    {
                        oBOI.bIsAudit = false;
                    }
                    if (sAttrValue.ToLower() == "dttoday")
                    {
                        sAttrValue = DateTime.Now.ToString();
                    }
                    oBOI.SetAttribute(sAttrName, sAttrValue);
                    oResponse = oBOI.Save(oBOI, false);
                }
                if (oResponse.bOK && oResponse.oResult != null)
                {
                    oResponse.oResult = "True";
                }
                else
                {
                    oResponse.oResult = "False";
                }
                oCResult.oResult = oResponse.oResult;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while saving BO Attribute using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }

        //public CResult SetBOAttribute(List<CNV> Params)
        //{
        //    CResult oCResult = new CResult(); // always
        //    CResult oCR = new CResult(); // always
        //    long iTraceLevel = 10;

        //    //get iTraceLevel from ??somewhere fast - cache against user??

        //    oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
        //    oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

        //    if (iTraceLevel > 0)
        //    {
        //        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
        //    }
        //    //if iTraceLevel>0 then 
        //    //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
        //    //oCResult.oTraceStack.Trace("Stage",sError)
        //    //end if

        //    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
        //    {
        //        oCResult.xiStatus = oCR.xiStatus;
        //        //oCResult.oTraceStack.Trace("Stage",sError)
        //    }
        //    //in the case of
        //    //xiEnumSystem.xiFuncResult.xiLogicalError
        //    oCResult.sMessage = "someone tried to do something they shouldnt";

        //    //tracing
        //    // if tracing is on (which is a config switch for this user) then
        //    //   oCResult.Trace.Write
        //    //ALL OK?
        //    try
        //    {
        //        string sName = Params.Where(m => m.sName.ToLower() == "sName".ToLower()).Select(m => m.sValue).FirstOrDefault();
        //        string sValue = Params.Where(m => m.sName.ToLower() == "sValue".ToLower()).Select(m => m.sValue).FirstOrDefault();
        //        string sGUID = Params.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
        //        string sParamVal = string.Empty;
        //        XIInfraCache oCache = new XIInfraCache();
        //        XIIXI oXI = new XIIXI();
        //        string sResult = string.Empty;
        //        List<CNV> oNVList = new List<CNV>();
        //        CNV oParam = new CNV();
        //        oParam.sName = sName;
        //        oParam.sValue = sValue;
        //        oNVList.Add(oParam);
        //        oCache.SetXIParams(oNVList, sGUID);
        //        //oCResult.oResult = oResponse.oResult;
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

        //    }
        //    catch (Exception ex)
        //    {
        //        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Setting BO Attribute" });
        //        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //        oCResult.LogToFile();
        //        SaveErrortoDB(oCResult);
        //    }
        //    return oCResult; // always
        //}

        public string GetValue(string sTableName, string sParamName, List<XIWhereParams> sWhereParamName, string sBo = "")
        {
            CResult oCResult = new CResult();
            string sValue = "";
            try
            {
                XID1Click oD1Click = new XID1Click();
                if (!string.IsNullOrEmpty(sTableName) && !string.IsNullOrEmpty(sParamName))
                {
                    var sQuery = "Select " + sParamName + " from " + sTableName;
                    List<string> whereParam = new List<string>();
                    foreach (var WHParam in sWhereParamName)
                    {
                        whereParam.Add(WHParam.sField + WHParam.sOperator + "'" + WHParam.sValue + "'");
                    }
                    if (whereParam.Count > 0)
                    {
                        var Condition = string.Join(" and ", whereParam.ToList());
                        sQuery += " Where " + Condition;
                    }
                    oD1Click.Query = sQuery;
                    if (!string.IsNullOrEmpty(sBo))
                    {
                        oD1Click.Name = sBo;
                    }
                    else
                    {
                        oD1Click.Name = sTableName;
                    }
                    var result = oD1Click.OneClick_Run(false);
                    foreach (var item in result.Values)
                    {
                        sValue = item.Attributes[sParamName].sValue;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting BO Attribute value by executing Query using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return sValue;
        }
        public double GetMonthlyPremiumAmount(double rPremium, double MinimumDeposit, int iProductID, double rAddonAdmin, double rAddon, int iPFSchemeID = 0)
        {
            CResult oCResult = new CResult();
            double rMonthlyPremium = 0.0;
            try
            {
                double zGrossRatePC = 0;
                XIIXI oXII = new XIIXI();
                if (iPFSchemeID > 0)
                {
                    var oPFSchemeI = oXII.BOI("PFScheme_T", iPFSchemeID.ToString());
                    if (oPFSchemeI != null && oPFSchemeI.Attributes != null && oPFSchemeI.Attributes.ContainsKey("zGrossRatePC"))
                    {
                        if (double.TryParse(oPFSchemeI.Attributes["zGrossRatePC"].sValue, out zGrossRatePC)) { }
                    }
                }
                else
                {
                    XID1Click oD1Click = new XID1Click();
                    string sQuery = "select sDefaultValue from PFBNPFields_T WHERE sDefaultValue is not null AND sServiceName='NEWBUSINESS' AND sXIFieldName='CreditProductCodePF'";
                    oD1Click.Query = sQuery;
                    oD1Click.Name = "PFBNPFields_T";
                    var oresult = oD1Click.OneClick_Run(false);
                    foreach (var result in oresult)
                    {
                        if (result.Value.Attributes != null && result.Value.Attributes.ContainsKey("sDefaultValue"))
                        {
                            string ProductCode = result.Value.Attributes["sDefaultValue"].sValue;
                            if (!string.IsNullOrEmpty(ProductCode))
                            {
                                string Query = "select zGrossRatePC from PFScheme_T where sSchemeRefCode=" + "'" + ProductCode + "'" + " and iType = 0";
                                oD1Click.Query = Query;
                                oD1Click.Name = "PFScheme_T";
                                var oresult1 = oD1Click.OneClick_Run(false);
                                string GrossRate = "";
                                foreach (var item in oresult1.Values)
                                {
                                    GrossRate = item.Attributes["zGrossRatePC"].sValue;
                                }
                                if (double.TryParse(GrossRate, out zGrossRatePC)) { }
                            }
                        }
                    }
                }
                if (zGrossRatePC <= 0)
                {
                    zGrossRatePC = 19.25;
                }
                rPremium += rAddonAdmin + rAddon;
                double rTotalFinance = rPremium - MinimumDeposit;
                double rIntrestAmount = rTotalFinance * zGrossRatePC / 100;
                double rTotalFinancePayable = rTotalFinance + rIntrestAmount;
                rMonthlyPremium = rTotalFinancePayable / 10;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting monthly premium amount using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return rMonthlyPremium;
        }
        public string GetMinimumDepostAmount(double rPaymentCharge, double rInsurerCharge, double rFinalQuote, double rAdminCharge, string sGUID, string QSIID, int iProductID, double rAddonAdmin, double rAddon)
        {
            CResult oCResult = new CResult();
            string obj = string.Empty;
            try
            {
                XIIXI oXIIXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                var oProductI = oXIIXI.BOI("Product", iProductID.ToString());
                oProductI.Attributes["iDefaultAmountType"].BOD = oProductI.BOD;
                double rTotal = 0;
                rTotal += rAddon + rAddonAdmin;

                double rDefaultDepositP = 0;
                double rPayableAmount = 0;
                XIIBO oBOI2 = new XIIBO();
                oBOI2.iBODID = oProductI.iBODID;
                oProductI.Attributes["iMinPaymentCalType"].BOI = oBOI2;
                if (oProductI.AttributeI("iDefaultAmountType").sResolvedValue == "Percentage")
                {
                    if (double.TryParse(oProductI.Attributes["zDefaultDeposit"].sValue, out rDefaultDepositP)) { }
                    if (oProductI.AttributeI("iMinPaymentCalType").sResolvedValue == "FullPremium")
                    {
                        rTotal += rFinalQuote;
                        rPayableAmount = rDefaultDepositP * 0.01 * rTotal;
                    }
                    else
                    {
                        rFinalQuote -= rPaymentCharge + rInsurerCharge + rAdminCharge;
                        rTotal += rPaymentCharge + rInsurerCharge + rAdminCharge;
                        rPayableAmount = rTotal + (rFinalQuote * 0.01 * rDefaultDepositP);
                    }
                }
                else
                {
                    if (double.TryParse(oProductI.Attributes["rDefaultAmount"].sValue, out rPayableAmount)) { }
                }
                obj = rPayableAmount.ToString("F");
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting minimum desposit amount using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return obj;
        }
        public CResult GetCount(List<CNV> Params)
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
                string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInstanceID = Params.Where(m => m.sName.ToLower() == "sInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAttrName = Params.Where(m => m.sName.ToLower() == "sAttrName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var count = 0;
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                List<SqlParameter> SqlParams = new List<SqlParameter>();
                XIIXI oXII = new XIIXI();
                if (string.IsNullOrEmpty(sAttrName))
                {
                    sAttrName = "id";
                }
                oWParams.Add(new XIWhereParams { sField = sAttrName, sOperator = "=", sValue = sInstanceID });
                SqlParams.Add(new SqlParameter { ParameterName = "@" + sAttrName, Value = sInstanceID });
                oWParams.Add(new XIWhereParams { sField = XIConstant.Key_XIDeleted, sOperator = "=", sValue = "0" });
                SqlParams.Add(new SqlParameter { ParameterName = "@" + XIConstant.Key_XIDeleted, Value = "0" });
                oQE.AddBO(sBOName, "", oWParams);
                CResult oCresult1 = oQE.BuildQuery();
                if (oCresult1.bOK && oCresult1.oResult != null)
                {
                    var sSql1 = (string)oCresult1.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql1;
                    oEE.SqlParams = SqlParams;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        var oBOIList1 = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        if (oBOIList1 != null)
                        {
                            count = oBOIList1.Count;
                        }
                    }
                }
                oCResult.oResult = count;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting count of records using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        public CResult InsertBO(List<CNV> Params)
        {
            CResult oCResult = new CResult();
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
            try
            {
                string sBOName = Params.Where(m => m.sName.ToLower() == "sBOName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var oAttrList = Params.Where(m => m.sType.ToLower() == "Attribute".ToLower()).ToList();
                XIIBO oBOI = new XIIBO();
                XIInfraCache oCache = new XIInfraCache();
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                oBOI.BOD = oBOD;
                foreach (var oAttr in oAttrList)
                {
                    oBOI.SetAttribute(oAttr.sName, oAttr.sValue);
                }
                oBOI.Save(oBOI, false);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while saving BOI using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult GetSettingConfig(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
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
            try
            {
                string sSettingName = oParams.Where(m => m.sName.ToLower() == "sSettingName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sSecurity = oParams.Where(m => m.sType.ToLower() == "sSecurity".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSessionID = oParams.Where(m => m.sName.ToLower() == "sSessionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIInfraCache oCache = new XIInfraCache();
                string sApplicationID = oCache.Get_ParamVal(sSessionID, sGUID, null, "-iApplicationID");
                string sOrgID = oCache.Get_ParamVal(sSessionID, sGUID, null, "-iOrgID");
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                List<SqlParameter> oSQLParams = new List<SqlParameter>();
                string sResult = string.Empty;
                XIIXI oXII = new XIIXI();
                if (!string.IsNullOrEmpty(sApplicationID))
                {
                    oWParams.Add(new XIWhereParams { sField = "FkiApplicationID", sOperator = "=", sValue = sApplicationID });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@FkiApplicationID", Value = sApplicationID });
                }
                if (!string.IsNullOrEmpty(sOrgID))
                {
                    oWParams.Add(new XIWhereParams { sField = "FkiOrgID", sOperator = "=", sValue = sOrgID });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@FkiOrgID", Value = sOrgID });
                }
                if (!string.IsNullOrEmpty(sSettingName))
                {
                    oWParams.Add(new XIWhereParams { sField = "sSettingName", sOperator = "=", sValue = sSettingName });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@sSettingName", Value = sSettingName });
                }
                if (!string.IsNullOrEmpty(sSecurity))
                {
                    oWParams.Add(new XIWhereParams { sField = "sSecurityGUID", sOperator = "=", sValue = sSecurity });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@sSecurityGUID", Value = sSecurity });
                }
                var AppEnvironment = ConfigurationManager.AppSettings["AppEnvironment"];
                if (!string.IsNullOrEmpty(AppEnvironment))
                {
                    oWParams.Add(new XIWhereParams { sField = "sEnvironment", sOperator = "=", sValue = AppEnvironment });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@sEnvironment", Value = AppEnvironment });
                }
                oQE.AddBO("XINVSetting_T", "", oWParams);
                CResult oCresult1 = oQE.BuildQuery();
                if (oCresult1.bOK && oCresult1.oResult != null)
                {
                    var sSql1 = (string)oCresult1.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql1;
                    oEE.SqlParams = oSQLParams;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        var oBOIList1 = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.FirstOrDefault();
                        if (oBOIList1 != null && oBOIList1.Attributes.ContainsKey("sSettingValue"))
                        {
                            sResult = oBOIList1.Attributes["sSettingValue"].sValue;
                            if (oBOIList1.Attributes.ContainsKey("bIsEncrypt") && oBOIList1.Attributes["bIsEncrypt"].bValue == true)
                            {
                                XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                                sResult = xifEncrypt.DecryptData(sResult, true, oBOIList1.Attributes["id"].sValue);
                            }
                        }
                    }
                }
                oCResult.oResult = sResult;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while saving BOI using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult Insert_QSI(List<CNV> oParams)
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
                XIDQS oQSD = new XIDQS();
                XIIXI oXII = new XIIXI();
                string sGUID = Guid.NewGuid().ToString();
                int iQSDID = 0;
                var sQSDID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_QSDID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sQSName = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_QSName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sQSDID))
                {
                    int.TryParse(sQSDID, out iQSDID);
                    oTrace.oParams.Add(new CNV { sName = "iQSDID", sValue = iQSDID.ToString() });
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString());
                }
                else if (!string.IsNullOrEmpty(sQSName))
                {
                    oTrace.oParams.Add(new CNV { sName = "sQSName", sValue = sQSName });
                    oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, sQSName);
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
                }
                if (oQSD != null && oQSD.ID > 0)
                {
                    oCR = oXII.CreateQSI(null, oQSD.ID.ToString(), null, null, 0, 0, "");
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var oQSInstance = (XIIQS)oCR.oResult;
                        var iActiveStepID = oQSD.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault().XIGUID;
                        oQSInstance.QSDefinition = oQSD;
                        oQSInstance.iCurrentStepIDXIGUID = iActiveStepID;
                        oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepID.ToString(), sGUID);
                        oQSInstance = oQSInstance.Save(oQSInstance, "");
                        oCResult.oResult = oQSInstance.ID;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
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
        public CResult InsertRuleGroup(List<CNV> Params)
        {
            CResult oCResult = new CResult();
            int iRuleID = 0;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XI Rule Groups");
                int.TryParse(Params.Where(t => t.sName == "iInstanceID").Select(f => f.sValue).FirstOrDefault(), out iRuleID);
                if (iRuleID > 0)
                {
                    XIIXI oXII = new XIIXI();
                    List<CNV> oNV = new List<CNV>();
                    oNV.Add(new CNV { sName = "FkiRuleID", sValue = iRuleID.ToString() });
                    oNV.Add(new CNV { sName = "izxDeleted", sValue = "0" });
                    var RuleGroupI = oXII.BOI("XI Rule Groups", "", "", oNV);
                    if (RuleGroupI == null)
                    {
                        var RuleI = oXII.BOI("XI Rules", iRuleID.ToString());
                        if (RuleI != null && RuleI.Attributes.Count > 0)
                        {
                            RuleGroupI = new XIIBO();
                            RuleGroupI.BOD = oBOD;
                            RuleGroupI.Attributes["FKiRuleID"] = new XIIAttribute { sName = "FKiRuleID", sValue = iRuleID.ToString(), bDirty = true };
                            RuleGroupI.Attributes["FKiRuleIDXIGUID"] = new XIIAttribute { sName = "FKiRuleIDXIGUID", sValue = RuleI.Attributes["XIGUID"].sValue, bDirty = true };
                            RuleGroupI.Attributes["iParentRootID"] = new XIIAttribute { sName = "iParentRootID", sValue = "0", bDirty = true };
                            RuleGroupI.Attributes["iRuleNo"] = new XIIAttribute { sName = "iRuleNo", sValue = "1", bDirty = true };
                            RuleGroupI.Attributes["sRuleOperator"] = new XIIAttribute { sName = "sRuleOperator", sValue = RuleI.Attributes["sDefaultNode"].sValue, bDirty = true };
                            RuleGroupI.Attributes["izxDeleted"] = new XIIAttribute { sName = "izxDeleted", sValue = "0", bDirty = true };
                            RuleGroupI.Attributes["BOIDXIGUID"] = new XIIAttribute { sName = "BOIDXIGUID", sValue = RuleI.Attributes["BOIDXIGUID"].sValue, bDirty = true };
                            RuleGroupI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = "", bDirty = true };
                            RuleGroupI.Attributes["iGroupID"] = new XIIAttribute { sName = "iGroupID", sValue = "0", bDirty = true };
                            var oResult = RuleGroupI.Save(RuleGroupI);

                            if (oResult.bOK && oResult.oResult != null)
                            {
                                var oRule = (XIIBO)oResult.oResult;
                                var RuleGID = oRule.Attributes.Values.Where(m => m.sName.ToLower() == "id").Select(m => m.sValue).FirstOrDefault();
                                RuleGroupI.Attributes["iGroupID"].sValue = RuleGID;
                                RuleGroupI.Attributes["iGroupID"].bDirty = true;
                                RuleGroupI.Attributes["ID"].sValue = RuleGID;
                                RuleGroupI.Attributes["ID"].bDirty = true;
                                RuleGroupI.Save(RuleGroupI);
                                //var esfd = XiLinkRepository.RemoveXilinkID(ExistXilinkID);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: [Rule:" + iRuleID + "" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] " + ex.Message.ToString() + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult MatchString(List<CNV> Params)
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
                bool bMatch = false;
                string sStringToMatch = Params.Where(m => m.sName.ToLower() == "sStringToMatch".ToLower()).Select(m => m.sValue).FirstOrDefault();//eg:EmailSubject
                string sMatchSearchFor = Params.Where(m => m.sName.ToLower() == "sMatchSearchFor".ToLower()).Select(m => m.sValue).FirstOrDefault();//eg:ravi@systemsdna.com or l2m@xyz.com
                //string sRegex = Params.Where(m => m.sName.ToLower() == "sRegex".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //if (string.IsNullOrEmpty(sRegex))
                //{
                //    sRegex = "";
                //}
                string sRegexCode = sMatchSearchFor;//@"(?<=\{)[^}]*(?=\})";@".*\@systemsdna.com$"

                Regex regex = new Regex(sRegexCode, RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(sStringToMatch);
                if (matches.Count > 0)
                {
                    bMatch = true;
                }
                oCResult.oResult = bMatch;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while getting parameter from cache using XIAPI" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }
    }
}