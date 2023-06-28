using AutoMapper;
using TracklyApi.Dtos.Url;
using TracklyApi.Entities;

namespace TracklyApi.Mappings;
public class UrlProfile : Profile
{
    public UrlProfile()
    {
        CreateMap<ManagedUrl, UrlDto>();
        CreateMap<ManagedUrl, UrlShortDto>();
        CreateMap<UrlVisit, UrlVisitDto>()
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress.ToString()));

        CreateMap<UrlDto, ManagedUrl>();
    }
}
