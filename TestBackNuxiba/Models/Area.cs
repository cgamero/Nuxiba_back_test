namespace TestBackNuxiba.Models;

public class Area
{
    public int IDArea { get; set; }

    public string AreaName { get; set; } = string.Empty;

    public bool StatusArea { get; set; }

    public DateTime CreateDate { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; } = new List<User>();
}