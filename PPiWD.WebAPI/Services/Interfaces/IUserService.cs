using PPiWD.WebAPI.Models.Authentication;

namespace PPiWD.WebAPI.Services.Interfaces;

public interface IUserService
{
    AuthorizedUser Authorize(AuthenticateUser model);
    
    User? Authenticate(string username, string password);

    User Create(User user, string password);

    void Update(User user, string password);
    
    User GetById(int id);

    void Delete(int id);
}