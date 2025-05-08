using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using XIDatabase;
using System.Data;

namespace XICore
{
    public class DBHelper
    {
        public int iDataSourceID { get; set; }
        public int iBOID { get; set; }
        public string sWhereParams { get; set; }
        public string sQuery { get; set; }
        public string sSelectParams { get; set; }

        List<string> IgnoreKeys = new List<string>() { "drop", "delete", "update", "alter", "set" };


        public CResult Run_Query()
        {
            CResult oCR = new CResult();
            bool bIsSelectSafe = false;
            bool bIsWhereSafe = false;
            bool bIsQuerySafe = false;
            string sTableName = string.Empty;
            string sFinalQuery = string.Empty;
            if (iBOID > 0 && sSelectParams.Length > 0)
            {
                if (string.IsNullOrEmpty(sQuery)) bIsQuerySafe = true;
                else if (IgnoreKeys.Any(s => sQuery.Contains(s)))
                {
                    bIsQuerySafe = false;
                }
                if (IgnoreKeys.Any(s => sSelectParams.Contains(s)))
                {
                    bIsSelectSafe = false;
                }
                else bIsSelectSafe = true;

                if (!string.IsNullOrEmpty(sWhereParams) && IgnoreKeys.Any(s => sWhereParams.Contains(s)))
                {
                    bIsWhereSafe = false;
                }
                else bIsWhereSafe = true;

                if (bIsQuerySafe && bIsSelectSafe && bIsWhereSafe)
                {
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO oBOD = new XIDBO();
                    XIDXI oXID = new XIDXI();
                    if (iBOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBOID.ToString());
                        sTableName = oBOD.TableName;
                    }
                    var XIDataSource = oXID.GetBODataSource(oBOD.iDataSource);
                    XIDBAPI Connection = new XIDBAPI(XIDataSource);
                    sFinalQuery = sFinalQuery + "Select " + sSelectParams + " from " + sTableName;
                    if (!string.IsNullOrEmpty(sWhereParams))
                    {
                        sFinalQuery = sFinalQuery + " Where " + sWhereParams;
                    }
                    var oBOIns = (DataTable)Connection.ExecuteQuery(sFinalQuery);
                }

            }

            return oCR;
        }

    }
}