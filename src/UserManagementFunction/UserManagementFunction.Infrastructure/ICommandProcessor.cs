using Telegram.Bot.Types;

namespace UserManagementFunction.Infrastructure;
public interface ICommandProcessor
{
    Task HandleCommand(Message message);
}
