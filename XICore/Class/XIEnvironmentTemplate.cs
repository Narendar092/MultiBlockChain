using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIEnvironmentTemplate
    {
        public int ID { get; set; }
        public int FKiTemplateID { get; set; }
        public int iTemplateType { get; set; }
        public string sTemplateType { get; set; }
        public string sScript { get; set; }
    }
}