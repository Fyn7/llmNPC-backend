namespace NPCDialogueServer.DTOs.Requests;

public class NewDialogueRequest
{
    public string Username { get; set; } = string.Empty;
    public string NpcName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
