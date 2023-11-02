using ExcelDataReader;
using System.Data;

namespace FileUploaderBackend.Services
{
    public class ExcelReaderService : IExcelReaderService
    {
        public List<Dictionary<string, string>> ReadExcelFile(Stream fileStream)
        {
            List<Dictionary<string, string>> excelData = new List<Dictionary<string, string>>();

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var result = reader.AsDataSet();
                var dataTable = result.Tables[0];

                if (dataTable != null)
                {
                    var columns = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var rowData = new Dictionary<string, string>();
                        for (var i = 0; i < columns.Count; i++)
                        {
                            rowData[columns[i]] = Convert.ToString(row[i]);
                        }
                        excelData.Add(rowData);
                    }
                }
            }

            return excelData;
        }
    }
}
