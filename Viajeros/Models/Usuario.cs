namespace Viajeros.Models
{
    public class Usuario
    {
        public string Nombres { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }
}