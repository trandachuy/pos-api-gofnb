using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;
using System.Linq;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class FileUploadRepository : GenericRepository<FileUpload>, IFileUploadRepository
    {
        public FileUploadRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<FileUpload> GetSliderImagesAsync()
        {
            return dbSet.Where(x => x.UsingById == EnumFileUsingBy.Slider);
        }
    }
}
