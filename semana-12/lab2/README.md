**MIA - Laboratorio No. 2 Semana #12: Azure Blob Storage (Cloud Storage)**  
   
 **Estudiante:** Pablo David Molina Son  
   
    
   
  **Carné:** 1338825  
   
    
   
  **Curso:** Manejo e Implementación de Archivos (MIA)  
   
    
   
  **Proyecto:** MIA_AzureBlob_PM  
   
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSPBCj7fFC6wwIgHRiywEZJWQZeZ2ao9AAD+4lyruzq+ngAA8Nr1AOIABebqJIqXAAAAAElFTkSuQmCC)  
   
 **1. Objetivo de la Aplicación**  
   
 El objetivo de esta práctica es desarrollar una aplicación de consola en **C# (.NET 10.0)** capaz de conectarse a   **Microsoft Azure Blob Storage** mediante una cadena de conexión (*Connection String*), gestionando operaciones esenciales de almacenamiento de datos no estructurados en la nube (Cloud Storage):  
- Subir archivos locales a la nube.  
- Listar los archivos (blobs) almacenados en el contenedor junto con su tamaño.  
- Descargar archivos desde el contenedor hacia un directorio local específico.  
- Eliminar archivos en la nube previa confirmación del usuario.  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAM0lEQVR4nO3OMQ0AIAwAwZKQ+sBphWAOJywYYCIkd9OP36pqRMQMAAB+sfqJfLoBAMCN3NYsAzBtPw8YAAAAAElFTkSuQmCC)  
 **2. Tecnologías Utilizadas**  
- **Lenguaje:** C# 13  
- **Plataforma:** .NET 10.0 (Aplicación de consola)  
- **SDK de Azure:**Azure.Storage.Blobs (versión 12.29.2), biblioteca oficial de Microsoft para interactuar con Azure Blob Storage  
- **Servicio en la Nube:** Microsoft Azure Blob Storage (Storage Account: miastorage26)  
- **Control de Versiones y Entorno:** Git, Linux / Bash, .NET CLI (dotnet)  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSNhYEUALhD4K0LxgQU2QtIq6DIzR3UFAMBf3Gu1VefXEwAAXtsfSp4DXi4fyswAAAAASUVORK5CYII=)  
 **3. Configuración de Azure**  
   
 Para la realización del laboratorio se efectuaron las siguientes configuraciones dentro del Portal de Azure:  
1. **Storage Account (Cuenta de almacenamiento):**  
- Nombre asignado: miastorage26  
- Rendimiento: Estándar  
- Redundancia: LRS (Almacenamiento con redundancia local)  
1. **Blob Container (Contenedor de Blobs):**  
- Nombre del contenedor: miaarchivos  
- *Nota técnica sobre nomenclatura:* Las políticas del DNS de Azure Blob Storage restringen los nombres a caracteres alfanuméricos en minúsculas y guiones medios (-), prohibiendo guiones bajos (_). Por ello, se utilizó miaarchivos en cumplimiento con las especificaciones.  
- **Nivel de acceso público (*****Public access level*****):** **Privado (*****Private / No anonymous access*****)**, garantizando que ninguna entidad externa pueda leer o listar el contenedor sin credenciales autenticadas.  
1. **Obtención de Credenciales:**  
- Desde el Azure Portal: *Storage Account* →   *Security + networking* →   *Access keys*.  
- Se copió la **Connection String** correspondiente a la clave primaria (*Key 1*).  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSPBCUbfDbIwwIAABiywEZJWQZeZ2ao9AAD+4liruzq/ngAA8Nr1ABwiBgererhLAAAAAElFTkSuQmCC)  
 **4. Arquitectura de la Solución**  
   
 La solución sigue una arquitectura cliente-servicio estructurada en tres niveles provistos por el SDK oficial de Azure Storage:  
   
 +-----------------------------------------------------------------------+  
   
  |                         Consola C# (.NET 10)                          |  
   
  |                                                                       |  
   
  |  [Menú Interactivo] ---> Subir | Listar | Descargar | Eliminar        |  
   
  +-----------------------------------------------------------------------+  
   
                                    |  
   
                                    v  
   
  +-----------------------------------------------------------------------+  
   
  |                    Azure.Storage.Blobs SDK (C#)                       |  
   
  |                                                                       |  
   
  |  1. BlobServiceClient       (Nivel cuenta: autenticación y conexión)  |  
   
  |  2. BlobContainerClient    (Nivel contenedor: 'miaarchivos')         |  
   
  |  3. BlobClient             (Nivel archivo: operaciones individuales) |  
   
  +-----------------------------------------------------------------------+  
   
                                    |  
   
                             HTTPS (REST API)  
   
                                    v  
   
  +-----------------------------------------------------------------------+  
   
  |                        Microsoft Azure Cloud                          |  
   
  |   Storage Account: miastorage26 / Container: miaarchivos              |  
   
  +-----------------------------------------------------------------------+  
   
    
 **Componentes principales del código:**  
- **BlobServiceClient**: Representa la conexión a la cuenta de almacenamiento en Azure utilizando la *Connection String*.  
- **BlobContainerClient**: Administra el contenedor miaarchivos. Valida su existencia o lo crea si no existe (CreateIfNotExistsAsync).  
- **BlobClient**: Permite interactuar con un blob en específico para la lectura, escritura, metadatos y eliminación.  
- **Top-Level Program & Modular Functions**: La aplicación organiza el flujo de ejecución en un bucle principal con funciones asíncronas (async/await) para cada una de las operaciones CRUD.  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OMQ2AABAAsSPBCj5fFDpwwIgHRiywEZJWQZeZ2ao9AAD+4lyruzq+ngAA8Nr1AOH4Becqws1iAAAAAElFTkSuQmCC)  
 **5. Descripción de las Cuatro Operaciones**  
   
 **5.1 Subir Archivo (**SubirArchivoAsync **)**  
