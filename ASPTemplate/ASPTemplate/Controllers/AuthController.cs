using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ASPTemplate.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserCreds> _userManager;
        private readonly RoleManager<UserRoles> _rolemanager;
        private readonly SignInManager<UserCreds> _signInManager;
        private readonly IUserService _userService;
        public AuthController(UserManager<UserCreds> userManager, RoleManager<UserRoles> roleManager, SignInManager<UserCreds> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _rolemanager = roleManager;
            _signInManager = signInManager;
            this._userService = userService;
        }

        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var appRole = new UserRoles
            {
                Name = request.Role,
            };
            var createRole = await _rolemanager.CreateAsync(appRole);

            return Ok(new { message = "role created successfully !!!" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequest request)
        {
            var result = await RegisterAsync(request);

            return result.Success ? Ok(request) : BadRequest(result.Message);

        }

        private async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "User already Exists !!!"
                    };
                }

                userExists = new UserCreds
                {
                    Email = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.Email,
                    Fullname = request.Fullname
                };
                var createUserResult = await _userManager.CreateAsync(userExists, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = $"Create User Failed !!!{createUserResult?.Errors?.First()?.Description}"
                    };
                }
                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExists, "USER");
                if (!addUserToRoleResult.Succeeded)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = $"Created User but has not been added to role !!!{addUserToRoleResult?.Errors?.First()?.Description}"
                    };
                }
                var user = new Users
                {
                    Email= request.Email,
                };
                _userService.Create(user);
                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully !!!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return new RegisterResponse { Success = false, Message = ex.Message };
            }
        }

        
        [HttpPost]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginRequest))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);

        }

        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    new LoginResponse
                    {
                        Message = "Invalid email/Password",
                        Success = false
                    };
                }
                else
                {
                    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                    if (!signInResult.Succeeded)
                    {
                        return new LoginResponse
                        {
                            Message = "Invalid password",
                            Success = false
                        };
                    }
                }
                var claims = new List<Claim>
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var roles = await _userManager.GetRolesAsync(user);
                var roleclaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleclaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKeyForApp12"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddMinutes(300);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:7220",
                    audience: "https://localhost:7220",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                    );

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principle = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                Response.Cookies.Append("X-Access-Token", token.ToString(), new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

                await HttpContext.SignInAsync(principle, properties);

                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login SuccessFull !!!",
                    Email = user.Email,
                    Success = true,
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
