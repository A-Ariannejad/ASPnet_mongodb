namespace ASPTemplate.Models
{
    public interface ICommentService
    {
        Comments Create(Comments comment);
        List<Comments> GetByPostId(string id);
        Comments Get(string id);
        List<Comments> GetByUserEmail(string email);
        void Remove(string id);
    }
}
