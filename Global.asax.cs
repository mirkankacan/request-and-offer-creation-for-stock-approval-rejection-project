using OfficeOpenXml;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using U_Project.Controllers;
using U_Project.Models;

namespace U_Project
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            // Hata durumuna göre yönlendirme yap
            if (exception is HttpException httpException)
            {
                int statusCode = httpException.GetHttpCode();
                if (statusCode == 404)
                {
                    Response.Redirect("/Errors/Error404");
                    return;
                }
            }

            // Hata bilgilerini hata sayfasına aktar
            var routeData = new RouteData();
            routeData.Values["controller"] = "Errors";
            routeData.Values["action"] = "Error500";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;

            // Diğer hata durumları için yönlendirme yap
            IController errorController = new ErrorsController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}