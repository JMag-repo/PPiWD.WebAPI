using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.Models.Authentication;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Services;
public class UserService : IUserService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration configuration;

    public UserService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public AuthorizedUser Authorize(AuthenticateUser model)
    {
        var user = Authenticate(model.Username, model.Password) ?? throw new AuthenticationException("Username or password is incorrect");
        var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtSecret") ?? throw new ArgumentNullException("Authorization key cannot be null"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // return basic user info and authentication token
        return new AuthorizedUser()
        {
            Id = user.Id,
            Username = user.Username,
            Token = tokenString,
        };
    }

    public User? Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return null;

        var user = _context.Users.SingleOrDefault(x => x.Username == username);

        if (user == null)
            return null;

        return !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt) ? null : user;
    }

    public User Create(User user, string password)
    {
        // validation
        if (string.IsNullOrWhiteSpace(password))
            throw new AuthenticationException("PASSWORD_IS_REQUIRED");

        if (_context.Users.Any(x => x.Username == user.Username))
            throw new AuthenticationException("USERNAME_TAKEN");

        CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public void Update(User userParam, string password)
    {
        var user = _context.Users.Find(userParam.Id);

        if (user == null)
            throw new ArgumentNullException(nameof(user), "User not found");


        if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
        {
            if (_context.Users.Any(x => x.Username == userParam.Username))
                throw new ArgumentException("Username " + userParam.Username + " is already taken");

            user.Username = userParam.Username;
        }

        // update password if provided
        if (!string.IsNullOrWhiteSpace(password))
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public User GetById(int id)
    {
        return _context.Users.Find(id);
    }

    public void Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return;
        }

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

        using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return !computedHash.Where((t, i) => t != storedHash[i]).Any();
    }
}