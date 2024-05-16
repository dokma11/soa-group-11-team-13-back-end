using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers
{
    public class BlogRecommendationsController : BlogRecommendationService.BlogRecommendationServiceBase
    {
        private readonly ILogger<BlogRecommendationsController> _logger;

        public BlogRecommendationsController(ILogger<BlogRecommendationsController> logger)
        {
            _logger = logger;
        }

        public override async Task<CreateBlogRecommendationResponse> CreateBlogRecommendation(CreateBlogRecommendationRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogRecommendationService.BlogRecommendationServiceClient(channel);
            Console.WriteLine(request);
            var response = await client.CreateBlogRecommendationAsync(request);
            Console.WriteLine("CreateBlogRecommendation " + response);

            return await Task.FromResult(new CreateBlogRecommendationResponse { });
        }

        public override async Task<GetBlogRecommendationByIdResponse> GetBlogRecommendationById(GetBlogRecommendationByIdRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogRecommendationService.BlogRecommendationServiceClient(channel);
            var response = await client.GetBlogRecommendationByIdAsync(request);
            Console.WriteLine("GetBlogRecommendationById " + response);

            return await Task.FromResult(new GetBlogRecommendationByIdResponse
            {
                Recommendation = response.Recommendation
            });
        }

        /*
          
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
         
         */
    }
}
