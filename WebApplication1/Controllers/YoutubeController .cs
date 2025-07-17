using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using WebApplication1.Services;
using YouTubeService = WebApplication1.Services.YouTubeService; 
using YouTubeServiceApi = Google.Apis.YouTube.v3.YouTubeService;


namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {

        private readonly IYoutubeService _youTubeService;

        public YoutubeController(IYoutubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }


        /*
        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, int maxResults)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            if (maxResults <= 0)
            {
                return BadRequest("maxResults cannot be empty or negative.");
            }

            var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,

                ApplicationName = this.GetType().ToString()
            });

            Console.WriteLine(this.GetType().ToString());

            string videoId = ExtractVideoId(query);
            if (!string.IsNullOrEmpty(videoId))
            {
                // Search by Video ID
                var videoRequest = youtubeService.Videos.List("snippet");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                if (videoResponse.Items.Count == 0)
                {
                    return NotFound($"No video found for ID: {videoId}");
                }

                var videoInfo = videoResponse.Items[0];
                var videoDetails = new
                {
                    Title = videoInfo.Snippet.Title,
                    ID = videoId,
                    Url = $"https://www.youtube.com/watch?v={videoId}",
                    Thumbnail = videoInfo.Snippet.Thumbnails.Default__.Url
                };

                return Ok(new { videoDetails });
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

                return Ok(new { videos });
            }
        }


        private string ExtractVideoId(string query)
        {
            var regex = new Regex(@"(?:https?:\/\/)?(?:www\.)?youtube\.com\/watch\?v=([^&]+)|youtu\.be\/([^?]+)");
            var match = regex.Match(query);
            return match.Success ? (match.Groups[1].Value ?? match.Groups[2].Value) : string.Empty;
        }*/



        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, int maxResults)
        {
            var result = await _youTubeService.SearchAsync(query, maxResults);

            if (result == null)
                return NotFound("No video(s) found for the given input.");

            return Ok(result);
        }


        [HttpGet("transcript")]
        public async Task<IActionResult> GetTranscript(string videoId, float start_time, float end_time)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return BadRequest("videoId is required.");

            var transcriptJson = await _youTubeService.GetTranscriptAsync(videoId, start_time, end_time);

            if (transcriptJson == null)
                return NotFound("Transcript not found or an error occurred.");

            return Content(transcriptJson, "application/json");
        }

    }
}



/*
[HttpGet("transcript/{videoId}")]
public async Task<IActionResult> GetTranscript(string videoId)
{
    var transcript = await _youtubeService.GetTranscriptAsync(videoId);
    return Ok(transcript);
}*/

/*

        [HttpGet("transcript-sync/{videoId}")]
        public async Task<IActionResult> GetTranscript(string videoId)
        {
            var transcriptTask = _youTubeService.GetTranscript(videoId);
            var transcript = await transcriptTask; // Await the Task to get the string result
            if (string.IsNullOrEmpty(transcript))
                return NotFound();
            return Ok(transcript);
        }
*/

/*
private readonly YouTubeService _transcriptFetcher;

public YoutubeController()
{
    string pythonScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","scripts", "transcript.py");

    _transcriptFetcher = new YouTubeService(pythonScriptPath);
}/*
/*
[HttpGet("GetTranscript")]
public IActionResult GetTranscript(string videoID)
{
    string transcript = _transcriptFetcher.GetTranscript(videoID);

    if (string.IsNullOrEmpty(transcript))
    {
        return NotFound(new { error = "cannot find a transcript for the given video" });
    }

    return Ok(transcript);


}*/



/*[HttpGet("GetBestMatchedCaption")]
public IActionResult GetBestMatchedCaption(string videoID, string prompt)
{
    string bestMatchedCaption = _transcriptFetcher.GetBestMatchedCaption(videoID, prompt);
    if (string.IsNullOrEmpty(bestMatchedCaption))
    {
        return NotFound(new { error = "cannot find a caption for the given video" });
    }

    return Ok(bestMatchedCaption);
}*/