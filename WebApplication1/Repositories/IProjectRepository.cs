using MongoDB.Bson;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IProjectRepository
    {
        //Task<double> GetProjectAverageRating(string ownerEmail, string projectName);
        //Task<List<ProjectRating>> GetProjectRatings(string ownerEmail, string projectName);
        //Task<bool> RateProject(string raterEmail, string ownerEmail, string projectName, int rating);

        // Project info management
        Task<List<Project>> GetAllProjects(string email);
        Task<string> getProjectId(string email, string project_name);
        Task<string> getProjectName(ObjectId project_id);

        Task<Project> ShareProject(string Id);

        Task<bool> SaveProject(string email, string project_name, List<Video> data, string avatar = default , ObjectId project_id = default);
        Task<List<Video>> GetProject(string email, string project_name);
        Task<Project> FindProject(string email, string project_name);

        Task<bool> DeleteProject(string email, string project_name);
        Task<bool> UpdateProjectAvatar(string email, string project_name, string avatarUrl);
        //Task<string> GetUserEmailByProjectId(ObjectId project_id);




        //Task<bool> RenameProject(string email, string oldName, string newName);


    }
}
