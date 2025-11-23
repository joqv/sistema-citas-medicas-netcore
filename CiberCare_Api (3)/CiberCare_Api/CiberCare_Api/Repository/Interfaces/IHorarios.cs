using PrimerApi.Model;

namespace CiberCare_Api.Repository.Interfaces
{
    public interface IHorarios
    {

        IEnumerable<Horarios> getHorario();
        Horarios getHorario(Int32 id_horario);
        Horarios getHorario(TimeOnly hora);
        string insertHorarios(Horarios reg);
        string updateHorarios(Horarios reg);
        string deleteHorarios(Horarios reg);
    }
}
