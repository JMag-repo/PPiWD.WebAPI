using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PPiWD.WebAPI.Models.Measurements;

namespace PPiWD.WebAPI.Models.Authentication;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    [JsonIgnore]
    public byte[] PasswordHash { get; set; }

    [JsonIgnore]
    public byte[] PasswordSalt { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}