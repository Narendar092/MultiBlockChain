using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    [Table("PaymentGateWay_T")]
    public class XIInfraPayment
    {
        [Key]
        public int ID { get; set; }
        public string sName { get; set; }
        public int OrganizationID { get; set; }
        public string sOrganizationName { get; set; }
        public int ApplicationID { get; set; }
        public string sApplicationName { get; set; }
        public string sMerchantID { get; set; }
        public string sSecret { get; set; }
        public string ResponseUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string Mode { get; set; }
        public int StatusTypeID { get; set; }
        public string sAccount { get; set; }
        public string sServerKey { get; set; }
        public string sHPPVerion { get; set; }

        public CResult Get_PaymentGatway(int OrganizationID, string sCoreDatabase, string sServerKey,int iGlobalPaymentID)
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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                XIInfraPayment oPaymentGateWay = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["StatusTypeID"] = 10;
                if (iGlobalPaymentID > 0)
                {
                    Params["ID"] = iGlobalPaymentID;
                }
                else { 
                 if (OrganizationID > 0)
                    {
                        Params["OrganizationID"] = OrganizationID;
                    }
                    if(!string.IsNullOrEmpty(sServerKey))
                    {
                        Params["sServerKey"] = sServerKey;
                    }
                }
                Params["XIDeleted"] = 0;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                oPaymentGateWay = Connection.Select<XIInfraPayment>("PaymentGateWay_T", Params).FirstOrDefault();
                oCResult.oResult = oPaymentGateWay;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }

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
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (true)//check mandatory params are passed or not
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
                }
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
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}