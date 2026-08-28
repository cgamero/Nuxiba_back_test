namespace TestBackNuxiba.Models;

public class User
{
    public int User_id { get; set; }

    public string Login { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string? ApellidoPaterno { get; set; }

    public string? ApellidoMaterno { get; set; }

    public string? Password { get; set; }

    public int? TipoUser_id { get; set; }

    public int Status { get; set; }

    public DateTime fCreate { get; set; }

    public int? IDArea { get; set; }

    public DateTime? LastLoginAttempt { get; set; }

    // Navigation properties
    public Area? Area { get; set; }

    public ICollection<Login> Logins { get; set; } = new List<Login>();
}