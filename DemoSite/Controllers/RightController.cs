using Bll;
using System.Web.Mvc;
using Newtonsoft.Json;
using DemoSite.Models;
using System.Linq;

namespace DemoSite.Controllers
{
    public class RightController : Controller
    {
        public ActionResult Right()
        {
            var menuResult = new OperationBll().GetMenuTree();
            ViewBag.Menus = menuResult;
            return View();
        }
        public JsonResult GetMenuTree()
        {
            var result = new OperationBll().GetMenuTree();
            return Json(result);
        }
        public JsonResult ModifyRights()
        {
            var param = HttpContext.Request["inputs"];
            var obj = JsonConvert.DeserializeObject<RoleRights>(param);
            var query = new Bll.RoleRightsBll
            {
                RoleId=obj.RoleId,
                RightsType=1,
                Data=obj.Data
            };
            var result = new OperationBll().ModifyRights(query);
            return Json(result);
        }
        public JsonResult GetRightsByRoleId()
        {
            var param = HttpContext.Request["inputs"];
            var res = new OperationBll().GetURights(int.Parse(param));
            return Json(res);
        }

        public JsonResult GetOperListByMenu()
        {
            var param = HttpContext.Request["data"];
            var total = 0;
            var res = new OperationBll().GetPageOperList(int.Parse(param));
            total = res.OperList.Count;
            var result = new { total = total, rows = res.OperList };
            return Json(result);
        }

        public JsonResult ModifyOperRights()
        {
            var param = HttpContext.Request["inputs"];
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<OperModel>(param);
            var res = new OperationBll().ModifyOperRights(obj.roleId, obj.pageId, (from p in obj.ids.Split(',') select int.Parse(p)).ToList());
            return Json(res);
        }
    }
}