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

            var usuario = UsuarioStore.Validar(model.Correo, model.Contrasena);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(model);
            }

            TempData["Bienvenida"] = $"Bienvenido(a), {usuario.Nombre}";
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
                ModelState.AddModelError(nameof(model.AceptaTerminos), "Debe aceptar los términos.");

            if (!ModelState.IsValid)
                return View(model);

            if (!UsuarioStore.Registrar(model.Nombre, model.Correo, model.Contrasena))
            {
                ModelState.AddModelError(nameof(model.Correo), "Ya existe una cuenta con este correo.");
                return View(model);
            }

            TempData["Mensaje"] = "Cuenta creada correctamente. Ya puede iniciar sesión.";
            return RedirectToAction("Index");
        }
    }
}
