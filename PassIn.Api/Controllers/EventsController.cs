using Microsoft.AspNetCore.Mvc;

namespace PassIn.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    [HttpPost]
    public IActionResult Register()
    {
        return Created();
    }
}
