using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using XIDatabase;
using XISystem;

namespace XICore
{
    [Table("XIDialog_T")]
    public class XIDDialog : XIDefinitionBase
    {
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public int OrganizationID { get; set; }
        public string DialogName { get; set; }
        public int LayoutID { get; set; }
        public Guid LayoutIDXIGUID { get; set; }
        public bool IsResizable { get; set; }
        public bool IsCloseIcon { get; set; }
        public bool IsMinimiseIcon { get; set; }
        public bool IsGrouping { get; set; }
        public bool IsPinIcon { get; set; }
        public bool IsMaximiseIcon { get; set; }
        public string BarPosition { get; set; }
        public string PopupSize { get; set; }
        public int DialogWidth { get; set; }
        public int DialogHeight { get; set; }
        public string Icon { get; set; }
        public string DialogMy1 { get; set; }
        public string DialogMy2 { get; set; }
        public string DialogAt1 { get; set; }
        public string DialogAt2 { get; set; }
        public int iTransparency { get; set; }
        public long FKiBOID { get; set; }
        public Guid FKiBOIDXIGUID { get; set; }
        public string sLabel { get; set; }
        public bool IsChildsGrouping { get; set; }
        [NotMapped]
        public List<XIDropDown> ddlApplications { get; set; }
        [NotMapped]
        public List<XIDropDown> Layouts { get; set; }
        [NotMapped]
        public bool bLayoutAddToParentTaskbar { get; set; }
        public string sGroupLabel { get; set; }
    }
}