using System.Text.Json;

namespace Viajeros.Services
{
    public class ImageUploadService
    {
        private readonly string _baseUrl = "http://10.0.2.2:5130";

        public async Task<UploadResult?> SubirImagen(Stream fileStream, string fileName)
        {
            try
            {
                Console.WriteLine($"📱 === INICIO UPLOAD ===");
                Console.WriteLine($"📱 Archivo: {fileName}");

                // Asegurar posición 0
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                }

                // URL DIRECTA (ya sabemos que funciona desde el navegador)
                string apiUrl = $"{_baseUrl}/api/upload/subir-imagen";
                Console.WriteLine($"📱 URL: {apiUrl}");

                // Leer archivo completo a memoria
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                Console.WriteLine($"📱 Archivo leído: {fileBytes.Length} bytes");

                // Preparar contenido multipart
                var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(fileBytes);

                // Detectar tipo MIME
                var ext = Path.GetExtension(fileName).ToLower();
                var mimeType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "image/jpeg"
                };

                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                content.Add(fileContent, "archivo", fileName);

                Console.WriteLine($"📱 Contenido preparado: {mimeType}");

                // Crear HttpClient con handler personalizado
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var httpClient = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };

                Console.WriteLine($"📤 Enviando POST...");

                var response = await httpClient.PostAsync(apiUrl, content);

                Console.WriteLine($"📥 Status: {response.StatusCode}");

                var responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📥 Response: {responseText}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonSerializer.Deserialize<UploadResult>(responseText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Console.WriteLine($"✅ SUCCESS - URL: {resultado?.Url}");
                    return resultado;
                }
                else
                {
                    Console.WriteLine($"❌ HTTP Error: {response.StatusCode}");
                    return new UploadResult
                    {
                        Exito = false,
                        Mensaje = $"Error HTTP {response.StatusCode}: {responseText}"
                    };
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"💥 HTTP Exception: {httpEx.Message}");
                Console.WriteLine($"💥 Inner: {httpEx.InnerException?.Message}");

                return new UploadResult
                {
                    Exito = false,
                    Mensaje = $"Error de conexión HTTP: {httpEx.Message}"
                };
            }
            catch (TaskCanceledException tcEx)
            {
                Console.WriteLine($"💥 Timeout: {tcEx.Message}");

                return new UploadResult
                {
                    Exito = false,
                    Mensaje = "Timeout - El servidor tardó demasiado en responder"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Exception: {ex.GetType().Name}");
                Console.WriteLine($"💥 Message: {ex.Message}");
                Console.WriteLine($"💥 Stack: {ex.StackTrace}");

                return new UploadResult
                {
                    Exito = false,
                    Mensaje = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<bool> EliminarImagen(string nombreArchivo)
        {
            try
            {
                if (string.IsNullOrEmpty(nombreArchivo))
                {
                    Console.WriteLine("⚠️ Nombre de archivo vacío para eliminar");
                    return false;
                }

                Console.WriteLine($"🗑️ === INICIO ELIMINACIÓN ===");
                Console.WriteLine($"🗑️ Archivo a eliminar: {nombreArchivo}");

                // Extraer solo el nombre del archivo si viene con ruta completa
                var nombreArchivoLimpio = ObtenerNombreArchivo(nombreArchivo);
                Console.WriteLine($"🗑️ Archivo limpio: {nombreArchivoLimpio}");

                // URL para eliminar imagen
                string apiUrl = $"{_baseUrl}/api/upload/eliminar-imagen/{Uri.EscapeDataString(nombreArchivoLimpio)}";
                Console.WriteLine($"🗑️ URL: {apiUrl}");

                // Crear HttpClient
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var httpClient = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                Console.WriteLine($"🗑️ Enviando DELETE...");

                var response = await httpClient.DeleteAsync(apiUrl);

                Console.WriteLine($"🗑️ Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✅ ELIMINACIÓN EXITOSA: {responseText}");
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"⚠️ Archivo no encontrado (puede que ya haya sido eliminado)");
                    return true; // Consideramos éxito si no existe
                }
                else
                {
                    var errorText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al eliminar: {response.StatusCode} - {errorText}");
                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"💥 HTTP Exception al eliminar: {httpEx.Message}");
                return false;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"💥 Timeout al eliminar imagen");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Exception al eliminar: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

        private string ObtenerNombreArchivo(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            // Si es una URL completa, extraer solo el nombre del archivo
            if (url.Contains("/"))
            {
                return Path.GetFileName(url);
            }

            return url;
        }
    }

    public class UploadResult
    {
        public bool Exito { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}