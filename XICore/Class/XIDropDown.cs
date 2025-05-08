using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIDropDown
    {
        public int ID { get; set; }
        public int Value { get; set; }
        public string sValue { get; set; }
        public string text { get; set; }
        public string Expression { get; set; }
        public string Type { get; set; }
        public int FKiAppID { get; set; }
        public Guid XIGUID { get; set; }
        public string sGUID { get; set; }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult Get_XILayoutsDDL(string sOrgDatabase)
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
                List<XIDLayout> oXIDLayout = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["LayoutType"] = "Inline";
                Params["FKiApplicationID"] = FKiAppID;
                oXIDLayout = Connection.Select<XIDLayout>("XILayout_T", Params).ToList();
                var oXIDLayoutDDL = oXIDLayout.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.LayoutName }).ToList();
                oXIDLayoutDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oCResult.oResult = oXIDLayoutDDL;
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

        public CResult Get_XIThemesDDL(string sOrgDatabase)
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
                List<XIDMasterData> oXIDThemes = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["Name"] = "Themes";
                Params["FKiApplicationID"] = FKiAppID;
                oXIDThemes = Connection.Select<XIDMasterData>("XIMasterData_T", Params).ToList();
                var oXIDTThemeDDL = oXIDThemes.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.Expression }).ToList();
                oXIDTThemeDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oCResult.oResult = oXIDTThemeDDL;
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
    }
}