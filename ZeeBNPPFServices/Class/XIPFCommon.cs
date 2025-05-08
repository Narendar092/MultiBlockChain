using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICore;
using XISystem;

namespace ZeeBNPPFServices
{
    public class XIPFCommon
    {
        #region Enums
        public enum EnumxiStatus
        {
            sucess, error
        }

        public enum EnumXIBNPMethods
        {
            None = 0,
            LiveRateQuote = 1,
            NewBusiness = 2,
            MidTermAdjustment = 3,
            Cancellation = 4,
            PolicyStatus = 5,
            ServiceStatus = 6,
            EditCustomerAddress = 7,
            EditCustomerBank = 8,
            EditCustomerEmail = 9,
            EditCustomerPaymentDay = 10,
            EditCustomerPhone = 11,
            PartnerQuote = 12
        }
        #endregion

        #region CLASS
        public class XIPFAttribute
        {
            private DateTime XIPFConvertToDtTime(string InputString)
            {

                try
                {
                    CResult oCResult = new CResult();
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy",

"dd/MM/yyyy hh:mm:ss",

"MM/dd/yyyy hh:mm:ss tt","MM/dd/yyyy hh:m:ss tt","MM/dd/yyyy hh:mm:s tt","MM/dd/yyyy hh:m:s tt","MM/dd/yyyy h:mm:s tt",
"MM/dd/yyyy h:mm:ss tt","MM/dd/yyyy h:m:ss tt","MM/dd/yyyy h:m:s tt","MM/dd/yyyy hh:mm:ss tt","M/dd/yyyy hh:m:ss tt",
"M/dd/yyyy hh:mm:s tt","M/dd/yyyy hh:m:s tt","M/dd/yyyy h:mm:s tt","M/dd/yyyy h:mm:ss tt","M/dd/yyyy h:m:ss tt","M/dd/yyyy h:m:s tt",

"MM/d/yyyy hh:mm:ss tt","MM/d/yyyy hh:m:ss tt","MM/d/yyyy hh:mm:s tt","MM/d/yyyy hh:m:s tt","MM/d/yyyy h:mm:s tt","MM/d/yyyy h:mm:ss tt",
"MM/d/yyyy h:m:ss tt","MM/d/yyyy h:m:s tt","MM/d/yyyy hh:mm:ss tt","M/d/yyyy hh:m:ss tt","M/d/yyyy hh:mm:s tt","M/d/yyyy hh:m:s tt",
"M/d/yyyy h:mm:s tt","M/d/yyyy h:mm:ss tt","M/d/yyyy h:m:ss tt","M/d/yyyy h:m:s tt",


"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
"dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt","yyyy-MM-dd h:mm:ss tt",

"dd/MM/yyyy HH:mm:ss",

"MM/dd/yyyy HH:mm:ss","MM/dd/yyyy HH:m:ss","MM/dd/yyyy HH:mm:s","MM/dd/yyyy HH:m:s","MM/dd/yyyy H:mm:s",
"MM/dd/yyyy H:mm:ss","MM/dd/yyyy H:m:ss","MM/dd/yyyy H:m:s","MM/dd/yyyy HH:mm:ss","M/dd/yyyy HH:m:ss",
"M/dd/yyyy HH:mm:s","M/dd/yyyy HH:m:s","M/dd/yyyy H:mm:s","M/dd/yyyy H:mm:ss","M/dd/yyyy H:m:ss","M/dd/yyyy H:m:s",

"MM/d/yyyy HH:mm:ss","MM/d/yyyy HH:m:ss","MM/d/yyyy HH:mm:s","MM/d/yyyy HH:m:s","MM/d/yyyy H:mm:s","MM/d/yyyy H:mm:ss",
"MM/d/yyyy H:m:ss","MM/d/yyyy H:m:s","MM/d/yyyy HH:mm:ss","M/d/yyyy HH:m:ss","M/d/yyyy HH:mm:s","M/d/yyyy HH:m:s",
"M/d/yyyy H:mm:s","M/d/yyyy H:mm:ss","M/d/yyyy H:m:ss","M/d/yyyy H:m:s",


"MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss", "MM/dd/yyyy H:mm:ss", "M/dd/yyyy H:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MMM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss","yyyy/MM/dd HH:mm:ss","yyyy/MMM/dd HH:mm:ss","yyyy.MMM.dd HH:mm:ss",
"dd-MM-yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "dd.MMM.yyyy HH:mm:ss",
"MMM-dd-yyyy HH:mm:ss","MM-dd-yyyy HH:mm:ss", "MM.dd.yyyy HH:mm:ss", "MMM.dd.yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm"
};
                    DateTime dateValue;
                    // var dt = "26.May.1975";
                    bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                    if (!IsValidDate)
                    {
                        dateValue = DateTime.MinValue;
                        XIInstanceBase oIB = new XIInstanceBase();
                        oCResult.sMessage = "PF PolicyInception Date: " + InputString;
                        oCResult.LogToFile();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    return dateValue; //Converting to Sql datetime format
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            private string sMyValue;
            public string sValue
            {
                get
                {
                    return string.IsNullOrEmpty(sMyValue) ? null : sMyValue;
                }
                set
                {
                    sMyValue = value;
                }
            }

            private DateTime dMyValue;
            public DateTime dValue
            {
                get
                {
                    return XIPFConvertToDtTime(sValue);
                }
                set
                {
                    dMyValue = value;
                }
            }
            private int iMyValue;
            public int iValue
            {
                get
                {
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        int.TryParse(sValue, out iMyValue);
                    }
                    return iMyValue;
                }
                set
                {
                    iMyValue = value;
                }
            }

            private double dblMyValue;
            public double dblValue
            {
                get
                {
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        double.TryParse(sValue, out dblMyValue);
                    }
                    return dblMyValue;
                }
                set
                {
                    dblMyValue = value;
                }
            }


