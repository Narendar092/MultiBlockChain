using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XISystem;
using XIDatabase;
using System.Configuration;

namespace XICore
{
    [Table("XILayoutDetail_T")]
    public class XIDLayoutDetails
    {
        [Key]
        public int PlaceHolderID { get; set; }
        public Guid PlaceHolderIDXIGUID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int LayoutID { get; set; }
        public string PlaceholderName { get; set; }
        public string PlaceholderArea { get; set; }
        public string PlaceholderUniqueName { get; set; }
        public string PlaceholderClass { get; set; }
        public Guid LayoutIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
        public string sCssClass { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        public CResult Get_LayoutDetailsByName()
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

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?

            try
            {
                XIDLayoutDetails oXILayoutDet = new XIDLayoutDetails();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(PlaceholderName))
                {
                    Params["PlaceholderName"] = PlaceholderName;
                    if (LayoutID > 0)
                    {
                        Params["LayoutID"] = LayoutID;
                    }
                    oXILayoutDet = Connection.Select<XIDLayoutDetails>("XILayoutDetail_T", Params).FirstOrDefault();
                }
                oCResult.oResult = oXILayoutDet;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

    }
}