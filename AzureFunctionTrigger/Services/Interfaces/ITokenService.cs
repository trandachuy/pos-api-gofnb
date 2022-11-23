using System.Threading.Tasks;

namespace AzureFunctionTrigger.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetInternalToolAccessTokenAsync();
    }
}
