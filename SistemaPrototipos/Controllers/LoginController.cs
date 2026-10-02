using System.Web.Mvc;
using Tarea1.Models;

namespace Tarea1.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
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

            Session["UsuarioNombre"] = usuario.Nombre;
            Session["UsuarioCorreo"] = usuario.Correo;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Registro()
        {
            return View(new RegistroModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(RegistroModel model)
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
        public ActionResult RecuperarContrasena()
        {
            return View(new RecuperarContrasenaModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarContrasena(
            RecuperarContrasenaModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = UsuarioStore.GenerarTokenRecuperacion(
                model.Correo
            );

            ViewBag.Mensaje =
                "Si el correo corresponde a una cuenta registrada, " +
                "se ha generado un enlace de recuperación.";

            if (!string.IsNullOrEmpty(token))
            {
                var enlace = Url.Action(
                    "RestablecerContrasena",
                    "Login",
                    new { token = token },
                    Request.Url.Scheme
                );

                ViewBag.EnlaceRecuperacion = enlace;
            }

            ModelState.Clear();

            return View(new RecuperarContrasenaModel());
        }

        [HttpGet]
        public ActionResult RestablecerContrasena(string token)
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
        public ActionResult RestablecerContrasena(
            RestablecerContrasenaModel model)
        {
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

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Login");
        }
    }
}