using System.ComponentModel.DataAnnotations;
namespace KeyVerse.Models
{
    public class Juego
    {
        [Key]
        public int IdJuego { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}