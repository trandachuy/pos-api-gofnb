using AutoMapper;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Material;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Common.Extensions;
using AutoMapper.QueryableExtensions;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class GetMaterialCategoriesRequest : IRequest<GetMaterialCategoriesResponse>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string KeySearch { get; set; }
    }

    public class GetMaterialCategoriesResponse
    {
        public IEnumerable<MaterialCategoryDatatableModel> MaterialCategories { get; set; }

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }

    public class GetMaterialCategoriesRequestHandler : IRequestHandler<GetMaterialCategoriesRequest, GetMaterialCategoriesResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetMaterialCategoriesRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetMaterialCategoriesResponse> Handle(GetMaterialCategoriesRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var materialCategories = new PagingExtensions.Pager<MaterialCategoryDatatableModel>(new List<MaterialCategoryDatatableModel>(), 0);
            if (string.IsNullOrEmpty(request.KeySearch))
            {
                materialCategories = await _unitOfWork.MaterialCategories
                                   .GetAllMaterialCategoriesInStore(loggedUser.StoreId.Value)
                                   .AsNoTracking()
                                   .Include(i => i.Materials)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ProjectTo<MaterialCategoryDatatableModel>(_mapperConfiguration)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }
            else
            {
                string keySearch = request.KeySearch.Trim().ToLower();
                materialCategories = await _unitOfWork.MaterialCategories
                                   .GetAllMaterialCategoriesInStore(loggedUser.StoreId.Value)
                                   .Where(s => s.Name.ToLower().Contains(keySearch))
                                   .Include(i => i.Materials)
                                   .OrderByDescending(p => p.CreatedTime)
                                   .ProjectTo<MaterialCategoryDatatableModel>(_mapperConfiguration)
                                   .ToPaginationAsync(request.PageNumber, request.PageSize);
            }

            var materialCategoriesResponse = materialCategories.Result.ToList();
            materialCategoriesResponse.ForEach(m =>
            {
                m.No = materialCategoriesResponse.IndexOf(m) + ((request.PageNumber - 1) * request.PageSize) + 1;
            });

            var response = new GetMaterialCategoriesResponse()
            {
                PageNumber = request.PageNumber,
                Total = materialCategories.Total,
                MaterialCategories = materialCategoriesResponse
            };

            return response;
        }
    }
}
