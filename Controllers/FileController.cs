using Microsoft.AspNetCore.Mvc;
using FileUploadDownload.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FileUploadDownload.Controllers
{
    public class FileController : Controller
    {
        private readonly FileUploadDbContext _context;
        public FileController(FileUploadDbContext context)
        {
            _context = context;
        }

        // GET: /File
        public IActionResult Index()
        {
            var files = _context.Files.ToList();
            return View(files);
        }

        // GET: /File/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /File/Upload
        [HttpPost]
        public IActionResult Upload(List<IFormFile> uploadedFiles)
        {
            try
            {
                if (uploadedFiles != null && uploadedFiles.Count > 0)
                {
                    foreach (var file in uploadedFiles)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            _context.Files.Add(new FileModel
                            {
                                FileName = file.FileName,
                                ContentType = file.ContentType,
                                FileData = ms.ToArray()
                            });
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi upload file: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // GET: /File/Download/5
        public IActionResult Download(int id)
        {
            var file = _context.Files.FirstOrDefault(f => f.Id == id);
            if (file == null) return NotFound();
            return File(file.FileData, file.ContentType, file.FileName);
        }

        // POST: /File/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var file = _context.Files.FirstOrDefault(f => f.Id == id);
                if (file != null)
                {
                    _context.Files.Remove(file);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi xóa file: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
