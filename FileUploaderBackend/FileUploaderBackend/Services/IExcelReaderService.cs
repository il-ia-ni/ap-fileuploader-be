namespace FileUploaderBackend.Services
{
    public interface IExcelReaderService
    {
        List<Dictionary<string, string>> ReadExcelFile(Stream fileStream);
    }
}