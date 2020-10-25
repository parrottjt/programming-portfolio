using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using TinyURL.Data.Models;

namespace TinyURL.Web.Models
{
    public class ImageViewModel
    {
        //Labeling the content to be a upload, to ensure uploading
        [DataType(DataType.Upload)]
        [Display(Name = "Image File")]
        [Required(ErrorMessage = "Please Choose A File To Upload.")]
        public string file { get; set; }

        public UploadedImage Image { get; set; }
    }
}