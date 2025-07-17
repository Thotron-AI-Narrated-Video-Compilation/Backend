using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAIService
    {
        Task<TopicExtractionResponse> ExtractTopicsFromPdfAsync(IFormFile pdfFile);
        Task<TopicExtractionResponse> ExtractTopicsFromScriptAsync(string script);
        Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request);
        //Task<byte[]> GenerateSummaryAsync(string transcript);
        Task<string> GenerateSmartSequenceForProjectAsync(List<string> VideoIds, string prompt, List<string> headings);

        Task<string> GenerateAvatarWithSummary(string _Summary, string voiceName, string characterPicUrl);



        //Task<string> GetAvatarAsync(string VoiceName, string charecterpic);
        Task<string> GenerateSummary(string transcript);


    }
}
