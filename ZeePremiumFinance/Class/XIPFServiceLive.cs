using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using XICore;
using XISystem;
using ZeeBNPPFServices;
using ZeeInsurance;
using System.Reflection;
//using System.Runtime.Caching;
using xiEnumSystem;
using XIDNA.Repository;
using ZeeBNPPFServices.BNPServiceLive;
using static ZeeBNPPFServices.XIPFCommon;
using System.Data.SqlClient;

namespace ZeePremiumFinance
{
    //public static class MemoryCacheHackExtensions
    //{
    //    public static long GetApproximateSize(this MemoryCache cache)
    //    {
    //        var statsField = typeof(MemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var statsValue = statsField.GetValue(cache);
    //        var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var monitorValue = monitorField.GetValue(statsValue);
    //        var sizeField = monitorValue.GetType().GetField("_sizedRef", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var sizeValue = sizeField.GetValue(monitorValue);
    //        var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
    //        return (long)approxProp.GetValue(sizeValue, null);
    //    }
    //}
    public class XIPFServiceLive
    {
        XIInfraCache oCache = new XIInfraCache();
        XIBNPFileWrapperLive xiwrapper = new XIBNPFileWrapperLive();

        private T GetInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
        private dynamic myxibnpapi;
        public string RequestMode
        {
            get
            {
                return ConfigurationManager.AppSettings["BNPRequestMode"];
            }
        }
        public dynamic xibnpapi
        {
            get
            {
                if (RequestMode == "Mock")
                {
                    return new XIMockAPI();
                }
                else
                {
                    return new XIBNPAPILive();
                }
            }
            set
            {
                myxibnpapi = value;
            }
        }
        public string FKsUserID = null;
        public string sDataBase = null;
        public string BrokerReference = null;
        public string sTransactionID = null;
        //string RequestMode = ConfigurationManager.AppSettings["BNPRequestMode"];
        #region NewBusiness_Method
        private Dictionary<string, XIPFfields> GetFormatedPFfields(EnumXIBNPMethods enumximethod)
        {
            try
            {
                var pfbnpfields = GetPFBNPFields(enumximethod);
                var reqesult = (Dictionary<string, XIIBO>)pfbnpfields.oResult;
                Dictionary<string, XIPFfields> dictpfs = new Dictionary<string, XIPFfields>();
                foreach (var field in reqesult)
                {
                    dictpfs.Add(field.Value.AttributeI("XIFieldName").sValue, new XIPFfields
                    {
                        bMandatory = field.Value.AttributeI("IsMandatory").bValue,
                        sType = field.Value.AttributeI("Type").sValue,
                        iMinlength = field.Value.AttributeI("MinLength").iValue,
                        iMaxlength = field.Value.AttributeI("MaxLength").iValue,
                        sNotation = field.Value.AttributeI("Notation").sValue,
                        sErrorMessage = field.Value.AttributeI("ErrorMessage").sValue,
                        sRegexp = field.Value.AttributeI("Regexp").sValue,
                        sXIFieldName = field.Value.AttributeI("XIFieldName").sValue,
                        sfDisplayName = field.Value.AttributeI("fDisplayName").sValue,
                        sDefaultValue = field.Value.AttributeI("DefaultValue").sValue,
                        sFormat = field.Value.AttributeI("Format").sValue,
                    });
                }
                return dictpfs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CResult PFPreValidations(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                var sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int PolicyID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyID}")); //22962;//22957; // Loading PolicyID from cache  oCache.Get_ParamVal(sSessionID, sGUID, null, ""{XIP|iACPolicyID}"");
                int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();//8100; //oParams.Where(t => t.sName == "iQSIID").FirstOrDefault();
                string sSuccessStep = oParams.Where(m => m.sName.ToLower() == "SuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sFailureStep = oParams.Where(m => m.sName.ToLower() == "FailureStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
                // Load PF Fields Object
                Dictionary<string, XIPFfields> dictpfs = GetFormatedPFfields(EnumXIBNPMethods.NewBusiness);
                // Validate input request object here
                Dictionary<string, XIPFAttribute> XIpfvalues = new Dictionary<string, XIPFAttribute>();
                if (oQSInstance != null)
                {
                    XIpfvalues = oQSInstance.XIValues.Where(x => x.Key != "TitlePF").Select(d => new { skey = d.Key, svalue = d.Value.sValue }).ToDictionary(x => x.skey, x => new XIPFAttribute { sValue = x.svalue });
                    if (oQSInstance.XIValues.ContainsKey("TitlePF"))
                    {
                        XIpfvalues.Add("TitlePF", oQSInstance.XIValues.Where(x => x.Key == "TitlePF").Select(d => new XIPFAttribute { sValue = d.Value.sDerivedValue }).FirstOrDefault());
                    }
                }
                XIpfvalues = SetPolicyDetails(PolicyID, iQSIID, XIpfvalues);
                XIpfvalues = SetDefaultValues(XIpfvalues, dictpfs);
                var isvalirequest = xiwrapper.IsValidInputObject(XIpfvalues, EnumXIBNPMethods.NewBusiness, dictpfs);

                var oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, oQSInstance.FKiQSDefinitionID.ToString());
                var StepDef = oQSD.Steps[sFailureStep];
                Dictionary<string, XIDFieldDefinition> FieldDefs = new Dictionary<string, XIDFieldDefinition>();
                foreach (var Section in StepDef.Sections)
                {
                    foreach (var item in Section.Value.FieldDefs)
                    {
                        FieldDefs.Add(item.Key, item.Value);
                    }
                }
                foreach (var item in dictpfs.Select(x => x.Key).ToList())
                {
                    if (FieldDefs.Select(x => x.Key).ToList().Contains(item))
                    {
                    }
                    else
                    {
                        dictpfs.Remove(item);
                    }
                }
                string skey = "StepPreValidationMessage";
                Dictionary<string, List<string>> dictMsgs = isvalirequest.ToDictionary(d => d.Key, t => t.Value.Select(y => y.Description).ToList()); //TO DO
                bool ismatched = Utility.IsMatchedwithAny(dictMsgs.Select(d => d.Key).ToList(), dictpfs.Select(d => d.Key).ToList());
                if (ismatched) // IF ERRORS EXIT THEN LOG INTO THE QS OBJECT
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sFailureStep, null, null);
                    oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                }
                else
                {
                    dictMsgs = new Dictionary<string, List<string>>();
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                    oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                }
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        private XIIBO GetPFSchemeDetails(int SchemeID, string productcode)
        {
            // build query here
            XIIBO oXiBo = new XIIBO();
            QueryEngine oQE = new QueryEngine();
            string sWhereCondition = null;
            sWhereCondition = SchemeID > 0 ? "id=" + SchemeID : "sSchemeRefCode=" + productcode;
            var oQResult = oQE.Execute_QueryEngine("PFScheme_T", "sshopcode,id,FKiSupplierID,sSchemeRefCode", sWhereCondition);
            if (oQResult.bOK && oQResult.oResult != null)
            {
                oXiBo = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.FirstOrDefault();
            }
            //string sresult = oXiBo.AttributeI("sshopcode").sValue;
            return oXiBo;
        }

