using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using ProLibrary.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace FileUploaderBackend.Services
{
    public class ProRepository : IProRepository
    {
        private readonly PROContext _dbContext;
        private readonly ILogger<ProRepository> _logger;

        public ProRepository(PROContext dbContext, ILogger<ProRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ICollection<DcMetadata>> GetAllMetadataItems()
        {
            try
            {
                return await _dbContext.Set<DcMetadata>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving metadata entities");
                throw;
            }
        }

        public async Task<DcMetadata?> GetSingleMetadataItem(int mdId)
        {
            try
            {
                return await _dbContext.DcMetadata.FindAsync(mdId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving the specified metadata entity");
                throw;
            }
        }

        public async Task CreateMetadataItem(DcMetadata mdEntity)
        {
            if (mdEntity == null)
            {
                throw new ArgumentNullException($"{nameof(mdEntity)} entity must not be null");
            }
            try
            {
                await _dbContext.DcMetadata.AddAsync(mdEntity);
                await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(mdEntity)} could not be added");
                throw;
            }
        }

        public async Task UpdateMetadataItem(int mdId, DcMetadata mdEntity)
        {
            try
            {
                DcMetadata mdItemStored = await GetSingleMetadataItem(mdId);

                mdItemStored.ItemSource = String.IsNullOrEmpty(mdEntity.ItemSource) ? mdItemStored.ItemSource : mdEntity.ItemSource;
                mdItemStored.Host = String.IsNullOrEmpty(mdEntity.Host) ? mdItemStored.Host : mdEntity.Host;
                mdItemStored.ItemContainer = String.IsNullOrEmpty(mdEntity.ItemContainer) ? mdItemStored.ItemContainer : mdEntity.ItemContainer;
                mdItemStored.ItemName = String.IsNullOrEmpty(mdEntity.ItemName) ? mdItemStored.ItemName : mdEntity.ItemName;
                mdItemStored.ItemType = String.IsNullOrEmpty(mdEntity.ItemType) ? mdItemStored.ItemType : mdEntity.ItemType;
                mdItemStored.ItemComment = String.IsNullOrEmpty(mdEntity.ItemComment) ? mdItemStored.ItemComment : mdEntity.ItemComment;
                mdItemStored.Unit = String.IsNullOrEmpty(mdEntity.Unit) ? mdItemStored.Unit : mdEntity.Unit;


                mdItemStored.Scaling = mdEntity.Scaling is null ? mdItemStored.Scaling : mdEntity.Scaling;
                mdItemStored.UpdateCycle = mdEntity.UpdateCycle is null ? mdItemStored.UpdateCycle : mdEntity.UpdateCycle;

                mdItemStored.Sensor = String.IsNullOrEmpty(mdEntity.Sensor) ? mdItemStored.Sensor : mdEntity.Sensor;

                mdItemStored.MinVal = mdEntity.MinVal is null ? mdItemStored.MinVal : mdEntity.MinVal;
                mdItemStored.MaxVal = mdEntity.MaxVal is null ? mdItemStored.MaxVal : mdEntity.MaxVal;

                mdItemStored.Orientation = String.IsNullOrEmpty(mdEntity.Orientation) ? mdItemStored.Orientation : mdEntity.Orientation;


                await Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating metadata item");
                throw;
            }
        }

        public async Task DeleteAllMdItems()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [DC_METADATA]");
            // For raw sql in EF core 6 see https://stackoverflow.com/a/18986676
            await Save();
        }

        public async Task DeleteSingleMetadataItem(int mdId)
        {
            DcMetadata mdItem = await GetSingleMetadataItem(mdId);
            _dbContext.DcMetadata.Remove(mdItem);
            await Save();
        }

        public async Task Save()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict");
                // Hier die Konfliktauflösung implementieren (falls gebraucht)
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                throw;
            }
        }
    }
}
