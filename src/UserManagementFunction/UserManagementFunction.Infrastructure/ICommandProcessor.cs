using Telegram.Bot.Types;

namespace UserManagementFunction.Infrastructure;
public interface ICommandProcessor
{
    Task<string> HandleCommand(Message message, CancellationToken cancellationToken = default);
}
