# Plataforma de Cursos Online - Assessment Técnico

API REST desarrollada con .NET 10 para la gestión de cursos y lecciones, siguiendo principios de Clean Architecture.

## 🎯 Características

- ✅ Clean Architecture (Domain, Application, Infrastructure, API)
- ✅ Entity Framework Core 10.0 con PostgreSQL
- ✅ Autenticación JWT con Identity
- ✅ Soft Delete en todas las entidades
- ✅ Validación de reglas de negocio
- ✅ Paginación y filtros
- ✅ Swagger/OpenAPI documentación
- ✅ 5 Tests unitarios con xUnit, Moq y FluentAssertions

## 📋 Prerrequisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Node.js 18+](https://nodejs.org/) (para el frontend)

## 🚀 Configuración Inicial

### 1. Clonar el Repositorio

```bash
git clone https://github.com/Camotti/AssestmentTecnico.git
cd AssestmentTecnico
```

### 2. Configurar PostgreSQL

**Opción A: Usar base de datos existente**

Actualizar el archivo `Api/appsettings.json` con tus credenciales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=postgres;Username=postgres;Password=TU_CONTRASEÑA"
  }
}
```

**Opción B: Configurar PostgreSQL desde cero**

```bash
# Instalar PostgreSQL (Ubuntu/Debian)
sudo apt update
sudo apt install postgresql postgresql-contrib

# Iniciar servicio
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Configurar contraseña para usuario postgres
sudo -u postgres psql
ALTER USER postgres PASSWORD 'tu_contraseña';
\q
```

### 3. Aplicar Migraciones

```bash
# Instalar herramienta dotnet-ef (si no está instalada)
dotnet tool install --global dotnet-ef

# Aplicar migraciones
dotnet ef database update --project Infrastructure --startup-project Api
```

### 4. Ejecutar la API

```bash
dotnet run --project Api
```

La API estará disponible en:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger: `https://localhost:5001/swagger`

### 5. Ejecutar el Frontend

```bash
cd frontend
npm install  # Solo la primera vez
npm run dev
```

El frontend estará disponible en:
- `http://localhost:5173`

**Nota:** Asegúrate de que la API esté corriendo antes de usar el frontend.

## 🔐 Credenciales de Usuario de Prueba

Al iniciar la API por primera vez, se crea automáticamente un usuario de prueba:

- **Email:** `test@example.com`
- **Password:** `Test123!`

## 📚 Endpoints de la API

### Autenticación (Sin autenticación requerida)

```http
POST /api/auth/register
POST /api/auth/login
```

### Cursos (Requieren autenticación JWT)

```http
GET    /api/courses/search?q=&status=&page=1&pageSize=10
GET    /api/courses/{id}/summary
POST   /api/courses
PUT    /api/courses/{id}
DELETE /api/courses/{id}
PATCH  /api/courses/{id}/publish
PATCH  /api/courses/{id}/unpublish
```

### Lecciones (Requieren autenticación JWT)

```http
GET    /api/courses/{courseId}/lessons
POST   /api/courses/{courseId}/lessons
PUT    /api/lessons/{id}
DELETE /api/lessons/{id}
PATCH  /api/lessons/{id}/reorder
```

## 🧪 Ejecutar Tests

```bash
dotnet test
```

**Tests implementados:**
1. `PublishCourse_WithLessons_ShouldSucceed`
2. `PublishCourse_WithoutLessons_ShouldFail`
3. `CreateLesson_WithUniqueOrder_ShouldSucceed`
4. `CreateLesson_WithDuplicateOrder_ShouldFail`
5. `DeleteCourse_ShouldBeSoftDelete`

## 📖 Reglas de Negocio

1. **Publicación de Cursos:** Un curso solo puede publicarse si tiene al menos una lección activa (no eliminada).

2. **Orden Único de Lecciones:** El campo `Order` de las lecciones debe ser único dentro del mismo curso.

3. **Soft Delete:** Todas las eliminaciones son lógicas (se marca `IsDeleted = true`), no se eliminan físicamente de la base de datos.

4. **Reordenamiento:** Al reordenar lecciones, se intercambian los valores de `Order` sin generar duplicados.

5. **Endpoint /publish:** Valida que el curso cumpla con las reglas de negocio antes de publicar.

6. **Endpoint /summary:** Retorna información del curso, total de lecciones activas y fecha de última modificación.

## 🏗️ Estructura del Proyecto

```
AssestmentTecnico/
├── Domain/                 # Entidades, interfaces, excepciones
│   ├── Entities/
│   ├── Enums/
│   ├── Exceptions/
│   └── Interfaces/
├── Application/            # Lógica de negocio, DTOs, servicios
│   ├── DTOs/
│   └── Services/
├── Infrastructure/         # EF Core, repositorios, Identity
│   ├── Data/
│   ├── Repositories/
│   ├── Identity/
│   └── Services/
├── Api/                    # Controllers, middleware, configuración
│   ├── Controllers/
│   └── Middleware/
├── Tests/                  # Tests unitarios
└── frontend/               # Aplicación React
    ├── src/
    │   ├── components/     # Login, Courses, Lessons
    │   └── services/       # API service layer
    └── public/
```

## 🔧 Tecnologías Utilizadas

### Backend
- .NET 10
- Entity Framework Core 10.0.1
- PostgreSQL (Npgsql 10.0.0)
- ASP.NET Core Identity
- JWT Bearer Authentication
- Swagger/OpenAPI

### Frontend
- React 18
- Vite 7.3
- React Router DOM 7
- Axios
- CSS3 (Vanilla CSS con gradientes y animaciones)

### Testing
- xUnit 2.9.3
- Moq 4.20.72
- FluentAssertions 7.0.0

## 📝 Notas Adicionales

### Migraciones

Para crear una nueva migración:
```bash
dotnet ef migrations add NombreMigracion --project Infrastructure --startup-project Api
```

Para revertir la última migración:
```bash
dotnet ef migrations remove --project Infrastructure --startup-project Api
```

### Configuración de JWT

El token JWT está configurado en `appsettings.json`:
- **Expiración:** 60 minutos
- **Issuer:** OnlineCoursesApi
- **Audience:** OnlineCoursesClient

### CORS

La API está configurada con política CORS "AllowAll" para desarrollo. En producción, configurar orígenes específicos.

## 🐛 Solución de Problemas

### Error de conexión a PostgreSQL

Si obtienes error de autenticación:
1. Verifica que PostgreSQL esté corriendo: `sudo systemctl status postgresql`
2. Verifica las credenciales en `appsettings.json`
3. Asegúrate de que el usuario tenga permisos en la base de datos

### Error "dotnet-ef not found"

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
# O agregar permanentemente a ~/.bashrc
```

### Compilación fallida

```bash
dotnet clean
dotnet restore
dotnet build
```

## 👥 Autor

Desarrollado como parte del Assessment Técnico para demostrar conocimientos en:
- Clean Architecture
- .NET Core
- Entity Framework Core
- Autenticación JWT
- Testing unitario
- Reglas de negocio

## 📄 Licencia

Este proyecto es parte de un assessment técnico.
