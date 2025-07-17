namespace WebApplication1.Services
{
    public interface IYoutubeService
    {
        //public string GetTranscript(string videoId);
        //public string GetBestMatchedCaption(string videoId, string prompt);
        //Task<string> GetTranscriptAsync(string videoId);
        Task<string?> GetTranscriptAsync(string videoId, float start_time, float end_time);
        Task<object> SearchAsync(string query, int maxResults);


    }
}