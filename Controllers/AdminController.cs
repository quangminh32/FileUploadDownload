using Microsoft.AspNetCore.Mvc;

namespace FileUploadDownload.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 