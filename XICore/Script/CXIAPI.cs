using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;
using XIDataBase.Hubs;
using Microsoft.AspNet.SignalR;


namespace XICore
{
    public class CXIAPI
    {

        public CResult LoadBO(string sBODef, string iInstID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult Execute_1Click(long i1ClickID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult LoadBOFK(string sBODef, string iInstID, string sFKAttr)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult Attr(string sBODef, string iInstID, string sAttrName, string sAttrValue = "", string sLoadByAttr = "", string IsAudit = "", string sSession = "", string sGUID = "")
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            try
            {
                // get the attr value
                //CDataLoad oMyData = new CDataLoad();
                XIAPI xiAPI = new XIAPI();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSession });
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sBOName", sValue = sBODef });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sAttrName", sValue = sAttrName });
                Params.Add(new CNV() { sName = "sLoadByAttr", sValue = sLoadByAttr });
                Params.Add(new CNV() { sName = "IsAudit", sValue = IsAudit });


                if (string.IsNullOrEmpty(sAttrValue))
                {
                    oMyResult = (CResult)xiAPI.GetBOAttribute(Params); //((CResult)(oMyData.API_Load("GetBOAttribute", Params)));
                }
                else
                {
                    Params.Add(new CNV() { sName = "sAttrValue", sValue = sAttrValue });
                    oMyResult = (CResult)xiAPI.SaveBOAttribute(Params); //((CResult)(oMyData.API_Load("SaveBOAttribute", Params)));
                }
                //  If sLoadByAttr="" then load by PK
                // TEMP
                // oMyResult.oResult = "1234"
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        // API
        public CResult API(string sConcatRequest)
        {
            CResult oMyResult = new CResult();
            try
            {
                // TO DO - split the concat request, it should be in square brackets
                // TEMP!!!!!!!!!!!!!
                if (sConcatRequest == "logintype")
                {
                    oMyResult.oResult = "private";
                }
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        // Reserved
        public CResult Reserved(string sReservedWord)
        {
            CResult oMyResult = new CResult();
            try
            {
                //CDataLoad oMyData = new CDataLoad();
                XIAPI xiAPI = new XIAPI();
                switch (sReservedWord)
                {
                    case "currentuser":
                        oMyResult = (CResult)xiAPI.GetUserRole(); //((CResult)(oMyData.API_Load("GetUserRole")));
                        if (oMyResult.oResult == null)
                        {
                            // oMyResult.oResult = ""
                        }

                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentuserid":
                        oMyResult = (CResult)xiAPI.GetUserID(); //((CResult)(oMyData.API_Load("GetUserID")));
                        if (oMyResult.oResult == null)
                        {
                            // oMyResult.oResult = ""
                        }
                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentyear":
                        oMyResult.oResult = DateTime.Now.Year;
                        if (oMyResult.oResult == null)
                        {
                            // oMyResult.oResult = ""
                        }
                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentdate":
                        oMyResult.oResult = DateTime.Now.Date.ToString();
                        if (oMyResult.oResult == null)
                        {
                            // oMyResult.oResult = ""
                        }
                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentdatetime":
                        oMyResult.oResult = DateTime.Now.ToString();                        
                        break;
                }
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult AttrFormatted(string sBODef, string iInstID, string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult AttrFK(string sBODef, string iInstID, string sFKAttrName, string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult UserAttribute(string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult UserRole(string sRoleName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // in this role or not?
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        // 
        public CResult QuestionSetFieldValue(string sStepID, string sFieldID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // ASSUMES CURRENT QS INSTANCE, but should we allow any QS?
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult Parameter(string sParamName, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sParamName", sValue = sParamName });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                oMyResult = (CResult)xiAPI.Parameter(Params);  //((CResult)(oMyData.API_Load("Parameter", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult Set_Parameter(string sParamName, string sParamValue, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sParamName", sValue = sParamName });
                Params.Add(new CNV() { sName = "sParamValue", sValue = sParamValue });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                oMyResult = (CResult)xiAPI.Set_Parameter(Params);  //((CResult)(oMyData.API_Load("Parameter", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        //QS Instance
        public CResult QSI(string iInstID, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                //         Params.Add(New CNV With {
                //     .sName = "sBOName",
                //     .sValue = sBODef
                // })
                oMyResult = (CResult)xiAPI.GetQSI(Params); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        // QSXIValue
        public CResult QSXIValue(string sParamName, string iInstID, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sAttrName", sValue = sParamName });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                //         Params.Add(New CNV With {
                //     .sName = "sBOName",
                //     .sValue = sBODef
                // })
                oMyResult = (CResult)xiAPI.GetQSAttributeValue(Params); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult QSStepXIValue(string sStepName, string sParamName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // GET FROM Questionset - named step
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        //QS Instance Insertiong
        public CResult Insert_QSI(List<CNV> oParams)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                oMyResult = (CResult)xiAPI.Insert_QSI(oParams); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        //Copy Structure
        public CResult Copy_Structure(List<CNV> oParams)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                oMyResult = (CResult)xiAPI.Call_OM(oParams); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        //Call OM
        public CResult Call_OM(List<CNV> oParams)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                oMyResult = (CResult)xiAPI.Call_OM(oParams); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }


        public CResult Get_FKToObject(List<CNV> oParams)
        {
            CResult oMyResult = new CResult();
            try
            {
                XIAPI xiAPI = new XIAPI();
                oMyResult = (CResult)xiAPI.Get_FKToObject(oParams); //((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }


        public CResult OM(string sMethod = "", string sUser = "", string sNotificationType = "", string sDoc = "")
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            try
            {
                XIAPI xiAPI = new XIAPI();
                // get the attr value
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sMethod", sValue = sMethod });
                Params.Add(new CNV() { sName = "sUser", sValue = sUser });
                Params.Add(new CNV() { sName = "sNotificationType", sValue = sNotificationType });
                Params.Add(new CNV() { sName = "sDoc", sValue = sDoc });
                oMyResult = (CResult)xiAPI.OM(Params); //((CResult)(oMyData.API_Load("OM", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }

            return oMyResult;
        }
        public CResult Count(string sBODef, string iInstID, string sAttrName = "")
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            try
            {
                // get count
                //CDataLoad oMyData = new CDataLoad();
                XIAPI xiAPI = new XIAPI();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sBOName", sValue = sBODef });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sAttrName", sValue = sAttrName });
                oMyResult = (CResult)xiAPI.GetCount(Params);
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult InsertBO(string sMethodName, string sBO, string sInstanceID, string sGroup, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oMyResult.sClassName = oMyResult.Get_Class(); //AUTO-DERIVE
            oMyResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oMyResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oMyResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                Assembly exceutable;
                Type Ltype;
                object objclass;
                exceutable = Assembly.Load("ZeeInsurance");
                Ltype = exceutable.GetType("ZeeInsurance" + "." + "Policy");
                objclass = Activator.CreateInstance(Ltype);
                if (!string.IsNullOrEmpty(sMethodName))
                {
                    MethodInfo method = Ltype.GetMethod(sMethodName);
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
                    oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID });
                    oParams.Add(new CNV { sName = "sBOGroupName", sValue = sGroup });
                    oParams.Add(new CNV { sName = "sBO", sValue = sBO });
                    oParams.Add(new CNV { sName = "iInstanceID", sValue = sInstanceID });
                    object Response = new object();
                    object[] parametersArray = new object[] { oParams };
                    Response = (object)method.Invoke(objclass, parametersArray);
                    oMyResult = (CResult)Response;
                }
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult Invoke(string sMethodName, string sBO, string sInstanceID, string sGroup, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oMyResult.sClassName = oMyResult.Get_Class(); //AUTO-DERIVE
            oMyResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oMyResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oMyResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                string sDll = string.Empty;
                string sClass = string.Empty;
                if (sMethodName.Contains("/"))
                {
                    sDll = sMethodName.Split('/')[0];
                    sClass = sMethodName.Split('/')[1];
                    sMethodName = sMethodName.Split('/')[2];
                }
                if (!string.IsNullOrEmpty(sDll) && !string.IsNullOrEmpty(sClass) && !string.IsNullOrEmpty(sMethodName))
                {
                    Assembly exceutable;
                    Type Ltype;
                    object objclass;
                    exceutable = Assembly.Load(sDll);
                    Ltype = exceutable.GetType(sDll + "." + sClass);
                    Assembly exceutable1;
                    Type Ltype1;
                    object objclass1 = null;
                    if (sClass.ToLower() == "xiconfigs")
                    {
                        exceutable1 = Assembly.Load("XIDNA");
                        Ltype1 = exceutable1.GetType("XIDNA" + "." + "SignalR");
                        objclass1 = Activator.CreateInstance(Ltype1);
                        object[] args = new object[1];
                        if (objclass1 != null)
                        {
                            args[0] = objclass1;
                        }
                        objclass = Activator.CreateInstance(Ltype, args);
                    }
                    else
                    {
                        objclass = Activator.CreateInstance(Ltype);
                    }
                    if (!string.IsNullOrEmpty(sMethodName))
                    {
                        MethodInfo method = Ltype.GetMethod(sMethodName);
                        List<CNV> oParams = new List<CNV>();
                        oParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
                        oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID });
                        oParams.Add(new CNV { sName = "sBOGroupName", sValue = sGroup });
                        oParams.Add(new CNV { sName = XIConstant.Param_BO, sValue = sBO });
                        oParams.Add(new CNV { sName = XIConstant.Param_InstanceID, sValue = sInstanceID });
                        XIInfraCache oCache = new XIInfraCache();
                        var AppID = oCache.Get_ParamVal(sSessionID, sGUID, null, "FKiAppID");
                        oParams.Add(new CNV { sName = XIConstant.Param_ApplicationID, sValue = AppID });
                        object Response = new object();
                        object[] parametersArray = new object[] { oParams };
                        Response = (object)method.Invoke(objclass, parametersArray);
                        oMyResult = (CResult)Response;
                    }
                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }
        public CResult GetSettingConfig(string sSettingName, string sSecurity, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oMyResult.sClassName = oMyResult.Get_Class(); //AUTO-DERIVE
            oMyResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oMyResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oMyResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                XIAPI xiAPI = new XIAPI();
                // get the attr value
                //CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sSettingName", sValue = sSettingName });
                Params.Add(new CNV() { sName = "sSecurity", sValue = sSecurity });
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                oMyResult = (CResult)xiAPI.GetSettingConfig(Params);
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }

        public CResult XIfunction(List<CNV> oParams)
        {
            CResult oMyResult = new CResult();
            try
            {
                var FunctionName = oParams.Where(m => m.sName.ToLower() == "xifunction").Select(m => m.sValue).FirstOrDefault().ToLower();
                switch (FunctionName)
                {
                    case "convertdate":
                        var InputDate = oParams.Where(m => m.sName.ToLower() == "inputdate").Select(m => m.sValue).FirstOrDefault();
                        var Format = oParams.Where(m => m.sName.ToLower() == "format").Select(m => m.sValue).FirstOrDefault();
                        var sDate = string.Empty;
                        XIIAttribute oAttrI = new XIIAttribute();
                        var Dt = oAttrI.ConvertToDtTime(InputDate);
                        sDate = Dt.ToString(Format);
                        oMyResult.oResult = sDate;
                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        break;
                    case "dateadd":
                        InputDate = oParams.Where(m => m.sName.ToLower() == "inputdate").Select(m => m.sValue).FirstOrDefault();
                        var Days = oParams.Where(m => m.sName.ToLower() == "days").Select(m => m.sValue).FirstOrDefault();
                        sDate = string.Empty;
                        Dt = Convert.ToDateTime(InputDate).AddDays(Convert.ToInt32(Days));
                        sDate = Dt.ToString();
                        oMyResult.oResult = sDate;
                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        break;
                    case "age":
                        XIDFormat format = new XIDFormat();
                        var age = format.XIFormat_Age(oParams);
                        oMyResult.oResult = age;
                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        break;
                    default:
                        break;
                }
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }
        public CResult BindSignalRData(string sBO, string sInstance, string sGroup, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oMyResult.sClassName = oMyResult.Get_Class(); //AUTO-DERIVE
            oMyResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oMyResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oMyResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var ConnectionID = oCache.Get_ParamVal(sSessionID, sGUID, "", "SignalRConnectID");
                XIIXI oXII = new XIIXI();
                var oBOI = oXII.BOI(sBO, sInstance, sGroup);
                oBOI.BOI = oBOI;
                if (oBOI != null && oBOI.Attributes.Count > 0)
                {
                    XIIBO oXiBo = new XIIBO();
                    List<XIIBO> oRiskFactorI = new List<XIIBO>();
                    QueryEngine oQE = new QueryEngine();
                    var RiskDetails = "";
                    //string sWhereCondition = $"FKiQSInstanceID={InstanceID},FKiProductVersionID={ProductversionID},XIDeleted=0";
                    string sWhereCondition = "FKiQuoteID=" + sInstance + ",XIDeleted=0";
                    var oQResult = oQE.Execute_QueryEngine("RiskFactor_T", "ID, sFactorName, sValue, sMessage", sWhereCondition);
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oRiskFactorI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.OrderByDescending(f => f.AttributeI("ID").iValue).ToList();
                        foreach (var oRisk in oRiskFactorI)
                        {
                            RiskDetails += "<tr><td>" + oRisk.Attributes["sfactorname"].sValue + ":" + oRisk.Attributes["svalue"].sValue + " " + oRisk.Attributes["smessage"].sValue + "<td></tr>";
                        }
                    }
                    //if (oBOI.BOD != null)
                    //{
                    //    var AttrD = oBOI.BOD.Attributes.Where(m => m.Value.OptionList.Count > 0).ToList();
                    //    foreach (var attr in AttrD)
                    //    {
                    //        if (oBOI.Attributes.ContainsKey(attr.Key))
                    //        {
                    //            oBOI.Attributes[attr.Key].sResolvedValue = oBOI.AttributeI(attr.Key).sResolvedValue;
                    //        }
                    //    }
                    //}
                    var QSInstanceID = oBOI.Attributes["FKiQSInstanceID"].sValue;
                    if (string.IsNullOrEmpty(ConnectionID))
                    {
                        ConnectionID = oCache.Get_ParamVal("SignalRConnect", "SignalRConnect", "", "SignalRConnectID_" + QSInstanceID);
                    }

                    oBOI.SetAttribute("RiskDetails", RiskDetails);
                    var oAnnonymous = from Attr in oBOI.Attributes
                                      select new
                                      {
                                          Attr.Value.sName,
                                          Attr.Value.sValue
                                      };
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    //hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                    hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oBOI.Attributes);
                }
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }
        public CResult ExecuteOneclick(string sOneclickName, string sOneclickID,string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oMyResult.sClassName = oMyResult.Get_Class(); //AUTO-DERIVE
            oMyResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oMyResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oMyResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                XIInfraCache oCache = new XIInfraCache();
                if (!string.IsNullOrEmpty(sOneclickName))
                {
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneclickName, null);
                }
                else if(!string.IsNullOrEmpty(sOneclickID))
                {
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, sOneclickID);
                }
                o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);

                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> nParams = new List<CNV>();
                nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                o1ClickC.ReplaceFKExpressions(nParams);
                var oCRes = o1ClickC.OneClick_Execute();
                oMyResult.oResult = oCRes;
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + ex.Message + "\r\n";
            }
            return oMyResult;
        }
    }
}