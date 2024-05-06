using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers
{

    public class FollowerController : FollowersService.FollowersServiceBase
    {

        private readonly ILogger<FollowerController> _logger;

        public FollowerController(ILogger<FollowerController> logger)
        {
            _logger = logger;
        }

        public override async Task<GetUserByUsernameResponse> GetUserByUsername(GetUserByUsernameRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("https://localhost:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetUserByUsernameAsync(request);
            Console.WriteLine(response.User);

            return await Task.FromResult(new GetUserByUsernameResponse
            {
                User = response.User
            });
        }

        public override async Task<GetFollowersResponse> GetFollowers(GetFollowersRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetFollowersAsync(request);
            Console.WriteLine(response.Users);

            return await Task.FromResult(new GetFollowersResponse
            {
                Users = { response.Users }
            });
        }

        public override async Task<GetFollowingsResponse> GetFollowings(GetFollowingsRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetFollowingsAsync(request);
            Console.WriteLine(response.Users);

            return await Task.FromResult(new GetFollowingsResponse
            {
                Users = { response.Users }
            });
        }

        public override async Task<GetRecommendedUsersResponse> GetRecommendedUsers(GetRecommendedUsersRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetRecommendedUsersAsync(request);
            Console.WriteLine(response.Users);

            return await Task.FromResult(new GetRecommendedUsersResponse
            {
                Users = { response.Users }
            });
        }

        public override async Task<FollowResponse> Follow(FollowRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.FollowAsync(request);

            return await Task.FromResult(new FollowResponse { });
        }

        public override async Task<UnfollowResponse> Unfollow(UnfollowRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.UnfollowAsync(request);

            return await Task.FromResult(new UnfollowResponse { });
        }

        /*private readonly IFollowerService _followerService;
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
        public async Task<ActionResult<PagedResult<FollowUserResponseDto>>> GetFollowers([FromQuery] int page, [FromQuery] int pageSize, long id)
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
        public async Task<ActionResult<PagedResult<FollowUserResponseDto>>> GetFollowings([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            //var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowUserResponseDto>>("users/followings/" + id);

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
        public async Task<ActionResult<FollowUserResponseDto>> Create([FromBody] FollowerCreateDto follower)
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

        [HttpGet("recommended")]
        public async Task<ActionResult<PagedResult<FollowUserResponseDto>>> GetRecommended()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FollowUserResponseDto>>("users/recommended/" + userId);

            if (response != null)
            {
                var pagedResult = new PagedResult<FollowUserResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }*/

    }

}
