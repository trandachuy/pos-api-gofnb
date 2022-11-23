using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class GetComboByIdRequest : IRequest<GetComborByIdResponse>
    {
        public Guid? Id { get; set; }
    }

    public class GetComborByIdResponse
    {
        public bool IsSuccess { get; set; }

        public ComboDataTableModel Combo { get; set; }
    }

    public class GetComboByIdRequestHandler : IRequestHandler<GetComboByIdRequest, GetComborByIdResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTime;

        public GetComboByIdRequestHandler(IUserProvider userProvider, IUnitOfWork unitOfWork, IMapper mapper, IDateTimeService dateTime)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTime = dateTime;
        }

        public async Task<GetComborByIdResponse> Handle(GetComboByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var comboData = await _unitOfWork.Combos
                .GetComboByIdAsync(request.Id.Value, loggedUser.StoreId);

            var combo = _mapper.Map<ComboDataTableModel>(comboData);

            if (combo.IsStopped == true)
            {
                combo.StatusId = EnumComboStatus.Finished;
            }
            else
            {
                combo.StatusId = GetComboStatus(combo.StartDate, combo.EndDate);
            }

            return new GetComborByIdResponse
            {
                IsSuccess = true,
                Combo = combo
            };
        }

        /// <summary>
        /// Start date and due date must be UTC time
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        private EnumComboStatus GetComboStatus(DateTime startDate, DateTime? dueDate)
        {
            var result = _dateTime.NowUtc.Date.CompareTo(startDate.Date);

            if (result < 0)
            {
                return EnumComboStatus.Scheduled;
            }
            else if (result == 0)
            {
                return EnumComboStatus.Active;
            }
            else
            {
                if (dueDate.HasValue)
                {
                    return _dateTime.NowUtc.Date.CompareTo(dueDate.Value.Date) >= 0 ? EnumComboStatus.Finished : EnumComboStatus.Active;
                }
                else
                {
                    return EnumComboStatus.Active;
                }
            }
        }
    }
}
