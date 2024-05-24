using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionStockApi.Services;
using GestionStockApi.Enums;

namespace GestionStockApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(IProductoService productoService, ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            _logger.LogInformation("Inicio del método GetProductos");

            try
            {
                var (response, productos) = await _productoService.GetProductos();

                switch (response)
                {
                    case ProductResult.Success:
                        _logger.LogInformation("Productos obtenidos exitosamente.");
                        return Ok(productos);
                    case ProductResult.NoProducts:
                        _logger.LogWarning("No hay productos disponibles.");
                        return NotFound("No hay productos disponibles.");
                    default:
                        _logger.LogError("Error desconocido al obtener productos.");
                        return StatusCode(500, "Error interno del servidor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            _logger.LogInformation("Inicio del método GetProducto");

            try
            {
                var (response, producto) = await _productoService.GetProducto(id);

                switch (response)
                {
                    case ProductResult.Success:
                        _logger.LogInformation($"Producto con ID {id} obtenido exitosamente.");
                        return Ok(producto);
                    case ProductResult.ProductNotFound:
                        _logger.LogWarning($"No existe ningún producto con el ID: {id}.");
                        return NotFound($"No existe ningún producto con el ID: {id}.");
                    default:
                        _logger.LogError("Error desconocido al obtener el producto.");
                        return StatusCode(500, "Error interno del servidor.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID {id}.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CrearProducto(int idCategoria, decimal precio)
        {
            _logger.LogInformation("Inicio del método CrearProducto");

            try
            {
                // Validar que el precio sea positivo y razonable
                if (precio < 1 || precio > 1000000)
                {
                    _logger.LogWarning("El precio debe estar entre 1 y 1,000,000.");
                    return BadRequest("El precio debe estar entre 1 y 1,000,000.");
                }

                var response = await _productoService.CrearProducto(idCategoria, precio);

                switch (response)
                {
                    case ProductResult.CategoryNotFound:
                        _logger.LogWarning("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                        return BadRequest("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                    case ProductResult.Success:
                        _logger.LogInformation("Producto creado exitosamente.");
                        return Ok("Producto creado exitosamente.");
                    default:
                        _logger.LogError($"Ocurrió un error al crear el producto. El método CrearProducto devolvió un valor inesperado: {response}");
                        return StatusCode(500, "Error interno del servidor");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificarProducto(int id, int idCategoria, decimal precio)
        {
            _logger.LogInformation("Inicio del método ModificarProducto");

            try
            {
                // Valido que el precio sea positivo y razonable
                if (precio < 1 || precio > 1000000)
                {
                    _logger.LogWarning("El precio debe estar entre 1 y 1,000,000.");
                    return BadRequest("El precio debe estar entre 1 y 1,000,000.");
                }

                var response = await _productoService.ModificarProducto(id, idCategoria, precio);

                switch (response)
                {
                    case ProductResult.ProductNotFound:
                        _logger.LogWarning($"No existe ningún producto con el ID: {id}.");
                        return NotFound($"No existe ningún producto con el ID: {id}.");
                    case ProductResult.CategoryNotFound:
                        _logger.LogWarning("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                        return BadRequest("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                    case ProductResult.Success:
                        _logger.LogInformation($"Producto con ID {id} modificado exitosamente.");
                        return Ok("Producto modificado exitosamente.");
                    default:
                        _logger.LogError($"Ocurrió un error al modificar el producto con ID {id}. El método ModificarProducto devolvió un valor inesperado: {response}");
                        return StatusCode(500, "Error interno del servidor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar el producto con ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            _logger.LogInformation("Inicio del método EliminarProducto");

            try
            {
                var response = await _productoService.EliminarProducto(id);

                switch (response)
                {
                    case ProductResult.Success:
                        _logger.LogInformation("Producto con ID {id} eliminado exitosamente.", id);
                        return Ok("Producto eliminado exitosamente.");
                    case ProductResult.ProductNotFound:
                        _logger.LogWarning($"No existe ningún producto con el ID: {id}.");
                        return NotFound($"No existe ningún producto con el ID: {id}.");
                    default:
                        _logger.LogError($"Error al eliminar el producto con ID {id}");
                        return StatusCode(500, "Error interno del servidor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el producto con ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [Route("Filtrar")]
        public async Task<IActionResult> FiltrarProductos(int presupuesto)
        {
            _logger.LogInformation("Inicio del método FiltrarProductos");

            try
            {
                // Valido que el presupuesto este en el rango correcto, sino es así directamente no accedo al servicio
                if (presupuesto < 1 || presupuesto > 1000000)
                {
                    _logger.LogWarning("El presupuesto debe estar entre 1 y 1,000,000.");
                    return BadRequest("El presupuesto debe estar entre 1 y 1,000,000.");
                }

                var (response, productosFiltrados) = await _productoService.FiltrarProductosPorPresupuesto(presupuesto);

                switch (response)
                {
                    case ProductResult.NoProducts:
                        _logger.LogInformation("No se encontraron productos.");
                        return NotFound("No se encontraron productos.");
                    case ProductResult.NoValidCombination:
                        _logger.LogInformation("No se encontró un producto de cada categoria que se ajuste al presupuesto.");
                        return NotFound("No se encontró un producto de cada categoria que se ajuste al presupuesto.");
                    case ProductResult.Success:
                        return Ok(productosFiltrados);
                    default:
                        _logger.LogError("Error desconocido al filtrar productos por presupuesto");
                        return StatusCode(500, "Error interno del servidor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar productos por presupuesto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
}