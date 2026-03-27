using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class InvalidatedToken : BaseEntity
{ 
    public string Jti { get; set; } = null!;            // JWT ID claim
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime InvalidatedAt { get; set; } = DateTime.UtcNow;
}