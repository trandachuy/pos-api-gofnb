using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Order
{
    public class OrderReportFilterModel
    {
        public List<ServiceTypeDto> ServiceTypes { get; set; }

        public List<PaymentMethodDto> PaymentMethods { get; set; }

        public List<OrderReportFilterCustomerDto> Customers { get; set; }

        public List<OrderStatusDto> OrderStatus { get; set; }

        public class ServiceTypeDto
        {
            public EnumOrderType Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }

        public class PaymentMethodDto
        {
            public EnumPaymentMethod Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }

        public class OrderReportFilterCustomerDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class OrderStatusDto
        {
            public EnumOrderStatus Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }
    }
}
