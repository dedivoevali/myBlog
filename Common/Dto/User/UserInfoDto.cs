using System.ComponentModel.DataAnnotations;
using static Common.Validation.EntityConfigurationConstants;

namespace Common.Dto.User
{
    public class UserInfoDto
    {
        [RegularExpression(UsernameRegEx)]
        [MinLength(UsernameMinLength)]
        [MaxLength(UsernameMaxLength)]
        public string? Username { get; set; }


        [RegularExpression(UserFirstnameAndLastnameRegEx)]
        [MinLength(UserFirstAndLastNameMinLength)]
        [MaxLength(UserFirstAndLastNameMaxLength)]
        public string? FirstName { get; set; }


        [RegularExpression(UserFirstnameAndLastnameRegEx)]
        [MinLength(UserFirstAndLastNameMinLength)]
        [MaxLength(UserFirstAndLastNameMaxLength)]
        public string? LastName { get; set; }
    }
}