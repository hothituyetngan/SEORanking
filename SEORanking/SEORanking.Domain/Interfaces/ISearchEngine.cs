namespace SEORanking.Domain.Interfaces
{
    public interface ISearchEngine
    {
        Task<string> GetUrlPositionsAsync(string keyword, string url);
    }
}
