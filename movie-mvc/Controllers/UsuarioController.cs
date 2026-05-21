using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_mvc.Models;

namespace movie_mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Agrega esta línea para proteger contra ataques CSRF
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, usuario.Recordarme, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, resultado.ToString());
                }
            }
            return View(usuario);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        public IActionResult Registro()
        { 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Agrega esta línea para proteger contra ataques CSRF
        public async Task<IActionResult> Registro(RegistroViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                // Aquí puedes agregar la lógica para registrar al usuario utilizando UserManager
                var nuevoUsuario = new Usuario
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "default-profile.png" // Puedes asignar una imagen predeterminada o permitir que el usuario la suba
                };
                var resultado = await _userManager.CreateAsync(nuevoUsuario, usuario.Clave);
                if (resultado.Succeeded)
                {
                    // Si el registro es exitoso, puedes iniciar sesión automáticamente o redirigir al usuario a la página de inicio de sesión
                    await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Si hay errores en el registro, puedes agregarlos al ModelState para mostrarlos en la vista
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
