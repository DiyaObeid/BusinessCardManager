// The BusinessCard entity represents a single business card within the Business Card Manager application.
// It captures essential information such as Name, Gender, Date of Birth, Email, Phone, and Address.
// Additionally, it includes an optional Photo field to store a base64 encoded image.
// This entity serves as the foundational model for managing business card data across the application.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardManager.Core.Entities
{
    public class BusinessCard
    {
        public int Id { get; set; } // Primary Key for the BusinessCard entity

        public string Name { get; set; } // Name of the individual on the business card

        public string Gender { get; set; } // Gender of the individual (e.g., Male, Female, Other)

        public DateTime DateOfBirth { get; set; } // Date of birth of the individual

        public string Email { get; set; } // Email address of the individual

        public string Phone { get; set; } // Phone number of the individual

        public string Address { get; set; } // Physical address of the individual

        public string? Photo { get; set; } // Optional Photo (Base64 encoded string representation of the image)
    }

}

