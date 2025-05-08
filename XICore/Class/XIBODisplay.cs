using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIBODisplay
    {
        public XIIBO BOInstance { get; set; }
        public List<XIVisualisation> Visualisations { get; set; }
        public Dictionary<string, List<string>> oAccordGroupAttr { get; set; }
    }
}