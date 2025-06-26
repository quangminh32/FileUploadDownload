using Microsoft.AspNetCore.Mvc;

namespace FileUploadDownload.Controllers
{
    public class ChatHubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 