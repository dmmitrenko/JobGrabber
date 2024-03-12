using MediatR;
using UserManagementFunction.Domain.Models;

namespace UserManagementFunction.Application.Commands;
public class GetSubscriptionsCommand : IRequest<List<Subscription>>
{
    public long UserId { get; set; }
}
