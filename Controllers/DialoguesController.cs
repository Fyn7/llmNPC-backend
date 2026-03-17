using Microsoft.AspNetCore.Mvc;
using NPCDialogueServer.DTOs.Requests;
using NPCDialogueServer.DTOs.Responses;
using NPCDialogueServer.Services.Interfaces;

namespace NPCDialogueServer.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DialoguesController : ControllerBase
{
    private readonly IDialogueService _dialogueService;

    public DialoguesController(IDialogueService dialogueService)
    {
        _dialogueService = dialogueService;
    }

    // 1. 获取历史上下文
    [HttpGet("history")]
    public async Task<ActionResult<List<DialogueResponse>>> GetHistory([FromQuery] string username, [FromQuery] string npc_name)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(npc_name))
            return BadRequest(ApiResponse<string>.Fail(400, "参数不能为空"));

        var history = await _dialogueService.GetHistoryAsync(username, npc_name);
        
        if (history == null || history.Count == 0)
            return NotFound(ApiResponse<string>.Fail(404, $"未找到用户「{username}」与NPC「{npc_name}」的对话记录"));

        return Ok(history); // 直接返回数组格式，符合你最初的需求示例
    }

    // 2. 产生新对话
    [HttpPost]
    public async Task<ActionResult<DialogueResponse>> CreateDialogue([FromBody] NewDialogueRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
            return BadRequest(ApiResponse<string>.Fail(400, "对话内容不能为空"));

        var result = await _dialogueService.CreateDialogueAsync(request);
        
        if (result == null)
            return StatusCode(500, ApiResponse<string>.Fail(500, "生成对话失败"));

        return Ok(result); // 直接返回单个对象，符合需求
    }

    // 3. 清除指定NPC上下文
    [HttpDelete("history")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteHistory([FromBody] DeleteDialogueRequest request)
    {
        var success = await _dialogueService.ClearHistoryAsync(request.Username, request.NpcName);
        
        if (!success)
            return NotFound(ApiResponse<string>.Fail(404, "清除失败，记录不存在"));

        return Ok(ApiResponse<string>.Success(null!, $"NPC「{request.NpcName}」的记忆已清除"));
    }
}
