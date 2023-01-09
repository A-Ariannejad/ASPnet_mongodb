using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace ASPTemplate.Models
{
    [CollectionName("Comments")]
    public class Comments
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("EmailUser")]
        public string EmailUser { get; set; }

        [BsonElement("IdPost")]
        public string IdPost { get; set; }

        [BsonElement("Context")]
        public string Context { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }
    }
}
