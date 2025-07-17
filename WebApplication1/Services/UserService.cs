using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> GetUserByEmailAsync(string email) => await _userRepository.GetByEmailAsync(email);
        public async Task<bool> CheckUserPassword(string email,string password) => await _userRepository.CheckPassword(email,password);
        public async Task<bool> CreateUserAsync(User user) => await _userRepository.CreateAsync(user);
        public async Task UpdateUserAsync(User user) => await _userRepository.UpdateAsync(user);
        public async Task DeleteUserAsync(string email) => await _userRepository.DeleteAsync(email);

        //public Task<List<Dictionary<string, List<Video>>>> GetAllUserProjects(string email) => _userRepository.GetAllProjects(email);

        //public Task<List<Video>> GetProject(string email, string project_name) => _userRepository.GetProject(email, project_name);

        //public Task<bool> SaveProject(string email, string project_name, List<Video> data) => _userRepository.SaveProject(email, project_name, data);
        //public Task<bool> DeleteProject(string email, string project_name) => _userRepository.DeleteProject(email, project_name);
    }
}
