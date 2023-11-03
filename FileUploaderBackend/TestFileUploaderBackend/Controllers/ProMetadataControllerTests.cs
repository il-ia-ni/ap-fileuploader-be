using FileUploaderBackend.Controllers;
using FileUploaderBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProLibrary.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestFileUploaderBackend.Controllers
{
    public class ProMetadataControllerTests
    {
        private MockRepository mockFactory;

        private Mock<IProRepository> mockProRepository;

        public ProMetadataControllerTests()
        {
            // var repository = new Mock<IProRepository>();

            mockFactory = new MockRepository(MockBehavior.Strict);  // Factory to create and set up behaviours of all mocks of the Testing cls
            mockProRepository = mockFactory.Create<IProRepository>();
        }

        private ProMetadataController CreateProMetadataController()
        {
            return new ProMetadataController(
                mockProRepository.Object);
        }

        [Fact]
        public void GetTableSchema_RUT_ExpectedBehavior()
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

            mockProRepository.Setup(r => r.GetTableSchema(tableName)).Returns(expectedSchema);

            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = proMetadataController.GetTableSchema(tableName);  // UUT

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(actionResult.Value);
            Assert.Equal(expectedSchema, actionResult.Value);
        }

        [Fact]
        public void GetTableSchema_SUT_NotFound_ExpectedBehavior()
        {
            // Arrange
            var tableName = "NotExistantTable";

            mockProRepository.Setup(r => r.GetTableSchema(tableName)).Returns(new Dictionary<string, string>());

            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = proMetadataController.GetTableSchema(tableName);  // UUT

            // Assert
            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetMetadata_RUT_ExpectedBehavior()
        {
            // Arrange
            mockProRepository.Setup(r => r.GetAllMetadataItems()).ReturnsAsync(new List<DcMetadata>());
            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = await proMetadataController.GetMetadata();  // UUT

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode);
            mockFactory.VerifyAll();  // makes sure that all assertions in set ups were fullfilled
        }

        [Fact]
        public async Task GetMetadata_SUT_UnexpectedBehavior_InternalServerThrown()
        {
            // Arrange
            mockProRepository.Setup(r => r.GetAllMetadataItems()).Throws(new Exception("Test exception"));
            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = await proMetadataController.GetMetadata();  // UUT

            // Assert
            var actionResult = result.Result;
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, statusCodeResult.StatusCode);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task GetMetadataById_RUT_ExpectedBehavior()
        {
            // Arrange
            int mdId = 1;
            var mdResponseResult = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };

            mockProRepository.Setup(r => r.GetSingleMetadataItem(mdId)).ReturnsAsync(mdResponseResult);
            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = await proMetadataController.GetMetadataById(mdId);  // UUT

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var metadata = Assert.IsType<DcMetadata>(objectResult.Value);  // includes not null check
            Assert.Equal(1, metadata.ItemId);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task GetMetadataById_RUT_ExpectedBehavior_NotFound()
        {
            // Arrange
            int mdId = 1;

            mockProRepository.Setup(r => r.GetSingleMetadataItem(It.IsAny<int>())).ReturnsAsync((DcMetadata)null);
            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.GetMetadataById(mdId);

            // Assert
            var objectResult = Assert.IsType<NotFoundResult>(result.Result);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task GetMetadataById_SUT_UnexpectedBehavior_InternalServerThrown()
        {
            // Arrange
            int mdId = 1;

            mockProRepository.Setup(r => r.GetSingleMetadataItem(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));
            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.GetMetadataById(mdId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objectResult!.StatusCode);

            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task CreateMetadata_RUT_ExpectedBehavior()
        {
            // Arrange
            var metadata = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };

            mockProRepository.Setup(r => r.CreateMetadataItem(It.IsAny<DcMetadata>())).Returns(Task.CompletedTask);
            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = await proMetadataController.CreateMetadata(metadata);  // UUT

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdMetadata = Assert.IsType<DcMetadata>(createdAtActionResult.Value);
            Assert.Equal(metadata.ItemId, createdMetadata.ItemId);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task CreateMetadata_RUT_ExpectedBehavior_BadRequest()
        {
            // Arrange
            var proMetadataController = CreateProMetadataController();  // CUT
            proMetadataController.ModelState.AddModelError("ItemName", "ItemName is required");

            // Act
            var result = await proMetadataController.CreateMetadata(null); // Übergeben eines ungültigen Modells

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data provided", badRequestResult.Value);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task CreateMetadata_SUT_UnexpectedBehavior_InternalServerThrown()
        {
            // Arrange
            var metadata = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };

            mockProRepository.Setup(r => r.CreateMetadataItem(It.IsAny<DcMetadata>())).ThrowsAsync(new Exception("Test exception"));
            var proMetadataController = CreateProMetadataController();  // CUT

            // Act
            var result = await proMetadataController.CreateMetadata(metadata);  // UUT

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task DeleteMetadata_RUT_ExpectedBehavior()
        {
            // Arrange
            mockProRepository.Setup(r => r.DeleteAllMdItems()).Returns(Task.CompletedTask);
            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.DeleteMetadata();  // UUT

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task DeleteMetadata_SUT_UnexpectedBehavior_InternalServerThrown()
        {
            // Arrange
            mockProRepository.Setup(r => r.DeleteAllMdItems()).ThrowsAsync(new Exception("Test exception"));
            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.DeleteMetadata();  // UUT

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task DeleteMetadataById_RUT_ExpectedBehavior()
        {
            // Arrange
            int mdId = 1;
            var existingMetadata = new DcMetadata { ItemId = mdId, /* andere Eigenschaften? */ };

            mockProRepository.Setup(r => r.GetSingleMetadataItem(mdId)).ReturnsAsync(existingMetadata);
            mockProRepository.Setup(r => r.DeleteSingleMetadataItem(mdId)).Returns(Task.CompletedTask);

            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.DeleteMetadataById(mdId);  // UUT

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task DeleteMetadataById_RUT_ExpectedBehavior_NotFound()
        {
            // Arrange
            int mdId = 1;

            mockProRepository.Setup(r => r.GetSingleMetadataItem(mdId)).ReturnsAsync((DcMetadata)null);

            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.DeleteMetadataById(mdId);  // UUT

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task UpdateMetadata_RUT_ExpectedBehavior()
        {
            // Arrange
            int mdId = 1;
            var existingMetadata = new DcMetadata { ItemId = mdId, /* andere Eigenschaften? */ };
            var updatedMetadata = new DcMetadata { ItemId = mdId, /* aktualisierte Eigenschaften? */ };

            mockProRepository.Setup(r => r.GetSingleMetadataItem(mdId)).ReturnsAsync(existingMetadata);
            mockProRepository.Setup(r => r.UpdateMetadataItem(mdId, updatedMetadata)).Returns(Task.CompletedTask);

            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.UpdateMetadata(mdId, updatedMetadata);  // UUT

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockFactory.VerifyAll();
        }

        [Fact]
        public async Task UpdateMetadata_RUT_ExpectedBehavior_NotFound()
        {
            // Arrange
            int mdId = 1;
            var updatedMetadata = new DcMetadata { ItemId = mdId, /* aktualisierte Eigenschaften? */ };

            mockProRepository.Setup(r => r.GetSingleMetadataItem(mdId)).ReturnsAsync((DcMetadata)null);

            var proMetadataController = CreateProMetadataController();  // SUT

            // Act
            var result = await proMetadataController.UpdateMetadata(mdId, updatedMetadata);  // UUT

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockFactory.VerifyAll();
        }
    }
}
