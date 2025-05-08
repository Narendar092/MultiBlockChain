using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class VMDTResponses
    {
        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public IEnumerable<string[]> aaData { get; set; }
    }
}