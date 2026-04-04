using gsst.Model.User;

namespace gsst.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();
        User CreateUser(string fullName, string username, string password, string role);
        void DeleteUser(int id);
        User GetUserById(int id);
        User UpdateUser(int id, string fullName, string username, string password, string role);
    }
}