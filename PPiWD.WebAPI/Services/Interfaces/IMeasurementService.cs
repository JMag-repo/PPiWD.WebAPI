using PPiWD.WebAPI.Models.Measurements;

namespace PPiWD.WebAPI.Services.Interfaces;

public interface IMeasurementService
{
    Guid Create(Measurement measurement);

    Guid Update(Measurement measurement);

    Measurement? GetById(Guid id);

    void Delete(Guid id);
}