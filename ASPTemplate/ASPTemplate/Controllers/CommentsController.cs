using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPTemplate.Controllers
{
    [Route("api/Commnets")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly UserManager<UserCreds> _userManager;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public CommentsController(UserManager<UserCreds> userManager, IPostService postService, IUserService userService, ICommentService commentService)
        {
            _userManager = userManager;
            _postService = postService;
            _userService = userService;
            _commentService = commentService;
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
        [Route("commenting")]
        public async Task<IActionResult> Commenting([FromBody] InstaCreateCommentRequest request)
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
                var comment = new Comments
                {
                    EmailUser = user.Email,
                    Context = request.Context,
                    IdPost = request.PostId,
                    Date = DateTime.Now
                };
                _commentService.Create(comment);
                userInfo.Comments.Add(comment.Id);
                post.Comments.Add(comment.Id);
                _userService.Update(userInfo.Id, userInfo);
                _postService.Update(post.Id, post);
                return Ok("Commenting was Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("comments/mine")]
        public async Task<IActionResult> SeeingAllTheUserComments()
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var comment = _commentService.GetByUserEmail(user.Email);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("comments/byuseremail")]
        public async Task<IActionResult> SeeingAllOneUserComments([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var comment = _commentService.GetByUserEmail(request.UserEmail);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("comments/byid")]
        public async Task<IActionResult> SeeingAllUserComments([FromBody] InstaCommentRequest request)
        {
            try
            {
                var comment = _commentService.Get(request.CommentId);
                if (comment == null)
                {
                    return BadRequest("Comment not Found !!!");
                }
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("comments/delete")]
        public async Task<IActionResult> DeletingComments([FromBody] InstaCommentRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                var comment = _commentService.Get(request.CommentId);
                var post = _postService.Get(comment.IdPost);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var userInfo = _userService.GetByEmail(user.Email);
                _commentService.Remove(request.CommentId);
                userInfo.Comments.Remove(request.CommentId);
                post.Comments.Remove(request.CommentId);
                _userService.Update(userInfo.Id, userInfo);
                _postService.Update(post.Id, post);
                return Ok("Comment Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }
    }
}
