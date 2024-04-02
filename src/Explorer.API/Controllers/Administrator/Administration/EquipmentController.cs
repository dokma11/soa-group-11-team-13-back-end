using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.Tours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/equipment")]
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
        public ActionResult<PagedResult<EquipmentResponseDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _equipmentService.GetPaged(page, pageSize);
            return CreateResponse(result);
        }

        [HttpPost]
        public async Task<ActionResult<EquipmentResponseDto>> Create([FromBody] EquipmentCreateDto equipment)
        {
            //var result = _equipmentService.Create(equipment);
            //return CreateResponse(result);

            string json = JsonConvert.SerializeObject(equipment);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _sharedClient.PostAsync("equipment", content);

            response.EnsureSuccessStatusCode();

            return Ok(response);

        }

        [HttpPut("{id:long}")]
        public ActionResult<EquipmentResponseDto> Update([FromBody] EquipmentUpdateDto equipment)
        {
            var result = _equipmentService.Update(equipment);
            return CreateResponse(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _equipmentService.Delete(id);
            return CreateResponse(result);
        }
    }
}
