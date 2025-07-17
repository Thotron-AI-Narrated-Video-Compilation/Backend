namespace WebApplication1.Repositories
{
    public interface IYoutubeRepository
    {
        Task<string?> GetTranscriptAsync(string videoId, float start_time, float end_time);

        Task<object?> SearchAsync(string query, int maxResults);

    }
}
