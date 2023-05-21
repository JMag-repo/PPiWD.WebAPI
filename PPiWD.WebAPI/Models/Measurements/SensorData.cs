using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PPiWD.WebAPI.Models.Measurements
{
    public class SensorData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [JsonIgnore]
        public virtual Measurement Measurement { get; set; }

        [Required]
        public string Sensor { get; set; }

        public int MeasurementId { get; set; }

        [Required]
        public float XAxis { get; set; } = 0;

        [Required]
        public float YAxis { get; set; } = 0;

        [Required]
        public float ZAxis { get; set; } = 0;
    }
}
