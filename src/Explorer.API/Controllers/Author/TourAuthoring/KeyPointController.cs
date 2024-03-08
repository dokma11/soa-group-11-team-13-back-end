using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.TourAuthoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Explorer.API.Controllers.Author.TourAuthoring;

[Route("api/tour-authoring")]
public class KeyPointController : BaseApiController
{
    private readonly IKeyPointService _keyPointService;

    public KeyPointController(IKeyPointService keyPointService)
    {
        _keyPointService = keyPointService;
    }

    private static readonly HttpClient _sharedClient = new()
    {
        BaseAddress = new Uri("http://localhost:8081/"),
    };

    [Authorize(Roles = "author")]
    [HttpPost("tours/{tourId:long}/key-points")]
    public async Task<ActionResult<KeyPointResponseDto>> CreateKeyPoint([FromRoute] long tourId, [FromBody] KeyPointCreateDto keyPoint)
    {
        keyPoint.TourId = tourId;
        string json = JsonConvert.SerializeObject(keyPoint);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _sharedClient.PostAsync("keyPoints", content);
        response.EnsureSuccessStatusCode();

        return Ok(response);
    }

    [Authorize(Roles = "author")]
    [HttpPut("tours/{tourId:long}/key-points/{id:long}")]
    public async Task<ActionResult<KeyPointResponseDto>> Update(long tourId, long id, [FromBody] KeyPointUpdateDto keyPoint)
    {
        keyPoint.Id = id;
        string json = JsonConvert.SerializeObject(keyPoint);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await _sharedClient.PutAsync("keyPoints/", content);
        response.EnsureSuccessStatusCode();

        return Ok(response);
    }

    [Authorize(Roles = "author, tourist")]
    [HttpDelete("tours/{tourId:long}/key-points/{id:long}")]
    public async Task<ActionResult> Delete(long tourId, long id)
    {
        var response = await _sharedClient.DeleteAsync("keyPoints/" + id);
        response.EnsureSuccessStatusCode();
        return Ok(response);
    }

    [Authorize(Roles = "author")]
    [HttpGet]
    public ActionResult<PagedResult<KeyPointResponseDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _keyPointService.GetPaged(page, pageSize);
        return CreateResponse(result);
    }
}
