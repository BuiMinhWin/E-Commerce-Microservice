using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eCommerce.SharedLibrary.Responses;

using Microsoft.AspNetCore.Authorization;

namespace AuthenticationAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await userInterface.Register(userDTO);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await userInterface.Login(loginDTO);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task <ActionResult<GetUserDTO>> GetUser (int id)
        {
            if(id <=0) return BadRequest("Invalid user id");
            var user = await userInterface.GetUser(id);
            return user.Id > 0 ? Ok(user) : NotFound();
        }


    }
}
