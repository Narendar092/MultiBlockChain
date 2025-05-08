using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XICommunicationI
    {
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int iType { get; set; }
        public int iStatus { get; set; }
        public int iComType { get; set; }
        public int FKiComTypeID { get; set; }
        public int iDirection { get; set; }
        public int iSendStatus { get; set; }
        public int iExternalStatus { get; set; }
        public int FKiTemplateID { get; set; }
        public Guid XIOrigin { get; set; }
        public string XIOriginType { get; set; }
        public Guid XIInstanceOrigin { get; set; }
        public Guid XIInstanceDefinition { get; set; }
        public int iRetryCount { get; set; }
        public int iRetryMax { get; set; }
        public string sFrom { get; set; }
        public string sTo { get; set; }
        public string sHeader { get; set; }
        public string sContent { get; set; }
        public int FKiXAttachmentsID { get; set; }
        public string sSendReceiveResult { get; set; }
        public string sSendReceiveID { get;set; }
        public int iRateLimit { get; set; }
        public int iInboundProcess { get; set; }
    }
}