namespace Explorer.Blog.API.Dtos;

public class BlogRecommendationMicorserviceRequestDto
{
    public long BlogId { get; set; }
    public long RecommendationReceiverId { get; set; }
    public long RecommenderId { get; set; }
}
