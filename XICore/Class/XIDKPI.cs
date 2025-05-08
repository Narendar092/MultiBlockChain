using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class XIDKPI
    {
        public int ID { get; set; }
        public string XIGUID { get; set; }
        public int FKi1ClickID { get; set; }
        public string FKi1ClickIDXIGUID { get; set; }
        public int FKiComponentID { get; set; }
        public int FKiRoleID { get; set; }
        public int FKiApplicationID { get; set; }
        public int FKiXILinkID { get; set; }
        public string FKiXILinkIDXIGUID { get; set; }
        public int iRefreshingType { get; set; }
        public int iSetinterval {  get; set; }
        public string FKiComponentIDXIGUID { get; set; }
        public string FKiKPIGroupIDXIGUID { get; set; }
        public int iSize { get; set; }
        public string sColors { get; set; }
        public string sIocn {  get; set; }
        public bool bToolTip {  get; set; } 
        public bool bGridLines { get; set; }
        public string sSubTitle { get; set; }
        public string sGroupTitle { get; set; }
        public string sKpiTitle { get; set; }
        public bool bIsCursor { get; set; }
        public bool bIsLegends { get; set; }
        public string sLegendPosition { get; set; }
        public int RowXiLinkID { get; set; }
        public string RowXiLinkIDXIGUID { get; set; }
    }
}