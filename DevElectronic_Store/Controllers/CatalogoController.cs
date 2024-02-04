using Microsoft.AspNetCore.Mvc;

namespace DevElectronic_Store.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("produto-detalhe")]
        public IActionResult ProdutoDetalhe()
        {
            return View();
        }
    }
}
