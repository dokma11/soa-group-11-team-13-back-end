using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers.Author
{
    public class FacilityController : FacilitiesService.FacilitiesServiceBase
    {
        private readonly ILogger<FollowerController> _logger;

        public FacilityController(ILogger<FollowerController> logger)
        {
            _logger = logger;
        }

        public override async Task<GetAllFacilitiesResponse> GetAllFacilities(GetAllFacilitiesRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FacilitiesService.FacilitiesServiceClient(channel);
            var response = await client.GetAllFacilitiesAsync(request);
            Console.WriteLine("GET ALL FACILITIES: " + response.Facilities);

            return await Task.FromResult(new GetAllFacilitiesResponse
            {
                Facilities = { response.Facilities }
            });
        }

        public override async Task<GetFacilitiesByAuthorIdResponse> GetFacilitiesByAuthorId(GetFacilitiesByAuthorIdRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FacilitiesService.FacilitiesServiceClient(channel);
            var response = await client.GetFacilitiesByAuthorIdAsync(request);
            Console.WriteLine("GET FACILITIES BY AUTHORID: " + response.Facilities);

            return await Task.FromResult(new GetFacilitiesByAuthorIdResponse
            {
                Facilities = { response.Facilities }
            });
        }

        public override async Task<CreateFacilityResponse> CreateFacility(CreateFacilityRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FacilitiesService.FacilitiesServiceClient(channel);
            var response = await client.CreateFacilityAsync(request);
            Console.WriteLine("CREATE A FACILITY: " + request);

            return await Task.FromResult(new CreateFacilityResponse { });
        }

        public override async Task<UpdateFacilityResponse> UpdateFacility(UpdateFacilityRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FacilitiesService.FacilitiesServiceClient(channel);
            var response = await client.UpdateFacilityAsync(request);
            Console.WriteLine("UPDATE A FACILITY: ");

            return await Task.FromResult(new UpdateFacilityResponse { });
        }

        public override async Task<DeleteFacilityResponse> DeleteFacility(DeleteFacilityRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new FacilitiesService.FacilitiesServiceClient(channel);
            var response = await client.DeleteFacilityAsync(request);
            Console.WriteLine("DELETE A FACILITY: ");

            return await Task.FromResult(new DeleteFacilityResponse { });
        }
    }
}
