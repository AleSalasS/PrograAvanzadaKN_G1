using System.ComponentModel.DataAnnotations;

namespace Tarea1.Models
{
    public class RegistroModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(
            50,
            MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres."
        )]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare(
            "Contrasena",
            ErrorMessage = "Las contraseñas no coinciden."
        )]
        [DataType(DataType.Password)]
        public string ConfirmarContrasena { get; set; }

        public bool AceptaTerminos { get; set; }
    }
}