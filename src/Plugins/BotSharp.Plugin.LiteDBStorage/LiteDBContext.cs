using LiteDB;
using System.Globalization;

namespace BotSharp.Plugin.LiteDBStorage;

public class LiteDBContext: IDisposable
{
    private readonly LiteDatabase _liteDBClient;
    private readonly string _collectionPrefix;

    private const string DB_NAME_INDEX = "authSource";

    public LiteDBContext(BotSharpDatabaseSettings dbSettings)
    {
        var mapper = new BsonMapper();
        mapper.RegisterType<DateTime>(
            value => value.ToString("o", CultureInfo.InvariantCulture),
            bson => DateTime.ParseExact
            (bson, "o", CultureInfo.InvariantCulture, 
            DateTimeStyles.RoundtripKind));

        mapper.RegisterType<DateTimeOffset>(
            value => value.ToString("o", CultureInfo.InvariantCulture),
            bson => DateTimeOffset.ParseExact
            (bson, "o", CultureInfo.InvariantCulture, 
            DateTimeStyles.RoundtripKind));

        var dbConnectionString = dbSettings.BotSharpLiteDB;
        var connectionString = new ConnectionString(dbConnectionString)
        {
            Connection = ConnectionType.Shared
        };
        _liteDBClient = new LiteDatabase(connectionString, mapper);

        _collectionPrefix = dbSettings.TablePrefix.IfNullOrEmptyAs("BotSharp");
    }

    private LiteDatabase Database { get { return _liteDBClient; } }

    #region Indexes
    private ILiteCollection<ConversationDocument> CreateConversationIndex()
    {
        var collection = Database.GetCollection<ConversationDocument>($"{_collectionPrefix}_Conversations");
        collection.EnsureIndex(x => x.CreatedTime);
        return collection;
    }

    private ILiteCollection<ConversationStateDocument> CreateConversationStateIndex()
    {
        var collection = Database.GetCollection<ConversationStateDocument>($"{_collectionPrefix}_ConversationStates");
        return collection;
    }

    private ILiteCollection<AgentTaskDocument> CreateAgentTaskIndex()
    {
        var collection = Database.GetCollection<AgentTaskDocument>($"{_collectionPrefix}_AgentTasks");

        collection.EnsureIndex(x => x.CreatedTime);

        return collection;
    }

    private ILiteCollection<ConversationContentLogDocument> CreateContentLogIndex()
    {
        var collection = Database.GetCollection<ConversationContentLogDocument>($"{_collectionPrefix}_ConversationContentLogs");
        collection.EnsureIndex(x => x.CreateTime);
        return collection;
    }

    private ILiteCollection<ConversationStateLogDocument> CreateStateLogIndex()
    {
        var collection = Database.GetCollection<ConversationStateLogDocument>($"{_collectionPrefix}_ConversationStateLogs");
        collection.EnsureIndex(x => x.CreateTime);
        return collection;
    }

    public void Dispose()
    {
        _liteDBClient?.Dispose();
    }
    #endregion

    public ILiteCollection<AgentDocument> Agents
        => Database.GetCollection<AgentDocument>($"{_collectionPrefix}_Agents");

    public ILiteCollection<AgentTaskDocument> AgentTasks
        => CreateAgentTaskIndex();

    public ILiteCollection<ConversationDocument> Conversations
        => CreateConversationIndex();

    public ILiteCollection<ConversationDialogDocument> ConversationDialogs
        => Database.GetCollection<ConversationDialogDocument>($"{_collectionPrefix}_ConversationDialogs");

    public ILiteCollection<ConversationStateDocument> ConversationStates
        => CreateConversationStateIndex();

    public ILiteCollection<ExecutionLogDocument> ExectionLogs
        => Database.GetCollection<ExecutionLogDocument>($"{_collectionPrefix}_ExecutionLogs");

    public ILiteCollection<LlmCompletionLogDocument> LlmCompletionLogs
        => Database.GetCollection<LlmCompletionLogDocument>($"{_collectionPrefix}_LlmCompletionLogs");

    public ILiteCollection<ConversationContentLogDocument> ContentLogs
        => CreateContentLogIndex();

    public ILiteCollection<ConversationStateLogDocument> StateLogs
        => CreateStateLogIndex();

    public ILiteCollection<UserDocument> Users
        => Database.GetCollection<UserDocument>($"{_collectionPrefix}_Users");

    public ILiteCollection<UserAgentDocument> UserAgents
        => Database.GetCollection<UserAgentDocument>($"{_collectionPrefix}_UserAgents");

    public ILiteCollection<PluginDocument> Plugins
        => Database.GetCollection<PluginDocument>($"{_collectionPrefix}_Plugins");

    public ILiteCollection<TranslationMemoryDocument> TranslationMemories
        => Database.GetCollection<TranslationMemoryDocument>($"{_collectionPrefix}_TranslationMemories");

    public ILiteCollection<KnowledgeCollectionConfigDocument> KnowledgeCollectionConfigs
        => Database.GetCollection<KnowledgeCollectionConfigDocument>($"{_collectionPrefix}_KnowledgeCollectionConfigs");

    public ILiteCollection<KnowledgeCollectionFileMetaDocument> KnowledgeCollectionFileMeta
        => Database.GetCollection<KnowledgeCollectionFileMetaDocument>($"{_collectionPrefix}_KnowledgeCollectionFileMeta");
    public ILiteCollection<RoleDocument> Roles
    => Database.GetCollection<RoleDocument>($"{_collectionPrefix}_Roles");

    public ILiteCollection<RoleAgentDocument> RoleAgents
        => Database.GetCollection<RoleAgentDocument>($"{_collectionPrefix}_RoleAgents");

    public ILiteCollection<CrontabItemDocument> CrontabItems
        => Database.GetCollection<CrontabItemDocument>($"{_collectionPrefix}_CronTabItems");

    public ILiteCollection<GlobalStatisticsDocument> GlobalStatistics
        => Database.GetCollection<GlobalStatisticsDocument>($"{_collectionPrefix}_GlobalStatistics");
}
