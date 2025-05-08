using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using XICore;
using XIDataBase.Hubs;
using XIDNA.Repository;
using XISystem;
using ZeeBNPPFServices;
using ZeePremiumFinance;
using static ZeeBNPPFServices.XIPFCommon;

namespace XIDNA.Controllers
{
    [AllowAnonymous]
    public class PremiumFinanceController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CommonRepository Common = new CommonRepository();
        // GET: PremiumFinance
        [AllowAnonymous]
        public ActionResult PFManageRefer()
        {
            try
            {
                Common.SaveErrorLog("PF Refer case ApplicationLog: Started Execution", "");
                string partnetrascode = Request.Params["PartnerTransactionToken"];

                //var reqkeys = Request.Params.AllKeys.ToList();
                //string jsonreq = new JavaScriptSerializer().Serialize(Request.Params.AllKeys);
                string jsonreq = new JavaScriptSerializer().Serialize(Request.Form.AllKeys);
                Common.SaveErrorLog("PF Refer case ApplicationLog Request Object:" + jsonreq, "");
                Dictionary<string, string> lstobject = new Dictionary<string, string>();
                foreach (var key in Request.Form.AllKeys)
                {
                    lstobject.Add(key, Request[key]);
                }
                string jsonkeyvalue = new JavaScriptSerializer().Serialize(lstobject);
                Common.SaveErrorLog("PF Refer case ApplicationLog Request Object With Value:" + jsonkeyvalue, "");
                Dictionary<string, XIPFAttribute> XIpfvalues = new Dictionary<string, XIPFAttribute>();
                XIpfvalues.Add("PartnerTransactionToken", new XIPFAttribute { sValue = partnetrascode });
                XIpfvalues.Add("iPFDecision", new XIPFAttribute { sValue = Request.Params["Decision"] });
                XIpfvalues.Add("sPFAccountNo", new XIPFAttribute { sValue = Request.Params["AccountNumber"] });
                XIpfvalues.Add("Pdf", new XIPFAttribute { sValue = Request.Params["Pdf"] });
                XIPFValues xiqs = new XIPFValues();
                xiqs.XIValues = XIpfvalues;
                Common.SaveErrorLog("PF Refer case ApplicationLog: params" + xiqs.GetXIPFValue("Decision").sValue + " Account number" + xiqs.GetXIPFValue("AccountNumber").sValue, "");
                //XIPFServices wrapper = new XIPFServices();
                //wrapper.UpdatePFNewBusinessRefer(xiqs);
                Common.SaveErrorLog("PF Refer case ApplicationLog Execution Completed Successfully", "");
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                Common.SaveErrorLog("PF Refer case ErrorLog: " + ex.ToString(), "");
                throw ex;
            }
        }

        #region Signalr


        [HttpGet]
        [AllowAnonymous]
        [Route("PremiumFinance/SignalREvent/{InstanceID}/{ProductversionID}/{sRoleName}")]
        public void SignalREvent(int InstanceID, int ProductversionID, string sRoleName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                Common.SaveErrorLog("Log: SignalREvent Method started Execution " + InstanceID + " " + ProductversionID, sDatabase);
                // NOTE: EXECUTE THE BELOW QUERIES HERE
                // Select ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote AS Yearly from Aggregations_T Where FKiQSInstanceID = 51787 and FKiProductVersionID = 2 and "+XIConstant.Key_XIDeleted+" = 0
                //SELECT rBestQuote FROM Lead_T where FKiQSInstanceID=51787
                XIIBO oXiBo = new XIIBO();
                QueryEngine oQE = new QueryEngine();
                //string sWhereCondition = $"FKiQSInstanceID={InstanceID},FKiProductVersionID={ProductversionID},"+XIConstant.Key_XIDeleted+"=0";
                string sWhereCondition = "FKiQSInstanceID=" + InstanceID + ",FKiProductVersionID=" + ProductversionID + "," + XIConstant.Key_XIDeleted + "=0";
                var oQResult = oQE.Execute_QueryEngine("Aggregations", "sGUID,ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oXiBo = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.OrderByDescending(f => f.AttributeI("ID").iValue).FirstOrDefault();
                }
                // Get the product id from ProductVersionID
                XIIBO oXiProduct = new XIIBO();
                XIIXI oIXI = new XIIXI();
                oXiProduct = oIXI.BOI("ProductVersion_T", ProductversionID.ToString(), "FKiProductID,bIsIndicativePrice");
                int iProductID = 0; bool bIsIndicativePrice = false;
                if (oXiProduct != null && oXiProduct.Attributes.ContainsKey("FKiProductID"))
                {
                    iProductID = oXiProduct.AttributeI("FKiProductID").iValue;
                    bIsIndicativePrice = oXiProduct.AttributeI("bIsIndicativePrice").bValue;
                }
                // QUERY FOR BEST QUOTE. 
                XIIBO OLeadBO = new XIIBO();
                string sWhereLead = "FKiQSInstanceID = " + InstanceID;
                var oLeadQResult = oQE.Execute_QueryEngine("Lead_T", "rBestQuote", sWhereLead);
                if (oLeadQResult.bOK && oLeadQResult.oResult != null)
                {
                    OLeadBO = ((Dictionary<string, XIIBO>)oLeadQResult.oResult).Values.FirstOrDefault();
                }
                // BUILD ONE STANDARD ANNONYMOUS OBJECT HERE
                if (oXiBo != null && OLeadBO != null)
                {
                    var oAnnonymous = new
                    {
                        ProductversionID = ProductversionID,
                        QSInstanceID = InstanceID,
                        ProductID = iProductID,
                        iQuoteStatus = oXiBo.AttributeI("iQuoteStatus").sValue,
                        rCompulsoryExcess = oXiBo.AttributeI("rCompulsoryExcess").rValue,
                        rVoluntaryExcess = oXiBo.AttributeI("rVoluntaryExcess").rValue,
                        rTotalExcess = oXiBo.AttributeI("rTotalExcess").rValue,
                        rMonthlyPrice = oXiBo.AttributeI("rMonthlyPrice").rValue,
                        rMonthlyTotal = oXiBo.AttributeI("rMonthlyTotal").rValue,
                        zDefaultDeposit = oXiBo.AttributeI("zDefaultDeposit").rValue,
                        rFinalQuote = oXiBo.AttributeI("rFinalQuote").rValue,
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QuoteID = oXiBo.AttributeI("sGUID").sValue,
                        RoleName = sRoleName,
                        bIsIndicativePrice= bIsIndicativePrice
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                }
                else if (OLeadBO != null)
                {
                    var oAnnonymous = new
                    {
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QSInstanceID = InstanceID,
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed rBestQuote addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                throw ex;
            }
        }
        #endregion
    }
}