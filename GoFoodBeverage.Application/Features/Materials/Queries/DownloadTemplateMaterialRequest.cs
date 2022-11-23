using System;
using MediatR;
using System.IO;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Models.Unit;
using System.Collections.Generic;
using GoFoodBeverage.Models.Store;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Common.Exceptions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GoFoodBeverage.Application.Features.Materials.Queries
{
    public class DownloadTemplateMaterialRequest : IRequest<DownloadTemplateMaterialResponse>
    {
        public string LanguageCode { get; set; }

        public Guid? StoreId { get; set; }
    }

    public class DownloadTemplateMaterialResponse
    {
        public string Result { get; set; }
    }

    public class DownloadTemplateMaterialRequestHandler : IRequestHandler<DownloadTemplateMaterialRequest, DownloadTemplateMaterialResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MapperConfiguration _mapperConfiguration;

        public DownloadTemplateMaterialRequestHandler(
            IUnitOfWork unitOfWork,
            MapperConfiguration mapperConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<DownloadTemplateMaterialResponse> Handle(DownloadTemplateMaterialRequest request, CancellationToken cancellationToken)
        {
            ThrowError.Against(request.StoreId == Guid.Empty || request.StoreId == null, "Cannot find store information");

            var materialCategories = await _unitOfWork.MaterialCategories
                .GetAllMaterialCategoriesInStore(request.StoreId.Value)
                .AsNoTracking()
                .ProjectTo<MaterialCategoryModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var units = await _unitOfWork.Units
                .GetAllUnitsInStore(request.StoreId.Value)
                .OrderByDescending(u => u.Position)
                .AsNoTracking()
                .ProjectTo<UnitModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            var branches = await _unitOfWork.StoreBranches
                .GetStoreBranchesByStoreId(request.StoreId.Value)
                .AsNoTracking()
                .ProjectTo<StoreBranchModel>(_mapperConfiguration)
                .ToListAsync(cancellationToken: cancellationToken);

            string fileName = "Import Material Template";
            var outputFileDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\DocumentTemplates";
            string filePath = Path.Combine(outputFileDirectory, $"{fileName + "_" + request.LanguageCode + ".xlsx"}");

            InsertText(filePath, request.LanguageCode, materialCategories, units, branches);

            var response = new DownloadTemplateMaterialResponse()
            {
                Result = filePath
            };

            return response;
        }

        // Given a document name and text, 
        // inserts a new worksheet and writes the text to cell "A1" of the new worksheet.
        public static void InsertText(string docName, string languageCode, List<MaterialCategoryModel> materialCategories, List<UnitModel> units, List<StoreBranchModel> branches)
        {
            var sheetMaterialCategory = "CATEGORY";
            var sheetUnit = "UNIT";
            var sheetBranch = "BRANCH";
            if(languageCode == "vi")
            {
                sheetMaterialCategory = "DANH MỤC";
                sheetUnit = "ĐƠN VỊ";
                sheetBranch = "CHI NHÁNH";
            }
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

                

                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetMaterialCategory).FirstOrDefault();
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                uint numberRow = 3;
                int indexCategory = 1;
                materialCategories.ForEach(mc =>
                {
                    // Insert the text into the SharedStringTablePart.
                    int index = InsertSharedStringItem(indexCategory.ToString(), shareStringPart);
                    int indexCode = InsertSharedStringItem(mc.Code.ToString(), shareStringPart);
                    int indexName = InsertSharedStringItem(mc.Name.ToString(), shareStringPart);

                    // Insert cell A1 into the new worksheet.
                    Cell cellA = InsertCellInWorksheet("A", numberRow, wsPart);
                    cellA.CellValue = new CellValue(index.ToString());
                    cellA.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellB = InsertCellInWorksheet("B", numberRow, wsPart);
                    cellB.CellValue = new CellValue(indexCode.ToString());
                    cellB.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellC = InsertCellInWorksheet("C", numberRow, wsPart);
                    cellC.CellValue = new CellValue(indexName.ToString());
                    cellC.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    numberRow += 1;
                    indexCategory++;
                });

                Sheet theSheetUnit = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetUnit).FirstOrDefault();
                WorksheetPart wsPartUnit = (WorksheetPart)(wbPart.GetPartById(theSheetUnit.Id));

                uint numberRowUnit = 3;
                int indexUnit = 1;
                units.ForEach(b =>
                {
                    // Insert the text into the SharedStringTablePart.
                    int index = InsertSharedStringItem(indexUnit.ToString(), shareStringPart);
                    int indexCode = InsertSharedStringItem(b.Code.ToString(), shareStringPart);
                    int indexName = InsertSharedStringItem(b.Name.ToString(), shareStringPart);

                    // Insert cell A1 into the new worksheet.
                    Cell cellA = InsertCellInWorksheet("A", numberRowUnit, wsPartUnit);
                    cellA.CellValue = new CellValue(index.ToString());
                    cellA.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellB = InsertCellInWorksheet("B", numberRowUnit, wsPartUnit);
                    cellB.CellValue = new CellValue(indexCode.ToString());
                    cellB.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellC = InsertCellInWorksheet("C", numberRowUnit, wsPartUnit);
                    cellC.CellValue = new CellValue(indexName.ToString());
                    cellC.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    numberRowUnit += 1;
                    indexUnit += 1;
                });



                Sheet theSheetBranch = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetBranch).FirstOrDefault();
                WorksheetPart wsPartBranch = (WorksheetPart)(wbPart.GetPartById(theSheetBranch.Id));

                uint numberRowBranch = 3;
                int indexBranch = 1;
                branches.ForEach(b =>
                {
                    // Insert the text into the SharedStringTablePart.
                    int index = InsertSharedStringItem(indexBranch.ToString(), shareStringPart);
                    int indexCode = InsertSharedStringItem(b.Code.ToString(), shareStringPart);
                    int indexName = InsertSharedStringItem(b.Name.ToString(), shareStringPart);

                    // Insert cell A1 into the new worksheet.
                    Cell cellA = InsertCellInWorksheet("A", numberRowBranch, wsPartBranch);
                    cellA.CellValue = new CellValue(index.ToString());
                    cellA.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellB = InsertCellInWorksheet("B", numberRowBranch, wsPartBranch);
                    cellB.CellValue = new CellValue(indexCode.ToString());
                    cellB.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    Cell cellC = InsertCellInWorksheet("C", numberRowBranch, wsPartBranch);
                    cellC.CellValue = new CellValue(indexName.ToString());
                    cellC.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                    numberRowBranch += 1;
                    indexBranch += 1;
                });

                // Save the new worksheet.
                wsPart.Worksheet.Save();
            }
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
