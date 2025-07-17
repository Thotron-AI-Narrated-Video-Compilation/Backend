using MongoDB.Driver;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Project> _projects;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            //var projection = Builders<User>.Projection.Exclude("_id");
            var current_user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            return current_user;
        }

        





        public async Task<bool> CheckPassword(string email, string password)
        {
            var user = await GetByEmailAsync(email);

            if (user == null || user.Password != password)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> CreateAsync(User user)
        {
            var current_user = await GetByEmailAsync(user.Email);
            if (current_user != null)
            {
                return false; // User already exists
            }
            try
            {

                await _users.InsertOneAsync(user);
                return true; // Insertion successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Insert failed: {ex.Message}");
                return false; // Insertion failed
            }
        }
        public async Task UpdateAsync(User user) => await _users.ReplaceOneAsync(u => u.Email == user.Email, user);
        public async Task DeleteAsync(string email) => await _users.DeleteOneAsync(u => u.Email == email);

        /*
        public async Task<List<Dictionary<string, List<Video>>>> GetAllProjects(string email)
        {
            var current_user = await GetByEmailAsync(email);
            if (current_user == null || current_user.Projects == null)
                return new List<Dictionary<string, List<Video>>>();

            // Convert each Project's project_info to a dictionary and return as a list
            var result = current_user.Projects
                .Select(p => p.project_info)
                .ToList();

            return result;
        }

        public async Task<Project> GetProjectById(string projectId)
        {
            var filter = Builders<User>.Filter.ElemMatch(u => u.Projects, p => p.Id == projectId);
            var user = await _users.Find(filter).FirstOrDefaultAsync();
            if (user == null) return null;
            return user.Projects.FirstOrDefault(p => p.Id == projectId);
        }*/

        /*
        public async Task<bool> SaveProject(string email, string project_name, List<Video> data)
        {
            var current_user = await GetByEmailAsync(email);
            bool found = false;

            for (int i = 0; i < current_user.Projects.Count; i++)
            {
                if (current_user.Projects[i].ContainsKey(project_name))
                {
                    current_user.Projects[i][project_name] = data;
                    await UpdateAsync(current_user);
                    return true;
                }
            }

            if (!found)
            {
                current_user.Projects.Add(new Dictionary<string, List<Video>> { { project_name, data } });
                await UpdateAsync(current_user);
                return true;
            }
            return false;
        }
        */

        /*
        public async Task<bool> SaveProject(string email, string project_name, List<Video> data , string avatar = "")
        {
            var current_user = await GetByEmailAsync(email);
            if (current_user == null) return false;

            // Find the project by name
            var project = current_user.Projects.FirstOrDefault(p => p.Name == project_name);

            if (project != null)
            {
                // Update the project's video data
                // Assuming you want to store the videos under a default key, e.g., "videos"
                project.project_info["videos"] = data;
                project.Avatar = avatar;
                await UpdateAsync(current_user);
                return true;
            }
            else
            {
                // Create a new project with the given name and videos
                var newProject = new Project
                {   Id = "",
                    Name = project_name,
                    project_info = new Dictionary<string, List<Video>> { { "videos", data } },
                    Avatar = avatar // or set as needed
                };
                _projects.InsertOneAsync(newProject);
                await UpdateAsync(current_user);
                return true;
            }
        }
        */
        /*
        public async Task<bool> DeleteProject(string email, string project_name)
        {
            var current_user = await GetByEmailAsync(email);
            if (current_user == null) return false;

            var projectToRemove = current_user.Projects.FirstOrDefault(p => p.Name == project_name);
            if (projectToRemove != null)
            {
                current_user.Projects.Remove(projectToRemove);
                await UpdateAsync(current_user);
                return true;
            }
            return false;
        }
        */
        /*
        public async Task<List<Video>> GetProject(string email, string project_name)
        {
            var current_user = await GetByEmailAsync(email);
            if (current_user == null) return null;

            var project = current_user.Projects.FirstOrDefault(p => p.Name == project_name);
            if (project != null && project.project_info.ContainsKey("videos"))
            {
                return project.project_info["videos"];
            }
            return null;
        }
        */
    }
}
