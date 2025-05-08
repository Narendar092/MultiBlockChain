using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using XIDataBase;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace XICore
{
    public class QueryEngine
    {
        //Takes List of BOD, Group, Where
        public List<QueryParameter> QParams = new List<QueryParameter>();
        private string sSelectFields;
        private string sWhereFields;
        private string sTableName;
        //Takes Join type like Left Join
        //Takes BO Object Model and gets BO Defintion, group, Where condtions of all BOs and Builds Query
        private XIDataSource oMyDataSource;
        public XIDataSource XIDataSource
        {
            get
            {
                return oMyDataSource;
            }
            set
            {
                oMyDataSource = value;
            }
        }

        CResult oCResult = new CResult();
        XIDBBase oDBBase = new XIDBBase();
        XIInstanceBase oXID = new XIInstanceBase();
        public CResult BuildQuery()
        {
            //BOD1
            //--BOD
            //--Group
            //--Where
            //--
            //--
            //BOD2
            //--BOD
            //--Group
            //--Where
            //--
            //--
            try
            {
                foreach (var Param in QParams)
                {
                    if (!string.IsNullOrEmpty(Param.sGroup))
                    {
                        if (Get_SelectClause(Param.BOD, Param.sGroup) != XIConstant.Success.ToString())
                        {
                            return oCResult;
                        }
                        oDBBase.sSelectFields = sSelectFields;
                    }
                    else
                    {
                        oDBBase.sSelectFields = "*";
                    }
                    if (Param.WhereParams != null && Param.WhereParams.Count() > 0)
                    {
                        if (Get_WhereClause(Param.BOD, Param.WhereParams) != XIConstant.Success.ToString())
                        {
                            return oCResult;
                        }
                        oDBBase.sWhereFields = sWhereFields;
                    }
                }
                if (CheckDataSource() != XIConstant.Success.ToString())
                {
                    return oCResult;
                }
                if (Get_FromClause() == null || Get_FromClause().Length == 0)
                {
                    return oCResult;
                }
                oDBBase.sTableName = sTableName;
                string sSQL = null;
                if (XIDataSource.sQueryType.ToLower() == "mssql" && XIDataSource.sDataSourceType.ToLower() == "database")
                {
                    XIDBMSSQL oMSSQL = new XIDBMSSQL(oDBBase);
                    sSQL = oMSSQL.Build();
                }
                oCResult.oResult = sSQL;
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Query Builded successfully." });
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //oXID.SaveErrortoDB(oCResult);
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while building the Query." });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        private string Get_FromClause()
        {
            var oParentBOD = QParams.Select(m => m.BOD).FirstOrDefault();
            if (oParentBOD != null)
            {
                sTableName = oParentBOD.TableName;
            }
            return sTableName;
        }

        private string CheckDataSource()
        {
            try
            {
                var ALLDS = QParams.Select(m => m.BOD).ToList().Select(m => m.iDataSource).ToList().Distinct().ToList();
                if (ALLDS.Count() == 0)
                {

                }
                else if (ALLDS.Count() > 1)
                {

                }
                else
                {
                    XIDataSource = QParams.Select(m => m.BOD).ToList().Select(m => m.XIDataSource).FirstOrDefault();
                }
                return XIConstant.Success.ToString();
            }
            catch (Exception ex)
            {
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
                return XIConstant.Error.ToString();
            }
        }

        private string Get_SelectClause(XIDBO oBOD, string sGroupName)
        {
            return ResolveGroup(oBOD, sGroupName);
        }

        public string AddBO(string sBOName, string sGroupName, List<XIWhereParams> WhereP)
        {
            try
            {
                QueryParameter oParam = new QueryParameter();
                XIDBO oBOD = new XIDBO();
                XIInfraCache oCache = new XIInfraCache();
                object Response = oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, "0");
                if (!string.IsNullOrEmpty(sBOName) && (sBOName.ToLower() == "XIAPPUsers".ToLower() || sBOName.ToLower() == "XIUserSetting".ToLower() || sBOName.ToLower() == "usersetting".ToLower() || sBOName.ToLower() == "Organisations".ToLower() || sBOName.ToLower() == "XIUserOrgMapping".ToLower()))
                {
                    CUserInfo oInfo = new CUserInfo();
                    XIInfraUsers oUser = new XIInfraUsers();
                    XIDXI oXID = new XIDXI();
                    oInfo = oUser.Get_UserInfo();
                    if (oInfo.sCoreDataBase != null)
                    {
                        var DataSource = oXID.Get_DataSourceDefinition(oInfo.sCoreDataBase);
                        var BODS = ((XIDataSource)DataSource.oResult);
                        var iDataSource = BODS.XIGUID;
                        if (Response != null)
                        {
                            oBOD = (XIDBO)Response;
                            oBOD.XIDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, iDataSource.ToString());
                            oParam.BOD = oBOD;
                            oParam.sGroup = sGroupName;
                            oParam.WhereParams = WhereP;
                        }
                        QParams.Add(oParam);
                    }
                }
                else
                {
                    if (Response != null)
                    {
                        oBOD = (XIDBO)Response;
                        oBOD.XIDataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBOD.iDataSourceXIGUID.ToString());
                        oParam.BOD = oBOD;
                        oParam.sGroup = sGroupName;
                        oParam.WhereParams = WhereP;
                    }
                    QParams.Add(oParam);
                }

                return XIConstant.Success.ToString();
            }
            catch (Exception ex)
            {
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
                return XIConstant.Error.ToString();
            }
        }

        private string ResolveGroup(XIDBO oBOD, string sGroupName)
        {
            try
            {
                Dictionary<string, string> outFields = new Dictionary<string, string>();
                var nSubFields = sGroupName.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (nSubFields.Count() > 0)
                {
                    foreach (var sField in nSubFields)
                    {
                        if (oBOD.Groups.ContainsKey(sField.ToLower()))
                        {
                            var oGroupD = oBOD.Groups[sField.ToLower()];
                            if (!string.IsNullOrEmpty(oGroupD.BOFieldNames))
                            {
                                var nGFields = oGroupD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                foreach (var sGField in nGFields)
                                {
                                    if (oBOD.Attributes.ContainsKey(sGField) && oBOD.Attributes[sGField.ToLower()].sFKBOName != null)
                                    {
                                        //TO DO
                                        //Resolve FK Field with its label group as subQuery or Join Query                                        
                                        outFields[sGField] = sGField;
                                    }
                                    else
                                    {
                                        outFields[sGField] = sGField;
                                    }
                                }
                            }
                        }
                        else if (!outFields.ContainsKey(sField))
                        {
                            outFields[sField] = sField;
                        }
                    }
                }
                var sFields = string.Join(",", outFields.Values.ToList());
                sSelectFields = sFields;
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Query Select Fields", sValue = "Success: Select Fields Resolved" });
                }
                return XIConstant.Success;
            }
            catch (Exception ex)
            {
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
                return XIConstant.Error.ToString();
            }
        }

        private string Get_WhereClause(XIDBO BOD, List<XIWhereParams> whereParams)
        {
            try
            {
                string[] oConditions = new string[whereParams.Count];
                string sWhereCondition = string.Empty;
                int i = 0;
                string sIDRef = string.Empty;
                XIInfraUsers oUser = new XIInfraUsers();
                XIInfraCache oCache = new XIInfraCache();
                CUserInfo oInfo = new CUserInfo();
                oInfo = oUser.Get_UserInfo();
                if (oInfo != null && oInfo.iUserID > 0)
                {
                    var iOrgId = oInfo.iOrganizationID;
                    var iAppID = oInfo.iApplicationID;
                    var iRoleID = oInfo.iRoleID;
                    if (oInfo.iRoleID != 55)
                    {
                        sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + iRoleID + "_" + XIConstant.IDRef_internal);
                    }
                    else
                    {
                        sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, iAppID + "_" + iOrgId + "_" + XIConstant.IDRef_public);
                    }
                }
                foreach (var cond in whereParams)
                {
                    if (!string.IsNullOrEmpty(cond.sField) && !string.IsNullOrEmpty(cond.sOperator) && !string.IsNullOrEmpty(cond.sValue))
                    {

                        if (cond.sField.ToLower() == "pk")
                        {
                            string sPrimaryKey = string.Empty;
                            //if (BOD.bUID)
                            //{
                            //    sPrimaryKey = "xiguid";
                            //}
                            //else
                            //{
                            //    sPrimaryKey = BOD.sPrimaryKey;
                            //}
                            Guid OutputGUID;
                            bool ISGUID = Guid.TryParse(cond.sValue, out OutputGUID);
                            if (BOD.bUID || (!string.IsNullOrEmpty(sIDRef) && sIDRef.ToLower() == "xiguid"))
                            {
                                if (ISGUID)
                                {
                                    sPrimaryKey = "XIGUID";
                                }
                                else
                                {
                                    sPrimaryKey = BOD.sPrimaryKey;
                                }
                            }
                            else
                            {
                                sPrimaryKey = BOD.sPrimaryKey;
                            }                            
                            var condtion = sPrimaryKey + cond.sOperator + (cond.sOperator == " in" ? cond.sValue : "'" + cond.sValue + "'");
                            oConditions[i] = condtion;
                        }
                        else
                        {
                            var Fields = cond.sValue.Split(',');
                            string[] ParamsName = new string[Fields.Count()];
                            for (int j = 0; j < Fields.Count(); j++)
                            {
                                ParamsName[j] = "@" + cond.sField + j.ToString();
                            }
                            string inClause = string.Join(",", ParamsName);
                            //string whereInClause = string.Format(partialClause.Trim(), inClause);
                            var condtion = cond.sField + cond.sOperator + (cond.sOperator == " in" ? "(" + inClause + ")" : "@" + cond.sField);
                            //var condtion = cond.sField + cond.sOperator + (cond.sOperator == " in" ? cond.sValue : "'" + cond.sValue + "'");
                            //var condtion = cond.sField + cond.sOperator + "@"+cond.sField;
                            oConditions[i] = condtion;
                        }
                        i++;
                    }
                }
                if (oConditions.Count() > 0)
                {
                    sWhereCondition = string.Join(" and ", oConditions.ToList());
                }
                sWhereFields = sWhereCondition;
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Query Where Fields", sValue = "Success: Where Fields Added" });
                }
                return XIConstant.Success;
            }
            catch (Exception ex)
            {
                if (oCResult.iTraceLevel > 0)
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
                return XIConstant.Error.ToString();
            }
        }

        public CResult Execute_QueryEngine(string BOName, string sColumns, string SWhereConditions = "")
        {
            CResult oCresult = new CResult();
            try
            {
                if (string.IsNullOrEmpty(sColumns))
                    sColumns = "*";
                Dictionary<string, XIIBO> oXIBOI = new Dictionary<string, XIIBO>();
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = null;
                SqlParameter osqlparam = new SqlParameter();
                List<SqlParameter> SqlParams = new List<SqlParameter>();
                if (!string.IsNullOrEmpty(SWhereConditions))
                {
                    oWParams = new List<XIWhereParams>();
                    SWhereConditions = SWhereConditions.Trim();
                    List<string> operators = new List<string>() { "<>", "=", "!=", "<", "<=", ">=", ">", "between", " in", "(", ")" };
                    List<string[]> lstItems = SWhereConditions.Split(',').Select(f => new { Field = f }).Select(c => c.Field.Trim().Split(operators.ToArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();
                    for (int i = 0; i < lstItems.Count; i++)
                    {
                        osqlparam = new SqlParameter();
                        Regex pattern = new Regex(Regex.Escape(lstItems[i][0]) + "(.+?)" + Regex.Escape(lstItems[i][1].Replace("|||", ",").Replace("|||", ",")));
                        var getoperators = pattern.Matches(SWhereConditions).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().ToList();
                        if (getoperators.Count() > 1)
                        {
                            oWParams.Add(new XIWhereParams { sField = lstItems[i][0], sValue = lstItems[i][1], sOperator = getoperators[i] });
                            osqlparam.Value = lstItems[i][1];
                            osqlparam.ParameterName = "@" + lstItems[i][0];
                            SqlParams.Add(osqlparam);
                        }
                        else
                        {
                            oWParams.Add(new XIWhereParams { sField = lstItems[i][0], sValue = lstItems[i][1] == " " ? "(" + lstItems[i][2].Replace("|||", ",").Replace("|||", ",") + ")" : lstItems[i][1], sOperator = getoperators[0] });
                            var Value = lstItems[i][1] == " " ? lstItems[i][2].Replace("|||", ",").Replace("|||", ",") : lstItems[i][1];
                            var InValues = Value.Split(',');
                            if (InValues.Count() == 1 && lstItems[i].Count() <= 2)
                            {
                                osqlparam.ParameterName = "@" + lstItems[i][0];
                                osqlparam.Value = lstItems[i][1];
                                SqlParams.Add(osqlparam);
                            }
                            else
                            {
                                for (int j = 0; j < InValues.Count(); j++)
                                {
                                    osqlparam = new SqlParameter();
                                    osqlparam.ParameterName = "@" + lstItems[i][0] + j.ToString();
                                    osqlparam.Value = InValues[j];
                                    SqlParams.Add(osqlparam);
                                }
                            }
                        }

                    }
                }

                oQE.AddBO(BOName, sColumns, oWParams);
                oCresult = oQE.BuildQuery();
                if (oCresult.bOK && oCresult.oResult != null)
                {
                    var sSql = (string)oCresult.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    oEE.SqlParams = SqlParams;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                        oCresult.oResult = oXIBOI;
                    }
                }
            }
            catch (Exception ex)
            {
                oCresult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCresult.sMessage = ex.Message;
            }
            return oCresult;
        }
    }

    public class QueryParameter
    {
        public XIDBO BOD { get; set; }
        public long BOID { get; set; }
        public string sGroup { get; set; }
        public bool bIsResolveFK { get; set; }
        public List<XIWhereParams> WhereParams { get; set; }
    }

    public class XIWhereParams
    {
        public string sField { get; set; }
        public string sOperator { get; set; }
        public string sValue { get; set; }
    }
}