using GoFoodBeverage.Common.Exceptions.ErrorModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GoFoodBeverage.Common.Exceptions
{
    [Serializable]
    public class ExternalBadRequestException : BadRequestException
    {
        public ExternalBadRequestException() : this("A bad request was made") { }

        public ExternalBadRequestException(string message) : base(message) { }

        public ExternalBadRequestException(List<ErrorItemModel> errors) : base(errors) { }

        public ExternalBadRequestException(ErrorItemModel error) : base(error) { }

        public ExternalBadRequestException(string message, Exception innerException) : base(message, innerException) { }

        protected ExternalBadRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
