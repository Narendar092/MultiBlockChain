using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraDocumentTreeComponent
    {
        public CResult XILoad(List<CNV> Params)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";
            try
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = "Success";
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