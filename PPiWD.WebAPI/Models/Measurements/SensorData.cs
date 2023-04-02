using System.ComponentModel.DataAnnotations;

namespace PPiWD.WebAPI.Models.Measurements
{
    public class SensorData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Measurement Measurement { get; set; }

        [Required]
        public string Sensor { get; set; }

        [Required]
        public double XAxis { get; set; } = 0;

        [Required]
        public double YAxis { get; set; } = 0;

        [Required]
        public double ZAxis { get; set; } = 0;
    }
}
