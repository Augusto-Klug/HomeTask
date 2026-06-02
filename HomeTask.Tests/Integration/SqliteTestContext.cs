using HomeTask.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HomeTask.Tests.Integration;

internal sealed class SqliteTestContext : IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestContext()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<HomeTaskDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new HomeTaskDbContext(options);
        Db.Database.EnsureCreated();
    }

    public HomeTaskDbContext Db { get; }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
