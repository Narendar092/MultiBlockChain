using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDFieldDefinition
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiXIFieldOriginID { get; set; }
        public Guid FKiXIFieldOriginIDXIGUID { get; set; }
        public int FKiXIStepDefinitionID { get; set; }
        public Guid FKiXIStepDefinitionIDXIGUID { get; set; }
        public int FKiStepSectionID { get; set; }
        public int XIDeleted { get; set; }
        public Guid FKiStepSectionIDXIGUID { get; set; }

        private XIDFieldOrigin oMyFieldOrigin = new XIDFieldOrigin();
        public XIDFieldOrigin FieldOrigin
        {
            get
            {
                return oMyFieldOrigin;
            }
            set
            {
                oMyFieldOrigin = value;
            }
        }
    }
}