using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeeBNPPFServices.BNPServiceDev;

namespace ZeeBNPPFServices
{
    public class XIMockAPI
    {

        #region XIBNPConfig
        protected UserCredential GetCredentials()
        {
            UserCredential retunobj = new UserCredential();
            retunobj.Username = "QMT"; //GetValue("");//get username from app.config file
            retunobj.Password = "wn5uVY9kXKpLfdcM";//GetValue(""); //get password from app.config file
            return retunobj;
        }
        protected HeaderWithCredential GetHeader()
        {
            HeaderWithCredential headers = new HeaderWithCredential();
            headers.ShopCode = "9829";
            headers.SystemType = EnumsSystemType.Ipf; //need to change
            headers.TransactionRef = "T" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            headers.UserId = "QMT";//QM1
            headers.ClientCode = "QM1";//QM1
            headers.CostCentre = "123456";
            headers.PartnerTransactionToken = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            headers.UserCredential = GetCredentials();
            return headers;
        }
        #endregion

        #region XIMock_Methods
        public NewBusinessResponse XINewBusiness(NewBusinessRequest request, string ShopeCode = null)
        {
            try
            {
                var response = GetNewBusinessResponse(request);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LiveRateQuoteResponse XILiveRateQuote(LiveRateQuoteRequest request)
        {
            try
            {
                var response = GetLiveRateResponse(request);
                return response;
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
                MidTermAdjustmentResponse response = new MidTermAdjustmentResponse();
                response.Header = GetHeader();
                MidTermAdjustmentResponseType resp = new MidTermAdjustmentResponseType();
                resp.Decision = EnumsDecision.Accepted;
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.MidTermAdjustmentResponseType = resp;
                return response;
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
                CancellationResponse response = new CancellationResponse();
                response.Header = GetHeader();
                CancellationResponseType resp = new CancellationResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.CancellationResponseType = resp;
                return response;
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
                PolicyStatusResponse response = new PolicyStatusResponse();
                response.Header = GetHeader();
                PolicyStatusResponseType resp = new PolicyStatusResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.PolicyStatusResponseType = resp;
                return response;
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
                ServiceStatusResponse response = new ServiceStatusResponse();
                response.Header = GetHeader();
                ServiceStatusResponseType resp = new ServiceStatusResponseType();
                resp.IsServiceAvailable = true;
                response.ServiceStatusResponseType = resp;
                return response;
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
                EditCustomerAddressResponse response = new EditCustomerAddressResponse();
                response.Header = GetHeader();
                EditCustomerAddressResponseType resp = new EditCustomerAddressResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.EditCustomerAddressResponseType = resp;
                return response;
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
                EditCustomerBankResponse response = new EditCustomerBankResponse();
                response.Header = GetHeader();
                EditCustomerBankResponseType resp = new EditCustomerBankResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.EditCustomerBankResponseType = resp;
                return response;
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
                EditCustomerEmailResponse response = new EditCustomerEmailResponse();
                response.Header = GetHeader();
                EditCustomerEmailResponseType resp = new EditCustomerEmailResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.EditCustomerEmailResponseType = resp;
                return response;
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
                EditCustomerPaymentDayResponse response = new EditCustomerPaymentDayResponse();
                response.Header = GetHeader();
                EditCustomerPaymentDayResponseType resp = new EditCustomerPaymentDayResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.EditCustomerPaymentDayResponseType = resp;
                return response;
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
                EditCustomerPhoneResponse response = new EditCustomerPhoneResponse();
                response.Header = GetHeader();
                EditCustomerPhoneResponseType resp = new EditCustomerPhoneResponseType();
                resp.ResponseStatus = EnumsResponseStatus.Success;
                response.EditCustomerPhoneResponseType = resp;
                return response;
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
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private LiveRateQuoteResponse GetLiveRateResponse(LiveRateQuoteRequest request)
        {
            try
            {
                LiveRateQuoteResponse response = new LiveRateQuoteResponse();
                if (request != null)
                {
                    string[] strcodes = { "TNC", "TNE", "TNF", "MINS", "DINS", "CIF1", "CTF1" };
                    string[] fmindeprate = { "10", "20", "0", "30" };
                    string[] decapr = { "44.77", "44.72", "24.58", "31.42" };
                    string[] sterm = { "7", "8", "9", "10" };
                    string[] decselloutrate = { "14.95", "10", "6", "5" };
                    response.Header = GetHeader();
                    List<RateQuote> lstrates = new List<RateQuote>();
                    for (int i = 0; i < 5; i++)
                    {
                        string strcode = GetRandomValuefromObject(strcodes);
                        while (lstrates.Any(d => d.CreditProductCode == strcode))
                        {
                            strcode = GetRandomValuefromObject(strcodes);
                        }
                        RateQuote res = new RateQuote();
                        res.CreditProductCode = strcode;
                        res.InsuranceType = EnumsInsuranceType.PersonalLine;
                        res.MinDepositRate = Convert.ToSingle(GetRandomValuefromObject(fmindeprate));
                        res.RopRateType = EnumsRopRateType.ProRata;
                        res.Apr = Convert.ToDecimal(GetRandomValuefromObject(decapr));
                        res.Term = GetRandomValuefromObject(sterm);
                        res.SelloutRate = Convert.ToDecimal(GetRandomValuefromObject(decselloutrate));
                        lstrates.Add(res);
                    }
                    response.LiveRateQuoteResponseType = lstrates.ToArray();
                }
                else
                {
                    response.Errors = new Error[] { new Error { Description = "Request Object Should not be NULL" } };
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private NewBusinessResponse GetNewBusinessResponse(NewBusinessRequest request)
        {
            try
            {
                NewBusinessResponse response = new NewBusinessResponse();
                if (request != null)
                {
                    response.Header = GetHeader();
                    NewBusinessResponseType req = new NewBusinessResponseType();
                    req.AccountNumber = GenerateRandomNumber(12);
                    req.BrokerReference = request.NewBusinessRequestType.Application.BrokerReference;
                    req.Decision = EnumsDecision.Accepted;//(EnumsDecision)Enum.Parse(typeof(EnumsDecision), GetRandomValue(typeof(EnumsDecision)), true);
                    req.ResponseStatus = EnumsResponseStatus.Success;//req.Decision.ToString() == "Accepted" ? EnumsResponseStatus.Success : EnumsResponseStatus.Fail;
                    response.NewBusinessResponseType = req;
                }
                else
                {
                    response.Errors = new Error[] { new Error { Description = "Request Object Should not be NULL" } };
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetRandomValue(Type type)
        {
            string strresult = string.Empty;
            string[] strarray = { "Accepted", "Declined", "Refer" };
            if (type.IsEnum)
            {
                List<string> lstenums = Enum.GetValues(type).Cast<int>().Select(t => Enum.GetName(type, t)).Where(d => strarray.Contains(d)).ToList();
                Random rand = new Random();
                int Index = rand.Next(lstenums.Count());
                strresult = lstenums[Index];
            }
            return strresult;
        }

        private string GetRandomValuefromObject(string[] strarray)
        {
            string strresult = string.Empty;
            Random rand = new Random();
            int Index = rand.Next(0, strarray.Count());
            strresult = strarray[Index];
            return strresult;
        }
        private string GenerateRandomNumber(int Precision)
        {
            try
            {
                string numbers = "0123456789";
                Random objrandom = new Random();
                StringBuilder strrandom = new StringBuilder();
                int noofnumbers = Precision;
                for (int i = 0; i < noofnumbers; i++)
                {
                    int temp = objrandom.Next(0, numbers.Length);
                    strrandom.Append(temp);
                }
                return strrandom.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

}
