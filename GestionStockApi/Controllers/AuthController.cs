using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionStockApi.Models;
using GestionStockApi.Services;
using GestionStockApi.Enums;

namespace GestionStockApi.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(Usuario usuario)
        {
            if (usuario == null)
            {
                _logger.LogWarning("SignUp: Usuario es nulo.");
                return BadRequest("El usuario es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Nombre))
            {
                _logger.LogWarning("SignUp: Nombre es nulo o vacío.");
                return BadRequest("El nombre es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Contraseña))
            {
                _logger.LogWarning("SignUp: Contraseña es nula o vacía.");
                return BadRequest("La contraseña es requerida.");
            }

            var result = await _authService.SignUp(usuario);

            switch (result)
            {
                case AuthResult.Success:
                    _logger.LogInformation("SignUp: Usuario registrado exitosamente.");
                    return Ok("Usuario registrado exitosamente.");
                case AuthResult.UserAlreadyExists:
                    _logger.LogWarning("SignUp: El nombre de usuario ya está en uso.");
                    return BadRequest("El nombre de usuario ya está en uso.");
                default:
                    _logger.LogError("SignUp: Error desconocido.");
                    return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            if (usuario == null)
            {
                _logger.LogWarning("Login: Usuario es nulo.");
                return BadRequest("El usuario es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Nombre))
            {
                _logger.LogWarning("Login: Nombre es nulo o vacío.");
                return BadRequest("El nombre es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Contraseña))
            {
                _logger.LogWarning("Login: Contraseña es nula o vacía.");
                return BadRequest("La contraseña es requerida.");
            }

            var (result, token) = await _authService.Login(usuario);

            switch (result)
            {
                case AuthResult.Success:
                    _logger.LogInformation("Login: Inicio de sesión exitoso.");
                    return Ok(new { Token = token });
                case AuthResult.UserNotFound:
                    _logger.LogWarning("Login: Usuario no existe.");
                    return NotFound("Usuario no existe.");
                case AuthResult.IncorrectPassword:
                    _logger.LogWarning("Login: Contraseña incorrecta.");
                    return NotFound("Contraseña incorrecta.");
                default:
                    _logger.LogError("Login: Error desconocido.");
                    return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
