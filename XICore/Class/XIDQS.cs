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
using XISystem;
using System.Web;
using XIDatabase;
using System.Configuration;
using System.Data.SqlClient;

namespace XICore
{
    public class XIDQS : XIDefinitionBase
    {
        public int ID { get; set; }
        public int FKiOrgID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public int iVisualisationID { get; set; }
        public Guid iVisualisationIDXIGUID { get; set; }
        public string SaveType { get; set; }
        public bool bIsTemplate { get; set; }
        public string sHTMLPage { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int iLayoutID { get; set; }
        public bool bInMemoryOnly { get; set; }
        public string sStructureCode { get; set; }
        public string sMode { get; set; }
        public int FKiParameterID { get; set; }
        public Guid FKiParameterIDXIGUID { get; set; }
        public bool bIsContextObject { get; set; }
        public int FKiBOStructureID { get; set; }
        public Guid FKiBOStructureIDXIGUID { get; set; }
        public int FKiOriginID { get; set; }
        public int FKiClassID { get; set; }
        public int FKiSourceID { get; set; }
        public bool bIsStage { get; set; }
        public int iMaxStage { get; set; }
        public string sCssClass { get; set; }
        public List<XIDropDown> ddlXIParameters { get; set; }
        public List<XIDropDown> ddlXIVisualisations { get; set; }
        public List<XIDropDown> ddlXIStructures { get; set; }
        public List<XIDropDown> ddlSourceList { get; set; }
        public List<XIDropDown> ddlApplications { get; set; }
        public List<string> DependentFields { get; set; }
        public Guid XIGUID { get; set; }

        public bool bNavigationBar { get;set; }
        public int RelatedObjectID { get; set; }
        public Guid RelatedObjectIDXIGUID { get; set; }
        public string sRelatedStatusAttr { get; set; }
        public Guid FKiDefaultStepDIDXIGUID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public int BOID { get; set; }
        public bool bStepsAlwaysActive { get; set; }
        public List<Guid> OrgFieldOrigins = new List<Guid>();

        private Dictionary<string, XIDQSStep> oMySteps = new Dictionary<string, XIDQSStep>();

        public Dictionary<string, XIDQSStep> Steps
        {
            get
            {
                return oMySteps;
            }
            set
            {
                oMySteps = value;
            }
        }
        private Dictionary<string, XIDPartialQS> oMyPartialQS = new Dictionary<string, XIDPartialQS>();

        public Dictionary<string, XIDPartialQS> PartialQS
        {
            get
            {
                return oMyPartialQS;
            }
            set
            {
                oMyPartialQS = value;
            }
        }

        private XIVisualisation oMyVisualisation;
        public XIVisualisation Visualisation
        {
            get
            {
                return oMyVisualisation;
            }
            set
            {
                oMyVisualisation = value;
            }
        }

        private List<XIQSVisualisation> oMyQSVisualisations = new List<XIQSVisualisation>();

        public List<XIQSVisualisation> QSVisualisations
        {
            get
            {
                return oMyQSVisualisations;
            }
            set
            {
                oMyQSVisualisations = value;
            }
        }
        private Dictionary<string, XIDFieldOrigin> oXIDFieldOrigin = new Dictionary<string, XIDFieldOrigin>();

        public Dictionary<string, XIDFieldOrigin> XIDFieldOrigin
        {
            get
            {
                return oXIDFieldOrigin;
            }
            set
            {
                oXIDFieldOrigin = value;
            }
        }

        public XIDQSStep StepD(string sStepName)
        {
            XIDQSStep oThisStepD = null/* TODO Change to default(_) if this is not a reference type */;

            // The steps of this QS must be loaded

            //sStepName = sStepName.ToLower();

            if (oMySteps.ContainsKey(sStepName) == false)
            {
            }

            if (oMySteps.ContainsKey(sStepName))
            {
                oThisStepD = oMySteps[sStepName];
            }            
            else
            {
                oThisStepD = oMySteps.Values.ToList().Where(m => m.sDisplayName != null && m.sDisplayName.ToLower() == sStepName.ToLower()).FirstOrDefault();
            }

            return oThisStepD;
        }
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public List<int> QSEvent(List<CNV> QSInfo, string QSEvents)
        {
            List<int> nXILinks = new List<int>();
            XIDQS oQSD = new XIDQS();
            XIDQSSection oQSSectionD = new XIDQSSection();
            var QSID = QSInfo.Where(m => m.sName.ToLower() == "iqsdid").Select(m => m.sValue).FirstOrDefault();
            if (QSID != null)
            {
                var sSessionID = HttpContext.Current.Session.SessionID;
                ID = Convert.ToInt32(QSID);
                XIInfraCache oCache = new XIInfraCache();
                oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, ID.ToString());
                if (oQSD != null)
                {
                    var SectionID = QSInfo.Where(m => m.sName.ToLower() == "iSectionDID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (SectionID != null)
                    {
                        var Steps = oQSD.Steps;
                        foreach (var items in Steps.Values)
                        {
                            XIDQSSection oSecD = new XIDQSSection();
                            if (items.Sections.TryGetValue(SectionID, out oSecD))
                            {
                                oQSSectionD = oSecD;
                            }
                            //oQSSectionD = items.Sections[SectionID];
                        }
                    }
                }
            }
            if (oQSSectionD != null && oQSSectionD.ID > 0)
            {
                List<string> nEvents = new List<string>();
                if (!string.IsNullOrEmpty(QSEvents))
                {
                    nEvents = QSEvents.Split(',').ToList();
                    foreach (var eve in nEvents)
                    {
                        if (eve.IndexOf('|') > 0)
                        {
                            var XIIdentifier = eve.Split('|')[0];
                            if (!string.IsNullOrEmpty(XIIdentifier) && XIIdentifier.ToLower() == "xiqsl")
                            {
                                var QSL = XIIdentifier = eve.Split('|')[1];
                                if (!string.IsNullOrEmpty(QSL))
                                {
                                    XIQSLink oQSLink = new XIQSLink();
                                    if (oQSSectionD.QSLinks.TryGetValue(QSL, out oQSLink))
                                    {
                                        nXILinks.Add(oQSLink.FKiXILinkID);
                                    }
                                    //var oQSL = oQSSectionD.QSLinks[QSL];

                                }
                            }
                        }
                    }
                }
            }
            return nXILinks;
        }

        #region QuestionSetComponent       


        public XIDQS GetQuestionSetComponent(int iBODID, string sCode, string sMode, string sQSName, int iUserID, string OrgName, string sDatabase)
        {
            XIDQS oQS = new XIDQS();
            XIDQS FixedTemplate = new XIDQS();
            FixedTemplate.Steps = new Dictionary<string, XIDQSStep>();
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params["sName"] = sQSName;
            oQS = Connection.Select<XIDQS>("XIQSDefinition_T", Params).FirstOrDefault();
            //oQS = dbContext.QSDefinition.Where(m => m.sName.ToLower() == sQSName.ToLower()).FirstOrDefault();
            var NewSteps = new Dictionary<string, XIDQSStep>();
            List<CNV> oTreeParams = new List<CNV>();
            XIDStructure oStrct = new XIDStructure();
            var oTree = oStrct.GetXIStructureTreeDetails(iBODID.ToString(), sCode);
            if (oQS != null)
            {
                int i = 0;
                List<int> StepIDs = new List<int>();
                foreach (var items in oTree)
                {
                    if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Single" || items.sMode == "SingleView"))
                    {
                        var iStepID = GenerateRandomStepID(StepIDs);
                        StepIDs.Add(iStepID);
                        XIDQSStep oStepD = new XIDQSStep();
                        oStepD.sName = "Single Fixed Step".ToLower() + "_" + i++;
                        oStepD.FKiQSDefintionID = oQS.ID;
                        oStepD.ID = iStepID;
                        //Getting Form Component
                        Dictionary<string, object> CmptParams = new Dictionary<string, object>();
                        CmptParams["sName"] = "Form Component";
                        var ComponentDefinition = Connection.Select<XIDComponent>("XIComponents_XC_T", CmptParams).FirstOrDefault();
                        //Assign Form Component Params
                        List<XIDComponentParam> nParms = new List<XIDComponentParam>();
                        nParms.Add(new XIDComponentParam() { sName = "BO", sValue = items.sBO, FKiComponentID = ComponentDefinition.ID, iStepSectionID = iStepID, });
                        nParms.Add(new XIDComponentParam() { sName = "Group", sValue = "Create", FKiComponentID = ComponentDefinition.ID, iStepSectionID = iStepID });
                        if (items.sMode == "SingleView")
                        {
                            nParms.Add(new XIDComponentParam() { sName = "DisplayMode", sValue = "View" });
                        }
                        foreach (var parm in nParms)
                        {
                            Connection.Insert<XIDComponentParam>(parm, "XIComponentParams_T", "ID");
                        }
                        ComponentDefinition.Params.Clear();
                        ComponentDefinition.Params = nParms;
                        oStepD.ComponentDefinition = ComponentDefinition;

                        oQS.Steps[oStepD.sName] = oStepD;
                    }
                    else if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Multiple" || items.sMode == "MultipleView"))
                    {
                        var iStepID = GenerateRandomStepID(StepIDs);
                        StepIDs.Add(iStepID);
                        //Get Default 1-Click of BO
                        var BOUI1ClickID = 0;
                        Dictionary<string, object> BOStructParams = new Dictionary<string, object>();
                        BOStructParams["FKiStructureID"] = items.ID.ToString();
                        var BOStructure = Connection.Select<XIDStructureDetail>("XIBOStructureDetail_T", BOStructParams).FirstOrDefault();
                        //var BOStructure = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == items.ID).FirstOrDefault();
                        if (BOStructure != null)
                        {
                            BOUI1ClickID = BOStructure.i1ClickID;
                        }
                        XIDQSStep oStepD = new XIDQSStep();
                        oStepD.sName = "Multiple Fixed Step" + "_" + i++;
                        //Getting Form Component
                        Dictionary<string, object> CmptParams = new Dictionary<string, object>();
                        CmptParams["sName"] = "OneClickComponent";
                        var ComponentDefinition = Connection.Select<XIDComponent>("XIComponents_XC_T", CmptParams).FirstOrDefault();
                        //Assign Form Component Params
                        List<XIDComponentParam> nParms = new List<XIDComponentParam>();
                        nParms.Add(new XIDComponentParam() { sName = "1ClickID", sValue = BOUI1ClickID.ToString(), FKiComponentID = ComponentDefinition.ID, iStepSectionID = iStepID });
                        nParms.Add(new XIDComponentParam() { sName = "Register", sValue = "yes", FKiComponentID = ComponentDefinition.ID, iStepSectionID = iStepID });
                        if (items.sMode == "MultipleView")
                        {
                            nParms.Add(new XIDComponentParam() { sName = "DisplayMode", sValue = "View" });
                        }
                        foreach (var parm in nParms)
                        {
                            Connection.Insert<XIDComponentParam>(parm, "XIComponentParams_T", "ID");
                        }
                        ComponentDefinition.Params.Clear();
                        ComponentDefinition.Params = nParms;
                        oStepD.ComponentDefinition = ComponentDefinition;
                        oStepD.ID = iStepID;
                        oQS.Steps[oStepD.sName] = oStepD;
                    }


                }



                //int i = -1;
                //Dictionary<string, object> StParams = new Dictionary<string, object>();
                //Params["FKiQSDefintionID"] = oQS.ID.ToString();
                //Params["StatusTypeID"] = 10.ToString();
                //var Steps = Connection.Select<XIDQSStep>("XIQSStepDefinition_T", StParams).ToList();
                ////var Steps = dbContext.QSStepDefiniton.Where(m => m.FKiQSDefintionID == oQS.ID).ToList();
                //oQS.Steps = new Dictionary<string, XIDQSStep>();
                //foreach (var stp in Steps)
                //{
                //    oQS.Steps[stp.sName] = stp;
                //}
                ////oQS.Steps.AddRange(Steps);
                //foreach (var items in oTree)
                //{
                //    if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Single" || items.sMode == "SingleView"))
                //    {
                //        XIDQSStep oStep = new XIDQSStep();
                //        oStep = oQS.Steps.Values.Where(m => m.sName.ToLower() == "Single Fixed Step".ToLower()).FirstOrDefault();
                //        Dictionary<string, object> StcParams = new Dictionary<string, object>();
                //        Params["iStepDefinitionID"] = oStep.ID.ToString();
                //        var ExistingParams = Connection.Select<XIDQSStep>("XIComponentParams_T", StcParams).ToList();
                //        //var ExistingParams = dbContext.XIComponentParams.Where(m => m.iStepDefinitionID == oStep.ID).ToList();
                //        if (ExistingParams.Count() > 0)
                //        {
                //            //ExistingParams.ToList().ForEach(m => m.iStepDefinitionID = 0);
                //            //dbContext.SaveChanges();
                //        }
                //        Dictionary<string, object> CmptParams = new Dictionary<string, object>();
                //        Params["ID"] = oStep.iXIComponentID.ToString();
                //        var ComponentDefinition = Connection.Select<XIDComponent>("XIComponents_XC_T", CmptParams).FirstOrDefault();
                //        //var ComponentDefinition = dbContext.XIComponents.Where(m => m.ID == oStep.iXIComponentID).FirstOrDefault();
                //        List<XIDComponentParam> nParms = new List<XIDComponentParam>();
                //        nParms.Add(new XIDComponentParam() { sName = "BO", sValue = items.sBO });
                //        nParms.Add(new XIDComponentParam() { sName = "Group", sValue = "Create" });
                //        if (items.sMode == "SingleView")
                //        {
                //            nParms.Add(new XIDComponentParam() { sName = "DisplayMode", sValue = "View" });
                //        }
                //        ComponentDefinition.Params.Clear();
                //        ComponentDefinition.Params = nParms;
                //        oStep.ComponentDefinition = ComponentDefinition;
                //        if (oStep.sName != null)
                //        {
                //            NewSteps[oStep.sName] = oStep;
                //        }
                //        else
                //        {
                //            NewSteps[oStep.ID.ToString()] = oStep;
                //        }
                //        //NewSteps.Add(oStep);
                //    }
                //    else if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Multiple" || items.sMode == "MultipleView"))
                //    {
                //        var BOUI1ClickID = 0;
                //        Dictionary<string, object> CmptParams = new Dictionary<string, object>();
                //        Params["FKiStructureID"] = items.ID.ToString();
                //        var BOStructure = Connection.Select<XIDStructure>("XIBOStructure_T", CmptParams).FirstOrDefault();
                //        //var BOStructure = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == items.ID).FirstOrDefault();
                //        if (BOStructure != null)
                //        {
                //            BOUI1ClickID = BOStructure.i1ClickID;
                //        }
                //        XIDQSStep oStep = new XIDQSStep();
                //        var StepDef = oQS.Steps.Values.Where(m => m.sName.ToLower() == "Multiple Fixed Step".ToLower()).FirstOrDefault();
                //        var ComDef = new XIDComponent();
                //        ComDef.ID = StepDef.iXIComponentID;
                //        Dictionary<string, object> secParams = new Dictionary<string, object>();
                //        Params["iStepSectionID"] = i.ToString();
                //        var ExistingParams = Connection.Select<XIDComponentParam>("XIComponentParams_T", secParams).ToList();
                //        //var ExistingParams = dbContext.XIComponentParams.Where(m => m.iStepSectionID == i).ToList();
                //        if (ExistingParams.Count() > 0)
                //        {
                //            foreach (var parm in ExistingParams)
                //            {
                //                using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                //                {
                //                    Conn.Open();
                //                    SqlCommand SqlCmd = new SqlCommand();
                //                    SqlCmd.Connection = Conn;
                //                    string cmdText = "delete from XIComponentParams_T where id =" + parm.ID;
                //                    SqlCommand cmd = new SqlCommand(cmdText, Conn);
                //                    cmd.ExecuteNonQuery();
                //                    Conn.Close();
                //                }
                //            }
                //            //ExistingParams.ToList().ForEach(m => m.iStepDefinitionID = 0);

                //        }
                //        List<XIDComponentParam> nParms = new List<XIDComponentParam>();
                //        nParms.Add(new XIDComponentParam() { sName = "1ClickID", sValue = BOUI1ClickID.ToString(), FKiComponentID = StepDef.iXIComponentID, iStepSectionID = StepDef.ID });
                //        nParms.Add(new XIDComponentParam() { sName = "Register", sValue = "yes", FKiComponentID = StepDef.iXIComponentID, iStepSectionID = StepDef.ID });
                //        if (items.sMode == "MultipleView")
                //        {
                //            nParms.Add(new XIDComponentParam() { sName = "DisplayMode", sValue = "View" });
                //        }
                //        foreach (var param in nParms)
                //        {
                //            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                //            {
                //                Conn.Open();
                //                SqlCommand SqlCmd = new SqlCommand();
                //                SqlCmd.Connection = Conn;
                //                string cmdText = "INSERT INTO XIComponentParams_T(sName, sValue, FKiComponentID, iStepSectionID) VALUES('" + param.sName + "','" + param.sValue + "', " + param.FKiComponentID + "," + i + ")";
                //                SqlCommand cmd = new SqlCommand(cmdText, Conn);
                //                cmd.ExecuteNonQuery();
                //                Conn.Close();
                //            }
                //        }


                //        ComDef.ID = StepDef.iXIComponentID;
                //        ComDef.Params = nParms;
                //        oStep.ComponentDefinition = ComDef;
                //        oStep.ID = i;
                //        if (oStep.sName != null)
                //        {
                //            NewSteps[oStep.sName] = oStep;
                //        }
                //        else
                //        {
                //            NewSteps[oStep.ID.ToString()] = oStep;
                //        }
                //        //NewSteps.Add(oStep);
                //        i--;
                //    }
                //}
            }
            //FixedTemplate.Steps = NewSteps;
            return oQS;
        }

        Random rnd = new Random();
        private int GenerateRandomStepID(List<int> stepIDs)
        {
            // creates a number between 1 and 6
            int iStepID = rnd.Next(20000, 40000);
            if (iStepID % 15 != 0)
            {
                GenerateRandomStepID(stepIDs); // This is the recursion
            }
            return iStepID;
        }

        #endregion QuestionSetComponent

        #region QSConfigComponent

        public CResult Get_QSConfigDef()
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
                XIDQS oQS = new XIDQS();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (ID != 0)
                {
                    Params["ID"] = ID;
                    oQS = Connection.Select<XIDQS>("XIQSDefinition_T", Params).FirstOrDefault();
                }
                //Visualisation DropDowns
                List<XIDropDown> oVisualDDL = new List<XIDropDown>();
                Dictionary<string, object> oVisual = new Dictionary<string, object>();
                //oVisual["FKiApplicationID"] = FKiApplicationID;
                //oVisual["OrganisationID"] = FKiOrgID;
                var oVisualDef = Connection.Select<XIVisualisation>("XiVisualisations", oVisual).ToList();
                oVisualDDL = oVisualDef.Select(m => new XIDropDown { Value = m.XiVisualID, text = m.Name }).ToList();
                oVisualDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQS.ddlXIVisualisations = oVisualDDL;

                //Parameters DropDowns
                List<XIDropDown> oParmasDDL = new List<XIDropDown>();
                Dictionary<string, object> oParams = new Dictionary<string, object>();
                oParams["FKiApplicationID"] = FKiApplicationID;
                oParams["OrganisationID"] = FKiOrgID;
                oParams["StatusTypeID"] = 10;
                var oParamDef = Connection.Select<XIParameter>("XIParameters", oParams).ToList();
                oParmasDDL = oParamDef.Select(m => new XIDropDown { Value = m.XiParameterID, text = m.Name }).ToList();
                oParmasDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQS.ddlXIParameters = oParmasDDL;

                //BOStructure DropDowns
                List<XIDropDown> oBOStruDDL = new List<XIDropDown>();
                Dictionary<string, object> oBOStr = new Dictionary<string, object>();
                oBOStr["FKiXIApplicationID"] = FKiApplicationID;
                oBOStr["OrganisationID"] = FKiOrgID;
                oBOStr["FKiParentID"] = "#";
                var oBOStruDef = Connection.Select<XIDStructure>("XIBOStructure_T", oBOStr).ToList();
                oBOStruDDL = oBOStruDef.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sCode }).ToList();
                oBOStruDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQS.ddlXIStructures = oBOStruDDL;

