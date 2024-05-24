using GestionStockApi.DTOs;
using GestionStockApi.Models;
using GestionStockApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionStockApi.Services
{

    public interface IProductoService
    {
        Task<(ProductResult result, IEnumerable<ProductoDTO>? productos)> GetProductos();
        Task<(ProductResult result, ProductoDTO? producto)> GetProducto(int id);
        Task<ProductResult> CrearProducto(int idCategoria, decimal precio);
        Task<ProductResult> ModificarProducto(int id, int idCategoria, decimal precio);
        Task<ProductResult> EliminarProducto(int id);
        Task<(ProductResult result, List<ProductoDTO>? productos)> FiltrarProductosPorPresupuesto(int presupuesto);
    }

    public class ProductoService : IProductoService
    {
        private readonly GestionStockDbContext _dbContext;

        public ProductoService(GestionStockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(ProductResult, IEnumerable<ProductoDTO>?)> GetProductos()
        {
            var productos = await _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();

            // Verifico si existen productos 
            if (!productos.Any())
            {
                return (ProductResult.NoProducts, null);
            }

            return (ProductResult.Success, productos.Select(p => MapProducto(p)).ToList());
        }

        public async Task<(ProductResult, ProductoDTO?)> GetProducto(int id)
        {
            var producto = await _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(p => p.Id == id);

            // Verifico si el producto existe
            if (producto == null)
            {
                return (ProductResult.ProductNotFound, null);
            }

            return (ProductResult.Success, MapProducto(producto));
        }

        public async Task<ProductResult> CrearProducto(int idCategoria, decimal precio)
        {
            // Verifico si la categoría existe
            var categoria = await _dbContext.Categorias.FindAsync(idCategoria);
            if (categoria == null)
            {
                return ProductResult.CategoryNotFound; // La categoría no es válida
            }

            var nuevoProducto = new Producto
            {
                Precio = precio,
                FechaCarga = DateTime.Now,
                IdCategoria = idCategoria
            };

            // Guarda el producto
            _dbContext.Productos.Add(nuevoProducto);
            await _dbContext.SaveChangesAsync();

            return ProductResult.Success;
        }

        public async Task<ProductResult> ModificarProducto(int id, int idCategoria, decimal precio)
        {
            // Verifico si el producto existe
            var producto = await _dbContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return ProductResult.ProductNotFound;
            }

            // Verifica si la categoría es válida
            var categoria = await _dbContext.Categorias.FindAsync(idCategoria);
            if (categoria == null)
            {
                return ProductResult.CategoryNotFound; // La categoría no es válida
            }

            // Modifico los atributos del producto
            producto.IdCategoria = idCategoria;
            producto.Precio = precio;

            // Guardo los cambios
            await _dbContext.SaveChangesAsync();

            return ProductResult.Success;
        }


        public async Task<ProductResult> EliminarProducto(int id)
        {
            // Verifico si el producto existe
            var producto = await _dbContext.Productos.FindAsync(id);
            if (producto == null)
            {
                return ProductResult.ProductNotFound;
            }

            // Elimino y guardo los cambios
            _dbContext.Productos.Remove(producto);
            await _dbContext.SaveChangesAsync();

            return ProductResult.Success;
        }

        public async Task<(ProductResult, List<ProductoDTO>?)> FiltrarProductosPorPresupuesto(int presupuesto)
        {
            var productosFiltrados = new List<ProductoDTO>();

            // Obtengo todos los productos ordenados por precio descendente
            var todosLosProductos = await _dbContext.Productos
                .Include(p => p.IdCategoriaNavigation)
                .OrderByDescending(p => p.Precio)
                .ToListAsync();

            if (!todosLosProductos.Any())
            {
                return (ProductResult.NoProducts, null);
            }

            // Variables para almacenar los productos de cada categoría
            Producto? productoComputacion = null;
            Producto? productoTelefonia = null;

            // Filtro productos por presupuesto y por categoría, solo devuelvo uno por categoría
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
                    if (productoMasCaro.IdCategoria == 1) // Computación
                    {
                        productoComputacion = productoMasCaro;
                    }
                    else if (productoMasCaro.IdCategoria == 2) // Telefonía
                    {
                        productoTelefonia = productoMasCaro;
                    }
                    presupuesto -= (int)productoMasCaro.Precio;
                }
            }

            // Verifico que se hayan encontrado productos de ambas categorías
            if (productoComputacion == null || productoTelefonia == null)
            {
                return (ProductResult.NoValidCombination, null);
            }

            productosFiltrados.Add(MapProducto(productoComputacion));
            productosFiltrados.Add(MapProducto(productoTelefonia));

            return (ProductResult.Success, productosFiltrados);
        }

        // Mapeo para devolver solo los datos necesarios,junto nombre de la categoria y con un formato de fecha amigable
        private ProductoDTO MapProducto(Producto producto)
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
    }
}
