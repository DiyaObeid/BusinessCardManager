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
using Image = SixLabors.ImageSharp.Image;
using BusinessCardManager.Infrastructure.Extensions; // This may be needed for specific image formats


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


        public async Task<List<BusinessCardCsvXmlDto>> ImportBusinessCardsAsync(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file provided or file is empty.");

            if (fileType.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                return await ImportFromCsvAsync(file);
            }
            else if (fileType.Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                return await ImportFromXmlAsync(file);
            }
            else
            {
                throw new Exception("Unsupported file type. Please upload a CSV or XML file.");
            }
        }

        private async Task<List<BusinessCardCsvXmlDto>> ImportFromCsvAsync(IFormFile file)
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
                        throw new Exception("No records found in the CSV file.");
                    }

                    return records;
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow or handle the exception as needed
                throw new Exception($"Error importing CSV: {ex.Message}", ex);
            }
        }

        // Method to import business cards from an XML file
        private async Task<List<BusinessCardCsvXmlDto>> ImportFromXmlAsync(IFormFile file)
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
                            throw new Exception("No records found in the XML file.");
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
                                throw new Exception($"Failed to add business card: {result.Message}");
                            }
                        }

                        // Return the list of imported records
                        return csvXmlDtos;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing XML: {ex.Message}", ex);
            }
        }

        // Method to get all business cards
        public async Task<IEnumerable<BusinessCardCsvXmlDto?>> GetAllBusinessCardsAsync()
        {
            var businessCards = await _businessCardRepository.GetAllAsync(); // Delegate to repository
            return _mapper.Map<IEnumerable<BusinessCardCsvXmlDto>>(businessCards);
        }


        //public async Task<List<BusinessCardCsvXmlDto>> SearchBusinessCards(string term, string searchString)
        //{

        //    return (await _businessCardRepository.SearchAsync(term, searchString)).ToList();
        //    // Map BusinessCard entities to BusinessCardCsvXmlDto
        //    return _mapper.Map<IEnumerable<BusinessCardCsvXmlDto>>(BusinessCard);
        //}

        public async Task<IEnumerable<BusinessCardCsvXmlDto>> SearchBusinessCards(string term, string searchString)
        {
            // Retrieve BusinessCard entities based on search
            var businessCards = await _businessCardRepository.SearchAsync(term, searchString);

            // Map the collection of BusinessCard entities to a collection of BusinessCardCsvXmlDto
            return _mapper.Map<IEnumerable<BusinessCardCsvXmlDto>>(businessCards);
        } 


        // Method to remove a business card
        public async Task<ResultDto> RemoveBusinessCardAsync(int Id)
        {
            var businessCard = await _businessCardRepository.GetByIdAsync(Id);
            if (businessCard == null)
            {
                return new ResultDto { Succeeded = false, Message = "Business card not found." };
            }

            return await _businessCardRepository.RemoveAsync(businessCard);
        }

        //  Method to Export a specific data record using id
        public async Task<FileContentResult> ExportToCsvAsync(int id)
        {
            // Step 1: Fetch all business card records from the database
            var businessCard = await _businessCardRepository.GetByIdAsync(id);

            if (businessCard == null)
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

            
                csvBuilder.AppendLine($"{businessCard.Name},{businessCard.Email},{businessCard.Phone},{businessCard.Gender},{businessCard.DateOfBirth:yyyy-MM-dd},{businessCard.Address}");
            

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Step 3: Return the CSV file as a downloadable response
            return new FileContentResult(csvBytes, "text/csv")
            {
                FileDownloadName = "BusinessCards.csv"
            };
        }

       
    }
}