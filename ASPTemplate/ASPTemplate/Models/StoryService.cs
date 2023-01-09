using MongoDB.Driver;

namespace ASPTemplate.Models
{
    public class StoryService : IStoryService
    {

        private readonly IMongoCollection<Stories> _storyController;

        public StoryService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DataBaseName);
            _storyController = database.GetCollection<Stories>(settings.StoryCollectionName);
        }
        public Stories Create(Stories story)
        {
            _storyController.InsertOne(story);
            return story;
        }

        public List<Stories> Get()
        {
            return _storyController.Find(x => true).ToList();
        }

        public Stories Get(string id)
        {
            return _storyController.Find(x => x.Id == id).FirstOrDefault();
        }

        public List<Stories> GetByUserEmail(string email)
        {
            return _storyController.Find(x => x.EmailUser == email).ToList();
        }

        public void Remove(string id)
        {
            _storyController.DeleteOneAsync(x => x.Id == id);
        }

        public void Update(string id, Stories story)
        {
            _storyController.ReplaceOneAsync(x => x.Id == id, story);
        }
    }
}
