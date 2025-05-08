using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICore
{
    public class XI1QueryAttributeDetails
    {
        public int ID { get; set; }
        public Guid? FKiBOIDXIGUID { get; set; }
        public Guid? FKiAttrXIGUID { get; set; }
        public string sLabel { get; set; }
        public bool bIsExposeOperator { get; set; }
        public string FKiOperator { get; set; }
        public string FKiOneClickIDXIGUID { get; set; }
        public string sDefaultValue { get; set; }
        public bool bIsMultiCheckbox { get; set; }
        public bool bIsOverrideReferenceData { get; set; }
        public string sReferenceData { get; set; }
        public Guid? FKsRangeBOXIGUID { get; set; }
        public Guid? FKsRangeDetailsXIGUID { get; set; }
        public bool bIsOverrideFKiOneclickIDXIGUID { get; set; }
        public string FKiOverrideOneClickIDXIGUID { get; set; }
    }
}
