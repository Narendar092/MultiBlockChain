using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDSource
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public int iStatus { get; set; }
        public string sCode { get; set; }
        public string XICreatedBy { get; set; }
        public DateTime XICreatedWhen { get; set; }
        public string XIUpdatedBy { get; set; }
        public DateTime XIUpdatedWhen { get; set; }
        public string sPrefixCode { get; set; }
        public int refAccountCategory { get; set; }
        public int XIDeleted { get; set; }
        public int FKiOriginID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
    }
}