using MongoDB.Driver;

namespace ASPTemplate.Models
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<Users> _userController;

        public UserService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DataBaseName);
            _userController = database.GetCollection<Users>(settings.UserCollectionName);
        }

        public Users Create(Users user)
        {
            _userController.InsertOne(user);
            return user;
        }
        public List<Users> Get()
        {
            return _userController.Find(x => true).ToList();
        }

        public Users Get(string id)
        {
            return _userController.Find(x => x.Id == id).FirstOrDefault();
        }

        public Users GetByEmail(string email)
        {
            return _userController.Find(x => x.Email == email).FirstOrDefault();
        }

        public void Remove(string id)
        {
            _userController.DeleteOneAsync(x => x.Id == id);
        }

        public void Update(string id, Users user)
        {
            _userController.ReplaceOneAsync(x => x.Id == id, user);
        }
    }
}
