using System;

namespace KeyVerseMVC.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public int IdMetodoPago { get; set; }
        public DateTime FechaCompra { get; set; }
        public string EstadoPago { get; set; }
        public decimal TotalPagado { get; set; }
    }
}