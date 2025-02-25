using AuthenticationAPI.Application.DTOs;
using eCommerce.SharedLibrary.Responses;



namespace AuthenticationAPI.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(UserDTO userDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<GetUserDTO> GetUser(int userId);
    }
}
