using Microsoft.AspNetCore.Mvc;
using ProLibrary.Models;
using FileUploaderBackend.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FileUploaderBackend.Controllers
{
    // [Authorize(Roles = "admin,user")]
    [Authorize(Policy = "AdminOrUser")]
    [Route("[controller]")]
    [ApiController]
    public class ProMetadataController : Controller
    {
        private readonly IProRepository _repository;

        public ProMetadataController(IProRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("schema/{tableName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Dictionary<string, string>> GetTableSchema(string tableName)
        {
            try
            {
                var tableSchema = _repository.GetTableSchema(tableName);
                if (tableSchema.Count > 0)
                {
                    return Ok(tableSchema);
                }
                else
                {
                    return NotFound("Table not found or has no columns.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DcMetadata>>> GetMetadata()
        {
            try
            {
                var metadataItems = await _repository.GetAllMetadataItems();
                return Ok(metadataItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{mdId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DcMetadata>> GetMetadataById(int mdId)
        {
            try
            {
                var metadataItem = await _repository.GetSingleMetadataItem(mdId);
                if (metadataItem == null)
                {
                    return NotFound();
                }
                return Ok(metadataItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateMetadata([FromBody] DcMetadata mdEntity)
        {
            if (mdEntity == null)  // if data couldn't be parsed to DcMetadata type
            {
                return BadRequest("Invalid data provided");
            }

            try
            {
                await _repository.CreateMetadataItem(mdEntity);
                return CreatedAtAction("GetMetadataById", new { mdId = mdEntity.ItemId }, mdEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{mdId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateMetadata(int mdId, [FromBody] DcMetadata mdEntity)
        {
            try
            {
                var existingMetadata = await _repository.GetSingleMetadataItem(mdId);
                if (existingMetadata == null)
                {
                    return NotFound();
                }

                await _repository.UpdateMetadataItem(mdId, mdEntity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("all")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteMetadata()
        {
            try
            {
                await _repository.DeleteAllMdItems();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{mdId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteMetadataById(int mdId)
        {
            try
            {
                var existingMetadata = await _repository.GetSingleMetadataItem(mdId);
                if (existingMetadata == null)
                {
                    return NotFound();
                }

                await _repository.DeleteSingleMetadataItem(mdId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
