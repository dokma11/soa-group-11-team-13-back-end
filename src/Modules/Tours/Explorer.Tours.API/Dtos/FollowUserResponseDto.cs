namespace Explorer.Tours.API.Dtos;

public class FollowUserResponseDto
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ProfilePicture { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive{ get; set; }
}

public enum UserRole
{
    Administrator, 
    Author,
    Tourist
}
