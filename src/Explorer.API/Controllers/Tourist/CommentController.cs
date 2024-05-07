using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers.Tourist
{
    public class CommentController : CommentsService.CommentsServiceBase
    {
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public override async Task<CreateCommentResponse> CreateComment(CreateCommentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new CommentsService.CommentsServiceClient(channel);
            var response = await client.CreateCommentAsync(request);
            Console.WriteLine("CREATE COMMENT ");

            return await Task.FromResult(new CreateCommentResponse { });
        }

        public override async Task<UpdateCommentResponse> UpdateComment(UpdateCommentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new CommentsService.CommentsServiceClient(channel);
            var response = await client.UpdateCommentAsync(request);
            Console.WriteLine("UPDATE COMMENT ");

            return await Task.FromResult(new UpdateCommentResponse { });
        }

        public override async Task<DeleteCommentResponse> DeleteComment(DeleteCommentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new CommentsService.CommentsServiceClient(channel);
            var response = await client.DeleteCommentAsync(request);
            Console.WriteLine("DELETE COMMENT ");

            return await Task.FromResult(new DeleteCommentResponse { });
        }

        public override async Task<GetAllCommentsResponse> GetAllComments(GetAllCommentsRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://blogs:8082/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new CommentsService.CommentsServiceClient(channel);
            var response = await client.GetAllCommentsAsync(request);
            Console.WriteLine("GET ALL COMMENTS: " + response.Comments);

            return await Task.FromResult(new GetAllCommentsResponse
            {
                Comments = { response.Comments }
            });
        }
    }
}
