using SEORanking.Application.Constants;
using SEORanking.Domain.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace SEORanking.Infrastructure.SearchEngines
{
    public class GoogleSearchEngine : ISearchEngine
    {
        private const int NumberOfSearchItems = 100;
        private const string GoogleCiteTagPattern = @"<div\s+class=""byrV5b""><cite\s+class="".*?""\s+role=""text""\>(.*?)<\/cite>";

        public async Task<string> GetUrlPositionsAsync(string keyword, string url)
        {
            var result = new StringBuilder("Google: ");
            var positions = new List<int>();

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", HttpClientConstant.UserAgent);

            var searchUrl = $"https://www.google.com/search?q={keyword}&num={NumberOfSearchItems}";
            var response = await httpClient.GetAsync(searchUrl);

            if (response.IsSuccessStatusCode)
            {
                var htmlContent = await response.Content.ReadAsStringAsync();
                var extractedUrls = ParseGoogleSearchResults(htmlContent);

                extractedUrls = RemoveConsecutiveDuplicates(extractedUrls);
                int currentPosition = 0;
                foreach (var extractedUrl in extractedUrls)
                {
                    currentPosition++;
                    if (extractedUrl?.Contains(url) ?? false)
                    {
                        positions.Add(currentPosition);
                    }
                }
            }

            return result.Append(string.Join(", ", positions)).ToString();
        }

        private List<string> ParseGoogleSearchResults(string htmlContent)
        {
            MatchCollection matches = Regex.Matches(htmlContent, GoogleCiteTagPattern, RegexOptions.IgnoreCase);
            var extractedUrls = new List<string>();

            foreach (Match match in matches)
            {
                string result = match.Groups[1].Value;
                var extractedUrl = ExtractUrl(result);
                if (extractedUrl != null)
                {
                    extractedUrls.Add(extractedUrl);
                }
            }

            return extractedUrls;
        }

        private List<string> RemoveConsecutiveDuplicates(List<string> list)
        {
            var distinctList = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                distinctList.Add(list[i]);

                while (i < list.Count - 1 && list[i] == list[i + 1])
                {
                    i++;
                }
            }

            return distinctList;
        }

        private string? ExtractUrl(string input)
        {
            const string UrlPattern = @"https?://[^\s/$.?#].[^\s]*";
            var match = Regex.Match(input, UrlPattern, RegexOptions.IgnoreCase);

            return match.Success ? match.Value : null;
        }
    }
}
