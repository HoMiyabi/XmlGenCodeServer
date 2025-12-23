using Microsoft.EntityFrameworkCore;
using XmlGenCodeServer.Models;

namespace XmlGenCodeServer.Data;

public class AppDbContext : DbContext
{
    public DbSet<FileEntity> Files => Set<FileEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        if (!Directory.Exists(dataDir))
        {
            Directory.CreateDirectory(dataDir);
        }
        var dbPath = Path.Combine(dataDir, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
