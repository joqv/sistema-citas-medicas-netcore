using CiberCare_Api.Model;
using PrimerApi.Model;

namespace CiberCare_Api.Repository.Interfaces
{
    public interface ICitas
    {
        IEnumerable<Citas> getCitas();
        Citas getCitas(string dni_paciente);
        Citas getCitas(Int32 id_cita);
        string insertCitas(Citas reg);
        string updateCitas(CrearCitaDTO reg);

        string deleteCitas(Citas reg);
    }
}
