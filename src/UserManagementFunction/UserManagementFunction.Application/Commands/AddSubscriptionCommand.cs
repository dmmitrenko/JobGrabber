using MediatR;
using UserManagementFunction.Domain.Models;

namespace UserManagementFunction.Application.Commands;
public class AddSubscriptionCommand : IRequest
{
    public Subscription Subscription { get; set; }
}
