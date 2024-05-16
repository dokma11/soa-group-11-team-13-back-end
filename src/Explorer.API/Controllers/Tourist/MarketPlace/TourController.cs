using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist.MarketPlace
{
    [Route("api/market-place")]
    public class TourController : BaseApiController
    {
        private readonly ITourService _tourService;
        private readonly IShoppingCartService _shoppingCartService;

        public TourController(ITourService service, IShoppingCartService shoppingCartService)
        {
            _tourService = service;
            _shoppingCartService = shoppingCartService;
        }

        private static readonly HttpClient _sharedClient = new()
        {
            BaseAddress = new Uri("http://tours:8081/"),
        };

        /*[Authorize(Roles = "author, tourist")]
        [HttpGet("tours/published")]
        public async Task<ActionResult<PagedResult<TourResponseDto>>> GetPublishedTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var response = await _sharedClient.GetFromJsonAsync<List<TourResponseDto>>("tours/published");

            if (response != null)
            {
                var pagedResult = new PagedResult<TourResponseDto>(response, response.Count);
                return Ok(pagedResult);
            }

            return BadRequest();
        }

        [HttpGet("tours/{tourId:long}")]
        public async Task<ActionResult<TourResponseDto>> GetById(long tourId)
        {
            var response = await _sharedClient.GetFromJsonAsync<TourResponseDto>("tours/" + tourId);

            if (response != null)
            {
                return Ok(response);
            }

            return BadRequest();
        }*/

        [HttpGet("tours/can-be-rated/{tourId:long}")]
        public bool CanTourBeRated(long tourId)
        {
            long userId = extractUserIdFromHttpContext();
            return _tourService.CanTourBeRated(tourId, userId).Value;
        }

        private long extractUserIdFromHttpContext()
        {
            return long.Parse((HttpContext.User.Identity as ClaimsIdentity).FindFirst("id")?.Value);
        }

        [Authorize(Policy = "touristPolicy")]
        [Authorize(Roles = "tourist")]
        [HttpGet("tours/inCart/{id:long}")]
        public ActionResult<PagedResult<LimitedTourViewResponseDto>> GetToursInCart([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            var cart = _shoppingCartService.GetByTouristId(id);
            if (cart.Value == null)
            {
                return NotFound();
            }
            var tourIds = cart.Value.OrderItems.Select(order => order.TourId).ToList();
            var result = _tourService.GetLimitedInfoTours(page, pageSize, tourIds);
            return CreateResponse(result);
        }
        /*[HttpGet("tours/inCart/{id:long}")]
        public ActionResult<PagedResult<LimitedTourViewResponseDto>> GetToursInCart([FromQuery] int page, [FromQuery] int pageSize, long id)
        {
            var cart = _shoppingCartService.GetByTouristId(id);
            if (cart == null)
            {
                return NotFound();
            }
            var tourIds = cart.Value.OrderItems.Select(order => order.TourId).ToList();
            var result = _tourService.GetLimitedInfoTours(page, pageSize, tourIds);
            return CreateResponse(result);
        }*/

        [HttpGet("tours/adventure")]
        public ActionResult<PagedResult<TourResponseDto>> GetPopularAdventureTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetAdventureTours(page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("tours/family")]
        public ActionResult<PagedResult<TourResponseDto>> GetPopularFamilyTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetFamilyTours(page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("tours/cruise")]
        public ActionResult<PagedResult<TourResponseDto>> GetPopularCruiseTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetCruiseTours(page, pageSize);
            return CreateResponse(result);
        }

        [HttpGet("tours/cultural")]
        public ActionResult<PagedResult<TourResponseDto>> GetPopularCulturalTours([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourService.GetCulturalTours(page, pageSize);
            return CreateResponse(result);
        }

    }
}
