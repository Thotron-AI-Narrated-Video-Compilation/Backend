using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id;

        public string Name { get; set; } ="";

        public string? Avatar { get; set; }

        public Dictionary<string, List<Video>> project_info { get; set; } = new Dictionary<string, List<Video>>();

        public string user_id;


    }

}