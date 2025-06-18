using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class ChannelInstructionMappers
{
    public static ChannelInstructionElement ToEntity(this ChannelInstruction model)
    {
        return new ChannelInstructionElement
        {
            Channel = model.Channel,
            Instruction = model.Instruction
        };
    }

    public static ChannelInstruction ToModel(this ChannelInstructionElement model)
    {
        return new ChannelInstruction
        {
            Channel = model.Channel,
            Instruction = model.Instruction
        };
    }
}
