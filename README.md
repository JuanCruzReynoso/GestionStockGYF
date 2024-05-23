## Bienvenido a GestionStockAPI

¡Hola! Soy Juan Cruz Reynoso, y te doy la bienvenida a mi solución para el challenge de gestión de stock.

### Descripción del Proyecto

Este proyecto consiste en una web API desarrollada en .NET 8 que ofrece funcionalidades para la gestión de stock de un comercio que vende productos de dos categorías específicas. Utiliza Entity Framework Core y SQL Server como base de datos.

### Estructura del Repositorio

En este repositorio encontrarás los siguientes componentes:

- **GestionStockApi:** Proyecto Web API, contiene .sln para iniciar el proyecto.
  
- **GestionStockApi.Tests:** Proyecto xUnit, incluye pruebas unitarias para SignUp y Login en AuthController y ServiceController.

- **GestionStockDB.sql:** Este archivo contiene el script SQL para crear la base de datos necesaria para la aplicación,tambien inserté 2 categorias y los 5 productos dados en el ejemplo de filtrado.

### Instrucciones de Uso

1. Ejecutar los scripts en el archivo GestionStockDB.sql para crear la base de datos y sus tablas, así como insertar los datos iniciales de categorías y productos.

2. Antes de ejecutar la API, asegúrate de reemplazar la cadena de conexión en el archivo `appsettings.json` con los detalles de tu servidor SQL. La cadena de conexión se encuentra bajo la clave `"GestionStockDB"`.
   
   Ejemplo de cadena de conexión:
   ```json
   "GestionStockDB": "Server=JUANOSK8\\SQLEXPRESS;Database=GestionStockDB;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True;"
   ```

3. Una vez configurada la cadena de conexión, podes ejecutar la API y acceder a los diferentes endpoints en Swagger.

### Endpoints Disponibles

- **Autenticación de Usuario:** Permite autenticar usuarios y obtener un token JWT para acceder a los otros endpoints de la API, configuré swagger para poder cargar el token (Ejemplo formato: bearer JwaSDJLE232).
  
- **ABM de Productos:** Proporciona endpoints CRUD para la gestión de productos, incluyendo la creación, lectura, actualización y eliminación de productos.

- **Obtención Filtrada de Productos:** Permite obtener una lista de productos filtrados según un presupuesto especificado, cumpliendo las condiciones del challenge.

### Conclusión

Gracias por considerar mi solución para el challenge! Si tenes alguna pregunta o necesitas más información, no dudes en contactarme.

Saludos cordiales, Juan Cruz Reynoso.