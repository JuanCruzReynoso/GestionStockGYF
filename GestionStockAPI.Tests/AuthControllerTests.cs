using GestionStockApi.Controllers;
using GestionStockApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using GestionStockApi.Services;
using GestionStockApi.Enums;

namespace GestionStockApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenUsuarioIsNull()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.SignUp(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenNombreIsNullOrEmpty()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.SignUp(new Usuario { Contraseña = "123456" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre es requerido.", badRequestResult.Value);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenContraseñaIsNullOrEmpty()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.SignUp(new Usuario { Nombre = "usuario1" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("La contraseña es requerida.", badRequestResult.Value);
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenUsuarioAlreadyExists()
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.SignUp(It.IsAny<Usuario>())).ReturnsAsync(AuthResult.UserAlreadyExists);

            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.SignUp(new Usuario { Nombre = "usuario1", Contraseña = "123456" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre de usuario ya está en uso.", badRequestResult.Value);
        }

        [Fact]
        public async Task SignUp_ReturnsOk_WhenUsuarioIsSuccessfullyRegistered()
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.SignUp(It.IsAny<Usuario>())).ReturnsAsync(AuthResult.Success);

            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.SignUp(new Usuario { Nombre = "usuario1", Contraseña = "123456" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuario registrado exitosamente.", okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUsuarioIsNull()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El usuario es requerido.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenNombreIsNullOrEmpty()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(new Usuario { Contraseña = "123456" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre es requerido.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenContraseñaIsNullOrEmpty()
        {
            var authServiceMock = new Mock<IAuthService>();
            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(new Usuario { Nombre = "usuario1" });

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("La contraseña es requerida.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenUsuarioIsNotFound()
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.Login(It.IsAny<Usuario>())).ReturnsAsync((AuthResult.UserNotFound, string.Empty));

            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(new Usuario { Nombre = "usuario1", Contraseña = "123456" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no existe.", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsNotFound_WhenContraseñaIsIncorrect()
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.Login(It.IsAny<Usuario>())).ReturnsAsync((AuthResult.IncorrectPassword, string.Empty));

            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(new Usuario { Nombre = "usuario1", Contraseña = "123456" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Contraseña incorrecta.", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.Login(It.IsAny<Usuario>())).ReturnsAsync((AuthResult.Success, "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZ"));

            var loggerMock = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(authServiceMock.Object, loggerMock.Object);

            var result = await controller.Login(new Usuario { Nombre = "usuario1", Contraseña = "123456" });

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = okResult.Value;
            var tokenProperty = value.GetType().GetProperty("Token");
            Assert.NotNull(tokenProperty);

            var tokenValue = tokenProperty.GetValue(value) as string;
            Assert.Equal("eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZ", tokenValue);
        }
    }

}

