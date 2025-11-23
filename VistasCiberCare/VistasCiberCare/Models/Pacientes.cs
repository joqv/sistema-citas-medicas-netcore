using System.ComponentModel.DataAnnotations;

namespace VistasCiberCare.Models
{
    public class Pacientes
    {
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        public string? dni { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string? apellido { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string? telefono { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string? email { get; set; }
        public string? contrasena { get; set; }

    }
}
