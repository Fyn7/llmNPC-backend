using NPCDialogueServer.DTOs.Responses;

namespace NPCDialogueServer.Services.Interfaces;

public interface ILLMService
{
    /// <summary>
    /// 调用大模型获取回复
    /// </summary>
    /// <param name="history">历史对话上下文</param>
    /// <param name="newMessage">用户刚发送的新消息</param>
    /// <param name="npcName">NPC名字（用于可能的人设注入）</param>
    /// <returns>AI回复的纯文本内容</returns>
    Task<string> GetChatResponseAsync(List<DialogueResponse> history, string newMessage, string npcName);
}
