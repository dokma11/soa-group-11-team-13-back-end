using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowUserResponseDto>>("users/followers/" + userId);

            if (response != null)
            {
                var pagedResult = new PagedResult<FollowUserResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [HttpGet("followings/{id:long}")]
        public async Task<ActionResult<PagedResult<FollowingResponseWithUserDto>>> GetFollowings([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowUserResponseDto>>("users/followings/" + userId);

            if (response != null)
            {
                var pagedResult = new PagedResult<FollowUserResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();

        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            try
            {
                HttpResponseMessage response = await _sharedClient.DeleteAsync("users/unfollow/" + userId + "/" + id);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string json = JsonConvert.SerializeObject(follower);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            try
            {

                HttpResponseMessage response = await _sharedClient.PostAsync("users/follow/" + follower.UserId + "/" + follower.FollowedById, null);
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
        public async Task<ActionResult<FollowUserResponseDto>> GetByUsername(string searchUsername)
        {
            var user = await _sharedClient.GetFromJsonAsync<FollowUserResponseDto>("users/" + searchUsername);

            if (user != null)
            {
                return user;
            }

            return NotFound();         
        }
    }

}
