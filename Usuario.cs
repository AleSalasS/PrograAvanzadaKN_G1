using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace SistemaPrototipos.Models
{
    public class Usuario
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
    }

    public static class UsuarioStore
    {
        private static readonly ConcurrentDictionary<string, Usuario> usuarios =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, TokenRecuperacion> tokensRecuperacion =
            new();

        private static readonly PasswordHasher<Usuario> hasher = new();

        private static readonly TimeSpan DuracionToken =
            TimeSpan.FromMinutes(30);

        public static bool Registrar(
            string nombre,
            string correo,
            string contrasena)
        {
            var usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Correo = correo.Trim()
            };

            usuario.ContrasenaHash =
                hasher.HashPassword(usuario, contrasena);

            return usuarios.TryAdd(usuario.Correo, usuario);
        }

        public static Usuario? Validar(
            string correo,
            string contrasena)
        {
            if (!usuarios.TryGetValue(correo.Trim(), out var usuario))
                return null;

            var resultado =
                hasher.VerifyHashedPassword(
                    usuario,
                    usuario.ContrasenaHash,
                    contrasena);

            return resultado == PasswordVerificationResult.Failed
                ? null
                : usuario;
        }

        public static string? GenerarTokenRecuperacion(string correo)
        {
            correo = correo.Trim();

            if (!usuarios.ContainsKey(correo))
                return null;

            var token = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            tokensRecuperacion[token] = new TokenRecuperacion
            {
                Correo = correo,
                Expira = DateTime.UtcNow.Add(DuracionToken)
            };

            return token;
        }

        public static bool TokenEsValido(string token)
        {
            if (!tokensRecuperacion.TryGetValue(token, out var informacion))
                return false;

            if (informacion.Expira < DateTime.UtcNow)
            {
                tokensRecuperacion.TryRemove(token, out _);
                return false;
            }

            return true;
        }

        public static bool RestablecerContrasena(
            string token,
            string nuevaContrasena)
        {
            if (!tokensRecuperacion.TryGetValue(
                    token,
                    out var informacion))
            {
                return false;
            }

            if (informacion.Expira < DateTime.UtcNow)
            {
                tokensRecuperacion.TryRemove(token, out _);
                return false;
            }

            if (!usuarios.TryGetValue(
                    informacion.Correo,
                    out var usuario))
            {
                return false;
            }

            usuario.ContrasenaHash =
                hasher.HashPassword(
                    usuario,
                    nuevaContrasena);

            tokensRecuperacion.TryRemove(token, out _);

            return true;
        }
    }

    public class TokenRecuperacion
    {
        public string Correo { get; set; } = string.Empty;

        public DateTime Expira { get; set; }
    }
}