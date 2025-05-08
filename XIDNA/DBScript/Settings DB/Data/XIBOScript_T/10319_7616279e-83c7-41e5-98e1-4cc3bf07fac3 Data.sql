INSERT INTO [XIBOScript_T] ([ID],
 [FKiBOID],
 [FKiBOAttributeID],
 [sExecutionType],
 [sName],
 [sDescription],
 [sScript],
 [sType],
 [sLanguage],
 [StatusTypeID],
 [CreatedTime],
 [CreatedBy],
 [CreatedBySYSID],
 [UpdatedBy],
 [UpdatedBySYSID],
 [UpdatedTime],
 [FKiApplicationID],
 [sMethodName],
 [sVersion],
 [iOrder],
 [OrganisationID],
 [izXDeleted],
 [zXCrtdBy],
 [zXCrtdWhn],
 [zXUpdtdBy],
 [zXUpdtdWhn],
 [sLevel],
 [sCategory],
 [sClassification],
 [XIGUID],
 [XIDeleted],
 [XICreatedBy],
 [XIUpdatedBy],
 [XICreatedWhen],
 [XIUpdatedWhen],
 [sTag],
 [FKiTagSpaceID],
 [FKiVersionID],
 [FKiBOIDXIGUID],
 [FKiBOAttributeIDXIGUID],
 [FKiApplicationIDXIGUID],
 [FKiTagSpaceIDXIGUID],
 [FKiVersionIDXIGUID]) Values(N'10319',
 N'0',
 N'0',
 NULL,
 N'TIG Convert Quote Request',
 N'Convert Quotes',
 N' public static CResult ConvertQuoteRequest(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            ConvertQuotes oConQuotes = new ConvertQuotes();
            //XIIQS oQsInstance = new XIIQS();         
            CResult oResult = new CResult();
            oResult.sMessage = "API Convert quotes";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
                int iProductID = Convert.ToInt32(lParam.Where(m => m.sName == "Version").FirstOrDefault().sValue);
                oResult = oConQuotes.SendRequest(sUID, iInsatnceID, iUserID, iProductID);
                oNV.sValue = "00";
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.oResult = "Error";
                oNV.sValue = "100";
                oIB.SaveErrortoDB(oResult);
            }
            oResult.oCollectionResult.Add(oNV);
            return oResult;
        }
        public class ConvertQuotes
        {
            public CResult BuildCResultObject(string Value, string Name, string Status)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oresult = new CResult();
                try
                {
                    CNV oNV = new CNV();
                    oNV.sName = "sMessage";
                    oNV.sValue = Status;
                    CNV oNV1 = new CNV();
                    oNV1.sName = "oResult";
                    oNV1.sValue = Value;
                    CNV oNV2 = new CNV();
                    oNV2.sName = "LoadFactorName";
                    oNV2.sValue = Name;
                    oresult.oCollectionResult.Add(oNV);
                    oresult.oCollectionResult.Add(oNV1);
                    oresult.oCollectionResult.Add(oNV2);
                }
                catch (Exception ex)
                {
                    oresult.sMessage = "ERROR: [" + oresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oresult.LogToFile();
                    oIB.SaveErrortoDB(oresult);
                }
                return oresult;
            }
            public HttpWebRequest BuildWebRequest(List<XIIBO> APIValues, string APIKey, string APIPWD, string sAPICall, string sGUID)
            {
                string url = APIValues.Where(x => x.AttributeI("sAPICall").sValue.ToLower() == sAPICall.ToLower() && x.AttributeI("sKey").sValue.ToLower() == "url").Select(t => t.AttributeI("sValue").sValue).FirstOrDefault(); //ProduceQuote
                HttpWebRequest oRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                oRequest.Timeout = -1;
                oRequest.Method = APIValues.Where(x => x.AttributeI("sName").sValue.ToLower() == "Method".ToLower()).Select(t => t.AttributeI("sValue").sValue).FirstOrDefault();
                oRequest.ContentType = APIValues.Where(x => x.AttributeI("sName").sValue.ToLower() == "ContentType".ToLower()).Select(t => t.AttributeI("sValue").sValue).FirstOrDefault();
                oRequest.UserAgent = APIValues.Where(x => x.AttributeI("sName").sValue.ToLower() == "UserAgent".ToLower()).Select(t => t.AttributeI("sValue").sValue).FirstOrDefault();

                foreach (var item in APIValues.Where(x => x.AttributeI("bISHeader").sValue.ToLower() == "true" && (x.AttributeI("sAPICall").sValue.ToLower() == sAPICall.ToLower() || x.AttributeI("sAPICall").sValue == "")).ToList())
                {
                    if (item.AttributeI("sKey").sValue.ToLower() == "authorization")
                    {
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(APIKey + ":" + APIPWD);
                        string val = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(APIKey + ":" + APIPWD));
                        oRequest.Headers.Add(item.AttributeI("sKey").sValue, item.AttributeI("sValue").sValue + val);
                    }
                    else if (item.AttributeI("sKey").sValue.ToLower() == "cookie")
                    {
                        oRequest.Headers.Add(item.AttributeI("sKey").sValue, item.AttributeI("sValue").sValue + sGUID);
                    }
                    else
                    {
                        oRequest.Headers.Add(item.AttributeI("sKey").sValue, item.AttributeI("sValue").sValue);
                    }
                }
                return oRequest;
            }
            public CResult SendRequest(string sGUID, int iInstanceID, int iUserID, int iProductID)
            {
                List<string> Info = new List<string>();
                Info.Add("QsInstanceID_" + iInstanceID);
                XIInstanceBase oIB = new XIInstanceBase();
                CResult oCResult = new CResult();
                try
                {
                    List<CResult> oGeneralDeclines = new List<CResult>();
                    XIIXI oIXI = new XIIXI();
                    XIInfraCache oCache = new XIInfraCache();
                    var result = new CResult();
                    XIIBO oboi = new XIIBO();
                    XIDXI oxid = new XIDXI();
                    XIDBO obod = (XIDBO)oxid.Get_BODefinition("APIQuotes_T").oResult;
                    oboi.BOD = obod;
                    string APIKey = string.Empty, APIPWD = string.Empty, Reference = string.Empty;
                    List<XIIBO> APIValues = new List<XIIBO>();
                    XIContentEditors oXIContent = new XIContentEditors();
                    string sTypeofCover = string.Empty;
                    string ProduceQuoteResponse = "select id,sResponseObject from APIQuotes_T where FKQSInstanceID=" + iInstanceID.ToString() + " order by id desc";
                    string sReturnValue = string.Empty;
                    XID1Click oXI1Click = new XID1Click();
                    oXI1Click.Query = ProduceQuoteResponse;
                    oXI1Click.Name = "APIQuotes_T";
                    var QueryResult = oXI1Click.Execute_Query();
                    XDocument doc1 = new XDocument();
                    XNamespace ns = "";
                    if (QueryResult.Rows.Count > 0)
                    {
                        doc1 = XDocument.Parse(QueryResult.Rows[0][1].ToString());
                        ns = "https://cfsnetwork.co.uk";
                        Reference = doc1.Descendants(XNamespace.Get(ns.ToString()) + "Reference").First().Value.ToString();
                    }
                    var oConvertQuotes = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Convert Quotes", "0");
                    var oConvertQuotesC = oConvertQuotes.FirstOrDefault().GetCopy();
                    if (oConvertQuotesC != null)
                    {
                        sTypeofCover = oConvertQuotesC.Content.ToString();
                        sTypeofCover = sTypeofCover.Replace("{Reference}", Reference);
                        oCResult.oResult = sTypeofCover;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(sTypeofCover);
                        string sFunction = "xi.s|{xi.a|''XIConfig_T'',''APIKey'',''sValue'','''',''sName''}";
                        CResult oCR = new CResult();
                        XIDScript oXIScript = new XIDScript();
                        oXIScript.sScript = sFunction.ToString();
                        oCR = oXIScript.Execute_Script("", "");
                        APIKey = oCR.oResult.ToString() == null ? "" : oCR.oResult.ToString();
                        sFunction = "xi.s|{xi.a|''XIConfig_T'',''APIPWD'',''sValue'','''',''sName''}";
                        oCR = new CResult();
                        oXIScript = new XIDScript();
                        oXIScript.sScript = sFunction.ToString();
                        oCR = oXIScript.Execute_Script("", "");
                        APIPWD = oCR.oResult.ToString() == null ? "" : oCR.oResult.ToString();
                        QueryEngine oQE = new QueryEngine();
                        string sWhereCondition = "FKiProductID=" + iProductID;
                        var oQResult = oQE.Execute_QueryEngine("API Adapter", "*", sWhereCondition);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            APIValues = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        }

                        HttpWebRequest rqst = BuildWebRequest(APIValues, APIKey, APIPWD, "ConvertQuote", sGUID);
                        byte[] byteData = Encoding.UTF8.GetBytes(sTypeofCover);
                        rqst.ContentLength = byteData.Length;
                        using (Stream postStream = rqst.GetRequestStream())
                        {
                            postStream.Write(byteData, 0, byteData.Length);
                            postStream.Close();
                        }
                        StreamReader rsps = new StreamReader(rqst.GetResponse().GetResponseStream());
                        oConvertQuotesC.Content = rsps.ReadToEnd();
                        oCResult.sMessage = " API" + oConvertQuotesC.Content;
                        oIB.SaveErrortoDB(oCResult);
                        //using (StreamReader rsps = new StreamReader(rqst.GetResponse().GetResponseStream()))
                        //{
                        //    oConvertQuotesC.Content = rsps.ReadToEnd();
                        //    rsps.Close();
                        //}

                        oboi = oIXI.BOI("APIQuotes_T", QueryResult.Rows[0][0].ToString(), "*");
                        oboi.BOD = obod;
                        oboi.SetAttribute("ID", QueryResult.Rows[0][0].ToString());

                        oboi.SetAttribute("sConvertQuoteRequest", sTypeofCover);
                        oboi.SetAttribute("sConvertQuoteResponse", oConvertQuotesC.Content);
                        var Result = oboi.Save(oboi);
                    }
                    string Link = string.Empty, FileName = string.Empty;
                    MemoryStream _ms = new MemoryStream();
                    if (oConvertQuotesC.Content != null)
                    {
                        doc1 = XDocument.Parse(oConvertQuotesC.Content);
                        ns = "https://cfsnetwork.co.uk";
                        IEnumerable<XElement> responses = doc1.Descendants(ns + "Document").ToList();
                        foreach (XElement response in responses)
                        {
                            Link = response.Descendants(XNamespace.Get(ns.ToString()) + "Link").First().Value.ToString();
                            FileName = response.Descendants(XNamespace.Get(ns.ToString()) + "Description").First().Value.ToString();

                            var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];

                            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~\\" + sVirtualDir + "\\");
                            var index = Link.LastIndexOf(''/'') + 1;
                            FileName = Link.Contains("?") ? FileName + ".pdf" : Link.Substring(index, Link.Length - index);
                            var count = FileName.Count(x => x == ''.'');
                            if (count > 1)
                            {
                                int lastIndex = FileName.LastIndexOf(''.'');
                                var items = FileName.Substring(0, lastIndex);
                                items = items.Replace(''.'', ''^'');
                                FileName = items + FileName.Substring(lastIndex, FileName.Length - lastIndex);
                            }
                            string sfileName = physicalPath + FileName;
                            if (!Link.EndsWith("/"))
                            {
                                WebClient myWebClient = new WebClient();
                                myWebClient.DownloadFile(Link, sfileName);
                                FileStream fs = new FileStream(sfileName, FileMode.Open, FileAccess.Read);
                                byte[] tmpBytes = new byte[fs.Length];
                                fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                                MemoryStream mystream = new MemoryStream(tmpBytes);
                                StreamReader reader = new StreamReader(mystream);
                                XIInfraDocs oInfraDoc = new XIInfraDocs();
                                oInfraDoc.FKiQSInstanceID = iInstanceID;
                                oInfraDoc.FKiUserID = iUserID;
                                oInfraDoc.sOrgName = "Org";//sOrgName;
                                oInfraDoc.iOrgID = 5;// iOrgID;
                                oInfraDoc.iInstanceID = Convert.ToInt32(iInstanceID);
                                oInfraDoc.iType = 10;// iType;
                                oInfraDoc.FKiPolicyVersionID = 10;// iPolicyVersioID;

                                int iBOID = 0; List<CNV> oCNVParams = new List<CNV>();
                                oInfraDoc.SaveDocuments(mystream, FileName);
                                XIInfraDocumentComponent oInfraDoccomponent = new XIInfraDocumentComponent();
                                //stream.Position = 0;
                                string str = "";
                                oInfraDoccomponent.SavetoEDITransaction(iProductID, iBOID, oConvertQuotesC, str, iInstanceID.ToString(), oCNVParams);
                                fs.Close();
                                System.IO.File.Delete(sfileName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                    oCResult.sMessage = sInfo;
                    oIB.SaveErrortoDB(oCResult);
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.oResult = "Error";
                    oIB.SaveErrortoDB(oCResult);
                }
                return oCResult;
            }
        }',
 N'Prepersist',
 N'C# Code',
 N'0',
 N'2020-09-15 12:31:06.000',
 N'183',
 N'169.254.204.77',
 N'183',
 N'169.254.204.77',
 N'2020-09-15 12:31:06.000',
 N'15',
 N'ConvertQuoteRequest',
 NULL,
 NULL,
 N'5',
 N'0',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'7616279e-83c7-41e5-98e1-4cc3bf07fac3',
 N'0',
 N'0',
 N'0',
 N'2021-08-18 14:03:31.000',
 N'2021-08-18 14:03:31.000',
 NULL,
 NULL,
 NULL,
 NULL,
 NULL,
 N'64f97dfc-8306-4e18-868e-92f58bd9ca32',
 NULL,
 NULL)
