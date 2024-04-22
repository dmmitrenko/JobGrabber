using WebScrapperFunction.Domain.Enums;

namespace WebScrapperFunction.Infrastructure.Settings;
public class JobSearchingSettings
{
    public List<JobWebsites> DefaultWebsites { get; set; }
    public List<JobWebsites> PremiumWebsites { get; set; }
    public string DefaultCheckInterval { get; set; }
    public string PremiumCheckInterval { get; set; }
}
