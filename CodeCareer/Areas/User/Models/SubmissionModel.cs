using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeCareer.Areas.User.Models;

public class SubmissionModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel? User { get; set; }

    public int TaskId { get; set; }

    [ForeignKey(nameof(TaskId))]
    public TaskModel? Task { get; set; }

    [MaxLength(32)]
    public string Language { get; set; } = "csharp";

    [Column(TypeName = "longtext")]
    public string SourceCode { get; set; } = string.Empty;

    [MaxLength(32)]
    public string Status { get; set; } = "Pending";

    public int Score { get; set; }

    public double? ExecutionTime { get; set; }

    public int? MemoryUsed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
