using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumVNPayStatus
    {
        //Là trạng thái thanh toán của giao dịch chưa có IPN lưu tại hệ thống của merchant chiều khởi tạo URL thanh toán.
        Pending = 0,
        // Trạng thái thanh toán thành công
        Completed = 1,
        // Trạng thái thanh toán thất bại / lỗi
        Error = 2

    }
}
