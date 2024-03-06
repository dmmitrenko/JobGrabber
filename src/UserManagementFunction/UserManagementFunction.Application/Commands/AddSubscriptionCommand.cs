using MediatR;
using Telegram.Bot.Types;

namespace UserManagementFunction.Application.Commands;
public class AddSubscriptionCommand : IRequest
{
    public Message Message { get; set; }
}
