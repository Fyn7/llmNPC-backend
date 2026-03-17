using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NPCDialogueServer.Configurations;
using NPCDialogueServer.DTOs.Responses;
using NPCDialogueServer.Services.Interfaces;

namespace NPCDialogueServer.Services.Implementations;

public class LLMService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly LLMConfig _config;

    public LLMService(HttpClient httpClient, IOptions<LLMConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
    }

    public async Task<string> GetChatResponseAsync(List<DialogueResponse> history, string newMessage, string npcName)
    {
        // TODO: 这里根据你未来确定的模型（OpenAI/DeepSeek/文心一言等）调整请求体结构
        // 目前采用最通用的 OpenAI Chat Completion 格式
        var messages = history.Select(h => new { role = h.Role, content = h.Content }).ToList();
        messages.Add(new { role = "user", content = newMessage });

        var requestBody = new
        {
            model = _config.ModelName, // 在 appsettings.json 中配置
            messages = messages,
            temperature = 0.7
        };

        var jsonPayload = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // TODO: 确认你的模型是否需要特殊的 Header (如 Authorization)
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

        try
        {
            // TODO: _config.ApiUrl 需要在 appsettings.json 中填写完整的 API 地址
            var response = await _httpClient.PostAsync(_config.ApiUrl, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            
            // 使用 System.Text.Json 解析返回结果
            using var doc = JsonDocument.Parse(responseJson);
            // TODO: 根据不同模型的返回 JSON 路径进行调整。以下是标准 OpenAI 路径：
            // choices[0].message.content
            var reply = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return reply ?? "NPC 陷入了沉思...";
        }
        catch (Exception ex)
        {
            // 实际开发建议记录日志
            return $"[转发服务器错误]: {ex.Message}";
        }
    }
}
