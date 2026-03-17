namespace NPCDialogueServer.Configurations;

public class LLMConfig
{
    public string ApiUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    public int Timeout { get; set; } = 30; // 秒
}
