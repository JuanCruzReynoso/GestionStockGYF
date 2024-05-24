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
            // Valido que el formato sea correcto
            if (usuario == null)
            {
                _logger.LogWarning("SignUp: Usuario es nulo.");
                return BadRequest("El usuario es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Nombre))
            {
                _logger.LogWarning("SignUp: El nombre de usuario es nulo o vacío.");
                return BadRequest(" El nombre de usuario es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Contraseña))
            {
                _logger.LogWarning("SignUp: La contraseña es nula o vacía.");
                return BadRequest("La contraseña es requerida.");
            }

            if (usuario.Nombre == usuario.Contraseña)
            {
                _logger.LogWarning("SignUp: El nombre de usuario no puede ser igual a la contraseña.");
                return BadRequest("El nombre de usuario no puede ser igual a la contraseña.");
            }

            var response = await _authService.SignUp(usuario);

            switch (response)
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
            // Valido que el formato sea correcto
            if (usuario == null)
            {
                _logger.LogWarning("Login: El usuario es nulo.");
                return BadRequest("El usuario es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Nombre))
            {
                _logger.LogWarning("Login: El nombre de usuario es nulo o vacío.");
                return BadRequest("El nombre es requerido.");
            }

            if (string.IsNullOrEmpty(usuario.Contraseña))
            {
                _logger.LogWarning("Login: La contraseña es nula o vacía.");
                return BadRequest("La contraseña es requerida.");
            }

            var (response, token) = await _authService.Login(usuario);

            switch (response)
            {
                case AuthResult.Success:
                    _logger.LogInformation("Login: Inicio de sesión exitoso.");
                    return Ok(new { Token = token });
                case AuthResult.UserNotFound:
                    _logger.LogWarning("Login: El usuario no existe.");
                    return NotFound("El usuario no existe.");
                case AuthResult.IncorrectPassword:
                    _logger.LogWarning("Login: La contraseña es incorrecta.");
                    return NotFound("La contraseña es incorrecta.");
                default:
                    _logger.LogError("Login: Error desconocido.");
                    return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
