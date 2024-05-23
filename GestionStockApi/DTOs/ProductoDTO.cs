namespace GestionStockApi.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public string? FechaCarga { get; set; }
        public int IdCategoria { get; set; }
        public string? Categoria { get; set; }
    }
}
