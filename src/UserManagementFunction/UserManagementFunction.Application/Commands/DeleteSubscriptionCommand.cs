using MediatR;

namespace UserManagementFunction.Application.Commands;
public class DeleteSubscriptionCommand : IRequest
{
    public long UserId { get; set; }
    public string SubscriptionTitle { get; set; }
}
