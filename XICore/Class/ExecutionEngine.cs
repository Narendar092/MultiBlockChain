using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;
using static XIDatabase.XIDBAPI;

namespace XICore
{
    public interface iSiganlR
    {
        void HitSignalR(string InstanceID, int ProductversionID, string sRoleName, string sDatabase, string sGUID, string sSessionID,int iQuoteType);
        void ShowSignalRMsg(string sMessage);
        void ShowSignalRUserMsg(string sMessage);
        void ListRefresh(dynamic sigRObj, string sOrgID);
        void ListAddRefresh(dynamic sigRObj, string sOrgID);
    }

    public class ExecutionEngine
    {
        //Takes Query Engine object or string of Query
        //Takes Retrun Result type
        public string sSQL;
        public string sResultType;
        private XIDataSource oMyDataSource;
        private string sConnectionString;
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
        public List<SqlParameter> SqlParams { get; set; }
        public CResult Execute(TransactionInitiation TXInitiation = null)
        {
            CResult oCResult = new CResult();
            try
            {
                XIEncryption oEncrypt = new XIEncryption();
                sConnectionString = oEncrypt.DecryptData(XIDataSource.sConnectionString, true, XIDataSource.XIGUID.ToString());
                XIDBAPI Connection = new XIDBAPI(sConnectionString);
                XID1Click o1Click = new XID1Click();
                sSQL = o1Click.AddSearchParameters(sSQL, "");
                var oBOIns = (DataTable)Connection.ExecuteQuery(sSQL, SqlParams, TXInitiation);
                Dictionary<string, XIIBO> nBOIns = new Dictionary<string, XIIBO>();
                Dictionary<string, XIIAttribute> dictionary = new Dictionary<string, XIIAttribute>();
                Dictionary<string, XIIValue> XIValuedictionary = new Dictionary<string, XIIValue>();
                var j = 0;
                foreach (DataRow row in oBOIns.Rows)
                {
                    XIIBO oBOII = new XIIBO();
                    dictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                        .ToDictionary(i => oBOIns.Columns[i].ColumnName, i => new XIIAttribute { sName = oBOIns.Columns[i].ColumnName, sValue = row.ItemArray[i].ToString(), sPreviousValue = row.ItemArray[i].ToString() }, StringComparer.CurrentCultureIgnoreCase);
                    oBOII.Attributes = dictionary;
                    XIValuedictionary = Enumerable.Range(0, oBOIns.Columns.Count)
                     .ToDictionary(i => oBOIns.Columns[i].ColumnName, i => new XIIValue { sValue = row.ItemArray[i].ToString() }, StringComparer.CurrentCultureIgnoreCase);
                    oBOII.XIIValues = XIValuedictionary;
                    nBOIns[j.ToString()] = oBOII;
                    j++;
                }
                oCResult.oResult = nBOIns;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                return oCResult;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                return oCResult;
            }

        }
    }
}