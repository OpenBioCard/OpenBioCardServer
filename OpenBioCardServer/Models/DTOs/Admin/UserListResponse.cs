namespace OpenBioCardServer.Models.DTOs.Admin;

public class UserListResponse
{
    public List<UserInfoDto> Users { get; set; } = new();
}