using Microsoft.AspNetCore.Mvc;
using Moq;
using SEORanking.API.Controllers;
using SEORanking.Application.DTOs;
using SEORanking.Application.Interfaces;
using Xunit;

namespace SEORanking.WebAPI.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class SearchControllerTests
    {
        [Fact]
        public async Task Search_ReturnsOkResult_WithValidParameters()
        {
            // Arrange
            var mockSearchService = new Mock<ISearchService>();
            var searchDto = new SearchRequestDto { Keyword = "e-settlements", Url = "www.sympli.com.au" };
            var expectedResult = new SearchResultDto { Position = "1, 2, 3" };

            mockSearchService.Setup(service => service.SearchAsync(It.IsAny<SearchRequestDto>()))
                             .ReturnsAsync(expectedResult);

            var controller = new SearchController(mockSearchService.Object);

            // Act
            var result = await controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task Search_SetsDefaultValues_WhenParametersAreMissing()
        {
            // Arrange
            var mockSearchService = new Mock<ISearchService>();
            var defaultSearchDto = new SearchRequestDto { Keyword = null, Url = null };
            var expectedKeyword = "e-settlements";
            var expectedUrl = "www.sympli.com.au";

            var controller = new SearchController(mockSearchService.Object);

            // Act
            var result = await controller.Search(defaultSearchDto);

            // Assert
            mockSearchService.Verify(service => service.SearchAsync(
                It.Is<SearchRequestDto>(dto => dto.Keyword == expectedKeyword && dto.Url == expectedUrl)),
                Times.Once);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}