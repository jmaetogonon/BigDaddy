namespace BigDaddy.Domain.Users;

public class InvalidatedToken
{
    public int Id { get; set; }
    public string Jti { get; set; } = null!;            // JWT ID claim
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime InvalidatedAt { get; set; } = DateTime.UtcNow;
}