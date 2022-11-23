using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Storage.Azure.Constants;

namespace GoFoodBeverage.Storage.Azure.Models
{
    public class FileUploadRequestModel
    {
        public FileUploadRequestModel()
        {
            FileSizeLimit = DefaultConstants.STORE_IMAGE_LIMIT;
        }

        public IFormFile File { get; set; }

        public string FileName { get; set; }

        public int FileSizeLimit { get; set; } 
    }
}
