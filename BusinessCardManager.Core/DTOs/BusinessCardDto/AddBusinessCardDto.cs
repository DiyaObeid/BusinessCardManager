// AddBusinessCardDto.cs defines a data transfer object (DTO) 
// that encapsulates the information required to create a new 
// business card in the Business Card Manager application. 
// It includes properties for personal details and an optional 
// photo file, ensuring a complete representation of a business card.

using Microsoft.AspNetCore.Http;
using System;

namespace BusinessCardManager.Core.DTOs.BusinessCardDto
{
    public class AddBusinessCardDto
    {
        // Name of the individual or entity associated with the business card
        public string Name { get; set; }

        // Email address associated with the business card
        public string Email { get; set; }

        // Phone number associated with the business card
        public string Phone { get; set; }

        // Gender of the individual or entity associated with the business card
        public string Gender { get; set; }

        // Date of birth of the individual associated with the business card
        public DateTime DateOfBirth { get; set; }

        // Address associated with the business card
        public string Address { get; set; }

        // Optional property for uploading a photo file of the business card
        public IFormFile? PhotoFile { get; set; }
    }
}
