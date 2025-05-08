using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using Microsoft.AspNet.Identity;
using XIDNA.Common;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using XIDNA.Mailer;
using XICore;
using XIInfrastructure;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using XISystem;
using System.IO;
//using NPOI.XWPF.UserModel;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using ZeeInsurance;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Web.UI;
using ZeeBNPPFServices;
//using ZeePremiumFinance;
//using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Web.UI.WebControls;
//using System.Web.UI;
//using ZeePremiumFinance;
//using ZeeBNPPFServices;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class XiLinkController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IXiLinkRepository XiLinkRepository;
        private readonly IContentRepository ContentRepository;

        public XiLinkController() : this(new XiLinkRepository(), new ContentRepository()) { }

        public XiLinkController(IXiLinkRepository XiLinkRepository, IContentRepository ContentRepository)
        {
            this.XiLinkRepository = XiLinkRepository;
            this.ContentRepository = ContentRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        XIDropDown XIDrop = new XIDropDown();
        CommonRepository Common = new CommonRepository();
        BusinessObjectsRepository BusinessObjectsRepository = new BusinessObjectsRepository();
        XIComponentsRepository XIComponentsRepository = new XIComponentsRepository();
        XIInfraCache oCache = new XIInfraCache();
        XIInfraEmail oEmail = new XIInfraEmail();
        CXiAPI oXiAPI = new CXiAPI();
        ContentController ContentController = new ContentController();
        XIIXI oIXI = new XIIXI();
        XID1Click oD1Click = new XID1Click();

        Dictionary<string, object> Params = new Dictionary<string, object>();
        private int OCID { get; set; }
        private string sSearchType { get; set; }
        private string sSearchText { get; set; }
        private int LoadingPattern = -1;
        private int PopupID { get; set; }
        public string BO { get; set; }
        public string Group { get; set; }
        public string StartAction { get; set; }
        public string ContentType { get; set; }
        public string BespokeURL { get; set; }
        public List<string> MenuIDs { get; set; }
        public string MenuName { get; set; }
        public int DialogID { get; set; }
        public int iInstanceID { get; set; }
        public string sVisualisation { get; set; }
        public int iQSDID { get; set; }
        public string sQSType { get; set; }
        public string sStepName { get; set; }
        public string FormName { get; set; }
        public string ActiveForeginKey { get; set; }
        public int iBODID { get; set; }
        public string sMode { get; set; }
        public int iCustomerID { get; set; }
        public int iLayoutID { get; set; }
        int iStructureLoopCount = 0;
        int iLoopCount = 0;
        //
        // GET: /XiLink/
        public ActionResult Index()
        {
            return View();
        }

        #region Reconcilliation_Methods

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PostReconcilliation(int ReconcilliationID, string ActRec = "")
        {
            try
            {
                Reconcilliation Reconl = new Reconcilliation();
                var Reconcilliation = Reconl.PreProcess(ReconcilliationID, ActRec);
                if (Reconcilliation.bOK == true)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region AddEditXiLink
        public ActionResult XiLinksList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = XiLinkRepository.XiLinksList(param, iUserID, sOrgName, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ViewXiLink(int XiLinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiLinks model = new VMXiLinks();
                VMXiLinks Result = XiLinkRepository.GetXiLinkByID(XiLinkID, sDatabase);
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    Result.ddlApplications = Common.GetApplicationsDDL();
                }
                Result.FKiApplicationID = fkiApplicationID;
                return PartialView("_ViewXiLink", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddEditXiLink(int XiLinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                VMXiLinks model = new VMXiLinks();
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                model.ddlXIComponents = Common.GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
                ModelDbContext code = new ModelDbContext();
                var res = code.XIComponents.ToList();
                if (fkiApplicationID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                if (XiLinkID == 0)
                {
                    model.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiLink", model);
                }
                else
                {
                    VMXiLinks Result = XiLinkRepository.GetXiLinkByID(XiLinkID, sDatabase);
                    Result.ddlXIComponents = Common.GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Result.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Result.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiLink", Result);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveEditXiLink(int XiLinkID, string Name, string URL, int OneClickID, int FKiApplicationID, string[] NVPairs, string[] LNVPairs, int FKiComponentID, int Status, string sActive, string sType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiLinks Link = new VMXiLinks();
                Link.XiLinkID = XiLinkID;
                Link.Name = Name;
                Link.URL = URL;
                Link.OneClickID = OneClickID;
                Link.FKiComponentID = FKiComponentID;
                Link.FKiApplicationID = FKiApplicationID;
                Link.NVPairs = NVPairs;
                Link.LNVPairs = LNVPairs;
                Link.sActive = sActive;
                Link.sType = sType;
                Link.StatusTypeID = Status;
                Link.CreatedBy = iUserID;
                Link.UpdatedBy = iUserID;
                var Result = XiLinkRepository.SaveXiLink(Link, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IsExistsXiLinkName(string Name, int XiLinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                return XiLinkRepository.IsExistsXiLinkName(Name, XiLinkID, iUserID, sOrgName, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //CopyXiLinkByXiLinkID
        public ActionResult CopyXiLink(int XiLinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                var CopyXiLink = XiLinkRepository.CopyXiLinkByID(XiLinkID, OrgID, iUserID, sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion AddEditXiLink

        #region AddEditXiParameters

        public ActionResult XiParameters()
        {
            return View();
        }

        public ActionResult XiParametersList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = XiLinkRepository.XiParametersList(param, iUserID, sOrgName, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ViewXiParameter(int XiParameterID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiParameters model = new VMXiParameters();
                VMXiParameters Result = XiLinkRepository.GetXiParameterByID(XiParameterID, sDatabase);
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    Result.ddlApplications = Common.GetApplicationsDDL();
                }
                Result.FKiApplicationID = fkiApplicationID;
                return PartialView("_ViewXiParameter", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddEditXiParameter(int XiParameterID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var oParam = XiLinkRepository.GetXIParameterDetails(XiParameterID, sDatabase);
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiParameters model = new VMXiParameters();
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                if (XiParameterID == 0)
                {
                    model.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiParameter", model);
                }
                else
                {
                    VMXiParameters Result = XiLinkRepository.GetXiParameterByID(XiParameterID, sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Result.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Result.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiParameter", Result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveEditXiParameter(int XiParameterID, string Name, int FKiApplicationID, string[] NVPairs, string[] LNVPairs, int Status)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiParameters Parameter = new VMXiParameters();
                Parameter.XiParameterID = XiParameterID;
                Parameter.FKiApplicationID = FKiApplicationID;
                Parameter.Name = Name;
                //Parameter.URL = URL;
                //Parameter.OneClickID = OneClickID;
                Parameter.NVPairs = NVPairs;
                Parameter.LNVPairs = LNVPairs;
                Parameter.StatusTypeID = Status;
                Parameter.CreatedBy = iUserID;
                Parameter.UpdatedBy = iUserID;
                var Result = XiLinkRepository.SaveXiParameter(Parameter, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion AddEditXiParameters
        #region AddEditXiVisualisations

        public ActionResult XiVisualisations()
        {
            return View();
        }

        public ActionResult XiVisualisationsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = XiLinkRepository.XiVisualisationsList(param, iUserID, sOrgName, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ViewXiVisualisation(int XiVisualID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiVisualisations model = new VMXiVisualisations();
                VMXiVisualisations Result = XiLinkRepository.GetXiVisualisationByID(XiVisualID, iUserID, sOrgName, sDatabase);
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    Result.ddlApplications = Common.GetApplicationsDDL();
                }
                Result.FKiApplicationID = fkiApplicationID;
                return PartialView("_ViewXiVisualisation", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddEditXiVisualisation(int XiVisualID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiVisualisations model = new VMXiVisualisations();
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                if (XiVisualID == 0)
                {
                    model.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiVisualisation", model);
                }
                else
                {
                    VMXiVisualisations Result = XiLinkRepository.GetXiVisualisationByID(XiVisualID, iUserID, sOrgName, sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Result.ddlApplications = Common.GetApplicationsDDL();
                    }
                    //Result.FKiApplicationID = fkiApplicationID;
                    Result.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddEditXiVisualisation", Result);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveEditXiVisualisation(int XiVisualID, string Type, int FKiApplicationID, string Name, string[] NVPairs, string[] LNVPairs, int Status)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMXiVisualisations Visualisation = new VMXiVisualisations();
                Visualisation.XiVisualID = XiVisualID;
                Visualisation.FKiApplicationID = FKiApplicationID;
                Visualisation.Name = Name;
                Visualisation.Type = Type;
                Visualisation.NVPairs = NVPairs;
                Visualisation.LNVPairs = LNVPairs;
                Visualisation.StatusTypeID = Status;
                Visualisation.CreatedBy = iUserID;
                Visualisation.UpdatedBy = iUserID;
                var Result = XiLinkRepository.SaveEditXiVisualisation(Visualisation, iUserID, sOrgName, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IsExistsXiVisualisationsName(string Name, int XiVisualID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                return XiLinkRepository.IsExistsXiVisualisationsName(Name, XiVisualID, iUserID, sOrgName, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        #endregion AddEditXiVisualisations

        #region XiLinkData

        [AllowAnonymous]
        public ActionResult GetXiLinkContent(int XiLinkID, string sGUID, int BODID = 0, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            //if (string.IsNullOrEmpty(sGUID))
            //{
            //    if (SessionManager.sGUID != null) 
            //    {
            //        sGUID = SessionManager.sGUID;
            //    }
            //}
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                var sSessionID = HttpContext.Session.SessionID;
                if (sGUID != null)
                {
                    //var sParentGUID = oCache.Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
                    //if (!string.IsNullOrEmpty(sParentGUID))
                    //{
                    //    sGUID = sParentGUID;
                    //}
                    if (BODID != 0 && ID != 0)
                    {
                        XIDBO oBOD = new XIDBO();
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODID.ToString());
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|" + oBOD.Name + ".id}", ID.ToString(), null, null);
                    }
                    BO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");
                    ActiveForeginKey = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveFK}");
                    iInstanceID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + BO + ".id}"));
                    iCustomerID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iCustomerID}"));
                    //iUserid = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iUserID}"));
                }
                else
                {
                    iInstanceID = 0;
                }
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = 0;
                string sUserRoleName = string.Empty;
                if (oUser != null)
                {
                    OrgID = oUser.FKiOrganisationID;
                    if (oUser.Role != null)
                    {
                        sUserRoleName = oUser.Role.sRoleName;
                    }
                }
                var XiLink = GetXiLinkDetails(XiLinkID, sGUID);
                var ReportID = OCID;
                string ResultIn = "";
                string Query = "";
                int ClassValue = 0;
                int DateValue = 0;
                XIRun(XiLink, sGUID);
                if (XiLink.FKiComponentID > 0)
                {
                    ViewBag.sGUID = sGUID;
                    ViewBag.XiLinkID = XiLinkID;
                    cXIComponents oXIComponent = XIComponentsRepository.XIInitialise(XiLink.FKiComponentID, iUserID, sOrgName, sDatabase);
                    var Params = oXIComponent.XIComponentParams.Where(m => m.iXiLinkID == XiLink.XiLinkID).ToList();
                    oXIComponent.XIComponentParams = Params;
                    oXIComponent.XIComponentParams.Where(m => m.sValue == "{XIP|BODID}").ToList().ForEach(m => m.sValue = BODID.ToString());
                    return PartialView("_LoadComponent", oXIComponent); // RedirectToAction("LoadComponentByID", "XIComponents", new { iXIComponentID = XiLink.FKiComponentID, sGUID = "" });
                }
                else if (StartAction == null)
                {
                    if (!string.IsNullOrEmpty(XiLink.URL))
                    {
                        var URL = XiLink.URL;
                        if (URL.IndexOf("|") > 0)
                        {
                            var URLParams = URL.Split('|').ToList();
                            if (URLParams[0] == "XIN")
                            {
                                XIIXI oXII = new XIIXI();
                                sStepName = URLParams[1];
                                var QSDID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}");
                                XIIQS oQSI = oXII.GetQSInstanceByID(Convert.ToInt32(QSDID));
                                var oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, oQSI.FKiQSDefinitionID.ToString());
                                var StepDef = oQSD.Steps[sStepName];
                                var oParams = new List<CNV>();
                                oParams.Add(new CNV { sName = "{XIP|iStepDID}", sValue = StepDef.ID.ToString() });

                                var SessionItems = SessionManager.SessionItems();
                                oParams.AddRange(SessionItems);
                                XIDComponent oComponentD = new XIDComponent();
                                oComponentD.sName = "Step Component";
                                //oComponentD.iQSIID = QSDID;
                                oComponentD.sGUID = sGUID;
                                oComponentD.nParams = oParams;
                                var sQSIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}");
                                int iQSIID = 0;
                                XIIQS oQSInstance = new XIIQS();
                                CResult oCResult = oCache.MergeXILinkParameters(XiLink, sGUID, null, sSessionID);
                                if (int.TryParse(sQSIID, out iQSIID))
                                {
                                    oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, iQSIID);
                                    oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, StepDef.ID);
                                    var oCurrentStep = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == StepDef.ID).FirstOrDefault().Value;
                                    if (oCurrentStep.QSLinks != null && oCurrentStep.QSLinks.Count() > 0)
                                    {
                                        foreach (var oQSLink in oCurrentStep.QSLinks.Values)
                                        {
                                            foreach (var oLink in oQSLink.XiLink.OrderBy(m => m.Value.rOrder))
                                            {
                                                if (oLink.Value.sType == "Pre")
                                                {
                                                    GetXiLinkContent(oLink.Value.FKiXILInkID, sGUID);
                                                }
                                            }
                                        }
                                    }

                                    oQSInstance.Steps.Values.ToList().ForEach(m => m.bIsCurrentStep = false);
                                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == StepDef.ID).FirstOrDefault().bIsCurrentStep = true;
                                    oQSInstance.iCurrentStepID = StepDef.ID;
                                    oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == StepDef.ID).Select(m => m.sName).FirstOrDefault();
                                }
                                //var Response = (XIDQSStep)oComponentD.LoadComponent("QSStep", StepDef.ID).oResult;
                                //var oXIComponent = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, "Step Component", "");
                                //ViewBag.sGUID = sGUID;
                                //return PartialView(oXIComponent.sHTMLPage, Response);

                                //return RedirectToAction("LoadComponentByID", "XIComponents", new { iXIComponentID = 0, sGUID = sGUID, nParams = oParams, sName = "Step Component", sType = "QSStep", ID = StepDef.ID, iInstanceID = 0, sContext = string.Empty, iQSIID = QSDID });
                                //var oQSInstance = GetQSInstance(iQSDID, sGUID, null, 0, 0);
                                ViewBag.sGUID = sGUID;
                                return PartialView("_QuestionSet", oQSInstance);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            CResult oCResult = oCache.MergeXILinkParameters(XiLink, sGUID, null, sSessionID);
                            return XIMethod(XiLink, sGUID);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (StartAction.ToLower() == "Search".ToLower())
                {
                    if (OCID > 0)
                    {
                        ViewBag.XiLinkID = XiLinkID;
                        var Result = OneClickResult(sGUID);
                        Result.PreviewType = null;
                        Result.sGUID = sGUID;
                        List<VMNameValuePairs> Params = new List<VMNameValuePairs>();
                        Params.Add(new VMNameValuePairs { sName = "Context", sValue = "1Click" });
                        if (!string.IsNullOrEmpty(ActiveForeginKey))
                        {
                            Params.Add(new VMNameValuePairs { sName = ActiveForeginKey, sValue = iInstanceID.ToString(), sType = "autoset", sContext = "1Click" });
                        }
                        if (!string.IsNullOrEmpty(BO))
                        {
                            Params.Add(new VMNameValuePairs { sName = "ActiveBO", sValue = BO.ToString(), sContext = "1Click" });
                        }
                        if (!string.IsNullOrEmpty(ActiveForeginKey))
                        {
                            Params.Add(new VMNameValuePairs { sName = "ActiveFK", sValue = ActiveForeginKey.ToString(), sContext = "1Click" });
                        }
                        if (iCustomerID == 0)
                        {
                            Params.Add(new VMNameValuePairs { sName = "{XIP|iCustomerID}", sValue = 34.ToString(), sContext = null });
                        }
                        Params.Add(new VMNameValuePairs { sName = "{XIP|iUserID}", sValue = iUserID.ToString(), sContext = null });


                        Result.nParams = Params;
                        List<cNameValuePairs> oParams = new List<cNameValuePairs>();
                        oParams = Params.Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue, sContext = m.sContext, sType = m.sType }).ToList();
                        //SetXIParams(oParams, sGUID);
                        ViewBag.RoleName = sUserRoleName;
                        return View("OneClickResults", Result);
                        //  return View("OneClickResultsList", Result);
                    }
                }
                else if (StartAction.ToLower() == "SearchList".ToLower())
                {
                    if (OCID > 0)
                    {
                        ViewBag.XiLinkID = XiLinkID;
                        var Result = OneClickResult(sGUID);
                        Result.PreviewType = "Popup";
                        Result.sGUID = sGUID;
                        return View("_SearchList", Result);
                        //  return View("OneClickResultsList", Result);
                    }
                }
                else if (StartAction.ToLower() == "PopupLeftContent".ToLower())
                {
                    if (BO != null && Group != null)
                    {
                        var oBODisplay = oXiAPI.GetFormData(BO, Group, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                        if (!string.IsNullOrEmpty(sVisualisation))
                        {
                            var Visualisations = oXiAPI.GetVisualistions(BO, sVisualisation, iInstanceID, iUserID, sOrgName, sDatabase);
                            oBODisplay.Visualisations = Visualisations;
                        }
                        return PartialView("_LeadPopupLeftContent", oBODisplay);
                    }
                }
                else if (StartAction.ToLower() == "InlineView".ToLower())
                {
                    if (BO != null && Group != null)
                    {
                        var oBODisplay = oXiAPI.GetFormData(BO, Group, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                        if (!string.IsNullOrEmpty(sVisualisation))
                        {
                            var Visualisations = oXiAPI.GetVisualistions(BO, sVisualisation, iInstanceID, iUserID, sOrgName, sDatabase);
                            oBODisplay.Visualisations = Visualisations;
                        }
                        return PartialView("_InlineView", oBODisplay);
                    }
                }
                else if (StartAction.ToLower() == "InlineEdit".ToLower() || StartAction.ToLower() == "CreateForm".ToLower())
                {
                    if (BO != null && Group != null)
                    {
                        var oBODisplay = oXiAPI.GetFormData(BO, Group, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                        if (StartAction.ToLower() == "CreateForm".ToLower())
                        {
                            var IDPair = oBODisplay.BOInstance.NVPairs.Where(m => m.sName.ToLower() == "id").FirstOrDefault();
                            if (IDPair != null && IDPair.sValue == null)
                            {
                                oBODisplay.BOInstance.NVPairs.Remove(IDPair);
                            }
                        }

                        if (!string.IsNullOrEmpty(sVisualisation))
                        {
                            var Visualisations = oXiAPI.GetVisualistions(BO, sVisualisation, iInstanceID, iUserID, sOrgName, sDatabase);
                            oBODisplay.Visualisations = Visualisations;
                        }
                        return PartialView("_CreateForm", oBODisplay);
                    }
                }
                else if (StartAction.ToLower() == "List".ToLower())
                {
                    if (OCID > 0)
                    {
                        var Result = XiLinkRepository.GetHeadings(OCID, null, sDatabase, OrgID, Convert.ToInt32(User.Identity.GetUserId()), sOrgName);
                        Result.ReportID = OCID;
                        Result.LeadID = iInstanceID;
                        Result.sGUID = sGUID;
                        //Result.PreviewType = "Inline";
                        return PartialView("_List", Result);
                    }
                }
                else if (StartAction.ToLower() == "OneClickList".ToLower())
                {
                    if (OCID > 0)
                    {
                        var Result = XiLinkRepository.GetHeadings(OCID, null, sDatabase, OrgID, Convert.ToInt32(User.Identity.GetUserId()), sOrgName);
                        Result.ReportID = OCID;
                        Result.PreviewType = "Inline";
                        return PartialView("_List", Result);
                    }
                }
                else if (StartAction.ToLower() == "BarChart".ToLower())
                {
                    if (OCID > 0)
                    {
                        ReportID = OCID;
                        VMChart Charts = new VMChart();
                        Charts.ReportID = ReportID;
                        Charts.OrgID = OrgID;
                        Charts.UserID = iUserID;
                        Charts.Database = sDatabase;
                        Charts.DateFilter = DateValue;
                        Charts.ClassFilter = ClassValue;
                        Charts.Query = Query;
                        LineGraph graph = XiLinkRepository.GetBarChart(Charts, sDatabase, iUserID, sOrgName);
                        graph.ReportID = ReportID;
                        graph.Type = "Run";
                        graph.SectionName = "";
                        graph.ClassDDL = new List<VMDropDown>();
                        graph.DateDDL = new List<VMDropDown>();
                        return PartialView("_KPIBarChart", graph);
                    }
                }

                else if (StartAction.ToLower() == "PieChart".ToLower())
                {
                    if (OCID > 0)
                    {
                        ReportID = OCID;
                        VMChart Chart = new VMChart();
                        Chart.ReportID = ReportID;
                        Chart.Query = Query;
                        Chart.ClassFilter = ClassValue;
                        Chart.DateFilter = DateValue;
                        Chart.OrgID = OrgID;
                        Chart.UserID = iUserID;
                        Chart.Database = sDatabase;
                        GraphData Graph = new GraphData();
                        Graph = XiLinkRepository.GetPieChart(Chart, iUserID, sOrgName, sDatabase);
                        Graph.Type = "Run";
                        Graph.SectionName = "";
                        Graph.ClassDDL = new List<VMDropDown>();
                        Graph.DateDDL = new List<VMDropDown>();
                        return PartialView("_KPIPieChart", Graph);
                    }
                }

                else if (StartAction.ToLower() == "LineGraph".ToLower())
                {
                    if (OCID > 0)
                    {
                        ReportID = OCID;
                        VMChart Chart = new VMChart();
                        Chart.ReportID = ReportID;
                        Chart.OrgID = OrgID;
                        Chart.UserID = iUserID;
                        Chart.Database = sDatabase;
                        Chart.DateFilter = DateValue;
                        Chart.ClassFilter = ClassValue;
                        Chart.Query = Query;
                        LineGraph graph = XiLinkRepository.GetLineChart(Chart, iUserID, sOrgName, sDatabase);
                        graph.ReportID = ReportID;
                        graph.Type = "Run";
                        graph.SectionName = "";
                        graph.ClassDDL = new List<VMDropDown>();
                        graph.DateDDL = new List<VMDropDown>();
                        return PartialView("_LineGraph", graph);
                    }
                }

                else if (StartAction.ToLower() == "Circle".ToLower())
                {
                    ReportID = OCID;
                    VMChart Chart = new VMChart();
                    Chart.ReportID = ReportID;
                    Chart.OrgID = OrgID;
                    Chart.UserID = iUserID;
                    Chart.Database = sDatabase;
                    Chart.DateFilter = DateValue;
                    Chart.ClassFilter = ClassValue;
                    Chart.Query = Query;
                    List<VMKPIResult> KPIs = XiLinkRepository.GetKPICircleResult(Chart, iUserID, sDatabase, sOrgName);
                    KPIs.FirstOrDefault().UserID = iUserID;
                    return PartialView("_KPICircles", KPIs);
                }

                else if (StartAction.ToLower() == "Popup".ToLower() || StartAction.ToLower() == "InlinePopup".ToLower())
                {
                    var PopDetails = XiLinkRepository.GetPopupLayoutDetails(PopupID, 0, 0, sGUID, iUserID, sOrgName, sDatabase);
                    return PartialView("_PopupContent", PopDetails);
                }
                else if (StartAction.ToLower() == "Inline".ToLower())
                {
                    var PopDetails = XiLinkRepository.GetPopupLayoutDetails(PopupID, 0, 0, sGUID, iUserID, sOrgName, sDatabase);
                    return PartialView("_InlineContent", PopDetails);
                }
                else if (StartAction.ToLower() == "Dialog".ToLower())
                {
                    var DailogDetails = XiLinkRepository.GetDialogLayoutDetails(DialogID, 0, 0, sGUID, iUserID, sOrgName, sDatabase);
                    return PartialView("_DialogContent", DailogDetails);
                }
                //else if (StartAction.ToLower() == "Menu".ToLower())
                //{
                //    var url = XiLink.URL.Split('/').ToList();
                //    return RedirectToAction(url[1], url[0]);
                //}
                else if (StartAction.ToLower() == "Bespoke".ToLower())
                {
                    return PartialView("_Bespoke", BespokeURL);
                }
                else if (StartAction.ToLower() == "Menu".ToLower())
                {
                    List<RightMenuTrees> Menus = new List<RightMenuTrees>();
                    if (MenuName != null && MenuName.Length > 0)
                    {
                        int UserID = iUserID;
                        // Menus = XiLinkRepository.GetMenus(MenuIDs, OrgID);
                        Menus = XiLinkRepository.GetMenus(MenuName, UserID, OrgID, sDatabase);
                    }
                    return PartialView("_Menus", Menus);
                }
                else if (StartAction.ToLower() == "Tabs".ToLower())
                {
                    List<XiLinkNVs> Tabs = new List<XiLinkNVs>();
                    var TabsDetails = XiLinkRepository.GetTabsDetails(XiLinkID, sDatabase);
                    ViewBag.LeadID = iInstanceID;
                    return PartialView("_TabsContent", TabsDetails);
                }
                else if (StartAction.ToLower() == "DynamicForm".ToLower())
                {
                    var url = XiLink.URL.Split('/').ToList();
                    return RedirectToAction(url[1], url[0], new { XiLinkID = XiLinkID });
                }
                else if (StartAction.ToLower() == "Editable Grid".ToLower())
                {
                    if (OCID > 0)
                    {
                        ViewBag.XiLinkID = XiLinkID;
                        var Result = OneClickResult(sGUID);
                        var oBODisplay = oXiAPI.GetFormData(Result.BO, ServiceConstants.SaveGroup.ToString(), iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                        ViewBag.oBODisplay = oBODisplay;
                        Result.PreviewType = null;
                        // return View("OneClickResults", Result);
                        return View("OneClickResultsList", Result);
                    }
                }
                else if (StartAction.ToLower() == "QuestionSet".ToLower())
                {
                    var stepName = oCache.Get_ParamVal(sSessionID, sGUID, "", "sCurrentStepName");
                    if (!string.IsNullOrEmpty(stepName))
                    {
                        sStepName = stepName;
                    }
                    var oQSInstance = GetQSInstance(iQSDID, sGUID, sMode, iBODID, iInstanceID);
                    ViewBag.sGUID = sGUID;
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}", Convert.ToString(oQSInstance.ID), "autoset", null);
                    if (!string.IsNullOrEmpty(sQSType))
                    {
                        oQSInstance.sQSType = sQSType;
                    }
                    oCache.Set_ParamVal(sSessionID, sGUID, "", "sCurrentStepName", "", null, null);
                    oCache.Set_QuestionSetCache("QSI", sGUID, oQSInstance.ID, oQSInstance);
                    oCache.Set_ParamVal(sSessionID, sGUID, "", "sCurrentStepName", "", null, null);
                    return PartialView("QuestionSet", oQSInstance);
                }
                else if (StartAction.ToLower() == "DataSource".ToLower())
                {
                    var url = XiLink.URL.Split('/').ToList();
                    return RedirectToAction(url[1], url[0]);
                }
                else if (StartAction.ToLower() == "QSStep".ToLower())
                {
                    var oQSInstance = GetQSInstance(iQSDID, sGUID, sMode, iBODID, iInstanceID);
                    ViewBag.sGUID = sGUID;
                    return PartialView("QuestionSet", oQSInstance);
                }
                else if (StartAction.ToLower() == "QuestionSetInternal".ToLower())
                {
                    var oQSInstance = GetInternalQuestionSet(0);
                    ViewBag.sGUID = sGUID;
                    //oQSInstance.iCurrentStepID = oQSInstance.nStepInstances.Where(m => m.bIsCurrentStep == true).Select(m => m.FKiQSStepDefinitionID).FirstOrDefault();
                    return PartialView("QuestionSetInternal", oQSInstance);
                }
                else if (StartAction.ToLower() == "MyPolicies".ToLower())
                {
                    var Result = OneClickResult(sGUID);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iUserID}", iUserID.ToString(), null, null);
                    ViewBag.sGUID = sGUID;
                    return View("_MyPolicies", Result);
                }
                //else if (StartAction.ToLower() == "DocumentViewer".ToLower())
                //{
                //    var Result = OneClickResult(sGUID);
                //    //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iUserID}", iUserID.ToString(), null, null);
                //    //ViewBag.sGUID = sGUID;
                //    var str = "http://localhost:53996//";
                //    var sPath = "Attachments//33030e0b-9979-411b-9ef8-b6f48270d6c0//devorg//PDF//RunTime_Attachment.pdf";
                //    ViewBag.sFinalPath = str + sPath;
                //    ViewBag.sPath = sPath;
                //    return View("_PDFViewer");
                //}
                else if (StartAction.ToLower() == "MyQuotes".ToLower())
                {

                }
                else if (StartAction.ToLower() == "FAInbox".ToLower())
                {
                    return PartialView("_FAInbox");
                }
                else if (StartAction.ToLower() == "FAEvents".ToLower())
                {
                    return PartialView("_FAEvents");
                }
                else if (StartAction.ToLower() == "Layout".ToLower())
                {
                    //load notifications and noification count
                    XIDLayout oLayout = new XIDLayout();
                    var oLayDef = oCache.GetObjectFromCache(XIConstant.CacheLayout, null, iLayoutID.ToString()); //oXID.Get_LayoutDefinition(null, iLayoutID.ToString());
                    //var oLayDef = oLayout.Load();
                    if (oLayDef != null)
                    {
                        oLayout = (XIDLayout)oLayDef;
                    }
                    oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|iUserID}", iUserID.ToString(), null, null);
                    ViewBag.sGUID = oLayout.sGUID;
                    //sGUID = oLayout.sGUID;

                    return View("_UserLayoutContent", oLayout);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        private ActionResult XIMethod(XILink xiLink, string sGUID)
        {
            var oXILinkC = (XILink)(xiLink.Clone(xiLink));
            var sSessionID = HttpContext.Session.SessionID;
            var sMethodName = oXILinkC.XiLinkNVs.Where(m => m.Name.ToLower() == "sMethodName".ToLower()).Select(m => m.Value).FirstOrDefault();
            string sHTMLPage = string.Empty;
            sHTMLPage = oXILinkC.XiLinkNVs.Where(m => m.Name.ToLower() == "sHTMLPage".ToLower()).Select(m => m.Value).FirstOrDefault();
            var sURL = oXILinkC.URL.Split('/').ToList().First();
            var sClass = oXILinkC.URL.Split('/').ToList().Last();
            //Creating Instance
            Assembly exceutable;
            Type Ltype;
            object objclass;
            exceutable = Assembly.Load(sURL);
            Ltype = exceutable.GetType(sURL + "." + sClass);
            objclass = Activator.CreateInstance(Ltype);
            if (!string.IsNullOrEmpty(sMethodName))
            {
                MethodInfo method = Ltype.GetMethod(sMethodName);
                var sParentGUID = oCache.Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
                if (!string.IsNullOrEmpty(sParentGUID))
                {
                    sGUID = sParentGUID;
                }
                List<CNV> oParams = new List<CNV>();
                //oParams = oCache.ResolveParameters(oParams, sSessionID, sGUID);

                foreach (var items in oXILinkC.XiLinkNVs)
                {
                    if (items.Value != null && items.Value.IndexOf("{XIP") >= 0)
                    {
                        items.Value = oCache.Get_ParamVal(sSessionID, sGUID, null, items.Value);
                    }
                }
                oParams = oXILinkC.XiLinkNVs.Select(m => new CNV { sName = m.Name, sValue = m.Value }).ToList();
                oParams.Add(new CNV { sName = "sGUID", sValue = sGUID });
                oParams.Add(new CNV { sName = "sDataBase", sValue = SessionManager.CoreDatabase });
                oParams.Add(new CNV { sName = "iUserID", sValue = SessionManager.UserID.ToString() });
                oParams.Add(new CNV { sName = "srolename", sValue = SessionManager.sRoleName });
                oParams.Add(new CNV { sName = "sOrgDatabase", sValue = SessionManager.OrgDatabase });
                oParams.Add(new CNV { sName = "sOrgName", sValue = SessionManager.OrganisationName });
                oParams.Add(new CNV { sName = "iOrganizationID", sValue = SessionManager.OrganizationID.ToString() });
                oParams.Add(new CNV { sName = "sSessionID", sValue = sSessionID });
                object[] parametersArray = new object[] { oParams };
                object Response = (object)method.Invoke(objclass, parametersArray);
                if (((CResult)Response).bOK && ((CResult)Response).oResult != null)
                {
                    var oResult = ((CResult)Response).oResult;
                    if (!string.IsNullOrEmpty(sHTMLPage))
                    {
                        ViewBag.sGUID = sGUID;
                        return PartialView(sHTMLPage, oResult);
                    }
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }
                var UserID = oCache.Get_ParamVal(sSessionID, sGUID, null, "-iUserID");
                int iUserID = 0;
                if (int.TryParse(UserID, out iUserID))
                {
                    XIInfraUsers oUser = new XIInfraUsers();
                    oUser.UserID = iUserID;
                    var UserDetails = oUser.Get_UserDetails(SessionManager.CoreDatabase);
                    if (UserDetails.xiStatus == 0 && UserDetails.oResult != null)
                    {
                        oUser = (XIInfraUsers)UserDetails.oResult;
                        SessionManager.UserID = oUser.UserID;
                        SessionManager.sUserName = oUser.sUserName;
                        SessionManager.sRoleName = oUser.Role.sRoleName;
                        SessionManager.OrgDatabase = oUser.sDatabaseName;
                        SessionManager.sEmail = oUser.sEmail;
                        SessionManager.OrganizationID = oUser.FKiOrganisationID;
                    }
                }
            }
            return null;
        }

        public void SetXIParams(List<cNameValuePairs> oParams, string sGUID)
        {
            cXICache oCache = new cXICache();
            string sSessionID = HttpContext.Session.SessionID;
            foreach (var items in oParams)
            {
                if (!string.IsNullOrEmpty(items.sValue))
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, null, items.sName, items.sValue, null, items.nSubParams);
                }
            }
        }

        public XILink GetXiLinkDetails(int XiLinkID, string sGUID)
        {

            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                var XiLink = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, XiLinkID.ToString());
                return XiLink;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [AllowAnonymous]
        public ActionResult GetXiLinkData(int XiLinkID, string sGUID, bool IsMerge)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                var XiLink = (XILink)oCache.GetObjectFromCache(XIConstant.CacheXILink, null, XiLinkID.ToString());
                return Json(XiLink, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult XIRun(XILink oXilink, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (oXilink != null)
                {
                    OCID = oXilink.OneClickID;
                    foreach (var NVPair in oXilink.XiLinkNVs.Where(m => m.XiLinkListID == 0))
                    {
                        if (NVPair.Name.ToLower() == "StartAction".ToLower())
                        {
                            //get the value and assign to property of 1click instance

                            if (NVPair.Value.ToLower() == "Search".ToLower())
                            {
                                sSearchType = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "SearchType".ToLower()).Select(m => m.Value).FirstOrDefault();
                                //get the value and assign to  firststep property of 1click instance
                            }
                            else if (NVPair.Value.ToLower() == "List".ToLower())
                            {
                                //get the value and assign to  firststep property of 1click instance }
                            }
                            else if (NVPair.Value.ToLower() == "InlineView".ToLower() || NVPair.Value.ToLower() == "InlineEdit".ToLower() || NVPair.Value.ToLower() == "CreateForm".ToLower() || NVPair.Value.ToLower() == "PopupLeftContent".ToLower())
                            {
                                ContentType = NVPair.Value;
                                BO = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BO".ToLower()).Select(m => m.Value).FirstOrDefault();
                                Group = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Group".ToLower() || m.Name.ToLower() == "Save Group".ToLower() || m.Name.ToLower() == "Show Group".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sVisualisation = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "visualisation".ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else if (NVPair.Value.ToLower() == "Popup".ToLower() || NVPair.Value.ToLower() == "InlinePopup".ToLower() || NVPair.Value.ToLower() == "Inline".ToLower())
                            {
                                PopupID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "PopupID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                            else if (NVPair.Value.ToLower() == "Bespoke".ToLower())
                            {
                                BespokeURL = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Bespoke url".ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else if (NVPair.Value.ToLower() == "Menu".ToLower())
                            {
                                MenuName = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "MenuName".ToLower()).Select(m => m.Value).FirstOrDefault();
                            }
                            else if (NVPair.Value.ToLower() == "Dialog".ToLower())
                            {
                                DialogID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "DialogID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                            else if (NVPair.Value.ToLower() == "QuestionSet".ToLower())
                            {
                                var sSessionID = HttpContext.Session.SessionID;
                                iQSDID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QuestionSetID".ToLower()).Select(m => m.Value).FirstOrDefault());
                                sQSType = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "sQSType".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sMode = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Mode".ToLower()).Select(m => m.Value).FirstOrDefault();
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|BODID}".ToLower())
                                    {
                                        var ParamBOID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}");
                                        if (!string.IsNullOrEmpty(ParamBOID))
                                        {
                                            iBODID = Convert.ToInt32(ParamBOID);
                                        }
                                    }
                                }
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{xip|boiid}".ToLower())
                                    {
                                        var ParamBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + BO + ".id}");
                                        if (!string.IsNullOrEmpty(ParamBOIID))
                                        {
                                            iInstanceID = Convert.ToInt32(ParamBOIID);
                                        }
                                    }
                                }
                            }
                            else if (NVPair.Value.ToLower() == "QSStep".ToLower())
                            {
                                var sSessionID = HttpContext.Session.SessionID;
                                iQSDID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QSDID".ToLower()).Select(m => m.Value).FirstOrDefault());
                                sStepName = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "QSStepName".ToLower()).Select(m => m.Value).FirstOrDefault();
                                sMode = oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "Mode".ToLower()).Select(m => m.Value).FirstOrDefault();
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BODID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|BODID}".ToLower())
                                    {
                                        var ParamBOID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}");
                                        if (!string.IsNullOrEmpty(ParamBOID))
                                        {
                                            iBODID = Convert.ToInt32(ParamBOID);
                                        }
                                    }
                                }
                                if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).FirstOrDefault() != null)
                                {
                                    if (oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "BOIID".ToLower()).Select(m => m.Value).FirstOrDefault().ToLower() == "{XIP|boiid}".ToLower())
                                    {
                                        var ParamBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|" + BO + ".id}");
                                        if (!string.IsNullOrEmpty(ParamBOIID))
                                        {
                                            iInstanceID = Convert.ToInt32(ParamBOIID);
                                        }
                                    }
                                }
                            }
                            else if (NVPair.Value.ToLower() == "Layout".ToLower())
                            {
                                iLayoutID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "LayoutID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                            StartAction = NVPair.Value;
                        }
                        else if (NVPair.Name.ToLower() == "ListClick".ToLower())
                        {
                            if (NVPair.Value.ToLower() == "Popup".ToLower())
                            {
                                PopupID = Convert.ToInt32(oXilink.XiLinkNVs.Where(m => m.Name.ToLower() == "PopupID".ToLower()).Select(m => m.Value).FirstOrDefault());
                            }
                        }
                        else if (NVPair.Name.ToLower() == "LoadingType".ToLower())
                        {
                            LoadingPattern = Convert.ToInt32(NVPair.Value);
                        }
                    }
                    //check mandatory params are gathered and if not return error with specified params
                    //var Result = ReportResult(ID, 1, "Inline", null, SearchType, null, 0);

                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetParentGUID(string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sSessionID = HttpContext.Session.SessionID;
                var sParentGUID = oCache.Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
                return Json(sParentGUID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateInstanceID(XIIBO oBOInstance, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var iNewInstanceID = string.Empty;
                var id = oBOInstance.Attributes["ID"].sValue;
                iNewInstanceID = id;
                //var IDPair = oBOInstance.NVPairs.Where(m => m.sName.ToLower() == "id".ToLower()).FirstOrDefault();
                //if (IDPair != null)
                //{
                //    iNewInstanceID = IDPair.sValue;
                //}
                if (!string.IsNullOrEmpty(iNewInstanceID))
                {
                    string sSessionID = HttpContext.Session.SessionID;
                    //var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");
                    var ActiveBO = oBOInstance.BOD.TableName;
                    if (ActiveBO == "Driver_T")
                    {
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}", "Driver_T", "autoset", null);
                    }
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|" + ActiveBO + ".id}", Convert.ToString(iNewInstanceID), "autoset", null);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public void ClearCache()
        {
            oCache.ClearCache();
        }

        #endregion XiLinkData

        #region 1ClickResult
        public VMResultList OneClickResult(string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string ResultIn = "Inline";
                string SearchText = sSearchText;
                int PageIndex = 1;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var LoadingType = XiLinkRepository.GetLoadingType(OCID, sDatabase);
                if (LoadingPattern != -1)
                {
                    LoadingType[0] = LoadingPattern;
                }
                if (LoadingType[1] == 1 && sSearchType != "Structured")
                {
                    var Result = XiLinkRepository.GetStructuredOneClicks(OrgID, OCID, sDatabase);
                    if (ResultIn == "Run")
                    {
                        Result.FirstOrDefault().ResultIn = ResultIn;
                    }
                    return null;
                    //return PartialView("_StructuredOneClicksTree", Result);
                }
                else
                {
                    if (LoadingType[0] == 1)
                    {
                        var Report = XiLinkRepository.GetHeadings(OCID, sSearchType, sDatabase, OrgID, iUserID, sOrgName);
                        Report.ResultListDisplayType = LoadingPattern;
                        Report.ReportID = OCID;
                        Report.QueryID = OCID;
                        Report.PreviewType = ResultIn;
                        Report.SearchText = SearchText;
                        if (sSearchType == "Quick")
                        {
                            Report.SearchType = sSearchType;
                        }
                        else if (sSearchType == "Structured")
                        {
                            Report.PreviewType = "Structured";
                        }
                        Report.ResultListDisplayType = LoadingType[0];
                        //else
                        //{
                        //    Report.SearchType = SearchType;
                        //}
                        //Report.BO = BO;
                        //return View(Report);
                        return Report;
                    }
                    else
                    {
                        int? LeadID = 0;
                        VMRunUserQuery UserQuery = new VMRunUserQuery();
                        UserQuery.OrgID = OrgID;
                        UserQuery.UserID = iUserID;
                        UserQuery.database = sDatabase;
                        UserQuery.PageIndex = PageIndex;
                        UserQuery.ReportID = OCID;
                        UserQuery.LeadID = 0;
                        UserQuery.ClassFilter = 0;
                        UserQuery.DateFilter = 0;
                        UserQuery.SearchText = SearchText;
                        //UserQuery.BO = BO;
                        UserQuery.SearchType = sSearchType;
                        UserQuery.ResultListDisplayType = LoadingPattern;
                        VMResultList model = XiLinkRepository.RunUserQuery(UserQuery, iUserID, sOrgName, sDatabase);
                        model.ReportID = OCID;
                        var AutoLoadResultLlist = model.Rows;
                        if (sSearchType == "Structured")
                        {
                            model.PreviewType = "Structured";
                        }
                        else
                        {
                            model.PreviewType = ResultIn;
                        }
                        model.QueryID = OCID;
                        model.SearchText = SearchText;
                        //model.BO = BO;
                        model.SearchType = sSearchType;
                        if (PageIndex >= 2)
                        {
                            //return model;
                        }
                        //return View(model);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [AllowAnonymous]
        public ActionResult GetOneClickResult(jQueryDataTableParamModel param, int i1ClickID, string sGUID, string sSearchText)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (Singleton.Instance.oParentGUID.ContainsKey(sGUID))
                {
                    sGUID = Singleton.Instance.oParentGUID[sGUID];
                }
                string sSessionID = HttpContext.Session.SessionID;
                XIInfraCache oCache = new XIInfraCache();
                XID1Click o1Click = new XID1Click();
                o1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                var oCopy = (XID1Click)o1Click.Clone(o1Click);
                //oCopy.bIsResolveFK = true;
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> nParams = new List<CNV>();
                nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                oCopy.ReplaceFKExpressions(nParams);
                XID1Click oOneClickCopy = new XID1Click();
                if (oCopy.oOneClickParameters.Where(x => x.iType == 10).Count() > 0)
                {
                    oOneClickCopy = (XID1Click)o1Click.Clone(o1Click);
                    oOneClickCopy.ReplaceFKExpressions(nParams);
                    oOneClickCopy.OneClick_Execute();
                }
                oCopy.iSkip = param.iDisplayStart;
                oCopy.iTake = param.iDisplayLength;
                oCopy.NVPairs = param.NVPairs;
                oCopy.SearchType = param.Type;
                oCopy.Fields = param.Fields;
                oCopy.Optrs = param.Optrs;
                oCopy.Values = param.Values;
                oCopy.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                oCopy.sSortDir = Request["sSortDir_0"].ToString();
                oCopy.SearchText = sSearchText;
                if (string.IsNullOrEmpty(sSearchText) && !string.IsNullOrEmpty(param.SearchText))
                {
                    oCopy.SearchText = param.SearchText;
                }
                oCopy.bIsResolveFK = true;
                oCopy.Resolve1Click();
                oCopy.OneClick_Execute();
                List<string[]> oDataTableResult = new List<string[]>();
                var MyLinks = oCopy.MyLinks;
                string sButtons = string.Empty;
                var oSetting = MyLinks.Where(m => m.iType == 0).FirstOrDefault();
                string sCheckBox = string.Empty;
                if (oCopy != null)
                {
                    string sEditBtn = string.Empty;
                    string sCopyBtn = string.Empty;
                    string sDeleteBtn = string.Empty;
                    string sViewbtn = string.Empty;
                    if (oCopy.IsEdit)
                    {
                        sEditBtn = "<input type='button' class='btn btn-theme' value='Edit' onclick ='fncEditBO(this," + oCopy.ID + "," + oCopy.CreateGroupID + "," + oCopy.BOID + "," + oCopy.iCreateXILinkID + ")' />";
                    }
                    if (oCopy.bIsCopy)
                    {
                        sCopyBtn = "<input type='button' class='btn btn-theme' value='Copy' onclick ='fncCopyBO(this," + oCopy.ID + "," + oCopy.CreateGroupID + "," + oCopy.BOID + "," + oCopy.iCreateXILinkID + ")' />";
                    }
                    if (oCopy.bIsDelete)
                    {
                        sDeleteBtn = "<input type='button' class='btn btn-theme' value='Delete' onclick ='fncDeleteBO(this," + oCopy.ID + "," + oCopy.CreateGroupID + "," + oCopy.BOID + "," + oCopy.iCreateXILinkID + ")' />";
                    }
                    if (oCopy.bIsView)
                    {
                        sViewbtn = "<input type='button' class='btn btn-theme' value='View' onclick ='fncViewBO(this," + oCopy.ID + "," + oCopy.CreateGroupID + "," + oCopy.BOID + "," + oCopy.iCreateXILinkID + ")' />";
                    }
                    if (oCopy.bIsCheckbox) // * TO DO THIS IS HARD CODED FOR RECONCILLIATION NEED TO CHANGE IT LATER
                    {
                        sCheckBox = "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this," + oCopy.ID + "," + oCopy.CreateGroupID + "," + oCopy.BOID + "," + oCopy.iCreateXILinkID + ")' />";
                    }
                    sButtons = sEditBtn + sCopyBtn + sDeleteBtn + sViewbtn;
                }
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, oCopy.BOID.ToString());
                foreach (var boi in oCopy.oDataSet)
                {
                    string[] sHiddenArray = oCopy.oOneClickParameters.Where(x => x.iType == 10).Select(x => x.sName).ToArray();
                    foreach (var oBOI in oCopy.oDataSet)
                    {
                        var instanceID = oBOI.Value.Attributes.Where(x => x.Key.ToLower() == oBOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        if (oOneClickCopy != null)
                        {
                            foreach (var item in oOneClickCopy.oDataSet)
                            {
                                var snstanceID = item.Value.Attributes.Where(x => x.Key.ToLower() == oBOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                                string sHiddenAttrName = "Hidden Data";
                                string sHiddenAttrValue = string.Empty;
                                int i = 1;
                                foreach (var sHiddenAttr in sHiddenArray)
                                {
                                    string sAttrValue = string.Empty;
                                    if (item.Value.Attributes.ContainsKey(sHiddenAttr))
                                    {
                                        sAttrValue = item.Value.AttributeI(sHiddenAttr).sValue;
                                    }
                                    sHiddenAttrValue += sHiddenAttr + "_" + sAttrValue;
                                    if (i != sHiddenArray.Count())
                                    {
                                        sHiddenAttrValue = sHiddenAttrValue + ",";
                                    }
                                    i++;
                                }
                                if (snstanceID == instanceID)
                                {
                                    oBOI.Value.Attributes[sHiddenAttrName] = new XIIAttribute { sName = sHiddenAttrName, sValue = sHiddenAttrValue };
                                }
                            }
                        }
                    }
                }
                oCopy.oDataSet.ToList().ForEach(m =>
                {
                    foreach (var Link in MyLinks.Where(n => n.iType == 10))
                    {
                        XIIAttribute oAttrI = new XIIAttribute();
                        oAttrI.sValue = "<input type='button' class='btn btn-theme' value='Post' onclick='XIRun('" + Link.FKiXILinkID + "')' />";
                        m.Value.Attributes["Post"] = oAttrI;
                    }
                    if (!string.IsNullOrEmpty(sButtons))
                    {
                        XIIAttribute oAttrI = new XIIAttribute();
                        oAttrI.sValue = sButtons;
                        m.Value.Attributes["Actions"] = oAttrI;
                    }
                    //m.Value.Attributes.ToList().ForEach(s => s.Value.BOD = oBOD);
                    var dtrow = m.Value.Attributes.Values;
                    string[] strarray = dtrow.Select(n => n.sValue).ToArray();
                    if (!string.IsNullOrEmpty(sCheckBox))
                    {
                        string[] newarray = new string[strarray.Length + 1];
                        newarray[0] = sCheckBox;
                        for (int i = 0; i < strarray.Length; i++)
                        {
                            newarray[i + 1] = strarray[i];
                        }
                        oDataTableResult.Add(newarray);
                    }
                    else
                    {
                        oDataTableResult.Add(strarray);
                    }
                });
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = oCopy.iTotalRecords,
                    iTotalDisplayRecords = oCopy.iTotalRecords,
                    aaData = oDataTableResult
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        [AllowAnonymous]
        public ActionResult GetReportResult(jQueryDataTableParamModel param, int ReportID, string SearchText, string SearchType, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sSessionID = HttpContext.Session.SessionID;
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<cNameValuePairs> nParams = new List<cNameValuePairs>();
                nParams = oGUIDParams.NMyInstance.Select(m => new cNameValuePairs { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                string sCurrentGuestUser = string.Empty;
                if (SessionManager.UserUniqueID != null)
                {
                    sCurrentGuestUser = SessionManager.UserUniqueID;
                }
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                VMQuickSearch Search = new VMQuickSearch();
                Search.ReportID = ReportID;
                Search.UserID = iUserID;
                Search.OrgID = OrgID;
                Search.database = sDatabase;
                Search.SearchType = SearchType;
                Search.SearchText = SearchText;
                if (oUser.Role != null)
                {
                    Search.Role = oUser.Role.sRoleName;
                }
                Search.BO = BO;
                Search.sGUID = sGUID;
                var result = XiLinkRepository.GetReportResult(param, Search, iUserID, sOrgName, sDatabase, sCurrentGuestUser, nParams);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult RunUserQuery(VMRunUserQuery QValues)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                QValues.OrgID = OrgID;
                QValues.UserID = iUserID;
                QValues.database = sDatabase;
                QValues.LeadID = 0;
                QValues.ClassFilter = 0;
                QValues.DateFilter = 0;
                VMResultList model = XiLinkRepository.RunUserQuery(QValues, iUserID, sOrgName, sDatabase);
                ViewBag.queryid = QValues.ReportID;
                var AutoLoadResultLlist = model.Rows;
                if (QValues.PageIndex >= 2 || QValues.SearchType == "FilterSearch" || QValues.SearchType == "NaturalSearch" || QValues.SearchType == "Quick")
                {
                    return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult ReportResult(int QueryID, int PageIndex, string ResultIn, string SearchText, string SearchType)
        {
            OCID = QueryID;
            sSearchType = SearchType;
            sSearchText = SearchText;
            var Result = OneClickResult(null);
            return View("OneClickResults", Result);
        }

        public ActionResult GetPieChart(int ReportID, string ResultIn, string Query)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                Chart.Query = Query;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                GraphData Graph = new GraphData();
                Graph = XiLinkRepository.GetPieChart(Chart, iUserID, sOrgName, sDatabase);
                Graph.Type = "Run";
                Graph.SectionName = "";
                Graph.ClassDDL = new List<VMDropDown>();
                Graph.DateDDL = new List<VMDropDown>();
                return PartialView("_KPIPieChart", Graph);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public ActionResult GetBarChart(int ReportID, string ResultIn, string Query)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                //ReportID = OCID;
                VMChart Charts = new VMChart();
                Charts.ReportID = ReportID;
                Charts.OrgID = OrgID;
                Charts.UserID = iUserID;
                Charts.Database = sDatabase;
                Charts.Query = Query;
                LineGraph graph = XiLinkRepository.GetBarChart(Charts, sDatabase, iUserID, sOrgName);
                graph.ReportID = ReportID;
                graph.Type = "Run";
                graph.SectionName = "";
                graph.ClassDDL = new List<VMDropDown>();
                graph.DateDDL = new List<VMDropDown>();
                return PartialView("_KPIBarChart", graph);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public ActionResult GetLineGraph(int ReportID, string ResultIn, string Query)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                //ReportID = OCID;
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                Chart.Query = Query;
                LineGraph graph = XiLinkRepository.GetLineChart(Chart, iUserID, sOrgName, sDatabase);
                graph.ReportID = ReportID;
                graph.Type = "Run";
                graph.SectionName = "";
                graph.ClassDDL = new List<VMDropDown>();
                graph.DateDDL = new List<VMDropDown>();
                return PartialView("_LineGraph", graph);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public ActionResult GetFormData(string sBOName, string sGroupName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                if (sBOName != null && sGroupName != null)
                {
                    var oBODisplay = oXiAPI.GetFormData(sBOName, sGroupName, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                    return PartialView("_CreateForm", oBODisplay);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetAutoCompleteData(int i1ClickID, string sAutoText, string sField)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOID.ToString());
                o1ClickC.BOD = oBOD;

                o1ClickC.SearchText = sAutoText;
                //o1ClickC.bIsResolveFK = true;
                o1ClickC.Get_1ClickHeadings();
                //o1ClickC.OneClick_Execute();
                return PartialView("~/Views/XIComponents/_oneclickcomponent.cshtml", o1ClickC);

                //OCID = i1ClickID;
                //sSearchText = sAutoText;
                //var Result = OneClickResult(null);
                //ViewBag.Field = sField;
                //Result.SearchType = "Quick";
                //return PartialView("_AutoComplete", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetLabelData(int iBOID = 0, int iInstanceID = 0, int i1ClickID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                var Result = XiLinkRepository.GetLabelData(iBOID, iInstanceID, i1ClickID, iUserID, sOrgName, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        #endregion 1ClickResult

        #region Popup

        [AllowAnonymous]
        public ActionResult GetPopupORDialog(int XiLinkID, string sGUID, string sNewGuid, string BO = "", string sID = "", int PRDID = 0, int BODID = 0, string ActRec = "", string EnumReconciliation = "", string Type = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            string sParGUID = string.Empty;
            try
            {
                int ID = 0;
                if (int.TryParse(sID, out ID))
                { }
                XILink oXiLink = null;
                string sOrgName = SessionManager.OrganisationName;
                string sSessionID = HttpContext.Session.SessionID;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                if (XiLinkID > 0)
                {
                    oXiLink = GetXiLinkDetails(XiLinkID, sGUID);
                    XIRun(oXiLink, sGUID);
                }
                else
                {
                    DialogID = PRDID;
                }
                if (string.IsNullOrEmpty(BO) && BODID != 0)
                {
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BODID.ToString());
                    BO = oBOD.Name;
                }

                if (PopupID > 0)
                {
                    //var PopDetails = XiLinkRepository.GetPopupLayoutDetails(PopupID, ID, BOID, sGUID, iUserID, sOrgName, sDatabase);
                    //PopDetails.sNewGUID = sGUID;
                    //return PartialView("Popup", PopDetails);
                }
                else if (DialogID > 0)
                {
                    XIDDialog oDialog = null;
                    oDialog = (XIDDialog)oCache.GetObjectFromCache(XIConstant.CacheDialog, null, DialogID.ToString()); //oXID.Get_DialogDefinition(DialogID.ToString());
                    XIDLayout oLayout = new XIDLayout();
                    oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, oDialog.LayoutID.ToString()); //oXID.Get_LayoutDefinition(null, oDialog.LayoutID.ToString());
                    //var sSessionID = HttpContext.Session.SessionID;
                    if (!string.IsNullOrEmpty(sGUID))
                    {
                        oCache.sSessionID = sSessionID;
                        var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                        sParGUID = ParentGUID;
                    }
                    else
                    {
                        sParGUID = sNewGuid;
                    }
                    oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|ActRec}", ActRec, null, null);
                    oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|EnumReconciliations_T.id}", EnumReconciliation, null, null);
                    oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|EnumReconType}", Type, null, null);
                    //sParGUID = sNewGuid;
                    oCache.Set_ParamVal(sSessionID, sParGUID, null, "XICache", "XICache", null, null);
                    if (!string.IsNullOrEmpty(BO))
                    {
                        oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|ActiveBO}", BO.ToString(), null, null);
                        oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|sBOName}", BO.ToString(), null, null);
                    }
                    if (ID > 0)
                    {
                        oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|" + BO + ".id}", ID.ToString(), "autoset", null);
                        oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|iInstanceID}", ID.ToString(), null, null);
                    }

                    oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|BODID}", BODID.ToString(), null, null);
                    if (BO.ToLower() == "riskfactors")
                    {
                        oCache.Set_ParamVal(sSessionID, sParGUID, null, "{XIP|FKsQuoteGUID}", sID, null, null);
                    }
                    // oCache.Get_ParamVal(sSessionID, sParGUID, null, "|XIParent");

                    CResult oCResult = oCache.MergeXILinkParameters(oXiLink, sParGUID, null, sSessionID);
                    oLayout.sNewGUID = sParGUID;
                    oLayout.LayoutMappings = oLayout.LayoutMappings.Where(m => m.PopupID == oDialog.ID).ToList();
                    if (oLayout.XiParameterID > 0)
                    {
                        oCache.AddParamsToGUID(oLayout.XiParameterID, sParGUID);
                    }
                    if (!string.IsNullOrEmpty(sGUID) && !string.IsNullOrEmpty(sParGUID))
                    {
                        XIInfraCache oCache = new XIInfraCache();
                        oCache.Init_RuntimeParamSet(sSessionID, sNewGuid, sGUID, null);
                    }
                    return PartialView("Dialog", oLayout);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        [AllowAnonymous]
        public ActionResult GetLayoutDetails(int iLayoutID = 0, string sParentGUID = "", string sSection = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, iLayoutID.ToString());
                //var Result = XiLinkRepository.GetLayoutDetails(iLayoutID, sParentGUID, sSection, sDatabase);
                return PartialView("Dialog", oLayout);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetPopupContent(VMPopupLayout model)
        {
            return PartialView("_PopupContent", model);
        }

        [AllowAnonymous]
        public ActionResult ShowPopupContent(int ID)
        {
            return PartialView("MultiPopupContent");
        }

        [AllowAnonymous]
        public ActionResult GetDialogContent(XIDLayout model)
        {
            var code = model.LayoutCode.Replace(@"\", string.Empty);
            return PartialView("_DialogContent", model);
        }

        [AllowAnonymous]
        public ActionResult GetLayoutData(XIDLayout model)
        {
            //Check for parentguid
            if (model.sGUID != null)
            {
                string sSessionID = HttpContext.Session.SessionID;
                //SessionManager.sGUID = model.sGUID;
                var sParentGUID = oCache.Get_ParamVal(sSessionID, model.sGUID, null, "{XIP|ParentGUID}");
                if (!string.IsNullOrEmpty(sParentGUID))
                {
                    //model.sGUID = sParentGUID;
                }
                else
                {
                    sParentGUID = oCache.Set_ParamVal(sSessionID, model.sGUID, null, "{XIP|ParentGUID}", model.sGUID.ToString(), null, null);
                }
                // model.sGUID = sParentGUID;
            }
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PartialView("_LayoutContent", model);
                //XIInstanceBase oData = new XIInstanceBase();
                //oData.oContent[XIConstant.ContentLayout] = model;
                //dynamic oMergedData = MergeHTMLRecurrsive(oData);
                //var LayoutData = oMergedData.oContent[XIConstant.ContentLayout];
                //return PartialView("_LayoutContent", LayoutData);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [AllowAnonymous]
        public ActionResult GetLayoutContent(XIDLayout model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInstanceBase oData = new XIInstanceBase();
                oData.oContent[XIConstant.ContentLayout] = model;
                dynamic oMergedData = MergeHTMLRecurrsive(oData);
                var LayoutData = oMergedData.oContent[XIConstant.ContentLayout];
                //return Json(oMergedData, JsonRequestBehavior.AllowGet);
                return PartialView("_LayoutData", LayoutData);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            //XIDLayout oLayout = new XIDLayout();
            ////var oLayDef = oXID.Get_LayoutDefinition(null, iLayoutID.ToString());
            //var oLayDef = model.Load();
            //if (oLayDef.bOK && oLayDef.oResult != null)
            //{
            //    oLayout = (XIDLayout)((XIInstanceBase)oLayDef.oResult).oContent[XIConstant.ContentLayout];
            //}
            ////Check for parentguid
            //if (model.sGUID != null)
            //{
            //    string sSessionID = HttpContext.Session.SessionID;
            //    //SessionManager.sGUID = model.sGUID;
            //    var sParentGUID = oCache.Get_ParamVal(sSessionID, model.sGUID, null, "{XIP|ParentGUID}");
            //    if (!string.IsNullOrEmpty(sParentGUID))
            //    {
            //        //model.sGUID = sParentGUID;
            //    }
            //    else
            //    {
            //        sParentGUID = oCache.Set_ParamVal(sSessionID, model.sGUID, null, "{XIP|ParentGUID}", model.sGUID.ToString(), null, null);
            //    }
            //    // model.sGUID = sParentGUID;
            //}

        }

        [AllowAnonymous]
        public ActionResult GetStepLayoutContent(cLayouts model)
        {
            return PartialView("_StepLayoutContent", model);
        }

        public ActionResult GetMenus()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Menus = XiLinkRepository.GetMenus(string.Empty, iUserID, OrgID, sDatabase);
                return Json(Menus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetDialog(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Res = XiLinkRepository.GetDialog(ID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetPopupDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Res = XiLinkRepository.GetPopupDetailsByID(ID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult GetInlineDetails(int ID)
        //{
        //    try
        //    {
        //        int OrgID = OrgID;
        //        string database = sDatabase;
        //        var Res = XiLinkRepository.GetInlineDetails(ID, database);
        //        return Json(Res, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public ActionResult GetChildForMenu(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<RightMenuTrees> Models = XiLinkRepository.GetChildForMenu(ID, OrgID, sDatabase);
                return Json(Models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLeadPopupOLD(int LeadID, int XiLinkID, string PopType, int ClientID = 0, int StageID = 0, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var XiLink = GetXiLinkDetails(XiLinkID, null);
                //XIRun(XiLink);
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = LeadID;
                popup.PopupID = PopupID;
                popup.UserName = oUser.Role.sRoleName;
                popup.StageID = StageID;
                popup.PopType = PopType;
                popup.ClientID = ClientID;
                popup.RowID = ID;
                popup.DailogID = ID;
                var Res = XiLinkRepository.GetPopupDetails(PopupID, sDatabase);
                if (Res.IsLeftMenu)
                {
                    popup.IsLeftMenu = true;
                }
                else
                {
                    popup.IsLeftMenu = false;
                }
                popup.LayoutType = Res.LayoutID;
                return View("LeadPopup", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLeadPopupLeftContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                VMLeadPopupLeft model = XiLinkRepository.GetLeadPopupLeftContent(Popup, sDatabase, OrgID, iUserID, sOrgName);
                return PartialView("_LeadPopupLeftContent", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult LeadContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = Popup.LeadID;
                popup.PopupID = Popup.ReportID;
                popup.Tabs = XiLinkRepository.GetAllTabs(Popup.ReportID, Popup.PopupID, sDatabase);
                popup.ClientID = Popup.ClientID;
                popup.PopType = Popup.PopType;
                popup.StageID = Popup.StageID;
                popup.RowID = Popup.RowID;
                popup.LayoutType = Popup.LayoutType;
                return PartialView("_LeadPopupContent", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CallAction(VMLeadActions model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var result = XiLinkRepository.CallAction(model, OrgID, iUserID, sOrgName, sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveLeadTransaction(Stages model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var result = XiLinkRepository.SaveLeadTransaction(model, OrgID, sDatabase, iUserID, sOrgName);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetNextStages(int LeadID, int StageID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var result = XiLinkRepository.GetNextStages(LeadID, StageID, OrgID, iUserID, sOrgName, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult SendClientRequest(int LeadID, string Email, int OrgID, string OrgName, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //LeadRepository.SendRegisterMail(Email, "Register", OrgID, sDatabase, ClassID);
                WalletRequests Request = new WalletRequests();
                Request.LeadID = LeadID;
                Request.EmailID = Email;
                Request.OrganizationID = OrgID;
                Request.IsActivated = false;
                Request.FKiLeadClassID = 0;
                var Res = XiLinkRepository.SaveWalletRequest(Request, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetTabContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                if (Popup.Name == "Reminders")
                {
                    Reminders model = new Reminders();
                    model.ReportID = Popup.ReportID;
                    model.LeadID = Popup.LeadID;
                    return PartialView("_LeadReminder", model);
                }
                else if (Popup.Name == "Sent Documents")
                {
                    return PartialView("_TabUploadDocument", Popup.LeadID);
                }
                else if (Popup.Name == "Received Documents")
                {
                    return PartialView("_TabClientDocument", Popup.LeadID);
                }
                else
                {
                    List<VMQueryPreview> content = XiLinkRepository.GetTabContent(Popup, iUserID, OrgID, sDatabase, sOrgName);
                    if (content.Count() > 0 && content != null)
                    {
                        content.ToList().ForEach(m => m.PopType = Popup.PopType);
                        content.ToList().ForEach(m => m.StageID = Popup.StageID);
                        VMViewPopup popup = new VMViewPopup();
                        popup.TabID = Popup.TabID;
                        popup.ClassID = Popup.ClassID;
                        content.FirstOrDefault().popup = popup;
                        content.FirstOrDefault().LeadID = Popup.LeadID;
                        return PartialView("_LeadPopupTabContent", content);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult CreateData(List<VMFormData> FormValues, string BOName, int iInstanceID = 0)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    try
        //    {
        //        VMSaveInlineEdit SaveData = new VMSaveInlineEdit();
        //        SaveData.FormValues = FormValues;
        //        SaveData.iInstanceID = iInstanceID;
        //        int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //        oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //        var Response = BusinessObjectsRepository.CreateFormData(SaveData, sDatabase, OrgID, iUserID, BOName, sOrgName);
        //        return Json(Response, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json("Failure", JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult CliamData()
        {
            return View();
        }
        public ActionResult ClaimConvictionData()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult CliamTermData(List<VMFormData> FormValues)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    try
        //    { 
        //        int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //        oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //        var Response = XiLinkRepository.ClaimTerms(FormValues, sDatabase, iUserID, sOrgName);
        //        return Json(Response, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json("Failure", JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpPost]
        //public ActionResult CliamConvictionData(List<VMFormData> FormValues)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    try
        //    {
        //        int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //        oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //        var Response = XiLinkRepository.CliamConvictionData(FormValues, sDatabase, iUserID, sOrgName);
        //        return Json(Response, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json("Failure", JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveBO(List<XIIAttribute> Attributes, string sGUID, string sContext, string sBOName)
        {
            List<XIIBO> oBOIList = new List<XIIBO>();
            //string str = Singleton.Instance.oParentGUID[sGUID];
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIBO Response = new XIIBO();
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                oBOI.BOD = oBOD;
                Dictionary<string, XIIAttribute> oAttrs = new Dictionary<string, XIIAttribute>();
                oAttrs = Attributes.ToDictionary(x => x.sName.ToLower(), x => x);
                oBOI.Attributes = oAttrs;
                string sActiveBO = string.Empty;
                string sActiveFK = string.Empty;
                string sSessionID = HttpContext.Session.SessionID;
                var ISS = oCache.Get_ParamVal(sSessionID, sGUID, sContext, "|XIParent");
                XICacheInstance parentparams = new XICacheInstance();
                if (!string.IsNullOrEmpty(ISS))
                {
                    parentparams = oCache.GetAllParamsUnderGUID(sSessionID, ISS, sContext);
                }
                else
                {
                    parentparams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, sContext);
                }
                sActiveBO = parentparams.NMyInstance.Where(m => m.Key == "{XIP|ActiveBO}").Select(m => m.Value.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sActiveBO))
                {
                    sActiveFK = oBOI.BOD.Attributes.Values.Where(m => m.FKTableName == sActiveBO).Select(m => m.Name).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(sActiveFK))
                {
                    var FKValue = parentparams.NMyInstance.Where(m => m.Key == "{XIP|" + sActiveBO + ".id}".ToLower()).Select(m => m.Value.sValue).FirstOrDefault();
                    if (string.IsNullOrEmpty(FKValue))
                    {
                        FKValue = parentparams.NMyInstance.Where(m => m.Key == "{XIP|" + sActiveFK + "}".ToLower()).Select(m => m.Value.sValue).FirstOrDefault();
                    }
                    var ColExists = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault();
                    if (ColExists == null)
                    {

                        oBOI.Attributes[sActiveFK.ToLower()] = new XIIAttribute { sName = sActiveFK, sValue = FKValue };
                        oBOI.Attributes[sActiveFK.ToLower()].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().sValue = FKValue;
                        oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().bDirty = true;
                    }
                }
                foreach (var itesm in parentparams.NMyInstance)
                {
                    if (itesm.Value.sType == "autoset")
                    {
                        var ColExists = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault();
                        if (ColExists == null)
                        {
                            oBOI.Attributes[itesm.Key.ToLower()] = new XIIAttribute { sName = itesm.Key.ToLower(), sValue = itesm.Value.sValue };
                            oBOI.Attributes[itesm.Key.ToLower()].bDirty = true;
                        }
                        else
                        {
                            oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().sValue = itesm.Value.sValue;
                            oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().bDirty = true;
                        }
                    }
                }
                //var Response = oBOI.Save(oBOI);
                var oResult = oBOI.Save(oBOI);
                if (oResult.bOK && oResult.oResult != null)
                {
                    Response.oScriptErrors = new Dictionary<string, string>();
                    Response = (XIIBO)oResult.oResult;
                    if (Response.BOD.Scripts.Values.Where(m => m.IsSuccess == false).Count() >= 0)
                    {
                        foreach (var script in Response.BOD.Scripts.Values)
                        {
                            if (!script.IsSuccess)
                            {
                                string sFieldName = script.sFieldName;
                                foreach (var scriptresult in script.ScriptResults)
                                {
                                    if (scriptresult.iType == 30)
                                    {
                                        string sUserError = scriptresult.sUserError;
                                        string sKey = sFieldName;

                                        Response.oScriptErrors[sKey] = sUserError;
                                    }
                                }
                            }
                        }
                    }
                    oBOIList.Add(Response);
                }
                else
                {
                    oBOI.sErrorMessage = "Failure";
                    oBOIList.Add(oBOI);
                }
                if (oBOI.BOD.Name.ToLower() == "driver_t")
                {
                    string sPrimaryKey = oBOI.BOD.sPrimaryKey;
                    var iDriverID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}", "Driver_T", null, null);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|Driver_T.id}", iDriverID, null, null);
                }

                else if (oBOI.BOD.Name.ToLower() == "ACReconcilliation_T".ToLower()) // NOTE: Setting Primarykey ID in Cache this is Reconcilliations Form
                {
                    string iPKID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ACReconcilliation_T.id}", iPKID, null, new List<CNV>());
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|refAccountCategory}", Attributes.Where(m => m.sName.ToLower() == "refAccountCategory".ToLower()).Select(m => m.sValue).FirstOrDefault(), null, new List<CNV>());
                }
                //string svalue = oCache.Get_ParamVal(sSessionID, sGUID, null, "ACReconcilliationID");
                List<XIIBO> oResponse = new List<XIIBO>();
                foreach (var oInstance in oBOIList)
                {
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = new XIDBO();
                    //oBO.BOD.Scripts = oInstance.BOD.Scripts;
                    oBO.sBOName = oInstance.BOD.Name;
                    oBO.BOD.sPrimaryKey = oInstance.BOD.sPrimaryKey;
                    var sPKValue = oInstance.Attributes.Where(x => x.Key.ToLower() == oInstance.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    oBO.iInstanceID = Convert.ToInt32(sPKValue);
                    //oBO.Attributes.Where(x => x.Key.ToLower() == oInstance.BOD.sPrimaryKey.ToLower()).ToList().ForEach(m => m.Value.sValue = sPKValue);
                    oBO.BOD.BOID = oInstance.BOD.BOID;
                    oBO.sErrorMessage = oInstance.sErrorMessage;
                    oBO.oScriptErrors = oInstance.oScriptErrors;
                    oResponse.Add(oBO);
                }

                //var Responsea = oXiAPI.SaveFormData(Attributes, sGUID, sContext, sDatabase, iUserID, sOrgName);
                return Json(oResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveGrid(List<XIIAttribute> Attributes, string sGUID, string sContext, string sBOName, List<XIIBO> oBOIInstance)
        {
            int iUserID = 0; string sDatabase = SessionManager.CoreDatabase;
            if (SessionManager.UserID > 0)
            {
                iUserID = SessionManager.UserID;
            }
            try
            {
                List<XIIAttribute> oAttributes = new List<XIIAttribute>();
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                oBOI.BOD = oBOD;
                var attr = Attributes.FirstOrDefault().sName.Split('_')[0];
                var oRowData = Attributes.Where(x => x.sName.StartsWith(attr)).ToList();
                XIDefinitionBase oDefBase = new XIDefinitionBase();
                oAttributes = (List<XIIAttribute>)(oDefBase.Clone(oRowData));
                foreach (var item in oAttributes)
                {
                    item.sName = item.sName.Replace(attr + "_", "");
                }
                //Check For Empty row
                bool bIsSave = true;
                var Creatable = oAttributes.Where(s => !string.IsNullOrEmpty(s.sValue)).ToList();
                if (Creatable != null && Creatable.Count() > 0)
                {

                }
                else
                {
                    bIsSave = false;
                }

                oBOI.Attributes = oAttributes.ToDictionary(x => x.sName.ToLower(), x => x);
                string sActiveBO = string.Empty;
                string sActiveFK = string.Empty;
                string sSessionID = HttpContext.Session.SessionID;
                var ISS = oCache.Get_ParamVal(sSessionID, sGUID, sContext, "|XIParent");
                XICacheInstance parentparams = new XICacheInstance();
                if (!string.IsNullOrEmpty(ISS))
                {
                    parentparams = oCache.GetAllParamsUnderGUID(sSessionID, ISS, sContext);
                }
                else
                {
                    parentparams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, sContext);
                }
                sActiveBO = parentparams.NMyInstance.Where(m => m.Key == "{XIP|ActiveBO}").Select(m => m.Value.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sActiveBO))
                {
                    sActiveFK = oBOI.BOD.Attributes.Values.Where(m => m.FKTableName == sActiveBO).Select(m => m.Name).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(sActiveFK))
                {
                    var FKValue = parentparams.NMyInstance.Where(m => m.Key == "{XIP|" + sActiveBO + ".id}").Select(m => m.Value.sValue).FirstOrDefault();
                    var ColExists = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault();
                    if (ColExists == null)
                    {
                        oBOI.Attributes[sActiveFK.ToLower()] = new XIIAttribute { sName = sActiveFK, sValue = FKValue };
                        oBOI.Attributes[sActiveFK.ToLower()].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().sValue = FKValue;
                        oBOI.Attributes.Values.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().bDirty = true;
                    }
                }
                foreach (var itesm in parentparams.NMyInstance)
                {
                    if (itesm.Value.sType == "autoset")
                    {
                        var ColExists = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault();
                        if (ColExists == null)
                        {
                            oBOI.Attributes[itesm.Key.ToLower()] = new XIIAttribute { sName = itesm.Key.ToLower(), sValue = itesm.Value.sValue };
                            oBOI.Attributes[itesm.Key.ToLower()].bDirty = true;
                        }
                        else
                        {
                            oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().sValue = itesm.Value.sValue;
                            oBOI.Attributes.Values.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().bDirty = true;
                        }
                    }
                }
                var oRes = oBOI;
                if (bIsSave)
                {
                    if (oBOIInstance == null)
                    {
                        oBOIInstance = new List<XIIBO>();
                    }
                    if (oBOI.Attributes.ContainsKey("StatusScript".ToLower()))
                    {
                        if (!string.IsNullOrEmpty(oBOI.Attributes["StatusScript".ToLower()].sValue))
                        {
                            string sScript = oBOI.Attributes["StatusScript".ToLower()].sValue;
                            if (!string.IsNullOrEmpty(sScript))
                            {
                                //string sGUID = Guid.NewGuid().ToString();
                                string sReturnValue = string.Empty;
                                CResult oCR = new CResult();
                                XIDScript oXIScript = new XIDScript();
                                oXIScript.sScript = sScript.ToString();
                                oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    sReturnValue = (string)oCR.oResult;
                                }
                            }
                        }
                    }

                    var oResult = oBOI.Save(oBOI);
                    if (oResult.bOK && oResult.oResult != null)
                    {
                        oRes.oScriptErrors = new Dictionary<string, string>();
                        oRes = (XIIBO)oResult.oResult;
                        if (oRes.BOD.Scripts.Values.Where(m => m.IsSuccess == false).Count() >= 0)
                        {
                            foreach (var script in oRes.BOD.Scripts.Values)
                            {
                                if (!script.IsSuccess)
                                {
                                    string sFieldName = script.sFieldName;
                                    foreach (var scriptresult in script.ScriptResults)
                                    {
                                        if (scriptresult.iType == 30)
                                        {
                                            string sUserError = scriptresult.sUserError;
                                            string sKey = attr + "_" + sFieldName;
                                            oRes.oScriptErrors[sKey] = sUserError;
                                        }
                                    }
                                }
                            }
                        }
                        oBOIInstance.Add(oRes);
                    }
                    else
                    {
                        oBOI.sErrorMessage = "Failure";
                        oBOIInstance.Add(oBOI);
                    }
                }
                //var Response = oBOI.Save(oBOI);
                Attributes.RemoveAll(t => t.sName.StartsWith(attr + "_"));
                if (Attributes != null && Attributes.Count() > 0)
                {
                    SaveGrid(Attributes, sGUID, sContext, sBOName, oBOIInstance);
                }
                //var Response = oXiAPI.SaveFormData(oBOInstance, sGUID, sContext, sDatabase, iUserID, sOrgName);
                List<XIIBO> oResponse = new List<XIIBO>();
                foreach (var oInstance in oBOIInstance)
                {
                    XIIBO oBO = new XIIBO();
                    oBO.BOD = new XIDBO();
                    // oBO.BOD.Scripts = oInstance.BOD.Scripts;
                    oBO.sBOName = oInstance.BOD.Name;
                    oBO.BOD.sPrimaryKey = oInstance.BOD.sPrimaryKey;
                    var sPKValue = oInstance.Attributes.Where(x => x.Key.ToLower() == oInstance.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    oBO.iInstanceID = Convert.ToInt32(sPKValue);
                    //oBO.Attributes.Where(x => x.Key.ToLower() == oInstance.BOD.sPrimaryKey.ToLower()).ToList().ForEach(m => m.Value.sValue = sPKValue);
                    oBO.BOD.BOID = oInstance.BOD.BOID;
                    oBO.sErrorMessage = oInstance.sErrorMessage;
                    oBO.oScriptErrors = oInstance.oScriptErrors;
                    oResponse.Add(oBO);
                }
                return Json(oResponse, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Popup

        #region UserDialogs

        [HttpPost]
        public ActionResult SaveUserDialog(int QueryID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Result = XiLinkRepository.SaveUserDialog(QueryID, iUserID, OrgID, sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetUserDialogs(int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Dialogs = XiLinkRepository.GetUserDialogs(iUserID, OrgID, sDatabase);
                //Session["IsLogin"] = false;
                return Json(Dialogs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion UserDialogs

        #region EditableGrid

        public VMResultList OneClickResultForEditableGrid()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string ResultIn = "Inline";
                string SearchText = null;
                int PageIndex = 1;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var LoadingType = XiLinkRepository.GetLoadingType(OCID, sDatabase);
                if (LoadingPattern != -1)
                {
                    LoadingType[0] = LoadingPattern;
                }
                if (LoadingType[1] == 1 && sSearchType != "Structured")
                {
                    var Result = XiLinkRepository.GetStructuredOneClicks(OrgID, OCID, sDatabase);
                    if (ResultIn == "Run")
                    {
                        Result.FirstOrDefault().ResultIn = ResultIn;
                    }
                    return null;
                    //return PartialView("_StructuredOneClicksTree", Result);
                }
                else
                {
                    if (LoadingType[0] == 1)
                    {
                        var Report = XiLinkRepository.GetHeadings(OCID, sSearchType, sDatabase, OrgID, iUserID, sOrgName);
                        Report.ResultListDisplayType = LoadingPattern;
                        Report.ReportID = OCID;
                        Report.QueryID = OCID;
                        Report.PreviewType = ResultIn;
                        Report.SearchText = SearchText;
                        if (sSearchType == "Quick")
                        {
                            Report.SearchType = sSearchType;
                        }
                        else if (sSearchType == "Structured")
                        {
                            Report.PreviewType = "Structured";
                        }
                        Report.ResultListDisplayType = LoadingType[0];
                        //else
                        //{
                        //    Report.SearchType = SearchType;
                        //}
                        //Report.BO = BO;
                        //return View(Report);
                        return Report;
                    }
                    else
                    {
                        int? LeadID = 0;
                        VMRunUserQuery UserQuery = new VMRunUserQuery();
                        UserQuery.OrgID = OrgID;
                        UserQuery.UserID = iUserID;
                        UserQuery.database = sDatabase;
                        UserQuery.PageIndex = PageIndex;
                        UserQuery.ReportID = OCID;
                        UserQuery.LeadID = 0;
                        UserQuery.ClassFilter = 0;
                        UserQuery.DateFilter = 0;
                        UserQuery.SearchText = SearchText;
                        //UserQuery.BO = BO;
                        UserQuery.SearchType = sSearchType;
                        UserQuery.ResultListDisplayType = LoadingPattern;
                        VMResultList model = XiLinkRepository.RunUserQuery(UserQuery, iUserID, sOrgName, sDatabase);
                        model.ReportID = OCID;
                        var AutoLoadResultLlist = model.Rows;
                        if (sSearchType == "Structured")
                        {
                            model.PreviewType = "Structured";
                        }
                        else
                        {
                            model.PreviewType = ResultIn;
                        }
                        model.QueryID = OCID;
                        model.SearchText = SearchText;
                        //model.BO = BO;
                        model.SearchType = sSearchType;
                        if (PageIndex >= 2)
                        {
                            //return model;
                        }
                        //return View(model);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult GetReportResult(jQueryDataTableParamModel param, int ReportID)
        //{
        //    try
        //    {
        //        int OrgID = OrgID;
        //        int UserID = iUserID
        //        string database = sDatabase;
        //        param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
        //        param.sSortDir = Request["sSortDir_0"].ToString();
        //        VMQuickSearch Search = new VMQuickSearch();
        //        Search.ReportID = ReportID;
        //        Search.UserID = UserID;
        //        Search.OrgID = OrgID;
        //        Search.database = database;
        //        Search.SearchType = SearchType;
        //        Search.Role = Util.GetRoleName();
        //        Search.BO = BO;
        //        var result = XiLinkRepository.GetReportResult(param, Search);
        //        return Json(new
        //        {
        //            sEcho = result.sEcho,
        //            iTotalRecords = result.iTotalRecords,
        //            iTotalDisplayRecords = result.iTotalDisplayRecords,
        //            aaData = result.aaData
        //        },
        //        JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return null;
        //    }
        //}

        #endregion EditableGrid

        #region UploadImage
        //[HttpPost]  

        //public ActionResult SaveFiles(int ID, int BOFieldID, string sInstanceID, List<HttpPostedFileBase> UploadImage)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //    oUser.UserID = iUserID;
        //    oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
        //    int OrgID = oUser.FKiOrganisationID;
        //    try
        //    {
        //        VMCustomResponse Docs = new VMCustomResponse();
        //        if (UploadImage != null)
        //        {
        //            Docs = XiLinkRepository.SaveFiles(ID, BOFieldID, OrgID, UploadImage, iUserID, sOrgName, sDatabase, sInstanceID);
        //        }

        //        return Content(Docs.sID.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult SaveDocx(int ID, int BOFieldID, string sInstanceID, List<HttpPostedFileBase> UploadFile)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            int OrgID = oUser.FKiOrganisationID;
            try
            {
                VMCustomResponse SaveDocs = new VMCustomResponse();
                if (UploadFile != null)
                {
                    SaveDocs = XiLinkRepository.SaveFiles(ID, BOFieldID, OrgID, UploadFile, iUserID, sOrgName, sDatabase, sInstanceID);

                }
                // return Json(true, "text/html");
                return Content(SaveDocs.sID.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                //return Json(false, "text/html");
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion UploadImage

        #region Delete attribute image
        public ActionResult DeleteAttrImage(int ImgID, int BOFieldID, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iStatus = XiLinkRepository.DeleteAttrImage(sDatabase, ImgID, BOFieldID, LeadID);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(false, "text/html");
            }
        }


        #endregion Delete attribute image

        #region LayoutPanel

        [HttpPost]
        public ActionResult GetCacheParameterValue(string sParamName, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sSessionID = HttpContext.Session.SessionID;
                //sUID = "3";
                var sParamVal = oCache.Get_ParamVal(sSessionID, sGUID, null, sParamName);
                if (sParamVal != null)
                {
                    return Json(sParamVal, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Value", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion LayoutPanel

        #region ListActions

        public ActionResult ListHover(int ID = 0, int BOID = 0, string ColumnName = null)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                XIIBO oBOI = new XIIBO();
                var Result = oBOI.Get_FKHover(ID, BOID, ColumnName);
                //var Result = oXiAPI.GetListHover(ID, BOID, BOName, ColumnName, iUserID, sOrgName, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        #endregion ListActions

        #region QuestionSet

        [AllowAnonymous]
        public XIIQS GetQSInstance(int iQSDID, string sGUID, string sMode, int iBODID, int iBOIID)
        {
            XIIXI oIXI = new XIIXI();
            XIInfraCache xifCache = new XIInfraCache();
            string sDatabase = SessionManager.CoreDatabase;
            string sOrgName = SessionManager.OrganisationName;
            int iUserID = 0;
            if (SessionManager.UserID > 0)
            {
                iUserID = SessionManager.UserID;
            }
            string sCurrentGuestUser = string.Empty;
            if (SessionManager.UserUniqueID != null)
            {
                sCurrentGuestUser = SessionManager.UserUniqueID;
            }
            //Get Question Set Object
            XIIQS oQSI = new XIIQS();
            XIDQS oQSD = (XIDQS)xifCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString());
            //var oQSD = oDXI.Get_QSDefinition(null, iQSDID);
            CResult oCR = oIXI.CreateQSI(null, iQSDID, null, null, iBODID, iInstanceID, sCurrentGuestUser);
            var oQSInstance = (XIIQS)oCR.oResult;
            oQSInstance.QSDefinition = oQSD;
            //var oQSInstance = XiLinkRepository.GetQuestionSetInstance(iQSDID, sGUID, sMode, iBODID, iInstanceID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
            if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iInstanceID > 0)
            {
                oQSInstance.iBOIID = iInstanceID;
                oQSInstance.FKiBODID = iBODID;
            }
            var iActiveStepID = 0;
            if (oQSInstance.Steps == null || oQSInstance.Steps.Count() == 0)
            {
                //Load First Step Instance
                if (!string.IsNullOrEmpty(sStepName))
                {
                    iActiveStepID = oQSInstance.QSDefinition.Steps[sStepName].ID;
                }
                else
                {
                    iActiveStepID = oQSInstance.GetActiveStepID(0, sGUID);
                }
            }
            else
            {
                if (oQSInstance.iCurrentStepID == -1)
                {
                    oQSInstance.iCurrentStepID = oQSInstance.QSDefinition.Steps.Last().Value.ID;
                }
                //Checking Incomplete Question Set
                var StepDefs = oQSInstance.QSDefinition.Steps.ToList();
                var LastStepID = oQSInstance.Steps.Last().Value.FKiQSStepDefinitionID;
                //var RemainingSteps = oQSInstance.QSDefinition.QSSteps.Where(m => m.iOrder > StepDefs.Where(n => n.ID == oQSInstance.iCurrentStepID).FirstOrDefault().iOrder).Select(m => m.ID).ToList().Except(oQSInstance.nStepInstances.Select(m => m.FKiQSStepDefinitionID).ToList()).ToList();
                if (!string.IsNullOrEmpty(sStepName))
                {
                    int iStepID = oQSInstance.QSDefinition.Steps[sStepName].ID;
                    if (iStepID > 0)
                    {
                        iActiveStepID = iStepID;
                    }
                }
                else if (oQSInstance.iCurrentStepID == 0)
                {
                    iActiveStepID = oQSInstance.GetActiveStepID(0, sGUID);
                }
                else if (oQSInstance.iCurrentStepID > 0)
                {
                    var RemainingSteps = oQSInstance.QSDefinition.Steps.Where(m => m.Value.iOrder > StepDefs.Where(n => n.Value.ID == oQSInstance.iCurrentStepID).FirstOrDefault().Value.iOrder).OrderBy(m => m.Value.iOrder).Select(m => m.Value.ID).ToList();
                    if (RemainingSteps != null && RemainingSteps.Count() > 0)
                    {
                        iActiveStepID = oQSInstance.GetActiveStepID(RemainingSteps.FirstOrDefault(), sGUID);
                    }
                    else
                    {
                        iActiveStepID = oQSInstance.GetActiveStepID(oQSInstance.iCurrentStepID, sGUID);
                    }
                }
                else
                {
                    if (oQSInstance.Steps.Count() == 1)
                    {
                        oQSInstance.Steps.FirstOrDefault().Value.bIsCurrentStep = true;
                    }
                    else
                    {
                        oQSInstance.Steps.FirstOrDefault().Value.bIsCurrentStep = true;
                    }
                }
            }
            if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks != null && oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks.Count() > 0)
            {
                foreach (var oQSLink in oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks.Values)
                {
                    foreach (var oLink in oQSLink.XiLink.OrderBy(m => m.Value.rOrder))
                    {
                        if (oLink.Value.sType == "Pre")
                        {
                            GetXiLinkContent(oLink.Value.FKiXILInkID, sGUID);
                        }
                    }
                }
            }
            oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepID, sGUID);
            //if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks != null && oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks.Count() > 0)
            //{
            //    foreach (var oQSLink in oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).FirstOrDefault().Value.QSLinks.Values)
            //    {
            //        foreach (var oLink in oQSLink.XiLink.OrderBy(m => m.Value.rOrder))
            //        {
            //            if (oLink.Value.sType == "Post")
            //            {
            //                GetXiLinkContent(oLink.Value.FKiXILInkID, sGUID);
            //            }
            //        }
            //    }
            //}
            if (oQSInstance.QSDefinition != null && oQSInstance.QSDefinition.Steps.Count() > 0)
            {
                oQSInstance.iCurrentStepID = oQSInstance.Steps.Where(m => m.Value.bIsCurrentStep == true).FirstOrDefault().Value.FKiQSStepDefinitionID;
                oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == oQSInstance.iCurrentStepID).Select(m => m.sName).FirstOrDefault();
                //Updating CurrentStepID and Question Set Instance in Cache
                //oCache.Set_ParamVal(sSessionID, sGUID, "CurrentStepID_" + iXISemanticID, oQSInstance.nStepInstances.Where(m => m.bIsCurrentStep == true).FirstOrDefault().FKiQSStepDefinitionID.ToString());
                //oCache.UpdateCacheObject("QuestionSet", sGUID, oQSInstance, sDatabase, oQSInstance.FKiQSDefinitionID);
            }
            if (!string.IsNullOrEmpty(sStepName))
            {
                int iStepID = oQSInstance.QSDefinition.Steps[sStepName].ID;
                if (iStepID > 0)
                {
                    oQSInstance.Steps.ToList().ForEach(m => m.Value.bIsCurrentStep = false);
                    oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionID == iStepID).FirstOrDefault().Value.bIsCurrentStep = true;
                }
            }
            if (oQSInstance.History == null)
            {
                oQSInstance.History = new List<int>();
            }
            bool IsHistory = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == oQSInstance.iCurrentStepID).Select(m => m.bIsHistory).FirstOrDefault();
            if (oQSInstance.History.IndexOf(oQSInstance.iCurrentStepID) == -1 && IsHistory)
            {
                oQSInstance.History.Add(oQSInstance.iCurrentStepID);
            }
            return oQSInstance;
        }

        [HttpPost]
        public ActionResult GetQuestionSetInstance(int iQSID, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sCurrentGuestUser = string.Empty;
                if (SessionManager.UserUniqueID != null)
                {
                    sCurrentGuestUser = SessionManager.UserUniqueID;
                }
                string sOrgName = SessionManager.OrganisationName;
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                var oQSInstance = XiLinkRepository.GetQuestionSetInstance(iQSID, sGUID, sMode, iBODID, iInstanceID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                return Json(oQSInstance, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
        }


        public XIIQS GetInternalQuestionSet(int iQSIID)
        {
            int iUserID = 0;
            if (SessionManager.UserID > 0)
            {
                iUserID = SessionManager.UserID;
            }
            //var iQSIID = iInstanceID;

            XIIXI oXI = new XIIXI();
            var oQSIns = oXI.GetQSInstanceByID(iQSIID);
            var oQSInstance = oXI.GetQuestionSetInstanceByID(oQSIns.FKiQSDefinitionID, iQSIID, null, 0, 0, oQSIns.FKiUserCookieID);
            if (oQSInstance.Steps.Values.Count() > 0)
            {
                oQSInstance.Steps.Values.FirstOrDefault().bIsCurrentStep = true;
            }
            //cQSInstance oQSIns = new cQSInstance();
            //oQSIns = XiLinkRepository.GetQSInstanceByID(iQSIID);
            // var oQSInstance = XiLinkRepository.GetQuestionSetInstance(oQSIns.FKiQSDefinitionID, null, null, 0, 0, iUserID, sOrgName, sDatabase, oQSIns.FKiUserCookieID);
            //oQSInstance = LoadStepInstance(oQSInstance, null, 0);//Commented
            return oQSInstance;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetNextStep(XIIQSStep oStepI, string sGUID, string sType, int iQSIID = 0)
        {
            try
            {
                XIIXI oIXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                XIIQS oQSInstance = new XIIQS();
                oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QSI", sGUID, iQSIID);
                if (oQSInstance.QSDefinition == null)
                {
                    oQSInstance = oIXI.GetQuestionSetInstanceByID(0, iQSIID, null, 0, 0, null);
                }
                var oStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == oStepI.FKiQSStepDefinitionID).FirstOrDefault();
                if (oQSInstance.iCurrentStepID == 0)
                {
                    oQSInstance.iCurrentStepID = oStepD.ID;
                }
                if (oQSInstance.Steps.ContainsKey(oStepD.sName))
                {
                    oQSInstance.Steps[oStepD.sName] = oStepI;
                    foreach (var Section in oQSInstance.Steps[oStepD.sName].Sections)
                    {
                        foreach (var Field in Section.Value.XIValues)
                        {
                            oQSInstance.XIValues[Field.Key].sValue = Field.Value.sValue;
                        }
                    }
                }
                //oQSInstance.Steps.Values.Where(m=>m.ID == oStepI.ID).FirstOrDefault()
                SessionManager.sGUID = sGUID;
                string sCurrentGuestUser = string.Empty;
                if (SessionManager.UserUniqueID != null)
                {
                    sCurrentGuestUser = SessionManager.UserUniqueID;
                }
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                var sSessionID = HttpContext.Session.SessionID;
                string sDatabase = SessionManager.CoreDatabase;

                string sOrgName = SessionManager.OrganisationName;
                //var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID));
                var CurrentStepID = oQSInstance.iCurrentStepID;
                ViewBag.sGUID = sGUID;
                var LastStepID = oQSInstance.QSDefinition.Steps.Select(m => m.Value.ID).LastOrDefault();
                //Checking Database Save type for Question Set
                if (oQSInstance.QSDefinition.SaveType.ToLower() == "Save at end".ToLower() && CurrentStepID == LastStepID)
                {
                    if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == CurrentStepID).Select(m => m.Value.bInMemoryOnly).FirstOrDefault() == false)
                    {
                        SaveQSInstances(oQSInstance, sGUID);
                    }
                }
                else if (oQSInstance.QSDefinition.SaveType.ToLower() == "Save as Populated".ToLower())
                {
                    if (oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == CurrentStepID).Select(m => m.Value.bInMemoryOnly).FirstOrDefault() == false)
                    {
                        SaveQSInstances(oQSInstance, sGUID);
                    }
                }
                var oCurrentStep = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == CurrentStepID).Select(m => m.Value).FirstOrDefault();
                if (oCurrentStep.QSLinks != null && oCurrentStep.QSLinks.Count() > 0)
                {
                    foreach (var oQSLink in oCurrentStep.QSLinks.Values)
                    {
                        foreach (var oLink in oQSLink.XiLink.OrderBy(m => m.Value.rOrder))
                        {
                            if (oLink.Value.sType == "Post")
                            {
                                GetXiLinkContent(oLink.Value.FKiXILInkID, sGUID);
                            }
                        }
                    }
                }
                var CurrentOrder = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == CurrentStepID).Select(m => m.Value.iOrder).FirstOrDefault();
                //oQSInstance.nStepInstances = oQSInstance.nStepInstances;
                int NextStepID = 0;
                //Checking Navigations of a step to decide next step
                if (oCurrentStep.Navigations != null && oCurrentStep.Navigations.Count() > 0)
                {
                    foreach (var Navs in oCurrentStep.Navigations)
                    {
                        if (NextStepID == 0)
                        {
                            var Nav = Navs.Value;
                            if (Nav.sField != null && Nav.sOperator != null && Nav.sValue != null)
                            {
                                var oStepIns = oQSInstance.Steps.Where(m => m.Value.FKiQSStepDefinitionID == CurrentStepID).FirstOrDefault();
                                var StepDef = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == oStepIns.Value.FKiQSStepDefinitionID).FirstOrDefault();
                                var FieldDef = StepDef.Value.FieldDefs.Where(m => m.Value.FieldOrigin.sName == Nav.sField).FirstOrDefault();
                                var FieldValue = oStepIns.Value.XIValues.Where(m => m.Value.FKiFieldDefinitionID == FieldDef.Value.ID).FirstOrDefault();
                                //var Result = EvaluateExpression(FieldValue.Value, Nav.sOperator, Nav.sValue, FieldDef.FieldOrigin);
                                //if (Result)
                                //{
                                //    NextStepID = Nav.iNextStepID;
                                //}//Commented
                            }
                        }
                    }
                }
                //Getting Next Step
                var NextSteps = new List<XIDQSStep>();
                var NextStep = new XIDQSStep();
                string sNextStep = oCache.Get_ParamVal(sSessionID, sGUID, null, "NextStep");
                string sIsQsLoad = oCache.Get_ParamVal(sSessionID, sGUID, null, "IsQsLoad");

                Common.SaveErrorLog("sNextStep:" + sNextStep, "XIDNA");
                if (!string.IsNullOrEmpty(sNextStep))
                {
                    NextStep = oQSInstance.QSDefinition.StepD(sNextStep);
                    if (NextStep != null)
                    {
                        NextStepID = NextStep.ID;
                    }
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", "", null, null);
                }

                if (NextStepID == 0)
                {
                    NextSteps = oQSInstance.QSDefinition.Steps.Values.OrderBy(m => m.iOrder).Where(m => m.iOrder > CurrentOrder).ToList();
                    NextStep = NextSteps.FirstOrDefault();
                }
                else
                {
                    NextStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == NextStepID).FirstOrDefault();
                    NextSteps = oQSInstance.QSDefinition.Steps.Values.OrderBy(m => m.iOrder).Where(m => m.iOrder > NextStep.iOrder).ToList();
                }
                //Getting Next Step Instance
                int iActiveStepID = 0;
                if (NextStep != null)
                {
                    iActiveStepID = oQSInstance.GetActiveStepID(NextStep.ID, sGUID);
                    var oNextStep = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iActiveStepID).Select(m => m.Value).FirstOrDefault();

                    if (oNextStep.QSLinks != null && oNextStep.QSLinks.Count() > 0 && string.IsNullOrEmpty(sIsQsLoad))
                    {
                        foreach (var oQSLink in oNextStep.QSLinks.Values)
                        {
                            foreach (var oLink in oQSLink.XiLink.OrderBy(m => m.Value.rOrder))
                            {
                                if (oLink.Value.sType == "Pre")
                                {
                                    GetXiLinkContent(oLink.Value.FKiXILInkID, sGUID);
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sIsQsLoad))
                    {
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "IsQsLoad", "", null, null);
                    }
                    if (oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == iActiveStepID).FirstOrDefault() == null)
                    {
                        oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, iActiveStepID, sGUID);
                    }
                    else
                    {
                        oQSInstance.Steps.Values.ToList().Where(m => m.FKiQSStepDefinitionID == iActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                    }
                    oQSInstance.Steps.Values.ToList().ForEach(m => m.bIsCurrentStep = false);
                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == iActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                    oQSInstance.iCurrentStepID = iActiveStepID;
                    oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == iActiveStepID).Select(m => m.sName).FirstOrDefault();// NextStep.sName;
                    Common.SaveErrorLog("Step Execution: Started " + oQSInstance.sCurrentStepName + " Step", "XIDNA");

                    var oStepMessage = oCache.Get_ObjectSetCache("StepMessage", sGUID, sSessionID);//(sSessionID, sGUID, null, "StepMessage");
                    oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == iActiveStepID).FirstOrDefault().XiMessages = new Dictionary<string, string>();
                    if (oStepMessage != null)
                    {
                        if (NextStep != null)
                        {
                            Dictionary<string, string> dictmsgs = new Dictionary<string, string>();
                            dictmsgs = (Dictionary<string, string>)oStepMessage;
                            oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == iActiveStepID).FirstOrDefault().XiMessages = dictmsgs;
                            // oCache.Set_ObjectSetCache(sSessionID, "StepMessage", sGUID, null);
                            HttpRuntime.Cache.Remove("StepMessage" + "_" + sGUID + "_" + sSessionID);
                        }
                    }
                    //oQSInstanceNew.nStepInstances = oQSInstance.nStepInstances;
                    //oCache.Set_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID, NextStep.ID.ToString());
                    //oCache.UpdateCacheObject("QuestionSet", sGUID, oQSInstance, sDatabase, oQSInstance.FKiQSDefinitionID);
                }
                else
                {
                }
                bool IsHistory = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == iActiveStepID).Select(m => m.bIsHistory).FirstOrDefault();
                if (oQSInstance.History.IndexOf(oQSInstance.iCurrentStepID) == -1 && IsHistory)
                {
                    oQSInstance.History.Add(oQSInstance.iCurrentStepID);
                }
                oCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.ID, oQSInstance);
                oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}", Convert.ToString(oQSInstance.ID), "autoset", null);
                int LastStep = oQSInstance.QSDefinition.Steps.Values.Where(m => m.sName.ToLower() == "Your Quotes".ToLower()).Select(m => m.ID).FirstOrDefault();
                if (sType.ToLower() == "public")
                {
                    Common.SaveErrorLog("Step Execution: Returned " + oQSInstance.sCurrentStepName + " Step", "XIDNA");
                    return PartialView("_QuestionSet", oQSInstance);
                }
                else
                {
                    return PartialView("_QuestionSetInternal", oQSInstance);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "XIDNA");
                return null;
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool EvaluateExpression(cFieldInstance fieldValue, string sOperator, string sExpectedValue, cFieldOrigin oFieldOrigin)
        {
            MethodInfo function = CreateDynamicFunction();
            var Datatype = oFieldOrigin.DataTypes.sBaseDataType;
            object[] parametersArray = new object[] { Datatype, fieldValue.sValue, sOperator, sExpectedValue };
            var Dynresult = function.Invoke(null, parametersArray);
            return (bool)Dynresult;
        }

        public static MethodInfo CreateDynamicFunction()
        {
            string code = @"
                using System;
            
                namespace UserFunctionsNew
                {                
                    public class EvaluateExpression
                    {                
                        public static bool GetResult(string DataType, string InputValue, string Operator, string ExpectedValue)
                        {
                            if(DataType == ""int""){
                                var ConvertedValue = Convert.ToInt32(InputValue);
                                if(Operator == "">""){
                                    if(ConvertedValue > Convert.ToInt32(ExpectedValue))
                                    {
                                        return true;
                                    }
                                }
                                else if(Operator == ""<""){
                                    if(ConvertedValue < Convert.ToInt32(ExpectedValue))
                                    {
                                        return true;
                                    }
                                }else if(Operator == "">=""){
                                    if(ConvertedValue >= Convert.ToInt32(ExpectedValue))
                                    {
                                        return true;
                                    }
                                }else if(Operator == ""<=""){
                                    if(ConvertedValue <= Convert.ToInt32(ExpectedValue))
                                    {
                                        return true;
                                    }
                                }else if(Operator == ""==""){
                                    if(ConvertedValue == Convert.ToInt32(ExpectedValue))
                                    {
                                        return true;
                                    }
                                }
                                else{ return false;}
                            }
                            else {
                                if(Operator == ""=="" || Operator == ""=""){
                                    if(InputValue == ExpectedValue)
                                    {
                                        return true;
                                    }
                                }
                                else{ return false;}
                            }
                            return false;
                        }
                    }
                }
            ";

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(), code);

            Type binaryFunction = results.CompiledAssembly.GetType("UserFunctionsNew.EvaluateExpression");
            return binaryFunction.GetMethod("GetResult");
        }



        [HttpPost]
        public ActionResult GetCurrentStepID(int iQSID, string sGUID)
        {
            var sSessionID = HttpContext.Session.SessionID;
            var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "CurrentStepID_" + iQSID));
            return Json(CurrentStepID, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetPreviousStep(int iQSIID, string sGUID, string sType)
        {
            XIInfraCache oCache = new XIInfraCache();
            XIIQS oQSInstance = new XIIQS();
            oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QSI", sGUID, iQSIID);
            string sCurrentGuestUser = string.Empty;
            if (SessionManager.UserUniqueID != null)
            {
                sCurrentGuestUser = SessionManager.UserUniqueID;
            }
            var sSessionID = HttpContext.Session.SessionID;
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = 0;
            if (SessionManager.UserID > 0)
            {
                iUserID = SessionManager.UserID;
            }
            string sOrgName = SessionManager.OrganisationName;
            ViewBag.sGUID = sGUID;
            //var oQSInstance = XiLinkRepository.GetQuestionSetInstance(iQSID, sGUID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
            //var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID));

            var CurrentStepID = oQSInstance.iCurrentStepID;
            //Getting Previous StepInstance Based on Index of nStepInstances list
            int index = oQSInstance.Steps.Values.ToList().FindIndex(a => a.FKiQSStepDefinitionID == CurrentStepID);
            if (index == 0)
            {
                return PartialView("_QuestionSet", oQSInstance);
            }
            //var CurrentOrder = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == CurrentStepID).Select(m => m.iOrder).FirstOrDefault();

            //var PreviousOrder = oQSInstance.QSDefinition.Steps.Values.Where(m => m.sIsHidden == "off").OrderByDescending(m => m.iOrder).Where(m => m.iOrder < CurrentOrder).Select(m => m.ID).FirstOrDefault();
            //var NxtStp = oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == PreviousOrder).FirstOrDefault();

            //var IsVisible = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == NxtStp.FKiQSStepDefinitionID).FirstOrDefault();
            //if (IsVisible.sIsHidden == "on")
            //{
            //    PreviousOrder = oQSInstance.QSDefinition.Steps.Values.Where(m => m.sIsHidden == "off").OrderByDescending(m => m.iOrder).Where(m => m.iOrder < PreviousOrder).Select(m => m.ID).FirstOrDefault();
            //    NxtStp = oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == PreviousOrder).FirstOrDefault();
            //}

            int iPrevStepID = 0;
            var iCurrentStepIndex = oQSInstance.History.IndexOf(oQSInstance.iCurrentStepID);
            if (iCurrentStepIndex >= 0)
            {
                iPrevStepID = oQSInstance.History[iCurrentStepIndex - 1];
            }
            //var Step = QSSteps.FirstOrDefault();
            oQSInstance.Steps.Values.ToList().ForEach(m => m.bIsCurrentStep = false);
            oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == iPrevStepID).FirstOrDefault().bIsCurrentStep = true;
            oQSInstance.iCurrentStepID = iPrevStepID;
            oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Where(m => m.Value.ID == iPrevStepID).Select(m => m.Value.sName).FirstOrDefault();
            //oQSInstance.History.Add(iPrevStepID);
            //oCache.Set_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID, NxtStp.FKiQSStepDefinitionID.ToString());
            if (sType.ToLower() == "public")
            {
                return PartialView("_QuestionSet", oQSInstance);
            }
            else
            {
                return PartialView("_QuestionSetInternal", oQSInstance);
            }
        }
        [HttpPost]
        public XIIQS SaveQSInstances(XIIQS oQSInstance, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIQS oQSI = new XIIQS();
                string sCurrentGuestUser = string.Empty;
                int iUserID = 0;
                if (SessionManager.UserUniqueID != null)
                {
                    sCurrentGuestUser = SessionManager.UserUniqueID;
                }
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                //var CurrentStepID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, "CurrentStepID_" + oQSInstance.FKiQSDefinitionID));
                //var CurrentStepID = oQSInstance.iCurrentStepID;
                var Response = oQSI.Save(oQSInstance, sCurrentGuestUser);
                //var Response = XiLinkRepository.SaveQSInstance(oQSInstance, CurrentStepID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                return oQSInstance;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetAboutInsurer(string sInsurer)
        {
            return PartialView("_AboutInsurer", sInsurer);
        }

        [HttpPost]
        public ActionResult GetStepContent(int iStepID, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                var Step = XiLinkRepository.GetStepDefinition(iStepID, sGUID, iUserID, sOrgName, sDatabase);
                return Json(Step, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        [AllowAnonymous]
        public ActionResult GetStepData(XIDQSStep oStepDef, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIQS oQSInstance = new XIIQS();
                XIDQS QSDefinition = new XIDQS();
                int iUserID = 0;
                if (SessionManager.UserID > 0)
                {
                    iUserID = SessionManager.UserID;
                }
                string sOrgName = SessionManager.OrganisationName;
                var Step = oStepDef; //XiLinkRepository.GetStepDefinition(iStepID, sGUID, iUserID, sOrgName, sDatabase);
                QSDefinition.Steps = new Dictionary<string, XIDQSStep>();
                if (!string.IsNullOrEmpty(Step.sName))
                {
                    QSDefinition.Steps[Step.sName] = Step;
                }
                else
                {
                    QSDefinition.Steps[Step.ID.ToString()] = Step;
                }

                XIIQSStep oStepInstance = new XIIQSStep();
                List<XIIValue> nFieldValues = new List<XIIValue>();
                if (((EnumSemanticsDisplayAs)Step.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Fields.ToString())
                {
                    //if (Step.FieldDefs != null)
                    //{
                    //    nFieldValues = Step.FieldDefs.Select(m => new XIIValue { FKiFieldDefinitionID = m.Value.ID }).ToList();
                    //    oStepInstance.XIValues = nFieldValues;
                    //}
                }
                //Loading Sections for Step
                else if (((EnumSemanticsDisplayAs)Step.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Sections.ToString())
                {
                    Dictionary<string, XIIQSSection> nSecIns = new Dictionary<string, XIIQSSection>();
                    if (Step.Sections != null && Step.Sections.Count() > 0)
                    {
                        foreach (var sec in Step.Sections)
                        {
                            XIIQSSection oSecIns = new XIIQSSection();
                            oSecIns.FKiStepSectionDefinitionID = sec.Value.ID;
                            //if (((EnumSemanticsDisplayAs)sec.Value.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Fields.ToString())
                            //{
                            //    List<XIIValue> nSecFieldValues = new List<XIIValue>();
                            //    if (sec.Value.FieldDefs != null && sec.Value.FieldDefs.Count() > 0)
                            //    {
                            //        nSecFieldValues = sec.Value.FieldDefs.Select(m => new XIIValue { FKiFieldDefinitionID = m.Value.ID }).ToList();
                            //    }
                            //    oSecIns.XIValues = nSecFieldValues;
                            //}
                            nSecIns[sec.Value.ID.ToString()] = oSecIns;
                        }
                        //oStepInstance.nSectionInstances = Step.Sections;
                    }
                    oStepInstance.Sections = nSecIns;
                }
                oStepInstance.bIsCurrentStep = true;
                oStepInstance.FKiQSStepDefinitionID = oStepDef.ID;
                oQSInstance.QSDefinition = QSDefinition;
                oQSInstance.Steps = new Dictionary<string, XIIQSStep>();
                if (!string.IsNullOrEmpty(Step.sName))
                {
                    oQSInstance.Steps[Step.sName] = oStepInstance;
                }
                else
                {
                    oQSInstance.Steps[Step.ID.ToString()] = oStepInstance;
                }
                ViewBag.sGUID = sGUID;
                return PartialView("~/Views/XIComponents/_StepComponentContent.cshtml", oQSInstance);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AmendDetails(int iQSIID, string sGUID, string sType)
        {
            XIInfraCache oCache = new XIInfraCache();
            XIIQS oQSInstance = new XIIQS();
            oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QSI", sGUID, iQSIID);
            string sCurrentGuestUser = string.Empty;
            if (SessionManager.UserUniqueID != null)
            {
                sCurrentGuestUser = SessionManager.UserUniqueID;
            }
            var sSessionID = HttpContext.Session.SessionID;
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = 0;
            if (SessionManager.UserID > 0)
            {
                iUserID = SessionManager.UserID;
            }
            string sOrgName = SessionManager.OrganisationName;
            ViewBag.sGUID = sGUID;

            if (oQSInstance.Steps.Values.Count() > 0)
            {
                var StepID = oQSInstance.QSDefinition.Steps.Values.ToList().OrderBy(m => m.iOrder).Select(m => m.ID).FirstOrDefault();
                var oCurrentStep = oQSInstance.Steps.Values.ToList().Where(m => m.FKiQSStepDefinitionID == StepID).FirstOrDefault();
                if (oCurrentStep == null)
                {
                    var ActiveStepID = oQSInstance.GetActiveStepID(StepID, sGUID);
                    oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, ActiveStepID);
                    oQSInstance.Steps.Values.ToList().Where(m => m.FKiQSStepDefinitionID == ActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                }
                else
                {
                    var ActiveStepID = oQSInstance.GetActiveStepID(StepID, sGUID);
                    oQSInstance.Steps.Values.ToList().Where(m => m.FKiQSStepDefinitionID == ActiveStepID).FirstOrDefault().bIsCurrentStep = true;
                }
            }
            oQSInstance.iCurrentStepID = oQSInstance.Steps.Values.Where(m => m.bIsCurrentStep == true).Select(m => m.FKiQSStepDefinitionID).FirstOrDefault();
            oQSInstance.sCurrentStepName = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == oQSInstance.iCurrentStepID).Select(m => m.sName).FirstOrDefault();
            oQSInstance.sQSType = "Internal";
            oQSInstance.History = new List<int>();
            oQSInstance.History.Add(oQSInstance.iCurrentStepID);
            oCache.Set_QuestionSetCache("QSI", sGUID, oQSInstance.ID, oQSInstance);
            if (sType.ToLower() == "public")
            {
                return PartialView("_QuestionSet", oQSInstance);
            }
            else
            {
                return PartialView("_QuestionSetInternal", oQSInstance);
            }
        }

        #endregion QuestionSet

        #region DemoXIScripts
        [AllowAnonymous]
        public ActionResult XIScripting(int XILinkID, string sGUID, int iInstanceID, int iBOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //iBOID = 2;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iCustomerID = SessionManager.CustomerID;
                var ScriptResults = XiLinkRepository.XIScripting(XILinkID, sGUID, iInstanceID, iBOID, iUserID, sOrgName, sDatabase, iCustomerID);
                return Json(ScriptResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion DemoXIScripts
        #region EditableGrid
        public ActionResult GetBOInstance(int iBOID, string sGroupName, string sBOName, int iInstanceID = 0)
        {

            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                if (sBOName != null && sGroupName != null)
                {
                    var oBODisplay = oXiAPI.GetFormData(sBOName, sGroupName, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                    return PartialView("_AddEditableView", oBODisplay);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            return null;
        }
        public ActionResult DeleteBOInstance(int iBOID, string sGroupName, string sBOName, int iInstanceID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;

                var oBODisplay = BusinessObjectsRepository.DeleteFormData(iBOID, sBOName, sGroupName, iInstanceID, sVisualisation, iUserID, sOrgName, sDatabase, null);
                return Json(oBODisplay, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion


        [HttpPost]
        [AllowAnonymous]
        public ActionResult DeleteBO(int iInstanceID, string sGUID, string sBOName)
        {
            XIIXI oXIIXI = new XIIXI();
            var oBOI = oXIIXI.BOI(sBOName, iInstanceID.ToString());
            try
            {
                var Response = oBOI.Delete(oBOI);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        //, bool lazyload
        public XIIBO GetStructureBoInstanceList(string strName)
        {
            XIIBO oBOI = new XIIBO();
            XIDStructure oXIDStructure = new XIDStructure();
            Dictionary<string, List<XIIBO>> oMyClass = null;
            XIIBO obj = null;
            int iBOID = 17;
            string sDatabase = SessionManager.CoreDatabase; var sSessionID = HttpContext.Session.SessionID;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            ModelDbContext dbcontext = new ModelDbContext();
            string sUID = SessionManager.sGUID;
            oUser.UserID = iUserID;
            oXIDStructure.sCoreDataBase = sDatabase;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            oXIDStructure.sOrgDataBase = oUser.sDatabaseName;
            oXIDStructure.iOrgId = SessionManager.OrganizationID;
            //oMyClass=oXIDStructure.GetStructureBoInstance(strName, obj, 0, oMyClass, iBOID, "ACPolicy_T", 0,null,null);
            //oMyClass = GetStructureBoInstance(strName, obj, 0, oMyClass, iBOID, "ACPolicy_T",0);
            oBOI.SubChildI = oMyClass;
            //oBOInstanceLIst = GetStructureBoInstance(strName);
            return oBOI;
        }

        [AllowAnonymous]
        public Dictionary<string, List<XIIBO>> GetStructureBoInstance(string sStructureCode, XIIBO oBOIParent, int iOneClickID, Dictionary<string, List<XIIBO>> oMainBOInstances, int iBOID, string sBOName, int iParentStructureID)
        {
            List<XIIBO> oTempList = new List<XIIBO>();
            List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
            Dictionary<string, List<XIIBO>> oMyClass = new Dictionary<string, List<XIIBO>>();
            Dictionary<string, XIIBO> oDictionaryBOI = null;
            Dictionary<string, List<XIIBO>> oMyDictClass = new Dictionary<string, List<XIIBO>>();
            if (oMainBOInstances != null)
            {
                oMyDictClass = oMainBOInstances;
            }
            XIIBO oBOInstance = new XIIBO();
            string sDatabase = SessionManager.CoreDatabase; var sSessionID = HttpContext.Session.SessionID;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            ModelDbContext dbcontext = new ModelDbContext();
            string sUID = SessionManager.sGUID;
            oUser.UserID = iUserID;
            oUser.sCoreDatabaseName = sDatabase;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            var sOrgDB = oUser.sDatabaseName;
            //laod structure def
            XIDStructure oXIDStructure = new XIDStructure();
            List<XIDStructure> oLXIDStructure = new List<XIDStructure>();
            List<XIDStructure> oLXIDSubStructure = new List<XIDStructure>();
            oXIDStructure.sCode = sStructureCode;
            oXIDStructure.BOID = iBOID;
            oXIDStructure.FKiParentID = "#";
            var XIDStruct = oXIDStructure.Get_XIStructureDefinition();
            if (XIDStruct.xiStatus == 0 && XIDStruct.oResult != null)
            {
                oLXIDStructure = (List<XIDStructure>)XIDStruct.oResult;
                oXIDStructure = oLXIDStructure.FirstOrDefault();
                if (oXIDStructure == null)
                {
                    if (iParentStructureID != 0)
                    {
                        XIDStructure oXIDSubStructure = new XIDStructure();
                        oXIDSubStructure.FKiParentID = iParentStructureID.ToString();
                        oXIDSubStructure.sCode = sStructureCode;
                        oXIDSubStructure.BOID = iBOID;
                        var XIDSubStruct = oXIDSubStructure.Get_XIStructureDefinition();
                        if (XIDSubStruct.xiStatus == 0 && XIDSubStruct.oResult != null)
                        {
                            oLXIDSubStructure = (List<XIDStructure>)XIDSubStruct.oResult;
                        }
                    }
                    oXIDStructure = oLXIDSubStructure.FirstOrDefault();
                }
                if (oXIDStructure != null)
                {
                    long iMainStructureid = oXIDStructure.ID;
                    string sMainStructureid = Convert.ToString(iMainStructureid);
                    int imainBOID = oXIDStructure.BOID;
                    string MainBoName = oXIDStructure.sBO;
                    int iMainOneClickID = 436;
                    if (iOneClickID != 0)
                    {
                        iMainOneClickID = iOneClickID;
                    }
                    XIDXI oXID = new XIDXI();
                    XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("", iBOID.ToString()).oResult;
                    var param = oCache.GetAllParamsUnderGUID(sSessionID, sSessionID, "");
                    List<CNV> nParams = new List<CNV>();
                    nParams = param.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                    //oIXI.oNVParams = nParams;
                    XID1Click oD1Click = new XID1Click();
                    oXID.sCoreDatabase = sDatabase;
                    oXID.sOrgDatabase = sOrgDB;
                    oXID.iOrgID = oUser.FKiOrganisationID;
                    //list data loading
                    oD1Click.ID = iMainOneClickID;
                    Dictionary<string, object> Params = new Dictionary<string, object>();
                    Params["Name"] = sBOName;
                    var oOneclickDef = oXID.Get_1ClickDefinition(null, iMainOneClickID.ToString());
                    if (oOneclickDef.xiStatus == 0 && oOneclickDef.oResult != null)
                    {
                        XID1Click oXID1Click = (XID1Click)oOneclickDef.oResult;
                        XIDStructure oStructure = new XIDStructure();
                        oD1Click.Query = oStructure.ReplaceExpressionWithCacheValue(oXID1Click.Query, nParams);
                    }
                    var DataSource = oXID.GetBODataSource(oBOD.iDataSource);
                    oD1Click.sConnectionString = DataSource;
                    oDictionaryBOI = oD1Click.OneClick_Execute();
                    if (oBOIParent != null)
                    {
                        oBOIParent.SubChildI.Add(sBOName, oDictionaryBOI.Values.ToList());//add substructures dictionarylist to main structure
                    }
                    //check for sub stru
                    oTempList = oDictionaryBOI.Values.ToList();
                    if (oMainBOInstances == null)
                    {
                        oMyDictClass.Add(sBOName, oTempList);
                    }
                    oXIDStructure = new XIDStructure();
                    oXIDStructure.FKiParentID = sMainStructureid;
                    XIDStruct = oXIDStructure.Get_XIStructureDefinition();
                    if (XIDStruct.xiStatus == 0 && XIDStruct.oResult != null)
                    {
                        oLXIDStructure = (List<XIDStructure>)XIDStruct.oResult;
                        oXIDStructure = oLXIDStructure.FirstOrDefault();
                        var oSubChildBOList = oLXIDStructure.Where(x => x.sType == "Sub Entity").ToList();//substructures list
                        if (oSubChildBOList != null)
                        {
                            foreach (var item in oTempList)// for every BO load sub structure
                            {
                                foreach (var subchild in oSubChildBOList) //for every sub structure
                                {
                                    string sParamValue = item.Attributes.Where(x => x.Key.ToLower() == "id".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                                    string sParamName = MainBoName;
                                    oCache.Set_ParamVal(sSessionID, sSessionID, null, "{XIP|" + sParamName + ".id}", sParamValue, "autoset", null);
                                    int iSubChildOneClickID = 0;
                                    List<XIIBO> oBOSubChildInstanceLIst = new List<XIIBO>(); // load sub structure & verify
                                    string sSubChildBOName = subchild.sBO;
                                    List<XIDStructure> oLXIDSubStructure1 = new List<XIDStructure>();
                                    XIDStructure oXIDSubStructure = new XIDStructure();
                                    oXIDSubStructure.FKiParentID = "#";
                                    oXIDSubStructure.sBO = sSubChildBOName;
                                    oXIDSubStructure.sCode = sStructureCode;
                                    XIDStruct = oXIDSubStructure.Get_XIStructureDefinition();
                                    if (XIDStruct.xiStatus == 0 && XIDStruct.oResult != null)
                                    {
                                        oLXIDSubStructure1 = (List<XIDStructure>)XIDStruct.oResult;
                                        oXIDSubStructure = oLXIDSubStructure1.FirstOrDefault();
                                    }
                                    XIDStructureDetail oXIDStructureDetails = new XIDStructureDetail();
                                    List<XIDStructureDetail> oLXIDStructureDetails = new List<XIDStructureDetail>();
                                    if (oXIDSubStructure == null)//if substructure don't have structure 
                                    {
                                        iParentStructureID = Convert.ToInt32(subchild.FKiParentID);
                                        imainBOID = subchild.BOID;
                                        oXIDStructureDetails.iParentStructureID = iParentStructureID;
                                        oXIDStructureDetails.FKiStructureID = subchild.ID;
                                    }
                                    else
                                    {
                                        if (oXIDSubStructure.ID != 0)
                                        {
                                            iParentStructureID = Convert.ToInt32(oXIDSubStructure.ID);
                                            oXIDStructureDetails.iParentStructureID = iParentStructureID;
                                            oXIDStructureDetails.FKiStructureID = oXIDSubStructure.ID;
                                            imainBOID = oXIDSubStructure.BOID;
                                        }
                                    }
                                    var oXIStructureRes = oXIDStructureDetails.Get_XIStructureDetailsDefinition();
                                    if (oXIStructureRes.xiStatus == 00 && oXIStructureRes.oResult != null)
                                    {
                                        oLXIDStructureDetails = (List<XIDStructureDetail>)oXIStructureRes.oResult;
                                        oXIDStructureDetails = oLXIDStructureDetails.FirstOrDefault();
                                    }
                                    iSubChildOneClickID = oXIDStructureDetails.i1ClickID;
                                    GetStructureBoInstance(sStructureCode, item, iSubChildOneClickID, oMyDictClass, imainBOID, subchild.sBO, iParentStructureID);
                                }
                            }
                        }
                    }
                    List<XIIBO> oSTRList = oBOInstanceLIst;
                    oMyClass = new Dictionary<string, List<XIIBO>>();
                    oMyClass.Add("", oDictionaryBOI.Values.ToList());
                }
            }
            return oMainBOInstances == null ? oMyDictClass : oMainBOInstances;
        }

        [AllowAnonymous]
        public XIIBO GetNotationValue()
        {
            string sDatabase = SessionManager.CoreDatabase; var sSessionID = HttpContext.Session.SessionID;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName; int iOrgID = SessionManager.OrganizationID;
            ModelDbContext dbcontext = new ModelDbContext();
            string sUID = SessionManager.sGUID;
            oUser.UserID = iUserID;
            oUser.sCoreDatabaseName = sDatabase;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            var sOrgDB = oUser.sDatabaseName;
            XIIBO oBII = new XIIBO(); long rTotalCost = 0;
            var boi = oIXI.BOI("ACPolicy_T", "127");
            var oLIst = oIXI.BOI("ACPolicy_T", "127").Structure("New Policy").XILoad();
            return oBII;
        }
        [AllowAnonymous]
        public ActionResult SetParams(string sID, string sGUID, string sName, string sBO)
        {
            string sSessionID = HttpContext.Session.SessionID;

            //if (!string.IsNullOrEmpty(sID))
            //{
            oCache.Set_ParamVal(sSessionID, sGUID, null, sName, sID, null, null);
            if (sBO == "Aggregations")
            {
                List<CNV> oNV = new List<CNV>();
                oNV.Add(new CNV { sName = "sGUID", sValue = sID });
                var oQuoteI = oIXI.BOI("Aggregations", "", "", oNV);
                if (oQuoteI.Attributes != null)
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|Aggregations.id}", oQuoteI.Attributes["id"].sValue, null, null);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|FKiQuoteID}", oQuoteI.Attributes["id"].sValue, null, null);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQuoteID}", oQuoteI.Attributes["id"].sValue, null, null);
                    var iProductVersionID = oQuoteI.Attributes["FKiProductVersionID"].sValue;
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", iProductVersionID);
                    string iProductID = oProductVersionI.Attributes["FKiProductID"].sValue;
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|FKiProductID}", iProductID, null, null);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|Product.id}", iProductID, null, null);
                    List<CNV> oNV1 = new List<CNV>();
                    oNV1.Add(new CNV { sName = "FKiProductID", sValue = iProductID });
                    var oTaskI = oIXI.BOI("XProdDiaryBatch_T", "", "", oNV1);
                    if (oTaskI.Attributes != null && oTaskI.Attributes.ContainsKey("FKiDiaryBatchID"))
                    {
                        oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|TaskBatchTempalte_T.ID}", oTaskI.Attributes["FKiDiaryBatchID"].sValue, null, null);
                    }
                }
            }
            //}
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public ActionResult StructureCopy()
        //{
        //    XIDStructure oStructure = new XIDStructure();
        //    XIIXI oXI = new XIIXI();
        //    XIBOBuilding oBOBuilding = new XIBOBuilding();
        //    int iACPolicyID = 127;
        //    string FKiProductID = "1";
        //    string sPolicyStartDate = "2018-10-10 16:16:50.220";
        //    //var oInstance = oIXI.BOI("ACPolicy_T", Convert.ToString(22677)).Structure("TS").XILoad();
        //    //var oResponse = oBOBuilding.BuildRequirementsBO(iACPolicyID, FKiProductID, sPolicyStartDate);
        //    //if (!oResponse.bOK && oResponse.xiStatus == xiEnumSystem.xiFuncResult.xiError)
        //    //{
        //    //    //oResult.oTraceStack.Add(new CNV { sName = "Create Requirements", sValue = "Error: While Creating Requirements" });
        //    //}
        //    XIInfraMenuComponent oMenuComponent = new XIInfraMenuComponent();
        //    List<CNV> oParams = new List<CNV>();
        //    CNV ONVPairs = new CNV();
        //    ONVPairs.sName = "MenuName";
        //    ONVPairs.sValue = "HomeMenu";
        //    oParams.Add(ONVPairs);

        //    ONVPairs = new CNV();
        //    ONVPairs.sName = "iUserID";
        //    ONVPairs.sValue = SessionManager.UserID.ToString();
        //    oParams.Add(ONVPairs);

        //    ONVPairs = new CNV();
        //    ONVPairs.sName = "sDatabase";
        //    ONVPairs.sValue = SessionManager.CoreDatabase;
        //    oParams.Add(ONVPairs);

        //    ONVPairs = new CNV();
        //    ONVPairs.sName = "iOrganizationID";
        //    ONVPairs.sValue = SessionManager.OrganizationID.ToString();
        //    oParams.Add(ONVPairs);
        //    var olist = oMenuComponent.XILoad(oParams);
        //    List<XIMenu> oRightMenus = new List<XIMenu>();
        //    if (olist.bOK && olist.oResult!=null)
        //    {
        //        oRightMenus = (List<XIMenu>)olist.oResult;
        //    }
        //    //var oList = oStructure.StructureCopy("ACPolicy_T", "127", "New Policy");
        //    //return Json(null, JsonRequestBehavior.AllowGet);
        //    return View("~/Views/XIComponents/_MenuComponent.cshtml", oRightMenus);
        //}


        public ActionResult BOICopy(string sInstanceID, string sBOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iInstanceID = 0;
            try
            {
                XIIBO oBOI = new XIIBO();
                var oCResult = oBOI.BOICopy(sInstanceID, sBOName);
                if (oCResult.bOK && oCResult.oResult != null)
                {
                    iInstanceID = (int)oCResult.oResult;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

            return Json(iInstanceID, JsonRequestBehavior.AllowGet);
        }

        #region XILinkLoad

        public ActionResult XILinkLoad(int iXILinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {

                XIInfraCache oCache = new XIInfraCache();
                XIInstanceBase oInstBase = new XIInstanceBase();
                XIILink oXILinkI = new XIILink();
                XIILink oXILinkRes = new XIILink();
                if (iXILinkID > 0)
                {
                    oXILinkI.iXILinkID = iXILinkID;
                    var Result = oXILinkI.Load();
                    if (Result.bOK && Result.oResult != null)
                    {
                        oInstBase = (XIInstanceBase)Result.oResult;
                    }
                }
                return PartialView("_XILinkContent", oInstBase);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [Compress]
        public JsonResult XILinkLoadJson(int iXILinkID = 0, string sGUID = "", List<CNV> oParams = null)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sSessionID = HttpContext.Session.SessionID;
                if (string.IsNullOrEmpty(sGUID))
                {
                    Singleton.Instance.sGUID = null;
                    Singleton.Instance.sActiveGUID.Remove(sSessionID);
                }
                XIInstanceBase oInstBase = new XIInstanceBase();
                XIILink oXILinkI = new XIILink();
                XIILink oXILinkRes = new XIILink();
                if (iXILinkID > 0)
                {
                    oXILinkI.sGUID = sGUID;
                    oXILinkI.iXILinkID = iXILinkID;
                    oXILinkI.oParams = oParams;
                    //oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|" + oBOD.Name + ".id}", ID.ToString(), null, null);
                    var Result = oXILinkI.Load();
                    if (Result.bOK && Result.oResult != null)
                    {
                        oInstBase = (XIInstanceBase)Result.oResult;
                        dynamic oMergedData = MergeHTMLRecurrsive(oInstBase);
                        return Json(oMergedData, JsonRequestBehavior.AllowGet);
                    }
                }
                //string json = JsonConvert.SerializeObject(oInstBase, Newtonsoft.Json.Formatting.Indented);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        private object MergeHTMLRecurrsive(dynamic oInstBase)
        {
            if (oInstBase.oContent.ContainsKey(XIConstant.ContentXIComponent))
            {
                var control = oInstBase.oContent[XIConstant.ContentXIComponent];
                var sType = control.GetType();
                XIIComponent compI = new XIIComponent();
                var oIns = new XIInstanceBase();
                if (sType.Name == "XIInstanceBase")
                {
                    oIns = (XIInstanceBase)oInstBase.oContent[XIConstant.ContentXIComponent];
                    compI = (XIIComponent)oIns.oContent[XIConstant.ContentXIComponent];
                }
                else if (sType.Name == "XIIComponent")
                {
                    compI = (XIIComponent)oInstBase.oContent[XIConstant.ContentXIComponent];
                }
                //var comp = (XIIComponent)XiiComp.oContent[XIConstant.ContentXIComponent];
                var compD = (XIDComponent)compI.oDefintion;
                if (compD.sName.ToLower() == XIConstant.OneClickComponent.ToLower())
                {
                    var o1ClickD = (XID1Click)compI.oContent[XIConstant.OneClickComponent];
                    var DATAMerge = MergeHTML("~/views/XIComponents/_OneClickComponent.cshtml", compI);
                    compI.oContent[XIConstant.OneClickComponent] = DATAMerge;
                    if (sType.Name == "XIInstanceBase")
                    {
                        oIns.oContent[XIConstant.ContentXIComponent] = compI;
                        oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
                    }
                    else
                    {
                        oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
                    }
                }
                if (compD.sName.ToLower() == XIConstant.FormComponent.ToLower())
                {
                    var o1ClickD = (XIBODisplay)compI.oContent[XIConstant.FormComponent];
                    var DATAMerge = MergeHTML("~/views/XIComponents/_FormComponent.cshtml", compI);
                    compI.oContent[XIConstant.FormComponent] = DATAMerge;
                    if (sType.Name == "XIInstanceBase")
                    {
                        oIns.oContent[XIConstant.ContentXIComponent] = compI;
                        oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
                    }
                    else
                    {
                        oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
                    }
                }
                else if (compD.sName.ToLower() == XIConstant.MenuComponent.ToLower())
                {
                    var o1ClickD = (List<XIMenu>)compI.oContent[XIConstant.MenuComponent];
                    var DATAMerge = MergeHTML("~/views/XIComponents/_MenuComponent.cshtml", compI);
                    compI.oContent[XIConstant.MenuComponent] = DATAMerge;
                    if (sType.Name == "XIInstanceBase")
                    {
                        oIns.oContent[XIConstant.ContentXIComponent] = compI;
                        oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
                    }
                    else
                    {
                        oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
                    }
                }
                else if (compD.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                {
                    var o1ClickD = (XID1Click)compI.oContent[XIConstant.HTMLComponent];
                    var DATAMerge = MergeHTML("~/views/XIComponents/_HtmlRepeaterComponent.cshtml", compI);
                    compI.oContent[XIConstant.HTMLComponent] = DATAMerge;
                    if (sType.Name == "XIInstanceBase")
                    {
                        oIns.oContent[XIConstant.ContentXIComponent] = compI;
                        oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
                    }
                    else
                    {
                        oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
                    }
                }
                else if (compD.sName.ToLower() == XIConstant.GridComponent.ToLower())
                {
                    var o1ClickD = (XID1Click)compI.oContent[XIConstant.GridComponent];
                    var DATAMerge = MergeHTML("~/views/XIComponents/_GridComponent.cshtml", compI);
                    compI.oContent[XIConstant.GridComponent] = DATAMerge;
                    if (sType.Name == "XIInstanceBase")
                    {
                        oIns.oContent[XIConstant.ContentXIComponent] = compI;
                        oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
                    }
                    else
                    {
                        oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
                    }
                }
                else if (compD.sName.ToLower() == XIConstant.QSComponent.ToLower())
                {
                    var oQSI = (XIIQS)compI.oContent[XIConstant.QSComponent];
                    if (oQSI.Steps != null && oQSI.Steps.Count() > 0)
                    {
                        var oStepI = oQSI.Steps.FirstOrDefault().Value;
                        var oStepD = (XIDQSStep)oStepI.oDefintion;
                        if (oStepD.iLayoutID > 0)
                        {
                            //var oLayoutIns = oStepI.oContent[XIConstant.ContentLayout];
                            var oLayoutD = (XIDLayout)oStepI.oContent[XIConstant.ContentLayout];
                            for (int i = 0; i < oLayoutD.LayoutMappings.Count(); i++)
                            {
                                MergeHTMLRecurrsive(oLayoutD.LayoutMappings[i]);
                            }
                        }
                        else if(oStepD.Sections.Count() >0)
                        {
                            //TO DO : Load XIFields as other components loading
                            foreach(var sec in oStepI.Sections)
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                if (oInstBase.oContent.ContainsKey(XIConstant.ContentDialog))
                {
                    var con = (XIDDialog)oInstBase.oContent[XIConstant.ContentDialog];
                    //var xifs = (XIDDialog)con.oContent.Values.FirstOrDefault();
                    MergeHTMLRecurrsive(con);
                }
                else if (oInstBase.oContent.ContainsKey(XIConstant.ContentLayout))
                {
                    var oLayoutD = (XIDLayout)oInstBase.oContent[XIConstant.ContentLayout];
                    for (int i = 0; i < oLayoutD.LayoutMappings.Count(); i++)
                    {
                        MergeHTMLRecurrsive(oLayoutD.LayoutMappings[i]);
                    }
                }
                else if (oInstBase.oContent.ContainsKey(XIConstant.ContentXILink))
                {
                    var XiLinkD = oInstBase.oContent[XIConstant.ContentXILink];
                    MergeHTMLRecurrsive(XiLinkD);
                }
                else if (oInstBase.oContent.ContainsKey(XIConstant.ContentStep))
                {
                    var oStepI = (XIIQSStep)oInstBase.oContent[XIConstant.ContentStep];
                    if (oStepI.Sections != null && oStepI.Sections.Count() > 0)
                    {
                        var oSecI = oStepI.Sections.FirstOrDefault().Value;
                        var oSecD = (XIDQSSection)oSecI.oDefintion;
                        //var osecIns = new XIInstanceBase();
                        if (oSecI.oContent.ContainsKey(XIConstant.ContentXIComponent))
                        {
                            //osecIns = (XIInstanceBase)oSecI.oContent[XIConstant.ContentXIComponent];
                            var compI = (XIIComponent)oSecI.oContent[XIConstant.ContentXIComponent];
                            var CompD = (XIDComponent)compI.oDefintion;
                            if (CompD.sName.ToLower() == XIConstant.OneClickComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_OneClickComponent.cshtml", compI);
                                compI.oContent[XIConstant.OneClickComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.FormComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_FormComponent.cshtml", compI);
                                compI.oContent[XIConstant.FormComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.XITreeStructure.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_XITreeStructureView.cshtml", compI);
                                compI.oContent[XIConstant.XITreeStructure] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.TabComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_TabComponent.cshtml", compI);
                                compI.oContent[XIConstant.TabComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.MenuComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_MenuComponent.cshtml", compI);
                                compI.oContent[XIConstant.MenuComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.GridComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_GridComponent.cshtml", compI);
                                compI.oContent[XIConstant.GridComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            else if (CompD.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                            {
                                var DATAMerge = MergeHTML("~/views/XIComponents/_HtmlRepeaterComponent.cshtml", compI);
                                compI.oContent[XIConstant.HTMLComponent] = DATAMerge;
                                oSecI.oContent[XIConstant.ContentXIComponent] = compI;
                            }
                            //oSecI.oContent[XIConstant.ContentXIComponent] = osecIns;
                            oStepI.Sections[oSecD.ID + "_Sec"] = oSecI;
                        }
                    }
                    else if (oStepI.oContent.ContainsKey(XIConstant.ContentLayout))
                    {
                        var oLayoutD = (XIDLayout)oStepI.oContent[XIConstant.ContentLayout];
                        for (int i = 0; i < oLayoutD.LayoutMappings.Count(); i++)
                        {
                            MergeHTMLRecurrsive(oLayoutD.LayoutMappings[i]);
                        }
                    }
                }
            }
            return oInstBase;
        }

        //private XIInstanceBase MergeHTMLRecurrsive(XIInstanceBase oInstBase)
        //{
        //    if (oInstBase.oContent.ContainsKey(XIConstant.ContentXIComponent))
        //    {
        //        var control = oInstBase.oContent[XIConstant.ContentXIComponent];
        //        var sType = control.GetType();
        //        XIIComponent compI = new XIIComponent();
        //        var oIns = new XIInstanceBase();
        //        if (sType.Name == "XIInstanceBase")
        //        {
        //            oIns = (XIInstanceBase)oInstBase.oContent[XIConstant.ContentXIComponent];
        //            compI = (XIIComponent)oIns.oContent[XIConstant.ContentXIComponent];
        //        }
        //        else if (sType.Name == "XIIComponent")
        //        {
        //            compI = (XIIComponent)oInstBase.oContent[XIConstant.ContentXIComponent];
        //        }
        //        //var comp = (XIIComponent)XiiComp.oContent[XIConstant.ContentXIComponent];
        //        var compD = (XIDComponent)compI.oDefintion;
        //        if (compD.sName.ToLower() == XIConstant.OneClickComponent.ToLower())
        //        {
        //            var o1ClickD = (XID1Click)compI.oContent[XIConstant.OneClickComponent];
        //            var DATAMerge = MergeHTML("~/views/XIComponents/_OneClickComponent.cshtml", compI);
        //            compI.oContent[XIConstant.OneClickComponent] = DATAMerge;
        //            if (sType.Name == "XIInstanceBase")
        //            {
        //                oIns.oContent[XIConstant.ContentXIComponent] = compI;
        //                oInstBase.oContent[XIConstant.ContentXIComponent] = oIns;
        //            }
        //            else
        //            {
        //                oInstBase.oContent[XIConstant.ContentXIComponent] = compI;
        //            }
        //        }
        //        else if (compD.sName.ToLower() == XIConstant.QSComponent.ToLower())
        //        {
        //            var oQSI = (XIIQS)compI.oContent[XIConstant.QSComponent];
        //            if (oQSI.Steps != null && oQSI.Steps.Count() > 0)
        //            {
        //                var oStepI = oQSI.Steps.FirstOrDefault().Value;
        //                var oStepD = (XIDQSStep)oStepI.oDefintion;
        //                if (oStepD.iLayoutID > 0)
        //                {
        //                    var oLayoutIns = (XIInstanceBase)oStepI.oContent[XIConstant.ContentLayout];
        //                    var oLayoutD = (XIDLayout)oLayoutIns.oContent[XIConstant.ContentLayout];
        //                    for (int i = 0; i < oLayoutD.LayoutMappings.Count(); i++)
        //                    {
        //                        MergeHTMLRecurrsive((XIInstanceBase)oLayoutD.LayoutMappings[i].oContent.Values.FirstOrDefault());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (oInstBase.oContent.ContainsKey(XIConstant.ContentDialog))
        //        {
        //            var con = (XIInstanceBase)oInstBase.oContent[XIConstant.ContentDialog];
        //            var xifs = (XIDDialog)con.oContent.Values.FirstOrDefault();
        //            MergeHTMLRecurrsive((XIInstanceBase)xifs.oContent.Values.FirstOrDefault());
        //        }
        //        else if (oInstBase.oContent.ContainsKey(XIConstant.ContentLayout))
        //        {
        //            var oLayoutD = (XIDLayout)oInstBase.oContent[XIConstant.ContentLayout];
        //            for (int i = 0; i < oLayoutD.LayoutMappings.Count(); i++)
        //            {
        //                MergeHTMLRecurrsive((XIInstanceBase)oLayoutD.LayoutMappings[i].oContent.Values.FirstOrDefault());
        //            }
        //        }
        //        else if (oInstBase.oContent.ContainsKey(XIConstant.ContentXILink))
        //        {
        //            var XiLinkD = oInstBase.oContent[XIConstant.ContentXILink];
        //            MergeHTMLRecurrsive((XIInstanceBase)oInstBase.oContent.Values.FirstOrDefault());
        //        }
        //        else if (oInstBase.oContent.ContainsKey(XIConstant.ContentStep))
        //        {
        //            var oStepI = (XIIQSStep)oInstBase.oContent[XIConstant.ContentStep];
        //            if (oStepI.Sections != null && oStepI.Sections.Count() > 0)
        //            {
        //                var oSecI = oStepI.Sections.FirstOrDefault().Value;
        //                var oSecD = (XIDQSSection)oSecI.oDefintion;
        //                var osecIns = new XIInstanceBase();
        //                if (oSecI.oContent.ContainsKey(XIConstant.ContentXIComponent))
        //                {
        //                    osecIns = (XIInstanceBase)oSecI.oContent[XIConstant.ContentXIComponent];
        //                    var compI = (XIIComponent)osecIns.oContent[XIConstant.ContentXIComponent];
        //                    var CompD = (XIDComponent)compI.oDefintion;
        //                    if (CompD.sName.ToLower() == XIConstant.OneClickComponent.ToLower())
        //                    {
        //                        var DATAMerge = MergeHTML("~/views/XIComponents/_OneClickComponent.cshtml", compI);
        //                        compI.oContent[XIConstant.OneClickComponent] = DATAMerge;
        //                        osecIns.oContent[XIConstant.ContentXIComponent] = compI;
        //                    }
        //                    else if (CompD.sName.ToLower() == XIConstant.FormComponent.ToLower())
        //                    {
        //                        var DATAMerge = MergeHTML("~/views/XIComponents/_FormComponent.cshtml", compI);
        //                        compI.oContent[XIConstant.FormComponent] = DATAMerge;
        //                        osecIns.oContent[XIConstant.ContentXIComponent] = compI;
        //                    }
        //                    else if (CompD.sName.ToLower() == XIConstant.XITreeStructure.ToLower())
        //                    {
        //                        var DATAMerge = MergeHTML("~/views/XIComponents/_XITreeStructureView.cshtml", compI);
        //                        compI.oContent[XIConstant.XITreeStructure] = DATAMerge;
        //                        osecIns.oContent[XIConstant.ContentXIComponent] = compI;
        //                    }
        //                    oSecI.oContent[XIConstant.ContentXIComponent] = osecIns;
        //                    oStepI.Sections[oSecD.ID + "_Sec"] = oSecI;
        //                }
        //            }
        //        }

        //    }
        //    return oInstBase;
        //}

        [HttpPost]
        public ActionResult XIContent(string oData)
        {
            string sDatabase = SessionManager.CoreDatabase;
            string sPartialView = string.Empty;
            object oResult = null;
            try
            {
                XIInstanceBase account = JsonConvert.DeserializeObject<XIInstanceBase>(oData);
                var Data = account.oContent.Values.FirstOrDefault();
                if (account.oContent.ContainsKey(XIConstant.ContentXIComponent))
                {
                    var oComponent = Deserialise(oData.ToString());
                    sPartialView = "_XILinkContent";
                    oResult = oComponent.Values.FirstOrDefault();
                }
                if (account.oContent.ContainsKey(XIConstant.ContentLayout))
                {
                    var oLayoutD = JsonConvert.DeserializeObject<XIDLayout>(Data.ToString());
                    foreach (var items in oLayoutD.LayoutMappings)
                    {
                        if (!string.IsNullOrEmpty(items.HTMLCode))
                        {
                            items.oContent["html"] = items.HTMLCode;
                        }
                        else
                        {
                            var Response = Deserialise(items.oContent.Values.FirstOrDefault().ToString());
                            items.oContent = Response;
                        }

                    }
                    sPartialView = "_LayoutData";
                    oResult = oLayoutD;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            return PartialView(sPartialView, oResult);
        }


        public Dictionary<string, object> Deserialise(string sData)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                Dictionary<string, object> oInstanceBase = new Dictionary<string, object>();
                XIInstanceBase oData = JsonConvert.DeserializeObject<XIInstanceBase>(sData);
                if (oData.oContent.ContainsKey(XIConstant.ContentLayout))
                {
                    string oSerlize = oData.oContent[XIConstant.ContentLayout].ToString();
                    var oLayoutD = JsonConvert.DeserializeObject<XIDLayout>(oSerlize.ToString());
                    foreach (var items in oLayoutD.LayoutMappings)
                    {
                        var Response = Deserialise(items.oContent.Values.FirstOrDefault().ToString());
                        items.oContent = Response;
                    }
                    oData.oContent[XIConstant.ContentLayout] = oLayoutD;
                    oInstanceBase[XIConstant.ContentLayout] = oData;
                }
                else if (oData.oContent.ContainsKey(XIConstant.ContentXILink))
                {

                    var oStrXiLink = oData.oContent.Values.FirstOrDefault();
                    var oXiLinkD = JsonConvert.DeserializeObject<XIILink>(oStrXiLink.ToString());
                    if (oXiLinkD.oContent != null)
                    {
                        var oResult3 = Deserialise(oXiLinkD.oContent.Values.FirstOrDefault().ToString());
                        oXiLinkD.oContent = oResult3;
                        oData.oContent[XIConstant.ContentXILink] = oXiLinkD;
                        oInstanceBase[XIConstant.ContentXILink] = oData;
                    }
                }
                else if (oData.oContent.ContainsKey(XIConstant.ContentXIComponent))
                {
                    var oStrXiLink = oData.oContent.Values.FirstOrDefault();
                    var oXICompD = JsonConvert.DeserializeObject<XIIComponent>(oStrXiLink.ToString());
                    if (oXICompD.oContent.ContainsKey(XIConstant.QSComponent))
                    {
                        var oStrQSet = oXICompD.oContent[XIConstant.QSComponent];
                        var oQSI = JsonConvert.DeserializeObject<XIIQS>(oStrQSet.ToString());
                        if (oQSI.Steps != null)
                        {
                            foreach (var oStepI in oQSI.Steps)
                            {
                                if (oStepI.Value != null && oStepI.Value.oContent != null)
                                {
                                    if (oStepI.Value.oContent.ContainsKey(XIConstant.ContentLayout))
                                    {
                                        var oResult3 = Deserialise(oStepI.Value.oContent.Values.FirstOrDefault().ToString());
                                        oStepI.Value.oContent = oResult3;
                                    }
                                }
                            }
                        }


                        oXICompD.oContent[XIConstant.QSComponent] = oQSI;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.OneClickComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.OneClickComponent];
                        var result2 = JsonConvert.DeserializeObject<XID1Click>(result.ToString());
                        oXICompD.oContent[XIConstant.OneClickComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.TabComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.TabComponent];
                        var result2 = JsonConvert.DeserializeObject<List<XIDStructure>>(result.ToString());
                        oXICompD.oContent[XIConstant.TabComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.FormComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.FormComponent];
                        var result2 = JsonConvert.DeserializeObject<XIBODisplay>(result.ToString());
                        oXICompD.oContent[XIConstant.FormComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.MenuComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.MenuComponent];
                        var result2 = JsonConvert.DeserializeObject<List<XIMenu>>(result.ToString());
                        oXICompD.oContent[XIConstant.MenuComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.XITreeStructure))
                    {
                        var result = oXICompD.oContent[XIConstant.XITreeStructure];
                        var result2 = JsonConvert.DeserializeObject<List<XIDStructure>>(result.ToString());
                        oXICompD.oContent[XIConstant.XITreeStructure] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.GridComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.GridComponent];
                        var result2 = JsonConvert.DeserializeObject<XID1Click>(result.ToString());
                        oXICompD.oContent[XIConstant.GridComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                    else if (oXICompD.oContent.ContainsKey(XIConstant.HTMLComponent))
                    {
                        var result = oXICompD.oContent[XIConstant.HTMLComponent];
                        var result2 = JsonConvert.DeserializeObject<XID1Click>(result.ToString());
                        oXICompD.oContent[XIConstant.HTMLComponent] = result2;
                        oData.oContent[XIConstant.ContentXIComponent] = oXICompD;
                        oInstanceBase[XIConstant.ContentXIComponent] = oData;
                    }
                }
                else if (oData.oContent.ContainsKey(XIConstant.ContentStep))
                {
                    var oStrStep = oData.oContent.Values.FirstOrDefault().ToString();
                    var oStepI = JsonConvert.DeserializeObject<XIIQSStep>(oStrStep);
                    if (oStepI.Sections != null && oStepI.Sections.Count() > 0)
                    {
                        foreach (var oSecI in oStepI.Sections)
                        {
                            if (oSecI.Value.oContent != null)
                            {
                                if (oSecI.Value.oContent.ContainsKey(XIConstant.ContentXIComponent))
                                {
                                    var oResult3 = Deserialise(oSecI.Value.oContent.Values.FirstOrDefault().ToString());
                                    oSecI.Value.oContent = oResult3;
                                }
                            }
                        }
                    }
                    else if (oStepI.oContent != null)
                    {
                        if (oStepI.oContent.ContainsKey(XIConstant.ContentLayout))
                        {
                            var oResult3 = Deserialise(oStepI.oContent.Values.FirstOrDefault().ToString());
                            oStepI.oContent = oResult3;
                        }
                    }
                    oData.oContent[XIConstant.ContentStep] = oStepI;
                    oInstanceBase[XIConstant.ContentStep] = oData;
                }
                else if (oData.oContent.ContainsKey(XIConstant.ContentHTML))
                {

                }
                return oInstanceBase;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult MergeHTML(string HTMLPage, object oContent)
        {
            var viewStr = RenderRazorViewToString(HTMLPage, oContent);
            return Content(viewStr);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public ActionResult XIContentLoad(string oData)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = Deserialise(oData);
                //XIInstanceBase oInsBase = JsonConvert.DeserializeObject<XIInstanceBase>(oData);
                //if (oInsBase.oContent.ContainsKey(XIConstant.ContentXILink))
                //{
                //    var xiLink = oInsBase.oContent[XIConstant.ContentXILink].ToString();
                //    var oXILinkD = JsonConvert.DeserializeObject<XILink>(xiLink.ToString());
                //    var Data = oXILinkD.oContent.Values.FirstOrDefault();
                //    XIInstanceBase oInsBase2 = JsonConvert.DeserializeObject<XIInstanceBase>(Data.ToString());
                //    var xiComp = oInsBase2.oContent[XIConstant.ContentXIComponent].ToString();
                //    var oXICompD = JsonConvert.DeserializeObject<XIIComponent>(xiComp.ToString());
                //    var result = oXICompD.oContent["OneClickComponent"];
                //    var result2 = JsonConvert.DeserializeObject<XID1Click>(result.ToString());
                //    oXICompD.oContent["OneClickComponent"] = result2;
                //    oInsBase2.oContent["xicomponent"] = oXICompD;
                //    oXILinkD.oContent["xicomponent"] = oInsBase2;
                //    oInsBase.oContent["xilink"] = oXILinkD;
                //}
                //else if (oInsBase.oContent.ContainsKey(XIConstant.ContentXIComponent))
                //{
                //    var xiComp = oInsBase.oContent[XIConstant.ContentXIComponent].ToString();
                //    var oXICompD = JsonConvert.DeserializeObject<XIIComponent>(xiComp.ToString());
                //}
                //else
                //{
                //    var Data = oInsBase.oContent.Values.FirstOrDefault();
                //    var oLayoutD = JsonConvert.DeserializeObject<XIDLayout>(Data.ToString());
                //    foreach (var items in oLayoutD.LayoutMappings)
                //    {
                //        var fdfsfsf = items.oContent["xilink"].ToString();
                //        var oInstance = JsonConvert.DeserializeObject<XIInstanceBase>(fdfsfsf.ToString());
                //        var Data2 = oInstance.oContent.Values.FirstOrDefault();
                //        var oXiLink = JsonConvert.DeserializeObject<XILink>(Data2.ToString());
                //        var xicomp = oXiLink.oContent["xicomponent"].ToString();
                //        var oXICompD = JsonConvert.DeserializeObject<XIInstanceBase>(xicomp.ToString());
                //        var data3 = oXICompD.oContent.Values.FirstOrDefault();
                //        var xicompD = JsonConvert.DeserializeObject<XIIComponent>(data3.ToString());
                //        var result = xicompD.oContent["OneClickComponent"];
                //        var result2 = JsonConvert.DeserializeObject<XID1Click>(result.ToString());
                //        xicompD.oContent["OneClickComponent"] = result2;
                //        oXICompD.oContent["xicomponent"] = xicompD;
                //        oXiLink.oContent["xicomponent"] = oXICompD;
                //        oInstance.oContent["xilink"] = oXiLink;
                //        items.oContent["xilink"] = oInstance;
                //    }
                //}                
                var oData2 = Result.Values.FirstOrDefault();
                return PartialView("_XILinkContent", oData2);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult PreviewXiLink(int XiLinkID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDefinitionBase oXID = new XIDefinitionBase();
                XIILink oXII = new XIILink();
                oXII.iXILinkID = XiLinkID;
                var oXIPreview = oXII.Preview();
                if (oXIPreview.bOK && oXIPreview.oResult != null)
                {

                    oXID = (XIDefinitionBase)oXIPreview.oResult;
                }
                return PartialView("_XIPreview", oXID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        //public ActionResult LoadTabContent(string sGUID, string TabResultType, string i1ClickID, string sBO, string iInstanceID)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    try
        //    {
        //        var sSessionID = HttpContext.Session.SessionID;
        //        var sBOName = oCache.Get_ParamVal(sSessionID, sGUID, null, "sBOName");
        //        var iInsID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{-iInstanceID}");
        //        var iBODID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBODID");
        //        var xiboins = Singleton.Instance.oFullData[sBOName + "_" + iInsID];
        //        if (TabResultType.ToLower() == "single")
        //        {
        //            if (xiboins != null)
        //            {
        //                var Prodata = xiboins.oSubStructureI(sBOName);
        //                if (Prodata != null && Prodata.oBOIList != null && Prodata.oBOIList.Count() > 0)
        //                {
        //                    var oboi = Prodata.oBOIList.FirstOrDefault();
        //                    XIIComponent oCompI = new XIIComponent();
        //                    XIBODisplay oBOIns = new XIBODisplay();
        //                    oboi.iBODID = Convert.ToInt32(iBODID);
        //                    oboi.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
        //                    oBOIns.BOInstance = oboi;
        //                    oCompI.oContent[XIConstant.FormComponent] = oBOIns;
        //                    return PartialView("~/views/xicomponents/_formcomponent.cshtml", oCompI);
        //                }
        //            }

        //        }
        //        else if (TabResultType.ToLower() == "multiple")
        //        {
        //            XIDComponent oXICompD = new XIDComponent();
        //            oXICompD = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, XIConstant.OneClickComponent, null);
        //            XIDComponent oXICompC = new XIDComponent();
        //            oXICompC = (XIDComponent)oXICompD.Clone(oXICompD);
        //            oXICompC.Params = new List<XIDComponentParam>();
        //            oXICompC.Params.Add(new XIDComponentParam { sName = "1ClickID", sValue = i1ClickID });
        //            XIIComponent oCompI = new XIIComponent();
        //            oCompI.sGUID = sGUID;
        //            oCompI.oDefintion = oXICompC;
        //            var oCompRes = oCompI.Load();
        //            if (oCompRes.bOK && oCompRes.oResult != null)
        //            {
        //                var oXiBase = ((XIIComponent)((XIInstanceBase)oCompRes.oResult).oContent[XIConstant.ContentXIComponent]).oContent[XIConstant.OneClickComponent];
        //                oCompI.oContent[XIConstant.OneClickComponent] = oXiBase;
        //                return PartialView(oXICompC.sHTMLPage, oCompI);
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return null;
        //    }
        //}

        public ActionResult LoadStep(string sResultType, int iQSID, int i1ClickID, string sDefaultStep, int iInstanceID = 0, string sGUID = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            XIIQSStep oStepICont = new XIIQSStep();
            try
            {
                string sStepName = sResultType;
                XIInfraCache oCache = new XIInfraCache();
                XIIQSStep oQSStepI = new XIIQSStep();
                //XIDQSStep oQSStepD = new XIDQSStep();
                XIDQS oQSD = new XIDQS();
                oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSID.ToString());

                if (oQSD != null && oQSD.Steps.Count() > 0)
                {
                    if (oQSD.Steps.ContainsKey(sStepName))
                    {
                        XIDQSStep oQSStepD = oQSD.Steps[sStepName];
                        if (oQSStepD != null)
                        {
                            var result = oQSStepD.Clone(oQSStepD);
                            XIDQSStep oQSStepC = (XIDQSStep)oQSStepD.Clone(oQSStepD);
                            oQSStepI.oDefintion = oQSStepC;
                            oQSStepI.sGUID = sGUID;
                            var oResult = oQSStepI.Load();
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                oStepICont = (XIIQSStep)oResult.oResult;
                                XIInstanceBase oIns = new XIInstanceBase();
                                oIns.oContent[XIConstant.ContentStep] = oStepICont;
                                dynamic oMergedData = MergeHTMLRecurrsive(oIns);
                                var StepData = oMergedData.oContent[XIConstant.ContentStep];
                                return Json(StepData, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                //string json = JsonConvert.SerializeObject(oInstance, Newtonsoft.Json.Formatting.Indented);
                //var oDeserializeData = Deserialise(json);
                //oInstance = (XIInstanceBase)oDeserializeData.Values.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            return Json(oStepICont, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TriggerStep(string sStep, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIQSStep oStepICont = new XIIQSStep();
                int iStepID = 0;
                if (sStep.Contains('_'))
                {
                    var oDetails = sStep.Split('_');
                    if (oDetails != null && oDetails.Count() == 2)
                    {
                        int.TryParse(oDetails[1], out iStepID);
                        if (iStepID > 0)
                        {
                            XIIQSStep oStepI = new XIIQSStep();
                            oStepI.sGUID = sGUID;
                            oStepI.FKiQSStepDefinitionID = iStepID;
                            var oResult = oStepI.Load();
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                oStepICont = (XIIQSStep)oResult.oResult;
                                XIInstanceBase oIns = new XIInstanceBase();
                                oIns.oContent[XIConstant.ContentStep] = oStepICont;
                                dynamic oMergedData = MergeHTMLRecurrsive(oIns);
                                var StepData = oMergedData.oContent[XIConstant.ContentStep];
                                return Json(StepData, JsonRequestBehavior.AllowGet);
                            }
                            return Json(oStepICont, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion XILinkLoad

        public void PostTransactions()
        {
            try
            {
                JournalTransactions jr = new JournalTransactions();
                //jr.PostTransaction("NEWP", 3000, 1059, false);
                //var data = jr.PostTransaction("CREC", 1000, 540, false);
                //var _data = jr.PostTransaction("ACSI", 40, 0, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void PFBNP()
        {
            try
            {
                var cacheobj = HttpRuntime.Cache.GetEnumerator();
                int cachecount = 0;
                List<string> lsterrors = new List<string>();
                List<string> lstkeys = new List<string>();
                long bytes = 0;
                while (cacheobj.MoveNext())
                {
                    var pair = (DictionaryEntry)cacheobj.Current;
                    cachecount++;
                    lstkeys.Add(pair.Key.ToString() + "_" + pair.Value.GetType().ToString());
                    try
                    {
                        using (Stream s = new MemoryStream())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(s, pair.Value);
                            bytes = bytes + s.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        lsterrors.Add(ex.Message.ToString());
                    }

                }
                //XIBNPFileWrapper wrapper = new XIBNPFileWrapper(); // REQUEST FROM IPMSYSTEM
                //XIPFServices wrapper = new XIPFServices();
                //wrapper.PFNewBusiness(new List<CNV>());
                //Dictionary<string, XIPFfields> dictlst = new Dictionary<string, XIPFfields>();
                //string strxml = "<?xml version=\"1.0\" ?><BNPRequest><LiveRateRequestFlag>true</LiveRateRequestFlag></BNPRequest>";
                //dictlst.Add("LiveRateRequestFlag", new XIPFfields { bMandatory = true, sType = "System.Boolean", sFiledName = "LiveRateRequestFlag", sErrorMessage = "$KEY$ Please fill {skey} | $DATA$ Invalid Data for {skey} |$REGEXP$ Please give valid {skey}" });
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.LiveRateQuote, strxml, dictlst);

                string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\NewBusinessReq_test2.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.NewBusiness, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\Cancellation.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.Cancellation, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\EditCustomerAddress.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.EditCustomerAddress, strxml);


                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\EditCustomerBank.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.EditCustomerBank, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\EditCustomerEmail.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.EditCustomerEmail, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\EditCustomerPaymentDay.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.EditCustomerPaymentDay, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\EditCustomerPhone.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.EditCustomerPhone, strxml);

                //string strxml = System.IO.File.ReadAllText(@"E:\Documents\XISystems\BNPRequestXMLs\MidTermAdjustment.txt");
                //var resultobj = wrapper.LoadXIBNPAPI(EnumXIBNPMethods.MidTermAdjustment, strxml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult MakeaChange(string sInstanceID, string sType)
        {
            string sMessage = string.Empty;
            try
            {
                int iUserID = SessionManager.UserID;
                XIInfraNotifications oNotifications = new XIInfraNotifications();
                oNotifications.iUserID = iUserID;
                oNotifications.sInstanceID = sInstanceID;
                oNotifications.iStatus = 10;
                var oResponse = oNotifications.Create(Convert.ToString(iUserID), sType, oNotifications.iDocumentID.ToString());
                if (oResponse.bOK && oResponse.oResult != null)
                {
                    sMessage = "Your request has sent to admin,admin will contact against to your request";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "XIDNA");
                return null;
            }
            return Json(sMessage, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveFiles(int ID, int BOFieldID, string sInstanceID, string sFileAliasName, List<HttpPostedFileBase> UploadImage)
        {
            var sNotUploaded = new List<string>();
            int iStatus = 0;
            int iSavedToDoc = 0;
            int iUserID = SessionManager.UserID;
            string sOrgName = SessionManager.OrganisationName;
            string sDatabase = SessionManager.OrgDatabase;
            int iOrgID = SessionManager.OrganizationID;
            ModelDbContext dbContext = new ModelDbContext();
            //ModelDbContext sdbContext = new ModelDbContext(sDatabase);
            //var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            //BOFields BO = new BOFields();
            string sQuery = "";
            string sValues = string.Empty;
            string sLabelValue = "";
            if (ID > 0 || ID == 0)
            {
                BOFields BODetails = dbContext.BOFields.Find(BOFieldID);
                int FileTypeID = BODetails.FKiFileTypeID;
                int BOID = BODetails.BOID;
                string sFieldName = BODetails.Name;
                string sBOName = dbContext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                string sTableName = dbContext.BOs.Where(m => m.Name == sBOName).Select(m => m.TableName).FirstOrDefault();
                XIFileTypes FileDetails = dbContext.XIFileTypes.Find(FileTypeID);
                string FileType = FileDetails.Type;
                var sDocIDList = new List<string>();
                if (FileType == "10")
                {
                    int iDeletDocIDifNull = 0;
                    foreach (var items in UploadImage)
                    {
                        int iDocID = 0;
                        try
                        {
                            //First save the empty file name and get the docID
                            string sDFileName = "";
                            string sID = string.Empty;
                            XIIBO oBOI = new XIIBO();
                            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T", null); //oXID.Get_BODefinition("Documents_T").oResult;
                            oBOI.BOD = oBOD;
                            oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                            oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sDFileName, bDirty = true };
                            oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = "0", bDirty = true };
                            oBOI.Attributes["dCreatedTime"] = new XIIAttribute { sName = "dCreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                            oBOI.Attributes["dUpdatedTime"] = new XIIAttribute { sName = "dUpdatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                            oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                            oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };

                            var Response = oBOI.Save(oBOI);//to save 
                            XIIBO oBOInstance = new XIIBO();
                            if (Response.bOK && Response.oResult != null)
                            {
                                oBOInstance = (XIIBO)Response.oResult;
                            }
                            if (oBOInstance != null)
                            {
                                sID = oBOInstance.Attributes.Where(x => x.Key.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                            }
                            string sNewImageName = "";
                            if (!string.IsNullOrEmpty(sID))
                            {
                                iDocID = Convert.ToInt32(sID);
                            }
                            iDeletDocIDifNull = iDocID;
                            sLabelValue = iDocID.ToString();
                            sQuery = sQuery + iDocID + ',';
                            //get the details of filename form uploaded file
                            var sImageName = items.FileName;
                            string[] sFormat = sImageName.Split('.');
                            string sImageFormat = sFormat[1];
                            //create a new filename
                            if (iDocID > 0)
                            {
                                sNewImageName = "images_" + iOrgID + "_" + iUserID + "_" + iDocID + "_Org";
                            }
                            string sNewPathForSubDir = "";
                            int iDocTypeID = 0;
                            string sNewPath = "";
                            List<CNV> oNVList = new List<CNV>();
                            CNV oNV = new CNV();
                            oNV.sName = "";
                            oNV.sValue = sImageFormat;
                            oNVList.Add(oNV);
                            List<XIDocTypes> DocTypes = dbContext.XIDocTypes.Where(m => m.Type == sImageFormat).ToList();
                            int iFileTypeID = Convert.ToInt32(FileDetails.FileType);
                            string sFileTypeCheck = dbContext.XIDocTypes.Where(m => m.ID == iFileTypeID).Select(m => m.Type).FirstOrDefault();
                            //Check if the file format matches the doctype details
                            if (sFileTypeCheck.ToLower() != sImageFormat.ToLower())
                            {
                                //do nothing as file format dosnot match
                                sNotUploaded.Add(items.FileName);
                            }
                            else
                            {
                                List<string> sNewPathDetails = CheckAndCreateDirectory(DocTypes);
                                for (var i = 0; i < sNewPathDetails.Count(); i++)
                                {
                                    iDocTypeID = Convert.ToInt32(sNewPathDetails[0]);
                                    sNewPathForSubDir = sNewPathDetails[1];
                                    sNewPath = sNewPathDetails[2];
                                }

                                items.SaveAs(sNewPath + "\\" + sNewImageName + "." + sImageFormat);
                                //Aspect ratio
                                //Get max and min height of image from xi filesetttings
                                var iMaxWidth = Convert.ToInt32(FileDetails.MaxWidth);
                                var iMaxHeight = Convert.ToInt32(FileDetails.MaxHeight);
                                using (var image = Image.FromFile(sNewPath + "\\" + sNewImageName + "." + sImageFormat))
                                using (var newImage = ScaleImage(image, iMaxWidth, iMaxHeight))
                                {
                                    string sImgNme = sNewImageName.Replace("_Org", "");
                                    newImage.Save(sNewPath + "\\" + sImgNme + "." + sImageFormat);
                                }
                                try
                                {
                                    //check DocID and update the details
                                    if (iDocID > 0)
                                    {
                                        oBOI = new XIIBO();
                                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T", null);
                                        oBOI.BOD = oBOD;
                                        oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iDocID.ToString(), bDirty = true };
                                        oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sNewImageName.Replace("_Org", ""), bDirty = true };
                                        oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = iDocTypeID.ToString(), bDirty = true };
                                        oBOI.Attributes["SubDirectoryPath"] = new XIIAttribute { sName = "SubDirectoryPath", sValue = sNewPathForSubDir, bDirty = true };
                                        oBOI.Attributes["iSystemgeneratedorUploaded"] = new XIIAttribute { sName = "iSystemgeneratedorUploaded", sValue = "20", bDirty = true };
                                        if (!string.IsNullOrEmpty(sFileAliasName))
                                        {
                                            oBOI.Attributes["sAliasName"] = new XIIAttribute { sName = "sAliasName", sValue = sFileAliasName + "." + sImageFormat, bDirty = true };
                                        }
                                        var Result = oBOI.Save(oBOI);
                                        iSavedToDoc = 1;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                //if (FileType == "10")
                                //{
                                if (iSavedToDoc == 1)
                                {
                                    if (FileDetails.Thumbnails == "10")
                                    {
                                        int iThHeight = Convert.ToInt32(FileDetails.ThumbHeight);
                                        int iThWidth = Convert.ToInt32(FileDetails.ThumbWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + iOrgID + "_" + iUserID + "_" + iDocID + "_Org." + sImageFormat);
                                        using (var thumbImage = ThumbImage(image, iThWidth, iThHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            thumbImage.Save(sNewPath + "\\" + sImgNme + "_thumb." + sImageFormat);
                                        }

                                    }

                                    if (FileDetails.Preview == "10")
                                    {
                                        int iPrevHeight = Convert.ToInt32(FileDetails.PreviewHeight);
                                        int iPrevWidth = Convert.ToInt32(FileDetails.PreviewWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + iOrgID + "_" + iUserID + "_" + iDocID + "." + sImageFormat);
                                        // Image prev = image.GetThumbnailImage(iPrevHeight, iPrevWidth, () => false, IntPtr.Zero);
                                        //thumb.Save(Path.ChangeExtension(sPath + "\\" + "images_" + OrgID + "_" + ID + "." + sImageFormat, "thumb"));
                                        // prev.Save(sPath + "\\" + sNewImageName + "_prev." + sImageFormat);
                                        using (var newImage = ScaleImage(image, iPrevWidth, iPrevHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            newImage.Save(sNewPath + "\\" + sImgNme + "_prev." + sImageFormat);
                                        }
                                    }

                                    if (FileDetails.Drilldown == "10")
                                    {
                                        int iDrillHeight = Convert.ToInt32(FileDetails.DrillHeight);
                                        int iDrillWidth = Convert.ToInt32(FileDetails.DrillWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + iOrgID + "_" + iUserID + "_" + iDocID + "." + sImageFormat);
                                        using (var newImage = ScaleImage(image, iDrillWidth, iDrillHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            newImage.Save(sNewPath + "\\" + sImgNme + "_drill." + sImageFormat);
                                        }
                                    }
                                }
                            }
                            //Add Doc ID where the image name is saved to a list.
                            sDocIDList.Add(iDocID.ToString());
                        }//try

                        catch (Exception ex)
                        {
                            //if (iDocID > 0)
                            //{
                            //    XIDocs Doc = sdbContext.XIDocs.Find(iDocID);
                            //    sdbContext.XIDocs.Remove(Doc);
                            //    sdbContext.SaveChanges();
                            //}
                        }
                    }//end upload image for
                    //save the Doc ID where the file name is stored to perticular table
                    //check the not uploaded files
                    if (sNotUploaded.Count == 0)
                    {

                        if (ID != 0)
                        {
                            //check if the table with field has a image ID.
                            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAdbContext"].ConnectionString);
                            Con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = Con;
                            //if (sTableName != "Reports")
                            //{
                            //    //Con.ChangeDatabase(sOrgDB);
                            //}
                            cmd.CommandText = "SELECT " + sFieldName + " FROM " + sTableName + " WHERE ID=" + ID;
                            SqlDataReader reader = cmd.ExecuteReader();
                            string sDocID = "";
                            while (reader.Read())
                            {
                                sDocID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                            }
                            Con.Close();

                            string sNewDocID = "";
                            if (sDocID != null)
                            {
                                string sDocIDs = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                sNewDocID = sDocID + "," + sDocIDs;
                            }
                            else
                            {
                                sNewDocID = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                            }

                            Con.Open();
                            SqlCommand cmd1 = new SqlCommand();
                            cmd1.Connection = Con;
                            //if (sTableName != "Reports")
                            //{
                            //    Con.ChangeDatabase(sOrgDB);
                            //}
                            cmd1.CommandText = "UPDATE " + sTableName + " SET " + sFieldName + "='" + sNewDocID + "' WHERE ID=" + ID;
                            cmd1.ExecuteNonQuery();
                            Con.Close();
                            iStatus = 1;
                        }
                    }
                    else
                    {
                        iStatus = 0;
                        if (iDeletDocIDifNull != 0)
                        {
                            //XIDocs Doc = sdbContext.XIDocs.Find(iDeletDocIDifNull);
                            //sdbContext.XIDocs.Remove(Doc);
                            //sdbContext.SaveChanges();
                        }

                    }

                }
                else if (FileType == "20")
                {
                    string sID = string.Empty;
                    int iDeleteDocIfNull = 0;
                    foreach (var items in UploadImage)
                    {
                        int iDocID = 0;
                        try
                        {
                            //First save the empty file name and get the docID
                            string sDFileName = "";
                            //XIDocs Docs = new XIDocs();
                            //Docs.FileName = sDFileName;
                            //Docs.FKiDocType = 0;
                            //Docs.FKiUserID = iUserID;
                            //Docs.dCreatedTime = DateTime.Now;
                            //Docs.dUpdatedTime = DateTime.Now;
                            //if (!string.IsNullOrEmpty(sInstanceID))
                            //{
                            //    Docs.FKiACPolicyID = Convert.ToInt32(sInstanceID);
                            //}
                            ////dbContext = new ModelDbContext(sDatabase);
                            //sdbContext.XIDocs.Add(Docs);
                            //sdbContext.SaveChanges();


                            XIIBO oBOI = new XIIBO();
                            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T", null);
                            oBOI.BOD = oBOD;
                            oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                            oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sDFileName, bDirty = true };
                            oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = "0", bDirty = true };
                            oBOI.Attributes["dCreatedTime"] = new XIIAttribute { sName = "dCreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                            oBOI.Attributes["dUpdatedTime"] = new XIIAttribute { sName = "dUpdatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                            oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                            oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };

                            var Response = oBOI.Save(oBOI);//to save 
                            XIIBO oBOInstance = new XIIBO();
                            if (Response.bOK && Response.oResult != null)
                            {
                                oBOInstance = (XIIBO)Response.oResult;
                            }
                            if (oBOInstance != null)
                            {
                                sID = oBOInstance.Attributes.Where(x => x.Key.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(sID))
                            {
                                iDocID = Convert.ToInt32(sID);
                            }
                            sQuery = sQuery + iDocID + ',';
                            iDeleteDocIfNull = iDocID;
                            //get the details of filename form uploaded file
                            var sFileName = items.FileName;
                            string[] sFormat = sFileName.Split('.');
                            string sImageFormat = sFormat[1];
                            //create a new filename
                            string sNewFileName = "";
                            if (iDocID > 0)
                            {
                                sNewFileName = "file_" + iOrgID + "_" + iUserID + "_" + iDocID;
                            }
                            string sNewPathForSubDir = "";
                            int iDocTypeID = 0;
                            string sNewPath = "";
                            List<XIDocTypes> DocTypes = dbContext.XIDocTypes.Where(m => m.Type.ToLower() == sImageFormat.ToLower()).ToList();
                            int iFileTypeID = Convert.ToInt32(FileDetails.FileType);
                            string sFileTypeCheck = dbContext.XIDocTypes.Where(m => m.ID == iFileTypeID).Select(m => m.Type).FirstOrDefault();
                            //Check if the file format matches the doctype details
                            //if (sFileTypeCheck.ToLower() != sImageFormat.ToLower())
                            //{
                            //    //do nothing as file format dosnot match
                            //    sNotUploaded.Add(items.FileName);
                            //}
                            //else
                            //{
                            //call method to check and create directory
                            List<string> sNewPathDetails = CheckAndCreateDirectory(DocTypes);
                            for (var i = 0; i < sNewPathDetails.Count(); i++)
                            {
                                iDocTypeID = Convert.ToInt32(sNewPathDetails[0]);
                                sNewPathForSubDir = sNewPathDetails[1];
                                sNewPath = sNewPathDetails[2];

                            }
                            items.SaveAs(sNewPath + "\\" + sNewFileName + "." + sImageFormat);
                            try
                            {
                                //check DocID and update the details
                                if (iDocID > 0)
                                {
                                    oBOI = new XIIBO();
                                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T", null);
                                    oBOI.BOD = oBOD;
                                    oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = iDocID.ToString(), bDirty = true };
                                    oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sNewFileName + "." + sImageFormat, bDirty = true };
                                    oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = iDocTypeID.ToString(), bDirty = true };
                                    oBOI.Attributes["SubDirectoryPath"] = new XIIAttribute { sName = "SubDirectoryPath", sValue = sNewPathForSubDir, bDirty = true };
                                    oBOI.Attributes["iSystemgeneratedorUploaded"] = new XIIAttribute { sName = "iSystemgeneratedorUploaded", sValue = "20", bDirty = true };
                                    oBOI.Attributes["sFullPath"] = new XIIAttribute { sName = "sFullPath", sValue = sNewPathForSubDir + "/" + sNewFileName + "." + sImageFormat, bDirty = true };
                                    if (!string.IsNullOrEmpty(sFileAliasName))
                                    {
                                        oBOI.Attributes["sAliasName"] = new XIIAttribute { sName = "sAliasName", sValue = sFileAliasName + "." + sImageFormat, bDirty = true };
                                    }
                                    var Result = oBOI.Save(oBOI);
                                    iSavedToDoc = 1;
                                }
                            }
                            catch (Exception ex)
                            {
                                //do nothin
                            }
                            if (iSavedToDoc == 1)
                            {
                                //for now no preview
                            }
                            //}
                            //Add Doc ID where the image name is saved to a list.
                            sDocIDList.Add(iDocID.ToString());
                        }//try

                        catch (Exception ex)
                        {
                            if (iDocID > 0)
                            {
                                //XIDocs Doc = sdbContext.XIDocs.Find(iDocID);
                                //sdbContext.XIDocs.Remove(Doc);
                                //sdbContext.SaveChanges();
                            }
                        }
                    }//end upload image for

                    //save the Doc ID where the file name is stored to perticular table
                    //check the not uploaded files
                    if (sNotUploaded.Count == 0)
                    {

                        if (ID != 0)
                        {
                            //check if the table with field has a image ID.
                            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAdbContext"].ConnectionString);
                            Con.Open();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = Con;
                            //if (sTableName != "Reports")
                            //{
                            //    Con.ChangeDatabase(sOrgDB);
                            //}
                            cmd.CommandText = "SELECT " + sFieldName + " FROM " + sTableName + " WHERE ID=" + ID;
                            SqlDataReader reader = cmd.ExecuteReader();
                            string sDocID = "";
                            while (reader.Read())
                            {
                                sDocID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                            }
                            Con.Close();

                            string sNewDocID = "";
                            if (sDocID != null)
                            {
                                string sDocIDs = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                sNewDocID = sDocID + "," + sDocIDs;
                            }
                            else
                            {
                                sNewDocID = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                            }

                            Con.Open();
                            SqlCommand cmd1 = new SqlCommand();
                            cmd1.Connection = Con;
                            //if (sTableName != "Reports")
                            //{
                            //    Con.ChangeDatabase(sOrgDB);
                            //}
                            cmd1.CommandText = "UPDATE " + sTableName + " SET " + sFieldName + "='" + sNewDocID + "' WHERE ID=" + ID;
                            cmd1.ExecuteNonQuery();
                            Con.Close();
                            iStatus = 1;
                        }
                    }
                    else
                    {
                        iStatus = 0;
                        if (iDeleteDocIfNull != 0)
                        {
                            //XIDocs Doc = sdbContext.XIDocs.Find(iDeleteDocIfNull);
                            //sdbContext.XIDocs.Remove(Doc);
                            //sdbContext.SaveChanges();
                        }
                    }
                }
                else
                {
                    //save the image as Blob
                }
            }
            if (!string.IsNullOrEmpty(sQuery))
            {
                sQuery = sQuery.Substring(0, sQuery.Length - 1);
            }
            return Content(sQuery.ToString());
            //return Json(sQuery.ToString(), JsonRequestBehavior.AllowGet);
        }
        //Calculate the aspect ratio and recreate the image
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        //thumbnail
        public static Image ThumbImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            Image thumb = image.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
            return thumb;
        }

        //Create dir

        public List<string> CheckAndCreateDirectory(List<XIDocTypes> DocTypes)
        {
            string physicalPath = "";
            string sPath = "";
            string sFilePath = "";
            int iDocTypeID = 0;
            string sSubDirectory = "";
            string sNewPath = "";
            string sNewPathForSubDir = "";
            var lPathDetails = new List<string>();
            foreach (var DTypes in DocTypes)
            {
                sFilePath = DTypes.Path;
                iDocTypeID = DTypes.ID;
                lPathDetails.Add(iDocTypeID.ToString());
                sSubDirectory = DTypes.SubDirectory.ToLower();
                if (sSubDirectory == "year/month/day")
                {
                    //check if sub directory has "/"
                    if ((sSubDirectory.Contains("/")))
                    {
                        sSubDirectory = sSubDirectory.Replace(@"\", "/");
                        string sSubDirCsV = sSubDirectory.Replace("/", ",").TrimStart();
                        List<string> sSubDirList = sSubDirCsV.Split(',').ToList();
                        List<string> sNewSubDirPath = new List<string>(); ;
                        foreach (var DirNames in sSubDirList)
                        {
                            string sVal = "";
                            DateTime DateTme = DateTime.Now;
                            if (DirNames.ToLower() == "year")
                            {
                                sVal = DateTme.Year.ToString();
                            }
                            else if (DirNames.ToLower() == "month")
                            {
                                sVal = DateTme.Month.ToString();
                            }
                            else if (DirNames.ToLower() == "day")
                            {
                                sVal = DateTme.Day.ToString();
                            }
                            else
                            {
                                sVal = DirNames;
                            }
                            sNewSubDirPath.Add(sVal);
                        }//for
                        physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                        sPath = physicalPath.Substring(0, physicalPath.Length) + "\\" + sFilePath;
                        if (sPath.Contains("~/"))
                        {
                            sPath = sPath.Replace("~/", "");
                        }

                        if (sPath.Contains('/'))
                        {
                            sPath = sPath.Replace("/", "\\");
                        }
                        ////Save the new created sub dir path to "XI Doc Settings"
                        string sNewSubDirPathCSV = String.Join(",", sNewSubDirPath.Select(x => x.ToString()).ToArray());
                        sNewPathForSubDir = sNewSubDirPathCSV.Replace(",", "/");
                        //Add subdirpath to list
                        lPathDetails.Add(sNewPathForSubDir);
                        foreach (var sNwSubdir in sNewSubDirPath)
                        {
                            //sNewPath = "";
                            if (sNewPath == "" || sNewPath == null)
                            {
                                sNewPath = sPath + "\\" + sNwSubdir;
                            }
                            else
                            {
                                sNewPath = sNewPath + "\\" + sNwSubdir;
                            }

                            if (Directory.Exists(sNewPath))
                            {

                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(sNewPath);
                            }
                        }
                    }//sub dir
                    lPathDetails.Add(sNewPath);
                }//sub=10;
                else
                {

                    physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    sPath = physicalPath.Substring(0, physicalPath.Length) + sFilePath;
                    if (sPath.Contains("~/"))
                    {
                        sPath = sPath.Replace("~/", "");
                    }

                    if (sPath.Contains('/'))
                    {
                        sPath = sPath.Replace("/", "\\");
                    }
                    sNewPath = sPath;
                    if (Directory.Exists(sNewPath))
                    {

                    }
                    else
                    {
                        var createDir = System.IO.Directory.CreateDirectory(sNewPath);
                    }
                    lPathDetails.Add(sNewPath);
                }
            }
            return lPathDetails;
        }
        #region LexisNexis
        //public ActionResult CheckLexisNexis()
        //{
        //    Policy oPolicy = new Policy();
        //    var oResponse = oPolicy.TestLexisNexisResult();
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}
        public string CheckCSVGeneration()
        {
            Policy oPolicy = new Policy();
            List<CNV> oParams = new List<CNV>();
            CNV oCNV = new CNV();
            oCNV.sName = "sBOName";
            oCNV.sValue = "ACPolicy_T";
            oParams.Add(oCNV);
            oCNV = new CNV();
            oCNV.sName = "iInstanceID";
            oCNV.sValue = "23150";//23053//23113
            oParams.Add(oCNV);
            oCNV = new CNV();
            oCNV.sName = "sTemplate";
            oCNV.sValue = "Policy CSV";
            oParams.Add(oCNV);
            oCNV = new CNV();
            oCNV.sName = "sStructure";
            oCNV.sValue = "MTA Copy";
            oParams.Add(oCNV);
            var oResponse = oPolicy.GenerateCSVFile(oParams);
            var sResult = (string)oResponse.oResult;
            return sResult;
        }
        public ActionResult CheckDocumentGeneration()
        {
            Policy oPolicy = new Policy();
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = "sGUID", sValue = Guid.NewGuid().ToString() });
            oParams.Add(new CNV { sName = "sDataBase", sValue = SessionManager.CoreDatabase });
            oParams.Add(new CNV { sName = "iUserID", sValue = SessionManager.UserID.ToString() });
            oParams.Add(new CNV { sName = "srolename", sValue = SessionManager.sRoleName });
            oParams.Add(new CNV { sName = "sOrgDatabase", sValue = SessionManager.OrgDatabase });
            oParams.Add(new CNV { sName = "sOrgName", sValue = SessionManager.OrganisationName });
            oParams.Add(new CNV { sName = "iOrganizationID", sValue = SessionManager.OrganizationID.ToString() });
            oParams.Add(new CNV { sName = "sBOName", sValue = "ACPolicy_T" });
            oParams.Add(new CNV { sName = "iInstanceID", sValue = "23053" });
            oParams.Add(new CNV { sName = "iProductID", sValue = "1" });
            oPolicy.GeneratePolicyDocuments(oParams);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcel()
        {
            var gv = new System.Web.UI.WebControls.GridView();
            gv.DataSource = this.CheckCSVGeneration();
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckPolicyIDUpdate()
        {
            Policy oPolicy = new Policy();
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = "iACPolicyID", sValue = "23053" });
            oParams.Add(new CNV { sName = "sQSInstanceID", sValue = "10420" });
            oPolicy.UpdatePolicyIDtoBOI(oParams);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateBordereau()
        {
            try
            {
                //int iOneClickID = 2429;
                string sOneClickName = "Generate Bordereau";
                XID1Click o1ClickD = new XID1Click();
                if (!string.IsNullOrEmpty(sOneClickName))
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    Dictionary<string, XIIBO> oRes = o1ClickD.OneClick_Execute();
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBOIList = oRes.Values.ToList();
                    List<string> oEDIData = new List<string>();
                    if (oBOIList != null && oBOIList.Count() > 0)
                    {
                        string sMergedTextHeader = oBOIList.FirstOrDefault().AttributeI("sMergedTextHeader").sValue;
                        DataTable dt = new DataTable();
                        List<string> sInstanceIDList = new List<string>();
                        if (!string.IsNullOrEmpty(sMergedTextHeader))
                        {
                            oEDIData.Add(sMergedTextHeader);
                        }
                        foreach (var oBOI in oBOIList)
                        {
                            string sMergedContent = oBOI.AttributeI("MergedText").sValue;
                            string sInstanceID = oBOI.AttributeI("iInstanceID").sValue;
                            sInstanceIDList.Add(sInstanceID);
                            oEDIData.Add(sMergedContent);
                        }
                        string sFinalString = string.Join("\r\n", oEDIData);
                        string sInstanceIDs = string.Join(",", sInstanceIDList);
                        string sFileName = "KGM Bordereau";
                        var oResponse = ExportsToCSV(sFinalString, sFileName);
                        if (oResponse.bOK)
                        {
                            XIDBO oBOD = new XIDBO();
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Bordereau_T");
                            XIIBO oBOI = new XIIBO();
                            oBOI.BOD = oBOD;
                            oBOI.SetAttribute("sBordereauFileName", sFileName);
                            oBOI.SetAttribute("iStatus", "10");
                            var Response = oBOI.Save(oBOI);
                            if (Response.bOK && Response.oResult != null)
                            {
                                oBOI = (XIIBO)Response.oResult;
                                var sBordereauID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).FirstOrDefault().sValue;
                                oBOD = new XIDBO();
                                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BordereauDetails_T");
                                oBOI = new XIIBO();
                                oBOI.BOD = oBOD;
                                oBOI.SetAttribute("FKiBordereauID", sBordereauID);
                                oBOI.SetAttribute("sInstanceIDs", sInstanceIDs);
                                var oBOIResponse = oBOI.Save(oBOI);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public CResult ExportsToCSV(string sResult, string sFileName)
        {
            CResult oCResult = new CResult();
            try
            {
                sFileName = sFileName + ".csv";
                string sCurrentDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string filePath = physicalPath.Substring(0, physicalPath.Length) + "content//BordereauFiles//" + sCurrentDate + "";
                // Check if file already exists. If yes, delete it.     
                //if (File.Exists(filePath))
                //{
                //    File.Delete(filePath);
                //}
                if (Directory.Exists(filePath))
                {

                }
                else
                {
                    var createDir = System.IO.Directory.CreateDirectory(filePath);
                }
                System.IO.File.WriteAllText(filePath + "//" + sFileName, sResult.ToString());
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                //SaveErrortoDB(oCResult);
                // oCResult.LogToFile();
            }
            return oCResult;
        }
        public ActionResult GenerateBordereauWithAddons()
        {
            try
            {
                //int iOneClickID = 2507;
                string sOneClickName = "Generate Broadreau WithAddons";
                XID1Click o1ClickD = new XID1Click();
                if (!string.IsNullOrEmpty(sOneClickName))
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    Dictionary<string, XIIBO> oRes = o1ClickD.OneClick_Execute();
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBOIList = oRes.Values.ToList();
                    if (oBOIList != null && oBOIList.Count() > 0)
                    {
                        if (oBOIList.Any(x => x.Attributes.ContainsKey("FKiAddonID")))
                        {
                            var oProductGroupList = oBOIList.Where(x => string.IsNullOrEmpty(x.AttributeI("FKiAddonID").sValue)).ToList();
                            var oAddonGroupList = oBOIList.Where(x => !string.IsNullOrEmpty(x.AttributeI("FKiAddonID").sValue) && x.AttributeI("FKiAddonID").sValue != "0").ToList();
                            if (oProductGroupList != null && oProductGroupList.Count() > 0)
                            {
                                if (oProductGroupList.Any(x => x.Attributes.ContainsKey("FKiProductID")))
                                {
                                    List<string> oProductIDs = oProductGroupList.Select(x => x.AttributeI("FKiProductID").sValue).ToList();
                                    List<string> oDistinctProductIDs = new List<string>();
                                    oDistinctProductIDs.AddRange(oProductIDs.Distinct());
                                    foreach (var product in oDistinctProductIDs)
                                    {
                                        string sFileName = string.Empty;
                                        var oProductI = oIXI.BOI("Product", product);
                                        if (oProductI != null && oProductI.Attributes.ContainsKey("sName"))
                                        {
                                            sFileName = oProductI.Attributes["sName"].sValue;
                                        }
                                        var oProductGroupBOIList = oProductGroupList.Where(x => x.AttributeI("FKiProductID").sValue == product).ToList();
                                        if (oProductGroupBOIList != null && oProductGroupBOIList.Count() > 0)
                                        {
                                            SaveBroadreau(oProductGroupBOIList, sFileName);
                                        }
                                    }
                                }
                            }
                            if (oAddonGroupList != null && oAddonGroupList.Count() > 0)
                            {
                                if (oAddonGroupList.Any(x => x.Attributes.ContainsKey("FKiAddonID")))
                                {
                                    List<string> oAddonIDs = oAddonGroupList.Select(x => x.AttributeI("FKiAddonID").sValue).ToList();
                                    List<string> oDistinctAddonIDs = new List<string>();
                                    oDistinctAddonIDs.AddRange(oAddonIDs.Distinct());
                                    foreach (var addon in oDistinctAddonIDs)
                                    {
                                        string sFileName = string.Empty;
                                        var oAddonI = oIXI.BOI("refAddon_T", addon);
                                        if (oAddonI != null && oAddonI.Attributes.ContainsKey("sName"))
                                        {
                                            sFileName = oAddonI.Attributes["sName"].sValue;
                                        }
                                        var oAddonGroupBOIList = oAddonGroupList.Where(x => x.AttributeI("FKiAddonID").sValue == addon).ToList();
                                        if (oAddonGroupBOIList != null && oAddonGroupBOIList.Count() > 0)
                                        {
                                            SaveBroadreau(oAddonGroupBOIList, sFileName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        private void SaveBroadreau(List<XIIBO> oList, string sFileName)
        {
            try
            {
                List<string> oEDIData = new List<string>();
                string sMergedTextHeader = string.Empty;
                if (oList.FirstOrDefault().Attributes.ContainsKey("sMergedTextHeader"))
                {
                    sMergedTextHeader = oList.FirstOrDefault().AttributeI("sMergedTextHeader").sValue;
                }
                List<string> sInstanceIDList = new List<string>();
                if (!string.IsNullOrEmpty(sMergedTextHeader))
                {
                    oEDIData.Add(sMergedTextHeader);
                }
                foreach (var oBOI in oList)
                {
                    string sMergedContent = string.Empty; string sInstanceID = string.Empty;
                    if (oBOI.Attributes.ContainsKey("MergedText"))
                    {
                        sMergedContent = oBOI.AttributeI("MergedText").sValue;
                    }
                    if (oBOI.Attributes.ContainsKey("iInstanceID"))
                    {
                        sInstanceID = oBOI.AttributeI("iInstanceID").sValue;
                    }
                    sInstanceIDList.Add(sInstanceID);
                    if (!string.IsNullOrEmpty(sMergedContent))
                    {
                        oEDIData.Add(sMergedContent);
                    }
                }
                string sFinalString = string.Empty;
                if (oEDIData != null && oEDIData.Count() > 0)
                {
                    sFinalString = string.Join("\r\n", oEDIData);
                }
                string sInstanceIDs = string.Join(",", sInstanceIDList);
                //string sFileName = "KGM Bordereau";
                if (!string.IsNullOrEmpty(sFinalString))
                {
                    var oResponse = ExportsToCSV(sFinalString, sFileName);
                    if (oResponse.bOK)
                    {
                        XIDBO oBOD = new XIDBO();
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Bordereau_T");
                        XIIBO oBOI = new XIIBO();
                        oBOI.BOD = oBOD;
                        oBOI.SetAttribute("sBordereauFileName", sFileName);
                        oBOI.SetAttribute("iStatus", "10");
                        var Response = oBOI.Save(oBOI);
                        if (Response.bOK && Response.oResult != null)
                        {
                            oBOI = (XIIBO)Response.oResult;
                            var sBordereauID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).FirstOrDefault().sValue;
                            oBOD = new XIDBO();
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BordereauDetails_T");
                            oBOI = new XIIBO();
                            oBOI.BOD = oBOD;
                            oBOI.SetAttribute("FKiBordereauID", sBordereauID);
                            oBOI.SetAttribute("sInstanceIDs", sInstanceIDs);
                            var oBOIResponse = oBOI.Save(oBOI);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
            }
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Reconcilliation(int ReconciliationID)
        {
            try
            {
                Reconcilliation oRec = new Reconcilliation();
                var oCResult = oRec.ReverseReconciliation(ReconciliationID);
                if (oCResult.iTraceLevel == 1)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CancelTransaction(string XILinkID, string sGUID)
        {
            try
            {
                XILinkLoadJson(Convert.ToInt32(XILinkID), sGUID);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenerateBordereauWithAddons()
        {
            try
            {
                //int iOneClickID = 2507;
                string sOneClickName = "Generate Broadreau WithAddons";
                XID1Click o1ClickD = new XID1Click();
                if (!string.IsNullOrEmpty(sOneClickName))
                {
                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    Dictionary<string, XIIBO> oRes = o1ClickD.OneClick_Execute();
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBOIList = oRes.Values.ToList();
                    if (oBOIList != null && oBOIList.Count() > 0)
                    {
                        if (oBOIList.Any(x => x.Attributes.ContainsKey("FKiAddonID")))
                        {
                            var oProductGroupList = oBOIList.Where(x => string.IsNullOrEmpty(x.AttributeI("FKiAddonID").sValue)).ToList();
                            var oAddonGroupList = oBOIList.Where(x => !string.IsNullOrEmpty(x.AttributeI("FKiAddonID").sValue) && x.AttributeI("FKiAddonID").sValue != "0").ToList();
                            if (oProductGroupList != null && oProductGroupList.Count() > 0)
                            {
                                if (oProductGroupList.Any(x => x.Attributes.ContainsKey("FKiProductID")))
                                {
                                    List<string> oProductIDs = oProductGroupList.Select(x => x.AttributeI("FKiProductID").sValue).ToList();
                                    List<string> oDistinctProductIDs = new List<string>();
                                    oDistinctProductIDs.AddRange(oProductIDs.Distinct());
                                    foreach (var product in oDistinctProductIDs)
                                    {
                                        string sFileName = string.Empty;
                                        var oProductI = oIXI.BOI("Product", product);
                                        if (oProductI != null && oProductI.Attributes.ContainsKey("sName"))
                                        {
                                            sFileName = oProductI.Attributes["sName"].sValue;
                                        }
                                        var oProductGroupBOIList = oProductGroupList.Where(x => x.AttributeI("FKiProductID").sValue == product).ToList();
                                        if (oProductGroupBOIList != null && oProductGroupBOIList.Count() > 0)
                                        {
                                            SaveBroadreau(oProductGroupBOIList, sFileName);
                                        }
                                    }
                                }
                            }
                            if (oAddonGroupList != null && oAddonGroupList.Count() > 0)
                            {
                                if (oAddonGroupList.Any(x => x.Attributes.ContainsKey("FKiAddonID")))
                                {
                                    List<string> oAddonIDs = oAddonGroupList.Select(x => x.AttributeI("FKiAddonID").sValue).ToList();
                                    List<string> oDistinctAddonIDs = new List<string>();
                                    oDistinctAddonIDs.AddRange(oAddonIDs.Distinct());
                                    foreach (var addon in oDistinctAddonIDs)
                                    {
                                        string sFileName = string.Empty;
                                        var oAddonI = oIXI.BOI("refAddon_T", addon);
                                        if (oAddonI != null && oAddonI.Attributes.ContainsKey("sName"))
                                        {
                                            sFileName = oAddonI.Attributes["sName"].sValue;
                                        }
                                        var oAddonGroupBOIList = oAddonGroupList.Where(x => x.AttributeI("FKiAddonID").sValue == addon).ToList();
                                        if (oAddonGroupBOIList != null && oAddonGroupBOIList.Count() > 0)
                                        {
                                            SaveBroadreau(oAddonGroupBOIList, sFileName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private void SaveBroadreau(List<XIIBO> oList, string sFileName)
        {
            try
            {
                List<string> oEDIData = new List<string>();
                string sMergedTextHeader = string.Empty;
                if (oList.FirstOrDefault().Attributes.ContainsKey("sMergedTextHeader"))
                {
                    sMergedTextHeader = oList.FirstOrDefault().AttributeI("sMergedTextHeader").sValue;
                }
                List<string> sInstanceIDList = new List<string>();
                if (!string.IsNullOrEmpty(sMergedTextHeader))
                {
                    oEDIData.Add(sMergedTextHeader);
                }
                foreach (var oBOI in oList)
                {
                    string sMergedContent = string.Empty; string sInstanceID = string.Empty;
                    if (oBOI.Attributes.ContainsKey("MergedText"))
                    {
                        sMergedContent = oBOI.AttributeI("MergedText").sValue;
                    }
                    if (oBOI.Attributes.ContainsKey("iInstanceID"))
                    {
                        sInstanceID = oBOI.AttributeI("iInstanceID").sValue;
                    }
                    sInstanceIDList.Add(sInstanceID);
                    if (!string.IsNullOrEmpty(sMergedContent))
                    {
                        oEDIData.Add(sMergedContent);
                    }
                }
                string sFinalString = string.Empty;
                if (oEDIData != null && oEDIData.Count() > 0)
                {
                    sFinalString = string.Join("\r\n", oEDIData);
                }
                string sInstanceIDs = string.Join(",", sInstanceIDList);
                //string sFileName = "KGM Bordereau";
                if (!string.IsNullOrEmpty(sFinalString))
                {
                    var oResponse = ExportsToCSV(sFinalString, sFileName);
                    if (oResponse.bOK)
                    {
                        XIDBO oBOD = new XIDBO();
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Bordereau_T");
                        XIIBO oBOI = new XIIBO();
                        oBOI.BOD = oBOD;
                        oBOI.SetAttribute("sBordereauFileName", sFileName);
                        oBOI.SetAttribute("iStatus", "10");
                        var Response = oBOI.Save(oBOI);
                        if (Response.bOK && Response.oResult != null)
                        {
                            oBOI = (XIIBO)Response.oResult;
                            var sBordereauID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).FirstOrDefault().sValue;
                            oBOD = new XIDBO();
                            oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "BordereauDetails_T");
                            oBOI = new XIIBO();
                            oBOI.BOD = oBOD;
                            oBOI.SetAttribute("FKiBordereauID", sBordereauID);
                            oBOI.SetAttribute("sInstanceIDs", sInstanceIDs);
                            var oBOIResponse = oBOI.Save(oBOI);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
            }
        }
    }
}



