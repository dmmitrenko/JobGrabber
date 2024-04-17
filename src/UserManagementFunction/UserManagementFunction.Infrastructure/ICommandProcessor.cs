using Telegram.Bot.Types;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Infrastructure.Models;

namespace UserManagementFunction.Infrastructure;
public interface ICommandProcessor
{
    Task<CommandResult> HandleCommand(Message message, Commands command, CancellationToken cancellationToken = default);
}
