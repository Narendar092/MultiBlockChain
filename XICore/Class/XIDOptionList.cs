using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDOptionList
    {
        public int ID { get; set; }
        public int BOID { get; set; }
        public int BOFieldID { get; set; }
        public string Name { get; set; }
        public string sOptionName { get; set; }
        public string sValues { get; set; }
        public string sOptionCode { get; set; }
        public string sShowField { get; set; }
        public string sHideField { get; set; }
        public bool bIsGroup { get; set; }
        public int iType { get; set; }
        public int XIDeleted { get; set; }
        public int StatusTypeID { get; set; }
        public Nullable<Guid> BOIDXIGUID { get; set; }
        public Nullable<Guid> BOFieldIDXIGUID { get; set; }
        public string sUIStyle { get; set; }
    }
}