using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;
using System.Net.Mail;
using System.IO;
using XIDatabase;
using System.Configuration;
namespace XICore
{
    public class XIInfraDocumentComponent : XIDefinitionBase
    {
        public long iInstanceID;
        public string sDocumentType { get; set; }
        public string sBOName { get; set; }
        public string sStructureName { get; set; }
        public int iDocumentID { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCoreDatabase { get; set; }
        public int iUserID { get; set; }
        public int iOrgID { get; set; }
        public string sOutput { get; set; }
        public string sSubject { get; set; }
        public int iAddonID { get; set; }
        public string sContext { get; set; }
        public string sSubContext { get; set; }
        public string sStructure { get; set; }
        public XIBOInstance oXIBOInstance { get; set; }

        XIInfraCache oCache = new XIInfraCache();
        XIContentEditors oXIContent = new XIContentEditors();
        XIInfraDocs oXIDocs = new XIInfraDocs();
        XIDXI oXIDX = new XIDXI();
        XIInfraNotifications oNotifications = new XIInfraNotifications();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            List<CNV> oNVPairsList = new List<CNV>();
            CNV oNVPairs = new CNV();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                XIInfraEmail oEmail = new XIInfraEmail();
                XIIQS oXIQS = new XIIQS();
                XIIXI oIXI = new XIIXI();
                int iType = 0;
                object oStructureresult = ""; int iProductID = 0; string sEmail = string.Empty; string sUserID = string.Empty; int iBOID = 0; string sProductName = string.Empty;
                string sCustomerReference = string.Empty; string sClassName = string.Empty;
                string sFKiACPolicyVersionID = string.Empty;
                string bIsDocumentCopy = "";
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sBOName = oParams.Where(m => m.sName == "Object").Select(m => m.sValue).FirstOrDefault();
                sDocumentType = oParams.Where(m => m.sName == "Document Type").Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName == "Structure Name").Select(m => m.sValue).FirstOrDefault();
                iDocumentID = Convert.ToInt32(oParams.Where(m => m.sName == "DocumentID").Select(m => m.sValue).FirstOrDefault());
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                iOrgID = Convert.ToInt32(oParams.Where(m => m.sName == "iOrganizationID").Select(m => m.sValue).FirstOrDefault());
                sOutput = oParams.Where(m => m.sName == "Output").Select(m => m.sValue).FirstOrDefault();
                sSubject = oParams.Where(m => m.sName == "Subject").Select(m => m.sValue).FirstOrDefault();
                iAddonID = Convert.ToInt32(oParams.Where(m => m.sName == "iAddonID").Select(m => m.sValue).FirstOrDefault());
                sFKiACPolicyVersionID = oParams.Where(m => m.sName == "FKiPolicyVersionID").Select(m => m.sValue).FirstOrDefault();
                sContext= oParams.Where(m => m.sName == "sContext").Select(m => m.sValue).FirstOrDefault();
                sSubContext = oParams.Where(m => m.sName == "sSubContext").Select(m => m.sValue).FirstOrDefault();
                sStructure = oParams.Where(m => m.sName == "sStructure").Select(m => m.sValue).FirstOrDefault();
                string sWhenGenerate = oParams.Where(m => m.sName == "sWhenGenerate").Select(m => m.sValue).FirstOrDefault();
                bIsDocumentCopy = oParams.Where(m => m.sName == "bIsDocumentCopy").Select(m => m.sValue).FirstOrDefault();
                string bIsOnCover = oParams.Where(m => m.sName == "bIsOnCover").Select(m => m.sValue).FirstOrDefault();
                List<CNV> oCNVParams = new List<CNV>();
                var UserID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|FKiUserID}");
                var LeadID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iLeadID}");
                DateTime dEffectiveFrom = DateTime.MinValue;
                int iLeadID = 0;
                if (int.TryParse(LeadID, out iLeadID))
                { }
                int iPolicyVersionID = 0;
                if (int.TryParse(sFKiACPolicyVersionID, out iPolicyVersionID))
                { }
                var oPolicyVersionI = oIXI.BOI("ACPolicyVersion_T", iPolicyVersionID.ToString());
                if (oPolicyVersionI != null && oPolicyVersionI.Attributes.Count > 0)
                {
                    if (oPolicyVersionI.Attributes.ContainsKey("FkiACPolicyID") && !string.IsNullOrEmpty(oPolicyVersionI.Attributes["FkiACPolicyID"].sValue))
                    {
                        var oPolicyI = oIXI.BOI("ACPolicy_T", oPolicyVersionI.Attributes["FkiACPolicyID"].sValue,"Create");
                        if(oPolicyI != null && oPolicyI.Attributes.ContainsKey("dCurrentPolicyOnCover"))
                        {
                            DateTime.TryParse(oPolicyI.Attributes["dCurrentPolicyOnCover"].sValue, out dEffectiveFrom);
                        }
                    }
                }
                if (iLeadID == 0)
                {
                    if (oPolicyVersionI != null && oPolicyVersionI.Attributes.ContainsKey("FKiQsInstanceID"))
                    {
                        List<CNV> oLeadNV = new List<CNV>();
                        oLeadNV.Add(new CNV { sName = "FKiQSInstanceID", sValue = oPolicyVersionI.Attributes["FKiQsInstanceID"].sValue });
                        var oLeadI = oIXI.BOI("Lead_T", "", "Create", oLeadNV);
                        if (oLeadI != null && oLeadI.Attributes.ContainsKey("id"))
                        {
                            if (int.TryParse(oLeadI.Attributes["id"].sValue, out iLeadID))
                            { }
                        }
                    }
                }
                int iUser = 0;
                if (int.TryParse(UserID, out iUser))
                { }
                //var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{-iInstanceID}");
                var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyID}");
                var sTransactionTypeID = string.Empty;
                var sPolicyNumber = string.Empty; string sPolicyHolderName = string.Empty; bool IsIndicativeProduct = false;
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> oNV = new List<CNV>();
                bool IsSendMail = true;
                oNV.Add(new CNV { sName = "FkiUserID", sValue = iUser.ToString() });
                long iCustomerID = 0;
                int iProductVersionID = 0;
                var oCustomerI = oIXI.BOI("Customer_T", "", "Create", oNV);
                if (oCustomerI != null && oCustomerI.Attributes != null && oCustomerI.Attributes.ContainsKey("bDisableClientLogin"))
                {
                    if (oCustomerI != null && oCustomerI.BOD != null)
                    {
                        if (oCustomerI.Attributes.ContainsKey(oCustomerI.BOD.sPrimaryKey))
                        {
                            var cusID = oCustomerI.Attributes[oCustomerI.BOD.sPrimaryKey].sValue;
                            long.TryParse(cusID, out iCustomerID);
                        }
                    }
                    var IsDisableClient = oCustomerI.Attributes["bDisableClientLogin"].sValue;
                    if (!string.IsNullOrEmpty(IsDisableClient) && IsDisableClient.ToLower() == "true")
                    {
                        IsSendMail = false;
                    }
                }
                long.TryParse(sInstanceID, out iInstanceID);
                if (iInstanceID == 0)
                {
                    iInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault());
                }
                var oDocContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sDocumentType, iDocumentID.ToString()); //oXIDX.Get_ContentDefinition(iDocumentID, sDocumentType);
                //var oInstance = oXIQS.QSI(oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).oSubStruct("Policy Version").oSubStruct("QS Instance"));
                //if(oContentDef.Category==1)
                var oDocTemplateList = oDocContent.GetCopy();
                var oContentDef = oDocTemplateList.FirstOrDefault();
                //{
                if (oContentDef != null && ((oContentDef.dActiveFrom == DateTime.MinValue && oContentDef.dActiveTo == DateTime.MinValue) || (oContentDef.dActiveFrom != DateTime.MinValue && oContentDef.dActiveTo != DateTime.MinValue && oContentDef.dActiveFrom <= dEffectiveFrom && oContentDef.dActiveTo >= dEffectiveFrom) || (oContentDef.dActiveFrom == DateTime.MinValue && oContentDef.dActiveTo != DateTime.MinValue && oContentDef.dActiveTo <= dEffectiveFrom) || (oContentDef.dActiveFrom <= dEffectiveFrom && oContentDef.dActiveFrom != DateTime.MinValue && oContentDef.dActiveTo == DateTime.MinValue)))
                {
                    int iMainTemplateID = oContentDef.ID;
                    var oLIst = oIXI.BOI(sBOName, iInstanceID.ToString());
                    iBOID = oLIst.BOD.BOID;
                    XIIBO oDocumentTemplate = new XIIBO();
                    bool IsclientEmail = true;
                    //if (string.IsNullOrEmpty(sSubject))
                    //{

                    if (oLIst.Attributes.ContainsKey("bIsClientEmail"))
                    {
                        string sclientEmail = oLIst.AttributeI("bIsClientEmail").sValue;
                        if (!string.IsNullOrEmpty(sclientEmail) && sclientEmail.ToLower() == "false")
                        {
                            IsclientEmail = false;
                        }

                    }
                    if (oLIst.Attributes.ContainsKey("sPolicyNo"))
                    {
                        sPolicyNumber = oLIst.AttributeI("sPolicyNo").sValue;

                    }
                    if (oLIst.Attributes.ContainsKey("sName"))
                    {
                        sPolicyHolderName = oLIst.AttributeI("sName").sValue;
                    }
                    if (oLIst != null && oLIst.Attributes.ContainsKey("FKiProductID"))
                    {
                        sEmail = oLIst.Attributes.Where(x => x.Key.ToLower() == "sEmail".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        sUserID = oLIst.Attributes.Where(x => x.Key.ToLower() == "FKiUserID".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        iProductID = Convert.ToInt32(oLIst.Attributes["FKiProductID"].sValue);
                        int.TryParse(oLIst.Attributes["FKiProductVersionID"].sValue, out iProductVersionID);
                        sProductName = oLIst.AttributeI("FKiProductID").ResolveFK("display");
                        sClassName = oLIst.AttributeI("FKsClass").sValue;
                        sCustomerReference = oLIst.AttributeI("FKiCustomerID").ResolveFK("display");
                        if (iProductID != 0)
                        {
                            List<CNV> oNVParams = new List<CNV>();
                            CNV oCNV = new CNV();
                            if (iAddonID != 0)
                            {
                                oCNV.sName = "FKiProductAddonID";
                                oCNV.sValue = iAddonID.ToString();
                                oNVParams.Add(oCNV);
                            }
                            else
                            {
                                oCNV.sName = "FKiProductID";
                                oCNV.sValue = iProductID.ToString();
                                oNVParams.Add(oCNV);
                            }
                            oCNV = new CNV();
                            oCNV.sName = "FKiTemplateID";
                            oCNV.sValue = iMainTemplateID.ToString();
                            oNVParams.Add(oCNV);
                            oDocumentTemplate = oIXI.BOI("DocumentTemplate", null, null, oNVParams);
                            if (oDocumentTemplate != null)
                            {
                                if (oDocumentTemplate.Attributes.ContainsKey("sDefaultSubject"))
                                {
                                    sSubject = oDocumentTemplate.Attributes["sDefaultSubject"].sValue;
                                }
                                if (oDocumentTemplate.Attributes.ContainsKey("iTransactionType"))
                                {
                                    sTransactionTypeID = oDocumentTemplate.Attributes["iTransactionType"].sValue;
                                }
                            }
                        }
                    }
                    oCNVParams.Add(new CNV { sName = "sPolicyNumber", sValue = sPolicyNumber });
                    oCNVParams.Add(new CNV { sName = "sTransactionTypeID", sValue = sTransactionTypeID });
                    oCNVParams.Add(new CNV { sName = "sPolicyHolderName", sValue = sPolicyHolderName });
                    oCNVParams.Add(new CNV { sName = "sWhenGenerate", sValue = sWhenGenerate });
                    oCNVParams.Add(new CNV { sName = "sCustomerReference", sValue = sCustomerReference });
                    oCNVParams.Add(new CNV { sName = "FKiACPolicyVersionID", sValue = sFKiACPolicyVersionID });
                    string sContext = string.Empty; string sBrokerContext = string.Empty;
                    if (sTransactionTypeID == "10")
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCover;
                            sBrokerContext = XIConstant.Email_BrokerOnCover;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCover;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCover;
                        }
                    }
                    else if (sTransactionTypeID == "25")
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCoverRenewal;
                            sBrokerContext = XIConstant.Email_BrokerOnCoverRenewal;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCoveRenewal;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCoverRenewal;
                        }
                    }
                    else
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCoverMTA;
                            sBrokerContext = XIConstant.Email_BrokerOnCoverMTA;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCoverMTA;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCoverMTA;
                        }
                    }
                    if (iProductVersionID > 0)
                    {
                        var oProductVersionI = oIXI.BOI("ProductVersion_T", iProductVersionID.ToString(), "");
                        if (oProductVersionI != null && oProductVersionI.Attributes.ContainsKey("bIsIndicativePrice"))
                        {
                            IsIndicativeProduct = oProductVersionI.Attributes["bIsIndicativePrice"].bValue;
                        }
                        if (IsIndicativeProduct == true)
                        {
                            sContext = XIConstant.Email_OnCoverIndicative;
                        }
                    }
                    if (string.IsNullOrEmpty(sPolicyNumber))
                    {
                        sContext = XIConstant.Email_NoSequence;
                        sBrokerContext = XIConstant.Email_BrokerNoSequence;
                        //IsSendMail = false;
                    }
                    if (!IsclientEmail)
                    {
                        sContext = XIConstant.Email_Internal_DisableClientEmail;
                        sBrokerContext = XIConstant.Email_Internal_DisableClientEmail_Broker;
                    }
                    //}
                    if (oDocumentTemplate != null)
                    {
                        //var oInstance = oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).XILoad();
                        XIBOInstance oInstance = new XIBOInstance();
                        if (oXIBOInstance == null)
                        {
                            oInstance = oLIst.Structure(sStructureName).XILoad();
                        }
                        else
                        {
                            oInstance = oXIBOInstance;
                        }
                        string sTypeofCover = string.Empty;
                        var oXIIContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "TypeofCover HTML", "0");
                        var oXIContentC = oXIIContent.GetCopy();
                        var oXIContentCDef = oXIContentC.FirstOrDefault();
                        if (oXIContentCDef != null)
                        {
                            sTypeofCover = (string)oXIContent.MergeContentTemplate(oXIContentCDef, oInstance).oResult;
                        }
                        //if (oContentDef != null)
                        //{
                        //XIContentEditors oDocumentContent = oContentDef;
                        oXIContent.sSessionID = sSessionID;
                        oXIContent.FKiProductID = iProductID;
                        if (oContentDef.Category == 10)//Email with Attachments
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            //var sStructureresult = oXIContent.MergeTemplateContent(oDocumentContent, oInstance);//Get template content with dynamic data
                            var sStructureresult = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentDef.Name + " Template merging Completed" });

                            if (sStructureresult.bOK && sStructureresult.oResult != null)
                            {
                                string sContent = (string)sStructureresult.oResult;

                                //XIBOInstance oStructureI = new XIBOInstance();
                                //oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                //var oPolicyInfo = oStructureI.oSubStructureI("QS Instance");
                                //var oPolicyInfo = oStructureI.oSubStructureI("ACPolicy_T");
                                //if (oPolicyInfo != null && oPolicyInfo.oBOIList != null)
                                //{
                                oContentDef.iStatus = 10;
                                SavetoEDITransaction(iProductID, iBOID, oContentDef, sContent, iInstanceID.ToString(), oCNVParams);
                                //}
                                //if (oContentDef.Category == 1)//Email with Attachments
                                //{
                                if (oContentDef.bIsHavingAttachments)
                                {
                                    var oAttachmnetContentDef = oDocTemplateList.Where(x => x.ID != iMainTemplateID && ((x.dActiveFrom == DateTime.MinValue && x.dActiveTo == DateTime.MinValue) || (x.dActiveFrom != DateTime.MinValue && x.dActiveTo != DateTime.MinValue && x.dActiveFrom <= dEffectiveFrom && x.dActiveTo >= dEffectiveFrom) || (x.dActiveFrom == DateTime.MinValue && x.dActiveTo != DateTime.MinValue && x.dActiveTo >= dEffectiveFrom) || (x.dActiveFrom <= dEffectiveFrom && x.dActiveFrom != DateTime.MinValue && x.dActiveTo == DateTime.MinValue))).ToList();
                                    List<Attachment> oAttachments = new List<Attachment>();
                                    foreach (var oContentD in oAttachmnetContentDef)
                                    {
                                        string sTemplateContent = oContentD.Content; int iTemplateType = oContentD.Category;
                                        if (iTemplateType != 0)
                                        {
                                            switch (iTemplateType)
                                            {
                                                case 30://pdf
                                                    if (!string.IsNullOrEmpty(sTemplateContent))
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentD.Name + " Template merging Started" });
                                                        //var sMergeTempResult1 = oXIContent.MergeTemplateContent(oContentD, oInstance);//Get template content with dynamic data
                                                        var sMergeTempResult = oXIContent.MergeContentTemplate(oContentD, oInstance);//Get template content with dynamic data
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentD.Name + " Template merging Completed" });
                                                        if (sMergeTempResult.bOK && sMergeTempResult.oResult != null)
                                                        {
                                                            string sMergeTempContent = (string)sMergeTempResult.oResult; string sDOBPasswordRange = string.Empty;
                                                            if (!string.IsNullOrEmpty(sMergeTempContent))
                                                            {
                                                                string sSurNamePasswordRange = string.Empty;
                                                                //SavetoEDITransaction(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString());
                                                                if (oContentD.bIsPaswordProtected)
                                                                {
                                                                    Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                                                                    AttrNvs["BOFieldID"] = 17609;
                                                                    AttrNvs["sValues"] = oContentD.iSurNamePasswordRange;
                                                                    //AttrNvs["StatusTypeID"] = "10";
                                                                    var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();

                                                                    if (optionList != null)
                                                                    {
                                                                        sSurNamePasswordRange = optionList.sOptionName;
                                                                    }
                                                                    AttrNvs = new Dictionary<string, object>();
                                                                    AttrNvs["BOFieldID"] = 17610;
                                                                    AttrNvs["sValues"] = oContentD.iDOBPasswordRange;
                                                                    //AttrNvs["StatusTypeID"] = "10";
                                                                    var oOptionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();
                                                                    if (oOptionList != null)
                                                                    {
                                                                        sDOBPasswordRange = oOptionList.sOptionName;
                                                                    }

                                                                    if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                                                                    {
                                                                        oEmail.sSurName = oInstance.oSubStructureI("QS Instance").Item(0).XIIValue("sLastName").sResolvedValue;
                                                                    }
                                                                    if (!string.IsNullOrEmpty(sDOBPasswordRange))
                                                                    {
                                                                        oEmail.sDOB = oInstance.oSubStructureI("Driver_T").Item(0).AttributeI("dDOB").sResolvedValue;
                                                                    }
                                                                }

                                                                CResult oResult = new CResult();
                                                                if (oContentD.iTypeofPDF == 20)//using ironPDF
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oContentD.Name + " PDF generation Started" });
                                                                    oResult = oEmail.IronPdf(sMergeTempContent, oContentD.sCSSFileName, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oContentD.Name + " PDF generated" });
                                                                }
                                                                else if (oContentD.iTypeofPDF == 10)//using iTextsharp
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oContentD.Name + " PDF generation Started" });
                                                                    oResult = oEmail.PDFGenerate(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oContentD.Name + " PDF generated" });
                                                                }
                                                                else
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "SyncfusionPDF Generation", sValue = "" + oContentD.Name + " PDF generation Started" });
                                                                    //oResult = oEmail.SyncFusionPDF(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    //oResult = oEmail.PDFGenerate(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "SyncfusionPDF Generation", sValue = "" + oContentD.Name + " PDF generated" });
                                                                }
                                                                // SyncfusionPDF
                                                                //oResult = oEmail.SyncFusionPDF(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                if (oResult.bOK && oResult.oResult != null)
                                                                {
                                                                    oContentD.iStatus = 10;
                                                                    SavetoEDITransaction(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString(), oCNVParams);
                                                                    string sDocumentName = oContentD.Name;
                                                                    oEmail.sDocumentName = sDocumentName;
                                                                    var oAttachment = oEmail.GeneratePDFFile((MemoryStream)oResult.oResult);
                                                                    if (oAttachment.bOK && oAttachment.oResult != null)
                                                                    {
                                                                        Attachment data = (Attachment)oAttachment.oResult;
                                                                        MemoryStream file = (MemoryStream)oResult.oResult;
                                                                        if (oContentD.bIsEmail)
                                                                        {
                                                                            oAttachments.Add(data);
                                                                        }
                                                                        oXIDocs.FKiUserID = Convert.ToInt32(sUserID);
                                                                        oXIDocs.sOrgName = sOrgName;
                                                                        oXIDocs.iOrgID = iOrgID;
                                                                        oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                                                                        oXIDocs.FKiPolicyVersionID = iPolicyVersionID;
                                                                        var oResponse = oXIDocs.SaveDocuments(file, data.Name);//save documents to folder
                                                                        if (oResponse.bOK && oResponse.oResult != null)
                                                                        {
                                                                            string sDocID = (string)oResponse.oResult;
                                                                            oNotifications.iDocumentID = Convert.ToInt32(sDocID);
                                                                            if (oNotifications.iDocumentID != 0)
                                                                            {
                                                                                List<XIIBO> nBOI = new List<XIIBO>();
                                                                                XIContentEditors oContent = new XIContentEditors();
                                                                                var oNotificationContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Document recieved", "0");
                                                                                if (oNotificationContentDef != null && oNotificationContentDef.Count() > 0)
                                                                                {
                                                                                    var oNotificationContent = oNotificationContentDef.FirstOrDefault();
                                                                                    if (oNotificationContent != null)
                                                                                    {
                                                                                        var oDocumentBOI = oIXI.BOI("Documents_T", sDocID);
                                                                                        nBOI.Add(oDocumentBOI);
                                                                                        oNotifications.iStatus = 20;
                                                                                        XIBOInstance oBOIns = new XIBOInstance();
                                                                                        oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                                                                        oBOIns.oStructureInstance[oDocumentBOI.BOD.Name.ToLower()] = nBOI;
                                                                                        var Result = oContent.MergeContentTemplate(oNotificationContent, oBOIns);
                                                                                        if (Result.bOK && Result.oResult != null)
                                                                                        {
                                                                                            var sNotificationResult = (string)Result.oResult;
                                                                                            //Dan Explained
                                                                                            //oxii.xiinfra.coms.notifications.create("CLIENTDOC", sDocID)
                                                                                            oNotifications.Create(sUserID, oNotificationContent.Name, sDocID, oNotificationContent.sSubject, sNotificationResult, iInstanceID.ToString(), iOrgID.ToString());
                                                                                            //oNotifications.Create(sUserID, "CLIENTDOC", sDocID, "", sNotificationResult, iInstanceID.ToString(), iOrgID.ToString());
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            oCResult.oTraceStack.Add(new CNV { sName = "Document saving", sValue = "" + oContentD.Name + " Document saved successfully,DocID:" + sDocID + "" });
                                                                        }

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    oContentD.iStatus = 20;
                                                                    SavetoEDITransaction(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString(), oCNVParams);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string sNotGeneratedContent = (string)sMergeTempResult.oResult;
                                                            oContentD.iStatus = 20;
                                                            SavetoEDITransaction(iProductID, iBOID, oContentD, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                                                        }
                                                    }
                                                    break;
                                                case 40://Nonmerged Document
                                                    if (sTypeofCover != oContentD.Name)
                                                    {
                                                        var oCResponse = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentD.Name, sProductName, 0, sClassName, sFKiACPolicyVersionID);
                                                        if (oCResponse.bOK && oCResponse.oResult != null)
                                                        {
                                                            var oRes = (MemoryStream)oCResponse.oResult;
                                                            string sFileName = oContentD.Name + ".pdf";
                                                            if (oContentD.bIsEmail)
                                                            {
                                                                oAttachments.Add(new Attachment(new MemoryStream(oRes.ToArray()), sFileName));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oContentD.iStatus = 20;
                                                            SavetoEDITransaction(iProductID, iBOID, oContentD, "", iInstanceID.ToString(), oCNVParams);
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }

                                    if (oContentDef.Category == 10 && oContentDef.bIsEmail && bIsOnCover != "false")
                                    //switch (oContentDef.Category)
                                    {
                                        //case 10:
                                        oEmail.EmailID = sEmail;
                                        oEmail.sSubject = sSubject;
                                        oEmail.Bcc = oContentDef.sBCC;
                                        oEmail.cc = oContentDef.sCC;
                                        oEmail.From = oContentDef.sFrom;
                                        oEmail.iServerID = oContentDef.FkiServerID;
                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + sEmail + "" });
                                        var oMailResult = oEmail.Sendmail(iOrgID, sContent, oAttachments, iCustomerID, sContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                        if (oMailResult.bOK && oMailResult.oResult != null)
                                        {
                                            oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + sEmail + "" });
                                            SaveErrortoDB(oCResult);
                                        }
                                        if (!string.IsNullOrEmpty(bIsDocumentCopy) && bIsDocumentCopy.ToLower() == "true")
                                        {
                                            string sFunction = "xi.s|{xi.a|'XIConfig_T','BrokerEmail','sValue','','sName'}";
                                            CResult oCR = new CResult();
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = sFunction.ToString();
                                            oCR = oXIScript.Execute_Script("", "");
                                            var oMailsList = oCR.oResult.ToString().Split(',');
                                            if (oMailsList.Count() > 0)
                                            {
                                                foreach (var oMail in oMailsList)
                                                {
                                                    oEmail.EmailID = oMail;
                                                    var oBrokerMailResult = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sBrokerContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                                    if (oBrokerMailResult.bOK && oBrokerMailResult.oResult != null)
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail successfully sent to Broker", sValue = "Mail sent successfully to email:" + oMail + "" });
                                                        SaveErrortoDB(oCResult);
                                                    }
                                                }

                                            }
                                        }
                                        //break;
                                    }
                                    else if (oContentDef.Category == 10 && bIsOnCover != "false")
                                    {
                                        List<CNV> oAuditParams = new List<CNV>();
                                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iInstanceID.ToString() });
                                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = "Generated " + oContentDef.Name + " mail sent failed due to email Configuration is off against this template" });
                                        var oARes = oEmail.Audit_Policy(oAuditParams);
                                    }
                                }
                                else
                                {
                                    if (oContentDef.Category == 10 && oContentDef.bIsEmail)
                                    //switch (oContentDef.Category)
                                    {
                                        //case 10:
                                        oEmail.EmailID = sEmail;
                                        oEmail.sSubject = sSubject;
                                        oEmail.Bcc = oContentDef.sBCC;
                                        oEmail.cc = oContentDef.sCC;
                                        oEmail.From = oContentDef.sFrom;
                                        oEmail.iServerID = oContentDef.FkiServerID;
                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started without attachments, email:" + sEmail + "" });
                                        var oMailResult = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail without attachments
                                        if (oMailResult.bOK && oMailResult.oResult != null)
                                        {
                                            oCResult.oTraceStack.Add(new CNV { sName = "Mail sent successfully", sValue = "Mail sent successfully to email:" + sEmail + "" });
                                            SaveErrortoDB(oCResult);
                                        }
                                        if (!string.IsNullOrEmpty(bIsDocumentCopy) && bIsDocumentCopy.ToLower() == "true")
                                        {
                                            string sFunction = "xi.s|{xi.a|'XIConfig_T','BrokerEmail','sValue','','sName'}";
                                            CResult oCR = new CResult();
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = sFunction.ToString();
                                            oCR = oXIScript.Execute_Script("", "");
                                            var oMailsList = oCR.oResult.ToString().Split(',');
                                            if (oMailsList.Count() > 0)
                                            {
                                                foreach (var oMail in oMailsList)
                                                {
                                                    oEmail.EmailID = oMail;
                                                    var oBrokerMailResult = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sBrokerContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                                    if (oBrokerMailResult.bOK && oBrokerMailResult.oResult != null)
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail successfully sent to Broker", sValue = "Mail sent successfully to email:" + oMail + "" });
                                                        SaveErrortoDB(oCResult);
                                                    }
                                                }

                                            }
                                        }
                                        //break;
                                    }
                                    else if (oContentDef.Category == 10)
                                    {
                                        List<CNV> oAuditParams = new List<CNV>();
                                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iInstanceID.ToString() });
                                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = "Generated " + oContentDef.Name + " sent failed due to email Configuration is off against this template" });
                                        var oARes = oEmail.Audit_Policy(oAuditParams);
                                    }
                                }

                                // }

                            }
                            else
                            {
                                string sNotGeneratedContent = (string)sStructureresult.oResult;
                                oContentDef.iStatus = 20;
                                SavetoEDITransaction(iProductID, iBOID, oContentDef, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                            }
                        }
                        else if (oContentDef.Category == 20)//EDI
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            var sStructureresult = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oContentDef.Name + " Template merging Completed" });

                            if (sStructureresult.bOK && sStructureresult.oResult != null)
                            {
                                oContentDef.iStatus = 10;
                                string sContent = (string)sStructureresult.oResult;
                                XIBOInstance oStructureI = new XIBOInstance();
                                oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                SavetoEDITransaction(iProductID, iBOID, oContentDef, sContent, iInstanceID.ToString(), oCNVParams);
                            }
                            else
                            {
                                string sNotGeneratedContent = (string)sStructureresult.oResult;
                                oContentDef.iStatus = 20;
                                SavetoEDITransaction(iProductID, iBOID, oContentDef, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                            }
                        }
                        else if (oContentDef.Category == 40 || oContentDef.Category == 60)//non merged docs with out sending email
                        {
                            if (oContentDef.iParentID == 0)
                            {
                                iType = oContentDef.Category;
                            }
                            var oCResponse = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentDef.Name, sProductName, iType, sClassName, sFKiACPolicyVersionID);
                            if (!oCResponse.bOK && oCResponse.oResult == null)
                            {
                                oContentDef.iStatus = 20;
                                SavetoEDITransaction(iProductID, iBOID, oContentDef, "", iInstanceID.ToString(), oCNVParams);
                            }
                        }
                        else if (oContentDef.Category == 50)//Notifications
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "NotificationTemplate merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            var sStructureresult = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oCResult.oTraceStack.Add(new CNV { sName = "NotificationTemplate merging", sValue = "" + oContentDef.Name + " Template merging Completed" });
                            if (sStructureresult.bOK && sStructureresult.oResult != null)
                            {
                                oContentDef.iStatus = 10;
                                string sContent = (string)sStructureresult.oResult;
                                //oNotifications.sSubject = sSubject;
                                //oNotifications.sInstanceID = Convert.ToString(iInstanceID);
                                //oNotifications.sOrgName = sOrgName;
                                //oNotifications.iOrgID = iOrgID;
                                oNotifications.iStatus = 10;
                                oNotifications.Create(sUserID, oContentDef.Name, oNotifications.iDocumentID.ToString(), sSubject, sContent, iInstanceID.ToString(), iOrgID.ToString());

                            }
                        }
                        // }
                    }
                }
                //}

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

        public void SavetoEDITransaction(int iProductID, int iBOID, XIContentEditors oDocumentContent, string sContent, string sInstanceID, List<CNV> ONVParams)
        {
            CResult oCResult = new CResult();
            try
            {
                string sPolicyNo = string.Empty; string sTransactiontype = string.Empty; string sPolicyHolderName = string.Empty; string sWhenGenerate = string.Empty;
                string sCustomerReference = string.Empty; string FKiACPolicyVersionID = string.Empty;
                XIIBO oBOI = new XIIBO();
                XIDXI oXID = new XIDXI();
                if (ONVParams != null && ONVParams.Count() > 0)
                {
                    sPolicyNo = ONVParams.Where(x => x.sName.ToLower() == "sPolicyNumber".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sTransactiontype = ONVParams.Where(x => x.sName.ToLower() == "sTransactionTypeID".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sPolicyHolderName = ONVParams.Where(x => x.sName.ToLower() == "sPolicyHolderName".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sWhenGenerate = ONVParams.Where(x => x.sName.ToLower() == "sWhenGenerate".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sCustomerReference = ONVParams.Where(x => x.sName.ToLower() == "sCustomerReference".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    FKiACPolicyVersionID = ONVParams.Where(x => x.sName.ToLower() == "FKiACPolicyVersionID".ToLower()).Select(x => x.sValue).FirstOrDefault();
                }
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "EDI_Transaction"); //oXID.Get_BODefinition("EDI_Transaction").oResult;
                oBOI.BOD = oBOD;
                oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                oBOI.Attributes["sMergedText"] = new XIIAttribute { sName = "sMergedText", sValue = sContent.Replace("'", "''"), bDirty = true };
                oBOI.Attributes["FkiBOID"] = new XIIAttribute { sName = "FkiBOID", sValue = iBOID.ToString(), bDirty = true };
                oBOI.Attributes["FkiTemplateID"] = new XIIAttribute { sName = "FkiTemplateID", sValue = oDocumentContent.ID.ToString(), bDirty = true };
                oBOI.Attributes["iStatus"] = new XIIAttribute { sName = "iStatus", sValue = Convert.ToString(oDocumentContent.iStatus), bDirty = true };
                oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };
                oBOI.Attributes["iType"] = new XIIAttribute { sName = "iType", sValue = oDocumentContent.Category.ToString(), bDirty = true };
                oBOI.Attributes["FKiProductID"] = new XIIAttribute { sName = "FKiProductID", sValue = iProductID.ToString(), bDirty = true };
                oBOI.Attributes["FKsPolicyNo"] = new XIIAttribute { sName = "FKsPolicyNo", sValue = sPolicyNo, bDirty = true };
                oBOI.Attributes["iTransactionType"] = new XIIAttribute { sName = "iTransactionType", sValue = sTransactiontype, bDirty = true };
                if (!string.IsNullOrEmpty(oDocumentContent.sTemplateHeader))
                {
                    oBOI.Attributes["sMergedTextHeader"] = new XIIAttribute { sName = "sMergedTextHeader", sValue = oDocumentContent.sTemplateHeader, bDirty = true };
                }
                oBOI.Attributes["FKiAddonID"] = new XIIAttribute { sName = "FKiAddonID", sValue = iAddonID.ToString(), bDirty = true };
                oBOI.Attributes["sDocumentName"] = new XIIAttribute { sName = "sDocumentName", sValue = oDocumentContent.Name, bDirty = true };
                oBOI.Attributes["sPolicyHolderName"] = new XIIAttribute { sName = "sPolicyHolderName", sValue = sPolicyHolderName, bDirty = true };
                oBOI.Attributes["sWhenGenerate"] = new XIIAttribute { sName = "sWhenGenerate", sValue = sWhenGenerate, bDirty = true };
                oBOI.Attributes["FKsClientReferenceNumber"] = new XIIAttribute { sName = "FKsClientReferenceNumber", sValue = sCustomerReference, bDirty = true };
                oBOI.Attributes["FKiACPolicyVersionID"] = new XIIAttribute { sName = "FKiACPolicyVersionID", sValue = FKiACPolicyVersionID, bDirty = true };
                var response = oBOI.Save(oBOI);
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Saving EDITransaction" });
                oCResult.sMessage = "ERROR: [ Copy Documenst:" + oDocumentContent.Name + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
        }
        public CResult SaveProductNonMergedDocuments(int iInstanceID, int iUserID, string sOrgName, int iOrgID, string sCopyDocumentName, string sProductName, int iType, string sClassName, string sPolicyVersionID, int iQSInstanceID=0, int iBODID = 0, int iClientID = 0)
        {
            CResult oCResult = new CResult(); string sInstanceID = string.Empty;
            try
            {
                int iPolicyVersioID = 0;
                if (int.TryParse(sPolicyVersionID, out iPolicyVersioID)) { }
                XIInfraDocs oXIDocs = new XIInfraDocs();
                string sCopyDocumentPath = string.Empty;
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\ProductNonMergedDocuments\\" + sProductName + "_" + sClassName + "";
                string[] filePaths = Directory.GetFiles(@"" + sPath + "");
                foreach (var item in filePaths)
                {
                    int ipos = item.LastIndexOf("\\") + 1;
                    string sFileName = item.Substring(ipos, item.Length - ipos);
                    if (sCopyDocumentName == sFileName.Split('.')[0])
                    {
                        sCopyDocumentPath = item;
                    }
                }
                int pos = sCopyDocumentPath.LastIndexOf("\\") + 1;
                string sCopyDocFileName = sCopyDocumentPath.Substring(pos, sCopyDocumentPath.Length - pos);
                //if(sCopyDocFileName!="" && File.Exists(sCopyDocumentPath))
                if (!string.IsNullOrEmpty(sCopyDocFileName))
                {
                    FileStream fs = new FileStream(sCopyDocumentPath, FileMode.Open, FileAccess.Read);
                    byte[] tmpBytes = new byte[fs.Length];
                    fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                    MemoryStream mystream = new MemoryStream(tmpBytes);
                    StreamReader reader = new StreamReader(mystream);
                    oXIDocs.iInstanceID = iInstanceID;
                    oXIDocs.FKiUserID = iUserID;
                    oXIDocs.sOrgName = sOrgName;
                    oXIDocs.iOrgID = iOrgID;
                    oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                    oXIDocs.iType = iType;
                    oXIDocs.FKiBOID = iBODID;
                    oXIDocs.FkiClientID = iClientID;
                    oXIDocs.FKiQSInstanceID = iQSInstanceID;
                    oXIDocs.FKiPolicyVersionID = iPolicyVersioID;
                    var oResponse = oXIDocs.SaveDocuments(mystream, sCopyDocFileName);//save documents to folder
                    if (oResponse.bOK && oResponse.oResult != null)
                    {
                        sInstanceID = (string)oResponse.oResult;
                        oCResult.oTraceStack.Add(new CNV { sName = "Non-Merging Document saving", sValue = "" + sCopyDocFileName + " Document saved successfully,DocID:" + sInstanceID + "" });
                    }
                    oCResult.oResult = mystream;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Non-Merging Documents" });
                oCResult.sMessage = "ERROR: [ Copy Documenst:" + sCopyDocumentName + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult XILoadV2(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            List<CNV> oNVPairsList = new List<CNV>();
            CNV oNVPairs = new CNV();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                oTrace.sClass = "XIInfraDocumentComponent";
                oTrace.sMethod = "XILoad";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                XIInfraEmail oEmail = new XIInfraEmail();
                XIIQS oXIQS = new XIIQS();
                XIIXI oIXI = new XIIXI();
                int iType = 0;
                object oStructureresult = ""; int iProductID = 0; string sEmail = string.Empty; string sUserID = string.Empty; int iBOID = 0; string sProductName = string.Empty;
                string sCustomerReference = string.Empty; string sClassName = string.Empty;
                string sFKiACPolicyVersionID = string.Empty;
                string bIsDocumentCopy = "";
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sBOName = oParams.Where(m => m.sName == "Object").Select(m => m.sValue).FirstOrDefault();
                sDocumentType = oParams.Where(m => m.sName == "Document Type").Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName == "Structure Name").Select(m => m.sValue).FirstOrDefault();
                iDocumentID = Convert.ToInt32(oParams.Where(m => m.sName == "DocumentID").Select(m => m.sValue).FirstOrDefault());
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                iOrgID = Convert.ToInt32(oParams.Where(m => m.sName == "iOrganizationID").Select(m => m.sValue).FirstOrDefault());
                sOutput = oParams.Where(m => m.sName == "Output").Select(m => m.sValue).FirstOrDefault();
                sSubject = oParams.Where(m => m.sName == "Subject").Select(m => m.sValue).FirstOrDefault();
                iAddonID = Convert.ToInt32(oParams.Where(m => m.sName == "iAddonID").Select(m => m.sValue).FirstOrDefault());
                sFKiACPolicyVersionID = oParams.Where(m => m.sName == "FKiPolicyVersionID").Select(m => m.sValue).FirstOrDefault();
                string sWhenGenerate = oParams.Where(m => m.sName == "sWhenGenerate").Select(m => m.sValue).FirstOrDefault();
                bIsDocumentCopy = oParams.Where(m => m.sName == "bIsDocumentCopy").Select(m => m.sValue).FirstOrDefault();
                string bIsOnCover = oParams.Where(m => m.sName == "bIsOnCover").Select(m => m.sValue).FirstOrDefault();
                List<CNV> oCNVParams = new List<CNV>();
                var UserID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|FKiUserID}");
                var LeadID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iLeadID}");
                int iLeadID = 0;
                if (int.TryParse(LeadID, out iLeadID))
                { }
                if (iLeadID == 0)
                {
                    var oPolicyVersionI = oIXI.BOI("ACPolicyVersion_T", sFKiACPolicyVersionID);
                    if (oPolicyVersionI != null && oPolicyVersionI.Attributes.ContainsKey("FKiQsInstanceID"))
                    {
                        List<CNV> oLeadNV = new List<CNV>();
                        oLeadNV.Add(new CNV { sName = "FKiQSInstanceID", sValue = oPolicyVersionI.Attributes["FKiQsInstanceID"].sValue });
                        var oLeadI = oIXI.BOI("Lead_T", "", "Create", oLeadNV);
                        if (oLeadI != null && oLeadI.Attributes.ContainsKey("id"))
                        {
                            if (int.TryParse(oLeadI.Attributes["id"].sValue, out iLeadID))
                            { }
                        }
                    }
                }
                oTrace.oParams.Add(new CNV { sName = "iLeadID", sValue = iLeadID.ToString() });
                int iUser = 0;
                if (int.TryParse(UserID, out iUser))
                { }
                //var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{-iInstanceID}");
                var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyID}");
                var sTransactionTypeID = string.Empty;
                var sPolicyNumber = string.Empty; string sPolicyHolderName = string.Empty;bool IsIndicativeProduct = false;
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> oNV = new List<CNV>();
                bool IsSendMail = true;
                oNV.Add(new CNV { sName = "FkiUserID", sValue = iUser.ToString() });
                long iCustomerID = 0;
                int iProductVersionID = 0;
                var oCustomerI = oIXI.BOI("Customer_T", "", "Create", oNV);
                if (oCustomerI != null && oCustomerI.Attributes != null && oCustomerI.Attributes.ContainsKey("bDisableClientLogin"))
                {
                    if (oCustomerI != null && oCustomerI.BOD != null)
                    {
                        if (oCustomerI.Attributes.ContainsKey(oCustomerI.BOD.sPrimaryKey))
                        {
                            var cusID = oCustomerI.Attributes[oCustomerI.BOD.sPrimaryKey].sValue;
                            long.TryParse(cusID, out iCustomerID);
                        }
                    }
                    var IsDisableClient = oCustomerI.Attributes["bDisableClientLogin"].sValue;
                    if (!string.IsNullOrEmpty(IsDisableClient) && IsDisableClient.ToLower() == "true")
                    {
                        IsSendMail = false;
                    }
                }
                long.TryParse(sInstanceID, out iInstanceID);
                if (iInstanceID == 0)
                {
                    iInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault());
                }
                oTrace.oParams.Add(new CNV { sName = "iPolicyID", sValue = iInstanceID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iTemplateID", sValue = iDocumentID.ToString() });
                var oDocContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sDocumentType, iDocumentID.ToString()); //oXIDX.Get_ContentDefinition(iDocumentID, sDocumentType);
                //var oInstance = oXIQS.QSI(oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).oSubStruct("Policy Version").oSubStruct("QS Instance"));
                //if(oContentDef.Category==1)
                var oDocTemplateList = oDocContent.GetCopy();
                var oContentDef = oDocTemplateList.FirstOrDefault();
                //{
                if (oContentDef != null)
                {
                    int iMainTemplateID = oContentDef.ID;
                    var oLIst = oIXI.BOI(sBOName, iInstanceID.ToString());
                    iBOID = oLIst.BOD.BOID;
                    XIIBO oDocumentTemplate = new XIIBO();
                    bool IsclientEmail = true;
                    //if (string.IsNullOrEmpty(sSubject))
                    //{

                    if (oLIst.Attributes.ContainsKey("bIsClientEmail"))
                    {
                        string sclientEmail = oLIst.AttributeI("bIsClientEmail").sValue;
                        if (!string.IsNullOrEmpty(sclientEmail) && sclientEmail.ToLower() == "false")
                        {
                            IsclientEmail = false;
                        }

                    }
                    if (oLIst.Attributes.ContainsKey("sPolicyNo"))
                    {
                        sPolicyNumber = oLIst.AttributeI("sPolicyNo").sValue;

                    }
                    if (oLIst.Attributes.ContainsKey("sName"))
                    {
                        sPolicyHolderName = oLIst.AttributeI("sName").sValue;
                    }
                    if (oLIst != null && oLIst.Attributes.ContainsKey("FKiProductID"))
                    {
                        sEmail = oLIst.Attributes.Where(x => x.Key.ToLower() == "sEmail".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        sUserID = oLIst.Attributes.Where(x => x.Key.ToLower() == "FKiUserID".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        iProductID = Convert.ToInt32(oLIst.Attributes["FKiProductID"].sValue);
                        int.TryParse(oLIst.Attributes["FKiProductVersionID"].sValue, out iProductVersionID);
                        sProductName = oLIst.AttributeI("FKiProductID").ResolveFK("display");
                        sClassName = oLIst.AttributeI("FKsClass").sValue;
                        sCustomerReference = oLIst.AttributeI("FKiCustomerID").ResolveFK("display");
                        if (iProductID != 0)
                        {
                            List<CNV> oNVParams = new List<CNV>();
                            CNV oCNV = new CNV();
                            if (iAddonID != 0)
                            {
                                oCNV.sName = "FKiProductAddonID";
                                oCNV.sValue = iAddonID.ToString();
                                oNVParams.Add(oCNV);
                            }
                            else
                            {
                                oCNV.sName = "FKiProductID";
                                oCNV.sValue = iProductID.ToString();
                                oNVParams.Add(oCNV);
                            }
                            oCNV = new CNV();
                            oCNV.sName = "FKiTemplateID";
                            oCNV.sValue = iMainTemplateID.ToString();
                            oNVParams.Add(oCNV);
                            oDocumentTemplate = oIXI.BOI("DocumentTemplate", null, null, oNVParams);
                            if (oDocumentTemplate != null)
                            {
                                if (oDocumentTemplate.Attributes.ContainsKey("sDefaultSubject"))
                                {
                                    sSubject = oDocumentTemplate.Attributes["sDefaultSubject"].sValue;
                                }
                                if (oDocumentTemplate.Attributes.ContainsKey("iTransactionType"))
                                {
                                    sTransactionTypeID = oDocumentTemplate.Attributes["iTransactionType"].sValue;
                                }
                            }
                        }
                    }
                    oCNVParams.Add(new CNV { sName = "sPolicyNumber", sValue = sPolicyNumber });
                    oCNVParams.Add(new CNV { sName = "sTransactionTypeID", sValue = sTransactionTypeID });
                    oCNVParams.Add(new CNV { sName = "sPolicyHolderName", sValue = sPolicyHolderName });
                    oCNVParams.Add(new CNV { sName = "sWhenGenerate", sValue = sWhenGenerate });
                    oCNVParams.Add(new CNV { sName = "sCustomerReference", sValue = sCustomerReference });
                    oCNVParams.Add(new CNV { sName = "FKiACPolicyVersionID", sValue = sFKiACPolicyVersionID });
                    string sContext = string.Empty; string sBrokerContext = string.Empty;
                    if (sTransactionTypeID == "10")
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCover;
                            sBrokerContext = XIConstant.Email_BrokerOnCover;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCover;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCover;
                        }
                    }
                    else if (sTransactionTypeID == "25")
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCoverRenewal;
                            sBrokerContext = XIConstant.Email_BrokerOnCoverRenewal;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCoveRenewal;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCoverRenewal;
                        }
                    }
                    else
                    {
                        if (IsSendMail)
                        {
                            sContext = XIConstant.Email_OnCoverMTA;
                            sBrokerContext = XIConstant.Email_BrokerOnCoverMTA;
                        }
                        else
                        {
                            sContext = XIConstant.Email_InternalNewUserOnCoverMTA;
                            sBrokerContext = XIConstant.Email_InternalNewUserBrokerOnCoverMTA;
                        }
                    }
                    if (iProductVersionID > 0)
                    {
                        var oProductVersionI = oIXI.BOI("ProductVersion_T", iProductVersionID.ToString(), "");
                        if (oProductVersionI != null && oProductVersionI.Attributes.ContainsKey("bIsIndicativePrice"))
                        {
                            IsIndicativeProduct = oProductVersionI.Attributes["bIsIndicativePrice"].bValue;
                        }
                        if (IsIndicativeProduct == true)
                        {
                            sContext = XIConstant.Email_OnCoverIndicative;
                        }
                    }
                    if (string.IsNullOrEmpty(sPolicyNumber))
                    {
                        sContext = XIConstant.Email_NoSequence;
                        sBrokerContext = XIConstant.Email_BrokerNoSequence;
                        //IsSendMail = false;
                    }
                    if (!IsclientEmail)
                    {
                        sContext = XIConstant.Email_Internal_DisableClientEmail;
                        sBrokerContext = XIConstant.Email_Internal_DisableClientEmail_Broker;
                    }
                    //}
                    if (oDocumentTemplate != null)
                    {
                        //var oInstance = oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).XILoad();
                        XIBOInstance oInstance = new XIBOInstance();
                        if (oXIBOInstance == null)
                        {
                            oInstance = oLIst.Structure(sStructureName).XILoad();
                        }
                        else
                        {
                            oInstance = oXIBOInstance;
                        }
                        string sTypeofCover = string.Empty;
                        var oXIIContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "TypeofCover HTML", "0");
                        var oXIContentC = oXIIContent.GetCopy();
                        var oXIContentCDef = oXIContentC.FirstOrDefault();
                        if (oXIContentCDef != null)
                        {
                            sTypeofCover = (string)oXIContent.MergeContentTemplate(oXIContentCDef, oInstance).oResult;
                        }
                        //if (oContentDef != null)
                        //{
                        //XIContentEditors oDocumentContent = oContentDef;
                        oXIContent.sSessionID = sSessionID;
                        oXIContent.FKiProductID = iProductID;
                        if (oContentDef.Category == 10)//Email with Attachments
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            //var sStructureresult = oXIContent.MergeTemplateContent(oDocumentContent, oInstance);//Get template content with dynamic data
                            oCR = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oTrace.oTrace.Add(oCR.oTrace);
                            oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentDef.Name + " Template merging Completed" });

                            if (oCR.bOK && oCR.oResult != null)
                            {
                                string sContent = (string)oCR.oResult;

                                //XIBOInstance oStructureI = new XIBOInstance();
                                //oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                //var oPolicyInfo = oStructureI.oSubStructureI("QS Instance");
                                //var oPolicyInfo = oStructureI.oSubStructureI("ACPolicy_T");
                                //if (oPolicyInfo != null && oPolicyInfo.oBOIList != null)
                                //{
                                oContentDef.iStatus = 10;
                                oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentDef, sContent, iInstanceID.ToString(), oCNVParams);
                                oTrace.oTrace.Add(oCR.oTrace);
                                //}
                                //if (oContentDef.Category == 1)//Email with Attachments
                                //{
                                if (oContentDef.bIsHavingAttachments)
                                {
                                    var oAttachmnetContentDef = oDocTemplateList.Where(x => x.ID != iMainTemplateID).ToList();
                                    List<Attachment> oAttachments = new List<Attachment>();
                                    foreach (var oContentD in oAttachmnetContentDef)
                                    {
                                        string sTemplateContent = oContentD.Content; int iTemplateType = oContentD.Category;
                                        if (iTemplateType != 0)
                                        {
                                            switch (iTemplateType)
                                            {
                                                case 30://pdf
                                                    if (!string.IsNullOrEmpty(sTemplateContent))
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentD.Name + " Template merging Started" });
                                                        //var sMergeTempResult1 = oXIContent.MergeTemplateContent(oContentD, oInstance);//Get template content with dynamic data
                                                        oCR = oXIContent.MergeContentTemplate(oContentD, oInstance);//Get template content with dynamic data
                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentD.Name + " Template merging Completed" });
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            string sMergeTempContent = (string)oCR.oResult; string sDOBPasswordRange = string.Empty;
                                                            if (!string.IsNullOrEmpty(sMergeTempContent))
                                                            {
                                                                string sSurNamePasswordRange = string.Empty;
                                                                //SavetoEDITransaction(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString());
                                                                if (oContentD.bIsPaswordProtected)
                                                                {
                                                                    Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                                                                    AttrNvs["BOFieldID"] = 17609;
                                                                    AttrNvs["sValues"] = oContentD.iSurNamePasswordRange;
                                                                    //AttrNvs["StatusTypeID"] = "10";
                                                                    var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();

                                                                    if (optionList != null)
                                                                    {
                                                                        sSurNamePasswordRange = optionList.sOptionName;
                                                                    }
                                                                    AttrNvs = new Dictionary<string, object>();
                                                                    AttrNvs["BOFieldID"] = 17610;
                                                                    AttrNvs["sValues"] = oContentD.iDOBPasswordRange;
                                                                    //AttrNvs["StatusTypeID"] = "10";
                                                                    var oOptionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();
                                                                    if (oOptionList != null)
                                                                    {
                                                                        sDOBPasswordRange = oOptionList.sOptionName;
                                                                    }

                                                                    if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                                                                    {
                                                                        oEmail.sSurName = oInstance.oSubStructureI("QS Instance").Item(0).XIIValue("sLastName").sResolvedValue;
                                                                    }
                                                                    if (!string.IsNullOrEmpty(sDOBPasswordRange))
                                                                    {
                                                                        oEmail.sDOB = oInstance.oSubStructureI("Driver_T").Item(0).AttributeI("dDOB").sResolvedValue;
                                                                    }
                                                                }

                                                                CResult oResult = new CResult();
                                                                if (oContentD.iTypeofPDF == 20)//using ironPDF
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oContentD.Name + " PDF generation Started" });
                                                                    oResult = oEmail.IronPdf(sMergeTempContent, oContentD.sCSSFileName, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oTrace.oTrace.Add(oResult.oTrace);
                                                                    if(oResult.bOK && oResult.oResult != null)
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oContentD.Name + " PDF generated" });
                                                                }
                                                                else//using iTextsharp
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oContentD.Name + " PDF generation Started" });
                                                                    oResult = oEmail.PDFGenerate(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oContentD.Name + " PDF generated" });
                                                                }
                                                                if (oResult.bOK && oResult.oResult != null)
                                                                {
                                                                    oContentD.iStatus = 10;
                                                                    oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString(), oCNVParams);
                                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                    string sDocumentName = oContentD.Name;
                                                                    oEmail.sDocumentName = sDocumentName;
                                                                    var oAttachment = oEmail.GeneratePDFFile((MemoryStream)oResult.oResult);
                                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                                    if (oAttachment.bOK && oAttachment.oResult != null)
                                                                    {
                                                                        Attachment data = (Attachment)oAttachment.oResult;
                                                                        MemoryStream file = (MemoryStream)oResult.oResult;
                                                                        if (oContentD.bIsEmail)
                                                                        {
                                                                            oAttachments.Add(data);
                                                                        }
                                                                        oXIDocs.FKiUserID = Convert.ToInt32(sUserID);
                                                                        oXIDocs.sOrgName = sOrgName;
                                                                        oXIDocs.iOrgID = iOrgID;
                                                                        oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                                                                        int iPolicyVersionID = 0;
                                                                        if (int.TryParse(sFKiACPolicyVersionID, out iPolicyVersionID)) { }
                                                                        oXIDocs.FKiPolicyVersionID = iPolicyVersionID;
                                                                        oCR = oXIDocs.SaveDocuments(file, data.Name);//save documents to folder
                                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                        {
                                                                            string sDocID = (string)oCR.oResult;
                                                                            oNotifications.iDocumentID = Convert.ToInt32(sDocID);
                                                                            if (oNotifications.iDocumentID != 0)
                                                                            {
                                                                                List<XIIBO> nBOI = new List<XIIBO>();
                                                                                XIContentEditors oContent = new XIContentEditors();
                                                                                var oNotificationContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "Document recieved", "0");
                                                                                if (oNotificationContentDef != null && oNotificationContentDef.Count() > 0)
                                                                                {
                                                                                    var oNotificationContent = oNotificationContentDef.FirstOrDefault();
                                                                                    if (oNotificationContent != null)
                                                                                    {
                                                                                        var oDocumentBOI = oIXI.BOI("Documents_T", sDocID);
                                                                                        nBOI.Add(oDocumentBOI);
                                                                                        oNotifications.iStatus = 20;
                                                                                        XIBOInstance oBOIns = new XIBOInstance();
                                                                                        oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                                                                        oBOIns.oStructureInstance[oDocumentBOI.BOD.Name.ToLower()] = nBOI;
                                                                                        oCR = oContent.MergeContentTemplate(oNotificationContent, oBOIns);
                                                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                                                        if (oCR.bOK && oCR.oResult != null)
                                                                                        {
                                                                                            var sNotificationResult = (string)oCR.oResult;
                                                                                            //Dan Explained
                                                                                            //oxii.xiinfra.coms.notifications.create("CLIENTDOC", sDocID)
                                                                                            oCR = oNotifications.Create(sUserID, oNotificationContent.Name, sDocID, oNotificationContent.sSubject, sNotificationResult, iInstanceID.ToString(), iOrgID.ToString());
                                                                                            oTrace.oTrace.Add(oCR.oTrace);
                                                                                            if (oCR.bOK && oCR.oResult != null)
                                                                                            {
                                                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                                            }
                                                                                            //oNotifications.Create(sUserID, "CLIENTDOC", sDocID, "", sNotificationResult, iInstanceID.ToString(), iOrgID.ToString());
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            oCResult.oTraceStack.Add(new CNV { sName = "Document saving", sValue = "" + oContentD.Name + " Document saved successfully,DocID:" + sDocID + "" });
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    oContentD.iStatus = 20;
                                                                    oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString(), oCNVParams);
                                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                                    if (oCR.bOK && oCR.oResult != null)
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                                    }
                                                                    else
                                                                    {
                                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string sNotGeneratedContent = (string)oCR.oResult;
                                                            oContentD.iStatus = 20;
                                                            oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentD, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                                                            oTrace.oTrace.Add(oCR.oTrace);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                            }
                                                            else
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case 40://Nonmerged Document
                                                    if (sTypeofCover != oContentD.Name)
                                                    {
                                                        oCR = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentD.Name, sProductName, 0, sClassName, sFKiACPolicyVersionID);
                                                        oTrace.oTrace.Add(oCR.oTrace);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                        }
                                                        else
                                                        {
                                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                        }
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            var oRes = (MemoryStream)oCR.oResult;
                                                            string sFileName = oContentD.Name + ".pdf";
                                                            if (oContentD.bIsEmail)
                                                            {
                                                                oAttachments.Add(new Attachment(new MemoryStream(oRes.ToArray()), sFileName));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            oContentD.iStatus = 20;
                                                            oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentD, "", iInstanceID.ToString(), oCNVParams);
                                                            oTrace.oTrace.Add(oCR.oTrace);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                            }
                                                            else
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                    }

                                    if (oContentDef.Category == 10 && oContentDef.bIsEmail && bIsOnCover != "false")
                                    //switch (oContentDef.Category)
                                    {
                                        //case 10:
                                        oEmail.EmailID = sEmail;
                                        oEmail.sSubject = sSubject;
                                        oEmail.Bcc = oContentDef.sBCC;
                                        oEmail.cc = oContentDef.sCC;
                                        oEmail.From = oContentDef.sFrom;
                                        oEmail.iServerID = oContentDef.FkiServerID;
                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + sEmail + "" });
                                        oCR = oEmail.Sendmail(iOrgID, sContent, oAttachments, iCustomerID, sContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                        oTrace.oTrace.Add(oCR.oTrace);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + sEmail + "" });
                                            SaveErrortoDB(oCResult);
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        }
                                        if (!string.IsNullOrEmpty(bIsDocumentCopy) && bIsDocumentCopy.ToLower() == "true")
                                        {
                                            string sFunction = "xi.s|{xi.a|'XIConfig_T','BrokerEmail','sValue','','sName'}";
                                            oCR = new CResult();
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = sFunction.ToString();
                                            oCR = oXIScript.Execute_Script("", "");
                                            var oMailsList = oCR.oResult.ToString().Split(',');
                                            if (oMailsList.Count() > 0)
                                            {
                                                foreach (var oMail in oMailsList)
                                                {
                                                    oEmail.EmailID = oMail;
                                                    oCR = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sBrokerContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail successfully sent to Broker", sValue = "Mail sent successfully to email:" + oMail + "" });
                                                        SaveErrortoDB(oCResult);
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }

                                            }
                                        }
                                        //break;
                                    }
                                    else if (oContentDef.Category == 10 && bIsOnCover != "false")
                                    {
                                        List<CNV> oAuditParams = new List<CNV>();
                                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iInstanceID.ToString() });
                                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = "Generated " + oContentDef.Name + " mail sent failed due to email Configuration is off against this template" });
                                        var oARes = oEmail.Audit_Policy(oAuditParams);
                                    }
                                }
                                else
                                {
                                    if (oContentDef.Category == 10 && oContentDef.bIsEmail)
                                    //switch (oContentDef.Category)
                                    {
                                        //case 10:
                                        oEmail.EmailID = sEmail;
                                        oEmail.sSubject = sSubject;
                                        oEmail.Bcc = oContentDef.sBCC;
                                        oEmail.cc = oContentDef.sCC;
                                        oEmail.From = oContentDef.sFrom;
                                        oEmail.iServerID = oContentDef.FkiServerID;
                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started without attachments, email:" + sEmail + "" });
                                        oCR = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail without attachments
                                        oTrace.oTrace.Add(oCR.oTrace);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oCResult.oTraceStack.Add(new CNV { sName = "Mail sent successfully", sValue = "Mail sent successfully to email:" + sEmail + "" });
                                            SaveErrortoDB(oCResult);
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        }
                                        if (!string.IsNullOrEmpty(bIsDocumentCopy) && bIsDocumentCopy.ToLower() == "true")
                                        {
                                            string sFunction = "xi.s|{xi.a|'XIConfig_T','BrokerEmail','sValue','','sName'}";
                                            oCR = new CResult();
                                            XIDScript oXIScript = new XIDScript();
                                            oXIScript.sScript = sFunction.ToString();
                                            oCR = oXIScript.Execute_Script("", "");
                                            var oMailsList = oCR.oResult.ToString().Split(',');
                                            if (oMailsList.Count() > 0)
                                            {
                                                foreach (var oMail in oMailsList)
                                                {
                                                    oEmail.EmailID = oMail;
                                                    oCR = oEmail.Sendmail(iOrgID, sContent, null, iCustomerID, sBrokerContext, iLeadID, null, iInstanceID, oContentDef.bIsBCCOnly);//send mail with attachments
                                                    oTrace.oTrace.Add(oCR.oTrace);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Mail successfully sent to Broker", sValue = "Mail sent successfully to email:" + oMail + "" });
                                                        SaveErrortoDB(oCResult);
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                                    }
                                                    else
                                                    {
                                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                    }
                                                }

                                            }
                                        }
                                        //break;
                                    }
                                    else if (oContentDef.Category == 10)
                                    {
                                        List<CNV> oAuditParams = new List<CNV>();
                                        oAuditParams.Add(new CNV { sName = "iACPolicyID", sValue = iInstanceID.ToString() });
                                        oAuditParams.Add(new CNV { sName = "sAuditInfo", sValue = "Generated " + oContentDef.Name + " sent failed due to email Configuration is off against this template" });
                                        var oARes = oEmail.Audit_Policy(oAuditParams);
                                    }
                                }

                                // }

                            }
                            else
                            {
                                string sNotGeneratedContent = (string)oCR.oResult;
                                oContentDef.iStatus = 20;
                                oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentDef, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        else if (oContentDef.Category == 20)//EDI
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            oCR = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oTrace.oTrace.Add(oCR.oTrace);
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oContentDef.Name + " Template merging Completed" });

                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oContentDef.iStatus = 10;
                                string sContent = (string)oCR.oResult;
                                XIBOInstance oStructureI = new XIBOInstance();
                                oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentDef, sContent, iInstanceID.ToString(), oCNVParams);
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                            else
                            {
                                string sNotGeneratedContent = (string)oCR.oResult;
                                oContentDef.iStatus = 20;
                                oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentDef, sNotGeneratedContent, iInstanceID.ToString(), oCNVParams);
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        else if (oContentDef.Category == 40 || oContentDef.Category == 60)//non merged docs with out sending email
                        {
                            if (oContentDef.iParentID == 0)
                            {
                                iType = oContentDef.Category;
                            }
                            oCR = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentDef.Name, sProductName, iType, sClassName, sFKiACPolicyVersionID);
                            oTrace.oTrace.Add(oCR.oTrace);
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            if (!oCR.bOK && oCR.oResult == null)
                            {
                                oContentDef.iStatus = 20;
                                oCR = SavetoEDITransactionV2(iProductID, iBOID, oContentDef, "", iInstanceID.ToString(), oCNVParams);
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                }
                            }
                        }
                        else if (oContentDef.Category == 50)//Notifications
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "NotificationTemplate merging", sValue = "" + oContentDef.Name + " Template merging Started" });
                            oCR = oXIContent.MergeContentTemplate(oContentDef, oInstance);//Get template content with dynamic data
                            oTrace.oTrace.Add(oCR.oTrace);
                            oCResult.oTraceStack.Add(new CNV { sName = "NotificationTemplate merging", sValue = "" + oContentDef.Name + " Template merging Completed" });
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                oContentDef.iStatus = 10;
                                string sContent = (string)oCR.oResult;
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                //oNotifications.sSubject = sSubject;
                                //oNotifications.sInstanceID = Convert.ToString(iInstanceID);
                                //oNotifications.sOrgName = sOrgName;
                                //oNotifications.iOrgID = iOrgID;
                                oNotifications.iStatus = 10;
                                oCR = oNotifications.Create(sUserID, oContentDef.Name, oNotifications.iDocumentID.ToString(), sSubject, sContent, iInstanceID.ToString(), iOrgID.ToString());
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
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
                        // }
                    }
                }
                //}

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

        private CResult SavetoEDITransactionV2(int iProductID, int iBOID, XIContentEditors oDocumentContent, string sContent, string sInstanceID, List<CNV> ONVParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            try
            {
                oTrace.sClass = "XIInfraDocumentComponent";
                oTrace.sMethod = "SavetoEDITransaction";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                string sPolicyNo = string.Empty; string sTransactiontype = string.Empty; string sPolicyHolderName = string.Empty; string sWhenGenerate = string.Empty;
                string sCustomerReference = string.Empty; string FKiACPolicyVersionID = string.Empty;
                XIIBO oBOI = new XIIBO();
                XIDXI oXID = new XIDXI();
                if (ONVParams != null && ONVParams.Count() > 0)
                {
                    sPolicyNo = ONVParams.Where(x => x.sName.ToLower() == "sPolicyNumber".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sTransactiontype = ONVParams.Where(x => x.sName.ToLower() == "sTransactionTypeID".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sPolicyHolderName = ONVParams.Where(x => x.sName.ToLower() == "sPolicyHolderName".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sWhenGenerate = ONVParams.Where(x => x.sName.ToLower() == "sWhenGenerate".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    sCustomerReference = ONVParams.Where(x => x.sName.ToLower() == "sCustomerReference".ToLower()).Select(x => x.sValue).FirstOrDefault();
                    FKiACPolicyVersionID = ONVParams.Where(x => x.sName.ToLower() == "FKiACPolicyVersionID".ToLower()).Select(x => x.sValue).FirstOrDefault();
                }
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "EDI_Transaction"); //oXID.Get_BODefinition("EDI_Transaction").oResult;
                oBOI.BOD = oBOD;
                oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                oBOI.Attributes["sMergedText"] = new XIIAttribute { sName = "sMergedText", sValue = sContent.Replace("'", "''"), bDirty = true };
                oBOI.Attributes["FkiBOID"] = new XIIAttribute { sName = "FkiBOID", sValue = iBOID.ToString(), bDirty = true };
                oBOI.Attributes["FkiTemplateID"] = new XIIAttribute { sName = "FkiTemplateID", sValue = oDocumentContent.ID.ToString(), bDirty = true };
                oBOI.Attributes["iStatus"] = new XIIAttribute { sName = "iStatus", sValue = Convert.ToString(oDocumentContent.iStatus), bDirty = true };
                oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };
                oBOI.Attributes["iType"] = new XIIAttribute { sName = "iType", sValue = oDocumentContent.Category.ToString(), bDirty = true };
                oBOI.Attributes["FKiProductID"] = new XIIAttribute { sName = "FKiProductID", sValue = iProductID.ToString(), bDirty = true };
                oBOI.Attributes["FKsPolicyNo"] = new XIIAttribute { sName = "FKsPolicyNo", sValue = sPolicyNo, bDirty = true };
                oBOI.Attributes["iTransactionType"] = new XIIAttribute { sName = "iTransactionType", sValue = sTransactiontype, bDirty = true };
                if (!string.IsNullOrEmpty(oDocumentContent.sTemplateHeader))
                {
                    oBOI.Attributes["sMergedTextHeader"] = new XIIAttribute { sName = "sMergedTextHeader", sValue = oDocumentContent.sTemplateHeader, bDirty = true };
                }
                oBOI.Attributes["FKiAddonID"] = new XIIAttribute { sName = "FKiAddonID", sValue = iAddonID.ToString(), bDirty = true };
                oBOI.Attributes["sDocumentName"] = new XIIAttribute { sName = "sDocumentName", sValue = oDocumentContent.Name, bDirty = true };
                oBOI.Attributes["sPolicyHolderName"] = new XIIAttribute { sName = "sPolicyHolderName", sValue = sPolicyHolderName, bDirty = true };
                oBOI.Attributes["sWhenGenerate"] = new XIIAttribute { sName = "sWhenGenerate", sValue = sWhenGenerate, bDirty = true };
                oBOI.Attributes["FKsClientReferenceNumber"] = new XIIAttribute { sName = "FKsClientReferenceNumber", sValue = sCustomerReference, bDirty = true };
                oBOI.Attributes["FKiACPolicyVersionID"] = new XIIAttribute { sName = "FKiACPolicyVersionID", sValue = FKiACPolicyVersionID, bDirty = true };
                oCR = oBOI.Save(oBOI);
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
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Saving EDITransaction" });
                oCResult.sMessage = "ERROR: [ Copy Documenst:" + oDocumentContent.Name + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult SaveProductNonMergedDocumentsV2(int iInstanceID, int iUserID, string sOrgName, int iOrgID, string sCopyDocumentName, string sProductName, int iType, string sClassName, string sPolicyVersionID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            string sInstanceID = string.Empty;
            try
            {
                oTrace.sClass = "XIInfraDocumentComponent";
                oTrace.sMethod = "SaveProductNonMergedDocuments";
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
                int iPolicyVersioID = 0;
                if (int.TryParse(sPolicyVersionID, out iPolicyVersioID)) { }
                XIInfraDocs oXIDocs = new XIInfraDocs();
                string sCopyDocumentPath = string.Empty;
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\ProductNonMergedDocuments\\" + sProductName + "_" + sClassName + "";
                string[] filePaths = Directory.GetFiles(@"" + sPath + "");
                foreach (var item in filePaths)
                {
                    int ipos = item.LastIndexOf("\\") + 1;
                    string sFileName = item.Substring(ipos, item.Length - ipos);
                    if (sCopyDocumentName == sFileName.Split('.')[0])
                    {
                        sCopyDocumentPath = item;
                    }
                }
                int pos = sCopyDocumentPath.LastIndexOf("\\") + 1;
                string sCopyDocFileName = sCopyDocumentPath.Substring(pos, sCopyDocumentPath.Length - pos);
                //if(sCopyDocFileName!="" && File.Exists(sCopyDocumentPath))
                if (!string.IsNullOrEmpty(sCopyDocFileName))
                {
                    oTrace.oParams.Add(new CNV { sName = "FileName", sValue = sCopyDocFileName });
                    FileStream fs = new FileStream(sCopyDocumentPath, FileMode.Open, FileAccess.Read);
                    byte[] tmpBytes = new byte[fs.Length];
                    fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                    MemoryStream mystream = new MemoryStream(tmpBytes);
                    StreamReader reader = new StreamReader(mystream);
                    oXIDocs.iInstanceID = iInstanceID;
                    oXIDocs.FKiUserID = iUserID;
                    oXIDocs.sOrgName = sOrgName;
                    oXIDocs.iOrgID = iOrgID;
                    oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                    oXIDocs.iType = iType;
                    oXIDocs.FKiPolicyVersionID = iPolicyVersioID;
                    oCR = oXIDocs.SaveDocuments(mystream, sCopyDocFileName);//save documents to folder
                    oTrace.oTrace.Add(oCR.oTrace);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sInstanceID = (string)oCR.oResult;
                        oCResult.oTraceStack.Add(new CNV { sName = "Non-Merging Document saving", sValue = "" + sCopyDocFileName + " Document saved successfully,DocID:" + sInstanceID + "" });
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                    oCResult.oResult = mystream;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Non-Merging Documents" });
                oCResult.sMessage = "ERROR: [ Copy Documenst:" + sCopyDocumentName + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oTrace.sMessage = ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}