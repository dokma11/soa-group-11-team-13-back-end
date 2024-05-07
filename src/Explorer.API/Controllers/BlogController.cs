using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers
{
    public class BlogController : BlogsService.BlogsServiceBase
    {
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger)
        {
            _logger = logger;
        }

        public override async Task<CreateBlogResponse> CreateBlog(CreateBlogRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.CreateBlogAsync(request);
            Console.WriteLine("CREATE BLOG");

            return await Task.FromResult(new CreateBlogResponse { });
        }

        public override async Task<SearchBlogByNameResponse> SearchBlogByName(SearchBlogByNameRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.SearchBlogByNameAsync(request);
            Console.WriteLine("SEARCH BLOGS BY NAME: " + response.Blogs);

            return await Task.FromResult(new SearchBlogByNameResponse
            {
                Blogs = { response.Blogs }
            });
        }

        /*[Authorize(Policy = "userPolicy")]
        [HttpPut("update")]
        public ActionResult<BlogResponseDto> Update([FromBody] BlogUpdateDto blog)
        {
            blog.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var result = _blogService.Update(blog);
            return CreateResponse(result);
        }
*/

        public override async Task<DeleteBlogResponse> DeleteBlog(DeleteBlogRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.DeleteBlogAsync(request);
            Console.WriteLine("DELETE BLOG");

            return await Task.FromResult(new DeleteBlogResponse { });
        }

        public override async Task<GetAllBlogsResponse> GetAllBlogs(GetAllBlogsRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.GetAllBlogsAsync(request);
            Console.WriteLine("Get All Blogs: " + response.Blogs);

            return await Task.FromResult(new GetAllBlogsResponse
            {
                Blogs = { response.Blogs }
            });
        }

        public override async Task<GetBlogByIdResponse> GetBlogById(GetBlogByIdRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.GetBlogByIdAsync(request);
            Console.WriteLine("Get Blog by id: " + response.Blog);

            return await Task.FromResult(new GetBlogByIdResponse
            {
                Blog = response.Blog
            });
        }

        /*         
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
        */

        /*          OVE POTENCIJALNO PREBACITI U NOVI MIKROSERVIS          */

        /*//[Authorize(Policy = "touristPolicy")]
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
*/
        public override async Task<PublishBlogResponse> PublishBlog(PublishBlogRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new BlogsService.BlogsServiceClient(channel);
            var response = await client.PublishBlogAsync(request);
            Console.WriteLine("PUBLISH BLOG");

            return await Task.FromResult(new PublishBlogResponse { });
        }
    }
}
