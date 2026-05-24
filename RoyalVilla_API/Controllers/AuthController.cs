using Microsoft.AspNetCore.Mvc;
using RoyalVilla_API.Models.DTO;
using RoyalVilla_API.Services;

namespace RoyalVilla_API.Controllers
{
   
        [Route("api/auth")]
        [ApiController]
        public class AuthController(IAuthService authService) : ControllerBase
        {
        private readonly IAuthService _authService = authService;


        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse<UserDTO>>> Register(
            RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                if (registrationRequestDTO == null)
                {
                    return BadRequest(
                        APIResponse<object>.BadRequest(
                            "Registration data is required"));
                }

                if (await _authService.IsEmailExistsAsync(
                    registrationRequestDTO.Email))
                {
                    return Conflict(
                        APIResponse<object>.Conflict(
                           ($"User With Email '{registrationRequestDTO.Email}' already exists")));
                }

                var user = await _authService.RegisterAsync(
                    registrationRequestDTO);

                if (user == null)
                {
                    return BadRequest(
                        APIResponse<object>.BadRequest(
                            "Registration failed"));
                }

                var response = APIResponse<UserDTO>.CreatedAt(
                    user,
                    "User registered successfully");

                return CreatedAtAction(
                    nameof(Register),
                    response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(
                    500,
                    "An error occurred during registration",
                    ex.Message);

                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("login")]
        //[ProducesResponseType(typeof(APIResponse<IEnumerable<LoginRequestDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Login([FromBody]
           LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(
                        APIResponse<object>.BadRequest(
                            "Login data is required"));
                }

           

                var loginResponse = await _authService.LoginAsync(loginRequestDTO);

                if (loginResponse == null)
                {
                    return BadRequest(
                        APIResponse<object>.BadRequest(
                            "Login failed"));
                }

                var response = APIResponse<LoginResponseDTO>.Ok(
                    loginResponse,
                    "Login successfully");

                return Ok(response);
            }
            //catch (Exception ex)
            //{
            //    var errorResponse = APIResponse<object>.Error(
            //        500,
            //        "An error occurred during Login",
            //        ex.Message);

            //    return StatusCode(500, errorResponse);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    throw;
            //}
            catch (Exception ex)
            {
                return StatusCode(500,
                    APIResponse<object>.Error(
                        500,
                        "ERROR",
                        ex.ToString()));
            }
        
        }
    }
    }

