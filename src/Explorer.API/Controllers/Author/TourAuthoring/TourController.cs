using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers.Author.TourAuthoring
{

    public class TourController : ToursService.ToursServiceBase
    {
        private readonly ILogger<TourController> _logger;

        public TourController(ILogger<TourController> logger)
        {
            _logger = logger;
        }

        public override async Task<GetAllToursResponse> GetAllTours(GetAllToursRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.GetAllToursAsync(request);
            Console.WriteLine("GET ALL TOURS: " + response.Tours);

            return await Task.FromResult(new GetAllToursResponse
            {
                Tours = { response.Tours }
            });
        }

        public override async Task<GetPublishedToursResponse> GetPublishedTours(GetPublishedToursRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.GetPublishedToursAsync(request);
            Console.WriteLine("GET PUBLISHED TOURS: " + response.Tours);

            return await Task.FromResult(new GetPublishedToursResponse
            {
                Tours = { response.Tours }
            });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<GetToursByAuthorIdResponse> GetToursByAuthorId(GetToursByAuthorIdRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.GetToursByAuthorIdAsync(request);
            Console.WriteLine("GET AUTHORS TOURS: " + response.Tours);

            return await Task.FromResult(new GetToursByAuthorIdResponse
            {
                Tours = { response.Tours }
            });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<CreateTourResponse> CreateTour(CreateTourRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.CreateTourAsync(request);
            Console.WriteLine("CREATE TOUR ");

            return await Task.FromResult(new CreateTourResponse { });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<UpdateTourResponse> UpdateTour(UpdateTourRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.UpdateTourAsync(request);
            Console.WriteLine("UPDATE TOUR ");

            return await Task.FromResult(new UpdateTourResponse { });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<DeleteTourResponse> DeleteTour(DeleteTourRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.DeleteTourAsync(request);
            Console.WriteLine("DELETE TOUR ");

            return await Task.FromResult(new DeleteTourResponse { });
        }

        public override async Task<GetToursEquipmentResponse> GetToursEquipment(GetToursEquipmentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.GetToursEquipmentAsync(request);
            Console.WriteLine("GET TOUR EQUIPMENT: " + response.Equipment);

            return await Task.FromResult(new GetToursEquipmentResponse
            {
                Equipment = { response.Equipment }
            });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<AddToursEquipmentResponse> AddToursEquipment(AddToursEquipmentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.AddToursEquipmentAsync(request);
            Console.WriteLine("ADD EQUIPMENT ");

            return await Task.FromResult(new AddToursEquipmentResponse { });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<DeleteToursEquipmentResponse> DeleteToursEquipment(DeleteToursEquipmentRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.DeleteToursEquipmentAsync(request);
            Console.WriteLine("DELETE EQUIPMENT ");

            return await Task.FromResult(new DeleteToursEquipmentResponse { });
        }

        public override async Task<GetTourByIdResponse> GetTourById(GetTourByIdRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.GetTourByIdAsync(request);
            Console.WriteLine("GET TOURS BY ID: " + response.Tour);

            return await Task.FromResult(new GetTourByIdResponse
            {
                Tour = response.Tour
            });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<PublishTourResponse> PublishTour(PublishTourRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.PublishTourAsync(request);
            Console.WriteLine("PUBLISH TPUR");

            return await Task.FromResult(new PublishTourResponse { });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<AddToursDurationsResponse> AddToursDurations(AddToursDurationsRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.AddToursDurationsAsync(request);
            Console.WriteLine("ADD TPUR DURATIon");

            return await Task.FromResult(new AddToursDurationsResponse { });
        }

        [Authorize(Policy = "authorPolicy")]
        public override async Task<ArchiveTourResponse> ArchiveTour(ArchiveTourRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new ToursService.ToursServiceClient(channel);
            var response = await client.ArchiveTourAsync(request);
            Console.WriteLine("PUBLISH TPUR");

            return await Task.FromResult(new ArchiveTourResponse { });
        }

        /*[Authorize(Roles = "tourist")]
        [HttpPut("markAsReady/{id:int}")]
        public ActionResult<TourResponseDto> MarkAsReady(long id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            long authorId = -1;
            if (identity != null && identity.IsAuthenticated)
            {
                authorId = long.Parse(identity.FindFirst("id").Value);
            }
            var result = _tourService.MarkAsReady(id, authorId);
            return CreateResponse(result);
        }

        [Authorize(Roles = "tourist")]
        [HttpGet("recommended/{publicKeyPointIds}")]
        public ActionResult<TourResponseDto> GetRecommended([FromQuery] int page, [FromQuery] int pageSize, string publicKeyPointIds)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            long authorId = -1;
            if (identity != null && identity.IsAuthenticated)
            {
                authorId = long.Parse(identity.FindFirst("id").Value);
            }

            var keyValuePairs = publicKeyPointIds.Split('=');

            var keyPointIdsList = keyValuePairs[1].Split(',').Select(long.Parse).ToList();

            var result = _tourService.GetToursBasedOnSelectedKeyPoints(page, pageSize, keyPointIdsList, authorId);
            return CreateResponse(result);
        }*/
    }
}
