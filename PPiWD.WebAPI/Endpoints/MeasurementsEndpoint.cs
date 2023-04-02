using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using PPiWD.WebAPI.Models.Measurements;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Endpoints;

public static class MeasurementsEndpoint
{
    public static void MapMeasurementsEnpoints(this WebApplication app)
    {
        app.MapPost("/Measurements/", ([FromBody] Measurement measurement, [FromServices] IMeasurementService measurementService) =>
        {
            try
            {
                var responseObject = measurementService.Create(measurement);
                return Results.Ok(responseObject);
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new { message = e.Message });
            }
        }).WithName("PostMeasurements");

        app.MapGet("/Measurements/{id:Guid}", (Guid id, [FromServices] IMeasurementService measurementService) =>
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
        });

        app.MapDelete("/Measurements/{id:Guid}", (Guid id, [FromServices] IMeasurementService measurementService) =>
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
        });

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
        });

        app.MapControllers();
    }
}