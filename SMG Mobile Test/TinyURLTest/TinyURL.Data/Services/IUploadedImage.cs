using System.Collections;
using System.Collections.Generic;
using TinyURL.Data.Models;

namespace TinyURL.Data.Services
{
    public interface IUploadedImage
    {
        IEnumerable<UploadedImage> GetAll();
        UploadedImage Get(int id);
        UploadedImage Get(string url);
        void AddUploadedImage(UploadedImage uploadedImages);
        void UpdateUploadedImage(UploadedImage uploadedImages);
        void DeleteUploadedImage(UploadedImage uploadedImages);
    }
}