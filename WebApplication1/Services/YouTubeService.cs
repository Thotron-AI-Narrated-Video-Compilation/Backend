using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using YoutubeExplode.Videos;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1.Services
{
    public class YouTubeService : IYoutubeService
    {
        private readonly IYoutubeRepository _youtubeRepository;

        public YouTubeService(IYoutubeRepository youtubeRepository)
        {
            _youtubeRepository = youtubeRepository;
        }

        public async Task<string?> GetTranscriptAsync(string videoId, float start_time, float end_time)
        {
            return await _youtubeRepository.GetTranscriptAsync(videoId, start_time, end_time);
        }

        public async Task<object?> SearchAsync(string query, int maxResults)
        {
            return await _youtubeRepository.SearchAsync(query, maxResults);
        }



    }
}

/*
        public async Task<string> GetTranscriptAsync(string videoId)
        {
            return await _youtubeRepository.GetTranscriptAsync(videoId);
        }
        

        public Task<string> GetTranscript(string videoId)
        {
            return _youtubeRepository.GetTranscript(videoId);
        }

        public string GetTranscript(string videoId)
        {
            return _youtubeRepository.GetTranscript(videoId);
        }

        public string GetBestMatchedCaption(string videoID, string prompt)
        {
            return _youtubeRepository.GetBestMatchedCaption(videoID, prompt);
        }*/