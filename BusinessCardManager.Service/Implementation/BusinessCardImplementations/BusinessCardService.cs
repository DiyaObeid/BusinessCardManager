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
using Microsoft.EntityFrameworkCore;
using CsvHelper.Configuration;

using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using Image = SixLabors.ImageSharp.Image; // This may be needed for specific image formats


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

        //private string EncodeImageToBase64(IFormFile imageFile)
        //{
        //    if (imageFile == null || imageFile.Length == 0)
        //    {
        //        return string.Empty; // Return an empty string if no file is provided
        //    }

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        imageFile.CopyTo(memoryStream); // Copy the file content to a memory stream
        //        var imageBytes = memoryStream.ToArray(); // Convert to byte array
        //        return Convert.ToBase64String(imageBytes); // Convert byte array to Base64 string
        //    }
        //}

        private string EncodeImageToBase64(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty; // Return an empty string if no file is provided
            }

            using (var memoryStream = new MemoryStream())
            {
                // Load the image from the file
                using (var image = Image.Load(imageFile.OpenReadStream()))
                {
                    // Resize the image to a smaller size (e.g., 300x300 pixels)
                    image.Mutate(x => x.Resize(300, 300)); // Adjust the dimensions as needed

                    // Save the resized image to the memory stream
                    image.SaveAsJpeg(memoryStream); // You can use SaveAsPng or other formats as needed
                }

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

        //private async Task<ResultDto> ImportFromCsvAsync(IFormFile file)
        //{
        //    try
        //    {
        //        using (var reader = new StreamReader(file.OpenReadStream()))
        //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //        {
        //            // Materialize the records into a list
        //            var records = csv.GetRecords<BusinessCard>().ToList();

        //            // Debugging: check if records were materialized correctly
        //            if (!records.Any())
        //            {
        //                return new ResultDto { Succeeded = false, Message = "No records found in the CSV file." };
        //            }

        //            foreach (var businessCard in records)
        //            {
        //                // Add each business card to the database
        //                var result = await _businessCardRepository.AddAsync(businessCard);

        //                // Check if adding the record was successful, else break the loop
        //                if (!result.Succeeded)
        //                {
        //                    return new ResultDto { Succeeded = false, Message = $"Failed to add business card: {result.Message}" };
        //                }
        //            }
        //        }
        //        return new ResultDto { Succeeded = true, Message = "Business cards imported successfully." };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultDto { Succeeded = false, Message = $"Error importing CSV: {ex.Message}" };
        //    }
        //}

        private async Task<ResultDto> ImportFromCsvAsync(IFormFile file)
        {
            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null, // Ignore header validation
                    MissingFieldFound = null // Ignore missing fields
                };

                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // Parse the records into BusinessCardCsvXmlDto
                    var records = csv.GetRecords<BusinessCardCsvXmlDto>().ToList();

                    // Check if records were materialized correctly
                    if (!records.Any())
                    {
                        return new ResultDto { Succeeded = false, Message = "No records found in the CSV file." };
                    }

                    foreach (var csvDto in records)
                    {
                        // Directly map BusinessCardCsvXmlDto to BusinessCard entity
                        var businessCard = _mapper.Map<BusinessCard>(csvDto);

                        // Add each business card to the database
                        var result = await _businessCardRepository.AddAsync(businessCard);

                        // Check if adding the record was successful
                        if (!result.Succeeded)
                        {
                            return new ResultDto { Succeeded = false, Message = $"Failed to add business card: {result.Message}" };
                        }
                    }
                }
                return new ResultDto { Succeeded = true, Message = "Business cards imported successfully." };
            }
            catch (Exception ex)
            {
                return new ResultDto { Succeeded = false, Message = $"Error importing CSV: {ex.Message}" };
            }
        }



        //// Method to import business cards from an XML file
        //private async Task<ResultDto> ImportFromXmlAsync(IFormFile file)
        //{
        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            stream.Position = 0;



        //            var serializer = new XmlSerializer(typeof(List<BusinessCardCsvXmlDto>));
        //            var csvXmlDtos = (List<BusinessCardCsvXmlDto>)serializer.Deserialize(stream);

        //            // Check if records were materialized correctly
        //            if (csvXmlDtos == null || !csvXmlDtos.Any())
        //            {
        //                return new ResultDto { Succeeded = false, Message = "No records found in the XML file." };
        //            }

        //            foreach (var csvDto in csvXmlDtos)
        //            {
        //                // Map BusinessCardCsvXmlDto to BusinessCard entity
        //                var businessCard = _mapper.Map<BusinessCardCsvXmlDto, BusinessCard>(csvDto);

        //                // Add each business card to the database
        //                var result = await _businessCardRepository.AddAsync(businessCard);

        //                // Check if adding the record was successful
        //                if (!result.Succeeded)
        //                {
        //                    return new ResultDto { Succeeded = false, Message = $"Failed to add business card: {result.Message}" };
        //                }
        //            }
        //        }

        //        return new ResultDto { Succeeded = true, Message = "Business cards imported successfully." };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultDto { Succeeded = false, Message = $"Error importing XML: {ex.Message}" };
        //    }
        //}
        // Method to import business cards from an XML file
        private async Task<ResultDto> ImportFromXmlAsync(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    // Use StreamReader with UTF-8 encoding
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        var serializer = new XmlSerializer(typeof(List<BusinessCardCsvXmlDto>));
                        var csvXmlDtos = (List<BusinessCardCsvXmlDto>)serializer.Deserialize(reader);

                        // Check if records were materialized correctly
                        if (csvXmlDtos == null || !csvXmlDtos.Any())
                        {
                            return new ResultDto { Succeeded = false, Message = "No records found in the XML file." };
                        }

                        foreach (var csvDto in csvXmlDtos)
                        {
                            // Map BusinessCardCsvXmlDto to BusinessCard entity
                            var businessCard = _mapper.Map<BusinessCardCsvXmlDto, BusinessCard>(csvDto);

                            // Add each business card to the database
                            var result = await _businessCardRepository.AddAsync(businessCard);

                            // Check if adding the record was successful
                            if (!result.Succeeded)
                            {
                                return new ResultDto { Succeeded = false, Message = $"Failed to add business card: {result.Message}" };
                            }
                        }
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

        // Method to Export all of data as csv file 
        public async Task<FileContentResult> ExportToCsvAsync()
        {
            // Step 1: Fetch all business card records from the database
            var businessCards = await _businessCardRepository.GetAllAsync();

            if (businessCards == null || !businessCards.Any())
            {
                // Return an empty file if no records exist
                return new FileContentResult(Encoding.UTF8.GetBytes(""), "text/csv")
                {
                    FileDownloadName = "BusinessCards.csv"
                };
            }

            // Step 2: Convert records to CSV format
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Name,Email,Phone,Gender,DateOfBirth,Address");

            foreach (var card in businessCards)
            {
                csvBuilder.AppendLine($"{card.Name},{card.Email},{card.Phone},{card.Gender},{card.DateOfBirth:yyyy-MM-dd},{card.Address}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Step 3: Return the CSV file as a downloadable response
            return new FileContentResult(csvBytes, "text/csv")
            {
                FileDownloadName = "BusinessCards.csv"
            };
        }

        // Method to Export a specific data record using id
        public async Task<FileContentResult> ExportToCsvByIdAsync(int id)
        {
            // Fetch the business card by ID
            var businessCard = await _businessCardRepository.GetByIdAsync(id);

            // Check if the business card exists
            if (businessCard == null)
            {
                // Handle the case when the record does not exist (e.g., throw an exception or return a specific result)
                throw new KeyNotFoundException("Business card not found.");
            }

            // Convert the found business card to CSV format
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Name,Email,Phone,Gender,DateOfBirth,Address");

            // Create a CSV line for the found business card
            csvBuilder.AppendLine($"{businessCard.Name},{businessCard.Email},{businessCard.Phone},{businessCard.Gender},{businessCard.DateOfBirth:yyyy-MM-dd},{businessCard.Address}");

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Return the CSV file as a downloadable response
            return new FileContentResult(csvBytes, "text/csv")
            {
                FileDownloadName = $"{businessCard.Name}_BusinessCard.csv"
            };
        }

        Task<ResultDto> IBusinessCardService.ExportToCsvByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}