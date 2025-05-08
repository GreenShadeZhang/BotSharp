using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Users.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public User? GetUserByEmail(string email)
    {
        var user = _dc.Users.FindOne(x => x.Email == email.ToLower());
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserByPhone(string phone, string type = UserType.Client, string regionCode = "CN")
    {
        string phoneSecond = string.Empty;
        // if phone number length is less than 4, return null
        if (string.IsNullOrWhiteSpace(phone) || phone?.Length < 4)
        {
            return null;
        }

        if (regionCode == "CN")
        {
            phoneSecond = (phone ?? "").StartsWith("+86") ? (phone ?? "").Replace("+86", "") : ($"+86{phone ?? ""}");
        }
        else
        {
            phoneSecond = (phone ?? "").Substring(regionCode == "US" ? 2 : 3);
        }

        var user = _dc.Users.FindOne(x => (x.Phone == phone || x.Phone == phoneSecond)
        && (x.RegionCode == regionCode || string.IsNullOrWhiteSpace(x.RegionCode))
        && (x.Type == type));
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserByPhoneV2(string phone, string source = UserType.Internal, string regionCode = "CN")
    {
        string phoneSecond = string.Empty;
        // if phone number length is less than 4, return null
        if (string.IsNullOrWhiteSpace(phone) || phone?.Length < 4)
        {
            return null;
        }

        if (regionCode == "CN")
        {
            phoneSecond = (phone ?? "").StartsWith("+86") ? (phone ?? "").Replace("+86", "") : ($"+86{phone ?? ""}");
        }
        else
        {
            phoneSecond = (phone ?? "").Substring(regionCode == "US" ? 2 : 3);
        }

        var user = _dc.Users.FindOne(x => (x.Phone == phone || x.Phone == phoneSecond)
        && (x.RegionCode == regionCode || string.IsNullOrWhiteSpace(x.RegionCode))
        && (x.Source == source));
        return user != null ? user.ToUser() : null;
    }

    public User? GetAffiliateUserByPhone(string phone)
    {
        var user = _dc.Users.FindOne(x => x.Phone == phone && x.Type == UserType.Affiliate);
        return user != null ? user.ToUser() : null;
    }

    public User? GetUserById(string id)
    {
        var user = _dc.Users.FindOne(x => x.Id == id || (x.ExternalId != null && x.ExternalId == id));
        return user != null ? user.ToUser() : null;
    }

    public List<User> GetUserByIds(List<string> ids)
    {
        var users = _dc.Users.Query().Where(x => ids.Contains(x.Id) || (x.ExternalId != null && ids.Contains(x.ExternalId))).ToList();
        return users?.Any() == true ? users.Select(x => x.ToUser()).ToList() : new List<User>();
    }

    public List<User> GetUsersByAffiliateId(string affiliateId)
    {
        var users = _dc.Users.Query().Where(x => x.AffiliateId == affiliateId).ToList();
        return users?.Any() == true ? users.Select(x => x.ToUser()).ToList() : new List<User>();
    }

    public User? GetUserByUserName(string userName)
    {
        var user = _dc.Users.FindOne(x => x.UserName == userName.ToLower());
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
            RegionCode = user.RegionCode,
            AffiliateId = user.AffiliateId,
            EmployeeId = user.EmployeeId,
            IsDisabled = user.IsDisabled,
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        };

        _dc.Users.Insert(userCollection);
    }

    public void UpdateExistUser(string userId, User user)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();

        if (userDoc != null)
        {
            userDoc.Email = user.Email;
            userDoc.Phone = user.Phone;
            userDoc.Salt = user.Salt;
            userDoc.Password = user.Password;
            userDoc.VerificationCode = user.VerificationCode;
            userDoc.UpdatedTime = DateTime.UtcNow;
            userDoc.RegionCode = user.RegionCode;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserName(string userId, string userName)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.UserName = userName;
            userDoc.UpdatedTime = DateTime.UtcNow;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserVerified(string userId)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.Verified = true;
            userDoc.UpdatedTime = DateTime.UtcNow;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserVerificationCode(string userId, string verficationCode)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.VerificationCode = verficationCode;
            userDoc.VerificationCodeExpireAt = DateTime.UtcNow.AddMinutes(5);
            userDoc.UpdatedTime = DateTime.UtcNow;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserPassword(string userId, string password)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.Password = password;
            userDoc.UpdatedTime = DateTime.UtcNow;
            userDoc.Verified = true;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserEmail(string userId, string email)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.Email = email;
            userDoc.UpdatedTime = DateTime.UtcNow;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserPhone(string userId, string phone, string regionCode)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.Phone = phone;
            userDoc.UpdatedTime = DateTime.UtcNow;
            userDoc.RegionCode = regionCode;
            userDoc.FirstName = phone;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUserIsDisable(string userId, bool isDisable)
    {
        var userDoc = _dc.Users.Query().Where(x => x.Id == userId).FirstOrDefault();
        if (userDoc != null)
        {
            userDoc.IsDisabled = isDisable;
            userDoc.UpdatedTime = DateTime.UtcNow;
            _dc.Users.Update(userDoc);
        }
    }

    public void UpdateUsersIsDisable(List<string> userIds, bool isDisable)
    {
        foreach (var userId in userIds)
        {
            UpdateUserIsDisable(userId, isDisable);
        }
    }

    public PagedItems<User> GetUsers(UserFilter filter)
    {
        if (filter == null)
        {
            filter = UserFilter.Empty();
        }

        var query = _dc.Users.Query();

        if (filter?.UserIds != null)
        {
            query = query.Where(x => filter.UserIds.Contains(x.Id));
        }
        if (filter?.UserNames != null)
        {
            query = query.Where(x => filter.UserNames.Contains(x.UserName));
        }
        if (filter?.ExternalIds != null)
        {
            query = query.Where(x => filter.ExternalIds.Contains(x.ExternalId));
        }
        if (filter?.Roles != null)
        {
            query = query.Where(x => filter.Roles.Contains(x.Role));
        }
        if (filter?.Types != null)
        {
            query = query.Where(x => filter.Types.Contains(x.Type));
        }
        if (filter?.Sources != null)
        {
            query = query.Where(x => filter.Sources.Contains(x.Source));
        }

        var total = query.Count();
        var items = query.OrderByDescending(x => x.CreatedTime)
                         .Skip(filter.Offset)
                         .Limit(filter.Size)
                         .ToList()
                         .Select(x => x.ToUser())
                         .ToList();

        return new PagedItems<User>
        {
            Items = items,
            Count = total
        };
    }

    public List<User> SearchLoginUsers(User filter, string source = UserSource.Internal)
    {
        List<User> searchResult = new List<User>();

        // search by filters
        if (!string.IsNullOrWhiteSpace(filter.Id))
        {
            var curUser = _dc.Users.FindOne(x => x.Source == source && x.Id == filter.Id.ToLower());
            User user = curUser != null ? curUser.ToUser() : null;
            if (user != null)
            {
                searchResult.Add(user);
            }
        }
        else if (!string.IsNullOrWhiteSpace(filter.Phone) && !string.IsNullOrWhiteSpace(filter.RegionCode))
        {
            string[] regionCodeData = filter.RegionCode.Split('|');
            if (regionCodeData.Length == 2)
            {
                string phoneNoCallingCode = filter.Phone;
                string phoneWithCallingCode = filter.Phone;
                if (!filter.Phone.StartsWith('+'))
                {
                    phoneNoCallingCode = filter.Phone;
                    phoneWithCallingCode = $"{regionCodeData[1]}{filter.Phone}";
                }
                else
                {
                    phoneNoCallingCode = filter.Phone.Replace(regionCodeData[1], "");
                }
                var phoneUsers = _dc.Users.Query()
                                          .Where(x => x.Source == source && (x.Phone == phoneNoCallingCode || x.Phone == phoneWithCallingCode) && x.RegionCode == regionCodeData[0])
                                          .ToList();

                if (phoneUsers != null && phoneUsers.Count > 0)
                {
                    foreach (var user in phoneUsers)
                    {
                        if (user != null)
                        {
                            searchResult.Add(user.ToUser());
                        }
                    }
                }

            }
        }
        else if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            var curUser = _dc.Users.FindOne(x => x.Source == source && x.Email == filter.Email.ToLower());
            User user = curUser != null ? curUser.ToUser() : null;
            if (user != null)
            {
                searchResult.Add(user);
            }
        }


        if (searchResult.Count == 0 && !string.IsNullOrWhiteSpace(filter.UserName))
        {
            var curUser = _dc.Users.FindOne(x => x.Source == source && x.UserName == filter.UserName);
            User user = curUser != null ? curUser.ToUser() : null;
            if (user != null)
            {
                searchResult.Add(user);
            }
        }

        return searchResult;
    }

    public User? GetUserDetails(string userId, bool includeAgent = false)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;

        var userDoc = _dc.Users.FindOne(x => x.Id == userId || x.ExternalId == userId);
        if (userDoc == null) return null;

        var agentActions = new List<UserAgentAction>();
        var user = userDoc.ToUser();
        var userAgentsDocs = _dc.UserAgents.Query().Where(x => x.UserId == userId).ToList();

        var userAgents = userAgentsDocs.Select(x => new UserAgent
        {
            Id = x.Id,
            UserId = x.UserId,
            AgentId = x.AgentId,
            Actions = x.Actions ?? Enumerable.Empty<string>()
        }).ToList();

        if (!includeAgent)
        {
            agentActions = userAgents.Select(x => new UserAgentAction
            {
                Id = x.Id,
                AgentId = x.AgentId,
                Actions = x.Actions
            }).ToList();
            user.AgentActions = agentActions;
            return user;
        }

        var agentIds = userAgents.Select(x => x.AgentId)?.Distinct().ToList();
        if (!agentIds.IsNullOrEmpty())
        {
            var agents = GetAgents(new AgentFilter { AgentIds = agentIds });

            foreach (var item in userAgents)
            {
                var found = agents.FirstOrDefault(x => x.Id == item.AgentId);
                if (found == null) continue;

                agentActions.Add(new UserAgentAction
                {
                    Id = item.Id,
                    AgentId = found.Id,
                    Agent = found,
                    Actions = item.Actions
                });
            }
        }

        user.AgentActions = agentActions;
        return user;
    }

    public bool UpdateUser(User user, bool updateUserAgents = false)
    {
        if (string.IsNullOrEmpty(user?.Id)) return false;

        var userDoc = _dc.Users.FindById(user.Id);
        if (userDoc == null) return false;

        userDoc.Type = user.Type;
        userDoc.Role = user.Role;
        userDoc.Permissions = user.Permissions;
        userDoc.UpdatedTime = DateTime.UtcNow;

        _dc.Users.Update(userDoc);

        if (updateUserAgents)
        {
            var userAgentDocs = user.AgentActions?.Select(x => new UserAgentDocument
            {
                Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                UserId = user.Id,
                AgentId = x.AgentId,
                Actions = x.Actions,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow
            })?.ToList() ?? new List<UserAgentDocument>();

            var existingUserAgents = _dc.UserAgents.Find(x => x.UserId == user.Id).ToList();
            var toDelete = existingUserAgents.Where(x => !userAgentDocs.Any(ua => ua.Id == x.Id)).ToList();

            foreach (var doc in toDelete)
            {
                _dc.UserAgents.Delete(doc.Id);
            }

            foreach (var doc in userAgentDocs)
            {
                _dc.UserAgents.Upsert(doc);
            }
        }

        return true;
    }

    public Dashboard? GetDashboard(string userId = null)
    {
        return null;
    }

    public void AddDashboardConversation(string userId, string conversationId)
    {
        var user = _dc.Users.FindById(userId) ?? _dc.Users.FindOne(x => x.ExternalId == userId);
        if (user == null) return;
        var curDash = user.Dashboard ?? new Dashboard();
        curDash.ConversationList.Add(new DashboardConversation
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversationId
        });

        user.Dashboard = curDash;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void RemoveDashboardConversation(string userId, string conversationId)
    {
        var user = _dc.Users.FindById(userId) ?? _dc.Users.FindOne(x => x.ExternalId == userId);
        if (user == null || user.Dashboard == null || user.Dashboard.ConversationList.IsNullOrEmpty()) return;
        var curDash = user.Dashboard;
        var unpinConv = user.Dashboard.ConversationList.FirstOrDefault(
            x => string.Equals(x.ConversationId, conversationId, StringComparison.OrdinalIgnoreCase));
        if (unpinConv == null) return;
        curDash.ConversationList.Remove(unpinConv);

        user.Dashboard = curDash;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }

    public void UpdateDashboardConversation(string userId, DashboardConversation dashConv)
    {
        var user = _dc.Users.FindById(userId) ?? _dc.Users.FindOne(x => x.ExternalId == userId);
        if (user == null || user.Dashboard == null || user.Dashboard.ConversationList.IsNullOrEmpty()) return;
        var curIdx = user.Dashboard.ConversationList.ToList().FindIndex(
            x => string.Equals(x.ConversationId, dashConv.ConversationId, StringComparison.OrdinalIgnoreCase));
        if (curIdx < 0) return;

        user.Dashboard.ConversationList[curIdx] = dashConv;
        user.UpdatedTime = DateTime.UtcNow;
        _dc.Users.Update(user);
    }
}
