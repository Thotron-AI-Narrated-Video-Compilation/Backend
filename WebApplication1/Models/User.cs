using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace WebApplication1.Models
{
    public class User
    {
        [BsonId]  // Marks this as the MongoDB document ID
        [BsonRepresentation(BsonType.ObjectId)]  // Converts string to ObjectId internally
        private string Id = "";

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string Phone { get; set; }

        //public bool IsPremium { get; set; } = false;


        //public List<Project> Projects = new();
    }
}
