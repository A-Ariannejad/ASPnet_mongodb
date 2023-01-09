using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace ASPTemplate.Models
{
    [CollectionName("Stories")]
    public class Stories
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("EmailUser")]
        public string EmailUser { get; set; }

        [BsonElement("Image")]
        public byte[] Image { get; set; }

    }
}
