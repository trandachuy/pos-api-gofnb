using GoFoodBeverage.Storage.Azure.Models;
using System.IO;
using System.Threading.Tasks;

namespace GoFoodBeverage.Storage.Azure
{
    public interface IAzureStorageService
    {
        Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName);

        Task<string> UploadFileToStorageAsync(FileUploadRequestModel request);
    }
}
