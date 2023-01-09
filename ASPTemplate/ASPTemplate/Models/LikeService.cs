using MongoDB.Driver;

namespace ASPTemplate.Models
{
    public class LikeService : ILikeService
    {
        private readonly IMongoCollection<Likes> _likeController;

        public LikeService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DataBaseName);
            _likeController = database.GetCollection<Likes>(settings.LikesCollectionName);
        }
        public Likes Create(Likes like)
        {
            _likeController.InsertOne(like);
            return like;
        }

        public Likes Get(string id)
        {
            return _likeController.Find(x => x.Id == id).FirstOrDefault();
        }            

        public List<Likes> GetByPostId(string id)
        {
            return _likeController.Find(x => x.Id == id).ToList();
        }

        public List<Likes> GetByUserEmail(string email)
        {
            return _likeController.Find(x => x.EmailUser == email).ToList();
        }

        public void Remove(string id)
        {
            _likeController.DeleteOneAsync(x => x.Id == id);
        }
    }
}
