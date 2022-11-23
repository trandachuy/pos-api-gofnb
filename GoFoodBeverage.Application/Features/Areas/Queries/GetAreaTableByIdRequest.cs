using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Area;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Areas.Queries
{
    public class GetAreaTableByIdRequest : IRequest<GetAreaTableByIdResponse>
    {
        public Guid Id { get; set; }

        public Guid StoreBranchId { get; set; }
    }

    public class GetAreaTableByIdResponse
    {
        public AreaTableByIdModel AreaTable { get; set; }
    }

    public class GetAreaTableByIdRequestHandler : IRequestHandler<GetAreaTableByIdRequest, GetAreaTableByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;


        public GetAreaTableByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
        }

        public async Task<GetAreaTableByIdResponse> Handle(GetAreaTableByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var areaTableData = await _unitOfWork.AreaTables
                .GetAreaTableByIdAsync(request.Id, loggedUser.StoreId);

            ThrowError.Against(areaTableData == null, "Cannot find area table information");

            var areaTable = _mapper.Map<AreaTableByIdModel>(areaTableData);
            areaTable.Tables = new List<AreaTableByIdModel.AreaTableDto>
            {
                 new AreaTableByIdModel.AreaTableDto
                 {
                     Id = areaTableData.Id,
                     Name = areaTableData.Name,
                     NumberOfSeat = areaTableData.NumberOfSeat
                 }
            };

            var response = new GetAreaTableByIdResponse()
            {
                AreaTable = areaTable
            };

            return response;
        }
    }
}
