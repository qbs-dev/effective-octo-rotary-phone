using AutoMapper;
using TracklyApi.Dtos.Url;
using TracklyApi.Entities;

namespace TracklyApi.Mappings;
public class UrlProfile : Profile
{
    public UrlProfile()
    {
        CreateMap<UrlAction, UrlActionDto>().ReverseMap();
        CreateMap<ManagedUrl, UrlDto>()
            .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.UrlActions));
        CreateMap<ManagedUrl, UrlShortDto>();
        CreateMap<UrlVisit, UrlVisitDto>();

        CreateMap<UrlDto, ManagedUrl>();
    }
}
