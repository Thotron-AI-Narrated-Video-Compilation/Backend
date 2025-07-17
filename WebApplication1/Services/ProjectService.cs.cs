using MongoDB.Bson;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        
        // Project info management
        public Task<List<Project>> GetAllProjects(string email)
            => _projectRepository.GetAllProjects(email);

        public Task<bool> SaveProject(string email, string project_name, List<Video> data, string avatar = default, ObjectId project_id = default)
            => _projectRepository.SaveProject(email, project_name, data, avatar , project_id);


        public Task<string> getProjectId(string email, string project_name)
            => _projectRepository.getProjectId(email, project_name);
        public Task<Project> ShareProject(string Id)
            => _projectRepository.ShareProject(Id);


        public Task<List<Video>> GetProject(string email, string project_name)
            => _projectRepository.GetProject(email, project_name);

        public  Task<Project> FindProject(string email, string project_name)
            => _projectRepository.FindProject(email,  project_name);


        public Task<string> getProjectName(ObjectId project_id)
            => _projectRepository.getProjectName(project_id);


        public Task<bool> DeleteProject(string email, string project_name)
            => _projectRepository.DeleteProject(email, project_name);

        public Task<bool> UpdateProjectAvatar(string email, string project_name, string avatarUrl)
            => _projectRepository.UpdateProjectAvatar(email, project_name, avatarUrl);

        /*
        public Task<string> GetUserEmailByProjectId(ObjectId project_id)
            => _projectRepository.GetUserEmailByProjectId(project_id);
        */


        /*
  

        public Task<bool> RenameProject(string email, string oldName, string newName)
            => _projectRepository.RenameProject(email, oldName, newName);

        */


        /*
        public async Task<double> GetProjectAverageRating(string ownerEmail, string projectName)
        {
            return await _projectRepository.GetProjectAverageRating(ownerEmail, projectName);
        }

        public async Task<List<ProjectRating>> GetProjectRatings(string ownerEmail, string projectName)
        {
            return await _projectRepository.GetProjectRatings(ownerEmail, projectName);
        }

        public async Task<bool> RateProject(string raterEmail, string ownerEmail, string projectName, int rating)
        {
            // Validate rating
            if (rating < 1 || rating > 5) return false;

            // Check if rater is premium
            var rater = await _userRepository.GetByEmailAsync(raterEmail);
            if (rater == null || !rater.IsPremium) return false;

            // Delegate to repository
            return await _projectRepository.RateProject(raterEmail, ownerEmail, projectName, rating);
        }*/
    }
}