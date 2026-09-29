
using System.ComponentModel.DataAnnotations;

namespace SistemaPrototipos.Models
{
    public class RestablecerContrasenaModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(
            50,
            MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres."
        )]
        [DataType(DataType.Password)]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare(
            nameof(NuevaContrasena),
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}