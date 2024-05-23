using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using GestionStockApi.Enums;
using GestionStockApi.Models;
using GestionStockApi.Services;
using GestionStockApi.Uitilities;

namespace GestionStockApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly DbContextOptions<GestionStockDbContext> _dbContextOptions;

        public AuthServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Jwt:key"]).Returns("B2791522-30DF-4D17-AAD9-87E3D1AB0BDB-8991FF7890EEAPP7");

            _dbContextOptions = new DbContextOptionsBuilder<GestionStockDbContext>()
                .UseInMemoryDatabase(databaseName: "GestionStockDb")
                .Options;
        }

        [Fact]
        public async Task SignUp_ReturnsUserAlreadyExists_WhenUserAlreadyExists()
        {
            using (var context = new GestionStockDbContext(_dbContextOptions))
            {
                context.Usuarios.Add(new Usuario { Nombre = "usuario1", Contraseña = "password1" });
                await context.SaveChangesAsync();

                var authService = new AuthService(_configurationMock.Object, context);
                var result = await authService.SignUp(new Usuario { Nombre = "usuario1", Contraseña = "password2" });

                Assert.Equal(AuthResult.UserAlreadyExists, result);
            }
        }

        [Fact]
        public async Task SignUp_ReturnsSuccess_WhenUserIsSuccessfullyRegistered()
        {
            using (var context = new GestionStockDbContext(_dbContextOptions))
            {
                var authService = new AuthService(_configurationMock.Object, context);
                var result = await authService.SignUp(new Usuario { Nombre = "usuario2", Contraseña = "password2" });

                Assert.Equal(AuthResult.Success, result);

                var usuarioRegistrado = await context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == "usuario2");
                Assert.NotNull(usuarioRegistrado);
            }
        }

        [Fact]
        public async Task Login_ReturnsUserNotFound_WhenUserDoesNotExist()
        {
            using (var context = new GestionStockDbContext(_dbContextOptions))
            {
                var authService = new AuthService(_configurationMock.Object, context);
                var (result, token) = await authService.Login(new Usuario { Nombre = "usuario3", Contraseña = "password3" });

                Assert.Equal(AuthResult.UserNotFound, result);
                Assert.True(string.IsNullOrEmpty(token));
            }
        }

        [Fact]
        public async Task Login_ReturnsIncorrectPassword_WhenPasswordIsIncorrect()
        {
            using (var context = new GestionStockDbContext(_dbContextOptions))
            {
                context.Usuarios.Add(new Usuario { Nombre = "usuario4", Contraseña = Encriptador.Encriptar("password4") });
                await context.SaveChangesAsync();

                var authService = new AuthService(_configurationMock.Object, context);
                var (result, token) = await authService.Login(new Usuario { Nombre = "usuario4", Contraseña = "wrongpassword" });

                Assert.Equal(AuthResult.IncorrectPassword, result);
                Assert.True(string.IsNullOrEmpty(token));
            }
        }

        [Fact]
        public async Task Login_ReturnsSuccessAndToken_WhenLoginIsSuccessful()
        {
            using (var context = new GestionStockDbContext(_dbContextOptions))
            {
                context.Usuarios.Add(new Usuario { Nombre = "usuario5", Contraseña = Encriptador.Encriptar("password5") });
                await context.SaveChangesAsync();

                var authService = new AuthService(_configurationMock.Object, context);
                var (result, token) = await authService.Login(new Usuario { Nombre = "usuario5", Contraseña = "password5" });

                Assert.Equal(AuthResult.Success, result);
                Assert.False(string.IsNullOrEmpty(token));
            }
        }
    }
}
