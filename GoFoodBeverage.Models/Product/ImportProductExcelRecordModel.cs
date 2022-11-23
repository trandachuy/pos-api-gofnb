using ExcelMapper;
using GoFoodBeverage.Common.Attributes;
using GoFoodBeverage.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoFoodBeverage.Models.Product
{
    /// <summary>
    /// Import product model for product sheet
    /// </summary>
    public class ImportProductExcelRecordModel
    {
        [ExcelColumn("A")]
        [ExcelColumnIndex(0)]
        public string ProductName { get; set; }

        [ExcelColumn("B")]
        [ExcelColumnIndex(1)]
        public string Description { get; set; }

        [ExcelColumn("C")]
        [ExcelColumnIndex(2)]
        public string IsTopping { get; set; }

        /// <summary>
        /// Product category name
        /// </summary>
        [ExcelColumn("D")]
        [ExcelColumnIndex(3)]
        public string Category { get; set; }

        /// <summary>
        /// Product unit
        /// </summary>
        [ExcelColumn("E")]
        [ExcelColumnIndex(4)]
        public string Unit { get; set; }

        /// <summary>
        /// List option name: O1001, O1002
        /// </summary>
        [ExcelColumn("F")]
        [ExcelColumnIndex(5)]
        public string Option { get; set; }

        /// <summary>
        /// List topping name: T1001, T1004
        /// </summary>
        [ExcelColumn("G")]
        [ExcelColumnIndex(6)]
        public string Topping { get; set; }

        [ExcelColumn("H")]
        [ExcelColumnIndex(7)]
        public string PriceName { get; set; }

        [ExcelColumn("I")]
        [ExcelColumnIndex(8)]
        public string Price { get; set; }

        public decimal PriceValue
        {
            get
            {
                bool isNumber = decimal.TryParse(Price.Replace(",", ""), out decimal price);
                return isNumber ? price : 0;
            }
        }

        /// <summary>
        /// Material name
        /// </summary>
        [ExcelColumn("J")]
        [ExcelColumnIndex(9)]
        public string Material { get; set; }

        [ExcelColumn("K")]
        [ExcelColumnIndex(10)]
        public string Quantity { get; set; }

        public int QuantityValue
        {
            get
            {
                bool isNumber = int.TryParse(Quantity.Replace(",", ""), out int quantity);
                return isNumber ? quantity : 0;
            }
        }
    }

    public static class ImportProductModelExtension
    {
        public static string GetColunmLabel(this Type type, string attributeName)
        {
            var props = type.GetProperties();
            Dictionary<string, string> propertyColumns = new();
            foreach (var prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr is ExcelColumnAttribute excelColumnAttr)
                    {
                        string propName = prop.Name;
                        string excelColumn = excelColumnAttr.ExcelColumn;
                        propertyColumns.Add(propName, excelColumn);
                    }
                }
            }

            var column = propertyColumns.FirstOrDefault(p => p.Key == attributeName);

            return column.Value ?? string.Empty; ;
        }
    }

    public class ImportSubModelDto
    {
        [ExcelColumnIndex(0)]
        public int No { get; set; }

        [ExcelColumnIndex(1)]
        public string Code { get; set; }

        [ExcelColumnIndex(2)]
        public string Name { get; set; }
    }

    public class ProductMaterialDto
    {
        [ExcelColumnIndex(0)]
        public int No { get; set; }

        [ExcelColumnIndex(1)]
        public string Code { get; set; }

        [ExcelColumnIndex(2)]
        public string Name { get; set; }

        [ExcelColumnIndex(3)]
        public string UnitName { get; set; }
    }

    public class ProductUnitDto
    {
        [ExcelColumnIndex(0)]
        public int No { get; set; }

        [ExcelColumnIndex(1)]
        public string Code { get; set; }

        [ExcelColumnIndex(2)]
        public string Name { get; set; }
    }

    public class ImportProductFileConstants
    {
        public const string IMPORT_PRODUCT_FILE_NAME = "Import Product Template";

        public const int PRODUCT_SHEET_INDEX = 0;

        public const int CATEGORY_SHEET_INDEX = 1;

        public const int UNIT_SHEET_INDEX = 2;

        public const int OPTION_SHEET_INDEX = 3;

        public const int TOPPING_SHEET_INDEX = 4;

        public const int MATERIAL_SHEET_INDEX = 5;

        public const int PRODUCT_SHEET_HEADING_INDEX = 2;

        public const int SUB_SHEET_HEADING_POSITION = 2;
    }

    public class ImportProductMessage
    {
        private string _langCode = LanguageCodeConstants.EN;

        public ImportProductMessage()
        {
        }

        public ImportProductMessage(string langCode)
        {
            _langCode = langCode?.ToLower();
        }

        public string InvalidFileFormat
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Mẫu nhập từ tệp không hợp lệ, vui lòng kiểm tra lại!",
                    _ => "Invalid file, please check again!",
                };

                return text;
            }
        }

        public string ProductNameEmpty
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Vui lòng nhập tên sản phẩm.",
                    _ => "Please enter product name.",
                };

                return text;
            }
        }

        public string ProductUnitEmpty
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Vui lòng nhập đơn vị.",
                    _ => "Please enter unit.",
                };

                return text;
            }
        }

        public string UnitNotExist
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Đơn vị {0} không tồn tại trong cửa hàng.",
                    _ => "Unit {0} has not exist in store.",
                };

                return text;
            }
        }

        public string ProductPriceEmpty
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Vui lòng nhập giá sản phẩm.",
                    _ => "Please enter product price.",
                };

                return text;
            }
        }

        public string MaterialQuantityEmpty
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Vui lòng nhập số lượng nguyên vật liệu.",
                    _ => "Please enter material quantity.",
                };

                return text;
            }
        }

        public string MaterialEmpty
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Vui lòng chọn nguyên vật liệu.",
                    _ => "Please select material.",
                };

                return text;
            }
        }

        public string InvalidQuantity
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Số lượng phải lớn hơn 0.",
                    _ => "Quantity not empty and larger than 0.",
                };

                return text;
            }
        }

        public string ExistedProduct
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Sản phẩm {0} đã tồn tại trong cửa hàng.",
                    _ => "Product {0} has been existed in store.",
                };

                return text;
            }
        }

        public string InvalidOption
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Không tìm thấy thông tin tùy chọn {0} trong cửa hàng.",
                    _ => "Cannot find option {0} in store.",
                };

                return text;
            }
        }

        public string InvalidTopping
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Không tìm thấy thông tin sản phẩm bán kèm {0} trong cửa hàng",
                    _ => "Cannot find topping {0} in store.",
                };

                return text;
            }
        }

        public string InvalidPriceValue
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Giá phải lớn hơn 0.",
                    _ => "Price not empty and larger than 0.",
                };

                return text;
            }
        }

        public string InvalidMaterial
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Không tìm thấy thông tin nguyên vật liệu {0} trong cửa hàng.",
                    _ => "Cannot find material {0} in store.",
                };

                return text;
            }
        }

        public string DuplicatedMaterial
        {
            get
            {
                string text = (_langCode) switch
                {
                    LanguageCodeConstants.VI => "Nguyên vật liệu {0} đã áp dụng cho sản phẩm này rồi.",
                    _ => "The material {0} has been used in this product.",
                };

                return text;
            }
        }
    }
}
