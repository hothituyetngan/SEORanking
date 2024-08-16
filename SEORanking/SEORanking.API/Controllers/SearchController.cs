using Microsoft.AspNetCore.Mvc;
using SEORanking.Application.Constants;
using SEORanking.Application.DTOs;
using SEORanking.Application.Interfaces;

namespace SEORanking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Keyword))
            {
                request.Keyword = SearchConstants.DefaultKeyword;
            }

            if (string.IsNullOrEmpty(request.Url))
            {
                request.Url = SearchConstants.DefaultUrl;
            }

            var result = await _searchService.SearchAsync(request);
            return Ok(result);
        }
    }
}
