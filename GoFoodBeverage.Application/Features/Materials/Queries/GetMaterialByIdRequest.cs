using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Unit;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Common.Exceptions;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialByIdRequest : IRequest<GetMaterialByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetMaterialByIdResponse
    {
        public MaterialByIdModel Material { get; set; }

        public IList<UnitConversionUnitDto> UnitConversions { get; set; }
    }

    public class GetMaterialByIdHandler : IRequestHandler<GetMaterialByIdRequest, GetMaterialByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetMaterialByIdHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetMaterialByIdResponse> Handle(GetMaterialByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            ThrowError.Against(!loggedUser.StoreId.HasValue, "Cannot find store information");

            var material = await _unitOfWork.Materials
                .GetMaterialByIdInStoreAsync(request.Id, loggedUser.StoreId.Value);

            var unitConversion = _unitOfWork.UnitConversions
                .GetUnitConversionsByMaterialIdInStore(request.Id, loggedUser.StoreId.Value)
                .AsNoTracking()
                .Include(u => u.Unit)
                .ToListAsync();
            
            var materialDetail = _mapper.Map<MaterialByIdModel>(material);

            var unitConversionDetail = _mapper.Map<IList<UnitConversionUnitDto>>(unitConversion.Result);

            return new GetMaterialByIdResponse
            {
                Material = materialDetail,
                UnitConversions = unitConversionDetail
            };
        }
    }
}
