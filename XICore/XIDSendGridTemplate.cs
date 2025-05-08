using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;
using System.Diagnostics;
using System.Configuration;

namespace XICore
{
    public class XIDSendGridTemplate : XIDefinitionBase
    {
        public int iSGTID { get; set; }
        public int FKiSGADID { get; set; }
        public string sTemplateName { get; set; }
        public string sTemplateID { get; set; }
        public string sTemplateObject { get; set; }
             
    }

    public class XIDSendGridAccountDetails
    {
        public int iSGADID { get; set; }
        public string sSingleSender { get; set; }
        public string sSingleSenderName { get; set; }
        public string sAPIKey { get; set; }
    }
}