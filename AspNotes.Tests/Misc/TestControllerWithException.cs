using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNotes.Tests.Misc;
public class TestControllerWithException : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("test")]
    public IActionResult Test()
    {
        throw new Exception("Test exception");
    }
}