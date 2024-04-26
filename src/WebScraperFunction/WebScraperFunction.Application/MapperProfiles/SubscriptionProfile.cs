using AutoMapper;
using Newtonsoft.Json;
using WebScraperFunction.DataContext.Entities;
using WebScraperFunction.Domain.Enums;

namespace WebScraperFunction.Application.MapperProfiles;
public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Subscription, Domain.Models.Subscription>()
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.DeserializeObject<List<JobWebsites>>(dest.PreferredWebsites)));
    }
}
