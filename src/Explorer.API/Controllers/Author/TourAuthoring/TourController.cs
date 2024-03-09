using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.Tours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Explorer.API.Controllers.Author.TourAuthoring
{

    [Route("api/tour")]
    public class TourController : BaseApiController
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://localhost:8081/"),
        };

        [Authorize(Roles = "author")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<TourResponseDto>>> GetAll()
        {
            using HttpResponseMessage response = await _sharedClient.GetAsync("tours");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PagedResult<TourResponseDto>>(jsonResponse);

            return Ok(data);
        }

        [Authorize(Roles = "author")]
        [HttpGet("published")]
        public ActionResult<PagedResult<TourResponseDto>> GetPublished([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetPublished(page, pageSize);
            return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpGet("authors")]
        public async Task<ActionResult<PagedResult<TourResponseDto>>> GetAuthorsTours()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var id = long.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<TourResponseDto>>("tours/authors/" + id);

            if (response != null)
            {
                var pagedResult = new PagedResult<TourResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [Authorize(Roles = "author, tourist")]
        [HttpPost]
        public async Task<ActionResult<TourResponseDto>> Create([FromBody] TourCreateDto tour)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                tour.AuthorId = long.Parse(identity.FindFirst("id").Value);
            }

            string json = JsonConvert.SerializeObject(tour);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _sharedClient.PostAsync("tours", content);

            response.EnsureSuccessStatusCode();

            return Ok(response);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TourResponseDto>> Update([FromBody] TourUpdateDto tour)
        {
            string json = JsonConvert.SerializeObject(tour);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _sharedClient.PutAsync("tours", content);
            response.EnsureSuccessStatusCode();
            return Ok(response);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var result = _tourService.DeleteCascade(id);
            return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpGet("equipment/{tourId:int}")]
        public async Task<ActionResult> GetEquipment(int tourId)
        {
            //var result = _tourService.GetEquipment(tourId);

            var response = await _sharedClient.GetFromJsonAsync<List<EquipmentResponseDto>>("tours/" + tourId + "/equipment");
            //response.EnsureSuccessStatusCode();

            //var jsonResponse = await response.Content.ReadAsStringAsync();
            //var data = JsonConvert.DeserializeObject<PagedResult<EquipmentResponseDto>>(jsonResponse);

            // za go dodati http
            if (response != null)
            {
                var pagedResult = new PagedResult<EquipmentResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
            //return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpPost("equipment/{tourId:int}/{equipmentId:int}")]
        public async Task<ActionResult> AddEquipment(int tourId, int equipmentId)
        {
            //var result = _tourService.AddEquipment(tourId, equipmentId);

            // za go dodati http

            HttpResponseMessage response = await _sharedClient.PostAsync("tours/" + tourId + "/equipment/" + equipmentId, null);

            response.EnsureSuccessStatusCode();

            return Ok(response);
            //return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpDelete("equipment/{tourId:int}/{equipmentId:int}")]
        public async Task<ActionResult> DeleteEquipment(int tourId, int equipmentId)
        {
            //var result = _tourService.DeleteEquipment(tourId, equipmentId);

            // za go dodati http
            HttpResponseMessage response = await _sharedClient.DeleteAsync("tours/" + tourId + "/equipment/" + equipmentId);

            response.EnsureSuccessStatusCode();

            return Ok(response);
            //return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpGet("{tourId:long}")]
        public async Task<ActionResult<PagedResult<TourResponseDto>>> GetById(long tourId)
        {
            var response = await _sharedClient.GetFromJsonAsync<List<TourResponseDto>>("tours/" + tourId);

            if (response != null)
            {
                var pagedResult = new PagedResult<TourResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [Authorize(Roles = "author")]
        [HttpPut("publish/{id:int}")]
        public async Task<ActionResult<TourResponseDto>> Publish(long id)
        {
            var response = await _sharedClient.PutAsync("tours/publish/" + id, null);
            response.EnsureSuccessStatusCode();
            return Ok(response);
        }


        [Authorize(Roles = "author, tourist")]
        [HttpPut("durations/{id:int}")]
        public async Task<ActionResult<TourResponseDto>> AddDurations([FromBody] TourUpdateDto tour)
        {
            string json = JsonConvert.SerializeObject(tour);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _sharedClient.PutAsync("tours/durations", content);
            response.EnsureSuccessStatusCode();
            return Ok(response);
        }

        [Authorize(Roles = "author")]
        [HttpPut("archive/{id:int}")]
        public async Task<ActionResult<TourResponseDto>> Archive(long id)
        {
            var response = await _sharedClient.PutAsync("tours/archive/" + id, null);
            response.EnsureSuccessStatusCode();
            return Ok(response);
        }

        [Authorize(Roles = "tourist")]
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
        }
    }
}
