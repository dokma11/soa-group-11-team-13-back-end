using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/equipment")]
    public class EquipmentController : BaseApiController
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://localhost:8081/"),
        };

        [HttpGet]
        public async Task<ActionResult<PagedResult<EquipmentResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            using HttpResponseMessage response = await _sharedClient.GetAsync("equipment");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PagedResult<TourResponseDto>>(jsonResponse);

            return Ok(data);
        }
    }
}
