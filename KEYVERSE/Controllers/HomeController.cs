using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

       
        public IActionResult Index()
        {
            // 🔥 SOLUCIÓN: Añadimos OrderBy para que NUNCA se muevan de su lugar al actualizar el stock
            var listaJuegos = _context.Juegos.OrderBy(j => j.IdJuego).ToList();
            return View(listaJuegos);
        }

        // 2. Buscador optimizado para PostgreSQL
        public IActionResult Buscar(string nombre)
        {
            // Si el buscador llega vacío, lo devolvemos al inicio
            if (string.IsNullOrEmpty(nombre))
            {
                return RedirectToAction("Index");
            }

            // 🔥 Búsqueda blindada: Convertimos a minúsculas tanto la BD como el texto del usuario
            string busqueda = nombre.ToLower();

            var resultados = _context.Juegos
                .Where(j => j.Nombre.ToLower().Contains(busqueda))
                .OrderBy(j => j.IdJuego)
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

            // 🔥 TRUCO UX: Leer la URL de donde vino el clic y devolverlo ahí mismo
            string urlAnterior = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(urlAnterior))
            {
                return Redirect(urlAnterior); // Lo deja en la página del juego
            }

            // Si por alguna razón falla, el plan B es mandarlo al carrito
            return RedirectToAction("Carrito");
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
                    // 🔥 BALA DE PLATA: Le ordenamos directamente a Postgres que reste 1, 
                    // sin importar lo que opine Entity Framework ni la memoria caché.
                    _context.Database.ExecuteSqlRaw($"UPDATE \"Juegos\" SET \"Stock\" = \"Stock\" - 1 WHERE \"IdJuego\" = {item.IdJuego} AND \"Stock\" > 0");
                }

                // Como lanzamos la orden directo al motor, ya no necesitamos usar _context.SaveChanges()

                // Vaciamos el carrito de la sesión
                HttpContext.Session.Remove("MiCarrito");

                ViewBag.NumeroOrden = new Random().Next(100000, 999999);
                return View(carritoFinal);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();
    }
}