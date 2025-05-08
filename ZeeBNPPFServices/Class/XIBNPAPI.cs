using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeeBNPPFServices.BNPServiceDev;
using System.Web.Script.Serialization;
using System.Net;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using XICore;
using XISystem;

namespace ZeeBNPPFServices
{
    public class XIBNPAPI : XIDefinitionBase
    {
        //CALLING 
        IntegratedGatewayHost client;
        public Error[] ErrorList { get; set; }
        public string sShopCode { get; set; }
        public XIBNPAPI() // Load headers,UserCredentials Constructor
        {
            if (client == null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client = new ZeeBNPPFServices.BNPServiceDev.IntegratedGatewayHost();
            }
        }
        #region XIBNPConfig
        protected UserCredential GetCredentials()
        {
            UserCredential retunobj = new UserCredential();
            retunobj.Username = GetValue("PFUsername");//"QMT"; //GetValue("");//get username from app.config file
            retunobj.Password = GetValue("PFPassword");//"wn5uVY9kXKpLfdcM";//GetValue(""); //get password from app.config file
            return retunobj;
        }
        protected HeaderWithCredential GetHeader()
        {
            HeaderWithCredential headers = new HeaderWithCredential();
            if (!string.IsNullOrEmpty(sShopCode))
            {
                headers.ShopCode = sShopCode;
            }
            else
            {
                headers.ShopCode = GetValue("PFShopCode");
            }
            headers.SystemType = EnumsSystemType.Ipf;
            headers.TransactionRef = "T" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            headers.UserId = GetValue("PFUserId");//"QMT";
            headers.ClientCode = GetValue("PFClientCode");// "QM1";
            headers.CostCentre = GetValue("PFCostCentre");//"123456";
            headers.PartnerTransactionToken = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            headers.UserCredential = GetCredentials();

            CResult oCr = new CResult();
            string sMessage = "BNPConnection Header -> ";
            sMessage = sMessage + "ShopCode: " + headers.ShopCode + "  ->";
            sMessage = sMessage + "UserId: " + headers.UserId + " -> ";
            sMessage = sMessage + "ClientCode: " + headers.ClientCode + " -> ";
            sMessage = sMessage + "CostCentre: " + headers.CostCentre + " -> ";
            sMessage = sMessage + "Username: " + headers.UserCredential.Username + " -> ";
            sMessage = sMessage + "Pwd: " + headers.UserCredential.Password;
            oCr.sMessage = sMessage;
            SaveErrortoDB(oCr);
            return headers;
        }
        public string GetValue(string skey)
        {
            try
            {
                return ConfigurationManager.AppSettings[skey];
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region ServerNotRespondingError
        private Error[] GetServerError()
        {
            if (ErrorList != null)
            {
                return ErrorList;
            }
            else
            {
                Error[] error = new Error[1];
                error[0] = new Error { Code = 999, Description = "Error: Server not Responding" };
                return error;
            }
        }
        #endregion

        #region XIBNP_Methods
        public LiveRateQuoteResponse XILiveRateQuote(LiveRateQuoteRequest request)
        {
            try
            {
                LiveRateQuoteResponse result = new LiveRateQuoteResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.LiveRateQuote(request);//new LiveRateQuoteResponse(); //client.LiveRateQuote(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool VerifyBNPConnectionStatus()
        {
            var result=VerifyBNPConnection();
            return result;
        }
        public NewBusinessResponse XINewBusiness(NewBusinessRequest request, string ShopCode = null)
        {
            try
            {
                NewBusinessResponse result = new NewBusinessResponse();
                sShopCode = ShopCode;
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.NewBusiness(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MidTermAdjustmentResponse XIMidTermAdjustment(MidTermAdjustmentRequest request)
        {
            try
            {
                MidTermAdjustmentResponse result = new MidTermAdjustmentResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.MidTermAdjustment(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CancellationResponse XICancellation(CancellationRequest request)
        {
            try
            {
                CancellationResponse result = new CancellationResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.Cancellation(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PolicyStatusResponse XIPolicyStatus(PolicyStatusRequest request)
        {
            try
            {
                PolicyStatusResponse result = new PolicyStatusResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.PolicyStatus(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ServiceStatusResponse XIServiceStatus(ServiceStatusRequest request)
        {
            try
            {

                request.Header = GetHeader();
                var result = client.ServiceStatus(request);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EditCustomerAddressResponse XIEditCustomerAddress(EditCustomerAddressRequest request)
        {
            try
            {
                EditCustomerAddressResponse result = new EditCustomerAddressResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.EditCustomerAddress(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EditCustomerBankResponse XIEditCustomerBank(EditCustomerBankRequest request)
        {
            try
            {
                EditCustomerBankResponse result = new EditCustomerBankResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.EditCustomerBank(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EditCustomerEmailResponse XIEditCustomerEmail(EditCustomerEmailRequest request)
        {
            try
            {
                EditCustomerEmailResponse result = new EditCustomerEmailResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.EditCustomerEmail(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EditCustomerPaymentDayResponse XIEditCustomerPaymentDay(EditCustomerPaymentDayRequest request)
        {
            try
            {
                EditCustomerPaymentDayResponse result = new EditCustomerPaymentDayResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.EditCustomerPaymentDay(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EditCustomerPhoneResponse XIEditCustomerPhone(EditCustomerPhoneRequest request)
        {
            try
            {
                EditCustomerPhoneResponse result = new EditCustomerPhoneResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.EditCustomerPhone(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartnerQuoteResponse XIPartnerQuote(PartnerQuoteRequest request)
        {
            try
            {
                PartnerQuoteResponse result = new PartnerQuoteResponse();
                if (VerifyBNPConnection())
                {
                    request.Header = GetHeader();
                    result = client.PartnerQuote(request);
                }
                else
                {
                    result.Errors = GetServerError();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //#region InsertUPDATE_Request_ResponseObject
        //public CResult InsertRequestObject(Dictionary<string, string> InputObject)
        //{
        //    try
        //    {
        //        StringBuilder strreqbuilder = new StringBuilder();
        //        strreqbuilder.Append(InputObject["sRequestObject"].Trim());
        //        if (InputObject["sType"] == "xml")
        //        {
        //            //ADD HEADERS
        //            var headerobj = GetHeader();
        //            headerobj.UserCredential.Password = "****";
        //            var headerxml = GetXMLFromObject(GetHeader());
        //            headerxml = headerxml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
        //            strreqbuilder.Append(headerxml);
        //            // strreqbuilder.Replace("<?xml version=\"1.0\" ?>", "<?xml version=\"1.0\" ?>"+headerxml);
        //        }
        //        CResult oCResult = new CResult();
        //        XIIBO oboiACT = new XIIBO();
        //        XIDXI oxidACT = new XIDXI();
        //        XIDBO obodACT = (XIDBO)oxidACT.Get_BODefinition("PFBNPRequests_T").oResult;
        //        oboiACT.BOD = obodACT;
        //        oboiACT.SetAttribute("sRequestObject", strreqbuilder.ToString());
        //        oboiACT.SetAttribute("sResponseObject", InputObject["sResponseObject"]);
        //        oboiACT.SetAttribute("sType", InputObject["sType"]);
        //        oboiACT.SetAttribute("sRequestedService", InputObject["sRequestedService"]);
        //        var Inserted = oboiACT.Save(oboiACT);
        //        if (!Inserted.bOK && Inserted.oResult == null)
        //        {
        //            XIInstanceBase oIB = new XIInstanceBase();
        //            oCResult.sMessage = "ERROR: PFBNPRequests_T Insert Failed \r\n";
        //            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //            oCResult.LogToFile();
        //            oIB.SaveErrortoDB(oCResult);
        //        }
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
        //        oCResult.oResult = Inserted;
        //        return oCResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public CResult UpdateRequestObject(Dictionary<string, string> InputObject)
        //{
        //    try
        //    {
        //        CResult oCResult = new CResult();
        //        XIIBO oboiPol = new XIIBO();
        //        XIDXI oxidPol = new XIDXI();
        //        XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("PFBNPRequests_T").oResult;
        //        oboiPol.BOD = obodPol;
        //        oboiPol.SetAttribute("id", InputObject["id"].ToString());
        //        oboiPol.SetAttribute("sResponseObject", InputObject["sResponseObject"].ToString());
        //        var Updated = oboiPol.Save(oboiPol);
        //        if (!Updated.bOK && Updated.oResult == null)
        //        {
        //            XIInstanceBase oIB = new XIInstanceBase();
        //            oCResult.sMessage = "ERROR: PFBNPRequests_T Update Failed \r\n";
        //            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //            oCResult.LogToFile();
        //            oIB.SaveErrortoDB(oCResult);
        //        }
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
        //        oCResult.oResult = Updated;
        //        return oCResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //#endregion

        #region TestRequest
        protected bool VerifyBNPConnection()
        {
            bool bresult = true;
            ServiceStatusResponse response = new ServiceStatusResponse();
            ServiceStatusRequest request = new ServiceStatusRequest();
            request.ServiceStatusRequestType = new ServiceStatusRequestType { ProvideStatus = true };
            response = XIServiceStatus(request);
            ErrorList = response.Errors;
            if(ErrorList!=null && ErrorList.Count() > 0)
            {
                string sMesage = string.Empty;
                CResult oCR = new CResult();
                foreach(var item in ErrorList)
                {
                    sMesage = sMesage + "BNPConnection Error -> Code: " + item.Code + ", Description: " + item.Description;
                }
                oCR.sMessage = sMesage;
                SaveErrortoDB(oCR);
            }
            bresult = response.ServiceStatusResponseType.IsServiceAvailable;
            return bresult;
        }
        #endregion

        //#region PFBNPFieldSData
        //private CResult GetPFBNPFields(EnumXIBNPMethods input)
        //{
        //    CResult oCResult = new CResult();
        //    CResult oCR = new CResult();
        //    XIInstanceBase oIB = new XIInstanceBase();
        //    try
        //    {
        //        XIInfraCache oCache = new XIInfraCache();
        //        var oOneClick = (XID1Click)oCache.GetObjectFromCache("oneclick", "PFBNPFields", "");
        //        XIDBO oBOD = new XIDBO();
        //        oBOD = (XIDBO)oCache.GetObjectFromCache("bo", null, oOneClick.BOID.ToString());
        //        oOneClick.BOD = oBOD;
        //        XIDXI oDXI = new XIDXI();
        //        List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
        //        //Dictionary<string, XIIBO> oDictionaryBOI = null;
        //        XIDStructure oStructure = new XIDStructure();
        //        List<CNV> nParams = new List<CNV>();
        //        nParams.Add(new CNV { sName = "{XIP|" + input.ToString() + "}", sValue = input.ToString() });
        //        var sDataSource = oDXI.GetBODataSource(oBOD.iDataSource);
        //        oOneClick.sConnectionString = sDataSource;
        //        oOneClick.BOID = oOneClick.BOID;
        //        oOneClick.Query = "SELECT id AS 'id', sFiledName AS 'FiledName', sType AS 'Type', sServiceName AS 'ServiceName', bIsMandatory AS 'IsMandatory',iMinLength AS 'MinLength',iMaxLength AS 'MaxLength',sRegexp AS 'Regexp', sErrorMessage AS 'ErrorMessage',sNotation AS 'Notation' FROM PFBNPFields_T WHERE sServiceName='" + input.ToString() + "'";
        //        //oOneClick.Query = oStructure.ReplaceExpressionWithCacheValue(oOneClick.Query, nParams);
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
        //        oCResult.oResult = oOneClick.OneClick_Execute();
        //    }
        //    catch (Exception ex)
        //    {
        //        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
        //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
        //        oCResult.LogToFile();
        //        oIB.SaveErrortoDB(oCResult);
        //    }
        //    return oCResult;
        //}
        //#endregion
    }

}
