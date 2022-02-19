namespace Users.MinimalWebAPI.Services
{
    public interface IUsersService
    {
        public Task<List<User>> Get();
        public Task<User> Get(string id);
        public Task Create(User user);
        public Task Update(User updateUser);
        public Task Delete(string id);
        public Task DeleteAll();
    }
}
