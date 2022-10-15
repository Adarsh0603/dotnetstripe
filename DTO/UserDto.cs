using System.ComponentModel.DataAnnotations;

namespace dotnetstripe.DTO
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
