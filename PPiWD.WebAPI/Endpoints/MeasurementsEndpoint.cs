using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.Models.Measurements;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Endpoints;

public static class MeasurementsEndpoint
{
    public static void MapMeasurementsEndpoints(this WebApplication app)
    {
        app.MapPost("/Measurements/", ([FromBody] Measurement measurement, [FromServices] IMeasurementService measurementService, ClaimsPrincipal claimsPrincipal, [FromServices] DatabaseContext context) =>
        {
            try
            {
                var userId = claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
                var user = context.Users.FirstOrDefault(x => x.Id == int.Parse(userId));

                if (user == null)
                {
                    throw new Exception("Internal error");
                }

                measurement.User = user;
                var responseObject = measurementService.Create(measurement);
                return Results.Ok(responseObject);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message });
            }
        })
            .WithName("PostMeasurements")
            .RequireAuthorization();

        app.MapGet("/Measurements/{id:int}", (int id, [FromServices] IMeasurementService measurementService) =>
        {
            try
            {
                var responseObject = measurementService.GetById(id);
                return Results.Ok(responseObject);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message });
            }
            catch (ArgumentNullException e)
            {
                return Results.NotFound(new { message = e.Message });
            }
        }).RequireAuthorization();;

        app.MapDelete("/Measurements/{id:Guid}", (int id, [FromServices] IMeasurementService measurementService) =>
        {
            try
            {
                measurementService.Delete(id);
                return Results.Ok();
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message });
            }
            catch (ArgumentNullException e)
            {
                return Results.NotFound(new { message = e.Message });
            }
        }).RequireAuthorization();;

        app.MapPut("/Measurements/", ([FromBody] Measurement measurement, [FromServices] IMeasurementService measurementService) =>
        {
            try
            {
                var responseObject = measurementService.Update(measurement);
                return Results.Ok(responseObject);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message });
            }
            catch (ArgumentNullException e)
            {
                return Results.NotFound(new { message = e.Message });
            }
        }).RequireAuthorization();;

        app.MapControllers();
    }
}