using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XmlGenCodeServer.Models;

[Table("file")]
public class FileEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("file_name")]
    public string FileName { get; set; } = string.Empty;

    [Column("file_content")]
    public string FileContent { get; set; } = string.Empty;
}
