using Microsoft.AspNetCore.Mvc;

namespace FileUploadDownload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file selected");

                // Kiểm tra file có phải là ảnh không
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    return BadRequest("Only image files are allowed");

                // Giới hạn kích thước file (5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size must be less than 5MB");

                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                // Tạo tên file unique
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = "/uploads/" + fileName;
                return Ok(new { 
                    success = true,
                    imageUrl = imageUrl,
                    fileName = file.FileName,
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = "Error uploading file: " + ex.Message 
                });
            }
        }
    }
} 