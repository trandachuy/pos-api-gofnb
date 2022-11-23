using GoFoodBeverage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoFoodBeverage.Services.Customer
{
    public class CustomerSegmentActivityService : ICustomerSegmentActivityService
    {
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerSegmentActivityService(
            IUserProvider userProvider,
            IUnitOfWork unitOfWork
            )
        {
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task ClassificationCustomersByCustomerSegmentAsync()
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var customerSegmentsInStore = await _unitOfWork.CustomerSegments
                .GetAllCustomerSegmentsInStore(loggedUser.StoreId.Value)
                .ToListAsync();
            
            if (customerSegmentsInStore != null && customerSegmentsInStore.Any())
            {
                foreach (var customerSegment in customerSegmentsInStore)
                {
                    var currentCustomerCustomerSegmentData = customerSegment.CustomerCustomerSegments.ToList();

                    var newCustomerCustomerSegments = new List<CustomerCustomerSegment>();

                    var customersData = _unitOfWork.Customers.GetCustomerDataBySegmentInStore(loggedUser.StoreId.Value, customerSegment.Id);

                    #region Handle calculate customers
                    if (customersData != null && customersData.Any())
                    {
                        /// Delete
                        var deleteItems = currentCustomerCustomerSegmentData
                            .Where(x => x.CustomerSegmentId == customerSegment.Id && !customersData.Select(x => x.Id).Contains(x.CustomerId));
                        if (deleteItems.Any())
                        {
                            _unitOfWork.CustomerCustomerSegments.RemoveRange(deleteItems);
                        }

                        /// Add new
                        foreach (var customer in customersData)
                        {
                            var customerCustomerSegment = currentCustomerCustomerSegmentData
                                .FirstOrDefault(x => x.CustomerId == customer.Id);

                            if (customerCustomerSegment == null)
                            {
                                var newCustomer = new CustomerCustomerSegment()
                                {
                                    CustomerSegmentId = customerSegment.Id,
                                    CustomerId = customer.Id,
                                    StoreId = loggedUser.StoreId.Value,
                                };
                                newCustomerCustomerSegments.Add(newCustomer);
                            }
                            _unitOfWork.CustomerCustomerSegments.AddRange(newCustomerCustomerSegments);
                        }
                    }
                    else
                    {
                        _unitOfWork.CustomerCustomerSegments.RemoveRange(currentCustomerCustomerSegmentData);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    #endregion
                }
            }
        }
    }
}
