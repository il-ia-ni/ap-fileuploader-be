using FileUploaderBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace FileUploaderBackend.Controllers
{
    // [Authorize(Roles = "admin,user")]
    [Authorize(Policy = "AdminOrUser")]
    [Route("[controller]")]
    [ApiController]
    public class ExcelController : Controller
    {
        private readonly IExcelReaderService _excelService;

        public ExcelController(IExcelReaderService excelService)
        {
            _excelService = excelService;
        }

        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataTable))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                if (Path.GetExtension(file.FileName) != ".xlsx")
                {
                    return BadRequest("Only .xlsx files are supported.");
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    var fallbackEncoding = Encoding.UTF8;

                    var dataTable = _excelService.ReadExcelFile(stream, fallbackEncoding);

                    if (dataTable != null)
                    {
                        return Ok(dataTable);
                    }
                    else
                    {
                        return BadRequest("Failed to read the Excel file.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}
