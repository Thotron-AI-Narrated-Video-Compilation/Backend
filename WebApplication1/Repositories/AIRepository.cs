using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Repositories
{
    public class AIRepository : IAIRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IProjectRepository _projectRepository;
        private static readonly string autoclipping = @"C:\Users\Farah\Downloads\download.txt"; // change this to your  path
        private static readonly string SummaryPath = @"C:\Users\Farah\Downloads\summary_url.txt"; // change this to your  path
        private static readonly string avatarpath = @"C:\Users\Farah\Downloads\autoclipping.txt"; // change this to your  path



        public AIRepository(HttpClient httpClient, IProjectRepository projectRepository )
        {
            _httpClient = httpClient;
            _projectRepository = projectRepository;
            _httpClient.BaseAddress = new Uri("https://youtubesearch-ai-dvajd7c6e6cbeafm.canadacentral-01.azurewebsites.net/");
        }


        public async Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/generate-image", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error calling API: {response.StatusCode}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ImageGenerationResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TopicExtractionResponse> ExtractTopicsFromScriptAsync(string script)
        {
            var jsonContent = JsonSerializer.Serialize(new { script });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("api/extract-topics", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error extracting topics: {response.StatusCode}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TopicExtractionResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TopicExtractionResponse> ExtractTopicsFromPdfAsync(IFormFile pdfFile)
        {
            using var content = new MultipartFormDataContent();
            using var stream = pdfFile.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "file", pdfFile.FileName);

            HttpResponseMessage response = await _httpClient.PostAsync("api/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error uploading PDF: {response.StatusCode}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TopicExtractionResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        public async Task<string> GenerateSummary(string transcript)
        {
            var ngrokUrl = GetNgrokBaseUrl(SummaryPath);
            if (string.IsNullOrWhiteSpace(ngrokUrl))
                throw new Exception("Ngrok URL not found or invalid.");

            var apiUrl = ngrokUrl.TrimEnd('/') + "/generate-summary-pdf";

            var payload = new { transcript = transcript }; ;
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to call Python API: {response.StatusCode} - {errorBody}");
            }

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> GenerateAvatarWithSummary(string _Summary, string voiceName, string characterPicUrl)
        {
            /*
            // Step 1: Call the existing summary function
            var summaryJson = await GenerateSummary(transcript);

            // Step 2: Parse the summary text from the JSON response
            using var summaryDoc = JsonDocument.Parse(summaryJson);
            if (!summaryDoc.RootElement.TryGetProperty("summary", out var summaryElement))
                throw new Exception("Summary field not found in response.");

            */



            // Step 1: Call avatar API with summary text
            /*
            var avatarNgrokUrl = GetNgrokBaseUrl(avatarpath);
            if (string.IsNullOrWhiteSpace(avatarNgrokUrl))
                throw new Exception("Ngrok URL not found or invalid for avatar service.");

            var avatarApiUrl = avatarNgrokUrl.TrimEnd('/') + "/avatar";
            */

            var avatarApiUrl = "http://127.0.0.1:5000/avatar";// Replace with your actual avatar API URL
            var avatarPayload = new
            {
                pic= characterPicUrl,
                voice_name = voiceName,
                summary= _Summary
            };

            var avatarJson = JsonSerializer.Serialize(avatarPayload);
            var avatarContent = new StringContent(avatarJson, Encoding.UTF8, "application/json");

            var avatarResponse = await _httpClient.PostAsync(avatarApiUrl, avatarContent);

            if (!avatarResponse.IsSuccessStatusCode)
            {
                var errorBody = await avatarResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to call avatar API: {avatarResponse.StatusCode} - {errorBody}");
            }

            // Step 4: Return the avatar response (string or URL)
            return await avatarResponse.Content.ReadAsStringAsync();
            


        }





        /*
        public async Task<string> GetAvatarAsync(string VoiceName, string charecterpic)
        {

            // Get ngrok URL
            var ngrokUrl = GetNgrokBaseUrl(avatarpath);
            if (string.IsNullOrWhiteSpace(ngrokUrl))
                throw new Exception("Ngrok URL not found or invalid.");

            // Prepare request to Python summarizing  API
            


            var apiUrl = ngrokUrl.TrimEnd('/') + "/avatar";

            var payload = new
            {
                voice_name = VoiceName,
                charecter_pic = charecterpic
            };




            
            using var httpClient = new HttpClient();
            //  Replace with your actual Python API or external endpoint
           // var url = $"http://localhost:5001/api/avatar/{projectOrVideoId}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            // Assume the API returns the avatar URL as plain text
            var avatarUrl = await response.Content.ReadAsStringAsync();
            
            var avatarUrl = string.Empty;
            return avatarUrl;

        }*/



        /*
        /// <summary>
        public async Task<string> GenerateSummary(string transcript)
        {
            // Get ngrok URL
            var ngrokUrl = GetNgrokBaseUrl(summary);
            if (string.IsNullOrWhiteSpace(ngrokUrl))
                throw new Exception("Ngrok URL not found.");

            // Prepare request to Python API
            var apiUrl = ngrokUrl.TrimEnd('/') + "/generate-summary-pdf";
            var payload = new { transcript = transcript };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to call Python API: {response.StatusCode}");

            var responseString = await response.Content.ReadAsStringAsync();

            // Parse the response to extract pdf_url and html
            using var doc = JsonDocument.Parse(responseString);
            var pdfUrl = doc.RootElement.GetProperty("pdf_url").GetString();
            var html = doc.RootElement.GetProperty("html").GetString();

            // Return both as a JSON string
            var result = new
            {
                pdf_url = pdfUrl,
                html = html
            };

            return JsonSerializer.Serialize(result);
        }*/

        /*
        public async Task<byte[]> GenerateSummaryAsync(string projectId)
        {

            /*
            // 1. Get the project and its videos
            var project = await _projectRepository.getProjectId(projectId);
            if (project == null || project.Videos == null || !project.Videos.Any())
                throw new Exception("Project or videos not found.");

            // 2. Get transcripts for all videos depending on it's start time and end time or duration 
            //    (assuming _youtubeRepository.GetTranscriptAsync(videoId) returns the transcript as a string)

            var transcriptBuilder = new StringBuilder();
            foreach (var video in project.Videos)
            {
                var transcript = await _youtubeRepository.GetTranscriptAsync(video.VideoId);
                transcriptBuilder.AppendLine(transcript);
            }
            var allTranscripts = transcriptBuilder.ToString();

            // 3. Call the Python API to generate summary and PDF
           
            using var httpClient = new HttpClient();
            var payload = new { transcript = allTranscripts };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("http://localhost:5001/api/summarize", content);
            response.EnsureSuccessStatusCode();
            

            // 4. Parse the response (assume JSON: { "summary": "...", "pdf": "base64string" })
            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var summaryText = doc.RootElement.GetProperty("summary").GetString();
            var pdfBase64 = doc.RootElement.GetProperty("pdf").GetString();
            var pdfBytes = Convert.FromBase64String(pdfBase64);

            // 5. Save summary text in project
            project.Summary = summaryText;
            SaveProject(string email, string project_name, List<Video> data, string avatar = "" , string summary)
            //  or use a separate method to save the summary
            await _projectRepository.SaveProject or save summary function (project)
;

            // 6. Return PDF bytes
            var pdfBytes = new byte[0]; // Placeholder for actual PDF bytes
            return pdfBytes;
            
        }
        */


        public async Task<string> GenerateSmartSequenceForProjectAsync(List<string> VideoIds, string prompt, List<string> headings)
        {


            // Get ngrok URL
            var ngrokUrl = GetNgrokBaseUrl(autoclipping);
            if (string.IsNullOrWhiteSpace(ngrokUrl))
                throw new Exception("Ngrok URL not found.");

            // Prepare request to Python API
            var apiUrl = ngrokUrl.TrimEnd('/') + "/predict";
            var payload = new
            {
                video_ids = VideoIds,
                prompt = prompt,
                headings = headings
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to call Python API: {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            var videosJson = doc.RootElement.GetProperty("videos");

            List<object> clips = [];

            foreach (var videoResult in videosJson.EnumerateArray())
            {
                var videoId = videoResult.GetProperty("video_id").GetString();
                var matchResult = videoResult.GetProperty("match_result");

                if (videoId != null && matchResult.TryGetProperty("best_start", out var bestStartElem) &&
                                    matchResult.TryGetProperty("best_end", out var bestEndElem))
                {


                    var BestStart = bestStartElem.GetSingle();
                    var BestEnd = bestEndElem.GetSingle();

                    clips.Add(new
                    {
                        best_start = BestStart,
                        best_end = BestEnd,
                        video_id = videoId

                    });

                    /*
                    var captions = new List<SmartCaption>();
                    foreach (var cap in matchResult.GetProperty("captions").EnumerateArray())
                    {
                        captions.Add(new SmartCaption
                        {
                            Text = cap.GetProperty("text").GetString(),
                            Start = cap.GetProperty("start").GetSingle(),
                            End = cap.GetProperty("end").GetSingle(),
                            Similarity = cap.GetProperty("similarity").GetSingle()
                        });
                    }

                    video.Captions = captions;

                    */
                }
            }



            return JsonSerializer.Serialize(clips);
        }


        private static readonly Regex UrlRegex = new Regex(@"https?://[^\s""']+", RegexOptions.Compiled);


        private string GetNgrokBaseUrl(string FilePath)
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



    }
}
