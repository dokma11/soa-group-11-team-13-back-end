namespace Explorer.Blog.API.Dtos;

public class BlogRecommendationResponseDto
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public int RecommenderId { get; set; }
    public int RecommendationReceiverId { get; set; }
    public BlogResponseDto Blog { get; set; }
}
