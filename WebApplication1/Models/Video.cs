using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Security.Policy;

namespace WebApplication1.Models
{
    public class Video
    {
        public string title { get; set; } = string.Empty;
        public string id { get; set; }
        public string url { get; set; }
        public string thumbnail { get; set; }
        public float start_time { get; set; }
        public float end_time { get; set; }

        // Add smart sequence fields
        public float? BestStart;
        public float? BestEnd;
        public List<SmartCaption> Captions = new List<SmartCaption>(); 

    }

    public class SmartCaption
    {
        public string Text;
        public float Start;
        public float End;
        public float Similarity;
    }
}