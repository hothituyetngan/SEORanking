using Moq;
using SEORanking.Application.DTOs;
using SEORanking.Application.Interfaces;
using SEORanking.Application.Services;
using SEORanking.Domain.Interfaces;
using Xunit;

namespace SEORanking.Application.Tests.Services
{
    public class SearchServiceTests
    {
        [Fact]
        public async Task SearchAsync_ReturnsCachedResult_IfCacheExists()
        {
            // Arrange
            var mockSearchEngines = new Mock<IEnumerable<ISearchEngine>>();
            var mockCacheService = new Mock<ICacheService>();
            var cacheKey = "e-settlements:www.sympli.com.au";
            var cachedResult = new SearchResultDto { Position = "Google: 3, 33. Bing: 1" };

            mockCacheService.Setup(x => x.TryGet(cacheKey, out cachedResult)).Returns(true);

            var service = new SearchService(mockSearchEngines.Object, mockCacheService.Object);

            // Act
            var result = await service.SearchAsync(new SearchRequestDto { Keyword = "e-settlements", Url = "www.sympli.com.au" });

            // Assert
            Assert.Equal("Google: 3, 33. Bing: 1", result.Position);
            mockSearchEngines.Verify(x => x.GetEnumerator(), Times.Never());
        }

        [Fact]
        public async Task SearchAsync_CallsSearchEngines_IfNoCache()
        {
            // Arrange
            var searchEngines = new List<ISearchEngine>();
            var mockSearchEngine = new Mock<ISearchEngine>();
            searchEngines.Add(mockSearchEngine.Object);
            var mockCacheService = new Mock<ICacheService>();
            var cacheKey = "e-settlements:www.sympli.com.au";
            SearchResultDto cachedResult = null;  // Ensure it's set to null if not used

            // Setup mock to handle out parameters correctly (if used)
            mockCacheService.Setup(x => x.TryGet(cacheKey, out cachedResult)).Returns(false);
            mockSearchEngine.Setup(x => x.GetUrlPositionsAsync("e-settlements", "www.sympli.com.au"))
                            .ReturnsAsync("Google: 3, 33. Bing: 1");

            var service = new SearchService(searchEngines, mockCacheService.Object);

            // Act
            var result = await service.SearchAsync(new SearchRequestDto { Keyword = "e-settlements", Url = "www.sympli.com.au" });

            // Assert
            Assert.Equal("Google: 3, 33. Bing: 1", result.Position);
            mockCacheService.Verify(x => x.Set(cacheKey, It.IsAny<SearchResultDto>(), TimeSpan.FromHours(1)), Times.Once);
        }
    }
}
