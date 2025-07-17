using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Subtopic
    {
        public string Text { get; set; }
    }



    public class scriptObj
    {
        public string Script { get; set; }
    }
    public class Topic
    {
        public string topic { get; set; }
        public List<string> subtopics { get; set; }
    }

    public class TopicExtractionResponse
    {
        public List<Topic> Topics { get; set; }
    }

    public class ImageGenerationRequest
    {
        public string topic { get; set; }
        public string subtopic { get; set; }
    }

    public class ImageGenerationResponse
    {
        public string ImageUrl { get; set; }
    }

    public class GenerateSummaryAsync
    {
        public string Transcript { get; set; }
        public string ProjectId { get; set; }
        public string Summary { get; set; }

    }
}
