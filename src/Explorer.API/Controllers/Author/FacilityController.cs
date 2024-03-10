using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/facility")]
    public class FacilityController : BaseApiController
    {
        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://localhost:8081/"),
        };

        [HttpGet]
        public async Task<ActionResult<PagedResult<FacilityResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _sharedClient.GetFromJsonAsync<List<FacilityResponseDto>>("facilities");

            if (response != null)
            {
                var pagedResult = new PagedResult<FacilityResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [HttpGet("authorsFacilities")]
        public async Task<ActionResult<PagedResult<FacilityResponseDto>>> GetByAuthorId([FromQuery] int page, [FromQuery] int pageSize)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var loggedInAuthor = int.Parse(identity.FindFirst("id").Value);

            var response = await _sharedClient.GetFromJsonAsync<List<FacilityResponseDto>>("facilities/author/" + loggedInAuthor);

            if (response != null)
            {
                var pagedResult = new PagedResult<FacilityResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<FacilityResponseDto>> Create([FromBody] FacilityCreateDto facility)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                facility.AuthorId = int.Parse(identity.FindFirst("id").Value);
            }

            try
            {
                string json = JsonConvert.SerializeObject(facility);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _sharedClient.PostAsync("facilities", content);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<FacilityResponseDto>> Update(int id, [FromBody] FacilityUpdateDto facility)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            facility.Id = id;

            try
            {
                string json = JsonConvert.SerializeObject(facility);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                var response = await _sharedClient.PutAsync("facilities/", content);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await _sharedClient.DeleteAsync("facilities/" + id);
                response.EnsureSuccessStatusCode();
                return Ok(response);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
