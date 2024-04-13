using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;

namespace Explorer.API.Controllers
{

    [Route("api/blog")]
    public class BlogController : BaseApiController
    {
        private readonly IBlogService _blogService;
        private readonly IClubMemberManagementService _clubMemberManagmentService;
        private readonly IClubService _clubService;
        private readonly IUserService _userService;

        private static readonly HttpClient _sharedClient = new HttpClient() { BaseAddress = new Uri("http://blogs:8082/") };

        public BlogController(IBlogService authenticationService, IClubMemberManagementService clubMemberManagmentService, IClubService clubService, IUserService userService)
        {
            _blogService = authenticationService;
            _clubMemberManagmentService = clubMemberManagmentService;
            _clubService = clubService;
            _userService = userService;
        }

        [Authorize(Policy = "userPolicy")]
        [HttpPost("create")]
        public async Task<ActionResult<BlogResponseDto>> Create([FromBody] BlogCreateDto blog)
        {
            try
            {
                blog.Date = DateTime.UtcNow;
                blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
                string json = JsonConvert.SerializeObject(blog);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _sharedClient.PostAsync("blogs", content);
                response.EnsureSuccessStatusCode();
                _blogService.Create(blog);
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

        [HttpGet("search/{name}")]
        public async Task<ActionResult<List<BlogResponseDto>>> SearchByName(string name)
        {
            try
            {
                using HttpResponseMessage response = await _sharedClient.GetAsync("blogs/search/" + name);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<BlogResponseDto>>(jsonResponse);

                if (data != null)
                {
                    return Ok(data);
                }
                return new List<BlogResponseDto>();
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

        [Authorize(Policy = "userPolicy")]
        [HttpPut("update")]
        public ActionResult<BlogResponseDto> Update([FromBody] BlogUpdateDto blog)
        {
            blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.Update(blog);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            //var result = _blogService.Delete(id);
            //return CreateResponse(result);

            try
            {
                HttpResponseMessage response = await _sharedClient.DeleteAsync("blogs/" + id);
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

        [HttpGet]
        public async Task<ActionResult<PagedResult<BlogResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                using HttpResponseMessage response = await _sharedClient.GetAsync("blogs");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<BlogResponseDto>>(jsonResponse);
                var pagedResult = new PagedResult<BlogResponseDto>(data!, data!.Count);

                return Ok(pagedResult);
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

        [HttpGet("{id:long}")]
        public async Task<ActionResult<BlogResponseDto>> Get(long id)
        {
            //var result = _blogService.GetById(id);
            //return CreateResponse(result);
            try
            {
                using HttpResponseMessage response = await _sharedClient.GetAsync("blogs/" + id.ToString());

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BlogResponseDto>(jsonResponse);

                return Ok(data);
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

        [HttpPut("{id:int}")]
        public ActionResult<BlogResponseDto> Update([FromBody] BlogUpdateDto blog, int id)
        {
            blog.Id = id;
            var result = _blogService.UpdateBlog(blog);
            return CreateResponse(result);
        }

        [Authorize(Policy = "userPolicy")]
        [HttpGet("upvote/{id:long}")]
        public ActionResult Upvote(long id)
        {
            if (_blogService.IsBlogClosed(id)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));

            var userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.SetVote(id, userId, Blog.API.Dtos.VoteType.UPVOTE);
            return CreateResponse(result);
        }

        [Authorize(Policy = "userPolicy")]
        [HttpGet("downvote/{id:long}")]
        public ActionResult Downvote(long id)
        {
            if (_blogService.IsBlogClosed(id)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));

            var userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.SetVote(id, userId, Blog.API.Dtos.VoteType.DOWNVOTE);
            return CreateResponse(result);
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpPost("createClubBlog")]
        public ActionResult<BlogResponseDto> CreateClubBlog([FromBody] BlogCreateDto blog)
        {
            blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            if (blog.ClubId == null)
            {
                return BadRequest();
            }
            bool touristInClub = false;
            foreach (int touristId in _clubMemberManagmentService.GetMembers((long)blog.ClubId).Value.Results.Select(member => member.UserId))
            {
                if (touristId == blog.AuthorId)
                {
                    touristInClub = true;
                    break;
                }
            }
            if (!touristInClub)
            {
                if (_clubService.GetById((int)blog.ClubId).Value.OwnerId != blog.AuthorId)
                {
                    return BadRequest();
                }
            }
            blog.Date = DateTime.UtcNow;
            var result = _blogService.Create(blog);
            return CreateResponse(result);
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpGet("getClubBlogs")]
        public ActionResult<BlogResponseDto> GetClubBlogs([FromQuery] int page, [FromQuery] int pageSize, long clubId)
        {
            var result = _blogService.GetAllFromBlogs(page, pageSize, clubId);
            return CreateResponse(result);
        }

        //[Authorize(Policy = "touristPolicy")]
        [HttpPost("recommendations")]
        public async Task<ActionResult<BlogRecommendationResponseDto>> RecommendBlog([FromBody] BlogRecommendationRequestDto request)
        {
            try
            {
                var recommenderId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
                var receiverId = this._userService.GetByUsername(request.RecommendationReceiverUsername).Value.Id;
                var blogRecommendationMicroserviceRequest = new BlogRecommendationMicorserviceRequestDto() { BlogId = request.BlogId, RecommendationReceiverId = receiverId, RecommenderId = recommenderId };
                string json = JsonConvert.SerializeObject(blogRecommendationMicroserviceRequest);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _sharedClient.PostAsync("blog-recommendations", content);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpGet("recommendations/{id:long}")]
        public async Task<ActionResult<BlogRecommendationResponseDto>> GetBlogRecommendationById(long id)
        {
            try
            {
                using HttpResponseMessage response = await _sharedClient.GetAsync("blog-recommendations/" + id.ToString());

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BlogRecommendationResponseDto>(jsonResponse);

                return Ok(data);
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

        [Authorize(Policy = "touristPolicy")]
        [HttpGet("recommendations/notifications")]
        public async Task<ActionResult<List<BlogRecommendationNotificationDto>>> GetBlogRecommendationNotifications()
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
                using HttpResponseMessage response = await _sharedClient.GetAsync("blog-recommendations/by-receiver/" + loggedInUserId.ToString());

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<BlogRecommendationResponseDto>>(jsonResponse);

                var notifications = new List<BlogRecommendationNotificationDto>();

                if (data == null) return NotFound();

                foreach (var blogRecommendation in data)
                {
                    var receiverUsername = _userService.GetNameById(blogRecommendation.RecommenderId).Value;
                    var description = "You have received a recommendation for blog '" + blogRecommendation.Blog.Title + "' from " + receiverUsername + ".";
                    var notification = new BlogRecommendationNotificationDto() { Description = description };
                    notifications.Add(notification);
                }

                return Ok(notifications);
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

        [HttpPatch("publish/{id:long}")]
        public async Task<ActionResult> PublishBlog(long id)
        {
            try
            {
                using HttpResponseMessage response = await _sharedClient.PatchAsync("blogs/publish/" + id.ToString(), new StringContent(""));

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BlogResponseDto>(jsonResponse);

                return Ok(data);
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
    }
}
