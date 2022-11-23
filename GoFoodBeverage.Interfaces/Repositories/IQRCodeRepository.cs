using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IQRCodeRepostiory : IGenericRepository<QRCode>
    {
        IQueryable<QRCode> GetAllQRCodeInStore(Guid storeId);

        Task<QRCode> GetQRCodeByIdAsync(Guid qrCodeId, Guid? storeId);
    }
}
