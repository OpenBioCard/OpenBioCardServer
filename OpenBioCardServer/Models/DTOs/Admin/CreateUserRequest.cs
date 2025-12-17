namespace OpenBioCardServer.Models.DTOs.Admin;

public class CreateUserRequest
{
    public string NewUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}