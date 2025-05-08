using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XICore;
using XIDatabase;
using XIDNA.Models;
using XISystem;


namespace XIDNA.Controllers
{
    public class DocxTemplaterController : Controller
    {
        XIContentEditors XIContentResult = new XIContentEditors();
        public string sUnresolvedNotation { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        XIDefinitionBase oXIDef = new XIDefinitionBase();
        // GET: DocxTemplater
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GenerateDocument(string TemplateType, int TemplateID)
        {
            XIContentEditors oContent = new XIContentEditors();
            XIContentEditors ContentEditor = new XIContentEditors();
            CResult oContentreturn = new CResult();
            ModelDbContext db = new ModelDbContext();
            //XIContentEditors oConent = new XIContentEditors();
            //oEmail.sSubject = oContent.sSubject;
            XIBOInstance oBOIInstance = new XIBOInstance();
            XIBOInstance oBOIns = new XIBOInstance();
            Dictionary<string, List<XIIBO>> nBOIns = new Dictionary<string, List<XIIBO>>();
            string sBo = string.Empty;
            XIDBO oBOD = new XIDBO();
            System.Data.DataTable oBOInsdt = new System.Data.DataTable();
            Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
            Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
            XID1Click o1ClickD1 = new XID1Click();

            // XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            XIDXI oXIDc = new XIDXI();
            XIDBAPI Connection = new XIDBAPI();
            var Record = "";
            XIIXI oIXI = new XIIXI();
            oBOIns = oIXI.BOI("ACPolicy_T", TemplateID.ToString()).Structure("New Policy").XILoad(null, true);


            //if (TemplateType.ToLower() == "json" || TemplateType.ToLower() == "html" || TemplateType.ToLower() == "xml")
            //{
            //    var DataContent = db.XITemplater.Where(u => u.sType == TemplateType).FirstOrDefault();
            //    ContentEditor.Content = DataContent.sContent;
            //    ContentEditor.BO = DataContent.BO;
            //    ContentEditor.sSubject = DataContent.sSubject;
            //    oContentreturn = oContent.MergeContentTemplate(ContentEditor, oBOIns, 0, 0);
            //    oContentreturn.sCode = DataContent.sContent;
            //}
            //else
            //{
            //    ContentEditor.Content = TemplateType;
            //    oContentreturn = oContent.MergeContentTemplate(ContentEditor, oBOIns, 0, 0);
            //}
            return Json(oContentreturn, JsonRequestBehavior.AllowGet);
        }



        public ActionResult DownloadNewWordDocument(string sType = null, int ID = 0, string DocType = null)
        {
            //var StringResult = GenerateDocument(sType, ID);
            //var Resultre = StringResult.GetHashCode();
            //DocxTemplaterController controller = new DocxTemplaterController();
            JsonResult result = GenerateDocument(sType, ID);
            CResult oResultData = (CResult)(result.Data);
            MemoryStream memoryStream = new MemoryStream();
            byte[] buf = (new UTF8Encoding()).GetBytes(oResultData.oResult.ToString());
            memoryStream.Write(buf, 0, buf.Length);//.WriteTo(stream);
            if (DocType == "docx")
            {
                // string htmlText = oResultData.oResult.ToString();
                // WordDocument wordDocument = new WordDocument();
                // IWSection section = wordDocument.AddSection();
                // section.Body.IsValidXHTML(new XhtmlString(htmlText), wordDocument.XHTMLValidateOption);
                //// section.Body.InsertXHTML(htmlContent);
                // wordDocument.Save("Sample.doc", FormatType.Html, Response, HttpContentDisposition.InBrowser);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ID + ".docx");
            }
            else if (DocType == "pdf")
                return File(memoryStream.ToArray(), "application/pdf", ID + ".pdf");
            else
                return File(memoryStream.ToArray(), "application/xml", ID + ".xml");
        }

        public void ConvertDoctoPDF(HttpPostedFileBase file)
        {
            XIIScript oXIScript = new XIIScript();
            XIIXI oIXI = new XIIXI();
            XIBOInstance result = oIXI.BOI("ACPolicy_T", 1336.ToString()).Structure("New Policy").XILoad(null, true);

            WordDocument document = new WordDocument();
            //Opens the input Word document.
            Stream docStream = file.InputStream;
            document.Open(docStream, FormatType.Docx);
            docStream.Dispose();
            Regex oregexpatternObj = null;
            oregexpatternObj = new Regex(Regex.Escape("{{") + "(.+?)" + Regex.Escape("}}"));
            TextSelection[] textSelections = document.FindAll(oregexpatternObj);
            string[] searchedPlaceholders = new string[textSelections.Length];
            for (int i = 0; i < textSelections.Length; i++)
            {
                searchedPlaceholders[i] = textSelections[i].SelectedText.Replace("{{", "").Replace("}}", "");
            }

            oregexpatternObj = new Regex(Regex.Escape("[{") + "(.+?)" + Regex.Escape("}]"));
            // var dictnotations1 = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
            TextSelection[] textSelections1 = document.FindAll(oregexpatternObj);
            string[] searchedPlaceholders1;
            if (textSelections1 != null)
            {
                searchedPlaceholders1 = new string[textSelections1.Length];
                for (int i = 0; i < textSelections1.Length; i++)
                {
                    searchedPlaceholders1[i] = textSelections1[i].SelectedText.Replace("{{", "").Replace("}}", "");
                }
            }
            oregexpatternObj = new Regex(Regex.Escape("[(") + "(.*?)" + Regex.Escape(")]"));
            // var dictnotations1 = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
            TextSelection[] textSelections2 = document.FindAll(oregexpatternObj);
            string[] searchedPlaceholders2;
            if (textSelections2 != null)
            {
                searchedPlaceholders2 = new string[textSelections2.Length];
                for (int i = 0; i < textSelections2.Length; i++)
                {
                    searchedPlaceholders2[i] = textSelections2[i].SelectedText.Replace("[(", "").Replace(")]", "");
                }
                string SourceContent = string.Empty;
                string sNotation = string.Empty;
                var odictnotations = XIContentResult.GetNotationsWithinDelimeter1(searchedPlaceholders2, oregexpatternObj);

                // var oResult = (CResult)oXIScript.EvalueNotation(sScript, result, 0, "[()]", null);
                //var oResult = (CResult)oXIScript.EvaluateMultiNotation(searchedPlaceholders2.Distinct().ToArray(), result, 0, "[()]");
            }
            var oScrResult = (CResult)oXIScript.EvaluateMultiNotation(searchedPlaceholders.Distinct().ToArray(), result, 0, "{{}}");
            string[] ResultKey = new string[textSelections.Length];
            string[] ResultValue = new string[textSelections.Length];

            Dictionary<string, string> Res = new Dictionary<string, string>();
            if (oScrResult.oResult != null)
            {
                Res = (Dictionary<string, string>)(oScrResult.oResult);
                ResultKey = Res.Keys.ToArray();
                ResultValue = Res.Values.ToArray();
            }

            for (int i = 0; i < ResultKey.Count(); i++)
            {
                document.Replace("{{", " ", false, false);
                document.Replace("}}", " ", false, false);
                if (string.IsNullOrEmpty(ResultValue[i]))
                    document.Replace(ResultKey[i], "", false, true);
                else
                    document.Replace(ResultKey[i], ResultValue[i], false, true);
            }
            //Saves the resultant file in the given path.
            //docStream = File.Create(Path.GetFullPath(@"Result.docx"));
            DocToPDFConverter converter = new DocToPDFConverter();
            //Sets true to enable the fast rendering using direct PDF conversion.
            converter.Settings.EnableFastRendering = true;
            //Converts Word document into PDF document
            PdfDocument pdfDocument = converter.ConvertToPDF(document);
            //Saves the PDF file to file system
            pdfDocument.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
            // document.Save("Output.docx", FormatType.Docx, HttpContext.ApplicationInstance.Response, HttpContentDisposition.InBrowser);
            // document.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
            docStream.Dispose();
        }
        public void HTMLtoDocx(HttpPostedFileBase file)
        {
            WordDocument document = new WordDocument(file.InputStream);
            HTMLExport export = new HTMLExport();
            //The images in the input document are copied to this folder
            document.SaveOptions.HtmlExportImagesFolder = @"D:\";
            //The headers and footers in the input are exported
            document.SaveOptions.HtmlExportHeadersFooters = true;
            //Exports the text form fields as editable
            document.SaveOptions.HtmlExportTextInputFormFieldAsText = false;
            //Sets the style sheet type
            document.SaveOptions.HtmlExportCssStyleSheetType = CssStyleSheetType.External;
            //Sets name for style sheet
            document.SaveOptions.HtmlExportCssStyleSheetFileName = "UserDefinedFileName.css";
            //Export the Word document image as Base-64 embedded image
            document.SaveOptions.HTMLExportImageAsBase64 = true;
            ////Saves the document as html file
            //string docPath = @"C:\Users\narendar.n\Desktop\Test Doc Sample.doc";
            //string readText = System.IO.File.ReadAllText(Path.Combine(docPath, "TestTxt.txt"));
            export.SaveAsXhtml(document, "WordtoHtml.html");
            document.Close();
        }

        public void PdffromDocx(HttpPostedFileBase file)
        {
            //Loads an existing document

            WordDocument document1 = new WordDocument(file.InputStream);
            HTMLExport export = new HTMLExport();
            //The images in the input document are copied to this folder
            //document1.SaveOptions.HtmlExportImagesFolder = @"D:\";
            //The headers and footers in the input are exported
            document1.SaveOptions.HtmlExportHeadersFooters = true;
            //Exports the text form fields as editable
            document1.SaveOptions.HtmlExportTextInputFormFieldAsText = false;
            //Sets the style sheet type
            document1.SaveOptions.HtmlExportCssStyleSheetType = CssStyleSheetType.Internal;
            document1.SaveOptions.HTMLExportImageAsBase64 = true;
            var textfile = document1.GetText();


            XIIXI oIXI = new XIIXI();
            XIBOInstance result = oIXI.BOI("ACPolicy_T", 1336.ToString()).Structure("New Policy").XILoad(null, true);

            WordDocument document = new WordDocument();
            Dictionary<string, string> Res = new Dictionary<string, string>();
            DocToPDFConverter converter = new DocToPDFConverter();
            //Sets true to enable the fast rendering using direct PDF conversion.
            converter.Settings.EnableFastRendering = true;
            //Converts Word document into PDF document
            PdfDocument pdfDocument = converter.ConvertToPDF(document);
            //Saves the PDF file to file system
            pdfDocument.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
        }



        public CResult CopyConvertDoctoPDF(HttpPostedFileBase file)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Docx file convert to pdf";//expalin about this method logic
            try
            {
               //  oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (file != null)//check mandatory params are passed or not
                {
                    //XIInfraCache oCacheR = new XIInfraCache();
                    XIIXI oIXI = new XIIXI();
                    XIBOInstance oStructureInstance = oIXI.BOI("ACPolicy_T", 2005.ToString()).Structure("New Policy").XILoad(null, true);
                    WordDocument document = new WordDocument();
                    XIIScript oXIScript = new XIIScript();
                    //Opens the input Word document.
                    Stream docStream = file.InputStream;
                    document.Open(docStream, FormatType.Docx);
                    // var tesxc = document.GetText();
                    oCR = CopyDocumentMergingData(document, oStructureInstance);
                    //oCR = SubMethod();
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        //Saves the resultant file in the given path.
                        DocToPDFConverter converter = new DocToPDFConverter();
                        //Sets true to enable the fast rendering using direct PDF conversion.
                        converter.Settings.EnableFastRendering = true;
                        //Converts Word document into PDF document
                        PdfDocument pdfDocument = converter.ConvertToPDF(document);
                        //Saves the PDF file to file system
                        pdfDocument.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Open);
                        // document.Save("Output.docx", FormatType.Docx, HttpContext.ApplicationInstance.Response, HttpContentDisposition.InBrowser);
                        // document.Save("Output.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                        docStream.Dispose();
                        oTrace.oTrace.Add(oCR.oTrace);
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
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: file="+file+"  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXIDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //oCResult.oResult = pdfDocument;
            return oCResult;
        }


        public CResult CopyDocumentMergingData(WordDocument document, XIBOInstance oStructureInstance)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Merging fields data with docx file";//expalin about this method logic
            int iLoopCount = 0;
            XIIScript oXIScript = new XIIScript();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (document != null && oStructureInstance != null)//check mandatory params are passed or not
                {

                    #region TYPE3 <||>
                    // var SourceContent = "";
                    List<string> oList = new List<string>();
                    var oregexpatternObj = new Regex(Regex.Escape("<|") + "(.+?)" + Regex.Escape("|>"));
                    //  var nData = document.FindAll(oregexpatternObj);
                    // var ndictnotations = (List<string>)XIContentResult.SubStringNotationList(SourceContent, "<|", "|>", oList);
                    TextSelection[] textSelections2 = document.FindAll(oregexpatternObj);
                    if (textSelections2 != null)
                    {
                        string[] searchedPlaceholders2 = new string[textSelections2.Length];
                        for (int i = 0; i < textSelections2.Length; i++)
                        {
                            searchedPlaceholders2[i] = textSelections2[i].SelectedText;
                        }
                        foreach (var exp in searchedPlaceholders2)
                        {
                            var sNotation = exp;
                            CResult oResult = XIContentResult.XIIScriptResolving(oStructureInstance, exp);
                            string sValue = string.Empty;
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                sValue = (string)oResult.oResult;
                                if (!string.IsNullOrEmpty(sValue))
                                {
                                    //string sSubNotation = SubStringNotation(sValue, "<|", "|>"); string sSubNotationValue = string.Empty;
                                    //if (!string.IsNullOrEmpty(sSubNotation))
                                    //{
                                    //    var oResponse = XIIScriptResolving(oStructureInstance, sSubNotation);
                                    //    if (oResponse.bOK && oResponse.oResult != null)
                                    //    {
                                    //        sSubNotationValue = (string)oResponse.oResult;
                                    //        if (!string.IsNullOrEmpty(sSubNotationValue))
                                    //        {
                                    //            sValue = sValue.Replace(sSubNotation, sSubNotationValue);
                                    //        }
                                    //        else
                                    //        {
                                    //            sValue = sValue.Replace(sSubNotation, string.Empty);
                                    //        }
                                    //    }
                                    //}
                                    //sValue = sValue.Replace("|||", ",");
                                    document.Replace(exp.ToString(), sValue, false, false);
                                    // document.Replace("", sValue.Contains("\r\n") ? sValue.TrimEnd('\n', '\r') : sValue);
                                    XIContentEditors oXISubNotationContent = new XIContentEditors();
                                    oXISubNotationContent.Content = sValue;
                                    var oCResultobj = XIContentResult.MergeContentTemplate(oXISubNotationContent, oStructureInstance, 0, 0);
                                    if (oCResultobj.bOK && oCResultobj.oResult != null)
                                    {
                                        string sXISubNotationResult = (string)oCResultobj.oResult;
                                        if (!string.IsNullOrEmpty(sXISubNotationResult))
                                        {
                                            document.Replace(sValue, sXISubNotationResult, false, false);
                                        }
                                        else
                                        {
                                            document.Replace(sValue, string.Empty, false, false);
                                        }
                                    }

                                }
                                else
                                {
                                    document.Replace(exp.ToString(), string.Empty, false, false);
                                }
                            }
                            else
                            {
                                sUnresolvedNotation += "Notation : " + exp;
                            }
                            // string sSubNotationValue = string.Empty;

                        }
                    }
                    oregexpatternObj = new Regex(Regex.Escape("<|") + Regex.Escape("|>"));
                    //  var nData = document.FindAll(oregexpatternObj);
                    // var ndictnotations = (List<string>)XIContentResult.SubStringNotationList(SourceContent, "<|", "|>", oList);
                    TextSelection[] textSelections3 = document.FindAll(oregexpatternObj);
                    if (textSelections3 != null)
                    {
                        string[] searchedPlaceholders3 = new string[textSelections3.Length];
                        for (int i = 0; i < textSelections3.Length; i++)
                        {
                            searchedPlaceholders3[i] = textSelections3[i].SelectedText;
                        }
                        foreach (var exp in searchedPlaceholders3)
                        {
                            document.Replace(exp.ToString(), string.Empty, false, false);
                        }
                    }
                    #endregion


                    #region TYPE0 {{}}
                    oregexpatternObj = new Regex(Regex.Escape("{{") + "(.+?)" + Regex.Escape("}}"));
                    TextSelection[] textSelections = document.FindAll(oregexpatternObj);
                    if (textSelections != null)
                    {
                        string[] searchedPlaceholders = new string[textSelections.Length];
                        for (int i = 0; i < textSelections.Length; i++)
                        {
                            searchedPlaceholders[i] = textSelections[i].SelectedText.Replace("{{", "").Replace("}}", "");
                        }
                        var oScrResult = (CResult)oXIScript.EvaluateMultiNotation(searchedPlaceholders.Distinct().ToArray(), oStructureInstance, 0, "{{}}");
                        string[] ResultKey = new string[textSelections.Length];
                        string[] ResultValue = new string[textSelections.Length];

                        Dictionary<string, string> Res = new Dictionary<string, string>();
                        if (oScrResult.oResult != null)
                        {
                            Res = (Dictionary<string, string>)(oScrResult.oResult);
                            ResultKey = Res.Keys.ToArray();
                            ResultValue = Res.Values.ToArray();
                        }

                        for (int i = 0; i < ResultKey.Count(); i++)
                        {
                            document.Replace("{{", " ", false, false);
                            document.Replace("}}", " ", false, false);
                            if (string.IsNullOrEmpty(ResultValue[i]))
                                document.Replace(ResultKey[i], string.Empty, false, true);
                            else
                                document.Replace(ResultKey[i], ResultValue[i], false, true);
                        }
                    }
                    #endregion TYPE0 [{}]

                    #region TYPE2 [{}]
                    var SourceContent = "";
                    oregexpatternObj = new Regex(Regex.Escape("[{") + "(.+?)" + Regex.Escape("}]"));
                    var textContent1 = document.FindAll(oregexpatternObj);
                    var dictnotations1 = XIContentResult.GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                    if (dictnotations1.Count() > 0)
                    {
                        foreach (var item in dictnotations1)
                        {
                            var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, item, "0");
                            SourceContent = SourceContent.Replace("[{" + item + "}]", oContentDef.First().Content.ToString());
                        }
                    }
                    #endregion

