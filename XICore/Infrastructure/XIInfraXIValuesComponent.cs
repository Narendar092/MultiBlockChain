using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraXIValuesComponent
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
            try
            {
                int i1ClickID = 0;
                Guid ClickGUID = Guid.Empty;
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                Dictionary<string, string> IDEParams = new Dictionary<string, string>();
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                var sParentInsID = "";
                oParams.Add(new CNV { sName = "watchparam", sValue = "-subQSRiskTree" });
                XIInfraCache oCache = new XIInfraCache();
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
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    string sOneClickID = WrapperParms.Where(m => m.sName == XIConstant.XIP_1ClickID).Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(sOneClickID, out i1ClickID))
                    {

                    }
                    else
                    {
                        i1ClickID = 0;
                    }
                    var sParentFK = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentFKColumn).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentFK))
                    {
                        sParentInsID = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentInsID).Select(m => m.sValue).FirstOrDefault();
                    }

                    var sTreeGUID = WrapperParms.Where(m => m.sName == XIConstant.Param_TreeGUID).Select(m => m.sValue).FirstOrDefault();
                    var iNodeID = WrapperParms.Where(m => m.sName == XIConstant.Param_NodeID).Select(m => m.sValue).FirstOrDefault();
                    IDEParams[XIConstant.Param_Mode] = "IDE";
                    IDEParams[XIConstant.Param_TreeGUID] = sTreeGUID;
                    IDEParams[XIConstant.Param_NodeID] = iNodeID;
                }
                else
                {
                    string sOneClickID = oParams.Where(m => m.sName == XIConstant.Param_i1ClickID).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(sOneClickID, out i1ClickID);
                    Guid.TryParse(sOneClickID, out ClickGUID);
                }
                oTrace.oParams.Add(new CNV { sName = "i1ClickID", sValue = i1ClickID.ToString() });
                if (i1ClickID > 0 || (ClickGUID != null && ClickGUID != Guid.Empty))//check mandatory params are passed or not
                {
                    List<string[]> Rows = new List<string[]>();
                    List<string> Row = new List<string>();
                    List<string> IDs = new List<string>();
                    List<string> Headings = new List<string>();
                    Headings.Add("ID");
                    var iCount = 0;
                    List<string> FldIDs = new List<string>();
                    string FieldOrgins = "";
                    List<CNV> nParams = new List<CNV>();
                    var o1ClickNVD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "1ClickNV XIValues");
                    var o1ClickNVC = (XID1Click)o1ClickNVD.Clone(o1ClickNVD);
                    if (i1ClickID > 0)
                        nParams.Add(new CNV { sName = "{XIP|i1ClickID}", sValue = i1ClickID.ToString() });
                    if (ClickGUID != null && ClickGUID != Guid.Empty)
                        nParams.Add(new CNV { sName = "{XIP|i1ClickID}", sValue = ClickGUID.ToString() });
                    o1ClickNVC.ReplaceFKExpressions(nParams);
                    var ResNV = o1ClickNVC.OneClick_Execute();
                    if (ResNV != null && ResNV.Count() > 0)
                    {
                        iCount = ResNV.Count();
                        foreach (var flds in ResNV.Values.ToList())
                        {
                            var fID = flds.AttributeI("sValue").sValue;
                            FldIDs.Add(fID);
                            var oFO = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, fID);
                            Headings.Add(oFO.sDisplayName);
                        }
                        FieldOrgins = string.Join(",", FldIDs);
                    }
                    if (ClickGUID != null && ClickGUID != Guid.Empty)
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, ClickGUID.ToString());
                    if (i1ClickID > 0)
                        o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);


                    if (string.IsNullOrEmpty(sParentInsID))
                    {
                        var iQSIID = oGUIDParams.NMyInstance["{XIP|iQSInstanceID}"].sValue;
                        nParams.Add(new CNV { sName = "{XIP|iQSInstanceID}", sValue = iQSIID });
                    }
                    else
                    {
                        nParams.Add(new CNV { sName = "{XIP|iQSInstanceID}", sValue = sParentInsID });
                    }
                    nParams.Add(new CNV { sName = "{XIP|sOrigins}", sValue = FieldOrgins });
                    //nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    o1ClickC.ReplaceFKExpressions(nParams);
                    //var Res1 = o1ClickC.Execute_Query();
                    var Res = o1ClickC.OneClick_Execute();
                    int iColCount = 0;
                    if (Res != null && Res.Count() > 0)
                    {
                        //var Cols = Res1.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                        //List<string> Colslist = new List<string>(Cols);
                        //int iDisplayIndex = Colslist.IndexOf("sDisplayName");
                        //int iValueIndex = Colslist.IndexOf("sValue");
                        //int iIDIndex = Colslist.IndexOf("id");
                        //List<string> RowIDs = new List<string>();
                        //foreach (DataRow row in Res1.Rows)
                        //{
                        //    if (!RowIDs.Contains(row.ItemArray[iIDIndex].ToString()))
                        //        RowIDs.Add(row.ItemArray[iIDIndex].ToString());
                        //}
                        //List<string> Data = new List<string>();
                        //foreach(var ID in RowIDs)
                        //{
                        //    var AllIns = Res1.AsEnumerable().ToList().Select(m => m.ItemArray).ToList().Where(m=>m[0].ToString() == ID.ToString()).Select(m => m).ToList();
                        //}
                        //foreach (DataRow row in Res1.Rows)
                        //{
                        //    Data.Add(row.ItemArray[iValueIndex].ToString());
                        //}


                        //var AllData = Res.Values.ToList().Select(m => m.Attributes.Values.ToList()).ToList();



                        //var AllIDs = Res.Values.ToList().Select(m => m.Attributes.Values.ToList().Where(n => n.sName == "id").Select(n => n.sValue).FirstOrDefault()).ToList();
                        //var UniqueIDs = AllIDs.Distinct().ToList();

                        //foreach (var itesm in UniqueIDs)
                        //{
                        //    var data = Res.Values.ToList().Select(m => m.Attributes.Values.ToList().Where(n => n.sName == "id" && n.sValue == itesm).FirstOrDefault()).ToList();
                        //    foreach (var fo in FldIDs)
                        //    {
                        //        var foi = data.Where(m => m.sName == "FKiFieldOriginID" && m.sValue == fo).FirstOrDefault();
                        //        if (foi != null)
                        //        {

                        //        }
                        //    }
                        //}



                        //foreach (var data in Res.Values.ToList())
                        //{
                        //    Headings.Add(data.AttributeI("sdisplayname").sValue);
                        //    iColCount++;
                        //    if (iColCount == iCount)
                        //    {
                        //        break;
                        //    }
                        //}
                        int iRowCount = 0;
                        List<CNV> list = new List<CNV>();
                        foreach (var data in Res.Values.ToList())
                        {
                            list.Add(new CNV { sType = data.AttributeI("id").sValue, sName = data.AttributeI("sdisplayname").sValue, sValue = data.AttributeI("sderivedvalue").sValue });
                            //if (iRowCount == 0)
                            //{
                            //    Row.Add(data.AttributeI("id").sValue);
                            //}
                            //iRowCount++;
                            //Row.Add(data.AttributeI("sderivedvalue").sValue);
                            //if (iRowCount == iCount)
                            //{
                            //    iRowCount = 0;
                            //    var NewRow = Row.ToArray();
                            //    Rows.Add(NewRow);
                            //    Row = new List<string>();
                            //}
                        }
                        var UniqueIDs = list.Select(m => m.sType).Distinct().ToList();
                        foreach (var id in UniqueIDs)
                        {
                            foreach (var head in Headings)
                            {
                                if (head.ToLower() == "id")
                                {
                                    Row.Add(list.Where(m => m.sType == id).Select(m => m.sType).FirstOrDefault());
                                }
                                else
                                {
                                    var sValue = list.Where(m => m.sType == id && m.sName == head).Select(m => m.sValue).FirstOrDefault();
                                    if (sValue != null)
                                        Row.Add(sValue);
                                    else
                                        Row.Add("");
                                }
                            }
                            var NewRow = Row.ToArray();
                            Rows.Add(NewRow);
                            Row = new List<string>();
                        }

                    }
                    else
                    {
                        //Headings.Add("ID");
                        //foreach (var flds in FldIDs)
                        //{
                        //    var Origin = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, flds);
                        //    Headings.Add(Origin.sDisplayName);
                        //}
                    }
                    o1ClickC.Headings = Headings;
                    o1ClickC.Rows = Rows;
                    o1ClickC.FilterGroup = IDEParams;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = o1ClickC;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: i1ClickID is missing";
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
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Expand_XIValues(List<CNV> oParams)
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
                var sSessionID = oParams.Where(m => m.sName == XIConstant.Param_SessionID).Select(m => m.sValue).FirstOrDefault();
                var sGUID = oParams.Where(m => m.sName == XIConstant.Param_GUID).Select(m => m.sValue).FirstOrDefault();
                oParams.Add(new CNV { sName = "watchparam", sValue = "-subQSRiskTree" });
                XIInfraCache oCache = new XIInfraCache();
                List<string> FldIDs = new List<string>();
                string FieldOrgins = string.Empty;
                int i1ClickID = 0;
                string sOneClickID = oParams.Where(m => m.sName == XIConstant.Param_i1ClickID).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sOneClickID, out i1ClickID);
                Guid ClickGUID = Guid.Empty;
                Guid.TryParse(sOneClickID, out ClickGUID);
                string sQSIIID = oParams.Where(m => m.sName.ToLower() == "iQSIID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iQSIID = 0;
                int.TryParse(sQSIIID, out iQSIID);
                Guid QSIIDGUID = Guid.Empty;
                Guid.TryParse(sQSIIID, out QSIIDGUID);
                oTrace.oParams.Add(new CNV { sName = "i1ClickID", sValue = i1ClickID.ToString() });
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
                string sParentInsID = string.Empty;
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    var sParentFK = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentFKColumn).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentFK))
                    {
                        sParentInsID = WrapperParms.Where(m => m.sName == XIConstant.Param_ParentInsID).Select(m => m.sValue).FirstOrDefault();
                    }
                }
                if (i1ClickID > 0 || (ClickGUID != null && ClickGUID != Guid.Empty))//check mandatory params are passed or not
                {
                    List<CNV> nParams = new List<CNV>();
                    List<CNV> oNV = new List<CNV>();
                    var o1ClickNVD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "1ClickNV XIValues Expand");
                    var o1ClickNVC = (XID1Click)o1ClickNVD.Clone(o1ClickNVD);
                    if (ClickGUID != null && ClickGUID != Guid.Empty)
                    {
                        nParams.Add(new CNV { sName = "{XIP|i1ClickID}", sValue = ClickGUID.ToString() });
                    }
                    else if (i1ClickID > 0)
                        nParams.Add(new CNV { sName = "{XIP|i1ClickID}", sValue = i1ClickID.ToString() });
                    o1ClickNVC.ReplaceFKExpressions(nParams);
                    var ResNV = o1ClickNVC.OneClick_Execute();
                    if (ResNV != null && ResNV.Count() > 0)
                    {
                        foreach (var flds in ResNV.Values.ToList())
                        {
                            var fID = flds.AttributeI("sValue").sValue;
                            FldIDs.Add(fID);
                            var Origin = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, fID);
                            CNV NV = new CNV();
                            NV.sName = Origin.sDisplayName;
                            NV.sType = Origin.sName;
                            oNV.Add(NV);
                        }
                        FieldOrgins = string.Join(",", FldIDs);
                    }
                    var o1ClickC = new XID1Click();
                    if (i1ClickID > 0)
                    {
                        var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                        o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    }
                    else if (ClickGUID != null && ClickGUID != Guid.Empty)
                    {
                        var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, ClickGUID.ToString());
                        o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    }
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);

                    if (string.IsNullOrEmpty(sParentInsID))
                    {
                        var QSIID = oGUIDParams.NMyInstance["{XIP|iQSInstanceID}"].sValue;
                        nParams.Add(new CNV { sName = "{XIP|iQSInstanceID}", sValue = QSIID });
                    }
                    else
                    {
                        nParams.Add(new CNV { sName = "{XIP|iQSInstanceID}", sValue = sParentInsID });
                    }
                    nParams.Add(new CNV { sName = "{XIP|sOrigins}", sValue = FieldOrgins });
                    o1ClickC.ReplaceFKExpressions(nParams);
                    var Res = o1ClickC.OneClick_Execute();

                    if (Res != null && Res.Count() > 0)
                    {
                        foreach (var data in Res.Values.ToList())
                        {
                            var id = data.AttributeI("id").sValue;
                            var sName = data.AttributeI("sName").sValue;
                            if (id == iQSIID.ToString())
                            {
                                var FoundNV = oNV.Where(m => m.sType == sName).FirstOrDefault();
                                if (FoundNV != null)
                                {
                                    oNV.Where(m => m.sType == sName).FirstOrDefault().sValue = data.AttributeI("sderivedvalue").sValue;
                                }
                            }
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = oNV;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: i1ClickID is missing";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
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