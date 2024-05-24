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
            //Creo la informacion del usuario para token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre.ToString()),
            };

            // Obtenengo la clave de seguridad de appsettings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Creo detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            // Genero y devuelvp el token JWT
            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

        public async Task<AuthResult> SignUp(Usuario usuario)
        {
            // Verifico si el usuario ya existe
            var usuarioExistente = await _dbContext.Usuarios
                .FirstOrDefaultAsync(u => u.Nombre == usuario.Nombre);

            if (usuarioExistente != null)
            {
                return AuthResult.UserAlreadyExists;
            }

            // Creo un nuevo usuario y encripto la contraseña
            var nuevoUsuario = new Usuario
            {
                Nombre = usuario.Nombre,
                Contraseña = Encriptador.Encriptar(usuario.Contraseña)
            };

            // Guardo el usuario en la base de datos
            _dbContext.Usuarios.Add(nuevoUsuario);
            await _dbContext.SaveChangesAsync();

            return AuthResult.Success;
        }

        public async Task<(AuthResult result, string token)> Login(Usuario usuario)
        {
            // Primero valido que el usuario exista
            var usuarioEncontrado = await _dbContext.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Nombre == usuario.Nombre);

            if (usuarioEncontrado == null)
            {
                return (AuthResult.UserNotFound, string.Empty);
            }

            // Encripto la contraseña ingresada
            var contraseñaEncriptada = Encriptador.Encriptar(usuario.Contraseña);

            // Luego valido la contraseña 
            if (usuarioEncontrado.Contraseña != contraseñaEncriptada)
            {
                return (AuthResult.IncorrectPassword, String.Empty);
            }

            // Si las credenciales son correctas genero y devuelvo el token JWT
            var token = GenerarToken(usuarioEncontrado);

            return (AuthResult.Success, token);
        }

    }
}
