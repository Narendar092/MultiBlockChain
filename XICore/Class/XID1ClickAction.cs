using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XID1ClickAction
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiActionID { get; set; } 
        public int iType { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        public Guid FKiActionIDXIGUID { get; set; }
    }
}