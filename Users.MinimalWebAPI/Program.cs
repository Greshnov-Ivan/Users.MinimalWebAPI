using Users.MinimalWebAPI;
using Users.MinimalWebAPI.Infrastructure;
using Users.MinimalWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.Configure<DbConfig>(builder.Configuration.GetSection("UsersDbSettings"));
builder.Services.Configure<KafkaProducerConfig>(builder.Configuration.GetSection("KafkaProducerConfigure"));
builder.Services.AddSingleton<IDbClient, DbClient>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IKafkaProducerService, KafkaProducerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "I'am Users API!")
    .ExcludeFromDescription();
app.MapGet("/api/users", async (IUsersService usersService) => await usersService.Get())
    .Produces<List<User>>(StatusCodes.Status200OK)
    .WithName("GetUser")
    .WithTags("Getters");
app.MapGet("/api/users/{id}", async (IUsersService usersService, string id) =>
{
    var user = await usersService.Get(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
})
    .Produces<User>(StatusCodes.Status200OK)
    .WithName("GetAllUsers")
    .WithTags("Getters");
app.MapPost("/api/users", async (IUsersService usersService, IKafkaProducerService kafkaProduceService, User user) =>
{
    var cts = new CancellationTokenSource();
    await usersService.Create(user);
    Task timer = Task.Run(() => SomeWork(cts));
    await kafkaProduceService.SendMessageAsync<string>("NewUserCreated", user.Id, cts.Token);
    return Results.Created($"/api/users/", user.Id);
})
    .Accepts<User>("application/json")
    .Produces<User>(StatusCodes.Status201Created)
    .WithName("CreateUser")
    .WithTags("Creators");
app.MapPut("/api/users", async (IUsersService usersService, User updateUser) =>
{
    var user = await usersService.Get(updateUser.Id);
    if (user is null)
        return Results.NotFound();

    await usersService.Update(updateUser);
    return Results.Ok();
})
    .Accepts<User>("application/json")
    .WithName("UpdateUser")
    .WithTags("Updaters");
app.MapDelete("/api/users/{id}", async (IUsersService usersService, string id) =>
{
    var user = await usersService.Get(id);
    if (user is null)
        return Results.NotFound();

    await usersService.Delete(id);
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteUser")
    .WithTags("Deleters");
app.MapDelete("/api/users", async (IUsersService usersService) =>
{
    await usersService.DeleteAll();
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteAllUsers")
    .WithTags("Deleters");

app.UseHttpsRedirection();

app.Run();

async Task SomeWork(CancellationTokenSource cts)
{
    await Task.Delay(TimeSpan.FromSeconds(60));

    cts.Cancel();
}
