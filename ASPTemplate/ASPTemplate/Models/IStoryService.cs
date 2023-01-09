namespace ASPTemplate.Models
{
    public interface IStoryService
    {
        Stories Create(Stories story);
        List<Stories> Get();
        Stories Get(string id);
        List<Stories> GetByUserEmail(string email);
        void Update(string id, Stories story);
        void Remove(string id);
    }
}
