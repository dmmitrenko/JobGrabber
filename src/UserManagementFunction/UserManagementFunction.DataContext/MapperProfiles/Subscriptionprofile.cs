using AutoMapper;
using Newtonsoft.Json;
using UserManagementFunction.DataContext.Entities;
using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.DataContext.MapperProfiles;
public class SubscriptionProfile : Profile
{
    protected SubscriptionProfile()
    {
        CreateMap<Domain.Models.Subscription, Subscription>()
            .ForMember(x => x.PartitionKey, src => src.MapFrom(dest => dest.UserId.ToString()))
            .ForMember(x => x.RowKey, src => src.MapFrom(dest => dest.Id.ToString()))
            .ForMember(x => x.EnglishLevel, src => src.MapFrom(dest => dest.EnglishLevel.ToString()))
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.SerializeObject(dest.PreferredWebsites)));

        CreateMap<Subscription, Domain.Models.Subscription>()
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.DeserializeObject<List<JobWebsites>>(dest.PreferredWebsites)))
            .ForMember(x => x.EnglishLevel, src => src.MapFrom(dest => Enum.Parse<EnglishLevel>(dest.EnglishLevel, true)));
    }
}
