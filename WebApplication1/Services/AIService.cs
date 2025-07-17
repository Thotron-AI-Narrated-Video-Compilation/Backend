using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class AIService : IAIService
    {
        private readonly IAIRepository _aiRepository;

        public AIService(IAIRepository aiRepository)
        {
            _aiRepository = aiRepository;
        }



        public async Task<TopicExtractionResponse> ExtractTopicsFromPdfAsync(IFormFile pdfFile)
        {
            return await _aiRepository.ExtractTopicsFromPdfAsync(pdfFile);
        }

        public async Task<TopicExtractionResponse> ExtractTopicsFromScriptAsync(string script)
        {
            return await _aiRepository.ExtractTopicsFromScriptAsync(script);
        }

        public async Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request)
        {
            return await _aiRepository.GenerateImageAsync(request);
        }

        public async Task<string> GenerateSmartSequenceForProjectAsync(List<string> VideoIds, string prompt, List<string> headings)
        {
            return await _aiRepository.GenerateSmartSequenceForProjectAsync(VideoIds, prompt, headings);
        }

        public async Task<string> GenerateSummary(string transcript)
        {
            return await _aiRepository.GenerateSummary(transcript);
        }


        public async Task<string> GenerateAvatarWithSummary(string _Summary, string voiceName, string characterPicUrl)
        {
            return await _aiRepository.GenerateAvatarWithSummary(_Summary, voiceName, characterPicUrl);
        }


        /*
        public async Task<byte[]> GenerateSummaryAsync(string transcript)
        {
            return await _aiRepository.GenerateSummaryAsync(transcript);
        }
        */

        /*
        public async Task<string> GetAvatarAsync(string VoiceName, string charecterpic)
        {
            return await _aiRepository.GetAvatarAsync(VoiceName,charecterpic);
        }
        */
    }
}
