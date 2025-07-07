using BotSharp.Core.Infrastructures;

namespace BotSharp.Plugin.ChatHub.Hooks
{
    public class CustomConversationHook(IServiceProvider services) : ConversationHookBase
    {
        private const string CONVERSATION_SUMMARY_AGENT_ID = "b3561753-0753-4598-9248-8f66553f784d";

        private readonly IServiceProvider _services = services;

        public override async Task OnResponseGenerated(RoleDialogModel message)
        {
            var convService = _services.GetRequiredService<IConversationService>();

            var conv = await convService.GetConversation(convService.ConversationId, false);

            if (conv.Title == "New Conversation" && message.Role == "assistant")
            {
                var firstMsg = convService.GetDialogHistory(1, false).FirstOrDefault(x => x.Role == "user");
                if (firstMsg != null)
                {
                    var agentSettings = _services.GetRequiredService<AgentSettings>();
                    var provider = agentSettings.LlmConfig.Provider;
                    var model = agentSettings.LlmConfig.Model;
                    var chatCompletion = CompletionProvider.GetChatCompletion(_services, provider, model);
                    var response = await chatCompletion.GetChatCompletions(new Agent
                    {
                        Id = CONVERSATION_SUMMARY_AGENT_ID,
                        Name = "会话标题总结智能体",
                        Instruction = "你是一个会话标题总结的智能体,一句话总结内容作为会话标题"
                    },
                    [
                        firstMsg,
                        message,
                        new RoleDialogModel(AgentRole.User, "请总结当前的会话，作为会话的标题使用")
                    ]);

                    await convService.UpdateConversationTitle(convService.ConversationId, response.Content);
                }
            }

            await base.OnResponseGenerated(message);
        }
    }
}
