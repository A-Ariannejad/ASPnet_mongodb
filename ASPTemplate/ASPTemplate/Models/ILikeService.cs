namespace ASPTemplate.Models
{
    public interface ILikeService
    {
        Likes Create(Likes like);
        List<Likes> GetByPostId(string id);
        Likes Get(string id);
        List<Likes> GetByUserEmail(string email);
        void Remove(string id);
    }
}
