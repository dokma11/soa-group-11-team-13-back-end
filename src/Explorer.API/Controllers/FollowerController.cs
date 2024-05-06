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
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetUserByUsernameAsync(request);
            Console.WriteLine("USERNAME: " + response.User);

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
            Console.WriteLine("FOLLOWERS RESPONSE: " + response.Users);

            return await Task.FromResult(new GetFollowersResponse
            {
                Users = { response.Users }
            });
        }

        public override async Task<GetFollowingsResponse> GetFollowings(GetFollowingsRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://followers:8084/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FollowersService.FollowersServiceClient(channel);
            var response = await client.GetFollowingsAsync(request);
            Console.WriteLine("FOLLOWINGS: " + response.Users);

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
            Console.WriteLine("RECOMMENDED: " + response.Users);

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

    }

}
