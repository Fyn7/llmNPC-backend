using NPCDialogueServer.DTOs.Requests;
using NPCDialogueServer.DTOs.Responses;

namespace NPCDialogueServer.Services.Interfaces;

public interface IDialogueService
{
    /// <summary>
    /// 获取用户与指定NPC的历史记录
    /// </summary>
    Task<List<DialogueResponse>> GetHistoryAsync(string username, string npcName);

    /// <summary>
    /// 处理新消息：保存用户消息 -> 调LLM -> 保存AI回复 -> 返回结果
    /// </summary>
    Task<DialogueResponse?> CreateDialogueAsync(NewDialogueRequest request);

    /// <summary>
    /// 清除用户与指定NPC的对话记忆
    /// </summary>
    Task<bool> ClearHistoryAsync(string username, string npcName);
}
