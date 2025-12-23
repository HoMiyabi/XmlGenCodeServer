using System.Text;

namespace XmlGenCodeServer.Helpers;

public static class Base64Helper
{
    public static string Encode(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return "";
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Decode(string base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData)) return "";
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
