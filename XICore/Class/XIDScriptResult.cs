using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;
using System.Configuration;

namespace XICore
{
    public class XIDScriptResult
    {
        public int ID { get; set; }
        public int FKiScriptID { get; set; }
        public Guid FKiScriptIDXIGUID { get; set; }
        public string sResultcode { get; set; }
        public int iType { get; set; }
        public int iAction { get; set; }
        public string sUserError { get; set; }
        public string sResultCode { get; set; }
        public string sType { get; set; }
        public string sScriptResult { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult Get_BOScriptResult(string sDatabase)
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
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                XIDScriptResult XIDScriptResult = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if(FKiScriptIDXIGUID != null && FKiScriptIDXIGUID != Guid.Empty)
                {
                    Params["FKiScriptIDXIGUID"] = FKiScriptIDXIGUID;
                }
                else if (FKiScriptID > 0)
                {
                    Params["FKiScriptID"] = FKiScriptID;
                }
                if (!string.IsNullOrEmpty(sResultcode))
                {
                    Params["sResultcode"] = sResultcode;
                }
                XIDScriptResult = Connection.Select<XIDScriptResult>("XIBOScriptResult_T", Params).FirstOrDefault();
                oCResult.oResult = XIDScriptResult;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
    }
}