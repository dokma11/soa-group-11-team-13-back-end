using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.Tours;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceTranscoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Administrator.Administration
{
    public class EquipmentController : EquipmentService.EquipmentServiceBase
    {
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(ILogger<EquipmentController> logger)
        {
            _logger = logger;
        }

        public override async Task<EquipmentGetAllResponse> GetAll(EquipmentGetAllRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new EquipmentService.EquipmentServiceClient(channel);
            var response = await client.GetAllAsync(request);

            return await Task.FromResult(new EquipmentGetAllResponse
            {
                Equipment = { response.Equipment }
            });
        }


        public override async Task<EquipmentCreateResponse> Create(EquipmentCreateRequest request, ServerCallContext context)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress("http://tours:8081/", new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new EquipmentService.EquipmentServiceClient(channel);
            var response = await client.CreateAsync(request);

            return await Task.FromResult(new EquipmentCreateResponse {});

        }

        /*public ActionResult<EquipmentResponseDto> Update([FromBody] EquipmentUpdateDto equipment)
        {
            var result = _equipmentService.Update(equipment);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _equipmentService.Delete(id);
            return CreateResponse(result);
        }*/
    }
}
