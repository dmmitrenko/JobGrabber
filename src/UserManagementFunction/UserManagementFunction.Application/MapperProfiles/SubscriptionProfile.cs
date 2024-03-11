using AutoMapper;
using Newtonsoft.Json;
using UserManagementFunction.DataContext.Entities;
using UserManagementFunction.Domain.Enums;

namespace UserManagementFunction.Application.MapperProfiles;
public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Domain.Models.Subscription, Subscription>()
            .ForMember(x => x.PartitionKey, src => src.MapFrom(dest => dest.UserId.ToString()))
            .ForMember(x => x.RowKey, src => src.MapFrom(dest => dest.Title))
            .ForMember(x => x.PreferredWebsites, src => src.Ignore())
            .ForMember(x => x.Timestamp, src => src.Ignore())
            .ForMember(x => x.ETag, src => src.Ignore())
            .ForMember(x => x.Experience, src => src.MapFrom(dest => dest.Experience))
            .ForMember(x => x.Specialty, src => src.MapFrom(dest => dest.Specialty))
            .ForMember(x => x.UserId, src => src.MapFrom(dest => dest.UserId))
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.SerializeObject(dest.PreferredWebsites)));

        CreateMap<Subscription, Domain.Models.Subscription>()
            .ForMember(x => x.PreferredWebsites, src => src.MapFrom(dest => JsonConvert.DeserializeObject<List<JobWebsites>>(dest.PreferredWebsites)));
    }
}
