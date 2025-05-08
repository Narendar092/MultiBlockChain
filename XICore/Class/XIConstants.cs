using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public static class XIUtility
    {
        public static string IDInsteadOfGUID(string ID, string sContext)
        {
            CResult oCR = new CResult();
            XIDefinitionBase oBase = new XIDefinitionBase();
            int iID = 0;
            try
            {
                int.TryParse(ID, out iID);
                if (iID > 0)
                {
                    oCR.sMessage = "ID Passed instead of GUID for " + sContext + " and ID is:" + iID;
                    oCR.sCategory = "ID Instead of GUID";
                    oBase.SaveErrortoDB(oCR);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: While Executing IDInsteadOfGUID for:" + sContext + " and ID is:" + iID + ":" + ex.ToString();
                oCR.sCategory = "ID Instead of GUID";
                oBase.SaveErrortoDB(oCR);
            }
            return string.Empty;
        }
    }
}