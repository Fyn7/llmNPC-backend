using Microsoft.EntityFrameworkCore;
using NPCDialogueServer.Models;

namespace NPCDialogueServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Dialogue> Dialogues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. 配置用户表
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            // 给用户名加上唯一索引，查询更快且防止重复
            entity.HasIndex(u => u.Username).IsUnique();
        });

        // 2. 配置对话记录表
        modelBuilder.Entity<Dialogue>(entity =>
        {
            entity.ToTable("dialogues");
            
            // 配置外键关系
            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // 用户删除时级联删除对话

            // 为查询优化添加复合索引 (UserId + NpcName)
            entity.HasIndex(d => new { d.UserId, d.NpcName });
        });
    }
}
