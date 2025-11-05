using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Viajeros.Models
{
    public class ComentarioDto
    {
        public int Id_comentario { get; set; }
        public string comentario { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public int Id_usuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
    }

    public class EstrellaDto
    {
        public int Id_estrella { get; set; }
        public int Id_usuario { get; set; }
        public decimal Calificacion { get; set; }
    }

    public class ComentarioCreateDto
    {
        public int Id_usuario { get; set; }
        public int Id_lugar { get; set; }
        public string comentario { get; set; } = string.Empty;
        public string? Foto { get; set; } = string.Empty; // opcional
    }

    public class EstrellaCreateDto
    {
        public int Id_usuario { get; set; }
        public int Id_lugar { get; set; }
        public int Calificacion { get; set; }
    }



    public class ActividadDto
    {
        public int Id_actividad { get; set; }
        public string Actividad { get; set; } = string.Empty;
    }

    public class TransporteDto
    {
        public int Id_transporte { get; set; }
        public string Transporte { get; set; } = string.Empty;
    }

    // =====================================================
    // DTOs PARA LUGARES
    // =====================================================
    public class LugarConUltimoComentarioYEstrellasDto
    {
        public int Id_lugar { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty; // ✅ AGREGAR ESTA LÍNEA
        public ComentarioDto? UltimoComentario { get; set; }
        public List<EstrellaDto> Estrellas { get; set; } = new();
        public decimal CalificacionPromedio { get; set; }
    }

    public class LugarCompletoDto
    {
        public int Id_lugar { get; set; }
        public string NombreLugar { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public List<ComentarioDto> Comentarios { get; set; } = new();
        public List<EstrellaDto> Estrellas { get; set; } = new();
        public decimal CalificacionPromedio { get; set; }
        public List<ActividadDto> Actividades { get; set; } = new();
        public List<TransporteDto> Transportes { get; set; } = new();
        public List<string> Imagenes { get; set; } = new();

        // IDs existentes
        public int Id_pais { get; set; }
        public int Id_estado { get; set; }
        public int Id_categoria { get; set; }
        public int Id_ciudad { get; set; }
        public string? Imagen { get; set; }

        // ✅ AGREGAR ESTOS CAMPOS PARA MOSTRAR LOS NOMBRES
        public string NombrePais { get; set; } = string.Empty;
        public string NombreEstado { get; set; } = string.Empty;
        public string NombreCiudad { get; set; } = string.Empty;
    }

    public class LugarCreateDto
    {
        [Required(ErrorMessage = "El nombre del lugar es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string lugar { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(500, ErrorMessage = "La ubicación no puede exceder 500 caracteres")]
        public string ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del país es obligatorio")]
        public int id_pais { get; set; }

        [Required(ErrorMessage = "El ID del estado es obligatorio")]
        public int id_estado { get; set; }

        [Required(ErrorMessage = "El ID de la ciudad es obligatorio")] // ✅ NUEVO
        public int id_ciudad { get; set; }

        [Required(ErrorMessage = "El ID de la categoría es obligatorio")]
        public int id_categoria { get; set; }

        [StringLength(200, ErrorMessage = "La URL de la imagen no puede exceder 200 caracteres")]
        public string? imagen { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria")]
        [RegularExpression("^(1|2)$", ErrorMessage = "La prioridad debe ser '1' o '2'")]
        public string prioridad { get; set; } = "2";

        public List<int>? Actividades { get; set; }
        public List<int>? Transportes { get; set; }
    }

    // 👇 DTOs PARA ACTUALIZAR LUGARES (AGREGAR AL FINAL DEL ARCHIVO)
    public class LugarUpdateDto
    {
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string? lugar { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? descripcion { get; set; }

        [StringLength(500, ErrorMessage = "La ubicación no puede exceder 500 caracteres")]
        public string? ubicacion { get; set; }

        public int? id_pais { get; set; }
        public int? id_estado { get; set; }
        public int? id_ciudad { get; set; } // ✅ AGREGAR ESTA LÍNEA
        public int? id_categoria { get; set; }

        [StringLength(200, ErrorMessage = "La URL de la imagen no puede exceder 200 caracteres")]
        public string? imagen { get; set; }

        [RegularExpression("^(1|2)$", ErrorMessage = "La prioridad debe ser '1' o '2'")]
        public string? prioridad { get; set; }

        public List<int>? Actividades { get; set; }
        public List<int>? Transportes { get; set; }
    }

    public class LugarPartialUpdateDto
    {
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string? NombreLugar { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? Descripcion { get; set; }

        [StringLength(500, ErrorMessage = "La ubicación no puede exceder 500 caracteres")]
        public string? Ubicacion { get; set; }

        [StringLength(200, ErrorMessage = "La URL de la imagen no puede exceder 200 caracteres")]
        public string? Imagen { get; set; }

        [RegularExpression("^(1|2)$", ErrorMessage = "La prioridad debe ser '1' o '2'")]
        public string? Prioridad { get; set; }

        public int? Id_categoria { get; set; }
    }

    public class CategoriaDto
    {
        public int id_categoria { get; set; }
        public string categoria { get; set; } = string.Empty;
    }

    public class PaisDto
    {
        public int id_pais { get; set; }
        public string pais { get; set; } = string.Empty;
    }

    public class EstadoDto
    {
        public int id_estado { get; set; }
        public string estado { get; set; } = string.Empty;
        public int id_pais { get; set; }
        public string pais_nombre { get; set; } = string.Empty;
    }

    public class CiudadDto
    {
        public int id_ciudad { get; set; }
        public string ciudad { get; set; } = string.Empty;
        public int id_estado { get; set; }
        public string estado_nombre { get; set; } = string.Empty;
        public string pais_nombre { get; set; } = string.Empty;
    }

    // 👇 DTOs PARA CREAR NUEVOS ELEMENTOS
    public class PaisCreateDto
    {
        [Required(ErrorMessage = "El nombre del país es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string pais { get; set; } = string.Empty;
    }

    public class EstadoCreateDto
    {
        [Required(ErrorMessage = "El nombre del estado es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del país es obligatorio")]
        public int id_pais { get; set; }
    }

    public class CiudadCreateDto
    {
        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del estado es obligatorio")]
        public int id_estado { get; set; }

        [Required(ErrorMessage = "El ID del país es obligatorio")]
        public int id_pais { get; set; }
    }

    public class TransporteCreateDto
    {
        [Required(ErrorMessage = "El nombre del transporte es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string transporte { get; set; } = string.Empty;
    }

    public class ActividadCreateDto
    {
        [Required(ErrorMessage = "El nombre de la actividad es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string actividad { get; set; } = string.Empty;
    }

    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(100, ErrorMessage = "La categoría no puede exceder 100 caracteres")]
        public string categoria { get; set; } = string.Empty;
    }

    // 👇 DTOs PARA ADMINISTRADORES
    public class AdminRegistroDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Debe tener al menos 6 caracteres")]
        public string Contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }

    public class AdminLoginDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contraseña { get; set; } = string.Empty;
    }

    public class AdminResponseDto
    {
        public int Id_admin { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    // 👇 DTO PARA LOGIN UNIFICADO
    public class LoginUnificadoDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contraseña { get; set; } = string.Empty;
    }

    public class UsuarioPerfil
    {
        // Agregado: ID del usuario
        public int Id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        public string? Paterno { get; set; }

        public string? Materno { get; set; }

        public string? Imgperfil { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El país es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un país")]
        public int Id_pais { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un estado")]
        public int Id_estado { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una ciudad")]
        public int Id_ciudad { get; set; }
    }


    public class UsuarioUpdateDto
    {
        public string? Imgperfil { get; set; }
        public string? Nombres { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public string? Email { get; set; }
        public int? Id_pais { get; set; }
        public int? Id_estado { get; set; }
        public int? Id_ciudad { get; set; }
    }

    // Agrega estas clases a tu archivo Viajeros/Models/Dtos.cs

    public class FavoritosResponse
    {
        public int TotalFavoritos { get; set; }
        public List<LugarConUltimoComentarioYEstrellasDto> Lugares { get; set; } = new();
    }

    public class FavoritoDto
    {
        public int Id_usuario { get; set; }
        public int Id_lugar { get; set; }
    }
    public class RecomendacionDto
        {
            [Required(ErrorMessage = "El nombre del lugar es obligatorio")]
            public string NombreLugar { get; set; } = string.Empty;

            [Required(ErrorMessage = "La descripción es obligatoria")]
            public string Descripcion { get; set; } = string.Empty;

            [Required(ErrorMessage = "El país es obligatorio")]
            public string Pais { get; set; } = string.Empty;

            [Required(ErrorMessage = "El estado es obligatorio")]
            public string Estado { get; set; } = string.Empty;

            [Required(ErrorMessage = "La ciudad es obligatoria")]
            public string Ciudad { get; set; } = string.Empty;

            public string? Direccion { get; set; }
        }

}