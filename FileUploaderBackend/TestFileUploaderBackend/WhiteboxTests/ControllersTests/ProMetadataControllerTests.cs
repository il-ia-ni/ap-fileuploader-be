using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProLibrary.Models;
using FileUploaderBackend.Controllers;
using FileUploaderBackend.Services;

namespace TestFileUploaderBackend.WhiteboxTests.ControllersTests
{
    public class ProMetadataControllerTests
    {
        [Fact]
        public async Task GetAllMetadataItems_ReturnsOkResult()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            repository.Setup(r => r.GetAllMetadataItems()).ReturnsAsync(new List<DcMetadata>());

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.GetAllMetadataItems();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSingleMetadataItem_ReturnsNotFoundResult()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            repository.Setup(r => r.GetSingleMetadataItem(It.IsAny<int>())).ReturnsAsync((DcMetadata)null);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.GetSingleMetadataItem(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateMetadataItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            var metadata = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };
            repository.Setup(r => r.CreateMetadataItem(It.IsAny<DcMetadata>())).Returns(Task.CompletedTask);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.CreateMetadataItem(metadata);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdMetadata = Assert.IsType<DcMetadata>(createdAtActionResult.Value);
            Assert.Equal(metadata.ItemId, createdMetadata.ItemId);
            // Weitere Eigenschaften überprüfen?
        }

        [Fact]
        public async Task CreateMetadataItem_ReturnsBadRequestResult_WhenModelIsInvalid()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            var controller = new ProMetadataController(repository.Object);
            controller.ModelState.AddModelError("ItemName", "ItemName is required");

            // Act
            var result = await controller.CreateMetadataItem(null); // Übergeben eines ungültigen Modells

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateMetadataItem_ReturnsNoContentResult()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            var metadata = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };
            repository.Setup(r => r.UpdateMetadataItem(1, It.IsAny<DcMetadata>())).Returns(Task.CompletedTask);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.UpdateMetadataItem(1, metadata);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateMetadataItem_ReturnsNotFoundResult_WhenMetadataNotFound()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            repository.Setup(r => r.GetSingleMetadataItem(1)).ReturnsAsync((DcMetadata)null);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.UpdateMetadataItem(1, new DcMetadata());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteSingleMetadataItem_ReturnsNoContentResult()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            var metadata = new DcMetadata { ItemId = 1, /* andere Eigenschaften? */ };
            repository.Setup(r => r.GetSingleMetadataItem(1)).ReturnsAsync(metadata);
            repository.Setup(r => r.DeleteSingleMetadataItem(1)).Returns(Task.CompletedTask);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.DeleteSingleMetadataItem(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSingleMetadataItem_ReturnsNotFoundResult_WhenMetadataNotFound()
        {
            // Arrange
            var repository = new Mock<IProRepository>();
            repository.Setup(r => r.GetSingleMetadataItem(1)).ReturnsAsync((DcMetadata)null);

            var controller = new ProMetadataController(repository.Object);

            // Act
            var result = await controller.DeleteSingleMetadataItem(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
