using FileUploaderBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using ProLibrary.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestFileUploaderBackend.Services
{
    public class ProRepositoryTests : IDisposable
    {
        private MockRepository mockFactory;

        private PROContext fakeDbContext;
        private Mock<ILogger<ProRepository>> mockLogger;

        private DbContextOptions<PROContext> dbContextOptions;

        public ProRepositoryTests()
        {
            mockFactory = new MockRepository(MockBehavior.Strict);

            //mockPROContext = mockFactory.Create<fakeDbContext>();
            var inmemoryDbName = $"proMd_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<PROContext>()
                .UseInMemoryDatabase(inmemoryDbName)
                .Options;
            fakeDbContext = new PROContext(dbContextOptions);

            mockLogger = mockFactory.Create<ILogger<ProRepository>>();
        }

        public void Dispose()
        {
            fakeDbContext.Database.EnsureDeleted();
            fakeDbContext.Dispose();
        }

        private async Task<ProRepository> CreateProRepositoryAsync()
        {
            await PopulateDataInMemoryAsync(fakeDbContext);
            return new ProRepository(fakeDbContext, mockLogger.Object);
        }

        private async Task PopulateDataInMemoryAsync(PROContext context)
        {
            int idx = 1;

            while (idx <= 3)
            {
                var md = new DcMetadata()
                {
                    ItemId = idx,
                };

                idx++;
                await context.DcMetadata.AddAsync(md);
            }

            await context.SaveChangesAsync();
        }

        [Fact]
        public async void GetTableSchema_ReturnsTableSchema()
        {
            // Arrange
            var tableName = "DcMetadata"; 
            var expectedSchema = new Dictionary<string, string>
            {
                { "ItemId", "Int32" },
                { "Host", "String" },
                { "ItemComment", "String" },
                { "ItemContainer", "String" },
                { "ItemName", "String" },
                { "ItemSource", "String" },
                { "ItemType", "String" },
                { "LastModifiedAt", "DateTime" },
                { "MaxVal", "Nullable`1" },
                { "MinVal", "Nullable`1" },
                { "Orientation", "String" },
                { "Scaling", "Nullable`1" },
                { "Sensor", "String" },
                { "Unit", "String" },
                { "UpdateCycle", "Nullable`1" },
            };

            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            var result = proRepository.GetTableSchema(tableName);  // UUT

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSchema, result);
        }

        [Fact]
        public async Task GetAllMetadataItems_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            var result = await proRepository.GetAllMetadataItems();  // UUT

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);

            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task GetSingleMetadataItem_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            var result = await proRepository.GetSingleMetadataItem(1);  // UUT

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ItemId);

            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task CreateMetadataItem_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            await proRepository.CreateMetadataItem(new DcMetadata()
            {
                ItemId = 4,
                ItemName = "test_4",
            });  // UUT
            var result = await proRepository.GetAllMetadataItems();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count);
            Assert.Equal("test_4", result.Single(i => i.ItemId == 4).ItemName);

            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task UpdateMetadataItem_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT
            var testUpdMd = new DcMetadata() { ItemName = "test_update"};

            // Act
            await proRepository.UpdateMetadataItem(1, testUpdMd);  // UUT
            var result = await proRepository.GetAllMetadataItems();

            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("test_update", result.Single(i => i.ItemId == 1).ItemName);

            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task DeleteSingleMetadataItem_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT
            var testMd = new DcMetadata() { ItemId = 1 };

            // Act
            await proRepository.DeleteSingleMetadataItem(testMd.ItemId);  // UUT
            var result = await proRepository.GetAllMetadataItems();

            // Assert
            Assert.DoesNotContain(testMd, result);
            mockFactory.VerifyAll();
        }
    }
}
