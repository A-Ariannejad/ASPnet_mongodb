using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace ASPTemplate.Controllers
{
    [ApiController]
    [Route("api/Posts")]
    public class PostsController : ControllerBase
    {
        private readonly UserManager<UserCreds> _userManager;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public PostsController(UserManager<UserCreds> userManager, IPostService postService, IUserService userService)
        {
            _userManager = userManager;
            _postService = postService;
            _userService = userService;
        }

        private string GetIdFromCookie()
        {
            var IdStr = HttpContext.User.Identities
                .FirstOrDefault()
                .Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .FirstOrDefault()
                .Value;

            return IdStr.ToString();
        }

        [Authorize]
        [HttpPost]
        [Route("posting")]
        public async Task<IActionResult> Posting([FromForm] InstaPostRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                byte[] file;
                using (var ms = new MemoryStream())
                {
                    request.Image.CopyTo(ms);
                    file = ms.ToArray();
                };
                if (file != null)
                {
                    var post = new Posts
                    {
                        EmailUser = user.Email,
                        Caption = request.Caption,
                        Image = file,
                        Location = request.Location
                    };
                    _postService.Create(post);
                    userInfo.Posts.Add(post.Id);
                    _userService.Update(userInfo.Id, userInfo);
                }
                else
                {
                    return BadRequest("Image is Empty !!!");
                }
                return Ok("Posting was Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("posts/mine")]
        public async Task<IActionResult> SeeingAllTheUserPosts()
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var post = _postService.GetByUserEmail(user.Email);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/byuseremail")]
        public async Task<IActionResult> SeeingAllOneUserPosts([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var post = _postService.GetByUserEmail(request.UserEmail);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/byid")]
        public async Task<IActionResult> SeeingAllUserPosts([FromBody] InstaPostIdRequest request)
        {
            try
            {
                var post = _postService.Get(request.PostId);
                if (post == null)
                {
                    return BadRequest("Post not Found !!!");
                }
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/update")]
        public async Task<IActionResult> UpdatePosts([FromForm] InstaPostUpdateRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                byte[] file;
                using (var ms = new MemoryStream())
                {
                    request.Image.CopyTo(ms);
                    file = ms.ToArray();
                };
                var existingPost = _postService.Get(request.PostId);
                if (existingPost == null)
                {
                    return BadRequest("post does not exist !!!");
                }
                if (existingPost.EmailUser != user.Email)
                {
                    return Unauthorized("Not this user post");
                }
                if (file != null)
                {
                    var post = new Posts
                    {
                        Id = request.PostId,
                        EmailUser = user.Email,
                        Caption = request.Caption,
                        Image = file,
                        Location = request.Location
                    };
                    _postService.Update(request.PostId, post);
                }
                else
                {
                    return BadRequest("Image is Empty !!!");
                }
                return Ok("Updating was Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/delete")]
        public async Task<IActionResult> DeletingPosts([FromBody] InstaPostIdRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                _postService.Remove(request.PostId);
                userInfo.Posts.Remove(request.PostId);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("Post Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/UserSave")]
        public async Task<IActionResult> SavingPosts([FromBody] InstaPostIdRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                userInfo.SavedPosts.Add(request.PostId);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("Saving Post Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("posts/UserUnsave")]
        public async Task<IActionResult> UnsavingPosts([FromBody] InstaPostIdRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                userInfo.SavedPosts.Remove(request.PostId);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("Unsaving Post Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

    }
}
