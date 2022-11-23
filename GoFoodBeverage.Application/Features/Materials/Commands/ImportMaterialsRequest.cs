using System;
using MediatR;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using DocumentFormat.OpenXml.Packaging;
using GoFoodBeverage.Common.Exceptions;
using DocumentFormat.OpenXml.Spreadsheet;
using GoFoodBeverage.Common.Constants;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class ImportMaterialsRequest : IRequest<ImportMaterialResponse>
    {
        public IFormFile File { get; set; }
    }

    public class ImportMaterialResponse
    {
        public bool Success { get; set; }

        public InfoImportModel InfoImport { get; set; }

        public class InfoImportModel
        {
            public int NumberRecordImport { get; set; }

            public int NumberRecordImportSuccess { get; set; }

            public bool IsImportSuccess { get; set; }

            public IEnumerable<ErrorModel> Errors { get; set; }

            public bool IsInValidUnit { get; set; }

            public bool IsInValidCategory { get; set; }

            public class ErrorModel
            {
                public int Row { get; set; }

                public IEnumerable<DetailErrorModel> DetailErrors { get; set; }

                public class DetailErrorModel
                {
                    public string Cell { get; set; }

                    public int Column { get; set; }

                    public string Sheet { get; set; }

                    public string Name { get; set; }

                    public int Row { get; set; }
                }
            }

            public IEnumerable<MaterialModel> ListMaterialSuccess { get; set; }

            public class MaterialModel
            {
                public int No { get; set; }

                public string Name { get; set; }

                public string Description { get; set; }

                public string Category { get; set; }

                public string Unit { get; set; }

                public string SKU { get; set; }

                public string MinQuanity { get; set; }

                public string Branch { get; set; }
            }
        }
    }

    public class ImportMaterialsRequestHandler : IRequestHandler<ImportMaterialsRequest, ImportMaterialResponse>
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public ImportMaterialsRequestHandler(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
        )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<ImportMaterialResponse> Handle(ImportMaterialsRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var store = await _unitOfWork.Stores.GetStoreByIdAsync(loggedUser.StoreId.Value);
            ThrowError.Against(store == null, "Cannot find store information");

            string extension = Path.GetExtension(request.File?.FileName);
            if (!FileFormatConstants.ExcelFileExtensions.Contains(extension))
            {
                var stringError = $"The specified file {request.File?.FileName} could not be uploaded. Only file with the following extension is allowed: {FileFormatConstants.ExcelFileExtensions}";
                throw new InvalidOperationException(stringError);
            }

            bool isCheckFormatFile = false;
            var infoImport = new ImportMaterialResponse.InfoImportModel();

            using (var stream = request.File?.OpenReadStream())
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
                {
                    using (var transaction = await _unitOfWork.BeginTransactionAsync())
                        try
                        {
                            WorkbookPart wbPart = doc.WorkbookPart;
                            //Add Unit, Material Category
                            var listSheets = wbPart.Workbook.Sheets;
                            var sheetMaterialCategory = new { isFormatMaterialCategory = false, nameSheetMaterialCategory = "" };
                            var sheetUnit = new { isFormatUnit = false, nameSheetUnit = "" };
                            var errorUnits = new List<ImportMaterialResponse.InfoImportModel.ErrorModel>();
                            var errorDetailUnits = new List<ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel>();
                            var errorCategories = new List<ImportMaterialResponse.InfoImportModel.ErrorModel>();
                            var errorDetailCategories = new List<ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel>();

                            foreach (Sheet item in listSheets)
                            {
                                if (item.Name == "CATEGORY")
                                {
                                    sheetMaterialCategory = new { isFormatMaterialCategory = true, nameSheetMaterialCategory = "CATEGORY" };
                                }
                                if (item.Name == "DANH MỤC")
                                {
                                    sheetMaterialCategory = new { isFormatMaterialCategory = true, nameSheetMaterialCategory = "DANH MỤC" };
                                }
                                if (item.Name == "UNIT")
                                {
                                    sheetUnit = new { isFormatUnit = true, nameSheetUnit = "UNIT" };
                                }
                                if (item.Name == "ĐƠN VỊ")
                                {
                                    sheetUnit = new { isFormatUnit = true, nameSheetUnit = "ĐƠN VỊ" };
                                }
                            }

                            if (sheetMaterialCategory.isFormatMaterialCategory)
                            {
                                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetMaterialCategory.nameSheetMaterialCategory).FirstOrDefault();
                                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                                Worksheet sheet = wsPart.Worksheet;

                                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                SharedStringTable sst = stringTable.SharedStringTable;

                                var rows = sheet.GetFirstChild<SheetData>().Descendants<Row>();
                                var rowIndex = rows.Count() - 2;
                                var listCategoryAdd = new List<MaterialCategory>();

                                for (int i = rowIndex; i <= rows.Count(); i++)
                                {
                                    var cellCategoryCode = $"B{i}";
                                    var cellCategory = $"C{i}";
                                    var categoryName = GetCellValueByCellName(wbPart, wsPart, cellCategory);
                                    var categoryCode = GetCellValueByCellName(wbPart, wsPart, cellCategoryCode);

                                    if (string.IsNullOrEmpty(categoryCode) && !string.IsNullOrEmpty(categoryName))
                                    {
                                        var checkExistCategory = _unitOfWork.MaterialCategories.Find(x => x.StoreId == store.Id && x.Name.ToLower() == categoryName.ToLower()).FirstOrDefault();
                                        if (checkExistCategory == null)
                                        {
                                            var newMaterialCategory = new MaterialCategory()
                                            {
                                                StoreId = store.Id,
                                                Name = categoryName,
                                            };
                                            listCategoryAdd.Add(newMaterialCategory);
                                        }
                                        else
                                        {
                                            var errorDetail = AddErrorDetail(cellCategory, 16, sheetMaterialCategory.nameSheetMaterialCategory, categoryName);
                                            errorDetail.Row = i;
                                            errorDetailCategories.Add(errorDetail);
                                        }
                                    }
                                }

                                foreach (var errorDetailCategory in errorDetailCategories)
                                {
                                    var errorCategory = new ImportMaterialResponse.InfoImportModel.ErrorModel();
                                    errorCategory.Row = errorDetailCategory.Row;
                                    errorCategory.DetailErrors = new List<ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel> { errorDetailCategory };
                                    errorCategories.Add(errorCategory);
                                }

                                if (errorCategories.Count == 0)
                                {
                                    if (listCategoryAdd.Count > 0)
                                    {
                                        await _unitOfWork.MaterialCategories.AddRangeAsync(listCategoryAdd);
                                    }
                                }
                            }


                            if (sheetUnit.isFormatUnit)
                            {
                                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetUnit.nameSheetUnit).FirstOrDefault();
                                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                                Worksheet sheet = wsPart.Worksheet;

                                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                SharedStringTable sst = stringTable.SharedStringTable;

                                var rows = sheet.GetFirstChild<SheetData>().Descendants<Row>();
                                var rowIndex = rows.Count() - 2;
                                var totalRecords = await _unitOfWork.Units.GetAll().CountAsync() - 1;
                                var listUnitAdd = new List<Domain.Entities.Unit>();
                                for (int i = rowIndex; i <= rows.Count(); i++)
                                {                                 
                                    var cellCodeUnit = $"B{i}";
                                    var cellUnit = $"C{i}";
                                    var unitName = GetCellValueByCellName(wbPart, wsPart, cellUnit);
                                    var unitCode = GetCellValueByCellName(wbPart, wsPart, cellCodeUnit);

                                    if (string.IsNullOrEmpty(unitCode) && !string.IsNullOrEmpty(unitName))
                                    {
                                        var checkExistUnit = _unitOfWork.Units.Find(x => x.StoreId == store.Id && x.Name.ToLower() == unitName.ToLower()).FirstOrDefault();
                                        if (checkExistUnit == null)
                                        {
                                            totalRecords = totalRecords + 1;
                                            var newUnit = new Domain.Entities.Unit()
                                            {
                                                StoreId = store.Id,
                                                Name = unitName,
                                                CreatedUser = loggedUser.AccountId,
                                                Position = totalRecords
                                            };
                                            listUnitAdd.Add(newUnit);
                                        }
                                        else
                                        {
                                            var errorDetail = AddErrorDetail(cellUnit, 17, sheetUnit.nameSheetUnit, unitName);
                                            errorDetail.Row = i;
                                            errorDetailUnits.Add(errorDetail);
                                        }
                                    }
                                }

                                foreach (var errorDetailUnit in errorDetailUnits)
                                {
                                    var errorUnit = new ImportMaterialResponse.InfoImportModel.ErrorModel();
                                    errorUnit.Row = errorDetailUnit.Row;
                                    errorUnit.DetailErrors = new List<ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel> { errorDetailUnit };
                                    errorUnits.Add(errorUnit);
                                }

                                if (errorUnits.Count == 0)
                                {
                                    if (listUnitAdd.Count > 0)
                                    {
                                        await _unitOfWork.Units.AddRangeAsync(listUnitAdd);
                                    }
                                }
                            }

                            //Import material
                            object getNameSheetMaterial = GetNameSheetMaterial(wbPart);
                            System.Reflection.PropertyInfo formatFileImport = getNameSheetMaterial.GetType().GetProperty("isFormat");
                            isCheckFormatFile = (bool)formatFileImport.GetValue(getNameSheetMaterial);

                            if (isCheckFormatFile)
                            {
                                System.Reflection.PropertyInfo sheetName = getNameSheetMaterial.GetType().GetProperty("nameSheetMaterial");
                                var sheetNameMaterial = sheetName.GetValue(getNameSheetMaterial).ToString();

                                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetNameMaterial).FirstOrDefault();
                                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                                Worksheet sheet = wsPart.Worksheet;

                                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                SharedStringTable sst = stringTable.SharedStringTable;

                                var rows = sheet.GetFirstChild<SheetData>().Descendants<Row>();

                                infoImport = await HandleCreateMaterial(wbPart, loggedUser.StoreId.Value, loggedUser.AccountId.Value, sheetNameMaterial, errorUnits, errorCategories);
                                if (infoImport.IsImportSuccess)
                                {
                                    await transaction.CommitAsync();
                                }
                                else
                                {
                                    await transaction.RollbackAsync();
                                }
                            }
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            return new ImportMaterialResponse()
                            {
                                Success = false,
                                InfoImport = infoImport
                            }; ;
                        }
                }
            }
            var response = new ImportMaterialResponse()
            {
                Success = isCheckFormatFile,
                InfoImport = infoImport
            };

            return response;
        }

        private static object GetNameSheetMaterial(WorkbookPart workbookPart)
        {
            object infoFileImport = new { };

            var listSheets = workbookPart.Workbook.Sheets;

            if(listSheets.Count() == 4)
            {
                foreach (Sheet item in listSheets)
                {
                    if(item.Name == "MATERIAL")
                    {
                        infoFileImport = new
                        {
                            isFormat = true,
                            nameSheetMaterial = item.Name
                        };
                    }
                    if (item.Name == "NGUYÊN VẬT LIỆU")
                    {
                        infoFileImport = new
                        {
                            isFormat = true,
                            nameSheetMaterial = item.Name
                        };
                    }
                    
                }
            } else
            {
                infoFileImport = new
                {
                    isFormat = false
                };
            }

            return infoFileImport;
        }

        private static string GetCellValueByCellName(WorkbookPart wbPart, WorksheetPart wsPart, string cellName)
        {
            string cellValue = null;
            var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            Cell theCell = wsPart.Worksheet.Descendants<Cell>().Where(c => c.CellReference == cellName).FirstOrDefault();

            // If the cell does not exist, return an empty string.
            if (theCell != null && theCell.InnerText.Length > 0)
            {
                cellValue = theCell.InnerText;

                if (theCell.DataType != null)
                {
                    cellValue = stringTable.SharedStringTable.ElementAt(int.Parse(cellValue)).InnerText;
                }
            }
            else
            {
                cellValue = "";
            }
            return cellValue;
        }

        private static ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel AddErrorDetail(string cellCode, int column, string sheet, string name ="")
        {
            var errorDetail = new ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel() {
                Cell = cellCode,
                Column = column,
                Sheet = sheet, 
                Name = name
            };
            return errorDetail;
        }

        private async Task<ImportMaterialResponse.InfoImportModel> HandleCreateMaterial(WorkbookPart wbPart, Guid storeId, Guid accountId, string sheetName, List<ImportMaterialResponse.InfoImportModel.ErrorModel> errorUnits, List<ImportMaterialResponse.InfoImportModel.ErrorModel> errorCategories)
        {        
            Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
            WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
            Worksheet sheet = wsPart.Worksheet;

            var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            SharedStringTable sst = stringTable.SharedStringTable;

            var rows = sheet.GetFirstChild<SheetData>().Descendants<Row>();
            var rowIndex = 1;
            var newMaterialList = new List<Material>();
            var newMaterialInventoryBranches = new List<MaterialInventoryBranch>();
            var updateMaterialInventoryBranches = new List<MaterialInventoryBranch>();
            var newUnitConversions = new List<UnitConversion>();
            var deleteUnitConversions = new List<UnitConversion>();
            var updateMaterialList = new List<Material>();
            var listMaterialSuccess = new List<ImportMaterialResponse.InfoImportModel.MaterialModel>();
            var errors = new List<ImportMaterialResponse.InfoImportModel.ErrorModel>();

            bool isDataImportFail = true;
            int numberRecordImport = 0;
            bool isAddNewUnitConversion = false;

            if (errorCategories.Count > 0)
            {
                errors.AddRange(errorCategories);
            }

            if (errorUnits.Count > 0)
            {
                errors.AddRange(errorUnits);
            }
         

            if (rows.Count() > 3)
            {
                numberRecordImport = rows.Count() - 3;

                Guid materialId = new Guid();

                var listBranches = await _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(storeId).ToListAsync();
                var listMaterials = await _unitOfWork.Materials.GetAll().Where(b => b.StoreId == storeId)
                                            .Include(mib => mib.MaterialInventoryBranches)
                                            .Include(u => u.Unit).ThenInclude(uc => uc.UnitConversions)
                                            .ToListAsync();
                var listMaterialCategory = await _unitOfWork.MaterialCategories.GetAll().Where(b => b.StoreId == storeId).ToListAsync();
                var baseUnitExisted = new Domain.Entities.Unit();

                foreach (Row row in rows)
                {
                    if (rowIndex == 1 || rowIndex == 2 || rowIndex == 3)
                    {
                        rowIndex += 1;
                        continue;
                    }

                    var newMaterial = new Material();
                    var materialExisted = new Material();
                    var newUnitConversion = new UnitConversion();

                    bool isUpdateMaterial = false;

                    var error = new ImportMaterialResponse.InfoImportModel.ErrorModel();
                    var errorDetails = new List<ImportMaterialResponse.InfoImportModel.ErrorModel.DetailErrorModel>();

                    var materialCategoryExisted = new MaterialCategory();

                    bool isErrorRow = false;
                    var listBranchNames = new List<string>();

                    //Get location column in sheet Material by rowIndex
                    var cellCode = $"A{rowIndex}";
                    var cellName = $"B{rowIndex}";
                    var cellDescription = $"C{rowIndex}";
                    var cellMaterialCategory = $"D{rowIndex}";
                    var cellUnit = $"E{rowIndex}";
                    var cellCapacity = $"F{rowIndex}";
                    var cellBaseUnit = $"G{rowIndex}";
                    var cellSku = $"H{rowIndex}";
                    var cellMinQuanity = $"I{rowIndex}";
                    var cellBranch = $"J{rowIndex}";


                    //Code
                    var cellValueCode = GetCellValueByCellName(wbPart, wsPart, cellCode);
                    //Name
                    var cellValueName = GetCellValueByCellName(wbPart, wsPart, cellName);
                    //Description
                    var cellValueDescription = GetCellValueByCellName(wbPart, wsPart, cellDescription);
                    //MaterialCategory
                    var cellValueMaterialCategory = GetCellValueByCellName(wbPart, wsPart, cellMaterialCategory);
                    //MinQuanity
                    var cellValueMinQuanity = GetCellValueByCellName(wbPart, wsPart, cellMinQuanity);
                    //SKU
                    var cellValueSku = GetCellValueByCellName(wbPart, wsPart, cellSku);
                    //Branch
                    var cellValueBranch = GetCellValueByCellName(wbPart, wsPart, cellBranch);
                    //UnitConversion
                    var cellValueUnit = GetCellValueByCellName(wbPart, wsPart, cellUnit);
                    var unitImportExisted = await _unitOfWork.Units.Find(g => g.StoreId == storeId && g.Name == cellValueUnit).FirstOrDefaultAsync();
                    //Capacity
                    var cellValueCapacity = GetCellValueByCellName(wbPart, wsPart, cellCapacity);
                    //BaseUnit
                    var cellValueBaseUnit = GetCellValueByCellName(wbPart, wsPart, cellBaseUnit);

                    if (!string.IsNullOrEmpty(cellValueBaseUnit))
                    {
                        baseUnitExisted = await _unitOfWork.Units.Find(g => g.StoreId == storeId && g.Name == cellValueBaseUnit).AsNoTracking().FirstOrDefaultAsync();
                    }

                    //Check row is material record or unit record.
                    if (string.IsNullOrEmpty(cellValueCode) && string.IsNullOrEmpty(cellValueName) && string.IsNullOrEmpty(cellValueDescription) && string.IsNullOrEmpty(cellValueSku) && string.IsNullOrEmpty(cellValueMinQuanity) && string.IsNullOrEmpty(cellValueBranch) )
                    {
                        isAddNewUnitConversion = true;
                    } else
                    {
                        //Code
                        isAddNewUnitConversion = false;
                        if (!string.IsNullOrEmpty(cellValueCode) && CheckIsNumber(cellValueCode))
                        {
                            //Check and search info material by materialName and storeId
                            materialExisted = listMaterials.Where(g => g.Code == int.Parse(cellValueCode)).FirstOrDefault();
                            if (materialExisted == null)
                            {
                                isErrorRow = true;
                                var errorDetail = AddErrorDetail(cellCode, 1, sheetName);
                                errorDetails.Add(errorDetail);
                            }
                            else
                            {
                                isUpdateMaterial = true;
                                materialId = materialExisted.Id;
                            }
                        }
                        else if (!string.IsNullOrEmpty(cellValueCode) && !CheckIsNumber(cellValueCode))
                        {
                            isErrorRow = true;
                            var errorDetail = AddErrorDetail(cellCode, 2, sheetName);
                            errorDetails.Add(errorDetail);
                        }

                        //Name
                        if (string.IsNullOrEmpty(cellValueName))
                        {
                            isErrorRow = true;
                            var errorDetail = AddErrorDetail(cellName, 4, sheetName);

                            errorDetails.Add(errorDetail);
                        }
                        else if (cellValueName.Length > 1000)
                        {
                            isErrorRow = true;
                            var errorDetail = AddErrorDetail(cellName, 5, sheetName);

                            errorDetails.Add(errorDetail);
                        }
                        else
                        {
                            if (isUpdateMaterial)
                            {
                                var materials = listMaterials.Where(m => m.Id != materialExisted.Id).ToList();
                                var findMaterialNameExisted = materials.Where(g => g.Name == cellValueName).FirstOrDefault();
                                if (findMaterialNameExisted != null)
                                {
                                    isErrorRow = true;
                                    var errorDetail = AddErrorDetail(cellName, 3, sheetName);

                                    errorDetails.Add(errorDetail);
                                }
                            }
                            else
                            {
                                var materialNameExisted = listMaterials.Where(g => g.Name == cellValueName).FirstOrDefault();
                                if (materialNameExisted != null)
                                {
                                    isErrorRow = true;
                                    var errorDetail = AddErrorDetail(cellName, 3, sheetName);

                                    errorDetails.Add(errorDetail);
                                }
                                else
                                {
                                    string checkNameInListNewMaterial = newMaterialList.Where(m => m.Name == cellValueName).FirstOrDefault()?.Name;
                                    if (checkNameInListNewMaterial != null)
                                    {
                                        isErrorRow = true;
                                        var errorDetail = AddErrorDetail(cellName, 0, sheetName);
                                        errorDetails.Add(errorDetail);
                                    }
                                }

                                materialId = newMaterial.Id;
                            }
                        }

                        //Description
                        if (cellValueDescription.Length > 1000)
                        {
                            isErrorRow = true;
                            var errorDetail = AddErrorDetail(cellDescription, 6, sheetName);
                            errorDetails.Add(errorDetail);
                        }

                        //MaterialCategory
                        materialCategoryExisted = listMaterialCategory.Where(g => g.Name == cellValueMaterialCategory).FirstOrDefault();

                        //SKU
                        if (!string.IsNullOrEmpty(cellValueSku))
                        {
                            if (cellValueSku.Length > 50)
                            {
                                isErrorRow = true;
                                var errorDetail = AddErrorDetail(cellSku, 12, sheetName);
                                errorDetails.Add(errorDetail);
                            }
                            else
                            {
                                if (!isUpdateMaterial)
                                {
                                    var isCheckSkuExisted = listMaterials.Where(m => m.Sku == cellValueSku).FirstOrDefault();
                                    if (isCheckSkuExisted != null)
                                    {
                                        isErrorRow = true;
                                        var errorDetail = AddErrorDetail(cellSku, 13, sheetName);
                                        errorDetails.Add(errorDetail);
                                    }
                                }
                            }
                        }

                        //MinQuanity
                        if (!string.IsNullOrEmpty(cellValueMinQuanity) && !CheckIsNumber(cellValueMinQuanity))
                        {
                            isErrorRow = true;
                            var errorDetail = AddErrorDetail(cellMinQuanity, 14, sheetName);
                            errorDetails.Add(errorDetail);
                        }


                        //Branch
                        if (string.IsNullOrEmpty(cellValueBranch))
                        {
                            //All Branch
                            if (isUpdateMaterial)
                            {
                                var branchIds = materialExisted.MaterialInventoryBranches.Select(x => x.BranchId);
                                var listBranchIdRemains = _unitOfWork.StoreBranches.GetRemainingStoreBranchesByStoreId(storeId, branchIds).Select(x => x.Id).ToList();
                                foreach (var branchId in listBranchIdRemains)
                                {
                                    var materialInventoryBranch = new MaterialInventoryBranch()
                                    {
                                        StoreId = storeId,
                                        BranchId = branchId,
                                        MaterialId = materialExisted.Id,
                                        Position = 0,
                                        Quantity = 0
                                    };
                                    newMaterialInventoryBranches.Add(materialInventoryBranch);
                                }
                            }
                            else
                            {
                                var listBranchIds = _unitOfWork.StoreBranches.GetStoreBranchesByStoreId(storeId).Select(x => x.Id).ToList();
                                foreach (var branchId in listBranchIds)
                                {
                                    var materialInventoryBranch = new MaterialInventoryBranch()
                                    {
                                        StoreId = storeId,
                                        BranchId = branchId,
                                        MaterialId = newMaterial.Id,
                                        Position = 0,
                                        Quantity = 0
                                    };
                                    newMaterialInventoryBranches.Add(materialInventoryBranch);
                                }
                            }
                            listBranchNames.Add(DefaultConstants.All_BRANCHES);
                        }
                        else
                        {
                            var branchCodeArr = cellValueBranch.Split(",");
                            newMaterial.MaterialInventoryBranches = new List<MaterialInventoryBranch>();
                            foreach (var branchCode in branchCodeArr)
                            {
                                var branchExisted = listBranches.Where(g => g.Code == int.Parse(branchCode.Trim())).FirstOrDefault();
                                listBranchNames.Add(branchExisted.Name);
                                if (branchExisted == null)
                                {
                                    isErrorRow = true;
                                    var errorDetail = AddErrorDetail(cellBranch, 15, sheetName);
                                    errorDetails.Add(errorDetail);
                                }
                                else
                                {
                                    if (isUpdateMaterial)
                                    {

                                        var deleteListMaterialInventoryBranches = materialExisted.MaterialInventoryBranches.Where(x => x.BranchId != branchExisted.Id).ToList();

                                        var currentMaterialInventoryBranches = materialExisted.MaterialInventoryBranches.Where(x => x.BranchId == branchExisted.Id).FirstOrDefault();

                                        if (currentMaterialInventoryBranches == null)
                                        {
                                            var materialInventoryBranch = new MaterialInventoryBranch()
                                            {
                                                StoreId = storeId,
                                                BranchId = branchExisted.Id,
                                                MaterialId = materialExisted.Id,
                                                Position = 0,
                                                Quantity = 0
                                            };
                                            newMaterialInventoryBranches.Add(materialInventoryBranch);
                                        }
                                        else
                                        {
                                            currentMaterialInventoryBranches.StoreId = storeId;
                                            currentMaterialInventoryBranches.BranchId = branchExisted.Id;
                                            currentMaterialInventoryBranches.MaterialId = materialExisted.Id;
                                            currentMaterialInventoryBranches.Position = 0;
                                            currentMaterialInventoryBranches.Quantity = 0;

                                            updateMaterialInventoryBranches.Add(currentMaterialInventoryBranches);
                                        }

                                        _unitOfWork.MaterialInventoryBranches.RemoveRange(deleteListMaterialInventoryBranches);
                                    }
                                    else
                                    {       
                                        var newMaterialInventoryBranch = new MaterialInventoryBranch()
                                        {
                                            StoreId = storeId,
                                            BranchId = branchExisted.Id,
                                            MaterialId = newMaterial.Id,
                                            Position = 0,
                                            Quantity = 0
                                        };
                                        newMaterial.MaterialInventoryBranches.Add(newMaterialInventoryBranch);
                                    }
                                }
                            }
                        }
                    }

                    //Capacity
                    int capacity = 0;
                    if (CheckIsNumber(cellValueCapacity) || !string.IsNullOrEmpty(cellValueCapacity))
                    {
                        capacity = int.Parse(cellValueCapacity);
                    }
                    else if(unitImportExisted != null && string.IsNullOrEmpty(cellValueCapacity))
                    {
                        isErrorRow = true;
                        var errorDetail = AddErrorDetail(cellCapacity, 9, sheetName);
                        errorDetails.Add(errorDetail);
                    }

                    //BaseUnit
                    if (baseUnitExisted == null)
                    {
                        isErrorRow = true;
                        var errorDetail = AddErrorDetail(cellBaseUnit, 10, sheetName);
                        errorDetails.Add(errorDetail);
                    }
                    else if(unitImportExisted != null)
                    {
                        if (unitImportExisted.Id != baseUnitExisted.Id)
                        {
                            newUnitConversion.UnitId = unitImportExisted.Id;
                            newUnitConversion.Capacity = capacity;
                            newUnitConversion.StoreId = storeId;

                            if (isUpdateMaterial)
                            {
                                deleteUnitConversions = materialExisted.Unit.UnitConversions.ToList();
                                newUnitConversion.MaterialId = materialId;
                            }
                            else
                            {
                                newUnitConversion.MaterialId = materialId;
                            }

                            newUnitConversions.Add(newUnitConversion);
                        }
                    }

                    if (!isAddNewUnitConversion)
                    {
                        if (isErrorRow)
                        {
                            isDataImportFail = false;
                            error.Row = rowIndex;
                            error.DetailErrors = errorDetails;
                            errors.Add(error);
                        }
                        else
                        {
                            if (!isUpdateMaterial)
                            {
                                newMaterial.Name = cellValueName;
                                newMaterial.Description = cellValueDescription;
                                newMaterial.MaterialCategoryId = materialCategoryExisted?.Id;
                                newMaterial.Sku = cellValueSku;
                                newMaterial.MinQuantity = string.IsNullOrEmpty(cellValueMinQuanity) ? 0 : int.Parse(cellValueMinQuanity);
                                newMaterial.UnitId = baseUnitExisted.Id;

                                newMaterial.StoreId = storeId;
                                newMaterial.Quantity = 0;
                                newMaterial.CostPerUnit = 0;
                                newMaterial.HasExpiryDate = false;
                                newMaterial.CreatedUser = accountId;
                                newMaterial.IsActive = true;

                                newMaterialList.Add(newMaterial);
                            }
                            else
                            {
                                materialExisted.StoreId = storeId;
                                materialExisted.HasExpiryDate = materialExisted.HasExpiryDate;
                                materialExisted.IsActive = true;
                                materialExisted.Name = cellValueName;
                                materialExisted.Description = cellValueDescription;
                                materialExisted.MaterialCategoryId = materialCategoryExisted?.Id;
                                materialExisted.Sku = cellValueSku;
                                materialExisted.MinQuantity = string.IsNullOrEmpty(cellValueMinQuanity) ? 0 : int.Parse(cellValueMinQuanity);
                                materialExisted.UnitId = baseUnitExisted.Id;

                                updateMaterialList.Add(materialExisted);
                            }
                            var materialSuccess = new ImportMaterialResponse.InfoImportModel.MaterialModel()
                            {
                                No = rowIndex - 3,
                                Name = cellValueName,
                                Description = cellValueDescription,
                                Category = cellValueMaterialCategory,
                                Unit = cellValueBaseUnit,
                                SKU = cellValueSku,
                                MinQuanity = cellValueMinQuanity,
                                Branch = string.Join(",", listBranchNames)
                            };
                            listMaterialSuccess.Add(materialSuccess);
                        }
                    }
                    
                    rowIndex += 1;
                }

            } else
            {
                isDataImportFail = false;
            }

            if (isDataImportFail)
            {
                if(updateMaterialList.Count >0)
                {
                    var listUnitConversionRemove = new List<UnitConversion>();
                    foreach (var materialItem in updateMaterialList)
                    {
                        var listUnitConversions = _unitOfWork.UnitConversions.Find(x => x.StoreId == storeId && x.MaterialId == materialItem.Id).ToList();
                        listUnitConversionRemove.AddRange(listUnitConversions);
                    }    

                    if(listUnitConversionRemove.Count>0)
                    {
                        _unitOfWork.UnitConversions.RemoveRange(listUnitConversionRemove);
                    }
                }    
                _unitOfWork.Materials.AddRange(newMaterialList);
                _unitOfWork.Materials.UpdateRange(updateMaterialList);
                _unitOfWork.UnitConversions.AddRange(newUnitConversions);
                
                _unitOfWork.MaterialInventoryBranches.UpdateRange(updateMaterialInventoryBranches);
                _unitOfWork.MaterialInventoryBranches.AddRange(newMaterialInventoryBranches);
                _unitOfWork.UnitConversions.RemoveRange(deleteUnitConversions);
                await _unitOfWork.SaveChangesAsync();
            }

            var InfoImportModel = new ImportMaterialResponse.InfoImportModel()
            {
                NumberRecordImportSuccess = newMaterialList.Count(),
                NumberRecordImport = numberRecordImport,
                IsImportSuccess = isDataImportFail,
                Errors = errors,
                ListMaterialSuccess = listMaterialSuccess,
                IsInValidCategory = errorCategories.Count > 0,
                IsInValidUnit = errorUnits.Count > 0
            };

            if(errorCategories.Count > 0 || errorUnits.Count > 0)
            {
                InfoImportModel.IsImportSuccess = false;
            }

            return InfoImportModel;
        }

        private static bool CheckIsNumber(string data)
        {
            bool isNumber = int.TryParse(data, out int n);
            return isNumber;
        }
    }
}
