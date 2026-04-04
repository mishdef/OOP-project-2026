using gsst.Model.User;

namespace gsst.Interfaces
{
    public interface IAuthService
    {
        User Login(string username, string password);
    }
}