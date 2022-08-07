using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CommonHelper
{
    public static class ConvertDataTableToList
    {
        public static (string, List<T>) ConvertDataTable<T>(DataTable dt)
        {
            try
            {
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return ("", data);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return (ex.Message, null);
            }

        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            int counter = 0;

            foreach (DataColumn column in dr.Table.Columns)
            {
                if (column != null)
                {
                    string columnName = "Column" + counter.ToString();
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName.Replace(" ", string.Empty))
                        {
                            if (dr[column.ColumnName] != System.DBNull.Value)
                            {
                                var columnData = dr[column.ColumnName];
                                pro.SetValue(obj, columnData.ToString(), null);
                            }
                            else
                            {
                                pro.SetValue(obj, string.Empty, null);
                            }


                        }

                        else
                            continue;
                    }
                }

            }
            return obj;
        }
    }
}
