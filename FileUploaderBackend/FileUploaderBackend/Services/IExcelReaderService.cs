using System.Data;
using System.Text;

namespace FileUploaderBackend.Services
{
    public interface IExcelReaderService
    {
        DataTable ReadExcelFile(Stream fileStream, Encoding encoding);
    }
}