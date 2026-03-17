using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPCDialogueServer.Entities
{
    [Table("dialogues")]
    public class Dialogue
    {
        [Key]
        [Column("dialogue_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DialogueId { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("npc_name")]
        [Required]
        [MaxLength(50)]
        public string NpcName { get; set; } = string.Empty;

        [Column("role")]
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty; // user/assistant

        [Column("content")]
        [Required]
        public string Content { get; set; } = string.Empty;

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // 导航属性：关联用户
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
