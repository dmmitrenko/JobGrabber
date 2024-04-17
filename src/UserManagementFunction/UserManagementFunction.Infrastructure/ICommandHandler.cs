using Telegram.Bot.Types;
using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Infrastructure.Models;

namespace UserManagementFunction.Infrastructure;
public interface ICommandHandler
{
    Commands CommandKey { get; }
    Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}
