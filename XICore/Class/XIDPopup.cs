using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDPopup : XIDefinitionBase
    {
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public int BOID { get; set; }
        public int FKColumnID { get; set; }
        public bool IsFKPopup { get; set; }
        public bool IsLeftMenu { get; set; }
        public bool IsGrouping { get; set; }
        public int ParentID { get; set; }
        public string BarPosition { get; set; }
        public string PopupSize { get; set; }
        public int PopupWidth { get; set; }
        public int PopupHeight { get; set; }
        public string Name { get; set; }
        public int LayoutID { get; set; }
    }
}