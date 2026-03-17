using Microsoft.EntityFrameworkCore;
using NPCDialogueServer.Data;
using NPCDialogueServer.DTOs.Requests;
using NPCDialogueServer.DTOs.Responses;
using NPCDialogueServer.Models;
using NPCDialogueServer.Services.Interfaces;

namespace NPCDialogueServer.Services.Implementations;

public class DialogueService : IDialogueService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;
    private readonly ILLMService _llmService;

    public DialogueService(AppDbContext context, IUserService userService, ILLMService llmService)
    {
        _context = context;
        _userService = userService;
        _llmService = llmService;
    }

    public async Task<List<DialogueResponse>> GetHistoryAsync(string username, string npcName)
    {
        return await _context.Dialogues
            .Include(d => d.User)
            .Where(d => d.User!.Username == username && d.NpcName == npcName)
            .OrderBy(d => d.CreatedAt)
            .Select(d => new DialogueResponse
            {
                Role = d.Role,
                Content = d.Content
            })
            .ToListAsync();
    }

    public async Task<DialogueResponse?> CreateDialogueAsync(NewDialogueRequest request)
    {
        // 1. 获取或创建用户
        var user = await _userService.GetOrCreateUserAsync(request.Username);

        // 2. 获取该 NPC 的历史上下文
        var history = await GetHistoryAsync(request.Username, request.NpcName);

        // 3. 调用大模型获取回复
        var aiReply = await _llmService.GetChatResponseAsync(history, request.Content, request.NpcName);

        // 4. 将用户消息和AI回复持久化到数据库
        var userMsg = new Dialogue
        {
            UserId = user.UserId,
            NpcName = request.NpcName,
            Role = "user",
            Content = request.Content
        };

        var assistantMsg = new Dialogue
        {
            UserId = user.UserId,
            NpcName = request.NpcName,
            Role = "assistant",
            Content = aiReply
        };

        _context.Dialogues.AddRange(userMsg, assistantMsg);
        await _context.SaveChangesAsync();

        return new DialogueResponse { Role = "assistant", Content = aiReply };
    }

    public async Task<bool> ClearHistoryAsync(string username, string npcName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var records = _context.Dialogues
            .Where(d => d.UserId == user.UserId && d.NpcName == npcName);

        _context.Dialogues.RemoveRange(records);
        await _context.SaveChangesAsync();
        return true;
    }
}
