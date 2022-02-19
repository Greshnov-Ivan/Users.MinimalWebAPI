using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Users.MinimalWebAPI.Infrastructure;

namespace Users.MinimalWebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IMongoCollection<User> _users;

        public UsersService(IDbClient dbClient) =>
            _users = dbClient.GetUsersCollection();

        public async Task<List<User>> Get() =>
            await _users.Find(_ => true).ToListAsync();
        public async Task<User> Get(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        public async Task Create(User user) =>
            await _users.InsertOneAsync(user);
        public async Task Update(User updateUser) =>
            await _users.ReplaceOneAsync(u => u.Id == updateUser.Id, updateUser);
        public async Task Delete(string id) =>
            await _users.DeleteOneAsync(u => u.Id == id);
        public async Task DeleteAll() =>
            await _users.DeleteManyAsync(_ => true);
    }
}
