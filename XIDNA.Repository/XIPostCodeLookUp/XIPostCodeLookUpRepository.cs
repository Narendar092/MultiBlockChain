using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XICore;
using XIDatabase;
using XISystem;

namespace XIDNA.Repository.XIPostCodeLookUp
{
        [Table("PostCodeLookUps_T")]
        public abstract class XIPostCodeLookUpAbstract
        {
            [Key]
            public int ID { get; set; }
            public string sPostCode { get; set; }
            public string sMPostCode { get; set; }
            public string Group { get; set; }
            public string sProductName { get; set; }
            public string sProductCode { get; set; }
            public string sVersion { get; set; }
            public virtual CResult Get_PostCode(string sCoreDatabase)
            {
                CResult oCResult = new CResult(); // always
                CResult oCR = new CResult(); // always
                long iTraceLevel = 10;

                //get iTraceLevel from ??somewhere fast - cache against user??

                oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
                oCResult.sFunctionName = "METHODNAME";

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

                if (iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
                }
               
                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
               
                oCResult.sMessage = "someone tried to do something they shouldnt";

              
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                try
                {
                    XID1Click oD1Click = new XID1Click();
                    string sGroup = string.Empty;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    sMPostCode = sMPostCode.Replace(" ", "").ToUpper();
                    sMPostCode = sMPostCode.Remove(sMPostCode.Length - 2);
                    sMPostCode = sMPostCode.Insert(sMPostCode.Length - 1, " ");
                   
                    cConnectionString oConString = new cConnectionString();
                    string sConString = oConString.ConnectionString(sCoreDatabase);
                    var sql = "SELECT [Group] FROM OverridePostCodeLookUps_T WHERE sProductName ='" + sProductName + "'  and sVersion = '" + sVersion + "' and sMPostCode='" + sMPostCode + "' UNION ALL SELECT[Group] FROM PostCodeLookUps_T WHERE sMPostCode = '" + sMPostCode + "' AND NOT EXISTS(SELECT[Group] FROM OverridePostCodeLookUps_T  WHERE sProductName = '" + sProductName + "' and sVersion = '" + sVersion + "' and sMPostCode = '" + sMPostCode + "')";
                    oD1Click.Query = sql;
                    oD1Click.sConnectionString = sConString;
                    var result = oD1Click.OneClick_Execute();

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
                }
                return oCResult; // always
            }






            public virtual CResult Get_PartialPostCode(string sCoreDatabase)
            {
                CResult oCResult = new CResult(); // always
                CResult oCR = new CResult(); // always
                long iTraceLevel = 10;

                //get iTraceLevel from ??somewhere fast - cache against user??

                oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
                oCResult.sFunctionName = "METHODNAME";

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

                if (iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
                }

                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
                oCResult.sMessage = "someone tried to do something they shouldnt";

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                try
                {
                    XID1Click oD1Click = new XID1Click();
                    string sGroup = string.Empty;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    sMPostCode = sMPostCode.Replace(" ", "").ToUpper();
                    sMPostCode = sMPostCode.Remove(sMPostCode.Length - 2);
                    sMPostCode = sMPostCode.Insert(sMPostCode.Length - 1, " ");
                    cConnectionString oConString = new cConnectionString();
                    string sConString = oConString.ConnectionString(sCoreDatabase);
                    var sql = "";
                    sql = "SELECT [Group] FROM PartialPostCodeLookUps_T WHERE sProductName ='" + sProductName + "'  and sVersion = '" + sVersion + "' and sMPostCode='" + sMPostCode + "'";
                    oD1Click.Query = sql;
                    oD1Click.sConnectionString = sConString;
                    var result = new Dictionary<string, XIIBO>();
                    result = oD1Click.OneClick_Execute();
                    if (result.Count() == 0)
                    {
                        while (sMPostCode.Length > 1)
                        {
                            while (result.Count() < 1)
                            {
                                sMPostCode = sMPostCode.TrimEnd();
                                sMPostCode = sMPostCode.Remove(sMPostCode.Length - 1).TrimEnd();
                                //sMPostCode = sMPostCode.Replace(" ", "").ToUpper();
                                sql = "";
                                sql = "SELECT [Group] FROM PartialPostCodeLookUps_T WHERE sProductName ='" + sProductName + "'  and sVersion = '" + sVersion + "' and sMPostCode='" + sMPostCode + "'";
                                oD1Click.Query = sql;
                                oD1Click.sConnectionString = sConString;
                                result = oD1Click.OneClick_Execute();
                            }
                        }


                    }
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
                }
                return oCResult; // always
            }

        }
 
}
