using GestionStockApi.DTOs;
using GestionStockApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionStockApi.Services
{

    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> GetProductos();
        Task<ProductoDTO> GetProducto(int id);
        Task<ProductoDTO> CrearProducto(int idCategoria, decimal precio);
        Task<int> ModificarProducto(int id, int idCategoria, decimal precio);
        Task<bool> EliminarProducto(int id);
        List<ProductoDTO> FiltrarProductosPorPresupuesto(int presupuesto);
    }

    public class ProductoService : IProductoService
    {
        private readonly GestionStockDbContext _dbContext;

        public ProductoService(GestionStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductoDTO>> GetProductos()
        {
            var productos = await _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();

            if (!productos.Any())
            {
                return new List<ProductoDTO>(); // Retorna una lista vacía en caso de no existir registros.
            }

            return productos.Select(p => MapProducto(p)).ToList();
        }

        public async Task<ProductoDTO> GetProducto(int id)
        {
            var producto = await _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return null;
            }

            return MapProducto(producto);
        }

        public async Task<ProductoDTO> CrearProducto(int idCategoria, decimal precio)
        {
            // Verifica si la categoría existe
            var categoria = await _dbContext.Categorias.FindAsync(idCategoria);
            if (categoria == null)
            {
                return null;
            }

            var nuevoProducto = new Producto
            {
                Precio = precio,
                FechaCarga = DateTime.Now,
                IdCategoria = idCategoria
            };

            _dbContext.Productos.Add(nuevoProducto);
            await _dbContext.SaveChangesAsync();

            return MapProducto(nuevoProducto);
        }

        public async Task<int> ModificarProducto(int id, int idCategoria, decimal precio)
        {
            // Verifica si el producto existe
            var producto = await _dbContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return 0; // El producto no existe
            }

            // Verifica si la categoría es válida
            var categoria = await _dbContext.Categorias.FindAsync(idCategoria);
            if (categoria == null)
            {
                return -1; // La categoría no es válida
            }

            // Actualiza los atributos del producto
            producto.IdCategoria = idCategoria;
            producto.Precio = precio;

            // Guarda los cambios
            return await _dbContext.SaveChangesAsync();

        }


        public async Task<bool> EliminarProducto(int id)
        {
            var producto = await _dbContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return false;
            }

            _dbContext.Productos.Remove(producto);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public List<ProductoDTO> FiltrarProductosPorPresupuesto(int presupuesto)
        {
            var productosFiltrados = new List<ProductoDTO>();

            // Obtener todos los productos ordenados por precio descendente
            var todosLosProductos = _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .OrderByDescending(p => p.Precio)
                .ToList();

            // Filtrar productos por presupuesto
            foreach (var categoriaProductos in todosLosProductos.GroupBy(p => p.IdCategoria))
            {
                var productoMasCaro = categoriaProductos.FirstOrDefault();
                foreach (var producto in categoriaProductos)
                {
                    if (producto.Precio <= presupuesto)
                    {
                        productoMasCaro = producto;
                        break;
                    }
                }

                if (productoMasCaro != null)
                {
                    productosFiltrados.Add(MapProducto(productoMasCaro));
                    presupuesto -= (int)productoMasCaro.Precio;
                }
            }

            return productosFiltrados;
        }

        public ProductoDTO MapProducto(Producto producto)
        {
            return new ProductoDTO
            {
                Id = producto.Id,
                Precio = producto.Precio,
                FechaCarga = producto.FechaCarga.ToString("dd/MM/yyyy HH:mm"),
                IdCategoria = producto.IdCategoria,
                Categoria = producto.IdCategoriaNavigation.Nombre
            };
        }

        public List<ProductoDTO> MapProductos(List<Producto> productos)
        {
            return productos.Select(p => MapProducto(p)).ToList();
        }

    }
}
