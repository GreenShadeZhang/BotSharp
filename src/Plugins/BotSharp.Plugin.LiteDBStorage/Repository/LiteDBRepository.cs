using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Users.Models;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository : IBotSharpRepository
{
    private readonly LiteDBContext _dc;
    private readonly IServiceProvider _services;
    private readonly ILogger<LiteDBRepository> _logger;

    public LiteDBRepository(
        LiteDBContext dc,
        IServiceProvider services,
        ILogger<LiteDBRepository> logger)
    {
        _dc = dc;
        _services = services;
        _logger = logger;
    }

    private List<Agent> _agents = new List<Agent>();
    private List<User> _users = new List<User>();
    private List<UserAgent> _userAgents = new List<UserAgent>();
    private List<Conversation> _conversations = new List<Conversation>();
    List<string> _changedTableNames = new List<string>();    
}
