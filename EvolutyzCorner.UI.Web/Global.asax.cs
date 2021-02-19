using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Data.SqlClient;
using System.IO.Compression;
using System.Web.Security;
using System.Security.Principal;

namespace EvolutyzCorner.UI.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
          
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            MvcHandler.DisableMvcResponseHeader = true;
            
        }

        public class SessionExpireAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;

                if (ctx.Session != null)
                {
                    // check if a new session id was generated
                    if (ctx.Session.IsNewSession)
                    {
                        // If it says it is a new session, but an existing cookie exists, then it must have timed out
                        string sessionCookie = ctx.Request.Headers["Cookie"];
                        if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                        {
                            FormsAuthentication.SignOut();
                            ctx.Response.Redirect("~/Home/Login");
                        }
                    }
                }

                base.OnActionExecuting(filterContext);
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                   RouteValueDictionary(new { controller = "Home", action = "Login", area = "" }));
                }
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;
                        string userData = ticket.UserData;
                        string[] roles = userData.Split(',');
                        HttpContext.Current.User = new GenericPrincipal(id, roles);
                    }
                }
            }
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();
        //    Server.ClearError();
        //    Response.Clear();
        //    if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404)
        //    {
        //        Response.Redirect("~/Home/Login");
        //    }
        //    else
        //    {
        //        Response.Redirect("~/Home/Login");
        //        // your global error handling here!
        //    }
        //}
        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }

        protected void Application_PreSendRequestHeaders()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
            }
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies[".ASPXAUTH"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    // redirect to the login page in real application
                    Response.Redirect("~/Home/Login");
                }
            }
            else
            {
                Response.Redirect("~/Home/Login");

            }

        }

    }
}
