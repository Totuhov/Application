
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
    /// <returns>userName, null if not found</returns>
    Task<string?> GetUsernameByIdAsync(string userId);

    Task<string?> GetUserEmailByUsernameAsync(string userName);
}
