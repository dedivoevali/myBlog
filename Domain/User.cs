using System.ComponentModel.DataAnnotations.Schema;
using Domain.Abstract;

namespace Domain
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public DateTime LastActivity { get; set; }
        public Avatar Avatar { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostReaction> PostReactions { get; set; }
        public ICollection<Passkey> Passkeys { get; set; }
        public ICollection<UserWarning> Warnings { get; set; } 
        public ICollection<UserBanLog> BanLogs { get; set; }
        public bool IsBanned { get; set; }
        public string Initials { get; set; }
        public string? FullName => FirstName != null && LastName != null ? $"{FirstName} {LastName}" : null;
    }
}