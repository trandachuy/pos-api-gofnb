using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Features.Stores.Queries
{
    public class GetAllBranchInStoreByStoreIdRequest : IRequest<List<GetAllBranchInStoreByStoreIdResponse>>
    {
        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }
    }

    public class GetAllBranchInStoreByStoreIdResponse
    {
        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public int Rate { get; set; }

        public bool IsMainStore { get; set; }

        public Guid StoreId { get; set; }

        public Guid StoreBranchId { get; set; }

        public string StoreName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }

    public class GetAllBranchInStoreByStoreIdRequestHandler : IRequestHandler<GetAllBranchInStoreByStoreIdRequest, List<GetAllBranchInStoreByStoreIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBranchInStoreByStoreIdRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetAllBranchInStoreByStoreIdResponse>> Handle(GetAllBranchInStoreByStoreIdRequest request, CancellationToken cancellationToken)
        {
            Store storeEntity = await _unitOfWork.Stores.GetStoreAllBranchByStoreIdOrStoreBranchIdAsync(request.StoreId, request.BranchId);

            List<GetAllBranchInStoreByStoreIdResponse> response = new List<GetAllBranchInStoreByStoreIdResponse>();

            if (storeEntity.StoreBranches != null && storeEntity.StoreBranches.Count > 0)
            {
                response = storeEntity.StoreBranches.Where(b => !b.IsDeleted).Select(a => new GetAllBranchInStoreByStoreIdResponse()
                {
                    Address = $"{a.Address?.Address1}, {a.Address?.Ward?.Prefix} {a.Address?.Ward?.Name}, " +
                        $"{a.Address?.District?.Prefix} {a.Address?.District?.Name}, {a.Address?.City?.Name}, {a.Address?.Country?.Name}.",
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    Website = string.Empty,
                    IsMainStore = false,
                    StoreId = a.StoreId,
                    StoreBranchId = a.Id,
                    StoreName = a.Name,
                    Latitude = a.Address.Latitude,
                    Longitude = a.Address.Longitude
                }).OrderBy(x=>x.StoreName).ToList();
            }

            return response;
        }
    }
}
