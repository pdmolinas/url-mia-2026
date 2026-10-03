using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

// Cargar variables desde .env si existe
string envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (!File.Exists(envPath))
{
    envPath = Path.Combine(AppContext.BaseDirectory, ".env");
}

if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;
        var parts = trimmed.Split('=', 2);
        if (parts.Length == 2)
        {
            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }
}

// =========================================================================
// Encabezado y configuración de conexión a Azure Blob Storage
// =========================================================================
string connectionString =
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING_GH") ??
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING_P") ??
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING_L") ??
    "";

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("[ERROR] No se encontró la variable de conexión.");
    Console.WriteLine("Asegúrate de tener configurada AZURE_STORAGE_CONNECTION_STRING en tu .env local o AZURE_STORAGE_CONNECTION_STRING_GH en GitHub Actions.");
    return;
}

string containerName = "miaarchivos";

BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

// Asegurar que el contenedor exista
await containerClient.CreateIfNotExistsAsync();

// =========================================================================
// Menú principal y operaciones (Laboratorio No. 2 Semana #12)
// =========================================================================
bool salir = false;

while (!salir)
{
    try { Console.Clear(); } catch { }
    Console.WriteLine("=================================");
    Console.WriteLine("     MIA - AZURE BLOB STORAGE    ");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Subir archivo");
    Console.WriteLine("2. Listar archivos");
    Console.WriteLine("3. Descargar archivo");
    Console.WriteLine("4. Eliminar archivo");
    Console.WriteLine("5. Salir");
    Console.WriteLine("=================================");
    Console.Write("Seleccione una opción: ");

    string? opcion = Console.ReadLine()?.Trim();
    if (opcion == null)
    {
        salir = true;
        break;
    }
    Console.WriteLine();

    try
    {
        switch (opcion)
        {
            case "1":
                await SubirArchivoAsync(containerClient);
                break;
            case "2":
                await ListarArchivosAsync(containerClient);
                break;
            case "3":
                await DescargarArchivoAsync(containerClient);
                break;
            case "4":
                await EliminarArchivoAsync(containerClient);
                break;
            case "5":
                salir = true;
                Console.WriteLine("Saliendo del programa. ¡Hasta pronto!");
                break;
            default:
                Console.WriteLine("Opción no válida. Por favor, seleccione una opción del 1 al 5.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Se produjo un error en la operación: {ex.Message}");
    }

    if (!salir)
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        if (Console.IsInputRedirected)
        {
            Console.ReadLine();
        }
        else
        {
            Console.ReadKey();
        }
    }
}

// =========================================================================
// Métodos de operaciones funcionales
// =========================================================================

// 3.2 Subir archivo
static async Task SubirArchivoAsync(BlobContainerClient containerClient)
{
    Console.WriteLine("--- SUBIR ARCHIVO ---");
    Console.Write("Ingrese la ruta completa del archivo local a subir: ");
    string? rutaLocal = Console.ReadLine()?.Trim('"', ' ', '\'');

    if (string.IsNullOrWhiteSpace(rutaLocal) || !File.Exists(rutaLocal))
    {
        Console.WriteLine("Error: El archivo especificado no existe o la ruta no es válida.");
        return;
    }

    string nombreArchivo = Path.GetFileName(rutaLocal);
    BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

    Console.WriteLine($"Subiendo '{nombreArchivo}' al contenedor '{containerClient.Name}'...");

    using (FileStream uploadFileStream = File.OpenRead(rutaLocal))
    {
        await blobClient.UploadAsync(uploadFileStream, overwrite: true);
    }

    Console.WriteLine($"¡Archivo '{nombreArchivo}' subido con éxito a Azure Blob Storage!");
}

// 3.3 Listar archivos
static async Task ListarArchivosAsync(BlobContainerClient containerClient)
{
    Console.WriteLine("--- LISTADO DE ARCHIVOS ---");
    Console.WriteLine("{0,-30} {1,15}", "Nombre", "Tamaño");
    Console.WriteLine(new string('-', 47));

    int count = 0;
    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        count++;
        long tamano = blobItem.Properties.ContentLength ?? 0;
        Console.WriteLine("{0,-30} {1,9} bytes", blobItem.Name, tamano);
    }

    if (count == 0)
    {
        Console.WriteLine("No se encontraron archivos en el contenedor.");
    }
    else
    {
        Console.WriteLine(new string('-', 47));
        Console.WriteLine($"Total de archivos: {count}");
    }
}

// 3.4 Descargar archivo
static async Task DescargarArchivoAsync(BlobContainerClient containerClient)
{
    Console.WriteLine("--- DESCARGAR ARCHIVO ---");
    Console.Write("Ingrese el nombre del blob en Azure: ");
    string? nombreBlob = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Error: Debe ingresar un nombre de blob válido.");
        return;
    }

    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine($"Error: El blob '{nombreBlob}' no existe en el contenedor.");
        return;
    }

    Console.Write("Ingrese la ruta de la carpeta de destino: ");
    string? carpetaDestino = Console.ReadLine()?.Trim('"', ' ', '\'');

    if (string.IsNullOrWhiteSpace(carpetaDestino))
    {
        Console.WriteLine("Error: La carpeta de destino no puede estar vacía.");
        return;
    }

    if (!Directory.Exists(carpetaDestino))
    {
        Directory.CreateDirectory(carpetaDestino);
        Console.WriteLine($"Directorio creado: {carpetaDestino}");
    }

    string rutaDescarga = Path.Combine(carpetaDestino, nombreBlob);

    Console.WriteLine($"Descargando '{nombreBlob}'...");
    await blobClient.DownloadToAsync(rutaDescarga);

    Console.WriteLine("¡Descarga completada exitosamente!");
    Console.WriteLine($"Ubicación donde fue guardado: {Path.GetFullPath(rutaDescarga)}");
}

// 3.5 Eliminar archivo
static async Task EliminarArchivoAsync(BlobContainerClient containerClient)
{
    Console.WriteLine("--- ELIMINAR ARCHIVO ---");
    Console.Write("Ingrese el nombre del blob a eliminar: ");
    string? nombreBlob = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("Error: Debe ingresar un nombre de blob válido.");
        return;
    }

    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine($"Error: El blob '{nombreBlob}' no existe en el contenedor.");
        return;
    }

    Console.Write($"¿Está seguro que desea eliminar '{nombreBlob}' de Azure? (s/n): ");
    string? confirmacion = Console.ReadLine()?.Trim().ToLower();

    if (confirmacion == "s" || confirmacion == "si" || confirmacion == "sí")
    {
        await blobClient.DeleteIfExistsAsync();
        Console.WriteLine($"El archivo '{nombreBlob}' ha sido eliminado exitosamente de Azure.");
    }
    else
    {
        Console.WriteLine("Operación de eliminación cancelada por el usuario.");
    }
}
