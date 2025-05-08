using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using XIDatabase;
using xiEnumSystem;
using XISystem;

namespace XICore
{
    public class XIContentEditors : XIDefinitionBase
    {
        public int ID { get; set; }
        [AllowHtml]
        public string Content { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public int OrganizationID { get; set; }
        public int FKiApplicationID { get; set; }
        public int Type { get; set; }
        public int iTemplateType { get; set; }
        public string sTemplateHeader { get; set; }
        public int iParentID { get; set; }
        public bool bIsHavingAttachments { get; set; }
        public bool bIsPaswordProtected { get; set; }
        public int iSurNamePasswordRange { get; set; }
        public int iDOBPasswordRange { get; set; }
        public string sCSSFileName { get; set; }
        public int iTypeofPDF { get; set; }
        public string sSubject { get; set; }
        public int BO { get; set; }
        public string Email { get; set; }
        public string Feilds { get; set; }
        public string TypeC { get; set; }
        public int SContent { get; set; }
        public int DateField { get; set; }
        //public string str { get; set; }
        public string SMSContent { get; set; }
        public string sOrgDatabase { get; set; }
        public string sSessionID { get; set; }
        public int FKiProductID { get; set; }
        public int iStatus { get; set; }
        public DateTime dActiveFrom { get; set; }
        public DateTime dActiveTo { get; set; }

        public int iDriverEDICount { get; set; }
        public int iClaimEDICount { get; set; }
        public int iCovictionEDICount { get; set; }
        public int iEndorsementEDICount { get; set; }

        public int iDriverEDICommaCount { get; set; }
        public int iClaimEDICommaCount { get; set; }
        public int iConvictionEDICommaCount { get; set; }
        public int iEndorsementEDICommaCount { get; set; }
        public bool bIsEmail { get; set; }
        public int FKiClassID { get; set; }
        public string sCode { get; set; }
        public string sBCC { get; set; }
        public string sCC { get; set; }
        public string sFrom { get; set; }
        public int FkiServerID { get; set; }
        public bool bIsBCCOnly { get; set; }
        public string sUnresolvedNotation { get; set; }
        //public int iCount { get; set; }
        // XIInfraScript oXIScrpit = new XIInfraScript();
        XIIScript oXIScript = new XIIScript();
        XIInfraCache oCache = new XIInfraCache();
        public string sGUID { get; set; }

        public string sContainerHTML { get; set; }

        public Guid XIGUID {  get; set; }

        public CResult MergeTemplateContent(XIContentEditors oContent, XIBOInstance oStructureInstance, int iLoopCount = 0)
        {
            CResult oCResult = new CResult();
            //Get Template Result
            int iStartIndex = 0;
            XIIQS oQsInstance = new XIIQS();
            XIBOInstance oBOI = new XIBOInstance();
            string sFinalString = "";
            try
            {
                var oContentC = (XIContentEditors)oContent.Clone(oContent);
                //Replace the Notation for CSV 
                sFinalString = CheckCSVNotions(oContentC.Content, "(|", "|)");
                while (sFinalString.Length > 0 && sFinalString.Contains("[|"))
                {
                    string SubString = CheckCSVNotions(sFinalString, "[|", "|]");
                    if (SubString.Length > 0)
                    {
                        string sBOName = sFinalString.Replace("(|", "").Replace("|)", "").Split('[')[0].Trim();
                        string sResultString = string.Empty;
                        int i = 1;
                        if (!string.IsNullOrEmpty(sBOName))
                        {
                            var sScript = "oStructureInstance.oChildBOI(" + '"' + sBOName + '"' + ")";
                            var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContentC.ID, "", sBOName);
                            var oXIIBO = (List<XIIBO>)oResult.oResult;
                            int iActualCount = 0; int iRequiredCount = 0; int iCommaCount = 0;
                            //if (oContent.Name.ToLower() == "Policy CSV".ToLower())
                            if (oContent.Category == 20)
                            {
                                if (FKiProductID != 0)
                                {
                                    XIIXI oIXI = new XIIXI();
                                    var oProductI = oIXI.BOI("Product", FKiProductID.ToString());
                                    if (sBOName.ToLower() == "Driver_T".ToLower())
                                    {
                                        if (oProductI.Attributes.ContainsKey("iDriverEDICount"))
                                        {
                                            iActualCount = Convert.ToInt32(oProductI.AttributeI("iDriverEDICount").sValue);
                                        }
                                    }
                                    if (sBOName.ToLower() == "Conviction_T".ToLower())
                                    {
                                        if (oProductI.Attributes.ContainsKey("iCovictionEDICount"))
                                        {
                                            iActualCount = Convert.ToInt32(oProductI.AttributeI("iCovictionEDICount").sValue);
                                        }
                                    }
                                    if (sBOName.ToLower() == "Claim_T".ToLower())
                                    {
                                        if (oProductI.Attributes.ContainsKey("iClaimEDICount"))
                                        {
                                            iActualCount = Convert.ToInt32(oProductI.AttributeI("iClaimEDICount").sValue);
                                        }
                                    }
                                    if (sBOName.ToLower() == "Term_T".ToLower())
                                    {
                                        if (oProductI.Attributes.ContainsKey("iEndorsementEDICount"))
                                        {
                                            iActualCount = Convert.ToInt32(oProductI.AttributeI("iEndorsementEDICount").sValue);
                                        }
                                    }
                                }
                            }
                            if (oXIIBO != null)
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
                                        oContentDef.Content = SubString.Replace("[|", "").Replace("|]", "");
                                        if (SubString.Contains("{{"))
                                        {
                                            var oCResultobj = MergeTemplateContent(oContentDef, oBOInstance);
                                            sResultString += (string)oCResultobj.oResult;
                                        }
                                        else
                                        {
                                            if (iLoopCount != 0)
                                            {
                                                sResultString += SubString.Replace("[|", "").Replace("|]", "").Replace("@@", iLoopCount.ToString());
                                            }
                                            else
                                            {
                                                sResultString += SubString.Replace("[|", "").Replace("|]", "").Replace("@@", i.ToString());
                                            }
                                        }
                                        if (string.IsNullOrEmpty(sResultString))
                                        {
                                            sFinalString += ',';
                                        }
                                        if (i != oXIIBO.Count())
                                        {
                                            if (!string.IsNullOrEmpty(sResultString))
                                            {
                                                sResultString += ',';
                                            }
                                        }
                                        i++;
                                    });
                                }
                                string sAddCommaString = string.Empty;
                                if (iActualCount != iCount)
                                {
                                    string sSubString = SubString.Replace("[|", "").Replace("|]", "");
                                    string[] sCommaCountArray = sSubString.Split(',');
                                    iCommaCount = sCommaCountArray.Count();
                                    //for (int k = 0; k < iCommaCount - 1; k++)
                                    for (int k = 0; k < iCommaCount; k++)
                                    {
                                        sAddCommaString += ",";
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
                                        if (SubString.Contains("{{"))
                                        {
                                            sResultString += sAddCommaString;
                                        }
                                        else
                                        {
                                            sResultString += "," + SubString.Replace("[|", "").Replace("|]", "").Replace("@@", i.ToString());
                                            i++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int iCount = 0;
                                XIContentEditors oContentDef = new XIContentEditors();
                                oContentDef.Content = SubString.Replace("[|", "").Replace("|]", "");
                                string sAddCommaString = string.Empty;
                                if (iActualCount != iCount)
                                {
                                    string sSubString = SubString.Replace("[|", "").Replace("|]", "");
                                    string[] sCommaCountArray = sSubString.Split(',');
                                    iCommaCount = sCommaCountArray.Count();
                                    for (int k = 0; k < iCommaCount; k++)
                                    {
                                        sAddCommaString += ",";
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
                                        if (SubString.Contains("{{"))
                                        {
                                            sResultString += sAddCommaString;
                                        }
                                        else
                                        {
                                            sResultString += SubString.Replace("[|", "").Replace("|]", "").Replace("@@", len.ToString());
                                            if (len != iRequiredCount)
                                            {
                                                sResultString = sResultString + ",";
                                            }
                                            //i++;
                                        }
                                    }
                                }
                                sResultString = sResultString.Remove(sResultString.LastIndexOf(","));
                                //sResultString += SubString.Replace("[|", "").Replace("|]", "").Replace("@@", i.ToString());
                            }
                            oContentC.Content = oContentC.Content.Replace(sFinalString, sResultString);
                        }
                    }
                    sFinalString = CheckCSVNotions(oContentC.Content, "(|", "|)");
                }

                //Check if htmlcontent has structure notaion or not 
                sFinalString = CheckModelHtmlContentNoation(oContentC.Content);
                while (sFinalString != null && sFinalString.Length > 0)
                {
                    Dictionary<string, string> sNotations = new Dictionary<string, string>();
                    sNotations.Add(sFinalString, "");
                    if (sFinalString.Contains("."))
                    {
                        List<CNV> oParamsList = new List<CNV>();
                        string sScript = sFinalString.Replace("{{", "").Replace("}}", "");
                        sScript = "oStructureInstance." + sScript;
                        //Get structure notaion result
                        var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContentC.ID);
                        string sReturnValue = (string)oResult.oResult;
                        if (!string.IsNullOrEmpty(sReturnValue))
                        {
                            oContentC.Content = oContentC.Content.Replace(sFinalString, sReturnValue);
                        }
                        else
                        {
                            oContentC.Content = oContentC.Content.Replace(sFinalString, string.Empty);
                        }
                    }
                    //Check if htmlcontent has another structure notaion or not 
                    sFinalString = CheckModelHtmlContentNoation(oContentC.Content);
                }

