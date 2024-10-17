using BusinessCardManager.Core.DTOs;
using BusinessCardManager.Core.DTOs.BusinessCardDto;
using BusinessCardManager.Presentation.Controller;
using BusinessCardManager.Service.Contract.IBusinessCardContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BusinessCardManager.Tests
{
    public class BusinessCardControllerTests
    {
        private readonly Mock<IBusinessCardService> _mockBusinessCardService;
        private readonly BusinessCardController _controller;

        public BusinessCardControllerTests()
        {
            _mockBusinessCardService = new Mock<IBusinessCardService>();
            _controller = new BusinessCardController(_mockBusinessCardService.Object);
        }

        [Fact]
        public async Task AddBusinessCard_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var dto = new AddBusinessCardDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "123456789",
                Gender = "Male",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Address = "123 Main St",
                PhotoFile = null
            };
            _mockBusinessCardService.Setup(service => service.AddBusinessCardAsync(dto))
                .ReturnsAsync(new ResultDto { Succeeded = true, Message = "Business card added successfully." });

            // Act
            var result = await _controller.AddBusinessCard(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<ResultDto>(okResult.Value);
            Assert.True(resultDto.Succeeded);
            Assert.Equal("Business card added successfully.", resultDto.Message);
        }

        [Fact]
        public async Task ImportBusinessCards_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var fileName = "businesscards.csv";
            var fileContent = "Name,Email,Phone,Gender,DateOfBirth,Address\nJohn Doe,john@example.com,123456789,Male,1993-01-01,123 Main St";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(stream.Length);

            _mockBusinessCardService.Setup(service => service.ImportBusinessCardsAsync(fileMock.Object, "csv"))
                .ReturnsAsync(new List<BusinessCardCsvXmlDto>());

            // Act
            var result = await _controller.ImportBusinessCards(fileMock.Object, "csv");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAllBusinessCards_ReturnsOkResult_WithListOfBusinessCards()
        {
            // Arrange
            var businessCards = new List<BusinessCardCsvXmlDto>
            {
                new BusinessCardCsvXmlDto { Name = "John Doe", Email = "john@example.com", Phone = "123456789", Gender = "Male", DateOfBirth = DateTime.Now.AddYears(-30), Address = "123 Main St" }
            };
            _mockBusinessCardService.Setup(service => service.GetAllBusinessCardsAsync())
                .ReturnsAsync(businessCards);

            // Act
            var result = await _controller.GetAllBusinessCards();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultList = Assert.IsAssignableFrom<IEnumerable<BusinessCardCsvXmlDto>>(okResult.Value);
            Assert.Single(resultList);
        }

        [Fact]
public async Task GetBusinessCardsByFilters_ReturnsOkResult_WhenSuccessful()
{
    // Arrange
    var term = "name";
    var searchString = "Doe";
    var businessCards = new List<BusinessCardCsvXmlDto>
    {
        new BusinessCardCsvXmlDto 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789", 
            Gender = "Male", 
            DateOfBirth = DateTime.Now.AddYears(-30), 
            Address = "123 Main St" 
        }
    }.AsEnumerable();

    // Mock the service call with correct return type
    _mockBusinessCardService.Setup(service => service.SearchBusinessCards(term, searchString))
        .ReturnsAsync(businessCards);

    // Act
    var result = await _controller.GetBusinessCardsByFilters(term, searchString);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var resultList = Assert.IsAssignableFrom<IEnumerable<BusinessCardCsvXmlDto>>(okResult.Value);
    Assert.Single(resultList);
}


        [Fact]
        public async Task RemoveBusinessCard_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            _mockBusinessCardService.Setup(service => service.RemoveBusinessCardAsync(id))
                .ReturnsAsync(new ResultDto { Succeeded = true, Message = "Business card removed successfully." });

            // Act
            var result = await _controller.RemoveBusinessCard(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultDto = Assert.IsType<ResultDto>(okResult.Value);
            Assert.True(resultDto.Succeeded);
            Assert.Equal("Business card removed successfully.", resultDto.Message);
        }

        [Fact]
        public async Task ExportToCsv_ReturnsFileContentResult_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            var csvContent = "Name,Email,Phone,Gender,DateOfBirth,Address\nJohn Doe,john@example.com,123456789,Male,1993-01-01,123 Main St";
            var fileContentResult = new FileContentResult(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv")
            {
                FileDownloadName = "BusinessCards.csv"
            };
            _mockBusinessCardService.Setup(service => service.ExportToCsvAsync(id))
                .ReturnsAsync(fileContentResult);

            // Act
            var result = await _controller.ExportToCsv(id);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("BusinessCards.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ExportToCsv_ReturnsNotFound_WhenBusinessCardNotFound()
        {
            // Arrange
            int id = 99; // Assuming this ID does not exist
            _mockBusinessCardService.Setup(service => service.ExportToCsvAsync(id))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.ExportToCsv(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Business card not found.", notFoundResult.Value);
        }
    }
}
