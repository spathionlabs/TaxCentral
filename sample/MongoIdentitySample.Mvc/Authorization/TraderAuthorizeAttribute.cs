using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;


namespace Main.Mvc.Authorization
{
    public class TraderAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter

    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
             var controllerInfo = filterContext.ActionDescriptor as ControllerActionDescriptor;
            if (filterContext != null)
            {
                if (!filterContext.HttpContext.User.IsInRole("Trader"))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
           {
            "Controller",
            "Home"
           }, {
            "Action",
            "Index"
           }
                        });
                }


                string controllerName = controllerInfo.ControllerName;

                if (controllerName != "Home")
                {
                    //filterContext.HttpContext.Request.Headers.ContainsKey
                    if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        filterContext.Result = new JsonResult("")
                        {
                            Value = new
                            {
                                Status = "Error"
                            },
                        };
                    }
       //             else
       //             {
       //                 filterContext.Result = new RedirectToRouteResult(
       //                  new RouteValueDictionary {
       //{
       // "Controller",
       // "Home"
       //}, {
       // "Action",
       // "SessionExpired"
       //}
       //                  });
       //             }
                }
            }

        }
    }
}

