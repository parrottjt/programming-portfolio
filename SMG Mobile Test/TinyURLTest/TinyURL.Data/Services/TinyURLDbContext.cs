using System.Data.Entity;
using TinyURL.Data.Models;

namespace TinyURL.Data.Services
{
    public class TinyURLDbContext : DbContext
    {
        public DbSet<UploadedImage> UploadedImages { get; set; }
    }
}