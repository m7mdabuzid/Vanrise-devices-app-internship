using Microsoft.AspNetCore.Mvc;

namespace Devices_tasks1_2_3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Clients()
        {
            return View();
        }

        public IActionResult PhoneNumbers()
        {
            return View();
        }

        public IActionResult PhoneNumberReservations()
        {
            return View();
        }
        public IActionResult ClientTypeReport()
        {
            return View("~/Views/Home/ClientTypeReport.cshtml");
        }

        public IActionResult PhoneNumberStatusReport()
        {
            return View(
                "~/Views/Home/PhoneNumberStatusReport.cshtml"
            );
        }
    }
}