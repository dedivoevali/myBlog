using System.ComponentModel.DataAnnotations;
using static Common.Validation.EntityConfigurationConstants;

namespace Common.Dto.Auth
{
    public class PasswordAuthorizeRequest
    {
        [Required]
        [MinLength(UsernameMinLength)]
        [MaxLength(UsernameMaxLength)]
        [RegularExpression(UsernameRegEx)]
        public string Username { get; set; }

        [Required]
        [MinLength(UserPasswordMinLength)]
        [MaxLength(UserPasswordMaxLength)]
        public string Password { get; set; }
    }
}