using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("extract-topics")]
        public async Task<IActionResult> ExtractTopics(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest("Please upload a valid PDF file.");
            }

            if (!pdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only PDF files are supported.");
            }

            try
            {
                var result = await _aiService.ExtractTopicsFromPdfAsync(pdfFile);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("generate-image")]
        public async Task<IActionResult> GenerateImage(string Topic, string Subtopic)
        {
            if (string.IsNullOrWhiteSpace(Topic) || string.IsNullOrWhiteSpace(Subtopic))
            {
                return BadRequest("Both topic and subtopic are required.");
            }

            try
            {
                var request = new ImageGenerationRequest
                {
                    topic = Topic,
                    subtopic = Subtopic
                };

                var result = await _aiService.GenerateImageAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("extract-topics-byscript")]
        public async Task<IActionResult> ExtractTopicsByScript([FromBody] scriptObj script)
        {
            if (script == null || string.IsNullOrWhiteSpace(script.Script))
            {
                return BadRequest("Script content cannot be empty.");
            }

            try
            {
                var result = await _aiService.ExtractTopicsFromScriptAsync(script.Script);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("generate-summary")]
        public async Task<IActionResult> GenerateSummary([FromBody] JsonElement body)
        {
            if ((body.ValueKind != JsonValueKind.Object) || (!body.TryGetProperty("transcript", out var transcriptElement) ))
            {
                return BadRequest("Transcript is required.");
            }

            string transcript = transcriptElement.GetString();
            if (string.IsNullOrWhiteSpace(transcript))
            {
                return BadRequest("Transcript cannot be empty.");
            }



            try
            {
                var jsonResult = await _aiService.GenerateSummary(transcript);

                // Return raw JSON as a response
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        /*
        [HttpPost("generate-summary")]
        public async Task<IActionResult> GenerateSummary([FromBody] JsonElement body)
        {
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("transcript", out var transcriptElement) ||
                string.IsNullOrWhiteSpace(transcriptElement.GetString()))
            {
                return BadRequest("Transcript is required.");
            }

            string transcript = transcriptElement.GetString();
            try
            {
                var url_html = await _aiService.GenerateSummary(transcript);
                if (string.IsNullOrEmpty(url_html))
                {
                    return StatusCode(500, "Failed to call generate summary.");
                }

                // Parse the returned JSON string to extract pdf_url and html
                using var doc = JsonDocument.Parse(url_html);
                var root = doc.RootElement;

                if (!root.TryGetProperty("pdf_url", out var pdfUrlElement) || string.IsNullOrEmpty(pdfUrlElement.GetString()))
                {
                    return StatusCode(500, "Failed to extract PDF url.");
                }
                if (!root.TryGetProperty("html", out var htmlElement) || string.IsNullOrEmpty(htmlElement.GetString()))
                {
                    return StatusCode(500, "Failed to extract HTML content.");
                }

                string pdfUrl = pdfUrlElement.GetString();
                string html = htmlElement.GetString();

                return Ok(new { pdf_url = pdfUrl, html = html });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }*/

        /*
        [HttpPost("generate-summary/{projectId}")]
        public async Task<IActionResult> GenerateSummary(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return BadRequest("Project ID is required.");
            }

            try
            {
                var pdfBytes = await _aiService.GenerateSummaryAsync(projectId);
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return StatusCode(500, "Failed to generate summary PDF.");
                }
                // Return the PDF file
                return File(pdfBytes, "application/pdf", "summary.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        */


        [HttpPost("generate-smart-sequence")]
        public async Task<IActionResult> GenerateSmartSequence([FromBody] JsonElement body)
        {

            try
            {
                if (!body.TryGetProperty("video_ids", out var videoIdElement) || videoIdElement.ValueKind != JsonValueKind.Array)
                    return BadRequest("Video ids must be an array.");

                if (!body.TryGetProperty("prompt", out var promptElement) || string.IsNullOrWhiteSpace(promptElement.GetString()))
                    return BadRequest("Prompt is required.");

                if (!body.TryGetProperty("headings", out var headingsElement) || headingsElement.ValueKind != JsonValueKind.Array)
                    return BadRequest("Headings must be an array.");


                var video_ids = videoIdElement.EnumerateArray()
                                              .Where(h => h.ValueKind == JsonValueKind.String)
                                              .Select(h => h.GetString())
                                              .ToList();
                string prompt = promptElement.GetString();
                var headings = headingsElement.EnumerateArray()
                                              .Where(h => h.ValueKind == JsonValueKind.String)
                                              .Select(h => h.GetString())
                                              .ToList();
                

                var videos = await _aiService.GenerateSmartSequenceForProjectAsync(video_ids, prompt, headings);

                return Ok(new { videos }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpPost("generate-avatar-with-summary")]
        public async Task<IActionResult> GenerateAvatarWithSummary([FromBody] JsonElement requestBody)
        {
            try
            {
                var _Summary = requestBody.GetProperty("summary").GetString();
                var voiceName = requestBody.GetProperty("voice_name").GetString();
                var characterPicUrl = requestBody.GetProperty("pic").GetString();

                var result = await _aiService.GenerateAvatarWithSummary(_Summary, voiceName, characterPicUrl);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        /*
        [HttpGet("avatar/{projectOrVideoId}")]
        public async Task<IActionResult> GetAvatar(string VoiceName, string charecterpic)
        {
            var avatarUrl = await _aiService.GetAvatarAsync(VoiceName , charecterpic);
            if (string.IsNullOrEmpty(avatarUrl))
                return NotFound();
            return Ok(avatarUrl);
        }
        */
    }
}
