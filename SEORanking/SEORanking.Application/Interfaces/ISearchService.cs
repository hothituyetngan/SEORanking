using SEORanking.Application.DTOs;

namespace SEORanking.Application.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(SearchRequestDto request);
    }
}
