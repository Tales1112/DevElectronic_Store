using Microsoft.AspNetCore.Mvc;

namespace DevElectronic_Store.Controllers
{
    public class IdentidadeController : Controller
    {
        [HttpGet("nova-conta")]
        public IActionResult Registro()
        {
            return View();
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}
