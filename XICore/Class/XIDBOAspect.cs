using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDBOAspect
    {
        public long ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sCode { get; set; }
        public int FKiBOID { get; set; }
        public int FKi1ClickID { get; set; }
        public string sWhere { get; set; }
    }
}