using AutoMapper;
using Common.Dto.Auth;
using Common.Models;

namespace Common.MappingProfiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<AuthorizationResponse, AuthorizationResponseModel>();
    }
}