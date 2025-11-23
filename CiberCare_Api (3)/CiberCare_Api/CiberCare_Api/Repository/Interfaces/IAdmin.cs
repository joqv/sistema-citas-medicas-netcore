using CiberCare_Api.Model;
using PrimerApi.Model;

namespace CiberCare_Api.Repository.Interfaces
{
    public interface IAdmin
    {
        IEnumerable<Admin> GetAdmins();

        Admin? GetAdminmLogin(string usuario, string contrasena);

    }
}
