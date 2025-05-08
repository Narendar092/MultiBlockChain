using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIInfraAccountComponent : XIDefinitionBase
    {
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sVisualisation { get; set; }
        public string sUserName { get; set; }

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
                sVisualisation = oParams.Where(m => m.sName == "Visualisation").Select(m => m.sValue).FirstOrDefault();
                sUserName = oParams.Where(m => m.sName == "UserName").Select(m => m.sValue).FirstOrDefault();
                if (sUserName != null && (sUserName.StartsWith("{XIP|") || sUserName.StartsWith("-") || sUserName.StartsWith("{-")))
                {
                    sUserName = oCache.Get_ParamVal(sSessionID, sGUID, null, sUserName);
                }
                var Type = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|sLoginType}");
                var IsEmail = oParams.Where(m => m.sName == "IsEmail").Select(m => m.sValue).FirstOrDefault();
                var IsSMS = oParams.Where(m => m.sName == "IsSMS").Select(m => m.sValue).FirstOrDefault();
                var OTPType = oParams.Where(m => m.sName == "OTPType").Select(m => m.sValue).FirstOrDefault();
                var OTPCase = oParams.Where(m => m.sName == "OTPCase").Select(m => m.sValue).FirstOrDefault();
                var OTPLength = oParams.Where(m => m.sName == "OTPLength").Select(m => m.sValue).FirstOrDefault();
                var IsTwoWayAuthentication = oParams.Where(m => m.sName == "IsTwoWayAuthentication").Select(m => m.sValue).FirstOrDefault();
                var sLoginType = oParams.Where(m => m.sName == "LoginType").Select(m => m.sValue).FirstOrDefault();
                var IsRegistration = oParams.Where(m => m.sName == "IsRegistration").Select(m => m.sValue).FirstOrDefault();
                var IsForgotPassword = oParams.Where(m => m.sName == "IsForgotPassword").Select(m => m.sValue).FirstOrDefault();
                var sCallHierarchy = oParams.Where(m => m.sName == XIConstant.Param_CallHierarchy).Select(m => m.sValue).FirstOrDefault();
                var sCoreDB = oParams.Where(m => m.sName.ToLower() == "sDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iUserID = 0;
                int.TryParse(sUserID, out iUserID);
                int iPageCount = 0;
                var FKiVisualisationID = 0;
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
                //if (WrapperParms != null && WrapperParms.Count() > 0)
                //{
                //    string sOneClickID = WrapperParms.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault();
                //    if (int.TryParse(sOneClickID, out OneClickID))
                //    {

                //    }
                //    else
                //    {
                //        //IDE
                //        var s1ClickID = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.XIP_1ClickID);
                //        int.TryParse(s1ClickID, out OneClickID);

                //        var sParentFKColumn = oCache.Get_ParamVal(sSessionID, sGUID, null, "ParentFKColumn");

                //        var iParentInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, "ParentInsID");

                //        if (!string.IsNullOrEmpty(sParentFKColumn) && iParentInsID != "0")
                //        {
                //            sCondition = sParentFKColumn + "=" + iParentInsID;
                //        }
                //    }

                //    //IDE
                //    var sFKiVisualisationID = oCache.Get_ParamVal(sSessionID, sGUID, null, XIConstant.Param_FKiVisualisationID);
                //    int.TryParse(sFKiVisualisationID, out FKiVisualisationID);
                //}
                //else
                //{
                //    var s1ClickID = oParams.Where(m => m.sName.ToLower() == "1ClickID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //    int.TryParse(s1ClickID, out OneClickID);
                //}
                List<XIVisualisation> oVisualL = new List<XIVisualisation>();
                List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                oNV.Add(new XIVisualisationNV { sName = "IsTwoWayAuthentication", sValue = IsTwoWayAuthentication });
                oNV.Add(new XIVisualisationNV { sName = "IsSMS", sValue = IsSMS });
                oNV.Add(new XIVisualisationNV { sName = "IsEmail", sValue = IsEmail });
                oNV.Add(new XIVisualisationNV { sName = "OTPCase", sValue = OTPCase });
                oNV.Add(new XIVisualisationNV { sName = "OTPLength", sValue = OTPLength });
                oNV.Add(new XIVisualisationNV { sName = "OTPType", sValue = OTPType });
                oNV.Add(new XIVisualisationNV { sName = "LoginType", sValue = sLoginType });
                oNV.Add(new XIVisualisationNV { sName = "IsRegistration", sValue = IsRegistration });
                oNV.Add(new XIVisualisationNV { sName = "IsForgotPassword", sValue = IsForgotPassword });
                if (!string.IsNullOrEmpty(sUserName))
                {
                    XIInfraUsers oUserD = new XIInfraUsers();
                    oUserD.sUserName = sUserName;
                    var UserDetails = oUserD.Get_UserDetails(sCoreDB);
                    if (UserDetails.xiStatus == 0 && UserDetails.oResult != null)
                    {
                        // If User exist
                        oUserD = (XIInfraUsers)UserDetails.oResult;
                        if (oUserD.UserID != iUserID)
                        {
                            if (Type == "FP")
                            {
                                XIInfraEmail oEmail = new XIInfraEmail();
                                XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                                string sTemporaryPWD = RandomNumber(7)/*random.Next(1, 100000000).ToString(new String('0', 7))*/;
                                var EncryptedPwd = xifEncrypt.EncryptData(sTemporaryPWD, true, oUserD.UserID.ToString());
                                oUserD.sTemporaryPasswordHash = EncryptedPwd;
                                oUserD.UpdatedTime = DateTime.Now;
                                oUserD.dtLastLogin = DateTime.Now;
                                var oUserData = oUserD.Update_User(sCoreDB);
                                XIContentEditors oDocumentContent = new XIContentEditors();
                                XIDXI oXIDXI = new XIDXI();
                                var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "38");
                                if (oContentDef != null && oContentDef.Count() > 0)
                                {
                                    XIIBO oBOI = new XIIBO();
                                    string sBOName = "XIAPPUsers";
                                    if (sBOName != null)
                                    {
                                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                                    }
                                    XIBOInstance oBOIInstance = new XIBOInstance();
                                    oBOIInstance.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                    oBOI.SetAttribute("stemppassword", sTemporaryPWD);
                                    oBOI.SetAttribute("sname", oUserD.sFirstName + " " + oUserD.sLastName);
                                    List<XIIBO> oBOIList = new List<XIIBO>();
                                    //oBOI.Attributes = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => x.Value);
                                    oBOI.XIIValues = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => new XIIValue { sValue = x.Value.sValue });
                                    oBOIList.Add(oBOI);
                                    oBOIInstance.oStructureInstance["User"] = oBOIList;
                                    oDocumentContent = oContentDef.FirstOrDefault();
                                    //Get Document Template htmlcontent with dynamic data
                                    XIContentEditors oConent = new XIContentEditors();
                                    oConent.sSessionID = "";
                                    //var oRes = oConent.MergeTemplateContent(oDocumentContent, oBOIInstance);
                                    var oRes = oConent.MergeContentTemplate(oDocumentContent, oBOIInstance);
                                    if (!oRes.bOK)
                                    {
                                        return null;
                                    }
                                    string sContent = (string)oRes.oResult;
                                    oEmail.EmailID = sUserName;
                                    oEmail.sSubject = oDocumentContent.sSubject;
                                    oEmail.Bcc = oDocumentContent.sBCC;
                                    oEmail.cc = oDocumentContent.sCC;
                                    oEmail.From = oDocumentContent.sFrom;
                                    oEmail.iServerID = oDocumentContent.FkiServerID;
                                    oEmail.Sendmail(0, sContent, null, 0, XIConstant.Email_ForgotPassword, 0, null, 0, oDocumentContent.bIsBCCOnly);//send mail with attachment
                                    oCResult.sMessage = "Mail send successfully";
                                    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully" });
                                    XIIBO oBO = new XIIBO();
                                    oBO.BOD = oBOI.BOD;
                                    oBO.SetAttribute("sUserName", sUserName);
                                    //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                                    oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "FP" });
                                    oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                                    oBO.oVisualisation = oVisualL;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                                    oCResult.oResult = oBO;
                                    //oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                                    //}
                                }
                            }
                            else if (Type == "RP")
                            {
                                XIIBO oBO = new XIIBO();
                                string sBOName = "XIAPPUsers";
                                if (sBOName != null)
                                {
                                    oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                                }
                                oBO.SetAttribute("sUserName", oUserD.sUserName);
                                //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                                oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "RP" });
                                oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                                oBO.oVisualisation = oVisualL;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                                oCResult.oResult = oBO;
                            }
                            else if (Type == "Authentication")
                            {
                                XIIBO oBO = new XIIBO();
                                string sBOName = "XIAPPUsers";
                                if (sBOName != null)
                                {
                                    oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                                }
                                oBO.SetAttribute("sUserName", oUserD.sUserName);
                                oBO.SetAttribute("UserID", oUserD.UserID.ToString());
                                oBO.SetAttribute("UserLogLevel", oUserD.iLogLevel.ToString());
                                //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                                oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Authentication" });
                                oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                                oBO.oVisualisation = oVisualL;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                                oCResult.oResult = oBO;
                            }
                            else
                            {
                                XIIBO oBO = new XIIBO();
                                string sBOName = "XIAPPUsers";
                                if (sBOName != null)
                                {
                                    oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                                }
                                oBO.SetAttribute("sUserName", oUserD.sUserName);
                                //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                                oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Login" });
                                oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                                oBO.oVisualisation = oVisualL;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                                oCResult.oResult = oBO;
                                // login
                                //oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                            }

                        }
                        else
                        {
                            //User 
                            XIIBO oBO = new XIIBO();
                            string sBOName = "XIAPPUsers";
                            if (sBOName != null)
                            {
                                oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                            }
                            oBO.SetAttribute("sUserName", oUserD.sUserName);
                            //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                            oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Login" });
                            //oNV.Add(new XIVisualisationNV { sName = "IsTwoWayAuthentication", sValue = IsTwoWayAuthentication });
                            //oNV.Add(new XIVisualisationNV { sName = "IsSMS", sValue = IsSMS });
                            //oNV.Add(new XIVisualisationNV { sName = "IsEmail", sValue = IsEmail });
                            //oNV.Add(new XIVisualisationNV { sName = "OTPCase", sValue = OTPCase });
                            //oNV.Add(new XIVisualisationNV { sName = "OTPLength", sValue = OTPLength });
                            //oNV.Add(new XIVisualisationNV { sName = "OTPType", sValue = OTPType });
                            //oNV.Add(new XIVisualisationNV { sName = "LoginType", sValue = sLoginType });
                            oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                            oBO.oVisualisation = oVisualL;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                            oCResult.oResult = oBO;
                            // login
                            //oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        }
                    }
                    else
                    {
                        //Registration 
                        XIIBO oBO = new XIIBO();
                        //string sBOName = "XIAPPUsers";
                        //if (sBOName != null)
                        //{
                        //    oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                        //}
                        //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                        oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Registration" });
                        //oNV.Add(new XIVisualisationNV { sName = "LoginType", sValue = sLoginType });
                        oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                        oBO.oVisualisation = oVisualL;
                        oBO.SetAttribute("sUserName", oUserD.sUserName);
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                        oCResult.oResult = oBO;
                        //InsertUser(oParams);
                        //oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sAdminSuccessStep, null, null);
                    }
                }
                else if (Type == "Registration")
                {
                    XIIBO oBO = new XIIBO();
                    oBO.SetAttribute("sUserName", "");
                    //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                    oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Registration" });
                    oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                    oBO.oVisualisation = oVisualL;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                    oCResult.oResult = oBO;
                }
                else
                {

                    XIIBO oBO = new XIIBO();
                    //string sBOName = "XIAPPUsers";
                    //if (sBOName != null)
                    //{
                    //    oBO.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                    //}
                    //List<XIVisualisationNV> oNV = new List<XIVisualisationNV>();
                    oNV.Add(new XIVisualisationNV { sName = "AcType", sValue = "Login" });
                    //oNV.Add(new XIVisualisationNV { sName = "IsTwoWayAuthentication", sValue = IsTwoWayAuthentication });
                    //oNV.Add(new XIVisualisationNV { sName = "IsSMS", sValue = IsSMS });
                    //oNV.Add(new XIVisualisationNV { sName = "IsEmail", sValue = IsEmail });
                    //oNV.Add(new XIVisualisationNV { sName = "OTPCase", sValue = OTPCase });
                    //oNV.Add(new XIVisualisationNV { sName = "OTPLength", sValue = OTPLength });
                    //oNV.Add(new XIVisualisationNV { sName = "OTPType", sValue = OTPType });
                    //oNV.Add(new XIVisualisationNV { sName = "LoginType", sValue = sLoginType });
                    oVisualL.Add(new XIVisualisation { Name = "AcType", XiVisualisationNVs = oNV });
                    oBO.oVisualisation = oVisualL;
                    oBO.SetAttribute("sUserName", "");
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess; ;
                    oCResult.oResult = oBO;
                    oCR.sMessage = "Config Error: XIInfraAccountComponent_XILoad() : 1-Click ID is not passed as parameter - Call Hierarchy: " + sCallHierarchy;
                    oCR.sCode = "Config Error";
                    SaveErrortoDB(oCR);
                }
                oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sLoginType}", "", null, null);
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
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}