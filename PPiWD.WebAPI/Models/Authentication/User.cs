using System.ComponentModel.DataAnnotations;
using PPiWD.WebAPI.Models.Measurements;

namespace PPiWD.WebAPI.Models.Authentication;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }
    
    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}