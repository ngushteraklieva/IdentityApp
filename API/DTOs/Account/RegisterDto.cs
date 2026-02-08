using API.Utility;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength=3, ErrorMessage="User name must be at least {2}, and maximum {1} characters")]
        [RegularExpression(SD.UserNameRegex, ErrorMessage = "User name must contain only a-z A-Z 0-9 characters")]

        public string Name { get; set; }

        private string _email;
        [Required]
        [RegularExpression(SD.EmailRegex, ErrorMessage = "Invalid email address")]

        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }

        private string _password;
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]

        public string Password
        {
            get => _password;
            set => _password = value.ToLower();
        }
    }
}
