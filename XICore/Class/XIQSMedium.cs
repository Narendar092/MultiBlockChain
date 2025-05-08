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
    public class XIQSMedium
    {

        public CResult RenderResponse(XIIQS oQSI, string sGUID)
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
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                CNV oQSNV = new CNV();
                CNV oSecNV = new CNV();
                CNV oValNV = new CNV();
                XIDQS oQSD = oQSI.QSDefinition;
                //XIIQS oQSI = new XIIQS();
                XIInfraCache oCache = new XIInfraCache();
                if (oQSI.Steps != null && oQSI.Steps.Count() > 0)
                {
                    oQSNV.sValue = oQSI.XIGUID.ToString();
                    oQSNV.sType = "QuestionSet|" + sGUID;
                    oQSNV.sContext = oQSD.XIGUID.ToString();
                    foreach (var Step in oQSI.Steps.Values.ToList())
                    {
                        var oStepD = oQSD.Steps.Values.ToList().Where(m => m.XIGUID == Step.FKiQSStepDefinitionIDXIGUID).FirstOrDefault();
                        CNV oNV = oQSNV.NInstance("Step-" + oStepD.sName);
                        oNV.sType = "Step";
                        oNV.sName = oStepD.sName;
                        oNV.sContext = oStepD.XIGUID.ToString();
                        if (Step.Sections != null && Step.Sections.Count() > 0)
                        {
                            foreach (var Sec in Step.Sections.Values.ToList())
                            {
                                var oSecD = oStepD.Sections.Values.ToList().Where(m => m.XIGUID == Sec.FKiStepSectionDefinitionIDXIGUID).FirstOrDefault();
                                if (oSecD != null)
                                {
                                    //List<CNV> oNVs = new List<CNV>();
                                    //oNVs.Add(new CNV { sName = "Section", sValue = oSecD.sName });
                                    oSecNV = oNV.NInstance(oSecD.ID.ToString());
                                    oSecNV.sType = "Section";
                                    oSecNV.sContext = oSecD.ID.ToString() + "_Sec|" + oSecD.XIGUID;
                                    oSecNV.fOrder = oSecD.iOrder;
                                    if (oSecD.iDisplayAs == 30 && Sec.XIValues != null && Sec.XIValues.Count() > 0)
                                    {
                                        foreach (var xivalue in Sec.XIValues.Values.ToList().OrderBy(m => m.ID).ToList())
                                        {
                                            var FieldOriginGUID = xivalue.FKiFieldOriginIDXIGUID; //oSecD.FieldDefs.Values.ToList().Where(m => m.FKiXIFieldOriginIDXIGUID == xivalue.FKiFieldOriginIDXIGUID).FirstOrDefault();
                                            var oFieldOriginD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, FieldOriginGUID.ToString());
                                            oValNV = oSecNV.NInstance(oFieldOriginD.sName);
                                            oValNV.sType = "XIValue";
                                            oValNV.sLabel = oFieldOriginD.sDisplayName;
                                            oValNV.sContext = xivalue.FKiFieldDefinitionIDXIGUID.ToString() + "|" + xivalue.FKiFieldOriginIDXIGUID;
                                            oValNV.sValue = xivalue.sValue;
                                            if (oFieldOriginD.bIsOptionList && oFieldOriginD.FieldOptionList != null && oFieldOriginD.FieldOptionList.Count() > 0)
                                            {
                                                foreach (var opt in oFieldOriginD.FieldOptionList)
                                                {
                                                    oValNV.nSubParams.Add(new CNV { sName = opt.sOptionName, sValue = opt.sOptionValue, ID = opt.ID });
                                                }
                                            }
                                            //oValNV.sName = oFieldOriginD.sDisplayName;
                                        }
                                    }
                                    else if (oSecD.iDisplayAs == 40)
                                    {
                                        //IF Component is configured in the section
                                        if (oSecD.iXIComponentIDXIGUID != null && oSecD.iXIComponentIDXIGUID != Guid.Empty)
                                        {
                                            oValNV.sValue = "HTML Merge started";
                                            oValNV = oSecNV.NInstance(oSecD.XIGUID.ToString());
                                            XIIQSSection oSecI = new XIIQSSection();
                                            oSecI.FKiStepSectionDefinitionIDXIGUID = oSecD.XIGUID;
                                            oSecI.oDefintion = oSecD;
                                            oSecI.sGUID = sGUID;
                                            oCR = oSecI.Load();
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oValNV.sValue = "HTML Merge result";
                                                var oRes1 = (XIIQSSection)oCR.oResult;
                                                if (oRes1.oContent.ContainsKey(XIConstant.ContentXIComponent))
                                                {
                                                    var oRes2 = (XIIComponent)oRes1.oContent[XIConstant.ContentXIComponent];
                                                    if (oRes2.oContent.ContainsKey(XIConstant.HTMLComponent))
                                                    {
                                                        var oRes3 = (XID1Click)oRes2.oContent[XIConstant.HTMLComponent];
                                                        var sHTML = string.Empty;
                                                        if (oRes3.RepeaterResult != null && oRes3.RepeaterResult.Count > 0)
                                                        {
                                                            oValNV.sValue = "HTML Merge data assigned";
                                                            sHTML = oRes3.RepeaterResult.FirstOrDefault();
                                                            oValNV.sType = "HTML";
                                                            oValNV.sValue = sHTML;//If we use image of PDf then the content will be 'image_https etc','pdf_https etc'
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (oSecD.iDisplayAs == 50)
                                    {
                                        //IF HTML Component is configured in the section
                                        if (!string.IsNullOrEmpty(oSecD.HTMLContent))
                                        {
                                            oValNV = oSecNV.NInstance(oSecD.XIGUID.ToString());
                                            oValNV.sType = "HTML";
                                            oValNV.sValue = oSecD.HTMLContent;//If we use image of PDf then the content will be 'image_https etc','pdf_https etc'
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oQSNV;
                //if (true)//check mandatory params are passed or not
                //{
                //    //oCR = SubMethod();
                //    if (oCR.bOK && oCR.oResult != null)
                //    {
                //        oTrace.oTrace.Add(oCR.oTrace);
                //        if (oCR.bOK && oCR.oResult != null)
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //            oCResult.oResult = "Success";
                //        }
                //        else
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //        }
                //    }
                //    else
                //    {
                //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //    }
                //}
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTrace.sMessage = "Mandatory Param:  is missing";
                //}
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
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

        public CResult PrintToText(CNV oNV)
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
                List<string> oData = new List<string>();
                oData.Add(oNV.sName.ToString() + "\r\n");
                foreach (var SecNV in oNV.NNVs.Values.ToList())
                {
                    oData.Add("\r\n" + SecNV.sName.ToString());
                    foreach (var ValueNV in SecNV.NNVs.Values.ToList())
                    {
                        switch (ValueNV.sType.ToLower())
                        {
                            case "html":
                                oData.Add(ValueNV.sValue + "\r\n");
                                break;
                            default:
                                oData.Add(ValueNV.sName + ":" + ValueNV.sValue);
                                break;
                        }
                    }
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = string.Join("\r\n", oData);
                //if (true)//check mandatory params are passed or not
                //{
                //    //oCR = SubMethod();
                //    if (oCR.bOK && oCR.oResult != null)
                //    {
                //        oTrace.oTrace.Add(oCR.oTrace);
                //        if (oCR.bOK && oCR.oResult != null)
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //            oCResult.oResult = "Success";
                //        }
                //        else
                //        {
                //            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //        }
                //    }
                //    else
                //    {
                //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                //    }
                //}
                //else
                //{
                //    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                //    oTrace.sMessage = "Mandatory Param:  is missing";
                //}
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
    }
}
