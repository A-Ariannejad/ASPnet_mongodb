namespace ASPTemplate.Models
{
    public interface IPostService
    {
        Posts Create(Posts post);
        List<Posts> Get();
        Posts Get(string id);
        List<Posts> GetByUserEmail(string email);
        void Update(string id, Posts post);
        void Remove(string id);
    }
}
