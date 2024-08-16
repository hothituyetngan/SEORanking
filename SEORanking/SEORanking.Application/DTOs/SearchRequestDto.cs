using SEORanking.Application.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SEORanking.Application.DTOs
{
    public class SearchRequestDto
    {
        [DefaultValue(SearchConstants.DefaultKeyword)]
        [Required]
        public string Keyword { get; set; }

        [DefaultValue(SearchConstants.DefaultUrl)]
        [Required]
        public string Url { get; set; }
    }
}
