using Microsoft.EntityFrameworkCore;
using NPCDialogueServer.Configurations;
using NPCDialogueServer.Data;
using NPCDialogueServer.Services.Implementations;
using NPCDialogueServer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- 1. 注册核心服务 ---

// 控制器支持，并配置 JSON 策略为 CamelCase（小驼峰）
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 注册 OpenAPI (Swagger)
builder.Services.AddOpenApi();

// 注册数据库上下文 (SQLite)
var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册配置映射
builder.Services.Configure<LLMConfig>(builder.Configuration.GetSection("LLMConfig"));

// 注册 HTTP 客户端（用于 LLMService）
builder.Services.AddHttpClient<ILLMService, LLMService>();

// 注册业务逻辑服务 (Scoped)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDialogueService, DialogueService>();
// 注意：LLMService 已通过 AddHttpClient 注册

// 配置 CORS (允许前端跨域访问)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// --- 2. 配置中间件管道 ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 启用跨域
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// 映射控制器路由
app.MapControllers();

app.Run();
