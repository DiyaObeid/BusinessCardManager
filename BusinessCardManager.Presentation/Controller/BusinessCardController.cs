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

        


        // GET api/businesscards/filter

        [HttpGet("FilterBusinessCards")]
        public async Task<IActionResult> GetBusinessCardsByFilters(string term, string searchString)
        {
            var businessCards = await _businessCardService.SearchBusinessCards(term, searchString);
            return Ok(businessCards);
        }



        // Remove a business card
        [HttpDelete("RemoveBusinessCard/{Id}")]
        public async Task<IActionResult> RemoveBusinessCard(int Id)
        {
            var result = await _businessCardService.RemoveBusinessCardAsync(Id);
            return Ok(result);
        }

        // Export specific record to csv file 
        [HttpGet("export/csv/{id}")]
        public async Task<IActionResult> ExportToCsv(int id)
        {
            try
            {
                var fileResult = await _businessCardService.ExportToCsvAsync(id);
                return File(fileResult.FileContents, fileResult.ContentType, fileResult.FileDownloadName);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Business card not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting CSV: {ex.Message}");
            }
        }



    }
}

