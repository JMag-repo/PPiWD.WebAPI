using PPiWD.WebAPI.Models.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PPiWD.WebAPI.Models.Measurements
{
    public class Measurement
    {
        [Key]
        public int Id { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public int Duration { get; set; }
        
        public int UserId { get; set; }

        public virtual ICollection<SensorData> SensorDatas { get; set; }
    }
}
