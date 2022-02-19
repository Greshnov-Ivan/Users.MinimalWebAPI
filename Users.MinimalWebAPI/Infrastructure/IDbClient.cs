using MongoDB.Driver;

namespace Users.MinimalWebAPI.Infrastructure
{
    public interface IDbClient
    {
        IMongoCollection<User> GetUsersCollection();
    }
}
