using PrimerApi.Model;

namespace CiberCare_Api.Repository.Interfaces
{
    public interface IDoctores
    {
        IEnumerable<Doctores> getDoctor();
        Doctores getDoctores(Int32 id_doctor);
        Doctores getDoctor(string nombre);
        string insertDoctores(Doctores reg);
        string updateDoctores(Doctores reg);

        string deleteDoctores(Doctores reg);
    }
}
