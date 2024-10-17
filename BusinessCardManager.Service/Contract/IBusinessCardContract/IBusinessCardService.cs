/*
 * IBusinessCardService.cs
 * 
 * This interface defines the contract for the business card service, outlining core methods for 
 * managing business card operations. It includes methods for adding, retrieving, and removing 
 * business cards, as well as importing business card data from CSV or XML files. These methods 
 * provide an abstraction layer for the business card management functionality, promoting a 
 * decoupled architecture that adheres to the Dependency Inversion Principle.
 * 
 * Methods:
 * - AddBusinessCardAsync: Adds a new business card.
 * - GetAllBusinessCardsAsync: Retrieves all business cards.
 * - GetBusinessCardsByFiltersAsync: Retrieves business cards based on specified filter criteria.
 * - RemoveBusinessCardAsync: Removes a specified business card.
 * - ImportBusinessCardsAsync: Imports business cards from a file, supporting CSV and XML formats.
 */


using BusinessCardManager.Core.DTOs;
using BusinessCardManager.Core.DTOs.BusinessCardDto;
using BusinessCardManager.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace BusinessCardManager.Service.Contract.IBusinessCardContract
{
    public interface IBusinessCardService
    {
        /// <summary>
        /// Asynchronously adds a new business card.
        /// </summary>
        /// <param name="addBusinessCardDto">DTO containing business card data to be added.</param>
        /// <returns>A Task containing a ResultDto indicating success or failure of the operation.</returns>
        Task<ResultDto> AddBusinessCardAsync(AddBusinessCardDto addBusinessCardDto);

        /// <summary>
        /// Asynchronously retrieves all business cards.
        /// </summary>
        /// <returns>A Task containing an IEnumerable of BusinessCard objects.</returns>
        Task<IEnumerable<BusinessCardCsvXmlDto?>> GetAllBusinessCardsAsync();


        
        //Task<List<BusinessCard>> SearchBusinessCards(string term, string searchString);


        // Method to search business cards and return a list of BusinessCardCsvXmlDto
        Task<IEnumerable<BusinessCardCsvXmlDto>> SearchBusinessCards(string term, string searchString);



        /// <summary>
        /// Asynchronously removes a specified business card.
        /// </summary>
        /// <param name="businessCard">Business card entity to be removed.</param>
        /// <returns>A Task containing a ResultDto indicating success or failure of the operation.</returns>
        Task<ResultDto> RemoveBusinessCardAsync(int id);

        /// <summary>
        /// Asynchronously imports business cards from a file (supports CSV and XML formats).
        /// </summary>
        /// <param name="file">The file containing business card data to import.</param>
        /// <param name="fileType">The type of file to be imported (e.g., "csv" or "xml").</param>
        /// <returns>A Task containing a ResultDto indicating success or failure of the import process.</returns>
        //Task<ResultDto> ImportBusinessCardsAsync(IFormFile file, string fileType);
        Task<List<BusinessCardCsvXmlDto>> ImportBusinessCardsAsync(IFormFile file, string fileType);

        //Exports a business card to CSV format by its ID.
        Task<FileContentResult> ExportToCsvAsync(int id);

        
       
    }
}

