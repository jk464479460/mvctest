using Bll;
using DemoSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DemoSite.Controllers
{
    public class UserGroupController : Controller
    {
        //角色管理
        public ActionResult UserGroup()
        {
            var menuResult = new OperationBll().GetMenuTree();
            ViewBag.Menus = menuResult;
            return View();
        }
        public JsonResult GetRoleList()
        {
            var pageSize = HttpContext.Request["rows"];
            var curPage = HttpContext.Request["page"];
            var whereStr = HttpContext.Request["inputs"];
            var total = 0;

            var roles = new OperationBll().GetRoleList(int.Parse(pageSize), int.Parse(curPage), out total,whereStr);//

            
            var res = (from d in roles select new RoleModel { RoleID = d.ID, Description=d.DESCRIPTION,
                        LastUpdateTime=d.LastUpdateTime==null?"-":string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                        d.LastUpdateTime.Value.Year,d.LastUpdateTime.Value.Month,
                        d.LastUpdateTime.Value.Day, d.LastUpdateTime.Value.Hour,
                        d.LastUpdateTime.Value.Minute, d.LastUpdateTime.Value.Second),RoleName=d.CNAME }).ToList();
            var result = new { total = total, rows = res };
            return Json(result);
        }
        public JsonResult GetRoleListById()
        {
            var result = new OperationBll().GetRoleById(null);
            return Json(result);
        }
        public JsonResult AddRole()
        {
            var param = HttpContext.Request["roleName"];
            var descri = HttpContext.Request["roleDescri"];
            var role = new TB_USERGROUP {
                CNAME = param,
                DESCRIPTION = string.IsNullOrEmpty(descri) ? string.Empty : descri
            };
            var res = new OperationBll().AddRole(role);
            return Json(res);
        }
        public JsonResult DelRole()
        {
            var param = HttpContext.Request["inputs"];
            var res = new OperationBll().DelRole(param);
            return Json(res);
        }
    }
}