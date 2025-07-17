using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IAIRepository
    {
        Task<TopicExtractionResponse> ExtractTopicsFromPdfAsync(IFormFile pdfFile);
        Task<TopicExtractionResponse> ExtractTopicsFromScriptAsync(string script);
        Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request);

        Task<string> GenerateSmartSequenceForProjectAsync(List<string> VideoIds, string prompt, List<string> headings);

        //Task<byte[]> GenerateSummaryAsync(string projectId);
        Task<string> GenerateSummary(string transcript);
        Task<string> GenerateAvatarWithSummary(string _Summary, string voiceName, string characterPicUrl);


        //Task<string> GetAvatarAsync(string VoiceName, string charecterpic);




    }
}
