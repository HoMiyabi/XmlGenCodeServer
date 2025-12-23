namespace XmlGenCodeServer.DTOs;

public class Result<T>
{
    public int Code { get; set; }
    public string Msg { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static Result<T> Success(T? data, string msg = "success") => new() { Code = 0, Msg = msg, Data = data };
    public static Result<T> Error(string msg, int code = 1) => new() { Code = code, Msg = msg };
}

public record CreateFileRequest();
public record DeleteFileRequest(int Id);
public record UpdateFileNameRequest(int Id, string FileName);
public record UpdateFileContentRequest(int Id, string FileContent);

public class FileResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileContent { get; set; } = string.Empty;
}
