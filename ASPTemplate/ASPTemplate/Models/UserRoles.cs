using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace ASPTemplate.Models
{
    [CollectionName("roles")]
    public class UserRoles : MongoIdentityRole<Guid>
    {

    }
}
