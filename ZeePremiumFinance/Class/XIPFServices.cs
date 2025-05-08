using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using XICore;
using XISystem;
using ZeeInsurance;
using System.Reflection;
//using System.Runtime.Caching;
using xiEnumSystem;
using XIDNA.Repository;
using static ZeeBNPPFServices.XIPFCommon;

namespace ZeePremiumFinance
{
    //public static class MemoryCacheHackExtensions
    //{
    //    public static long GetApproximateSize(this MemoryCache cache)
    //    {
    //        var statsField = typeof(MemoryCache).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var statsValue = statsField.GetValue(cache);
    //        var monitorField = statsValue.GetType().GetField("_cacheMemoryMonitor", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var monitorValue = monitorField.GetValue(statsValue);
    //        var sizeField = monitorValue.GetType().GetField("_sizedRef", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var sizeValue = sizeField.GetValue(monitorValue);
    //        var approxProp = sizeValue.GetType().GetProperty("ApproximateSize", BindingFlags.NonPublic | BindingFlags.Instance);
    //        return (long)approxProp.GetValue(sizeValue, null);
    //    }
    //}
    public class XIPFServices
    {
        string sServerKey = string.Empty;
        public XIPFServices()
        {
            sServerKey = GetValue("ServerEnvironment");
        }

        public string GetValue(string skey)
        {
            try
            {
                return ConfigurationManager.AppSettings[skey];
            }
            catch
            {
                return "";
            }
        }

        #region NewBusiness_Method
        public CResult PFPreValidations(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFServiceDev oPFDev = new XIPFServiceDev();
            XIPFServiceLive oPFLive = new XIPFServiceLive();
            if (sServerKey.ToLower() == "dev")
            {
                oCResult = oPFDev.PFPreValidations(oParams);
            }
            else if (sServerKey.ToLower() == "live")
            {
                oCResult = oPFLive.PFPreValidations(oParams);
            }
            return oCResult;
        }

        public CResult PFNewBusiness(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFServiceDev oPFDev = new XIPFServiceDev();
            XIPFServiceLive oPFLive = new XIPFServiceLive();
            if (sServerKey.ToLower() == "dev")
            {
                oCResult = oPFDev.PFNewBusiness(oParams);
            }
            else if (sServerKey.ToLower() == "live")
            {
                oCResult = oPFLive.PFNewBusiness(oParams);
            }
            return oCResult;
        }
        #endregion

        #region Accept Decline PF Manually
        public CResult AcceptPF(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFServiceDev oPFDev = new XIPFServiceDev();
            XIPFServiceLive oPFLive = new XIPFServiceLive();
            if (sServerKey.ToLower() == "dev")
            {
                oCResult = oPFDev.AcceptPF(oParams);
            }
            else if (sServerKey.ToLower() == "live")
            {
                oCResult = oPFLive.AcceptPF(oParams);
            }
            return oCResult;
        }
        #endregion

        #region PolicyPopup_PFTx
        public CResult PFPolicyPopupBNPTX(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFServiceDev oPFDev = new XIPFServiceDev();
            XIPFServiceLive oPFLive = new XIPFServiceLive();
            if (sServerKey.ToLower() == "dev")
            {
                oCResult = oPFDev.PFPolicyPopupBNPTX(oParams);
            }
            else if (sServerKey.ToLower() == "live")
            {
                oCResult = oPFLive.PFPolicyPopupBNPTX(oParams);
            }
            return oCResult;
        }
        public CResult PolicyLive(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            XIPFServiceDev oPFDev = new XIPFServiceDev();
            XIPFServiceLive oPFLive = new XIPFServiceLive();
            if (sServerKey.ToLower() == "dev")
            {
                oCResult = oPFDev.PolicyLive(oParams);
            }
            else if (sServerKey.ToLower() == "live")
            {
                oCResult = oPFLive.PolicyLive(oParams);
            }
                return oCResult;
        }
        #endregion
    }
}
