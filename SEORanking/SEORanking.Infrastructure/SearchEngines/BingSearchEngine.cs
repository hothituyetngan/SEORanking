using SEORanking.Application.Constants;
using SEORanking.Domain.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace SEORanking.Infrastructure.SearchEngines
{
    public class BingSearchEngine : ISearchEngine
    {
        public async Task<string> GetUrlPositionsAsync(string keyword, string url)
        {
            var result = new StringBuilder("Bing: ");
            var positions = new List<int>();
            int resultsToFetch = 100;
            int resultsPerPage = 10;
            int pagesToFetch = (int)Math.Ceiling((double)resultsToFetch / resultsPerPage);

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("User-Agent", HttpClientConstant.UserAgent);

            for (int page = 0; page < pagesToFetch; page++)
            {
                await Task.Delay(300);

                var offset = page * resultsPerPage;
                var searchUrl = $"https://www.bing.com/search?q={keyword}&first={offset + 1}";

                var response = await httpClient.GetStringAsync(searchUrl);

                var regex = new Regex("<cite>(.*?)<\\/cite>", RegexOptions.Singleline);
                var matches = regex.Matches(response);

                for (int i = 0; i < matches.Count && positions.Count < resultsToFetch; i++)
                {
                    if (matches[i].Groups[1].Value.Contains(url, StringComparison.OrdinalIgnoreCase))
                    {
                        positions.Add(offset + i + 1);
                    }
                }
            }

            return result.Append(string.Join(", ", positions)).ToString();
        }
    }
}
