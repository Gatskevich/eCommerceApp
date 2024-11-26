using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController(IUser userInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userInterface.Register(appUserDTO);

            if (!result.Flag)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await userInterface.Login(loginDTO);

            if (!result.Flag)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Response>> GetUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user Id");
            }

            var user = await userInterface.GetUser(id);

            if (user!.Id > 0)
            {
                return Ok(user);
            }

            return NotFound();
        }
    }
}
