using Microsoft.AspNet.SignalR;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XICore;
using XIDataBase.Hubs;
using XIDNA.Common;
using XIDNA.Repository;
using XISystem;

namespace XIDNA
{
    public class Constants
    {
        public const string Admin = "Admin";
        public const int SetupAdminMenuID = 1;
    }

    public class SignalR : iSiganlR
    {
        public void HitSignalR(string InstanceID, int ProductversionID, string sRoleName, string sDatabase, string sGUID, string sSessionID, int iQuoteType)
        {
            //string sDatabase = "XICoreQA";
            CommonRepository Common = new CommonRepository();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var ConnectionID = oCache.Get_ParamVal(sSessionID, sGUID, "", "SignalRConnectionID");
                var Transtype = oCache.Get_ParamVal(sSessionID, sGUID, "", "-transtype");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, InstanceID.ToString());
                var oStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).FirstOrDefault();
                var IsStepLock = false;
                if (oStepD.iLockStage != 0 && oStepD.iLockStage <= oQSInstance.iStage)
                {
                    IsStepLock = true;
                }
                Common.SaveErrorLog("Log: SignalREvent Method started Execution" + InstanceID + " " + ProductversionID, sDatabase);
                // NOTE: EXECUTE THE BELOW QUERIES HERE
                // Select ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote AS Yearly from Aggregations_T Where FKiQSInstanceID = 51787 and FKiProductVersionID = 2 and XIDeleted = 0
                //SELECT rBestQuote FROM Lead_T where FKiQSInstanceID=51787
                XIIBO oXiBo = new XIIBO();
                XIIBO oQuoteI = new XIIBO();
                QueryEngine oQE = new QueryEngine();
                //string sWhereCondition = $"FKiQSInstanceID={InstanceID},FKiProductVersionID={ProductversionID},XIDeleted=0";
                string sWhereCondition = "FKiQSInstanceIDXIGUID=" + InstanceID + ",FKiProductVersionID=" + ProductversionID + ",XIDeleted=0, iType=" + iQuoteType;

                var oQResult = oQE.Execute_QueryEngine("Aggregations", "sGUID,ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote, bIsFlood, bIsApplyFlood", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oXiBo = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.OrderByDescending(f => f.AttributeI("ID").iValue).FirstOrDefault();
                }
                if (oXiBo == null)
                {
                    string sWhereQuote = "FKiQSInstanceIDXIGUID=" + InstanceID;
                    var oQuoteResult = oQE.Execute_QueryEngine("Aggregations", "ID,iQuoteStatus", sWhereQuote);
                    if (oQuoteResult.bOK && oQuoteResult.oResult != null)
                    {
                        oQuoteI = ((Dictionary<string, XIIBO>)oQuoteResult.oResult).Values.FirstOrDefault();
                    }
                }
                // Get the product id from ProductVersionID
                XIIBO oXiProduct = new XIIBO();
                XIIXI oIXI = new XIIXI();
                oXiProduct = oIXI.BOI("ProductVersion_T", ProductversionID.ToString(), "FKiProductID,bIsIndicativePrice");
                int iProductID = 0; bool bIsIndicativePrice = false;
                if (oXiProduct != null && oXiProduct.Attributes.ContainsKey("FKiProductID"))
                {
                    iProductID = oXiProduct.AttributeI("FKiProductID").iValue;
                    bIsIndicativePrice = oXiProduct.AttributeI("bIsIndicativePrice").bValue;
                }
                // QUERY FOR BEST QUOTE. 
                XIIBO OLeadBO = new XIIBO();

                string sWhereLead = "FKiQSInstanceIDXIGUID=" + InstanceID;

