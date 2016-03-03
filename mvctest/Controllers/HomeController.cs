using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Bll;

namespace mvctest.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index ()
		{
			var mvcName = typeof(Controller).Assembly.GetName ();
			var isMono = Type.GetType ("Mono.Runtime") != null;

			ViewData ["Version"] = mvcName.Version.Major + "." + mvcName.Version.Minor;
			ViewData ["Runtime"] = isMono ? "Mono" : ".NET";
			var sb = new StringBuilder();
			var menuResult = new OperationBll().GetMenuTree();
			ViewBag.Menus = menuResult;
			/*foreach (var menu in menuResult) {
				if (menu.children.Count == 0) {
					sb.Append ("<ul class=\"Menus\">");
					sb.Append (string.Format ("<li class=\"menuClick\" url=\"/{0}/{1}\">{2}</li>", menu.link, menu.link, menu.text));
					sb.Append ("</ul>");
				} else {
					sb.Append (string.Format(" <div title={0}>",menu.text));
					sb.Append ("<ul title=\"Menus\">");
					foreach (var ch in menu.children) {
						sb.Append (string.Format ("<li class=\"menuClick\" url=\"/{0}/{1}\">{2}</li>", ch.link, ch.link, ch.text));
					}
					sb.Append ("</ul");
					sb.Append ("</div>");
				}
			}*/
			if(menuResult.Count>0) ViewBag.HasMenu="yes";
			else ViewBag.HasMenu="No";
			ViewData ["Menus"] = menuResult;
			return View("Index");
		}

	}
}

