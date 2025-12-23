using Microsoft.EntityFrameworkCore;
using XmlGenCodeServer.Data;
using XmlGenCodeServer.DTOs;
using XmlGenCodeServer.Helpers;
using XmlGenCodeServer.Models;

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

        // 获取所有文件列表
        app.MapPost("/get_all_files", async (AppDbContext db) =>
        {
            var files = await db.Files.ToListAsync();
            var data = files.Select(f => new FileResponse
            {
                Id = f.Id,
                FileName = f.FileName,
                FileContent = Base64Helper.Encode(f.FileContent)
            }).ToList();
            return Results.Json(Result<List<FileResponse>>.Success(data));
        });

        // 创建文件
        app.MapPost("/create_file", async (AppDbContext db) =>
        {
            var file = new FileEntity { FileName = "新程序_temp", FileContent = "" };
            db.Files.Add(file);
            await db.SaveChangesAsync();

            file.FileName = $"新程序_{file.Id}";
            await db.SaveChangesAsync();

            var data = new FileResponse
            {
                Id = file.Id,
                FileName = file.FileName,
                FileContent = Base64Helper.Encode(file.FileContent)
            };
            return Results.Json(Result<FileResponse>.Success(data));
        });

        // 删除文件
        app.MapPost("/delete_file", async (DeleteFileRequest req, AppDbContext db) =>
        {
            var file = await db.Files.FindAsync(req.Id);
            if (file != null)
            {
                db.Files.Remove(file);
                await db.SaveChangesAsync();
            }
            return Results.Json(Result<object>.Success(null));
        });

        // 修改文件名
        app.MapPost("/update_file_name", async (UpdateFileNameRequest req, AppDbContext db) =>
        {
            var file = await db.Files.FindAsync(req.Id);
            if (file != null)
            {
                file.FileName = req.FileName;
                await db.SaveChangesAsync();
            }
            return Results.Json(Result<object>.Success(null));
        });

        // 修改文件内容
        app.MapPost("/update_file_content", async (UpdateFileContentRequest req, AppDbContext db) =>
        {
            var file = await db.Files.FindAsync(req.Id);
            if (file != null)
            {
                file.FileContent = Base64Helper.Decode(req.FileContent);
                await db.SaveChangesAsync();
            }
            return Results.Json(Result<object>.Success(null));
        });

        await app.RunAsync();
    }
}
