using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace EmiManager.Domain.Models; 
public class User {
    public ObjectId Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? EmailVerifiedAt { get; set; }
    public string? VerificationCode { get; set; }

    public bool IsEmailVerified => EmailVerifiedAt != null;
}
