using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Core.DTOs.BusinessCardDto
{
    public class BusinessCardCsvXmlDto
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
    }
}
