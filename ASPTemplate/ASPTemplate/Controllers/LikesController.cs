using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPTemplate.Controllers
{
    [Route("api/Likes")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly UserManager<UserCreds> _userManager;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly ILikeService _likeService;

        public LikesController(UserManager<UserCreds> userManager, IPostService postService, IUserService userService, ILikeService likeService)
        {
            _userManager = userManager;
            _postService = postService;
            _userService = userService;
            _likeService = likeService;
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
        [Route("liking")]
        public async Task<IActionResult> Liking([FromBody] InstaCreateLikeRequest request)
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
                var post = _postService.Get(request.PostId);
                if (post == null)
                {
                    return BadRequest("Post not found \n");
                }
                var like = new Likes
                {
                    EmailUser = user.Email,
                    IdPost = request.PostId,
                    Date = DateTime.Now
                };
                _likeService.Create(like);
                userInfo.Likes.Add(like.Id);
                post.Likes.Add(like.Id);
                _userService.Update(userInfo.Id, userInfo);
                _postService.Update(post.Id, post);
                return Ok("Liking was Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("likes/mine")]
        public async Task<IActionResult> SeeingAllTheUserLikes()
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var like = _likeService.GetByUserEmail(user.Email);
                return Ok(like);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("likes/byuseremail")]
        public async Task<IActionResult> SeeingAllOneUserLikes([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var like = _likeService.GetByUserEmail(request.UserEmail);
                return Ok(like);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("likes/byid")]
        public async Task<IActionResult> SeeingAllUserLikes([FromBody] InstaLikeRequest request)
        {
            try
            {
                var like = _likeService.Get(request.LikeId);
                if (like == null)
                {
                    return BadRequest("Comment not Found !!!");
                }
                return Ok(like);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("likes/delete")]
        public async Task<IActionResult> DeletingLikes([FromBody] InstaLikeRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                var comment = _likeService.Get(request.LikeId);
                var post = _postService.Get(comment.IdPost);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                _likeService.Remove(request.LikeId);
                userInfo.Likes.Remove(request.LikeId);
                post.Likes.Remove(request.LikeId);
                _userService.Update(userInfo.Id, userInfo);
                _postService.Update(post.Id, post);
                return Ok("Like is Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

    }
}
