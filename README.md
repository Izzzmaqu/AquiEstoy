# Aquí Estoy

> Plataforma web para la identificación temprana de riesgos, seguimiento de casos y comunicación segura entre usuarios y profesionales de salud mental.

> Proyecto académico desarrollado para el curso de Arquitectura del Software de la Universidad Internacional de las Américas, Costa Rica.

## Aviso importante

Este proyecto es un prototipo académico creado exclusivamente con fines educativos.

No es un sistema clínico, servicio de emergencias ni sustituto de atención profesional en salud mental. No debe utilizarse para diagnosticar, evaluar ni gestionar situaciones reales de crisis.

Ante una emergencia o situación de riesgo inmediato, se debe contactar a los servicios de emergencia y líneas oficiales de atención correspondientes.

## Descripción

**Aquí Estoy** es una plataforma web orientada a la prevención del suicidio y al acompañamiento oportuno de personas en condición de vulnerabilidad.

El sistema centraliza el registro y seguimiento de casos, la gestión de factores de riesgo, la asignación de profesionales, la comunicación segura en tiempo real, la generación de alertas automáticas, la consulta de líneas de ayuda y el análisis estadístico para el nivel administrativo.

## Funcionalidades principales

* Registro y seguimiento de casos
* Gestión de factores de riesgo y niveles de severidad
* Asignación de casos a profesionales
* Chat privado entre usuario y profesional
* Comunicación en tiempo real mediante SignalR
* Generación de alertas automáticas de riesgo
* Directorio de líneas de ayuda
* Panel administrativo con análisis estadístico
* Historial de cambios de severidad
* Bitácora de auditoría de acciones relevantes
* Gestión de documentos o adjuntos asociados a un caso

## Arquitectura

La solución sigue principios de **Clean Architecture**, separando las responsabilidades del dominio, los casos de uso, la infraestructura y las capas de presentación.

```text
AquiEstou.API
        |
AquiEstou.Application
        |
AquiEstou.Domain
        ^
        |
AquiEstou.Infrastructure

AquiEstou.Web
        |
AquiEstou.API
```

| Proyecto | Responsabilidad |
|---|---|
| `AquiEstou.Domain` | Entidades centrales, reglas de negocio y contratos del dominio |
| `AquiEstou.Application` | Casos de uso, DTOs, interfaces, validaciones y lógica de aplicación |
| `AquiEstou.Infrastructure` | Persistencia con EF Core, acceso a SQL Server, repositorios e integraciones externas |
| `AquiEstou.API` | Endpoints REST, inyección de dependencias, autenticación y hubs de SignalR |
| `AquiEstou.Web` | Interfaz de usuario desarrollada con ASP.NET Core MVC |

## Tecnologías utilizadas

| Área | Tecnología |
|---|---|
| Backend | ASP.NET Core |
| Frontend | ASP.NET Core MVC |
| Arquitectura | Clean Architecture y arquitectura en capas |
| ORM | Entity Framework Core |
| Base de datos | SQL Server |
| Comunicación en tiempo real | SignalR |
| Lenguaje | C# |
| Control de versiones | Git y GitHub |
| Modelado visual de BD | MySQL Workbench |

## Diseño de base de datos

La base de datos principal se implementa en **SQL Server** mediante el enfoque Code First de Entity Framework Core y migraciones.

El modelo incorpora entidades y catálogos para:

* Usuarios, roles, profesionales y especialidades
* Casos, estados, niveles de severidad y factores de riesgo
* Alertas automáticas y líneas de ayuda
* Conversaciones, mensajes y sesiones de chat
* Provincias y cantones para análisis geográfico
* Adjuntos, auditoría y reportes estadísticos

MySQL Workbench se utiliza únicamente para visualizar y compartir el diagrama entidad relación. La aplicación utilizará SQL Server como motor de base de datos.

## Estructura del proyecto

```text
Aqui-Estoy/
|
|-- AquiEstou.API/
|-- AquiEstou.Application/
|-- AquiEstou.Domain/
|-- AquiEstou.Infrastructure/
|-- AquiEstou.Web/
```

## Requisitos previos

* .NET SDK 8.0 o superior
* SQL Server Express, LocalDB o SQL Server Developer Edition
* Visual Studio 2022 o Visual Studio Code
* Herramientas de Entity Framework Core

## Instalación y ejecución

1. Clona el repositorio:

```bash
git clone https://github.com/TU_USUARIO/Aqui-Estoy.git
cd Aqui-Estoy
```

2. Configura la cadena de conexión de SQL Server en `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=AquiEstoyDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. Restaura las dependencias:

```bash
dotnet restore
```

4. Aplica las migraciones de Entity Framework Core:

```bash
dotnet ef database update --project AquiEstou.Infrastructure --startup-project AquiEstou.API
```

5. Ejecuta la API:

```bash
dotnet run --project AquiEstou.API
```

## Módulos del equipo

| Área | Alcance principal |
|---|---|
| Arquitectura y casos | Arquitectura en capas, diseño de BD, registro de casos y factores de riesgo |
| Comunicación segura | Chat individual entre usuario y profesional mediante SignalR |
| Respuesta ante riesgo | Alertas automáticas e integración de líneas de ayuda |
| Análisis e interfaz | Panel estadístico y diseño de pantallas |

## Estado actual

El proyecto se encuentra en desarrollo académico.

Actualmente se ha creado la estructura inicial de la solución, la arquitectura en capas, el modelo de base de datos y se trabaja en el módulo de registro de casos y factores de riesgo.

## Privacidad y seguridad

Debido a que el sistema maneja información altamente sensible, una versión de producción requeriría controles adicionales, entre ellos:

* Autenticación segura y autorización basada en roles
* Cifrado de datos en tránsito y en reposo
* Gestión de consentimiento y minimización de datos
* Restricción de acceso de profesionales a casos asignados
* Bitácoras de auditoría y políticas de retención de información
* Revisión clínica, legal y de ciberseguridad


Proyecto desarrollado para el curso de Arquitectura del Software de la Universidad Internacional de las Américas.
