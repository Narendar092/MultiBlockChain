using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{    
    public class XIPerformance
    {
        public string sName { get; set; }
        public string sCode { get; set; }
        public Guid FKiDefIDXIGUID { get; set; }
        public int iType { get; set; }
        public int iCalls { get; set; }
        public double iMinMS { get; set; }
        public double iMaxMS { get; set; }
        public double iAverage { get; set; }
    }
}