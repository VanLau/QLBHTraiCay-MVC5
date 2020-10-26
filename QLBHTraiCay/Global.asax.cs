using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QLBHTraiCay
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application.Lock();
            Application["SoNguoiOnline"] = 0;
            Application.UnLock();
        }

        protected void Session_Start()
        {
            Application.Lock();
            int d = (int)Application["SoNguoiOnline"];
            Application["SoNguoiOnline"] = d + 1;
            Application.UnLock();
        }

        protected void Session_End()
        {
            Application.Lock();
            int d = (int)Application["SoNguoiOnline"];
            Application["SoNguoiOnline"] = d - 1;
            Application.UnLock();
        }

        //protected void Application_End()
        //{

        //}
    }
}
