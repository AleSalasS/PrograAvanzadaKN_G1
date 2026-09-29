
using System.ComponentModel.DataAnnotations;

namespace SistemaPrototipos.Models
{
    public class RecuperarContrasenaModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Correo { get; set; } = string.Empty;
    }
}