namespace SEORanking.Domain.Entities
{
    public class SearchResult
    {
        public required string Keyword { get; set; }
        public required string Url { get; set; }
        public List<int> Positions { get; set; } = new List<int>();
    }
}
