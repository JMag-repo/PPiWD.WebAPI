namespace PPiWD.WebAPI.Models.Authentication;

public class AuthorizedUser
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Token { get; set; }
}