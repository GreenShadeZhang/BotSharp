using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class ChannelInstructionLiteDBElement
{
    public string Channel { get; set; }
    public string Instruction { get; set; }

    public static ChannelInstructionLiteDBElement ToLiteDBElement(ChannelInstruction instruction)
    {
        return new ChannelInstructionLiteDBElement
        {
            Channel = instruction.Channel,
            Instruction = instruction.Instruction
        };
    }

    public static ChannelInstruction ToDomainElement(ChannelInstructionLiteDBElement instruction)
    {
        return new ChannelInstruction
        {
            Channel = instruction.Channel,
            Instruction = instruction.Instruction
        };
    }
}
