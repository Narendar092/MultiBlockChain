using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XID1ClickLink
    {
        public long ID { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiXILinkID { get; set; }
        public string sName { get; set; }
        public int iType { get; set; }
        public string sCode { get; set; }
        public bool bIsCreate { get; set; }
        public bool bIsEdit { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsDelete { get; set; }
        public bool bIsRefresh { get; set; }
        public string XILinkName { get; set; }
        public string sProcessControllers { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        public Guid FKiXILinkIDXIGUID { get; set; }
    }
}