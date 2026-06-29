using Microsoft.AspNetCore.Mvc;
using KeyVerse.Data;
using KeyVerse.Models;
using System.Text.Json;

namespace KeyVerse.Controllers
{
    public class HomeController : Controller
    {
        private readonly KeyVerseContext _context;

        // Inyectamos la base de datos automáticamente
        public HomeController(KeyVerseContext context)
        {
            _context = context;
        }

        // 1. Catálogo Principal con EF Core
        public IActionResult Index()
        {
            var listaJuegos = _context.Juegos.ToList();
            return View(listaJuegos);
        }

        // 2. Buscador en tiempo real con EF Core (LINQ)
        public IActionResult Buscar(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return RedirectToAction("Index");
            }

            // Busca coincidencias ignorando mayúsculas/minúsculas
            var resultados = _context.Juegos
                .Where(j => j.Nombre.ToLower().Contains(nombre.ToLower()))
                .ToList();

            return View("Index", resultados);
        }

        // 3. Detalles del Juego con EF Core
        public IActionResult Detalles(int id)
        {
            var juego = _context.Juegos.FirstOrDefault(j => j.IdJuego == id);

            if (juego == null)
            {
                return RedirectToAction("Index");
            }

            return View(juego);
        }

        // --- LOS MÉTODOS DEL CARRITO SE QUEDAN EXACTAMENTE IGUAL ---
        public IActionResult AgregarAlCarrito(int id)
        {
            var juego = _context.Juegos.FirstOrDefault(j => j.IdJuego == id);
            if (juego != null)
            {
                string carritoJson = HttpContext.Session.GetString("MiCarrito");
                List<Juego> carritoUsuario = string.IsNullOrEmpty(carritoJson)
                    ? new List<Juego>()
                    : JsonSerializer.Deserialize<List<Juego>>(carritoJson);

                carritoUsuario.Add(juego);
                HttpContext.Session.SetString("MiCarrito", JsonSerializer.Serialize(carritoUsuario));
            }
            return RedirectToAction("Index");
        }

        public IActionResult Carrito()
        {
            string carritoJson = HttpContext.Session.GetString("MiCarrito");
            List<Juego> carritoUsuario = string.IsNullOrEmpty(carritoJson)
                ? new List<Juego>()
                : JsonSerializer.Deserialize<List<Juego>>(carritoJson);

            return View(carritoUsuario);
        }

        public IActionResult EliminarDelCarrito(int id)
        {
            string carritoJson = HttpContext.Session.GetString("MiCarrito");
            if (!string.IsNullOrEmpty(carritoJson))
            {
                List<Juego> carritoUsuario = JsonSerializer.Deserialize<List<Juego>>(carritoJson);
                carritoUsuario.RemoveAll(j => j.IdJuego == id);
                HttpContext.Session.SetString("MiCarrito", JsonSerializer.Serialize(carritoUsuario));
            }
            return RedirectToAction("Carrito");
        }

        public IActionResult ConfirmarCompra()
        {
            string carritoJson = HttpContext.Session.GetString("MiCarrito");
            List<Juego> carritoFinal = string.IsNullOrEmpty(carritoJson)
                ? new List<Juego>()
                : JsonSerializer.Deserialize<List<Juego>>(carritoJson);

            if (carritoFinal.Count > 0)
            {
                foreach (var item in carritoFinal)
                {
                    // Buscamos el juego específico en la base de datos usando su ID
                    var juegoEnDb = _context.Juegos.FirstOrDefault(j => j.IdJuego == item.IdJuego);

                    if (juegoEnDb != null)
                    {
                        if (juegoEnDb.Stock > 0)
                        {
                            juegoEnDb.Stock -= 1; // Restamos una unidad por cada compra
                        }
                    }
                }

                // Guardamos los cambios de forma definitiva en la nube
                _context.SaveChanges();
                // ------------------------------------------------------------

                // Vaciamos el carrito de la memoria del usuario
                HttpContext.Session.Remove("MiCarrito");

                ViewBag.NumeroOrden = new Random().Next(100000, 999999);
                return View(carritoFinal);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();
    }
}