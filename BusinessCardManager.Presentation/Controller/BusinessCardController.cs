using BusinessCardManager.Core.DTOs.BusinessCardDto;
using BusinessCardManager.Core.DTOs;
using BusinessCardManager.Service.Contract.IBusinessCardContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCardManager.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardController : ControllerBase
    {
        private readonly IBusinessCardService _businessCardService;
        public BusinessCardController(IBusinessCardService businessCardService)
        {
            _businessCardService = businessCardService;
        }
        [HttpPost("AddBusinessCard")]
        public async Task<IActionResult> AddBusinessCard(AddBusinessCardDto addBusinessCardDto)
        {
            var result = await _businessCardService.AddBusinessCardAsync(addBusinessCardDto);
            return Ok(result);
        }
        // Import business cards from file
        [HttpPost("ImportBusinessCards")]
        public async Task<IActionResult> ImportBusinessCards(IFormFile file, string fileType)
        {
            var result = await _businessCardService.ImportBusinessCardsAsync(file, fileType);
            return Ok(result);
        }

        // Get all business cards
        [HttpGet("GetAllBusinessCards")]
        public async Task<IActionResult> GetAllBusinessCards()
        {
            var businessCards = await _businessCardService.GetAllBusinessCardsAsync();
            return Ok(businessCards);
        }

        // Get business cards by filters
        [HttpGet("FilterBusinessCards")]
        public async Task<IActionResult> GetBusinessCardsByFilters(
            [FromQuery] string? name = null,
            [FromQuery] DateTime? dob = null,
            [FromQuery] string? phone = null,
            [FromQuery] string? gender = null,
            [FromQuery] string? email = null)
        {
            var filteredCards = await _businessCardService.GetBusinessCardsByFiltersAsync(name, dob, phone, gender, email);
            return Ok(filteredCards);
        }

        // Remove a business card
        [HttpDelete("RemoveBusinessCard")]
        public async Task<IActionResult> RemoveBusinessCard(RemoveBusinessCardDto removeBusinessCardDto)
        {
            var result = await _businessCardService.RemoveBusinessCardAsync(removeBusinessCardDto);
            return Ok(result);
        }

        // Export all of data for csv file
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            var result = await _businessCardService.ExportToCsvAsync();
            return result;
        }

        // Export specific record to csv file 
        //[HttpGet("export/csv/{id}")]
        //public async Task<IActionResult> ExportCsv(int id)
        //{
        //    try
        //    {
        //        var fileContentResult = await _businessCardService.ExportToCsvByIdAsync(id);
        //        return fileContentResult; // This returns the CSV file
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message); // Handle not found
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error exporting CSV: {ex.Message}"); // Handle other errors
        //    }
        //}



    }
}

