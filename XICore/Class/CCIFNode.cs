using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XICore
{
    public class CCIFNode
    {
        public Dictionary<string, CNV> Attributes { get; set; }
        public bool bHasChilds { get; set; }
        public Dictionary<string, List<CCIFNode>> SubChildI { get; set; }
    }
}