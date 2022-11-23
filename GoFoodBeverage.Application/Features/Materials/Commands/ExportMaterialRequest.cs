using System;
using MediatR;
using MoreLinq;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Reflection;
using DocumentFormat.OpenXml;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Common.Exceptions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Materials.Commands
{
    public class ExportMaterialRequest : IRequest<ExportMaterialResponse>
    {
        public string KeySearch { get; set; }

        public Guid? UnitId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? MaterialCategoryId { get; set; }

        public bool? IsActive { get; set; }

        public string LanguageCode { get; set; }

        public Guid? StoreId { get; set; }
    }

    public class ExportMaterialResponse
    {
        public string Result { get; set; }
    }

    public class ExportMaterialRequestHandler : IRequestHandler<ExportMaterialRequest, ExportMaterialResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserProvider _userProvider;

        public ExportMaterialRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<ExportMaterialResponse> Handle(ExportMaterialRequest request, CancellationToken cancellationToken)
        {
            ThrowError.Against(request.StoreId == Guid.Empty || request.StoreId == null, "Cannot find store information");

            var materials = _unitOfWork.Materials.GetAllMaterialsInStore(request.StoreId.Value);

            if (materials != null)
            {
                if (request.MaterialCategoryId != null)
                {
                    /// Find materials by material categoryId
                    materials = materials.Where(m => m.MaterialCategoryId == request.MaterialCategoryId);
                }

                if (request.BranchId != null)
                {
                    /// Find materials by branchId
                    var materialIdsInMaterialInventoryBranch = _unitOfWork.MaterialInventoryBranches
                        .Find(m => m.StoreId == request.StoreId && m.BranchId == request.BranchId)
                        .Select(m => m.MaterialId);

                    materials = materials.Where(x => materialIdsInMaterialInventoryBranch.Contains(x.Id));
                }

                if (request.UnitId != null)
                {
                    materials = materials.Where(m => m.UnitId == request.UnitId);
                }

                if (request.IsActive != null)
                {
                    materials = materials.Where(m => m.IsActive == request.IsActive);
                }

                if (!string.IsNullOrEmpty(request.KeySearch))
                {
                    string keySearch = request.KeySearch.Trim().ToLower();
                    materials = materials.Where(g => g.Name.ToLower().Contains(keySearch) || g.Sku.ToLower().Contains(keySearch));
                }
            }

            var allMaterialsInStore = materials
                .Include(m => m.Unit).ThenInclude(m => m.UnitConversions)
                .Include(m => m.MaterialInventoryBranches).ThenInclude(b => b.Branch)
                .Include(m => m.MaterialCategory).ToList();

            var materialListResponse = _mapper.Map<List<ExportMaterialModel>>(allMaterialsInStore);

            var materialIds = materialListResponse.Select(p => p.Id);
            var unitConversions = _unitOfWork.UnitConversions.GetAllUnitConversionsInStore(request.StoreId.Value)
                .Include(uc => uc.Unit).ToList();

            string fileName = "Export Material Template";
            var outputFileDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\DocumentTemplates";
            string filePath = Path.Combine(outputFileDirectory, $"{fileName + "_" + request.LanguageCode + ".xlsx"}");

            InsertText(filePath, materialListResponse, unitConversions, request.LanguageCode);

            var response = new ExportMaterialResponse()
            {
                Result = filePath
            };

            return response;
        }

        // Given a document name and text, 
        // inserts a new worksheet and writes the text to cell "A1" of the new worksheet.
        public static void InsertText(string docName,List<ExportMaterialModel> materials, List<UnitConversion> unitConversions, string languageCode)
        {
            var sheetMaterial = "MATERIAL";
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(docName, true))
            {
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                WorkbookPart wbPart = spreadSheet.WorkbookPart;

                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetMaterial).FirstOrDefault();
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Open template to determine the index of first row
                uint startRowIndex = 4;
                materials.ForEach(mc =>
                {
                    // Insert the text into the SharedStringTablePart.
                    int indexCode = InsertSharedStringItem(mc.Code.ToString(), shareStringPart);
                    int indexName = InsertSharedStringItem(mc.Name?.ToString(), shareStringPart);
                    int indexDescription = InsertSharedStringItem(mc.Description?.ToString(), shareStringPart);
                    int indexMaterialCategory = InsertSharedStringItem(mc.MaterialCategoryName?.ToString(), shareStringPart);
                    int indexActive = InsertSharedStringItem(mc.IsActive.TranslateStatus(languageCode), shareStringPart);
                    int indexBaseUnit = InsertSharedStringItem(mc.UnitName?.ToString(), shareStringPart);

                    //Get list unitConversions by materialId
                    var unitConversion = unitConversions.Where(uc => uc.MaterialId == mc.Id).ToList();
                    int indexImportUnit = InsertSharedStringItem(string.Join(", ", unitConversion?.Select(uc => uc.Unit.Name)), shareStringPart);

                    int indexCostPerUnit = InsertSharedStringItem(mc.CostPerUnit?.ToString(), shareStringPart);

                    int indexSku = InsertSharedStringItem(mc.Sku?.ToString(), shareStringPart);
                    int indexQuantity = InsertSharedStringItem(mc.Quantity?.ToString(), shareStringPart);
                    int indexMinQuantity = InsertSharedStringItem(mc.MinQuantity?.ToString(), shareStringPart);
                    int indexBranches = InsertSharedStringItem(string.Join(", ", mc.MaterialInventoryBranches.Select(b => b.Name)), shareStringPart);

                    _ = CreateCellData("A", startRowIndex, indexCode.ToString(), CellValues.Number, wsPart);
                    _ = CreateCellData("B", startRowIndex, indexName.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("C", startRowIndex, indexDescription.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("D", startRowIndex, indexMaterialCategory.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("G", startRowIndex, indexBaseUnit.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("H", startRowIndex, indexImportUnit.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("I", startRowIndex, indexCostPerUnit.ToString(), CellValues.Number, wsPart);
                    _ = CreateCellData("F", startRowIndex, indexActive.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("J", startRowIndex, indexSku.ToString(), CellValues.SharedString, wsPart);
                    _ = CreateCellData("K", startRowIndex, indexQuantity.ToString(), CellValues.Number, wsPart);
                    _ = CreateCellData("L", startRowIndex, indexMinQuantity.ToString(), CellValues.Number, wsPart);
                    _ = CreateCellData("M", startRowIndex, indexBranches.ToString(), CellValues.SharedString, wsPart);

                    startRowIndex++;
                });

                // Save the new worksheet.
                wsPart.Worksheet.Save();
            }
        }

        private static Cell CreateCellData(string columnName, uint rowIndex, string data, CellValues dataType, WorksheetPart part)
        {
            Cell cell = InsertCellInWorksheet(columnName, rowIndex, part);
            cell.CellValue = new CellValue(data.ToString());
            cell.DataType = new EnumValue<CellValues>(dataType);
            return cell;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

    }
}
