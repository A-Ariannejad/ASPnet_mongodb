using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace ASPTemplate.Models
{
    [CollectionName("Users")]
    [BsonIgnoreExtraElements]
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("Gender")]
        public string? Gender { get; set; } = string.Empty;

        [BsonElement("Bio")]
        public string? Bio { get; set; } = string.Empty;

        [BsonElement("Image")]
        public byte[]? Image { get; set; } = null;

        [BsonElement("Posts")]
        public List<string>? Posts { get; set; } = new List<string>() { };

        [BsonElement("Stories")]
        public List<string>? Stories { get; set; } = new List<string>() { };

        [BsonElement("Followers")]
        public List<string>? Followers { get; set; } = new List<string>() { };

        [BsonElement("Followings")]
        public List<string>? Followings { get; set; } = new List<string>() { };

        [BsonElement("Comments")]
        public List<string>? Comments { get; set; } = new List<string>() { };

        [BsonElement("Likes")]
        public List<string>? Likes { get; set; } = new List<string>() { };

        [BsonElement("SavedPosts")]
        public List<string>? SavedPosts { get; set; } = new List<string>() { };

        [BsonElement("SavedStories")]
        public List<string>? SavedStories { get; set; } = new List<string>() { };
    }
}
