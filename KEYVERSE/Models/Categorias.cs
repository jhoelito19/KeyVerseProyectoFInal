using System.ComponentModel.DataAnnotations;

namespace KeyVerse.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
    }
}