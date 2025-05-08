using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    public class XIQSLinkDefintion : XIDefinitionBase
    {
        public long ID { get; set; }
        public Guid XIGUID { get; set; }
        public string sName { get; set; }
        public string sCode { get; set; }
        public int FKiXILInkID { get; set; }
        public Guid FKiXILInkIDXIGUID { get; set; }
        public string sType { get; set; }
        public decimal rOrder { get; set; }
        public string sRunType { get; set; }
        public int FKIXIScriptID { get; set; }
        public Guid FKiXIScriptIDXIGUID { get; set; }
        public List<XIQSLinkDefintion> NVs { get; set; }
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields
        public string XiLinkName { get; set; }
        public int FKiApplicationID { get; set; }
        public int iOrgID { get; set; }
        private Dictionary<string, XILink> oMyXILink = new Dictionary<string, XILink>();

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public Dictionary<string, XILink> XiLink
        {
            get
            {
                return oMyXILink;
            }
            set
            {
                oMyXILink = value;
            }
        }

        #region QSLinkDefination

        public CResult Get_QSLinkDefinition()
        {
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

            try
            {
                XIQSLinkDefintion oLDef = new XIQSLinkDefintion();

                //AutoComplete For Xilinks
                //Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                Dictionary<string, object> oLinks = new Dictionary<string, object>();
                //oLinks["FKiApplicationID"] = FKiAppID;
                //oLinks["OrganisationID"] = iOrgID;
                if (ID > 0)
                {
                    Dictionary<string, object> oQSL = new Dictionary<string, object>();
                    oQSL["ID"] = ID;
                    oLDef = Connection.Select<XIQSLinkDefintion>("XIQSLinkDefinition_T", oQSL).FirstOrDefault();
                    var oLDefs = Connection.Select<XIQSLinkDefintion>("XIQSLinkDefinition_T", oQSL).ToList();
                    var sCode = oLDefs.FirstOrDefault().sCode;
                    oQSL = new Dictionary<string, object>();
                    oQSL["sCode"] = sCode;
                    oQSL[XIConstant.Key_XIDeleted] = "0";
                    oLDefs = Connection.Select<XIQSLinkDefintion>("XIQSLinkDefinition_T", oQSL).ToList();
                    oLDef.NVs = oLDefs.ToList().Select(m => new XIQSLinkDefintion
                    {
                        ID = m.ID,
                        sName = m.sName,
                        sCode = sCode,
                        rOrder = m.rOrder,
                        FKiXILInkID = m.FKiXILInkID,
                        FKiXILInkIDXIGUID = m.FKiXILInkIDXIGUID,
                        sType = m.sType,
                        StatusTypeID = m.StatusTypeID,
                        sRunType = m.sRunType,
                        FKIXIScriptID = m.FKIXIScriptID
                    }).ToList();
                    foreach (var items in oLDef.NVs)
                    {
                        if (items.FKiXILInkIDXIGUID != null && items.FKiXILInkIDXIGUID != Guid.Empty)
                        {
                            oLinks["XIGUID"] = items.FKiXILInkIDXIGUID;
                        }
                        else if (items.FKiXILInkID > 0)
                        {
                            oLinks["XiLinkID"] = items.FKiXILInkID;
                        }
                        var oXiLinkDe = Connection.Select<XILink>("XILink_T", oLinks).FirstOrDefault();
                        if (oXiLinkDe != null)
                        {
                            items.XiLinkName = oXiLinkDe.Name;
                        }
                    }
                }
                //var oXiLinkDef = Connection.Select<XILink>("XILink_T", oLinks).ToList();
                //foreach (var items in oXiLinkDef)
                //{
                //    XiLinks[items.Name] = items.Name;
                //}
                //oLDef.XILinks = XiLinks;
                oCResult.oResult = oLDef;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while loading Layout definition" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        #endregion QSLinkDefination

    }
}