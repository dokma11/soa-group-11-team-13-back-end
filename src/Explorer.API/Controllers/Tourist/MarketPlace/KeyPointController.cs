using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.TourAuthoring;
using Explorer.Tours.Core.Domain.Tours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.MarketPlace
{
    [Route("api/market-place")]
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

        [Authorize(Roles = "author, tourist")]
        [HttpGet("tours/{tourId:long}/key-points")]
        public async Task<ActionResult<KeyPointResponseDto>> GetKeyPoints(long tourId)
        {
            var response = await _sharedClient.GetFromJsonAsync<List<KeyPointResponseDto>>("keyPoints/tour/" + tourId);

            return Ok(response);
        }

        [Authorize(Roles = "tourist")]
        [HttpGet("{campaignId:long}/key-points")]
        public ActionResult<KeyPointResponseDto> GetCampaignKeyPoints(long campaignId)
        {
            var result = _keyPointService.GetByCampaignId(campaignId);
            return CreateResponse(result);
        }

        [Authorize(Roles = "author, tourist")]
        [HttpGet("tours/{tourId:long}/firts-key-point")]
        public ActionResult<KeyPointResponseDto> GetToursFirstKeyPoint(long tourId)
        {
            var result = _keyPointService.GetFirstByTourId(tourId);
            return CreateResponse(result);
        }
    }
}
