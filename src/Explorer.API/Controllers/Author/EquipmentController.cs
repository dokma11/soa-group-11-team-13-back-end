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
            BaseAddress = new Uri("http://tours:8081/"),
        };

        [HttpGet]
        public async Task<ActionResult<PagedResult<EquipmentResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _sharedClient.GetFromJsonAsync<List<EquipmentResponseDto>>("equipment");
            //response.EnsureSuccessStatusCode();

            //var jsonResponse = await response.Content.ReadAsStringAsync();
            //var data = JsonConvert.DeserializeObject<PagedResult<EquipmentResponseDto>>(jsonResponse);

            if (response != null)
            {
                var pagedResult = new PagedResult<EquipmentResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }
    }
}
