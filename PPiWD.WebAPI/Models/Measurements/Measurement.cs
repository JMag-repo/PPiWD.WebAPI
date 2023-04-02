using PPiWD.WebAPI.Models.Authentication;
using System.ComponentModel.DataAnnotations;

namespace PPiWD.WebAPI.Models.Measurements
{
    public class Measurement
    {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }

        [Required]
        public DateTime MeasurementTime { get; set; }

        [Required]
        public int Duration { get; set; }

        public ICollection<SensorData> SensorDatas { get; set; }
    }
}
