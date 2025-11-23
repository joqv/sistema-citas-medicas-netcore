using PrimerApi.Model;

namespace PrimerApi.Repository.Interfaces
{
    public interface IPacientes
    {
        IEnumerable<Pacientes> getPaciente();
        Pacientes getPacientes(string dni);
        string insertPacientes(Pacientes reg);
        string updatePacientes(Pacientes reg);

        string deletePacientes(Pacientes reg);
        Pacientes? GetPacienteLogin(string dni, string contrasena);

    }

}
