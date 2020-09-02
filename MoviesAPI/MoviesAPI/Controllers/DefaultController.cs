using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : ControllerBase
    {
        [Route("/")]
        public RedirectResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}