using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using ProLibrary.Models;

namespace FileUploaderBackend.Services
{
    public class ProRepository : IProRepository
    {
        private readonly PROContext _dbContext;

        public ProRepository(PROContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<ICollection<DcMetadata>> GetAllMetadataItems()
        {
            try
            {
                return await _dbContext.Set<DcMetadata>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve metadata entities: {ex.Message}");
            }
        }

        public virtual async Task<DcMetadata?> GetSingleMetadataItem(int mdId)
        {

            try
            {
                return await _dbContext.DcMetadata.FindAsync(mdId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve the specified metadata entity: {ex.Message}");
            }
        }

        public virtual async Task CreateMetadataItem(DcMetadata mdEntity)
        {
            if (mdEntity == null)
            {
                throw new ArgumentNullException($"{nameof(mdEntity)} entity must not be null");
            }
            try
            {
                if (mdEntity != null)
                {
                    await _dbContext.DcMetadata.AddAsync(mdEntity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(mdEntity)} could not be added: {ex.Message}");
            }
        }

        public async void UpdateMetadataItem(int mdId, DcMetadata mdEntity)
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


            await SaveWithPessimisticConcurrency();
        }

        public void DeleteAllMdItems()
        {
            _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [DC_METADATA]");
            // For raw sql in EF core 6 see https://stackoverflow.com/a/18986676
            Save();
        }

        public void DeleteSingleMetadataItem(int mdId)
        {
            _dbContext.DcMetadata.Remove(GetSingleMetadataItem(mdId).Result);
            SaveWithPessimisticConcurrency();
        }

        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }

        public virtual async Task<int> SaveWithPessimisticConcurrency()
        {
            int records = 0;
            IDbContextTransaction tx = null;

            try
            {
                using (tx = await _dbContext.Database.BeginTransactionAsync())
                {
                    records = await _dbContext.SaveChangesAsync();
                    tx.Commit();
                    return records;
                }
            }
            catch (DbUpdateConcurrencyException ex)  // area for resolving a concurrency conflict (thrown only on update and delete, never on add): https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is DcMetadata)
                    {
                        var originalValues = entry.OriginalValues;  // originally retrieved before any edits 
                        var proposedValues = entry.CurrentValues;  // application was attempting to write
                        var databaseValues = entry.GetDatabaseValues();  // currently stored in the database

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // TODO: decide which value should be written to database
                            proposedValues[property] = proposedValue;
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);

                        await SaveWithPessimisticConcurrency();
                    }
                    else
                    {
                        throw new NotSupportedException("Unable to save changes. The metadata item details were updated by another user. " + entry.Metadata.Name);
                    }
                }
                throw ex;
            }
            catch (DbUpdateException ex)
            {
                SqlException s = ex.InnerException as SqlException;
                var errorMessage = $"{ex.Message}" + " {ex?.InnerException.Message}" + " rolling back…";
                tx.Rollback();
            }
            return records;
        }
    }
}
