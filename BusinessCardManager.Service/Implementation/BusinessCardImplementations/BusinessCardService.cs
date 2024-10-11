/*
 * BusinessCardService.cs
 * 
 * This service class implements the IBusinessCardService interface to manage business card operations, 
 * including adding new business cards, importing from CSV and XML files, retrieving all business cards 
 * or filtered results, and deleting business cards. It leverages the IRepository interface for database 
 * operations and utilizes AutoMapper for DTO-to-entity mappings. The class also provides helper methods 
 * for encoding images to Base64 format and handling file uploads for business card imports.
 * 
 * Dependencies:
 * - IRepository<BusinessCard>: Interface for repository pattern to handle CRUD operations.
 * - IMapper: AutoMapper instance for DTO and entity mappings.
 * - CsvHelper: Library for handling CSV file parsing.
 * - System.Xml.Serialization: Library for handling XML file parsing.
 */
using BusinessCardManager.Core.DTOs;
using BusinessCardManager.Core.Entities;
using BusinessCardManager.Core.IRepositories;
using BusinessCardManager.Service.Contract.IBusinessCardContract;
using Microsoft.AspNetCore.Http;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using AutoMapper;
using BusinessCardManager.Core.DTOs.BusinessCardDto;

namespace BusinessCardManager.Service.Implementation.BusinessCardImplementations
{
    public class BusinessCardService : IBusinessCardService
    {
        private readonly IRepository<BusinessCard> _businessCardRepository; // Repository for business cards

        private readonly IMapper _mapper;

        // Constructor to initialize the service with the business card repository and Mapper 
        public BusinessCardService(IRepository<BusinessCard> businessCardRepository, IMapper mapper)
        {
            _businessCardRepository = businessCardRepository ?? throw new ArgumentNullException(nameof(businessCardRepository)); // Ensure repository is not null
            _mapper = mapper;
        }

        // Method to add a new business card
        public async Task<ResultDto> AddBusinessCardAsync(AddBusinessCardDto addBusinessCardDto)
        {
            var MappedBusinessCard = _mapper.Map<AddBusinessCardDto, BusinessCard>(addBusinessCardDto);

            if (addBusinessCardDto.PhotoFile != null) // Assuming PhotoFile is IFormFile in your DTO
            {
                MappedBusinessCard.Photo = EncodeImageToBase64(addBusinessCardDto.PhotoFile);
            }

            return await _businessCardRepository.AddAsync(MappedBusinessCard); // Delegate to repository 
        }

        private string EncodeImageToBase64(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty; // Return an empty string if no file is provided
            }

            using (var memoryStream = new MemoryStream())
            {
                imageFile.CopyTo(memoryStream); // Copy the file content to a memory stream
                var imageBytes = memoryStream.ToArray(); // Convert to byte array
                return Convert.ToBase64String(imageBytes); // Convert byte array to Base64 string
            }
        }




        // Method to handle file upload
        public async Task<ResultDto> ImportBusinessCardsAsync(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
                return new ResultDto { Succeeded = false, Message = "File is empty." };

            if (fileType == "csv")
            {
                return await ImportFromCsvAsync(file);
            }
            else if (fileType == "xml")
            {
                return await ImportFromXmlAsync(file);
            }

            return new ResultDto { Succeeded = false, Message = "Unsupported file type." };
        }

        // Method to import business cards from a CSV file
        private async Task<ResultDto> ImportFromCsvAsync(IFormFile file)
        {
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<BusinessCard>();
                    foreach (var businessCard in records)
                    {
                        await _businessCardRepository.AddAsync(businessCard);
                    }
                }
                return new ResultDto { Succeeded = true, Message = "Business cards imported successfully." };
            }
            catch (Exception ex)
            {
                return new ResultDto { Succeeded = false, Message = $"Error importing CSV: {ex.Message}" };
            }
        }

        // Method to import business cards from an XML file
        private async Task<ResultDto> ImportFromXmlAsync(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var serializer = new XmlSerializer(typeof(List<BusinessCard>));
                    var businessCards = (List<BusinessCard>)serializer.Deserialize(stream);

                    foreach (var businessCard in businessCards)
                    {
                        await _businessCardRepository.AddAsync(businessCard);
                    }
                }
                return new ResultDto { Succeeded = true, Message = "Business cards imported successfully." };
            }
            catch (Exception ex)
            {
                return new ResultDto { Succeeded = false, Message = $"Error importing XML: {ex.Message}" };
            }
        }


        // Method to get all business cards
        public async Task<IEnumerable<BusinessCard?>> GetAllBusinessCardsAsync()
        {
            return await _businessCardRepository.GetAllAsync(); // Delegate to repository
        }

        // Method to get business cards by filters
        /// <summary>
        /// Asynchronously retrieves business cards based on optional filter criteria.
        /// </summary>
        /// <param name="name">Optional filter by business card name.</param>
        /// <param name="dob">Optional filter by date of birth.</param>
        /// <param name="phone">Optional filter by phone number.</param>
        /// <param name="gender">Optional filter by gender.</param>
        /// <param name="email">Optional filter by email address.</param>
        /// <returns>A Task containing an IEnumerable of filtered BusinessCard objects.</returns>
        public async Task<IEnumerable<BusinessCard?>> GetBusinessCardsByFiltersAsync(
            string? name = null,
            DateTime? dob = null,
            string? phone = null,
            string? gender = null,
            string? email = null)
        {
            
            Expression<Func<BusinessCard, bool>> filterExpression = card =>
            (string.IsNullOrEmpty(name) || card.Name.Contains(name)) &&
            (!dob.HasValue || card.DateOfBirth.Date == dob.Value.Date) &&
            (string.IsNullOrEmpty(phone) || card.Phone.Contains(phone)) &&
            (string.IsNullOrEmpty(gender) || card.Gender.ToLower() == gender.ToLower()) &&
            (string.IsNullOrEmpty(email) || card.Email.Contains(email));



            // Get filtered business cards from the repository
            return await _businessCardRepository.GetByFiltersAsync(filterExpression);
        }


        // Method to remove a business card
        public async Task<ResultDto> RemoveBusinessCardAsync(RemoveBusinessCardDto removeBusinessCardDto)
        {
            var businessCard = await _businessCardRepository.GetByIdAsync(removeBusinessCardDto.Id);
            if (businessCard == null)
            {
                return new ResultDto { Succeeded = false, Message = "Business card not found." };
            }

            return await _businessCardRepository.RemoveAsync(businessCard);
        }
    }
}