using ExcelDataReader;
using System.Data;
using System.IO;
using System.Text;

namespace FileUploaderBackend.Services
{
    public class ExcelReaderService : IExcelReaderService
    {
        public DataTable ReadExcelFile(Stream fileStream, Encoding fallbackEncoding)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  // Stellt einen Encoding Provider bereit,
                                                                            // der Code Pages für Zeichencodierungen wie UTF-8,
                                                                            // UTF-16 und andere hinzufügt, die möglicherweise
                                                                            // nicht automatisch erkannt werden.

            using (var reader = ExcelReaderFactory.CreateReader(fileStream, new ExcelReaderConfiguration()
            {
                FallbackEncoding = fallbackEncoding,
                AutodetectSeparators = new char[] { ',', ';', '\t' }
            }))
            {
                var dataSetConfig = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        EmptyColumnNamePrefix = "col",
                        UseHeaderRow = false
                    }
                };

                var result = reader.AsDataSet(dataSetConfig);
                if (result.Tables.Count > 0)
                {
                    return result.Tables[0];
                }
                return null;
            }
        }
    }
}
