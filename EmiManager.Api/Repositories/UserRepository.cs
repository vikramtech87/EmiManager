using EmiManager.Api.Data;
using EmiManager.Api.Repositories.Contracts;
using EmiManager.Domain.Models;

using MongoDB.Driver;

namespace EmiManager.Api.Repositories;
public class UserRepository : IUserRepository {
    private readonly IMongoCollection<User> _userCollection;

    public UserRepository(MongoDbContext dbContext) {
        _userCollection = dbContext.Db.GetCollection<User>("Users");
    }

    public async Task CreateUser(User user) {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task<User?> GetUserByEmail(string email) {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _userCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<string> GetVerificationCodeForUser(User user) {
        if(user.VerificationCode is not null) {
            return user.VerificationCode;
        }

        user.VerificationCode = Guid.NewGuid().ToString();
        await UpdateUser(user);

        return user.VerificationCode;
    }

    public async Task<bool> VerifyEmail(User user, string code) {
        if(user.VerificationCode == code) {
            user.EmailVerifiedAt = DateTime.UtcNow;
            user.VerificationCode = null;
            await UpdateUser(user);
            return true;
        }

        return false;
    }

    private async Task UpdateUser(User user) {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        await _userCollection.ReplaceOneAsync(filter, user);
    }
}
