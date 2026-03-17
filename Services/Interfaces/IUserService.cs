using NPCDialogueServer.Models;

namespace NPCDialogueServer.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// 获取用户，如果用户不存在则自动创建一个
    /// </summary>
    Task<User> GetOrCreateUserAsync(string username);
}
