using System.Data;

namespace CommonHelper
{
    public static class DataTableHelper
    {
        public static DataTable MakeFirstRowAsColumnName(DataTable dt)
        {
            foreach (DataColumn column in dt.Columns)
            {
                string cName = dt.Rows[0][column.ColumnName].ToString();
                if (!dt.Columns.Contains(cName) && cName != "")
                {
                    column.ColumnName = cName;
                }
            }

            //dt.Rows[0].Delete(); 
            return dt;
        }
    }
}
