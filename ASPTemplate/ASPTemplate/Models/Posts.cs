using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace ASPTemplate.Models
{
    [CollectionName("Posts")]
    public class Posts
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("EmailUser")]
        public string EmailUser { get; set; }

        [BsonElement("Caption")]
        public string? Caption { get; set; } = string.Empty;

        [BsonElement("Location")]
        public string? Location { get; set; } = string.Empty;
        
        [BsonElement("Image")]
        public byte[] Image  { get; set; }

        [BsonElement("Likes")]
        public List<string>? Likes { get; set; } = new List<string>() { };

        [BsonElement("Comments")]
        public List<string>? Comments { get; set; } = new List<string>() { };

    }
}
