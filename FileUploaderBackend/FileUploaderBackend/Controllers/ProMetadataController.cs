using Microsoft.AspNetCore.Mvc;
using ProLibrary.Models;
using FileUploaderBackend.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProMetadataController : Controller
    {
        private readonly IProRepository _repository;

        public ProMetadataController(IProRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DcMetadata>>> GetAllMetadataItems()
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
        public async Task<ActionResult<DcMetadata>> GetSingleMetadataItem(int mdId)
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
        public async Task<ActionResult> CreateMetadataItem([FromBody] DcMetadata mdEntity)
        {
            if (mdEntity == null)
            {
                return BadRequest("Invalid data provided");
            }

            try
            {
                await _repository.CreateMetadataItem(mdEntity);
                return CreatedAtAction("GetSingleMetadataItem", new { mdId = mdEntity.ItemId }, mdEntity);
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
        public async Task<ActionResult> UpdateMetadataItem(int mdId, [FromBody] DcMetadata mdEntity)
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

        [HttpDelete("{mdId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteSingleMetadataItem(int mdId)
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

        [HttpDelete("all")]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteAllMdItems()
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
    }
}
