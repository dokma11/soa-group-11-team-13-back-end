using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.Core.Domain.Tours;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/tourist/comment")]
    public class CommentController : BaseApiController
    {
        private readonly ICommentService _commentService;
        private readonly IBlogService _blogService;
        private readonly IInternalUserService _internalUserService;

        private static readonly HttpClient _sharedClient = new HttpClient() { BaseAddress = new Uri("http://blogs:8082/") };

        public CommentController(ICommentService commentService, IBlogService blogService, IInternalUserService internalUserService)
        {
            _commentService = commentService;
            _blogService = blogService;
            _internalUserService = internalUserService;
        }


        [HttpPost]
        public async Task<ActionResult<CommentResponseDto>> Create([FromBody] CommentCreateDto comment)
        {
            /*
            if (_blogService.IsBlogClosed(comment.BlogId)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));

            var authorId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            comment.AuthorId = authorId;
            comment.CreatedAt = DateTime.UtcNow;
            var result = _commentService.Create(comment);
            return CreateResponse(result);
            */

            try
            { 

                comment.CreatedAt = DateTime.UtcNow;
                comment.AuthorId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
                
                string json = JsonConvert.SerializeObject(comment);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _sharedClient.PostAsync("comments", content);
                response.EnsureSuccessStatusCode();
                //_commentService.Create(comment); //kreiranje u PSW bazi
                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"HttpRequestException occurred: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{commentId:long}")]
        public async Task<ActionResult<CommentResponseDto>> Update([FromBody] CommentUpdateDto commentData, long commentId)
        {
            /*
            var senderId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var comment = _commentService.Get(commentId);

            if (_blogService.IsBlogClosed(comment.Value.BlogId)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));


            if (senderId != comment.Value.AuthorId)
            {
                return CreateResponse(Result.Fail(FailureCode.Forbidden));
            }
            commentData.Id = commentId;
            var result = _commentService.UpdateComment(commentData);
            return CreateResponse(result);
            */

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            commentData.Id = commentId;

            

            try
            {
                var senderId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

                HttpResponseMessage commentResponse = await _sharedClient.GetAsync($"comments/{commentId}");
                if (!commentResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)commentResponse.StatusCode, "Error retrieving comment");
                }

                var commentContent = await commentResponse.Content.ReadAsStringAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(commentContent);

                //var comment = _commentService.Get(commentId); //This is where I should call the _sharedClient.GetAsync("comments" + commentId); 

                if (senderId != comment.AuthorId)
                {
                    return CreateResponse(Result.Fail(FailureCode.Forbidden));
                }

                string json = JsonConvert.SerializeObject(commentData);
                StringContent content = new(json, Encoding.UTF8, "application/json");

                var response = await _sharedClient.PutAsync("comments", content);
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

        [HttpDelete("{commentId:long}")]
        public async Task<ActionResult> Delete(long commentId)
        {
            /*
            var senderId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
            var comment = _commentService.Get(commentId);

            if (_blogService.IsBlogClosed(comment.Value.BlogId)) return CreateResponse(Result.Fail(FailureCode.InvalidArgument));


            if (senderId != comment.Value.AuthorId)
            {
                return CreateResponse(Result.Fail(FailureCode.Forbidden));
            }
            var result = _commentService.Delete(commentId);
            return CreateResponse(result);
            */

            try
            {
                var senderId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);

                HttpResponseMessage commentResponse = await _sharedClient.GetAsync($"comments/{commentId}");
                if (!commentResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)commentResponse.StatusCode, "Error retrieving comment");
                }

                var commentContent = await commentResponse.Content.ReadAsStringAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(commentContent);

                //var comment = _commentService.Get(commentId);
                if (senderId != comment.AuthorId)
                {
                    return CreateResponse(Result.Fail(FailureCode.Forbidden));
                }
                var response = await _sharedClient.DeleteAsync("comments/" + commentId);
                response.EnsureSuccessStatusCode();
                _commentService.Delete(commentId);
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

        [HttpGet("{blogId:long}")]
        public async Task<ActionResult<PagedResult<CommentResponseDto>>> GetAll([FromQuery] int page, [FromQuery] int pageSize, long blogId)
        {
            
            //var result = _commentService.GetPagedByBlogId(page, pageSize, blogId);
            //return CreateResponse(result);
            /*
            try
            {
                using HttpResponseMessage response = await _sharedClient.GetAsync("comments/byBlog/" + blogId);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<CommentResponseDto>>(jsonResponse);
                var pagedResult = new PagedResult<CommentResponseDto>(data!, data!.Count);

                return Ok(pagedResult);
            }
            catch (HttpRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
            */
            try
            {
                string requestUri = $"comments/byBlog/{blogId}?page={page}&pageSize={pageSize}";
                HttpResponseMessage response = await _sharedClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var commentPagedResponseDto = JsonConvert.DeserializeObject<CommentPagedResponseDto>(jsonResponse);

                foreach(var commentDto in commentPagedResponseDto.Comments) {
                    var user = _internalUserService.Get(commentDto.AuthorId).Value;
                    commentDto.Author = user;
                }

                var pagedResult = new PagedResult<CommentResponseDto>(commentPagedResponseDto.Comments, commentPagedResponseDto.TotalCount);

                // You need to also retrieve the total count of comments for pagination purposes
                // This could be included in the response headers, or as part of the response body if your microservice supports it

                // Assuming totalCount is retrieved from the response
                //int totalCount = RetrieveTotalCount(response); // You'll need to implement this based on your microservice's response

                //var pagedResult = new PagedResult<CommentResponseDto>(comments, totalCount);

                return Ok(pagedResult);
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
