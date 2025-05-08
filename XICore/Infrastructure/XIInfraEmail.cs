//using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;
using iTextSharp.tool.xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.Services.Description;
using iTextSharp.text.pdf.events;
using System.Globalization;
using System.Configuration;
using IronPdf;
using System.Web.UI.WebControls;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using System.Text;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf.parser;
//using Syncfusion.Pdf;
//using Syncfusion.HtmlConverter;
using System.Web.Mvc;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

namespace XICore
{
    public class XIInfraEmail : XIInstanceBase
    {
        public string sOrgDatabase { get; set; }
        public string sCoreDataBase { get; set; }
        public string EmailID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string SMTP { get; set; }
        public string UserName { get; set; }
        public string sSubject { get; set; }
        public string Bcc { get; set; }
        public string cc { get; set; }
        public string From { get; set; }

        public string sSurName { get; set; }
        public string sDOB { get; set; }
        public string sStage { get; set; }
        public string sDocumentName { get; set; }
        public int iServerID { get; set; }
        int Port = 0;
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        XIInfraScript oIScript = new XIInfraScript();
        XIIXI oIXI = new XIIXI();

        public CResult Sendmail(int iOrgID, string sContent, List<Attachment> oAttachments, long iCustomerID = 0, string sContext = "", int iLeadID = 0, List<string> sAttachementPath = null, long iPolicyID = 0, bool bIsBCCOnly = false, bool bIsNotes = false, string sNotes = "", string sDocIDs = "", int iServerID = 0)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            //sAttachementPath = new List<string>();
            //sAttachementPath.Add(@"E:\New folder\TFS\XIDNAOM\XIDNA_PreLive_Branch_17_02_2020-branch\XIDNA\Documents\BordereauFiles\20-Aug-2020\KGM.csv");
            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            string Messagebody = string.Empty;
            string sMailInfo = string.Empty;
            List<CNV> oAuditParams = new List<CNV>();
            int iAuditID = 0;
            var sDefaultEmailC = string.Empty;
            var sClientEmailC = string.Empty;
            var sOverAllEmailC = string.Empty;
            try
            {
                oTrace.sClass = "XIInfraEmail";
                oTrace.sMethod = "Sendmail";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                if (iPolicyID > 0)
                {
                    var oPolicyI = oIXI.BOI("ACPolicy_T", iPolicyID.ToString(), "Create");
                    if (oPolicyI != null)
                    {
                        if (oPolicyI.Attributes.ContainsKey("FkiProductID"))
                        {
                            var ProductID = oPolicyI.Attributes["FkiProductID"].sValue;
                            if (!string.IsNullOrEmpty(ProductID))
                            {
                                var oProductI = oIXI.BOI("Product", ProductID);
                                if (oProductI != null && oProductI.Attributes.ContainsKey("bIsEmail"))
                                {
                                    sDefaultEmailC = oProductI.Attributes["bIsEmail"].sValue;
                                }
                            }
                        }
                        if (oPolicyI.Attributes.ContainsKey("bIsClientEmail"))
                        {
                            sClientEmailC = oPolicyI.Attributes["bIsClientEmail"].sValue;
                        }
                    }
                    oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iPolicyID.ToString() });
                    if (bIsNotes)
                    {
                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sNotes + "mail" + " Processing.." });
                    }
                    else
                    {
                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sSubject + "mail" + " Processing.." });
                    }
                    var oARes = Audit_Policy(oAuditParams);
                    var iAuditInsID = oARes.oResult;
                    int.TryParse(iAuditInsID.ToString(), out iAuditID);
                }
                sStage = "Mail sending initiated";
                XIInfraEncryption xiinfra = new XIInfraEncryption();
                XIInfraEmail oXIMail = new XIInfraEmail();
                string Mailbody = "";
                //Getting Server Details
                XIDXI oXID = new XIDXI();
                XIInfraCache oCache = new XIInfraCache();
                var ServerDetails = (XIIOServerDetails)oCache.GetObjectFromCache(XIConstant.CacheIOServer, iServerID.ToString(), iOrgID.ToString()); //oXID.Get_IOSServerDetails(iOrgID);
                XIIXI oXII = new XIIXI();
                List<CNV> oNV = new List<CNV>();
                oNV.Add(new CNV { sName = "sCode", sValue = sContext });
                var oMailI = oXII.BOI("MailConfig_T", "", "", oNV);
                bool IsSendMail = true;
                if (oMailI != null && oMailI.Attributes.ContainsKey("iStatus"))
                {
                    var iStatus = oMailI.Attributes["iStatus"].sValue;
                    if (!string.IsNullOrEmpty(iStatus) && iStatus == "10")
                    {
                        IsSendMail = false;
                        sStage = "Mail sending Disabled to Customer: " + iCustomerID + "- Email: " + EmailID + " - and Context: " + sContext;
                    }
                }
                sOverAllEmailC = IsSendMail.ToString();
                bool bIsHavingAttachment = false;
                if ((oAttachments != null && oAttachments.Count() > 0) || sAttachementPath != null)
                {
                    bIsHavingAttachment = true;
                }
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "MergeEmailContent_T", null);
                oBOI.BOD = oBOD;
                oBOI.LoadBOI("create");
                if (oBOI.Attributes.ContainsKey("scontent"))
                {
                    oBOI.Attributes["scontent"].sValue = @sContent;
                }
                if (oBOI.Attributes.ContainsKey("semail"))
                {
                    oBOI.Attributes["semail"].sValue = EmailID;
                }
                if (oBOI.Attributes.ContainsKey("ssubject"))
                {
                    oBOI.Attributes["ssubject"].sValue = sSubject;
                }
                if (oBOI.Attributes.ContainsKey("istatus"))
                {
                    oBOI.Attributes["istatus"].sValue = "0";
                }
                if (oBOI.Attributes.ContainsKey("fkiorgid"))
                {
                    oBOI.Attributes["fkiorgid"].sValue = iOrgID.ToString();
                }
                if (oBOI.Attributes.ContainsKey("bishavingattachments"))
                {
                    oBOI.Attributes["bishavingattachments"].sValue = bIsHavingAttachment.ToString();
                }
                if (oBOI.Attributes.ContainsKey("fkizxdoc"))
                {
                    oBOI.Attributes["fkizxdoc"].sValue = sDocIDs;
                }
                oBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                var oResult = oBOI.Save(oBOI);
                var iMergeMailID = string.Empty;
                if (oResult.bOK && oResult.oResult != null)
                {
                    var oMergeMailI = (XIIBO)oResult.oResult;
                    iMergeMailID = oMergeMailI.Attributes["id"].sValue;
                }
                if (oAttachments != null && oAttachments.Count() > 0)
                {
                    foreach (var Attachment in oAttachments)
                    {
                        XIIBO oMergeBOI = new XIIBO();
                        oMergeBOI.BOD = oBOD;
                        oMergeBOI.LoadBOI("create");
                        if (oMergeBOI.Attributes.ContainsKey("scontent"))
                        {
                            StreamReader oReader = new StreamReader(Attachment.ContentStream);
                            oMergeBOI.Attributes["scontent"].sValue = oReader.ReadToEnd();
                        }
                        if (oMergeBOI.Attributes.ContainsKey("semail"))
                        {
                            oMergeBOI.Attributes["semail"].sValue = EmailID;
                        }
                        if (oMergeBOI.Attributes.ContainsKey("ssubject"))
                        {
                            oMergeBOI.Attributes["ssubject"].sValue = Attachment.Name;
                        }
                        if (oMergeBOI.Attributes.ContainsKey("istatus"))
                        {
                            oMergeBOI.Attributes["istatus"].sValue = "0";
                        }
                        if (oMergeBOI.Attributes.ContainsKey("fkiorgid"))
                        {
                            oMergeBOI.Attributes["fkiorgid"].sValue = iOrgID.ToString();
                        }
                        if (oMergeBOI.Attributes.ContainsKey("iparentid"))
                        {
                            oMergeBOI.Attributes["iparentid"].sValue = iMergeMailID;
                        }
                        oMergeBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                        oMergeBOI.Save(oMergeBOI);
                    }
                }

                XIIBO oCallBOI = new XIIBO();
                XIDBO oCallBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Call_T", null);
                oCallBOI.BOD = oCallBOD;
                oCallBOI.LoadBOI("CreateEmail");
                if (oCallBOI.Attributes.ContainsKey("istatus"))
                {
                    oCallBOI.Attributes["istatus"].sValue = "10";
                }
                if (oCallBOI.Attributes.ContainsKey("itype"))
                {
                    oCallBOI.Attributes["itype"].sValue = "20";
                }
                if (oCallBOI.Attributes.ContainsKey("sname"))
                {
                    oCallBOI.Attributes["sname"].sValue = sSubject;
                }
                if (oCallBOI.Attributes.ContainsKey("snotes"))
                {
                    oCallBOI.Attributes["snotes"].sValue = "Processing";
                }
                if (oCallBOI.Attributes.ContainsKey("fkileadid"))
                {
                    oCallBOI.Attributes["fkileadid"].sValue = iLeadID.ToString();
                }
                if (oCallBOI.Attributes.ContainsKey("fkiacpolicyid"))
                {
                    oCallBOI.Attributes["fkiacpolicyid"].sValue = iPolicyID.ToString();
                }
                if (oCallBOI.Attributes.ContainsKey("fkiorgid"))
                {
                    oCallBOI.Attributes["fkiorgid"].sValue = iOrgID.ToString();
                }
                if (oCallBOI.Attributes.ContainsKey("fkimergeemailcontentid"))
                {
                    oCallBOI.Attributes["fkimergeemailcontentid"].sValue = iMergeMailID;
                }
                oCallBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                oCallBOI.Save(oCallBOI);

                if (ServerDetails != null && IsSendMail)
                {
                    XIIOServerDetails oServerDetail = ServerDetails;
                    MailMessage mail = new MailMessage();

                    if (!string.IsNullOrEmpty(From))
                    {
                        mail.From = new MailAddress(From);
                    }
                    else
                    {
                        mail.From = new MailAddress(oServerDetail.FromAddress);
                    }
                    oTrace.oParams.Add(new CNV { sName = "From", sValue = mail.From.Address });
                    if (!bIsBCCOnly)
                    {
                        mail.To.Add(EmailID);

                        oTrace.oParams.Add(new CNV { sName = "From", sValue = EmailID });
                        if (!string.IsNullOrEmpty(cc))
                        {
                            var oCCList = cc.Split(',');
                            foreach (var oCC in oCCList)
                            {
                                mail.CC.Add(oCC);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(Bcc))
                    {
                        var oBccList = Bcc.Split(',');
                        foreach (var oBCC in oBccList)
                        {
                            mail.Bcc.Add(oBCC);
                        }
                    }
                    if (sContent == null)
                    {
                        mail.Subject = "Zeeinsurance Password reset link";

                        //Encrypting Password with Email and Current Date Time.
                        string EncryptedHash = xiinfra.EncryptData(EmailID + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), true, "URP");
                        Messagebody = "http://localhost:53996/Account/ResetPasswordLink?key=" + EncryptedHash;
                        string html = @"<html><body>" + Messagebody + "</body></html>";
                        Mailbody = @"<html><body><p>";
                        Mailbody += " Hi <b>" + UserName + "</b>" + "," + "<p>We have receieved request from you to reset your login password.</p> ";
                        Mailbody += "<p> Please <a href = " + Messagebody + " style = 'text - decoration:underline; ' > Click here </a> Here to reset your password or use the link below:</p>";
                        Mailbody += "<br/ ><a>Temporary link: <a href = " + Messagebody + " > " + Messagebody + " </a></a>";
                        Mailbody += "<p> This link was expired in 24 hours. </p><span><br />If you have any queries, please contact: <a href = 'https://Zeeinsurance.co.uk/' > SystemsDNA@zeeinsurance.co.uk </a></span><br /><br /><span> Kind regards,<br />Admin <br /> Zeeinsurance </span></p></body></html >";
                    }
                    else
                    {
                        //mail.Subject = "Certificate of your policy";
                        mail.Subject = sSubject;
                        Mailbody = @sContent;
                    }
                    sMailInfo = "To: " + EmailID + " - Subject: " + mail.Subject;
                    AlternateView altView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
                    mail.AlternateViews.Add(altView);
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = oServerDetail.ServerName;
                    smtp.Port = oServerDetail.Port;
                    smtp.EnableSsl = false;
                    //Chcecking Server Credentials
                    smtp.Credentials = new System.Net.NetworkCredential(oServerDetail.UserName, oServerDetail.Password);
                    if (oAttachments != null && oAttachments.Count() > 0)
                    {
                        foreach (var attachment in oAttachments)
                        {
                            mail.Attachments.Add(attachment);
                        }
                    }
                    if (sAttachementPath != null && sAttachementPath.Count > 0)
                    {
                        foreach (var attachement in sAttachementPath)
                        {
                            mail.Attachments.Add(new Attachment(attachement));
                        }
                    }
                    oTrace.oParams.Add(new CNV { sName = "AttachmentCount", sValue = mail.Attachments.Count().ToString() });
                    oCResult.sCode = "Email";
                    smtp.Send(mail); //Mail Sending
                    oCResult.sMessage = "Mail Sent Successfully to Customer: " + iCustomerID + " - and Context: " + sContext + " - Mail Info: " + sMailInfo;
                    sStage = "Mail sending completed";
                    var oMergeI = oIXI.BOI("MergeEmailContent_T", iMergeMailID);
                    if (oMergeI != null && oMergeI.Attributes.ContainsKey("iStatus"))
                    {
                        oMergeI.Attributes["iStatus"].sValue = "10";
                        oMergeI.Attributes["iStatus"].bDirty = true;
                        if (oMergeI.Attributes.ContainsKey("sNotes") && oMailI != null && oMailI.Attributes.ContainsKey("sSuccessMessage"))
                        {
                            oMergeI.Attributes["sNotes"].sValue = oMailI.Attributes["sSuccessMessage"].sValue;
                            oMergeI.Attributes["sNotes"].bDirty = true;
                        }
                        oMergeI.Save(oMergeI);
                    }
                    if (oCallBOI.Attributes.ContainsKey("snotes") && oMailI != null && oMailI.Attributes.ContainsKey("sSuccessMessage"))
                    {
                        if (bIsNotes)
                        {
                            oCallBOI.Attributes["snotes"].sValue = sNotes + " mail sent successfully to " + EmailID;
                        }
                        else
                        {
                            oCallBOI.Attributes["snotes"].sValue = oMailI.Attributes["sSuccessMessage"].sValue + " to " + EmailID;
                        }
                        oCallBOI.Attributes["snotes"].bDirty = true;
                        oCallBOI.Save(oCallBOI);
                    }
                    if (iPolicyID > 0)
                    {
                        oAuditParams = new List<CNV>();
                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iPolicyID.ToString() });
                        if (oMailI.Attributes.ContainsKey("bIsOnActivity") && oMailI.Attributes["bIsOnActivity"].sValue == "True")
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sSubject /*+ " mail sent Successsfully to " + EmailID */+ "<br/>" + "Default email configuration on product : " + sDefaultEmailC + "<br/>" + "Send email checkbox checked on questionset  : " + sClientEmailC + "<br/>" + "Client email status : " + sOverAllEmailC + "<br/>" + oMailI.Attributes["sSuccessMessage"].sValue });
                        }
                        else if (bIsNotes)
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sNotes + " " + oMailI.Attributes["sSuccessMessage"].sValue });
                        }
                        else
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = oMailI.Attributes["sSuccessMessage"].sValue });
                        }
                        oAuditParams.Add(new CNV { sName = "iAuditID", sValue = iAuditID.ToString() });
                        Audit_Policy(oAuditParams);
                    }
                    SaveErrortoDB(oCResult);
                }
                else
                {
                    var oMergeI = oIXI.BOI("MergeEmailContent_T", iMergeMailID);
                    if (oMergeI != null && oMergeI.Attributes.ContainsKey("iStatus"))
                    {
                        oMergeI.Attributes["iStatus"].sValue = "20";
                        oMergeI.Attributes["iStatus"].bDirty = true;
                        if (oMergeI.Attributes.ContainsKey("sNotes") && oMailI != null && oMailI.Attributes.ContainsKey("sFailedMessage"))
                        {
                            oMergeI.Attributes["sNotes"].sValue = oMailI.Attributes["sFailedMessage"].sValue;
                            oMergeI.Attributes["sNotes"].bDirty = true;
                        }
                        oMergeI.Save(oMergeI);
                    }
                    if (oCallBOI.Attributes.ContainsKey("snotes") && oMailI != null && oMailI.Attributes.ContainsKey("sFailedMessage"))
                    {
                        oCallBOI.Attributes["snotes"].sValue = oMailI.Attributes["sFailedMessage"].sValue + " to " + EmailID;
                        oCallBOI.Attributes["snotes"].bDirty = true;
                        oCallBOI.Save(oCallBOI);
                    }
                    if (iPolicyID > 0)
                    {
                        oAuditParams = new List<CNV>();
                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iPolicyID.ToString() });
                        if (oMailI.Attributes.ContainsKey("bIsOnActivity") && oMailI.Attributes["bIsOnActivity"].sValue == "True")
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sSubject /*+ " mail sent Successsfully to " + EmailID */+ "<br/>" + "Default email configuration on product : " + sDefaultEmailC + "<br/>" + "Send email checkbox checked on questionset  : " + sClientEmailC + "<br/>" + "Client email status : " + sOverAllEmailC + "<br/>" + oMailI.Attributes["sFailedMessage"].sValue });
                        }
                        else if (bIsNotes)
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = sNotes + " " + oMailI.Attributes["sSuccessMessage"].sValue });
                        }
                        else
                        {
                            oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = oMailI.Attributes["sFailedMessage"].sValue });
                        }
                        oAuditParams.Add(new CNV { sName = "iAuditID", sValue = iAuditID.ToString() });
                        Audit_Policy(oAuditParams);
                    }
                }
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oCResult;
            }
            catch (Exception ex)
            {
                sStage = "Mail sending Failed to Customer: " + iCustomerID + " - and Context: " + sContext;
                oCResult.sCode = "Email";
                oCResult.sMessage = sStage + " - Mail Info: " + sMailInfo;
                SaveErrortoDB(oCResult);
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult; // always
        }
        public CResult Audit_Policy(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

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
            int iPolicyID = 0;
            try
            {
                var iInstanceID = 0;
                string iACPolicyID = oParams.Where(m => m.sName.ToLower() == "iACPolicyID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (int.TryParse(iACPolicyID, out iPolicyID))
                { }
                string iInsID = oParams.Where(m => m.sName.ToLower() == "iAuditID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sInfo = oParams.Where(m => m.sName.ToLower() == "sAuditInfo".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sPolicyVersionID = oParams.Where(m => m.sName.ToLower() == "FKiPolicyVersionID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIIBO oBOI = new XIIBO();
                XIInfraCache oCache = new XIInfraCache();
                oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "Audit_T", null);
                //oBOICopy.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, oBOInstance.BOD.sAuditBOName);
                if (iInsID != null)
                {
                    int.TryParse(iInsID, out iInstanceID);
                }
                oBOI.SetAttribute(oBOI.BOD.sPrimaryKey, iInstanceID.ToString());
                oBOI.SetAttribute("FKiBOID", "17");
                oBOI.SetAttribute("sBOName", "ACPolicy_T");
                oBOI.SetAttribute("sData", sInfo);
                oBOI.SetAttribute("sOldData", sInfo);
                //oAuditBOI.SetAttribute("XICreatedBy", "");
                //oAuditBOI.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                oBOI.SetAttribute("sType", "Info");
                oBOI.SetAttribute("sActivity", "Info");
                oBOI.SetAttribute("FKiInstanceID", iACPolicyID);
                oBOI.SetAttribute("FKiPolicyVersionID", sPolicyVersionID);
                var oAuditBOResponse = oBOI.Save(oBOI);
                if (oAuditBOResponse.bOK && oAuditBOResponse.oResult != null)
                {
                    var oRes = (XIIBO)oAuditBOResponse.oResult;
                    var iID = oRes.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).FirstOrDefault();
                    if (iID != null)
                    {
                        int.TryParse(iID.sValue, out iInstanceID);
                    }
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = iInstanceID;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing MTAAddon" });
                oCResult.sMessage = "ERROR: [PolicyID: " + iPolicyID + " " + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult GeneratePDFFile(MemoryStream MStream)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                oTrace.sClass = "XIInfraEmail";
                oTrace.sMethod = "GeneratePDFFile";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                MemoryStream file = MStream;
                file = new MemoryStream(file.ToArray());
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                file.Seek(0, SeekOrigin.Begin);
                Attachment data = new Attachment(file, sDocumentName + ".pdf", "application/pdf");
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.DateTime.Now;
                disposition.ModificationDate = System.DateTime.Now;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                oCResult.oResult = data;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult IronPdf(string message, string sCSSFileName, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            try
            {
                oTrace.sClass = "XIInfraEmail";
                oTrace.sMethod = "IronPdf";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                MemoryStream output = new MemoryStream();
                HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = string.Empty;
                if (!string.IsNullOrEmpty(sCSSFileName))
                {
                    sPath = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\css\\DocumentCSS\\" + sCSSFileName + ".css";
                    Renderer.PrintOptions.CustomCssUrl = new Uri(sPath);
                }
                Renderer.PrintOptions.CssMediaType =
PdfPrintOptions.PdfCssMediaType.Print;
                //Renderer.RenderHtmlAsPdf(message).SaveAs("D:\\Test1.pdf");
                //PdfResource myPdf = Renderer.RenderHtmlAsPdf(message);
                var myPdf = Renderer.RenderHtmlAsPdf(message);
                MemoryStream memoryStream = myPdf.Stream;
                byte[] bytes = memoryStream.ToArray();
                string sSurNamePW = string.Empty; string sDOBPW = string.Empty;
                if (bIsPaswordProtected)
                {
                    if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                    {
                        if (!string.IsNullOrEmpty(sSurName))
                        {
                            int iStartPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[0]);
                            int iEndPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[1]);
                            if (sSurName.Length >= iEndPos)
                            {
                                sSurNamePW = sSurName.Substring(iStartPos, iEndPos);
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(sDOBPasswordRange))
                    {
                        if (!string.IsNullOrEmpty(sDOB))
                        {
                            if (sDOBPasswordRange == "yyyy")
                            {
                                XIIBO oBOI = new XIIBO();
                                DateTime dDate = oBOI.ConvertToDtTime(sDOB);
                                sDOBPW = dDate.Year.ToString();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sSurNamePW) && !string.IsNullOrEmpty(sDOBPW))
                    {
                        using (MemoryStream input = new MemoryStream(bytes))
                        {
                            string password = sSurNamePW + sDOBPW;
                            PdfReader reader = new PdfReader(input);
                            PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                            bytes = output.ToArray();
                        }
                    }
                    else
                    {
                        output = memoryStream;
                    }
                }
                else
                {
                    output = memoryStream;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = output;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                //System.IO.MemoryStream stream = HtmlToPdf.RenderHtmlAsPdf(message).Stream;
                //byte[] data = HtmlToPdf.RenderHtmlAsPdf(message).BinaryData;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult PDFGenerate(string message, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                oTrace.sClass = "XIInfraEmail";
                oTrace.sMethod = "PDFGenerate";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                MemoryStream output = new MemoryStream();
                StringReader sr = new StringReader(message);
                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                List<string> cssFiles = new List<string>();
                cssFiles.Add(@"~/Content/css/pdfSendStyle.css");
                //PdfPTable table = new PdfPTable(columnWidths);
                //table.WidthPercentage = 100f;
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //string watermark = "Test";

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    //PdfPTable pdfPTable = new PdfPTable(7);
                    //pdfPTable.SetExtendLastRow(true, true);
                    //PageHandler oPageHandler = new PageHandler();
                    //writer.PageEvent = oPageHandler.OnEndPage(writer, pdfDoc);
                    //writer.PageEvent = oPageHandler;
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    //htmlparser.Parse(sr);

                    pdfDoc.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    string sSurNamePW = string.Empty; string sDOBPW = string.Empty;
                    if (bIsPaswordProtected)
                    {
                        if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                        {
                            if (!string.IsNullOrEmpty(sSurName))
                            {
                                int iStartPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[0]);
                                int iEndPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[1]);
                                if (sSurName.Length >= iEndPos)
                                {
                                    sSurNamePW = sSurName.Substring(iStartPos, iEndPos);
                                }

                            }
                        }
                        if (!string.IsNullOrEmpty(sDOBPasswordRange))
                        {
                            if (!string.IsNullOrEmpty(sDOB))
                            {
                                if (sDOBPasswordRange == "yyyy")
                                {
                                    XIIBO oBOI = new XIIBO();
                                    DateTime dDate = oBOI.ConvertToDtTime(sDOB);
                                    sDOBPW = dDate.Year.ToString();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(sSurNamePW) && !string.IsNullOrEmpty(sDOBPW))
                        {
                            using (MemoryStream input = new MemoryStream(bytes))
                            {
                                //using (MemoryStream soutput = new MemoryStream())
                                //{
                                string password = sSurNamePW + sDOBPW;
                                PdfReader reader = new PdfReader(input);
                                PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                                bytes = output.ToArray();
                                //}
                            }
                        }
                        else
                        {
                            output = memoryStream;
                        }
                    }
                    else
                    {
                        output = memoryStream;
                    }
                    //Response.Clear();
                    //output = memoryStream;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = output;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public string GetTextFromPDF(string sPath, string DocID)
        {
            XIInfraCache oCache = new XIInfraCache();
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(sPath))
            {
                List<CNV> WhrParams = new List<CNV>();
                WhrParams.Add(new CNV { sName = "id", sValue = DocID });
                XIIXI oXII = new XIIXI();
                var oBOII = oXII.BOI("XIDocumentTree", null, null, WhrParams);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree");
                    XIIBO oBOIDoc = new XIIBO();
                    if (i == 1)
                    {
                        oBOII.SetAttribute("sPageNo", i.ToString());
                        oBOII.SetAttribute("stags", PdfTextExtractor.GetTextFromPage(reader, i));
                        oBOII.BOD = BOD;
                        var oCR = oBOII.Save(oBOII);
                    }
                    else
                    {
                        oBOIDoc.SetAttribute("sName", oBOII.AttributeI("sname").sValue);
                        oBOIDoc.SetAttribute("sPath", oBOII.AttributeI("sPath").sValue);
                        oBOIDoc.SetAttribute("sParentID", oBOII.AttributeI("sParentID").sValue);
                        oBOIDoc.SetAttribute("iBuildingID", oBOII.AttributeI("iBuildingID").sValue);
                        oBOIDoc.SetAttribute("sType", "20");
                        oBOIDoc.SetAttribute("sPageNo", i.ToString());
                        oBOIDoc.SetAttribute("sVersion", oBOII.AttributeI("sVersion").sValue);
                        oBOIDoc.SetAttribute("iVersionBatchID", oBOII.AttributeI("iVersionBatchID").sValue);
                        oBOIDoc.SetAttribute("iApprovalStatus", "20");
                        oBOIDoc.SetAttribute("stags", PdfTextExtractor.GetTextFromPage(reader, i));
                        oBOIDoc.BOD = BOD;
                        var oCR = oBOIDoc.Save(oBOIDoc);
                    }



                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }

            return text.ToString();
        }
        //public CResult SyncFusionPDF(string message, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        //{
        //    CResult oCResult = new CResult();
        //    CResult oCR = new CResult();
        //    var watch = System.Diagnostics.Stopwatch.StartNew();
        //    CTraceStack oTrace = new CTraceStack();
        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
        //    try
        //    {
        //        oTrace.sClass = "XIInfraEmail";
        //        oTrace.sMethod = "SyncFusionPDF";
        //        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
        //        MemoryStream output = new MemoryStream();
        //        HtmlToPdfConverter converter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
        //        WebKitConverterSettings settings = new WebKitConverterSettings();
        //        //convert header HTML and set its template to webkit settings
        //        //settings.PdfHeader = HeaderHTMLtoPDF();
        //        //convert footer HTML and set its template to webkit settings
        //        //settings.PdfFooter = FooterHTMLtoPDF();
        //        string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
        //        settings.WebKitPath = physicalPath.Substring(0, physicalPath.Length) + "\\QtBinaries";
        //        settings.PdfPageSize = PdfPageSize.A4;
        //        //settings.EnableJavaScript = true;
        //        //settings.EnableRepeatTableHeader = true;
        //        //settings.EnableRepeatTableFooter = true;
        //        settings.Margin = new Syncfusion.Pdf.Graphics.PdfMargins { Top = 30, Left = 20, Right = 20, Bottom = 40 };
        //        settings.WebKitViewPort = new Size(1024, 0);
        //        //settings.WebKitPath = @"/QtBinaries/"; //System.Configuration.ConfigurationManager.AppSettings["QtBinaries"];
        //        converter.ConverterSettings = settings;

        //        string htmlText = message;// "<html><body Align='Left'><br><p> <font size='12'>Hello World </p></font> </body></html>";
        //        string baseUrl = string.Empty;
        //        Syncfusion.Pdf.PdfDocument document = converter.Convert(htmlText, baseUrl);
        //        document.PageSettings.Size = PdfPageSize.A4;
        //        MemoryStream ms = new MemoryStream();

        //        document.Save(ms);
        //        document.Close(true);
        //        ms.Position = 0;
        //        byte[] bytes = ms.ToArray();
        //        string sSurNamePW = string.Empty; string sDOBPW = string.Empty;
        //        if (bIsPaswordProtected)
        //        {
        //            if (!string.IsNullOrEmpty(sSurNamePasswordRange))
        //            {
        //                if (!string.IsNullOrEmpty(sSurName))
        //                {
        //                    int iStartPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[0]);
        //                    int iEndPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[1]);
        //                    if (sSurName.Length >= iEndPos)
        //                    {
        //                        sSurNamePW = sSurName.Substring(iStartPos, iEndPos);
        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(sDOBPasswordRange))
        //            {
        //                if (!string.IsNullOrEmpty(sDOB))
        //                {
        //                    if (sDOBPasswordRange == "yyyy")
        //                    {
        //                        XIIBO oBOI = new XIIBO();
        //                        DateTime dDate = oBOI.ConvertToDtTime(sDOB);
        //                        sDOBPW = dDate.Year.ToString();
        //                    }
        //                }
        //            }
        //            if (!string.IsNullOrEmpty(sSurNamePW) && !string.IsNullOrEmpty(sDOBPW))
        //            {
        //                using (MemoryStream input = new MemoryStream(bytes))
        //                {
        //                    //using (MemoryStream soutput = new MemoryStream())
        //                    //{
        //                    string password = sSurNamePW + sDOBPW;
        //                    PdfReader reader = new PdfReader(input);
        //                    PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
        //                    bytes = output.ToArray();
        //                }
        //            }
        //            else
        //            {
        //                output = ms;
        //            }
        //        }
        //        else
        //        {
        //            output = ms;
        //        }
        //        oCResult.oResult = output;
        //    }
        //    catch (Exception ex)
        //    {
        //        oCResult.sMessage = "ERROR: SyncFusionPDF [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //        SaveErrortoDB(oCResult);
        //        oCResult.LogToFile();
        //    }
        //    watch.Stop();
        //    oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
        //    oCResult.oTrace = oTrace;
        //    return oCResult;
        //}
        //private static PdfPageTemplateElement HeaderHTMLtoPDF()
        //{
        //    HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
        //    WebKitConverterSettings webKitSettings = new WebKitConverterSettings();
        //    webKitSettings.PdfPageSize = new SizeF(PdfPageSize.A4.Width, 50);
        //    webKitSettings.Orientation = PdfPageOrientation.Landscape;
        //    //Set webkitpath
        //    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
        //    webKitSettings.WebKitPath = physicalPath.Substring(0, physicalPath.Length) + "\\QtBinaries";
        //    //webKitSettings.WebKitPath = @"../../QtBinaries";
        //    webKitSettings.WebKitViewPort = new Size(1024, 0);
        //    htmlConverter.ConverterSettings = webKitSettings;
        //    string url = physicalPath.Substring(0, physicalPath.Length) + "Views\\Docs\\header.html";
        //    //Convert URL to PDF
        //    Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(url);
        //    RectangleF bounds = new RectangleF(0, 0, document.Pages[0].GetClientSize().Width, 50);
        //    PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
        //    header.Graphics.DrawPdfTemplate(document.Pages[0].CreateTemplate(), bounds.Location, bounds.Size);
        //    return header;
        //}
        ////Convert footer HTML to PDF and get pdf page template element of the result
        //private static PdfPageTemplateElement FooterHTMLtoPDF()
        //{
        //    HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
        //    WebKitConverterSettings webKitSettings = new WebKitConverterSettings();
        //    webKitSettings.PdfPageSize = new SizeF(PdfPageSize.A4.Width, 25);
        //    webKitSettings.Orientation = PdfPageOrientation.Landscape;
        //    //Set webkitpath
        //    webKitSettings.WebKitPath = @"../../QtBinaries";
        //    webKitSettings.WebKitViewPort = new Size(1024, 0);
        //    htmlConverter.ConverterSettings = webKitSettings;
        //    string url = System.IO.Path.GetFullPath("../../Data/footer.html");
        //    //Convert URL to PDF
        //    Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(url);
        //    RectangleF bounds = new RectangleF(0, 0, document.Pages[0].GetClientSize().Width, 25);
        //    PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);
        //    footer.Graphics.DrawPdfTemplate(document.Pages[0].CreateTemplate(), bounds.Location, bounds.Size);
        //    return footer;
        //}
        public CResult ModificationPDFGenerate(string message, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        {
            CResult oCResult = new CResult();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                MemoryStream output = new MemoryStream();
                StringReader sr = new StringReader(message);
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    output = memoryStream;
                    oCResult.oResult = output;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }

        public CResult Send_SMTPMail(List<CNV> oParams, List<string> sAttachementPath=null)
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
                var AccountUID = oParams.Where(m => m.sName.ToLower() == "AccountUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                Guid AccountGUID = Guid.Empty;
                int iAccountID = 0;
                var sContent = oParams.Where(m => m.sName.ToLower() == "scontent").Select(m => m.sValue).FirstOrDefault();
                var sTo = oParams.Where(m => m.sName.ToLower() == "sto").Select(m => m.sValue).FirstOrDefault();
                var sFrom = oParams.Where(m => m.sName.ToLower() == "sfrom").Select(m => m.sValue).FirstOrDefault();
                var sSubject = oParams.Where(m => m.sName.ToLower() == "sSubject".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var sCC = oParams.Where(m => m.sName.ToLower() == "scc").Select(m => m.sValue).FirstOrDefault();
                var sBCC = oParams.Where(m => m.sName.ToLower() == "sbcc").Select(m => m.sValue).FirstOrDefault();
                oTrace.oParams.Add(new CNV { sName = "AccountUID", sValue = AccountUID });
                oTrace.oParams.Add(new CNV { sName = "sContent", sValue = sContent });
                oTrace.oParams.Add(new CNV { sName = "sTo", sValue = sTo });
                oTrace.oParams.Add(new CNV { sName = "sFrom", sValue = sFrom });
                if ((int.TryParse(AccountUID, out iAccountID) ||( Guid.TryParse(AccountUID, out AccountGUID))) && !string.IsNullOrEmpty(sTo) && !string.IsNullOrEmpty(sContent))
                {
                    XIDAccount oAccountD = (XIDAccount)oCache.GetObjectFromCache(XIConstant.CacheXIAccount, null, AccountUID);
                    string Mailbody = string.Empty;
                    //List<string> sAttachementPath = new List<string>();
                    List<Attachment> oAttachments = new List<Attachment>();
                    MailMessage mail = new MailMessage();
                    if (!string.IsNullOrEmpty(From))
                    {
                        mail.From = new MailAddress(From);
                    }
                    else
                    {
                        mail.From = new MailAddress(oAccountD.sFromAddress);
                    }
                    mail.To.Add(sTo);
                    if (!string.IsNullOrEmpty(sCC))
                    {
                        var oCCList = sCC.Split(',');
                        foreach (var oCC in oCCList)
                        {
                            mail.CC.Add(oCC);
                        }
                    }
                    if (!string.IsNullOrEmpty(sBCC))
                    {
                        var oBccList = sBCC.Split(',');
                        foreach (var oBCC in oBccList)
                        {
                            mail.Bcc.Add(oBCC);
                        }
                    }
                    mail.Subject = sSubject;
                    @sContent = @sContent.Replace("\r\n", "<br>");
                    Mailbody = @sContent;
                    AlternateView altView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
                    mail.AlternateViews.Add(altView);
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = oAccountD.sName;
                    smtp.Port = oAccountD.iPort;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = true;
                    //Chcecking Server Credentials
                    smtp.Credentials = new System.Net.NetworkCredential(oAccountD.sUserName, oAccountD.sPassword);
                    if (oAttachments != null && oAttachments.Count() > 0)
                    {
                        foreach (var attachment in oAttachments)
                        {
                            mail.Attachments.Add(attachment);
                        }
                    }
                    if (sAttachementPath != null && sAttachementPath.Count > 0)
                    {
                        foreach (var attachement in sAttachementPath)
                        {
                            mail.Attachments.Add(new Attachment(attachement));
                        }
                    }
                    smtp.Send(mail);
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = "Mail sent successfully";
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: AccountUID :" + AccountUID + " or sTo:" + sTo + " or sFrom:" + sFrom + " or sContent:" + sContent.Substring(0, 16) + " is missing";
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
                SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
    //    public class PageHandler: PdfPageEventHelper
    //    {
    //        Font ffont = new Font(Font.FontFamily.UNDEFINED, 5, Font.ITALIC);

    //    public override void OnEndPage(PdfWriter writer, Document document)
    //    {
    //        PdfContentByte cb = writer.DirectContent;
    //        Phrase header = new Phrase("this is a header", ffont);
    //        Phrase footer = new Phrase("this is a footer", ffont);
    //        ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
    //                header,
    //                (document.Right - document.Left) / 2 + document.LeftMargin,
    //                document.Top + 10, 0);
    //        ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER,
    //                footer,
    //                (document.Right - document.Left) / 2 + document.LeftMargin,
    //                document.Bottom - 200, 0);
    //    }
    //}
}