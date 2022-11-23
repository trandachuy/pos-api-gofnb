using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Domain.Settings;
using GoFoodBeverage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.QRCodes.Queries
{
    public class GetCreateQRCodePrepareDataRequest : IRequest<GetCreateQRCodePrepareDataResponse>
    {
    }

    public class GetCreateQRCodePrepareDataResponse
    {
        public Guid QrCodeId { get; set; } = Guid.NewGuid();

        public string QrCodeUrl { get; set; }

        public IEnumerable<BranchDto> Branches { get; set; }

        public IEnumerable<ServiceTypeDto> ServiceTypes { get; set; }

        public IEnumerable<AreaDto> Areas { get; set; }

        public IEnumerable<TargetDto> Targets { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        public class BranchDto
        {
            public Guid BranchId { get; set; }

            public string BranchName { get; set; }
        }

        public class ServiceTypeDto
        {
            public EnumQRCodeServingType ServiceTypeId { get; set; }

            public string ServiceTypeName { get; set; }
        }

        public class AreaTableDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class AreaDto : AreaTableDto
        {
            public Guid BranchId { get; set; }

            public IEnumerable<AreaTableDto> Tables { get; set; }
        }

        public class TargetDto
        {
            public EnumTargetQRCode TargetType { get; set; }

            public string TargetName { get; set; }
        }

        public class ProductDto
        {
            public int No { get; set; }

            public string Thumbnail { get; set; }

            public Guid ProductId { get; set; }

            public string ProductName { get; set; }

            public string UnitName { get; set; }
        }
    }

    public class GetCreateQRCodePrepareDataRequestHandler : IRequestHandler<GetCreateQRCodePrepareDataRequest, GetCreateQRCodePrepareDataResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DomainFE _domainFE;

        public GetCreateQRCodePrepareDataRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IOptions<DomainFE> domainFE
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _domainFE = domainFE.Value;
        }

        public async Task<GetCreateQRCodePrepareDataResponse> Handle(GetCreateQRCodePrepareDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var response = new GetCreateQRCodePrepareDataResponse();

            // get all branches
            response.Branches = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(loggedUser.StoreId)
                .AsNoTracking()
                .Select(branch => new GetCreateQRCodePrepareDataResponse.BranchDto()
                {
                    BranchId = branch.Id,
                    BranchName = branch.Name
                })
                .ToListAsync();

            // get service types
            var serviceTypes = new List<GetCreateQRCodePrepareDataResponse.ServiceTypeDto>();
            serviceTypes.Add(new GetCreateQRCodePrepareDataResponse.ServiceTypeDto()
            {
                ServiceTypeId = EnumQRCodeServingType.Instore,
                ServiceTypeName = EnumQRCodeServingType.Instore.GetName()
            });
            serviceTypes.Add(new GetCreateQRCodePrepareDataResponse.ServiceTypeDto()
            {
                ServiceTypeId = EnumQRCodeServingType.Online,
                ServiceTypeName = EnumQRCodeServingType.Online.GetName()
            });

            response.ServiceTypes = serviceTypes;

            response.Areas = await _unitOfWork.Areas
                .GetAllAreasActiveByStoreId(loggedUser.StoreId)
                .Include(area => area.AreaTables)
                .OrderBy(area => area.CreatedTime)
                .AsNoTracking()
                .Select(area => new GetCreateQRCodePrepareDataResponse.AreaDto()
                {
                    Id = area.Id,
                    BranchId = area.StoreBranchId,
                    Name = area.Name,
                    Tables = area.AreaTables
                    .Where(table => table.IsActive == true)
                    .OrderBy(table => table.CreatedTime).Select(table => new GetCreateQRCodePrepareDataResponse.AreaTableDto()
                    {
                        Id = table.Id,
                        Name = table.Name
                    })
                })
                .ToListAsync(cancellationToken: cancellationToken);

            // get qrcode targets
            var targets = new List<GetCreateQRCodePrepareDataResponse.TargetDto>();
            targets.Add(new GetCreateQRCodePrepareDataResponse.TargetDto()
            {
                TargetType = EnumTargetQRCode.ShopMenu,
                TargetName = EnumTargetQRCode.ShopMenu.GetName()
            });
            targets.Add(new GetCreateQRCodePrepareDataResponse.TargetDto()
            {
                TargetType = EnumTargetQRCode.AddProductToCart,
                TargetName = EnumTargetQRCode.AddProductToCart.GetName(),
            });

            response.Targets = targets;

            // get products
            var products = await _unitOfWork.Products
                .GetAllProductInStoreActive(loggedUser.StoreId.Value)
                .AsNoTracking()
                .Select(product => new GetCreateQRCodePrepareDataResponse.ProductDto()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Thumbnail = product.Thumbnail,
                    UnitName = product.Unit.Name
                })
                .ToListAsync();
            products.ForEach(p => {
                var index = products.IndexOf(p);
                p.No = index + 1;
            });
            response.Products = products;

            response.QrCodeUrl = GenerateQRCodeUrl(response.QrCodeId);

            return response;
        }
    
    
        private string GenerateQRCodeUrl(Guid qrCodeId)
        {
            var qrCodeUrl = $"{_domainFE.EndUser}/qrcode?id={qrCodeId}";

            return qrCodeUrl;
        }
    }
}
