using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPCDialogueServer.Models;

public class Dialogue
{
    [Key]
    public int DialogueId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string NpcName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = string.Empty; // user 或 assistant

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 导航属性，便于 EF Core 进行关联查询
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
