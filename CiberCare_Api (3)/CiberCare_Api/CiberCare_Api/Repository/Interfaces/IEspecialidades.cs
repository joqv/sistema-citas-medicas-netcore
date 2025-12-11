using PrimerApi.Model;

namespace PrimerApi.Repository.Interfaces
{
    public interface IEspecialidades
    {

        IEnumerable<Especialidades> getEspecialidade();
        Especialidades getEspecialidades(Int32 id_especialidad);
        Especialidades getEspecialidades(string nombre);
        string insertEspecialidades(Especialidades reg);
        string updateEspecialidades(Especialidades reg);

        string deleteEspecialidades(Especialidades reg);
    }
}


