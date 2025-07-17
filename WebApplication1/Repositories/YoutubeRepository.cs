using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace WebApplication1.Repositories
{
    public class YoutubeRepository : IYoutubeRepository
    {
        private readonly HttpClient _httpClient;
        private string _flaskApiBaseUrl;
        private static readonly string get_transcriptpath = @"C:\Users\Farah\Downloads\download.txt"; // change this to your  path
        private readonly string ApiKey = "AIzaSyDrFEXYjrl42K56GXLMFwmn9HPMWOGLjNc"; // Your API Key



        public YoutubeRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private static readonly Regex UrlRegex = new Regex(
            @"https:\/\/[a-zA-Z0-9\-]+\.ngrok\-free\.app",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
         );


        private  string? GetNgrokBaseUrl(string FilePath)
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                var content = File.ReadAllText(FilePath);

                // Find the ngrok base URL
                var match = UrlRegex.Match(content);
                if (match.Success)
                    return match.Value;

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading ngrok URL: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetTranscriptAsync(string videoId, float start_time, float end_time)
        {
            _flaskApiBaseUrl = GetNgrokBaseUrl(get_transcriptpath);
            if (_flaskApiBaseUrl != null)
            {
                var url = $"{_flaskApiBaseUrl}/GetTranscript?video_id={videoId}&start_time={start_time}&end_time={end_time}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

        public async Task<object?> SearchAsync(string query, int maxResults)
        {
            if (string.IsNullOrEmpty(query) || maxResults <= 0)
                return null;

            string apiKey = ApiKey; // Replace with your actual API key

            var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = this.GetType().ToString()
            });

            string videoId = ExtractVideoId(query);
            if (!string.IsNullOrEmpty(videoId))
            {
                // Search by Video ID
                var videoRequest = youtubeService.Videos.List("snippet");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                if (videoResponse.Items.Count == 0)
                {
                    return null;
                }

                var videoInfo = videoResponse.Items[0];
                return new
                {
                    videoDetails = new
                    {
                        Title = videoInfo.Snippet.Title,
                        ID = videoId,
                        Url = $"https://www.youtube.com/watch?v={videoId}",
                        Thumbnail = videoInfo.Snippet.Thumbnails.Default__.Url
                    }
                };
            }
            else
            {
                // Search by Topic
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = query;
                searchListRequest.MaxResults = maxResults;

                var searchListResponse = await searchListRequest.ExecuteAsync();
                List<object> videos = new List<object>();

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        var videoInfo = new
                        {
                            Title = searchResult.Snippet.Title,
                            ID = searchResult.Id.VideoId,
                            Url = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}",
                            Thumbnail = searchResult.Snippet.Thumbnails.Default__.Url
                        };
                        videos.Add(videoInfo);
                    }
                }

                return new { videos };
            }
        }

        // Keep this helper private
        private string ExtractVideoId(string query)
        {
            var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?youtube\.com\/watch\?v=([^&]+)|youtu\.be\/([^?]+)");
            var match = regex.Match(query);
            return match.Success ? (match.Groups[1].Value ?? match.Groups[2].Value) : string.Empty;
        }


        /*
        public async Task<string?> GetTranscriptAsync(string videoId)
        {
            _flaskApiBaseUrl = GetNgrokBaseUrl(get_transcriptpath);

            // print the url at the console
            if (_flaskApiBaseUrl != null)
            {

                var url = $"{_flaskApiBaseUrl}/GetTranscript?video_id={videoId}";
                Console.WriteLine($"Requesting transcript from: {url}");


                var response = await _httpClient.GetAsync($"{_flaskApiBaseUrl}/GetTranscript?video_id={videoId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return json;

            }
            return null;
        }
        */
    }
}



/*
namespace WebApplication1.Repositories
{
    public class YoutubeRepository:IYoutubeRepository
    {

        private readonly HttpClient _httpClient;

        public YoutubeRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        
        public async Task<string> GetTranscript(string videoId)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5000/GetTranscript?video_id={videoId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
        public async Task<string> GetTranscriptAsync(string videoId)
        {
            
            // Example: Replace with your actual logic to call the Python API or YouTube API
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:5001/api/youtube/transcript/{videoId}");
            
            var response = await new HttpClient().GetAsync("nnbgvgcgchgvh" + videoId);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();

        }
        */
/*
public string GetTranscript(string videoId)
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "D://home//python354x64//python.exe",
                Arguments = $"scripts/transcript.py {videoId}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","wwwroot")
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(output))
        {
            Console.WriteLine($"Error: {error}");
            return null;
        }

        return output;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return null;
    }
}

public string GetBestMatchedCaption(string videoID, string prompt)
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{_pythonScriptPath}\" \"{videoID}\" \"{prompt}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(output))
        {
            Console.WriteLine($"Error: {error}");
            return null;
        }

        return output;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return null;
    }
}*/
/*
    }
}*/
