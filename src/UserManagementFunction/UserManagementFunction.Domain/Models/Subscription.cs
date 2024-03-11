using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.Domain.Models;

public class Subscription
{
    public string Title { get; set; }
    public long UserId { get; set; }
    public string Keyword { get; set; }
    public double Experience { get; set; }
    public EnglishLevels EnglishLevel { get; set; }
    public List<JobWebsites> PreferredWebsites { get; set; }
}
