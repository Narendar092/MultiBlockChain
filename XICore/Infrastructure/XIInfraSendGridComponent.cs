using Microsoft.SqlServer.Management.Smo;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using xiEnumSystem;
using XISystem;
using System.Net.Mail;
using System.Web.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace XICore
{
    public class XIInfraSendGridComponent : XIInstanceBase
    {
        public string sTo { get; set; }
        public string sName { get; set; }
        public string sCC { get; set; }
        public string sCCName { get; set; }
        public object oDynamicData { get; set; }
        public string sBody { get; set; }
        public string sSubject { get; set; }
        public string sAccountID { get; set; }

        public string CommID { get; set; }
        public List<AttachmentSG> Attachments { get; set; }
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Sending mail through Send Grid";//expalin about this method logic
            string sCode = "Send Grid";
            List<CNV> oTraceInfo = new List<CNV>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            var AppID = oInfo.iApplicationID;
            var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Send grid mail sending started" });
                int iAccountID = 0;
                Guid AccountGUID = Guid.Empty;
                var AccountID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridAccountID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sTo = oParams.Where(m => m.sName.ToLower() == "sTo".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(AccountID, out iAccountID);
                Guid.TryParse(AccountID, out AccountGUID);
                var oSendGridTemplate = new XIDSendGridTemplate();
                if (!string.IsNullOrEmpty(sTo))
                {
                    oTraceInfo.Add(new CNV { sValue = "sTo:" + sTo });
                    if (iAccountID > 0 || (AccountGUID != null && AccountGUID != Guid.Empty))
                    {
                        var oAccountD = new XIDAccount();
                        if (AccountGUID != null && AccountGUID != Guid.Empty)
                        {
                            oAccountD = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, AccountGUID.ToString());
                        }
                        else if (iAccountID > 0)
                        {
                            oAccountD = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, iAccountID.ToString());
                        }
                        if (oAccountD != null)
                        {
                            oTraceInfo.Add(new CNV { sValue = "Sendgrid account loaded successfully" });
                            string sTemplateName = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridTemplateName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                            string sTemplateID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridTemplateID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(sTemplateName) || !string.IsNullOrEmpty(sTemplateID))
                            {
                                oSendGridTemplate = (XIDSendGridTemplate)oCache.GetObjectFromCache(XIConstant.CacheSendGridTemplate, sTemplateName, sTemplateID);
                                if (!string.IsNullOrEmpty(oSendGridTemplate.sTemplateID))
                                {
                                    oTraceInfo.Add(new CNV { sValue = "Sendgrid template loaded successfully" });
                                    string sSGAPIKey = oAccountD.sAPIURL;
                                    SendGridMessage sendGridMessage = new SendGridMessage();
                                    string sSendGridAPIKey = oAccountD.sAPIURL,
                                    sSingleSender = oAccountD.sFromAddress,
                                    sSingleSenderName = oAccountD.sName;
                                    var Client = new SendGridClient(sSendGridAPIKey);
                                    // var sendGridMessage = new SendGridMessage();
                                    sendGridMessage.SetFrom(sSingleSender, sSingleSenderName);
                                    sendGridMessage.AddTo(sTo, sName);
                                    sendGridMessage.SetTemplateId(oSendGridTemplate.sTemplateID);

                                    // Attachement Code
                                    //var bytes = File.ReadAllBytes(HostingEnvironment.MapPath("~\\UploadedFiles\\Files\\docx\\2022\\7\\1\\e766a18b-9edf-431f-bb4e-d953a88ed5cf.docx"));
                                    //var file = Convert.ToBase64String(bytes);
                                    //sendGridMessage.AddAttachment("Document.docx", file);

                                    oDynamicData = new
                                    {
                                        url = "https://www.google.com/",
                                        button = "Click Me"
                                    };
                                    if (oDynamicData != null)
                                    {
                                        sendGridMessage.SetTemplateData(oDynamicData);
                                    }
                                    if (!string.IsNullOrEmpty(sCC))
                                        sendGridMessage.AddBcc(sCC, sCCName);
                                    oTraceInfo.Add(new CNV { sValue = "mail sending initiated" });
                                    var response = Client.SendEmailAsync(sendGridMessage).Result;
                                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                                    {
                                        Dictionary<string, string> Headers = response.DeserializeResponseHeaders(response.Headers);
                                        if (Headers.Keys.Any(m => m.ToLower() == "x-message-id"))
                                        {
                                            string sMessageID = Headers["X-Message-Id"];
                                            oCResult.oResult = sMessageID.ToString();
                                            if (oCResult.oResult != null)
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "mail sent successfully" });
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                            }
                                            else
                                            {
                                                oTraceInfo.Add(new CNV { sValue = "mail sending failed" });
                                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                oCResult.sMessage = "Mail sending failed using send grid to " + sTo;
                                                SaveErrortoDB(oCResult);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        oTraceInfo.Add(new CNV { sValue = "mail sending failed with response code:" + response.StatusCode });
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "Mail sending failed to: " + sTo;
                                        SaveErrortoDB(oCResult);
                                    }
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV { sValue = "Template loading failed for - " + sTemplateID + " or Template Name - " + sTemplateName });
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Template loading failed for - " + sTemplateID + " or Template Name - " + sTemplateName;
                                    SaveErrortoDB(oCResult);
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Mandatory params TemplateID - " + sTemplateID + " or Template Name - " + sTemplateName + " not passed correctly" });
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "Mandatory params TemplateID- " + sTemplateID + " or Template Name- " + sTemplateName + " not passed correctly";
                                SaveErrortoDB(oCResult);
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV { sValue = "Account loading failed for - " + AccountID });
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Account loading failed for - " + AccountID;
                            SaveErrortoDB(oCResult);
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "Mandatory params Send Grid Account ID: " + AccountID + " not passed correctly" });
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Mandatory params Send Grid Account ID: " + AccountID + " not passed correctly";
                        SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oTraceInfo.Add(new CNV { sValue = "Mandatory param sTo: " + sTo + " not passed correctly" });
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "Mandatory param sTo: " + sTo + " not passed correctly";
                    SaveErrortoDB(oCResult);
                }
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                {
                    oCResult.oTraceStack = oTraceInfo;
                    SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oTraceInfo.Add(new CNV { sValue = "mail sending failed" });
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + " - " + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.oTraceStack = oTraceInfo;
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        private readonly XIInfraCache oCache = new XIInfraCache();
        public CResult Load(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            try
            {
                //accountid
                //templateName  --> welcome mail
                //new method for template with templateName

                //get params
                XIDSendGridTemplate oSendGridTemplate = new XIDSendGridTemplate();
                int iSGADID = 0;
                int.TryParse(oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridAccountID.ToLower()).Select(m => m.sValue).FirstOrDefault(), out iSGADID);
                string sTemplateName = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridTemplateName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sTemplateID = oParams.Where(m => m.sName.ToLower() == XIConstant.Param_SendGridTemplateID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                sTo = oParams.Where(m => m.sName.ToLower() == "sTo".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //if (iSGADID > 0 && (!string.IsNullOrEmpty(sTemplateName) || !string.IsNullOrEmpty(sTemplateID)))
                if (iSGADID > 0)
                {
                    var oSendGridInfo = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, iSGADID.ToString());
                    if (!string.IsNullOrEmpty(sTemplateName) || !string.IsNullOrEmpty(sTemplateID))
                    {
                        oSendGridTemplate = (XIDSendGridTemplate)oCache.GetObjectFromCache(XIConstant.CacheSendGridTemplate, sTemplateName, sTemplateID);
                    }

                    if (!string.IsNullOrEmpty(sTo))
                    {
                        string sSGAPIKey = oSendGridInfo?.sAPIURL;
                        SendGridMessage sendGridMessage = new SendGridMessage();
                        string sSendGridAPIKey = oSendGridInfo?.sAPIURL,
                        sSingleSender = oSendGridInfo?.sFromAddress,
                        sSingleSenderName = oSendGridInfo?.sName;

                        if (!string.IsNullOrEmpty(oSendGridInfo?.sAPIURL) && !string.IsNullOrEmpty(sSingleSender))
                        {
                            var Client = new SendGridClient(sSendGridAPIKey);
                            // var sendGridMessage = new SendGridMessage();
                            sendGridMessage.SetFrom(sSingleSender, sSingleSenderName);
                            sendGridMessage.AddTo(sTo, sName);
                            if (!string.IsNullOrEmpty(oSendGridTemplate?.sTemplateID))
                            {
                                sendGridMessage.SetTemplateId(oSendGridTemplate.sTemplateID);

                                //var file = "file.jpg";
                                //var bytes = File.ReadAllBytes("C:\\Users\\ravit\\XIDNA\\VS Projects\\CreateIF\\wetransfer_cif-docs_2022-10-10_0951\\H&S\\B - Health & Safety File\\B - Health & Safety File\\App J - Landscape as-built\\5442-L-201-202&208 AS BUILT 201.pdf");
                                var bytes = File.ReadAllBytes(HostingEnvironment.MapPath("~\\UploadedFiles\\Files\\docx\\2022\\7\\1\\e766a18b-9edf-431f-bb4e-d953a88ed5cf.docx"));
                                var file = Convert.ToBase64String(bytes);
                                //sendGridMessage.AddAttachment("Document.docx", file);
                                //sendGridMessage.AddBccs("");
                            }
                            sendGridMessage.AddContent(MimeType.Text, "and easy to do anywhere, even with C#");
                            sendGridMessage.PlainTextContent = "Hello Plain Text";
                            sendGridMessage.HtmlContent = "Hello";
                            List<Content> contents = new List<Content>();
                            Content content = new Content("text/plain", "Test contents");
                            contents.Add(content);
                            sendGridMessage.Contents = contents;
                            if (oDynamicData != null)
                            {
                                sendGridMessage.SetTemplateData(oDynamicData);
                            }
                            else
                            {
                                //*******Getting dynamic tempalte from db 

                                //sendGridMessage.Subject = "Sendgrid without Using Template";
                                //XIIXI oboi = new XIIXI();
                                //var template = oboi.BOI("XITemplate_T", "909");
                                //var MessageContent = template.Attributes["Content"].sValue;
                                //sendGridMessage.HtmlContent = MessageContent;

                                //************************************

                                //sendGridMessage.PlainTextContent = "This messsage is from plane text without template";
                                //sendGridMessage.HtmlContent = "<html><body>Hi this message is from html Format<body></body>";
                                //List<EmailAddress> MutipleBcc = new List<EmailAddress>();
                                //MutipleBcc.Add(new EmailAddress { Email = "ashrafshaik523@gmail.com", Name = "Ashraf" });
                                //MutipleBcc.Add(new EmailAddress { Email = "", Name = "" });
                                //sendGridMessage.AddBccs(MutipleBcc);
                                //var myFile = new SendGrid.Helpers.Mail.Attachment();
                                // var bytes = File.ReadAllBytes("D:\\Statement Of Facts.pdf");
                                //var file = Convert.ToBase64String(bytes);
                                //myFile.Filename = "This is the Attachment";
                                //myFile.Content = file;
                                //XIInfraEmail Convertions = new XIInfraEmail();
                                //Convertions.sSurName = "Ashu";
                                //var PdfFile = Convertions.PDFGenerate(sendGridMessage.HtmlContent, true,"3-5","yyyy");
                                //var IronPdf = Convertions.IronPdf(sendGridMessage.HtmlContent,"MyFile",true, "3-5", "yyyy");
                                //var oAttachment = Convertions.GeneratePDFFile((MemoryStream)IronPdf.oResult);
                                //if (oAttachment.bOK && oAttachment.oResult != null)
                                // {
                                // System.Net.Mail.Attachment data = (System.Net.Mail.Attachment)oAttachment.oResult;
                                //var myFile = new SendGrid.Helpers.Mail.Attachment();
                                //myFile.ContentId = data.ContentId;
                                //myFile.Disposition = data.ContentDisposition.ToString();

                                //var bytes = File.ReadAllBytes(MessageContent.ToString());
                                //var file = Convert.ToBase64String(bytes);
                                //myFile.Content = MessageContent;
                                //myFile.Type = data.Name;
                                //myFile.Filename = data.Name;

                                //MemoryStream OMyfile = (MemoryStream)PdfFile.oResult;
                                //List<Attachment> oAttachments = new List<Attachment>();
                                //oAttachments.Add(data);
                                //myFile.Type = "pdf";
                                // Attachment attachment = new Attachment();
                                //myFile.Content = "TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2NpbmcgZWxpdC4gQ3JhcyBwdW12";
                                //myFile.Type = "application/pdf";
                                //myFile.Filename = "balance_001.pdf";
                                //myFile.Disposition = "attachment";
                                //myFile.ContentId = "Balance Sheet";
                                //mail.AddAttachment(attachment);
                                // sendGridMessage.AddAttachment(myFile);
                                //}
                                //sendGridMessage.AddAttachment(myFile);
                            }
                            if (!string.IsNullOrEmpty(sCC))
                                sendGridMessage.AddBcc(sCC, sCCName);
                            var response = Client.SendEmailAsync(sendGridMessage).Result;
                            //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                Dictionary<string, string> Headers = response.DeserializeResponseHeaders(response.Headers);
                                if (Headers.Keys.Any(m => m.ToLower() == "x-message-id"))
                                {
                                    string sMessageID = Headers["X-Message-Id"];

                                    oCResult.oResult = sMessageID.ToString();
                                    if (oCResult.oResult != null)
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "Mail Not Sent";
                                        SaveErrortoDB(oCResult);
                                    }
                                }
                            }
                            else
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "Mail Not Sent";
                                SaveErrortoDB(oCResult);
                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Mandatory params " + sSendGridAPIKey + " and " + sSingleSender + " not passed correctly";
                            SaveErrortoDB(oCResult);
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "param sTo : " + sTo + " not passed correctly";
                        SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "Mandatory params SendGridAccount ID : " + iSGADID + " or " + sTemplateID + " or " + sTemplateName + " not passed correctly";
                    SaveErrortoDB(oCResult);
                }

            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult SaveEmailActivity(string sMessageID, string sFrom, string sTo, string leadID, string CampaignID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDBO BOD = new XIDBO();
            XIDefinitionBase oXID = new XIDefinitionBase();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "communicationinstance");
                XIIBO oBOI = new XIIBO();
                oBOI.BOD = BOD;
                oBOI.SetAttribute("sFrom", sFrom);
                oBOI.SetAttribute("sTo", sTo);
                //oBOI.SetAttribute("iType", ((int)EnumCommnicationType.Email).ToString());
                oBOI.SetAttribute("sReference", sMessageID);
                oBOI.SetAttribute("FKiToContactID", "0");//will be change later
                oBOI.SetAttribute("sDirection", "outbound");
                oBOI.SetAttribute("iRetryCount", "0");
                oBOI.SetAttribute("sAPI", "");
                oBOI.SetAttribute("iStatus", ((int)EnumEmailStatus.sent).ToString());
                //oBOI.SetAttribute("iDeliveryStatus", ((int)EnumEmailStatus.processed).ToString());
                oBOI.SetAttribute("iInteractionStatus", ((int)EnumEmailStatus.processed).ToString());
                oBOI.SetAttribute("FKiCampaignID", CampaignID);
                oBOI.SetAttribute("FKiLeadID", leadID);
                oCR = oBOI.Save(oBOI);
                int ID = 0;
                if (oCR.bOK && oCR.oResult != null)
                {
                    int.TryParse(oBOI.Attributes["ID"].sValue, out ID);
                    if (ID > 0)
                    {
                        oCache = new XIInfraCache();
                        BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsTransaction");
                        oBOI = new XIIBO();
                        oBOI.BOD = BOD;
                        oBOI.SetAttribute("FKiCommInstanceID", ID.ToString());
                        oBOI.SetAttribute("iType", ((int)EnumEmailStatus.processed).ToString());
                        oBOI.SetAttribute("FKiCampaignID", CampaignID);
                        oCR = oBOI.Save(oBOI);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            XIIBO CheckLead = new XIIBO();
                            XIIXI LeadCampaign = new XIIXI();
                            List<CNV> oWhrLeadParams = new List<CNV>();
                            //sMessageID = item.sg_message_id.Split('.')[0];
                            oWhrLeadParams.Add(new CNV { sName = "FKiLeadID", sValue = leadID });
                            oWhrLeadParams.Add(new CNV { sName = "FKiCampaignID", sValue = CampaignID });
                            CheckLead = LeadCampaign.BOI("XLeadCampaign", null, null, oWhrLeadParams);
                            if (CheckLead == null)
                            {
                                var response = SaveEmailToXLeadCampaign(leadID, CampaignID, sMessageID, ID);
                                if (response.bOK && response.oResult != null)
                                {
                                    oCResult.oResult = oCR.oResult;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Error while saving into commsTransaction object";
                                    oXID.SaveErrortoDB(oCResult);
                                }

                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Error while saving into commsTransaction object";
                            oXID.SaveErrortoDB(oCResult);
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Error while saving into communication instance object";
                        oXID.SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = "Error while saving into communication instance object";
                    oXID.SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;

        }

        public CResult SaveEmailToXLeadCampaign(string leadID, string CampaignID, string sMessageID, int commsInstanceID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDBO BOD = new XIDBO();
            XIDefinitionBase oXID = new XIDefinitionBase();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "saving Email details in XLeadCampaign table";//expalin about this method logic
            try
            {
                if (!string.IsNullOrEmpty(leadID) && !string.IsNullOrEmpty(CampaignID) && !string.IsNullOrEmpty(sMessageID) && commsInstanceID > 0)//check mandatory params are passed or not
                {
                    BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XLeadCampaign");
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("fkileadid", leadID);
                    oBOI.SetAttribute("fkicampaignid", CampaignID);
                    oBOI.SetAttribute("sReference", sMessageID);
                    oBOI.SetAttribute("FKiCommInstanceID", commsInstanceID.ToString());
                    oBOI.SetAttribute("ileadcampaignstatus", ((int)EnumEmailStatus.processed).ToString());
                    //oBOI.SetAttribute("fkifunnelid", "");
                    oCResult.oResult = oBOI.Save(oBOI);
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: " + leadID + " or " + CampaignID + " or " + sMessageID + " or " + commsInstanceID + " is missing";
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
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public async Task<CResult> Send_Mail()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Call api to send send grid mail";//expalin about this method logic
            var oAccountD = new XIDAccount();
            if (!string.IsNullOrEmpty(sAccountID))
            {
                oAccountD = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, sAccountID.ToString());
            }
            // Your API endpoint URL
            string apiUrl = oAccountD.sAPIURL; //"https://platformfactory.in:8444/api/mail/send";
            // Your API key
            string apiKey = oAccountD.sSendgridKey;// "3719d531-ca35-415b-b343-d06469caa0ee";
            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Set the API key in the request headers
                client.DefaultRequestHeaders.Add("xi-api-secret", apiKey);
                try
                {
                    SendEmailRequest request = new SendEmailRequest();
                    request.ToEmails = new List<string>() { sTo };
                    request.Subject = sSubject;
                    request.PlainTextContent = sBody;
                    request.HtmlContent = sBody;
                    if(Attachments != null && Attachments.Count() > 0)
                    {
                        request.Attachments = Attachments;
                    }
                    string jsonData = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    // Make a GET request to the API
                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string resp = response.Content.ReadAsStringAsync().Result;
                        //return JsonConvert.DeserializeObject<BaseResponse>(resp);
                        var oResponse = JsonConvert.DeserializeObject<BaseResponse>(resp);
                        oCResult.oResult = oResponse;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        return oCResult;
                    }
                    else
                    {
                        string resp = response.Content.ReadAsStringAsync().Result;
                        BaseResponse oResponse = JsonConvert.DeserializeObject<BaseResponse>(resp);
                        oCResult.oResult = oResponse;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        return oCResult;
                    }
                }
                catch (HttpRequestException ex)
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    return oCResult;
                }
            }
        }
    }

    public class BaseResponse
    {
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        public MessageInfo1 Data { get; set; }
    }

    public class MessageInfo1
    {
        public string X_Message_Id { get; set; }
    }

    public class SendEmailRequest
    {
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
        public List<string> CcEmails { get; set; }
        public List<string> BccEmails { get; set; }
        public List<AttachmentSG> Attachments { get; set; }
        public string TemplateId { get; set; }
    }
    public class AttachmentSG
    {
        public string FileName { get; set; }   //file name
        public string Content { get; set; }    // base64 string content
    }
}