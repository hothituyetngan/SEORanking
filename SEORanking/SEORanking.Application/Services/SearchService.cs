using SEORanking.Application.DTOs;
using SEORanking.Application.Interfaces;
using SEORanking.Domain.Interfaces;
using System.Diagnostics.Metrics;
using System.Text;

namespace SEORanking.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IEnumerable<ISearchEngine> _searchEngines;
        private readonly ICacheService _cacheService;

        public SearchService(IEnumerable<ISearchEngine> searchEngines, ICacheService cacheService)
        {
            _searchEngines = searchEngines;
            _cacheService = cacheService;
        }

        public async Task<SearchResultDto> SearchAsync(SearchRequestDto request)
        {
            var cacheKey = $"{request.Keyword}:{request.Url}";
            if (_cacheService.TryGet(cacheKey, out SearchResultDto cachedResult))
            {
                return cachedResult;
            }

            var position = new StringBuilder();
            int count = 0;
            foreach (var engine in _searchEngines)
            {
                count++;
                var enginePositions = await engine.GetUrlPositionsAsync(request.Keyword, request.Url);
                position.Append(enginePositions);
                if (count < _searchEngines.Count())
                    position.Append(". ");
            }

            var result = new SearchResultDto { Position = position.ToString() };
            _cacheService.Set(cacheKey, result, TimeSpan.FromHours(1));
            return result;
        }
    }
}
