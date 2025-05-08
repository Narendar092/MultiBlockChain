using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XICore;
using XISystem;
using ZeeBNPPFServices.BNPServiceDev;
using static ZeeBNPPFServices.XIPFCommon;

namespace ZeeBNPPFServices
{
    public class XIBNPFileWrapper
    {
        public XIBNPFileWrapper()
        {
        }
        public Dictionary<string, List<Error>> dictError = new Dictionary<string, List<Error>>();

        #region XMLValidation
        private Dictionary<string, XIPFAttribute> ConvertXMlToDictionary(string strinputXMl)
        {
            try
            {
                Dictionary<string, XIPFAttribute> result = new Dictionary<string, XIPFAttribute>();
                // strinputXMl = "<?xml version=\"1.0\" ?><Policies><FileNumber>01</FileNumber><Policy><Header><MCSISupplier>QMT</MCSISupplier>        <MCSISoftware>XML</MCSISoftware>        <MessageCount>000000000001</MessageCount>        <BrokerABICode>A08124</BrokerABICode>        <BrokerId>4720</BrokerId>        <BrokerRef>37</BrokerRef>        <SchemeId>15</SchemeId>        <BusinessId>PV</BusinessId>        <PolicyNumber></PolicyNumber>        <PolicyStartDate>2008-01-16</PolicyStartDate>        <PolicyStartTime></PolicyStartTime>        <PolicyEndDate>2008-01-16</PolicyEndDate>        <SequenceNo></SequenceNo>        <Action></Action>        <IPTExempt></IPTExempt>        <CommissionPercentage>0</CommissionPercentage>        <PremiumIncIPT>0</PremiumIncIPT>      </Header>      <RiskDetails>        <CompanyIndicator></CompanyIndicator>        <BusinessName></BusinessName>        <FullTimeOccupation></FullTimeOccupation>        <FullTimeBusinessType></FullTimeBusinessType>        <RiskAddress>          <AddressL1>23 Manor Road                           </AddressL1>          <AddressL2>Barlestone                              </AddressL2>          <AddressL3>Nuneaton                                </AddressL3>          <AddressL4>Warwickshire                            </AddressL4>          <HomeTelephone>07908 251580</HomeTelephone>          <WorkTelephone>07973 364986ash</WorkTelephone>          <FaxNumber></FaxNumber>          <EmailAddress></EmailAddress>          <NoDirectMail>Cannot Resolve function: translate('N', 'Y','Y','N','N'</NoDirectMail>          <Postcode>CV13 0HY  </Postcode>        </RiskAddress>        <Drivers>           <Driver>            <PersonalDetails>              <Title>001</Title>              <ForeName>John</ForeName>              <Surname>J J</Surname>              <Sex>,10,'M',20,'F')]</Sex>              <DateOfBirth>', 'yyyy-mm-dd')]</DateOfBirth>              <RelationshipToProposer>A</RelationshipToProposer>              <LicenceType>F</LicenceType>              <YearsResidentInUK>full</YearsResidentInUK>              <LicenceHeldDate>', 'yyyy-mm-dd')]</LicenceHeldDate>              <FullTimeOccupation>002</FullTimeOccupation>              <FullTimeBusinessType>1</FullTimeBusinessType>              <FullTimeEmploymentType></FullTimeEmploymentType>              <PartTimeOccupation>002</PartTimeOccupation>              <PartTimeBusinessType></PartTimeBusinessType>              <PartTimeEmploymentType></PartTimeEmploymentType>              <ClaimsLast3Yrs></ClaimsLast3Yrs>              <ConvictionLast5Yrs></ConvictionLast5Yrs>              <MedicalConditions></MedicalConditions>            </PersonalDetails>            <Claims>                </Claims>            <MedicalConditions>                <MedicalCondition>                <DateDiagnosed>func.format(11/12/2006, 'yyyy-mm-dd')</DateDiagnosed>                <Condition>Kleptomania</Condition>              </MedicalCondition>              <MedicalCondition>                <DateDiagnosed>func.format(11/12/2006, 'yyyy-mm-dd')</DateDiagnosed>                <Condition>Kleptomania</Condition>              </MedicalCondition>              </MedicalConditions>            <Convictions>                <Conviction>                <TypeOfConviction></TypeOfConviction>                <ConvictionDate>, 'yyyy-mm-dd')]</ConvictionDate>                <OffenceDate>, 'yyyy-mm-dd')]</OffenceDate>                <TotalFine>99</TotalFine>                <PenaltyPoints>1</PenaltyPoints>                <NoOfMonthsDisqualified>1 month</NoOfMonthsDisqualified>              </Conviction>              </Convictions>            <DriverCover>              <BusinessUse></BusinessUse>              <MainDriver></MainDriver>              <RatedDriver></RatedDriver>              <OtherVehicles></OtherVehicles>            </DriverCover>          </Driver>          </Drivers>        <Vehicles>           <Vehicle>            <ABICode></ABICode>            <Make>BMW</Make>            <Model>318i</Model>            <Year>1994</Year>            <EngineSize>1800</EngineSize>            <Doors>3</Doors>            <ManualAuto></ManualAuto>            <NumberofSeats>5</NumberofSeats>            <PurchaseDate>', 'yyyy-mm-dd')]</PurchaseDate>            <Value>3000</Value>            <Mileage>181282</Mileage>            <VehicleKeptPostCode>SO505SH</VehicleKeptPostCode>            <PillionPassenger>False</PillionPassenger>            <VehicleCover>0</VehicleCover>            <VehicleUse></VehicleUse>            <Garaging></Garaging>            <Security></Security>            <DrivingRestriction></DrivingRestriction>            <NCDYears></NCDYears>            <ProtectedNCD></ProtectedNCD>            <Intro></Intro>            <Registration>L974VLB</Registration>            <LHD>False</LHD>            <Imported>False</Imported>            <Modifications>False</Modifications>            <NotRegOwner>False</NotRegOwner>            <RegisteredOwner></RegisteredOwner>            <VoluntaryExcess></VoluntaryExcess>            <AdditionalDamageExcess></AdditionalDamageExcess>            <AdditionalFireAndTheftExcess></AdditionalFireAndTheftExcess>            <TrailerCoverDetails>                            </TrailerCoverDetails>          </Vehicle>          </Vehicles>      </RiskDetails>    </Policy>  </Policies>";
                XDocument doc = XDocument.Parse(strinputXMl);
                foreach (XElement element in doc.Descendants().Where(p => p.HasElements == false))
                {
                    //int keyInt = 0;
                    string keyName = element.Name.LocalName;
                    XIPFAttribute Xival = new XIPFAttribute();
                    Xival.sValue = element.Value.Trim();
                    if (result.ContainsKey(keyName))
                    {
                        result[keyName] = Xival;// element.Name.LocalName; //+ "_" + keyInt++;
                    }
                    else
                    {
                        result.Add(keyName, Xival);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //private Dictionary<bool, Dictionary<string, XIIValue>> IsValidXMlInputRequest(string xmlinput, EnumXIBNPMethods requestedservice)
        //{
        //    try
        //    {
        //        Dictionary<bool, Dictionary<string, XIIValue>> fresult = new Dictionary<bool, Dictionary<string, XIIValue>>();
        //        bool result = true;
        //        // HERE CONVERT XML STRING TO DICTIONARY OBJECT
        //        Dictionary<string, XIIValue> dictxml = ConvertXMlToDictionary(xmlinput);
        //        dictxml = dictxml ?? new Dictionary<string, XIIValue>();

        //        // LOAD PF BNP FIELDS HERE
        //        var pfbnpfields = GetPFBNPFields(requestedservice);
        //        var reqesult = (Dictionary<string, XIIBO>)pfbnpfields.oResult;
        //        foreach (var itemobj in reqesult)
        //        {
        //            string skeyname = itemobj.Value.AttributeI("FiledName"+ strpostfix).sValue;
        //            bool ismandatory = itemobj.Value.AttributeI("IsMandatory"+ strpostfix).bValue;
        //            if (ismandatory)
        //            {
        //                if (!dictxml.ContainsKey(skeyname)) //if key is not exist in the xml dict return false
        //                {
        //                    result = false;
        //                    break;
        //                }
        //            }
        //        }
        //        fresult.Add(result, dictxml);
        //        return fresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion

        #region ConfigMethods

        private string GetXmlWithContract(object obj)
        {
            try
            {
                var serializer = new DataContractSerializer(obj.GetType(), null, int.MaxValue, true, false, null);
                string xmlString;
                using (var sw = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(sw))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        serializer.WriteObject(writer, obj);
                        writer.Flush();
                        xmlString = sw.ToString();
                    }
                }
                return xmlString;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        private DateTime ConvertToDate(string InputString)
        {
            // * NOTE: below codeto convert string to Datetime with current culture info
            //var currentculture = CultureInfo.CurrentCulture;
            //string getcurrent = currentculture.ToString();
            //CultureInfo us = new CultureInfo(getcurrent);
            //string[] formats = us.DateTimeFormat.GetAllDateTimePatterns();
            //CultureInfo provider = CultureInfo.InvariantCulture;
            //string InputString = "31.12.18";
            //DateTime dateValue;
            //bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
            CultureInfo provider = CultureInfo.InvariantCulture;
            string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy"
};
            DateTime dateValue;
            bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
            if (!IsValidDate)
            {
                dateValue = DateTime.MinValue;
            }
            return dateValue;
        }

        private bool IsErrorsExist(Error[] errolist, XIPFCResult fresultobj)
        {
            bool result = false;
            if (errolist != null)
            {
                if (errolist.Count() > 0)
                {
                    result = true;
                    fresultobj.xiStatus = EnumxiStatus.error.ToString();
                }
            }
            return result;
        }
        private void SetResponseError(EnumsResponseStatus input, XIPFCResult fresultobj)
        {
            if (input.ToString() == EnumsResponseStatus.Success.ToString())
            {
                fresultobj.xiStatus = EnumxiStatus.sucess.ToString();
            }
            else
            {
                fresultobj.xiStatus = EnumxiStatus.error.ToString();
            }
        }

        #region LoadingBNP_Services
        private T GetInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
        public XIPFCResult LoadXIBNPAPI(EnumXIBNPMethods enummethod, string xmlinput, Dictionary<string, XIPFfields> pffieldslist)
        {
            XIPFCResult oPFResult = new XIPFCResult();
            dynamic xibnpapi = GetInstance<XIBNPAPI>();
            string RequestMode = ConfigurationManager.AppSettings["BNPRequestMode"];
            if (RequestMode == "Mock")
            {
                xibnpapi = GetInstance<XIMockAPI>();
            }
            try
            {
                object responseobj = null;
                //CONVERT XML OBJECT TO REQUIRED OBJECT
                var reqobj = ConvertXMlToDictionary(xmlinput);
                if (pffieldslist.Count <= 0)
                {
                    oPFResult.xiStatus = EnumxiStatus.error.ToString();
                    oPFResult.OResult = "PF fields Object not given";
                    return oPFResult;
                }
                var isvalirequest = IsValidInputObject(reqobj, enummethod, pffieldslist);

                oPFResult.bprevalidation = false;
                oPFResult.xiStatus = EnumxiStatus.sucess.ToString();
                if (isvalirequest.Count() <= 0) //if valid input object
                {
                    oPFResult.bprevalidation = true;
                    Dictionary<string, XIPFAttribute> _dict = reqobj;//reg.Value;
                    XIPFValues xiqs = new XIPFValues();
                    xiqs.XIValues = _dict;

                    switch (enummethod)
                    {
                        case EnumXIBNPMethods.LiveRateQuote:
                            {
                                LiveRateQuoteRequest request = new LiveRateQuoteRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                request = (LiveRateQuoteRequest)_req;
                                responseobj = xibnpapi.XILiveRateQuote(request);
                                var _resp = (LiveRateQuoteResponse)responseobj;
                                IsErrorsExist(_resp.Errors, oPFResult);
                                break;
                            }
                        case EnumXIBNPMethods.NewBusiness:
                            {
                                NewBusinessRequest request = new NewBusinessRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (NewBusinessRequest)_req;
                                    responseobj = xibnpapi.XINewBusiness(request);
                                    var _resp = (NewBusinessResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.NewBusinessResponseType.ResponseStatus, oPFResult);
                                    }
                                    //if (!string.IsNullOrEmpty(_resp.NewBusinessResponseType.Pdf))
                                    //{
                                    //    var pdfbytes = Convert.FromBase64String(_resp.NewBusinessResponseType.Pdf);
                                    //    File.WriteAllBytes(@"E:\Documents\XISystems\PFPdfs\pdffile2.pdf", pdfbytes)
                                    //        utility.SaveFile;
                                    //}
                                }
                                break;
                            }
                        case EnumXIBNPMethods.MidTermAdjustment:
                            {
                                MidTermAdjustmentRequest request = new MidTermAdjustmentRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (MidTermAdjustmentRequest)_req;
                                    responseobj = xibnpapi.XIMidTermAdjustment(request);
                                    var _resp = (MidTermAdjustmentResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.MidTermAdjustmentResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.Cancellation:
                            {
                                MidTermAdjustmentRequest request = new MidTermAdjustmentRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (MidTermAdjustmentRequest)_req;
                                    responseobj = xibnpapi.XIMidTermAdjustment(request);
                                    var _resp = (MidTermAdjustmentResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.MidTermAdjustmentResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.PolicyStatus:
                            {
                                PolicyStatusRequest request = new PolicyStatusRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (PolicyStatusRequest)_req;
                                    responseobj = xibnpapi.XIPolicyStatus(request);
                                    var _resp = (PolicyStatusResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.PolicyStatusResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.ServiceStatus:
                            {
                                ServiceStatusRequest request = new ServiceStatusRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (ServiceStatusRequest)_req;
                                    responseobj = xibnpapi.XIServiceStatus(request);
                                    var _resp = (ServiceStatusResponse)responseobj;
                                    IsErrorsExist(_resp.Errors, oPFResult);
                                }
                                break;
                            }
                        case EnumXIBNPMethods.EditCustomerAddress:
                            {
                                EditCustomerAddressRequest request = new EditCustomerAddressRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (EditCustomerAddressRequest)_req;
                                    responseobj = xibnpapi.XIEditCustomerAddress(request);
                                    var _resp = (EditCustomerAddressResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.EditCustomerAddressResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.EditCustomerBank:
                            {
                                EditCustomerBankRequest request = new EditCustomerBankRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (EditCustomerBankRequest)_req;
                                    responseobj = xibnpapi.XIEditCustomerBank(request);
                                    var _resp = (EditCustomerBankResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.EditCustomerBankResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.EditCustomerEmail:
                            {
                                EditCustomerEmailRequest request = new EditCustomerEmailRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (EditCustomerEmailRequest)_req;
                                    responseobj = xibnpapi.XIEditCustomerEmail(request);
                                    var _resp = (EditCustomerEmailResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.EditCustomerEmailResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.EditCustomerPaymentDay:
                            {
                                EditCustomerPaymentDayRequest request = new EditCustomerPaymentDayRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (EditCustomerPaymentDayRequest)_req;
                                    responseobj = xibnpapi.XIEditCustomerPaymentDay(request);
                                    var _resp = (EditCustomerPaymentDayResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.EditCustomerPaymentDayResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.EditCustomerPhone:
                            {
                                EditCustomerPhoneRequest request = new EditCustomerPhoneRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (EditCustomerPhoneRequest)_req;
                                    responseobj = xibnpapi.XIEditCustomerPhone(request);
                                    var _resp = (EditCustomerPhoneResponse)responseobj;
                                    if (!IsErrorsExist(_resp.Errors, oPFResult))
                                    {
                                        SetResponseError(_resp.EditCustomerPhoneResponseType.ResponseStatus, oPFResult);
                                    }
                                }
                                break;
                            }
                        case EnumXIBNPMethods.PartnerQuote:
                            {
                                PartnerQuoteRequest request = new PartnerQuoteRequest();
                                var _req = BuildRequestObject(xiqs, enummethod, null, null);
                                if (_req != null)
                                {
                                    request = (PartnerQuoteRequest)_req;
                                    responseobj = xibnpapi.XIPartnerQuote(request);
                                    var _resp = (PartnerQuoteResponse)responseobj;
                                    IsErrorsExist(_resp.Errors, oPFResult);
                                }
                                break;
                            }
                    }
                }
                else
                {
                    responseobj = isvalirequest;
                    oPFResult.xiStatus = EnumxiStatus.error.ToString();
                }
                oPFResult.OResult = responseobj;
                return oPFResult;
            }
            catch (Exception ex)
            {
                oPFResult.xiStatus = EnumxiStatus.error.ToString();
                oPFResult.xiErrorMessage = ex.Message.ToString();
                oPFResult.IsException = true;
                oPFResult.OResult = ex;
                oPFResult.bprevalidation = false;
                return oPFResult;
            }
        }
        #endregion

        #region VALIDATION
        public Dictionary<string, List<Error>> IsValidInputObject(Dictionary<string, XIPFAttribute> Inputobj, EnumXIBNPMethods requestedservice, Dictionary<string, XIPFfields> dictpffields)
        {
            try
            {
                dictError = new Dictionary<string, List<Error>>();
                foreach (KeyValuePair<string, XIPFfields> itemobj in dictpffields) // Iterations 
                {
                    string skeyname = itemobj.Key;
                    bool ismandatory = itemobj.Value.bMandatory;
                    int minlength = itemobj.Value.iMinlength;
                    int maxlength = itemobj.Value.iMaxlength;
                    string Regexp = itemobj.Value.sRegexp;
                    string ErrorMsg = itemobj.Value.sErrorMessage;
                    string dType = itemobj.Value.sType;
                    string Notation = itemobj.Value.sNotation;
                    string sXIFieldName = itemobj.Value.sXIFieldName;
                    string sfDisplayName = itemobj.Value.sfDisplayName;
                    List<string> lsterrors = new List<string>();
                    if (!string.IsNullOrEmpty(ErrorMsg))
                    {
                        lsterrors = ErrorMsg.Split('|').ToList();
                    }

                    if (!Inputobj.ContainsKey(skeyname)) // Key exist or not Validation Here
                    {
                        //result = false;
                        if (ismandatory)
                            AddError(skeyname, lsterrors, "$KEY$", sfDisplayName);
                    }
                    else
                    {
                        // VALIDATE FIELD DATA TYPE HERE if the Key is exist
                        string keyvalue = string.IsNullOrEmpty(Inputobj[skeyname].sValue) ? Inputobj[skeyname].sValue : (Inputobj[skeyname].sValue.Length > maxlength && maxlength > 0) ? Inputobj[skeyname].sValue.Substring(0, maxlength) : Inputobj[skeyname].sValue;
                        keyvalue = keyvalue ?? "";
                        if (!string.IsNullOrEmpty(keyvalue))
                        {
                            if (dType != "System.String")
                            {
                                if (dType == "System.Boolean" && keyvalue.ToLower() == "on")
                                {
                                    keyvalue = "true";
                                }
                                else if (dType == "System.Boolean" && keyvalue.ToLower() == "off")
                                {
                                    keyvalue = "false";
                                }
                                if (!TryConvert(dType, keyvalue))
                                {
                                    AddError(skeyname, lsterrors, "$DATA$", sfDisplayName);
                                }
                            }
                            // Perform Regexp Validations
                            if (!string.IsNullOrEmpty(Regexp))
                            {
                                Regex regxppattern = new Regex(Regexp);
                                var match = Regex.Match(keyvalue, Regexp.Trim());
                                if (!match.Success && match.Value != keyvalue)
                                {
                                    AddError(skeyname, lsterrors, "$REGEXP$", sfDisplayName);
                                }
                            }
                            // PERFORM MAX MIN VALIDATIONS EHERE
                            if ((minlength > 0 && minlength > keyvalue.Length) || (maxlength > 0 && maxlength < keyvalue.Length)) //if(minlength <= keyvalue.Length && maxlength >= keyvalue.Length)
                            {
                                AddError(skeyname, lsterrors, "$MINMAX$", sfDisplayName);
                            }
                            // PERFORM NOTATION EXECUTION
                            if (!string.IsNullOrEmpty(Notation))
                            {
                                int _res = NotationEvaluation(skeyname, Inputobj, Notation);
                                if (_res == 1)
                                {
                                    AddError(skeyname, lsterrors, "$KEY$", sfDisplayName);
                                }
                                else if (_res == 3) //Log NOTATION Error
                                {
                                    AddError(skeyname, lsterrors, "$NOTATION$", sfDisplayName);
                                }
                            }
                            //DATE OF BIRTH VALIDATION 
                            if (skeyname == "DateOfBirthPF")
                            {
                                if (!string.IsNullOrEmpty(keyvalue))
                                {
                                    DateTime dgvndt = ConvertToDate(keyvalue);
                                    if (dgvndt != DateTime.MinValue)
                                    {
                                        int _Age = Convert.ToInt32((DateTime.Now - dgvndt).TotalDays) / 365;
                                        if (_Age <= 18)
                                        {
                                            AddError(skeyname, lsterrors, "$NOTATION$", sfDisplayName);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ismandatory)
                                AddError(skeyname, lsterrors, "$KEY$", sfDisplayName);
                        }
                    }
                }
                //dictError = new Dictionary<string, List<Error>>();
                return dictError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int NotationEvaluation(string skey, Dictionary<string, XIPFAttribute> Inputobj, string Notation)
        {
            try
            {
                int res = 1; // 1= Keys Not present in the dictionary,2= All keys Exist but
                var Columns = Notation.Split('|')[1].Split(',').Select(f => f.Trim()).ToList();
                if (Columns.Count() > 0) //Columns exist
                {
                    Columns.Add(skey);
                    bool ismatched = Utility.IsMatchedwithAny(dictError.Select(d => d.Key).ToList(), Columns);
                    if (!ismatched)
                    {
                        /* BELOW CODE IS SIMPLIFED WITHOUT FOREACH AND NEED TO CHECK */
                        //int matchingCount = Inputobj.Select(f => f.Key).ToList().Intersect(Columns).Count();
                        //if (matchingCount == Columns.Count())
                        //{
                        //    res = 2;//existing all the keys in dictionary
                        //}
                        //else
                        //{
                        //    res = 1;
                        //}
                        foreach (var item in Columns)
                        {
                            if (!Inputobj.ContainsKey(item)) //check in the dictionary all keys that need to evaluate expression
                            {
                                res = 1;
                                break;
                            }
                            else
                            {
                                res = 2;//existing all the keys in dictionary
                            }
                        }
                        if (res == 2) // then build data table
                        {
                            DataTable dtobj = new DataTable();
                            foreach (var clm in Columns)
                            {
                                DataColumn _dtclm = new DataColumn();
                                _dtclm.DataType = typeof(string);
                                _dtclm.ColumnName = clm;
                                _dtclm.DefaultValue = Inputobj[clm].sValue;
                                dtobj.Columns.Add(_dtclm);
                            }
                            DataColumn dtres = new DataColumn();
                            dtres.DataType = typeof(Boolean);
                            dtres.ColumnName = "Result";
                            dtres.Expression = Notation.Split('|')[0].Trim();
                            dtobj.Columns.Add(dtres);
                            System.Data.DataRow r = dtobj.NewRow();
                            dtobj.Rows.Add(r);
                            Boolean result = (Boolean)r[Columns.Count()];
                            if (result)
                                res = 3;
                        }
                    } //end for 2nd IF
                    else
                        res = 2;
                }
                return res;
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }
        private bool TryConvert(string DataType, string svalue)
        {
            bool result = false;
            if (DataType == "System.Decimal")
            {
                decimal _reqtype;
                result = decimal.TryParse(svalue, out _reqtype);
            }
            else if (DataType == "System.Date")
            {
                DateTime _reqtype;
                result = DateTime.TryParse(svalue, out _reqtype);
            }
            else if (DataType == "System.Boolean")
            {
                bool _reqtype;
                result = Boolean.TryParse(svalue, out _reqtype);
            }
            else if (DataType == "System.Int32")
            {
                int _reqtype;
                result = int.TryParse(svalue, out _reqtype);
            }
            else if (DataType == "EnumsInsuranceType")
            {
                result = Enum.IsDefined(typeof(EnumsInsuranceType), svalue);
            }
            else if (DataType == "EnumsPolicyType")
            {
                result = Enum.IsDefined(typeof(EnumsPolicyType), svalue);
            }
            else if (DataType == "EnumsRopRateType")
            {
                result = Enum.IsDefined(typeof(EnumsRopRateType), svalue);
            }
            else if (DataType == "EnumsCustomerBusinessType")
            {
                result = Enum.IsDefined(typeof(EnumsCustomerBusinessType), svalue);
            }
            else if (DataType == "EnumsPolicyStatusType")
            {
                result = Enum.IsDefined(typeof(EnumsPolicyStatusType), svalue);
            }
            else if (DataType == "System.String")
            {
                result = true;
            }
            return result;
        }

        private void AddError(string skey, List<string> lstErrors, string errortype, string sfDisplayName)
        {
            string Errormsg = string.Empty;
            Errormsg = lstErrors.Where(f => f.Contains(errortype)).Select(t => t).FirstOrDefault();
            if (!string.IsNullOrEmpty(Errormsg))
                Errormsg = Errormsg.Replace(errortype, "").Replace("{skey}", " " + sfDisplayName);
            else
                Errormsg = "";
            if (dictError.ContainsKey(skey))
            {
                dictError[skey].Add(new Error { Description = Errormsg.Trim() });
            }
            else
            {
                dictError.Add(skey, new List<Error> { new Error { Description = Errormsg.Trim() } });
            }
        }
        #endregion

        #region Build_RequestObject
        public object BuildRequestObject(XIPFValues InputObj, EnumXIBNPMethods requestedservice, string BrokerReference, Dictionary<string, XIPFfields> dictpffields)
        {
            try
            {
                string strpostfix = "PF";
                object resutlobj = null;
                if (EnumXIBNPMethods.NewBusiness == requestedservice)
                {
                    NewBusinessRequest request = new NewBusinessRequest();
                    NewBusinessRequestType newbreq = new NewBusinessRequestType();
                    Policy policy = new Policy();
                    BankAccount bank = new BankAccount();
                    Customer customer = new Customer();
                    Applicant applicant = new Applicant();
                    Address address = new Address();
                    Application application = new Application();
                    Telephone telephone = new Telephone();
                    Employment employee = new Employment();
                    Payment payment = new Payment();
                    payment.ThirdPartyApplicantAddress = new Address();
                    Marketing marketing = new Marketing();
                    Income income = new Income();
                    Affordability affordability = new Affordability();
                    PersonalTrading personalTrading = new PersonalTrading();

                    application.InsuranceType = EnumsInsuranceType.PersonalLine;//InputObj.GetXIPFValue("InsuranceType"+ strpostfix).iValue == 10 ? EnumsInsuranceType.PersonalLine : InputObj.GetXIPFValue("InsuranceType"+ strpostfix).iValue == 20 ? EnumsInsuranceType.CommercialLine : EnumsInsuranceType.Undefined;
                    // newbreq.Applicant /* TO DO Assign (Affordability,Bank,Customer,Income,Marketing,Payment,Trading,Commercial)
                    application.IsRenewal = InputObj.GetXIPFValue("IsRenewal" + strpostfix).bValue;
                    application.AllowSearchAuthorisation = InputObj.GetXIPFValue("AllowSearchAuthorisation" + strpostfix).bValue;
                    application.IsCustomerPresent = InputObj.GetXIPFValue("IsCustomerPresent" + strpostfix).bValue;
                    application.HasCustomerDeclaration = InputObj.GetXIPFValue("HasCustomerDeclaration" + strpostfix).bValue;
                    application.BrokerReference = BrokerReference;

                    policy.PolicyType = EnumsPolicyType.Motor; //need to change
                    policy.PolicyInceptionDate = InputObj.GetXIPFValue("PolicyInceptionDate" + strpostfix).sValue;//Convert.ToDateTime(InputObj.GetXIPFValue("PolicyInceptionDate" + strpostfix).sValue).ToString("yyyy-MM-dd");
                    policy.PolicyRenewalDate = InputObj.GetXIPFValue("PolicyRenewalDate" + strpostfix).sValue;//Convert.ToDateTime(InputObj.GetXIPFValue("PolicyRenewalDate" + strpostfix).sValue).ToString("yyyy-MM-dd");
                    policy.TotalCashPrice = InputObj.GetXIPFValue("TotalCashPrice" + strpostfix).rValue;
                    policy.Deposit = InputObj.GetXIPFValue("Deposit" + strpostfix).rValue;
                    policy.CreditProductCode = InputObj.GetXIPFValue("CreditProductCode" + strpostfix).sValue;
                    policy.Term = InputObj.GetXIPFValue("Term" + strpostfix).iValue;

                    //PreferredPaymentDay = between 1 to 28
                    policy.PreferredPaymentDay = InputObj.GetXIPFValue("PreferredPaymentDay" + strpostfix).iValue == 0 ?
                       Convert.ToDateTime(InputObj.GetXIPFValue("PolicyInceptionDate" + strpostfix).sValue).Day > 28 ? 28 :
                       Convert.ToDateTime(InputObj.GetXIPFValue("PolicyInceptionDate" + strpostfix).sValue).Day
                       : InputObj.GetXIPFValue("PreferredPaymentDay" + strpostfix).iValue;

                    policy.PolicyPremiumAmount = InputObj.GetXIPFValue("PolicyPremiumAmount" + strpostfix).rValue + InputObj.GetXIPFValue("InsurerCharge" + strpostfix).rValue;// - InputObj.GetXIPFValue("BrokerFeeAmount" + strpostfix).rValue;
                    policy.BrokerCreditFeeAmount = InputObj.GetXIPFValue("BrokerCreditFeeAmount" + strpostfix).rValue;
                    policy.BrokerCreditFeeDescription = InputObj.GetXIPFValue("BrokerCreditFeeDescription" + strpostfix).sValue;
                    //BrokerFeeAmount = Admin Charges(not ADDON admin) + Policy Agrrangement fee
                    policy.BrokerFeeAmount = InputObj.GetXIPFValue("BrokerFeeAmount" + strpostfix).rValue + (InputObj.GetXIPFValue("TotalAdmin" + strpostfix).rValue > 0 ? (InputObj.GetXIPFValue("TotalAdmin" + strpostfix).rValue - InputObj.GetXIPFValue("AddonAdmin" + strpostfix).rValue) : 0);
                    policy.BrokerFeeAmountDescription = InputObj.GetXIPFValue("BrokerFeeAmountDescription" + strpostfix).sValue;

                    //AddOnAmount = ADDON + ADDON admin
                    policy.AddOnAmount = InputObj.GetXIPFValue("AddOnAmount" + strpostfix).rValue + InputObj.GetXIPFValue("AddonAdmin" + strpostfix).rValue;
                    policy.AddOnAmountDescription = "add-ons";//InputObj.GetXIPFValue("AddOnAmountDescription" + strpostfix).sValue.
                                                              // Substring(0, dictpffields["AddOnAmountDescription" + strpostfix].iMaxlength);

                    //DiscountAmount = -ve admin Charges
                    policy.DiscountAmount = (InputObj.GetXIPFValue("TotalAdmin" + strpostfix).rValue - InputObj.GetXIPFValue("AddonAdmin" + strpostfix).rValue) < 0 ?
                        (InputObj.GetXIPFValue("TotalAdmin" + strpostfix).rValue - InputObj.GetXIPFValue("AddonAdmin" + strpostfix).rValue) * (-1) :
                        InputObj.GetXIPFValue("DiscountAmount" + strpostfix).rValue;
                    policy.DiscountAmountDescription = InputObj.GetXIPFValue("DiscountAmountDescription" + strpostfix).sValue;
                    policy.PremiumDescription = InputObj.GetXIPFValue("PremiumDescription" + strpostfix).sValue;
                    policy.PolicyNumber = InputObj.GetXIPFValue("PolicyID").sValue;
                    policy.InsuranceCompany = InputObj.GetXIPFValue("InsuranceCompany" + strpostfix).sValue;
                    policy.RopRateType = (EnumsRopRateType)Enum.Parse(typeof(EnumsRopRateType), InputObj.GetXIPFValue("RopRateType" + strpostfix).sValue, true);
                    policy.IsEdiFlag = InputObj.GetXIPFValue("IsEdiFlag" + strpostfix).bValue;

                    bank.BankAccountNumber = InputObj.GetXIPFValue("BankAccountNumber" + strpostfix).sValue;
                    bank.BankSortCode = InputObj.GetXIPFValue("BankSortCode" + strpostfix).sValue;

                    customer.FirstName = InputObj.GetXIPFValue("FirstName" + strpostfix).sValue;
                    customer.LastName = InputObj.GetXIPFValue("LastName" + strpostfix).sValue;
                    customer.DateOfBirth = InputObj.GetXIPFValue("DateOfBirth" + strpostfix).sValue;
                    customer.Email = InputObj.GetXIPFValue("Email" + strpostfix).sValue;
                    customer.Title = InputObj.GetXIPFValue("Title" + strpostfix).sValue;
                    customer.PolicyholderName = InputObj.GetXIPFValue("PolicyholderName" + strpostfix).sValue;
                    customer.Nationality = InputObj.GetXIPFValue("Nationality" + strpostfix).sValue;
                    customer.CountryOfBirth = InputObj.GetXIPFValue("CountryOfBirth" + strpostfix).sValue;
                    customer.YearsAtAddress = InputObj.GetXIPFValue("YearsAtAddress" + strpostfix).iValue;

                    address.HouseName = InputObj.GetXIPFValue("HouseName" + strpostfix).sValue;
                    address.HouseNumber = InputObj.GetXIPFValue("HouseNumber" + strpostfix).sValue;
                    address.StreetName = InputObj.GetXIPFValue("StreetName" + strpostfix).sValue;
                    address.Townland = InputObj.GetXIPFValue("Townland" + strpostfix).sValue;
                    address.PostTown = InputObj.GetXIPFValue("PostTown" + strpostfix).sValue;
                    address.County = InputObj.GetXIPFValue("County" + strpostfix).sValue;
                    address.PostCode = InputObj.GetXIPFValue("PostCode" + strpostfix).sValue;
                    address.AddressLineOne = address.HouseNumber + "," + address.HouseName + "," + address.StreetName;

                    telephone.MobileNumber = InputObj.GetXIPFValue("MobileNumber" + strpostfix).sValue;
                    telephone.PhoneNumber = InputObj.GetXIPFValue("PhoneNumber" + strpostfix).sValue;

                    employee.EmploymentLevel = InputObj.GetXIPFValue("EmploymentLevel" + strpostfix).bValue;
                    employee.EmploymentStatus = InputObj.GetXIPFValue("EmploymentStatus" + strpostfix).sValue;
                    employee.IndustrySector = InputObj.GetXIPFValue("IndustrySector" + strpostfix).sValue;
                    employee.EmploymentSector = InputObj.GetXIPFValue("EmploymentSector" + strpostfix).sValue;
                    //Payment
                    payment.IsJointAccount = InputObj.GetXIPFValue("IsJointAccount" + strpostfix).bValue;
                    payment.IsApplicantAccount = InputObj.GetXIPFValue("IsApplicantAccount" + strpostfix).bValue;
                    payment.ThirdPartyAccountName = InputObj.GetXIPFValue("ThirdPartyAccountName" + strpostfix).sValue;
                    payment.ThirdPartyApplicantAddress.HouseName = InputObj.GetXIPFValue("ThirdPartyApplicantHousename").sValue;
                    payment.ThirdPartyApplicantAddress.HouseNumber = InputObj.GetXIPFValue("ThirdPartyApplicantHouseNumber").sValue;
                    payment.ThirdPartyApplicantAddress.StreetName = InputObj.GetXIPFValue("ThirdPartyApplicantstreetname").sValue;
                    payment.ThirdPartyApplicantAddress.Townland = InputObj.GetXIPFValue("sThirdPartyApplicanttown").sValue;
                    payment.ThirdPartyApplicantAddress.County = InputObj.GetXIPFValue("ThirdPartyApplicantcounty").sValue;
                    payment.ThirdPartyApplicantAddress.PostCode = InputObj.GetXIPFValue("ThirdPartyApplicantPostcode").sValue;
                    //marketing
                    marketing.IsMarketingOptInEmail = InputObj.GetXIPFValue("IsMarketingOptInEmail" + strpostfix).bValue;
                    marketing.IsMarketingOptInSms = InputObj.GetXIPFValue("IsMarketingOptInSms" + strpostfix).bValue;
                    marketing.IsMarketingOptInPost = InputObj.GetXIPFValue("IsMarketingOptInPost" + strpostfix).bValue;
                    marketing.IsMarketingOptInPhone = InputObj.GetXIPFValue("IsMarketingOptInPhone" + strpostfix).bValue;

                    //income
                    income.NetMonthlyIncome = InputObj.GetXIPFValue("NetMonthlyIncome" + strpostfix).rValue;

                    //affordability
                    affordability.NetMonthlyIncome = InputObj.GetXIPFValue("NetMonthlyIncome" + strpostfix).rValue;
                    affordability.SpousePartnerNetMonthyIncome = InputObj.GetXIPFValue("SpousePartnerNetMonthyIncome" + strpostfix).rValue;
                    affordability.MonthlyMortgageRentRepayment = InputObj.GetXIPFValue("MonthlyMortgageRentRepayment" + strpostfix).rValue;
                    affordability.MaritalStatus = InputObj.GetXIPFValue("MaritalStatus" + strpostfix).sValue;
                    affordability.ResidentialStatus = InputObj.GetXIPFValue("ResidentialStatus" + strpostfix).sValue;
                    affordability.NumberOfDependants = InputObj.GetXIPFValue("NumberOfDependants" + strpostfix).iValue;

                    //personalTrading
                    personalTrading.TradeOutsideUk = InputObj.GetXIPFValue("TradeOutsideUk" + strpostfix).bValue;
                    personalTrading.TradingName = InputObj.GetXIPFValue("TradingName" + strpostfix).sValue;

                    newbreq.Application = application;
                    newbreq.Applicant = applicant;
                    newbreq.Policy = policy;
                    newbreq.Applicant.Customer = customer;
                    newbreq.Applicant.Income = income;
                    newbreq.Applicant.Marketing = marketing;
                    newbreq.Applicant.Payment = payment;
                    newbreq.Applicant.Affordability = affordability;
                    newbreq.Applicant.Customer.Employment = employee;
                    if (customer.YearsAtAddress < 3)
                    {
                        Address paddress = new Address();
                        paddress.HouseName = InputObj.GetXIPFValue("PrevHouseName" + strpostfix).sValue;
                        paddress.HouseNumber = InputObj.GetXIPFValue("PrevHouseNumber" + strpostfix).sValue;
                        paddress.StreetName = InputObj.GetXIPFValue("PrevStreetName" + strpostfix).sValue;
                        paddress.Townland = InputObj.GetXIPFValue("PrevTownland" + strpostfix).sValue;
                        paddress.PostTown = InputObj.GetXIPFValue("PrevPostTown" + strpostfix).sValue;
                        paddress.County = InputObj.GetXIPFValue("PrevCounty" + strpostfix).sValue;
                        paddress.PostCode = InputObj.GetXIPFValue("PrevPostCode" + strpostfix).sValue;
                        paddress.AddressLineOne = paddress.HouseNumber + "," + paddress.HouseName + "," + paddress.StreetName;
                        newbreq.Applicant.Customer.PreviousAddress = paddress;
                    }
                    newbreq.Applicant.Customer.Telephone = telephone;
                    newbreq.Applicant.Customer.Address = address;
                    newbreq.Applicant.Bank = bank;
                    request.NewBusinessRequestType = newbreq;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.LiveRateQuote == requestedservice)
                {
                    LiveRateQuoteRequest request = new LiveRateQuoteRequest();
                    request.LiveRateQuoteRequestType = new LiveRateQuoteRequestType { LiveRateRequestFlag = InputObj.GetXIPFValue("LiveRateRequestFlag").bValue };
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.MidTermAdjustment == requestedservice)
                {
                    MidTermAdjustmentRequest request = new MidTermAdjustmentRequest();
                    MidTermAdjustmentRequestType req = new MidTermAdjustmentRequestType();
                    req.BrokerReference = BrokerReference;//InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue;
                    req.AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue;
                    req.FundingDifference = InputObj.GetXIPFValue("FundingDifference" + strpostfix).rValue;
                    req.IsApplyPremium = InputObj.GetXIPFValue("ApplyPremium" + strpostfix).bValue;
                    req.MtaReasonDescription = InputObj.GetXIPFValue("MtaReasonDescription" + strpostfix).sValue;
                    req.MtaFeeAmount = InputObj.GetXIPFValue("MtaFeeAmount" + strpostfix).rValue;
                    req.IsEdiFlag = InputObj.GetXIPFValue("IsEdiFlag" + strpostfix).bValue;
                    req.PolicyNumber = InputObj.GetXIPFValue("PolicyNumber" + strpostfix).sValue;
                    //Affordability affor = new Affordability(); // /* TO DO Need to set this Object based on Condition
                    //affor.NetMonthlyIncome = InputObj.GetXIPFValue("NetMonthlyIncome"+ strpostfix).rValue;
                    //affor.SpousePartnerNetMonthyIncome = InputObj.GetXIPFValue("SpousePartnerNetMonthyIncome"+ strpostfix).rValue;
                    //affor.MonthlyMortgageRentRepayment = InputObj.GetXIPFValue("MonthlyMortgageRentRepayment"+ strpostfix).rValue;
                    //affor.MaritalStatus = InputObj.GetXIPFValue("MaritalStatus"+ strpostfix).sValue;
                    //affor.ResidentialStatus = InputObj.GetXIPFValue("ResidentialStatus"+ strpostfix).sValue;
                    //affor.NumberOfDependants = InputObj.GetXIPFValue("NumberOfDependants"+ strpostfix).iValue;
                    //req.Affordability = affor;
                    request.MidTermAdjustmentRequestType = req;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.Cancellation == requestedservice)
                {
                    CancellationRequest request = new CancellationRequest();
                    CancellationRequestType can = new CancellationRequestType();
                    can.BrokerReference = BrokerReference;//InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue;
                    can.AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue;
                    can.ReturnOfPremiumValue = InputObj.GetXIPFValue("ReturnOfPremiumValue" + strpostfix).rValue;
                    can.CancellationReasonType = InputObj.GetXIPFValue("CancellationReasonType" + strpostfix).sValue;
                    can.PolicyCancellationDate = InputObj.GetXIPFValue("PolicyCancellationDate" + strpostfix).sValue;
                    can.InsurerReturnOfPremiumValue = InputObj.GetXIPFValue("InsurerReturnOfPremiumValue" + strpostfix).rValue;
                    can.RopDeductionAmount = InputObj.GetXIPFValue("RopDeductionAmount" + strpostfix).rValue;
                    request.CancellationRequestType = can;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.PolicyStatus == requestedservice)
                {
                    PolicyStatusRequest request = new PolicyStatusRequest();
                    PolicyStatusRequestType plc = new PolicyStatusRequestType();
                    plc.BrokerReference = BrokerReference;// InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue;
                    plc.AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue;
                    plc.PolicyStatusType = (EnumsPolicyStatusType)Enum.Parse(typeof(EnumsPolicyStatusType), InputObj.GetXIPFValue("PolicyStatusType" + strpostfix).sValue, true);
                    request.PolicyStatusRequestType = plc;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.ServiceStatus == requestedservice) // /* TO KNOW THE SERVICE STATUS TO DO
                {
                    ServiceStatusRequest request = new ServiceStatusRequest();
                    request.ServiceStatusRequestType = new ServiceStatusRequestType { ProvideStatus = true };
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.EditCustomerAddress == requestedservice)
                {
                    EditCustomerAddressRequest request = new EditCustomerAddressRequest();
                    EditCustomerAddressRequestType edit = new EditCustomerAddressRequestType();
                    edit.BrokerReference = BrokerReference;// DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    edit.AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue;
                    Address adr = new Address();
                    adr.HouseName = InputObj.GetXIPFValue("HouseName" + strpostfix).sValue;
                    adr.HouseNumber = InputObj.GetXIPFValue("HouseNumber" + strpostfix).sValue;
                    adr.StreetName = InputObj.GetXIPFValue("StreetName" + strpostfix).sValue;
                    adr.Townland = InputObj.GetXIPFValue("Townland" + strpostfix).sValue;
                    adr.PostTown = InputObj.GetXIPFValue("PostTown" + strpostfix).sValue;
                    adr.County = InputObj.GetXIPFValue("County" + strpostfix).sValue;
                    adr.PostCode = InputObj.GetXIPFValue("PostCode" + strpostfix).sValue;
                    adr.AddressLineOne = adr.HouseNumber + "," + adr.HouseName + "," + adr.StreetName;
                    edit.Address = adr;
                    request.EditCustomerAddressRequestType = edit;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.EditCustomerBank == requestedservice)
                {
                    EditCustomerBankRequest request = new EditCustomerBankRequest();
                    EditCustomerBankRequestType edit = new EditCustomerBankRequestType();
                    edit.BrokerReference = BrokerReference;//InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue;
                    edit.AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue;
                    BankAccount bank = new BankAccount();
                    bank.BankAccountNumber = InputObj.GetXIPFValue("BankAccountNumber" + strpostfix).sValue;
                    bank.BankSortCode = InputObj.GetXIPFValue("BankSortCode" + strpostfix).sValue;
                    bank.Iban = InputObj.GetXIPFValue("Iban" + strpostfix).sValue;
                    bank.Bic = InputObj.GetXIPFValue("Bic" + strpostfix).sValue;
                    edit.Bank = bank;
                    request.EditCustomerBankRequestType = edit;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.EditCustomerEmail == requestedservice)
                {
                    EditCustomerEmailRequest request = new EditCustomerEmailRequest();
                    request.EditCustomerEmailRequestType = new EditCustomerEmailRequestType
                    {
                        BrokerReference = BrokerReference, //InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue,
                        AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue,
                        Email = InputObj.GetXIPFValue("Email" + strpostfix).sValue
                    };
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.EditCustomerPaymentDay == requestedservice)
                {
                    EditCustomerPaymentDayRequest request = new EditCustomerPaymentDayRequest();
                    request.EditCustomerPaymentDayRequestType = new EditCustomerPaymentDayRequestType
                    {
                        BrokerReference = BrokerReference,//InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue,
                        AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue,
                        PreferredPaymentDay = InputObj.GetXIPFValue("PreferredPaymentDay" + strpostfix).iValue
                    };
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.EditCustomerPhone == requestedservice)
                {
                    EditCustomerPhoneRequest request = new EditCustomerPhoneRequest();
                    request.EditCustomerPhoneRequestType = new EditCustomerPhoneRequestType
                    {
                        BrokerReference = BrokerReference,// InputObj.GetXIPFValue("BrokerReference"+ strpostfix).sValue,
                        AccountNumber = InputObj.GetXIPFValue("AccountNumber" + strpostfix).sValue,
                        //Telephone = InputObj.GetXIPFValue("Telephone"+ strpostfix).sValue
                    };
                    Telephone tel = new Telephone();
                    tel.MobileNumber = InputObj.GetXIPFValue("MobileNumber" + strpostfix).sValue;
                    tel.PhoneNumber = InputObj.GetXIPFValue("PhoneNumber" + strpostfix).sValue;
                    request.EditCustomerPhoneRequestType.Telephone = tel;
                    resutlobj = request;
                }
                else if (EnumXIBNPMethods.PartnerQuote == requestedservice)
                {
                    PartnerQuoteRequest request = new PartnerQuoteRequest();
                    request.PartnerQuoteRequestType = new PartnerQuoteRequestType
                    {
                        PolicyPremiumAmount = InputObj.GetXIPFValue("PolicyPremiumAmount" + strpostfix).rValue,
                        Deposit = InputObj.GetXIPFValue("Deposit" + strpostfix).rValue,
                        CreditProductCode = InputObj.GetXIPFValue("CreditProductCode" + strpostfix).sValue
                    };
                }
                else
                {
                    resutlobj = "Invalid Format";
                }
                return resutlobj;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }

    //    #region Enums
    //    public enum EnumxiStatus
    //    {
    //        sucess, error
    //    }

    //    public enum EnumXIBNPMethods
    //    {
    //        None = 0,
    //        LiveRateQuote = 1,
    //        NewBusiness = 2,
    //        MidTermAdjustment = 3,
    //        Cancellation = 4,
    //        PolicyStatus = 5,
    //        ServiceStatus = 6,
    //        EditCustomerAddress = 7,
    //        EditCustomerBank = 8,
    //        EditCustomerEmail = 9,
    //        EditCustomerPaymentDay = 10,
    //        EditCustomerPhone = 11,
    //        PartnerQuote = 12
    //    }
    //    #endregion

    //    #region CLASS
    //    public class XIPFAttribute
    //    {
    //        private DateTime XIPFConvertToDtTime(string InputString)
    //        {

    //            try
    //            {
    //                CResult oCResult = new CResult();
    //                CultureInfo provider = CultureInfo.InvariantCulture;
    //                string[] formats = {
    //"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
    //"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
    //"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy",

    //"dd/MM/yyyy hh:mm:ss",

    //"MM/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:m:ss tt","MM/dd/yyyy hh:mm:s tt","MM/dd/yyyy hh:m:s tt","MM/dd/yyyy h:mm:s tt",
    //"MM/dd/yyyy h:mm:ss tt","MM/dd/yyyy h:m:ss tt","MM/dd/yyyy h:m:s tt","MM/dd/yyyy hh:mm:ss tt","M/dd/yyyy hh:m:ss tt",
    //"M/dd/yyyy hh:mm:s tt","M/dd/yyyy hh:m:s tt","M/dd/yyyy h:mm:s tt","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:m:ss tt","M/dd/yyyy h:m:s tt",

    //"MM/d/yyyy hh:mm:ss tt","MM/d/yyyy hh:m:ss tt","MM/d/yyyy hh:mm:s tt","MM/d/yyyy hh:m:s tt","MM/d/yyyy h:mm:s tt","MM/d/yyyy h:mm:ss tt",
    //"MM/d/yyyy h:m:ss tt","MM/d/yyyy h:m:s tt","MM/d/yyyy hh:mm:ss tt","M/d/yyyy hh:m:ss tt","M/d/yyyy hh:mm:s tt","M/d/yyyy hh:m:s tt",
    //"M/d/yyyy h:mm:s tt","M/d/yyyy h:mm:ss tt","M/d/yyyy h:m:ss tt","M/d/yyyy h:m:s tt",


    //"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
    //"dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
    //"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt","yyyy-MM-dd h:mm:ss tt",

    //"dd/MM/yyyy HH:mm:ss",

    //"MM/dd/yyyy HH:mm:ss","MM/dd/yyyy HH:m:ss","MM/dd/yyyy HH:mm:s","MM/dd/yyyy HH:m:s","MM/dd/yyyy H:mm:s",
    //"MM/dd/yyyy H:mm:ss","MM/dd/yyyy H:m:ss","MM/dd/yyyy H:m:s","MM/dd/yyyy HH:mm:ss","M/dd/yyyy HH:m:ss",
    //"M/dd/yyyy HH:mm:s","M/dd/yyyy HH:m:s","M/dd/yyyy H:mm:s","M/dd/yyyy H:mm:ss","M/dd/yyyy H:m:ss","M/dd/yyyy H:m:s",

    //"MM/d/yyyy HH:mm:ss","MM/d/yyyy HH:m:ss","MM/d/yyyy HH:mm:s","MM/d/yyyy HH:m:s","MM/d/yyyy H:mm:s","MM/d/yyyy H:mm:ss",
    //"MM/d/yyyy H:m:ss","MM/d/yyyy H:m:s","MM/d/yyyy HH:mm:ss","M/d/yyyy HH:m:ss","M/d/yyyy HH:mm:s","M/d/yyyy HH:m:s",
    //"M/d/yyyy H:mm:s","M/d/yyyy H:mm:ss","M/d/yyyy H:m:ss","M/d/yyyy H:m:s",


    //"MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy H:mm:ss", "M/dd/yyyy H:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MMM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss","yyyy/MM/dd HH:mm:ss","yyyy/MMM/dd HH:mm:ss","yyyy.MMM.dd HH:mm:ss",
    //"dd-MM-yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "dd.MMM.yyyy HH:mm:ss",
    //"MMM-dd-yyyy HH:mm:ss","MM-dd-yyyy HH:mm:ss", "MM.dd.yyyy HH:mm:ss", "MMM.dd.yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm"
    //};
    //                DateTime dateValue;
    //                // var dt = "26.May.1975";
    //                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
    //                if (!IsValidDate)
    //                {
    //                    dateValue = DateTime.MinValue;
    //                    XIInstanceBase oIB = new XIInstanceBase();
    //                    oCResult.sMessage = "PF PolicyInception Date: " + InputString;
    //                    oCResult.LogToFile();
    //                    oIB.SaveErrortoDB(oCResult);
    //                }
    //                return dateValue; //Converting to Sql datetime format
    //            }
    //            catch (Exception ex)
    //            {
    //                throw ex;
    //            }
    //        }

    //        private string sMyValue;
    //        public string sValue
    //        {
    //            get
    //            {
    //                return string.IsNullOrEmpty(sMyValue) ? null : sMyValue;
    //            }
    //            set
    //            {
    //                sMyValue = value;
    //            }
    //        }

    //        private DateTime dMyValue;
    //        public DateTime dValue
    //        {
    //            get
    //            {
    //                return XIPFConvertToDtTime(sValue);
    //            }
    //            set
    //            {
    //                dMyValue = value;
    //            }
    //        }
    //        private int iMyValue;
    //        public int iValue
    //        {
    //            get
    //            {
    //                if (!string.IsNullOrEmpty(sValue))
    //                {
    //                    int.TryParse(sValue, out iMyValue);
    //                }
    //                return iMyValue;
    //            }
    //            set
    //            {
    //                iMyValue = value;
    //            }
    //        }

    //        private double dblMyValue;
    //        public double dblValue
    //        {
    //            get
    //            {
    //                if (!string.IsNullOrEmpty(sValue))
    //                {
    //                    double.TryParse(sValue, out dblMyValue);
    //                }
    //                return dblMyValue;
    //            }
    //            set
    //            {
    //                dblMyValue = value;
    //            }
    //        }


    //        private long lMyValue;
    //        public long lValue
    //        {
    //            get
    //            {
    //                if (!string.IsNullOrEmpty(sValue))
    //                {
    //                    long.TryParse(sValue, out lMyValue);
    //                }
    //                return lMyValue;
    //            }
    //            set
    //            {
    //                lMyValue = value;
    //            }
    //        }

    //        private bool bMyValue;
    //        public bool bValue
    //        {
    //            get
    //            {
    //                if (!string.IsNullOrEmpty(sValue))
    //                {
    //                    bool.TryParse(sValue, out bMyValue);
    //                }
    //                return bMyValue;
    //            }
    //            set
    //            {
    //                bMyValue = value;
    //            }
    //        }

    //        private decimal rMyValue;
    //        public decimal rValue
    //        {
    //            get
    //            {
    //                if (!string.IsNullOrEmpty(sValue))
    //                {
    //                    decimal.TryParse(sValue, out rMyValue);
    //                }
    //                return rMyValue;
    //            }
    //            set
    //            {
    //                rMyValue = value;
    //            }
    //        }


    //    }
    //    public class XIPFValues
    //    {
    //        private Dictionary<string, XIPFAttribute> oMyXIValues = new Dictionary<string, XIPFAttribute>();
    //        public Dictionary<string, XIPFAttribute> XIValues
    //        {
    //            get
    //            {
    //                return oMyXIValues;
    //            }
    //            set
    //            {
    //                oMyXIValues = value;
    //            }
    //        }
    //        public XIPFAttribute GetXIPFValue(string sName)
    //        {
    //            XIPFAttribute xivalue = new XIPFAttribute();
    //            var oQsInstance = this;
    //            if (oQsInstance != null)
    //            {
    //                Dictionary<string, XIPFAttribute> oXIIValues = this.XIValues;
    //                if (oXIIValues.ContainsKey(sName))
    //                {
    //                    xivalue = oXIIValues[sName];
    //                }
    //            }
    //            return xivalue;
    //        }
    //    }

    //    public class XIPFfields
    //    {
    //        public string sFormat { get; set; }
    //        public string sNotation { get; set; }
    //        public string sRegexp { get; set; }
    //        public int iMinlength { get; set; }
    //        public int iMaxlength { get; set; }
    //        public bool bMandatory { get; set; }
    //        public string sType { get; set; }
    //        public string sFiledName { get; set; }
    //        public string sErrorMessage { get; set; }
    //        public string sXIFieldName { get; set; }
    //        public string sfDisplayName { get; set; }
    //        public string sDefaultValue { get; set; }

    //    }
    //    public class XIPFCResult
    //    {
    //        public string xiStatus { get; set; }
    //        public string xiErrorMessage { get; set; }
    //        public bool bprevalidation { get; set; } //true means prevalidation passed or failed
    //        private object oMyResult;
    //        public object OResult
    //        {
    //            get
    //            {
    //                return oMyResult;
    //            }
    //            set
    //            {
    //                oMyResult = value;
    //            }
    //        }
    //        public bool IsException { get; set; }
    //    }

    //    #endregion
}
