using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace AbdusCo.Matbaa.Http
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
}