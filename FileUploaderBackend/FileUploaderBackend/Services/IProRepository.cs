﻿using ProLibrary.Models;

namespace FileUploaderBackend.Services
{
    public interface IProRepository
    {
        IDictionary<string, string> GetTableSchema(string tableName);
        Task CreateMetadataItem(DcMetadata mdEntity);
        Task DeleteAllMdItems();
        Task DeleteSingleMetadataItem(int mdId);
        Task<ICollection<DcMetadata>> GetAllMetadataItems();
        Task<DcMetadata?> GetSingleMetadataItem(int mdId);
        Task Save();
        Task UpdateMetadataItem(int mdId, DcMetadata mdEntity);
    }
}