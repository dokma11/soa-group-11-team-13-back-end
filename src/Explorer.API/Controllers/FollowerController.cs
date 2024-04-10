using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.UseCases;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "nonAdministratorPolicy")]
    [Route("api/follower")]
    public class FollowerController : BaseApiController
    {
        private readonly IFollowerService _followerService;
        private readonly IUserService _userService;
        public FollowerController(IFollowerService followerService, IUserService userService)
        {
            _followerService = followerService;
            _userService = userService;
        }

        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://followers:8084/"),
        };

        [HttpGet("followers/{id:long}")]
        public async Task<ActionResult<PagedResult<FollowerResponseWithUserDto>>> GetFollowers([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            /* long userId = id;
             var identity = HttpContext.User.Identity as ClaimsIdentity;
             if (identity != null && identity.IsAuthenticated)
             {
                 userId = long.Parse(identity.FindFirst("id").Value);
             }
             var result = _followerService.GetFollowers(page, pageSize, userId);
             return CreateResponse(result);*/
            Console.WriteLine("USAO U BEK ZA GET FOLLOWERS");
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowerResponseWithUserDto>>("users/followers/" + userId);

            if (response != null)
            {
                var pagedResult = new PagedResult<FollowerResponseWithUserDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }
        [HttpGet("followings/{id:long}")]
        public async Task<ActionResult<PagedResult<FollowingResponseWithUserDto>>> GetFollowings([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            /*long userId = id;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                userId = long.Parse(identity.FindFirst("id").Value);
            }
            var result = _followerService.GetFollowings(page, pageSize, userId);
            return CreateResponse(result);*/

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowingResponseWithUserDto>>("users/followings/" + userId);

            if (response != null)
            {
                var pagedResult = new PagedResult<FollowingResponseWithUserDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();


        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            /*var result = _followerService.Delete(id);
            return CreateResponse(result);*/
            ///OVO ISPARAVI I NA FRONTU I BEKU
             try
            {
                HttpResponseMessage response = await _sharedClient.DeleteAsync("users/unfollow/" + id);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }


        }

        [HttpPost]
        public async Task<ActionResult<FollowerResponseDto>> Create([FromBody] FollowerCreateDto follower)
        {
            /*var result = _followerService.Create(follower);
            return CreateResponse(result);*/
            //ovo ispravi i na beku i na frontu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string json = JsonConvert.SerializeObject(follower);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            try
            {

                HttpResponseMessage response = await _sharedClient.PostAsync("users/follow/" + follower.FollowedById + "/" + follower.UserId, null);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }


        }

        [HttpGet("search/{searchUsername}")]
        public async Task<ActionResult<PagedResult<UserResponseDto>>> GetSearch([FromQuery] int page, [FromQuery] int pageSize, string searchUsername)
        {
            /*ong userId = 0;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                userId = long.Parse(identity.FindFirst("id").Value);
            }
            var result = _userService.SearchUsers(0, 0, searchUsername, userId);
            return CreateResponse(result);*/

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var id = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<UserResponseDto>>("users/" + searchUsername);

            if (response != null)
            {
                var pagedResult = new PagedResult<UserResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();

        }
    }

}
