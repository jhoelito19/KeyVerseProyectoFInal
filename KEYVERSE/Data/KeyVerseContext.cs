using Microsoft.EntityFrameworkCore;
using KeyVerse.Models;

namespace KeyVerse.Data
{
    public class KeyVerseContext : DbContext
    {
        public KeyVerseContext(DbContextOptions<KeyVerseContext> options) : base(options) { }

        // Aquí le decimos qué tablas queremos que automatice
        public DbSet<Juego> Juegos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}