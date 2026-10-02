using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Tarea1.Models
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }

        public Usuario()
        {
            Nombre = string.Empty;
            Correo = string.Empty;
            ContrasenaHash = string.Empty;
        }
    }

    public static class UsuarioStore
    {
        private static readonly ConcurrentDictionary<string, Usuario> usuarios =
            new ConcurrentDictionary<string, Usuario>(
                StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, TokenRecuperacion> tokensRecuperacion =
            new ConcurrentDictionary<string, TokenRecuperacion>();

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

            usuario.ContrasenaHash = HashContrasena(contrasena);

            return usuarios.TryAdd(usuario.Correo, usuario);
        }

        public static Usuario Validar(
            string correo,
            string contrasena)
        {
            Usuario usuario;

            if (!usuarios.TryGetValue(correo.Trim(), out usuario))
                return null;

            return VerificarContrasena(
                contrasena,
                usuario.ContrasenaHash)
                ? usuario
                : null;
        }

        public static string GenerarTokenRecuperacion(string correo)
        {
            correo = correo.Trim();

            if (!usuarios.ContainsKey(correo))
                return null;

            string token;

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);

                token = BitConverter
                    .ToString(bytes)
                    .Replace("-", "")
                    .ToLowerInvariant();
            }

            tokensRecuperacion[token] = new TokenRecuperacion
            {
                Correo = correo,
                Expira = DateTime.UtcNow.Add(DuracionToken)
            };

            return token;
        }

        public static bool TokenEsValido(string token)
        {
            TokenRecuperacion informacion;

            if (string.IsNullOrWhiteSpace(token) ||
                !tokensRecuperacion.TryGetValue(token, out informacion))
            {
                return false;
            }

            if (informacion.Expira < DateTime.UtcNow)
            {
                TokenRecuperacion eliminado;
                tokensRecuperacion.TryRemove(token, out eliminado);
                return false;
            }

            return true;
        }

        public static bool RestablecerContrasena(
            string token,
            string nuevaContrasena)
        {
            TokenRecuperacion informacion;

            if (!tokensRecuperacion.TryGetValue(
                token,
                out informacion))
            {
                return false;
            }

            if (informacion.Expira < DateTime.UtcNow)
            {
                TokenRecuperacion eliminado;
                tokensRecuperacion.TryRemove(token, out eliminado);
                return false;
            }

            Usuario usuario;

            if (!usuarios.TryGetValue(
                informacion.Correo,
                out usuario))
            {
                return false;
            }

            usuario.ContrasenaHash =
                HashContrasena(nuevaContrasena);

            TokenRecuperacion tokenEliminado;
            tokensRecuperacion.TryRemove(
                token,
                out tokenEliminado);

            return true;
        }

        private static string HashContrasena(string contrasena)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(contrasena);
                var hash = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        private static bool VerificarContrasena(
            string contrasena,
            string hashGuardado)
        {
            return HashContrasena(contrasena) == hashGuardado;
        }
    }

    public class TokenRecuperacion
    {
        public string Correo { get; set; }
        public DateTime Expira { get; set; }

        public TokenRecuperacion()
        {
            Correo = string.Empty;
        }
    }
}