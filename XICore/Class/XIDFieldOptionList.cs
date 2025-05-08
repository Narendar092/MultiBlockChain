using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDFieldOptionList
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiQSFieldID { get; set; }
        public string sOptionName { get; set; }
        public string sOptionValue { get; set; }
        public int iType { get; set; }
        public string sShowField { get; set; }
        public string sHideField { get; set; }
        public string sOptionCode { get; set; }
        public bool bIsRemoveDependency { get; set; }
        public int iDependencyType { get; set; }
        public string sName { get; set; }
        public int XIDeleted { get; set; }
        public int XIOrder { get; set; }
    }
}