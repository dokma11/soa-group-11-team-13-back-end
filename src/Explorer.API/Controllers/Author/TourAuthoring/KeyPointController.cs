using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Author.TourAuthoring;

[Route("api/tour-authoring")]
public class KeyPointController : BaseApiController
{
    private static readonly HttpClient _sharedClient = new()
    {
        BaseAddress = new Uri("http://tours:8081/"),
    };

    [Authorize(Roles = "author")]
    [HttpPost("tours/{tourId:long}/key-points")]
    public async Task<ActionResult<KeyPointResponseDto>> CreateKeyPoint([FromRoute] long tourId, [FromBody] KeyPointCreateDto keyPoint)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        keyPoint.TourId = tourId;

        try
        {
            string json = JsonConvert.SerializeObject(keyPoint);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _sharedClient.PostAsync("keyPoints", content);
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

    [Authorize(Roles = "author")]
    [HttpPut("tours/{tourId:long}/key-points/{id:long}")]
    public async Task<ActionResult<KeyPointResponseDto>> Update(long tourId, long id, [FromBody] KeyPointUpdateDto keyPoint)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        keyPoint.Id = id;

        try
        {
            string json = JsonConvert.SerializeObject(keyPoint);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _sharedClient.PutAsync("keyPoints/", content);
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

    [Authorize(Roles = "author, tourist")]
    [HttpDelete("tours/{tourId:long}/key-points/{id:long}")]
    public async Task<ActionResult> Delete(long tourId, long id)
    {
        try
        {
            var response = await _sharedClient.DeleteAsync("keyPoints/" + id);
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

    [Authorize(Roles = "author")]
    [HttpGet]
    public async Task<ActionResult<PagedResult<KeyPointResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var response = await _sharedClient.GetFromJsonAsync<List<KeyPointResponseDto>>("keyPoints/");

        if (response != null)
        {
            var pagedResult = new PagedResult<KeyPointResponseDto>(response, response.Count);
            return Ok(pagedResult);
        }

        return BadRequest();
    }
}
