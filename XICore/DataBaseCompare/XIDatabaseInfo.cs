using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using XISystem;

namespace XICore
{
    public class XIDatabaseInfo : XIInstanceBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServerName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Database { get; set; }
        public string ConnectionString { get; set; }
        public string bIsSuccess { get; set; }
        public string Query { get; set; }
        public CommandType CommandType { get; set; }
        public SqlConnection SqlCon { get; set; }
        #region Server_Connection_Query_Execution

        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
          :: Create Connection string based on UserName, Password and ServerName ::
          :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        public void GetConnectionString()
        {

            CResult oCResult = new CResult();
            try
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    if (!string.IsNullOrEmpty(Database))
                        Database = "initial catalog=" + Database + ";";
                    ConnectionString = string.Format("Data Source={0};{3}User Id={1}; Password={2}; MultipleActiveResultSets=True;", ServerName, UserName, Password, Database);
                }

            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
        }
        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
          :: Get All Databses of Server Based on Connection String ::
          :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        public List<XIDatabaseInfo> GetDataBases()
        {
            CResult oCResult = new CResult();
            List<XIDatabaseInfo> Databases = new List<XIDatabaseInfo>();
            try
            {

                if (CheckConnection())
                {
                    using (var conn = Connection())
                    {
                        DataTable oDataTable = conn.GetSchema("Databases");
                        foreach (DataRow Row in oDataTable.Rows)
                        {
                            XIDatabaseInfo DbInfo = new XIDatabaseInfo();
                            Databases.Add(new XIDatabaseInfo { Name = Row["database_name"].ToString() });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return Databases;
        }
        /**************************************************Connection Related ***************************************************************/
        public SqlConnection OpenConnection()
        {
            CResult oCResult = new CResult();
            try
            {
                if (SqlCon == null || SqlCon.State == ConnectionState.Closed || !ConnectionString.Contains(SqlCon.Database + ";"))
                {
                    if (SqlCon != null && SqlCon.State == ConnectionState.Open)
                        CloseConnection();
                    SqlCon = Connection();
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return SqlCon;
        }
        public SqlConnection Connection()
        {
            CResult oCResult = new CResult();
            GetConnectionString();
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return con;
        }
        public void CloseConnection()
        {
            CResult oCResult = new CResult();
            try
            {
                if (SqlCon != null && SqlCon.State == ConnectionState.Open)
                    SqlCon.Close();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
        }
        public bool CheckConnection()
        {
            CResult oCResult = new CResult();
            GetConnectionString();
            bool bIsConnectionTrue = false;
            try
            {

                using (DbConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConnectionString;
                    connection.Open();
                    connection.Close();
                    bIsConnectionTrue = true;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return bIsConnectionTrue;
        }
        public string GetConnectionInfo()
        {
            CResult oCResult = new CResult();
            string sServerAndDataBase = string.Empty;
            try
            {

                using (DbConnection connection = new SqlConnection())
                {
                    connection.ConnectionString = ConnectionString;
                    sServerAndDataBase = "Server=" + connection.DataSource + ", Database=" + connection.Database;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
            }
            return sServerAndDataBase;
        }

        /***************************************************Command Related ***********************************************************/
        public SqlCommand Command(CommandType CommandType)
        {
            CResult oCResult = new CResult();
            try
            {
                using (var cmd = new SqlCommand(Query, OpenConnection()))
                {
                    cmd.CommandType = CommandType;
                    return cmd;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

        /********************************************************** Query Execution Related **********************************************/
        public SqlDataReader ExecuteReader()
        {
            CResult oCResult = new CResult();
            try
            {
                using (var cmd = Command(CommandType))
                    return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }
        public object ExecuteScalar()
        {
            CResult oCResult = new CResult();
            try
            {
                using (var cmd = Command(CommandType))
                    return cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }
        public DataTable ExecuteReaderToDataTable()
        {
            CResult oCResult = new CResult();
            DataTable DT = new DataTable();
            try
            {
                using (var Result = ExecuteReader())
                    DT.Load(Result);
                return DT;
            }
            catch (SqlException ex)
            {
                if (ex.Errors.Count > 0)
                {
                    DT.Columns.Add(new DataColumn { ColumnName = "LineNumber", DataType = typeof(int) });
                    DT.Columns.Add(new DataColumn { ColumnName = "Message", DataType = typeof(string) });
                    foreach (var error in ex.Errors)
                    {
                        SqlError e = ((SqlError)error);
                        object[] obj = new object[2];
                        obj[0] = e.LineNumber;
                        obj[1] = e.Message;
                        DT.Rows.Add(obj);
                    }
                    return DT;
                }
                else
                {
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    SaveErrortoDB(oCResult);
                    throw ex;
                }

            }
        }
        #endregion Server_Connection_Query_Execution

        #region System_Queries
        public const string sSystemTableColumnsQuery = @"SELECT tbl.name AS sTableName, col.name AS sColumnName, col.column_id AS iColumnOrder , col.max_length AS lMaxLength, col.is_nullable AS bIsNullable, col.is_identity AS bIsIdentity, col.is_computed AS bIsComputed , def.definition AS sDefaultValue ,typ.name as sDataType
                       FROM SYS.COLUMNS col
                       INNER JOIN SYS.TABLES tbl  ON col.object_id = tbl.object_id and tbl.type = 'U'
                       LEFT JOIN SYS.DEFAULT_CONSTRAINTS def ON col.default_object_id != 0 AND col.object_id = def.parent_object_id AND col.column_id = def.parent_column_id
        INNER JOIN SYS.TYPES typ ON typ.user_type_id = col.user_type_id";

        //public const string sTablesWithPrimaryKeyQuery = @"SELECT tbl.name AS sTableName, constUse.COLUMN_NAME AS sPrimaryKey FROM SYS.TABLES tbl
        //                                                   INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS const
        //                                                   ON tbl.name = const.TABLE_NAME AND tbl.type = 'U'
        //                                                   INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE constUse
        //                                                   ON constUse.CONSTRAINT_NAME = const.CONSTRAINT_NAME AND const.CONSTRAINT_TYPE = 'PRIMARY KEY'";

        public const string sTablesWithPrimaryKeyQuery = @"SELECT tbl.name AS sTableName , col.name AS sPrimaryKey FROM SYS.TABLES tbl
        LEFT JOIN SYS.COLUMNS col 
        ON tbl.object_id = col.object_id AND col.is_identity = 1
        LEFT JOIN ( SELECT tblConst.TABLE_NAME, colUse.COLUMN_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE colUse
        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tblConst
        ON coluse.CONSTRAINT_NAME = tblConst.CONSTRAINT_NAME AND colUse.TABLE_NAME= tblConst.TABLE_NAME AND tblConst.CONSTRAINT_TYPE = 'PRIMARY KEY') AS const 
        ON col.name = const.COLUMN_NAME AND col.object_id = OBJECT_ID(const.TABLE_NAME)";
        public const string sDropQuery = "DROP TABLE [dbo].[@@@]";

        public const string TransactionQuery = @"BEGIN TRANSACTION trans 
    BEGIN TRY
    @@@
        BEGIN 
        COMMIT TRANSACTION trans
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            BEGIN 
            SELECT ERROR_LINE() AS LineNumber, ERROR_MESSAGE() AS Message;
            ROLLBACK TRANSACTION trans
        END
    END CATCH";
        public const string IdentityAddQuery = @" BEGIN TRANSACTION;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON; 

#TempTable

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[@@@])
    BEGIN
       #IdentityON
         #InsertTempTable
         #SelectTable
      #IdentityOFF
    END

DROP TABLE [dbo].[@@@]

EXECUTE sp_rename N'[dbo].[temp_@@@]', N'@@@';

COMMIT TRANSACTION;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;";
        public const string IdentityON = "SET IDENTITY_INSERT [dbo].[temp_@@@] ON;";
        public const string IdentityOFF = "SET IDENTITY_INSERT [dbo].[temp_@@@] OFF;";
        #endregion System_Queries

        #region Self_Database_Queries
        //public const string sBOTableColumnsQuery = @"SELECT BO.TableName AS sTableName,  ATTR.Name AS sColumnName, ATTR.TypeID AS iType, CASE WHEN BO.sPrimaryKey = ATTR.Name THEN 1 ELSE 0 END AS bIsIdentity , CASE WHEN ATTR.MaxLength = 'MAX' THEN -1 ELSE ATTR.MaxLength END AS lMaxLength, ATTR.IsNull AS bIsNullable,  DefaultValue AS sDefaultValue FROM XIBO_T_N  BO 
        //                                             INNER JOIN XIBOATTRIBUTE_T_N ATTR ON BO.BOID = ATTR.BOID
        //                                             WHERE bo.TableName NOT LIKE '{%' AND BO.iDataSource = @@@ AND ATTR.Name NOT IN ('zXCrtdBy', 'zXCrtdWhn', 'zXUpdtdBy', 'zXUpdtdWhn', 'izXDeleted', 'sHierarchy')";

        public const string sBOTableColumnsQuery = @"SELECT BO.TableName AS sTableName,  ATTR.Name AS sColumnName, ATTR.TypeID AS iType, CASE WHEN BO.sPrimaryKey = ATTR.Name THEN 1 ELSE 0 END AS bIsIdentity, ATTR.IsNull AS bIsNullable, ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS iColumnOrder FROM XIBO_T_N  BO 
                                                     INNER JOIN XIBOATTRIBUTE_T_N ATTR ON BO.BOID = ATTR.BOID
                                                     WHERE bo.TableName NOT LIKE '{%' AND BO.iDataSource = @@@ AND ATTR.Name NOT IN ('zXCrtdBy', 'zXCrtdWhn', 'zXUpdtdBy', 'zXUpdtdWhn', 'izXDeleted', 'sHierarchy')";
        public const string sNotInQuery = " WHERE col.name NOT IN ('zXCrtdBy', 'zXCrtdWhn', 'zXUpdtdBy', 'zXUpdtdWhn', 'izXDeleted', 'sHierarchy', 'XIGUID')";
        public const string sWhereTablesInQuery = @"SELECT BO.TableName AS sTableName,  ATTR.Name AS sColumnName, ATTR.TypeID AS iType, CASE WHEN BO.sPrimaryKey = ATTR.Name THEN 1 ELSE 0 END AS bIsIdentity, ATTR.IsNull AS bIsNullable, ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS iColumnOrder FROM XIBO_T_N  BO 
                                                     INNER JOIN XIBOATTRIBUTE_T_N ATTR ON BO.BOID = ATTR.BOID and BO.Name in (@@@) AND ATTR.Name NOT IN ('zXCrtdBy', 'zXCrtdWhn', 'zXUpdtdBy', 'zXUpdtdWhn', 'izXDeleted', 'sHierarchy', 'XIGUID')";
        public const string sSystemQueryForBO = @"SELECT tbl.name AS sTableName, col.name AS sColumnName, col.is_identity AS bIsIdentity, col.is_nullable AS bIsNullable, typ.name as sDataType, ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS iColumnOrder
                       FROM SYS.COLUMNS col
                       INNER JOIN SYS.TABLES tbl  ON col.object_id = tbl.object_id and tbl.type = 'U'
                       LEFT JOIN SYS.DEFAULT_CONSTRAINTS def ON col.default_object_id != 0 AND col.object_id = def.parent_object_id AND col.column_id = def.parent_column_id
        INNER JOIN SYS.TYPES typ ON typ.user_type_id = col.user_type_id";
        #endregion Self_Database_Queries

        #region Covert_Datatable_to_List
        public List<T> DataTableToList<T>(DataTable OTable) where T : class, new()
        {
            CResult oCResult = new CResult();
            try
            {
                List<T> list = new List<T>();
                foreach (var row in OTable.AsEnumerable())
                {
                    T obj = new T();
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        var type = propertyInfo.PropertyType;
                        if (OTable.Columns.Contains(prop.Name))
                            propertyInfo.SetValue(obj, ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                return null;
            }
        }
        public object ChangeType(object value, Type t)
        {
            CResult oCResult = new CResult();
            try
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (value == null)
                        return null;
                    t = Nullable.GetUnderlyingType(t);
                }
                return Convert.ChangeType(value, t);
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }

        }

        #endregion Covert_Datatable_to_List

        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
          :: Generate Column script with starting character of Attribute ::
          :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        public string GetColumn(string Attribute)
        {
            CResult oCResult = new CResult();
            try
            {
                StringBuilder sString = new StringBuilder();
                if (Attribute.ToLower().StartsWith("i"))
                {
                    sString.Append("[" + Attribute + "] INT ");
                    if (Attribute.ToLower() == "id")
                        sString.Append("NOT NULL PRIMARY KEY IDENTITY(1,1)");
                    else
                        sString.Append("NULL");
                }
                else if (Attribute.ToLower().StartsWith("s")) sString.Append("[" + Attribute + "] VARCHAR(256) NOT NULL");
                else if (Attribute.ToLower().StartsWith("d")) sString.Append("[" + Attribute + "] DATETIME NULL");
                else if (Attribute.ToLower().StartsWith("r") || Attribute.ToLower().StartsWith("f")) sString.Append("[" + Attribute + "] FLOAT NULL");
                else if (Attribute.ToLower().StartsWith("n")) sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL");
                else if (Attribute.ToLower().StartsWith("b")) sString.Append("[" + Attribute + "] BIT NULL");
                else if (Attribute.ToLower().StartsWith("t")) sString.Append("[" + Attribute + "] VARCHAR(50) NULL");
                else if (Attribute.ToLower().StartsWith("fk"))
                {
                    if (Attribute.ToLower().IndexOf('i', 2) > 0) sString.Append("[" + Attribute + "] INT NULL");
                    if (Attribute.ToLower().IndexOf('s', 2) > 0) sString.Append("[" + Attribute + "] VARCHAR(256) NOT NULL");
                }
                return sString.ToString();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

        public string GetColumn(XITableColumns AttributeList)
        {
            CResult oCResult = new CResult();
            try
            {
                var Attribute = AttributeList.sColumnName;
                StringBuilder sString = new StringBuilder();
                if (AttributeList.bIsIdentity == true)
                {
                    if (AttributeList.sDataType.ToLower() == "int")
                        sString.Append("[" + Attribute + "] INT NOT NULL PRIMARY KEY IDENTITY(1,1)");
                    else if (AttributeList.sDataType.ToLower() == "bigint")
                        sString.Append("[" + Attribute + "] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1)");
                }
                else if (AttributeList.sDataType.ToLower() == "VARCHAR".ToLower())
                {
                    if (AttributeList.lMaxLength > 0)
                    {
                        if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                        {
                            if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] VARCHAR(" + AttributeList.lMaxLength + ") NOT NULL default" + AttributeList.sDefaultValue);
                            else
                                sString.Append("[" + Attribute + "] VARCHAR(" + AttributeList.lMaxLength + ") NULL default" + AttributeList.sDefaultValue);
                        }
                        else if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] VARCHAR(" + AttributeList.lMaxLength + ") Not NULL");
                        else
                            sString.Append("[" + Attribute + "] VARCHAR(" + AttributeList.lMaxLength + ") NULL");
                    }
                    else if (AttributeList.lMaxLength == -1)
                    {
                        if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                        {
                            if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] VARCHAR(MAX) NOT NULL default" + AttributeList.sDefaultValue);
                            else
                                sString.Append("[" + Attribute + "] VARCHAR(MAX) NULL default" + AttributeList.sDefaultValue);
                        }
                        else if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] VARCHAR(MAX) Not NULL");
                        else
                            sString.Append("[" + Attribute + "] VARCHAR(MAX) NULL");
                    }
                    else
                    {
                        sString.Append("[" + Attribute + "] VARCHAR(MAX) NULL");
                    }
                }
                else if (AttributeList.sDataType.ToLower() == "nvarchar")
                {
                    if (AttributeList.lMaxLength > 0)
                    {
                        if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                        {
                            if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength / 2 + ") NOT NULL default" + AttributeList.sDefaultValue);
                            else
                                sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength / 2 + ") NOT NULL default" + AttributeList.sDefaultValue);
                        }
                        else if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength / 2 + ") NOT NULL");
                        else
                            sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength / 2 + ") NULL");
                    }
                    else if (AttributeList.lMaxLength == -1)
                    {
                        if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                        {
                            if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL default" + AttributeList.sDefaultValue);
                            else
                                sString.Append("[" + Attribute + "] NVARCHAR(MAX) NULL default" + AttributeList.sDefaultValue);
                        }
                        else if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL");
                        else
                            sString.Append("[" + Attribute + "] NVARCHAR(MAX) NULL");
                    }
                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL");
                    else
                        sString.Append("[" + Attribute + "] NVARCHAR(MAX) NULL");
                }
                else if (AttributeList.sDataType.ToLower() == "int")
                {
                    if (Attribute.ToLower().StartsWith("fk"))
                    {
                        if (Attribute.ToLower().IndexOf('i', 1) > 0)
                        {
                            if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                            {
                                if (AttributeList.bIsNullable == false)
                                    sString.Append("[" + Attribute + "] INT NOT NULL default" + AttributeList.sDefaultValue);
                                else
                                    sString.Append("[" + Attribute + "] INT NULL default" + AttributeList.sDefaultValue);
                            }
                            else if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] INT not NULL");
                            else
                                sString.Append("[" + Attribute + "] INT NULL");
                        }
                        else if (Attribute.ToLower().IndexOf('s', 1) > 0)
                        {
                            if (AttributeList.lMaxLength > 0)
                            {
                                if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                                {
                                    if (AttributeList.bIsNullable == false)
                                        sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength + ") NOT NULL default" + AttributeList.sDefaultValue);
                                    else
                                        sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength + ") NULL");
                                }
                                else if (AttributeList.bIsNullable == false)
                                    sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength + ") NOT NULL");
                                else
                                    sString.Append("[" + Attribute + "] NVARCHAR(" + AttributeList.lMaxLength + ") NULL");
                            }
                            else if (AttributeList.lMaxLength == -1)
                                sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL");
                            else if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] NVARCHAR(MAX) NOT NULL");
                            else
                            {
                                sString.Append("[" + Attribute + "] NVARCHAR(50) NULL");
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                    {
                        if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] INT NOT NULL default" + AttributeList.sDefaultValue);
                        else
                            sString.Append("[" + Attribute + "] INT NULL default" + AttributeList.sDefaultValue);
                    }
                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] INT NOT NULL");
                    else
                        sString.Append("[" + Attribute + "] INT NULL");
                }
                else if (AttributeList.sDataType.ToLower() == "UNIQUEIDENTIFIER".ToLower())
                {
                    if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                    {
                        if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] UNIQUEIDENTIFIER not null default" + AttributeList.sDefaultValue);
                        else
                            sString.Append("[" + Attribute + "] UNIQUEIDENTIFIER null default" + AttributeList.sDefaultValue);
                    }
                    else
                        sString.Append("[" + Attribute + "] UNIQUEIDENTIFIER NULL");
                }
                else if (AttributeList.sDataType.ToLower() == "varbinary")
                {
                    if (!string.IsNullOrEmpty(AttributeList.sDefaultValue) && AttributeList.lMaxLength > 0)
                    {
                        if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] varbinary(" + AttributeList.lMaxLength + ") NOT NULL default" + AttributeList.sDefaultValue);
                        else
                            sString.Append("[" + Attribute + "] varbinary(" + AttributeList.lMaxLength + ") NULL default" + AttributeList.sDefaultValue);
                    }
                    else if (AttributeList.lMaxLength == -1)
                        sString.Append("[" + Attribute + "] varbinary(MAX) NOT NULL");
                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] varbinary(MAX) NOT NULL");
                    else
                        sString.Append("[" + Attribute + "] varbinary(MAX) NULL");
                }
                else if (AttributeList.sDataType.ToLower() == "decimal")
                {
                    if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                    {
                        if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] decimal(18,6) NOT NULL default" + AttributeList.sDefaultValue);
                        else
                            sString.Append("[" + Attribute + "] decimal(18,6) NULL default" + AttributeList.sDefaultValue);
                    }

                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] decimal(18,6) NOT NULL");
                    else
                        sString.Append("[" + Attribute + "] decimal(18,6) NULL");
                }
                else if (AttributeList.sDataType.ToLower() == "nchar")
                {

                    if (AttributeList.lMaxLength > 0)
                    {
                        if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                        {
                            if (AttributeList.bIsNullable == false)
                                sString.Append("[" + Attribute + "] nchar(" + AttributeList.lMaxLength / 2 + ") NOT NULL default" + AttributeList.sDefaultValue);
                            else
                                sString.Append("[" + Attribute + "] nchar(" + AttributeList.lMaxLength / 2 + ") NULL");
                        }
                        else if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] nchar(" + AttributeList.lMaxLength / 2 + ") NOT NULL");
                        else
                            sString.Append("[" + Attribute + "] nchar(" + AttributeList.lMaxLength / 2 + ") NULL");
                    }
                    else if (AttributeList.lMaxLength == -1)
                        sString.Append("[" + Attribute + "] nchar(MAX) NOT NULL");
                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] nchar(MAX) NOT NULL");
                    else
                    {
                        sString.Append("[" + Attribute + "] nchar(50) NULL");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(AttributeList.sDefaultValue))
                    {
                        if (AttributeList.bIsNullable == false)
                            sString.Append("[" + Attribute + "] " + AttributeList.sDataType + " NOT NULL default" + AttributeList.sDefaultValue);
                        else
                            sString.Append("[" + Attribute + "] " + AttributeList.sDataType + " NULL default" + AttributeList.sDefaultValue);
                    }

                    else if (AttributeList.bIsNullable == false)
                        sString.Append("[" + Attribute + "] " + AttributeList.sDataType + " NOT NULL");
                    else
                        sString.Append("[" + Attribute + "] " + AttributeList.sDataType + " NULL");
                }
                return sString.ToString();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                throw ex;
            }
        }

    }
}