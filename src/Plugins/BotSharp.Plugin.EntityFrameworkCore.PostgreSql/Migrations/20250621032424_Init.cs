using System;
using System.Collections.Generic;
using System.Text.Json;
using BotSharp.Abstraction.Users.Models;
using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotSharp.Plugin.EntityFrameworkCore.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotSharp_Agents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: true),
                    InheritAgentId = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    Instruction = table.Column<string>(type: "text", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    MergeUtility = table.Column<bool>(type: "boolean", nullable: false),
                    MaxMessageCount = table.Column<int>(type: "integer", nullable: true),
                    ChannelInstructions = table.Column<List<ChannelInstructionElement>>(type: "json", nullable: false),
                    Templates = table.Column<List<AgentTemplateElement>>(type: "json", nullable: false),
                    Functions = table.Column<List<FunctionDefElement>>(type: "json", nullable: false),
                    Responses = table.Column<List<AgentResponseElement>>(type: "json", nullable: false),
                    Samples = table.Column<List<string>>(type: "json", nullable: false),
                    Utilities = table.Column<List<AgentUtilityElement>>(type: "json", nullable: false),
                    McpTools = table.Column<List<McpToolElement>>(type: "json", nullable: false),
                    KnowledgeBases = table.Column<List<AgentKnowledgeBaseElement>>(type: "json", nullable: false),
                    Profiles = table.Column<List<string>>(type: "json", nullable: false),
                    Labels = table.Column<List<string>>(type: "json", nullable: false),
                    RoutingRules = table.Column<List<RoutingRuleElement>>(type: "json", nullable: false),
                    Rules = table.Column<List<AgentRuleElement>>(type: "json", nullable: false),
                    LlmConfig = table.Column<AgentLlmConfigElement>(type: "json", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_AgentTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    AgentId = table.Column<string>(type: "text", nullable: false),
                    DirectAgentId = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_AgentTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_ConversationContentLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MessageId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    AgentId = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_ConversationContentLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_ConversationDialogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AgentId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetaData = table.Column<DialogMetaDataElement>(type: "json", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    SecondaryContent = table.Column<string>(type: "text", nullable: true),
                    RichContent = table.Column<string>(type: "text", nullable: true),
                    SecondaryRichContent = table.Column<string>(type: "text", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_ConversationDialogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_Conversations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AgentId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    TitleAlias = table.Column<string>(type: "text", nullable: false),
                    Channel = table.Column<string>(type: "text", nullable: false),
                    ChannelId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DialogCount = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<List<string>>(type: "json", nullable: false),
                    LatestStates = table.Column<Dictionary<string, JsonDocument>>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_ConversationStateLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MessageId = table.Column<string>(type: "text", nullable: false),
                    States = table.Column<string>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_ConversationStateLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_ConversationStates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AgentId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    States = table.Column<List<State>>(type: "json", nullable: false),
                    Breakpoints = table.Column<List<BreakpointInfoElement>>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_ConversationStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_CrontabItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    AgentId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    ExecutionResult = table.Column<string>(type: "text", nullable: false),
                    Cron = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ExecutionCount = table.Column<int>(type: "integer", nullable: false),
                    MaxExecutionCount = table.Column<int>(type: "integer", nullable: false),
                    ExpireSeconds = table.Column<int>(type: "integer", nullable: false),
                    LastExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LessThan60Seconds = table.Column<bool>(type: "boolean", nullable: false),
                    Tasks = table.Column<IEnumerable<CronTaskElement>>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_CrontabItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_ExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Logs = table.Column<List<string>>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_ExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_GlobalStat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    AgentId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Count = table.Column<string>(type: "TEXT", nullable: false),
                    LlmCost = table.Column<string>(type: "TEXT", nullable: false),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Interval = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_GlobalStat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_InstructionLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    AgentId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
                    UserMessage = table.Column<string>(type: "text", nullable: false),
                    SystemInstruction = table.Column<string>(type: "text", nullable: true),
                    CompletionText = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    States = table.Column<Dictionary<string, JsonDocument>>(type: "json", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_InstructionLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_KnowledgeCollectionConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VectorStore = table.Column<string>(type: "TEXT", nullable: false),
                    TextEmbedding = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_KnowledgeCollectionConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_KnowledgeDocument",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Collection = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    FileSource = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    VectorStoreProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VectorDataIds = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_KnowledgeDocument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_LlmCompletionLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConversationId = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<string>(type: "text", nullable: false),
                    AgentId = table.Column<string>(type: "text", nullable: false),
                    Prompt = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_LlmCompletionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_Plugins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EnabledPlugins = table.Column<List<string>>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Permissions = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_TranslationMemorys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OriginalText = table.Column<string>(type: "text", nullable: false),
                    HashText = table.Column<string>(type: "text", nullable: false),
                    Translations = table.Column<List<TranslationMemoryElement>>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_TranslationMemorys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_UserAgents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AgentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Actions = table.Column<List<string>>(type: "json", nullable: false),
                    Editable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_UserAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Salt = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    VerificationCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    VerificationCodeExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    RegionCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AffiliateId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EmployeeId = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Permissions = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Dashboard = table.Column<Dashboard>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotSharp_RoleAgent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    RoleId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    AgentId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSharp_RoleAgent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BotSharp_RoleAgent_BotSharp_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "BotSharp_Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotSharp_RoleAgent_BotSharp_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "BotSharp_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Agents_Id",
                table: "BotSharp_Agents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_AgentTasks_Id",
                table: "BotSharp_AgentTasks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationContentLogs_ConversationId",
                table: "BotSharp_ConversationContentLogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationContentLogs_CreatedTime",
                table: "BotSharp_ConversationContentLogs",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationContentLogs_Id",
                table: "BotSharp_ConversationContentLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationDialogs_ConversationId",
                table: "BotSharp_ConversationDialogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationDialogs_Id",
                table: "BotSharp_ConversationDialogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Conversations_AgentId",
                table: "BotSharp_Conversations",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Conversations_CreatedTime",
                table: "BotSharp_Conversations",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Conversations_Id",
                table: "BotSharp_Conversations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Conversations_Title",
                table: "BotSharp_Conversations",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Conversations_TitleAlias",
                table: "BotSharp_Conversations",
                column: "TitleAlias");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationStateLogs_ConversationId",
                table: "BotSharp_ConversationStateLogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationStateLogs_CreatedTime",
                table: "BotSharp_ConversationStateLogs",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationStateLogs_Id",
                table: "BotSharp_ConversationStateLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationStates_ConversationId",
                table: "BotSharp_ConversationStates",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ConversationStates_Id",
                table: "BotSharp_ConversationStates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_CrontabItem_AgentId",
                table: "BotSharp_CrontabItem",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_CrontabItem_ConversationId",
                table: "BotSharp_CrontabItem",
                column: "ConversationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_CrontabItem_UserId",
                table: "BotSharp_CrontabItem",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ExecutionLogs_ConversationId",
                table: "BotSharp_ExecutionLogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_ExecutionLogs_Id",
                table: "BotSharp_ExecutionLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_GlobalStat_AgentId",
                table: "BotSharp_GlobalStat",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_GlobalStat_AgentId_StartTime_EndTime",
                table: "BotSharp_GlobalStat",
                columns: new[] { "AgentId", "StartTime", "EndTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_GlobalStat_RecordTime",
                table: "BotSharp_GlobalStat",
                column: "RecordTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_InstructionLog_AgentId",
                table: "BotSharp_InstructionLog",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_InstructionLog_CreatedTime",
                table: "BotSharp_InstructionLog",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeCollectionConfig_Name",
                table: "BotSharp_KnowledgeCollectionConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeCollectionConfig_Type",
                table: "BotSharp_KnowledgeCollectionConfig",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeDocument_Collection",
                table: "BotSharp_KnowledgeDocument",
                column: "Collection");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeDocument_Collection_VectorStoreProvider_F~",
                table: "BotSharp_KnowledgeDocument",
                columns: new[] { "Collection", "VectorStoreProvider", "FileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeDocument_CreateDate",
                table: "BotSharp_KnowledgeDocument",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeDocument_FileId",
                table: "BotSharp_KnowledgeDocument",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_KnowledgeDocument_VectorStoreProvider",
                table: "BotSharp_KnowledgeDocument",
                column: "VectorStoreProvider");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_LlmCompletionLogs_ConversationId",
                table: "BotSharp_LlmCompletionLogs",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_LlmCompletionLogs_Id",
                table: "BotSharp_LlmCompletionLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Plugins_Id",
                table: "BotSharp_Plugins",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Role_Name",
                table: "BotSharp_Role",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_RoleAgent_AgentId",
                table: "BotSharp_RoleAgent",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_RoleAgent_RoleId",
                table: "BotSharp_RoleAgent",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_RoleAgent_RoleId_AgentId",
                table: "BotSharp_RoleAgent",
                columns: new[] { "RoleId", "AgentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_TranslationMemorys_Id",
                table: "BotSharp_TranslationMemorys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_UserAgents_CreatedTime",
                table: "BotSharp_UserAgents",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_UserAgents_Id",
                table: "BotSharp_UserAgents",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Users_CreatedTime",
                table: "BotSharp_Users",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Users_Email",
                table: "BotSharp_Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Users_ExternalId",
                table: "BotSharp_Users",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Users_Phone",
                table: "BotSharp_Users",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_BotSharp_Users_UserName",
                table: "BotSharp_Users",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotSharp_AgentTasks");

            migrationBuilder.DropTable(
                name: "BotSharp_ConversationContentLogs");

            migrationBuilder.DropTable(
                name: "BotSharp_ConversationDialogs");

            migrationBuilder.DropTable(
                name: "BotSharp_Conversations");

            migrationBuilder.DropTable(
                name: "BotSharp_ConversationStateLogs");

            migrationBuilder.DropTable(
                name: "BotSharp_ConversationStates");

            migrationBuilder.DropTable(
                name: "BotSharp_CrontabItem");

            migrationBuilder.DropTable(
                name: "BotSharp_ExecutionLogs");

            migrationBuilder.DropTable(
                name: "BotSharp_GlobalStat");

            migrationBuilder.DropTable(
                name: "BotSharp_InstructionLog");

            migrationBuilder.DropTable(
                name: "BotSharp_KnowledgeCollectionConfig");

            migrationBuilder.DropTable(
                name: "BotSharp_KnowledgeDocument");

            migrationBuilder.DropTable(
                name: "BotSharp_LlmCompletionLogs");

            migrationBuilder.DropTable(
                name: "BotSharp_Plugins");

            migrationBuilder.DropTable(
                name: "BotSharp_RoleAgent");

            migrationBuilder.DropTable(
                name: "BotSharp_TranslationMemorys");

            migrationBuilder.DropTable(
                name: "BotSharp_UserAgents");

            migrationBuilder.DropTable(
                name: "BotSharp_Users");

            migrationBuilder.DropTable(
                name: "BotSharp_Agents");

            migrationBuilder.DropTable(
                name: "BotSharp_Role");
        }
    }
}
