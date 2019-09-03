using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MileageTracker.WebAPI
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MTBaseController : ControllerBase
    {

    }
}