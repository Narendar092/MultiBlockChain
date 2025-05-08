using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xiEnumSystem;
using XISystem;

namespace XICore
{
    public class XIDFormat
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int iType { get; set; }
        public int iStatus { get; set; }
        public string sFormat { get; set; }
        public int iScript { get; set; }
        
        public string XIFormat(List<CNV> oParams)
        {
            var sFormattedVal =oParams.Where(n=>n.sName.ToLower()== "sNonFormatedVal".ToLower()).Select(k=>k.sValue).FirstOrDefault();
            switch (sCode.ToLower())
            {
                case "html wrapper":
                    return sFormattedVal = sFormat.Replace("{xi|currentvalue}", sFormattedVal);
                case "time ago":
                    sFormattedVal = XIFormat_Date(oParams);
                    return sFormattedVal;
                case "age":
                    sFormattedVal = XIFormat_Age(oParams);
                    return sFormattedVal;
                default:
                    return null;
            }
        }
        public string XIFormat_Date1(string sInupt)
        {
            if (iType == 150)
            {
                XIIBO oBOI = new XIIBO();
                var dtDate = oBOI.ConvertToDtTime(sInupt);
                //var dtDate = Convert.ToDateTime(sInupt);
                if (!string.IsNullOrEmpty(sCode) && sCode.ToLower() == "Date".ToLower())
                {
                    return dtDate.ToString(sFormat);
                }
                else if (!string.IsNullOrEmpty(sCode) && sCode.ToLower() == "time ago".ToLower())
                {
                    DateTime Now = DateTime.Now;
                    int Years = new DateTime(DateTime.Now.Subtract(dtDate).Ticks).Year - 1;
                    DateTime PastYearDate = dtDate.AddYears(Years);
                    int Months = 0;
                    for (int i = 1; i <= 12; i++)
                    {
                        if (PastYearDate.AddMonths(i) == Now)
                        {
                            Months = i;
                            break;
                        }
                        else if (PastYearDate.AddMonths(i) >= Now)
                        {
                            Months = i - 1;
                            break;
                        }
                    }
                    int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
                    int Hours = Now.Subtract(PastYearDate).Hours;
                    int Minutes = Now.Subtract(PastYearDate).Minutes;
                    int Seconds = Now.Subtract(PastYearDate).Seconds;
                    return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",
                                        Years, Months, Days, Hours, Seconds);
                }
                else
                {
                    return dtDate.ToString();
                }
            }
            else
            {
                return sInupt;
            }
        }

        public string XIFormat_Date(List<CNV> oParams)
        {
            string sInupt = "";
            //assumes date2 is the bigger date for simplicity
            //----------------------------------------------
            //years
            XIIBO oBOI = new XIIBO();
            var date1 = oBOI.ConvertToDtTime(sInupt);
            var date2 = DateTime.Now;
            TimeSpan diff = date2 - date1;
            int Years = 0;
            int Months = 0;
            int Weeks = 0;
            int Days = 0;
            Years = diff.Days / 366;
            DateTime workingDate = date1.AddYears(Years);
            while (workingDate.AddYears(1) <= date2)
            {
                workingDate = workingDate.AddYears(1);
                Years++;
            }
            //---------------------------------------------
            //months
            diff = date2 - workingDate;
            Months = diff.Days / 31;
            workingDate = workingDate.AddMonths(Months);
            while (workingDate.AddMonths(1) <= date2)
            {
                workingDate = workingDate.AddMonths(1);
                Months++;
            }
            //---------------------------------------------
            //weeks and days
            diff = date2 - workingDate;
            Weeks = diff.Days / 7; //weeks always have 7 days
            Days = diff.Days % 7;
            //---------------------------------------------
            return Years + " Years " + Months + " months " + diff.Days + " Days ";
        }
        public string XIFormat_Age(List<CNV> oParams)
        {
            XIDefinitionBase oXID = new XIDefinitionBase();
            CResult oCResult=new CResult();
            int age = 0;
            List<string> Info = new List<string>();
            try
            {
                string fromDateStr = oParams.FirstOrDefault(o => o.sName.ToLower() == "fromdate")?.sValue;
                Info.Add("before convertion from Date:" + fromDateStr);
                string ToDateStr = oParams.FirstOrDefault(o => o.sName.ToLower() == "todate")?.sValue;
                Info.Add("before convertion TO Date:" + ToDateStr);
                Info.Add("TO Date:" + ToDateStr);
                if (string.IsNullOrWhiteSpace(fromDateStr) || string.IsNullOrWhiteSpace(ToDateStr))
                    throw new ArgumentException("Required date parameters are missing.");

                XIIAttribute oAttrI = new XIIAttribute();
                DateTime fromDate = oAttrI.ConvertToDtTime(fromDateStr);
                fromDate= fromDate.Date;
                Info.Add("after convertion From Date:" + fromDate);
                DateTime qsToDate = oAttrI.ConvertToDtTime(ToDateStr);
                qsToDate = qsToDate.Date;
                Info.Add("after convertion TO Date:" + qsToDate);

                age = fromDate.Year - qsToDate.Year;
                Info.Add("fromDate.Year"+ fromDate.Year + " - qsToDate.Year "+ qsToDate.Year+" age" + age);
                //// Adjust age if the birthday has not yet occurred in the reference year
                if (qsToDate.Month > fromDate.Month || (qsToDate.Month == fromDate.Month && qsToDate.Day > fromDate.Day))
                {
                    age--;
                    Info.Add("based on dates calculation age " + age);
                }
                string sInfo = "INFO: " + string.Join(",\r\n ", Info);
                oCResult.sMessage= sInfo;
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                oXID.SaveErrortoDB(oCResult);
            }
            catch (Exception ex) {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.iLogLevel = (int)EnumXIErrorPriority.Critical;
                oXID.SaveErrortoDB(oCResult);
            }
            return age.ToString();
        }

    }

    public class XIDFormatMapping
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int iType { get; set; }
        public int iStatus { get; set; }
        public int FKiBODID { get; set; }
        public Guid FKiBODIDXIGUID { get; set; }
        public int FKiAttributeID { get; set; }
        public Guid FKiAttributeIDXIGUID { get; set; }
        public int FKiFieldOriginID { get; set; }
        public Guid FKiFieldOriginIDXIGUID { get; set; }
        public int FKi1ClickID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        public int FKiFormatID { get; set; }
        public Guid FKiFormatIDXIGUID { get; set; }
    }
}
