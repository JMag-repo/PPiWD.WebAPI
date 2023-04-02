using System.ComponentModel.DataAnnotations;

namespace PPiWD.WebAPI.Models.Authentication;

public class AuthenticateUser
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}