using System.Collections.Generic;
using System.Linq;
using TinyURL.Data.Models;

namespace TinyURL.Data.Services
{
    public class InMemoryUploadedImages : IUploadedImage
    {
        List<UploadedImage> db = new List<UploadedImage>();

        public IEnumerable<UploadedImage> GetAll()
        {
            return db.OrderBy(image => image.Id);
        }

        public UploadedImage Get(int id)
        {
            return db.FirstOrDefault(image => image.Id == id);
        }

        //This is the start for the web scraper
        public UploadedImage Get(string url)
        {
            return db.FirstOrDefault(image => image.TinyURL == url);
        }

        public void AddUploadedImage(UploadedImage uploadedImages)
        {
            uploadedImages.Id = db.Count;
            db.Add(uploadedImages);
        }

        public void UpdateUploadedImage(UploadedImage uploadedImage)
        {
            db[db.FindIndex(image => image == uploadedImage)] = uploadedImage;
        }

        public void DeleteUploadedImage(UploadedImage uploadedImages)
        {
            db.Remove(uploadedImages);
        }
    }
}