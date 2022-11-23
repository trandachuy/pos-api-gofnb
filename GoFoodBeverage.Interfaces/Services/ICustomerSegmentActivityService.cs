
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface ICustomerSegmentActivityService
    {
        /// <summary>
        /// Classification of Customers By Customer Segment
        /// </summary>
        /// <returns></returns>
        Task ClassificationCustomersByCustomerSegmentAsync();
    }
}
