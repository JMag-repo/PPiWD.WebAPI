﻿using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.Models.Measurements;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Services;
public class MeasurementService : IMeasurementService
{
    private readonly DatabaseContext _context;
    private readonly ILogger<MeasurementService> _logger;

    public MeasurementService(DatabaseContext context, ILogger<MeasurementService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Guid Create(Measurement measurement)
    {
        measurement.Id = Guid.NewGuid();
        _context.Measurements.Add(measurement);
        foreach (var data in measurement.SensorDatas)
        {
            _context.SensorDatas.Add(data);
        }
        _context.SaveChanges();

        _logger.LogWarning("Added new measurement. [Id]: {measurement.Id}", measurement.Id);

        return measurement.Id;
    }

    public void Delete(Guid id)
    {
        var foundMeasurement = _context.Measurements.Find(id);
        _ = foundMeasurement ?? throw new ArgumentNullException(nameof(foundMeasurement), "Measurement not found");

        _context.Measurements.Remove(foundMeasurement);
        _context.SaveChanges();
    }

    public Measurement? GetById(Guid id)
    {
        var foundMeasurement = _context.Measurements.Find(id);
        _ = foundMeasurement ?? throw new ArgumentNullException(nameof(foundMeasurement), "Measurement not found");

        return _context.Measurements.Find(id);
    }

    public Guid Update(Measurement measurement)
    {
        var foundMeasurement = _context.Measurements.Find(measurement.Id);

        _ = foundMeasurement ?? throw new ArgumentNullException(nameof(foundMeasurement), "Measurement not found");

        _context.SensorDatas.RemoveRange(foundMeasurement.SensorDatas);
        _context.Measurements.Update(measurement);
        foreach (var data in measurement.SensorDatas)
        {
            _context.SensorDatas.Add(data);
        }

        _context.SaveChanges();
        return measurement.Id;
    }
}