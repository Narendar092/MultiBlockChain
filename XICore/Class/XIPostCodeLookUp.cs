using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    [Table("PostCodeLookUps_T")]
    public class XIPostCodeLookUp
    {
        [Key]
        public int ID { get; set; }
        public string sPostCode { get; set; }
        public string sMPostCode { get; set; }
        public string Group { get; set; }
        public string sProductCode { get; set; }
        public string sProductName { get; set; }
        public string sVersion { get; set; }

        public CResult Get_PostCode()
        {
            XIDefinitionBase oXID = new XIDefinitionBase();
            CResult oCResult = new CResult(); // always
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
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                string sGroup = string.Empty;
                XID1Click oD1Click = new XID1Click();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                sMPostCode = sMPostCode.Replace(" ", "").ToUpper();
                sMPostCode = sMPostCode.Remove(sMPostCode.Length - 2);
                sMPostCode = sMPostCode.Insert(sMPostCode.Length - 1, " ");
                //if (sMPostCode != "")
                //{
                //    Params["sMPostCode"] = sMPostCode;
                //}                
                cConnectionString oConString = new cConnectionString();
                //XIDBAPI Connection = new XIDBAPI(sConString);
                //sGroup = Connection.SelectString("[Group]", "PostCodeLookUps_T", Params).FirstOrDefault().ToString();
                //oCResult.oResult = sGroup;
                var sql = "SELECT [Group] FROM OverridePostCodeLookUps_T WHERE sProductName ='" + sProductName + "'  and sVersion = '" + sVersion + "' and sMPostCode='" + sMPostCode + "' and sProductCode='"+ sProductCode +"' UNION ALL SELECT[Group] FROM PostCodeLookUps_T WHERE sMPostCode = '" + sMPostCode + "' and sProductName ='" + sProductName + "' and sProductCode ='" + sProductCode + "' AND NOT EXISTS(SELECT[Group] FROM OverridePostCodeLookUps_T  WHERE sProductName = '" + sProductName + "' and sVersion = '" + sVersion + "' and sMPostCode = '" + sMPostCode + "' and sProductCode='" + sProductCode + "')";
                oD1Click.Query = sql;
                oD1Click.Name = "PostCodeLookUps_T";
                var result = oD1Click.OneClick_Run(false);
                foreach (var item in result.Values)
                {
                    oCResult.oResult = item.Attributes["Group"].sValue;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
    }
}