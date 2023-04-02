using PPiWD.WebAPI.Database;
using PPiWD.WebAPI.Models.Measurements;
using PPiWD.WebAPI.Services.Interfaces;

namespace PPiWD.WebAPI.Services;
public class MeasurementService : IMeasurementService
{
    private readonly DatabaseContext _context;

    public MeasurementService(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Guid Create(Measurement measurement)
    {
        _context.Measurements.Add(measurement);
        foreach (var data in measurement.SensorDatas)
        {
            _context.SensorDatas.Add(data);
        }

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