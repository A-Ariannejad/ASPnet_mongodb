namespace ASPTemplate.Models
{
    public interface IMongoDBSettings
    {
        public string UserCollectionName { get; set; }
        public string PostCollectionName { get; set; }
        public string StoryCollectionName { get; set; }
        public string LikesCollectionName { get; set; }
        public string CommentsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }
}
