
using Microsoft.AspNetCore.Mvc;
using SistemaPrototipos.Models;

namespace SistemaPrototipos.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = UsuarioStore.Validar(
                model.Correo,
                model.Contrasena
            );

            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Correo o contraseña incorrectos."
                );

                return View(model);
            }

            TempData["Bienvenida"] =
                $"Bienvenido(a), {usuario.Nombre}";

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View(new RegistroModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(RegistroModel model)
        {
            if (!model.AceptaTerminos)
            {
                ModelState.AddModelError(
                    nameof(model.AceptaTerminos),
                    "Debe aceptar los términos."
                );
            }

            if (!ModelState.IsValid)
                return View(model);

            if (!UsuarioStore.Registrar(
                model.Nombre,
                model.Correo,
                model.Contrasena))
            {
                ModelState.AddModelError(
                    nameof(model.Correo),
                    "Ya existe una cuenta con este correo."
                );

                return View(model);
            }

            TempData["Mensaje"] =
                "Cuenta creada correctamente. Ya puede iniciar sesión.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View(new RecuperarContrasenaModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecuperarContrasena(
            RecuperarContrasenaModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Genera un token solamente si existe el usuario.
            var token = UsuarioStore.GenerarTokenRecuperacion(
                model.Correo
            );

            // Mensaje genérico para no revelar si el correo existe.
            ViewBag.Mensaje =
                "Si el correo corresponde a una cuenta registrada, " +
                "se ha generado un enlace de recuperación.";

            // MODO DEMOSTRACIÓN:
            // Mostramos el enlace en pantalla en lugar de enviarlo.
            if (!string.IsNullOrEmpty(token))
            {
                var enlace = Url.Action(
                    nameof(RestablecerContrasena),
                    "Login",
                    new { token = token },
                    Request.Scheme
                );

                ViewBag.EnlaceRecuperacion = enlace;
            }

            ModelState.Clear();

            return View(new RecuperarContrasenaModel());
        }

        [HttpGet]
        public IActionResult RestablecerContrasena(string token)
        {
            if (string.IsNullOrWhiteSpace(token) ||
                !UsuarioStore.TokenEsValido(token))
            {
                ViewBag.TokenInvalido = true;

                return View(new RestablecerContrasenaModel());
            }

            return View(new RestablecerContrasenaModel
            {
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestablecerContrasena(
            RestablecerContrasenaModel model)
        {
            // Se comprueba nuevamente el token al enviar
            // el formulario, no solamente al abrir el enlace.
            if (!UsuarioStore.TokenEsValido(model.Token))
            {
                ViewBag.TokenInvalido = true;

                ModelState.Clear();

                return View(new RestablecerContrasenaModel());
            }

            if (!ModelState.IsValid)
                return View(model);

            var resultado = UsuarioStore.RestablecerContrasena(
                model.Token,
                model.NuevaContrasena
            );

            if (!resultado)
            {
                ViewBag.TokenInvalido = true;

                ModelState.Clear();

                return View(new RestablecerContrasenaModel());
            }

            TempData["Mensaje"] =
                "Contraseña restablecida correctamente. " +
                "Ya puede iniciar sesión con su nueva contraseña.";

            return RedirectToAction(nameof(Index));
        }
    }
}