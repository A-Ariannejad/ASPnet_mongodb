using MongoDB.Driver;

namespace ASPTemplate.Models
{
    public class CommentService : ICommentService
    {
        private readonly IMongoCollection<Comments> _commentController;

        public CommentService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DataBaseName);
            _commentController = database.GetCollection<Comments>(settings.CommentsCollectionName);
        }
        public Comments Create(Comments comment)
        {
            _commentController.InsertOne(comment);
            return comment;
        }

        public Comments Get(string id)
        {
            return _commentController.Find(x => x.Id == id).FirstOrDefault();
        }

        public List<Comments> GetByPostId(string id)
        {
            return _commentController.Find(x => x.Id == id).ToList();
        }

        public List<Comments> GetByUserEmail(string email)
        {
            return _commentController.Find(x => x.EmailUser == email).ToList();
        }

        public void Remove(string id)
        {
            _commentController.DeleteOneAsync(x => x.Id == id);
        }
    }
}
