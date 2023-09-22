using Microsoft.EntityFrameworkCore;

namespace DynamicLinq;

public class DataWrapper
{
    public UserContext Context;
    public DataWrapper()
    {
        Context = new UserContext();
        Context.Fill();
    }
}

public class UserContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "User");
    }

    public void Fill()
    {
        for (int i = 1; i <= 100; i++)
        {
            var name = RandomString(6);
            Users.Add(new User() { Id = i, Name = name, Email = $"{name}@test.com", Score = Random.Shared.Next(0, 100), Blocked = Random.Shared.NextDouble() >= 0.5, Deleted = Random.Shared.NextDouble() >= 0.5 });
        }
        SaveChanges();
        string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }
    }
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


    public DbSet<User> Users { get; set; }
}