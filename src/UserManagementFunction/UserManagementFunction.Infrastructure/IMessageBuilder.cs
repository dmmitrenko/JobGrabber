using UserManagementFunction.Infrastructure.Models;

namespace UserManagementFunction.Infrastructure;
public interface IMessageBuilder
{
    string GetResponseMessage(CommandResult commandResult);
    string DeleteHelperMessage();
    string AddHelperMessage();
}
