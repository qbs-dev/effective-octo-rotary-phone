using AutoMapper;
using TracklyApi.Dtos.Profile;
using TracklyApi.Entities;

namespace TracklyApi.Mappings;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, ProfileDto>().ReverseMap();

        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(x => 0))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(x => false))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(x => DateTime.UtcNow))
            .ForMember(dest => dest.LastAccessDate, opt => opt.MapFrom(x => DateTime.UnixEpoch));

        CreateMap<ProfileBaseDto, User>();
    }
}
