using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OpenBioCardServer.Models.DTOs;

namespace OpenBioCardServer.Controllers;

[ApiController]
[Route("")]
public class SystemInfoController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public SystemInfoController(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    /// <summary>
    /// GET /check-backend - 获取后端实现信息
    /// </summary>
    [HttpGet("check-backend")]
    public IActionResult CheckBackend()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        var assemblyVersion = version != null 
            ? $"{version.Major}.{version.Minor}.{version.Build}" 
            : "1.0.0";

        // 尝试从配置中读取自定义版本号
        var customVersion = _configuration["AppSettings:Version"];
        var response = new BackendInfoResponse
        {
            Backend = "OpenBioCardServer",
            Version = customVersion ?? assemblyVersion,
            Environment = _environment.EnvironmentName,
            BuildDate = null,
        };

        return Ok(response);
    }
}
