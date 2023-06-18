using System.Globalization;
using System.Net.Mime;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.MachineLearning;
using PPiWD.WebAPI.Models.Measurements;
using PPiWD.WebAPI.Services.Interfaces;
using Python.Runtime;

namespace PPiWD.WebAPI.Endpoints;

public static class MeasurementsEndpoint
{
    public static void MapMeasurementsEndpoints(this WebApplication app)
    {
        app.MapPost("/Measurements/",
                ([FromBody] Measurement measurement, [FromServices] IMeasurementService measurementService,
                    ClaimsPrincipal claimsPrincipal, [FromServices] DatabaseContext context,
                    [FromServices] MLModel model,[FromServices] IHostApplicationLifetime appLifetime) =>
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
                        int jumpCount = 0;
                        try
                        {
                            jumpCount = model.Calculate(measurement);
                        }
                        catch (PythonException e)
                        {
                            Console.WriteLine(e);
                            appLifetime.StopApplication();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        return Results.Ok(new
                        {
                            id = measurement.Id,
                            Date = measurement.Date,
                            Duration = measurement.Duration,
                            userId = userId,
                            sensorDatas = measurement.SensorDatas,
                            jumpCount = jumpCount
                        });
                    }
                    catch (AuthenticationException e)
                    {
                        return Results.BadRequest(new {message = e.Message});
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
                return Results.BadRequest(new {message = e.Message});
            }
            catch (ArgumentNullException e)
            {
                return Results.NotFound(new {message = e.Message});
            }
        }).RequireAuthorization();

        app.MapGet("/Measurements", (DatabaseContext context, ClaimsPrincipal claimsPrincipal) =>
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
            var user = context.Users.FirstOrDefault(x => x.Id == int.Parse(userId));

            if (user == null)
            {
                return Results.BadRequest("Could not resolve user");
            }

            var measurements = context.Measurements.Where(x => x.UserId == user.Id).ToList();

            return Results.Ok(measurements);
        }).RequireAuthorization();

        app.MapDelete("/Measurements/{id:Guid}", (int id, [FromServices] IMeasurementService measurementService) =>
        {
            try
            {
                measurementService.Delete(id);
                return Results.Ok();
            }
            catch (AuthenticationException e)
            {
                return Results.BadRequest(new {message = e.Message});
            }
            catch (ArgumentNullException e)
            {
                return Results.NotFound(new {message = e.Message});
            }
        }).RequireAuthorization();
        ;

        app.MapPut("/Measurements/",
            ([FromBody] Measurement measurement, [FromServices] IMeasurementService measurementService) =>
            {
                try
                {
                    var responseObject = measurementService.Update(measurement);
                    return Results.Ok(responseObject);
                }
                catch (AuthenticationException e)
                {
                    return Results.BadRequest(new {message = e.Message});
                }
                catch (ArgumentNullException e)
                {
                    return Results.NotFound(new {message = e.Message});
                }
            }).RequireAuthorization();

        app.MapGet("/Calculate", ([FromServices] MLModel mlModel) =>
        {
            string path = Environment.CurrentDirectory;
            string dataPath = Path.Combine(path, "data.csv");
            List<SensorData> sensorDataList = new List<SensorData>();

            using (StreamReader reader = new StreamReader(dataPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] cells = line.Split(';');

                    if (cells.Length >= 3) // Ensure at least 3 values are present
                    {
                        SensorData sensorData = new SensorData
                        {
                            XAxis = float.Parse(cells[1], NumberStyles.Float, CultureInfo.InvariantCulture),
                            YAxis = float.Parse(cells[2],NumberStyles.Float, CultureInfo.InvariantCulture),
                            ZAxis = float.Parse(cells[3],NumberStyles.Float, CultureInfo.InvariantCulture)
                        };

                        sensorDataList.Add(sensorData);
                    }
                }

                return Results.Ok(mlModel.Calculate(new Measurement()
                {
                    SensorDatas = sensorDataList
                }));
            }
        });

        app.MapControllers();
    }
}