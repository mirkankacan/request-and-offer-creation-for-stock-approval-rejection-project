using System;
using System.Web.Mvc;

namespace U_Project.Controllers
{
    public class ErrorsController : Controller
    {
        // GET: Errors
        public ActionResult Error404(Exception exception)
        {
            var model = new HandleErrorInfo(exception, "Error", "Index");
            return View(model);
        }

        public ActionResult Error500(Exception exception)
        {
            var model = new HandleErrorInfo(exception, "Error", "Index");
            return View(model);
        }
    }
}