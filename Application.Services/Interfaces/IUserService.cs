
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Application.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Checks if there is a user with this username
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>true if there is a user with username in the database</returns>
    Task<bool> IsUserExists(string userName);

    /// <summary>
    /// Return userName by id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>userName</returns>
    Task<string> GetUsernameByIdAsync(string userId);

    /// <summary>
    /// Returns user's email by username
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>

    Task<string> GetUserEmailByUsernameAsync(string userName);
}
