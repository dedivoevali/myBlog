using AutoMapper;
using Common.Dto.Comment;
using Common.Models;
using Domain;

namespace Common.MappingProfiles;

public class CommentEntityProfile : Profile
{
    public CommentEntityProfile()
    {
        CreateMap<Comment, CommentModel>()
            .ForMember(e => e.Content, opt => opt.MapFrom(e => e.Content))
            .ForMember(e => e.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(e => e.RegistrationDate, opt => opt.MapFrom(e => e.RegistrationDate.ToString("u")))
            .ForMember(e => e.AuthorId, opt => opt.MapFrom(e => e.UserId))
            .ForMember(e => e.PostId, opt => opt.MapFrom(e => e.PostId))
            .ForMember(e => e.AuthorUsername, opt => opt.MapFrom(e => e.User.Username))
            .ForMember(dst => dst.AuthorInitials, opt => opt.MapFrom(src => src.User.Initials))
            .ForMember(e => e.PostTitle, opt => opt.MapFrom(e => e.Post.Title));

        CreateMap<CommentDto, Comment>()
            .ForMember(e => e.Id, opt => opt.Ignore())
            .ForMember(e => e.UserId, opt => opt.Ignore())
            .ForMember(e => e.User, opt => opt.Ignore())
            .ForMember(e => e.Post, opt => opt.Ignore())
            .ForMember(e => e.RegistrationDate, opt => opt.Ignore())
            .ForMember(e => e.Content, opt => opt.MapFrom(e => e.Content))
            .ForMember(e => e.PostId, opt => opt.MapFrom(e => e.PostId));
    }
}