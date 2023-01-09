using MongoDB.Driver;

namespace ASPTemplate.Models
{
    public class PostService : IPostService
    {
        private readonly IMongoCollection<Posts> _postController;

        public PostService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DataBaseName);
            _postController = database.GetCollection<Posts>(settings.PostCollectionName);
        }

        public Posts Create(Posts post)
        {
            _postController.InsertOne(post);
            return post;
        }

        public List<Posts> Get()
        {
            return _postController.Find(x => true).ToList();
        }

        public Posts Get(string id)
        {
            return _postController.Find(x => x.Id == id).FirstOrDefault();
        }

        public List<Posts> GetByUserEmail(string email)
        {
            return _postController.Find(x => x.EmailUser == email).ToList();
        }

        public void Remove(string id)
        {
            _postController.DeleteOneAsync(x => x.Id == id);
        }

        public void Update(string id, Posts post)
        {
            _postController.ReplaceOneAsync(x => x.Id == id, post);
        }
    }
}
