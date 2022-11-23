using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Bill;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Package.Queries
{
    public class GetPackagePricingsRequest : IRequest<GetPackagePricingsResponse>
    {
    }

    public class GetPackagePricingsResponse
    {
        public IEnumerable<int> PeriodPackageDurations { get; set; }

        public IEnumerable<PackageModel> Packages { get; set; }

        public IEnumerable<FunctionGroupModel> AllFunctionGroups { get; set; }
    }

    public class GetBillPackageHandler : IRequestHandler<GetPackagePricingsRequest, GetPackagePricingsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetBillPackageHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetPackagePricingsResponse> Handle(GetPackagePricingsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var periodPackageDurations = await _unitOfWork.PackageDurationByMonths.GetAll().OrderBy(x => x.Id).Select(x => x.Period).ToListAsync(cancellationToken: cancellationToken);

            var allFunctionGroups = await _unitOfWork.FunctionGroups
                .GetAll()
                .Include(f => f.Functions)
                .AsNoTracking()
                .OrderBy(x => x.Order)
                .ProjectTo<FunctionGroupModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var packages = await GetPackageDetailsAsync();
            var response = new GetPackagePricingsResponse()
            {
                PeriodPackageDurations = periodPackageDurations,
                Packages = packages,
                AllFunctionGroups = allFunctionGroups
            };

            return response;
        }

        private async Task<List<PackageModel>> GetPackageDetailsAsync()
        {
            var packages = await _unitOfWork.Packages
               .GetAll()
               .Include(p => p.PackageFunctions)
               .ThenInclude(fg => fg.Function)
               .OrderBy(p => p.Id)
               .ToListAsync();

            var functions = packages.SelectMany(p => p.PackageFunctions.Select(pfg => pfg.Function));
            var functionModels = _mapper.Map<IEnumerable<FunctionModel>>(functions);
            var packageModels = new List<PackageModel>();
            foreach (var package in packages)
            {
                var functionIds = package.PackageFunctions.Where(pfg => pfg.PackageId == package.Id).Select(pfg => pfg.FunctionId);
                var packageFunctionModels = functionModels.Where(fgm => functionIds.Any(fgid => fgid == fgm.Id)).ToList();
                var packageModel = new PackageModel()
                {
                    Id = package.Id,
                    Name = package.Name,
                    CostPerMonth = package.CostPerMonth,
                    Tax = package.Tax,
                    IsActive = package.IsActive,
                    Functions = packageFunctionModels
                };

                packageModels.Add(packageModel);
            }

            return packageModels;
        }
    }
}
