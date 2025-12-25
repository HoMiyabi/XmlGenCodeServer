using Microsoft.EntityFrameworkCore;
using XmlGenCodeServer.Data;
using XmlGenCodeServer.DTOs;
using XmlGenCodeServer.Helpers;
using XmlGenCodeServer.Models;

namespace XmlGenCodeServer.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/get_all_files", GetAllFiles);
        app.MapPost("/create_file", CreateFile);
        app.MapPost("/delete_file", DeleteFile);
        app.MapPost("/duplicate_file", DuplicateFile);
        app.MapPost("/update_file_name", UpdateFileName);
        app.MapPost("/update_file_content", UpdateFileContent);
    }

    private static async Task<IResult> GetAllFiles(AppDbContext db)
    {
        var files = await db.Files.ToListAsync();
        var data = files.Select(f => new FileResponse
        {
            Id = f.Id,
            FileName = f.FileName,
            FileContent = Base64Helper.Encode(f.FileContent)
        }).ToList();
        return Results.Json(Result<List<FileResponse>>.Success(data));
    }

    private static async Task<IResult> CreateFile(AppDbContext db)
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
    }

    private static async Task<IResult> DeleteFile(DeleteFileRequest req, AppDbContext db)
    {
        var file = await db.Files.FindAsync(req.Id);
        if (file != null)
        {
            db.Files.Remove(file);
            await db.SaveChangesAsync();
        }
        return Results.Json(Result<object>.Success(null));
    }

    private static async Task<IResult> DuplicateFile(DuplicateFileRequest req, AppDbContext db)
    {
        var originalFile = await db.Files.FindAsync(req.Id);
        if (originalFile == null)
        {
            return Results.Json(Result<FileResponse>.Error("文件不存在"));
        }

        var baseName = originalFile.FileName;
        string? newFileName = null;
        const int maxAttempts = 100;

        for (int i = 1; i <= maxAttempts; i++)
        {
            var candidate = $"{baseName}_{i}";
            if (!await db.Files.AnyAsync(f => f.FileName == candidate))
            {
                newFileName = candidate;
                break;
            }
        }

        if (newFileName == null)
        {
            return Results.Json(Result<FileResponse>.Error("复制失败：重名尝试次数过多"));
        }

        var newFile = new FileEntity
        {
            FileName = newFileName,
            FileContent = originalFile.FileContent
        };

        db.Files.Add(newFile);
        await db.SaveChangesAsync();

        var data = new FileResponse
        {
            Id = newFile.Id,
            FileName = newFile.FileName,
            FileContent = Base64Helper.Encode(newFile.FileContent)
        };
        return Results.Json(Result<FileResponse>.Success(data));
    }

    private static async Task<IResult> UpdateFileName(UpdateFileNameRequest req, AppDbContext db)
    {
        var file = await db.Files.FindAsync(req.Id);
        if (file != null)
        {
            file.FileName = req.FileName;
            await db.SaveChangesAsync();
        }
        return Results.Json(Result<object>.Success(null));
    }

    private static async Task<IResult> UpdateFileContent(UpdateFileContentRequest req, AppDbContext db)
    {
        var file = await db.Files.FindAsync(req.Id);
        if (file != null)
        {
            file.FileContent = Base64Helper.Decode(req.FileContent);
            await db.SaveChangesAsync();
        }
        return Results.Json(Result<object>.Success(null));
    }
}
