using MySqlConnector;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string connectionString = "Server=localhost;Database=appdb;User=appuser;Password=MySuperSecretPass123!;";
string redisConnString = "localhost";

var redis = ConnectionMultiplexer.Connect(redisConnString);
var cache = redis.GetDatabase();

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.MapGet("/api/users", async () =>
{
    
    string? cachedData = await cache.StringGetAsync("users_list");
    if (!string.IsNullOrEmpty(cachedData))
    {
       
        var usersList = JsonSerializer.Deserialize<List<string>>(cachedData);
        usersList.Add("--- (Served from Redis Cache) ---"); 
        return usersList;
    }

    
    var users = new List<string>();
    using (var conn = new MySqlConnection(connectionString))
    {
        await conn.OpenAsync();
        using (var cmd = new MySqlCommand("SELECT Name FROM Users", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                users.Add(reader.GetString(0));
            }
        }
    }

    
    await cache.StringSetAsync("users_list", JsonSerializer.Serialize(users), TimeSpan.FromSeconds(60));
    
    users.Add("--- (Served from MySQL Database) ---");
    return users;
});
app.Run("http://localhost:5000");
