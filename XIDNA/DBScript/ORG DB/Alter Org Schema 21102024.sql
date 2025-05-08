-- Please do add all alter or update commands of Org table in this FILE

/*
--Date - USER - task
--Commands

IF NOT EXISTS (
  SELECT
    *
  FROM
    INFORMATION_SCHEMA.COLUMNS
  WHERE
    TABLE_NAME = 'table_name' AND COLUMN_NAME = 'col_name')
BEGIN
  ALTER TABLE table_name
    ADD col_name data_type NULL
END;
Or this shorter query:

IF COL_LENGTH ('schema_name.table_name.col_name') IS NULL
BEGIN
  ALTER TABLE table_name
    ADD col_name data_type NULL
END;
*/