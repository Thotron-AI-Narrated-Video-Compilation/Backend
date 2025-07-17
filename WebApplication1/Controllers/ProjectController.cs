using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: api/users/{userEmail}/projects
        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjects(string userEmail)
        {
            try
            {
                var projects = await _projectService.GetAllProjects(userEmail);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("SaveProject")]
        public async Task<IActionResult> SaveProject(string email, string project_name, [FromBody] List<Video> data, string avatar = default, ObjectId project_id = default)
        {
            bool status = await _projectService.SaveProject(email, project_name, data, avatar, project_id);
            return status ? Ok(new { status = "project saved successfully!" }) : BadRequest(new { status = "an error occured while saving the project" });
        }


        [HttpGet("GetProjectID")]
        public async Task<IActionResult> getProjectId(string email, string project_name)
        {
            string Id = await _projectService.getProjectId(email, project_name);
            return Ok(new { project_Id = Id });

        }

        [HttpGet("ShareProject")]

        public async Task<IActionResult> ShareProject(string Id)
        {
            var project = await _projectService.ShareProject(Id);
            return Ok(project);
        }

        [HttpGet("GetProject")]
        public async Task<IActionResult> GetProject(string email, string project_name)
        {
            var project = await _projectService.GetProject(email, project_name);
            return Ok( project);
        }

        [HttpGet("FindProject")]
        public async Task<IActionResult> FindProject(string email, string project_name)
        {
            var project = await _projectService.FindProject(email, project_name);
            return Ok(project);
        }

        [HttpGet("DeleteProject")]
        public async Task<IActionResult> DeleteProject(string email, string project_name)
        {
            bool status = await _projectService.DeleteProject( email, project_name);
            return status ? Ok(new { status = "project deleted successfully!" }) : BadRequest(new { status = "an error occured while deleting the project" });

        }

        [HttpGet("Updatedavatar")]

        public async Task<IActionResult> UpdateProjectAvatar(string email, string project_name, string avatarUrl)
        {
            bool status = await _projectService.UpdateProjectAvatar( email, project_name,  avatarUrl);
            return status ? Ok(new { status = "Avatar Updated successfully!" }) : BadRequest(new { status = "an error occured while Updating the Avatar" });

        }

        

        /*
 

        // PUT: api/users/{userEmail}/projects/{projectName}/rename
        [HttpPut("rename")]
        public async Task<IActionResult> RenameProject(
            string userEmail,
            string projectName,
            [FromBody] RenameProjectRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NewName))
                    return BadRequest("New project name is required");

                if (projectName.Equals(request.NewName, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("New name cannot be the same as current name");

                bool success = await _projectService.RenameProject(userEmail, projectName, request.NewName);

                return success ?
                    NoContent() :
                    BadRequest("Rename failed. Project might not exist or new name is already taken");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        */


    }

    public class UpdateAvatarRequest
    {
        public string AvatarUrl { get; set; }
    }
    /*
    public class RenameProjectRequest
    {
        public string NewName { get; set; }
    }
    */
}