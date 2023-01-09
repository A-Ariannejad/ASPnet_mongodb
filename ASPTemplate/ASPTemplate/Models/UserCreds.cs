using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Buffers.Text;
using System.Collections.Generic;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System.Security.Policy;

namespace ASPTemplate.Models
{
    [CollectionName("UsersCreds")]
    [BsonIgnoreExtraElements]
    public class UserCreds : MongoIdentityUser<Guid>
    {
        public string Fullname { get; set; } = string.Empty;

    }
}