                    #region TYPE1 (||)
                    // var SourceContent = "";
                    oregexpatternObj = new Regex(Regex.Escape("(|") + "(.*?)" + Regex.Escape("|)"));
                    var nnotations = XIContentResult.GetNotationsWithinDelimeter(SourceContent, oregexpatternObj);
                    foreach (var exp in nnotations)
                    {
                        string delimterNotaion = "(|" + exp + "|)";
                        if (exp.Length > 0 && exp.Contains("[|"))
                        {
                            oregexpatternObj = new Regex(Regex.Escape("[|") + "(.*?)" + Regex.Escape("|]"));
                            var nSubDicnotations = XIContentResult.GetNotationsWithinDelimeter(exp, oregexpatternObj);
                            foreach (var exp1 in nSubDicnotations)
                            {
                                List<CNV> oWhereParams = new List<CNV>();
                                if (exp1.Length > 0)
                                {
                                    string sWhereParamString = string.Empty;
                                    var sWhereParamsList = exp.Split(']');
                                    if (sWhereParamsList != null && sWhereParamsList.Count() > 0)
                                    {
                                        int iWhereParamsListCount = sWhereParamsList.Count();
                                        sWhereParamString = sWhereParamsList[iWhereParamsListCount - 1];
                                        if (sWhereParamString.Contains(":"))
                                        {
                                            sWhereParamString = sWhereParamString.TrimStart(',');
                                            var oLoadByAttrs = sWhereParamString.Split('_');
                                            if (oLoadByAttrs != null && oLoadByAttrs.Count() > 0)
                                            {
                                                foreach (var sAttr in oLoadByAttrs)
                                                {
                                                    if (!string.IsNullOrEmpty(sAttr))
                                                    {
                                                        oWhereParams.Add(new CNV { sName = sAttr.Split(':')[0].Trim(), sValue = sAttr.Split(':')[1] });
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    string sBOName = exp.Split('[')[0].Trim();
                                    string sResultString = string.Empty;
                                    StringBuilder sb = new StringBuilder();
                                    int i = 1;
                                    if (!string.IsNullOrEmpty(sBOName))
                                    {
                                        var sScript = "oStructureInstance.oChildBOI(" + '"' + sBOName + '"' + ")";
                                        var sNotation = sScript;
                                        var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, 0, "(||)", sBOName);
                                        if (!oResult.bOK && oResult.oResult == null)
                                        {
                                            sUnresolvedNotation += "Notation : " + sScript;
                                        }
                                        else
                                        {
                                            var oXIIBO = (List<XIIBO>)oResult.oResult;
                                            List<XIIBO> oBOIList = oXIIBO;
                                            if (oWhereParams != null && oWhereParams.Count() > 0)
                                            {
                                                oXIIBO = new List<XIIBO>();
                                                if (oBOIList != null && oBOIList.Count() > 0)
                                                {
                                                    foreach (var oBOI in oBOIList)
                                                    {
                                                        foreach (var sWhereParam in oWhereParams)
                                                        {
                                                            if (oBOI.Attributes.ContainsKey(sWhereParam.sName))
                                                            {
                                                                if (oBOI.AttributeI(sWhereParam.sName).sValue == sWhereParam.sValue)
                                                                {
                                                                    oXIIBO.Add(oBOI);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            int iActualCount = 0; int iRequiredCount = 0; int iCommaCount = 0;
                                            //if (oContent.Category == 20)
                                            //{
                                            //    if (sBOName.ToLower() == "Driver_T".ToLower())
                                            //    {
                                            //        iActualCount = oContent.iDriverEDICount;
                                            //        iCommaCount = oContent.iDriverEDICommaCount;
                                            //    }
                                            //    if (sBOName.ToLower() == "Conviction_T".ToLower())
                                            //    {
                                            //        iActualCount = oContent.iCovictionEDICount;
                                            //        iCommaCount = oContent.iConvictionEDICommaCount;
                                            //    }
                                            //    if (sBOName.ToLower() == "Claim_T".ToLower())
                                            //    {
                                            //        iActualCount = oContent.iClaimEDICount;
                                            //        iCommaCount = oContent.iClaimEDICommaCount;
                                            //    }
                                            //    if (sBOName.ToLower() == "Term_T".ToLower())
                                            //    {
                                            //        iActualCount = oContent.iEndorsementEDICount;
                                            //        iCommaCount = oContent.iEndorsementEDICommaCount;
                                            //    }

                                            //}
                                            if (oXIIBO != null && oXIIBO.Count() > 0)
                                            {
                                                int iCount = oXIIBO.Count();

                                                if (oXIIBO != null && oXIIBO.Count() > 0)
                                                {
                                                    oXIIBO.ForEach(m =>
                                                    {
                                                        XIBOInstance oBOInstance = new XIBOInstance();
                                                        List<XIIBO> ListXIIBO = new List<XIIBO>();
                                                        Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                                        ListXIIBO.Add(m);
                                                        oStructure[sBOName] = ListXIIBO;
                                                        oBOInstance.oStructureInstance = oStructure;
                                                        XIContentEditors oContentDef = new XIContentEditors();
                                                        oContentDef.Content = exp1.Replace("[|", "").Replace("|]", "");
                                                        if (exp1.Contains("{{"))
                                                        {
                                                            var oCResultobj = XIContentResult.MergeContentTemplate(oContentDef, oBOInstance, 0, 0);
                                                            sb.Append((string)oCResultobj.oResult);
                                                        }
                                                        else
                                                        {
                                                            if (iLoopCount != 0)
                                                            {
                                                                sb.Append(exp1.Replace("[|", "").Replace("|]", "").Replace("@@", iLoopCount.ToString()));
                                                            }
                                                            else
                                                            {
                                                                sb.Append(exp1.Replace("[|", "").Replace("|]", "").Replace("@@", i.ToString()));
                                                            }
                                                        }
                                                        if (sb.Length == 0)
                                                        {
                                                            delimterNotaion += ',';
                                                        }
                                                        if (i != oXIIBO.Count())
                                                        {
                                                            if (sb.Length != 0)
                                                            {
                                                                sb.Append(',');
                                                            }
                                                        }
                                                        i++;
                                                    });
                                                }
                                                string sAddCommaString = string.Empty;
                                                StringBuilder sCommaSb = new StringBuilder();
                                                if (iActualCount != iCount)
                                                {
                                                    //string sSubString = exp1.Replace("[|", "").Replace("|]", "");
                                                    //string[] sCommaCountArray = sSubString.Split(',');
                                                    //iCommaCount = sCommaCountArray.Count();
                                                    //if(iCommaCount==1)
                                                    //{
                                                    //    for (int k = 1; k <= iCommaCount; k++)
                                                    //    {
                                                    //        sCommaSb.Append(",");
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    for (int k = 1; k <= iCommaCount; k++)
                                                    //    {
                                                    //        sCommaSb.Append(",");
                                                    //    }
                                                    //}
                                                    for (int k = 1; k <= iCommaCount; k++)
                                                    {
                                                        sCommaSb.Append(",");
                                                    }
                                                    if (iCount < iActualCount)
                                                    {
                                                        iRequiredCount = iActualCount - iCount;
                                                    }
                                                }
                                                if (iRequiredCount != 0)
                                                {
                                                    for (int len = 1; len <= iRequiredCount; len++)
                                                    {
                                                        if (exp1.Contains("{{"))
                                                        {
                                                            sb.Append(sCommaSb);
                                                        }
                                                        else
                                                        {
                                                            sb.Append("," + exp1.Replace("[|", "").Replace("|]", "").Replace("@@", i.ToString()));
                                                            i++;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                int iCount = 0;
                                                XIContentEditors oContentDef = new XIContentEditors();
                                                oContentDef.Content = exp1.Replace("[|", "").Replace("|]", "");
                                                StringBuilder sCommaSb = new StringBuilder();
                                                if (iActualCount != iCount)
                                                {
                                                    //string sSubString = exp1.Replace("[|", "").Replace("|]", "");
                                                    //string[] sCommaCountArray = sSubString.Split(',');
                                                    //iCommaCount = sCommaCountArray.Count();
                                                    //if (iCommaCount == 1)
                                                    //{
                                                    //    for (int k = 1; k <= iCommaCount; k++)
                                                    //    {
                                                    //        sCommaSb.Append(",");
                                                    //    }
                                                    //}
                                                    //else
                                                    //{
                                                    //    for (int k = 1; k < iCommaCount; k++)
                                                    //    {
                                                    //        sCommaSb.Append(",");
                                                    //    }
                                                    //}
                                                    for (int k = 1; k <= iCommaCount; k++)
                                                    {
                                                        sCommaSb.Append(",");
                                                    }
                                                    if (iCount < iActualCount)
                                                    {
                                                        iRequiredCount = iActualCount - iCount;
                                                    }
                                                }
                                                if (iRequiredCount != 0)
                                                {
                                                    for (int len = 1; len <= iRequiredCount; len++)
                                                    {
                                                        if (exp1.Contains("{{"))
                                                        {
                                                            sb.Append(sCommaSb.ToString());
                                                        }
                                                        else
                                                        {
                                                            sb.Append(exp1.Replace("[|", "").Replace("|]", "").Replace("@@", len.ToString()));
                                                            if (len != iRequiredCount)
                                                            {
                                                                sb.Append(",");
                                                            }
                                                        }
                                                    }
                                                }
                                                if (sb.Length != 0)
                                                {
                                                    sb.Remove(sb.Length - 1, 1);
                                                }
                                            }
                                            SourceContent = SourceContent.Replace(delimterNotaion, sb.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region TYPE6 {{{}}}
                    //var SourceContent = "";
                    oregexpatternObj = new Regex(Regex.Escape("{{{") + "(.+?)" + Regex.Escape("}}}"));
                    var ndictnotationsList = XIContentResult.GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                    if (ndictnotationsList.Count() > 0)
                    {
                        var oScrResultList = (CResult)oXIScript.EvaluateMultiNotation(ndictnotationsList, oStructureInstance, 0, "{{{}}}");
                        Dictionary<string, string> nDict = new Dictionary<string, string>();
                        if (oScrResultList.bOK && oScrResultList.oResult != null)
                        {
                            nDict = (Dictionary<string, string>)oScrResultList.oResult;
                            foreach (KeyValuePair<string, string> entry in nDict)
                            {
                                SourceContent = SourceContent.Replace("{{{" + entry.Key + "}}}", entry.Value);
                            }
                        }
                        else
                        {
                            foreach (var entry in ndictnotationsList)
                            {
                                SourceContent = SourceContent.Replace("{{{" + entry + "}}}", "");
                                sUnresolvedNotation += "Notation : {{{" + entry + "}}}";
                            }
                        }
                    }

                    #endregion


                    #region TYPE4 [[]]
                    //var SourceContent = "";
                    oregexpatternObj = new Regex(Regex.Escape("[[") + "(.+?)" + Regex.Escape("]]"));
                    //var oNotations = document.FindAll(oregexpatternObj);
                    TextSelection[] textSelections4 = document.FindAll(oregexpatternObj);
                    if (textSelections4 != null)
                    {
                        string[] searchedPlaceholders4 = new string[textSelections4.Length];
                        if (textSelections4 != null)
                        {
                            for (int i = 0; i < textSelections4.Length; i++)
                            {
                                searchedPlaceholders4[i] = textSelections4[i].SelectedText.Replace("[[", "").Replace("]]", "");
                            }

                        }
                        //var onotations = XIContentResult.GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                        //if (onotations != null && onotations.Count() > 0)
                        //foreach (var onotations in textSelections)
                        //{
                        var oScrResult2 = (CResult)oXIScript.EvaluateMultiNotation(searchedPlaceholders4, oStructureInstance, 0, "[[]]");
                        Dictionary<string, string> oDict2 = new Dictionary<string, string>();
                        if (oScrResult2.bOK && oScrResult2.oResult != null && oScrResult2.oResult != "")
                        {
                            oDict2 = (Dictionary<string, string>)oScrResult2.oResult;
                            foreach (KeyValuePair<string, string> entry in oDict2)
                            {
                                document.Replace("[[" + searchedPlaceholders4[0] + "]]", entry.Value, false, false);
                                //SourceContent = SourceContent.Replace("[[" + entry.Key + "]]", entry.Value);
                            }
                        }
                        else
                        {
                            document.Replace(searchedPlaceholders4[0], string.Empty, false, false);
                            //foreach (var entry in onotations)
                            //{
                            //    SourceContent = SourceContent.Replace("[[" + entry + "]]", "");
                            //    sUnresolvedNotation += "Notation : [[" + entry + "]]";
                            //}
                        }
                    }
                    // }

                    //foreach (var exp in onotations)
                    //{
                    //    sNotation = exp;
                    //    string delimterNotaion = "[[" + exp + "]]";
                    //    string sformatted = "oStructureInstance." + exp;
                    //    var oResult = (CResult)oXIScript.EvalueNotation(exp, null);
                    //    string sReturnValue = (string)oResult.oResult;
                    //    // Replacing value in the SourceContent string
                    //    if (!string.IsNullOrEmpty(sReturnValue))
                    //    {
                    //        SourceContent = SourceContent.Replace(delimterNotaion, sReturnValue);
                    //    }
                    //    else
                    //    {
                    //        SourceContent = SourceContent.Replace(delimterNotaion, string.Empty);
                    //    }
                    //}
                    #endregion

                    #region TYPE5 [()]
                    //string sSourceContent = Copyobject.Content;
                    Regex regexpatternObj = new Regex(Regex.Escape("[(") + "(.*?)" + Regex.Escape(")]"));
                    TextSelection[] textSelections1 = document.FindAll(regexpatternObj);
                    if (textSelections1 != null)
                    {
                        string[] searchedPlaceholders1 = new string[textSelections1.Length];
                        for (int i = 0; i < textSelections1.Length; i++)
                        {
                            searchedPlaceholders1[i] = textSelections1[i].SelectedText.Replace("[(", "").Replace(")]", "");
                        }
                        // var odictnotations = XIContentResult.GetNotationsWithinDelimeter1(searchedPlaceholders1.Distinct().ToArray(), regexpatternObj);
                        if (searchedPlaceholders1.Count() > 0)
                        {
                            foreach (var exp in searchedPlaceholders1)
                            {

                                //var sNotation = exp.Replace("[(", "").Replace(")]", "");
                                string delimterNotaion = "[(" + exp + ")]";
                                List<string> aScript = exp.Split(',').ToList(); int iEmptyLoopCount = -1; int iTotalLoopCount = 0;
                                if (aScript.Count() == 4)
                                {
                                    if (!string.IsNullOrEmpty(aScript[3]))
                                    {
                                        iTotalLoopCount = Convert.ToInt32(aScript[3]);
                                        if (aScript.Any()) //prevent IndexOutOfRangeException for empty list
                                        {
                                            aScript.RemoveAt(aScript.Count - 1);
                                        }
                                    }
                                }
                                if (iTotalLoopCount == 1)
                                {
                                    var BOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                    string sScript = "oStructureInstance." + aScript[0];
                                    var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, 0, "[()]", BOName);
                                    var oXIIBO = (List<XIIBO>)oResult.oResult;
                                    string sFinalContent = string.Empty; StringBuilder sFinalContentSb = new StringBuilder();
                                    XIBOInstance oBOInstance = new XIBOInstance();
                                    //List<XIIBO> ListXIIBO = new List<XIIBO>();
                                    Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                    oStructure[BOName] = oXIIBO;
                                    oBOInstance.oStructureInstance = oStructure;
                                    var oBODCache = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName);
                                    XIDefinitionBase oDefBase = new XIDefinitionBase();
                                    XIContentEditors oContentDef = new XIContentEditors();
                                    var oContentList = (List<XIContentEditors>)(oDefBase.Clone(oBODCache.Templates.Values.ToList()));
                                    if (oContentList != null && oContentList.Count > 0)
                                    {
                                        oContentDef = oContentList.Where(x => x.Name.ToLower() == aScript[1].ToLower()).FirstOrDefault();
                                        if (oContentDef != null)
                                        {
                                            var oContentCopy = (XIContentEditors)(oDefBase.Clone(oContentDef));
                                            var oCResultobj = XIContentResult.MergeContentTemplate(oContentCopy, oBOInstance, 0, 0);
                                            if (oCResultobj.bOK && oCResultobj.oResult != null)
                                            {
                                                sFinalContentSb.Append((string)oCResultobj.oResult);
                                            }
                                            else
                                            {
                                                sUnresolvedNotation += "Notation : " + delimterNotaion;
                                            }
                                        }
                                    }
                                    if (sFinalContentSb.Length != 0)
                                    {
                                        // Replacing value in the SourceContent string
                                        document.Replace(delimterNotaion, sFinalContentSb.ToString(), false, false);
                                    }
                                    else
                                    {
                                        document.Replace(delimterNotaion, string.Empty, false, false);
                                    }
                                }
                                else
                                {
                                    if (aScript.Count() == 3)
                                    {
                                        if (!string.IsNullOrEmpty(aScript[2]))
                                        {
                                            iEmptyLoopCount = Convert.ToInt32(aScript[2]);
                                            if (aScript.Any()) //prevent IndexOutOfRangeException for empty list
                                            {
                                                aScript.RemoveAt(aScript.Count - 1);
                                            }
                                        }
                                    }
                                    if (aScript.Count() == 2)
                                    {
                                        string sScript = "oStructureInstance." + aScript[0].Replace("\\", "");
                                        string sBOName = aScript[0].Split('(')[1].Trim(')').Replace("\\", "").ToString();
                                        sBOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                        var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, 0, "[()]", sBOName);
                                        var oXIIBO = (List<XIIBO>)oResult.oResult;
                                        StringBuilder sFinalContentSb = new StringBuilder();
                                        //var TestData = "";// "<p>";// "<html><body><table>";
                                        if (oXIIBO != null && oXIIBO.Count() > 0 && !string.IsNullOrEmpty(aScript[1]))
                                        {
                                            iLoopCount = 1;
                                            if (iEmptyLoopCount > 0)
                                            {
                                                if (oXIIBO.Count() <= iEmptyLoopCount)
                                                {
                                                    int iActualEmptyLoopCount = iEmptyLoopCount - oXIIBO.Count();
                                                    if (iActualEmptyLoopCount != 0)
                                                    {
                                                        XIIBO oBOI = new XIIBO();
                                                        for (int v = 1; v <= iActualEmptyLoopCount; v++)
                                                        {
                                                            oXIIBO.Add(oBOI);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (oXIIBO.Count >= iEmptyLoopCount)
                                                    {
                                                        IEnumerable<XIIBO> values = oXIIBO;
                                                        var oBOI = values.ElementAt(iEmptyLoopCount);
                                                        oXIIBO = new List<XIIBO>();
                                                        oXIIBO.Add(oBOI);
                                                    }
                                                }
                                            }
                                            else if (iEmptyLoopCount == 0)
                                            {
                                                if (oXIIBO.Count >= iEmptyLoopCount)
                                                {
                                                    IEnumerable<XIIBO> values = oXIIBO;
                                                    var oBOI = values.ElementAt(iEmptyLoopCount);
                                                    oXIIBO = new List<XIIBO>();
                                                    oXIIBO.Add(oBOI);
                                                    //document = document.Clone(exp);
                                                }
                                            }
                                            WordDocument Childdocument = new WordDocument();

                                            //foreach (var m in oXIIBO)
                                            //oXIIBO.ForEach(m =>
                                            // {
                                            var BOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                            XIContentEditors oContentDef = new XIContentEditors();
                                            //XIBOInstance oBOInstance = new XIBOInstance();
                                            // Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                            //ListXIIBO.Add(m);
                                            //ListXIIBO.Add(oXIIBO.FirstOrDefault());
                                            // oStructure[BOName] = ListXIIBO;
                                            //oBOInstance.oStructureInstance = oStructure;
                                            if (exp.Contains("MSDrivers HTML"))
                                            {
                                                Childdocument = new WordDocument(Server.MapPath("~/DocxTemplater/MSDrivers Html.Docx"));
                                                for (var m = 0; m < oXIIBO.Count; m++)
                                                {
                                                    List<XIIBO> ListXIIBO = new List<XIIBO>();
                                                    Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                                    XIBOInstance oBOInstance = new XIBOInstance();
                                                    ListXIIBO.Add(oXIIBO[m]);
                                                    //ListXIIBO.Add(oXIIBO.FirstOrDefault());
                                                    oStructure[BOName] = ListXIIBO;
                                                    oBOInstance.oStructureInstance = oStructure;
                                                    WordDocument Copydoc = Childdocument.Clone();

                                                    oCR = CopyDocumentMergingData(Copydoc, oBOInstance);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        document.Replace(delimterNotaion, Copydoc, false, false);
                                                        //var variable = document.GetText();
                                                        document.ReplaceSingleLine("Child Docx", delimterNotaion, false, false);
                                                        oTrace.oTrace.Add(oCR.oTrace);
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
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }
                                            }

                                            else if (exp.Contains("MSMedicalConditions HTML"))
                                            {
                                                Childdocument = new WordDocument(Server.MapPath("~/DocxTemplater/MedicalCondition-Html.Docx"));
                                                // foreach (var m in oXIIBO)
                                                for (var m = 0; m < oXIIBO.Count; m++)
                                                {
                                                    List<XIIBO> ListXIIBO = new List<XIIBO>();
                                                    Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                                    XIBOInstance oBOInstance = new XIBOInstance();
                                                    ListXIIBO.Add(oXIIBO[m]);
                                                    //ListXIIBO.Add(oXIIBO.FirstOrDefault());
                                                    oStructure[BOName] = ListXIIBO;
                                                    oBOInstance.oStructureInstance = oStructure;
                                                    WordDocument Copydoc = Childdocument.Clone();
                                                    // var text = Copydoc.GetText();
                                                    WTextBody textBody = null;
                                                    //Iterates sections in Word document.
                                                    for (int i = Copydoc.Sections.Count - 1; i >= 0; i--)
                                                    {
                                                        //Accesses the Body of section where all the contents in document are apart
                                                        textBody = Copydoc.Sections[i].Body;
                                                        //Removes the last empty page in the Word document
                                                        oCR = RemoveEmptyItems(textBody);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oTrace.oTrace.Add(oCR.oTrace);
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
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }
                                                        //Removes the empty sections in the document
                                                        if (textBody.ChildEntities.Count == 0)
                                                        {
                                                            int SectionIndex = document.ChildEntities.IndexOf(document.Sections[i]);
                                                            document.ChildEntities.RemoveAt(SectionIndex);
                                                        }
                                                    }

                                                    oCR = CopyDocumentMergingData(Copydoc, oBOInstance);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            if (m == oXIIBO.Count - 1)
                                                            {
                                                                //Accesses the instance of the first section in the Word document
                                                                WSection section = Copydoc.Sections[0];
                                                                //Accesses the instance of the first table in the section
                                                                WTable table = section.Tables[0] as WTable;
                                                                //Removes a table from the text body
                                                                if (section.Body.ChildEntities.Count > 1)
                                                                    section.Body.ChildEntities.RemoveAt(1);
                                                                else
                                                                    section.Body.ChildEntities.RemoveAt(0);
                                                                document.Replace("Child Docx", string.Empty, false, false);
                                                            }
                                                            document.Replace(delimterNotaion, Copydoc, false, false);
                                                            //var variable = document.GetText();
                                                            document.ReplaceSingleLine("Child Docx", delimterNotaion, false, false);
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                            oCResult.oResult = "Success";
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }

                                            }
                                            else if (exp.Contains("MSClaims HTML"))
                                            {
                                                Childdocument = new WordDocument(Server.MapPath("~/DocxTemplater/MSClaims HTML.Docx"));
                                                for (var m = 0; m < oXIIBO.Count; m++)
                                                {
                                                    List<XIIBO> ListXIIBO = new List<XIIBO>();
                                                    Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                                    XIBOInstance oBOInstance = new XIBOInstance();
                                                    ListXIIBO.Add(oXIIBO[m]);
                                                    //ListXIIBO.Add(oXIIBO.FirstOrDefault());
                                                    oStructure[BOName] = ListXIIBO;
                                                    oBOInstance.oStructureInstance = oStructure;
                                                    WordDocument Copydoc = Childdocument.Clone();
                                                    var text = Copydoc.GetText();
                                                    WTextBody textBody = null;
                                                    //Iterates sections in Word document.
                                                    for (int i = Copydoc.Sections.Count - 1; i >= 0; i--)
                                                    {
                                                        //Accesses the Body of section where all the contents in document are apart
                                                        textBody = Copydoc.Sections[i].Body;
                                                        //Removes the last empty page in the Word document
                                                        oCR = RemoveEmptyItems(textBody);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oTrace.oTrace.Add(oCR.oTrace);
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
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }

                                                    }

                                                    oCR = CopyDocumentMergingData(Copydoc, oBOInstance);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            if (m == oXIIBO.Count - 1)
                                                            {
                                                                //Accesses the instance of the first section in the Word document
                                                                WSection section = Copydoc.Sections[0];
                                                                //Accesses the instance of the first table in the section
                                                                WTable table = section.Tables[0] as WTable;
                                                                //Removes a table from the text body
                                                                if (section.Body.ChildEntities.Count > 1)
                                                                    section.Body.ChildEntities.RemoveAt(1);
                                                                else
                                                                    section.Body.ChildEntities.RemoveAt(0);
                                                                // document.Replace("Child Docx", string.Empty, false, false);
                                                            }
                                                            document.Replace(delimterNotaion, Copydoc, false, false);
                                                            //var variable = document.GetText();
                                                            document.Replace("Child Docx", delimterNotaion, false, false);
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                            oCResult.oResult = "Success";
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }
                                            }
                                            else if (exp.Contains("MSConvictions HTML"))
                                            {
                                                Childdocument = new WordDocument(Server.MapPath("~/DocxTemplater/MSConvictions HTML.Docx"));
                                                for (var m = 0; m < oXIIBO.Count; m++)
                                                {
                                                    List<XIIBO> ListXIIBO = new List<XIIBO>();
                                                    Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                                    XIBOInstance oBOInstance = new XIBOInstance();
                                                    ListXIIBO.Add(oXIIBO[m]);
                                                    //ListXIIBO.Add(oXIIBO.FirstOrDefault());
                                                    oStructure[BOName] = ListXIIBO;
                                                    oBOInstance.oStructureInstance = oStructure;
                                                    WordDocument Copydoc = Childdocument.Clone();
                                                    var text = Copydoc.GetText();
                                                    WTextBody textBody = null;
                                                    //Iterates sections in Word document.
                                                    for (int i = Copydoc.Sections.Count - 1; i >= 0; i--)
                                                    {
                                                        //Accesses the Body of section where all the contents in document are apart
                                                        textBody = Copydoc.Sections[i].Body;
                                                        //Removes the last empty page in the Word document
                                                        oCR = RemoveEmptyItems(textBody);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oTrace.oTrace.Add(oCR.oTrace);
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
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }
                                                    }

                                                    oCR = CopyDocumentMergingData(Copydoc, oBOInstance);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            if (m == oXIIBO.Count - 1)
                                                            {
                                                                //Accesses the instance of the first section in the Word document
                                                                WSection section = Copydoc.Sections[0];
                                                                //Accesses the instance of the first table in the section
                                                                WTable table = section.Tables[0] as WTable;
                                                                //Removes a table from the text body
                                                                section.Body.ChildEntities.RemoveAt(1);
                                                                //document.Replace("Child Docx", string.Empty, false, false);
                                                            }
                                                            document.Replace(delimterNotaion, Copydoc, false, false);
                                                            //var variable = document.GetText();
                                                            document.Replace("Child Docx", delimterNotaion, false, false);
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                            oCResult.oResult = "Success";
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                document.Replace(delimterNotaion, string.Empty, false, false);
                                                //sUnresolvedNotation += "Notation : " + delimterNotaion;
                                            }
                                        }
                                        else document.Replace(delimterNotaion, string.Empty, false, false);

                                        document.Replace(delimterNotaion, "", false, false);
                                    }
                                    if (aScript.Count == 5)
                                        document.Replace(delimterNotaion, string.Empty, false, false);
                                }
                            }
                        }
                    }
                    #endregion


                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: document="+ document + " oStructureInstance "+ oStructureInstance + " is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXIDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oCResult.oResult = document;
            return oCResult;
        }

        public CResult RemoveEmptyItems(WTextBody textBody)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Removes the last empty page in the Word document";//expalin about this method logic
            try
            {
                // oTrace.oParams.Add(new CNV { sName = "", sValue = "" });

                if (textBody != null)//check mandatory params are passed or not
                {
                    //A flag to determine any renderable item found in the Word document.
                    bool IsRenderableItem = false;
                    //Iterates into textbody items.
                    for (int itemIndex = textBody.ChildEntities.Count - 1; itemIndex >= 0 && !IsRenderableItem; itemIndex--)
                    {
                        #region Removes last empty paragraph
                        //Checks item is empty paragraph and removes it.
                        if (textBody.ChildEntities[itemIndex] is WParagraph)
                        {
                            WParagraph paragraph = textBody.ChildEntities[itemIndex] as WParagraph;
                            //Iterates into paragraph
                            for (int pIndex = paragraph.Items.Count - 1; pIndex >= 0; pIndex--)
                            {
                                ParagraphItem paragraphItem = paragraph.Items[pIndex];

                                //Removes page breaks
                                if ((paragraphItem is Break && (paragraphItem as Break).BreakType == BreakType.PageBreak))
                                    paragraph.Items.RemoveAt(pIndex);

                                //Check paragraph contains any renderable items.
                                else if (!(paragraphItem is BookmarkStart || paragraphItem is BookmarkEnd))
                                {
                                    //Found renderable item and break the iteration.
                                    IsRenderableItem = true;
                                    break;
                                }
                            }
                            //Remove empty paragraph and the paragraph with bookmarks only
                            if (paragraph.Items.Count == 0 || !IsRenderableItem)
                                textBody.ChildEntities.RemoveAt(itemIndex);
                        }

                        #endregion
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: textBody="+ textBody+" is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString(); oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXIDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oCResult.oResult = (int)xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;
        }
    }
}