using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using movie_mvc.Data;
using movie_mvc.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace movie_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDbContext _context;
        private const int PageSize = 8;

        public HomeController(ILogger<HomeController> logger, MovieDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int pagina = 1, string txtBusqueda = "", int generoId = 0)
        {
            if (pagina < 1) pagina = 1;

            var consulta = _context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(txtBusqueda))
            {
                consulta = consulta.Where(p => p.Titulo.Contains(txtBusqueda));
            }

            if (generoId > 0)
            {
                consulta = consulta.Where(p => p.GeneroId == generoId);
            }

            var totalPeliculas = await consulta.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalPeliculas / (double)PageSize);

            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            var peliculas = await consulta
                .Skip((pagina - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalPeliculas = totalPeliculas;
            //Este viewbag se utiliza para mantener el texto de búsqueda en la vista después de realizar una búsqueda
            ViewBag.TxtBusqueda = txtBusqueda;

            //Este bloque de código se encarga de cargar los géneros desde la base de datos,
            //ordenarlos por descripción, agregar una opción predeterminada
            //al inicio de la lista y luego pasar esa lista a la vista a través de ViewBag para
            //que se pueda utilizar en un dropdown o select en la interfaz de usuario.
            var generos = await _context.Generos.OrderBy(g => g.Descripcion).ToListAsync();
            generos.Insert(0, new Genero { Id = 0, Descripcion = "Género" });
            ViewBag.GeneroId = new SelectList(
                generos,
                "Id",
                "Descripcion",
                generoId
            );

            return View(peliculas);
        }
        public async Task<IActionResult> Details(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.ListaReviews)
                .ThenInclude(r => r.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            //Validacion para anular el boton de reseña si el usuario ya creo una reseña
            ViewBag.UserReview = false;
            if (User?.Identity?.IsAuthenticated == true && pelicula.ListaReviews != null)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.UserReview = !(pelicula.ListaReviews.FirstOrDefault(r => r.UsuarioId == userId) == null);
            }

            return View(pelicula);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
