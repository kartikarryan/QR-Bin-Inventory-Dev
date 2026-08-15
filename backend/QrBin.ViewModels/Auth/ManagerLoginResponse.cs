namespace QrBin.ViewModels.Auth;

public class ManagerLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public ManagerProfileResponse Manager { get; set; } = null!;
}

public class ManagerProfileResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
}
