using System.Text.Json;
using Viajeros.Models;
using Viajeros.Services;

namespace Viajeros.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private string _baseUrl;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<(bool success, string message, dynamic userData)> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                var conexion = await _apiService.ProbarConexion();
                if (!conexion.success)
                {
                    return (false, "No hay conexión con el servidor", null);
                }
                _baseUrl = conexion.workingUrl;
            }

            return await _apiService.LoginUnificado(email, password, _baseUrl);
        }

        public async Task SaveAuthData(dynamic userData)
        {
            try
            {
                if (userData != null)
                {
                    var authData = new AuthData
                    {
                        IsAuthenticated = true,
                        UserId = userData.id ?? 0,
                        UserName = userData.nombres ?? string.Empty,
                        Email = userData.email ?? string.Empty,
                        Token = userData.token ?? string.Empty,
                        IsAdmin = userData.esAdmin ?? false,
                        Rol = userData.rol ?? string.Empty
                    };

                    var json = JsonSerializer.Serialize(authData);
                    await SecureStorage.SetAsync("authData", json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar datos de autenticación: {ex.Message}");
            }
        }

        public async Task<AuthData> GetAuthStateAsync()
        {
            try
            {
                var json = await SecureStorage.GetAsync("authData");
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<AuthData>(json) ?? new AuthData { IsAuthenticated = false };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos de autenticación: {ex.Message}");
            }

            return new AuthData { IsAuthenticated = false };
        }

        public async Task LogoutAsync()
        {
            try
            {
                SecureStorage.Remove("authData");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }
    }

    public class AuthData
    {
        public bool IsAuthenticated { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}