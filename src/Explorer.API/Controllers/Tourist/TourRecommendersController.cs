using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tourrecommenders")]
    public class TourRecommendersController : BaseApiController
    {
        private readonly IToursRecommendersService _tourRecommendersService;

        public TourRecommendersController(IToursRecommendersService tourRecommendersService)
        {
            _tourRecommendersService = tourRecommendersService;
        }

        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://localhost:8081/"),
        };

        [Route("activetours")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<TourResponseDto>>> GetActiveTours()
        {
            var response = await _sharedClient.GetFromJsonAsync<List<TourResponseDto>>("tours/published");

            if (response != null)
            {
                var pagedResult = new PagedResult<TourResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }
        [Route("recommendedtours")]
        [HttpGet]
        public ActionResult<PagedResult<TourResponseDto>> getRecommendedTours()
        {
            long id = 0;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                id = long.Parse(identity.FindFirst("id").Value);
            }
            var result = _tourRecommendersService.GetRecommendedTours(id);
            return CreateResponse(result);
        }
    }
}
