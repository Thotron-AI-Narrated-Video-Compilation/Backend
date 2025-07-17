using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CheckUserPassword(string email,string password);
        Task<bool> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string email);

        //Task<List<Dictionary<string, List<Video>>>> GetAllUserProjects(string email);
        //Task<List<Video>> GetProject(string email, string project_name);
        //Task<bool> SaveProject(string email, string project_name, List<Video> data);
        //Task<bool> DeleteProject(string email, string project_name);
    }
}
