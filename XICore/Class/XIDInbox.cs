using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDInbox
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKi1ClickID { get; set; }
        public string FKi1ClickIDXIGUID { get; set; }
        public int FKiBOID { get; set; }
        public int ClassID { get; set; }
        public int TypeID { get; set; }
        public int OrganizationID { get; set; }
        public int RoleID { get; set; }
        public int Rank { get; set; }
        public string Icon { get; set; }
        public int Target { get; set; }
        public int Location { get; set; }
        public int DisplayAs { get; set; }
        public string TargetResultType { get; set; }
        public int TargetTemplateID { get; set; }
        public int MenuID { get; set; }
        public int FKiApplicationID { get; set; }
        public int FKiXILinkID { get; set; }
        public string FKiXILinkIDXIGUID { get; set; }
        public string InboxCount { get; set; }
        public string Percentage { get; set; }
        public string sHTML { get; set; }
        public int StatusTypeID { get; set; }
        public string CountColour { get; set; }
        public string ParentID { get; set; }
        public List<XIDInbox> SubGroups { get; set; }
        public string Name { get; set; }
        public int XIDeleted { get; set; }
        public int iRefreshingType { get; set; }
        public int iSetinterval { get; set; }
        public decimal rRTagetcolor { get; set; }
        public decimal rATagetcolor { get; set; }
        private XID1Click oMy1ClickD;
        public XID1Click o1ClickD
        {
            get
            {
                return oMy1ClickD;
            }
            set
            {
                oMy1ClickD = value;
            }
        }

        private List<XIDInbox> oMySub1Clicks;
        public List<XIDInbox> Sub1Clicks
        {
            get
            {
                return oMySub1Clicks;
            }
            set
            {
                oMySub1Clicks = value;
            }
        }
    }
}