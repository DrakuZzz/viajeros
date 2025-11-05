using Newtonsoft.Json;
using System.Buffers.Text;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Viajeros.Models;



namespace Viajeros.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "http://10.0.2.2:5130"; // 👈 AGREGAR ESTA LÍNEA

        public ApiService()
        {
            var handler = new HttpClientHandler();

#if ANDROID
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
#endif

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<(bool success, string workingUrl)> ProbarConexion()
        {
            var testUrls = new[]
            {
                "http://10.0.2.2:5130",
                "http://192.168.1.71:5130",  // ✅ PRIMERO: Tu IP local real
                "http://localhost:5130",
                "https://10.0.2.2:7000",
                "https://localhost:7000",
            };

            foreach (var baseUrl in testUrls)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"🔗 PROBANDO: {baseUrl}");
                    var testUrl = $"{baseUrl}/api/Lugares";
                    var response = await _httpClient.GetAsync(testUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ CONEXIÓN EXITOSA: {baseUrl}");
                        _baseUrl = baseUrl; // 👈 GUARDAR LA URL QUE FUNCIONA
                        return (true, baseUrl);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ HTTP Error: {response.StatusCode} para {baseUrl}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Falló {baseUrl}: {ex.GetType().Name} - {ex.Message}");
                }
            }

            return (false, "Ninguna URL funcionó");
        }

        public async Task<(bool success, string message)> RegistrarUsuario(Usuario usuario, string baseUrl)
        {
            try
            {
                // CORREGIDO: Usar las propiedades exactas que espera el backend
                var registroDto = new
                {
                    Nombres = usuario.Nombres,           // ← MAYÚSCULA
                    Email = usuario.Email,               // ← MAYÚSCULA  
                    Contraseña = usuario.Contraseña,     // ← MAYÚSCULA
                    ConfirmarContraseña = usuario.ConfirmarContraseña  // ← MAYÚSCULA
                };

                var json = JsonConvert.SerializeObject(registroDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Usuarios/registro";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Usuario registrado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        // 👇 MÉTODO PARA LOGIN UNIFICADO
        public async Task<(bool success, string message, dynamic userData)> LoginUnificado(string email, string contraseña, string baseUrl)
        {
            try
            {
                var loginDto = new
                {
                    Email = email,
                    Contraseña = contraseña
                };

                var json = JsonConvert.SerializeObject(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Usuarios/login-unificado";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    return (true, "Login exitoso", result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    return (false, errorResult?.message ?? "Error en login", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}", null);
            }
        }

        // 👇 MÉTODOS PARA LUGARES
        public async Task<List<LugarConUltimoComentarioYEstrellasDto>> ObtenerTodosLugares(string baseUrl, int idPais, int idEstado)
        {
            try
            {
                var url = $"{baseUrl}/api/Lugares?idPais={idPais}&idEstado={idEstado}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var lugares = JsonConvert.DeserializeObject<List<LugarConUltimoComentarioYEstrellasDto>>(json);
                    return lugares ?? new List<LugarConUltimoComentarioYEstrellasDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener lugares: {response.StatusCode}");
                    return new List<LugarConUltimoComentarioYEstrellasDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener lugares: {ex.Message}");
                return new List<LugarConUltimoComentarioYEstrellasDto>();
            }
        }

        public async Task<List<LugarConUltimoComentarioYEstrellasDto>> ObtenerLugaresPorCategoria(string baseUrl, int idCategoria, int idPais, int idEstado)
        {
            try
            {
                var url = $"{baseUrl}/api/Lugares/categoria/{idCategoria}?idPais={idPais}&idEstado={idEstado}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var lugares = JsonConvert.DeserializeObject<List<LugarConUltimoComentarioYEstrellasDto>>(json);
                    return lugares ?? new List<LugarConUltimoComentarioYEstrellasDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener lugares por categoría: {response.StatusCode}");
                    return new List<LugarConUltimoComentarioYEstrellasDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener lugares por categoría: {ex.Message}");
                return new List<LugarConUltimoComentarioYEstrellasDto>();
            }
        }

        public async Task<LugarCompletoDto> ObtenerLugarPorId(string baseUrl, int idLugar)
        {
            try
            {
                var url = $"{baseUrl}/api/Lugares/{idLugar}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var lugar = JsonConvert.DeserializeObject<LugarCompletoDto>(json);
                    return lugar;
                }
                else
                {
                    Console.WriteLine($"Error al obtener lugar: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener lugar: {ex.Message}");
                return null;
            }
        }

        public async Task<List<LugarConUltimoComentarioYEstrellasDto>> BuscarLugares(string baseUrl, string termino)
        {
            try
            {
                var url = $"{baseUrl}/api/Lugares/buscar/{Uri.EscapeDataString(termino)}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var lugares = JsonConvert.DeserializeObject<List<LugarConUltimoComentarioYEstrellasDto>>(json);
                    return lugares ?? new List<LugarConUltimoComentarioYEstrellasDto>();
                }
                else
                {
                    Console.WriteLine($"Error al buscar lugares: {response.StatusCode}");
                    return new List<LugarConUltimoComentarioYEstrellasDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al buscar lugares: {ex.Message}");
                return new List<LugarConUltimoComentarioYEstrellasDto>();
            }
        }

        public async Task<bool> ActualizarLugar(int idLugar, LugarUpdateDto lugarUpdate, string baseUrl)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/api/Lugares/{idLugar}", lugarUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar lugar: {ex.Message}");
                return false;
            }
        }

        // 👇 AGREGAR ESTOS MÉTODOS AL ApiService.cs
        public async Task<List<CategoriaDto>> ObtenerCategorias(string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Categorias";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categorias = JsonConvert.DeserializeObject<List<CategoriaDto>>(json);
                    return categorias ?? new List<CategoriaDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener categorías: {response.StatusCode}");
                    return new List<CategoriaDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener categorías: {ex.Message}");
                return new List<CategoriaDto>();
            }
        }

        public async Task<List<TransporteDto>> ObtenerTransportes(string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Transportes";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var transportes = JsonConvert.DeserializeObject<List<TransporteDto>>(json);
                    return transportes ?? new List<TransporteDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener transportes: {response.StatusCode}");
                    return new List<TransporteDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener transportes: {ex.Message}");
                return new List<TransporteDto>();
            }
        }

        public async Task<List<ActividadDto>> ObtenerActividades(string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Actividades";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var actividades = JsonConvert.DeserializeObject<List<ActividadDto>>(json);
                    return actividades ?? new List<ActividadDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener actividades: {response.StatusCode}");
                    return new List<ActividadDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener actividades: {ex.Message}");
                return new List<ActividadDto>();
            }
        }

        public async Task<List<PaisDto>> ObtenerPaises(string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Ubicaciones/paises";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var paises = JsonConvert.DeserializeObject<List<PaisDto>>(json);
                    return paises ?? new List<PaisDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener países: {response.StatusCode}");
                    return new List<PaisDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener países: {ex.Message}");
                return new List<PaisDto>();
            }
        }

        public async Task<List<EstadoDto>> ObtenerEstadosPorPais(string baseUrl, int idPais)
        {
            try
            {
                var url = $"{baseUrl}/api/Ubicaciones/estados/pais/{idPais}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var estados = JsonConvert.DeserializeObject<List<EstadoDto>>(json);
                    return estados ?? new List<EstadoDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener estados: {response.StatusCode}");
                    return new List<EstadoDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener estados: {ex.Message}");
                return new List<EstadoDto>();
            }
        }

        public async Task<List<CiudadDto>> ObtenerCiudadesPorEstado(string baseUrl, int idEstado)
        {
            try
            {
                var url = $"{baseUrl}/api/Ubicaciones/ciudades/estado/{idEstado}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var ciudades = JsonConvert.DeserializeObject<List<CiudadDto>>(json);
                    return ciudades ?? new List<CiudadDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener ciudades: {response.StatusCode}");
                    return new List<CiudadDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener ciudades: {ex.Message}");
                return new List<CiudadDto>();
            }
        }

        public async Task<(bool success, string message)> CrearLugar(LugarCreateDto lugar, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(lugar);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Lugares";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Lugar creado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> RegistrarAdministrador(AdminRegistroDto admin, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(admin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Administradores/registro";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Administrador registrado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        // 👇 AGREGAR ESTOS MÉTODOS AL ApiService.cs

        // Métodos para crear elementos
        public async Task<(bool success, string message)> CrearTransporte(TransporteCreateDto transporte, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(transporte);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Transportes";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Transporte creado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CrearActividad(ActividadCreateDto actividad, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(actividad);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Actividades";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Actividad creada exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CrearCategoria(CategoriaCreateDto categoria, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(categoria);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Categorias";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Categoría creada exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CrearPais(PaisCreateDto pais, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(pais);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Ubicaciones/paises";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "País creado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CrearEstado(EstadoCreateDto estado, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(estado);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Ubicaciones/estados";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Estado creado exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> CrearCiudad(CiudadCreateDto ciudad, string baseUrl)
        {
            try
            {
                var json = JsonConvert.SerializeObject(ciudad);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Ubicaciones/ciudades";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Ciudad creada exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (false, $"Error del servidor: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ActualizarUsuarioAsync(int idUsuario, UsuarioUpdateDto usuario, string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Usuarios/{idUsuario}"; // Usa el mismo nombre de ruta de tu controlador
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    string msg = data?.message ?? "Perfil actualizado correctamente";
                    return (true, msg);
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    string msg = data?.message ?? "Error al actualizar el usuario";
                    return (false, msg);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Excepción: {ex.Message}");
            }
        }

        public async Task<UsuarioPerfil> ObtenerUsuarioPorId(string baseUrl, int userId)
        {
            try
            {
                var url = $"{baseUrl}/api/Usuarios/{userId}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var usuario = await response.Content.ReadFromJsonAsync<UsuarioPerfil>();
                    return usuario ?? new UsuarioPerfil();
                }
                else
                {
                    Console.WriteLine($"Error al obtener usuario: {response.StatusCode}");
                    return new UsuarioPerfil();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al obtener usuario: {ex.Message}");
                return new UsuarioPerfil();
            }
        }

        public async Task<(bool success, string message)> CrearOActualizarComentario(
    int idLugar, ComentarioDto comentario, decimal calificacion, string baseUrl)
        {
            try
            {
                var payload = new
                {
                    Id_usuario = comentario.Id_usuario,
                    Comentario = comentario.comentario,
                    Calificacion = calificacion
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Lugares/{idLugar}/comentarios";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                    return (true, "Comentario guardado");
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, error);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // VERIFICAR SI ES FAVORITO
        public async Task<bool> EsFavorito(int idUsuario, int idLugar, string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Favoritos/verificar?idUsuario={idUsuario}&idLugar={idLugar}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);
                    return result?.esFavorito ?? false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // TOGGLE FAVORITO
        public async Task<bool> ToggleFavorito(int idUsuario, int idLugar, string baseUrl)
        {
            try
            {
                var payload = new { Id_usuario = idUsuario, Id_lugar = idLugar };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{baseUrl}/api/Favoritos";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(respJson);
                    return result?.esFavorito ?? false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // ============================================
        // CLASES AUXILIARES PARA RESPUESTAS DEL API
        // ============================================
        // Agregar estas clases en tu archivo Models o al inicio del ApiService

        public class ApiResponseEstrella
        {
            public string message { get; set; } = string.Empty;
            public EstrellaDto? estrella { get; set; }
        }

        public class ApiResponseComentario
        {
            public string message { get; set; } = string.Empty;
            public ComentarioDto? comentario { get; set; }
        }

        // ============================================
        // MÉTODOS CORREGIDOS EN ApiService.cs
        // ============================================

        public async Task<EstrellaDto?> GuardarEstrella(EstrellaCreateDto estrella, string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Estrellas";
                var json = JsonConvert.SerializeObject(estrella);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📤 Enviando estrella a: {url}");
                Console.WriteLine($"📤 Datos: {json}");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"📥 Respuesta del servidor: {responseContent}");

                    // ✅ CORREGIDO: Deserializar el objeto anidado
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseEstrella>(responseContent);

                    if (apiResponse?.estrella != null)
                    {
                        Console.WriteLine($"⭐ Estrella recibida: ID={apiResponse.estrella.Id_estrella}, Usuario={apiResponse.estrella.Id_usuario}, Calificación={apiResponse.estrella.Calificacion}");
                        return apiResponse.estrella;
                    }
                    else
                    {
                        Console.WriteLine("❌ La respuesta no contiene el objeto 'estrella'");
                        return null;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error del servidor: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Excepción en GuardarEstrella: {ex.Message}");
                return null;
            }
        }

        public async Task<ComentarioDto?> GuardarComentario(ComentarioCreateDto comentario, string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Comentarios";
                var json = JsonConvert.SerializeObject(comentario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📤 Enviando comentario a: {url}");
                Console.WriteLine($"📤 Datos: {json}");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"📥 Respuesta del servidor: {responseContent}");

                    // ✅ CORREGIDO: Deserializar el objeto anidado
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseComentario>(responseContent);

                    if (apiResponse?.comentario != null)
                    {
                        Console.WriteLine($"💬 Comentario recibido: ID={apiResponse.comentario.Id_comentario}, Usuario={apiResponse.comentario.Id_usuario}");
                        return apiResponse.comentario;
                    }
                    else
                    {
                        Console.WriteLine("❌ La respuesta no contiene el objeto 'comentario'");
                        return null;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error del servidor: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Excepción en GuardarComentario: {ex.Message}");
                return null;
            }
        }


        public async Task<List<LugarConUltimoComentarioYEstrellasDto>> ObtenerFavoritosPorUsuario(int idUsuario, string baseUrl)
        {
            try
            {
                var url = $"{baseUrl}/api/Favoritos/usuario/{idUsuario}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FavoritosResponse>(json);
                    return result?.Lugares ?? new List<LugarConUltimoComentarioYEstrellasDto>();
                }
                else
                {
                    Console.WriteLine($"❌ Error al obtener favoritos: {response.StatusCode}");
                    return new List<LugarConUltimoComentarioYEstrellasDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error en ObtenerFavoritosPorUsuario: {ex.Message}");
                return new List<LugarConUltimoComentarioYEstrellasDto>();
            }
        }

        // Método para enviar recomendación
        public async Task<(bool success, string message)> EnviarRecomendacion(RecomendacionDto recomendacion)
        {
            try
            {
                var json = JsonConvert.SerializeObject(recomendacion);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 👇 AHORA SÍ USA LA VARIABLE _baseUrl
                var url = $"{_baseUrl}/api/Recomendaciones";

                System.Diagnostics.Debug.WriteLine($"📤 Enviando recomendación a: {url}");

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Recomendación enviada exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"❌ Error: {errorContent}");
                    return (false, $"Error del servidor: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}");
            }

        }
 
        }
}