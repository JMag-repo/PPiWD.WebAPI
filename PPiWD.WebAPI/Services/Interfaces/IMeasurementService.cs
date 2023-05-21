using PPiWD.WebAPI.Models.Measurements;

namespace PPiWD.WebAPI.Services.Interfaces;

public interface IMeasurementService
{
    int Create(Measurement measurement);

    int Update(Measurement measurement);

    Measurement? GetById(int id);

    void Delete(int id);
}