                //BOSource DropDowns
                List<XIDropDown> oBOSourceDDL = new List<XIDropDown>();
                Dictionary<string, object> oBOSou = new Dictionary<string, object>();
                //oBOSou["FKiApplicationID"] = FKiApplicationID;
                //oBOSou["OrganisationID"] = FKiOrgID;
                var oBOSourceDef = Connection.Select<XIDSource>("XISource_T", oBOSou).ToList();
                oBOSourceDDL = oBOSourceDef.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
                oBOSourceDDL.Insert(0, new XIDropDown
                {
                    text = "--Select--",
                    Value = 0
                });
                oQS.ddlSourceList = oBOSourceDDL;
                if (ID > 0)
                {
                    oQS.FKiApplicationID = oQS.FKiApplicationID;
                }
                else
                {
                    oQS.FKiApplicationID = FKiApplicationID;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oQS;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }

        #endregion QSConfigComponent

    }

    public class XIRequiredParamDef
    {
        public int ID { get; set; }
        public int FKiQSDID { get; set; }
        public Guid FKiQSDIDXIGUID { get; set; }
        public string sParamName { get; set; }
        public string sDefaultValue { get; set;}
        public int iDefault { get; set; }
        public int FKiQSIID { get; set; }
        public Guid FKiQSIIDXIGUID { get; set; }
        public int iRequired { get; set; }
        public int iPersisted { get; set; }
        public int iSetValue { get; set; }
    }
}