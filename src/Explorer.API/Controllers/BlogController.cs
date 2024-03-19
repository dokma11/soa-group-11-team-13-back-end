using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Explorer.API.Controllers
{

    [Route("api/blog")]
    public class BlogController : BaseApiController
    {
        private readonly IBlogService _blogService;
        private readonly IClubMemberManagementService _clubMemberManagmentService;
        private readonly IClubService _clubService;

        private static readonly HttpClient _sharedClient = new HttpClient() { BaseAddress = new Uri("http://localhost:8082/") };

        public BlogController(IBlogService authenticationService, IClubMemberManagementService clubMemberManagmentService, IClubService clubService)
        {
            _blogService = authenticationService;
            _clubMemberManagmentService = clubMemberManagmentService;
            _clubService = clubService;
        }

        [Authorize(Policy = "userPolicy")]
        [HttpPost("create")]
        public ActionResult<BlogResponseDto> Create([FromBody] BlogCreateDto blog)
        {
            blog.Date = DateTime.UtcNow;
            blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.Create(blog);
            return CreateResponse(result);
        }

        [Authorize(Policy = "userPolicy")]
        [HttpPut("update")]
        public ActionResult<BlogResponseDto> Update([FromBody] BlogUpdateDto blog)
        {
            blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.Update(blog);
            return CreateResponse(result);
        }

        [Authorize(Policy = "userPolicy")]
        [HttpDelete("delete/{id:long}")]
        public ActionResult<BlogResponseDto> Delete(int id)
        {
            var result = _blogService.Delete(id);
            return CreateResponse(result);
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
            var result = _blogService.SetVote(id, userId, VoteType.UPVOTE);
            return CreateResponse(result);
        }

        [Authorize(Policy = "userPolicy")]
        [HttpGet("downvote/{id:long}")]
        public ActionResult Downvote(long id)
        {
            if (_blogService.IsBlogClosed(id)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));

            var userId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.SetVote(id, userId, VoteType.DOWNVOTE);
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
    }

}

