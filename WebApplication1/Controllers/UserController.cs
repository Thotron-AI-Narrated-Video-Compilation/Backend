using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

      

        [HttpGet("GetUserData")]
        public async Task<IActionResult> GetUserData(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpGet("CheckUserPassword")]
        public async Task<IActionResult> GetUserPassword(string email, string password)
        {
            bool status = await _userService.CheckUserPassword(email,password);
            return Ok(new {isLoggedin = status});
        }

        [HttpPost("InsertUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            
            bool status =  await _userService.CreateUserAsync(user);
            return status ? Ok(new {AccountStatus = status}) : BadRequest(new {AccountStatus = status});
        }

        //mail to be givien by frontend when calling api
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var current_user = await _userService.GetUserByEmailAsync(user.Email);
            if (current_user is null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(user.Name))
            {
                current_user.Name = user.Name;
            }
            if (!string.IsNullOrEmpty(user.Phone))
            {
                current_user.Phone = user.Phone;
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                current_user.Password = user.Password;
            }

            await _userService.UpdateUserAsync(current_user);
            return Ok();
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            await _userService.DeleteUserAsync(email);
            return Ok();
        }
        /*
        [HttpGet("GetAllUserProjects")]
        public async Task<IActionResult> GetAllUserProjects(string email)
        {
            var projects = await _userService.GetAllUserProjects(email);
            return projects is null ? NotFound() : Ok(projects);
        }
        [HttpGet("GetProject")]
        public async Task<IActionResult> GetProject(string email, string project_name)
        {
            var project = await _userService.GetProject(email, project_name);
            return project is null ? NotFound() : Ok(project);
        }
        [HttpPost("SaveProject")]
        public async Task<IActionResult> SaveProject(string email, string project_name, [FromBody] List<Video> data)
        {
            bool status = await _userService.SaveProject(email, project_name, data);
            return status ? Ok(new { status = "project saved successfully!" }) : BadRequest(new { status = "an error occured while saving the project" });
        }

        [HttpDelete("DeleteProject")]
        public async Task<IActionResult> DeleteProject(string email, string project_name)
        {
            bool status = await _userService.DeleteProject(email, project_name);
            return status ? Ok(new { isDeleted = status }) : BadRequest(new { isDeleted = status });
        }
        */
    }
}
