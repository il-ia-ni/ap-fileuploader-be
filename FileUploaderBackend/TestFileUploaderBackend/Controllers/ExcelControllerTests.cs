using FileUploaderBackend.Controllers;
using FileUploaderBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestFileUploaderBackend.Controllers
{
    public class ExcelControllerTests
    {
        private MockRepository mockFactory;

        private Mock<IExcelReaderService> mockExcelReaderService;

        public ExcelControllerTests()
        {
            mockFactory = new MockRepository(MockBehavior.Strict);

            mockExcelReaderService = mockFactory.Create<IExcelReaderService>();
        }

        private ExcelController CreateExcelController()
        {
            return new ExcelController(
                this.mockExcelReaderService.Object);
        }

        [Theory]
        [InlineData("sample.xlsx", true)] // Gültige Excel-Datei
        [InlineData("sample.xls", false)]  // Ungültige Excel-Datei
        [InlineData("sample.txt", false)]  // Eine Textdatei (nicht Excel)
        public async Task UploadExcel_SUT_FormatValidationTests(string fileName, bool isSupportedFormat)
        {
            // Arrange
            var excelController = CreateExcelController();  // CUT

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(10);

            var stubDataTable = new DataTable("testDataTable");
            stubDataTable.Columns.Add("col1", typeof(int));
            stubDataTable.Columns.Add("col2", typeof(string));

            mockExcelReaderService.Setup(er => er.ReadExcelFile(It.IsAny<Stream>(), It.IsAny<Encoding>()))
                .Returns(stubDataTable);

            // Act
            var result = await excelController.UploadExcel(mockFile.Object);  // UUT

            // Assert
            if (isSupportedFormat)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnedDataTable = Assert.IsType<DataTable>(okResult.Value);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Only .xlsx files are supported.", badRequestResult.Value);
            }
        }

        [Theory]
        [InlineData(0, false)] // eine leere Excel-Datei
        [InlineData(10, true)]  // eine nicht leere Excel-Datei
        public async Task UploadExcel_SUT_FileSizeValidationTests(int byteLength, bool isNotEmpty)
        {
            // Arrange
            var excelController = CreateExcelController();  // CUT

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.xlsx");
            mockFile.Setup(f => f.Length).Returns(byteLength);

            var stubDataTable = new DataTable("testDataTable");
            stubDataTable.Columns.Add("col1", typeof(int));
            stubDataTable.Columns.Add("col2", typeof(string));

            mockExcelReaderService.Setup(er => er.ReadExcelFile(It.IsAny<Stream>(), It.IsAny<Encoding>()))
                .Returns(stubDataTable);

            // Act
            var result = await excelController.UploadExcel(mockFile.Object);  // UUT

            // Assert
            if (isNotEmpty)
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnedDataTable = Assert.IsType<DataTable>(okResult.Value);
            }
            else
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("No file uploaded.", badRequestResult.Value);
            }
        }
    }
}
