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

    }
    }

