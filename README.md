# AutoTallerManager

Sistema completo de gestión para talleres automotrices con API backend en .NET y frontend web en HTML/CSS/JavaScript.

##  Descripción

`AutoTallerManager` es una solución integral para administración de talleres mecánicos. Incluye una API REST robusta y una interfaz web moderna  para gestionar citas, órdenes de servicio, productos, proveedores, facturas, cotizaciones, vehículos y usuarios.

##  Arquitectura del proyecto

Estructura principal:

- `Api/` - API ASP.NET Core con controladores, filtros, middleware, autenticación JWT, Swagger y SignalR.
- `Application/` - Lógica de negocio, contratos, DTOs, servicios y mapeos.
- `Domain/` - Entidades, enums y objetos de valor del dominio.
- `Infrastructure/` - Acceso a datos, configuración de EF Core, repositorios, servicios y migraciones.
- `Tests/` - Pruebas unitarias e integración.

##  Características principales

- Autenticación y autorización JWT.
- PostgreSQL como base de datos.
- Migraciones EF Core y semilla automática de datos.
- Swagger para documentación y pruebas de API.
- Rate limiting por rutas.
- SignalR para notificaciones en tiempo real.
- Respuesta unificada con filtros y manejo global de errores.

##  Requisitos

### Backend
- .NET SDK 10 
- PostgreSQL 12 o superior

### Frontend
- Navegador web moderno (Chrome, Firefox, Edge, Safari)
- Servidor web local (Live Server, Python, Node.js, etc.)

### General
- Git
- Visual Studio / VS Code o editor compatible

##  Configuración y ejecución

### Backend (.NET)

1. Clonar el repositorio:

```powershell
git clone https://github.com/corredor29/AutoTallerManager_ProyectNET.git
cd AutoTallerManager_ProyectNET
```

2. Crear y configurar la base de datos PostgreSQL:

```sql
CREATE DATABASE AutoTaller;
```

3. Ajustar la cadena de conexión en `Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=AutoTaller;Username=postgres;Password=TuPassword"
}
```

4. Restaurar paquetes NuGet:

```powershell
dotnet restore
```

5. Compilar el proyecto:

```powershell
dotnet build
```

6. Ejecutary flujo de trabajo

### Documentación de API (Swagger)

- `https://localhost:5001/swagger` (o puerto configurado)

### Aplicación web

- `http://localhost:5500` (o puerto configurado)
### Backend (.NET)

- **JWT**: revisa `Api/appsettings.json` en la sección `Jwt`. Cambiar la clave en producción.
- **Rate limiting**: revisa `Api/appsettings.json` en la sección `RateLimiting`.
- **CORS**: la política `AllowFront` permite orígenes locales (localhost:5500, 127.0.0.1:5500, localhost:3000).
- **Database**: Configura la conexión a PostgreSQL en `ConnectionStrings`.

### Frontend (HTML/CSS/JS)

- **API URL**: configura el endpoint de la API en `js/api.js`
- **Autenticación**: los tokens JWT se almacenan en localStorage
- **Notificaciones en tiempo real**: conecta con SignalR hub en `/hubs/notifications`
- Contraseña: `Admin123!` 

### Módulos disponibles en el frontend

-  **Autenticación** - Login y registro de usuarios
-  **Dashboard** - Resumen de actividades
-  **Clientes** - Gestión de clientes y contactos
-  **Vehículos** - Registro y seguimiento de vehículos
-  **Citas** - Programación de citas y mantenimiento
-  **Órdenes de servicio** - Gestión de trabajos y servicios
-  **Partes** - Inventario de repuestos
-  **Órdenes de compra** - Solicitudes a proveedores
-  **Facturas** - Gestión de facturas y cobros
-  **Cotizaciones** - Presupuestos y cotizaciones
-  **Proveedores** - Gestión de proveedores
-  **Usuarios** - Administración de usuarios del sistema
-  **Auditoría** - Registro de cambios del sistema
> Al iniciar, el proyecto migrará automáticamente la base de datos y sembrará datos iniciales como estados, roles, métodos de pago, tipos de servicio y un usuario administrador predeterminado.

La API estará disponible en `https://localhost:5001` (o el puerto configurado).

### Frontend (HTML/CSS/JavaScript)

1. Clonar o descargar el proyecto frontend:

```powershell
git clone https://github.com/corredor29/Proyect.net-Front.git
cd Proyect.net-Front-main
```

### Backend

- `Api/Controllers` - endpoints HTTP REST
- `Api/Middleware` - middleware personalizado (excepciones, logging)
- `Api/Filters` - filtros (respuesta unificada)
- `Infrastructure/Context` - contexto de EF Core
- `Infrastructure/Data` - migraciones y seeder
- `Infrastructure/Repositories` - acceso a datos
- `Application/Contracts` - interfaces de repositorios y servicios
- `Application/Services` - lógica de negocio
- `Domain/Entities` - modelos de dominio
- `Domain/ValueObjects` - objetos de valor

### Frontend

- `index.html` - página de login
- `Dashboard.html` - dashboard principal
- `register.html` - registro de usuarios
- `pages/` - páginas de módulos (clientes, vehículos, citas, etc.)
- `js/api.js` - cliente centralizado para llamadas a API
- `js/*.js` - lógica de cada módulo
- `css/` - estilos (main.css, login.css)


##  Notas importantes

- **Seguridad**: Cambia la clave JWT, contraseña de admin y credenciales de BD en producción
- **CORS**: Ajusta los orígenes permitidos según el despliegue
- **Variables de entorno**: usa `appsettings.Production.json` para no exponer secretos
- **Notificaciones**: SignalR está configurado para notificaciones en tiempo real
- **Rate Limiting**: está habilitado para proteger endpoints críticos


##  Configuración adicional

- JWT: revisa `Api/appsettings.json` en la sección `Jwt`.
- Rate limiting: revisa `Api/appsettings.json` en la sección `RateLimiting`.
- CORS: la política `AllowFront` permite orígenes locales comunes para desarrollo.

##  Pruebas

Para ejecutar las pruebas unitarias:

```powershell
dotnet test Tests\AutoTallerManager.Tests.csproj
```
