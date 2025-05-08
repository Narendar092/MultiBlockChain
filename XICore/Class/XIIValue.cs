using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static XIDatabase.XIDBAPI;
using XISystem;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    public class XIIValue : XIInstanceBase
    {

        public long ID { get; set; }
        public int FKiFieldDefinitionID { get; set; }
        public Guid FKiFieldDefinitionIDXIGUID { get; set; }
        public int FKiQSInstanceID { get; set; }
        public Guid FKiQSInstanceIDXIGUID { get; set; }
        public int FKiQSStepDefinitionID { get; set; }
        public Guid FKiQSStepDefinitionIDXIGUID { get; set; }
        public int FKiQSSectionDefinitionID { get; set; }
        public Guid FKiQSSectionDefinitionIDXIGUID { get; set; }
        public int FKiStepInstanceID { get; set; }
        public Guid FKiStepInstanceIDXIGUID { get; set; }
        public int FKiSectionInstanceID { get; set; }
        public Guid FKiSectionInstanceIDXIGUID { get; set; }
        public int iStage { get; set; }
        public int FKiFieldOriginID { get; set; }
        public Guid FKiFieldOriginIDXIGUID { get; set; }
        public bool bIsDisplay { get; set; }
        public bool bIsModify { get; set; }
        public int XIDeleted { get; set; }
        public int FKiOrgID { get; set; }
        public Guid XIGUID { get; set; }
        [DapperIgnore]
        public string sDisplayName { get; set; }
        [DapperIgnore]
        public bool IsOptionList { get; set; }
        [DapperIgnore]
        public List<XIDOptionList> FieldOptionList { get; set; }
        [DapperIgnore]
        public int iEnumBOD { get; set; }
        [DapperIgnore]
        public string sResolvedValue { get; set; }
        [DapperIgnore]
        public string sResolvedValueCode { get; set; }
        public string sPreviousValue { set; get; }
        [DapperIgnore]
        public string sType { get; set; }
        private string sMyValue;
        public string sValue
        {
            get
            {
                return sMyValue;
            }
            set
            {
                sMyValue = value;
            }
        }

        private string sMyDerivedValue;
        public string sDerivedValue
        {
            get
            {
                return sMyDerivedValue;
            }
            set
            {
                sMyDerivedValue = value;
            }
        }

        private DateTime dMyValue;
        public DateTime dValue
        {
            get
            {
                //if (!string.IsNullOrEmpty(sValue) && )
                //{
                //    dMyValue = ConvertToDtTime(sValue);
                //}
                return dMyValue;
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


        private long lMyValue;
        [DapperIgnore]
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
        private DateTime ConvertToDtTime(string InputString)
        {
            try
            {

                CultureInfo provider = CultureInfo.InvariantCulture;
                string[] formats = {
"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy",
"MM/dd/yyyy hh:mm:ss tt",
"MM/dd/yyyy hh:m:ss tt",
"MM/dd/yyyy hh:mm:s tt",
"MM/dd/yyyy hh:m:s tt",
"MM/dd/yyyy h:mm:s tt",
"MM/dd/yyyy h:mm:ss tt",
"MM/dd/yyyy h:m:ss tt",
"MM/dd/yyyy h:m:s tt",
"MM/dd/yyyy hh:mm:ss tt",
"M/dd/yyyy hh:m:ss tt",
"M/dd/yyyy hh:mm:s tt",
"M/dd/yyyy hh:m:s tt",
"M/dd/yyyy h:mm:s tt",
"M/dd/yyyy h:mm:ss tt",
"M/dd/yyyy h:m:ss tt",
"M/dd/yyyy h:m:s tt",


"MM/d/yyyy hh:mm:ss tt",
"MM/d/yyyy hh:m:ss tt",
"MM/d/yyyy hh:mm:s tt",
"MM/d/yyyy hh:m:s tt",
"MM/d/yyyy h:mm:s tt",
"MM/d/yyyy h:mm:ss tt",
"MM/d/yyyy h:m:ss tt",
"MM/d/yyyy h:m:s tt",
"MM/d/yyyy hh:mm:ss tt",
"M/d/yyyy hh:m:ss tt",
"M/d/yyyy hh:mm:s tt",
"M/d/yyyy hh:m:s tt",
"M/d/yyyy h:mm:s tt",
"M/d/yyyy h:mm:ss tt",
"M/d/yyyy h:m:ss tt",
"M/d/yyyy h:m:s tt",

"MM/dd/yyyy hh:mm:ss tt", "M/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy h:mm:ss tt", "M/dd/yyyy h:mm:ss tt", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MMM-dd hh:mm:ss tt", "yyyy.MM.dd hh:mm:ss tt","yyyy/MM/dd hh:mm:ss tt","yyyy/MMM/dd hh:mm:ss tt","yyyy.MMM.dd hh:mm:ss tt",
"dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "dd-MMM-yyyy hh:mm:ss tt", "dd.MMM.yyyy hh:mm:ss tt",
"MMM-dd-yyyy hh:mm:ss tt","MM-dd-yyyy hh:mm:ss tt", "MM.dd.yyyy hh:mm:ss tt", "MMM.dd.yyyy hh:mm:ss tt"
};
                DateTime dateValue;
                // var dt = "26.May.1975";
                bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
                if (!IsValidDate)
                {
                    dateValue = new DateTime();
                }
                return dateValue;
            }
            catch (Exception ex)
            {
                CResult oCResult = new CResult();
                oCResult.sMessage = "Error In ConvertToDtTime() - Can't convert string to datetime, Data String:" + sValue;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }
        public string ResolveFK(string sGroup = "")
        {
            string sResolvedValue = string.Empty;
            XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            XIDXI oXID = new XIDXI();
            int iXIFieldOriginID = this.FKiFieldOriginID;
            Dictionary<string, object> FieldOrgParams = new Dictionary<string, object>();
            FieldOrgParams["ID"] = iXIFieldOriginID;
            var QSFieldOrigin = Connection.Select<XIDFieldOrigin>("XIFieldOrigin_T", FieldOrgParams).FirstOrDefault();
            if (QSFieldOrigin != null && QSFieldOrigin.FKiBOID > 0)
            {
                XIInfraCache oCache = new XIInfraCache();
                var oBOD = new XIDBO();
                if (QSFieldOrigin.FKiBOIDXIGUID != null && QSFieldOrigin.FKiBOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, QSFieldOrigin.FKiBOIDXIGUID.ToString());
                }
                else if (QSFieldOrigin.FKiBOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, QSFieldOrigin.FKiBOID.ToString());
                }
                var GroupD = new XIDGroup();
                if (oBOD.Groups.TryGetValue(sGroup.ToLower(), out GroupD))
                {
                    XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                    if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == sGroup.ToLower()) && !string.IsNullOrEmpty(this.sValue) && this.sValue != "0")
                    {
                        var BODParams = new Dictionary<string, object>();
                        BODParams[oBOD.sPrimaryKey] = this.sValue;
                        string Result = "";
                        XIIXI oXII = new XIIXI();
                        var oBOI = oXII.BOI(oBOD.Name, this.sValue);
                        var BOFields = oBOD.GroupD(sGroup.ToLower()).BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in BOFields)
                        {
                            var AttrD = oBOD.Attributes[item];
                            if (AttrD.IsOptionList)
                            {
                                var value = oBOI.Attributes[item].sValue;
                                Result = Result + " " + AttrD.OptionList.Where(m => m.sValues == value).Select(m => m.sOptionName).FirstOrDefault();
                            }
                            else
                            {
                                Result = Result + " " + oBOI.Attributes[item].sValue;
                            }
                        }
                        sResolvedValue = Result.Trim();
                        //string FinalString = oBOD.GroupD(sGroup.ToLower()).ConcatanateGroupFields(" ");//concatenate the string with join String 
                        //if (!string.IsNullOrEmpty(FinalString))
                        //{
                        //    //var Result = Myconntection.Select<string>(oBOD.TableName, BODParams, FinalString + " As Result ").FirstOrDefault();
                        //    sResolvedValue = Result;
                        //}
                    }
                }
            }
            return sResolvedValue;
        }
        public string EngineSize()
        {
            int Size = Convert.ToInt32(sValue);
            string Result = "";
            if (Size <= 1300)
                Result = "1301cc";
            else if (Size > 1300 && Size <= 1500)
                Result = "1301cc-1500cc";
            else if (Size > 1500 && Size <= 1700)
                Result = "1501cc-1700cc";
            else if (Size > 1700 && Size <= 1900)
                Result = "1701cc-1900cc";
            else if (Size > 1900 && Size <= 2000)
                Result = "1901cc-2000cc";
            else if (Size > 2000 && Size <= 2300)
                Result = "2001cc-2300cc";
            else if (Size > 2300 && Size <= 2500)
                Result = "2301cc-2500cc";
            else if (Size > 2500 && Size <= 2800)
                Result = "2501cc-2800cc";
            else if (Size > 2800 && Size <= 3000)
                Result = "2801cc-3000cc";
            else if (Size > 3000 && Size <= 3600)
                Result = "3001cc-3600cc";
            else if (Size > 3600 && Size <= 4000)
                Result = "3601cc-4000cc";
            else if (Size > 4000 && Size <= 5000)
                Result = "4001cc-5000cc";
            else if (Size > 5000)
                Result = "5000cc+";

            return Result;
        }
        public string sVehicleUse()
        {
            var Result = "";
            if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "sdp")
                Result = "SDPec";
            else if (!string.IsNullOrEmpty(sValue) && sValue.ToLower() == "sdpc")
                Result = "SDPc";
            return Result;
        }
        public string GetValue(string sBOName = "", string sColumn = "")
        {
            string sResolvedValue = string.Empty;
            XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            XIDXI oXID = new XIDXI();
            XIInfraCache oCache = new XIInfraCache();
            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
            var GroupD = new XIDGroup();
            if (oBOD.Groups.TryGetValue(sColumn.ToLower(), out GroupD))
            {
                XIDBAPI Myconntection = new XIDBAPI(oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID));
                if (oBOD != null && oBOD.Groups.Any(group => group.Key.ToLower() == sColumn.ToLower()) && !string.IsNullOrEmpty(this.sValue) && this.sValue != "0")
                {
                    var BODParams = new Dictionary<string, object>();
                    BODParams[oBOD.sPrimaryKey] = this.sValue;
                    string Result = "";
                    XIIXI oXII = new XIIXI();
                    var oBOI = oXII.BOI(oBOD.Name, this.sValue);
                    var BOFields = oBOD.GroupD(sColumn.ToLower()).BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var item in BOFields)
                    {
                        var AttrD = oBOD.Attributes[item];
                        if (AttrD.IsOptionList)
                        {
                            var value = oBOI.Attributes[item].sValue;
                            Result = Result + " " + AttrD.OptionList.Where(m => m.sValues == value).Select(m => m.sOptionName).FirstOrDefault();
                        }
                        else
                        {
                            Result = Result + " " + oBOI.Attributes[item].sValue;
                        }
                    }
                    sResolvedValue = Result.Trim();
                }
            }
            return sResolvedValue;
        }
    }
}