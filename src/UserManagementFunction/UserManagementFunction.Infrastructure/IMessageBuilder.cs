using UserManagementFunction.Domain.Enums;
using UserManagementFunction.Infrastructure.Models;

namespace UserManagementFunction.Infrastructure;
public interface IMessageBuilder
{
    string GetResponseMessage(CommandResult commandResult);
    string GetCommandHelp(Commands command);
}
