using XmlGenCodeServer.Data;
using XmlGenCodeServer.Endpoints;

namespace XmlGenCodeServer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDbContext>();

        var app = builder.Build();

        // 自动创建数据库
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        // 注册路由
        app.MapFileEndpoints();

        await app.RunAsync();
    }
}
