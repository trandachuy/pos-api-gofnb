using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Staff;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Staffs.Queries
{
    public class GetStaffActivitiesRequest : IRequest<GetStaffActivityResponse>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }

    public class GetStaffActivityResponse
    {
        public int TotalItem { get; set; }

        public List<StaffActivityResponse> StaffActivities { get; set; }
    }


    public class StaffActivityResponse : StaffActivityModel { }

    public class GetStaffActivityRequestHandler : IRequestHandler<GetStaffActivitiesRequest, GetStaffActivityResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetStaffActivityRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
        )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetStaffActivityResponse> Handle(GetStaffActivitiesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            int skip = (request.PageIndex - 1) * request.PageSize;
            IQueryable<StaffActivity> queryable = _unitOfWork.StaffActivities
                .GetAll()
                .Where(a => a.StoreId == loggedUser.StoreId)
                .AsNoTracking()
                .AsQueryable();

            int totalItem = await queryable.CountAsync();
            List<StaffActivityResponse> staffActivities = await queryable
                .Select(a => new StaffActivityResponse()
                {
                    StoreId = a.StoreId,
                    StaffId = a.StaffId,
                    ActionGroup = a.ActionGroup,
                    ActionType = a.ActionType,
                    ExecutedTime = a.ExecutedTime,
                    ObjectId = a.ObjectId,
                    ObjectName = a.ObjectName,
                    ObjectThumbnail = a.ObjectThumbnail,
                    StaffName = a.Staff.FullName,
                    ActionGroupDescribe = a.ActionGroup.GetDescription(),
                    ActionTypeDescribe = a.ActionType.GetDescription(),
                })
                .OrderByDescending(a => a.ExecutedTime)
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync();

            GetStaffActivityResponse response = new GetStaffActivityResponse()
            {
                StaffActivities = staffActivities,
                TotalItem = totalItem
            };

            return response;
        }
    }
}
