using ASPTemplate.Dtos;
using ASPTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Security.Claims;

namespace ASPTemplate.Controllers
{
    [ApiController]
    [Route("api/userinteraction")]
    public class UserControllers : ControllerBase
    {
        private readonly UserManager<UserCreds> _userManager;
        private readonly IUserService _userService;

        public UserControllers(UserManager<UserCreds> userManager, IUserService userService)
        {
            _userManager = userManager;
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
        [Route("follow")]
        public async Task<IActionResult> FollowUser([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var usertofollow = await _userManager.FindByEmailAsync(request.UserEmail);
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (usertofollow == null || user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var usertofollowInfo = _userService.GetByEmail(usertofollow.Email);
                var userInfo = _userService.GetByEmail(user.Email);
                if (userInfo.Email != usertofollowInfo.Email)
                {
                    if (!usertofollowInfo.Followers.Contains(user.Email.ToString()))
                    {
                        usertofollowInfo.Followers.Add(user.Email.ToString());
                    }
                    else
                    {
                        return BadRequest("User is already in the Followers");
                    }
                    if (!userInfo.Followings.Contains(usertofollow.Email.ToString()))
                    {
                        userInfo.Followings.Add(usertofollow.Email.ToString());
                    }
                    else
                    {
                        return BadRequest("User is already in the Followings");
                    }
                }
                else
                {
                    return BadRequest("Can't follow yourself");
                }
                _userService.Update(usertofollowInfo.Id, usertofollowInfo);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("Followed Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("unfollow")]
        public async Task<IActionResult> UnFollowUser([FromBody] InstaUserEmailRequest request)
        {
            try
            {
                var usertofollow = await _userManager.FindByEmailAsync(request.UserEmail);
                var id = GetIdFromCookie();
                var user = await _userManager.FindByIdAsync(id);
                if (usertofollow == null || user == null)
                {
                    return BadRequest("User not Found!!!");
                }
                var usertofollowInfo = _userService.GetByEmail(usertofollow.Email);
                var userInfo = _userService.GetByEmail(user.Email);
                if (userInfo.Email != usertofollowInfo.Email)
                {
                    if (usertofollowInfo.Followers.Contains(user.Email.ToString()))
                    {
                        usertofollowInfo.Followers.Remove(user.Email.ToString());
                    }
                    else
                    {
                        return BadRequest("User not found in the Followers");
                    }
                    if (userInfo.Followings.Contains(usertofollow.Email.ToString()))
                    {
                        userInfo.Followings.Remove(usertofollow.Email.ToString());
                    }
                    else
                    {
                        return BadRequest("User not found in the Followings");
                    }
                }
                else
                {
                    return BadRequest("Can't follow yourself");
                }
                _userService.Update(usertofollowInfo.Id, usertofollowInfo);
                _userService.Update(userInfo.Id, userInfo);
                return Ok("UnFollowed Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request !!! \n" + ex);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("Followers/mine")]
        public async Task<IActionResult> MyUserFollwers()
        {

            var id = GetIdFromCookie();
            var user = await _userManager.FindByIdAsync(id);
            var result = _userService.GetByEmail(user.Email);
            return Ok(result.Followers);
        }

        [Authorize]
        [HttpGet]
        [Route("Followeings/mine")]
        public async Task<IActionResult> MyUserFollwings()
        {

            var id = GetIdFromCookie();
            var user = await _userManager.FindByIdAsync(id);
            var result = _userService.GetByEmail(user.Email);
            return Ok(result.Followings);
        }

        [Authorize]
        [HttpPost]
        [Route("Followers/byuseremail")]
        public async Task<IActionResult> OneUserFollwers([FromBody] InstaUserEmailRequest request)
        {
            var result = _userService.GetByEmail(request.UserEmail);
            return Ok(result.Followings);
        }

        [Authorize]
        [HttpPost]
        [Route("Followeings/byuseremail")]
        public async Task<IActionResult> OneUserFollwings([FromBody] InstaUserEmailRequest request)
        {
            var result = _userService.GetByEmail(request.UserEmail);
            return Ok(result.Followings);
        }

        [Authorize]
        [HttpGet]
        [Route("User/mine")]
        public async Task<IActionResult> ShowMyProfile()
        {

            var id = GetIdFromCookie();
            var user = await _userManager.FindByIdAsync(id);
            var result = _userService.GetByEmail(user.Email);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [Route("User/byuseremail")]
        public async Task<IActionResult> ShowOneUserProfile([FromBody] InstaUserEmailRequest request)
        {
            var result = _userService.GetByEmail(request.UserEmail);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [Route("User/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] InstaUpdateUserRequest request)
        {
            var id = GetIdFromCookie();
            var user = await _userManager.FindByIdAsync(id);
            var result = _userService.GetByEmail(user.Email);
            byte[] file = null;
            if (request.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    request.Image.CopyTo(ms);
                    file = ms.ToArray();
                };
            }
            var NewUser = new Users
            {
                Id= result.Id,
                Email = result.Email,
                Gender = request.Gender,
                Bio = request.Bio,
                Image = file,
                Posts = result.Posts,
                Stories= result.Stories,
                Followers= result.Followers,
                Followings= result.Followings,
                Comments= result.Comments,
                Likes= result.Likes,    
                SavedPosts= result.SavedPosts,
                SavedStories= result.SavedStories
            };
            _userService.Update(result.Id, NewUser);
            return Ok("Updated successfully !!!");
        }
    }
}
