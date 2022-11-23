using AutoMapper;
using GoFoodBeverage.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class UpdateCostPerUnitByMaterialIdRequest : IRequest<bool>
    {
        public Guid Id { get; set; }

        public decimal? CostPerUnit { get; set; }
    }

    public class UpdateCostPerUnitByMaterialIdHandler : IRequestHandler<UpdateCostPerUnitByMaterialIdRequest, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IUserActivityService _userActivityService;

        public UpdateCostPerUnitByMaterialIdHandler(
            IMapper mapper,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IUserActivityService userActivityService
        )
        {
            _mapper = mapper;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _userActivityService = userActivityService;
        }

        public async Task<bool> Handle(UpdateCostPerUnitByMaterialIdRequest request, CancellationToken cancellationToken)
        {
            var material = _unitOfWork.Materials.Find(x => x.Id == request.Id).FirstOrDefault();
            material.CostPerUnit = request.CostPerUnit ?? 0;
            await _unitOfWork.Materials.UpdateAsync(material);
            return true;
        }
    }
}
