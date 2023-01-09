namespace ASPTemplate.Models
{
    public interface IUserService
    {
        Users Create(Users user);
        List<Users> Get();
        Users Get(string id);
        Users GetByEmail(string email);
        void Update(string id, Users user);
        void Remove(string id);
    }
}
