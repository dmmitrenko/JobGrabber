using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.Domain.Models;
public class Subscription
{
    public Guid Id { get; set; }
    public string Keyword { get; set; }
    public double Experience { get; set; }
    public EnglishLevel EnglishLevel { get; set; }
}
