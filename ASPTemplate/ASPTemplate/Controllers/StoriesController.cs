using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPTemplate.Controllers
{
    [Route("api/Stories")]
    [ApiController]
    public class StoriesController : ControllerBase
    {

        private readonly UserManager<UserCreds> _userManager;
        private readonly IUserService _userService;
        private readonly IStoryService _storyService;

        public StoriesController(UserManager<UserCreds> userManager, IStoryService storyService, IUserService userService)
        {
            _userManager = userManager;
            _storyService = storyService;
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
        [Route("storying")]
        public async Task<IActionResult> Storying([FromForm] InstaStoryRequest request)
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
                    var story = new Stories
                    {
                        EmailUser = user.Email,
                        Image = file,
                    };
                    _storyService.Create(story);
                    userInfo.Stories.Add(story.Id);
                    _userService.Update(userInfo.Id, userInfo);
                }
                else
                {
                    return BadRequest("Image is Empty !!!");
                }
                return Ok("Storying was Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("stories/mine")]
        public async Task<IActionResult> SeeingAllTheUserStories()
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var story = _storyService.GetByUserEmail(user.Email);
                return Ok(story);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("stories/byuseremail")]
        public async Task<IActionResult> SeeingAllOneUserStories([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var story = _storyService.GetByUserEmail(request.UserEmail);
                return Ok(story);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("stories/byid")]
        public async Task<IActionResult> SeeingAllOneUserStories([FromBody] InstaStoryIdRequest request)
        {
            try
            {
                var story = _storyService.Get(request.StoryId);
                if (story == null)
                {
                    return BadRequest("Story not Found !!!");
                }
                return Ok(story);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("stories/delete")]
        public async Task<IActionResult> DeletingStories([FromBody] InstaStoryIdRequest request)
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
                _storyService.Remove(request.StoryId);
                userInfo.Stories.Remove(request.StoryId);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("Story Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }

    }
}
