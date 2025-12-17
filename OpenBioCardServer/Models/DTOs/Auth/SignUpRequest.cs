namespace OpenBioCardServer.Models.DTOs.Auth;

public class SignUpRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // "user" or "admin"
}