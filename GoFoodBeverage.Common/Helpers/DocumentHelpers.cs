using ClosedXML.Excel;
using ExcelMapper;
using GoFoodBeverage.Common.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoFoodBeverage.Common.Helpers
{
    public class File
    {
        public string FileName { get; set; }

        public string Path { get; set; }
    }

    public static class DocumentHelpers
    {
        #region Excel Reader

        /// <summary>
        /// Read rows data in the first sheet as default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadRows<T>(this System.IO.Stream stream)
        {
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet(); /// read first sheet as default
            IEnumerable<T> result = sheet.ReadRows<T>().ToList();

            return result;
        }

        /// <summary>
        /// Read rows data in the specific sheet index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetindex"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadRows<T>(this System.IO.Stream stream, int sheetindex)
        {
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet(sheetindex);
            IEnumerable<T> result = sheet.ReadRows<T>().ToList();

            return result;
        }

        /// <summary>
        /// Read rows data in the specific sheet index and start read from heading index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetindex"></param>
        /// <param name="headingIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadRows<T>(this System.IO.Stream stream, int sheetindex, int headingIndex)
        {
            using var importer = new ExcelImporter(stream);
            ExcelSheet sheet = importer.ReadSheet(sheetindex);
            sheet.HeadingIndex = headingIndex;
            IEnumerable<T> result = sheet.ReadRows<T>().ToList();

            return result;
        }

        /// <summary>
        /// Read rows data in the specific sheet index and start read from heading index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetindex"></param>
        /// <param name="headingIndex"></param>
        /// <returns></returns>
        public static IEnumerable<T> ReadRows<T>(this System.IO.Stream stream, int sheetindex, int headingIndex, string errMessage)
        {
            try
            {
                using var importer = new ExcelImporter(stream);
                importer.Configuration.SkipBlankLines = true;

                ExcelSheet sheet = importer.ReadSheet(sheetindex);
                sheet.HeadingIndex = headingIndex;
                IEnumerable<T> result = sheet.ReadRows<T>().ToList();

                return result;
            }
            catch (Exception)
            {
                throw new Exception(errMessage);
            }
        }
        #endregion

        #region Export data to excel file

        /// <summary>
        /// Inserts the IEnumerable data elements to worksheet
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="workbook"></param>
        /// <param name="data"></param>
        /// <param name="startInsertRowPosition"></param>
        /// <param name="sheetIndex"></param>
        public static void InsertDataToExcelFile<T>(this IXLWorkbook workbook, IEnumerable<T> data, int startInsertRowPosition, int sheetIndex)
        {
            var worksheet = workbook.Worksheets.Worksheet(sheetIndex + 1);
            var firstCell = worksheet.Cell(startInsertRowPosition, 1);
            var rowNumber = worksheet.RangeUsed().RowCount(); 
            var columnNumber = worksheet.RangeUsed().ColumnCount();
            var lastCell = worksheet.Cell(rowNumber, columnNumber);
            var range = worksheet.Range(firstCell, lastCell);
            range.Delete(XLShiftDeletedCells.ShiftCellsUp);

            worksheet.Cell(startInsertRowPosition, 1).InsertData(data);

            /// Auto fit width
            worksheet.Columns().AdjustToContents();
        }

        /// <summary>
        /// Protects this instance using the specified password and password hash algorithm.
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetIndex"></param>
        public static void ProtectSheet(this IXLWorkbook workbook, int sheetIndex)
        {
            workbook.Worksheets.Worksheet(sheetIndex + 1).Protect(StringHelpers.GenerateKey());
        }

        /// <summary>
        /// Protects this instance using the specified password and password hash algorithm.
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetIndex"></param>
        public static void ProtectSheet(this IXLWorkbook workbook, int sheetIndex, string password)
        {
            workbook.Worksheets.Worksheet(sheetIndex + 1).Protect(password);
        }

        public static byte[] SaveAsBytes(this IXLWorkbook workbook)
        {
            using MemoryStream memoryStream = new();
            workbook.SaveAs(memoryStream);
            byte[] bytesInStream = memoryStream.ToArray();

            memoryStream.Close();

            return bytesInStream;
        }

        #endregion

        public static Dictionary<string, string> GetPropertyColumns(Type type)
        {
            var props = type.GetProperties();
            Dictionary<string, string> propertyColumns = new();
            foreach (var prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    var excelColumnAttr = attr as ExcelColumnAttribute;
                    if (excelColumnAttr != null)
                    {
                        string propName = prop.Name;
                        string excelColumn = excelColumnAttr.ExcelColumn;
                        propertyColumns.Add(propName, excelColumn);
                    }
                }
            }

            return propertyColumns;
        }

        public static string GetColumnByProperty(this Dictionary<string, string> keyValues, string key)
        {
            var column = keyValues.FirstOrDefault(p => p.Key == key);

            return column.Value ?? string.Empty;
        }
    }
}
