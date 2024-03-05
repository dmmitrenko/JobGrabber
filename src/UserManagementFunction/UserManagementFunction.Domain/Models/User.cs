namespace UserManagementFunction.Domain.Models;
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public List<Subscription> Subscriptions { get; set; }
}