- **Flujo:**  
1. Solicita al usuario ingresar la ruta completa o relativa del archivo local.  
2. Valida que el archivo exista en disco mediante File.Exists(rutaLocal).  
3. Extrae el nombre del archivo con Path.GetFileName(rutaLocal).  
4. Obtiene una referencia BlobClient asociada al contenedor.  
5. Abre un flujo de lectura (FileStream) y transmite el contenido hacia Azure utilizando UploadAsync(uploadFileStream, overwrite: true).  
6. Notifica en consola si la operación fue exitosa.  
   
 **5.2 Listar Archivos (**ListarArchivosAsync **)**  
- **Flujo:**  
1. Utiliza containerClient.GetBlobsAsync() para enumerar de manera asíncrona todos los objetos dentro del contenedor.  
2. Imprime los resultados en una tabla alineada con las columnas **Nombre** y   **Tamaño** (en bytes).  
3. Muestra el número total de blobs almacenados o avisa si el contenedor está vacío.  
   
 **5.3 Descargar Archivo (**DescargarArchivoAsync **)**  
- **Flujo:**  
1. Solicita al usuario el nombre del blob que desea descargar.  
2. Valida su existencia en la nube mediante await blobClient.ExistsAsync().  
3. Solicita la ruta de la carpeta de destino local. Si el directorio especificado no existe, lo crea automáticamente con Directory.CreateDirectory.  
4. Realiza la descarga directa hacia el archivo local con await blobClient.DownloadToAsync(rutaDescarga).  
5. Confirma la descarga y muestra la ruta absoluta del archivo guardado.  
   
 **5.4 Eliminar Archivo (**EliminarArchivoAsync **)**  
- **Flujo:**  
1. Solicita el nombre del blob a eliminar.  
2. Comprueba que el archivo exista en Azure mediante await blobClient.ExistsAsync().  
3. Solicita confirmación explícita al usuario (¿Está seguro que desea eliminar...? (s/n)) para evitar pérdidas accidentales.  
4. Si la respuesta es afirmativa, invoca await blobClient.DeleteIfExistsAsync() y notifica la eliminación exitosa.  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSPBCUbfDqpYGZDAgAU2QtIq6DIzW7UHAMBfHGt1V+fXEwAAXrseHC4GAc+PQWgAAAAASUVORK5CYII=)  
 **6. Manejo de Errores**  
   
 Para asegurar la robustez de la aplicación y una adecuada experiencia de usuario, se implementaron múltiples mecanismos de validación y control de excepciones:  
