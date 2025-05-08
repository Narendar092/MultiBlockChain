using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic;
using XISystem;
using System.Web;
using System.Web.Caching;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Data;
using System.Web.Hosting;
//using ExcelApp = Microsoft.Office.Interop.Excel;
using Microsoft.SqlServer.Management.Smo.Mail;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;
using System.Windows.Input;

namespace XICore
{
    public class XIDValue : XIDefinitionBase
    {
        private string sMyDefaultValue;
        public string sDefaultValue
        {
            get
            {
                return sMyDefaultValue;
            }
            set
            {
                sMyDefaultValue = value;
            }
        }

        public CResult ImportFiles(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Attaching mail attachments to communication";//expalin about this method logic
            List<CNV> oTraceInfo = new List<CNV>();
            string sSourcePath = oParams.Where(m => m.sName.ToLower() == "sSourcePath".ToLower()).Select(m => m.sValue).FirstOrDefault();
            string sErrorFolder = oParams.Where(m => m.sName.ToLower() == "sErrorPath".ToLower()).Select(m => m.sValue).FirstOrDefault();
            var sErrorPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + sErrorFolder);
            sErrorPath = sErrorPath + "\\Error";
            string sSubDirectoryPath = string.Empty;
            var sFileExtension = string.Empty;
            string sFileName = string.Empty;
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Class is XIDValue and Method is ImportFiles" });
                string sTargetPath = oParams.Where(m => m.sName.ToLower() == "sTargetPath".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sCategory = oParams.Where(m => m.sName.ToLower() == "sCategory".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sDelim = oParams.Where(m => m.sName.ToLower() == "sDelim".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sXMappingDefinition = oParams.Where(m => m.sName.ToLower() == "sXMappingDefinition".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSpecificReference = oParams.Where(m => m.sName.ToLower() == "sSpecificReference".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParentFK = oParams.Where(m => m.sName.ToLower() == "sParentFK".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParentFKAttr = oParams.Where(m => m.sName.ToLower() == "sParentFKAttr".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sFrom = oParams.Where(m => m.sName.ToLower() == "sFrom".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //Attachment path is  not correct we should be using the document type path based on file type
                //var sSharedPath = System.Configuration.ConfigurationManager.AppSettings["SharedPath"];
                var sTargetPathFolder = System.Configuration.ConfigurationManager.AppSettings["TargetPathFolder"];
                //var sVirtualPath = "~\\" + sTargetPathFolder + "\\";
                var sTargetFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/" + sTargetPath); //HttpContext.Current.Server.MapPath(sVirtualPath) + "\\Files\\";
                //var sTargetFolder2 = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadedFiles/");
                //var sTargetFolder = "C:\\inetpub\\wwwroot\\Mavericks2\\UploadedFiles\\Files\\";
                oCResult.sMessage = " XIDValue :" + sTargetFolder;
                SaveErrortoDB(oCResult);
                //var sTargetFolder = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\" + sTargetPathFolder + "\\Files\\";
                sTargetFolder = sTargetFolder + "\\Files\\";
                //sCategory = "Email";
                //sDelim = "_";
                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sSourcePath is: " + sSourcePath + " and sTargetPath is:" + sTargetFolder });
                string ErrorFolder = string.Empty;
                DirectoryInfo diTarget = new DirectoryInfo(sTargetFolder);
                XIInfraCache oCache = new XIInfraCache();
                oTrace.oParams.Add(new CNV { sName = "sSourcePath", sValue = sSourcePath });
                XIIXI oXI = new XIIXI();
                XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                XIIBO oDocTreeI = new XIIBO();
                var sParentID = string.Empty;
                bool bImport = false;
                if (!string.IsNullOrEmpty(sSourcePath) && !string.IsNullOrEmpty(sTargetFolder))
                {
                    DirectoryInfo diSource = new DirectoryInfo(sSourcePath);
                    foreach (FileInfo fi in diSource.GetFiles())
                    {
                        bImport = false;
                        sFileName = fi.Name;
                        if (!string.IsNullOrEmpty(sSpecificReference))
                        {
                            if (fi.Name.ToLower().StartsWith(sSpecificReference.ToLower()))
                            {
                                bImport = true;
                            }
                        }
                        else
                        {
                            bImport = true;
                        }
                        //Get document tree node name, sCategory and / loadAttr eg Email/alsde33s/myfile.png
                        //check if document tree node already exists or not, if exists get the id
                        if (bImport)
                        {
                            oTraceInfo.Add(new CNV { sValue = "Attachment found for " + sParentFKAttr + " is: " + sParentFK + " and file name:" + fi.Name });
                            var Splits = fi.Name.Split(new string[] { sDelim }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (Splits.Count() > 0 && Splits.Count() == 2)
                            {
                                sFileExtension = fi.Extension;
                                sFileExtension = sFileExtension.Replace(".", "");
                                var oDocType = (XIInfraDocTypes)oCache.GetObjectFromCache(XIConstant.CacheXIDocType, sFileExtension);
                                oCR = Create_SubDirectory(sTargetFolder + "\\" + sFileExtension);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    Dictionary<string, string> oResponse = new Dictionary<string, string>();
                                    oResponse = (Dictionary<string, string>)oCR.oResult;
                                    sSubDirectoryPath = oResponse["SubDirectory"];
                                    var sLoadAttr = Splits[0];
                                    var sOriginalFileName = Splits[1];
                                    oCR = Check_DocumentTree(sCategory, "");
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        string sParent = (string)oCR.oResult;
                                        oCR = Check_DocumentTree(sFrom, sParent);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            sParent = (string)oCR.oResult;
                                            oCR = Check_DocumentTree(sOriginalFileName, sParent);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                var DocTreeID = (string)oCR.oResult;
                                                var iDocID = string.Empty;
                                                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T");
                                                var oBOI = new XIIBO();
                                                oBOI.BOD = oBOD;
                                                oBOI.SetAttribute("iInstanceID", "");
                                                oBOI.SetAttribute("FileName", fi.Name);
                                                oBOI.SetAttribute("sAliasName", sOriginalFileName);
                                                oBOI.SetAttribute("FKiDocType", oDocType.ID.ToString());
                                                oBOI.SetAttribute("SubDirectoryPath", sSubDirectoryPath);
                                                oCR = oBOI.Save(oBOI);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    oBOI = (XIIBO)oCR.oResult;
                                                    iDocID = oBOI.AttributeI("id").sValue;
                                                    oTraceInfo.Add(new CNV { sValue = "Document created for Attachment file name:" + fi.Name + " and DocID:" + iDocID });
                                                    string sFileTargetPath = sTargetFolder + "\\" + sFileExtension + "\\" + sSubDirectoryPath + "\\" + fi.Name;
                                                    oTraceInfo.Add(new CNV { sValue = "File coping from Source path:" + sSourcePath + "\\" + fi.Name + " to Target path:" + sFileTargetPath });
                                                    File.Copy(sSourcePath + "\\" + fi.Name, sFileTargetPath);
                                                    oBOI.SetAttribute("sfullPath", sFileTargetPath);
                                                    oCR = oBOI.Save(oBOI);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        string sPdfText = string.Empty;
                                                        //Extract content from PDF
                                                        if (!string.IsNullOrEmpty(sFileExtension) && sFileExtension.ToLower() == "pdf")
                                                        {
                                                            oCR = Get_TextFromPDF(sFileTargetPath);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                sPdfText = (string)oCR.oResult;
                                                                oTraceInfo.Add(new CNV { sValue = "Pdf content extraction completed for file:" + sFileTargetPath });
                                                            }
                                                            else
                                                            {
                                                                oTraceInfo.Add(new CNV { sValue = "Pdf content extraction failed for file:" + sFileTargetPath });
                                                            }
                                                        }
                                                        //XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                                                        oDocTreeI = new XIIBO();
                                                        oDocTreeI.BOD = oXIDOCBOD;
                                                        oDocTreeI.SetAttribute("id", DocTreeID);
                                                        oDocTreeI.SetAttribute("sName", sOriginalFileName);
                                                        oDocTreeI.SetAttribute("sParentId", sParent);
                                                        oDocTreeI.SetAttribute("sTags", sPdfText);
                                                        oDocTreeI.SetAttribute("sPageNo", "1");
                                                        oDocTreeI.SetAttribute("sFolderName", "");
                                                        oDocTreeI.SetAttribute("spath", iDocID);
                                                        oCR = oDocTreeI.Save(oDocTreeI, false);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            oTraceInfo.Add(new CNV { sValue = "Error while updating document tree object for instance:" + DocTreeID });
                                                        }
                                                        if (!string.IsNullOrEmpty(sXMappingDefinition))//Enhancement required to extract the sloadattr, we will do only when specific
                                                        {
                                                            XIDBO oMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sXMappingDefinition);
                                                            XIIBO oMapBOI = new XIIBO();
                                                            oMapBOI.BOD = oMapBOD;
                                                            oMapBOI.SetAttribute("sName", sOriginalFileName);
                                                            oMapBOI.SetAttribute(sParentFKAttr, sParentFK);//FKiCommunicationID
                                                            oMapBOI.SetAttribute("FKiDocumentID", iDocID);//XMappingObject should have this Document FK
                                                            oCR = oMapBOI.Save(oMapBOI);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oTraceInfo.Add(new CNV { sValue = "File :" + fi.Name + " is successfully attached to communication:" + sParentFK });
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                                            }
                                                            else
                                                            {
                                                                oTraceInfo.Add(new CNV { sValue = "File :" + fi.Name + " is failed to attach to communication:" + sParentFK });
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            oTraceInfo.Add(new CNV { sValue = "Attachment mapping object name is not provided" });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        oTraceInfo.Add(new CNV { sValue = "Error while updating document object for instance:" + iDocID });
                                                    }
                                                }
                                                else
                                                {
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    oTraceInfo.Add(new CNV { sValue = "Error while inserting into document object" });
                                                }
                                            }
                                            else
                                            {
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oTraceInfo.Add(new CNV { sValue = "Error while inserting into document tree object" });
                                            }
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oTraceInfo.Add(new CNV { sValue = "Error while inserting into document tree object" });
                                        }
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oTraceInfo.Add(new CNV { sValue = "Error while inserting into document tree object" });
                                    }
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oTraceInfo.Add(new CNV { sValue = "Error while creating sub directory in:" + sTargetFolder });
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Splitting delimiter not found for file name:" + fi.Name });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }

                        }

                        //Create XIDocument
                        //Copy file on origin to XIDocument target path
                        //if error move file from source to error directory with year/month/date subdirectories
                        //if success move from source to completed directory with year/month/date subdirectories
                        //Split the file name by delimiter and take the first split item as the load attribute
                        //load attribute is passed in as a parameter as Example: sExtRef
                        //load FKDefinition instance by sLoadAttribute(for this we need sFKDefinition and sLoadAtrribute)
                        //use the document fkdefinition and fkinstance to attach to fkobject(XICommunication)

                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sSourcePath or sTargetPath is missing";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //Move file to error directory
                    oCR = Move_FiletoErrorPath(sErrorPath, sFileExtension, sFileName, sSourcePath);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTraceInfo.Add(new CNV { sValue = "File moved to error path for " + sErrorPath + "\\" + sFileName });
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "Error while creating sub directory in error file path " + sErrorPath + "\\" + sFileName });
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                oCResult.sCategory = "Email attachement";
                oCResult.oTraceStack = oTraceInfo;
                SaveErrortoDB(oCResult);
            }
            catch (Exception ex)
            {
                oTraceInfo.Add(new CNV { sValue = "Error while moving file to target path" });
                //Move file to error directory
                oCR = Move_FiletoErrorPath(sErrorPath, sFileExtension, sFileName, sSourcePath);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oTraceInfo.Add(new CNV { sValue = "File moved to error path for " + sErrorPath + "\\" + sFileName });
                }
                else
                {
                    oTraceInfo.Add(new CNV { sValue = "Error while creating sub directory in error file path " + sErrorPath + "\\" + sFileName });
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.oTraceStack = oTraceInfo;
                oCResult.sCategory = "Email attachement";
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Move_FiletoErrorPath(string sErrorPath, string sFileExtension, string sFileName, string sSourcePath)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Save file to error path";//expalin about this method logic
            try
            {
                if (!string.IsNullOrEmpty(sErrorPath) && !string.IsNullOrEmpty(sFileExtension) && !string.IsNullOrEmpty(sFileName) && !string.IsNullOrEmpty(sSourcePath))
                {
                    oCR = Create_SubDirectory(sErrorPath + "\\" + sFileExtension);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        Dictionary<string, string> oResponse = new Dictionary<string, string>();
                        oResponse = (Dictionary<string, string>)oCR.oResult;
                        var sSubDirectoryPath = oResponse["SubDirectory"];
                        string sFileTargetPath = sErrorPath + "\\" + sFileExtension + "\\" + sSubDirectoryPath + "\\" + sFileName;
                        File.Copy(sSourcePath + "\\" + sFileName, sFileTargetPath);
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
                    }
                    else
                    {
                        oTrace.sMessage = "Error while creating sub directory for:" + sErrorPath + "\\" + sFileExtension;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oTrace.sMessage = "Mandatory Params sErrorPath:" + sErrorPath + " or sFileExtension:" + sFileExtension + ", or sFileName:" + sFileName + ", or sSourcePath:" + sSourcePath + " missing";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Get_TextFromPDF(string sPath)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Extract content from pdf document";//expalin about this method logic
            try
            {
                StringBuilder sPdfText = new StringBuilder();
                using (PdfReader reader = new PdfReader(sPath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        sPdfText.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = sPdfText.ToString();
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Create_SubDirectory(string sTargetPath)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Create sub directory in year/month/day pattern";//expalin about this method logic
            try
            {
                Dictionary<string, string> oResponse = new Dictionary<string, string>();
                string sPath = sTargetPath;
                List<string> sSubDirList = new List<string>();
                sSubDirList.Add("year");
                sSubDirList.Add("month");
                sSubDirList.Add("day");
                string sNewPath = string.Empty;
                foreach (var DirNames in sSubDirList)
                {
                    string sVal = "";
                    DateTime DateTme = DateTime.Now;
                    if (DirNames.ToLower() == "year")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Year;
                        sNewPath = sNewPath + "//" + DateTime.Now.Year;
                    }
                    else if (DirNames.ToLower() == "month")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Month;
                        sNewPath = sNewPath + "//" + DateTime.Now.Month;
                        sVal = DateTme.Month.ToString();
                    }
                    else if (DirNames.ToLower() == "day")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Day;
                        sNewPath = sNewPath + "//" + DateTime.Now.Day;
                    }
                    if (Directory.Exists(sPath))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(sPath);
                    }
                }
                oResponse["SubDirectory"] = sNewPath;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oResponse;
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Check_DocumentTree(string sName, string sParent)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Create or get the instance in document tree object";//expalin about this method logic
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                string sParentID = String.Empty;
                var oWhrParams = new List<CNV>();
                oWhrParams.Add(new CNV { sName = "sName", sValue = sName });
                if (!string.IsNullOrEmpty(sParent))
                {
                    oWhrParams.Add(new CNV { sName = "sParentID", sValue = sParent });
                }
                var oNode = oXI.BOI("XIDocumentTree_T", null, null, oWhrParams);
                XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                if (oNode != null && oNode.Attributes.Values.Count() > 0)
                {
                    sParentID = oNode.AttributeI("id").sValue;
                    oCResult.oResult = sParentID;
                }
                else
                {
                    XIIBO oDocTreeI = new XIIBO();
                    oDocTreeI.BOD = oXIDOCBOD;
                    oDocTreeI.SetAttribute("sName", sName);
                    oDocTreeI.SetAttribute("sParentId", sParent);
                    oDocTreeI.SetAttribute("sTags", "");
                    oDocTreeI.SetAttribute("sPageNo", "");
                    oDocTreeI.SetAttribute("sFolderName", "");
                    //oDocTreeI.SetAttribute("spath", iDocID);
                    oCR = oDocTreeI.Save(oDocTreeI, false);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oDocTreeI = (XIIBO)oCR.oResult;
                        sParentID = oDocTreeI.AttributeI("id").sValue;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = sParentID;
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Import_LeadFromExcelV2(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Import leads from excel sheet into database";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var ID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                var iDocumentid = oParams.Where(m => m.sName.ToLower() == "idocumentid").Select(m => m.sValue).FirstOrDefault();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ImportedLead");
                XIIXI oXI = new XIIXI();
                var oDocI = oXI.BOI("Documents_T", iDocumentid);
                if (oDocI != null && oDocI.Attributes.Count() > 0)
                {
                    var sFullPath = oDocI.AttributeI("sfullPath").sValue;
                    if (!string.IsNullOrEmpty(sFullPath))
                    {

                        var sFilePath = HostingEnvironment.MapPath("~/UploadedFiles") + "//Files//xlsx//" + sFullPath;
                        DataTable rs = new DataTable();
                        string sSheetName = string.Empty;
                        using (var odConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sFilePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
                        {
                            odConnection.Open();
                            DataTable dtTablesList = odConnection.GetSchema("Tables");
                            if (dtTablesList.Rows.Count > 0)
                            {
                                //sSheetName = dtTablesList.Rows(0)("TABLE_NAME").ToString;
                            }
                            using (OleDbCommand cmd = new OleDbCommand())
                            {
                                cmd.Connection = odConnection;
                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.CommandText = "SELECT * FROM [Sheet1$]";
                                using (OleDbDataAdapter oleda = new OleDbDataAdapter(cmd))
                                {
                                    oleda.Fill(rs);
                                }
                            }
                            odConnection.Close();
                        }
                        foreach (DataRow row in rs.Rows)
                        {
                            int i = 1;
                            string sFirstName = string.Empty;
                            string sLastName = string.Empty;
                            string sMob = string.Empty;
                            string sEmail = string.Empty;
                            string sPostCode = string.Empty;
                            sFirstName = row["FirstName"] == null ? null : row["FirstName"].ToString();
                            sLastName = row["LastName"] == null ? null : row["LastName"].ToString();
                            sMob = row["Mobile No"] == null ? null : row["Mobile No"].ToString();
                            sPostCode = row["PostCode"] == null ? null : row["PostCode"].ToString();
                            sEmail = row["Email"] == null ? null : row["Email"].ToString();

                            XIIBO oBOI = new XIIBO();
                            oBOI.BOD = oBOD;
                            oBOI.SetAttribute("FKiLeadImportID", ID);
                            oBOI.SetAttribute("sFirstName", sFirstName);
                            oBOI.SetAttribute("sLastName", sLastName);
                            oBOI.SetAttribute("sMob", sMob);
                            oBOI.SetAttribute("sPostCode", sPostCode);
                            oBOI.SetAttribute("sEmail", sEmail);
                            oBOI.SetAttribute("iStatus", "20");
                            oCR = oBOI.Save(oBOI);
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                            else
                            {
                                oCResult.sMessage = "Import_LeadFromExcelV2() : Error while saving row:" + i;
                                SaveErrortoDB(oCResult);
                            }
                            i++;
                        }
                    }
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = "Success";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Import_TargetFromExcelV3(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Import leads from excel sheet into database";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var ID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                int iID = 0;
                int.TryParse(ID, out iID);
                if (iID > 0)
                {
                    var iDocumentid = oParams.Where(m => m.sName.ToLower() == "idocumentid").Select(m => m.sValue).FirstOrDefault();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPTargetImportDetail");
                    var oDocI = oXI.BOI("Documents_T", iDocumentid);
                    if (oDocI != null && oDocI.Attributes.Count() > 0)
                    {
                        var sFullPath = oDocI.AttributeI("sfullPath").sValue;
                        if (!string.IsNullOrEmpty(sFullPath))
                        {
                            var sFilePath = HostingEnvironment.MapPath("~/UploadedFiles") + "//Files//xlsx//" + sFullPath;
                            DataTable rs = new DataTable();
                            string sSheetName = string.Empty;
                            using (var odConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sFilePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
                            {
                                odConnection.Open();
                                DataTable dtTablesList = odConnection.GetSchema("Tables");
                                if (dtTablesList.Rows.Count > 0)
                                {
                                    //sSheetName = dtTablesList.Rows(0)("TABLE_NAME").ToString;
                                }
                                using (OleDbCommand cmd = new OleDbCommand())
                                {
                                    cmd.Connection = odConnection;
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.CommandText = "SELECT * FROM [Sheet1$]";
                                    using (OleDbDataAdapter oleda = new OleDbDataAdapter(cmd))
                                    {
                                        oleda.Fill(rs);
                                    }
                                }
                                odConnection.Close();
                            }
                            string[] columnNames = rs.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
                            foreach (DataRow row in rs.Rows)
                            {
                                int i = 1;
                                XIIBO oRowI = new XIIBO();
                                oRowI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPTargetData");
                                oRowI.SetAttribute("FKiTargetImportID", ID);
                                oRowI.SetAttribute("iStatus", "20");
                                oRowI.SetAttribute("sName", columnNames[0] + ":" + row[columnNames[0]] == null ? null : row[columnNames[0]].ToString());
                                oCR = oRowI.Save(oRowI);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oRowI = (XIIBO)oCR.oResult;
                                    var RowID = oRowI.AttributeI("id").sValue;
                                    for (int j = 0; j < columnNames.Length; j++)
                                    {
                                        XIIBO oBOI = new XIIBO();
                                        oBOI.BOD = oBOD;
                                        oBOI.SetAttribute("FKiTargetImportID", ID);
                                        oBOI.SetAttribute("FKiTargetDataID", RowID);
                                        oBOI.SetAttribute("sValue", row[columnNames[j]] == null ? null : row[columnNames[j]].ToString());
                                        oBOI.SetAttribute("sName", columnNames[j]);
                                        oBOI.SetAttribute("iStatus", "20");
                                        oCR = oBOI.Save(oBOI);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {

                                        }
                                        else
                                        {
                                            oCResult.sMessage = "Import_LeadFromExcelV2() : Error while saving into BSPTargetImportDetail for row:" + i + " and column:" + j;
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            SaveErrortoDB(oCResult);
                                        }
                                    }
                                }
                                else
                                {
                                    oCResult.sMessage = "Import_LeadFromExcelV2() : Error while saving into BSPTargetData for row:" + i;
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    SaveErrortoDB(oCResult);
                                }
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    oCResult.sMessage = "Import_LeadFromExcelV2() : Mandatory instance id not passed:" + iID + "-" + ID;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    SaveErrortoDB(oCResult);
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    //TODO: log error messages
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Import_TargetToObject(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Import leads from excel sheet into database";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                List<CNV> MappedNVs = new List<CNV>();
                XIDBO oTargetBOD = new XIDBO();
                var FKiSubscriptionID = 0;
                var ID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                var SelectedID = oParams.Where(m => m.sName.ToLower() == "SelectedID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iID = 0;
                int.TryParse(ID, out iID);
                if (iID > 0)
                {
                    var oImportI = oXI.BOI("BSLeadImport", iID.ToString());
                    if (oImportI != null && oImportI.Attributes.Count() > 0)
                    {
                        Guid FKiBODIDXIGUID = Guid.Empty;
                        var BODXIGUID = oImportI.AttributeI("FKiBODIDXIGUID").sValue;
                        FKiSubscriptionID = oImportI.AttributeI("FKiSubscriptionID").iValue;
                        oCResult.sMessage = "Import_TargetToObject() : Target object for import:" + BODXIGUID;
                        SaveErrortoDB(oCResult);
                        if (Guid.TryParse(BODXIGUID, out FKiBODIDXIGUID))
                        {
                            oTargetBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FKiBODIDXIGUID.ToString());
                            XID1Click o1Click = new XID1Click();
                            o1Click.BOIDXIGUID = new Guid("E45A3D8E-9F20-49AF-9C4D-494067D8AC2D");
                            o1Click.Query = "select * from XILeadImportMapping_T where FKiBODIDXIGUID = '" + FKiBODIDXIGUID + "'";
                            var Response = o1Click.OneClick_Run();
                            if (Response != null && Response.Count() > 0)
                            {
                                foreach (var items in Response.Values.ToList())
                                {
                                    string sType = string.Empty;
                                    var Type = items.AttributeI("iType").sValue;
                                    if (!string.IsNullOrEmpty(Type))
                                    {
                                        if (Type == "10")
                                        {
                                            sType = "unique";
                                        }
                                    }
                                    MappedNVs.Add(new CNV { sName = items.AttributeI("sName").sValue, sValue = items.AttributeI("FKiAttributeIDXIGUID").sValue, sLabel = items.AttributeI("sCode").sValue, sType = sType });
                                }
                            }
                        }
                    }
                    else
                    {
                        oCResult.sMessage = "Import_TargetToObject() : Error while loading BSLeadImport instance for:" + iID;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oCResult.sMessage = "Import_TargetToObject() : Mandatory instance id not passed:" + iID + "-" + ID;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    SaveErrortoDB(oCResult);
                }
                oCResult.sMessage = "Import_TargetToObject() : Mapping objects count: " + MappedNVs.Count() + " and SelectedID:" + SelectedID;
                SaveErrortoDB(oCResult);
                XIDStructure oXIDStructure = new XIDStructure();
                XID1Click o1ClickD = new XID1Click();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Target Data List");
                XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                if (o1ClickD != null && o1ClickD.XIGUID != null && o1ClickD.XIGUID != Guid.Empty)
                {
                    List<CNV> nParms = new List<CNV>();
                    nParms.Add(new CNV { sName = "{XIP|iInstanceID}", sValue = ID });
                    o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                    var oResult = o1ClickC.OneClick_Run();
                    oCResult.sMessage = "Import_TargetToObject() : Target Data List Query: " + o1ClickC.Query;
                    SaveErrortoDB(oCResult);
                    List<string> UniqueData = new List<string>();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPTargetData");
                    var oITBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPImportTargetMap");
                    var xSubTargetMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XSubscriptionTargetMap");
                    foreach (var oBOI in oResult.Values.ToList())
                    {
                        oBOI.BOD = oBOD;
                        oBOI.SetAttribute("iStatus", "30");
                        oBOI.Save(oBOI);
                        XIIBO oTargetBOI = new XIIBO();
                        oTargetBOI.BOD = oTargetBOD;
                        var DataID = oBOI.AttributeI("id").sValue;
                        XID1Click o1ClickDet = new XID1Click();
                        o1ClickDet = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Target Detail List");
                        XID1Click o1ClickCDet = (XID1Click)o1ClickDet.Clone(o1ClickDet);
                        nParms = new List<CNV>();
                        nParms.Add(new CNV { sName = "{XIP|iInstanceID}", sValue = DataID });
                        o1ClickCDet.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickCDet.Query, nParms);
                        oCResult.sMessage = "Import_TargetToObject() : Target Detail List Query: " + o1ClickCDet.Query;
                        SaveErrortoDB(oCResult);
                        var oDetails = o1ClickCDet.OneClick_Run();
                        bool bDuplicate = false;
                        bool bIncomplete = false;
                        string sTargetUniqueName = string.Empty;
                        string sTargetUniqueVal = string.Empty;
                        foreach (var oDetI in oDetails.Values.ToList())
                        {
                            var sName = oDetI.AttributeI("sName").sValue.Trim();
                            var sValue = oDetI.AttributeI("sValue").sValue.Trim();
                            if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                            {
                                var MapNV = MappedNVs.Where(m => m.sName.ToLower() == sName.ToLower()).FirstOrDefault();
                                if (MapNV != null)
                                {
                                    var AttrXIGUID = MapNV.sValue;
                                    Guid AttrGUID = Guid.Empty;
                                    if (Guid.TryParse(AttrXIGUID, out AttrGUID))
                                    {
                                        if (!string.IsNullOrEmpty(MapNV.sType) && MapNV.sType.ToLower() == "unique")
                                        {
                                            if (UniqueData.Contains(sValue))
                                            {
                                                bDuplicate = true;
                                            }
                                            else
                                            {
                                                UniqueData.Add(sValue);
                                            }
                                        }
                                        var oAttrD = oTargetBOD.Attributes.Values.ToList().Where(m => m.XIGUID == AttrGUID).FirstOrDefault();
                                        if (oAttrD != null)
                                        {
                                            oTargetBOI.SetAttribute(oAttrD.Name, sValue);
                                            if (!string.IsNullOrEmpty(MapNV.sType) && MapNV.sType.ToLower() == "unique")
                                            {
                                                sTargetUniqueName = oAttrD.Name;
                                                sTargetUniqueVal = sValue;
                                            }
                                        }
                                        else
                                        {
                                            oCResult.sMessage = "Import_TargetToObject() : Attribute not found for Attr:" + AttrXIGUID + " and data name:" + oAttrD.Name + ", value:" + sValue;
                                            SaveErrortoDB(oCResult);
                                        }
                                    }
                                    else
                                    {
                                        oCResult.sMessage = "Import_TargetToObject() : Attribute guid is not valid:" + AttrXIGUID;
                                        SaveErrortoDB(oCResult);
                                    }
                                }
                                else
                                {
                                    oCResult.sMessage = "Import_TargetToObject() : Mapping not found for column:" + sName;
                                    SaveErrortoDB(oCResult);
                                }
                            }
                            else
                            {
                                bIncomplete = true;
                            }
                        }
                        //oTargetBOI.SetAttribute("FKiImportID", ID);
                        if (!bDuplicate && !bIncomplete)
                        {
                            var XTargetID = 0;
                            List<CNV> oWhr = new List<CNV>();
                            oWhr.Add(new CNV { sName = sTargetUniqueName, sValue = sTargetUniqueVal });
                            var oTargetIns = oXI.BOI(oTargetBOD.Name, null, null, oWhr);
                            if (oTargetIns != null && oTargetIns.Attributes.Count() > 0)
                            {
                                var TargetID = oTargetIns.AttributeI("id").iValue;
                                if (TargetID > 0)
                                {
                                    XIIBO oITMap = new XIIBO();
                                    oITMap.BOD = oITBOD;
                                    oITMap.SetAttribute("FKiBODIDXIGUID", oTargetBOD.XIGUID.ToString());//Ex:Lead BO def XIGUID or Client BO def XIGUID
                                    oITMap.SetAttribute("FKiBOIID", TargetID.ToString());//Ex: Lead instance id or Client instance id
                                    oITMap.SetAttribute("FKiImportID", ID);//Import batch id
                                    oITMap.SetAttribute("iStatus", "10");
                                    oCR = oITMap.Save(oITMap);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oITMap = (XIIBO)oCR.oResult;
                                        XTargetID = oITMap.AttributeI("id").iValue;
                                    }
                                }
                                oBOI.SetAttribute("iStatus", "90");
                                oBOI.Save(oBOI);
                            }
                            else
                            {
                                oCR = oTargetBOI.Save(oTargetBOI, false);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTargetBOI = (XIIBO)oCR.oResult;
                                    var TargetID = oTargetBOI.AttributeI("id").iValue;
                                    if (TargetID > 0)
                                    {
                                        XIIBO oITMap = new XIIBO();
                                        oITMap.BOD = oITBOD;
                                        oITMap.SetAttribute("FKiBODIDXIGUID", oTargetBOD.XIGUID.ToString());//Ex:Lead BO def XIGUID or Client BO def XIGUID
                                        oITMap.SetAttribute("FKiBOIID", TargetID.ToString());//Ex: Lead instance id or Client instance id
                                        oITMap.SetAttribute("FKiImportID", ID);//Import batch id
                                        oITMap.SetAttribute("iStatus", "10");
                                        oCR = oITMap.Save(oITMap);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oITMap = (XIIBO)oCR.oResult;
                                            XTargetID = oITMap.AttributeI("id").iValue;
                                        }
                                    }
                                    oBOI.SetAttribute("iStatus", "10");
                                    oBOI.Save(oBOI);
                                }
                                else
                                {
                                    oCResult.sMessage = "Import_TargetToObject() : Error while saving into Target object : " + oTargetBOD.LabelName + " for Target data row:" + DataID;
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    SaveErrortoDB(oCResult);
                                    oBOI.SetAttribute("iStatus", "40");
                                    oBOI.Save(oBOI);
                                }
                            }
                            if (FKiSubscriptionID > 0 && XTargetID > 0)
                            {
                                XIIBO xSubTargetMapI = new XIIBO();
                                xSubTargetMapI.BOD = xSubTargetMapBOD;
                                xSubTargetMapI.SetAttribute("FKiSubscriptionID", FKiSubscriptionID.ToString());
                                xSubTargetMapI.SetAttribute("FKiTargetID", XTargetID.ToString());
                                xSubTargetMapI.SetAttribute("iStatus", "10");
                                oCR = xSubTargetMapI.Save(xSubTargetMapI);
                                if (oCR.bOK && oCR.oResult != null)
                                {

                                }
                            }
                        }
                        else if (bDuplicate)
                        {
                            oBOI.SetAttribute("iStatus", "70");
                            oBOI.Save(oBOI);
                        }

                        else if (bIncomplete)
                        {
                            oBOI.SetAttribute("iStatus", "80");
                            oBOI.Save(oBOI);
                        }
                    }
                }
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
                else
                {
                    //TODO: log error messages
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Import_LeadFromExcel(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Extract content from pdf document";//expalin about this method logic
            try
            {
                var ID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                XIIXI oXI = new XIIXI();
                var oDocI = oXI.BOI("Documents_T", ID);
                if (oDocI != null && oDocI.Attributes.Count() > 0)
                {
                    var sFullPath = oDocI.AttributeI("sfullPath").sValue;
                    if (!string.IsNullOrEmpty(sFullPath))
                    {

                        var sFilePath = HostingEnvironment.MapPath("~/UploadedFiles") + "//Files//xlsx//" + sFullPath;
                        DataTable rs = new DataTable();
                        string sSheetName = string.Empty;
                        using (var odConnection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sFilePath + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
                        {
                            odConnection.Open();
                            DataTable dtTablesList = odConnection.GetSchema("Tables");
                            if (dtTablesList.Rows.Count > 0)
                            {
                                //sSheetName = dtTablesList.Rows(0)("TABLE_NAME").ToString;
                            }
                            using (OleDbCommand cmd = new OleDbCommand())
                            {
                                cmd.Connection = odConnection;
                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.CommandText = "SELECT * FROM [Sheet1$]";
                                using (OleDbDataAdapter oleda = new OleDbDataAdapter(cmd))
                                {
                                    oleda.Fill(rs);
                                }
                            }
                            odConnection.Close();
                        }
                        foreach (DataRow row in rs.Rows)
                        {
                            Guid QSGuid = Guid.Empty;
                            oCR = Save_QSInstance(QSGuid);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var sQSIID = (string)oCR.oResult;
                                Guid.TryParse(sQSIID, out QSGuid);
                            }
                            List<CNV> oNV = new List<CNV>();
                            oNV.Add(new CNV { sName = "FirstName", sValue = row["FirstName"].ToString() });
                            oNV.Add(new CNV { sName = "LastName", sValue = row["LastName"].ToString() });
                            oNV.Add(new CNV { sName = "MobileNo", sValue = row["Mobile No"].ToString() });
                            oNV.Add(new CNV { sName = "PostCode", sValue = row["PostCode"].ToString() });
                            oNV.Add(new CNV { sName = "Email", sValue = row["Email"].ToString() });
                            oCR = Save_XIValues(QSGuid.ToString(), oNV);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oCR = Save_Lead(QSGuid.ToString(), oNV, "0");
                            }
                        }

                        //Create COM Objects.
                        //ExcelApp.Application excelApp = new ExcelApp.Application();
                        //ExcelApp.Workbook excelBook = excelApp.Workbooks.Open(HostingEnvironment.MapPath("~/UploadedFiles") + "//Files//xlsx//" + sFullPath);
                        //ExcelApp._Worksheet excelSheet = excelBook.Sheets[1];
                        //ExcelApp.Range excelRange = excelSheet.UsedRange;

                        //int rows = excelRange.Rows.Count;
                        //int cols = excelRange.Columns.Count;

                        //var XLRows = excelRange.Rows;

                        //for (int i = 2; i <= rows; i++)
                        //{
                        //    List<CNV> oNV = new List<CNV>();
                        //    oNV.Add(new CNV { sName = "FirstName", sValue = excelRange.Cells[i, 1].Value2.ToString() });
                        //    oNV.Add(new CNV { sName = "LastName", sValue = excelRange.Cells[i, 2].Value2.ToString() });
                        //    oNV.Add(new CNV { sName = "MobileNo", sValue = excelRange.Cells[i, 3].Value2.ToString() });
                        //    oNV.Add(new CNV { sName = "PostCode", sValue = excelRange.Cells[i, 4].Value2.ToString() });
                        //    oNV.Add(new CNV { sName = "Email", sValue = excelRange.Cells[i, 5].Value2.ToString() });
                        //    oCR = Save_XIValues(QSGuid.ToString(), oNV);
                        //    if (oCR.bOK && oCR.oResult != null)
                        //    {
                        //        oCR = Save_Lead(QSGuid.ToString(), oNV);
                        //    }
                        //}
                        ////after reading, release the excel project
                        //excelApp.Quit();
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = "Success";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Import_Leads(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Extract content from pdf document";//expalin about this method logic
            try
            {
                var iLeadImportID = oParams.Where(m => m.sName.ToLower() == "instanceid").Select(m => m.sValue).FirstOrDefault();
                var SelectedID = oParams.Where(m => m.sName.ToLower() == "SelectedID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                Guid QSDXIGUID = Guid.Empty;
                XIIXI oXI = new XIIXI();
                var oLeadImportI = oXI.BOI("ImportedLead", iLeadImportID);
                if (oLeadImportI != null && oLeadImportI.Attributes.Count() > 0)
                {
                    var FKiQSDID = oLeadImportI.AttributeI("FKiQuestionSetIDXIGUID").sValue;
                    Guid.TryParse(FKiQSDID, out QSDXIGUID);
                }
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ImportedLead");
                XIDStructure oXIDStructure = new XIDStructure();
                XID1Click o1ClickD = new XID1Click();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "ImportBatchLeads");
                XID1Click o1ClickC = new XID1Click();
                o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                List<CNV> nParms = new List<CNV>();
                nParms.Add(new CNV { sName = "{XIP|iInstanceID}", sValue = iLeadImportID });
                o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                if (!string.IsNullOrEmpty(SelectedID))
                {
                    o1ClickC.Query = o1ClickC.AddSearchParameters(o1ClickC.Query, "ID in (" + SelectedID + ")");
                }
                var oResult = o1ClickC.OneClick_Run();
                foreach (var oLead in oResult.Values.ToList())
                {
                    Guid QSGuid = Guid.Empty;
                    oCR = Save_QSInstance(QSDXIGUID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var sQSIID = (string)oCR.oResult;
                        Guid.TryParse(sQSIID, out QSGuid);
                    }
                    List<CNV> oNV = new List<CNV>();
                    oNV.Add(new CNV { sName = "FirstName", sValue = oLead.AttributeI("sFirstName").sValue });
                    oNV.Add(new CNV { sName = "LastName", sValue = oLead.AttributeI("sLastName").sValue });
                    oNV.Add(new CNV { sName = "MobileNo", sValue = oLead.AttributeI("sMob").sValue });
                    oNV.Add(new CNV { sName = "PostCode", sValue = oLead.AttributeI("sPostcode").sValue });
                    oNV.Add(new CNV { sName = "Email", sValue = oLead.AttributeI("sEmail").sValue });
                    oCR = Save_XIValues(QSGuid.ToString(), oNV);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oCR = Save_Lead(QSGuid.ToString(), oNV, iLeadImportID);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oLead.BOD = oBOD;
                            oLead.SetAttribute("iStatus", "10");
                            oCR = oLead.Save(oLead);
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = "Success";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Save_QSInstance(Guid QSDXIGUID)
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
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance");
                XIIBO oBOI = new XIIBO();
                oBOI.BOD = oBOD;
                oBOI.sBOName = "QS Instance";
                oBOI.SetAttribute("sName", "Lead import QS");
                if (QSDXIGUID == null || QSDXIGUID == Guid.Empty)
                {
                    var sQSDXIGUID = "6AF00009-F4E7-491A-80A6-EB29358C0134";
                    Guid.TryParse(sQSDXIGUID, out QSDXIGUID);
                }
                oBOI.SetAttribute("FKiQSDefinitionIDXIGUID", QSDXIGUID.ToString());
                oBOI.SetAttribute("iStage", "10");
                oCR = oBOI.Save(oBOI);
                oTrace.oTrace.Add(oCR.oTrace);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oBOI = (XIIBO)oCR.oResult;
                    var iQSIID = oBOI.AttributeI("xiguid").sValue;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = iQSIID;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
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

        public CResult Save_XIValues(string iQSIID, List<CNV> Data)
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
                XIInfraCache oCache = new XIInfraCache();
                XIIBO BulkInsert = new XIIBO();
                List<XIIBO> oBulkBO = new List<XIIBO>();
                XIIValue oFIns = new XIIValue();
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                foreach (var NV in Data)
                {
                    var FKiFieldOriginIDXIGUID = string.Empty;
                    var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, NV.sName.Replace(" ", ""));
                    if (oFieldD != null && oFieldD.ID > 0)
                    {
                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID.ToString();
                    }
                    else
                    {
                        XIIBO oFO = new XIIBO();
                        oFO.BOD = oFieldDBOD;
                        oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                        oFO.SetAttribute("sName", NV.sName.Replace(" ", ""));
                        oFO.SetAttribute("sDisplayName", NV.sName);
                        oCR = oFO.Save(oFO);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oFO = (XIIBO)oCR.oResult;
                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, NV.sName.Replace(" ", ""));
                            if (oFieldD != null && oFieldD.ID > 0)
                            {
                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID.ToString();
                            }
                        }
                    }
                    var sValue = NV.sValue;
                    XIIBO oBOI = new XIIBO();
                    oBOI.SetAttribute("sDerivedValue", sValue);
                    oBOI.SetAttribute("sValue", sValue);
                    oBOI.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                    oBOI.SetAttribute("FKiQSInstanceIDXIGUID", iQSIID.ToString());
                    oBulkBO.Add(oBOI);
                }
                if (oBulkBO != null && oBulkBO.Count() > 0)
                {
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T", null);
                    oBulkBO.ForEach(f => f.BOD = oBOD);
                    var MakeDatatble = BulkInsert.MakeBulkSqlTable(oBulkBO);
                    oCR = BulkInsert.SaveBulk(MakeDatatble, oBOD.iDataSourceXIGUID.ToString(), "XIFieldInstance_T");
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
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

        public CResult Save_Lead(string iQSIID, List<CNV> Data, string iLeadImportID)
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
                XIInfraCache oCache = new XIInfraCache();
                var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Lead_T");
                XIIBO oLeadI = new XIIBO();
                oLeadI.BOD = oFieldDBOD;
                oLeadI.SetAttribute("sFirstName", Data.Where(m => m.sName.ToLower() == "firstname").Select(m => m.sValue).FirstOrDefault());
                oLeadI.SetAttribute("sLastName", Data.Where(m => m.sName.ToLower() == "lastname").Select(m => m.sValue).FirstOrDefault());
                oLeadI.SetAttribute("sMob", Data.Where(m => m.sName.ToLower() == "mobileno").Select(m => m.sValue).FirstOrDefault());
                oLeadI.SetAttribute("sPostCode", Data.Where(m => m.sName.ToLower() == "postcode").Select(m => m.sValue).FirstOrDefault());
                oLeadI.SetAttribute("sEmail", Data.Where(m => m.sName.ToLower() == "email").Select(m => m.sValue).FirstOrDefault());
                oLeadI.SetAttribute("FKiQSInstanceIDXIGUID", iQSIID);
                oLeadI.SetAttribute("iStatus", "0");
                oLeadI.SetAttribute("FKiImportID", iLeadImportID);
                oCR = oLeadI.Save(oLeadI, false);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Success";
                }
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

        public static CResult DynamicDefinitionV1(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            //oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> oTraceInfo = new List<CNV>();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                bool bDefDone = false;
                int iQSDID = 0;
                Guid QSDXIGUID = Guid.Empty;
                var iBODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iboiid").Select(m => m.sValue).FirstOrDefault();
                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter definition uid is: " + iBODID + " and instance uid is: " + iBOIID });
                if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iBOIID))//check mandatory params are passed or not
                {
                    XIConfigs oConfig = new XIConfigs();
                    Guid QSDStepXIGUID = Guid.Empty;
                    int iQSStepDID = 0;
                    Guid QSDSecXIGUID = Guid.Empty;
                    int iQSSecDID = 0;
                    var sSubject = string.Empty;
                    var sBody = string.Empty;
                    var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                    var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        sSubject = oBOI.AttributeI("sHeader").sValue;
                        if (sSubject.Length >= 128)
                        {
                            sSubject = sSubject.Substring(0, 127);
                        }
                        sBody = oBOI.AttributeI("sContentPlain").sValue;
                        if (!string.IsNullOrEmpty(sSubject))
                        {
                            //var iIndex = sSubject.IndexOf("Leads2Market");
                            var sName = sSubject.Trim();
                            //sName = sName.Replace("Leads2Market - ", "");
                            oTraceInfo.Add(new CNV { sValue = "QS definition code is: " + sName });

                            XIDQS oQSD = new XIDQS();
                            oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, sName);
                            if (oQSD != null && oQSD.XIGUID != null && oQSD.XIGUID != Guid.Empty)
                            {
                                QSDXIGUID = oQSD.XIGUID;
                                if (oQSD.Steps != null && oQSD.Steps.Values.FirstOrDefault() != null)
                                {
                                    QSDStepXIGUID = oQSD.Steps.Values.FirstOrDefault().XIGUID;
                                    if (oQSD.Steps.Values.FirstOrDefault().Sections != null && oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault() != null)
                                    {
                                        QSDSecXIGUID = oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault().XIGUID;
                                    }
                                }
                            }
                            else
                            {
                                oQSD.sName = sName;
                                oQSD.sDescription = sName;
                                oCR = oConfig.Save_QuestionSet(oQSD);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var oQSDBOI = (XIIBO)oCR.oResult;
                                    var QSDUID = oQSDBOI.AttributeI("xiguid").sValue;
                                    Guid.TryParse(QSDUID, out QSDXIGUID);
                                    if (QSDXIGUID == null || QSDXIGUID == Guid.Empty)
                                    {
                                        var QSDID = oQSDBOI.AttributeI("id").sValue;
                                        int.TryParse(QSDID, out iQSDID);
                                    }
                                    oTraceInfo.Add(new CNV { sValue = "QS definition saved: " + QSDXIGUID });
                                    XIDQSStep oStepD = new XIDQSStep();
                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                    oStepD.FKiQSDefintionID = iQSDID;
                                    oStepD.sName = "Step1";
                                    oStepD.sDisplayName = "Step";
                                    oStepD.iDisplayAs = 20;
                                    oStepD.StatusTypeID = 10;
                                    oStepD.bIsMerge = true;
                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                        {
                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                            int.TryParse(QSStepDID, out iQSStepDID);
                                        }
                                        oTraceInfo.Add(new CNV { sValue = "QS step definition saved: " + QSDStepXIGUID });
                                        XIDQSSection oSecD = new XIDQSSection();
                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                        oSecD.iDisplayAs = 30;
                                        oSecD.sIsHidden = "off";
                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                            {
                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                int.TryParse(QSSecDID, out iQSSecDID);
                                            }
                                            oTraceInfo.Add(new CNV { sValue = "QS step section definition saved: " + QSDSecXIGUID });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string sFname = string.Empty;
                    string sLname = string.Empty;
                    string sMob = string.Empty;
                    string sEmail = string.Empty;
                    string sPostcode = string.Empty;
                    List<CNV> oRisk = new List<CNV>();
                    if ((iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)) && (iQSStepDID > 0 || (QSDStepXIGUID != null && QSDStepXIGUID != Guid.Empty)) && (iQSSecDID > 0 || (QSDSecXIGUID != null && QSDSecXIGUID != Guid.Empty)))
                    {
                        if (!string.IsNullOrEmpty(sBody))
                        {
                            string[] NVs = Regex.Split(sBody.Trim(), @"\s{2,}");
                            string[] NVs2 = Regex.Split(sBody.Trim(), @" {2,8}");
                            string[] RegNvs2 = Regex.Split(sBody, @"  ");
                            var NVs1 = sBody.Trim().Split(new string[] { "  " }, StringSplitOptions.None).ToList();
                            var SplitCount = "Nvs:" + NVs.Count() + "-Nvs2:" + NVs2.Count() + "-RegNvs2:" + RegNvs2.Count() + "-NVs1:" + NVs1.Count();
                            oTraceInfo.Add(new CNV { sValue = SplitCount });
                            if (NVs != null && NVs.Count() > 0)
                            {
                                foreach (var NV in NVs)
                                {
                                    if (NV.Contains(':'))
                                    {
                                        var Splits = NV.Split(':').ToList();
                                        var sName = Splits[0].Trim();
                                        var sValue = Splits[1].Trim();
                                        if (sName.ToLower() == "full name" || sName.ToLower() == "first name")
                                        {
                                            sFname = sValue;
                                        }
                                        if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone")
                                        {
                                            sMob = sValue;
                                        }
                                        if (sName.ToLower() == "last name")
                                        {
                                            sLname = sValue;
                                        }
                                        if (sName.ToLower() == "postcode")
                                        {
                                            sPostcode = sValue;
                                        }
                                        if (sName.ToLower() == "email")
                                        {
                                            sEmail = sValue;
                                        }
                                        var FKiFieldOriginIDXIGUID = Guid.Empty;
                                        var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                        if (oFieldD != null && oFieldD.ID > 0)
                                        {
                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                        }
                                        else
                                        {
                                            XIIBO oFO = new XIIBO();
                                            oFO.BOD = oFieldDBOD;
                                            oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                            oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                            oFO.SetAttribute("sDisplayName", sName);
                                            oCR = oFO.Save(oFO);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oFO = (XIIBO)oCR.oResult;
                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                if (oFieldD != null && oFieldD.ID > 0)
                                                {
                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                }
                                                else
                                                {
                                                    XIIBO oFDI = new XIIBO();
                                                    List<CNV> oWhr = new List<CNV>();
                                                    oWhr.Add(new CNV { sName = "sName", sValue = sName.Replace(" ", "") });
                                                    oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                    var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                    Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                }
                                            }
                                        }
                                        XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                        oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                        oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                        oFieldDef.FKiXIStepDefinitionIDXIGUID = QSDStepXIGUID;
                                        oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                        oFieldDef.FKiXIStepDefinitionID = iQSStepDID;
                                        oFieldDef.FKiStepSectionID = iQSSecDID;
                                        oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oTraceInfo.Add(new CNV { sValue = "Field definition saved for Field: " + sName });
                                        }
                                        else
                                        {
                                            oTraceInfo.Add(new CNV { sValue = "Field definition not saved for Field: " + sName });
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }
                                        oTraceInfo.Add(new CNV { sValue = "NV: " + NV + " and Risk value for field " + sName + ": is " + sValue });
                                        oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = FKiFieldOriginIDXIGUID.ToString() });
                                    }
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "NVs not found in Body" });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV { sValue = "Body is null for instance: " + iBOIID });
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                    {
                        bDefDone = true;
                    }
                    if (bDefDone && (iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)))
                    {
                        //save qs instance
                        string sGUID = Guid.NewGuid().ToString();
                        XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString(), "", sGUID, 0, 0);
                        oTraceInfo.Add(new CNV { sValue = "QS Definition loaded from Cache" });
                        XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                        oCR = oXI.CreateQSI(null, QSDXIGUID.ToString(), null, null, 0, 0, null, 0, null, 0, sGUID);
                        oTraceInfo.Add(new CNV { sValue = "QS Instance is created" });
                        var oQSInstance = (XIIQS)oCR.oResult;
                        oQSInstance.QSDefinition = oQSDC;
                        //Guid iActiveStepIDXIGUID = oQSDC.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault().XIGUID;
                        //oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepIDXIGUID.ToString(), sGUID);
                        oTraceInfo.Add(new CNV { sValue = "Step instance loaded" });
                        var RiskBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                        if (oRisk != null && oRisk.Count() > 0)
                        {
                            foreach (var risk in oRisk)
                            {
                                XIIBO oRiskI = new XIIBO();
                                oRiskI.BOD = RiskBOD;
                                oRiskI.SetAttribute("sDerivedValue", risk.sValue);
                                oRiskI.SetAttribute("sValue", risk.sValue);
                                oRiskI.SetAttribute("FKiFieldOriginIDXIGUID", risk.sType.ToString());
                                //oRiskI.SetAttribute("FKiStepInstanceIDXIGUID", oQSInstance.Steps.Values.ToList().FirstOrDefault().XIGUID.ToString());
                                oRiskI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                                oRiskI.Save(oRiskI);
                            }
                        }

                        //Save Lead
                        var oLeadBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Lead");
                        XIIBO oLeadI = new XIIBO();
                        oLeadI.BOD = oLeadBOD;
                        oLeadI.SetAttribute("sFirstName", sFname);
                        oLeadI.SetAttribute("sLastName", sLname);
                        oLeadI.SetAttribute("sMob", sMob);
                        oLeadI.SetAttribute("sPostCode", sPostcode);
                        oLeadI.SetAttribute("sEmail", sEmail);
                        oLeadI.SetAttribute("iStatus", "0");
                        oLeadI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                        oCR = oLeadI.Save(oLeadI, false);
                        if (oCR.bOK && oCR.oResult != null)
                        { }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTraceInfo.Add(new CNV { sValue = "Mandatory Params: iBODID " + iBODID + " or iBOIID:" + iBOIID + " are missing" });
                }
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = QSDXIGUID;
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
                oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public static CResult DynamicDefinition1(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            //oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> oTraceInfo = new List<CNV>();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                bool bDefDone = false;
                int iQSDID = 0;
                Guid QSDXIGUID = Guid.Empty;
                var iBODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iboiid").Select(m => m.sValue).FirstOrDefault();
                oTraceInfo.Add(new CNV { sValue = "DynamicDefinition Method called for Mandatory parameter definition uid is: " + iBODID + " and instance uid is: " + iBOIID });
                if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iBOIID))//check mandatory params are passed or not
                {
                    XIConfigs oConfig = new XIConfigs();
                    Guid QSDStepXIGUID = Guid.Empty;
                    int iQSStepDID = 0;
                    Guid QSDSecXIGUID = Guid.Empty;
                    int iQSSecDID = 0;
                    var sSubject = string.Empty;
                    var sBody = string.Empty;
                    var sFrom = string.Empty;
                    var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                    var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        sSubject = oBOI.AttributeI("sHeader").sValue;
                        sFrom = oBOI.AttributeI("sFrom").sValue;
                        if (sFrom.Contains('<'))
                        {
                            var sFromIndex = sFrom.IndexOf("<");
                            sFrom = sFrom.Substring(sFromIndex + 1, sFrom.Length - sFromIndex - 1);
                            if (sFrom.EndsWith(">"))
                            {
                                sFrom = sFrom.Substring(0, sFrom.Length - 1);
                            }
                        }
                        if (sSubject.Length >= 128)
                        {
                            sSubject = sSubject.Substring(0, 127);
                        }
                        //sBody = oBOI.AttributeI("sContent").sValue;
                        if (!string.IsNullOrEmpty(sSubject))
                        {
                            //var iIndex = sSubject.IndexOf("Leads2Market");
                            var sName = sSubject.Trim();
                            //sName = sName.Replace("Leads2Market - ", "");
                            oTraceInfo.Add(new CNV { sValue = "QS definition code is: " + sName });

                            XIDQS oQSD = new XIDQS();
                            oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, sName);
                            if (oQSD != null && oQSD.XIGUID != null && oQSD.XIGUID != Guid.Empty)
                            {
                                QSDXIGUID = oQSD.XIGUID;
                                if (oQSD.Steps != null && oQSD.Steps.Values.FirstOrDefault() != null)
                                {
                                    QSDStepXIGUID = oQSD.Steps.Values.FirstOrDefault().XIGUID;
                                    if (oQSD.Steps.Values.FirstOrDefault().Sections != null && oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault() != null)
                                    {
                                        QSDSecXIGUID = oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault().XIGUID;
                                    }
                                }
                            }
                            else
                            {
                                oQSD.sName = sName;
                                oQSD.sDescription = sName;
                                oCR = oConfig.Save_QuestionSet(oQSD);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var oQSDBOI = (XIIBO)oCR.oResult;
                                    var QSDUID = oQSDBOI.AttributeI("xiguid").sValue;
                                    Guid.TryParse(QSDUID, out QSDXIGUID);
                                    if (QSDXIGUID == null || QSDXIGUID == Guid.Empty)
                                    {
                                        var QSDID = oQSDBOI.AttributeI("id").sValue;
                                        int.TryParse(QSDID, out iQSDID);
                                    }
                                    oTraceInfo.Add(new CNV { sValue = "QS definition saved: " + QSDXIGUID });
                                    XIDQSStep oStepD = new XIDQSStep();
                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                    oStepD.FKiQSDefintionID = iQSDID;
                                    oStepD.sName = "Step1";
                                    oStepD.sDisplayName = "Step";
                                    oStepD.iDisplayAs = 20;
                                    oStepD.StatusTypeID = 10;
                                    oStepD.bIsMerge = true;
                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                        {
                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                            int.TryParse(QSStepDID, out iQSStepDID);
                                        }
                                        oTraceInfo.Add(new CNV { sValue = "QS step definition saved: " + QSDStepXIGUID });
                                        XIDQSSection oSecD = new XIDQSSection();
                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                        oSecD.iDisplayAs = 30;
                                        oSecD.sIsHidden = "off";
                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                            {
                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                int.TryParse(QSSecDID, out iQSSecDID);
                                            }
                                            oTraceInfo.Add(new CNV { sValue = "QS step section definition saved: " + QSDSecXIGUID });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string sFname = string.Empty;
                    string sLname = string.Empty;
                    string sMob = string.Empty;
                    string sEmail = string.Empty;
                    string sPostcode = string.Empty;
                    List<CNV> oRisk = new List<CNV>();
                    if ((iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)) && (iQSStepDID > 0 || (QSDStepXIGUID != null && QSDStepXIGUID != Guid.Empty)) && (iQSSecDID > 0 || (QSDSecXIGUID != null && QSDSecXIGUID != Guid.Empty)))
                    {
                        if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("PC NCB Dream Cars"))
                        {
                            sBody = oBOI.AttributeI("sContent").sValue;
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                var TRContent = Regex.Matches(sBody.Trim(), @"(?<1><TR[^>]*>\s*<td.*?</tr>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                foreach (var tr in TRContent)
                                {
                                    string sName = string.Empty;
                                    string sValue = string.Empty;
                                    var trContent = tr.ToString();
                                    var tdRegex = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";
                                    var tdContent = Regex.Matches(trContent, tdRegex, RegexOptions.IgnoreCase);
                                    if (tdContent.Count > 1)
                                    {
                                        var tdContent1 = tdContent[0].ToString();
                                        var tdContent2 = tdContent[1].ToString();
                                        if (!string.IsNullOrEmpty(tdContent1) && !string.IsNullOrEmpty(tdContent2))
                                        {
                                            if (tdContent1.Contains("rgb(237, 244, 255)") || tdContent1.Contains("rgb(247, 250, 255)"))
                                            {
                                                if (tdContent2.Contains("rgb(200, 223, 179)") || tdContent2.Contains("rgb(237, 244, 255)") || tdContent2.Contains("rgb(247, 250, 255)"))
                                                {
                                                    var SpanRegex = @"<span\b[^>]*?>(?<V>[\s\S]*?)</\s*span>";
                                                    var td1SpanContent = Regex.Matches(tdContent1, SpanRegex, RegexOptions.IgnoreCase);
                                                    if (td1SpanContent.Count > 0)
                                                    {
                                                        var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                        var sNameMatch = regex.Match(td1SpanContent[0].ToString());
                                                        if (sNameMatch.Groups.Count > 2)
                                                        {
                                                            sName = sNameMatch.Groups[2].Value;
                                                        }

                                                    }
                                                    var td2SpanContent = Regex.Matches(tdContent2, SpanRegex, RegexOptions.IgnoreCase);
                                                    if (td2SpanContent.Count > 0)
                                                    {
                                                        var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                        var sValueMatch = regex.Match(td2SpanContent[0].ToString());
                                                        if (sValueMatch.Groups.Count > 2)
                                                        {
                                                            sValue = sValueMatch.Groups[2].Value;
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                    {
                                                        if (sName.ToLower() == "full name" || sName.ToLower() == "first name")
                                                        {
                                                            sFname = sValue;
                                                        }
                                                        if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone")
                                                        {
                                                            sMob = sValue;
                                                        }
                                                        if (sName.ToLower() == "last name")
                                                        {
                                                            sLname = sValue;
                                                        }
                                                        if (sName.ToLower() == "postcode")
                                                        {
                                                            sPostcode = sValue;
                                                        }
                                                        if (sName.ToLower() == "email")
                                                        {
                                                            sEmail = sValue;
                                                        }
                                                        var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                        var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                        if (oFieldD != null && oFieldD.ID > 0)
                                                        {
                                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                        }
                                                        else
                                                        {
                                                            XIIBO oFO = new XIIBO();
                                                            oFO.BOD = oFieldDBOD;
                                                            oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                            oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                            oFO.SetAttribute("sDisplayName", sName);
                                                            oCR = oFO.Save(oFO);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oFO = (XIIBO)oCR.oResult;
                                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                if (oFieldD != null && oFieldD.ID > 0)
                                                                {
                                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                }
                                                                else
                                                                {
                                                                    XIIBO oFDI = new XIIBO();
                                                                    List<CNV> oWhr = new List<CNV>();
                                                                    oWhr.Add(new CNV { sName = "sName", sValue = sName.Replace(" ", "") });
                                                                    oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                    var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                    Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                }
                                                            }
                                                        }
                                                        XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                        oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                        oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                        oFieldDef.FKiXIStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                        oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                        oFieldDef.FKiXIStepDefinitionID = iQSStepDID;
                                                        oFieldDef.FKiStepSectionID = iQSSecDID;
                                                        oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oTraceInfo.Add(new CNV { sValue = "Field definition saved for Field: " + sName });
                                                        }
                                                        else
                                                        {
                                                            oTraceInfo.Add(new CNV { sValue = "Field definition not saved for Field: " + sName });
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                        }
                                                        //oTraceInfo.Add(new CNV { sValue = "NV: " + NV + " and Risk value for field " + sName + ": is " + sValue });
                                                        oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = FKiFieldOriginIDXIGUID.ToString() });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Body is null for instance: " + iBOIID });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            sBody = oBOI.AttributeI("sContentPlain").sValue;
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                string[] NVs = Regex.Split(sBody.Trim(), @"\s{2,}");
                                string[] NVs2 = Regex.Split(sBody.Trim(), @" {2,8}");
                                string[] RegNvs2 = Regex.Split(sBody, @"  ");
                                var NVs1 = sBody.Trim().Split(new string[] { "  " }, StringSplitOptions.None).ToList();
                                var SplitCount = "Nvs:" + NVs.Count() + "-Nvs2:" + NVs2.Count() + "-RegNvs2:" + RegNvs2.Count() + "-NVs1:" + NVs1.Count();
                                oTraceInfo.Add(new CNV { sValue = SplitCount });
                                if (NVs != null && NVs.Count() > 0)
                                {
                                    foreach (var NV in NVs)
                                    {
                                        if (NV.Contains(':'))
                                        {
                                            var Splits = NV.Split(':').ToList();
                                            var sName = Splits[0].Trim();
                                            var sValue = Splits[1].Trim();
                                            if (sName.ToLower() == "full name" || sName.ToLower() == "first name")
                                            {
                                                sFname = sValue;
                                            }
                                            if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone")
                                            {
                                                sMob = sValue;
                                            }
                                            if (sName.ToLower() == "last name")
                                            {
                                                sLname = sValue;
                                            }
                                            if (sName.ToLower() == "postcode")
                                            {
                                                sPostcode = sValue;
                                            }
                                            if (sName.ToLower() == "email")
                                            {
                                                sEmail = sValue;
                                            }
                                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                                            var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                            if (oFieldD != null && oFieldD.ID > 0)
                                            {
                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                            }
                                            else
                                            {
                                                XIIBO oFO = new XIIBO();
                                                oFO.BOD = oFieldDBOD;
                                                oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                oFO.SetAttribute("sDisplayName", sName);
                                                oCR = oFO.Save(oFO);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    oFO = (XIIBO)oCR.oResult;
                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                    {
                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                    }
                                                    else
                                                    {
                                                        XIIBO oFDI = new XIIBO();
                                                        List<CNV> oWhr = new List<CNV>();
                                                        oWhr.Add(new CNV { sName = "sName", sValue = sName.Replace(" ", "") });
                                                        oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                        var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                        Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                    }
                                                }
                                            }
                                            XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                            oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                            oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                            oFieldDef.FKiXIStepDefinitionIDXIGUID = QSDStepXIGUID;
                                            oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                            oFieldDef.FKiXIStepDefinitionID = iQSStepDID;
                                            oFieldDef.FKiStepSectionID = iQSSecDID;
                                            oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "Field definition saved for Field: " + sName });
                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "Field definition not saved for Field: " + sName });
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            }
                                            oTraceInfo.Add(new CNV { sValue = "NV: " + NV + " and Risk value for field " + sName + ": is " + sValue });
                                            oRisk.Add(new CNV { sName = sName, sValue = sValue, sType = FKiFieldOriginIDXIGUID.ToString() });
                                        }
                                    }
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV { sValue = "NVs not found in Body" });
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Body is null for instance: " + iBOIID });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "Data parsing falied for:" + QSDXIGUID + "_" + QSDStepXIGUID + "_" + QSDSecXIGUID });
                    }
                    if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                    {
                        bDefDone = true;
                    }
                    string sLeadInfo = sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail;
                    if (bDefDone && (iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)))
                    {
                        //save qs instance
                        string sGUID = Guid.NewGuid().ToString();
                        XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString(), "", sGUID, 0, 0);
                        oTraceInfo.Add(new CNV { sValue = "QS Definition loaded from Cache" });
                        XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                        oCR = oXI.CreateQSI(null, QSDXIGUID.ToString(), null, null, 0, 0, null, 0, null, 0, sGUID);
                        oTraceInfo.Add(new CNV { sValue = "QS Instance is created" });
                        var oQSInstance = (XIIQS)oCR.oResult;
                        oQSInstance.QSDefinition = oQSDC;
                        //Guid iActiveStepIDXIGUID = oQSDC.Steps.Values.ToList().OrderBy(m => m.iOrder).FirstOrDefault().XIGUID;
                        //oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepIDXIGUID.ToString(), sGUID);
                        oTraceInfo.Add(new CNV { sValue = "Step instance loaded" });
                        var RiskBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                        if (oRisk != null && oRisk.Count() > 0)
                        {
                            foreach (var risk in oRisk)
                            {
                                XIIBO oRiskI = new XIIBO();
                                oRiskI.BOD = RiskBOD;
                                oRiskI.SetAttribute("sDerivedValue", risk.sValue);
                                oRiskI.SetAttribute("sValue", risk.sValue);
                                oRiskI.SetAttribute("FKiFieldOriginIDXIGUID", risk.sType.ToString());
                                //oRiskI.SetAttribute("FKiStepInstanceIDXIGUID", oQSInstance.Steps.Values.ToList().FirstOrDefault().XIGUID.ToString());
                                oRiskI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                                oRiskI.Save(oRiskI);
                            }
                        }
                        oTraceInfo.Add(new CNV { sValue = "Lead creation statred for:" + sLeadInfo + "-" + oQSInstance.XIGUID });
                        //Save Lead
                        var oLeadBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Lead");
                        XIIBO oLeadI = new XIIBO();
                        oLeadI.BOD = oLeadBOD;
                        oLeadI.SetAttribute("sFirstName", sFname);
                        oLeadI.SetAttribute("sLastName", sLname);
                        oLeadI.SetAttribute("sMob", sMob);
                        oLeadI.SetAttribute("sPostCode", sPostcode);
                        oLeadI.SetAttribute("sEmail", sEmail);
                        oLeadI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                        oLeadI.SetAttribute("iStatus", "0");
                        oCR = oLeadI.Save(oLeadI, false);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            int iLeadID = 0;
                            oLeadI = (XIIBO)oCR.oResult;
                            var LeadID = oLeadI.AttributeI("id").sValue;
                            int.TryParse(LeadID, out iLeadID);
                            if (iLeadID > 0)
                            {
                                oTraceInfo.Add(new CNV { sValue = "Lead creation success for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID });
                                //Link communication to Lead
                                var CommMatchBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XCommunicationMatch");
                                XIIBO oCommMatch = new XIIBO();
                                oCommMatch.BOD = CommMatchBOD;
                                oCommMatch.SetAttribute("FKiBODID", "717");
                                oCommMatch.SetAttribute("FKiBOIID", iLeadID.ToString());
                                oCommMatch.SetAttribute("FKiCommunicationID", iBOIID);
                                oCR = oCommMatch.Save(oCommMatch);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTraceInfo.Add(new CNV { sValue = "Communication linked successfully to Lead" });
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV { sValue = "Communication linking to Lead is falied" });
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID });
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV { sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "QS instance creation falied for:" + sLeadInfo });
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTraceInfo.Add(new CNV { sValue = "Mandatory Params: iBODID " + iBODID + " or iBOIID:" + iBOIID + " are missing" });
                }
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = QSDXIGUID;
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
                oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public  CResult DynamicDefinition(List<CNV> oParams, iSiganlR oSignalR)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";
            List<CNV> oTraceInfo = new List<CNV>();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                bool bDefDone = false;
                int iQSDID = 0;
                Guid QSDXIGUID = Guid.Empty;
                Guid QSXIGUID = Guid.Empty;
                bool bPreDefinedQSD = false;
                int FKiDefaultStepDID = 0;
                Guid FKiDefaultStepDIDXIGUID = Guid.Empty;
                List<CNV> MappedNVs = new List<CNV>();
                var iBODID = oParams.Where(m => m.sName.ToLower() == "ibodid").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iboiid").Select(m => m.sValue).FirstOrDefault();
                oTraceInfo.Add(new CNV
                {
                    sValue = "DynamicDefinition Method called for Mandatory parameter definition uid is: " + iBODID + " and instance uid is: " + iBOIID
                });
                if (!string.IsNullOrEmpty(iBODID) && !string.IsNullOrEmpty(iBOIID))
                {
                    XIConfigs oConfig = new XIConfigs();
                    Guid QSDStepXIGUID = Guid.Empty;
                    int iQSStepDID = 0;
                    Guid QSDSecXIGUID = Guid.Empty;
                    int iQSSecDID = 0;
                    var sSubject = string.Empty;
                    var sBody = string.Empty;
                    var sFrom = string.Empty;
                    var iSource = string.Empty;
                    var oFieldDBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldOrigin_T");
                    var oMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XILeadImportMapping");
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                    var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        sSubject = oBOI.AttributeI("sHeader").sValue;
                        sFrom = oBOI.AttributeI("sFrom").sValue;
                        if (sFrom.Contains('<'))
                        {
                            var sFromIndex = sFrom.IndexOf("<");
                            sFrom = sFrom.Substring(sFromIndex + 1, sFrom.Length - sFromIndex - 1);
                            if (sFrom.EndsWith(">"))
                            {
                                sFrom = sFrom.Substring(0, sFrom.Length - 1);
                            }
                        }
                        if (sSubject.Length >= 128)
                        {
                            sSubject = sSubject.Substring(0, 127);
                        }
                        if (!string.IsNullOrEmpty(sSubject))
                        {
                            var sName = sSubject.Trim();
                            XID1Click o1Click = new XID1Click();
                            o1Click.BOIDXIGUID = new Guid("77227E41-917A-4476-A549-BEBB6D3A593E");
                            o1Click.Query = "select * from XIGenericMapping_T";
                            var Response = o1Click.OneClick_Run();
                            if (Response != null && Response.Count() > 0)
                            {
                                foreach (var map in Response.Values.ToList())
                                {
                                    var sMapName = map.AttributeI("sName").sValue;
                                    if (sSubject.ToLower().StartsWith(sMapName.ToLower()))
                                    {
                                        var sQSDXIGUID = map.AttributeI("sValue").sValue;
                                        iSource = map.AttributeI("FKiSourceID").sValue;
                                        if (Guid.TryParse(sQSDXIGUID, out QSXIGUID))
                                        {
                                            bPreDefinedQSD = true;
                                        }
                                        else if (!string.IsNullOrEmpty(sQSDXIGUID))
                                        {
                                            bPreDefinedQSD = true;
                                            sName = sQSDXIGUID;
                                        }
                                    }
                                }
                            }
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "QS definition code is: " + sName
                            });
                            XIDQS oQSD = new XIDQS();
                            if (QSXIGUID != null && QSXIGUID != Guid.Empty)
                            {
                                oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSXIGUID.ToString());
                            }
                            else
                            {
                                oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, sName);
                            }
                            if (oQSD != null && oQSD.XIGUID != null && oQSD.XIGUID != Guid.Empty)
                            {
                                QSDXIGUID = oQSD.XIGUID;
                                QSXIGUID = oQSD.XIGUID;
                                if (oQSD.Steps != null && oQSD.Steps.Values.FirstOrDefault() != null)
                                {
                                    QSDStepXIGUID = oQSD.Steps.Values.FirstOrDefault().XIGUID;
                                    if (oQSD.Steps.Values.FirstOrDefault().Sections != null && oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault() != null)
                                    {
                                        QSDSecXIGUID = oQSD.Steps.Values.FirstOrDefault().Sections.Values.FirstOrDefault().XIGUID;
                                    }
                                }
                                if (oQSD.FKiDefaultStepDIDXIGUID == null || oQSD.FKiDefaultStepDIDXIGUID == Guid.Empty)
                                {
                                    var DefaultStep = oQSD.Steps.Values.ToList().Where(m => m.sName.ToLower() == "Default Step".ToLower()).FirstOrDefault();
                                    if (DefaultStep != null && DefaultStep.XIGUID != null && DefaultStep.XIGUID != Guid.Empty)
                                    {
                                        FKiDefaultStepDIDXIGUID = DefaultStep.XIGUID;
                                        var DefaultSecD = DefaultStep.Sections.Values.ToList().FirstOrDefault();
                                        if (DefaultSecD != null && DefaultSecD.XIGUID != null && DefaultSecD.XIGUID != Guid.Empty)
                                        {
                                            QSDSecXIGUID = DefaultSecD.XIGUID;
                                            iQSSecDID = DefaultSecD.ID;
                                        }
                                    }
                                }
                                else FKiDefaultStepDIDXIGUID = oQSD.FKiDefaultStepDIDXIGUID;
                            }
                            else
                            {
                                oQSD.sName = sName;
                                oQSD.sDescription = sName;
                                oCR = oConfig.Save_QuestionSet(oQSD);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var oQSDBOI = (XIIBO)oCR.oResult;
                                    var QSDUID = oQSDBOI.AttributeI("xiguid").sValue;
                                    Guid.TryParse(QSDUID, out QSDXIGUID);
                                    if (QSDXIGUID == null || QSDXIGUID == Guid.Empty)
                                    {
                                        var QSDID = oQSDBOI.AttributeI("id").sValue;
                                        int.TryParse(QSDID, out iQSDID);
                                    }
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "QS definition saved: " + QSDXIGUID
                                    });
                                    XIDQSStep oStepD = new XIDQSStep();
                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                    oStepD.FKiQSDefintionID = iQSDID;
                                    oStepD.sName = "Step1";
                                    oStepD.sDisplayName = "Step";
                                    oStepD.iDisplayAs = 20;
                                    oStepD.StatusTypeID = 10;
                                    oStepD.bIsMerge = true;
                                    oStepD.sIsHidden = "off";
                                    oStepD.bIsCopy = true;
                                    oStepD.bIsHistory = true;
                                    oStepD.bIsSaveNext = true;
                                    oStepD.bIsBack = true;
                                    oStepD.bIsReload = true;
                                    oStepD.bIsAutoSaving = true;
                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                        {
                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                            int.TryParse(QSStepDID, out iQSStepDID);
                                        }
                                        oTraceInfo.Add(new CNV
                                        {
                                            sValue = "QS step definition saved: " + QSDStepXIGUID
                                        });
                                        XIDQSSection oSecD = new XIDQSSection();
                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                        oSecD.iDisplayAs = 30;
                                        oSecD.sIsHidden = "off";
                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                            {
                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                int.TryParse(QSSecDID, out iQSSecDID);
                                            }
                                            oTraceInfo.Add(new CNV
                                            {
                                                sValue = "QS step section definition saved: " + QSDSecXIGUID
                                            });
                                        }
                                    }
                                }
                            }
                            if (bPreDefinedQSD)
                            {
                                o1Click = new XID1Click();
                                o1Click.BOIDXIGUID = new Guid("E45A3D8E-9F20-49AF-9C4D-494067D8AC2D");
                                o1Click.Query = "select * from XILeadImportMapping_T where FKiQSDIDXIGUID = '" + QSXIGUID + "'";
                                Response = o1Click.OneClick_Run();
                                if (Response != null && Response.Count() > 0)
                                {
                                    foreach (var items in Response.Values.ToList())
                                    {
                                        MappedNVs.Add(new CNV
                                        {
                                            sName = items.AttributeI("sName").sValue,
                                            sValue = items.AttributeI("FKiFieldOriginIDXIGUID").sValue,
                                            sLabel = items.AttributeI("sCode").sValue
                                        });
                                    }
                                }
                            }
                        }
                    }
                    string sFname = string.Empty;
                    string sLname = string.Empty;
                    string sMob = string.Empty;
                    string sEmail = string.Empty;
                    string sPostcode = string.Empty;
                    string dDob = string.Empty;
                    List<CNV> oRisk = new List<CNV>();
                    List<string> NVs = new List<string>();

                    if ((iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)) && (iQSStepDID > 0 || (QSDStepXIGUID != null && QSDStepXIGUID != Guid.Empty)) && (iQSSecDID > 0 || (QSDSecXIGUID != null && QSDSecXIGUID != Guid.Empty)))
                    {
                        if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("PC NCB Dream Cars") || sSubject.Contains("Motor Trade (0 NCB) (SVO)") || sSubject.Contains("Motor Trade (PC NCB) (Sales)") || sSubject.Contains("Motor Trade (PC NCB) (Skilled)") || sSubject.Contains("Motor Trade (PC NCB) (BRCD)") || sSubject.Contains("Motor Trade (MT NCB) (Skilled)") || sSubject.Contains("Motor Trade (MT NCB) (BRCD)") || sSubject.Contains("Motor Trade (MT NCB) (SVO)") || sSubject.Contains("Motor Trade (0 NCB) (Sales)") || sSubject.Contains("Motor Trade (0 NCB) (Skilled)") || sSubject.Contains("Motor Trade (0 NCB) (BRCD)") || sSubject.Contains("(MT NCB) (Sales)") || sSubject.Contains("New submission"))
                        {
                            sBody = oBOI.AttributeI("sContent").sValue;
                            string sName = string.Empty;
                            string sValue = string.Empty;
                            var td3 = "";
                            var td4 = "";
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                var TRContent = Regex.Matches(sBody.Trim(), @"(?<1><TR[^>]*>\s*<td.*?</tr>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (!string.IsNullOrEmpty(sSubject) && sSubject.Contains("New submission"))
                                {
                                    int j = 0;
                                    foreach (var tr in TRContent)
                                    {
                                        j++;
                                        var Content = tr.ToString();
                                        if (Content.Contains("<tr bgcolor=\"#EAF2FA\">"))
                                        {
                                            if (Content.Contains("<table width=\"100%"))
                                            {
                                                td3 = Content.Replace("<tr>", "").Replace("<td>", "").Replace("<table width=\"100%\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\" bgcolor=\"#FFFFFF\">", "").Replace("<tr bgcolor=\"#EAF2FA\">", "").Replace("<td colspan=\"2\">", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("<strong>", "").Replace("</strong>", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                            }
                                            else
                                            {
                                                td3 = Content.Replace("<tr bgcolor=\"#EAF2FA\">", "").Replace("<td colspan=\"2\">", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("<strong>", "").Replace("</strong>", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                            }
                                        }
                                        if (Content.Contains("<tr bgcolor=\"#FFFFFF\">"))
                                        {
                                            if (Content.Contains("<a href="))
                                           {
                                                td4 = Content.Replace("<tr bgcolor=\"#FFFFFF\">", "").Replace("<td width=\"20\">&nbsp;", "").Replace("</td>", "").Replace("<td>", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("<a href=\"mailto:", "").Replace("\">", "").Replace("</a>", "").Replace("</font>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                           }
                                           else
                                           { 
                                            td4 = Content.Replace("<tr bgcolor=\"#FFFFFF\">", "").Replace("<td width=\"20\">&nbsp;", "").Replace("<td>", "").Replace("<font style=\"font-family: sans-serif; font-size:12px;\">", "").Replace("</font>", "").Replace("</td>", "").Replace("</td>", "").Replace("</tr>", "").Trim();
                                           }
                                        }
                                        if (j == 1)
                                            sName = td3;
                                        if (j == 2)
                                            sValue = td4;
                                        if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                        {
                                            if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name" || sName.ToLower() == "name of policyholder" || sName.ToLower() == "policyholder name")
                                            {
                                                if (sName.ToLower() == "name of policyholder" || sName.ToLower() == "policyholder name")
                                                {
                                                    if (sValue.Contains(' '))
                                                    {
                                                        var Valuespliting = sValue.Split(' ');
                                                        sFname = Valuespliting[0].Trim();
                                                        sLname = Valuespliting[1].Trim();
                                                        sValue = sFname;
                                                        if (!string.IsNullOrEmpty(sLname))
                                                        {
                                                            var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                            oRisk.Add(new CNV
                                                            {
                                                                sName = sName,
                                                                sValue = sLname,
                                                                sType = Surnamefieldguid.ToString()
                                                            });
                                                        }
                                                    }
                                                }
                                                else 
                                                { 
                                                sFname = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number" || sName.ToLower() == "contact no." || sName.ToLower() == "phone")
                                            {
                                                sMob = sValue;
                                            }
                                            if (sName.ToLower() == "last name")
                                            {
                                                sLname = sValue;
                                            }
                                            if (sName.ToLower() == "postcode")
                                            {
                                                sPostcode = sValue;
                                            }
                                            if (sName.ToLower() == "date of birth of policyholder")
                                            {
                                                dDob = sValue;
                                            }
                                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                                            var oFieldD = new XIDFieldOrigin();
                                            var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                            if (MappedNV == null)
                                            {
                                                oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                oDefBase.SaveErrortoDB(oCResult);
                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                if (oFieldD != null && oFieldD.ID > 0)
                                                {
                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                    XIIBO oMapNV = new XIIBO();
                                                    oMapNV.BOD = oMapBOD;
                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                    oMapNV.SetAttribute("sName", sName);
                                                    oMapNV.SetAttribute("sCode", sName);
                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                    oCR = oMapNV.Save(oMapNV);
                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                }
                                                else
                                                {
                                                    XIIBO oFO = new XIIBO();
                                                    oFO.BOD = oFieldDBOD;
                                                    oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                    oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                    oFO.SetAttribute("sDisplayName", sName);
                                                    oCR = oFO.Save(oFO);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oFO = (XIIBO)oCR.oResult;
                                                        oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                        if (oFieldD != null && oFieldD.ID > 0)
                                                        {
                                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                        }
                                                        else
                                                        {
                                                            XIIBO oFDI = new XIIBO();
                                                            List<CNV> oWhr = new List<CNV>();
                                                            oWhr.Add(new CNV
                                                            {
                                                                sName = "sName",
                                                                sValue = sName.Replace(" ", "")
                                                            });
                                                            oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                            var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                        }
                                                    }
                                                    XIIBO oMapNV = new XIIBO();
                                                    oMapNV.BOD = oMapBOD;
                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                    oMapNV.SetAttribute("sName", sName);
                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                    oCR = oMapNV.Save(oMapNV);
                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                }
                                                if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                else
                                                {
                                                    XIDQSStep oStepD = new XIDQSStep();
                                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                    oStepD.FKiQSDefintionID = iQSDID;
                                                    oStepD.sName = "Default Step";
                                                    oStepD.sDisplayName = "Default Step";
                                                    oStepD.iDisplayAs = 20;
                                                    oStepD.StatusTypeID = 10;
                                                    oStepD.bIsMerge = true;
                                                    oStepD.sIsHidden = "off";
                                                    oStepD.iOrder = 100;
                                                    oStepD.bIsSaveNext = true;
                                                    oStepD.bIsBack = true;
                                                    oStepD.bIsCopy = true;
                                                    oStepD.bIsHistory = true;
                                                    oStepD.bIsReload = true;
                                                    oStepD.bIsAutoSaving = true;
                                                    oStepD.iStage = 200;
                                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                    oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                        {
                                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                            int.TryParse(QSStepDID, out iQSStepDID);
                                                            FKiDefaultStepDID = iQSStepDID;
                                                        }
                                                        FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                        XIDQSSection oSecD = new XIDQSSection();
                                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                                        oSecD.iDisplayAs = 30;
                                                        oSecD.sIsHidden = "off";
                                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                            {
                                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                int.TryParse(QSSecDID, out iQSSecDID);
                                                            }
                                                        }
                                                    }
                                                }
                                                XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                oFieldDef.FKiStepSectionID = iQSSecDID;
                                                oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Field definition saved for Field: " + sName
                                                    });
                                                }
                                                else
                                                {
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Field definition not saved for Field: " + sName
                                                    });
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                }
                                                oRisk.Add(new CNV
                                                {
                                                    sName = sName,
                                                    sValue = sValue,
                                                    sType = FKiFieldOriginIDXIGUID.ToString()
                                                });
                                            }
                                            else
                                            {
                                                FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                            }
                                            oRisk.Add(new CNV
                                            {
                                                sName = sName,
                                                sValue = sValue,
                                                sType = FKiFieldOriginIDXIGUID.ToString()
                                            });
                                            j = 0;
                                            sName = "";
                                            sValue = "";
                                        }
                                    }
                                }
                               else
                               { 
                                foreach (var tr in TRContent)
                                {
                                    var trContent = tr.ToString();
                                    var tdRegex = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";
                                    var tdContent = Regex.Matches(trContent, tdRegex, RegexOptions.IgnoreCase);
                                    if (tdContent.Count > 1)
                                    {
                                        var tdContent1 = tdContent[0].ToString();
                                        var tdContent2 = tdContent[1].ToString();
                                        if (!string.IsNullOrEmpty(tdContent1) && !string.IsNullOrEmpty(tdContent2))
                                        {
                                            if (tdContent1.Contains("rgb(237, 244, 255)") || tdContent1.Contains("rgb(247, 250, 255)"))
                                            {
                                                if (tdContent2.Contains("rgb(200, 223, 179)") || tdContent2.Contains("rgb(237, 244, 255)") || tdContent2.Contains("rgb(247, 250, 255)"))
                                                {
                                                    var SpanRegex = @"<span\b[^>]*?>(?<V>[\s\S]*?)</\s*span>";
                                                    var td1SpanContent = Regex.Matches(tdContent1, SpanRegex, RegexOptions.IgnoreCase);
                                                    if (td1SpanContent.Count > 0)
                                                    {
                                                        var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                        var sNameMatch = regex.Match(td1SpanContent[0].ToString());
                                                        if (sNameMatch.Groups.Count > 2)
                                                        {
                                                            sName = sNameMatch.Groups[2].Value;
                                                        }
                                                    }
                                                    var td2SpanContent = Regex.Matches(tdContent2, SpanRegex, RegexOptions.IgnoreCase);
                                                    if (td2SpanContent.Count > 0)
                                                    {
                                                        var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                        var sValueMatch = regex.Match(td2SpanContent[0].ToString());
                                                        if (sValueMatch.Groups.Count > 2)
                                                        {
                                                            sValue = sValueMatch.Groups[2].Value;
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                    {
                                                        if (sName.ToLower() == "full name" || sName.ToLower() == "first name")
                                                        {
                                                            sFname = sValue;
                                                        }
                                                        if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone")
                                                        {
                                                            sMob = sValue;
                                                        }
                                                        if (sName.ToLower() == "last name")
                                                        {
                                                            sLname = sValue;
                                                        }
                                                        if (sName.ToLower() == "postcode")
                                                        {
                                                            sPostcode = sValue;
                                                        }
                                                        if (sName.ToLower() == "email")
                                                        {
                                                            sEmail = sValue;
                                                        }
                                                        var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                        var oFieldD = new XIDFieldOrigin();
                                                        var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                        if (MappedNV == null)
                                                        {
                                                            oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                            oDefBase.SaveErrortoDB(oCResult);
                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                            {
                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                XIIBO oMapNV = new XIIBO();
                                                                oMapNV.BOD = oMapBOD;
                                                                oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                oMapNV.SetAttribute("sName", sName);
                                                                oMapNV.SetAttribute("sCode", sName);
                                                                oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                oCR = oMapNV.Save(oMapNV);
                                                                if (oCR.bOK && oCR.oResult != null) { } else { }
                                                            }
                                                            else
                                                            {
                                                                XIIBO oFO = new XIIBO();
                                                                oFO.BOD = oFieldDBOD;
                                                                oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                oFO.SetAttribute("sDisplayName", sName);
                                                                oCR = oFO.Save(oFO);
                                                                if (oCR.bOK && oCR.oResult != null)
                                                                {
                                                                    oFO = (XIIBO)oCR.oResult;
                                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                    }
                                                                    else
                                                                    {
                                                                        XIIBO oFDI = new XIIBO();
                                                                        List<CNV> oWhr = new List<CNV>();
                                                                        oWhr.Add(new CNV
                                                                        {
                                                                            sName = "sName",
                                                                            sValue = sName.Replace(" ", "")
                                                                        });
                                                                        oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                        var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                        Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                    }
                                                                }
                                                                XIIBO oMapNV = new XIIBO();
                                                                oMapNV.BOD = oMapBOD;
                                                                oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                oMapNV.SetAttribute("sName", sName);
                                                                oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                oCR = oMapNV.Save(oMapNV);
                                                                if (oCR.bOK && oCR.oResult != null) { } else { }
                                                            }
                                                            if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                            else
                                                            {
                                                                XIDQSStep oStepD = new XIDQSStep();
                                                                oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                oStepD.FKiQSDefintionID = iQSDID;
                                                                oStepD.sName = "Default Step";
                                                                oStepD.sDisplayName = "Default Step";
                                                                oStepD.iDisplayAs = 20;
                                                                oStepD.StatusTypeID = 10;
                                                                oStepD.bIsMerge = true;
                                                                oStepD.sIsHidden = "off";
                                                                oStepD.iOrder = 100;
                                                                oStepD.bIsSaveNext = true;
                                                                oStepD.bIsBack = true;
                                                                oStepD.bIsCopy = true;
                                                                oStepD.bIsHistory = true;
                                                                oStepD.bIsReload = true;
                                                                oStepD.bIsAutoSaving = true;
                                                                oStepD.iStage = 50;
                                                                oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                if (oCR.bOK && oCR.oResult != null)
                                                                {
                                                                    var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                    var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                    Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                    if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                    {
                                                                        var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                        int.TryParse(QSStepDID, out iQSStepDID);
                                                                        FKiDefaultStepDID = iQSStepDID;
                                                                    }
                                                                    FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                    XIDQSSection oSecD = new XIDQSSection();
                                                                    oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                    oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                    oSecD.iDisplayAs = 30;
                                                                    oSecD.sIsHidden = "off";
                                                                    oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                        var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                        Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                        if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                        {
                                                                            var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                            int.TryParse(QSSecDID, out iQSSecDID);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                            oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                            oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                            oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                            oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                            oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                            oFieldDef.FKiStepSectionID = iQSSecDID;
                                                            oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oTraceInfo.Add(new CNV
                                                                {
                                                                    sValue = "Field definition saved for Field: " + sName
                                                                });
                                                            }
                                                            else
                                                            {
                                                                oTraceInfo.Add(new CNV
                                                                {
                                                                    sValue = "Field definition not saved for Field: " + sName
                                                                });
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                            }
                                                            oRisk.Add(new CNV
                                                            {
                                                                sName = sName,
                                                                sValue = sValue,
                                                                sType = FKiFieldOriginIDXIGUID.ToString()
                                                            });
                                                        }
                                                        else
                                                        {
                                                            FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                        }
                                                    }
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(tdContent1) && !string.IsNullOrEmpty(tdContent2))
                                            {
                                                if (tdContent1.Contains("td style=\"text-indent: 0px; line-height: normal; width: 300px;\"") || tdContent1.Contains("td style=\"line-height: normal; width: 300px; box-sizing: border-box;\""))
                                                {
                                                    if (tdContent2.Contains("td style=\"text-indent: 0px; line-height: normal; width: 300px;\"") || tdContent2.Contains("td style=\"line-height: normal; width: 300px; box-sizing: border-box;\""))
                                                    {
                                                        var SpanRegex = @"<span\b[^>]*?>(?<V>[\s\S]*?)</\s*span>";
                                                        var td1SpanContent = Regex.Matches(tdContent1, SpanRegex, RegexOptions.IgnoreCase);
                                                        if (td1SpanContent.Count > 0)
                                                        {
                                                            var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                            var sNameMatch = regex.Match(td1SpanContent[0].ToString());
                                                            if (sNameMatch.Groups.Count > 2)
                                                            {
                                                                var sName1 = sNameMatch.Groups[2].Value.ToString();
                                                                sName = sName1.Replace("<b>", "").Replace("</b>", "");
                                                            }
                                                        }
                                                        var td2SpanContent = Regex.Matches(tdContent2, SpanRegex, RegexOptions.IgnoreCase);
                                                        if (td2SpanContent.Count > 0)
                                                        {
                                                            var regex = new System.Text.RegularExpressions.Regex("<span(.*?)>(.*?)</span>");
                                                            var sValueMatch = regex.Match(td2SpanContent[0].ToString());
                                                            if (sValueMatch.Groups.Count > 2)
                                                            {
                                                                var sValue1 = sValueMatch.Groups[2].Value;
                                                                sValue = sValue1.Replace("<b>", "").Replace("</b>", "");
                                                            }
                                                        }
                                                        if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                        {
                                                            if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                                            {
                                                                    var sTname = "";
                                                                    if (sName.ToLower() == "name" && sValue.Contains(' '))
                                                                    {
                                                                        var Valuespliting = sValue.Split(' ').ToList();
                                                                        if (Valuespliting.Count() == 3)
                                                                        {
                                                                            sTname = Valuespliting[0].Trim();
                                                                            sFname = Valuespliting[1].Trim();
                                                                            sValue = sFname;
                                                                            sLname = Valuespliting[2].Trim();
                                                                        }
                                                                        else
                                                                        {
                                                                            sFname = Valuespliting[0].Trim();
                                                                            sValue = sFname;
                                                                            sLname = Valuespliting[1].Trim();
                                                                        }
                                                                        if (!string.IsNullOrEmpty(sLname))
                                                                        {
                                                                            var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                                            oRisk.Add(new CNV
                                                                            {
                                                                                sName = sName,
                                                                                sValue = sLname,
                                                                                sType = Surnamefieldguid.ToString()
                                                                            });
                                                                        }
                                                                        if (!string.IsNullOrEmpty(sTname))
                                                                        {
                                                                            var Titlefieldguid = "F1AB572A-DE7D-4817-928A-718C6FED4F56";
                                                                            oRisk.Add(new CNV
                                                                            {
                                                                                sName = sName,
                                                                                sValue = sTname,
                                                                                sType = Titlefieldguid.ToString()
                                                                            });
                                                                        }
                                                                    }
                                                                    else
                                                                    { 
                                                                    sFname = sValue;
                                                                    }
                                                            }
                                                            if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number")
                                                            {
                                                                sMob = sValue;
                                                            }
                                                            if (sName.ToLower() == "last name")
                                                            {
                                                                sLname = sValue;
                                                            }
                                                            if (sName.ToLower() == "postcode")
                                                            {
                                                                sPostcode = sValue;
                                                            }
                                                            if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                                            {
                                                                sEmail = sValue;
                                                            }
                                                            if (sName.ToLower() == "date of birth")
                                                            {
                                                                dDob = sValue;
                                                            }
                                                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                            var oFieldD = new XIDFieldOrigin();
                                                            var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                            if (MappedNV == null)
                                                            {
                                                                oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                                oDefBase.SaveErrortoDB(oCResult);
                                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                if (oFieldD != null && oFieldD.ID > 0)
                                                                {
                                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                    XIIBO oMapNV = new XIIBO();
                                                                    oMapNV.BOD = oMapBOD;
                                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                    oMapNV.SetAttribute("sName", sName);
                                                                    oMapNV.SetAttribute("sCode", sName);
                                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                    oCR = oMapNV.Save(oMapNV);
                                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                }
                                                                else
                                                                {
                                                                    XIIBO oFO = new XIIBO();
                                                                    oFO.BOD = oFieldDBOD;
                                                                    oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                    oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                    oFO.SetAttribute("sDisplayName", sName);
                                                                    oCR = oFO.Save(oFO);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oFO = (XIIBO)oCR.oResult;
                                                                        oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                        if (oFieldD != null && oFieldD.ID > 0)
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                        }
                                                                        else
                                                                        {
                                                                            XIIBO oFDI = new XIIBO();
                                                                            List<CNV> oWhr = new List<CNV>();
                                                                            oWhr.Add(new CNV
                                                                            {
                                                                                sName = "sName",
                                                                                sValue = sName.Replace(" ", "")
                                                                            });
                                                                            oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                            var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                            Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                        }
                                                                    }
                                                                    XIIBO oMapNV = new XIIBO();
                                                                    oMapNV.BOD = oMapBOD;
                                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                    oMapNV.SetAttribute("sName", sName);
                                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                    oCR = oMapNV.Save(oMapNV);
                                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                }
                                                                if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                                else
                                                                {
                                                                    XIDQSStep oStepD = new XIDQSStep();
                                                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                    oStepD.FKiQSDefintionID = iQSDID;
                                                                    oStepD.sName = "Default Step";
                                                                    oStepD.sDisplayName = "Default Step";
                                                                    oStepD.iDisplayAs = 20;
                                                                    oStepD.StatusTypeID = 10;
                                                                    oStepD.bIsMerge = true;
                                                                    oStepD.sIsHidden = "off";
                                                                    oStepD.iOrder = 100;
                                                                    oStepD.bIsSaveNext = true;
                                                                    oStepD.bIsBack = true;
                                                                    oStepD.bIsCopy = true;
                                                                    oStepD.bIsHistory = true;
                                                                    oStepD.bIsReload = true;
                                                                    oStepD.bIsAutoSaving = true;
                                                                    oStepD.iStage = 200;
                                                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                   oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                        {
                                                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                            int.TryParse(QSStepDID, out iQSStepDID);
                                                                            FKiDefaultStepDID = iQSStepDID;
                                                                        }
                                                                        FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                        XIDQSSection oSecD = new XIDQSSection();
                                                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                        oSecD.iDisplayAs = 30;
                                                                        oSecD.sIsHidden = "off";
                                                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                            {
                                                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                                int.TryParse(QSSecDID, out iQSSecDID);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                                oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                                oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                                oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                                oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                                oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                                oFieldDef.FKiStepSectionID = iQSSecDID;
                                                                oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                                if (oCR.bOK && oCR.oResult != null)
                                                                {
                                                                    oTraceInfo.Add(new CNV
                                                                    {
                                                                        sValue = "Field definition saved for Field: " + sName
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    oTraceInfo.Add(new CNV
                                                                    {
                                                                        sValue = "Field definition not saved for Field: " + sName
                                                                    });
                                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                }
                                                                    if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                        if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                        {
                                                                            sValue = "Full-Time";
                                                                        }
                                                                        else
                                                                        {
                                                                            sValue = "Part-Time";
                                                                        }
                                                                    }
                                                                oRisk.Add(new CNV
                                                                {
                                                                    sName = sName,
                                                                    sValue = sValue,
                                                                    sType = FKiFieldOriginIDXIGUID.ToString()
                                                                });
                                                            }
                                                            else
                                                            {
                                                                FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                            }
                                                                if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                {
                                                                    FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                    if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                    {
                                                                        sValue = "Full-Time";
                                                                    }
                                                                    else
                                                                    {
                                                                        sValue = "Part-Time";
                                                                    }
                                                                }
                                                            oRisk.Add(new CNV
                                                            {
                                                                sName = sName,
                                                                sValue = sValue,
                                                                sType = FKiFieldOriginIDXIGUID.ToString()
                                                            });
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (tdContent1.Contains("td width=\"300\""))
                                                    {
                                                        var td1 = tdContent1.Replace("<td width=\"300\">", "").Replace("</td>", "");
                                                        sName = td1;
                                                        if (tdContent2.Contains("td width=\"300\""))
                                                        {
                                                            var td2 = tdContent2.Replace("<td width=\"300\">", "").Replace("</td>", "");
                                                            sValue = td2;
                                                            if (!string.IsNullOrEmpty(sName) && !string.IsNullOrEmpty(sValue))
                                                            {
                                                                if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                                                {
                                                                        var sTname = "";
                                                                        if (sName.ToLower() == "name" && sValue.Contains(' '))
                                                                        {
                                                                            var Valuespliting = sValue.Split(' ').ToList();
                                                                            if (Valuespliting.Count() == 3)
                                                                            {
                                                                                sTname = Valuespliting[0].Trim();
                                                                                sFname = Valuespliting[1].Trim();
                                                                                sValue = sFname;
                                                                                sLname = Valuespliting[2].Trim();
                                                                            }
                                                                            else
                                                                            {
                                                                                sFname = Valuespliting[0].Trim();
                                                                                sValue = sFname;
                                                                                sLname = Valuespliting[1].Trim();
                                                                            }
                                                                            if (!string.IsNullOrEmpty(sLname))
                                                                            {
                                                                                var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                                                oRisk.Add(new CNV
                                                                                {
                                                                                    sName = sName,
                                                                                    sValue = sLname,
                                                                                    sType = Surnamefieldguid.ToString()
                                                                                });
                                                                            }
                                                                            if (!string.IsNullOrEmpty(sTname))
                                                                            {
                                                                                var Titlefieldguid = "F1AB572A-DE7D-4817-928A-718C6FED4F56";
                                                                                oRisk.Add(new CNV
                                                                                {
                                                                                    sName = sName,
                                                                                    sValue = sTname,
                                                                                    sType = Titlefieldguid.ToString()
                                                                                });
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                        sFname = sValue;
                                                                        }

                                                                }
                                                                if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "preferred telephone number")
                                                                {
                                                                    sMob = sValue;
                                                                }
                                                                if (sName.ToLower() == "last name")
                                                                {
                                                                    sLname = sValue;
                                                                }
                                                                if (sName.ToLower() == "postcode")
                                                                {
                                                                    sPostcode = sValue;
                                                                }
                                                                if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                                                {
                                                                    sEmail = sValue;
                                                                }
                                                                if(sName.ToLower() == "date of birth")
                                                                {
                                                                    dDob = sValue;
                                                                }
                                                                var FKiFieldOriginIDXIGUID = Guid.Empty;
                                                                var oFieldD = new XIDFieldOrigin();
                                                                var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                                                if (MappedNV == null)
                                                                {
                                                                    oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                                    oDefBase.SaveErrortoDB(oCResult);
                                                                    oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                    if (oFieldD != null && oFieldD.ID > 0)
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                        XIIBO oMapNV = new XIIBO();
                                                                        oMapNV.BOD = oMapBOD;
                                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                        oMapNV.SetAttribute("sName", sName);
                                                                        oMapNV.SetAttribute("sCode", sName);
                                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                        oCR = oMapNV.Save(oMapNV);
                                                                        if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                    }
                                                                    else
                                                                    {
                                                                        XIIBO oFO = new XIIBO();
                                                                        oFO.BOD = oFieldDBOD;
                                                                        oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                                        oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                                        oFO.SetAttribute("sDisplayName", sName);
                                                                        oCR = oFO.Save(oFO);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            oFO = (XIIBO)oCR.oResult;
                                                                            oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                                            if (oFieldD != null && oFieldD.ID > 0)
                                                                            {
                                                                                FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                                            }
                                                                            else
                                                                            {
                                                                                XIIBO oFDI = new XIIBO();
                                                                                List<CNV> oWhr = new List<CNV>();
                                                                                oWhr.Add(new CNV
                                                                                {
                                                                                    sName = "sName",
                                                                                    sValue = sName.Replace(" ", "")
                                                                                });
                                                                                oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                                                var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                                                Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                                            }
                                                                        }
                                                                        XIIBO oMapNV = new XIIBO();
                                                                        oMapNV.BOD = oMapBOD;
                                                                        oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                                        oMapNV.SetAttribute("sName", sName);
                                                                        oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                                        oCR = oMapNV.Save(oMapNV);
                                                                        if (oCR.bOK && oCR.oResult != null) { } else { }
                                                                    }
                                                                    if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                                    else
                                                                    {
                                                                        XIDQSStep oStepD = new XIDQSStep();
                                                                        oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                                        oStepD.FKiQSDefintionID = iQSDID;
                                                                        oStepD.sName = "Default Step";
                                                                        oStepD.sDisplayName = "Default Step";
                                                                        oStepD.iDisplayAs = 20;
                                                                        oStepD.StatusTypeID = 10;
                                                                        oStepD.bIsMerge = true;
                                                                        oStepD.sIsHidden = "off";
                                                                        oStepD.iOrder = 100;
                                                                        oStepD.bIsSaveNext = true;
                                                                        oStepD.bIsBack = true;
                                                                        oStepD.bIsCopy = true;
                                                                        oStepD.bIsHistory = true;
                                                                        oStepD.bIsReload = true;
                                                                        oStepD.bIsAutoSaving = true;
                                                                        oStepD.iStage = 200;
                                                                        oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                                        oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                                            var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                                            Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                                            if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                                            {
                                                                                var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                                                int.TryParse(QSStepDID, out iQSStepDID);
                                                                                FKiDefaultStepDID = iQSStepDID;
                                                                            }
                                                                            FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                                            XIDQSSection oSecD = new XIDQSSection();
                                                                            oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                                            oSecD.FKiStepDefinitionID = iQSStepDID;
                                                                            oSecD.iDisplayAs = 30;
                                                                            oSecD.sIsHidden = "off";
                                                                            oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                            {
                                                                                var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                                                var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                                                Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                                                if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                                                {
                                                                                    var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                                    int.TryParse(QSSecDID, out iQSSecDID);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                                    oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                                    oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                                    oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                                    oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                                    oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                                    oFieldDef.FKiStepSectionID = iQSSecDID;
                                                                    oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTraceInfo.Add(new CNV
                                                                        {
                                                                            sValue = "Field definition saved for Field: " + sName
                                                                        });
                                                                    }
                                                                    else
                                                                    {
                                                                        oTraceInfo.Add(new CNV
                                                                        {
                                                                            sValue = "Field definition not saved for Field: " + sName
                                                                        });
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                        if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                        {
                                                                            FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                            if (sValue.Contains("Yes") || sValue.Contains("full-time"))
                                                                            {
                                                                                sValue = "Full-Time";
                                                                            }
                                                                            else
                                                                            {
                                                                                sValue = "Part-Time";
                                                                            }
                                                                        }
                                                                    oRisk.Add(new CNV
                                                                    {
                                                                        sName = sName,
                                                                        sValue = sValue,
                                                                        sType = FKiFieldOriginIDXIGUID.ToString()
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                                                }
                                                                    if (sName.Contains("ARE YOU A FULL-TIME TRADER?"))
                                                                    {
                                                                        FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                                        if (sValue.Contains("Yes"))
                                                                        {
                                                                            sValue = "Full-Time";
                                                                        }
                                                                        else
                                                                        {
                                                                            sValue = "Part-Time";
                                                                        }
                                                                    }
                                                                    oRisk.Add(new CNV
                                                                {
                                                                    sName = sName,
                                                                    sValue = sValue,
                                                                    sType = FKiFieldOriginIDXIGUID.ToString()
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                               }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Body is null for instance: " + iBOIID
                                });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        else
                        {
                            sBody = oBOI.AttributeI("sContent").sValue;
                            if (!string.IsNullOrEmpty(sBody))
                            {
                                if (sBody.Contains("<html>")) {
                                    XIDValue oVal = new XIDValue();
                                    sBody = oVal.getHtmlBody(sBody);
                                sBody = Regex.Replace(sBody, @"\r\n", "");
                                     NVs = Regex.Split(sBody.Trim(), "<br>").ToList();
                                }
                                else
                                {
                                    sBody = Regex.Replace(sBody, @"\n", "");
                                     NVs = Regex.Split(sBody.Trim(), "<br/>").ToList();
                                }
            
                                if (NVs != null && NVs.Count() > 0)
                                {
                                    foreach (var NV in NVs)
                                    {
                                        if (NV.Contains(':'))
                                        {
                                            var Splits = NV.Split(':').ToList();
                                            var sName = Splits[0].Trim();
                                            var sValue = Splits[1].Trim();
                                            if (sName.ToLower() == "full name" || sName.ToLower() == "first name" || sName.ToLower() == "name")
                                            {
                                                if (sName.ToLower() == "full name" && sValue.Contains(' ')) 
                                                { 
                                                    var Valuespliting = sValue.Split(' ').ToList();
                                                    if (Valuespliting.Count() == 3) 
                                                    {
                                                        sFname = Valuespliting[0].Trim();
                                                        sLname = Valuespliting[1].Trim();
                                                        sLname = Valuespliting[2].Trim();
                                                        sValue = sFname;
                                                    }
                                                    else
                                                    {
                                                    sFname = Valuespliting[0].Trim();
                                                    sLname = Valuespliting[1].Trim();
                                                    sValue = sFname;
                                                    }
                                                    if (!string.IsNullOrEmpty(sLname)) 
                                                    { 
                                                    var Surnamefieldguid = "307527F8-ABC4-40F2-ADDB-914528253BE8";
                                                        oRisk.Add(new CNV
                                                        {
                                                            sName = sName,
                                                            sValue = sLname,
                                                            sType = Surnamefieldguid.ToString()
                                                        });
                                                    }
                                                }
                                                else 
                                                { 
                                                sFname = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "mobile number" || sName.ToLower() == "telephone" || sName.ToLower() == "telephone number" || sName.ToLower() == "main telephone number")
                                            {
                                                if(sValue.Contains("</span"))
                                                { 
                                                var value = sValue.Replace("<p style=\"text-align", "").Replace("</span></p>", "");
                                                    sMob = value;
                                                }
                                                else 
                                                { 
                                                sMob = sValue;
                                                }
                                            }
                                            if (sName.ToLower() == "last name" || sName.ToLower() == "sur name" || sName.ToLower() == "surname")
                                            {
                                                sLname = sValue;
                                            }
                                            if (sName.ToLower() == "postcode")
                                            {
                                                sPostcode = sValue;
                                            }
                                            if (sName.ToLower() == "email" || sName.ToLower() == "email address")
                                            {
                                                sEmail = sValue;
                                            }
                                            if (sName.ToLower() == "date of birth" || sName.ToLower() == "dob")
                                            {
                                                dDob = sValue;
                                            }
                                            var FKiFieldOriginIDXIGUID = Guid.Empty;
                                            var oFieldD = new XIDFieldOrigin();
                                            var MappedNV = MappedNVs.Where(m => m.sLabel.ToLower() == sName.ToLower()).FirstOrDefault();
                                            if (MappedNV == null)
                                            {
                                                oCResult.sMessage = "Mapping NV not found for:" + sName;
                                                oDefBase.SaveErrortoDB(oCResult);
                                                oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                if (oFieldD != null && oFieldD.ID > 0)
                                                {
                                                    FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                    XIIBO oMapNV = new XIIBO();
                                                    oMapNV.BOD = oMapBOD;
                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                    oMapNV.SetAttribute("sName", sName);
                                                    oMapNV.SetAttribute("sCode", sName);
                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                    oCR = oMapNV.Save(oMapNV);
                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                }
                                                else
                                                {
                                                    XIIBO oFO = new XIIBO();
                                                    oFO.BOD = oFieldDBOD;
                                                    oFO.SetAttribute("FKiDataTypeXIGUID", "97B8DACA-1DD3-462C-B2BA-ACAF8E9ACB81");
                                                    oFO.SetAttribute("sName", sName.Replace(" ", ""));
                                                    oFO.SetAttribute("sDisplayName", sName);
                                                    oCR = oFO.Save(oFO);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oFO = (XIIBO)oCR.oResult;
                                                        oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, sName.Replace(" ", ""));
                                                        if (oFieldD != null && oFieldD.ID > 0)
                                                        {
                                                            FKiFieldOriginIDXIGUID = oFieldD.XIGUID;
                                                        }
                                                        else
                                                        {
                                                            XIIBO oFDI = new XIIBO();
                                                            List<CNV> oWhr = new List<CNV>();
                                                            oWhr.Add(new CNV
                                                            {
                                                                sName = "sName",
                                                                sValue = sName.Replace(" ", "")
                                                            });
                                                            oFDI = oXI.BOI("XIFieldOrigin_T", null, null, oWhr);
                                                            var oFIDXIGUID = oFDI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(oFIDXIGUID, out FKiFieldOriginIDXIGUID);
                                                        }
                                                    }
                                                    XIIBO oMapNV = new XIIBO();
                                                    oMapNV.BOD = oMapBOD;
                                                    oMapNV.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                                    oMapNV.SetAttribute("sName", sName);
                                                    oMapNV.SetAttribute("FKiFieldOriginIDXIGUID", FKiFieldOriginIDXIGUID.ToString());
                                                    oCR = oMapNV.Save(oMapNV);
                                                    if (oCR.bOK && oCR.oResult != null) { } else { }
                                                }
                                                if (FKiDefaultStepDIDXIGUID != null && FKiDefaultStepDIDXIGUID != Guid.Empty) { }
                                                else
                                                {
                                                    XIDQSStep oStepD = new XIDQSStep();
                                                    oStepD.FKiQSDefintionIDXIGUID = QSDXIGUID;
                                                    oStepD.FKiQSDefintionID = iQSDID;
                                                    oStepD.sName = "Default Step";
                                                    oStepD.sDisplayName = "Default Step";
                                                    oStepD.iDisplayAs = 20;
                                                    oStepD.StatusTypeID = 10;
                                                    oStepD.bIsMerge = true;
                                                    oStepD.sIsHidden = "off";
                                                    oStepD.iOrder = 100;
                                                    oStepD.bIsSaveNext = true;
                                                    oStepD.bIsBack = true;
                                                    oStepD.bIsHistory = true;
                                                    oStepD.bIsCopy = true;
                                                    oStepD.bIsReload = true;
                                                    oStepD.bIsAutoSaving = true;
                                                    oStepD.iStage = 200;
                                                    oCR = oConfig.Save_QuestionSetStep(oStepD);
                                                    oCache.Clear_DefInCache("IO_Definition_questionset_" + QSDXIGUID.ToString());
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        var oQSDStepBOI = (XIIBO)oCR.oResult;
                                                        var QSDStepUID = oQSDStepBOI.AttributeI("xiguid").sValue;
                                                        Guid.TryParse(QSDStepUID, out QSDStepXIGUID);
                                                        if (QSDStepXIGUID == null || QSDStepXIGUID == Guid.Empty)
                                                        {
                                                            var QSStepDID = oQSDStepBOI.AttributeI("id").sValue;
                                                            int.TryParse(QSStepDID, out iQSStepDID);
                                                            FKiDefaultStepDID = iQSStepDID;
                                                        }
                                                        FKiDefaultStepDIDXIGUID = QSDStepXIGUID;
                                                        XIDQSSection oSecD = new XIDQSSection();
                                                        oSecD.FKiStepDefinitionIDXIGUID = QSDStepXIGUID;
                                                        oSecD.FKiStepDefinitionID = iQSStepDID;
                                                        oSecD.iDisplayAs = 30;
                                                        oSecD.sIsHidden = "off";
                                                        oCR = oConfig.Save_QuestionSetStepSection(oSecD);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var oQSDSecBOI = (XIIBO)oCR.oResult;
                                                            var QSDSecUID = oQSDSecBOI.AttributeI("xiguid").sValue;
                                                            Guid.TryParse(QSDSecUID, out QSDSecXIGUID);
                                                            if (QSDSecXIGUID == null || QSDSecXIGUID == Guid.Empty)
                                                            {
                                                                var QSSecDID = oQSDSecBOI.AttributeI("id").sValue;
                                                                int.TryParse(QSSecDID, out iQSSecDID);
                                                            }
                                                        }
                                                    }
                                                }
                                                XIDFieldDefinition oFieldDef = new XIDFieldDefinition();
                                                oFieldDef.FKiXIFieldOriginID = oFieldD.ID;
                                                oFieldDef.FKiXIFieldOriginIDXIGUID = FKiFieldOriginIDXIGUID;
                                                oFieldDef.FKiXIStepDefinitionIDXIGUID = FKiDefaultStepDIDXIGUID;
                                                oFieldDef.FKiStepSectionIDXIGUID = QSDSecXIGUID;
                                                oFieldDef.FKiXIStepDefinitionID = FKiDefaultStepDID;
                                                oFieldDef.FKiStepSectionID = iQSSecDID;
                                                oCR = oConfig.Save_FieldDefinition(oFieldDef);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Field definition saved for Field: " + sName
                                                    });
                                                }
                                                else
                                                {
                                                    oTraceInfo.Add(new CNV
                                                    {
                                                        sValue = "Field definition not saved for Field: " + sName
                                                    });
                                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                }
                                            }
                                            else
                                            {
                                                FKiFieldOriginIDXIGUID = new Guid(MappedNV.sValue);
                                            }
                                            if (sName == "Full Time Trader") 
                                            {
                                                FKiFieldOriginIDXIGUID = new Guid("D6CBC718-2358-4EED-83B5-FE99A8EA8E29");
                                                if (sValue == "Yes")
                                                {
                                                    sValue = "Full-Time";
                                                }
                                                else
                                                {
                                                    sValue = "Part-Time";
                                                }
                                            }
                                            oTraceInfo.Add(new CNV
                                            {
                                                sValue = "NV: " + NV + " and Risk value for field " + sName + ": is " + sValue
                                            });
                                            oRisk.Add(new CNV
                                            {
                                                sName = sName,
                                                sValue = sValue,
                                                sType = FKiFieldOriginIDXIGUID.ToString()
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "NVs not found in Body"
                                    });
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Body is null for instance: " + iBOIID
                                });
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                        var Fieldidguid = "860702CD-1D09-4E09-946D-714F595D0931";
                        if (!string.IsNullOrEmpty(iSource))
                        {
                            oRisk.Add(new CNV
                            {
                                sName = "iMTSource",
                                sValue = iSource,
                                sType = Fieldidguid.ToString()
                            });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Data parsing falied for:" + QSDXIGUID + "_" + QSDStepXIGUID + "_" + QSDSecXIGUID
                        });
                    }
                    if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                    {
                        bDefDone = true;
                    }
                    string sLeadInfo = sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail;
                    if (bDefDone && (iQSDID > 0 || (QSDXIGUID != null && QSDXIGUID != Guid.Empty)))
                    {
                        string sGUID = Guid.NewGuid().ToString();
                        XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, QSDXIGUID.ToString(), "", sGUID, 0, 0);
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "QS Definition loaded from Cache"
                        });
                        XIDQS oQSDC = (XIDQS)oQSD.Clone(oQSD);
                        oCR = oXI.CreateQSI(null, QSDXIGUID.ToString(), null, null, 0, 0, null, 0, null, 0, sGUID);
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "QS Instance is created"
                        });
                        var oQSInstance = (XIIQS)oCR.oResult;
                        oQSInstance.QSDefinition = oQSDC;
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Step instance loaded"
                        });
                        var RiskBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T");
                        if (oRisk != null && oRisk.Count() > 0)
                        {
                            foreach (var risk in oRisk)
                            {
                                XIIBO oRiskI = new XIIBO();
                                oRiskI.BOD = RiskBOD;
                                oRiskI.SetAttribute("sDerivedValue", risk.sValue);
                                var oFieldD = (XIDFieldOrigin)oCache.GetObjectFromCache(XIConstant.CacheFieldOrigin, null, risk.sType);
                                if (oFieldD != null && oFieldD.XIGUID != null && oFieldD.XIGUID != Guid.Empty)
                                {
                                    if (oFieldD.bIsOptionList)
                                    {
                                        var optionValue = oFieldD.FieldOptionList.Where(m => m.sOptionName.ToLower() == risk.sValue.ToLower()).FirstOrDefault();
                                        if (optionValue != null)
                                        {
                                            var iOptValue = optionValue.sOptionValue;
                                            oRiskI.SetAttribute("sValue", iOptValue);
                                        }
                                    }
                                    else
                                    {
                                        oRiskI.SetAttribute("sValue", risk.sValue);
                                    }
                                }
                                oRiskI.SetAttribute("FKiFieldOriginIDXIGUID", risk.sType);
                                oRiskI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                                oRiskI.Save(oRiskI);
                            }
                        }
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "Lead creation statred for:" + sLeadInfo + "-" + oQSInstance.XIGUID
                        });
                        var oLeadBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Lead");
                        XIIBO oLeadI = new XIIBO();
                        oLeadI.BOD = oLeadBOD;
                        oLeadI.SetAttribute("sFirstName", sFname);
                        oLeadI.SetAttribute("sLastName", sLname);
                        oLeadI.SetAttribute("sMob", sMob);
                        oLeadI.SetAttribute("sPostCode", sPostcode);
                        oLeadI.SetAttribute("sEmail", sEmail);
                        oLeadI.SetAttribute("FKiQSInstanceIDXIGUID", oQSInstance.XIGUID.ToString());
                        oLeadI.SetAttribute("iStatus", "0");
                        oLeadI.SetAttribute("sName", sFname);
                        oLeadI.SetAttribute("dDOB", dDob);
                        if (!string.IsNullOrEmpty(iSource)) {
                            oLeadI.SetAttribute("FKiSourceID", iSource);
                        }
                        oLeadI.oSignalR = oSignalR;
                        oCR = oLeadI.Save(oLeadI, false);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            int iLeadID = 0;
                            oLeadI = (XIIBO)oCR.oResult;
                            var LeadID = oLeadI.AttributeI("id").sValue;
                            int.TryParse(LeadID, out iLeadID);
                            if (iLeadID > 0)
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Lead creation success for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                                });
                                var CommMatchBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XCommunicationMatch");
                                XIIBO oCommMatch = new XIIBO();
                                oCommMatch.BOD = CommMatchBOD;
                                oCommMatch.SetAttribute("FKiBODID", "717");
                                oCommMatch.SetAttribute("FKiBOIID", iLeadID.ToString());
                                oCommMatch.SetAttribute("FKiCommunicationID", iBOIID);
                                oCR = oCommMatch.Save(oCommMatch);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "Communication linked successfully to Lead"
                                    });
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV
                                    {
                                        sValue = "Communication linking to Lead is falied"
                                    });
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV
                                {
                                    sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                                });
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV
                            {
                                sValue = "Lead creation failed for:" + sFname + "-" + sLname + "-" + sMob + "-" + sPostcode + "-" + sEmail + "-" + oQSInstance.XIGUID
                            });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV
                        {
                            sValue = "QS instance creation falied for:" + sLeadInfo
                        });
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTraceInfo.Add(new CNV
                    {
                        sValue = "Mandatory Params: iBODID " + iBODID + " or iBOIID:" + iBOIID + " are missing"
                    });
                }
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
                if (oCResult.xiStatus != xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = QSDXIGUID;
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
                oTraceInfo.Add(new CNV
                {
                    sValue = oCResult.sMessage
                });
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public string getHtmlBody(string sBodyStr)
        {
            string sBody = sBodyStr.ToLower();
            int start = sBody.IndexOf("<body>");
            int bodyTagLength = 6;
            if (start == -1)
            {
                start = sBody.IndexOf("<body ");
                if (start != -1)
                {
                    start = sBody.IndexOf(">", start);
                    bodyTagLength = 0;
                }
            }
            if (start > 0)
            {
                start = start + bodyTagLength;
                int end = sBody.IndexOf("</body>");
                return sBodyStr.Substring(start, end - start);
            }
            else
            {
                return sBodyStr;
            }
        }

        public void Vechicalculation(List<CNV> oRisk)
        {
            foreach (var item in oRisk)
            {
                if (item.sName.ToLower() == "vehiclevalue")
                {
                    int vehiclevalue = Convert.ToInt32(item.sValue);
                    XID1Click PV1Click = new XID1Click();
                    var VechicleQuery = "select* from XIFieldOptionList_T where FKiQSFieldIDXIGUID='" + item.sType + "'";
                    PV1Click.Query = VechicleQuery;
                    PV1Click.Name = "XIFieldOptionList_T";
                    var Result = PV1Click.OneClick_Execute();
                    if (Result != null && Result.Count() > 0)
                    {
                        for (int i = 0; i < Result.Count; i++)
                        {
                            var optionvalue = Result[i.ToString()].AttributeI("soptionname").sValue;
                            var Valuespliting = optionvalue.Split('-');
                            int max = Convert.ToInt32(Valuespliting[1].Replace("£", "").Replace("k", "")) * 1000;

                            if (vehiclevalue <= max)
                            {
                                item.sValue = optionvalue;
                                break;
                            }
                        }
                    }
				}
			}
		}

        public void MTLeadGenMapping(List<CNV> oRisk)
        {
            foreach (var risko in oRisk)
            {
                if (risko.sValue.Contains("CarNCB") && risko.sType == "112526d2-1a79-480b-996c-f6a6f5ec783b")
                {
                    risko.sValue = "Car";
                }
                if (risko.sValue.Contains("TradeNCB") && risko.sType == "112526d2-1a79-480b-996c-f6a6f5ec783b")
                {
                    risko.sValue = "Motor Trade";
                }
                if (risko.sName.Contains("CarNCB") && risko.sType == "11d07762-e624-4216-b58a-c0509fc30958")
                {
                    var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
                    if (singleYears.Contains(risko.sValue))
                    {
                        risko.sValue += " Years";
                    }
                    else if (risko.sValue.Contains("+"))
                    {
                        risko.sValue = risko.sValue.Replace("+", "");
                    }
                    else if (risko.sValue.Contains("10"))
                    {
                        risko.sValue = risko.sValue + "+ Years";
                    }
                    else if (risko.sValue.Contains("1"))
                    {
                        risko.sValue = risko.sValue + " Year";
                    }
                }
                if (risko.sName.Contains("TradeNCB") && risko.sType == "11d07762-e624-4216-b58a-c0509fc30958")
                {
                    var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
                    if (singleYears.Contains(risko.sValue))
                    {
                        risko.sValue += " Years";
                    }
                    else if (risko.sValue.Contains("+"))
                    {
                        risko.sValue = risko.sValue.Replace("+", "");
                    }
                    else if (risko.sValue.Contains("10"))
                    {
                        risko.sValue = risko.sValue + "+ Years";
                    }
                    else if (risko.sValue.Contains("1"))
                    {
                        risko.sValue = risko.sValue + " Year";
                    }
                }
					if (risko.sName.ToLower().Contains("TYPE OF COVER".ToLower()) || risko.sName.ToLower().Contains("Level of Cover Required".ToLower()))
					{
						if (risko.sValue.ToLower().Contains("Fully Comprehensive".ToLower()) || risko.sValue.ToLower().Contains("Comprehensive Cover".ToLower()))
						{
							risko.sValue = "Comprehensive";
						}
						else if (risko.sValue.ToLower().Contains("Third Party".ToLower()))
						{
							risko.sValue = "Third Party Only";
						}
						else if (risko.sValue.ToLower().Contains("Third Party, Fire & Theft".ToLower()))
						{
							risko.sValue = "Third Party Fire and Theft";
						}
					}
					if (risko.sValue.ToLower().Contains("PRIVATE CAR NO CLAIMS BONUS".ToLower()) && risko.sType == "112526d2-1a79-480b-996c-f6a6f5ec783b")
					{
						risko.sValue = "Car";
					}
					if (risko.sValue.ToLower().Contains("MOTOR TRADE NO CLAIMS BONUS".ToLower()) && risko.sType == "112526d2-1a79-480b-996c-f6a6f5ec783b")
					{
						risko.sValue = "Motor Trade";
					}
					if (risko.sName.ToLower().Contains("PRIVATE CAR NO CLAIMS BONUS".ToLower()) && risko.sType == "11d07762-e624-4216-b58a-c0509fc30958")
					{
						var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
						if (singleYears.Contains(risko.sValue))
						{
							risko.sValue += " Years";
						}
						else if (risko.sValue.Contains("+"))
						{
							risko.sValue = risko.sValue.Replace("+", "");
						}
						else if (risko.sValue.Contains("10"))
						{
							risko.sValue = risko.sValue + "+ Years";
						}
						else if (risko.sValue.Contains("1"))
						{
							risko.sValue = risko.sValue + " Year";
						}
					}
					if (risko.sName.ToLower().Contains("MOTOR TRADE NO CLAIMS BONUS".ToLower()) && risko.sType == "11d07762-e624-4216-b58a-c0509fc30958")
					{
						var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
						if (singleYears.Contains(risko.sValue))
						{
							risko.sValue += " Years";
						}
						else if (risko.sValue.Contains("+"))
						{
							risko.sValue = risko.sValue.Replace("+", "");
						}
						else if (risko.sValue.Contains("10"))
						{
							risko.sValue = risko.sValue + "+ Years";
						}
						else if (risko.sValue.Contains("1"))
						{
							risko.sValue = risko.sValue + " Year";
						}
					}
					if (risko.sName.Contains("HAVE YOU MADE ANY CLAIMS IN THE LAST 5 YEARS?"))
					{
						if (risko.sValue.Contains("No Claims"))
						{
							risko.sValue = "No";
						}
						else
						{
							risko.sValue = "Yes";
						}
					}
					if (risko.sName.Contains("HAVE YOU HAD ANY MOTORING CONVICTIONS IN THE LAST 5 YEARS?"))
					{
						if (risko.sValue.Contains("No Motoring Convictions"))
						{
							risko.sValue = "No";
						}
						else
						{
							risko.sValue = "Yes";
						}
					}
					if (risko.sName.Contains("HOW LONG HAVE YOU HELD A FULL UK LICENCE?"))
					{
						if (risko.sValue.Contains("+"))
						{
							risko.sValue = risko.sValue.Replace("+", "");
						}
					}
					if (risko.sName.Contains("CLAIMS LAST 5 YEARS"))
					{
						if (risko.sValue.Contains("No Claims"))
						{
							risko.sValue = "No";
						}
						else
						{
							risko.sValue = "Yes";
						}
					}
					if (risko.sName.Contains("MOTORING CONVICTIONS LAST 5 YEARS"))
					{
						if (risko.sValue.Contains("No Motoring Convictions"))
						{
							risko.sValue = "No";
						}
						else
						{
							risko.sValue = "Yes";
						}
					}
					if (risko.sName.Contains("YEARS LICENCE HELD"))
					{
						var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
						if (singleYears.Contains(risko.sValue))
						{
							risko.sValue += " Years";
						}
						else if (risko.sValue.Contains("Over"))
						{
							var licence = risko.sValue.Replace("Over ", "").Replace(" Years", "");
							risko.sValue = licence + "+ Years";
						}
						else if (risko.sValue.Contains("10"))
						{
							risko.sValue = risko.sValue + "+ Years";
						}
						else if (risko.sValue.Contains("1"))
						{
							risko.sValue = risko.sValue + " Year";
						}
					}
					if (risko.sName.ToLower().Contains("Years Trade Experience".ToLower()) || risko.sName.ToLower().Contains("YEARS TRADING EXPERIENCE".ToLower()))
					{
						var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
						if (singleYears.Contains(risko.sValue))
						{
							risko.sValue += " Years";
						}
						else if (risko.sValue.Contains("+"))
						{
							risko.sValue = risko.sValue.Replace("+", "");
						}
						else if (risko.sValue.Contains("10"))
						{
							risko.sValue = risko.sValue + "+ Years";
						}
						else if (risko.sValue.Contains("1"))
						{
							risko.sValue = risko.sValue + " Year";
						}
					}
					if (risko.sName.Contains("UK Resident For") || risko.sName.Contains("Full UK licence held for"))
					{
						var singleYears = new List<string> { "0", "2", "3", "4", "5", "6", "7", "8", "9" };
						if (singleYears.Contains(risko.sValue))
						{
							risko.sValue += " Years";
						}
						else if (risko.sValue.Contains("10"))
						{
							risko.sValue = risko.sValue + "+ Years";
						}
						else if (risko.sValue.Contains("1"))
						{
							risko.sValue = risko.sValue + " Year";
						}
					}
					if (risko.sName.ToLower().Contains("WHERE DO YOU TRADE FROM?".ToLower()))
					{
						if (risko.sValue.ToLower().Contains("Mobile trader".ToLower()) || risko.sValue.ToLower().Contains("Mobile (based from home)".ToLower()))
						{
							risko.sValue = "Mobile";
						}
						else if (risko.sValue.ToLower().Contains("Business Premises".ToLower()))
						{
							risko.sValue = "Premises";
						}
					}
					if (risko.sName.ToLower().Contains("Type of no claims discount".ToLower()) && risko.sValue.ToLower().Contains("Private Car No Claim Bonus".ToLower()))
					{
						risko.sValue = "Car";
					}
					if (risko.sName.ToLower().Contains("Type of no claims discount".ToLower()) && risko.sValue.ToLower().Contains("Motor Trade No Claims Bonus".ToLower()))
					{
						risko.sValue = "Motor Trade";
					}
					if (risko.sName.ToLower().Contains("Number of years no claim discount (NCB)".ToLower()))
					{
						if (risko.sValue.Contains("+"))
						{
							risko.sValue = risko.sValue.Replace("+", "");
						}
					}
			}
		}

        public void APISourceMapping(List<CNV> oRisk, string FKiorgid)
        {
            foreach (var item in oRisk)
            {
                if (item.sName.ToLower() == "source")
                {
                    XID1Click PV1Click = new XID1Click();
                    var VechicleQuery = "select* from XISource_T where sName='" + item.sValue + "' and FKiOrgID = " + FKiorgid + "";
                    PV1Click.Query = VechicleQuery;
                    PV1Click.Name = "XISource_T";
                    var Result = PV1Click.OneClick_Execute();
                    if (Result != null && Result.Count() > 0)
                    {

                        var test = Result.Select(m => m.Value.AttributeI("id").sValue).FirstOrDefault();
                        item.sValue = test;
                    }
                }
            }
        }
    }
}