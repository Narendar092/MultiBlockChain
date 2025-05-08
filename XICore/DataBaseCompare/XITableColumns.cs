using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using xiEnumSystem;
using XISystem;

namespace XICore
{
    public class XITableColumns : IEquatable<XITableColumns>
    {

        public string sTableName { get; set; }
        public string sColumnName { get; set; }
        public string sDataType { get; set; }
        public long lMaxLength { get; set; }
        public bool bIsNullable { get; set; }
        public bool bIsIdentity { get; set; }
        public int iColumnOrder { get; set; }
        public string sDefaultValue { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public int iActionType { get; set; }
        public string sPrimaryKey { get; set; }
        public bool IsSelected { get; set; }
        public List<int> iDataSource { get; set; }
        public string sBOName { get; set; }
        public int iType
        {
            get
            {
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    sDataType = ((BODatatypes)value).ToString().ToLower();
                }
            }
        }

        public bool Equals(XITableColumns obj)
        {
            try
            {
                if (obj == null || !(obj is XITableColumns))
                    return false;
                Properties = new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Compare(this, obj); ;
        }
        public bool Compare(object obj, object another)
        {
            var result = true;
            List<string> PropertiesOnly = new List<string> { "sDataType", "lMaxLength", "bIsNullable", "bIsIdentity", "sDefaultValue" };
            try
            {
                if (ReferenceEquals(obj, another)) return true;
                if ((obj == null) || (another == null)) return false;
                if (obj.GetType() != another.GetType()) return false;

                if (!obj.GetType().IsClass) return obj.Equals(another);


                foreach (var property in obj.GetType().GetProperties().Where(m => PropertiesOnly.Any(s => s.ToLower() == m.Name.ToLower())))
                {
                    var objValue = property.GetValue(obj);
                    var anotherValue = property.GetValue(another);
                    if (!(objValue == null && anotherValue == null) && objValue != null && !objValue.Equals(anotherValue))
                    {
                        string sPropertyName = string.Empty;
                        switch (property.Name)
                        {
                            case "sDataType":
                                sPropertyName = XIConstant.DataType;
                                break;
                            case "lMaxLength":
                                sPropertyName = XIConstant.MaxLength;
                                break;
                            case "bIsNullable":
                                sPropertyName = XIConstant.IsNull;
                                break;
                            case "bIsIdentity":
                                sPropertyName = XIConstant.IsIdentity;
                                break;
                            case "sDefaultValue":
                                sPropertyName = XIConstant.DefaultValue;
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(sPropertyName))
                            Properties.Add(sPropertyName, objValue.ToString());
                        result = false;
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
    }
    public class XITableRecords
    {
        public string sTableName { get; set; }
        public string sBOName { get; set; }
        public int iSourceCount { get; set; }
        public int iTargetCount { get; set; }
        public int iDiffCount { get; set; }
        public int iEqualCount { get; set; }
        public string sActionCode { get; set; }
        public string sCompareWith { get; set; }
        public string sTargetConnectionString { get; set; }
        public string sPrimaryKey { get; set; }
        public List<XITableColumns> Columns { get; set; }
        public IEnumerable<string> DiffIDs { get; set; }
        public IEnumerable<string> OnlySourceIDs { get; set; }
        public IEnumerable<string> OnlyTargetIDs { get; set; }
        public List<DataTable> Tables { get; set; }
        public string sSource { get; set; }
        public string sTarget { get; set; }
        public string sStructureID { get; set; }
    }

}