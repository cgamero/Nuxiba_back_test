using System.ComponentModel.DataAnnotations;

namespace TestBackNuxiba.DTOs;

public class CreateLoginDto
{
    [Required]
    public int User_id { get; set; }

    [Required]
    public int Extension { get; set; }

    [Required]
    [Range(0, 1, ErrorMessage = "TipoMov must be 0 (logout) or 1 (login).")]
    public int TipoMov { get; set; }

    [Required]
    public DateTime fecha { get; set; }
}