using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraCheckboxComponent : XIDefinitionBase
    {
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            string sParGUID = string.Empty;
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
                XIInfraCache oCache = new XIInfraCache();
                int i1ClickID = 0;
                Guid OneClickXIGUID = Guid.Empty;
                var s1ClickID = oParams.Where(m => m.sName == XIConstant.Param_1ClickID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(s1ClickID, out i1ClickID);
                Guid.TryParse(s1ClickID, out OneClickXIGUID);
                if (i1ClickID > 0 || (OneClickXIGUID != null && OneClickXIGUID != Guid.Empty))
                {
                    var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                    var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    List<CNV> nParms = new List<CNV>();
                    nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    var o1CActions = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "XI1ClickActionList");
                    var o1CActionsC = (XID1Click)o1CActions.Clone(o1CActions);
                    o1CActionsC.ReplaceFKExpressions(nParms);
                    var o1CActionsResult = o1CActionsC.OneClick_Run();
                    var List2 = o1CActionsResult.Values.ToList();
                    var o1ClickD = new XID1Click();
                    if(OneClickXIGUID != null && OneClickXIGUID != Guid.Empty)
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickXIGUID.ToString());
                    }
                    else if(i1ClickID > 0)
                    {
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                    }
                    var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    o1ClickC.ReplaceFKExpressions(nParms);
                    var Result = o1ClickC.OneClick_Run();
                    List<CNV> List = new List<CNV>();
                    if (Result != null && Result.Count() > 0)
                    {
                        foreach (var item in Result.Values.ToList())
                        {
                            var ID = item.AttributeI("id").sValue;
                            var Name = item.AttributeI("sname").sValue;
                            var Inline = List2.Where(m => m.Attributes.Values.Where(n => n.sName.ToLower() == "fkiactionid").FirstOrDefault().sValue == ID && m.Attributes.Values.Where(n => n.sName.ToLower() == "itype").FirstOrDefault().sValue == "10").FirstOrDefault();
                            var Row = List2.Where(m => m.Attributes.Values.Where(n => n.sName.ToLower() == "fkiactionid").FirstOrDefault().sValue == ID && m.Attributes.Values.Where(n => n.sName.ToLower() == "itype").FirstOrDefault().sValue == "20").FirstOrDefault();
                            if (Inline != null && Inline.Attributes.Count() > 0 && Row != null && Row.Attributes.Count() > 0)
                            {
                                List.Add(new CNV { sName = Name, sValue = ID, sType = "both", sContext = (Inline.AttributeI("id").sValue + "_" + Row.AttributeI("id").sValue).ToString() });
                            }
                            else if (Inline != null && Inline.Attributes.Count() > 0)
                            {
                                List.Add(new CNV { sName = Name, sValue = ID, sType = "inline", sContext = Inline.AttributeI("id").sValue });
                            }
                            else if (Row != null && Row.Attributes.Count() > 0)
                            {
                                List.Add(new CNV { sName = Name, sValue = ID, sType = "row", sContext = Row.AttributeI("id").sValue });
                            }
                            else
                            {
                                List.Add(new CNV { sName = Name, sValue = ID });
                            }

                        }
                    }
                    oCResult.oResult = List;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }

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
    }
}