            private long lMyValue;
            public long lValue
            {
                get
                {
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        long.TryParse(sValue, out lMyValue);
                    }
                    return lMyValue;
                }
                set
                {
                    lMyValue = value;
                }
            }

            private bool bMyValue;
            public bool bValue
            {
                get
                {
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        bool.TryParse(sValue, out bMyValue);
                    }
                    return bMyValue;
                }
                set
                {
                    bMyValue = value;
                }
            }

            private decimal rMyValue;
            public decimal rValue
            {
                get
                {
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        decimal.TryParse(sValue, out rMyValue);
                    }
                    return rMyValue;
                }
                set
                {
                    rMyValue = value;
                }
            }


        }
        public class XIPFValues
        {
            private Dictionary<string, XIPFAttribute> oMyXIValues = new Dictionary<string, XIPFAttribute>();
            public Dictionary<string, XIPFAttribute> XIValues
            {
                get
                {
                    return oMyXIValues;
                }
                set
                {
                    oMyXIValues = value;
                }
            }
            public XIPFAttribute GetXIPFValue(string sName)
            {
                XIPFAttribute xivalue = new XIPFAttribute();
                var oQsInstance = this;
                if (oQsInstance != null)
                {
                    Dictionary<string, XIPFAttribute> oXIIValues = this.XIValues;
                    if (oXIIValues.ContainsKey(sName))
                    {
                        xivalue = oXIIValues[sName];
                    }
                }
                return xivalue;
            }
        }

        public class XIPFfields
        {
            public string sFormat { get; set; }
            public string sNotation { get; set; }
            public string sRegexp { get; set; }
            public int iMinlength { get; set; }
            public int iMaxlength { get; set; }
            public bool bMandatory { get; set; }
            public string sType { get; set; }
            public string sFiledName { get; set; }
            public string sErrorMessage { get; set; }
            public string sXIFieldName { get; set; }
            public string sfDisplayName { get; set; }
            public string sDefaultValue { get; set; }

        }
        public class XIPFCResult
        {
            public string xiStatus { get; set; }
            public string xiErrorMessage { get; set; }
            public bool bprevalidation { get; set; } //true means prevalidation passed or failed
            private object oMyResult;
            public object OResult
            {
                get
                {
                    return oMyResult;
                }
                set
                {
                    oMyResult = value;
                }
            }
            public bool IsException { get; set; }
        }

        #endregion
    }
}
