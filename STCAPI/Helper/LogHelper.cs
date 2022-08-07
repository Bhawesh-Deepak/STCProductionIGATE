namespace STCAPI.Helper
{
    public class LogHelper
    {
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public string TableName { get; set; }
        public string CreatedBy { get; set; }
        public string Description { get; set; }

        public void UploadLogDetail()
        {
            var logDescription = $"The ModuleName: {ModuleName} SubMoudleName: {SubModuleName} " +
                $"tableName: {TableName} has been updatedBy/CreatedBy {CreatedBy} with Details {Description} ";
        }
    }
}
