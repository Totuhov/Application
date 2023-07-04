﻿

namespace Application.Services;

using Application.Data;
using Application.Data.Models;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }    
    public async Task<bool> IsUserExists(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName);
    }

    public async Task<string?> GetUsernameByIdAsync(string userId)
    {
        ApplicationUser? user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return null;
        }
        return user.UserName;
    }

    public async Task<string?> GetUserEmailByUsernameAsync(string userName)
    {
        ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user != null)
        {
            return user.Email;
        }

        return null;
    }
}
