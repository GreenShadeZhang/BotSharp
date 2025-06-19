using BotSharp.Abstraction.Users.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class User
{
    public string Id { get; set; }
    public string UserName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Salt { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Source { get; set; } = "internal";
    public string? ExternalId { get; set; }
    public string Type { get; set; } = "client";
    public string Role { get; set; } = null!;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpireAt { get; set; }
    public bool Verified { get; set; }
    public string? RegionCode { get; set; }
    public string? AffiliateId { get; set; }
    public string? EmployeeId { get; set; }
    public bool IsDisabled { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
    public Dashboard? Dashboard { get; set; }
}
