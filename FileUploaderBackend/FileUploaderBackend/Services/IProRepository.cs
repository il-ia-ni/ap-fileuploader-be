using ProLibrary.Models;

namespace FileUploaderBackend.Services
{
    public interface IProRepository
    {
        Task CreateMetadataItem(DcMetadata mdEntity);
        void DeleteAllMdItems();
        void DeleteSingleMetadataItem(int mdId);
        Task<ICollection<DcMetadata>> GetAllMetadataItems();
        Task<DcMetadata?> GetSingleMetadataItem(int mdId);
        void Save();
        Task<int> SaveWithPessimisticConcurrency();
        void UpdateMetadataItem(int mdId, DcMetadata mdEntity);
    }
}