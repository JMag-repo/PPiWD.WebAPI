using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PPiWD.WebAPI.Database;
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

        app.MapGet("/User", (DatabaseContext context, ClaimsPrincipal claimsPrincipal) =>
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
            var user = context.Users.FirstOrDefault(x => x.Id == int.Parse(userId));

            return user == null ? Results.BadRequest("Could not resolve user") : Results.Ok(new { id = user.Id, username = user.Username});
        });
    }
}