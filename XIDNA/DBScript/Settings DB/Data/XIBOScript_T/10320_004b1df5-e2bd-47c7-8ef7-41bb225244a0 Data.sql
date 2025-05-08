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
 [FKiVersionIDXIGUID]) Values(N'10320',
 N'0',
 N'0',
 NULL,
 N'TIG Renewal Quote',
 N'Renewal Quote',
 N' public static CResult ProduceRenewalRequest(List<CNV> lParam)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            ConvertQuotes oConQuotes = new ConvertQuotes();
            //XIIQS oQsInstance = new XIIQS();         
            CResult oResult = new CResult();
            oResult.sMessage = "API Produce renewal";
            oIB.SaveErrortoDB(oResult);
            CNV oNV = new CNV();
            oNV.sName = "sCode";
            try
            {
                string sUID = lParam.Where(m => m.sName == "sUID").FirstOrDefault().sValue;
                int iInsatnceID = Convert.ToInt32(lParam.Where(m => m.sName == "iInsatnceID").FirstOrDefault().sValue);
                int iUserID = Convert.ToInt32(lParam.Where(m => m.sName == "iUserID").FirstOrDefault().sValue);
                string sDataBase = lParam.Where(m => m.sName == "sDataBase").FirstOrDefault().sValue;
                int iProductID = Convert.ToInt32(lParam.Where(m => m.sName == "iProductID").FirstOrDefault().sValue);
                int iQuoteID = Convert.ToInt32(lParam.Where(m => m.sName == "iQuoteID").FirstOrDefault().sValue);
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
            public CResult SendRequest(string sGUID, int iInstanceID, int iUserID, int iCustomerID, string sDataBase, string sProductName, string sVersion, string sProductCode, int iQuoteID, string sSessionID = null)
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
                    string APIKey = string.Empty, APIPWD = string.Empty, PolicyRef = string.Empty;
                    List<XIIBO> APIValues = new List<XIIBO>();
                    XIContentEditors oXIContent = new XIContentEditors();
                    string sTypeofCover = string.Empty;
                    string ConvertQuoteResponse = "select id,sConvertQuoteResponse from APIQuotes_T where FKQSInstanceID=" + iInstanceID.ToString() + " order by id desc";
                    string sReturnValue = string.Empty;
                    XID1Click oXI1Click = new XID1Click();
                    oXI1Click.Query = ConvertQuoteResponse;
                    oXI1Click.Name = "APIQuotes_T";
                    var QueryResult = oXI1Click.Execute_Query();
                    XDocument doc1 = new XDocument();
                    XNamespace ns = "";
                    if (QueryResult.Rows.Count > 0)
                    {
                        doc1 = XDocument.Parse(QueryResult.Rows[0][1].ToString());
                        ns = "https://cfsnetwork.co.uk";
                        PolicyRef = doc1.Descendants(XNamespace.Get(ns.ToString()) + "PolicyRef").First().Value.ToString();
                    }
                    var oProduceRenewal = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Produce Renewal", "0");
                    var oProduceRenewalC = oProduceRenewal.FirstOrDefault().GetCopy();
                    if (oProduceRenewalC != null)
                    {
                        sTypeofCover = oProduceRenewalC.Content.ToString();
                        sTypeofCover = sTypeofCover.Replace("{PolicyRef}", PolicyRef);
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
                        var oQResult = oQE.Execute_QueryEngine("XIAPI_Adapter_T", "*", sWhereCondition);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            APIValues = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        }
                        string url = APIValues.Where(x => x.AttributeI("sName").sValue.ToLower() == "ProduceRenewalURL".ToLower()).Select(t => t.AttributeI("sValue").sValue).FirstOrDefault();
                        HttpWebRequest rqst = (HttpWebRequest)HttpWebRequest.Create(url);
                        rqst.Timeout = -1;
                        rqst = BuildWebRequest(APIValues, APIKey, APIPWD, "ProduceRenewal", sGUID);
                        byte[] byteData = Encoding.UTF8.GetBytes(sTypeofCover);
                        rqst.ContentLength = byteData.Length;
                        using (Stream postStream = rqst.GetRequestStream())
                        {
                            postStream.Write(byteData, 0, byteData.Length);
                            postStream.Close();
                        }
                        StreamReader rsps = new StreamReader(rqst.GetResponse().GetResponseStream());
                        oProduceRenewalC.Content = rsps.ReadToEnd();
                        oboi = oIXI.BOI("APIQuotes_T", QueryResult.Rows[0][1].ToString(), "*");

                        oboi.SetAttribute("sProduceRenewalRequest", sTypeofCover);
                        oboi.SetAttribute("sProduceRenewalResponse", oProduceRenewalC.Content);
                        var Result = oboi.Save(oboi);
                    }
                    string Link = string.Empty, FileName = string.Empty;
                    MemoryStream _ms = new MemoryStream();
                    if (oProduceRenewalC.Content != null)
                    {
                        doc1 = XDocument.Parse(oProduceRenewalC.Content);
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
                                oInfraDoccomponent.SavetoEDITransaction(iProductID, iBOID, oProduceRenewalC, str, iInstanceID.ToString(), oCNVParams);
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
 N'2020-07-01 10:09:30.000',
 N'183',
 N'169.254.204.77',
 N'183',
 N'169.254.204.77',
 N'2020-07-01 10:09:30.000',
 N'15',
 N'PolicyMainCal',
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
 N'004b1df5-e2bd-47c7-8ef7-41bb225244a0',
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
