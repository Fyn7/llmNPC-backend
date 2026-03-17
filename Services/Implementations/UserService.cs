using Microsoft.EntityFrameworkCore;
using NPCDialogueServer.Data;
using NPCDialogueServer.Models;
using NPCDialogueServer.Services.Interfaces;

namespace NPCDialogueServer.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetOrCreateUserAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            user = new User { Username = username };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        return user;
    }
}
