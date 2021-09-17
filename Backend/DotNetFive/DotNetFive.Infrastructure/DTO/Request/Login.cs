using System.ComponentModel.DataAnnotations;

namespace DotNetFive.Infrastructure.DTO.Request
{
    public class Login
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
