using System.Collections.Generic;
using System.Linq;
using TinyURL.Data.Models;

namespace TinyURL.Data.Services
{
    public class SqlUploadedImageData : IUploadedImage
    {
        readonly TinyURLDbContext db;

        public SqlUploadedImageData(TinyURLDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<UploadedImage> GetAll()
        {
            var images = db.UploadedImages.OrderBy(image => image.Id);
            return images;
        }

        public UploadedImage Get(int id)
        {
            return db.UploadedImages.FirstOrDefault(image => image.Id == id);
        }

        public UploadedImage Get(string url)
        {
            return db.UploadedImages.FirstOrDefault(image => image.TinyURL == url);
        }

        public void AddUploadedImage(UploadedImage uploadedImages)
        {
            db.UploadedImages.Add(uploadedImages);
            db.SaveChanges();
        }

        public void UpdateUploadedImage(UploadedImage uploadedImages)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteUploadedImage(UploadedImage uploadedImages)
        {
            throw new System.NotImplementedException();
        }
    }
}