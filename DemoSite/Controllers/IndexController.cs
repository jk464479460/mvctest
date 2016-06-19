using Bll;
using System.Web.Mvc;

namespace DemoSite.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            var menuResult = new OperationBll().GetMenuTree();
            ViewBag.Menus = menuResult;
            return View("Index");
        }
    }
}