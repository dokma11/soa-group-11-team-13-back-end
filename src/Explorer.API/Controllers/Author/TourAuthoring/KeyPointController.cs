using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Author.TourAuthoring;

public class KeyPointController : KeyPointsService.KeyPointsServiceBase
{

    private readonly ILogger<KeyPointController> _logger;

    public KeyPointController(ILogger<KeyPointController> logger)
    {
        _logger = logger;
    }

    [Authorize(Policy = "authorPolicy")]
    public override async Task<KeyPointCreateResponse> Create(KeyPointCreateRequest request, ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

        var client = new KeyPointsService.KeyPointsServiceClient(channel);
        var response = await client.CreateAsync(request);

        return await Task.FromResult(new KeyPointCreateResponse {});
    }


    [Authorize(Policy = "authorPolicy")]
    public override async Task<KeyPointUpdateResponse> Update(KeyPointUpdateRequest request, ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

        var client = new KeyPointsService.KeyPointsServiceClient(channel);
        var response = await client.UpdateAsync(request);

        return await Task.FromResult(new KeyPointUpdateResponse { });
    }

    [Authorize(Policy = "authorPolicy")]
    public override async Task<KeyPointDeleteResponse> Delete(KeyPointDeleteRequest request, ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

        var client = new KeyPointsService.KeyPointsServiceClient(channel);
        var response = await client.DeleteAsync(request);

        return await Task.FromResult(new KeyPointDeleteResponse { });
    }


    public override async Task<KeyPointGetAllResponse> GetAll(KeyPointGetAllRequest request, ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

        var client = new KeyPointsService.KeyPointsServiceClient(channel);
        var response = await client.GetAllAsync(request);

        return await Task.FromResult(new KeyPointGetAllResponse { KeyPoints = { response.KeyPoints } });
    }


    public override async Task<KeyPointGetByTourIdResponse> GetByTourId(KeyPointGetByTourIdRequest request, ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

        var client = new KeyPointsService.KeyPointsServiceClient(channel);
        var response = await client.GetByTourIdAsync(request);

        return await Task.FromResult(new KeyPointGetByTourIdResponse { KeyPoints = { response.KeyPoints } });
    }

}
