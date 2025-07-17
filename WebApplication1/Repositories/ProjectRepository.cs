using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories;
using ZstdSharp.Unsafe;

public class ProjectRepository : IProjectRepository
{
    private readonly IUserRepository _userRepository;
    private readonly IMongoCollection<Project> _projects;
    private readonly IMongoCollection<User> _users;

    public ProjectRepository(MongoDbContext context)
    {
        _projects = context.Projects;
        _users = context.Users;
        _userRepository = new UserRepository(context);

    }

    private async Task UpdateAsync(Project project) => await _projects.ReplaceOneAsync(p => p.Name == project.Name && p.user_id == project.user_id, project);
    private async Task DeleteAsync(Project project) => await _projects.DeleteOneAsync(p => p.user_id == project.user_id && p.Name == project.Name );
    private async Task RenameProjectAsync(Project project) => await _projects.ReplaceOneAsync(p => p.Id == project.Id && p.user_id == project.user_id, project);

    /*
    private static string ComputeHashOfProjectInfo(Dictionary<string, List<Video>> projectInfo)
    {
        var json = JsonSerializer.Serialize(projectInfo);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(hashBytes);
    }
    */

    public async Task<List<Project>> GetAllProjects(string email)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null)
            return new List<Project>();

        // Convert each Project's project_info to a dictionary and return as a list
        var result = _projects.Find(u => u.user_id == email).ToList();
        return result;
    }

    // save summary of project and check if the videos changed or not and is there any summary before or not
    /*
    public async Task<bool> SaveProjectSummary(string email, ObjectId project_id, string summary)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) & Builders<Project>.Filter.Eq(p => p.Id, project_id);
        var project = await _projects.Find(filter).FirstOrDefaultAsync();

        if (project != null)
        {
            // Recompute hash based on current videos (to lock summary to current state)
            if (project.project_info.TryGetValue("videos", out var videos))
            {
                project.ProjectInfoHash = ComputeHashOfProjectInfo(project.project_info);  // 👈 Store the hash at summary generation time
            }

            // Save the summary
            project.Summary = summary;
            await UpdateAsync(project);
            return true;
        }

        return false;
    }

    */

    public async Task<bool> SaveProject(string email  , string project_name, List<Video> data, string avatar = default, ObjectId project_id = default  )
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;



        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) & Builders<Project>.Filter.Eq(p => p.Id, project_id);

        var project = await _projects.Find(filter).FirstOrDefaultAsync();

        // Compute new hash from project_info (the videos)
        Task<List<Project>> userProjects = GetAllProjects(email);



        bool namefound = false;
        foreach (var userproject in userProjects.Result)
        {
            if (userproject.Name == project_name)
            {
                // If a project with the same name already exists, return false
                namefound = true;
                break;
            }
        }


        if (project != null)
        {
            //string newHash = ComputeHashOfProjectInfo(project.project_info);

            project.project_info["videos"] = data;
            project.Avatar = string.IsNullOrWhiteSpace(avatar) ? null : avatar;

            /*
            if(summary != null)
            {
                // If the summary is already set, we can update it
                project.Summary = summary;
            }
            */

            /*
            // Check if content has changed
            if (project.ProjectInfoHash != newHash)
            {
                project.Summary = null; // Invalidate summary
                project.ProjectInfoHash = newHash;
            }
            */

            // Check if a project id  with the same project name
            var existingProject = await _projects.Find(p => p.user_id == email && p.Id == project_id).FirstOrDefaultAsync();
            if ( existingProject.Name != project_name)
            {
                //change project name to the new one
                if(namefound==true)
                {
                    return false; // If a project with the same name already exists, return false
                }
                project.Name = project_name;
                await RenameProjectAsync(project);
                return true;

            }

            // Update the project's video data
            // Assuming you want to store the videos under a default key, e.g., "videos"

            await UpdateAsync(project);
            return true;
        }
        else
        {

            if (namefound)
            {
                // If a project with the same name already exists, return false
                return false;
            }

            var project_info = new Dictionary<string, List<Video>> { { "videos", data } };

            // Create a new project with the given name and videos
            var newProject = new Project
            {
                user_id = email,
                Name = project_name,
                Avatar = string.IsNullOrWhiteSpace(avatar) ? null : avatar,
                project_info = new Dictionary<string, List<Video>> { { "videos", data } }

            };
            _projects.InsertOneAsync(newProject);
            await UpdateAsync(newProject);
            return true;
        }

    }

    



    public async Task<string> getProjectId(string email , string project_name)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, project_name);

        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return null;
        return project.Id.ToString();
    }

    // get project name from project id 
    public async Task<string> getProjectName(ObjectId project_id)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id, project_id);
        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return null;
        return project.Name;
    }
    public async Task<Project> ShareProject(string Id)
    {
        var objectId = new ObjectId(Id);
        var filter = Builders<Project>.Filter.Eq(p => p.Id, objectId);
        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return null;
        return project;
    }

    // find project by email and project name
    // this method will be used in dashboard search bar for project 

    public async Task<Project> FindProject(string email, string project_name)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, project_name);
        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        return project;
    }


    public async Task<List<Video>> GetProject(string email, string project_name)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return null;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, project_name);

        var project = await _projects.Find(filter).FirstOrDefaultAsync();

        var videos = project.project_info["videos"];

        if (project != null)
        {

            if (project.Avatar != null || project.Avatar != "")
            {
                // Create a virtual Video object for the avatar
                var avatarVideo = new Video
                {
                    title = "Project Avatar",
                    id = "avatar",
                    url = project.Avatar,
                    thumbnail = project.Avatar,
                    start_time = 0,
                    end_time = 0
                };
                // Add the avatar video to the beginning of the list    
                videos.Insert(0, avatarVideo);
            }

            return videos;
        }
        return null;
    }
    
    /*
    public async Task<string> GetUserEmailByProjectId(ObjectId project_id)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id, project_id);
        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return null;
        var user = await _users.Find(u => u.Email == project.user_id).FirstOrDefaultAsync(); 
        return user.Email;
    }
    */

    public async Task<bool> DeleteProject(string email, string project_name)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, project_name);

        var projectToRemove = await _projects.Find(filter).FirstOrDefaultAsync();

        if (projectToRemove != null)
        {
            await DeleteAsync(projectToRemove);
            return true;
        }
        return false;
    }


    public async Task<bool> UpdateProjectAvatar(string email, string projectName, string avatarUrl)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, projectName);

        //get the project of user owner 


        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return false;

        project.Avatar = avatarUrl;
        await UpdateAsync(project);
        return true;
    }


    /*
    public async Task<bool> SaveProject(string email, string project_name, List<Video> data, string avatar )
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;


            var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
             Builders<Project>.Filter.Eq(p => p.Name, project_name);

            var project = await _projects.Find(filter).FirstOrDefaultAsync();

        if (project != null)
        {
            // Update the project's video data
            // Assuming you want to store the videos under a default key, e.g., "videos"
            project.project_info["videos"] = data;
            project.Avatar = avatar;
            await UpdateAsync(project);
            return true;
        }
        else
        {
            // Create a new project with the given name and videos
            var newProject = new Project
            {
                //Id = "",
                user_id = email,
                Name = project_name,
                project_info = new Dictionary<string, List<Video>> { { "videos", data } },
                Avatar = avatar // or set as needed
            };
            _projects.InsertOneAsync(newProject);
            await UpdateAsync(newProject);
            return true;
        }
        
    }*/

    /*

    public async Task<List<Video>> GetProject(string email, string project_name)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return null;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, project_name);

        var project = await _projects.Find(filter).FirstOrDefaultAsync();

        var videos = project.project_info["videos"];



        if (project != null)
        {

            if (project.Avatar != null || project.Avatar != "")
            {
                // Create a virtual Video object for the avatar
                var avatarVideo = new Video
                {
                    title = "Project Avatar",
                    id = "avatar",
                    url = project.Avatar,
                    thumbnail = project.Avatar,
                    start_time = 0,
                    end_time = 0
                };
                // Add the avatar video to the beginning of the list    
                videos.Insert(0, avatarVideo);
            }

            return videos;
        }
        return null;
    }*/


    /*
*/

    /*
    public async Task<bool> UpdateProjectAvatar(string email, string projectName, string avatarUrl)
    {
        var current_user = await _userRepository.GetByEmailAsync(email);
        if (current_user == null) return false;

        var filter = Builders<Project>.Filter.Eq(p => p.user_id, email) &
                    Builders<Project>.Filter.Eq(p => p.Name, projectName);

        //get the project of user owner 


        var project = await _projects.Find(filter).FirstOrDefaultAsync();
        if (project == null) return false;

        project.Avatar = avatarUrl;
        await UpdateAsync(project);
        return true;
    }

    */

    /*
    public async Task<bool> RenameProject(string email, string oldName, string newName)
    {
        // 1. Check if new name exists
        var existingProject = await _userRepository.GetProject(email, newName);
        if (existingProject != null) return false;

        // 2. Get current project
        var project = await _userRepository.GetProject(email, oldName);
        if (project == null) return false;

        // 3. Save with new name first
        bool saveSuccess = await _userRepository.SaveProject(email, newName, project);
        if (!saveSuccess) return false;

        // 4. Delete old name only after successful save
        return await _userRepository.DeleteProject(email, oldName);
    }
    */

}