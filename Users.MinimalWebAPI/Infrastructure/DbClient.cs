using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Users.MinimalWebAPI.Infrastructure
{
    public class DbClient : IDbClient
    {
        private readonly IMongoCollection<User> _users;
        public DbClient(IOptions<DbConfig> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);

            var database = client.GetDatabase(options.Value.DatabaseName);

            _users = database.GetCollection<User>(options.Value.UsersCollectionName);
        }
        public IMongoCollection<User> GetUsersCollection() => _users;
    }
}
