using AutoMapper;
using ClosedXML.Excel;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Product;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using GoFoodBeverage.Common.Helpers;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Products.Queries
{
    public class DownloadImportProductTemplateRequest : IRequest<DownloadImportProductTemplateResponse>
    {
        public string LanguageCode { get; set; }
    }

    public class DownloadImportProductTemplateResponse
    {
        public string FileName { get; set; }

        public byte[] Bytes { get; set; }
    }

    public class DownloadImportProductTemplateRequestHandler : IRequestHandler<DownloadImportProductTemplateRequest, DownloadImportProductTemplateResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public DownloadImportProductTemplateRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<DownloadImportProductTemplateResponse> Handle(DownloadImportProductTemplateRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            #region Get prepare data to export the file template
            // get all product categories
            var allProductCategories = await _unitOfWork.ProductCategories
                .GetAllProductCategoriesInStore(loggedUser.StoreId.Value)
                .OrderBy(productCategory => productCategory.CreatedTime)
                .AsNoTracking()
                .Select(productCategory => new ImportSubModelDto()
                {
                    Code = productCategory.Code,
                    Name = productCategory.Name
                })
                .ToListAsync();

            // get all options
            var allOptions = await _unitOfWork.Options
               .GetAllOptionsInStore(loggedUser.StoreId.Value)
               .OrderBy(o => o.CreatedTime)
               .AsNoTracking()
               .Select(o => new ImportSubModelDto()
               {
                   Code = o.Code,
                   Name = o.Name
               })
               .ToListAsync();

            // get all toppings
            var allToppings = await _unitOfWork.Products
              .GetAllToppingActivatedInStore(loggedUser.StoreId.Value)
              .OrderBy(o => o.CreatedTime)
              .AsNoTracking()
              .Select(o => new ImportSubModelDto()
              {
                  Code = o.Code.ToString(),
                  Name = o.Name
              })
              .ToListAsync();

            // get all materials
            var allMaterials = await _unitOfWork.Materials
              .GetAllMaterialsActivatedInStore(loggedUser.StoreId.Value)
              .OrderBy(o => o.CreatedTime)
              .Include(m => m.Unit)
              .AsNoTracking()
              .Select(m => new ProductMaterialDto()
              {
                  Code = m.Code.ToString(),
                  Name = m.Name,
                  UnitName = m.Unit.Name
              })
              .ToListAsync();

            var allUnits = await _unitOfWork.Units
                .GetAllUnitsInStore(loggedUser.StoreId.Value)
                .OrderBy(o => o.CreatedTime)
                .AsNoTracking()
                .Select(m => new ProductUnitDto()
                {
                    Code = m.Code.ToString(),
                    Name = m.Name,
                })
                .ToListAsync();
            #endregion

            // get the empty file template
            string fileName = ImportProductFileConstants.IMPORT_PRODUCT_FILE_NAME;
            var outputFileDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\DocumentTemplates";
            string filePath = (request.LanguageCode?.ToLower()) switch
            {
                LanguageCodeConstants.VI => Path.Combine(outputFileDirectory, $"{fileName}_{LanguageCodeConstants.VI}.xlsx"),
                _ => Path.Combine(outputFileDirectory, $"{fileName}_{LanguageCodeConstants.EN}.xlsx"),
            };

            // open the excel file
            using IXLWorkbook workbook = new XLWorkbook(filePath);

            // set the start row position to insert on the sheet
            var startInsertRowPosition = ImportProductFileConstants.SUB_SHEET_HEADING_POSITION + 1;

            #region Insert product categories to category sheet
            // prepare data before insert
            allProductCategories.ForEach((p) =>
            {
                var index = allProductCategories.IndexOf(p);
                p.No = index + 1;
            });
            workbook.InsertDataToExcelFile(allProductCategories, startInsertRowPosition, ImportProductFileConstants.CATEGORY_SHEET_INDEX);
            workbook.ProtectSheet(ImportProductFileConstants.CATEGORY_SHEET_INDEX);
            #endregion

            #region Insert unit to unit sheet
            allUnits.ForEach((i) =>
            {
                var index = allUnits.IndexOf(i);
                i.No = index + 1;
            });
            workbook.InsertDataToExcelFile(allUnits, startInsertRowPosition, ImportProductFileConstants.UNIT_SHEET_INDEX);
            workbook.ProtectSheet(ImportProductFileConstants.UNIT_SHEET_INDEX);
            #endregion

            #region Insert option to option sheet
            allOptions.ForEach((i) =>
            {
                var index = allOptions.IndexOf(i);
                i.No = index + 1;
            });
            workbook.InsertDataToExcelFile(allOptions, startInsertRowPosition, ImportProductFileConstants.OPTION_SHEET_INDEX);
            workbook.ProtectSheet(ImportProductFileConstants.OPTION_SHEET_INDEX);
            #endregion

            #region Insert topping to topping sheet
            allToppings.ForEach((i) =>
            {
                var index = allToppings.IndexOf(i);
                i.No = index + 1;
            });
            workbook.InsertDataToExcelFile(allToppings, startInsertRowPosition, ImportProductFileConstants.TOPPING_SHEET_INDEX);
            workbook.ProtectSheet(ImportProductFileConstants.TOPPING_SHEET_INDEX);
            #endregion

            #region Insert material to material sheet
            allMaterials.ForEach((i) =>
            {
                var index = allMaterials.IndexOf(i);
                i.No = index + 1;
            });
            workbook.InsertDataToExcelFile(allMaterials, startInsertRowPosition, ImportProductFileConstants.MATERIAL_SHEET_INDEX);
            workbook.ProtectSheet(ImportProductFileConstants.MATERIAL_SHEET_INDEX);
            #endregion

            byte[] bytes = workbook.SaveAsBytes();
           
            var response = new DownloadImportProductTemplateResponse()
            {
                FileName = fileName,
                Bytes = bytes
            };

            return response;
        }
    }
}
