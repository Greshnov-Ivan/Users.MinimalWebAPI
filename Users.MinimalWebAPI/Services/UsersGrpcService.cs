using Grpc.Core;

namespace Users.MinimalWebAPI.Services
{
    internal sealed class UsersGrpcService : UsersGrpc.UsersGrpcBase
    {
        private readonly IUsersService _usersService;
        public UsersGrpcService(IUsersService usersService) => (_usersService) = (usersService);
        public override async Task GetListUsers(GetUsersRequest request, IServerStreamWriter<UserResponse> responseStream, ServerCallContext context)
        {
            var users = await _usersService.Get();

            foreach (var response in users.Select(u => new UserResponse { Id = u.Id, Name = u.Name}))
            {
                await responseStream.WriteAsync(response);
            }
        }
    }
}
