using BotSharp.Abstraction.Users.Models;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;
public partial class EfCoreRepository
{
    public User? GetUserByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email.ToLower());
        return user?.ToModel();
    }

    public User? GetUserById(string id)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == id || (x.ExternalId != null && x.ExternalId == id));
        return user?.ToModel();
    }

    public User? GetUserByUserName(string userName)
    {
        var user = _context.Users.FirstOrDefault(x => x.UserName == userName.ToLower());
        return user?.ToModel();
    }

    public User? GetUserByPhone(string phone, string type = UserType.Client, string regionCode = "CN")
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 4)
        {
            return null;
        }

        string phoneSecond = string.Empty;
        if (regionCode == "CN")
        {
            phoneSecond = phone.StartsWith("+86") ? phone.Replace("+86", "") : $"+86{phone}";
        }
        else
        {
            phoneSecond = phone.Substring(regionCode == "US" ? 2 : 3);
        }

        var user = _context.Users.FirstOrDefault(x => (x.Phone == phone || x.Phone == phoneSecond) &&
                                                    (x.RegionCode == regionCode || string.IsNullOrWhiteSpace(x.RegionCode)) &&
                                                    x.Type == type);
        return user?.ToModel();
    }

    public User? GetUserByPhoneV2(string phone, string source = UserType.Internal, string regionCode = "CN")
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 4)
        {
            return null;
        }

        string phoneSecond = string.Empty;
        if (regionCode == "CN")
        {
            phoneSecond = phone.StartsWith("+86") ? phone.Replace("+86", "") : $"+86{phone}";
        }
        else
        {
            phoneSecond = phone.Substring(regionCode == "US" ? 2 : 3);
        }

        var user = _context.Users.FirstOrDefault(x => (x.Phone == phone || x.Phone == phoneSecond) &&
                                                    (x.RegionCode == regionCode || string.IsNullOrWhiteSpace(x.RegionCode)) &&
                                                    x.Source == source);
        return user?.ToModel();
    }

    public User? GetAffiliateUserByPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;

        var user = _context.Users.FirstOrDefault(x => x.Phone == phone && x.Type == UserType.Affiliate);
        return user?.ToModel();
    }

    public List<User> GetUserByIds(List<string> ids)
    {
        if (ids?.Any() != true) return new List<User>();

        var users = _context.Users.Where(x => ids.Contains(x.Id)).ToList();
        return users.Select(x => x.ToModel()).ToList();
    }

    public List<User> GetUsersByAffiliateId(string affiliateId)
    {
        if (string.IsNullOrWhiteSpace(affiliateId)) return new List<User>();

        var users = _context.Users.Where(x => x.AffiliateId == affiliateId).ToList();
        return users.Select(x => x.ToModel()).ToList();
    }

    public void UpdateUserName(string userId, string userName)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.UserName = userName;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public Dashboard? GetDashboard(string? userId = null)
    {
        return null;
    }

    public void CreateUser(User user)
    {
        if (user == null) return;

        var userEntity = new Entities.User
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
            VerificationCode = user.VerificationCode,
            Verified = user.Verified,
            Type = user.Type,
            RegionCode = user.RegionCode,
            AffiliateId = user.AffiliateId,
            IsDisabled = user.IsDisabled,
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        };

        _context.Users.Add(userEntity);
        _context.SaveChanges();
    }

    public void UpdateExistUser(string userId, User user)
    {
        if (string.IsNullOrWhiteSpace(userId) || user == null) return;

        var existingUser = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (existingUser == null) return;

        existingUser.UserName = user.UserName ?? existingUser.UserName;
        existingUser.FirstName = user.FirstName ?? existingUser.FirstName;
        existingUser.LastName = user.LastName ?? existingUser.LastName;
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.Phone = user.Phone ?? existingUser.Phone;
        existingUser.Role = user.Role ?? existingUser.Role;
        existingUser.Type = user.Type ?? existingUser.Type;
        existingUser.Source = user.Source ?? existingUser.Source;
        existingUser.RegionCode = user.RegionCode ?? existingUser.RegionCode;
        existingUser.AffiliateId = user.AffiliateId ?? existingUser.AffiliateId;
        existingUser.UpdatedTime = DateTime.UtcNow;

        _context.SaveChanges();
    }

    public void UpdateUserVerified(string userId)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.Verified = true;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void AddDashboardConversation(string userId, string conversationId)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId || (x.ExternalId != null && x.ExternalId == userId));
        if (user == null) return;
        if (user.Dashboard == null)
        {
            user.Dashboard = new Dashboard();
        }
        if (user.Dashboard.ConversationList == null)
        {
            user.Dashboard.ConversationList = new List<DashboardConversation>();
        }
        user.Dashboard.ConversationList.Add(new DashboardConversation
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversationId
        });
        user.UpdatedTime = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void RemoveDashboardConversation(string userId, string conversationId)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId || (x.ExternalId != null && x.ExternalId == userId));
        if (user == null || user.Dashboard == null || user.Dashboard.ConversationList == null || !user.Dashboard.ConversationList.Any()) return;
        var unpinConv = user.Dashboard.ConversationList.FirstOrDefault(
            x => string.Equals(x.ConversationId, conversationId, StringComparison.OrdinalIgnoreCase));
        if (unpinConv == null) return;
        user.Dashboard.ConversationList.Remove(unpinConv);
        user.UpdatedTime = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void UpdateDashboardConversation(string userId, DashboardConversation dashConv)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId || (x.ExternalId != null && x.ExternalId == userId));
        if (user == null || user.Dashboard == null || user.Dashboard.ConversationList == null || !user.Dashboard.ConversationList.Any()) return;
        var curIdx = user.Dashboard.ConversationList.ToList().FindIndex(
            x => string.Equals(x.ConversationId, dashConv.ConversationId, StringComparison.OrdinalIgnoreCase));
        if (curIdx < 0) return;
        user.Dashboard.ConversationList[curIdx] = dashConv;
        user.UpdatedTime = DateTime.UtcNow;
        _context.SaveChanges();
    }
    public void UpdateUserVerificationCode(string userId, string verficationCode)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.VerificationCode = verficationCode;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateUserPassword(string userId, string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.Password = password;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateUserEmail(string userId, string email)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.Email = email;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateUserPhone(string userId, string phone)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.Phone = phone;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    public void UpdateUserRole(string userId, string role)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.Role = role;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateUserIsDisable(string userId, bool isDisable)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user != null)
        {
            user.IsDisabled = isDisable;
            user.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateUsersIsDisable(List<string> userIds, bool isDisable)
    {
        if (userIds?.Any() != true) return;

        var users = _context.Users.Where(x => userIds.Contains(x.Id)).ToList();
        foreach (var user in users)
        {
            user.IsDisabled = isDisable;
            user.UpdatedTime = DateTime.UtcNow;
        }
        _context.SaveChanges();
    }

    public PagedItems<User> GetUsers(UserFilter filter)
    {
        var query = _context.Users.AsQueryable();

        if (filter != null)
        {
            if (!filter.UserNames.IsNullOrEmpty())
            {
                foreach (var userName in filter.UserNames)
                {
                    query = query.Where(x => x.UserName.Contains(userName));
                }
            }

            if (!filter.ExternalIds.IsNullOrEmpty())
            {
                foreach (var externalId in filter.ExternalIds)
                {
                    query = query.Where(x => x.ExternalId == externalId);
                }
            }

            if (!filter.Roles.IsNullOrEmpty())
            {
                foreach (var role in filter.Roles)
                {
                    query = query.Where(x => x.Role == role);
                }
            }

            if (!filter.Types.IsNullOrEmpty())
            {
                foreach (var type in filter.Types)
                {
                    query = query.Where(x => x.Type == type);
                }
            }

            if (!filter.Sources.IsNullOrEmpty())
            {
                foreach (var source in filter.Sources)
                {
                    query = query.Where(x => x.Source == source);
                }
            }
        }

        var totalCount = query.Count();
        var users = query.Skip(filter?.Offset ?? 0)
                         .Take(filter?.Size ?? 10)
                         .ToList()
                         .Select(x => x.ToModel())
                         .ToList();

        return new PagedItems<User>
        {
            Items = users,
            Count = totalCount
        };
    }

    public List<User> SearchLoginUsers(User filter, string source = UserSource.Internal)
    {
        var query = _context.Users.Where(x => x.Source == source);

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                query = query.Where(x => x.UserName.Contains(filter.UserName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                query = query.Where(x => x.Email == filter.Email);
            }

            if (!string.IsNullOrWhiteSpace(filter.Phone))
            {
                query = query.Where(x => x.Phone == filter.Phone);
            }
        }

        return query.ToList().Select(x => x.ToModel()).ToList();
    }

    public User? GetUserDetails(string userId, bool includeAgent = false)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;

        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null) return null;

        var result = user.ToModel();

        if (includeAgent)
        {
            var userAgents = _context.UserAgents.Where(x => x.UserId == userId).ToList();
            result.AgentActions = userAgents.Select(x => new UserAgentAction
            {
                Id = x.Id,
                AgentId = x.AgentId,
                Actions = x.Actions,
            }).ToList();
        }

        return result;
    }

    public bool UpdateUser(User user, bool updateUserAgents = false)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.Id)) return false;

        try
        {
            var existingUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser == null) return false;

            existingUser.UserName = user.UserName;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Role = user.Role;
            existingUser.Type = user.Type;
            existingUser.Source = user.Source;
            existingUser.RegionCode = user.RegionCode;
            existingUser.AffiliateId = user.AffiliateId;
            existingUser.IsDisabled = user.IsDisabled;
            existingUser.Verified = user.Verified;
            existingUser.UpdatedTime = DateTime.UtcNow;

            if (updateUserAgents && user.AgentActions?.Any() == true)
            {
                // Remove existing user agents
                var existingUserAgents = _context.UserAgents.Where(x => x.UserId == user.Id);
                _context.UserAgents.RemoveRange(existingUserAgents);

                // Add new user agents
                var newUserAgents = user.AgentActions.Select(x => new Entities.UserAgent
                {
                    Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    AgentId = x.AgentId,
                    Actions = x.Actions.ToList() ?? new List<string>(),
                    CreatedTime = DateTime.UtcNow,
                    UpdatedTime = DateTime.UtcNow
                });

                _context.UserAgents.AddRange(newUserAgents);
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", user.Id);
            return false;
        }
    }
}