using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace movie_mvc.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }
        public List<Favorito>? PeliculasFavoritas { get; set; }
        public List<Review>? ReviewsUsuarios { get; set; }

    }
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Debes ingresar un Nombre.")]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Debes ingresar un Apellido.")]
        [StringLength(50)]
        public string Apellido { get; set; }
        [EmailAddress(ErrorMessage = "Ingresa un email valido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Clave { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarClave{ get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Ingresa un email valido.")]
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }
    }
}