5. **Validación de Entradas:**  
- Detección de cadenas vacías o compuestas sólo por espacios en nombres de archivo y rutas.  
- Limpieza automática de comillas envolventes (" o ') que suelen agregarse al copiar rutas en terminales o exploradores de archivos.  
1. **Validación Previa de Estado:**  
- Se comprueba la existencia de archivos locales antes de iniciar transferencias a la red.  
- Se verifica la existencia remota en Azure antes de intentar descargar o borrar un blob, evitando excepciones innecesarias.  
1. **Control Global de Excepciones (** **try-catch** **):**  
- El bucle del menú y las operaciones están protegidas con bloques try-catch que capturan excepciones tanto del sistema (IOException, UnauthorizedAccessException) como del SDK de Azure (RequestFailedException). En caso de error, se muestra un mensaje informativo sin abortar la aplicación.  
1. **Compatibilidad con Entornos de Consola:**  
- Se implementó detección de redirección de entrada/salida (Console.IsInputRedirected), lo que permite que el programa funcione adecuadamente tanto en consolas interactivas como en pruebas automatizadas o pipelines.  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANklEQVR4nO3OQQmAABRAsSfYxZo/lheTGMACBrCCNxG2BFtmZquOAAD4i3Ot7mr/egIAwGvXA6fKBdgPS8dhAAAAAElFTkSuQmCC)  
 **7. Mecanismo Utilizado para Proteger la Connection String**  
 **Análisis de Riesgo**  
   
 La *Connection String* contiene la clave secreta de la cuenta de almacenamiento (AccountKey), la cual otorga **control total y administrativo** sobre los datos. Publicar esta credencial en un repositorio público de GitHub compromete la seguridad y puede ocasionar robo, borrado de información o cargos no autorizados.  
 **Estrategia Implementada**  
1. **Soporte de Variables de Entorno:**  
   
    
   
  El código implementa la lectura de la cadena mediante la variable de entorno AZURE_STORAGE_CONNECTION_STRING:  
2. string connectionString =  
   
      Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??  
   
      "DefaultEndpointsProtocol=https;AccountName=miastorage26;AccountKey=...;EndpointSuffix=core.windows.net";  
   
    
3. Esto permite que en entornos de producción, servidores o al ejecutar localmente, la clave se mantenga en el sistema operativo sin necesidad de estar escrita en el código fuente.  
4. **Exclusión en Control de Versiones:**  
- La cadena quemada debe ser reemplazada por una referencia a la variable de entorno antes de realizar el commit final al repositorio de GitHub.  
- En caso de utilizar archivos de configuración locales (appsettings.json, .env), estos deben estar listados en el .gitignore del proyecto.  
1. **Buenas Prácticas Recomendadas para Producción:**  
- Empleo de **Azure Key Vault** para el almacenamiento y rotación centralizada de secretos.  
- Migración hacia **Managed Identities (Identidades Administradas)** y autenticación mediante Azure Active Directory / Microsoft Entra ID (DefaultAzureCredential del paquete Azure.Identity), prescindiendo totalmente del uso de contraseñas o claves estáticas.  
 ![](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAnEAAAACCAYAAAA3pIp+AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAANUlEQVR4nO3OMQ2AABAAsSNBCzpfFxNCmJHAjAU2QtIq6DIzW7UHAMBfnGt1V8fHEQAA3rsexO0F3jmX9Q8AAAAASUVORK5CYII=)  
 **8. Instrucciones para Ejecutar el Proyecto**  
 **Prerrequisitos**  
- Tener instalado el [.NET SDK (versión 8.0, 9.0 o 10.0).  
 **Pasos de Ejecución**](https://dotnet.microsoft.com/download "https://dotnet.microsoft.com/download")  
1. **Abrir una terminal y posicionarse en la carpeta del proyecto:**  
2. cd MIA_AzureBlob_PM  
   
    
3. **(Opcional pero recomendado) Configurar la variable de entorno con la Connection String:**  
- **En Linux / macOS:**  
- export AZURE_STORAGE_CONNECTION_STRING="DefaultEndpointsProtocol=https;AccountName=miastorage26;AccountKey=...;EndpointSuffix=core.windows.net"  
   
    
- **En Windows (PowerShell):**  
- $env:AZURE_STORAGE_CONNECTION_STRING="DefaultEndpointsProtocol=https;AccountName=miastorage26;AccountKey=...;EndpointSuffix=core.windows.net"  
   
    
1. **Restaurar paquetes NuGet:**  
2. dotnet restore  
   
    
3. **Compilar el proyecto:**  
4. dotnet build  
   
    
5. **Ejecutar la aplicación:**  
6. dotnet run  
   
    
 **Ejemplo de Uso Interactivo:**  
   
 Al iniciar el programa se presentará el siguiente menú:  
   
 =================================  
   
       MIA - AZURE BLOB STORAGE     
   
  =================================  
   
  1. Subir archivo  
   
  2. Listar archivos  
   
  3. Descargar archivo  
   
  4. Eliminar archivo  
   
  5. Salir  
   
  =================================  
   
  Seleccione una opción:  
   
    
- Ingrese 1 para subir un archivo indicando la ruta local (ej. /home/pdms/mi_archivo.txt).  
- Ingrese 2 para visualizar todos los blobs existentes en miaarchivos.  
- Ingrese 3 para descargar un archivo indicando el nombre del blob y la carpeta destino.  
- Ingrese 4 para eliminar un archivo indicando el nombre y confirmando con s.  
- Ingrese 5 para cerrar la aplicación.  
