using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Users.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public User? GetUserByEmail(string email)
    {
        var user = _dc.Users.Query().Where(x => x.Email == email.ToLower()).FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserByPhone(string phone)
    {
        var user = _dc.Users.Query().Where(x => x.Phone == phone).FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public User? GetAffiliateUserByPhone(string phone)
    {
        var user = _dc.Users.Query().Where(x => x.Phone == phone && x.Type == UserType.Affiliate).FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserById(string id)
    {
        var user = _dc.Users.Query().Where(x => x.Id == id || (x.ExternalId != null && x.ExternalId == id))
            .FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public List<User> GetUserByIds(List<string> ids)
    {
        var users = _dc.Users.Query()
            .Where(x => ids.Contains(x.Id) || (x.ExternalId != null && ids.Contains(x.ExternalId))).ToList();
        return users?.Any() == true ? users.Select(x => x.ToUser()).ToList() : new List<User>();
    }

    public User? GetUserByAffiliateId(string affiliateId)
    {
        var user = _dc.Users.Query().Where(x => x.AffiliateId == affiliateId)
            .FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserByUserName(string userName)
    {
        var user = _dc.Users.Query().Where(x => x.UserName == userName.ToLower()).FirstOrDefault();
        return user != null ? user.ToUser() : null;
    }

    public void CreateUser(User user)
    {
        if (user == null) return;

        var userCollection = new UserDocument
        {
            Id = user.Id ?? Guid.NewGuid().ToString(),
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Salt = user.Salt,
            Password = user.Password,
            Email = user.Email,
            Phone = user.Phone,
            Source = user.Source,
            ExternalId = user.ExternalId,
            Role = user.Role,
            Type = user.Type,
            VerificationCode = user.VerificationCode,
            Verified = user.Verified,
            AffiliateId = user.AffiliateId,
            IsDisabled = user.IsDisabled,
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        };

        _dc.Users.Insert(userCollection);
    }

    public void UpdateUserVerified(string userId)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();

        if (user == null) return;

        user.Verified = true;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUserVerificationCode(string userId, string verficationCode)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (user == null) return;
        user.VerificationCode = verficationCode;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUserPassword(string userId, string password)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (user == null) return;

        user.Password = password;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUserEmail(string userId, string email)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (user == null) return;
        user.Email = email;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUserPhone(string userId, string phone)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (user == null) return;
        user.Phone = phone;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUserIsDisable(string userId, bool isDisable)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (user == null) return;
        user.IsDisabled = isDisable;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateUsersIsDisable(List<string> userIds, bool isDisable)
    {
        foreach (var userId in userIds)
        {
            UpdateUserIsDisable(userId, isDisable);
        }
    }
}
