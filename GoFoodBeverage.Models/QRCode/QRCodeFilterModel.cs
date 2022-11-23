using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.QRCode
{
    public class QRCodeFilterModel
    {
        public List<BranchDto> Branches { get; set; }

        public List<ServiceTypeDto> ServiceTypes { get; set; }

        public List<TargetDto> Targets { get; set; }

        public List<StatusDto> Status { get; set; }

        public class BranchDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class ServiceTypeDto
        {
            public EnumOrderType Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }

        public class TargetDto
        {
            public EnumTargetQRCode Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }

        public class StatusDto
        {
            public EnumQRCodeStatus Id { get; set; }

            public string Name { get { return Id.GetName(); } }
        }
    }
}
