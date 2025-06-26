using Microsoft.EntityFrameworkCore;

namespace FileUploadDownload.Models
{
    public class FileUploadDbContext : DbContext
    {
        public FileUploadDbContext(DbContextOptions<FileUploadDbContext> options) : base(options) { }

        public DbSet<FileModel> Files { get; set; }
    }
} 