using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionStockApi.Models;
using GestionStockApi.Uitilities;
using GestionStockApi.Enums;


namespace GestionStockApi.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignUp(Usuario usuario);
        Task<(AuthResult result, string token)> Login(Usuario usuario);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly GestionStockDbContext _dbContext;

        public AuthService(IConfiguration configuration, GestionStockDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        private string GenerarToken(Usuario usuario)
        {
            //Crea la informacion del usuario para token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre.ToString()),
            };

            // Obtene la clave de seguridad de appsettings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Crea detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            // Genera y devuelve el token JWT
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        public async Task<AuthResult> SignUp(Usuario usuario)
        {
            // Verifica si el usuario ya existe
            var usuarioExistente = await _dbContext.Usuarios
                .FirstOrDefaultAsync(u => u.Nombre == usuario.Nombre);

            if (usuarioExistente != null)
            {
                return AuthResult.UserAlreadyExists;
            }

            // Crea un nuevo usuario y encripta la contraseña
            var nuevoUsuario = new Usuario
            {
                Nombre = usuario.Nombre,
                Contraseña = Encriptador.Encriptar(usuario.Contraseña)
            };

            // Guarda el usuario en la base de datos
            _dbContext.Usuarios.Add(nuevoUsuario);
            await _dbContext.SaveChangesAsync();

            return AuthResult.Success;
        }

        public async Task<(AuthResult result, string token)> Login(Usuario usuario)
        {
            // Primero valida nombre de usuario
            var usuarioEncontrado = await _dbContext.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Nombre == usuario.Nombre);

            if (usuarioEncontrado == null)
            {
                return (AuthResult.UserNotFound, string.Empty);
            }

            // Encripta la contraseña ingresada
            var contraseñaEncriptada = Encriptador.Encriptar(usuario.Contraseña);

            // Luego valida contraseña 
            if (usuarioEncontrado.Contraseña != contraseñaEncriptada)
            {
                return (AuthResult.IncorrectPassword, String.Empty);
            }

            // Si las credenciales son correctas genera el token JWT
            var token = GenerarToken(usuarioEncontrado);

            return (AuthResult.Success, token);
        }

    }
}
