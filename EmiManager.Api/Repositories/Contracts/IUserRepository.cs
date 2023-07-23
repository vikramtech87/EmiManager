using EmiManager.Domain.Models;

namespace EmiManager.Api.Repositories.Contracts; 
public interface IUserRepository {
    Task CreateUser(User user);
    Task<User?> GetUserByEmail(string email);
    Task<string> GetVerificationCodeForUser(User user);
    Task<bool> VerifyEmail(User user, string code);
}
