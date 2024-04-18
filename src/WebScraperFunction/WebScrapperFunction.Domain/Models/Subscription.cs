using WebScrapperFunction.Domain.Enums;

namespace WebScrapperFunction.Domain.Models;
public class Subscription
{
    public long ChatId { get; set; }
    public string Title { get; set; }
    public string Specialty { get; set; }
    public double Experience { get; set; }
    public List<JobWebsites> PreferredWebsites { get; set; }
}
