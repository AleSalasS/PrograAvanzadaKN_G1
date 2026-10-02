using System.ComponentModel.DataAnnotations;

namespace Tarea1.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        public bool Recordarme { get; set; }
    }
}