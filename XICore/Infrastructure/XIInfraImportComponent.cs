using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XISystem;

namespace XICore
{
    public class XIInfraImportComponent : XIDefinitionBase
    {
        public CResult Import_File(List<CNV> oParams)
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
            oTrace.sTask = "import objects from files";//expalin about this method logic            
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Class is XIInfraImportComponent and Method is Import_File" });
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                var AppID = oInfo.iApplicationID;
                var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
                var ImportID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sSessionID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SessionID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(ImportID))//check mandatory param communication instance uid passed or not
                {
                    var sLines = new List<string>();
                    oTraceInfo.Add(new CNV { sValue = "Mandatory parameter import instance uid is: " + ImportID });
                    var oImportI = oXI.BOI("XIImportBatch", ImportID);
                    if (oImportI != null && oImportI.Attributes.Count() > 0)
                    {
                        var BODID = oImportI.AttributeI("FKiBODID").sValue;
                        var Attributes = oImportI.AttributeI("sAttributes").sValue;
                        var FileID = oImportI.AttributeI("FKiFileID").sValue;
                        int iFileID = 0;
                        int.TryParse(FileID, out iFileID);
                        if (iFileID > 0 && !string.IsNullOrEmpty(Attributes))
                        {
                            var DocI = oXI.BOI("Documents_T", iFileID.ToString());
                            if (DocI != null && DocI.Attributes.Count() > 0)
                            {
                                var sFilePath = string.Empty;
                                var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                                var sVirtualPath = "~\\" + sVirtualDir + "\\";
                                sFilePath = System.Web.Hosting.HostingEnvironment.MapPath(sVirtualPath) + "//Files//csv//" + DocI.AttributeI("sFullPath").sValue;
                                oTraceInfo.Add(new CNV { sValue = "sFilePath is: " + sFilePath });
                                using (StreamReader sr = new StreamReader(sFilePath))
                                {
                                    string currentLine = string.Empty;
                                    while ((currentLine = sr.ReadLine()) != null)
                                    {
                                        string sLine = string.Empty;
                                        string sRemoveNewLine = string.Empty;
                                        string sRemoveSpace1 = string.Empty;
                                        string sRemoveSpace2 = string.Empty;
                                        string sRemoveQuotes = string.Empty;
                                        string sRemoveDoubleQuotes = string.Empty;
                                        // Search, case insensitive, if the currentLine contains the searched keyword
                                        //if (currentLine.IndexOf("CRLF", StringComparison.CurrentCultureIgnoreCase) >= 0)

                                        //remove CRLF
                                        Regex rgxNewLine = new Regex("CRLF");
                                        sRemoveNewLine = rgxNewLine.Replace(currentLine, "");

                                        //replace double quotes
                                        Regex rgxDouble = new Regex("\"\"");
                                        sRemoveDoubleQuotes = rgxDouble.Replace(sRemoveNewLine, " ");

                                        //replace space
                                        Regex rgxSpace1 = new Regex("\" ");
                                        sRemoveSpace1 = rgxSpace1.Replace(sRemoveDoubleQuotes, "\"");

                                        //replace space
                                        Regex rgxSpace2 = new Regex(" \"");
                                        sRemoveSpace2 = rgxSpace2.Replace(sRemoveSpace1, "\"");

                                        //replace single quotes
                                        Regex rgxSingle = new Regex("\"");
                                        sRemoveQuotes = rgxSingle.Replace(sRemoveSpace2, "");

                                        Regex rgxSpace3 = new Regex(" ,");
                                        string sRemoveSpace3 = rgxSingle.Replace(sRemoveQuotes, ",");

                                        //add to the list.
                                        sLines.Add(sRemoveSpace3);
                                    }
                                    oCR = Validate_File(BODID, Attributes, sLines, oParams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        int i = 0;
                                        var nBOI = (List<XIIBO>)oCR.oResult;
                                        foreach (var BOI in nBOI)
                                        {
                                            XIIBO oBOI = new XIIBO();
                                            oCR = oBOI.Save(BOI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                i++;
                                            }
                                        }
                                        if (nBOI.Count() == i)
                                        {
                                            oImportI.SetAttribute("iStatus", "10");
                                            oCR = oImportI.Save(oImportI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {

                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "Error while updating import status for instance:" + ImportID });
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Error while updating import status for instance:" + ImportID;
                                            }
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            oCResult.oResult = "All lines saved successfully";
                                        }
                                        else
                                        {
                                            oImportI.SetAttribute("iStatus", "20");
                                            oCR = oImportI.Save(oImportI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {

                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "Error while updating import status for instance:" + ImportID });
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Error while updating import status for instance:" + ImportID;
                                            }
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.sMessage = "Error while saving the line:" + i + " instance";
                                        }
                                    }
                                    else
                                    {
                                        var Errors = (List<string>)oCR.oResult;
                                        if (Errors != null && Errors.Count() > 0)
                                        {
                                            oImportI.SetAttribute("iStatus", "20");
                                            oCR = oImportI.Save(oImportI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {

                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "Error while updating import status for instance:" + ImportID });
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Error while updating import status for instance:" + ImportID;
                                            }
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            XIIBO oBOI = new XIIBO();
                                            var BIDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIBatchDetail");
                                            foreach (var Msg in Errors)
                                            {
                                                oBOI = new XIIBO();
                                                oBOI.BOD = BIDBOD;
                                                oBOI.SetAttribute("FKiBatchID", ImportID);
                                                oBOI.SetAttribute("sMessage", Msg);
                                                oCR = oBOI.Save(oBOI);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {

                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.sMessage = "Error while updating batch import error information for Message" + Msg;
                                                }
                                            }
                                            if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Data validation failed and information returned and information updated successfully to batch details";
                                            }
                                            else
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Data validation failed and information returned and failed to update information to batch details";
                                            }
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            oCResult.oResult = "Success";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "Document instance not loaded for instance:" + FileID;
                            }
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Mandatory params FileID:" + FileID + " or Gropup/Attributes:" + Attributes + " missing";
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Import instance not loaded for instance:" + ImportID;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "ImportID for XIImportBatch is received null";
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + oCResult.sMessage;
                    if (string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "no")
                    {
                        oCResult.oTraceStack = oTraceInfo;
                        SaveErrortoDB(oCResult);
                    }
                }
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                {
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Validate_File(string BODID, string sGroupName, List<string> Lines, List<CNV> oParams)
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
            oTrace.sTask = "Validate file data and return info";//expalin about this method logic            
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Class is XIInfraImportComponent and Method is Validate_File" });
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIInfraUsers oUser = new XIInfraUsers();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                var AppID = oInfo.iApplicationID;
                var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
                //var CommID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sSessionID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SessionID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                List<string> Errors = new List<string>();
                List<XIIBO> nBOI = new List<XIIBO>();
                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter object instance uid is: " + BODID });
                //Validate file data
                int iBODID = 0;
                int.TryParse(BODID, out iBODID);
                Guid BOGUID = Guid.Empty;
                Guid.TryParse(BODID, out BOGUID);
                if (iBODID > 0 || (BOGUID != null && BOGUID != Guid.Empty))
                {
                    XIDBO oBOD = new XIDBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODID);
                    XIIBO oBOI = new XIIBO();
                    List<string> Attrs = new List<string>();
                    bool bIsGroup = false;
                    if (sGroupName.Contains(','))
                    {
                        Attrs = sGroupName.Split(',').ToList();
                    }
                    else
                    {
                        bIsGroup = true;
                        var oGroupD = oBOD.Groups.Values.ToList().Where(m => m.GroupName.ToLower() == sGroupName.ToLower()).FirstOrDefault();
                        Attrs = oGroupD.BOSqlFieldNames.Split(',').ToList();
                    }
                    var DuplicateHeaders = Attrs.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                    if (DuplicateHeaders != null && DuplicateHeaders.Count() > 0)
                    {
                        oTraceInfo.Add(new CNV { sValue = "Duplicate headers found " });
                        Errors.Add("Duplicate Attributes found ");
                    }
                    else
                    {
                        for (int j = 0; j < Lines.Count(); j++)
                        {
                            oBOI = new XIIBO();
                            string sLine = Lines[j];
                            List<string> Data = sLine.Split(',').ToList();
                            List<XIDScript> Scrpts = new List<XIDScript>();
                            if (Attrs.Count() == Data.Count())
                            {
                                for (int i = 0; i < Attrs.Count(); i++)
                                {
                                    var sName = Attrs[i].Trim();
                                    var AttrD = new XIDAttribute();
                                    if (bIsGroup)
                                    {
                                        AttrD = oBOD.AttributeD(sName); ;
                                    }
                                    else
                                    {
                                        AttrD = oBOD.Attributes.Values.ToList().Where(m => m.LabelName.ToLower() == sName.ToLower()).FirstOrDefault();
                                    }
                                    var sValue = string.Empty;
                                    if (AttrD != null)
                                    {
                                        if (AttrD.IsOptionList && AttrD.OptionList != null && AttrD.OptionList.Count() > 0)
                                        {
                                            sValue = AttrD.OptionList.Where(m => m.sOptionName.ToLower() == Data[i].Trim().ToLower()).Select(m => m.sValues).FirstOrDefault();
                                            if (string.IsNullOrEmpty(sValue))
                                            {
                                                Errors.Add("Option List data not found for:" + Data[i].Trim() + "for line no:" + (j + 1));
                                            }
                                        }
                                        else if (AttrD.FKiType > 0)
                                        {
                                            var FKBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, AttrD.sFKBOName);
                                            if (FKBOD != null && !string.IsNullOrEmpty(FKBOD.sNameAttribute))
                                            {
                                                List<CNV> oWhrPrms = new List<CNV>();
                                                oWhrPrms.Add(new CNV { sName = FKBOD.sNameAttribute, sValue = Data[i].Trim() });
                                                var oFKBOI = oXI.BOI(AttrD.sFKBOName, null, null, oWhrPrms);
                                                if (oFKBOI != null && oFKBOI.Attributes.Count() > 0)
                                                {
                                                    sValue = oFKBOI.AttributeI(FKBOD.sPrimaryKey).sValue;
                                                }
                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "sNameAttribute is not configured for Object: " + AttrD.sFKBOName });
                                                Errors.Add("sNameAttribute is not configured for Object: " + AttrD.sFKBOName + " for line no:" + (j + 1));
                                            }
                                        }
                                        else
                                        {
                                            sValue = Data[i].Trim();
                                        }
                                        oBOI.SetAttribute(AttrD.Name, sValue);
                                    }
                                    else
                                    {
                                        oTraceInfo.Add(new CNV { sValue = "Attribute not found for: " + sName + " for line no:" + (j + 1) });
                                        Errors.Add("Attribute not found for: " + sName);
                                    }
                                }
                                nBOI.Add(oBOI);
                                oBOI.BOD = oBOD;
                                Scrpts = oBOI.RunScript(oBOI.BOD.Scripts.Values.Where(m => m.sType.ToLower() == "prepersist").ToList(), oBOI, "", sSessionID, "", "");
                                if (Scrpts.Count() == 0 || Scrpts.Where(m => m.IsSuccess == false).Count() == 0)
                                {

                                }
                                else
                                {
                                    foreach (var script in Scrpts.Where(m => m.IsSuccess == false).OrderBy(x => x.iOrder))
                                    {
                                        if (!script.IsSuccess)
                                        {
                                            foreach (var scriptresult in script.ScriptResults)
                                            {
                                                Errors.Add(scriptresult.sUserError + " for line no:" + (j + 1));
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Errors.Add("Attribute count and data count mismatch for line no:" + (j + 1) + ", Please check for empty spaces in the uploaded file");
                            }
                        }
                    }
                    if (Errors.Count() > 0)
                    {
                        oCResult.oResult = Errors;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                    else
                    {
                        oCResult.oResult = nBOI;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "Mandatory param BODID:" + BODID + " not passed";
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + oCResult.sMessage;
                    if (string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "no")
                    {
                        oCResult.oTraceStack = oTraceInfo;
                        SaveErrortoDB(oCResult);
                    }
                } 
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                { 
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}
