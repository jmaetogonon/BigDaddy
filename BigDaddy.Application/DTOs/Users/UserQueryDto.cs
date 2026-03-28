namespace BigDaddy.Application.DTOs.Users;

public class UserQueryDto
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int? RoleId { get; set; }
    public int? TeamId { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDir { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
