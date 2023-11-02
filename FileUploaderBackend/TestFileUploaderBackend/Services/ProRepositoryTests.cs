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
            this.mockFactory = new MockRepository(MockBehavior.Strict);

            //this.mockPROContext = this.mockFactory.Create<fakeDbContext>();
            var inmemoryDbName = $"proMd_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<PROContext>()
                .UseInMemoryDatabase(inmemoryDbName)
                .Options;
            fakeDbContext = new PROContext(dbContextOptions);

            this.mockLogger = this.mockFactory.Create<ILogger<ProRepository>>();
        }

        public void Dispose()
        {
            fakeDbContext.Database.EnsureDeleted();
            fakeDbContext.Dispose();
        }

        private async Task<ProRepository> CreateProRepositoryAsync()
        {
            await PopulateDataInMemoryAsync(fakeDbContext);
            return new ProRepository(
                fakeDbContext,
                this.mockLogger.Object);
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
        public async Task GetAllMetadataItems_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            var result = await proRepository.GetAllMetadataItems();  // UUT

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);

            this.mockFactory.VerifyAll();
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

            this.mockFactory.VerifyAll();
        }

        [Fact]
        public async Task CreateMetadataItem_RUT_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();  // CUT

            // Act
            await proRepository.CreateMetadataItem(new DcMetadata()
            {
                ItemName = "test_4",
            });  // UUT
            var result = await proRepository.GetAllMetadataItems();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count);
            Assert.Equal("test_4", result.Single(i => i.ItemId == 4).ItemName);

            this.mockFactory.VerifyAll();
        }

        [Fact]
        public async Task UpdateMetadataItem_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var proRepository = await CreateProRepositoryAsync();
            

            this.mockFactory.VerifyAll();
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
            this.mockFactory.VerifyAll();
        }
    }
}
