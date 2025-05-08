using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.ViewModels;
using Microsoft.AspNet.SignalR;
using XIDNA.Hubs;
using System.Data.SqlClient;
using System.Data.Entity;
using System.IO;
using XIDNA.Common;
using ZeeInsurance;
using System.Configuration;
using XICore;
using XISystem;

namespace XIDNA.Controllers
{
    // [System.Web.Mvc.Authorize]
    public class MailSchedulerController : Controller
    {
        LeadImport LeadImport = new LeadImport();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMailRepository MailRepository;

        public MailSchedulerController() : this(new MailRepository()) { }

        public MailSchedulerController(MailRepository MailRepository)
        {
            this.MailRepository = MailRepository;
        }
        CommonRepository Common = new CommonRepository();
        XIInfraCache oCache = new XIInfraCache();
        //
        // GET: /Mail/
        //[AllowAnonymous]
        public ActionResult Index()
        {
            // string sDatabase = SessionManager.ConfigDatabase;
            string sDatabase = ConfigurationManager.AppSettings["DBName"];
            try
            {
                int OrgID = 0;
                XID1Click PV1Click = new XID1Click();
                XIDStructure oStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();

                var o1ClickIO = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "IOServerDetails", null);
                PV1Click = (XID1Click)o1ClickIO.Clone(o1ClickIO);
                var oQueryIO = oStructure.ReplaceExpressionWithCacheValue(o1ClickIO.Query, nParams);
                PV1Click.Query = oQueryIO;
                PV1Click.Name = "XIIOServerDetails_T";
                var oOneClickIO = PV1Click.OneClick_Execute();

                var o1ClickAPP = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "AppSettings", null);
                PV1Click = (XID1Click)o1ClickAPP.Clone(o1ClickAPP);
                var oQueryAPP = oStructure.ReplaceExpressionWithCacheValue(o1ClickAPP.Query, nParams);
                PV1Click.Query = oQueryAPP;
                PV1Click.Name = "XIAPISettings_T";
                var oOneClickAPP = PV1Click.OneClick_Execute();
                List<string> ListApp = new List<string>();
                ListApp = oOneClickAPP.ToList().Select(m => m.Value.Attributes.Where(s => s.Key == "scode").Select(s => s.Value.sValue).FirstOrDefault()).ToList();
                Dictionary<int, string> ListIO = new Dictionary<int, string>();
                for (int j = 0; j < oOneClickIO.Values.Count(); j++)
                {
                    var Value = oOneClickIO.Values.ElementAt(j).Attributes.Values.Where(s => s.sName == "ID").Select(s => s.iValue).FirstOrDefault();
                    var text = oOneClickIO.Values.ElementAt(j).Attributes.Values.Where(s => s.sName.ToLower() == "fromaddress").Select(s => s.sPreviousValue).FirstOrDefault();
                    ListIO.Add(Value, text);
                }
                var APPIO = ListApp.Where(b => ListIO.Any(a => b.Contains(a.Value)));
                Common.SaveErrorLog("XIIOServerDetails_T Saving Started", sDatabase);
                if (APPIO != null)
                {
                    var sFolder = "INBOX";
                    var Flag = "S";

                    foreach (var item in APPIO)
                    {
                        int Value = ListIO.Where(s => s.Value == item).Select(s => s.Key).FirstOrDefault();
                        var sSubjects = LeadImport.Get_EmailSubjects(Value, sFolder, OrgID, Flag);
                        if (sSubjects.oResult != null)
                        {
                            List<string> test = new List<string>();
                            var sEmailDetails = sSubjects.oResult.ToString();
                            test = (List<string>)sSubjects.oResult;
                            CResult Response = new CResult();
                            for (int i = 0; i < test.Count(); i++)
                            {
                                var sSubjectDetails = test[i];
                                var sDetailsSplit = sSubjectDetails.Split(new string[] { "<>" }, StringSplitOptions.None);
                                var iUID = sDetailsSplit[0];
                                var sFolder1 = sDetailsSplit[1];
                                var sSubject = sDetailsSplit[2];
                                var sFromName = sDetailsSplit[3];
                                var sDate = sDetailsSplit[4];
                                var ID = sDetailsSplit[5];
                                Response = LeadImport.Save_MailContent(Convert.ToInt32(ID), Convert.ToInt32(iUID), sFolder);
                                Common.SaveErrorLog("XIIOServerDetails_T Saving Completed", sDatabase);
                            }
                            if (Response.oResult == null)
                            {
                                Common.SaveErrorLog("XIIOServerDetails_T Not Saved", sDatabase);
                            }
                        }
                    }
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
    }
}