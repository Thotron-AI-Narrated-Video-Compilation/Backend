using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string email);

        Task<bool> CheckPassword(string email,string password);

        //Task<List<Dictionary<string, List<Video>>>> GetAllProjects(string email);
        //Task<Project> GetProjectById(string projectId);
        //Task<List<Video>> GetProject(string email, string project_name);
        //Task<bool> SaveProject(string email, string project_name, List<Video> data , string avatar = "");
        //Task<bool> DeleteProject(string email, string project_name);
    }
}