                var oLeadQResult = oQE.Execute_QueryEngine("Lead_T", "rBestQuote", sWhereLead);
                if (oLeadQResult.bOK && oLeadQResult.oResult != null)
                {
                    OLeadBO = ((Dictionary<string, XIIBO>)oLeadQResult.oResult).Values.FirstOrDefault();
                }
                // BUILD ONE STANDARD ANNONYMOUS OBJECT HERE
                if (oXiBo != null && OLeadBO != null)
                {
                    var oAnnonymous = new
                    {
                        IsLockStep = IsStepLock,
                        ProductversionID = ProductversionID,
                        QSInstanceID = InstanceID,
                        ProductID = iProductID,
                        iQuoteStatus = oXiBo.AttributeI("iQuoteStatus").sValue,
                        rCompulsoryExcess = oXiBo.AttributeI("rCompulsoryExcess").rValue,
                        rVoluntaryExcess = oXiBo.AttributeI("rVoluntaryExcess").rValue,
                        rTotalExcess = oXiBo.AttributeI("rTotalExcess").rValue,
                        rMonthlyPrice = oXiBo.AttributeI("rMonthlyPrice").rValue,
                        rMonthlyTotal = oXiBo.AttributeI("rMonthlyTotal").rValue,
                        zDefaultDeposit = oXiBo.AttributeI("zDefaultDeposit").rValue,
                        rFinalQuote = oXiBo.AttributeI("rFinalQuote").rValue,
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QuoteID = oXiBo.AttributeI("sGUID").sValue,
                        iQuoteID = oXiBo.AttributeI("ID").sValue,
                        RoleName = sRoleName,
                        bIsIndicativePrice = bIsIndicativePrice,
                        sQSType = oQSInstance.sQSType,
                        bIsFlood = oXiBo.AttributeI("bIsFlood").sValue,
                        bIsApplyFlood = oXiBo.AttributeI("bIsApplyFlood").sValue,
                        sTranstype = Transtype
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    //hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                    //string ConnectionID = "";
                    hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
                else if (OLeadBO != null && oQuoteI != null)
                {
                    var oAnnonymous = new
                    {
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QSInstanceID = InstanceID,
                        iQuoteStatus = oQuoteI.AttributeI("iQuoteStatus").sValue,
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed rBestQuote addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    //hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                    hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
                else if (oXiBo != null)
                {
                    var oAnnonymous = new
                    {
                        IsLockStep = IsStepLock,
                        ProductversionID = ProductversionID,
                        QSInstanceID = InstanceID,
                        ProductID = iProductID,
                        iQuoteStatus = oXiBo.AttributeI("iQuoteStatus").sValue,
                        rCompulsoryExcess = oXiBo.AttributeI("rCompulsoryExcess").rValue,
                        rVoluntaryExcess = oXiBo.AttributeI("rVoluntaryExcess").rValue,
                        rTotalExcess = oXiBo.AttributeI("rTotalExcess").rValue,
                        rMonthlyPrice = oXiBo.AttributeI("rMonthlyPrice").rValue,
                        rMonthlyTotal = oXiBo.AttributeI("rMonthlyTotal").rValue,
                        zDefaultDeposit = oXiBo.AttributeI("zDefaultDeposit").rValue,
                        rFinalQuote = oXiBo.AttributeI("rFinalQuote").rValue,
                        QuoteID = oXiBo.AttributeI("sGUID").sValue,
                        RoleName = sRoleName,
                        bIsIndicativePrice = bIsIndicativePrice,
                        sQSType = oQSInstance.sQSType,
                        bIsFlood = oXiBo.AttributeI("bIsFlood").sValue,
                        bIsApplyFlood = oXiBo.AttributeI("bIsApplyFlood").sValue,
                        sTranstype = Transtype
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                Common.SaveErrorLog("ErrorLog: SignalREvent" + ex.ToString(), sDatabase);
                throw ex;
            }
        }

        public void ShowSignalRMsg(string sMessage)
        {
            XIInfraCache oCache = new XIInfraCache();
            var connid = SessionManager.sSignalRCID;
            if (string.IsNullOrEmpty(connid))
            {
                connid = Guid.NewGuid().ToString();
            }
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            //hubContext.Clients.Client(connid).addNewMessageToPage(sMessage);
            hubContext.Clients.All.addNewMessageToPage(sMessage);
        }

        public void ShowSignalRUserMsg(string sMessage)
        {
            XIInfraCache oCache = new XIInfraCache();
            //var ConnectionID = oCache.Get_ParamVal(sSessionID, sGUID, "", "SignalRConnectionID");
            var connid = SessionManager.sSignalRCID;
            if (string.IsNullOrEmpty(connid))
            {
                connid = Guid.NewGuid().ToString();
            }
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            //hubContext.Clients.Client(connid).addNewMessageToPage(sMessage);
            //hubContext.Clients.All.addNewMessageToPage(sMessage);
            hubContext.Clients.All.addUserMessage(sMessage);
        }
        public void ListRefresh(dynamic sigRObj, string sOrgID)
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
            //context.Clients.All.ListRefresh(sigRObj);
            NotifyHub notifyHub = new NotifyHub();
            notifyHub.SendLeadUpdateSignal(sigRObj, sOrgID);
        }
        public void ListAddRefresh(dynamic sigRObj, string sOrgID)
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
            //context.Clients.All.ListAddRefresh(sigRObj);
            NotifyHub notifyHub = new NotifyHub();
            notifyHub.SendNewLeadSignal(sigRObj, sOrgID);
        }
    }

    public class APIInvoke
    {
        public CResult SetAppToSGMessage(List<CNV> oParams)
        {
            CResult oCR = new CResult();
            var UpdateResponse = API.PostListGetString(oParams, "api/SendGrid/SetAppToSGMessage");
            if (!string.IsNullOrEmpty(UpdateResponse) && UpdateResponse == "Success")
            {
                oCR.oResult = "Success";
            }
            else
            {
                oCR.oResult = "Failure";
            }
            oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCR;
        }

        public async Task WebhookOutbound(string sIdentifier, string sParam1 = "", string sParam2 = "", string sParam3 = "", string sParam4 = "", string sParam5 = "")
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            CTraceStack oTrace = new CTraceStack();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                if (!string.IsNullOrEmpty(sIdentifier))
                {
                    var sParam = string.Empty;
                    XIIXI oXI = new XIIXI();
                    List<CNV> oWhrParams = new List<CNV>();
                    oWhrParams.Add(new CNV { sName = "sIdentifier", sValue = sIdentifier });
                    oWhrParams.Add(new CNV { sName = "iDirection", sValue = "20" });
                    var oWebhookI = new XIIBO();
                    oWebhookI = oXI.BOI("onewebhook", null, null, oWhrParams);
                    if (oWebhookI != null && oWebhookI.Attributes.Count() > 0)
                    {
                        var sConcatenator = oWebhookI.AttributeI("sConcatenator").sValue;
                        var sOutboundURL = oWebhookI.AttributeI("sOutboundURL").sValue;
                        var sHeader = oWebhookI.AttributeI("sHeader").sValue;
                        var sParam1Name = oWebhookI.AttributeI("sParam1").sValue;
                        var sParam2Name = oWebhookI.AttributeI("sParam2").sValue;
                        var sParam3Name = oWebhookI.AttributeI("sParam3").sValue;
                        var sParam4Name = oWebhookI.AttributeI("sParam4").sValue;
                        var sParam5Name = oWebhookI.AttributeI("sParam5").sValue;
                        if (!string.IsNullOrEmpty(sConcatenator) && string.IsNullOrEmpty(sOutboundURL) && string.IsNullOrEmpty(sHeader))
                        {
                            sParam = sParam1 == null ? null : sParam + sParam1Name + "=" + sParam1 + sConcatenator;
                            sParam = sParam2 == null ? null : sParam + sParam2Name + "=" + sParam2 + sConcatenator;
                            sParam = sParam3 == null ? null : sParam + sParam3Name + "=" + sParam3 + sConcatenator;
                            sParam = sParam4 == null ? null : sParam + sParam4Name + "=" + sParam4 + sConcatenator;
                            sParam = sParam5 == null ? null : sParam + sParam5Name + "=" + sParam5 + sConcatenator;
                            if (!string.IsNullOrEmpty(sParam))
                            {
                                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sParam is: " + sParam });
                                sParam = sParam.Substring(0, sParam.Length - sConcatenator.Length);
                                using (var client = new HttpClient())
                                {
                                    //Load Step1
                                    client.BaseAddress = new Uri("http://localhost:63722/");
                                    client.DefaultRequestHeaders.Accept.Clear();
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("Test:Test123")); //("Username:Password")  
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                                    #region Consume GET method  
                                    HttpResponseMessage response = await client.GetAsync("api/" + sOutboundURL + "?sParam=" + sParam);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var httpResponseResult = await response.Content.ReadAsStringAsync();
                                        List<CNV> oResponse = JsonConvert.DeserializeObject<List<CNV>>(httpResponseResult);
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sParam is empty" });
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV { sValue = "Mandatory parameters sConcatenator :" + sConcatenator + " or sOutboundURL:" + sOutboundURL + " or sHeader:" + sHeader + " are empty" });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "outbound webhook not found for Identifier:" + sIdentifier });
                    }
                }
                else
                {
                    oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sIdentifier is empty" });
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
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
        }

        public async Task PushToWhatsapp(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var sTemplate = oParams.Where(m => m.sName.ToLower() == "stemplate").Select(m => m.sValue).FirstOrDefault();
                var sMob = oParams.Where(m => m.sName.ToLower() == "smob").Select(m => m.sValue).FirstOrDefault();
                var FKiBODID = oParams.Where(m => m.sName.ToLower() == "fkibodid").Select(m => m.sValue).FirstOrDefault();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var FKiCommID = oParams.Where(m => m.sName.ToLower() == "fkicommid").Select(m => m.sValue).FirstOrDefault();
                var FKiCampaignIDXIGUID = oParams.Where(m => m.sName.ToLower() == "FKiCampaignIDXIGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");
                if (!string.IsNullOrEmpty(sMobCode))
                {
                    if (!sMob.StartsWith(sMobCode))
                    {
                        sMob = sMobCode + sMob;
                    }
                }
                if (!string.IsNullOrEmpty(sTemplate) && !string.IsNullOrEmpty(sMob))
                {
                    XIIBO oBOI = new XIIBO();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XWhatsappObject");
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sMob", sMob);
                    oBOI.SetAttribute("sContentSID", sTemplate);
                    oBOI.SetAttribute("FKiBODID", FKiBODID);
                    oBOI.SetAttribute("FKiBOIID", FKiBOIID);
                    oBOI.SetAttribute("FKiCommunicationID", FKiCommID);
                    oBOI.SetAttribute("FKiCampaignIDXIGUID", FKiCampaignIDXIGUID);
                    oBOI.SetAttribute("iStatus", "10");
                    oBOI.Save(oBOI);
                    //Lead campaign map object status change
                    List<CNV> oWhr = new List<CNV>();
                    oWhr.Add(new CNV { sName = "FKiCommunicationID", sValue = FKiCommID });
                    int iLCMapID = 0;
                    XIIBO oLCBOI = new XIIBO();
                    oLCBOI = oXI.BOI("BSPLeadCampaignMap", null, null, oWhr);
                    if (oLCBOI != null && oLCBOI.Attributes.Count() > 0)
                    {
                        oLCBOI.SetAttribute("iStatus", "10");
                        oLCBOI.SetAttribute("iCampaignStatus", "30");
                        oLCBOI.Save(oLCBOI);
                        iLCMapID = oLCBOI.AttributeI("id").iValue;
                    }
                    SendTemplateRequest oSend = new SendTemplateRequest();
                    oSend.ContentSID = sTemplate;
                    oSend.MobileNumber = sMob;
                    Dictionary<string, string> Variables = new Dictionary<string, string>();
                    Variables["XLeadCampaignID"] = iLCMapID.ToString();
                    oSend.Variables = Variables;
                    var clientRes = new HttpClient();
                    clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                    clientRes.DefaultRequestHeaders.Accept.Clear();
                    clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var requeststring = JsonConvert.SerializeObject(oSend);
                    HttpContent content = new StringContent(requeststring, Encoding.UTF8, "application/json");
                    var oResponse = clientRes.PostAsync("api/message/send-template", content).Result;
                }
                else
                {
                    oCR.sMessage = "ERROR: PushToWhatsapp(): Mandatory params sTemplate or sMob are missing";
                    oDefBase.SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushToWhatsapp(): " + ex.ToString();
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task PushToWhatsappCallBack(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                var sMob = oParams.Where(m => m.sName.ToLower() == "smob").Select(m => m.sValue).FirstOrDefault();
                Dictionary<string, string> Variables = new Dictionary<string, string>();
                Variables["TemplateID"] = "Value";
                Variables["TemplateType"] = "Value";
                MessageCallbackInput oFile = new MessageCallbackInput();
                oFile.PhoneNumber = sMob;
                //oFile.MediaUrl = "https://www.systemsdna.com/whatsapp_bot/wwwroot/QSImages/Caribbean.pdf";
                oFile.MediaUrl = "https://oneplatformfactory.com/Whatsapp_Bot/wwwroot/QSImages/Caribbean.pdf";
                oFile.MessageType = MediaType.File;
                oFile.Message = "Policy document";
                oFile.Variables = Variables;
                var clientRes1 = new HttpClient();
                clientRes1.BaseAddress = new Uri("https://www.systemsdna.com/Whatsapp_Bot/");
                clientRes1.DefaultRequestHeaders.Accept.Clear();
                clientRes1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var requeststring1 = JsonConvert.SerializeObject(oFile);
                HttpContent content1 = new StringContent(requeststring1, Encoding.UTF8, "application/json");
                await clientRes1.PostAsync("api/message/callback", content1);
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushToWhatsapp(): " + ex.ToString();
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task Test_Sendgrid(SendEmailRequest oRequest)
        {
            // Your API endpoint URL
            XIInfraCache oCache = new XIInfraCache();
            var apiUrl = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "SendgridAPIURL");
            //string apiUrl = "https://platformfactory.in:8444/api/mail/send";
            // Your API key
            string apiKey = "3719d531-ca35-415b-b343-d06469caa0ee";
            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Set the API key in the request headers
                client.DefaultRequestHeaders.Add("xi-api-secret", apiKey);
                try
                {
                    string jsonData = JsonConvert.SerializeObject(oRequest);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    // Make a GET request to the API
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string resp = await response.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        string resp = await response.Content.ReadAsStringAsync();

                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"API request failed: {ex.Message}");
                }
            }
        }

        public async Task PushToWhatsappMeta(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                oCR.sMessage = "ERROR: PushToWhatsappMeta(): Called from XIDNA";
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var sMob = oParams.Where(m => m.sName.ToLower() == "smob").Select(m => m.sValue).FirstOrDefault();
                var FKiBODID = oParams.Where(m => m.sName.ToLower() == "fkibodid").Select(m => m.sValue).FirstOrDefault();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var FKiCommID = oParams.Where(m => m.sName.ToLower() == "fkicommid").Select(m => m.sValue).FirstOrDefault();
                var FKiCampaignID = oParams.Where(m => m.sName.ToLower() == "FKiCampaignID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iCampaignID = 0;
                int.TryParse(FKiCampaignID, out iCampaignID);
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");

                if (!string.IsNullOrEmpty(sMob) && !string.IsNullOrEmpty(FKiCommID) && !string.IsNullOrEmpty(FKiCampaignID))
                {
                    bool bProceed = true;
                    var oCampaignI = oXI.BOI("BSPCampaign", FKiCampaignID);
                    if (oCampaignI != null && oCampaignI.Attributes.Count() > 0)
                    {
                        var iPilotID = oCampaignI.AttributeI("FKiPilotID").iValue;
                        List<CNV> oNV = new List<CNV>();
                        oNV.Add(new CNV { sName = "sMobileNo", sValue = sMob });
                        oNV.Add(new CNV { sName = "FKiPilotID", sValue = iPilotID.ToString() });
                        var oFilter = oXI.BOI("BSPFilterCampaign", null, null, oNV);
                        if (oFilter != null && oFilter.Attributes.Count() > 0)
                        {
                            bProceed = false;
                        }
                    }
                    if (bProceed)
                    {
                        var oCommI = oXI.BOI("XICommunicationI", FKiCommID);
                        if (oCommI != null && oCommI.Attributes.Count() > 0)
                        {
                            Guid QSDXIGUID = Guid.Empty;
                            var sFKQSDXIGUID = oCommI.AttributeI("FKiQSDIDXIGUID").sValue;
                            if (Guid.TryParse(sFKQSDXIGUID, out QSDXIGUID))
                            {
                                XIIBO oBOI = new XIIBO();
                                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XWhatsappObject");
                                oBOI.BOD = oBOD;
                                oBOI.SetAttribute("sMob", sMob);
                                oBOI.SetAttribute("FKiBODID", FKiBODID);
                                oBOI.SetAttribute("FKiBOIID", FKiBOIID);
                                oBOI.SetAttribute("FKiCommunicationID", FKiCommID);
                                oBOI.SetAttribute("FKiCampaignID", FKiCampaignID);
                                oBOI.SetAttribute("iStatus", "10");
                                oBOI.Save(oBOI);
                                //Lead campaign map object status change
                                List<CNV> oWhr = new List<CNV>();
                                oWhr.Add(new CNV { sName = "FKiCommunicationID", sValue = FKiCommID });
                                int iLCMapID = 0;
                                XIIBO oLCBOI = new XIIBO();
                                oLCBOI = oXI.BOI("BSPLeadCampaignMap", null, null, oWhr);
                                if (oLCBOI != null && oLCBOI.Attributes.Count() > 0)
                                {
                                    oLCBOI.SetAttribute("iStatus", "10");
                                    oLCBOI.SetAttribute("iCampaignStatus", "30");
                                    oLCBOI.Save(oLCBOI);
                                    iLCMapID = oLCBOI.AttributeI("id").iValue;
                                }
                                var clientRes = new HttpClient();
                                sWhatsappAPIURL = "http://localhost:63720/";
                                //QSDXIGUID = new Guid("110f0ca1-be8f-44cc-877f-b0e30902f591");
                                clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                                clientRes.DefaultRequestHeaders.Accept.Clear();
                                clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                oCR.sMessage = "Info: PushToWhatsappMeta(): CreateMetaQS API method calling started";
                                oCR.sCategory = "Whatsapp API";
                                oDefBase.SaveErrortoDB(oCR);
                                HttpResponseMessage oResponse = await clientRes.GetAsync("api/QSAPIMeta/CreateMetaQS?QSD=" + QSDXIGUID.ToString() + "&sUniqueRef=" + sMob + "&FKiCampaignID=" + iCampaignID);
                                if (oResponse != null)
                                {
                                    var httpResponseResult = oResponse.Content.ReadAsStringAsync().Result;
                                    string oResponse1 = JsonConvert.DeserializeObject<string>(httpResponseResult);
                                    oCR.sMessage = "Info: PushToWhatsappMeta(): CreateMetaQS API Response serialize:" + oResponse1.ToString();
                                    oCR.sCategory = "Whatsapp API";
                                    oDefBase.SaveErrortoDB(oCR);
                                    var QSIGUID = Guid.Empty;
                                    if (Guid.TryParse(oResponse1, out QSIGUID))
                                    {
                                        oBOI.SetAttribute("FKiQSIIDXIGUID", QSIGUID.ToString());
                                        oBOI.SetAttribute("iStatus", "10");
                                        oBOI.Save(oBOI);
                                        oLCBOI.SetAttribute("FKiQSIIDXIGUID", QSIGUID.ToString());
                                        oLCBOI.SetAttribute("iStatus", "10");
                                        oLCBOI.Save(oLCBOI);
                                        oCommI.SetAttribute("iSendStatus", "20");
                                        oCR = oCommI.Save(oCommI);
                                    }
                                    else if (!string.IsNullOrEmpty(oResponse1) && oResponse1.ToLower() == "failure")
                                    {
                                        oCR.sMessage = "Error: PushToWhatsappMeta(): received failure status from CreateMetaQS API call";
                                        oCR.sCategory = "Whatsapp API";
                                        oDefBase.SaveErrortoDB(oCR);
                                        oBOI.SetAttribute("iStatus", "20");
                                        oBOI.Save(oBOI);
                                        oLCBOI.SetAttribute("iStatus", "20");
                                        oLCBOI.Save(oLCBOI);
                                        oCommI.SetAttribute("iSendStatus", "50");
                                        oCR = oCommI.Save(oCommI);
                                    }
                                    else
                                    {
                                        oCR.sMessage = "Error: PushToWhatsappMeta(): while updating QSIIDXIGUID to XWhatsappObject and API Response QSIIDXIGUID:" + oResponse1.ToString() + " ";
                                        oCR.sCategory = "Whatsapp API";
                                        oDefBase.SaveErrortoDB(oCR);
                                    }
                                }
                            }
                            else
                            {
                                oCR.sMessage = "ERROR: PushToWhatsappMeta(): QS definition guid is not loaded for comminication:" + FKiCommID;
                                oCR.sCategory = "Whatsapp API";
                                oDefBase.SaveErrortoDB(oCR);
                            }
                        }
                        else
                        {
                            oCR.sMessage = "ERROR: PushToWhatsappMeta(): Communication is not loaded for:" + FKiCommID;
                            oCR.sCategory = "Whatsapp API";
                            oDefBase.SaveErrortoDB(oCR);
                        }
                    }
                    else
                    {
                        var oCommI = oXI.BOI("XICommunicationI", FKiCommID);
                        if (oCommI != null && oCommI.Attributes.Count() > 0)
                        {
                            oCommI.SetAttribute("iSendStatus", "110");
                            oCR = oCommI.Save(oCommI);
                            List<CNV> oWhr = new List<CNV>();
                            oWhr.Add(new CNV { sName = "FKiCommunicationID", sValue = FKiCommID });
                            XIIBO oLCBOI = new XIIBO();
                            oLCBOI = oXI.BOI("BSPLeadCampaignMap", null, null, oWhr);
                            if (oLCBOI != null && oLCBOI.Attributes.Count() > 0)
                            {
                                oLCBOI.SetAttribute("iStatus", "20");
                                oLCBOI.SetAttribute("iCampaignStatus", "60");
                                oLCBOI.Save(oLCBOI);
                            }
                        }
                    }
                }
                else
                {
                    oCR.sMessage = "ERROR: PushToWhatsappMeta(): Mandatory params sMob or CommID is missing";
                    oCR.sCategory = "Whatsapp API";
                    oDefBase.SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushToWhatsappMeta(): " + ex.ToString();
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task PushDelayWhatsappMeta(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");
                oCR.sMessage = "Info: PushDelayWhatsappMeta(): DelayMetaQS calling started from XIDNA";
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
                if (!string.IsNullOrEmpty(FKiBOIID))
                {
                    //Load schedular instance
                    var oScheduleIns = oXI.BOI("XIScheduleInstance", FKiBOIID);
                    if (oScheduleIns != null && oScheduleIns.Attributes.Count() > 0)
                    {
                        oScheduleIns.SetAttribute("iStatus", "40");
                        oCR = oScheduleIns.Save(oScheduleIns);
                        Guid QSIXIGUID = Guid.Empty;
                        Guid StepDGuid = Guid.Empty;
                        var sFKQSIXIGUID = oScheduleIns.AttributeI("FKiQSIIDXIGUID").sValue;
                        var sStepDGuid = oScheduleIns.AttributeI("FKiStepDXIGUID").sValue;
                        //var FKiCampaignID = oScheduleIns.AttributeI("FKiCampaignID").sValue;
                        if (Guid.TryParse(sFKQSIXIGUID, out QSIXIGUID) && Guid.TryParse(sStepDGuid, out StepDGuid))
                        {
                            List<CNV> oWhr = new List<CNV>();
                            oWhr.Add(new CNV { sName = "FKiQSIIDXIGUID", sValue = sFKQSIXIGUID });
                            var oXWhatsapp = oXI.BOI("XWhatsappObject", null, null, oWhr);
                            if (oXWhatsapp != null && oXWhatsapp.Attributes.Count() > 0)
                            {
                                var sMob = oXWhatsapp.AttributeI("sMob").sValue;
                                var FKiCampaignID = oXWhatsapp.AttributeI("FKiCampaignID").sValue;
                                var FKiCommunicationID = oXWhatsapp.AttributeI("FKiCommunicationID").iValue;
                                if (!string.IsNullOrEmpty(sMob))
                                {
                                    var clientRes = new HttpClient();
                                    clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                                    clientRes.DefaultRequestHeaders.Accept.Clear();
                                    clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    var oResponse = clientRes.GetAsync("api/QSAPIMeta/DelayMetaQS?sQSIIDXIGUID=" + QSIXIGUID.ToString() + "&sUniqueRef=" + sMob + "&sNextStep=" + StepDGuid + "&FKiCampaignID=" + FKiCampaignID + "&iCommunicationID=" + FKiCommunicationID).Result;
                                    if (oResponse != null)
                                    {
                                        var httpResponseResult = await oResponse.Content.ReadAsStringAsync();
                                        string oResponse1 = JsonConvert.DeserializeObject<string>(httpResponseResult);
                                        Guid ResQSIXIGUID = Guid.Empty;
                                        Guid.TryParse(oResponse1, out ResQSIXIGUID);
                                        if (ResQSIXIGUID != null && ResQSIXIGUID != Guid.Empty)
                                        {

                                        }
                                        else if (!string.IsNullOrEmpty(oResponse1) && oResponse1.ToLower() == "failure")
                                        {
                                            oScheduleIns.SetAttribute("iStatus", "60");
                                            oCR = oScheduleIns.Save(oScheduleIns);
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    oCR.sMessage = "ERROR: PushDelayWhatsappMeta(): Mobile number is not loaded for schedule instance:" + FKiBOIID + " and QS instance:" + sFKQSIXIGUID;
                                    oCR.sCategory = "Whatsapp API";
                                    oDefBase.SaveErrortoDB(oCR);
                                }
                            }
                            else
                            {
                                oCR.sMessage = "ERROR: PushDelayWhatsappMeta(): XWhatsappObject instance is not loaded for QS instance:" + sFKQSIXIGUID;
                                oCR.sCategory = "Whatsapp API";
                                oDefBase.SaveErrortoDB(oCR);
                            }
                        }
                        else
                        {
                            oCR.sMessage = "ERROR: PushDelayWhatsappMeta(): QS definition guid or Step definition guid is not loaded for schedule instance:" + FKiBOIID;
                            oCR.sCategory = "Whatsapp API";
                            oDefBase.SaveErrortoDB(oCR);
                        }
                    }
                    else
                    {
                        oCR.sMessage = "ERROR: PushDelayWhatsappMeta(): Schedule instance is not loaded for:" + FKiBOIID;
                        oCR.sCategory = "Whatsapp API";
                        oDefBase.SaveErrortoDB(oCR);
                    }
                }
                else
                {
                    oCR.sMessage = "ERROR: PushDelayWhatsappMeta(): Mandatory params sMob or CommID is missing";
                    oCR.sCategory = "Whatsapp API";
                    oDefBase.SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushToWhatsappMeta(): " + ex.ToString();
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task PushToWhatsappMeta1(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var sMob = oParams.Where(m => m.sName.ToLower() == "smob").Select(m => m.sValue).FirstOrDefault();
                var FKiCommID = oParams.Where(m => m.sName.ToLower() == "fkicommid").Select(m => m.sValue).FirstOrDefault();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");
                oCR.sMessage = "Info: PushToWhatsappMeta(): calling started from XIDNA";
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
                if (!string.IsNullOrEmpty(sMob) && !string.IsNullOrEmpty(FKiCommID))
                {
                    var oCommI = oXI.BOI("XICommunicationI", FKiCommID);
                    if (oCommI != null && oCommI.Attributes.Count() > 0)
                    {
                        Guid QSDXIGUID = Guid.Empty;
                        var sFKQSDXIGUID = oCommI.AttributeI("FKiQSDIDXIGUID").sValue;
                        if (Guid.TryParse(sFKQSDXIGUID, out QSDXIGUID))
                        {
                            var clientRes = new HttpClient();
                            clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                            clientRes.DefaultRequestHeaders.Accept.Clear();
                            clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            oCR.sMessage = "Info: PushToWhatsappMeta(): CreateMetaQS API method calling started from XIDNA";
                            oCR.sCategory = "Whatsapp API";
                            oDefBase.SaveErrortoDB(oCR);
                            HttpResponseMessage oResponse = clientRes.GetAsync("api/QSAPIMeta/CreateMetaQS?QSD=" + QSDXIGUID + "&sUniqueRef=" + sMob).Result;
                            if (oResponse != null)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushToWhatsappMeta(): " + ex.ToString();
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task PushScheduleInstance(List<CNV> oParams)
        {
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            CResult oCR = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");
                oCR.sMessage = "Info: PushScheduleInstance(): calling started from XIDNA";
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
                if (!string.IsNullOrEmpty(FKiBOIID))
                {
                    var oSchdI = oXI.BOI("XIScheduleInstance", FKiBOIID);
                    if (oSchdI != null && oSchdI.Attributes.Count() > 0)
                    {
                        Guid QSDXIGUID = Guid.Empty;
                        var sFKQSDXIGUID = oSchdI.AttributeI("FKiQSDIDXIGUID").sValue;
                        var sMob = oSchdI.AttributeI("sMobileNo").sValue;
                        var FKiCampaignID = oSchdI.AttributeI("FKiCampaignID").sValue;
                        bool bProceed = true;
                        var oCampaignI = oXI.BOI("BSPCampaign", FKiCampaignID);
                        if (oCampaignI != null && oCampaignI.Attributes.Count() > 0)
                        {
                            var iPilotID = oCampaignI.AttributeI("FKiPilotID").iValue;
                            List<CNV> oNV = new List<CNV>();
                            oNV.Add(new CNV { sName = "sMobileNo", sValue = sMob });
                            oNV.Add(new CNV { sName = "FKiPilotID", sValue = iPilotID.ToString() });
                            var oFilter = oXI.BOI("BSPFilterCampaign", null, null, oNV);
                            if (oFilter != null && oFilter.Attributes.Count() > 0)
                            {
                                bProceed = false;
                            }
                        }
                        if (bProceed)
                        {
                            if (Guid.TryParse(sFKQSDXIGUID, out QSDXIGUID) && !string.IsNullOrEmpty(sMob))
                            {
                                oSchdI.SetAttribute("iStatus", "40");
                                oCR = oSchdI.Save(oSchdI);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var iBOIID = oSchdI.AttributeI("FKiBOIID").sValue;
                                    var BODGuid = oSchdI.AttributeI("FKiBODIDXIGUID").sValue;
                                    XIIBO oCommI = new XIIBO();
                                    oCommI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI");
                                    oCommI.SetAttribute("sTO", sMob);
                                    oCommI.SetAttribute("iSendStatus", "10");
                                    oCommI.SetAttribute("iDirection", "20");
                                    //oCommI.SetAttribute("iMethod", "30");
                                    oCommI.SetAttribute("iComType", "30");
                                    oCommI.SetAttribute("FKiQSDIDXIGUID", QSDXIGUID.ToString());
                                    oCommI.SetAttribute("XIInstanceOriginID", oSchdI.AttributeI("FKiBOIID").sValue);
                                    oCommI.SetAttribute("XIInstanceDefinitionIDXIGUID", oSchdI.AttributeI("FKiBODIDXIGUID").sValue);
                                    oCR = oCommI.Save(oCommI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oCommI = (XIIBO)oCR.oResult;
                                        var iCommID = oCommI.AttributeI("id").sValue;
                                        XIIBO oLCMapI = new XIIBO();
                                        oLCMapI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BSPLeadCampaignMap");
                                        oLCMapI.SetAttribute("FKiCampaignID", FKiCampaignID);
                                        oLCMapI.SetAttribute("iStatus", "20");
                                        oLCMapI.SetAttribute("iScheduled", "20");
                                        oLCMapI.SetAttribute("FKiCommunicationID", iCommID);
                                        oLCMapI.SetAttribute("sMobileNo", sMob);
                                        oLCMapI.SetAttribute("FKiBOIID", iBOIID);
                                        oLCMapI.SetAttribute("FKiBODIDXIGUID", BODGuid);
                                        oCR = oLCMapI.Save(oLCMapI);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oLCMapI = (XIIBO)oCR.oResult;
                                            var iLCMapIID = oLCMapI.AttributeI("id").sValue;
                                            XIIBO oBOI = new XIIBO();
                                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XWhatsappObject");
                                            oBOI.BOD = oBOD;
                                            oBOI.SetAttribute("sMob", sMob);
                                            oBOI.SetAttribute("FKiCommunicationID", iCommID);
                                            oBOI.SetAttribute("iStatus", "10");
                                            oBOI.SetAttribute("FKiCampaignID", FKiCampaignID);
                                            oCR = oBOI.Save(oBOI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                var clientRes = new HttpClient();
                                                //sWhatsappAPIURL = "http://localhost:63720/";
                                                clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                                                clientRes.DefaultRequestHeaders.Accept.Clear();
                                                clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                oCR.sMessage = "Info: PushToWhatsappMeta(): CreateMetaQS_Schedule API method calling started from XIDNA";
                                                oCR.sCategory = "Whatsapp API";
                                                oDefBase.SaveErrortoDB(oCR);
                                                HttpResponseMessage oResponse = clientRes.GetAsync("api/QSAPIMeta/CreateMetaQS_Schedule?QSD=" + QSDXIGUID + "&sUniqueRef=" + sMob + "&iCommID=" + iCommID + "&FKiCampaignID=" + FKiCampaignID).Result;
                                                if (oResponse != null)
                                                {
                                                    var httpResponseResult = await oResponse.Content.ReadAsStringAsync();
                                                    string oResponse1 = JsonConvert.DeserializeObject<string>(httpResponseResult);
                                                    oCR.sMessage = "Info: PushScheduleInstance(): CreateMetaQS_Schedule API Response serialize:" + oResponse1.ToString();
                                                    oCR.sCategory = "Whatsapp API";
                                                    oDefBase.SaveErrortoDB(oCR);
                                                    Guid QSIGUID = Guid.Empty;
                                                    Guid.TryParse(oResponse1, out QSIGUID);
                                                    if (QSIGUID != null && QSIGUID != Guid.Empty)
                                                    {
                                                        oSchdI.SetAttribute("iStatus", "10");
                                                        oCR = oSchdI.Save(oSchdI);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            oCR.sMessage = "ERROR: PushScheduleInstance(): instance status:10 update saving failed for instance" + FKiBOIID;
                                                            oCR.sCategory = "Whatsapp API";
                                                            oDefBase.SaveErrortoDB(oCR);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        oSchdI.SetAttribute("iStatus", "60");
                                                        oCR = oSchdI.Save(oSchdI);
                                                        oCR.sMessage = "ERROR: PushScheduleInstance(): CreateMetaQS_Schedule api response is not a valid guid";
                                                        oCR.sCategory = "Whatsapp API";
                                                        oDefBase.SaveErrortoDB(oCR);
                                                    }
                                                }
                                                else
                                                {
                                                    oCR.sMessage = "ERROR: PushScheduleInstance(): CreateMetaQS_Schedule api response is null";
                                                    oCR.sCategory = "Whatsapp API";
                                                    oDefBase.SaveErrortoDB(oCR);
                                                }
                                            }
                                            else
                                            {
                                                oCR.sMessage = "ERROR: PushScheduleInstance(): XWhatsappObject instance saving failed";
                                                oCR.sCategory = "Whatsapp API";
                                                oDefBase.SaveErrortoDB(oCR);
                                            }
                                        }
                                        else
                                        {
                                            oCR.sMessage = "ERROR: PushScheduleInstance(): XTragetCampaign instance saving failed";
                                            oCR.sCategory = "Whatsapp API";
                                            oDefBase.SaveErrortoDB(oCR);
                                        }
                                    }
                                    else
                                    {
                                        oCR.sMessage = "ERROR: PushScheduleInstance(): Communication instance saving failed";
                                        oCR.sCategory = "Whatsapp API";
                                        oDefBase.SaveErrortoDB(oCR);
                                    }
                                }
                                else
                                {
                                    oCR.sMessage = "ERROR: PushScheduleInstance(): instance status:40 update saving failed for instance" + FKiBOIID;
                                    oCR.sCategory = "Whatsapp API";
                                    oDefBase.SaveErrortoDB(oCR);
                                }
                            }
                            else
                            {
                                oCR.sMessage = "ERROR: PushScheduleInstance(): QSDID or Mobile number Mandatory parameters not passed for instance:" + FKiBOIID;
                                oCR.sCategory = "Whatsapp API";
                                oDefBase.SaveErrortoDB(oCR);
                            }
                        }
                        else
                        {
                            oSchdI.SetAttribute("istatus", "50");
                            oCR = oSchdI.Save(oSchdI);
                        }
                    }
                    else
                    {
                        oCR.sMessage = "ERROR: PushScheduleInstance(): Schedule instance not loaded for :" + FKiBOIID;
                        oCR.sCategory = "Whatsapp API";
                        oDefBase.SaveErrortoDB(oCR);
                    }
                }
                else
                {
                    oCR.sMessage = "ERROR: PushScheduleInstance(): BOIID Mandatory parameter not passed";
                    oCR.sCategory = "Whatsapp API";
                    oDefBase.SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: PushScheduleInstance(): " + ex.ToString();
                oCR.sCategory = "Whatsapp API";
                oDefBase.SaveErrortoDB(oCR);
            }
        }

        public async Task PushScheduleInstanceSMS(List<CNV> oParams)
        {
            XIDefinitionBase oDef = new XIDefinitionBase();
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var FKiBOIID = oParams.Where(m => m.sName.ToLower() == "fkiboiid").Select(m => m.sValue).FirstOrDefault();
                var sMobCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappMobileCode");
                var sWhatsappAPIURL = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "WhatsappAPIURL");
                oCR.sMessage = "Info: PushScheduleInstanceSMS(): calling started from XIDNA";
                oCR.sCategory = "Whatsapp API";
                oDef.SaveErrortoDB(oCR);
                if (!string.IsNullOrEmpty(FKiBOIID))
                {
                    var oSchdI = oXI.BOI("XIScheduleInstance", FKiBOIID);
                    if (oSchdI != null && oSchdI.Attributes.Count() > 0)
                    {
                        var sMob = oSchdI.AttributeI("sMobileNo").sValue;
                        var FKiCampaignID = oSchdI.AttributeI("FKiCampaignID").sValue;
                        oSchdI.SetAttribute("iStatus", "40");
                        oCR = oSchdI.Save(oSchdI);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            XIIBO oCommI = new XIIBO();
                            oCommI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI");
                            oCommI.SetAttribute("sTO", sMob);
                            oCommI.SetAttribute("iSendStatus", "10");
                            oCommI.SetAttribute("iDirection", "20");
                            oCommI.SetAttribute("iMethod", "20");
                            oCommI.SetAttribute("iComType", "30");
                            oCommI.SetAttribute("XIInstanceOriginID", oSchdI.AttributeI("FKiBOIID").sValue);
                            oCommI.SetAttribute("XIInstanceDefinitionIDXIGUID", oSchdI.AttributeI("FKiBODIDXIGUID").sValue);
                            oCR = oCommI.Save(oCommI);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oCommI = (XIIBO)oCR.oResult;
                                var iCommID = oCommI.AttributeI("id").sValue;
                                XIIBO oBOI = new XIIBO();
                                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XWhatsappObject");
                                oBOI.BOD = oBOD;
                                oBOI.SetAttribute("sMob", sMob);
                                oBOI.SetAttribute("FKiCommunicationID", iCommID);
                                oBOI.SetAttribute("iStatus", "10");
                                oBOI.SetAttribute("FKiCampaignID", FKiCampaignID);
                                oBOI.SetAttribute("iType", "20");
                                oCR = oBOI.Save(oBOI);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    //string sMob = "+447753613487";
                                    string sPhoneNo = "447979213225";
                                    string sQSName = "Investor Media";
                                    var clientRes = new HttpClient();
                                    //sWhatsappAPIURL = "http://localhost:63720/";
                                    clientRes.BaseAddress = new Uri(sWhatsappAPIURL);
                                    clientRes.DefaultRequestHeaders.Accept.Clear();
                                    clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    oCR.sMessage = "Info: PushToWhatsappMeta(): Send_SMSCampaign API method calling started from XIDNA";
                                    oCR.sCategory = "Whatsapp API";
                                    oDef.SaveErrortoDB(oCR);
                                    HttpResponseMessage oResponse = clientRes.GetAsync("api/QSAPIMeta/Send_SMSCampaign?sMob=" + sMob + "&sPhoneNo=" + sPhoneNo + "&sQSName=" + sQSName).Result;
                                    if (oResponse != null)
                                    {
                                        var httpResponseResult = await oResponse.Content.ReadAsStringAsync();
                                        string oResponse1 = JsonConvert.DeserializeObject<string>(httpResponseResult);
                                        oCR.sMessage = "Info: PushScheduleInstanceSMS(): Send_SMSCampaign API Response serialize:" + oResponse1.ToString();
                                        oCR.sCategory = "Whatsapp API";
                                        oDef.SaveErrortoDB(oCR);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: Mobile Number: - Class:QSAPIMetaController, Method:Send_SMS, Exception: " + ex.ToString();
                oCResult.sCategory = "Whatsapp API";
                oDef.SaveErrortoDB(oCResult);
                //return "Faliure");
            }
        }
    }

    public class SendTemplateRequest
    {
        public string ContentSID { get; set; }
        public string MobileNumber { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }

    public class MessageCallbackInput
    {
        [Required]
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string MediaUrl { get; set; }
        public MediaType MessageType { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
    public enum MediaType
    {
        Text,
        File,
        ClickToAction,
        ListItem
    }
}