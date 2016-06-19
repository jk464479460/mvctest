using DemoSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bll;

namespace DemoSite.Controllers
{
    public class UserController : Controller
    {
        public new ActionResult User()
        {
            var menuResult = new OperationBll().GetMenuTree();
            ViewBag.Menus = menuResult;
            return View();
        }

        public JsonResult GetUserList()
        {
            var pageSize = HttpContext.Request["rows"];
            var curPage = HttpContext.Request["page"];
            var whereStr = HttpContext.Request["inputs"];
            var total = 0;
            var users = new OperationBll().GetUserList(int.Parse(pageSize), int.Parse(curPage), out total, whereStr);

            var res = (from d in users
                       select new UserModel
                       {
                           UserId = d.ID,
                           RoleId=(new OperationBll().GetRoleById(int.Parse(d.GROUPS))[0]).ID,
                           RoleName= (new OperationBll().GetRoleById(int.Parse(d.GROUPS))[0]).CNAME,
                           LastUpdateTime = d.LastUpdateTime == null ? "-" : string.Format("{0}-{1}-{2} {3}:{4}:{5}",
        d.LastUpdateTime.Value.Year, d.LastUpdateTime.Value.Month,
        d.LastUpdateTime.Value.Day, d.LastUpdateTime.Value.Hour,
        d.LastUpdateTime.Value.Minute, d.LastUpdateTime.Value.Second),
                           UserName = d.CNAME,
                           Status=d.STATUS
                       }).ToList();
            var result = new { total = total, rows = res };

            return Json(result);
        }

        public JsonResult AddUser()
        {
            var param1 = HttpContext.Request["userName"];
            var param2 = HttpContext.Request["roleID"];
            var password = HttpContext.Request["password"];
            var status = HttpContext.Request["status"];
            var user = new TB_USER {
                CNAME = param1,
                GROUPS = param2,
                PASSWORD = password,
                LastUpdateTime = DateTime.Now,
                STATUS=short.Parse(status)
            };
            var res = new OperationBll().AddUser(user);
            return Json(res);
        }

        public JsonResult DelUser()
        {
            var param = HttpContext.Request["inputs"];
            var res = new OperationBll().DelUser(param);
            return Json(res);
        }
    }
}