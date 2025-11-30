using System.ComponentModel.DataAnnotations;

namespace VistasCiberCare.Models
{
    public class Horarios
    {
        public int id_horario { get; set; }
        [Required]
        public DateOnly? fecha { get; set; }
        [Required]
        public TimeOnly? hora { get; set; }
        [Required]
        public int id_doctor { get; set; }
        public string? apellido { get; set; }
        public bool disponible { get; set; }

        public int id_especialidad { get; set; }


    }
}
