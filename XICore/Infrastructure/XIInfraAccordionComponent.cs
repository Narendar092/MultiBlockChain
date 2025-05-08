using iText.Kernel.Pdf.Tagging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XISystem;

namespace XICore
{
    public class XIInfraAccordionComponent
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
            XIDefinitionBase oDef = new XIDefinitionBase();
            try
            {
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                var BODID = oParams.Where(m => m.sName.ToLower() == "bouid").Select(m => m.sValue).FirstOrDefault();
                int iBODID = 0;
                Guid BOIDXIGUID = Guid.Empty;
                int.TryParse(BODID, out iBODID);
                Guid.TryParse(BODID, out BOIDXIGUID);
                var sStructureCode = oParams.Where(m => m.sName.ToLower() == "sStructureCode".ToLower()).Select(m => m.sValue).FirstOrDefault();
                List<XIDStructure> oTree = new List<XIDStructure>();
                if ((iBODID > 0 || (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)) && !string.IsNullOrEmpty(sStructureCode))
                {
                    XIDStructure oStrct = new XIDStructure();
                    if (BOIDXIGUID != null && BOIDXIGUID != Guid.Empty)
                    {
                        oTree = oStrct.GetXIStructureTreeDetails(BOIDXIGUID.ToString(), sStructureCode);
                    }
                    else if (iBODID > 0)
                    {
                        oTree = oStrct.GetXIStructureTreeDetails(iBODID.ToString(), sStructureCode);
                    }
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oTree;
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
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}
