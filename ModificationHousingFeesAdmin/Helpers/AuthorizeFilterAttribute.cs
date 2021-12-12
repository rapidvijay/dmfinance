using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModificationHousingFeesAdmin.Helpers
{
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Controller controller = filterContext.Controller as Controller;

            if (controller != null)
            {
                if (Common.User == null && !(filterContext.RouteData.Values.ContainsValue("Login")))
                {
                    controller.HttpContext.Response.Redirect("/Home/Login");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}