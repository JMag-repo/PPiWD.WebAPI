using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using PPiWD.WebAPI.Models.Authentication;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Endpoints;

public static class AuthorizationEndpoints
{
    public static void MapAuthorizationEndpoints(this WebApplication app)
    {
        app.MapPost("/User/Authorize", ([FromBody]AuthenticateUser user, [FromServices]IUserService userService) =>
        {
            try
            {
                var authorized = userService.Authorize(user);
                return Results.Ok(authorized);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message});
            }
        }).WithName("Authorize");

        app.MapPost("/User/Register", ([FromBody] AuthenticateUser user, [FromServices] IUserService userService) =>
        {
            try
            {
                var authorized = userService.Create(new User(){ Username = user.Username}, user.Password);
                return Results.Ok(authorized);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message});
            }
        }).WithName("Register");
    }
}