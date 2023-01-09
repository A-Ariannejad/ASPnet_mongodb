namespace ASPTemplate.Models
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string UserCollectionName { get; set; } = string.Empty;
        public string PostCollectionName { get; set; } = string.Empty;
        public string StoryCollectionName { get; set; } = string.Empty;
        public string LikesCollectionName { get; set; } = string.Empty;
        public string CommentsCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DataBaseName { get; set; } = string.Empty;
    }
}
