using AutoMapper;
using Newtonsoft.Json;
using WebScrapperFunction.DataContext.Entities;
using WebScrapperFunction.Domain.Enums;

namespace WebScrapperFunction.Application.MapperProfiles;
public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Subscription, Domain.Models.Subscription>()
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.DeserializeObject<List<JobWebsites>>(dest.PreferredWebsites)));
    }
}
