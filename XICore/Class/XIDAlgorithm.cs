using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIDAlgorithm : XIDefinitionBase
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sAlgorithm { get; set; }
        public string sOrigin { get; set; }
        public string sOriginType { get; set; }

        public int FKiUserID { get; set; }
        public List<XIDAlgorithmLine> Lines { get; set; }
        public Guid XIGUID { get; set; }
        public Guid FKiParameterIDXIGUID { get; set; }
        public int FKiParameterID { get; set; }
        private iSiganlR oSignalR = null;
        public XIDAlgorithm() { }
        public XIDAlgorithm(iSiganlR oSignalRI)
        {
            oSignalR = oSignalRI;
        }

        public CResult Execute_XIAlgorithm(string sSessionID, string sGUID, List<CNV> oParams = null, iSiganlR oSignalR = null)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            List<CNV> oTraceInfo = new List<CNV>();
            oTrace.sTask = "Execute process controller";//expalin about this method logic
            int PCIID = 0;
            try
            {
                //Save PCInstance
                oCR = Save_PCInstance((Int32)Enum.Parse(typeof(xiEnumSystem.EnumPCStatus), xiEnumSystem.EnumPCStatus.Process.ToString()));
                if (oCR.bOK && oCR.oResult != null)
                {
                    string sID = (string)oCR.oResult;
                    int.TryParse(sID, out PCIID);
                }
                CResult oCRCompile = new CResult();
                CResult oCRRun = new CResult();
                CAlgorithmDefinition oAlgo = new CAlgorithmDefinition();
                CAlgorithmInstance oAlgoI = new CAlgorithmInstance();
                oAlgoI.iPCIID = PCIID;
                string sParameterValues = sSessionID + "," + sGUID;
                if (Lines != null && Lines.Count() > 0)
                {
                    var FirstLine = Lines.OrderBy(m => m.iOrder).Select(m => m.sScript).FirstOrDefault();
                    if (oParams != null && oParams.Count() > 0)
                    {
                        var sPrmName = string.Empty;
                        var sPrmVal = string.Empty;
                        foreach (var prm in oParams)
                        {
                            sPrmName = sPrmName + "p." + prm.sName + ",";
                            sPrmVal = sPrmVal + prm.sValue + ",";
                        }
                        FirstLine = FirstLine + "," + sPrmName.Substring(0, sPrmName.Length - 1);
                        sParameterValues = sParameterValues + "," + sPrmVal.Substring(0, sPrmVal.Length - 1);
                    }
                    Lines.OrderBy(m => m.iOrder).FirstOrDefault().sScript = FirstLine;
                    sAlgorithm = string.Join("&", Lines.OrderBy(m => m.iOrder).Select(m => m.sScript).ToList());
                }
                if (!string.IsNullOrEmpty(sAlgorithm))
                {
                    oCRCompile = oAlgoI.Compile_FromText(sAlgorithm);
                    oAlgoI.Definition = (CAlgorithmDefinition)oCRCompile.oResult;
                    //remove sessionid and GUID

                    //CXIAPI_RT oCXIAPI = new CXIAPI_RT();
                    //oCXIAPI.sSessionID = sSessionID;
                    //oCXIAPI.sGUID = sGUID;


                    //CXIAPI_EXE ocXIAPI = new CXIAPI_EXE();
                    //ocXIAPI.qsid = 1234;

                    //oCRRun = oAlgoI.Execute_OM(sParameterValues, ocXIAPI);

                    oAlgoI.oSignalR = oSignalR;
                    oAlgoI.FKiPCDID = XIGUID.ToString();
                    oCRRun = oAlgoI.Execute_OM(sParameterValues, sSessionID, sGUID, Lines);
                    if (oCRRun.bOK && oCRRun.oResult != null)
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = (CResult)oCRRun;
                        string sMessage = oCResult.sMessage;
                        oCR = Save_PCInstance((Int32)Enum.Parse(typeof(xiEnumSystem.EnumPCStatus), xiEnumSystem.EnumPCStatus.PCSuccess.ToString()), sMessage, "", PCIID.ToString());
                    }
                    else
                    {
                        string sMessage = oCResult.sMessage;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCR = Save_PCInstance((Int32)Enum.Parse(typeof(xiEnumSystem.EnumPCStatus), xiEnumSystem.EnumPCStatus.PCError.ToString()), sMessage, "", PCIID.ToString());
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
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                string sMessage = oCResult.sMessage;
                oCR = Save_PCInstance((Int32)Enum.Parse(typeof(xiEnumSystem.EnumPCStatus), xiEnumSystem.EnumPCStatus.PCError.ToString()), sMessage, "", PCIID.ToString());
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oCR = Log_Performance(sName, oTrace.iLapsedTime, XIGUID, 10);
            return oCResult;
        }

        public CResult Save_PCInstance(int iStatus, string sMessage = "", string LogMessage = "", string sPCIID = "")
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
                oTrace.oParams.Add(new CNV { sName = "XIGUID", sValue = XIGUID.ToString() });
                if (iStatus > 0)//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIIXI oXI = new XIIXI();
                    XIIBO oBOI = new XIIBO();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIPCInstance");
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("ID", sPCIID);
                    oBOI.SetAttribute("FKiPCDID", ID.ToString());
                    oBOI.SetAttribute("FKiPCDIDXIGUID", XIGUID.ToString());
                    oBOI.SetAttribute("iStatus", iStatus.ToString());
                    oBOI.SetAttribute("sResultMessage", sMessage);
                    oBOI.SetAttribute("FKiLogMessageID", LogMessage);
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oBOI = (XIIBO)oCR.oResult;
                        var PCIID = oBOI.AttributeI("id").sValue;
                        oCResult.oResult = PCIID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iStatus is missing";
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

    public class XIDAlgorithmLine
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sScript { get; set; }
        public int FKiAlgorithmID { get; set; }
        public decimal iOrder { get; set; }
        public bool bISReturnUIMessage { get; set; }
        public bool bUserMessage { get; set; }
    }
}