                sFinalString = SubStringNotation(oContentC.Content, "<|", "|>");
                while (sFinalString != null && sFinalString.Length > 0)
                {
                    string sScriptString = sFinalString.Replace("<|", "").Replace("|>", "");
                    if (sScriptString.StartsWith("<xi.s"))
                    {
                        string sGUID = Guid.NewGuid().ToString();
                        CNV oParam = new CNV();
                        List<CNV> oNVList = new List<CNV>();
                        //string sScript = SubStringNotation(sScriptString, "<", ">");
                        //sScript = sScript.Replace("<", "").Replace(">", "");
                        int iLastStrinfIndex = sScriptString.LastIndexOf(">");
                        int iStartStringIndex = sScriptString.IndexOf("<");
                        string sScript = sScriptString.Substring(iStartStringIndex, iLastStrinfIndex);
                        sScript = sScript.Remove(iStartStringIndex, 1);
                        if (sScriptString.Contains("}>{"))
                        {
                            int iScriptColumnLastIndex = sScriptString.LastIndexOf(">");
                            string sScriptColumn = sScriptString.Substring(iScriptColumnLastIndex).Replace(">", "");
                            // if (!string.IsNullOrEmpty(sScriptColumn))
                            //{
                            sScriptColumn = sScriptColumn.Replace("{", "").Replace("}", "");
                            string sScriptBOName = sScriptColumn.Split('.')[0];
                            sScriptColumn = sScriptColumn.Split('.')[1];
                            string sInstnaceID = string.Empty;
                            sInstnaceID = oStructureInstance.oSubStructureI(sScriptBOName).Item(0).AttributeI(sScriptColumn).sResolvedValue;
                            oParam.sName = "sScript";
                            oParam.sValue = sScript;
                            oNVList.Add(oParam);
                            oParam = new CNV();
                            oParam.sName = "-" + sScriptColumn;
                            oParam.sValue = sInstnaceID;
                            oNVList.Add(oParam);
                            oCache.SetXIParams(oNVList, sGUID, sSessionID);
                            //}
                        }
                        CResult oResult = new CResult();
                        XIDScript oXIScript = new XIDScript();
                        oXIScript.sScript = sScript.ToString();
                        oResult = oXIScript.Execute_Script(sGUID, sSessionID);
                        string sValue = string.Empty;
                        if (oResult.bOK && oResult.oResult != null)
                        {
                            sValue = (string)oResult.oResult;
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                //sValue = sValue.Replace("|||", ",");
                                oContentC.Content = oContentC.Content.Replace(sFinalString, sValue);
                            }
                            else
                            {
                                oContentC.Content = oContentC.Content.Replace(sFinalString, string.Empty);
                            }
                        }

                        //string[] sScriptColumnArray = sScriptString.Split('>');

                        //else
                        //{

                        //}
                    }
                    //Check if htmlcontent has another structure notaion or not 
                    sFinalString = SubStringNotation(oContentC.Content, "<|", "|>");
                }
                //Check if htmlcontent has structure notaion list or not 
                //not in use
                sFinalString = CheckCollectionNoation(oContentC.Content);
                while (sFinalString != null && sFinalString.Length > 0)
                {
                    //var sNotationValues = oQsInstance.GetNotationValue(oXIValues, oSubStructuresList, sNotations);
                    List<CNV> oParamsList = new List<CNV>();
                    string sScript = sFinalString.Replace("[{", "").Replace("}]", "");
                    sScript = "oStructureInstance." + sScript;
                    var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContentC.ID);
                    List<XIIBO> oBOIList = (List<XIIBO>)oResult.oResult;
                    if (oBOIList.Count() > 0)
                    {
                        oContentC.Content = ReplaceHtmlContentWithData(oContentC.Content, sFinalString, oBOIList);
                    }
                    else
                    {
                        oContentC.Content = oContentC.Content.Replace(sFinalString, string.Empty);
                    }
                    //Check if htmlcontent has another structure notaion list or not 
                    sFinalString = CheckCollectionNoation(oContentC.Content);
                }
                //Check if htmlcontent has function notaion or not
                sFinalString = CheckHtmlfunctionsNoation(oContentC.Content);
                while (sFinalString.Length > 0)
                {
                    string sFunctionString = sFinalString.Replace("[[", "").Replace("]]", "");
                    List<CNV> oParamsList = new List<CNV>();
                    string sScript = sFunctionString;
                    //Get function notaion result
                    var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContentC.ID);
                    string sReturnValue = (string)oResult.oResult;
                    if (!string.IsNullOrEmpty(sReturnValue))
                    {
                        oContentC.Content = oContentC.Content.Replace(sFinalString, sReturnValue);
                    }
                    else
                    {
                        oContentC.Content = oContentC.Content.Replace(sFinalString, string.Empty);
                    }
                    //Check if htmlcontent has another function notaion or not
                    sFinalString = CheckHtmlfunctionsNoation(oContentC.Content);
                }
                //sFinalString = CheckModelHtmlContentNoation(oContent.Content);
                //while (sFinalString != null && sFinalString.Length > 0)
                //{
                //    oContent.Content = ReplaceHtmlWithDynamicModelData(oContent, sFinalString, iInstanceID);//Replace model properties with model values
                //    sFinalString = CheckModelHtmlContentNoation(oContent.Content);//Check if htmlcontent has another model properties
                //}
                //ViewBag.HtmlContent = oContent.Content;


                //Create XML,HTML Based On Notation for List Of Sub BO's
                sFinalString = CheckSubNotation(oContentC.Content);
                while (sFinalString.Length > 0)
                {
                    string sReplace = sFinalString.Replace("[(", "").Replace(")]", "");
                    List<string> aScript = sReplace.Split(',').ToList();
                    if (aScript.Count() == 2)
                    {
                        string sScript = "oStructureInstance." + aScript[0];
                        string sBOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                        var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContent.ID, "[()]", sBOName);
                        var oXIIBO = (List<XIIBO>)oResult.oResult;
                        string sFinalContent = string.Empty;

                        if (oXIIBO != null && oXIIBO.Count() > 0 && !string.IsNullOrEmpty(aScript[1]))
                        {
                            iLoopCount = 1;
                            oXIIBO.ForEach(m =>
                            {
                                string BOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                XIContentEditors oContentDef = new XIContentEditors();
                                XIBOInstance oBOInstance = new XIBOInstance();
                                List<XIIBO> ListXIIBO = new List<XIIBO>();
                                Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                ListXIIBO.Add(m);
                                oStructure[BOName] = ListXIIBO;
                                oBOInstance.oStructureInstance = oStructure;
                                XIInfraCache oCache = new XIInfraCache();
                                var oBODCache = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName);
                                XIDefinitionBase oDefBase = new XIDefinitionBase();

                                var oContentList = (List<XIContentEditors>)(oDefBase.Clone(oBODCache.Templates.Values.ToList()));
                                if (oContentList != null && oContentList.Count > 0)
                                {
                                    oContentDef = oContentList.Where(s => s.Name.ToLower() == aScript[1].ToLower()).FirstOrDefault();
                                    if (oContentDef != null)
                                    {
                                        var oContentCopy = (XIContentEditors)(oDefBase.Clone(oContentDef));
                                        var oCResultobj = MergeTemplateContent(oContentCopy, oBOInstance, iLoopCount);
                                        sFinalContent += (string)oCResultobj.oResult;//Append List of Sub BO XML Content for Replace the Notation
                                    }
                                }
                                iLoopCount++;
                            });
                        }
                        if (!string.IsNullOrEmpty(sFinalContent))
                        {
                            oContentC.Content = oContentC.Content.Replace(sFinalString, sFinalContent);
                        }
                        else
                        {
                            oContentC.Content = oContentC.Content.Replace(sFinalString, string.Empty);
                        }
                        sFinalString = CheckSubNotation(oContentC.Content);
                    }
                }
                oContentC.Content = oContentC.Content.Replace("|||", ",");
                if (!string.IsNullOrEmpty(oContentC.Content))
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                oCResult.oResult = oContentC.Content;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [ Merge Script:" + sFinalString + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            // MergeContentTemplate(oContent, oStructureInstance);
            return oCResult;
        }
        public CResult MergeContentTemplate(XIContentEditors oContent, XIBOInstance oStructureInstance, int iLoopCount = 0, int Type = 0)
        {
            iTemplateType = Type == 0 ? oContent.iTemplateType : Type;
            CResult oCResult = new CResult();
            var Copyobject = oContent.GetCopy();
            string SourceContent = Copyobject.Content; string sNotation = string.Empty;
            try
            {
                /*TO DO MERGE CONTENT FOR DIFFERENT TYPE OF NOTATIONS*/
                /* {{}},CSV,IF,Child Templates */
                Regex oregexpatternObj = null;
                #region TYPE0 [{}]
                oregexpatternObj = new Regex(Regex.Escape("[{") + "(.+?)" + Regex.Escape("}]"));
                var dictnotations1 = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                if (dictnotations1.Count() > 0)
                {
                    foreach (var item in dictnotations1)
                    {
                        var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, item, "0");
                        oCResult = MergeContentTemplate(oContentDef.First(), oStructureInstance);
                        if (oCResult.bOK && oCResult.oResult != null)
                        {
                            SourceContent = SourceContent.Replace("[{" + item + "}]", (string)oCResult.oResult);
                        }
                        else
                        {
                            SourceContent = SourceContent.Replace("[{" + item + "}]", oContentDef.First().Content.ToString());
                        }
                    }
                }
                #endregion
                #region TYPE1 (||)
                oregexpatternObj = new Regex(Regex.Escape("(|") + "(.*?)" + Regex.Escape("|)"));
                var nnotations = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj);
                foreach (var exp in nnotations)
                {
                    string delimterNotaion = "(|" + exp + "|)";
                    if (exp.Length > 0 && exp.Contains("[|"))
                    {
                        oregexpatternObj = new Regex(Regex.Escape("[|") + "(.*?)" + Regex.Escape("|]"));
                        var nSubDicnotations = GetNotationsWithinDelimeter(exp, oregexpatternObj);
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
                                    sNotation = sScript;
                                    var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContent.ID, "(||)", sBOName);
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
                                        if (oContent.Category == 20)
                                        {
                                            if (sBOName.ToLower() == "Driver_T".ToLower())
                                            {
                                                iActualCount = oContent.iDriverEDICount;
                                                iCommaCount = oContent.iDriverEDICommaCount;
                                            }
                                            if (sBOName.ToLower() == "Conviction_T".ToLower())
                                            {
                                                iActualCount = oContent.iCovictionEDICount;
                                                iCommaCount = oContent.iConvictionEDICommaCount;
                                            }
                                            if (sBOName.ToLower() == "Claim_T".ToLower())
                                            {
                                                iActualCount = oContent.iClaimEDICount;
                                                iCommaCount = oContent.iClaimEDICommaCount;
                                            }
                                            if (sBOName.ToLower() == "Term_T".ToLower())
                                            {
                                                iActualCount = oContent.iEndorsementEDICount;
                                                iCommaCount = oContent.iEndorsementEDICommaCount;
                                            }
                                            //if (FKiProductID != 0)
                                            //{
                                            //var oProductI = oIXI.BOI("Product", FKiProductID.ToString());
                                            //if (sBOName.ToLower() == "Driver_T".ToLower())
                                            //{
                                            //    if (oProductI.Attributes.ContainsKey("iDriverEDICount"))
                                            //    {
                                            //        iActualCount = Convert.ToInt32(oProductI.AttributeI("iDriverEDICount").sValue);
                                            //    }
                                            //    if (oProductI.Attributes.ContainsKey("iDriverEDICommaCount"))
                                            //    {
                                            //        iCommaCount = Convert.ToInt32(oProductI.AttributeI("iDriverEDICommaCount").sValue);
                                            //    }
                                            //}
                                            //if (sBOName.ToLower() == "Conviction_T".ToLower())
                                            //{
                                            //    if (oProductI.Attributes.ContainsKey("iCovictionEDICount"))
                                            //    {
                                            //        iActualCount = Convert.ToInt32(oProductI.AttributeI("iCovictionEDICount").sValue);
                                            //    }
                                            //    if (oProductI.Attributes.ContainsKey("iConvictionEDICommaCount"))
                                            //    {
                                            //        iCommaCount = Convert.ToInt32(oProductI.AttributeI("iConvictionEDICommaCount").sValue);
                                            //    }
                                            //}
                                            //if (sBOName.ToLower() == "Claim_T".ToLower())
                                            //{
                                            //    if (oProductI.Attributes.ContainsKey("iClaimEDICount"))
                                            //    {
                                            //        iActualCount = Convert.ToInt32(oProductI.AttributeI("iClaimEDICount").sValue);
                                            //    }
                                            //    if (oProductI.Attributes.ContainsKey("iClaimEDICommaCount"))
                                            //    {
                                            //        iCommaCount = Convert.ToInt32(oProductI.AttributeI("iClaimEDICommaCount").sValue);
                                            //    }
                                            //}
                                            //if (sBOName.ToLower() == "Term_T".ToLower())
                                            //{
                                            //    if (oProductI.Attributes.ContainsKey("iEndorsementEDICount"))
                                            //    {
                                            //        iActualCount = Convert.ToInt32(oProductI.AttributeI("iEndorsementEDICount").sValue);
                                            //    }
                                            //    if (oProductI.Attributes.ContainsKey("iEndorsementEDICommaCount"))
                                            //    {
                                            //        iCommaCount = Convert.ToInt32(oProductI.AttributeI("iEndorsementEDICommaCount").sValue);
                                            //    }
                                            //}
                                            //}
                                        }
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
                                                        var oCResultobj = MergeContentTemplate(oContentDef, oBOInstance, 0, iTemplateType);
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
                oregexpatternObj = new Regex(Regex.Escape("{{{") + "(.+?)" + Regex.Escape("}}}"));
                var ndictnotationsList = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                if (ndictnotationsList.Count() > 0)
                {
                    var oScrResultList = (CResult)oXIScript.EvaluateMultiNotation(ndictnotationsList, oStructureInstance, oContent.ID, "{{{}}}");
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
                #region TYPE2 {{}}
                oregexpatternObj = new Regex(Regex.Escape("{{") + "(.+?)" + Regex.Escape("}}"));
                Dictionary<string, string> ResolvedNotations = new Dictionary<string, string>();
                var dictnotations = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                var BoName = "";
                if (dictnotations.Count() > 0)
                {
                    var NewNotations = dictnotations.Where(m => m.StartsWith("|")).ToList();
                    if (NewNotations != null && NewNotations.Count() > 0)
                    {
                        var oCR = new CResult();
                        if (oStructureInstance.BOI == null)
                        {
                            oStructureInstance.BOI = oStructureInstance.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                            oStructureInstance.BOI.BOD.Name= oStructureInstance.oStructureInstance.FirstOrDefault().Key;
                            BoName = oStructureInstance.BOI.BOD.Name;
                        }
                        else
                        {
                            BoName = oStructureInstance.BOI.BOD.Name;
                        }
                        oCR = Build_FullNotations(dictnotations, oStructureInstance.BOI.BOD.Name);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            ResolvedNotations = ((Dictionary<string, string>)oCR.oResult);
                            dictnotations = ResolvedNotations.Keys.ToArray();
                        }
                    }
                    var NonresolvedNotations = dictnotations.Where(m => m.StartsWith("|")).ToList();
                    if (NonresolvedNotations != null && NonresolvedNotations.Count() > 0)
                    {
                        sUnresolvedNotation += "New notations were not resolved properly";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(BoName))
                        {
                            BoName = "{{}}";
                        }
                        var oScrResult = (CResult)oXIScript.EvaluateMultiNotation(dictnotations, oStructureInstance, oContent.ID, BoName);
                        Dictionary<string, string> oDict = new Dictionary<string, string>();
                        if (oScrResult.bOK && oScrResult.oResult != null)
                        {
                            oDict = (Dictionary<string, string>)oScrResult.oResult;
                            foreach (KeyValuePair<string, string> entry in oDict)
                            {
                                if (entry.Key.Contains("GetConcatStringFromList"))
                                {
                                    SourceContent = SourceContent.Replace("{{" + entry.Key + "}}", entry.Value);
                                }
                                else
                                {
                                    var sKey = string.Empty;
                                    var sSimpleNotation = string.Empty;
                                    if (ResolvedNotations.ContainsKey(entry.Key)) { sSimpleNotation = ResolvedNotations[entry.Key]; }
                                    if (!string.IsNullOrEmpty(sSimpleNotation))
                                    {
                                        sKey = sSimpleNotation;
                                    }
                                    else
                                    {
                                        sKey = entry.Key;
                                    }
                                    SourceContent = SourceContent.Replace("{{" + sKey + "}}", iTemplateType == 20 ? (string.IsNullOrEmpty(entry.Value) || !entry.Value.Contains(",") ? entry.Value : "\"" + entry.Value.Replace(",", "|||") + "\"") : (string.IsNullOrEmpty(entry.Value) ? entry.Value : entry.Value.Replace(",", "|||")));
                                }
                            }
                        }
                        else
                        {
                            foreach (var entry in dictnotations)
                            {
                                SourceContent = SourceContent.Replace("{{" + entry + "}}", "");
                                sUnresolvedNotation += "Notation : {{" + entry + "}}";
                            }
                        }
                    }
                }
                //foreach (var exp in dictnotations)
                //{
                //    sNotation = exp;
                //    string sformatted = "oStructureInstance." + exp;
                //    var oResult = (CResult)oXIScript.EvalueNotation(sformatted, oStructureInstance);
                //    string sReturnValue = (string)oResult.oResult;
                //    string delimterNotaion = "{{" + exp + "}}";
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

                #region TYPE3 <||>
                List<string> oList = new List<string>();
                var ndictnotations = (List<string>)SubStringNotationList(SourceContent, "<|", "|>", oList);
                foreach (var exp in ndictnotations)
                {
                    sNotation = exp;
                    CResult oResult = XIIScriptResolving(oStructureInstance, exp);
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
                            SourceContent = SourceContent.Replace(exp, sValue.Contains("\r\n") ? sValue.TrimEnd('\n', '\r') : sValue);
                            XIContentEditors oXISubNotationContent = new XIContentEditors();
                            oXISubNotationContent.Content = sValue;
                            var oCResultobj = MergeContentTemplate(oXISubNotationContent, oStructureInstance, 0, iTemplateType);
                            if (oCResultobj.bOK && oCResultobj.oResult != null)
                            {
                                string sXISubNotationResult = (string)oCResultobj.oResult;
                                if (!string.IsNullOrEmpty(sXISubNotationResult))
                                {
                                    SourceContent = SourceContent.Replace(sValue, sXISubNotationResult);
                                }
                                else
                                {
                                    SourceContent = SourceContent.Replace(sValue, string.Empty);
                                }
                            }

                        }
                        else
                        {
                            SourceContent = SourceContent.Replace(exp, string.Empty);
                        }
                    }
                    else
                    {
                        sUnresolvedNotation += "Notation : " + exp;
                    }
                    // string sSubNotationValue = string.Empty;

                }
                #endregion

                #region TYPE4 [[]]
                oregexpatternObj = new Regex(Regex.Escape("[[") + "(.+?)" + Regex.Escape("]]"));
                var onotations = GetNotationsWithinDelimeter(SourceContent, oregexpatternObj).ToArray();
                if (onotations != null && onotations.Count() > 0)
                {
                    var oScrResult2 = (CResult)oXIScript.EvaluateMultiNotation(onotations, oStructureInstance, oContent.ID, "[[]]");
                    Dictionary<string, string> oDict2 = new Dictionary<string, string>();
                    if (oScrResult2.bOK && oScrResult2.oResult != null)
                    {
                        oDict2 = (Dictionary<string, string>)oScrResult2.oResult;
                        foreach (KeyValuePair<string, string> entry in oDict2)
                        {
                            SourceContent = SourceContent.Replace("[[" + entry.Key + "]]", entry.Value);
                        }
                    }
                    else
                    {
                        foreach (var entry in onotations)
                        {
                            SourceContent = SourceContent.Replace("[[" + entry + "]]", "");
                            sUnresolvedNotation += "Notation : [[" + entry + "]]";
                        }
                    }
                }

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
                var odictnotations = GetNotationsWithinDelimeter(SourceContent, regexpatternObj);
                if (odictnotations.Count() > 0)
                {
                    CResult ocrs = new CResult();
                    List<string> info = new List<string>();
                    foreach (var exp in odictnotations)
                    {
                        info.Add("oChild Notations " + exp);
                        sNotation = exp;
                        string delimterNotaion = "[(" + exp + ")]";
                        List<string> aScript = exp.Split(',').ToList(); int iEmptyLoopCount = -1; int iTotalLoopCount = 0;
                        if (aScript.Count() == 4)
                        {
                            iTotalLoopCount = Convert.ToInt32(aScript[3]);
                            if (aScript.Any()) //prevent IndexOutOfRangeException for empty list
                            {
                                aScript.RemoveAt(aScript.Count - 1);
                            }
                        }
                        if (iTotalLoopCount == 1)
                        {
                            string BOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                            string sScript = "oStructureInstance." + aScript[0];
                            var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContent.ID, "[()]", BOName);
                            var oXIIBO = (List<XIIBO>)oResult.oResult;
                            string sFinalContent = string.Empty; StringBuilder sFinalContentSb = new StringBuilder();
                            XIBOInstance oBOInstance = new XIBOInstance();
                            List<XIIBO> ListXIIBO = new List<XIIBO>();
                            Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                            oStructure[BOName] = oXIIBO;
                            oBOInstance.oStructureInstance = oStructure;
                            XIInfraCache oCache = new XIInfraCache();
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
                                    var oCResultobj = MergeContentTemplate(oContentCopy, oBOInstance, 0, iTemplateType);
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
                                SourceContent = SourceContent.Replace(delimterNotaion, sFinalContentSb.ToString());
                            }
                            else
                            {
                                SourceContent = SourceContent.Replace(delimterNotaion, string.Empty);
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
                                string sScript = "oStructureInstance." + aScript[0];
                                string sBOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                var oResult = (CResult)oXIScript.EvalueNotation(sScript, oStructureInstance, oContent.ID, "[()]", sBOName);
                                List<XIIBO> oXIIBO = (List<XIIBO>)oResult.oResult;
                                if (oXIIBO !=null && oXIIBO.Count > 0) {
                                    var Firstbo = oXIIBO.FirstOrDefault();
                                    if(Firstbo.BOD.TableName!=null && Firstbo.BOD.TableName.ToLower()== "productversions_t")
                                    {
                                        if (Firstbo.Attributes != null && (Firstbo.Attributes.ContainsKey("sCancellationPolicy") || Firstbo.Attributes.ContainsKey("scancellationpolicy")))
                                            info.Add("Cacellation script uppercase " + Firstbo.Attributes["sCancellationPolicy"].sValue);
                                            info.Add("Cacellation script lowercase" + Firstbo.Attributes["scancellationpolicy"].sValue);
                                    }
                                }
                                StringBuilder sFinalContentSb = new StringBuilder();
                                if(iEmptyLoopCount > 0 && (oXIIBO == null || oXIIBO.Count == 0) && !string.IsNullOrEmpty(aScript[1])) // to handle minimum no. of BOs when none exist.
                                {
                                    oXIIBO = new List<XIIBO>();
                                    oXIIBO.Add(new XIIBO());
                                }
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
                                        }
                                    }
                                    oXIIBO.ForEach(m =>
                                    {
                                        string BOName = aScript[0].Split('(')[1].Trim(')').Replace("\"", "").ToString();
                                        XIContentEditors oContentDef = new XIContentEditors();
                                        XIBOInstance oBOInstance = new XIBOInstance();
                                        List<XIIBO> ListXIIBO = new List<XIIBO>();
                                        Dictionary<string, List<XIIBO>> oStructure = new Dictionary<string, List<XIIBO>>();
                                        ListXIIBO.Add(m);
                                        oStructure[BOName] = ListXIIBO;
                                        oBOInstance.oStructureInstance = oStructure;
                                        XIInfraCache oCache = new XIInfraCache();
                                        var oBODCache = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName);
                                        if (oBODCache.TableName == null)
                                        {
                                            oBODCache = (XIDBO)oBOInstance.oStructureInstance.FirstOrDefault().Value.FirstOrDefault().BOD;//(XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, BOName);
                                        }
                                        XIDefinitionBase oDefBase = new XIDefinitionBase();

                                        if (oBODCache.Attributes.Count() == 0)
                                        {
                                            var oContentList = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, aScript[1].ToLower(), "0"); //oContentList.Where(x => x.Name.ToLower() == aScript[1].ToLower()).FirstOrDefault();
                                            oContentDef = oContentList.FirstOrDefault();
                                            if (oContentDef != null)
                                            {
                                                var oContentCopy = (XIContentEditors)(oDefBase.Clone(oContentDef));
                                                var oCResultobj = MergeContentTemplate(oContentCopy, oBOInstance, iLoopCount, iTemplateType);
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
                                        else
                                        {
                                            var oContentList = (List<XIContentEditors>)(oDefBase.Clone(oBODCache.Templates.Values.ToList()));
                                            if (oContentList != null && oContentList.Count > 0)
                                            {
                                                oContentDef = oContentList.Where(x => x.Name.ToLower() == aScript[1].ToLower()).FirstOrDefault();
                                                if (oContentDef != null)
                                                {
                                                    var oContentCopy = (XIContentEditors)(oDefBase.Clone(oContentDef));
                                                    var oCResultobj = MergeContentTemplate(oContentCopy, oBOInstance, iLoopCount, iTemplateType);
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
                                        }
                                        iLoopCount++;
                                    });
                                }

                                if (sFinalContentSb.Length != 0)
                                {
                                    // Replacing value in the SourceContent string
                                    SourceContent = SourceContent.Replace(delimterNotaion, sFinalContentSb.ToString());
                                }
                                else
                                {
                                    SourceContent = SourceContent.Replace(delimterNotaion, string.Empty);
                                }
                            }
                        }

                    }

                    string sInfo = "INFO: " + string.Join(",\r\n ", info);
                    ocrs.sMessage = sInfo;
                    ocrs.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                    ocrs.iLogLevel = (int)EnumXIErrorPriority.Critical;
                    SaveErrortoDB(ocrs);
                }
                #endregion
                if (!string.IsNullOrEmpty(sUnresolvedNotation))
                {
                    oCResult.sMessage = "ERROR: [Un-Merged Script:" + sUnresolvedNotation + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "]";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    SaveErrortoDB(oCResult);
                    sUnresolvedNotation = "";
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [ Merge Script:" + sNotation + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            SourceContent = SourceContent.Replace("|||", ",");
            oCResult.oResult = SourceContent;
            return oCResult;
        }
        private CResult XIIScriptResolving(XIBOInstance oStructureInstance, string exp)
        {
            CResult oCResult = new CResult();
            string sNotation = exp; string sValue = string.Empty;
            try
            {
                string sScriptString = exp.Substring(2, exp.Length - 4);
                string sSubNotation = SubStringNotation(sScriptString, "<|", "|>");
                if (!string.IsNullOrEmpty(sSubNotation))
                {
                    string sSubNotationValue = string.Empty;
                    var oResult = XIIScriptResolving(oStructureInstance, sSubNotation);
                    if (oResult.bOK && oResult.oResult != null)
                    {
                        sSubNotationValue = (string)oResult.oResult;
                        if (!string.IsNullOrEmpty(sSubNotationValue))
                        {
                            sScriptString = sScriptString.Replace(sSubNotation, sSubNotationValue);
                        }
                        else
                        {
                            sScriptString = sScriptString.Replace(sSubNotation, string.Empty);
                        }
                    }
                }
                //string sScriptString = exp.Replace("<|", "").Replace("|>", "");
                if (sScriptString.StartsWith("<xi.s"))
                {
                    if (string.IsNullOrEmpty(sGUID))
                    {
                        sGUID = Guid.NewGuid().ToString();
                    }
                    CNV oParam = new CNV();
                    List<CNV> oNVList = new List<CNV>();
                    int iLastStringIndex = sScriptString.LastIndexOf(">");
                    int iStartStringIndex = sScriptString.IndexOf("<");
                    string sScript = sScriptString.Substring(iStartStringIndex, iLastStringIndex);
                    sScript = sScript.Remove(iStartStringIndex, 1);
                    if (sScriptString.Contains("}>{"))
                    {
                        int iScriptColumnLastIndex = sScriptString.LastIndexOf(">");
                        string sScriptColumn = sScriptString.Substring(iScriptColumnLastIndex).Replace(">", "");
                        sScriptColumn = sScriptColumn.Replace("{", "").Replace("}", "");
                        string sScriptBOName = sScriptColumn.Split('.')[0];
                        sScriptColumn = sScriptColumn.Split('.')[1];
                        string sInstnaceID = string.Empty;
                        sInstnaceID = oStructureInstance.oSubStructureI(sScriptBOName).Item(0).AttributeI(sScriptColumn).sResolvedValue;
                        oParam.sName = "sScript";
                        oParam.sValue = sScript;
                        oNVList.Add(oParam);
                        oParam = new CNV();
                        oParam.sName = "-" + sScriptColumn;
                        oParam.sValue = sInstnaceID;
                        oNVList.Add(oParam);
                        oCache.SetXIParams(oNVList, sGUID, sSessionID);
                    }
                    CResult oResult = new CResult();
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sScript.ToString();
                    oResult = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oResult.bOK && oResult.oResult != null)
                    {
                        sValue = (string)oResult.oResult;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [ Merge XIIScript:" + sNotation + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            oCResult.oResult = sValue;
            return oCResult;
        }
        private List<string> GetNotationsWithinDelimeter(string sSource, Regex regexObj)
        {
            /*GET DISTINCT NOTATIONS*/
            List<string> dictresult = new List<string>();
            if (!string.IsNullOrEmpty(sSource))
            {
                dictresult = regexObj.Matches(sSource).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().ToList();
                //dictresult = matches.ToDictionary(d => d);
            }
            return dictresult;
        }

        public string CheckCSVNotions(string sContent, string sStartString, string sEndString)
        {
            try
            {
                string sFinalString = "";
                if (sContent.Contains(sEndString))
                {
                    int iCollectionIndex = sContent.IndexOf(sEndString);
                    int iStartPosi = sContent.LastIndexOf(sStartString, iCollectionIndex);
                    int iStringLength = sEndString.Length;
                    sFinalString = sContent.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckSubNotation(string sContent)
        {
            try
            {
                string sFinalString = "";
                if (sContent.Contains(")]"))
                {
                    int iCollectionIndex = sContent.IndexOf(")]");
                    int iStartPosi = sContent.LastIndexOf("[(", iCollectionIndex);
                    int iStringLength = ")]".Length;
                    sFinalString = sContent.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }

                return sFinalString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string CheckStructureNotation(string HtmlContentTable, int iStartingIndex)
        {
            try
            {
                //here we get structure notation string
                string sHtmlTableRow = "";
                int iIndex = HtmlContentTable.IndexOf("]]", iStartingIndex);
                if (iIndex > 0)
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf("]]");
                    int iStartPosi = HtmlContentTable.LastIndexOf("[[", iCollectionIndex);
                    int iStringLength = "]]".Length;
                    sHtmlTableRow = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sHtmlTableRow;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //Common.SaveErrorLog(ex.ToString(), "");
                return null;
            }
        }
        //public object GetStructureContent(int iUserID, string sOrgName, string sDatabase, string sFinalString, int iTemplateID, int iInstanceID)
        //{
        //    try
        //    {
        //        XIIQS oQsInstance = new XIIQS(); XIIBO oBOI = new XIIBO();
        //        string sInstanceName = "";
        //        string sStructureName = sFinalString.Replace("[[", "");
        //        sStructureName = sStructureName.Replace("]]", "");
        //        int iOrgID = SessionManager.OrganizationID; string sOrgDB = SessionManager.OrgDatabase;
        //        List<CNV> nvPairsList = new List<CNV>();
        //        CNV nvPairs = new CNV();
        //        nvPairs.sName = "sCoreDatabaseName";
        //        nvPairs.sValue = sDatabase;
        //        nvPairsList.Add(nvPairs);
        //        nvPairs = new CNV();
        //        nvPairs.sName = "sOrgDatabase";
        //        nvPairs.sValue = sOrgDB;
        //        nvPairsList.Add(nvPairs);
        //        nvPairs = new CNV();
        //        nvPairs.sName = "iOrgID";
        //        nvPairs.sValue = Convert.ToString(iOrgID);
        //        oIXI.sCoreDatabase = sDatabase;
        //        oIXI.sOrgDatabase = sOrgDB;
        //        oIXI.iOrgID = iOrgID;
        //        //Check if QS notaion has another notation or not
        //        sInstanceName = CheckModelHtmlContentNoation(sStructureName);
        //        oIXI.sOrgDatabase = sOrgDB;
        //        oIXI.sCoreDatabase = sDatabase;
        //        oIXI.iOrgID = iOrgID;
        //        QueryEngine oQE = new QueryEngine();
        //        List<XIWhereParams> oWParams = new List<XIWhereParams>();
        //        XIWhereParams oWP = new XIWhereParams();
        //        oWP.sField = "DocumentTemplateID";
        //        oWP.sOperator = "=";
        //        oWP.sValue = Convert.ToString(iTemplateID);
        //        oWParams.Add(oWP);
        //        oQE.AddBO("DocumentGeneration", "", oWParams);
        //        CResult oCResult = oQE.BuildQuery();
        //        if (oCResult.bOK && oCResult.oResult != null)
        //        {
        //            var sSql = (string)oCResult.oResult;
        //            ExecutionEngine oEE = new ExecutionEngine();
        //            oEE.XIDataSource = oQE.XIDataSource;
        //            oEE.sSQL = sSql;
        //            var oQResult = oEE.Execute();
        //            if (oQResult.bOK && oQResult.oResult != null)
        //            {
        //                oBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.FirstOrDefault();
        //                //oBOI = oIXI.BOI("DocumentGeneration", Convert.ToString(3));

        //                List<XIIBO> oBOIList = new List<XIIBO>();
        //                //var oBoi = oIXI.BOI("TemplateInstances", Convert.ToString(1));
        //                oQE = new QueryEngine();
        //                oWParams = new List<XIWhereParams>();
        //                oWP = new XIWhereParams();
        //                oWP.sField = "FKiDocumentID";
        //                oWP.sOperator = "=";
        //                oWP.sValue = oBOI.Attributes.Where(x => x.Key.ToLower() == "ID".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
        //                oWParams.Add(oWP);
        //                oQE.AddBO("TemplateInstances", "", oWParams);
        //                oCResult = oQE.BuildQuery();
        //                if (oCResult.bOK && oCResult.oResult != null)
        //                {
        //                    sSql = (string)oCResult.oResult;
        //                    oEE = new ExecutionEngine();
        //                    oEE.XIDataSource = oQE.XIDataSource;
        //                    oEE.sSQL = sSql;
        //                    oQResult = oEE.Execute();
        //                    if (oQResult.bOK && oQResult.oResult != null)
        //                    {
        //                        oBOIList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
        //                        while (sInstanceName.Length > 0)
        //                        {
        //                            string sName = sInstanceName.Replace("{{", "").Replace("}}", "");
        //                            if (oBOIList != null)
        //                            {
        //                                foreach (var item in oBOIList)
        //                                {
        //                                    string sValue = item.Attributes.Where(x => x.Value.sValue.ToLower() == sName.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
        //                                    if (sValue != null)
        //                                    {
        //                                        sValue = item.Attributes.Where(x => x.Key == "TemplateParameterValue").Select(x => x.Value.sValue).FirstOrDefault();
        //                                    }
        //                                    if (sValue != null)
        //                                    {
        //                                        sStructureName = sStructureName.Replace(sInstanceName, sValue);
        //                                    }

        //                                }
        //                            }
        //                            //Check if QS notaion has another notation or not
        //                            sInstanceName = CheckModelHtmlContentNoation(sStructureName);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        nvPairsList.Add(nvPairs);
        //        var oResult = oIScript.EvalueNotation(sStructureName, nvPairsList);
        //        return oResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.Error(ex);
        //        //Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return null;
        //    }
        //}

        public string ReplaceHtmlContentWithData(string sHtmlContent, string sReplaceString, List<XIIBO> oResult)
        {
            try
            {
                //here we get html content string with data
                int i = 0;
                string sFinalHtmlContent = "<table style='width:100%;' cellpadding='4' cellspacing='0' border='1'>";
                foreach (var item in oResult)
                {
                    if (i == 0)
                    {
                        sFinalHtmlContent += "<tr>";
                        // sFinalHtmlContent += "<thead>";
                        foreach (var attribute in item.Attributes)
                        {

                            sFinalHtmlContent += "<th style='background-color:#e7e7e7;'>" + attribute.Key + "</th>";
                        }
                        sFinalHtmlContent += "</tr>";
                    }

                    sFinalHtmlContent += "<tr>";
                    foreach (var attribute in item.Attributes)
                    {
                        if (string.IsNullOrEmpty(attribute.Value.sValue))
                        {
                            sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + "-" + "</td>";
                        }
                        else
                        {
                            if (attribute.Key == "DOB")
                            {
                                sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + Convert.ToDateTime(attribute.Value.sValue).Date.ToString(XIConstant.Date_Format) + "</td>";
                            }
                            else
                            {
                                sFinalHtmlContent += "<td style='background-color:#c0c0c0;'>" + attribute.Value.sValue + "</td>";
                            }
                        }
                    }
                    sFinalHtmlContent += "</tr>";
                    i++;
                }
                sFinalHtmlContent += "</table>";
                sHtmlContent = sHtmlContent.Replace(sReplaceString, sFinalHtmlContent);
                return sHtmlContent;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public string CheckModelHtmlContentNoation(string HtmlContentTable)
        {
            try
            {

                //we get model properties
                string sFinalString = "";
                if (HtmlContentTable.Contains("}}"))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf("}}");
                    int iStartPosi = HtmlContentTable.LastIndexOf("{{", iCollectionIndex);
                    int iStringLength = "}}".Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public string CheckCollectionNoation(string HtmlContentTable)
        {
            try
            {

                //we get model properties
                string sFinalString = "";
                if (HtmlContentTable.Contains("}]"))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf("}]");
                    int iStartPosi = HtmlContentTable.LastIndexOf("[{", iCollectionIndex);
                    int iStringLength = "}]".Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public string CheckHtmlfunctionsNoation(string HtmlContentTable)
        {
            try
            {

                //here we get function notations
                string sFinalString = "";
                if (HtmlContentTable.Contains("[["))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf("]]");
                    int iStartPosi = HtmlContentTable.LastIndexOf("[[", iCollectionIndex);
                    int iStringLength = "]]".Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public string SubStringNotation(string HtmlContentTable, string sStartFormat, string sEndFormat)
        {
            try
            {
                string sFinalString = "";
                if (HtmlContentTable.Contains(sEndFormat))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf(sEndFormat);
                    int iStartPosi = HtmlContentTable.LastIndexOf(sStartFormat, iCollectionIndex);
                    int iStringLength = sEndFormat.Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        string sChildScript = string.Empty;
        public List<string> SubStringNotationList(string HtmlContentTable, string sStartFormat, string sEndFormat, List<string> sMatchesList)
        {
            try
            {
                string sFinalString = "";
                if (HtmlContentTable.Contains(sEndFormat))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf(sEndFormat);
                    int iStartPosi = HtmlContentTable.LastIndexOf(sStartFormat, iCollectionIndex);
                    int iStringLength = sEndFormat.Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                    if (!string.IsNullOrEmpty(sFinalString))
                    {
                        var sRemovestring = string.Empty;
                        if (sFinalString.Contains("NULL"))
                        {
                            var sReplacestring = sFinalString.Replace("NULL", sChildScript);
                            if (sMatchesList.Contains(sChildScript))
                            {
                                sMatchesList.Remove(sChildScript);
                            }
                            sMatchesList.Add(sReplacestring);
                        }
                        else
                        {
                            sMatchesList.Add(sFinalString);
                        }
                        sChildScript = sFinalString;
                        HtmlContentTable = HtmlContentTable.Replace(sFinalString, "NULL");
                        var sMatchString = SubStringNotationList(HtmlContentTable, "<|", "|>", sMatchesList);
                    }

                }
                return sMatchesList;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        //public string ReplaceHtmlWithDynamicModelData(XIContentEditors model, string sFinalString, int iInstanceID)
        //{
        //    try
        //    {
        //        XIInfraCache oCache = new XIInfraCache(); XIEncryption oEncrypt = new XIEncryption();
        //        int iOrgID = SessionManager.OrganizationID;
        //        string sOrgDB = SessionManager.OrgDatabase;
        //        string sReturnValue = "";
        //        string sReturnType = ""; List<string[]> Rows = new List<string[]>();
        //        string sModelParamName = sFinalString.Replace("{{", "").Replace("}}", "");
        //        var oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", model.BO.ToString());
        //        string sBOTableName = oBODef.TableName;
        //        string sPrimaryKeyColumn = oBODef.sPrimaryKey;
        //        var XIDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBODef.iDataSource.ToString());
        //        string sConnectionString = oEncrypt.DecryptData(XIDataSource.sConnectionString, true, XIDataSource.ID.ToString());
        //        // sBODataSource = oXiAPI.GetBODataSource(oBODef.iDataSource, iOrgID, sDatabase, sOrgDB);
        //        SqlConnection Con = new SqlConnection(sConnectionString);
        //        using (SqlCommand cmd = new SqlCommand("", Con))
        //        {
        //            cmd.CommandText = "select CONVERT(varchar," + sModelParamName + ") from " + sBOTableName + " where " + sPrimaryKeyColumn + "=" + iInstanceID + "";
        //            Con.Open();
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            if (reader.Read())
        //            {
        //                sReturnValue = reader.IsDBNull(0) ? "" : reader.GetString(0);
        //                //sReturnValue = reader.GetString(0);
        //                if (sModelParamName.ToLower() == "sPasswordHash".ToLower())
        //                {
        //                    sReturnValue = oEncrypt.DecryptData(sReturnValue, true, iUserID.ToString());
        //                }

        //                // sReturnType = reader.GetString(1);
        //            }
        //            //if (sReturnType == "datetime")
        //            //{
        //            //    DateTime dtvalue = Convert.ToDateTime(sReturnValue);
        //            //    sReturnValue = dtvalue.ToString("MM/dd/yyyy");
        //            //}
        //            //if(sReturnType=="int")
        //            //{

        //            //}
        //            Con.Dispose();
        //        }

        //        if (sReturnValue.Length > 0)
        //        {
        //            model.Content = model.Content.Replace(sFinalString, sReturnValue);
        //        }
        //        else
        //        {
        //            model.Content = model.Content.Replace(sFinalString, sModelParamName);
        //        }
        //        return model.Content;

        //    }
        //    catch (Exception ex)
        //    {
        //        //logger.Error(ex);
        //        //Common.SaveErrorLog(ex.ToString(), "");
        //        return null;
        //    }
        //}

        public CResult Build_FullNotations(string[] Notations, string sParentBO)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Convert new notations to old format of structure notations";//expalin about this method logic            

            //Task goal is to convert below notations which starts with '#'
            //{{|sPolicyNumber}} --> {{oSubStructureI("Policy").Item(0).AttributeI("sPolicyNumber").sValue}}

            //{{|Driver.sName}} --> {{oSubStructureI("Driver").Item(0).AttributeI("sName").sValue}}

            //{{|Driver.sName:sDerivedValue}} --> {{oSubStructureI("Driver").Item(0).AttributeI("sName").sDerivedValue}}

            //{{|Driver[1].sName}} --> {{oSubStructureI("Driver").Item(1).AttributeI("sName").sValue}}

            //{{|Driver[2].Claim[0].ID}} --> {{oSubStructureI("Driver").Item(0).ChildI("Claim","0").AttributeI(\"ID\").sValue}}

            //Examples
            //Notations[0] = "|sPolicyNumber";
            //Notations[1] = "|Driver.sName";
            //Notations[2] = "|Driver.sName:sDerivedValue";
            //Notations[3] = "|Driver[1].sName";
            //Notations[4] = "|Driver[2].Claim[0].ID";
            //Notations[5] = "|Driver[2].Claim[0].Conviction[0].ID";
            string OldNotation = string.Empty;
            try
            {
                if (Notations != null && Notations.Count() > 0 && !string.IsNullOrEmpty(sParentBO))//check mandatory params are passed or not
                {
                    Dictionary<string, string> ResolvedNotations = new Dictionary<string, string>();
                    foreach (var nota in Notations)
                    {
                        if (nota.StartsWith("|"))
                        {
                            var notation = nota.Substring(1, nota.Length - 1);
                            OldNotation = notation;
                            var NewNotation = string.Empty;
                            var sBO = string.Empty;
                            if (notation.Contains('.'))
                            {
                                NewNotation = "oSubStructureI(";
                                var Splits = notation.Split('.').ToList();
                                if (Splits.Count() > 1)
                                {
                                    foreach (var items in Splits)
                                    {
                                        var Last = Splits.Last();
                                        string sChildBO = string.Empty;
                                        if (items == Last)
                                        {
                                            if (items.Contains(':'))
                                            {
                                                var Splits2 = items.Split(':').ToList();
                                                NewNotation = NewNotation + "AttributeI(\"" + Splits2[0] + "\")." + Splits2[1];
                                            }
                                            else
                                            {
                                                NewNotation = NewNotation + "AttributeI(\"" + items + "\").sValue";
                                            }
                                        }
                                        else if (items.Contains('['))
                                        {
                                            if (string.IsNullOrEmpty(sBO))
                                            {
                                                sBO = items.Substring(0, items.IndexOf("["));
                                                Regex regex = new Regex(@"\[(.*?)\]", RegexOptions.IgnoreCase);
                                                MatchCollection matches = regex.Matches(items);
                                                foreach (var match in matches)
                                                {
                                                    NewNotation = NewNotation + "\"" + sBO + "\").Item(" + match.ToString().Replace("[", "").Replace("]", "") + ").";
                                                }
                                            }
                                            else
                                            {
                                                sChildBO = items.Substring(0, items.IndexOf("["));
                                                Regex regex = new Regex(@"\[(.*?)\]", RegexOptions.IgnoreCase);
                                                MatchCollection matches = regex.Matches(items);
                                                foreach (var match in matches)
                                                {
                                                    NewNotation = NewNotation + "ChildI(\"" + sChildBO + "\", \"" + match.ToString().Replace("[", "").Replace("]", "") + "\").";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            NewNotation = NewNotation + "\"" + items + "\").Item(0).";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (notation.Contains(':'))
                                {
                                    var Splits = notation.Split(':').ToList();
                                    NewNotation = "oSubStructureI(\"" + sParentBO + "\").Item(0).AttributeI(\"" + Splits[0] + "\")." + Splits[1];
                                }
                                else
                                {
                                    NewNotation = "oSubStructureI(\"" + sParentBO + "\").Item(0).AttributeI(\"" + notation + "\").sResolvedValue";
                                }
                            }
                            ResolvedNotations[NewNotation] = nota;
                        }
                        else
                        {
                            ResolvedNotations[nota] = nota;
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = ResolvedNotations;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: Notations or sParentBO missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = "Error while building the structure notation for " + OldNotation;
                oCResult.sCategory = "Structure notation failure";
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

    }

}
