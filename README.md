# Eventos · ASP.NET Core MVC

Solución `Eventos.sln`, proyecto `Eventos.Web` (.NET 10, EF Core y SQL Server). La interfaz utiliza una paleta clara de grises y es adaptable a pantallas pequeñas.

## Inicio en Visual Studio

1. Abra `Eventos.sln` con una versión de Visual Studio compatible con .NET 10 y establezca `Eventos.Web` como proyecto de inicio.
2. Revise `Eventos.Web/appsettings.json`. La conexión usa SQL Server LocalDB `(localdb)\mssqllocaldb` y la base `EventosDB`; cambie el servidor si utiliza otra instancia. La cuenta necesita permisos para crear la base.
3. En **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**, seleccione `Eventos.Web` como proyecto predeterminado. Como las migraciones ya están incluidas, ejecute solamente:

   ```powershell
   Update-Database
   ```

4. Inicie la aplicación. Los datos de ejemplo se agregan al arrancar, únicamente cuando cada tabla correspondiente está vacía. La base debe estar migrada antes de iniciar la aplicación.

## Migraciones y evidencia

Se generaron y aplicaron en orden `InitialCreate`, `AgregarModalidadEvento` y `AgregarTelefonoAsistente`. Para un proyecto nuevo, los comandos equivalentes en la consola de Visual Studio son:

```powershell
Add-Migration InitialCreate
Update-Database
# Agregar Modalidad al modelo Evento.
Add-Migration AgregarModalidadEvento
# Antes de aplicar, cambiar defaultValue a "Presencial" en la migración generada.
Update-Database
# Agregar Telefono al modelo Asistente.
Add-Migration AgregarTelefonoAsistente
Update-Database
```

**No vuelva a ejecutar `Add-Migration` en esta solución:** los tres archivos de migración ya existen. En otra base vacía basta con `Update-Database`. Para la evidencia, abra la instancia LocalDB en SQL Server Management Studio, seleccione `EventosDB` y ejecute:

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Capture las tres filas y, en Visual Studio, la carpeta `Migrations` expandida, incluido `EventosContextModelSnapshot.cs`. La consulta es únicamente para inspección; no se necesita SQL para crear ni modificar la base.

## Observaciones y conclusiones

- Code First describe el modelo en C# y las migraciones registran la evolución del esquema de manera reproducible. `__EFMigrationsHistory` permite comprobar qué cambios se aplicaron.
- Las anotaciones expresan restricciones e índices; la validación del formulario complementa, pero no reemplaza, las restricciones únicas de la base.
- Los repositorios separan LINQ y persistencia de los controladores. Las lecturas sin seguimiento reducen trabajo del contexto; las operaciones de modificación sí usan seguimiento.
- La desactivación lógica y la cancelación preservan el historial. Las validaciones de inscripciones comprueban fecha, duplicados y aforo; una transacción serializable protege estas comprobaciones frente a solicitudes concurrentes, aunque pueden producirse conflictos transitorios que requieran reintento.
- MVC, vistas tipadas, validación de modelo, antifalsificación y Post/Redirect/Get separan responsabilidades y mejoran la experiencia y seguridad del formulario.
