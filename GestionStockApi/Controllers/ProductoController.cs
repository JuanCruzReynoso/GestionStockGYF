using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionStockApi.Services;
using GestionStockApi.DTOs;

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
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            _logger.LogInformation("Inicio del método GetProductos");

            try
            {
                var productos = await _productoService.GetProductos();

                if (!productos.Any())
                {
                    _logger.LogInformation("No hay productos disponibles.");
                    return NotFound("No hay productos disponibles.");
                }

                _logger.LogInformation("Productos obtenidos exitosamente.");
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                return StatusCode(500, "Error interno del servidor");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            _logger.LogInformation("Inicio del método GetProducto");

            try
            {
                var producto = await _productoService.GetProducto(id);

                if (producto == null)
                {
                    _logger.LogWarning($"No existe ningun producto con el ID: {id}.");
                    return NotFound($"No existe ningun producto con el ID: {id}.");
                }

                _logger.LogInformation("Producto con ID: {id} obtenido exitosamente.", id);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar obtener el producto con ID: {id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(int idCategoria, decimal precio)
        {
            _logger.LogInformation("Inicio del método CrearProducto");

            try
            {
                // Validar que el precio sea positivo y razonable
                if (precio <= 0 || precio > 1000000)
                {
                    _logger.LogWarning("El precio debe estar entre 1 y 1,000,000.");
                    return BadRequest("El precio debe estar entre 1 y 1,000,000.");
                }

                var producto = await _productoService.CrearProducto(idCategoria, precio);

                if (producto == null)
                {
                    _logger.LogWarning("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                    return BadRequest("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                }

                _logger.LogInformation("Producto creado exitosamente.");
                return Ok("Producto creado exitosamente.");
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
                // Validar que el precio sea positivo
                if (precio <= 0 || precio > 1000000)
                {
                    _logger.LogWarning("El precio debe estar entre 1 y 1,000,000.");
                    return BadRequest("El precio debe estar entre 1 y 1,000,000.");
                }

                var resultado = await _productoService.ModificarProducto(id, idCategoria, precio);

                switch (resultado)
                {
                    case 0:
                        _logger.LogWarning($"No existe ningún producto con el ID: {id}.");
                        return NotFound($"No existe ningún producto con el ID: {id}.");
                    case -1:
                        _logger.LogWarning("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                        return BadRequest("La categoría especificada no es válida. Categorías disponibles: 1 (Computación), 2 (Telefonía)");
                    case > 0:
                        _logger.LogInformation("Producto con ID {id} modificado exitosamente.", id);
                        return Ok("Producto modificado exitosamente.");
                    default:
                        _logger.LogError("Ocurrió un error al modificar el producto con ID {id}. El método ModificarProducto devolvió un valor inesperado: {resultado}", id, resultado);
                        return StatusCode(500, "Error interno del servidor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el producto con ID {id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            _logger.LogInformation("Inicio del método EliminarProducto");

            try
            {
                var eliminado = await _productoService.EliminarProducto(id);

                if (eliminado)
                {
                    _logger.LogInformation("Producto con ID {id} eliminado exitosamente.", id);
                    return Ok("Producto eliminado exitosamente.");
                }
                else
                {
                    _logger.LogWarning($"No existe ningún producto con el ID: {id}.");
                    return NotFound($"No existe ningún producto con el ID: {id}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        [Route("Filtrar")]
        public IActionResult FiltrarProductos(int presupuesto)
        {
            _logger.LogInformation("Inicio del método FiltrarProductos");

            try
            {
                if (presupuesto < 1 || presupuesto > 1000000)
                {
                    _logger.LogWarning("El presupuesto debe estar entre 1 y 1,000,000.");
                    return BadRequest("El presupuesto debe estar entre 1 y 1,000,000.");
                }

                var productosFiltrados = _productoService.FiltrarProductosPorPresupuesto(presupuesto);

                if (productosFiltrados.Count == 0)
                {
                    _logger.LogInformation("No se encontró un producto de cada categoría que cumpla con las condiciones.");
                    return NotFound("No se encontró un producto de cada categoría que cumpla con las condiciones.");
                }

                return Ok(productosFiltrados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar productos por presupuesto");
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
}