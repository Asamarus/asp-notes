using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Tests.Misc;

[ApiController]
[AllowAnonymous]
[Route("test")]
public class TestControllerWithRequestHeaderEcho : ControllerBase
{
    [HttpGet("echo-headers")]
    public IActionResult EchoHeaders()
    {
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        return Ok(headers);
    }
}