        public CResult PFNewBusiness(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFCResult oPFresult = new XIPFCResult();
            try
            {
                //XIInfraCache oCache = new XIInfraCache();
                xiwrapper = new XIBNPFileWrapperLive();
                // dynamic xibnpapi = GetInstance<XIBNPAPI>();
                //string RequestMode = ConfigurationManager.AppSettings["BNPRequestMode"];
                //if (RequestMode == "Mock")
                //{
                //    xibnpapi = GetInstance<XIMockAPI>();
                //}
                var sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int PolicyID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyID}")); //22962;//22957; // Loading PolicyID from cache  oCache.Get_ParamVal(sSessionID, sGUID, null, ""{XIP|iACPolicyID}"");
                int PolicyVersionID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyVersionID}"));
                int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();//8100; //oParams.Where(t => t.sName == "iQSIID").FirstOrDefault();
                FKsUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sDataBase = oParams.Where(m => m.sName.ToLower() == "sDataBase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSuccessStep = oParams.Where(m => m.sName.ToLower() == "SuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sFailureStep = oParams.Where(m => m.sName.ToLower() == "FailureStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sQuoteID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|FkiQuoteID}");
                oParams.Add(new CNV { sName = "iQuoteID", sValue = sQuoteID });
                string sTranstype = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|TransactionType}");
                oParams.Add(new CNV { sName = "sTranstype", sValue = sTranstype });
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
                // Load PF Fields Object
                Dictionary<string, XIPFfields> dictpfs = GetFormatedPFfields(EnumXIBNPMethods.NewBusiness);
                // Validate input request object here
                Dictionary<string, XIPFAttribute> XIpfvalues = new Dictionary<string, XIPFAttribute>();
                if (oQSInstance != null)
                {
                    XIpfvalues = oQSInstance.XIValues.Where(x => x.Key != "TitlePF").Select(d => new { skey = d.Key, svalue = d.Value.sValue }).ToDictionary(x => x.skey, x => new XIPFAttribute { sValue = x.svalue });
                    if (oQSInstance.XIValues.ContainsKey("TitlePF"))
                    {
                        XIpfvalues.Add("TitlePF", oQSInstance.XIValues.Where(x => x.Key == "TitlePF").Select(d => new XIPFAttribute { sValue = d.Value.sDerivedValue }).FirstOrDefault());
                    }
                }
                XIpfvalues = SetPolicyDetails(PolicyID, iQSIID, XIpfvalues);
                if (!string.IsNullOrEmpty(sTranstype) && sTranstype.ToLower() == "renewal")
                    XIpfvalues.Add("sTypeCode", new XIPFAttribute { sValue = "PFRN" });
                else if (!string.IsNullOrEmpty(sTranstype) && sTranstype.ToLower() == "rebroke")
                    XIpfvalues.Add("sTypeCode", new XIPFAttribute { sValue = "PFRB" });
                else
                    XIpfvalues.Add("sTypeCode", new XIPFAttribute { sValue = "PFNB" });
                XIpfvalues.Add("PolicyVersionID", new XIPFAttribute { sValue = PolicyVersionID.ToString() });
                XIpfvalues = SetDefaultValues(XIpfvalues, dictpfs);
                var isvalirequest = xiwrapper.IsValidInputObject(XIpfvalues, EnumXIBNPMethods.NewBusiness, dictpfs);
                if (isvalirequest.Count() <= 0)
                {
                    oPFresult.bprevalidation = true;
                    XIPFValues xiqs = new XIPFValues();
                    xiqs.XIValues = XIpfvalues;
                    /*GET SHOP CODE HERE FROM DATABASE USING PRODUCTCODE*/
                    string productcode = xiqs.GetXIPFValue("CreditProductCodePF").sValue;
                    string sShopCode = null;
                    if (!string.IsNullOrEmpty(productcode))
                    {
                        var PFSchemeDatails = GetPFSchemeDetails(XIpfvalues["FKiPFSchemeIDPF"].iValue, productcode);
                        sShopCode = PFSchemeDatails.AttributeI("sshopcode").sValue;
                        xiqs.XIValues.Add("PFSchemeID", new XIPFAttribute { sValue = PFSchemeDatails.AttributeI("id").sValue });
                        xiqs.XIValues.Add("FKiSupplierID", new XIPFAttribute { sValue = PFSchemeDatails.AttributeI("FKiSupplierID").sValue });
                        xiqs.XIValues.Remove("CreditProductCodePF");
                        xiqs.XIValues.Add("CreditProductCodePF", new XIPFAttribute { sValue = PFSchemeDatails.AttributeI("sSchemeRefCode").sValue });
                    }
                    xiqs.XIValues.Add("sQuoteID", new XIPFAttribute { sValue = sQuoteID });

                    var InsertApplication = InsertPFApplication(xiqs, 0, 1, sSessionID, sGUID);
                    //BUILDING REQUEST OBJECT 
                    //Random generator = new Random();
                    //String r = generator.Next(0, 9999).ToString();
                    if (InsertApplication.bOK == true)
                    {
                        BrokerReference = xiqs.GetXIPFValue("PolicyID").sValue + "I" + ((XIIBO)InsertApplication.oResult).AttributeI("id").sValue; //+ xiqs.GetXIPFValue("FKiSupplierID").sValue + r;// DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    }

                    var _req = xiwrapper.BuildRequestObject(xiqs, EnumXIBNPMethods.NewBusiness, BrokerReference, dictpfs);
                    if (_req != null)
                    {
                        NewBusinessRequest request = new NewBusinessRequest();
                        request = (NewBusinessRequest)_req;
                        //request.Header = GetHeader();
                        //request.Header.UserCredential.Password = "****";
                        string jsonreq = new JavaScriptSerializer().Serialize(request);
                        xiqs.XIValues.Add("sRequestObject", new XIPFAttribute { sValue = jsonreq });
                        InsertPFApplication(xiqs, ((XIIBO)InsertApplication.oResult).AttributeI("id").iValue, 1);
                        // INSERTING REQUEST OBJECT INTO BO HERE
                        //Dictionary<string, string> dictinsett = new Dictionary<string, string>();
                        //dictinsett.Add("sRequestObject", jsonreq);
                        //dictinsett.Add("sResponseObject", "");
                        //dictinsett.Add("sType", "json");
                        //dictinsett.Add("sRequestedService", EnumXIBNPMethods.NewBusiness.ToString());
                        //var cresult = InsertRequestObject(dictinsett);
                        //XIIBO pfbnprequest = new XIIBO();
                        //if (cresult.bOK)
                        //{
                        //    var opfbnprequest = (CResult)cresult.oResult;
                        //    pfbnprequest = (XIIBO)opfbnprequest.oResult;
                        //}
                        //CALL BNP SERVICE HERE
                        NewBusinessResponse response = new NewBusinessResponse();
                        response = xibnpapi.XINewBusiness(request, sShopCode);
                        JavaScriptSerializer Ojson = new JavaScriptSerializer();
                        Ojson.MaxJsonLength = Int32.MaxValue;
                        string strjsonreq = Ojson.Serialize(request);

                        XIIBO oboiPFReq = new XIIBO();
                        XIDXI oxidACT = new XIDXI();
                        XIDBO obodPFReq = (XIDBO)oxidACT.Get_BODefinition("PFRequestResponses_T").oResult;
                        oboiPFReq.BOD = obodPFReq;
                        oboiPFReq.SetAttribute("FKiACPolicyID", PolicyID.ToString());
                        oboiPFReq.SetAttribute("sRequest", strjsonreq);
                        Ojson = new JavaScriptSerializer();
                        Ojson.MaxJsonLength = Int32.MaxValue;
                        string strjsonres = Ojson.Serialize(response);
                        oboiPFReq.SetAttribute("sResponse", strjsonres.ToString());
                        oCResult = oboiPFReq.Save(oboiPFReq);
                        if (!oCResult.bOK && oCResult.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: While inserting record into PFRequestResponses_T table.";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            XIInstanceBase oIB = new XIInstanceBase();
                            oIB.SaveErrortoDB(oCResult);
                        }
                        Dictionary<string, string> dictMsgs = new Dictionary<string, string>();
                        string skey = "StepMessage";
                        if (response.Errors != null)
                        {
                            foreach (var item in response.Errors)
                            {
                                dictMsgs.Add(item.Code.ToString(), item.Description);
                            }
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sFailureStep, null, null);
                            oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                            string sResponseStatus = "";
                            if (response.NewBusinessResponseType != null)
                            {
                                sResponseStatus = Convert.ToInt32(response.NewBusinessResponseType.Decision).ToString();
                                xiqs.XIValues.Add("sPFAccountNo", new XIPFAttribute { sValue = response.NewBusinessResponseType.AccountNumber });
                                //xiqs.XIValues.Add("sRequestObject", new XIPFAttribute { sValue = jsonreq });
                                xiqs.XIValues.Add("iPFDecision", new XIPFAttribute { sValue = sResponseStatus });
                                xiqs.XIValues.Add("sPFDecision", new XIPFAttribute { sValue = response.NewBusinessResponseType.Decision.ToString() });
                                xiqs.XIValues.Add("sPFResponseStatus", new XIPFAttribute { sValue = response.NewBusinessResponseType.ResponseStatus.ToString() });
                                xiqs.XIValues.Add("sPFPartnerTransactionToken", new XIPFAttribute { sValue = response.Header.PartnerTransactionToken });
                                xiqs.XIValues.Add("iPFResponseType", new XIPFAttribute { sValue = Convert.ToInt32(response.NewBusinessResponseType.ResponseStatus).ToString() });
                                //Saving pdf file in the physical path
                                string spdffile = "";
                                if (!string.IsNullOrEmpty(response.NewBusinessResponseType.Pdf))
                                {
                                    var pdfbytes = Convert.FromBase64String(response.NewBusinessResponseType.Pdf);
                                    string filepath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Attachments/PremiumFinance/");
                                    string spolicynumber = request.NewBusinessRequestType.Policy.PolicyNumber;
                                    string sfilename = response.NewBusinessResponseType.AccountNumber;
                                    if (!string.IsNullOrEmpty(spolicynumber))
                                    {
                                        sfilename = spolicynumber + "_" + sfilename;
                                    }
                                    spdffile = Utility.SaveFile(filepath, sfilename, EnumFileExtensions.pdf.ToString(), response.NewBusinessResponseType.Pdf);
                                    response.NewBusinessResponseType.Pdf = spdffile;
                                }
                                if (spdffile.Contains("Fail"))
                                {
                                    // Log Error Failed to Save Pdf file
                                    spdffile = "";
                                }
                                Ojson = new JavaScriptSerializer();
                                Ojson.MaxJsonLength = Int32.MaxValue;
                                string strjsonresponse = Ojson.Serialize(response);
                                xiqs.XIValues.Add("sResponseObject", new XIPFAttribute { sValue = strjsonresponse });
                                xiqs.XIValues.Add("sPFPdffilepath", new XIPFAttribute { sValue = spdffile });
                            }
                            InsertPFApplication(xiqs, ((XIIBO)InsertApplication.oResult).AttributeI("id").iValue, 1);
                        }
                        else
                        {
                            dictMsgs = new Dictionary<string, string>();
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                            oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);

                            string sResponseStatus = Convert.ToInt32(response.NewBusinessResponseType.Decision).ToString();
                            xiqs.XIValues.Add("sPFAccountNo", new XIPFAttribute { sValue = response.NewBusinessResponseType.AccountNumber });
                            //xiqs.XIValues.Add("sRequestObject", new XIPFAttribute { sValue = jsonreq });
                            xiqs.XIValues.Add("iPFDecision", new XIPFAttribute { sValue = sResponseStatus });
                            xiqs.XIValues.Add("sPFDecision", new XIPFAttribute { sValue = response.NewBusinessResponseType.Decision.ToString() });
                            xiqs.XIValues.Add("sPFResponseStatus", new XIPFAttribute { sValue = response.NewBusinessResponseType.ResponseStatus.ToString() });
                            xiqs.XIValues.Add("sPFPartnerTransactionToken", new XIPFAttribute { sValue = response.Header.PartnerTransactionToken });
                            xiqs.XIValues.Add("iPFResponseType", new XIPFAttribute { sValue = Convert.ToInt32(response.NewBusinessResponseType.ResponseStatus).ToString() });
                            //Saving pdf file in the physical path
                            string spdffile = "";
                            if (!string.IsNullOrEmpty(response.NewBusinessResponseType.Pdf))
                            {
                                var pdfbytes = Convert.FromBase64String(response.NewBusinessResponseType.Pdf);
                                string filepath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Attachments/PremiumFinance/");
                                string spolicynumber = request.NewBusinessRequestType.Policy.PolicyNumber;
                                string sfilename = response.NewBusinessResponseType.AccountNumber;
                                if (!string.IsNullOrEmpty(spolicynumber))
                                {
                                    sfilename = spolicynumber + "_" + sfilename;
                                }
                                spdffile = Utility.SaveFile(filepath, sfilename, EnumFileExtensions.pdf.ToString(), response.NewBusinessResponseType.Pdf);
                                response.NewBusinessResponseType.Pdf = spdffile;
                            }
                            if (spdffile.Contains("Fail"))
                            {
                                // Log Error Failed to Save Pdf file
                                spdffile = "";
                            }
                            Ojson = new JavaScriptSerializer();
                            Ojson.MaxJsonLength = Int32.MaxValue;
                            string strjsonresponse = Ojson.Serialize(response);
                            xiqs.XIValues.Add("sResponseObject", new XIPFAttribute { sValue = strjsonresponse });
                            xiqs.XIValues.Add("sPFPdffilepath", new XIPFAttribute { sValue = spdffile });

                            //Update response object in Table
                            //Dictionary<string, string> dictupdate = new Dictionary<string, string>();
                            //dictupdate.Add("id", pfbnprequest.AttributeI("id").sValue);
                            //dictupdate.Add("sResponseObject", strjsonresponse);
                            //dictupdate.Add("sPFPdffilepath", spdffile);
                            //dictupdate.Add("sPFPartnerTransactionToken", response.Header.PartnerTransactionToken);
                            //UpdateRequestObject(dictupdate);
                            //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oPFresult.OResult = response;

                            if (response.NewBusinessResponseType.ResponseStatus == EnumsResponseStatus.Success && response.NewBusinessResponseType.Decision == EnumsDecision.Accepted)
                            {
                                //* DO CAPTURE DATA INTO DATABASE ACPFApplication_T Insert,ACPolicy_T Update, ACTransaction_T Insert
                                PFDBTransactions(xiqs, EnumXIBNPMethods.NewBusiness, ((XIIBO)InsertApplication.oResult).AttributeI("id").iValue, sTranstype); //   New Business Finance
                                oParams.Add(new CNV { sName = "iPaymentMethodType", sValue = "20" });
                                oParams.Add(new CNV { sName = "FKiPolicyVersionID", sValue = PolicyVersionID.ToString() });
                                oParams.Add(new CNV { sName = "iACPolicyID", sValue = PolicyID.ToString() });
                                oParams.Add(new CNV { sName = "sPolicyNo", sValue = xiqs.GetXIPFValue("sPolicyNo").sValue });
                                oParams.Add(new CNV { sName = "iQSInstanceID", sValue = iQSIID.ToString() });
                                ZeeInsurance.Policy oPolicy = new ZeeInsurance.Policy();
                                Thread threadObj = new Thread(new ThreadStart(() => { oPolicy.ThreadRunMethods(oParams); }));
                                threadObj.Start();
                            }
                            else
                            {
                                PremiumfinanceTransactionsAsync(xiqs, sTranstype);
                                XIIBO oTransaction = oXI.BOI("ACTransaction_T", sTransactionID);
                                oTransaction.SetAttribute(XIConstant.Key_XIDeleted, "1");
                                oCResult = oTransaction.Save(oTransaction);
                                InsertPFApplication(xiqs, ((XIIBO)InsertApplication.oResult).AttributeI("id").iValue, 0);
                                InsertPFMTAApplication(xiqs);
                                oCache.Set_ObjectSetCache(sSessionID, "Decision", sGUID, Convert.ToInt32(response.NewBusinessResponseType.Decision));
                            }
                        }
                    }
                }
                else // Capturing Error here
                {
                    // INSERTING REQUEST OBJECT INTO BO HERE
                    //string jsonreq = new JavaScriptSerializer().Serialize(XIpfvalues);
                    //string jsonresponse = new JavaScriptSerializer().Serialize(isvalirequest);
                    //Dictionary<string, string> dictinsett = new Dictionary<string, string>();
                    //dictinsett.Add("sRequestObject", jsonreq);
                    //dictinsett.Add("sResponseObject", jsonresponse);
                    //dictinsett.Add("sType", "json");
                    //dictinsett.Add("sRequestedService", EnumXIBNPMethods.NewBusiness.ToString());
                    //var cresult = InsertRequestObject(dictinsett);
                    //XIIBO pfbnprequest = new XIIBO();
                    //if (cresult.bOK)
                    //{
                    //    var opfbnprequest = (CResult)cresult.oResult;
                    //    pfbnprequest = (XIIBO)opfbnprequest.oResult;
                    //}
                    oCResult.sMessage = "ERROR:  " + System.Reflection.MethodBase.GetCurrentMethod().Name;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oPFresult.OResult = isvalirequest;
                }
                SetQSResponse(EnumXIBNPMethods.NewBusiness, oPFresult, oCache, sGUID, sSessionID);
            }
            catch (Exception ex)
            {
                oPFresult.IsException = true;
                oPFresult.xiErrorMessage = ex.Message.ToString();
                oPFresult.xiStatus = EnumxiStatus.error.ToString();
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        private void SetResponseError(EnumsResponseStatus input, Dictionary<string, string> dictQSresp, string sDecision = null)
        {
            if (input.ToString() == EnumsResponseStatus.Success.ToString() && sDecision == "Accepted")
            {
                dictQSresp.Add("Success", "PF Application Posted Successfully");
            }
            else
            {
                dictQSresp.Add("Error", "Refered PF Application to Post");
            }
        }
        private void SetQSResponse(EnumXIBNPMethods enumximethod, XIPFCResult result, XIInfraCache cacheobj, string sGUID, string sSessionID)
        {
            string skey = "StepMessage";
            Dictionary<string, string> dictMsgs = new Dictionary<string, string>();
            if (!result.IsException)
            {
                if (!result.bprevalidation)
                {
                    // Set Error Messages 
                    var errors = (Dictionary<string, List<Error>>)result.OResult;
                    if (errors != null)
                    {
                        //var lsterrors = errors.Values.Select(d => d.Select(u => u.Description)).ToList();
                        dictMsgs = errors.SelectMany(e => e.Value.Where(z => z.Description != "").Distinct().Select(v => v.Description)).Distinct().ToDictionary(x => x, x => x);
                    }
                }
                else
                {
                    dynamic dynObject = result.OResult;
                    Error[] error = dynObject.Errors;
                    error = error ?? new Error[] { };
                    if (error == null || error.Count() <= 0)
                    {
                        switch (enumximethod)
                        {
                            case EnumXIBNPMethods.NewBusiness:
                                {
                                    var resp = (NewBusinessResponse)result.OResult;
                                    if (resp.NewBusinessResponseType.Decision != EnumsDecision.Accepted)
                                        dictMsgs.Add("Error", resp.NewBusinessResponseType.Decision + " PF Application to Post");
                                    else
                                        SetResponseError(resp.NewBusinessResponseType.ResponseStatus, dictMsgs, resp.NewBusinessResponseType.Decision.ToString());
                                    break;
                                }
                            case EnumXIBNPMethods.MidTermAdjustment:
                                {
                                    var resp = (MidTermAdjustmentResponse)result.OResult;
                                    if (resp.MidTermAdjustmentResponseType.Decision != EnumsDecision.Accepted)
                                        dictMsgs.Add("Error", resp.MidTermAdjustmentResponseType.Decision + " PF MTA Application to Post");
                                    else
                                        dictMsgs.Add(resp.MidTermAdjustmentResponseType.ResponseStatus.ToString(), "Customer Finance is Edited Successfully");
                                    //SetResponseError(resp.MidTermAdjustmentResponseType.ResponseStatus, dictMsgs, resp.MidTermAdjustmentResponseType.Decision.ToString());
                                    break;
                                }
                            case EnumXIBNPMethods.Cancellation:
                                {
                                    var resp = (CancellationResponse)result.OResult;
                                    dictMsgs.Add(resp.CancellationResponseType.ResponseStatus.ToString(), "Premium Finance Cancelled Successfully");
                                    //SetResponseError(resp.CancellationResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                            case EnumXIBNPMethods.EditCustomerAddress:
                                {
                                    var resp = (EditCustomerAddressResponse)result.OResult;
                                    dictMsgs.Add(resp.EditCustomerAddressResponseType.ResponseStatus.ToString(), "Customer Address is Edited Successfully");
                                    //SetResponseError(resp.EditCustomerAddressResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                            case EnumXIBNPMethods.EditCustomerBank:
                                {
                                    var resp = (EditCustomerBankResponse)result.OResult;
                                    dictMsgs.Add(resp.EditCustomerBankResponseType.ResponseStatus.ToString(), "Customer Bank details is Edited Successfully");
                                    //SetResponseError(resp.EditCustomerBankResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                            case EnumXIBNPMethods.EditCustomerEmail:
                                {
                                    var resp = (EditCustomerEmailResponse)result.OResult;
                                    dictMsgs.Add(resp.EditCustomerEmailResponseType.ResponseStatus.ToString(), "Customer Email is Edited Successfully");
                                    //SetResponseError(resp.EditCustomerEmailResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                            case EnumXIBNPMethods.EditCustomerPaymentDay:
                                {
                                    var resp = (EditCustomerPaymentDayResponse)result.OResult;
                                    dictMsgs.Add(resp.EditCustomerPaymentDayResponseType.ResponseStatus.ToString(), "Customer Payment day Edited Successfully");
                                    //SetResponseError(resp.EditCustomerPaymentDayResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                            case EnumXIBNPMethods.EditCustomerPhone:
                                {
                                    var resp = (EditCustomerPhoneResponse)result.OResult;
                                    dictMsgs.Add(resp.EditCustomerPhoneResponseType.ResponseStatus.ToString(), "Customer Phone Number Edited Successfully");
                                    //SetResponseError(resp.EditCustomerPhoneResponseType.ResponseStatus, dictMsgs);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        if (error[0] == null)
                        {
                            dictMsgs.Add("Error", "Your Application is not Accepted.");
                        }
                        else
                        {
                            dictMsgs = error.Select(v => v.Description).ToDictionary(x => x, x => x);
                        }
                    }
                }
            }
            else
            {
                dictMsgs.Add("Error", " Error occured");
            }
            if (dictMsgs.Count() > 0)
            {
                cacheobj.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
            }
        }
        #endregion

        #region InsertUPDATE_Request_ResponseObject
        public CResult InsertRequestObject(Dictionary<string, string> InputObject)
        {
            try
            {
                StringBuilder strreqbuilder = new StringBuilder();
                strreqbuilder.Append(InputObject["sRequestObject"].Trim());
                CResult oCResult = new CResult();
                XIIBO oboiACT = new XIIBO();
                XIDXI oxidACT = new XIDXI();
                XIDBO obodACT = (XIDBO)oxidACT.Get_BODefinition("PFBNPRequests_T").oResult;
                oboiACT.BOD = obodACT;
                oboiACT.SetAttribute("sRequestObject", strreqbuilder.ToString());
                oboiACT.SetAttribute("sResponseObject", InputObject["sResponseObject"]);
                oboiACT.SetAttribute("sType", InputObject["sType"]);
                oboiACT.SetAttribute("sRequestedService", InputObject["sRequestedService"]);
                var Inserted = oboiACT.Save(oboiACT);
                if (!Inserted.bOK && Inserted.oResult == null)
                {
                    XIInstanceBase oIB = new XIInstanceBase();
                    oCResult.sMessage = "ERROR: PFBNPRequests_T Insert Failed \r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Inserted;
                return oCResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CResult UpdateRequestObject(Dictionary<string, string> InputObject)
        {
            try
            {
                CResult oCResult = new CResult();
                XIIBO oboiPol = new XIIBO();
                XIDXI oxidPol = new XIDXI();
                XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("PFBNPRequests_T").oResult;
                oboiPol.BOD = obodPol;
                oboiPol.SetAttribute("id", InputObject["id"].ToString());
                oboiPol.SetAttribute("sResponseObject", InputObject["sResponseObject"].ToString());
                oboiPol.SetAttribute("sPartnerTransactionToken", InputObject["sPFPartnerTransactionToken"].ToString());
                oboiPol.SetAttribute("sPdffilepath", InputObject["sPFPdffilepath"].ToString());
                var Updated = oboiPol.Save(oboiPol);
                if (!Updated.bOK && Updated.oResult == null)
                {
                    XIInstanceBase oIB = new XIInstanceBase();
                    oCResult.sMessage = "ERROR: PFBNPRequests_T Update Failed \r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Updated;
                return oCResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region PFBNPFieldSData
        private CResult GetPFBNPFields(EnumXIBNPMethods input)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                //XIInfraCache oCache = new XIInfraCache();
                var oOneClick = (XID1Click)oCache.GetObjectFromCache("oneclick", "PFBNPFields", "");
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache("bo", null, oOneClick.BOID.ToString());
                oOneClick.BOD = oBOD;
                XIDXI oDXI = new XIDXI();
                List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                //Dictionary<string, XIIBO> oDictionaryBOI = null;
                XIDStructure oStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "{XIP|" + input.ToString() + "}", sValue = input.ToString() });
                var sDataSource = oDXI.GetBODataSource(oBOD.iDataSource, oBOD.FKiApplicationID);
                oOneClick.sConnectionString = sDataSource;
                oOneClick.BOID = oOneClick.BOID;
                oOneClick.Query = "SELECT id AS 'id',sFormat AS 'Format',sDefaultValue AS 'DefaultValue', sXIFieldName AS 'XIFieldName', sfDisplayName AS 'fDisplayName', sType AS 'Type', sServiceName AS 'ServiceName', bIsMandatory AS 'IsMandatory',iMinLength AS 'MinLength',iMaxLength AS 'MaxLength',sRegexp AS 'Regexp', sErrorMessage AS 'ErrorMessage',sNotation AS 'Notation' FROM PFBNPFields_T WHERE sServiceName='" + input.ToString() + "'";
                //oOneClick.Query = oStructure.ReplaceExpressionWithCacheValue(oOneClick.Query, nParams);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oOneClick.OneClick_Execute();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion

        private string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }
        public XIPFCResult LoadXIBNPAPI(EnumXIBNPMethods enummethod, string xmlinput)
        {
            try
            {
                var pfbnpfields = GetPFBNPFields(enummethod);
                var reqesult = (Dictionary<string, XIIBO>)pfbnpfields.oResult;
                Dictionary<string, XIPFfields> _dictpfs = new Dictionary<string, XIPFfields>();
                foreach (var field in reqesult)
                {
                    _dictpfs.Add(field.Value.AttributeI("XIFieldName").sValue, new XIPFfields
                    {
                        bMandatory = field.Value.AttributeI("IsMandatory").bValue,
                        sType = field.Value.AttributeI("Type").sValue,
                        iMinlength = field.Value.AttributeI("MinLength").iValue,
                        iMaxlength = field.Value.AttributeI("MaxLength").iValue,
                        sNotation = field.Value.AttributeI("Notation").sValue,
                        sErrorMessage = field.Value.AttributeI("ErrorMessage").sValue,
                        sRegexp = field.Value.AttributeI("Regexp").sValue,
                        sXIFieldName = field.Value.AttributeI("XIFieldName").sValue,
                        sfDisplayName = field.Value.AttributeI("fDisplayName").sValue,
                        sFormat = field.Value.AttributeI("sFormat").sValue,
                    });
                }
                var result = xiwrapper.LoadXIBNPAPI(enummethod, xmlinput, _dictpfs);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region PFDBTransactions
        private void PremiumfinanceTransactionsAsync(XIPFValues dictQSfields, string sTransType = null, int XIDeleted = 0)
        {
            try
            {
                var context = System.Web.HttpContext.Current;
                int PolicyID = dictQSfields.GetXIPFValue("PolicyID").iValue;
                //Thread threadObj = new Thread(new ThreadStart(() =>
                //{
                CallContext.HostContext = context;
                var rTotalPF = dictQSfields.GetXIPFValue("PFAmount").dblValue;
                double rTotalPFTrans = rTotalPF;
                string sTransCode = "PFNB";
                if (rTotalPF < 0)
                {
                    rTotalPFTrans = (rTotalPF * -1);
                    sTransCode = "PFRP";
                }
                if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "renewal")
                {
                    sTransCode = "PFRN";
                }
                else if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "rebroke")
                {
                    sTransCode = "PFRB";
                }
                XIIXI oXII = new XIIXI();
                XIIBO oBOIPolicy = oXII.BOI("ACPolicy_T", PolicyID.ToString(), "refAccountCategory");
                JournalTransactions oJTransactions = new JournalTransactions();
                XIIBO oTransaction = new XIIBO();
                oTransaction.SetAttribute("zBaseValue", rTotalPFTrans.ToString());
                oTransaction.SetAttribute(XIConstant.Key_XIDeleted, XIDeleted.ToString());
                oTransaction.SetAttribute("FKiACPolicyID", PolicyID.ToString());
                oTransaction.SetAttribute("FKiSupplierID", dictQSfields.GetXIPFValue("FKiSupplierID").sValue);
                oTransaction.SetAttribute("iType", "20");
                oTransaction.SetAttribute("FKiPFSchemeID", dictQSfields.GetXIPFValue("PFSchemeID").sValue);
                oTransaction.SetAttribute("FKiCustomerID", dictQSfields.GetXIPFValue("FKiCustomerID").sValue);
                oTransaction.SetAttribute("FKiProductID", dictQSfields.GetXIPFValue("FKiProductID").sValue);
                oTransaction.SetAttribute("sTransCode", sTransCode);
                oTransaction.SetAttribute("iPostedFrom", "10");
                oTransaction.SetAttribute("sName", "Premium Finance setup in QuestionSet");
                oTransaction.SetAttribute("zCommission", "0");
                oTransaction.SetAttribute("zDefaultAdmin", "0");
                oTransaction.SetAttribute("zDefaultDeposit", "0");
                oTransaction.SetAttribute("rAdmin", "0");
                oTransaction.SetAttribute("iEDIStatus", "10");
                oTransaction.SetAttribute("FKsWhoID", FKsUserID);
                oTransaction.SetAttribute("refAccountCategory", oBOIPolicy.AttributeI("refAccountCategory").sValue);
                var PFTransaction = oJTransactions.PostTransaction(oTransaction, sDataBase);
                sTransactionID = ((XIIBO)PFTransaction.oResult).AttributeI("ID").sValue;
                oJTransactions.Update_PolicyBalance(PolicyID, 10);
                oJTransactions.Update_PolicyBalance(PolicyID, 10, 10);
                // }));
                //threadObj.SetApartmentState(ApartmentState.MTA);
                //threadObj.IsBackground = true;
                //threadObj.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CResult InsertPFApplication(XIPFValues dictQSfields, int PFApplicationID, int iSDeleted, string sSessionID = null, string sGUID = null)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                XIIXI oXiiPO = new XIIXI();
                var oPol = oXiiPO.BOI("ACPolicy_T", dictQSfields.GetXIPFValue("PolicyID").sValue, "FKiCustomerID");
                int FKiClientID = 0;
                if (oPol != null)
                {
                    FKiClientID = oPol.AttributeI("FKiCustomerID").iValue;
                }
                XIIBO oboiACT = new XIIBO();
                XIDXI oxidACT = new XIDXI();
                XIDBO obodACT = (XIDBO)oxidACT.Get_BODefinition("ACPFApplication_T").oResult;
                oboiACT.BOD = obodACT;
                if (PFApplicationID > 0)
                {
                    oboiACT.SetAttribute("id", PFApplicationID.ToString());
                }
                oboiACT.SetAttribute(XIConstant.Key_XIDeleted, iSDeleted.ToString());
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPolicyNo").sValue))
                {
                    oboiACT.SetAttribute("sPolicyNo", dictQSfields.GetXIPFValue("sPolicyNo").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("FirstNamePF").sValue))
                {
                    oboiACT.SetAttribute("sFirstName", dictQSfields.GetXIPFValue("FirstNamePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("LastNamePF").sValue))
                {
                    oboiACT.SetAttribute("sLastName", dictQSfields.GetXIPFValue("LastNamePF").sValue);
                    oboiACT.SetAttribute("sName", dictQSfields.GetXIPFValue("FirstNamePF").sValue + " " + dictQSfields.GetXIPFValue("LastNamePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PostCodePF").sValue))
                {
                    oboiACT.SetAttribute("sPostCode", dictQSfields.GetXIPFValue("PostCodePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("LastNamePF").sValue))
                {
                    oboiACT.SetAttribute("dDOB", dictQSfields.GetXIPFValue("DateOfBirthPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PreferredPaymentDayPF").sValue))
                {
                    int InceptionDay = Convert.ToDateTime(dictQSfields.GetXIPFValue("dCoverStart").sValue).Day;
                    oboiACT.SetAttribute("sPaymentDay", dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue == 0 ?
                        InceptionDay > 28 ? "28" : InceptionDay.ToString() : dictQSfields.GetXIPFValue("PreferredPaymentDayPF").sValue);
                    oboiACT.SetAttribute("sFormattedPaymentDay", (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue == 0 || dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue > 28) ?
                        (InceptionDay > 28 ? "28th" : InceptionDay.ToString() +
                                   ((InceptionDay % 10 == 1 && InceptionDay != 11) ? "st"
                                   : (InceptionDay % 10 == 2 && InceptionDay != 12) ? "nd"
                                   : (InceptionDay % 10 == 3 && InceptionDay != 13) ? "rd"
                                   : "th"))
                                   : dictQSfields.GetXIPFValue("PreferredPaymentDayPF").sValue + ((dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 1 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 11) ? "st"
                                   : (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 2 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 12) ? "nd"
                                   : (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 3 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 13) ? "rd"
                                   : "th"));
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("EmailPF").sValue))
                {
                    oboiACT.SetAttribute("sEMail", dictQSfields.GetXIPFValue("EmailPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sRequestObject").sValue))
                {
                    oboiACT.SetAttribute("sRequestObject", dictQSfields.GetXIPFValue("sRequestObject").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sResponseObject").sValue))
                {
                    oboiACT.SetAttribute("sResponseObject", dictQSfields.GetXIPFValue("sResponseObject").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PolicyID").sValue))
                {
                    oboiACT.SetAttribute("FKiACPolicyID", dictQSfields.GetXIPFValue("PolicyID").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sTypeCode").sValue))
                {
                    oboiACT.SetAttribute("sTypeCode", dictQSfields.GetXIPFValue("sTypeCode").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("FKiSupplierID").sValue))
                {
                    oboiACT.SetAttribute("FKiSupplierID", dictQSfields.GetXIPFValue("FKiSupplierID").sValue);
                }
                if (FKiClientID != 0)
                {
                    oboiACT.SetAttribute("FKiClientID", FKiClientID.ToString());
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("TotalCashPricePF").sValue))
                {
                    oboiACT.SetAttribute("rSumTotalPremium", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("DepositPF").sValue))
                {
                    oboiACT.SetAttribute("rSumDepositAmount", dictQSfields.GetXIPFValue("DepositPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("InterestPF").sValue))
                {
                    oboiACT.SetAttribute("rSumInterestPayable", dictQSfields.GetXIPFValue("InterestPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("InterestRatePF").sValue))
                {
                    oboiACT.SetAttribute("rSumInterestRate", dictQSfields.GetXIPFValue("InterestRatePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("InstallmentPF").sValue))
                {
                    oboiACT.SetAttribute("rSumInstallmentAmount", dictQSfields.GetXIPFValue("InstallmentPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("TotalCashPricePF").sValue))
                {
                    oboiACT.SetAttribute("rSumTotalPayable", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("TotalCashPricePF").sValue))
                {
                    oboiACT.SetAttribute("rSumTotalPayableToPFCo", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PFSchemeID").sValue))
                {
                    oboiACT.SetAttribute("FKiPFSchemeID", dictQSfields.GetXIPFValue("PFSchemeID").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPFPartnerTransactionToken").sValue))
                {
                    oboiACT.SetAttribute("sPartnerTransactionToken", dictQSfields.GetXIPFValue("sPFPartnerTransactionToken").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("iPFDecision").sValue) && !dictQSfields.XIValues.ContainsKey("iMTAPFDecision"))
                {
                    oboiACT.SetAttribute("iType", dictQSfields.GetXIPFValue("iPFDecision").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("iMTAPFDecision").sValue))
                {
                    oboiACT.SetAttribute("iMTAType", dictQSfields.GetXIPFValue("iMTAPFDecision").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPFAccountNo").sValue))
                {
                    oboiACT.SetAttribute("sSupplierPFAccountNo", dictQSfields.GetXIPFValue("sPFAccountNo").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPFPdffilepath").sValue))
                {
                    oboiACT.SetAttribute("sPdffilepath", dictQSfields.GetXIPFValue("sPFPdffilepath").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("iPFResponseType").sValue))
                {
                    oboiACT.SetAttribute("iResponseType", dictQSfields.GetXIPFValue("iPFResponseType").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("TitlePF").sValue))
                {
                    oboiACT.SetAttribute("sTitle", dictQSfields.GetXIPFValue("TitlePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("dCoverStart").sValue))
                {
                    oboiACT.SetAttribute("dDateProcessed", DateTime.Now.ToString());
                }
                oboiACT.SetAttribute("dSumPaymentDate", DateTime.Now.Day <= 28 ? DateTime.Now.ToString() : "28-" + DateTime.Now.ToString("MMM") + "-"
                    + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToString("hh:MM:ss tt"));
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("BankAccountNumberPF").sValue))
                {
                    oboiACT.SetAttribute("iBankAccNo", dictQSfields.GetXIPFValue("BankAccountNumberPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sAccountName").sValue))
                {
                    oboiACT.SetAttribute("sBankAccName", dictQSfields.GetXIPFValue("sAccountName").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("BankSortCodePF").sValue))
                {
                    oboiACT.SetAttribute("iSortCode1", dictQSfields.GetXIPFValue("BankSortCodePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sBankName").sValue))
                {
                    oboiACT.SetAttribute("sBankName", dictQSfields.GetXIPFValue("sBankName").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("HouseNamePF").sValue))
                {
                    oboiACT.SetAttribute("sAdd1", dictQSfields.GetXIPFValue("HouseNamePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("HouseNumberPF").sValue))
                {
                    oboiACT.SetAttribute("sAdd2", dictQSfields.GetXIPFValue("HouseNumberPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("StreetNamePF").sValue))
                {
                    oboiACT.SetAttribute("sAdd3", dictQSfields.GetXIPFValue("StreetNamePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PostTownPF").sValue))
                {
                    oboiACT.SetAttribute("sAdd4", dictQSfields.GetXIPFValue("PostTownPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("CountyPF").sValue))
                {
                    oboiACT.SetAttribute("sAdd5", dictQSfields.GetXIPFValue("CountyPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PostCodePF").sValue))
                {
                    oboiACT.SetAttribute("sPostCode", dictQSfields.GetXIPFValue("PostCodePF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("MobileNumberPF").sValue))
                {
                    oboiACT.SetAttribute("sTelH", dictQSfields.GetXIPFValue("MobileNumberPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PhoneNumberPF").sValue))
                {
                    oboiACT.SetAttribute("sTelW", dictQSfields.GetXIPFValue("PhoneNumberPF").sValue);
                }
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("PFAmount").sValue))
                {
                    oboiACT.SetAttribute("rPFAmount", dictQSfields.GetXIPFValue("PFAmount").sValue);
                }
                oboiACT.SetAttribute("sBrokerReference", BrokerReference);
                oCResult = oboiACT.Save(oboiACT);
                oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ACPFApplication_T.id}", ((XIIBO)oCResult.oResult).AttributeI("id").sValue, null, null);
                if (!string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPFAccountNo").sValue) && string.IsNullOrEmpty(dictQSfields.GetXIPFValue("sPFPdffilepath").sValue))
                {
                    XIIBO oboiPol = new XIIBO();
                    XIDXI oxidPol = new XIDXI();
                    XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                    oboiPol.BOD = obodPol;
                    oboiPol.SetAttribute("id", dictQSfields.GetXIPFValue("PolicyID").sValue);
                    oboiPol.SetAttribute("iPFStatus", "40");
                    oboiPol.SetAttribute("sPFAccountNo", dictQSfields.GetXIPFValue("sPFAccountNo").sValue);
                    oboiPol.SetAttribute("FKiPFApplicationID", ((XIIBO)oCResult.oResult).AttributeI("id").sValue);
                    oboiPol.SetAttribute("iStatus", "5");
                    var Updated = oboiPol.Save(oboiPol);
                    if (!Updated.bOK && Updated.oResult == null)
                    {
                        oCResult.sMessage = "ERROR: ACPolicy_T PF Update Failed \r\n" + dictQSfields.GetXIPFValue("PolicyID").sValue;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
            }
            catch (Exception e)
            {
                oCResult.sMessage = "ERROR:  ACPFApplication_T " + System.Reflection.MethodBase.GetCurrentMethod().Name + e;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR:  ACPFApplication_T " + System.Reflection.MethodBase.GetCurrentMethod().Name + e;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        private CResult InsertPFMTAApplication(XIPFValues dictQSfields)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                XIIXI oXiiPO = new XIIXI();
                var oPol = oXiiPO.BOI("ACPolicy_T", dictQSfields.GetXIPFValue("PolicyID").sValue, "FKiCustomerID");
                int FKiClientID = 0;
                if (oPol != null)
                {
                    FKiClientID = oPol.AttributeI("FKiCustomerID").iValue;
                }
                XIIBO oboiACT = new XIIBO();
                XIDXI oxidACT = new XIDXI();
                XIDBO obodACT = (XIDBO)oxidACT.Get_BODefinition("ACPFMTAApplication_T").oResult;
                oboiACT.BOD = obodACT;
                oboiACT.SetAttribute("sPolicyNo", dictQSfields.GetXIPFValue("sPolicyNo").sValue);
                oboiACT.SetAttribute("sFirstName", dictQSfields.GetXIPFValue("FirstNamePF").sValue);
                oboiACT.SetAttribute("sLastName", dictQSfields.GetXIPFValue("LastNamePF").sValue);
                oboiACT.SetAttribute("sName", dictQSfields.GetXIPFValue("FirstNamePF").sValue + " " + dictQSfields.GetXIPFValue("LastNamePF").sValue);
                oboiACT.SetAttribute("sPostCode", dictQSfields.GetXIPFValue("PostCodePF").sValue);
                oboiACT.SetAttribute("dDOB", dictQSfields.GetXIPFValue("DateOfBirthPF").sValue);
                oboiACT.SetAttribute("sEMail", dictQSfields.GetXIPFValue("EmailPF").sValue);
                oboiACT.SetAttribute("sMob", dictQSfields.GetXIPFValue("MobileNumberPF").sValue);
                oboiACT.SetAttribute("sRequestObject", dictQSfields.GetXIPFValue("sRequestObject").sValue);
                oboiACT.SetAttribute("sResponseObject", dictQSfields.GetXIPFValue("sResponseObject").sValue);
                oboiACT.SetAttribute("FKiACPolicyID", dictQSfields.GetXIPFValue("PolicyID").sValue);
                oboiACT.SetAttribute("sTypeCode", dictQSfields.GetXIPFValue("sTypeCode").sValue);
                oboiACT.SetAttribute("FKiClientID", FKiClientID.ToString());
                oboiACT.SetAttribute("rSumTotalPremium", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                oboiACT.SetAttribute("rSumDepositAmount", dictQSfields.GetXIPFValue("DepositPF").sValue);
                oboiACT.SetAttribute("rSumInterestPayable", dictQSfields.GetXIPFValue("InterestPF").sValue);
                oboiACT.SetAttribute("rSumInterestRate", dictQSfields.GetXIPFValue("InterestRatePF").sValue);
                oboiACT.SetAttribute("rSumInstallmentAmount", dictQSfields.GetXIPFValue("InstallmentPF").sValue);
                oboiACT.SetAttribute("rSumTotalPayable", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                oboiACT.SetAttribute("rSumTotalPayableToPFCo", dictQSfields.GetXIPFValue("TotalCashPricePF").sValue);
                oboiACT.SetAttribute("FKiPFSchemeID", dictQSfields.GetXIPFValue("PFSchemeID").sValue);
                oboiACT.SetAttribute("sPartnerTransactionToken", dictQSfields.GetXIPFValue("sPFPartnerTransactionToken").sValue);
                oboiACT.SetAttribute("iType", dictQSfields.GetXIPFValue("iPFDecision").sValue);
                oboiACT.SetAttribute("sSupplierPFAccountNo", dictQSfields.GetXIPFValue("sPFAccountNo").sValue);
                oboiACT.SetAttribute("sPdffilepath", dictQSfields.GetXIPFValue("sPFPdffilepath").sValue);
                oboiACT.SetAttribute("iResponseType", dictQSfields.GetXIPFValue("iPFResponseType").sValue);
                oboiACT.SetAttribute("sTitle", dictQSfields.GetXIPFValue("TitlePF").sValue);
                oboiACT.SetAttribute("dDateProcessed", DateTime.Now.ToString());
                oboiACT.SetAttribute("dSumPaymentDate", DateTime.Now.ToString());
                oboiACT.SetAttribute("iBankAccNo", dictQSfields.GetXIPFValue("BankAccountNumberPF").sValue);
                oboiACT.SetAttribute("sBankAccName", dictQSfields.GetXIPFValue("sAccountName").sValue);
                oboiACT.SetAttribute("iSortCode1", dictQSfields.GetXIPFValue("BankSortCodePF").sValue);
                oboiACT.SetAttribute("sBankName", dictQSfields.GetXIPFValue("sBankName").sValue);
                oboiACT.SetAttribute("sAdd1", dictQSfields.GetXIPFValue("HouseNamePF").sValue);
                oboiACT.SetAttribute("sAdd2", dictQSfields.GetXIPFValue("HouseNumberPF").sValue);
                oboiACT.SetAttribute("sAdd3", dictQSfields.GetXIPFValue("StreetNamePF").sValue);
                oboiACT.SetAttribute("sAdd4", dictQSfields.GetXIPFValue("PostTownPF").sValue);
                oboiACT.SetAttribute("sAdd5", dictQSfields.GetXIPFValue("CountyPF").sValue);
                oboiACT.SetAttribute("sPostCode", dictQSfields.GetXIPFValue("PostCodePF").sValue);
                oboiACT.SetAttribute("sTelH", dictQSfields.GetXIPFValue("MobileNumberPF").sValue);
                oboiACT.SetAttribute("sTelW", dictQSfields.GetXIPFValue("PhoneNumberPF").sValue);
                oboiACT.SetAttribute("rPFAmount", dictQSfields.GetXIPFValue("PFAmount").sValue);
                oboiACT.SetAttribute("FKiSupplierID", dictQSfields.GetXIPFValue("FKiSupplierID").sValue);
                int InceptionDay = Convert.ToDateTime(dictQSfields.GetXIPFValue("dCoverStart").sValue).Day;
                oboiACT.SetAttribute("sPaymentDay", dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue == 0 ?
                    InceptionDay > 28 ? "28" : InceptionDay.ToString() : dictQSfields.GetXIPFValue("PreferredPaymentDayPF").sValue);
                oboiACT.SetAttribute("sFormattedPaymentDay", (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue == 0 || dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue > 28) ?
                    (InceptionDay > 28 ? "28th" : InceptionDay.ToString() +
                                   ((InceptionDay % 10 == 1 && InceptionDay != 11) ? "st"
                                   : (InceptionDay % 10 == 2 && InceptionDay != 12) ? "nd"
                                   : (InceptionDay % 10 == 3 && InceptionDay != 13) ? "rd"
                                   : "th"))
                                   : dictQSfields.GetXIPFValue("PreferredPaymentDayPF").sValue + ((dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 1 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 11) ? "st"
                               : (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 2 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 12) ? "nd"
                               : (dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue % 10 == 3 && dictQSfields.GetXIPFValue("PreferredPaymentDayPF").iValue != 13) ? "rd"
                               : "th"));
                oCResult = oboiACT.Save(oboiACT);
            }
            catch (Exception e)
            {
                oCResult.sMessage = "ERROR:  ACPFMTAApplication_T " + System.Reflection.MethodBase.GetCurrentMethod().Name + e;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR:  ACPFMTAApplication_T " + System.Reflection.MethodBase.GetCurrentMethod().Name + e;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        private CResult PFDBTransactions(XIPFValues dictQSfields, EnumXIBNPMethods enummethod, int PfApplicationID, string sTransType = null) //PFNB
        {
            // Insert into ACPFApplication_T
            XIInstanceBase oIB = new XIInstanceBase();
            int PolicyID = dictQSfields.GetXIPFValue("PolicyID").iValue;
            int PolicyVersionID = dictQSfields.GetXIPFValue("PolicyVersionID").iValue;
            CResult oCResult = new CResult();
            oCResult.sClassName = oCResult.Get_Class();
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            XIIBO oboiPol = new XIIBO();
            XIDXI oxidPol = new XIDXI();
            XIIXI oIXI = new XIIXI();
            var oProductI = oIXI.BOI("Product", dictQSfields.GetXIPFValue("FKiProductID").sValue);
            ZeeInsurance.Policy oPolicy = new ZeeInsurance.Policy();
            var oQuoteI = oIXI.BOI("Aggregations", dictQSfields.GetXIPFValue("sQuoteID").sValue);
            string sPolicyNo = null;
            if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "renewal")
            {
                sPolicyNo = dictQSfields.GetXIPFValue("PolicyNumberPF").sValue;
            }
            else
            {
                var Result = oPolicy.PolicyNoGeneration(oProductI);
                if (Result.bOK && Result.oResult != null)
                {
                    sPolicyNo = (string)Result.oResult;
                }
            }

            dictQSfields.XIValues.Add("sPolicyNo", new XIPFAttribute { sValue = sPolicyNo });
            var Inserted = InsertPFApplication(dictQSfields, PfApplicationID, 0); //Insert into ACPFApplication
            if (!Inserted.bOK && Inserted.oResult == null)
            {
                oCResult.sMessage = "ERROR: ACPFApplication_T Insert Failed \r\n PolicyID" + dictQSfields.GetXIPFValue("PolicyID").sValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            var MTAInserted = InsertPFMTAApplication(dictQSfields); //Insert into ACPFMTAApplication
            if (!MTAInserted.bOK && MTAInserted.oResult == null)
            {
                oCResult.sMessage = "ERROR: ACPFMTAApplication_T Insert Failed \r\n PolicyID" + dictQSfields.GetXIPFValue("PolicyID").sValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            XIIBO insertedPFApp = new XIIBO();
            if (Inserted.bOK)
            {
                insertedPFApp = (XIIBO)Inserted.oResult;
            }
            var rTotalPF = dictQSfields.GetXIPFValue("PFAmount").dblValue;
            #region UPDATE_POLICY
            // UPDATE INTO ACPolicy_T rPFTotal,sPFAccountNo,FKiPFApplicationID      
            XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
            oboiPol.BOD = obodPol;
            oboiPol.SetAttribute("id", PolicyID.ToString());
            oboiPol.SetAttribute("iPFStatus", "10");
            oboiPol.SetAttribute("sPFAccountNo", dictQSfields.GetXIPFValue("sPFAccountNo").sValue);
            oboiPol.SetAttribute("FKiPFApplicationID", insertedPFApp.AttributeI("id").sValue);
            oboiPol.SetAttribute("rPFTotal", rTotalPF.ToString()); // Total PF Amount
            oboiPol.SetAttribute("sPolicyNo", sPolicyNo);
            if (string.IsNullOrEmpty(sTransType) || (!string.IsNullOrEmpty(sTransType) && (sTransType.ToLower() != "renewal" && sTransType.ToLower() != "rebroke")))
            {
                oboiPol.SetAttribute("iStatus", "10");
            }
            else if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "renewal")
            {
                oboiPol.SetAttribute("iPolicyStatus", "25");
            }
            else if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "rebroke")
            {
                oboiPol.SetAttribute("iPolicyStatus", "50");
            }
            oboiPol.SetAttribute("iVersionNo", PolicyVersionID.ToString());
            oboiPol.SetAttribute("rBalance", "0");
            var Updated = oboiPol.Save(oboiPol);
            if (!Updated.bOK && Updated.oResult == null)
            {
                oCResult.sMessage = "ERROR: ACPolicy_T PF Update Failed \r\n" + PolicyID.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            // Update Transactions
            List<XIIBO> oTRBOI = new List<XIIBO>();
            XIDXI oTXI = new XIDXI();
            List<XIWhereParams> oWParams = new List<XIWhereParams>();
            List<SqlParameter> oSQLParams = new List<SqlParameter>();
            QueryEngine oQE = new QueryEngine();
            oWParams.Add(new XIWhereParams { sField = "FKiACPolicyID", sOperator = "=", sValue = PolicyID.ToString() });
            oSQLParams.Add(new SqlParameter { ParameterName = "@FKiACPolicyID", Value = PolicyID.ToString() });
            oWParams.Add(new XIWhereParams { sField = XIConstant.Key_XIDeleted, sOperator = "=", sValue = "1" });
            oSQLParams.Add(new SqlParameter { ParameterName = XIConstant.Key_XIDeleted, Value = "1" });
            oQE.AddBO("ACTransaction_T", "id," + XIConstant.Key_XIDeleted + ",FKsPolicyNo", oWParams);
            oCResult = oQE.BuildQuery();
            if (oCResult.bOK && oCResult.oResult != null)
            {
                var sSql = (string)oCResult.oResult;
                ExecutionEngine oEE = new ExecutionEngine();
                oEE.XIDataSource = oQE.XIDataSource;
                oEE.sSQL = sSql;
                oEE.SqlParams = oSQLParams;
                var oQResult = oEE.Execute();
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oTRBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                }
            }
            if (oTRBOI == null)
            {
                oCResult.sMessage = oCResult.sFunctionName + ", Unable to DB2Collection Transactions ";//i.zX.trace.formatStack
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile(); oCResult.sMessage = oCResult.sFunctionName + ", Unable to DB2Collection Transactions ";//i.zX.trace.formatStack
                oIB.SaveErrortoDB(oCResult);
            }
            else
            {
                var BOD = (XIDBO)oTXI.Get_BODefinition("ACTransaction_T").oResult;
                foreach (var item in oTRBOI)
                {
                    item.BOD = BOD;
                    item.SetAttribute(XIConstant.Key_XIDeleted, "0");
                    item.SetAttribute("FKsPolicyNo", sPolicyNo);
                    var res = item.Save(item);
                    if (!res.bOK && res.oResult == null)
                    {
                        oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
            }
            #endregion
            #region UPDATE_POLICYVersion
            // UPDATE INTO ACPolicy_T rPFTotal,sPFAccountNo,FKiPFApplicationID      
            XIDBO obodPolversion = (XIDBO)oxidPol.Get_BODefinition("ACPolicyVersion_T").oResult;
            XIIBO PolicyVersionI = new XIIBO();
            PolicyVersionI.BOD = obodPolversion;
            PolicyVersionI.SetAttribute("id", PolicyVersionID.ToString());
            if (string.IsNullOrEmpty(sTransType) || (!string.IsNullOrEmpty(sTransType) && (sTransType.ToLower() != "renewal" && sTransType.ToLower() != "rebroke")))
            {
                PolicyVersionI.SetAttribute("iPolicyStatus", "10");
            }
            else if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "renewal")
            {
                PolicyVersionI.SetAttribute("iPolicyStatus", "25");
            }
            else if (!string.IsNullOrEmpty(sTransType) && sTransType.ToLower() == "rebroke")
            {
                PolicyVersionI.SetAttribute("iPolicyStatus", "50");
            }
            var oPolicyVersionI = oboiPol.Save(PolicyVersionI);
            if (!oPolicyVersionI.bOK && oPolicyVersionI.oResult == null)
            {
                oCResult.sMessage = "ERROR: ACPolicyVersion_T PF Update Failed \r\n" + PolicyID.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }

            #endregion
            #region TRANSACTIONS
            PremiumfinanceTransactionsAsync(dictQSfields, sTransType);
            #endregion
            #region UPDATE_ACPFApplication_T
            //UPDATING ISTATUS IN ACPFApplication_T TO 10
            XIIBO uoboiPol = new XIIBO();
            XIDXI uoxidPol = new XIDXI();
            XIDBO uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
            uoboiPol.BOD = uobodPol;
            uoboiPol.SetAttribute("id", insertedPFApp.AttributeI("id").sValue);
            uoboiPol.SetAttribute("iStatus", "10");
            var PFApUpdated = uoboiPol.Save(uoboiPol);
            if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
            {
                oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status Failed. Policy Create id=" + insertedPFApp.AttributeI("id").sValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            #endregion
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;
        }
        #endregion
        #region Accept Decline PF Manually
        public CResult AcceptPF(List<CNV> oParams)
        {
            XIPFCResult xiresult = new XIPFCResult();
            XIInstanceBase oIB = new XIInstanceBase();
            int PolicyID = oParams.Where(t => t.sName == "PolicyID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            int ACPFApplicationID = oParams.Where(t => t.sName == "ACPFApplicationID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            string sPFTypeCode = oParams.Where(t => t.sName == "sPFTypeCode").Select(f => f.sValue).FirstOrDefault();
            FKsUserID = oParams.Where(t => t.sName == "iUserID").Select(f => f.sValue).FirstOrDefault();
            sDataBase = oParams.Where(m => m.sName.ToLower() == "sDataBase".ToLower()).Select(m => m.sValue).FirstOrDefault();

            var sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
            string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
            int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            string sSuccessStep = oParams.Where(m => m.sName.ToLower() == "SuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
            XIIXI oXI = new XIIXI();
            var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
            Dictionary<string, string> dictMsgs = new Dictionary<string, string>();
            string skey = "StepMessage";
            var oPol = oXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString(), "iStatus,dSumPaymentDate,sTypeCode,iType");
            CResult oCResult = new CResult();
            try
            {
                XIIBO oboiPol = new XIIBO();
                XIDXI oxidPol = new XIDXI();
                XIIBO uoboiPol = new XIIBO();
                XIDXI uoxidPol = new XIDXI();
                XIDBO uobodPol = new XIDBO();
                XIIXI oIXI = new XIIXI();
                List<XIIBO> oTRBOI = new List<XIIBO>();
                XIDXI oTXI = new XIDXI();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                QueryEngine oQE = new QueryEngine();
                if (sPFTypeCode == "Accept")
                {
                    if (oPol.Attributes["sTypeCode"].sValue == "PFMTA" && oPol.Attributes["iType"].iValue == 4)
                    {
                        uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                        uoboiPol.BOD = uobodPol;
                        uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                        uoboiPol.SetAttribute("iStatus", "10");
                        uoboiPol.SetAttribute("iType", "1");
                        var PFApUpdated = uoboiPol.Save(uoboiPol);
                        if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status Posted. Policy Create id=" + ACPFApplicationID.ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oIB.SaveErrortoDB(oCResult);
                        }
                        XIIBO oBOIMTAPF = new XIIBO();
                        uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFMTAApplication_T").oResult;
                        oBOIMTAPF = oIXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                        oBOIMTAPF.BOD = uobodPol;
                        oBOIMTAPF.Exclude("id");
                        oBOIMTAPF.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                        var PFMTA = oBOIMTAPF.Save(oBOIMTAPF);
                        if (!PFMTA.bOK && PFMTA.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: Premium Finance MTA Application Posted. Policy Create id=" + ACPFApplicationID.ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oIB.SaveErrortoDB(oCResult);
                        }
                        oTRBOI = new List<XIIBO>();
                        oTXI = new XIDXI();
                        oWParams = new List<XIWhereParams>();
                        List <SqlParameter> oSQLParams = new List<SqlParameter>();
                        oQE = new QueryEngine();
                        oWParams.Add(new XIWhereParams { sField = "FKiACPolicyID", sOperator = "=", sValue = PolicyID.ToString() });
                        oWParams.Add(new XIWhereParams { sField = XIConstant.Key_XIDeleted, sOperator = "=", sValue = "1" });
                        oSQLParams.Add(new SqlParameter { ParameterName = "@FKiACPolicyID", Value = PolicyID.ToString() });
                        oSQLParams.Add(new SqlParameter { ParameterName = XIConstant.Key_XIDeleted, Value = "1" });
                        oQE.AddBO("ACTransaction_T", "id," + XIConstant.Key_XIDeleted, oWParams);
                        oCResult = oQE.BuildQuery();
                        if (oCResult.bOK && oCResult.oResult != null)
                        {
                            var sSql = (string)oCResult.oResult;
                            ExecutionEngine oEE = new ExecutionEngine();
                            oEE.XIDataSource = oQE.XIDataSource;
                            oEE.sSQL = sSql;
                            oEE.SqlParams = oSQLParams;
                            var oQResult = oEE.Execute();
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                oTRBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                            }
                        }
                        if (oTRBOI == null)
                        {
                            oCResult.sMessage = oCResult.sFunctionName + ", Unable to DB2Collection Transactions ";//i.zX.trace.formatStack
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                        }
                        var BOD = (XIDBO)oTXI.Get_BODefinition("ACTransaction_T").oResult;
                        foreach (var item in oTRBOI)
                        {
                            item.BOD = BOD;
                            item.SetAttribute(XIConstant.Key_XIDeleted, "0");
                            var res = item.Save(item);
                            if (!res.bOK && res.oResult == null)
                            {
                                oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                                oIB.SaveErrortoDB(oCResult);
                            }
                        }
                        dictMsgs.Add("10", "PF MTA Application Accepted.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["iStatus"].iValue == 10)
                    {
                        dictMsgs.Add("10", "Already PF Application Posted.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["iStatus"].iValue == 20)
                    {
                        dictMsgs.Add("20", "Already PF Application Cancelled.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["sTypeCode"].sValue != "PFMTA" && (oPol.Attributes["iType"].iValue == 2 || oPol.Attributes["iType"].iValue == 4))
                    {
                        var oPolicyI = oXI.BOI("ACPolicy_T", PolicyID.ToString(), "FKiProductID,FKiCustomerID,FKiQuoteID,iStatus,sPolicyNo");
                        if (string.IsNullOrEmpty(oPolicyI.AttributeI("sPolicyNo").sValue))
                        {
                            var oQuoteI = oXI.BOI("Aggregations", oPolicyI.AttributeI("FKiQuoteID").sValue, "iQuoteStatus");
                            var oProductI = oXI.BOI("Product", oPolicyI.AttributeI("FKiProductID").sValue);
                            ZeeInsurance.Policy oPolicy = new ZeeInsurance.Policy();
                            string sPolicyNo = null;
                            var Result = oPolicy.PolicyNoGeneration(oProductI);
                            if (Result.bOK && Result.oResult != null)
                            {
                                sPolicyNo = (string)Result.oResult;
                            }
                            CNV Param = new CNV();
                            Param.sName = "iProductID";
                            Param.sValue = oPolicyI.AttributeI("FKiProductID").sValue;
                            oParams.Add(Param);
                            Param = new CNV();
                            Param.sName = "sPolicyNo";
                            Param.sValue = sPolicyNo;
                            oParams.Add(Param);
                            XIIBO oAggregation = new XIIBO();
                            oAggregation = oIXI.BOI("Aggregations", oPolicyI.AttributeI("FKiQuoteID").sValue, "FKiQSInstanceID");
                            Param = new CNV();
                            Param.sName = "iQSInstanceID";
                            Param.sValue = oAggregation.AttributeI("FKiQSInstanceID").sValue;
                            oParams.Add(Param);
                            Param = new CNV();
                            Param.sName = "sQSInstanceID";
                            Param.sValue = oAggregation.AttributeI("FKiQSInstanceID").sValue;
                            oParams.Add(Param);
                            var oPFApplicationI = oXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                            XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                            oboiPol.BOD = obodPol;
                            oboiPol.SetAttribute("id", PolicyID.ToString());
                            if (oPol.Attributes["iType"].iValue == 4)  //For Refered application
                            {
                                oboiPol.SetAttribute("iPFStatus", "10");
                                oboiPol.SetAttribute("sPFAccountNo", oPFApplicationI.AttributeI("sSupplierPFAccountNo").sValue);
                                oboiPol.SetAttribute("FKiPFApplicationID", ACPFApplicationID.ToString());
                                oboiPol.SetAttribute("rPFTotal", oPFApplicationI.AttributeI("rPFAmount").sValue); // Total PF Amount
                                oboiPol.SetAttribute("rBalance", "0");
                            }
                            else if (oPol.Attributes["iType"].iValue == 2)
                            {
                                oboiPol.SetAttribute("rPolicyBalance", oPFApplicationI.AttributeI("rPFAmount").sValue);
                            }
                            oboiPol.SetAttribute("sPolicyNo", sPolicyNo);
                            oboiPol.SetAttribute("iStatus", "10");
                            var Updated = oboiPol.Save(oboiPol);
                            if (!Updated.bOK && Updated.oResult == null)
                            {
                                oCResult.sMessage = "ERROR: ACPolicy_T PF Update Failed \r\n" + PolicyID.ToString();
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oIB.SaveErrortoDB(oCResult);
                            }
                            //Update Policy version
                            var QuoteID = oXI.BOI("ACPolicy_T", PolicyID.ToString(), "FKiQuoteID,iVersionNo");
                            XIDBO obodPolversion = (XIDBO)oxidPol.Get_BODefinition("ACPolicyVersion_T").oResult;
                            XIIBO PolicyVersionI = new XIIBO();
                            PolicyVersionI.BOD = obodPolversion;
                            PolicyVersionI.SetAttribute("id", QuoteID.AttributeI("iVersionNo").sValue);
                            PolicyVersionI.SetAttribute("iPolicyStatus", "10");
                            var oPolicyVersionI = oboiPol.Save(PolicyVersionI);
                            if (!oPolicyVersionI.bOK && oPolicyVersionI.oResult == null)
                            {
                                oCResult.sMessage = "ERROR: ACPolicyVersion_T PF Update Failed \r\n" + PolicyID.ToString();
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oIB.SaveErrortoDB(oCResult);
                            }
                            // Update Transactions
                            oTRBOI = new List<XIIBO>();
                            oTXI = new XIDXI();
                            oWParams = new List<XIWhereParams>();
                            oQE = new QueryEngine();
                            List<SqlParameter> oSQLParams = new List<SqlParameter>();
                            oWParams.Add(new XIWhereParams { sField = "FKiACPolicyID", sOperator = "=", sValue = PolicyID.ToString() });
                            oQE.AddBO("ACTransaction_T", "id," + XIConstant.Key_XIDeleted + ",FKsPolicyNo", oWParams);
                            oSQLParams.Add(new SqlParameter { ParameterName = "@FKiACPolicyID", Value = PolicyID.ToString() });
                            oSQLParams.Add(new SqlParameter { ParameterName = XIConstant.Key_XIDeleted, Value = "1" });
                            oCResult = oQE.BuildQuery();
                            if (oCResult.bOK && oCResult.oResult != null)
                            {
                                var sSql = (string)oCResult.oResult;
                                ExecutionEngine oEE = new ExecutionEngine();
                                oEE.XIDataSource = oQE.XIDataSource;
                                oEE.sSQL = sSql;
                                oEE.SqlParams = oSQLParams;
                                var oQResult = oEE.Execute();
                                if (oQResult.bOK && oQResult.oResult != null)
                                {
                                    oTRBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                                }
                            }
                            if (oTRBOI == null)
                            {
                                oCResult.sMessage = oCResult.sFunctionName + ", Unable to DB2Collection Transactions ";//i.zX.trace.formatStack
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                            }
                            var BOD = (XIDBO)oTXI.Get_BODefinition("ACTransaction_T").oResult;
                            foreach (var item in oTRBOI)
                            {
                                item.BOD = BOD;
                                item.SetAttribute(XIConstant.Key_XIDeleted, "0");
                                item.SetAttribute("FKsPolicyNo", sPolicyNo);
                                var res = item.Save(item);
                                if (!res.bOK && res.oResult == null)
                                {
                                    oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.LogToFile();
                                    oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                                    oIB.SaveErrortoDB(oCResult);
                                }
                            }
                            if (oPol.Attributes["iType"].iValue == 4)  //For Refered Application
                            {
                                #region UPDATE_ACPFApplication_T
                                //UPDATING ISTATUS IN ACPFApplication_T TO 10
                                int PaymentDay = Convert.ToDateTime(oPol.Attributes["dSumPaymentDate"].sValue).Day > 28 ? 28 : Convert.ToDateTime(oPol.Attributes["dSumPaymentDate"].sValue).Day;
                                uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                                uoboiPol.BOD = uobodPol;
                                uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                                uoboiPol.SetAttribute("iStatus", "10");
                                uoboiPol.SetAttribute("iType", "1");
                                uoboiPol.SetAttribute("sPaymentDay", string.IsNullOrEmpty(PaymentDay.ToString()) ?
                           DateTime.Now.Day > 28 ? "28" : DateTime.Now.Day.ToString() : PaymentDay.ToString());
                                uoboiPol.SetAttribute("sFormattedPaymentDay", (string.IsNullOrEmpty(PaymentDay.ToString()) || PaymentDay > 28) ?
                                    (DateTime.Now.Day > 28 ? "28th" : DateTime.Now.Day.ToString() +
                                               ((DateTime.Now.Day % 10 == 1 && DateTime.Now.Day != 11) ? "st"
                                               : (DateTime.Now.Day % 10 == 2 && DateTime.Now.Day != 12) ? "nd"
                                               : (DateTime.Now.Day % 10 == 3 && DateTime.Now.Day != 13) ? "rd"
                                               : "th"))
                                               : PaymentDay.ToString() + ((PaymentDay % 10 == 1 && PaymentDay != 11) ? "st"
                                               : (PaymentDay % 10 == 2 && PaymentDay != 12) ? "nd"
                                               : (PaymentDay % 10 == 3 && PaymentDay != 13) ? "rd"
                                               : "th"));
                                uoboiPol.SetAttribute("sPolicyNo", sPolicyNo);
                                var PFApUpdated = uoboiPol.Save(uoboiPol);
                                if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                                {
                                    oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status Posted. Policy Create id=" + ACPFApplicationID.ToString();
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.LogToFile();
                                    oIB.SaveErrortoDB(oCResult);
                                }
                                XIIBO oBOIMTAPF = new XIIBO();
                                uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFMTAApplication_T").oResult;
                                oBOIMTAPF = oIXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                                oBOIMTAPF.BOD = uobodPol;
                                oBOIMTAPF.Exclude("id");
                                oBOIMTAPF.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                                var PFMTA = oBOIMTAPF.Save(oBOIMTAPF);
                                if (!PFMTA.bOK && PFMTA.oResult == null)
                                {
                                    oCResult.sMessage = "ERROR: Premium Finance MTA Application Posted. Policy Create id=" + ACPFApplicationID.ToString();
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.LogToFile();
                                    oIB.SaveErrortoDB(oCResult);
                                }
                                #endregion
                                XIPFValues dictQSfields = new XIPFValues();
                                dictQSfields.XIValues.Add("PolicyID", new XIPFAttribute { sValue = PolicyID.ToString() });
                                dictQSfields.XIValues.Add("PFAmount", new XIPFAttribute { sValue = oPFApplicationI.AttributeI("rPFAmount").sValue });
                                dictQSfields.XIValues.Add("FKiSupplierID", new XIPFAttribute { sValue = oPFApplicationI.AttributeI("FKiSupplierID").sValue });
                                dictQSfields.XIValues.Add("PFSchemeID", new XIPFAttribute { sValue = oPFApplicationI.AttributeI("FKiPFSchemeID").sValue });
                                dictQSfields.XIValues.Add("FKiCustomerID", new XIPFAttribute { sValue = oPolicyI.AttributeI("FKiCustomerID").sValue });
                                dictQSfields.XIValues.Add("FKiProductID", new XIPFAttribute { sValue = oProductI.AttributeI("id").sValue });
                                PremiumfinanceTransactionsAsync(dictQSfields);
                            } //
                            string sTranstype = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|TransactionType}");
                            oParams.Add(new CNV { sName = "sTranstype", sValue = sTranstype });
                            oParams.Add(new CNV { sName = "iQuoteID", sValue = QuoteID.AttributeI("FKiQuoteID").sValue });
                            oParams.Add(new CNV { sName = "iPaymentMethodType", sValue = "20" });
                            ZeeInsurance.Policy objPolicy = new ZeeInsurance.Policy();
                            Thread threadObj = new Thread(new ThreadStart(() => { objPolicy.ThreadRunMethods(oParams); }));
                            threadObj.Start();
                            if (oPol.Attributes["iType"].iValue == 2)
                            {
                                dictMsgs.Add("10", "Policy Created successfully.");
                                oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                                oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                            }
                            else if (oPol.Attributes["iType"].iValue == 4)
                            {
                                dictMsgs.Add("10", "PF Application Posted successfully.");
                                oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                                oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                            }
                        }
                        else
                        {
                            dictMsgs.Add("10", "Policy Accepted.");
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                            oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                        }
                    }
                    else if (oPol.Attributes["sTypeCode"].sValue == "PFMTA")
                    {
                        dictMsgs.Add("10", "Already PF MTA Application Accepted.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                }
                else
                {
                    if (oPol.Attributes["sTypeCode"].sValue == "PFMTA" && oPol.Attributes["iType"].iValue == 4)
                    {
                        uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                        uoboiPol.BOD = uobodPol;
                        uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                        uoboiPol.SetAttribute("iStatus", "20");
                        uoboiPol.SetAttribute("iType", "2");
                        var PFApUpdated = uoboiPol.Save(uoboiPol);
                        if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status Posted. Policy Create id=" + ACPFApplicationID.ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oIB.SaveErrortoDB(oCResult);
                        }
                        XIIBO oBOIMTAPF = new XIIBO();
                        uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFMTAApplication_T").oResult;
                        oBOIMTAPF = oIXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                        oBOIMTAPF.BOD = uobodPol;
                        oBOIMTAPF.Exclude("id");
                        oBOIMTAPF.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                        var PFMTA = oBOIMTAPF.Save(oBOIMTAPF);
                        if (!PFMTA.bOK && PFMTA.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: Premium Finance MTA Application Posted. Policy Create id=" + ACPFApplicationID.ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oIB.SaveErrortoDB(oCResult);
                        }

                        dictMsgs.Add("10", "PF MTA Application Declined.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["iStatus"].iValue == 20)
                    {
                        dictMsgs.Add("20", "Already PF Application Cancelled.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["sTypeCode"].sValue != "PFMTA")
                    {
                        //UPDATING ISTATUS IN ACPFApplication_T TO 20
                        uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                        uoboiPol.BOD = uobodPol;
                        uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                        uoboiPol.SetAttribute("iStatus", "20");
                        uoboiPol.SetAttribute("iType", "2");
                        var PFApUpdated = uoboiPol.Save(uoboiPol);
                        if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status Cancelled. Policy Create id=" + ACPFApplicationID.ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oIB.SaveErrortoDB(oCResult);
                        }
                        dictMsgs.Add("20", "PF Application Cancelled.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else if (oPol.Attributes["sTypeCode"].sValue == "PFMTA")
                    {
                        dictMsgs.Add("20", "Already PF MTA Application Cancelled.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                xiresult.IsException = true;
                xiresult.OResult = ex;
                xiresult.xiStatus = EnumxiStatus.error.ToString();
                xiresult.xiErrorMessage = ex.Message.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion
        #region SetDefaultValues
        private Dictionary<string, XIPFAttribute> SetDefaultValues(Dictionary<string, XIPFAttribute> QSvalues, Dictionary<string, XIPFfields> dictfields)
        {
            try
            {
                // Load DEFAULT VALUES NOT NULL List here
                if (dictfields != null)
                {
                    foreach (var item in dictfields.Where(f => !string.IsNullOrEmpty(f.Value.sDefaultValue) || !string.IsNullOrEmpty(f.Value.sFormat)))
                    {
                        if (!string.IsNullOrEmpty(item.Value.sDefaultValue))
                        {
                            if (QSvalues.ContainsKey(item.Key)) //if value exist dont override with default value
                            {
                                if (string.IsNullOrEmpty(QSvalues[item.Key].sValue))
                                {
                                    QSvalues[item.Key].sValue = item.Value.sDefaultValue;
                                    //QSvalues.Add(item.Key, new XIPFAttribute { sValue = item.Value.sDefaultValue });
                                }
                                if (item.Value.sType.ToLower() == "system.int32" && QSvalues[item.Key].dValue.ToString() != "0001-01-01 12:00:00 AM")
                                {
                                    QSvalues[item.Key].sValue = QSvalues[item.Key].dValue.Day > 28 ? "28" : QSvalues[item.Key].dValue.Day.ToString();
                                }
                                else if (item.Value.sType.ToLower() == "system.int32" && QSvalues[item.Key].dValue.ToString() == "0001-01-01 12:00:00 AM")
                                {
                                    QSvalues[item.Key].sValue = DateTime.Now.Day.ToString();
                                }
                            }
                            else
                            {
                                QSvalues.Add(item.Key, new XIPFAttribute { sValue = item.Value.sDefaultValue });
                            }
                        }
                        // Format Values 
                        if (!string.IsNullOrEmpty(item.Value.sFormat))
                        {
                            if (QSvalues.ContainsKey(item.Key))
                            {
                                if (!string.IsNullOrEmpty(QSvalues[item.Key].sValue))
                                {
                                    if (QSvalues[item.Key].dValue != DateTime.MinValue)
                                    {
                                        QSvalues[item.Key].sValue = QSvalues[item.Key].dValue.ToString(item.Value.sFormat);
                                    }
                                }
                            }
                        }
                    }
                }
                return QSvalues;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region POLICYDETAILS_Method
        private Dictionary<string, XIPFAttribute> SetPolicyDetails(int PolicyID, int QSInstanceID, Dictionary<string, XIPFAttribute> QSfields)
        {
            try
            {
                XIIXI oIXI = new XIIXI();
                XIIBO oPolicy = new XIIBO(); //PFSchemeID
                oPolicy = oIXI.BOI("ACPolicy_T", PolicyID.ToString(), "FKiQuoteID,FKiProductID,FKiCustomerID,dCoverStart,rGrossPremium,sPolicyNo,rQuotePremium", null);
                if (oPolicy != null)
                {
                    QSfields.Add("PolicyID", new XIPFAttribute { sValue = PolicyID.ToString() });
                    QSfields.Add("PolicyPremiumAmountPF", new XIPFAttribute { sValue = oPolicy.AttributeI("rQuotePremium").sValue });
                    QSfields.Add("PolicyNumberPF", new XIPFAttribute { sValue = oPolicy.AttributeI("sPolicyNo").sValue });
                    QSfields.Add("FKiCustomerID", new XIPFAttribute { sValue = oPolicy.AttributeI("FKiCustomerID").sValue });
                    QSfields.Add("FKiProductID", new XIPFAttribute { sValue = oPolicy.AttributeI("FKiProductID").sValue });
                    List<CNV> Params = new List<CNV>();
                    Params.Add(new CNV { sName = "iStatus", sValue = "10" });
                    Params.Add(new CNV { sName = "FKiACPolicyID", sValue = PolicyID.ToString() });
                    XIIBO oBOI = oIXI.BOI("ACPolicyVersion_T", null, "dEffectiveFrom,dEffectiveTo", Params);
                    QSfields.Add("PolicyInceptionDatePF", new XIPFAttribute { sValue = oBOI.AttributeI("dEffectiveFrom").sValue });
                    var RenewalDate = Utility.ConvertToDate(oBOI.AttributeI("dEffectiveFrom").sValue);
                    QSfields.Add("PolicyRenewalDatePF", new XIPFAttribute { sValue = RenewalDate.AddYears(1).ToString() });
                    oIXI = new XIIXI();
                    XIIBO oAggregation = new XIIBO();
                    oAggregation = oIXI.BOI("Aggregations", oPolicy.AttributeI("FKiQuoteID").sValue,
                        "rPremiumFinanceAmount,sInsurer,rPrice,rTotal,zDefaultDeposit,rMonthlyTotal,rAddonPrice,rAddonAdmin,rMonthlyPrice,rInterestAmount,rInterestRate,rPaymentCharge,sAddonDescription,FKiPFSchemeID,rTotalAdmin,rFinalQuote,rInsurerCharge", null);
                    if (oAggregation != null)
                    {
                        //TotalCashPrice =Total cost of the policy Excluding Interest
                        QSfields.Add("TotalCashPricePF", new XIPFAttribute { sValue = oAggregation.AttributeI("rTotal").sValue });
                        QSfields.Add("DepositPF", new XIPFAttribute { sValue = oAggregation.AttributeI("zDefaultDeposit").sValue });
                        QSfields.Add("AddOnAmountPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rAddonPrice").sValue });
                        QSfields.Add("AddonAdminPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rAddonAdmin").sValue });
                        QSfields.Add("InstallmentPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rMonthlyPrice").sValue });
                        QSfields.Add("InterestPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rInterestAmount").sValue });
                        QSfields.Add("InterestRatePF", new XIPFAttribute { sValue = oAggregation.AttributeI("rInterestRate").sValue });
                        QSfields.Add("PFAmount", new XIPFAttribute { sValue = oAggregation.AttributeI("rPremiumFinanceAmount").sValue });
                        QSfields.Add("BrokerFeeAmountPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rPaymentCharge").sValue });
                        QSfields.Add("AddOnAmountDescriptionPF", new XIPFAttribute { sValue = oAggregation.AttributeI("sAddonDescription").sValue });
                        QSfields.Add("FKiPFSchemeIDPF", new XIPFAttribute { sValue = oAggregation.AttributeI("FKiPFSchemeID").sValue });
                        QSfields.Add("TotalAdminPF", new XIPFAttribute { sValue = oAggregation.AttributeI("rTotalAdmin").sValue });
                        QSfields.Add("InsurerChargePF", new XIPFAttribute { sValue = oAggregation.AttributeI("rInsurerCharge").sValue });
                    }

                    // ADD INSURER CODE
                    if (!string.IsNullOrEmpty(oPolicy.AttributeI("FKiProductID").sValue))
                    {
                        oIXI = new XIIXI();
                        XIIBO oProduct = new XIIBO();
                        oProduct = oIXI.BOI("Product", oPolicy.AttributeI("FKiProductID").sValue, "FKiSupplierID", null);
                        if (!string.IsNullOrEmpty(oProduct.AttributeI("FKiSupplierID").sValue))
                        {
                            // LOAD INSURER CODES BASED ON THE INSURER Supplier_T
                            oIXI = new XIIXI();
                            XIIBO oSupplier = new XIIBO();
                            oSupplier = oIXI.BOI("Supplier_T", oProduct.AttributeI("FKiSupplierID").sValue, "enumInsurerCodeID", null);
                            QSfields.Add("InsuranceCompanyPF", new XIPFAttribute { sValue = oSupplier.AttributeI("enumInsurerCodeID").sValue });
                        }
                    }
                }
                return QSfields;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region PolicyPopup_PFTx
        public CResult PFPolicyPopupBNPTX(List<CNV> oParams)
        {
            // * NOTE: Non Financial Transactions 
            // EditCustomerAddress = 7,EditCustomerBank = 8,EditCustomerEmail = 9,EditCustomerPaymentDay = 10,EditCustomerPhone = 11,
            XIPFCResult oCResult = new XIPFCResult();
            XIInstanceBase oIB = new XIInstanceBase();
            CResult xiresult = new CResult();
            bool IsResponse = false;
            try
            {
                EnumXIBNPMethods enumximethod = EnumXIBNPMethods.None;
                XIIXI oXI = new XIIXI();
                //dynamic xibnpapi = GetInstance<XIBNPAPI>();
                //string RequestMode = ConfigurationManager.AppSettings["BNPRequestMode"];
                //if (RequestMode == "Mock")
                //{
                //    xibnpapi = GetInstance<XIMockAPI>();
                //}
                var sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sType = oParams.Where(m => m.sName.ToLower() == "sType".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                int PolicyID = oParams.Where(t => t.sName == "PolicyID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                string sTypeCode = oParams.Where(m => m.sName.ToLower() == "sPFTypeCode".ToLower()).Select(m => m.sValue).FirstOrDefault();
                FKsUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int PFApplicationID = oParams.Where(t => t.sName == "ACPFApplicationID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                BrokerReference = oParams.Where(t => t.sName == "sBrokerReference").Select(f => f.sValue).FirstOrDefault();
                string sSuccessStep = oParams.Where(m => m.sName.ToLower() == "SuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sFailureStep = oParams.Where(m => m.sName.ToLower() == "FailureStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                sDataBase = oParams.Where(m => m.sName.ToLower() == "sDataBase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
                Dictionary<string, string> dictMsgs = new Dictionary<string, string>();
                string skey = "StepMessage";
                XIIXI oXII = new XIIXI();
                var oPol = oXII.BOI("ACPFApplication_T", PFApplicationID.ToString(), "iStatus,iType,sTypeCode");
                if (oPol.Attributes["iStatus"].iValue == 20)
                {
                    dictMsgs.Add("20", "Already PF Application Cancelled.");
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                    oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                }
                else if (oPol.Attributes["iType"].iValue == 1 || oPol.Attributes["sTypeCode"].sValue != "PFNB")
                {
                    if (Enum.IsDefined(typeof(EnumXIBNPMethods), sType))
                    {
                        enumximethod = (EnumXIBNPMethods)Enum.Parse(typeof(EnumXIBNPMethods), sType, true);
                    }
                    if (enumximethod.ToString() == EnumXIBNPMethods.None.ToString())
                    {
                        xiresult.sMessage = "ERROR: [" + xiresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] sType Key Value is Incorrect sType:" + sType;
                        xiresult.LogToFile();
                        oIB.SaveErrortoDB(xiresult);
                        return xiresult;
                    }
                    //Validate input request object here
                    Dictionary<string, XIPFAttribute> XIpfvalues = new Dictionary<string, XIPFAttribute>();
                    if (oQSInstance != null)
                    {
                        XIpfvalues = oQSInstance.XIValues.Where(x => x.Key != "TitlePF").Select(d => new { skey = d.Key, svalue = d.Value.sValue }).ToDictionary(x => x.skey, x => new XIPFAttribute { sValue = x.svalue });
                        if (oQSInstance.XIValues.ContainsKey("TitlePF"))
                        {
                            XIpfvalues.Add("TitlePF", oQSInstance.XIValues.Where(x => x.Key == "TitlePF").Select(d => new XIPFAttribute { sValue = d.Value.sDerivedValue }).FirstOrDefault());
                        }
                    }
                    Dictionary<string, XIPFfields> dictpfs = GetFormatedPFfields(enumximethod);
                    XIpfvalues = SetDefaultValues(XIpfvalues, dictpfs); // Setting Default Values

                    //Setting Values
                    XIpfvalues.Add("sTypeCode", new XIPFAttribute { sValue = sTypeCode });

                    var isvalirequest = xiwrapper.IsValidInputObject(XIpfvalues, enumximethod, dictpfs);
                    if (isvalirequest.Count() <= 0)
                    {
                        oCResult.bprevalidation = true;
                        XIPFValues xiqs = new XIPFValues();
                        xiqs.XIValues = XIpfvalues;
                        //BUILDING REQUEST OBJECT
                        //Random generator = new Random();
                        //String r = generator.Next(0, 9999).ToString();
                        //BrokerReference = BrokerReference.Substring(0, BrokerReference.Length - 4);
                        //BrokerReference = BrokerReference + r;// DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        dynamic _req = xiwrapper.BuildRequestObject(xiqs, enumximethod, BrokerReference, dictpfs);
                        if (_req != null)
                        {
                            //MidTermAdjustmentRequest request = new MidTermAdjustmentRequest();
                            //request = (MidTermAdjustmentRequest)_req;
                            string jsonreq = new JavaScriptSerializer().Serialize(_req);
                            int iRequestType = 0;
                            // INSERTING REQUEST OBJECT INTO BO HERE
                            //Dictionary<string, string> dictinsett = new Dictionary<string, string>();
                            //dictinsett.Add("sRequestObject", jsonreq);
                            //dictinsett.Add("sResponseObject", "");
                            //dictinsett.Add("sType", "json");
                            //dictinsett.Add("sRequestedService", enumximethod.ToString());
                            //var cresult = InsertRequestObject(dictinsett);
                            //XIIBO pfbnprequest = new XIIBO();
                            //if (cresult.bOK)
                            //{
                            //    var opfbnprequest = (CResult)cresult.oResult;
                            //    pfbnprequest = (XIIBO)opfbnprequest.oResult;
                            //}
                            //CALL BNP SERVICE HERE
                            dynamic response = null;
                            switch (enumximethod)
                            {
                                case EnumXIBNPMethods.EditCustomerAddress:
                                    {
                                        response = xibnpapi.XIEditCustomerAddress(_req);
                                        if (response != null)
                                        {
                                            iRequestType = Convert.ToInt32(response.EditCustomerAddressResponseType.ResponseStatus);
                                            if ((response.Errors == null || response.Errors.Length <= 0))
                                                IsResponse = true;
                                        }
                                        break;
                                    }
                                case EnumXIBNPMethods.EditCustomerBank:
                                    {
                                        response = xibnpapi.XIEditCustomerBank(_req);
                                        iRequestType = Convert.ToInt32(response.EditCustomerBankResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                                case EnumXIBNPMethods.EditCustomerEmail:
                                    {
                                        response = xibnpapi.XIEditCustomerEmail(_req);
                                        iRequestType = Convert.ToInt32(response.EditCustomerEmailResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                                case EnumXIBNPMethods.EditCustomerPaymentDay:
                                    {
                                        response = xibnpapi.XIEditCustomerPaymentDay(_req);
                                        iRequestType = Convert.ToInt32(response.EditCustomerPaymentDayResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                                case EnumXIBNPMethods.EditCustomerPhone:
                                    {
                                        response = xibnpapi.XIEditCustomerPhone(_req);
                                        iRequestType = Convert.ToInt32(response.EditCustomerPhoneResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                                case EnumXIBNPMethods.MidTermAdjustment:
                                    {
                                        response = xibnpapi.XIMidTermAdjustment(_req);
                                        iRequestType = Convert.ToInt32(response.MidTermAdjustmentResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                                case EnumXIBNPMethods.Cancellation:
                                    {
                                        response = xibnpapi.XICancellation(_req);
                                        iRequestType = Convert.ToInt32(response.CancellationResponseType.ResponseStatus);
                                        if (response.Errors == null || response.Errors.Length <= 0)
                                            IsResponse = true;
                                        break;
                                    }
                            }
                            string spdffile = "";
                            //Update response object in Table
                            //Dictionary<string, string> dictupdate = new Dictionary<string, string>();
                            //dictupdate.Add("id", pfbnprequest.AttributeI("id").sValue);
                            //dictupdate.Add("sResponseObject", new JavaScriptSerializer().Serialize(response));
                            //dictupdate.Add("sPFPdffilepath", spdffile);
                            //dictupdate.Add("sPFPartnerTransactionToken", response.Header.PartnerTransactionToken);
                            //UpdateRequestObject(dictupdate);
                            oCResult.OResult = response;
                            xiqs.XIValues.Add("sResponseObject", new XIPFAttribute { sValue = new JavaScriptSerializer().Serialize(response) });
                            xiqs.XIValues.Add("sRequestObject", new XIPFAttribute { sValue = jsonreq });
                            xiqs.XIValues.Add("PolicyID", new XIPFAttribute { sValue = PolicyID.ToString() });
                            xiqs.XIValues.Add("iPFResponseType", new XIPFAttribute { sValue = iRequestType.ToString() });
                            if (response.Errors != null)
                            {
                                foreach (var item in response.Errors)
                                {
                                    if (item == null)
                                    {
                                        dictMsgs.Add("10", "Your Application is not Accepted.");
                                    }
                                    else
                                    {
                                        dictMsgs.Add(item.Code.ToString(), item.Description);
                                    }
                                }
                                if (enumximethod.ToString() == "MidTermAdjustment")
                                {
                                    xiqs.XIValues.Add("iPFDecision", new XIPFAttribute { sValue = Convert.ToInt32(response.MidTermAdjustmentResponseType.Decision).ToString() });
                                    xiqs.XIValues.Add("TotalCashPricePF", new XIPFAttribute { sValue = Convert.ToInt32(xiqs.GetXIPFValue("MtaFeeAmountPF").rValue).ToString() });
                                }
                                var Inserted = InsertPFApplication(xiqs, PFApplicationID, 0);
                                var MTAInserted = InsertPFMTAApplication(xiqs);
                                oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sFailureStep, null, null);
                                oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                            }
                            else
                            {
                                dictMsgs = new Dictionary<string, string>();
                                oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                                oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);

                                if (IsResponse)
                                {
                                    string spartnertoken = response.Header.PartnerTransactionToken;

                                    xiqs.XIValues.Add("iStatus", new XIPFAttribute { sValue = "10" });
                                    if (enumximethod.ToString() == "MidTermAdjustment")
                                    {
                                        xiqs.XIValues.Add("iPFDecision", new XIPFAttribute { sValue = Convert.ToInt32(response.MidTermAdjustmentResponseType.Decision).ToString() });
                                        xiqs.XIValues.Add("iMTAPFDecision", new XIPFAttribute { sValue = Convert.ToInt32(response.MidTermAdjustmentResponseType.Decision).ToString() });
                                        xiqs.XIValues.Add("TotalCashPricePF", new XIPFAttribute { sValue = Convert.ToInt32(xiqs.GetXIPFValue("MtaFeeAmountPF").rValue).ToString() });
                                    }
                                    xiqs.XIValues.Add("sPFPartnerTransactionToken", new XIPFAttribute { sValue = spartnertoken });
                                    if (enumximethod == EnumXIBNPMethods.Cancellation)
                                    {
                                        //IF THE TRANCATION TYPE IS CANCELLATION THEN DO SOME OTHER TRANSCTIONS
                                        xiqs.XIValues.Add("TotalCashPricePF", new XIPFAttribute { sValue = Convert.ToInt32(xiqs.GetXIPFValue("ReturnOfPremiumValuePF").rValue).ToString() });
                                        PFCancelTransactions(xiqs, iQSIID);
                                    }
                                    var Inserted = InsertPFApplication(xiqs, PFApplicationID, 0);
                                    if (!Inserted.bOK && Inserted.oResult == null)
                                    {
                                        xiresult.sMessage = "ERROR: ACPFApplication_T Insert Failed \r\n PolicyID" + xiqs.GetXIPFValue("PolicyID").sValue;
                                        xiresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        xiresult.LogToFile();
                                        oIB.SaveErrortoDB(xiresult);
                                    }
                                    var MTAInserted = InsertPFMTAApplication(xiqs); //Insert into ACPFMTAApplication
                                    if (!MTAInserted.bOK && MTAInserted.oResult == null)
                                    {
                                        xiresult.sMessage = "ERROR: ACPFMTAApplication_T Insert Failed \r\n PolicyID" + xiqs.GetXIPFValue("PolicyID").sValue;
                                        xiresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        xiresult.LogToFile();
                                        oIB.SaveErrortoDB(xiresult);
                                    }
                                    if (enumximethod == EnumXIBNPMethods.MidTermAdjustment && (Convert.ToInt32(response.MidTermAdjustmentResponseType.Decision) == 1 || Convert.ToInt32(response.MidTermAdjustmentResponseType.Decision) == 4))
                                    {
                                        Execute_PFApp_Pol(xiqs, PFApplicationID, FKsUserID, sDataBase);
                                    }

                                }
                            }
                        }
                    }
                    else if (oPol.Attributes["iType"].iValue != 1)
                    {
                        dictMsgs.Add("20", "PF Application is Not in Live.");
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                        oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    }
                    else // Capturing Error here
                    {
                        oCResult.bprevalidation = false;
                        oCResult.xiStatus = EnumxiStatus.error.ToString();
                        oCResult.OResult = isvalirequest;
                    }
                }
                SetQSResponse(enumximethod, oCResult, oCache, sGUID, sSessionID); // Setting Response to Display in QS
            }
            catch (Exception ex)
            {
                oCResult.IsException = true;
                oCResult.OResult = ex;
                oCResult.xiStatus = EnumxiStatus.error.ToString();
                oCResult.xiErrorMessage = ex.Message.ToString();
                xiresult.sMessage = "ERROR: [" + xiresult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                xiresult.LogToFile();
                oIB.SaveErrortoDB(xiresult);
            }
            return xiresult;
        }

        private void PFCancelTransactions(XIPFValues xiqsvalues, int iQSIID)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                //GET LATEST RECORD FROM ACPFAPPLICATION USING POLICYID
                string PolicyID = xiqsvalues.GetXIPFValue("PolicyID").sValue;
                XID1Click pfappClick = new XID1Click();
                XID1Click pfappClickCopy = new XID1Click();
                XIDBO oBOD = new XIDBO();
                pfappClick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "PF Application Scheme", null);
                pfappClickCopy = (XID1Click)pfappClick.Clone(pfappClick);
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, pfappClick.BOID.ToString());
                XIDXI oXID = new XIDXI();
                var DataSource = oXID.GetBODataSource(oBOD.iDataSource, oBOD.FKiApplicationID);
                List<CNV> nParms = new List<CNV>();
                CNV oNVParams = new CNV();
                oNVParams.sName = "{XIP|ACPolicy_T.id}";
                oNVParams.sValue = PolicyID.ToString();
                nParms.Add(oNVParams);
                pfappClickCopy.ReplaceFKExpressions(nParms);
                var oProductClick = pfappClickCopy.OneClick_Execute().FirstOrDefault();
                string PFApplicationID = oProductClick.Value.Attributes["id"].sValue;
                string FKiPFSchemeID = oProductClick.Value.Attributes["FKiPFSchemeID"].sValue;

                //insert PFCancel
                XIIXI oXI = new XIIXI();
                var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
                XIIBO oBOIPFCancel = new XIIBO();
                XIDXI oXIDPFCancel = new XIDXI();
                XIDBO oBODPFCancel = (XIDBO)oXIDPFCancel.Get_BODefinition("ACPFCancel_T").oResult;
                oBOIPFCancel.BOD = oBODPFCancel;
                oBOIPFCancel.SetAttribute("rCancelAmount", oQSInstance.GetXIIValue("ReturnOfPremiumValuePF").sValue);
                oBOIPFCancel.SetAttribute("rNotes", oQSInstance.GetXIIValue("CancellationReasonTypePF").sResolvedValue);
                oBOIPFCancel.SetAttribute("FKiPFSchemeID", FKiPFSchemeID);
                oBOIPFCancel.SetAttribute("FKiACPolicyID", PolicyID.ToString());
                oBOIPFCancel.SetAttribute("rFinanceBalance", oQSInstance.GetXIIValue("InsurerReturnOfPremiumValuePF").sValue);
                oBOIPFCancel.SetAttribute("iStatus", "0");
                oBOIPFCancel.SetAttribute(XIConstant.Key_XIDeleted, "0");
                var PFCancel = oBOIPFCancel.Save(oBOIPFCancel);
                if (!PFCancel.bOK && PFCancel.oResult == null)
                {
                    oCResult.sMessage = "ERROR: Unable to insert the PF Cancellation for Application=" + PFApplicationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                #region UPDATE_ACPFApplication_T
                //UPDATING ACPFApplication_T iStatus to 20 for Cancel
                XIIBO uoboiPol = new XIIBO();
                XIDXI uoxidPol = new XIDXI();
                //XIDBO uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                uoboiPol.BOD = oBOD;
                uoboiPol.SetAttribute("id", PFApplicationID);
                uoboiPol.SetAttribute("iStatus", "20");
                var PFApUpdated = uoboiPol.Save(uoboiPol);
                if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                {
                    oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application iStatus Failed to Cancel : 20. Policy Create ACPFApplication=" + PFApplicationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                #endregion

                #region UPDATE_POLICY IPFSTATUS
                uoboiPol = new XIIBO();
                uoxidPol = new XIDXI();
                oBOD = (XIDBO)uoxidPol.Get_BODefinition("ACPolicy_T").oResult;
                uoboiPol.BOD = oBOD;
                uoboiPol.SetAttribute("id", PolicyID);
                uoboiPol.SetAttribute("iPFStatus", "40");
                var polupdated = uoboiPol.Save(uoboiPol);
                if (!polupdated.bOK && polupdated.oResult == null)
                {
                    oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application ACPolicy_T, iPFStatus Failed to Cancel : 40. Policy Create FkiPolicyID=" + PolicyID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oIB.SaveErrortoDB(oCResult);
                }
                #endregion

                #region TRANSACTIONS
                XIIBO oBOI = oXI.BOI("ACPFApplication_T", PolicyID, "refAccountCategory");
                string sTransCode = "PFCO";
                JournalTransactions oJTransactions = new JournalTransactions();
                XIIBO oTransaction = new XIIBO();
                oJTransactions.zCommission = 0;
                oJTransactions.zDefaultAdmin = 0;
                oJTransactions.rAdmin = 0; // NEED TO CHAGE rCancelAmount hardcoded value
                oTransaction.SetAttribute("zBaseValue", oQSInstance.GetXIIValue("ReturnOfPremiumValuePF").sValue);
                oTransaction.SetAttribute("FKiACPolicyID", PolicyID);
                oTransaction.SetAttribute("FKiPFSchemeID", FKiPFSchemeID);
                oTransaction.SetAttribute("FKiProductID", xiqsvalues.GetXIPFValue("FKiProductID").sValue);
                oTransaction.SetAttribute("sTransCode", sTransCode);
                oTransaction.SetAttribute("FKsWhoID", FKsUserID);
                oTransaction.SetAttribute("iPostedFrom", "20");
                oTransaction.SetAttribute("FKiSupplierID", oProductClick.Value.Attributes["FKiSupplierID"].sValue);
                oTransaction.SetAttribute("refAccountCategory", oBOI.AttributeI("refAccountCategory").sValue);
                var PFTransaction = oJTransactions.PostTransaction(oTransaction, sDataBase);
                oJTransactions.Update_PolicyBalance(Convert.ToInt32(PolicyID), 10);
                oJTransactions.Update_PolicyBalance(Convert.ToInt32(PolicyID), 10, 10);
                #endregion
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: UPDATE PF Cancel Transaction PolicyID :" + xiqsvalues.GetXIPFValue("PolicyID").sValue + " " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
        }

        #endregion

        #region ReferCaseHandling
        public bool UpdatePFNewBusinessRefer(XIPFValues xivalues) // /* TO DO HERE
        {
            CommonRepository Common = new CommonRepository();
            try
            {

                CResult oCResult = new CResult();
                XIInstanceBase oIB = new XIInstanceBase();
                if (xivalues != null && xivalues.XIValues.Count() > 0)
                {
                    string spartnertoken = xivalues.GetXIPFValue("PartnerTransactionToken").sValue;
                    string iDecision = xivalues.GetXIPFValue("iPFDecision").sValue;
                    string Pdfbase64 = xivalues.GetXIPFValue("Pdf").sValue;
                    string accountnumber = xivalues.GetXIPFValue("sPFAccountNo").sValue;
                    string pdfpath = "";
                    if (!string.IsNullOrEmpty(spartnertoken))
                    {
                        // Save pdf file in physical folder
                        if (!string.IsNullOrEmpty(Pdfbase64))
                        {
                            string filepath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Attachments/PremiumFinance/");
                            pdfpath = Utility.SaveFile(filepath, accountnumber, EnumFileExtensions.pdf.ToString(), Pdfbase64);
                        }
                        if (pdfpath.Contains("Fail"))
                        {
                            //log here pdf not saved
                            Common.SaveErrorLog("PF Refer case ApplicationLog: pdf file not saved", "");
                            pdfpath = "";
                        }
                        XIIXI oIXI = new XIIXI();
                        int PFID = 0;
                        List<CNV> Params = new List<CNV>();
                        Params.Add(new CNV { sName = "sPartnerTransactionToken", sValue = spartnertoken });
                        XIIBO oBOI = oIXI.BOI("ACPFApplication_T", null, "id", Params);
                        PFID = oBOI.AttributeI("id").iValue;
                        if (PFID > 0)
                        {
                            #region UPDATE_ACPFApplication_T
                            //UPDATING iType IN ACPFApplication_T
                            XIIBO uoboiPol = new XIIBO();
                            XIDXI uoxidPol = new XIDXI();
                            XIDBO uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                            uoboiPol.BOD = uobodPol;
                            uoboiPol.SetAttribute("id", PFID.ToString());
                            uoboiPol.SetAttribute("iType", iDecision);
                            uoboiPol.SetAttribute("sPdffilepath", pdfpath);
                            // uoboiPol.SetAttribute("sSupplierPFAccountNo", xivalues.GetXIPFValue("sPFAccountNo").sValue);
                            var PFApUpdated = uoboiPol.Save(uoboiPol);
                            if (!PFApUpdated.bOK && PFApUpdated.oResult == null)
                            {
                                oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application iType Failed. Policy Create id=" + PFID;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oIB.SaveErrortoDB(oCResult);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        // Log into DB

                        Common.SaveErrorLog("PF Refer case ApplicationLog: getting partner token empty", "");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("PF Refercase ErrorLog: " + ex.ToString(), "");
                throw ex;
            }
        }
        #endregion
        #region PF MTA
        public CResult Execute_PFApp_Pol(XIPFValues xiqs, int PFApplicationID, string FksWhoID, string sDataBase)
        {
            const string ROUTINE_NAME = "[Execute_PFApplication]";
            //If i.zX.tracingActive Then i.zX.trace.enter CLASS_NAME, ROUTINE_NAME
            XIIBO oPFScheme = new XIIBO();
            double rPFAmount = 0;
            double rTotalFinance = 0;
            string sTransCode = null;
            XIIBO oTransType = new XIIBO();
            CResult tRtn = new CResult();
            XIIBO oLivePolicy = new XIIBO();
            XIIBO oNewTransaction = new XIIBO();
            XIIBO oPFApplication = new XIIBO();
            XIInstanceBase oIB = new XIInstanceBase();
            CResult oCResult = new CResult();
            try
            {
                //'ALERT -  COPIED FROM CPOLICYCREATE FOR RUNNING PF MTAS
                rPFAmount = xiqs.GetXIPFValue("FundingDifferencePF").dblValue;
                //'Set oPFApplication = i.zX.BOS.quickFKLoad(Me, "FKiPFApplicationID")
                XIIXI oIXI = new XIIXI();
                oPFApplication = oIXI.BOI("ACPFApplication_T", PFApplicationID.ToString());
                oPFScheme = oIXI.BOI("PFScheme_T", oPFApplication.AttributeI("FKiPFSchemeID").sValue);
                oLivePolicy = oIXI.BOI("ACPolicy_T", oPFApplication.AttributeI("FKiACPolicyID").sValue);
                rTotalFinance = oLivePolicy.AttributeI("rTotalFinance").doValue;
                rTotalFinance = rTotalFinance + rPFAmount;
                oPFApplication.SetAttribute("rTotalFinance", rTotalFinance.ToString());
                if (rPFAmount < 0)
                {
                    sTransCode = "PFRP";
                    rPFAmount = rPFAmount * -1;
                }
                else
                {
                    sTransCode = "PFAP";
                }
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "sCode", sValue = sTransCode });
                oParams.Add(new CNV { sName = XIConstant.Key_XIDeleted, sValue = "0" });
                oTransType = oIXI.BOI("ACTransType_T", "", "*", oParams);
                if (oTransType == null)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + ", Cannot load Transaction Type for Policy Create Client Payment: " + oLivePolicy.AttributeI("id").sValue + " and Product: " + oLivePolicy.AttributeI("FKiProductID").sValue;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + ", Cannot load Transaction Type for Policy Create Client Payment: " + oLivePolicy.AttributeI("id").sValue + " and Product: " + oLivePolicy.AttributeI("FKiProductID").sValue;
                    oIB.SaveErrortoDB(oCResult);
                }
                JournalTransactions oJTransactions = new JournalTransactions();
                XIDXI oxid = new XIDXI();
                XIDBO obod = (XIDBO)oxid.Get_BODefinition("ACTransaction_T").oResult;
                oNewTransaction.BOD = obod;
                oNewTransaction.SetAttribute("sName", "Premium Finance setup in QuestionSet");
                oNewTransaction.SetAttribute("iType", oTransType.AttributeI("iType").sValue);   //'premium finance
                oNewTransaction.SetAttribute("FKiPFSchemeID", oPFScheme.AttributeI("id").sValue);
                oNewTransaction.SetAttribute("iPostedFrom", 20.ToString());
                oNewTransaction.SetAttribute("FKiACPolicyID", oLivePolicy.AttributeI("id").sValue);
                oNewTransaction.SetAttribute("FKiACTransTypeID", oTransType.AttributeI("id").sValue);
                oNewTransaction.SetAttribute("refAccountCategory", oLivePolicy.AttributeI("refAccountCategory").sValue);
                //if (xiqs.GetXIPFValue("iType").sValue == "1")
                //{
                //    oNewTransaction.SetAttribute(XIConstant.Key_XIDeleted, "0");
                //}
                //else
                //{
                //    oNewTransaction.SetAttribute(XIConstant.Key_XIDeleted, "1");
                //}
                //'inherit percentages from product. Could do this with a copy group but so important i'd rather make it explicit
                oNewTransaction.SetAttribute("zCommission", "0");
                oNewTransaction.SetAttribute("zDefaultAdmin", "0");
                oNewTransaction.SetAttribute("zDefaultDeposit", "0");
                oNewTransaction.SetAttribute("FKiSupplierID", oPFApplication.AttributeI("FKiSupplierID").sValue);
                oNewTransaction.SetAttribute("FKsWhoID", FksWhoID);// ' i.GetAttr("FKsPostUserID")
                oNewTransaction.SetAttribute("zBaseValue", rPFAmount.ToString());
                oNewTransaction.SetAttribute("rAdmin", "0");
                oNewTransaction.SetAttribute("sTransCode", sTransCode);
                oNewTransaction.SetAttribute("sNotes", xiqs.GetXIPFValue("MtaReasonDescriptionPF").sValue);

                if (rPFAmount != 0)
                {
                    //'post the transactions for a new policy
                    var result = oJTransactions.PostTransaction(oNewTransaction, sDataBase);
                    oJTransactions.Update_PolicyBalance(oLivePolicy.AttributeI("id").iValue, 10);
                    oJTransactions.Update_PolicyBalance(oLivePolicy.AttributeI("id").iValue, 10, 10);
                    if (result.bOK != true)
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + "Unable to insert instance of AC/ACTransaction";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + "Unable to insert instance of AC/ACTransaction";
                        oIB.SaveErrortoDB(oCResult);
                    }
                }

                //Set oLivePolicy = Me 'i.zX.BOS.quickFKLoad(Me, "FKiLivePolicyID")

                oLivePolicy.SetAttribute("iPFStatus", "10");
                var oPolicy = oLivePolicy.Save(oLivePolicy);
                if (oPolicy.bOK != true)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + " - Cannot update Policy iPFStatus id=" + oLivePolicy.AttributeI("id").sValue;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + " - Cannot update Policy iPFStatus id=" + oLivePolicy.AttributeI("id").sValue;
                    oIB.SaveErrortoDB(oCResult);
                }
                oJTransactions.Update_PolicyBalance(oLivePolicy.AttributeI("id").iValue, 10, 10);
            }
            catch (Exception e)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + e.Message;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + ROUTINE_NAME + e.Message;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion
        public CResult PolicyLive(List<CNV> oParams)
        {
            XIPFCResult xiresult = new XIPFCResult();
            XIInstanceBase oIB = new XIInstanceBase();
            string PolicyID = oParams.Where(t => t.sName == "iACPolicyID").Select(f => f.sValue).FirstOrDefault();
            string ACPFApplicationID = oParams.Where(t => t.sName == "ACPFApplicationID").Select(f => f.sValue).FirstOrDefault();
            int iProductID = oParams.Where(t => t.sName == "iProductID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            string sPFTypeCode = oParams.Where(t => t.sName == "sPolicyCode").Select(f => f.sValue).FirstOrDefault();
            FKsUserID = oParams.Where(t => t.sName == "iUserID").Select(f => f.sValue).FirstOrDefault();
            sDataBase = oParams.Where(m => m.sName.ToLower() == "sDataBase".ToLower()).Select(m => m.sValue).FirstOrDefault();

            var sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
            string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
            int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            string sSuccessStep = oParams.Where(m => m.sName.ToLower() == "SuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
            XIIXI oXI = new XIIXI();
            var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID);
            Dictionary<string, string> dictMsgs = new Dictionary<string, string>();
            string skey = "StepMessage";
            List<CNV> oWhereParams = new List<CNV>();
            oWhereParams.Add(new CNV { sName = "FKiACPolicyID", sValue = PolicyID });
            if (string.IsNullOrEmpty(ACPFApplicationID))
            {
                var PFApplication = oXI.BOI("ACPFApplication_T", null, "id", oWhereParams);
                ACPFApplicationID = PFApplication.AttributeI("id").sValue;
            }
            var oPol = oXI.BOI("ACPFApplication_T", ACPFApplicationID, "iStatus,dSumPaymentDate,sTypeCode,iType");
            CResult oCResult = new CResult();
            var iAuditID = 0;
            ZeeInsurance.Policy oPolicy = new ZeeInsurance.Policy();
            try
            {
                if (oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault() != null)
                {
                    oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault().sValue = "Policy Live in-process";
                }
                else
                {
                    oParams.Add(new CNV() { sName = "sAuditInfo", sValue = "Policy Live in-process" });
                }
                oParams.Add(new CNV() { sName = "iAuditID", sValue = "" });
                var oARes = oPolicy.Audit_Policy(oParams);
                var iAuditInsID = oARes.oResult;
                int.TryParse(iAuditInsID.ToString(), out iAuditID);

                XIIBO oboiPol = new XIIBO();
                XIDXI oxidPol = new XIDXI();
                XIIXI oIXI = new XIIXI();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                QueryEngine oQE = new QueryEngine();
                List<XIIBO> oTRBOI = new List<XIIBO>();
                XIDXI oTXI = new XIDXI();
                XIDXI uoxidPol = new XIDXI();
                XIIBO uoboiPol = new XIIBO();
                XIDBO uobodPol = new XIDBO();
                XIIBO oBOIMTAPF = new XIIBO();
                string sTranstype = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|TransactionType}");
                if (sPFTypeCode == "Accept")
                {
                    string sPolicyNo = null;
                    if (string.IsNullOrEmpty(sTranstype) || sTranstype.ToLower() == "new" || sTranstype.ToLower() == "rebroke")
                    {
                        var oProductI = oXI.BOI("Product", iProductID.ToString());
                        var Result = oPolicy.PolicyNoGeneration(oProductI);
                        if (Result.bOK && Result.oResult != null)
                        {
                            sPolicyNo = (string)Result.oResult;
                        }
                    }
                    else
                    {
                        var oPolicyNo = oXI.BOI("ACPolicy_T", PolicyID, "sPolicyNo");
                        sPolicyNo = oPolicyNo.AttributeI("sPolicyNo").sValue;
                    }
                    uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                    uoboiPol.BOD = uobodPol;
                    uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                    uoboiPol.SetAttribute("iStatus", "10");
                    uoboiPol.SetAttribute("iType", "7");
                    uoboiPol.SetAttribute("sPolicyNo", sPolicyNo);
                    uoboiPol.SetAttribute(XIConstant.Key_XIDeleted, "0");
                    var PFApUpdated = uoboiPol.Save(uoboiPol);
                    if (PFApUpdated.bOK && PFApUpdated.oResult != null)
                    {
                        oCResult.sMessage = "Success: UPDATE of Premium Finance Application Status for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFMTAApplication_T").oResult;
                    oBOIMTAPF = oIXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                    oBOIMTAPF.BOD = uobodPol;
                    oBOIMTAPF.Exclude("id");
                    oBOIMTAPF.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                    var PFMTA = oBOIMTAPF.Save(oBOIMTAPF);
                    if (PFMTA.bOK && PFMTA.oResult != null)
                    {
                        oCResult.sMessage = "Success: Premium Finance MTA Application is Posted for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: Premium Finance MTA Application is not Posted for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    oTRBOI = new List<XIIBO>();
                    oWParams = new List<XIWhereParams>();
                    List<SqlParameter> oSQLParams = new List<SqlParameter>();
                    oQE = new QueryEngine();
                    oWParams.Add(new XIWhereParams { sField = "FKiACPolicyID", sOperator = "=", sValue = PolicyID.ToString() });
                    oWParams.Add(new XIWhereParams { sField = XIConstant.Key_XIDeleted, sOperator = "=", sValue = "1" });
                    oSQLParams.Add(new SqlParameter { ParameterName = "@FKiACPolicyID", Value = PolicyID.ToString() });
                    oSQLParams.Add(new SqlParameter { ParameterName = XIConstant.Key_XIDeleted, Value = "1" });
                    oQE.AddBO("ACTransaction_T", "id," + XIConstant.Key_XIDeleted, oWParams);
                    oCResult = oQE.BuildQuery();
                    if (oCResult.bOK && oCResult.oResult != null)
                    {
                        var sSql = (string)oCResult.oResult;
                        ExecutionEngine oEE = new ExecutionEngine();
                        oEE.XIDataSource = oQE.XIDataSource;
                        oEE.sSQL = sSql;
                        oEE.SqlParams = oSQLParams;
                        var oQResult = oEE.Execute();
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            oTRBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        }
                    }
                    if (oTRBOI == null)
                    {
                        oCResult.sMessage = oCResult.sFunctionName + ", Unable to get DB2Collection from Transactions ";//i.zX.trace.formatStack
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                    }
                    var BOD = (XIDBO)oTXI.Get_BODefinition("ACTransaction_T").oResult;
                    foreach (var item in oTRBOI)
                    {
                        item.BOD = BOD;
                        item.SetAttribute(XIConstant.Key_XIDeleted, "0");
                        var res = item.Save(item);
                        if (!res.bOK && res.oResult == null)
                        {
                            oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = oCResult.sFunctionName + ", Update of " + XIConstant.Key_XIDeleted + " (Active) for Transaction failed for id=: " + item.AttributeI("id").ToString();
                            oIB.SaveErrortoDB(oCResult);
                        }
                    }
                    #region UPDATE_POLICY
                    // UPDATE INTO ACPolicy_T rPFTotal,sPFAccountNo,FKiPFApplicationID      
                    XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                    oboiPol.BOD = obodPol;
                    oboiPol.SetAttribute("id", PolicyID.ToString());
                    oboiPol.SetAttribute("iPFStatus", "0");
                    oboiPol.SetAttribute("FKiPFApplicationID", ACPFApplicationID.ToString());
                    oboiPol.SetAttribute("rPFTotal", oBOIMTAPF.AttributeI("rPFAmount").sValue); // Total PF Amount
                    oboiPol.SetAttribute("sPolicyNo", sPolicyNo);
                    if (string.IsNullOrEmpty(sTranstype) || sTranstype.ToLower() == "new")
                    { oboiPol.SetAttribute("iStatus", "10"); }
                    else if (!string.IsNullOrEmpty(sTranstype) && sTranstype.ToLower() == "renewal")
                    {
                        oboiPol.SetAttribute("iStatus", "25");
                    }
                    else if (!string.IsNullOrEmpty(sTranstype) && sTranstype.ToLower() == "rebroke")
                    {
                        oboiPol.SetAttribute("iStatus", "50");
                    }
                    int PolicyVersionID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyVersionID}"));
                    oboiPol.SetAttribute("iVersionNo", PolicyVersionID.ToString());
                    oboiPol.SetAttribute("rBalance", oBOIMTAPF.AttributeI("rPFAmount").sValue);
                    oboiPol.SetAttribute("rPolicyBalance", oBOIMTAPF.AttributeI("rPFAmount").sValue);

                    var Updated = oboiPol.Save(oboiPol);
                    if (!Updated.bOK && Updated.oResult == null)
                    {
                        oCResult.sMessage = "ERROR: PF status Update Failed in ACPolicy_T \r\n" + PolicyID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    #endregion
                    #region UPDATE_POLICYVersion
                    // UPDATE INTO ACPolicy_T rPFTotal,sPFAccountNo,FKiPFApplicationID      
                    XIDBO obodPolversion = (XIDBO)oxidPol.Get_BODefinition("ACPolicyVersion_T").oResult;
                    XIIBO PolicyVersionI = new XIIBO();
                    PolicyVersionI.BOD = obodPolversion;
                    PolicyVersionI.SetAttribute("id", PolicyVersionID.ToString());
                    PolicyVersionI.SetAttribute("iPolicyStatus", "10");
                    var oPolicyVersionI = oboiPol.Save(PolicyVersionI);
                    if (!oPolicyVersionI.bOK && oPolicyVersionI.oResult == null)
                    {
                        oCResult.sMessage = "ERROR: PF Update Failed in ACPolicyVersion_T \r\n" + PolicyID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    #endregion  
                    oParams.Add(new CNV { sName = "iPaymentMethodType", sValue = "20" });
                    oParams.Add(new CNV { sName = "FKiPolicyVersionID", sValue = PolicyVersionID.ToString() });
                    oParams.Add(new CNV { sName = "iACPolicyID", sValue = PolicyID.ToString() });
                    oParams.Add(new CNV { sName = "sPolicyNo", sValue = sPolicyNo });
                    oParams.Add(new CNV { sName = "iQSInstanceID", sValue = iQSIID.ToString() });
                    //oParams.Add(new CNV { sName = "PolicyLive", sValue = "true" });
                    Thread threadObj = new Thread(new ThreadStart(() => { oPolicy.ThreadRunMethods(oParams); }));
                    threadObj.Start();
                    dictMsgs.Add("10", "Policy in Live & Pf application is not in Live.");
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                    oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    if (oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault() != null)
                    {
                        oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault().sValue = "Policy in Live & Pf application is not in Live.";
                    }
                    //if (oParams.Where(m => m.sName == "iAuditID").FirstOrDefault() != null)
                    //{
                    //    oParams.Where(m => m.sName == "iAuditID").FirstOrDefault().sValue = iAuditID.ToString();
                    //}
                    if (oParams.Where(s => s.sName == "FKiPolicyVersionID").FirstOrDefault() != null)
                    {
                        oParams.Where(s => s.sName == "FKiPolicyVersionID").FirstOrDefault().sValue = PolicyVersionID.ToString();
                    }
                    else
                    {
                        oParams.Add(new CNV { sName = "FKiPolicyVersionID", sValue = PolicyVersionID.ToString() });
                    }
                    oPolicy.Audit_Policy(oParams);
                    XIPFValues xiqs = new XIPFValues();
                    xiqs.XIValues.Add("FKiSupplierID", new XIPFAttribute { sValue = oBOIMTAPF.AttributeI("FKiSupplierID").sValue });
                    xiqs.XIValues.Add("FKiPFSchemeID", new XIPFAttribute { sValue = oBOIMTAPF.AttributeI("FKiPFSchemeID").sValue });
                    xiqs.XIValues.Add("FKiCustomerID", new XIPFAttribute { sValue = oBOIMTAPF.AttributeI("FKiClientID").sValue });
                    xiqs.XIValues.Add("FKiProductID", new XIPFAttribute { sValue = iProductID.ToString() });
                    xiqs.XIValues.Add("PolicyID", new XIPFAttribute { sValue = PolicyID.ToString() });
                    xiqs.XIValues.Add("PFAmount", new XIPFAttribute { sValue = oBOIMTAPF.AttributeI("rPFAmount").sValue });
                    PremiumfinanceTransactionsAsync(xiqs, sTranstype, 1);
                    XIIBO oTransaction = oIXI.BOI("ACTransaction_T", sTransactionID);
                    oTransaction.SetAttribute(XIConstant.Key_XIDeleted, "1");
                    oCResult = oTransaction.Save(oTransaction);
                }
                else
                {
                    uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFApplication_T").oResult;
                    uoboiPol.BOD = uobodPol;
                    uoboiPol.SetAttribute("id", ACPFApplicationID.ToString());
                    uoboiPol.SetAttribute("iStatus", "10");
                    uoboiPol.SetAttribute("iType", "7");
                    uoboiPol.SetAttribute(XIConstant.Key_XIDeleted, "0");
                    var PFApUpdated = uoboiPol.Save(uoboiPol);
                    if (PFApUpdated.bOK && PFApUpdated.oResult != null)
                    {
                        oCResult.sMessage = "Success: UPDATE of Premium Finance Application Status for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: UPDATE of Premium Finance Application Status for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    uobodPol = (XIDBO)uoxidPol.Get_BODefinition("ACPFMTAApplication_T").oResult;
                    oBOIMTAPF = oIXI.BOI("ACPFApplication_T", ACPFApplicationID.ToString());
                    oBOIMTAPF.BOD = uobodPol;
                    oBOIMTAPF.Exclude("id");
                    oBOIMTAPF.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                    var PFMTA = oBOIMTAPF.Save(oBOIMTAPF);
                    if (PFMTA.bOK && PFMTA.oResult != null)
                    {
                        oCResult.sMessage = "Success: Premium Finance MTA Application is Posted for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: Premium Finance MTA Application is not Posted for Policy id=" + ACPFApplicationID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    #region UPDATE_POLICY
                    // UPDATE INTO ACPolicy_T rPFTotal,sPFAccountNo,FKiPFApplicationID      
                    XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                    oboiPol.BOD = obodPol;
                    oboiPol.SetAttribute("id", PolicyID.ToString());
                    oboiPol.SetAttribute("iPFStatus", "0");
                    oboiPol.SetAttribute("FKiPFApplicationID", ACPFApplicationID.ToString());
                    oboiPol.SetAttribute("rPFTotal", oBOIMTAPF.AttributeI("rPFAmount").sValue); // Total PF Amount
                    oboiPol.SetAttribute("iStatus", "5");
                    int PolicyVersionID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iACPolicyVersionID}"));
                    oboiPol.SetAttribute("iVersionNo", PolicyVersionID.ToString());
                    oboiPol.SetAttribute("rBalance", oBOIMTAPF.AttributeI("rPFAmount").sValue);
                    oboiPol.SetAttribute("rPolicyBalance", oBOIMTAPF.AttributeI("rPFAmount").sValue);
                    var Updated = oboiPol.Save(oboiPol);
                    if (!Updated.bOK && Updated.oResult == null)
                    {
                        oCResult.sMessage = "ERROR: PF status Update Failed in ACPolicy_T \r\n" + PolicyID.ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    #endregion
                    dictMsgs.Add("20", "Policy is not in Live & Pf application is not in Live.");
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sSuccessStep, null, null);
                    oCache.Set_ObjectSetCache(sSessionID, skey, sGUID, dictMsgs);
                    if (oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault() != null)
                    {
                        oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault().sValue = "Policy is not in Live & Pf application is not in Live.";
                    }
                    //if (oParams.Where(m => m.sName == "iAuditID").FirstOrDefault() != null)
                    //{
                    //    oParams.Where(m => m.sName == "iAuditID").FirstOrDefault().sValue = iAuditID.ToString();
                    //}
                    if (oParams.Where(s => s.sName == "FKiPolicyVersionID").FirstOrDefault() != null)
                    {
                        oParams.Where(s => s.sName == "FKiPolicyVersionID").FirstOrDefault().sValue = PolicyVersionID.ToString();
                    }
                    else
                    {
                        oParams.Add(new CNV { sName = "FKiPolicyVersionID", sValue = PolicyVersionID.ToString() });
                    }
                    oPolicy.Audit_Policy(oParams);
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                if (oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault() != null)
                {
                    oParams.Where(m => m.sName == "sAuditInfo").FirstOrDefault().sValue = "Policy live process failed";
                }
                if (oParams.Where(m => m.sName == "iAuditID").FirstOrDefault() != null)
                {
                    oParams.Where(m => m.sName == "iAuditID").FirstOrDefault().sValue = iAuditID.ToString();
                }
                oPolicy.Audit_Policy(oParams);
                xiresult.IsException = true;
                xiresult.OResult = ex;
                xiresult.xiStatus = EnumxiStatus.error.ToString();
                xiresult.xiErrorMessage = ex.Message.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

    }
}
