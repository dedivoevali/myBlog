using AutoMapper;
using Common.Dto.Post;
using Common.Models;
using Domain;

namespace Common.MappingProfiles
{
    public class PostEntityProfile : Profile
    {
        public PostEntityProfile()
        {
            CreateMap<Post, PostModel>()
                .ForMember(dst => dst.AuthorId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dst => dst.AuthorUsername, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dst => dst.AuthorInitials, opt => opt.MapFrom(src => src.User.Initials))
                .ForMember(dst => dst.Topic, opt => opt.MapFrom(src => src.Topic))
                .ForMember(dst => dst.AmountOfComments, opt => opt.MapFrom(src => src.Comments.Count()))
                .ForMember(dst => dst.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate.ToString("u")))
                .ForMember(dst => dst.Language, opt => opt.MapFrom(src => src.DetectedLanguage));

            CreateMap<PostDto, Post>()
                .ForMember(dst => dst.Title,
                    opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dst => dst.Topic,
                    opt => opt.MapFrom(src => String.IsNullOrWhiteSpace(src.Topic) ? null : src.Topic));
        }
    }
}