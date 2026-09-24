using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace SistemaPrototipos.Models
{
    public class Usuario
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
    }

    // Almacenamiento temporal en memoria mientras no exista la base de datos.
    // Los usuarios registrados se pierden al reiniciar la aplicación.
    public static class UsuarioStore
    {
        private static readonly ConcurrentDictionary<string, Usuario> usuarios = new(StringComparer.OrdinalIgnoreCase);
        private static readonly PasswordHasher<Usuario> hasher = new();

        public static bool Registrar(string nombre, string correo, string contrasena)
        {
            var usuario = new Usuario { Nombre = nombre.Trim(), Correo = correo.Trim() };
            usuario.ContrasenaHash = hasher.HashPassword(usuario, contrasena);
            return usuarios.TryAdd(usuario.Correo, usuario);
        }

        public static Usuario? Validar(string correo, string contrasena)
        {
            if (!usuarios.TryGetValue(correo.Trim(), out var usuario))
                return null;

            var resultado = hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, contrasena);
            return resultado == PasswordVerificationResult.Failed ? null : usuario;
        }
    }
}
