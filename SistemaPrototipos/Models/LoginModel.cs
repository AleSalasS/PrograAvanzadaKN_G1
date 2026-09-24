using System.ComponentModel.DataAnnotations;

namespace SistemaPrototipos.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        public bool Recordarme { get; set; }
    }
}
