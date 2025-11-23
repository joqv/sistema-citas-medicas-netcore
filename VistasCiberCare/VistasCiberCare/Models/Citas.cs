namespace VistasCiberCare.Models
{
    public class Citas
    {
        public int id_cita { get; set; }
        public string? dni_paciente { get; set; }
        public string? nombre_paciente { get; set; }
        public int? id_horario { get; set; }
        public DateTime fecha { get; set; }
        public string? hora { get; set; }
        public string? nombre_doctor { get; set; }
        public string? especialidad { get; set; }
    }
}
