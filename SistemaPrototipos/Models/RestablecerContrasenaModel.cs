using System.ComponentModel.DataAnnotations;

namespace Tarea1.Models
{
    public class RestablecerContrasenaModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(
            50,
            MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres."
        )]
        [DataType(DataType.Password)]
        public string NuevaContrasena { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare(
            "NuevaContrasena",
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; }
    }
}